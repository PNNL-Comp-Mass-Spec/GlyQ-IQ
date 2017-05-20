using GetPeaks_DLL.Go_Decon_Modules;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.Objects.Enumerations;
using DeconTools.Backend;
using GetPeaks_DLL.Functions;
using DeconTools.Backend.Core;
using OrbitrapPeakDetection;

namespace Parallel.THRASH
{
    public class ParametersTHRASH:ParalellParameters
    {
        /// <summary>
        /// Deisotoping parameters go here.  Goform is converted from DeconEngineV2
        /// </summary>
        public GoTransformParameters ThrashParameters { get; set; }

        /// <summary>
        /// Orbitrap Filtering Parameters
        /// </summary>
        public HammerThresholdParameters HammerParameters { get; set; }

        public int ScansToSum { get; set; }

        //limits the data set to this scan.  perhaps this is not needed
        public int MaxScanLimitation { get; set; }

        //true will just work on MS1
        public bool ProcessMsms { get; set; }

        //place to hold the scanssets
        public DeconTools.Backend.Core.ScanSetCollection EngineScanSetCollection { get; set; }

        public PeakDetectors PeakDetectionMethod { get; set; }

        //public double OrbitrapThresholdLevel { get; set; }
       
        
        /// <summary>
        /// decon tools peak detector
        /// </summary>
        public double MsPeakDetectorPeakToBackground { get; set; }

        public double SignalToNoiseRatio { get; set; }

        public Globals.PeakFitType PeakFitType { get; set; }

        public string DeconToolsParameterFile { get; set; }

        /// <summary>
        /// how many points go into each threshold group
        /// </summary>
        //public int PointsPerRange { get; set; }


        public ParametersTHRASH()
        {
            ThrashParameters = new GoTransformParameters();
            HammerParameters = new HammerThresholdParameters(); 
        }

        public ParametersTHRASH(string sqLiteFolderIn, string sqLiteNameIn, string deconToolsParameterFile, PeakDetectors peakDecector)
        {
            ThrashParameters = new GoTransformParameters(deconToolsParameterFile, true);
            HammerParameters = new HammerThresholdParameters(); 

            DeconToolsParameterFile = deconToolsParameterFile;

            OldDecon2LSParameters parametersDT = new OldDecon2LSParameters();
            parametersDT.Load(deconToolsParameterFile);
            ThrashParameters.DeconThrashParameters = parametersDT.HornTransformParameters;

            FileInforamation.OutputSQLFileName = sqLiteNameIn;
            FileInforamation.OutputPath = sqLiteFolderIn;
            ScansToSum = 0;
            MaxScanLimitation = 999999;
            ProcessMsms = false;
            PeakDetectionMethod = peakDecector;
            MsPeakDetectorPeakToBackground = 10;
            SignalToNoiseRatio = 10;
            PeakFitType = Globals.PeakFitType.QUADRATIC;
            
        }

        public ParametersTHRASH(string sqLiteFolderIn, string sqLiteNameIn, string datasetFileNameIn, string deconToolsParameterFile, int scansToSum, int computersToDivideOver, int coresPerComputer, int maxScanLimitation, bool processMSMS, PeakDetectors peakDecector, bool multithreadHardDrive, double fitScore, double peptideMinBackgroundRatio, bool overwriteParameters)
        {
            ThrashParameters = new GoTransformParameters(deconToolsParameterFile, true);
            HammerParameters = new HammerThresholdParameters(); 

            DeconToolsParameterFile = deconToolsParameterFile;
            OldDecon2LSParameters parametersDT = new OldDecon2LSParameters();
            parametersDT.Load(deconToolsParameterFile);
            ThrashParameters.DeconThrashParameters = parametersDT.HornTransformParameters;
            ScansToSum = parametersDT.HornTransformParameters.NumScansToSumOver;

            //manual overide of file values
            bool overRideFile = overwriteParameters;
            if (overRideFile)
            {
                ThrashParameters.DeconThrashParameters.MaxFit = fitScore; //default is 0.25   
                ThrashParameters.DeconThrashParameters.PeptideMinBackgroundRatio = peptideMinBackgroundRatio;//default is 4
                ScansToSum = scansToSum;
            }

            //this is a non decon copy
            ThrashParameters.Parameters = ConvertParameters(ThrashParameters.DeconThrashParameters);

            this.FileInforamation.OutputSQLFileName = sqLiteNameIn;
            this.FileInforamation.OutputPath = sqLiteFolderIn;
            this.FileInforamation.InputFileName = datasetFileNameIn;
            this.UniqueFileName = RemoveEnding.RAW(datasetFileNameIn) + " (0).RAW";
            this.ComputersToDivideOver = computersToDivideOver;
            this.CoresPerComputer = coresPerComputer;
            this.MultithreadedHardDriveMode = multithreadHardDrive;

            
            MaxScanLimitation = maxScanLimitation;
            ProcessMsms = processMSMS;

            PeakDetectionMethod = peakDecector;           
            PeakFitType = Globals.PeakFitType.QUADRATIC;           
        }

