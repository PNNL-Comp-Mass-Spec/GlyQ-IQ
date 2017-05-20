using System;
using System.Collections.Generic;
using GetPeaksDllLite.DataFIFO;
using GetPeaksDllLite.Functions;
using HammerPeakDetector.Objects.GetPeaksDifferenceFinder;
using HammerPeakDetector.Parameters;
using PNNLOmics.Data;
using HammerPeakDetector.Objects;
using MathNet.Numerics.Statistics;
using HammerPeakDetector.Utilities;
using HammerPeakDetector.Enumerations;
using PNNLOmics.Data.Peaks;

namespace HammerPeakDetector
{
    public static class Hammer
    {
        //convert centroidedPeaks to ProcessedPeak
        public static List<ProcessedPeak> Detect(List<Peak> centroidedPeaks, HammerThresholdParameters parameters, string writeFolder, bool writeDiagnostics, out HammerThresholdResults hammerResults)
        {
            #region Stopwatch - start
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            #endregion

            //double pValue = GetPeaks_DLL.Functions.ConvertSigmaAndPValue.SigmaToPvalue(.248);
            //double pValue2 = GetPeaks_DLL.Functions.ConvertSigmaAndPValue.SigmaToPvalue(.561);


            #region 1) Prep step; create variables, convert centroided peaks to processed peaks, and find clusters using initial parameters

            DataConverter converter = new DataConverter();
            ClusterInterpretationTools interpretClusters = new ClusterInterpretationTools();

            List<ProcessedPeak> allPeaks = converter.PeaksToProcessedPeaks(centroidedPeaks);
            List<ProcessedPeak> results = new List<ProcessedPeak>();

            hammerResults = new HammerThresholdResults(parameters);

            //Finds clusters of peaks
            CreateClusters clusterFinder = new CreateClusters(hammerResults.NewParameters);
            List<ClusterCP<double>> clusters = clusterFinder.FindClusters(centroidedPeaks);
            
            #endregion

            #region 2) Optimize parameters and recluster

            List<string> logAddition;
            double areaUnderCurve;

            //Optimizes SeedClusterSpacing and SeedMassTolerance parameters
            if (clusters.Count > 0 && hammerResults.NewParameters.Iterations != 0)
            {
                double newMassSpacing;

                Optimize optimizer = new Optimize();
                clusters = optimizer.OptimizeDifferences(clusters, centroidedPeaks, writeFolder, writeDiagnostics, out logAddition, hammerResults.NewParameters, out areaUnderCurve);
                newMassSpacing = hammerResults.NewParameters.SeedClusterSpacingCenter;
                logAddition.Add("Scan, MassDifference " + newMassSpacing);
            }

            #endregion

            #region 3) Filter clusters (write final histogram to file)

            //Filters clusters that contain repeating peaks; for instance, the same peaks may be found in a +1 and +2 cluster; filters repetition
            ClusterFilter hammerClusterFilter = new ClusterFilter();
            clusters = hammerClusterFilter.FilterRepeatedPeaks(centroidedPeaks, clusters);

            if (writeDiagnostics == true)
            {

                #region Optional region - Currently only used to write histogram to file--not using reparameterization

                #region Statistics (Create histogram ==> Gaussian)

                List<double> differenceList = interpretClusters.CreateDifferenceList(clusters);
                int numBuckets = differenceList.Count/hammerResults.NewParameters.MinimumSizeOfRegion/2;
                if (numBuckets < 50)
                {
                    numBuckets = 50;
                }

                //Create histogram
                double minDifference = hammerResults.NewParameters.SeedClusterSpacingCenter -
                                       hammerResults.NewParameters.SeedMassToleranceDa;
                double maxDifference = hammerResults.NewParameters.SeedClusterSpacingCenter +
                                       hammerResults.NewParameters.SeedMassToleranceDa;
                Histogram histogram = new Histogram(differenceList, numBuckets, minDifference, maxDifference);

                List<XYData> xyPeaks = converter.PeakToXYData(centroidedPeaks);
                List<XYData> xyHistogram = converter.HistogramToXYData(histogram);

                List<XYData> fitHistogramToGaussian;
                List<XYData> clippedList;
                List<XYData> smoothedHistogramXYData;
                List<XYData> modeledData;

                //Find coefficients based on histogram to Gaussian fitting
                var coefficients = HistogramProcessing.ProcessAndFitHistogramToGetNewParameters(xyHistogram, out fitHistogramToGaussian, out clippedList, out smoothedHistogramXYData, out areaUnderCurve, out modeledData);
                double sigma = coefficients[0];
                double height = coefficients[1];
                double center = coefficients[2];
                Console.WriteLine("sigma is: " + sigma);
                Console.WriteLine("height is: " + height);
                Console.WriteLine("center is: " + center);

                List<string> stats = new List<string>();
                stats.Add(center + "," + sigma);

                //Provides parameters for one optimization of the current clusters (TODO I don't think this is needed; if we get rid of it,
                //that's two less parameters to worry about)
                hammerResults.NewParameters.finalCenter = center;
                hammerResults.NewParameters.finalSigma = sigma;

                //TODO
                //is center too far from reference
                //is sigma positive or perhaps less than ref window
                //perhaps return center and sigma

                //todo add result class and return it
                //is Successull
                //cwnter
                //sigma

                ////Maybe this should not change
                //if (center > 0)
                //{
                //    hammerResults.NewParameters.SeedMassToleranceDa = sigma * hammerResults.NewParameters.MassTolleranceSigmaMultiplier;
                //    hammerResults.NewParameters.SeedClusterSpacingCenter = center;
                //}


                #endregion

                #region Write to file

                //List<String> outputList = converter.XYDataToString(xyHistogram);
                //List<String> outputListFit = converter.XYDataToString(clippedList);

                //StringListToDisk writer = new StringListToDisk();
                //writer.toDiskStringList("C:\\Users\\poly317\\Documents\\Text Files - Output\\Output.csv", outputList);
                //writer.toDiskStringList(@"D:\PNNL\Projects\Christina Polyukh\Histograms\Output.csv", outputList);
                //writer.toDiskStringList(@"D:\PNNL\Projects\Christina Polyukh\Histograms\OutputFit.csv", outputListFit); 

                int counter = 1;

                bool write = false;
                if (write)
                {
                    IXYDataWriter writer = new DataXYDataWriter();
                    StringListToDisk writerStrings = new StringListToDisk();
                    writerStrings.toDiskStringList(writeFolder + @"\" + @"Histograms\Histogram_L" + counter + "Stats.csv", stats);
                    writer.WriteOmicsXYData(xyHistogram, writeFolder + @"\" + @"Histograms\Histogram_L" + counter + "Raw.csv");
                    writer.WriteOmicsXYData(smoothedHistogramXYData, writeFolder + @"\" + @"Histograms\Histogram_L" + counter + "Smoothed.csv");
                    writer.WriteOmicsXYData(modeledData, writeFolder + @"\" + @"Histograms\Histogram_L" + counter + "Fit.csv");
                    writer.WriteOmicsXYData(clippedList, writeFolder + @"\" + @"Histograms\Histogram_L" + counter + "SmoothedClipped.csv");
                }
                #endregion

                #endregion

            }

            #endregion

            #region 4) Add p-values
            
            //Set p values
            PValue findPVal = new PValue();
            findPVal.SetPValue(hammerResults.NewParameters, clusters, allPeaks);

            #endregion

            #region 5) Create list of signal peaks using either thresholder or clusters

            //Filters according to preferred method for identifying signal
            switch (hammerResults.NewParameters.FilteringMethod)
            {
                //Creates a moving threshold and filters for signal based on background parameter
                case HammerFilteringMethod.Threshold:
                    {
                        //Finds background noise threshold for each peak
                        NoiseThresholder noiseThresholder = new NoiseThresholder();
                        noiseThresholder.MovingThreshold(allPeaks, clusters, hammerResults.NewParameters);
                        foreach (ProcessedPeak peak in allPeaks)
                        {
                            if (peak.Height > peak.Background)
                            {
                                results.Add(peak);
                            }
                        }
                    }
                    break;
                //All clustered peaks are considered signal
                case HammerFilteringMethod.Cluster:
                    {
                        foreach (ClusterCP<double> cluster in clusters)
                        {
                            results.Add(allPeaks[cluster.Peaks[0].IndexData]);
                            foreach (DifferenceObject<double> diffObject in cluster.Peaks)
                            {
                                results.Add(allPeaks[diffObject.IndexMatch]);
                            }
                        }
                    }
                    break;
                //Peaks must be clustered with a higher p-value than the cutoff to be considered signal
                case HammerFilteringMethod.SmartCluster:
                    {
                        //double cutoff = MathNet.Numerics.SpecialFunctions.Erf(hammerResults.NewParameters.MassTolleranceSigmaMultiplier / Math.Sqrt(2));
                        double cutoff = ConvertSigmaAndPValue.SigmaToPvalue(hammerResults.NewParameters.MassTolleranceSigmaMultiplier);
                        //double cutoff = hammerResults.NewParameters.Cutoff;                        
                        //double cutoff = 0.3;
                        
                        foreach (ProcessedPeak peak in allPeaks)
                        {
                            if (peak.Pvalue < cutoff && peak.Pvalue > -1)
                            //if (peak.Pvalue > cutoff)
                            //if (peak.Pvalue < 1 - cutoff && peak.Pvalue > -1)
                            {
                                results.Add(peak);
                            }
                        }
                    }
                    break;
            }

            #endregion
            
            #region Stopwatch - stop
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds.");

            #endregion

            //Return ProcessedPeaks identified as signal with information about the peaks (P-value, Background, etc.)
            return results;
        }
    }
}
