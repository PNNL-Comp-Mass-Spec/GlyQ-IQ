using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend;

namespace GetPeaks_DLL.Go_Decon_Modules
{
    public class ParametersDeconToolsPeakDetection
    {
        /// <summary>
        /// decon tools peak detector
        /// </summary>
        public double MsPeakDetectorPeakToBackground { get; set; }

        public double SignalToNoiseRatio { get; set; }

        public Globals.PeakFitType PeakFitType { get; set; }

        public ParametersDeconToolsPeakDetection()
        {
            MsPeakDetectorPeakToBackground = 1.3;
            SignalToNoiseRatio = 2;
            PeakFitType = Globals.PeakFitType.QUADRATIC;
        }
    }
}
