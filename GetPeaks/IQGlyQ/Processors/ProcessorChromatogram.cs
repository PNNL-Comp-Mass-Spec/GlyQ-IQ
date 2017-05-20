using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend;
using DeconTools.Backend.Core;
using DeconTools.Backend.ProcessingTasks;
using DeconTools.Backend.ProcessingTasks.PeakDetectors;
using DeconTools.Backend.ProcessingTasks.Smoothers;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.Functions;
using IQGlyQ.Functions;
using MultiDimensionalPeakFinding;
using NUnit.Framework;
using PNNLOmics.Algorithms.PeakDetection;
using PNNLOmics.Data;
using UIMFLibrary;
using Peak = PNNLOmics.Data.Peak;
using IQGlyQ.Objects;
using XYData = PNNLOmics.Data.XYData;


namespace IQGlyQ.Processors
{
    public class ProcessorChromatogram : BaseProcessor
    {
        public static ProcessingParametersChromatogram ProcessingParameters { get; set; }

        public int id { get; set; }
        public int ChargeState { get; set; }

        /// <summary>
        /// calculate the scan to retention time once and save it in a dictionary
        /// </summary>
        private static SortedDictionary<int, double> ElutionTimesCache { get; set; }

        #region constructors

        //public ProcessorChromatogram()
        //{
        //    ProcessingParameters = new ChromatogramProcessingParameters();

        //    ProcessingParameters.InitializeEngines();

        //    id = 0;
        //}

        public ProcessorChromatogram(ProcessingParametersChromatogram parameters)
        {
            ProcessingParameters = parameters;

            parameters.InitializeEngines();

            ElutionTimesCache = new SortedDictionary<int, double>();

            id = 0;
        }


        #endregion

        #region executors

        /// <summary>
        /// DeconTools full chromatogram processing.  buffering is done outside and should present larger xydata than scanobject start/stop
        /// </summary>
        /// <param name="chromatogramRaw"></param>
        /// <param name="scans"></param>
        /// <param name="chooseMethod"></param>
        /// <returns></returns>
        public List<DeconTools.Backend.Core.Peak> Execute(DeconTools.Backend.XYData chromatogramRaw, ScanObject scans, EnumerationChromatogramProcessing chooseMethod)
        {
            if (chromatogramRaw == null)
            {
                List<ChromPeak> failList = new List<ChromPeak>();//zero list
                return Utiliites.ConvertDeconPeakToPeakForIQ(failList);
            }
            
            //1.  convert deconTools --> Omics
            List<XYData> omicsRaw = ConvertXYData.DeconXYDataToOmicsXYData(chromatogramRaw);

            //2.  run processor
            List<ProcessedPeak> omicsProcessed = Processor(omicsRaw, id, scans, chooseMethod);

            //3.  convert Omics --> deconTools
            List<ChromPeak> deconProcessedChrome  = ConvertProcessedPeakToDeconPeak.ConvertChromPeakList(omicsProcessed);

            //4.  convert to Peaks--> ChromPeaks
            List<DeconTools.Backend.Core.Peak> deconProcessed = Utiliites.ConvertDeconPeakToPeakForIQ(deconProcessedChrome);
            
            return deconProcessed;
        }

        /// <summary>
        /// DeconTools full chromatogram processing.  buffering is done outside and should present larger xydata than scanobject start/stop
        /// </summary>
        /// <param name="chromatogramRaw"></param>
        /// <param name="scans"></param>
        /// <param name="chooseMethod"></param>
        /// <returns></returns>
        public List<ProcessedPeak> Execute(DeconTools.Backend.XYData chromatogramRaw, EnumerationChromatogramProcessing chooseMethod)
        {
            List<ProcessedPeak> omicsProcessed;
            if (chromatogramRaw == null)
            {
                omicsProcessed = new List<ProcessedPeak>();//zero list
                return omicsProcessed;
            }

            //1.  convert deconTools --> Omics
            List<XYData> omicsRaw = ConvertXYData.DeconXYDataToOmicsXYData(chromatogramRaw);

            //2.  run processor
            ScanObject scans = new ScanObject(0,0);
            omicsProcessed = Processor(omicsRaw, id, scans, chooseMethod);

            return omicsProcessed;
        }

        /// <summary>
        /// Omics full chromatogram processing.  buffering is done outside and should present larger xydata than scanobject start/stop
        /// </summary>
        /// <param name="chromatogramRaw"></param>
        /// <param name="scans"></param>
        /// <param name="chooseMethod"></param>
        /// <returns></returns>
        public List<ProcessedPeak> Execute(List<XYData> chromatogramRaw, EnumerationChromatogramProcessing chooseMethod)
        {
            List<ProcessedPeak> omicsProcessed;
            if (chromatogramRaw == null)
            {
                omicsProcessed = new List<ProcessedPeak>();//zero list
                return omicsProcessed;
            }

            //2.  run processor
            ScanObject scans = new ScanObject(0, 0);
            omicsProcessed = Processor(chromatogramRaw, id, scans, chooseMethod);

            return omicsProcessed;
        }

