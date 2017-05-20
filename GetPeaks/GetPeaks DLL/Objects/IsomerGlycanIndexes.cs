using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Objects
{
    public class IsomerGlycanIndexes
    {
        public List<int> FeatureIndexes { get; set; }
        public List<int> OmniFinderIndexes { get; set; }

        public IsomerGlycanIndexes()
        {
            FeatureIndexes = new List<int>();
            OmniFinderIndexes = new List<int>();
        }
    }
}
