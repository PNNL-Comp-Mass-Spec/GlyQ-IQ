using System.Collections.Generic;
using IQ.Workflows;
using IQ.Workflows.WorkFlowParameters;
using IQGlyQ.Enumerations;
using IQGlyQ.Functions;
using IQGlyQ.Processors;
using IQGlyQ.Tasks.FitScores;
using PNNLOmics.Data.Constants;
using IQGlyQ.TargetGenerators;
using IQGlyQ.Objects;
using Run32.Backend;

//namespace DeconTools.Workflows.Backend.Core
namespace IQGlyQ
{


    public class FragmentedTargetedWorkflowParametersIQ : TargetedWorkflowParameters
    {

        #region Properties

        /// <summary>
        /// sets which type of isotope profile to use based on labeling type
        /// </summary>
        public Run32.Backend.Globals.LabellingType IsotopeProfileType { get; set; }

        /// <summary>
        /// sets which type of isotope profile to use... Either a labeled profile or unlabeled profile
        /// </summary>
        public Run32.Backend.Globals.IsotopicProfileType LabelingTypeSwitch { get; set; }

        /// <summary>
        /// LabelingEfficiency Ranges between 0 and 1.0
        /// </summary>
        public double IsotopeLabelingEfficiency { get; set; }

        /// <summary>
        /// how the two samples were mixed together.  0.5 means 1:1 ratio.  Range is 0 to 1.  .25 ia 
        /// </summary>
        public double MolarMixingFractionOfH { get; set; }

        /// <summary>
        /// how low to cuttoff tail of the isotope pattern
        /// </summary>
        public double IsotopeLowPeakCuttoff { get; set; }

        /// <summary>
        /// how low to cuttoff tail of the peak correlator
        /// </summary>
        public double MinRelativeIntensityForChromCorrelator { get; set; }

        /// <summary>
        /// which fragments tol search for
        /// </summary>
        public List<FragmentIQTarget> FragmentsIq { get; set; }

        /// <summary>
        /// highest allowable fit score for iff
        /// </summary>
        public double FitScoreCuttoff { get; set; }

        /// <summary>
        /// determines if two EIXC peaks correlate.  Higher than this value correlates while lower does not correlate.  suggested 0.5 0.6
        /// </summary>
        public double CorrelationScoreCuttoff { get; set; }

        /// <summary>
        /// -1 will take all peaks.  0 will use the average of minima.  +1, +2 etc will be average + sigma thresholding
        /// </summary>
        public float ChromPeakSigmaThreshold { get; set; }

        /// <summary>
        /// all you need for processing lc chromatograms
        /// </summary>
        public ProcessingParametersChromatogram LCParameters { get; set; }

        /// <summary>
        /// all you need for processig mass spectra
        /// </summary>
        public ProcessingParametersMassSpectra MSParameters { get; set; }


        ///// <summary>
        ///// all your iso modification parameters
        ///// </summary>
        //public IsotopeParameters IsoParameters { get; set; }//this lives in ms parameters

        /// <summary>
        /// do we want to test parents or children (larger or smaller
        /// </summary>
        public EnumerationParentOrChild GenerationDirection { get; set; }

        /// <summary>
        /// file strucutre fo PIC processing
        /// </summary>
        public EnumerationIsPic IsPic { get; set; }

        /// <summary>
        /// max allowable charge state.  1000 etc. will use automatic determination based on mass range
        /// </summary>
        public int ChargeStateMax { get; set; }

        /// <summary>
        /// min allowable charge state.  1 is typical
        /// </summary>
        public int ChargeStateMin { get; set; }

        ///// <summary>
        ///// which type of data set are we tuning for
        ///// </summary>
        //public EnumerationDataset DatasetType { get; set; }

        ///// <summary>
        ///// which type of data set are we tuning for
        ///// </summary>
        //public EnumerationIsotopicProfileMode IsotopeProfileMode { get; set; }

        //TODO THESE Need to be added for minimum requirements for correlations and they will show up in the correlation object
        //int minPeaksForAcceptableIsotopeProfile = 3;
        //int minSizeOfEIC = 3;


