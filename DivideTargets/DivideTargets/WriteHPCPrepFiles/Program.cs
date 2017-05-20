using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DivideTargetsLibrary.Parameters;
using IQGlyQ.Objects;
using GetPeaksDllLite.DataFIFO;

namespace WriteHPCPrepFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Checking for Files needed..." + Environment.NewLine);
            
            //inputs
            //int cores = 800;
            //string workingDirectory = @"\\picfs\projects\DMS\PIC_HPC\Home";
            //string datasetFileName = "Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12";
            //string workingDirectoryForExe = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC";
            //string targets = "L_10_IQ_TargetsFirstAll_R";
            //string factorsName = "Factors_L10.txt";
            //string executorParameterFile = "SN111-SN117_OrbiStandard_workflowExecutorParametersSK_Work HM Test.xml";
            //string IQParameterFile = "FragmentedTargetedWorkflowParameters_Velos_DH.txt";


            ////parameter files to make
            //string makeResultsListName = "HPC_MakeResultsList_Asterisks.bat";
            //string divideTargetsParameterFile = "HPC-Parameters_DivideTargetsPIC_Asterisks.txt";
            //string consoleOperatingParameters = @"WorkingParameters\GlyQIQ_Diabetes_Parameters_PICFS.txt";

            HPCPrepParameters parameters = new HPCPrepParameters();
            //parameters.Write(@"\\picfs\projects\DMS\PIC_HPC\Home\0y_HPC_OperationParameters.txt");
            //parameters.LoadParameters(@"\\picfs\projects\DMS\PIC_HPC\Home\0y_HPC_OperationParameters.txt");
            parameters.LoadParameters(args[0]);
            
            Console.WriteLine("Parameters Loaded...");

            string datasetType = ".raw";
            bool deleteLogs = false;

            string workingLocation = Path.Combine(parameters.WorkingDirectory, parameters.WorkingFolder);

            //from here we should be able to check all files needed to run
            

            //folder working directory
            bool test1 = CheckDirectoryLocationOnDisk(workingLocation, 1, workingLocation);

            //folder with parameters
            bool test2 = CheckDirectoryLocationOnDisk(Path.Combine(workingLocation, "WorkingParameters"), 2, "WorkingParameters");

            //location of raw data file
            bool test3 = CheckDirectoryLocationOnDisk(parameters.DataSetDirectory, 3, "DataSetDirectory");

            //actual raw file
            //bool test4 = CheckFileLocationOnDisk(parameters.DataSetDirectory + @"\" + parameters.DatasetFileNameNoEnding + "_1" + datasetType,4);
            bool test4 = CheckFileLocationOnDisk(Path.Combine(parameters.DataSetDirectory, parameters.DatasetFileNameNoEnding + datasetType), 4, parameters.DatasetFileNameNoEnding + datasetType);

            //place to launch applications from
            bool test5 = CheckDirectoryLocationOnDisk(parameters.WorkingDirectoryForExe, 5, "WorkingDirectoryForExe");

            //actual targets file
            bool test6 = CheckFileLocationOnDisk(Path.Combine(workingLocation, "WorkingParameters", parameters.TargetsNoEnding + ".txt"), 6, parameters.TargetsNoEnding + ".txt");

            //actual factors file that goes with the targets
            bool test7 = CheckFileLocationOnDisk(Path.Combine(workingLocation, "WorkingParameters", parameters.FactorsName), 7, parameters.FactorsName);

            //actual executor parameters
            bool test8 = CheckFileLocationOnDisk(Path.Combine(workingLocation, "WorkingParameters", parameters.ExecutorParameterFile), 8, parameters.ExecutorParameterFile);

            //actual IQ workflow parameters
            bool test9 = CheckFileLocationOnDisk(Path.Combine(workingLocation, "WorkingParameters", parameters.IQParameterFile), 9, parameters.IQParameterFile);

            //actual file listing files to be consolidated
            bool test10 = CheckFileLocationOnDisk(Path.Combine(workingLocation, parameters.makeResultsListName + "_" + parameters.DatasetFileNameNoEnding + ".bat"), 10, parameters.makeResultsListName + "_" + parameters.DatasetFileNameNoEnding + ".bat"); 
            if(test10==false)
            {
                Console.WriteLine(" - this will will be created by this application" + Environment.NewLine);
            }

            //actual parameter file for breaking up the problem
            bool test11 = CheckFileLocationOnDisk(Path.Combine(workingLocation, parameters.divideTargetsParameterFile), 11, parameters.divideTargetsParameterFile);
            if (test11 == false)
            {
                Console.WriteLine(" - this will will be created by this application" + Environment.NewLine);
            }

            //IQ operating parameters for iq
            bool test12 = CheckFileLocationOnDisk(Path.Combine(workingLocation, "WorkingParameters", parameters.consoleOperatingParameters), 12, parameters.consoleOperatingParameters);//GlyQIQ_Diabetes_Parameters_PICFS
            bool test12b = false;
            if (test12)
            {
                //read in GlyQIQ_Diabetes_Parameters_PICFS_SPIN_SN129.txt and make sure it links to FragmentedTargetedWorkflowParameters_Velos_DH.txt(IQParameterFile)
                var parametersCheck = new IQGlyQ.Objects.ParametersIQGlyQ();
                parametersCheck.SetParameters(Path.Combine(workingLocation, "WorkingParameters",parameters.consoleOperatingParameters));
                string fragmentedWorkflowParamaterFile = parametersCheck.fragmentWorkFlowParameters;
                test12b = CheckFileLocationOnDisk(Path.Combine(workingLocation, "WorkingParameters" , fragmentedWorkflowParamaterFile), 12, fragmentedWorkflowParamaterFile);
            }


            //HPC launcher batch file.  this will be created if it does not exist
            bool test13 = CheckFileLocationOnDisk(Path.Combine(workingLocation, parameters.scottsFirstHPLCLauncher + "_" + parameters.DatasetFileNameNoEnding + ".bat"), 13, parameters.scottsFirstHPLCLauncher + "_" + parameters.DatasetFileNameNoEnding + ".bat");
            if (test13 == false)
            {
                Console.WriteLine(" - this will will be created by this application" + Environment.NewLine);
            }
            //sleep parameters
            bool test14 = CheckFileLocationOnDisk(Path.Combine(workingLocation, "WorkingParameters", parameters.HPC_MultiSleepParameterFileGlobal + "_" + parameters.DatasetFileNameNoEnding + ".txt"), 14, parameters.HPC_MultiSleepParameterFileGlobal + "_" + parameters.DatasetFileNameNoEnding + ".txt");
            if (test14 == false)
            {
                Console.WriteLine(" - this will will be created by this application" + Environment.NewLine);
            }

            //Actual exe location for HPC runs
            bool test15 = CheckFileLocationOnDisk(Path.Combine(parameters.WorkingDirectoryForExe, "GlyQ-IQ_Application", "Release", "IQGlyQ_Console.exe"), 15, "IQGlyQ_Console.exe");
            if (test15 == false)
            {
                Console.WriteLine(" - Error: this file is required" + Environment.NewLine);
            }

            //thermo dll setup folder
            bool test16 = CheckDirectoryLocationOnDisk(Path.Combine(workingLocation, "RemoteThermo"), 16, "RemoteThermo");
            if (test16 == false)
            {
                Console.WriteLine("Prep task of installing thermo dll will not work because folder is not there 16" + Environment.NewLine);
            }

            if (test1 && test2 && test3 && test4 && test5 && test6 && test7 && test8 && test9 && test10 && test11 && test12 && test12b && test13 && test14 && test15 && test16)
            {
                Console.WriteLine("All Parameter Files Present");
            }
            else
            {
                if (test1 && test2 && test3 && test4 && test5 && test6 && test7 && test8 && test9 && test12 && test12b && test15 && test16)
                    Console.WriteLine("Missing one or more parameter files, but they will be auto-created next");
                else
                    Console.WriteLine("Missing one or more parameter files (and they cannot be auto-created)");
            }

            var writer = new StringListToDisk();
            string q = "\"";

            #region DivideTargets Parameters (1.  HPC-Parameters_DivideTargetsPIC_Asterisks.txt")

            ParameterDivideTargets divideParameters = new ParameterDivideTargets
            {
	            TargetsFileName = parameters.TargetsNoEnding + ".txt",
	            TargetsFileFolder = Path.Combine(workingLocation, "WorkingParameters"),
	            DataFileFolder = Path.Combine(workingLocation, "RawData"),
	            DataFileFileName = parameters.DatasetFileNameNoEnding,
	            DataFileEnding = "raw",
	            FactorsFileName = parameters.FactorsName,
	            CoresString = parameters.MaxTargetNumber.ToString(CultureInfo.InvariantCulture),
	            AppIqGlyQConsoleLocationPath = Path.Combine(parameters.WorkingDirectoryForExe, @"GlyQ-IQ_Application\Release\IQGlyQ_Console.exe"),
	            OutputLocationPath = Path.Combine(workingLocation, "WorkingParameters"),
	            GlyQIQparameterFile = parameters.consoleOperatingParameters,
	            LockController = "LockController.txt",
	            LockControllerDone = "LockController_Done.txt",
	            WriteFolder = workingLocation,
	            DeleteXYDataName = "PUB100X_DeleteXYDataFolder.bat",
	            CopyResultBackName = "PUB100X_CopyResultBack.bat",
	            PIC_DeleteFilesFileName = "PIC_DeleteFiles.bat",
	            DuplicateDataBool = "FALSE"
            };

	        divideParameters.WriteParameters(Path.Combine(workingLocation, parameters.divideTargetsParameterFile));

            #endregion

            #region HPCListCombineMaker Parameters (2.  HPC_MakeResultsList_Asterisks_Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.bat)

            string HPC_MakeResultsList_AsterisksName = parameters.makeResultsListName + "_" + parameters.DatasetFileNameNoEnding + ".bat";
            
            List<string> lines = new List<string>();

            string makeResultsExeLocation = Path.Combine(parameters.WorkingDirectoryForExe, @"GlyQ-IQ_HPCListCombineMaker\Release\CombineListMaker.exe");
            int minRange = 1;
            //int maxRange = parameters.cores;
            int maxRange = parameters.MaxTargetNumber;
            string HPCResultsListFilePath = @"WorkingParameters\HPC_ResultList_" + parameters.DatasetFileNameNoEnding + ".txt";

            string fullLine =
                q + makeResultsExeLocation + q + " " +
                parameters.DatasetFileNameNoEnding + " " +
                q + ".txt" + q + " " +
                minRange + " " +
                maxRange + " " +
                q + Path.Combine(workingLocation, HPCResultsListFilePath) + q + " " +
                q + Path.Combine(workingLocation, @"WorkingParameters\HPC_ConsolidationParameterForAllNodes_" + parameters.DatasetFileNameNoEnding + ".txt") + q + " " +
                q + workingLocation + q;

            lines.Add(fullLine);

            writer.toDiskStringList(Path.Combine(workingLocation, HPC_MakeResultsList_AsterisksName), lines);

            #endregion

            #region GlyQIQ console Parameters (3.  GlyQIQ_Diabetes_Parameters_PICFS_D10.txt)

            IQGlyQ.Objects.ParametersIQGlyQ consoleParameters = new ParametersIQGlyQ();
            consoleParameters.resultsFolderPath = Path.Combine(workingLocation, "Results");
            consoleParameters.loggingFolderPath = Path.Combine(workingLocation, "Results");
            consoleParameters.factorsFile = parameters.FactorsName;
            consoleParameters.executorParameterFile = parameters.ExecutorParameterFile;
            consoleParameters.XYDataFolder = "XYDataWriter";
            consoleParameters.fragmentWorkFlowParameters = parameters.IQParameterFile;
            consoleParameters.TargetedAlignmentWorkflowParameters = Path.Combine(workingLocation, @"WorkingParameters\TargetedAlignmentWorkflowParameters.xml");
            consoleParameters.BasicTargetedWorkflowParameters = Path.Combine(workingLocation, @"WorkingParameters\BasicTargetedWorkflowParameters.xml");

            consoleParameters.WriteParameters(Path.Combine(workingLocation, parameters.consoleOperatingParameters));

            #endregion

            #region HPC_ScottsFirstHPLCLauncher.bat Parameters


            List<string> linesHPCLaunch = new List<string>();
            if (parameters.IsHPC == "true")
            {
                //string scottsFirstHPLCLauncher = @"D:\PNNL CSharp1\SVN HPC_JobCreator\HPC_JobCreator\ScottsFirstHPLCLauncher\bin\Release\ScottsFirstHPLCLauncher.exe";
                string scottsFirstHPLCLauncher = Path.Combine(parameters.WorkingDirectoryForExe, @"GlyQ-IQ_HPC_JobCreation\Release\ScottsFirstHPLCLauncher.exe");
                //string datatfile = parameters.DatasetFileNameNoEnding + "_1";
                string datatfile = parameters.DatasetFileNameNoEnding;
                string targets = parameters.TargetsNoEnding;
                int cores = parameters.cores;
                string workingDirectory = workingLocation;
                string workingDirectoryIP = parameters.ipaddress;
                string workingDirectoryIPLog = parameters.LogIpaddress;
                string consoleParametersForCMD = parameters.consoleOperatingParameters;
                string exepathdirectory = parameters.WorkingDirectoryForExe;
                string workerNodeGroup = parameters.HPCNodeGroupName;
                string clusterName = parameters.ClusterName;//"Deception2.pnnl.gov"
                string templateName = parameters.TemplateName;
	            string hpcUserName = parameters.PICHPCUsername;
				string hpcPassword = parameters.PICHPCPassword;

                int maxTargetNumber = Convert.ToInt32(parameters.MaxTargetNumber);

                string fullLineHPC =
                    q + scottsFirstHPLCLauncher + q + " " +
                    datatfile + " " +
                    targets + " " +
                    cores + " " +
                    q + workingDirectory + q + " " +
                    consoleParametersForCMD + " " +
                    q + workingDirectoryIP + q + " " +
                    q + workingDirectoryIPLog + q + " " +
                    q + exepathdirectory + q + " " +
                    q + clusterName + q + " " +
                    q + templateName + q + " " +
                    maxTargetNumber + " " +
					q + hpcUserName + q + " " +
					q + hpcPassword + q;

                linesHPCLaunch.Add(fullLineHPC);
                
            }
            else
            {
                linesHPCLaunch.Add("rem To use fewer (or more) cores, change the Cores parameter in 0y_HPC_OperationParameters_1240183582.txt");
                linesHPCLaunch.Add("rem Then add/delete the following batch file calls");
                linesHPCLaunch.Add("");
                for(int i=1;i<parameters.cores+1;i++)
                {
                    string coreString = q  + "Core" + i + q;
                    linesHPCLaunch.Add("Start " + coreString + " " + q + @"%1\Core" + i + ".bat" + q + " " + @"%" + 1);
                }

                //write core files
                for (int i = 1; i < parameters.cores + 1; i++)
                {
                    List<string> linesCores = new List<string>();
                    string coreFileName = "Core" + i + ".bat";

                    string word1 = @"%1\ApplicationFiles\GlyQ-IQ_Application\Release\IQGlyQ_Console.exe";
                    string word2 = @"%1\RawData";
                    string word3 = parameters.DatasetFileNameNoEnding; //ESI_SN138_21Dec_C15_2530_1
                    string word4 = parameters.DatasetFileExtenstion;//raw
                    string word5 = parameters.TargetsNoEnding + "_" + i + ".txt";
                    string word6 = parameters.consoleOperatingParameters;//GlyQIQ_Params_Velos4_SN138_L10PSA.txt
                    string word7 = @"%1\WorkingParameters";
                    string word8 = "Lock_" + i;
                    string word9 = @"%1\Results\Results";  //%1\Results\Results
                    string word10 = i.ToString();
                    string s = " ";

                    linesCores.Add(word1 + s + word2 + s + word3 + s + word4 + s + word5 + s + word6 + s + word7 + s + word8 + s + word9 + s + word10);

                    linesCores.Add("Exit");
                    writer.toDiskStringList(Path.Combine(workingLocation, coreFileName), linesCores);

                }
            }
            writer.toDiskStringList(Path.Combine(workingLocation, parameters.scottsFirstHPLCLauncher + "_" + parameters.DatasetFileNameNoEnding + ".bat"), linesHPCLaunch);
            


            
            
          


            #endregion

            #region 1_HPC StartCollectingResults and associated HPCMultiSleepParametersFile

            //step 1.  write 1_HPC StartCollectingResults bat file
            
            //string HPC_StartCollectResultsBatName = "1_HPC_StartCollectResults" + "_" + parameters.DatasetFileNameNoEnding + "_1" + ".bat";//the _1 is needed for the direct raw file
            string HPC_StartCollectResultsBatName = "1_HPC_StartCollectResults" + "_" + parameters.DatasetFileNameNoEnding + ".bat";//the _1 is needed for the direct raw file
            
            string stopwatchFileName = "HPC_Stopwatch" + "_" + parameters.DatasetFileNameNoEnding + ".txt";
            string HPCMultiSleepParametersFileName = "HPC_MultiSleepParameterFileGlobal" + "_" + parameters.DatasetFileNameNoEnding + ".txt";

            List<string> linesHStartCollecting = new List<string>();

            linesHStartCollecting.Add(q + Path.Combine(parameters.WorkingDirectoryForExe, @"GlyQ-IQ_Timer\Release\WriteTime.exe") + q + " " + q + Path.Combine(workingLocation, stopwatchFileName) + q);
            //linesHStartCollecting.Add(q + @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ Timer\Release\WriteTime.exe" +q + " " + q +  @"\\picfs\projects\DMS\PIC_HPC\Home\" + stopwatchFileName + q);

            linesHStartCollecting.Add("Call" + " " + Path.Combine(workingLocation, HPC_MakeResultsList_AsterisksName));
            //linesHStartCollecting.Add("Call" + " " + @"\\picfs\projects\DMS\PIC_HPC\Home\" + HPC_MakeResultsList_AsterisksName);

            linesHStartCollecting.Add("Call" + " " + q + Path.Combine(parameters.WorkingDirectoryForExe, @"GlyQ-IQ_MultiSleep\Release\MultiSleep.exe") + q + " " + q + Path.Combine(workingLocation, "WorkingParameters", HPCMultiSleepParametersFileName) + q + " " + 1);
            //linesHStartCollecting.Add("Call" + " " + q + @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ MultiSleep\Release\MultiSleep.exe" + q + " " + q + @"\\picfs\projects\DMS\PIC_HPC\Home\WorkingParameters\" + HPCMultiSleepParametersFileName + q + " " + 1);

            writer.toDiskStringList(Path.Combine(workingLocation, HPC_StartCollectResultsBatName), linesHStartCollecting);

            //step 2. write custom HPCMultiSleepParametersFileName to go with 1_HPC StartCollectingResults
            string HPC_NodeConsolidationFileName = "HPC_NodeConsolidation" + "_" + parameters.DatasetFileNameNoEnding + ".bat";
            List<string> linesMultiSleepParameterFile = new List<string>();


            //linesMultiSleepParameterFile.Add(@"FileToWaitFor,\\picfs\projects\DMS\PIC_HPC\Home\WorkingParameters\HPC_ResultList.txt");
            //linesMultiSleepParameterFile.Add(@"BatchFileToRunAfterLoop,\\picfs\projects\DMS\PIC_HPC\Home\" + HPC_NodeConsolidationFileName);
            //linesMultiSleepParameterFile.Add(@"WorkingFolder,\\picfs\projects\DMS\PIC_HPC\Home\Results");
            //linesMultiSleepParameterFile.Add(@"Seconds,20");
            int secondsToWaitBeforeCheckingForCompletion = 120;//120;//20 is default but is short for many datasets

            linesMultiSleepParameterFile.Add(@"FileToWaitFor," + Path.Combine(workingLocation, @"WorkingParameters\HPC_ResultList_" + parameters.DatasetFileNameNoEnding + ".txt"));
            linesMultiSleepParameterFile.Add(@"BatchFileToRunAfterLoop," + Path.Combine(workingLocation, HPC_NodeConsolidationFileName));
            linesMultiSleepParameterFile.Add(@"WorkingFolder," + Path.Combine(workingLocation, "Results"));
            string secondsLine = @"Seconds," + secondsToWaitBeforeCheckingForCompletion;
            linesMultiSleepParameterFile.Add(secondsLine);

            writer.toDiskStringList(Path.Combine(workingLocation, "WorkingParameters", HPCMultiSleepParametersFileName), linesMultiSleepParameterFile);

            #endregion

            #region NODE CONSOLIDATION
            //setp 3 write new nodeconsolidation bat

            int digitsToCompareForSingleLinkage = 4;

            var lineForNodeConsolidation = new List<string>();
            //lineForNodeConsolidation.Add(@"pushd " + parameters.WorkingDirectoryForExe);
            lineForNodeConsolidation.Add(q + Path.Combine(parameters.WorkingDirectoryForExe, @"GlyQ-IQ_Timer\Release\WriteTime.exe") + q + " " + q + Path.Combine(workingLocation, stopwatchFileName) + q);
            lineForNodeConsolidation.Add(q + Path.Combine(parameters.WorkingDirectoryForExe, @"GlyQ-IQ_CombineNodeResults\Release\CombineNodeResults.exe") + q + " " + q + Path.Combine(workingLocation, @"WorkingParameters\HPC_ConsolidationParameterForAllNodes_" + parameters.DatasetFileNameNoEnding + ".txt"));
            lineForNodeConsolidation.Add(q + Path.Combine(parameters.WorkingDirectoryForExe, @"GlyQ-IQ_SingleLinkage\Release\SingleLinkage.exe") + q + " " + q + Path.Combine(workingLocation, "ResultsSummary", parameters.DatasetFileNameNoEnding + "_Global_iqResults.txt") + q + " " + q + Path.Combine(workingLocation, "ResultsSummary", parameters.DatasetFileNameNoEnding + "_Family_iqResults.txt") + q + " " + digitsToCompareForSingleLinkage);
            lineForNodeConsolidation.Add(q + Path.Combine(parameters.WorkingDirectoryForExe, @"GlyQ-IQ_Compositions\Release\CompositionConsolidation.exe") + q + " " + q + Path.Combine(workingLocation, "ResultsSummary", parameters.DatasetFileNameNoEnding + "_Family_iqResults.txt") + q + " " + q + Path.Combine(workingLocation, "ResultsSummary", parameters.DatasetFileNameNoEnding + "_Composition_iqResults.txt") + q);
            lineForNodeConsolidation.Add(q + Path.Combine(parameters.WorkingDirectoryForExe, @"GlyQ-IQ_ToGlycoGrid\Release\ResultsToGlycoGridX64.exe") + q + " " + q + Path.Combine(workingLocation, "ResultsSummary", parameters.DatasetFileNameNoEnding + "_Family_iqResults.txt") + q + " " + q + workingLocation + q + " " + parameters.DatasetFileNameNoEnding + " " + parameters.FactorsName + " " + q + Path.Combine(workingLocation, "WorkingParameters") + q);
            
            //lineForNodeConsolidation.Add(q + @"GlyQ-IQ_ToGlycoGrid\Release\ResultsToGlycoGridX64.exe" + q + " " + q + Path.Combine(workingLocation,  + "ResultsSummary" + @"\" + parameters.DatasetFileNameNoEnding + "_Global_iqResults.txt" + q + " " + q + workingLocation + q + " " + parameters.DatasetFileNameNoEnding + " " + parameters.FactorsName + " " + q +Path.Combine(workingLocation,  + "WorkingParameters" + q);
            //lineForNodeConsolidation.Add("popd");

            if (Convert.ToBoolean(parameters.FrankenDelete))
            {
                lineForNodeConsolidation.Add(Path.Combine(workingLocation, @"1x_FrankenDelete" + "_" + parameters.DatasetFileNameNoEnding + ".bat"));
                if (deleteLogs)
                {
                    lineForNodeConsolidation.Add(Path.Combine(workingLocation, @"2x_DeleteResultsFolder" + "_" + parameters.DatasetFileNameNoEnding + ".bat"));
                }
            }

            //lineForNodeConsolidation.Add(@"pushd \\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC");
            //lineForNodeConsolidation.Add(q + @"GlyQ-IQ Timer\Release\WriteTime.exe" + q + " " + q + @"\\picfs\projects\DMS\PIC_HPC\Home\" +stopwatchFileName + q);
            //lineForNodeConsolidation.Add(q + @"GlyQ-IQ CombineNodeResults\Release\CombineNodeResults.exe" + q + " " + q +  @"\\picfs\projects\DMS\PIC_HPC\Home\WorkingParameters\HPC_ConsolidationParameterForAllNodes.txt");
            //lineForNodeConsolidation.Add(q + @"GlyQ-IQ ToGlycoGrid\Release\ResultsToGlycoGrid.exe" + q + " " + q + @"\\picfs\projects\DMS\PIC_HPC\Home\" + parameters.datasetFileName + "_Global_iqResults.txt" + q + " " + q + @"\\picfs\projects\DMS\PIC_HPC\Home");
            //lineForNodeConsolidation.Add("popd");

            writer.toDiskStringList(Path.Combine(workingLocation, HPC_NodeConsolidationFileName), lineForNodeConsolidation);



            #endregion

            #region POST verify NodeConsolidation CHECK


            string HPC_PostNodeConsolidationFileName = "HPC_NodeConsolidation_Check" + "_" + parameters.DatasetFileNameNoEnding + ".bat";

            List<string> lineForPostVerifyNodeConsolidation = new List<string>();
            lineForPostVerifyNodeConsolidation.Add(@"pushd " + parameters.WorkingDirectoryForExe);
            lineForPostVerifyNodeConsolidation.Add(q + @"GlyQ-IQ_ConvertValidatedIQToChecked\Release\ConvertValidatedIQResultsToChecked.exe" + q + " " + parameters.DatasetFileNameNoEnding + " " + q + Path.Combine(workingLocation, "ResultsSummary") + q);
            lineForPostVerifyNodeConsolidation.Add("popd");

            lineForPostVerifyNodeConsolidation.Add(@"pushd " + @"F:\ScottK\ToPIC");
            //lineForPostVerifyNodeConsolidation.Add(@"pushd " + @"\\picfs\projects\DMS\PIC_HPC\Home\ApplicationFiles");
            lineForPostVerifyNodeConsolidation.Add(q + @"GlyQ-IQ_SingleLinkage\Release\SingleLinkage.exe" + q + " " + q + Path.Combine(workingLocation, "ResultsSummary", parameters.DatasetFileNameNoEnding + "_Family_iqResults_Check.txt") + q + " " + q + Path.Combine(workingLocation, "ResultsSummary", parameters.DatasetFileNameNoEnding + "_Validated_Family_iqResults.txt") + q + " " + digitsToCompareForSingleLinkage);
            lineForPostVerifyNodeConsolidation.Add(q + @"GlyQ-IQ_Compositions\Release\CompositionConsolidation.exe" + q + " " + q + Path.Combine(workingLocation, "ResultsSummary", parameters.DatasetFileNameNoEnding + "_Validated_Family_iqResults.txt") + q + " " + q + Path.Combine(workingLocation, "ResultsSummary", parameters.DatasetFileNameNoEnding + "_Validated_Composition_iqResults.txt") + q);
            lineForPostVerifyNodeConsolidation.Add(q + @"GlyQ-IQ_ToGlycoGrid\Release\ResultsToGlycoGridX64.exe" + q + " " + q + Path.Combine(workingLocation, "ResultsSummary", parameters.DatasetFileNameNoEnding + "_Validated_Family_iqResults.txt") + q + " " + q + workingLocation + q + " " + parameters.DatasetFileNameNoEnding + " " + parameters.FactorsName + " " + q + Path.Combine(workingLocation, "WorkingParameters") + q);
            lineForPostVerifyNodeConsolidation.Add("popd");
            //lineForPostVerifyNodeConsolidation.Add("pause");
            writer.toDiskStringList(Path.Combine(workingLocation, HPC_PostNodeConsolidationFileName), lineForPostVerifyNodeConsolidation);

            #endregion

            #region 2_HPC_DivideTargets
            List<string> linesDivideTargets = new List<string>();
            string HPC_DivideTargetsFileName = parameters.HPC_DivideTargetsLaunch;
            //"\\picfs\projects\DMS\ScottK\ScottK_PUB-100X_Launch_Folder\ToPIC\GlyQ-IQ_DivideTargets\Release\DivideTargets.exe" "\\picfs\projects\DMS\PIC_HPC\Home\HPC-Parameters_DivideTargetsPIC_Asterisks.txt"
            linesDivideTargets.Add(q + Path.Combine(parameters.WorkingDirectoryForExe, @"GlyQ-IQ_DivideTargets\Release\DivideTargets.exe") + q + " " + q + Path.Combine(workingLocation, "HPC-Parameters_DivideTargetsPIC_Asterisks.txt") + q);
            writer.toDiskStringList(Path.Combine(workingLocation, HPC_DivideTargetsFileName), linesDivideTargets);
            #endregion

            #region setup thermo dll FieldOfficeCopyToC.bat
            //Xcopy /e /y /s /v /i \\picfs\projects\DMS\PIC_HPC\FieldOffice_Velos_HM5-10_Azure\RemoteThermo\MassSpectrometerDLLs C:\MassSpectrometerDLLs
            string thermoPath = Path.Combine(workingLocation, "RemoteThermo");
            string thermoName = Path.Combine(thermoPath, "MassSpectrometerDLLs", "RemoteThermo", "FieldOfficeCopyToC.bat");
            List<string> thermoDllSetup = new List<string>();
            thermoDllSetup.Add("Xcopy /e /y /s /v /i " + Path.Combine(thermoPath, @"MassSpectrometerDLLs") + @" C:\MassSpectrometerDLLs");
            writer.toDiskStringList(thermoName, thermoDllSetup);

            #endregion

            #region setup Azure prep and copy
            //Xcopy /e /y /s /v /i \\picfs\projects\DMS\PIC_HPC\FieldOffice_Velos_HM5-10_Azure\RemoteThermo\MassSpectrometerDLLs C:\MassSpectrometerDLLs
            
            string azureBatchSetupName = Path.Combine(workingLocation, "3x_ConvertToAzureZip" + "_" + parameters.DatasetFileNameNoEnding + ".bat");
            
            List<string> azureSetup = new List<string>();
            azureSetup.Add("IF EXIST " + workingLocation + ".zip" + " (del " + workingLocation + ".zip)"); 
            azureSetup.Add("hpcPack create " + workingLocation + ".zip" + " " + workingLocation); 
            //azureSetup.Add("hpcpack upload " + workingLocation + ".zip" + " " + @"/nodetemplate:" + q + "Azure - Glyq-IQ" + q + " " + @"/relativepath:" + parameters.WorkingFolder + @" /blocksize:" + 4194304);

            string key = @"Si9Mqi09DeMJ42vl2LG8NFDwBg82/MJfuik23vX3b7/QPybGm39EbitATLxPPDQo7+iuBHXUeqC0rJrDbw13SA==";
            string account = "pichpc";
            azureSetup.Add("hpcpack upload " + workingLocation + ".zip" + " " + @"/account:" + account + " " + @"/key:" + key + " "  + @"/relativepath:" + parameters.WorkingFolder + @" /blocksize:" + 4194304);
           
            //azureSetup.Add("clusrun /nodegroup:AzureNodes hpcsync /packagename:FO_V_SN129AZ.Zip");
            //azureSetup.Add("clusrun /nodegroup:AzureNodes hpcsync " + "/packagename:" + parameters.WorkingFolder + ".zip");
           
            //this may need to be run from the head node
            //azureSetup.Add("clusrun" + " " + @"/account:" + account + " " + @"/key:" + key + " " +   "hpcsync");


            //allow the program to run
            //azureSetup.Add("clusrun /nodegroup:AzureNodes hpcfwutil register RegisterDLL " + parameters.WorkingFolder + @"ApplicationFiles\GlyQ-IQ_DLL\Release\RegisterDLL.exe");
            //azureSetup.Add("clusrun /nodegroup:AzureNodes hpcfwutil register RegisterDLL " + parameters.WorkingFolder + @"ApplicationFiles\GlyQ-IQ_ThermoDLL\Release\RegisterDLL.exe"); 
            writer.toDiskStringList(azureBatchSetupName, azureSetup);

            #endregion

        }

        private static bool CheckDirectoryLocationOnDisk(string directory, int i, string text)
        {
            bool isPresent = true;
            bool workingDirectoryTest = Directory.Exists(directory);
            if (!workingDirectoryTest)
            {
                Console.WriteLine("?? " + i + "_" + text  + ": " + directory + Environment.NewLine);
                isPresent = false;
            }
            return isPresent;
        }

        private static bool CheckFileLocationOnDisk(string file, int test, string word)
        {
            bool isPresent = true;
            bool workingDirectoryTest = File.Exists(file);
            if (!workingDirectoryTest)
            {
                Console.WriteLine("?? " + test + " " + word + " " + file);
                isPresent = false;
            }
            return isPresent;
        }
    }
}
