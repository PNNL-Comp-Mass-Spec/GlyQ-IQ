using Run32.Backend;

namespace IQ.Workflows.WorkFlowParameters
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
