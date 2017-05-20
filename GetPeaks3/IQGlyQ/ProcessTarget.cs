using System;
using System.Collections.Generic;
using System.Linq;
using GetPeaksDllLite.DataFIFO;
using GetPeaksDllLite.Functions;
using IQGlyQ.Enumerations;
using IQGlyQ.Functions;
using IQGlyQ.Processors;
using IQGlyQ.Results;
using IQ_X64.Backend.ProcessingTasks.TargetedFeatureFinders;
using IQ_X64.Workflows.Core;
using PNNLOmics.Data;
using IQGlyQ.Objects;
using PNNLOmics.Data.Peaks;
using Run64.Backend.Core;
using PNNLOmics.Data.Constants;

namespace IQGlyQ
{
    public static class ProcessTarget
    {
        /// <summary>
        /// This is the key workflow that takes a target and turns it into a clippled, fit XY chromatogram of one peak
        /// </summary>
        /// <param name="target">INPUT:  The target for analtysis</param>
        /// <param name="myResult">ChildResults for GLYQIQ results</param>
        /// <param name="runIn">current run</param>
        /// <param name="iQresult">current result to append to</param>
        /// <param name="_msfeatureFinder">finds features</param>
        /// <param name="errorlog">Error logging for when the workflow fails</param>
        /// <param name="printString">type of target we are looking at</param>
        /// <param name="targetEicSingleXYDataFit"></param>
        /// <param name="fitScoreCuttoff"></param>
        /// <param name="ppmCuttoff">OUTPUT:  the XYData to send to the correlator</param>
        /// <param name="LcProcessor">All LC processing is done here</param>
        /// <param name="MsProcessor">All MS processing is done here</param>
        /// <param name="workflowParameters">global parameters</param>
        /// <returns></returns>
        public static ProcessedPeak Process(FragmentIQTarget target,
            ref FragmentResultsObjectHolderIq myResult, 
            Run runIn, 
            IqResult iQresult,             
            ref IterativeTFF _msfeatureFinder,
            ref Tuple<string, string> errorlog, string printString,
            out List<XYData> targetEicSingleXYDataFit,
            double fitScoreCuttoff,
            double ppmCuttoff,
            ref Processors.ProcessorChromatogram LcProcessor,
            ref Processors.ProcessorMassSpectra MsProcessor,
            FragmentedTargetedWorkflowParametersIQ workflowParameters)
        {
            //extract chroatograms with 2x sg number (_numPointsInSmoother) more points and then truncate it down to remove edge effects

            List<ProcessedPeak> peaksInEIC = new List<ProcessedPeak>();
            List<PNNLOmics.Data.XYData> targetEicClipped = new List<XYData>();
            List<PNNLOmics.Data.XYData> targetEic = new List<XYData>();

            if (errorlog.Item2 == "Success")
            {
                #region inside for correlation object
                //todo this is not necessarily the best scan set.  just the mid point of the origional window.  it is best to pull the scan set at the apex of the candidate
                

                //calculate centerscan so we can pull the best mass spec possible
                
                double massToExtract = Utiliites.GetMassToExtractFromIsotopeProfile(target.TheorIsotopicProfile, workflowParameters);


                //these are different
                var deconChromatogram = workflowParameters.LCParameters.Engine_PeakChromGenerator.GenerateChromatogram(runIn,massToExtract);
                
                
                //this is what is was and picks the max ion based onthe isotope profile.  old
                //Run64.Backend.Data.XYData deconChromatogram3 = workflowParameters.LCParameters.Engine_PeakChromGenerator.GenerateChromatogram(runIn, target.TheorIsotopicProfile);

                //this is different here and pulls full dataset length xic
                //Run64.Backend.Data.XYData deconChromatogram2 = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(runIn, massToExtract);
                

                //this was good.  keep lc processing the same 1-8-2014
                //List<ProcessedPeak> resultPeaks = LcProcessor.Execute(deconChromatogram, EnumerationChromatogramProcessing.ChromatogramLevel);

                List<ProcessedPeak> resultPeaks = LcProcessor.Execute(deconChromatogram, workflowParameters.LCParameters.ProcessLcChromatogram);
                int start = myResult.ScanBoundsInfo.Start;
                int stop = myResult.ScanBoundsInfo.Stop;
                List<ProcessedPeak> resultPeaksWithinBounds = (from n in resultPeaks where n.XValue > start && n.XValue < stop select n).ToList();
                ProcessedPeak tallestPeak = new ProcessedPeak();
                if (resultPeaksWithinBounds.Count>1)
                {
                    resultPeaksWithinBounds = resultPeaks.OrderByDescending(n => n.Height).ToList();
                    tallestPeak = resultPeaks[0];
                }
                else
                {
                    tallestPeak = resultPeaksWithinBounds.FirstOrDefault();
                }

                if (tallestPeak!= null && tallestPeak.XValue > 0)
                {
                    //select ms from top of lc peak yet still inside the bounds
                    iQresult.LCScanSetSelected = Utiliites.ScanSetFromCenterScan(runIn, Convert.ToInt32(tallestPeak.XValue), myResult.ScanBoundsInfo.ScansToSum);
                }
                else
                {
                    iQresult.LCScanSetSelected = Utiliites.ScanSetFromStartStop(runIn, myResult.ScanBoundsInfo);//old but we can keep it around if no peak was found in the range
                }

                myResult.Scan = iQresult.LCScanSetSelected.PrimaryScanNumber;
                //iQresult.LCScanSetSelected = utilities.GetLCScanSetForChromPeak(iQresult.ChromPeakSelected, runIn, scansToSum);
                //iQresult.LCScanSetSelected = runIn.ScanSetCollection.ScanSetList[0];

                //iQresult.IqResultDetail.MassSpectrum = _msGenerator.GenerateMS(runIn, iQresult.LCScanSetSelected);
                iQresult.IqResultDetail.MassSpectrum = MsProcessor.DeconMSGeneratorWrapper(runIn, iQresult.LCScanSetSelected);

                

                #region IFF workaround.  
                //we need a work around here because IFF is looking for a mono in position 0. when we append a 0 in front, it looks like a mono
                IsotopicProfile isotopicProfileObserved = new IsotopicProfile();

                //isotopicProfileObserved = IterativeFeatureFinderWrapper(target.TheorIsotopicProfile, iQresult.IqResultDetail.MassSpectrum, _msfeatureFinder);
                isotopicProfileObserved = IterativelyFindMSFeatureWrapper.IterativeFeatureFind(target.TheorIsotopicProfile, iQresult.IqResultDetail.MassSpectrum, _msfeatureFinder);


                //finally store data again.  this is key
                //this the singlton where we save the reslts for printing.  thiere is also a failed result below
                iQresult.ObservedIsotopicProfile = isotopicProfileObserved;
                myResult.Primary_Observed_IsotopeProfile = iQresult.ObservedIsotopicProfile;
                

               

                #endregion
                //TODO we need XYData

                if (iQresult.ObservedIsotopicProfile != null)
                {
                    PreProcessForFitScore(ref iQresult, target, fitScoreCuttoff, ppmCuttoff, MsProcessor, workflowParameters.MSParameters.IsoParameters);

                    //iQresult.InterferenceScore = MsProcessor.ExecuteInterference(iQresult.ObservedIsotopicProfile, mspeakList);
                    //myResult.InterfearenceScore = iQresult.InterferenceScore;
                    //I hope
                    if (iQresult.ObservedIsotopicProfile.Score >= fitScoreCuttoff)
                    {
                        //errorlog = new Tuple<string,string>("Process","fitscore");
                        //for the deuterated, we need to check the pure compound for a better fit

                        myResult.Error = EnumerationError.FailedFitScore;
                    }

                    //we need to build in the calibration here???

                    
                    int charge = target.TheorIsotopicProfile.ChargeState;
                    double massProton = Constants.SubAtomicParticles[SubAtomicParticleName.Proton].MassMonoIsotopic;
                    double calibratedTheoreticalMono = ConvertMzToMono.Execute(target.TheorIsotopicProfile.MonoPeakMZ, charge, massProton);

                    //this may be taken into affount in the FF
                    double ppmError = ErrorCalculator.PPMAbsolute(iQresult.ObservedIsotopicProfile.MonoIsotopicMass, calibratedTheoreticalMono);
                    myResult.PPMError = ppmError;
                    if(ppmError>ppmCuttoff)
                    {
                        errorlog = new Tuple<string, string>("ProcessdTarget", "FailedFeatureFinder");
                        myResult.Error = EnumerationError.FailedMass;
                    }
                }
                else
                {
                    //failed feature finder
                    Console.WriteLine("   --We were NOT able to obtain an Observed Isotope Profile at scan: " + Environment.NewLine +"     " + iQresult.LCScanSetSelected);
                    errorlog = new Tuple<string,string>("ProcessdTarget","FailedFeatureFinder");
                    myResult.Error = EnumerationError.FailedFeatureFinder;
                }

                
                //CorrelationObject targetPeak = Utiliites.CreateCorrelationObject(target.IsotopicProfile, startScan, stopScan, chromToleranceInPPM, MinRelativeIntensityForChromCorrelator, runIn, ref _peakChromGen, ref _smoother, ref errorlog);
                EnumerationError errorCode;
                CorrelationObject targetPeak = Utiliites.CreateCorrelationObject(target.TheorIsotopicProfile, myResult.ScanBoundsInfo, runIn, ref errorlog, out errorCode, ref LcProcessor, workflowParameters);

                

                targetEic = Utiliites.PullBestEIC(targetPeak, ref errorlog);
                //targetEicClipped = Utiliites.ReWindowDataListIQ(myResult.ScanBoundsInfo, targetEic, 0, ref errorlog); //clipping is to remove artificial tails
                targetEicClipped = ChangeRange.ClipXyDataToScanRange(targetEic, myResult.ScanBoundsInfo, false); //clipping is to remove artificial tails//this may not be needed
                //Console.WriteLine("Base XYData " + target.ScanLCTarget);
                //Console.WriteLine(printString + " XYData " + target.ScanLCTarget + " at mono " + target.TheorIsotopicProfile.MonoIsotopicMass);
                //Utiliites.WriteXYData(targetEicClipped, ref errorlog);

                if (errorlog.Item2 == "Success") Console.WriteLine(Environment.NewLine + printString + "   initial " + targetEic.Count + " " + printString + " clipped " + targetEicClipped.Count);


                //if we failed the fit score due to penalty effect we need to check for multiple ions
                //1.  identify condition to branch out
                ///if targetPeak.AcceptableChromList[0] == true then there is a peak in front with enough data to make a peak and the peak is within the LC window of interest
                if (targetPeak != null && myResult.Error == EnumerationError.FailedFitScore && targetPeak.AcceptableChromList[0] == true && targetEicClipped.Count > 0)
                {
                    //pull penalty peak since it is not pulled in the correlation Object
                    //var clippedEIC = CreateBufferSmoothClip(myResult.ScanBoundsInfo, runIn, ref LcProcessor, peak, workflowParameters.LCParameters);
                    //var peaksInEIC = _lcProcessor.Execute(clippedEIC, EnumerationChromatogramProcessing.LCPeakDetectOnly);
                    //var peaksInEIC = Utiliites.FilterByPointsPerSide(peaksInEIC, workflowParameters.LCParameters.PointsPerShoulder);
                    
                    //if this is successfull, it will madify the following 
                    //1.  EnumerationError successfulll
                    //2.  iQresult.FitScore = iQresult.ObservedIsotopicProfile.Score;
                    ConfirmPenaltyPeakIsRelatedToMainProfile(target, myResult, runIn, iQresult, fitScoreCuttoff, ppmCuttoff, LcProcessor, MsProcessor, workflowParameters, targetEicClipped, targetPeak);
                }
                
               

                //PeakCentroider engineOmicsPeakDetection = workflowParameters.LCParameters.Engine_OmicsPeakDetection;
                //peaksInEIC = Utiliites.DiscoverPeaks(targetEicClipped, ref engineOmicsPeakDetection, ref errorlog);
                peaksInEIC = LcProcessor.Execute(targetEicClipped, EnumerationChromatogramProcessing.LCPeakDetectOnly);
                

                Utiliites.ConvertPeakXyListToScan(peaksInEIC, ref errorlog);

                if (errorlog.Item2 == "Success")//make sure there are points
                {
                    //make sure we have the coorect peak to fit to the ScanLC target
                    Utiliites.SelectClosestLCPeak(ref peaksInEIC, target.ScanLCTarget, ref errorlog); //just incase there are more than one
                    ScanObject scanBoundsInfo = myResult.ScanBoundsInfo;
                    Utiliites.ConvertProcessedPeakToScanObject(peaksInEIC.First(), targetEicClipped, ref scanBoundsInfo, ref errorlog);
                }

                //we need to convert a PNNL Omics processed peak into a scan object

                

                #endregion
            }

            if (peaksInEIC != null) Console.WriteLine("   --We discovered " + peaksInEIC.Count + " LC peak(s)" + Environment.NewLine + "     in " + printString);

            //Base
            //Console.WriteLine(Environment.NewLine + "Base LM Notch " + possibleFragmentTarget.ScanLCTarget + "_" + targetParent.IsotopicProfile.ChargeState);

            List<ProcessedPeak> targetEicSinglePeakFitPeaks = new List<ProcessedPeak>(); //targetEicSinglePeakFitPeaks[0].Feature.Abundance=fit area
            targetEicSingleXYDataFit = new List<XYData>();

            ProcessedPeak peakFromFitData = null;

            if (errorlog.Item2 == "Success" && peaksInEIC != null && peaksInEIC.Count>0)//output is peakFromFitData
            {
                Console.WriteLine(Environment.NewLine + printString + " LM Notch " + target.ScanLCTarget + "_Generic");

                //Utiliites.WriteXYData(targetEicClipped, ref errorlog);

                //Utiliites.ClipFitAndCalculateArea(targetEicClipped, peaksInEIC[0], out targetEicSingleXYDataFit, out targetEicSinglePeakFitPeaks, ref startScan, ref stopScan, ref omicsPeakDetection, ref errorlog);

                //double[] fitCoefficents;  
                //ScanObject scanRangeFit = new ScanObject(target.ScanInfo.Start,target.ScanInfo.Stop);//this is not right.  we need start and stop from peaksInEIC

                ///gets LM fit score here
                ///the idea here is to populate the targetEicSingleXYDataFit with the integrated area targetEicSinglePeakFitPeaks -->processedPeak-->Feature-->Base-->Abundance
                /// also, at processedPeak-->Heiht will corrspond to the peak height
                double areaForTesting;
                targetEicSinglePeakFitPeaks = Utiliites.ClipFitAndCalculateArea(targetEicClipped, peaksInEIC[0], workflowParameters.LCParameters.LM_RsquaredCuttoff, out targetEicSingleXYDataFit, ref myResult, ref LcProcessor, ref errorlog, out areaForTesting);

                //error trap when the fit is off
                //targetEicSinglePeakFitPeaks = Utiliites.NoFitSoClipAndIterate(targetEicSinglePeakFitPeaks, targetEicClipped, ref target,workflowParameters.LCParameters.LM_RsquaredCuttoff, ref peaksInEIC, ref targetEicSingleXYDataFit, ref myResult, ref LcProcessor, ref errorlog);
                if (targetEicSinglePeakFitPeaks != null && targetEicSinglePeakFitPeaks.Count > 0)
                {
                    double integratedArea = targetEicSinglePeakFitPeaks[0].Feature.Abundance;
                }
                //myResult.ScanBoundsInfo = scanRangeFit;
                //myResult.CorrelationCoefficients = fitCoefficents;

                //Console.WriteLine("print results from inside ProcessTarget" + Environment.NewLine + "X\tY");
                //Utiliites.WriteXYData(targetEicSingleXYDataFit, ref errorlog);

                //find common range between both fit distributions
                bool test1 = Utiliites.SignPostRequire(targetEicSinglePeakFitPeaks != null && targetEicSinglePeakFitPeaks.Count == 1, "peak list is present");
                if (test1)
                {
                    peakFromFitData = targetEicSinglePeakFitPeaks[0];//This is the bottom line for this block
                    peakFromFitData.ScanNumber = target.ScanLCTarget;
                }
                else
                {
                    peakFromFitData = null;
                    Utiliites.MakeSignPostForTrue(test1, "No Result Found", "ProcessTarget", ref errorlog);
                }
            }
            else
            {
                Console.WriteLine("   --We do not have enough points in the EIC");
                myResult.Error = EnumerationError.MissingPoints;
            }

            Console.WriteLine("Peak Finished Procssing" + Environment.NewLine);

            return peakFromFitData;//peakFromFitData.Feature.Abundance=fit integrated area under the fit curve
        }