        /// <summary>
        /// Omics full chromatogram processing.  buffering is done outside and should present larger xydata than scanobject start/stop
        /// </summary>
        /// <param name="chromatogramRaw"></param>
        /// <param name="scans"></param>
        /// <param name="chooseMethod"></param>
        /// <returns></returns>
        public List<ProcessedPeak> Execute(List<XYData> chromatogramRaw, ScanObject scans, EnumerationChromatogramProcessing chooseMethod)
        {
            if (chromatogramRaw == null) return null;

            //1.  run processor
            List<ProcessedPeak> omicsProcessed = Processor(chromatogramRaw, id, scans, chooseMethod);

            return omicsProcessed;
        }

        #endregion

        private List<ProcessedPeak> Processor(List<XYData> chromatogramRaw, int id, ScanObject scans, EnumerationChromatogramProcessing chooseMethod)
        {
            //blank lists
            List<ProcessedPeak> chromatogramOmicsPeaks = new List<ProcessedPeak>();
            List<XYData> chromatogramOmicsXYData =  new List<XYData>();

            List<ProcessedPeak> centroidedPeaks;
            List<ProcessedPeak> thresholdedPeaks;
            List<ProcessedPeak> filteredPeaks;

            switch(chooseMethod)
            {
                case EnumerationChromatogramProcessing.StandardIQ:
                {
                    chromatogramOmicsXYData = SmoothWrapper(chromatogramRaw);
                    chromatogramOmicsPeaks = ChromPeakDetectorWrapper(chromatogramOmicsXYData);
                }
                break;
                
                case EnumerationChromatogramProcessing.QTof:
                {
                    chromatogramOmicsXYData = MovingAverageWrapper(chromatogramRaw, ProcessingParameters.MovingAveragePoints);
                    chromatogramOmicsXYData = SmoothWrapper(chromatogramOmicsXYData);
                    centroidedPeaks = ProcessingParameters.Engine_OmicsPeakDetection.DiscoverPeaks(chromatogramOmicsXYData);
                    filteredPeaks = Utiliites.FilterByPointsPerSide(centroidedPeaks, ProcessingParameters.PointsPerShoulder);
                    thresholdedPeaks = ProcessingParameters.Engine_OmicsPeakThresholding.ApplyThreshold(filteredPeaks);

                    chromatogramOmicsPeaks = ReturnCentroidOrThresholded(centroidedPeaks, thresholdedPeaks, ProcessingParameters.ParametersOmicsThreshold.SignalToShoulderCuttoff);
                }
                break;

                case EnumerationChromatogramProcessing.ChromatogramLevel:
                {
                    
                    
                    
                    bool toWrite = false;

                    string addon = "_" + id + "_" + ChargeState + ".txt";
                    DataXYDataWriter writer = new DataXYDataWriter();

                    //0.  write base chromatrogram
                    string rawPath = ProcessingParameters.XYDataWriterPath + @"\MS_0_Raw";
                    if (toWrite) writer.WriteOmicsXYData(chromatogramRaw, rawPath + addon);

                    //1. core processing
                    chromatogramOmicsPeaks = SmoothCentroidThreshold(chromatogramRaw, id, toWrite);

                    //2.  extra filter after peaks have been selected
                    chromatogramOmicsPeaks = Utiliites.FilterByPointsPerSide(chromatogramOmicsPeaks, ProcessingParameters.PointsPerShoulder);

                    //3.  calculate widths
                    Utiliites.ConvertFWHMwidthToScanWidth(chromatogramOmicsPeaks, chromatogramRaw);

                    //3.  write filtered peaks
                    string filteredPeaksPath = ProcessingParameters.XYDataWriterPath + @"\MS_4_Peaks";
                    if(toWrite) writer.WriteOmicsXYData(ConvertProcessedPeakToXYData.ConvertPoints(chromatogramOmicsPeaks), filteredPeaksPath + addon);
                }
                break;

                case EnumerationChromatogramProcessing.ChromatogramLevelPrint:
                {
                    bool toWrite = false;

                    string addon = "_" + id + "_" + ChargeState + ".txt";
                    DataXYDataWriter writer = new DataXYDataWriter();

                    //0.  write base chromatrogram
                    string rawPath = ProcessingParameters.XYDataWriterPath + @"\MS_0_Raw";
                    if (toWrite) writer.WriteOmicsXYData(chromatogramRaw, rawPath + addon);

                    //1. core processing
                    chromatogramOmicsPeaks = SmoothCentroidThreshold(chromatogramRaw, id, toWrite);

                    //2.  extra filter after peaks have been selected
                    chromatogramOmicsPeaks = Utiliites.FilterByPointsPerSide(chromatogramOmicsPeaks, ProcessingParameters.PointsPerShoulder);

                    //3.  calculate widths
                    Utiliites.ConvertFWHMwidthToScanWidth(chromatogramOmicsPeaks, chromatogramRaw);

                    //3.  write filtered peaks
                    string filteredPeaksPath = ProcessingParameters.XYDataWriterPath + @"\MS_4_Peaks";
                    if (toWrite)
                    {
                        writer.WriteOmicsXYData(ConvertProcessedPeakToXYData.ConvertPoints(chromatogramOmicsPeaks), filteredPeaksPath + addon);
                        Console.WriteLine("Data Written to: " + ProcessingParameters.XYDataWriterPath);
                    }
                }
                break;

                case EnumerationChromatogramProcessing.ChromatogramLevelUnitTest:
                {
                    bool toWrite = false;

                    string addon = "_" + id + "_" + ChargeState + ".txt";
                    DataXYDataWriter writer = new DataXYDataWriter();

                    //0.  write base chromatrogram
                    string rawPath = ProcessingParameters.XYDataWriterPath + @"\MS_0_Raw";
                    if (toWrite) writer.WriteOmicsXYData(chromatogramRaw, rawPath + addon);

                    //1. core processing
                    chromatogramOmicsPeaks = SmoothCentroidThresholdUnitTest(chromatogramRaw, id, toWrite);

                   

                    //2.  extra filter after peaks have been selected
                    chromatogramOmicsPeaks = Utiliites.FilterByPointsPerSide(chromatogramOmicsPeaks, ProcessingParameters.PointsPerShoulder);

                    //3.  write filtered peaks
                    string filteredPeaksPath = ProcessingParameters.XYDataWriterPath + @"\MS_4_Peaks";
                    if (toWrite) writer.WriteOmicsXYData(ConvertProcessedPeakToXYData.ConvertPoints(chromatogramOmicsPeaks), filteredPeaksPath + addon);
                }
                break;

                case EnumerationChromatogramProcessing.ChromatogramLevelWithAverage:
                {
                    bool toWrite = false;

                    string addon = "_" + id + "_" + ChargeState + ".txt";
                    DataXYDataWriter writer = new DataXYDataWriter();

                    //0.  write base chromatrogram
                    string rawPath = ProcessingParameters.XYDataWriterPath + @"\MS_0_Raw";
                    if (toWrite) writer.WriteOmicsXYData(chromatogramRaw, rawPath + addon);

                    //0.  simple moving average first
                    chromatogramOmicsXYData = Utiliites.MovingAverage(chromatogramRaw, ProcessingParameters.MovingAveragePoints);

                    //1. core processing
                    chromatogramOmicsPeaks = SmoothCentroidThreshold(chromatogramOmicsXYData, id, toWrite);

                    //2.  extra filter after peaks have been selected
                    chromatogramOmicsPeaks = Utiliites.FilterByPointsPerSide(chromatogramOmicsPeaks, ProcessingParameters.PointsPerShoulder);

                    //3.  write filtered peaks
                    string filteredPeaksPath = ProcessingParameters.XYDataWriterPath + @"\MS_4_Peaks";
                    if (toWrite) writer.WriteOmicsXYData(ConvertProcessedPeakToXYData.ConvertPoints(chromatogramOmicsPeaks), filteredPeaksPath + addon);
                }
                break;

                case EnumerationChromatogramProcessing.SmoothingOnly:
                {
                    //this is probably not the way to go and will be slow
                    chromatogramOmicsXYData = SmoothWrapper(chromatogramRaw);

                    chromatogramOmicsPeaks = ConvertXYData.OmicsXYDataToProcessedPeaks(chromatogramOmicsXYData);
                }
                break;


                case EnumerationChromatogramProcessing.SmoothSection:
                {
                    chromatogramOmicsXYData = SmoothWrapper(chromatogramRaw);

                    chromatogramOmicsXYData = ChangeRange.ClipXyDataToScanRange(chromatogramOmicsXYData, scans, false);

                    chromatogramOmicsPeaks = ConvertXYData.OmicsXYDataToProcessedPeaks(chromatogramOmicsXYData);
                }
                break;

                case EnumerationChromatogramProcessing.SmoothSectionWithAverage:
                {
                    chromatogramOmicsXYData = Utiliites.MovingAverage(chromatogramRaw, ProcessingParameters.MovingAveragePoints);

                    chromatogramOmicsXYData = SmoothWrapper(chromatogramOmicsXYData);

                    chromatogramOmicsXYData = ChangeRange.ClipXyDataToScanRange(chromatogramOmicsXYData, scans, false);

                    chromatogramOmicsPeaks = ConvertXYData.OmicsXYDataToProcessedPeaks(chromatogramOmicsXYData);
                }
                break;

                case EnumerationChromatogramProcessing.LCPeakDetectOnly:
                {
                    centroidedPeaks = ProcessingParameters.Engine_OmicsPeakDetection.DiscoverPeaks(chromatogramRaw);

                    thresholdedPeaks = ProcessingParameters.Engine_OmicsPeakThresholding.ApplyThreshold(centroidedPeaks);

                    chromatogramOmicsPeaks = ReturnCentroidOrThresholded(centroidedPeaks, thresholdedPeaks, ProcessingParameters.ParametersOmicsThreshold.SignalToShoulderCuttoff);
                }
                break;

                default:
                {
                    bool toWrite = false;

                    //1. core processing
                    chromatogramOmicsPeaks = SmoothCentroidThreshold(chromatogramRaw, id, toWrite);
                }
                break;
            }

            return chromatogramOmicsPeaks;
        }

        

