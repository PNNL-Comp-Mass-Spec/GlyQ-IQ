using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Objects.ParameterObjects
{
    public class PeakProcessingParameters
    {
        public string parameterFilePath { get; set; }
        public string fileName { get; set; } //Gly06_250nL_100min_07Jan12_LYNX_SN88_TFA_10_peaks.txt
        public string folderIN { get; set; } //@"D:\Csharp\Peaks test file\"
        public string folderOut { get; set; } //@"E:\"
        public decimal slope { get; set; }  //0.999984656323467, slope for calibration y=mx+b where X=raw data and Y =calibrated Data
        public decimal intercept { get; set; } //0.00179408173084994, intercept for calibration y=mx+b where X=raw data and Y =calibrated Data
    }
}
