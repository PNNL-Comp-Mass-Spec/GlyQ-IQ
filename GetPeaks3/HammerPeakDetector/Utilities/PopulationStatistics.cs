using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.Statistics;
using HammerPeakDetector.Objects;
using MathNet.Numerics;

namespace HammerPeakDetector.Utilities
{
    public class PopulationStatistics
    {
        /// <summary>
        /// Find t-statistic based on a list of samples and population mean
        /// </summary>
        /// <param name="sampleList"></param>
        /// <param name="populationMean"></param>
        /// <returns></returns>
        public double FindTStatistic(List<double> sampleList, double populationMean)
        {
            double sampleMean = Statistics.Mean(sampleList);
            double sampleStdDev = Statistics.StandardDeviation(sampleList);
            double tStat = (sampleMean - populationMean) / (sampleStdDev / Math.Sqrt(sampleList.Count));
            return tStat;
        }

        /// <summary>
        /// Find z-score based on a sample and list of all values in the population
        /// </summary>
        /// <param name="sampleList"></param>
        /// <param name="populationMean"></param>
        /// <returns></returns>
        public double FindZScore(double sample, List<double> population)
        {
            double mean = Statistics.Mean(population);
            double stdDev = Statistics.StandardDeviation(population);
            double zScore = (sample - mean) / stdDev;
            return zScore;
        }

        /// <summary>
        /// Find z-score based on a sample, mean, and standard deviation
        /// </summary>
        /// <param name="sampleList"></param>
        /// <param name="populationMean"></param>
        /// <returns></returns>
        public double FindZScore(double sample, double stdDev, double mean)
        {
            double zScore = (sample - mean) / stdDev;
            return zScore;
        }

        /// <summary>
        /// Find list of z-score based on a list of all values in the population
        /// </summary>
        /// <param name="sampleList"></param>
        /// <param name="populationMean"></param>
        /// <returns></returns>
        public List<double> FindZScore(List<double> population)
        {
            List<double> zScores = new List<double>();
            double mean = Statistics.Mean(population);
            double stdDev = Statistics.StandardDeviation(population);
            foreach (double currentSample in population)
            {
                zScores.Add((currentSample - mean) / (stdDev));
            }
            return zScores;
        }

        /// <summary>
        /// Finds p-value based on mass differences in clusters compared to overall average and standard deviation of cluster differences
        /// </summary>
        /// <param name="cluster"></param>
        /// <param name="index"></param>
        /// <param name="average"></param>
        /// <param name="standardDeviation"></param>
        /// <returns></returns>
        public double FindClusterPValue(ClusterCP<double> cluster, int index, double average, double standardDeviation)
        {
            double pValue = 0;
            double difference = (cluster.Peaks[index].Value2 - cluster.Peaks[index].Value1) * cluster.Charge;

            double sigmaDifference = Math.Abs((average - difference) / standardDeviation);

            //pValue = GetPeaks_DLL.Functions.ConvertSigmaAndPValue.SigmaToPvalue(sigmaDifference);

            pValue = SpecialFunctions.Erf(sigmaDifference / Math.Sqrt(2));

            return pValue;
        }
    }
}
