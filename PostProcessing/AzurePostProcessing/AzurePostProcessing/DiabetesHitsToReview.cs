using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using FilesAndFolders;
using GetPeaksDllLite.DataFIFO;
using IQGlyQ.FIFO;
using IQGlyQ.Objects;
using MultiDatasetToolBox;
using MultiDatasetToolBox.FIFO;
using NUnit.Framework;

namespace AzurePostProcessing
{
    class DiabetesHitsToReview
    {
        public static string workingDirectory = @"D:\PNNL\Projects\DATA DIABETES PIC 2014-03-27\Results";
        public static string currentworkingDirectory = Path.Combine(workingDirectory, "GlyQIQResults", "Stages", "3_AlignedData_LocatedInElutionTimeTheor", "ManuallyModified");
        //public static string outputDirectory = Path.Combine(workingDirectory, "Output");
        public static string outputDirectory = @"X:\Output";

        
        string[] joiner = new string[2];

        public static char seperator = ',';
        public static string seperatorString = seperator.ToString(CultureInfo.InvariantCulture);
        string falseResult = "0";//place holder

        public double maxNET = 120;//min

        StringListToDisk writer = new StringListToDisk();

        [Test]
        //4.  this is used to extracte the desired glycans and write out data for the Glycolzyer
        public void PullHitsForGlyQIQViewer()
        {
            //1.  load in datafile names and short names

            UtilitiesForGet.DataSetGroup currentGroup = UtilitiesForGet.DataSetGroup.All26;

            List<string> currentFileNamesShort;
            List<string> currentFileNames;
            UtilitiesForGet.SetDataInput(currentGroup, out currentFileNamesShort, out currentFileNames);


            for (int i = 0; i < currentFileNames.Count; i++)
            {
                currentFileNames[i] = "LCAligned_" + currentFileNames[i];
            }

            Assert.AreEqual(currentFileNames.Count, currentFileNamesShort.Count);

            //2.  convert filenames into data
            List<Dataset> datasetPile = LoadDatasets.Load(currentFileNames, currentworkingDirectory);

            Assert.AreEqual(datasetPile.Count, currentFileNames.Count);

            Assert.AreEqual(datasetPile[0].DataSetName, currentFileNames[0]);

            //3.  which glycans do we care about
            //List<string> codesToPull = GetCodes.PolySialicAcidTest();
            //List<string> codesToPull = GetCodes.CodesToPullFullLibrary();//A.  do this first and see how missting data looks
            //List<string> codesToPull = GetCodes.DiabetesHits();

            //List<string> codesToPull = GetCodes.HighMannose();//B.  this is a good and contains some missing data filled in
            List<string> codesToPull = GetCodes.Best45();//B.  this is a good and contains some missing data filled in
            //List<string> codesToPull = GetCodes.Best25();//B.  no missing data.  all present
            //List<string> codesToPull = GetCodes.Best105();//B.  this is a good missing data list where 50% of ions are present

            //4.  take headers from first dataset loaded
            string headers = datasetPile[0].headerToResults;

            //5.  what we will write to disk! *(1/8)
            List<Dataset> datasetToWrite = new List<Dataset>();//this is the main dataoutput for a given set of targets

            List<string> datasetWithCode = new List<string>();//this tells us which datasets had hits

            



            //6. set column header *(2/8)
            UtilitiesForGet.SetHeader(datasetWithCode, codesToPull, seperatorString);
            

            int length = codesToPull.Count + 2;//+1 for counts and + 1 for side header

            //initialize Array
            List<int> hitsWithinACode = Enumerable.Repeat(0, length).ToArray().ToList();
            hitsWithinACode[0] = 0;//filler.  need a +1 later on

            for (int i = 0; i < datasetPile.Count; i++)
            {
                Dataset currentDataset = datasetPile[i];
                Dataset datasetOut = new Dataset();
                datasetOut.DataSetName = currentDataset.DataSetName;
                datasetOut.results = new List<GlyQIqResult>(); //this may not be needed
                datasetOut.headerToResults = headers;

                //within a dataset variables *(3/8)
                string interDatasetHitsCount = "Datasets";
                string interScansHitsCount = currentFileNamesShort[i];
                string internetHitsCount = currentFileNamesShort[i];
                string intensityHCount = currentFileNamesShort[i];
                string intensityDCount = currentFileNamesShort[i];

                //how many glycans are detected in each dataset
                int hitsWithinADataset = 0;

                for (int j = 0; j < codesToPull.Count; j++)
                {

                    string code = codesToPull[j];
                    //string[] GlyQIqResultHits = (from n in dataset.results where n.Code == code select n.Abundance).ToArray();
                    //string[] GlyQIqResultHitsScans = (from n in dataset.results where n.Code == code select n.Scan).ToArray();
                    //List<double> GlyQIqResultHitsDouble = Array.ConvertAll(GlyQIqResultHits, double.Parse).ToList();

                    //filter 1, get all records for a given code
                    List<GlyQIqResult> GlyQIqResultHits = (from n in currentDataset.results where n.Code == code select n).ToList();

                    //filter 2, off for alignment
                    GlyQIqResultHits = FilterResults.Standard(GlyQIqResultHits, FilterResults.Filters.MostAbundant);

                    ///Actual work *(4/8 below)

                    #region 1. dataworkup

                    //only works for low glycan counds
                    bool smallTargetList = true; //if you run out of memory, make this false
                    if (smallTargetList)
                    {
                        datasetOut.results.AddRange(GlyQIqResultHits);
                    }

                    #endregion end data workup

                    #region 2. bookkeeping

                    if (GlyQIqResultHits.Count > 0)
                    {
                        hitsWithinADataset++;
                        hitsWithinACode[j + 1]++;
                    }

                    #endregion
                }
                //Add data to pile *(5/8)

                datasetToWrite.Add(datasetOut);
            }

            //Add data to pile *(6/8)
            datasetWithCode.Add(String.Join(seperatorString, hitsWithinACode));

            //set paths *(7/8)
            string matrixToWritePath = Path.Combine(outputDirectory, "GlyQ-IQ-Viewer", "Hits");

            string directoryHits = Path.Combine(outputDirectory, "GlyQ-IQ-Viewer", "Hits");
            if (Directory.Exists(matrixToWritePath))
            {
                Directory.Delete(matrixToWritePath, true);
            }
            Directory.CreateDirectory(matrixToWritePath);

            string datasetbasePath = Path.Combine(outputDirectory, "GlyQ-IQ-Viewer", "Hits", "Check_");

             bool writeDatasets = true;
             if (writeDatasets)
             {
                 for (int j = 0; j < datasetToWrite.Count; j++)
                 {
                     List<string> dataLines = new List<string>();


                     Dataset currentDataset = datasetToWrite[j];

                     dataLines.Add(currentDataset.headerToResults);

                     foreach (var result in currentDataset.results)
                     {
                         dataLines.Add(result.GlyQIqResultToString());
                         
                         writer.toDiskStringList(datasetbasePath + "_" + currentDataset.DataSetName + ".txt",dataLines);
                     }
                 }

                 
             }
        }
    }
}
