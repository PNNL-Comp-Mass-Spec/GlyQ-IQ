using System;
using System.Collections.Generic;
using GetPeaksDllLite.Objects;
using GetPeaksDllLite.TandemSupport;
using GetPeaksDllLite.UnitTests;
using IQGlyQ.Enumerations;
using IQGlyQ.Objects;
using IQGlyQ.Objects.EverythingIsotope;
using IQGlyQ.Results;
using IQ_X64.Backend.Core;
using IQ_X64.Backend.ProcessingTasks.TargetedFeatureFinders;
using IQ_X64.Backend.ProcessingTasks.TheorFeatureGenerator;
using IQ_X64.Workflows.Core;
using NUnit.Framework;
using IQGlyQ.Processors;
using Run64.Backend.Core;
using Run64.Backend.Data;
using Run64.Backend.Runs;
using YAFMS_DB;

//using ScanCentric = GetPeaks_DLL.Objects.TandemMSObjects.ScanCentric;

namespace IQGlyQ.UnitTesting
{
    public class YAFMS_DB_UnitTestsIQ
    {
        //string testFolderGlobal = @"\\picfs\projects\DMS\ScottK\";
        //string testFolderGlobal = @"C:\Users\kron626\Documents\YAFMSDB\";
        //string testFolderGlobal = @"Y:\ScottK\WorkingResults\";
        string testFolderGlobal = @"E:\ScottK\WorkingResults\";
        //string testFolderGlobal = @"T:\ScottK\WorkingResults\";
        //string testFileDBGlobal = @"YAFMS-DB_Test";

        private string testFileDBGlobal = @"Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12_Sum5";


        [Test]
        public void CycleThroughMS1Test()
        {
            string testFile = @"E:\ScottK\GetPeaks Data\Diabetes_LC\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw";

            InputOutputFileName newFile = new InputOutputFileName();
            newFile.InputFileName = testFile;
            int limitScansTo = 999999;
            int sizeOfDatabase = 0;
            List<Run64.Backend.Data.PrecursorInfo> precursorMetaData;

            GatherDatasetInfo.GetMSLevelandSize(newFile, limitScansTo, out sizeOfDatabase, out precursorMetaData);

            Assert.AreEqual(precursorMetaData.Count, 7389);

            List<int> scanLevels = new List<int>();
            List<int> scanLevelsWithTandem = new List<int>();
            YafmsDbUtilities.ProcessScanSpectraNumbers(newFile, limitScansTo, out precursorMetaData, out scanLevels, out scanLevelsWithTandem);

            //TEST Code
            int count = 0;

            for (int i = 0; i < scanLevelsWithTandem.Count; i++)
            {
                int scan = scanLevelsWithTandem[i]; //+1 to sync with file
                Run64.Backend.Data.PrecursorInfo precursor = precursorMetaData[scanLevelsWithTandem[i]];

                Console.WriteLine("Scan: " + scan + " MSLevel: " + precursor.MSLevel + " Precursor MZ:" + precursor.PrecursorMZ);
                count++;
            }

            Assert.AreEqual(count, scanLevelsWithTandem.Count);
        }

        //[Test]
        //public void ReadAllPeaks()
        //{
        //    string testFolderGlobalUnit = @"D:\Csharp\ConosleApps\LocalServer\IQ\YAFMS-DB\100\";//default unit tests


        //    string testFolder = testFolderGlobalUnit;
        //    string testFileDB = testFileDBGlobal;

        //    string databaseFile = testFolder + testFileDB + "_(0).db";
        //    List<int> scans = GetAllScanNumbers.ReadFromDatabase(databaseFile);

        //    Console.WriteLine(scans.Count);

        //    //ICollection<List<DatabasePeakCentricLiteObject>> peakPile = new Collection<List<DatabasePeakCentricLiteObject>>();

        //    //List<DatabasePeakCentricLiteObject> results;
        //    List<PeakArrays> results = new List<PeakArrays>();
        //    //for(int i=0;i< scans.Count;i++)
        //    for (int i = 0; i < 10; i++)
        //    {
        //        int scan = scans[i];
        //        //int scan = i;
        //        //scan = 135;
        //        results.Add(GetCentroidedPeaks.ReadPeaks(scan, databaseFile));
        //        //peakPile.Add(results);
        //        Console.WriteLine("index is " + i + " and scan is " + scan);
        //    }

        //    Assert.AreEqual(161.25570678710938d, results[3].IntensityArray[5]);

        //    Console.WriteLine("peakPile has " + results.Count + " spectra");
        //}

        [Test]
        public void ReadAllScannumbers()
        {
            string testFolderGlobalUnit = @"D:\Csharp\ConosleApps\LocalServer\IQ\YAFMS-DB\100\";//default unit tests


            //string testFolder = testFolderGlobal;
            //string testFileDB = testFileDBGlobal;

            //string databaseFile = testFolder + testFileDB + "_(0).db";
            //int scan = 0;
            //List<int> results = GetAllScanNumbers.ReadFromDatabase(databaseFile);

            //Console.WriteLine(results.Count);

            //Assert.AreEqual(2, results.Count);

            string testFolder = testFolderGlobalUnit;
            string testFileDB = testFileDBGlobal;

            string databaseFile = testFolder + testFileDB + "_(0).db";
            //string databaseFile = testFolder + testFileDB + "_(V2).db";

            ReadDatabase reader = new ReadDatabase(databaseFile);
            int[] scansArray;
            reader.GetMs1ScanNumbers(out scansArray);

            Console.WriteLine(scansArray.Length);
            Assert.AreEqual(scansArray.Length, 100);

            Assert.AreEqual(scansArray[45], 66);
            Assert.AreEqual(scansArray[99], 138);
        }

