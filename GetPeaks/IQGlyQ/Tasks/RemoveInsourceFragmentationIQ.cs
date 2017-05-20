using System;
using System.Collections.Generic;
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
using DeconTools.Workflows.Backend.Core;
using IQGlyQ.Enumerations;
using IQGlyQ.Processors;
using IQGlyQ.Results;
using IQGlyQ.TargetGenerators;
using PNNLOmics.Algorithms.PeakDetection;
using PNNLOmics.Data;
using XYData = PNNLOmics.Data.XYData;
using IQGlyQ.Objects;
using Peak = DeconTools.Backend.Core.Peak;
using GetPeaks_DLL.Functions;

namespace IQGlyQ.Tasks
{
    public class RemoveInsourceFragmentationIQ : Task
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
        /// Theoretical Isootpe Profiles//JoshTheorFeatureGenerator
        /// </summary>
        protected ITheorFeatureGenerator _theorFeatureGen;

        /// <summary>
        /// fit scores for isotope filteing
        /// </summary>
        protected Task _fitScoreCalc;
        /// <summary>
        /// Monosaccharide Fragments
        /// </summary>
        private List<FragmentIQTarget> Fragments {get; set;} 

        /// <summary>
        /// mass proton to use
        /// </summary>
        private double MassProton {get;set;}

        /// <summary>
        /// output results
        /// </summary>
        public List<FragmentResultsObjectIq> processedResults { get; set; }

        /// <summary>
        /// for coorelating between fragment EIC notches
        /// </summary>
        protected ChromatogramCorrelatorBase _chromatogramCorrelator;

        /// <summary>
        /// general parameter file
        /// </summary>
        private FragmentedTargetedWorkflowParametersIQ _workflowParameters { get; set; }

        /// <summary>
        /// not used
        /// </summary>
        public List<FragmentIQTarget> _futureTargets { get; set; }

        /// <summary>
        /// internal results summary.  Contains good and bad results
        /// </summary>
        private List<FragmentResultsObjectIq> _processedResults { get; set; }

        /// <summary>
        /// external results summary.  Contains good results
        /// </summary>
        public List<ChromPeakQualityData> _resultsToExport { get; set; }

        /// <summary>
        /// external results summary.  Contains bad results
        /// </summary>
        public List<ChromPeakQualityData> _resultsFailed { get; set; }

        /// <summary>
        /// lets get those chromatograms
        /// </summary>
        public Processors.ProcessorChromatogram _lcProcessor { get; set; }

        /// <summary>
        /// lets get those mass spectra
        /// </summary>
        public Processors.ProcessorMassSpectra _msProcessor { get; set; }

        /// <summary>
        /// summary of parents that we will use later
        /// </summary>
        public List<double> _furtureTargets { get; set; }


        //public RemoveInsourceFragmentationIQ(FragmentedTargetedWorkflowParametersIQ parameters, MSGenerator msGenerator, Run run, ProcessorChromatogram lcProcessor, ProcessorMassSpectra msProcessor)
        public RemoveInsourceFragmentationIQ(FragmentedTargetedWorkflowParametersIQ parameters, Run run, ProcessorChromatogram lcProcessor, ProcessorMassSpectra msProcessor)
        
        {
            _workflowParameters = parameters;
            
            //_msGenerator = MSGeneratorFactory.CreateMSGenerator(this.Run.MSFileType);
            //_msGenerator = msGenerator;

            _theorFeatureGen = new JoshTheorFeatureGenerator(parameters.IsotopeProfileType, parameters.IsotopeLowPeakCuttoff);//perhaps simple constructor

            _iterativeTFFParameters = new IterativeTFFParameters();
            _iterativeTFFParameters.ToleranceInPPM = parameters.MSToleranceInPPM;// 7;
            _iterativeTFFParameters.RequiresMonoIsotopicPeak = true;
            _msfeatureFinder = new IterativeTFF(_iterativeTFFParameters);//default

            Fragments = parameters.FragmentsIq;

            MassProton = DeconTools.Backend.Globals.PROTON_MASS;

            processedResults = new List<FragmentResultsObjectIq>();

            //LC stuff
            //_lcProcessor = new ProcessorChromatogram(parameters.LCParameters);
            _lcProcessor = lcProcessor;

            _futureTargets = new List<FragmentIQTarget>();

            _chromatogramCorrelator = new SimplePeakCorrelator(run, _workflowParameters, _workflowParameters.MinRelativeIntensityForChromCorrelator,lcProcessor);

            _processedResults = new List<FragmentResultsObjectIq>();

            _resultsToExport = new List<ChromPeakQualityData>();

            _resultsFailed = new List<ChromPeakQualityData>();

            _furtureTargets = new List<double>();

            //_fitScoreCalc = new IsotopicProfileFitScoreCalculator();
            //_fitScoreCalc = new IsotopicPeakFitScoreCalculator();

            //_msProcessor = new ProcessorMassSpectra(_workflowParameters.MSParameters);
            _msProcessor = msProcessor;

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
        }

