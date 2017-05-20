using System;
using System.Collections.Generic;
using System.IO;
using GetPeaksDllLite.DataFIFO;
using GetPeaksDllLite.Functions;
using IQGlyQ;
using IQGlyQ.Objects;
using IQGlyQ.UnitTesting;

namespace IQGlyQ_Console
{
    class Program
    {
        //1-14-2014
        //"\\picfs\projects\DMS\PIC_HPC\Home\RawData" "Gly09_Velos3_Jaguar_200nL_C12_SN129_3X_23Dec12" "raw" "L_10_IQ_TargetsFirstAll_52000.txt" "GlyQIQ_Diabetes_Parameters_PICFS_Velos_SN129_L10.txt" "\\picfs\projects\DMS\PIC_HPC\Home\WorkingParameters" "Lock_0" "E:\ScottK\ToPic\Results" "33"


        //static void Main(string[] args)
        //{
        //    //Home//"Gly09_Velos3_Jaguar_200nL_C13_SN119SN122_3X_23Dec12" "L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data" "L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\SN111-SN117_OrbiStandard_workflowExecutorParametersSK_Home.xml"
        //    //Work //"Gly09_Velos3_Jaguar_200nL_C13_SN119SN122_3X_23Dec12" "D:\Csharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data" "D:\Csharp\ConosleApps\LocalServer\IQ\Sarwall\SN111-SN117_OrbiStandard_workflowExecutorParametersSK_Work.xml"
        //    List<string> mainParameterFromBatch;
        //    ArgsProcessing(args, out mainParameterFromBatch);

        //    string testDatasetName = mainParameterFromBatch[0];
        //    string testDatasetPath = mainParameterFromBatch[1];
        //    string executorParameterFile = mainParameterFromBatch[2];

        //    Console.WriteLine("");
        //    Console.WriteLine("Args1: " + testDatasetName);
        //    Console.WriteLine("Args2: " + testDatasetPath);
        //    Console.WriteLine("Args3: " + executorParameterFile);
        //    Console.WriteLine("");

        //    testDatasetPath = testDatasetPath + "\\" + testDatasetName + ".raw";
        //    Console.WriteLine("Path: " + testDatasetPath);
        //    Console.WriteLine("");

        //    //Console.ReadKey();


        //    RunMeIQGlyQ wizard = new RunMeIQGlyQ();
        //    wizard.ExecuteDeuteratedTargetedWorkflow(executorParameterFile, testDatasetPath, testDatasetName);

        //}

        //static void Main(string[] args)
        //{
        //    //Home//"Gly09_Velos3_Jaguar_200nL_C13_SN119SN122_3X_23Dec12" "L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data" "L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\SN111-SN117_OrbiStandard_workflowExecutorParametersSK_Home.xml"
        //    //Work //"Gly09_Velos3_Jaguar_200nL_C13_SN119SN122_3X_23Dec12" "D:\Csharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data" "D:\Csharp\ConosleApps\LocalServer\IQ\Sarwall\SN111-SN117_OrbiStandard_workflowExecutorParametersSK_Work.xml"

        //    List<string> mainParameterFromBatch;
        //    ArgsProcessing(args, out mainParameterFromBatch);

        //    string testDatasetName = mainParameterFromBatch[0];
        //    string testDatasetPath = mainParameterFromBatch[1];
        //    string executorParameterFile = mainParameterFromBatch[2];

        //    Console.WriteLine("");
        //    Console.WriteLine("Args1: " + testDatasetName);
        //    Console.WriteLine("Args2: " + testDatasetPath);
        //    Console.WriteLine("Args3: " + executorParameterFile);
        //    Console.WriteLine("");

        //    testDatasetPath = testDatasetPath + "\\" + testDatasetName + ".raw";
        //    Console.WriteLine("Path: " + testDatasetPath);
        //    Console.WriteLine("");

