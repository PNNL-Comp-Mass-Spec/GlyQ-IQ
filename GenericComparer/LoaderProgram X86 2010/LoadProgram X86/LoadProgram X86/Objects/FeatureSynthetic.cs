using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using PNNLOmics.Data.Features;

namespace ConsoleApplication1
{
    public class FeatureSynthetic:Feature

    {
        //public decimal MassMonoisotopicD { get; set; }
        //public double AbundanceD { get; set; }

        private decimal m_MassMonoisotopicD = 0;
        private double m_AbundanceD = 0;

        public decimal MassMonoisotopicD
        {
            get
            {
                return m_MassMonoisotopicD;
            }
            set
            {
                this.MassMonoisotopic = (double)value;
                m_MassMonoisotopicD = (decimal)value;
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
