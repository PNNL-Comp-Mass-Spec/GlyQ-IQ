using System.Collections.Generic;
using PNNLOmics.Data;
using PNNLOmics.Data.Peaks;

namespace GetPeaksDllLite.Functions
{
    public static class ConvertProcessedPeakToXYData
    {
        public static XYData ConvertPoint(ProcessedPeak peak)
        {
            return new XYData(peak.XValue,peak.Height);
        }

        public static List<XYData> ConvertPoints(List<ProcessedPeak> peaks)
        {
            List<XYData> points = new List<XYData>();
            foreach (var peak in peaks)
            {
                points.Add(new XYData(peak.XValue, peak.Height));
            }

            return points;
        }

        public static Run64.Backend.Data.XYData ConvertPointsToDecon(List<ProcessedPeak> peaks)
        {
            if (peaks == null)
                return null;

            Run64.Backend.Data.XYData points = new Run64.Backend.Data.XYData();

            int length = peaks.Count;
            points.Xvalues = new double[length];
            points.Yvalues = new double[length];

            for(int i=0;i<length;i++)
            {
                ProcessedPeak peak = peaks[i];
                points.Xvalues[i] = peak.XValue;
                points.Yvalues[i] = peak.Height;
            }

            return points;
        }
    }
}
