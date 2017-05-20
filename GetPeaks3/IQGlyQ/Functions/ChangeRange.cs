using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQGlyQ.Objects;
using PNNLOmics.Data;
using PNNLOmics.Data.Peaks;

namespace IQGlyQ.Functions
{
    public static class ChangeRange
    {
        public static List<XYData> ClipXyDataToScanRange(List<XYData> data, ScanObject scans, bool shouldWeBuffer)
        {
            List<XYData> clippedList = null;
            
            clippedList = shouldWeBuffer ? ExpandXyDataToScanRange(data, scans) : ClipXyDataToScanRange(data, scans.Start, scans.Stop);

            return clippedList;
        }

        public static List<XYData> ClipXyDataToScanRange(List<XYData> data, int newStart, int newStop)
        {
            bool test1 = data == null;
            bool test2 = data != null && data.Count < 2;
            if (test1) return null;
            if (test2) return data;

            List<XYData> clippedList = (from peak in data where peak.X >= newStart && peak.X <= newStop select peak).ToList();
            return clippedList;
        }

        private static List<XYData> ExpandXyDataToScanRange(List<PNNLOmics.Data.XYData> data, ScanObject scans)
        {
            int newStart = scans.Start - scans.Buffer;
            int newStop = scans.Stop + scans.Buffer;
            
            if (newStart < scans.Min)
            {
                newStart = scans.Min;
            }

            if (newStop > scans.Max)
            {
                newStop = scans.Max;
            }

            List<XYData> bufferedList = ClipXyDataToScanRange(data, newStart, newStop); 

            return bufferedList;
        }

        public static List<ProcessedPeak> ClipProcessedPeakToScanRange(List<ProcessedPeak> data, int newStart, int newStop)
        {
            bool test1 = data == null;
            bool test2 = data != null && data.Count < 2;
            if (test1) return null;
            if (test2) return data;

            List<ProcessedPeak> clippedList = (from peak in data where peak.XValue >= newStart && peak.XValue <= newStop select peak).ToList();
            return clippedList;
        }

        public static List<ProcessedPeak> ClipProcessedPeakToScanRange(List<ProcessedPeak> data, ScanObject scans, bool shouldWeBuffer)
        {
            List<ProcessedPeak> clippedList = null;

            if (shouldWeBuffer)
            {
                clippedList = ExpandProcessedPeakToScanRange(data, scans);
            }
            else
            {
                clippedList = ClipProcessedPeakToScanRange(data, scans.Start, scans.Stop);
            }

            return clippedList;
        }

        private static List<ProcessedPeak> ExpandProcessedPeakToScanRange(List<ProcessedPeak> data, ScanObject scans)
        {
            int newStart = scans.Start - scans.Buffer;
            int newStop = scans.Stop + scans.Buffer;

            if (newStart < scans.Min)
            {
                newStart = scans.Min;
            }

            if (newStop > scans.Max)
            {
                newStop = scans.Max;
            }

            List<ProcessedPeak> bufferedList = ClipProcessedPeakToScanRange(data, newStart, newStop);

            return bufferedList;
        }
    }
}
