using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using DeconTools.Backend.ProcessingTasks.FitScoreCalculators;
using DeconTools.Backend.ProcessingTasks.MSGenerators;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.Functions;
using IQGlyQ.Objects;
using IQGlyQ.Tasks;
using PNNLOmics.Data;
using XYData = DeconTools.Backend.XYData;

namespace IQGlyQ.Processors
{
    public class ProcessorMassSpectra : BaseProcessor
    {
        public static ProcessingParametersMassSpectra ProcessingParameters { get; set; }

        public int id { get; set; }
        public int ChargeState { get; set; }

        #region constructors

        public ProcessorMassSpectra()
        {
            ProcessingParameters = new ProcessingParametersMassSpectra();

            ProcessingParameters.InitializeEngines();

            id = 0;
        }

        public ProcessorMassSpectra(ProcessingParametersMassSpectra parameters)
        {
            ProcessingParameters = parameters;

            parameters.InitializeEngines();

            id = 0;
        }

        #endregion

       


        public static XYData ReCalibrateMassSpectraForIFF(IsotopicProfile theoreticalProfile, XYData rawMassSpectrum, double daltonOffset)
        {
            ScanObject ranges = new ScanObject(Convert.ToInt32(theoreticalProfile.MonoIsotopicMass - 10), Convert.ToInt32(theoreticalProfile.MonoIsotopicMass + 20));

            rawMassSpectrum.TrimData(ranges.Start, ranges.Stop);

            double massCalibration = daltonOffset;
            double[] newNumbers = rawMassSpectrum.Xvalues.Select(i => i + massCalibration).ToArray();

            XYData calibratedata = new XYData();
            calibratedata.Yvalues = rawMassSpectrum.Yvalues;
            calibratedata.Xvalues = newNumbers;

            return calibratedata;
        }

        #region executors

        /// <summary>
        /// DeconTools full Mass Spectra processing
        /// </summary>
        /// <param name="spectraRaw"></param>
        /// <param name="scans"></param>
        /// <param name="chooseMethod"></param>
        /// <returns></returns>
        public List<ProcessedPeak> Execute(DeconTools.Backend.XYData spectraRaw, EnumerationMassSpectraProcessing chooseMethod)
        {
            List<ProcessedPeak> omicsProcessed;
            if (spectraRaw == null)
            {
                omicsProcessed = new List<ProcessedPeak>();//zero list
                return omicsProcessed;
            }

            //1.  convert deconTools --> Omics
            List<PNNLOmics.Data.XYData> omicsRaw = ConvertXYData.DeconXYDataToOmicsXYData(spectraRaw);

            //2.  run processor
            ScanObject scans = new ScanObject(0, 0);
            omicsProcessed = Processor(omicsRaw, id, scans, chooseMethod);

            return omicsProcessed;
        }

        /// <summary>
        /// Omics full Mass Spectra processing
        /// </summary>
        /// <param name="spectraRaw"></param>
        /// <param name="scans"></param>
        /// <param name="chooseMethod"></param>
        /// <returns></returns>
        public List<ProcessedPeak> Execute(List<PNNLOmics.Data.XYData> spectraRaw, EnumerationMassSpectraProcessing chooseMethod)
        {
            List<ProcessedPeak> omicsProcessed;
            if (spectraRaw == null)
            {
                omicsProcessed = new List<ProcessedPeak>();//zero list
                return omicsProcessed;
            }

            //2.  run processor
            ScanObject scans = new ScanObject(0, 0);
            omicsProcessed = Processor(spectraRaw, id, scans, chooseMethod);

            return omicsProcessed;
        }


        /// <summary>
        /// InterfearanceScore
        /// </summary>
        /// <param name="spectraRaw"></param>
        /// <param name="scans"></param>
        /// <param name="chooseMethod"></param>
        /// <returns></returns>
        public double ExecuteInterference(IsotopicProfile iso,  List<DeconTools.Backend.Core.Peak> mspeakList)
        {
            double score = 0;
            score = ProcessingParameters.Engine_InterferenceScorer.GetInterferenceScore(iso, mspeakList);
            return score;
        }

