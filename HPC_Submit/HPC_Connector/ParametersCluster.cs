
namespace HPC_Connector
{
    public class ParametersCluster
    {
        /// <summary>
        /// name of HPC cluster
        /// </summary>
        public string ClusterName { get; set; }

        ///// <summary>
        ///// Which Node Group to Send the Job to
        ///// </summary>
        //public string WorkerNodeGroup { get; set; }

        public ParametersCluster(string clusterName)
        {
            ClusterName = clusterName;
        }
    }
}