        [Test]
        public void ReadPeaksFor2Scans()
        {
            string testFolderGlobalUnit = @"D:\Csharp\ConosleApps\LocalServer\IQ\YAFMS-DB\100\";//default unit tests

            string testFolder = testFolderGlobalUnit;
            string testFileDB = testFileDBGlobal;

            string databaseFile = testFolder + testFileDB + "_(0).db";

            ReadDatabase reader = new ReadDatabase(databaseFile);

            int[] scansArray;
            reader.GetMs1ScanNumbers(out scansArray);

            int scan = scansArray[0];

            double[] mzArray;
            double[] intensityArray;
            double[] widthArray;
            reader.GetPeaksSpectrum(scan, out mzArray, out intensityArray, out widthArray);

            PeakArrays results = new PeakArrays(intensityArray.Length, EnumerationPeaksArrays.MS);
            results.MzArray = mzArray;
            results.IntensityArray = intensityArray;
            //PeakArrays results = GetCentroidedPeaks.ReadPeaks(scan, databaseFile);

            Console.WriteLine(results.IntensityArray.Length);
            Assert.AreEqual(5251, results.IntensityArray.Length);

            Assert.AreEqual(276.45037841796875d, results.MzArray[0]);
            Assert.AreEqual(218.78079223632813d, results.IntensityArray[0]);
            //Assert.AreEqual(0.0033473849762231112d, results.WidthArray[0]);//this may be zero here?


            Assert.AreEqual(284.96087646484375d, results.MzArray[10]);

            scan = scansArray[1];
            //results = GetCentroidedPeaks.ReadPeaks(scan, databaseFile);
            reader.GetPeaksSpectrum(scan, out mzArray, out intensityArray, out widthArray);

            results = new PeakArrays(intensityArray.Length, EnumerationPeaksArrays.MS);
            results.MzArray = mzArray;
            results.IntensityArray = intensityArray;

            Console.WriteLine(results.IntensityArray.Length);
            Assert.AreEqual(276.45037841796875d, results.MzArray[0]);
            Assert.AreEqual(164.08560180664063d, results.IntensityArray[0]);
            Assert.AreEqual(6588, results.IntensityArray.Length);
        }

        [Test]
        public void TestMassSpecGeneration()
        {

            EnumerationIsPic isPic = EnumerationIsPic.IsNotPic;
            PopulateGlobalVariables(isPic);

            string testFile = "";
            testFile = @"E:\ScottK\GetPeaks Data\Diabetes_LC\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw";
            //testFile = @"E:\PNNL Data\2012_09_05 SPIN Q-TOF\Gly08_SQTOF_SP02_3X_C1_12_HPIF20Torr_LPRF96_T160_6Sept12.d";

            //testFile = @"E:\PNNL Data\2012_09_05 SPIN Q-TOF\Gly08_SQTOF_SL26_100_C2_13_HPIF30Torr_LPRF96_T160_18Sept12.d";//work
            //testFile = @"L:\PNNL Files\PNNL Data for Tests\Gly08_SQTOF_SL26_100_C2_13_HPIF30Torr_LPRF96_T160_18Sept12.d";//home
            
            RunFactory factor = new RunFactory();
            Run runIn = factor.CreateRun(testFile);

            ScanObject scanInfo = new ScanObject(2615, 2684);
            //possibleFragmentTarget.ScanInfo.Start = 1648;
            //possibleFragmentTarget.ScanInfo.Stop = 1674;
            scanInfo.Max = runIn.MaxLCScan;
            scanInfo.Min = runIn.MinLCScan;
            scanInfo.ScansToSum = 5;
            scanInfo.Buffer = 9 * 2;

            runIn.ScanSetCollection.Create(runIn, scanInfo.Start, scanInfo.Stop, scanInfo.ScansToSum, 1, false);

            IqResult iQresult = new IqResult(new IqTargetBasic());

            //TEST Code

            iQresult.LCScanSetSelected = Utiliites.ScanSetFromCenterScan(runIn, 2648, scanInfo.ScansToSum);
            

            Run64.Backend.Data.XYData massSpectrum = _msProcessor.DeconMSGeneratorWrapper(runIn, iQresult.LCScanSetSelected);

            Assert.AreEqual(massSpectrum.Xvalues.Length, 56963);

            iQresult.LCScanSetSelected = Utiliites.ScanSetFromStartStop(runIn, scanInfo);

            Run64.Backend.Data.XYData massSpectrum2 = _msProcessor.DeconMSGeneratorWrapper(runIn, iQresult.LCScanSetSelected);

            Assert.AreEqual(massSpectrum2.Xvalues.Length, 56963);
        }

        [Test]
        public void TestMS1ScansetGeneration()
        {
            EnumerationIsPic isPic = EnumerationIsPic.IsNotPic;
            PopulateGlobalVariables(isPic);

            string testFile = "";
            testFile = @"E:\ScottK\GetPeaks Data\Diabetes_LC\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw";
            //testFile = @"E:\PNNL Data\2012_09_05 SPIN Q-TOF\Gly08_SQTOF_SP02_3X_C1_12_HPIF20Torr_LPRF96_T160_6Sept12.d";

            //testFile = @"E:\PNNL Data\2012_09_05 SPIN Q-TOF\Gly08_SQTOF_SL26_100_C2_13_HPIF30Torr_LPRF96_T160_18Sept12.d";//work
            //testFile = @"L:\PNNL Files\PNNL Data for Tests\Gly08_SQTOF_SL26_100_C2_13_HPIF30Torr_LPRF96_T160_18Sept12.d";//home

            RunFactory factor = new RunFactory();
            Run runIn = factor.CreateRun(testFile);

            ScanObject scanInfo = new ScanObject(0, 99999);
            //possibleFragmentTarget.ScanInfo.Start = 1648;
            //possibleFragmentTarget.ScanInfo.Stop = 1674;
            scanInfo.Max = runIn.MaxLCScan;
            scanInfo.Min = runIn.MinLCScan;
            scanInfo.ScansToSum = 5;
            scanInfo.Buffer = 9 * 2;

            runIn.ScanSetCollection.Create(runIn, scanInfo.Start, scanInfo.Stop, scanInfo.ScansToSum, 1, false);

            Assert.AreEqual(runIn.ScanSetCollection.ScanSetList.Count, 3218);
        }

