using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DeconTools.Backend;
using DeconTools.Backend.Core;
using DeconTools.Backend.ProcessingTasks.FitScoreCalculators;
using DeconTools.Backend.ProcessingTasks.TargetedFeatureFinders;
using DeconTools.Backend.ProcessingTasks.MSGenerators;
using DeconTools.Backend.ProcessingTasks;
using DeconTools.Backend.ProcessingTasks.TheorFeatureGenerator;
using DeconTools.Backend.ProcessingTasks.ChromatogramProcessing;
using DeconTools.Backend.ProcessingTasks.Smoothers;
using PNNLOmics.Algorithms.PeakDetection;
using PNNLOmics.Data;
using XYData = PNNLOmics.Data.XYData;

namespace IQGlyQ.Tasks
{
    public class RemoveInsourceFragmentationV2 : Task
    {
        /// <summary>
        /// feature finder for finding peaks in a mass spec
        /// </summary>
        protected IterativeTFF _msfeatureFinder;

        /// <summary>
        /// generate an XYData from a raw file
        /// </summary>
        protected MSGenerator _msGenerator;

        /// <summary>
        /// parameters for feature finder
        /// </summary>
        protected IterativeTFFParameters _iterativeTFFParameters;

        /// <summary>
        /// Theoretical Isootpe Profiles
        /// </summary>
        protected JoshTheorFeatureGenerator _theorFeatureGen;

        /// <summary>
        /// smoothing data.  Key variable in peak detection
        /// </summary>
        protected SavitzkyGolaySmoother _smoother;

        /// <summary>
        /// EIC generation from peak data
        /// </summary>
        protected PeakChromatogramGenerator _peakChromGen;

        /// <summary>
        /// fit scores for isotope filteing
        /// </summary>
        //protected MassTagFitScoreCalculator _fitScoreCalc;
        protected IsotopicProfileFitScoreCalculator _fitScoreCalc;
        /// <summary>
        /// Monosaccharide Fragments
        /// </summary>
        private List<FragmentTarget> Fragments {get; set;} 

        private double MassProton {get;set;}

        /// <summary>
        /// output results
        /// </summary>
        public List<FragmentResultsObject> processedResults { get; set; }

        /// <summary>
        /// for coorelating between fragment EIC notches
        /// </summary>
        protected ChromatogramCorrelatorBase _chromatogramCorrelator;

        /// <summary>
        /// general parameter file
        /// </summary>
        private FragmentedTargetedWorkflowParameters _workflowParameters { get ; set ; }


        //public DeconToolsPeakDetectorV2 _mSPeakDetector { get; set; }

        public List<FragmentTarget> _futureTargets { get; set; }

        public int _numPointsInSmoother { get; set; }

        /// <summary>
        /// PNNL omics peak detection
        /// </summary>
        public PeakCentroider _omicsPeakDetection { get; set; }

        /// <summary>
        /// internal results summary.  Contains good and bad results
        /// </summary>
        private List<FragmentResultsObject> _processedResults { get; set; }

        /// <summary>
        /// external results summary.  Contains good results
        /// </summary>
        public List<ChromPeakQualityData> _resultsToExport { get; set; }

        /// <summary>
        /// external results summary.  Contains bad results
        /// </summary>
        public List<ChromPeakQualityData> _resultsFailed { get; set; }

        /// <summary>
        /// summary of parents that we will use later
        /// </summary>
        public List<double> _furtureTargets { get; set; }


