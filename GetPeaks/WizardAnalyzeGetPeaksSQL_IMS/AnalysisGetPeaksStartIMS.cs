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
using PNNLOmics.Data;
using GetPeaks_DLL.Objects.ResultsObjects;

//Debug work:  "D:\PNNL CSharp\0_BatchFiles\0_WorkWizardAnalysisGetPeaksSQL_IMS_SN09.txt" "D:\Csharp\GetPeaksOutAnalysis" "D:\Csharp\VIPER Output" "SQLite"
//Debug work feature:  "D:\PNNL CSharp\0_BatchFiles\0_WorkWizardAnalysisGetPeaksFeature_IMS_SN52_1000.txt" "D:\Csharp\GetPeaksOutAnalysis" "D:\Csharp\VIPER Output" "Feature"
//Debug work feature:  "D:\PNNL CSharp\0_BatchFiles\0_WorkWizardAnalysisGetPeaksFeature_IMS_SN52_2000.txt" "D:\Csharp\GetPeaksOutAnalysis" "D:\Csharp\VIPER Output" "Feature"


namespace WizardAnalyzeGetPeaksSQL_IMS
{
    class AnalysisGetPeaksStartIMS
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Analyze Peaks");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            bool readKeyToggle = false;//toggles breakpoints in console

            #region args to values
            List<string> mainParameterFile;
            ConvertArgsToValues argsConverter = new ConvertArgsToValues();
            argsConverter.ArgsTo4Values(args, readKeyToggle, out mainParameterFile);  
            #endregion

            //load parameters
            List<string> stringListFromParameterFile = new List<string>();
            FileIteratorStringOnly loadParameter = new FileIteratorStringOnly();
            stringListFromParameterFile = loadParameter.loadStrings(mainParameterFile[0]);

            #region convert parameter file to variables
            //specific parmater section

            bool exportViper;
            bool analyzeGlycans;
            bool exportIsotopes;
            InputOutputFileName dataFileInput = new InputOutputFileName();
            InputOutputFileName newLibrary= new InputOutputFileName();
            InputOutputFileName textFileOutput= new InputOutputFileName();
            InputOutputFileName ViperOutputFile= new InputOutputFileName();
            InputOutputFileName isotopeOutputFile= new InputOutputFileName();
            string baseName;
            string justTheDataFilePath;
            string justTheViperOutputFolderPath;
            string justTheIsotopeOutputFolderPath;
            string justTheFeatureFileCompleteName;

