using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Workflows.Backend.Core;
using DeconTools.Backend.Core;
using DeconTools.Backend.ProcessingTasks.TheorFeatureGenerator;
using DeconTools.Backend.ProcessingTasks.TargetedFeatureFinders;
using DeconTools.Backend.ProcessingTasks.ChromatogramProcessing;
using DeconTools.Workflows.Backend.Core.ChromPeakSelection;

namespace IQGlyQ
{
    public class DeuteratedTargetedWorkflow : TargetedWorkflow

    {
        private DeuteratedQuantifierTask _quant;

        private SmartChromPeakSelector _chromPeakSelectorDeuterated;

        #region Constructors

        public DeuteratedTargetedWorkflow(Run run, DeuteratedTargetedWorkflowParameters parameters)
            : base(run, parameters)
        {

        }

        public DeuteratedTargetedWorkflow(DeuteratedTargetedWorkflowParameters parameters) : base(parameters)
        {

        }

        #endregion

   
        #region IWorkflow Members

       
        protected override void ExecutePostWorkflowHook()
        {
            base.ExecutePostWorkflowHook();
            ExecuteTask(_quant);
        }


        protected override void DoPostInitialization()
        {
            base.DoPostInitialization();

            DeuteratedTargetedWorkflowParameters tempParameters = (DeuteratedTargetedWorkflowParameters)this._workflowParameters;

            _theorFeatureGen = new JoshTheorFeatureGenerator(tempParameters.IsotopeProfileType, tempParameters.IsotopeLabelingEfficiency, tempParameters.IsotopeLowPeakCuttoff, tempParameters.MolarMixingFractionOfH);
            
            _iterativeTFFParameters = new IterativeTFFParameters();
            _iterativeTFFParameters.ToleranceInPPM = _workflowParameters.MSToleranceInPPM;
            _iterativeTFFParameters.IsotopicProfileType = DeconTools.Backend.Globals.IsotopicProfileType.UNLABELLED;

            _msfeatureFinder = new IterativeTFF(_iterativeTFFParameters);
            //_msfeatureFinder = new O16O18TargetedIterativeFeatureFinder(_iterativeTFFParameters);


            //start smart section
            SmartChromPeakSelectorParameters smartchrompeakSelectorParams = new SmartChromPeakSelectorParameters();
            smartchrompeakSelectorParams.MSFeatureFinderType = DeconTools.Backend.Globals.TargetedFeatureFinderType.ITERATIVE;
            smartchrompeakSelectorParams.MSPeakDetectorPeakBR = _workflowParameters.MSPeakDetectorPeakBR;
            smartchrompeakSelectorParams.MSPeakDetectorSigNoiseThresh = _workflowParameters.MSPeakDetectorSigNoise;
            smartchrompeakSelectorParams.MSToleranceInPPM = _workflowParameters.MSToleranceInPPM;
            smartchrompeakSelectorParams.NETTolerance = (float)_workflowParameters.ChromNETTolerance;
            smartchrompeakSelectorParams.NumScansToSum = _workflowParameters.NumMSScansToSum;
            smartchrompeakSelectorParams.NumChromPeaksAllowed = 10;
            smartchrompeakSelectorParams.IterativeTffMinRelIntensityForPeakInclusion = 0.5;
            smartchrompeakSelectorParams.MultipleHighQualityMatchesAreAllowed = _workflowParameters.MultipleHighQualityMatchesAreAllowed;

            _chromPeakSelectorDeuterated = new SmartChromPeakSelector(smartchrompeakSelectorParams);

            //_chromPeakSelectorDeuterated.TargetedMSFeatureFinder = _msfeatureFinder;
            _chromPeakSelectorDeuterated.IsotopicProfileType = DeconTools.Backend.Globals.IsotopicProfileType.UNLABELLED;

            //TODO need to find a way to call fit score calculator so it uses labeled.  this is done in the SmartChromPeakSelector
            //TODO targeted result base AddLabelledIso is not set up yet

            double minValue = 1;
            //_quant = new DeuteratedQuantifierTask(minValue, WorkflowParametersV2.IsotopeLabelingEfficiency);
            _quant = new DeuteratedQuantifierTask(minValue, tempParameters.IsotopeLabelingEfficiency);
        }

        public override void DoWorkflow()
        {
            this.Result = this.Run.ResultCollection.GetTargetedResult(this.Run.CurrentMassTag);
            this.Result.ResetResult();

            ExecuteTask(_theorFeatureGen);
            ExecuteTask(_chromGen);
            ExecuteTask(_chromSmoother);
            updateChromDataXYValues(Run.XYData);

            ExecuteTask(_chromPeakDetector);
            UpdateChromDetectedPeaks(Run.PeakList);

            //ExecuteTask(_chromPeakSelector);//origional
            ExecuteTask(_chromPeakSelectorDeuterated);//origional
            ChromPeakSelected = Result.ChromPeakSelected;

            Result.ResetMassSpectrumRelatedInfo();

            ExecuteTask(MSGenerator);
            updateMassSpectrumXYValues(Run.XYData);

            TrimData(Run.XYData, Run.CurrentMassTag.MZ, MsLeftTrimAmount, MsRightTrimAmount);

            ExecuteTask(_msfeatureFinder);

            ExecuteTask(_fitScoreCalc);
            ExecuteTask(_resultValidator);

            if (_workflowParameters.ChromatogramCorrelationIsPerformed)
            {
                //ExecuteTask(_chromatogramCorrelatorTask);
            }

            Success = true;
        }

        protected override DeconTools.Backend.Globals.ResultType GetResultType()
        {
            return DeconTools.Backend.Globals.ResultType.DEUTERATED_TARGETED_RESULT;
        }

        #endregion

        
  
    }
}