        private static void ConfirmPenaltyPeakIsRelatedToMainProfile(FragmentIQTarget target, FragmentResultsObjectHolderIq myResult, Run runIn, IqResult iQresult, double fitScoreCuttoff, double ppmCuttoff, ProcessorChromatogram LcProcessor, ProcessorMassSpectra MsProcessor, FragmentedTargetedWorkflowParametersIQ workflowParameters, List<XYData> targetEicClipped, CorrelationObject targetPeak)
        {
            double oldFitScore = iQresult.ObservedIsotopicProfile.Score;
            
            List<XYData> penaltyXYData = ConvertXYData.DeconXYDataToOmicsXYData(targetPeak.PeakChromXYData[0]);
            List<XYData> penaltyEicClipped = ChangeRange.ClipXyDataToScanRange(penaltyXYData, myResult.ScanBoundsInfo, false);
                //clipping is to remove artificial tails//this may not be needed

            //2.  check correlation of first and most abundant isotopes

            //correlate targetEicClipped and penaltyEicClipped
            

            SimplePeakCorrelator localCorrerlator = new SimplePeakCorrelator(runIn, workflowParameters, 0, LcProcessor);
            //0 common peak
           
            ProcessedPeak commmonPeakForClipping = new ProcessedPeak(0,0);
            commmonPeakForClipping.MinimaOfLowerMassIndex = 0;
            commmonPeakForClipping.MinimaOfHigherMassIndex = targetEicClipped.Count-1;

            //a.  peak detect
            //b.  peak clip
            //c.  peak fit
            //d.  return model for correlation
            double areaForTesting;

            Tuple<string, string> errorlogTarget = new Tuple<string, string>("PenaltyFit", "Success");
            List<XYData> targetEicDataForCorrelation = new List<XYData>();
            List<ProcessedPeak> targetEicMainIsotopePeaks = Utiliites.ClipFitAndCalculateArea(targetEicClipped, commmonPeakForClipping, workflowParameters.LCParameters.LM_RsquaredCuttoff, out targetEicDataForCorrelation, ref myResult, ref LcProcessor, ref errorlogTarget, out areaForTesting);

            Tuple<string, string> errorlogPenalty = new Tuple<string, string>("PenaltyFit", "Success");
            List<XYData> targeEicPenaltyForCorrelation = new List<XYData>();
            List<ProcessedPeak> penaltyEicMainIsotopePeaks = Utiliites.ClipFitAndCalculateArea(penaltyEicClipped, commmonPeakForClipping, workflowParameters.LCParameters.LM_RsquaredCuttoff, out targeEicPenaltyForCorrelation, ref myResult, ref LcProcessor, ref errorlogPenalty, out areaForTesting);

            bool test1 = errorlogTarget.Item2 == "Success";
            bool test2 = errorlogPenalty.Item2 == "Success";
            bool test3 = targetEicClipped != null && targetEicClipped.Count > 0;
            bool test4 = penaltyEicClipped != null && targetEicClipped.Count > 0;
            bool test5 = targetEicDataForCorrelation != null && targetEicDataForCorrelation.Count > 0;
            bool test6 = targeEicPenaltyForCorrelation != null && targeEicPenaltyForCorrelation.Count > 0;

            if (test1 && test2 && test3 && test4 && test5 && test6)
            {

                ChromCorrelationData correlationResultNonFitData = localCorrerlator.CorrelateDataXY(targetEicClipped, penaltyEicClipped, myResult.ScanBoundsInfo.Start, myResult.ScanBoundsInfo.Stop);

                Console.WriteLine(correlationResultNonFitData.RSquaredValsMedian);

                ChromCorrelationData correlationResult = localCorrerlator.CorrelateDataXY(targetEicDataForCorrelation, targeEicPenaltyForCorrelation, myResult.ScanBoundsInfo.Start, myResult.ScanBoundsInfo.Stop);

                Console.WriteLine("non fit corr = " + correlationResultNonFitData.RSquaredValsMedian + " compared to crispy fit corr = " + correlationResult.RSquaredValsMedian);

                Console.WriteLine("Did This work " + correlationResult.RSquaredValsMedian);
                if (correlationResult.RSquaredValsMedian < 0.95)
                {
                    //3.  rescore since the penalty ion does not correlate
                    //somehow get rid of penalty ion for rescoring.  besure to reset this after

                    iQresult = ScoreWithOutPenalty(target, iQresult, fitScoreCuttoff, ppmCuttoff, MsProcessor, workflowParameters);

                    if (iQresult.ObservedIsotopicProfile.Score >= fitScoreCuttoff)
                    {
                        //errorlog = new Tuple<string,string>("Process","fitscore");
                        //for the deuterated, we need to check the pure compound for a better fit

                        ///Case1.  The penalty ion is there
                        ///In this case, the penalty ion does not correlate with the target
                        /// after removing the penalty from the fit score, it is still bad suggesting the penalty ion is required for success
                        /// Penalty worked well and we fail this
                        myResult.Error = EnumerationError.FailedFitScore;
                    }
                    else
                    {
                        ///Case2.  The penalty ion is there
                        ///In this case, the penalty ion does not correlate with the target
                        /// after removing the penalty from the fit score, the fit score was saved indicating a perfect storm where the coeluting peak is muddeling up the target
                        /// this should be kept as CORRECT and DETECTED IN THE DATA and perhaps corrected downstream for contributions
                        myResult.Error = EnumerationError.Success;

                        if (targetEicClipped.Count == penaltyEicClipped.Count)
                        {
                            bool toWriteOverlapLC = false;
                            if (toWriteOverlapLC)
                            {
                                StringListToDisk writer = new StringListToDisk();
                                List<string> lines = new List<string>();
                                lines.Add("old fit " + oldFitScore + " charge " + iQresult.ObservedIsotopicProfile.ChargeState + " scan " + iQresult.LCScanSetSelected.PrimaryScanNumber + " old corr " + correlationResultNonFitData.RSquaredValsMedian + " new corr " + correlationResult.RSquaredValsMedian + " new fit score " + iQresult.ObservedIsotopicProfile.Score);
                                for (int i = 0; i < targetEicClipped.Count; i++)
                                {
                                    lines.Add(String.Concat(targetEicClipped[i].X, @",", targetEicClipped[i].Y, @",", penaltyEicClipped[i].Y));
                                }
                                //string file = @"Y:\allighment1and2isotopes" + target.Code + @".csv";
                                string file = @"\\winhpcfs\Projects\DMS\PIC_HPC\Hot\allighment1and2_" + target.Code + "_" + iQresult.ObservedIsotopicProfile.ChargeState + "_" + iQresult.LCScanSetSelected.PrimaryScanNumber + @".csv";
                                writer.toDiskStringList(file, lines);

                                List<string> lines2 = new List<string>();
                                lines.Add("old fit " + oldFitScore + " charge " + iQresult.ObservedIsotopicProfile.ChargeState + " scan " + iQresult.LCScanSetSelected.PrimaryScanNumber + " old corr " + correlationResultNonFitData.RSquaredValsMedian + " new corr " + correlationResult.RSquaredValsMedian + " new fit score " + iQresult.ObservedIsotopicProfile.Score);
                                for (int i = 0; i < targetEicDataForCorrelation.Count; i++)
                                {
                                    lines2.Add(String.Concat(targetEicDataForCorrelation[i].X, @",", targetEicDataForCorrelation[i].Y, @",", targeEicPenaltyForCorrelation[i].X, @",", targeEicPenaltyForCorrelation[i].Y));
                                }
                                string file2 = @"Y:\allighment1and2isotopes" + target.Code + @".csv";
                                //string file2 = @"\\winhpcfs\Projects\DMS\PIC_HPC\Hot\alligh1and2fit_" + target.Code + "_" + iQresult.ObservedIsotopicProfile.ChargeState + "_" + iQresult.LCScanSetSelected.PrimaryScanNumber + @".csv";
                                writer.toDiskStringList(file2, lines2);
                            }
                        }
                    }
                }
                else
                {
                    //it is allready marked for failure.  Penalty ion correlates with main peak so it is a true penalty
                }
            }
            else
            {
                //it is allready marked for failure.  Penalty ion correlates with main peak so it is a true penalty
            }
        }

