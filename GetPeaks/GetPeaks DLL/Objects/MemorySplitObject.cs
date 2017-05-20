using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Objects
{
    [Serializable]
    public class MemorySplitObject
    {
        /// <summary>
        /// how many blocks to break the eluting peak spectra processing into
        /// </summary>
        public int NumberOfBlocks { get; set; }

        /// <summary>
        /// which block to work on
        /// </summary>
        public int BlockNumber { get; set; }
    }
}
