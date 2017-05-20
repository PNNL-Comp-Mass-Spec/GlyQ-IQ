using System;
using System.Collections.Generic;
using GetPeaks_DLL.SQLite.OneLineCalls;
using GetPeaks_DLL.THRASH;
using OrbitrapPeakDetection;
using System.Diagnostics;
using GetPeaks_DLL.Functions;
using GetPeaks_DLL.Objects.Enumerations;
using Parallel.SQLite;
using Parallel.THRASH;
using GetPeaks_DLL.Parallel;
using GetPeaks_DLL.SQLiteEngine;
using GetPeaks_DLL.Go_Decon_Modules;
using GetPeaks.UnitTests;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.Objects.ParameterObjects;
using IQGlyQ;

//http://www.albahari.com/threading/part5.aspx

namespace Parallel
{
    public class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            System.DateTime starttime = DateTime.Now;

            TandemLoadTests tester = new TandemLoadTests();

            //args "E:\ScottK\GetPeaks Data" "Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12" "RAW" "E:\ScottK\Populator" "E:\ScottK\Populator\Logs1.txt" "E:\ScottK\Populator" "0_Glyco08 LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_Fitpt2_Velos"

            //"E:\ScottK\GetPeaks Data\2012_09_05 SPIN Q-TOF" "Gly08_SQTOF_D60B1X_1X_C2_13_HPIF20Torr_LPRF74_T160_6Sept12" "d" "E:\ScottK\Populator" "E:\ScottK\Populator\Logs1.txt" "E:\ScottK\Populator" "0_Glyco08 LTQ_Orb_SN2_PeakBR4_PeptideBR1_Thrash_Fitpt2_SPIN"
            //"E:\ScottK\GetPeaks Data" "Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12" "RAW" "E:\ScottK\Populator" "E:\ScottK\Populator\Logs1.txt" "E:\ScottK\Populator" "0_Glyco08 LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_Fitpt2_Velos"
            tester.testLoadingChromatogramTandem(args);//this is what we use for writing new processed databases//Current DB writer

            //IsotopicProfile.IntensityAggregateAdjusted, IntensityAggregate was moved out of IsotopicProfile to the IsosResult.  

            //SQLiteTestsPreprocessedDatabase.GetPrecursorPeaksFromScan();///this is how we read from the database
            //SQLiteTestsPreprocessedDatabase.SK_GetThresholdedPeaksFromScan();
            //SQLiteTestsPreprocessedDatabase.SK_ReadScansViaScan();
            //SQLiteTestsPreprocessedDatabase.SK_ReadScansViaParentScan();
            //SQLiteTestsPreprocessedDatabase.SK_GetMonoPeaksFromScan();
            //SQLiteTestsPreprocessedDatabase.SK_GetPrecursorParentPeakViaTandemScan();
            //SQLiteTestsPreprocessedDatabase.SK_GetPrecursorPeakAndTandemMonoViaTandemScan();
            //SQLiteTestsPreprocessedDatabase.SK_ReadAllFragmentedScans();
            //SQLiteTestsPreprocessedDatabase.SK2_GetPrecursorPeakAndTandemMonoAndPeaksViaTandemScan();

    //        string fileNameTest = @"K:\Csharp\0_TestDataFiles\Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_OrbitrapFilter_ClusterDefault__sum1_1cor_1div_0oLev_130PBR_20fit_(0).db";
            
            //args "E:\ScottK\Populator" "Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_DeconTools_ClusterDefault__sum1_1cor_1div_0oLev_130PBR_20fit_(145)" ".db"
            //args 11-28 "E:\ScottK\Populator" "Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_DeconTools_ClusterDefault__sum1_1cor_1div_0oLev_130PBR_20fit_(145)" ".db"
            //args 11-30 "E:\ScottK\Populator" "Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_OrbitrapFilter_ClusterDefault__sum1_1cor_1div_0oLev_13PBR_20fit_0HPBR_(0)" ".db"
            //args 12-3 "E:\ScottK\Populator" "Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_DeconTools_ClusterDefault__sum1_1cor_1div_0oLev_13PBR_20fit_6HPBR_(0)" ".db"
            //args 5-14-2013 "E:\ScottK\Populator" "Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_DeconTools_ClusterDefault__sum1_1cor_1div_0oLev_13PBR_20fit_6HPBR_(0)" ".db"

            //FragnemtationAnalysisTest.Run(args);//Current DB Glycolyzer