        public ParametersTHRASH(ParametersTHRASH parametersToCopy)
        {
            ThrashParameters = new GoTransformParameters();
            HammerParameters = new HammerThresholdParameters(); 
            ThrashParameters.DeconThrashParameters = new DeconToolsV2.HornTransform.clsHornTransformParameters();

            CopyDeconThrashParameters(ThrashParameters.DeconThrashParameters, parametersToCopy);

            this.ComputersToDivideOver = parametersToCopy.ComputersToDivideOver;
            this.CoresPerComputer = parametersToCopy.CoresPerComputer;
            this.EngineScanSetCollection = parametersToCopy.EngineScanSetCollection;
            this.FileInforamation = parametersToCopy.FileInforamation;
            this.UniqueFileName = parametersToCopy.UniqueFileName;
            this.MaxScanLimitation = parametersToCopy.MaxScanLimitation;
            this.MsPeakDetectorPeakToBackground = parametersToCopy.MsPeakDetectorPeakToBackground;
            this.PeakDetectionMethod = parametersToCopy.PeakDetectionMethod;
            this.PeakFitType = parametersToCopy.PeakFitType;
            this.ProcessMsms = parametersToCopy.ProcessMsms; 
            this.ScansToSum = parametersToCopy.ScansToSum;
            this.SignalToNoiseRatio = parametersToCopy.SignalToNoiseRatio;

            this.DeconToolsParameterFile = parametersToCopy.DeconToolsParameterFile;
            this.HammerParameters = parametersToCopy.HammerParameters;
            this.MultithreadedHardDriveMode = parametersToCopy.MultithreadedHardDriveMode;
            //TODO this needs to be checked.  Clone may be better
        }

