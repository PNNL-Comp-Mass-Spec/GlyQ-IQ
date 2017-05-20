using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend;
using DeconTools.Backend.Core;
using DeconTools.Backend.ProcessingTasks.PeakDetectors;
using DeconTools.Backend.ProcessingTasks.TargetedFeatureFinders;
using DeconTools.Backend.ProcessingTasks.MSGenerators;
using DeconTools.Backend.ProcessingTasks;
using DeconTools.Backend.ProcessingTasks.TheorFeatureGenerator;
using DeconTools.Workflows.Backend.Core;
using DeconTools.Backend.Utilities;
using DeconTools.Backend.ProcessingTasks.ChromatogramProcessing;
using DeconTools.Backend.ProcessingTasks.Smoothers;
using PNNLOmics.Algorithms.PeakDetection;
using PNNLOmics.Data;
using XYData = PNNLOmics.Data.XYData;
using GetPeaks_DLL.Functions;
using Accord.MachineLearning;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Distributions.Univariate;


namespace IQGlyQ
{
    public class RemoveInsourceFragmentation : Task
    {
        
        protected IterativeTFF _msfeatureFinder;

        protected MSGenerator _msGenerator;

        protected IterativeTFFParameters _iterativeTFFParameters;

        protected JoshTheorFeatureGenerator _theorFeatureGen;

        protected SavitzkyGolaySmoother Smoother;

        protected PeakChromatogramGenerator PeakChromGen;

        private List<FragmentTarget> Fragments {get; set;} 

        private double MassProton {get;set;}

        public List<ChromPeakQualityData> nonFragmentedHits { get; set; }

        protected ChromatogramCorrelatorBase _chromatogramCorrelator;

        private FragmentedTargetedWorkflowParameters _workflowParameters { get ; set ; }

        public DeconToolsPeakDetectorV2 MSPeakDetector { get; set; }

        public RemoveInsourceFragmentation(FragmentedTargetedWorkflowParameters parameters, MSGenerator msGenerator)
        {
            _workflowParameters = parameters;
            _iterativeTFFParameters = new IterativeTFFParameters();
            //_msGenerator = MSGeneratorFactory.CreateMSGenerator(this.Run.MSFileType);
            _msGenerator = msGenerator;
            _theorFeatureGen = new JoshTheorFeatureGenerator(parameters.IsotopeProfileType, parameters.IsotopeLowPeakCuttoff);//perhaps simple constructor
            _msfeatureFinder = new IterativeTFF(_iterativeTFFParameters);//default
            Fragments = parameters.Fragments;

            MassProton = DeconTools.Backend.Globals.PROTON_MASS;

            nonFragmentedHits = new List<ChromPeakQualityData>();

            _chromatogramCorrelator = new ChromatogramCorrelator(parameters.ChromSmootherNumPointsInSmooth,
                                                                parameters.ChromGenTolerance, 0.01);


            int SavitzkyGolaySmoothingOrder = 2;//default
            int NumPointsInSmoother = parameters.ChromSmootherNumPointsInSmooth;

            double ChromToleranceInPPM = parameters.ChromGenTolerance;
            double MinimumRelativeIntensityForChromCorr = parameters.MinRelativeIntensityForChromCorrelator;

            PeakChromGen = new PeakChromatogramGenerator(ChromToleranceInPPM, Globals.ChromatogramGeneratorMode.MZ_BASED);

            Smoother = new SavitzkyGolaySmoother(NumPointsInSmoother, SavitzkyGolaySmoothingOrder, false);

            //TODO failed peak detection
            MSPeakDetector = new DeconToolsPeakDetectorV2(_workflowParameters.MSPeakDetectorPeakBR,
                _workflowParameters.MSPeakDetectorSigNoise, DeconTools.Backend.Globals.PeakFitType.QUADRATIC, false);
        }

