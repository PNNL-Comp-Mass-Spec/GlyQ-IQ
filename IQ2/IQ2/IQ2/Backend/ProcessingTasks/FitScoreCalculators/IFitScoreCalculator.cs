using System.Collections.Generic;
using IQ.Backend.Core;
using Run32.Backend.Core;
using Run32.Backend.Core.Results;

namespace IQ.Backend.ProcessingTasks.FitScoreCalculators
{
    public abstract class IFitScoreCalculator : TaskIQ
    {
        public abstract void GetFitScores(IEnumerable<IsosResult> isosResults);

        public override void Execute(ResultCollection resultList)
        {
            GetFitScores(resultList.IsosResultBin);
        }
    }
}