        private List<ProcessedPeak> SmoothCentroidThreshold(List<XYData> chromatogramRaw, int id, bool toWrite)
        {
            List<XYData> chromatogramOmicsXYData;
            List<ProcessedPeak> thresholdedPeaks;
            List<ProcessedPeak> centroidedPeaks;
            List<ProcessedPeak> chromatogramOmicsPeaks;

            string smoothedPath = ProcessingParameters.XYDataWriterPath + @"\MS_2_Smoothed";
            string peaksPath = ProcessingParameters.XYDataWriterPath + @"\MS_3_Peaks";
            
            DataXYDataWriter writer = new DataXYDataWriter();
            string addon = "_" + id + "_" + ChargeState + ".txt";

            //1.  smooth data
            chromatogramOmicsXYData = SmoothWrapper(chromatogramRaw);

            //write smoothed data
            if (toWrite) writer.WriteOmicsXYData(chromatogramOmicsXYData, smoothedPath + addon);

            //2.  discover peaks and threshold if >0
            centroidedPeaks = ProcessingParameters.Engine_OmicsPeakDetection.DiscoverPeaks(chromatogramOmicsXYData);

            thresholdedPeaks = ProcessingParameters.Engine_OmicsPeakThresholding.ApplyThreshold(centroidedPeaks);

            chromatogramOmicsPeaks = ReturnCentroidOrThresholded(centroidedPeaks, thresholdedPeaks, ProcessingParameters.ParametersOmicsThreshold.SignalToShoulderCuttoff);

            //write peaks
            if (toWrite) writer.WriteOmicsXYData(ConvertProcessedPeakToXYData.ConvertPoints(chromatogramOmicsPeaks), peaksPath + addon);

            return chromatogramOmicsPeaks;
        }

