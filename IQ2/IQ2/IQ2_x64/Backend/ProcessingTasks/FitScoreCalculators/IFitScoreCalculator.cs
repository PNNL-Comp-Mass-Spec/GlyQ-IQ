using System.Collections.Generic;
using IQ_X64.Backend.Core;
using Run64.Backend.Core;
using Run64.Backend.Core.Results;

namespace IQ_X64.Backend.ProcessingTasks.FitScoreCalculators
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
