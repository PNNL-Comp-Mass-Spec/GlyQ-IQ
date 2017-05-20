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
    public class DiabetesTests
    {
        public static string workingDirectory = @"D:\PNNL\Projects\DATA DIABETES PIC 2014-03-27\Results";
        public static string currentworkingDirectory = Path.Combine(workingDirectory, "GlyQIQResults", "Stages", "3_AlignedData_LocatedInElutionTimeTheor", "ManuallyModified");
        //public static string outputDirectory = Path.Combine(workingDirectory, "Output");
        public static string outputDirectory = @"X:\Output";

        string dictionaryPath = Path.Combine(workingDirectory, "GlyQIQResults", "Stages", "4_aMakeDictionartMapFileByHand","L_13_Alditol_LactoneComboCodeToMass.txt");

        string[] joiner = new string[2];

        public static char seperator = ',';
        public static string seperatorString = seperator.ToString(CultureInfo.InvariantCulture);
        string falseResult = "0";//place holder

        public double maxNET = 120;//min

        StringListToDisk writer = new StringListToDisk();



        [Test]
        //4.  this is used to extracte the desired glycans and write out data for the Glycolzyer
        public void ExtractAlignedGlycans()
        {
            //1.  load in datafile names and short names

            UtilitiesForGet.DataSetGroup currentGroup = UtilitiesForGet.DataSetGroup.Control6;

            List<string> currentFileNamesShort;
            List<string> currentFileNames;
            UtilitiesForGet.SetDataInput(currentGroup, out currentFileNamesShort, out currentFileNames);

           
            for(int i=0;i<currentFileNames.Count;i++)
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

  //          List<string> codesToPull = GetCodes.HighMannose();//B.  this is a good and contains some missing data filled in
            List<string> codesToPull = GetCodes.Best45();//B.  this is a good and contains some missing data filled in
            //List<string> codesToPull = GetCodes.Best25();//B.  no missing data.  all present
            //List<string> codesToPull = GetCodes.Best105();//B.  this is a good missing data list where 50% of ions are present

            //4.  take headers from first dataset loaded
            string headers = datasetPile[0].headerToResults;

            //5.  what we will write to disk! *(1/8)
            List<Dataset> datasetToWrite = new List<Dataset>();//this is the main dataoutput for a given set of targets

            List<string> datasetWithCode = new List<string>();//this tells us which datasets had hits

            List<string> datasetWithCodeMaxScan = new List<string>();//this tells us which datasets had hits

            List<string> datasetWithCodeMaxnet = new List<string>();//this tells us which datasets had hits

            List<string> datasetWithCodeIntensityH = new List<string>();//this tells us which datasets had hits

            List<string> datasetWithCodeIntensityD = new List<string>();//this tells us which datasets had hits



            //6. set column header *(2/8)
            UtilitiesForGet.SetHeader(datasetWithCode, codesToPull, seperatorString);
            UtilitiesForGet.SetHeader(datasetWithCodeMaxScan, codesToPull, seperatorString);
            UtilitiesForGet.SetHeader(datasetWithCodeMaxnet, codesToPull, seperatorString);
            UtilitiesForGet.SetHeader(datasetWithCodeIntensityH, codesToPull, seperatorString);
            UtilitiesForGet.SetHeader(datasetWithCodeIntensityD, codesToPull, seperatorString);

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
                    GlyQIqResultHits = FilterResults.Standard(GlyQIqResultHits, FilterResults.Filters.None);

                    ///Actual work *(4/8 below)

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

                    var hitType = FilterResults.TypeOfHitsToSelect.MostAbundantMsAbundance;

                    #region 3b.  metadata interdataset results (Scan)

                    joiner[0] = interScansHitsCount;
                    if (GlyQIqResultHits.Count > 0)
                    {
                        //find most abundant scan
                        //List<GlyQIqResult> GlyQIqResultHitsByAbundance = (from n in GlyQIqResultHits orderby Convert.ToDouble(n.GlobalAggregateAbundance) descending select n).ToList();
                        
                        
                        //List<GlyQIqResult> glyQIqResultHitsByAbundance = GlyQIqResultHits.OrderByDescending(n => Convert.ToDouble(n.GlobalAggregateAbundance)).ToList();

                        //List<GlyQIqResult> glyQIqResultHitsByAbundance = FilterResults.Standard(GlyQIqResultHits, FilterResults.Filters.MostAbundant);


                        List<GlyQIqResult> glyQIqResultHitsByAbundance = SelectHitIsomer(GlyQIqResultHits, hitType);


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
                        List<GlyQIqResult> glyQIqResultHitsByAbundance = SelectHitIsomer(GlyQIqResultHits, hitType);

                        joiner[1] = glyQIqResultHitsByAbundance[0].ElutionTimeTheor;//payload.  works only for aligned data
                    }
                    else
                    {
                        joiner[1] = falseResult;

                    }
                    internetHitsCount = String.Join(seperatorString, joiner);

                    #endregion end meta data

                    double labelingEfficiency = 0.99;

                    #region 3c.  metadata interdataset results Intensity (H)

                    joiner[0] = intensityHCount;
                    if (GlyQIqResultHits.Count > 0)
                    {
                        //find most abundant scan
                        List<GlyQIqResult> glyQIqResultHitsByAbundance = SelectHitIsomer(GlyQIqResultHits, hitType);


                        double baseAbundanceH = Convert.ToDouble(glyQIqResultHitsByAbundance[0].Abundance);

                        joiner[1] = Convert.ToString(baseAbundanceH *labelingEfficiency);//payload
                    }
                    else
                    {
                        joiner[1] = falseResult;

                    }
                    intensityHCount = String.Join(seperatorString, joiner);

                    #endregion end meta data

                    #region 3c.  metadata interdataset results Intensity (D)

                    joiner[0] = intensityDCount;
                    if (GlyQIqResultHits.Count > 0)
                    {
                        //find most abundant scan
                        List<GlyQIqResult> glyQIqResultHitsByAbundance = SelectHitIsomer(GlyQIqResultHits, hitType);

                        
                        double baseAbundanceH = Convert.ToDouble(glyQIqResultHitsByAbundance[0].Abundance);
                        double dhRatio = Convert.ToDouble(glyQIqResultHitsByAbundance[0].DHRatio);
                        double calculatedAbundanceD = baseAbundanceH * dhRatio * (1+ (1-labelingEfficiency));
                        joiner[1] = Convert.ToString(calculatedAbundanceD);//payload
                    }
                    else
                    {
                        joiner[1] = falseResult;

                    }
                    intensityDCount = String.Join(seperatorString, joiner);

                    #endregion end meta data
                }



                //Add data to pile *(5/8)

                datasetToWrite.Add(datasetOut);
                datasetWithCode.Add(interDatasetHitsCount + "," + hitsWithinADataset);
                datasetWithCodeMaxScan.Add(interScansHitsCount + "," + hitsWithinADataset);
                datasetWithCodeMaxnet.Add(internetHitsCount + "," + hitsWithinADataset);
                datasetWithCodeIntensityH.Add(intensityHCount + "," + hitsWithinADataset);
                datasetWithCodeIntensityD.Add(intensityDCount + "," + hitsWithinADataset);
            }

            //Add data to pile *(6/8)
            datasetWithCode.Add(String.Join(seperatorString, hitsWithinACode));
            datasetWithCodeMaxScan.Add(String.Join(seperatorString, hitsWithinACode));
            datasetWithCodeMaxnet.Add(String.Join(seperatorString, hitsWithinACode));
            datasetWithCodeIntensityH.Add(String.Join(seperatorString, hitsWithinACode));
            datasetWithCodeIntensityD.Add(String.Join(seperatorString, hitsWithinACode));
            //end of loop




            //set paths *(7/8)
            string matrixToWritePath = Path.Combine(outputDirectory, "Matrix_IsoFit.txt");
            string interDatasetPath = Path.Combine(outputDirectory, "InterDatasetCount.csv");
            string interDatasetPathScan = Path.Combine(outputDirectory, "InterScanCount.csv");
            string interDatasetPathNet = Path.Combine(outputDirectory, "InterNetCount.csv");
            string interDatasetPathH = Path.Combine(outputDirectory, "InterNetAbundanceH.csv");
            string interDatasetPathD = Path.Combine(outputDirectory, "InterNetAbundanceD.csv");
            string interDatasetNormalizationValuesPath = Path.Combine(outputDirectory, "IntensityNormalization.csv");
            
            //make new paths
            string directoryH = Path.Combine(outputDirectory, "Igor", "Patient");
            if(Directory.Exists(directoryH))
            {
                Directory.Delete(directoryH,true);
            }    
            Directory.CreateDirectory(directoryH);

            string directoryD = Path.Combine(outputDirectory, "Igor", "Control");
            if (Directory.Exists(directoryD))
            {
                Directory.Delete(directoryD, true);
            } 
            Directory.CreateDirectory(directoryD);

            string spectraHbasePath = Path.Combine(outputDirectory,"Igor","Patient", "IgorH_");
            string spectraDbasePath = Path.Combine(outputDirectory, "Igor", "Control", "IgorD_");


            //transpose and write *(8/8)  This is non normalizedData

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

            List<string> transposedListH = TransposeLines.SwapRowsAndColumns(datasetWithCodeIntensityH, seperatorString);
            writer.toDiskStringList(interDatasetPathH, transposedListH);

            List<string> transposedListD = TransposeLines.SwapRowsAndColumns(datasetWithCodeIntensityD, seperatorString);
            writer.toDiskStringList(interDatasetPathD, transposedListD);


            bool normalizeSpectra = true;
            bool takeLog = true;
            //double intensityMultiplier = 1000000;
            double intensityMultiplier = 1;
            List<double> normalizationArrayByDataset = new List<double>();
            normalizationArrayByDataset = Enumerable.Repeat(-1.0, datasetPile.Count).ToList();

            bool writeIgorSpectra = true;
            if (writeIgorSpectra)
            {
                //load codesToMass Dictionarty
                
                Dictionary<string, double> codeToMassDictionary = LoadCodeToMassDictionary(dictionaryPath);

                WriteSpectra(transposedListH, spectraHbasePath, codeToMassDictionary, datasetPile, normalizeSpectra, ref normalizationArrayByDataset, intensityMultiplier, takeLog);

                WriteSpectra(transposedListD, spectraDbasePath, codeToMassDictionary, datasetPile, normalizeSpectra, ref normalizationArrayByDataset, intensityMultiplier, takeLog);
            }


            //write out normalization factors
            List<string> normalizationConstantsByString = normalizationArrayByDataset.ConvertAll(x => x.ToString());
            List<string>normalizedToWrite = new List<string>();
            for (int i = 0; i < datasetToWrite.Count; i++)
            {
                normalizedToWrite.Add(currentFileNamesShort[i] + seperator + normalizationConstantsByString[i]);
            }
            List<string> transposedListNormalization = TransposeLines.SwapRowsAndColumns(normalizedToWrite, seperatorString);

            writer.toDiskStringList(interDatasetNormalizationValuesPath, transposedListNormalization);
        }

        private static List<GlyQIqResult> SelectHitIsomer(List<GlyQIqResult> GlyQIqResultHits, FilterResults.TypeOfHitsToSelect selectType)
        {
            List<GlyQIqResult> glyQIqResultHitsByAbundance = new List<GlyQIqResult>();

            switch (selectType)
            {
                case FilterResults.TypeOfHitsToSelect.MostAbundantAgregateAbundance:
                    {
                        glyQIqResultHitsByAbundance = GlyQIqResultHits.OrderByDescending(n => Convert.ToDouble(n.GlobalAggregateAbundance)).ToList();
                    }
                    break;
                         case FilterResults.TypeOfHitsToSelect.MostAbundantMsAbundance:
                    {
                        glyQIqResultHitsByAbundance = GlyQIqResultHits.OrderByDescending(n => Convert.ToDouble(n.Abundance)).ToList();
                    }
                    break;
                     case FilterResults.TypeOfHitsToSelect.First:
                    {
                        GlyQIqResult tempHit = GlyQIqResultHits.FirstOrDefault();
                        glyQIqResultHitsByAbundance = new List<GlyQIqResult>();
                        glyQIqResultHitsByAbundance.Add(tempHit);
                    }
                    break;
            }

            return glyQIqResultHitsByAbundance;
        }


        private static Dictionary<string, double> LoadCodeToMassDictionary(string pathToDirectory)
        {
            StringLoadTextFileLine reader = new StringLoadTextFileLine();

            List<string> linesOfCodeToMass = reader.SingleFileByLine(pathToDirectory);

            Dictionary<string, double> codeToMassDictionary = new Dictionary<string, double>();

            bool firsLine = true;
            foreach (var line in linesOfCodeToMass)
            {
                if (firsLine == false)
                {
                    string[] words = line.Split('\t');
                    codeToMassDictionary.Add(words[0], Convert.ToDouble(words[1]));
                }
                else
                {
                    firsLine = false;
                }
            }
            return codeToMassDictionary;
        }

        private void WriteSpectra(List<string> transposedListH, string spectraHbasePath, Dictionary<string, double> codeToMassDictionary, List<Dataset> datasetPile, bool normalizeSpectre, ref List<double> normalizationArrayByDataset, double intensityMultiplier, bool takeLog)
        {
            
            for (int i = 0; i < datasetPile.Count; i++)
            {
                double normaliztaion = 0;

                //calculate sum for normalization
                bool firsLine2 = true;
                for (int j = 0; j < transposedListH.Count - 1; j++)
                {
                    if (firsLine2 == false)
                    {
                        string line = transposedListH[j];
                        string[] words = line.Split(seperator);

                        double intensity = Convert.ToDouble(words[i + 1]);

                        if (takeLog)
                        {
                            intensity = Math.Log10(intensity);
                        }

                        normaliztaion += intensity;
                    }
                    else
                    {
                        firsLine2 = false;
                    }
                }

                if (normalizationArrayByDataset[i] < 0)//less than 0 has not been calculated yet
                {
                    normalizationArrayByDataset[i] = normaliztaion;
                }
                

                List<string> spectra = new List<string>();
                bool firsLine3 = true;
                for (int j = 0; j < transposedListH.Count - 1; j++)
                {
                    
                    if (firsLine3 == false)
                    {
                        string line = transposedListH[j];
                        string[] words = line.Split(seperator);

                        string currentCode = words[0];
                        double mass = 0;
                        if (codeToMassDictionary.ContainsKey(currentCode))
                        {
                            mass = codeToMassDictionary[currentCode];
                        }
                        double intensity = Convert.ToDouble(words[i + 1]);

                        if (takeLog)
                        {
                            intensity = Math.Log10(intensity);
                        }

                        if (normalizeSpectre)
                        {
                            intensity = intensity/normaliztaion * intensityMultiplier;
                        }

                        //if (takeLog)
                        //{
                        //    intensity = Math.Log10(intensity);
                        //}

                        StringBuilder db = new StringBuilder();
                        db.Append(mass);
                        db.Append(",");
                        db.Append(intensity.ToString());
                        string point = db.ToString();
                        spectra.Add(point);
                    }
                    else
                    {
                        firsLine3 = false;
                    }
                }

                string id = "";
                if(i<100)
                {
                    id += "0";
                    if(i<10)
                    {
                        id += "0";
                    }
                }
                id += i.ToString();
                writer.toDiskStringList(spectraHbasePath + id + " .txt", spectra);
            }
        }

       

        
    }

}