        [Test]
        public void TestPrecursorMS1MS2Generation()
        {
            string testFile = @"E:\ScottK\GetPeaks Data\Diabetes_LC\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw";
            
            InputOutputFileName newFile = new InputOutputFileName();
            newFile.InputFileName = testFile;
            int limitScansTo = 999999;
            int sizeOfDatabase = 0;
            List<PrecursorInfo> precursorMetaData;

            GatherDatasetInfo.GetMSLevelandSize(newFile, limitScansTo, out sizeOfDatabase, out precursorMetaData);

            Assert.AreEqual(precursorMetaData.Count, 7389);

            List<int> scanLevels = new List<int>();
            List<int> scanLevelsWithTandem = new List<int>();
            YafmsDbUtilities.ProcessScanSpectraNumbers(newFile, limitScansTo, out precursorMetaData, out scanLevels, out scanLevelsWithTandem);
            Assert.AreEqual(precursorMetaData.Count, 7389);
            Assert.AreEqual(scanLevels.Count, 7389);
            Assert.AreEqual(scanLevelsWithTandem.Count, 3216);
        }

        ///// <summary>
        ///// testdatabase
        ///// </summary>
        //[Test]
        //public void testSavingTandemTOSQliteOld()
        //{
        //    MSSpectra newSpectra = new MSSpectra(); //parent scan
        //    newSpectra.MSLevel = 7;
        //    newSpectra.PeakProcessingLevel = PeakProcessingLevel.Other;
        //    newSpectra.PeaksProcessed = new List<ProcessedPeak>();

        //    ProcessedPeak newPeak = new ProcessedPeak();
        //    newPeak.Background = 10;
        //    newPeak.Height = 10000000;
        //    newPeak.LocalLowestMinimaHeight = 5;
        //    newPeak.LocalSignalToNoise = 7;
        //    newPeak.MinimaOfHigherMassIndex = 18;
        //    newPeak.MinimaOfLowerMassIndex = 19;
        //    newPeak.SignalToBackground = 12;
        //    newPeak.SignalToNoiseGlobal = 13;
        //    newPeak.SignalToNoiseLocalHighestMinima = 11;
        //    newPeak.Width = 35;
        //    newPeak.XValue = 1000.123456789;

        //    ProcessedPeak newPeak2 = new ProcessedPeak();
        //    newPeak2.Background = 11;
        //    newPeak2.Height = 10000001;
        //    newPeak2.LocalLowestMinimaHeight = 51;
        //    newPeak2.LocalSignalToNoise = 71;
        //    newPeak2.MinimaOfHigherMassIndex = 181;
        //    newPeak2.MinimaOfLowerMassIndex = 191;
        //    newPeak2.SignalToBackground = 121;
        //    newPeak2.SignalToNoiseGlobal = 131;
        //    newPeak2.SignalToNoiseLocalHighestMinima = 111;
        //    newPeak2.Width = 351;
        //    newPeak2.XValue = 1001.123456789;

        //    int scan = 5509;
        //    newSpectra.PeaksProcessed.Add(newPeak);
        //    newSpectra.PeaksProcessed.Add(newPeak2);

        //    string testFolder = @"E:\ScottK\WorkingResults\";
        //    string testFile = @"YAFMS-DB_Test";

        //    //ParametersSQLite sqliteDetails = new ParametersSQLite(@"K:\SQLite\", "PeaksDatabase", 1, 1);
        //    ParametersSQLite sqliteDetails = new ParametersSQLite(testFolder, testFile, 1, 1);


        //    YafmsDbUtilities.SetUpSimplePeaksPage(ref sqliteDetails);
        //    YafmsDbUtilities.SetUpScansRelationshipPage(ref sqliteDetails);

        //    Object databaseLock = new Object();// for creating the main file.  if one thread is used, the lock is not needed
        //    EngineSQLite sqLiteEngineBuilder = new EngineSQLite(databaseLock, 0);
        //    sqliteDetails.CoresPerComputer = 1;
        //    ParalellEngineStation sqlEngineStation = sqLiteEngineBuilder.SetupEngines(sqliteDetails);

        //    EngineSQLite currentEngine = (EngineSQLite)sqlEngineStation.Engines[0];

        //    sqliteDetails.PageName = sqliteDetails.ColumnHeadersCounts[0];
        //    bool didthiswork = currentEngine.WriteProcessedPeakList(currentEngine, newSpectra.PeaksProcessed, sqliteDetails);

        //    List<MSSpectra> SpectraPile = new List<MSSpectra>();
        //    SpectraPile.Add(newSpectra);

        //    sqliteDetails.PageName = sqliteDetails.ColumnHeadersCounts[1];
        //    bool didthiswork2 = currentEngine.WriteScanList(currentEngine, SpectraPile, sqliteDetails);
        //    //    //bool didthiswork = currentEngine.WriteIsosData((EngineSQLite)SQLEngineStation.Engines[0], thrashResults.ResultsFromRunConverted, scanNumber);

        //}

        //[Test]
        //public void Write2Peaks()
        //{
        //    //0.  strings
        //    string testFolder = testFolderGlobal;
        //    string testFileDB = testFileDBGlobal;

        //    string databaseFile = testFolder + testFileDB + "_(0).db";
        //    if (File.Exists(databaseFile))
        //    {

        //        Console.WriteLine("Delete Database");
        //        System.IO.File.Delete(databaseFile);
        //    }
            
        //    //1.  sqliteDetails
        //    ParametersSQLite sqliteDetails = new ParametersSQLite(testFolder, testFileDB, 1, 1);

