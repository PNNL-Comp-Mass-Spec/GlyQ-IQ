using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Synthetic_Spectra_DLL_X64_Net_4.Framework;

namespace Synthetic_Spectra_DLL_X64_Net_4.Synthetic_Spectra_Module
{
    public class SyntheticChromatogramParameters
    {
        public List<List<PeakGeneric<Int64, float>>> Chromatogram { get; set; }
        public XYDataGeneric<Int64,double> PeakToAdd { get; set; }
        public XYDataGeneric<int, int> RTFWHMToAdd {get;set;}
        public SyntheticSpectraRun<Int64, double> SpectraFiles { get; set; }
        public int MassInt64Shift { get; set; }
        public int numberOfScans { get; set; }
        public bool GenerateWithNoise { get; set; }

        public SyntheticChromatogramParameters(int numberOfScans)
        {
            this.Chromatogram = new List<List<PeakGeneric<Int64, float>>>();
            for (int i = 0; i < numberOfScans; i++)
            { 
                List<PeakGeneric<Int64, float>> newZeroSpectraforGrid = new List<PeakGeneric<long,float>>();
                this.Chromatogram.Add(newZeroSpectraforGrid);
            }
            this.RTFWHMToAdd = null;
            this.PeakToAdd = null;
            this.SpectraFiles = null;
            this.MassInt64Shift = 1;
            this.GenerateWithNoise = false;
        }
    }
}
