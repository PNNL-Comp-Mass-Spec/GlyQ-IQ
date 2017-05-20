using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OrbitrapPeakThresholder;
using NUnit.Framework;
using System.Diagnostics;
using GetPeaks_DLL.Objects;
using OrbitrapPeakThresholder.Utilities;
using Deisotoping.DeconToolsWrapper.DeconToolsWrapper;
using OrbitrapPeakThresholder.Objects;
using PNNLOmics.Data;
using Deisotoping.DeconToolsWrapper;
using OrbitrapPeakThresholderUnitTests.Enumerations;
using GetPeaks_DLL.DataFIFO;

namespace OrbitrapPeakThresholderUnitTests
{
    class UnitTests
    {

        [Test]
        public void OrbitrapFilterUnitTest()
        {
            InputOutputFileName newFile = SetupFiles();

            OrbitrapPeakParameters parameters = new OrbitrapPeakParameters();
            parameters.massTolleranceMatch = 3;
            parameters.orbitrapFilterSigmaMultiplier = 0;

            OrbitrapPeaksController controller = new OrbitrapPeaksController();
            controller.Threshold(parameters);

            

            Assert.AreEqual("Finished", "Finished");
        }

        [Test]
        public void DeconToolsUnitTest()
        {
            InputOutputFileName newFile = SetupFiles();

            DeconParameters parameters = new DeconParameters();
            parameters.PeakToBackgroundRatio = 2;//for decon thresholding


            Assert.AreEqual("Finished", "Finished");
        }

        [Test]
        public void LoadManualData()
        {
            InputOutputFileName newFile = SetupFiles();
            ManualData results  = LoadManuallyAnotatedData(newFile);

            Assert.AreEqual(results.PeaksAll.Count, 1798);
            Assert.AreEqual(results.PeaksMono.Count, 345);
            Assert.AreEqual(results.PeaksNoise.Count, 846);
            Assert.AreEqual(results.PeaksSignal.Count, 952);
            Assert.AreEqual(results.RawData.Count, 24300);
            Assert.AreEqual(results.Clusters.Count, 339);
            

            Assert.AreEqual("Finished", "Finished");
        }

        [Test]
        public void RaceTheAlgorithm()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            System.DateTime starttime = DateTime.Now;

            System.DateTime stoptime = DateTime.Now;
            Console.WriteLine("This started at " + starttime + " and ended at" + stoptime);
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds");
            Console.WriteLine("");

            Console.WriteLine("");
            Console.Write("\npress a key to close");
            
        }

        private InputOutputFileName SetupFiles()
        {
            string locationOfXYData = "";
            string locationOfDeconRawFile = "";
            InputOutputFileName newFile = new InputOutputFileName();
            ConvertAutoData dataConverter = new ConvertAutoData();
            DeconToolsDeisotopeCP monoGenerator = new DeconToolsDeisotopeCP();

            int switchint = 1; //0 = Christina, 1 = Scott, 2=Home


            switch (switchint)
            {
                #region inside
                case 0:
                    {
                        locationOfXYData = @"D:\QC Shew Analysis\QC Shew 5509.txt";
                        locationOfDeconRawFile = @"D:\QC Shew Analysis\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";

                        newFile.OutputSQLFileName = @"D:\DeisotopingDatabaseHi.db";
                        newFile.OutputPath = @"D:";
                        newFile.InputFileName = @"D:\QC Shew Analysis\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";
                        newFile.InputSQLFileName = locationOfXYData;//temporary storage

                    }
                    break;
                case 1:
                    {
                        locationOfXYData = @"D:\PNNL\Projects\Christina Polyukh\QC Shew Analysis\QC Shew 5509.txt";
                        locationOfDeconRawFile = @"D:\PNNL\Projects\Christina Polyukh\QC Shew Analysis\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";

                        newFile.OutputSQLFileName = @"V:\DeisotopingDatabaseHi.db";
                        newFile.OutputPath = @"V:";
                        newFile.InputFileName = locationOfDeconRawFile;
                        newFile.InputSQLFileName = locationOfXYData;//temporary storage

                    }
                    break;
                case 2:
                    {
                        locationOfXYData = @"L:\PNNL Files\PNNL\Projects\Christina Polyukh\QC Shew Analysis\QC Shew 5509.txt";
                        locationOfDeconRawFile = @"L:\PNNL Files\PNNL\Projects\Christina Polyukh\QC Shew Analysis\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";

                        newFile.OutputSQLFileName = @"V:\DeisotopingDatabaseHi.db";
                        newFile.OutputPath = @"V:";
                        newFile.InputFileName = locationOfDeconRawFile;
                        newFile.InputSQLFileName = locationOfXYData;//temporary storage

                    }
                    break;
                #endregion
            }

            return newFile;
        }

        private ManualData LoadManuallyAnotatedData(InputOutputFileName newFile)
        {
            string locationOfXYData = newFile.InputSQLFileName;
            string rawFileToLoad = newFile.InputSQLFileName;

            GetManualData dataLoader = new GetManualData();
            List<XYData> peaks = new List<XYData>();
            peaks = dataLoader.Load(SpectraDataType.RawData);

            List<ClusterCP<double>> clustersManual = new List<ClusterCP<double>>();
            clustersManual = dataLoader.LoadClusters();

            List<XYData> signalPeaksManual = new List<XYData>();
            signalPeaksManual = dataLoader.LoadPeakAnswers(SpectraDataType.SignalPeaks);

            List<XYData> noisePeaksManual = new List<XYData>();
            noisePeaksManual = dataLoader.LoadPeakAnswers(SpectraDataType.NoisePeaksFull);
            
            SortedDictionary<int, XYData> loadedRawDataFromFileDictionary = LoadRawDataTextFileTab(rawFileToLoad);

            List<XYData> monoisotopicMassesManual = new List<XYData>();
            monoisotopicMassesManual = dataLoader.LoadPeakAnswers(SpectraDataType.MonoisotopicMasses);

            ManualData results = new ManualData();
            results.PeaksAll = peaks;
            results.Clusters = clustersManual;
            results.PeaksSignal = signalPeaksManual;
            results.PeaksNoise = noisePeaksManual;
            results.RawData = loadedRawDataFromFileDictionary;
            results.PeaksMono = monoisotopicMassesManual;

            return results;
        }
        private static SortedDictionary<int, XYData> LoadRawDataTextFileTab(string fileToLoad)
        {
            string columnHeaders = "";
            List<XYData> loadedDataFromFile = new List<XYData>();
            LoadXYData XYloader = new LoadXYData();
            loadedDataFromFile = XYloader.Import(fileToLoad, out columnHeaders);
            //TEMP convert to Dictionary
            SortedDictionary<int, XYData> loadedDataFromFileDictionary = new SortedDictionary<int, XYData>();
            int i = 0;
            foreach (XYData dataPoint in loadedDataFromFile)
            {
                loadedDataFromFileDictionary.Add(i, dataPoint);
                i++;
            }
            return loadedDataFromFileDictionary;
        }

    }
}
