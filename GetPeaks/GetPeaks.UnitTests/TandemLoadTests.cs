using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompareContrastDLL;
using DeconTools.Backend;
using DeconTools.Backend.Core;
using NUnit.Framework;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.Functions;
using GetPeaks_DLL.TandemSupport;
using GetPeaks_DLL.Objects.TandemMSObjects;
using GetPeaks_DLL;
using GetPeaks_DLL.ConosleUtilities;
using PNNLOmics.Algorithms.PeakDetection;
using PNNLOmics.Data;
using MemoryOverloadProfilierX86;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.SQLiteEngine;
using GetPeaks_DLL.Parallel;
using GetPeaks_DLL.Objects.Enumerations;
using GetPeaks_DLL.Go_Decon_Modules;
using GetPeaks_DLL.THRASH;
using GetPeaks_DLL.Objects.ParameterObjects;
using YAFMS_DB.GetPeaks;
using Peak = PNNLOmics.Data.Peak;
using XYData = PNNLOmics.Data.XYData;
using PNNLOmics.Data.Constants;
using GetPeaks_DLL.SQLite.DataTransferObjects;
using ScanCentric = GetPeaks_DLL.Objects.TandemMSObjects.ScanCentric;


namespace GetPeaks.UnitTests
{
    public class TandemLoadTests
    {
        /// <summary>
        /// sets up data for the tests
        /// </summary>
        /// <param name="newFile"></param>
        /// <param name="precursors"></param>
        /// <param name="scanLevelsWithTandem"></param>
        /// <param name="parameters"></param>
        public void PrepData(string[] argsIn, out InputOutputFileName newFile, out List<PrecursorInfo> precursors, out List<int> scanMSLevelList, out List<int> scanLevelsWithTandem, out SimpleWorkflowParameters parameters)
        {
            #region fail
            //string[] args = argsIn;//  

            ////set up file
            //ConvertArgsToFileName.GetFileName(args, out newFile);//newfile is setup here

            ////get number of scans in total
            //int limitFileToThisManyScans = 3000;//convert this from args
            //ProcessScanSpectraNumbers(newFile, limitFileToThisManyScans, out precursors, out scanMSLevelList, out scanLevelsWithTandem);

            //parameters = ArgsToParameters.Load(args);
            #endregion
        
            #region old
            string[] args = argsIn;//  

            //set up file
            ConvertArgsToFileName.GetFileName(args, out newFile);//newfile is setup here

            //get number of scans in total
            int sizeOfDatabase;
            //int limitFileToThisManyScans = 30000;
            int limitFileToThisManyScans = 3000;
            GatherDatasetInfo.GetMSLevelandSize(newFile, limitFileToThisManyScans, out sizeOfDatabase, out precursors);

            bool areTandemDetected = false;
            foreach (PrecursorInfo scan in precursors)
            {
                if (scan.MSLevel > 1)
                {
                    areTandemDetected = true;
                    break;
                }
            }

            scanMSLevelList = new List<int>();
            if (areTandemDetected)
            {
                foreach (PrecursorInfo parent in precursors)
                {
                    scanMSLevelList.Add(parent.MSLevel);
                }
                scanLevelsWithTandem = SelectScans.Ms1PrecursorScansWithTandem(scanMSLevelList);
            }
            else
            {
                scanLevelsWithTandem = new List<int>();
            }

            parameters = ArgsToParameters.Load(args);
            #endregion

        }

        

        private static string[] TestingArgs()
        {
            string[] args = new string[3];
            args[0] = @"P:\0_WorkParameterFileMSMS_GlycoPeptide.txt";
            args[1] = @"D:\Csharp\GetPeaksOutput";
            args[2] = @"V:\SQLiteBatchResult";
            return args;
        }
       
        /// <summary>
        /// load in data, convert args, and filter down to list of precursor scans with tandem, and set up parameters
        /// </summary>
        [Test]
        public void IdentifyActivePrecursorScansAndSetUpParameters()
        {
            InputOutputFileName newFile2;
            List<PrecursorInfo> precursors;
            List<int> scanLevels2;
            List<int> scanLevelsWithTandem2;
            SimpleWorkflowParameters parameters2;

            string[] args = TestingArgs();
            args[0] = @"D:\PNNL CSharp\0_BatchFiles\0_WorkParameterFileMSMS_GlycoPeptide.txt";
            
            PrepData(args, out newFile2, out precursors, out scanLevels2, out scanLevelsWithTandem2, out parameters2);

            Assert.AreEqual(3000, scanLevels2.Count);
            Assert.AreEqual(311, scanLevelsWithTandem2.Count);
            Assert.AreEqual(parameters2.Part1Parameters.StartScan, 1);
        }

        /// <summary>
        /// load tandem data and attach scan numbers to each
        /// </summary>
        [Test]
        public void TestLoadingTandemScanLocations()
        {
            InputOutputFileName newFile;
            List<PrecursorInfo> precursors;
            List<int> scanLevels;
            List<int> scanLevelsWithTandem;
            SimpleWorkflowParameters parameters;

            string[] args = TestingArgs();
            args[0] = @"D:\PNNL CSharp\0_BatchFiles\0_WorkParameterFileMSMS_GlycoPeptide.txt";

            PrepData(args, out newFile, out precursors, out scanLevels, out scanLevelsWithTandem, out parameters);

            char letter = 'K';
            const bool overrideParameterFile = true; //fit and ThrashPBR//false will use the parameter file
            ParalellController engineController;
            string sqLiteFile;
            string sqLiteFolder;
            int computersToDivideOver;
            int coresPerComputer;
            string logFile;
            //InputOutputFileName newFile;
            ParametersSQLite sqliteDetails;

            PopulatorArgs argsSK = new PopulatorArgs(args);

            ParametersForTesting.Load(letter, overrideParameterFile, out engineController, out sqLiteFile, out sqLiteFolder, out computersToDivideOver, out coresPerComputer, out logFile, out newFile, out sqliteDetails, argsSK);

            ParametersTHRASH thrashParameters = (ParametersTHRASH)engineController.ParameterStorage1;
            
            thrashParameters.FileInforamation = newFile;

            //new stuff
            List<TandemObject> TandemObjectList = GenerateTandemObjects.WithScans(newFile, scanLevels, scanLevelsWithTandem, thrashParameters);

            Assert.AreEqual(2353,TandemObjectList.Count);
        }

        /// <summary>
        /// attaches XYData each object
        /// </summary>
        [Test]
        public void LoadingXYDataToTandemObjects()
        {
            InputOutputFileName newFile;
            List<PrecursorInfo> precursors;
            List<int> scanLevels;
            List<int> scanLevelsWithTandem;
            SimpleWorkflowParameters parameters;

            string[] args = TestingArgs();
            args[0] = @"D:\PNNL CSharp\0_BatchFiles\0_WorkParameterFileMSMS_GlycoPeptide.txt";

            PrepData(args, out newFile, out precursors, out scanLevels, out scanLevelsWithTandem, out parameters);
            parameters.Part1Parameters.MSPeakDetectorPeakBR = 3;

            const char letter = 'K';
            const bool overrideParameterFile = true; //fit and ThrashPBR//false will use the parameter file
            ParalellController engineController;
            string sqLiteFile;
            string sqLiteFolder;
            int computersToDivideOver;
            int coresPerComputer;
            string logFile;
            //InputOutputFileName newFile;
            ParametersSQLite sqliteDetails;

            PopulatorArgs argsSK = new PopulatorArgs(args);

            ParametersForTesting.Load(letter, overrideParameterFile, out engineController, out sqLiteFile, out sqLiteFolder, out computersToDivideOver, out coresPerComputer, out logFile, out newFile, out sqliteDetails, argsSK);

            ParametersTHRASH thrashParameters = (ParametersTHRASH)engineController.ParameterStorage1;

            thrashParameters.FileInforamation = newFile;

            //new stuff
            List<TandemObject> TandemObjectList = GenerateTandemObjects.WithScans(newFile, scanLevels, scanLevelsWithTandem, thrashParameters);
            Assert.AreEqual(2353, TandemObjectList.Count);

            TandemObject testObject = TandemObjectList[0];
            //testObject.Parameters = parameters;

            testObject.LoadRun();
            
            testObject.LoadFragmentationData();
            Assert.AreEqual(10552, testObject.FragmentationData.Count);

            testObject.PeakPickFragmentationData();
            //Assert.AreEqual(945, testObject.FragmentationPeaks.Count);//peak data only
            //Assert.AreEqual(76, testObject.FragmentationPeaks.Count);// thresholded peak data
            Assert.AreEqual(331, testObject.FragmentationPeaks.Count);// thresholded peak data

            testObject.LoadPrecursorData();
            Assert.AreEqual(39115, testObject.PrecursorData.Count);
            
            testObject.PeakPickPrecursorData();
            //Assert.AreEqual(3194, testObject.PrecursorScanPeaks.Count);//peak data only
            Assert.AreEqual(1270, testObject.PrecursorScanPeaks.Count);// thresholded peak data

            testObject.LoadPrecursorMass();
            Assert.AreEqual(413.27, testObject.PrecursorMZ);
            Assert.AreEqual(413.27167923963498d, testObject.PrecursorMZCorrected);

            newFile.OutputSQLFileName = @"V:\DeisotopingDatabase.db";
            newFile.OutputPath = @"V:";
            testObject.NewFile = newFile;
            //testObject.Parameters.Part2Parameters.numberOfDeconvolutionThreads = 3;
            testObject.DeisotopeFragmentationPeaks();
            Assert.AreEqual(148, testObject.PrecursorMonoIsotopicPeaks.Count);
            testObject.PrecursorMonoIsotopicPeaks = null;

            testObject.DisposeRun();
        }

