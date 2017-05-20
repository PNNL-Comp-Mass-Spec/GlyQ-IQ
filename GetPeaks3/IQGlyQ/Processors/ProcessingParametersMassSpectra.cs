using IQGlyQ.Objects;
using IQGlyQ.Objects.EverythingIsotope;
using IQGlyQ.Tasks.FitScores;
using IQ_X64.Backend.ProcessingTasks;
using IQ_X64.Backend.ProcessingTasks.FitScoreCalculators;
using IQ_X64.Backend.ProcessingTasks.MSGenerators;
using IQ_X64.Backend.ProcessingTasks.ResultValidators;
using IQ_X64.Backend.ProcessingTasks.Smoothers;
using IQ_X64.Backend.ProcessingTasks.TargetedFeatureFinders;
using IQ_X64.Backend.ProcessingTasks.TheorFeatureGenerator;
using PNNLOmics.Algorithms.PeakDetection;
using Run64.Backend;

namespace IQGlyQ.Processors
{
    public class ProcessingParametersMassSpectra: ProcessingParameters
    {
        public PeakThresholder Engine_OmicsPeakThresholding { get; set; }

        public PeakCentroider Engine_OmicsPeakDetection { get; set; }

        public PNNLOmics.Algorithms.SpectralProcessing.SavitzkyGolaySmoother Engine_Smoother { get; set; }

        public IQ_X64.Backend.ProcessingTasks.Smoothers.SavitzkyGolaySmoother Engine_Smoother_Decon { get; set; }

        public IterativeTFF Engine_msfeatureFinder { get; set; }

        public MSGenerator Engine_msGenerator { get; set; }

        public InterferenceScorer Engine_InterferenceScorer { get; set; }

        //public  IQ.Backend.Core.TaskIQ Engine_FitScoreCalculator { get; set; }
        public IFitScoring Engine_FitScoreCalculator { get; set; }

        //parameters

        /// <summary>
        /// mass accuracy in ppm
        /// </summary>
        double MSToleranceInPPM = 15;

        /// <summary>
        /// low cuttoff for isotope profile generation
        /// </summary>
        double IsotopeLowPeakCuttoff = 0.005;

    //    /// <summary>
    //    /// 1Da correction number of peaks to the left to zero out
    //    /// </summary>
    //    int NumberOfPeaksToLeftForPenalty = 0;

        /// <summary>
        /// parameters for feature finder
        /// </summary>
        public IterativeTFFParameters IterativeTFFParameters;

        /// <summary>
        /// parameters for ms generator
        /// </summary>
        public MSGeneratorParameters MsGeneratorParameters;

        /// <summary>
        /// parameters for fitscore and 1Da errors
        /// </summary>
        public IsotopeParameters IsoParameters;

        /// <summary>
        /// used to create complex isotope paterns inside the code
        /// </summary>
        public ParametersSimpleIsotope ParametersIsotopeGeneration { get; set; }

        public ProcessingParametersMassSpectra()
        {
            InitializeParameters();
            InitializeEngines();
        }

        public void InitializeEngines()
        {
            //omics
            Engine_OmicsPeakThresholding = new PeakThresholder(ParametersOmicsThreshold);//the threshold noise level is based on the local minima.  they are then averaged and sigma is calculated 
            Engine_OmicsPeakDetection = new PeakCentroider(ParametersOmicsPeakCentroid);

            //decon
            //Engine_Smoother = new SavitzkyGolaySmoother(ParametersSavitskyGolay.PointsForSmoothing, ParametersSavitskyGolay.PolynomialOrder, ParametersSavitskyGolay.AllowNegativeValues);
            Engine_Smoother = new PNNLOmics.Algorithms.SpectralProcessing.SavitzkyGolaySmoother(ParametersSavitskyGolay.PointsForSmoothing, ParametersSavitskyGolay.PolynomialOrder, ParametersSavitskyGolay.AllowNegativeValues);
            
            Engine_Smoother_Decon = new SavitzkyGolaySmoother(ParametersSavitskyGolay.PointsForSmoothing, ParametersSavitskyGolay.PolynomialOrder, ParametersSavitskyGolay.AllowNegativeValues);

            Engine_msfeatureFinder = new IterativeTFF(new IterativeTFFParameters());

            Engine_msGenerator = MSGeneratorFactory.CreateMSGenerator(MsGeneratorParameters.MsFileType);

            Engine_InterferenceScorer = new InterferenceScorer();

            //initialize fitScore here.  IsoParameters.FitScoreParameters is not set up yet the first time through but is the seccond
            //the seccond time through is the singleton score calculator
            Engine_FitScoreCalculator = FitScoreFactory.Build(IsoParameters.FitScoreParameters);            
        }

        private void InitializeParameters()
        {
            //parameters
            ParametersSavitskyGolay = new SavitskyGolaySmootherParameters();
            ParametersOmicsThreshold = new PeakThresholderParameters();
            ParametersOmicsPeakCentroid = new PeakCentroiderParameters();
            IterativeTFFParameters = new IterativeTFFParameters();
            PointsPerShoulder = 2;
            CalibrationDaltonOffset = 0;
            MsGeneratorParameters = new MSGeneratorParameters();
            IsoParameters = new IsotopeParameters();
            
            //perhaps use some sort of default here.  this is here for a place holder although it is overwrittern later upon instance construction
            ParametersIsotopeGeneration = new ParametersSimpleIsotope(new JoshTheorFeatureGenerator(Globals.LabellingType.NONE, 0.1));
        }
    }
}
