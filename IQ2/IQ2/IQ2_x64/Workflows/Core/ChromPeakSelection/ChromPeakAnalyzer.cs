using System;
using System.Collections.Generic;
using IQ_X64.Backend.ProcessingTasks;
using IQ_X64.Backend.ProcessingTasks.FitScoreCalculators;
using IQ_X64.Backend.ProcessingTasks.MSGenerators;
using IQ_X64.Backend.ProcessingTasks.PeakDetectors;
using IQ_X64.Backend.ProcessingTasks.ResultValidators;
using IQ_X64.Backend.ProcessingTasks.TargetedFeatureFinders;
using IQ_X64.Workflows.Utilities;
using IQ_X64.Workflows.WorkFlowParameters;
using Run64.Backend.Core;

namespace IQ_X64.Workflows.Core.ChromPeakSelection
{
    public class ChromPeakAnalyzer
    {

        protected MSGenerator MSGenerator;
        protected ResultValidatorTask ResultValidator;
        protected IsotopicProfileFitScoreCalculator FitScoreCalc;
        protected InterferenceScorer InterferenceScorer;
        protected DeconToolsPeakDetectorV2 MSPeakDetector;
        protected IterativeTFF TargetedMSFeatureFinder;


        private ChromPeakUtilities _chromPeakUtilities = new ChromPeakUtilities();

        #region Constructors

        public ChromPeakAnalyzer(TargetedWorkflowParameters parameters)
        {
            Parameters = parameters;

            IterativeTFFParameters iterativeTffParameters = new IterativeTFFParameters();

            TargetedMSFeatureFinder = new IterativeTFF(iterativeTffParameters);
            InterferenceScorer = new InterferenceScorer();
            MSPeakDetector = new DeconToolsPeakDetectorV2();
            FitScoreCalc = new IsotopicProfileFitScoreCalculator();
            ResultValidator = new ResultValidatorTask();


        }

        protected TargetedWorkflowParameters Parameters { get; set; }

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public List<ChromPeakQualityData> GetChromPeakQualityData(Run run, IqTarget target, List<Peak> chromPeakList)
        {
            List<ChromPeakQualityData> peakQualityList = new List<ChromPeakQualityData>();


            if (MSGenerator == null)
            {
                MSGenerator = MSGeneratorFactory.CreateMSGenerator(run.MSFileType);
                MSGenerator.IsTICRequested = false;
            }

            //iterate over peaks within tolerance and score each peak according to MSFeature quality
#if DEBUG
            int tempMinScanWithinTol = run.GetScanValueForNET((float)(target.ElutionTimeTheor - Parameters.ChromNETTolerance));
            int tempMaxScanWithinTol = run.GetScanValueForNET((float)(target.ElutionTimeTheor + Parameters.ChromNETTolerance));
            int tempCenterTol = run.GetScanValueForNET((float)target.ElutionTimeTheor);


            Console.WriteLine("SmartPeakSelector --> NETTolerance= " + Parameters.ChromNETTolerance + ";  chromMinCenterMax= " +
                              tempMinScanWithinTol + "\t" + tempCenterTol + "" +
                              "\t" + tempMaxScanWithinTol);
            Console.WriteLine("MT= " + target.ID + ";z= " + target.ChargeState + "; mz= " + target.MZTheor.ToString("0.000") +
                              ";  ------------------------- PeaksWithinTol = " + chromPeakList.Count);
#endif



            //target.NumChromPeaksWithinTolerance = peaksWithinTol.Count;


            foreach (ChromPeak chromPeak in chromPeakList)
            {
                // TODO: Currently hard-coded to sum only 1 scan
                var lcscanset =_chromPeakUtilities.GetLCScanSetForChromPeak(chromPeak, run, 1);

                //generate a mass spectrum
                var massSpectrumXYData = MSGenerator.GenerateMS(run, lcscanset);

                //find isotopic profile
                List<Peak> mspeakList = new List<Peak>();
                var observedIso = TargetedMSFeatureFinder.IterativelyFindMSFeature(massSpectrumXYData, target.TheorIsotopicProfile, out mspeakList);

                double fitScore = 1;

                double iscore = 1;

                //get fit score
                fitScore = FitScoreCalc.CalculateFitScore(target.TheorIsotopicProfile, observedIso, massSpectrumXYData);

                //get i_score
                iscore = InterferenceScorer.GetInterferenceScore(target.TheorIsotopicProfile, mspeakList);

                LeftOfMonoPeakLooker leftOfMonoPeakLooker = new LeftOfMonoPeakLooker();
                var peakToTheLeft = leftOfMonoPeakLooker.LookforPeakToTheLeftOfMonoPeak(target.TheorIsotopicProfile.getMonoPeak(), target.ChargeState,
                                                                    mspeakList);


                bool hasPeakTotheLeft = peakToTheLeft != null;

                //collect the results together


                ChromPeakQualityData pq = new ChromPeakQualityData(chromPeak);

                if (observedIso == null)
                {
                    pq.IsotopicProfileFound = false;

                }
                else
                {
                    pq.IsotopicProfileFound = true;
                    pq.Abundance = observedIso.IntensityMostAbundant;
                    pq.FitScore = fitScore;
                    pq.InterferenceScore = iscore;
                    pq.IsotopicProfile = observedIso;
                    pq.IsIsotopicProfileFlagged = hasPeakTotheLeft;
                    pq.ScanLc = lcscanset.PrimaryScanNumber;
                }

                peakQualityList.Add(pq);
#if DEBUG
                pq.Display();
#endif
            }

            return peakQualityList;



        }


        #endregion

        #region Private Methods

        #endregion

    }
}