        private List<ProcessedPeak> SmoothCentroidThresholdUnitTest(List<XYData> chromatogramRaw, int id, bool toWrite)
        {
            List<XYData> chromatogramOmicsXYData;
            List<ProcessedPeak> thresholdedPeaks;
            List<ProcessedPeak> centroidedPeaks;
            List<ProcessedPeak> chromatogramOmicsPeaks;

            string smoothedPath = ProcessingParameters.XYDataWriterPath + @"\MS_2_Smoothed";
            string peaksPath = ProcessingParameters.XYDataWriterPath + @"\MS_3_Peaks";

            DataXYDataWriter writer = new DataXYDataWriter();
            string addon = "_" + id + "_" + ChargeState + ".txt";

            Assert.AreEqual(1847, chromatogramRaw[590].X);
            Assert.AreEqual(148702.40625d, chromatogramRaw[590].Y);
            Assert.AreEqual(1817, chromatogramRaw[580].X);
            Assert.AreEqual(557311.1875d, chromatogramRaw[580].Y);

            //1.  smooth data
            chromatogramOmicsXYData = SmoothWrapper(chromatogramRaw);

            Assert.AreEqual(1847, chromatogramOmicsXYData[590].X);
            Assert.AreEqual(126319.24181547623d, chromatogramOmicsXYData[590].Y);
            Assert.AreEqual(1817, chromatogramOmicsXYData[580].X);
            Assert.AreEqual(606164.8125d, chromatogramOmicsXYData[580].Y);

            //write smoothed data
            if (toWrite) writer.WriteOmicsXYData(chromatogramOmicsXYData, smoothedPath + addon);

            //2.  discover peaks and threshold if >0
            centroidedPeaks = ProcessingParameters.Engine_OmicsPeakDetection.DiscoverPeaks(chromatogramOmicsXYData);

            thresholdedPeaks = ProcessingParameters.Engine_OmicsPeakThresholding.ApplyThreshold(centroidedPeaks);

            chromatogramOmicsPeaks = ReturnCentroidOrThresholded(centroidedPeaks, thresholdedPeaks, ProcessingParameters.ParametersOmicsThreshold.SignalToShoulderCuttoff);

            //write peaks
            if (toWrite) writer.WriteOmicsXYData(ConvertProcessedPeakToXYData.ConvertPoints(chromatogramOmicsPeaks), peaksPath + addon);

            return chromatogramOmicsPeaks;
        }