        private void CopyDeconThrashParameters(DeconToolsV2.HornTransform.clsHornTransformParameters existingSpaceToFill, ParametersTHRASH parametersToCopy)
        {
            existingSpaceToFill.AbsolutePeptideIntensity = parametersToCopy.ThrashParameters.DeconThrashParameters.AbsolutePeptideIntensity;
            existingSpaceToFill.AveragineFormula = parametersToCopy.ThrashParameters.DeconThrashParameters.AveragineFormula;
            existingSpaceToFill.CCMass = parametersToCopy.ThrashParameters.DeconThrashParameters.CCMass;
            existingSpaceToFill.CheckAllPatternsAgainstCharge1 = parametersToCopy.ThrashParameters.DeconThrashParameters.CheckAllPatternsAgainstCharge1;
            existingSpaceToFill.CompleteFit = parametersToCopy.ThrashParameters.DeconThrashParameters.CompleteFit;
            existingSpaceToFill.DeleteIntensityThreshold = parametersToCopy.ThrashParameters.DeconThrashParameters.DeleteIntensityThreshold;
            existingSpaceToFill.DetectPeaksOnlyWithNoDeconvolution = parametersToCopy.ThrashParameters.DeconThrashParameters.DetectPeaksOnlyWithNoDeconvolution;
            existingSpaceToFill.ElementIsotopeComposition = parametersToCopy.ThrashParameters.DeconThrashParameters.ElementIsotopeComposition;
            existingSpaceToFill.ExportFileType = parametersToCopy.ThrashParameters.DeconThrashParameters.ExportFileType;
            existingSpaceToFill.IsActualMonoMZUsed = parametersToCopy.ThrashParameters.DeconThrashParameters.IsActualMonoMZUsed;
            existingSpaceToFill.IsotopeFitType = parametersToCopy.ThrashParameters.DeconThrashParameters.IsotopeFitType;
            existingSpaceToFill.LeftFitStringencyFactor = parametersToCopy.ThrashParameters.DeconThrashParameters.LeftFitStringencyFactor;
            existingSpaceToFill.MaxCharge = parametersToCopy.ThrashParameters.DeconThrashParameters.MaxCharge;
            existingSpaceToFill.MaxFit = parametersToCopy.ThrashParameters.DeconThrashParameters.MaxFit;
            existingSpaceToFill.MaxMW = parametersToCopy.ThrashParameters.DeconThrashParameters.MaxMW;
            existingSpaceToFill.MaxMZ = parametersToCopy.ThrashParameters.DeconThrashParameters.MaxMZ;
            existingSpaceToFill.MaxScan = parametersToCopy.ThrashParameters.DeconThrashParameters.MaxScan;
            existingSpaceToFill.MinIntensityForScore = parametersToCopy.ThrashParameters.DeconThrashParameters.MinIntensityForScore;
            existingSpaceToFill.MinMZ = parametersToCopy.ThrashParameters.DeconThrashParameters.MinMZ;
            existingSpaceToFill.MinS2N = parametersToCopy.ThrashParameters.DeconThrashParameters.MinS2N;
            existingSpaceToFill.MinScan = parametersToCopy.ThrashParameters.DeconThrashParameters.MinScan;
            existingSpaceToFill.NumFramesToSumOver = parametersToCopy.ThrashParameters.DeconThrashParameters.NumFramesToSumOver;
            existingSpaceToFill.NumPeaksForShoulder = parametersToCopy.ThrashParameters.DeconThrashParameters.NumPeaksForShoulder;
            existingSpaceToFill.NumPeaksUsedInAbundance = parametersToCopy.ThrashParameters.DeconThrashParameters.NumPeaksUsedInAbundance;
            existingSpaceToFill.NumScansToAdvance = parametersToCopy.ThrashParameters.DeconThrashParameters.NumScansToAdvance;
            existingSpaceToFill.NumScansToSumOver = parametersToCopy.ThrashParameters.DeconThrashParameters.NumScansToSumOver;
            existingSpaceToFill.NumZerosToFill = parametersToCopy.ThrashParameters.DeconThrashParameters.NumZerosToFill;
            existingSpaceToFill.O16O18Media = parametersToCopy.ThrashParameters.DeconThrashParameters.O16O18Media;
            existingSpaceToFill.PeptideMinBackgroundRatio = parametersToCopy.ThrashParameters.DeconThrashParameters.PeptideMinBackgroundRatio;
            existingSpaceToFill.ProcessMS = parametersToCopy.ThrashParameters.DeconThrashParameters.ProcessMS;
            existingSpaceToFill.ProcessMSMS = parametersToCopy.ThrashParameters.DeconThrashParameters.ProcessMSMS;
            existingSpaceToFill.ReplaceRAPIDScoreWithHornFitScore = parametersToCopy.ThrashParameters.DeconThrashParameters.ReplaceRAPIDScoreWithHornFitScore;
            existingSpaceToFill.RightFitStringencyFactor = parametersToCopy.ThrashParameters.DeconThrashParameters.RightFitStringencyFactor;
            existingSpaceToFill.SaturationThreshold = parametersToCopy.ThrashParameters.DeconThrashParameters.SaturationThreshold;
            existingSpaceToFill.ScanBasedWorkflowType = parametersToCopy.ThrashParameters.DeconThrashParameters.ScanBasedWorkflowType;
            existingSpaceToFill.SGNumLeft = parametersToCopy.ThrashParameters.DeconThrashParameters.SGNumLeft;
            existingSpaceToFill.SGNumRight = parametersToCopy.ThrashParameters.DeconThrashParameters.SGNumRight;
            existingSpaceToFill.SGOrder = parametersToCopy.ThrashParameters.DeconThrashParameters.SGOrder;
            existingSpaceToFill.SumSpectra = parametersToCopy.ThrashParameters.DeconThrashParameters.SumSpectra;
            existingSpaceToFill.SumSpectraAcrossFrameRange = parametersToCopy.ThrashParameters.DeconThrashParameters.SumSpectraAcrossFrameRange;
            existingSpaceToFill.SumSpectraAcrossScanRange = parametersToCopy.ThrashParameters.DeconThrashParameters.SumSpectraAcrossScanRange;
            existingSpaceToFill.TagFormula = parametersToCopy.ThrashParameters.DeconThrashParameters.TagFormula;
            existingSpaceToFill.ThrashOrNot = parametersToCopy.ThrashParameters.DeconThrashParameters.ThrashOrNot;
            existingSpaceToFill.UseAbsolutePeptideIntensity = parametersToCopy.ThrashParameters.DeconThrashParameters.UseAbsolutePeptideIntensity;
            existingSpaceToFill.UseMercuryCaching = parametersToCopy.ThrashParameters.DeconThrashParameters.UseMercuryCaching;
            existingSpaceToFill.UseMZRange = parametersToCopy.ThrashParameters.DeconThrashParameters.UseMZRange;
            existingSpaceToFill.UseRAPIDDeconvolution = parametersToCopy.ThrashParameters.DeconThrashParameters.UseRAPIDDeconvolution;
            existingSpaceToFill.UseSavitzkyGolaySmooth = parametersToCopy.ThrashParameters.DeconThrashParameters.UseSavitzkyGolaySmooth;
            existingSpaceToFill.UseScanRange = parametersToCopy.ThrashParameters.DeconThrashParameters.UseScanRange;
            existingSpaceToFill.ZeroFill = parametersToCopy.ThrashParameters.DeconThrashParameters.ZeroFill;
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
