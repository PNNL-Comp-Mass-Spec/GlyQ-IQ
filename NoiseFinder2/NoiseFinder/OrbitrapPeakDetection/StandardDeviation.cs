using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

namespace OrbitrapPeakDetection
{
    public class StandardDeviation
    {
        /// <summary>
        /// Finds standard deviation of a list of doubles
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public double FindStandardDeviation(List<double> data)
        {
            double total = 0;
            double mean = 0;
            double numberOfData = data.Count;
            double deviationTotal = 0;
            double standardDeviation = 0;

            //Accumulate sum of intensities
            for (int i = 0; i < numberOfData; i++)
            {
                total += data[i];
            }

            //Find mean
            mean = total / numberOfData;

            for (int i = 0; i < numberOfData; i++)
            {
                deviationTotal+=Math.Pow((mean - (data[i])), 2);
            }

            //Find standard deviation
            standardDeviation = Math.Sqrt(deviationTotal / (numberOfData - 1));

            return standardDeviation;
        }

        public double FindStandardDeviation(List<double> data, out double mean)
        {
            double total = 0;
            mean = 0;
            double numberOfData = data.Count;
            double deviationTotal = 0;
            double standardDeviation = 0;

            //Accumulate sum of intensities
            for (int i = 0; i < numberOfData; i++)
            {
                total += data[i];
            }

            //Find mean
            mean = total / numberOfData;

            for (int i = 0; i < numberOfData; i++)
            {
                deviationTotal += Math.Pow((mean - (data[i])), 2);
            }

            //Find standard deviation
            standardDeviation = Math.Sqrt(deviationTotal / (numberOfData - 1));

            return standardDeviation;
        }

        public void FindMaxAndMin(List<double> data, out double max, out double min)
        {
            min = data[0];
            max = data[0];

            double test;
            for (int i = 0; i < data.Count; i++)
            {
                test = data[i];
                if(test > max)
                {
                    max = test;
                }
                if(test < min)
                {
                    min = test;
                }
            }
        }
    }
}