        /// <summary>
        /// check scan 7 msms and scan 8 msms
        /// </summary>
        [Test]
        public void testLoadingSpectraV2()
        {
            double massProton = Constants.SubAtomicParticles[SubAtomicParticleName.Proton].MassMonoIsotopic;
            
            InputOutputFileName newFile;
            List<PrecursorInfo> precursors;
            List<int> scanLevels;
            List<int> scanLevelsWithTandem;
            SimpleWorkflowParameters parameters;

            string[] args = TestingArgs();
            args[0] = @"D:\PNNL CSharp\0_BatchFiles\0_WorkParameterFileMSMS_GlycoPeptide.txt";

            PrepData(args, out newFile, out precursors, out scanLevels, out scanLevelsWithTandem, out parameters);
            parameters.Part1Parameters.MSPeakDetectorPeakBR = 3;

            //char letter = 'K';
            //string sqLiteFile;
            //string sqLiteFolder;
            //int computersToDivideOver;
            //int coresPerComputer;
            //string logFile;
            //ParametersTHRASH thrashParameters = ParametersForTesting.Load(letter, out sqLiteFile, out sqLiteFolder, out computersToDivideOver, out coresPerComputer, out logFile);
            //

            const char letter = 'K';
            const bool overrideParameterFile = true; //fit and ThrashPBR//false will use the parameter file
            ParalellController engineController;
            string sqLiteFile;
            string sqLiteFolder;
            int computersToDivideOver;
            int coresPerComputer;
            string logFile;
            //InputOutputFileName newFile;
            ParametersSQLite sqliteDetails;


            PopulatorArgs argsSK = new PopulatorArgs(args);

            ParametersForTesting.Load(letter, overrideParameterFile, out engineController, out sqLiteFile, out sqLiteFolder, out computersToDivideOver, out coresPerComputer, out logFile, out newFile, out sqliteDetails, argsSK);

            ParametersTHRASH thrashParameters = (ParametersTHRASH) engineController.ParameterStorage1;

            thrashParameters.FileInforamation = newFile;

            //new stuff
            int precursorScanNumber = 7;
            int tandemScanNumber = 8;
            PrecursorInfo precursor = precursors[tandemScanNumber];
   
            MSSpectra newSpectra = new MSSpectra();
            
            SpectraTandemPopulator newPopulator = new SpectraTandemPopulator();
            newPopulator.InputFileName = newFile;
            newPopulator.LoadRun();
            newSpectra.Scan = precursorScanNumber;
            MSSpectra tandemScan = new MSSpectra();
            newSpectra.ChildSpectra = new List<MSSpectra>();
            newSpectra.ChildSpectra.Add(tandemScan);
            newSpectra.ChildSpectra[0].Scan = tandemScanNumber;
            newSpectra.ChildSpectra[0].CollisionType = CollisionType.CID;
            newSpectra.ChildSpectra[0].MSLevel = 2;
            
            //TODO Update Paremters to a omics format
            newPopulator.XYDataSpectraGenerator(newSpectra);
            Assert.AreEqual(39115, newSpectra.Peaks.Count);

            newPopulator.XYDataSpectraGenerator(newSpectra.ChildSpectra[0]);
            Assert.AreEqual(10552, newSpectra.ChildSpectra[0].Peaks.Count);

            newPopulator.PeakGenerator(newSpectra, thrashParameters.PeakDetectionMultiParameters);
            Assert.AreEqual(1270, newSpectra.PeaksProcessed.Count);

            newPopulator.PeakGenerator(newSpectra.ChildSpectra[0], thrashParameters.PeakDetectionMultiParameters);
            Assert.AreEqual(331, newSpectra.ChildSpectra[0].PeaksProcessed.Count);

            newPopulator.LoadPrecursorMZ(newSpectra.ChildSpectra[0], precursor);
            Assert.AreEqual(413.27,newSpectra.ChildSpectra[0].PrecursorMZ);

            newPopulator.LoadRefinedMZ(newSpectra,newSpectra.ChildSpectra[0],massProton);
            Assert.AreEqual(413.2716792396352d, newSpectra.ChildSpectra[0].PrecursorPeak.XValue);
        }

