using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GetPeaks_DLL.Objects;
using System.IO;
using GetPeaks_DLL.SQLite;
using PNNLOmics.Data.Features;
using GetPeaks_DLL.CompareContrast;
using PNNLOmics.Data;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.ConosleUtilities;
using AnalyzePeaks.DataFIFO;
using GetPeaks_DLL.AnalysisSupport;
using GetPeaks_DLL.AnalysisSupport.Tools;
using GetPeaks_DLL.Objects.ResultsObjects;


//Debug work:  "D:\PNNL CSharp\0_BatchFiles\0_WorkParameterAnalysisSTD.txt" "D:\Csharp\GetPeaksOutAnalysis" "D:\Csharp\VIPER Output"
//Debug work:  "D:\PNNL CSharp\0_BatchFiles\0_WorkParameterAnalysisSN09.txt" "D:\Csharp\GetPeaksOutAnalysis" "D:\Csharp\VIPER Output"
//Debug work:  "D:\PNNL CSharp\0_BatchFiles\0_WorkParameterAnalysisFeature.txt" "D:\Csharp\GetPeaksOutAnalysis" "D:\Csharp\VIPER Output"  //for loading a feature output
//Debug work:  "D:\PNNL CSharp\0_BatchFiles\0_WorkParameterAnalysisSTDCompare.txt" "D:\Csharp\GetPeaksOutAnalysis" "D:\Csharp\VIPER Output"

//Debug home:  "G:\PNNL Files\PNNL CSharp\0_BatchFiles\0_HomeParameterAnalysisSTD.txt" "G:\PNNL Files\CSharp\GetPeaksOutAnalysis" "G:\PNNL Files\CSharp\VIPER Output"
//Debug home:  "G:\PNNL Files\PNNL CSharp\0_BatchFiles\0_HomeParameterAnalysisSTDCompare.txt" "G:\PNNL Files\CSharp\GetPeaksOutAnalysis" "G:\PNNL Files\CSharp\VIPER Output"

namespace AnalyzePeaks
{
    class AnalysisStart
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Analyze Peaks");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            bool readKeyToggle = false;//toggles breakpoints in console

            Console.WriteLine("Hi " + args[0]);
            Console.WriteLine("Hi " + args[1]);
            Console.WriteLine("Hi " + args[2]);
            Console.ReadKey();

            #region args to values
            //setup main parameter file from args
            List<string> mainParameterFile = new List<string>();
            #region switch from server to desktop based on number of args
            if (args.Length == 0)//debugging
            {
                mainParameterFile.Add(""); mainParameterFile.Add(""); mainParameterFile.Add("");
            }
            else
            {
                Console.WriteLine("ParseArgs");
                ParseStrings parser = new ParseStrings();
                mainParameterFile = parser.Parse3Args(args); //three arguments are needed.  first one is the path to the parameter file
                //mainParameterFile = parser.Parse4Args(args); //three arguments are needed.  first one is the path to the parameter file
            }
            #endregion

            //override and load seperate parameter file here
            //mainParameterFile[0] = @"D:\PNNL CSharp\0_BatchFiles\0_WorkParameterAnalysisSN09.txt";
            //

            //load parameters
            List<string> stringListFromParameterFile = new List<string>();
            FileIteratorStringOnly loadParameter = new FileIteratorStringOnly();
            stringListFromParameterFile = loadParameter.loadStrings(mainParameterFile[0]);



            #region convert parameter file to variables

            if (stringListFromParameterFile.Count != 14)
            {
                Console.WriteLine("Parameter file is missing parameters");
                Console.Write("There are only " + stringListFromParameterFile.Count + " parameters");
                Console.ReadKey();
            }

            //TODO is this still needed!!!!!!!!!!!!!!!!!!!!!!!
            //Parameter files!!!!!!!!!!!!!!!!!!!! 8-11-11
            /// offset the arry by one to account for the version

            string version = stringListFromParameterFile[0];
            string LibraryPath = stringListFromParameterFile[1];
            string LibraryFileName = stringListFromParameterFile[2];
            string DataFilePath = stringListFromParameterFile[3];
            string DataFileName = stringListFromParameterFile[4];
            string DataFileIdentifier = stringListFromParameterFile[5];
            string DataFileSeperator = stringListFromParameterFile[6];
            string DataFileType = stringListFromParameterFile[7];
            string ViperResultsPath = stringListFromParameterFile[8];
            string folderToExportTextTo = stringListFromParameterFile[9];
            string writeViper = stringListFromParameterFile[10];
            string glycanAnalysis = stringListFromParameterFile[11];
            string isotopeAnalysis = stringListFromParameterFile[12];
            string isotopeResultsPath = stringListFromParameterFile[13];

