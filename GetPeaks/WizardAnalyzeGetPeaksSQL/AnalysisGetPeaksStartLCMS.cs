using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GetPeaks_DLL.ConosleUtilities;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.AnalysisSupport.Tools;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.AnalysisSupport;
using PNNLOmics.Data.Features;
using GetPeaks_DLL.Objects.ParameterObjects;
using GetPeaks_DLL.Objects.ResultsObjects;

//Debug home:  "G:\PNNL Files\PNNL CSharp\0_BatchFiles\0_HomeWizardAnalysisGetPeaksSQL.txt" "G:\PNNL Files\CSharp\GetPeaksOutAnalysis" "G:\PNNL Files\CSharp\VIPER Output"
//Debug work:  "D:\PNNL CSharp\0_BatchFiles\0_WorkWizardAnalysisGetPeaksSQL.txt" "D:\Csharp\GetPeaksOutAnalysis" "D:\Csharp\VIPER Output"
//Debug work:  "D:\PNNL CSharp\0_BatchFiles\0_WorkWizardAnalysisGetPeaksSQLSN09.txt" "D:\Csharp\GetPeaksOutAnalysis" "D:\Csharp\VIPER Output"
//Debug work:  "D:\PNNL CSharp\0_BatchFiles\0_WorkWizardAnalysisGetPeaksSQLSN36.txt" "D:\Csharp\GetPeaksOutAnalysis" "D:\Csharp\VIPER Output" "SN36_75_4uL_S3"
//"D:\PNNL CSharp\0_BatchFiles\0_WorkWizardAnalysisGetPeaksSQLSN09.txt" "D:\Csharp\GetPeaksOutAnalysis" "D:\Csharp\VIPER Output" "myName"
//
//TODO there is a bug where the number of eluting peaks and features do not equal.  may be caused by locked database!

