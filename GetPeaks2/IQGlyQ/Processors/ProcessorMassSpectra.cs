using System;
using System.Collections.Generic;
using System.Linq;
using GetPeaksDllLite.DataFIFO;
using GetPeaksDllLite.Functions;
using IQ.Backend.Core;
using IQGlyQ.Enumerations;
using IQGlyQ.Objects;
using IQGlyQ.Objects.EverythingIsotope;
using IQGlyQ.Tasks;
using PNNLOmics.Data;
using PNNLOmics.Data.Peaks;
using Run32.Backend.Core;
using XYData =  Run32.Backend.Data.XYData;
using IQGlyQ.Tasks.FitScores;

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
        public List<ProcessedPeak> Execute(Run32.Backend.Data.XYData spectraRaw, EnumerationMassSpectraProcessing chooseMethod)
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
        public double ExecuteInterference(IsotopicProfile iso,  List<Run32.Backend.Core.Peak> mspeakList)
        {
            double score = 0;
            score = ProcessingParameters.Engine_InterferenceScorer.GetInterferenceScore(iso, mspeakList);
            return score;
        }

        /// <summary>
        /// Simple entrance for fit scoreing.  First part here removes low abundant peaks and makes sure the peak lists are the correct length.
        /// </summary>
        /// <param name="isoTheoretical">Theoretical profile, allready normalized to 1</param>
        /// <param name="isoExperimental">Experimental profile, real abundances</param>
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

                //FIT SCORING NOW
                IFitScoring currentCalculator = ProcessingParameters.Engine_FitScoreCalculator;

                int minPeaksToScore = 3;

                if (truncatedObservedPeaks.Count > minPeaksToScore)
                {
                    switch (ProcessingParameters.IsoParameters.IsotopeProfileMode)
                    {
                        case EnumerationIsotopicProfileMode.H:
                            {
                                score = ScoreModule(truncatedObservedPeaks, truncatedTheoreticalPeaks, ref currentCalculator, isoTheoretical.MonoIsotopicMass, isoTheoretical.ChargeState);
                                isoTheoretical.EstablishAlternatvePeakIntensites(0); //locked for future feature finding
                            }
                            break;
                        case EnumerationIsotopicProfileMode.DH:
                            {
                                if (!isoTheoretical.isEstablished)
                                {

                                    //1.  modify currentCalculator or create a new one
                                    IsotopeProfileBlended blendGenerator = new IsotopeProfileBlended(new ParametersBlendedIsotope(ProcessingParameters.ParametersIsotopeGeneration));
                                    int iteratorRange = 10000;

                                    List<int> Offsets = new List<int>();
                                    List<float> ratiosToReference = new List<float>();

                                    //2  create range to score over
                                    List<Tuple<double, float>> scores = new List<Tuple<double, float>>();

                                    //take penalty peak off in advance of multiplexing
                                    IsotopicProfile localIsotopeProfile = isoTheoretical.CloneIsotopicProfile();


                                    for (int i = 0; i < ProcessingParameters.IsoParameters.NumberOfPeaksToLeftForPenalty; i++)
                                    {
                                        localIsotopeProfile.Peaklist.RemoveAt(0);
                                        float abundanceToRemove = localIsotopeProfile.AlternatePeakIntensities[0];
                                        List<float> localList = localIsotopeProfile.AlternatePeakIntensities.ToList();
                                        localList.RemoveAt(0);

                                        localIsotopeProfile.SetAlternatePeakIntensities(localList.ToArray());
                                    }

                                    ParametersIsoShift bringInParameters = new ParametersIsoShift(ProcessingParameters.IsoParameters.PenaltyMode, ProcessingParameters.IsoParameters.NumberOfPeaksToLeftForPenalty);
                                    IUpgradeIsotope shifter = new UpgradeIsotopeShift(bringInParameters);

                                    for (int i = 0; i <= iteratorRange; i++)
                                    {
                                        //3.  create isotheroetricals

                                        float ratioOfinterest = (float)i / (float)iteratorRange;

                                        Offsets = new[] { 1 }.ToList();
                                        ratiosToReference = new[] { ratioOfinterest }.ToList();

                                        IsotopicProfile blendedProfile = blendGenerator.GeneratorFromExisting(localIsotopeProfile, ref Offsets, ref ratiosToReference);
                                        blendedProfile.RefreshAlternatePeakIntenstiesFromPeakList();

                                        //4.  Add penalty spot

                                        shifter.UpgradeMe(ref blendedProfile);

                                        //4.  truncate properly
                                        int extraPeaksRange = blendedProfile.Peaklist.Count - truncatedObservedPeaks.Count;
                                        blendedProfile.Peaklist.RemoveRange(truncatedObservedPeaks.Count, extraPeaksRange);
                                        //this is not correct yet

                                        List<MSPeak> localTruncatedTheoreticalPeaks = blendedProfile.Peaklist;

                                        double tempscore = ScoreModule(truncatedObservedPeaks, localTruncatedTheoreticalPeaks, ref currentCalculator, localIsotopeProfile.MonoIsotopicMass, localIsotopeProfile.ChargeState);
                                        var result = new Tuple<double, float>(tempscore, ratioOfinterest);
                                        scores.Add(result);
                                    }

                                    //4.  select best score
                                    List<Tuple<double, float>> sortedResults = scores.OrderBy(var => var.Item1).ToList();
                                    var bestScore = sortedResults.FirstOrDefault();
                                    Console.WriteLine("The Lowest Score is " + bestScore.Item1 + " and goes wiht a mixing fraction of " + bestScore.Item2 + Environment.NewLine + " D/H: " + bestScore.Item2 / 1);

                                    score = bestScore.Item1;
                                    //5.  store best ration and best fit score

                                    //store the bestisotope profile as a model for future fits (parents etc.)
                                    Offsets = new[] { 1 }.ToList();
                                    ratiosToReference = new[] { bestScore.Item2 }.ToList();

                                    IsotopicProfile bestblendedProfile = blendGenerator.GeneratorFromExisting(localIsotopeProfile, ref Offsets, ref ratiosToReference);
                                    shifter.UpgradeMe(ref bestblendedProfile);
                                    bestblendedProfile.RefreshAlternatePeakIntenstiesFromPeakList();

                                    isoTheoretical.SetAlternatePeakIntensities(bestblendedProfile.Peaklist);
                                    isoTheoretical.EstablishAlternatvePeakIntensites(bestScore.Item2); //locked for future feature finding  
                                }
                                else
                                {
                                    //IsotopicProfile localTheoreticalProfile = isoTheoretical.CloneIsotopicProfile();
                                    //localTheoreticalProfile.Peaklist = truncatedTheoreticalPeaks;
                                    //localTheoreticalProfile.UpdatePeakListWithAlternatePeakIntensties();

                                    //score = ScoreModule(truncatedObservedPeaks, localTheoreticalProfile.Peaklist, ref currentCalculator, localTheoreticalProfile);

                                    score = ScoreModule(truncatedObservedPeaks, truncatedTheoreticalPeaks, ref currentCalculator, isoTheoretical.MonoIsotopicMass, isoTheoretical.ChargeState);

                                    //this not what we want here
                                    //isoTheoretical.EstablishAlternatvePeakIntensites(0); //locked for future feature finding
                                }
                            }

                            break;
                        case EnumerationIsotopicProfileMode.Unknown:
                            {
                                score = ScoreModule(truncatedObservedPeaks, truncatedTheoreticalPeaks, ref currentCalculator, isoTheoretical.MonoIsotopicMass, isoTheoretical.ChargeState);

                            }
                            break;
                        default:
                            {
                                score = ScoreModule(truncatedObservedPeaks, truncatedTheoreticalPeaks, ref currentCalculator, isoTheoretical.MonoIsotopicMass, isoTheoretical.ChargeState);
                            }
                            break;
                    }
                }
                else
                {
                    score = 2;
                }
            }
            else
            {
                score = 1;
            }
            return score;
        }

        /// <summary>
        /// both truncatedObservedPeaks and truncatedTheoreticalPeaks must be equal
        /// </summary>
        /// <param name="truncatedObservedPeaks"></param>
        /// <param name="truncatedTheoreticalPeaks"></param>
        /// <param name="currentCalculator"></param>
        /// <param name="fullTheoreticalProfile"></param>
        /// <returns></returns>
        private static double ScoreModule(List<MSPeak> truncatedObservedPeaks, List<MSPeak> truncatedTheoreticalPeaks, ref IFitScoring currentCalculator, double monoMassToPrint, int chargestateToPrint)
        {
            double score;

            //base for testing multiple fit scores.  true will do multi scoring
            bool singleScore = true;
            //testing for multiple D/H ratios
            bool multiIntensity = false;
            //testing for mass to the left and right +1da - 1 Da
            bool multiMass = true;

            if (singleScore)
            {
                score = currentCalculator.CalculateFitScore(truncatedTheoreticalPeaks, truncatedObservedPeaks);
            }
            else
            {
                bool ionIsAcceptable = false;
                double bestScore = 1;

                if (multiMass)
                {
                    ionIsAcceptable = IonIsAcceptable(truncatedTheoreticalPeaks, truncatedObservedPeaks, ref currentCalculator, out bestScore);
                }

                score = ionIsAcceptable ? bestScore : 1;
            }


            //Console.WriteLine("Fit Score is " + score);

            bool writeScoreAndProfile = false;
            if (writeScoreAndProfile)
            {
                Write(monoMassToPrint, chargestateToPrint, score, truncatedObservedPeaks, truncatedTheoreticalPeaks);
            }
            return score;
        }

        private static bool IonIsAcceptable(List<MSPeak> truncatedTheoreticalPeaks, List<MSPeak> truncatedObservedPeaks, ref IFitScoring currentCalculator, out double bestScore)
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

            double score1DaLower = currentCalculator.CalculateFitScore(truncatedModified1, truncatedObservedPeaks);
            double score1DaHigher = currentCalculator.CalculateFitScore(truncatedModified2, truncatedObservedPeaks);
            double scoreAsIs = currentCalculator.CalculateFitScore(truncatedTheoreticalPeaks, truncatedObservedPeaks);


            //double score1DaLower = currentCalculator.CalculateFitScore(truncatedModified1, truncatedObservedPeaks, ProcessingParameters.IsoParameters.DivideFitScoreByNumberOfIons);
            //double score1DaHigher = currentCalculator.CalculateFitScore(truncatedModified2, truncatedObservedPeaks, ProcessingParameters.IsoParameters.DivideFitScoreByNumberOfIons);
            //double scoreAsIs = currentCalculator.CalculateFitScore(truncatedTheoreticalPeaks, truncatedObservedPeaks,ProcessingParameters.IsoParameters.DivideFitScoreByNumberOfIons);

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

        private static void Write(double mass, int chargestate , double score, List<MSPeak> observedPeaks, List<MSPeak> theoreticalPeaks)
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
            lines.Add(mass + "," + chargestate + "," + score);
            for (int i = 0; i < theoreticalPeaks.Count; i++)
            {
                lines.Add(i + "," + normalizedTheo[i] + "," + normalizedObs[i]);
            }

            int UniqueName = Convert.ToInt32(Math.Truncate(mass));
            writer.toDiskStringList(@"X:\FitScore\" + UniqueName + "_" + chargestate + ".csv", lines);
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
                        Console.WriteLine("Hammer not set up yet ProcesorMassSpectra in ProcessorMassSpectra.Processor");
                        System.Threading.Thread.Sleep(3000);
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

        public Run32.Backend.Data.XYData DeconMSGeneratorWrapper(Run runIn, ScanSet lcScanSetSelected)
        {
            Run32.Backend.Data.XYData deconMassSpecta = ProcessingParameters.Engine_msGenerator.GenerateMS(runIn, lcScanSetSelected);
            return deconMassSpecta;
        }

        #endregion

       
    }
}