        //    //Console.ReadKey();
        //    FragmentIQTarget newtarget = new FragmentIQTarget();


        //    IQ_FullDataset_Test tester = new IQ_FullDataset_Test();

        //    //default
        //    tester.ExecuteMultipleTargetsTest1();

        //    //all inputs
        //    //tester.ExecuteMultipleTargetsTest2()
        //}

        static int Main(string[] args)
        {

            Console.WriteLine("4-15-2014 4:00PM, V2.0 X64");

            Console.WriteLine("Loading Dlls...");

            Utiliites.CheckDllsForGlyQIQ();

            Console.WriteLine("   Loading Dlls Complete");
            //search for 1-8-2014 for old lc settings
            //TO PIC build location
            //F:\ScottK\ToPIC\GlyQ-IQ Application\Release\
            //old args// \\picfs\projects\DMS\PIC_HPC\Hot\F_Std_V9_Local_ESI_SN138_21Dec_C15_2530_1\ApplicationFiles\GlyQ-IQ_Application\Release\IQGlyQ_Console.exe \\picfs\projects\DMS\PIC_HPC\Hot\F_Std_V9_Local_ESI_SN138_21Dec_C15_2530_1\RawData ESI_SN138_21Dec_C15_2530_1 raw L_13_HighMannose_TargetsFirstAll_1.txt GlyQIQ_Params_Velos4_SN138_L10PSA.txt \\picfs\projects\DMS\PIC_HPC\Hot\F_Std_V9_Local_ESI_SN138_21Dec_C15_2530_1\WorkingParameters Lock_1 \\picfs\projects\DMS\PIC_HPC\Hot\F_Std_V9_Local_ESI_SN138_21Dec_C15_2530_1\Results 1

            //standard args
            //"E:\ScottK\GetPeaks Data\Diabetes_LC" "Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12_0" "raw" "L_10_IQ_TargetsFirst3_0.txt" "GlyQIQ_Diabetes_Parameters.txt" "E:\ScottK\IQ\RunFiles" "Lock_0" "E:\ScottK\ToPic\Results" "1"


            //"\\picfs\projects\DMS\PIC_HPC\Home\RawData" "Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12_1" "raw" "L_10_IQ_TargetsFirstAll_10200.txt" "GlyQIQ_Diabetes_Parameters_PICFS.txt" "\\picfs\projects\DMS\PIC_HPC\Home\WorkingParameters" "Lock_0" "E:\ScottK\IQ\RunFiles" "777"
            // 8-28  "\\picfs\projects\DMS\PIC_HPC\Home\RawData" "Gly09_Velos3_Jaguar_200nL_C13_SN123_3X_23Dec12_R1" "raw" "L_10_IQ_TargetsFirstAll_10200.txt" "GlyQIQ_Diabetes_Parameters_PICFS.txt" "\\picfs\projects\DMS\PIC_HPC\Home\WorkingParameters" "Lock_0" "E:\ScottK\ToPic\Results" "33"
            //9-17-2013 "\\picfs\projects\DMS\PIC_HPC\Home\RawData" "Gly09_SN129_21Feb13_Cheetah_C14_230nL_SPIN_1700V_300mlmin_22Torr_100C_60kHDR2M" "raw" "L_10_IQ_TargetsFirstAll_44101.txt" "GlyQIQ_Diabetes_Parameters_PICFS_SPIN_SN129.txt" "\\picfs\projects\DMS\PIC_HPC\Home\WorkingParameters" "Lock_0" "E:\ScottK\ToPic\Results" "33"

            //PICFS Args

            //"\\picfs\projects\DMS\PIC_HPC\Home\RawData" "Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12" "raw" "L_10_IQ_TargetsFirstAll_0.txt" "GlyQIQ_Diabetes_Parameters_PICFS.txt" "\\picfs\projects\DMS\PIC_HPC\Home\WorkingParameters" "Lock_0" "E:\ScottK\ToPic\Results" "99"
            //"\\picfs\projects\DMS\PIC_HPC\Home\RawData" "Gly09_Velos3_Jaguar_200nL_C13_SN123_3X_23Dec12_R1" "raw" "L_10_IQ_TargetsFirstAll_10200.txt" "GlyQIQ_Diabetes_Parameters_PICFS.txt" "\\picfs\projects\DMS\PIC_HPC\Home\WorkingParameters" "Lock_0" "E:\ScottK\ToPic\Results" "33"

            //"\\picfs\projects\DMS\PIC_HPC\Home\RawData" "Gly09_Velos3_Jaguar_200nL_C13_SN123_3X_23Dec12_R1" "raw" "L_10_IQ_TargetsFirstAll_10200.txt" "GlyQIQ_Diabetes_Parameters_PICFS.txt" "\\picfs\projects\DMS\PIC_HPC\Home\WorkingParameters" "Lock_0" "E:\ScottK\ToPic\Results" "33"

            if (args.Length < 9)
            {
                Console.WriteLine("Error: this program requires that 9 command line arguments be provided:");

                Console.WriteLine(" DatasetFolderPath");
                Console.WriteLine(" DatasetName");
                Console.WriteLine(" DatasetEnding");
                Console.WriteLine(" TargetsFilePath");
                Console.WriteLine(" GlyQIqParameterFile");
                Console.WriteLine(" FileFolderPath");
                Console.WriteLine(" LocksFile");
                Console.WriteLine(" ResultsFolder");
                Console.WriteLine(" CoreID");

                return -5;
            }
            Console.WriteLine("There are " + args.Length + " args");


            //1.  args

            IQGlyQ.Objects.ParametersIQGlyQ parameters = new IQGlyQ.Objects.ParametersIQGlyQ();
            List<string> argsStrings = parameters.ParseArgsIQ(args);


            //testing PICfs
            //argsStrings[0] = @"\\pichvnas01\data\MassSpecOmic\Home\RawData";
            //argsStrings[1] = "Gly09_SN129_21Feb13_Cheetah_C14_230nL_SPIN_1700V_300mlmin_22Torr_100C_60kHDR2M_1";
            //argsStrings[2] = "raw";
            //argsStrings[3] = "L_10_IQ_TargetsFirstAll_R_8.txt";
            //argsStrings[4] = "GlyQIQ_Diabetes_Parameters_PICFS_SPIN_SN129.txt";
            //argsStrings[5] = @"\\pichvnas01\data\MassSpecOmic\Home\WorkingParameters";
            //argsStrings[6] = "Lock_8";
            //argsStrings[7] = @"\\pichvnas01\data\MassSpecOmic\Home\Results\Results";
            //argsStrings[8] = "8";



            //Work "E:\ScottK\GetPeaks Data\Diabetes_LC" "Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12_0" "raw" "L_10_IQ_TargetsFirst3_0.txt" "GlyQIQ_Diabetes_Parameters.txt" "E:\ScottK\IQ\RunFiles"

            //Home old//"Gly09_Velos3_Jaguar_200nL_C13_SN119SN122_3X_23Dec12" "L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data" "L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\SN111-SN117_OrbiStandard_workflowExecutorParametersSK_Home.xml"
            //Work old //"Gly09_Velos3_Jaguar_200nL_C13_SN119SN122_3X_23Dec12" "D:\Csharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data" "D:\Csharp\ConosleApps\LocalServer\IQ\Sarwall\SN111-SN117_OrbiStandard_workflowExecutorParametersSK_Work.xml"
            ////Work //"E:\ScottK\GetPeaks Data\Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_0.raw" "E:\ScottK\IQ\RunFiles\L_10_IQ_TargetsFirst3_0.txt" "E:\ScottK\IQ\NewIQRunFiles\GlyQIQ_Diabetes_Parameters.txt



            //List<string> mainParameterFromBatch;
            //ArgsProcessing(args, out mainParameterFromBatch);

            string datasetFolderPath = argsStrings[0];
            string datasetName = argsStrings[1];
            string datasetEnding = argsStrings[2];
            string targetsFilePath = argsStrings[3];
            string glyQIqParameterFile = argsStrings[4];
            string fileFolderPath = argsStrings[5];
            string locksFile = argsStrings[6];
            string resultsFolder = argsStrings[7];
            string coreID = argsStrings[8];

            //summing data:  
            //summing by 4 scans will decrease the average standfard deciation by 50%
            //suming by 9 scans will decrease the average standard deviation by 66%
            //summing by 16 scans will decrease the averge standar deviation by 75%



            bool overrideParams = false;
            bool singlecharge = false;



            if (overrideParams || singlecharge)
            {
                Console.WriteLine("TestingMode, Press a key to continue...");
                Console.ReadKey();
            }

            if (overrideParams)
            {
                string folder = @"D:\Csharp\0_TestDataFiles\FB_S_SN129_1";
                //datasetName = "S_SN129_1";
                //targetsFilePath = "L_13_Alditol_PSAX10_52.txt";
                //glyQIqParameterFile = "GlyQIQ_Params_SPIN_SN129_L10PSA.txt";


                folder = @"D:\Csharp\0_TestDataFiles\F_Std_S_SN129_1";
                datasetName = "S_SN129_1";
                targetsFilePath = "L_13_Alditol_5401-5402_1.txt";
                //targetsFilePath = "L_13_Alditol_5401-5402_b.txt";


                #region standard

                bool isVelos = false;
                if (isVelos)
                {
                    folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\F_Std_V10_PsaLac_ESI_SN138_21Dec_C15_2530_1";
                    folder = @"\\picfs\projects\DMS\PIC_HPC\Hot_Storage\F_Std_V10_PsaLac_ESI_SN138_21Dec_C15_2530_1 good";
                    folder = @"\\winhpcfs\Projects\DMS\PIC_HPC\Hot\F_HSum9_s5_ESI_SN138_21Dec_C15_2530_1";
                    //folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\FY_HighMannoseFuc_ESI_SN138_21Dec_C15_2530_1";
                    datasetName = "ESI_SN138_21Dec_C15_2530_1";
                    glyQIqParameterFile = "GlyQIQ_Params_Velos4_SN138_L10PSA.txt";
                    //glyQIqParameterFile = "GlyQIQ_Params_Velos4_SN138_L10PSAavg.txt";//15/15/3
                }

                bool isSPIN = false;
                if (isSPIN)
                {
                    folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\F_TestingLong3257D2_SPIN_SN138_16Dec13_C15_1";
                    //folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\F_Std_V9_PsaLac_SPIN_SN138_16Dec13_C15_1";
                    datasetName = "SPIN_SN138_16Dec13_C15_1";
                    glyQIqParameterFile = "Default.txt"; //9/9/0 //GlyQIQ_Params_SPIN_SN138_L10PSAC127

                }

                bool isVWF = true;
                if (isVWF)
                {
                    //folder = @"\\winhpcfs\Projects\DMS\PIC_HPC\Hot\F_SN_V12_L6_SPINV_VWF04B_19Mar14_10uL_230nL_120C_C15_1";
                    //datasetName = "SPINV_VWF04B_19Mar14_10uL_230nL_120C_C15_1";
                    folder = @"\\winhpcfs\projects\DMS\PIC_HPC\Hot\F_SN_V12_LPO4_SPINV_VWF10A_18Mar14_25uL_230nL_120C_C15_1";
                    datasetName = "SPINV_VWF10A_18Mar14_25uL_230nL_120C_C15_1";
                    glyQIqParameterFile = "Default.txt"; //9/9/0 //GlyQIQ_Params_SPIN_SN138_L10PSAC127

                }

                bool isCaSPIN = false;
                if (isCaSPIN)
                {
                    folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\F_CA_V10P006vS100V1_ESI_CAcid_L28d_300C_C16_1";
                    //folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\F_Std_V9_PsaLac_SPIN_SN138_16Dec13_C15_1";
                    datasetName = "ESI_CAcid_L28d_300C_C16_1";
                    glyQIqParameterFile = "Default.txt"; //9/9/0 //GlyQIQ_Params_SPIN_SN138_L10PSAC127

                }

                bool isCell = false;
                if (isCell)
                {
                    //folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\F_CellLines0127_Cell_00mM_1_C16_2";
                    folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\F_CellLines0127c_Cell_00mM_1_C16_1";
                    datasetName = "Cell_00mM_1_C16_1";
                    glyQIqParameterFile = "Default.txt"; //9/9/0 //GlyQIQ_Params_SPIN_SN138_L10PSAC127

                }
                //targetsFilePath = "L_13_HighMannose_TargetsFirstAll_wFucose_12.txt";//9-2-0-0-0
                //targetsFilePath = "L_10_IQ_TargetsFirstAll_R_1828.txt";//6-4-1-0
                //targetsFilePath = "L_10_IQ_TargetsFirstAll_R_552.txt";//6-4-1-0
                //targetsFilePath = "L_13_Alditol_LactoneCombo_2082.txt";//6-5-0-2-0
                //targetsFilePath = "L_13_Alditol_LactoneCombo_3128.txt";//
                //targetsFilePath = "L_10_IQ_ColominicAcid_907.txt";//or 907 //17
                //targetsFilePath = "L_10_PSA21_TargetsFirstAll_R_10.txt";//3-5-1.
                //targetsFilePath = "L_10_IQ_TargetsFirstAll_R_2172.txt";//5-4-3-3-0
                //targetsFilePath = "L_10_IQ_TargetsFirstAll_R_2029.txt";//6-3-1-1
                //targetsFilePath = "L_10_IQ_TargetsFirstAll_R_1504.txt";//
                //targetsFilePath = "L_10_IQ_TargetsFirstAll_R_484.txt";//6-6-1-1// used for 1da figurei n part 1
                //targetsFilePath = "L_10_IQ_TargetsFirstAll_R.txt";//
                //targetsFilePath = "L_10_IQ_TargetsFirstAll_R_1106.txt";//
                //targetsFilePath = "L_10_IQ_TargetsFirstAll_R_330.txt";//
                targetsFilePath = "L_13_Alditol_LactoneCombo_PO4_654.txt";

                #endregion


                #region colominic acid

                //velos
                //folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\F_CA_V7P006v2_ESI_ColominicAcid_22Dec13_3X_230nL_C16_2530_325C_1";
                //datasetName = "ESI_ColominicAcid_22Dec13_3X_230nL_C16_2530_325C_1";
                //targetsFilePath = "L_10_IQ_ColominicAcid_1082.txt";
                //glyQIqParameterFile = "GlyQIQ_Params_Velos4_CA_L10CA.txt";

                //SPIN
                //folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\F_CA_V9P006v1_SPIN_CAcid_18Dec_L30_140C_C16_1";
                //datasetName = "SPIN_CAcid_18Dec_L30_140C_C16_1";
                //targetsFilePath = "L_10_IQ_ColominicAcid_39.txt";
                //glyQIqParameterFile = "GlyQIQ_Params_Exact_SPIN_CA_L10CA007.txt";



                #endregion

                datasetFolderPath = folder + @"\" + "RawData";
                fileFolderPath = folder + @"\" + "WorkingParameters";
                coreID = "39";



            }

            Console.WriteLine("");
            Console.WriteLine("Args1: (datasetFolderPath) " + datasetFolderPath);
            Console.WriteLine("Args2: (datasetName)" + datasetName);
            Console.WriteLine("Args3: (datasetEnding)" + datasetEnding);
            Console.WriteLine("Args4: (targetsFilePath)" + targetsFilePath);
            Console.WriteLine("Args5: (glyQIqParameterFile)" + glyQIqParameterFile);
            Console.WriteLine("Args6: (fileFolderPath)" + fileFolderPath);
            Console.WriteLine("Args7: (locksFile)" + locksFile);
            Console.WriteLine("Args8: (resultsFolder)" + resultsFolder);
            Console.WriteLine("Args9: (coreID)" + coreID);
            Console.WriteLine("");

            int argsCount = args.Length;

            Console.WriteLine("There are " + argsCount + " out of 9 args");
            if (argsCount < 9)
            {
                Console.WriteLine("Missing key arguments; aborting");
                System.Threading.Thread.Sleep(3000);
                return -6;
            }


            //2.  load parameter file
            parameters.SetParameters(Path.Combine(fileFolderPath, glyQIqParameterFile));
            parameters.resultsFolderPath = resultsFolder + "_" + datasetName + "_" + coreID;

            //update parameters from divide targets paramaters and or args
            parameters.dataFolderPath = datasetFolderPath;
            parameters.testFile = datasetName;
            parameters.datasetEnding = datasetEnding;
            parameters.fileFolderPath = fileFolderPath;
            parameters.targetsFile = targetsFilePath;
            parameters.LocksFile = Path.Combine("LocksFolder", locksFile);
            //parameters.LocksFile = @"LocksFolder" + "_" + coreID + @"\" + locksFile;

            //3  setup parameters now that they are loaded
            string pathTestFile = Path.Combine(parameters.dataFolderPath,  parameters.testFile + "." + parameters.datasetEnding);
            //string pathPeaksfile = Path.Combine(parameters.peaksFolderPath,  parameters.peaksTestFile);
            string pathPeaksfile = Path.Combine(parameters.dataFolderPath,  parameters.testFile + "_peaks.txt");
            //Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12_0_peaks.txt
            string pathTargetsFile = Path.Combine(parameters.fileFolderPath,  parameters.targetsFile);
            string pathexecutorParameterFile = Path.Combine(parameters.fileFolderPath,  parameters.executorParameterFile);
            string pathFactors = Path.Combine(parameters.fileFolderPath,  parameters.factorsFile);
            string pathFragmentParameters = Path.Combine(parameters.fileFolderPath,  parameters.fragmentWorkFlowParameters);
            string filefolderPath = parameters.fileFolderPath;
            string lockLocation = Path.Combine(parameters.fileFolderPath,  parameters.LocksFile + ".txt");


            //pathTargetsFile = @"\\picfs\projects\DMS\PIC_HPC\FO_V_SN129_1\WorkingParameters\L_10_HighMannose_TargetsFirstAll_wFucose_15.txt";

            //from divide targets
            //Directory.CreateDirectory(parameters.fileFolderPath);
            StringListToDisk writer = new StringListToDisk();
            List<string> initialInformation = new List<string>();
            initialInformation.Add("running");

            Console.WriteLine("Write Lock...");
            writer.toDiskStringList(lockLocation, initialInformation);

            Console.WriteLine("Testing the key files...");

            IQ_FullDataset_Test tester = new IQ_FullDataset_Test();



            int goodFiles = 0;

            Console.WriteLine(Environment.NewLine + pathTestFile);
            if (File.Exists(pathTestFile)) { goodFiles++; Console.WriteLine("--Pass"); }

            Console.WriteLine(Environment.NewLine + pathTargetsFile);
            if (File.Exists(pathTargetsFile)) { goodFiles++; Console.WriteLine("--Pass"); }

            Console.WriteLine(Environment.NewLine + pathexecutorParameterFile);
            if (File.Exists(pathexecutorParameterFile)) { goodFiles++; Console.WriteLine("--Pass"); }

            Console.WriteLine(Environment.NewLine + pathFactors);
            if (File.Exists(pathFactors)) { goodFiles++; Console.WriteLine("--Pass"); }

            Console.WriteLine(Environment.NewLine + pathFragmentParameters);
            if (File.Exists(pathFragmentParameters)) { goodFiles++; Console.WriteLine("--Pass"); }

            Console.WriteLine(Environment.NewLine + filefolderPath);
            if (Directory.Exists(filefolderPath)) { goodFiles++; Console.WriteLine("--Pass"); }

            Console.WriteLine(Environment.NewLine + lockLocation);
            if (File.Exists(lockLocation)) { goodFiles++; Console.WriteLine("--Pass"); }

            Console.WriteLine(Environment.NewLine + pathPeaksfile);
            if (File.Exists(pathPeaksfile)) { goodFiles++; Console.WriteLine("--Pass"); }

            if (File.Exists(pathPeaksfile))
            {
                if (goodFiles != 8)
                {
                    Console.WriteLine("Error: Did not find 8 files (Missing File Top)");
                    System.Threading.Thread.Sleep(3000);
                    return -7;
                }
            }
            else
            {
                //we will create a new one
                if (goodFiles != 7)
                {
                    Console.WriteLine("Error: Did not find 7 files (Missing File Bottom)");
                    System.Threading.Thread.Sleep(3000);
                    return -6;
                }
            }

            //4  check for all files


            List<string> simpleOutput = tester.ExecuteMultipleTargetsTest2(pathTestFile, pathPeaksfile, parameters.resultsFolderPath, pathTargetsFile, pathexecutorParameterFile, pathFactors, parameters.loggingFolderPath, parameters.XYDataFolder, parameters.datasetEnding, pathFragmentParameters, filefolderPath, lockLocation, coreID, singlecharge);

            List<SmallDHResult> globalHitsIndexes = new List<SmallDHResult>();

            SmallDHResult hitToReport;
            SmallScoreCalculator.SelectForScore(simpleOutput, globalHitsIndexes, out hitToReport);

            Console.WriteLine(simpleOutput.Count + " results made it home");
            Console.WriteLine("Target Analysis Finished");

            return 0;
        }




