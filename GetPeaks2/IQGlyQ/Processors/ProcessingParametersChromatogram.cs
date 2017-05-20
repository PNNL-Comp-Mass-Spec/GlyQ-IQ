using IQ.Backend.ProcessingTasks.LCGenerators;
using IQ.Backend.ProcessingTasks.PeakDetectors;
using IQ.Backend.ProcessingTasks.Smoothers;
using PNNLOmics.Algorithms.PeakDetection;
using IQGlyQ.Objects;

namespace IQGlyQ.Processors
{
    public class ProcessingParametersChromatogram : ProcessingParameters
    {
        //engines

        public PeakThresholder Engine_OmicsPeakThresholding { get; set; }

        public PeakCentroider Engine_OmicsPeakDetection { get; set; }

        public ChromPeakDetector Engine_ChromPeakDetector { get; set; }

        public PeakChromatogramGenerator Engine_PeakChromGenerator { get; set; }

        public PNNLOmics.Algorithms.SpectralProcessing.SavitzkyGolaySmoother Engine_Smoother { get; set; }

        public IQ.Backend.ProcessingTasks.Smoothers.SavitzkyGolaySmoother Engine_Smoother_Decon { get; set; }

        //parameters

        /// <summary>
        /// decon chrom peak detector parameters
        /// </summary>
        public ChromPeakDetectorParameters ParametersChromPeakDetector { get; set; }

        /// <summary>
        /// decon chrompeak generator parameters
        /// </summary>
        public ChromPeakGeneratorParameters ParametersChromGenerator { get; set; }

        /// <summary>
        /// for processing a broken up piece of the LC
        /// </summary>
        public EnumerationChromatogramProcessing ProcessLcSectionCorrelationObject { get; set; }

        /// <summary>
        /// for processing a EIC at the start
        /// </summary>
        public EnumerationChromatogramProcessing ProcessLcChromatogram { get; set; }

        /// <summary>
        /// moving average filtering.  3,5,7 etc
        /// </summary>
        public int MovingAveragePoints { get; set; }

        /// <summary>
        /// use IMS LC Generation
        /// </summary>
        public bool isIMS { get; set; }

        public ProcessingParametersChromatogram()
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
            Engine_ChromPeakDetector = new ChromPeakDetector(ParametersChromPeakDetector.ChromPeakDetectorPeakBR, ParametersChromPeakDetector.ChromPeakDetectorSignalToNoise);
            //Engine_Smoother = new SavitzkyGolaySmoother(ParametersSavitskyGolay.PointsForSmoothing, ParametersSavitskyGolay.PolynomialOrder, ParametersSavitskyGolay.AllowNegativeValues);
            Engine_Smoother = new PNNLOmics.Algorithms.SpectralProcessing.SavitzkyGolaySmoother(ParametersSavitskyGolay.PointsForSmoothing, ParametersSavitskyGolay.PolynomialOrder, ParametersSavitskyGolay.AllowNegativeValues);

            Engine_Smoother_Decon = new SavitzkyGolaySmoother(ParametersSavitskyGolay.PointsForSmoothing, ParametersSavitskyGolay.PolynomialOrder, ParametersSavitskyGolay.AllowNegativeValues);
            
            Engine_PeakChromGenerator = new PeakChromatogramGenerator(ParametersChromGenerator.ChromToleranceInPPM, ParametersChromGenerator.ChromeGeneratorMode, ParametersChromGenerator.IsotopeProfileType, ParametersChromGenerator.ErrorUnit);
            
            Engine_PeakChromGenerator.ChromWindowWidthForAlignedData = 1.0f;//1 will guarantee full chromatogram across all scans because x-1 or x+1 will be <0 or >1
            Engine_PeakChromGenerator.TopNPeaksLowerCutOff = 0.33;//TODO this needs to be pulled out if we use top N peaks mode which is no good anyways

        }

        private void InitializeParameters()
        {
            //parameters
            ParametersSavitskyGolay = new SavitskyGolaySmootherParameters();
            ParametersOmicsThreshold = new PeakThresholderParameters();
            ParametersOmicsPeakCentroid = new PeakCentroiderParameters();
            ParametersChromPeakDetector = new ChromPeakDetectorParameters();
            ParametersChromGenerator = new ChromPeakGeneratorParameters();
            MovingAveragePoints = 3;
            PointsPerShoulder = 2;
            ProcessLcSectionCorrelationObject = EnumerationChromatogramProcessing.SmoothSection;
            ProcessLcChromatogram = EnumerationChromatogramProcessing.ChromatogramLevel;
            CalibrationDaltonOffset = 0;
            isIMS = false;
        }

        
    }
}
