using System;
using System.Collections.Generic;
using IQ.Backend.Core;
using IQ.Backend.ProcessingTasks.ChromatogramProcessing;
using IQ.Backend.ProcessingTasks.FitScoreCalculators;
using IQ.Backend.ProcessingTasks.LCGenerators;
using IQ.Backend.ProcessingTasks.PeakDetectors;
using IQ.Backend.ProcessingTasks.ResultValidators;
using IQ.Backend.ProcessingTasks.Smoothers;
using IQ.Backend.ProcessingTasks.TargetedFeatureFinders;
using IQ.Backend.ProcessingTasks.TheorFeatureGenerator;
using IQ.Workflows;
using IQ.Workflows.Core;
using IQ.Workflows.Core.ChromPeakSelection;
using IQ.Workflows.WorkFlowParameters;
using IQ.Workflows.WorkFlowPile;
using IQGlyQ;
using IQGlyQ.Enumerations;
using IQGlyQ.Objects;
using IQGlyQ.Objects.EverythingIsotope;
using IQGlyQ.Processors;
using PNNLOmics.Data;
using PNNLOmics.Data.Peaks;
using Run32.Backend;
using Run32.Backend.Core;
using Run32.Backend.Core.Results;
using Run32.Backend.Data;
using Run32.Utilities;

//thjis needs to ovverride targeted workflows not workflow base

namespace Sipper
{
    public class GlyQIQTargetedWorkflow : TargetedWorkflow
    {
        #region Properties

        //for correlation values
        private SipperQuantifier _quantifier;


        public IList<ChromPeak> ChromPeaksDetected { get; set; }

        public ChromPeak ChromPeakSelected { get; set; }

        public Run32.Backend.Data.XYData MassSpectrumXYData { get; set; }

        public Run32.Backend.Data.XYData ChromatogramXYData { get; set; }

        public bool Success { get; set; }

        public bool IsWorkflowInitialized { get; set; }

        public string WorkflowStatusMessage { get; set; }

        public string Name
        {
            get { return ToString(); }
        }

        /// <summary>
        /// For trimming the final mass spectrum. A value of '2' means 
        /// that the mass spectrum will be trimmed -2 to the given m/z value.
        /// </summary>
        public double MsLeftTrimAmount { get; set; }

        /// <summary>
        /// For trimming the final mass spectrum. A value of '10' means 
        /// that the mass spectrum will be trimmed +10 to the given m/z value.
        /// </summary>
        public double MsRightTrimAmount { get; set; }

        #endregion

        private TargetedWorkflowParameters _workflowParameters;
        //private ITheorFeatureGenerator _theorFeatureGen;
        //private PeakChromatogramGenerator _chromGen;
        //private SavitzkyGolaySmoother _chromSmoother;
        //private ChromPeakDetector _chromPeakDetector;
        //private ChromPeakSelectorBase _chromPeakSelector;
        private IterativeTFF _msfeatureFinder;
        //private IsotopicProfileFitScoreCalculator _fitScoreCalc;
        private TheoreticalIsotopicProfileWrapper TheoryIsotopeGenerator { get; set; }
        private IGenerateIsotopeProfile TheorFeatureGenV2 { get; set; }
        private TaskIQ _fitScoreCalc;
        private ResultValidatorTask _resultValidator;
        private ChromatogramCorrelatorBase _chromatogramCorrelator;
       
        private IterativeTFFParameters _iterativeTFFParameters = new IterativeTFFParameters();

        public Run32.Backend.Data.XYData ChromCorrelationRSquaredVals { get; set; }

        public Run32.Backend.Data.XYData RatioVals { get; set; }

        public IsotopicProfile SubtractedIso { get; set; }

        private EnumerationIsotopicProfileMode IsotopeProfileMode { get; set; }

        private ParametersIsoCalibration IsoCalParameters { get; set; }
        private ParametersIsoShift IsoShiftParameters { get; set; }

