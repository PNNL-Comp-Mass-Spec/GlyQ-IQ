using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects.DifferenceFinderObjects;
using PNNLOmics.Data;
using GetPeaks_DLL.DataFIFO;
using OrbitrapPeakDetection.Objects;
using OrbitrapPeakDetection;

namespace OrbitrapPeakDetection
{
    /// <summary>
    /// Finds the indexes of the last peaks in each cluster
    /// </summary>
    public class LastPeaks
    {
        public List<double> FindLastPeaks(List<PNNLOmics.Data.Peak> data)
        {
            List<double> sortedLastPeaks = new List<double>();

            //Creates lists to be used for finding the last peaks of each feature
            List<DifferenceObject<double>> mostRecentPeak = new List<DifferenceObject<double>>();
            List<DifferenceObject<double>> lastPeakGroups = new List<DifferenceObject<double>>();
            List<double> lastPeaks = new List<double>();
            List<double> lastPeaksWithoutRepetition = new List<double>();
            //List<ClusterCP<double>> peakClusters = new List<ClusterCP<double>>();
            List<ClusterCP<double>> peakClusters = new List<ClusterCP<double>>();

            int clusterType = 0; //0 = filtered clusters; 1 = nonfiltered clusters
            //FILTERONOFF
            //Allows user to choose whether clusters are filtered to have are no repeating indexes
            switch (clusterType)
            {
                case 0:
                    {
                        FindPeakClusters clusterFinder = new FindPeakClusters();
                        List<ClusterCP<double>> clustersOfPeaks = new List<ClusterCP<double>>();
                        clustersOfPeaks = clusterFinder.FindClusters(data);

                        RepeatedClusterFilter findPeakClusters = new RepeatedClusterFilter();
                        peakClusters = findPeakClusters.FilterRepeatedPeaks(data, clustersOfPeaks);
                    }
                    break;
                case 1:
                    {
                        FindPeakClusters findPeakClusters = new FindPeakClusters();
                        peakClusters = findPeakClusters.FindClusters(data);
                    }
                    break;
            }
                    
            //Indicates last peak in feature
            int last = 0;

            //Adds the index of each last peak from the peak clusters to lastPeaks
            for (int i = 0; i < peakClusters.Count; i++)
            {
                last = ((peakClusters[i].Peaks.Count) - 1);
                lastPeaks.Add(peakClusters[i].Peaks[last].IndexMatch);
            }

            bool done = false;

            #region Only needed if using non-filtered data

            //Loop to prevent repetition; Only for use with non-filtered data
            if (clusterType == 1)
            {
                for (int i = 0; i < lastPeaks.Count; i++)
                {
                    for (int j = 0; j < lastPeaksWithoutRepetition.Count; j++)
                    {
                        if (lastPeaks[i] == lastPeaksWithoutRepetition[j])
                        {
                            done = true;
                        }
                    }
                    if (done == false)
                    {
                        lastPeaksWithoutRepetition.Add(lastPeaks[i]);
                    }
                    done = false;
                }
            }

            #endregion

            //Puts lastPeaksWithoutRepetition in order by index
            sortedLastPeaks = lastPeaks.OrderBy(p => p).ToList();

            return sortedLastPeaks;
        }

        public List<double> FindLastPeaks(List<PNNLOmics.Data.Peak> data, List<ClusterCP<double>> peakClusters)
        {
            List<double> sortedLastPeaks = new List<double>();

            //Creates lists to be used for finding the last peaks of each feature
            List<DifferenceObject<double>> mostRecentPeak = new List<DifferenceObject<double>>();
            List<DifferenceObject<double>> lastPeakGroups = new List<DifferenceObject<double>>();
            List<double> lastPeaks = new List<double>();
            List<double> lastPeaksWithoutRepetition = new List<double>();

            int clusterType = 0; //0 = filtered clusters; 1 = nonfiltered clusters

            //Indicates last peak in feature
            int last = 0;

            //Adds the index of each last peak from the peak clusters to lastPeaks
            for (int i = 0; i < peakClusters.Count; i++)
            {
                last = ((peakClusters[i].Peaks.Count) - 1);
                lastPeaks.Add(peakClusters[i].Peaks[last].IndexMatch);
            }

            bool done = false;

            #region Only needed if using non-filtered data

            //Loop to prevent repetition; Only for use with non-filtered data
            if (clusterType == 1)
            {
                for (int i = 0; i < lastPeaks.Count; i++)
                {
                    for (int j = 0; j < lastPeaksWithoutRepetition.Count; j++)
                    {
                        if (lastPeaks[i] == lastPeaksWithoutRepetition[j])
                        {
                            done = true;
                        }
                    }
                    if (done == false)
                    {
                        lastPeaksWithoutRepetition.Add(lastPeaks[i]);
                    }
                    done = false;
                }
            }

            #endregion

            //Puts lastPeaksWithoutRepetition in order by index
            sortedLastPeaks = lastPeaks.OrderBy(p => p).ToList();

            return sortedLastPeaks;
        }

    }
}
