using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using OmniFinder.Objects;
using GetPeaks_DLL.Functions;
using GetPeaks_DLL.Objects.ResultsObjects;

namespace GetPeaks_DLL.DataFIFO
{
    public class WriteGlycolyzerToDisk
    {
        public void WriteGlycanFile(string filenameOut, GlycolyzerResults glycolyzerResults, OmniFinderOutput omniResults)
        {
            filenameOut = filenameOut.TrimEnd('t');
            filenameOut = filenameOut.TrimEnd('x');
            filenameOut = filenameOut.TrimEnd('t');
            filenameOut = filenameOut.TrimEnd('.');
            string glycanFileNameOut = filenameOut + "_LCMSGlycans.xls";
            string header = System.DateTime.Today.Month + "\\" + System.DateTime.Today.Day + "\\" + System.DateTime.Today.Year + "\t" + filenameOut;
            string columnHeader = "ExpMass\tCalcMass\tPPM\tAbundance\t";
            foreach (string monosaccharideName in omniResults.CompositionHeaders)
            {
                columnHeader += monosaccharideName + "\t";
            }
            columnHeader += "ScanMin\tScanMax\tChargeMin\tChargeMax\tIsomers";

            List<string> dataToWrite = new List<string>();
            dataToWrite.Add(columnHeader);
            for (int i = 0; i < glycolyzerResults.GlycanHitsInLibraryOrder.Count; i++)
            {

                string line = "";
                line += glycolyzerResults.GlycanHitsInLibraryOrder[i].GlycanHitsExperimentalMass.ToString() + "\t";
                line += glycolyzerResults.GlycanHitsInLibraryOrder[i].GlycanHitsLibraryExactMass.ToString() + "\t";
                line += PPMToPrint(glycolyzerResults, i).ToString() + "\t";
                line += glycolyzerResults.GlycanHitsInLibraryOrder[i].GlycanHitsExperimentalAbundance.ToString() + "\t";
                if (glycolyzerResults.GlycanHitsInLibraryOrder[i].GlycanHitsComposition.ListOfCompositions.Count > 0)
                {
                    foreach (int composition in glycolyzerResults.GlycanHitsInLibraryOrder[i].GlycanHitsComposition.ListOfCompositions)
                    {
                        line += composition.ToString() + "\t";
                    }
                }
                else
                {
                    for (int h = 0; h < glycolyzerResults.GlycanLibraryHeaders.Count; h++)
                    {
                        line += "0\t";
                    }
                }
                line += glycolyzerResults.GlycanHitsInLibraryOrder[i].GlycanHitsExperimentalScanMin.ToString() + "\t";
                line += glycolyzerResults.GlycanHitsInLibraryOrder[i].GlycanHitsExperimentalScanMax.ToString() + "\t";
                line += glycolyzerResults.GlycanHitsInLibraryOrder[i].GlycanHitsExperimentalChargeMin.ToString() + "\t";
                line += glycolyzerResults.GlycanHitsInLibraryOrder[i].GlycanHitsExperimentalChargeMax.ToString() + "\t";
                line += glycolyzerResults.GlycanHitsInLibraryOrder[i].GlycanHitsExperimentalNumberOfIsomers.ToString();

                dataToWrite.Add(line);
            }

            StringListToDisk writer = new StringListToDisk();
            writer.toDiskStringList(glycanFileNameOut, dataToWrite, header);
        }

        