            //FragnemtationAnalysisTest.Run(fileNameTest, starttime, stopWatch, outputLocation);
            //args 12-17 "K:\SQLite" "Gly08_Velos4_Jaguar_200nL_D60A_1X_C1_2Sept12_OrbitrapFilter_ClusterDefault__sum1_1cor_1div_0oLev_13PBR_20fit_0HPBR_(0)" ".db"

            ///pass through is what we use for applying peak detectors and multithreaded thrash.  ROC curves are in Christina's program

            System.DateTime stoptime = DateTime.Now;
            Console.WriteLine("This started at " + starttime + " and ended at" + stoptime);
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to find " + 4 + " eluting peaks");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("finished with tandem, press return to end");
            //Console.ReadKey();
            
            Console.ReadKey();

            //OldDecon2LSParameters parametersDT = new OldDecon2LSParameters();
            //string paramFile = @"\\protoapps\UserData\Slysz\DeconTools_TestFiles\ParameterFiles\LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_MaxFit1.xml";
            //parametersDT.Load(paramFile);
            //ThrashParameters.DeconThrashParameters = parametersDT.HornTransformParameters;

            //things to do
            //1. Setup Parameters (abstract)
            //2. Setup Controller
            //3. CreateEngines (abstract)
            //4. Run Controller

//__________1. Setup Parameters
            //int numberOfScans = 150;

            //which file to use

            //char letter = 'K';
            //string sqLiteFile;
            //string sqLiteFolder;
            //int computersToDivideOver;
            //int coresPerComputer;
            //string logFile;
            //ParametersTHRASH ThrashParameters = ParametersForTesting.Load(letter, out sqLiteFile, out sqLiteFolder, out computersToDivideOver, out coresPerComputer, out logFile);

            PopulatorArgs argsSK = new PopulatorArgs(args);

            const char letter = 'K';
            const bool overrideParameterFile = true;
            ParalellController engineController;
            string sqLiteFile;
            string sqLiteFolder;
            int computersToDivideOver;
            int coresPerComputer;
            string logFile;
            InputOutputFileName newFile;
            ParametersSQLite sqliteDetails;

            ParametersForTesting.Load(letter, overrideParameterFile, out engineController, out sqLiteFile, out sqLiteFolder, out computersToDivideOver, out coresPerComputer, out logFile, out newFile, out sqliteDetails, argsSK);

            ParametersTHRASH ThrashParameters = (ParametersTHRASH)engineController.ParameterStorage1;
            //string sqLiteFolder = ThrashParameters.FileInforamation.OutputSQLFileName;


            //string sqLiteFolder;
            //string logFile;
            //string inputDatasetFileName;// = @"D:\PNNL Files\DataCopy\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";
            //string parameterFileName;// = @"K:\Data\Glyco07 Decon ParametersSum2for5xSN_MT_All.xml";
            //string sqLiteFile = "PD";

            //char letter = 'K';
           
            //switch (letter)
            //{
            //    #region inside
            //    case 'E':
            //    {
            //        sqLiteFolder = @"E:\ScottK\NoisePeakDetector\Release\";
            //        logFile = @"E:\ScottK\NoisePeakDetector\Release\Logs.txt";
            //        inputDatasetFileName = @"E:\ScottK\GetPeaks Data\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";
            //        //parameterFileName = @"E:\ScottK\DeconToolsParameter\Glyco07 Decon ParametersSum2for5xSN_MT_All.xml";
            //        parameterFileName = @"E:\ScottK\NoisePeakDetector\LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_MaxFit1.xml";
            //    } break;
            //    case 'F':
            //    {
            //        sqLiteFolder = @"F:\ScottK\NoisePeakDetector\Release\";
            //        logFile = @"F:\ScottK\NoisePeakDetector\Release\Logs.txt";
            //        inputDatasetFileName = @"F:\ScottK\GetPeaks Data\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";
            //        //parameterFileName = @"F:\ScottK\DeconToolsParameter\Glyco07 Decon ParametersSum2for5xSN_MT_All.xml";
            //        parameterFileName = @"F:\ScottK\NoisePeakDetector\LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_MaxFit1.xml";
            //    } break;
            //    case 'D':
            //    {
            //        sqLiteFolder = @"D:\ScottK\NoisePeakDetector\Release\";
            //        logFile = @"D:\ScottK\NoisePeakDetector\Release\Logs.txt";
            //        inputDatasetFileName = @"D:\ScottK\GetPeaks Data\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";
            //        //parameterFileName = @"D:\ScottK\DeconToolsParameter\Glyco07 Decon ParametersSum2for5xSN_MT_All.xml";
            //        parameterFileName = @"D:\ScottK\NoisePeakDetector\LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_MaxFit1.xml";
            //        sqLiteFile = "PD";
            //    } break;
            //    case 'K':
            //    {
            //        sqLiteFolder = @"K:\"; ;
            //        logFile = @"K:\Logs.txt";
            //        inputDatasetFileName = @"D:\PNNL Files\DataCopy\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";
            //        //parameterFileName = @"H:\ScottK\DeconToolsParameter\Glyco07 Decon ParametersSum2for5xSN_MT_All.xml";
            //        //parameterFileName = @"K:\Data\LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_MaxFit1.xml";
            //        parameterFileName = @"K:\Data\Glyco08 LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_Fitpt2_Velos.xml";
            //        sqLiteFile = "PD";
            //    } break;
            //    default:
            //        {
            //            sqLiteFolder = @"K:\"; ;
            //            logFile = @"K:\Logs.txt";
            //            inputDatasetFileName = @"H:\ScottK\GetPeaks Data\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";
            //            //parameterFileName = @"H:\ScottK\DeconToolsParameter\Glyco07 Decon ParametersSum2for5xSN_MT_All.xml";
            //            parameterFileName = @"H:\ScottK\NoisePeakDetector\LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_MaxFit1.xml";
            //            sqLiteFile = "PD";
            //        } break;
            //    #endregion
            //}
           
            
            ////TODO the problem is related to the number of cores.  2 cores had 2 problems.  4 cores have 4 problems.  there must be a rogue engine

