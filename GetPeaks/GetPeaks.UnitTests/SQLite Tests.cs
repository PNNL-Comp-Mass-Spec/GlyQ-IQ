using System;
using System.Collections.Generic;
using System.Text;
using GetPeaks_DLL.SQLite.OneLineCalls;
using NUnit.Framework;
using GetPeaks_DLL.SQLite;
using PNNLOmics.Data.Features;
using System.IO;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.SQLite.DataTransferObjects;
using GetPeaks_DLL.SQLite.TableInformation;
//using FeatureFinder.Data.Maps;
namespace GetPeaks.UnitTests
{
    class SQLite_Tests
    {
        //string fileName = @"L:\PNNL Files\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\Full data set\B_0_1 coreMT_sum5 on 25 divider 28min 24 sec\HammerPeakDetector_8Cores_1_Cores_25divider_A (0).db";
        //string fileName = @"D:\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\Full data set\set01\PD_OrbitrapFilter_1sum_24cores_1divider_True-MULTIMode_0oLevel_1.3PBR_2SNT_10fit_(1).db";
        string fileName = @"L:\PNNL Files\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\Full data set\set01\PD_OrbitrapFilter_1sum_24cores_1divider_True-MULTIMode_0oLevel_1.3PBR_2SNT_10fit_(1).db";

        //private string testCreateFileName = @"V:\test.db";
        private string testCreateFileName = @"K:\test.db";

        [Test]
        public void TestSQLite()
        {
            string dataBaseFileName = @"D:\PNNL\Projects\Rat Gut Brooke\SQLite Breakdown\gly06_ratc1_8.db3";

            ILoadFile loader = new LoadOptions();
            
            //LoadFeatureMultiAlign loader = new LoadFeatureMultiAlign();
            string columnHeaders;

            List<FeatureMultiAlign> featureList = loader.LoadFeatureMultiAlign(dataBaseFileName, out columnHeaders);


            //int UMCIndex = myFirstUMC.Index;
            //int ScanStart = myFirstUMC.ScanLCStart;
            //int ScanEnd = myFirstUMC.ScanLCEnd;
            //int ScanClassRep = myFirstUMC.ScanLCOfMaxAbundance;
            //double NETClassRep = myFirstUMC.NET;
            //double UMCMonoMW = myFirstUMC.MassMonoisotopic;
            //double UMCMWStDev = myFirstUMC.MassMonoisotopicStandardDeviation;
            //double UMCMWMin = myFirstUMC.MZ;
            //double UMCMWMax = myFirstUMC.MZ;
            //int UMCAbundance= myFirstUMC.Abundance;
            //double UMCClassRepAbundance = myFirstUMC.AbundanceSum;
            //int ClassStatsChargeBasis= myFirstUMC.ChargeState;
            //int ChargeStateMin=myFirstUMC.ChargeMinimum;
            //int ChargeStateMax= myFirstUMC.ChargeMaximum;
            //int UMCMZForChargeBasis = myFirstUMC.ChargeState;
            //int UMCMemberCount = 0;
            //int UMCMemberCountUsedForAbu = 0;
            //double UMCAverageFit = myFirstUMC.FitScoreAverage;
            //int MassShiftPPMClassRep = 0;
            //int PairIndex = 0;
            //int PairMemberType = 0;
            //int ExpressionRatio = 0;
            //int ExpressionRatioStDev;
            //int ExpressionRatioChargeStateBasisCount = 0;
            //int ExpressionRatioMemberBasisCount = 0;
            //int MultiMassTagHitCount = 0;
            //int MassTagID = 0;;
            //int MassTagMonoMW = 0;
            //int MassTagNET = 0;
            //int MassTagNETStDev = 0;
            //int SLiC = 0;
            //int Score = 0;
            //int DelSLiC = 0;
            //int MemberCountMatchingMassTag = 0;
            //int IsInternalStdMatch = 0;
            //int PeptideProphetProbability = 0;
            //int Peptide = 0;

            ////myFirstUMC.Abundance = UMCAbundance;
            ////myFirstUMC.AbundanceMaximum = 0;
            ////myFirstUMC.ChargeMaximum = ChargeStateMax;
            ////myFirstUMC.ChargeMinimum = ChargeStateMin;
            ////myFirstUMC.ChargeState = ClassStatsChargeBasis;
            ////myFirstUMC.ConformationFitScore = 0;
            ////myFirstUMC.ConformationIndex = 0;
            ////myFirstUMC.DaltonCorrectionMax = 0;
            ////myFirstUMC.DriftTime = 0;
            ////myFirstUMC.DriftTimeAligned = 0;
            ////myFirstUMC.ElutionTime = 





            //    List<MassTag> massTagList = new List<MassTag>();

        //    loadOrbitrapData(ref umcList, ref massTagList);

        //    Assert.AreEqual(14182, umcList.Count);
        //    Assert.AreEqual(36549, massTagList.Count);

        //    umcList = (from n in umcList where n.ChargeState==3 select n).ToList();


            Assert.AreEqual(413.27166748046875, 44);
                                       
        }

