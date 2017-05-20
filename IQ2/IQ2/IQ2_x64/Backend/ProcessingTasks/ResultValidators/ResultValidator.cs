
using IQ_X64.Backend.Core;
using Run64.Backend.Core;
using Run64.Backend.Core.Results;

namespace IQ_X64.Backend.ProcessingTasks.ResultValidators
{
    public abstract class ResultValidator:TaskIQ
    {
        #region Constructors
        #endregion

        #region Properties
        public abstract IsosResult CurrentResult { get; set; }
        #endregion

        #region Public Methods
        public abstract void ValidateResult(ResultCollection resultColl, IsosResult currentResult);
        #endregion

        #region Private Methods
        #endregion
        public override void Execute(ResultCollection resultList)
        {
            ValidateResult(resultList, this.CurrentResult);
        }
    }
}
