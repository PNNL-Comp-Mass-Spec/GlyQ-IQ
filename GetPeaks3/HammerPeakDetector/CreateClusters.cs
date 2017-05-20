using System;
using System.Collections.Generic;
using System.Linq;
using GetPeaksDllLite.DataFIFO;
using HammerPeakDetector.Objects;
using HammerPeakDetector.Objects.GetPeaksDifferenceFinder;
using PNNLOmics.Data;
using MathNet.Numerics.Statistics;
using HammerPeakDetector.Utilities;
using HammerPeakDetector.Parameters;

namespace HammerPeakDetector
{
    public class CreateClusters
    {
        /// <summary>
        /// mass tollerance for clustering
        /// </summary>
        public double MassToleranceDa { get; set; }

        /// <summary>
        /// Mass of c12-C13 in Da
        /// </summary>
        public double MassOfSpace { get; set; }

        public int MinChargeState { get; set; }

        public int MaxChargeState { get; set; }

        public CreateClusters()
        {
            MassToleranceDa = 0.018;
            MassOfSpace = 1.00310;
            MinChargeState = 1;
            MaxChargeState = 5;
        }

        public CreateClusters(HammerThresholdParameters parameters)
        {
            MassToleranceDa = parameters.SeedMassToleranceDa;
            MassOfSpace = parameters.SeedClusterSpacingCenter;
            MinChargeState = parameters.MinChargeState;
            MaxChargeState = parameters.MaxChargeState;
        }
        
        public List<ClusterCP<double>> FindClusters(List<Peak> data)
        {
            //Finds acceptable differences between peaks based on MassOfSpace
            List<double> differences = new List<double>();
            List<double> differenceErrors = new List<double>();
            for (int i = MinChargeState; i <= MaxChargeState; i++)
            {
                differences.Add(MassOfSpace / i);
                differenceErrors.Add(MassToleranceDa / i);
            }

            DataConverter converter = new DataConverter();
            List<double> DATA = converter.PeaksToMass(data);
            
            //Assists in finding differences between peaks
            DifferenceFinder newFinder = new DifferenceFinder();

            //Creates list of objects, sorted by index, containing data regarding m/z, differences between peaks, and indexes
            List<DifferenceObject<double>> differenceObjectReturnList = newFinder.FindDifferencesDa(differences, ref DATA, differenceErrors);
            differenceObjectReturnList = differenceObjectReturnList.OrderBy(p => p.IndexData).ToList();

            #region Used to determine the number of peaks in each feature - optional data (off)
            //int currentNumberOfPeaks = 2;
            //int totalNumberOfPeakGroups = 0;
            //DictionaryReplacement<int> numberOfPeaksInFeatureDictionary = new DictionaryReplacement<int>();
            //DictionaryReplacement<double> percentOfFeaturesWithPeakNumber = new DictionaryReplacement<double>();
            
            #endregion
            
            //Creates a list of features; Each feature contains corresponding peaks based on m/z ratio
            List<ClusterCP<double>> featureList = new List<ClusterCP<double>>();
            ClusterCP<double> currentFeature = new ClusterCP<double>();

            #region Cluster loop
            //Creates clusters of peaks by spectra differences
            //Outer loop controls the spectra difference being analyzed; various spectra differences are determined by max and min charge
            foreach (double currentDifference in differences)
            {
                //All difference objects with currentDifference
                List<DifferenceObject<double>> chargeSortedDifferenceObjectReturnList = (from n in differenceObjectReturnList where n.Difference == currentDifference select n).ToList();
                int resultLength = chargeSortedDifferenceObjectReturnList.Count;

                //Places each difference object into a feature
                for (int i = 0; i < resultLength; i++)
                {
                    DifferenceObject<double> currDiffObject = chargeSortedDifferenceObjectReturnList[i];

                    if (currDiffObject.Used == false)
                    {
                        currentFeature = new ClusterCP<double>();
                        featureList.Add(currentFeature);
                        currentFeature.Peaks.Add(currDiffObject);
                        currDiffObject.Used = true;
                        //Finds other difference objects corresponding to the current difference object by index
                        for (int j = i + 1; j < resultLength; j++)
                        {
                            DifferenceObject<double> otherDiffObject = chargeSortedDifferenceObjectReturnList[j];
                            if (currDiffObject.IndexMatch == otherDiffObject.IndexData)
                            {
                                currentFeature.Peaks.Add(otherDiffObject);
                                otherDiffObject.Used = true;
                                currDiffObject = otherDiffObject;
                                #region Number of peaks in feature computation - optional data (off)
                                //currentNumberOfPeaks++;
                                #endregion
                            }
                        }
                        #region Increment numberOfPeaksInFeatureDictionary values - optional data (off)
                        //if (numberOfPeaksInFeatureDictionary.ContainsKey(currentNumberOfPeaks))
                        //{
                        //    numberOfPeaksInFeatureDictionary.UpdateValue(currentNumberOfPeaks, numberOfPeaksInFeatureDictionary.FetchValue(currentNumberOfPeaks) + 1);
                        //}
                        //else
                        //{
                        //    numberOfPeaksInFeatureDictionary.AddElement(currentNumberOfPeaks, 1);
                        //}
                        //totalNumberOfPeakGroups++;
                        //currentNumberOfPeaks = 2;

                        #endregion
                    }
                }
                //Resets the used property to false to allow a peak to be used in features of multiple charge states
                foreach (DifferenceObject<double> diffObject in chargeSortedDifferenceObjectReturnList)
                {
                    diffObject.Used = false;
                }
            }

            #endregion

            #region Calculate charge
            ChargeCalculator.CalculateChargeStateFromDifference(ref featureList);

            #endregion

            #region Number of peaks in feature computation - optional data (off)
            //foreach (Tuple<int, int> pair in numberOfPeaksInFeatureDictionary.Data)
            //{
            //    double percent = pair.Item2 / totalNumberOfPeakGroups * 100;
            //    percentOfFeaturesWithPeakNumber.AddElement(pair.Item1, percent);
            //}

            #endregion
            
            return featureList;
        }

