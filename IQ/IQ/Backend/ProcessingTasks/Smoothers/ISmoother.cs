using IQ.Backend.Core;
using Run32.Backend.Core;
using Run32.Backend.Data;
using Run32.Utilities;

namespace IQ.Backend.ProcessingTasks.Smoothers
{
    public abstract class Smoother:TaskIQ
    {

        public override void Execute(ResultCollection resultList)
        {
            Check.Require(resultList.Run.XYData != null, "Smoother not executed; no data in XYData object");
            resultList.Run.XYData = Smooth(resultList.Run.XYData);
        }

        public abstract XYData Smooth(XYData xYData);


     
    }
}
