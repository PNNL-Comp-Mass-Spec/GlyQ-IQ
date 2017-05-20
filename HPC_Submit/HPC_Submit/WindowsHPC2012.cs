using System;
using System.Collections.Generic;
using System.Threading;
using HPC_Connector;
using Microsoft.Hpc.Scheduler;
using Microsoft.Hpc.Scheduler.Properties;

namespace HPC_Submit
{
	public class WindowsHPC2012
	{
		#region "Class-wide Variables"

		private ISchedulerJob mJob;
		private ISchedulerTask mTask;

		private readonly ManualResetEvent mJobFinishedEvent = new ManualResetEvent(false);

		private IScheduler mScheduler;

		private string mErrorMessage = string.Empty;
		private bool mAbortJob;

		private readonly string mHPCUserName;
		private readonly string mHPCUserPassword;

		#endregion

		#region "Properties"
		public string ErrorMessage
		{
			get
			{
				if (string.IsNullOrEmpty(mErrorMessage))
					return string.Empty;

				return mErrorMessage;
			}
		}

		public bool ShowProgressAtConsole { get; set; }

		/// <summary>
		/// Allows for programatically accessing the job scheduler
		/// </summary>
		/// <remarks>Will be Nothing until after Send() is called</remarks>
		public IScheduler Scheduler
		{
			get
			{
				return mScheduler;
			}
		}

		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		public WindowsHPC2012()
		{
			mHPCUserName = string.Empty;
			mHPCUserPassword = string.Empty;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public WindowsHPC2012(string hpcUserName, string hpcUserPassword)
		{
			mHPCUserName = hpcUserName;
			mHPCUserPassword = hpcUserPassword;
		}

		public void AbortNow()
		{
			mAbortJob = true;
		}

		private ISchedulerTask CreateTask(ISchedulerJob job, ParametersTask taskParameters)
		{

			var newTask = job.CreateTask();

			// Note that Task names cannot contain commas
			newTask.Name = taskParameters.TaskName.Replace(',', '_');

			Console.WriteLine("Creating a task {0}", newTask.Name);

			newTask.CommandLine = taskParameters.CommandLine;
			newTask.WorkDirectory = taskParameters.WorkDirectory;
			newTask.StdOutFilePath = taskParameters.StdOutFilePath;
			newTask.FailJobOnFailure = taskParameters.FailJobOnFailure;

			newTask.Type = SetTaskType(taskParameters.TaskTypeOption);

			if (newTask.Type == TaskType.ParametricSweep)
			{
				newTask.StartValue = taskParameters.ParametricStartIndex;
				newTask.EndValue = taskParameters.ParametricStopIndex;
				newTask.IncrementValue = taskParameters.ParametricIncrement;
			}

			return newTask;
		}

		public bool MonitorCurrentJob()
		{
			return MonitorJob(mJob);
		}


		public bool MonitorJob(ISchedulerJob hpcJob)
		{
			const int DEFAULT_TIMEOUT_HOURS = 72;
			return MonitorJob(hpcJob, DEFAULT_TIMEOUT_HOURS);
		}

		public bool MonitorJob(ISchedulerJob hpcJob, int monitorTimeoutHours)
		{
			var dtStartTime = DateTime.UtcNow;

			mErrorMessage = string.Empty;

			Console.WriteLine("Monitoring job " + hpcJob.Id);

			ReportProgress("Monitoring job " + hpcJob.Id, 0, 0);

			while (true)
			{
				hpcJob.Refresh();

				var elapsedHours = DateTime.UtcNow.Subtract(dtStartTime).TotalHours;

				var jobState = hpcJob.State;
				switch (jobState)
				{
					case JobState.Failed:
						ReportError("Job failed");
						return false;
					case JobState.Canceled:
						ReportError("Job cancelled");
						return false;
					case JobState.Finished:
						ReportMessage("Job finished");
						return true;
					default:
						ReportProgress("Processing", hpcJob.Progress, elapsedHours);
						break;
				}

				if (elapsedHours > monitorTimeoutHours)
				{
					ReportError("Over " + monitorTimeoutHours + " hours have elapsed; aborting monitoring of the job");
					return false;
				}

				if (mAbortJob)
				{
					ReportError("Job abort requested (" + elapsedHours.ToString("0.0") + " hours have elapsed); cancelling task");

					try
					{
						mJob.Finish();
					}
					// ReSharper disable once EmptyGeneralCatchClause
					catch
					{
						// Ignore errors calling Job finish
					}

					try
					{
						
					}
					// ReSharper disable once EmptyGeneralCatchClause
					catch
					{
						// Ignore errors cancelling the task
					}

					return false;
				}

				Thread.Sleep(2000);

			}

		}

		/// <summary>
		/// Starts a job
		/// </summary>
		/// <param name="currentJob"></param>
		/// <returns>Job ID if success; 0 if an error</returns>
		public int Send(JobToHPC currentJob)
		{
			mErrorMessage = string.Empty;
			mAbortJob = false;

			string clusterName = currentJob.ClusterParameters.ClusterName;

			// Create a scheduler object to be used to 
			// establish a connection to the scheduler on the headnode
			mScheduler = new Scheduler();


			// Connect to the scheduler
			Console.WriteLine("Connecting to {0}...", clusterName);
			try
			{

				mScheduler.Connect(clusterName);

				if (!string.IsNullOrEmpty(mHPCUserName) && !string.IsNullOrEmpty(mHPCUserPassword))
					mScheduler.SetCachedCredentials(mHPCUserName, mHPCUserPassword);
			}
			catch (Exception e)
			{
				mErrorMessage = "Could not connect to the scheduler: " + e.Message;
				Console.Error.WriteLine(mErrorMessage);
				return 0;
			}


			//Create a job to submit to the scheduler
			//the job will be equivalent to the CLI command: job submit /numcores:1-1 "echo hello world"
			mJob = mScheduler.CreateJob();

			//Some of the optional job parameters to specify. If omitted, defaults are:
			// Name = {blank}
			// UnitType = Core
			// Min/Max Resources = Autocalculated
			// etc...

			// Example templates are "Default" and "DMS"
			if (!string.IsNullOrEmpty(currentJob.JobParameters.TemplateName))
				mJob.SetJobTemplate(currentJob.JobParameters.TemplateName);

			mJob.Name = currentJob.JobParameters.JobName;
			Console.WriteLine("Creating job name {0}...", mJob.Name);

			mJob.Project = currentJob.JobParameters.ProjectName;
			mJob.Priority = SetLevel(currentJob.JobParameters.PriorityLevel);
			mJob.OrderBy = "Cores";
			mJob.AutoCalculateMin = true;
			mJob.AutoCalculateMax = true;

			mJob.IsExclusive = currentJob.JobParameters.isExclusive;

			// Define the maximum runtime for the job (convert from hours to seconds)
			if (currentJob.JobParameters.MaxRunTimeHours > 0)
				mJob.Runtime = currentJob.JobParameters.MaxRunTimeHours * 3600;

			mJob.UnitType = SetUnitType(currentJob.JobParameters.TargetHardwareUnitType);

			if (mJob.UnitType == JobUnitType.Core && (currentJob.JobParameters.MinNumberOfCores > 0 || currentJob.JobParameters.MaxNumberOfCores > 0))
			{
				// Note that we only define the minimum and maximum number of cores if the unit type is Cores

				if (currentJob.JobParameters.MinNumberOfCores < 1)
					currentJob.JobParameters.MinNumberOfCores = 1;
				if (currentJob.JobParameters.MaxNumberOfCores < currentJob.JobParameters.MinNumberOfCores)
					currentJob.JobParameters.MaxNumberOfCores = currentJob.JobParameters.MinNumberOfCores;

				mJob.MinimumNumberOfCores = currentJob.JobParameters.MinNumberOfCores;
				mJob.MaximumNumberOfCores = currentJob.JobParameters.MaxNumberOfCores;
			}

			if (mJob.UnitType == JobUnitType.Socket && (currentJob.JobParameters.MinNumberOfCores > 0 || currentJob.JobParameters.MaxNumberOfCores > 0))
			{
				// Note that we only define the minimum and maximum number of cores if the unit type is Cores

				if (currentJob.JobParameters.MinNumberOfCores < 1)
					currentJob.JobParameters.MinNumberOfCores = 1;
				if (currentJob.JobParameters.MaxNumberOfCores < currentJob.JobParameters.MinNumberOfCores)
					currentJob.JobParameters.MaxNumberOfCores = currentJob.JobParameters.MinNumberOfCores;

				mJob.MinimumNumberOfSockets = currentJob.JobParameters.MinNumberOfCores;
				mJob.MaximumNumberOfSockets = currentJob.JobParameters.MaxNumberOfCores;
			}

			if (mJob.UnitType == JobUnitType.Node && (currentJob.JobParameters.MinNumberOfCores > 0 || currentJob.JobParameters.MaxNumberOfCores > 0))
			{
				// Note that we only define the minimum and maximum number of cores if the unit type is Cores

				if (currentJob.JobParameters.MinNumberOfCores < 1)
					currentJob.JobParameters.MinNumberOfCores = 1;
				if (currentJob.JobParameters.MaxNumberOfCores < currentJob.JobParameters.MinNumberOfCores)
					currentJob.JobParameters.MaxNumberOfCores = currentJob.JobParameters.MinNumberOfCores;

				mJob.MinimumNumberOfNodes = currentJob.JobParameters.MinNumberOfCores;
				mJob.MaximumNumberOfNodes = currentJob.JobParameters.MaxNumberOfCores;
			}

			//mJob.NodeGroups.Add(currentJob.ClusterParameters.WorkerNodeGroup);


			//Create a task to submit to the job
			mTask = CreateTask(mJob, currentJob.TaskParameters);

			//Don't forget to add the task to the job!
			mJob.AddTask(mTask);

			if (currentJob.PrepNodeTaskParameters.CommandLine != "skip")
			{
				currentJob.PrepNodeTaskParameters.TaskTypeOption = HPCTaskType.NodePrep;
				ISchedulerTask mPrepTask = CreateTask(mJob, currentJob.PrepNodeTaskParameters);

				mJob.AddTask(mPrepTask);
			}

			if (currentJob.ReleaseNodeTaskParameters.CommandLine != "skip")
			{
				currentJob.ReleaseNodeTaskParameters.TaskTypeOption = HPCTaskType.NodeRelease;
				ISchedulerTask mReleaseTask = CreateTask(mJob, currentJob.ReleaseNodeTaskParameters);

				mJob.AddTask(mReleaseTask);
			}

			// Add any subsequent tasks
			// Assume that each task depends on the completion of the previous tasks
			var previousTaskName = string.Copy(mTask.Name);

			foreach (var subsequentTaskParams in currentJob.SubsequentTaskParameters)
			{
				var subsequentTask = CreateTask(mJob, subsequentTaskParams);
				subsequentTask.DependsOn.Add(previousTaskName);

				mJob.AddTask(subsequentTask);

				previousTaskName = string.Copy(subsequentTask.Name);
			}

			// Can use the OnJobState callback to check if a job is finished
			mJob.OnJobState += new EventHandler<JobStateEventArg>(job_OnJobState);

			//And to submit the job.
			//You can specify your username and password in the parameters, or set them to null and you will be prompted for your credentials
			Console.WriteLine("Submitting job to the cluster...");
			Console.WriteLine();

			mScheduler.SubmitJob(mJob, null, null);

			// Could use this to wait for the job to finish
			//jobFinishedEvent.WaitOne();

			// Note: leave the scheduler open so that the calling task can monitor the job's progress

			return mJob.Id;

		}

		private int GetCoreCount()
		{
			IIntCollection nodId = mScheduler.GetNodeIdList(null, null);

			int firstSampleNode = nodId[0];

			ISchedulerNode openNode = mScheduler.OpenNode(firstSampleNode);

			int cores = openNode.NumberOfCores;

			//ISchedulerCollection returnedCores = openNode.GetCores();
			//int maxCores = returnedCores.Count;

			return cores;
		}

		void job_OnJobState(object sender, JobStateEventArg e)
		{
			if (e.NewState == JobState.Finished) //the job is finished
			{
				mTask.Refresh(); // update the task object with updates from the scheduler

				if (!string.IsNullOrEmpty(mTask.Output))
					Console.WriteLine("Output: " + mTask.Output); //print the task's output

				mJobFinishedEvent.Set();
			}
			else if (e.NewState == JobState.Canceled || e.NewState == JobState.Failed)
			{
				Console.WriteLine("Job did not finish.");
				mJobFinishedEvent.Set();
			}
			else if (e.NewState == JobState.Queued && e.PreviousState != JobState.Validating)
			{
				Console.WriteLine("The job is currently queued.");
				Console.WriteLine("Waiting for job to start...");
			}
		}

		private JobPriority SetLevel(PriorityLevel currentLevelIn)
		{
			JobPriority level = new JobPriority();
			switch (currentLevelIn)
			{
				case PriorityLevel.Lowest:
					{
						level = JobPriority.Lowest;
					}
					break;
				case PriorityLevel.BelowNormal:
					{
						level = JobPriority.BelowNormal;
					}
					break;
				case PriorityLevel.Normal:
					{
						level = JobPriority.Normal;
					}
					break;
				case PriorityLevel.AboveNormal:
					{
						level = JobPriority.AboveNormal;
					}
					break;
				case PriorityLevel.Highest:
					{
						level = JobPriority.Highest;
					}
					break;
				default:
					{
						level = JobPriority.Normal;
					}
					break;
			}
			return level;
		}

		private JobUnitType SetUnitType(HardwareUnitType currentLevelIn)
		{
			JobUnitType unitType = new JobUnitType();
			switch (currentLevelIn)
			{
				case HardwareUnitType.Core:
					{
						unitType = JobUnitType.Core;
					}
					break;
				case HardwareUnitType.Socket:
					{
						unitType = JobUnitType.Socket;
					}
					break;
				case HardwareUnitType.Node:
					{
						unitType = JobUnitType.Node;
					}
					break;
				default:
					{
						unitType = JobUnitType.Core;
					}
					break;
			}
			return unitType;
		}

		private TaskType SetTaskType(HPCTaskType typeIn)
		{
			TaskType taskType = new TaskType();
			switch (typeIn)
			{
				case HPCTaskType.Basic:
					{
						taskType = TaskType.Basic;
					}
					break;
				case HPCTaskType.ParametricSweep:
					{
						taskType = TaskType.ParametricSweep;
					}
					break;
				case HPCTaskType.NodePrep:
					{
						taskType = TaskType.NodePrep;
					}
					break;
				case HPCTaskType.NodeRelease:
					{
						taskType = TaskType.NodeRelease; ;
					}
					break;
				case HPCTaskType.Service:
					{
						taskType = TaskType.Service;
					}
					break;
				default:
					{
					}
					break;
			}

			return taskType;
		}

		#region "Events"

		public event MessageEventHandler ErrorEvent;
		public event MessageEventHandler MessageEvent;
		public event ProgressEventHandler ProgressEvent;

		/// <summary>
		/// Report an error.
		/// </summary>
		/// <param name="errorMessage"></param>
		protected void ReportError(string errorMessage)
		{
			mErrorMessage = errorMessage;

			OnErrorMessage(new MessageEventArgs(errorMessage));

			Console.WriteLine(errorMessage);

		}

		protected void ReportMessage(string message)
		{

			if (!string.IsNullOrEmpty(message))
				OnMessage(new MessageEventArgs(message));

			Console.WriteLine(message);

		}

		protected void ReportProgress(string currentTask, double percentComplete, double hoursElapsed)
		{

			OnProgressUpdate(new ProgressEventArgs(currentTask, percentComplete, hoursElapsed));

			if (ShowProgressAtConsole)
				Console.WriteLine("  " + percentComplete + ", " + hoursElapsed.ToString("0.00") + " hours elapsed");
		}


		#endregion

		#region "Event Handlers"

		public void OnErrorMessage(MessageEventArgs e)
		{
			if (ErrorEvent != null)
				ErrorEvent(this, e);
		}

		public void OnMessage(MessageEventArgs e)
		{
			if (MessageEvent != null)
				MessageEvent(this, e);
		}

		public void OnProgressUpdate(ProgressEventArgs e)
		{
			if (ProgressEvent != null)
				ProgressEvent(this, e);
		}
		#endregion

	}
}
