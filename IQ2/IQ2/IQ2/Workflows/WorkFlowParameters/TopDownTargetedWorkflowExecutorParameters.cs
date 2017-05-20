using IQ.Workflows;
using IQ.Workflows.WorkFlowParameters;
using Run32.Backend;

namespace DeconTools.Workflows.Backend.Core
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
