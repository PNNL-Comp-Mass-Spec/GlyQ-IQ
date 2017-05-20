using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Workflows.Backend.Core;
using IQGlyQ.Results;

namespace IQGlyQ.Workflows
{
    public static class WorkflowUtilities
    {
        public static List<IqGlyQResult> CastToGlyQiQResult(IqResult result, double fitscoreCuttoff)
        {
            List<IqResult> children = result.ChildResults().ToList();

            List<IqGlyQResult> glyQIQChildren = new List<IqGlyQResult>();

            foreach (var iqResult in children)
            {
                List<IqResult> children2 = iqResult.ChildResults().ToList();
                foreach (var iqResult1 in children2)
                {
                    IqGlyQResult iqResulttemp = (IqGlyQResult)iqResult1;
                    //if(iqResulttemp.ObservedIsotopicProfile !=null && iqResulttemp.ObservedIsotopicProfile.Score<fitscoreCuttoff)
                    //{
                    glyQIQChildren.Add(iqResulttemp);
                    //}
                    //glyQIQChildren.Add((IqGlyQResult)iqResult1);
                }

            }
            return glyQIQChildren;
        }


        public static void PrintHits(IqResult result)
        {
            var chldren = result.ChildResults().ToList(); //3 children


            foreach (var iqResult in chldren)
            {
                Console.WriteLine("charge " + iqResult.Target.ChargeState);

                var chldrenTargets = iqResult.ChildResults().ToList(); //3 7

                foreach (var baseTarget in chldrenTargets)
                {
                    var convertedBaseTarget = (IqGlyQResult)baseTarget;
                    Console.WriteLine("++ " + convertedBaseTarget.ToChild.TargetFragment.ScanLCTarget + "_" +
                                      convertedBaseTarget.ToChild.TargetFragment.ChargeState);
                }
            }
        }
    }
}
