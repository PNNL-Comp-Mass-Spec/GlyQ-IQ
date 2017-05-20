using System.Collections.Generic;
using HammerPeakDetector.Parameters;
using PNNLOmics.Data;
using HammerPeakDetector.Objects;

namespace HammerPeakDetector
{
    public class Optimize
    {
        /// <summary>
        /// Optimizes mass tolerance and mass of space used in clustering
        /// </summary>
        /// <param name="clusters"></param>
        /// <param name="massOfSpaceDefault"></param>
        /// <param name="centroidedPeaks"></param>
        /// <param name="massOfErrorDaDefault"></param>
        /// <param name="clusterFinder"></param>
        /// <param name="massOfSpace"></param>
        /// <param name="newMassOfSpace"></param>
        /// <param name="newStandardDeviation"></param>
        /// <param name="logAddition"></param>
        /// <param name="hammerParameters"></param>
        /// <returns></returns>
        //public List<ClusterCP<double>> OptimizeDifferences(List<ClusterCP<double>> clusters, double massOfSpaceDefault, List<Peak> centroidedPeaks, double massOfErrorDaDefault, CreateClusters clusterFinder, double massOfSpace, out double newMassOfSpace, out double newStandardDeviation, out List<string> logAddition, HammerThresholdParameters parameters, out double areaUnderCurve, int sigmaForOptimization)
        public List<ClusterCP<double>> OptimizeDifferences(List<ClusterCP<double>> clusters, List<Peak> centroidedPeaks, string writeFolder, bool writeDiagnostics , out List<string> logAddition, HammerThresholdParameters hammerParameters, out double areaUnderCurve)
        
