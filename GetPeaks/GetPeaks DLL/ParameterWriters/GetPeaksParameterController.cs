using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using PNNLOmics.Algorithms.PeakDetection;
using GetPeaks_DLL.PNNLOmics_Modules;

namespace GetPeaks_DLL.ParameterWriters
{
    public class GetPeaksParameterController
    {
        #region properties

        /// <summary>
        /// allignment drift from scan to scan for elution peaks part 1
        /// </summary>
        int m_AllignmentToleranceInPPM { get; set; }
        
        /// <summary>
        /// allowed deviation of returned monoisotopic mass from summed spectra from the weighted corrected mass
        /// </summary>
        int m_ConsistancyCrossErrorCorrectedMass { get; set; }

        /// <summary>
        /// when we are checking to see if the eluting peak yields a monoisotopic peak with the same mass.
        /// If we are witin the error, the eluting peak is a monoisotopic mass.  otherwise, the eluting peak is likely an isotope
        /// </summary>
        int m_ConsistancyCrossErrorPPM { get; set; }

        /// <summary>
        /// for this instrument, does the data contain noise or not.  the Orbitrap filters out the noise
        /// </summary>
        bool m_isDataThresholded { get; set; }

        /// <summary>
        /// which algorithm to use (Thrash or Rapid)
        /// </summary>
        DeconvolutionType m_DeconvolutionAlgorithim { get; set; }

        /// <summary>
        /// Part 2 crazyness TODO what is this?
        /// </summary>
        int m_DynamicRangeToOne { get; set; }

        /// <summary>
        /// non smart number of scans to sum.  1,3,5,7 etc
        /// </summary>
        int m_GeneralScansToSum_Part1 { get; set; }

        /// <summary>
        /// the difference between monoisotopic peak and C13 peak
        /// </summary>
        double m_MassNeutron { get; set; }

        /// <summary>
        /// TOODO:  Part 1 crazyness filtering??????
        /// </summary>
        double m_MaxHeightForNewPeak { get; set; }

        /// <summary>
        /// so we can cutt off a runaway eluting peak the elutes over the whole chromatorgram
        /// </summary>
        int m_MaxScanSpread { get; set; }

        /// <summary>
        /// for multithreading in part 2?
        /// </summary>
        MemorySplitObject m_MemorySplitObject { get; set; }

        /// <summary>
        /// should we multithrad part 2.  this may be legacy  //TODO check this
        /// </summary>
        bool m_Multithread { get; set; }

        /// <summary>
        /// Is this used in the omics or just the decon tools.  aka ElutingPeakNoiseThreshold  
        /// </summary>
        int m_MSPeakDetectorPeakBR_Part1 { get; set; }

        /// <summary>
        /// Is this used in the omics or just the decon tools
        /// </summary>
        int m_MSPeakDetectorPeakBR_Part2 { get; set; }

        /// <summary>
        /// Is this used in the omics or just the decon tools
        /// </summary>
        int m_MSPeakDetectorSigNoise_Part2 { get; set; }

        /// <summary>
        /// for this instrument, does the data contain noise or not.  the Orbitrap filters out the noise
        /// </summary>
        InstrumentDataNoiseType m_NoiseType_Part1 { get; set; }

        /// <summary>
        /// for this instrument, does the data contain noise or not.  the Orbitrap filters out the noise
        /// </summary>
        InstrumentDataNoiseType m_NoiseType_Part2 { get; set; }

        /// <summary>
        /// for multithreading in part 2.  1=single thread
        /// </summary>
        int m_NumberOfDeconvolutionThreads { get; set; }

        /// <summary>
        /// Orbitrap thresholding/filtering parameters used in part1 and 2.  Perhaps divide this in to 2
        /// </summary>
        OrbitrapFilterParameters m_ParametersOrbitrap { get; set; }

        /// <summary>
        /// how we define the apex for raw peak detection with Decon Tools
        /// </summary>
        DeconTools.Backend.Globals.PeakFitType m_PeakFitType_Decon { get; set; }

        /// <summary>
        /// how we define the apex for raw peak detection with PNNL Omics
        /// </summary>         
        PNNLOmics.Algorithms.PeakDetection.PeakFitType m_PeakFitType_Omics { get; set; }

        /// <summary>
        /// how many scans in part 1 should we sum 1,3,5
        /// </summary>
        int m_ScansToBeSummed { get; set; }

        /// <summary>
        /// start processing at this scan
        /// </summary>
        int m_StartScan { get; set; }

