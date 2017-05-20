using System;
using IQ.Backend.ProcessingTasks;
using IQ.Backend.ProcessingTasks.MSGenerators;
using IQ.Backend.Core;
using IQ.Workflows.Core;
using IQ.Workflows.WorkFlowParameters;
using Run32.Backend.Core;
using Run32.Backend.Core.Results;

namespace IQ.Workflows.WorkFlowPile
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
