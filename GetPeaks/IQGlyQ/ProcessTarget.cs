using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.ProcessingTasks.ResultValidators;
using DeconTools.Backend.ProcessingTasks.TargetedFeatureFinders;
using GetPeaks_DLL;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.Functions;
using IQGlyQ.Enumerations;
using IQGlyQ.Functions;
using IQGlyQ.Processors;
using IQGlyQ.Results;
using PNNLOmics.Data;
using IQGlyQ.Objects;
using DeconTools.Backend.Core;
using DeconTools.Workflows.Backend.Core;
using DeconTools.Backend.ProcessingTasks.MSGenerators;
using DeconTools.Backend.ProcessingTasks.Smoothers;
using DeconTools.Backend.ProcessingTasks;
using PNNLOmics.Algorithms.PeakDetection;
using PNNLOmics.Data.Constants;
using Peak = DeconTools.Backend.Core.Peak;
using DeconTools.Backend.ProcessingTasks.FitScoreCalculators;

namespace IQGlyQ
{
    public static class ProcessTarget
    {

        /// <summary>
        /// This is the key workflow that takes a target and turns it into a clippled, fit XY chromatogram of one peak
        /// </summary>
        /// <param name="numberOfScansToBuffer">buffering for Savitsky Golay Smoothing</param>
        /// <param name="scansToSum">We make the EIC from summed data</param>
        /// <param name="chromToleranceInPPM">ppm for EIC</param>
        /// <param name="MinRelativeIntensityForChromCorrelator">removes noise from EIC</param>
        /// <param name="omicsPeakDetection">differential peak picking for smoothed chromatograms</param>
        /// <param name="_msGenerator">generates XYData and stores it in the run</param>
        /// <param name="_smoother">Savitsky Golay Smother for EIC</param>
        /// <param name="_peakChromGen">Enerates EIC</param>
        /// <param name="errorlog">Error logging for when the workflow fails</param>
        /// <param name="printString">type of target we are looking at</param>
        /// <param name="target">INPUT:  The target for analtysis</param>
        /// <param name="targetEicSingleXYDataFit">OUTPUT:  the XYData to send to the correlator</param>
        /// <param name="peakFromFitData">OUTPUT:  contains the peak information from the XY </param>
        //public static ProcessedPeak ProcessTarget(int numberOfScansToBuffer, int scansToSum, ResultCollection resultList, double chromToleranceInPPM, double MinRelativeIntensityForChromCorrelator, ref PeakCentroider omicsPeakDetection, ref MSGenerator _msGenerator, ref SavitzkyGolaySmoother _smoother, ref PeakChromatogramGenerator _peakChromGen, ref Tuple<string, string> errorlog, string printString,
        //    FragmentTarget target, out List<XYData> targetEicSingleXYDataFit)
        //public static ProcessedPeak Process(ScanObject scans, Run runIn, IqResult iQresult, double chromToleranceInPPM, double MinRelativeIntensityForChromCorrelator, ref PeakCentroider omicsPeakDetection, ref MSGenerator _msGenerator, ref SavitzkyGolaySmoother _smoother, ref PeakChromatogramGenerator _peakChromGen, ref Tuple<string, string> errorlog, string printString,
        //    FragmentIQTarget target, out List<XYData> targetEicSingleXYDataFit)
        //public static ProcessedPeak Process(FragmentIQTarget target, ref FragmentResultsObjectHolderIQ myResult, Run runIn, IqResult iQresult, double chromToleranceInPPM, double MinRelativeIntensityForChromCorrelator, out ScanObject scanRangeFit, ref PeakCentroider omicsPeakDetection, ref MSGenerator _msGenerator, ref SavitzkyGolaySmoother _smoother, ref PeakChromatogramGenerator _peakChromGen, ref Tuple<string, string> errorlog, string printString,
        //    out List<XYData> targetEicSingleXYDataFit)

