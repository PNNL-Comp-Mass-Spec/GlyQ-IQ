using System.Collections.Generic;
using PNNLOmics.Data.Peaks;
using Run64.Backend.Core;

namespace GetPeaksDllLite.Functions
{
    public static class ConvertPeakListDeconToOmics
    {
        public static List<PNNLOmics.Data.Peak> Convert(List<Run64.Backend.Core.MSPeak> deconPeakList, bool keepPeaks)
        {
            List<PNNLOmics.Data.Peak> omicsPeakList = new List<PNNLOmics.Data.Peak>();

            for (int i = 0; i < deconPeakList.Count;i++)
            {
                Run64.Backend.Core.MSPeak dPeak = deconPeakList[i];

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

        public static List<PNNLOmics.Data.Peak> ConvertPeak(List<Run64.Backend.Core.Peak> deconPeakList, bool keepPeaks)
        {
            List<PNNLOmics.Data.Peak> omicsPeakList = new List<PNNLOmics.Data.Peak>();

            for (int i = 0; i < deconPeakList.Count; i++)
            {
                Run64.Backend.Core.Peak dPeak = deconPeakList[i];

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

        public static List<ProcessedPeak> ConvertToProcessedPeak(List<Run64.Backend.Core.Peak> deconPeakList, bool keepPeaks)
        {
            List<ProcessedPeak> omicsPeakList = new List<ProcessedPeak>();

            for (int i = 0; i < deconPeakList.Count; i++)
            {
                Run64.Backend.Core.Peak dPeak = deconPeakList[i];

                ProcessedPeak newOmicsPeak = new ProcessedPeak();
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