        //    YafmsDbUtilities.SetUpSimplePeaksPage(ref sqliteDetails);
        //    YafmsDbUtilities.SetUpScansRelationshipPage(ref sqliteDetails);
        //    YafmsDbUtilities.SetUpPrecursorPeakPage(ref sqliteDetails);
        //    YafmsDbUtilities.SetUpMonoisotopicMassPeaksPage(ref sqliteDetails);
        //    YafmsDbUtilities.SetUpPeaksCentricPage(ref sqliteDetails);
        //    YafmsDbUtilities.SetUpScanCentricPage(ref sqliteDetails);

        //    Assert.AreEqual(sqliteDetails.ColumnHeadersCounts[0], "T_Scan_Peaks");
        //    Assert.AreEqual(sqliteDetails.ColumnHeadersCounts[1], "T_Scans");
        //    Assert.AreEqual(sqliteDetails.ColumnHeadersCounts[2], "T_Scans_Precursor_Peaks");
        //    Assert.AreEqual(sqliteDetails.ColumnHeadersCounts[3], "T_Scan_MonoPeaks");
        //    Assert.AreEqual(sqliteDetails.ColumnHeadersCounts[4], "T_Peak_Centric");
        //    Assert.AreEqual(sqliteDetails.ColumnHeadersCounts[5], "T_Scan_Centric");

        //    //2.  engine time
        //    Object databaseLock = new Object();// for creating the main file.  if one thread is used, the lock is not needed
        //    int engineNumber = 0;
        //    EngineSQLite sqLiteEngineBuilder = new EngineSQLite(databaseLock, engineNumber);
        //    sqliteDetails.CoresPerComputer = 1;
        //    ParalellEngineStation sqlEngineStation = sqLiteEngineBuilder.SetupEngines(sqliteDetails);
        //    EngineSQLite currentEngine = (EngineSQLite)sqlEngineStation.Engines[0];

        //    //3.  this is the meta data for this scan
        //    MSSpectra newSpectra = new MSSpectra(); //parent scan
        //    newSpectra.MSLevel = 1;
        //    newSpectra.PeakProcessingLevel = PeakProcessingLevel.Centroided;
        //    newSpectra.PeaksProcessed = new List<ProcessedPeak>();
        //    newSpectra.Scan = 1200;
            
        //    //4. here are the peaks
        //    ProcessedPeak newPeak = new ProcessedPeak();
        //    newPeak.Background = 10;
        //    newPeak.Height = 10000000;
        //    newPeak.LocalLowestMinimaHeight = 5;
        //    newPeak.LocalSignalToNoise = 7;
        //    newPeak.MinimaOfHigherMassIndex = 18;
        //    newPeak.MinimaOfLowerMassIndex = 19;
        //    newPeak.SignalToBackground = 12;
        //    newPeak.SignalToNoiseGlobal = 13;
        //    newPeak.SignalToNoiseLocalHighestMinima = 11;
        //    newPeak.Width = 35;
        //    newPeak.XValue = 1000.123456789;

        //    ProcessedPeak newPeak2 = new ProcessedPeak();
        //    newPeak2.Background = 11;
        //    newPeak2.Height = 10000001;
        //    newPeak2.LocalLowestMinimaHeight = 51;
        //    newPeak2.LocalSignalToNoise = 71;
        //    newPeak2.MinimaOfHigherMassIndex = 181;
        //    newPeak2.MinimaOfLowerMassIndex = 191;
        //    newPeak2.SignalToBackground = 121;
        //    newPeak2.SignalToNoiseGlobal = 131;
        //    newPeak2.SignalToNoiseLocalHighestMinima = 111;
        //    newPeak2.Width = 351;
        //    newPeak2.XValue = 1001.123456789;
         
        //    newSpectra.PeaksProcessed.Add(newPeak);
        //    newSpectra.PeaksProcessed.Add(newPeak2);

        //    List<PeakCentric> centroidedMS1PeaksToWrite = new List<PeakCentric>();
        //    List<PeakCentric> centroidedMS1ThresholdedPeaksToWrite = new List<PeakCentric>();
            
        //    //TEST Code

        //    int idCounterForScan = 10;
        //    int IdCounterForPeakCentric = 5;


        //    //either will contain all centroids or signal
        //    ScanCentric currentLocationInDataset = YafmsDbUtilities.SetLocationInDataset(ref idCounterForScan, newSpectra.MSLevel, newSpectra.Scan, 0);
        //    YafmsDbUtilities.SaveProcessedPeaksAtCentroidLevel(newSpectra, newSpectra.Scan, ref IdCounterForPeakCentric, ref currentLocationInDataset, out centroidedMS1PeaksToWrite, out centroidedMS1ThresholdedPeaksToWrite, currentEngine, sqliteDetails);
            
        //}

        //[Test]
        //public void Write3Scans()
        //{
        //    EnumerationIsPic isPic = EnumerationIsPic.IsNotPic;
        //    PopulateGlobalVariables(isPic);
            
        //    //0.  strings
        //    string testFolder = testFolderGlobal;
        //    string testFileDB = testFileDBGlobal;

        //    string databaseFile = testFolder + testFileDB + "_(0).db";
        //    if (File.Exists(databaseFile))
        //    {

        //        Console.WriteLine("Delete Database");
        //        System.IO.File.Delete(databaseFile);
        //    }

        //    //1.  sqliteDetails
        //    ParametersSQLite sqliteDetails = new ParametersSQLite(testFolder, testFileDB, 1, 1);

        //    YafmsDbUtilities.SetUpSimplePeaksPage(ref sqliteDetails);
        //    YafmsDbUtilities.SetUpScansRelationshipPage(ref sqliteDetails);
        //    YafmsDbUtilities.SetUpPrecursorPeakPage(ref sqliteDetails);
        //    YafmsDbUtilities.SetUpMonoisotopicMassPeaksPage(ref sqliteDetails);
        //    YafmsDbUtilities.SetUpPeaksCentricPage(ref sqliteDetails);
        //    YafmsDbUtilities.SetUpScanCentricPage(ref sqliteDetails);

