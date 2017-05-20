using DeconToolsV2.Peaks;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.Objects.Enumerations;
using DeconTools.Backend;
using GetPeaks_DLL.Functions;
using DeconTools.Backend.Core;
using GetPeaks_DLL.Parallel;

namespace GetPeaks_DLL.Go_Decon_Modules
{
    public class ParametersTHRASH:ParalellParameters
    {
       

        /// <summary>
        /// Orbitrap Filtering Parameters
        /// </summary>
        //public HammerThresholdParameters HammerParameters { get; set; }

        public int ScansToSum { get; set; }

        //limits the data set to this scan for the scansets.  perhaps this is not needed
        public int MaxScanLimitation { get; set; }

        //true will just work on MS1
        public bool ProcessMsms { get; set; }


        //simplified
        public string DeconToolsParameterFileName { get; set; }

        /// <summary>
        /// All peak parameters go here
        /// </summary>
        public ParametersPeakDetection PeakDetectionMultiParameters { get; set; }

        /// <summary>
        /// Deisotoping parameters go here.  Goform is converted from DeconEngineV2
        /// this is loaded from the file
        /// </summary>
        public GoTransformParameters DeisotopingParametersThrash { get; set; }

        /// <summary>
        /// place to hold the scanssets.  This may be better in side an engine
        /// </summary>
        public DeconTools.Backend.Core.ScanSetCollection EngineScanSetCollection { get; set; }


        public ParametersTHRASH()
        {
            PeakDetectionMultiParameters = new ParametersPeakDetection();
            DeisotopingParametersThrash = new GoTransformParameters();
        }

        public ParametersTHRASH(string sqLiteFolderIn, string sqLiteNameIn, string deconToolsParameterFile, PeakDetectors peakDecector)
        {
            DeisotopingParametersThrash = new GoTransformParameters(deconToolsParameterFile, true);
            PeakDetectionMultiParameters = new ParametersPeakDetection();

            DeconToolsParameterFileName = deconToolsParameterFile;

            OldDecon2LSParameters parametersDT = new OldDecon2LSParameters();
            parametersDT.Load(deconToolsParameterFile);
            DeisotopingParametersThrash.DeconThrashParameters = parametersDT.HornTransformParameters;

            FileInforamation.OutputSQLFileName = sqLiteNameIn;
            FileInforamation.OutputPath = sqLiteFolderIn;
            ScansToSum = 0;
            MaxScanLimitation = 999999;
            ProcessMsms = false;
            PeakDetectionMultiParameters.PeakDetectionMethod = peakDecector;
        }

        //public ParametersTHRASH(string sqLiteFolderIn, string sqLiteNameIn, string datasetFileNameIn, string deconToolsParameterFile, int scansToSum, int computersToDivideOver, int coresPerComputer, int maxScanLimitation, bool processMSMS, PeakDetectors peakDecector, bool multithreadHardDrive, double fitScore, double peptideMinBackgroundRatio, bool overwriteParameters)
        public ParametersTHRASH(
            string sqLiteFolderIn,
            string sqLiteNameIn,
            string datasetFileNameIn,
            string deconToolsParameterFile,
            int scansToSum,
            int maxScanLimitation,
            bool processMSMS,
            bool multithreadHardDrive,
            bool useParameterFileValues,
            ParametersPeakDetection peakParameters,
            double fitScore,
            double peptideMinBackgroundRatio)


