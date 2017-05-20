using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.ProcessingTasks;
using DeconTools.Backend.ProcessingTasks.FitScoreCalculators;
using DeconTools.Backend.ProcessingTasks.MSGenerators;
using DeconTools.Backend.ProcessingTasks.ResultValidators;
using DeconTools.Backend.ProcessingTasks.Smoothers;
using DeconTools.Backend.ProcessingTasks.TargetedFeatureFinders;
using DeconTools.Backend.ProcessingTasks.TheorFeatureGenerator;
using IQGlyQ.Enumerations;
using IQGlyQ.Objects;
using IQGlyQ.Tasks;
using PNNLOmics.Algorithms.PeakDetection;

namespace IQGlyQ.Processors
{
    public class ProcessingParametersMassSpectra: ProcessingParameters
    {
        public PeakThresholder Engine_OmicsPeakThresholding { get; set; }

        public PeakCentroider Engine_OmicsPeakDetection { get; set; }

        public PNNLOmics.Algorithms.SpectralProcessing.SavitzkyGolaySmoother Engine_Smoother { get; set; }

        public SavitzkyGolaySmoother Engine_Smoother_Decon { get; set; }

        public IterativeTFF Engine_msfeatureFinder { get; set; }

        public MSGenerator Engine_msGenerator { get; set; }

        public InterferenceScorer Engine_InterferenceScorer { get; set; }

        public DeconTools.Backend.Core.Task Engine_FitScoreCalculator { get; set; }

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

            //EnumerationIsotopePenaltyMode mode = EnumerationIsotopePenaltyMode.None;
            //FragmentedTargetedWorkflowParametersIQ tempParameters = this;
            //ITheorFeatureGenerator theoryIsotopeGenerator = new JoshTheorFeatureGenerator();

            //Engine_FitScoreCalculator = new IsotopicPeakFitScoreCalculator(FitScoreParameters.NumberOfPeaksToLeftForPenalty);baic IQ
            Engine_FitScoreCalculator = new IsotopicPeakFitScoreCalculatorSK(IsoParameters);//GlyQ
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
            IsoParameters =new IsotopeParameters();
        }



    }
}