            switch (mainParameterFile[3])
            {
                case "Feature":
                    WizardAnalyzeGetPeaksFeature_IMSParameters analysisParametersFeature = new WizardAnalyzeGetPeaksFeature_IMSParameters(stringListFromParameterFile);
                    dataFileInput.InputFileName = analysisParametersFeature.FullFeaturesFile;
                    isotopeOutputFile.OutputFileName = analysisParametersFeature.FullIsotopeTextResultsFile;
                    newLibrary.InputFileName = analysisParametersFeature.FullLibraryFile;
                    textFileOutput.OutputFileName = analysisParametersFeature.FullResultsTextFileOutputFile;
                    ViperOutputFile.OutputFileName = analysisParametersFeature.FullViperTextResultsFile;
                    baseName = analysisParametersFeature.JustTheFeatureFileBaseName;
                    justTheDataFilePath = analysisParametersFeature.JustTheFeatureFilePath;
                    justTheViperOutputFolderPath = analysisParametersFeature.JustTheViperOutputFolderPath;
                    justTheFeatureFileCompleteName = analysisParametersFeature.JustTheFeatureFileCompleteName;
                    justTheIsotopeOutputFolderPath = analysisParametersFeature.JustTheIsotopeOutputFolderPath;

                    analyzeGlycans = analysisParametersFeature.ShouldWeAnalyzeGlycans;
                    exportIsotopes = analysisParametersFeature.ShouldWeExportIsotopes;
                    exportViper= analysisParametersFeature.ShouldWeExportViperData;
                    break;

                case "SQLite":
                    WizardAnalyzeGetPeaksSQL_IMSParameters analysisParametersSQL = new WizardAnalyzeGetPeaksSQL_IMSParameters(stringListFromParameterFile);
                    //NewMethod(readKeyToggle, out exportViper, out analyzeGlycans, out exportIsotopes, out newLibrary, out textFileOutput, out ViperOutputFile, out isotopeOutputFile);
                    dataFileInput.InputFileName = analysisParametersSQL.FullFeaturesFile;
                    isotopeOutputFile.OutputFileName = analysisParametersSQL.FullIsotopeTextResultsFile;
                    newLibrary.InputFileName = analysisParametersSQL.FullLibraryFile;
                    textFileOutput.OutputFileName = analysisParametersSQL.FullResultsTextFileOutputFile;
                    ViperOutputFile.OutputFileName = analysisParametersSQL.FullViperTextResultsFile;
                    baseName = analysisParametersSQL.JustTheFeatureFileBaseName;
                    justTheDataFilePath = analysisParametersSQL.JustTheFeatureFilePath;
                    justTheViperOutputFolderPath = analysisParametersSQL.JustTheViperOutputFolderPath;
                    justTheFeatureFileCompleteName = analysisParametersSQL.JustTheFeatureFileCompleteName;
                    justTheIsotopeOutputFolderPath = analysisParametersSQL.JustTheIsotopeOutputFolderPath;

                    analyzeGlycans = analysisParametersSQL.ShouldWeAnalyzeGlycans;
                    exportIsotopes = analysisParametersSQL.ShouldWeExportIsotopes;
                    exportViper = analysisParametersSQL.ShouldWeExportViperData;
                    break;
                default:
                    Console.WriteLine("Oops Analyze Get Peaks IMS");
                    Console.ReadKey();
                    //place holder code
                    #region
                    WizardAnalyzeGetPeaksSQL_IMSParameters analysisParametersSQLdefault = new WizardAnalyzeGetPeaksSQL_IMSParameters(stringListFromParameterFile);
                    //NewMethod(readKeyToggle, out exportViper, out analyzeGlycans, out exportIsotopes, out newLibrary, out textFileOutput, out ViperOutputFile, out isotopeOutputFile);
                    dataFileInput.InputFileName = analysisParametersSQLdefault.FullFeaturesFile;
                    isotopeOutputFile.OutputFileName = analysisParametersSQLdefault.FullIsotopeTextResultsFile;
                    newLibrary.InputFileName = analysisParametersSQLdefault.FullLibraryFile;
                    textFileOutput.OutputFileName = analysisParametersSQLdefault.FullResultsTextFileOutputFile;
                    ViperOutputFile.OutputFileName = analysisParametersSQLdefault.FullViperTextResultsFile;
                    baseName = analysisParametersSQLdefault.JustTheFeatureFileBaseName;
                    justTheDataFilePath = analysisParametersSQLdefault.JustTheFeatureFilePath;
                    justTheViperOutputFolderPath = analysisParametersSQLdefault.JustTheViperOutputFolderPath;
                    justTheFeatureFileCompleteName = analysisParametersSQLdefault.JustTheFeatureFileCompleteName;
                    justTheIsotopeOutputFolderPath = analysisParametersSQLdefault.JustTheIsotopeOutputFolderPath;

                    analyzeGlycans = analysisParametersSQLdefault.ShouldWeAnalyzeGlycans;
                    exportIsotopes = analysisParametersSQLdefault.ShouldWeExportIsotopes;
                    exportViper = analysisParametersSQLdefault.ShouldWeExportViperData;
                    #endregion
                    break;        
            }
            

            //generic parameter setup
            
            
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
                        
