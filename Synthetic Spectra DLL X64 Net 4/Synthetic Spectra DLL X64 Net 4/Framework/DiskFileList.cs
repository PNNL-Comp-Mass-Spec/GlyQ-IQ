using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Synthetic_Spectra_DLL_X64_Net_4.Launch;

namespace Synthetic_Spectra_DLL_X64_Net_4.Framework
{
    public class DiskFileList
    {
        public string FileName { get; set; }
        public SyntheticSpectraFileType FileKey { get; set; }
        
        public DiskFileList()
        {
            this.FileName = "";
            this.FileKey = SyntheticSpectraFileType.MZParameters;
        }
    }
    public enum SyntheticSpectraFileType
    {
        TheoryData,
        MZParameters,
        Noise,
        LCParameters
    }

}
