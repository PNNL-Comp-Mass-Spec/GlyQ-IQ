using System;
using IQ.Backend.Core;
using IQ.Workflows.Core;
using IQGlyQ.Results;
using Run32.Backend.Core;

namespace IQGlyQ.Tasks
{
    public class ConvertResultToChildren: TaskIQ
    {

        public void Convert(IqResult iQresultIn)
        {
            IqGlyQResult iQresult = (IqGlyQResult)iQresultIn;

            if (iQresult.TargetAddOns != null)
            {
                foreach (FragmentResultsObjectHolderIq result in iQresult.TargetAddOns)
                {
                    IqGlyQResult convertedResult = new IqGlyQResult(result.Primary_Target);
                    convertedResult.ToChild = result;

                    iQresult.AddResult(convertedResult);
                }
            }

            if (iQresult.FutureTargets != null)
            {
                foreach (FragmentResultsObjectHolderIq result in iQresult.FutureTargets)
                {
                    IqGlyQResult convertedResult = new IqGlyQResult(result.TargetParent);
                    convertedResult.ToChild = result;

                    iQresult.AddResult(convertedResult);
                }
            }
        }

        public override void Execute(ResultCollection resultList)
        {
            throw new NotImplementedException();
        }
    }
}
