using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQGlyQ.Objects
{
    public class SmallDHResult
    {
        
        public string Code { get; set; }

        public string Scan { get; set; }

        public string GlobalResult { get; set; }

        public string FinalDecision { get; set; }

        public double H_12 { get; set; }

        public double D_12 { get; set; }

        public double HMono { get; set; }

        public double DHratio { get; set; }

        public double DMono { get; set; }
    }
}
