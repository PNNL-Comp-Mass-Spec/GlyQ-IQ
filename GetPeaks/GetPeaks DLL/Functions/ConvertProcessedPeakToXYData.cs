using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

namespace GetPeaks_DLL.Functions
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

        public static DeconTools.Backend.XYData ConvertPointsToDecon(List<ProcessedPeak> peaks)
        {
            if (peaks == null)
                return null;
            
            DeconTools.Backend.XYData points = new DeconTools.Backend.XYData();

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
