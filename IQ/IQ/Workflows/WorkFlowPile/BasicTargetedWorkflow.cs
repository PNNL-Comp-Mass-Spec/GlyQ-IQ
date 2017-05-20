using IQ.Workflows.Core;
using IQ.Workflows.WorkFlowParameters;
using Run32.Backend;
using Run32.Backend.Core;

namespace IQ.Workflows.WorkFlowPile
{
    public class BasicTargetedWorkflow : TargetedWorkflow
    {


        #region Constructors

        public BasicTargetedWorkflow(Run run, TargetedWorkflowParameters parameters)
            : base(run, parameters)
        {
        }

        public BasicTargetedWorkflow(TargetedWorkflowParameters parameters):base (parameters)
        {
        }


        #endregion


        protected override Globals.ResultType GetResultType()
        {
            return Globals.ResultType.BASIC_TARGETED_RESULT;
        }


        protected override void ExecutePostWorkflowHook()
        {
            base.ExecutePostWorkflowHook();

            if (Result != null && Result.Target != null && Result.IsotopicProfile!=null && Success)
            {
               if (Run.IsMsAbundanceReportedAsAverage)
               {
                   Result.IntensityAggregate = Result.IntensityAggregate * Result.NumMSScansSummed;
               }

            }

        }

    }
}
