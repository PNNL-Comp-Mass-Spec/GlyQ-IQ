using System;
using System.Collections.Generic;

namespace GetPeaksDllLite.Objects
{
    public class IsotopeObject:IDisposable
    {
        public double MonoIsotopicMass { get; set; }

        /// <summary>
        /// MZ from first point in Isotope Peak List
        /// </summary>
        public double ExperimentMass { get; set; }
        public List<PNNLOmics.Data.Peak> IsotopeList { get; set; }
        public string IsotopeMassString { get; set; }
        public string IsotopeIntensityString { get; set; }

        public int Charge { get; set; }

        public double FitScore { get; set; }

        public IsotopeObject()
        {
            IsotopeList = new List<PNNLOmics.Data.Peak>();
            IsotopeIntensityString = "";
            IsotopeMassString = "";
        }

        #region IDisposable Members

        public void Dispose()
        {
            for (int i = 0; i < IsotopeList.Count;i++)
            {
                IsotopeList[i] = null;
            }
            IsotopeList = null;
        }

        #endregion
    }


}