        /// <summary>
        /// complete test for all scans
        /// </summary>
        [Test]
        public void testLoadingChromatogramTandem(string[] args)
        {
            Profiler profilier = new Profiler();
            profilier.printMemory("Start");

            bool saveMS1CentroidPeaks = true;
            bool saveMS2CentroidPeaks = true;
        
            bool saveMS1MonoisotopicHits = true;
            bool saveMS2MonoisotopicHits = true;
            bool savePrecursorMass = true;
            
            PopulatorArgs fileParameters = new PopulatorArgs(args);

            Object databaseLock = new Object();// for creating the main file.  if one thread is used, the lock is not needed

            #region finally clean loading

            ///////////////////////////////////////////////////////////////////////////////////////////////////
            const char letter = 'Z';
            const bool useParameterFileValues = true; //True will use fit and ThrashPBR from the file.  File will use it from the code 
            const int limitFileToThisManyScans = 9999999;//400//3000/99999
            //////////////////////////////////////////////////////////////////////////////////////////////////
            ParalellController engineController;
            string sqLiteFile;
            string sqLiteFolder;
            int computersToDivideOver;
            int coresPerComputer;
            string logFile;
            InputOutputFileName newFile;
            ParametersSQLite sqliteDetails;

            ParametersForTesting.Load(letter, useParameterFileValues, out engineController, out sqLiteFile, out sqLiteFolder, out computersToDivideOver, out coresPerComputer, out logFile, out newFile, out sqliteDetails, fileParameters);

            List<PrecursorInfo> precursors = new List<PrecursorInfo>();
            //List<int> scanMSLevelList = new List<int>();
            List<int> scanLevels = new List<int>();
            List<int> scanLevelsWithTandem = new List<int>();

            YafmsDbUtilities.ProcessScanSpectraNumbers(newFile, limitFileToThisManyScans, out precursors, out scanLevels, out scanLevelsWithTandem);
            //scanLevels = scanMSLevelList;

            EngineThrashDeconvolutor currentTHRASHEngine = (EngineThrashDeconvolutor)engineController.EngineStation.Engines[0];
            EngineSQLite currentSQLiteEngine = (EngineSQLite)engineController.SqlEngineStation.Engines[0];
            ParametersTHRASH currentThrashParameters = (ParametersTHRASH)engineController.ParameterStorage1;

            double massProton = Constants.SubAtomicParticles[SubAtomicParticleName.Proton].MassMonoIsotopic;

            #endregion

            //#region inside
            List<MSSpectra> results = new List<MSSpectra>();
            List<string> summary = new List<string>();

            int IdCounterForPeakCentric = 0;
            int IdCounterForMonoClusters = 0;
            int idCounterForScan = 0;
            //710
            //FragmentCentric currentLocationInDataset = new FragmentCentric();
            ScanCentric currentLocationInDataset = new ScanCentric();
            //int minCounter = 73;//500 scan
            int minCounter = 0;
            //ultimatly we need to add a loop outside of this that includes scans not selected for fragmentation
            //for(int i=701; i<scanLevelsWithTandem.Count-1;i++)//-1 because we are looking one ahead to see how many exist.  this could be replaced with a check loop
            for (int i = minCounter; i < scanLevelsWithTandem.Count - 1; i++)//-1 because we are looking one ahead to see how many exist.  this could be replaced with a check loop
            
            //for (int i = 0; i < scanLevelsWithTandem.Count - 200; i++)//-1 because we are looking one ahead to see how many exist.  this could be replaced with a check loop
            {
                #region inside
                //i = 500;
                //PrecursorInfo precursor = precursors[scanLevelsWithTandem[i]];

                int scan = scanLevelsWithTandem[i];//+1 to sync with file

                //scan = 500;
                //int msLevel = 0;
                //int parentScanNumber = 0;
                //int tandemScanNumber = 0;
                
                //scan = 6067;
                MSSpectra newSpectra = new MSSpectra();//parent scan
                 
                results.Add(newSpectra);

                SpectraTandemPopulator dataPopulator = new SpectraTandemPopulator();
                dataPopulator.InputFileName = newFile;
                dataPopulator.LoadRun();//this prevents making extra runs!
                newSpectra.Scan = scan;
                newSpectra.CollisionType = CollisionType.None;
                newSpectra.MSLevel = scanLevels[scanLevelsWithTandem[i]];

                #region populate precursor scan

                dataPopulator.XYDataSpectraGenerator(newSpectra);
                //Assert.AreEqual(39115, newSpectra.Peaks.Count);

                dataPopulator.CentroidGenerator(newSpectra, currentThrashParameters.PeakDetectionMultiParameters);

                List<ProcessedPeak> centroidedMS1PeaksHold = newSpectra.PeaksProcessed;

                List<PeakCentric> centroidedMS1PeaksToWrite = new List<PeakCentric>();//either will contain all centroids or signal
                if (saveMS1CentroidPeaks==true)
                {
                    YafmsDbUtilities.SetLocationInDataset(ref idCounterForScan, ref currentLocationInDataset, 1, 0, 0);//msLevel, ParentScan, TandemScan
                    SaveProcessedPeaksAtCentroidLevel(centroidedMS1PeaksHold, scan, ref IdCounterForPeakCentric, ref currentLocationInDataset, out centroidedMS1PeaksToWrite, currentSQLiteEngine, sqliteDetails);
                }

                //write  thermo scanInfoHeader to Scans Page
                //Scan + Peak is primiary Key
                //Background and NoiseLevelGlobal can be in scans page
                //benefits of SQLIte  don't have memory problems with loaging huge text file.  Especially multi raw files
                //database normmalization
                //HDF5 is competitor to SLQIte.  SQLite is on phones
                //XIC plot viewer
                //XIC is the new paradigm and an emerging market
                //possible eztend it into ta spectral network like library
                //firefox plugin
                //.JAR dll + >net DLL + webservice
                //Add YAFMS Page

                dataPopulator.PeakGenerator(newSpectra,currentThrashParameters.PeakDetectionMultiParameters);
                List<ProcessedPeak> thresholdedMs1PeaksHold = newSpectra.PeaksProcessed;

                if (saveMS1CentroidPeaks==false)
                {
                    YafmsDbUtilities.SetLocationInDataset(ref idCounterForScan, ref currentLocationInDataset, 1, 0, 0);//msLevel, ParentScan, TandemScan
                    SaveProcessedPeaksAtCentroidLevel(thresholdedMs1PeaksHold, scan, ref IdCounterForPeakCentric, ref currentLocationInDataset, out centroidedMS1PeaksToWrite, currentSQLiteEngine, sqliteDetails);
                    UpdateThresholdingInformation(thresholdedMs1PeaksHold, centroidedMS1PeaksToWrite, scan, currentSQLiteEngine, sqliteDetails);
                }

                StoreSummary(newSpectra, summary);
                List<MSSpectra> spectraPile = new List<MSSpectra>();
                spectraPile.Add(newSpectra);
                bool didthiswork2 = false;

                #endregion

                #region deisotope infront so we can use the information for charge state on the tandem scans

                //store peaks after
                List<IsotopeObject> precursorMonoMS1ResultsOut;//monos are stored here1
                dataPopulator.DeisotopePeaks(newSpectra, newSpectra.Scan, currentTHRASHEngine, out precursorMonoMS1ResultsOut);

                //TODO write precursor mono peaks so they are not lost with the overwrite below
                WriteMonoPeakListToDatabase(currentSQLiteEngine, newSpectra.PeaksProcessed, sqliteDetails);

                //sync precursorResultsOut with List<PeakCentric> centroidedPeaksToWrite and write atributes
                if (saveMS1MonoisotopicHits)
                {
                    //TODO the problem is here on scan 8 ashould be 654
                    UpdateMonoisotopicInformation(precursorMonoMS1ResultsOut, massProton, centroidedMS1PeaksToWrite, ref IdCounterForMonoClusters, currentSQLiteEngine, sqliteDetails);
                }

                if (newSpectra.PeaksProcessed.Count != precursorMonoMS1ResultsOut.Count)
                {
                    Console.WriteLine("no match at scan " + scan);
                }

                //repeat peak generator so information is in correct place
                dataPopulator.PeakGenerator(newSpectra, currentThrashParameters.PeakDetectionMultiParameters);
                //List<ProcessedPeak> thresholdedMs1PeaksHold = newSpectra.PeaksProcessed;

                if (saveMS1CentroidPeaks == true)//we still need to do this
                {
                    UpdateThresholdingInformation(thresholdedMs1PeaksHold, centroidedMS1PeaksToWrite, scan, currentSQLiteEngine, sqliteDetails);
                }

                //TODO write precursor peaks but keep them around (overwrite is line above)
                WritePeakListToDatabase(currentSQLiteEngine, newSpectra.PeaksProcessed, sqliteDetails);


                WriteScanToDatabase(currentSQLiteEngine, newSpectra, sqliteDetails);//this is quite usefull.  old but usefull.  soon to be replaced

                Console.WriteLine("There are " + newSpectra.PeaksProcessed.Count + " Precursor Scan Peaks and " + precursorMonoMS1ResultsOut.Count + " MonoisotopicMasses");

                #endregion

                //loop
                newSpectra.ChildSpectra = new List<MSSpectra>();
                int numberOfChildSpectra = scanLevelsWithTandem[i + 1] - scanLevelsWithTandem[i];//this tallies up how many ms2 to loop over
                for (int s = 1; s < numberOfChildSpectra; s++)//s=1 so we do not include precursor which is s=0
                {
                    #region inside

                    int testIndex = scanLevelsWithTandem[i] + s;
                    int testScanLevel = precursors[testIndex].MSLevel;
                    PrecursorInfo precursor = precursors[testIndex];

                    if (testScanLevel == 2)
                    {
                        #region inside
                        Console.WriteLine("--> working with i=" + i.ToString() + " and child = " + s.ToString());
                        MSSpectra tandemScan = new MSSpectra();
                        newSpectra.ChildSpectra.Add(tandemScan);
                        newSpectra.ChildSpectra[s - 1].Scan = scan + s;//-1 offset from precursor
                        newSpectra.ChildSpectra[s - 1].CollisionType = CollisionType.CID;//-1 offset from precursor
                        newSpectra.ChildSpectra[s - 1].MSLevel = testScanLevel;//-1 offset from precursor
                        newSpectra.ChildSpectra[s - 1].GroupID = scan;//precursor spectra is stored here
                        //TODO Update Paremters to a omics format

                        #region populate fragmentation scans

                        dataPopulator.XYDataSpectraGenerator(tandemScan);
                        //Assert.AreEqual(10552, newSpectra.ChildSpectra[s-1].Peaks.Count);//-1 offset from precursor

                        dataPopulator.CentroidGenerator(tandemScan, currentThrashParameters.PeakDetectionMultiParameters);
                        List<ProcessedPeak> centroidedMS2PeaksHold = tandemScan.PeaksProcessed;

                        //TODO write centroid data to database
                        List<PeakCentric> centroidedMS2PeaksToWrite = new List<PeakCentric>();
                        if (saveMS2CentroidPeaks==true)
                        {
                            YafmsDbUtilities.SetLocationInDataset(ref idCounterForScan, ref currentLocationInDataset, tandemScan.MSLevel, newSpectra.Scan, tandemScan.Scan);//msLevel, ParentScan, TandemScan
                            List<string> indexes = new List<string>();

                            SaveProcessedPeaksAtCentroidLevel(centroidedMS2PeaksHold, tandemScan.Scan, ref IdCounterForPeakCentric, ref currentLocationInDataset, out centroidedMS2PeaksToWrite, currentSQLiteEngine, sqliteDetails);
                        }

                        dataPopulator.PeakGenerator(tandemScan, currentThrashParameters.PeakDetectionMultiParameters);
                        List<ProcessedPeak> thresholdedMs2PeaksHold = tandemScan.PeaksProcessed;

                        if (saveMS2CentroidPeaks == false)
                        {
                            //SaveThresholdingInformationViaUpdate(thresholdedMs1PeaksHold, centroidedMS1PeaksToWrite, scan, currentSQLiteEngine, sqliteDetails);

                            //msLevel = 1;???
                            YafmsDbUtilities.SetLocationInDataset(ref idCounterForScan, ref currentLocationInDataset, tandemScan.MSLevel, newSpectra.Scan, tandemScan.Scan);//msLevel, ParentScan, TandemScan
                            SaveProcessedPeaksAtCentroidLevel(thresholdedMs2PeaksHold, tandemScan.Scan, ref IdCounterForPeakCentric, ref currentLocationInDataset, out centroidedMS2PeaksToWrite, currentSQLiteEngine, sqliteDetails);
                            UpdateThresholdingInformation(thresholdedMs2PeaksHold, centroidedMS2PeaksToWrite, tandemScan.Scan, currentSQLiteEngine, sqliteDetails);
                        }

                        //Assert.AreEqual(76, newSpectra.ChildSpectra[s-1].PeaksProcessed.Count);//-1 offset from precursor

                        //// write tandem peaks
                        //sqliteDetails.PageName = sqliteDetails.ColumnHeadersCounts[0];
                        //WritePeakListToDatabase(currentSQLiteEngine, tandemScan.PeaksProcessed, sqliteDetails);

                        //TODO write tandem peaks before they are repaced
                        WritePeakListToDatabase(currentSQLiteEngine, tandemScan.PeaksProcessed, sqliteDetails);
                        WriteScanToDatabase(currentSQLiteEngine, tandemScan, sqliteDetails);

                        if (saveMS2CentroidPeaks==true)
                        {
                            UpdateThresholdingInformation(thresholdedMs2PeaksHold, centroidedMS2PeaksToWrite, tandemScan.Scan, currentSQLiteEngine, sqliteDetails);
                        }
                        //continue with the precursor corrections
                        
                        dataPopulator.LoadPrecursorMZ(tandemScan, precursor);//grabs mass from dataset string

                        int maxCharge = 4;//TODO 5 does not work
                        
                        //possible work flow
                        //1.  take mass from thermo and run through glycolzyer
                        //2.  convert glycan hit to element composition
                        //3.  apply targeted to ms1 scan at several charge states
                        //4.  take best hit

                        dataPopulator.LoadRefinedMZ(newSpectra, tandemScan, massProton);

                        //TODO refine charge state based on precursor monolist
                        dataPopulator.ChargeStateViaTargeted(newSpectra, tandemScan, centroidedMS1PeaksHold, maxCharge);//current best
                        int charge = tandemScan.PrecursorPeak.Charge;

                        //newPopulator.RefineChargeStateOfPrecursor(precursorResultsOut, tandemScan);//store in tandem scan
                        //TODO then write


                        if (savePrecursorMass && tandemScan.PrecursorPeak.XValue>0)
                        {
                            List<ProcessedPeak> precursorPeak = new List<ProcessedPeak>(); precursorPeak.Add(tandemScan.PrecursorPeak);
                            tandemScan.PrecursorPeak.Charge = tandemScan.PrecursorPeak.Charge;
                            List<int> chargesForPrecursor = new List<int>(); chargesForPrecursor.Add(tandemScan.PrecursorPeak.Charge);//this is redundant as seen by line above

                            UpdatePrecursorMassInformation(precursorPeak, chargesForPrecursor, tandemScan, centroidedMS1PeaksToWrite, scan, currentSQLiteEngine, sqliteDetails);
                        }

                        WritePrecursorMzToDatabase(sqliteDetails, tandemScan, currentSQLiteEngine);

                        #endregion
                        //newPopulator.LoadPrecursorMZ(newSpectra, precursor, parameters);
                        //Assert.AreEqual(413.27, newSpectra.PrecursorMZ);
                        //Assert.AreEqual(413.27166748046875, newSpectra.PrecursorPeak.XValue);
                        if (1 == 0)
                        {
                            #region inside
                            switch (s)
                            {
                                case 1:
                                    {
                                        //Assert.AreEqual(413.27166748046875, newSpectra.PrecursorPeak.XValue);
                                        Assert.AreEqual(534.25848388671875, tandemScan.PrecursorPeak.XValue);
                                    }
                                    break;
                                case 2:
                                    {
                                        Assert.AreEqual(503.11404418945312, newSpectra.PrecursorPeak.XValue);
                                    }
                                    break;
                                case 3:
                                    {
                                        Assert.AreEqual(585.33258056640625, newSpectra.PrecursorPeak.XValue);
                                    }
                                    break;
                                case 4:
                                    {
                                        Assert.AreEqual(415.2587890625, newSpectra.PrecursorPeak.XValue);
                                    }
                                    break;
                                case 5:
                                    {
                                        Assert.AreEqual(783.11566162109375, newSpectra.PrecursorPeak.XValue);
                                    }
                                    break;
                                case 6:
                                    {
                                        Assert.AreEqual(718.023681640625, newSpectra.PrecursorPeak.XValue);
                                    }
                                    break;
                                case 7:
                                    {
                                        Assert.AreEqual(408.03079223632812, newSpectra.PrecursorPeak.XValue);
                                    }
                                    break;
                                case 8:
                                    {
                                        Assert.AreEqual(861.2279052734375, newSpectra.PrecursorPeak.XValue);
                                    }
                                    break;
                                case 9:
                                    {
                                        Assert.AreEqual(446.28042602539062, newSpectra.PrecursorPeak.XValue);
                                    }
                                    break;
                                case 10:
                                    {
                                        Assert.AreEqual(419.32125854492188, newSpectra.PrecursorPeak.XValue);
                                    }
                                    break;
                            }//endswitch
                            #endregion
                        }

                        StoreSummary(tandemScan, summary);
                        
                        //deisotope tandem scan Here!!!!!!!!!!
                        List<IsotopeObject> tandemResultsMonoOutMS2;
                        dataPopulator.DeisotopePeaks(tandemScan, tandemScan.Scan, currentTHRASHEngine, out tandemResultsMonoOutMS2);

                        //TODO write tandem mono peaks post deisotoping
                        WriteMonoPeakListToDatabase(currentSQLiteEngine, tandemScan.PeaksProcessed, sqliteDetails);


                        if (saveMS2MonoisotopicHits)
                        {
                            UpdateMonoisotopicInformation(tandemResultsMonoOutMS2, massProton, centroidedMS2PeaksToWrite, ref IdCounterForMonoClusters, currentSQLiteEngine, sqliteDetails);
                        }

                        if (tandemScan.PeaksProcessed.Count != tandemResultsMonoOutMS2.Count)
                        {
                            Console.WriteLine("no match at scan " + scan);
                        }
                        
                        CleanUpSpectra(tandemScan);

                        //idCounterForScan++; //next tandemscan (MS2)
                        #endregion
                    }

                    
                    #endregion  
                }//next spectra

                //deisotope precursor scan Here!!!!!!!!!!//moved up to top
                //newPopulator.DeisotopePeaks(newSpectra, newSpectra.Scan, currentTHRASHEngine);
                //WriteMonoPeakListToDatabase(currentSQLiteEngine, newSpectra.PeaksProcessed, sqliteDetails);
                //idCounterForScan++;//next precursor scan (MS1)

                newSpectra.Peaks = null;
                newSpectra.PeaksProcessed = null;

                //profilier.printMemory("PreDisposeRun");
                dataPopulator.DisposeRun();

                //profilier.printMemory("After PreDisposeRun");

                #endregion


               
            }//next precursor
            //results = null;

           

            List<string> TandemLines = new List<string>();

            string line;
            string header;
            string TandemMasses;
            string TandemMassesCorrected;
            string TandemPrecursorScans;
            string TandemAttachedScans ;
            string TandemChargeStates;

            foreach(MSSpectra scan in results)
            {
                foreach (MSSpectra childscan in scan.ChildSpectra)
                {
                    TandemMasses = childscan.PrecursorMZ.ToString();
                    TandemMassesCorrected = childscan.PrecursorPeak.XValue.ToString();
                    TandemPrecursorScans = (scan.Scan+1).ToString();//+1 to sync with Qual
                    TandemAttachedScans = (childscan.Scan+1).ToString();//+1 to sync with Qual
                    TandemChargeStates = childscan.PrecursorChargeState.ToString();

                    line =
                        TandemMasses + "," +
                        TandemMassesCorrected + "," +
                        TandemPrecursorScans + "," +
                        TandemAttachedScans + "," +
                        TandemChargeStates;
                    TandemLines.Add(line);
                }
            }

            header = "PrecursorMZ, DataPrecursorMZ, PrecursorScan, FragScan, PrecursorChargeState";

            StringListToDisk writer = new StringListToDisk();
            string outfile = @"V:\TandemLines.csv";
            //writer.toDiskStringList(outfile, TandemLines, header);

           
            profilier.printMemory("EndOFRun");
            profilier.printMemory("EndOFRun");
            //parameters.Part1Parameters.AllignmentToleranceInPPM = 1;
        }

