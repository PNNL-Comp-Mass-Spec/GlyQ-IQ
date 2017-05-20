using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HPC_Connector;
using HPC_Submit;
using Microsoft.Hpc.Scheduler;
using Microsoft.Hpc.Scheduler.Properties;

namespace HPC_TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string clusterName = "Deception.pnl.gov";
            string jobName = "MyFirstJob";
            string taskName = "MyFirstTask";
            HPC_Connector.JobToHPC currentJob = new HPC_Connector.JobToHPC(clusterName, jobName, taskName);



            int cores = 9;
            string datafileName = "V_SN129_1";
            string workDirectory = @"\\picfs\projects\DMS\PIC_HPC\Hot\F_Testing_V_SN129_1";
            string targetsFile = "L_13_HighMannose_TargetsFirstAll";
            string parameterFile = "GlyQIQ_Params_Velos_SN129_L10PSA.txt";
            currentJob.ClusterParameters.ClusterName = "Deception.pnl.gov";
            //currentJob.ClusterParameters.WorkerNodeGroup = "ComputeNodes";

            currentJob.JobParameters.JobName = SetName(cores, datafileName);
            currentJob.JobParameters.MinNumberOfCores = 1;
            currentJob.JobParameters.MaxNumberOfCores = cores;
            currentJob.JobParameters.PriorityLevel = PriorityLevel.AboveNormal;
            currentJob.JobParameters.ProjectName = "PIC";
            currentJob.JobParameters.TargetHardwareUnitType = HardwareUnitType.Core;

            currentJob.TaskParameters.TaskName = cores + "Core_RealDataD10_NoCall Raw_star";
            currentJob.TaskParameters.CommandLine = SetCommandLine2(workDirectory, workDirectory, datafileName,targetsFile, parameterFile);
            currentJob.TaskParameters.WorkDirectory = workDirectory;           
            currentJob.TaskParameters.StdOutFilePath = workDirectory + @"\" + @"Results\test" + datafileName + "_" + "*" + @".log";
            currentJob.TaskParameters.TaskTypeOption = HPCTaskType.ParametricSweep;
            currentJob.TaskParameters.ParametricStartIndex = 1;
            currentJob.TaskParameters.ParametricStopIndex = cores;
            currentJob.TaskParameters.ParametricIncrement = 1;

            //\\picfs\projects\DMS\PIC_HPC\Hot\F_Testing_V_SN129_1
            //\\picfs\projects\DMS\PIC_HPC\Hot\F_Testing_V_SN129_1\Results\testV_SN129_1_*.log

            var myCluster = new WindowsHPC2012
            {
	            ShowProgressAtConsole = true
            };

	        var jobID = myCluster.Send(currentJob);

			if (jobID <= 0)
			{
				Console.WriteLine("Error: job was no created");
		        return;
	        }

	        if (myCluster.Scheduler == null)
	        {
				Console.WriteLine("Error: Scheduler is null");
				return;
	        }

			var hpcJob = myCluster.Scheduler.OpenJob(jobID);			

			var success = myCluster.MonitorJob(hpcJob);

			if (success)
				Console.WriteLine("Done");
			else
				Console.WriteLine("MonitorJob reported false");

			return;

        }

	    private static string SetCommandLine()
        {
            string q = "\"";
            string s = " ";
            string commandLine = q +
                                 @"\\picfs\projects\DMS\PIC_HPC\Hot\F_Testing_V_SN129_1\ApplicationFiles\GlyQ-IQ_Application\Release\IQGlyQ_Console.exe" +
                                 q + s +
                                 @"\\picfs\projects\DMS\PIC_HPC\Hot\F_Testing_V_SN129_1\RawData" + s +
                                 "V_SN129_1" + s +
                                 "raw" + s +
                                 "L_13_HighMannose_TargetsFirstAll_2.txt" + s +
                                 "GlyQIQ_Params_Velos_SN129_L10PSA.txt" + s +
                                 @"\\picfs\projects\DMS\PIC_HPC\Hot\F_Testing_V_SN129_1\WorkingParameters" + s +
                                 "Lock_2" + s +
                                 @"\\picfs\projects\DMS\PIC_HPC\Hot\F_Testing_V_SN129_1\Results\Results" + s +
                                 "2";
            return commandLine;
        }

        private static string SetCommandLine2(string workDirectory, string workDirectoryIP, string datafileName, string targetsFile, string parameterFile)
        {
            //string cmdNewLine = @"&";
            const string q = "\"";
            //string star = @"\*";

            bool isKronies = false;
            bool isAzure = false;



            string fullCommandLine = "";

            List<string> commandLines = new List<string>();

            if (isKronies)//(2/3)
            {
                //commandLines.Add(q + @"\\picfs\projects\DMS\PIC_HPC\FieldOffice\ApplicationFiles\GlyQ-IQ_Application\Release\IQGlyQ_Console.exe" + q);
                commandLines.Add(q + workDirectory + @"\" + "ApplicationFiles" + @"\" + @"GlyQ-IQ_Application\Release\IQGlyQ_Console.exe" + q);
            }
            else
            {
                //commandLines.Add(q + @"\\172.16.112.12\projects\DMS\ScottK\ScottK_PUB-100X_Launch_Folder\ToPIC\GlyQ-IQ_Application\Release\IQGlyQ_Console.exe" + q);
                //commandLines.Add(q + @"\\172.16.112.12\projects\DMS\PIC_HPC\FieldOffice\ApplicationFiles\GlyQ-IQ_Application\Release\IQGlyQ_Console.exe" + q);

                if (isAzure)
                {
                    //workDirectoryIP = @"\%CCP_PACKAGE_ROOT\%\FieldOffice";

                    //commandLines.Add(q +  @"%CCP_PACKAGE_ROOT%\FieldOffice" + @"\" + "ApplicationFiles" + @"\" + @"GlyQ-IQ_Application\Release\IQGlyQ_Console.exe" + q);

                    //commandLines.Add(q + workDirectoryIP + @"\" + "ApplicationFiles" + @"\" + @"GlyQ-IQ_Application\Release\IQGlyQ_Console.exe" + q);
                    commandLines.Add(workDirectoryIP + @"\" + "ApplicationFiles" + @"\" + @"GlyQ-IQ_Application\Release\IQGlyQ_Console.exe");

                }
                else
                {
                    commandLines.Add(q + workDirectoryIP + @"\" + "ApplicationFiles" + @"\" + @"GlyQ-IQ_Application\Release\IQGlyQ_Console.exe" + q);
                }
            }

            commandLines.Add(workDirectoryIP + @"\" + "RawData");
            commandLines.Add(q + datafileName + q);
            commandLines.Add(q + @"raw" + q);
            commandLines.Add(q + targetsFile + @"_*.txt" + q);
            commandLines.Add(q + parameterFile + q);
            commandLines.Add(workDirectoryIP + @"\" + "WorkingParameters");
            commandLines.Add(@"Lock_*");
            commandLines.Add(workDirectoryIP + @"\" + @"Results\Results");
            commandLines.Add(@"*");

			fullCommandLine = string.Join(" ", commandLines);

            return fullCommandLine;
        }

        private static string SetName(int cores, string datafileName)
        {
            string jobNameMustBeShort = cores + " Cores and x nodes " + datafileName;

            List<char> letters = jobNameMustBeShort.ToList();
            if (jobNameMustBeShort.Length > 80)
            {
                jobNameMustBeShort = "";
                for (int i = 0; i < 80; i++)
                {
                    jobNameMustBeShort += letters[i];
                }
            }


            return jobNameMustBeShort;
        }
    }
}
