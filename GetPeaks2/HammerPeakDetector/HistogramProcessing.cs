using System.Collections.Generic;
using System.Linq;
using PNNLOmics.Algorithms.PeakDetection;
using PNNLOmics.Algorithms.SpectralProcessing;
using PNNLOmics.Data;
using PNNLOmics.Data.Peaks;

namespace HammerPeakDetector
{
    public static class HistogramProcessing
    {
        public static double[] ProcessAndFitHistogramToGetNewParameters(List<XYData> xyHistogram, out List<XYData> fitHistogramToGaussian, out List<XYData> clippedList, out List<XYData> smoothedHistogramXYData, out double areaUnderCurve, out List<XYData> modeledData)
        {
            //1.  smooth data
            int pointsToSmooth = 13;
            SavitzkyGolaySmoother smoother = new SavitzkyGolaySmoother(pointsToSmooth, 2, false);

            List<XYData> xyHistogramForSmoothing = new List<XYData>();
            foreach (var xyData in xyHistogram)
            {
                xyHistogramForSmoothing.Add(new XYData(xyData.X,xyData.Y));
            }
            
          
            
            //this chech is not necessary because we fixed the bins to 50
            if (xyHistogram.Count > 1)
            {
                smoothedHistogramXYData = smoother.Smooth(xyHistogramForSmoothing);
            }
            else
            {
                smoothedHistogramXYData = xyHistogram;
            }

            //2.  detect peak in from smoothed data
            PeakCentroider centroider = new PeakCentroider();
            List<ProcessedPeak> peaks = centroider.DiscoverPeaks(smoothedHistogramXYData);

            //3.  find largest peak in distribution
            List<ProcessedPeak> sortedPeaks = new List<ProcessedPeak>();
            if (peaks.Count > 0)
            {
                sortedPeaks = peaks.OrderByDescending(n => n.Height).ToList();

                ProcessedPeak tallestPeak = sortedPeaks[0];

                //4.  find bounds on tallest peak in distribution
                double newStart = smoothedHistogramXYData[tallestPeak.MinimaOfLowerMassIndex].X;
                double newStop = smoothedHistogramXYData[tallestPeak.MinimaOfHigherMassIndex].X;

                //5.  clip data so we have the center peak isolated
                //clippedList = (from peak in xyHistogram where peak.X >= newStart && peak.X <= newStop select peak).ToList();
                clippedList = (from peak in smoothedHistogramXYData where peak.X >= newStart && peak.X <= newStop select peak).ToList();
            }
            else
            {
                clippedList = xyHistogram;
            }

            //6.  fit smoothed data to gaussian to get peak statistics
            double[] coefficients;
            ProbabilityDistribution curveFit = new ProbabilityDistribution();
            //List<XYData> modeledData = new List<XYData>();
            fitHistogramToGaussian = curveFit.FitDistribution(clippedList, out coefficients, out areaUnderCurve, out modeledData);

            //GetPeaks_DLL.DataFIFO.IXYDataWriter writer = new DataXYDataWriter();
            //writer.WriteOmicsXYData(xyHistogram, @"D:\PNNL\Projects\Christina Polyukh\Histograms\HistogramL1Raw.csv");
            //writer.WriteOmicsXYData(smoothedHistogramXYData, @"D:\PNNL\Projects\Christina Polyukh\Histograms\HistogramL1Smoothed.csv");
            //writer.WriteOmicsXYData(modeledData, @"D:\PNNL\Projects\Christina Polyukh\Histograms\HistogramL1Fit.csv");
            //writer.WriteOmicsXYData(clippedList, @"D:\PNNL\Projects\Christina Polyukh\Histograms\HistogramL1SmoothedClipped.csv");

            return coefficients;
        }
    }
}