        {
            const bool loadFromFile = true;//this is a good idea for the deisootping parameters

            //setup THRASH and Decon
            DeconToolsParameterFileName = deconToolsParameterFile;
            DeisotopingParametersThrash = new GoTransformParameters(deconToolsParameterFile, loadFromFile);
            //load parameters from file
            OldDecon2LSParameters parametersFromFile = new OldDecon2LSParameters();
            parametersFromFile.Load(deconToolsParameterFile);
            
            //update parameters from code inputs
            DeisotopingParametersThrash.DeconThrashParameters = parametersFromFile.HornTransformParameters;
            DeisotopingParametersThrash.DeconThrashParameters.MaxFit = fitScore;
            DeisotopingParametersThrash.DeconThrashParameters.PeptideMinBackgroundRatio = peptideMinBackgroundRatio;
            //Set up rest

            PeakDetectionMultiParameters = new ParametersPeakDetection();
            PeakDetectionMultiParameters = peakParameters;
            
            this.FileInforamation.OutputSQLFileName = sqLiteNameIn;
            this.FileInforamation.OutputPath = sqLiteFolderIn;
            this.FileInforamation.InputFileName = datasetFileNameIn;
            this.UniqueFileName = RemoveEnding.RAW(datasetFileNameIn) + " (0).RAW";
            this.ScansToSum = scansToSum;
            this.MultithreadedHardDriveMode = multithreadHardDrive;
            this.MaxScanLimitation = maxScanLimitation;
            this.ProcessMsms = processMSMS;
            

            //manual overide of file values
            bool overRideFile = useParameterFileValues;
            if (overRideFile==true)
            {
                PeakDetectionMultiParameters.DeconToolsPeakDetectionDeconVersion.IsDataThresholded = parametersFromFile.PeakProcessorParameters.ThresholdedData;
                PeakDetectionMultiParameters.DeconToolsPeakDetectionDeconVersion.PeakToBackgroundRatio = parametersFromFile.PeakProcessorParameters.PeakBackgroundRatio;
                PeakDetectionMultiParameters.DeconToolsPeakDetectionDeconVersion.SignalToNoiseThreshold = parametersFromFile.PeakProcessorParameters.SignalToNoiseThreshold;

                PeakDetectionMultiParameters.DeconToolsPeakDetection.MsPeakDetectorPeakToBackground = PeakDetectionMultiParameters.DeconToolsPeakDetectionDeconVersion.PeakToBackgroundRatio;
                PeakDetectionMultiParameters.DeconToolsPeakDetection.SignalToNoiseRatio = PeakDetectionMultiParameters.DeconToolsPeakDetectionDeconVersion.SignalToNoiseThreshold;

                switch(parametersFromFile.PeakProcessorParameters.PeakFitType)
                {
                    case PEAK_FIT_TYPE.APEX:
                        {
                            PeakDetectionMultiParameters.DeconToolsPeakDetectionDeconVersion.PeakFitType = Globals.PeakFitType.APEX;
                            PeakDetectionMultiParameters.DeconToolsPeakDetection.PeakFitType = Globals.PeakFitType.APEX;
                        }
                        break;
                        case PEAK_FIT_TYPE.QUADRATIC:
                        {
                            PeakDetectionMultiParameters.DeconToolsPeakDetectionDeconVersion.PeakFitType = Globals.PeakFitType.QUADRATIC;
                            PeakDetectionMultiParameters.DeconToolsPeakDetection.PeakFitType = Globals.PeakFitType.QUADRATIC;
                        }
                        break;
                    case PEAK_FIT_TYPE.LORENTZIAN:
                        {
                            PeakDetectionMultiParameters.DeconToolsPeakDetectionDeconVersion.PeakFitType = Globals.PeakFitType.LORENTZIAN;
                            PeakDetectionMultiParameters.DeconToolsPeakDetection.PeakFitType = Globals.PeakFitType.LORENTZIAN;
                        }
                        break;
                    default:
                        {
                            PeakDetectionMultiParameters.DeconToolsPeakDetectionDeconVersion.PeakFitType = Globals.PeakFitType.Undefined;
                            PeakDetectionMultiParameters.DeconToolsPeakDetection.PeakFitType = Globals.PeakFitType.Undefined;
                        }
                        break;
                }
                

                DeisotopingParametersThrash.DeconThrashParameters.MaxFit = parametersFromFile.HornTransformParameters.MaxFit; //default is 0.25
                
                //this needs to be fixed
                PeakDetectionMultiParameters.ScansToSum = parametersFromFile.HornTransformParameters.NumScansToSumOver;
                DeisotopingParametersThrash.DeconThrashParameters.MaxFit = parametersFromFile.HornTransformParameters.MaxFit;
                DeisotopingParametersThrash.DeconThrashParameters.PeptideMinBackgroundRatio = parametersFromFile.HornTransformParameters.PeptideMinBackgroundRatio;
            }

            

            //this is a non decon copy
            DeisotopingParametersThrash.Parameters = ConvertParameters(DeisotopingParametersThrash.DeconThrashParameters);

           

                        
        }

