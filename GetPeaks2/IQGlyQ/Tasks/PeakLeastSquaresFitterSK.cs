using System;
using System.Collections.Generic;
using GetPeaksDllLite.Functions;
using PNNLOmics.Data.Constants;
using IQ.Backend.ProcessingTasks.FitScoreCalculators;
using Run32.Backend.Core;

namespace IQGlyQ.Tasks
{
    public class PeakLeastSquaresFitterSK:LeastSquaresFitter
    {

        #region Constructors
        #endregion

        #region Properties

        #endregion

        #region Public Methods


        /// <summary>
        /// The theorPeakList and obsPeakList must be a 1:1 correspondence.  theoryetical list can look like 0,1,0,1,0,1,1,1,1,1   the first 0 is for 1Da errors, the seccond 0 is for first 2x reporter and the third zero is for the second 2x reporter
        /// </summary>
        /// <param name="theorPeakList"></param>
        /// <param name="observedPeakList"></param>
        /// <param name="minIntensityForScore"></param>
        /// <param name="toleranceInPPM"></param>
        /// <param name="numPeaksToTheLeftForScoring"></param>
        /// <returns></returns>
        public double GetFit(List<MSPeak>theorPeakList,List<MSPeak>observedPeakList)
        {
            //List<double> theorIntensitiesUsedInCalc = new List<double>();
            //var observedIntensitiesUsedInCalc = new List<double>();
           //step 1 normalize distributions

            List<double> normalizedObs = new List<double>();
            double normalizeObservedConstant = observedPeakList[0].Height;
            for (int i = 1; i < observedPeakList.Count; i++)
            {
                if (observedPeakList[i].Height > normalizeObservedConstant)
                {
                    normalizeObservedConstant = observedPeakList[i].Height;
                }
            }

            List<double> normalizedTheo = new List<double>();
            double normalizeTheoryConstant = theorPeakList[0].Height;
            for (int i = 1; i < theorPeakList.Count; i++)
            {
                if (theorPeakList[i].Height > normalizeTheoryConstant)
                {
                    normalizeTheoryConstant = theorPeakList[i].Height;
                }
            }

            for (int i = 0; i < observedPeakList.Count; i++)
            {
                normalizedObs.Add(observedPeakList[i].Height / normalizeObservedConstant);
                normalizedTheo.Add(theorPeakList[i].Height / normalizeTheoryConstant);
            }


            //step 2 least squares

            //first gather all the intensities from theor and obs peaks

            //int indexMaxTheor = 0;
            //double maxTheorIntensity = double.MinValue;
            //for (int i = 0; i < theorPeakList.Count; i++)
            //{
            //    if (theorPeakList[i].Height>maxTheorIntensity)
            //    {
            //        maxTheorIntensity = theorPeakList[i].Height;
            //        indexMaxTheor = i;

            //    }
            //}

            //for (int index = 0; index < theorPeakList.Count; index++)
            //{
            //    var peak = theorPeakList[index];

            //    bool overrideMinIntensityCutoff = index < numPeaksToTheLeftForScoring;

            //    if (peak.Height > minIntensityForScore || overrideMinIntensityCutoff)
            //    {
            //        theorIntensitiesUsedInCalc.Add(peak.Height);

            //        DeconTools.Backend.Utilities.IqLogger.IqLogger.SamPayneLog("Theoretical Peak Selected!	Peak Height: " + peak.Height + " Peak X-Value: " + peak.XValue);

            //        //find peak in obs data
            //        double mzTolerance = toleranceInPPM*peak.XValue/1e6;
            //        var foundPeaks = PeakUtilities.GetPeaksWithinTolerance(observedPeakList, peak.XValue, mzTolerance);

            //        double obsIntensity;
            //        if (foundPeaks.Count == 0)
            //        {
            //            DeconTools.Backend.Utilities.IqLogger.IqLogger.SamPayneLog("No Observed Peaks Found Within Tolerance" + Environment.NewLine);
            //            obsIntensity = 0;
            //        }
            //        else if (foundPeaks.Count == 1)
            //        {
            //            obsIntensity = foundPeaks.First().Height;
            //            DeconTools.Backend.Utilities.IqLogger.IqLogger.SamPayneLog("Observed Peak Selected!	Peak Height: " + foundPeaks[0].Height + " Peak X-Value " + foundPeaks[0].XValue + Environment.NewLine);
            //        }
            //        else
            //        {
            //            obsIntensity = foundPeaks.OrderByDescending(p => p.Height).First().Height;
            //            DeconTools.Backend.Utilities.IqLogger.IqLogger.SamPayneLog("Observed Peak Selected!	Peak Height: " + foundPeaks[0].Height + " Peak X-Value " + foundPeaks[0].XValue + Environment.NewLine);
            //        }

            //        observedIntensitiesUsedInCalc.Add(obsIntensity);
            //    }
            //    else
            //    {
            //        DeconTools.Backend.Utilities.IqLogger.IqLogger.SamPayneLog("Theoretical Peak Not Selected!	Peak Height: " + peak.Height + " Peak X-Value: " + peak.XValue + Environment.NewLine);
            //    }
            //}

            ////the minIntensityForScore is too high and no theor peaks qualified. This is bad. But we don't
            ////want to throw errors here
            //if (theorIntensitiesUsedInCalc.Count == 0)
            //{
            //    DeconTools.Backend.Utilities.IqLogger.IqLogger.SamPayneLog("No peaks meet minIntensityForScore." + Environment.NewLine);
            //    return 1.0;
            //}

            //double maxObs = observedIntensitiesUsedInCalc.Max();
            //if (Math.Abs(maxObs - 0) < float.Epsilon) maxObs = double.PositiveInfinity;
            //DeconTools.Backend.Utilities.IqLogger.IqLogger.SamPayneLog("Max Observed Intensity: " + maxObs);

            //List<double> normalizedObs = observedIntensitiesUsedInCalc.Select(p => p / maxObs).ToList();

            //double maxTheor = theorIntensitiesUsedInCalc.Max();
            //List<double> normalizedTheo = theorIntensitiesUsedInCalc.Select(p => p / maxTheor).ToList();
            //DeconTools.Backend.Utilities.IqLogger.IqLogger.SamPayneLog("Max Theoretical Intensity: " + maxTheor + Environment.NewLine);


            //foreach (var val in normalizedObs)
            //{
            //    Console.WriteLine(val);
            //}

            //Console.WriteLine();
            //foreach (var val in normalizedTheo)
            //{
            //    Console.WriteLine(val);
            //}


            double sumSquareOfDiffs = 0;
            double sumSquareOfTheor = 0;
            for (int i = 0; i < normalizedTheo.Count; i++)
            {
                double diff = normalizedObs[i] - normalizedTheo[i];

                sumSquareOfDiffs += (diff*diff);
                sumSquareOfTheor += (normalizedTheo[i]*normalizedTheo[i]);
				//DeconTools.Backend.Utilities.IqLogger.IqLogger.SamPayneLog("Normalized Observed: " + normalizedObs[i]);
				//DeconTools.Backend.Utilities.IqLogger.IqLogger.SamPayneLog("Normalized Theoretical: " + normalizedTheo[i]);
				//DeconTools.Backend.Utilities.IqLogger.IqLogger.SamPayneLog("Iterator: " + i + " Sum of Squares Differences: " + sumSquareOfDiffs + " Sum of Squares Theoretical: " + sumSquareOfTheor + Environment.NewLine);
            }

            double fitScore = sumSquareOfDiffs/sumSquareOfTheor;
            if (double.IsNaN(fitScore) || fitScore > 1) fitScore = 1;

            return fitScore;
        }