        /// <summary>
        /// stop prcessing at this scan
        /// </summary>
        int m_StopScan { get; set; }

        /// <summary>
        /// which summing method to implement
        /// </summary>
        ScanSumSelectSwitch m_SummingMethod { get; set; }

        #endregion

        public GetPeaksParameterController()//defaults
        {
            #region defaults

            m_AllignmentToleranceInPPM = 15;//ppm not connected yet
            m_ConsistancyCrossErrorCorrectedMass = 17;//ppm not connected yet
            m_ConsistancyCrossErrorPPM = 20;//ppm not connected yet
            m_isDataThresholded = false;
            m_DeconvolutionAlgorithim = DeconvolutionType.Thrash;
            m_DynamicRangeToOne = 300000;//not connected yet
            m_GeneralScansToSum_Part1 = 1;
            m_MassNeutron = 1.002149286;
            m_MaxHeightForNewPeak = 0.75;//not connected yet
            m_MaxScanSpread = 500;

            m_MemorySplitObject = new MemorySplitObject();
            m_MemorySplitObject.BlockNumber = 0;
            m_MemorySplitObject.NumberOfBlocks = 1;

            m_MSPeakDetectorPeakBR_Part1 = 1;//ElutingPeakNoiseThreshold //this needs to be checked
            m_MSPeakDetectorPeakBR_Part2 = 1;
            m_MSPeakDetectorSigNoise_Part2 = 1;//not connected yet
            m_Multithread = true;
            m_NoiseType_Part1 = InstrumentDataNoiseType.NoiseRemoved;
            m_NoiseType_Part2 = InstrumentDataNoiseType.Standard;
            m_NumberOfDeconvolutionThreads = 1;

            m_ParametersOrbitrap = new OrbitrapFilterParameters();
            m_ParametersOrbitrap.ChargeStateList = new List<int>{1,2,3,4};
            m_ParametersOrbitrap.DeltaMassTollerancePPM = 6000;
            m_ParametersOrbitrap.ExtraSigmaFactor = 1.5;
            // m_ParametersOrbitrap.LowMassFilterAveragine = 
            m_ParametersOrbitrap.massNeutron = m_MassNeutron;
            m_ParametersOrbitrap.PeakSkipList = new List<int> { 1, 2, 3, 4 };
            m_ParametersOrbitrap.withLowMassesAllowed = true;

            m_PeakFitType_Decon = DeconTools.Backend.Globals.PeakFitType.APEX;//not connected yet
            m_PeakFitType_Omics = PeakFitType.Lorentzian;
            m_ScansToBeSummed = 3;
            m_StartScan = 1;
            m_StopScan = 9999999;
            m_SummingMethod = ScanSumSelectSwitch.SumScan;

            #endregion
        }

