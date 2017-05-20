using System;


using IQ_X64.Backend.Core;
using Run64.Backend.Core;
using Run64.Utilities;

namespace IQ_X64.Backend.ProcessingTasks.TheorFeatureGenerator
{
    public abstract class ITheorFeatureGenerator : TaskIQ
    {
        #region Constructors
        #endregion

        #region Properties
        #endregion

        #region Public Methods
        public abstract void LoadRunRelatedInfo(ResultCollection results);
        public abstract void GenerateTheorFeature(TargetBase mt);

        public virtual IsotopicProfile GenerateTheorProfile(string empiricalFormula, int chargeState)
        {
            throw new NotImplementedException("Not implemented");
        }

        #endregion

        #region Private Methods
        #endregion
        public override void Execute(ResultCollection resultList)
        {
            Check.Require(resultList.Run.CurrentMassTag != null, "Theoretical feature generator failed. No target mass tag was provided");

            LoadRunRelatedInfo(resultList);
            
            GenerateTheorFeature(resultList.Run.CurrentMassTag);
        }


        
    }
}
