using System.ComponentModel;
using IQ.Workflows.WorkFlowParameters;

namespace IQ.Workflows.WorkFlowPile
{
    public class BasicTargetedWorkflowExecutor : TargetedWorkflowExecutor
    {

        #region Constructors
        public BasicTargetedWorkflowExecutor(WorkflowExecutorBaseParameters parameters, string datasetPath, BackgroundWorker backgroundWorker = null) 
            : base(parameters, datasetPath, backgroundWorker) { }
		public BasicTargetedWorkflowExecutor(WorkflowExecutorBaseParameters workflowExecutorParameters, WorkflowParameters workflowParameters, string datasetPath) : base(workflowExecutorParameters, workflowParameters, datasetPath) { }
        public BasicTargetedWorkflowExecutor(WorkflowExecutorBaseParameters workflowExecutorParameters, 
            TargetedWorkflow workflow, string datasetPath, BackgroundWorker backgroundWorker =null) : base(workflowExecutorParameters, workflow, datasetPath,backgroundWorker) { }

        #endregion


        #region Public Methods

        #endregion

    }
}
