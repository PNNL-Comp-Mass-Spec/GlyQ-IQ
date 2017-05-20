using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiDatasetToolBox
{
    public static class HistogramGenerator
    {
        public static List<double> Generate(List<double> results, double minRange, double maxRange, int numberOfBuckets)
        {
            
            if (results != null && results.Count > 0)
            {
                MathNet.Numerics.Statistics.Histogram myHistogram = new MathNet.Numerics.Statistics.Histogram(results, numberOfBuckets, 0, maxRange);



                double[] histogramResults = new double[numberOfBuckets];
                for (int i = 0; i < numberOfBuckets; i++)
                {
                    histogramResults[i] = myHistogram[i].Count;
                }

                return histogramResults.ToList();
            }
            else
            {
                return null;
            }
        }

        public static List<double> GenerateAxis(double minRange, double maxRange, int numberOfBuckets)
        {
            List<double> axis = new List<double>();
            for (int i = 0; i < numberOfBuckets; i++)
            {
                axis.Add(minRange + maxRange / numberOfBuckets * i);
            }
            
            return axis;
        }
    }
}
