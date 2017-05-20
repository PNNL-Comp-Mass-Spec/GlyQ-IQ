using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using OrbitrapPeakDetection;
using OrbitrapPeakDetection.Objects;
using HammerPeakDetector;

namespace OrbitrapPeakDetection
{
    public class RelativeNoiseThreshold
    {
        //Establishes a threshold for what will be considered noise vs. signal in this scan
        public List<PNNLOmics.Data.Peak> FindRelativeNoiseThresholds(List<PNNLOmics.Data.Peak> data, HammerThresholdParameters parameters)
        {
            List<PNNLOmics.Data.Peak> relativeThreshold = new List<PNNLOmics.Data.Peak>();

            //Loads in data regarding the last peaks of each feature
            LastPeaks lastPeaks = new LastPeaks();


            List<double> lastPeaksIndexes = new List<double>();
            lastPeaksIndexes = lastPeaks.FindLastPeaks(data);

            //Finds intensities of last peaks
            List<double> intensity = new List<double>();
            for (int i = 0; i < lastPeaksIndexes.Count; i++)
            {
                intensity.Add(data[Convert.ToInt32(lastPeaksIndexes[i])].Height);
            }

            //Sets minimum size of a noise region
            int minNumPointsPerRegion = parameters.MinimumSizeOfRegion;
            //Sets the noise threshold a certain number of sigma above or below the average of the last peaks
            double sigmaMultiplier = parameters.SigmaMultiplier;

            int numberOfClusters = lastPeaksIndexes.Count;
            NumberOfPointsPerRegionFinder findNumberOfPointsPerRegion = new NumberOfPointsPerRegionFinder();
            //Using this cluster number only works if the data is filtered
            List<int> numPerThreshold = findNumberOfPointsPerRegion.FindNumberOfPointsPerRegionList(numberOfClusters, minNumPointsPerRegion);

            //loop variables
            int point = 0;
            double average = 0;
            double regionTotal = 0;

            StandardDeviation thresholds = new StandardDeviation();
            double currentNoiseThreshold = new double();
            int noiseThresholdNum = 0;

            ///Loop establishes relative noise thresholds for different regions of data using SigmaMultiplier parameter
            while (point < intensity.Count)
            {
                List<double> intensitiesForRegion = new List<double>();
                for (int i = 0; i < numPerThreshold[noiseThresholdNum]; i++)
                {
                    regionTotal+=intensity[point];
                    intensitiesForRegion.Add(intensity[point]);
                    point++;
                }
                average=regionTotal/numPerThreshold[noiseThresholdNum];
                currentNoiseThreshold = average+(sigmaMultiplier*(thresholds.FindStandardDeviation(intensitiesForRegion)));
                PNNLOmics.Data.Peak newPeak = new Peak();
                newPeak.XValue = data[Convert.ToInt32(lastPeaksIndexes[point - 1])].XValue;
                newPeak.Height = currentNoiseThreshold;
                newPeak.Background = Convert.ToSingle(currentNoiseThreshold);
                relativeThreshold.Add(newPeak);
                //relativeThreshold.Add(new XYData(data[Convert.ToInt32(lastPeaksIndexes[point-1])].XValue,currentNoiseThreshold));
                regionTotal=0;
                noiseThresholdNum++;
            }

            return relativeThreshold;
        }

        public List<PNNLOmics.Data.Peak> FindRelativeNoiseThresholds(List<PNNLOmics.Data.Peak> data, List<ClusterCP<double>> peakClusters, HammerThresholdParameters parameters)
        {
            List<PNNLOmics.Data.Peak> relativeThreshold = new List<PNNLOmics.Data.Peak>();

            //Loads in data regarding the last peaks of each feature
            LastPeaks lastPeaks = new LastPeaks();


            List<double> lastPeaksIndexes = new List<double>();
            lastPeaksIndexes = lastPeaks.FindLastPeaks(data, peakClusters);

            //Finds intensities of last peaks
            List<double> intensity = new List<double>();
            for (int i = 0; i < lastPeaksIndexes.Count; i++)
            {
                intensity.Add(data[Convert.ToInt32(lastPeaksIndexes[i])].Height);
            }

            //Sets minimum size of a noise region
            int minNumPointsPerRegion = parameters.MinimumSizeOfRegion;
            //Sets the noise threshold a certain number of sigma above or below the average of the last peaks
            double sigmaMultiplier = parameters.SigmaMultiplier;

            int numberOfClusters = lastPeaksIndexes.Count;
            NumberOfPointsPerRegionFinder findNumberOfPointsPerRegion = new NumberOfPointsPerRegionFinder();
            //Using this cluster number only works if the data is filtered
            List<int> numPerThreshold = findNumberOfPointsPerRegion.FindNumberOfPointsPerRegionList(numberOfClusters, parameters.MinimumSizeOfRegion);

            //loop variables
            int point = 0;
            double average = 0;
            double regionTotal = 0;

            StandardDeviation thresholds = new StandardDeviation();
            double currentNoiseThreshold = new double();
            int noiseThresholdNum = 0;

            ///Loop establishes relative noise thresholds for different regions of data using SigmaMultiplier parameter
            while (point < intensity.Count)
            {
                List<double> intensitiesForRegion = new List<double>();
                for (int i = 0; i < numPerThreshold[noiseThresholdNum]; i++)
                {
                    regionTotal += intensity[point];
                    intensitiesForRegion.Add(intensity[point]);
                    point++;
                }
                average = regionTotal / numPerThreshold[noiseThresholdNum];
                currentNoiseThreshold = average + (sigmaMultiplier * (thresholds.FindStandardDeviation(intensitiesForRegion)));
                PNNLOmics.Data.Peak newPeak = new Peak();
                newPeak.XValue = data[Convert.ToInt32(lastPeaksIndexes[point - 1])].XValue;
                newPeak.Height = currentNoiseThreshold;
                newPeak.Background = Convert.ToSingle(currentNoiseThreshold);
                relativeThreshold.Add(newPeak);
                //relativeThreshold.Add(new XYData(data[Convert.ToInt32(lastPeaksIndexes[point - 1])].XValue, currentNoiseThreshold));
                regionTotal = 0;
                noiseThresholdNum++;
            }

            return relativeThreshold;
        }
    }
}