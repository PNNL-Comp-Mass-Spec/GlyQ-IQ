using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Objects.TandemMSObjects
{
    public class ScanCentric
    {
        public int ScanID { get; set; }

        /// <summary>
        /// for precursor match
        /// </summary>
        public int PeakID { get; set; }

        public int ScanNumLc { get; set; }

        public double ElutionTime { get; set; }

        public int ScanNumDt { get; set; }

        public int FrameNumberDt { get; set; }

        public double DriftTime { get; set; }

        public int MsLevel { get; set; }

        public int ParentScanNumber { get; set; }

        public int TandemScanNumber { get; set; }

        public ScanCentric()
        {
        }
    }
}
