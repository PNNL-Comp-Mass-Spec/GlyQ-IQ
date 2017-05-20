using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using MathNet.Numerics.Statistics;
using PNNLOmics.Data.Peaks;

namespace HammerPeakDetector.Utilities
{
    public class DataConverter
    {
        /// <summary>
        /// Convert a list of Peaks into a list of XYData
        /// </summary>
        /// <param name="peaks"></param>
        /// <returns></returns>
        public List<XYData> PeakToXYData(List<Peak> peaks)
        {
            List<XYData> newPeaks = new List<XYData>();
            foreach (Peak currentPeak in peaks)
            {
                XYData newPeak = new XYData(currentPeak.XValue, currentPeak.Height);
                newPeaks.Add(newPeak);
            }

            return newPeaks;
        }

        /// <summary>
        /// Convert a histogram into a list of XYData where X is the average of the max and min bounds and Y is the number of items
        /// in the i'th bucket
        /// </summary>
        /// <param name="histogram"></param>
        /// <returns></returns>
        public List<XYData> HistogramToXYData(Histogram histogram)
        {
            List<XYData> newPeaks = new List<XYData>();

            for (int i = 0; i < histogram.BucketCount; i++)
            { 
                double averageOfBounds = (histogram[i].LowerBound + histogram[i].UpperBound) / 2;
                XYData newXY = new XYData(averageOfBounds, histogram[i].Count);
                newPeaks.Add(newXY);
            }

            return newPeaks;
        }

        /// <summary>
        /// Convert a list of XYData to a list of strings
        /// </summary>
        /// <param name="xyData"></param>
        /// <returns></returns>
        public List<string> XYDataToString(List<XYData> xyData)
        {
            List<string> output = new List<string>();

            foreach (XYData xy in xyData)
            {
                output.Add(xy.X + ", " + xy.Y);
            }

            return output;
        }

        /// <summary>
        /// Converts a list of Peaks to a list of ProcessedPeaks
        /// </summary>
        /// <param name="peaks"></param>
        /// <returns></returns>
        public List<ProcessedPeak> PeaksToProcessedPeaks(List<Peak> peaks)
        {
            List<ProcessedPeak> processedPeaks = new List<ProcessedPeak>();
            
            foreach (Peak peak in peaks)
            {
                ProcessedPeak result = new ProcessedPeak();
                result.Height = peak.Height;
                result.LeftWidth = peak.LeftWidth;
                result.RightWidth = peak.RightWidth;
                result.Background = peak.Background;
                result.Width = peak.Width;
                result.XValue = peak.XValue;
                result.LocalSignalToNoise = peak.LocalSignalToNoise;
                result.Points = peak.Points;
                processedPeaks.Add(result);
            }

            return processedPeaks;
        }

        /// <summary>
        /// Converts a Peak to a ProcessedPeak
        /// </summary>
        /// <param name="peaks"></param>
        /// <returns></returns>
        public ProcessedPeak PeaksToProcessedPeaks(Peak peak)
        {
            ProcessedPeak result = new ProcessedPeak();
            result.Height = peak.Height;
            result.LeftWidth = peak.LeftWidth;
            result.RightWidth = peak.RightWidth;
            result.Background = peak.Background;
            result.Width = peak.Width;
            result.XValue = peak.XValue;
            result.LocalSignalToNoise = peak.LocalSignalToNoise;
            result.Points = peak.Points;

            return result;
        }

        /// <summary>
        /// Converts a list of ProcessedPeaks to a list of Peaks
        /// </summary>
        /// <param name="processedPeaks"></param>
        /// <returns></returns>
        public List<Peak> ProcessedPeaksToPeaks(List<ProcessedPeak> processedPeaks)
        {
            List<Peak> peaks = new List<Peak>();

            foreach (ProcessedPeak processedPeak in processedPeaks)
            {
                ProcessedPeak result = new ProcessedPeak();
                result.Height = processedPeak.Height;
                result.LeftWidth = processedPeak.LeftWidth;
                result.RightWidth = processedPeak.RightWidth;
                result.Background = processedPeak.Background;
                result.Width = processedPeak.Width;
                result.XValue = processedPeak.XValue;
                result.LocalSignalToNoise = processedPeak.LocalSignalToNoise;
                result.Points = processedPeak.Points;
                peaks.Add(result);
            }

            return peaks;
        }

        /// <summary>
        /// Converts a list of ProcessedPeaks to a list of doubles containing the XValues of the ProcessedPeaks
        /// </summary>
        /// <param name="processedPeaks"></param>
        /// <returns></returns>
        public List<double> ProcessedPeaksToMass(List<ProcessedPeak> processedPeaks)
        {
            List<double> newList = new List<double>();
            foreach (ProcessedPeak processedPeak in processedPeaks)
            {
                newList.Add(processedPeak.XValue);
            }

            return newList;
        }

        /// <summary>
        /// Converts a list of Peaks to a list of doubles containing the XValues of the ProcessedPeaks
        /// </summary>
        /// <param name="processedPeaks"></param>
        /// <returns></returns>
        public List<double> PeaksToMass(List<Peak> peaks)
        {
            List<double> newList = new List<double>();
            foreach (Peak peak in peaks)
            {
                newList.Add(peak.XValue);
            }

            return newList;
        }
    }
}