        /// <summary>
        /// this is where we have several candidates for the feature.  some are peaks and some are isomers.  
        /// we need to go into each of these scans and see if there is a glycan larger.  if there is, remove it from the list and save as new target for export
        /// if we export the new targets we don't have to deal with recursion
        /// </summary>
        /// <param name="resultList"></param>
        public override void Execute(ResultCollection resultList)
        {
            //setup
            double correlationscorecuttoff = 0.75;
            int initialandFinalScanWindow = 20;//bin size for initial and final possibiliies

            List<FragmentResultsObject> ResultSummary = new List<FragmentResultsObject>();
                
            _msfeatureFinder = new IterativeTFF(_iterativeTFFParameters);//update with current parameters

            TargetedResultBase Result = resultList.GetTargetedResult(resultList.Run.CurrentMassTag);


            //the idea is that for each posibility, check each fragment at each charge state.  
            //if a fragment at any charge states is found, we have insource.  this is tested by all charge states returning null
            //TODO, we need to check chromatogram allignment when we to find one larger.  if they don't allign, iso=null
            //multiple results could be returned (nonFragmentedHits) and stored in 
            for (int i=0; i<Result.ChromPeakQualityList.Count;i++)
            {
                ChromPeakQualityData possibiity = Result.ChromPeakQualityList[i];
                IsotopicProfile isotopeProfilePossibleFragment = possibiity.IsotopicProfile;
                

                FragmentResultsObject storage = new FragmentResultsObject();
                ResultSummary.Add(storage);
                storage.PeakQualityObject = possibiity;
                storage.FragmentsConsidered = Fragments;

                #region inside
                //step 1, get XY Data
                Result.Run.CurrentScanSet = SetScanSet(Result, possibiity);

                _msGenerator.GenerateMS(Result.Run);
                int scanAssociatedWithXYData = possibiity.ScanLc;

                //for each difference
                foreach (FragmentTarget fragment in Fragments)
                {
                    double MonoDifference = fragment.MonoIsotopicMass;

                    #region inside

                    //step 2 get theoretical isootpe profile
                    FragmentTarget targetPlusDifference = new FragmentTarget(Result.Target);
                    targetPlusDifference.ScanLCTarget = possibiity.ScanLc;

                    targetPlusDifference.DifferenceName = fragment.DifferenceName;
                    targetPlusDifference.EmpiricalFormula = Result.Target.EmpiricalFormula;//TODO +add new elements
                    string newformula = EmpiricalFormulaUtilities.AddFormula(Result.Target.EmpiricalFormula, fragment.EmpiricalFormula);
                    targetPlusDifference.EmpiricalFormula = newformula;
                    targetPlusDifference.MonoIsotopicMass = Result.Target.MonoIsotopicMass + fragment.MonoIsotopicMass;


                    //look one charge greater and one charge smaller for difference ion.  family should be similiary charged
                    //targetPlusDifference.ChargeStateTargets = new List<int> { 1, 2, 3, 4 };//perhaps look one greater and one smaller than result

                    DeviseChargeStates(possibiity, targetPlusDifference);


                    int chargeStateSuccessCounter = 0;  //we need to turn up null across all charge states
                    //in this loop, we scan across each charge state because the parent may be a different charge state than the fragement
                    foreach (Int16 charge in targetPlusDifference.ChargeStateTargets)
                    {
                        FragmentResultsObjectHolder chargeResult = new FragmentResultsObjectHolder();
                        storage.Results.Add(chargeResult);
                        chargeResult.Charge = charge;
                        chargeResult.FragmentConsidered = fragment;
                        chargeResult.IsIntact = false; //default unless proven otherwise

                        targetPlusDifference.MZ = (isotopeProfilePossibleFragment.MonoIsotopicMass + MonoDifference + charge * MassProton) / charge;
                        targetPlusDifference.ChargeState = charge;

                        _theorFeatureGen.GenerateTheorFeature(targetPlusDifference);

                        IsotopicProfile isotopeProfileParent = targetPlusDifference.IsotopicProfile;

                        IsotopicProfile iso = new IsotopicProfile();
                        Console.WriteLine("Look for parent in PQ Scan " + scanAssociatedWithXYData);
                        iso = _msfeatureFinder.IterativelyFindMSFeature(Result.Run.XYData, isotopeProfileParent);//we need to look over several charge states.  does this bring back any positives like EVE

                        //here we are looking for iso==null.  When iso is null, there is no parent and thus the fragment is the intact for this charge
                        if (iso != null)
                        {
                            #region inside
                            Console.WriteLine("We Found One so this is a candidate for insource fragmentation and should be removed from EIC list");
                            Console.WriteLine("We can varify this by lining up the chromatograms");
                            
                            //////////////////////////////////////////////////////////////////////////////////////////////////
                            //TODO test LC here.  Correlation will not work because of isomers

                            //SimplePeakCorrelator correlator = new SimplePeakCorrelator(resultList.Run, _workflowParameters, _workflowParameters.MinRelativeIntensityForChromCorrelator);
                            //correlator.BasePeakIso = possibiity.IsotopicProfile;
                            //correlator.IsoPeaklist = new List<IsotopicProfile>();
                            //correlator.IsoPeaklist.Add(iso);//simple correlation with largest intensity in candidates

                            //correlator.Execute(resultList);

                            ///////////////////////////////////////////////////////////////////////////////////////////////////
                            //TODO plan B.  sum smoothed chromatograms, peak pick summed data to find bins, count peaks per bins 2= insource, 1=intact

                            int startScan = 0;
                            int stopScan = 0;
                            int indexInChromPeakQualityList = i;

                            startScan = Result.ChromPeakQualityList[0].ScanLc - initialandFinalScanWindow;
                            stopScan = Result.ChromPeakQualityList[Result.ChromPeakQualityList.Count-1].ScanLc + initialandFinalScanWindow;

                            bool checkMinimumOrMaxScanOfSet = startScan > 0 && stopScan < resultList.Run.MaxLCScan;

                            //this is to check if we have a decent scan range for the convoluted isomer pile 
                            if (checkMinimumOrMaxScanOfSet)
                            {

                                ScanSetCollection oldScanset = resultList.Run.ScanSetCollection;
                                int scansToSum = 3;//perhapps 5 to decrease noise.  summing is required for orbitrap data
                                resultList.Run.ScanSetCollection.Create(resultList.Run,startScan, stopScan, scansToSum, 1, false);

                                //_msGenerator.Execute(resultList);

                                //this is the fragment EIC for the range
                                Console.WriteLine("Fragment");
                                CorrelationObject loadPossiblePeakFragment = new CorrelationObject(isotopeProfilePossibleFragment, startScan, stopScan, _workflowParameters.ChromGenTolerance, _workflowParameters.MinRelativeIntensityForChromCorrelator, resultList.Run, ref PeakChromGen, ref Smoother);
                                List<PNNLOmics.Data.XYData> loadFragmentBaseEic = PullBestEIC(loadPossiblePeakFragment);

                                //this is the parent EIC for the range
                                Console.WriteLine("Parent");
                                CorrelationObject loadParentPeak = new CorrelationObject(targetPlusDifference.IsotopicProfileLabelled, startScan, stopScan, _workflowParameters.ChromGenTolerance, _workflowParameters.MinRelativeIntensityForChromCorrelator, resultList.Run, ref PeakChromGen, ref Smoother);
                                List<PNNLOmics.Data.XYData> loadParentBaseEic = PullBestEIC(loadParentPeak);

                                //this is the summed EIC for the range
                                List<PNNLOmics.Data.XYData> summedXYOmics = new List<PNNLOmics.Data.XYData>();
                               
                                double newYValue = 0;

                                for(int j = 0; j<loadPossiblePeakFragment.PeakChromXYData[loadPossiblePeakFragment.IndexMostAbundantPeak].Yvalues.Length;j++)
                                {
                                    newYValue = loadPossiblePeakFragment.PeakChromXYData[loadPossiblePeakFragment.IndexMostAbundantPeak].Yvalues[j] + loadParentPeak.PeakChromXYData[loadPossiblePeakFragment.IndexMostAbundantPeak].Yvalues[j];
                                    PNNLOmics.Data.XYData newPoint = new PNNLOmics.Data.XYData(loadPossiblePeakFragment.PeakChromXYData[loadPossiblePeakFragment.IndexMostAbundantPeak].Xvalues[j], newYValue);
                                    summedXYOmics.Add(newPoint);
                                }

                                Console.WriteLine("Summed");
                                List<XYData> summedLC = new List<XYData>();
                                for (int k = 0; k < summedXYOmics.Count; k++)
                                {
                                    summedLC.Add(summedXYOmics[k]);
                                }

                                //write data
                                Console.WriteLine("LC\t" + "Scan" + "\t" + "fragment" + "\t" + "Parent" + "\t" + "Summed");
                                for (int k = 0; k < summedXYOmics.Count; k++)
                                {
                                    Console.WriteLine("LC\t" + loadFragmentBaseEic[k].X + "\t" + loadFragmentBaseEic[k].Y +"\t" + loadParentBaseEic[k].Y + "\t" + summedXYOmics[k].Y);
                                }

                                //find each peak summed scan ranges
                                bool checkMinimumOrMaxScan = possibiity.ScanLc > initialandFinalScanWindow && possibiity.ScanLc < resultList.Run.MaxLCScan;


                                //peak detect summed EIC
                                SetScanRanges(initialandFinalScanWindow, indexInChromPeakQualityList, possibiity, Result, out startScan, out stopScan);

                                //not used
                                ConvertXYData.OmicsXYDataToRunXYDataRun(ref resultList, summedXYOmics);
                                //not used
                                MSPeakDetector.Execute(resultList);
                                
                                PNNLOmics.Algorithms.PeakDetection.PeakCentroider omicsPeakDetection = new PeakCentroider();
                                List<ProcessedPeak> peaksOmicsSummed = omicsPeakDetection.DiscoverPeaks(summedXYOmics);
                                List<ProcessedPeak> peaksOmicsFromFragment = omicsPeakDetection.DiscoverPeaks(loadFragmentBaseEic);
                                List<ProcessedPeak> peaksOmicsFromParent = omicsPeakDetection.DiscoverPeaks(loadParentBaseEic);

                                //dynamic range filer or peak floating 
                                
                                FilteredPeaksToPreventFalseDiscovery(ref peaksOmicsFromFragment, ref peaksOmicsFromParent);

                                foreach (MSPeak mPeak in resultList.Run.PeakList)
                                {
                                    Console.WriteLine("Peak Decon\t" + mPeak.XValue + "\t" + mPeak.Height);
                                }

                                foreach (ProcessedPeak mPeak in peaksOmicsSummed)
                                {
                                    Console.WriteLine("Peak Omics \t" + mPeak.XValue + "\t" + mPeak.Height);
                                }

                                foreach (ProcessedPeak mPeak in peaksOmicsFromFragment)
                                {
                                    Console.WriteLine("Peak Omics Fragment \t" + mPeak.XValue + "\t" + mPeak.Height);
                                }

                                foreach (ProcessedPeak mPeak in peaksOmicsFromParent)
                                {
                                    Console.WriteLine("Peak Omics Parent \t" + mPeak.XValue + "\t" + mPeak.Height);
                                }

                                //from the summed data, and peak selected data, we now know the cuttoff points for the bins
                                List<int> startScansForBins = new List<int>();
                                List<int> stopScansForBins = new List<int>();

                                double divisorFactor = 2;  //cuts FWHM by this value 2=hwhm.  increasing this makes the allighment more stringent

                                foreach (ProcessedPeak mPeak in peaksOmicsSummed)
                                {
                                    //startScansForBins.Add(Convert.ToInt32(summedXYOmics[mPeak.MinimaOfLowerMassIndex].X));
                                    //stopScansForBins.Add(Convert.ToInt32(summedXYOmics[mPeak.MinimaOfHigherMassIndex].X));

                                    startScansForBins.Add(Convert.ToInt32(mPeak.XValue - (mPeak.XValue - summedXYOmics[mPeak.MinimaOfLowerMassIndex].X) / divisorFactor));
                                    stopScansForBins.Add(Convert.ToInt32((mPeak.XValue + (summedXYOmics[mPeak.MinimaOfHigherMassIndex].X - mPeak.XValue) / divisorFactor)));
                                    Console.WriteLine("Bin range " + Convert.ToInt32(startScansForBins[startScansForBins.Count - 1]) + " to " + Convert.ToInt32(stopScansForBins[startScansForBins.Count - 1]));
                                }

                                //accord failed on real data
                                //select first three points and send to model, then cycle through.  this helps direct the models around the center point
                                //int set = 3;
                                //int peakscount = 2;

                                //Console.WriteLine("start = " + startScansForBins[set] + " stop = " + stopScansForBins[peakscount + set-1] + "for " + peaksOmicsFromFragment[set].XValue + " and " + peaksOmicsFromFragment[set + peakscount-1].XValue);
                                //List<XYData> threePointSalute = (from peak in loadFragmentBaseEic where peak.X >= startScansForBins[set] && peak.X <= stopScansForBins[peakscount+set-1] select peak).ToList();
                                //MixtureModel(threePointSalute, peakscount);

                                MixtureModelFX.MixtureModel(loadFragmentBaseEic, peaksOmicsFromFragment.Count);


                                //finally test each EIC across the bins to see if 1 or 2 peaks are present
                                SimplePeakCorrelator correlator = new SimplePeakCorrelator(resultList.Run, _workflowParameters, _workflowParameters.MinRelativeIntensityForChromCorrelator);           
                                for (int j = 0; j < peaksOmicsSummed.Count; j++)
                                {
                                    List<ProcessedPeak> baseHits = (from peak in peaksOmicsFromFragment where peak.XValue >= startScansForBins[j] && peak.XValue <= stopScansForBins[j] select peak).ToList();
                                    List<ProcessedPeak> parentHits = (from peak in peaksOmicsFromParent where peak.XValue >= startScansForBins[j] && peak.XValue <= stopScansForBins[j] select peak).ToList();


                                    Console.WriteLine("bin " + j + " has " + baseHits.Count + " baseHits and " + parentHits.Count + " parentsHits at scan " + Math.Round(peaksOmicsSummed[j].XValue,0));

                                    //almost finlly
                                    //for those that have 2 peaks within the FWHM, we need an overlap score
                                    //1.  scale up intensity so they are the same abundance
                                    //2.  coorelate OR subtract areas while local minima exist for both peaks
                                    
                                    if (baseHits.Count == 1 && parentHits.Count ==1)
                                    {
                                        #region inside
                                        //find common range
                                        ProcessedPeak fragmentCandiate = baseHits[0];
                                        ProcessedPeak parentCandiate = parentHits[0];

                                        int localStartscan = Math.Max(fragmentCandiate.MinimaOfLowerMassIndex, parentCandiate.MinimaOfLowerMassIndex);
                                        int localStopScan = Math.Min(fragmentCandiate.MinimaOfHigherMassIndex, parentCandiate.MinimaOfHigherMassIndex);

                                        localStartscan = Convert.ToInt32(loadFragmentBaseEic[localStartscan].X);
                                        localStopScan = Convert.ToInt32(loadFragmentBaseEic[localStopScan].X);


                                        if (localStartscan < localStopScan)
                                        {
                                            List<XYData> EICOfRegionFragment = (from peak in loadFragmentBaseEic where peak.X >= localStartscan && peak.X <= localStopScan select peak).ToList();
                                            List<XYData> EICOfRegionParent = (from peak in loadParentBaseEic where peak.X >= localStartscan && peak.X <= localStopScan select peak).ToList();

                                            double maxOfParent = EICOfRegionParent.Max(x => x.Y);
                                            double maxOffragment = EICOfRegionFragment.Max(x => x.Y);

                                            foreach (var xyData in EICOfRegionParent)
                                            {
                                                xyData.Y = xyData.Y * maxOfParent / maxOffragment;
                                            }

                                            for (int k = 0; k < EICOfRegionFragment.Count; k++)
                                            {
                                                string x1 = EICOfRegionFragment[k].X.ToString();
                                                string y1 = EICOfRegionFragment[k].Y.ToString();
                                                string y2 = EICOfRegionParent[k].Y.ToString();
                                                Console.WriteLine("notch\t" + x1 + "\t" + y1 + "\t" + y2);
                                            }

                                            //we need to set up result list with pertinant data

                                            //SimplePeakCorrelator correlator = new SimplePeakCorrelator(resultList.Run, _workflowParameters, _workflowParameters.MinRelativeIntensityForChromCorrelator);
                                            correlator.BasePeakIso = isotopeProfilePossibleFragment;
                                            correlator.IsoPeaklist = new List<IsotopicProfile>();
                                            correlator.IsoPeaklist.Add(isotopeProfileParent);//simple correlation with largest intensity in candidates
                                            
                                            correlator.Execute(resultList);

                                            ChromCorrelationData chromeResults = resultList.CurrentTargetedResult.ChromCorrelationData;

                                            //Does the notch correlate at this charge state
                                            if (chromeResults.RSquaredValsAverage > correlationscorecuttoff)
                                            {
                                                //this means the lc notches correlate and ar indeed insourse related
                                                chargeResult.IsIntact = false;
                                            }
                                            else
                                            {
                                                //we have an intact fragment for this potential parrent
                                                chargeResult.IsIntact = true;
                                            }
                                        
                                        }
                                        else
                                        {
                                            //different distirbutions because the ranges do not match up
                                        }
                                        #endregion
                                    }
                                }



                            }//iso
                            #endregion
                        }
                        else
                        {
                            chargeResult.IsIntact = true;
                            chargeStateSuccessCounter++;
                            Console.WriteLine("Since there is no feature larger (feature + difference[i]), this is intact at this charge state");
                        }
                    }//next charge

                    if (chargeStateSuccessCounter == targetPlusDifference.ChargeStateTargets.Count)
                    {
                        ChromPeakQualityData goodHit = new ChromPeakQualityData(possibiity.Peak);
                        goodHit.Abundance = possibiity.Abundance;
                        goodHit.FitScore = possibiity.FitScore;
                        goodHit.InterferenceScore = possibiity.InterferenceScore;
                        goodHit.IsotopicProfile = possibiity.IsotopicProfile;
                        goodHit.ScanLc = possibiity.ScanLc;

                        nonFragmentedHits.Add(goodHit);

                    }

                    #endregion

                }//next fragment



                #endregion
            }

            //perhaps store all goodpeaks in the peak quaility list
            //Result.ChromPeakQualityList

            Console.WriteLine("We found " + nonFragmentedHits.Count + " non fragmentedPeaks in the EIC and are all isomers");

            if (nonFragmentedHits.Count > 0)
            {
                //somehow we need to settle on one
                ChromPeakQualityData bestIntactFeature = nonFragmentedHits[0];

                Result.ChromPeakSelected = bestIntactFeature.Peak;
                Result.ChromPeakQualityList = nonFragmentedHits;//store all good hits (non fragments) here
            }
            else
            {
                Console.WriteLine("all peaks are insourse fragments, return null");
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

        private static List<PNNLOmics.Data.XYData> PullBestEIC(CorrelationObject loadBasePeak)
        {
            List<PNNLOmics.Data.XYData> loadedLC = new List<XYData>();
            double xValue = 0;
            double yValue = 0;

            for (int k = 0; k < loadBasePeak.PeakChromXYData[loadBasePeak.IndexMostAbundantPeak].Xvalues.Length; k++)
            {
                xValue = loadBasePeak.PeakChromXYData[loadBasePeak.IndexMostAbundantPeak].Xvalues[k];
                yValue = loadBasePeak.PeakChromXYData[loadBasePeak.IndexMostAbundantPeak].Yvalues[k];
                PNNLOmics.Data.XYData peak = new PNNLOmics.Data.XYData(xValue, yValue);
                loadedLC.Add(peak);
                //Console.WriteLine("F," + xValue + "," + yValue);
            }
            return loadedLC;
        }

        private static void SetScanRanges(int initialandFinalScanWindow, int i, ChromPeakQualityData possibiity, TargetedResultBase Result, out int startScan, out int stopScan)
        {
            double scanDifferenceToEarlier = initialandFinalScanWindow;
            double scanDifferenceToLater = initialandFinalScanWindow;
            if (i > 0)
            {
                scanDifferenceToEarlier = (possibiity.ScanLc - Result.ChromPeakQualityList[i - 1].ScanLc)/2.0;
                startScan = possibiity.ScanLc - Convert.ToInt32(Math.Round(scanDifferenceToEarlier, 0));
                scanDifferenceToLater = (possibiity.ScanLc - Result.ChromPeakQualityList[i + 1].ScanLc)/2.0;
                stopScan = possibiity.ScanLc + Convert.ToInt32(Math.Round(scanDifferenceToLater, 0));
            }
            else
            {
                startScan = possibiity.ScanLc - Convert.ToInt32(Math.Round(scanDifferenceToEarlier, 0));
                stopScan = possibiity.ScanLc + Convert.ToInt32(Math.Round(scanDifferenceToLater, 0));

                if (i == 0)
                {
                    scanDifferenceToLater = (Result.ChromPeakQualityList[i + 1].ScanLc - possibiity.ScanLc)/2.0;
                    stopScan = possibiity.ScanLc + Convert.ToInt32(Math.Round(scanDifferenceToLater, 0));
                }
                if (i == Result.ChromPeakQualityList.Count)
                {
                    scanDifferenceToEarlier = (possibiity.ScanLc - Result.ChromPeakQualityList[i - 1].ScanLc)/2.0;
                    startScan = possibiity.ScanLc - Convert.ToInt32(Math.Round(scanDifferenceToEarlier, 0));
                }
            }
        }

        private static void DeviseChargeStates(ChromPeakQualityData possibiity, FragmentTarget targetPlusDifference)
        {
            int lowCharge = possibiity.IsotopicProfile.ChargeState - 1;//look one behind
            if (possibiity.IsotopicProfile.ChargeState > 1)
            {
                lowCharge = possibiity.IsotopicProfile.ChargeState - 1;
            }
            else
            {
                lowCharge = 1;
            }

            for (int i = lowCharge; i <= possibiity.IsotopicProfile.ChargeState + 1; i++)//look one ahead
            {
                targetPlusDifference.ChargeStateTargets.Add(i);
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