        //    Assert.AreEqual(sqliteDetails.ColumnHeadersCounts[0], "T_Scan_Peaks");
        //    Assert.AreEqual(sqliteDetails.ColumnHeadersCounts[1], "T_Scans");
        //    Assert.AreEqual(sqliteDetails.ColumnHeadersCounts[2], "T_Scans_Precursor_Peaks");
        //    Assert.AreEqual(sqliteDetails.ColumnHeadersCounts[3], "T_Scan_MonoPeaks");
        //    Assert.AreEqual(sqliteDetails.ColumnHeadersCounts[4], "T_Peak_Centric");
        //    Assert.AreEqual(sqliteDetails.ColumnHeadersCounts[5], "T_Scan_Centric");

        //    //2.  engine time
        //    Object databaseLock = new Object();// for creating the main file.  if one thread is used, the lock is not needed
        //    int engineNumber = 0;
        //    EngineSQLite sqLiteEngineBuilder = new EngineSQLite(databaseLock, engineNumber);
        //    sqliteDetails.CoresPerComputer = 1;
        //    ParalellEngineStation sqlEngineStation = sqLiteEngineBuilder.SetupEngines(sqliteDetails);
        //    EngineSQLite currentEngine = (EngineSQLite)sqlEngineStation.Engines[0];

           
        //    //3.  Run
        //    string testFile = "";
        //    testFile = @"E:\ScottK\GetPeaks Data\Diabetes_LC\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw";
        //    //testFile = @"E:\PNNL Data\2012_09_05 SPIN Q-TOF\Gly08_SQTOF_SP02_3X_C1_12_HPIF20Torr_LPRF96_T160_6Sept12.d";

        //    //testFile = @"E:\PNNL Data\2012_09_05 SPIN Q-TOF\Gly08_SQTOF_SL26_100_C2_13_HPIF30Torr_LPRF96_T160_18Sept12.d";//work
        //    //testFile = @"L:\PNNL Files\PNNL Data for Tests\Gly08_SQTOF_SL26_100_C2_13_HPIF30Torr_LPRF96_T160_18Sept12.d";//home

        //    RunFactory factor = new RunFactory();
        //    Run runIn = factor.CreateRun(testFile);

        //    //3.  scan parameters
        //    FragmentedTargetedWorkflowParametersIQ fragmentedTargetedWorkflowParameters = new FragmentedTargetedWorkflowParametersIQ();
        //    fragmentedTargetedWorkflowParameters.SummingMode = SummingModeEnum.SUMMINGMODE_STATIC;
        //    fragmentedTargetedWorkflowParameters.NumMSScansToSum = 13;

        //    fragmentedTargetedWorkflowParameters.MSParameters.PointsPerShoulder = fragmentedTargetedWorkflowParameters.LCParameters.CalculatePointsPerShoulderAsAFunctionOfSgPoints(fragmentedTargetedWorkflowParameters);
        //    fragmentedTargetedWorkflowParameters.MSParameters.ParametersOmicsPeakCentroid.FWHMPeakFitType = PeakFitType.Parabola;


        //    //4.  scanset
        //    ScanObject scanInfo = new ScanObject(500, 501);
        //    //possibleFragmentTarget.ScanInfo.Start = 1648;
        //    //possibleFragmentTarget.ScanInfo.Stop = 1674;
        //    scanInfo.Max = runIn.MaxLCScan;
        //    scanInfo.Min = runIn.MinLCScan;
        //    scanInfo.ScansToSum = fragmentedTargetedWorkflowParameters.NumMSScansToSum;
        //    scanInfo.Buffer = 9 * 2;

        //    runIn.ScanSetCollection.Create(runIn, scanInfo.Min, scanInfo.Max, scanInfo.ScansToSum, 1, false);

        //    //5.  select scan
        //    int scan = 2648;
        //    ScanSet lcScanSetSelected = Utiliites.ScanSetFromCenterScan(runIn,scan,scanInfo.ScansToSum);


        //    ProcessingParametersMassSpectra massSpecParameters = new ProcessingParametersMassSpectra();
        //    Processors.ProcessorMassSpectra msProcessor = new ProcessorMassSpectra(massSpecParameters);
        //    //6.  get mass spec

        //    //TEST Code

        //    DeconTools.Backend.XYData massSpectrum = msProcessor.DeconMSGeneratorWrapper(runIn, lcScanSetSelected);

        //    List<ProcessedPeak> peaks = msProcessor.Execute(massSpectrum, EnumerationMassSpectraProcessing.OmicsCentroid_Only);

        //    Assert.AreEqual(7415, peaks.Count);

        //    //3.  this is the meta data for this scan
        //    MSSpectra newSpectra = new MSSpectra(); //parent scan
        //    newSpectra.MSLevel = 1;
        //    newSpectra.PeakProcessingLevel = PeakProcessingLevel.Centroided;
        //    newSpectra.PeaksProcessed = peaks;
        //    newSpectra.Scan = scan;

        //    List<PeakCentric> centroidedMS1PeaksToWrite = new List<PeakCentric>();
        //    List<PeakCentric> centroidedMS1ThresholdedPeaksToWrite = new List<PeakCentric>();

        //    int idCounterForScan = 0;//iniitial conditions
        //    int IdCounterForPeakCentric = 0;//iniitial conditions

        //    //TEST Code

        //    //either will contain all centroids or signal
        //    ScanCentric currentLocationInDataset = YafmsDbUtilities.SetLocationInDataset(ref idCounterForScan, newSpectra.MSLevel, newSpectra.Scan, 0);
        //    YafmsDbUtilities.SaveProcessedPeaksAtCentroidLevel(newSpectra, newSpectra.Scan, ref IdCounterForPeakCentric, ref currentLocationInDataset, out centroidedMS1PeaksToWrite, out centroidedMS1ThresholdedPeaksToWrite, currentEngine, sqliteDetails);


