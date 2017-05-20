
using IQ.Workflows;
using IQ.Workflows.WorkFlowParameters;
using Run32.Backend;

namespace DeconTools.Workflows.Backend.Core
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