        public FragmentedTargetedPeakProcessingParameters ProcessingParameters { get; set; }

        string PathFragmentParameters { get; set; }


        #region Constructors

        public GlyQIQTargetedWorkflow(Run run, TargetedWorkflowParameters parameters, string pathFragmentParameters)
            : base(run, parameters)
        {
            Run = run;
            WorkflowParameters = parameters;

            MsLeftTrimAmount = 1e10;     // set this high so, by default, nothing is trimmed
            MsRightTrimAmount = 1e10;  // set this high so, by default, nothing is trimmed
            PathFragmentParameters = pathFragmentParameters;
        }

        public GlyQIQTargetedWorkflow(TargetedWorkflowParameters parameters, string pathFragmentParameters)
            : this(null, parameters, pathFragmentParameters)
        {
        }

        #endregion


       

        private void ValidateParameters()
        {
            Check.Require(_workflowParameters != null, "Cannot validate workflow parameters. Parameters are null");

            bool pointsInSmoothIsEvenNumber = (_workflowParameters.ChromSmootherNumPointsInSmooth % 2 == 0);
            if (pointsInSmoothIsEvenNumber)
            {
                throw new ArgumentOutOfRangeException("Points in chrom smoother is an even number, but must be an odd number.");
            }
        }

        private void updateChromDataXYValues(Run32.Backend.Data.XYData xydata)
        {
            if (xydata == null)
            {
                //ResetStoredXYData(ChromatogramXYData);
                return;
            }
            else
            {
                this.ChromatogramXYData.Xvalues = xydata.Xvalues;
                this.ChromatogramXYData.Yvalues = xydata.Yvalues;
            }

        }

        private void updateMassSpectrumXYValues(Run32.Backend.Data.XYData xydata)
        {
            if (xydata == null)
            {
                //ResetStoredXYData(ChromatogramXYData);
                return;
            }
            else
            {
                this.MassSpectrumXYData.Xvalues = xydata.Xvalues;
                this.MassSpectrumXYData.Yvalues = xydata.Yvalues;
            }
        }


        protected override void DoPreInitialization()
        {
            //for correlation plot
            _quantifier = new SipperQuantifier();
        }

        protected override void DoPostInitialization() { }

        protected override void DoMainInitialization()
        {
            ValidateParameters();

            _theorFeatureGen = new JoshTheorFeatureGenerator(Globals.LabellingType.NONE, 0.005);
            _chromGen = new PeakChromatogramGenerator(_workflowParameters.ChromGenTolerance, _workflowParameters.ChromGeneratorMode, Globals.IsotopicProfileType.UNLABELLED, _workflowParameters.ChromGenToleranceUnit)
                            {
                                TopNPeaksLowerCutOff = 0.333,
                                ChromWindowWidthForAlignedData = (float) _workflowParameters.ChromNETTolerance*2,
                                ChromWindowWidthForNonAlignedData = (float) _workflowParameters.ChromNETTolerance*2
                            };

            //only

            const bool allowNegativeValues = false;
            _chromSmoother = new SavitzkyGolaySmoother(_workflowParameters.ChromSmootherNumPointsInSmooth, 2, allowNegativeValues);
            _chromPeakDetector = new ChromPeakDetectorMedianBased(_workflowParameters.ChromPeakDetectorPeakBR, _workflowParameters.ChromPeakDetectorSigNoise);
            


            _chromPeakSelector = CreateChromPeakSelector(_workflowParameters);

            _iterativeTFFParameters = new IterativeTFFParameters();
            _iterativeTFFParameters.ToleranceInPPM = _workflowParameters.MSToleranceInPPM;

            _msfeatureFinder = new IterativeTFF(_iterativeTFFParameters);
            _fitScoreCalc = new IsotopicProfileFitScoreCalculator();
            _resultValidator = new ResultValidatorTask();
            _chromatogramCorrelator = new ChromatogramCorrelator(_workflowParameters.ChromSmootherNumPointsInSmooth, 0.01, _workflowParameters.ChromGenTolerance);

            ChromatogramXYData = new Run32.Backend.Data.XYData();
            MassSpectrumXYData = new Run32.Backend.Data.XYData();
            ChromPeaksDetected = new List<ChromPeak>();




            IsotopeParameters isoParameters = new IsotopeParameters();
            isoParameters.PenaltyMode = EnumerationIsotopePenaltyMode.PointToLeft;
            isoParameters.NumberOfPeaksToLeftForPenalty = 1;

            IsoCalParameters = new ParametersIsoCalibration(ProcessingParameters.CalibrateShiftMZ, ProcessingParameters.CalibrateShiftMono);
            IsoShiftParameters = new ParametersIsoShift(isoParameters.PenaltyMode, isoParameters.NumberOfPeaksToLeftForPenalty);

            TheoryIsotopeGenerator = new TheoreticalIsotopicProfileWrapper(IsoCalParameters, IsoShiftParameters);

            TheorFeatureGenV2 = new IsotopeProfileSimple(new ParametersSimpleIsotope(_theorFeatureGen));
            
        }




