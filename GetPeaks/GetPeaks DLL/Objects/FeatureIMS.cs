using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Objects
{
    public class FeatureIMS:FeatureAbstract
    {
        public int FeatureIndex { get; set; }
        public int OriginalIndex { get; set; }
        //public double UMCMonoMW { get; set; }
        public double UMCMWStDev { get; set; }
        public double UMCMWMin { get; set; }
        public double UMCMWMax { get; set; }

        //public int ScanStart { get; set; }
        //public int ScanEnd { get; set; }
        public double NETClassRep { get; set; }
        public int IMS_Scan { get; set; }
        public int IMS_Scan_Start { get; set; }
        public int IMS_Scan_Stop { get; set; }

        public double IMS_Conformation_Fit_Score { get; set; }
        public double Decon2lsFitScore { get; set; }
        public int UMCMemberCount { get; set; }
        public int SaturatedMemberCount { get; set; }
        public double UMCClassRepAbundance { get; set; }
        //public double UMCAbundance { get; set; }//TODO is this one level below?9-4-12

        public double MZ { get; set; }
        public int ClassRepCharge { get; set; }
        //public int ChargeStateMin { get; set; }
        //public int ChargeStateMax { get; set; }
        public double DriftTime { get; set; }
        public double FitScoreConformation { get; set; }
        public float FitScoreLC { get; set; }
        public float FitScoreIsotopicAvg { get; set; }
        public int MembersPercentage { get; set; }
        public float FitScoreCombined { get; set; }
    }
}
