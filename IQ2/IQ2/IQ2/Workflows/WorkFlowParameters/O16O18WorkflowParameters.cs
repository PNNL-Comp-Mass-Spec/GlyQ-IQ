using Run32.Backend;

namespace IQ.Workflows.WorkFlowParameters
{
    public class O16O18WorkflowParameters : TargetedWorkflowParameters
    {

        #region Constructors
        public O16O18WorkflowParameters()
        {
            ChromGeneratorMode = Globals.ChromatogramGeneratorMode.O16O18_THREE_MONOPEAKS;

            ResultType = Globals.ResultType.O16O18_TARGETED_RESULT;

        }
        #endregion


        public override GlobalsWorkFlow.TargetedWorkflowTypes WorkflowType
        {
            get { return GlobalsWorkFlow.TargetedWorkflowTypes.O16O18Targeted1; }
        }
    }
}
