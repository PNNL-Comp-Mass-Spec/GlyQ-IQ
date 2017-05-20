using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.DataFIFO;

namespace GetPeaks_DLL.Functions
{
    public class ParseStringListToViperFeature
    {
        public void Parse(List<string> stringList, FileIterator.deliminator textDeliminator, out List<FeatureViper> outputFeatureList, out string columnHeadders)
        {
            int startline = 1;//0 is the headder

            outputFeatureList = new List<FeatureViper>();

            string[] wordArray;

            int UMCIndex = 0;
            int ScanStart = 0;
            int ScanEnd = 0;
            int ScanClassRep = 0;
            double NETClassRep = 0;
            double UMCMonoMW = 0;
            double UMCMWStDev = 0;
            double UMCMWMin = 0;
            double UMCMWMax = 0;
            double UMCAbundance = 0;
            double UMCClassRepAbundance = 0;
            int ClassStatsChargeBasis = 0;
            int ChargeStateMin = 0;
            int ChargeStateMax = 0;
            double UMCMZForChargeBasis = 0;
            int UMCMemberCount = 0;
            int UMCMemberCountUsedForAbu = 0;
            float UMCAverageFit = 0;
            int MassShiftPPMClassRep = 0;
            int PairIndex = 0;
            int PairMemberType = 0;
            int ExpressionRatio = 0;
            int ExpressionRatioStDev = 0;
            int ExpressionRatioChargeStateBasisCount = 0;
            int ExpressionRatioMemberBasisCount = 0;
            int MultiMassTagHitCount = 0;
            int MassTagID = 0;
            int MassTagMonoMW = 0;
            int MassTagNET = 0;
            int MassTagNETStDev = 0;
            int SLiCScore = 0;
            int DelSLiC = 0;
            int MemberCountMatchingMassTag = 0;
            bool IsInternalStdMatch = true;
            float PeptideProphetProbability = 0;
            string Peptide = "";


            char spliter;
            switch (textDeliminator)
            {
                case GetPeaks_DLL.DataFIFO.FileIterator.deliminator.Comma:
                    {
                        spliter = ',';
                    }
                    break;
                case GetPeaks_DLL.DataFIFO.FileIterator.deliminator.Tab:
                    {
                        spliter = '\t';
                    }
                    break;
                default:
                    {
                        spliter = ',';
                    }
                    break;
            }

            columnHeadders = stringList[0];
            int length = stringList.Count;
            for (int i = startline; i < length; i++)//i=0 is the headder
            {
                string line = stringList[i];

                FeatureViper newFeature = new FeatureViper();

                wordArray = line.Split(spliter);

                //Tryparse is best and should be fastest
                int.TryParse(wordArray[0], out UMCIndex);
                int.TryParse(wordArray[1], out ScanStart);
                int.TryParse(wordArray[2], out ScanEnd);
                int.TryParse(wordArray[3], out ScanClassRep);
                double.TryParse(wordArray[4], out NETClassRep);
                double.TryParse(wordArray[5], out UMCMonoMW);
                double.TryParse(wordArray[6], out UMCMWStDev);
                double.TryParse(wordArray[7], out UMCMWMin);
                double.TryParse(wordArray[8], out UMCMWMax);
                double.TryParse(wordArray[9], out UMCAbundance);
                double.TryParse(wordArray[10], out UMCClassRepAbundance);
                int.TryParse(wordArray[11], out ClassStatsChargeBasis);
                int.TryParse(wordArray[12], out ChargeStateMin);
                int.TryParse(wordArray[13], out ChargeStateMax);
                double.TryParse(wordArray[14], out UMCMZForChargeBasis);
                int.TryParse(wordArray[15], out UMCMemberCount);
                int.TryParse(wordArray[16], out UMCMemberCountUsedForAbu);
                float.TryParse(wordArray[17], out UMCAverageFit);
                int.TryParse(wordArray[18], out MassShiftPPMClassRep);
                int.TryParse(wordArray[19], out PairIndex);
                int.TryParse(wordArray[20], out PairMemberType);
                int.TryParse(wordArray[21], out ExpressionRatio);
                int.TryParse(wordArray[22], out ExpressionRatioStDev);
                int.TryParse(wordArray[23], out ExpressionRatioChargeStateBasisCount);
                int.TryParse(wordArray[24], out ExpressionRatioMemberBasisCount);
                int.TryParse(wordArray[25], out MultiMassTagHitCount);
                int.TryParse(wordArray[26], out MassTagID);
                int.TryParse(wordArray[27], out MassTagMonoMW);
                int.TryParse(wordArray[28], out MassTagNET);
                int.TryParse(wordArray[29], out MassTagNETStDev);
                int.TryParse(wordArray[30], out SLiCScore);
                int.TryParse(wordArray[31], out DelSLiC);
                int.TryParse(wordArray[32], out MemberCountMatchingMassTag);
                bool.TryParse(wordArray[33], out IsInternalStdMatch);
                float.TryParse(wordArray[34], out PeptideProphetProbability);
                Peptide = wordArray[35];
                //TODO how to import peptide????
                //Peptide = wordArray[36];


                newFeature.UMCIndex = UMCIndex;
                newFeature.ScanStart = ScanStart;
                newFeature.ScanEnd = ScanEnd;
                newFeature.ScanClassRep = ScanClassRep;
                newFeature.NETClassRep = NETClassRep;
                newFeature.UMCMonoMW = UMCMonoMW;
                newFeature.UMCMWStDev = UMCMWStDev;
                newFeature.UMCMWMin = UMCMWMin;
                newFeature.UMCMWMax = UMCMWMax;
                newFeature.UMCAbundance = UMCAbundance;
                newFeature.UMCClassRepAbundance = UMCClassRepAbundance;
                newFeature.ClassStatsChargeBasis = ClassStatsChargeBasis;
                newFeature.ChargeStateMin = ChargeStateMin;
                newFeature.ChargeStateMax = ChargeStateMax;
                newFeature.UMCMZForChargeBasis = UMCMZForChargeBasis;
                newFeature.UMCMemberCount = UMCMemberCount;
                newFeature.UMCMemberCountUsedForAbu = UMCMemberCountUsedForAbu;
                newFeature.UMCAverageFit = UMCAverageFit;
                newFeature.MassShiftPPMClassRep = MassShiftPPMClassRep;
                newFeature.PairIndex = PairIndex;
                newFeature.PairMemberType = PairMemberType;
                newFeature.ExpressionRatio = ExpressionRatio;
                newFeature.ExpressionRatioStDev = ExpressionRatioStDev;
                newFeature.ExpressionRatioChargeStateBasisCount = ExpressionRatioChargeStateBasisCount;
                newFeature.ExpressionRatioMemberBasisCount = ExpressionRatioMemberBasisCount;
                newFeature.MultiMassTagHitCount = MultiMassTagHitCount;
                newFeature.MassTagID = MassTagID;
                newFeature.MassTagMonoMW = MassTagMonoMW;
                newFeature.MassTagNET = MassTagNET;
                newFeature.MassTagNETStDev = MassTagNETStDev;
                newFeature.SLiCScore = SLiCScore;
                newFeature.DelSLiC = DelSLiC;
                newFeature.MemberCountMatchingMassTag = MemberCountMatchingMassTag;
                newFeature.IsInternalStdMatch = IsInternalStdMatch;
                newFeature.PeptideProphetProbability = PeptideProphetProbability;
                newFeature.Peptide = Peptide;


                outputFeatureList.Add(newFeature);
            }
        }
    }
}
