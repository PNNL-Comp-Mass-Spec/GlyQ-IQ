using DeconTools.Workflows.Backend.Core;

namespace Sipper.ViewModel
{
    public class SipperWorkflowParameterViewModel:ViewModelBase
    {

        #region Constructors
        public SipperWorkflowParameterViewModel(TargetedWorkflowParameters parameters)
        {
            WorkflowParameters = parameters;
        }

        #endregion

        #region Properties

        protected TargetedWorkflowParameters WorkflowParameters { get; set; }


        public double ChromToleranceInPPM { get; set; }


        //TODO:  fill in others


        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion

    }
}