        //public static void WriteLCIMSMSFeatureToFile(IEnumerable<LCIMSMSFeature> lcimsmsFeatureEnumerable, string outputFileName)
        public static void WriteLCIMSMSFeatureToFile(IEnumerable<FeatureMultiAlign> lcimsmsFeatureEnumerable, string outputFileName)
        {
            //string outFileName = outputFileName;
            string outFileName = "myFeatureFile";
            //String baseFileName = Regex.Split(Settings.InputFileName, "_isos")[0];
            String baseFileName = outFileName;
            String outputDirectory = "";

            //if (!Settings.OutputDirectory.Equals(String.Empty))
            //{
            //    outputDirectory = Settings.OutputDirectory + "\\";
            //}

            TextWriter featureWriter = new StreamWriter(outputDirectory + baseFileName + "_LCMSFeatures.txt");
            //TextWriter mapWriter = new StreamWriter(outputDirectory + baseFileName + "_LCMSFeatureToPeakMap.txt");

            StringBuilder labelStringBuilder = new StringBuilder();
            labelStringBuilder.Append("Feature_Index" + "\t");
            labelStringBuilder.Append("Original_Index" + "\t");
            labelStringBuilder.Append("Monoisotopic_Mass" + "\t");
            labelStringBuilder.Append("Average_Mono_Mass" + "\t");
            labelStringBuilder.Append("UMC_MW_Min" + "\t");
            labelStringBuilder.Append("UMC_MW_Max" + "\t");
            labelStringBuilder.Append("Scan_Start" + "\t");
            labelStringBuilder.Append("Scan_End" + "\t");
            labelStringBuilder.Append("Scan" + "\t");
            labelStringBuilder.Append("IMS_Scan" + "\t");
            labelStringBuilder.Append("IMS_Scan_Start" + "\t");
            labelStringBuilder.Append("IMS_Scan_End" + "\t");
            labelStringBuilder.Append("Avg_Interference_Score" + "\t");
            labelStringBuilder.Append("Decon2ls_Fit_Score" + "\t");
            labelStringBuilder.Append("UMC_Member_Count" + "\t");
            labelStringBuilder.Append("Saturated_Member_Count" + "\t");
            labelStringBuilder.Append("Max_Abundance" + "\t");
            labelStringBuilder.Append("Abundance" + "\t");
            labelStringBuilder.Append("Class_Rep_MZ" + "\t");
            labelStringBuilder.Append("Class_Rep_Charge" + "\t");
            labelStringBuilder.Append("Charge_Max" + "\t");
            labelStringBuilder.Append("Drift_Time" + "\t");
            labelStringBuilder.Append("Conformation_Fit_Score" + "\t");
            labelStringBuilder.Append("LC_Fit_Score" + "\t");
            labelStringBuilder.Append("Average_Isotopic_Fit" + "\t");
            labelStringBuilder.Append("Members_Percentage" + "\t");
            labelStringBuilder.Append("Combined_Score");

            featureWriter.WriteLine(labelStringBuilder.ToString());

            //mapWriter.WriteLine("Feature_Index\tPeak_Index\tFiltered_Peak_Index");

            int index = 0;

            foreach (FeatureMultiAlign MultiAlignFeature in lcimsmsFeatureEnumerable)
            {
                MSFeature msFeatureRep = null;

                int maxAbundance = int.MinValue;
                int msFeatureCount = 0;
                int saturatedMSFeatureCount = 0;
                int repMinIMSScan = 0;
                int repMaxIMSScan = 0;
                long totalAbundance = 0;
                double minMass = double.MaxValue;
                double maxMass = double.MinValue;
                double totalMass = 0;
                double totalFit = 0;
                double totalInterferenceScore = 0;
                double totalAbundanceTimesDriftTime = 0;


                //var sortByScanLCQuery = from imsmsFeature in MultiAlignFeature.IMSMSFeatureList
                //                        orderby imsmsFeature.ScanLC ascending
                //                        select imsmsFeature;

                //int scanLCStart = sortByScanLCQuery.First().ScanLC;
                //int scanLCEnd = sortByScanLCQuery.Last().ScanLC;

                //foreach (IMSMSFeature imsmsFeature in sortByScanLCQuery)
                //{
                //    int minIMSScan = int.MaxValue;
                //    int maxIMSScan = int.MinValue;

                //    bool isFeatureRep = false;

                //    foreach (MSFeature msFeature in imsmsFeature.MSFeatureList)
                //    {
                //        String filteredFeatureId = msFeature.FilteredIndex >= 0 ? msFeature.FilteredIndex.ToString() : "";
                //        mapWriter.WriteLine(index + "\t" + msFeature.IndexInFile + "\t" + filteredFeatureId);

                //        if (msFeature.Abundance > maxAbundance)
                //        {
                //            msFeatureRep = msFeature;
                //            maxAbundance = msFeature.Abundance;
                //            isFeatureRep = true;
                //        }

                //        if (msFeature.MassMonoisotopic < minMass) minMass = msFeature.MassMonoisotopic;
                //        if (msFeature.MassMonoisotopic > maxMass) maxMass = msFeature.MassMonoisotopic;

                //        if (msFeature.ScanIMS < minIMSScan) minIMSScan = msFeature.ScanIMS;
                //        if (msFeature.ScanIMS > maxIMSScan) maxIMSScan = msFeature.ScanIMS;

                //        if (msFeature.IsSaturated) saturatedMSFeatureCount++;

                //        totalAbundance += msFeature.Abundance;
                //        totalAbundanceTimesDriftTime += ((double)msFeature.Abundance * msFeature.DriftTime);
                //        totalMass += msFeature.MassMonoisotopic;
                //        totalFit += msFeature.Fit;
                //        totalInterferenceScore += msFeature.InterferenceScore;
                //        msFeatureCount++;
                //    }

                //    if (isFeatureRep)
                //    {
                //        repMinIMSScan = minIMSScan;
                //        repMaxIMSScan = maxIMSScan;
                //    }
                //}

                double averageMass = totalMass / msFeatureCount;
                //double averageFit = 1.0 - ((totalFit / msFeatureCount) / Settings.FitMax);
                double averageInterferenceScore = (totalInterferenceScore / msFeatureCount);
                double averageDecon2lsFit = (totalFit / msFeatureCount);

                //if (float.IsInfinity(MultiAlignFeature.IMSScore) || float.IsNaN(MultiAlignFeature.IMSScore)) MultiAlignFeature.IMSScore = 0;
                //if (float.IsInfinity(MultiAlignFeature.LCScore) || float.IsNaN(MultiAlignFeature.LCScore)) MultiAlignFeature.IMSScore = 0;

                //double memberPercentage = (double)msFeatureCount / (double)MultiAlignFeature.MaxMemberCount;
                //if (double.IsInfinity(memberPercentage) || double.IsNaN(memberPercentage)) memberPercentage = 0.0;

                //double combinedScore = (MultiAlignFeature.IMSScore + averageFit + memberPercentage) / 3.0;
                //if (double.IsInfinity(combinedScore) || double.IsNaN(combinedScore)) combinedScore = 0.0;

                double driftTimeWeightedAverage = totalAbundanceTimesDriftTime / (double)totalAbundance;

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(index + "\t");
                //stringBuilder.Append(MultiAlignFeature.OriginalIndex + "\t");
                stringBuilder.Append(averageMass.ToString("0.00000") + "\t");
                stringBuilder.Append(averageMass.ToString("0.00000") + "\t");
                stringBuilder.Append(minMass.ToString("0.00000") + "\t");
                stringBuilder.Append(maxMass.ToString("0.00000") + "\t");
                //stringBuilder.Append(ScanLCMap.Mapping[scanLCStart] + "\t");
                //stringBuilder.Append(ScanLCMap.Mapping[scanLCEnd] + "\t");
                //stringBuilder.Append(ScanLCMap.Mapping[msFeatureRep.ScanLC] + "\t");
                stringBuilder.Append(msFeatureRep.ScanIMS + "\t");
                stringBuilder.Append(repMinIMSScan + "\t");
                stringBuilder.Append(repMaxIMSScan + "\t");
                stringBuilder.Append(averageInterferenceScore.ToString("0.00000") + "\t");
                stringBuilder.Append(averageDecon2lsFit.ToString("0.00000") + "\t");
                stringBuilder.Append(msFeatureCount + "\t");
                stringBuilder.Append(saturatedMSFeatureCount + "\t");
                stringBuilder.Append(maxAbundance + "\t");
                //if (Settings.UseConformationDetection)
                //{
                //    stringBuilder.Append(MultiAlignFeature.AbundanceSumRaw + "\t");
                //}
                //else
                //{
                    stringBuilder.Append(totalAbundance + "\t");
                //}
                stringBuilder.Append(msFeatureRep.Mz + "\t");
                //stringBuilder.Append(FeatureMultiAlign.Charge + "\t");
                //stringBuilder.Append(FeatureMultiAlign.Charge + "\t");
                //if (Settings.UseConformationDetection)
                //{
                //    stringBuilder.Append(MultiAlignFeature.DriftTime.ToString("0.00000") + "\t");
                //}
                //else
                //{
                //    stringBuilder.Append(driftTimeWeightedAverage.ToString("0.00000") + "\t");
                //}
                //stringBuilder.Append(MultiAlignFeature.IMSScore.ToString("0.00000") + "\t");
                //stringBuilder.Append(MultiAlignFeature.LCScore.ToString("0.00000") + "\t");
                //stringBuilder.Append(averageFit.ToString("0.00000") + "\t");
                //stringBuilder.Append(memberPercentage.ToString("0.00000") + "\t"); // Mem Percent
                //stringBuilder.Append(combinedScore.ToString("0.00000")); // Combined

                featureWriter.WriteLine(stringBuilder.ToString());

                index++;
            }

            featureWriter.Close();
            //mapWriter.Close();
        }

