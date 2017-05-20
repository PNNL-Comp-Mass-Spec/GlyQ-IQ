using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HammerPeakDetector.Enumerations;

namespace HammerPeakDetector.Parameters
{
    public class HammerThresholdParameters
    {
        /// <summary>
        /// Multiplier for the noise threshold.  0= average noise.  Negative goes below the average and Postive raises the noise line above the average 
        /// </summary>
        public double ThresholdSigmaMultiplier { get; set; }

        /// <summary>
        /// Multiplier for the setting the width of the new difference window 
        /// </summary>
        public double MassTolleranceSigmaMultiplier { get; set; }

        //TODO remove old thresholder now that we have a moving window
        /// <summary>
        /// Minimum size of noise threshold mass region based on number of points used to find calculations. Must be at least 30 for accurate calculations
        /// </summary>
        public int MinimumSizeOfRegion { get; set; }

        /// <summary>
        /// number of points in a region.  typically 30 so we have a large enough N for statistics.
        /// </summary>
        public List<int> NumberOfPointsPerNoiseRegion { get; set; }

        /// <summary>
        /// Threshold will find ends of clusters and draw a threshold line.  Cluster will take all clustered peaks. PValue will take all
        /// peaks with a p-value above the cutoff value
        /// </summary>
        //public HammerFilteringMethod ThresholdOrClusterChoise { get; set; }
        public HammerFilteringMethod FilteringMethod { get; set; }
        
        /// <summary>
        /// 0= default with no iterations (faster) and # = iterate and -1 is for converge
        /// number of iterations for optimizer.  each pass will calculate a new center and sigma.  -1 will converve (set convege criteria, Not implemented yet).  OptimizeOrDefaultChoise must be set to optimizer
        /// </summary>
        public int Iterations { get; set; }

        /// <summary>
        /// similar to the mass of a neutron or 1.00235 from THRASH
        /// </summary>
        public double SeedClusterSpacingCenter { get; set; }

        /// <summary>
        /// window width to search for in Da
        /// </summary>
        public double SeedMassToleranceDa { get; set; }

        //TODO. this needs to be pulled outside
        /// <summary>
        /// good for non thresholded data such as QTOF.  To use pure Hammer, set this to 0.  
        /// </summary>
        public double CentroidPeakToBackgroundRatio { get; set; }

        /// <summary>
        /// Maximum charge state for clustering  
        /// </summary>
        public int MaxChargeState { get; set; }

        /// <summary>
        /// Minimum charge state for clustering  
        /// </summary>
        public int MinChargeState { get; set; }

        /// <summary>
        /// final center to report
        /// </summary>
        public double finalCenter { get; set; }

        /// <summary>
        /// final center to report
        /// </summary>
        public double finalSigma { get; set; }

        public HammerThresholdParameters()
        {
            NumberOfPointsPerNoiseRegion = new List<int>();
            MinimumSizeOfRegion = 30;
            ThresholdSigmaMultiplier = 0;
            FilteringMethod = HammerFilteringMethod.Threshold;
            //OptimizeOrDefaultChoise = OptimizeOrDefaultMassSpacing.Default;
            SeedClusterSpacingCenter = 1.00325;
            SeedMassToleranceDa = 0.009;//0.006 is the variation across several scans
            CentroidPeakToBackgroundRatio = 0;
            MassTolleranceSigmaMultiplier = 2;
            MinChargeState = 1;
            MaxChargeState = 5;
            Iterations = 0;
        }
    }
}
