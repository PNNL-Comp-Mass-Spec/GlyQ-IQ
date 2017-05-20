using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HammerPeakDetector.Objects.GetPeaksDifferenceFinder;

namespace HammerPeakDetector.Objects
{
    /// <summary>
    /// Feature of peaks related by m/z value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClusterCP<T>
    {
        /// <summary>
        /// List of difference objects contained in the current cluster
        /// </summary>
        public List<DifferenceObject<T>> Peaks { get; set; }

        /// <summary>
        /// Charge (z) of current cluster
        /// </summary>
        public int Charge { get; set; }

        public double MonoisotopicMass { get; set; }

        public ClusterCP()
        {
            Peaks = new List<DifferenceObject<T>>();
        }
    }
}
