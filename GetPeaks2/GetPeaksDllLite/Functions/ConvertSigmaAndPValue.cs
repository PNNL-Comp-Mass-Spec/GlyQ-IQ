using System;
using MathNet.Numerics;

namespace GetPeaksDllLite.Functions
{
    public static class ConvertSigmaAndPValue
    {
        public static double SigmaToPvalue(double sigma)
        {            
            double erfArea = SpecialFunctions.Erf(sigma / Math.Sqrt(2));
            //p value is 1-area under curve
            return 1 - erfArea;
        }

        public static double PvalueToSigma(double pValue)
        {
            return SpecialFunctions.ErfInv(1 - pValue) * Math.Sqrt(2);;
        }
    }
}
