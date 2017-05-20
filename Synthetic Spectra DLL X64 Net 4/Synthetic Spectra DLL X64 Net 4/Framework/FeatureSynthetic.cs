using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Synthetic_Spectra_DLL_X64_Net_4.Framework
{
    public class FeatureSynthetic : Feature
    {
        //public Int64 MassMonoisotopicD { get; set; }
        //public double AbundanceD { get; set; }

        private Int64 m_MassMonoisotopicD = 0;
        private double m_AbundanceD = 0;

        public Int64 MassMonoisotopicD
        {
            get
            {
                return m_MassMonoisotopicD;
            }
            set
            {
                this.MassMonoisotopic = (double)value;
                m_MassMonoisotopicD = (Int64)value;
            }
        }

        public double AbundanceD
        {
            get
            {
                return m_AbundanceD;
            }
            set
            {
                this.Abundance = (int)value;
                m_AbundanceD = (double)value;
            }
        }
    }
}
