using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaksDllLite.CompareAndContrast
{
    public interface IRapidCompare
    {
        void CompareContrast(List<decimal> libraryMasses, List<decimal> dataMasses, CompareResults Results, double tolleranceIN);
        void CompareOnly(List<decimal> libraryMasses, List<decimal> dataMasses, CompareResults Results, double tolleranceIN);

    }
}
