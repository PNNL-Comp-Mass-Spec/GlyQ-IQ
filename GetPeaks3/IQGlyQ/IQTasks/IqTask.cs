using System;
using IQ_X64.Workflows.Core;
using IQ_X64.Workflows.WorkFlowParameters;
using Run64.Utilities;

namespace IQGlyQ.IQTasks
{
    public abstract class IqTask
    {
        public IqTask(WorkflowParameters parameters)
        {
            WorkflowParameters = parameters;
        }

        public abstract WorkflowParameters WorkflowParameters { get; set; }

        
        #region Properties

        public bool Success { get; set; }

        public bool IsIqTaskInitialized { get; set; }

        public string IqTaskStatusMessage { get; set; }

        public string Name
        {
            get { return ToString(); }
        }

        #endregion


        protected virtual void ValidateParameters()
        {
            Check.Require(WorkflowParameters != null, "Cannot validate workflow parameters. Parameters are null");

            //bool pointsInSmoothIsEvenNumber = (WorkflowParameters.ChromSmootherNumPointsInSmooth % 2 == 0);
            //if (pointsInSmoothIsEvenNumber)
            //{
            //    throw new ArgumentOutOfRangeException("Points in chrom smoother is an even number, but must be an odd number.");
            //}
        }


        public virtual void InitializeIqTask()
        {
            DoPreInitialization();

            DoMainInitialization();

            DoPostInitialization();

            IsIqTaskInitialized = true;

        }


        protected virtual void DoPreInitialization() { }

        protected virtual void DoPostInitialization() { }


        protected virtual void DoMainInitialization()
        {
            ValidateParameters();
        }



        public virtual void Execute(IqResult result)
        {
            ValidateParameters();

            if (!IsIqTaskInitialized)
            {
                InitializeIqTask();
            }
            
            ExecuteIqTask(result);

            ExecutePostIqTaskHook(result);
        }


        protected virtual void HandleIqTaskError(IqResult result, Exception ex)
        {
            Success = false;
            IqTaskStatusMessage = "Unexpected IQ workflow error. Error info: " + ex.Message;

            if (ex.Message.Contains("COM") || ex.Message.ToLower().Contains(".dll"))
            {
                throw new ApplicationException("There was a critical failure! Error info: " + ex.Message);
            }

            //result.Target.ErrorDescription = ex.Message + "\n" + ex.StackTrace;
            //result.FailedResult = true;
        }


        protected virtual void ExecutePostIqTaskHook(IqResult result)
        {
            if (result != null && Success)
            {
                IqTaskStatusMessage = "IqTarget " + result.Target.ID + "; m/z= " + result.Target.Code;
            }
        }

        protected virtual void ExecuteIqTask(IqResult result)
        {

        }

        //TODO:  later will make this abstract/virtual.  Workflow creates the type of IqResult we want
        protected internal virtual IqResult CreateIQResult(IqTarget target)
        {
            return new IqResult(target);
        }
    }
}
