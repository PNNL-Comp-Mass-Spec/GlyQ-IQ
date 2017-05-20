using System;
using System.Collections.Generic;
using System.IO;
using GetPeaks_DLL.ConosleUtilities;
using GetPeaks_DLL.DataFIFO;
using IQGlyQ;
using IQGlyQ.FIFO;
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

        static void Main(string[] args)
        {
            
            Console.WriteLine("2-19-2014 815am");
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

            if(overrideParams || singlecharge)
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
                glyQIqParameterFile = "Default.txt";


                //datasetName = "Gly09_Velos3_Jaguar_200nL_C13_SN123_3X_23Dec12_R1_1";
                //targetsFilePath = "L_10_IQ_TargetsFirstAll_R_65.txt";
                //targetsFilePath = "L_10_HighMannose_TargetsFirstAll_wFucose.txt";

                //datasetName = "Gly08_Velos4_Jaguar_200nL_D3030A_1X_C2_2Sept12_1";
                //targetsFilePath = "L_10_PSA21_TargetsFirstAll_R_256.txt";
                //targetsFilePath = "L_10_PSA21_TargetsFirstAll_R_98.txt";
                //glyQIqParameterFile = "GlyQIQ_Diabetes_Parameters_PICFS_Velos_SN129_L10PSA.txt";

                //datasetName = "V_SN129_1";
                //targetsFilePath = "L_10_IQ_TargetsFirstAll_52000-6200.txt";
                //glyQIqParameterFile = "GlyQIQ_Params_Velos_SN129_L10PSA.txt";

                //ecoli testing
                //datasetName = "Peptide_Merc_06_2Feb11_Sphinx_11-01-17_1";
                //targetsFilePath = "L_11_AlditolNeutral_42.txt";
                //glyQIqParameterFile = "GlyQIQ_Params_Velos_SN129_L10PSA.txt";
                //fileFolderPath = @"\\picfs\projects\DMS\PIC_HPC\Hot\FB_Peptide_Merc_06_2Feb11_Sphinx_11-01-17_1\WorkingParameters";
                //coreID = "36";

                //cell testing
                //string folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\FB_ESI_Cell_Norm1_26Nov13_40uL_230nL_C14_1";

                //folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\FR_Gly09_Velos3_Jaguar_200nL_C12_AntB1_3X_25Dec12_1";
                //datasetName = "Gly09_Velos3_Jaguar_200nL_C12_AntB1_3X_25Dec12_1";
                //targetsFilePath = "L_13_Ant2Xylos_4.txt";
                //glyQIqParameterFile = "GlyQIQ_Params_Velos_SN129_Lant.txt";

                //folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\FA_Gly09_Velos3_Jaguar_200nL_C13_AntT1_3X_25Dec12_1";
                //datasetName = "Gly09_Velos3_Jaguar_200nL_C13_AntT1_3X_25Dec12_1";
                //targetsFilePath = "L_13_Ant2Xylos_13.txt";
                //glyQIqParameterFile = "GlyQIQ_Params_Velos_SN129_Lant.txt";

               
                //A is with N divisor
                //D si no N divisor
                //E is with N divisor at 0.2
                //F is hammer2
                //G is hammer0
                
                //folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\FG_V_SN129_1";
                //datasetName = "V_SN129_1";
                //targetsFilePath = "L_13_Alditol_LactoneCombo_1184.txt";
                //glyQIqParameterFile = "GlyQIQ_Params_Velos_SN129_L10PSA.txt";

                //ecoli with N
                //int target = 8193;//2194//220/378//1325//3518//8634//5327//8193
                //folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\FY_Peptide_Merc_06_2Feb11_Sphinx_11-01-17_1";
                //datasetName = "Peptide_Merc_06_2Feb11_Sphinx_11-01-17_1";
                //targetsFilePath = "L_13_Alditol_LactoneCombo_"+ target +".txt";
                //glyQIqParameterFile = "GlyQIQ_Params_Velos_SN129_L10PSA.txt";
                //coreID = "8193";

                //folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\F_Std000fullb_SPIN_SN138_16Dec13_40uL_230nL_140C_300mL_22T_C15_1";
                //datasetName = "SPIN_SN138_16Dec13_40uL_230nL_140C_300mL_22T_C15_1";
                //targetsFilePath = "L_13_HighMannose_TargetsFirstAll_wFucose_10.txt";
                //glyQIqParameterFile = "GlyQIQ_Diabetes_Parameters_PICFS_SPIN_SN138.txt";

                //folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\F_Std95Corr1Fit85LMHMb_ESI_SN138_21Dec_C15_2530_1";
                //datasetName = "ESI_SN138_21Dec_C15_2530_1";
                //targetsFilePath = "L_13_HighMannose_TargetsFirstAll_wFucose_8.txt";
                //glyQIqParameterFile = "GlyQIQ_Params_Velos4_SN138_L10PSA.txt";

                //folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\F_CA_V2_ESI_ColominicAcid_22Dec13_3X_230nL_C16_2530_325C_1";
                //datasetName = "ESI_ColominicAcid_22Dec13_3X_230nL_C16_2530_325C_1";
                //targetsFilePath = "L_10_IQ_ColominicAcid_1.txt";
                //glyQIqParameterFile = "GlyQIQ_Params_Velos4_CA_L10CA.txt";

                //folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\F_CA_V4_SPIN_CAcid660uM_18Dec13_40uL_230nL_140C_300mL_22T_C16_1";
                //datasetName = "SPIN_CAcid660uM_18Dec13_40uL_230nL_140C_300mL_22T_C16_1";
                //targetsFilePath = "L_10_IQ_ColominicAcid_840.txt";
                //glyQIqParameterFile = "GlyQIQ_Params_Exact_SPIN_CA_L10CA.txt";

                //folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\F_CA_V6_ESI_ColominicAcid_22Dec13_3X_230nL_C16_2530_325C_1";
                //datasetName = "ESI_ColominicAcid_22Dec13_3X_230nL_C16_2530_325C_1";
                //targetsFilePath = "L_10_IQ_ColominicAcid_1306.txt";
                //glyQIqParameterFile = "GlyQIQ_Params_Velos4_CA_L10CA.txt";

                #region standard

                bool isVelos = false;
                if (isVelos)
                {
                    folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\F_Std_V10_PsaLac_ESI_SN138_21Dec_C15_2530_1";
                    //folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\FY_HighMannoseFuc_ESI_SN138_21Dec_C15_2530_1";
                    datasetName = "ESI_SN138_21Dec_C15_2530_1";
                    glyQIqParameterFile = "GlyQIQ_Params_Velos4_SN138_L10PSA.txt";
                    //glyQIqParameterFile = "GlyQIQ_Params_Velos4_SN138_L10PSAavg.txt";//15/15/3
                }

                bool isSPIN = true;
                if (isSPIN)
                {
                    folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\F_TestingLong512_SPIN_SN138_16Dec13_C15_1";
                    //folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\F_Std_V9_PsaLac_SPIN_SN138_16Dec13_C15_1";
                    datasetName = "SPIN_SN138_16Dec13_C15_1";
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
                //targetsFilePath = "L_13_HighMannose_TargetsFirstAll_wFucose_12.txt";//9-2-0-0-0
                //targetsFilePath = "L_10_IQ_TargetsFirstAll_R_1828.txt";//6-4-1-0
                //targetsFilePath = "L_10_IQ_TargetsFirstAll_R_552.txt";//6-4-1-0
                targetsFilePath = "L_10_PSA21_TargetsFirstAll_R_0.txt";//special 32000
                //targetsFilePath = "L_13_Alditol_LactoneCombo_2082.txt";//6-5-0-2-0
                //targetsFilePath = "L_13_Alditol_LactoneCombo_3128.txt";//
                //targetsFilePath = "L_10_IQ_ColominicAcid_907.txt";//or 907 //17

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
            if (argsCount != 9)
            {
                Console.WriteLine("Missing Targets File, Raw File, parameter file");
                Console.ReadKey();
            }


            //2.  load parameter file
            parameters.SetParameters(fileFolderPath + "\\" + glyQIqParameterFile);
            parameters.resultsFolderPath = resultsFolder + "_" + datasetName + "_" + coreID;

            //update parameters from divide targets paramaters and or args
            parameters.dataFolderPath = datasetFolderPath;
            parameters.testFile = datasetName;
            parameters.datasetEnding = datasetEnding;
            parameters.fileFolderPath = fileFolderPath;
            parameters.targetsFile = targetsFilePath;
            parameters.LocksFile = @"LocksFolder" + @"\" + locksFile;
            //parameters.LocksFile = @"LocksFolder" + "_" + coreID + @"\" + locksFile;

            //3  setup parameters now that they are loaded
            string sh = "\\";
            string pathTestFile = parameters.dataFolderPath + sh + parameters.testFile + "." + parameters.datasetEnding;
            //string pathPeaksfile = parameters.peaksFolderPath + sh + parameters.peaksTestFile;
            string pathPeaksfile = parameters.dataFolderPath + sh + parameters.testFile + "_peaks.txt";
            //Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12_0_peaks.txt
            string pathTargetsFile = parameters.fileFolderPath + sh + parameters.targetsFile;
            string pathexecutorParameterFile = parameters.fileFolderPath + sh + parameters.executorParameterFile;
            string pathFactors = parameters.fileFolderPath + sh + parameters.factorsFile;
            string pathFragmentParameters = parameters.fileFolderPath + sh + parameters.fragmentWorkFlowParameters;
            string filefolderPath = parameters.fileFolderPath;
            string lockLocation = parameters.fileFolderPath + sh + parameters.LocksFile + ".txt";


            //pathTargetsFile = @"\\picfs\projects\DMS\PIC_HPC\FO_V_SN129_1\WorkingParameters\L_10_HighMannose_TargetsFirstAll_wFucose_15.txt";

            //from divide targets
            //Directory.CreateDirectory(parameters.fileFolderPath);
            GetPeaks_DLL.DataFIFO.StringListToDisk writer = new StringListToDisk();
            List<string> initialInformation = new List<string>();
            initialInformation.Add("running");

            Console.WriteLine("Write Lock...");
            writer.toDiskStringList(lockLocation, initialInformation);

            Console.WriteLine("Testing the key files...");

            IQ_FullDataset_Test tester = new IQ_FullDataset_Test();



            int goodFiles = 0;

            Console.WriteLine(Environment.NewLine + pathTestFile);
            if (File.Exists(pathTestFile)) {goodFiles++; Console.WriteLine("--Pass");}

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
                    Console.WriteLine("Missing File Top");
                    Console.ReadKey();
                }
            }
            else
            {
                //we will create a new one
                if (goodFiles != 7)
                {
                    Console.WriteLine("Missing File Bottom");
                    Console.ReadKey();
                }
            }

            Console.WriteLine("Press enter to continue");
            //Console.ReadKey();

            //4  check for all files





            tester.ExecuteMultipleTargetsTest2(pathTestFile, pathPeaksfile, parameters.resultsFolderPath, pathTargetsFile, pathexecutorParameterFile, pathFactors, parameters.loggingFolderPath, parameters.XYDataFolder, parameters.datasetEnding, pathFragmentParameters, filefolderPath, lockLocation, coreID, singlecharge);
        
            
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
                Console.WriteLine("Args is not long enough");
                Console.ReadKey();
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