        public ParametersTHRASH(ParametersTHRASH parametersToCopy)
        {
            DeisotopingParametersThrash = new GoTransformParameters();
            PeakDetectionMultiParameters = new ParametersPeakDetection();
            DeisotopingParametersThrash.DeconThrashParameters = new DeconToolsV2.HornTransform.clsHornTransformParameters();

            CopyDeconThrashParameters(DeisotopingParametersThrash.DeconThrashParameters, parametersToCopy);

            this.ComputersToDivideOver = parametersToCopy.ComputersToDivideOver;
            this.CoresPerComputer = parametersToCopy.CoresPerComputer;
            this.EngineScanSetCollection = parametersToCopy.EngineScanSetCollection;
            this.FileInforamation = parametersToCopy.FileInforamation;
            this.UniqueFileName = parametersToCopy.UniqueFileName;
            this.MaxScanLimitation = parametersToCopy.MaxScanLimitation;    
            this.PeakDetectionMultiParameters.PeakDetectionMethod = parametersToCopy.PeakDetectionMultiParameters.PeakDetectionMethod;
            this.ProcessMsms = parametersToCopy.ProcessMsms; 
            this.ScansToSum = parametersToCopy.ScansToSum;
            this.DeconToolsParameterFileName = parametersToCopy.DeconToolsParameterFileName;
            this.PeakDetectionMultiParameters = parametersToCopy.PeakDetectionMultiParameters;
            this.MultithreadedHardDriveMode = parametersToCopy.MultithreadedHardDriveMode;
            //TODO this needs to be checked.  Clone may be better
        }