        //     /// <summary>
        //     /// Penalize Fitscore  based on this any peaks to the left of the monoisotopiic peak.  Zeroes need to be added to the theoretical isotope profile
        //     /// </summary>
        //     public int NumberOfPeaksToLeftForPenalty { get; set; }


        public override GlobalsWorkFlow.TargetedWorkflowTypes WorkflowType
        {
            get { return GlobalsWorkFlow.TargetedWorkflowTypes.Deuterated; }
        }

        #endregion

        #region Constructors

        public FragmentedTargetedWorkflowParametersIQ()
        {
            Initialize();

            FragmentsIq = SetUpDefaultTargets();
        }


        public FragmentedTargetedWorkflowParametersIQ(List<FragmentIQTarget> fragmentsIqIn)
        {
            Initialize();

            FragmentsIq = fragmentsIqIn;
        }

        #endregion

        private void Initialize()
        {
            //DH labeling
            IsotopeProfileType = Run32.Backend.Globals.LabellingType.Deuterium;//default
            //IsotopeProfileType = DeconTools.Backend.Globals.LabellingType.NONE;
            LabelingTypeSwitch = Run32.Backend.Globals.IsotopicProfileType.LABELLED;//default
            //LabelingTypeSwitch = DeconTools.Backend.Globals.IsotopicProfileType.UNLABELLED;
            IsotopeLabelingEfficiency = 1;//this is defult and does not effect number
            //IsotopeLabelingEfficiency = isotopeLabelingEfficiency;
            MolarMixingFractionOfH = 0.51;
            ResultType = Globals.ResultType.DEUTERATED_TARGETED_RESULT;

            //LC
            MinRelativeIntensityForChromCorrelator = 0.1;
            LCParameters = new ProcessingParametersChromatogram();

            //Enumerations
            GenerationDirection = EnumerationParentOrChild.ParentsOnly;
            IsPic = EnumerationIsPic.IsNotPic;
            //DatasetType = EnumerationDataset.Diabetes;


            //Mass Spec default parameters
            MSToleranceInPPM = 15;
            IsotopeLowPeakCuttoff = 0.005;
            //NumberOfPeaksToLeftForPenalty = 0;
            MSParameters = new ProcessingParametersMassSpectra();
            MSParameters.IsoParameters.NumberOfPeaksToLeftForPenalty = 0;
            MSParameters.IsoParameters.DivideFitScoreByNumberOfIons = false;//true is better for high mass

            EnumerationIsotopePenaltyMode mode = EnumerationIsotopePenaltyMode.None;
            MSParameters.IsoParameters = new IsotopeParameters();
            MSParameters.IsoParameters.NumberOfPeaksToLeftForPenalty = 0;
            MSParameters.IsoParameters.PenaltyMode = mode;
            MSParameters.IsoParameters.FractionalIntensityCuttoffForTheoretical = 0.1;
            MSParameters.IsoParameters.DeltaMassCalibrationMZ = 0;
            MSParameters.IsoParameters.DeltaMassCalibrationMono = 0;
            MSParameters.IsoParameters.ToMassCalibrate = false;
            MSParameters.IsoParameters.IsotopeProfileMode = EnumerationIsotopicProfileMode.H;
            MSParameters.IsoParameters.CuttOffArea = 0.75;//75% of the isotopic area needs to be detectable in the data

            //this is still initialze since default parameters are set in the lies above.  Still not set yet
            ParametersLeastSquares currentIsoParameters = (ParametersLeastSquares)MSParameters.IsoParameters.FitScoreParameters;
            currentIsoParameters.UseIsotopePeakCountCorrection = true;

            ChargeStateMin = 1;
            ChargeStateMax = 1000;

        }

        private static List<FragmentIQTarget> SetUpDefaultTargets()
        {
            List<FragmentIQTarget> myFragments = new List<FragmentIQTarget>();

            AddMonoSaccharide.Add(myFragments, MonosaccharideName.Hexose, 3, 12);
            AddMonoSaccharide.Add(myFragments, MonosaccharideName.Deoxyhexose, 0, 7);
            AddMonoSaccharide.Add(myFragments, MonosaccharideName.NAcetylhexosamine, 2, 8);
            AddMonoSaccharide.Add(myFragments, MonosaccharideName.NeuraminicAcid, 0, 9);

            return myFragments;
        }
    }
}
