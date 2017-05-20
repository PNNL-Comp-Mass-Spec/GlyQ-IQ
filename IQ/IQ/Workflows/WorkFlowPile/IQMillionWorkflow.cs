using System.Collections.Generic;
using IQ.Backend.ProcessingTasks.LCGenerators;
using IQ.Backend.ProcessingTasks.PeakDetectors;
using IQ.Backend.ProcessingTasks.Smoothers;
using IQ.Backend.ProcessingTasks.TheorFeatureGenerator;
using IQ.Workflows.Core;
using IQ.Workflows.WorkFlowParameters;
using Run32.Backend;
using Run32.Backend.Core;

namespace IQ.Workflows.WorkFlowPile
{
    public class IQMillionWorkflow : TargetedWorkflow
    {
        #region Constructors

        public IQMillionWorkflow(Run run, TargetedWorkflowParameters parameters)
            : base(run, parameters)
        {

        }

        public IQMillionWorkflow(TargetedWorkflowParameters parameters)
            : base(parameters)
        {
        }

        #endregion


        #region IWorkflow Members

        protected override void DoMainInitialization()
        {
            ValidateParameters();

            _theorFeatureGen = new NominalMassFeatureGenerator();
            _chromGen = new PeakChromatogramGenerator(_workflowParameters.ChromGenTolerance,
                                                      _workflowParameters.ChromGeneratorMode,
                                                      Globals.IsotopicProfileType.UNLABELLED,
                                                      _workflowParameters.ChromGenToleranceUnit)
                            {
                                TopNPeaksLowerCutOff = 0.333,
                                ChromWindowWidthForNonAlignedData = (float) _workflowParameters.ChromNETTolerance*2,
                                ChromWindowWidthForAlignedData = (float) _workflowParameters.ChromNETTolerance*2
                            };

            const bool allowNegativeValues = false;
            _chromSmoother = new SavitzkyGolaySmoother(_workflowParameters.ChromSmootherNumPointsInSmooth, 2,
                                                       allowNegativeValues);
            _chromPeakDetector = new ChromPeakDetector(_workflowParameters.ChromPeakDetectorPeakBR,
                                                       _workflowParameters.ChromPeakDetectorSigNoise);

            ChromatogramXYData = new Run32.Backend.Data.XYData();
            ChromPeaksDetected = new List<ChromPeak>();
            MassSpectrumXYData = new Run32.Backend.Data.XYData();
            ChromPeaksDetected = new List<ChromPeak>();
        }

        public override void DoWorkflow()
        {
            Result = Run.ResultCollection.GetTargetedResult(Run.CurrentMassTag);
            Result.ResetResult();

            ExecuteTask(_theorFeatureGen);
            ExecuteTask(_chromGen);
            ExecuteTask(_chromSmoother);
            updateChromDataXYValues(Run.XYData);

            ExecuteTask(_chromPeakDetector);
            UpdateChromDetectedPeaks(Run.PeakList);
        }

        protected override Globals.ResultType GetResultType()
        {
            return Globals.ResultType.BASIC_TARGETED_RESULT;
        }

        #endregion
    }
}