        /// <summary>
        /// FitScore
        /// </summary>
        /// <param name="spectraRaw"></param>
        /// <param name="scans"></param>
        /// <param name="chooseMethod"></param>
        /// <returns></returns>
        public double ExecuteFitScore(IsotopicProfile isoTheoretical, IsotopicProfile isoExperimental)
        {
            double score = 0;
            
            if (isoExperimental.Peaklist != null && isoExperimental.Peaklist.Count > 0 && isoTheoretical.Peaklist != null && isoTheoretical.Peaklist.Count > 0)
            {
                //at this point we need a 1:1 between TheorIsotopicProfile and ObservedIsotopicProfile
                //step 1  take first point in observed and find corresponding index in theoretical so that they line up,  if they need to be offset, add 0 to observed profile
                MSPeak firstPeak = isoExperimental.Peaklist[0];

                //step 2. find nearest peak in isoTheoretical
                MSPeak firstTheoreticalPeak = SelectClosest.SelectNearestMSPeakToCenter(isoTheoretical.Peaklist, firstPeak.XValue);
                double difference = firstTheoreticalPeak.XValue - firstPeak.XValue;
                double normalizedDifference = difference/(isoTheoretical.Peaklist[1].XValue - isoTheoretical.Peaklist[0].XValue);
                double index = Convert.ToInt32(Math.Round(normalizedDifference))*ChargeState;

                List<MSPeak> newObservedPeaks = new List<MSPeak>();
                for(int i=0;i<index;i++)
                {
                    newObservedPeaks.Add(new MSPeak(0,0,0,0));//since we are dealing with inxedex peaks we can just put in empty peaks
                }
                for(int i=0;i<isoExperimental.Peaklist.Count;i++)//fill in rest of peaks
                {
                    newObservedPeaks.Add(isoExperimental.Peaklist[i]);
                }

                isoExperimental.Peaklist = newObservedPeaks;
                //peak lists should be synchronized now

                //remove points that will not be compared
                //step 1.  find max of theoretical distribution by searcing backwards untill a peak is above the cuttoff
                double cuttoff = ProcessingParameters.IsoParameters.FractionalIntensityCuttoffForTheoretical;
                int counter = isoTheoretical.Peaklist.Count - 1;
                while (counter>0)
                {
                    if(isoTheoretical.Peaklist[counter].Height >= cuttoff)
                    {
                       
                        break;
                    }
                    counter--;

                }

                counter++;//captures last peak above threshold
                int stopIndex = counter;

                List<MSPeak> truncatedObservedPeaks = new List<MSPeak>();
                List<MSPeak> truncatedTheoreticalPeaks = new List<MSPeak>();
                if (stopIndex > newObservedPeaks.Count)
                {
                    stopIndex = newObservedPeaks.Count;//make sure we don't over run the experimental data
                }
                for(int i=0;i<stopIndex;i++)
                {
                    
                    truncatedTheoreticalPeaks.Add(isoTheoretical.Peaklist[i]);

                    if (i < newObservedPeaks.Count)//make sure we don't rey to add an observed peak that is not there
                    {
                        truncatedObservedPeaks.Add(newObservedPeaks[i]);
                    }
                    else
                    {
                        truncatedObservedPeaks.Add(new MSPeak(0,0,0,0));//no longer needed
                    }
                }
                
                //IsotopicPeakFitScoreCalculator currentCalculator = (IsotopicPeakFitScoreCalculator)ProcessingParameters.Engine_FitScoreCalculator;
                //score = currentCalculator.CalculateFitScore(isoTheoretical, isoExperimental, spectraRaw, currentCalculator.NumberOfPeaksToLeftForPenalty);
                IsotopicPeakFitScoreCalculatorSK currentCalculator = (IsotopicPeakFitScoreCalculatorSK) ProcessingParameters.Engine_FitScoreCalculator;

                //base for testing multiple fit scores.  false will do multi scoring (default)
                bool singleScore = false;
                //testing for multiple D/H ratios
                bool multiIntensity = false;
                //testing for mass to the left and right +1da - 1 Da
                bool multiMass = true;

                if (singleScore)
                {
                    score = currentCalculator.CalculateFitScore(truncatedTheoreticalPeaks, truncatedObservedPeaks, ProcessingParameters.IsoParameters.DivideFitScoreByNumberOfIons);
                }
                else
                {
                    bool ionIsAcceptable = false;
                    double bestScore = 1;

                    if (multiMass)
                    {
                        ionIsAcceptable = IonIsAcceptable(truncatedTheoreticalPeaks, truncatedObservedPeaks, currentCalculator, out bestScore);
                    }

                    if(ionIsAcceptable)
                    {
                        score = bestScore;
                    }
                    else
                    {
                        score = 1;
                    }
                }
                //Write(isoTheoretical, score, truncatedObservedPeaks, truncatedTheoreticalPeaks);
            }
            else
            {
                score = 1;
            }
            return score;
        }