        [Test]
        public void WriteListToSQLite()
        {
            #region set up data

            List<IsotopeObject> isotopePile = new List<IsotopeObject>();
            
            for (int i = 0; i < 3; i++)
            {
                IsotopeObject newIObject = new IsotopeObject();
                newIObject.ExperimentMass = i * 100;
                newIObject.IsotopeIntensityString = "500";
                
                PNNLOmics.Data.Peak iPeak = new PNNLOmics.Data.Peak();
                iPeak.Height = 1000;
                newIObject.IsotopeList.Add(iPeak);

                newIObject.IsotopeMassString = "400";
                newIObject.MonoIsotopicMass = i * 10;

                isotopePile.Add(newIObject);
            }

            List<string> columnNames = new List<string>();
            columnNames.Add("MonoisotopicMass");

            List<int> columnCountValues = new List<int>();
            columnCountValues.Add(1992);

            string tableName = "IsotopeTable";
            string tableNameCountName = "TableInfo";
            int scanNumber = 100;

            List<string> columnheadersforcount = new List<string>();
            columnheadersforcount.Add("MonoCount");
            columnheadersforcount.Add("IsosCount");

            #endregion

            DatabaseAdaptor firstAdaptor = new DatabaseAdaptor(testCreateFileName);
            //this is needed to create the tables

            Object myLock = new object();

            bool didThisWork = DatabaseFramework.Create(testCreateFileName, myLock, columnheadersforcount);

            didThisWork = firstAdaptor.WriteIsotopeStorageMonoListToSQLite(isotopePile, scanNumber, tableName, tableNameCountName, columnheadersforcount);

            Assert.AreEqual(didThisWork,true);
        }