            ////copy file

            //int computersToDivideOver = 110; //110 is a good size for debugging
            ////int computersToDivideOver = 25;//110 is a good size for debugging
            ////int computersToDivideOver = 4;//110 is a good size for debugging
            ////int computersToDivideOver = 4;//110 is a good size for debugging
            //int coresPerComputer = 1; //24 is default for pubs, 1 is single core

            //bool multithreadMode = coresPerComputer > 1;
            //bool chooseMultithreadedHardDriveMode = multithreadMode;
            //if (chooseMultithreadedHardDriveMode == true)
            //{
            //    CopyFile.RAW(inputDatasetFileName, coresPerComputer + 2);
            //}

            //ScanSummingRanges sumThisManyScans = ScanSummingRanges.OneScan; 
            //PeakDetectors peakDetector = PeakDetectors.DeconTools;
            
            //const int maxScanLimitation = 99999;
            //const bool processMSMS = false;
            //int scanstoSum = SumRange.ConvertRangeToInt(sumThisManyScans);
            //double fitScore = 0.1;//deisotoping fit score
            //const double peptideMinBackgroundRatio = 0.00; //this effects the THRASH low level peak detector
            //const bool overrideParameterFile = false; //fit and ThrashPBR//false will use the parameter file

            //ParametersTHRASH ThrashParameters = new ParametersTHRASH(sqLiteFolder, sqLiteFile, inputDatasetFileName, parameterFileName, scanstoSum, computersToDivideOver, coresPerComputer, maxScanLimitation, processMSMS, peakDetector,
            //                                                         chooseMultithreadedHardDriveMode, fitScore, peptideMinBackgroundRatio, overrideParameterFile);

            //ThrashParameters.MultithreadOperation = multithreadMode;
            //ThrashParameters.ForEachCoreMultiplier = 3;

            ////peak detection
            //ThrashParameters.MsPeakDetectorPeakToBackground = 1.3;// 1.3 default PBR this affects deconTools peak detector
            //ThrashParameters.SignalToNoiseRatio = 2;//2 default
            //ThrashParameters.HammerParameters.SigmaMultiplier = 0;//this effects the orbitrap peak detector
            //ThrashParameters.HammerParameters.MinimumSizeOfRegion = 30;//30 is normal
            //ThrashParameters.HammerParameters.OptimizeOrDefaultChoise = HammerThresholdParameters.OptimizeOrDefaultMassSpacing.Default;
            //ThrashParameters.HammerParameters.ThresholdOrClusterChoise = HammerThresholdParameters.OrbitrapFilteringMethod.Cluster;
            //ThrashParameters.HammerParameters.SeedClusterSpacing = 1.0032;//Da// MassOfSpace = 1.00310;//1.00235//1.0008//1.0028//0.991 too low//1.004 ok//1.0032 is optimal
            //ThrashParameters.HammerParameters.SeedMassToleranceDa = 0.006;//Da//0.004 is 3 sigma//0.006 should get everything sigma is 0.01315

