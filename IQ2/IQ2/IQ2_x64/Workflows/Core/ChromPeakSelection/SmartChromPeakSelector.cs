using System;
 
using IQ_X64.Backend.ProcessingTasks.FitScoreCalculators;
using IQ_X64.Backend.ProcessingTasks.PeakDetectors;
using IQ_X64.Backend.ProcessingTasks.ResultValidators;
using IQ_X64.Backend.ProcessingTasks.TargetedFeatureFinders;
using Run64.Backend;

namespace IQ_X64.Workflows.Core.ChromPeakSelection
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
