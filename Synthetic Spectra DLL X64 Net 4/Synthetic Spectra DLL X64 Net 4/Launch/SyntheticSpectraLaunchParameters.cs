using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Synthetic_Spectra_DLL_X64_Net_4.Framework;

namespace Synthetic_Spectra_DLL_X64_Net_4.Launch
{
    public class SyntheticSpectraLaunchParameters
    {
        /// <summary>
        /// Fixed point shift factor
        /// </summary>
        public int MassInt64Shift { get; set; }
        
        /// <summary>
        /// Shifting of the intensity but since floats are good enough this is not needed and is usually set to 1
        /// </summary>
        public double IntensityInt64Shift { get; set; }

        /// <summary>
        /// File location of the text file that contains the locations of the various imput files
        /// </summary>
        public string FileListLocation { get; set; }

        /// <summary>
        /// Directory of folder to write the output files such as FileOutputLocation = @"d:\Csharp\Syn Output\";
        /// </summary>
        public string FileOutputLocation { get; set; }

        /// <summary>
        /// true of false for writing to a single YAFMS sqlite database
        /// </summary>
        public bool OutputToYafms { get; set; }

        /// <summary>
        /// true or false for writing each scan out to an individual text file containing m/z vs intensity
        /// </summary>
        public bool OutputToTextFiles { get; set; }

        /// <summary>
        /// Produce chromatogram is stored here
        /// </summary>
        public List<List<PeakGeneric<Int64, float>>> Chromatogram { get; set; }
    }
}
