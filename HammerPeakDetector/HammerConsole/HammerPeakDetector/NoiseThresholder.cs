using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HammerPeakDetector.Objects.GetPeaksDifferenceFinder;
using PNNLOmics.Data;
using HammerPeakDetector.Objects;
using HammerPeakDetector.Utilities;
using HammerPeakDetector.Parameters;
using PNNLOmics.Data.Peaks;

namespace HammerPeakDetector
{
    public class NoiseThresholder
    {

        #region Old method with Peaks (off)
        ////Creates moving noise threshold; uses 15 previous 'last peaks in cluster' and next 15 'last peaks in cluster' to calculate a noise
        ////threshold relevant to the current cluster of peaks
        //public void MovingThreshold(List<Peak> data, List<ClusterCP<double>> peakClusters, HammerThresholdParameters parameters)
        //{
        //    //Last index to be given a Background value
        //    int nextToCheck = 0;
        //    int minNumPerThreshold = parameters.MinimumSizeOfRegion;

        //    //Creates a noise threshold relevant to each individual cluster
        //    for (int i = 0; i < peakClusters.Count; i++)
        //    {
        //        double noiseThreshold = 0;
                
        //        //Determines start and end point of peaks to be considered in current cluster
        //        int startPoint = i - (minNumPerThreshold + 1) / 2;
        //        int endPoint = i + minNumPerThreshold / 2;
        //        //This if-else is used in the case that the peak is one of the first or last 15 'last peaks in cluster' thus making
        //        //it impossible to take 15 peaks before and after; start and end points are appropriately altered
        //        if (peakClusters.Count < minNumPerThreshold + 1)
        //        {
        //            startPoint = 0;
        //            endPoint = peakClusters.Count - 1;
        //        }
        //        else
        //        {
        //            if (i <= (minNumPerThreshold + 1) / 2)
        //            {
        //                startPoint = 0;
        //                endPoint = minNumPerThreshold;
        //            }
        //            else if (i >= peakClusters.Count - minNumPerThreshold / 2)
        //            {
        //                endPoint = peakClusters.Count - 1;
        //                startPoint = endPoint - minNumPerThreshold;
        //            }
        //        }

        //        //Calculates height of noise threshold by averaging 'last peaks in cluster' from startPoint to endPoint
        //        for (int j = startPoint; j <= endPoint; j++)
        //        {
        //            noiseThreshold += data[peakClusters[j].Peaks[peakClusters[j].Peaks.Count - 1].IndexMatch].Height;
        //        }
        //        noiseThreshold /= (minNumPerThreshold + 1);

        //        float convertedThreshold = (float)noiseThreshold;

        //        //Sets Background of each peak in the current cluster to noiseThreshold
        //        foreach (DifferenceObject<double> peak in peakClusters[i].Peaks)
        //        {
        //            data[peak.IndexData].Background = convertedThreshold;
        //        }
        //        //Sets Background of each non-clustered peak to the Background of the nearest 'last peak in cluster'
        //        for (int j = nextToCheck; j < peakClusters[i].Peaks[0].IndexData; j++)
        //        {
        //            if (data[j].Background == 0)
        //            {
        //                data[j].Background = convertedThreshold;
        //            }
        //        }
        //        nextToCheck = peakClusters[i].Peaks[0].IndexData + 1;
        //    }

        //    //Ensures every peak is given a Background value
        //    for (int j = nextToCheck; j < data.Count; j++)
        //    {
        //        if (data[j].Background == 0)
        //        {
        //            data[j].Background = data[peakClusters[peakClusters.Count - 1].Peaks[0].IndexMatch].Background;
        //        }
        //    }
        //}

#endregion