        //    //scan 2

        //    scan = 2651;
        //    lcScanSetSelected = Utiliites.ScanSetFromCenterScan(runIn, scan, scanInfo.ScansToSum);

        //    massSpectrum = msProcessor.DeconMSGeneratorWrapper(runIn, lcScanSetSelected);

        //    peaks = msProcessor.Execute(massSpectrum, EnumerationMassSpectraProcessing.OmicsCentroid_Only);

        //    Assert.AreEqual(7396, peaks.Count);

        //    newSpectra = new MSSpectra(); //parent scan
        //    newSpectra.MSLevel = 1;
        //    newSpectra.PeakProcessingLevel = PeakProcessingLevel.Centroided;
        //    newSpectra.PeaksProcessed = peaks;
        //    newSpectra.Scan = scan;

        //    currentLocationInDataset = YafmsDbUtilities.SetLocationInDataset(ref idCounterForScan, newSpectra.MSLevel, newSpectra.Scan, 0);
        //    YafmsDbUtilities.SaveProcessedPeaksAtCentroidLevel(newSpectra, newSpectra.Scan, ref IdCounterForPeakCentric, ref currentLocationInDataset, out centroidedMS1PeaksToWrite, out centroidedMS1ThresholdedPeaksToWrite, currentEngine, sqliteDetails);


        //    //scan 3
        //    scan = 6;  //this one has a special case in which the FWHM is a specia case and was a negative in the square root in omics at mx 1276.03
        //    lcScanSetSelected = Utiliites.ScanSetFromCenterScan(runIn, scan, scanInfo.ScansToSum);

        //    massSpectrum = msProcessor.DeconMSGeneratorWrapper(runIn, lcScanSetSelected);

        //    peaks = msProcessor.Execute(massSpectrum, EnumerationMassSpectraProcessing.OmicsCentroid_Only);

        //    List<double> widths = new List<double>();
        //    foreach (var peak in peaks)
        //    {
        //        widths.Add(peak.Width);
        //    }

        //    //Assert.AreEqual(7396, peaks.Count);

        //    newSpectra = new MSSpectra(); //parent scan
        //    newSpectra.MSLevel = 1;
        //    newSpectra.PeakProcessingLevel = PeakProcessingLevel.Centroided;
        //    newSpectra.PeaksProcessed = peaks;
        //    newSpectra.Scan = scan;

        //    currentLocationInDataset = YafmsDbUtilities.SetLocationInDataset(ref idCounterForScan, newSpectra.MSLevel, newSpectra.Scan, 0);
        //    YafmsDbUtilities.SaveProcessedPeaksAtCentroidLevel(newSpectra, newSpectra.Scan, ref IdCounterForPeakCentric, ref currentLocationInDataset, out centroidedMS1PeaksToWrite, out centroidedMS1ThresholdedPeaksToWrite, currentEngine, sqliteDetails);
        


        //    //indexes
            
        //    YafmsDbUtilities.WriteIndexesToDatabases(currentEngine, sqliteDetails, 4);//4 PeakCentric
        //}
      





        //public void WriteDataset()
        //{
        //    string datasetFolder = @"E:\ScottK\GetPeaks Data\Diabetes_LC\";
        //    string datasetFileName = @"Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12";
        //    string datasetEnding = @".raw";

        //    string databaseFolder = testFolderGlobal;
        //    //string databaseFile = testFileDBGlobal;
        //    string databaseFile = datasetFileName;

        //    int scansToSum = 5;

        //    WriteDatasetFX(datasetFolder, datasetFileName, datasetEnding, databaseFolder, databaseFile, @".db", scansToSum);
        //}

        //public void WriteDatasetFX(string datasetFolderName, string datasetFileName, string datasetEnding, string databaseFolderName, string databaseFileName, string databaseEnding = @".db", int scansToSum = 1)
        //{
        //    //EnumerationIsPic isPic = EnumerationIsPic.IsPic;
        //    //PopulateGlobalVariables(isPic);
            
        //    //0.  strings
        //    //string databaseFile = databaseFolderName + databaseFileName + "_(0).db";
           
        //    //DATABASE
        //    string databaseFileNameDB = databaseFileName + "_Sum" + scansToSum;
        //    string databaseFile = databaseFolderName + databaseFileNameDB;

        //    if (File.Exists(databaseFile + "_(0).db"))
        //    {

        //        Console.WriteLine("Delete Database.  Database was found...  Press enter");
        //        //Console.ReadKey();
        //        System.IO.File.Delete(databaseFile + "_(0).db");
        //    }
        //    Console.WriteLine("Did we delete the file");
        //   // Console.ReadKey();
        //    //DATASET
        //    string datasetFile = datasetFolderName + datasetFileName + datasetEnding;


        //    //1.  sqliteDetails
        //    ParametersSQLite sqliteDetails = new ParametersSQLite(databaseFolderName, databaseFileNameDB, 1, 1);

        //    YafmsDbUtilities.SetUpSimplePeaksPage(ref sqliteDetails);
        //    YafmsDbUtilities.SetUpScansRelationshipPage(ref sqliteDetails);
        //    YafmsDbUtilities.SetUpPrecursorPeakPage(ref sqliteDetails);
        //    YafmsDbUtilities.SetUpMonoisotopicMassPeaksPage(ref sqliteDetails);
        //    YafmsDbUtilities.SetUpPeaksCentricPage(ref sqliteDetails);
        //    YafmsDbUtilities.SetUpScanCentricPage(ref sqliteDetails);

        //    Assert.AreEqual(sqliteDetails.ColumnHeadersCounts[0], "T_Scan_Peaks");
        //    Assert.AreEqual(sqliteDetails.ColumnHeadersCounts[1], "T_Scans");
        //    Assert.AreEqual(sqliteDetails.ColumnHeadersCounts[2], "T_Scans_Precursor_Peaks");
        //    Assert.AreEqual(sqliteDetails.ColumnHeadersCounts[3], "T_Scan_MonoPeaks");
        //    Assert.AreEqual(sqliteDetails.ColumnHeadersCounts[4], "T_Peak_Centric");
        //    Assert.AreEqual(sqliteDetails.ColumnHeadersCounts[5], "T_Scan_Centric");