                        identifierList.Add("GlycoHumanSerum03_SN26_2uL_8uLH20_Col2_T0_23Feb11_Roc_0000_LCMSFeatures");
                        identifierList.Add("GlycoHumanSerum03_SN26_2uL_8uLH20_Col2_T1_23Feb11_Roc_0000_LCMSFeatures");
                        identifierList.Add("GlycoHumanSerum03_SN26_2uL_8uLH20_Col2_T2_23Feb11_Roc_0000_LCMSFeatures");
                        identifierList.Add("GlycoHumanSerum03_SN26_2uL_8uLH20_Col2_T3inf_24Feb11_Roc_0000_LCMSFeatures");
                        identifierList.Add("GlycoHumanSerum03_SN29_2uL_8uLH20_Col3_T0_24Feb11_Roc_0000_LCMSFeatures");
                        identifierList.Add("GlycoHumanSerum03_SN29_2uL_8uLH20_Col3_T1_24Feb11_Roc_0000_LCMSFeatures");
                        identifierList.Add("GlycoHumanSerum03_SN29_2uL_8uLH20_Col3_T2_24Feb11_Roc_0000_LCMSFeatures");
                        identifierList.Add("GlycoHumanSerum03_SN29_2uL_8uLH20_Col3_T3inf_24Feb11_Roc_0000_LCMSFeatures");
                        identifierList.Add("GlycoHumanSerum03_SN30_2uL_8uLH20_T0_Col2_24Feb11_Roc_0000_LCMSFeatures");
                        identifierList.Add("GlycoHumanSerum03_SN30_2uL_8uLH20_T1_Col2_24Feb11_Roc_0000_LCMSFeatures");
                        identifierList.Add("GlycoHumanSerum03_SN30_2uL_8uLH20_T2_Col2_24Feb11_Roc_0000_LCMSFeatures");
                        //string baseName = analysisParameters.JustTheFeatureFileBaseName;
                        foreach (string identifier in identifierList)
                        {
                            DataFileIdentifierList.Add(         justTheDataFilePath             + identifier + @".txt");
                            ViperOutputFileIdentifierList.Add(  justTheViperOutputFolderPath    + identifier + @".txt");
                            IsotopeOutputFileIdentifierList.Add(justTheIsotopeOutputFolderPath  + identifier + @"_Iso.txt");
                        }
                    }
                    break;
                case false:
                    {
                        DataFileIdentifierList.Add(dataFileInput.InputFileName);
                        ViperOutputFileIdentifierList.Add(ViperOutputFile.OutputFileName);
                        IsotopeOutputFileIdentifierList.Add(isotopeOutputFile.OutputFileName);

                    }
                    break;
                default:
                    {
                        DataFileIdentifierList.Add(dataFileInput.InputFileName);
                        ViperOutputFileIdentifierList.Add(ViperOutputFile.OutputFileName);
                        IsotopeOutputFileIdentifierList.Add(isotopeOutputFile.OutputFileName);
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
                newFile.InputFileName = filepath;
                CheckFileIsPresent.CheckFile(readKeyToggle, newFile.InputFileName, "Final Target File");

                //prep objects so they can be used in the reader and writer
                List<ElutingPeakLite> elutingPeakList = new List<ElutingPeakLite>();
                List<FeatureLight> featureLitelist = new List<FeatureLight>();
                List<IsotopeObject> isotopeList = new List<IsotopeObject>();
                GetDataController dataLoad = new GetDataController();

                #region getData
                elutingPeakList = dataLoad.GetDataElutingPeakIMS(newFile);
                featureLitelist = dataLoad.GetDataFeaturesIMS(newFile);
                List<DataSet> newLibraryDataset = dataLoad.GetDataLibrary(newLibrary);
                //TODO load isotope data from IMS.  Till then use a blank array
                //isotopeList = dataLoad.GetIsotopes(newFile);
                for (int j = 0; j < elutingPeakList.Count; j++)
                {
                    IsotopeObject newIsoObject = new IsotopeObject();
                    Peak blankPeak = new Peak();
                    blankPeak.Height = 1;
                    newIsoObject.IsotopeList.Add(blankPeak);
                    isotopeList.Add(newIsoObject);
                }
                

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
                newAppender.AppendToTable(resultsCollection, textFileOutput.OutputFileName + justTheFeatureFileCompleteName);
                //newAppender.AppendToTable(resultsCollection, textFileOutput.OutputFileName + analysisParameters.JustTheFeatureFileCompleteName);
            }
            stopWatch.Stop();
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to find and assign features in eluting peaks");
            Console.Write("Finished.  Press Return to Exit"); Console.ReadKey();
            }

        //private static void NewMethod(bool readKeyToggle, out bool exportViper, out bool analyzeGlycans, out bool exportIsotopes, out InputOutputFileName newLibrary, out InputOutputFileName textFileOutput, out InputOutputFileName ViperOutputFile, out InputOutputFileName isotopeOutputFile)
        //{
        //    exportViper = analysisParameters.ShouldWeExportViperData;
        //    analyzeGlycans = analysisParameters.ShouldWeAnalyzeGlycans;
        //    exportIsotopes = analysisParameters.ShouldWeExportIsotopes;

        //    newLibrary = new InputOutputFileName();
        //    newLibrary.InputFileName = analysisParameters.FullLibraryFile;
        //    CheckFileIsPresent.CheckFile(readKeyToggle, newLibrary.InputFileName, "Library File");

        //    List<string> filesToAnalyze = new List<string>();

        //    InputOutputFileName newFeatureFileData = new InputOutputFileName();
        //    newFeatureFileData.InputFileName = analysisParameters.FullFeaturesFile;

        //    textFileOutput = new InputOutputFileName();
        //    textFileOutput.OutputFileName = analysisParameters.FullResultsTextFileOutputFile;

        //    ViperOutputFile = new InputOutputFileName();
        //    ViperOutputFile.OutputFileName = analysisParameters.FullViperTextResultsFile;

        //    isotopeOutputFile = new InputOutputFileName();
        //    isotopeOutputFile.OutputFileName = analysisParameters.FullIsotopeTextResultsFile;

        //    CheckFileIsPresent.CheckFile(readKeyToggle, newFeatureFileData.InputFileName, "Feature Data File");
        //    CheckFileIsPresent.CheckFile(readKeyToggle, textFileOutput.OutputFileName, "Results Output File");
        //    CheckFileIsPresent.CheckFile(readKeyToggle, ViperOutputFile.OutputFileName, "Viper Output File");
        //    CheckFileIsPresent.CheckFile(readKeyToggle, isotopeOutputFile.OutputFileName, "Isotope Output File");
        //}
    }
}
    