        private static bool IonIsAcceptable(List<MSPeak> truncatedTheoreticalPeaks, List<MSPeak> truncatedObservedPeaks,IsotopicPeakFitScoreCalculatorSK currentCalculator, out double bestScore)
        {
            bool ionIsAcceptable;
            
            List<MSPeak> truncatedModified1 = new List<MSPeak>();
            List<MSPeak> truncatedModified2 = new List<MSPeak>();
            truncatedModified1 = truncatedTheoreticalPeaks.ToList();
            truncatedModified2 = truncatedTheoreticalPeaks.ToList();

            //plan 1.  we shit the observed profile to fill in the 0 penalty spot and move the penalty spot to the end.  the end should be low anyways because it was not detected
            for(int i=1;i<truncatedModified1.Count;i++)
            {
                truncatedModified1[i - 1] = truncatedModified1[i];
            }
            truncatedModified1[truncatedModified1.Count - 1] = new MSPeak(0,0,0,0);

            //plan 2, insert zero infront of theoretical to move the theoretical we have to higher mass
            truncatedModified2.Insert(0, new MSPeak(0, 0, 0, 0));
            truncatedModified2.RemoveAt(truncatedModified2.Count-1);



            double score1DaLower = currentCalculator.CalculateFitScore(truncatedModified1, truncatedObservedPeaks, ProcessingParameters.IsoParameters.DivideFitScoreByNumberOfIons);
            double score1DaHigher = currentCalculator.CalculateFitScore(truncatedModified2, truncatedObservedPeaks, ProcessingParameters.IsoParameters.DivideFitScoreByNumberOfIons);
            double scoreAsIs = currentCalculator.CalculateFitScore(truncatedTheoreticalPeaks, truncatedObservedPeaks,ProcessingParameters.IsoParameters.DivideFitScoreByNumberOfIons);

            if (score1DaHigher < scoreAsIs || score1DaLower < scoreAsIs)
            {
                ionIsAcceptable = false;
                bestScore = 1;
            }
            else
            {
                ionIsAcceptable = true;
                bestScore = scoreAsIs;
            }
            return ionIsAcceptable;
        }

