 
using IQ.Workflows;
using Run64.Backend;

namespace IQ_X64.Workflows.WorkFlowParameters
{
    public class SipperTargetedWorkflowParameters : TargetedWorkflowParameters
    {

        public SipperTargetedWorkflowParameters()
        {
            ChromPeakSelectorMode = Globals.PeakSelectorMode.ClosestToTarget;
            
            ResultType = Globals.ResultType.SIPPER_TARGETED_RESULT;
        }


        public override GlobalsWorkFlow.TargetedWorkflowTypes WorkflowType
        {
            get { return GlobalsWorkFlow.TargetedWorkflowTypes.SipperTargeted1; }
        }
    }
}