        ////private static void SetLocationInDataset(ref int idCounterForScan, ref FragmentCentric currentLocationInDataset, int msLevel, int parentScan, int tandemScan)
        //private static void SetLocationInDataset(ref int idCounterForScan, ref ScanCentric currentLocationInDataset, int msLevel, int parentScan, int tandemScan)
        //{
        //    currentLocationInDataset.ScanID = idCounterForScan;
        //    currentLocationInDataset.MsLevel = msLevel;
        //    currentLocationInDataset.ParentScanNumber = parentScan;
        //    currentLocationInDataset.TandemScanNumber = tandemScan;
        //    idCounterForScan++;
        //}

        private static void UpdateMonoisotopicInformation(List<IsotopeObject> precursorResultsOut, double massProton, List<PeakCentric> centroidedMS1PeaksToWrite, ref int IdCounterForMonoClusters , EngineSQLite currentSQLiteEngine, ParametersSQLite sqliteDetails)
        {
            List<PeakCentric> monoPeakResults;
            //List<AttributeCentric> monoAtributes;

            //RemapIsotpeInforIntoCentricFormat(precursorResultsOut, ref IdCounterForMonoClusters, massProton, out monoPeakResults, out monoAtributes);
            RemapIsotpeInforIntoCentricFormat(precursorResultsOut, ref IdCounterForMonoClusters, massProton, out monoPeakResults);

            CompareResultsIndexes indexesFromCompare = new CompareResultsIndexes();

            //assign id to centric array

            List<XYData> peakXYData = new List<XYData>(); //mass vs id
            List<XYData> monoXYData = new List<XYData>(); //mass vs index in centric array

            foreach (var centroidedPeak in centroidedMS1PeaksToWrite)
            {
                XYData newPoint = new XYData(centroidedPeak.Mass, centroidedPeak.PeakID);
                peakXYData.Add(newPoint);
            }

            foreach (var monoPeak in monoPeakResults)
            {
                XYData newPoint = new XYData(monoPeak.Mass, monoPeak.PeakID);
                monoXYData.Add(newPoint);
            }

            CompareXYData(monoXYData, peakXYData, out indexesFromCompare);

            if (indexesFromCompare.IndexListAMatch.Count == monoPeakResults.Count)
            {
                for (int j = 0; j < indexesFromCompare.IndexListAMatch.Count; j++)
                {
                    int idfromPeak = Convert.ToInt32(peakXYData[indexesFromCompare.IndexListAMatch[j]].Y);
                    int indexFromMono = indexesFromCompare.IndexListBMatch[j];
                    monoPeakResults[indexFromMono].PeakID = idfromPeak;
                    //monoAtributes[indexFromMono].PeakID = idfromPeak;
                }
            }
            else
            {
                Console.WriteLine("Problem with writer sync " + indexesFromCompare.IndexListAMatch.Count + " vs " + monoPeakResults.Count);
                Console.ReadKey();
            }

            //DatabaseAttributeCentricObject tempAttributeCentric = new DatabaseAttributeCentricObject();
            //List<int> activeColumnsAttribute = SetActiveColumns(tempAttributeCentric, CentricOptions.AttributeCentricForMonoisotopicUpdate);
            //UpdateAttributeCentricToDatabase(currentSQLiteEngine, monoAtributes, sqliteDetails, activeColumnsAttribute);


            DatabasePeakCentricObject tempPeakCentric = new DatabasePeakCentricObject();
            List<int> activeColumnsPeak = SetActiveColumns(tempPeakCentric, CentricOptions.PeakCentricForMonoisotopicUpdate);
            UpdatePeakCentricToDatabase(currentSQLiteEngine, monoPeakResults, sqliteDetails, activeColumnsPeak);
        }

        private static void UpdateThresholdingInformation(List<ProcessedPeak> peaksResultsOut, List<PeakCentric> centroidedMS1PeaksToWrite, int scan, EngineSQLite currentSQLiteEngine, ParametersSQLite sqliteDetails)
        {
            //List<AttributeCentric> centroidedAttributesToWrite;
            List<ScanCentric> centroidedScansToWrite;
            List<PeakCentric> centroidedPeaksToWrite;
            //RemapPeaksToCentricForUpdate(peaksResultsOut, scan, out centroidedPeaksToWrite, out centroidedAttributesToWrite, out centroidedScansToWrite);
            RemapPeaksToCentricForUpdate(peaksResultsOut, scan, out centroidedPeaksToWrite, out centroidedScansToWrite);

            CompareResultsIndexes indexesFromCompare = new CompareResultsIndexes();

            //assign id to centric array

            List<XYData> peakXYData = new List<XYData>(); //mass vs id
            List<XYData> monoXYData = new List<XYData>(); //mass vs index in centric array

            foreach (var centroidedPeak in centroidedMS1PeaksToWrite)
            {
                XYData newPoint = new XYData(centroidedPeak.Mass, centroidedPeak.PeakID);
                peakXYData.Add(newPoint);
            }

            foreach (var monoPeak in peaksResultsOut)
            {
                XYData newPoint = new XYData(monoPeak.XValue, monoPeak.Height);
                monoXYData.Add(newPoint);
            }

            CompareXYData(monoXYData, peakXYData, out indexesFromCompare);

            if (indexesFromCompare.IndexListAMatch.Count == peaksResultsOut.Count)
            {
                for (int j = 0; j < indexesFromCompare.IndexListAMatch.Count; j++)
                {
                    int idfromPeak = Convert.ToInt32(peakXYData[indexesFromCompare.IndexListAMatch[j]].Y);
                    int indexFromMono = indexesFromCompare.IndexListBMatch[j];
                    centroidedPeaksToWrite[indexFromMono].PeakID = idfromPeak;
                    //centroidedAttributesToWrite[indexFromMono].PeakID = idfromPeak;
                }
            }
            else
            {
                Console.WriteLine("Problem with writer sync " + indexesFromCompare.IndexListAMatch.Count + " vs " + peaksResultsOut.Count);
                Console.ReadKey();
            }

            //DatabaseAttributeCentricObject tempAttributeCentric = new DatabaseAttributeCentricObject();
            //List<int> activeColumnsAttribute = SetActiveColumns(tempAttributeCentric, CentricOptions.AttributeCentricForThresholding);
            //UpdateAttributeCentricToDatabase(currentSQLiteEngine, centroidedAttributesToWrite, sqliteDetails, activeColumnsAttribute);

            DatabasePeakCentricObject tempPeakCentric = new DatabasePeakCentricObject();
            List<int> activeColumnsPeak = SetActiveColumns(tempPeakCentric, CentricOptions.PeakCentricForThresholding);
            UpdatePeakCentricToDatabase(currentSQLiteEngine, centroidedPeaksToWrite, sqliteDetails, activeColumnsPeak);
        }

