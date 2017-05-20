using System;
using System.Collections.Generic;
using System.Linq;
using GetPeaksDllLite.Functions;
using IQGlyQ.Enumerations;
using IQGlyQ.Functions;
using IQGlyQ.Objects.EverythingIsotope;
using IQGlyQ.Processors;
using IQGlyQ.Results;
using IQGlyQ.TargetGenerators;
using IQGlyQ.Objects;
using IQ_X64.Backend.ProcessingTasks.LCGenerators;
using IQ_X64.Backend.ProcessingTasks.TargetedFeatureFinders;
using IQ_X64.Workflows.Core;
using IQ_X64.Workflows.Utilities;
using PNNLOmics.Algorithms.Solvers.LevenburgMarquadt;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.Peaks;
using Run64.Backend.Core;
using Peak = Run64.Backend.Core.Peak;
using XYData = PNNLOmics.Data.XYData;

namespace IQGlyQ
{
    public static class Utiliites
    {
        //public static void SetScanRanges(FragmentTarget target, int bufferScans, int maxScanInDataset, out int startScan, out int stopScan, ref Tuple<string, string> errorLog)
        //{
        //    bool test1 = SignPostRequire(target.StartScan - bufferScans < 0, "Scan less than 0");
        //    bool test2 = SignPostRequire(target.StopScan + bufferScans > maxScanInDataset, "Scan larger than dataset");

        //    startScan = 0;
        //    stopScan = 0;

        //    if (test1==false && test2==false)
        //    {
        //        startScan = target.StartScan - bufferScans;
        //        stopScan = target.StopScan + bufferScans;
        //    }
        //    else
        //    {
        //        MakeSignPostForTrue(test1, "Scan less than 0", "SetScanRange", ref errorLog);
        //        MakeSignPostForTrue(test2, "Scan larger than dataset", "SetScanRange", ref errorLog);
        //        if (target.StartScan - bufferScans < 0)
        //        {
        //            startScan = 0;
        //        }
        //        if (target.StopScan + bufferScans > maxScanInDataset)
        //        {
        //            stopScan = maxScanInDataset;//-1?
        //        }
        //        target.ErrorCode = EnumerationError.FailedScanRange;
        //    }

        //}

        //public static void SetScanRangesIQ(ref FragmentIQTarget target, ref Tuple<string, string> errorLog, out EnumerationError error)
        //{
        //    int bufferScans = target.ScanInfo.Buffer;
        //    int maxScanInDataset = target.ScanInfo.Max;
        //    int minScanInDataset = target.ScanInfo.Min;
            
        //    error = EnumerationError.NoError;
        //    bool test1 = SignPostRequire(target.ScanInfo.Start - bufferScans < minScanInDataset, "Scan less than 0");
        //    bool test2 = SignPostRequire(target.ScanInfo.Stop + bufferScans > maxScanInDataset, "Scan larger than dataset");

        //    //target.ScanInfo.Start = 0;
        //    //target.ScanInfo.Stop = 0;

        //    if (test1 == false && test2 == false)
        //    {
        //        target.ScanInfo.Start = target.ScanInfo.Start - bufferScans;
        //        target.ScanInfo.Stop = target.ScanInfo.Stop + bufferScans;
        //    }
        //    else
        //    {
        //        MakeSignPostForTrue(test1, "Scan less than 0", "SetScanRange", ref errorLog);
        //        MakeSignPostForTrue(test2, "Scan larger than dataset", "SetScanRange", ref errorLog);
        //        if (target.ScanInfo.Start - bufferScans < minScanInDataset)
        //        {
        //            target.ScanInfo.Start = minScanInDataset;
        //        }
        //        if (target.ScanInfo.Stop + bufferScans > maxScanInDataset)
        //        {
        //            target.ScanInfo.Stop = maxScanInDataset;//-1?
        //        }
        //        error = EnumerationError.FailedScanRange;
        //    }

        //}

        public static void CheckDllsForGlyQIQ()
        {
            Run64.HelloWorld.Check();
            IQ2_x64.HelloWorld.Check();
            GetPeaksDllLite.HelloWorld.Check();
            IQGlyQ.HelloWorld.Check();
            IQGlyQ.HelloWorld.CheckAlgilib();
            IQGlyQ.HelloWorld.CheckMathDotNetNumerics();
            IQGlyQ.HelloWorld.CheckPNNLOmics();
        }

        public static Run64.Backend.Data.XYData GenerateBufferedChromatogramAbolute(PeakChromatogramGenerator _peakChromGen, Run runIn, ScanObject scans, double massToExtract, double chromTollerencePPM)
        {
            
            
            ScanObject bufferedRange = new ScanObject(scans);

            bufferedRange.Start = scans.Start - scans.Buffer;
            bufferedRange.Stop = scans.Stop + scans.Buffer;

            if (bufferedRange.Start < scans.Min)
            {
                bufferedRange.Start = scans.Min;
            }
            if (bufferedRange.Stop > scans.Max)
            {
                bufferedRange.Stop = scans.Max;//-1?
            }

            //DeconTools.Backend.XYData deconChromatogram = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(runIn, massToExtract, bufferedRange, chromTollerencePPM);old
            var deconChromatogram = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(runIn, massToExtract, bufferedRange);

            return deconChromatogram;
        }


       

        public static CorrelationObject CreateCorrelationObject(IsotopicProfile iso, ScanObject scans, Run run, ref Tuple<string, string> errorLog, out EnumerationError errorCode, ref Processors.ProcessorChromatogram LcProcessor, FragmentedTargetedWorkflowParametersIQ workflowParameters)
        {
            errorCode = EnumerationError.NoError;
            CorrelationObject loadPossiblePeak = new CorrelationObject();

            bool test0 = SignPostRequire(iso!=null, "Is there an Iso");
            if (test0)
            {
                loadPossiblePeak = new CorrelationObject(iso, scans, run, ref LcProcessor, workflowParameters);
            }
            else
            {
                MakeSignPostForTrue(!test0, "Iso Is Missing", "CreateCorrelationObject", ref errorLog);
                errorCode = EnumerationError.FailedEIC;
            }

            bool test1 = SignPostRequire(loadPossiblePeak.AreChromDataOK, "AreChromDataIsOK is not OK");
            bool test2 = SignPostRequire(loadPossiblePeak.IsosPeakListIsOK, "IsosPeakListIsOK is not OK");

            bool test3 = true;
            if (test0)
            {
                //we want to know if the most abundant peak produces acceptable chrom data.  this allows for the mono to be in the grass
                int highestIndex = iso.GetIndexOfMostIntensePeak();
                test3 = SignPostRequire(loadPossiblePeak.AcceptableChromList[highestIndex], "The most abundant EIC is not OK");
            }

            bool isThereAPeakToTheLeft = workflowParameters.MSParameters.IsoParameters.PenaltyMode == EnumerationIsotopePenaltyMode.PointToLeft 
                || workflowParameters.MSParameters.IsoParameters.PenaltyMode == EnumerationIsotopePenaltyMode.PointToLeftAndHarmonic;

            //do we force a monoisotopic mass to Exist
            
            bool isItHighMass = false;//this is when the mono is expected to be in the grass.  This is based on the intensity of the mono relatice to the highest peak
            double minHeightForMonoEnforcement = 0.2;
            int indexOfMono = 0;
            indexOfMono = isThereAPeakToTheLeft ? iso.MonoIsotopicPeakIndex + 1 : iso.MonoIsotopicPeakIndex;

            if (iso.Peaklist != null && iso.Peaklist.Count > indexOfMono)
            {
                if(iso.Peaklist[indexOfMono].Height < minHeightForMonoEnforcement)//if the mono is greater than x%, we need to make sure it is present to continue
                {
                    isItHighMass = true;
                }
            }

            bool test4 = true;
            if(isItHighMass)//should we enforce a monoexists
            {
                //don't enfore that a mono exisits
                test4 = true;
            }
            else
            {
                test4 = false;//enforce that a mono is present
                if (isThereAPeakToTheLeft)
                {
                    //check if mono exists
                    indexOfMono = iso.MonoIsotopicPeakIndex + 1;
                    test4 = SignPostRequire(loadPossiblePeak.AcceptableChromList[indexOfMono], "MonoisotopicPeakOK is not OK");
                }
                else
                {
                    indexOfMono = iso.MonoIsotopicPeakIndex;
                    test4 = SignPostRequire(loadPossiblePeak.AcceptableChromList[indexOfMono], "MonoisotopicPeakOK is not OK");
                }
            }

            if (test1 && test2 && test3 && test4)
            {
            }
            else
            {
                loadPossiblePeak = null;

                if (test0)
                {
                    MakeSignPostForTrue(!test1, "Chromatogram Not Found", "CreateCorrelationObject", ref errorLog);
                    MakeSignPostForTrue(!test2, "IsotopeProfile not sufficient.  We need 3 points and a mono", "CreateCorrelationObject", ref errorLog);
                    MakeSignPostForTrue(!test3, "Is EIC 1 is not ok", "CreateCorrelationObject", ref errorLog);
                    errorCode = EnumerationError.FailedEIC;
                }
            }

            return loadPossiblePeak;
        }

        /// <summary>
        /// Select one EIC to represent the feature
        /// </summary>
        /// <param name="loadBasePeak"></param>
        /// <param name="errorLog"></param>
        /// <returns></returns>
        public static List<PNNLOmics.Data.XYData> PullBestEIC(CorrelationObject loadBasePeak, ref Tuple<string, string> errorLog)
        {
            bool test1 = SignPostRequire(loadBasePeak != null, "No corrleation object");
            bool test2;
            bool test3;
            

            List<PNNLOmics.Data.XYData> loadedLC = new List<PNNLOmics.Data.XYData>();

            if (test1 )
            {
                test2 = SignPostRequire(loadBasePeak.PeakChromXYData != null, "List PeakChromXYData values");
                //test2 = true;
                if (test2)
                {
                    double xValue = 0;
                    double yValue = 0;

                    test3 = SignPostRequire(loadBasePeak.PeakChromXYData[loadBasePeak.IndexMostAbundantPeak] != null, "List PeakChromXYData values");
                    //test3 = true;
                    if (test3)
                    {
                        for (int k = 0; k < loadBasePeak.PeakChromXYData[loadBasePeak.IndexMostAbundantPeak].Xvalues.Length; k++)
                        {
                            xValue = loadBasePeak.PeakChromXYData[loadBasePeak.IndexMostAbundantPeak].Xvalues[k];
                            yValue = loadBasePeak.PeakChromXYData[loadBasePeak.IndexMostAbundantPeak].Yvalues[k];
                            PNNLOmics.Data.XYData peak = new PNNLOmics.Data.XYData(xValue, yValue);
                            loadedLC.Add(peak);
                            //Console.WriteLine("F," + xValue + "," + yValue);
                        }
                    }
                    else
                    {
                        loadedLC = null;
                        MakeSignPostForTrue(!test3, "Missing desired PeakChromXYData", "PullBestEIC", ref errorLog);
                    }
                    return loadedLC;
                }
                else
                {
                    loadedLC = null;
                    MakeSignPostForTrue(!test2, "Missing desired PeakChromXYDataList", "PullBestEIC", ref errorLog);
                }
            }
            else
            {
                loadedLC = null;
                MakeSignPostForTrue(!test1, "No corrleation object", "PullBestEIC", ref errorLog);
                
                

            }

            return loadedLC;

        }

