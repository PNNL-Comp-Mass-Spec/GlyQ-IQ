using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using PNNLOmics.Data.Peaks;

namespace IQGlyQ.Processors
{
    public abstract class BaseProcessor
    {

        /// <summary>
        /// return either the centroided peaks or the centroid and thresholded peaks.  -1 will return the all centroided peaks            
        /// </summary>
        /// <param name="centroidedPeaks"></param>
        /// <param name="thresholdedPeaks"></param>
        /// <returns></returns>
        public static List<ProcessedPeak> ReturnCentroidOrThresholded(List<ProcessedPeak> centroidedPeaks, List<ProcessedPeak> thresholdedPeaks, double signalToShoulderCuttoff)
        {
            List<ProcessedPeak> chromatogramOmicsPeaks;
            if (signalToShoulderCuttoff >= 0)
            {
                chromatogramOmicsPeaks = thresholdedPeaks;
            }
            else
            {
                chromatogramOmicsPeaks = centroidedPeaks;
            }
            return chromatogramOmicsPeaks;
        }
    }
}
