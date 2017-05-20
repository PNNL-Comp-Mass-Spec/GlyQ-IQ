using System;
using IQ.Backend.ProcessingTasks.FitScoreCalculators;
using IQ.Backend.ProcessingTasks.PeakDetectors;
using IQ.Backend.ProcessingTasks.ResultValidators;
using IQ.Backend.ProcessingTasks.TargetedFeatureFinders;
using Run32.Backend;

namespace IQ.Workflows.Core.ChromPeakSelection
{
    public class SmartChromPeakSelector : SmartChromPeakSelectorBase
    {

        #region Constructors
        public SmartChromPeakSelector(SmartChromPeakSelectorParameters parameters)
        {
            this.Parameters = parameters;

            MSPeakDetector = new DeconToolsPeakDetectorV2(parameters.MSPeakDetectorPeakBR, parameters.MSPeakDetectorSigNoiseThresh, Globals.PeakFitType.QUADRATIC, true);

            var iterativeTFFParams = new IterativeTFFParameters();
            iterativeTFFParams.ToleranceInPPM = parameters.MSToleranceInPPM;
            iterativeTFFParams.MinimumRelIntensityForForPeakInclusion = parameters.IterativeTffMinRelIntensityForPeakInclusion;

            if (parameters.MSFeatureFinderType == Globals.TargetedFeatureFinderType.BASIC)
            {
                throw new NotSupportedException("Currently the Basic TFF is not supported in the SmartChromPeakSelector");
                
                //TargetedMSFeatureFinder = new TargetedFeatureFinders.BasicTFF(parameters.MSToleranceInPPM);
            }
            else
            {
                TargetedMSFeatureFinder = new IterativeTFF(iterativeTFFParams);
            }

            resultValidator = new ResultValidatorTask();
            fitScoreCalc = new IsotopicProfileFitScoreCalculator();

            InterferenceScorer = new InterferenceScorer();


        }

        #endregion

       
    }
}
