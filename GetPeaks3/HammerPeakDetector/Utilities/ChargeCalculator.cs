using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HammerPeakDetector.Objects;

namespace HammerPeakDetector.Utilities
{
    public static class ChargeCalculator
    {
        public static void CalculateChargeStateFromDifference(ref List<ClusterCP<double>> featureList)
        {
            double charge;
            ///Calculate charge based on mass difference in cluster
            foreach (ClusterCP<double> cluster in featureList)
            {
                if (cluster.Peaks.Count > 0)
                {
                    charge = 1 / cluster.Peaks[0].Difference;
                    cluster.Charge = Convert.ToInt32(Math.Round(charge, 0));
                }
            }
        }
    }
}
