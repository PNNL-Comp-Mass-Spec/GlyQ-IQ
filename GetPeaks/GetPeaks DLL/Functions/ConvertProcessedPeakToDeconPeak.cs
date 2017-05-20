using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

namespace GetPeaks_DLL.Functions
{
    public static class ConvertProcessedPeakToDeconPeak
    {
        public static List<DeconTools.Backend.Core.Peak> ConvertPeakList(List<ProcessedPeak> processedPeakList)
        {
            List<DeconTools.Backend.Core.Peak> deconData = new List<DeconTools.Backend.Core.Peak>();

            foreach (ProcessedPeak omicsPeak in processedPeakList)
            {
                DeconTools.Backend.Core.Peak deconPeak = new DeconTools.Backend.Core.Peak(omicsPeak.XValue, Convert.ToSingle(omicsPeak.Height), Convert.ToSingle(omicsPeak.Width));
                deconData.Add(deconPeak);
            }
            return deconData;
        }

        public static List<DeconTools.Backend.Core.ChromPeak> ConvertChromPeakList(List<ProcessedPeak> processedPeakList)
        {
            List<DeconTools.Backend.Core.ChromPeak> deconData = new List<DeconTools.Backend.Core.ChromPeak>();

            foreach (ProcessedPeak omicsPeak in processedPeakList)
            {
                double signalToNoiseReported = omicsPeak.LocalLowestMinimaHeight;
                DeconTools.Backend.Core.ChromPeak deconPeak = new DeconTools.Backend.Core.ChromPeak(omicsPeak.XValue, Convert.ToSingle(omicsPeak.Height), Convert.ToSingle(omicsPeak.Width), Convert.ToSingle(signalToNoiseReported));
                deconData.Add(deconPeak);
            }
            return deconData;
        }
    }
}
