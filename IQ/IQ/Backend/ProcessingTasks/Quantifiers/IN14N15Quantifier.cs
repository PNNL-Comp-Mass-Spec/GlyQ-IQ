using Run32.Backend.Core;

namespace IQ.Backend.ProcessingTasks.Quantifiers
{
    public abstract class N14N15Quantifier
    {
        #region Constructors
        #endregion

        #region Properties

        #endregion

        #region Public Methods
        public abstract double GetRatio(double[] xvals, double[] yvals,
            IsotopicProfile iso1, IsotopicProfile iso2,
            double backgroundIntensity);
     

        #endregion

        #region Private Methods
        #endregion
    }
}
