using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Synthetic_Spectra_DLL_X64_Net_4.Framework;

namespace Synthetic_Spectra_DLL_X64_Net_4.Synthetic_Spectra_Module
{
    public class SyntheticSpectraParameters
    {
        public decimal PeakSpacing { get; set; }
        public decimal StartMass { get; set; }
        public decimal EndMass { get; set; }
        public double Resolution { get; set; }
        public int MZextendLengthFactor { get; set; }
        public double PercentHanning { get; set; }
        public int RTSigmaMultiplier { get; set; }
        public AveragineType AveragineType { get; set; }
        public decimal ScanSpacing { get; set; }
        public int NumberOfScans { get; set; }
        public bool WithNoise { get; set; }

        public SyntheticSpectraParameters()
        {
            this.PeakSpacing = 0;
            this.StartMass = 0;
            this.EndMass = 0;
            this.Resolution = 0;
            this.MZextendLengthFactor = 100;//100 is typical
            this.PercentHanning = 0;
            this.RTSigmaMultiplier = 1;//1 is typical
            this.AveragineType = AveragineType.Peptide;
            this.ScanSpacing = 0;
            this.NumberOfScans = 1;
            this.WithNoise = false;
        }
    }
}
