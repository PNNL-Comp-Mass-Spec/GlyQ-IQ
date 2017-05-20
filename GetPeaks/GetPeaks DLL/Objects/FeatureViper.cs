using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Objects
{
    public class FeatureViper:FeatureAbstract
    {  
        public int UMCIndex { get; set; }
        //public int ScanStart { get; set; }
        //public int ScanEnd { get; set; }
        public int ScanClassRep { get; set; }
        public double NETClassRep { get; set; }
        //public double UMCMonoMW { get; set; }
        public double UMCMWStDev { get; set; }
        public double UMCMWMin { get; set; }
        public double UMCMWMax { get; set; }
        //public double UMCAbundance { get; set; }
        public double UMCClassRepAbundance { get; set; }
        public int ClassStatsChargeBasis { get; set; }
        //public int ChargeStateMin { get; set; }
        //public int ChargeStateMax { get; set; }
        public double UMCMZForChargeBasis { get; set; }
        public int UMCMemberCount { get; set; }
        public int UMCMemberCountUsedForAbu { get; set; }
        public float UMCAverageFit { get; set; }
        public int MassShiftPPMClassRep { get; set; }
        public int PairIndex { get; set; }
        public int PairMemberType { get; set; }
        public int ExpressionRatio { get; set; }
        public int ExpressionRatioStDev { get; set; }
        public int ExpressionRatioChargeStateBasisCount { get; set; }
        public int ExpressionRatioMemberBasisCount { get; set; }
        public int MultiMassTagHitCount { get; set; }
        public int MassTagID { get; set; }
        public int MassTagMonoMW { get; set; }
        public int MassTagNET { get; set; }
        public int MassTagNETStDev { get; set; }
        public int SLiCScore { get; set; }
        public int DelSLiC { get; set; }
        public int MemberCountMatchingMassTag { get; set; }
        public bool IsInternalStdMatch { get; set; }
        public float PeptideProphetProbability { get; set; }
        public string Peptide { get; set; }
    }
}
