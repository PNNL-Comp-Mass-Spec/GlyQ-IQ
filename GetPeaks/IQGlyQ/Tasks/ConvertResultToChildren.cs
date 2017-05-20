using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using DeconTools.Workflows.Backend.Core;
using IQGlyQ.Results;

namespace IQGlyQ.Tasks
{
    public class ConvertResultToChildren: Task
    {

        public void Convert(IqResult iQresultIn)
        {
            IqGlyQResult iQresult = (IqGlyQResult)iQresultIn;

            if (iQresult.TargetAddOns != null)
            {
                foreach (FragmentResultsObjectHolderIq result in iQresult.TargetAddOns)
                {
                    IqGlyQResult convertedResult = new IqGlyQResult(result.TargetFragment);
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
