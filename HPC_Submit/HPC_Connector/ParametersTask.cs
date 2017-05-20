namespace HPC_Connector
{
    public class ParametersTask
    {
        /// <summary>
        /// Name of Task
        /// </summary>
        public string TaskName { get; set; }
        
        /// <summary>
        /// Command line for execution
        /// </summary>
        public string CommandLine { get; set; }

        /// <summary>
        /// Work directory used in task
        /// </summary>
        public string WorkDirectory { get; set; }

        /// <summary>
        /// Standard output for task
        /// </summary>
        public string StdOutFilePath { get; set; }

	    public bool FailJobOnFailure { get; set; }

        /// <summary>
        /// which type of task is this (Basic,ParametricSweep,NodePrep,NodeRelease,Service
        /// </summary>
	    public HPCTaskType TaskTypeOption { get; set; }

        /// <summary>
        /// for parametric tasks, we need a start index
        /// </summary>
        public int ParametricStartIndex { get; set; }

        /// <summary>
        /// for parametric tasks, we need a stop index
        /// </summary>
        public int ParametricStopIndex { get; set; }

        /// <summary>
        /// for parametric tasks, we need an increment.  usually 1
        /// </summary>
        public int ParametricIncrement { get; set; }

        public ParametersTask(string taskName)
        {
			if (string.IsNullOrEmpty(taskName))
				taskName = "GenericTask";

			// Make sure the task name is less than 80 characters long
			if (taskName.Length > 80)
				taskName = taskName.Substring(0, 80);

            TaskName = taskName;
            ParametricIncrement = 1;
        }
    }
}
