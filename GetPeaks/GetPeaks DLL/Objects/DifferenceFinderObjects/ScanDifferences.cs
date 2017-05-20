using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Objects.DifferenceFinderObjects
{
    public class ScanDifferences<T>
    {
        public ScanDifferences()
        {
            DifferencesIsos = new List<DifferenceObjectIsos<T>>();
            KeyIsos = new List<DifferenceObjectIsos<T>>();
        }

        /// <summary>
        /// all the differencs present in this scan
        /// </summary>
        public List<DifferenceObjectIsos<T>> DifferencesIsos { get; set; }

        /// <summary>
        /// differences related to KeyFeature
        /// </summary>
        public List<DifferenceObjectIsos<T>> KeyIsos { get; set; }

        /// <summary>
        /// Scan containing the differences
        /// </summary>
        public int ScanNumber { get; set; }

        /// <summary>
        /// feature associated with this scan number
        /// </summary>
        public FeatureAbstract KeyFeature { get; set; }

        
    }
}
