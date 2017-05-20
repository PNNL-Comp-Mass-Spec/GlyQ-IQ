using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;

namespace GetPeaks_DLL.Objects
{
    public class ChargeObject
    {
        public double MonoisotopicMass { get; set; }
        public double MassToCharge { get; set; }
        public int Charge { get; set; }
        public double LowestMassToCharge { get; set; }
        public string EmpericalFormula { get; set; }
        public TargetBase TheoreticalModel { get; set; }
        public int PeakCount { get; set; }
        public double Score { get; set; }

        public ChargeObject()
        {
            EmpericalFormula = "";
        }
    }
}
