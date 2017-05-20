using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQGlyQ.Objects
{
    public class ScanCharacteristics
    {
        /// <summary>
        /// center LC scan
        /// </summary>
        public int CenterScan { get; set; }
        
        /// <summary>
        /// MS abundance associated with max integrated abundance
        /// </summary>
        public float MSAbundance { get; set; }

        /// <summary>
        /// max integrated Abundance
        /// </summary>
        public float MaxIntegratedAbundance { get; set; }

        /// <summary>
        /// fit score associated with max integrated abundance
        /// </summary>
        public double FitScoreFromMaxAbundance { get; set; }

        /// <summary>
        /// LM fit score associated with max integrated abundance
        /// </summary>
        public double LMFitFromMaxAbundance { get; set; }

        public int ClosestLowerScan { get; set; }
        public int ClosestUpperScan { get; set; }

        public bool isFirstScan { get; set; }
        public bool isLastScan { get; set; }

        public bool isConsolidated { get; set; }

        public List<int> scansToConsolidate { get; set; }

        public double ChargeStateCorrelation { get; set; }

        public ScanCharacteristics(int centerScan, float integratedAbunance, double fitscore, double LMfit, float msAbundance)
        {
            CenterScan = centerScan;
            MaxIntegratedAbundance = integratedAbunance;
            FitScoreFromMaxAbundance = fitscore;
            LMFitFromMaxAbundance = LMfit;
            isFirstScan = false;
            isLastScan = false;
            isConsolidated = false;
            scansToConsolidate = new List<int>();
            MSAbundance = msAbundance;
        }
    }
}
