using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HammerPeakDetector.Comparisons
{
    public class ROCrates
    {
        public void Calculate(ref ROCobject ROCresults)
        {
            double TN = Convert.ToDouble(ROCresults.TrueNegatives.Count);
            double FP = Convert.ToDouble(ROCresults.FalsePositives.Count);
            double TP = Convert.ToDouble(ROCresults.TruePositives.Count);
            double FN = Convert.ToDouble(ROCresults.FalseNegatives.Count);

            ROCresults.Specificity = TN / (FP + TN);

            ROCresults.Sensitivity = TP / (TP + FN);

            ROCresults.FalsePositiveRate = FP / (FP + TN);

            ROCresults.Accuracy = (TP + TN) / (TP + TN + FP + FN);

            ROCresults.PercentCorrect = (TP / (TP + FP)) * 100; //PercentCorrect finds what percentage of the data labelled
            //as a mono or peak truly is a mono or peak
        }
    }
}
