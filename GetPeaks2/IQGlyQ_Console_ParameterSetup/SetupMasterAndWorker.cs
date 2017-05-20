using System;
using System.Collections.Generic;
using System.IO;
using DivideTargetsLibrary.Parameters;
using GetPeaksDllLite.DataFIFO;
using IQGlyQ.Enumerations;
using DivideTargetsLibrary;

namespace IQGlyQ_Console_ParameterSetup
{
    public class SetupMasterAndWorker
    {

        public void GeneralSetupRunFirst(string targetsFileNameIn, string processingParameters, string divideTargetsParametersName, string dividetargetsFolder, string specificParameters, string factors, string masterLocation, string workerLocation,string masterComputerName)
        {
            //input


            //TODO
            //we need to writeout a replacement Parameters_DivideTargetsPIC with the new library
            string divideTargetsParameterFile = dividetargetsFolder + @"\" + divideTargetsParametersName;

            string baseTargetsFile;
            string fullTargetPath;
            string textFileEnding;
            ParameterDivideTargets parameters = LoadParameters.SetupDivideTargetParameters(divideTargetsParameterFile, out baseTargetsFile, out fullTargetPath, out textFileEnding);
            parameters.TargetsFileName = targetsFileNameIn;

            int cores = Convert.ToInt32(parameters.CoresString);

            Console.WriteLine("We are now updating the Parameters_DivideTargets file with the correct targets");
            if (File.Exists(dividetargetsFolder + @"\" + targetsFileNameIn))
            {
                Console.WriteLine("The New Target File Exists: " + dividetargetsFolder + @"\" + targetsFileNameIn);
            }
            else
            {
                Console.WriteLine("The New Targets File Does Not Exist");
            }
            parameters.WriteParameters(divideTargetsParameterFile);

            string rawFileName = parameters.DataFileFileName + "." +parameters.DataFileEnding;
            string peaksFileName = parameters.DataFileFileName + "_peaks.txt";


            //FragmentedTargetedWorkflowParameters_Velos_DH.txt

            //changeable parameters
            string TargetsFilePath = "";
            //string RawFile = "";
            string RawFolderHome = "";
            string RawFolderHomePICFS = "";
            //string DivideTargetsParameters = "";
            //string ProcessingParameters = "";

            EnumerationDataset dataType;
            dataType = EnumerationDataset.Diabetes;
            //dataType = EnumerationDataset.SPINExactiveMuddiman;
            //dataType = EnumerationDataset.SPINExactive;

            //TargetsFilePath = "L_10_IQ_TargetsFirstAll.txt";
            //TargetsFilePath = "L_10_IQ_TargetsFirst3.txt";
            //TargetsFilePath = "L_PSA21_TargetsFirstAll.txt";
            //TargetsFilePath = "L_10_IQ_MuddimanTargetsH.txt";
            //TargetsFilePath = "L_PSA21_IQ_MuddimanH.txt";
            TargetsFilePath = targetsFileNameIn;

            //string basicTargetedAlignmentWorkflowParametersFileName = "TargetedAlignmentWorkflowParameters.xml";
            //string basicTargetedWorkflowParameters = "BasicTargetedWorkflowParameters.xml";


            //ProcessingParameters = "FragmentedTargetedWorkflowParameters_Velos_DH.txt";

            string randomIQFile = "SN111-SN117_OrbiStandard_workflowExecutorParametersSK_Work HM Test.xml";

            //string PexecLocation = @"C:\Download\SysinternalsSuite\psexec.exe";
            //string ipAddressForExecution = "192.168.3.20";
            //string NodeName = "pub-2000";

            string ipAddressForExecution = "192.168.3.22"; string NodeName = "PUB-2002";

            //string divideTargetsParameters = "Parameters_DivideTargetsPIC.txt";//input from parameters
            string divideTargetsParametersNode = "Parameters_DivideTargetsPICNodes.txt";

            //string ipAddressForExecution = "192.168.3.23"; string NodeName = "PUB-2003";
            string ZipBatchName = "Zip.bat";
            string ZipParameterFile = "FilesToZIP.txt";
            string LaunchJobsBatchFileName = "PIC_LaunchJobs.bat";

            string testBatchName = "testBatch.bat";
            string globalMultiSleepFile = "PIC_MultiSleepParameterFileGlobal.txt";
            string globalMultiSleepFileListFile = "PIC_MultiSleepParameterFileGlobal_List.txt";

            string ApplicationSetupParameters = "ApplicationSetupParameters.txt";
            string ApplicationSetupParametersWorker = "ApplicationSetupParametersF.txt";

            string adminAccount = "Administrator";
            string adminPassword = "Pub123456";
            //string SecondLaunchLocation = @"\\pub-1000\Shared_PICFS\ToPIC\PIC_RunMeSeccond_PUB100X.bat";
            string SecondLaunchLocation = @"PIC_RunMeSeccond_PUB100X.bat";
            string ConsolidationAtNodeLevelList = "ConsolidationParameterForAllNodes.txt";
            //string nodeConsolidation = @"PIC_NodeConsolidation.bat";

            string sleepParameterFile = "PIC_SleepParameterFile_" + NodeName + ".txt";
            string multiSleepParameterFile = "PIC_MultiSleepParameterFile.txt";
            string GoCrazyCopyParameterFile = "PIC_ParametarFileCopyExponential.txt";

            //C:\Download\SysinternalsSuite\psexec.exe \\192.168.3.22 -u 192.168.3.22\Administrator -p Pub123456 "E:\\NodeShareFolder\PIC_RunMeSeccond_PUB100X.bat"
            string adminAccountMasterPub10000 = "Administrator";
            string adminPasswordMasterPub10000 = "Pub123456";
            string ipAddressForExecutionMaster = "192.168.3.10";
            //string NodeNameMaster = "pub-1000";

            string nodeShareFolderLocationOnNode = @"E:\";

            string FolderHoldingElementsDataForOmics = "MassSpectrometerDLLs";
            string PNNLElementDataFolderONWorker = @"C:\MassSpectrometerDLLs";

            //string SecondLaunchLocation = @"F\ScottK\ToPIC\PUB100X_CopyResultBack.bat";//we need an xcopy here

            //this is for copying the raw file around.  actual run is via set strings
            switch (dataType)
            {
                case EnumerationDataset.SPINExactive:
                    {

                        //RawFile = "Gly09_SN130_4Mar13_Cheetah_C14_220nL_SPIN_1900V_1600mlmin_22Torr_100C_100kHDR2M2mbar.raw";
                        RawFolderHome = @"\\" + masterComputerName + @"\Shared_PICFS\RawData\2013_02_18 SPIN Exactive04";
                        //RawFolderHomePICFS = @"\\picfs\projects\DMS\ScottK\RawData\2012_12_24 Velos 3
                    }
                    break;
                case EnumerationDataset.SPINExactiveMuddiman:
                    {
                        //ALSO SET in SET STRINGS
                        //A 126-131
                        //RawFile = "Gly09_SN126131_6Mar13_Cheetah_C18_50cm_220nL_SPIN_1900V_1600mlmin_22Torr_100C_100kHDR2M2mbar.raw";

                        //B 125-128
                        //RawFile = "Gly09_SN125128_7Mar13_Cheetah_C18_50cm_220nL_SPIN_1900V_1600mlmin_22Torr_100C_100kHDR2M2mbar.raw";

                        //C 127-132
                        //RawFile = "Gly09_SN127132_6Mar13_Cheetah_C18_50cm_220nL_SPIN_1900V_1600mlmin_22Torr_100C_100kHDR2M2mbar.raw";


                        RawFolderHome = @"\\" + masterComputerName + @"\Shared_PICFS\RawData\2013_02_18 SPIN Exactive04";
                        //RawFolderHomePICFS = @"\\picfs\projects\DMS\ScottK\RawData\2012_12_24 Velos 3
                    }
                    break;
                case EnumerationDataset.SN123R8:
                    {
                        //RawFile = "Gly09_Velos3_Jaguar_230nL30_C15_SN123_3X_01Jan13_R8.raw";
                        RawFolderHome = @"\\" + masterComputerName + @"\Shared_PICFS\RawData\2012_12_24 Velos 3";
                        //RawFolderHomePICFS = @"\\picfs\projects\DMS\ScottK\RawData\2012_12_24 Velos 3
                    }
                    break;
                case EnumerationDataset.Diabetes:
                    {
                        //RawFile = "Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw";
                        RawFolderHome = @"\\" + masterComputerName + @"\Shared_PICFS\RawData\2012_12_24 Velos 3";
                        //RawFolderHomePICFS = @"\\picfs\projects\DMS\ScottK\RawData\2012_12_24 Velos 3
                    }
                    break;
                default:
                    {
                        //RawFile = "Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw";
                        RawFolderHome = @"\\" + masterComputerName + @"\Shared_PICFS\RawData\2012_12_24 Velos 3";
                        //RawFolderHomePICFS = @"\\picfs\projects\DMS\ScottK\RawData\2012_12_24 Velos 3
                    }
                    break;
            }

            string PubName = System.Environment.MachineName;


            //No change parameters
            //string writeFolder = @"F:\ScottK\ToPic";
            string Pub1000HomeFolder = @"E:\ScottK\Shared_PICFS\ToPIC";
            string onPub1000WorkingParametersFolder = Pub1000HomeFolder + @"\" + "WorkingParameters";
            string LaunchInitializion = @"\\"+ masterComputerName + @"\Shared_PICFS\ToPIC";//string LaunchInitializion = @"\\pub-1000\Shared_PICFS\ToPIC";
            string LibraryHome = @"\\"+ masterComputerName + @"\Shared_PICFS\ToPIC\WorkingParameters";

            string ApplciationHome = @"\\" + masterComputerName + @"\Shared_PICFS\ToPIC\GlyQ-IQ Application\Release";
            string ResultsFutureHome = @"\\" + masterComputerName + @"\Shared_PICFS\ToPIC\Results\" + PubName + "_Results";
            string LibraryOnPICFS = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\WorkingParameters";

            //ZippedFolder
            string RunMeFirstFromPICFSZipped = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\Zipped";
            string RunMeFirstFromPub1000Zipped = @"E:\ScottK\Shared_PICFS\ToPIC\Zipped";
            string ZippedPub100XSetup = @"\\" + masterComputerName + @"\Shared_PICFS\ToPIC\Zipped";

            //ZippedApp
            string RunMeFirstFromPICFSZippedApp = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ Zip";
            string RunMeFirstFromPub1000ZippedApp = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ Zip";


            //initial pub 1000 setup
            //this is the uploaded application setup program files living on PICFS
            string RunMeFirstFromPICFSSetup = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ Application Setup";
            string RunMeFirstFromPub1000Setup = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ Application Setup";

            //this is the uploaded application program files living on PICFS
            string RunMeFirstFromPICFSApplication = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ Application";
            string RunMeFirstFromPub1000Application = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ Application";

            //this is the uploaded divideTargets program files living on PICFS
            string RunMeFirstFromPICFSdivideTargets = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ DivideTargets";
            string RunMeFirstFromPub1000divideTargets = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ DivideTargets";
            string DivideTargetsHomePub100XSetup = @"\\" + masterComputerName + @"\Shared_PICFS\ToPIC\GlyQ-IQ DivideTargets";
            string DivideTargets100XPub100XSetupLocation = workerLocation + @"\GlyQ-IQ DivideTargets\Release\DivideTargets.exe";


            //this is the uploaded postprocessing (from divide targets) program files living on PICFS
            string RunMeFirstFromPICFSpostProcessing = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ PostProcessing";
            string RunMeFirstFromPub1000postProcessing = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ PostProcessing";
            string PostProcessingHomePub100XSetup = @"\\" + masterComputerName + @"\Shared_PICFS\ToPIC\GlyQ-IQ PostProcessing";
            string PostProcessing100XPub100XSetupLocation = workerLocation + @"\GlyQ-IQ PostProcessing\Release\PostProcessing.exe";


            //this is the uploaded sleep (from divide targets) program files living on PICFS
            string RunMeFirstFromPICFSSleep = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ Sleep";
            string RunMeFirstFromPub1000Sleep = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ Sleep";
            string SleepExeLocation = Pub1000HomeFolder + @"\" + @"GlyQ-IQ Sleep\Release\Sleep.exe";

            //this is the uploaded sleep (from divide targets) program files living on PICFS
            string RunMeFirstFromPICFSSleepMulti = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ MultiSleep";
            string RunMeFirstFromPub1000SleepMulti = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ MultiSleep";
            string SleepMultiExeLocation = workerLocation + @"\" + @"GlyQ-IQ MultiSleep\Release\MultiSleep.exe";
            string SleepMultiHomePub100XSetup = @"\\" + masterComputerName + @"\Shared_PICFS\ToPIC\GlyQ-IQ MultiSleep";

            //initial pub100X setup
            //this is where the setup app will live on pub 1000
            string ApplciationHomePub100XSetupHome = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC";
            string ApplciationHomePub100XSetup = @"\\" + masterComputerName + @"\Shared_PICFS\ToPIC\GlyQ-IQ Application Setup";
            string Applciation100XPub100XSetupLocation = workerLocation + @"\GlyQ-IQ Application Setup\Release\IQGlyQ_Console_ParameterSetup.exe";

            string outputLocation = "";
            string nodeShareFolder = "NodeShareFolder";

            //this is the uploaded GlyQ-IQ CheckFile (from divide targets) program files living on PICFS
            string RunMeFirstFromPICFSCheckFile = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ CheckFile";
            string RunMeFirstFromPub1000CheckFile = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ CheckFile";
            string CheckFileExeLocation = workerLocation + @"\" + @"GlyQ-IQ MultiSleep\Release\WriteCheckFile.exe";
            string CheckFileHomePub100XSetup = @"\\" + masterComputerName + @"\Shared_PICFS\ToPIC\GlyQ-IQ CheckFile";

            //this is the uploaded GlyQ-IQ DeleteFiles (from divide targets) program files living on PICFS
            string RunMeFirstFromPICFSDeleteFilesExpCopy = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ DeleteFiles";
            string RunMeFirstFromPub1000DeleteFilesExpCopy = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ DeleteFiles";
            string DeleteFilesExpCopyExeLocation = workerLocation + @"\" + @"GlyQ-IQ DeleteFiles\Release\DeleteFiles.exe";
            string DeleteFilesExpCopyHomePub100XSetup = @"\\" + masterComputerName + @"\Shared_PICFS\ToPIC\GlyQ-IQ DeleteFiles";


            //this is the uploaded GlyQ-IQ CrazyFilesCopy (from Geometric copy) program files living on PICFS
            string RunMeFirstFromPICFSCrazyFilesCopy = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ CrazyFileCopy";
            string RunMeFirstFromPub1000CrazyFilesCopy = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ CrazyFileCopy";
            string CrazyFilesCopyExeLocation = workerLocation + @"\" + @"GlyQ-IQ CrazyFileCopy\Release\MultiSleep.exe";
            string CrazyFilesCopyHomePub100XSetup = @"\\" + masterComputerName + @"\Shared_PICFS\ToPIC\GlyQ-IQ CrazyFileCopy";

            //this is the uploaded GlyQ-IQ DivideTargetsNode (from divide targets) program files living on PICFS
            string RunMeFirstFromPICFSDivideTargetsNode = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ DivideTargetsNode";
            string RunMeFirstFromPub1000DivideTargetsNode = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ DivideTargetsNode";
            string DivideTargetsNodeExeLocation = Pub1000HomeFolder + @"\" + @"GlyQ-IQ DivideTargetsNode\Release\DivideTargetsNodes.exe";
            string DivideTargetsNodeHomePub100XSetup = @"\\" + masterComputerName + @"\Shared_PICFS\ToPIC\GlyQ-IQ DivideTargetsNode";

            //this is the uploaded GlyQ-IQ CombineNodeResults (from divide targets) program files living on PICFS
            string RunMeFirstFromPICFSCombineNodeResults = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ CombineNodeResults";
            string RunMeFirstFromPub1000CombineNodeResults = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ CombineNodeResults";
            string CombineNodeResultsExeLocation = Pub1000HomeFolder + @"\" + @"GlyQ-IQ CombineNodeResults\Release\CombineNodeResults.exe";
            string CombineNodeResultsHomePub100XSetup = @"\\" + masterComputerName + @"\Shared_PICFS\ToPIC\GlyQ-IQ CombineNodeResults";

            //this is the uploaded GlyQ-IQ GlyQ-IQ Timer (from divide targets) program files living on PICFS
            string RunMeFirstFromPICFSTimer = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ Timer";
            string RunMeFirstFromPub1000Timer = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ Timer";
            string TimerExeLocation = Pub1000HomeFolder + @"\" + @"GlyQ-IQ Timer\Release\WriteTime.exe";
            string TImerResultsHomePub100XSetup = @"\\" + masterComputerName + @"\Shared_PICFS\ToPIC\GlyQ-IQ Timer";

            //code


            Directory.CreateDirectory(workerLocation);

            string workingRawDataFolder = workerLocation + @"\" + "RawData";

            Directory.CreateDirectory(workingRawDataFolder);

            string workingZippedFolder = workerLocation + @"\" + "Zipped";

            Directory.CreateDirectory(workingZippedFolder);

            string workingResultsFolder = masterLocation + @"\" + "Results";

            Directory.CreateDirectory(workingResultsFolder);

            string XYDataFolder = workingResultsFolder + @"\" + "XYDataWriter";

            Directory.CreateDirectory(XYDataFolder);

            string WorkingParametersFolder = workerLocation + @"\" + "WorkingParameters";

            Directory.CreateDirectory(WorkingParametersFolder);

            string AllignmentFolder = WorkingParametersFolder + @"\" + "AllignmentInfo";

            Directory.CreateDirectory(AllignmentFolder);

            string LogsFolder = WorkingParametersFolder + @"\" + "Logs";

            Directory.CreateDirectory(LogsFolder);

            string WorkingApplicationFolder = workerLocation + @"\" + @"GlyQ-IQ Application\Release";

            Directory.CreateDirectory(WorkingApplicationFolder);

            string WorkingLaunchConsoleFolder = workerLocation + @"\" + @"GlyQ-IQ Application Setup";

            Directory.CreateDirectory(WorkingLaunchConsoleFolder);

            string WorkingDivideTargetsFolder = workerLocation + @"\" + @"GlyQ-IQ DivideTargets";

            Directory.CreateDirectory(WorkingDivideTargetsFolder);

            string WorkingPostProcessingFolder = workerLocation + @"\" + @"GlyQ-IQ PostProcessing";

            Directory.CreateDirectory(WorkingPostProcessingFolder);

            string WorkingMultiSleepFolder = workerLocation + @"\" + @"GlyQ-IQ MultiSleep";

            Directory.CreateDirectory(WorkingMultiSleepFolder);

            string WorkingCheckFileFolder = workerLocation + @"\" + @"GlyQ-IQ CheckFile";

            Directory.CreateDirectory(WorkingCheckFileFolder);

            string WorkingDeleteFilesFolder = workerLocation + @"\" + @"GlyQ-IQ DeleteFiles";

            Directory.CreateDirectory(WorkingDeleteFilesFolder);

            string WorkingCrazyCopyFolder = workerLocation + @"\" + @"GlyQ-IQ CrazyFileCopy";

            Directory.CreateDirectory(WorkingCrazyCopyFolder);


            string LaunchFolder = masterLocation;
            Directory.CreateDirectory(LaunchFolder);


            StringListToDisk writer = new StringListToDisk();


            //workflow parameters
            string basicTargetedWorkflowParametersFileName = "BasicTargetedWorkflowParameters.xml";
            List<string> basicTargetedWorkflowParameters = SetupCloudUtilities.SetBasicTargetedWorkflowParameters();
            outputLocation = WorkingParametersFolder + @"\" + basicTargetedWorkflowParametersFileName;
            writer.toDiskStringList(outputLocation, basicTargetedWorkflowParameters);



            //GlyQ-IQ Parameters
            string GlyQ_IQFileName = "L_RunFile.xml";
            List<string> GlyQIQParameters = SetupCloudUtilities.SetBasicGlyQIQParameters(TargetsFilePath, WorkingParametersFolder, workingRawDataFolder, workingResultsFolder);
            outputLocation = WorkingParametersFolder + @"\" + GlyQ_IQFileName;
            writer.toDiskStringList(outputLocation, GlyQIQParameters);



            //allignment parameters
            string basicTargetedAlignmentWorkflowParametersFileName = "TargetedAlignmentWorkflowParameters.xml";
            List<string> targetedAlignmentWorkflowParametersData = SetupCloudUtilities.SetTargetedAlignmentWorkflowParameters();
            outputLocation = WorkingParametersFolder + @"\" + basicTargetedAlignmentWorkflowParametersFileName;
            writer.toDiskStringList(outputLocation, targetedAlignmentWorkflowParametersData);

            ///batch files


            //PIC_DeleteFiles.bat
            string PIC_DeleteFilesFileName = "PIC_DeleteFiles.bat";
            List<string> DeleteFilesData = new List<string>();
            DeleteFilesData.Add("if exist " + "\"" + workerLocation + "\"" + " rmdir /s /q " + "\"" + workerLocation + "\"" + @" /s/q");
            SetupCloudUtilities.WriteBatchFile(DeleteFilesData, PIC_DeleteFilesFileName, LaunchFolder);



            //PIC_RunGlyQConsole.bat
            string PIC_RunGlyQConsoleFileName = "PIC_RunGlyQConsole.bat";
            List<string> RunGlyQConsoleData = new List<string>();
            //RunGlyQConsoleData.Add("\"" + WorkingApplicationFolder + @"\IQGlyQ_Console.exe" + "\"");
            RunGlyQConsoleData.Add("Call " + WorkingParametersFolder + @"\RunMeThreads.bat");
            SetupCloudUtilities.WriteBatchFile(RunGlyQConsoleData, PIC_RunGlyQConsoleFileName, LaunchFolder);


            //PIC_PostProcessing.bat
            string PIC_PostProcessingFileName = "PIC_RunPostProcessing.bat";
            List<string> PostProcessingData = new List<string>();
            PostProcessingData.Add("\"" + PostProcessing100XPub100XSetupLocation + "\"" + " " + "\"" + WorkingParametersFolder + @"\LocksFolder" + "\"" + " " + "\"" + "LockController.txt" + "\"" + " " + "\"" + workerLocation + "\"");
            //reset slave driver
            PostProcessingData.Add("Call " + "\"" + nodeShareFolderLocationOnNode + nodeShareFolder + @"\" + @"ScottK\resetSlaveWhenDone.bat" + "\"");


            SetupCloudUtilities.WriteBatchFile(PostProcessingData, PIC_PostProcessingFileName, LaunchFolder);

            ////PIC_LockChecker.bat
            //string PIC_LockCheckerFileName = "PIC_LockChecker.bat";
            //List<string> LockCheckerData = new List<string>();
            //PostProcessingData.Add("\"" + SleepMultiExeLocation + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + "LockCheckerParameterFile.txt" + "\"");
            //WriteBatchFile(LockCheckerData, PIC_LockCheckerFileName, LaunchFolder);
            ////lock checker parameter file


            //PUB1000_CopyFiles.bat
            //string PUB100X_CopyFilesName = "PUB100X_CopyFiles.bat";
            //List<string> CopyFilesData = new List<string>();
            //CopyFilesData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + TargetsFilePath + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + TargetsFilePath + "\"" + @" /S");
            //CopyFilesData.Add("echo f | xcopy /Y " + "\"" + RawFolderHome + @"\" + RawFile + "\"" + " " + "\"" + workingRawDataFolder + @"\" + RawFile + "\"" + @" /S");
            //CopyFilesData.Add("echo D | xcopy /Y " + "\"" + ApplciationHome + "\"" + " " + "\"" + WorkingApplicationFolder + "\"" + @" /S");

            // WriteBatchFile(CopyFilesData, PUB100X_CopyFilesName, LaunchFolder);


            //PUB1000_CopyFiles.bat
            //string PUB100X_CopyLibrarysName = "PUB100X_CopyLibraryFiles.bat";
            //List<string> CopyLibraryData = new List<string>();
            //CopyLibraryData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + TargetsFilePath + "\"" + " " + "\"" + workingResultsFolder + @"\" + TargetsFilePath + "\"" + @" /S");
            //WriteBatchFile(CopyLibraryData, PUB100X_CopyLibrarysName, LaunchFolder);


            //POST PROCESSING SECTION
            //PUB1002_CopyResultBack.bat
            string CopyResultBackName = "PUB100X_CopyResultBack.bat";
            List<string> CopyResultBackData = new List<string>();
            CopyResultBackData.Add("MD " + "\"" + ResultsFutureHome + "\"");
            //copy locks
            CopyResultBackData.Add("echo D | xcopy /Y " + "\"" + WorkingParametersFolder + @"\LocksFolder" + "\"" + " " + "\"" + workingResultsFolder + @"\LocksFolder" + "\"" + @" /S");

            CopyResultBackData.Add("echo D | xcopy /Y " + "\"" + workingResultsFolder + "\"" + " " + "\"" + ResultsFutureHome + "\"" + @" /S");
            //CopyResultBackData.Add("echo f | xcopy /Y " + "\"" + workingResultsFolder + @"\" + "XYDataResults.zip" + "\"" + " " + "\"" + ResultsFutureHome + @"\" + "XYDataResults.zip" + "\"" + @" /S");
            //CopyResultBackData.Add("Pause");
            SetupCloudUtilities.WriteBatchFile(CopyResultBackData, CopyResultBackName, LaunchFolder);


            //DeleteXYDataFolder
            string DeleteXYDataName = "PUB100X_DeleteXYDataFolder.bat";
            List<string> XYFolderDeleteData = new List<string>();
            XYFolderDeleteData.Add(@"if exist " + "\"" + XYDataFolder + "\"" + " rmdir /s /q " + "\"" + XYDataFolder + "\"" + " /s/q");
            SetupCloudUtilities.WriteBatchFile(XYFolderDeleteData, DeleteXYDataName, LaunchFolder);


            ////RunMeThird.bat
            //string WorkflowFileName = "PIC_RunMeThird_" + PubName + ".bat";
            //List<string> WorkflowData = new List<string>();
            ////WorkflowData.Add("Call " + PUB100X_CopyFilesName);
            ////WorkflowData.Add("Call " + PUB100X_CopyLibrarysName);
            //WorkflowData.Add("Call " + writeFolder + @"\" + PIC_RunGlyQConsoleFileName);
            //WorkflowData.Add("Exit");
            ////WorkflowData.Add("Call " + writeFolder + @"\" + PIC_PostProcessingFileName);
            ////WorkflowData.Add("Pause");
            ////WorkflowData.Add("Call " + writeFolder + @"\" + DeleteXYDataName);
            ////WorkflowData.Add("Call " + writeFolder + @"\" + CopyResultBackName);
            ////WorkflowData.Add("Call " + writeFolder + @"\" + PIC_DeleteFilesFileName);
            ////WorkflowData.Add("Pause");
            //WriteBatchFile(WorkflowData, WorkflowFileName, LaunchFolder);



            //PIC_RunMeSeccondFromPub100X.bat
            string RunMeSeccond100XFileName = "PIC_RunMeSeccond_PUB100X.bat";
            List<string> RunMeSeccond100XData = new List<string>();

            RunMeSeccond100XData.Add("echo D | xcopy /Y " + "\"" + ZippedPub100XSetup + "\"" + " " + "\"" + workingZippedFolder + "\"" + @" /S");
            RunMeSeccond100XData.Add("echo D | xcopy /Y " + "\"" + ApplciationHomePub100XSetup + "\"" + " " + "\"" + WorkingLaunchConsoleFolder + "\"" + @" /S");
            RunMeSeccond100XData.Add("echo D | xcopy /Y " + "\"" + DivideTargetsHomePub100XSetup + "\"" + " " + "\"" + WorkingDivideTargetsFolder + "\"" + @" /S");
            RunMeSeccond100XData.Add("echo D | xcopy /Y " + "\"" + ApplciationHome + "\"" + " " + "\"" + WorkingApplicationFolder + "\"" + @" /S");
            RunMeSeccond100XData.Add("echo D | xcopy /Y " + "\"" + PostProcessingHomePub100XSetup + "\"" + " " + "\"" + WorkingPostProcessingFolder + "\"" + @" /S");
            RunMeSeccond100XData.Add("echo D | xcopy /Y " + "\"" + SleepMultiHomePub100XSetup + "\"" + " " + "\"" + WorkingMultiSleepFolder + "\"" + @" /S");
            RunMeSeccond100XData.Add("echo D | xcopy /Y " + "\"" + CheckFileHomePub100XSetup + "\"" + " " + "\"" + WorkingCheckFileFolder + "\"" + @" /S");
            RunMeSeccond100XData.Add("echo D | xcopy /Y " + "\"" + DeleteFilesExpCopyHomePub100XSetup + "\"" + " " + "\"" + WorkingDeleteFilesFolder + "\"" + @" /S");
            RunMeSeccond100XData.Add("echo D | xcopy /Y " + "\"" + CrazyFilesCopyHomePub100XSetup + "\"" + " " + "\"" + WorkingCrazyCopyFolder + "\"" + @" /S");

            RunMeSeccond100XData.Add("echo D | xcopy /Y " + "\"" + LaunchInitializion + @"\" + FolderHoldingElementsDataForOmics + "\"" + " " + "\"" + PNNLElementDataFolderONWorker + "\"" + @" /S");

            //copy application specific files first.  then run parameter setup.  then copy specific files
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + nodeShareFolderLocationOnNode + @"\" + nodeShareFolder + @"\" + ApplicationSetupParameters + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + ApplicationSetupParameters + "\"");//this is the worker version as setup by divide targets
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + nodeShareFolderLocationOnNode + @"\" + nodeShareFolder + @"\" + divideTargetsParametersName + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + divideTargetsParametersName + "\"");
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + nodeShareFolderLocationOnNode + @"\" + nodeShareFolder + @"\" + TargetsFilePath + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + TargetsFilePath + "\"");

            RunMeSeccond100XData.Add("\"" + Applciation100XPub100XSetupLocation + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + ApplicationSetupParameters + "\"");

            //raw file and peaks file
            for (int i = 1; i < cores+1;i++ )
            {
                RunMeSeccond100XData.Add("if exist " + "\"" + RawFolderHome + @"\" + peaksFileName + "\"" + " " + "echo f | xcopy /Y " + "\"" + RawFolderHome + @"\" + peaksFileName + "\"" + " " + "\"" + workingRawDataFolder + @"\" + parameters.DataFileFileName + "_" + i + "_peaks.txt" + "\"");
            }
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + RawFolderHome + @"\" + rawFileName + "\"" + " " + "\"" + workingRawDataFolder + @"\" + rawFileName + "\"");
            
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LaunchInitializion + @"\" + ZipParameterFile + "\"" + " " + "\"" + workerLocation + @"\" + ZipParameterFile + "\"");
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LaunchInitializion + @"\" + PIC_RunGlyQConsoleFileName + "\"" + " " + "\"" + workerLocation + @"\" + PIC_RunGlyQConsoleFileName + "\"");
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LaunchInitializion + @"\" + PIC_DeleteFilesFileName + "\"" + " " + "\"" + workerLocation + @"\" + PIC_DeleteFilesFileName + "\"");
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LaunchInitializion + @"\" + PIC_PostProcessingFileName + "\"" + " " + "\"" + workerLocation + @"\" + PIC_PostProcessingFileName + "\"");

            //RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + TargetsFilePath + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + TargetsFilePath + "\"" + @" /S");
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + processingParameters + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + processingParameters + "\"");
            //RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + divideTargetsParameters + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + divideTargetsParameters + "\"" + @" /S");
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + specificParameters + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + specificParameters + "\"");
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + factors + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + factors + "\"");
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + randomIQFile + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + randomIQFile + "\"");
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + multiSleepParameterFile + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + multiSleepParameterFile + "\"");
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + GoCrazyCopyParameterFile + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + GoCrazyCopyParameterFile + "\"");

            //RunPostProcessing
            //RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + CopyResultBackName + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + CopyResultBackName + "\"");
            //RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + DeleteXYDataName + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + DeleteXYDataName + "\"");

            //RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + ApplicationSetupParametersWorker + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + ApplicationSetupParametersWorker + "\"" + @" /S");

            //"FragmentedTargetedWorkflowParameters_Velos_DH.txt" "Parameters_DivideTargetsPIC.txt"
            //we are making a file in the node folder that contains all the node information so we need to pull Parameters_DivideTargetsPIC from the node folder to the normal run folder on the node.  same for the targets file
            //we are recopying the files to get the node specific files
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + nodeShareFolderLocationOnNode + @"\" + nodeShareFolder + @"\" + divideTargetsParametersName + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + divideTargetsParametersName + "\"");
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + nodeShareFolderLocationOnNode + @"\" + nodeShareFolder + @"\" + TargetsFilePath + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + TargetsFilePath + "\"");

            RunMeSeccond100XData.Add("\"" + DivideTargets100XPub100XSetupLocation + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + divideTargetsParametersName + "\"");

            RunMeSeccond100XData.Add("Call " + workerLocation + @"\" + PIC_RunGlyQConsoleFileName);//from run me third.  launched before multisleep
            RunMeSeccond100XData.Add("\"" + SleepMultiExeLocation + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + multiSleepParameterFile + "\""+ " " + 1);//post processing is launched from here
            //RunMeSeccond100XData.Add("Pause");
            SetupCloudUtilities.WriteBatchFile(RunMeSeccond100XData, RunMeSeccond100XFileName, LaunchFolder);



            //PIC_NodeSetupPrep.bat
            string PIC_NodeSetupPrepName = "PIC_NodeSetup.bat";
            List<string> PIC_NodeSetupPrepData = new List<string>();
            //the library needs to be copied from picFS to library home first
            PIC_NodeSetupPrepData.Add("\"" + DivideTargetsNodeExeLocation + "\"" + " " + "\"" + LibraryHome + @"\" + divideTargetsParametersName + "\"" + " " + "\"" + LibraryHome + @"\" + divideTargetsParametersNode + "\"");
            //PIC_NodeSetupPrepData.Add("\"" + DivideTargetsNodeExeLocation + "\"" + " " + "\"" + LibraryOnPICFS + @"\" + divideTargetsParameters + "\"" + " " + "\"" + LibraryOnPICFS + @"\" + divideTargetsParametersNode + "\"");

            //PIC_NodeSetupPrepData.Add("Pause");
            SetupCloudUtilities.WriteBatchFile(PIC_NodeSetupPrepData, PIC_NodeSetupPrepName, LaunchFolder);



            string PIC_NodeConsolidationFileName = "PIC_NodeConsolidation.bat";
            List<string> NodeConsolidationData = new List<string>();
            string lineTime = "\"" + @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ Timer\Release\WriteTime.exe" + "\"" + " " + "\"" + @"E:\ScottK\Shared_PICFS\ToPIC\PubFinal_Time.txt" + "\"";
            NodeConsolidationData.Add(lineTime);
            //string line = "E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ CombineNodeResults\Release\CombineNodeResults.exe" "E:\ScottK\Shared_PICFS\ToPIC\WorkingParameters\ConsolidationParameterForAllNodes.txt"
            string line = "\"" + CombineNodeResultsExeLocation + "\"" + " " + "\"" + LibraryHome + @"\" + ConsolidationAtNodeLevelList + "\"";
            NodeConsolidationData.Add(line);
            SetupCloudUtilities.WriteBatchFile(NodeConsolidationData, PIC_NodeConsolidationFileName, LaunchFolder);




            ////PIC_SendJob.bat
            //string PIC_SendJobFileName = "PIC_SendJob_" + NodeName + ".bat";
            //List<string> SendJobFileNameData = new List<string>();
            ////node name needs to be hard coded so it know where to go
            //SendJobFileNameData.Add("echo f | xcopy /Y " + "\"" + Pub1000HomeFolder + @"\" + RunMeSeccond100XFileName + "\"" + " " + "\"" + @"\\" + NodeName + @"\" + nodeShareFolder + @"\" + RunMeSeccond100XFileName + "\"" + @" /S");
            //SendJobFileNameData.Add(PexecLocation + @" \\" + ipAddressForExecution + " -u " + ipAddressForExecution + @"\" + adminAccount + " -p " + adminPassword + " " + "\"" + nodeShareFolderLocationOnNode + @"\" + nodeShareFolder + @"\" + RunMeSeccond100XFileName + "\"");
            ////sleep and wait for the job to finish on the slave computers
            //SendJobFileNameData.Add("\"" + SleepExeLocation + "\"" + " " + "\"" + Pub1000HomeFolder + @"\" + "WorkingParameters" + @"\" + sleepParameterFile + "\"");
            //SendJobFileNameData.Add("Pause");
            //WriteBatchFile(SendJobFileNameData, PIC_SendJobFileName, LaunchFolder);



            //PIC_RunMeFirstFromPub1000.bat
            string RunMeFirstFileName = "PIC_RunMeFirst_PUB1000.bat";
            List<string> RunMeFirstData = new List<string>();
            //RunMeFirstData.Add(@"if exist "F:\ScottK" rmdir /s /q "F:\ScottK" /s/q")
            RunMeFirstData.Add(@"if exist " + "\"" + RunMeFirstFromPub1000Setup + "\"" + " rmdir /s /q " + "\"" + RunMeFirstFromPub1000Setup + "\"" + " /s/q");
            RunMeFirstData.Add(@"if exist " + "\"" + RunMeFirstFromPub1000Application + "\"" + " rmdir /s /q " + "\"" + RunMeFirstFromPub1000Application + "\"" + " /s/q");
            RunMeFirstData.Add(@"if exist " + "\"" + RunMeFirstFromPub1000divideTargets + "\"" + " rmdir /s /q " + "\"" + RunMeFirstFromPub1000divideTargets + "\"" + " /s/q");
            RunMeFirstData.Add(@"if exist " + "\"" + RunMeFirstFromPub1000postProcessing + "\"" + " rmdir /s /q " + "\"" + RunMeFirstFromPub1000postProcessing + "\"" + " /s/q");
            RunMeFirstData.Add(@"if exist " + "\"" + RunMeFirstFromPub1000Sleep + "\"" + " rmdir /s /q " + "\"" + RunMeFirstFromPub1000Sleep + "\"" + " /s/q");
            RunMeFirstData.Add(@"if exist " + "\"" + RunMeFirstFromPub1000SleepMulti + "\"" + " rmdir /s /q " + "\"" + RunMeFirstFromPub1000SleepMulti + "\"" + " /s/q");

            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSZipped + "\"" + " " + "\"" + RunMeFirstFromPub1000Zipped + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSSetup + "\"" + " " + "\"" + RunMeFirstFromPub1000Setup + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSApplication + "\"" + " " + "\"" + RunMeFirstFromPub1000Application + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSdivideTargets + "\"" + " " + "\"" + RunMeFirstFromPub1000divideTargets + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSpostProcessing + "\"" + " " + "\"" + RunMeFirstFromPub1000postProcessing + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSSleep + "\"" + " " + "\"" + RunMeFirstFromPub1000Sleep + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSSleepMulti + "\"" + " " + "\"" + RunMeFirstFromPub1000SleepMulti + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSCheckFile + "\"" + " " + "\"" + RunMeFirstFromPub1000CheckFile + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSDeleteFilesExpCopy + "\"" + " " + "\"" + RunMeFirstFromPub1000DeleteFilesExpCopy + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSCrazyFilesCopy + "\"" + " " + "\"" + RunMeFirstFromPub1000CrazyFilesCopy + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSDivideTargetsNode + "\"" + " " + "\"" + RunMeFirstFromPub1000DivideTargetsNode + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSCombineNodeResults + "\"" + " " + "\"" + RunMeFirstFromPub1000CombineNodeResults + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSTimer + "\"" + " " + "\"" + RunMeFirstFromPub1000Timer + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSZippedApp + "\"" + " " + "\"" + RunMeFirstFromPub1000ZippedApp + "\"" + @" /S");

            //RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + ApplciationHomePub100XSetupHome + @"\" + RunMeSeccond100XFileName + "\"" + " " + "\"" + LaunchInitializion + @"\" + RunMeSeccond100XFileName + "\"" + @" /S");
            //RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + ApplciationHomePub100XSetupHome + @"\" + PIC_NodeSetupPrepName + "\"" + " " + "\"" + LaunchInitializion + @"\" + PIC_NodeSetupPrepName + "\"" + @" /S");
            //RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + ApplciationHomePub100XSetupHome + @"\" + ZipBatchName + "\"" + " " + "\"" + LaunchInitializion + @"\" + ZipBatchName + "\"" + @" /S");
            //RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + ApplciationHomePub100XSetupHome + @"\" + PIC_NodeConsolidationFileName + "\"" + " " + "\"" + LaunchInitializion + @"\" + PIC_NodeConsolidationFileName + "\"" + @" /S");
            //RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + ApplciationHomePub100XSetupHome + @"\" + ZipParameterFile + "\"" + " " + "\"" + LaunchInitializion + @"\" + ZipParameterFile + "\"" + @" /S");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + ApplciationHomePub100XSetupHome + @"\" + LaunchJobsBatchFileName + "\"" + " " + "\"" + LaunchInitializion + @"\" + LaunchJobsBatchFileName + "\"");

            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + ApplciationHomePub100XSetupHome + @"\" + PIC_NodeSetupPrepName + "\"" + " " + "\"" + LaunchInitializion + @"\" + PIC_NodeSetupPrepName + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + ApplciationHomePub100XSetupHome + @"\" + testBatchName + "\"" + " " + "\"" + LaunchInitializion + @"\" + testBatchName + "\"");
            //RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + ApplciationHomePub100XSetupHome + @"\" + PIC_SendJobBackFileName + "\"" + " " + "\"" + LaunchInitializion + @"\" + PIC_SendJobBackFileName + "\"" + @" /S");

            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + TargetsFilePath + "\"" + " " + "\"" + LibraryHome + @"\" + TargetsFilePath + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + divideTargetsParametersName + "\"" + " " + "\"" + LibraryHome + @"\" + divideTargetsParametersName + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + processingParameters + "\"" + " " + "\"" + LibraryHome + @"\" + processingParameters + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + specificParameters + "\"" + " " + "\"" + LibraryHome + @"\" + specificParameters + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + factors + "\"" + " " + "\"" + LibraryHome + @"\" + factors + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + randomIQFile + "\"" + " " + "\"" + LibraryHome + @"\" + randomIQFile + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + sleepParameterFile + "\"" + " " + "\"" + LibraryHome + @"\" + sleepParameterFile + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + multiSleepParameterFile + "\"" + " " + "\"" + LibraryHome + @"\" + multiSleepParameterFile + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + GoCrazyCopyParameterFile + "\"" + " " + "\"" + LibraryHome + @"\" + GoCrazyCopyParameterFile + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + divideTargetsParametersNode + "\"" + " " + "\"" + LibraryHome + @"\" + divideTargetsParametersNode + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + ApplicationSetupParameters + "\"" + " " + "\"" + LibraryHome + @"\" + ApplicationSetupParameters + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + ApplicationSetupParametersWorker + "\"" + " " + "\"" + LibraryHome + @"\" + ApplicationSetupParametersWorker + "\"");

            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + globalMultiSleepFile + "\"" + " " + "\"" + LibraryHome + @"\" + globalMultiSleepFile + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + globalMultiSleepFileListFile + "\"" + " " + "\"" + LibraryHome + @"\" + globalMultiSleepFileListFile + "\"");
            //RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + ZipParameterFile + "\"" + " " + "\"" + LibraryHome + @"\" + ZipParameterFile + "\"" + @" /S");

            //run the node divider
            //RunMeFirstData.Add("Call " + @"E:\ScottK\Shared_PICFS\ToPIC\PIC_NodeSetup.bat");
            //RunMeFirstData.Add("\"" + @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ Application Setup\Release\IQGlyQ_Console_ParameterSetup.exe" + "\"" + " " + "\"" + LibraryHome + @"\" + ApplicationSetupParameters + "\"");

            //RunMeFirstData.Add("Call " + @"E:\ScottK\Shared_PICFS\ToPIC\Zip.bat");
            // CopyFiles needed raw file to pub1000
            //RunMeFirst.Add("echo f | xcopy /Y " + "\"" + RawFolderHomePICFS + @"\" + RawFile + "\"" + " " + "\"" + RawFolderHome + @"\" + RawFile + "\"" + @" /S");
            SetupCloudUtilities.WriteBatchFile(RunMeFirstData, RunMeFirstFileName, LaunchFolder);









            ////write sleep file
            //string sleepFileName = "PIC_SleepParameterFile_" + NodeName + ".txt";
            //string sleepFilePath = WorkingParametersFolder + @"\" + sleepFileName;
            //string fileTOwaitFor = ThisIsWhatItTakesToFindFinalResultsFile(LibraryHome, divideTargetsParameters);
            //string lineFileToWaitFor = "FileToWaitFor," + Pub1000HomeFolder + @"\" + "Results" + @"\" + NodeName + "_Results" + @"\" + fileTOwaitFor;
            //string lineBatchFileToRunAfterLoop = "BatchFileToRunAfterLoop," + Pub1000HomeFolder + @"\" + "testBatch.bat";//we need line 1 from line 0 in the controller
            //string lineWorkingFolder = "WorkingFolder," + Pub1000HomeFolder + @"\" + "WorkingParameters";;
            //string lineSeconds = "Seconds," + 20;

            //List<string> linesForSleepParameteFile = new List<string>();
            //linesForSleepParameteFile.Add(lineFileToWaitFor);
            //linesForSleepParameteFile.Add(lineBatchFileToRunAfterLoop);
            //linesForSleepParameteFile.Add(lineWorkingFolder);
            //linesForSleepParameteFile.Add(lineSeconds);

            //writer.toDiskStringList(sleepFilePath, linesForSleepParameteFile);

            //write parameters launch files from node file

            //      string nodeFilePath = @"F:\ScottK\ToPIC\WorkingParameters" + @"\" + sleepFileName;

            //string lineFileToWaitFor = "FileToWaitFor," + Pub1000HomeFolder + @"\" + "Results" + @"\" + NodeName + "_Results" + @"\" + fileTOwaitFor;


        }

    }
}
