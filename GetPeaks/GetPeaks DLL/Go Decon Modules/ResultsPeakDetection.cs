using System.Collections.Generic;
using HammerPeakDetector.Objects;
using PNNLOmics.Data;

namespace GetPeaks_DLL.Go_Decon_Modules
{
    public class ResultsPeakDetection
    {
        /// <summary>
        /// how many peaks are in the raw (possibly summed) data
        /// </summary>
        public int RawPeaksInScan { get; set; }

        /// <summary>
        /// how many of these are turned into peaks
        /// </summary>
        public int CentroidedPeaksInScan { get; set; }

        /// <summary>
        /// how many are above the threshold and sent to deisotoping
        /// </summary>
        public int ThresholdedPeaksInScan { get; set; }

        /// <summary>
        /// home for clusters of peaks
        /// </summary>
        public List<ClusterCP<double>> ResultsFromPeakDetectorClusters { get; set; }

        /// <summary>
        /// signal peaks output
        /// </summary>
        public List<XYData> SignalPeaks { get; set; }

        /// <summary>
        /// noise peaks output
        /// </summary>
        public List<XYData> NoisePeaks { get; set; }

        public ResultsPeakDetection()
        {
            ResultsFromPeakDetectorClusters = new List<ClusterCP<double>>();
            NoisePeaks = new List<XYData>();
            SignalPeaks = new List<XYData>();
        }
    }

}