        public static IsotopicProfile GenerateCombinedIsotopicProfile(IqResult result, int isotopeOffset, float ratioProfile2)
        {
            IsotopicProfile comboProfile = new IsotopicProfile();

            //List<MSPeak> comboPeakList = new List<MSPeak>();
            //for (int i = 0; i < isotopeOffset; i++)
            //{
            //    MSPeak profile1Peak = result.Target.TheorIsotopicProfile.Peaklist[i];
            //    comboPeakList.Add(new MSPeak(profile1Peak.XValue, profile1Peak.Height, profile1Peak.Width, profile1Peak.SignalToNoise));
            //}

            //for (int i = isotopeOffset; i < result.Target.TheorIsotopicProfile.Peaklist.Count; i++)
            //{
            //    MSPeak profile1Peak = result.Target.TheorIsotopicProfile.Peaklist[i - isotopeOffset];
            //    MSPeak profile2Peak = result.Target.TheorIsotopicProfile.Peaklist[i];

            //    comboPeakList.Add(new MSPeak(
            //                          profile2Peak.XValue,
            //                          profile1Peak.Height + ratioProfile2 * profile2Peak.Height,
            //                          profile1Peak.Width,
            //                          profile1Peak.SignalToNoise));
            //}

            //float maxHeight = comboPeakList.Max(r => r.Height); //height
            //foreach (var msPeak in comboPeakList)
            //{
            //    msPeak.Height = msPeak.Height/maxHeight;
            //}

            //comboProfile = result.Target.TheorIsotopicProfile.CloneIsotopicProfile();
            //comboProfile.Peaklist = comboPeakList;
            comboProfile = TheoreticalIsotopicProfileWrapper.GenerateCombinedIsotopicProfile(result.Target.TheorIsotopicProfile, isotopeOffset, ratioProfile2);

            return comboProfile;
        }

        public static double GetMassToExtractFromIsotopeProfile(IsotopicProfile profile, FragmentedTargetedWorkflowParametersIQ tempParameters)
        {
            double massToExtract;
            switch (tempParameters.MSParameters.IsoParameters.IsotopeProfileMode)
            {
                case EnumerationIsotopicProfileMode.H:
                    {
                        massToExtract = profile.getMostIntensePeak().XValue;
                    }
                    break;
                case EnumerationIsotopicProfileMode.DH:
                    {
                        if(profile.AlternatePeakIntensities.Length==0)
                        {
                            Console.WriteLine("Error: EnumerationIsWrong");
                            System.Threading.Thread.Sleep(3000);
                        }
                        massToExtract = profile.GetMaxMassFromAlternatePeakIntensities();
                    }
                    break;
                default:
                    {
                        massToExtract = profile.getMostIntensePeak().XValue;
                    }
                    break;
            }
            return massToExtract;
        }
        

        public static void SelectClosestLCPeak(ref List<ProcessedPeak> basePeaksInEIC, int xLCCenter, ref Tuple<string, string> errorLog)
        {
            bool test1 = SignPostRequire(basePeaksInEIC != null, "Null Peaks");
            bool test2;
            //test1 = true;
            if (test1)
            {
                
                test2 = SignPostRequire(basePeaksInEIC.Count >= 1, "Not Enough Peaks");
                if (test2)
                {
                    //seleect closest peak to scanLC
                    int difference = (basePeaksInEIC[0].ScanNumber - xLCCenter) * (basePeaksInEIC[0].ScanNumber - xLCCenter); //difference squared
                    ProcessedPeak keeperPeak = basePeaksInEIC[0];

                    foreach (ProcessedPeak pPeak in basePeaksInEIC)
                    {
                        int differenceTest = (pPeak.ScanNumber - xLCCenter) * (pPeak.ScanNumber - xLCCenter); //difference squared;
                        if (differenceTest < difference)
                        {
                            keeperPeak = pPeak;
                            difference = differenceTest;
                        }
                    }
                    basePeaksInEIC = new List<ProcessedPeak>();
                    basePeaksInEIC.Add(keeperPeak);
                }
                else
                {
                    MakeSignPostForTrue(test1, "Not Enough Peaks", "SetClosestLCPeak", ref errorLog);
                }

            }
            else
            {
                MakeSignPostForTrue(test1, "Null Data", "SetClosestLCPeak", ref errorLog);
            }
        }

        public static void RenewChromPeakStartStop(ref Run runIn, FragmentedTargetedWorkflowParametersIQ workflowParameters, ref Processors.ProcessorChromatogram LcProcessor, FragmentResultsObjectIq processedTargetResult, ref IqGlyQResult iQResult, ref ScanObject peakQualityScanBounds, ref Tuple<string, string> errorlog)
        {

            if (iQResult.Target.TheorIsotopicProfile != null)
            {
                //1.  find most abundand isotopic profile
                //double massToExtract = iQResult.Target.TheorIsotopicProfile.Peaklist[iQResult.Target.TheorIsotopicProfile.GetIndexOfMostIntensePeak()].XValue;

                double massToExtract = Utiliites.GetMassToExtractFromIsotopeProfile(iQResult.Target.TheorIsotopicProfile, workflowParameters);

                //2.  generate buffered chromatogram vased on large bounds set in workflow
                //DeconTools.Backend.XYData deconBufferedChromatogram = LcProcessor.GenerateBufferedChromatogramByPoint(runIn, peakQualityScanBounds, currentMZMostAbundant, workflowParameters.LCParameters.ParametersChromGenerator.ChromToleranceInPPM);
               var deconBufferedChromatogram = LcProcessor.GenerateBufferedChromatogramByPoint(runIn, peakQualityScanBounds, massToExtract);

                List<ProcessedPeak> renewFeature = null;
                List<PNNLOmics.Data.XYData> omicsBufferedChromatogram = null;
                if (deconBufferedChromatogram != null)
                {
                    omicsBufferedChromatogram = ConvertXYData.DeconXYDataToOmicsXYData(deconBufferedChromatogram);

                    //3.  same ChromatogramLevel processing except on buffered data
                    //old used to work 1-8-2014
                    //List<ProcessedPeak> renewFeature = LcProcessor.Execute(omicsBufferedChromatogram, peakQualityScanBounds, EnumerationChromatogramProcessing.ChromatogramLevel);
                    renewFeature = LcProcessor.Execute(omicsBufferedChromatogram, peakQualityScanBounds, workflowParameters.LCParameters.ProcessLcChromatogram);
                }

                //4.  select desired list from mini list of features
                if (renewFeature !=null && renewFeature.Count > 1)
                {
                    foreach (ProcessedPeak processedPeak in renewFeature)
                    {
                        if (processedPeak.XValue < peakQualityScanBounds.Max)
                        {
                            processedPeak.ScanNumber = Convert.ToInt32(processedPeak.XValue);
                        }
                        else
                        {
                            
                            //this is an interesting case of infinity
                            Console.WriteLine("We have exceded the int (" + processedPeak.XValue + ") and replaced it with the max lc scan number" + runIn.MaxLCScan);
                            processedPeak.ScanNumber = Convert.ToInt32(runIn.MaxLCScan);
                            Console.WriteLine("runIn.MaxLCScan worked");
                            //processedPeak.ScanNumber = Convert.ToInt32(peakQualityScanBounds.Max);
                            //Console.ReadKey();
                        }
                    }
                    Utiliites.SelectClosestLCPeak(ref renewFeature, processedTargetResult.PeakQualityObject.ScanLc, ref errorlog);
                }

                //5. if we have a peak, set up the start and stop for our single LC target
                if (renewFeature != null && omicsBufferedChromatogram !=null && renewFeature.Count == 1)
                {
                    Utiliites.ConvertProcessedPeakToScanObject(renewFeature.FirstOrDefault(), omicsBufferedChromatogram, ref peakQualityScanBounds, ref errorlog);
                }
            }
            else
            {
                //where is the friggin isotope profile
                Console.WriteLine("Arg");
            }
        }

        public static void ConvertPeakXyListToScan(List<ProcessedPeak> peakList, ref Tuple<string, string> errorLog)
        {
            bool test1 = SignPostRequire(peakList == null || peakList.Count==0, "No peak list");

            if (test1)
            {
                MakeSignPostForTrue(test1, "Not Enough Peaks", "ConvertPeakXValueToScan", ref errorLog);
            }
            else
            {
                foreach (var processedPeak in peakList)
                {
                    processedPeak.ScanNumber = Convert.ToInt32(Math.Truncate(processedPeak.XValue));
                }
            }

            //bool test1 = SignPostRequire(peakList != null && peakList.Count > 0, "No peak list");

            //if (test1)
            //{
            //    foreach (var processedPeak in peakList)
            //    {
            //        processedPeak.ScanNumber = Convert.ToInt32(Math.Truncate(processedPeak.XValue));
            //    }
            //}
            //else
            //{
            //    MakeSignPostForTrue(test1, "Not Enough Peaks", "ConvertPeakXValueToScan", ref errorLog);
            //}
        }

        public static void ConvertProcessedPeakToScanObject(ProcessedPeak peak, List<XYData> dataList, ref ScanObject scans, ref Tuple<string, string> errorLog)
        {
            bool test1 = SignPostRequire(peak.MinimaOfLowerMassIndex<peak.MinimaOfHigherMassIndex, "peaks are not in order");

            if (test1)
            {
                //scans = new ScanObject(Convert.ToInt32(dataList[peak.MinimaOfLowerMassIndex].X),Convert.ToInt32(dataList[peak.MinimaOfHigherMassIndex].X));
                scans.Start = Convert.ToInt32(dataList[peak.MinimaOfLowerMassIndex].X);
                scans.Stop = Convert.ToInt32(dataList[peak.MinimaOfHigherMassIndex].X);
            }
            else
            {
                MakeSignPostForTrue(test1, "Peaks are not in order", "ConvertProcessedPeakToScanObject", ref errorLog);
            }

            //return scans;
        }