        /// <summary>
        /// Find a good cluster spacing to use.  this is one iteration
        /// </summary>
        /// <param name="clustersIn"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<ClusterCP<double>> RefineClusters(List<ClusterCP<double>> clustersIn, List<Peak> data, HammerThresholdParameters hammerParameters, string writeFolder, bool writeDiagnostics, out double sigma, out double center, int counter, out double areaUnderCurve)
        {
            //step 1 calculate histogram so we can calculate new coefficients

            //Create a histogram of X differences between clustered peaks
            ClusterInterpretationTools interpretClusters = new ClusterInterpretationTools();
            List<double> differencesDaScale = interpretClusters.CreateDifferenceList(clustersIn);

            int numBuckets = differencesDaScale.Count / hammerParameters.MinimumSizeOfRegion / 2;
            if (numBuckets < 50)
            {
                numBuckets = 50;
            }



            double minDifference = hammerParameters.SeedClusterSpacingCenter - hammerParameters.SeedMassToleranceDa;
            double maxDifference = hammerParameters.SeedClusterSpacingCenter + hammerParameters.SeedMassToleranceDa;
            Histogram histogram = new Histogram(differencesDaScale, numBuckets, minDifference, maxDifference);

            //Convert histogram to XYData
            DataConverter converter = new DataConverter();
            List<XYData> xyHistogram = converter.HistogramToXYData(histogram);


            //step 2 calculate new cofficents from the histrogram

            List<XYData> fitHistogramToGaussian;
            List<XYData> clippedList;
            List<XYData> smoothedHistogramXYData;
            List<XYData> modeledData;
            //Identify a new mass tolerance and mass of space based on histogram
            //smooth histogram first
            double[] coefficients = HistogramProcessing.ProcessAndFitHistogramToGetNewParameters(xyHistogram, out fitHistogramToGaussian, out clippedList, out smoothedHistogramXYData, out areaUnderCurve, out modeledData);
            sigma = Math.Abs(coefficients[0]);
            double height = coefficients[1];
            center = coefficients[2];
            Console.WriteLine("sigma is: " + sigma);
            Console.WriteLine("height is: " + height);
            Console.WriteLine("center is: " + center);

            List<string> stats = new List<string>();
            stats.Add(center + "," + sigma);

            //Where I left off 7-23-13 (hoping to get rid of reparameterization of clusters)
            //TODO make sure center is not too far off
            if (center > 0 && (center + (sigma * hammerParameters.MassTolleranceSigmaMultiplier) <= hammerParameters.SeedClusterSpacingCenter + hammerParameters.SeedMassToleranceDa) && (center - (sigma * hammerParameters.MassTolleranceSigmaMultiplier) >= hammerParameters.SeedClusterSpacingCenter - hammerParameters.SeedMassToleranceDa))
            //if (center > 0)            
            {
                this.MassToleranceDa = sigma * hammerParameters.MassTolleranceSigmaMultiplier;
                this.MassOfSpace = center;
            }
            
            if(writeDiagnostics)
            {
                #region Write histograms to csv files

                //string title = @"D:\PNNL\Projects\Christina Polyukh\Histograms\HistogramL2" + hammerParameters.MassTolleranceSigmaMultiplier + "sigmaRunNumber - " + counter + "_Raw.csv";
                //string titleSmoothed = @"D:\PNNL\Projects\Christina Polyukh\Histograms\HistogramL2" + hammerParameters.MassTolleranceSigmaMultiplier + "sigmaRunNumber - " + counter + "_Smoothed.csv";
                //DataConverter convertData = new DataConverter();
                //List<string> outputList = convertData.XYDataToString(xyHistogram);
                //List<string> outputListSmoothed = convertData.XYDataToString(smoothedHistogramXYData);

                //StringListToDisk writer = new StringListToDisk();
                //writer.toDiskStringList(title, outputList);
                //writer.toDiskStringList(titleSmoothed, outputListSmoothed);

                IXYDataWriter writer = new DataXYDataWriter();
                StringListToDisk writerStrings = new StringListToDisk();
                writerStrings.toDiskStringList(writeFolder + @"Histograms\Histogram_L" + counter + "Stats.csv",stats);
                writer.WriteOmicsXYData(xyHistogram, writeFolder + @"Histograms\Histogram_L" + counter + "Raw.csv");
                writer.WriteOmicsXYData(smoothedHistogramXYData, writeFolder + @"Histograms\Histogram_L" + counter + "Smoothed.csv");
                writer.WriteOmicsXYData(modeledData, writeFolder + @"\Histograms\Histogram_L" + counter + "Fit.csv");
                writer.WriteOmicsXYData(clippedList, writeFolder + @"Histograms\Histogram_L" + counter + "SmoothedClipped.csv");

                #endregion
            }
            //step 3 apply new coeffients

            List<ClusterCP<double>> featureList = this.FindClusters(data);

            return featureList;
        }
    }
}
