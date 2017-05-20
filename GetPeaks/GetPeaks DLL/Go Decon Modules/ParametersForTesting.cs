using System;
using GetPeaks_DLL.Functions;
using GetPeaks_DLL.Objects.Enumerations;
using HammerPeakDetector.Enumerations;
using GetPeaks_DLL.Parallel;
using GetPeaks_DLL.THRASH;
using GetPeaks_DLL.SQLiteEngine;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.Objects.ParameterObjects;

namespace GetPeaks_DLL.Go_Decon_Modules
{
    public static class ParametersForTesting
    {
        public static void Load(char letter, bool useParameterFileValues, out ParalellController engineController, out string sqLiteFile, out string sqLiteFolder, out int computersToDivideOver, out int coresPerComputer, out string logFile, out InputOutputFileName newFile, out ParametersSQLite sqliteDetails, PopulatorArgs fileParameters)
        {
            string inputDatasetFileName; // = @"D:\PNNL Files\DataCopy\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";
            string inputDatasetFolder;
            string fileNameOnly = "DefaultFileName";
            string fileExtension;
            string parameterFileName; // = @"K:\Data\Glyco07 Decon ParametersSum2for5xSN_MT_All.xml";
            sqLiteFile = "PD";

            switch (letter)
            {
                    #region inside
                case 'E':
                    {
                        //sqLiteFolder = @"E:\ScottK\NoisePeakDetector\Release\";
                        //logFile = @"E:\ScottK\NoisePeakDetector\Release\Logs.txt";
                        //inputDatasetFileName = @"E:\ScottK\GetPeaks Data\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";
                        //parameterFileName = @"E:\ScottK\DeconToolsParameter\Glyco07 Decon ParametersSum2for5xSN_MT_All.xml";
                        //parameterFileName = @"E:\ScottK\NoisePeakDetector\LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_MaxFit1.xml";

                        inputDatasetFolder = @"E:\ScottK\GetPeaks Data\"; //with \
                        //fileNameOnly = "Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12";
                        fileNameOnly = "Gly08_Velos4_Jaguar_200nL_MCF7_3X_C1_31Aug12";
                        fileExtension = "RAW";

                        sqLiteFolder = @"E:\ScottK\GetPeaks Data\SQLite\"; //with \
                        sqLiteFile = fileNameOnly;

                        logFile = @"E:\ScottK\GetPeaks Data\SQLite\Logs1.txt";
 
                        inputDatasetFileName = inputDatasetFolder + fileNameOnly + "." + fileExtension; 
                        parameterFileName = @"E:\ScottK\GetPeaks Data\SQLite\0_Glyco08 LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_Fitpt2_Velos.xml";
                    }
                    break;
                case 'F':
                    {
                        sqLiteFolder = @"F:\ScottK\NoisePeakDetector\Release\";
                        logFile = @"F:\ScottK\NoisePeakDetector\Release\Logs.txt";
                        inputDatasetFileName = @"F:\ScottK\GetPeaks Data\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";
                        //parameterFileName = @"F:\ScottK\DeconToolsParameter\Glyco07 Decon ParametersSum2for5xSN_MT_All.xml";
                        parameterFileName = @"F:\ScottK\NoisePeakDetector\LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_MaxFit1.xml";
                    }
                    break;
                case 'D':
                    {
                        #region inside
                        //sqLiteFolder = @"D:\ScottK\NoisePeakDetector\Release\";
                        //logFile = @"D:\ScottK\NoisePeakDetector\Release\Logs.txt";
                        //inputDatasetFileName = @"D:\ScottK\GetPeaks Data\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";
                        ////parameterFileName = @"D:\ScottK\DeconToolsParameter\Glyco07 Decon ParametersSum2for5xSN_MT_All.xml";
                        //parameterFileName = @"D:\ScottK\NoisePeakDetector\LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_MaxFit1.xml";
                        //sqLiteFile = "PD";

                        inputDatasetFolder = @"D:\ScottK\GetPeaks Data\"; //with \
                        //inputDatasetFolder = @"E:\PNNL Data\2012_09_05 SPIN Q-TOF\";
                        
                        //fileNameOnly = "Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12";
                        fileNameOnly = "Gly08_Velos4_Jaguar_200nL_MCF7_3X_C1_31Aug12";
                        //fileNameOnly = "Gly08_SQTOF_SP02PSA_3X_C1_12_HPIF20Torr_LPRF96_T160_6Sept12";
                        
                        fileExtension = "RAW";
                        //fileExtension = "d";

                        //sqLiteFolder = @"K:\SQLite\";
                        sqLiteFolder = @"D:\ScottK\GetPeaks Data\SQLite\";//with \
                        //sqLiteFile = "PD";
                        sqLiteFile = fileNameOnly;

                        logFile = @"D:\ScottK\GetPeaks Data\SQLite\Logs1.txt";
                        //logFile = @"K:\Logs2.txt";
                        //inputDatasetFileName = @"E:\ScottK\GetPeaks Data\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";
                        //inputDatasetFileName = @"D:\PNNL Files\DataCopy\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";

                        inputDatasetFileName = inputDatasetFolder + fileNameOnly + "." + fileExtension;
                        //parameterFileName = @"D:\ScottK\GetPeaks Data\SQLite\0_Glyco08 LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_Fitpt2_Velos.xml";
                        parameterFileName = @"D:\ScottK\GetPeaks Data\SQLite\0_Glyco08 LTQ_Orb_SN2_PeakBR4_PeptideBR1_Thrash_Fitpt2_SPIN.xml";
                        //parameterFileName = @"H:\ScottK\DeconToolsParameter\Glyco07 Decon ParametersSum2for5xSN_MT_All.xml";
                        //parameterFileName = @"K:\Data\LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_MaxFit1.xml";
                        //parameterFileName = @"K:\Data\Glyco08 LTQ_Orb_SN2_PeakBR1pt3_PeptideBR4_Thrash_Fitpt2_Velos.xml";
                        #endregion
                    }
                    break;
                case 'K':
                    {
                        inputDatasetFolder = @"E:\ScottK\GetPeaks Data For K\"; //with \
                        //fileNameOnly = "Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12";
                        fileNameOnly = "Gly08_Velos4_Jaguar_200nL_MCF7_3X_C1_31Aug12";
                        fileExtension = "RAW";

                        //sqLiteFolder = @"K:\SQLite\";
                        sqLiteFolder = @"E:\ScottK\GetPeaks Data For K\SQLite\";//with \
                        //sqLiteFile = "PD";
                        sqLiteFile = fileNameOnly;

                        logFile = @"K:\SQLite\Logs1.txt";
                        //logFile = @"K:\Logs2.txt";
                        //inputDatasetFileName = @"E:\ScottK\GetPeaks Data\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";
                        //inputDatasetFileName = @"D:\PNNL Files\DataCopy\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";

                        inputDatasetFileName = inputDatasetFolder + fileNameOnly + "." + fileExtension;
                        parameterFileName = @"E:\ScottK\GetPeaks Data For K\SQLite\0_Glyco08 LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_Fitpt2_Velos.xml";
                        //parameterFileName = @"H:\ScottK\DeconToolsParameter\Glyco07 Decon ParametersSum2for5xSN_MT_All.xml";
                        //parameterFileName = @"K:\Data\LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_MaxFit1.xml";
                        //parameterFileName = @"K:\Data\Glyco08 LTQ_Orb_SN2_PeakBR1pt3_PeptideBR4_Thrash_Fitpt2_Velos.xml";
                        
                        
                    }
                    break;
                case 'Z':
                    {
                        inputDatasetFolder = fileParameters.inputDatasetFolder + @"\"; //with \

                        fileNameOnly = fileParameters.fileNameOnly;
                        fileExtension = fileParameters.fileExtension;


                        sqLiteFolder = fileParameters.sqLiteFolder + @"\";//with \

                        sqLiteFile = sqLiteFolder;

                        logFile = fileParameters.logFile;
                        
                        inputDatasetFileName = inputDatasetFolder + fileNameOnly + "." + fileExtension;
                        parameterFileName = fileParameters.parameterFileFolder + @"\" +fileParameters.parameterFileNameOnly +".xml";

                        Console.WriteLine(inputDatasetFileName);
                        Console.WriteLine(parameterFileName);
                        Console.WriteLine(sqLiteFolder);
                        //Console.ReadKey();

                    }
                    break;
                default:
                    {
                        sqLiteFolder = @"K:\SQLite\";
                        logFile = @"K:\Logs.txt";
                        inputDatasetFileName = @"H:\ScottK\GetPeaks Data\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";

                        //parameterFileName = @"H:\ScottK\DeconToolsParameter\Glyco07 Decon ParametersSum2for5xSN_MT_All.xml";
                        parameterFileName = @"H:\ScottK\NoisePeakDetector\LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_MaxFit1.xml";
                        sqLiteFile = "PD";
                    }
                    break;

                    #endregion
            }

            //computersToDivideOver = 110; //110 is a good size for debugging
            //computersToDivideOver = 25;//110 is a good size for debugging
            //computersToDivideOver = 4;//110 is a good size for debugging
            computersToDivideOver = 1; //110 is a good size for debugging
            coresPerComputer = 1; //24 is default for pubs, 1 is single core
            int forEachCoreMultiplier = 3;

            bool multithreadMode = coresPerComputer > 1;
            bool chooseMultithreadedHardDriveMode = multithreadMode;
            if (chooseMultithreadedHardDriveMode == true)
            {
                CopyFile.RAW(inputDatasetFileName, coresPerComputer + 2);
            }

            //////////////////////////////////////////////////////////////////////////////////
             
            ScanSummingRanges sumThisManyScans = ScanSummingRanges.OneScan;
            

            const int maxScanLimitation = 99999;
            const bool processMSMS = false;//I think this is always false
            int scanstoSum = SumRange.ConvertRangeToInt(sumThisManyScans);
            const double fitScore = 0.20; //deisotoping fit score//SPIN 0.1
            const double peptideMinBackgroundRatio = 1.0011; //this effects the THRASH low level peak detector
            
            ParametersPeakDetection peakParameters = new ParametersPeakDetection();
            
            //TODO remember the parameter file will override these numbers if it is checked
            peakParameters.PeakDetectionMethod = PeakDetectors.OrbitrapFilter; 
            peakParameters.isDataThresholdedDecon = true;  //True for Orbitrap, False for TOF
            peakParameters.shouldWeApplyCentroidToTandemHammer = true;//this needs to be worked on a bit to see what happens to centroided data
            peakParameters.DeconToolsPeakDetection.MsPeakDetectorPeakToBackground = 1.3; // 1.3 default PBR this affects deconTools peak detector//SPIN = 4
            //peakParameters.DeconToolsPeakDetection.MsPeakDetectorPeakToBackground = 1.3; // 1.3 default PBR this affects deconTools peak detector//SPIN = 4
            
            peakParameters.DeconToolsPeakDetection.SignalToNoiseRatio = 2.0; //2 default always

            //TODO these are alyways read from here
            peakParameters.HammerParameters.ThresholdSigmaMultiplier = 0; //this effects the orbitrap peak detector
            peakParameters.HammerParameters.MinimumSizeOfRegion = 30; //30 is normal
            //peakParameters.HammerParameters.OptimizeOrDefaultChoise = HammerThresholdParameters.OptimizeOrDefaultMassSpacing.Default;
            //peakParameters.HammerParameters.ThresholdOrClusterChoise = HammerThresholdParameters.OrbitrapFilteringMethod.Cluster;
            //peakParameters.HammerParameters.OptimizeOrDefaultChoise = OptimizeOrDefaultMassSpacing.Default;
            peakParameters.HammerParameters.FilteringMethod = HammerFilteringMethod.Cluster;// OrbitrapFilteringMethod.Cluster;
            peakParameters.HammerParameters.SeedClusterSpacingCenter = 1.00321; //Da// MassOfSpace = 1.00310;//1.00235//1.0008//1.0028//0.991 too low//1.004 ok//1.00321 is optimal
            peakParameters.HammerParameters.SeedMassToleranceDa = 0.006; //Da//0.004 is 3 sigma//0.006 should get everything sigma is 0.01315
            peakParameters.HammerParameters.CentroidPeakToBackgroundRatio = 0; //0 is default for pure hammer.  raise to 5 for QTOF data that has background

            /////////////////////////////////////////////////////////////////////////////////////////////////

            ParametersTHRASH ThrashParameters = new ParametersTHRASH(
                sqLiteFolder,
                sqLiteFile,
                inputDatasetFileName,
                parameterFileName,
                scanstoSum,
                maxScanLimitation,
                processMSMS,
                chooseMultithreadedHardDriveMode,
                useParameterFileValues,
                peakParameters,
                fitScore,
                peptideMinBackgroundRatio);

            ThrashParameters.ComputersToDivideOver = computersToDivideOver;
            ThrashParameters.CoresPerComputer = coresPerComputer;
            ThrashParameters.MultithreadOperation = multithreadMode;
            ThrashParameters.ForEachCoreMultiplier = forEachCoreMultiplier;//how many threads are spawned

            //peak detection
            //ThrashParameters.FileInforamation.OutputSQLFileName = sqLiteFolder;

            //sqLiteFile = sqLiteFile + "_" + peakParameters.PeakDetectionMethod.ToString() + "_" + scanstoSum + "sum_" + coresPerComputer + "cores_" + computersToDivideOver + "divider_" + multithreadMode.ToString();
            //sqLiteFile += "-MULTIMode_" + ThrashParameters.PeakDetectionMultiParameters.HammerParameters.SigmaMultiplier + "oLevel_";
            //sqLiteFile += ThrashParameters.PeakDetectionMultiParameters.DeconToolsPeakDetection.MsPeakDetectorPeakToBackground + "PBR_" + ThrashParameters.PeakDetectionMultiParameters.DeconToolsPeakDetection.SignalToNoiseRatio + "SNT_";
            //sqLiteFile += ThrashParameters.DeisotopingParametersThrash.DeconThrashParameters.MaxFit*100 + "fit";
            //sqLiteFile += ThrashParameters.PeakDetectionMultiParameters.HammerParameters.SeedClusterSpacing * 10000 + "_" + ThrashParameters.PeakDetectionMultiParameters.HammerParameters.SeedMassToleranceDa * 1000;

            //sqLiteFile = sqLiteFile + "" + peakParameters.PeakDetectionMethod.ToString() + "_" + peakParameters.HammerParameters.ThresholdOrClusterChoise.ToString() + peakParameters.HammerParameters.OptimizeOrDefaultChoise + "_";

            //sqLiteFile = fileNameOnly + "_" + peakParameters.PeakDetectionMethod.ToString() + "_" + peakParameters.HammerParameters.ThresholdOrClusterChoise.ToString() + peakParameters.HammerParameters.OptimizeOrDefaultChoise + "_";
            sqLiteFile = fileNameOnly + "_" + peakParameters.PeakDetectionMethod.ToString() + "_" + peakParameters.HammerParameters.FilteringMethod.ToString() + "_";
            
            sqLiteFile += "_sum" + scanstoSum + "_" + coresPerComputer + "cor_" + computersToDivideOver + "div_";
            sqLiteFile += ThrashParameters.PeakDetectionMultiParameters.HammerParameters.ThresholdSigmaMultiplier + "oLev_";
            sqLiteFile += ThrashParameters.PeakDetectionMultiParameters.DeconToolsPeakDetection.MsPeakDetectorPeakToBackground*10 + "PBR_";
            sqLiteFile += ThrashParameters.DeisotopingParametersThrash.DeconThrashParameters.MaxFit * 100 + "fit_";
            sqLiteFile += ThrashParameters.PeakDetectionMultiParameters.HammerParameters.CentroidPeakToBackgroundRatio * 1 + "HPBR";//.db is not needed

            //string sqLiteFolder = ThrashParameters.FileInforamation.OutputSQLFileName;
            //ThrashParameters.ScansToSum = 1;//FORCED//TODO Summing still needs work
            Console.WriteLine("Creating Engines...");

            engineController = new ParalellController(ThrashParameters);
            EngineThrashDeconvolutor thrashEngineDeconvolutorBuilder = new EngineThrashDeconvolutor();
            engineController.EngineStation = thrashEngineDeconvolutorBuilder.SetupEngines(ThrashParameters);

            #region setup sqlite database

            Object databaseLock = new Object(); // for creating the main file.  if one thread is used, the lock is not needed
            sqliteDetails = new ParametersSQLite(sqLiteFolder, sqLiteFile, 1, 1);
            EngineSQLite sqLiteEngineBuilder = new EngineSQLite(databaseLock, 0);
            sqliteDetails.CoresPerComputer = 1;

            SetUpProcessedPeaksPage(ref sqliteDetails);//0
            SetUpScansRelationshipPage(ref sqliteDetails);//1
            SetUpPrecursorPeakPage(ref sqliteDetails);//2
            SetUpMonoisotopicMassPeaksPage(ref sqliteDetails);//3
            SetUpPeaksCentricPage(ref sqliteDetails);//4
            SetUpScanCentricPage(ref sqliteDetails);//5
            //SetUpAttributeCentricPage(ref sqliteDetails);//6
            //SetUpFragmentCentricPage(ref sqliteDetails);//7

            engineController.SqlEngineStation = sqLiteEngineBuilder.SetupEngines(sqliteDetails);

            #endregion

            newFile = engineController.ParameterStorage1.FileInforamation;
        }