        /// <summary>
        /// This is an interesting function that returnes a chromatorgram bounded by the scan object + buffer number of points on each side.  this does pull the full LC so it is slow but accurate
        /// </summary>
        /// <param name="_peakChromGen"></param>
        /// <param name="runIn"></param>
        /// <param name="scans"></param>
        /// <param name="massToExtract"></param>
        /// <param name="chromTollerencePPM"></param>
        /// <returns></returns>
        public DeconTools.Backend.XYData GenerateBufferedChromatogramByPoint(Run runIn, ScanObject scans, double massToExtract)
        {
            ScanObject bufferedRange = new ScanObject(scans);
            int pointsToExtend = scans.Buffer;

            //1.  get scan set
            if (runIn.ScanSetCollection == null || runIn.ScanSetCollection.ScanSetList.Count == 0)
            {
                //4-22-2013
                //runIn.ScanSetCollection.Create(runIn, scans.Min, scans.Max, scans.ScansToSum, 1, false);
                return null;
            }

            if (bufferedRange.Stop > bufferedRange.Max)
            {
                bufferedRange.Stop = bufferedRange.Max;
            }

            if (bufferedRange.Start < bufferedRange.Min)
            {
                bufferedRange.Start = bufferedRange.Min;
            }

            if (bufferedRange.Start < runIn.ScanSetCollection.ScanSetList[0].PrimaryScanNumber)
            {
                bufferedRange.Start = runIn.ScanSetCollection.ScanSetList[0].PrimaryScanNumber;
            }

            if (bufferedRange.Stop > runIn.ScanSetCollection.ScanSetList[runIn.ScanSetCollection.ScanSetList.Count-1].PrimaryScanNumber)
            {
                bufferedRange.Stop = runIn.ScanSetCollection.ScanSetList[runIn.ScanSetCollection.ScanSetList.Count-1].PrimaryScanNumber;
            }

            //2.  convert scan range to ScanSets
            ScanSet lowerScanSet = (from scan in runIn.ScanSetCollection.ScanSetList where scan.PrimaryScanNumber <= bufferedRange.Start select scan).Last();
            
            ScanSet upperScanSet = (from scan in runIn.ScanSetCollection.ScanSetList where scan.PrimaryScanNumber >= bufferedRange.Stop select scan).First();

            //3.  convert to index space
            int lowerIndex = runIn.ScanSetCollection.ScanSetList.IndexOf(lowerScanSet);
            int upperIndex = runIn.ScanSetCollection.ScanSetList.IndexOf(upperScanSet);

            //4.  deal with end points
            int newStartIndex;
            if (lowerIndex - pointsToExtend < 0)
            {
                newStartIndex = 0;
            }
            else
            {
                newStartIndex = lowerIndex - pointsToExtend;
            }

            //TODO check this but I think the indexes are fixed  5-1-2013
            int newStopIndex = upperIndex + pointsToExtend;
            if (newStopIndex >= runIn.ScanSetCollection.ScanSetList.Count)
            {
                newStopIndex = runIn.ScanSetCollection.ScanSetList.Count - 1;
            }
            else
            {
                newStopIndex = upperIndex + pointsToExtend;
            }

            //5.  convert indexes into scan numbers
            bufferedRange.Start = runIn.ScanSetCollection.ScanSetList[newStartIndex].PrimaryScanNumber;
            bufferedRange.Stop = runIn.ScanSetCollection.ScanSetList[newStopIndex].PrimaryScanNumber;


            //slow
            //DeconTools.Backend.XYData deconChromatogram = ProcessingParameters.Engine_PeakChromGenerator.GenerateChromatogram(runIn, bufferedRange.Min, bufferedRange.Max, massToExtract, chromTollerencePPM);
            //DeconTools.Backend.XYData clippedChromatogram = deconChromatogram.TrimData(bufferedRange.Start, bufferedRange.Stop);

            //fast
            

            //DeconTools.Backend.XYData deconChromatogram = ProcessingParameters.Engine_PeakChromGenerator.GenerateChromatogram(runIn, bufferedRange.Start, bufferedRange.Stop, massToExtract, chromTollerencePPM);
            //DeconTools.Backend.XYData deconChromatogram = DeconChromatogramGeneratorWrapper(runIn, massToExtract, bufferedRange,chromTollerencePPM, ProcessingParameters.Engine_PeakChromGenerator);//old
            //DeconTools.Backend.XYData deconChromatogram = DeconChromatogramGeneratorWrapper(runIn, massToExtract, bufferedRange, chromTollerencePPM);//old
            DeconTools.Backend.XYData deconChromatogram = DeconChromatogramGeneratorWrapper(runIn, massToExtract, bufferedRange);

            if (deconChromatogram == null)
                return null;
           
            DeconTools.Backend.XYData clippedChromatogram = deconChromatogram.TrimData(bufferedRange.Start, bufferedRange.Stop);
           
            //find first point and zero fill
            int firstScan = Convert.ToInt32(clippedChromatogram.Xvalues[0]);
            int lastScan = Convert.ToInt32(clippedChromatogram.Xvalues[clippedChromatogram.Xvalues.Length - 1]);

            int section1Length = 0;
            int lowerIndexZeroBefore = 0;
            int upperIndexZeroBefore = 0;
            if (firstScan != bufferedRange.Start && firstScan > bufferedRange.Start)
            {
                //2.  convert scan range to ScanSets
                ScanSet lowerScanSetZeroBefore = (from scan in runIn.ScanSetCollection.ScanSetList where scan.PrimaryScanNumber == bufferedRange.Start select scan).Last();
                ScanSet upperScanSetZeroBefore = (from scan in runIn.ScanSetCollection.ScanSetList where scan.PrimaryScanNumber == firstScan select scan).First();

                //3.  convert to index space
                lowerIndexZeroBefore = runIn.ScanSetCollection.ScanSetList.IndexOf(lowerScanSetZeroBefore);
                upperIndexZeroBefore = runIn.ScanSetCollection.ScanSetList.IndexOf(upperScanSetZeroBefore);

                section1Length = upperIndexZeroBefore - lowerIndexZeroBefore;
            }

            //zero fill rest of data
            int section3Length = 0;
            int lowerIndexZeroAfter = 0;
            int upperIndexZeroAfter = 0;
            if (lastScan != bufferedRange.Stop && lastScan < bufferedRange.Stop)
            {
                //2.  convert scan range to ScanSets
                ScanSet lowerScanSetZeroAfter = (from scan in runIn.ScanSetCollection.ScanSetList where scan.PrimaryScanNumber == lastScan select scan).Last();
                ScanSet upperScanSetZeroAfter = (from scan in runIn.ScanSetCollection.ScanSetList where scan.PrimaryScanNumber == bufferedRange.Stop select scan).First();

                //3.  convert to index space
                lowerIndexZeroAfter = runIn.ScanSetCollection.ScanSetList.IndexOf(lowerScanSetZeroAfter);
                upperIndexZeroAfter = runIn.ScanSetCollection.ScanSetList.IndexOf(upperScanSetZeroAfter);

                section3Length = upperIndexZeroAfter - lowerIndexZeroAfter;
            }

            int section2Length = clippedChromatogram.Xvalues.Length;

            int fulllength = section1Length + section2Length + section3Length;

            DeconTools.Backend.XYData clippedChromatogramNew = new DeconTools.Backend.XYData();
            clippedChromatogramNew.Xvalues = new double[fulllength];
            clippedChromatogramNew.Yvalues = new double[fulllength];

            //add base data

            for (int i = 0; i < section1Length; i++)
            {
                clippedChromatogramNew.Xvalues[i] = runIn.ScanSetCollection.ScanSetList[lowerIndexZeroBefore + i].PrimaryScanNumber;
                clippedChromatogramNew.Yvalues[i] = 0;
            }

            //r (int i = section1Length; i < section1Length + section2Length - 1; i++)
            for (int i = 0; i < section2Length; i++)
            {
                //clippedChromatogramNew.Xvalues[i] = clippedChromatogram.Xvalues[i];
                //clippedChromatogramNew.Yvalues[i] = clippedChromatogram.Yvalues[i];

                clippedChromatogramNew.Xvalues[i + section1Length] = clippedChromatogram.Xvalues[i];
                clippedChromatogramNew.Yvalues[i + section1Length] = clippedChromatogram.Yvalues[i];
            }

            for (int i = 0; i < section3Length; i++)
            {
                clippedChromatogramNew.Xvalues[i + section1Length + section2Length] = runIn.ScanSetCollection.ScanSetList[lowerIndexZeroAfter + i+1].PrimaryScanNumber;
                clippedChromatogramNew.Yvalues[i + section1Length + section2Length] = 0;
            }

            return clippedChromatogramNew;
        }





