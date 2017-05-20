using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Objects
{
    public class Isomer
    {
        public List<FeatureAbstract> FeatureList { get; set; }
        public int IsomerCount { get; set; }

        public Isomer()
        {
            FeatureList = new List<FeatureAbstract>();
            IsomerCount = 0;
        }
    }
}
