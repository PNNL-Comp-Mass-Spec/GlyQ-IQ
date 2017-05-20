using System.Collections.Generic;

namespace IQ.Workflows.FileIO
{
    public abstract class ExporterBase<T>
    {
        #region Constructors
        #endregion

        #region Properties
        #endregion

        #region Public Methods
        public abstract void ExportResults(IEnumerable<T> results);
        #endregion

        #region Private Methods
        #endregion
    }
}
