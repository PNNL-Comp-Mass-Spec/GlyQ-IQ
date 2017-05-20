using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using PNNLOmics.Data.Features;
using DeconTools.Backend.Core;

namespace GetPeaks_DLL.Functions
{
    public static class ConvertFeaturesToOtherFeatures
    {
        
        /// <summary>
        /// converts a list of IMS features into viper features
        /// </summary>
        /// <param name="IMSFeatures"></param>
        /// <returns></returns>
        public static List<FeatureViper> ConvertViperToIMS(List<FeatureIMS> IMSFeatures)
        {
            List<FeatureViper> loadedViperFeatures = new List<FeatureViper>();

            foreach (FeatureIMS feature in IMSFeatures)
            {
                FeatureViper viperFeature = new FeatureViper();
                loadedViperFeatures.Add(viperFeature);
                viperFeature.ChargeStateMax = feature.ChargeStateMax;
                viperFeature.ChargeStateMin = feature.ChargeStateMin;
                viperFeature.ClassStatsChargeBasis = feature.ChargeStateMin;
                viperFeature.DelSLiC = 0;
                viperFeature.ExpressionRatio = 0;
                viperFeature.ExpressionRatioChargeStateBasisCount = 0;
                viperFeature.ExpressionRatioMemberBasisCount = 0;
                viperFeature.ExpressionRatioStDev = 0;
                viperFeature.IsInternalStdMatch = false;
                viperFeature.MassShiftPPMClassRep = 0;
                viperFeature.MassTagID = 0;
                viperFeature.MassTagMonoMW = 0;
                viperFeature.MassTagNET = 0;
                viperFeature.MassTagNETStDev = 0;
                viperFeature.MemberCountMatchingMassTag = 0;
                viperFeature.MultiMassTagHitCount = 0;
                viperFeature.NETClassRep = 0;
                viperFeature.PairIndex = 0;
                viperFeature.PairMemberType = 0;
                viperFeature.Peptide = "";
                viperFeature.PeptideProphetProbability = 0;
                viperFeature.ScanClassRep = feature.IMS_Scan; ;
                viperFeature.ScanEnd = feature.IMS_Scan_Stop;
                viperFeature.ScanStart = feature.IMS_Scan_Start;
                viperFeature.SLiCScore = 0;
                viperFeature.UMCAbundance = feature.UMCAbundance;
                viperFeature.UMCAverageFit = feature.FitScoreCombined;
                viperFeature.UMCClassRepAbundance = 0;
                viperFeature.UMCIndex = feature.OriginalIndex;
                viperFeature.UMCMemberCount = feature.UMCMemberCount;
                viperFeature.UMCMemberCountUsedForAbu = 0;
                viperFeature.UMCMonoMW = feature.UMCMonoMW;
                viperFeature.UMCMWMax = feature.UMCMWMax;
                viperFeature.UMCMWMin = feature.UMCMWMin;
                viperFeature.UMCMWStDev = 0;
                viperFeature.UMCMZForChargeBasis = feature.MZ;
            }

            return loadedViperFeatures;
        }


        /// <summary>
        /// DeconTools ElutingPeak
        /// </summary>
        /// <param name="ePeak"></param>
        /// <returns></returns>
        public static FeatureLight ConvertElutingPeakToFeatureLite(ElutingPeak ePeak)
        {
            FeatureLight newFeature;
            if (ePeak.ID == 1)
            {
                newFeature = new FeatureLight();

                newFeature.ID = ePeak.ID;
                newFeature.Abundance = (long)ePeak.Intensity;
                newFeature.ChargeState = ePeak.ChargeState;
                newFeature.DriftTime = 99;
                newFeature.MassMonoisotopic = ePeak.Mass;
                newFeature.RetentionTime = ePeak.ScanMaxIntensity;
                newFeature.Score = ePeak.FitScore;
            }
            else
            {
                newFeature = new FeatureLight();
                newFeature.ID = 0;
            }
            return newFeature;
        }

        /// <summary>
        /// Omics ElutingPeak
        /// </summary>
        /// <param name="ePeak"></param>
        /// <returns></returns>
        public static FeatureLight ConvertElutingPeakOmicsToFeatureLight(ElutingPeakOmics ePeak)
        {
            FeatureLight newFeature;
            if (ePeak.ID == 1)
            {
                newFeature = new FeatureLight();

                newFeature.ID = ePeak.ID;
                newFeature.Abundance = (long)ePeak.Intensity;
                newFeature.ChargeState = ePeak.ChargeState;
                newFeature.DriftTime = 99;
                newFeature.MassMonoisotopic = ePeak.Mass;
                newFeature.RetentionTime = ePeak.ScanMaxIntensity;
                newFeature.Score = ePeak.FitScore;
            }
            else
            {
                newFeature = new FeatureLight();
                newFeature.ID = 0;
            }
            return newFeature;
        }



    }
}
