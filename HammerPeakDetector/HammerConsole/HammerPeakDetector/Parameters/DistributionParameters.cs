using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HammerPeakDetector.Parameters
{
    public class DistributionParameters
    {
        public double CenterDa { get; set; }

        public double WindowsSizeDa { get; set; }

        public double CenterInitialDa { get; set; }

        public double WindowsSizeInitialDa { get; set; }

        public DistributionParameters(HammerThresholdParameters parameters)
        {
            CenterInitialDa = parameters.SeedClusterSpacingCenter;
            WindowsSizeInitialDa = parameters.SeedMassToleranceDa;
        }
    }
}
