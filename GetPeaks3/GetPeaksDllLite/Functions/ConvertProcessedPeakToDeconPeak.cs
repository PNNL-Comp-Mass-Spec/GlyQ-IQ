using System;
using System.Collections.Generic;
using PNNLOmics.Data;
using PNNLOmics.Data.Peaks;

namespace GetPeaksDllLite.Functions
{
    public static class ConvertProcessedPeakToDeconPeak
    {
        public static List<Run64.Backend.Core.Peak> ConvertPeakList(List<ProcessedPeak> processedPeakList)
        {
            List<Run64.Backend.Core.Peak> deconData = new List<Run64.Backend.Core.Peak>();

            foreach (ProcessedPeak omicsPeak in processedPeakList)
            {
                Run64.Backend.Core.Peak deconPeak = new Run64.Backend.Core.Peak(omicsPeak.XValue, Convert.ToSingle(omicsPeak.Height), Convert.ToSingle(omicsPeak.Width));
                deconData.Add(deconPeak);
            }
            return deconData;
        }

        public static List<Run64.Backend.Core.ChromPeak> ConvertChromPeakList(List<ProcessedPeak> processedPeakList)
        {
            List<Run64.Backend.Core.ChromPeak> deconData = new List<Run64.Backend.Core.ChromPeak>();

            foreach (ProcessedPeak omicsPeak in processedPeakList)
            {
                double signalToNoiseReported = omicsPeak.LocalLowestMinimaHeight;
                Run64.Backend.Core.ChromPeak deconPeak = new Run64.Backend.Core.ChromPeak(omicsPeak.XValue, Convert.ToSingle(omicsPeak.Height), Convert.ToSingle(omicsPeak.Width), Convert.ToSingle(signalToNoiseReported));
                deconData.Add(deconPeak);
            }
            return deconData;
        }
    }
}