        /// <summary>
        /// The theorPeakList and obsPeakList must be a 1:1 correspondence.  theoryetical list can look like 0,1,0,1,0,1,1,1,1,1   the first 0 is for 1Da errors, the seccond 0 is for first 2x reporter and the third zero is for the second 2x reporter
        /// </summary>
        /// <param name="theorPeakList"></param>
        /// <param name="observedPeakList"></param>
        /// <param name="minIntensityForScore"></param>
        /// <param name="toleranceInPPM"></param>
        /// <param name="numPeaksToTheLeftForScoring"></param>
        /// <returns></returns>
        public double GetWeightedFit(List<MSPeak> theorPeakList, List<MSPeak> observedPeakList)
        {
            //List<double> theorIntensitiesUsedInCalc = new List<double>();
            //var observedIntensitiesUsedInCalc = new List<double>();
            //step 1 normalize distributions

            List<double> normalizedObs = new List<double>();
            double normalizeObservedConstant = observedPeakList[0].Height;
            for (int i = 1; i < observedPeakList.Count; i++)
            {
                if (observedPeakList[i].Height > normalizeObservedConstant)
                {
                    normalizeObservedConstant = observedPeakList[i].Height;
                }
            }

            List<double> normalizedTheo = new List<double>();
            double normalizeTheoryConstant = theorPeakList[0].Height;
            for (int i = 1; i < theorPeakList.Count; i++)
            {
                if (theorPeakList[i].Height > normalizeTheoryConstant)
                {
                    normalizeTheoryConstant = theorPeakList[i].Height;
                }
            }

            for (int i = 0; i < observedPeakList.Count; i++)
            {
                normalizedObs.Add(observedPeakList[i].Height / normalizeObservedConstant);
                normalizedTheo.Add(theorPeakList[i].Height / normalizeTheoryConstant);
            }


            //step 2 calculate differences and weight by theoretical

            //step 1 is to find the total area of the theoretical distribution and all penalty ions
            double penaltyFactor = 1;//how much we penalize for a rougue ion. this needs to bump the 1Da ions past the threshold

            double sumtotalVolume = 0;//this is the sum of all the theoretical peaks + all penalties

            for (int i = 0; i < normalizedTheo.Count; i++)
            {
                double penalty = 0;
                double theoreticalPeakHeight = normalizedTheo[i];

                if (theoreticalPeakHeight == 0)
                {
                    penalty = penaltyFactor; //when there is a rogue ion present
                }
                sumtotalVolume = sumtotalVolume + theoreticalPeakHeight + penalty;
            }

            //step 2.  calculate weights.  we want larger effects for deviations in the most abundant ions.  the top of the distribution is critical
            double sumcontribution = 0;
            
            for (int i = 0; i < normalizedTheo.Count; i++)
            {
                double penalty = 0;

                double theoreticalPeakHeight = normalizedTheo[i];

                if (theoreticalPeakHeight == 0)
                {
                    penalty = penaltyFactor;//when there is a rogue ion present
                }

                double diff = Math.Abs(normalizedObs[i] - theoreticalPeakHeight);//do we want diff or diff squared

                double weight = (theoreticalPeakHeight + penalty) / sumtotalVolume;  //we divide by the total volume so all weights add up to 1
                
                double contributionToError = diff * weight;

                sumcontribution += contributionToError;
            }

            double fitScore = sumcontribution;
            if (double.IsNaN(fitScore) || fitScore > 1) fitScore = 1;

            return fitScore;
        }

