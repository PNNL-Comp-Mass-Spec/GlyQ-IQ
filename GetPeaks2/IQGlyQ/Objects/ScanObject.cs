using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQGlyQ.Objects
{
    public class ScanObject
    {
        public int Start { get; set; }
        public int Stop { get; set; }

        /// <summary>
        /// Max Scan For LC
        /// </summary>
        public int Max { get; set; }

        /// <summary>
        /// Min scan for LC
        /// </summary>
        public int Min { get; set; }

        /// <summary>
        /// buffer for smoothing
        /// </summary>
        public int Buffer { get; set; }

        public int ScansToSum { get; set; }

        public ScanObject(int startScan, int stopScan)
        {
            Start = startScan;
            Stop = stopScan;
        }

        public ScanObject(int startScan, int stopScan, int minScan, int maxScan)
            : this(startScan, stopScan)
        {
            Min = minScan;
            Max = maxScan;
        }

        public ScanObject(int startScan, int stopScan, int minScan, int maxScan, int buffer, int scansToSum)
            : this(startScan, stopScan, minScan, maxScan)
        {
            Buffer = buffer;
            ScansToSum = scansToSum;
        }

        public ScanObject(int startScan, int stopScan, ScanObject baseScanToCopy)
            : this(startScan, stopScan)
        {
            Min = baseScanToCopy.Min;
            Max = baseScanToCopy.Max;
            Buffer = baseScanToCopy.Buffer;
            ScansToSum = baseScanToCopy.ScansToSum;
        }

        public ScanObject(ScanObject baseScanToCopy)
        {
            Start = baseScanToCopy.Start;
            Stop = baseScanToCopy.Stop;
            Min = baseScanToCopy.Min;
            Max = baseScanToCopy.Max;
            Buffer = baseScanToCopy.Buffer;
            ScansToSum = baseScanToCopy.ScansToSum;
        }
    }
}