        public void WriteViperFeatureFile(string filenameOut, GlycolyzerResults glycolyzerResults)
        //public void WriteViperFeatureFile(string filenameOut, string folder, GlycolyzerResults glycolyzerResults)
        {
            
            filenameOut = filenameOut.TrimEnd('t');
            filenameOut = filenameOut.TrimEnd('x');
            filenameOut = filenameOut.TrimEnd('t');
            filenameOut = filenameOut.TrimEnd('.');
            string featureFileNameOut = filenameOut + "_LCMSFeatures.txt";

            List<string> headderList = new List<string>();
            headderList.Add("UMCIndex");
            headderList.Add("ScanStart");
            headderList.Add("ScanEnd");
            headderList.Add("ScanClassRep");
            headderList.Add("NETClassRep");
            headderList.Add("UMCMonoMW");
            headderList.Add("UMCMWStDev");
            headderList.Add("UMCMWMin");
            headderList.Add("UMCMWMax");
            headderList.Add("UMCAbundance");
            headderList.Add("UMCClassRepAbundance");
            headderList.Add("ClassStatsChargeBasis");
            headderList.Add("ChargeStateMin");
            headderList.Add("ChargeStateMax");
            headderList.Add("UMCMZForChargeBasis");
            headderList.Add("UMCMemberCount");
            headderList.Add("UMCMemberCountUsedForAbu");
            headderList.Add("UMCAverageFit");
            headderList.Add("MassShiftPPMClassRep");
            headderList.Add("PairIndex");
            headderList.Add("PairMemberType");
            headderList.Add("ExpressionRatio");
            headderList.Add("ExpressionRatioStDev");
            headderList.Add("ExpressionRatioChargeStateBasisCount");
            headderList.Add("ExpressionRatioMemberBasisCount");
            headderList.Add("MultiMassTagHitCount");
            headderList.Add("MassTagID");
            headderList.Add("MassTagMonoMW");
            headderList.Add("MassTagNET");
            headderList.Add("MassTagNETStDev");
            headderList.Add("SLiC Score");
            headderList.Add("DelSLiC");
            headderList.Add("MemberCountMatchingMassTag");
            headderList.Add("IsInternalStdMatch");
            headderList.Add("PeptideProphetProbability");
            headderList.Add("Peptide");


            string header = "";
            foreach(string columnName in headderList)
            {
                header+=columnName+"\t";
            }
            
            List<string> dataToWrite = new List<string>();
            for (int i = 0; i < glycolyzerResults.GlycanHits.Count; i++)
            {
                FeatureViper feature = (FeatureViper)glycolyzerResults.GlycanHits[i].GlycanHitsExperimentalFeature;

                string line = "";
                string splitter = "\t";
                line += feature.UMCIndex.ToString() + splitter;
                line += feature.ScanStart.ToString() + splitter;
                line += feature.ScanEnd.ToString() + splitter;
                line += feature.ScanClassRep.ToString() + splitter;
                line += feature.NETClassRep.ToString() + splitter;
                line += feature.UMCMonoMW.ToString() + splitter;
                line += feature.UMCMWStDev.ToString() + splitter;
                line += feature.UMCMWMin.ToString() + splitter;
                line += feature.UMCMWMax.ToString() + splitter;
                line += feature.UMCAbundance.ToString() + splitter;
                line += feature.UMCClassRepAbundance.ToString() + splitter;
                line += feature.ClassStatsChargeBasis.ToString() + splitter;
                line += feature.ChargeStateMin.ToString() + splitter;
                line += feature.ChargeStateMax.ToString() + splitter;
                line += feature.UMCMZForChargeBasis.ToString() + splitter;
                line += feature.UMCMemberCount.ToString() + splitter;
                line += feature.UMCMemberCountUsedForAbu.ToString() + splitter;
                line += feature.UMCAverageFit.ToString() + splitter;
                line += feature.MassShiftPPMClassRep.ToString() + splitter;
                line += feature.PairIndex.ToString() + splitter;
                line += feature.PairMemberType.ToString() + splitter;
                line += feature.ExpressionRatio.ToString() + splitter;
                line += feature.ExpressionRatioStDev.ToString() + splitter;
                line += feature.ExpressionRatioChargeStateBasisCount.ToString() + splitter;
                line += feature.ExpressionRatioMemberBasisCount.ToString() + splitter;
                line += feature.MultiMassTagHitCount.ToString() + splitter;
                line += feature.MassTagID.ToString() + splitter;
                line += feature.MassTagMonoMW.ToString() + splitter;
                line += feature.MassTagNET.ToString() + splitter;
                line += feature.MassTagNETStDev.ToString() + splitter;
                line += feature.SLiCScore.ToString() + splitter;
                line += feature.DelSLiC.ToString() + splitter;
                line += feature.MemberCountMatchingMassTag.ToString() + splitter;
                line += feature.IsInternalStdMatch.ToString() + splitter;
                line += feature.PeptideProphetProbability.ToString() + splitter;
                if (feature.Peptide != null)
                {
                    line += feature.Peptide.ToString() + splitter;
                }
                else
                {
                    line += "" + splitter;
                }

                dataToWrite.Add(line);
            }

            StringListToDisk writer = new StringListToDisk();
            writer.toDiskStringList(featureFileNameOut, dataToWrite, header);
            //writer.toDiskStringList(folder + featureFileNameOut, dataToWrite, header);
        }

