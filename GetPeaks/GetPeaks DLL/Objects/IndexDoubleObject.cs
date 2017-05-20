using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Objects
{
    public class IndexIndexDoubleObject
    {
        /// <summary>
        /// Maps indexes between objects using doubles
        /// </summary>
        /// <param name="indexIn">index in list</param>
        /// <param name="keyIn">index in external array (objects)</param>
        /// <param name="valueIn">double</param>
        public IndexIndexDoubleObject(int indexIn, int keyIn, double valueIn)
        {
            Index = indexIn;
            IndexMap = keyIn;
            Value = valueIn;
        }

        public int Index { get; set; }
        public int IndexMap { get; set; }
        public double Value { get; set; }
    }
}
