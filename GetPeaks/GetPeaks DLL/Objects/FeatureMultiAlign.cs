using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Objects
{
    public class FeatureMultiAlign
    {
        public int Feature_ID { get; set; }
        public int Dataset_ID { get; set; }
        public int Cluster_ID { get; set; }
        public int Conformation_ID { get; set; }
        public double Mass { get; set; }
        public double Mass_Calibrated { get; set; }
        public double NET { get; set; }
        public double MZ { get; set; }
        public int Scan_LC { get; set; }
        public int Scan_LC_Start { get; set; }
        public int Scan_LC_End { get; set; }
        public int Scan_LC_Aligned { get; set; }
        public int Charge { get; set; }
        public double Abundance_Max { get; set; }
        public double Abundance_Sum { get; set; }
        public double Drift_Time { get; set; }
        public double Average_Interference_Score { get; set; }
        public double Average_Decon_Fit_Score { get; set; }
        public int UMC_Member_Count { get; set; }
        public int Saturated_Member_Count { get; set; }
    }
}