        private static void Write(IsotopicProfile isoTheoretical, double score, List<MSPeak> observedPeaks,
                                  List<MSPeak> theoreticalPeaks)
        {


            List<double> normalizedObs = new List<double>();
            double normalizeObservedConstant = observedPeaks[0].Height;
            for (int i = 1; i < observedPeaks.Count; i++)
            {
                if (observedPeaks[i].Height > normalizeObservedConstant)
                {
                    normalizeObservedConstant = observedPeaks[i].Height;
                }
            }

            List<double> normalizedTheo = new List<double>();
            double normalizeTheoryConstant = theoreticalPeaks[0].Height;
            for (int i = 1; i < theoreticalPeaks.Count; i++)
            {
                if (theoreticalPeaks[i].Height > normalizeTheoryConstant)
                {
                    normalizeTheoryConstant = theoreticalPeaks[i].Height;
                }
            }

            for (int i = 0; i < observedPeaks.Count; i++)
            {
                normalizedObs.Add(observedPeaks[i].Height / normalizeObservedConstant);
                normalizedTheo.Add(theoreticalPeaks[i].Height / normalizeTheoryConstant);
            }

            
            
            StringListToDisk writer = new StringListToDisk();
            List<string> lines = new List<string>();
            lines.Add(isoTheoretical.MonoIsotopicMass + "," + isoTheoretical.ChargeState + "," + score);
            for (int i = 0; i < theoreticalPeaks.Count; i++)
            {
                lines.Add(i + "," + normalizedTheo[i] + "," + normalizedObs[i]);
            }

            int UniqueName = Convert.ToInt32(Math.Truncate(isoTheoretical.MonoIsotopicMass));
            writer.toDiskStringList(@"X:\FitScore\" + UniqueName + "_" + isoTheoretical.ChargeState + ".txt", lines);
        }

        #endregion

        private List<ProcessedPeak> Processor(List<PNNLOmics.Data.XYData> spectraRaw, int id, ScanObject scans, EnumerationMassSpectraProcessing chooseMethod)
        {
            //blank lists
            List<ProcessedPeak> msPeaks = new List<ProcessedPeak>();
            List<XYData> msXYData = new List<XYData>();

            List<ProcessedPeak> centroidedPeaks;
            List<ProcessedPeak> thresholdedPeaks;
            List<ProcessedPeak> filteredPeaks;
            List<ProcessedPeak> selectedPeaks;
            switch (chooseMethod)
            {
                case EnumerationMassSpectraProcessing.OmicsCentroid_Only:
                    {
                        centroidedPeaks = ProcessingParameters.Engine_OmicsPeakDetection.DiscoverPeaks(spectraRaw);

                        selectedPeaks = centroidedPeaks;
                    }
                    break;



                case EnumerationMassSpectraProcessing.OmicsCentroid_OmicsThreshold:
                    {
                        centroidedPeaks = ProcessingParameters.Engine_OmicsPeakDetection.DiscoverPeaks(spectraRaw);

                        thresholdedPeaks = ProcessingParameters.Engine_OmicsPeakThresholding.ApplyThreshold(centroidedPeaks);

                        selectedPeaks = ReturnCentroidOrThresholded(centroidedPeaks, thresholdedPeaks, ProcessingParameters.Engine_OmicsPeakThresholding.Parameters.SignalToShoulderCuttoff);
                    }
                    break;

                case EnumerationMassSpectraProcessing.OmicsCentroid_OmicsHammer:
                    {
                        Console.WriteLine("Hammer not set up yet ProcesorMassSpectra");
                        Console.ReadKey();
                        //not setup yet
                        centroidedPeaks = ProcessingParameters.Engine_OmicsPeakDetection.DiscoverPeaks(spectraRaw);

                        thresholdedPeaks = ProcessingParameters.Engine_OmicsPeakThresholding.ApplyThreshold(centroidedPeaks);

                        selectedPeaks = ReturnCentroidOrThresholded(centroidedPeaks, thresholdedPeaks, ProcessingParameters.Engine_OmicsPeakThresholding.Parameters.SignalToShoulderCuttoff);
                    }
                    break;

                case EnumerationMassSpectraProcessing.OmicsCentroid_OmicsThreshold_OmicsPeakFilter:
                    {
                        centroidedPeaks = ProcessingParameters.Engine_OmicsPeakDetection.DiscoverPeaks(spectraRaw);

                        thresholdedPeaks = ProcessingParameters.Engine_OmicsPeakThresholding.ApplyThreshold(centroidedPeaks);

                        selectedPeaks = ReturnCentroidOrThresholded(centroidedPeaks, thresholdedPeaks, ProcessingParameters.Engine_OmicsPeakThresholding.Parameters.SignalToShoulderCuttoff);

                        filteredPeaks = Utiliites.FilterByPointsPerSide(selectedPeaks, ProcessingParameters.PointsPerShoulder);

                        selectedPeaks = filteredPeaks;
                    }
                    break;

                default:
                    {
                        centroidedPeaks = ProcessingParameters.Engine_OmicsPeakDetection.DiscoverPeaks(spectraRaw);

                        selectedPeaks = centroidedPeaks;
                    }
                    break;
            }

            return selectedPeaks;
        }


        #region mass spec generation section

        public DeconTools.Backend.XYData DeconMSGeneratorWrapper(Run runIn, ScanSet lcScanSetSelected)
        {
            DeconTools.Backend.XYData deconMassSpecta = ProcessingParameters.Engine_msGenerator.GenerateMS(runIn, lcScanSetSelected);
            return deconMassSpecta;
        }

        #endregion
    }
}
