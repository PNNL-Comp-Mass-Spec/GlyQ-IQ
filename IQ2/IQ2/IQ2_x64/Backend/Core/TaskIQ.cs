
using Run64.Backend.Core;

namespace IQ_X64.Backend.Core
{
    public abstract class TaskIQ
    {

        public abstract void Execute(ResultCollection resultList);

        public virtual string Name {get;set;}
        
        public virtual void Cleanup()
        {
            return;
        }


    }
}