        public static SortedDictionary<int, double> CalculateElutionTimes(Run run, List<int> scans)
        {
            SortedDictionary<int,double> elutionTimes = new SortedDictionary<int, double>();
            foreach (int scan in scans)
            {
                elutionTimes.Add(scan,CalculateElutionTime(run, scan));
            }
            return elutionTimes;
        }

        public static double CalculateElutionTime(Run run, int scan)
        {
            //populate cache if it is not allready
            if (ElutionTimesCache.Count==0)
            {
                List<ScanSet> scansInRun = run.ScanSetCollection.ScanSetList.ToList();
                foreach (var scanSet in scansInRun)
                {
                    ElutionTimesCache.Add(scanSet.PrimaryScanNumber, run.GetTime(scanSet.PrimaryScanNumber));
                }
            }

            //check if scan is in run
            if(run.PrimaryLcScanNumbers.Contains(scan))
            {
                //Console.WriteLine("Scan is in there");
            }
            else
            {
                Console.Write("Scan is not in there...");
                //add and interpolate time
                int scanbefore = 0;
                int scanAfter = 0;
                double timeBefore = 0;
                double timeAfter = 0;
                
                int findIndex = scan;
                while(findIndex>0 && findIndex<run.PrimaryLcScanNumbers.Last())
                {
                    findIndex--;
                    if(run.PrimaryLcScanNumbers.Contains(findIndex))
                    {
                        scanbefore = findIndex;
                        timeBefore = ElutionTimesCache[findIndex];
                        break;
                    }
                }

                while (findIndex > 0 && findIndex < run.PrimaryLcScanNumbers.Last())
                {
                    findIndex++;
                    if (run.PrimaryLcScanNumbers.Contains(findIndex))
                    {
                        scanAfter = findIndex;
                        timeAfter = ElutionTimesCache[findIndex];
                        break;
                    }
                }

                ElutionTimesCache.Add(scan,timeBefore + (timeAfter-timeBefore)*(scan-scanbefore)/(scanAfter-scanbefore));
                Console.Write(" Scan " + scan +" was added to ElutionTimesCache" + Environment.NewLine);
            }

            if (ElutionTimesCache.ContainsKey(scan))
            {
                return ElutionTimesCache[scan];
            }
            else
            {
                return 0;
            }

            //return run.NetAlignmentInfo.GetNETValueForScan(scan);//old Gordon
            //return run.GetTime(scan);
        }

        public static SortedDictionary<int, double> CalculateElutionTimes(Run run, List<ChromPeak> peakList)
        {
            List<int> scans = new List<int>();
            foreach (ChromPeak chromPeak in peakList)
            {
                scans.Add((int) Math.Round(chromPeak.XValue));
            }

            SortedDictionary<int, double> elutionTimes = CalculateElutionTimes(run, scans);

            return elutionTimes;
        }

        public static void CalculateElutionTimes(Run run, ref List<DeconTools.Backend.Core.Peak> peakList)
        {
            foreach (DeconTools.Backend.Core.ChromPeak chromPeak in peakList)
            {
                int scan = (int)Math.Round(chromPeak.XValue);
                chromPeak.NETValue = CalculateElutionTime(run, scan);
            }
        }
        

