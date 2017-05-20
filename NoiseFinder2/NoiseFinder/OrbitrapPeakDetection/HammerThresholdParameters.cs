using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrbitrapPeakDetection
{
    public class HammerThresholdParameters
    {
        /// <summary>
        /// Multiplier for the noise threshold.  0= average noise.  Negative goes below the average and Postive raises the noise line above the average 
        /// </summary>
        public double SigmaMultiplier { get; set; }

        /// <summary>
        /// Minimum size of noise threshold mass region based on number of points used to find calculations. Must be at least 30 for accurate calculations
        /// </summary>
        public int MinimumSizeOfRegion { get; set; }

        /// <summary>
        /// number of points in a region.  typically 30 so we have a large enough N for statistics.
        /// </summary>
        public List<int> NumberOfPointsPerNoiseRegion { get; set; }

        /// <summary>
        /// threshold will find ends of clusters and draw a threshold line.  Cluster will take all clustered peaks.
        /// </summary>
        public OrbitrapFilteringMethod ThresholdOrClusterChoise { get; set; }

        /// <summary>
        /// allow to iterate and find best cluter width to use
        /// </summary>
        public OptimizeOrDefaultMassSpacing OptimizeOrDefaultChoise { get; set; }

        /// <summary>
        /// similar to the mass of a neutron or 1.00235 from THRASH
        /// </summary>
        public double SeedClusterSpacing { get; set; }

        /// <summary>
        /// window width to search for in Da
        /// </summary>
        public double SeedMassToleranceDa { get; set; }

        /// <summary>
        /// good for non thresholded data such as QTOF.  To use pure Hammer, set this to 0.  
        /// </summary>
        public double CentroidPeakToBackgroundRatio { get; set; }

        public HammerThresholdParameters()
        {
            NumberOfPointsPerNoiseRegion = new List<int>();
            MinimumSizeOfRegion = 30;
            SigmaMultiplier = 0;
            ThresholdOrClusterChoise = OrbitrapFilteringMethod.Threshold;
            OptimizeOrDefaultChoise = OptimizeOrDefaultMassSpacing.Default;
            SeedClusterSpacing = 1.00235;
            SeedMassToleranceDa = 0.018;//0.006 is the variation across several scans
            CentroidPeakToBackgroundRatio = 0;
        }

        public enum OrbitrapFilteringMethod
        {
            Threshold,
            Cluster
        }

        public enum OptimizeOrDefaultMassSpacing
        {
            Default,
            Optimize
        }
    }
}
