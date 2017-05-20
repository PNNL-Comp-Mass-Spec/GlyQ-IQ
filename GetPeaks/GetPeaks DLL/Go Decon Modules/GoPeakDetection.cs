using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using GetPeaks_DLL.Objects.Enumerations;
using DeconTools.Backend.ProcessingTasks;
using HammerPeakDetector.Enumerations;
using HammerPeakDetector.Parameters;
using HammerPeakSupport;
using PNNLOmics.Data;
using Peak = DeconTools.Backend.Core.Peak;
using CompareContrastDLL;
using GetPeaks_DLL.Functions;
using PNNLOmics.Algorithms.PeakDetection;
using DeconTools.Backend.DTO;
using DeconToolsV2.Peaks;
using GetPeaks_DLL.TandemSupport;
using HammerPeakDetector.Objects;

namespace GetPeaks_DLL.Go_Decon_Modules
{
    public static class GoPeakDetection
    {
        /// <summary>
        /// This works on what ever XYdata is in the Run
        /// </summary>
        /// <param name="currentParameters"></param>
        /// <param name="runGo"></param>
        /// <param name="objectInsideThread"></param>
        /// <param name="logAddition"></param>
        public static void PeakProcessing(ParametersPeakDetection currentParameters, Run runGo, ResultsPeakDetection objectInsideThread, out List<string> logAddition)
        //public static void PeakProcessing(int scan, ParametersPeakDetection currentParameters, Run runGo, ResultsPeakDetection objectInsideThread, out List<string> logAddition)
        //public static void PeakProcessing(ParalellThreadDataTHRASH currentParameterObject, EngineThrashDeconvolutor currentEngine, int scan, ParametersTHRASH currentParameters, Run runGo, ResultsTHRASH objectInsideThread)
        {
            if(runGo.XYData.Xvalues.Length<3)
            {
                Console.WriteLine("Missing XYData");
                //Console.ReadKey();
            }

            #region peak processing (step 2)
            logAddition = new List<string>();

            switch (currentParameters.PeakDetectionMethod)
            {
                case PeakDetectors.DeconTools: //decon centroid peaks + decon thresholding at parameter values
                    {
                        
                        #region inside Decon

                        Console.WriteLine("These are " + runGo.XYData.Xvalues.Count() + " Raw Data Points for DECON detection");

                        runGo.ResultCollection.Run.PeakList = null;
                        objectInsideThread.RawPeaksInScan = runGo.XYData.Xvalues.Length;
                        //clear it out because this is a scan by scan peak list

                        double peakBackgroundratio = currentParameters.DeconToolsPeakDetection.MsPeakDetectorPeakToBackground;
                        double peakSn = currentParameters.DeconToolsPeakDetection.SignalToNoiseRatio;
                        DeconTools.Backend.Globals.PeakFitType peakFitType = currentParameters.DeconToolsPeakDetection.PeakFitType;
                        bool isDataThresholded = currentParameters.isDataThresholdedDecon;

                        DeconToolsPeakDetector msPeakDetector = new DeconToolsPeakDetector(peakBackgroundratio, peakSn, peakFitType, isDataThresholded);

                        //this needs to be pulled out front
                        //msPeakDetector.IsDataThresholded = true; //this does not give the best results I think 9-24-12.  False lowers bar so false postives are greater and monos are greater

                        msPeakDetector.Execute(runGo.ResultCollection);


                        foreach (DeconTools.Backend.Core.Peak dPeak in runGo.PeakList)
                        {
                            //ParalellLog.LogCentroidPeaks(currentEngine, scan, dPeak.XValue, dPeak.Height);
                        }

                        objectInsideThread.CentroidedPeaksInScan = -1;

                        Console.WriteLine("Centroiding combined with Thresholding via DeconTools");

                        foreach (DeconTools.Backend.Core.Peak dPeak in runGo.PeakList)
                        {
                            objectInsideThread.SignalPeaks.Add(new XYData(dPeak.XValue, dPeak.Height));
                        }

                        objectInsideThread.ThresholdedPeaksInScan = runGo.PeakList.Count;


                        bool calculateNoise = true;
                        if (calculateNoise)
                        {
                            //DeconToolsPeakDetector centroidOnly = new DeconToolsPeakDetector(0, 0, peakFitType, isDataThresholded);
                            ////centroidOnly.IsDataThresholded = true; //this does not give the best results I think 9-24-12
                            //centroidOnly.Execute(runGo.ResultCollection);

                            //List<PNNLOmics.Data.Peak> CentroidedOnlyPeaks = new List<PNNLOmics.Data.Peak>();
                            //foreach (DeconTools.Backend.Core.Peak dPeak in runGo.PeakList)
                            //{
                            //    PNNLOmics.Data.Peak newOmicsPeak = new PNNLOmics.Data.Peak();
                            //    newOmicsPeak.XValue = dPeak.XValue;
                            //    newOmicsPeak.Width = dPeak.Width;
                            //    newOmicsPeak.Height = dPeak.Height;
                            //    newOmicsPeak.LocalSignalToNoise = -1;
                            //    newOmicsPeak.Background = Convert.ToSingle(msPeakDetector.BackgroundIntensity);
                            //    CentroidedOnlyPeaks.Add(newOmicsPeak);
                            //}

                            PeakCentroidOptions centroidSwitch = PeakCentroidOptions.DeconPeakDetector; //this should be fixed so that the abundances are the same!

                            List<PNNLOmics.Data.Peak> CentroidedOnlyPeaks = CentroidPeaksDetector.CentroidPeaks(currentParameters, runGo, objectInsideThread, centroidSwitch);

                            objectInsideThread.CentroidedPeaksInScan = runGo.PeakList.Count;

                            Console.WriteLine("After Centroiding, ther are " + objectInsideThread.CentroidedPeaksInScan + " centroided peaks at " + @"%");

                            msPeakDetector.Execute(runGo.ResultCollection); //run it again to be sure

                            CompareContrastDLL.CompareInputLists newList = new CompareInputLists();

                            CompareController compareHere = new CompareController();
                            SetListsToCompare prepCompare = new SetListsToCompare();
                            IConvert letsConvert = new Converter();
                            double massTolleranceMatch = 0.01;

                            List<double> dataList = letsConvert.XYDataToMass(objectInsideThread.SignalPeaks);
                            List<double> libraryList = letsConvert.OmicsPeakToDouble(CentroidedOnlyPeaks);
                            //TODO fix the problems when there is 0 data
                            if (dataList.Count == 0 || libraryList.Count == 0)
                            {
                                Console.WriteLine("missing data because there are not any peaks");
                            }

                            if (dataList.Count > 0 && libraryList.Count > 0)
                            {
                                CompareInputLists inputListsGlycanOnly = prepCompare.SetThem(dataList, libraryList);
                                CompareResultsIndexes indexesFromCompareGlycanOnly = new CompareResultsIndexes();
                                CompareResultsValues valuesFromCompare = compareHere.compareFX(inputListsGlycanOnly, massTolleranceMatch, ref indexesFromCompareGlycanOnly);

                                foreach (int d in indexesFromCompareGlycanOnly.IndexListBandNotA)
                                {
                                    XYData peakOut = new XYData(CentroidedOnlyPeaks[d].XValue, CentroidedOnlyPeaks[d].Height);
                                    objectInsideThread.NoisePeaks.Add(peakOut);
                                }
                            }
                            Console.WriteLine("There are " + objectInsideThread.ThresholdedPeaksInScan + " signal peaks found");
                            Console.WriteLine("There are " + objectInsideThread.NoisePeaks.Count + " noise peaks found");
                            //Console.WriteLine("There are " + CentroidedOnlyPeaks.Count + " peaks in total found");
                        }

                        #endregion
                    }
                    break;
                case PeakDetectors.OrbitrapFilter: //decon or omics centroid peaks at CP thresholding levels
                    {
                        #region inside Christina

                        Console.WriteLine("These are " + runGo.XYData.Xvalues.Count() + " Raw Data Points for HAMMER detection");


                        List<PNNLOmics.Data.Peak> centroidedPeaks = new List<PNNLOmics.Data.Peak>();

                        if (currentParameters.shouldWeApplyCentroidToTandemHammer)
                        {
                            PeakCentroidOptions centroidSwitch = PeakCentroidOptions.DeconPeakDetector; //this should be fixed so that the abundances are the same!

                            centroidedPeaks = CentroidPeaksDetector.CentroidPeaks(currentParameters, runGo, objectInsideThread, centroidSwitch);
                        }
                        else//just copy all xydata to centroided peak list
                        {
                            for (int i = 0; i < runGo.XYData.Xvalues.Length; i++ )
                            {
                                PNNLOmics.Data.Peak newPeak = new PNNLOmics.Data.Peak();
                                newPeak.XValue = runGo.XYData.Xvalues[i];
                                newPeak.Height = runGo.XYData.Yvalues[i];
                                newPeak.Width = Convert.ToSingle(0.1);
                                centroidedPeaks.Add(newPeak);
                            }
                            
                            
                        }
                        double peaksSize = centroidedPeaks.Count;
                        double XYDataSize = runGo.XYData.Xvalues.Length;
                        double peakConsolidationPercentage = peaksSize / XYDataSize * 100;
                        Console.WriteLine("After Centroiding, ther are " + centroidedPeaks.Count() + " centroided peaks at " + peakConsolidationPercentage + @"%");

                        double orbitrapFilterSigmaMultiplier = currentParameters.HammerParameters.ThresholdSigmaMultiplier;
                        int minimumNumberofPointsInBin = currentParameters.HammerParameters.MinimumSizeOfRegion;

                        List<ClusterCP<double>> clusters = new List<ClusterCP<double>>();
                        int clusterCount = 0;

                        int clusterType = 1;
                        //1 = filtered clusters; 0 = nonfiltered clusters; this number must match the number from LastPeaks.cs
                        //FILTERONOFF
                        //Allows user to choose whether clusters are filtered to have no repeating indexes

                        switch (clusterType)
                        {
                            #region inside (cluster or cluster + filter repeated peaks

                            case 0:
                                {
                                    //no filter repeated peaks
                                    FindPeakClusters clusterFinder = new FindPeakClusters();
                                    clusterFinder.MassToleranceDa = currentParameters.HammerParameters.SeedClusterSpacingCenter;
                                    clusterFinder.MassToleranceDa = currentParameters.HammerParameters.SeedMassToleranceDa;

                                    clusters = clusterFinder.FindClusters(centroidedPeaks);
                                }
                                break;
                            case 1:
                                {
                                    //switch (currentParameters.HammerParameters.OptimizeOrDefaultChoise)
                                    //{
                                    //    //case HammerThresholdParameters.OptimizeOrDefaultMassSpacing.Default:
                                    //    case OptimizeOrDefaultMassSpacing.Default:
                                    //        {
                                                FindPeakClusters clusterFinder = new FindPeakClusters();
                                                clusterFinder.MassOfSpace = currentParameters.HammerParameters.SeedClusterSpacingCenter;
                                                clusterFinder.MassToleranceDa = currentParameters.HammerParameters.SeedMassToleranceDa;
                                                List<ClusterCP<double>> clustersOfPeaks = clusterFinder.FindClusters(centroidedPeaks);

                                                Console.WriteLine("There are " + clustersOfPeaks.Count + " clusters detected before filtering");

                                                if (clustersOfPeaks.Count > 0)
                                                {
                                                    RepeatedClusterFilter peakFilter = new RepeatedClusterFilter();
                                                    clusters = peakFilter.FilterRepeatedPeaks(centroidedPeaks, clustersOfPeaks);
                                                }
                                    //          }
                                    //        break;
                                        //case HammerThresholdParameters.OptimizeOrDefaultMassSpacing.Optimize:
                                    //    case OptimizeOrDefaultMassSpacing.Optimize:
                                    //        {
                                    //            FindPeakClusters clusterFinder = new FindPeakClusters();
                                    //            clusterFinder.MassOfSpace = currentParameters.HammerParameters.SeedClusterSpacingCenter;
                                    //            clusterFinder.MassToleranceDa = currentParameters.HammerParameters.SeedMassToleranceDa;
                                    //            List<ClusterCP<double>> clustersOfPeaks = clusterFinder.FindClusters(centroidedPeaks); //this searches at default values

                                    //            double massOfSpaceDefault = clusterFinder.MassOfSpace;
                                    //            double massOfErrorDaDefault = clusterFinder.MassToleranceDa;

                                    //            double massOfSpace = clusterFinder.MassOfSpace;
                                    //            double massToleranceDa = clusterFinder.MassToleranceDa;
                                    //            Console.WriteLine("Optimizing... The cluster space is " + massOfSpace + " with a " + massToleranceDa + " tollerence");

                                    //            if (clustersOfPeaks.Count > 0)
                                    //            {
                                    //                double newMassSpacing;
                                    //                double newStandardDeviation;

                                    //                clustersOfPeaks = OptimizeDifferences(clustersOfPeaks, massOfSpaceDefault, centroidedPeaks, massOfErrorDaDefault, clusterFinder, massOfSpace, out newMassSpacing, out newStandardDeviation, out logAddition);
                                    //                //clustersOfPeaks = OptimizeDifferences(clustersOfPeaks, scan, massOfSpaceDefault, centroidedPeaks, massOfErrorDaDefault, clusterFinder, massOfSpace, out newMassSpacing, out newStandardDeviation, out logAddition);
                                                    
                                    //                //string details = "Scan " + scan.ToString() + ",MassDifference " + newMassSpacing.ToString();
                                    //                string details = "Scan ,MassDifference " + newMassSpacing.ToString();

                                    //                logAddition.Add(details);
                                    //                //currentParameterObject.Engine.ErrorLog.Add(details);
                                                    
                                    //                Console.WriteLine("There are " + clustersOfPeaks.Count + " clusters detected before filtering");

                                    //                RepeatedClusterFilter peakFilter = new RepeatedClusterFilter();
                                    //                clusters = peakFilter.FilterRepeatedPeaks(centroidedPeaks, clustersOfPeaks);
                                    //            }
                                    //        }
                                    //        break;
                                    //}
                                }
                                break;

                            #endregion
                        }

                        clusterCount = clusters.Count;
                        Console.WriteLine("There are " + clusterCount + " clusters detected overall");

                        NumberOfPointsPerRegionFinder pointsPerRegionFinder = new NumberOfPointsPerRegionFinder();
                        currentParameters.HammerParameters.NumberOfPointsPerNoiseRegion = pointsPerRegionFinder.FindNumberOfPointsPerRegionList(clusterCount, currentParameters.HammerParameters.MinimumSizeOfRegion);

                        if (clusters.Count > 0)
                        {
                            TestRawData rawDataTest = new TestRawData();

                            List<ClusterCP<double>> clustersAutomated = new List<ClusterCP<double>>();
                            List<PNNLOmics.Data.Peak> signalPeaksAutomated = new List<PNNLOmics.Data.Peak>();
                            List<PNNLOmics.Data.Peak> noisePeaksAutomated = new List<PNNLOmics.Data.Peak>();

                            switch (currentParameters.HammerParameters.FilteringMethod)
                            {
                                //case HammerThresholdParameters.OrbitrapFilteringMethod.Threshold:
                                //case OrbitrapFilteringMethod.Threshold:
                                case HammerFilteringMethod.Threshold:
                                    {
                                        //rawDataTest.ParseSignalAndNoise(centroidedPeaks, currentParameters.HammerParameters, clusters, out clustersAutomated, out signalPeaksAutomated, out noisePeaksAutomated);

                                        HammerPeakDetector.Parameters.HammerThresholdParameters hammerParameters2 = new HammerPeakDetector.Parameters.HammerThresholdParameters();
                                        rawDataTest.ParseSignalAndNoise(centroidedPeaks, hammerParameters2, clusters, out clustersAutomated, out signalPeaksAutomated, out noisePeaksAutomated);
                                    }
                                    break;
                                //case HammerThresholdParameters.OrbitrapFilteringMethod.Cluster:
                                //case OrbitrapFilteringMethod.Cluster:
                                case HammerFilteringMethod.Cluster:
                                    {
                                        rawDataTest.ParseClusterToSignalWithoutThresholding(centroidedPeaks, clusters, out signalPeaksAutomated, out noisePeaksAutomated);
                                    }
                                    break;
                            }

                            List<double> viewOfSignal = new List<double>();
                            foreach (PNNLOmics.Data.Peak mass in signalPeaksAutomated)
                            {
                                viewOfSignal.Add(mass.XValue);
                            }
                            Console.WriteLine("There are " + viewOfSignal.Count + " signal peaks found");

                            //assign to run
                            runGo.ResultCollection.MSPeakResultList = new List<MSPeakResult>();
                            runGo.ResultCollection.Run.PeakList = new List<DeconTools.Backend.Core.Peak>();
                            int size = signalPeaksAutomated.Count;
                            runGo.ResultCollection.Run.DeconToolsPeakList = new clsPeak[size];
                            int enginePeakCounter = 0;

                            List<PNNLOmics.Data.Peak> sortedSignalPeaksAutomated = new List<PNNLOmics.Data.Peak>();
                            sortedSignalPeaksAutomated = signalPeaksAutomated.OrderBy(p => p.XValue).ToList();
                            //this may not be necessary

                            foreach (PNNLOmics.Data.Peak peak in sortedSignalPeaksAutomated)
                            {
                                objectInsideThread.SignalPeaks.Add(new XYData(peak.XValue, peak.Height));
                                ;
                            }

                            foreach (PNNLOmics.Data.Peak peak in noisePeaksAutomated)
                            {
                                objectInsideThread.NoisePeaks.Add(new XYData(peak.XValue, peak.Height));
                            }

                            Console.WriteLine("There are " + objectInsideThread.NoisePeaks.Count + " noise peaks found");

                            objectInsideThread.ResultsFromPeakDetectorClusters = clusters;

                            objectInsideThread.ThresholdedPeaksInScan = sortedSignalPeaksAutomated.Count;

                            foreach (PNNLOmics.Data.Peak signalPeak in sortedSignalPeaksAutomated)
                            {
                                int SN = 20;
                                //float FWHM = Convert.ToSingle(0.022); //THIS is really important!!!!!! 0.022
                                float fwhm = signalPeak.Width;
                                double xValue = signalPeak.XValue;
                                double yValue = signalPeak.Height;

                                DeconTools.Backend.Core.MSPeak deconPeak = new DeconTools.Backend.Core.MSPeak();
                                deconPeak.XValue = xValue;
                                deconPeak.Height = Convert.ToSingle(yValue);
                                deconPeak.Width = fwhm;
                                deconPeak.DataIndex = 100;
                                deconPeak.SignalToNoise = SN;

                                MSPeakResult dtoPeak = new MSPeakResult();
                                //dtoPeak.Scan_num = scan;
                                dtoPeak.MSPeak = deconPeak;
                                dtoPeak.XValue = xValue;
                                dtoPeak.Width = fwhm;
                                dtoPeak.Height = Convert.ToSingle(yValue);

                                DeconToolsV2.Peaks.clsPeak engineV2Peak = new DeconToolsV2.Peaks.clsPeak();
                                engineV2Peak.mdbl_FWHM = fwhm;
                                engineV2Peak.mdbl_SN = SN;
                                engineV2Peak.mdbl_mz = xValue;
                                engineV2Peak.mdbl_intensity = yValue;
                                engineV2Peak.mint_data_index = 100;
                                engineV2Peak.mint_peak_index = 100;

                                runGo.ResultCollection.Run.PeakList.Add(deconPeak);

                                //runGo.PeakList.Add(deconPeak);
                                //ParalellLog.LogCentroidPeaks(currentEngine, scan, engineV2Peak.mdbl_mz, engineV2Peak.mdbl_intensity);

                                runGo.ResultCollection.Run.DeconToolsPeakList[enginePeakCounter] = engineV2Peak;
                                enginePeakCounter++;
                            }
                        }
                        else //set peaks to zero if there are no clusters in the data
                        {
                            runGo.ResultCollection.Run.DeconToolsPeakList = new clsPeak[0];
                            runGo.ResultCollection.Run.PeakList = new List<DeconTools.Backend.Core.Peak>();
                        }




                        #endregion


                    }
                    break;
            }

            #endregion
        }

       
        //private static List<ClusterCP<double>> OptimizeDifferences(List<ClusterCP<double>> clustersOfPeaks, int scan, EngineThrashDeconvolutor currentEngine, double massOfSpaceDefault, List<Peak> centroidedPeaks, double massOfErrorDaDefault, FindPeakClusters clusterFinder, double massOfSpace, out double newMassOfSpace, out double newStandardDeviation, out List<string> logAddition)
        public static List<ClusterCP<double>> OptimizeDifferences(List<ClusterCP<double>> clustersOfPeaks, int scan, double massOfSpaceDefault, List<PNNLOmics.Data.Peak> centroidedPeaks, double massOfErrorDaDefault, FindPeakClusters clusterFinder, double massOfSpace, out double newMassOfSpace, out double newStandardDeviation, out List<string> logAddition)
        {
            logAddition = new List<string>();

            newMassOfSpace = 0;
            newStandardDeviation = 0;
            double massToleranceDa = clusterFinder.MassToleranceDa;
            double averageCenterPoint = massOfSpace;
            double deviationToConverge = 500000;
            int refineNumber = 0;

            const int bailOutCuttoff = 10;
            const double tetherLenghtFromAverage = 0.03;

            Queue<double> MassofSpaceList = new Queue<double>();
            //for (int refineNumber = 0; refineNumber < 20; refineNumber++)
            int parameterOptimizationCuttoff = 100;
            while (deviationToConverge > parameterOptimizationCuttoff) //100 will change at the 0.0001 level.  such as the difference between 1.0031 and 1.0032
            {
                //this has a findClustersinIt using this standard deviation.  mass of space comes out as a paramter in clusterFinder
                double currentStandardDeviation;
                clustersOfPeaks = clusterFinder.RefineClusters(clustersOfPeaks, centroidedPeaks, out currentStandardDeviation);

                //this is when there are not enough peaks to optimize.  run as normal
                if (clustersOfPeaks.Count < 3)
                {
                    clusterFinder.MassOfSpace = massOfSpaceDefault;
                    clusterFinder.MassToleranceDa = massOfErrorDaDefault;
                    clustersOfPeaks = clusterFinder.FindClusters(centroidedPeaks);
                    //ParalellLog.LogAddText(currentEngine, scan, "less than 3 clusters");
                    logAddition.Add("Scan " + scan.ToString() + " has less than 3 clusters");
                    break;
                }

                massOfSpace = clusterFinder.MassOfSpace;
                //massToleranceDa = clusterFinder.MassToleranceDa;

                Console.WriteLine("Optimizing... The Refined cluster space is " + massOfSpace + " with a " + massToleranceDa + " tollerence finding " + clustersOfPeaks.Count + " clusters");

                if (MassofSpaceList.Count < 3)
                {
                    MassofSpaceList.Enqueue(massOfSpace);
                }
                else
                {
                    MassofSpaceList.Dequeue();
                    MassofSpaceList.Enqueue(massOfSpace);
                }

                if (MassofSpaceList.Count == 3)
                {
                    List<double> queueArray = MassofSpaceList.ToList();
                    averageCenterPoint = (queueArray[0] + queueArray[1]) / 2;
                    deviationToConverge = Math.Abs((queueArray[2] - averageCenterPoint)) / averageCenterPoint * 1000000;
                    Console.WriteLine("Optimizing... the new point is " + deviationToConverge + "ppm above the average");//this ppm here is like a percent change but much finer
                }

                //ParalellLog.LogAddClusterMassDifference(currentEngine, scan, massOfSpace, massToleranceDa, parameterOptimizationCuttoff, Convert.ToInt32(Math.Round(deviationToConverge, 0)));
                int iterationValue = Convert.ToInt32(Math.Round(deviationToConverge, 0));
                int iterationCuttoff = parameterOptimizationCuttoff;
                logAddition.Add("Scan " + scan.ToString() + " has " + massOfSpace.ToString() + " delta with a window of +-" + massToleranceDa.ToString() + " iteration " + iterationValue + "/" + iterationCuttoff);

                //this is if this will not optimize and is tethered by a mass difference and refine iteration number
                double differencefromDefault = Math.Abs(massOfSpace - massOfSpaceDefault);
                if (refineNumber > bailOutCuttoff || differencefromDefault > tetherLenghtFromAverage)
                {
                    //this happens whn there is very little signal and is an escape
                    clusterFinder.MassOfSpace = massOfSpaceDefault;
                    clusterFinder.MassToleranceDa = massOfErrorDaDefault;
                    clustersOfPeaks = clusterFinder.FindClusters(centroidedPeaks);
                    //ParalellLog.LogAddText(currentEngine, scan, "failed to converge");
                    logAddition.Add("Scan " + scan.ToString() + " has failed to converge");
                    break;
                }

                //write to log and set up for stdev
                List<double> differences = new List<double>();
                foreach (ClusterCP<double> cluster in clustersOfPeaks)
                {
                    //int lastPeak = cluster.Peaks.Count;
                    //int firstPeak = 0;
                    int peakIndex = 0;
                    int charge = cluster.Peaks[peakIndex].DifferenceIndex + 1;
                    double difference = (cluster.Peaks[peakIndex].Value2 - cluster.Peaks[peakIndex].Value1) * charge;
                    differences.Add(difference);
                    //ParalellLog.LogAddClusterMassDifference(currentEngine, scan, difference, newMassOfSpace, -1, -1);
                    logAddition.Add("Scan " + scan.ToString() + " has " + difference.ToString() + " delta with a window of +-" + newMassOfSpace.ToString() + " iteration " + -1 + "/" + -1);
                }

                //calculate stdev
                //StandardDeviation calculator = new StandardDeviation();
                //newStandardDeviation = calculator.FindStandardDeviation(differences);
                //massToleranceDa = newStandardDeviation*3;
                newStandardDeviation = currentStandardDeviation;

                refineNumber++;
            }

            newMassOfSpace = massOfSpace;



            return clustersOfPeaks;
        }

