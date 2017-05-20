using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using PNNLOmics.Data.Peaks;

namespace HammerPeakDetector.Comparisons
{
    public class ROCobject
    {
        public ROCobject()
        {
            TruePositives = new SortedDictionary<int, ProcessedPeak>();
            FalsePositives = new SortedDictionary<int, ProcessedPeak>();
            TrueNegatives = new SortedDictionary<int, ProcessedPeak>();
            FalseNegatives = new SortedDictionary<int, ProcessedPeak>();
        }

        public SortedDictionary<int, ProcessedPeak> TruePositives { get; set; }  //Correct Hit

        public SortedDictionary<int, ProcessedPeak> FalsePositives { get; set; } //False Hit

        public SortedDictionary<int, ProcessedPeak> TrueNegatives { get; set; } // Correct Miss

        public SortedDictionary<int, ProcessedPeak> FalseNegatives { get; set; } // False Miss

        /// <summary>
        /// False Postive Rate (TPR)
        /// </summary>
        public double FalsePositiveRate { get; set; }

        /// <summary>
        /// Sensitivity (TPR) True Positive Rate
        /// </summary>
        public double Sensitivity { get; set; }

        /// <summary>
        /// Specificity (SPC) or True Negative Rate
        /// </summary>
        public double Specificity { get; set; }

        /// <summary>
        /// Accuracy
        /// </summary>
        public double Accuracy { get; set; }


        public double PercentCorrect { get; set; }
    }
}
