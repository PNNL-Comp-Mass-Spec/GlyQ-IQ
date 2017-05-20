using System;
using IQ_X64.Backend.Core;
using IQ_X64.Backend.ProcessingTasks;
using IQ_X64.Backend.ProcessingTasks.MSGenerators;
using IQ_X64.Workflows.Core;
using IQ_X64.Workflows.WorkFlowParameters;
using Run64.Backend.Core;
using Run64.Backend.Core.Results;

namespace IQ_X64.Workflows.WorkFlowPile
{
    public abstract class WorkflowBase
    {

        string Name { get; set; }

        protected MSGenerator MSGenerator { get; set; }

        public abstract WorkflowParameters WorkflowParameters { get; set; }


        #region Public Methods

        public abstract void InitializeWorkflow();

        public abstract void Execute();

        public virtual void ExecuteTask(TaskIQ task)
        {
            if (Result != null && !Result.FailedResult)
            {
                task.Execute(this.Run.ResultCollection);
            }
        }





        public virtual void InitializeRunRelatedTasks()
        {
            if (Run != null)
            {
                MSGenerator = MSGeneratorFactory.CreateMSGenerator(this.Run.MSFileType);

            }
        }


        private Run _run;
        public Run Run
        {
            get
            {
                return _run;
            }
            set
            {
                if (_run != value)
                {
                    _run = value;

                    if (_run!=null)
                    {
                        InitializeRunRelatedTasks(); 
                    }
                    
                }

            }
        }

        public TargetedResultBase Result { get; set; }


        //public static WorkflowBase CreateWorkflow(string workflowParameterFilename)
        //{



        //}




        #endregion

        public virtual void Execute(IqTarget target)
        {
            throw new NotImplementedException("IqWorkflow not implemented");
        }
    }
}
