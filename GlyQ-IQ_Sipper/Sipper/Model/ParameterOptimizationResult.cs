using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sipper.Model
{
    public class ParameterOptimizationResult
    {

        #region Constructors


        #endregion

        #region Properties


        public double FitScoreLabelled { get; set; }
        public double SumOfRatios { get; set; }
        public double Iscore { get; set; }
        public int ContigScore { get; set; }
        public double PercentIncorp { get; set; }
        public double PercentPeptidePopulation { get; set; }
        public int NumUnlabelledPassingFilter { get; set; }
        public int NumLabeledPassingFilter { get; set; }

        public double FalsePositiveRate
        {
            get
            {
                if (NumLabeledPassingFilter>0 || NumUnlabelledPassingFilter>0)
                {
                    return (NumUnlabelledPassingFilter/(double) (NumUnlabelledPassingFilter + NumLabeledPassingFilter));
                }

                return double.NaN;
            }
        }



        public string ToStringWithDetails(char delimiter='\t')
        {
            var sb = new StringBuilder();
            sb.Append(FitScoreLabelled);
            sb.Append(delimiter);
            sb.Append(SumOfRatios);
            sb.Append(delimiter);
            sb.Append(Iscore);
            sb.Append(delimiter);
            sb.Append(ContigScore);
            sb.Append(delimiter);
            sb.Append(PercentIncorp);
            sb.Append(delimiter);
            sb.Append(PercentPeptidePopulation);
            sb.Append(delimiter);
            sb.Append(NumUnlabelledPassingFilter);
            sb.Append(delimiter);
            sb.Append(NumLabeledPassingFilter);
            sb.Append(delimiter);
            sb.Append(FalsePositiveRate.ToString("0.###"));

            return sb.ToString();

        }


        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion

    }
}