        private static List<XYData> MovingAverageWrapper(List<XYData> chromatogramRaw, int numberOfPoints)
        {
            DeconTools.Backend.XYData chromatogramDeconProcessed = ConvertXYData.OmicsXYDataToRunXYData(chromatogramRaw);

            //decon code
            chromatogramDeconProcessed = Utiliites.MovingAverage(chromatogramDeconProcessed, numberOfPoints);

            List<XYData> chromatogramProcessed = ConvertXYData.DeconXYDataToOmicsXYData(chromatogramDeconProcessed);

            return chromatogramProcessed;
        }

        public static List<XYData> SmoothWrapper(List<XYData> chromatogramRaw)
        {
            bool pureOmics = true;//switches between omics based and decon based smoother

            if (pureOmics)
            {
                if (chromatogramRaw != null && chromatogramRaw.Count > ProcessingParameters.Engine_Smoother.PointsForSmoothing)
                {
                    chromatogramRaw = ProcessingParameters.Engine_Smoother.Smooth(chromatogramRaw);
                }
            }
            else
            {
                DeconTools.Backend.XYData chromatogramDeconProcessed = ConvertXYData.OmicsXYDataToRunXYData(chromatogramRaw);

                if (chromatogramRaw.Count > ProcessingParameters.Engine_Smoother.PointsForSmoothing)
                {
                    //decon code
                    chromatogramDeconProcessed = ProcessingParameters.Engine_Smoother_Decon.Smooth(chromatogramDeconProcessed);
                }

                chromatogramRaw = ConvertXYData.DeconXYDataToOmicsXYData(chromatogramDeconProcessed);
            }

            return chromatogramRaw;
        }

        private static List<ProcessedPeak> ChromPeakDetectorWrapper(List<XYData> chromatogramRaw)
        {
            DeconTools.Backend.XYData chromatogramDeconProcessed = ConvertXYData.OmicsXYDataToRunXYData(chromatogramRaw);

            //decon code
            List<DeconTools.Backend.Core.Peak> chromatogramDeconPeaks = ProcessingParameters.Engine_ChromPeakDetector.FindPeaks(chromatogramDeconProcessed);

            List<ProcessedPeak> omicsPeaks = ConvertPeakListDeconToOmics.ConvertToProcessedPeak(chromatogramDeconPeaks,false);

            return omicsPeaks;
        }


        #region peak chrom generation section



        //these three are for unit testing only
        //1 this is a base call for others
        //public static DeconTools.Backend.XYData DeconChromatogramGeneratorWrapper(Run runIn, double massToExtract, ScanObject bufferedRange, double chromTollerencePPM, PeakChromatogramGenerator _peakChromGen)
        //{
        //    DeconTools.Backend.XYData deconChromatogram; 
        //    if (ProcessingParameters.isIMS)
        //    {
        //        DeconTools.Backend.XYData imsChromatogram = IMS_XIC(runIn, massToExtract, chromTollerencePPM); deconChromatogram = imsChromatogram;   
        //    }
        //    else
        //    {
        //        deconChromatogram = _peakChromGen.GenerateChromatogram(runIn, bufferedRange.Start, bufferedRange.Stop, massToExtract, chromTollerencePPM);
        //    }
        //    return deconChromatogram;
        //}

        //2
        public static DeconTools.Backend.XYData DeconChromatogramGeneratorWrapper(Run runIn, IsotopicProfile theorIsotopicProfile, ScanObject bufferedRange, double chromTollerencePPM, Globals.ToleranceUnit unit, PeakChromatogramGenerator _peakChromGen)
        {
            DeconTools.Backend.XYData deconChromatogram;
            if (ProcessingParameters.isIMS)
            {
                
                double massToExtract = theorIsotopicProfile.GetMZofMostAbundantPeak();
                DeconTools.Backend.XYData imsChromatogram = IMS_XIC(runIn, massToExtract, chromTollerencePPM); deconChromatogram = imsChromatogram;
            }
            else
            {
                deconChromatogram = _peakChromGen.GenerateChromatogram(runIn, theorIsotopicProfile, bufferedRange.Min, bufferedRange.Max, chromTollerencePPM, unit);
            
            }
            return deconChromatogram;
        }

        //3
        public static DeconTools.Backend.XYData DeconChromatogramGeneratorWrapper(Run runIn, IsotopicProfile theorIsotopicProfile, double elutionTimeTheor, PeakChromatogramGenerator _peakChromGen)
        {
            DeconTools.Backend.XYData chromatogram;
            if (ProcessingParameters.isIMS)
            {

                double massToExtract = theorIsotopicProfile.GetMZofMostAbundantPeak();
                DeconTools.Backend.XYData imsChromatogram = IMS_XIC(runIn, massToExtract, ProcessingParameters.ParametersChromGenerator.ChromToleranceInPPM); chromatogram = imsChromatogram;
            }
            else
            {
                chromatogram = _peakChromGen.GenerateChromatogram(runIn, theorIsotopicProfile, elutionTimeTheor);
            
            }
            return chromatogram;
        }




        //working functions.  eic ppm is inside the PeakChromatogramGenerator

