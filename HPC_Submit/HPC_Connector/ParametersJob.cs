
namespace HPC_Connector
{
    public class ParametersJob
    {
        /// <summary>
        /// Name of Job
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// Name of HPC Project.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Level of Priority for which jobs run first
        /// </summary>
        public PriorityLevel PriorityLevel { get; set; }

        /// <summary>
        /// Targeting Cores, Sockets, or Nodes
        /// </summary>
        public HardwareUnitType TargetHardwareUnitType { get; set; }

		/// <summary>
		/// Maximum job runtime, in hours
		/// Defaults to 48 hours
		/// </summary>
	    public int MaxRunTimeHours { get; set; }

	    /// <summary>
        /// Minimum number of cores needed
        /// </summary>
        public int MinNumberOfCores { get; set; }

        /// <summary>
        /// Maximum cores asked for
        /// </summary>
        public int MaxNumberOfCores { get; set; }

        /// <summary>
        /// HPC tempate from Deception
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// when this job runs, does it block off the full node and not allow other jobs to open cores
        /// </summary>
        public bool isExclusive { get; set; }

       public ParametersJob(string jobName)
        {
	        if (string.IsNullOrEmpty(jobName))
		        jobName = "GenericJob";
			    
			// Make sure the job name is less than 80 characters long
	        if (jobName.Length > 80)
		        jobName = jobName.Substring(0, 80);

            JobName = jobName;

	        MaxRunTimeHours = 48;

            TemplateName = "Default";

            PriorityLevel = PriorityLevel.Normal;
        }
    }

 

    
}
