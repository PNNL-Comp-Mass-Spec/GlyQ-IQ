using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HammerPeakDetector;
using PNNLOmics.Data;
using OrbitrapPeakDetection.Objects;
using OrbitrapPeakDetection;
using GetPeaks_DLL.Functions;
using CompareContrastDLL;

namespace OrbitrapPeakDetection
{
    public class TestRawData
    {
        ///Identifies all raw data as either signal or noise based on intensity relative to noise thresholds
        public void ParseSignalAndNoise(List<PNNLOmics.Data.Peak> data, HammerThresholdParameters parameters, List<ClusterCP<double>> clusters, out List<ClusterCP<double>> clustersAutomated, out List<PNNLOmics.Data.Peak> signalPeaksAutomated,
            out List<PNNLOmics.Data.Peak> noisePeaksAutomated)
        {
            //Goes to RelativeNoiseThreshold to establish what the noise threshold is found to be
            RelativeNoiseThreshold thresholdData = new RelativeNoiseThreshold();
            List<PNNLOmics.Data.Peak> noiseThreshold = new List<PNNLOmics.Data.Peak>();


            noiseThreshold = thresholdData.FindRelativeNoiseThresholds(data, clusters, parameters);

            //Boolean used to track whether peak has been defined as signal or noise by the program
            bool[] signal = new bool[data.Count];

            //Indicates whether the peak has been found to correspond to a noise threshold or not
            bool dataUsed = false;
            //Threshold number that peak corresponds to by m/z
            int thresholdNumber = 0;

            // TODO add to signal XYData list and noise XYData list
            //Analyzes each peak to identify whether the noise threshold defines it as signal or noise

            noisePeaksAutomated = new List<PNNLOmics.Data.Peak>();
            signalPeaksAutomated = new List<PNNLOmics.Data.Peak>();

            for (int i = 0; i < data.Count; i++)
            {
                while (dataUsed == false)
                {
                    if ((data[i].XValue < noiseThreshold[thresholdNumber].XValue) | (thresholdNumber == ((noiseThreshold.Count) - 1)))
                    {
                        //If peak falls below the noise threshold, it is noise
                        if (data[i].Height < noiseThreshold[thresholdNumber].Height)
                        {
                            signal[i] = false;
                            noisePeaksAutomated.Add(data[i]);
                        }
                        //If peak falls above the noise threshold, it is signal
                        else
                        {
                            signal[i] = true;
                            signalPeaksAutomated.Add(data[i]);
                        }
                        dataUsed = true;
                    }
                    thresholdNumber++;
                }
                dataUsed = false;
                thresholdNumber = 0;
            }

            ////parse list into signal and noise
            //noisePeaksAutomated = new List<XYData>();
            //signalPeaksAutomated = new List<XYData>();

            ////Adds signal and noise to lists
            //for (int i = 0; i < data.Count; i++)
            //{
            //    if (signal[i] == true)
            //    {
            //        signalPeaksAutomated.Add(data[i]);
            //    }
            //    else
            //    {
            //        noisePeaksAutomated.Add(data[i]);
            //    }
            //}

            //Reads in peak clusters to test whether noise thresholds correctly identify signal vs. noise
            RepeatedClusterFilter featureList = new RepeatedClusterFilter();
            List<ClusterCP<double>> allClustersAutomatedFiltered = featureList.FilterRepeatedPeaks(data, clusters);
            clustersAutomated = allClustersAutomatedFiltered.OrderBy(p => p.Peaks[0].IndexData).ToList();
        }



        ///Identifies all raw data as either signal or noise based on intensity relative to noise thresholds
        public void PileClustersThenParseSignalAndNoise(List<PNNLOmics.Data.Peak> data, HammerThresholdParameters parameters, List<ClusterCP<double>> clusters,
            out List<ClusterCP<double>> clustersAutomated, out List<PNNLOmics.Data.Peak> signalPeaksAutomated,
            out List<PNNLOmics.Data.Peak> noisePeaksAutomated)
        {
            //Goes to RelativeNoiseThreshold to establish what the noise threshold is found to be
            RelativeNoiseThreshold thresholdData = new RelativeNoiseThreshold();
            List<PNNLOmics.Data.Peak> noiseThreshold = new List<PNNLOmics.Data.Peak>();


            noiseThreshold = thresholdData.FindRelativeNoiseThresholds(data, parameters);

            //Boolean used to track whether peak has been defined as signal or noise by the program
            bool[] signal = new bool[data.Count];

            //Indicates whether the peak has been found to correspond to a noise threshold or not
            bool dataUsed = false;
            //Threshold number that peak corresponds to by m/z
            int thresholdNumber = 0;

            // TODO add to signal XYData list and noise XYData list
            //Analyzes each peak to identify whether the noise threshold defines it as signal or noise

            noisePeaksAutomated = new List<PNNLOmics.Data.Peak>();
            signalPeaksAutomated = new List<PNNLOmics.Data.Peak>();

            for (int i = 0; i < data.Count; i++)
            {
                while (dataUsed == false)
                {
                    if ((data[i].XValue < noiseThreshold[thresholdNumber].XValue) | (thresholdNumber == ((noiseThreshold.Count) - 1)))
                    {
                        //If peak falls below the noise threshold, it is noise
                        if (data[i].Height < noiseThreshold[thresholdNumber].Height)
                        {
                            signal[i] = false;
                            noisePeaksAutomated.Add(data[i]);
                        }
                        //If peak falls above the noise threshold, it is signal
                        else
                        {
                            signal[i] = true;
                            signalPeaksAutomated.Add(data[i]);
                        }
                        dataUsed = true;
                    }
                    thresholdNumber++;
                }
                dataUsed = false;
                thresholdNumber = 0;
            }

            ////parse list into signal and noise
            //noisePeaksAutomated = new List<XYData>();
            //signalPeaksAutomated = new List<XYData>();

            ////Adds signal and noise to lists
            //for (int i = 0; i < data.Count; i++)
            //{
            //    if (signal[i] == true)
            //    {
            //        signalPeaksAutomated.Add(data[i]);
            //    }
            //    else
            //    {
            //        noisePeaksAutomated.Add(data[i]);
            //    }
            //}

            //Reads in peak clusters to test whether noise thresholds correctly identify signal vs. noise
            RepeatedClusterFilter featureList = new RepeatedClusterFilter();
            List<ClusterCP<double>> allClustersAutomatedFiltered = featureList.FilterRepeatedPeaks(data, clusters);
            clustersAutomated = allClustersAutomatedFiltered.OrderBy(p => p.Peaks[0].IndexData).ToList();
        }


