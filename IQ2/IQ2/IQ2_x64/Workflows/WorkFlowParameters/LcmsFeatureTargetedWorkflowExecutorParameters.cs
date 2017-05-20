using IQ.Workflows;

namespace IQ_X64.Workflows.WorkFlowParameters
{
    public class LcmsFeatureTargetedWorkflowExecutorParameters : WorkflowExecutorBaseParameters
    {

        public string MassTagsForReference { get; set; }

        public override GlobalsWorkFlow.TargetedWorkflowTypes WorkflowType
        {
            get { return GlobalsWorkFlow.TargetedWorkflowTypes.LcmsFeatureTargetedWorkflowExecutor1; }
        }
    }
}
