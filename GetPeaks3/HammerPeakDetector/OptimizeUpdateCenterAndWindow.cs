using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HammerPeakDetector.Objects;
using HammerPeakDetector.Parameters;
using PNNLOmics.Data;

namespace HammerPeakDetector
{
    public static class OptimizeUpdateCenterAndWindow
    {
        //public static bool UpdateCenterAndWindow(ref List<ClusterCP<double>> clusters, List<Peak> centroidedPeaks, DistributionParameters distributionParameters,
        //                             CreateClusters clusterFinder, List<string> logAddition, HammerThresholdParameters hammerParameters,
        //                             out double areaUnderCurve, int parameterOptimizationCutoff, int bailOutCutoff,
        //                             double tetherLengthFromAverage, double massToleranceDa, List<double> averageList, List<double> stdDevList,
        //                             Queue<double> massOfSpaceList, out double refinedCenterDa, ref int refineNumber,
        //                             ref double deviationToConverge, out double refiniedWindowDa)
        //{
        //    

        //    #region Clusters & parameterization

        //    //Finds clusters and adjusts MassToleranceDa and MassOfSpace according to stdev and average found
           
        //    //hammerParameters.SeedMassToleranceDa = clusterFinder.MassToleranceDa;
        //    //hammerParameters.SeedClusterSpacingCenter = clusterFinder.MassOfSpace;

        //    #endregion

        //    //return UpdateCenterAndWindow2(ref clusters, centroidedPeaks, distributionParameters, clusterFinder, logAddition, hammerParameters, parameterOptimizationCutoff, bailOutCutoff, tetherLengthFromAverage, massToleranceDa, averageList, stdDevList, massOfSpaceList, out refinedCenterDa, ref refineNumber, ref deviationToConverge, out refiniedWindowDa, currentStandardDeviation);
        //}

        public static bool UpdateCenterAndWindow(ref List<ClusterCP<double>> clusters, List<Peak> centroidedPeaks,
                                                   DistributionParameters distributionParameters, CreateClusters clusterFinder,
                                                   List<string> logAddition, HammerThresholdParameters hammerParameters,
                                                   int parameterOptimizationCutoff, int bailOutCutoff,
                                                   double tetherLengthFromAverage, double massToleranceDa, List<double> averageList,
                                                   List<double> stdDevList, Queue<double> massOfSpaceList, out double refinedCenterDa,
                                                   ref int refineNumber, ref double deviationToConverge,
                                                   out double refiniedWindowDa, double currentStandardDeviation)
        {
            double averageCenterPoint;
            
            #region Test optimization 2 for converging (on)//this is for iterations

            stdDevList.Add(currentStandardDeviation);
            averageList.Add(hammerParameters.SeedClusterSpacingCenter);

            #endregion

            #region Cases

            //Ends optimization if too few clusters are being produced
            if (clusters.Count < 3)
            {
                clusterFinder.MassOfSpace = distributionParameters.CenterInitialDa;
                clusterFinder.MassToleranceDa = distributionParameters.WindowsSizeInitialDa;
                clusters = clusterFinder.FindClusters(centroidedPeaks);
                logAddition.Add("Scan has less than 3 clusters");

                refinedCenterDa = distributionParameters.CenterInitialDa;
                refiniedWindowDa = distributionParameters.WindowsSizeInitialDa;
                return true;
            }

            refinedCenterDa = clusterFinder.MassOfSpace;

            //Creates list with 3 most recent massOfSpace values
            if (massOfSpaceList.Count < 3)
            {
                massOfSpaceList.Enqueue(refinedCenterDa);
            }
            else
            {
                massOfSpaceList.Dequeue();
                massOfSpaceList.Enqueue(refinedCenterDa);
            }

            if (massOfSpaceList.Count == 3)
            {
                List<double> queueArray = massOfSpaceList.ToList();
                averageCenterPoint = (queueArray[0] + queueArray[1])/2;
                deviationToConverge = Math.Abs((queueArray[2] - averageCenterPoint))/averageCenterPoint*1000000;
            }

            #endregion

            int iterationValue = Convert.ToInt32(Math.Round(deviationToConverge, 0));
            int iterationCutoff = parameterOptimizationCutoff;
            logAddition.Add("Scan has " + refinedCenterDa.ToString() + " delta with a window of +-" + massToleranceDa.ToString() +
                            " iteration " + iterationValue + "/" + iterationCutoff);

            #region Escape in case optimization fails

            //This is if this will not optimize and is tethered by a mass difference and refine iteration number
            double differenceFromDefault = Math.Abs(refinedCenterDa - distributionParameters.CenterInitialDa);
            if (refineNumber > bailOutCutoff || differenceFromDefault > tetherLengthFromAverage)
            {
                clusterFinder.MassOfSpace = distributionParameters.CenterInitialDa;
                clusterFinder.MassToleranceDa = distributionParameters.WindowsSizeInitialDa;

                clusters = clusterFinder.FindClusters(centroidedPeaks);
                logAddition.Add("Scan has failed to converge");

                refinedCenterDa = distributionParameters.CenterInitialDa;
                refiniedWindowDa = distributionParameters.WindowsSizeInitialDa;
                return true;
            }

            #endregion

            //Write to log and set up for stdev
            foreach (ClusterCP<double> currentCluster in clusters)
            {
                int peakIndex = 0; //peakIndex can also be cluster.Peaks.Count - 1 for last peak
                int charge = currentCluster.Charge;
                double difference = charge*(currentCluster.Peaks[peakIndex].Value2 - currentCluster.Peaks[peakIndex].Value1);
                logAddition.Add("Scan has " + difference.ToString() + " delta with a window of +-" +
                                distributionParameters.WindowsSizeDa.ToString() + " iteration " + -1 + "/" + -1);
            }

            //distributionParameters.WindowsSizeDa = currentStandardDeviation;
            //refinedCenterDa = 
            refiniedWindowDa = currentStandardDeviation;

            refineNumber++;
            return false;
        }
    }
}
