using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DivideTargetsLibraryX64;
using DivideTargetsLibraryX64.FromGetPeaks;
using DivideTargetsLibraryX64.Parameters;
using Ionic.Zip;


namespace FieldOffice
{
    class Program
    {
        /// <summary>
        /// for writing to disk
        /// </summary>
        private static StringListToDisk Writer { get; set; }

        /// <summary>
        /// unique seed
        /// </summary>
        private static int RandomIdentfier { get; set; }

        private static string KeyRawDataFile_with_1 { get; set; }

        static void Main(string[] args)
        {
            //string fileSystem = @"picfs";
            //string fileSystem = @"picfs.pnl.gov";
            string fileSystem = @"winhpcfs";

            string fileSystemPICFS = @"picfs.pnl.gov";
            string Targets_in = "L_13_HighMannose_TargetsFirstAll_wFucose";
            string Cores_in = "19";
            string Factors_in = "Factors_L10PSA.txt";
            string FieldOffice_in = "F_Local_V9";
            string dataFilesNoEnding_in = "SPIN_SN138_16Dec13_C15_1";
            string parametersFilesiQParameterFile_WithEnding_in = "FragmentedParameters_Spin_HAVG_HolyCellC127.txt";
            string parametersFilesglyQIQconsoleOperatingParameters_WithEnding_in = "GlyQIQ_Params_SPIN_SN138_L10PSAC127.txt";
            string RandomIdentfier_in = "5";
            
            string frankenDelete = "true";//"true" or "false"

            string DataFileExtension_in = "raw";
            ///where the working directory will be fabricated
            string WorkingDirectoryCreactionLocation_in = @"\\" + fileSystem + @"\projects\DMS\PIC_HPC\Hot";
             
            
            ///where the working directroy will launch from
            string WorkingDirectoryLaunchLocation_in = @"\\" + fileSystem + @"\projects\DMS\PIC_HPC\Hot";
            
            //string WorkingDirectoryLaunchLocation_in = @"E:\ScottK"; //E:\ScottK\F_Std_V9_Local_ESI_SN138_21Dec_C15_2530_1

            string HPCLoggingBaseDirectory_in = @"\\" + fileSystem + @"\projects\DMS\PIC_HPC\Hot";
            //THE HPC will try to write to here WorkingDirectoryLaunchLocation_in so all HPC jobs need to be PICFS

            string IsHPC_in = "true";
            //string IsHPC_in = "false"; WorkingDirectoryLaunchLocation_in = @"E:\ScottK";

            args = new string[12];
            args[0] = Targets_in;
            args[1] = Cores_in;
            args[2] = Factors_in;
            args[3] = FieldOffice_in;
            args[4] = dataFilesNoEnding_in;
            args[5] = parametersFilesiQParameterFile_WithEnding_in;
            args[6] = parametersFilesglyQIQconsoleOperatingParameters_WithEnding_in;
            args[7] = RandomIdentfier_in;
            args[8] = WorkingDirectoryCreactionLocation_in;
            args[9] = IsHPC_in;
            args[10] = HPCLoggingBaseDirectory_in;
            args[11] = DataFileExtension_in;
            Writer = new StringListToDisk();

            bool overrideArgs = true;
            bool isAzure = false;
            bool paralellUnZip = true;//also parallel copies
            bool ifCopyApplicationFiles = true;
            bool ifCopyDataFiles = true;
            bool ifCopyParameterFiles = true;

            string isHPC = IsHPC_in;

            string targetsFile_NoEnding = Targets_in;
            int cores = Convert.ToInt32(Cores_in);
            string factors_WithEnding = Factors_in;
            string fieldOfficeSeries = FieldOffice_in;
            RandomIdentfier = Convert.ToInt32(RandomIdentfier_in);
            string dataFileExtension = DataFileExtension_in;
            List<string> dataFilesNoEnding = new List<string>();
            List<string> parametersFilesiQParameterFile_WithEnding = new List<string>();
            List<string> parametersFilesglyQIQconsoleOperatingParameters_WithEnding = new List<string>();

            dataFilesNoEnding.Add(dataFilesNoEnding_in);
            //parametersFilesiQParameterFile_WithEnding.Add(parametersFilesiQParameterFile_WithEnding_in);
            parametersFilesglyQIQconsoleOperatingParameters_WithEnding.Add(parametersFilesglyQIQconsoleOperatingParameters_WithEnding_in);

            

            bool limitCores = false;
            int targetListSize = 0;//change below

            if (overrideArgs)
            {
                isAzure = false;
                //isHPC = "true";
                //D.
                //targetsFile_NoEnding = "L_10_PSA21_Cell_TargetsFirstAll_R";         targetListSize = 3267;   factors_WithEnding = "Factors_L10PSA.txt";
                //targetsFile_NoEnding = "L_13_HighMannose_TargetsFirstAll";          targetListSize = 9;      factors_WithEnding = "Factors_L10PSA.txt";//NOFrankenDelete
                //targetsFile_NoEnding = "L_13_HighMannose_TargetsFirstAll_wFucose";  targetListSize = 19;     factors_WithEnding = "Factors_L10PSA.txt"; //"Factors_HexFucose.txt";
                //targetsFile_NoEnding = "L_10_IQ_TargetsFirstAll_52000-6200";        targetListSize = 1;      factors_WithEnding = "Factors_L10PSA.txt";
                //targetsFile_NoEnding = "L_PSA21_LactoneOnly";                       targetListSize = 4000;   factors_WithEnding = "Factors_L10PSA.txt";//4000 is max for viewing but it looks like 5000 works as well
                //targetsFile_NoEnding = "L_11_AlditolNeutral";                       targetListSize = 4000;   factors_WithEnding = "Factors_L10PSA.txt";///4000 is max for viewing but it looks like 5000 works as well
                //targetsFile_NoEnding = "L_10_PSA21_TargetsFirstAll_R";              targetListSize = 3257; factors_WithEnding = "Factors_L10PSA.txt";
                
                //Part1
                //targetsFile_NoEnding = "L_10_IQ_TargetsFirstAll_R";                 targetListSize = 2194;   factors_WithEnding = "Factors_L10PSA.txt";//2194 max

                //part 2 no psa
                targetsFile_NoEnding = "L_13_Alditol_No_PSA_R";                     targetListSize = 1168; factors_WithEnding = "Factors_L10PSA.txt";//2194 max
               
                
                //targetsFile_NoEnding = "L_10_IQ_TargetsFirstAll_R";                 targetListSize = 2000; factors_WithEnding = "Factors_L10PSA.txt";

                //part 2 with psa
                //targetsFile_NoEnding = "L_13_Alditol_LactoneCombo";                   targetListSize = 3000;   factors_WithEnding = "Factors_L10PSA.txt";///4000 is max for viewing but it looks like 5000 works as well
                //targetsFile_NoEnding = "L_13_Alditol_LactoneCombo_Na";                targetListSize = 3000; factors_WithEnding = "Factors_L10PSA.txt";///4000 is max for viewing but it looks like 5000 works as well
                //targetsFile_NoEnding = "L_13_Alditol_LactoneCombo_Ca";                targetListSize = 3000; factors_WithEnding = "Factors_L10PSA.txt";///4000 is max for viewing but it looks like 5000 works as well

                //targetsFile_NoEnding = "L_13_Alditol_LactoneCombo_PO4";               targetListSize = 3000; factors_WithEnding = "Factors_L10PSA.txt";//2194 max
                //targetsFile_NoEnding = "L_13_Alditol_LactoneCombo_SO4";               targetListSize = 3000; factors_WithEnding = "Factors_L10PSA.txt";//2194 max
                //targetsFile_NoEnding = "L_13_Alditol_HighMannose_PO4";                  targetListSize = 19; factors_WithEnding = "Factors_L10PSA.txt";//2194 max
                
                
                //targetsFile_NoEnding = "L_13_Alditol_LactoneComboCell";             targetListSize = 3000;   factors_WithEnding = "Factors_L10PSACell.txt";//4000 is max for viewing but it looks like 5000 works as well
                //targetsFile_NoEnding = "L_13_Alditol_LactoneSelectPSA";               targetListSize = 128; factors_WithEnding = "Factors_L10PSACell.txt";//4000 is max for viewing but it looks like 5000 works as well

                //targetsFile_NoEnding = "L_13_Alditol_PSAX10";                       targetListSize = 89;     factors_WithEnding = "Factors_L10PSA.txt";
                //targetsFile_NoEnding = "L_13_Ant2Xylos";                            targetListSize = 210;    factors_WithEnding = "Factors_Ant.txt";
                //targetsFile_NoEnding = "L_13_Alditol_5401-5402";                    targetListSize = 2;        factors_WithEnding = "Factors_L10PSA.txt";
                //targetsFile_NoEnding = "L_13_AntCellulose";                         targetListSize = 24;       factors_WithEnding = "Factors_Ant.txt";
                //targetsFile_NoEnding = "L_10_IQ_ColominicAcid";                     targetListSize = 1324;     factors_WithEnding = "Factors_ColominicAcid.txt";
                //targetsFile_NoEnding = "L_10_IQ_ColominicAcid_Cal";                  targetListSize = 23; factors_WithEnding = "Factors_ColominicAcid.txt";
                //targetsFile_NoEnding = "L_10_IQ_ColominicAcid_Frag";                targetListSize = 6; factors_WithEnding = "Factors_ColominicAcid.txt";

                //args
                RandomIdentfier = 5;
                
                dataFilesNoEnding = new List<string>();
                parametersFilesiQParameterFile_WithEnding = new List<string>();
                parametersFilesglyQIQconsoleOperatingParameters_WithEnding = new List<string>();
                
                

                Datasets.SetData(out parametersFilesglyQIQconsoleOperatingParameters_WithEnding, out dataFilesNoEnding, out parametersFilesiQParameterFile_WithEnding, out fieldOfficeSeries);

            }

            if(limitCores)
            {
                cores = 1;//how many cores to allocate
            }
            else
            {
                cores = targetListSize;
            }

            //targetListSize = 1600;

            string clusterName = "Deception2.pnnl.gov";
            string templateName = "GlyQIQ";
            int maxTargetNumber = targetListSize;

            //A.  
            //KeyRawDataFile_with_1 = "Gly08_Velos4_Jaguar_200nL_D60A_1X_C1_2Sept12_1";//rawDataFile1 is the runing one
            //KeyRawDataFile_with_1 = "Gly09_SN129_21Feb13_Cheetah_C14_230nL_SPIN_X1_1";//rawDataFile1 is the runing one
            //KeyRawDataFile_with_1 = "Gly09_Velos3_Jaguar_200nL_C12_SN129_3X_23Dec12_1";
            //KeyRawDataFile_with_1 = "V_SN129_1";
            //KeyRawDataFile_with_1 = "S_SN129_1";

            
            //key parameters
            //string baseDirectory = @"\\" + fileSystem + @"\projects\DMS\PIC_HPC\Home";//copy files from here
            string baseDirectory = @"\\" + fileSystemPICFS + @"\projects\DMS\PIC_HPC\Home";//copy files from here

            //check this
            //string destinationstartPath = @"\\picfs\projects\DMS\PIC_HPC\Hot";
            string destinationstartPath = WorkingDirectoryCreactionLocation_in;
            string workingDirectoryPath = WorkingDirectoryLaunchLocation_in;
            string HPCLoggingPath = HPCLoggingBaseDirectory_in;

            //string ipAddressAsPath = @"\\172.16.112.12\projects\DMS\PIC_HPC";
            //string ipAddressAsPath = @"\\172.16.112.12\projects\DMS\PIC_HPC\Hot";
            string AzureContainerPath = @"\\" + fileSystem + @"\projects\DMS\PIC_HPC\Hot";

            if (isAzure) AzureContainerPath = @"%" + @"%" + "CCP_PACKAGE_ROOT" + @"%" + @"%";

            //check data files are present
            CheckRawAndPeaks(baseDirectory, dataFilesNoEnding);


            for(int j=0;j<dataFilesNoEnding.Count;j++)
            {
                string datafile = dataFilesNoEnding[j];
                
                KeyRawDataFile_with_1 = datafile;
                
                //B.  
                //string workDirectoryName = @"FO_V_SN129AZ";
                string workDirectoryName = fieldOfficeSeries + "_" + datafile;
                //string workDirectoryName = @"FO_V_SN129";
                //string workDirectoryName = "FieldOffice_Velos";
                //string workDirectoryName = "FieldOffice_SPIN";

                string applicationDirectory = "ApplicationFiles";
                //string applicationDirectory = "ApplicationFiles";

                string pathToApplicationZipStorage = "ToPIC_Zip";
                string dataDirectory = "RawData";
                string parametersDirectory = "WorkingParameters";
                string resultsDirectory = "Results";
                string resultsSummaryDirectory = "ResultsSummary";
                string locksFolder = "LocksFolder";
                string remoteThermoDirectory = "RemoteThermo";

                

                //string computeNodeName = "Richland";
                string computeNodeName = "ComputeNodes";
                //string computeNodeName = "Kronies";
                
                if (isAzure) computeNodeName = "AzureNodes";

                //string computerNodeNameForDelete = "Kronies";
                string computerNodeNameForDelete = "PrePost";
                int coresForDelete = 16;
                

                //0. initial preprocessing

                string azureBatchName = "3x_ConvertToAzureZip" + "_" + KeyRawDataFile_with_1 + ".bat";
                //string destinationDirectory = destinationstartPath + @"\" + workDirectoryName;
                string destinationDirectory = destinationstartPath;
                string ipAddressDirectory = AzureContainerPath + @"\" + workDirectoryName;


                if (isAzure) ipAddressDirectory = AzureContainerPath + @"\" + workDirectoryName;

                int uniqueFolder = RandomNumberFromSeed(RandomIdentfier);

                //1.  core exe folders needed

                List<string> applicationsToCopy = new List<string>();
                applicationsToCopy.Add("GlyQ-IQ Application_Setup");
                applicationsToCopy.Add("GlyQ-IQ_Application");
                applicationsToCopy.Add("GlyQ-IQ_CheckFile");
                applicationsToCopy.Add("GlyQ-IQ_CombineNodeResults");
                applicationsToCopy.Add("GlyQ-IQ_Compositions");
                applicationsToCopy.Add("GlyQ-IQ_DeleteFiles");
                applicationsToCopy.Add("GlyQ-IQ_DivideTargets");
                applicationsToCopy.Add("GlyQ-IQ_DivideTargetsNode");
                applicationsToCopy.Add("GlyQ-IQ_ThermoDLL");
                applicationsToCopy.Add("GlyQ-IQ_HPC_Check");
                applicationsToCopy.Add("GlyQ-IQ_HPC_DeleteCloud");
                applicationsToCopy.Add("GlyQ-IQ_HPC_DeleteEngine");
                applicationsToCopy.Add("GlyQ-IQ_HPC_DeleteFilesList");
                applicationsToCopy.Add("GlyQ-IQ_HPC_JobCreation");
                applicationsToCopy.Add("GlyQ-IQ_HPCListCombineMaker");
                applicationsToCopy.Add("GlyQ-IQ_MultiSleep");
                applicationsToCopy.Add("GlyQ-IQ_PostProcessing");
                applicationsToCopy.Add("GlyQ-IQ_SingleLinkage");
                applicationsToCopy.Add("GlyQ-IQ_Sleep");
                applicationsToCopy.Add("GlyQ-IQ_Timer");
                applicationsToCopy.Add("GlyQ-IQ_ToGlycoGrid");
                applicationsToCopy.Add("GlyQ-IQ_UnZip");
                applicationsToCopy.Add("GlyQ-IQ_WriteHPCFiles");
                applicationsToCopy.Add("GlyQ-IQ_Zip");
                applicationsToCopy.Add("GlyQ-IQ_ConvertValidatedIQToChecked");

                //2.  datafiles.  need to go in as arg parameter


                //string rawDataFile2 = "Gly09_SN129_21Feb13_Cheetah_C14_230nL_SPIN_1700V_300mlmin_22Torr_100C_60kHDR2M_1";
                //string rawDataFile3 = "Gly09_Velos3_Jaguar_200nL_C12_SN129_3X_23Dec12_1";
                List<string> dataFilesToCopy = new List<string>();
                //AddDataFile(KeyRawDataFile_with_1, dataFilesToCopy);  
                AddDataFile(datafile, dataFilesToCopy, dataFileExtension);
                //AddDataFile(rawDataFile2, dataFilesToCopy);
                //AddDataFile(rawDataFile3, dataFilesToCopy);

                //3. parameter files for run

                
                //string ExecutorParameterFile_WithEnding = "SN111-SN117_OrbiStandard_workflowExecutorParametersSK_Work HM Test.xml";
                string ExecutorParameterFile_WithEnding = "ExecutorParametersSK.xml";


                string iQParameterFile_WithEnding = parametersFilesiQParameterFile_WithEnding[j];
                string glyQIQconsoleOperatingParameters_WithEnding = parametersFilesglyQIQconsoleOperatingParameters_WithEnding[j];
               
                //4. Setup 0y_HPC_OperationParameters

                HPCPrepParameters hpcParameters = new HPCPrepParameters();
                hpcParameters.cores = cores;
                //hpcParameters.WorkingDirectory = destinationDirectory; 
                hpcParameters.WorkingDirectory = workingDirectoryPath;
                hpcParameters.WorkingFolder = workDirectoryName;
                hpcParameters.DataSetDirectory = hpcParameters.WorkingDirectory + @"\" + workDirectoryName + @"\" + dataDirectory;
                hpcParameters.DatasetFileNameNoEnding = KeyRawDataFile_with_1;
                hpcParameters.WorkingDirectoryForExe = hpcParameters.WorkingDirectory + @"\" + workDirectoryName + @"\" + applicationDirectory;
                hpcParameters.TargetsNoEnding = targetsFile_NoEnding;
                hpcParameters.FactorsName = factors_WithEnding;
                hpcParameters.ExecutorParameterFile = ExecutorParameterFile_WithEnding;
                hpcParameters.IQParameterFile = iQParameterFile_WithEnding;
                hpcParameters.makeResultsListName = "HPC_MakeResultsList_Asterisks";
                hpcParameters.divideTargetsParameterFile = "HPC-Parameters_DivideTargetsPIC_Asterisks.txt";
                hpcParameters.consoleOperatingParameters = glyQIQconsoleOperatingParameters_WithEnding;

               
                hpcParameters.HPC_MultiSleepParameterFileGlobal = "HPC_MultiSleepParameterFileGlobal";
                hpcParameters.ipaddress = hpcParameters.WorkingDirectory + @"\" + workDirectoryName;
                hpcParameters.LogIpaddress = hpcParameters.WorkingDirectory + @"\" + workDirectoryName;
                hpcParameters.ExeLaunchDirectory = hpcParameters.WorkingDirectory + @"\" + workDirectoryName + @"\" + applicationDirectory;
                hpcParameters.HPCNodeGroupName = computeNodeName;
                hpcParameters.IsHPC = isHPC;
                hpcParameters.DatasetFileExtenstion = dataFileExtension;
                hpcParameters.FrankenDelete = frankenDelete;

                hpcParameters.ClusterName = clusterName;
                hpcParameters.TemplateName = templateName;
                hpcParameters.MaxTargetNumber = maxTargetNumber;

                switch (isHPC)
                {
                    case "true":
                        {
                            hpcParameters.scottsFirstHPLCLauncher = "HPC_ScottsFirstHPLCLauncher";
                        }
                        break;
                    case "false":
                        {
                            hpcParameters.scottsFirstHPLCLauncher = "HPC_ScottsLocalLauncher";
                        }
                        break;
                    default:
                        {
                            hpcParameters.scottsFirstHPLCLauncher = "HPC_ScottsLocalLauncher";
                        }
                        break;
                }

                List<string> parameterFilesToCopy = new List<string>();
                parameterFilesToCopy.Add(hpcParameters.FactorsName);
                parameterFilesToCopy.Add(hpcParameters.IQParameterFile);
                //parameterFilesToCopy.Add(hpcParameters.consoleOperatingParameters);
                parameterFilesToCopy.Add(hpcParameters.TargetsNoEnding + @".txt");//(L_10_PSA21_TargetsFirstAll_R)
                parameterFilesToCopy.Add(hpcParameters.ExecutorParameterFile);
                parameterFilesToCopy.Add("L_RunFile.xml");
                //parameterFilesToCopy.Add("TargetedAlignmentWorkflowParameters.xml");
                parameterFilesToCopy.Add("AlignmentParameters.xml");
                //additional librarys and parameter files to copy
                //parameterFilesToCopy.Add("L_10_PSA21_Cell_TargetsFirstAll_R_3257.txt");//task testing
                //parameterFilesToCopy.Add("L_10_HighMannose_TargetsFirstAll.txt");//(L_10_PSA21_TargetsFirstAll_R)
                //parameterFilesToCopy.Add("FragmentedTargetedWorkflowParameters_SpinExactive_SN129HAVG.txt");
                //parameterFilesToCopy.Add("GlyQIQ_Diabetes_Parameters_PICFS_SPIN_SN129.txt");
                //parameterFilesToCopy.Add("FragmentedTargetedWorkflowParameters_Velos_H.txt");
                //parameterFilesToCopy.Add("GlyQIQ_Diabetes_Parameters_PICFS_Velos_SN129_L10PSA.txt");

                //5.  check files to make sure they are all there

                CheckFilesExist(
                    parameterFilesToCopy,
                    dataFilesToCopy,
                    dataDirectory,
                    pathToApplicationZipStorage,
                    baseDirectory,
                    applicationsToCopy,
                    parametersDirectory,
                    workDirectoryName);

                #region 1. Set Up FieldOffice folder

                bool addIdentifier = false;
                if (addIdentifier)
                {
                    destinationDirectory += "_" + uniqueFolder;
                }



                if (Directory.Exists(destinationDirectory + @"\" + workDirectoryName))
                {
                    Console.WriteLine("Do you want to delete this folder:" + Environment.NewLine + Environment.NewLine + destinationDirectory + @"\" + workDirectoryName + Environment.NewLine + Environment.NewLine + "press y to delete");


                    ConsoleKeyInfo key = Console.ReadKey();
                    if (key.KeyChar == 'y')
                    {
                        Console.WriteLine(Environment.NewLine + Environment.NewLine + "Deleting " + destinationDirectory + @"\" + workDirectoryName + "." + Environment.NewLine + "  This takes time...");
                        //Directory.Delete(destinationDirectory, true);
                        DeleteDirectoryWrapper.TryDelete(destinationDirectory + @"\" + workDirectoryName);//most robust for read only files
                    }
                    Console.WriteLine(Environment.NewLine);
                }

                Console.WriteLine("Old FieldOffice is deleted");

                Directory.CreateDirectory(destinationDirectory + @"\" + workDirectoryName);

                if (addIdentifier)
                {

                    List<string> folderName = new List<string>();
                    folderName.Add(uniqueFolder.ToString());

                    Writer.toDiskStringList(destinationDirectory + @"\" + workDirectoryName + @"\" + "addIdentifier.txt", folderName);
                }

                #endregion

                #region 2.  Copy and Unzip Application Files

                
                

                if (ifCopyApplicationFiles)
                {
                    Console.WriteLine(Environment.NewLine + "Unzipping files...");

                    string applictionDirectoryPath = destinationDirectory + @"\" + workDirectoryName + @"\" + applicationDirectory;
                    Directory.CreateDirectory(applictionDirectoryPath);

                    int zipcounter = 0;

                    
                    if (paralellUnZip)
                    {
                        System.Threading.Tasks.Parallel.ForEach(applicationsToCopy, new ParallelOptions { MaxDegreeOfParallelism = 30 }, zippedFile =>
                        {
                            Console.WriteLine(Environment.NewLine + zippedFile);
                            Console.Write("Extracting file " + zipcounter + " out of " + applicationsToCopy.Count + "...");
                            zipcounter++;
                            using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(baseDirectory + @"\" + pathToApplicationZipStorage + @"\" + zippedFile + ".zip"))
                            {
                                zip.ExtractAll(applictionDirectoryPath + @"\" + zippedFile, ExtractExistingFileAction.OverwriteSilently);
                            }
                            Console.WriteLine("Unzipped");
                        });
                    }
                    else
                    {
                        foreach (string zippedFile in applicationsToCopy)
                        {
                            Console.WriteLine(Environment.NewLine + zippedFile);
                            Console.Write("Extracting file " + zipcounter + " out of " + applicationsToCopy.Count + "...");
                            zipcounter++;
                            using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(baseDirectory + @"\" + pathToApplicationZipStorage + @"\" + zippedFile + ".zip"))
                            {
                                zip.ExtractAll(applictionDirectoryPath + @"\" + zippedFile, ExtractExistingFileAction.OverwriteSilently);
                            }
                            Console.WriteLine("Unzipped");
                        }
                    }
                }

                #endregion

                #region 3.  Copy data Files

                
                if (ifCopyDataFiles)
                {
                    Console.WriteLine(Environment.NewLine + "Copying Data");

                    string rawDataDirectoryPath = destinationDirectory + @"\" + workDirectoryName + @"\" + dataDirectory;
                    Directory.CreateDirectory(rawDataDirectoryPath);

                    int dataCounter = 0;

                    if (paralellUnZip)
                    {
                        System.Threading.Tasks.Parallel.ForEach(dataFilesToCopy, new ParallelOptions { MaxDegreeOfParallelism = 30 }, dataFile =>
                        {
                            Console.WriteLine(Environment.NewLine + dataFile);
                            Console.Write("Copying file " + dataCounter + " out of " + dataFilesToCopy.Count + "...");
                            System.IO.File.Copy(baseDirectory + @"\" + dataDirectory + @"\" + dataFile, rawDataDirectoryPath + @"\" + dataFile);
                            dataCounter++;
                            Console.WriteLine("Copied");
                        });
                    }
                    else
                    {
                        foreach (string dataFile in dataFilesToCopy)
                        {
                            Console.WriteLine(Environment.NewLine + dataFile);
                            Console.Write("Copying file " + dataCounter + " out of " + dataFilesToCopy.Count + "...");
                            System.IO.File.Copy(baseDirectory + @"\" + dataDirectory + @"\" + dataFile,
                                                rawDataDirectoryPath + @"\" + dataFile);
                            dataCounter++;
                            Console.WriteLine("Copied");
                        }
                    }
                }
                #endregion

                #region 4.  Copy Parameter Files
                if (ifCopyParameterFiles)
                {
                    Console.WriteLine(Environment.NewLine + "Copying Parameter Files");

                    string parameterDirectoryPath = destinationDirectory + @"\" + workDirectoryName + @"\" + parametersDirectory;
                    Directory.CreateDirectory(parameterDirectoryPath);

                    int parameterCounter = 1;

                    if (paralellUnZip)
                    {
                        System.Threading.Tasks.Parallel.ForEach(parameterFilesToCopy, new ParallelOptions { MaxDegreeOfParallelism = 30 }, parameterFile =>
                        {
                            Console.WriteLine(Environment.NewLine + parameterFile);
                            Console.Write("Copying file " + parameterCounter + " out of " + parameterFilesToCopy.Count + "...");
                            System.IO.File.Copy(baseDirectory + @"\" + parametersDirectory + @"\" + parameterFile, parameterDirectoryPath + @"\" + parameterFile);
                            parameterCounter++;
                            Console.WriteLine("Copied");
                        });
                    }
                    else
                    {
                        foreach (string parameterFile in parameterFilesToCopy)
                        {
                            Console.WriteLine(Environment.NewLine + parameterFile);
                            Console.Write("Copying file " + parameterCounter + " out of " + parameterFilesToCopy.Count +
                                          "...");
                            System.IO.File.Copy(baseDirectory + @"\" + parametersDirectory + @"\" + parameterFile,
                                                parameterDirectoryPath + @"\" + parameterFile);
                            parameterCounter++;
                            Console.WriteLine("Copied");
                        }

                    }
                }
                #endregion

                #region 5. Create New Results folder

                Directory.CreateDirectory(destinationDirectory + @"\" + workDirectoryName + @"\" + resultsDirectory);
                Directory.CreateDirectory(destinationDirectory + @"\" + workDirectoryName + @"\" + resultsSummaryDirectory);

                List<string> dummyfilesResults = new List<string>();
                dummyfilesResults.Add("hi");
                Writer.toDiskStringList(destinationDirectory + @"\" + workDirectoryName + @"\" + resultsDirectory + @"\" + "PlaceHolder.txt", dummyfilesResults);

                #endregion

                #region 6. Create New Locks folder

                Directory.CreateDirectory(destinationDirectory + @"\" + workDirectoryName + @"\" + parametersDirectory + @"\" + locksFolder);

                List<string> dummyfilesLocks = new List<string>();
                dummyfilesResults.Add("hi");

                for (int i = 0; i < cores + 1; i++)//+1 is to make sure we have enough since iterator is 1-n instead of 0-n
                {
                    Writer.toDiskStringList(destinationDirectory + @"\" + workDirectoryName + @"\" + parametersDirectory + @"\" + locksFolder + @"\" + "Lock_" + i + ".txt", dummyfilesLocks);
                }



                #endregion

                #region 7. Copy Remote Thermo for Thermo DLL setup.
                
                Console.WriteLine(Environment.NewLine + "Copying Remote Thermo DLL Files");

                string remoteThermoDirectoryPath = destinationDirectory + @"\" + workDirectoryName + @"\" + remoteThermoDirectory;
                Directory.CreateDirectory(remoteThermoDirectoryPath);

                Console.Write("Copying Remote Thermo DLL File...");

                copyFolder(baseDirectory + @"\" + remoteThermoDirectory, remoteThermoDirectoryPath);

                Console.WriteLine("Copied");

                #endregion

                #region write "GlyQIQ_Params"

                GlyQIQ_Parameters glyQParameters = new GlyQIQ_Parameters();
                glyQParameters.ResultsFolderPath = workingDirectoryPath + @"\" + workDirectoryName + @"\" + "Results";
                glyQParameters.LoggingFolderPath = HPCLoggingPath + @"\" + workDirectoryName + @"\" + "Results";
                glyQParameters.FactorsFile = factors_WithEnding;
                glyQParameters.ExecutorParameterFile = ExecutorParameterFile_WithEnding;
                glyQParameters.XYDataFolder = "XYDataWriter";
                glyQParameters.WorkflowParametersFile = iQParameterFile_WithEnding;//FragmentedParameters_Spin_HAVG_HolyCellC127.txt
                glyQParameters.Allignment = workingDirectoryPath + @"\" + workDirectoryName + @"\" + @"WorkingParameters\AlignmentParameters.xml";
                glyQParameters.BasicTargetedParameters = workingDirectoryPath + @"\" + workDirectoryName + @"\" + @"WorkingParameters\BasicTargetedWorkflowParameters.xml";

                glyQParameters.Write(destinationDirectory + @"\" + workDirectoryName + @"\" + "WorkingParameters" + @"\" + glyQIQconsoleOperatingParameters_WithEnding);


                #endregion

                #region 8.  Write 0y operating Parameters

                string operatingParametersName = "0y_HPC_OperationParameters" + "_" + uniqueFolder + ".txt";
                string operatingParametersFullPath = destinationDirectory + @"\" + workDirectoryName + @"\" + operatingParametersName;
                string operatingParametersFullPathForLaunch = workingDirectoryPath + @"\" + workDirectoryName + @"\" + operatingParametersName;
                hpcParameters.Write(operatingParametersFullPath);

                #endregion

                #region 9.  Write 0x_HPCLaunch

                string launchName = "0x_Launch" + "_" + KeyRawDataFile_with_1 + "_" + uniqueFolder + ".bat";

                //List<string> linesOfLaunch = new List<string>();
                //string line1 = workingDirectoryPath + @"\" + workDirectoryName + @"\" + applicationDirectory + @"\" + @"GlyQ-IQ_WriteHPCFiles\Release\WriteHPCPrepFiles.exe" + " " + operatingParametersFullPathForLaunch;
                //string line2 = "if exist " + workingDirectoryPath + @"\" + workDirectoryName + @"\" + "HPC_Stopwatch" + "_" + KeyRawDataFile_with_1 + ".txt del " + workingDirectoryPath + @"\" + workDirectoryName + @"\" + "HPC_Stopwatch" + "_" + KeyRawDataFile_with_1 + @".txt /q";
                //string line3 = "call " + workingDirectoryPath + @"\" + workDirectoryName + @"\" + "2_HPC_DivideTargets.bat";
                //string line4 = line1;
                ////azure add on so it gets packaged and shipped to Azure.  we need to copy this out somewhere so we can pack it up unattached
                //string line44 = @"Xcopy /e /y /s /v /i " + workingDirectoryPath + @"\" + workDirectoryName + @"\" + azureBatchName + " " + baseDirectory + @"\" + azureBatchName + "*";
                //string line45 = "call " + baseDirectory + @"\" + azureBatchName;
                //string line5 = "call " + workingDirectoryPath + @"\" + workDirectoryName + @"\" + hpcParameters.scottsFirstHPLCLauncher + "_" + KeyRawDataFile_with_1 + ".bat";

                string workDir = @"%WorkDir%";
                List<string> linesOfLaunch = new List<string>();

                bool pushD = false;
                if (pushD)
                {
                    linesOfLaunch = PushDVersion(hpcParameters, operatingParametersName, isHPC, workDir, workingDirectoryPath, workDirectoryName);
                }
                else
                {

                    linesOfLaunch = SimpleVersion(workDir, isHPC, hpcParameters, operatingParametersName, workingDirectoryPath, workDirectoryName);
                }
                //linesOfLaunch.Add("pause");
                
                //string line1 = workingDirectoryPath + @"\" + workDirectoryName + @"\" + applicationDirectory + @"\" + @"GlyQ-IQ_WriteHPCFiles\Release\WriteHPCPrepFiles.exe" + " " + operatingParametersFullPathForLaunch;
                //string line2 = "if exist " + workingDirectoryPath + @"\" + workDirectoryName + @"\" + "HPC_Stopwatch" + "_" + KeyRawDataFile_with_1 + ".txt del " + workingDirectoryPath + @"\" + workDirectoryName + @"\" + "HPC_Stopwatch" + "_" + KeyRawDataFile_with_1 + @".txt /q";
                //string line3 = "call " + workingDirectoryPath + @"\" + workDirectoryName + @"\" + "2_HPC_DivideTargets.bat";
                //string line4 = line1;
                ////azure add on so it gets packaged and shipped to Azure.  we need to copy this out somewhere so we can pack it up unattached
                //string line44 = @"Xcopy /e /y /s /v /i " + workingDirectoryPath + @"\" + workDirectoryName + @"\" + azureBatchName + " " + baseDirectory + @"\" + azureBatchName + "*";
                //string line45 = "call " + baseDirectory + @"\" + azureBatchName;
                //string line5 = "call " + workingDirectoryPath + @"\" + workDirectoryName + @"\" + hpcParameters.scottsFirstHPLCLauncher + "_" + KeyRawDataFile_with_1 + ".bat";

                //string line6 = "Pause";
                
                //string line6 = "";
                //if (isAzure)
                //{
                //    linesOfLaunch.Add(line1); linesOfLaunch.Add(line2); linesOfLaunch.Add(line3); linesOfLaunch.Add(line4); linesOfLaunch.Add(line44); linesOfLaunch.Add(line45); linesOfLaunch.Add(line6);
                //    //linesOfLaunch.Add(line1); linesOfLaunch.Add(line2); linesOfLaunch.Add(line3); linesOfLaunch.Add(line4); linesOfLaunch.Add(line44); linesOfLaunch.Add(line45); linesOfLaunch.Add(line6);//remmove launch
                //}
                //else
                //{
                //    linesOfLaunch.Add(line1); linesOfLaunch.Add(line2); linesOfLaunch.Add(line3); linesOfLaunch.Add(line4); linesOfLaunch.Add(line5); linesOfLaunch.Add(line6);
                //}

                Writer.toDiskStringList(destinationDirectory + @"\" + workDirectoryName + @"\" + launchName, linesOfLaunch);

                #endregion

                #region Write 1x_FrankenDelete

                string deleteName = "1x_FrankenDelete" + "_" + KeyRawDataFile_with_1 + ".bat";
                List<string> linesOfdelete = new List<string>();
                string lined1 = workingDirectoryPath + @"\" + workDirectoryName + @"\" + applicationDirectory + @"\" + @"GlyQ-IQ_HPC_DeleteFilesList\Release\DeleteViaHPCList.exe" + " " + operatingParametersFullPath;

                string paramterForDeleteCoud = workingDirectoryPath + @"\" + workDirectoryName + " " + computerNodeNameForDelete + " " + coresForDelete;
                string lined2 = workingDirectoryPath + @"\" + workDirectoryName + @"\" + applicationDirectory + @"\" + @"GlyQ-IQ_HPC_DeleteCloud\Release\HPC_DeleteCloud.exe" + " " + paramterForDeleteCoud;

                if (isAzure)
                {
                    linesOfdelete.Add(lined1); linesOfdelete.Add(lined2);
                    //linesOfLaunch.Add(line1); linesOfLaunch.Add(line2); linesOfLaunch.Add(line3); linesOfLaunch.Add(line4); linesOfLaunch.Add(line44); linesOfLaunch.Add(line45); linesOfLaunch.Add(line6);//remmove launch
                }
                else
                {
                    linesOfdelete.Add(lined1); linesOfdelete.Add(lined2);
                }

                Writer.toDiskStringList(destinationDirectory + @"\" + workDirectoryName + @"\" + deleteName, linesOfdelete);

                #endregion

                #region Write 2x_DeleteResultsFolder

                string deleteResultsName = "2x_DeleteResultsFolder" + "_" + KeyRawDataFile_with_1 + ".bat";
                List<string> linesOfResultsdelete = new List<string>();
                string liner1 = "rmdir " + workingDirectoryPath + @"\" + workDirectoryName + @"\" + "Results" + " " + @"/" + "S" + " " + @"/" + "Q";
                string liner2 = "rmdir " + workingDirectoryPath + @"\" + workDirectoryName + @"\" + "ApplicationFiles" + " " + @"/" + "S" + " " + @"/" + "Q";
               
                if (isAzure)
                {
                    linesOfResultsdelete.Add(liner1); linesOfResultsdelete.Add(liner2);
                    //linesOfLaunch.Add(line1); linesOfLaunch.Add(line2); linesOfLaunch.Add(line3); linesOfLaunch.Add(line4); linesOfLaunch.Add(line44); linesOfLaunch.Add(line45); linesOfLaunch.Add(line6);//remmove launch
                }
                else
                {
                    linesOfResultsdelete.Add(liner1); linesOfResultsdelete.Add(liner2);
                }

                Writer.toDiskStringList(destinationDirectory + @"\" + workDirectoryName + @"\" + deleteResultsName, linesOfResultsdelete);

                #endregion
            }
            Console.WriteLine(Environment.NewLine + "All Done.  Press a key to exit");
            //Console.Read();
        }

        private static List<string> SimpleVersion(string workDir, string isHPC, HPCPrepParameters hpcParameters, string operatingParametersName, string workingDirectoryPath, string workDirectoryName)
        {
            List<string> linesOfLaunch = new List<string>();
            linesOfLaunch.Add("rem Customize this work directory");
            linesOfLaunch.Add("set WorkDir=" + workingDirectoryPath + @"\" + workDirectoryName);
            linesOfLaunch.Add("");
            linesOfLaunch.Add(@"@echo Deleting existing results");
            linesOfLaunch.Add(""); 
            //linesOfLaunch.Add(@"pushd " + workDir);
            linesOfLaunch.Add(@"del Results\*.* " + @"/" + "s " + @"/" + "q"); //del Results\*.* /s /q" //linesOfLaunch.Add(@"del WorkingParameters\*_TargetsFirstAll_*.txt");
            linesOfLaunch.Add("");
            linesOfLaunch.Add(@"@echo Auto create parameter files using " + operatingParametersName);
            linesOfLaunch.Add(workDir + @"\ApplicationFiles\GlyQ-IQ_WriteHPCFiles\Release\WriteHPCPrepFiles.exe " + workDir + @"\" + operatingParametersName); //%WorkDir%\ApplicationFiles\GlyQ-IQ_WriteHPCFiles\Release\WriteHPCPrepFiles.exe %WorkDir%\0y_HPC_OperationParameters_1240183582.txt
            linesOfLaunch.Add("");
            linesOfLaunch.Add("if exist " + workDir + @"\HPC_Stopwatch_" + KeyRawDataFile_with_1 + ".txt del " + workDir + @"\HPC_Stopwatch_" + KeyRawDataFile_with_1 + ".txt /q"); //if exist %WorkDir%\HPC_Stopwatch_ESI_SN138_21Dec_C15_2530_1.txt del %WorkDir%\HPC_Stopwatch_ESI_SN138_21Dec_C15_2530_1.txt /q
            linesOfLaunch.Add("");
            linesOfLaunch.Add(@"@echo Dividing up the targets into 8 groups");
            linesOfLaunch.Add("call " + workDir + @"\2_HPC_DivideTargets.bat " + workDir);
            linesOfLaunch.Add("");
            linesOfLaunch.Add(@"@echo Auto creating more parameter files using " + operatingParametersName);
            linesOfLaunch.Add(workDir + @"\ApplicationFiles\GlyQ-IQ_WriteHPCFiles\Release\WriteHPCPrepFiles.exe " + workDir + @"\" + operatingParametersName);
            linesOfLaunch.Add("");
            linesOfLaunch.Add(@"@echo Spawning GlyQ-IQ analysis tasks");

            if (isHPC == "true")
            {
                linesOfLaunch.Add("call " + workDir + @"\" + hpcParameters.scottsFirstHPLCLauncher + "_" + KeyRawDataFile_with_1 + ".bat");
            }
            else
            {
                linesOfLaunch.Add("call " + workDir + @"\HPC_ScottsLocalLauncher_" + KeyRawDataFile_with_1 + ".bat " + workDir);
            }

            linesOfLaunch.Add("");
            linesOfLaunch.Add(@"@echo Starting timer that watches for results");
            linesOfLaunch.Add(workDir + @"\1_HPC_StartCollectResults_" + KeyRawDataFile_with_1 + ".bat " + workDir);

            linesOfLaunch.Add("");
            linesOfLaunch.Add(@"@echo Done Launching analysis; Result Collector will display progress messages here");
            //linesOfLaunch.Add("popd");

            return linesOfLaunch;
        }

        private static List<string> PushDVersion(HPCPrepParameters hpcParameters, string operatingParametersName, string isHPC,
                                         string workDir, string workingDirectoryPath, string workDirectoryName)
        {
            List<string> linesOfLaunch = new List<string>();
            linesOfLaunch.Add("rem Customize this work directory");
            linesOfLaunch.Add("set WorkDir=" + workingDirectoryPath + @"\" + workDirectoryName);
            linesOfLaunch.Add("");
            linesOfLaunch.Add(@"@echo Deleting existing results");
            linesOfLaunch.Add("");
            linesOfLaunch.Add(@"pushd " + workDir);
            linesOfLaunch.Add(@"del Results\*.* " + @"/" + "s " + @"/" + "q"); //del Results\*.* /s /q" //linesOfLaunch.Add(@"del WorkingParameters\*_TargetsFirstAll_*.txt");
            linesOfLaunch.Add("");
            linesOfLaunch.Add(@"@echo Auto create parameter files using " + operatingParametersName);
            linesOfLaunch.Add(workDir + @"\ApplicationFiles\GlyQ-IQ_WriteHPCFiles\Release\WriteHPCPrepFiles.exe " + workDir + @"\" + operatingParametersName); //%WorkDir%\ApplicationFiles\GlyQ-IQ_WriteHPCFiles\Release\WriteHPCPrepFiles.exe %WorkDir%\0y_HPC_OperationParameters_1240183582.txt
            linesOfLaunch.Add("");
            linesOfLaunch.Add("if exist " + workDir + @"\HPC_Stopwatch_" + KeyRawDataFile_with_1 + ".txt del " + workDir + @"\HPC_Stopwatch_" + KeyRawDataFile_with_1 + ".txt /q"); //if exist %WorkDir%\HPC_Stopwatch_ESI_SN138_21Dec_C15_2530_1.txt del %WorkDir%\HPC_Stopwatch_ESI_SN138_21Dec_C15_2530_1.txt /q
            linesOfLaunch.Add("");
            linesOfLaunch.Add(@"@echo Dividing up the targets into 8 groups");
            linesOfLaunch.Add("call " + workDir + @"\2_HPC_DivideTargets.bat " + workDir);
            linesOfLaunch.Add("");
            linesOfLaunch.Add(@"@echo Auto creating more parameter files using " + operatingParametersName);
            linesOfLaunch.Add(workDir + @"\ApplicationFiles\GlyQ-IQ_WriteHPCFiles\Release\WriteHPCPrepFiles.exe " + workDir + @"\" + operatingParametersName);
            linesOfLaunch.Add("");
            linesOfLaunch.Add(@"@echo Spawning GlyQ-IQ analysis tasks");

            if (isHPC == "true")
            {
                linesOfLaunch.Add("call " + workDir + @"\" + hpcParameters.scottsFirstHPLCLauncher + "_" + KeyRawDataFile_with_1 + ".bat");
            }
            else
            {
                linesOfLaunch.Add("call " + workDir + @"\HPC_ScottsLocalLauncher_" + KeyRawDataFile_with_1 + ".bat " + workDir);
            }

            linesOfLaunch.Add("");
            linesOfLaunch.Add(@"@echo Starting timer that watches for results");
            linesOfLaunch.Add(workDir + @"\1_HPC_StartCollectResults_" + KeyRawDataFile_with_1 + ".bat " + workDir);

            linesOfLaunch.Add("");
            linesOfLaunch.Add(@"@echo Done Launching analysis; Result Collector will display progress messages here");
            linesOfLaunch.Add("popd");

            return linesOfLaunch;
        }

        private static void CheckRawAndPeaks(string baseDirectory, List<string> dataFilesNoEnding)
        {
            int filesPresent = dataFilesNoEnding.Count*2;
            for (int j = 0; j < dataFilesNoEnding.Count; j++)
            {
                string data = baseDirectory + @"\" + "RawData" + @"\" + dataFilesNoEnding[j] + ".raw";
                string peaks = baseDirectory + @"\" + "RawData" + @"\" + dataFilesNoEnding[j] + "_Peaks.txt";
                if (!File.Exists(data))
                {
                    Console.WriteLine("Missing Raw Data File: " + data);
                }
                else
                {
                    filesPresent--;
                }

                if (!File.Exists(peaks))
                {
                    Console.WriteLine("Missing Peaks File: " + peaks);
                }
                else
                {
                    filesPresent--;
                }
            }

            if (filesPresent > 0)
            {
                Console.WriteLine("Missing Files: " + filesPresent);
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Data files present");
            }
        }

        private static void CheckFilesExist(List<string> parameterFilesToCopy, List<string> dataFilesToCopy, string dataDirectory, string pathToApplicationZipStorage, string baseDirectory, List<string> applicationsToCopy, string parametersDirectory, string workDirectoryName)
        {
            bool baseDirectoryCheck = false;
            if (Directory.Exists(baseDirectory)) baseDirectoryCheck = true;
            else Console.WriteLine("Missing " + "baseDirectory" + Environment.NewLine + baseDirectory);

            bool zipDirectoryCheck = false;
            if (Directory.Exists(baseDirectory + @"\" + pathToApplicationZipStorage)) zipDirectoryCheck = true;
            else Console.WriteLine("Missing " + "applicationDirectory" + Environment.NewLine + baseDirectory + @"\" + pathToApplicationZipStorage);

            bool overallZipFileCheck = true; //true untill proven false by a missing application
            foreach (string zippedFile in applicationsToCopy)
            {
                bool zipFileCheck1 = false;
                if (File.Exists(baseDirectory + @"\" + pathToApplicationZipStorage + @"\" + zippedFile + ".zip"))
                    zipFileCheck1 = true;
                else
                    Console.WriteLine("Missing " + "zipFile" + Environment.NewLine + baseDirectory + @"\" + pathToApplicationZipStorage + @"\" + zippedFile + ".zip");
                if (zipFileCheck1 == false) overallZipFileCheck = false;
            }

            bool overallDataFileCheck = true; //true untill proven false by a missing application
            foreach (string dataFile in dataFilesToCopy)
            {
                bool dataFileCheck1 = false;
                if (File.Exists(baseDirectory + @"\" + dataDirectory + @"\" + dataFile)) dataFileCheck1 = true;
                else Console.WriteLine("Missing " + "dataFile" + Environment.NewLine + baseDirectory + @"\" + dataDirectory + @"\" + dataFile);
                if (dataFileCheck1 == false) overallDataFileCheck = false;
            }

            bool overallParameterFileCheck = true; //true untill proven false by a missing application
            foreach (string parameterFile in parameterFilesToCopy)
            {
                bool dparameterFileCheck1 = false;
                if (File.Exists(baseDirectory + @"\" + parametersDirectory + @"\" + parameterFile)) dparameterFileCheck1 = true;
                else
                    Console.WriteLine("Missing " + "parameterFile" + Environment.NewLine + baseDirectory + @"\" + parametersDirectory + @"\" + parameterFile);
                if (dparameterFileCheck1 == false) overallParameterFileCheck = false;
            }

            if (
                baseDirectoryCheck &&
                zipDirectoryCheck &&
                overallZipFileCheck &&
                overallDataFileCheck &&
                overallParameterFileCheck)
            {
                Console.WriteLine("All Files are present" + Environment.NewLine);
            }
            else
            {
                Console.WriteLine("MissingFiles");
                Console.ReadKey();
            }
        }


        private static void copyFolder(string source_dir = @"E:\",string destination_dir = @"C:\")
        {
            // substring is to remove destination_dir absolute path (E:\).

            // Create subdirectory structure in destination    
            foreach (string dir in Directory.GetDirectories(source_dir, "*", System.IO.SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(destination_dir + dir.Substring(source_dir.Length));
                // Example:
                //     > C:\sources (and not C:\E:\sources)
            }

            foreach (string file_name in Directory.GetFiles(source_dir, "*.*", System.IO.SearchOption.AllDirectories))
            {
                File.Copy(file_name, destination_dir + file_name.Substring(source_dir.Length));
            }
        }



        private static void AddDataFile(string rawDataFile, List<string> dataFilesToCopy, string dataFileExtension)
        {
            dataFilesToCopy.Add(rawDataFile + "." + dataFileExtension);
            dataFilesToCopy.Add(rawDataFile + "_peaks.txt");
        }


        private static int RandomNumberFromSeed(int seedIn)
        {
            int inputSeed = Convert.ToInt32(seedIn);
            Random randomNumberGeneratorSeed = new Random();
            int seed = Convert.ToInt32(randomNumberGeneratorSeed.Next(0, int.MaxValue)) + inputSeed;
            Random randomNumberGenerator = new Random(seed);
            int randomNumber = randomNumberGenerator.Next(0, int.MaxValue);

            return randomNumber;
        }
    }
}