        private static IqResult ScoreWithOutPenalty(FragmentIQTarget target, IqResult iQresult, double fitScoreCuttoff,double ppmCuttoff, ProcessorMassSpectra MsProcessor,FragmentedTargetedWorkflowParametersIQ workflowParameters)
        {
            float observedPenaltyValue = iQresult.ObservedIsotopicProfile.Peaklist[0].Height;

            //zero out temporarily so no penalty is felt
            iQresult.ObservedIsotopicProfile.Peaklist[0].Height = 0;

            Console.WriteLine("Initial FitScore = " + iQresult.FitScore);

            PreProcessForFitScore(ref iQresult, target, fitScoreCuttoff, ppmCuttoff, MsProcessor,workflowParameters.MSParameters.IsoParameters);

            iQresult.ObservedIsotopicProfile.Peaklist[0].Height = observedPenaltyValue;

            Console.WriteLine("NonPenalized FitScore = " + iQresult.FitScore);
            return iQresult;
        }


        public static void PreProcessForFitScore(ref IqResult iQresult, FragmentIQTarget target, double fitScoreCuttoff, double ppmCuttoff, ProcessorMassSpectra MsProcessor, IsotopeParameters isoParams)
        {
            
            
            #region

            //we need to check and see if the core ions are present based on integrated area at some percentile of the theory
            //1.  add up theory (based on what ever dynamic range we have)
            //2.  sort theory by abundance
            //3.  add ions till 50% of area is acheived (or 75%)
            double cuttoffArea = isoParams.CuttOffArea;
            //double cuttoffArea = 0.75;
            //4.  this makes up the list of required ions
            //5.  if the required ions are not present, fitscore = 1;

            bool sufficientNumberOfIons = EnsureBetterPartOfIsotopeProfileIsAboveNoise(target.TheorIsotopicProfile, iQresult, ppmCuttoff, cuttoffArea);

            #endregion

            //iQresult.ObservedIsotopicProfile.Score = fitScoreCalculator.CalculateFitScore(target.TheorIsotopicProfile, iQresult.ObservedIsotopicProfile, iQresult.IqResultDetail.MassSpectrum);

            //do we need to calibrate this?
            //THIS IS THE KEY FIT SCORE THAT ACCCEPTS OR DENYS THE OBSERVED PROFILE
            if (sufficientNumberOfIons == true)
            {
                //set current isotope to use for calculations, not storage
                IsotopicProfile temporaryProfile;
                if (target.TheorIsotopicProfile.isEstablished)
                {
                    temporaryProfile = target.TheorIsotopicProfile.CloneIsotopicProfile();
                    temporaryProfile.UpdatePeakListWithAlternatePeakIntensties();
                    temporaryProfile.EstablishAlternatvePeakIntensites(target.TheorIsotopicProfile.EstablishedMixingFraction);
                }
                else
                {
                    temporaryProfile = target.TheorIsotopicProfile;
                }

                iQresult.ObservedIsotopicProfile.Score = MsProcessor.ExecuteFitScore(temporaryProfile, iQresult.ObservedIsotopicProfile);
                iQresult.Target.TheorIsotopicProfile = target.TheorIsotopicProfile;
            }
            else
            {
                iQresult.ObservedIsotopicProfile.Score = 1; //needs to be greater than fitscore cuttoff
                if (fitScoreCuttoff > 1)
                {
                    iQresult.ObservedIsotopicProfile.Score = 1 + fitScoreCuttoff; //thow shal not pass
                }
            }

            //populate other location for fit scores
            iQresult.FitScore = iQresult.ObservedIsotopicProfile.Score;
        }

