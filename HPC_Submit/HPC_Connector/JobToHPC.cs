
using System.Collections.Generic;

namespace HPC_Connector
{
    public class JobToHPC
    {
        /// <summary>
        /// Defines Cluster and Scheduler Parameters
        /// </summary>
        public ParametersCluster ClusterParameters { get; set; }

        /// <summary>
        /// Parameters related to Job
        /// </summary>
        public ParametersJob JobParameters { get; set; }
        
        /// <summary>
        /// Parameters related to the task in the Job
        /// </summary>
        public ParametersTask TaskParameters { get; set; }

        /// <summary>
        /// Parameters related to the prep task in the Job
        /// </summary>
        public ParametersTask PrepNodeTaskParameters { get; set; }

        /// <summary>
        /// Parameters related to the prep task in the Job
        /// </summary>
        public ParametersTask ReleaseNodeTaskParameters { get; set; }

        /// <summary>
        /// Additional tasks in the job
        /// </summary>
		public List<ParametersTask> SubsequentTaskParameters { get; set; }
                
        /// <summary>
        /// This object contains all the information needed to submit a job
        /// </summary>
        /// <param name="clusterName">Scheduler Name</param>
        /// <param name="jobName">Name of Job to be displayed</param>
        /// <param name="taskName">Name of the Task to be displayed</param>
        public JobToHPC(string clusterName, string jobName, string taskName)
        {
            ClusterParameters = new ParametersCluster(clusterName);
            JobParameters = new ParametersJob(jobName);
            TaskParameters = new ParametersTask(taskName);
			SubsequentTaskParameters = new List<ParametersTask>();

            PrepNodeTaskParameters = new ParametersTask("PrepNodeTask");
            PrepNodeTaskParameters.CommandLine = "skip";
            PrepNodeTaskParameters.TaskTypeOption = HPCTaskType.NodePrep;
            ReleaseNodeTaskParameters = new ParametersTask("ReleaseNodeTask");
            ReleaseNodeTaskParameters.CommandLine = "skip";
            ReleaseNodeTaskParameters.TaskTypeOption = HPCTaskType.NodeRelease;
        }
    }
}
