using System;
using System.Collections.Generic;
using HammerPeakDetector.Objects;

namespace HammerPeakSupport
{
    public static class ChargeCalculator
    {
        public static void CalculateChargeStateFromDifference(ref List<ClusterCP<double>> featureList)
        {
            double charge;
            ///Calculate charge based on mass difference in cluster
            foreach (ClusterCP<double> group in featureList)
            {
                if (group.Peaks.Count > 0)
                {
                    charge = 1 / group.Peaks[0].Difference;
                    group.Charge = Convert.ToInt32(Math.Round(charge, 0));
                }
            }
        }
    }
}
