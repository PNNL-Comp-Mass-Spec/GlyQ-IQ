using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.ProcessingTasks.TheorFeatureGenerator;
using IQ.Backend.ProcessingTasks.TheorFeatureGenerator;
using IQGlyQ.Enumerations;
using IQGlyQ.Tasks.FitScores;

namespace IQGlyQ.Objects
{
    public class IsotopeParameters
    {
        public int NumberOfPeaksToLeftForPenalty { get; set; }

        /// <summary>
        /// score experimental profile to labeled or unlabeled theoretical profile
        /// </summary>
        public EnumerationIsotopePenaltyMode PenaltyMode { get; set; }

        //public FragmentedTargetedWorkflowParametersIQ IQParameters { get; set; }

        //public ITheorFeatureGenerator TheorFeatureGen { get; set; }

        //should we offset the isotope profiles for penalty scoring
        public bool ToShift { get; set; }


        public double FractionalIntensityCuttoffForTheoretical { get; set; }

        /// <summary>
        /// how much to offset the target and iso masses so we match the dataset at the m/z level (spectra calibration)
        /// </summary>
        public double DeltaMassCalibrationMZ { get; set; } // = -0.00289 / 2;

        /// <summary>
        /// how much to offset the target and iso masses so we match the dataset (chemcial modifications)
        /// </summary>
        public double DeltaMassCalibrationMono { get; set; } // = -0.00289 / 2;

        /// <summary>
        /// apply the calibration to the Targets and Isos?
        /// </summary>
        public bool ToMassCalibrate { get; set; }

        /// <summary>
        /// single profile or overlapped
        /// </summary>
        public EnumerationIsotopicProfileMode IsotopeProfileMode { get; set; }

        /// <summary>
        /// adds a divisor to the fit score calcuator so that large and small number of isomers score similarly
        /// </summary>
        public bool DivideFitScoreByNumberOfIons { get; set; }

        /// <summary>
        /// fraction of integrated isotopic envelope that has to be detected.  0.75 means 75% of the area (peaks that add up to 75%) need to be detected in data to keep
        /// 0 means everything passes
        /// </summary>
        public double CuttOffArea { get; set; }

        /// <summary>
        /// new version parameters
        /// </summary>
        public FitScoreParameters FitScoreParameters { get; set; }
        


        public IsotopeParameters()
        {
            NumberOfPeaksToLeftForPenalty = 0;
            CuttOffArea = 0;
            FitScoreParameters = FitScoreParameterFactory.Build();
        }

        //public IsotopeParameters(int numberOfPeaksToLeftForPenalty, EnumerationIsotopePenaltyMode mode, EnumerationIsotopicProfileMode isotopeProfileMode, ITheorFeatureGenerator theorFeatureGen, double fractionalIntensityCuttoffForTheoretical, double cutOffArea, FitScoreParameters fitScoreParametersIn)
        
        public IsotopeParameters(int numberOfPeaksToLeftForPenalty, EnumerationIsotopePenaltyMode mode, EnumerationIsotopicProfileMode isotopeProfileMode, double fractionalIntensityCuttoffForTheoretical, double cutOffArea, FitScoreParameters fitScoreParametersIn)
        {
            NumberOfPeaksToLeftForPenalty = numberOfPeaksToLeftForPenalty;
            PenaltyMode = mode;
            //IQParameters = parameters;
            //TheorFeatureGen = theorFeatureGen;
            FractionalIntensityCuttoffForTheoretical = fractionalIntensityCuttoffForTheoretical;
            IsotopeProfileMode = isotopeProfileMode;
            DivideFitScoreByNumberOfIons = true;
            CuttOffArea = cutOffArea;
            FitScoreParameters = fitScoreParametersIn;
        }
    }
}