        public RemoveInsourceFragmentationV2(FragmentedTargetedWorkflowParameters parameters, MSGenerator msGenerator, Run run)
        {
            _workflowParameters = parameters;
            
            //_msGenerator = MSGeneratorFactory.CreateMSGenerator(this.Run.MSFileType);
            _msGenerator = msGenerator;
            _theorFeatureGen = new JoshTheorFeatureGenerator(parameters.IsotopeProfileType, parameters.IsotopeLowPeakCuttoff);//perhaps simple constructor

            _iterativeTFFParameters = new IterativeTFFParameters();
            _iterativeTFFParameters.ToleranceInPPM = 7;
            _iterativeTFFParameters.RequiresMonoIsotopicPeak = true;
            _msfeatureFinder = new IterativeTFF(_iterativeTFFParameters);//default
            Fragments = parameters.Fragments;

            MassProton = DeconTools.Backend.Globals.PROTON_MASS;

            processedResults = new List<FragmentResultsObject>();

            _chromatogramCorrelator = new ChromatogramCorrelator(parameters.ChromSmootherNumPointsInSmooth,
                                                                parameters.ChromGenTolerance, 0.01);


            int SavitzkyGolaySmoothingOrder = 2;//default
            _numPointsInSmoother = parameters.ChromSmootherNumPointsInSmooth;

            double ChromToleranceInPPM = parameters.ChromGenTolerance;
            double MinimumRelativeIntensityForChromCorr = parameters.MinRelativeIntensityForChromCorrelator;

            _peakChromGen = new PeakChromatogramGenerator(ChromToleranceInPPM, Globals.ChromatogramGeneratorMode.MZ_BASED);

            _smoother = new SavitzkyGolaySmoother(_numPointsInSmoother, SavitzkyGolaySmoothingOrder, false);

            //TODO failed peak detection.  Use omics
            //_mSPeakDetector = new DeconToolsPeakDetectorV2(_workflowParameters.MSPeakDetectorPeakBR,_workflowParameters.MSPeakDetectorSigNoise, DeconTools.Backend.Globals.PeakFitType.QUADRATIC, false);

            _futureTargets = new List<FragmentTarget>();

            _omicsPeakDetection = new PeakCentroider();

            _chromatogramCorrelator = new SimplePeakCorrelator(run, _workflowParameters, _workflowParameters.MinRelativeIntensityForChromCorrelator);

            _processedResults = new List<FragmentResultsObject>();

            _resultsToExport = new List<ChromPeakQualityData>();

            _resultsFailed = new List<ChromPeakQualityData>();

            _furtureTargets = new List<double>();

            _fitScoreCalc = new IsotopicProfileFitScoreCalculator();
            
        }

