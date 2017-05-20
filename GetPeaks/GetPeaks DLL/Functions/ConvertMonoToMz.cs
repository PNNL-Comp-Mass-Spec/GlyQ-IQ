using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Functions
{
    public static class ConvertMonoToMz
    {
        public static double Execute(double monoisotopicMass, int charge, double massproton)
        {
            return monoisotopicMass/charge + massproton;//mass to charge
            
        }
    }
}
