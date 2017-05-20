using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Objects
{
    public class HornTransformParameters
    {
        public double AbsolutePeptideIntensity {get;set;}
        public string AveragineFormula { get; set; }
        public double CCMass { get; set; }
        public bool CheckAllPatternsAgainstCharge1 { get; set; }
        public bool CompleteFit { get; set; }
        public double DeleteIntensityThreshold { get; set; }
        public bool IsActualMonoMZUsed { get; set; }
        public DeconToolsV2.enmIsotopeFitType IsotopeFitType { get; set; }
        public double LeftFitStringencyFactor { get; set; }
        public short MaxCharge { get; set; }
        public double MaxFit { get; set; }
        public double MaxMW { get; set; }
        public double MinMZ { get; set; }
        public double MaxMZ { get; set; }
        public double MinIntensityForScore { get; set; }
            // hornParameters.MinS2N = ??     //TODO:  verify that this is no longer used
        public short NumPeaksForShoulder { get; set; }
        public bool O16O18Media {get;set;}
        public double PeptideMinBackgroundRatio { get; set; }
        public bool ProcessMSMS { get; set; }
        public double RightFitStringencyFactor { get; set; }
        public string TagFormula { get; set; }
        public bool ThrashOrNot { get; set; }
        public bool UseAbsolutePeptideIntensity { get; set; }
        public bool UseMercuryCaching { get; set; }
        public bool UseMZRange { get; set; }
        public bool UseScanRange { get; set; }
        public short NumPeaksUsedInAbundance { get; set; }

        public bool SumSpectraAcrossFrameRange { get; set; }
        public bool SumSpectraAcrossScanRange { get; set; }
        public bool SumSpectra { get; set; }
        public int NumScansToSumOver { get; set; }
        public short NumScansToAdvance { get; set; }

        public int MaxScan { get; set; }
        public int MinScan { get; set; }

        public HornTransformParameters()
        {
            AbsolutePeptideIntensity = 0.0;
            AveragineFormula = "C4.9384 H7.7583 N1.3577 O1.4773 S0.0417";//SK
            DeleteIntensityThreshold = 1.0;
            MaxFit = 0.25;
            MinIntensityForScore = 10;
            MaxCharge = 5;
            MaxMW = 10000.0;
            NumPeaksForShoulder = 4;
            O16O18Media = false;
            PeptideMinBackgroundRatio = 4.0;
            UseAbsolutePeptideIntensity = false;
            ThrashOrNot = true;
            CheckAllPatternsAgainstCharge1 = false;
            CompleteFit = false;
            CCMass = 1.00727649;
            IsotopeFitType = DeconToolsV2.enmIsotopeFitType.AREA;
            UseMercuryCaching = true;
            
            IsActualMonoMZUsed = false;
            LeftFitStringencyFactor = 1.0;
            RightFitStringencyFactor = 1.0;


            //forced parameters to match decon
            MinMZ = 100;
            MaxMZ = 2000;
            MaxScan = 2147483647;
            MinScan = 1;
            ProcessMSMS = false;
                // hornParameters.MinS2N = ??     //TODO:  verify that this is no longer used
            TagFormula = "";
            UseMZRange = false;
            UseScanRange = false;

            SumSpectraAcrossFrameRange = true;
            SumSpectraAcrossScanRange = false;
            SumSpectra = false;
            NumScansToSumOver = 0;
            NumScansToAdvance = 0;

            NumPeaksUsedInAbundance = 1;           
        }
    }
}