        public void CleanData(Run runIn, IqResult iQresultIn, EnumerationParentOrChild whichDirectionToLook)
        {
        //setup
            IqGlyQResult iQresult = (IqGlyQResult)iQresultIn;

            Processors.ProcessorChromatogram LcProcessor = _lcProcessor;
            Processors.ProcessorMassSpectra MsProcessor = _msProcessor;

            double correlationscorecuttoff = _workflowParameters.CorrelationScoreCuttoff;
            
            //Run run = resultList.Run;
            Run run = runIn;

            List<int> PrimaryScanStorage = run.PrimaryLcScanNumbers.ToList();

            //MSGenerator.GenerateMS
            _msfeatureFinder = new IterativeTFF(_iterativeTFFParameters);//update with current parameters

            //TargetedResultBase result = resultList.GetTargetedResult(resultList.Run.CurrentMassTag);
            //IqTarget result = iQresult.Target;
            
            _resultsToExport = new List<ChromPeakQualityData>();
            _resultsFailed = new List<ChromPeakQualityData>();
            _furtureTargets = new List<double>();
            _processedResults = new List<FragmentResultsObjectIq>();

            //ScanObject scans = new ScanObject(0, 0);
            int scansToSum = _workflowParameters.NumMSScansToSum;//perhapps 9 to decrease noise.  summing is required for orbitrap data
            int maxLCScan = run.MaxLCScan;
            int minLcScan = run.MinLCScan;
            int scanBuffer =  2 * _workflowParameters.LCParameters.ParametersSavitskyGolay.PointsForSmoothing;

            ScanObject baseScanObject = new ScanObject(0, 0, minLcScan, maxLCScan, scanBuffer, scansToSum);
            
            double fitScoreCuttoff = _workflowParameters.FitScoreCuttoff;

            //the idea is that for each posibility, check each fragment at each charge state.  
            //if a fragment at any charge states is found, we have insource.  this is tested by all charge states returning null
            //TODO, we need to check chromatogram allignment when we to find one larger.  if they don't allign, iso=null
            //multiple results could be returned (nonFragmentedHits) and stored in 

            //dynamic ratio filter.  remove low lying species
            bool dynamicIntensityFiltering = true;
            double dynamicRange = 100;//100
            if (dynamicIntensityFiltering && iQresult.IqResultDetail.ChromPeakQualityData.Count>0)
            {
                List<ChromPeakQualityData> orderedList = iQresult.IqResultDetail.ChromPeakQualityData.OrderByDescending(r => r.Peak.Height).ToList();

                double maxHeight = orderedList[0].Peak.Height;

                iQresult.IqResultDetail.ChromPeakQualityData = new List<ChromPeakQualityData>();
                foreach (var chromPeakQualityData in orderedList)
                {
                    double normalizedHeight = chromPeakQualityData.Peak.Height/maxHeight*dynamicRange;
                    if (normalizedHeight > 1)
                    {
                        iQresult.IqResultDetail.ChromPeakQualityData.Add(chromPeakQualityData);
                    }
                }

                iQresult.IqResultDetail.ChromPeakQualityData = iQresult.IqResultDetail.ChromPeakQualityData.OrderBy(r => r.Peak.XValue).ToList();
            }

            Console.WriteLine( Environment.NewLine + "With DynamicRangeFilter: " + dynamicRange);
            PrintChromData(iQresult.IqResultDetail.ChromPeakQualityData);
            

            int maxLCLCPeakCount = 15;//35
            if (iQresult.IqResultDetail.ChromPeakQualityData.Count>maxLCLCPeakCount)
            {
                Console.WriteLine(Environment.NewLine + "Too many LC peaks: " + iQresult.IqResultDetail.ChromPeakQualityData.Count + " peaks need to be filtered by abundance to top " + maxLCLCPeakCount);

                List<ChromPeakQualityData> orderedList = iQresult.IqResultDetail.ChromPeakQualityData.OrderByDescending(r => r.Peak.Height).ToList();

                iQresult.IqResultDetail.ChromPeakQualityData =  new List<ChromPeakQualityData>();
                for (int i = 0; i < maxLCLCPeakCount;i++)
                {
                    iQresult.IqResultDetail.ChromPeakQualityData.Add(orderedList[i]);
                }

                iQresult.IqResultDetail.ChromPeakQualityData = iQresult.IqResultDetail.ChromPeakQualityData.OrderBy(r => r.Peak.XValue).ToList();
            }

            Console.WriteLine(Environment.NewLine + "Best: " + maxLCLCPeakCount);
            PrintChromData(iQresult.IqResultDetail.ChromPeakQualityData);


            if (iQresult.IqResultDetail.ChromPeakQualityData != null && iQresult.IqResultDetail.ChromPeakQualityData.Count > 0)
            {
                int rangeStart = 0;//first chromatographic peak
                int rangeStop = iQresult.IqResultDetail.ChromPeakQualityData.Count;//last chromatographic peak
                bool ovverrideranges = false;//3 of 3
                if(ovverrideranges)
                {
                    rangeStart = 4; rangeStop = 5;
                    //rangeStart = 11; rangeStop = 12;
                    //rangeStart = 11; rangeStop = 12;
                }
                for (int i = rangeStart; i < rangeStop ; i++)//normal
                {
                    #region inside
                    run.PrimaryLcScanNumbers = PrimaryScanStorage.ToList();

                    Tuple<string, string> errorlog = new Tuple<string, string>("Start", "Success");

                    Console.WriteLine(Environment.NewLine);
                    ChromPeakQualityData possibleFragmentPeakQuality = iQresult.IqResultDetail.ChromPeakQualityData[i];

                    Console.WriteLine("Working on LC " + possibleFragmentPeakQuality.ScanLc + " at i=" + i);
                    //error trap for when scan data is incomplete
                    if(possibleFragmentPeakQuality.ScanLc==0)
                    {
                        possibleFragmentPeakQuality.ScanLc = Convert.ToInt32(Math.Truncate(possibleFragmentPeakQuality.Peak.XValue));
                    }

                    if(possibleFragmentPeakQuality.ScanLc==1790)//1823
                    {
                        Console.WriteLine("here");
                    }
                    if (possibleFragmentPeakQuality.ScanLc == 2963)//1823
                    {
                        Console.WriteLine("here");
                    }
                    if (i == 7)//1823
                    {
                        Console.WriteLine("here");
                    }

                    FragmentResultsObjectIq processedTargetResult = new FragmentResultsObjectIq();
                    _processedResults.Add(processedTargetResult);
                    processedTargetResult.PeakQualityObject = possibleFragmentPeakQuality;


                    #region 1.  characterize possible fragment that we will compare all candidate parents to

                    //1.  recalculate peak from relatively larger range.  the chrom level peak witdh is based on the largest peak shape possible (largest number of peaks from one side of the peak)
                    //these are the encompasing bounds that need to be pruned back via peak detector
                    int tempPeakQualityScanStart = Convert.ToInt32(possibleFragmentPeakQuality.ScanLc - possibleFragmentPeakQuality.Peak.Width / 2);//a nice wide range
                    int tempPeakQualityScanStop = Convert.ToInt32(possibleFragmentPeakQuality.ScanLc + possibleFragmentPeakQuality.Peak.Width / 2);

                    ScanObject peakQualityScanBounds = new ScanObject(tempPeakQualityScanStart, tempPeakQualityScanStop, baseScanObject);
                    FragmentedTargetedWorkflowParametersIQ fragmentedTargetedWorkflowParametersIq = _workflowParameters;
                    ProcessorChromatogram processorChromatogram = _lcProcessor;
                    //this should reproduce the one performed in the workflow prior but using the buffered chromatogram generator
                    Utiliites.RenewChromPeakStartStop(ref runIn, fragmentedTargetedWorkflowParametersIq, ref processorChromatogram,processedTargetResult,ref iQresult, ref peakQualityScanBounds, ref errorlog);

                    //set up fragment here
                    FragmentIQTarget possibleFragmentTarget = new FragmentIQTarget(iQresult.Target);

                    string printString;
                    if(possibleFragmentPeakQuality.IsotopicProfile!=null) printString = "Base NO Parent Fragment with peak quality Charge" + possibleFragmentPeakQuality.IsotopicProfile.ChargeState;
                    else printString = "Base NO Parent Fragment with peak quality Charge ?";

                    //set up result fragment here
                    FragmentResultsObjectHolderIq noParentResult = new FragmentResultsObjectHolderIq(possibleFragmentTarget);//this is the base result for each parent 
                    noParentResult.TypeOfResultParentOrChildDifferenceApproach = whichDirectionToLook;//looking for children or parents from target
                    noParentResult.ChromPeakQualityIndex = i;
                    possibleFragmentTarget.ScanLCTarget = possibleFragmentPeakQuality.ScanLc;
                    noParentResult.ScanBoundsInfo = new ScanObject(peakQualityScanBounds);
                    noParentResult.TypeOfResultTargetOrModifiedTarget = TypeOfResult.Target;
                    noParentResult.ElutionTime = possibleFragmentPeakQuality.Peak.NETValue;
                    //noParentResult.FragmentFitAbundance = integratedArea
                    ////thuis is where we process the targetin (6-5-2013) noParentResults.FragmentObservedProfile gets isotope fit score
                    //List<XYData> fragmentEicFitXyData;        //this is key XYdata for fitting
                    //ProcessedPeak fragmentCandiateFitPeak = ProcessTarget.Process(possibleFragmentTarget, ref noParentResult, runIn, iQresult, ref _msGenerator, ref _msfeatureFinder, ref _fitScoreCalc, ref errorlog, printString, out fragmentEicFitXyData, fitScoreCuttoff, _workflowParameters.MSToleranceInPPM, ref LcProcessor, _workflowParameters);

                    //this is where we process the targetin (6-5-2013) noParentResults.FragmentObservedProfile gets isotope fit score
                    //this is where we process the peak area fragmentCandiateFitPeak.Feature.Base.Abundance
                    //this is where we process the peak height fragmentCandiateFitPeak.Base.Abundance
                    //at this point its not a part of noParentResult yet
                    List<XYData> fragmentEicFitXyData;        //this is key XYdata for fitting
                    
                    ProcessedPeak fragmentCandiateFitPeak = ProcessTarget.Process(possibleFragmentTarget, ref noParentResult, runIn, iQresult, ref _msfeatureFinder, ref errorlog, printString, out fragmentEicFitXyData, fitScoreCuttoff, _workflowParameters.MSToleranceInPPM, ref LcProcessor, ref MsProcessor, _workflowParameters);
                    

                    //bool fragmentPassesProcessing = Utiliites.TestProcessedPeak(fragmentCandiateFitPeak, fragmentEicFitXyData, possibleFragmentTarget, noParentResult, fitScoreCuttoff);
                    bool fragmentPassesProcessing = Utiliites.TestProcessedPeak(fragmentCandiateFitPeak, fragmentEicFitXyData, possibleFragmentTarget, noParentResult, fitScoreCuttoff);
                    Console.WriteLine("      Did Fragment Passed Preparation? " + fragmentPassesProcessing + Environment.NewLine);
                    
                    if(fragmentPassesProcessing)//10-28-2013
                    {
                        long integratedArea = fragmentCandiateFitPeak.Feature.Abundance;
                        noParentResult.FragmentFitAbundance = integratedArea;
                        noParentResult.TargetFragment.TheorIsotopicProfile.IntensityAggregateAdjusted = integratedArea;
                        noParentResult.Error = EnumerationError.Success;
                        
                        iQresult.TargetAddOns.Add(noParentResult);//all successfull chrom peaks get added here
                    }

                    //AT THIS POINT ALL Fragment Theoretical Target information should be ready to print

                    #endregion

                    #region 2. Generate list of parent candidates based on differences and charge.  Also remove bad lc peaks using iff

                    //this is the full list we have to search through.  To be intact, non of these have to survive
                    //List<FragmentIQTarget> passFragments = Fragments;
                    FragmentedTargetedWorkflowParametersIQ passWorkflowParameters = _workflowParameters;

                    //candidates are stored as children
                    
                    FragmentIQTarget chargedParentsFromAllFragmentsToSearchForMass = Utiliites.GenerateListOfCandidateParentsTestIQ(run, iQresult, possibleFragmentPeakQuality, peakQualityScanBounds, whichDirectionToLook, ref passWorkflowParameters, ref _theorFeatureGen, ref errorlog, ref LcProcessor);

                    int parentsFound = 0;
                    if (chargedParentsFromAllFragmentsToSearchForMass != null)
                    {
                        parentsFound = chargedParentsFromAllFragmentsToSearchForMass.GetChildCount();
                    }
                    if (errorlog.Item2 == "Success") Console.WriteLine("+ There are " + parentsFound + " candidates for scan " + possibleFragmentPeakQuality.ScanLc);


                    #endregion

                    #region 3.  filter out candidates that don't have features passing IQ

                    //veryify that all candidates return a feature when targeted.  this removes deisotoping problems
                    List<FragmentIQTarget> passFutureTargets = _futureTargets;

                    //FragmentIQTarget finalChargedParentsFromAllFragmentsToSearchForLc = Utiliites.VerifyByTargetedFeatureFindingTestIQ(ref noParentResult, chargedParentsFromAllFragmentsToSearchForMass, fitScoreCuttoff, run, ref _msGenerator, ref _msfeatureFinder, ref passFutureTargets, ref _fitScoreCalc, ref errorlog);
                    List<IqGlyQResult> finalChargedParentsFromAllFragmentsToSearchForLcResults = Utiliites.VerifyByTargetedFeatureFindingTestIQ(chargedParentsFromAllFragmentsToSearchForMass, fitScoreCuttoff, run, ref _msfeatureFinder, ref passFutureTargets, _workflowParameters.MSParameters.IsoParameters, MsProcessor, ref errorlog);

                    //unpack child targets into a list of results (that contain the targets)
                    List<IqResult> glycanChildren = Utiliites.UnfoldFeatureFinderResults(finalChargedParentsFromAllFragmentsToSearchForLcResults);

                    //FragmentIQTarget finalChargedParentsFromAllFragmentsToSearchForLc = null;
                    //foreach (IqGlyQResult result in finalChargedParentsFromAllFragmentsToSearchForLcResults)
                    //{
                    //    finalChargedParentsFromAllFragmentsToSearchForLc = result.ToChild.TargetFragment;
                    //}
                    

                    

                    //int candidatesRemaining = 0;
                    //if (finalChargedParentsFromAllFragmentsToSearchForLc != null)
                    //{
                    //    candidatesRemaining = finalChargedParentsFromAllFragmentsToSearchForLc.GetChildCount();
                    //}
                    //if (errorlog.Item2 == "Success" && finalChargedParentsFromAllFragmentsToSearchForLc != null) Console.WriteLine("+There are " + candidatesRemaining + " candidates after feature finding " + possibleFragmentPeakQuality.ScanLc);
                    
                    if (errorlog.Item2 == "Success" && glycanChildren.Count > 0) Console.WriteLine("+There are " + glycanChildren.Count + " candidates after feature finding " + possibleFragmentPeakQuality.ScanLc);


                    #endregion

                    //4. check candidates to see if they correlate.  step 1 is isolate peak shapes.  step 2 is fit gaussians to shapes 

                    FragmentResultsObjectHolderIq correlationResult;//this is where all the information needs to be

                    //if (errorlog.Item2 == "Success" && finalChargedParentsFromAllFragmentsToSearchForLc != null && finalChargedParentsFromAllFragmentsToSearchForLc.GetChildCount() > 0 && fragmentPassesProcessing==true) //if the fragment failed, there is no points continuing
                    if (errorlog.Item2 == "Success" && glycanChildren.Count > 0 && fragmentPassesProcessing == true) //if the fragment failed, there is no points continuing
                    {
                        //List<IqTarget> children = finalChargedParentsFromAllFragmentsToSearchForLc.ChildTargets().ToList();
                        //foreach (FragmentIQTarget targetParent in children)
                        //{
                        foreach (IqResult iqResult in glycanChildren)//this iterates across all of Utiliites.GenerateListOfCandidateParentsTestIQ (aka posible "difference targets" that match the target we are dealing with
                        {    
                            //make results here for each parent
                            errorlog = new Tuple<string, string>("NewParent", "Success");
                            
                            #region inside

                            FragmentIQTarget targetParentPrerequesite = (FragmentIQTarget)iqResult.Target;
                            FragmentIQTarget targetParent = new FragmentIQTarget(iqResult.Target);

                            targetParent.DifferenceID = targetParentPrerequesite.DifferenceID;
                            targetParent.DifferenceName = targetParentPrerequesite.DifferenceName;
                            //targetParent.AddTarget(targetParentPrerequesite);

                            FragmentResultsObjectHolderIq yesParentResult = new FragmentResultsObjectHolderIq(noParentResult);//this is the base result for each parent 
                            yesParentResult.ChromPeakQualityIndex = i;
                            //targetParent.ScanLCTarget = possibleFragmentPeakQuality.ScanLc;
                            targetParent.ScanLCTarget = possibleFragmentPeakQuality.ScanLc;
                            yesParentResult.ScanBoundsInfo = new ScanObject(peakQualityScanBounds);
                            yesParentResult.TargetParent = targetParent;
                            yesParentResult.TypeOfResultTargetOrModifiedTarget = TypeOfResult.Modified;
                            yesParentResult.ElutionTime = possibleFragmentPeakQuality.Peak.NETValue;
                            //new isotope profile for parent since it is not populated correctly
                            double deltaMassCalibrationMZ = _workflowParameters.MSParameters.IsoParameters.DeltaMassCalibrationMZ;
                            double deltaMassCalibrationMono = _workflowParameters.MSParameters.IsoParameters.DeltaMassCalibrationMono;
                            bool toMassCalibrate = _workflowParameters.MSParameters.IsoParameters.ToMassCalibrate;
                            var penaltyMode = _workflowParameters.MSParameters.IsoParameters.PenaltyMode;
                            
                            Utiliites.TherortIsotopeicProfileWrapper(ref _theorFeatureGen, targetParent, _workflowParameters.MSParameters.IsoParameters.IsotopeProfileMode, deltaMassCalibrationMZ, deltaMassCalibrationMono, toMassCalibrate, penaltyMode);

                            ////Parent processing/  //parentEicFitXYData this is key XYdata for fitting and subsequent correlations
                            //List<XYData> parentEicFitXYData; 
                            //ProcessedPeak parentCandiateFitPeak = ProcessTarget.Process(targetParent, ref yesParentResult, runIn, iQresult, ref _msGenerator, ref _msfeatureFinder, ref _fitScoreCalc, ref errorlog, printString, out parentEicFitXYData, fitScoreCuttoff, _workflowParameters.MSToleranceInPPM, ref LcProcessor, _workflowParameters);

                            //Parent processing/  //parentEicFitXYData this is key XYdata for fitting and subsequent correlations
                            List<XYData> parentEicFitXYData;
                            ProcessedPeak parentCandiateFitPeak = ProcessTarget.Process(targetParent, ref yesParentResult, runIn, iQresult, ref _msfeatureFinder, ref errorlog, printString, out parentEicFitXYData, fitScoreCuttoff, _workflowParameters.MSToleranceInPPM, ref LcProcessor, ref MsProcessor, _workflowParameters);
                            

                            bool parentPassesProcessing = IQGlyQ.Utiliites.TestProcessedPeak(parentCandiateFitPeak, parentEicFitXYData, targetParent, yesParentResult, fitScoreCuttoff);
                            //bool parentPassesProcessing = IQGlyQ.Utiliites.TestProcessedPeak(parentCandiateFitPeak, parentEicFitXYData, targetParent, yesParentResult, 1);
                            Console.WriteLine("Parent passed processing? " + parentPassesProcessing);

                            if (parentPassesProcessing)//added 10-28-2013
                            {
                                long integratedArea = parentCandiateFitPeak.Feature.Abundance;
                                yesParentResult.ParentFitAbundance = integratedArea;
                                yesParentResult.TargetParent.TheorIsotopicProfile.IntensityAggregateAdjusted = integratedArea;
                            }

                            #region write data for unit test (off)

                            /*
                        Console.WriteLine("possibleFragmentTarget ScanLCTarget:" + possibleFragmentTarget.ScanLCTarget + " Charge: " +possibleFragmentTarget.ChargeState);
                        Console.WriteLine("targetParent ScanLCTarget:" + targetParent.ScanLCTarget + " Charge: " + targetParent.ChargeState);
                        
                        Console.WriteLine("fragmentCandiateFitPeak Height:" + fragmentCandiateFitPeak.Height + " X: " + fragmentCandiateFitPeak.XValue);
                        Console.WriteLine("parentCandiateFitPeak Height:" + parentCandiateFitPeak.Height + " X: " + parentCandiateFitPeak.XValue);

                        Console.WriteLine("List<XYData> fragmentEicFitXYData = new List<XYData>();");
                        foreach (var xyData in fragmentEicFitXYData)
                        {
                            Console.WriteLine("fragmentEicFitXYData.Add(new XYData(" + xyData.X + "," + xyData.Y +"));");
                        }

                        Console.WriteLine("List<XYData> parentEicFitXYData = new List<XYData>();");
                        foreach (var xyData in parentEicFitXYData)
                        {
                            Console.WriteLine("parentEicFitXYData.Add(new XYData(" + xyData.X + "," + xyData.Y + "));");
                        }
                        */

                            #endregion

                            int minNumberOfPointsToOverlap = 3;
                            correlationResult = CorrelateWorkflow.Correlate(possibleFragmentTarget, targetParent, noParentResult, yesParentResult, targetParent.ScanLCTarget, fragmentCandiateFitPeak, parentCandiateFitPeak, fragmentEicFitXyData, parentEicFitXYData, ref _chromatogramCorrelator, correlationscorecuttoff,  minNumberOfPointsToOverlap);
                            Console.WriteLine("Is fragment intact?  " + correlationResult.IsIntact);

                            //this is where all the information needs to be for export (excluding what is set in correlator)

                            #region covered parameters in CorrelateWorkflow.Execute()

                            //dataToReturn.ParentCharge = targetParent.ChargeState;
                            //dataToReturn.FragmentCharge = fragmentTarget.ChargeState;
                            //dataToReturn.Scan = scanLCTarget;
                            //dataToReturn.TargetParent = (FragmentIQTarget) targetParent; 
                            //dataToReturn.TargetFragment = (FragmentIQTarget) fragmentTarget; 
                            //dataToReturn.CorrelationResults = chromeResults;
                            //dataToReturn.CorrelationScore = Convert.ToDouble(chromeResults.CorrelationDataItems[0].CorrelationRSquaredVal);
                            //dataToReturn.FragmentFitAbundance = fragmentCandiateFitPeak.Feature.Abundance;
                            //dataToReturn.IsAntiCorrelated = true;
                            //dataToReturn.IsIntact = true;

                            //convert values to yes
                            yesParentResult.CorrelationScore = correlationResult.CorrelationScore;
                            yesParentResult.IsAntiCorrelated = correlationResult.IsAntiCorrelated;
                            yesParentResult.IsIntact = correlationResult.IsIntact;
                            yesParentResult.CorrelationResults = correlationResult.CorrelationResults;

                            #endregion

                     //       if (parentCandiateFitPeak != null && parentCandiateFitPeak.Feature != null)
                     //       {
                     //           yesParentResult.ParentFitAbundance = parentCandiateFitPeak.Feature.Abundance;
                     //           yesParentResult.TargetParent.TheorIsotopicProfile.IntensityAggregateAdjusted = parentCandiateFitPeak.Feature.Abundance;
                     //       }

                     //       if (fragmentCandiateFitPeak != null && fragmentCandiateFitPeak.Feature != null)
                     //       {
                     //           yesParentResult.FragmentFitAbundance = fragmentCandiateFitPeak.Feature.Abundance;
                     //           yesParentResult.TargetFragment.TheorIsotopicProfile.IntensityAggregateAdjusted = fragmentCandiateFitPeak.Feature.Abundance;
                     //       }

                            //switch decision
                     //       noParentResult.Error = EnumerationError.Success;
                     //       iQresult.TargetAddOns.Add(noParentResult);


                            if (parentPassesProcessing)
                            {
                                if (correlationResult.IsIntact)
                                {
                                    yesParentResult.Error = EnumerationError.Success;
                                    iQresult.TargetAddOns.Add(yesParentResult); //this is an intact fragment

                                }
                                else
                                {
                                    yesParentResult.Error = EnumerationError.Success;
                                    iQresult.FutureTargets.Add(yesParentResult);//this parent was detected indicating the fragment is a fragment   
                                }
                            }

                            iQresult.Error = errorlog.Item2;
                            iQresult.DidThisWork = true;
                            //perhaps we need to summarize iQresult.TargetAddOns in the form of a  iQresult.AddResult(new IqResult(target));

                            #endregion
                        }

                        Console.WriteLine("Done with Parent Searching on success track");
                        Console.WriteLine("Next Lc peak");
                    }
                    else
                    {
                        //we have failed in the workflow somewhere so return an empty result with the error
                        //bool test5 = finalChargedParentsFromAllFragmentsToSearchForLc != null && finalChargedParentsFromAllFragmentsToSearchForLc.GetChildCount() > 0;
                        bool test5 = glycanChildren.Count > 0;
                        //this can mean all candidates failed to pull decent EIC
                        //if (test5) Console.WriteLine("generated " + finalChargedParentsFromAllFragmentsToSearchForLc.GetChildCount() + " children");
                        if (test5) Console.WriteLine("generated " + glycanChildren.Count + " children");

                        noParentResult.CorrelationResults = new ChromCorrelationData();//blank
                        noParentResult.CorrelationScore = -5;
                        noParentResult.Error = EnumerationError.DataNotComplete;
                       
                        //if there is no observed isotope profile, we need to create a place holder
                        if(noParentResult.FragmentObservedIsotopeProfile==null)
                        {
                            noParentResult.FragmentObservedIsotopeProfile = new IsotopicProfile();
                            noParentResult.FragmentObservedIsotopeProfile.Score = -1;
                        }
                    


                        //special case where the fragment failed 1/4
                        if (!fragmentPassesProcessing)
                        {
                            noParentResult.IsAntiCorrelated = false;//default
                            noParentResult.IsIntact = false;//this is the case because no fragment exist
                            noParentResult.Error = EnumerationError.NoFragment;
                            iQresult.TargetAddOns.Add(noParentResult);
                            Console.WriteLine("Fragment Failed" + Environment.NewLine);
                            
                        }

                        //hopefully this updates the reference that was added uptop iQresult.TargetAddOns.Add(noParentResult); aroung line 329
                        //special case where the parent failed 2/4
                        if (fragmentPassesProcessing && glycanChildren.Count == 0)
                        {
                            noParentResult.IsAntiCorrelated = false;//default
                            noParentResult.IsIntact = true;//this is the case because no parents passs the generation step
                            noParentResult.Error = EnumerationError.NoParents;

                        }

                        if(noParentResult.Error == EnumerationError.NoError)
                        {
                            noParentResult.Error = EnumerationError.DataNotComplete;//we don't know what case makes it through
                        }

                        //special case when we are missing the abundances 3/4
             //           if (fragmentCandiateFitPeak != null && fragmentCandiateFitPeak.Feature != null)
              //          {
              //              noParentResult.FragmentFitAbundance = fragmentCandiateFitPeak.Feature.Abundance;//needed?
             //               noParentResult.TargetFragment.TheorIsotopicProfile.IntensityAggregateAdjusted = fragmentCandiateFitPeak.Feature.Abundance;//needed?
             //               
             //           }

                        //double counting 10-28-2013
             //           iQresult.TargetAddOns.Add(noParentResult);
             //           Console.WriteLine("Fragment Failed" + Environment.NewLine);
                        //iQresult.Error = errorlog.Item2;
                        //iQresult.DidThisWork = false;//this will have zero children as a failure

                        //Console.WriteLine("Fragment Failed" + Environment.NewLine);
                        Console.WriteLine("Done with Searching since there are no parents");
                        Console.WriteLine("Next Lc peak");
                        Console.WriteLine("Failed Fragment delt with in RemoveInsourceFragmentation" + i);
                    }

                    #endregion //inside
                }

                Console.WriteLine("All LC peaks looked at...");
            }//main loop through each lc peak detected
            else
            {
                //special case when there are no chrom peaks 4/4
                FragmentIQTarget possibleFragmentTarget = new FragmentIQTarget(iQresult.Target);
                FragmentResultsObjectHolderIq noLcPeaksResult = new FragmentResultsObjectHolderIq(possibleFragmentTarget);
                noLcPeaksResult.Error = EnumerationError.MissingChromData;
                iQresult.TargetAddOns.Add(noLcPeaksResult);
                noLcPeaksResult.TypeOfResultTargetOrModifiedTarget = TypeOfResult.Target;
            }

            Console.WriteLine("Finished with looping through lc peaks " + iQresult.ToChild.TargetFragment.Code + Environment.NewLine);

            //GC.Collect();
            //summarize results
            //perhaps store all goodpeaks in the peak quaility list.  All fragments that don't have a parent are returned aka low corr score
            //Result.ChromPeakQualityList
            //the fragment is acceptible if all charge states have low scores below correlationscorecuttoff

            int intactHits = iQresult.TargetAddOns.Count;
            int futureTargets = iQresult.FutureTargets.Count;

            if(iQresult.TargetAddOns.Count==25)
            {
                Console.WriteLine("We have 25");
            }

            Console.WriteLine("We found " + intactHits + " intact peaks and " + futureTargets + " future targets");

            int printcounter = 0;
            #region  print console

            bool print = false;
            if (print)
            {
                foreach (FragmentResultsObjectHolderIq hit in iQresult.TargetAddOns)
                {
                    if (hit.TargetFragment != null && hit.TargetFragment.TheorIsotopicProfile != null)
                    {
                        Console.WriteLine(printcounter +
                                          " We found scan " + hit.TargetFragment.ScanLCTarget +
                                          " with mono mass " + hit.TargetFragment.TheorIsotopicProfile.MonoIsotopicMass +
                                          " That is intact with a charge of " + hit.FragmentCharge +
                                          " ___%Error: " + hit.ErrorValue);
                    }
                }

                foreach (FragmentResultsObjectHolderIq hit in iQresult.FutureTargets)
                {
                    if (hit.TargetFragment != null && hit.TargetParent != null &&
                        hit.TargetFragment.DifferenceName != null && hit.TargetParent.TheorIsotopicProfile != null)
                    {
                        Console.WriteLine(printcounter +
                                          " We found scan " + hit.TargetParent.ScanLCTarget +
                                          " with mono mass " + hit.TargetFragment.TheorIsotopicProfile.MonoIsotopicMass +
                                          " charge " + hit.FragmentCharge +
                                          " is a fragment with a mass larger by " + hit.TargetFragment.DifferenceName +
                                          " " + hit.TargetParent.TheorIsotopicProfile.MonoIsotopicMass +
                                          " ___%Error: " + hit.ErrorValue);
                    }
                }
            }

            #endregion

            Console.WriteLine("finished with Cleaning Target.  Move on");
            Console.WriteLine(Environment.NewLine);
            run.PrimaryLcScanNumbers = PrimaryScanStorage.ToList();

        }

