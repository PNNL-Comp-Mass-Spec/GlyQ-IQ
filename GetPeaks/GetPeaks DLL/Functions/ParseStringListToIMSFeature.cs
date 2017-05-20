using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.Objects;

namespace GetPeaks_DLL.Functions
{
    public class ParseStringListToIMSFeature
    {
        public void Parse(List<string> stringList, FileIterator.deliminator textDeliminator, out List<FeatureIMS> outputFeatureList, out string columnHeadders)
        {
            int startline = 1;//0 is the headder

            outputFeatureList = new List<FeatureIMS>();

            string[] wordArray;


            #region old (off)
            //int FeatureIndex = 0;
            //int UMCIndex = 0;
            //double UMCMonoMW = 0;
            //double UMCMonoMWAvg = 0;
            //double UMCMWMin = 0;
            //double UMCMWMax = 0;
            //int ScanStart = 0;
            //int ScanEnd = 0;
            //int ScanClassRep = 0;
            //int IMS_Scan = 0;
            //int IMS_Scan_Start = 0;
            //int IMS_Scan_Stop = 0;
            //float FitScoreInterferenceAvg = 0;
            //float FitScoreDecon2ls = 0;
            //int UMCMemberCount = 0;
            //int SaturatedMemberCount = 0;
            //double AbundanceMax = 0;
            //double UMCAbundance = 0;
            //double MZ = 0;
            //int ChargeStateMin = 0;
            //int ChargeStateMax = 0;
            //double DriftTIme = 0;
            //float FitScoreConformation = 0;
            //float FitScoreLC = 0;
            //float FitScoreIsotopeAvg = 0;
            //int MembersPercentage = 0;
            //float FitScoreCombined = 0;
            #endregion

            int FeatureIndex = 0;

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
            int Saturated_Member_Count = 0;
            double Percent_Saturated = 0;
            double UMCAverageFit = 0;
            double MassShiftPPMClassRep = 0;
            double IMS_Drift_Time = 0;
            double IMS_Conformation_Fit_Score = 0;
            int PairIndex = 0;
            string PairMemberType = "";
            double ExpressionRatio = 0;
            double ExpressionRatioStDev = 0;
            double ExpressionRatioChargeStateBasisCount = 0;
            double ExpressionRatioMemberBasisCount = 0;
            int MultiMassTagHitCount = 0;
            int MassTagID = 0;
            double MassTagMonoMW = 0;
            double MassTagNET = 0;
            double MassTagNETStDev = 0;
            double SLiCScore = 0;
            double DelSLiC = 0;
            double MemberCountMatchingMassTag = 0;
            double IsInternalStdMatch = 0;
            double PeptideProphetProbability = 0;
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

                FeatureIMS newFeature = new FeatureIMS();

                wordArray = line.Split(spliter);

                //Tryparse is best and should be fastest

                #region old (off)
                

                #endregion

                FeatureIndex = i;

                switch (wordArray.Length)
                {
                    case 40:
                        {
                            int.TryParse(wordArray[0], out UMCIndex);//UMCIndex
                            int.TryParse(wordArray[1], out ScanStart);//ScanStart
                            int.TryParse(wordArray[2], out ScanEnd);//ScanEnd
                            int.TryParse(wordArray[3], out ScanClassRep);//ScanClassRep
                            double.TryParse(wordArray[4], out NETClassRep);//NETClassRep
                            double.TryParse(wordArray[5], out UMCMonoMW);//UMCMonoMW
                            double.TryParse(wordArray[6], out UMCMWStDev);//UMCMWStDev
                            double.TryParse(wordArray[7], out UMCMWMin);//UMCMWMin
                            double.TryParse(wordArray[8], out UMCMWMax);//UMCMWMax
                            double.TryParse(wordArray[9], out UMCAbundance);//UMCAbundance
                            double.TryParse(wordArray[10], out UMCClassRepAbundance);//UMCClassRepAbundance
                            int.TryParse(wordArray[11], out ClassStatsChargeBasis);//ClassStatsChargeBasis
                            int.TryParse(wordArray[12], out ChargeStateMin);//ChargeStateMin
                            int.TryParse(wordArray[13], out ChargeStateMax);//ChargeStateMax
                            double.TryParse(wordArray[14], out UMCMZForChargeBasis);//UMCMZForChargeBasis
                            int.TryParse(wordArray[15], out UMCMemberCount);//UMCMemberCount
                            int.TryParse(wordArray[16], out UMCMemberCountUsedForAbu);//UMCMemberCountUsedForAbu
                            int.TryParse(wordArray[17], out Saturated_Member_Count);//Saturated_Member_Count
                            double.TryParse(wordArray[18], out Percent_Saturated);//Percent_Saturated
                            double.TryParse(wordArray[19], out UMCAverageFit);//UMCAverageFit
                            double.TryParse(wordArray[20], out MassShiftPPMClassRep);//MassShiftPPMClassRep
                            double.TryParse(wordArray[21], out IMS_Drift_Time);//IMS_Drift_Time
                            double.TryParse(wordArray[22], out IMS_Conformation_Fit_Score);//IMS_Conformation_Fit_Score
                            int.TryParse(wordArray[23], out PairIndex);//PairIndex
                            PairMemberType = wordArray[24];//PairMemberType
                            double.TryParse(wordArray[25], out ExpressionRatio);//ExpressionRatio
                            double.TryParse(wordArray[26], out ExpressionRatioStDev);//ExpressionRatioStDev
                            double.TryParse(wordArray[27], out ExpressionRatioChargeStateBasisCount);//ExpressionRatioChargeStateBasisCount
                            double.TryParse(wordArray[28], out ExpressionRatioMemberBasisCount);//ExpressionRatioMemberBasisCount
                            int.TryParse(wordArray[29], out MultiMassTagHitCount);//MultiMassTagHitCount
                            int.TryParse(wordArray[30], out MassTagID);//MassTagID
                            double.TryParse(wordArray[31], out MassTagMonoMW);//MassTagMonoMW
                            double.TryParse(wordArray[32], out MassTagNET);//MassTagNET
                            double.TryParse(wordArray[33], out MassTagNETStDev);//MassTagNETStDev
                            double.TryParse(wordArray[34], out SLiCScore);//SLiC Score
                            double.TryParse(wordArray[35], out DelSLiC);//DelSLiC
                            double.TryParse(wordArray[36], out MemberCountMatchingMassTag);//MemberCountMatchingMassTag
                            double.TryParse(wordArray[37], out IsInternalStdMatch);//IsInternalStdMatch
                            double.TryParse(wordArray[38], out PeptideProphetProbability);//PeptideProphetProbability
                            Peptide = wordArray[39];//Peptide

                            newFeature.FeatureIndex = FeatureIndex;
                            newFeature.OriginalIndex = UMCIndex;
                            newFeature.UMCMonoMW = UMCMonoMW;
                            newFeature.UMCMWStDev = UMCMWStDev;
                            newFeature.UMCMWMin = UMCMWMin;
                            newFeature.UMCMWMax = UMCMWMax;
                            newFeature.ScanStart = ScanStart;
                            newFeature.ScanEnd = ScanEnd;
                            newFeature.NETClassRep = NETClassRep;
                            newFeature.IMS_Scan = ScanClassRep;
                            newFeature.IMS_Scan_Start = ScanStart;
                            newFeature.IMS_Scan_Stop = ScanEnd;
                            newFeature.IMS_Conformation_Fit_Score = IMS_Conformation_Fit_Score;
                            newFeature.Decon2lsFitScore = UMCAverageFit;
                            newFeature.UMCMemberCount = UMCMemberCount;
                            newFeature.SaturatedMemberCount = Saturated_Member_Count;
                            newFeature.UMCClassRepAbundance = UMCClassRepAbundance;
                            newFeature.UMCAbundance = UMCAbundance;
                            newFeature.MZ = UMCMZForChargeBasis;
                            newFeature.ChargeStateMin = ChargeStateMin;
                            newFeature.ChargeStateMax = ChargeStateMax;
                            newFeature.DriftTime = IMS_Drift_Time;
                            newFeature.FitScoreConformation = IMS_Conformation_Fit_Score;
                            break;
                        }
                    case 36:
                        {
                            Console.WriteLine("Old feature file format(36).  Press Key to Continue");

                            //Console.ReadKey();
                            int.TryParse(wordArray[0], out UMCIndex);//UMCIndex
                            int.TryParse(wordArray[1], out ScanStart);//ScanStart
                            int.TryParse(wordArray[2], out ScanEnd);//ScanEnd
                            int.TryParse(wordArray[3], out ScanClassRep);//ScanClassRep
                            double.TryParse(wordArray[4], out NETClassRep);//NETClassRep
                            double.TryParse(wordArray[5], out UMCMonoMW);//UMCMonoMW
                            double.TryParse(wordArray[6], out UMCMWStDev);//UMCMWStDev
                            double.TryParse(wordArray[7], out UMCMWMin);//UMCMWMin
                            double.TryParse(wordArray[8], out UMCMWMax);//UMCMWMax
                            double.TryParse(wordArray[9], out UMCAbundance);//UMCAbundance
                            double.TryParse(wordArray[10], out UMCClassRepAbundance);//UMCClassRepAbundance
                            int.TryParse(wordArray[11], out ClassStatsChargeBasis);//ClassStatsChargeBasis
                            int.TryParse(wordArray[12], out ChargeStateMin);//ChargeStateMin
                            int.TryParse(wordArray[13], out ChargeStateMax);//ChargeStateMax
                            double.TryParse(wordArray[14], out UMCMZForChargeBasis);//UMCMZForChargeBasis
                            int.TryParse(wordArray[15], out UMCMemberCount);//UMCMemberCount
                            int.TryParse(wordArray[16], out UMCMemberCountUsedForAbu);//UMCMemberCountUsedForAbu
                            double.TryParse(wordArray[17], out UMCAverageFit);//UMCAverageFit
                            double.TryParse(wordArray[18], out MassShiftPPMClassRep);//MassShiftPPMClassRep
                            int.TryParse(wordArray[19], out PairIndex);//PairIndex
                            PairMemberType = wordArray[20];//PairMemberType
                            double.TryParse(wordArray[21], out ExpressionRatio);//ExpressionRatio
                            double.TryParse(wordArray[22], out ExpressionRatioStDev);//ExpressionRatioStDev
                            double.TryParse(wordArray[23], out ExpressionRatioChargeStateBasisCount);//ExpressionRatioChargeStateBasisCount
                            double.TryParse(wordArray[24], out ExpressionRatioMemberBasisCount);//ExpressionRatioMemberBasisCount
                            int.TryParse(wordArray[25], out MultiMassTagHitCount);//MultiMassTagHitCount
                            int.TryParse(wordArray[26], out MassTagID);//MassTagID
                            double.TryParse(wordArray[27], out MassTagMonoMW);//MassTagMonoMW
                            double.TryParse(wordArray[28], out MassTagNET);//MassTagNET
                            double.TryParse(wordArray[29], out MassTagNETStDev);//MassTagNETStDev
                            double.TryParse(wordArray[30], out SLiCScore);//SLiC Score
                            double.TryParse(wordArray[31], out DelSLiC);//DelSLiC
                            double.TryParse(wordArray[32], out MemberCountMatchingMassTag);//MemberCountMatchingMassTag
                            double.TryParse(wordArray[33], out IsInternalStdMatch);//IsInternalStdMatch
                            double.TryParse(wordArray[34], out PeptideProphetProbability);//PeptideProphetProbability
                            Peptide = wordArray[35];//Peptide

                            newFeature.FeatureIndex = FeatureIndex;
                            newFeature.OriginalIndex = UMCIndex;
                            newFeature.UMCMonoMW = UMCMonoMW;
                            newFeature.UMCMWStDev = UMCMWStDev;
                            newFeature.UMCMWMin = UMCMWMin;
                            newFeature.UMCMWMax = UMCMWMax;
                            newFeature.ScanStart = ScanStart;
                            newFeature.ScanEnd = ScanEnd;
                            newFeature.NETClassRep = NETClassRep;
                            newFeature.IMS_Scan = ScanClassRep;
                            newFeature.IMS_Scan_Start = ScanStart;
                            newFeature.IMS_Scan_Stop = ScanEnd;
                            newFeature.IMS_Conformation_Fit_Score = IMS_Conformation_Fit_Score;
                            newFeature.Decon2lsFitScore = UMCAverageFit;
                            newFeature.UMCMemberCount = UMCMemberCount;
                            newFeature.SaturatedMemberCount = Saturated_Member_Count;
                            newFeature.UMCClassRepAbundance = UMCClassRepAbundance;
                            newFeature.UMCAbundance = UMCAbundance;
                            newFeature.MZ = UMCMZForChargeBasis;
                            newFeature.ChargeStateMin = ChargeStateMin;
                            newFeature.ChargeStateMax = ChargeStateMax;
                            newFeature.DriftTime = IMS_Drift_Time;
                            newFeature.FitScoreConformation = IMS_Conformation_Fit_Score;
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Old feature file format(26).  Press Key to Continue");

                            Console.ReadKey();
                            int.TryParse(wordArray[0], out FeatureIndex);
                            int.TryParse(wordArray[1], out UMCIndex);
                            double.TryParse(wordArray[2], out UMCMonoMW);
                            //double.TryParse(wordArray[3], out UMCMonoMWAvg);
                            double.TryParse(wordArray[4], out UMCMWMin);
                            double.TryParse(wordArray[5], out UMCMWMax);
                            int.TryParse(wordArray[6], out ScanStart);
                            int.TryParse(wordArray[7], out ScanEnd);
                            int.TryParse(wordArray[8], out ScanClassRep);
                            int.TryParse(wordArray[9], out ScanClassRep);
                            int.TryParse(wordArray[10], out ScanStart);
                            int.TryParse(wordArray[11], out ScanEnd);
                            //float.TryParse(wordArray[12], out FitScoreInterferenceAvg);
                            double.TryParse(wordArray[13], out UMCAverageFit);
                            int.TryParse(wordArray[14], out UMCMemberCount);
                            int.TryParse(wordArray[15], out Saturated_Member_Count);
                            double.TryParse(wordArray[16], out UMCClassRepAbundance);
                            double.TryParse(wordArray[17], out UMCAbundance);
                            double.TryParse(wordArray[18], out UMCMZForChargeBasis);
                            int.TryParse(wordArray[19], out ChargeStateMin);
                            int.TryParse(wordArray[20], out ChargeStateMax);
                            double.TryParse(wordArray[21], out IMS_Drift_Time);
                            //float.TryParse(wordArray[22], out FitScoreConformation);
                            //float.TryParse(wordArray[23], out FitScoreLC);
                            //float.TryParse(wordArray[24], out FitScoreIsotopeAvg);
                            //int.TryParse(wordArray[25], out MembersPercentage);
                            //float.TryParse(wordArray[26], out FitScoreCombined);

                            newFeature.FeatureIndex = FeatureIndex;
                            newFeature.OriginalIndex = UMCIndex;
                            newFeature.UMCMonoMW = UMCMonoMW;
                            newFeature.UMCMWStDev = UMCMWStDev;
                            newFeature.UMCMWMin = UMCMWMin;
                            newFeature.UMCMWMax = UMCMWMax;
                            newFeature.ScanStart = ScanStart;
                            newFeature.ScanEnd = ScanEnd;
                            newFeature.NETClassRep = NETClassRep;
                            newFeature.IMS_Scan = ScanClassRep;
                            newFeature.IMS_Scan_Start = ScanStart;
                            newFeature.IMS_Scan_Stop = ScanEnd;
                            newFeature.IMS_Conformation_Fit_Score = IMS_Conformation_Fit_Score;
                            newFeature.Decon2lsFitScore = UMCAverageFit;
                            newFeature.UMCMemberCount = UMCMemberCount;
                            newFeature.SaturatedMemberCount = Saturated_Member_Count;
                            newFeature.UMCClassRepAbundance = UMCClassRepAbundance;
                            newFeature.UMCAbundance = UMCAbundance;
                            newFeature.MZ = UMCMZForChargeBasis;
                            newFeature.ChargeStateMin = ChargeStateMin;
                            newFeature.ChargeStateMax = ChargeStateMax;
                            newFeature.DriftTime = IMS_Drift_Time;
                            newFeature.FitScoreConformation = IMS_Conformation_Fit_Score;

                            break;
                        }
                }

                outputFeatureList.Add(newFeature);
            }
        }
    }
}
