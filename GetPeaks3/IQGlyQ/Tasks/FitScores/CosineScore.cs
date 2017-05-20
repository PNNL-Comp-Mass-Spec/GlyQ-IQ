using System;
using System.Collections.Generic;
using Run64.Backend.Core;

namespace IQGlyQ.Tasks.FitScores
{
    class CosineScore: IFitScoring
    {
        public FitScoreOptions FitScoreType { get; set; }

        public ParametersCosineFit Parameters { get; set; }

        public CosineScore(ParametersCosineFit parameters)
        {
            Parameters = parameters;
            FitScoreType = parameters.FitScoreType;
        }

        // the larger the better
        public double CalculateFitScore(List<MSPeak> theorProfile, List<MSPeak> observedProfile)
        {
            if (theorProfile.Count != observedProfile.Count || observedProfile.Count == 0) return 0;

            var innerProduct = 0.0;
            var magnitudeTheo = 0.0;
            var magnitudeObs = 0.0;
            for (var i = Parameters.InitialIonsToIgnore; i < theorProfile.Count; i++)
            {
                var theo = theorProfile[i].Height;
                var obs = observedProfile[i].Height;
                innerProduct += theo * obs;
                magnitudeTheo += theo * theo;
                magnitudeObs += obs * obs;
            }

            double scoreCos = innerProduct/Math.Sqrt(magnitudeTheo*magnitudeObs);
            return 1 - scoreCos;//so we behave like fit scores
        }
        
    }

    public class ParametersCosineFit : FitScoreParameters
    {
        public int InitialIonsToIgnore { get; set; }

        public ParametersCosineFit()
        {
            FitScoreType = FitScoreOptions.Cosine;
            InitialIonsToIgnore = 0;
        }

        public ParametersCosineFit(int initialIonsToIgnore)
            :this()
        {
            InitialIonsToIgnore = initialIonsToIgnore;
        }

    }
}
