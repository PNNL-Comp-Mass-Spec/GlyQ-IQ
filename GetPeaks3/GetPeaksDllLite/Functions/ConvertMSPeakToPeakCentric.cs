﻿using GetPeaksDllLite.Objects.TandemMSObjects;
using PNNLOmics.Data;
using PNNLOmics.Data.Peaks;

namespace GetPeaks_DLL.Functions
{
    public static class ConvertMSPeakToPeakCentric
    {
        public static PeakCentric Convert(ProcessedPeak peak)
        {
            PeakCentric newPeak = new PeakCentric();
            newPeak.PeakID = 0;
            newPeak.Mz = peak.XValue;
            newPeak.Height = peak.Height;
            newPeak.Width = peak.Width;
            newPeak.Background = peak.Background;
            newPeak.LocalSignalToNoise = peak.LocalSignalToNoise;
            newPeak.ChargeState = peak.Charge;
            //newPeak.MinimaOfLowerMassIndex = peak.MinimaOfLowerMassIndex;
            //newPeak.MinimaOfHigherMassIndex = peak.MinimaOfHigherMassIndex;
            return newPeak;
        }
    }
}
