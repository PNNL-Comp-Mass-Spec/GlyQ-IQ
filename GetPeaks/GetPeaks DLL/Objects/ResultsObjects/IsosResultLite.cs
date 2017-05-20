using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;

namespace GetPeaks_DLL.Objects.ResultsObjects
{
    public class IsosResultLite
    {
        public int MSFeatureID { get; set; }

        public IList<ResultFlag> Flags = new List<ResultFlag>();

        //public Run Run { get; set; }

        //public ScanSet ScanSet { get; set; }

        public IsotopicProfile IsotopicProfile { get; set; }

        public double InterferenceScore { get; set; }

        public int ChargeState { get; set; }

        public double FitScore { get; set; }

        public IsosResultLite()
        {
            this.IsotopicProfile = new IsotopicProfile();
        }
    }
}
