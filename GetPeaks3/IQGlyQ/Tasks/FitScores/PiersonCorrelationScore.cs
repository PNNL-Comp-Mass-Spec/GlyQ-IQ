using System;
using System.Collections.Generic;
using Run64.Backend.Core;

namespace IQGlyQ.Tasks.FitScores
{
   public class PiersonCorrelationScore : IFitScoring
    {
        public FitScoreOptions FitScoreType { get; set; }

        public ParametersPiersonCorrelation Parameters { get; set; }

        public PiersonCorrelationScore(ParametersPiersonCorrelation parameters)
        {
            Parameters = parameters;
            FitScoreType = parameters.FitScoreType;
        }

        public double CalculateFitScore(List<MSPeak> theorProfile, List<MSPeak> observedProfile)
        {
            var dimension = theorProfile.Count;
            if (dimension == 0 || dimension != observedProfile.Count) return 0.0;
            if (dimension == 1) return 1.0;

            // Compute means
            var m1 = 0.0;
            var m2 = 0.0;

            for (var i = Parameters.InitialIonsToIgnore; i < dimension; i++)
            {
                m1 += theorProfile[i].Height;
                m2 += observedProfile[i].Height;
            }

            m1 /= (dimension - Parameters.InitialIonsToIgnore);
            m2 /= (dimension - Parameters.InitialIonsToIgnore);

            // compute Pearson correlation
            var cov = 0.0;
            var s1 = 0.0;
            var s2 = 0.0;

            for (var i = Parameters.InitialIonsToIgnore; i < dimension; i++)
            {
                var d1 = theorProfile[i].Height - m1;
                var d2 = observedProfile[i].Height - m2;
                cov += d1 * d2;
                s1 += d1 * d1;
                s2 += d2 * d2;
            }

            if (s1 <= 0 || s2 <= 0) return 0;

            double scorePierson = cov < 0 ? 0f : cov/Math.Sqrt(s1*s2);
            return 1-scorePierson;
        }
    }

   public class ParametersPiersonCorrelation : FitScoreParameters
   {
       //extra parameters go here
       public int InitialIonsToIgnore { get; set; }

       public ParametersPiersonCorrelation()
       {
           FitScoreType = FitScoreOptions.PiersonCorrelation;
           InitialIonsToIgnore = 0;
       }

       public ParametersPiersonCorrelation(int initialIonsToIgnore)
           :this()
       {
           InitialIonsToIgnore = initialIonsToIgnore;
       }
   }

    //var dimension = theorProfile.Count - Parameters.InitialIonsToIgnore;
    //        if (dimension == 0 || dimension != observedProfile.Count) return 0.0;
    //        if (dimension == 1) return 1.0;

    //        // Compute means
    //        var m1 = 0.0;
    //        var m2 = 0.0;

    //        for (var i = Parameters.InitialIonsToIgnore; i < dimension; i++)
    //        {
    //            m1 += theorProfile[i].Height;
    //            m2 += observedProfile[i].Height;
    //        }

    //        m1 /= dimension;
    //        m2 /= dimension;

    //        // compute Pearson correlation
    //        var cov = 0.0;
    //        var s1 = 0.0;
    //        var s2 = 0.0;

    //        for (var i = 0; i < dimension; i++)
    //        {
    //            var d1 = theorProfile[i].Height - m1;
    //            var d2 = observedProfile[i].Height - m2;
    //            cov += d1 * d2;
    //            s1 += d1 * d1;
    //            s2 += d2 * d2;
    //        }

    //        if (s1 <= 0 || s2 <= 0) return 0;

    //        double scorePierson = cov < 0 ? 0f : cov/Math.Sqrt(s1*s2);
}