            bool exportViper = ConvertStringToBool.Convert(writeViper);
            bool analyzeGlycans = ConvertStringToBool.Convert(glycanAnalysis);
            bool exportIsotopes = ConvertStringToBool.Convert(isotopeAnalysis);

            #endregion

            InputOutputFileName newLibrary = new InputOutputFileName();
            newLibrary.InputFileName = LibraryPath + LibraryFileName;
            newLibrary.OutputPath = LibraryPath;//not implemented yet

            CheckFileIsPresent.CheckFile(readKeyToggle, newLibrary.InputFileName, "Library File");

            List<string> filesToAnalyze = new List<string>();
            InputOutputFileName newSQLData = new InputOutputFileName();
            InputOutputFileName newFeatureData = new InputOutputFileName();

            switch(DataFileType)
            {
                case "SQLite":
                    //newSQLData = new InputOutputFileName();
                    newSQLData.InputSQLFileName = DataFilePath + DataFileName + DataFileIdentifier + @".db";
                    CheckFileIsPresent.CheckFile(readKeyToggle, newSQLData.InputSQLFileName, "SQL Data File");
                    break;
                case "Feature":
                    //newFeatureData = new InputOutputFileName();
                    newFeatureData.InputSQLFileName = DataFilePath + DataFileName + DataFileIdentifier + @".txt";
                    filesToAnalyze.Add(newFeatureData.InputSQLFileName);
                    CheckFileIsPresent.CheckFile(readKeyToggle, newFeatureData.InputSQLFileName, "Feature Data File");
                    break;
            }

            InputOutputFileName textFileOutput = new InputOutputFileName();
            textFileOutput.OutputFileName = folderToExportTextTo + "Results" + DataFileName + @".txt";
            CheckFileIsPresent.CheckFile(readKeyToggle, textFileOutput.OutputFileName, "Results Output File");
           

            InputOutputFileName ViperOutputFile = new InputOutputFileName();
            ViperOutputFile.OutputFileName = ViperResultsPath + DataFileName + DataFileSeperator + DataFileIdentifier + @".txt";
            CheckFileIsPresent.CheckFile(readKeyToggle, ViperOutputFile.OutputFileName, "Viper Output File");

            InputOutputFileName isotopeOutputFile = new InputOutputFileName();
            isotopeOutputFile.OutputFileName = isotopeResultsPath + DataFileName + DataFileSeperator + DataFileIdentifier + @".txt";
            CheckFileIsPresent.CheckFile(readKeyToggle, isotopeOutputFile.OutputFileName, "Isotope Output File");

            #endregion

            

            List<string> DataFileIdentifierList = new List<string>();

            //DataFileName = "QC_Shew_09_05_pt5_c_4Jan10_Griffin_09-11-18";
            //DataFileIdentifier = "Thrash";
            //DataFileIdentifier = "SHEWR";
            //DataFileIdentifier = "SHEW20208";
            //DataFileIdentifier = "SHEW20208T";

            #region SN09-SN23 off
            //filesOutputName.Add(folderToExportTo + "SN09" + @".txt");
            //filesOutputName.Add(folderToExportTo + "SN10" + @".txt");
            //filesOutputName.Add(folderToExportTo + "SN11" + @".txt");
            //filesOutputName.Add(folderToExportTo + "SN12_1" + @".txt");
            //filesOutputName.Add(folderToExportTo + "SN13" + @".txt");
            //filesOutputName.Add(folderToExportTo + "SN14" + @".txt");
            //filesOutputName.Add(folderToExportTo + "SN15" + @".txt");
            //filesOutputName.Add(folderToExportTo + "SN16" + @".txt");
            //filesOutputName.Add(folderToExportTo + "SN17" + @".txt");
            //filesOutputName.Add(folderToExportTo + "SN18" + @".txt");
            //filesOutputName.Add(folderToExportTo + "SN19_1" + @".txt");
            //filesOutputName.Add(folderToExportTo + "SN20" + @".txt");
            //filesOutputName.Add(folderToExportTo + "SN21" + @".txt");
            //filesOutputName.Add(folderToExportTo + "SN22" + @".txt");
            //filesOutputName.Add(folderToExportTo + "SN23" + @".txt");
            //filesOutputName.Add(folderToExportTo + "SN23" + @".txt");
            #endregion