        private void CopyDeconThrashParameters(DeconToolsV2.HornTransform.clsHornTransformParameters existingSpaceToFill, ParametersTHRASH parametersToCopy)
        {
            existingSpaceToFill.AbsolutePeptideIntensity = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.AbsolutePeptideIntensity;
            existingSpaceToFill.AveragineFormula = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.AveragineFormula;
            existingSpaceToFill.CCMass = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.CCMass;
            existingSpaceToFill.CheckAllPatternsAgainstCharge1 = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.CheckAllPatternsAgainstCharge1;
            existingSpaceToFill.CompleteFit = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.CompleteFit;
            existingSpaceToFill.DeleteIntensityThreshold = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.DeleteIntensityThreshold;
            existingSpaceToFill.DetectPeaksOnlyWithNoDeconvolution = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.DetectPeaksOnlyWithNoDeconvolution;
            existingSpaceToFill.ElementIsotopeComposition = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.ElementIsotopeComposition;
            existingSpaceToFill.ExportFileType = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.ExportFileType;
            existingSpaceToFill.IsActualMonoMZUsed = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.IsActualMonoMZUsed;
            existingSpaceToFill.IsotopeFitType = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.IsotopeFitType;
            existingSpaceToFill.LeftFitStringencyFactor = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.LeftFitStringencyFactor;
            existingSpaceToFill.MaxCharge = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.MaxCharge;
            existingSpaceToFill.MaxFit = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.MaxFit;
            existingSpaceToFill.MaxMW = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.MaxMW;
            existingSpaceToFill.MaxMZ = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.MaxMZ;
            existingSpaceToFill.MaxScan = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.MaxScan;
            existingSpaceToFill.MinIntensityForScore = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.MinIntensityForScore;
            existingSpaceToFill.MinMZ = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.MinMZ;
            existingSpaceToFill.MinS2N = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.MinS2N;
            existingSpaceToFill.MinScan = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.MinScan;
            existingSpaceToFill.NumFramesToSumOver = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.NumFramesToSumOver;
            existingSpaceToFill.NumPeaksForShoulder = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.NumPeaksForShoulder;
            existingSpaceToFill.NumPeaksUsedInAbundance = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.NumPeaksUsedInAbundance;
            existingSpaceToFill.NumScansToAdvance = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.NumScansToAdvance;
            existingSpaceToFill.NumScansToSumOver = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.NumScansToSumOver;
            existingSpaceToFill.NumZerosToFill = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.NumZerosToFill;
            existingSpaceToFill.O16O18Media = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.O16O18Media;
            existingSpaceToFill.PeptideMinBackgroundRatio = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.PeptideMinBackgroundRatio;
            existingSpaceToFill.ProcessMS = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.ProcessMS;
            existingSpaceToFill.ProcessMSMS = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.ProcessMSMS;
            existingSpaceToFill.ReplaceRAPIDScoreWithHornFitScore = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.ReplaceRAPIDScoreWithHornFitScore;
            existingSpaceToFill.RightFitStringencyFactor = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.RightFitStringencyFactor;
            existingSpaceToFill.SaturationThreshold = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.SaturationThreshold;
            existingSpaceToFill.ScanBasedWorkflowType = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.ScanBasedWorkflowType;
            existingSpaceToFill.SGNumLeft = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.SGNumLeft;
            existingSpaceToFill.SGNumRight = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.SGNumRight;
            existingSpaceToFill.SGOrder = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.SGOrder;
            existingSpaceToFill.SumSpectra = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.SumSpectra;
            existingSpaceToFill.SumSpectraAcrossFrameRange = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.SumSpectraAcrossFrameRange;
            existingSpaceToFill.SumSpectraAcrossScanRange = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.SumSpectraAcrossScanRange;
            existingSpaceToFill.TagFormula = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.TagFormula;
            existingSpaceToFill.ThrashOrNot = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.ThrashOrNot;
            existingSpaceToFill.UseAbsolutePeptideIntensity = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.UseAbsolutePeptideIntensity;
            existingSpaceToFill.UseMercuryCaching = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.UseMercuryCaching;
            existingSpaceToFill.UseMZRange = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.UseMZRange;
            existingSpaceToFill.UseRAPIDDeconvolution = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.UseRAPIDDeconvolution;
            existingSpaceToFill.UseSavitzkyGolaySmooth = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.UseSavitzkyGolaySmooth;
            existingSpaceToFill.UseScanRange = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.UseScanRange;
            existingSpaceToFill.ZeroFill = parametersToCopy.DeisotopingParametersThrash.DeconThrashParameters.ZeroFill;
        }