        public static void ConvertFWHMwidthToScanWidth(List<ProcessedPeak> chromatogramOmicsPeaks, List<XYData> chromatogramRaw)
        {
            foreach (ProcessedPeak processedPeak in chromatogramOmicsPeaks)
            {
                int dataCount = chromatogramRaw.Count;

                int largerIndex = processedPeak.CenterIndexLeft;
                int smallerIndex = processedPeak.MinimaOfLowerMassIndex + 1;

                if (largerIndex >= dataCount)
                {
                    largerIndex = dataCount - 1;
                }
                if (smallerIndex >= dataCount)
                {
                    smallerIndex = dataCount - 1;
                }

                int leftPoints = Convert.ToInt32(chromatogramRaw[largerIndex].X - chromatogramRaw[smallerIndex].X);


                largerIndex = processedPeak.MinimaOfHigherMassIndex;
                smallerIndex = (processedPeak.CenterIndexLeft + 1) + 1;

                if (largerIndex >= dataCount)
                {
                    largerIndex = dataCount - 1;
                }
                if (smallerIndex >= dataCount)
                {
                    smallerIndex = dataCount - 1;
                }

                //We need to add one so we include centerIndexLeft
                int rightPoints = Convert.ToInt32(chromatogramRaw[largerIndex].X - chromatogramRaw[smallerIndex].X);
                //int rightPoints = Convert.ToInt32(chromatogramRaw[processedPeak.MinimaOfHigherMassIndex].X - chromatogramRaw[(processedPeak.CenterIndexLeft + 1) + 1].X);
                //we need to add one so we include centerIndexRight (centerindexLeft+1)

                int shortestSide = Math.Min(leftPoints, rightPoints);

                int longestSide = Math.Max(leftPoints, rightPoints);

                //processedPeak.Width = processedPeak.MinimaOfHigherMassIndex - processedPeak.MinimaOfLowerMassIndex;
                //int newWidth = processedPeak.MinimaOfHigherMassIndex - processedPeak.MinimaOfLowerMassIndex;
                //int newWidth = leftPoints + rightPoints +1;
                int newWidth = longestSide + longestSide + 1;
                processedPeak.Width = newWidth;
            }
        }



        //public static void DeviseChargeStates(ChromPeakQualityData possibiity, FragmentTarget targetPlusDifference, ref Tuple<string, string> errorLog)
        //{
        //    int lowCharge = possibiity.IsotopicProfile.ChargeState - 1;//look one behind
            
        //    if (possibiity.IsotopicProfile.ChargeState > 1)
        //    {
        //        lowCharge = possibiity.IsotopicProfile.ChargeState - 1;
        //    }
        //    else
        //    {
        //        lowCharge = 1;
        //    }

        //    for (int i = lowCharge; i <= possibiity.IsotopicProfile.ChargeState + 1; i++)//look one ahead
        //    {
        //        targetPlusDifference.ChargeStateTargets.Add(i);
        //    }
        //}