        [Test]
        public void ReadMonoIsosFromSQLIte()
        {
            DatabaseIsotopeObject fromFile;
            DatabaseReader reader = new DatabaseReader();
            reader.readMonoIsotopeData(fileName, 1, out fromFile);



            //L
            //Assert.AreEqual(2562.3395747908094d, fromFile.MonoIsotopicMass);
            
            //D
            Assert.AreEqual(814.20600468792134d, fromFile.MonoIsotopicMass);

          
        }

        [Test]
        public void ReadSizeFromSQLIte()
        {
            
            DatabaseLayer layer = new DatabaseLayer();
            int numberOfEntries = layer.ReadDatabaseSize(fileName, "TableInfo", "MonoCount");

            Assert.AreEqual(981, numberOfEntries);


        }

        [Test]
        ///load all monos from file
        public void CreateListOfAllMonos()
        {
            DatabaseLayer layer = new DatabaseLayer();
            int numberOfEntries = layer.ReadDatabaseSize(fileName, "TableInfo", "MonoCount");

            List<DatabaseIsotopeObject> fromFile;
            DatabaseReader reader = new DatabaseReader();
            List<DatabaseIsotopeObject> monosFromFile = new List<DatabaseIsotopeObject>();

            

            reader.readAllMonoIsotopeData(fileName, numberOfEntries, out fromFile);

            List<int> scanList = new List<int>();
            foreach (DatabaseIsotopeObject dObject in fromFile)
            {
                scanList.Add(dObject.scan_num);
            }
            scanList.Sort();
                //(p => p.MonoIsotopicMass).ToList();

            Assert.AreEqual(981, scanList.Count);
        }
    }