        public void GenerateFiles()
        {
            

            #region 1/3. Set Folders
            //1 select platform
            string platform = "Local";
            //string platform = "Server";

            //2 set folders
            string folderWithData;
            string folderWithGetPeaksApplication;
            string folderWithAnalysisApplication;
            string folderWithParameterFiles;
            string folderForTextFileCreation;
            string folderForGetPeaksSQLFiles;
            string folderForViperTextFiles;

            string folderWithLibraries;
            string folderForAnalysisOutput;
            string folderForIsotopeOutput;

            SetFolders(platform, 
                out folderWithData, 
                out folderWithGetPeaksApplication, 
                out folderWithAnalysisApplication, 
                out folderWithParameterFiles, 
                out folderForTextFileCreation, 
                out folderForGetPeaksSQLFiles, 
                out folderForViperTextFiles,
                out folderWithLibraries,
                out folderForAnalysisOutput,
                out folderForIsotopeOutput);

            //3 set library
            string selectedLibrary = SetLibrary(platform);

            //4 set parameter location and batch file launch place
            string parameterCreationFolder;
            string batchCreationFolder;//must have all acompanying files etc.  elements table, yafms etc
            SetLaunchLocation(platform, folderWithParameterFiles, out parameterCreationFolder, out batchCreationFolder);

            #endregion

            #region 2/3. file list goes here

            List<string> filesToProcess = new List<string>();
            //filesToProcess.Add("GLY05_SN71x4_9Sep11_Cougar_SK_Carbon75_60_6.raw");
            //filesToProcess.Add("GLY05_SN71x2_9Sep11_Cougar_SK_Carbon75_60_6.raw");
            //filesToProcess.Add("GLY05_SN60_9Sep11_Cougar_SK_Carbon75_60_5.raw");
            //filesToProcess.Add("GLY05_SN65_9Sep11_Cougar_SK_Carbon75_60_5.raw");
            //filesToProcess.Add("GLY05_SN59_9Sep11_Cougar_SK_Carbon75_60_6.raw");
            //filesToProcess.Add("GLY05_SN55_9Sep11_Cougar_SK_Carbon75_60_5.raw");
            //filesToProcess.Add("GLY05_SN71_9Sep11_Cougar_SK_Carbon75_60_6.raw");
            filesToProcess.Add("GLY05_SN62_9Sep11_Cougar_SK_Carbon75_60_5.raw");
            //filesToProcess.Add("GLY05_SN61_9Sep11_Cougar_SK_Carbon75_60_6.raw");
            //filesToProcess.Add("GLY05_SN54_9Sep11_Cougar_SK_Carbon75_60_5.raw");
            //filesToProcess.Add("GLY05_SN69_9Sep11_Cougar_SK_Carbon75_60_6.raw");
            //filesToProcess.Add("GLY05_SN67_9Sep11_Cougar_SK_Carbon75_60_5.raw");
            //filesToProcess.Add("GLY05_SN63_9Sep11_Cougar_SK_Carbon75_60_6.raw");
            //filesToProcess.Add("GLY05_SN57_9Sep11_Cougar_SK_Carbon75_60_5.raw");
            //filesToProcess.Add("GLY05_SN53_8Sep11_Cougar_SK_Carbon75_60_6.raw");
            //filesToProcess.Add("GLY05_SN66_8Sep11_Cougar_SK_Carbon75_60_5.raw");
            filesToProcess.Add("GLY05_SN59x4_9Sep13_Cougar_SK_Carbon75_60_6.raw");
            filesToProcess.Add("GLY05_SN69x4_9Sep13_Cougar_SK_Carbon75_60_6.raw");
            filesToProcess.Add("GLY05_SN60x20_9Sep14_Cougar_SK_Carbon75_60_6.raw");
                
                
                    
            List<string> folderIDToProcess = new List<string>();
            //folderIDToProcess.Add("GLY05_SN71x4");
            //folderIDToProcess.Add("GLY05_SN71x2");
            //folderIDToProcess.Add("GLY05_SN60");
            //folderIDToProcess.Add("GLY05_SN65");
            //folderIDToProcess.Add("GLY05_SN59");
            //folderIDToProcess.Add("GLY05_SN55");
            //folderIDToProcess.Add("GLY05_SN71");
            folderIDToProcess.Add("GLY05_SN62");
            //folderIDToProcess.Add("GLY05_SN61");
            //folderIDToProcess.Add("GLY05_SN54");
            //folderIDToProcess.Add("GLY05_SN69");
            //folderIDToProcess.Add("GLY05_SN67");
            //folderIDToProcess.Add("GLY05_SN63");
            //folderIDToProcess.Add("GLY05_SN57");
            //folderIDToProcess.Add("GLY05_SN53");
            //folderIDToProcess.Add("GLY05_SN66");
            folderIDToProcess.Add("GLY05_SN59x4");
            folderIDToProcess.Add("GLY05_SN69x4");
            folderIDToProcess.Add("GLY05_SN60x20");

            #endregion

            #region Parameter file locations (for parameter file creation)

            for (int i = 0; i < filesToProcess.Count; i++)
            {
                filesToProcess[i] = folderWithData + filesToProcess[i];
            }

            List<string> outputFolderLocationList = new List<string>();
            List<string> outputFolderLocationAnalysisList = new List<string>();
            List<string> batchList = new List<string>();

            for (int i = 0; i < filesToProcess.Count; i++)
            {

                string outputFolderLocation = parameterCreationFolder + "Parameter" + folderIDToProcess[i] + "_GetPeak.txt";
                outputFolderLocationList.Add(outputFolderLocation);

                string outputFolderLocationAnalysis = parameterCreationFolder + "Parameter" + folderIDToProcess[i] + "_Analysis.txt";
                outputFolderLocationAnalysisList.Add(outputFolderLocationAnalysis);

                string batchPath = batchCreationFolder+ "Batch_" + folderIDToProcess[i] + ".bat";
                batchList.Add(batchPath);
            }
            
            #endregion

            #region 3/3. change default values here

            //General           
            string SQLOutPrefix = "SQLiteBatchResult";
            string viperPrefix = "Viper";
            string dataTypeToAnalyze = "SQLite";  //SQLite or Feature
            
            //GET PEAKS
            bool shouldWeRunGetPeaks = true;
                string getPeakstextOutputPrefix = "GetPeaksResult";
                //GetPeaks Part1
                m_StartScan = 100;
                m_StopScan = 99999;
                m_MSPeakDetectorPeakBR_Part1 = 1;//1/Part1Sigma Threshold/// how many sigma above the noise
                m_GeneralScansToSum_Part1 = 3;
                //GetPeaks Part2
                m_MemorySplitObject.NumberOfBlocks = 1;
                m_MemorySplitObject.BlockNumber = 0;
                m_MSPeakDetectorPeakBR_Part2 = 3;//Part2Sigma Threshold/// how many sigma above the noise
                m_NumberOfDeconvolutionThreads = 3;
                m_SummingMethod = ScanSumSelectSwitch.SumScan;
                m_ConsistancyCrossErrorCorrectedMass = 20;//ppm Not connected yet
                m_ConsistancyCrossErrorPPM = 20;//ppm Not connected yet

            //ANALYSIS
            bool shouldWeRunAnalysis = true;
                string analysistextOutputPrefix = "AnalysisResult";
                //Analysis
                bool shouldWeAnalyzeGlycans = true;
                bool shouldWeAnalyzeIsotopes = true;
                bool shouldWeAnalyzeOutputToViper = true;

            #endregion

            #region Create Data transfer objects

            BatchFileTransferObject batchInfo = new BatchFileTransferObject();
            batchInfo.FolderWithGetPeaks = folderWithGetPeaksApplication;
            batchInfo.FolderWithAnalysis = folderWithAnalysisApplication;
            batchInfo.FolderWithParameters = folderWithParameterFiles;
            batchInfo.FolderForTextFiles = folderForTextFileCreation;
            batchInfo.FolderForSQLFiles = folderForGetPeaksSQLFiles + SQLOutPrefix;
            batchInfo.FolderForViperTextFiles = folderForViperTextFiles;
            batchInfo.PrefixAnalysistextOutput = analysistextOutputPrefix;
            batchInfo.PrefixGetPeakstextOutput = getPeakstextOutputPrefix;
            batchInfo.SQLPrefix = SQLOutPrefix;
            batchInfo.ViperPrefix = viperPrefix;

            AnalysisParamertersTransferObject analysisInfo = new AnalysisParamertersTransferObject();
            analysisInfo.libraryDataFolder = folderWithLibraries;
            analysisInfo.locationOfSQLFileToAnalyze = folderForGetPeaksSQLFiles;
            analysisInfo.ViperResultPath = folderForViperTextFiles;
            analysisInfo.outputResultsPath = folderForAnalysisOutput;
            analysisInfo.isotopesResultsPath = folderForIsotopeOutput;
            analysisInfo.DataTypeToAnalyze = dataTypeToAnalyze;
            analysisInfo.prefixOnSQLFile = SQLOutPrefix;
            analysisInfo.libraryDataName = selectedLibrary;
            analysisInfo.analyzeGlycans = shouldWeAnalyzeGlycans;
            analysisInfo.outputIsotopes = shouldWeAnalyzeIsotopes;
            analysisInfo.outputVipir = shouldWeAnalyzeOutputToViper;

            SimpleWorkflowParameters ParametersToSet = new SimpleWorkflowParameters();
            //ParametersToSet.FileNameUsed = filesToProcess[0];//below

            ParametersToSet.ConsistancyCrossErrorCorrectedMass = m_ConsistancyCrossErrorCorrectedMass;
            ParametersToSet.ConsistancyCrossErrorPPM = m_ConsistancyCrossErrorPPM;
            ParametersToSet.SummingMethod = m_SummingMethod;

            ParametersToSet.Part1Parameters.AllignmentToleranceInPPM = m_AllignmentToleranceInPPM;
            ParametersToSet.Part1Parameters.ElutingPeakNoiseThreshold = m_MSPeakDetectorPeakBR_Part1;
            ParametersToSet.Part1Parameters.isDataAlreadyThresholded = m_isDataThresholded;
            ParametersToSet.Part1Parameters.MaxHeightForNewPeak = m_MaxHeightForNewPeak;
            ParametersToSet.Part1Parameters.MSPeakDetectorPeakBR = m_MSPeakDetectorPeakBR_Part1;
            ParametersToSet.Part1Parameters.NoiseType = m_NoiseType_Part1;
            ParametersToSet.Part1Parameters.ParametersOrbitrap = m_ParametersOrbitrap;
            ParametersToSet.Part1Parameters.PeakFitType = m_PeakFitType_Decon;//decon
            ParametersToSet.Part1Parameters.ScansToBeSummed = m_ScansToBeSummed;
            ParametersToSet.Part1Parameters.StartScan = m_StartScan;
            ParametersToSet.Part1Parameters.StopScan = m_StopScan;

            ParametersToSet.Part2Parameters.DeconvolutionType = m_DeconvolutionAlgorithim;
            ParametersToSet.Part2Parameters.DynamicRangeToOne = m_DynamicRangeToOne;
            ParametersToSet.Part2Parameters.isDataThresholded = m_isDataThresholded;
            ParametersToSet.Part2Parameters.MaxScanSpread = m_MaxScanSpread;
            ParametersToSet.Part2Parameters.MemoryDivider = m_MemorySplitObject;
            ParametersToSet.Part2Parameters.MSPeakDetectorPeakBR = m_MSPeakDetectorPeakBR_Part2;
            ParametersToSet.Part2Parameters.MSPeakDetectorSigNoise = m_MSPeakDetectorSigNoise_Part2;
            ParametersToSet.Part2Parameters.Multithread = m_Multithread;
            ParametersToSet.Part2Parameters.numberOfDeconvolutionThreads = m_NumberOfDeconvolutionThreads;
            ParametersToSet.Part2Parameters.NoiseType = m_NoiseType_Part2;
            ParametersToSet.Part2Parameters.ParametersOrbitrap = m_ParametersOrbitrap;
            ParametersToSet.Part2Parameters.PeakFitType = m_PeakFitType_Decon;

            #endregion

            #region controller

            GetPeaksParameterFileCreator newFileGenerator = new GetPeaksParameterFileCreator();
            GetPeaksAnalysisFileCreator newFileGeneratorAnalysis = new GetPeaksAnalysisFileCreator();
            GetPeaksBatchFileCreator newBatchFileGenerator = new GetPeaksBatchFileCreator();

            for (int i = 0; i < filesToProcess.Count; i++)
            {
                //create files for each raw data file
                ParametersToSet.FileNameUsed = filesToProcess[i];
                newFileGenerator.CreateFile(ParametersToSet, filesToProcess[i], folderIDToProcess[i], outputFolderLocationList[i]);
                newFileGeneratorAnalysis.CreateFile(analysisInfo, filesToProcess[i], folderIDToProcess[i], outputFolderLocationAnalysisList[i]);
                newBatchFileGenerator.CreateFile(batchInfo,shouldWeRunGetPeaks,shouldWeRunAnalysis, folderIDToProcess[i], batchList[i]);
            }

            #endregion
        }

