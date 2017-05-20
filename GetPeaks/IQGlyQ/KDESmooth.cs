using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQGlyQ
{
    public static class KDESmooth
    {
        // TODO: Verify this actually works --- Da's code, slightly modified by Jeff
        public static List<double> Smooth(List<double> yValueList, double bandwidth)
        {
            int numPoints = yValueList.Count;

            List<double> newYValueList = new List<double>();

            for (int i = 0; i < numPoints; i++)
            {
                double sumWInv = 0;
                double sumXoW = 0;
                double sumX2oW = 0;
                double sumYoW = 0;
                double sumXYoW = 0;

                for (int j = 0; j < numPoints; j++)
                {
                    double x = j;
                    double y = yValueList[j];
                    double standardized = Math.Abs(x - i) / bandwidth;
                    double w = 0;
                    if (standardized < 6)
                    {
                        w = (2 * Math.Sqrt(2 * Math.PI) * Math.Exp(-2 * standardized * standardized));
                        sumWInv += 1 / w;
                    }
                    sumXoW += x * w;
                    sumX2oW += x * x * w;
                    sumYoW += y * w;
                    sumXYoW += x * y * w;
                }

                double intercept = 1 / (sumWInv * sumX2oW - sumXoW * sumXoW) * (sumX2oW * sumYoW - sumXoW * sumXYoW);
                double slope = 1 / (sumWInv * sumX2oW - sumXoW * sumXoW) * (sumWInv * sumXYoW - sumXoW * sumYoW);
                newYValueList.Add(intercept + slope * i);
            }

            return newYValueList;
        }
    }
}