        private static void UpdatePrecursorMassInformation(List<ProcessedPeak> precursorResultsOut, List<int> charges, MSSpectra tandemSpectra, List<PeakCentric> centroidedMS1PeaksToWrite, int parentScan, EngineSQLite currentSQLiteEngine, ParametersSQLite sqliteDetails)
        {
            //List<AttributeCentric> centroidedAttributesToWrite;
            List<PeakCentric> centroidedPeaksToWrite;
            //List<FragmentCentric> centroidedFragmentInfoToWrite;
            List<ScanCentric> centroidedScansToWrite;//precursor
            //RemapPrecursorToCentricForUpdate(precursorResultsOut, scan, tandemSpectra.Scan, out centroidedPeaksToWrite, out centroidedAttributesToWrite, out centroidedFragmentInfoToWrite, out centroidedScansToWrite);
            RemapPrecursorToCentricForUpdate(precursorResultsOut,charges, parentScan, tandemSpectra.Scan, out centroidedPeaksToWrite, out centroidedScansToWrite);


            CompareResultsIndexes indexesFromCompare = new CompareResultsIndexes();

            //assign id to centric array

            List<XYData> peakXYData = new List<XYData>(); //mass vs id
            List<XYData> monoXYData = new List<XYData>(); //mass vs index in centric array

            foreach (var centroidedPeak in centroidedMS1PeaksToWrite)
            {
                XYData newPoint = new XYData(centroidedPeak.Mass, centroidedPeak.PeakID);
                peakXYData.Add(newPoint);
            }

            foreach (var monoPeak in precursorResultsOut)
            {
                XYData newPoint = new XYData(monoPeak.XValue, monoPeak.Height);
                monoXYData.Add(newPoint);
            }

            CompareXYData(monoXYData, peakXYData, out indexesFromCompare);
            if (indexesFromCompare.IndexListAMatch.Count == precursorResultsOut.Count)
            {
                for (int j = 0; j < indexesFromCompare.IndexListAMatch.Count; j++)
                {
                    int idfromPeak = Convert.ToInt32(peakXYData[indexesFromCompare.IndexListAMatch[j]].Y);
                    int indexFromMono = indexesFromCompare.IndexListBMatch[j];
                    //centroidedAttributesToWrite[indexFromMono].PeakID = idfromPeak;
                    centroidedScansToWrite[indexFromMono].PeakID = idfromPeak;
                    //centroidedFragmentInfoToWrite[indexFromMono].ScanID = idfromPeak;
                    centroidedPeaksToWrite[indexFromMono].PeakID = idfromPeak;
                }
            }
            else
            {
                Console.WriteLine("Problem with writer sync " + indexesFromCompare.IndexListAMatch.Count + " vs " + precursorResultsOut.Count);
                Console.ReadKey();
            }

            if (indexesFromCompare.IndexListAMatch.Count > 0)//we need a peak id to write
            {
                //DatabaseAttributeCentricObject tempAttributeCentric = new DatabaseAttributeCentricObject();
                //List<int> activeColumnsAttribute = SetActiveColumns(tempAttributeCentric, CentricOptions.AttributeCentricForPrecursorIon);
                //UpdateAttributeCentricToDatabase(currentSQLiteEngine, centroidedAttributesToWrite, sqliteDetails, activeColumnsAttribute);
                //replace with scan centric

                DatabaseScanCentricObject tempScanCentric = new DatabaseScanCentricObject();
                List<int> activeColumnsScan = SetActiveColumns(tempScanCentric, CentricOptions.ScanCentricForPrecursor);
                UpdateScanCentricToDatabase(currentSQLiteEngine, centroidedScansToWrite, sqliteDetails, activeColumnsScan);//this works correctly when scan starts at 0

                DatabasePeakCentricObject tempPeakCentric = new DatabasePeakCentricObject();
                List<int> activeColumnsPeak = SetActiveColumns(tempPeakCentric, CentricOptions.PeakCentricForPrecursor);
                UpdatePeakCentricToDatabase(currentSQLiteEngine, centroidedPeaksToWrite, sqliteDetails, activeColumnsPeak);//this works correctly when scan starts at 0


                //DatabaseFragmentCentricObject tempFragmentCentric = new DatabaseFragmentCentricObject();
                //List<int> activeColumnsFragment = SetActiveColumns(tempFragmentCentric, CentricOptions.FragmentCentricForPrecursorIon);
                //UpdateFragmentCentricToDatabase(currentSQLiteEngine, centroidedFragmentInfoToWrite, sqliteDetails, activeColumnsFragment);
            }
            else
            {
                Console.WriteLine("no peak in database found so we can't write in scan " + parentScan);
                Console.ReadKey();
            }
        }

        //private static void SaveProcessedPeaksAtCentroidLevel(List<ProcessedPeak> centroidedPeaksHold, int scan, ref int IdCounterForPeakCentric, ref FragmentCentric currentLocationInDataset, out List<PeakCentric> centroidedPeaksToWrite, EngineSQLite currentSQLiteEngine, ParametersSQLite sqliteDetails)
        private static void SaveProcessedPeaksAtCentroidLevel(List<ProcessedPeak> centroidedPeaksHold, int scan, ref int IdCounterForPeakCentric, ref ScanCentric currentLocationInDataset, out List<PeakCentric> centroidedPeaksToWrite, EngineSQLite currentSQLiteEngine, ParametersSQLite sqliteDetails)

        {
            //RemapRegion
            //List<AttributeCentric> centroidedAttributesToWrite;
            List<ScanCentric> centroidedScansToWrite;
            //List<PeakCentric> centroidedPeaksToWrite;above
            //List<FragmentCentric> centroidedFragmentsToWrite;//not implemented
            //RemapPeaksToCentricForWrite(centroidedPeaksHold, scan, ref IdCounterForPeakCentric, ref currentLocationInDataset, out centroidedPeaksToWrite, out centroidedAttributesToWrite, out centroidedScansToWrite, out centroidedFragmentsToWrite);
            RemapPeaksToCentricForWrite(centroidedPeaksHold, scan, ref IdCounterForPeakCentric, ref currentLocationInDataset, out centroidedPeaksToWrite, out centroidedScansToWrite);

            DatabaseTransferObject getIndexes = new DatabasePeakCentricLiteObject();
            List<string> indexes = getIndexes.IndexedColumns;

            WritePeakCentricToDatabase(currentSQLiteEngine, centroidedPeaksToWrite, sqliteDetails);            
            //WriteAttributeCentricToDatabase(currentSQLiteEngine, centroidedAttributesToWrite, sqliteDetails);

            WriteScanCentricToDatabase(currentSQLiteEngine, centroidedScansToWrite, sqliteDetails);
            //WriteFragmentCentricToDatabase(currentSQLiteEngine, centroidedFragmentsToWrite, sqliteDetails);
        }

        //private static void RemapIsotpeInforIntoCentricFormat(List<IsotopeObject> precursorResultsOut, ref int IdCounterForMonoClusters, double massProton, out List<PeakCentric> monoResults, out List<AttributeCentric> monoAtributres)
        private static void RemapIsotpeInforIntoCentricFormat(List<IsotopeObject> precursorResultsOut, ref int IdCounterForMonoClusters, double massProton, out List<PeakCentric> monoResults)
        {
            monoResults = new List<PeakCentric>();
            //monoAtributres = new List<AttributeCentric>();

            foreach (IsotopeObject isotopeObject in precursorResultsOut)
            {
                int currentIsotope = IdCounterForMonoClusters;

                for (int j = 0; j < isotopeObject.IsotopeList.Count; j++)
                {
                    PNNLOmics.Data.Peak currentPeakInIsotope = isotopeObject.IsotopeList[j]; //for each

                    PeakCentric newPeak = new PeakCentric();
                    //AttributeCentric newAttribute = new AttributeCentric();

                    newPeak.MonoisotopicClusterID = currentIsotope;
                    newPeak.Score = isotopeObject.FitScore;
                    newPeak.ChargeState = isotopeObject.Charge;

                    newPeak.isSignal = true;
                    newPeak.isCentroided = true;
                    newPeak.isCharged = true;

                    MergePeakIntoPeakCentric(ref newPeak, currentPeakInIsotope);

                    //newAttribute.isSignal = true;
                    //newAttribute.isCentroided = true;
                    //newAttribute.isCharged = true;

                    if (j == 0) //dealing with monoisootpic mass
                    {
                        newPeak.MassMonoisotopic = isotopeObject.MonoIsotopicMass;
                        newPeak.isMonoisotopic = true;
                        //newAttribute.isMonoisotopic = true;
                    }
                    else //isotope
                    {
                        newPeak.MassMonoisotopic = ConvertMzToMono.Execute(currentPeakInIsotope.XValue, isotopeObject.Charge, massProton);
                        newPeak.isIsotope = true;
                        //newAttribute.isIsotope = true;
                    }

                    monoResults.Add(newPeak);
                    //monoAtributres.Add(newAttribute);
                }

                IdCounterForMonoClusters++;//next cluster
            }
        }