        public override void DoWorkflow()
        {
            Result = Run.ResultCollection.GetTargetedResult(Run.CurrentMassTag);
            Result.ResetResult();

            #region SK

            int currentScan = Run.CurrentMassTag.ScanLCTarget;
            double currentMZ = Run.CurrentMassTag.MZ;
            double currentMono = Run.CurrentMassTag.MonoIsotopicMass;

            ProcessingParametersChromatogram lcParameters = new ProcessingParametersChromatogram();
            lcParameters.ParametersChromGenerator = new ChromPeakGeneratorParameters();
            lcParameters.ParametersChromGenerator.ChromToleranceInPPM = ProcessingParameters.ChromToleranceInPPMInitialFactor;//  15;
            lcParameters.ParametersChromGenerator.ChromeGeneratorMode = Globals.ChromatogramGeneratorMode.MZ_BASED;
            lcParameters.ParametersChromGenerator.IsotopeProfileType = Globals.IsotopicProfileType.UNLABELLED;
            lcParameters.ParametersOmicsThreshold.SignalToShoulderCuttoff = -1;
            lcParameters.ParametersSavitskyGolay.PointsForSmoothing = ProcessingParameters.ChromSmootherNumPointsInSmooth;//9//_workflowParameters.ChromSmootherNumPointsInSmooth;//9;
            lcParameters.InitializeEngines();


            int scansToSum = ProcessingParameters.NumMSScansToSum;// _workflowParameters.NumMSScansToSum;

            //convert target
            TargetBase currentBaseTarget = Result.Target;
            IqTarget tempIQTarget = new IqTargetBasic();
            tempIQTarget.TheorIsotopicProfile = currentBaseTarget.IsotopicProfile;
            tempIQTarget.MonoMassTheor = currentBaseTarget.MonoIsotopicMass;
            tempIQTarget.MZTheor = currentBaseTarget.MZ;
            tempIQTarget.EmpiricalFormula = currentBaseTarget.EmpiricalFormula;
            tempIQTarget.ChargeState = currentBaseTarget.ChargeState;

            IGenerateIsotopeProfile theorFeatureGenV2 = TheorFeatureGenV2;
            TheoryIsotopeGenerator.Generate(ref theorFeatureGenV2, tempIQTarget, ProcessingParameters.IsotopicProfileMode, true, true);
            
            tempIQTarget.TheorIsotopicProfile.MostAbundantIsotopeMass = tempIQTarget.TheorIsotopicProfile.GetMZofMostAbundantPeak();

            double difference = tempIQTarget.MonoMassTheor - currentMono;
            difference = tempIQTarget.MonoMassTheor - currentMZ;

            Run.CurrentMassTag.IsotopicProfile = tempIQTarget.TheorIsotopicProfile;
            Run.CurrentMassTag.MonoIsotopicMass = tempIQTarget.MonoMassTheor;
            Run.CurrentMassTag.MZ = tempIQTarget.TheorIsotopicProfile.GetMZ();

            Result.IsotopicProfile = tempIQTarget.TheorIsotopicProfile;
            Run.CurrentMassTag.IsotopicProfile = tempIQTarget.TheorIsotopicProfile;

            IQGlyQ.Processors.ProcessorChromatogram _LcProcessor = new ProcessorChromatogram(lcParameters);
            Run32.Backend.Data.XYData rawChromatogram = lcParameters.Engine_PeakChromGenerator.GenerateChromatogram(Run, Run.CurrentMassTag.IsotopicProfile.MostAbundantIsotopeMass);
            List<ProcessedPeak> smoothedChromatogram = _LcProcessor.Execute(rawChromatogram, EnumerationChromatogramProcessing.SmoothingOnly);

            List<PNNLOmics.Data.XYData> smoothedChromatogramXYData = GetPeaksDllLite.Functions.ConvertProcessedPeakToXYData.ConvertPoints(smoothedChromatogram);
            
            //select best peak

            Run.XYData = GetPeaksDllLite.Functions.ConvertXYData.OmicsProcessedPeakToRunXYData(smoothedChromatogram);
            updateChromDataXYValues(Run.XYData);



            //select single peak for feature finding
            List<ProcessedPeak> LcPeaks = _LcProcessor.Execute(smoothedChromatogramXYData, EnumerationChromatogramProcessing.ChromatogramLevel);

            if (LcPeaks.Count > 0)
            {
                ProcessedPeak peak = IQGlyQ.SelectClosest.SelectClosestPeakToCenter(LcPeaks, currentScan);
                Result.ChromPeakSelected = new ChromPeak(peak.XValue, Convert.ToSingle(peak.Height), peak.Width, 100);
                ChromPeakSelected = Result.ChromPeakSelected;
            }

           

            //IqTarget tempTarget = new IqTargetBasic();
            //tempTarget.ChargeState = Run.CurrentMassTag.ChargeState;
            //tempTarget.EmpiricalFormula = Run.CurrentMassTag.EmpiricalFormula;
            //Utiliites.TherortIsotopeicProfileWrapper(ref _theorFeatureGen, tempTarget, tempParameters.IsotopeProfileMode, tempParameters.MSParameters.IsoParameters.DeltaMassCalibrationMZ, tempParameters.MSParameters.IsoParameters.DeltaMassCalibrationMono, tempParameters.MSParameters.IsoParameters.ToMassCalibrate,tempParameters.MSParameters.IsoParameters.PenaltyMode);
            //Result.IsotopicProfile = tempTarget.TheorIsotopicProfile;

            //Result.IsotopicProfile = IQGlyQ.Utiliites.TherortIsotopeicProfileWrapperLowLevel(ref _theorFeatureGen, Run.CurrentMassTag.EmpiricalFormula, Run.CurrentMassTag.ChargeState);
            //Run.CurrentMassTag.IsotopicProfile = Result.IsotopicProfile;

            #endregion

            //ExecuteTask(_theorFeatureGen);

            //ExecuteTask(_chromGen);
            //ExecuteTask(_chromSmoother);
            //updateChromDataXYValues(Run.XYData);

            //ExecuteTask(_chromPeakDetector);
            //UpdateChromDetectedPeaks(Run.PeakList);

            //ExecuteTask(_chromPeakSelector);
            //ChromPeakSelected = Result.ChromPeakSelected;

            //Result.ResetMassSpectrumRelatedInfo();


            //this is where we set the observed isotopic profile mass spec raw data
            
            Result.Run.ScanSetCollection.Create(Run, Run.MinLCScan, Run.MaxLCScan, scansToSum, 1, false);
            Result.Run.CurrentScanSet = IQGlyQ.SelectClosest.SelectClosestScanSetToScan(Run, currentScan);

            Run.ResultCollection.CurrentTargetedResult.ScanSet = Result.Run.CurrentScanSet;
            Result.FailedResult = false;

            //back to decon

            ExecuteTask(MSGenerator);

            updateMassSpectrumXYValues(Run.XYData);

            TrimData(Run.XYData, Run.CurrentMassTag.MZ, MsLeftTrimAmount, MsRightTrimAmount);

            ExecuteTask(_msfeatureFinder);

            ApplyMassCalibration(Result);

            //Run.ResultCollection.
            ExecuteTask(_fitScoreCalc);

            ExecuteTask(_resultValidator);

            if (_workflowParameters.ChromatogramCorrelationIsPerformed)
            {
                ExecuteTask(_chromatogramCorrelator);
            }

            
            //GetDataFromQuantifier();
            
            Success = true;
        }

