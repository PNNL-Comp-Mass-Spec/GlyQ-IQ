using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FilesAndFolders;
using GetPeaksDllLite.DataFIFO;
using IQGlyQ.FIFO;
using MathNet.Numerics.Statistics;
using MultiDatasetToolBox.FIFO;
using NUnit.Framework;
using IQGlyQ.Objects;
using MultiDatasetToolBox;

namespace AzurePostProcessing
{
    public class UnitTests
    {
        string workingDirectory = @"D:\Csharp\0_TestDataFiles\PostProcessingUnitTests";

        [Test]
        public void LoadStringsFromFilesAndCalculateHistogram()
        {

            string currentworkingDirectory = Path.Combine(workingDirectory,"Results1");

            List<string> currentFileNames = GetInfo.GetSummingFiles();

            List<Dataset> datasetPile = LoadDatasets.Load(currentFileNames, currentworkingDirectory);

            StringListToDisk writer = new StringListToDisk();

            List<List<double>> IsoFitsPile = new List<List<double>>();
            List<List<double>> IsoFitsHistogramPile = new List<List<double>>();

            int numberOfBuckets = 10;
            double minRange = 0;
            double maxRange = 0.1;
            
            List<double> axis = HistogramGenerator.GenerateAxis(minRange, maxRange, numberOfBuckets);
            IsoFitsHistogramPile.Add(axis);

            foreach (var dataset in datasetPile)
            {
                //get fit scores
                List<double> results = (from n in dataset.results select n.IsoFit).Select(Convert.ToDouble).ToList();
                //List<double> results = (from n in dataset.results select n.IsoFit).Select(x => Convert.ToDouble(x)).ToList();
                IsoFitsPile.Add(results);

                //calculate fitscore histograms
                List<double> histogramResults = HistogramGenerator.Generate(results, minRange, maxRange, numberOfBuckets);
                IsoFitsHistogramPile.Add(histogramResults);

                //stats
                double currentAverageFit = results.Average();

                //write individual files
                string writeLocation = Path.Combine(currentworkingDirectory, "IsoFit_" + dataset.DataSetName + ".txt");

                writer.toDiskStringList(writeLocation,results.Select(Convert.ToString).ToList());

                Console.WriteLine("The average fit is " + currentAverageFit + " for " + dataset.DataSetName + " with " + dataset.results.Count + " hits");
            }
            
            //var firscores = (from n in results select new double{n = results[0]});
                //List<IqGlyQResult> results_Children = (from n in scanResultsListFragments where n.ToChild.TypeOfResultParentOrChildDifferenceApproach == EnumerationParentOrChild.ChildrenOnly select n).ToList();
                            
            Assert.AreEqual(datasetPile.Count, currentFileNames.Count);


            string matrixToWritePath = Path.Combine(currentworkingDirectory, "Matrix_IsoFit.txt");
            string matrixHistogramToWritePath = Path.Combine(currentworkingDirectory, "Matrix_IsoFit_Histogram.txt");

            string filler = "0";
            
            List<string> headers = new List<string>();
            
            for (int i = 0; i < datasetPile.Count; i++)
            {
                headers.Add(datasetPile[i].DataSetName);
            }

            List<string> isoFitMaxtrixToWrite = CrossTabBuilder.BuildMatrix(IsoFitsPile, headers, filler);

            headers.Insert(0,"xAxis");
            List<string> histogramMaxtrixToWrite = CrossTabBuilder.BuildMatrix(IsoFitsHistogramPile, headers, filler);

            writer.toDiskStringList(matrixToWritePath, isoFitMaxtrixToWrite);
            writer.toDiskStringList(matrixHistogramToWritePath, histogramMaxtrixToWrite);
        }