        public static void DeviseChargeStatesIQ(ChromPeakQualityData possibiity, FragmentIQTarget targetPlusDifference, ref Tuple<string, string> errorLog)
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
                IqTargetUtilities utilities = new IqTargetUtilities();
                IqTarget clonedTarget = utilities.Clone(targetPlusDifference);
                clonedTarget.ChargeState = i;
                targetPlusDifference.AddTarget(clonedTarget);
            }
        }

        
        //TODO this is very similar to ClipFitAndCalculateArea
        /// <summary>
        /// Major fitting function for LC processor
        /// </summary>
        /// <param name="clippedXYData">experimental raw data to fit</param>
        /// <param name="processedPeak">LC peak detected used for setting stat and stop</param>
        /// <param name="minCurveFitScore">low end cuttof for a good fit.  fitscored returned from the LC algoritm should be triaged elseware or use the raw data</param>
        /// <param name="clippedFitXyData">xydata points from LM fit (gaussian in this case</param>
        /// <param name="fitLcPeaksWithArea">contains integrated area calculation as Feature.Abundance</param>
        /// <param name="result">Iq result to store coefficients</param>
        /// <param name="LcProcessor">"takes care of lc peak smoothing detection etc.</param>
        /// <param name="errorLog">errors</param>
        public static List<ProcessedPeak> ClipFitAndCalculateArea(List<XYData> clippedXYData, ProcessedPeak processedPeak, double LM_RsquaredCuttoff, out List<XYData> clippedFitXyData, ref FragmentResultsObjectHolderIq result, ref Processors.ProcessorChromatogram LcProcessor, ref Tuple<string, string> errorLog, out double areaForTesting)
        {
            areaForTesting = 0;
            List<ProcessedPeak> fitLcPeaksWithArea;

            ScanObject scanRangeFit = new ScanObject(result.ScanBoundsInfo);
            //coefficents = new double[0];
            //double LM_RsquaredCuttoff = 0.5;
            int minNumberOfPointsToAllow = 5;//when iterating, cut down to this number of points.  less should fail

            bool test1 = SignPostRequire(clippedXYData.Count > 0, "Scan less than 0");
            
            if (test1 == true)
            {
                //1.  set scan range
                ScanObject scanBoundsInfo = result.ScanBoundsInfo;
                SetScanRangeFromProcessedPeak(clippedXYData, processedPeak, ref scanBoundsInfo);

                //2.  rewindow data to remove tails from smoothing.  this is currently off 2-12-13
                int bonusPadPoints = 0;
                //List<PNNLOmics.Data.XYData> possibleEicSinglePeak = ReWindowDataListFX(result.ScanBoundsInfo.Start, result.ScanBoundsInfo.Stop, clippedXYData, bonusPadPoints); //clipping is to remove artificial tails

                //TODO 2
                List<PNNLOmics.Data.XYData> possibleEicSinglePeak = ChangeRange.ClipXyDataToScanRange(clippedXYData, result.ScanBoundsInfo, false); //clipping is to remove artificial tails

                if (possibleEicSinglePeak.Count >= minNumberOfPointsToAllow)
                {
                    //3.  fit data to create synthetic profile
                    List<XYData> modeledPeakList;
                    double areaUnderCurve;
                    double[] coefficents;
                    SolverReport fitMetrics = FitWith2SeedXValues(LM_RsquaredCuttoff, out clippedFitXyData, ref errorLog, possibleEicSinglePeak, out scanRangeFit, out modeledPeakList, out areaUnderCurve, out coefficents);

                    Console.WriteLine(Environment.NewLine +  "After multi seeding, Did we converge? " + fitMetrics.DidConverge);

                    result.ScanBoundsInfo.Start = scanRangeFit.Start;
                    result.ScanBoundsInfo.Stop = scanRangeFit.Stop;
                    result.CorrelationCoefficients = coefficents;
                    if (fitMetrics != null  && fitMetrics.DidConverge==true)
                    {
                        result.LMOptimizationCorrelationRsquared = fitMetrics.RSquared;
                    }
                    //generate x point long FitXYData

                    bool constantPointsInFit = true; //false passes old unit test 1/2
                    if (constantPointsInFit) clippedFitXyData = modeledPeakList;


                    //re peak pick so we can find the start and stop
                    
           //         if (modeledPeakList.Count > 0)//this means the fit worked and we can procede to area calculation
           //         {
                        //evaluate if fit is sufficient

                        //if we converged but have a poor fit, try to improve fit
                        // or if we failed, we can try to improve by removing points till there are a minimum of points left

                        
                        bool conditionDidNotConvergeButHasOfEnoughPoints = fitMetrics.DidConverge == false & possibleEicSinglePeak.Count >= minNumberOfPointsToAllow;
                        bool conditionStillHavePoorFitCalculated = fitMetrics.RSquared < LM_RsquaredCuttoff && fitMetrics.DidConverge == true;

                        //if (fitMetrics.RSquared < LM_RsquaredCuttoff && fitMetrics.DidConverge == true || fitMetrics.DidConverge == false & possibleEicSinglePeak.Count >= minNumberOfPointsToAllow) //bad fits we should integrate raw data to get area// convergence is required on all fit scores

                        if (conditionStillHavePoorFitCalculated || conditionDidNotConvergeButHasOfEnoughPoints) //bad fits we should integrate raw data to get area// convergence is required on all fit scores
                        {
                            Console.WriteLine("!!!!!!!!!!!We Have a bad fit.  Do something.  In this case, we return the origional data.  Perhaps we need a better guess");

                            //cut points, re test, recheck, cut again

                            //4. quantitate?

                            #region this is where we calculate the area under the LC curve.  we integrate under the model.  we also iterate by removing points till it fits.

                            Console.WriteLine(Environment.NewLine + "Test multi seed fit with less points till it fits...");
                            double[] coefficents2;
                            
                            //areaUnderCurve = CutAndRefitCurve(ref clippedFitXyData, ref errorLog, minNumberOfPointsToAllow, LM_RsquaredCuttoff, possibleEicSinglePeak, ref fitMetrics, out scanRangeFit, out coefficents2);//1-9-2014
                            areaUnderCurve = CutAndRefitCurve(ref clippedXYData, ref errorLog, minNumberOfPointsToAllow, LM_RsquaredCuttoff, possibleEicSinglePeak, ref fitMetrics, out scanRangeFit, out coefficents2, out clippedFitXyData);
                            Console.WriteLine(Environment.NewLine + "After multi seeding and removing low abundance points, Did we converge? " + fitMetrics.DidConverge);

                            result.ScanBoundsInfo.Start = scanRangeFit.Start;
                            result.ScanBoundsInfo.Stop = scanRangeFit.Stop;
                            result.CorrelationCoefficients = coefficents2;
                            if (fitMetrics != null && fitMetrics.DidConverge==true)
                            {
                                
                                result.LMOptimizationCorrelationRsquared = fitMetrics.RSquared;
                            }
                            else
                            {
                                //we could not fit under any condiitiosn, just add up raw data points and null coefficients
                                result.CorrelationCoefficients = null;//this needs to prevent correlations
                                result.LMOptimizationCorrelationRsquared = -1;//will this kill it?
                                areaUnderCurve = 0;
                                foreach (var points in clippedXYData)
                                {
                                    areaUnderCurve += points.Y;
                                }
                            }

                            #endregion
                        }

                        //5.  peak detect fit data.  this should return 1 peak
                        //_omicsPeakDetection.DiscoverPeaks(clippedFitXyData);
                        fitLcPeaksWithArea = LcProcessor.Execute(clippedFitXyData,EnumerationChromatogramProcessing.LCPeakDetectOnly);//there should be one peak at this point

                        if (fitLcPeaksWithArea.Count == 1)
                        {
                            #region this is where we assign the area under the LC curve
                            
                            fitLcPeaksWithArea[0].Feature = new MSFeatureLight();

                            fitLcPeaksWithArea[0].Feature.Abundance = Convert.ToInt64(areaUnderCurve);//Integrated area under fit LC curve
                            areaForTesting = Convert.ToInt64(areaUnderCurve);//Integrated area under fit LC curve
                            
                            fitLcPeaksWithArea[0].Feature.Scan = Convert.ToInt32(Math.Round(fitLcPeaksWithArea[0].XValue, 0));

                            #endregion
                        }
                    //}
              //      else
              //      {
              //          clippedFitXyData = null;
              //          fitLcPeaksWithArea = null;
              //      }
                }
                else
                {
                    clippedFitXyData = null;
                    fitLcPeaksWithArea = null;
                    MakeSignPostForTrue(test1, "Not enough points across a single point.  Fit will fail", "SetScanRange", ref errorLog);
                }
            }
            else
            {
                clippedFitXyData = null;
                fitLcPeaksWithArea = null;
                MakeSignPostForTrue(test1, "Scan less than 0", "SetScanRange", ref errorLog);
            }

            return fitLcPeaksWithArea;
        }

        private static SolverReport FitWith2SeedXValues(double LM_RsquaredCuttoff, out List<XYData> clippedFitXyData, ref Tuple<string, string> errorLog, List<XYData> possibleEicSinglePeak, out ScanObject scanRangeFit,out List<XYData> modeledPeakList, out double areaUnderCurve, out double[] coefficents)
        {
            SolverReport fitMetrics;
            //double areaUnderCurve;

            //List<XYData> modeledPeakList;
            int numberOfSamples = 100;
            int sampleWidth = 1;
            bool centerXAxisOnInteger = true;
            //double[] coefficents;

            //guess coeffiecients here.  the seex X here is in the middle of the data
            coefficents = new double[3];
            coefficents[0] = 2; //sigma
            coefficents[1] = possibleEicSinglePeak.Max(r => r.Y); //height
            //guess X by mid point of curve
            double center = possibleEicSinglePeak.Count/2;
            int possibleCenter = Convert.ToInt32(Math.Truncate(center));
            coefficents[2] = possibleEicSinglePeak[possibleCenter].X; //m/z

            Console.WriteLine("Curve fitting peak in EIC");

            try//try fit since it is an external call
            {
                // Uncomment to debug: Console.WriteLine("      Fit Seed 1");
                clippedFitXyData = CurveFit.Fit_LM(possibleEicSinglePeak, out fitMetrics, out areaUnderCurve, ref coefficents, out modeledPeakList, numberOfSamples, sampleWidth, ref errorLog, out scanRangeFit, EnumerationCurveType.Gaussian, centerXAxisOnInteger);

                if (fitMetrics == null)
                {
                    fitMetrics = new SolverReport(new alglib.lsfitreport(), false);
                }
                // Uncomment to debug: Console.WriteLine("      Fit Seed 1 converged? " + fitMetrics.DidConverge);

                //try different seed incase we missed the peak.  the seed we check here is x at max y
                //if we ahve a good score + peaks + converge = true, no need to reseed 1-9-2014
                //or if it failed and we have peaks, we can reeseed
                if (fitMetrics.RSquared < LM_RsquaredCuttoff && possibleEicSinglePeak.Count > 0 &&
                    fitMetrics.DidConverge == true || fitMetrics.DidConverge == false && possibleEicSinglePeak.Count > 0)
                    //if fit is already good, don't refit.  we need possibleEicSinglePeak points present inorder to refit OR if it did not converge and we have peaks, procede with seed 2
                {
                    double initialIntensity = possibleEicSinglePeak[0].Y;
                    coefficents[2] = possibleEicSinglePeak[0].X; //m/z
                    foreach (var dataPoint in possibleEicSinglePeak)
                    {
                        if (dataPoint.Y > initialIntensity)
                        {
                            coefficents[2] = dataPoint.X; //m/z
                            initialIntensity = dataPoint.Y;
                        }
                    }

                    // Uncomment to debug: Console.WriteLine("      Fit Seed 2");
                    SolverReport fitMetricsSeed2;
                    clippedFitXyData = CurveFit.Fit_LM(possibleEicSinglePeak, out fitMetricsSeed2, out areaUnderCurve, ref coefficents, out modeledPeakList, numberOfSamples, sampleWidth, ref errorLog, out scanRangeFit, EnumerationCurveType.Gaussian, centerXAxisOnInteger);
                    if (fitMetricsSeed2 == null)
                    {
                        fitMetricsSeed2 = new SolverReport(new alglib.lsfitreport(), false);
                    }
                    // Uncomment to debug: Console.WriteLine("      Fit Seed 2 converged? " + fitMetricsSeed2.DidConverge);

                    if (fitMetricsSeed2.DidConverge == true && fitMetricsSeed2.RSquared > fitMetrics.RSquared)
                    {
                        fitMetrics = fitMetricsSeed2;
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: failed fit, " + ex.Message);                
                throw;
            }

            return fitMetrics;//this will have a true or false for convergence. it has 2 chances to converge
        }

        //TODO this is very similar to ClipFitAndCalculateArea
        private static double CutAndRefitCurve(ref List<PNNLOmics.Data.XYData> XYDataToFit, ref Tuple<string, string> errorLog, int minNumberOfPointsToAllow, double LM_RsquaredCuttoff, List<PNNLOmics.Data.XYData> possibleFragmentTargetEicSinglePeak, ref SolverReport fitMetrics, out ScanObject scanRangeFit, out double[] coefficents, out List<PNNLOmics.Data.XYData> fitXYDataForReturn)
        {
            double area = 0;
            scanRangeFit = new ScanObject(0, 0);
            coefficents = new double[0];//need guess here
            fitXYDataForReturn = new List<XYData>();

            if (XYDataToFit.Count > minNumberOfPointsToAllow)
            {
                int iterationCount = 0;
                List<XYData> possibleFragmentTargetEicSinglePeakLowestRemoved = possibleFragmentTargetEicSinglePeak;

                bool continueLoop = true;
                bool conditionOfEnoughPointsRemaining = possibleFragmentTargetEicSinglePeakLowestRemoved.Count > minNumberOfPointsToAllow;
                bool conditionStillHavePoorFitCalculated = fitMetrics.RSquared < LM_RsquaredCuttoff && fitMetrics.DidConverge == true;
                bool conditionRsquaredFailedAndIsOne = fitMetrics.RSquared == 1;//this is not needed?
                
                //while (possibleFragmentTargetEicSinglePeakLowestRemoved.Count > minNumberOfPointsToAllow && fitMetrics.RSquared < LM_RsquaredCuttoff && fitMetrics.DidConverge==true || fitMetrics.RSquared == 1)//pre 1-9-2014
                while (continueLoop)
                {
                    //this works well on man5 1820_1
                    //remove lowest
                    // Uncomment to debug: Console.WriteLine("*****TryAgain***** " + XYDataToFit.Count + " point  " + fitMetrics.RSquared + " rsquared/" + LM_RsquaredCuttoff);

                    //repeat with lowest point from one end removed
                    possibleFragmentTargetEicSinglePeakLowestRemoved = RemoveLowestAbundanced(possibleFragmentTargetEicSinglePeakLowestRemoved);
                    
                    
                    
                    //try with multiple seeds.  one is the center of the range and the other is the most abundant
                    List<XYData> modeledPeakList;
                    //fitMetrics = FitWith2SeedXValues(LM_RsquaredCuttoff, out fitXYData, ref errorLog, possibleFragmentTargetEicSinglePeakLowestRemoved, out scanRangeFit, out modeledPeakList, out area, out coefficents);
                    fitMetrics = FitWith2SeedXValues(LM_RsquaredCuttoff, out XYDataToFit, ref errorLog, possibleFragmentTargetEicSinglePeakLowestRemoved, out scanRangeFit, out modeledPeakList, out area, out coefficents);

                    //TODO what we need to do is attach the correlation constants to the target so we can evaluate in the corelator
                    //bool constantPointsInFit = true;//false passes old unit test 2/2
                    //if (constantPointsInFit) XYDataToFit = modeledPeakList;

                    // Uncomment to debug: Console.WriteLine(iterationCount);

                    iterationCount++;
                    //if (fitXYData.Count < minNumberOfPointsToAllow)
                    //{
                    //    Console.WriteLine("BreakOut");
                    //    break;
                    //}

                    conditionOfEnoughPointsRemaining = possibleFragmentTargetEicSinglePeakLowestRemoved.Count > minNumberOfPointsToAllow;
                    conditionStillHavePoorFitCalculated = fitMetrics.RSquared < LM_RsquaredCuttoff && fitMetrics.DidConverge == true;
                    conditionRsquaredFailedAndIsOne = fitMetrics.RSquared == 1;

                    if (conditionOfEnoughPointsRemaining && conditionStillHavePoorFitCalculated || conditionRsquaredFailedAndIsOne)
                    {
                        continueLoop = true;
                    }
                    else
                    {
                        continueLoop = false;

                        bool constantPointsInFit = true;//false passes old unit test 2/2
                        if (constantPointsInFit) fitXYDataForReturn = modeledPeakList; ;
                        
                        // Uncomment to debug: Console.WriteLine("BreakOut");
                    }
                }
                if (XYDataToFit.Count < minNumberOfPointsToAllow)
                {
                    // Uncomment to debug: Console.WriteLine("Failed to Converge");
                    XYDataToFit = possibleFragmentTargetEicSinglePeak;
                }
            }
            else
            {
                XYDataToFit = possibleFragmentTargetEicSinglePeak;
            }

            return area;
        }

        public static List<ProcessedPeak> NoFitSoClipAndIterate(List<ProcessedPeak> peakListFromFitData, List<XYData> dataXY, ref FragmentIQTarget target, double minFitForAcceptableCurveLM, ref List<ProcessedPeak> possibleFragmentPeaksInEIC, ref List<XYData> singlePeakFit, ref FragmentResultsObjectHolderIq results, ref Processors.ProcessorChromatogram LcProcessor, ref Tuple<string, string> errorLog)
        {
            
            //this loop is designed to clip the lowest points (one at a time) and then refit the data.  THe hope is that by removing points at low abundacne, the peak top will fit properly
            if (peakListFromFitData != null && peakListFromFitData.Count == 0) //if no fit was found
            {
                //if no fit was found
                //scanRangeFit = new ScanObject(0, 0);
                Console.WriteLine("*****Try*****  Clip till we get a 3 point peak top");
                int tests = dataXY.Count - 3;//-3 so there are always 3 points
                for (int test = 0; test < tests; test++)
                {
                    //Console.WriteLine("*****TryAgain*****  Clip till we get a 3 point peak top");

                    //repeat with lowest point from one end removed
                    List<XYData> possibleFragmentTargetEicClippedLowestRemoved = RemoveLowestAbundanced(dataXY);

                    WriteXYData(possibleFragmentTargetEicClippedLowestRemoved, ref errorLog);

                    dataXY = possibleFragmentTargetEicClippedLowestRemoved;

                    //repick peaks from non fit data
                    
                    List<ProcessedPeak> repickPeaks = LcProcessor.Execute(possibleFragmentTargetEicClippedLowestRemoved, EnumerationChromatogramProcessing.LCPeakDetectOnly);//this is because the index changes for the lowest minima and maxima
                    
                    double areaForTesting = 0;
                    if (repickPeaks.Count > 0)
                    {
                        peakListFromFitData = Utiliites.ClipFitAndCalculateArea(possibleFragmentTargetEicClippedLowestRemoved, repickPeaks[0], minFitForAcceptableCurveLM, out singlePeakFit, ref results, ref LcProcessor, ref errorLog, out areaForTesting);
                    }
                    //TestAgainWithLessPoints(ref possibleFragmentTargetEicClipped, ref scanStart, ref possibleFragmentPeaksInEIC, ref scanStop, ref possibleFragmentTargetEicSinglePeakFit, ref defaultBaseTargetEicSinglePeakFitPeaks);

                    if (peakListFromFitData.Count > 0)
                    {
                        Console.WriteLine("Freedom!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        break; //by shortening the data, we fit a curve
                    }
                    //Console.WriteLine("try again..." + test + " of " + tests);
                }


                //if we get here the loop has ended and we have not found a better solution
                
                Console.WriteLine("Trying Failed");
            }
            else
            {
                //keep origional values
            }
            return peakListFromFitData;
        }

        public static bool FindCommonStartStopBetweenCurves(List<XYData> possibleFragmentTargetEicSinglePeakFit, ProcessedPeak fragmentCandiate, List<XYData> parentEicSinglePeakFit, ProcessedPeak parentCandiate, ref ScanObject scans, ref Tuple<string, string> errorLog)
        {
            scans.Start = SetCommonStartScanFromProcessedPeak(possibleFragmentTargetEicSinglePeakFit, fragmentCandiate, parentEicSinglePeakFit, parentCandiate);
            scans.Stop = SetCommonStopScanFromProcessedPeak(possibleFragmentTargetEicSinglePeakFit, fragmentCandiate, parentEicSinglePeakFit, parentCandiate);

            bool test1 = SignPostRequire(scans.Start < scans.Stop, "Scans are in the correct order");

            if (test1 == true)
            {
                scans.Start = SetCommonStartScanFromProcessedPeak(possibleFragmentTargetEicSinglePeakFit, fragmentCandiate, parentEicSinglePeakFit, parentCandiate);
                scans.Stop = SetCommonStopScanFromProcessedPeak(possibleFragmentTargetEicSinglePeakFit, fragmentCandiate, parentEicSinglePeakFit, parentCandiate);
            }
            else
            {
                MakeSignPostForTrue(test1, "Scans are not in the correct order", "FindCommonStartStopBetweenCurves", ref errorLog);
            }
            return test1;
        }

        public static bool FindCommonStartStopBetweenCurves(ScanObject scanSet1, ScanObject scanSet2, ref int startScan, ref int stopScan, ref Tuple<string, string> errorLog)
        {
            startScan = SetCommonStartScanFromProcessedPeak(scanSet1, scanSet2);
            stopScan = SetCommonStopScanFromProcessedPeak(scanSet1, scanSet2);

            bool test1 = SignPostRequire(startScan < stopScan, "Scans are in the correct order");

            if (test1 == true)
            {
                startScan = SetCommonStartScanFromProcessedPeak(scanSet1, scanSet2);
                stopScan = SetCommonStopScanFromProcessedPeak(scanSet1, scanSet2);
            }
            else
            {
                MakeSignPostForTrue(test1, "Scans are not in the correct order", "FindCommonStartStopBetweenCurves", ref errorLog);
            }
            return test1;
        }

        private static int SetCommonStopScanFromProcessedPeak(List<XYData> dataset1, ProcessedPeak peakFromDataset1, List<XYData> dataset2, ProcessedPeak peakFromDataset2)
        {
            int localStopScan = Math.Min(
                Convert.ToInt32(dataset1[peakFromDataset1.MinimaOfHigherMassIndex].X),
                Convert.ToInt32(dataset2[peakFromDataset2.MinimaOfHigherMassIndex].X));
            return localStopScan;
        }

        private static int SetCommonStartScanFromProcessedPeak(List<XYData> dataset1, ProcessedPeak peakFromDataset1, List<XYData> dataset2, ProcessedPeak peakFromDataset2)
        {
            int localStartscan = Math.Max(
                Convert.ToInt32(dataset1[peakFromDataset1.MinimaOfLowerMassIndex].X),
                Convert.ToInt32(dataset2[peakFromDataset2.MinimaOfLowerMassIndex].X));
            return localStartscan;
        }

        private static int SetCommonStopScanFromProcessedPeak(ScanObject scanSet1, ScanObject scanSet2)
        {
            return Math.Max(scanSet1.Stop, scanSet2.Stop);//we want the max here because it is the best bounds for the correlation.  taking the smaller would remove data.  We are interpolating.  Takiing the max means that one of the sets has non zero values above 10th height
        }

        private static int SetCommonStartScanFromProcessedPeak(ScanObject scanSet1, ScanObject scanSet2)
        {
            return Math.Min(scanSet1.Start, scanSet2.Start);//we want the min here because it is the best bounds for the correlation.  taking the larger would remove data.  We are interpolating.  Takiing the max means that one of the sets has non zero values above 10th height
        }

       

        private static List<XYData> RemoveLowestAbundanced(List<XYData> data)
        {
            List<XYData> dataLowestRemoved = new List<XYData>();
            if (data[0].Y < data[data.Count - 1].Y)
            {
                for (int i = 1; i < data.Count; i++) //-1 so we don't over run
                {
                    dataLowestRemoved.Add(data[i]);
                }
            }
            else
            {
                for (int i = 0; i < data.Count - 1; i++) //-2 so we stop 1 early
                {
                    dataLowestRemoved.Add(data[i]);
                }
            }
            return dataLowestRemoved;
        }

        public static void WriteXYData(List<PNNLOmics.Data.XYData> xyDataIn, ref Tuple<string, string> errorLog)
        {
            bool test1 = SignPostRequire(xyDataIn != null, "XYdataList is null");//perhaps more

            List<PNNLOmics.Data.XYData> loadPossibleEicTruncated = new List<XYData>();
            if (test1)
            {
                foreach (var xyData in xyDataIn)
                {
                    Console.WriteLine("XYData\t" + xyData.X + "\t" + xyData.Y);
                }
            }
            else
            {

                MakeSignPostForTrue(!test1, "Not enough data points or array is null", "WriteXYData", ref errorLog);
            }

        }

        public static void WriteXYData(List<ProcessedPeak> processedDataIn, ref Tuple<string, string> errorLog)
        {
            bool test1 = SignPostRequire(processedDataIn != null, "XYdataList is null");//perhaps more

            List<PNNLOmics.Data.XYData> loadPossibleEicTruncated = new List<XYData>();
            if (test1)
            {
                foreach (var xyData in processedDataIn)
                {
                    Console.WriteLine("XYData\t" + xyData.XValue + "\t" + xyData.Height);
                }
            }
            else
            {

                MakeSignPostForTrue(!test1, "Not enough data points or array is null", "WriteXYData", ref errorLog);
            }

        }

        public static List<Peak> ConvertDeconPeakToPeakForIQ(List<ChromPeak> deconData)
        {
            List<Peak> outputDeconResults = new List<Run64.Backend.Core.Peak>();
            foreach (var peak in deconData)
            {
                if (peak.Width > 0)
                    //processedPeaks.SignalToNoise is the Local Lowest Min To Height or the signal to highest shoulder
                {
                    outputDeconResults.Add((Run64.Backend.Core.Peak)peak);
                }
                else
                {
                    Console.WriteLine("Fail");
                }
            }
            return outputDeconResults;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="deconXYData"></param>
        /// <param name="averageOddNumberOfPoints">3,5,7 ect</param>
        /// <returns></returns>
        public static Run64.Backend.Data.XYData MovingAverage(Run64.Backend.Data.XYData deconXYData, int averageOddNumberOfPoints)
        {
            int deconXYDatalength = deconXYData.Xvalues.Length;
            double[] yValues = deconXYData.Yvalues;

            var newdeconXYData = new Run64.Backend.Data.XYData();
            newdeconXYData.Xvalues = deconXYData.Xvalues;
            newdeconXYData.Yvalues = new double[deconXYDatalength];
            double[] averagedYValues = newdeconXYData.Yvalues;
            int halfAverage = Convert.ToInt32((averageOddNumberOfPoints - 1)/2);
            

            //skip first and last points
            double sum = 0;
            int index = 0;

            //before averaging
            for (int i = 0; i < halfAverage;i++ )
            {
                averagedYValues[i] = yValues[i];
            }

            for (int i = halfAverage; i < deconXYDatalength - halfAverage; i++)
            {
                sum = 0;
                for (int j = -halfAverage; j < halfAverage+1; j++)//-1, 0, +1 etc.
                {
                    index = i + j;
                    sum += yValues[index];
                }
                averagedYValues[i] = sum / averageOddNumberOfPoints;
            }

            //after averaging
            for (int i = deconXYDatalength - halfAverage; i < deconXYDatalength; i++)
            {
                averagedYValues[i] = yValues[i];
            }

            return newdeconXYData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deconXYData"></param>
        /// <param name="averageOddNumberOfPoints">3,5,7 ect</param>
        /// <returns></returns>
        public static List<PNNLOmics.Data.XYData> MovingAverage(List<PNNLOmics.Data.XYData> deconXYData, int averageOddNumberOfPoints)
        {
            if(averageOddNumberOfPoints==0)
            {
                return deconXYData;
            }

            int deconXYDatalength = deconXYData.Count;
            List<double> yValues = new List<double>();

            List<XYData> averagedYValues = new List<XYData>();
            foreach (XYData xyData in deconXYData)
            {
                averagedYValues.Add(new XYData(xyData.X, 0));
                yValues.Add(xyData.Y);
            }
            //newdeconXYData.Xvalues = deconXYData.Xvalues;
            //newdeconXYData.Yvalues = new double[deconXYDatalength];
            //List<double>averagedYValues = new List<double>();

            int halfAverage = Convert.ToInt32((averageOddNumberOfPoints - 1) / 2);

            

            //skip first and last points
            double sum = 0;
            int index = 0;

            //before averaging
            for (int i = 0; i < halfAverage; i++)
            {
                averagedYValues[i].Y = yValues[i];
            }

            for (int i = halfAverage; i < deconXYDatalength - halfAverage; i++)
            {
                sum = 0;
                for (int j = -halfAverage; j < halfAverage + 1; j++)//-1, 0, +1 etc.
                {
                    index = i + j;
                    sum += yValues[index];
                }
                averagedYValues[i].Y = sum / averageOddNumberOfPoints;
            }

            //after averaging
            for (int i = deconXYDatalength - halfAverage; i < deconXYDatalength; i++)
            {
                averagedYValues[i].Y = yValues[i];
            }

            return averagedYValues;
        }


        public static List<ProcessedPeak> FilterByPointsPerPeak(List<ProcessedPeak> peaks, int minNumberOfPoints)
        {
            List<ProcessedPeak> filteredPeakList = new List<ProcessedPeak>();
            foreach (var processedPeak in peaks)
            {
                if (processedPeak.MinimaOfHigherMassIndex - processedPeak.MinimaOfLowerMassIndex > minNumberOfPoints)
                    filteredPeakList.Add(processedPeak);
            }

            return filteredPeakList;
        }

        public static List<ProcessedPeak> FilterByPointsPerSide(List<ProcessedPeak> peaks, int minNumberOfPointsPerSide)
        {
            int asymetryFactor = 2;
            
            List<ProcessedPeak> filteredPeakList = new List<ProcessedPeak>();
            foreach (var processedPeak in peaks)
            {
                int leftPoints = processedPeak.CenterIndexLeft - processedPeak.MinimaOfLowerMassIndex + 1;//We need to add one so we include centerIndexLeft
                int rightPoints = processedPeak.MinimaOfHigherMassIndex - (processedPeak.CenterIndexLeft + 1) + 1;//we need to add one so we include centerIndexRight (centerindexLeft+1)

                int shortestSide = Math.Min(leftPoints, rightPoints);

                if (shortestSide >= minNumberOfPointsPerSide)
                {
                    filteredPeakList.Add(processedPeak);
                }
                else
                {
                    //allow for shoulder peaks to have one less point if other side has one more point
                    int longestSide = Math.Max(leftPoints, rightPoints);
                    //if (longestSide >= minNumberOfPointsPerSide + asymetryFactor && shortestSide >= minNumberOfPointsPerSide - asymetryFactor)//this is the old way
                    if (longestSide - minNumberOfPointsPerSide <= asymetryFactor && minNumberOfPointsPerSide - shortestSide <= minNumberOfPointsPerSide)//deviations on both sides need to be less than the asymetry factor//TODO this is correct!  8-16-2013
                    {
                        filteredPeakList.Add(processedPeak);
                    }
                }
            }

            return filteredPeakList;
        }

        public static List<Run64.Backend.Core.Peak> FilterByPointsPerPeak(IqResult result, int minNumberOfPoints)
        {
            var chromPeakList = new List<Run64.Backend.Core.Peak>();
            if (result.IqResultDetail.Chromatogram != null && result.IqResultDetail.Chromatogram.Xvalues.Length > 2)
            {
                //int minNumberOfPoints = 7;
                int scanSpacing = Convert.ToInt32(result.IqResultDetail.Chromatogram.Xvalues[1] - result.IqResultDetail.Chromatogram.Xvalues[0]);
                chromPeakList = Utiliites.FilterChromatogramPeaksByWidth(result.ChromPeakList, scanSpacing, minNumberOfPoints);
            }
            else
            {
                int scanSpacing = 1;
                chromPeakList = Utiliites.FilterChromatogramPeaksByWidth(result.ChromPeakList, scanSpacing, minNumberOfPoints);
            }
            return chromPeakList;
        }

        public static List<Run64.Backend.Core.Peak> FilterChromatogramPeaksByWidth(List<Run64.Backend.Core.Peak> deconPeaks, int scanSpacing, int minNumberOfPoints)
        {
            bool test1 = SignPostRequire(deconPeaks != null, "XYdataList is null");//perhaps more


            var outputResults = new List<Run64.Backend.Core.Peak>();
            if (test1)
            {
                foreach (var peak in deconPeaks)
                {
                    if (peak.Width >= scanSpacing * minNumberOfPoints)
                    {
                        outputResults.Add((Run64.Backend.Core.Peak)peak);
                    }
                    else
                    {
                        Console.WriteLine("Fail");
                    }
                }

            }
            else
            {
                outputResults = new List<Run64.Backend.Core.Peak>();
            }

            return outputResults;
        }

        public static List<int> ConvertStringGlycanCodeToIntegers54000(string code)
        {
            //Hex:HexNAc:Fucose:SialicAcid
            //HH:N:F:SS  102124
            
            List<int> compositions = new List<int>();

            int codeAsInt = Convert.ToInt32(code);
            double codeAsDouble = Convert.ToDouble(codeAsInt);

            int preHexoseFactor = 10000;
            double preHexose = codeAsDouble / preHexoseFactor;

            int hexose =  Convert.ToInt32(Math.Truncate(preHexose));
            compositions.Add(hexose);

            int preHexNAcFactor = 1000;
            double preHexNAc = (codeAsInt - hexose * preHexoseFactor) / preHexNAcFactor;

            int hexNAc = Convert.ToInt32(Math.Truncate(preHexNAc));

            compositions.Add(hexNAc);

            int preFucoseFactor = 100;
            double preFucose = (codeAsInt - hexose * preHexoseFactor - hexNAc * preHexNAcFactor) / preFucoseFactor;

            int fucose = Convert.ToInt32(Math.Truncate(preFucose));
            compositions.Add(fucose);

            int preSialicAcidFactor = 1;
            double preSialicAcid = (codeAsInt - hexose * preHexoseFactor - hexNAc * preHexNAcFactor - fucose * preFucoseFactor) / preSialicAcidFactor;

            int sialicAcid = Convert.ToInt32(Math.Truncate(preSialicAcid));
            compositions.Add(sialicAcid);

            return compositions;
        }

        public static List<int> ConvertStringGlycanCodeToIntegers(string code)
        {
            //Hex-HexNAc-Fucose-SialicAcid-Lactose
            //H-N-F-S-L 5-4-2-2-1

            List<int> compositions = new List<int>();
            char splitter = '-';
            string[] letters = code.Split(splitter);

            if(letters.Length==1)
            {
                Console.WriteLine("We are missing the - in the nomenclature encountered by ConvertStringGlycanCodeToIntegers");
                System.Threading.Thread.Sleep(3000);
            }

            foreach(string number in letters)
            compositions.Add(Convert.ToInt32(number));

            return compositions;
        }

        #region old iQ
        //public static List<PNNLOmics.Data.XYData> ReWindowDataList(FragmentTarget target, List<PNNLOmics.Data.XYData> data, int bonuseScan, ref Tuple<string, string> errorLog)
        //{
        //    bool test1 = SignPostRequire(data != null && data.Count > 2, "XYdataList is null");//perhaps more

        //    List<PNNLOmics.Data.XYData> loadPossibleEicTruncated = new List<XYData>();
        //    if (test1)
        //    {
        //        loadPossibleEicTruncated = ReWindowDataList(target.StartScan, target.StopScan, data, bonuseScan);
        //    }
        //    else
        //    {
        //        loadPossibleEicTruncated = null;
        //        MakeSignPostForTrue(!test1, "Not enough data points or array is null", "ReWindowDataList", ref errorLog);
        //    }

        //    return loadPossibleEicTruncated;
        //}
        #endregion



       

        //public static void SetScanRangeFromProcessedPeak(List<XYData> data, ProcessedPeak processedPeak, ref int scanStart, ref int scanStop)
        public static void SetScanRangeFromProcessedPeak(List<XYData> data, ProcessedPeak processedPeak, ref ScanObject scanInfo)
        {
            scanInfo.Start = Convert.ToInt32(data[processedPeak.MinimaOfLowerMassIndex].X);
            scanInfo.Stop = Convert.ToInt32(data[processedPeak.MinimaOfHigherMassIndex].X);
        }

        //public static void SetNewScanStartStopOmics(List<XYData> chargedTargetEic, int scanStartPossibiilty, int scanStopPossibiilty, FragmentTarget chargedTarget, out int newScanStart, out int newScanStop, bool print, ref IQGlyQ.Processors.ProcessorChromatogram _LcProcessor, ref Tuple<string, string> errorLog)
        //{
        //    bool test1 = SignPostRequire(chargedTargetEic != null, "chargedTargetEic is null");//perhaps more

        //    newScanStart = 0;
        //    newScanStop = 0;

        //    if (test1)
        //    {
        //        List<ProcessedPeak> chargedTargetPeaks = _LcProcessor.Execute(chargedTargetEic, EnumerationChromatogramProcessing.LCPeakDetectOnly);
                
        //        List<ProcessedPeak> chargedTargetClosestPeak = (from peak in chargedTargetPeaks where peak.XValue >= scanStartPossibiilty && peak.XValue <= scanStopPossibiilty select peak).ToList();


        //        if (chargedTargetClosestPeak.Count == 1)
        //        {
        //            newScanStart = Convert.ToInt32(chargedTargetEic[chargedTargetClosestPeak[0].MinimaOfLowerMassIndex].X);
        //            newScanStop = Convert.ToInt32(chargedTargetEic[chargedTargetClosestPeak[0].MinimaOfHigherMassIndex].X);
        //        }
        //        else
        //        {
        //            int startX = chargedTarget.ScanLCTarget;
        //            int testX = Convert.ToInt32(chargedTargetClosestPeak[0].XValue);
        //            int differences = (testX - startX) * (testX - startX);

        //            ProcessedPeak selectedPeak = chargedTargetClosestPeak[0];
        //            foreach (var processedPeak in chargedTargetClosestPeak)
        //            {
        //                testX = Convert.ToInt32(processedPeak.XValue);
        //                int tempDiffernce = (testX - startX) * (testX - startX);
        //                if (tempDiffernce <= differences)
        //                {
        //                    differences = tempDiffernce;
        //                    selectedPeak = processedPeak;
        //                }
        //            }

        //            newScanStart = Convert.ToInt32(chargedTargetEic[selectedPeak.MinimaOfLowerMassIndex].X);
        //            newScanStop = Convert.ToInt32(chargedTargetEic[selectedPeak.MinimaOfHigherMassIndex].X);
        //        }

        //        if (print) Console.WriteLine("the old scan Range is " + scanStartPossibiilty + "-" + scanStopPossibiilty);
        //        if (print) Console.WriteLine("the new scan Range is " + newScanStart + "-" + newScanStop);
        //    }
        //    else
        //    {
        //        MakeSignPostForTrue(test1, "Null XYData List", "SetNewScanStartStopOmics", ref errorLog);
        //    }
        //}

        /// <summary>
        /// find peak and use it to set the scan range
        /// </summary>
        /// <param name="chargedTargetEic"></param>
        /// <param name="scanStartPossibiilty"></param>
        /// <param name="scanStopPossibiilty"></param>
        /// <param name="chargedTarget"></param>
        /// <param name="newScanStart"></param>
        /// <param name="newScanStop"></param>
        /// <param name="print"></param>
        /// <param name="errorLog"></param>
        public static void SetNewScanStartStopOmicsIQ(List<XYData> chargedTargetEic, ref ScanObject scans, int centerScan, ref IQGlyQ.Processors.ProcessorChromatogram _lcProcessor, bool print, ref Tuple<string, string> errorLog)
        
        {
            bool test1 = SignPostRequire(chargedTargetEic != null, "chargedTargetEic is null");//perhaps more
            ScanObject localScans = scans;

            if (test1)
            {
                List<ProcessedPeak> chargedTargetPeaks = _lcProcessor.Execute(chargedTargetEic, EnumerationChromatogramProcessing.LCPeakDetectOnly);

                if (chargedTargetPeaks.Count > 0)
                {
                    
                    List<ProcessedPeak> chargedTargetClosestPeak = (from peak in chargedTargetPeaks where peak.XValue >= localScans.Start && peak.XValue <= localScans.Stop select peak).ToList();

                    if (chargedTargetClosestPeak.Count == 1)
                    {
                        

                        if (chargedTargetEic != null)
                        {
                            localScans.Start = Convert.ToInt32(chargedTargetEic[chargedTargetClosestPeak[0].MinimaOfLowerMassIndex].X);
                            localScans.Stop = Convert.ToInt32(chargedTargetEic[chargedTargetClosestPeak[0].MinimaOfHigherMassIndex].X);
                        }
                    }
                    else
                    {
                        //int startX = chargedTarget.ScanLCTarget;
                        int centerX = centerScan;//start at the center scan

                        int testX = Convert.ToInt32(chargedTargetClosestPeak[0].XValue);
                        int differences = (testX - centerX)*(testX - centerX);

                        ProcessedPeak selectedPeak = chargedTargetClosestPeak[0];
                        foreach (var processedPeak in chargedTargetClosestPeak)
                        {
                            testX = Convert.ToInt32(processedPeak.XValue);
                            int tempDiffernce = (testX - centerX)*(testX - centerX);
                            if (tempDiffernce <= differences)
                            {
                                differences = tempDiffernce;
                                selectedPeak = processedPeak;
                            }
                        }

                        
                        if (chargedTargetEic != null)
                        {
                            localScans.Start = Convert.ToInt32(chargedTargetEic[selectedPeak.MinimaOfLowerMassIndex].X);
                            localScans.Stop = Convert.ToInt32(chargedTargetEic[selectedPeak.MinimaOfHigherMassIndex].X);
                        }
                    }
                }
                else
                {
                    MakeSignPostForTrue(test1, "NoPeakDetected from EIC", "SetNewScanStartStopOmics", ref errorLog);
                }
                if (print) Console.WriteLine("the old scan Range is " + scans.Start + "-" + scans.Stop);
                //if (print) Console.WriteLine("the new scan Range is " + newScanStart + "-" + newScanStop);
            }
            else
            {
                MakeSignPostForTrue(test1, "Null XYData List", "SetNewScanStartStopOmics", ref errorLog);
            }
        }

        #region old IQ
        //public static List<FragmentTarget> GenerateListOfCandidateParentsTest(Run run, TargetedResultBase result, ChromPeakQualityData possibiity, int scanStartPossibiilty, int scanStopPossibiilty, ref List<FragmentTarget> Fragments, ref FragmentedTargetedWorkflowParameters _workflowParameters, ref JoshTheorFeatureGenerator _theorFeatureGen, ref PeakChromatogramGenerator _peakChromGen, ref SavitzkyGolaySmoother _smoother, ref Tuple<string, string> errorLog)
        //{
        //    //TODO we need to have a nice test here
            
        //    bool test1 = SignPostRequire(scanStartPossibiilty >0, "Scan less than 0");
        //    bool test2 = SignPostRequire(possibiity != null, "Must have target");
        //    bool test3 = SignPostRequire(Fragments.Count > 0, "Scan less than 0");


        //    List<FragmentTarget> chargedParentsToSearchForMass = new List<FragmentTarget>();

        //    if (test1 && test2 && test3)
        //    {

        //        chargedParentsToSearchForMass = GenerateListOfCandidateParents.Generate(run, result, possibiity, scanStartPossibiilty, scanStopPossibiilty, ref Fragments, ref _workflowParameters, ref _theorFeatureGen, ref _peakChromGen, ref _smoother,ref errorLog);
                
        //    }
        //    else
        //    {
        //        chargedParentsToSearchForMass = null;
        //        MakeSignPostForTrue(test1, "Scan less than 0", "GenerateListOfCandidateParentsTest", ref errorLog);
        //    }

        //    return chargedParentsToSearchForMass;
        //}
        #endregion

        public static FragmentIQTarget GenerateListOfCandidateParentsTestIQ(Run run, IqResult result, ChromPeakQualityData possibiity, ScanObject scans, EnumerationParentOrChild parentsOrChildren, ref FragmentedTargetedWorkflowParametersIQ _workflowParameters, ref IGenerateIsotopeProfile _theorFeatureGen, ref Tuple<string, string> errorLog, ref Processors.ProcessorChromatogram _lcProcessor,  ref TheoreticalIsotopicProfileWrapper monster)
        {

            //TODO we need to have a nice test here

            bool test1 = SignPostRequire(scans.Start > 0, "Scan less than 0");
            bool test2 = SignPostRequire(possibiity != null, "Must have target");
            bool test3 = SignPostRequire(_workflowParameters.FragmentsIq.Count > 0, "Scan less than 0");
            bool test4 = SignPostRequire(possibiity.IsotopicProfile != null, "Need a Isotope Profile");

            FragmentIQTarget chargedParentsToSearchForMass = new FragmentIQTarget();

            List<FragmentResultsObjectHolderIq> moreFailedParents = new List<FragmentResultsObjectHolderIq>();
            if (test1 && test2 && test3 && test4)
            {
                List<FragmentIQTarget> failedTargets;
                //List<FragmentResultsObjectHolderIQ> moreFailedParents;
                chargedParentsToSearchForMass = GenerateListOfCandidateParents.GenerateIQ(run, result, possibiity, scans, parentsOrChildren, out failedTargets, out moreFailedParents, ref _workflowParameters, ref _theorFeatureGen, ref errorLog, ref _lcProcessor, ref monster);

                MakeSignPostForTrue(chargedParentsToSearchForMass.HasChildren(), "Success", "GenerateListOfCandidateParents", ref errorLog);
            }
            else
            {
                chargedParentsToSearchForMass = null;
                MakeSignPostForTrue(test1, "Scan less than 0", "GenerateListOfCandidateParentsTest", ref errorLog);
            }

            //Console.WriteLine(moreFailedParents.Count + " were looked at");
            //foreach (var parent in moreFailedParents)
            //{
            //    Console.WriteLine(parent.Error);
            //}
            
            return chargedParentsToSearchForMass;
        }

        public static ScanSet ScanSetFromStartStop(Run runIn, ScanObject scanInfo)
        {
            ChromPeakUtilities utilities = new ChromPeakUtilities();

            Run64.Backend.Core.Peak chromPeakSelected = new Run64.Backend.Core.Peak();
            double start = scanInfo.Start;
            double stop = scanInfo.Stop;
            int centerscan = Convert.ToInt32(Math.Truncate(start + (stop-start)/2));

            chromPeakSelected.XValue = centerscan;
            if (chromPeakSelected.XValue <= scanInfo.Min)
            {
                chromPeakSelected.XValue = scanInfo.Min;
            }
            if (chromPeakSelected.XValue >= scanInfo.Max)
            {
                chromPeakSelected.XValue = scanInfo.Max;
            }
            chromPeakSelected.Width =  Convert.ToSingle(stop - start);
            ScanSet LCScanSetSelected = utilities.GetLCScanSetForChromPeak(chromPeakSelected, runIn, scanInfo.ScansToSum);

            return LCScanSetSelected;
        }

        public static ScanSet ScanSetFromCenterScan(Run runIn, int centerScan, int scansToSum)
        {
            ChromPeakUtilities utilities = new ChromPeakUtilities();

            Run64.Backend.Core.Peak chromPeakSelected = new Run64.Backend.Core.Peak();

            chromPeakSelected.XValue = centerScan;
            chromPeakSelected.Width = 1;
            ScanSet LCScanSetSelected = utilities.GetLCScanSetForChromPeak(chromPeakSelected, runIn, scansToSum);

            return LCScanSetSelected;
        }
    

        

        //public static List<FragmentTarget> VerifyByTargetedFeatureFindingTest(List<FragmentTarget> finalChargedParentsToSearchForMass, double fitScoreCutoff, TargetedResultBase Result, Run run, ref MSGenerator _msGenerator, ref IterativeTFF _msfeatureFinder, ref List<FragmentTarget> _futureTargets, ref IsotopicProfileFitScoreCalculator fitScoreCalculator, ref Tuple<string, string> errorLog)
        //{
        //    bool test1 = SignPostRequire(finalChargedParentsToSearchForMass!=null && finalChargedParentsToSearchForMass.Count > 0, "Scan less than 0");
        //    bool test2 = false;
        //    if (test1)
        //    {
        //        foreach (FragmentTarget fragmentTarget in finalChargedParentsToSearchForMass)
        //        {
        //            test2 = SignPostRequire(fragmentTarget.StartScan > 0, "Scan less than 0");
        //            if (test2 == false)
        //            {
        //                break;
        //            }
        //        }
        //    }
        //    List<FragmentTarget> finalChargedParentsToSearchForLC = new List<FragmentTarget>();
        //    if (test1 == true && test2==true)
        //    {
        //        finalChargedParentsToSearchForLC = VerifyByTargetedFeatureFinding.Verify(finalChargedParentsToSearchForMass, fitScoreCutoff, Result, run, ref _msGenerator, ref _msfeatureFinder, ref _futureTargets, ref fitScoreCalculator, ref errorLog);
        //    }
        //    else
        //    {
        //        finalChargedParentsToSearchForLC = null;
        //        MakeSignPostForTrue(test1, "Scan less than 0", "VerifyByTargetedFeatureFindingTest", ref errorLog); 
        //    }

        //    return finalChargedParentsToSearchForLC;
        //}

        //public static FragmentIQTarget VerifyByTargetedFeatureFindingTestIQ(List<FragmentIQTarget> finalChargedParentsToSearchForMass, double fitScoreCutoff, IqResult result, Run run, ref MSGenerator _msGenerator, ref IterativeTFF _msfeatureFinder, ref List<FragmentIQTarget> _futureTargets, ref IsotopicProfileFitScoreCalculator fitScoreCalculator, ref Tuple<string, string> errorLog)
        //public static FragmentIQTarget VerifyByTargetedFeatureFindingTestIQ(ref FragmentResultsObjectHolderIQ result, FragmentIQTarget finalChargedParentsToSearchForMass, double fitScoreCutoff, Run run, ref MSGenerator _msGenerator, ref IterativeTFF _msfeatureFinder, ref List<FragmentIQTarget> _futureTargets, ref IsotopicProfileFitScoreCalculator fitScoreCalculator, ref Tuple<string, string> errorLog)
        //public static List<IqGlyQResult> VerifyByTargetedFeatureFindingTestIQ(FragmentIQTarget finalChargedParentsToSearchForMass, double fitScoreCutoff, Run run, ref MSGenerator _msGenerator, ref IterativeTFF _msfeatureFinder, ref List<FragmentIQTarget> _futureTargets, ref Task fitScoreCalculator, ref Tuple<string, string> errorLog)
        public static List<IqGlyQResult> VerifyByTargetedFeatureFindingTestIQ(FragmentIQTarget finalChargedParentsToSearchForMass, double fitScoreCutoff, Run run, ref IterativeTFF _msfeatureFinder, ref List<FragmentIQTarget> _futureTargets, IsotopeParameters isoParameters, Processors.ProcessorMassSpectra msProcessor, double ppmError, ref Tuple<string, string> errorLog)
        {
            //bool test1 = SignPostRequire(finalChargedParentsToSearchForMass != null && finalChargedParentsToSearchForMass.Count > 0, "Scan less than 0");
            bool test1 = SignPostRequire(finalChargedParentsToSearchForMass != null && finalChargedParentsToSearchForMass.HasChildren()==true, "NoChildren");
            bool test2 = false;
            if (test1)
            {
                List<IqTarget> children = finalChargedParentsToSearchForMass.ChildTargets().ToList();
                foreach (FragmentIQTarget fragmentTarget in children)
                {
                    //test2 = SignPostRequire(fragmentTarget.StartScan > 0, "Scan less than 0");
                    test2 = SignPostRequire(fragmentTarget.ScanLCTarget > 0, "Scan less than 0");
                    if (test2 == false)
                    {
                        break;
                    }
                }
            }

            //FragmentIQTarget finalChargedParentsToSearchForLC = new FragmentIQTarget();
            List<IqGlyQResult> finalChargedParentsToSearchForLC = new List<IqGlyQResult>();
            if (test1 == true && test2 == true)
            {
                List<FragmentIQTarget> failedTargets;
                EnumerationError verifyError;
                //finalChargedParentsToSearchForLC = VerifyByTargetedFeatureFinding.VerifyIQ(finalChargedParentsToSearchForMass, fitScoreCutoff, run, out failedTargets, ref _msGenerator, ref _msfeatureFinder, ref _futureTargets, ref fitScoreCalculator, ref errorLog, out verifyError);
                finalChargedParentsToSearchForLC = VerifyByTargetedFeatureFinding.VerifyIq(finalChargedParentsToSearchForMass, fitScoreCutoff, run, out failedTargets, ref msProcessor, ref _msfeatureFinder, ref _futureTargets, ref errorLog, out verifyError, isoParameters, ppmError);
                
                //MakeSignPostForTrue(finalChargedParentsToSearchForLC.HasChildren(), "Success", "VerifyByTargetedFeatureFinding", ref errorLog);
                //MakeSignPostForTrue(finalChargedParentsToSearchForLC[0].DidThisWork, "Success", "VerifyByTargetedFeatureFinding", ref errorLog);
                MakeSignPostForTrue(finalChargedParentsToSearchForLC.Count>0, "Success", "VerifyByTargetedFeatureFinding", ref errorLog);
            }
            else
            {
                finalChargedParentsToSearchForLC = null;
                MakeSignPostForTrue(test1, "Scan less than 0", "VerifyByTargetedFeatureFindingTest", ref errorLog);
            }

            return finalChargedParentsToSearchForLC;
        }

        public static List<IqResult> UnfoldFeatureFinderResults(List<IqGlyQResult> finalChargedParentsFromAllFragmentsToSearchForLcResults)
        {
            List<IqResult> glycanChildren = new List<IqResult>();
            if (finalChargedParentsFromAllFragmentsToSearchForLcResults != null &&
                finalChargedParentsFromAllFragmentsToSearchForLcResults.Count > 0)
            {
                foreach (IqGlyQResult result in finalChargedParentsFromAllFragmentsToSearchForLcResults)
                {
                    List<IqResult> children = result.ChildResults().ToList();
                    foreach (var iqGlyQResult in children)
                    {
                        glycanChildren.Add(iqGlyQResult);
                    }
                }
            }
            return glycanChildren;
        }

        

        public static bool TestProcessedPeak(ProcessedPeak fragmentCandiateFitPeak, List<XYData> fragmentEicFitXyData, FragmentIQTarget possibleFragmentTarget, FragmentResultsObjectHolderIq result, double fitscoreCuttoff)
        {
            bool overallPass = false;
            if (fragmentCandiateFitPeak != null)
            {
                Console.WriteLine("The peak quality fragment at scan " + fragmentCandiateFitPeak.ScanNumber + " Passes");
                Console.WriteLine("and has a fit EIC of " + fragmentEicFitXyData.Count + " points");
                double maxIntensity = fragmentEicFitXyData.Max(r => r.Y);

                double fitScore = 1;
               
                bool test1 = possibleFragmentTarget.ChargeState > 0;
                bool test2 = fragmentEicFitXyData.Count > 0;
                bool test3 = maxIntensity > 0;
                bool test4 = fragmentCandiateFitPeak.Height > 0;

                bool test5 = false;
                if(result.CorrelationCoefficients!=null)
                {
                    test5 = result.CorrelationCoefficients.Length > 0;
                }
                
                
                bool test6 = result.Primary_Observed_IsotopeProfile !=null;
                bool test7 = true;
                bool test8 = true;
                bool test9 = true;
                if (test6)
                {
                    fitScore = result.Primary_Observed_IsotopeProfile.Score;

                    test7 = result.Primary_Observed_IsotopeProfile.MonoIsotopicMass > 0;

                    test8 = result.Primary_Observed_IsotopeProfile.MonoPeakMZ > 0;

                    test9 = fitScore <= fitscoreCuttoff;
                }
                if (test1 && test2 && test3 && test4 && test5 && test6 && test7 && test8 && test9)
                {
                    Console.WriteLine("+ Testing Peak...");
                    Console.WriteLine("  Fragment Peak Looks Good");
                    overallPass = true;
                }
                else
                {
                    Console.WriteLine("+ Testing Peak... Missing Something...");

                    WriteProblemToConsole(test1, test2, test3, test4, test5, test6, test7, test8, test9, fitScore);
                }
            }
            else
            {
                Console.WriteLine("+ Testing Peak... Missing Something...");
                Console.WriteLine("  No fragmentCandiateFitPeak to check");
            }
            return overallPass;
        }

        private static void WriteProblemToConsole(bool test1, bool test2, bool test3, bool test4, bool test5, bool test6, bool test7, bool test8, bool test9, double fitScore)
        {
            if (!test1)
            {
                Console.WriteLine("  ChargeState Problem");
            }
            if (!test2)
            {
                Console.WriteLine("  Peak problem in EIC");
            }
            if (!test3)
            {
                Console.WriteLine("  maxIntensity Problem");
            }
            if (!test4)
            {
                Console.WriteLine("  fragmentCandiateFitPeak.Height Problem");
            }
            if (!test5)
            {
                Console.WriteLine("  CorrelationCoefficients Problem");
            }
            if (!test6)
            {
                Console.WriteLine("  FragmentObservedIsotopeProfile missing");
            }
            if (!test7)
            {
                Console.WriteLine("  MonoIsotopicMass missing");
            }
            if (!test8)
            {
                Console.WriteLine("  MonoPeakMZ missing");
            }
            if (!test9)
            {
                Console.WriteLine("  FitScore is too high: " + fitScore);
            }
        }

        public static bool SignPostRequire(bool assertion, string message)
        {
            //Trace.Assert(assertion, "Precondition: " + message);

            if (assertion)
            {
                return true;//continue
            }
            else
            {
                return false;//break out
            }

            //if (UseExceptions)
            //{
            //    if (!assertion) throw new PreconditionException(message);

            //}
            //else
            //{
            //    Trace.Assert(assertion, "Precondition: " + message);
            //}
        }

        public static void MakeSignPostForTrue(bool assertion, string messageTrue, string location, ref Tuple<string, string> errorLog)
        {
            if (assertion) errorLog = new Tuple<string, string>(location, messageTrue); //break out
        }
    }
}