        //public void ParseClusterToSignalWithoutThresholding(List<XYData> peaks, List<ClusterCP<double>> clustersAutomatedRefined, out List<XYData> signalPeaksAutomatedFromCluster, out List<XYData> noisePeaksAutomatedFromCluster)
        public void ParseClusterToSignalWithoutThresholding(List<PNNLOmics.Data.Peak> peaks, List<ClusterCP<double>> clustersAutomatedRefined, out List<PNNLOmics.Data.Peak> signalPeaksAutomatedFromCluster, out List<PNNLOmics.Data.Peak> noisePeaksAutomatedFromCluster)
        
        {
            signalPeaksAutomatedFromCluster = new List<PNNLOmics.Data.Peak>();
            foreach (ClusterCP<double> cluster in clustersAutomatedRefined)
            {
                //first point
                //signalPeaksAutomatedFromCluster.Add(peaks[cluster.Peaks[0].IndexData]);

                //second point it when i=0
                for (int i = 0; i < cluster.Peaks.Count; i++)//+1 to get the last point
                {
                    if (i == cluster.Peaks.Count - 1)//last piont take the data and the match
                    {
                        //signalPeaksAutomatedFromCluster.Add(peaks[cluster.Peaks[i].IndexData]);
                        //signalPeaksAutomatedFromCluster.Add(peaks[cluster.Peaks[i].IndexMatch]);
                        signalPeaksAutomatedFromCluster.Add(peaks[cluster.Peaks[i].IndexData]);
                        signalPeaksAutomatedFromCluster.Add(peaks[cluster.Peaks[i].IndexMatch]);
                    }
                    else
                    {
                        //signalPeaksAutomatedFromCluster.Add(peaks[cluster.Peaks[i].IndexData]);
                        signalPeaksAutomatedFromCluster.Add(peaks[cluster.Peaks[i].IndexData]);
                    }
                }

            }

            //signalPeaksAutomatedFromCluster = signalPeaksAutomatedFromCluster.OrderBy(p => p.X).ToList();
            signalPeaksAutomatedFromCluster = signalPeaksAutomatedFromCluster.OrderBy(p => p.XValue).ToList();

            double massTollerance = 0.001;
            IConvert letsConvert = new Converter();
            SetListsToCompare prepCompare = new SetListsToCompare();
            CompareController compareHere = new CompareController();

            //from manual annotation
            //List<double> manualMassLibrary = letsConvert.XYDataToMass(peaks);
            List<double> manualMassLibrary = new List<double>();
            foreach (PNNLOmics.Data.Peak d in peaks)
            {
                manualMassLibrary.Add(d.XValue);
            }

            

            //from algorithm
            //List<double> automatedData = letsConvert.XYDataToMass(signalPeaksAutomatedFromCluster);
            List<double> automatedData = new List<double>();
            foreach (PNNLOmics.Data.Peak d in signalPeaksAutomatedFromCluster)
            {
                automatedData.Add(d.XValue);
            }

            CompareInputLists inputListsPeakMasses = prepCompare.SetThem(automatedData, manualMassLibrary);

            CompareResultsIndexes indexesFromCompare = new CompareContrastDLL.CompareResultsIndexes();
            CompareResultsValues valuesFromCompare = compareHere.compareFX(inputListsPeakMasses, massTollerance, ref indexesFromCompare);

            //noisePeaksAutomatedFromCluster = new List<XYData>();
            noisePeaksAutomatedFromCluster = new List<PNNLOmics.Data.Peak>();
            foreach(int index in indexesFromCompare.IndexListBandNotA)
            {
                //noisePeaksAutomatedFromCluster.Add(peaks[index]);
                noisePeaksAutomatedFromCluster.Add(peaks[index]);
            }
        }
    }
}