        public static ProcessedPeak Process(
            FragmentIQTarget target, 
            ref FragmentResultsObjectHolderIq myResult, 
            Run runIn, 
            IqResult iQresult, 
            //ref MSGenerator _msGenerator,
            ref IterativeTFF _msfeatureFinder,
            //ref Task _fitScoreCalculator,
            ref Tuple<string, string> errorlog, string printString,
            out List<XYData> targetEicSingleXYDataFit,
            double fitScoreCuttoff,
            double ppmCuttoff,

            ref Processors.ProcessorChromatogram LcProcessor,
            ref Processors.ProcessorMassSpectra MsProcessor,
            FragmentedTargetedWorkflowParametersIQ workflowParameters)
        {
            //extract chroatograms with 2x sg number (_numPointsInSmoother) more points and then truncate it down to remove edge effects

            //IsotopicPeakFitScoreCalculator fitScoreCalculator = (IsotopicPeakFitScoreCalculator)_fitScoreCalculator;

            List<ProcessedPeak> peaksInEIC = new List<ProcessedPeak>();
            List<PNNLOmics.Data.XYData> targetEicClipped = new List<XYData>();
            List<PNNLOmics.Data.XYData> targetEic = new List<XYData>();

            if (errorlog.Item2 == "Success")
            {
                #region inside for correlation object
                //todo this is not necessarily the best scan set.  just the mid point of the origional window.  it is best to pull the scan set at the apex of the candidate
                

                //calculate centerscan so we can pull the best mass spec possible
                DeconTools.Backend.XYData deconChromatogram = workflowParameters.LCParameters.Engine_PeakChromGenerator.GenerateChromatogram(runIn, target.TheorIsotopicProfile);
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

                

                #region IFF workaround
                //we need a work around here because IFF is looking for a mono in position 0. when we append a 0 in front, it looks like a mono
                IsotopicProfile isotopicProfileObserved = new IsotopicProfile();

                //isotopicProfileObserved = IterativeFeatureFinderWrapper(target.TheorIsotopicProfile, iQresult.IqResultDetail.MassSpectrum, _msfeatureFinder);
                isotopicProfileObserved = IterativelyFindMSFeatureWrapper.IterativeFeatureFind(target.TheorIsotopicProfile, iQresult.IqResultDetail.MassSpectrum, _msfeatureFinder);


                //finally store data again
                
                iQresult.ObservedIsotopicProfile = isotopicProfileObserved;
                myResult.FragmentObservedIsotopeProfile = iQresult.ObservedIsotopicProfile;


               

                #endregion
                //TODO we need XYData

                if (iQresult.ObservedIsotopicProfile != null)
                {
                    #region

                    //we need to check and see if the core ions are present based on integrated area at some percentile of the theory
                    //1.  add up theory (based on what ever dynamic range we have)
                    //2.  sort theory by abundance
                    //3.  add ions till 50% of area is acheived (or 75%)
                    double cuttoffArea = workflowParameters.MSParameters.IsoParameters.CuttOffArea;
                    //double cuttoffArea = 0.75;
                    //4.  this makes up the list of required ions
                    //5.  if the required ions are not present, fitscore = 1;

                    bool sufficientNumberOfIons = EnsureBetterPartOfIsotopeProfileIsAboveNoise(target.TheorIsotopicProfile, iQresult, ppmCuttoff, cuttoffArea);

                    #endregion


                    //iQresult.ObservedIsotopicProfile.Score = fitScoreCalculator.CalculateFitScore(target.TheorIsotopicProfile, iQresult.ObservedIsotopicProfile, iQresult.IqResultDetail.MassSpectrum);

                    //do we need to calibrate this?
                    if (sufficientNumberOfIons == true)
                    {
                        iQresult.ObservedIsotopicProfile.Score = MsProcessor.ExecuteFitScore(target.TheorIsotopicProfile, iQresult.ObservedIsotopicProfile);
                        
                    }
                    else
                    {
                        iQresult.ObservedIsotopicProfile.Score = 1;//needs to be greater than fitscore cuttoff
                        if(fitScoreCuttoff>1)
                        {
                            iQresult.ObservedIsotopicProfile.Score = 1 + fitScoreCuttoff;//thow shal not pass
                        }
                    }

                    //populate other location for fit scores
                    iQresult.FitScore = iQresult.ObservedIsotopicProfile.Score;

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
                    double ppmError = GetPeaks_DLL.Functions.ErrorCalculator.PPMAbsolute(iQresult.ObservedIsotopicProfile.MonoIsotopicMass, calibratedTheoreticalMono);
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
                    double da = mass*ppmCuttoff/1000000;
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