            #region SN3SN10 off
            //filesToAnalyze.Add("FirstSQLiteBatchSN09_2");
            //filesToAnalyze.Add("FirstSQLiteBatchSN10_2");
            //filesToAnalyze.Add("FirstSQLiteBatchSN11");
            //filesToAnalyze.Add("FirstSQLiteBatchSN12");
            //filesToAnalyze.Add("FirstSQLiteBatchSN13");
            //filesToAnalyze.Add("FirstSQLiteBatchSN14_2");
            //filesToAnalyze.Add("FirstSQLiteBatchSN15");
            //filesToAnalyze.Add("FirstSQLiteBatchSN16");
            //filesToAnalyze.Add("FirstSQLiteBatchSN17");
            //filesToAnalyze.Add("FirstSQLiteBatchSN18");
            //filesToAnalyze.Add("FirstSQLiteBatchSN19");
            //filesToAnalyze.Add("FirstSQLiteBatchSN20");
            //filesToAnalyze.Add("FirstSQLiteBatchSN21");
            //filesToAnalyze.Add("FirstSQLiteBatchSN22");
            //filesToAnalyze.Add("FirstSQLiteBatchSN23_2");

            #endregion

            #region SN1SN3 off
            DataFileIdentifierList.Add("SN09");
            DataFileIdentifierList.Add("SN10");
            DataFileIdentifierList.Add("SN11");
            DataFileIdentifierList.Add("SN12_3");
            DataFileIdentifierList.Add("SN13");
            DataFileIdentifierList.Add("SN14");
            DataFileIdentifierList.Add("SN15");
            DataFileIdentifierList.Add("SN16");
            DataFileIdentifierList.Add("SN17");
            DataFileIdentifierList.Add("SN18");
            DataFileIdentifierList.Add("SN19_2");
            DataFileIdentifierList.Add("SN20");
            DataFileIdentifierList.Add("SN21");
            DataFileIdentifierList.Add("SN22");
            DataFileIdentifierList.Add("SN23");
            #endregion

           
            //DataFileIdentifierList.Add("SHEWR");
            //DataFileIdentifierList.Add("SHEWR");
            //DataFileIdentifierList.Add("SHEW20208");
            //DataFileIdentifierList.Add("SHEW20208T");

            #region SN1SN3 off
            //DataFileIdentifierList.Add("209539");
            //DataFileIdentifierList.Add("209616");
            //DataFileIdentifierList.Add("209617");
            //DataFileIdentifierList.Add("209955");
            //DataFileIdentifierList.Add("209542");
            //DataFileIdentifierList.Add("210111");
            //DataFileIdentifierList.Add("209537");
            //DataFileIdentifierList.Add("209538");
            //DataFileIdentifierList.Add("209540");
            //DataFileIdentifierList.Add("209541");
            //DataFileIdentifierList.Add("209541");
            #endregion

            List<SampleResutlsObject> resultsCollection = new List<SampleResutlsObject>();
            AppendToLibrary newAppender = new AppendToLibrary();

            bool batchmode = false;

            if (!batchmode)
            {
                DataFileIdentifierList.Clear();//only allow one loop
                DataFileIdentifierList.Add("placeholder");//so we can enter theloop
            }

