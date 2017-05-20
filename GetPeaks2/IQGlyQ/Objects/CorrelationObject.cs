using System.Collections.Generic;
using System.Linq;
using GetPeaksDllLite.Functions;
using IQGlyQ.Enumerations;
using IQGlyQ.Processors;
using PNNLOmics.Data;
using PNNLOmics.Data.Peaks;
using Run32.Backend.Core;


namespace IQGlyQ.Objects
{
    public class CorrelationObject
    {
        /// <summary>
        /// index in peakList of most abundant isotope
        /// </summary>
        public int IndexMostAbundantPeak { get; set; }

        /// <summary>
        /// m/z value of most abundant peak in isotope profile
        /// </summary>
        public double BaseMZValue { get; set; }

        /// <summary>
        /// chromatogams of each isotope
        /// </summary>
        public List<Run32.Backend.Data.XYData> PeakChromXYData { get; set; }

        /// <summary>
        /// peaks associated with PeakChromXYData
        /// </summary>
        public List<List<ProcessedPeak>> PeaksFromChromXYData { get; set; }

        /// <summary>
        /// tollerance to find chromatograms in ppm
        /// </summary>
        public double ChromToleranceInPPM { get; set; }

        /// <summary>
        /// cuttoff for noise on EIC as relative to max value
        /// </summary>
        public double MinimumRelativeIntensityForChromCorr { get; set; }

        /// <summary>
        /// this needs to pass statistics
        /// </summary>
        public bool IsosPeakListIsOK { get; set; }

        /// <summary>
        /// checking that we have enough EIC for statistics
        /// </summary>
        public bool AreChromDataOK { get; set; }

        /// <summary>
        /// did this EIC pass the tests
        /// </summary>
        public List<bool> AcceptableChromList { get; set; }

        public double maxBaseIntensity { get; set; }

        public double minBaseIntensity { get; set; }


        public CorrelationObject()
        {
            //ChromToleranceInPPM = 10;//ppm
            //MinimumRelativeIntensityForChromCorr = 0.01;


        }

        public CorrelationObject(IsotopicProfile iso, ScanObject scans, Run run, ref ProcessorChromatogram _lcProcessor, FragmentedTargetedWorkflowParametersIQ workflowParameters)
        {
            //int startScan = scans.Start;
            //int stopScan = scans.Stop;

            //Processors.ChromatogramProcessingParameters LCparameters = new ChromatogramProcessingParameters();
            //Processors.ProcessorChromatogram LcProcessor = new ProcessorChromatogram(LCparameters);

            //ChromToleranceInPPM = chromToleranceInPPM;
            IndexMostAbundantPeak = iso.GetIndexOfMostIntensePeak();
            BaseMZValue = iso.Peaklist[IndexMostAbundantPeak].XValue;
            PeakChromXYData = new List<Run32.Backend.Data.XYData>();
            PeaksFromChromXYData = new List<List<ProcessedPeak>>();
            List<ProcessedPeak> peaksInEIC;
            Run32.Backend.Data.XYData clippedEIC;
            foreach (MSPeak peak in iso.Peaklist)
            {
                if (peak.Height > 0)//avoid penalty values TODO check SK 8-28
                {
                    //PeakChromXYData.Add (GetBaseChromXYData(run, scans, peak.XValue, ref peakChromGen, ref smoother));
                    //PeakChromXYData.Add(GetBaseChromXYDataIQ(run, startScan, stopScan, peak.XValue, ref _chromGen, ref smoother));

                    //DeconTools.Backend.XYData clippedEIC = CreateFullSmoothClip(scans, run, ref _lcProcessor, peak, workflowParameters.LCParameters);

                    clippedEIC = CreateBufferSmoothClip(scans, run, ref _lcProcessor, peak, workflowParameters.LCParameters);
                    peaksInEIC = _lcProcessor.Execute(clippedEIC, EnumerationChromatogramProcessing.LCPeakDetectOnly);
                    peaksInEIC = Utiliites.FilterByPointsPerSide(peaksInEIC, workflowParameters.LCParameters.PointsPerShoulder);
                }
                else
                {
                    //we need to add something for the penalty peaks
                    clippedEIC = CreateBufferSmoothClip(scans, run, ref _lcProcessor, peak, workflowParameters.LCParameters);
                    peaksInEIC = _lcProcessor.Execute(clippedEIC, EnumerationChromatogramProcessing.LCPeakDetectOnly);
                    peaksInEIC = Utiliites.FilterByPointsPerSide(peaksInEIC, workflowParameters.LCParameters.PointsPerShoulder);

                    ////check fornull values
                    //if (clippedEIC == null) { clippedEIC = new Run64.Backend.Data.XYData(); }
                    //if (peaksInEIC.Count == 0)
                    //{
                    //    peaksInEIC = new List<ProcessedPeak>();//add empty peak
                    //}

                }

                //check fornull values
                if (clippedEIC == null)
                {
                    clippedEIC = new Run32.Backend.Data.XYData();
                }
                if (peaksInEIC.Count == 0)
                {
                    peaksInEIC = new List<ProcessedPeak>();//add empty peak
                }

                //add to pile
                PeakChromXYData.Add(clippedEIC);
                PeaksFromChromXYData.Add(peaksInEIC);
            }

            int minPeaksForAcceptableIsotopeProfile = 3;
            int minSizeOfEIC = 3;

            bool isThereAPeakInFront = workflowParameters.MSParameters.IsoParameters.PenaltyMode == EnumerationIsotopePenaltyMode.PointToLeft || workflowParameters.MSParameters.IsoParameters.PenaltyMode == EnumerationIsotopePenaltyMode.PointToLeftAndHarmonic;
            if (isThereAPeakInFront)
            {
                IsosPeakListIsOK = iso.Peaklist.Count > minPeaksForAcceptableIsotopeProfile + 1 && iso.MonoIsotopicMass > 0; //this is needed for statistics
            }
            else
            {
                IsosPeakListIsOK = iso.Peaklist.Count > minPeaksForAcceptableIsotopeProfile && iso.MonoIsotopicMass > 0; //this is needed for statistics
            }

            AreChromDataOK = CheckEIC(isThereAPeakInFront, minSizeOfEIC);

            //TODO we could add an advanced check to make sure they correlate with eachother in the profile

            maxBaseIntensity = iso.Peaklist[IndexMostAbundantPeak].Height;

            minBaseIntensity = maxBaseIntensity * MinimumRelativeIntensityForChromCorr;

        }