    public static class SQLiteTestsPreprocessedDatabase
    {
        //private const string fileName2 = @"D:\Csharp\0_TestDataFiles\Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_OrbitrapFilter_ClusterDefault_1sum_1cores_1divider_0oLevel_130PBR_20fit_(277).db";
        //private const string fileName2 = @"L:\PNNL Files\CSharp\0_TestDataFiles\Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_OrbitrapFilter_ClusterDefault_1sum_1cores_1divider_0oLevel_130PBR_20fit_(277).db";
        private const string fileName3 = @"D:\Csharp\0_TestDataFiles\DeisotopingDatabaseHi.db";
        private const string fileName2 = @"D:\Csharp\0_TestDataFiles\Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_OrbitrapFilter_ClusterDefault__sum1_1cor_1div_0oLev_130PBR_20fit_(0).db";
        
        
        [Test]
        public static void LoadScanNumbersWithFragmentation()
        {
            List<TableNames> namesOfTables = new List<TableNames>();
            namesOfTables.Add(TableNames.T_Scan_MonoPeaks);
            namesOfTables.Add(TableNames.T_Scan_Peaks);
            namesOfTables.Add(TableNames.T_Scans);
            namesOfTables.Add(TableNames.T_Scans_Precursor_Peaks);
            namesOfTables.Add(TableNames.TableInfo);
            
            string tableNameScans = "T_Scans";
            tableNameScans = fileName2;
            Console.WriteLine(fileName2);
        }

