using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Hpc.Scheduler;
using Microsoft.Hpc.Scheduler.Properties;

namespace HPC_JobCreator
{
    public class SendTask
    {
        static ISchedulerJob job;
        static ISchedulerTask task;
        static ISchedulerTask prepTask;
        static ManualResetEvent jobFinishedEvent = new ManualResetEvent(false);

        public void Send(int cores, string datafileName, string targetsFile, string workDirectory, string parameterFile, string workDirectoryIP, int taskNumber, string exeHomeLocationDirectory, string workerNodeGroup)
        //public void Send(int cores, string datafileName, string targetsFile, int taskNumber)
        {
            var password = GetValue();
            
            //string clusterName = "localhost";
            //string clusterName = "Deception.pnl.gov";
            string clusterName = "Deception2.pnl.gov";

            // Create a scheduler object to be used to 
            // establish a connection to the scheduler on the headnode
            using (IScheduler scheduler = new Scheduler())
            {
                // Connect to the scheduler
                Console.WriteLine("Connecting to {0}...", clusterName);
                try
                {
                   
                    scheduler.Connect(clusterName);
                    //scheduler.SetCachedCredentials(@"winhpc\scott", @"PicWinHpc!");
                    //scheduler.SetCachedCredentials(@"pnl\kron626", password);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Could not connect to the scheduler: {0}", e.Message);
                    return; //abort if no connection could be made
                }


                //Create a job to submit to the scheduler
                //the job will be equivalent to the CLI command: job submit /numcores:1-1 "echo hello world"
                job = scheduler.CreateJob();

                
                //Some of the optional job parameters to specify. If omitted, defaults are:
                // Name = {blank}
                // UnitType = Core
                // Min/Max Resources = Autocalculated
                // etc...
                job.SetJobTemplate("Glyqiq");

                job.Name = cores + "Core x nodes Task";
                Console.WriteLine("Creating job name {0}...", job.Name);
                
                job.Project = "GlyQIQ";//Extended or GlyQ-IQ
                job.Priority = JobPriority.AboveNormal;
                job.OrderBy = "Cores";
                job.AutoCalculateMin = true;
                job.AutoCalculateMax = true;

                job.UnitType = JobUnitType.Core;


                job.MinimumNumberOfCores = 1;
                job.MaximumNumberOfCores = cores;
                //job.MinimumNumberOfSockets = 2;
                //job.MaximumNumberOfSockets = 16;
                //job.MinimumNumberOfNodes = 2;
                //job.MaximumNumberOfNodes = 16;

                bool isAzure = false;
                bool isKronies = false;
                bool isIPAddress = false;
                bool addPrepandFinalize = false;
                bool frankenDelete = false;
                switch (workerNodeGroup)
                {
                    case "Kronies":
                        {
                            job.NodeGroups.Add(workerNodeGroup);
                            isKronies = true;//everything needs to be picfs
                            addPrepandFinalize = false;
                        }
                        break;
                    case "ComputeNodes":
                        {
                            job.NodeGroups.Add(workerNodeGroup);
                            isIPAddress = true;
                        }
                        break;
                    case "AzureNodes":
                        {
                            job.NodeGroups.Add(workerNodeGroup);
                            isAzure = true;
                            addPrepandFinalize = true;

                            //workDirectoryIP = @"%CCP_PACKAGE_ROOT%\FieldOffice";
                            //logDirectoryIP = @"%CCP_PACKAGE_ROOT%\FieldOffice";
                        }
                        break;
                    case @"@PNNL":
                        {
                            job.NodeGroups.Add(workerNodeGroup);
                            addPrepandFinalize = true;
                        }
                        break;
                    default:
                        {
                            Console.WriteLine("Wrong worker group Node name.  " + workerNodeGroup + " does not exist.");
                        }
                        break;
                }

                //job.AllocatedNodes.Remove("DECEPTION-031");does not work

                string cmdNewLine = @"&";
                string q = "\"";
                string star = @"\*";

                #region old
                //#region prep task
                //Console.WriteLine("PrepTask... Copy and Install Thermo Dlls");

                //ISchedulerTask prepTask = job.CreateTask();
                //prepTask.Name = "RegisterThermoDLL";
                //prepTask.Type = TaskType.NodePrep;
                //prepTask.WorkDirectory = workDirectoryIP;
                //prepTask.StdOutFilePath = workDirectoryIP + @"\" + @"Results\DLL" + datafileName + "_" + taskNumber + @".log";
                ////prepTask.CommandLine = @"\\picfs\projects\DMS\ScottK\ScottK_PUB-100X_Launch_Folder\ToPIC\GlyQ-IQ_DLL\Release\RegisterDLL.exe \\picfs\projects\DMS\PIC_HPC\ThermoDLL\InstallFilesToCDrive\XcalDLL\MSFileReader\MSFileReader.XRawfile2.dll add";

                ////prepTask.CommandLine = 
                ////    @"\\picfs\projects\DMS\PIC_HPC\FieldOffice\RemoteThermo\MassSpectrometerDLLs\RemoteThermo\FieldOfficeCopyToC.bat" + cmdNewLine  + " " + 
                ////    @"C:\MassSpectrometerDLLs\RemoteThermo\0_CallEverything.bat";

                ////check field office path in FieldOfficeCopyToC
                ////prepTask.CommandLine =
                ////   workDirectoryIP + @"\" + @"RemoteThermo\MassSpectrometerDLLs\RemoteThermo\FieldOfficeCopyToC.bat" + cmdNewLine + " " +
                ////   @"C:\MassSpectrometerDLLs\RemoteThermo\0_CallEverything.bat";

                ////home
                // prepTask.CommandLine =
                //   workDirectoryIP + @"\" + @"RemoteThermo\MassSpectrometerDLLs\RemoteThermo\FieldOfficeCopyToC_Home.bat" + cmdNewLine + " " +
                //   @"C:\MassSpectrometerDLLs\RemoteThermo\0_CallEverything.bat";

                
                ////prepTask.CommandLine = @"\\picfs\projects\DMS\PIC_HPC\FieldOffice\ApplicationFiles\GlyQ-IQ_ThermoDLL\Release\RegisterDLL.exe \\picfs\projects\DMS\PIC_HPC\FieldOffice\ApplicationFiles\GlyQ-IQ_DLL\Release\MSFileReader.XRawfile2.dll add";
                ////prepTask.CommandLine = @"\\picfs\projects\DMS\ScottK\ScottK_PUB-100X_Launch_Folder\ToPIC\GlyQ-IQ_ThermoDLL\Release\RegisterDLL.exe \\picfs\projects\DMS\ScottK\ScottK_PUB-100X_Launch_Folder\ToPIC\GlyQ-IQ_ThermoDLL\Release\MSFileReader.XRawfile2.dll add";

                //if (isAzure)
                //{
                //    prepTask.CommandLine = @"%CCP_PACKAGE_ROOT%\FieldOffice\ApplicationFiles\GlyQ-IQ_DLL\Release\RegisterDLL.exe %CCP_PACKAGE_ROOT%\FieldOffice\ApplicationFiles\GlyQ-IQ_DLL\Release\MSFileReader.XRawfile2.dll add";
                //}

                //job.AddTask(prepTask);

                //#endregion

                //#region Finalize task
                //Console.WriteLine("FinalizeTask");

                //ISchedulerTask finalizeTask = job.CreateTask();
                //finalizeTask.Name = "RegisterThermoDLL";
                //finalizeTask.Type = TaskType.NodeRelease;
                //finalizeTask.WorkDirectory = workDirectoryIP;
                
                //finalizeTask.StdOutFilePath = workDirectoryIP + @"\" + @"Results\DLLfinal" + datafileName + "_" + taskNumber + @".log";
                //finalizeTask.CommandLine =
                //    @"C:\MassSpectrometerDLLs\RemoteThermo\0_CleanUp.bat" + cmdNewLine + " " +
                //    @"cd " + @"C:\MassSpectrometerDLLs\" + cmdNewLine + " " +
                //    "del " + q + star + q + " /q" + cmdNewLine + " " +
                //    "IF EXIST " + @"C:\MassSpectrometerDLLs" + " (rmdir " + @"C:\MassSpectrometerDLLs" + @" /s /q )";

                //if (isAzure)
                //{
                //    finalizeTask.CommandLine = @"%CCP_PACKAGE_ROOT%\FieldOffice\ApplicationFiles\GlyQ-IQ_DLL\Release\RegisterDLL.exe %CCP_PACKAGE_ROOT%\FieldOffice\ApplicationFiles\GlyQ-IQ_DLL\Release\MSFileReader.XRawfile2.dll add";
                //}

                //job.AddTask(finalizeTask);

                //#endregion
                #endregion


                #region Prep DLL Task

                Console.WriteLine("PrepTask... Copy and Install Thermo Dlls");

                ISchedulerTask prepTask = job.CreateTask();
                prepTask.Name = "RegisterThermoDLL";
                prepTask.Type = TaskType.NodePrep;
                prepTask.WorkDirectory = workDirectoryIP;
                prepTask.StdOutFilePath = workDirectoryIP + @"\" + @"Results\DLL" + datafileName + "_" + taskNumber + @".log";
                prepTask.StdErrFilePath = workDirectoryIP + @"\" + @"Results\DLLError" + datafileName + "_" + taskNumber + @".log";
                //prepTask.CommandLine = @"\\picfs\projects\DMS\ScottK\ScottK_PUB-100X_Launch_Folder\ToPIC\GlyQ-IQ_DLL\Release\RegisterDLL.exe \\picfs\projects\DMS\PIC_HPC\ThermoDLL\InstallFilesToCDrive\XcalDLL\MSFileReader\MSFileReader.XRawfile2.dll add";

                //prepTask.CommandLine = @"\\picfs\projects\DMS\PIC_HPC\FieldOffice\ApplicationFiles\GlyQ-IQ_ThermoDLL\Release\RegisterDLL.exe \\picfs\projects\DMS\PIC_HPC\FieldOffice\ApplicationFiles\GlyQ-IQ_DLL\Release\MSFileReader.XRawfile2.dll add";
                //prepTask.CommandLine = @"\\picfs\projects\DMS\ScottK\ScottK_PUB-100X_Launch_Folder\ToPIC\GlyQ-IQ_ThermoDLL\Release\RegisterDLL.exe \\picfs\projects\DMS\ScottK\ScottK_PUB-100X_Launch_Folder\ToPIC\GlyQ-IQ_ThermoDLL\Release\MSFileReader.XRawfile2.dll add";


                //prepTask.CommandLine =
                //    @"\\picfs\projects\DMS\PIC_HPC\FieldOffice\RemoteThermo\MassSpectrometerDLLs\RemoteThermo\FieldOfficeCopyToC.bat" + cmdNewLine + " " +
                //    @"C:\MassSpectrometerDLLs\RemoteThermo\0_CallEverything.bat";

                prepTask.CommandLine = workDirectory + @"\" + @"RemoteThermo\MassSpectrometerDLLs\RemoteThermo\FieldOfficeCopyToC.bat" + cmdNewLine + " " + @"C:\MassSpectrometerDLLs\RemoteThermo\0_CallEverything.bat";


                if (isAzure)
                {
                    //prepTask.WorkDirectory = @"%CCP_PACKAGE_ROOT%\FieldOffice";
                    //prepTask.WorkDirectory = workDirectoryIP;
                    //prepTask.StdOutFilePath = workDirectoryIP + @"\" + @"Results\DLL" + datafileName + "_" + taskNumber + @".log";
                    //prepTask.CommandLine = workDirectoryIP + @"\" + @"ApplicationFiles\GlyQ-IQ_DLL\Release\RegisterDLL.exe" + " " + workDirectoryIP + @"\" +  @"ApplicationFiles\GlyQ-IQ_DLL\Release\MSFileReader.XRawfile2.dll add";

                    //prepTask.CommandLine = workDirectoryIP + @"\" + @"RemoteThermo\MassSpectrometerDLLs\RemoteThermo\Copy_FO_V_SN129AZ.bat" + cmdNewLine + " " + workDirectoryIP + @"\" + @"MassSpectrometerDLLs\RemoteThermo\0_CallEverything_Azure.bat";
                    prepTask.CommandLine = "";
                    prepTask.CommandLine += "pushd " + workDirectoryIP + cmdNewLine;
                    prepTask.CommandLine += @"Xcopy /e /y /s /v /i RemoteThermo\MassSpectrometerDLLs MassSpectrometerDLLs" + cmdNewLine;
                    prepTask.CommandLine += "echo 1" + cmdNewLine;
                    prepTask.CommandLine += @"Xcopy /e /y /s /v /i MassSpectrometerDLLs\RemoteThermo\GlyQ-IQ_ThermoDLL\Release\MSFileReader.XRawfile2.dll MassSpectrometerDLLs" + cmdNewLine;
                    prepTask.CommandLine += "popd" + cmdNewLine;

                    prepTask.CommandLine += "SCHTASKS.exe /Create /TN ThermoAdd /TR " + workDirectoryIP + @"\" + @"MassSpectrometerDLLs\RemoteThermo\Azure_CSharpDLL_Add.bat" + " /SC OnStart /RU SYSTEM /F /RL HIGHEST" + cmdNewLine;
                    prepTask.CommandLine += "echo 2" + cmdNewLine;
                    prepTask.CommandLine += "SCHTASKS.exe /Create /TN ThermoRemove /TR " + workDirectoryIP + @"\" + @"MassSpectrometerDLLs\RemoteThermo\Azure_CSharpDLL_Remove.bat" + " /SC OnStart /RU SYSTEM /F /RL HIGHEST" + cmdNewLine;
                    prepTask.CommandLine += "echo 3" + cmdNewLine;

                    prepTask.CommandLine += "pushd " + workDirectoryIP + cmdNewLine;
                    prepTask.CommandLine += @"MassSpectrometerDLLs\RemoteThermo\BatchDelay\Release\BatchDelay.exe 5" + cmdNewLine;
                    prepTask.CommandLine += "echo 4" + cmdNewLine;
                    prepTask.CommandLine += @"schtasks.exe /run /tn ThermoAdd" + cmdNewLine;
                    prepTask.CommandLine += "echo 5" + cmdNewLine;
                    prepTask.CommandLine += @"MassSpectrometerDLLs\RemoteThermo\BatchDelay\Release\BatchDelay.exe 5" + cmdNewLine;
                    prepTask.CommandLine += "popd";
                    //pushd %CCP_PACKAGE_ROOT%\FO_V_SN129AZ&Xcopy /e /y /s /v /i RemoteThermo\MassSpectrometerDLLs MassSpectrometerDLLs
                }



                #endregion

                #region Finalize Task
                Console.WriteLine("Finalize Task");

                ISchedulerTask finalizeTask = job.CreateTask();
                finalizeTask.Name = "RegisterThermoDLL";
                finalizeTask.Type = TaskType.NodeRelease;
                finalizeTask.WorkDirectory = workDirectoryIP;
                finalizeTask.StdOutFilePath = workDirectoryIP + @"\" + @"Results\DLLfinal" + datafileName + "_" + taskNumber + @".log";
                finalizeTask.StdErrFilePath = workDirectoryIP + @"\" + @"Results\DLLfinalError" + datafileName + "_" + taskNumber + @".log";


                if (isAzure)
                {
                    finalizeTask.WorkDirectory = workDirectoryIP;
                    finalizeTask.StdOutFilePath = workDirectoryIP + @"\" + @"Results\DLLfinal" + datafileName + "_" + taskNumber + @".log";
                    finalizeTask.CommandLine = workDirectoryIP + @"\" + @"ApplicationFiles\GlyQ-IQ_DLL\Release\RegisterDLL.exe " + workDirectoryIP + @"\" + @"ApplicationFiles\GlyQ-IQ_DLL\Release\MSFileReader.XRawfile2.dll add";
                }
                else
                {
                    finalizeTask.CommandLine =
                    @"C:\MassSpectrometerDLLs\RemoteThermo\0_CleanUp.bat" + cmdNewLine + " " +
                    @"cd " + @"C:\MassSpectrometerDLLs\" + cmdNewLine + " " +
                    "del " + q + star + q + " /q" + cmdNewLine + " " +
                    "IF EXIST " + @"C:\MassSpectrometerDLLs" + " (rmdir " + @"C:\MassSpectrometerDLLs" + @" /s /q )";
                }

                #endregion

                #region cleanup Task

                Console.WriteLine("FrankenDelete");

                ISchedulerTask cleanupTask = job.CreateTask();
                cleanupTask.Name = "FrankenDelete";
                cleanupTask.Type = TaskType.NodeRelease;
                cleanupTask.WorkDirectory = workDirectoryIP;
                cleanupTask.StdOutFilePath = workDirectoryIP + @"\" + @"Results\Delete" + datafileName + "_" + taskNumber + @".log";
                cleanupTask.StdErrFilePath = workDirectoryIP + @"\" + @"Results\DeleteError" + datafileName + "_" + taskNumber + @".log";


                if (isAzure)
                {
                    cleanupTask.WorkDirectory = workDirectoryIP;
                    cleanupTask.StdOutFilePath = workDirectoryIP + @"\" + @"Results\Delete" + datafileName + "_" + taskNumber + @".log";
                    cleanupTask.CommandLine = workDirectoryIP + @"\" + @"ApplicationFiles\GlyQ-IQ_DLL\Release\RegisterDLL.exe " + workDirectoryIP + @"\" + @"ApplicationFiles\GlyQ-IQ_DLL\Release\MSFileReader.XRawfile2.dll add";
                }
                else
                {
                    cleanupTask.CommandLine = workDirectoryIP + @"\" + "1x_FrankenDelete" + "_" + datafileName + ".bat";
                }

                #endregion

                if (addPrepandFinalize)
                {
                    job.AddTask(prepTask);
                    job.AddTask(finalizeTask);
                }

                if (frankenDelete)
                {
                    job.AddTask(cleanupTask);
                }

                //Create a task to submit to the job
                task = job.CreateTask();
                task.Name = cores + "Core_RealDataD10_NoCall Raw_star";

                Console.WriteLine("Creating a {0} task...", task.Name);


                
                //string cmdNewLine = @"&";
                string fullCommandLine = "";

                List<string> commandLines = new List<string>();

                if (isKronies)//(2/3)
                {
                    //commandLines.Add(q + @"\\picfs\projects\DMS\ScottK\ScottK_PUB-100X_Launch_Folder\ToPIC\GlyQ-IQ_Application\Release\IQGlyQ_Console.exe" + q);
                    commandLines.Add(q + workDirectory + @"\" + "ApplicationFiles" + @"\" + @"GlyQ-IQ_Application\Release\IQGlyQ_Console.exe" + q);
                
                    //if (isAzure)
                    //{
                    //    commandLines.Add(q + @"%CCP_PACKAGE_ROOT%\FieldOffice\ApplicationFiles\GlyQ-IQ_Application\Release\IQGlyQ_Console.exe" + q);
                    //}
                }
                else
                {
                    //commandLines.Add(q + @"\\172.16.112.12\projects\DMS\ScottK\ScottK_PUB-100X_Launch_Folder\ToPIC\GlyQ-IQ_Application\Release\IQGlyQ_Console.exe" + q);
                    //commandLines.Add(q + workDirectory + @"\" + "ApplicationFiles" + @"\" + @"GlyQ-IQ_Application\Release\IQGlyQ_Console.exe" + q);
                    if (isAzure)
                    {
                        //workDirectoryIP = @"\%CCP_PACKAGE_ROOT\%\FieldOffice";

                        //commandLines.Add(q +  @"%CCP_PACKAGE_ROOT%\FieldOffice" + @"\" + "ApplicationFiles" + @"\" + @"GlyQ-IQ_Application\Release\IQGlyQ_Console.exe" + q);

                        commandLines.Add(q + workDirectoryIP + @"\" + "ApplicationFiles" + @"\" + @"GlyQ-IQ_Application\Release\IQGlyQ_Console.exe" + q);
                    }
                    else
                    {
                        commandLines.Add(q + workDirectoryIP + @"\" + "ApplicationFiles" + @"\" + @"GlyQ-IQ_Application\Release\IQGlyQ_Console.exe" + q);
                    }
                }

                
                //commandLines.Add("pushd " + exeHomeLocationDirectory + @"\" + "GlyQ-IQ_Application" + @"\" + "Release" + " " + cmdNewLine + " " + "IQGlyQ_Console.exe");
                
                
                //commandLines.Add("pushd " + exeHomeLocationDirectory + @"\" + "GlyQ-IQ_Application" + @"\" + "Release" + " ");
                //commandLines.Add(cmdNewLine + " " + "IQGlyQ_Console.exe");//on next line, run code

                commandLines.Add(q + workDirectoryIP + @"\" + "RawData" + q);
                commandLines.Add(q + datafileName + q);
                commandLines.Add(q + @"raw" + q);
                commandLines.Add(q + targetsFile + @"_*.txt" + q);
                commandLines.Add(q + parameterFile + q);
                commandLines.Add(q + workDirectoryIP + @"\" + "WorkingParameters" + q);
                commandLines.Add(@"Lock_*");
                commandLines.Add(q + workDirectoryIP + @"\" + @"Results\Results" + q);
                commandLines.Add(@"*");

                //this is the popd ending
                //commandLines.Add(cmdNewLine  +" " + "echo HPC run completed...");
                //commandLines.Add(cmdNewLine + " " + "popd");
                //commandLines.Add(cmdNewLine + " " + "echo popd completed...");

                for (int i = 0; i < commandLines.Count - 1; i++)
                {
                    fullCommandLine += commandLines[i] + " ";
                }
                fullCommandLine += commandLines[commandLines.Count - 1];

                //The commandline parameter tells the scheduler what the task should do
                //CommandLine is the only mandatory parameter you must set for every task

                task.CommandLine = fullCommandLine;
                task.WorkDirectory = workDirectoryIP;
                task.StdOutFilePath = workDirectoryIP + @"\" + @"Results\test" + datafileName + "_" + "*" + @".log";
                task.IsParametric = true;
                task.StartValue = taskNumber;//Different
                task.EndValue = taskNumber;//Different
                task.IncrementValue = 1;


                //Don't forget to add the task to the job!
                job.AddTask(task);


                //prep task
                //prepTask = job.CreateTask();
                //prepTask.Name = "Copy ElementsData XML for PNNL Omics to C MassSpectromterDLL";
                //prepTask.Type = TaskType.NodePrep;
                //prepTask.StdOutFilePath = @"\\172.16.112.12\projects\DMS\PIC_HPC\Home\Results\ElementsCopy_" + "*" + @".log";
                //prepTask.CommandLine = @"xcopy " + q + @"\\172.16.112.12\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ Application\Release\PNNLOmicsElementData.xml" + q + " " + @"C:\MassSpectrometerDLLs /S /Y /Z";
                //job.AddTask(prepTask);



                //Use callback to check if a job is finished
                job.OnJobState += new EventHandler<JobStateEventArg>(job_OnJobState);

                //And to submit the job.
                //You can specify your username and password in the parameters, or set them to null and you will be prompted for your credentials
                Console.WriteLine("Submitting job to the cluster...");
                Console.WriteLine();

                scheduler.SubmitJob(job, null, null);

                //wait for job to finish.  wihtout this the console will close and the job will run remotely
                //jobFinishedEvent.WaitOne();

                //Close the connection
                scheduler.Close();
            } //Call scheduler.Dispose() to free the object when finished
        }

        private static string GetValue()
        {
            string password = "Hi";
            return password;
        }

        static void job_OnJobState(object sender, JobStateEventArg e)
        {
            if (e.NewState == JobState.Finished) //the job is finished
            {
                task.Refresh(); // update the task object with updates from the scheduler

                Console.WriteLine("Job completed.");
                Console.WriteLine("Output: " + task.Output); //print the task's output
                jobFinishedEvent.Set();
            }
            else if (e.NewState == JobState.Canceled || e.NewState == JobState.Failed)
            {
                Console.WriteLine("Job did not finish.");
                jobFinishedEvent.Set();
            }
            else if (e.NewState == JobState.Queued && e.PreviousState != JobState.Validating)
            {
                Console.WriteLine("The job is currently queued.");
                Console.WriteLine("Waiting for job to start...");
            }
        }
    }
}