        /// <summary>
        /// The theorPeakList and obsPeakList must be a 1:1 correspondence.  theoryetical list can look like 0,1,0,1,0,1,1,1,1,1   the first 0 is for 1Da errors, the seccond 0 is for first 2x reporter and the third zero is for the second 2x reporter
        /// </summary>
        /// <param name="theorPeakList"></param>
        /// <param name="observedPeakList"></param>
        /// <param name="minIntensityForScore"></param>
        /// <param name="toleranceInPPM"></param>
        /// <param name="numPeaksToTheLeftForScoring"></param>
        /// <returns></returns>
        public double GetPenaltyFit(List<MSPeak> theorPeakList, List<MSPeak> observedPeakList, bool usePeakNumberCorrection)
        {
            //List<double> theorIntensitiesUsedInCalc = new List<double>();
            //var observedIntensitiesUsedInCalc = new List<double>();
            //step 1 normalize distributions

            List<double> normalizedObs = new List<double>();
            double normalizeObservedConstant = observedPeakList[0].Height;
            for (int i = 1; i < observedPeakList.Count; i++)
            {
                if (observedPeakList[i].Height > normalizeObservedConstant)
                {
                    normalizeObservedConstant = observedPeakList[i].Height;
                }
            }

            List<double> normalizedTheo = new List<double>();
            double normalizeTheoryConstant = theorPeakList[0].Height;
            for (int i = 1; i < theorPeakList.Count; i++)
            {
                if (theorPeakList[i].Height > normalizeTheoryConstant)
                {
                    normalizeTheoryConstant = theorPeakList[i].Height;
                }
            }

            for (int i = 0; i < observedPeakList.Count; i++)
            {
                normalizedObs.Add(observedPeakList[i].Height / normalizeObservedConstant);
                normalizedTheo.Add(theorPeakList[i].Height / normalizeTheoryConstant);
            }


            //step 2 calculate differences and weight by theoretical

            //step 1 is to find the total area of the theoretical distribution and all penalty ions
            double penaltyFactor = 1;//how much we penalize for a rougue ion. this needs to bump the 1Da ions past the threshold

            double sumtotalVolume = 0;//this is the sum of all the theoretical peaks + all penalties

            double sumSquareOfDiffs = 0;
            double sumSquareOfTheor = 0;
            for (int i = 0; i < normalizedTheo.Count; i++)
            {
                double diff = normalizedObs[i] - normalizedTheo[i];

                sumSquareOfDiffs += 100*(diff * diff);
                sumSquareOfTheor += (normalizedTheo[i] * normalizedTheo[i]);
                //DeconTools.Backend.Utilities.IqLogger.IqLogger.SamPayneLog("Normalized Observed: " + normalizedObs[i]);
                //DeconTools.Backend.Utilities.IqLogger.IqLogger.SamPayneLog("Normalized Theoretical: " + normalizedTheo[i]);
                //DeconTools.Backend.Utilities.IqLogger.IqLogger.SamPayneLog("Iterator: " + i + " Sum of Squares Differences: " + sumSquareOfDiffs + " Sum of Squares Theoretical: " + sumSquareOfTheor + Environment.NewLine);
            }

            bool useDivisor = usePeakNumberCorrection;
            double divisorFactor = 1;//this is default decon tools but is strongly depenent on number of peaks included
            //divisorFactor = normalizedTheo.Count - 2;//the minus 2 takes in to acount the max peak =1 and one real variability

            //simple N divisor
            divisorFactor = normalizedTheo.Count;//nice and easy, based on data used in fit score

            //averagine based N divisor
            bool usePowerNCalculationBasedOnAveragine = false;
            if (usePowerNCalculationBasedOnAveragine)
            {
                double coeff = 0.06486; //glycan averagine
                double power = 0.55419; //glycan averagine
                //double coeff = 0.07257;//peptide averagine
                //double power = 0.54743;//peptide averagine
                int charge = Convert.ToInt32(Math.Round(1/(theorPeakList[1].XValue - theorPeakList[0].XValue)));
                double mono = ConvertMzToMono.Execute(theorPeakList[1].XValue, charge, Constants.SubAtomicParticles[SubAtomicParticleName.Proton].MassMonoIsotopic);
                divisorFactor = coeff*Math.Pow(mono, power); //index 1 is the mono because the penalty is at 0
            }

            if(divisorFactor<1)//prevents divide by zero
            {
                divisorFactor = 1;
            }

            double fitScore = sumSquareOfDiffs / sumSquareOfTheor;

            if (useDivisor)
            {
                fitScore = fitScore/divisorFactor; //new add on to take into account multiple peaks
            }

            if (double.IsNaN(fitScore) || fitScore > 1) fitScore = 1;


            return fitScore;
        }
        #endregion

        #region Private Methods

        #endregion

    }
}