        [Test]
        public static void SK2_ReadSizeFromSQLite2()
        {
            //fileName2 = @"D:\Csharp\0_TestDataFiles\Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_OrbitrapFilter_ClusterDefault__sum1_1cor_1div_0oLev_130PBR_20fit_(0).db";

            string tableName = TableNames.TableInfo.ToString();
            
            DatabaseLayer layer = new DatabaseLayer();
            int numberOfEntries = 0;
            numberOfEntries = layer.ReadDatabaseSize(fileName2, tableName, ColumnNames.T_Scan_Peaks.ToString());

            Assert.AreEqual(523443, numberOfEntries);

            numberOfEntries = layer.ReadDatabaseSize(fileName2, tableName, ColumnNames.T_Scans.ToString());

            Assert.AreEqual(6949, numberOfEntries);

            numberOfEntries = layer.ReadDatabaseSize(fileName2, tableName, ColumnNames.T_Scans_Precursor_Peaks.ToString());

            Assert.AreEqual(5916, numberOfEntries);

            numberOfEntries = layer.ReadDatabaseSize(fileName2, tableName, ColumnNames.T_Scan_MonoPeaks.ToString());
            //103618
            Assert.AreEqual(90003, numberOfEntries);
        }

        [Test]
        public static void ReadPrecursorPeaks()
        {
            //string tableNamePrecursor = TableNames.T_Scans_Precursor_Peaks.ToString();
            //string tableNamePeaks = TableNames.T_Scan_Peaks.ToString();
            //int scan = 613;

            //DatabaseLayer layer = new DatabaseLayer();
            //int numberOfEntries = 0;
            //numberOfEntries = layer.SelectPrecursorAndPeaks(fileName2, tableNamePrecursor, tableNamePeaks, ColumnNames.T_Scans_Precursor_Peaks.ToString(), ColumnNames.T_Scan_Peaks.ToString(), scan);

            //Assert.AreEqual(523443, numberOfEntries);

            int scan = 9;
            string tablename = "T_Scans";
            DatabaseScanObject sampleObject = new DatabaseScanObject();
            DatabaseLayer layer = new DatabaseLayer();
            List<DatabaseScanObject> results = layer.SK_SelectScansByScan(fileName2, scan, tablename, sampleObject);

            Assert.AreEqual(results.Count,1);
        }

        [Test]
        public static void SK_GetPrecursorPeaksFromScan()
        {
            int scan = 613;

            DatabaseLayer layer = new DatabaseLayer();
            int numberOfEntries = 0;
            numberOfEntries = layer.ReadAllPrecursorPeaksFromScan(fileName2, scan);

            Assert.AreEqual(1001, numberOfEntries);

            //old method using the database reader
            //List<DatabaseIsotopeObject> fromFile;
            //DatabaseReader reader = new DatabaseReader();

            //reader.readAllMonoIsotopeData(fileName3, numberOfEntries, out fromFile);

            //Assert.AreEqual(1001, numberOfEntries);
        }

        [Test]
        public static void SK2_GetPrecursorPeakAndTandemMonoViaTandemScan()
        {
            string fileNamex = @"D:\Csharp\0_TestDataFiles\Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_OrbitrapFilter_ClusterDefault__sum1_1cor_1div_0oLev_130PBR_20fit_(0).db";
            int scan = 34;
            PrecursorAndPeaksObject results = GetPrecursorPeakAndTandemMonoPeaks.Read(scan, fileNamex);

            Assert.AreEqual(285.98551137165771d, results.PrecursorPeak.XValue);
            Assert.AreEqual(389.0, results.TandemMonoPeakList[0].Height);
            Assert.AreEqual(5, results.TandemMonoPeakList.Count);
        }

        [Test]
        public static void SK2_GetPrecursorPeakAndTandemViaTandemScan()
        {
            
            //string fileName2 = @"D:\Csharp\0_TestDataFiles\Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_OrbitrapFilter_ClusterDefault__sum1_1cor_1div_0oLev_130PBR_20fit_(0).db";
            int scan = 34;
            PrecursorAndPeaksObject results = GetPrecursorPeakAndTandemPeaks.ReadMZPeaks(scan, fileName2);

            Assert.AreEqual(285.98551137165771d, results.PrecursorPeak.XValue);
            Assert.AreEqual(41.0, results.TandemPeakList[0].Height);
            Assert.AreEqual(57, results.TandemPeakList.Count);
        }

