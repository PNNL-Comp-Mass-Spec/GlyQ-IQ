using IQ_X64.Workflows;
using IQ_X64.Workflows.WorkFlowParameters;
using Run64.Backend;

namespace IQGlyQ.IQTasks
{
    public class IqDeuteratedParameter : WorkflowParameters
    {
        #region Constructors

        public IqDeuteratedParameter()
        {
            IsotopeProfileType = Globals.LabellingType.Deuterium;
            //IsotopeProfileType = DeconTools.Backend.Globals.LabellingType.NONE;
            LabelingTypeSwitch  = Globals.IsotopicProfileType.LABELLED;
            IsotopeLabelingEfficiency = 1;//this is defult and does not effect number
            //IsotopeLabelingEfficiency = isotopeLabelingEfficiency;
            MolarMixingFractionOfH = 0.51;
            IsotopeLowPeakCuttoff = 0.005;
            MinIntensity = 1;
            //ResultType = DeconTools.Backend.Globals.ResultType.DEUTERATED_TARGETED_RESULT;
            ChromToleranceInPPM = 10;
            ChromSmootherNumPointsInSmooth = 9;
            MinRelativeIntensityForChromCorrelator = 0.1;
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// sets which type of isotope profile to use based on labeling type
        /// </summary>
        public Globals.LabellingType IsotopeProfileType { get; set; }
        
         /// <summary>
        /// sets which type of isotope profile to use... Either a labeled profile or unlabeled profile
        /// </summary>
        public Globals.IsotopicProfileType LabelingTypeSwitch { get; set; }
        
        /// <summary>
        /// LabelingEfficiency Ranges between 0 and 1.0
        /// </summary>
        public double IsotopeLabelingEfficiency { get; set; }

        /// <summary>
        /// minimum intenstiy to consider.  typically =1
        /// </summary>
        public double MinIntensity { get; set; }

        /// <summary>
        /// how the two samples were mixed together.  0.5 means 1:1 ratio.  Range is 0 to 1.  .25 is 25%H and 75%D (check this)
        /// </summary>
        public double MolarMixingFractionOfH { get; set; }

        /// <summary>
        /// how low to cuttoff tail of the isotope pattern
        /// </summary>
        public double IsotopeLowPeakCuttoff { get; set; }

        /// <summary>
        /// windows for EIC
        /// </summary>
        public double ChromToleranceInPPM { get; set; }

        public int ChromSmootherNumPointsInSmooth { get; set; }

        /// <summary>
        /// removes noise fromEIC
        /// </summary>
        public double MinRelativeIntensityForChromCorrelator { get; set; }


        public override GlobalsWorkFlow.TargetedWorkflowTypes WorkflowType
        {
            get { return GlobalsWorkFlow.TargetedWorkflowTypes.Deuterated; }
        }

        #endregion

        
    }
}
