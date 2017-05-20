using IQ.Workflows;
using IQ.Workflows.WorkFlowParameters;
using Run32.Backend;

namespace DeconTools.Workflows.Backend.Core
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
