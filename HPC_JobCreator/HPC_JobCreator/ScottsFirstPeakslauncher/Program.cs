using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HPC_Connector;
using HPC_Submit;

namespace ScottsFirstPeakslauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Do you want to make peaks, press a key to continue");
            Console.ReadKey();
            
            string fileSystem = @"\\picfs.pnl.gov";
            string workDirectory = fileSystem;

            //sum5
            string parameterFileWithPath = "";

            string exeHomeLocationDirectory = fileSystem + @"\" + @"projects\DMS\PIC_HPC\Azure\GlyQ-IQ_HammerPeakDetector\Release\HammerPeakDetectorConsole.exe";
                

            string clusterName = "Deception2.pnnl.gov";//deception 2
            string workerNodeGroup = "PIC";
            string templateName = "GlyQIQ";
            //string templateName = "PrePost";
            
            List<string> dataFilesNoEnding = new List<string>();
            List<string> dataFilesEnding = new List<string>();


            HardwareUnitType unitType = HardwareUnitType.Node;

            bool isDB = false;
            if (isDB)
            {
                workDirectory = fileSystem + @"\" + @"projects\DMS\PIC_HPC\Azure\MasterV1";
                
                //sum5
                parameterFileWithPath = fileSystem + @"\" + @"projects\DMS\PIC_HPC\Home\WorkingParameters\HPC_Diabetes_PeakDetector_Parameters.txt";


                dataFilesNoEnding.Add("DB1_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("DB2_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("DB3_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("DB4_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("DB5_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("DB6_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("DB7_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("DB8_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("DB9_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("DB10_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("DB11_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("DB12_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("DB13_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("DB14_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("DB15_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("DB16_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("DB17_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("DB18_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("DB19_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("DB20_1"); dataFilesEnding.Add("raw");

                dataFilesNoEnding.Add("SN111SN114_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("SN112SN115_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("SN113SN116_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("SN117SN120_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("SN118SN121_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("SN119SN122_1"); dataFilesEnding.Add("raw");
            }
           

            bool isGlucose = false;
            if (isGlucose)
            {
                workDirectory = fileSystem + @"\" + @"projects\DMS\PIC_HPC\Home\RawData\CellLinesShort";

                parameterFileWithPath = fileSystem + @"\" + @"projects\DMS\PIC_HPC\Home\WorkingParameters\HPC_Cell_PeakDetector_Parameters.txt";
                
                dataFilesNoEnding.Add("Cell_00mM_1_C16_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_00mM_2_C15_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_00mM_3_C16_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_00mM_4_C15_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_00mM_5_C16_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_05mM_1_C15_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_05mM_2_C16_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_05mM_3_C15_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_05mM_4_C16_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_05mM_5_C15_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_10mM_1_C16_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_10mM_2_C15_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_10mM_3_C15_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_10mM_4_C15_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_10mM_5_C16_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_30mM_1_C15_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_30mM_2_C16_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_30mM_3_C15_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_30mM_4_C16_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_30mM_5_C15_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_50mM_1_C15_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_50mM_2_C15_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_50mM_3_C16_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_50mM_4_C15_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_50mM_5_C15_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_Norm2_12_C16_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_Norm2_14_C16_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Cell_Norm2_15_C16_1"); dataFilesEnding.Add("raw");
                

            }

             //bool isSpinExactive = true;
             //if (isSpinExactive)
             //{
             //    workDirectory = fileSystem + @"\" + @"projects\DMS\PIC_HPC\Home\RawData";

             //    parameterFileWithPath = fileSystem + @"\" + @"projects\DMS\PIC_HPC\Home\WorkingParameters\HPC_SpinExactive_PeakDetector_Parameters.txt";

             //    dataFilesNoEnding.Add("ESI_05Mar14_150C_055V_C16_1"); dataFilesEnding.Add("raw");
             //    dataFilesNoEnding.Add("ESI_05Mar14_150C_190V_C15_1"); dataFilesEnding.Add("raw");
             //    dataFilesNoEnding.Add("ESI_05Mar14_200C_055V_C16_1"); dataFilesEnding.Add("raw");
             //    dataFilesNoEnding.Add("ESI_05Mar14_200C_190_C15_1"); dataFilesEnding.Add("raw");
             //    dataFilesNoEnding.Add("ESI_05Mar14_300C_055V_C16_1"); dataFilesEnding.Add("raw");
             //    dataFilesNoEnding.Add("ESI_05Mar14_300C_190V_C15_1"); dataFilesEnding.Add("raw");
             //    dataFilesNoEnding.Add("ESI_05Mar14_300C_C16_1"); dataFilesEnding.Add("raw");
             //}

            bool isSpinExactiveMarch = true;
            if (isSpinExactiveMarch)
            {
                workDirectory = fileSystem + @"\" + @"projects\DMS\PIC_HPC\Home\RawData";

                parameterFileWithPath = fileSystem + @"\" + @"projects\DMS\PIC_HPC\Home\WorkingParameters\HPC_SpinExactive_PeakDetector_Parameters.txt";

                //dataFilesNoEnding.Add("ESI_SN142_12Mar14_60uL_230nL_150C_190V_C16_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("SPINV_SN142_14Mar14_60uL_230nL_120C_C16_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("SPINV_SN143_14Mar14_60uL_230nL_120C_C16_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("SPINV_SN148_14Mar14_60uL_230nL_120C_C15_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("SPINV_SN144_16Mar14_60uL_230nL_120C_C16_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("SPINV_SN145_16Mar14_60uL_230nL_120C_C16_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("SPINV_SN150_16Mar14_60uL_230nL_120C_C15_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("SPINV_SN149_16Mar14_60uL_230nL_120C_C15_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("SPINV_SN146_16Mar14_60uL_230nL_120C_C16_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("SPINV_SN147_16Mar14_60uL_230nL_120C_C16_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("SPINV_SN142_17Mar14_60uL_230nL_120C_C16_1"); dataFilesEnding.Add("raw");

                //dataFilesNoEnding.Add("SPINV_VWF04A_17Mar14_25uL_230nL_120C_C15_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("SPINV_VWF10A_17Mar14_25uL_230nL_120C_C15_1"); dataFilesEnding.Add("raw");

                //dataFilesNoEnding.Add("SPINV_SN144_17Mar14_60uL_230nL_120C_C15_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("SPINV_SN144_17Mar14_60uL_230nL_120C_C16_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("SPINV_SN151_17Mar14_60uL_230nL_120C_C16_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("SPINV_SN144_18Mar14_60uL_230nL_120C_C16_1"); dataFilesEnding.Add("raw");

                //dataFilesNoEnding.Add("SPINV_VWF04A_18Mar14_25uL_230nL_120C_C15_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("SPINV_SN146_18Mar14_60uL_230nL_120C_C16_2_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("SPINV_SN146_19Mar14_60uL_230nL_120C_C16_3_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("SPINV_VWF04B_19Mar14_10uL_230nL_120C_C15_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("SPINV_VWF10B_19Mar14_10uL_230nL_120C_C15_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("SPINV_VWF10A_18Mar14_25uL_230nL_120C_C15_1"); dataFilesEnding.Add("raw");

                //dataFilesNoEnding.Add("SPINV_Blank03_18Mar14_60uL_230nL_120C_C15_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("SPINV_Blank04_19Mar14_60uL_230nL_120C_C15_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("SPINV_SN144_18Mar14_60uL_230nL_120C_C15_1_Blank"); dataFilesEnding.Add("raw");

                //dataFilesNoEnding.Add("SPINV_SN144_18Mar14_60uL_230nL_120C_C16_1a"); dataFilesEnding.Add("raw");
                
                

                //dataFilesNoEnding.Add("Velos_SN150_21Mar14_60uL_230nL_120C_C16_3_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("SPINV_SN150_20Mar14_60uL_230nL_120C_C16_3_1"); dataFilesEnding.Add("raw");
                
                //dataFilesNoEnding.Add("SPINV_SN146_16Mar14_60uL_230nL_120C_C16_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("SPINV_Cell10-3_19Mar14_60uL_230nL_120C_C16_1"); dataFilesEnding.Add("raw");

                //dataFilesNoEnding.Add("SPINV_SN142_17Mar14_60uL_230nL_120C_C16_1"); dataFilesEnding.Add("raw");

                //done

                //dataFilesNoEnding.Add("ESI_SN157_15May14_60uL_230nL_150C_C16_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("ESI_SN158_15May14_60uL_230nL_150C_C15_1"); dataFilesEnding.Add("raw");
                
                //dataFilesNoEnding.Add("ESI_SN159_15May14_60uL_230nL_150C_C16_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("ESI_SN160_15May14_60uL_230nL_150C_C15_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("ESI_SN160_15May14_60uL_230nL_150C_C16_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("ESI_SN159_15May14_60uL_230nL_150C_C15_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("ESI_SN158_15May14_60uL_230nL_150C_C16_1"); dataFilesEnding.Add("raw");
                //dataFilesNoEnding.Add("ESI_SN157_15May14_60uL_230nL_150C_C15_1"); dataFilesEnding.Add("raw");

                dataFilesNoEnding.Add("Gly08_V4_200nL_D60A_1X_C1_2Sept12_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Gly08_V4_200nL_D60B_1X_C2_3Sept12_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Gly08_V4_200nL_D60C_1X_C2_4Sept12_1"); dataFilesEnding.Add("raw");
                                              
                dataFilesNoEnding.Add("Gly08_V4_200nL_D3030A_1X_C2_2Sept12_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Gly08_V4_200nL_D3030B_1X_C1_3Sept12_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Gly08_V4_200nL_D3030C_1X_C1_4Sept12_1"); dataFilesEnding.Add("raw");
                                              
                dataFilesNoEnding.Add("Gly08_V4_200nL_FT60A_1X_C1_2Sept12_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Gly08_V4_200nL_FT60B_1X_C2_3Sept12_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Gly08_V4_200nL_FT60C_1X_C2_4Sept12_1"); dataFilesEnding.Add("raw");
                                              
                dataFilesNoEnding.Add("Gly08_V4_200nL_FT3030A_1X_C2_2Sept12_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Gly08_V4_200nL_FT3030B_1X_C2_3Sept12_1"); dataFilesEnding.Add("raw");
                dataFilesNoEnding.Add("Gly08_V4_200nL_FT3030C_1X_C2_4Sept12_1"); dataFilesEnding.Add("raw");

            }

            for(int i=0;i<dataFilesNoEnding.Count;i++)
            {
                string datafileName = dataFilesNoEnding[i];
                string ending = dataFilesEnding[i];
                
                //\\picfs.pnl.gov\projects\DMS\PIC_HPC\Azure\MasterV1\GlyQ-IQ_HammerPeakDetector\Release\HammerPeakDetectorConsole.exe
                //\\picfs.pnl.gov\projects\DMS\PIC_HPC\Azure\MasterV1\HPC_Diabetes_PeakDetector_Parameters.txt
                //DB1_1
                //raw
                HPC_Connector.JobToHPC sendMe = SetUpNewJob(unitType, datafileName, ending, parameterFileWithPath, exeHomeLocationDirectory, workDirectory, workerNodeGroup, clusterName, templateName);
                HPC_Submit.WindowsHPC2012 toScheduler = new WindowsHPC2012();
                toScheduler.Send(sendMe);
            }
        }

        private static HPC_Connector.JobToHPC SetUpNewJob(HardwareUnitType unitType, string datafileName, string ending, string parameterFileWithPath, string exeHomeLocationDirectory, string workDirectory, string workerNodeGroup, string clusterName, string templateName)
        {
            int coresToUse = 1;
            int lastTarget = 1;

            ParametersCluster newClusterParameters = new ParametersCluster(clusterName);
            //newClusterParameters.WorkerNodeGroup = workerNodeGroup;

            

            string jobNameMustBeShort = coresToUse + " " + unitType.ToString() + " PeakDetect " + datafileName;

            List<char> letters = jobNameMustBeShort.ToList();
            if (jobNameMustBeShort.Length > 80)
            {
                jobNameMustBeShort = "";
                for (int i = 0; i < 80; i++)
                {
                    jobNameMustBeShort += letters[i];
                }
            }

            ParametersJob newJobParameters = new ParametersJob(jobNameMustBeShort);
            newJobParameters.MaxNumberOfCores = coresToUse;
            newJobParameters.MinNumberOfCores = 1;
            //newJobParameters.PriorityLevel = PriorityLevel.AboveNormal;
            newJobParameters.PriorityLevel = PriorityLevel.Normal;
            newJobParameters.ProjectName = "GlyQIQ";
            newJobParameters.TargetHardwareUnitType = unitType;
            newJobParameters.TemplateName = templateName;
            newJobParameters.isExclusive = true;

            Random rand = new Random();
            int taskNumber = rand.Next(1, 1000);
            string cmdNewLine = @"&";
            string q = "\"";
            string star = @"\*";

            string glyQIQConsoleCommandLine = SetCommandLine(exeHomeLocationDirectory, parameterFileWithPath, datafileName, ending);

            ParametersTask sweepGlyQIQParameters = new ParametersTask("GlyQIQPeaks");
            sweepGlyQIQParameters.CommandLine = glyQIQConsoleCommandLine;
            sweepGlyQIQParameters.ParametricStartIndex = 1;
            sweepGlyQIQParameters.ParametricStopIndex = lastTarget;
            sweepGlyQIQParameters.ParametricIncrement = 1;

            sweepGlyQIQParameters.StdOutFilePath = workDirectory + @"\" + @"test" + datafileName + @".log";
            sweepGlyQIQParameters.TaskTypeOption = HPCTaskType.Basic;
            sweepGlyQIQParameters.WorkDirectory = workDirectory;

            HPC_Connector.JobToHPC sendMe = new JobToHPC(clusterName, newJobParameters.JobName, sweepGlyQIQParameters.TaskName);
            sendMe.JobParameters = newJobParameters;
            sendMe.TaskParameters = sweepGlyQIQParameters;
            sendMe.SubsequentTaskParameters.Add(CreateWaitTask(45));//25 will work on picfs
            sendMe.ClusterParameters = newClusterParameters;
            return sendMe;
        }


        private static ParametersTask CreateWaitTask(int seconds = 30)
        {
            string cmdNewLine = @"&";
            ParametersTask wait60seconds = new ParametersTask("Wait");
            wait60seconds.TaskTypeOption = HPCTaskType.Basic;
            wait60seconds.CommandLine = @"ping 1.1.1.1 -n 1 -w " + seconds * 1000 + " > nul " + cmdNewLine + " echo Success";
            return wait60seconds;
        }

        private static string SetCommandLine(string exeHomeLocationDirectory, string parameterFileWithPath, string datafileName, string ending)
        {
            string fullCommandLine = "";

            string cmdNewLine = @"&";
            string q = "\"";
            string star = @"\*";

            List<string> commandLines = new List<string>();

            commandLines.Add(q + exeHomeLocationDirectory + q);
            commandLines.Add(q + parameterFileWithPath + q);
            commandLines.Add(datafileName);
            commandLines.Add(ending);

            fullCommandLine = String.Join(" ", commandLines);

            return fullCommandLine;
        }
    }
}