        public void WriteIMSFeatureFile(string filenameOut, GlycolyzerResults glycolyzerResults)
        //public void WriteIMSFeatureFile(string filenameOut, string folder, GlycolyzerResults glycolyzerResults)
        {

            filenameOut = filenameOut.TrimEnd('t');
            filenameOut = filenameOut.TrimEnd('x');
            filenameOut = filenameOut.TrimEnd('t');
            filenameOut = filenameOut.TrimEnd('.');
            string featureFileNameOut = filenameOut + "_LCMSFeatures.txt";

            List<string> headderList = new List<string>();
            headderList.Add("Feature_Index");
            headderList.Add("Original_Index");
            headderList.Add("Monoisotopic_Mass");
            headderList.Add("Average_Mono_Mass");
            headderList.Add("UMC_MW_Min");
            headderList.Add("UMC_MW_Max");

            headderList.Add("Scan_Start");
            headderList.Add("Scan_End");
            headderList.Add("Scan");
            headderList.Add("IMS_Scan");
            headderList.Add("IMS_Scan_Start");
            headderList.Add("IMS_Scan_End");

            headderList.Add("Avg_Interference_Score");
            headderList.Add("Decon2ls_Fit_Score");
            headderList.Add("UMC_Member_Count");
            headderList.Add("Saturated_Member_Count");
            headderList.Add("Max_Abundance");
            headderList.Add("Abundance");

            headderList.Add("Class_Rep_MZ");
            headderList.Add("Class_Rep_Charge");
            headderList.Add("Charge_Max");
            headderList.Add("Drift_Time");
            headderList.Add("Conformation_Fit_Score");
            headderList.Add("LC_Fit_Score");
            headderList.Add("Average_Isotopic_Fit");
            headderList.Add("Members_Percentage");
            headderList.Add("Combined_Score");




            string header = "";
            foreach (string columnName in headderList)
            {
                header += columnName + "\t";
            }

            List<string> dataToWrite = new List<string>();
            for (int i = 0; i < glycolyzerResults.GlycanHits.Count; i++)
            {
                FeatureIMS feature = (FeatureIMS)glycolyzerResults.GlycanHits[i].GlycanHitsExperimentalFeature;
                //TODO this needs to be converted to IMS feaures better.  Mapping is approximate
                string line = "";
                string splitter = "\t";
                line += i.ToString() + splitter;//must go from 0 to max number for the feature viewer
                line += feature.FeatureIndex.ToString() + splitter;
                line += feature.UMCMonoMW.ToString() + splitter;
                line += feature.UMCMWStDev.ToString() + splitter;
                line += feature.UMCMWMin.ToString() + splitter;
                line += feature.UMCMWMax.ToString() + splitter;

                line += feature.ScanStart.ToString() + splitter;//ScanStart
                line += feature.ScanEnd.ToString() + splitter;//ScanEnd
                line += feature.NETClassRep.ToString() + splitter;//Scan
                line += feature.IMS_Scan_Start.ToString() + splitter;//IMS Scan
                line += feature.IMS_Scan_Start.ToString() + splitter;//IMS Scan Start
                line += feature.IMS_Scan_Stop.ToString() + splitter;//IMS Scan End

                line += feature.IMS_Conformation_Fit_Score.ToString() + splitter;
                line += feature.Decon2lsFitScore.ToString() + splitter;
                line += feature.UMCMemberCount.ToString() + splitter;
                line += feature.SaturatedMemberCount.ToString() + splitter;
                line += feature.UMCClassRepAbundance.ToString() + splitter;
                line += feature.UMCAbundance.ToString() + splitter;

                line += feature.MZ.ToString() + splitter;
                line += feature.ClassRepCharge.ToString() + splitter;
                line += feature.ChargeStateMax.ToString() + splitter;
                line += feature.DriftTime.ToString() + splitter;
                line += feature.FitScoreConformation.ToString() + splitter;
                line += feature.FitScoreLC.ToString() + splitter;
                line += feature.FitScoreIsotopicAvg.ToString() + splitter;
                line += feature.MembersPercentage.ToString() + splitter;
                line += feature.FitScoreCombined.ToString()+splitter;


                dataToWrite.Add(line);
            }

            StringListToDisk writer = new StringListToDisk();
            //writer.toDiskStringList(folder + featureFileNameOut, dataToWrite, header);
            writer.toDiskStringList(featureFileNameOut, dataToWrite, header);
        }


        private static decimal PPMToPrint(GlycolyzerResults glycolyzerResults, int i)
        {
            decimal ppm;
            //if (glycolyzerResults.GlycanUniqueHitsInLibraryOrderExperimentalMasses[i] > 0)
            if (glycolyzerResults.GlycanHitsInLibraryOrder[i].GlycanHitsExperimentalMass > 0)
            {
                ppm = ErrorCalculator.PPM(
                    (decimal)glycolyzerResults.GlycanHitsInLibraryOrder[i].GlycanHitsExperimentalMass,
                    (decimal)glycolyzerResults.GlycanHitsInLibraryOrder[i].GlycanHitsLibraryExactMass);
            }
            else
            {
                ppm = 0;
            }
            ppm = Math.Round(ppm, 1);
            return ppm;
        }
    }
}
