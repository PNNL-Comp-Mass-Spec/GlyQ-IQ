using System.ComponentModel;
using DeconTools.Workflows.Backend.Core;

namespace IQGlyQ
{
    public class BasicMultiTargetedWorkflowExecutor : MultiTargetedWorkflowExecutor
    {

        #region Constructors
        public BasicMultiTargetedWorkflowExecutor(WorkflowExecutorBaseParameters parameters, string datasetPath, BackgroundWorker backgroundWorker = null) 
            : base(parameters, datasetPath, backgroundWorker) { }
		public BasicMultiTargetedWorkflowExecutor(WorkflowExecutorBaseParameters workflowExecutorParameters, WorkflowParameters workflowParameters, string datasetPath) : base(workflowExecutorParameters, workflowParameters, datasetPath) { }
        public BasicMultiTargetedWorkflowExecutor(WorkflowExecutorBaseParameters workflowExecutorParameters, 
            TargetedWorkflow workflow, string datasetPath, BackgroundWorker backgroundWorker =null) : base(workflowExecutorParameters, workflow, datasetPath,backgroundWorker) { }

        #endregion


        #region Public Methods

        #endregion

    }
}