        private static void SetLaunchLocation(string platform, string folderWithParameterFiles, out string ParameterCreationFolder, out string BatchCreationFolder)
        {
            switch (platform)
            {
                case "Local":
                    {
                        ParameterCreationFolder = folderWithParameterFiles;//writes to the same place it is used
                        //ParameterCreationFolder = @"D:\Csharp\ConosleApps\LocalServer\";
                        BatchCreationFolder = @"D:\Csharp\ConosleApps\LocalServer\";
                    }
                    break;
                case "Server":
                    {
                        ParameterCreationFolder = @"V:\RAM Files\";
                        BatchCreationFolder = @"V:\RAM Files\Batch\";
                    }
                    break;
                default:
                    {
                        Console.WriteLine("Select a Platform Local or Server");
                        Console.ReadKey();
                        ParameterCreationFolder = folderWithParameterFiles;//writes to the same place it is used
                        BatchCreationFolder = @"D:\Csharp\ConosleApps\LocalServer\";
                        //ParameterCreationFolder = @"V:\RAM Files\";
                        //BatchCreationFolder = @"V:\RAM Files\Batch\";
                    }
                    break;
            }
        }

        //select a library directory
        private static string SetLibrary(string platform)
        {
            string selectedLibrary;

            switch (platform)
            {
                case "Local":
                    {
                        selectedLibrary = "L_LibraryDirectoryLocalAlditol.txt";
                    }
                    break;
                case "Server":
                    {
                        selectedLibrary = "L_LibraryDirectoryServerAlditol.txt";
                    }
                    break;
                default:
                    {
                        Console.WriteLine("Select a Platform Local or Server");
                        Console.ReadKey();
                        selectedLibrary = "L_LibraryDirectoryServerAlditol.txt";
                    }
                    break;
            }
            return selectedLibrary;
        }