        public override WorkflowParameters WorkflowParameters
        {
            get
            {
                return _workflowParameters;
            }
            set
            {
                _workflowParameters = value as TargetedWorkflowParameters;
            }
        }
       
        protected override Globals.ResultType GetResultType()
        {
            return Globals.ResultType.SIPPER_TARGETED_RESULT;
        }

        private void ApplyMassCalibration(TargetedResultBase result)
        {
            //Result.MonoIsotopicMassCalibrated = Result.GetCalibratedMonoisotopicMass();
            Result.MassErrorBeforeAlignment = Result.GetMassErrorBeforeAlignmentInPPM();
            //Result.MassErrorAfterAlignment = Result.GetMassErrorAfterAlignmentInPPM();

        }

        protected override void ExecutePostWorkflowHook()
        {
            base.ExecutePostWorkflowHook();

            GetDataFromQuantifier(Run);
        }

        private void GetDataFromQuantifier(Run run)
        {
            
            
            List<double> peakNumList = new List<double>();
            List<double> mzList = new List<double>();
            List<double> rsquaredvalList = new List<double>();
            
            
            int counter = 0;

            if (run.ResultCollection.CurrentTargetedResult != null &&
                run.ResultCollection.CurrentTargetedResult.ChromCorrelationData != null &&
                run.ResultCollection.CurrentTargetedResult.ChromCorrelationData.CorrelationDataItems != null && 
                run.ResultCollection.CurrentTargetedResult.IsotopicProfile != null &&
                run.ResultCollection.CurrentTargetedResult.IsotopicProfile.Peaklist != null){

                for (int i = 0; i < run.ResultCollection.CurrentTargetedResult.ChromCorrelationData.CorrelationDataItems.Count; i++)
                {
                    ChromCorrelationDataItem item = run.ResultCollection.CurrentTargetedResult.ChromCorrelationData.CorrelationDataItems[i];

                    MSPeak peak = run.ResultCollection.CurrentTargetedResult.IsotopicProfile.Peaklist[i];

                    if (item.CorrelationRSquaredVal != null)
                    {
                        rsquaredvalList.Add(item.CorrelationRSquaredVal.Value);
                        peakNumList.Add(counter);
                        mzList.Add(Math.Round(peak.XValue,2));
                        counter++;
                    }

                }
            }

            if(ChromCorrelationRSquaredVals == null)
            {
                ChromCorrelationRSquaredVals = new Run32.Backend.Data.XYData();
            }
            if (ChromCorrelationRSquaredVals != null)
            {
                //ChromCorrelationRSquaredVals.Xvalues = peakNumList.ToArray();
                ChromCorrelationRSquaredVals.Xvalues = mzList.ToArray();
                ChromCorrelationRSquaredVals.Yvalues = rsquaredvalList.ToArray();
            }
            


        }
    }
}
