using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using PNNLOmics.Data.Peaks;

namespace HammerPeakDetector.Comparisons
{
    public class ROCstorage
    {
        public ROCstorage()
        {
            ManualSignal = new SortedDictionary<int, ProcessedPeak>();
            ManualNoise = new SortedDictionary<int, ProcessedPeak>();
            TestSignal = new SortedDictionary<int, ProcessedPeak>();
            TestNoise = new SortedDictionary<int, ProcessedPeak>();
        }

        public SortedDictionary<int, ProcessedPeak> ManualSignal { get; set; }  //Correct Hit

        public SortedDictionary<int, ProcessedPeak> ManualNoise { get; set; } //False Hit

        public SortedDictionary<int, ProcessedPeak> TestSignal { get; set; } // Correct Miss

        public SortedDictionary<int, ProcessedPeak> TestNoise { get; set; } // False Miss
    }
}
