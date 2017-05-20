using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Objects.DifferenceFinderObjects
{
    public class DifferenceObject<T>
    {
        /// <summary>
        /// index of data point of interest
        /// </summary>
        public int IndexData { get; set; }

        /// <summary>
        /// index of data point that is 1 difference from IndexData
        /// </summary>
        public int IndexMatch { get; set; }

        /// <summary>
        /// value associated with IndexData
        /// </summary>
        public T Value1 { get; set; }

        /// <summary>
        /// value associated with IndexMatch
        /// </summary>
        public T Value2 { get; set; }

        /// <summary>
        /// difference value between IndexData and IndexMatch
        /// </summary>
        public T Difference { get; set; }

        /// <summary>
        /// index of difference corresponding to the spot in the difference list
        /// </summary>
        public int DifferenceIndex { get; set; }

        /// <summary>
        /// weather this difference has been used or not
        /// </summary>
        public bool Used { get; set; }
    }
}