        private Run32.Backend.Data.XYData CreateBufferSmoothClip(ScanObject scans, Run run, ref ProcessorChromatogram lcProcessor, MSPeak peak, ProcessingParametersChromatogram LCparameters)
        {
            //1.  return a raw chromatorgram with buffers on each side
            Run32.Backend.Data.XYData deconBufferedChromatogram = lcProcessor.GenerateBufferedChromatogramByPoint(run, scans, peak.XValue);

            //2.  convert data
            List<PNNLOmics.Data.XYData> omcisBufferedChromatogram = ConvertXYData.DeconXYDataToOmicsXYData(deconBufferedChromatogram);

            //3.  process data
            List<ProcessedPeak> newMethodInfinity = lcProcessor.Execute(omcisBufferedChromatogram, scans, LCparameters.ProcessLcSectionCorrelationObject);

            //4.  convert back
            Run32.Backend.Data.XYData finallyData = ConvertProcessedPeakToXYData.ConvertPointsToDecon(newMethodInfinity);
            return finallyData;
        }

        //slower and should produce the same results as the buffered version
        private Run32.Backend.Data.XYData CreateFullSmoothClip(ScanObject scans, Run run, ref ProcessorChromatogram lcProcessor, MSPeak peak, ProcessingParametersChromatogram LCparameters)
        {
            //1.  return a raw chromatorgram with buffers on each side
            //DeconTools.Backend.XYData deconBufferedChromatogram = LCparameters.Engine_PeakChromGenerator.GenerateChromatogram(run, scans.Min, scans.Max, peak.XValue, LCparameters.ParametersChromGenerator.ChromToleranceInPPM, Globals.ToleranceUnit.PPM);
            //DeconTools.Backend.XYData deconBufferedChromatogram = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(run, peak.XValue, scans, LCparameters.ParametersChromGenerator.ChromToleranceInPPM);
            Run32.Backend.Data.XYData deconBufferedChromatogram = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(run, peak.XValue, scans);


            //2.  convert data
            List<PNNLOmics.Data.XYData> omcisBufferedChromatogram = ConvertXYData.DeconXYDataToOmicsXYData(deconBufferedChromatogram);

            //3.  process data
            List<ProcessedPeak> newMethodInfinity = lcProcessor.Execute(omcisBufferedChromatogram, scans, LCparameters.ProcessLcSectionCorrelationObject);

            //4.  convert back
            Run32.Backend.Data.XYData finallyData = ConvertProcessedPeakToXYData.ConvertPointsToDecon(newMethodInfinity);
            return finallyData;
        }

