using IQ.Workflows;

namespace IQ_X64.Workflows.WorkFlowParameters
{
	/// <summary>
	/// Concrete parameter class for the TopDownTargetedWorkflowExecutor
	/// </summary>
	public class TopDownTargetedWorkflowExecutorParameters : WorkflowExecutorBaseParameters
	{

	    public bool ExportChromatogramData { get; set; }

        public override GlobalsWorkFlow.TargetedWorkflowTypes WorkflowType
		{
            get { return GlobalsWorkFlow.TargetedWorkflowTypes.TopDownTargetedWorkflowExecutor1; }
		}
	}
}
