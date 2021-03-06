﻿using System.Collections.Generic;
using IQ_X64.Backend.Core;
using IQ_X64.Backend.Utilities.IsotopeDistributionCalculation.TomIsotopicDistribution;
using Run64.Backend;
using Run64.Backend.Core;
using Run64.Backend.Core.Results;
using Run64.Backend.Data;
using Run64.Utilities;

namespace IQ_X64.Backend.ProcessingTasks.FitScoreCalculators
{
    public class DeconToolsFitScoreCalculator : IFitScoreCalculator
    {
        //DECONENGINE: remove this.... it references deconEngine
        //MercuryDistributionCreator distributionCreator;
        TomIsotopicPattern _tomIsotopicPatternGenerator = new TomIsotopicPattern();


        PeakUtilities peakUtil = new PeakUtilities();

        public double MZVar { get; set; }
        public IsotopicProfile TheorIsotopicProfile { get; set; }

        public DeconToolsFitScoreCalculator()
        {
           
        }

        public DeconToolsFitScoreCalculator(double mzVar)
            : this()
        {
            this.MZVar = mzVar;
        }


        public override void GetFitScores(IEnumerable<IsosResult> isosResults)
        {

            foreach (IsosResult result in isosResults)
            {
                //create a temporary mass tag, as a data object for storing relevent info, and using the CalculateMassesForIsotopicProfile() method. 
                PeptideTarget mt = new PeptideTarget();

                mt.ChargeState = (short)result.IsotopicProfile.ChargeState;
                mt.MonoIsotopicMass = result.IsotopicProfile.MonoIsotopicMass;
                mt.MZ = (mt.MonoIsotopicMass / mt.ChargeState) + Globals.PROTON_MASS;

                //TODO: use Josh's isotopicDistribution calculator after confirming averagine formula
                mt.EmpiricalFormula = _tomIsotopicPatternGenerator.GetClosestAvnFormula(result.IsotopicProfile.MonoIsotopicMass, false);

                mt.IsotopicProfile = _tomIsotopicPatternGenerator.GetIsotopePattern(mt.EmpiricalFormula, _tomIsotopicPatternGenerator.aafIsos);
                this.TheorIsotopicProfile = mt.IsotopicProfile;

                mt.CalculateMassesForIsotopicProfile(mt.ChargeState);

                XYData theorXYData = mt.IsotopicProfile.GetTheoreticalIsotopicProfileXYData(result.IsotopicProfile.GetFWHM());

                offsetDistribution(theorXYData, mt.IsotopicProfile, result.IsotopicProfile);

                //double resolution = result.IsotopicProfile.GetMZofMostAbundantPeak() / result.IsotopicProfile.GetFWHM();
                //distributionCreator.CreateDistribution(result.IsotopicProfile.MonoIsotopicMass, result.IsotopicProfile.ChargeState, resolution);
                //distributionCreator.OffsetDistribution(result.IsotopicProfile);
                //XYData theorXYData = distributionCreator.Data;

                AreaFitter areafitter = new AreaFitter();
                double fitval = areafitter.GetFit(theorXYData, result.Run.XYData, 0.1);

                if (double.IsNaN(fitval) || fitval > 1) fitval = 1;

                result.IsotopicProfile.Score = fitval;


            }
        }

        private void offsetDistribution(XYData theorXYData, IsotopicProfile theorIsotopicProfile, IsotopicProfile observedIsotopicProfile)
        {
            double offset = 0;
            if (theorIsotopicProfile == null || theorIsotopicProfile.Peaklist == null || theorIsotopicProfile.Peaklist.Count == 0) return;

            MSPeak mostIntensePeak = theorIsotopicProfile.getMostIntensePeak();
            int indexOfMostIntensePeak = theorIsotopicProfile.Peaklist.IndexOf(mostIntensePeak);

            if (observedIsotopicProfile.Peaklist == null || observedIsotopicProfile.Peaklist.Count == 0) return;

            bool enoughPeaksInTarget = (indexOfMostIntensePeak <= observedIsotopicProfile.Peaklist.Count - 1);

            if (enoughPeaksInTarget)
            {
                MSPeak targetPeak = observedIsotopicProfile.Peaklist[indexOfMostIntensePeak];
                offset = targetPeak.XValue - mostIntensePeak.XValue;
                //offset = observedIsotopicProfile.Peaklist[0].XValue - theorIsotopicProfile.Peaklist[0].XValue;   //want to test to see if Thrash is same as rapid

            }
            else
            {
                offset = observedIsotopicProfile.Peaklist[0].XValue - theorIsotopicProfile.Peaklist[0].XValue;
            }

            for (int i = 0; i < theorXYData.Xvalues.Length; i++)
            {
                theorXYData.Xvalues[i] = theorXYData.Xvalues[i] + offset;
            }

            foreach (var peak in theorIsotopicProfile.Peaklist)
            {
                peak.XValue = peak.XValue + offset;
                
            }


        }

        public override void Cleanup()
        {
            base.Cleanup();

        }



    }
}
