using DeconTools.Workflows.Backend.Core;
using DeconTools.Workflows.Backend;

//namespace DeconTools.Workflows.Backend.Core
namespace IQGlyQ
{
    public class DeuteratedTargetedWorkflowParameters : TargetedWorkflowParameters
    {

        #region Constructors

        public DeuteratedTargetedWorkflowParameters()
        {
            IsotopeProfileType = DeconTools.Backend.Globals.LabellingType.Deuterium;
            //IsotopeProfileType = DeconTools.Backend.Globals.LabellingType.NONE;
            LabelingTypeSwitch  = DeconTools.Backend.Globals.IsotopicProfileType.LABELLED;
            IsotopeLabelingEfficiency = 1;//this is defult and does not effect number
            //IsotopeLabelingEfficiency = isotopeLabelingEfficiency;
            MolarMixingFractionOfH = 0.51;
            MinIntensity = 1;
            IsotopeLowPeakCuttoff = 0.005;
            ResultType = DeconTools.Backend.Globals.ResultType.DEUTERATED_TARGETED_RESULT;
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// sets which type of isotope profile to use based on labeling type
        /// </summary>
        public DeconTools.Backend.Globals.LabellingType IsotopeProfileType { get; set; }
        
         /// <summary>
        /// sets which type of isotope profile to use... Either a labeled profile or unlabeled profile
        /// </summary>
        public DeconTools.Backend.Globals.IsotopicProfileType LabelingTypeSwitch { get; set; }
        
        /// <summary>
        /// LabelingEfficiency Ranges between 0 and 1.0
        /// </summary>
        public double IsotopeLabelingEfficiency { get; set; }

        /// <summary>
        /// minimum intenstiy to consider.  typically =1
        /// </summary>
        public double MinIntensity { get; set; }

        /// <summary>
        /// how the two samples were mixed together.  0.5 means 1:1 ratio.  Range is 0 to 1.  .25 ia 
        /// </summary>
        public double MolarMixingFractionOfH { get; set; }

        /// <summary>
        /// how low to cuttoff tail of the isotope pattern
        /// </summary>
        public double IsotopeLowPeakCuttoff { get; set; }

        public override Globals.TargetedWorkflowTypes WorkflowType
        {
            get { return Globals.TargetedWorkflowTypes.Deuterated; }
        }

        #endregion
    }
}
