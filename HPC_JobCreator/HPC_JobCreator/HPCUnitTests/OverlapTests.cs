using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HPC_Connector;
using HPC_Submit;
using NUnit.Framework;

namespace HPCUnitTests
{
    public class OverlapTests
    {
        [Test]
        public void singleJob()
        {
            int iteration = 4;
            for (int i = 0; i < iteration; i++)
            {
                string clusterName = "deception2.pnnl.gov";
                HPC_Connector.ParametersCluster clusterParams = new ParametersCluster(clusterName);

                string nameOfJob = "singleJob";
                HPC_Connector.ParametersJob jobParams = new ParametersJob(nameOfJob);
                jobParams.MaxNumberOfCores = 1;
                jobParams.MinNumberOfCores = 1;
                jobParams.PriorityLevel = PriorityLevel.BelowNormal;
                jobParams.ProjectName = "GlyQIQ";
                jobParams.TargetHardwareUnitType = HardwareUnitType.Socket;
                jobParams.TemplateName = "PrePost";
                //jobParams.TemplateName = "GlyQIQ";
                jobParams.isExclusive = false;

                string nameTask = "ping";
                HPC_Connector.ParametersTask testTask = new ParametersTask(nameTask);
                testTask.CommandLine = @"ping localhost -n 20 >> %computername%_%CCP_JOBID%.txt";
                testTask.TaskTypeOption = HPCTaskType.ParametricSweep;
                testTask.ParametricIncrement = 1;
                testTask.ParametricStartIndex = 1;
                testTask.ParametricStopIndex = 1;
                testTask.WorkDirectory = @"\\picfs.pnl.gov\projects\DMS\PIC_HPC\ClayStuff";
                testTask.StdOutFilePath = @"\\picfs.pnl.gov\projects\DMS\PIC_HPC\ClayStuff\pingOut_%CCP_JOBID%.txt";

                JobToHPC sendMe = new JobToHPC(clusterName, nameOfJob, nameTask);
                sendMe.ClusterParameters = clusterParams;
                sendMe.JobParameters = jobParams;
                sendMe.TaskParameters = testTask;
                HPC_Submit.WindowsHPC2012 toScheduler = new WindowsHPC2012();
                toScheduler.Send(sendMe);

                
            }
        }

        public void singleJobWithPrepAndReleaseTask()
        {
            int iteration = 4;
            for (int i = 0; i < iteration; i++)
            {
                string clusterName = "deception2.pnnl.gov";
                HPC_Connector.ParametersCluster clusterParams = new ParametersCluster(clusterName);

                string nameOfJob = "singleJob";
                HPC_Connector.ParametersJob jobParams = new ParametersJob(nameOfJob);
                jobParams.MaxNumberOfCores = 1;
                jobParams.MinNumberOfCores = 1;
                jobParams.PriorityLevel = PriorityLevel.BelowNormal;
                jobParams.ProjectName = "GlyQIQ";
                jobParams.TargetHardwareUnitType = HardwareUnitType.Socket;
                jobParams.TemplateName = "PrePost";
                //jobParams.TemplateName = "GlyQIQ";
                jobParams.isExclusive = true;

                string nameTask = "ping";
                HPC_Connector.ParametersTask testTask = new ParametersTask(nameTask);
                testTask.CommandLine = @"ping localhost -n 20 >> %computername%.txt";
                testTask.TaskTypeOption = HPCTaskType.ParametricSweep;
                testTask.ParametricIncrement = 1;
                testTask.ParametricStartIndex = 1;
                testTask.ParametricStopIndex = 1;
                testTask.WorkDirectory = @"\\picfs.pnl.gov\projects\DMS\PIC_HPC\ClayStuff";
                testTask.StdOutFilePath = @"\\picfs.pnl.gov\projects\DMS\PIC_HPC\ClayStuff\pingOut.txt";


                HPC_Connector.ParametersTask prepTask = new ParametersTask("prep");
                prepTask.CommandLine = @"ping localhost -n 20 >> %computername%Prep.txt";
                prepTask.WorkDirectory = @"\\picfs.pnl.gov\projects\DMS\PIC_HPC\ClayStuff";
                prepTask.StdOutFilePath = @"\\picfs.pnl.gov\projects\DMS\PIC_HPC\ClayStuff\pingOutPrep.txt";

                HPC_Connector.ParametersTask releaseTask = new ParametersTask("release");
                releaseTask.CommandLine = @"ping localhost -n 1 >> %computername%Release.txt";
                releaseTask.WorkDirectory = @"\\picfs.pnl.gov\projects\DMS\PIC_HPC\ClayStuff";
                releaseTask.StdOutFilePath = @"\\picfs.pnl.gov\projects\DMS\PIC_HPC\ClayStuff\pingOutRelease.txt";

                JobToHPC sendMe = new JobToHPC(clusterName, nameOfJob, nameTask);
                sendMe.ClusterParameters = clusterParams;
                sendMe.JobParameters = jobParams;
                sendMe.TaskParameters = testTask;
                sendMe.PrepNodeTaskParameters = prepTask;
                sendMe.ReleaseNodeTaskParameters = releaseTask;
                HPC_Submit.WindowsHPC2012 toScheduler = new WindowsHPC2012();
                toScheduler.Send(sendMe);


            }
        }

        public void singleJobNoWorkDir()
        {
            int iteration = 6;
            for (int i = 0; i < iteration; i++)
            {
                string clusterName = "deception2.pnnl.gov";
                HPC_Connector.ParametersCluster clusterParams = new ParametersCluster(clusterName);

                string nameOfJob = "singleJob";
                HPC_Connector.ParametersJob jobParams = new ParametersJob(nameOfJob);
                jobParams.MaxNumberOfCores = 1;
                jobParams.MinNumberOfCores = 1;
                jobParams.PriorityLevel = PriorityLevel.BelowNormal;
                jobParams.ProjectName = "GlyQIQ";
                jobParams.TargetHardwareUnitType = HardwareUnitType.Socket;
                jobParams.TemplateName = "PrePost";
                //jobParams.TemplateName = "GlyQIQ";
                jobParams.isExclusive = false;

                string nameTask = "ping";
                HPC_Connector.ParametersTask testTask = new ParametersTask(nameTask);
                testTask.CommandLine = @"ping localhost -n 20";
                testTask.TaskTypeOption = HPCTaskType.ParametricSweep;
                testTask.ParametricIncrement = 1;
                testTask.ParametricStartIndex = 1;
                testTask.ParametricStopIndex = 1;
                //testTask.WorkDirectory = @"\\picfs.pnl.gov\projects\DMS\PIC_HPC\ClayStuff";
                //testTask.StdOutFilePath = @"\\picfs.pnl.gov\projects\DMS\PIC_HPC\ClayStuff\pingOut.txt";
                testTask.WorkDirectory = "";
                testTask.StdOutFilePath = "";

                JobToHPC sendMe = new JobToHPC(clusterName, nameOfJob, nameTask);
                sendMe.ClusterParameters = clusterParams;
                sendMe.JobParameters = jobParams;
                sendMe.TaskParameters = testTask;
                HPC_Submit.WindowsHPC2012 toScheduler = new WindowsHPC2012();
                toScheduler.Send(sendMe);


            }
        }

    }
}
