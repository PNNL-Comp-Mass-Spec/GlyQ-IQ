using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend;

namespace IQGlyQ.Objects
{
    public class ChromPeakGeneratorParameters
    {
        /// <summary>
        /// used for most of GlyQ-IQ chromatogram generation
        /// </summary>
        public double ChromToleranceInPPM { get; set; }

        /// <summary>
        /// used to bound on the fly window determination
        /// </summary>
        public double ChromToleranceInPPMMax { get; set; }

        /// <summary>
        /// seed value for setting windown on the fly
        /// </summary>
        public double ChromToleranceInPPMInitial { get; set; }

        /// <summary>
        /// used for on the fly window creation based on max peak in chromatogram
        /// //2 will restrict the window to half the width of the largest peak in the initial scan (0.5X)... a bit lost for 50K resolution
        /// //3 will restrict the window to a third (0.33X)
        /// //4 will restrict the window to a quarter (0.25X)
        /// </summary>
        public int AutoSelectEICAt_X_partOfPeakWidth { get; set; }

    

        public Globals.ChromatogramGeneratorMode ChromeGeneratorMode { get; set; }

        public DeconTools.Backend.Globals.IsotopicProfileType IsotopeProfileType {get; set;}

        public Globals.ToleranceUnit ErrorUnit { get; set; }

        public ChromPeakGeneratorParameters()
        {
            ChromToleranceInPPM = 10;
            ChromToleranceInPPMInitial = ChromToleranceInPPM;
            ChromToleranceInPPMMax = 2 * ChromToleranceInPPM;
            AutoSelectEICAt_X_partOfPeakWidth = 2;
            ChromeGeneratorMode = Globals.ChromatogramGeneratorMode.MOST_ABUNDANT_PEAK;
            IsotopeProfileType = DeconTools.Backend.Globals.IsotopicProfileType.UNLABELLED;
            ErrorUnit = Globals.ToleranceUnit.PPM;
        }
    }
}