        private bool CheckEIC(bool isThereAPeakInFront, int minSizeOfEIC)
        {
            int acceptableCount = 0;
            AcceptableChromList = new List<bool>();

            int startPoint = 0;
            //if (isThereAPeakInFront)
            //{
            //    startPoint = 1;
            //}
            for (int i = startPoint; i < PeakChromXYData.Count; i++)
            {
                Run32.Backend.Data.XYData EICofInterest = PeakChromXYData[i];
                List<ProcessedPeak> PeaksOfInterest = PeaksFromChromXYData[i];

                AreChromDataOK = false;//initialize
                //do we have a XYdata and a least one peak?
                if (PeaksOfInterest != null && EICofInterest != null)
                {
                    int lengthOfEIC = EICofInterest.Xvalues.Length;
                    AreChromDataOK =
                        EICofInterest.Xvalues != null &&
                        lengthOfEIC > minSizeOfEIC &&
                        PeaksOfInterest.Count > 0;
                }

                if (AreChromDataOK == true)
                {
                    AcceptableChromList.Add(true);
                    acceptableCount++;
                }
                else
                {
                    AcceptableChromList.Add(false);
                }
            }

            if (acceptableCount >= 1)//so we need atleast one chromatogram that has detectable peaks
            {
                AreChromDataOK = true;
            }
            else
            {
                AreChromDataOK = false;//not enough eic to proceede
            }

            return AreChromDataOK;
        }

        //public List<PNNLOmics.Data.XYData> GetBaseChromXYData(Run run, ScanObject scans, double baseMZValue, ref PeakChromatogramGenerator peakChromGen, ref PNNLOmics.Algorithms.SpectralProcessing.SavitzkyGolaySmoother smoother, double chromtolerencePPM)
        //{
        //    double ChromToleranceInPPM = chromtolerencePPM;
        //    //int startScan = scans.Start;
        //    //int stopScan = scans.Stop;

        //    //peakChromGen.GenerateChromatogram(run, startScan, stopScan, baseMZValue, ChromToleranceInPPM);
        //    //int lowerScan = startScan;
        //    //int upperScan = stopScan;
        //    //int lowerScan = 0;
        //    //int upperScan = run.MaxLCScan;
        //    if(scans.Buffer == 0)
        //    {
        //        Console.WriteLine("Problem, no scan buffer for smoothing");
        //    }

        //    int lowerScan = scans.Start - scans.Buffer;
        //    int upperScan = scans.Stop + scans.Buffer;

        //    if (lowerScan < 0)
        //    {
        //        lowerScan = 0;
        //    }
        //    if (upperScan > scans.Max)
        //    {
        //        upperScan = scans.Max;//-1?
        //    }

        //    DeconTools.Backend.XYData chromatogram = peakChromGen.GenerateChromatogram(run, lowerScan, upperScan, baseMZValue, ChromToleranceInPPM);//clipped EIC

        //    if (chromatogram == null || chromatogram.Xvalues.Length < 3) return null;

        //    //XYData basePeakChromXYData = smoother.Smooth(run.XYData);
        //    List<PNNLOmics.Data.XYData> convertedData = ConvertXYData.DeconXYDataToOmicsXYData(chromatogram);
        //    List<PNNLOmics.Data.XYData> basePeakChromXYData = smoother.Smooth(convertedData);
        //    //XYData basePeakChromXYData = run.XYData;

        //    bool baseChromDataIsOK = basePeakChromXYData != null && basePeakChromXYData != null &&
        //                             basePeakChromXYData.Count > 3;

        //    if (baseChromDataIsOK)
        //    {
        //        //basePeakChromXYData = basePeakChromXYData.TrimData(scans.Start, scans.Stop);
        //        basePeakChromXYData = ChangeRange.ClipXyDataToScanRange(basePeakChromXYData, scans.Start, scans.Stop);
        //    }