        /// <summary>
        /// Calculate moving threshold using clusters and a list of Peaks
        /// </summary>
        /// <param name="data"></param>
        /// <param name="peakClusters"></param>
        /// <param name="parameters"></param>
        public void MovingThreshold(List<Peak> data, List<ClusterCP<double>> peakClusters, HammerThresholdParameters parameters)
        {
            //Last index to be given a Background value
            int nextToCheck = 0;
            int minNumPerThreshold = parameters.MinimumSizeOfRegion;

            //Creates a noise threshold relevant to each individual cluster
            for (int i = 0; i < peakClusters.Count; i++)
            {
                //double noiseThreshold = 0;

                //Determines start and end point of peaks to be considered in current cluster
                int startPoint = i - (minNumPerThreshold + 1) / 2;
                int endPoint = i + minNumPerThreshold / 2;
                //This if-else is used in the case that the peak is one of the first or last 15 'last peaks in cluster' thus making
                //it impossible to take 15 peaks before and after; start and end points are appropriately altered
                if (peakClusters.Count < minNumPerThreshold + 1)
                {
                    startPoint = 0;
                    endPoint = peakClusters.Count - 1;
                }
                else if (i <= (minNumPerThreshold + 1) / 2)
                {
                    startPoint = 0;
                    endPoint = minNumPerThreshold;
                }
                else if (i >= peakClusters.Count - minNumPerThreshold / 2)
                {
                    endPoint = peakClusters.Count - 1;
                    startPoint = endPoint - minNumPerThreshold;
                }

                List<double> heightList = new List<double>();

                //Calculates height of noise threshold by averaging 'last peaks in cluster' from startPoint to endPoint
                for (int j = startPoint; j <= endPoint; j++)
                {
                    heightList.Add(data[peakClusters[j].Peaks[peakClusters[j].Peaks.Count - 1].IndexMatch].Height);
                }
                double mean = MathNet.Numerics.Statistics.Statistics.Mean(heightList);
                double stdDev = MathNet.Numerics.Statistics.Statistics.StandardDeviation(heightList);

                //float convertedThreshold = (float)noiseThreshold;

                float convertedThreshold = (float)(mean + (parameters.ThresholdSigmaMultiplier * stdDev));

                //Sets Background of each peak in the current cluster to noiseThreshold
                foreach (DifferenceObject<double> peak in peakClusters[i].Peaks)
                {
                    data[peak.IndexData].Background = convertedThreshold;
                }
                //Sets Background of each non-clustered peak to the Background of the nearest 'last peak in cluster'
                for (int j = nextToCheck; j < peakClusters[i].Peaks[0].IndexData; j++)
                {
                    if (data[j].Background == 0)
                    {
                        data[j].Background = convertedThreshold;
                    }
                }
                nextToCheck = peakClusters[i].Peaks[0].IndexData + 1;
            }

            //Ensures every peak is given a Background value
            for (int j = nextToCheck; j < data.Count; j++)
            {
                if (data[j].Background == 0)
                {
                    data[j].Background = data[peakClusters[peakClusters.Count - 1].Peaks[0].IndexMatch].Background;
                }
            }
        }

