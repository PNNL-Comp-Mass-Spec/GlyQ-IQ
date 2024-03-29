﻿ 

using Run64.Backend;

namespace IQ_X64.Backend.ProcessingTasks.TargetedFeatureFinders
{
    public class BasicTFF : TFFBase
    {
        #region Constructors
        public BasicTFF()
            : this(5)     // default toleranceInPPM
        {

        }

        public BasicTFF(double toleranceInPPM)
            : this(toleranceInPPM,true)
        {

        }

        public BasicTFF(double toleranceInPPM, bool requiresMonoPeak)
            : this(toleranceInPPM, requiresMonoPeak, Globals.IsotopicProfileType.UNLABELLED)
        {
        }

        public BasicTFF(double toleranceInPPM, bool requiresMonoPeak, Globals.IsotopicProfileType isotopicProfileTarget)
        {
            this.ToleranceInPPM = toleranceInPPM;
            this.NeedMonoIsotopicPeak = requiresMonoPeak;
            this.IsotopicProfileType = isotopicProfileTarget;
            this.NumPeaksUsedInAbundance = 1;

        }

        #endregion






    }
}