        private static void PrintChromData(List<ChromPeakQualityData> listOfChromPeaks)
        {
            List<double> heights = new List<double>();
            foreach (var i in listOfChromPeaks)
            {
                heights.Add(i.Peak.Height);
                Console.WriteLine("LC Peaks To Analyze:  Scan " + Math.Truncate(i.Peak.XValue) + " has a height of " + Math.Truncate(i.Peak.Height));
            }
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

        

        //private static ScanSet SetScanSet(TargetedResultBase Result, ChromPeakQualityData possibiity)
        //{
        //    int scan = possibiity.ScanLc;
        //    int scanBefore = FindLowerScan(Result, possibiity);
        //    int scanAfter = FindUpperScan(Result, possibiity);
        //    List<int> indexArray = new List<int>();
        //    indexArray.Add(scanBefore);
        //    indexArray.Add(scan);
        //    indexArray.Add(scanAfter);

        //    ScanSet tempScanset = new ScanSet(possibiity.ScanLc, indexArray);
        //    return tempScanset;
        //}

        //private static int FindLowerScan(TargetedResultBase Result, ChromPeakQualityData possibiity)
        //{
        //    //search for next scan below current scan
        //    bool found = false;
        //    int lowerScan = possibiity.ScanLc - 1;
        //    while (lowerScan > 0 && found == false)
        //    {
        //        if (Result.Run.GetMSLevel(lowerScan) == 1)
        //        {
        //            found = true;
        //            break;
        //        }
        //        else
        //        {
        //            lowerScan--;
        //        }
        //    }

        //    if (lowerScan > 0)
        //    {
        //        return lowerScan;
        //    }
        //    else
        //    {
        //        return possibiity.ScanLc;
        //    }
        //}

        //private static int FindUpperScan(TargetedResultBase Result, ChromPeakQualityData possibiity)
        //{
        //    //search for next scan above current scan
        //    bool found = false;
        //    int upperScan = possibiity.ScanLc + 1;
        //    while (upperScan > 0 && found == false)
        //    {
        //        if (Result.Run.GetMSLevel(upperScan) == 1)
        //        {
        //            found = true;
        //            break;
        //        }
        //        else
        //        {
        //            upperScan++;
        //        }
        //    }

        //    if (upperScan > 0)
        //    {
        //        return upperScan;
        //    }
        //    else
        //    {
        //        return possibiity.ScanLc;
        //    }
        //}

    }
}

