using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects.DifferenceFinderObjects;

namespace OrbitrapPeakDetection.Objects
{
    public class ClusterCP<T>
    {
        public List<DifferenceObject<T>> Peaks { get; set; }

        public int Charge { get; set; }

        public double MonoisotopicMass { get; set; }

      
        public ClusterCP()
        {
            Peaks = new List<DifferenceObject<T>>();        
        }

    }

}