        /// <summary>
        /// Calculate moving threshold using clusters and a list of ProcessedPeaks
        /// </summary>
        /// <param name="data"></param>
        /// <param name="peakClusters"></param>
        /// <param name="parameters"></param>
        public void MovingThreshold(List<ProcessedPeak> data, List<ClusterCP<double>> peakClusters, HammerThresholdParameters parameters)
        {
            //Last index to be given a Background value
            int nextToCheck = 0;
            int minNumPerThreshold = parameters.MinimumSizeOfRegion;

            //Creates a noise threshold relevant to each individual cluster
            for (int i = 0; i < peakClusters.Count; i++)
            {
                //double noiseThreshold = 0;

                //Determines start and end point of peaks to be considered in current cluster
                int startPoint = i - (minNumPerThreshold + 1) / 2;
                int endPoint = i + minNumPerThreshold / 2;
                //This if-else is used in the case that the peak is one of the first or last 15 'last peaks in cluster' thus making
                //it impossible to take 15 peaks before and after; start and end points are appropriately altered
                if (peakClusters.Count < minNumPerThreshold + 1)
                {
                    startPoint = 0;
                    endPoint = peakClusters.Count - 1;
                }
                else if (i <= (minNumPerThreshold + 1) / 2)
                {
                    startPoint = 0;
                    endPoint = minNumPerThreshold;
                }
                else if (i >= peakClusters.Count - minNumPerThreshold / 2)
                {
                    endPoint = peakClusters.Count - 1;
                    startPoint = endPoint - minNumPerThreshold;
                }

                List<double> heightList = new List<double>();

                //Calculates height of noise threshold by averaging 'last peaks in cluster' from startPoint to endPoint
                for (int j = startPoint; j <= endPoint; j++)
                {
                    heightList.Add(data[peakClusters[j].Peaks[peakClusters[j].Peaks.Count - 1].IndexMatch].Height);
                }
                double mean = MathNet.Numerics.Statistics.Statistics.Mean(heightList);
                double stdDev = MathNet.Numerics.Statistics.Statistics.StandardDeviation(heightList);

                //float convertedThreshold = (float)noiseThreshold;

                float convertedThreshold = (float)(mean + (parameters.ThresholdSigmaMultiplier * stdDev));

                //Sets Background of each peak in the current cluster to noiseThreshold
                foreach (DifferenceObject<double> peak in peakClusters[i].Peaks)
                {
                    data[peak.IndexData].Background = convertedThreshold;
                }
                //Sets Background of each non-clustered peak to the Background of the nearest 'last peak in cluster'
                for (int j = nextToCheck; j < peakClusters[i].Peaks[0].IndexData; j++)
                {
                    if (data[j].Background == 0)
                    {
                        data[j].Background = convertedThreshold;
                    }
                }
                nextToCheck = peakClusters[i].Peaks[0].IndexData + 1;
            }

            //Ensures every peak is given a Background value
            for (int j = nextToCheck; j < data.Count; j++)
            {
                if (data[j].Background == 0)
                {
                    data[j].Background = data[peakClusters[peakClusters.Count - 1].Peaks[0].IndexMatch].Background;
                }
            }
        }

        #region Old thresholder ThresholdRegions
        public void ThresholdRegions(List<Peak> data, List<ClusterCP<double>> peakClusters, HammerThresholdParameters parameters)
        {
            //Finds the number of points that should be in each noise threshold region
            PointsPerRegion pointsPerRegionFinder = new PointsPerRegion();
            pointsPerRegionFinder.FindPointsPerRegion(peakClusters.Count, parameters);
            List<int> pointsPerRegion = parameters.NumberOfPointsPerNoiseRegion;

            //Dictionary mapping noise regions to the max index they should affect
            Dictionary<int, double> noiseRegions = new Dictionary<int, double>();
            int index = 0;
            double currentAverage = 0;
            
            //Finds height of noise thresholds for each region
            foreach (int regionSize in pointsPerRegion)
            {
                currentAverage = 0;
                for (int i = 0; i < regionSize; i++)
                {
                    currentAverage += data[peakClusters[index].Peaks[peakClusters[index].Peaks.Count - 1].IndexMatch].Height;
                    index++;
                }
                currentAverage /= regionSize;
                noiseRegions.Add(peakClusters[index - 1].Peaks[peakClusters[index - 1].Peaks.Count - 1].IndexMatch, currentAverage);
            }

            //First index to receive a Background value in the loop
            int firstIndex = 0;
            
            //Sets Background from firstIndex to currentIndex
            foreach (int currentIndex in noiseRegions.Keys)
            {
                currentAverage = noiseRegions[currentIndex];
                for (int i = firstIndex; i < currentIndex; i++)
                {
                    data[i].Background = (float)currentAverage;
                }
                firstIndex = currentIndex + 1;
            }

            //Ensures all peaks are given a Background value
            for (int i = firstIndex; i < data.Count; i++)
            {
                data[i].Background = (float)currentAverage;
            }
        }

#endregion
    }
}