        //    Console.WriteLine("Here 1 ");

        //    //2.  engine time
        //    Object databaseLock = new Object();// for creating the main file.  if one thread is used, the lock is not needed
        //    //int engineNumber = 0;
        //    int engineNumber = scansToSum;
        //    EngineSQLite sqLiteEngineBuilder = new EngineSQLite(databaseLock, engineNumber);
        //    sqliteDetails.CoresPerComputer = 1;
        //    ParalellEngineStation sqlEngineStation = sqLiteEngineBuilder.SetupEngines(sqliteDetails);
        //    EngineSQLite currentEngine = (EngineSQLite)sqlEngineStation.Engines[0];


        //    //3.  Run
        //    //string testFile = "";
        //    //testFile = @"E:\ScottK\GetPeaks Data\Diabetes_LC\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw";
        //    //testFile = @"E:\PNNL Data\2012_09_05 SPIN Q-TOF\Gly08_SQTOF_SP02_3X_C1_12_HPIF20Torr_LPRF96_T160_6Sept12.d";

        //    //testFile = @"E:\PNNL Data\2012_09_05 SPIN Q-TOF\Gly08_SQTOF_SL26_100_C2_13_HPIF30Torr_LPRF96_T160_18Sept12.d";//work
        //    //testFile = @"L:\PNNL Files\PNNL Data for Tests\Gly08_SQTOF_SL26_100_C2_13_HPIF30Torr_LPRF96_T160_18Sept12.d";//home
        //    string testFile = datasetFile;

        //    RunFactory factor = new RunFactory();
        //    Run runIn = factor.CreateRun(testFile);

        //    //3.  scan parameters
        //    FragmentedTargetedWorkflowParametersIQ fragmentedTargetedWorkflowParameters = new FragmentedTargetedWorkflowParametersIQ();
        //    fragmentedTargetedWorkflowParameters.SummingMode = SummingModeEnum.SUMMINGMODE_STATIC;
        //    fragmentedTargetedWorkflowParameters.NumMSScansToSum = scansToSum;//5 could be better

        //    fragmentedTargetedWorkflowParameters.MSParameters.PointsPerShoulder = fragmentedTargetedWorkflowParameters.LCParameters.CalculatePointsPerShoulderAsAFunctionOfSgPoints(fragmentedTargetedWorkflowParameters);
        //    fragmentedTargetedWorkflowParameters.MSParameters.ParametersOmicsPeakCentroid.FWHMPeakFitType = PeakFitType.Parabola;



        //    //4.  scanset
        //    ScanObject scanInfo = new ScanObject(0, 0);
        //    //possibleFragmentTarget.ScanInfo.Start = 1648;
        //    //possibleFragmentTarget.ScanInfo.Stop = 1674;
        //    scanInfo.Max = runIn.MaxLCScan;
        //    scanInfo.Min = runIn.MinLCScan;
        //    scanInfo.ScansToSum = fragmentedTargetedWorkflowParameters.NumMSScansToSum;
        //    scanInfo.Buffer = 9 * 2;

        //    runIn.ScanSetCollection.Create(runIn, scanInfo.Min, scanInfo.Max, scanInfo.ScansToSum, 1, false);

        //    //5.  prep processor
        //    ProcessingParametersMassSpectra massSpecParameters = new ProcessingParametersMassSpectra();
        //    massSpecParameters.Engine_OmicsPeakDetection.Parameters.FWHMPeakFitType = PeakFitType.Parabola;
        //    Processors.ProcessorMassSpectra msProcessor = new ProcessorMassSpectra(massSpecParameters);

        //    //6.  prep variables
        //    DeconTools.Backend.XYData massSpectrum = new DeconTools.Backend.XYData();
        //    MSSpectra newSpectra = new MSSpectra(); //parent scan
        //    List<PeakCentric> centroidedMS1PeaksToWrite = new List<PeakCentric>();
        //    List<PeakCentric> centroidedMS1ThresholdedPeaksToWrite = new List<PeakCentric>();
        //    int idCounterForScan = 0;//iniitial conditions
        //    int IdCounterForPeakCentric = 0;//iniitial conditions

        //    //5.  loop

        //    Console.WriteLine("Start Loop");

        //    double sizeOfDataset = runIn.ScanSetCollection.ScanSetList.Count;

        //    for (int i = 0; i < 10; i++)
        //    //for (int i = 0; i < sizeOfDataset; i++)
        //    {
        //        Console.WriteLine("We are working on index: " + i + " out of " + sizeOfDataset);
        //        ScanSet scanSet = runIn.ScanSetCollection.ScanSetList[i];
        //        //5.  select scan
        //        int scan = scanSet.PrimaryScanNumber;
        //        ScanSet lcScanSetSelected = scanSet;

        //        //6.  get mass spec
        //        massSpectrum = msProcessor.DeconMSGeneratorWrapper(runIn, lcScanSetSelected);

        //        //7.  get peaks
        //        List<ProcessedPeak> peaksCentroidedRich = msProcessor.Execute(massSpectrum, EnumerationMassSpectraProcessing.OmicsCentroid_Only);

        //        List<PNNLOmics.Data.XYData> peaksSimple = new List<PNNLOmics.Data.XYData>();
        //        foreach (ProcessedPeak processedPeak in peaksCentroidedRich)
        //        {
        //            peaksSimple.Add(new PNNLOmics.Data.XYData(processedPeak.XValue, processedPeak.Height));
        //        }
        //        Console.WriteLine("there are " + peaksCentroidedRich.Count + " peaks");


