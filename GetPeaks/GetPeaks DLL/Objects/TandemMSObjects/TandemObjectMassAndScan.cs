using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;

namespace GetPeaks_DLL.Objects.TandemMSObjects
{
    public class TandemObjectMassAndScan
    {
        public double PrecursorMass { get; set; }

        public int ScanNumber { get; set; }

        public int MSLevel { get; set; }
    }
}