        [Test]
        public static void SK2_GetPrecursorPeakAndTandemMonoAndPeaksViaTandemScan()
        {
            //string fileNamex = @"E:\ScottK\Populator\Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_OrbitrapFilter_ClusterDefault__sum1_1cor_1div_0oLev_130PBR_20fit_(0).db";
            string fileNamex = @"D:\Csharp\0_TestDataFiles\Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_OrbitrapFilter_ClusterDefault__sum1_1cor_1div_0oLev_130PBR_20fit_(0).db";
            int scan = 34;
            //PrecursorAndPeaksObject results = GetPrecursorPeakAndTandemPeaksAndMonoPeaks.Read(scan, fileNamex);

            PrecursorAndPeaksObject resultsPeaks = GetPrecursorPeakAndTandemPeaks.ReadMZPeaks(scan, fileNamex);

            PrecursorAndPeaksObject resultsMono = GetPrecursorPeakAndTandemMonoPeaks.Read(scan, fileNamex);

            resultsMono.TandemPeakList = resultsPeaks.TandemPeakList;
            
            
            Assert.AreEqual(57, resultsMono.TandemPeakList.Count);
            Assert.AreEqual(5, resultsMono.TandemMonoPeakList.Count);
        }

        [Test]
        public static void SK2_GetPrecursorParentPeakViaTandemScan()
        {
            //string fileName2 = @"D:\Csharp\0_TestDataFiles\Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_OrbitrapFilter_ClusterDefault_1sum_1cores_1divider_0oLevel_130PBR_20fit_(277).db";

            int scan = 34;
            List<DatabasePeakProcessedWithMZObject> results = GetThresholdedPeaksFromScan.Read(scan, fileName2);

            Console.WriteLine(results.Count);
            Assert.AreEqual(285.98551137165771d, results[0].XValue);
            Assert.AreEqual(285.99, results[0].XValueRaw);
        }

        [Test]
        public static void SK_GetThresholdedPeaksFromScan()
        {
            //string fileName2 = @"D:\Csharp\0_TestDataFiles\Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_OrbitrapFilter_ClusterDefault_1sum_1cores_1divider_0oLevel_130PBR_20fit_(277).db";
            //fileName2 = @"D:\Csharp\0_TestDataFiles\Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_OrbitrapFilter_ClusterDefault__sum1_1cor_1div_0oLev_130PBR_20fit_(0).db";
            DatabaseLayer layer = new DatabaseLayer();

            int scan = 8;
            string tablename = "T_Scan_Peaks";
            DatabasePeakProcessedObject sampleObject = new DatabasePeakProcessedObject();
            List<DatabasePeakProcessedObject> results8 = layer.SK_ReadProcessedPeak(fileName2, scan, tablename, sampleObject);

            Console.WriteLine(results8.Count);
            Assert.AreEqual(85, results8.Count);

            int scan29 = 29;
            List<DatabasePeakProcessedObject> results29 = layer.SK_ReadProcessedPeak(fileName2, scan29, tablename, sampleObject);
            Console.WriteLine(results29.Count);
            Assert.AreEqual(182, results29.Count);

            int scan613 = 613;
            List<DatabasePeakProcessedObject> results613 = layer.SK_ReadProcessedPeak(fileName2, scan613, tablename, sampleObject);
            Console.WriteLine(results613.Count);
            Assert.AreEqual(33, results613.Count);
            //List<DatabaseIsotopeObject> fromFile;
            //DatabaseReader reader = new DatabaseReader();
            //reader.readAllMonoIsotopeData(fileName3, numberOfEntries, out fromFile);
        }

        [Test]
        public static void SK2_GetMonoPeaksFromScan()
        {
            //string fileName2 = @"D:\Csharp\0_TestDataFiles\Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_OrbitrapFilter_ClusterDefault__sum1_1cor_1div_0oLev_130PBR_20fit_(0).db";

            DatabaseLayer layer = new DatabaseLayer();

            int scan = 8;
            string tablename = "T_Scan_MonoPeaks";
            DatabasePeakProcessedObject sampleObject = new DatabasePeakProcessedObject();
            List<DatabasePeakProcessedObject> results8 = layer.SK_ReadProcessedPeak(fileName2, scan, tablename, sampleObject);

            Console.WriteLine(results8.Count);
            Assert.AreEqual(11, results8.Count);

            int scan29 = 29;
            List<DatabasePeakProcessedObject> results29 = layer.SK_ReadProcessedPeak(fileName2, scan29, tablename, sampleObject);
            Console.WriteLine(results29.Count);
            Assert.AreEqual(27, results29.Count);

            //List<DatabaseIsotopeObject> fromFile;
            //DatabaseReader reader = new DatabaseReader();
            //reader.readAllMonoIsotopeData(fileName3, numberOfEntries, out fromFile);
        }