        #region private functions

        private static void SetUpProcessedPeaksPage(ref ParametersSQLite sqliteDetails)
        {
            sqliteDetails.PageName = "T_Scan_Peaks";//peaks
            sqliteDetails.ColumnHeadersCounts.Add("T_Scan_Peaks");
        }

        private static void SetUpScansRelationshipPage(ref ParametersSQLite sqliteDetails)
        {
            sqliteDetails.PageName = "T_Scans";//relationships
            sqliteDetails.ColumnHeadersCounts.Add("T_Scans");
        }

        private static void SetUpPrecursorPeakPage(ref ParametersSQLite sqliteDetails)
        {
            sqliteDetails.PageName = "T_Scans_Precursor_Peaks";//precursor Peaks
            sqliteDetails.ColumnHeadersCounts.Add("T_Scans_Precursor_Peaks");
        }

        private static void SetUpMonoisotopicMassPeaksPage(ref ParametersSQLite sqliteDetails)
        {
            sqliteDetails.PageName = "T_Scan_MonoPeaks";//mono peaks only
            sqliteDetails.ColumnHeadersCounts.Add("T_Scan_MonoPeaks");
        }

        private static void SetUpPeaksCentricPage(ref ParametersSQLite sqliteDetails)
        {
            sqliteDetails.PageName = "T_Peak_Centric";//mono peaks only
            sqliteDetails.ColumnHeadersCounts.Add("T_Peak_Centric");
        }

        private static void SetUpScanCentricPage(ref ParametersSQLite sqliteDetails)
        {
            sqliteDetails.PageName = "T_Scan_Centric";//mono peaks only
            sqliteDetails.ColumnHeadersCounts.Add("T_Scan_Centric");
        }

        private static void SetUpFragmentCentricPage(ref ParametersSQLite sqliteDetails)
        {
            sqliteDetails.PageName = "T_Fragment_Centric";//mono peaks only
            sqliteDetails.ColumnHeadersCounts.Add("T_Fragment_Centric");
        }

        private static void SetUpAttributeCentricPage(ref ParametersSQLite sqliteDetails)
        {
            sqliteDetails.PageName = "T_Attribute_Centric";//mono peaks only
            sqliteDetails.ColumnHeadersCounts.Add("T_Attribute_Centric");
        }
       
        #endregion
    }
}
