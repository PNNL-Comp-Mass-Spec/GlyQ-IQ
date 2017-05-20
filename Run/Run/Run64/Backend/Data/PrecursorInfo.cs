using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Run64.Backend.Data
{
    public class PrecursorInfo
    {
        public int MSLevel { get; set; }
        public double PrecursorMZ { get; set; }
        public float PrecursorIntensity { get; set; }
        public int PrecursorCharge { get; set; }
        public int PrecursorScan { get; set; }

        public PrecursorInfo()
        {
            MSLevel = 1;
        }
    }
}