        //private static void RemapPeaksToCentricForWrite(List<ProcessedPeak> centroidedPeaksHold, int scan, ref int IdCounterForPeakCentric, ref FragmentCentric currentLocationInDataset, out List<PeakCentric> centroidedPeaksToWrite, out List<AttributeCentric> centroidedAttributesToWrite, out List<ScanCentric> centroidedScansToWrite, out List<FragmentCentric> centroidedFragmentsToWrite)
        private static void RemapPeaksToCentricForWrite(List<ProcessedPeak> centroidedPeaksHold, int scan, ref int IdCounterForPeakCentric, ref ScanCentric currentLocationInDataset, out List<PeakCentric> centroidedPeaksToWrite, out List<ScanCentric> centroidedScansToWrite)
        {
            centroidedPeaksToWrite = new List<PeakCentric>();
            //centroidedAttributesToWrite = new List<AttributeCentric>();
            centroidedScansToWrite = new List<ScanCentric>();
            //centroidedFragmentsToWrite = new List<FragmentCentric>();

            //FragmentCentric newFragmentInfo = new FragmentCentric();
            //newFragmentInfo.ScanID = currentLocationInDataset.ScanID;
            //newFragmentInfo.MsLevel = currentLocationInDataset.MsLevel;
            //newFragmentInfo.ParentScanNumber = currentLocationInDataset.ParentScanNumber;
            //newFragmentInfo.TandemScanNumber = currentLocationInDataset.TandemScanNumber;

            ScanCentric newScaninfo = new ScanCentric();
            newScaninfo.ScanID = currentLocationInDataset.ScanID;
            newScaninfo.ScanNumLc = scan;

            newScaninfo.MsLevel = currentLocationInDataset.MsLevel;
            newScaninfo.ParentScanNumber = currentLocationInDataset.ParentScanNumber;
            newScaninfo.TandemScanNumber = currentLocationInDataset.TandemScanNumber;

            foreach (ProcessedPeak peak in centroidedPeaksHold)
            {
                PeakCentric newPeak = ConvertMSPeakToPeakCentric.Convert(peak);
                newPeak.PeakID = IdCounterForPeakCentric;
                newPeak.ScanID = currentLocationInDataset.ScanID;

                //AttributeCentric newAtrribute = new AttributeCentric();
                //newAtrribute.PeakID = IdCounterForPeakCentric;
                //newAtrribute.isCentroided = true;
                //newAtrribute.isCharged = true;

                newPeak.isCentroided = true;
                newPeak.isCharged = true;

                //ScanCentric newScaninfo = new ScanCentric();
                //newScaninfo.ScanID = currentLocationInDataset.ScanID;
                //newScaninfo.ScanNumLc = scan;

                //FragmentCentric newFragmentInfo = new FragmentCentric();
                //newFragmentInfo.ScanID = idCounterForScan;
                //newFragmentInfo.MsLevel = msLevel;

                IdCounterForPeakCentric++;

                centroidedPeaksToWrite.Add(newPeak);
                //centroidedAttributesToWrite.Add(newAtrribute);
                
                //centroidedFragmentsToWrite.Add(newFragmentInfo);
            }

            centroidedScansToWrite.Add(newScaninfo);
            //centroidedFragmentsToWrite.Add(newFragmentInfo);
        }

        //private static void RemapPeaksToCentricForUpdate(List<ProcessedPeak> centroidedPeaksHold, int scan , out List<PeakCentric> centroidedPeaksToWrite, out List<AttributeCentric> centroidedAttributesToWrite, out List<ScanCentric> centroidedScansToWrite)
        private static void RemapPeaksToCentricForUpdate(List<ProcessedPeak> centroidedPeaksHold, int scan , out List<PeakCentric> centroidedPeaksToWrite, out List<ScanCentric> centroidedScansToWrite)
        
        {
            centroidedPeaksToWrite = new List<PeakCentric>();
            //centroidedAttributesToWrite = new List<AttributeCentric>();
            centroidedScansToWrite = new List<ScanCentric>();

            int newID = 0;//this needs to be determined for update
            int newScanID = 0;//this needs to be determined for update
            foreach (ProcessedPeak peak in centroidedPeaksHold)
            {
                PeakCentric newPeak = ConvertMSPeakToPeakCentric.Convert(peak);
                newPeak.PeakID = newID;

                newPeak.isCentroided = true;
                newPeak.isCharged = true;
                newPeak.isSignal = true;
                //AttributeCentric newAtrribute = new AttributeCentric();
                //newAtrribute.PeakID = newID;
                //newAtrribute.isCentroided = true;
                //newAtrribute.isCharged = true;
                //newAtrribute.isSignal = true;

                ScanCentric newScaninfo = new ScanCentric();
                newScaninfo.ScanID = newScanID;
                newScaninfo.ScanNumLc = scan;

                //IdCounter++;

                centroidedPeaksToWrite.Add(newPeak);
                //centroidedAttributesToWrite.Add(newAtrribute);
                centroidedScansToWrite.Add(newScaninfo);
            }
        }

        //private static void RemapPrecursorToCentricForUpdate(List<ProcessedPeak> centroidedPeaksHold, int parentScanNumber, int tandemScanNumber, out List<PeakCentric> centroidedPeaksToWrite, out List<AttributeCentric> centroidedAttributesToWrite, out List<FragmentCentric> centroidedFragmentToWrite, out List<ScanCentric> centroidedScanToWrite)
        private static void RemapPrecursorToCentricForUpdate(List<ProcessedPeak> centroidedPeaksHold, List<int> charges, int parentScanNumber, int tandemScanNumber, out List<PeakCentric> centroidedPeaksToWrite, out List<ScanCentric> centroidedScanToWrite)
        