        [Test]
        public static void SK2_ReadScansViaParentScan()
        {
            //fileName2 = @"D:\Csharp\0_TestDataFiles\Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_OrbitrapFilter_ClusterDefault__sum1_1cor_1div_0oLev_130PBR_20fit_(0).db";
            
            int scan = 1;
            string tablename = "T_Scans";
            DatabaseScanObject sampleObject = new DatabaseScanObject();
            DatabaseLayer layer = new DatabaseLayer();
            List<DatabaseScanObject> results = layer.SK_SelectScansByParentScan(fileName2, scan, tablename, sampleObject);

            Assert.AreEqual(6, results.Count);
            Assert.AreEqual(1292, results[2].Peaks);
        }

        [Test]
        public static void SK2_ReadScansViaScan()
        {
            //fileName2 = @"D:\Csharp\0_TestDataFiles\Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_OrbitrapFilter_ClusterDefault__sum1_1cor_1div_0oLev_130PBR_20fit_(0).db";

            int scan = 9;
            //int scan = 4745;

            //List<DatabaseScanCentricObject> results = GetScanInfo.Read(scan, fileName2);
            List<DatabaseScanObject> results = GetScanInfo.Read(scan, fileName2);

            Assert.AreEqual(0, results[0].IndexId);
            Assert.AreEqual(2, results[0].MSLevel);
            Assert.AreEqual(8, results[0].ParentScan);
            Assert.AreEqual(1343, results[0].Peaks);
            Assert.AreEqual(0, results[0].PeaksProcessed);
            Assert.AreEqual("Thresholded", results[0].PeakProcessingLevel);

            //Assert.AreEqual(0, results[0].ScanCentricData.ID);
            //Assert.AreEqual(2, results[0].ScanCentricData.ScanNumLc);
            
        }

        [Test]
        public static void SK2_ReadAllFragmentedScans()
        {
            //string fileName2 = @"D:\Csharp\0_TestDataFiles\Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_OrbitrapFilter_ClusterDefault__sum1_1cor_1div_0oLev_130PBR_20fit_(0).db";

            List<int> results = GetAllFragmentationScanNumbers.Read(fileName2);
            Assert.AreEqual(5916, results.Count);
            
        }

        [Test]
        public static void SK_DoSomething()
        {
            //string fileNamex = @"E:\ScottK\Populator\Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_OrbitrapFilter_ClusterDefault__sum1_1cor_1div_0oLev_130PBR_20fit_(0).db";
            //string fileName2 = @"D:\Csharp\0_TestDataFiles\Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_OrbitrapFilter_ClusterDefault__sum1_1cor_1div_0oLev_130PBR_20fit_(0).db";
            int scan = 34;
            PrecursorAndPeaksObject annotationResults = GetPrecursorPeakAndTandemMonoPeaks.Read(scan, fileName2);

            List<DatabaseScanObject> scanResults = GetScanInfo.Read(scan, fileName2);
            //List<DatabaseScanCentricObject> scanResults = GetScanInfo.Read(scan, fileName2);


            Console.WriteLine(annotationResults.TandemMonoPeakList.Count);
            Console.WriteLine(scanResults.Count);

            Console.WriteLine("END");
            //Console.ReadKey();
        }

        public enum TableNames
        {
            T_Scan_MonoPeaks,
            T_Scan_Peaks,
            T_Scans,
            T_Scans_Precursor_Peaks,
            TableInfo
        }

        public enum ColumnNames
        {
            T_Scan_MonoPeaks,
            T_Scan_Peaks,
            T_Scans,
            T_Scans_Precursor_Peaks,
        }
    }
}