        private static HornTransformParameters ConvertParameters(DeconToolsV2.HornTransform.clsHornTransformParameters parametersToCopy)
        {
            HornTransformParameters existingSpaceToFill = new HornTransformParameters();
            existingSpaceToFill.AbsolutePeptideIntensity = parametersToCopy.AbsolutePeptideIntensity;
            existingSpaceToFill.AveragineFormula = parametersToCopy.AveragineFormula;
            existingSpaceToFill.CCMass = parametersToCopy.CCMass;
            existingSpaceToFill.CheckAllPatternsAgainstCharge1 = parametersToCopy.CheckAllPatternsAgainstCharge1;
            existingSpaceToFill.CompleteFit = parametersToCopy.CompleteFit;
            existingSpaceToFill.DeleteIntensityThreshold = parametersToCopy.DeleteIntensityThreshold;
            //existingSpaceToFill.DetectPeaksOnlyWithNoDeconvolution = parametersToCopy.DetectPeaksOnlyWithNoDeconvolution;
            //existingSpaceToFill.ElementIsotopeComposition = parametersToCopy.ElementIsotopeComposition;
            //existingSpaceToFill.ExportFileType = parametersToCopy.ExportFileType;
            existingSpaceToFill.IsActualMonoMZUsed = parametersToCopy.IsActualMonoMZUsed;
            existingSpaceToFill.IsotopeFitType = parametersToCopy.IsotopeFitType;
            existingSpaceToFill.LeftFitStringencyFactor = parametersToCopy.LeftFitStringencyFactor;
            existingSpaceToFill.MaxCharge = parametersToCopy.MaxCharge;
            existingSpaceToFill.MaxFit = parametersToCopy.MaxFit;
            existingSpaceToFill.MaxMW = parametersToCopy.MaxMW;
            existingSpaceToFill.MaxMZ = parametersToCopy.MaxMZ;
            existingSpaceToFill.MaxScan = parametersToCopy.MaxScan;
            existingSpaceToFill.MinIntensityForScore = parametersToCopy.MinIntensityForScore;
            existingSpaceToFill.MinMZ = parametersToCopy.MinMZ;
            //existingSpaceToFill.MinS2N = parametersToCopy.MinS2N;
            existingSpaceToFill.MinScan = parametersToCopy.MinScan;
            //existingSpaceToFill.NumFramesToSumOver = parametersToCopy.NumFramesToSumOver;
            existingSpaceToFill.NumPeaksForShoulder = parametersToCopy.NumPeaksForShoulder;
            existingSpaceToFill.NumPeaksUsedInAbundance = parametersToCopy.NumPeaksUsedInAbundance;
            existingSpaceToFill.NumScansToAdvance = parametersToCopy.NumScansToAdvance;
            existingSpaceToFill.NumScansToSumOver = parametersToCopy.NumScansToSumOver;
            //existingSpaceToFill.NumZerosToFill = parametersToCopy.NumZerosToFill;
            existingSpaceToFill.O16O18Media = parametersToCopy.O16O18Media;
            existingSpaceToFill.PeptideMinBackgroundRatio = parametersToCopy.PeptideMinBackgroundRatio;
            //existingSpaceToFill.ProcessMS = parametersToCopy.ProcessMS;
            existingSpaceToFill.ProcessMSMS = parametersToCopy.ProcessMSMS;
            //existingSpaceToFill.ReplaceRAPIDScoreWithHornFitScore = parametersToCopy.ReplaceRAPIDScoreWithHornFitScore;
            existingSpaceToFill.RightFitStringencyFactor = parametersToCopy.RightFitStringencyFactor;
            //existingSpaceToFill.SaturationThreshold = parametersToCopy.SaturationThreshold;
            //existingSpaceToFill.ScanBasedWorkflowType = parametersToCopy.ScanBasedWorkflowType;
            //existingSpaceToFill.SGNumLeft = parametersToCopy.SGNumLeft;
            //existingSpaceToFill.SGNumRight = parametersToCopy.SGNumRight;
            //existingSpaceToFill.SGOrder = parametersToCopy.SGOrder;
            existingSpaceToFill.SumSpectra = parametersToCopy.SumSpectra;
            existingSpaceToFill.SumSpectraAcrossFrameRange = parametersToCopy.SumSpectraAcrossFrameRange;
            existingSpaceToFill.SumSpectraAcrossScanRange = parametersToCopy.SumSpectraAcrossScanRange;
            existingSpaceToFill.TagFormula = parametersToCopy.TagFormula;
            existingSpaceToFill.ThrashOrNot = parametersToCopy.ThrashOrNot;
            existingSpaceToFill.UseAbsolutePeptideIntensity = parametersToCopy.UseAbsolutePeptideIntensity;
            existingSpaceToFill.UseMercuryCaching = parametersToCopy.UseMercuryCaching;
            existingSpaceToFill.UseMZRange = parametersToCopy.UseMZRange;
            //existingSpaceToFill.UseRAPIDDeconvolution = parametersToCopy.UseRAPIDDeconvolution;
            //existingSpaceToFill.UseSavitzkyGolaySmooth = parametersToCopy.UseSavitzkyGolaySmooth;
            existingSpaceToFill.UseScanRange = parametersToCopy.UseScanRange;
            //existingSpaceToFill.ZeroFill = parametersToCopy.ZeroFill;

            return existingSpaceToFill;
        }
    }
}
