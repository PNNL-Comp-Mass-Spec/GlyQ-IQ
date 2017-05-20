using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Objects
{
    public class BioFactor
    {
        /// <summary>
        /// how much the ion is upregulaged on average
        /// </summary>
        public double BioFactorChange { get; set; }

        /// <summary>
        /// mass to modify and its 4 isotopes
        /// </summary>
        public double Mass  { get; set; }

        /// <summary>
        /// how wide are the peaks we want to increase
        /// </summary>
        public double WindowAroundMass  { get; set; }

        /// <summary>
        /// factor generated on the normal distribution
        /// </summary>
        public double IntensityMultiplyFactor { get; set; }

        public BioFactor(double bioFactorChange, double mass, double windowAroundMass, double multiplyFactor)
        {
            BioFactorChange = bioFactorChange;
            Mass = mass;
            WindowAroundMass = windowAroundMass;
            IntensityMultiplyFactor = multiplyFactor;
        }

        public enum SetType
        {
            Control,
            Case
        }
    }
}
