using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Hpc.Scheduler;
using Microsoft.Hpc.Scheduler.Properties;
using HPC_JobCreator;

namespace HPCSchedulerBasics
{
    internal class ProgramTask
    {
        private static void Main(string[] args)
        {
            //SEND A SINGLE TASK TO THE HPC   _44 = task number 44

            //string datafileName = "Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12_1";
            //string datafileName = "Gly09_Velos3_Jaguar_200nL_C13_SN123_3X_23Dec12_R1_1";
            //string datafileName = "Gly09_Velos3_Jaguar_200nL_C12_SN129_3X_23Dec12_1";
            //string datafileName = "Peptide_Merc_06_2Feb11_Sphinx_11-01-17_1";
            string datafileName = "SPIN_SN138_16Dec13_C15_1";
            //string datafileName = "Gly09_SN129_21Feb13_Cheetah_C14_230nL_SPIN_1700V_300mlmin_22Torr_100C_60kHDR2M_1";
            //string library = "L_10_PSA21_Cell_TargetsFirstAll_R";
            //string library = "L_10_IQ_TargetsFirstAll_52000-6200"; int taskNumber = 1;
            string library = "L_13_Alditol_LactoneCombo"; int taskNumber = 13000;
            
            //string datafileName = "Gly09_SN129_21Feb13_Cheetah_C14_230nL_SPIN_1700V_300mlmin_22Torr_100C_60kHDR2M_1";
            //string library = "L_10_IQ_TargetsFirstAll_52000-6200"; int taskNumber = 36;
            
            int cores = 1;

            //taskNumber = 3257;

            //datafileName = args[0];
            //library = args[1];
            //cores = Convert.ToInt32(args[2]);
            //string workDirectoryIPPath = @"\\172.16.112.12\projects\DMS\PIC_HPC\Home";//Standard IP Run
            //string workDirectory = @"\\172.16.112.12\projects\DMS\PIC_HPC\Home";

            //string workDirectoryIPPath = @"\\picfs\projects\DMS\PIC_HPC\Home";//Standard IP Run
            //string workDirectory = @"\\picfs\projects\DMS\PIC_HPC\Home";
            //string logDirectoryIPPath = @"\\picfs\projects\DMS\PIC_HPC\Home";

            string workDirectory = @"\\picfs\projects\DMS\PIC_HPC\Hot\F_Testing19391_SPIN_SN138_16Dec13_C15_1";
            string workDirectoryIPPath = workDirectory;
            string logDirectoryIPPath = workDirectory;

            string glyQIQconsoleOperatingParameters_WithEnding = "Default.txt";

            string parameterFile = glyQIQconsoleOperatingParameters_WithEnding;
            //string parameterFile = "GlyQIQ_Diabetes_Parameters_PICFS_SPIN_SN129.txt";

            //string exeHomeLocationDirectory = @"\\172.16.112.12\projects\DMS\ScottK\ScottK_PUB-100X_Launch_Folder\ToPIC";
            string exeHomeLocationDirectory = workDirectory;

            //string workerNodeGroup = "Kronies";
            string workerNodeGroup =  "ComputeNodes"; //"PIC";

            bool isKronies = true;//need PICFS

            bool isAzure = false;
            if (isKronies)
            {
                //workDirectoryIPPath = @"\\picfs\projects\DMS\PIC_HPC\Home";//kronies.  also update in Send below (1/3) Workes with ComputeNodes but slower
                //workDirectory = @"\\picfs\projects\DMS\PIC_HPC\Home";
                //logDirectoryIPPath = @"\\picfs\projects\DMS\PIC_HPC\Home";
                //exeHomeLocationDirectory = @"\\picfs\projects\DMS\ScottK\ScottK_PUB-100X_Launch_Folder\ToPIC";

                //workDirectory = @"\\picfs\projects\DMS\PIC_HPC\FieldOffice";
                //workDirectoryIPPath = @"\\picfs\projects\DMS\PIC_HPC\FieldOffice";
                //logDirectoryIPPath = @"\\picfs\projects\DMS\PIC_HPC\FieldOffice";
                //exeHomeLocationDirectory = @"\\picfs\projects\DMS\ScottK\ScottK_PUB-100X_Launch_Folder\ToPIC";

                if (isAzure)
                {
                    workDirectory = @"%CCP_PACKAGE_ROOT%\FieldOffice";
                    workDirectoryIPPath = @"%CCP_PACKAGE_ROOT%\FieldOffice";
                    logDirectoryIPPath = @"%CCP_PACKAGE_ROOT%\FieldOffice";
                    exeHomeLocationDirectory = @"%CCP_PACKAGE_ROOT%\FieldOffice\Applications";
                }
            }
           
            
            

            SendTask senderTask = new SendTask();
            //sender.Send(cores, datafileName, library, taskNumber);
            senderTask.Send(cores, datafileName, library, workDirectory, parameterFile, workDirectoryIPPath, taskNumber, exeHomeLocationDirectory, workerNodeGroup);

            Console.WriteLine("Do you want to restart the consolidator when done?");
            Console.WriteLine("Press y for restart");
            ConsoleKeyInfo input = Console.ReadKey();
            if (input.Key == ConsoleKey.Y)
            {
                Utilities.RunCMD(@"\\picfs\projects\DMS\PIC_HPC\Home", "1_HPC_StartCollectResults.bat");
            }
            
        }
    }
}