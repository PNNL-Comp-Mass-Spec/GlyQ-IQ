using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using HammerPeakDetector.Objects;
using MathNet.Numerics;
using HammerPeakDetector.Parameters;
using PNNLOmics.Data.Peaks;

namespace HammerPeakDetector.Utilities
{
    public class PValue
    {
        /// <summary>
        /// Finds p-value for a list of processed peaks based on mass differences in clustering
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="clusters"></param>
        /// <param name="peaks"></param>
        public void SetPValue(HammerThresholdParameters parameters, List<ClusterCP<double>> clusters, List<ProcessedPeak> peaks)
        {
            //Statistics for Gaussian distribution
            double stdDev = parameters.SeedMassToleranceDa / parameters.MassTolleranceSigmaMultiplier;
            double average = parameters.SeedClusterSpacingCenter;

            PopulationStatistics statistics = new PopulationStatistics();

            //Sets all p-values to -1 so that at the end it is apparent which peaks are unclustered since clustered peaks' p-values
            //will be between 0 and 1
            foreach (ProcessedPeak peak in peaks)
            {
                peak.Pvalue = -1;
            }
                       
            //Finds p-values for all clustered peaks
            foreach (ClusterCP<double> cluster in clusters)
            {
                //P-value for a peak is the average of the peak's two p-values; lastPValue keeps track of the preceding p-value
                double previousPValue = statistics.FindClusterPValue(cluster, 0, average, stdDev);

                //First and last peaks in cluster have only one p-value under consideration
                peaks[cluster.Peaks[0].IndexData].Pvalue = previousPValue;

                //Computes p-value for peaks 1 to n - 1 for all clusters with max cluster index n
                for (int i = 1; i < cluster.Peaks.Count; i++)
                {
                    double currentPValue = statistics.FindClusterPValue(cluster, i, average, stdDev);
                    peaks[cluster.Peaks[i].IndexData].Pvalue = (previousPValue + currentPValue) / 2;

                    previousPValue = currentPValue;
                }

                //First and last peaks in cluster have only one p-value under consideration                
                peaks[cluster.Peaks[cluster.Peaks.Count - 1].IndexMatch].Pvalue = previousPValue;
            }

            #region Old methods (off)
            #region Old method (off)
            //for (int i = 0; i < peaks.Count; i++)
            //{
            //    double pValue = -1;
            //    if (differenceDictionary.ContainsKey(i))
            //    {
            //        double value = differenceDictionary[i];
            //        double x = value - parameters.SeedClusterSpacingCenter + center;
            //        if (x > center)
            //        {
            //            x = max - x;
            //        }

            //        //Lorentzian
            //        pValue = ((1 / (Math.PI)) * (Math.Atan(x)) * 2) / areaUnderCurve;
            //    }

            //    peaks[i].Pvalue = pValue;
            //}

            #endregion

            #region Old method (off)
            //foreach (KeyValuePair<int, double> pair in differenceDictionary)
            //{
            //    double x = pair.Value - parameters.SeedClusterSpacingCenter + center;
            //    if (x > center)
            //    {
            //        x = max - x;
            //    }

            //    double pValue = 0;

            //    if (x > max)
            //    {
            //        pValue = -1;
            //    }
            //    else
            //    {
            //        pValue = ((1 / (Math.PI)) * (Math.Atan(x)) * 2) / areaUnderCurve;
            //    }
            //    peaks[pair.Key - 1].Pvalue = pValue;
            //}

            //foreach (ProcessedPeak peak in peaks)
            //{
            //    if (peak.Pvalue == 0)
            //    {
            //        peak.Pvalue = -1;
            //    }
            //}

            #endregion

            #endregion
        
        }
    }
}