            for (int i = 0; i < DataFileIdentifierList.Count; i++)
            {
                #region inside
                string filepath = newSQLData.InputSQLFileName;
                //if (batchmode)
                //{
                    switch (DataFileType)
                    {
                        case "SQLite":
                            //filepath = DataFilePath + DataFileName + DataFileIdentifierList[i] + @".db";//TODO good code
                            filepath = DataFilePath + DataFileName + @"SYN.db";//depug
                            break;
                        case "Feature":
                            filepath = newFeatureData.InputSQLFileName;
                            break;
                    }
                //}

                InputOutputFileName newFile = new InputOutputFileName();
                newFile.InputSQLFileName = filepath;
                CheckFileIsPresent.CheckFile(readKeyToggle, newFile.InputSQLFileName, "Final Target File");

                //prep objects so they can be used in the reader and writer
                List<ElutingPeakLite> elutingPeakList = new List<ElutingPeakLite>();
                List<FeatureLight> featureLitelist = new List<FeatureLight>();
                List<IsotopeObject> isotopeList = new List<IsotopeObject>();
                GetDataController dataLoad = new GetDataController();

                switch (DataFileType)
                {
                    case "SQLite":
                        {
                            #region getData
                            elutingPeakList = dataLoad.GetDataElutingPeak(newFile);
                            featureLitelist = dataLoad.GetDataFeatures(newFile);
                            List<DataSet> newLibraryDataset = dataLoad.GetDataLibrary(newLibrary);
                            isotopeList = dataLoad.GetIsotopes(newFile);

                            

                            if (analyzeGlycans)
                            {
                                bool convertTOAmino = false;
                                double massTollerance = 15;
                                SampleResutlsObject dataSummary = newAppender.AppendPrep(filepath, ref newLibraryDataset, ref featureLitelist, ref elutingPeakList, ref isotopeList, convertTOAmino, massTollerance);

                                resultsCollection.Add(dataSummary);
                            }

                            if (1 == 1) //simplecompare
                            {
                                bool convertTOAmino = false;
                                double massTollerance = 0.0001;

                                SampleResutlsObject dataSummary = newAppender.AppendPrep(ref newLibraryDataset, ref newLibraryDataset, convertTOAmino, massTollerance);

                                resultsCollection.Add(dataSummary);
                            }
                            #endregion
                        }
                        break;
                    case "Feature":
                        {
                            List<DataSet> newLibraryDataset = dataLoad.GetDataLibrary(newLibrary);

                            //TODO load in feature file 
                            //TODO convert to featureList
                            //TODO create different overload for AppendPrep or zero out the tagalongs
                            if (analyzeGlycans)
                            {
                                bool convertTOAmino = false;
                                double massTollerance = 15;
                                SampleResutlsObject dataSummary = newAppender.AppendPrep(filepath, ref newLibraryDataset, ref featureLitelist, ref elutingPeakList, ref isotopeList, convertTOAmino, massTollerance);

                                resultsCollection.Add(dataSummary);
                            }
                        }
                        break;
                }

                #region write data
                switch (DataFileType)
                {
                    case "SQLite":
                        { //output viper compatible text files (partial isos, features, and maps)
                            if (exportViper)
                            {
                                #region write VIPIR data
                                if (batchmode)
                                {
                                    ViperOutputFile.OutputFileName = ViperResultsPath + DataFileName + DataFileSeperator + DataFileIdentifierList[i] + @".txt";
                                }

                                DataPeaksToViper newWriter = new DataPeaksToViper();
                                newWriter.toDiskVIPEROutput(elutingPeakList, featureLitelist, ViperOutputFile.OutputFileName);
                                #endregion
                            }

                            //export isotope distributions to text files
                            if (exportIsotopes)
                            {
                                DataIsotopesToText newIsotopeWriter = new DataIsotopesToText();
                                newIsotopeWriter.toDiskIsotopeOutput(isotopeList, isotopeOutputFile.OutputFileName);
                            }
                        }
                        break;
                    case "Feature":
                        {

                        }
                        break;
                }
                //ResultsWriter newWriter = new ResultsWriter();
                //newWriter.toDiskSingle(dataSummary, newFile, newLibrary);
                #endregion
                #endregion
            }

