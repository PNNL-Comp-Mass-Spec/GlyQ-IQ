using System;
using Run64.Backend.Core.Results;
using Run64.Backend.Data;

namespace IQ_X64.Workflows.Core
{
    /// <summary>
    /// Contains information that is usually passed along to user interface
    /// via a background worker. This class represents a light class
    /// so that heavy objects (eg Workflow) are not passed. 
    /// </summary>
    public class TargetedWorkflowExecutorProgressInfo
    {

        public string ProgressInfoString { get; set; }

        public DateTime Time { get; set; }
     
        public TargetedResultBase Result { get; set; }

        public  XYData MassSpectrumXYData { get; set; }

        public  XYData ChromatogramXYData { get; set; }

        public bool IsGeneralProgress { get; set; }
    }
}