        {
            centroidedPeaksToWrite = new List<PeakCentric>();
            //centroidedAttributesToWrite = new List<AttributeCentric>();
            //centroidedFragmentToWrite = new List<FragmentCentric>();
            centroidedScanToWrite = new List<ScanCentric>();

            int newID = 0;//this needs to be determined for update
            int newScanID = tandemScanNumber;//this needs to be determined for update

            for (int i = 0; i < centroidedPeaksHold.Count; i++)
            {
                ProcessedPeak peak = centroidedPeaksHold[i];
                int charge = charges[i];
                
                PeakCentric newPeak = ConvertMSPeakToPeakCentric.Convert(peak);
                newPeak.PeakID = newID;
                newPeak.ChargeState = charge;
                newPeak.isCentroided = true;
                newPeak.isCharged = true;
                newPeak.isSignal = true;
                //AttributeCentric newAtrribute = new AttributeCentric();
                //newAtrribute.PeakID = newID;
                //newAtrribute.isPrecursorMass = true;

                //FragmentCentric newFragmentinfo = new FragmentCentric();
                //newFragmentinfo.ScanID = newScanID;
                //newFragmentinfo.ParentScanNumber = parentScanNumber;
                //newFragmentinfo.TandemScanNumber = tandemScanNumber;
                //newPeak.isPrecursorMass = true;

                

                ScanCentric newScanPrecursorInfo = new ScanCentric();
                newScanPrecursorInfo.PeakID = newID;
                newScanPrecursorInfo.ScanID = newScanID;

                newScanPrecursorInfo.ParentScanNumber = parentScanNumber;
                newScanPrecursorInfo.TandemScanNumber = tandemScanNumber;

                //IdCounter++;

                centroidedPeaksToWrite.Add(newPeak);
                //centroidedAttributesToWrite.Add(newAtrribute);
                //centroidedFragmentToWrite.Add(newFragmentinfo);
                centroidedScanToWrite.Add(newScanPrecursorInfo);
            }
        }

        
        /// <summary>
        /// so we only update the columns we need to
        /// </summary>
        /// <param name="temp">this is generic so any database transfer object can be used</param>
        /// <param name="centricType">enumerated options since it is harde coded options</param>
        /// <returns></returns>
        private static List<int> SetActiveColumns(DatabaseTransferObject temp, CentricOptions centricType)
        {
            List<int> activeColumns = new List<int>();
            for (int i = 0; i < temp.Columns.Count; i++)
            {
                activeColumns.Add(0);
            }

            //1 will be updated
            switch (centricType)
            {
                case CentricOptions.AttributeCentricForMonoisotopicUpdate:
                    {
                        activeColumns[0] = 1; //write PeakID
                        activeColumns[1] = 1; //write isSignal
                        activeColumns[2] = 1; //write isCentroided
                        activeColumns[3] = 1; //writes isMonoisotopic
                        activeColumns[4] = 1; //writes isIsotope
                        activeColumns[5] = 0; //writes isMostAbundant
                        activeColumns[6] = 0; //writes isCharged
                        activeColumns[7] = 0; //writes isCorrected
                        activeColumns[8] = 0; //writes isPrecursorMass
                    }
                    break;
                case CentricOptions.PeakCentricForMonoisotopicUpdate:
                    {
                        activeColumns[0] = 1; //write PeakID
                        activeColumns[1] = 0; //write ScanID
                        activeColumns[2] = 0; //write GroupID
                        activeColumns[3] = 1; //write MonoisotopicClusterID
                        activeColumns[4] = 0; //writes FeatureClusterID
                        activeColumns[5] = 0; //writes Mz
                        activeColumns[6] = 1; //writes Charge
                        activeColumns[7] = 0; //writes Height
                        activeColumns[8] = 0; //writes Width
                        activeColumns[9] = 0; //writes Background
                        activeColumns[10] = 0; //writes LocalSignalToNoise
                        activeColumns[11] = 1; //writes MassMonoisotopic
                        activeColumns[12] = 1; //writes Score
                        activeColumns[13] = 0; //writes AmbiguityScore

                        activeColumns[14] = 1; //write isSignal
                        activeColumns[15] = 1; //write isCentroided
                        activeColumns[16] = 1; //writes isMonoisotopic//writes 0 when this is an isotope
                        activeColumns[17] = 1; //writes isIsotope//writes 0 when this is a mono
                        activeColumns[18] = 0; //writes isMostAbundant
                        activeColumns[19] = 0; //writes isCharged
                        activeColumns[20] = 0; //writes isCorrected
                        //activeColumns[8] = 1; //writes isPrecursorMass
                    }
                    break;
                
                case CentricOptions.AttributeCentricForThresholding:
                    {
                        activeColumns[0] = 1; //write PeakID
                        activeColumns[1] = 1; //write isSignal
                        activeColumns[2] = 0; //write isCentroided
                        activeColumns[3] = 0; //writes isMonoisotopic
                        activeColumns[4] = 0; //writes isIsotope
                        activeColumns[5] = 0; //writes isMostAbundant
                        activeColumns[6] = 0; //writes isCharged
                        activeColumns[7] = 0; //writes isCorrected
                        activeColumns[8] = 0; //writes isPrecursorMass
                    }
                    break;
                case CentricOptions.PeakCentricForThresholding:
                    {
                        activeColumns[0] = 1; //write PeakID
                        activeColumns[1] = 0; //write ScanID
                        activeColumns[2] = 0; //write GroupID
                        activeColumns[3] = 0; //write MonoisotopicClusterID
                        activeColumns[4] = 0; //writes FeatureClusterID
                        activeColumns[5] = 0; //writes Mz
                        activeColumns[6] = 0; //writes Charge
                        activeColumns[7] = 0; //writes Height
                        activeColumns[8] = 0; //writes Width
                        activeColumns[9] = 1; //writes Background
                        activeColumns[10] = 1; //writes LocalSignalToNoise
                        activeColumns[11] = 0; //writes MassMonoisotopic
                        activeColumns[12] = 0; //writes Score
                        activeColumns[13] = 0; //writes AmbiguityScore

                        activeColumns[14] = 1; //write isSignal
                        activeColumns[15] = 0; //write isCentroided
                        activeColumns[16] = 0; //writes isMonoisotopic
                        activeColumns[17] = 0; //writes isIsotope
                        activeColumns[18] = 0; //writes isMostAbundant
                        activeColumns[19] = 0; //writes isCharged
                        activeColumns[20] = 0; //writes isCorrected
                        //activeColumns[21] = 0; //writes isPrecursorMass
                    }
                    break;
                case CentricOptions.AttributeCentricForPrecursorIon:
                    {
                        activeColumns[0] = 1; //write PeakID
                        activeColumns[1] = 0; //write isSignal
                        activeColumns[2] = 0; //write isCentroided
                        activeColumns[3] = 0; //writes isMonoisotopic
                        activeColumns[4] = 0; //writes isIsotope
                        activeColumns[5] = 0; //writes isMostAbundant
                        activeColumns[6] = 0; //writes isCharged
                        activeColumns[7] = 0; //writes isCorrected
                        activeColumns[8] = 1; //writes isPrecursorMass
                    }
                    break;
                case CentricOptions.FragmentCentricForPrecursorIon:
                    {
                        activeColumns[0] = 1; //write ScanID
                        activeColumns[1] = 0; //write MsLevel????????????????????????????????????????????????????????????????????
                        activeColumns[2] = 1; //write ParentScanNumber
                        activeColumns[3] = 1; //writes TandemScanNumber
                    }
                    break;
                case CentricOptions.ScanCentricForPrecursor:
                    {
                        activeColumns[0] = 1; //write ScanID
                        activeColumns[1] = 1; //write PeakID
                        activeColumns[2] = 0; //write ScanNumLc
                        activeColumns[3] = 0; //write ElutionTime
                        activeColumns[4] = 0; //writes FrameNumberDt
                        activeColumns[5] = 0; //write ScanNumDt
                        activeColumns[6] = 0; //writes DriftTime

                        activeColumns[7] = 0; //write MsLevel????????????????????????????????????????????????????????????????????
                        activeColumns[8] = 0; //write ParentScanNumber
                        activeColumns[9] = 0; //writes TandemScanNumber

                    }
                    break;
                case CentricOptions.PeakCentricForPrecursor:
                    {
                        activeColumns[0] = 1; //write PeakID
                        activeColumns[1] = 0; //write ScanID
                        activeColumns[2] = 0; //write GroupID
                        activeColumns[3] = 0; //write MonoisotopicClusterID
                        activeColumns[4] = 0; //writes FeatureClusterID
                        activeColumns[5] = 0; //writes Mz
                        activeColumns[6] = 1; //writes Charge
                        activeColumns[7] = 0; //writes Height
                        activeColumns[8] = 0; //writes Width
                        activeColumns[9] = 0; //writes Background
                        activeColumns[10] = 0; //writes LocalSignalToNoise
                        activeColumns[11] = 0; //writes MassMonoisotopic
                        activeColumns[12] = 0; //writes Score
                        activeColumns[13] = 0; //writes AmbiguityScore

                        activeColumns[14] = 1; //write isSignal
                        activeColumns[15] = 1; //write isCentroided
                        activeColumns[16] = 0; //writes isMonoisotopic
                        activeColumns[17] = 0; //writes isIsotope
                        activeColumns[18] = 0; //writes isMostAbundant
                        activeColumns[19] = 1; //writes isCharged
                        activeColumns[20] = 0; //writes isCorrected
                        //activeColumns[21] = 0; //writes isPrecursorMass

                    }
                    break;
                default:
                    {
                        Console.WriteLine("Fail");
                        Console.ReadKey();
                    }
                    break;
            }


            return activeColumns;
        }

        /// <summary>
        /// feeds into SetActiveColumns
        /// </summary>
        private enum CentricOptions
        {
            PeakCentricForMonoisotopicUpdate,
            ScanCentricForPrecursor,
            PeakCentricForPrecursor,
            AttributeCentricForMonoisotopicUpdate,
            FragmentCentricForPrecursorIon,
            AttributeCentricForPrecursorIon,
            AttributeCentricForThresholding,
            PeakCentricForThresholding
        }

        /// <summary>
        /// compare to centroided data so we can do updates
        /// </summary>
        /// <param name="dataXY"></param>
        /// <param name="libraryXY"></param>
        /// <param name="indexesFromCompare"></param>
        private static void CompareXYData(List<PNNLOmics.Data.XYData> dataXY, List<PNNLOmics.Data.XYData> libraryXY, out CompareResultsIndexes indexesFromCompare)
        {
            if (dataXY.Count > 0 && libraryXY.Count > 0)//TODO this needs to be built into prepCompare
            {
                //CompareFX
                IConvert letsConvert = new Converter();
                SetListsToCompare prepCompare = new SetListsToCompare();
                CompareController compareHere = new CompareController();
                double massTolleranceMatch = 2;

                List<double> dataList = letsConvert.XYDataToMass(libraryXY);
                //TODO changed 11-9-12
                List<double> libraryList = letsConvert.XYDataToMass(dataXY);
                ;

                //TODO changed 11-9-12
                //CompareInputLists inputListsNonGlycan = prepCompare.SetThem(sortedDataList, libraryListNonGlycan);
                CompareInputLists inputListsNonGlycan = prepCompare.SetThem(dataList, libraryList);

                indexesFromCompare = new CompareResultsIndexes();
                CompareResultsValues valuesFromCompare = compareHere.compareFX(inputListsNonGlycan, massTolleranceMatch, ref indexesFromCompare);
            }
            else
            {
                indexesFromCompare = new CompareResultsIndexes();//fail, there is nothing in common because one list is 0;
            }
        }

        /// <summary>
        /// memory management
        /// </summary>
        /// <param name="tandemScan"></param>
        private static void CleanUpSpectra(MSSpectra tandemScan)
        {
            tandemScan.Peaks = null;
            tandemScan.PeaksProcessed = null;
        }

        /// <summary>
        /// like a convert. could be pulled out
        /// </summary>
        /// <param name="newPeak"></param>
        /// <param name="currentPeakInIsotope"></param>
        private static void MergePeakIntoPeakCentric(ref PeakCentric newPeak, Peak currentPeakInIsotope)
        {
            newPeak.Mass = currentPeakInIsotope.XValue;
            newPeak.Height = currentPeakInIsotope.Height;
            newPeak.Width = currentPeakInIsotope.Width;
            newPeak.LocalSignalToNoise = currentPeakInIsotope.LocalSignalToNoise;
            newPeak.Background = currentPeakInIsotope.Background;
        }


        #region centric writers

        private static void WritePeakCentricToDatabase(EngineSQLite currentEngine, List<PeakCentric> peakList, ParametersSQLite sqliteDetails)
        {
            
            List<PeakCentric> currentList = peakList;
            if (peakList.Count > 0)
            {
                sqliteDetails.PageName = sqliteDetails.ColumnHeadersCounts[4];//4 PeakCentric 

                bool didthisworkTandem = currentEngine.WritePeakCentricList(currentEngine, currentList, sqliteDetails);
            }
        }

        private static void WriteScanCentricToDatabase(EngineSQLite currentEngine, List<ScanCentric> peakList, ParametersSQLite sqliteDetails)
        {
            List<ScanCentric> currentList = peakList;
            if (peakList.Count > 0)
            {
                sqliteDetails.PageName = sqliteDetails.ColumnHeadersCounts[5];//5 ScanCentric 
                bool didthisworkTandem = currentEngine.WriteScanCentricList(currentEngine, currentList, sqliteDetails);
            }
        }

        //private static void WriteAttributeCentricToDatabase(EngineSQLite currentEngine, List<AttributeCentric> peakList, ParametersSQLite sqliteDetails)
        //{
        //    List<AttributeCentric> currentList = peakList;
        //    if (peakList.Count > 0)
        //    {
        //        sqliteDetails.PageName = sqliteDetails.ColumnHeadersCounts[6];//6 AttributeCentric 
        //        bool didthisworkTandem = currentEngine.WriteAttributeCentricList(currentEngine, currentList, sqliteDetails);
        //    }
        //}

        //private static void WriteFragmentCentricToDatabase(EngineSQLite currentEngine, List<FragmentCentric> peakList, ParametersSQLite sqliteDetails)
        //{
        //    List<FragmentCentric> currentList = peakList;
        //    if (peakList.Count > 0)
        //    {
        //        sqliteDetails.PageName = sqliteDetails.ColumnHeadersCounts[7];//7 FragmentCentric 
        //        bool didthisworkTandem = currentEngine.WriteFragmentCentricList(currentEngine, currentList, sqliteDetails);
        //    }
        //}


