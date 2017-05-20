using System;
using System.Collections.Generic;
using IQ.Backend.Core;
using Run32.Backend.Core;

namespace IQGlyQ.Tasks.FitScores
{
    /// <summary>
    /// All fit score calculators need to calculate a fit score if this interfaceis implemented
    /// </summary>
    public interface IFitScoring
    {
        double CalculateFitScore(List<MSPeak> theorProfile, List<MSPeak> observedProfile);
    }

    /// <summary>
    /// optional fitscores are listed here
    /// </summary>
    public enum FitScoreOptions
    {
        LeastSquares,
        PiersonCorrelation,
        Cosine
    }

    /// <summary>
    /// here is where we make fitscore calcuators of different types
    /// </summary>
    public static class FitScoreFactory
    {
        public static IFitScoring Build(FitScoreParameters parameters)
        {
            IFitScoring scoreCalcuator;
            switch (parameters.FitScoreType)
            {
                case FitScoreOptions.LeastSquares:
                    {
                        scoreCalcuator = new LeastSquareScore((ParametersLeastSquares)parameters);
                    }
                    break;
                case FitScoreOptions.PiersonCorrelation:
                    {
                        scoreCalcuator = new PiersonCorrelationScore((ParametersPiersonCorrelation)parameters);
                    }
                    break;
                case FitScoreOptions.Cosine:
                    {
                        scoreCalcuator = new CosineScore((ParametersCosineFit)parameters);
                    }
                    break;
                default:
                    {
                        scoreCalcuator = new LeastSquareScore((ParametersLeastSquares)parameters);
                    }
                    break;
            }

            return scoreCalcuator;
        }
    }

    /// <summary>
    /// here is where we make fitscore calcuators of different types
    /// </summary>
    public static class FitScoreParameterFactory
    {
        public static FitScoreParameters Build()
        {
            return new ParametersLeastSquares();
        }

        public static FitScoreParameters Build(FitScoreOptions parameters)
        {
            FitScoreParameters scoreParameters;
            switch (parameters)
            {
                case FitScoreOptions.LeastSquares:
                    {
                        scoreParameters = new ParametersLeastSquares();
                    }
                    break;
                case FitScoreOptions.PiersonCorrelation:
                    {
                        scoreParameters = new ParametersPiersonCorrelation();
                    }
                    break;
                case FitScoreOptions.Cosine:
                    {
                        scoreParameters = new ParametersCosineFit();
                    }
                    break;
                default:
                    {
                        scoreParameters = Build();
                    }
                    break;
            }

            return scoreParameters;
        }
    }

    /// <summary>
    /// a base class for all fit score parameters.  each new fit score needs to inherit these and must have an enumerated type
    /// </summary>
    public abstract class FitScoreParameters
    {
        internal FitScoreOptions FitScoreType { get; set; }
    }
}
