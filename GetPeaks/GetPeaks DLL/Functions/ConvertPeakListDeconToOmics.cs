using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Functions
{
    public static class ConvertPeakListDeconToOmics
    {
        public static List<PNNLOmics.Data.Peak> Convert(List<DeconTools.Backend.Core.MSPeak> deconPeakList, bool keepPeaks)
        {
            List<PNNLOmics.Data.Peak> omicsPeakList = new List<PNNLOmics.Data.Peak>();

            for (int i = 0; i < deconPeakList.Count;i++)
            {
                DeconTools.Backend.Core.MSPeak dPeak = deconPeakList[i];

                PNNLOmics.Data.Peak newOmicsPeak = new PNNLOmics.Data.Peak();
                newOmicsPeak.Background = 0;
                newOmicsPeak.Height = dPeak.Height;
                newOmicsPeak.LocalSignalToNoise = dPeak.SignalToNoise;
                newOmicsPeak.Width = dPeak.Width;
                newOmicsPeak.XValue = dPeak.XValue;

                omicsPeakList.Add(newOmicsPeak);

                if (keepPeaks == false)
                {
                    dPeak = null;
                }
            }

            if (keepPeaks == false)
            {
                deconPeakList = null;
            }

            return omicsPeakList;
        }

        public static List<PNNLOmics.Data.Peak> ConvertPeak(List<DeconTools.Backend.Core.Peak> deconPeakList, bool keepPeaks)
        {
            List<PNNLOmics.Data.Peak> omicsPeakList = new List<PNNLOmics.Data.Peak>();

            for (int i = 0; i < deconPeakList.Count; i++)
            {
                DeconTools.Backend.Core.Peak dPeak = deconPeakList[i];

                PNNLOmics.Data.Peak newOmicsPeak = new PNNLOmics.Data.Peak();
                newOmicsPeak.Background = 0;
                newOmicsPeak.Height = dPeak.Height;
                newOmicsPeak.Width = dPeak.Width;
                newOmicsPeak.XValue = dPeak.XValue;

                omicsPeakList.Add(newOmicsPeak);

                if (keepPeaks == false)
                {
                    dPeak = null;
                }
            }

            if (keepPeaks == false)
            {
                deconPeakList = null;
            }

            return omicsPeakList;
        }

        public static List<PNNLOmics.Data.ProcessedPeak> ConvertToProcessedPeak(List<DeconTools.Backend.Core.Peak> deconPeakList, bool keepPeaks)
        {
            List<PNNLOmics.Data.ProcessedPeak> omicsPeakList = new List<PNNLOmics.Data.ProcessedPeak>();

            for (int i = 0; i < deconPeakList.Count; i++)
            {
                DeconTools.Backend.Core.Peak dPeak = deconPeakList[i];

                PNNLOmics.Data.ProcessedPeak newOmicsPeak = new PNNLOmics.Data.ProcessedPeak();
                newOmicsPeak.Background = 0;
                newOmicsPeak.Height = dPeak.Height;
                newOmicsPeak.Width = dPeak.Width;
                newOmicsPeak.XValue = dPeak.XValue;

                omicsPeakList.Add(newOmicsPeak);

                if (keepPeaks == false)
                {
                    dPeak = null;
                }
            }

            if (keepPeaks == false)
            {
                deconPeakList = null;
            }

            return omicsPeakList;
        }
    }
}