        //this is the one we want everywhere for full XIC (exactmass)
        public static DeconTools.Backend.XYData DeconChromatogramGeneratorWrapper(Run runIn, double massToExtract)
        {
            DeconTools.Backend.XYData deconChromatogram;
            ScanObject bufferedRange = new ScanObject(runIn.MinLCScan, runIn.MaxLCScan);

            if (ProcessingParameters.isIMS)
            {
                DeconTools.Backend.XYData imsChromatogram = IMS_XIC(runIn, massToExtract, ProcessingParameters.Engine_PeakChromGenerator.Tolerance); deconChromatogram = imsChromatogram;
            }
            else
            {
                deconChromatogram = ProcessingParameters.Engine_PeakChromGenerator.GenerateChromatogram(runIn, bufferedRange.Start, bufferedRange.Stop, massToExtract, ProcessingParameters.Engine_PeakChromGenerator.Tolerance);
            }
            return deconChromatogram;
        }

        //this is the one we want everywhere for scan bounded XIC (exactmass)
        public static DeconTools.Backend.XYData DeconChromatogramGeneratorWrapper(Run runIn, double massToExtract, ScanObject bufferedRange)
        {
            DeconTools.Backend.XYData deconChromatogram;
            
            if (ProcessingParameters.isIMS)
            {
                DeconTools.Backend.XYData imsChromatogram = IMS_XIC(runIn, massToExtract, ProcessingParameters.Engine_PeakChromGenerator.Tolerance); deconChromatogram = imsChromatogram;
            }
            else
            {
                deconChromatogram = ProcessingParameters.Engine_PeakChromGenerator.GenerateChromatogram(runIn, bufferedRange.Start, bufferedRange.Stop, massToExtract, ProcessingParameters.Engine_PeakChromGenerator.Tolerance);
            }
            return deconChromatogram;
        }


        //these are simple ones where there is no PeakChromatogramGenerator and use a direct call
        //this one need to go but is highly important
        //public static DeconTools.Backend.XYData DeconChromatogramGeneratorWrapper(Run runIn, double massToExtract, ScanObject bufferedRange, double chromTollerencePPM)
        //{
        //    //DeconTools.Backend.XYData deconChromatogram = DeconChromatogramGeneratorWrapper(runIn, massToExtract, bufferedRange, chromTollerencePPM, ProcessingParameters.Engine_PeakChromGenerator);//old
        //    DeconTools.Backend.XYData deconChromatogram = DeconChromatogramGeneratorWrapper(runIn, massToExtract, bufferedRange, chromTollerencePPM);
        //    return deconChromatogram;
        //}

        //public static DeconTools.Backend.XYData DeconChromatogramGeneratorWrapper(Run runIn, double massToExtract, double chromTollerencePPM)
        //{
        //    var bufferedRange = new ScanObject(runIn.MinLCScan,runIn.MaxLCScan);
        //    DeconTools.Backend.XYData deconChromatogram = DeconChromatogramGeneratorWrapper(runIn, massToExtract, bufferedRange, chromTollerencePPM, ProcessingParameters.Engine_PeakChromGenerator);
        //    return deconChromatogram;
        //}

        //public static DeconTools.Backend.XYData DeconChromatogramGeneratorWrapper(Run runIn, double massToExtract, double chromTollerencePPM)
        //{
        //    var bufferedRange = new ScanObject(runIn.MinLCScan, runIn.MaxLCScan);
        //    DeconTools.Backend.XYData deconChromatogram = DeconChromatogramGeneratorWrapper(runIn, massToExtract, bufferedRange, chromTollerencePPM, ProcessingParameters.Engine_PeakChromGenerator);
        //    return deconChromatogram;
        //}

        //this one is odd and calls _peakChromGen.GenerateChromatogram(runIn, theorIsotopicProfile, elutionTimeTheor)
        public static DeconTools.Backend.XYData DeconChromatogramGeneratorWrapper(Run runIn, IsotopicProfile theorIsotopicProfile, double elutionTimeTheor)
        {
            DeconTools.Backend.XYData deconChromatogram = DeconChromatogramGeneratorWrapper(runIn, theorIsotopicProfile, elutionTimeTheor, ProcessingParameters.Engine_PeakChromGenerator);
            return deconChromatogram;
        }
        //_workflowParameters.LCParameters.Engine_PeakChromGenerator






        private static DeconTools.Backend.XYData IMS_XIC(Run runIn, double massToExtract, double chromTollerencePPM)
        {
            MultiDimensionalPeakFinding.UimfUtil IMS = new UimfUtil(runIn.Filename);
            List<IntensityPoint> results = IMS.GetXic(massToExtract, chromTollerencePPM, UIMFLibrary.DataReader.FrameType.MS1,
                                                      DataReader.ToleranceType.PPM);

            DeconTools.Backend.XYData imsChromatogram = new DeconTools.Backend.XYData();
            imsChromatogram.Xvalues = new double[results.Count];
            imsChromatogram.Yvalues = new double[results.Count];
            int i = 0;
            foreach (IntensityPoint intensityPoint in results)
            {
                imsChromatogram.Xvalues[i] = intensityPoint.ScanLc;
                imsChromatogram.Yvalues[i] = intensityPoint.Intensity;
                i++;
            }

            return imsChromatogram;
        }
        #endregion
    }
}
