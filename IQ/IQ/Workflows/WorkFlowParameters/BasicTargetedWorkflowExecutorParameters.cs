using DeconTools.Workflows.Backend.Core;

namespace IQ.Workflows.WorkFlowParameters
{
    /// <summary>
    /// Concrete parameter class for the basicTargetedWorkflowExecutor
    /// </summary>
    public class BasicTargetedWorkflowExecutorParameters : WorkflowExecutorBaseParameters
    {


        public override GlobalsWorkFlow.TargetedWorkflowTypes WorkflowType
        {
            get { return GlobalsWorkFlow.TargetedWorkflowTypes.BasicTargetedWorkflowExecutor1; }
        }
    }
}
