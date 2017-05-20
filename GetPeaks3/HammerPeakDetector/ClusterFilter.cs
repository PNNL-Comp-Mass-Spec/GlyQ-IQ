using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HammerPeakDetector.Objects;
using HammerPeakDetector.Utilities;

namespace HammerPeakDetector
{
    public class ClusterFilter
    {

        public List<ClusterCP<double>> FilterRepeatedPeaks(List<PNNLOmics.Data.Peak> peaks, List<ClusterCP<double>> peakClusters)
        {
            List<ClusterCP<double>> newPeakClusters = new List<ClusterCP<double>>();

            #region Sort peaks for filtering
            List<ClusterCP<double>> sortedClusters = new List<ClusterCP<double>>();
            List<ClusterCP<double>> otherSortedClusters = new List<ClusterCP<double>>();
                        
            //Remove clusters with zero peaks
            foreach (ClusterCP<double> cluster in peakClusters)
            {
                if (cluster.Peaks.Count > 0)
                    newPeakClusters.Add(cluster);
            }

            //Sort by descending mass
            foreach (ClusterCP<double> cluster in newPeakClusters)
            {
                otherSortedClusters = (newPeakClusters.OrderByDescending(p => p.Peaks[0].IndexData)).ToList();
            }

            //Sort by descending cluster size
            foreach (ClusterCP<double> cluster in otherSortedClusters)
            {
                sortedClusters = (otherSortedClusters.OrderByDescending(p => p.Peaks.Count)).ToList();
            }

            #region Old sort peaks for filtering (off)
            //List<ClusterCP<double>> sortedClusters = new List<ClusterCP<double>>();

            ////Remove clusters with zero peaks
            //foreach (ClusterCP<double> cluster in peakClusters)
            //{
            //    if (cluster.Peaks.Count > 0)
            //        newPeakClusters.Add(cluster);
            //}

            ////Sort by ascending mass
            //foreach (ClusterCP<double> cluster in peakClusters)
            //{
            //    newPeakClusters = (newPeakClusters.OrderBy(p => p.Peaks[0].IndexData)).ToList();
            //}

            ////Reverse so that newPeakClusters is sorted by descending mass
            //for (int i = newPeakClusters.Count - 1; i >= 0; i--)
            //{
            //    sortedClusters.Add(newPeakClusters[i]);
            //}

            ////Sort by ascending cluster size
            //foreach (ClusterCP<double> cluster in sortedClusters)
            //{
            //    newPeakClusters = (sortedClusters.OrderBy(p => p.Peaks.Count)).ToList();
            //}

            //sortedClusters = new List<ClusterCP<double>>();

            ////Sort by descending cluster size
            //for (int i = newPeakClusters.Count - 1; i >= 0; i--)
            //{
            //    sortedClusters.Add(newPeakClusters[i]);
            //}

            #endregion

            #endregion

            //Currently, newPeakClusters is sorted in descending order of cluster size. Clusters with the same size are sorted
            //by descending mass

            #region Filter repeated peaks
            bool[] used = new bool[peaks.Count];

            newPeakClusters = new List<ClusterCP<double>>();

            //Loop eliminates repeating indexes in peakClusters so that each index belongs to a maximum of one cluster
            foreach (ClusterCP<double> cluster in sortedClusters)
            {
                ClusterCP<double> newCluster = new ClusterCP<double>();
                //Variables preventing a cluster from having an indexing gap because one of the middle indexes is used but outer
                //indexes are not; also allows a cluster to be split into two clusters if middle index is repeated but outer are not
                bool first = true;
                bool skipped = false;
                int lastUsed = 0;
                for (int i = 0; i < cluster.Peaks.Count; i++)
                {
                    //If indexes have not been used, they will be added to a cluster
                    if (!used[cluster.Peaks[i].IndexData] && !used[cluster.Peaks[i].IndexMatch])
                    {
                        //For when a cluster is split into two; new cluster created here
                        if (skipped)
                        {
                            AddCluster(newPeakClusters, used, newCluster, lastUsed);
                            newCluster = new ClusterCP<double>();
                        }
                        newCluster.Peaks.Add(cluster.Peaks[i]);
                        used[cluster.Peaks[i].IndexData] = true;
                        lastUsed = cluster.Peaks[i].IndexMatch;
                        first = false;
                    }
                    else
                    {
                        //Indicates that the cluster should be split
                        if (!first)
                        {
                            skipped = true;
                        }
                    }
                }
                //Adds cluster to newPeakClusters
                AddCluster(newPeakClusters, used, newCluster, lastUsed);               
            }

            #endregion

            #region Resorting clusters and adding charge state
            //Sort clusters by index
            foreach (ClusterCP<double> cluster in newPeakClusters)
            {
                newPeakClusters = (newPeakClusters.OrderBy(p => p.Peaks[0].IndexData)).ToList();
            }

            ChargeCalculator.CalculateChargeStateFromDifference(ref newPeakClusters);

            #endregion

            #region Check if indexes are repeating test (off)
            //List<int> indexes = new List<int>();
            //bool repeating = false;

            //for (int i = 0; i < newPeakClusters.Count; i++)
            //{
            //    for (int j = 0; j < newPeakClusters[i].Peaks.Count; j++)
            //    {
            //        indexes.Add(newPeakClusters[i].Peaks[j].IndexData);
            //    }
            //    indexes.Add(newPeakClusters[i].Peaks[newPeakClusters[i].Peaks.Count - 1].IndexMatch);
            //}

            //for (int i = 0; i < indexes.Count; i++)
            //{
            //    for (int j = i + 1; j < indexes.Count; j++)
            //    {
            //        if (indexes[i] == indexes[j])
            //        {
            //            repeating = true;
            //        }
            //    }
            //}

            #endregion

            return newPeakClusters;
        }

        #region Add cluster to newPeakClusters
        private void AddCluster(List<ClusterCP<double>> newPeakClusters, bool[] used, ClusterCP<double> newCluster, int lastUsed)
        {
            used[lastUsed] = true;
            if (newCluster.Peaks.Count > 0)
            {
                newPeakClusters.Add(newCluster);
            }
        }

        #endregion
    }
}
