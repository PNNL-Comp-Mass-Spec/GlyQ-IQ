using System;
using System.Collections.Generic;
using System.Linq;
using HammerPeakDetector.Objects;

namespace HammerPeakSupport
{
    public class RepeatedClusterFilter
    {
        public List<ClusterCP<double>> FilterRepeatedPeaks(List<PNNLOmics.Data.Peak> peaks, List<ClusterCP<double>> clustersOfPeaks)
        {

            #region Variables

            //FindPeakClusters clusterFinder = new FindPeakClusters();
            //List<ClusterCP<double>> clustersOfPeaks = new List<ClusterCP<double>>();
            //clustersOfPeaks = clusterFinder.FindClusters(peaks);

            List<ClusterCP<double>> firstSortedClusters = new List<ClusterCP<double>>();
            List<ClusterCP<double>> seccondSortedClusters = new List<ClusterCP<double>>();
            List<ClusterCP<double>> sortedClusters = new List<ClusterCP<double>>();
            List<ClusterCP<double>> orderedSortedClusters = new List<ClusterCP<double>>();
            List<ClusterCP<double>> finalSortedClusters = new List<ClusterCP<double>>();
            List<ClusterCP<double>> clusters = new List<ClusterCP<double>>();
            List<ClusterCP<double>> sortedNonRepeatingClusters = new List<ClusterCP<double>>();

            #endregion

            #region sort features
            //remove clulsters with zero peaks.  this is a bandaid
            for (int i = 0; i < clustersOfPeaks.Count; i++)
            {
                if (clustersOfPeaks[i].Peaks.Count>0)
                    seccondSortedClusters.Add(clustersOfPeaks[i]);
            }

            //Sort by ascending mass
            for (int i = 0; i < clustersOfPeaks.Count; i++)
            {
                firstSortedClusters = (seccondSortedClusters.OrderBy(p => p.Peaks[0].IndexData)).ToList();
                //firstSortedClusters = (clustersOfPeaks.OrderBy(p => p.Peaks[0].IndexData)).ToList();
            }

            //Reverse so that sortedClusters is sorted by descending mass
            for (int i = (firstSortedClusters.Count-1); i >=0 ; i--)
            {
                sortedClusters.Add(firstSortedClusters[i]);
            }

            //Sort by ascending cluster size
            for (int i = 0; i < sortedClusters.Count; i++)
            {
                orderedSortedClusters=(sortedClusters.OrderBy(p=>p.Peaks.Count)).ToList();
            }

            //Sort by descending cluster size
            for (int i = (orderedSortedClusters.Count-1); i >=0 ; i--)
            {
                finalSortedClusters.Add(orderedSortedClusters[i]);
            }


            //Now finalSortedClusters is sorted in descending order by cluster size and when clusters have the same size,
            //they are sorted by descending mass
            #endregion

            #region filter repeated peaks

            bool[] noUse = new bool[finalSortedClusters.Count];

            double currentClusterData = 0;
            double currentClusterMatch = 0;
            double comparedClusterData = 0;
            double comparedClusterMatch = 0;

            ///Loop eliminates repeating indexes in clusters so that each index can belong to a maximum of one cluster
            for (int i = 0; i < finalSortedClusters.Count; i++)
            {
                if (noUse[i] == false)
                {
                    for (int j = 0; j < finalSortedClusters[i].Peaks.Count; j++)
                    {
                        currentClusterData = finalSortedClusters[i].Peaks[j].IndexData;
                        currentClusterMatch = finalSortedClusters[i].Peaks[j].IndexMatch;
                        for (int k = i + 1; k < finalSortedClusters.Count; k++)
                        {
                            for (int l = 0; l < finalSortedClusters[k].Peaks.Count; l++)
                            {
                                if (noUse[i] == false)
                                {
                                    comparedClusterData = finalSortedClusters[k].Peaks[l].IndexData;
                                    comparedClusterMatch = finalSortedClusters[k].Peaks[l].IndexMatch;
                                    if ((currentClusterData == comparedClusterData) | (currentClusterData == comparedClusterMatch) |
                                        (currentClusterMatch == comparedClusterData) | (currentClusterMatch == comparedClusterMatch))
                                    {
                                        if (finalSortedClusters[i].Peaks.Count > finalSortedClusters[k].Peaks.Count)
                                        {
                                            noUse[k] = true;
                                        }
                                        else if (peaks[Convert.ToInt32(finalSortedClusters[i].Peaks[0].IndexData)].Height > peaks[Convert.ToInt32(finalSortedClusters[k].Peaks[0].IndexData)].Height)
                                        {
                                            noUse[k] = true;
                                        }
                                        else
                                        {
                                            noUse[i] = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region Making the clusters pretty

            for (int i = 0; i < finalSortedClusters.Count; i++)
            {
                if (noUse[i] == false)
                {
                    clusters.Add(finalSortedClusters[i]);
                }
            }
            
            for (int i = 0; i < clusters.Count; i++)
            {
                sortedNonRepeatingClusters = (clusters.OrderBy(p => p.Peaks[0].IndexData)).ToList();
            }

            ChargeCalculator.CalculateChargeStateFromDifference(ref sortedNonRepeatingClusters);

            #endregion

            #region old techniques (off)
            #region find clusters newest (off)
            //List<FeatureCP<double>> bestClusters = new List<FeatureCP<double>>();

            //List<List<int>> featureIndexesOfData = new List<List<int>>();

            //int maxIndex = sortedClusters[(sortedClusters.Count)-1].Peaks[(sortedClusters[(sortedClusters.Count)-1].Peaks.Count)-1].IndexMatch;

            //bool[] featureUsed = new bool[sortedClusters.Count];

            


            //for (int currentIndex=0; currentIndex < maxIndex; currentIndex++)
            //{
            //    List<int> currentFeatureIndex = new List<int>();
            //    featureIndexesOfData.Add(currentFeatureIndex);
            //    for (int i = 0; i < sortedClusters.Count; i++)
            //    {
            //        for (int j = 0; j < sortedClusters[i].Peaks.Count; j++)
            //        { 
            //            if (((sortedClusters[i].Peaks[j].IndexData==currentIndex)||(sortedClusters[i].Peaks[j].IndexMatch==currentIndex)) &&(featureUsed[i]==false))
            //            {
            //                currentFeatureIndex.Add(i);
            //                featureUsed[i] = true;
            //            }
            //        }
            //    }
            //    for (int i = 0; i < sortedClusters.Count; i++)
            //    {
            //        featureUsed[i] = false;
            //    }
            //}


            //List<int> bestFeaturesForIndexes = new List<int>();
            //int mostPeaks = 0;
            //int indexWithMostPeaks = 0;

            //for (int i = 0; i < featureIndexesOfData.Count; i++)
            //{
            //    if (featureIndexesOfData[i].Count > 1)
            //    {
            //        for (int j = 0; j < featureIndexesOfData[i].Count; j++)
            //        {
            //            if (sortedClusters[featureIndexesOfData[i][j]].Peaks.Count > mostPeaks)
            //            {
            //                mostPeaks = sortedClusters[featureIndexesOfData[i][j]].Peaks.Count;
            //                indexWithMostPeaks = featureIndexesOfData[i][j];
            //            }
            //        }
            //        bestFeaturesForIndexes.Add(indexWithMostPeaks);
            //        mostPeaks = 0;
            //    }
            //    else if (featureIndexesOfData[i].Count > 0)
            //    {
            //        bestFeaturesForIndexes.Add(featureIndexesOfData[i][0]);
            //    }
            //    else
            //    {
            //        bestFeaturesForIndexes.Add(-1);
            //    }
            //}

            //List<int> nonNullBestFeaturesForIndexes = new List<int>();

            //for (int i = 0; i < bestFeaturesForIndexes.Count; i++)
            //{
            //    if (bestFeaturesForIndexes[i] > 0)
            //    { 
            //        nonNullBestFeaturesForIndexes.Add(bestFeaturesForIndexes[i]);
            //    }
            //}

            //bool[] featureIndexUsed = new bool[sortedClusters.Count];
            //List<int> bestSingleFeatureIndexes = new List<int>();

            //for (int i = 0; i < sortedClusters.Count; i++)
            //{
            //    if (featureIndexUsed[nonNullBestFeaturesForIndexes[i]] == false)
            //    {
            //        bestSingleFeatureIndexes.Add(nonNullBestFeaturesForIndexes[i]);
            //        featureIndexUsed[nonNullBestFeaturesForIndexes[i]] = true;
            //    }
            //}

            //List<int> sortedBestFeatureIndexes = new List<int>();
            //sortedBestFeatureIndexes = bestSingleFeatureIndexes.OrderBy(p => p).ToList();

            //List<FeatureCP<double>> sortedFeatures = new List<FeatureCP<double>>();

            //for (int i = 0; i < sortedBestFeatureIndexes.Count; i++)
            //{
            //    sortedFeatures.Add(sortedClusters[sortedBestFeatureIndexes[i]]);
            //}
#endregion

            #region newer best cluster finder (off)
            //bool[] featureUsed = new bool[sortedClusters.Count];
            //bool peakClusterUsed=false;
            //int greatestIndex = 0;
            //int greatestFeature = 0;
            
            //int maxIndex;
            //maxIndex = sortedClusters[(sortedClusters.Count)-1].Peaks[(sortedClusters[(sortedClusters.Count)-1].Peaks.Count)-1].IndexMatch;

            //for (int index = 0;index<maxIndex;index++)
            //{
            //    List<int> bestClusterIndexes = new List<int>();
            //    for (int i=0;i<sortedClusters.Count;i++)
            //    {
            //        for (int j = 0; j < sortedClusters[i].Peaks.Count;j++ )
            //        {
            //            if ((sortedClusters[i].Peaks[j].IndexData == index)||(sortedClusters[i].Peaks[j].IndexMatch==index))
            //            {
            //                if (peakClusterUsed==false)
            //                {
            //                    bestClusterIndexes.Add(i);
            //                    peakClusterUsed=true;
            //                }
            //            }
            //        }
            //        peakClusterUsed=false;
            //    }
            //    if (bestClusterIndexes.Count > 1)
            //    {
            //        FeatureCP<double> greatestCluster = new FeatureCP<double>();
            //        for (int j = 0; j < bestClusterIndexes.Count; j++)
            //        {
            //            if (sortedClusters[bestClusterIndexes[j]].Peaks.Count > greatestIndex)
            //            {
            //                greatestIndex = sortedClusters[bestClusterIndexes[j]].Peaks.Count;
            //                greatestCluster = sortedClusters[bestClusterIndexes[j]];
            //                greatestFeature = bestClusterIndexes[j];
            //            }
            //        }
            //        greatestIndex = 0;
            //        if (featureUsed[greatestFeature]==false)
            //        {
            //            bestClusters.Add(greatestCluster);
            //            featureUsed[greatestFeature] = true;
            //        }
            //    }
            //    else if (bestClusterIndexes.Count>0)
            //    {
            //        bestClusters.Add(sortedClusters[bestClusterIndexes[0]]);
            //    }
            //}
            #endregion

            #region Exclude overlapping monoisotopic masses (off)
            //for (int i = 0; i < sortedClusters.Count; i++)
            //{
            //    for (int j = (i+1); j < sortedClusters.Count; j++)
            //    {
            //        if (sortedClusters[i].Peaks[0].IndexData == sortedClusters[j].Peaks[0].IndexData)
            //        {
            //            if (sortedClusters[i].Peaks.Count > sortedClusters[j].Peaks.Count)
            //            {
            //                if (featureUsed[i] == false)
            //                {
            //                    bestClusters.Add(sortedClusters[i]);
            //                    featureUsed[i] = true;
            //                }
            //                if ((sortedClusters[j].Peaks.Count > 1) && (featureUsed[j]==false))
            //                {
            //                    FeatureCP<double> featureHolder = new FeatureCP<double>();
            //                    for (int k = 1; k < sortedClusters[j].Peaks.Count; k++)
            //                    {
            //                        featureHolder.Peaks.Add(sortedClusters[j].Peaks[k]);
            //                    }
            //                    bestClusters.Add(featureHolder);
            //                    featureUsed[j] = true;
            //                }
            //            }
            //            else
            //            {
            //                if (featureUsed[j] == false)
            //                {
            //                    bestClusters.Add(sortedClusters[j]);
            //                    featureUsed[j] = true;
            //                }
            //                if ((sortedClusters[i].Peaks.Count > 1) && (featureUsed[i]==false))
            //                {
            //                    FeatureCP<double> featureHolder = new FeatureCP<double>();
            //                    for (int k = 1; k < sortedClusters[i].Peaks.Count; k++)
            //                    {
            //                        featureHolder.Peaks.Add(sortedClusters[i].Peaks[k]);
            //                    }
            //                    bestClusters.Add(featureHolder);
            //                }
            //                featureUsed[i] = true;
            //            }
            //        }
            //    }
            //    if (featureUsed[i] == false)
            //    {
            //        bestClusters.Add(sortedClusters[i]);
            //        featureUsed[i] = true;
            //    }
            //}
            #endregion
            #endregion

            return sortedNonRepeatingClusters;
        }
    }
}
