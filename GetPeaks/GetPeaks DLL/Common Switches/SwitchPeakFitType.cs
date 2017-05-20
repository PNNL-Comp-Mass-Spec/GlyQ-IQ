using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Algorithms.PeakDetection;
using DeconTools.Backend;

namespace GetPeaks_DLL.Common_Switches
{
    public class SwitchPeakFitType
    {
        /// <summary>
        /// set peak type used in omics peak detector
        /// </summary>
        /// <param name="peakFitType"></param>
        /// <returns>enumerated peak type for omics algorithm</returns>
        public static PeakFitType setPeakFitType(Globals.PeakFitType peakFitType)
        {
            PeakFitType setFitType = new PeakFitType();
            switch (peakFitType)
            {
                case Globals.PeakFitType.QUADRATIC:
                    {
                        setFitType = PeakFitType.Parabola;
                        //parametersPeakDetector.CentroidParameters.LowAbundanceFWHMPeakFitType = LowAbundanceFWHMPeakFit.Parabola;
                        //    parametersGetPeaksPeakDetector.CentroidParameters.LowAbundanceFWHMPeakFitType = LowAbundanceFWHMPeakFit.Parabola;
                    }
                    break;
                case Globals.PeakFitType.LORENTZIAN:
                    {
                        setFitType = PeakFitType.Lorentzian;
                        //parametersPeakDetector.CentroidParameters.LowAbundanceFWHMPeakFitType = LowAbundanceFWHMPeakFit.Lorentzian;
                        //    parametersGetPeaksPeakDetector.CentroidParameters.LowAbundanceFWHMPeakFitType = LowAbundanceFWHMPeakFit.Lorentzian;
                    }
                    break;
                default:
                    {
                        setFitType = PeakFitType.Parabola;
                        //parametersPeakDetector.CentroidParameters.LowAbundanceFWHMPeakFitType = LowAbundanceFWHMPeakFit.Parabola;
                        //    parametersGetPeaksPeakDetector.CentroidParameters.LowAbundanceFWHMPeakFitType = LowAbundanceFWHMPeakFit.Parabola;
                    }
                    break;
            }
            return setFitType;
        }
    }
}