        //    //check to see if chromatogram is all zero
        //    double sum = 0;
        //    for (int i = 0; i < basePeakChromXYData.Count; i++)
        //    {
        //        sum += basePeakChromXYData[i].Y;
        //    }

        //    if (sum > 0)
        //    {
        //        return basePeakChromXYData;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        //lower level
        //protected XYData GetBaseChromXYDataIQ(Run run, int startScan, int stopScan, double baseMZValue, ref ChromatogramGenerator _chromGen, ref SavitzkyGolaySmoother smoother)
        //{
        //    //peakChromGen.GenerateChromatogram(run, startScan, stopScan, baseMZValue, ChromToleranceInPPM);
        //    //XYData chromatogram = peakChromGen.GenerateChromatogram(run,startScan,stopScan,baseMZValue,ChromToleranceInPPM);
        //    int lowerScan = 0;
        //    int upperScan = run.MaxLCScan;
        //    List<double> targetMZList = new List<double>();
        //    targetMZList.Add(baseMZValue);
        //    double massTolerance = ChromToleranceInPPM;
        //    Globals.ToleranceUnit ToleranceUnit = Globals.ToleranceUnit.PPM;

        //    XYData chromValues = _chromGen.GenerateChromatogram(run.ResultCollection.MSPeakResultList, lowerScan, upperScan, targetMZList, massTolerance, ToleranceUnit);

        //    chromValues = FilterOutDataBasedOnMsMsLevel(run, chromValues, 1, false);//this is new

        //    //if (run.XYData == null || run.XYData.Xvalues.Length < 3) return null;

        //    if (chromValues == null || chromValues.Xvalues.Length < 3) return null;

        //        //XYData basePeakChromXYData = smoother.Smooth(run.XYData);
        //    XYData basePeakChromXYData = smoother.Smooth(chromValues);
        //        //XYData basePeakChromXYData = run.XYData;
        //        bool baseChromDataIsOK = basePeakChromXYData != null && basePeakChromXYData.Xvalues != null &&
        //                                 basePeakChromXYData.Xvalues.Length > 3;

        //    if (baseChromDataIsOK)
        //    {
        //        basePeakChromXYData = basePeakChromXYData.TrimData(startScan, stopScan);
        //    }
        //    return basePeakChromXYData;



        //}

        //copied from decon tools
        private Run32.Backend.Data.XYData FilterOutDataBasedOnMsMsLevel(Run run, Run32.Backend.Data.XYData xyData, int msLevelToUse = 1, bool usePrimaryLcScanNumberCache = true)
        {
            if (xyData == null || xyData.Xvalues.Length == 0) return xyData;

            Run32.Backend.Data.XYData filteredXYData = new Run32.Backend.Data.XYData();
            filteredXYData.Xvalues = xyData.Xvalues;
            filteredXYData.Yvalues = xyData.Yvalues;

            if (run.ContainsMSMSData)
            {
                Dictionary<int, double> filteredChromVals = new Dictionary<int, double>();

                bool usePrimaryLcScanNumbers = usePrimaryLcScanNumberCache && run.PrimaryLcScanNumbers != null && run.PrimaryLcScanNumbers.Count > 0;

                for (int i = 0; i < xyData.Xvalues.Length; i++)
                {
                    int currentScanVal = (int)xyData.Xvalues[i];

                    //TODO: this has a problem of cutting off ChromXYData that falls outside the range defined by PrimaryLcScanNumbers. Not good, since this is expected to filter only on MSMS Level
                    //
                    // If the scan is not a primary scan number, then we do not want to consider it
                    if (usePrimaryLcScanNumbers && run.PrimaryLcScanNumbers.BinarySearch(currentScanVal) < 0)
                    {
                        continue;
                    }

                    int msLevel = run.GetMSLevel(currentScanVal);
                    if (msLevel == msLevelToUse)
                    {
                        filteredChromVals.Add(currentScanVal, xyData.Yvalues[i]);
                    }
                }

                filteredXYData.Xvalues = filteredChromVals.Keys.Select(p => (double)p).ToArray();
                filteredXYData.Yvalues = filteredChromVals.Values.ToArray();
            }
            else
            {
                // If we are trying to find MS2 data from a run that does not contain MS2 data, then just return empty arrays
                if (msLevelToUse > 1)
                {
                    filteredXYData.Xvalues = new double[0];
                    filteredXYData.Yvalues = new double[0];
                }
            }

            return filteredXYData;
        }
    }
}
