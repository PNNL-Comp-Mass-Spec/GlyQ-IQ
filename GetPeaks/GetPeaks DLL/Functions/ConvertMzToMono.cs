using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Functions
{
    public static class ConvertMzToMono
    {
        public static double Execute(double mZ, int charge, double massproton)
        {
            return mZ * charge - massproton * charge;//mass to charge
        }
    }
}