        {
            //Variables
            logAddition = new List<string>();
            CreateClusters clusterFinder = new CreateClusters(hammerParameters);
            DistributionParameters distributionParameters = new DistributionParameters(hammerParameters);
            
            double massToleranceDa = clusterFinder.MassToleranceDa;
            double centerDa = distributionParameters.CenterInitialDa;
            double refiniedWindowDa = distributionParameters.WindowsSizeInitialDa;

            double averageCenterPoint = distributionParameters.CenterDa;
            double deviationToConverge = 500000;
            int refineNumber = 0;
            const int bailOutCutoff = 10;
            const double tetherLengthFromAverage = 0.03;
            Queue<double> massOfSpaceList = new Queue<double>();
            int parameterOptimizationCutoff = 100;
            areaUnderCurve = 0;

            

            #region Test optimization 1 (on)
            List<double> stdDevList = new List<double>();
            List<double> averageList = new List<double>();

            #endregion

            //int iterations = 0; //0 = run until mass tolerance and mass of space converge
            //int iterations = 1; //1 = 1 iteration
            int iterations = hammerParameters.Iterations;

            //-1 iteration: run to convergence; 0 iteration: no optimization; 1 iteration: run optimization once
            if(iterations == -1)
            {
                //TODO under construction
                
                //Runs until mass tolerance and mass of space converge
                while (deviationToConverge > parameterOptimizationCutoff)
                {
                    double currentStandardDeviation;
                    double currrentCenter;
                    clusters = clusterFinder.RefineClusters(clusters, centroidedPeaks, hammerParameters, writeFolder, writeDiagnostics, out currentStandardDeviation, out currrentCenter, refineNumber, out areaUnderCurve);

                    
                    bool canWeUpdate = true;
                    //canWeUpdate = OptimizeUpdateCenterAndWindow.UpdateCenterAndWindow(ref clusters, centroidedPeaks, distributionParameters, clusterFinder, logAddition, hammerParameters, out areaUnderCurve, parameterOptimizationCutoff, bailOutCutoff, tetherLengthFromAverage, massToleranceDa, averageList, stdDevList, massOfSpaceList, out centerDa, ref refineNumber, ref deviationToConverge, out refiniedWindowDa);

                    canWeUpdate = OptimizeUpdateCenterAndWindow.UpdateCenterAndWindow(ref clusters, centroidedPeaks, distributionParameters, clusterFinder, logAddition, hammerParameters, parameterOptimizationCutoff, bailOutCutoff, tetherLengthFromAverage, massToleranceDa, averageList, stdDevList, massOfSpaceList, out currrentCenter, ref refineNumber, ref deviationToConverge, out refiniedWindowDa, currentStandardDeviation);

                    if (centerDa > 0)
                    {
                        distributionParameters.CenterDa = centerDa;
                        distributionParameters.WindowsSizeDa = refiniedWindowDa;

                        distributionParameters.CenterInitialDa = distributionParameters.CenterDa;
                        distributionParameters.WindowsSizeInitialDa = distributionParameters.WindowsSizeDa;
                    }

                    if (canWeUpdate)//when we return false, we leave the while
                    {
                        break;
                    }
                }
            }
            else if (iterations == 1)
            {
                double currentStandardDeviation;
                double currentCenter;
                clusters = clusterFinder.RefineClusters(clusters, centroidedPeaks, hammerParameters,writeFolder, writeDiagnostics, out currentStandardDeviation, out currentCenter, refineNumber, out areaUnderCurve);

                //TODO
                if (currentCenter > 0 && (currentCenter + (currentStandardDeviation * hammerParameters.MassTolleranceSigmaMultiplier) <= hammerParameters.SeedClusterSpacingCenter + hammerParameters.SeedMassToleranceDa) 
                    && (currentCenter - (currentStandardDeviation * hammerParameters.MassTolleranceSigmaMultiplier) >= hammerParameters.SeedClusterSpacingCenter - hammerParameters.SeedMassToleranceDa))
                //if (currentCenter > 0)
                {
                    distributionParameters.CenterDa = currentCenter;
                    distributionParameters.WindowsSizeDa = currentStandardDeviation * hammerParameters.MassTolleranceSigmaMultiplier;

                    //global parameters
                    hammerParameters.SeedClusterSpacingCenter = currentCenter;
                    hammerParameters.SeedMassToleranceDa = currentStandardDeviation * hammerParameters.MassTolleranceSigmaMultiplier;
                }
                else
                {
                    hammerParameters.SeedClusterSpacingCenter = distributionParameters.CenterInitialDa;
                    hammerParameters.SeedMassToleranceDa = distributionParameters.WindowsSizeInitialDa;
                }
            }

            #region Switch statement (off)
            //switch (iterations)
            //{
            //    case 0:
            //        {
            //            //do nothing.  clusters are good already
            //        }
            //        break;
            //    case 1:
            //        {
            //            double currentStandardDeviation;
            //            double currrentCenter;
            //            clusters = clusterFinder.RefineClusters(clusters, centroidedPeaks, hammerParameters, out currentStandardDeviation, out currrentCenter, refineNumber, out areaUnderCurve);

            //            if (currrentCenter > 0)
            //            {
            //                distributionParameters.CenterDa = currrentCenter;
            //                distributionParameters.WindowsSizeDa = currentStandardDeviation;

            //                //global parameters
            //                hammerParameters.SeedClusterSpacingCenter = currrentCenter;
            //                hammerParameters.SeedMassToleranceDa = currentStandardDeviation;
            //            }

            //            #region old
            //            //#region Clusters & parameterization
            //            ////Finds clusters and adjusts MassToleranceDa and MassOfSpace according to stdev and average found
            //            //double currentStandardDeviation;
            //            //clusters = clusterFinder.RefineClusters(clusters, centroidedPeaks, hammerParameters, out currentStandardDeviation, refineNumber, out areaUnderCurve);

            //            //hammerParameters.SeedMassToleranceDa = clusterFinder.MassToleranceDa * hammerParameters.MassTolleranceSigmaMultiplier;
            //            //hammerParameters.SeedClusterSpacingCenter = clusterFinder.MassOfSpace;

            //            //#endregion

            //            //#region Test optimization 2 (on)
            //            //stdDevList.Add(currentStandardDeviation);
            //            //averageList.Add(hammerParameters.SeedClusterSpacingCenter);

            //            //#endregion

            //            //#region Cases
            //            ////Ends optimization if too few clusters are being produced
            //            //if (clusters.Count < 3)
            //            //{
            //            //    clusterFinder.MassOfSpace = distributionParameters.CenterInitialDa;
            //            //    clusterFinder.MassToleranceDa = distributionParameters.WindowsSizeInitialDa;
            //            //    clusters = clusterFinder.FindClusters(centroidedPeaks);
            //            //    logAddition.Add("Scan has less than 3 clusters");
            //            //    break;
            //            //}

            //            //centerDa = clusterFinder.MassOfSpace;

            //            ////Creates list with 3 most recent massOfSpace values
            //            //if (massOfSpaceList.Count < 3)
            //            //{
            //            //    massOfSpaceList.Enqueue(centerDa);
            //            //}
            //            //else
            //            //{
            //            //    massOfSpaceList.Dequeue();
            //            //    massOfSpaceList.Enqueue(centerDa);
            //            //}

            //            //if (massOfSpaceList.Count == 3)
            //            //{
            //            //    List<double> queueArray = massOfSpaceList.ToList();
            //            //    averageCenterPoint = (queueArray[0] + queueArray[1]) / 2;
            //            //    deviationToConverge = Math.Abs((queueArray[2] - averageCenterPoint)) / averageCenterPoint * 1000000;
            //            //}

            //            //#endregion

            //            //int iterationValue = Convert.ToInt32(Math.Round(deviationToConverge, 0));
            //            //int iterationCutoff = parameterOptimizationCutoff;
            //            //logAddition.Add("Scan has " + centerDa.ToString() + " delta with a window of +-" + massToleranceDa.ToString() + " iteration " + iterationValue + "/" + iterationCutoff);

            //            //#region Escape in case optimization fails
            //            ////This is if this will not optimize and is tethered by a mass difference and refine iteration number
            //            //double differenceFromDefault = Math.Abs(centerDa - distributionParameters.CenterInitialDa);
            //            //if (refineNumber > bailOutCutoff || differenceFromDefault > tetherLengthFromAverage)
            //            //{
            //            //    clusterFinder.MassOfSpace = distributionParameters.CenterInitialDa;
            //            //    clusterFinder.MassToleranceDa = distributionParameters.WindowsSizeInitialDa;
            //            //    clusters = clusterFinder.FindClusters(centroidedPeaks);
            //            //    logAddition.Add("Scan has failed to converge");
            //            //    break;
            //            //}

            //            //#endregion

            //            ////Write to log and set up for stdev
            //            //foreach (ClusterCP<double> currentCluster in clusters)
            //            //{
            //            //    int peakIndex = 0; //peakIndex can also be cluster.Peaks.Count - 1 for last peak
            //            //    int charge = currentCluster.Charge;
            //            //    double difference = charge * (currentCluster.Peaks[peakIndex].Value2 - currentCluster.Peaks[peakIndex].Value1);
            //            //    logAddition.Add("Scan has " + difference.ToString() + " delta with a window of +-" + distributionParameters.CenterDa.ToString() + " iteration " + -1 + "/" + -1);
            //            //}

            //            //distributionParameters.WindowsSizeDa = currentStandardDeviation;

            //            //refineNumber++;
            //            #endregion
            //        }
            //        break;
            //}

            #endregion

            //testing           
            #region Test optimization 3 (on)

            List<double> stdDevDifferences = new List<double>();
            List<double> averageDifferences = new List<double>();

            for (int i = 0; i < stdDevList.Count - 1; i++)
            {
                stdDevDifferences.Add(stdDevList[i + 1] - stdDevList[i]);
            }

            for (int i = 0; i < averageList.Count - 1; i++)
            {
                averageDifferences.Add(averageList[i + 1] - averageList[i]);
            }

            //List<XYData> stdAvgL

            //for (int i = 0; i < stdDevList.Count; i++)
            //{ 

            //}

            //List<string> outputList = new List<string>();

            //foreach (XYData xyData in xyHistogram)
            //{
            //    outputList.Add(xyData.X + ", " + xyData.Y);
            //}

            //StringListToDisk writer = new StringListToDisk();
            //writer.toDiskStringList(title, outputList);

            #endregion
            

            //distributionParameters.CenterDa = centerDa;//this is where we report the new center
            //distributionParameters.WindowsSizeDa = refiniedWindowDa;//this is where we report the new center

            return clusters;
        }

        
    }
}