        //private static void UpdateAttributeCentricToDatabase(EngineSQLite currentEngine, List<AttributeCentric> peakList, ParametersSQLite sqliteDetails, List<int> activeColumns)
        //{
        //    List<AttributeCentric> currentList = peakList;
        //    if (peakList.Count > 0)
        //    {
        //        sqliteDetails.PageName = sqliteDetails.ColumnHeadersCounts[6];//6 AttributeCentric 
        //        bool didthisworkTandem = currentEngine.UpdateAttributeCentricList(currentEngine, currentList, sqliteDetails, activeColumns);
        //    }
        //}

        private static void UpdatePeakCentricToDatabase(EngineSQLite currentEngine, List<PeakCentric> peakList, ParametersSQLite sqliteDetails, List<int> activeColumns)
        {
            List<PeakCentric> currentList = peakList;
            if (peakList.Count > 0)
            {
                sqliteDetails.PageName = sqliteDetails.ColumnHeadersCounts[4];//4 PeakCentric 

                //DatabaseTransferObject getIndexes = new DatabasePeakCentricLiteObject();
                //List<string> indexes = getIndexes.IndexedColumns;

                bool didthisworkTandem = currentEngine.UpdatePeakCentricList(currentEngine, currentList, sqliteDetails, activeColumns);
            }
        }

        //private static void UpdateFragmentCentricToDatabase(EngineSQLite currentEngine, List<FragmentCentric> peakList, ParametersSQLite sqliteDetails, List<int> activeColumns)
        //{
        //    List<FragmentCentric> currentList = peakList;
        //    if (peakList.Count > 0)
        //    {
        //        sqliteDetails.PageName = sqliteDetails.ColumnHeadersCounts[7];//7 FragmentCentric 
        //        bool didthisworkTandem = currentEngine.UpdateFragmentCentricList(currentEngine, currentList, sqliteDetails, activeColumns);
        //    }
        //}

        private static void UpdateScanCentricToDatabase(EngineSQLite currentEngine, List<ScanCentric> peakList, ParametersSQLite sqliteDetails, List<int> activeColumns)
        {
            List<ScanCentric> currentList = peakList;
            if (peakList.Count > 0)
            {
                sqliteDetails.PageName = sqliteDetails.ColumnHeadersCounts[5];//5 ScanCentric 
                bool didthisworkTandem = currentEngine.UpdateScanCentricList(currentEngine, currentList, sqliteDetails, activeColumns);
            }
        }

        #endregion









        /// <summary>
        /// testdatabase
        /// </summary>
        [Test]
        public void testSavingTandemTOSQlite()
        {
            MSSpectra newSpectra = new MSSpectra(); //parent scan
            newSpectra.MSLevel = 7;
            newSpectra.PeakProcessingLevel = PeakProcessingLevel.Other;
            newSpectra.PeaksProcessed = new List<ProcessedPeak>();

            ProcessedPeak newPeak = new ProcessedPeak();
            newPeak.Background = 10;
            newPeak.Height = 10000000;
            newPeak.LocalLowestMinimaHeight = 5;
            newPeak.LocalSignalToNoise = 7;
            newPeak.MinimaOfHigherMassIndex = 18;
            newPeak.MinimaOfLowerMassIndex = 19;
            newPeak.SignalToBackground = 12;
            newPeak.SignalToNoiseGlobal = 13;
            newPeak.SignalToNoiseLocalHighestMinima = 11;
            newPeak.Width = 35;
            newPeak.XValue = 1000.123456789;

            ProcessedPeak newPeak2 = new ProcessedPeak();
            newPeak2.Background = 11;
            newPeak2.Height = 10000001;
            newPeak2.LocalLowestMinimaHeight = 51;
            newPeak2.LocalSignalToNoise = 71;
            newPeak2.MinimaOfHigherMassIndex = 181;
            newPeak2.MinimaOfLowerMassIndex = 191;
            newPeak2.SignalToBackground = 121;
            newPeak2.SignalToNoiseGlobal = 131;
            newPeak2.SignalToNoiseLocalHighestMinima = 111;
            newPeak2.Width = 351;
            newPeak2.XValue = 1001.123456789;

            int scan = 5509;
            newSpectra.PeaksProcessed.Add(newPeak);
            newSpectra.PeaksProcessed.Add(newPeak2);

            ParametersSQLite sqliteDetails = new ParametersSQLite(@"K:\SQLite\", "PeaksDatabase", 1, 1);

            YafmsDbUtilities.SetUpSimplePeaksPage(ref sqliteDetails);
            YafmsDbUtilities.SetUpScansRelationshipPage(ref sqliteDetails);

            Object databaseLock = new Object();// for creating the main file.  if one thread is used, the lock is not needed
            EngineSQLite sqLiteEngineBuilder = new EngineSQLite(databaseLock, 0);
            sqliteDetails.CoresPerComputer = 1;
            ParalellEngineStation sqlEngineStation = sqLiteEngineBuilder.SetupEngines(sqliteDetails);

            EngineSQLite currentEngine = (EngineSQLite)sqlEngineStation.Engines[0];

            sqliteDetails.PageName = sqliteDetails.ColumnHeadersCounts[0];
            bool didthiswork = currentEngine.WriteProcessedPeakList(currentEngine, newSpectra.PeaksProcessed, sqliteDetails);

            List<MSSpectra> SpectraPile = new List<MSSpectra>();
            SpectraPile.Add(newSpectra);

            sqliteDetails.PageName = sqliteDetails.ColumnHeadersCounts[1];
            bool didthiswork2 = currentEngine.WriteScanList(currentEngine, SpectraPile, sqliteDetails);
            //    //bool didthiswork = currentEngine.WriteIsosData((EngineSQLite)SQLEngineStation.Engines[0], thrashResults.ResultsFromRunConverted, scanNumber);

        }

        private static void WritePrecursorMzToDatabase(ParametersSQLite sqliteDetails, MSSpectra tandemScan, EngineSQLite currentEngine)
        {
            List<ProcessedPeak> precursorList = new List<ProcessedPeak>();
            precursorList.Add(tandemScan.PrecursorPeak);
            sqliteDetails.PageName = sqliteDetails.ColumnHeadersCounts[2];
            double mzFromHeader = tandemScan.PrecursorMZ;
            int scanNumber = tandemScan.Scan;
            int charge = tandemScan.PrecursorChargeState;
            bool didthisworkTandem = currentEngine.WritePrecursorPeak(currentEngine, tandemScan.PrecursorPeak, sqliteDetails, mzFromHeader, scanNumber, charge);
        }

        private static void WritePeakListToDatabase(EngineSQLite currentEngine, List<ProcessedPeak> peakList, ParametersSQLite sqliteDetails)
        {
            List<ProcessedPeak> currentList = peakList;
            if (peakList.Count > 0)
            {
                sqliteDetails.PageName = sqliteDetails.ColumnHeadersCounts[0];
                bool didthisworkTandem = currentEngine.WriteProcessedPeakList(currentEngine, currentList, sqliteDetails);
            }
        }

        private static void WriteMonoPeakListToDatabase(EngineSQLite currentEngine, List<ProcessedPeak> peakList, ParametersSQLite sqliteDetails)
        {
            List<ProcessedPeak> currentList = peakList;
            if (peakList.Count > 0)
            {
                sqliteDetails.PageName = sqliteDetails.ColumnHeadersCounts[3];
                bool didthisworkTandem = currentEngine.WriteProcessedPeakList(currentEngine, currentList, sqliteDetails);
            }
        }

        private static void WriteScanToDatabase(EngineSQLite currentEngine, MSSpectra tandemScan, ParametersSQLite sqliteDetails)
        {
            List<MSSpectra> spectraPile;
            bool didthiswork2;
            spectraPile = new List<MSSpectra>();
            spectraPile.Add(tandemScan);
            sqliteDetails.PageName = sqliteDetails.ColumnHeadersCounts[1];
            didthiswork2 = currentEngine.WriteScanList(currentEngine, spectraPile, sqliteDetails);
        }

        //private static void SetUpProcessedPeaksPage(ref ParametersSQLite sqliteDetails)
        //{
        //    sqliteDetails.PageName = "T_Scan_Peaks";//peaks
        //    sqliteDetails.ColumnHeadersCounts.Add("T_Scan_Peaks");
        //}

        //private static void SetUpScansRelationshipPage(ref ParametersSQLite sqliteDetails)
        //{
        //    sqliteDetails.PageName = "T_Scans";//relationships
        //    sqliteDetails.ColumnHeadersCounts.Add("T_Scans");
        //}

        //private static void SetUpPrecursorPeakPage(ref ParametersSQLite sqliteDetails)
        //{
        //    sqliteDetails.PageName = "T_Scans_Precursor_Peaks";//precursor Peaks
        //    sqliteDetails.ColumnHeadersCounts.Add("T_Scans_Precursor_Peaks");
        //}

        //private static void SetUpMonoisotopicMassPeaksPage(ref ParametersSQLite sqliteDetails)
        //{
        //    sqliteDetails.PageName = "T_Scan_MonoPeaks";//mono peaks only
        //    sqliteDetails.ColumnHeadersCounts.Add("T_Scan_MonoPeaks");
        //}

        private static void StoreSummary(MSSpectra newSpectra, List<string> summary)
        {
            summary.Add(newSpectra.Scan.ToString() + "has " + newSpectra.PeaksProcessed.Count.ToString() + " peaks with a MSLevel of " + newSpectra.MSLevel.ToString());
        }

        private static void WriteVariablesToConsole(int startScan, int endScan, double DataSpecificMassNeutron, double part1SN, double part2SN, MemorySplitObject newMemorySplitter)
        {
            //Console.WriteLine("InputFileName: " + newFile.InputFileName +"\n");

            Console.WriteLine("Startscan: " + startScan + " EndScan: " + endScan);
            Console.WriteLine("Part1 SN: " + part1SN + " Part2 SN: " + part2SN);
            Console.WriteLine("DataSpecificMassNeutron: " + DataSpecificMassNeutron);
            Console.WriteLine("Processing Block# " + (newMemorySplitter.BlockNumber + 1).ToString() + " of " + newMemorySplitter.NumberOfBlocks);
            Console.WriteLine("Press Enter");
            //Console.ReadKey();
        }
    }
}