        [Test]
        ///Using a list of GlyQIQ results datafiles, load the data from the disk to memory objects as GlyQIqResult strings
        public void LoadStringsFromFiles()
        {
            string currentworkingDirectory = Path.Combine(workingDirectory, "Results1");

            List<string> currentFileNames = GetInfo.GetSummingFiles();

            List<Dataset> datasetPile = LoadDatasets.Load(currentFileNames, currentworkingDirectory);

            Assert.AreEqual(datasetPile.Count, currentFileNames.Count);

            Assert.AreEqual(datasetPile[0].DataSetName, currentFileNames[0]);
            Assert.AreEqual(Convert.ToInt32(datasetPile[1].results[0].Scan), 485);
            Assert.AreEqual(Convert.ToInt32(datasetPile[10].results[0].Scan), 1022);
            Assert.AreEqual(datasetPile[10].results[0].isotopes.Count, 47);
        }

        [Test]
        ///Load in PSA results files. then pull out the targets we want and write the hits out
        public void LoadStringsFromPSAFiles()
        {
            string currentworkingDirectory = Path.Combine(workingDirectory, "PSATest");

            List<string> currentFileNames = GetInfo.GetSPSAFiles();

            List<Dataset> datasetPile = LoadDatasets.Load(currentFileNames, currentworkingDirectory);

            Assert.AreEqual(datasetPile.Count, currentFileNames.Count);

            Assert.AreEqual(datasetPile[0].DataSetName, currentFileNames[0]);

            List<string> codesToPull = GetCodes.PolySialicAcidTest();

            //List<string> headers = new List<string>();
            string headers = datasetPile[0].headerToResults;

            List<Dataset> datasetToWrite = new List<Dataset>();
           
            foreach (var dataset in datasetPile)
            {
                //headers.Add(dataset.DataSetName);
                Dataset datasetOut = new Dataset();
                datasetOut.DataSetName = dataset.DataSetName;
                datasetOut.results = new List<GlyQIqResult>();//this may not be needed

                foreach (var code in codesToPull)
                {
                    //string[] GlyQIqResultHits = (from n in dataset.results where n.Code == code select n.Abundance).ToArray();
                    //string[] GlyQIqResultHitsScans = (from n in dataset.results where n.Code == code select n.Scan).ToArray();
                    //List<double> GlyQIqResultHitsDouble = Array.ConvertAll(GlyQIqResultHits, double.Parse).ToList();

                    //filter 1, get all records for a given code
                    List<GlyQIqResult> GlyQIqResultHits = (from n in dataset.results where n.Code == code select n).ToList();
                    //filter 2, get insource fragments

                    bool filterOnInsourceFragmentName = true;
                    if (filterOnInsourceFragmentName)
                    {
                        GlyQIqResultHits = FilterResults.Standard(GlyQIqResultHits, FilterResults.Filters.Name);
                    }

                    bool filterOnAnnotationType = true;
                    if (filterOnAnnotationType)
                    {
                        GlyQIqResultHits = FilterResults.Standard(GlyQIqResultHits, FilterResults.Filters.FinalDecision);
                    }

                    datasetOut.results.AddRange(GlyQIqResultHits);
                }

                datasetToWrite.Add(datasetOut);
            }

            string matrixToWritePath = Path.Combine(currentworkingDirectory, "Matrix_IsoFit.txt");

            StringListToDisk writer = new StringListToDisk();
            string filler = "0";
            List<string> abundanceFitMaxtrixToWrite = CrossTabBuilder.BuildOneToManyResults(datasetToWrite, headers);
            writer.toDiskStringList(matrixToWritePath, abundanceFitMaxtrixToWrite);
        }

        private double CalculateStdDev(IEnumerable<double> values)
        {
            double ret = 0;
            if (values.Count() > 0)
            {
                //Compute the Average      
                double avg = values.Average();
                //Perform the Sum of (value-avg)_2_2      
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                //Put it all together      
                ret = Math.Sqrt((sum) / (values.Count() - 1));
            }
            return ret;
        }
    }
}