        /// <summary>
        /// this is where we have several parent candidates for the possible fragment (base).  Some are peaks and some are isomers.  
        /// we need to go into each of these scans and see if there is a parent glycan larger.  if there is, remove it from the list and save as new target for export
        /// if we export the new targets (parents), we don't have to deal with recursion
        /// we need to know when we fail on the parent and when on the fragment
        /// </summary>
        /// <param name="resultList"></param>
        public override void Execute(ResultCollection resultList)
        {
            //setup
            double correlationscorecuttoff = 0.5;

            PeakCentroider omicsPeakDetection = _omicsPeakDetection;
            
            Run run = resultList.Run;

            List<int> PrimaryScanStorage = run.PrimaryLcScanNumbers.ToList();
            

            _msfeatureFinder = new IterativeTFF(_iterativeTFFParameters);//update with current parameters

            TargetedResultBase result = resultList.GetTargetedResult(resultList.Run.CurrentMassTag);
            
            _resultsToExport = new List<ChromPeakQualityData>();
            _resultsFailed = new List<ChromPeakQualityData>();
            _furtureTargets = new List<double>();
            _processedResults = new List<FragmentResultsObject>();

            int scanStart = 0;
            int scanStop = 0;

            int scansToSum = 3; //perhapps 5 to decrease noise.  summing is required for orbitrap data

            //the idea is that for each posibility, check each fragment at each charge state.  
            //if a fragment at any charge states is found, we have insource.  this is tested by all charge states returning null
            //TODO, we need to check chromatogram allignment when we to find one larger.  if they don't allign, iso=null
            //multiple results could be returned (nonFragmentedHits) and stored in 


            for (int i = 0; i < result.ChromPeakQualityList.Count; i++)
            {
                #region inside

                run.PrimaryLcScanNumbers = PrimaryScanStorage.ToList();

                bool isGreenForFragment = false;

                Tuple<string, string> errorlog = new Tuple<string, string>("Start", "Success");
                
                Console.WriteLine(Environment.NewLine);
                ChromPeakQualityData possibleFragmentPeakQuality = result.ChromPeakQualityList[i];


                FragmentResultsObject processedTargetResult = new FragmentResultsObject();
                _processedResults.Add(processedTargetResult);
                processedTargetResult.PeakQualityObject = possibleFragmentPeakQuality;


                #region 1.  characterize possible fragment that we will compare all candidate parents to

                int numberOfScansToBuffer = 2 * _numPointsInSmoother;

                //set up fragment here

                int scanStartPossibleFragment = Convert.ToInt32(possibleFragmentPeakQuality.ScanLc - possibleFragmentPeakQuality.Peak.Width / 2);
                int scanStopPossibleFragment = Convert.ToInt32(possibleFragmentPeakQuality.ScanLc + possibleFragmentPeakQuality.Peak.Width / 2);

                FragmentTarget possibleFragmentTarget = new FragmentTarget(result.Target);
                possibleFragmentTarget.StartScan = scanStartPossibleFragment;
                possibleFragmentTarget.StopScan = scanStopPossibleFragment;
                possibleFragmentTarget.ScanLCTarget = possibleFragmentPeakQuality.ScanLc;
                possibleFragmentTarget.IsotopicProfile = possibleFragmentPeakQuality.IsotopicProfile;

                //FragmentTarget possibleFragmentTarget;
                int startScan;
                int stopScan;
                string printString = "Base";

                List<XYData> fragmentEicFitXYData;        //this is key XYdata for fitting
                //ProcessedPeak fragmentCandiateFitPeak;                             //this is a key export peak from index and has integrated fit abundance stored in feature.abundance
                ProcessedPeak fragmentCandiateFitPeak = Utiliites.ProcessTarget(numberOfScansToBuffer, scansToSum, resultList, _workflowParameters.ChromGenTolerance, _workflowParameters.MinRelativeIntensityForChromCorrelator, ref omicsPeakDetection, ref _msGenerator, ref _smoother, ref _peakChromGen, ref errorlog, printString,
                    possibleFragmentTarget,out fragmentEicFitXYData);

                if (fragmentCandiateFitPeak != null)
                {
                    isGreenForFragment = true;
                }
                #endregion

                #region 2. Generate list of parent candidates based on differences and charge

                //this is the full list we have to search through.  To be intact, non of these have to survive
                List<FragmentTarget> passFragments = Fragments;
                FragmentedTargetedWorkflowParameters passWorkflowParameters = _workflowParameters;
                
                //List<FragmentTarget> chargedParentsToSearchForMass = GenerateListOfCandidateParents.Generate(run, result, possibleFragment, possibleFragmentTarget.StartScan, possibleFragmentTarget.StopScan, ref passFragments, ref passWorkflowParameters, ref _theorFeatureGen, ref _peakChromGen,ref _smoother, ref errorlog);
                List<FragmentTarget> chargedParentsFromAllFragmentsToSearchForMass = Utiliites.GenerateListOfCandidateParentsTest(run, result, possibleFragmentPeakQuality, possibleFragmentTarget.StartScan, possibleFragmentTarget.StopScan, ref passFragments, ref passWorkflowParameters, ref _theorFeatureGen, ref _peakChromGen, ref _smoother, ref errorlog);

                if(errorlog.Item2=="Success") Console.WriteLine("+There are " + chargedParentsFromAllFragmentsToSearchForMass.Count + " candidates for scan " + possibleFragmentPeakQuality.ScanLc);

                #endregion

                #region 3.  filter out candidates that don't have features passing IQ

                //veryify that all candidates return a feature when targeted.  this removes deisotoping problems
                List<FragmentTarget> passFutureTargets = _futureTargets;
                
                //List<FragmentTarget> finalChargedParentsToSearchForLC = VerifyByTargetedFeatureFinding.Verify(chargedParentsToSearchForMass, result, run, ref _msGenerator, ref _msfeatureFinder, ref passFutureTargets, ref errorlog);
                double fitScoreCuttoff = 0.15;
                List<FragmentTarget> finalChargedParentsFromAllFragmentsToSearchForLC = Utiliites.VerifyByTargetedFeatureFindingTest(chargedParentsFromAllFragmentsToSearchForMass, fitScoreCuttoff, result, run, ref _msGenerator, ref _msfeatureFinder, ref passFutureTargets, ref _fitScoreCalc, ref errorlog);

                if (errorlog.Item2 == "Success") Console.WriteLine("+There are " + finalChargedParentsFromAllFragmentsToSearchForLC.Count + " candidates after feature finding " + possibleFragmentPeakQuality.ScanLc);

                #endregion

                #region 4. check candidates to see if they correlate.  step 1 is isolate peak shapes.  step 2 is fit gaussians to shapes 

                //verify all pass via coorlating chromatograms.  If any passes this, it is insource.  If all fail, it is intact

                //check each target agains the default base.  charge states are taken into account in section 1
                if (errorlog.Item2 == "Success" && finalChargedParentsFromAllFragmentsToSearchForLC.Count>0)//if the fragment failed, there is no points continuing
                {
                    foreach (FragmentTarget targetParent in finalChargedParentsFromAllFragmentsToSearchForLC)
                    {
                        //for this charged parent target
                        bool isGreenForParent = true;

                        #region 4a. generate smoothed clipped EIC for Parent.  The clipping removes any edge artifacts.

                        int maxScanInDataset = resultList.Run.MaxLCScan;

                        printString = "Potential Parent";

                        List<XYData> parentEicFitXYData;        //this is key XYdata for fitting
                        //ProcessedPeak parentCandiateFitPeak;                             //this is a key export peak from index and has integrated fit abundance stored in feature.abundance
                        ProcessedPeak parentCandiateFitPeak = Utiliites.ProcessTarget(numberOfScansToBuffer, scansToSum, resultList, _workflowParameters.ChromGenTolerance, _workflowParameters.MinRelativeIntensityForChromCorrelator, ref omicsPeakDetection, ref _msGenerator, ref _smoother, ref _peakChromGen, ref errorlog, printString,
                            targetParent, out parentEicFitXYData);

                        #endregion

                        #region 4c, cut to same size and coorelate.  We need both fragment and parent processed at this point

                        //find common range between both fit distributions
                        //ProcessedPeak fragmentCandiate = defaultBaseTargetEicSinglePeakFitPeaks[0];
                        if (fragmentCandiateFitPeak == null)
                        {
                            Console.WriteLine("Fragment Failed Processing");
                        }

                        if (parentCandiateFitPeak == null)
                        {
                            Console.WriteLine("Parent Failed Processing");
                        }


                        if (fragmentCandiateFitPeak != null && parentCandiateFitPeak != null)
                        {
                            startScan = 0;
                            stopScan = 0;
                            bool checkScanRange = Utiliites.FindCommonStartStopBetweenCurves(fragmentEicFitXYData, fragmentCandiateFitPeak, parentEicFitXYData, parentCandiateFitPeak, ref startScan, ref stopScan, ref errorlog);

                            if (checkScanRange)
                            {
                                #region inside

                                //rewindow to common start and stop

                                //correlate fit data
                                List<XYData> possibleFragmentCorrelationEICWindow = Utiliites.ReWindowDataList(startScan, stopScan, fragmentEicFitXYData, 0);
                                List<XYData> parentCorrelationEICWindow = Utiliites.ReWindowDataList(startScan, stopScan, parentEicFitXYData, 0);

                                double maxOffragment = possibleFragmentCorrelationEICWindow.Max(x => x.Y);
                                double maxOfParent = parentCorrelationEICWindow.Max(x => x.Y);

                                //normalize?
                                //foreach (var xyData in parentCorrelationEIC)
                                //{
                                //    xyData.Y = xyData.Y * maxOffragment / maxOfParent;
                                //}

                                Console.WriteLine("\t m/z \t Fragment \t Parent");
                                for (int k = 0; k < possibleFragmentCorrelationEICWindow.Count; k++)
                                {
                                    string x1 = possibleFragmentCorrelationEICWindow[k].X.ToString(CultureInfo.InvariantCulture);
                                    string y1 = possibleFragmentCorrelationEICWindow[k].Y.ToString(CultureInfo.InvariantCulture);
                                    string y2 = parentCorrelationEICWindow[k].Y.ToString(CultureInfo.InvariantCulture);
                                    Console.WriteLine("Correlate \t" + x1 + "\t" + y1 + "\t" + y2);
                                }

                                //we need to set up result list with pertinant data

                                //SimplePeakCorrelator correlator = new SimplePeakCorrelator(resultList.Run, _workflowParameters, _workflowParameters.MinRelativeIntensityForChromCorrelator);
                                SimplePeakCorrelator correlator = (SimplePeakCorrelator)_chromatogramCorrelator;
                                correlator.BasePeakIso = possibleFragmentTarget.IsotopicProfile; //this is where the possibleFragmentTarget goes
                                correlator.IsoPeaklist = new List<IsotopicProfile>();
                                correlator.IsoPeaklist.Add(targetParent.IsotopicProfile); //simple correlation with largest intensity in candidate Parents

                                ChromCorrelationData chromeResults = correlator.CorrelateDataXY(possibleFragmentCorrelationEICWindow, parentCorrelationEICWindow, startScan, stopScan);
                                //correlator.CorrelateData(resultList.Run, Result, localStartscan, localStopScan);
                                //ChromCorrelationData chromeResults = resultList.CurrentTargetedResult.ChromCorrelationData;

                                FragmentResultsObjectHolder summary = new FragmentResultsObjectHolder();
                                processedTargetResult.Results.Add(summary);

                                summary.Charge = targetParent.ChargeState;
                                summary.FragmentConsidered = targetParent;
                                summary.BaseTarget = possibleFragmentPeakQuality;
                                
                                
                                summary.Charge = targetParent.ChargeState;
                                summary.CorrelationResults = chromeResults;
                                summary.CorrelationScore = Convert.ToDouble(chromeResults.CorrelationDataItems[0].CorrelationRSquaredVal);

                                if (fragmentCandiateFitPeak.Feature != null)
                                {
                                    summary.FitAbundance = fragmentCandiateFitPeak.Feature.Abundance;
                                }


                                summary.ErrorValue2 = errorlog.Item2;

                                if (chromeResults.CorrelationDataItems.Count > 0) //this should be true if the correlation returned data
                                {
                                    //Does the notch correlate at this charge state
                                    if (chromeResults.RSquaredValsAverage > correlationscorecuttoff)
                                    {
                                        if (chromeResults.CorrelationDataItems[0].CorrelationSlope < 0) //this is anticorrelated
                                        {
                                            //anticorrelated means the other is not related
                                            Console.WriteLine("#lc notches anti  correlate " + summary.BaseTarget.ScanLc + "_" + summary.Charge + " " + -1 * chromeResults.RSquaredValsAverage + Environment.NewLine);
                                            summary.IsAntiCorrelated = true;
                                            summary.IsIntact = true;
                                        }
                                        else
                                        {
                                            //this means the lc notches correlate and ar indeed insourse related
                                            //chargeResult.IsIntact = false;
                                            Console.WriteLine("#lc notches correlate " + summary.BaseTarget.ScanLc + "_" + summary.Charge + " " + chromeResults.RSquaredValsAverage + Environment.NewLine);
                                            summary.IsAntiCorrelated = false;
                                            summary.IsIntact = false;
                                        }
                                    }
                                    else
                                    {
                                        //we have an intact fragment for this potential parrent
                                        //chargeResult.IsIntact = true;
                                        Console.WriteLine("#lc notches do not correlate " + summary.BaseTarget.ScanLc + "_" + summary.Charge + " " + chromeResults.RSquaredValsAverage + Environment.NewLine);
                                        summary.IsAntiCorrelated = false;
                                        summary.IsIntact = true;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("#no parent data so it much be intact " + summary.BaseTarget.ScanLc + "_" + summary.Charge + " " + chromeResults.RSquaredValsAverage + Environment.NewLine);
                                    summary.IsAntiCorrelated = false;
                                    summary.IsIntact = true;
                                }

                                #endregion
                            }
                            else
                            {
                                //different distirbutions because the ranges do not match up
                                Console.WriteLine("#Failed Check, correlator failed.  Could be when the Xaxis does not overlap");
                            }
                        }
                        if (errorlog.Item2 != "Success")
                        {
                            Console.WriteLine(Environment.NewLine);
                            FragmentResultsObjectHolder summary = new FragmentResultsObjectHolder();
                            processedTargetResult.Results.Add(summary);
                            summary.Charge = targetParent.ChargeState;
                            summary.FragmentConsidered = targetParent;
                            summary.BaseTarget = possibleFragmentPeakQuality;
                            summary.IsAntiCorrelated = false;
                            summary.IsIntact = true;
                            summary.ErrorValue2 = errorlog.Item2;
                            if (fragmentCandiateFitPeak.Feature != null)
                            {
                                summary.FitAbundance = fragmentCandiateFitPeak.Feature.Abundance;
                            }
                            Console.WriteLine("There was a problem identifying a paired peak so none exists scan " + targetParent.ScanLCTarget + " " + errorlog.Item2);

                        }


                        #endregion

                    }//for each charge
                }
                else
                {
                    //no fragment analyzed
                    FragmentResultsObjectHolder summary = new FragmentResultsObjectHolder();
                    processedTargetResult.Results.Add(summary);
                    summary.ErrorValue2 = "Fragment Failed or No peaks passed the VerifyByTargetedFeatureFindingTest";
                    summary.IsIntact = true;
                    summary.BaseTarget = possibleFragmentPeakQuality;
                    if (fragmentCandiateFitPeak.Feature != null)
                    {
                        summary.FitAbundance = fragmentCandiateFitPeak.Feature.Abundance;
                    }
                    Console.WriteLine("Fragment Failed" + Environment.NewLine);
                }
                #endregion//region4

                #endregion //inside
            }

            //summarize results
            //perhaps store all goodpeaks in the peak quaility list.  All fragments that don't have a parent are returned aka low corr score
            //Result.ChromPeakQualityList
            //the fragment is acceptible if all charge states have low scores below correlationscorecuttoff


            Console.WriteLine("We found " + _processedResults.Count + " in total");

            int printcounter = 0;
            for (int i = 0; i < _processedResults.Count;i++ )
            {
                FragmentResultsObject resultbox = _processedResults[i];
               

                int keepCounter = 0;

                //This the key test for a given LC peak.  Each item (100%) in resultbox.Results needs to come back intact.  
                //Or one IsIntact==False will kill it becasuse we were able to find a pluasible fragment at a larger mass (all fragments conisidered)
                foreach (FragmentResultsObjectHolder chargedResult in resultbox.Results)
                {
                    //here is where the area ends up chargedResult.FitAbundance
                    
                    if (chargedResult.IsIntact)
                    {
                        keepCounter++;

                        Console.WriteLine(printcounter + " We found scan " + chargedResult.BaseTarget.ScanLc + 
                            " with mono mass " + resultbox.PeakQualityObject.IsotopicProfile.MonoIsotopicMass +
                            " That is intact with a charge of " + chargedResult.FragmentConsidered.ChargeState + " ___%Error: " + chargedResult.ErrorValue2);
                    }
                    else
                    {
                        Console.WriteLine(printcounter + " We found scan " + chargedResult.BaseTarget.ScanLc +
                            " with mono mass " + resultbox.PeakQualityObject.IsotopicProfile.MonoIsotopicMass +
                            " charge " + chargedResult.FragmentConsidered.ChargeState +
                            " is a fragment with a mass larger by " + chargedResult.FragmentConsidered.DifferenceName + " " + chargedResult.FragmentConsidered.MonoIsotopicMass + " ___%Error: " + chargedResult.ErrorValue2);
                        resultbox.NewTargets.Add(chargedResult.FragmentConsidered);

                        _furtureTargets.Add(chargedResult.FragmentConsidered.MonoIsotopicMass);
                    }
                }

                printcounter++;

                ChromPeakQualityData newResult = resultbox.PeakQualityObject;
                if (keepCounter == resultbox.Results.Count)
                {
                    string sum = "";
                    foreach (FragmentResultsObjectHolder chargedResult in resultbox.Results)
                    {
                        sum += chargedResult.FragmentConsidered.ScanLCTarget + "_" + chargedResult.Charge + "_" + chargedResult.ErrorValue2 + " ";
                        newResult.Peak.IntegratedAbundance = chargedResult.FitAbundance;
                        newResult.IsotopicProfile.IntensityAggregateAdjusted = chargedResult.FitAbundance;//Where to store our new area??????????
                    }

                    if (resultbox.Results.Count == 0)
                    {
                        sum += "no results returned";
                    }
                    Console.WriteLine(i + " " + newResult.ScanLc + " All charge states return intact. " + sum);

                    

                    _resultsToExport.Add(newResult);
                }
                else
                {
                    Console.WriteLine(i + newResult.ScanLc + " A parent was detected at at least one chargestate so we cannot keep this as intact");
                    _resultsFailed.Add(newResult);
                }
                Console.WriteLine(Environment.NewLine);
            }

            _furtureTargets = _furtureTargets.Distinct().ToList();

            foreach (ChromPeakQualityData exported in _resultsToExport)
            {
               Console.WriteLine("*We selected " + exported.ScanLc + " as intact and return it");
            }

            foreach (ChromPeakQualityData exported in _resultsFailed)
            {
                Console.WriteLine("*We labeled  " + exported.ScanLc + " as a fragment ");
            }

            Console.WriteLine(Environment.NewLine);

            foreach (double exported in _furtureTargets)
            {
                Console.WriteLine("We added  " + exported + " to the future library from ");
            }

            Console.WriteLine(Environment.NewLine);

            result.ChromPeakQualityList = _resultsToExport;

            //store future masses and scan numbers
            result.ChromValues = new DeconTools.Backend.XYData();
            result.ChromValues.Xvalues = new double[_futureTargets.Count];
            result.ChromValues.Yvalues = new double[_futureTargets.Count];
            for (int i=0;i<_futureTargets.Count;i++)
            {
                result.ChromValues.Xvalues[i] = _futureTargets[i].MonoIsotopicMass;
                result.ChromValues.Yvalues[i] = _futureTargets[i].ScanLCTarget;
            }

            run.PrimaryLcScanNumbers = PrimaryScanStorage.ToList();

        }

        private static bool ChangeLight(bool checkThis, Tuple<string, string> errorlog)
        {
            if (errorlog.Item2 != "Success" && checkThis==true)//if it was fine and just failed.  switch light
            {
                checkThis = false;
            }
            return checkThis;
        }

        private static void FilteredPeaksToPreventFalseDiscovery(ref List<ProcessedPeak> peaksOmicsFromFragment, ref List<ProcessedPeak> peaksOmicsFromParent)
        {
            double dynamicRangeCuttoffValue = 0.00; //noise level is at 5% of largest peak

            //double maxHeightFragment = FindMaxHeight(peaksOmicsFromFragment);
            //double maxHeightParent = FindMaxHeight(peaksOmicsFromParent);

            double maxHeightFragment = peaksOmicsFromFragment.Max(x => x.Height);
            double maxHeightParent = peaksOmicsFromParent.Max(x => x.Height);

            peaksOmicsFromFragment = (from peak in peaksOmicsFromFragment where peak.Height >= maxHeightFragment * dynamicRangeCuttoffValue select peak).ToList();
            peaksOmicsFromParent = (from peak in peaksOmicsFromParent where peak.Height >= maxHeightParent * dynamicRangeCuttoffValue select peak).ToList();

            List<ProcessedPeak> tempPeaksOmicsFromFragment = new List<ProcessedPeak>();
            List<ProcessedPeak> tempPeaksOmicsFromParent = new List<ProcessedPeak>();

            double peakAspectRatioCuttoff = 1.15;//a peak must be 15% taller than the local minima around it
            foreach (ProcessedPeak peak in peaksOmicsFromFragment)
            {
                double heightToBackground = peak.Height/ peak.LocalLowestMinimaHeight;
                
                if (heightToBackground < peakAspectRatioCuttoff)
                {
                    Console.WriteLine("~out Ratio Fragment= " + heightToBackground + " at scan " + peak.XValue);
                    
                }
                else
                {
                    Console.WriteLine("Ratio Fragment= " + heightToBackground + " at scan " + peak.XValue);
                    tempPeaksOmicsFromFragment.Add(peak);
                }
                
            }

            foreach (ProcessedPeak peak in peaksOmicsFromParent)
            {
                double heightToBackground = peak.Height / peak.LocalLowestMinimaHeight;
                
                if (heightToBackground < peakAspectRatioCuttoff)
                {
                    Console.WriteLine("~out Ratio Parent= " + heightToBackground + " at scan " + peak.XValue);
                    
                }
                else
                {
                    Console.WriteLine("Ratio Parent= " + heightToBackground + " at scan " + peak.XValue);
                    tempPeaksOmicsFromParent.Add(peak);
                }
                
            }

            peaksOmicsFromFragment = tempPeaksOmicsFromFragment;
            peaksOmicsFromParent = tempPeaksOmicsFromParent;
        }

        private static void SetScanRanges(int initialandFinalScanWindow, int i, int maxScanInDataset, ChromPeakQualityData possibiity, TargetedResultBase Result, out int startScan, out int stopScan, ref Tuple<string, string> errorLog)
        {
            double scanDifferenceToEarlier = initialandFinalScanWindow;
            double scanDifferenceToLater = initialandFinalScanWindow;
            if (i > 0)
            {
                scanDifferenceToEarlier = (possibiity.ScanLc - Result.ChromPeakQualityList[i - 1].ScanLc) / 2.0;
                startScan = possibiity.ScanLc - Convert.ToInt32(Math.Round(scanDifferenceToEarlier, 0));
                scanDifferenceToLater = (possibiity.ScanLc - Result.ChromPeakQualityList[i + 1].ScanLc) / 2.0;
                stopScan = possibiity.ScanLc + Convert.ToInt32(Math.Round(scanDifferenceToLater, 0));
            }
            else
            {
                startScan = possibiity.ScanLc - Convert.ToInt32(Math.Round(scanDifferenceToEarlier, 0));
                stopScan = possibiity.ScanLc + Convert.ToInt32(Math.Round(scanDifferenceToLater, 0));

                if (i == 0)
                {
                    scanDifferenceToLater = (Result.ChromPeakQualityList[i + 1].ScanLc - possibiity.ScanLc) / 2.0;
                    stopScan = possibiity.ScanLc + Convert.ToInt32(Math.Round(scanDifferenceToLater, 0));
                }
                if (i == Result.ChromPeakQualityList.Count)
                {
                    scanDifferenceToEarlier = (possibiity.ScanLc - Result.ChromPeakQualityList[i - 1].ScanLc) / 2.0;
                    startScan = possibiity.ScanLc - Convert.ToInt32(Math.Round(scanDifferenceToEarlier, 0));
                }
            }

            if (startScan < 0)
            {
                errorLog = new Tuple<string, string>("SetScanRange", "Scan less than 0");
            }

            if (stopScan > maxScanInDataset)
            {
                errorLog = new Tuple<string, string>("SetScanRange", "Scan larger than dataset");
            }
        }

        private static ScanSet SetScanSet(TargetedResultBase Result, ChromPeakQualityData possibiity)
        {
            int scan = possibiity.ScanLc;
            int scanBefore = FindLowerScan(Result, possibiity);
            int scanAfter = FindUpperScan(Result, possibiity);
            List<int> indexArray = new List<int>();
            indexArray.Add(scanBefore);
            indexArray.Add(scan);
            indexArray.Add(scanAfter);

            ScanSet tempScanset = new ScanSet(possibiity.ScanLc, indexArray);
            return tempScanset;
        }

        private static int FindLowerScan(TargetedResultBase Result, ChromPeakQualityData possibiity)
        {
            //search for next scan below current scan
            bool found = false;
            int lowerScan = possibiity.ScanLc - 1;
            while (lowerScan > 0 && found == false)
            {
                if (Result.Run.GetMSLevel(lowerScan) == 1)
                {
                    found = true;
                    break;
                }
                else
                {
                    lowerScan--;
                }
            }

            if (lowerScan > 0)
            {
                return lowerScan;
            }
            else
            {
                return possibiity.ScanLc;
            }
        }

        private static int FindUpperScan(TargetedResultBase Result, ChromPeakQualityData possibiity)
        {
            //search for next scan above current scan
            bool found = false;
            int upperScan = possibiity.ScanLc + 1;
            while (upperScan > 0 && found == false)
            {
                if (Result.Run.GetMSLevel(upperScan) == 1)
                {
                    found = true;
                    break;
                }
                else
                {
                    upperScan++;
                }
            }

            if (upperScan > 0)
            {
                return upperScan;
            }
            else
            {
                return possibiity.ScanLc;
            }
        }

    }
}

