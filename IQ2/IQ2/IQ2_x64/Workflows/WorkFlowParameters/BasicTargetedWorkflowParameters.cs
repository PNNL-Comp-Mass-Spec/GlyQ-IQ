 
using IQ.Workflows;
using Run64.Backend;

namespace IQ_X64.Workflows.WorkFlowParameters
{
    public class BasicTargetedWorkflowParameters:TargetedWorkflowParameters
    {

      
        public BasicTargetedWorkflowParameters()
        {
           
            this.ResultType = Globals.ResultType.BASIC_TARGETED_RESULT;
        }

        public override GlobalsWorkFlow.TargetedWorkflowTypes WorkflowType
        {
            get
            {
                return GlobalsWorkFlow.TargetedWorkflowTypes.UnlabelledTargeted1;
            }
        }

    }
}