            ////deisotoping
            ////ThrashParameters.ThrashParameters.Parameters.PeptideMinBackgroundRatio = 0;//this effects the THRASH low level peak detector
            ////ThrashParameters.ThrashParameters.DeconThrashParameters.PeptideMinBackgroundRatio = 0;//this effects the THRASH low level peak detector
            ////ThrashParameters.ThrashParameters.DeconThrashParameters.MaxFit = 0.10;
            ////ThrashParameters.ThrashParameters.Parameters.MaxFit = 0.10;


            Console.WriteLine("Thrash Setup");

//__________2. Setup Controller
            ParalellController newController = new ParalellController(ThrashParameters);

//__________3. Create Engines 
            #region engine station for transformer (off)
            //EngineTHRASH ThrashEngineBuilder = new EngineTHRASH();
            //newController.EngineStation = ThrashEngineBuilder.SetupEngines(ThrashParameters);
            #endregion
            EngineThrashDeconvolutor thrashEngineDeconvolutorBuilder = new EngineThrashDeconvolutor();
            newController.EngineStation = thrashEngineDeconvolutorBuilder.SetupEngines(ThrashParameters);  
            //test discreteness of engine station
            //CheckDiscretenessOfEngines(newController);

//__________4. Setup Parameters for SQLite writer
            Object databaseLock = new Object();// for creating the main file.  if one thread is used, the lock is not needed
            
            ParametersSQLite sqLiteParameters = new ParametersSQLite(sqLiteFolder, sqLiteFile, computersToDivideOver, coresPerComputer);
            sqLiteParameters.ColumnHeadersCounts.Add("MonoCount");
            sqLiteParameters.ColumnHeadersCounts.Add("IsosCount");

//__________5. Create SQL Engines 
            //EngineSQLite sqLiteEngineBuilder = new EngineSQLite(sqLiteParameters, databaseLock, 0);
            EngineSQLite sqLiteEngineBuilder = new EngineSQLite(databaseLock, 0);
            sqLiteParameters.CoresPerComputer = 1;
            newController.SqlEngineStation = sqLiteEngineBuilder.SetupEngines(sqLiteParameters);

//__________6. Run Controller
            Console.WriteLine("Go Algorithms");
            List<string> logs = newController.RunAlgorithms();

            GetPeaks_DLL.DataFIFO.StringListToDisk writer = new GetPeaks_DLL.DataFIFO.StringListToDisk();
            //writer.toDiskStringList(@"K:\Logs.txt",logs);
            writer.toDiskStringList(logFile, logs);

////__________4. Run Writer
//            int scanNumber = 4;
//            Peak resultPeak = new Peak();
//            resultPeak.XValue = 1000;
//            bool didthiswork = SQLiteEngineBuilder.WritePeakData((EngineSQLite)newController.SQLEngineStation.Engines[0], resultPeak, scanNumber);
//            Console.WriteLine("Press a Key to End\n");


//            System.DateTime stoptime = DateTime.Now;
            Console.WriteLine("This started at " + starttime + " and ended at" + stoptime);
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to find " + 4 + " eluting peaks");
            Console.WriteLine("");

            Console.ReadKey();
            Console.ReadKey();
////__________4. Run Writer
            //int scanNumber = 4;
            //Peak resultPeak = new Peak();
            //resultPeak.XValue = 1000;
            //EngineSQLite currentEngine = (EngineSQLite)newController.SQLEngineStation.Engines[0];
            //bool didthiswork = currentEngine.WritePeakData((EngineSQLite)newController.SQLEngineStation.Engines[0], resultPeak, scanNumber);
            //Console.WriteLine("Press a Key to End\n");

        }

        private static void CheckDiscretenessOfEngines(ParalellController newController)
        {
            for (int i = 0; i < newController.EngineStation.Engines.Count; i++)
            {
                EngineTHRASH engine = (EngineTHRASH) newController.EngineStation.Engines[i];
                Console.WriteLine("PASS " + i);

                Console.WriteLine(engine.EngineNumber);
                engine.EngineNumber = i + 10;

                Console.WriteLine(engine.Run.XYData.Xvalues[0]);
                engine.Run.XYData.Xvalues[0] = i + 10;
            }

            Console.WriteLine("_check_");
            for (int i = 0; i < newController.EngineStation.Engines.Count; i++)
            {
                Console.WriteLine("PASS " + i);
                EngineTHRASH engine = (EngineTHRASH) newController.EngineStation.Engines[i];
                Console.WriteLine(engine.EngineNumber);
                Console.WriteLine(engine.Run.XYData.Xvalues[0]);
            }
        }
    }


}
