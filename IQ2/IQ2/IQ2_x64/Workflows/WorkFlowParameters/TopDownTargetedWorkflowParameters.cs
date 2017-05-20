 
using IQ.Workflows;
using Run64.Backend;

namespace IQ_X64.Workflows.WorkFlowParameters
{
	public class TopDownTargetedWorkflowParameters : TargetedWorkflowParameters
	{
		public TopDownTargetedWorkflowParameters()
		{
			ResultType = Globals.ResultType.TOPDOWN_TARGETED_RESULT;
		}

        public bool SaveChromatogramData { get; set; }


        public override GlobalsWorkFlow.TargetedWorkflowTypes WorkflowType
		{
			get
			{
                return GlobalsWorkFlow.TargetedWorkflowTypes.TopDownTargeted1;
			}
		}
	}
}