        //        //8.  threshold peaks
        //        List<ProcessedPeak> peaksThresholded = msProcessor.Execute(massSpectrum, EnumerationMassSpectraProcessing.OmicsCentroid_OmicsThreshold);


        //        //3.  this is the meta data for this scan
        //        newSpectra.MSLevel = 1;
        //        newSpectra.PeakProcessingLevel = PeakProcessingLevel.Centroided;
        //        newSpectra.Peaks = peaksSimple;
        //        newSpectra.PeaksProcessed = peaksThresholded;
        //        newSpectra.Scan = scan;

        //        centroidedMS1PeaksToWrite = new List<PeakCentric>();
        //        centroidedMS1ThresholdedPeaksToWrite = new List<PeakCentric>();

        //        //Console.WriteLine("Write Dataset: " + datasetFile + " to Database: " + databaseFolderName + databaseFileNameDB + "_(0).db");

        //        //either will contain all centroids or signal
        //        ScanCentric currentLocationInDataset = YafmsDbUtilities.SetLocationInDataset(ref idCounterForScan, newSpectra.MSLevel, newSpectra.Scan, 0);
        //        YafmsDbUtilities.SaveProcessedPeaksAtCentroidLevel(newSpectra, newSpectra.Scan, ref IdCounterForPeakCentric, ref currentLocationInDataset, out centroidedMS1PeaksToWrite, out centroidedMS1ThresholdedPeaksToWrite, currentEngine, sqliteDetails);
        //    }

        //    //indexes

        //    Console.WriteLine("Writing Indexes.  This may take a while...");
        //    //YafmsDbUtilities.WriteIndexesToDatabases(currentEngine, sqliteDetails, sqliteDetails.ColumnHeadersCounts[4]);//4 PeakCentric);

        //    //scan peaks
        //    YafmsDbUtilities.WriteIndexesToDatabases(currentEngine, sqliteDetails, 0);//0 scan peaks);
        //    YafmsDbUtilities.WriteIndexesToDatabases(currentEngine, sqliteDetails, 4);//0 peakCentric);

        //    Console.WriteLine("Finsihed.  Presse Return to end");
        //} 



        #region global variables

        private FragmentIQTarget possibleFragmentTarget { get; set; }
        private FragmentResultsObjectHolderIq targetResult { get; set; }
        private FragmentedTargetedWorkflowParametersIQ _workflowParameters { get; set; }
        private Run runIn { get; set; }
        private IqGlyQResult iQresult { get; set; }
        private ProcessorMassSpectra _msProcessor { get; set; }
        private Tuple<string, string> errorlog { get; set; }
        private string printString { get; set; }
        private IGenerateIsotopeProfile _TheorFeatureGenV2 { get; set; }
        private IterativeTFF _msfeatureFinder { get; set; }
        private TaskIQ _fitScoreCalc { get; set; }
        private ProcessorChromatogram _lcProcessor { get; set; }

        #endregion

        private void PopulateGlobalVariables(EnumerationIsPic isPic)
        {
            bool isUnitTest = true;
            EnumerationDataset thisDataset = EnumerationDataset.Diabetes;
            //EnumerationIsPic isPic = EnumerationIsPic.IsNotPic;

            double deltaMassCalibrationMZ = 0;
            double deltaMassCalibrationMono = 0;
            bool toMassCalibrate = false;

            FragmentIQTarget fragmentIqTarget = possibleFragmentTarget;
            FragmentResultsObjectHolderIq fragmentResultsObjectHolderIq = targetResult;
            FragmentedTargetedWorkflowParametersIQ fragmentedTargetedWorkflowParametersIq = _workflowParameters;
            Run run = runIn;
            IqGlyQResult iqGlyQResult = iQresult;
            ProcessorMassSpectra msProcessor = _msProcessor;
            Tuple<string, string> tuple = errorlog;
            string s = printString;
            JoshTheorFeatureGenerator _theorFeatureGen = new JoshTheorFeatureGenerator(_workflowParameters.IsotopeProfileType, _workflowParameters.IsotopeLowPeakCuttoff);//perhaps simple constructor
            IGenerateIsotopeProfile TheorFeatureGenV2 = new IsotopeProfileSimple(new ParametersSimpleIsotope(_theorFeatureGen));
            IterativeTFF msfeatureFinder = _msfeatureFinder;
            TaskIQ isotopicProfileFitScoreCalculator = _fitScoreCalc;
            ProcessorChromatogram processorChromatogram = _lcProcessor;
            IQGlyQTestingUtilities.SetupTargetAndEnginesForOneTargetRef(ref fragmentIqTarget, ref fragmentResultsObjectHolderIq, ref fragmentedTargetedWorkflowParametersIq, ref run, ref iqGlyQResult, ref msProcessor, ref tuple, ref s, ref TheorFeatureGenV2, ref msfeatureFinder, ref isotopicProfileFitScoreCalculator, ref processorChromatogram, isUnitTest, thisDataset, isPic);

            possibleFragmentTarget = fragmentIqTarget;
            targetResult = fragmentResultsObjectHolderIq;
            _workflowParameters = fragmentedTargetedWorkflowParametersIq;
            runIn = run;
            iQresult = iqGlyQResult;
            _msProcessor = msProcessor;
            errorlog = tuple;
            printString = s;
            _TheorFeatureGenV2 = TheorFeatureGenV2;
            _msfeatureFinder = msfeatureFinder;
            _fitScoreCalc = isotopicProfileFitScoreCalculator;
            _lcProcessor = processorChromatogram;

        }
    }
}
//    SELECT Min(TP.scanid) AS 'TP_ScanID', 
//       TS.scannumlc   AS 'TS_ScanNumLc', 
//       Max(TP.height) AS 'TP_Height' 
//FROM   t_peak_centric TP 
//       INNER JOIN t_scan_centric TS 
//               ON TP.scanid = TS.scanid 
//WHERE  TP.mz > 280 
//       AND TP.mz < 282 
//GROUP  BY TS.scannumlc;  
