using System;
using System.Linq;
using DeconTools.Backend;
using DeconTools.Backend.Core;
using DeconTools.Backend.ProcessingTasks.FitScoreCalculators;
using DeconTools.Backend.ProcessingTasks.TheorFeatureGenerator;
using DeconTools.Backend.Utilities;
using DeconTools.Utilities;
using System.Collections.Generic;
using DeconTools.Workflows.Backend.Core;
using GetPeaks_DLL.DataFIFO;
using IQGlyQ.Enumerations;
using IQGlyQ.Objects;

namespace IQGlyQ.Tasks
{
    public class IsotopicPeakFitScoreCalculatorSK:Task
    {
        #region properties

        private IsotopeParameters IsotopeParameterPile { get; set; }
       
        public IsotopicPeakFitScoreCalculatorSK()
        {
        }

        public IsotopicPeakFitScoreCalculatorSK(IsotopeParameters isoParameters)
        {
            IsotopeParameterPile = isoParameters;
        }


        #endregion

        #region Public Methods


        public override void Execute(ResultCollection resultList)
        {
            Check.Require(resultList.Run.CurrentMassTag != null, this.Name + " failed; CurrentMassTag is empty");
            Check.Require(resultList.Run.XYData != null && resultList.Run.XYData.Xvalues != null && resultList.Run.XYData.Xvalues.Length > 0, this.Name + " failed; Run's XY data is empty. Need to Run an MSGenerator");
            Check.Require(resultList.CurrentTargetedResult != null, "No MassTagResult has been generated for CurrentMassTag");

            IqTarget target = new FragmentIQTarget();
            target.ChargeState = resultList.CurrentTargetedResult.Target.ChargeState;
            target.EmpiricalFormula = resultList.CurrentTargetedResult.Target.EmpiricalFormula;
            

            ITheorFeatureGenerator theorFeatureGenerator = IsotopeParameterPile.TheorFeatureGen;
            Utiliites.TherortIsotopeicProfileWrapper(ref theorFeatureGenerator, target, IsotopeParameterPile.IsotopeProfileMode, IsotopeParameterPile.DeltaMassCalibrationMZ, IsotopeParameterPile.DeltaMassCalibrationMono, IsotopeParameterPile.ToMassCalibrate, IsotopeParameterPile.PenaltyMode);

            resultList.CurrentTargetedResult.Score = CalculateFitScore(target.TheorIsotopicProfile.Peaklist, resultList.CurrentTargetedResult.IsotopicProfile.Peaklist, IsotopeParameterPile.DivideFitScoreByNumberOfIons);
        }
        #endregion

        /// <summary>
        /// these need to be synched and clipped prior to entering
        /// </summary>
        /// <param name="theorProfile"></param>
        /// <param name="observedProfile"></param>
        /// <returns></returns>
        public double CalculateFitScore(List<MSPeak> theorProfile, List<MSPeak> observedProfile, bool usePeakNumberCorrection)
        {
            int lengthTheory = theorProfile.Count;
            int lengthObserved = observedProfile.Count;

            if(lengthTheory!=lengthObserved)
            {
                Console.WriteLine("Isotope profiles are different lenghts");
                Console.Read();
            }

          
 
            PeakLeastSquaresFitterSK peakFitter = new PeakLeastSquaresFitterSK();
            //double fitval = peakFitter.GetFit(theorPeakList, observedPeakList, minCuttoffTheorPeakIntensityFraction, massErrorPPMBetweenPeaks, numberOfPeaksToLeftForPenalty);//0.1
            //double fitval = peakFitter.GetFit(theorProfile, observedProfile);//0.1
            //double fitval = peakFitter.GetWeightedFit(theorProfile, observedProfile);//0.1
             

            double fitval = peakFitter.GetPenaltyFit(theorProfile, observedProfile, usePeakNumberCorrection);//0.1)
            if (double.IsNaN(fitval) || fitval > 1) fitval = 1;

            

            return fitval;
        }

    }
}
