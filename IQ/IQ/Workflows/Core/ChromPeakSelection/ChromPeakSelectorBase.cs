using System.Collections.Generic;
using IQ.Backend.Core;
using IQ.Workflows.Utilities;
using Run32.Backend;
using Run32.Backend.Core;
using Run32.Backend.Core.Results;

namespace IQ.Workflows.Core.ChromPeakSelection
{
    public abstract class ChromPeakSelectorBase : TaskIQ
    {
        protected ChromPeakUtilities ChromPeakUtilities = new ChromPeakUtilities();

        #region Constructors

        protected ChromPeakSelectorBase()
        {
            IsotopicProfileType = Globals.IsotopicProfileType.UNLABELLED;
        }

        #endregion

        #region Public Methods

        public abstract Peak SelectBestPeak(List<ChromPeakQualityData> peakQualityList, bool filterOutFlaggedIsotopicProfiles);


        #endregion



        #region Properties

        public abstract ChromPeakSelectorParameters Parameters { get; set; }

        public Globals.IsotopicProfileType IsotopicProfileType { get; set; }


        #endregion
 
        protected virtual void UpdateResultWithChromPeakAndLCScanInfo(TargetedResultBase result, ChromPeak bestPeak)
        {
            result.AddSelectedChromPeakAndScanSet(bestPeak, result.Run.CurrentScanSet, IsotopicProfileType);
            result.WasPreviouslyProcessed = true;    //indicate that this result has been added to...  use this to help control the addition of labelled (N15) data
        }

    }
}
