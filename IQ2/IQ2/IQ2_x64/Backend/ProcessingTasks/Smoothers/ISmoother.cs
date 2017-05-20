

using IQ_X64.Backend.Core;
using Run64.Backend.Core;
using Run64.Backend.Data;
using Run64.Utilities;

namespace IQ_X64.Backend.ProcessingTasks.Smoothers
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
