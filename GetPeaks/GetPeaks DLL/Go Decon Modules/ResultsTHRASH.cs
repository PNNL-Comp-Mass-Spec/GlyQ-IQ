using System.Collections.Generic;
using GetPeaks_DLL.Objects.ResultsObjects;
using GetPeaks_DLL.Objects;
using HammerPeakDetector.Objects;
using PNNLOmics.Data;
using GetPeaks_DLL.Parallel;

namespace GetPeaks_DLL.Go_Decon_Modules
{
    public class ResultsTHRASH:ParalellResults
    {
        public int Scan { get; set; }

        public double Sum { get; set; }

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
        /// how many monoisotopic peak detected
        /// </summary>
        public int MonoisotopicPeaksInScan { get; set; }

        /// <summary>
        /// home of deisotoped data
        /// </summary>
        public ResultCollectionLite ResultsFromRun { get; set; }

        public List<IsotopeObject> ResultsFromRunConverted { get; set; }
        
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

        public ResultsTHRASH(int scan)
        {
            Scan = scan;
            ResultsFromRun  = new ResultCollectionLite();
            ResultsFromRunConverted = new List<IsotopeObject>();
            ResultsFromPeakDetectorClusters = new List<ClusterCP<double>>();
            NoisePeaks = new List<XYData>();
            SignalPeaks = new List<XYData>();
        }
    }

}
