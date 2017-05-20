using System;
using System.Collections.Generic;
using Run64.Backend.Core;

namespace IQGlyQ.Tasks.FitScores
{
    public class LeastSquareScore : IFitScoring
    {
        public FitScoreOptions FitScoreType { get; set; }

        public ParametersLeastSquares Parameters { get; set; }

        LeastSquareScore()
        {
            Parameters = new ParametersLeastSquares();
        }

        public LeastSquareScore(ParametersLeastSquares parameters):this()
        {
            Parameters = parameters;
            FitScoreType = parameters.FitScoreType;
        }

        public double CalculateFitScore(List<MSPeak> theorProfile, List<MSPeak> observedProfile)
        {
            int lengthTheory = theorProfile.Count;
            int lengthObserved = observedProfile.Count;

            if (lengthTheory != lengthObserved)
            {
                Console.WriteLine("Isotope profiles are different lengths in LeastSquareScore");
                System.Threading.Thread.Sleep(3000);
            }

            PeakLeastSquaresFitterSK peakFitter = new PeakLeastSquaresFitterSK();
            //double fitval = peakFitter.GetFit(theorPeakList, observedPeakList, minCuttoffTheorPeakIntensityFraction, massErrorPPMBetweenPeaks, numberOfPeaksToLeftForPenalty);//0.1
            //double fitval = peakFitter.GetFit(theorProfile, observedProfile);//0.1
            //double fitval = peakFitter.GetWeightedFit(theorProfile, observedProfile);//0.1


            double fitval = peakFitter.GetPenaltyFit(theorProfile, observedProfile, Parameters.UseIsotopePeakCountCorrection);//0.1)
            if (double.IsNaN(fitval) || fitval > 1) fitval = 1;



            return fitval;
        }
    }

    public class ParametersLeastSquares : FitScoreParameters
    {
        //extra parameters go here
        public bool UseIsotopePeakCountCorrection { get; set; }

        public ParametersLeastSquares()
        {
            Initialize();

            UseIsotopePeakCountCorrection = true;
        }

        public ParametersLeastSquares(bool useIsotopePeakCountCorrection)
        {
            Initialize();

            UseIsotopePeakCountCorrection = useIsotopePeakCountCorrection;
        }

        private void Initialize()
        {
            FitScoreType = FitScoreOptions.LeastSquares;
        }
    }
}
