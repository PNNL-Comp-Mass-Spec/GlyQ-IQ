using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmniFinder.Objects
{
    public class RangesMinMax
    {
        public int MinRange { get; set; }
        public int MaxRange { get; set; }

        public RangesMinMax(int minRange, int maxRange)
        {
            MinRange = minRange;
            MaxRange = maxRange;
        }

        public RangesMinMax(int singleRange)
        {
            MinRange = singleRange;
            MaxRange = singleRange;
        }
    }
}
