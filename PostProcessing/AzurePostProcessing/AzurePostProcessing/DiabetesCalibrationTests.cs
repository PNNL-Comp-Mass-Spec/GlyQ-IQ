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
    class DiabetesCalibrationTests
    {
        /// <summary>
        /// Base Information
        /// </summary>

        public static string workingDirectory = @"D:\PNNL\Projects\DATA DIABETES PIC 2014-03-27\Results";
        public static string currentworkingDirectory = Path.Combine(workingDirectory, "GlyQIQResults", "Stages", "0_Results_From_GlyQ-IQ");
        //public static string outputDirectory = Path.Combine(workingDirectory, "Output");
        public static string outputDirectory = @"X:\Output";

        string[] joiner = new string[2];

        public static char seperator = ',';
        public static string seperatorString = seperator.ToString(CultureInfo.InvariantCulture);
        string falseResult = "0";//place holder

        public double maxNET = 120;//min

        StringListToDisk writer = new StringListToDisk();


        [Test]
        //1.  pulls glycans from datasets (GetCodes.DiabetesHits();) will pull the common glycans to all datasets
        public void PullAlignmentDataFromAllDatasets()
        {
            
            //1.  load in datafile names and short names
            List<string> currentFileNames = GetInfo.DatasetDiabetesFiles26();
            List<string> currentFileNamesShort = GetInfo.DatasetID26();

            Assert.AreEqual(currentFileNames.Count, currentFileNamesShort.Count);

            //2.  convert filenames into data
            List<Dataset> datasetPile = LoadDatasets.Load(currentFileNames, currentworkingDirectory);

            Assert.AreEqual(datasetPile.Count, currentFileNames.Count);

            Assert.AreEqual(datasetPile[0].DataSetName, currentFileNames[0]);

            //3.  which glycans do we care about
            //List<string> codesToPull = GetCodes.PolySialicAcidTest();
            //List<string> codesToPull = GetCodes.CodesToPullFullLibrary();
            List<string> codesToPull = GetCodes.DiabetesHits();

            //4.  take headers from first dataset loaded
            string headers = datasetPile[0].headerToResults;





            //5.  what we will write to disk! *(1/)
            List<Dataset> datasetToWrite = new List<Dataset>();//this is the main dataoutput for a given set of targets

            List<string> datasetWithCode = new List<string>();//this tells us which datasets had hits

            List<string> datasetWithCodeMaxScan = new List<string>();//this tells us which datasets had hits

            List<string> datasetWithCodeMaxnet = new List<string>();//this tells us which datasets had hits



            //6. set column header *(2/)
            SetHeader(datasetWithCode, codesToPull);
            SetHeader(datasetWithCodeMaxScan, codesToPull);
            SetHeader(datasetWithCodeMaxnet, codesToPull);

            int length = codesToPull.Count + 2;//+1 for counts and + 1 for side header

            //initialize Array
            List<int> hitsWithinACode = Enumerable.Repeat(0, length).ToArray().ToList();
            hitsWithinACode[0] = 0;//filler.  need a +1 later on

            for (int i = 0; i < datasetPile.Count; i++)
            {
                Dataset currentDataset = datasetPile[i];
                Dataset datasetOut = new Dataset();
                datasetOut.DataSetName = currentDataset.DataSetName;
                datasetOut.results = new List<GlyQIqResult>();//this may not be needed

                //within a dataset variables *(3/)
                string interDatasetHitsCount = "Datasets";
                string interScansHitsCount = currentFileNamesShort[i];
                string internetHitsCount = currentFileNamesShort[i];

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
                    GlyQIqResultHits = FilterResults.Standard(GlyQIqResultHits, FilterResults.Filters.None);

                    ///Actual work *(4/)

                    #region 1. dataworkup

                    //only works for low glycan counds
                    bool smallTargetList = true;//if you run out of memory, make this false
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

                    #region 3a.  metadata interdataset results

                    joiner[0] = interDatasetHitsCount;
                    if (GlyQIqResultHits.Count > 0)
                    {
                        joiner[1] = currentFileNamesShort[i];//payload
                    }
                    else
                    {
                        joiner[1] = falseResult;
                    }
                    interDatasetHitsCount = String.Join(seperatorString, joiner);

                    #endregion end meta data

                    #region 3b.  metadata interdataset results (Scan)

                    joiner[0] = interScansHitsCount;
                    if (GlyQIqResultHits.Count > 0)
                    {
                        //find most abundant scan
                        //List<GlyQIqResult> GlyQIqResultHitsByAbundance = (from n in GlyQIqResultHits orderby Convert.ToDouble(n.GlobalAggregateAbundance) descending select n).ToList();
                        List<GlyQIqResult> glyQIqResultHitsByAbundance = GlyQIqResultHits.OrderByDescending(n => Convert.ToDouble(n.GlobalAggregateAbundance)).ToList();

                        joiner[1] = glyQIqResultHitsByAbundance[0].Scan;//payload
                    }
                    else
                    {
                        joiner[1] = falseResult;

                    }
                    interScansHitsCount = String.Join(seperatorString, joiner);

                    #endregion end meta data

                    #region 3b.  metadata interdataset results (NET)

                    joiner[0] = internetHitsCount;
                    if (GlyQIqResultHits.Count > 0)
                    {
                        //find most abundant scan
                        List<GlyQIqResult> glyQIqResultHitsByAbundance = GlyQIqResultHits.OrderByDescending(n => Convert.ToDouble(n.GlobalAggregateAbundance)).ToList();

                        joiner[1] = glyQIqResultHitsByAbundance[0].ElutionTimeObs;//payload
                    }
                    else
                    {
                        joiner[1] = falseResult;

                    }
                    internetHitsCount = String.Join(seperatorString, joiner);

                    #endregion end meta data

                }



                //Add data to pile *(5/)

                datasetToWrite.Add(datasetOut);
                datasetWithCode.Add(interDatasetHitsCount + "," + hitsWithinADataset);
                datasetWithCodeMaxScan.Add(interScansHitsCount + "," + hitsWithinADataset);
                datasetWithCodeMaxnet.Add(internetHitsCount + "," + hitsWithinADataset);
            }

            //Add data to pile *(6/)
            datasetWithCode.Add(String.Join(seperatorString, hitsWithinACode));
            datasetWithCodeMaxScan.Add(String.Join(seperatorString, hitsWithinACode));
            datasetWithCodeMaxnet.Add(String.Join(seperatorString, hitsWithinACode));
            //end of loop




            //set paths *(5/)
            string matrixToWritePath = Path.Combine(outputDirectory, "Matrix_IsoFit.txt");
            string interDatasetPath = Path.Combine(outputDirectory, "InterDatasetCount.csv");
            string interDatasetPathScan = Path.Combine(outputDirectory, "InterScanCount.csv");
            string interDatasetPathNet = Path.Combine(outputDirectory, "InterNetCount.csv");





            //transpose and write *(6/)

            string filler = "0";
            List<string> abundanceFitMaxtrixToWrite = CrossTabBuilder.BuildOneToManyResults(datasetToWrite, headers);
            writer.toDiskStringList(matrixToWritePath, abundanceFitMaxtrixToWrite);

            //write out hits list across all datasets
            List<string> transposedList = TransposeLines.SwapRowsAndColumns(datasetWithCode, seperatorString);
            writer.toDiskStringList(interDatasetPath, transposedList);

            List<string> transposedListScan = TransposeLines.SwapRowsAndColumns(datasetWithCodeMaxScan, seperatorString);
            writer.toDiskStringList(interDatasetPathScan, transposedListScan);

            List<string> transposedListNet = TransposeLines.SwapRowsAndColumns(datasetWithCodeMaxnet, seperatorString);
            writer.toDiskStringList(interDatasetPathNet, transposedListNet);
        }

        [Test]
        //2.  this needs to be run first so we can get the coefficents from the datafiles
        // Post_DatasetCoeficients.csv will have the code for part 2
        public void AlignDataToSpine()
        {
            
            
            //1.  pull spine data and make sure all posts have values

            //1.  load in datafile names and short names
            List<string> currentFileNames = GetInfo.DatasetDiabetesFiles26();
            List<string> currentFileNamesShort = GetInfo.DatasetID26();

            Assert.AreEqual(currentFileNames.Count, currentFileNamesShort.Count);

            //2.  convertt filenames into data
            List<Dataset> datasetPile = LoadDatasets.Load(currentFileNames, currentworkingDirectory);

            //convert to NET
            double timeFinal = maxNET;//min
            foreach (var dataset in datasetPile)
            {
                foreach (var result in dataset.results)
                {
                    result.ElutionTimeObs = Convert.ToString(Convert.ToDouble(result.ElutionTimeObs) / timeFinal);
                }
            }

            string spineFileName = "C14_DB10_31Dec12_1_Family_iqResults";

            //3.  which glycans do we care about
            //List<string> codesToPull = GetCodes.PolySialicAcidTest();
            //List<string> codesToPull = GetCodes.CodesToPullFullLibrary();
            List<string> codesToPull = GetCodes.DiabetesPosts();

            //4.  take headers from first dataset loaded
            string headers = datasetPile[0].headerToResults;

            //5.  what we will write to disk!

            List<string> datasetWithCodeMaxScanSpine = SetSpine(datasetPile, currentFileNamesShort, codesToPull, spineFileName);

            string interDatasetSpinePath = Path.Combine(outputDirectory, "Spine.csv");
            List<string> transposedListScan = TransposeLines.SwapRowsAndColumns(datasetWithCodeMaxScanSpine, seperatorString);
            writer.toDiskStringList(interDatasetSpinePath, transposedListScan);

            List<List<string>> existingAlignmentFiles = new List<List<string>>();
            List<LinearRegressionResult> alignmentResultsByFile = new List<LinearRegressionResult>();
            bool writeCoeffieicntsToDisk = true;
            string writeCoeffieicntsName = "DatasetCoeficients";

            for (int i = 0; i < datasetPile.Count; i++)
            {
                string experimentalDataName = currentFileNames[i];
                List<string> datasetWithCodeMaxScanExperimentalDataset = SetSpine(datasetPile, currentFileNamesShort, codesToPull, experimentalDataName);

                //add spine column to results file into column 1
                datasetWithCodeMaxScanExperimentalDataset.Insert(1, datasetWithCodeMaxScanSpine[1]);

                //remove zeroes from experimantal data so it is instep with the spine
                RemoveMissingData(datasetWithCodeMaxScanExperimentalDataset);

                //store for internal processing
                existingAlignmentFiles.Add(datasetWithCodeMaxScanExperimentalDataset);

                //and write to disk
                bool writeToDisk = false;
                if (writeToDisk)
                {
                    string interDatasetExperimantalPath = Path.Combine(outputDirectory, "Post_" + experimentalDataName + ".csv");
                    List<string> transposedListExperimental = TransposeLines.SwapRowsAndColumns(datasetWithCodeMaxScanExperimentalDataset, seperatorString);
                    writer.toDiskStringList(interDatasetExperimantalPath, transposedListExperimental);
                }
            }

            //for each dat file
            //calculate slope and intercept
            for (int i = 0; i < datasetPile.Count; i++)
            {
                string experimentalDataName = currentFileNames[i];
                LinearRegressionResult currentResult = new LinearRegressionResult();
                currentResult.DatasetName = experimentalDataName;

                List<string> existingAlignmentFile = existingAlignmentFiles[i];

                //int lengthofData = existingAlignmentFile[1].Split(seperator).ToList().Count;
                var xWords = existingAlignmentFile[1].Split(seperator).ToList();
                var yWords = existingAlignmentFile[2].Split(seperator).ToList();


                //remove number of hits
                xWords.RemoveAt(xWords.Count - 1);
                yWords.RemoveAt(yWords.Count - 1);

                //remove title
                xWords.RemoveAt(0);
                yWords.RemoveAt(0);

                double[] xRaw = (from n in xWords select Convert.ToDouble(n)).ToArray();
                double[] yRaw = (from n in yWords select Convert.ToDouble(n)).ToArray();

                List<double> differences = new List<double>();
                for (int j = 0; j < yRaw.Length; j++)
                {
                    differences.Add(yRaw[j] - xRaw[j]);
                }

                double[] x = yRaw;
                double[] y = differences.ToArray();

                double slope = 1;
                double intercept = 0;
                LinearRegressionFX.Calculate(x, y, ref slope, ref intercept);

                currentResult.Intercept = intercept;
                currentResult.Slope = slope;

                //convert Y
                List<string> alignedDataString = new List<string>();

                alignedDataString.Add("m" + Math.Round(slope, 4) + "x" + Math.Round(intercept, 4));

                List<string> randomXaxis = new List<string>();
                randomXaxis.Add("xAxis");
                int h = 0;

                List<double> alignedData = LinearRegressionFX.ApplyAlgnmentCoefficients(slope, intercept, yRaw);

                //convert to string and make h axis
                foreach (double calibrated in alignedData)
                {
                    alignedDataString.Add((Math.Round(calibrated, 3).ToString()));
                    randomXaxis.Add(h.ToString());
                    h++;
                }

                randomXaxis.Add("EndOfColumn");
                alignedDataString.Add("EndOfColumn");

                existingAlignmentFile.Insert(1, String.Join(seperatorString, randomXaxis));
                existingAlignmentFile.Insert(4, String.Join(seperatorString, alignedDataString));

                alignmentResultsByFile.Add(currentResult);

                //and write to disk
                bool writeToDisk = true;
                if (writeToDisk)
                {
                    string interDatasetExperimantalPath = Path.Combine(outputDirectory, "Post_" + experimentalDataName + ".csv");
                    List<string> transposedListExperimental = TransposeLines.SwapRowsAndColumns(existingAlignmentFile, seperatorString);
                    writer.toDiskStringList(interDatasetExperimantalPath, transposedListExperimental);
                }
            }

            if (writeCoeffieicntsToDisk)
            {
                string q = "\"";
                string coefficientsPath = Path.Combine(outputDirectory, "Post_" + writeCoeffieicntsName + ".csv");
                List<string> lines = new List<string>();
                lines.Add("DataSetName" + "," + "Slope" + "," + "Intercept" + "," + "Code");
                foreach (var result in alignmentResultsByFile)
                {
                    lines.Add(result.DatasetName + "," + result.Slope + "," + result.Intercept + "," + q + "dataLoaded.Add(new LinearRegressionResult(" + q + q + result.DatasetName + q + q + ", " + result.Slope + ", " + result.Intercept + "));" + q);
                }

                writer.toDiskStringList(coefficientsPath, lines);
            }
        }

        [Test]
        //3.  apply coefficients written out in step 1
        public void ApplyCalibrationCoeffientsToResults()
        {
            double timeFinal = maxNET;//min

            //load data
            var dataLoaded = GetCoefficients.GetDiabetesLCCoefficients();

            List<string> currentFileNames = new List<string>();
            for (int i = 0; i < dataLoaded.Count; i++)
            {
                currentFileNames.Add(dataLoaded[i].DatasetName);
            }

            //2.  convertt filenames into data
            List<Dataset> datasetPile = LoadDatasets.Load(currentFileNames, currentworkingDirectory);

            Assert.AreEqual(dataLoaded.Count, datasetPile.Count);

            Assert.IsTrue(datasetPile.Count > 0);

            string titleLine = datasetPile[0].headerToResults;

            for (int i = 0; i < dataLoaded.Count; i++)
            {
                List<string> datasetBuilder = new List<string>();
                datasetBuilder.Add(titleLine);

                Dataset currentDataset = datasetPile[i];
                for (int datapoint = 0; datapoint < currentDataset.results.Count; datapoint++)
                {
                    double elutionTimeObserved = Convert.ToDouble(currentDataset.results[datapoint].ElutionTimeObs) / timeFinal;
                    double currentSlope = dataLoaded[i].Slope;
                    double currentIntercept = dataLoaded[i].Intercept;

                    double allignnedNETInMinutes = LinearRegressionFX.ApplyAlgnmentCoefficients(currentSlope, currentIntercept, elutionTimeObserved) * timeFinal;

                    currentDataset.results[datapoint].ElutionTimeTheor = allignnedNETInMinutes.ToString();

                    string alignedLine = currentDataset.results[datapoint].GlyQIqResultToString("\t");

                    datasetBuilder.Add(alignedLine);
                }

                bool writeToDisk = true;
                if (writeToDisk)
                {
                    string interDatasetExperimantalPath = Path.Combine(outputDirectory, "LCAligned_" + currentFileNames[i] + ".txt");
                    //List<string> transposedListExperimental = TransposeLines.SwapRowsAndColumns(datasetWithCodeMaxScanExperimentalDataset, seperatorString);
                    writer.toDiskStringList(interDatasetExperimantalPath, datasetBuilder);
                }
            }


        }



        private static void RemoveMissingData(List<string> datasetWithCodeMaxScanExperimentalDataset)
        {
            string[] headerWords = datasetWithCodeMaxScanExperimentalDataset[0].Split(seperator);
            string[] spineWords = datasetWithCodeMaxScanExperimentalDataset[1].Split(seperator);
            string[] experimentalWords = datasetWithCodeMaxScanExperimentalDataset[2].Split(seperator);
            string[] hitsWords = datasetWithCodeMaxScanExperimentalDataset[3].Split(seperator);

            Assert.AreEqual(headerWords.Length, spineWords.Length);
            Assert.AreEqual(headerWords.Length, experimentalWords.Length);
            Assert.AreEqual(headerWords.Length, hitsWords.Length);

            List<string> keepHeaderWords = new List<string>();
            List<string> keepSpineWords = new List<string>();
            List<string> keepExperimentalWords = new List<string>();
            List<string> keepHitsWords = new List<string>();

            //add header


            for (int i = 0; i < headerWords.Length; i++)
            {
                if (i == 0) //header won't parse
                {
                    keepHeaderWords.Add(headerWords[i]);
                    keepSpineWords.Add(spineWords[i]);
                    keepExperimentalWords.Add(experimentalWords[i]);
                    keepHitsWords.Add(hitsWords[i]);
                }
                else
                {
                    if (Convert.ToDouble(experimentalWords[i]) > 0)
                    {
                        keepHeaderWords.Add(headerWords[i]);
                        keepSpineWords.Add(spineWords[i]);
                        keepExperimentalWords.Add(experimentalWords[i]);
                        keepHitsWords.Add(hitsWords[i]);
                    }
                }
            }

            datasetWithCodeMaxScanExperimentalDataset[0] = String.Join(seperatorString, keepHeaderWords);
            datasetWithCodeMaxScanExperimentalDataset[1] = String.Join(seperatorString, keepSpineWords);
            datasetWithCodeMaxScanExperimentalDataset[2] = String.Join(seperatorString, keepExperimentalWords);
            datasetWithCodeMaxScanExperimentalDataset[3] = String.Join(seperatorString, keepHitsWords);
        }

        private List<string> SetSpine(List<Dataset> datasetPile, List<string> currentFileNamesShort, List<string> codesToPull, string datasetFileName)
        {
            List<string> resultCollectionLines = new List<string>(); //this tells us which datasets had hits

            //6. set column header
            SetHeader(resultCollectionLines, codesToPull);

            int length = codesToPull.Count + 2; //+1 for counts and + 1 for side header

            //initialize Array
            List<int> hitsWithinACode = Enumerable.Repeat(0, length).ToArray().ToList();
            hitsWithinACode[0] = 0; //filler.  need a +1 later on

            //spine dataset 2 linq options
            Dataset currentDataset = (from n in datasetPile where n.DataSetName == datasetFileName select n).FirstOrDefault();
            var indexy = datasetPile.FindIndex(n => n.DataSetName == datasetFileName);
            var indexz = datasetPile.Select((dataset, index) => new { dataset, index }).First(n => n.dataset.DataSetName == datasetFileName).index;

            string spineFileNameShort = currentFileNamesShort[indexy];

            Console.WriteLine(indexy + " " + indexz);
            Dataset datasetOut = new Dataset();
            datasetOut.DataSetName = currentDataset.DataSetName;
            datasetOut.results = new List<GlyQIqResult>(); //this may not be needed

            //within a dataset variables
            string interScansHitsCount = spineFileNameShort;
            string falseResult = "0"; //place holder

            //how many glycans are detected in each dataset
            int hitsWithinADataset = 0;

            for (int j = 0; j < codesToPull.Count; j++)
            {
                string code = codesToPull[j];

                //filter 1, get all records for a given code
                List<GlyQIqResult> GlyQIqResultHits = (from n in currentDataset.results where n.Code == code select n).ToList();

                //filter 2, off for alignment
                GlyQIqResultHits = FilterResults.Standard(GlyQIqResultHits, FilterResults.Filters.None);

                #region 2. bookkeeping

                if (GlyQIqResultHits.Count > 0)
                {
                    hitsWithinADataset++;
                    hitsWithinACode[j + 1]++;
                }

                #endregion

                #region 3b.  metadata interdataset results

                joiner[0] = interScansHitsCount;
                if (GlyQIqResultHits.Count > 0)
                {
                    //find most abundant scan
                    //List<GlyQIqResult> GlyQIqResultHitsByAbundance = (from n in GlyQIqResultHits orderby Convert.ToDouble(n.GlobalAggregateAbundance) descending select n).ToList();
                    List<GlyQIqResult> GlyQIqResultHitsByAbundance = GlyQIqResultHits.OrderByDescending(n => Convert.ToDouble(n.GlobalAggregateAbundance)).ToList();

                    //joiner[1] = GlyQIqResultHitsByAbundance[0].Scan; //payload!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    joiner[1] = GlyQIqResultHitsByAbundance[0].ElutionTimeObs; //payload!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                }
                else
                {
                    joiner[1] = falseResult;
                }
                interScansHitsCount = String.Join(seperatorString, joiner);

                #endregion end meta data
            }

            resultCollectionLines.Add(interScansHitsCount + "," + hitsWithinADataset);

            resultCollectionLines.Add(String.Join(seperatorString, hitsWithinACode));

            Assert.AreEqual(hitsWithinACode.Count, resultCollectionLines[1].Split(seperator).Count());

            return resultCollectionLines;
        }

        private void SetHeader(List<string> datasetWithCode, List<string> codesToPull)
        {
            string codeHeaderStart = "Codes";
            List<string> codeHeaderAsList = new List<string>();
            codeHeaderAsList.Add(codeHeaderStart);
            codeHeaderAsList.AddRange(codesToPull);
            codeHeaderAsList.Add("HitsCount");
            datasetWithCode.Add(String.Join(seperatorString, codeHeaderAsList.ToArray())); //add header
        }
    }
}
