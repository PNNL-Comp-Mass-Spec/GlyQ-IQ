using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HammerPeakDetector.Enumerations;
using HammerPeakDetector.Parameters;

namespace HammerPeakDetector.Utilities
{
    public static class SetParameters
    {
        public static void SetHammerParameters(HammerThresholdParameters hammerParameters, int mode)
        {
            #region Already accomplished in constructor; seems unnecessary (off)
            ////set parameters here
            //hammerParameters.MinChargeState = 1; //1 is default
            //hammerParameters.MaxChargeState = 5; //5 is default
            //hammerParameters.MinimumSizeOfRegion = 30; //30 is default
            //hammerParameters.SeedClusterSpacingCenter = 1.00235; //1.00235 is default

            ////these 2 are related
            //hammerParameters.SeedMassToleranceDa = 0.018; //0.018 is default
            //hammerParameters.MassTolleranceSigmaMultiplier = 2;

            #endregion

            switch (mode)
            {
                case 0:
                    {
                        //hammerParameters.OptimizeOrDefaultChoise = OptimizeOrDefaultMassSpacing.Default;
                        hammerParameters.FilteringMethod = HammerFilteringMethod.Cluster;
                        hammerParameters.Iterations = 0;
                    }
                    break;
                case 1:
                    {
                        //hammerParameters.OptimizeOrDefaultChoise = OptimizeOrDefaultMassSpacing.Default;
                        hammerParameters.FilteringMethod = HammerFilteringMethod.Threshold;
                        hammerParameters.ThresholdSigmaMultiplier = 0; //0 is parasmeter free and uses the average
                        hammerParameters.Iterations = 0;
                    }
                    break;
                case 2:
                    {
                        // hammerParameters.OptimizeOrDefaultChoise = OptimizeOrDefaultMassSpacing.Optimize;
                        hammerParameters.FilteringMethod = HammerFilteringMethod.Cluster;
                        hammerParameters.Iterations = 1;
                    }
                    break;
                case 3:
                    {
                        //hammerParameters.OptimizeOrDefaultChoise = OptimizeOrDefaultMassSpacing.Optimize;
                        hammerParameters.FilteringMethod = HammerFilteringMethod.Threshold;
                        hammerParameters.ThresholdSigmaMultiplier = 0; //0 is parasmeter free and uses the average
                        hammerParameters.Iterations = 1;
                    }
                    break;
                case 4:
                    {
                        ////hammerParameters.OptimizeOrDefaultChoise = OptimizeOrDefaultMassSpacing.Optimize;
                        //hammerParameters.ThresholdOrClusterChoise = HammerFilteringMethod.Cluster;
                        //hammerParameters.Iterations = -1;

                        hammerParameters.FilteringMethod = HammerFilteringMethod.SmartCluster;
                        hammerParameters.MassTolleranceSigmaMultiplier = 2;
                        hammerParameters.Iterations = 0;
                    }
                    break;
            }
        }
    }
}