            bool appendData = true;
            if (appendData)
            {
                newAppender.AppendToTable(resultsCollection, folderToExportTextTo + DataFileName + DataFileSeperator + DataFileIdentifier);
            }
            stopWatch.Stop();
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to find and assign features in eluting peaks");
            Console.Write("Finished.  Press Return to Exit"); Console.ReadKey();
        }

        //private static bool ConvertStringToBool(string incommingString)
        //{
        //    bool tempBool = false;
        //    switch (incommingString)
        //    {
        //        case "TRUE":
        //            tempBool = true;
        //            break;
        //        case "FALSE":
        //            tempBool = false;
        //            break;
        //        default:
        //            Console.WriteLine("Select TRUE or FALSE in the input file");
        //            Console.ReadKey();
        //            break;
        //    }
        //    return tempBool;
        //}

        //private static void CheckFile(bool readKeyToggle, string name, string PrintType)
        //{
        //    FileInfo fiSQL = new FileInfo(name);
        //    bool existsSQL = fiSQL.Exists;

        //    Console.WriteLine(PrintType+ ": "+ name + " Correct?  Is Present: " + existsSQL + Environment.NewLine);
        //    if (readKeyToggle)
        //    {
        //        Console.ReadKey();
        //    }
        //}

        #region private functions
        /// <summary>
        /// set the input and output path
        /// </summary>
        /// <param name="target">target file</param>
        /// <returns>an IO object with the input and output path</returns>
        private static InputOutputFileName findFileInformation(FileTarget target)
        {
            
            InputOutputFileName newFile = new InputOutputFileName();

            switch (target)
            {
                #region switch
                case FileTarget.WorkStandardTest://work
                    {
                        newFile.InputSQLFileName = @"D:\Csharp\GetPeaksOutput\FirstSQLite.db";
                        newFile.OutputFileName = @"D:\Csharp\GetPeaksOutput\AnalysisResults.txt";
                    }
                    break;
                case FileTarget.HomeStandardTest:
                    {
                        newFile.InputSQLFileName = @"G:\PNNL Files\CSharp\GetPeaksOutput\Final Results\FirstSQLiteBatchSN09_1.db";
                        newFile.OutputFileName = @"D:\Csharp\GetPeaksOutput\AnalysisResults.txt";
                    }
                    break;
                case FileTarget.ServerStandardTest:
                    {
                        newFile.InputSQLFileName = @"e:\ScottK\GetPeaksOutput\FirstSQLite.db";
                        newFile.OutputFileName = @"D:\Csharp\GetPeaksOutput\AnalysisResults.txt";
                    }
                    break;
                case FileTarget.WorkRealData:
                    {
                        string ID = "SN17";
                        //newFile.InputSQLFileName = @"D:\Csharp\GetPeaksOutput\Final Results\FirstSQLiteBatch" + ID + @"_2.db";
                        //newFile.InputSQLFileName = @"D:\Csharp\GetPeaksOutput\Final Results\FirstSQLiteBatchSN09_2.db";
                        //newFile.InputSQLFileName = @"D:\Csharp\GetPeaksOutput\Final Results\FirstSQLiteBatchSN10_2.db";
                        //newFile.InputSQLFileName = @"D:\Csharp\GetPeaksOutput\Final Results\FirstSQLiteBatchSN14_2.db";
                        //newFile.InputSQLFileName = @"D:\Csharp\GetPeaksOutput\Final Results\FirstSQLiteBatchSN23_2.db";


                        newFile.InputSQLFileName = @"D:\Csharp\GetPeaksOutput\Final Results\FirstSQLiteBatch" + ID + @".db";
                        //newFile.OutputFileName = @"D:\Csharp\GetPeaksOutput\Final Results\AnalysisResultsSN09.txt";
                        newFile.OutputFileName = @"D:\Csharp\GetPeaksOutput\Final Results\AnalysisResults" + ID + @".txt";
                    }
                    break;
                case FileTarget.Server1://pub60 copy 1
                    {
                        newFile.InputSQLFileName = @"e:\ScottK\GetPeaksOutput\FirstSQLite1.db";
                        newFile.OutputFileName = @"D:\Csharp\GetPeaksOutput\AnalysisResults.txt";
                    }
                    break;
                case FileTarget.Server2://pub60 copy 2
                    {
                        newFile.InputSQLFileName = @"e:\ScottK\GetPeaksOutput\FirstSQLite2.db";
                        newFile.OutputFileName = @"D:\Csharp\GetPeaksOutput\AnalysisResults.txt";
                    }
                    break;
                case FileTarget.Server3://pub60 copy 3
                    {
                        newFile.InputSQLFileName = @"e:\ScottK\GetPeaksOutput\FirstSQLite3.db";
                        newFile.OutputFileName = @"D:\Csharp\GetPeaksOutput\AnalysisResults.txt";
                    }
                    break;
                case FileTarget.Server4://pub60 copy 4
                    {
                        newFile.InputSQLFileName = @"e:\ScottK\GetPeaksOutput\FirstSQLite4.db";
                        newFile.OutputFileName = @"D:\Csharp\GetPeaksOutput\AnalysisResults.txt";
                    }
                    break;
                default:
                    {
                        newFile.InputSQLFileName = @"d:\Csharp\GetPeaksOutput\FirstSQLite.db";
                        newFile.OutputFileName = @"D:\Csharp\GetPeaksOutput\AnalysisResults.txt";
                    }
                    break;
                #endregion
            }
            return newFile;
        }

        /// <summary>
        /// Various environments for working
        /// </summary>
        private enum FileTarget
        {
            WorkStandardTest,
            HomeStandardTest,
            ServerStandardTest,
            WorkRealData,
            Server1,
            Server2,
            Server3,
            Server4
        }

        #endregion
    }


}
