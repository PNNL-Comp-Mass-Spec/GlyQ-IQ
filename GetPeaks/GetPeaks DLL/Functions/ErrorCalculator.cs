using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Functions
{
    public static class ErrorCalculator
    {
        #region standard

        public static decimal PPM(decimal experimentalMass, decimal calculatedMass)
        {
            return (experimentalMass - calculatedMass) / calculatedMass *1000000m;;
        }

        public static double PPM(double experimentalMass, double calculatedMass)
        {
            return (experimentalMass - calculatedMass) / calculatedMass * 1000000; ;
        }

        public static decimal Da(decimal experimentalMass, decimal calculatedMass)
        {
            return experimentalMass - calculatedMass;;
        }

        #endregion

        #region exact under the hood for a double

        public static double PPMExact(double experimentalMass, double calculatedMass)
        {
            return Convert.ToDouble(PPM(Convert.ToDecimal(experimentalMass), Convert.ToDecimal(calculatedMass)));
        }

        public static double DaExact(double experimentalMass, double calculatedMass)
        {
            return Convert.ToDouble(Da(Convert.ToDecimal(experimentalMass), Convert.ToDecimal(calculatedMass)));
        }

        #endregion

        #region absolute value ppm

        public static double PPMAbsolute(double experimental, double actual)
        {
            return Math.Abs(experimental - actual) / actual * 1000000;
        }
        public static decimal PPMAbsolute(decimal experimental, decimal actual)
        {
            return Math.Abs(experimental - actual) / actual * 1000000m;
        }

        #endregion

        #region tolerance conversion

        public static double PPMtoDaTollerance(double ppm, double mass)
        {
            return mass*ppm/1000000;
        }

        #endregion
    }
}