        public static void ArgsProcessing(string[] args, out List<string> mainParameterFile)
        {
            mainParameterFile = new List<string>();

            #region switch from server to desktop based on number of args
            if (args.Length == 0)//debugging
            {
                mainParameterFile.Add(""); mainParameterFile.Add(""); mainParameterFile.Add("");
            }
            else
            {
                Console.WriteLine("ParseArgs");
                ParseStrings parser = new ParseStrings();
                mainParameterFile = Parse3ArgsIQ(args);
            }


            #endregion


            if (mainParameterFile.Count != 3)
            {
                Console.WriteLine("Args is not long enough; should have 3 arguments");
                System.Threading.Thread.Sleep(3000);
            }
        }

        public static List<string> Parse3ArgsIQ(string[] args)
        {
            List<string> paramatersStrings = new List<string>();
            paramatersStrings.Add("");
            paramatersStrings.Add("");
            paramatersStrings.Add("");

            string[] words = { };
            string argument1 = args[0];
            Console.WriteLine(args[1]);
            words = argument1.Split(Environment.NewLine.ToCharArray(),  //'\n', '\r'
                StringSplitOptions.RemoveEmptyEntries);

            int countArguments = 0;
            foreach (string argument in args)
            {
                //Console.WriteLine(argument);
                countArguments++;
            }
            if (countArguments == 3)
            {
                paramatersStrings[0] = args[0];
                paramatersStrings[1] = args[1];
                paramatersStrings[2] = args[2];
            }
            else
            {
                Console.WriteLine("MissingArguments.  There are: ", countArguments);
                Console.ReadKey();
            }
            return paramatersStrings;
        }
    }
}