        private static void SetFolders(string platform, 
            out string folderWithData, 
            out string folderWithGetPeaksApplication, 
            out string folderWithAnalysisApplication, 
            out string folderWithParameterFiles, 
            out string folderForTextFileCreation, 
            out string folderForGetPeaksSQLFiles, 
            out string folderForViperTextFiles,
            out string folderWithLibraries,
            out string folderForAnalysisOutput,
            out string folderForIsotopeOutput)

        {
            switch (platform)
            {
                case "Local":
                    {
                        folderWithData = @"P:\";
                        folderWithGetPeaksApplication = @"D:\Csharp\ConosleApps\LocalServer\XRSGetPeaksServerProgram\GetPeaks 4.0.exe";
                        folderWithAnalysisApplication = @"D:\Csharp\ConosleApps\LocalServer\XRSAnalysisProgram\WizardAnalyzeGetPeaksSQL\WizardAnalyzeGetPeaksSQL.exe";
                        folderWithParameterFiles = @"V:\RAM Files\";
                        //folderWithParameterFiles = @"D:\Csharp\ConosleApps\LocalServer\";
                        folderForTextFileCreation = @"D:\Csharp\ConosleApps\zGetPeaksOutAnalysis\";
                        folderForGetPeaksSQLFiles = @"V:\";
                        //folderForGetPeaksSQLFiles = @"D:\Csharp\ConosleApps\zSQL Results\";
                        folderForViperTextFiles = @"D:\Csharp\ConosleApps\zVIPER Output\";
                        folderWithLibraries = @"D:\Csharp\ConosleApps\zLibraries\";
                        folderForAnalysisOutput =  @"D:\Csharp\ConosleApps\zGetPeaksOutAnalysis\";
                        folderForIsotopeOutput = @"D:\Csharp\ConosleApps\zIsotopeOutput\";
                    }
                    break;
                case "Server":
                    {
                        folderWithData = @"E:\ScottK\GetPeaks Data\";
                        folderWithGetPeaksApplication = @"E:\ScottK\XRSGetPeaksServerProgram\GetPeaks 4.0.exe";
                        folderWithAnalysisApplication = @"E:\ScottK\XRSAnalysisProgram\WizardAnalyzeGetPeaksSQL\WizardAnalyzeGetPeaksSQL.exe";
                        folderWithParameterFiles = @"E:\ScottK\XRSGetPeaksServerParameterFiles\";
                        folderForTextFileCreation = @"E:\ScottK\XRSGetPeaksServerTextResults\";
                        folderForGetPeaksSQLFiles = @"E:\ScottK\XRSGetPeaksSQLOutputResults\";
                        folderForViperTextFiles = @"E:\ScottK\XRSzAnalysisOutput\VIPER Output\";
                        folderWithLibraries = @"E:\ScottK\XRSzAnalysisOutput\Libraries\";
                        folderForAnalysisOutput =  @"E:\ScottK\XRSzAnalysisOutput\GetPeaksOutAnalysis\";
                        folderForIsotopeOutput = @"E:\ScottK\XRSzAnalysisOutput\IsotopeOutput\";
                    }
                    break;
                default:
                    {
                        Console.WriteLine("Select a Platform Local or Server");
                        Console.ReadKey();
                        folderWithData = @"V:\";
                        folderWithGetPeaksApplication = @"D:\Csharp\ConosleApps\LocalServer\XRSGetPeaksServerProgram\GetPeaks 4.0.exe";
                        folderWithAnalysisApplication = @"D:\Csharp\ConosleApps\LocalServer\XRSAnalysisProgram\WizardAnalyzeGetPeaksSQL\WizardAnalyzeGetPeaksSQL.exe";
                        folderWithParameterFiles = @"D:\Csharp\ConosleApps\LocalServer\";
                        folderForTextFileCreation = @"D:\Csharp\ConosleApps\zGetPeaksOutAnalysis\";
                        folderForGetPeaksSQLFiles = @"D:\Csharp\ConosleApps\zSQL Results\";
                        folderForViperTextFiles = @"D:\Csharp\ConosleApps\zVIPER Output\";
                        folderWithLibraries = @"D:\Csharp\ConosleApps\zLibraries\";
                        folderForAnalysisOutput =  @"D:\Csharp\ConosleApps\zGetPeaksOutAnalysis\";
                        folderForIsotopeOutput = @"D:\Csharp\ConosleApps\zIsotopeOutput\";
                    }
                    break;
            }
        }
    }
}