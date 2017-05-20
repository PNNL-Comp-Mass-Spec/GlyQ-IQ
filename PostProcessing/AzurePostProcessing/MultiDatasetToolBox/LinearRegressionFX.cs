using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiDatasetToolBox
{
    public static class LinearRegressionFX
    {
        public static void Calculate(double[] x, double[] y, ref double slope, ref double intercept)
        {

            int lengthofData = x.Length;
            double sX = x.Sum();
            double sY = y.Sum();
            double sXX = 0;
            double sXY = 0;
            double sYY = 0;
            for (int i = 0; i < lengthofData; i++)
            {
                sXX += x[i] * x[i];
                sXY += x[i] * y[i];
                sYY += y[i] * y[i];
            }

            slope = (lengthofData * sXY - sX * sY) / (lengthofData * sXX - sX * sX);
            intercept = sY / lengthofData - slope / lengthofData * sX;
        }

        /// <summary>
        /// apply alignment coeffficents to experimentalData
        /// </summary>
        /// <param name="slope"></param>
        /// <param name="intercept"></param>
        /// <param name="experimentalData"></param>
        /// <returns></returns>
        public static List<double> ApplyAlgnmentCoefficients(double slope, double intercept, double[] experimentalData)
        {
            List<double> alignedData = new List<double>();
            foreach (double uncalibrated in experimentalData)
            {
                double calculatedError = (uncalibrated * slope + intercept);
                double calibrated = uncalibrated - calculatedError;
                alignedData.Add(calibrated);
            }
            return alignedData;
        }

        /// <summary>
        /// apply alignment coeffficents to experimentalData
        /// </summary>
        /// <param name="slope"></param>
        /// <param name="intercept"></param>
        /// <param name="experimentalData"></param>
        /// <returns></returns>
        public static double ApplyAlgnmentCoefficients(double slope, double intercept, double experimentalData)
        {
            double[] experimentalDataList = new double[1];
            experimentalDataList[0] = experimentalData;
            List<double> alignedData =  ApplyAlgnmentCoefficients(slope, intercept, experimentalDataList);

            if (alignedData != null)
            {
                return alignedData[0];
            }
            else
            {
                return 0;
            }
        }
    }
}
