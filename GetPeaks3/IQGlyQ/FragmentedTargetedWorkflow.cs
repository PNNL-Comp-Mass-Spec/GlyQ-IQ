using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Workflows.Backend.Core;
using DeconTools.Backend.Core;
using DeconTools.Backend.ProcessingTasks.TheorFeatureGenerator;
using DeconTools.Backend.ProcessingTasks.TargetedFeatureFinders;
using DeconTools.Backend.ProcessingTasks.ChromatogramProcessing;
using DeconTools.Backend.ProcessingTasks.MSGenerators;
using DeconTools.Backend.ProcessingTasks;
using DeconTools.Workflows.Backend.Core.ChromPeakSelection;
using IQGlyQ.Tasks;

namespace IQGlyQ
{
    public class FragmentedTargetedWorkflow : TargetedWorkflow

    {
        private DeuteratedQuantifierTask _quant;

        //private RemoveInsourceFragmentation _removeInsource;
        private RemoveInsourceFragmentationV2 _removeInsource;

        private SmartChromPeakSelector _chromPeakSelectorDeuterated;

        private MSGenerator _msGenerator;

        

        private List<FragmentTarget> Fragments { get; set; }

        public List<TargetedResultBase> PileOfResults { get; set; }

        #region Constructors

        public FragmentedTargetedWorkflow(Run run, FragmentedTargetedWorkflowParameters parameters)
            : base(run, parameters)
        {
            Fragments = parameters.Fragments;
            _msGenerator = MSGeneratorFactory.CreateMSGenerator(run.MSFileType);
            PileOfResults = new List<TargetedResultBase>();

           
           

        }

        public FragmentedTargetedWorkflow(FragmentedTargetedWorkflowParameters parameters)
            : base(parameters)
        {
            Fragments = parameters.Fragments;
            PileOfResults = new List<TargetedResultBase>();
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

            FragmentedTargetedWorkflowParameters tempParameters = (FragmentedTargetedWorkflowParameters)this._workflowParameters;

            _theorFeatureGen = new JoshTheorFeatureGenerator(tempParameters.IsotopeProfileType, tempParameters.IsotopeLabelingEfficiency, tempParameters.IsotopeLowPeakCuttoff, tempParameters.MolarMixingFractionOfH);
            
            _iterativeTFFParameters = new IterativeTFFParameters();
            _iterativeTFFParameters.ToleranceInPPM = _workflowParameters.MSToleranceInPPM;
            _iterativeTFFParameters.IsotopicProfileType = DeconTools.Backend.Globals.IsotopicProfileType.UNLABELLED;
            _iterativeTFFParameters.RequiresMonoIsotopicPeak = true;

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
            smartchrompeakSelectorParams.NumChromPeaksAllowed = 20;//changed from 10 to 20 because it will fail if ther are more than this number
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

            _msGenerator = MSGeneratorFactory.CreateMSGenerator(this.Run.MSFileType);


            _removeInsource = new RemoveInsourceFragmentationV2(tempParameters, _msGenerator, this.Run);
            //_removeInsource = new RemoveInsourceFragmentation(tempParameters, _msGenerator);
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

            //this is where we have several candidates for the feature.  some are peaks and some are isomers.  
            //we need to go into each of these scans and see if there is a glycan larger.  if there is, remove it from the list and save as new target for export
            //if we export the new targets we don't have to deal with recursion

            ExecuteTask(_removeInsource);

            //the four results here are intact and need to be finalized and exported.  these represent the isomers
            //foreach (ChromPeakQualityData variable in Result.ChromPeakQualityList)
            //{
            //    ChromPeakSelected = variable.Peak;


            //    Result.ResetMassSpectrumRelatedInfo();

            //    ExecuteTask(MSGenerator);
            //    updateMassSpectrumXYValues(Run.XYData);

            //    TrimData(Run.XYData, Run.CurrentMassTag.MZ, MsLeftTrimAmount, MsRightTrimAmount);

            //    ExecuteTask(_msfeatureFinder);

            //    ExecuteTask(_fitScoreCalc);
            //    ExecuteTask(_resultValidator);

            //    if (_workflowParameters.ChromatogramCorrelationIsPerformed)
            //    {
            //        ExecuteTask(_chromatogramCorrelator);
            //    }


            //    //TODO convert to result
            //    //PileOfResults.Add();

            //    Success = true;
            //}
        }

        

        protected override DeconTools.Backend.Globals.ResultType GetResultType()
        {
            return DeconTools.Backend.Globals.ResultType.DEUTERATED_TARGETED_RESULT;
        }

        #endregion

        
  
    }
}