        public static List<ClusterCP<double>> OptimizeDifferences(List<ClusterCP<double>> clustersOfPeaks, double massOfSpaceDefault, List<PNNLOmics.Data.Peak> centroidedPeaks, double massOfErrorDaDefault, FindPeakClusters clusterFinder, double massOfSpace, out double newMassOfSpace, out double newStandardDeviation, out List<string> logAddition)
        {
            logAddition = new List<string>();

            newMassOfSpace = 0;
            newStandardDeviation = 0;
            double massToleranceDa = clusterFinder.MassToleranceDa;
            double averageCenterPoint = massOfSpace;
            double deviationToConverge = 500000;
            int refineNumber = 0;

            const int bailOutCuttoff = 10;
            const double tetherLenghtFromAverage = 0.03;

            Queue<double> MassofSpaceList = new Queue<double>();
            //for (int refineNumber = 0; refineNumber < 20; refineNumber++)
            int parameterOptimizationCuttoff = 100;
            while (deviationToConverge > parameterOptimizationCuttoff) //100 will change at the 0.0001 level.  such as the difference between 1.0031 and 1.0032
            {
                //this has a findClustersinIt using this standard deviation.  mass of space comes out as a paramter in clusterFinder
                double currentStandardDeviation;
                clustersOfPeaks = clusterFinder.RefineClusters(clustersOfPeaks, centroidedPeaks, out currentStandardDeviation);

                //this is when there are not enough peaks to optimize.  run as normal
                if (clustersOfPeaks.Count < 3)
                {
                    clusterFinder.MassOfSpace = massOfSpaceDefault;
                    clusterFinder.MassToleranceDa = massOfErrorDaDefault;
                    clustersOfPeaks = clusterFinder.FindClusters(centroidedPeaks);
                    //ParalellLog.LogAddText(currentEngine, scan, "less than 3 clusters");
                    logAddition.Add("Scan has less than 3 clusters");
                    break;
                }

                massOfSpace = clusterFinder.MassOfSpace;
                //massToleranceDa = clusterFinder.MassToleranceDa;

                Console.WriteLine("Optimizing... The Refined cluster space is " + massOfSpace + " with a " + massToleranceDa + " tollerence finding " + clustersOfPeaks.Count + " clusters");

                if (MassofSpaceList.Count < 3)
                {
                    MassofSpaceList.Enqueue(massOfSpace);
                }
                else
                {
                    MassofSpaceList.Dequeue();
                    MassofSpaceList.Enqueue(massOfSpace);
                }

                if (MassofSpaceList.Count == 3)
                {
                    List<double> queueArray = MassofSpaceList.ToList();
                    averageCenterPoint = (queueArray[0] + queueArray[1]) / 2;
                    deviationToConverge = Math.Abs((queueArray[2] - averageCenterPoint)) / averageCenterPoint * 1000000;
                    Console.WriteLine("Optimizing... the new point is " + deviationToConverge + "ppm above the average");//this ppm here is like a percent change but much finer
                }

                //ParalellLog.LogAddClusterMassDifference(currentEngine, scan, massOfSpace, massToleranceDa, parameterOptimizationCuttoff, Convert.ToInt32(Math.Round(deviationToConverge, 0)));
                int iterationValue = Convert.ToInt32(Math.Round(deviationToConverge, 0));
                int iterationCuttoff = parameterOptimizationCuttoff;
                logAddition.Add("Scan has " + massOfSpace.ToString() + " delta with a window of +-" + massToleranceDa.ToString() + " iteration " + iterationValue + "/" + iterationCuttoff);

                //this is if this will not optimize and is tethered by a mass difference and refine iteration number
                double differencefromDefault = Math.Abs(massOfSpace - massOfSpaceDefault);
                if (refineNumber > bailOutCuttoff || differencefromDefault > tetherLenghtFromAverage)
                {
                    //this happens whn there is very little signal and is an escape
                    clusterFinder.MassOfSpace = massOfSpaceDefault;
                    clusterFinder.MassToleranceDa = massOfErrorDaDefault;
                    clustersOfPeaks = clusterFinder.FindClusters(centroidedPeaks);
                    //ParalellLog.LogAddText(currentEngine, scan, "failed to converge");
                    logAddition.Add("Scan has failed to converge");
                    break;
                }

                //write to log and set up for stdev
                List<double> differences = new List<double>();
                foreach (ClusterCP<double> cluster in clustersOfPeaks)
                {
                    //int lastPeak = cluster.Peaks.Count;
                    //int firstPeak = 0;
                    int peakIndex = 0;
                    int charge = cluster.Peaks[peakIndex].DifferenceIndex + 1;
                    double difference = (cluster.Peaks[peakIndex].Value2 - cluster.Peaks[peakIndex].Value1) * charge;
                    differences.Add(difference);
                    //ParalellLog.LogAddClusterMassDifference(currentEngine, scan, difference, newMassOfSpace, -1, -1);
                    logAddition.Add("Scan has " + difference.ToString() + " delta with a window of +-" + newMassOfSpace.ToString() + " iteration " + -1 + "/" + -1);
                }

                //calculate stdev
                //StandardDeviation calculator = new StandardDeviation();
                //newStandardDeviation = calculator.FindStandardDeviation(differences);
                //massToleranceDa = newStandardDeviation*3;
                newStandardDeviation = currentStandardDeviation;

                refineNumber++;
            }

            newMassOfSpace = massOfSpace;



            return clustersOfPeaks;
        }

    }
}
