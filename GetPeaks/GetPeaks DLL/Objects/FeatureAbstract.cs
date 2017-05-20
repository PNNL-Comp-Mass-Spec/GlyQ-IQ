using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Objects
{
    public abstract class FeatureAbstract
    {
        public string featureType { get; set; }

        public double UMCMonoMW { get; set; }
        public double UMCAbundance { get; set; }

        public int ScanStart { get; set; }
        public int ScanEnd { get; set; }

        public int ChargeStateMin { get; set; }
        public int ChargeStateMax { get; set; }

    }
}