namespace WizardAnalyzeGetPeaksSQL
{
    class AnalysisGetPeaksStartLCMS
    {
        static void Main(string[] args)
        {
            Console.WriteLine(" ");
            Console.WriteLine("Analyze Peaks");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            bool readKeyToggle = false;//toggles breakpoints in console

            #region args to values
            List<string> mainParameterFile;
            ConvertArgsToValues argsConverter = new ConvertArgsToValues();
            argsConverter.ArgsTo4Values(args, readKeyToggle, out mainParameterFile);  
            
            #endregion

            string fileIdentifier = mainParameterFile[3];
            //load parameters
            List<string> stringListFromParameterFile = new List<string>();
            FileIteratorStringOnly loadParameter = new FileIteratorStringOnly();
            stringListFromParameterFile = loadParameter.loadStrings(mainParameterFile[0]);

            #region convert parameter file to variables
            WizardAnalyzeGetPeaksSQLParameters analysisParameters = new WizardAnalyzeGetPeaksSQLParameters(stringListFromParameterFile, fileIdentifier);

            bool exportViper = analysisParameters.ShouldWeExportViperData;
            bool analyzeGlycans = analysisParameters.ShouldWeAnalyzeGlycans;
            bool exportIsotopes = analysisParameters.ShouldWeExportIsotopes;

            InputOutputFileName newLibrary = new InputOutputFileName();
            newLibrary.InputFileName = analysisParameters.FullLibraryFile;
            CheckFileIsPresent.CheckFile(readKeyToggle, newLibrary.InputFileName, "Library File");

            List<string> filesToAnalyze = new List<string>();
            
            InputOutputFileName newSQLData = new InputOutputFileName();
            newSQLData.InputSQLFileName = analysisParameters.FullSQLDatabaseFile;
           
            InputOutputFileName textFileOutput = new InputOutputFileName();
            textFileOutput.OutputFileName = analysisParameters.FullResultsTextFileOutputFile;
            
            InputOutputFileName ViperOutputFile = new InputOutputFileName();
            ViperOutputFile.OutputFileName = analysisParameters.FullViperTextResultsFile;

            InputOutputFileName isotopeOutputFile = new InputOutputFileName();
            isotopeOutputFile.OutputFileName = analysisParameters.FullIsotopeTextResultsFile;
            
            CheckFileIsPresent.CheckFile(readKeyToggle, newSQLData.InputSQLFileName, "SQL Data File");
            CheckFileIsPresent.CheckFile(readKeyToggle, textFileOutput.OutputFileName, "Results Output File");
            CheckFileIsPresent.CheckFile(readKeyToggle, ViperOutputFile.OutputFileName, "Viper Output File");
            CheckFileIsPresent.CheckFile(readKeyToggle, isotopeOutputFile.OutputFileName, "Isotope Output File");

            #endregion


            bool batchmode = false;
            List<string> DataFileIdentifierList = new List<string>();
            List<string> ViperOutputFileIdentifierList = new List<string>();
            List<string> IsotopeOutputFileIdentifierList = new List<string>();

            switch (batchmode)
            {
                #region inside
                case true:
                    {
                        //add additional files here
                        List<string> identifierList = new List<string>();
                        identifierList.Add("SYN");

                        string baseName = analysisParameters.JustTheSQLDatabaseFileBaseName;

                        foreach (string identifier in identifierList)
                        {
                            DataFileIdentifierList.Add(         analysisParameters.JustTheSQLDatabasePath +
                                                                baseName +
                                                                identifier + @".db");
                            ViperOutputFileIdentifierList.Add(  analysisParameters.JustTheViperOutputFolderPath +
                                                                baseName +
                                                                identifier + @".txt");
                            IsotopeOutputFileIdentifierList.Add(analysisParameters.JustTheIsotopeOutputFolderPath +
                                                                baseName + 
                                                                identifier + @"_Iso.txt");
                        }
                    }
                    break;
                case false:
                    {
                        DataFileIdentifierList.Add(analysisParameters.FullSQLDatabaseFile);
                        ViperOutputFileIdentifierList.Add(analysisParameters.FullViperTextResultsFile);
                        IsotopeOutputFileIdentifierList.Add(analysisParameters.FullIsotopeTextResultsFile);

                    }
                    break;
                default:
                    {
                        DataFileIdentifierList.Add(analysisParameters.FullSQLDatabaseFile);
                        ViperOutputFileIdentifierList.Add(analysisParameters.FullViperTextResultsFile);
                        IsotopeOutputFileIdentifierList.Add(analysisParameters.FullIsotopeTextResultsFile);
                    }
                    break;
                #endregion
            }


            List<SampleResutlsObject> resultsCollection = new List<SampleResutlsObject>();
            AppendToLibrary newAppender = new AppendToLibrary();

            for (int i = 0; i < DataFileIdentifierList.Count; i++)
            {
                string filepath = DataFileIdentifierList[i];

                        //filepath = DataFilePath + DataFileName + DataFileIdentifierList[i] + @".db";//TODO good code
                //filepath = DataFilePath + DataFileName + @"SYN.db";//depug
                //        break;

                InputOutputFileName newFile = new InputOutputFileName();
                newFile.InputSQLFileName = filepath;
                CheckFileIsPresent.CheckFile(readKeyToggle, newFile.InputSQLFileName, "Final Target File");

                //prep objects so they can be used in the reader and writer
                List<ElutingPeakLite> elutingPeakList = new List<ElutingPeakLite>();
                List<FeatureLight> featureLitelist = new List<FeatureLight>();
                List<IsotopeObject> isotopeList = new List<IsotopeObject>();
                GetDataController dataLoad = new GetDataController();

                #region getData
                elutingPeakList = dataLoad.GetDataElutingPeak(newFile);
                featureLitelist = dataLoad.GetDataFeatures(newFile);
                List<DataSet> newLibraryDataset = dataLoad.GetDataLibrary(newLibrary);
                isotopeList = dataLoad.GetIsotopes(newFile);

                #endregion

                #region AnalyzeGlycans
                if (analyzeGlycans)
                {
                    bool convertTOAmino = false;
                    double massTollerance = 15;
                    SampleResutlsObject dataSummary = newAppender.AppendPrep(filepath, ref newLibraryDataset, ref featureLitelist, ref elutingPeakList, ref isotopeList, convertTOAmino, massTollerance);

                    resultsCollection.Add(dataSummary);
                }

                
                #endregion
               
                #region write data
                
                { //output viper compatible text files (partial isos, features, and maps)
                    if (exportViper)
                    {
                        #region write VIPIR data
                        
                        ViperOutputFile.OutputFileName = ViperOutputFileIdentifierList[i];
                        
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

                #endregion                   
            }

            bool appendData = true;
            if (appendData)
            {
                string appendFileName = analysisParameters.JustTheFolderToExportTo + "WizardGetPeaksResults_" + analysisParameters.JustTheIdentifier;
                //newAppender.AppendToTable(resultsCollection, folderToExportTextTo + DataFileName + DataFileSeperator + DataFileIdentifier);
                newAppender.AppendToTable(resultsCollection, appendFileName);
            }
            stopWatch.Stop();
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to find and assign features in eluting peaks");
            Console.WriteLine("The file " + newSQLData.InputSQLFileName + " has been written");
            Console.Write("Finished.  Press Return to Exit"); Console.ReadKey();
         }
    }
}
    