        public static bool EnsureBetterPartOfIsotopeProfileIsAboveNoise(IsotopicProfile isoTheory, IqResult iQresult,double ppmCuttoff, double cuttoffArea)
        {
            List<MSPeak> theoryIsotopeIntensityValues = new List<MSPeak>();
            List<MSPeak> sortedTheoryIsotopeIntensityValues = new List<MSPeak>();
            float sumOfTheory = new float();
            foreach (MSPeak peak in isoTheory.Peaklist)
            {
                theoryIsotopeIntensityValues.Add(peak);
                sumOfTheory += peak.Height;
            }

            sortedTheoryIsotopeIntensityValues = theoryIsotopeIntensityValues.OrderByDescending(n => n.Height).ToList();

            List<double> requiredPeaks = new List<double>();
            float experimentalSum = 0;
            int cricialIonCountTheoretical = 0;
            while (experimentalSum < sumOfTheory*cuttoffArea)
            {
                experimentalSum += sortedTheoryIsotopeIntensityValues[cricialIonCountTheoretical].Height;
                requiredPeaks.Add(sortedTheoryIsotopeIntensityValues[cricialIonCountTheoretical].XValue);
                cricialIonCountTheoretical++;
            }

            int cricialIonCountExperiental = 0;
            foreach (double mass in requiredPeaks)
            {
                foreach (MSPeak requiredPeak in iQresult.ObservedIsotopicProfile.Peaklist)
                {
                    double da = ErrorCalculator.PPMtoDaTollerance(ppmCuttoff, mass);
                    if (requiredPeak.XValue >= mass - da && requiredPeak.XValue <= mass + da)
                    {
                        cricialIonCountExperiental++;
                    }
                }
            }

            if (cricialIonCountExperiental < cricialIonCountTheoretical)
            {
                Console.WriteLine("The critical ions are missing from the experimental data at the " + cuttoffArea + " level");
                //iQresult.ObservedIsotopicProfile.Peaklist = null;
                //iQresult.ObservedIsotopicProfile = null;
                return false;
            }
            else
            {
                Console.WriteLine("ObservedProfile looks great because most of it is present");
                return true;
            }
        }
    }
}
