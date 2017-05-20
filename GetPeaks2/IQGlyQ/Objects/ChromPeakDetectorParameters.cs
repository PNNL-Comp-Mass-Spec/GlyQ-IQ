using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQGlyQ.Objects
{
    public class ChromPeakDetectorParameters
    {
        /// <summary>
        /// peak to background ratio
        /// </summary>
        public double ChromPeakDetectorPeakBR { get; set; }

        /// <summary>
        /// signal to noise ratio
        /// </summary>
        public double ChromPeakDetectorSignalToNoise { get; set; }

        public ChromPeakDetectorParameters()
        {
            //ChromPeakDetectorPeakBR = 1.5;
            //ChromPeakDetectorSignalToNoise = 2;
            ChromPeakDetectorPeakBR = 1;
            ChromPeakDetectorSignalToNoise = 1;
        }
    }
}
