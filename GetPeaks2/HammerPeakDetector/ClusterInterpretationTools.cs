using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HammerPeakDetector.Objects;
using HammerPeakDetector.Objects.GetPeaksDifferenceFinder;

namespace HammerPeakDetector
{
    /// <summary>
    /// This class has been used in developing different methods but currently contains no needed functions
    /// </summary>
    public class ClusterInterpretationTools
    {
        /// <summary>
        /// Find differences between peaks in clusters
        /// </summary>
        /// <param name="clusters"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<double> CreateDifferenceList(List<ClusterCP<double>> clusters)
        {
            //Creates a list of differences to be used for statistics
            List<double> differenceList = new List<double>();
            foreach (ClusterCP<double> currentCluster in clusters)
            {
                int charge = currentCluster.Charge;
                foreach (DifferenceObject<double> peak in currentCluster.Peaks)
                {
                    differenceList.Add((peak.Value2 - peak.Value1) * charge);
                }
            }

            return differenceList;
        }

        /// <summary>
        /// Creates a dictionary that associates indexes with a difference found from clustering (method may be unnessecary)
        /// </summary>
        /// <param name="clusters"></param>
        /// <returns></returns>
        public Dictionary<int, double> CreateDifferenceDictionary(List<ClusterCP<double>> clusters)
        {
            Dictionary<int, double> differenceDictionary = new Dictionary<int, double>();
            //TODO Currently using average of differences for each cluster; is that the best way to do it?

            foreach (ClusterCP<double> cluster in clusters)
            { 
                double difference = (cluster.Peaks[0].Value2 - cluster.Peaks[0].Value1) * cluster.Charge;
                differenceDictionary.Add(cluster.Peaks[0].IndexData, difference);
                for (int i = 0; i < clusters.Count; i++)
                { 
                    
                }
            }

            foreach (ClusterCP<double> cluster in clusters)
            {
                double avgDifference = 0;
                int count = 0;
                foreach (DifferenceObject<double> diffObject in cluster.Peaks)
                {
                    avgDifference += diffObject.Value2 - diffObject.Value1;
                    count++;
                }
                avgDifference = (avgDifference / count) * cluster.Charge;

                differenceDictionary.Add(cluster.Peaks[0].IndexData, avgDifference);
                foreach (DifferenceObject<double> diffObject in cluster.Peaks)
                {
                    differenceDictionary.Add(diffObject.IndexMatch, avgDifference);
                }
            }
            return differenceDictionary;
        }
        
        /// <summary>
        /// Returns an array corresponding to peak indexes in original peak list; returns true for clustered peaks and false for
        /// nonclustered peaks
        /// </summary>
        /// <param name="clusters"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool[] TrueIfClustered(List<ClusterCP<double>> clusters, int totalNumData)
        {
            bool[] indexList = new bool[totalNumData];
            foreach (ClusterCP<double> cluster in clusters)
            {
                if (cluster.Peaks.Count > 0)
                {
                    indexList[cluster.Peaks[0].IndexData] = true;
                    foreach (DifferenceObject<double> currentPeak in cluster.Peaks)
                    {
                        indexList[currentPeak.IndexMatch] = true;
                    }
                }
            }

            return indexList;
        }
    }
}
