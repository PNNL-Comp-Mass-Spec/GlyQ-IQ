using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.Objects.DifferenceFinderObjects;
using GetPeaks_DLL.Functions;
using PNNLOmics.Data.Constants;
using PNNLOmics.Data;
using OrbitrapPeakDetection.Objects;

namespace OrbitrapPeakDetection
{
    public class FindPeakClusters
    {
        /// <summary>
        /// this is part of the histograming which will crash if it is too low
        /// </summary>
        //public int MaxNumberOfPeaksInCluster { get; set; }

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

        public FindPeakClusters()
        {
            //MaxNumberOfPeaksInCluster = 40;
            MassToleranceDa = 0.018; //Da
            //MassOfSpace = 1.00235;//THRASH
            MassOfSpace = 1.00310;
            MinChargeState = 1;
            MaxChargeState = 5;
        }

        public List<ClusterCP<double>> FindClusters(List<Peak> data)
        {
            #region Stopwatch
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            #endregion

            //Finds acceptable differences between peaks based on MassOfSpace
            List<double> differences = new List<double>();
            List<double> differenceErrors = new List<double>();
            for (int i = MinChargeState; i <= MaxChargeState; i++)
            {
                differences.Add(MassOfSpace / i);
                differenceErrors.Add(MassToleranceDa / i);
            }

            //Creates list of m/z
            List<double> DATA = new List<double>();
            foreach (Peak p in data)
            {
                DATA.Add(p.XValue);
            }

            //Assists in finding differences between peaks
            DifferenceFinder newFinder = new DifferenceFinder();

            //Creates list of objects, sorted by index, containing data regarding m/z, differences between peaks, and indexes
            List<DifferenceObject<double>> differenceObjectReturnList = newFinder.FindDifferencesDa(differences, ref DATA, differenceErrors);
            differenceObjectReturnList = differenceObjectReturnList.OrderBy(p => p.IndexData).ToList();

            #region Used to determine the number of peaks in each feature - optional (on)
            int currentNumberOfPeaks = 2;
            int totalNumberOfPeakGroups = 0;
            DictionaryReplacement<int> numberOfPeaksInFeatureDictionary = new DictionaryReplacement<int>();
            DictionaryReplacement<double> percentOfFeaturesWithPeakNumber = new DictionaryReplacement<double>();

            #endregion

            //Creates a list of features; Each feature contains corresponding peaks based on m/z ratio
            List<ClusterCP<double>> featureList = new List<ClusterCP<double>>();
            ClusterCP<double> currentFeature = new ClusterCP<double>();

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
                                #region Number of peaks in feature computation
                                currentNumberOfPeaks++;
                                #endregion
                            }
                        }
                        #region Increment numberOfPeaksInFeatureDictionary values
                        if (numberOfPeaksInFeatureDictionary.ContainsKey(currentNumberOfPeaks))
                        {
                            numberOfPeaksInFeatureDictionary.UpdateValue(currentNumberOfPeaks, numberOfPeaksInFeatureDictionary.FetchValue(currentNumberOfPeaks) + 1);
                        }
                        else
                        {
                            numberOfPeaksInFeatureDictionary.AddElement(currentNumberOfPeaks, 1);
                        }
                        totalNumberOfPeakGroups++;
                        currentNumberOfPeaks = 2;

                        #endregion
                    }
                }
                //Resets the used property to false to allow a peak to be used in features of multiple charge states
                foreach (DifferenceObject<double> diffObject in chargeSortedDifferenceObjectReturnList)
                {
                    diffObject.Used = false;
                }
            }

            foreach (Tuple<int, int> pair in numberOfPeaksInFeatureDictionary.Data)
            {
                double percent = pair.Item2 / totalNumberOfPeakGroups * 100;
                percentOfFeaturesWithPeakNumber.AddElement(pair.Item1, percent);
            }

            #region Stopwatch
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds");

            #endregion

            //HammerPeakDetector.NoiseThresholder noiseThresholder = new HammerPeakDetector.NoiseThresholder(data, featureList, new HammerThresholdParameters);

            return featureList;
        }

        /// <summary>
        /// this will be smart and figure out a good cluster spacing to use
        /// </summary>
        /// <param name="clustersIn"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<ClusterCP<double>> RefineClusters(List<ClusterCP<double>> clustersIn, List<PNNLOmics.Data.Peak> data, out double standardDeviation)
        {
            
            List<double> differencesDaScale = new List<double>();
            foreach (ClusterCP<double> cluster in clustersIn)
            {
                foreach (DifferenceObject<double> pair in cluster.Peaks)
                {
                    int charge = 1;
                    charge = getCharge(pair);

                    differencesDaScale.Add((pair.Value2 - pair.Value1)*charge);
                }
            }

            List<string> differencesString = new List<string>();
            foreach (double difference in differencesDaScale)
            {
                differencesString.Add(difference.ToString());
            }

            //string filename = @"K:\differences"+ MassOfSpace.ToString() + "_" + MassToleranceDa + ".txt";
            //GetPeaks_DLL.DataFIFO.StringListToDisk writer = new StringListToDisk();
            //writer.toDiskStringList(filename, differencesString);


            StandardDeviation calculator = new StandardDeviation();
            double average = 0;
            standardDeviation = calculator.FindStandardDeviation(differencesDaScale, out average);

            double max;

            double min;
            calculator.FindMaxAndMin(differencesDaScale, out max, out min);

            double rangeDifference = max - min;

            //double coefficeintOfVariation = standardDeviation/average*100;
            //if (coefficeintOfVariation > .1)
            //{
                this.MassToleranceDa = standardDeviation * 2;//this should decrease the size
                
            //}
            //else
            //{
             //   //cv is acceptable but we can expand a bit
            //    this.MassToleranceDa = standardDeviation * 3;//this should increase the size
            //}

            this.MassOfSpace = average;
            

            //Run algorithm again with new values learned from data.

            //Creates a new list of features that will contain features of corresponding peaks based on m/z ratio
            List<ClusterCP<double>> featureList = new List<ClusterCP<double>>();

            featureList = this.FindClusters(data);

            return featureList;
        }




        private static int getCharge(DifferenceObject<double> pair)
        {
            int charge = 1;
            int index = pair.DifferenceIndex;
            switch (index)
            {
                case 0:
                    {
                        charge = 1;
                    }
                    break;
                case 1:
                    {
                        charge = 2;
                    }
                    break;
                case 2:
                    {
                        charge = 3;
                    }
                    break;
                case 3:
                    {
                        charge = 4;
                    }
                    break;
                case 4:
                    {
                        charge = 5;
                    }
                    break;
            }

            return charge;
        }
    }
}
