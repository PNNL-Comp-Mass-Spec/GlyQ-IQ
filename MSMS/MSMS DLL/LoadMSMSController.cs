using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using DeconTools.Backend.DTO;
using DeconTools.Backend.ProcessingTasks;
using DeconTools.Backend.Runs;
using DeconTools.Backend.Utilities;
using GetPeaks_DLL;
using GetPeaks_DLL.Common_Switches;
using GetPeaks_DLL.ConosleUtilities;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.Objects;
using PNNLOmics.Algorithms.PeakDetection;
using PNNLOmics.Data;
using GetPeaks_DLL.AnalysisSupport;
using GetPeaks_DLL.Go_Decon_Modules;

namespace MSMS_DLL
{
    public class LoadMSMSController:IDisposable
    {
        #region Properties

        //public MSGeneratorFactory msgenFactory { get; set; }
        //public Task msGenerator {get;set;}
        //public SimpleWorkflowParameters Parameters;
        //public DeconToolsPeakDetector msPeakDetector;

        #endregion



        #region idisposable
        public virtual void Close()
        {
            //this.msgenFactory = null;
            //this.msGenerator = null;
            //this.Parameters = null;
            //this.msPeakDetector = null;
        }

        public virtual void Dispose()
        {
            this.Close();
        }

        #endregion
        #region Public Methods
        /// <summary>
        /// Create Eluting Peaks
        /// </summary>
        /// <param name="run"></param>
        public List<TandemObject> LoadData(string[] args, out InputOutputFileName newFile, out SimpleWorkflowParameters parameters)//create eluting peaks and store them in run.ResultsCollection.ElutingPeaks
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            List<TandemObject> tandemScan = new List<TandemObject>();
            TandemObject newpoint = new TandemObject();
            tandemScan.Add(newpoint);

            #region setup main parameter file from args

            List<string> mainParameterFile;
            List<string> stringListFromParameterFile;
            ConvertArgsToStringList(args, out mainParameterFile, out stringListFromParameterFile);

            #endregion

            #region convert parameter file to variables
            string serverDataFileName = stringListFromParameterFile[0];
            string folderID = stringListFromParameterFile[1];
            int startScan = Convert.ToInt32(stringListFromParameterFile[2]);
            int endScan = Convert.ToInt32(stringListFromParameterFile[3]);
            int serverBlockTotal = Convert.ToInt32(stringListFromParameterFile[4]);
            int serverBlock = Convert.ToInt32(stringListFromParameterFile[5]);
            double DataSpecificMassNeutron = Convert.ToDouble(stringListFromParameterFile[6]);
            double part1SN = Convert.ToDouble(stringListFromParameterFile[7]);
            double part2SN = Convert.ToDouble(stringListFromParameterFile[8]);
            string DeconType = stringListFromParameterFile[9];

            int numberOfDeconvolutionThreads = Convert.ToInt32(stringListFromParameterFile[10]);

            //part1SN = 1;
            DeconvolutionType loadedDeconvolutionType = new DeconvolutionType();
            ConvertAToB converter = new ConvertAToB();
            loadedDeconvolutionType = converter.stringTODeconvolutionType(DeconType);

            MemorySplitObject newMemorySplitter = new MemorySplitObject();
            newMemorySplitter.NumberOfBlocks = serverBlockTotal;
            newMemorySplitter.BlockNumber = serverBlock;
            #endregion

            #region create run and setup files to load and save
            //InputOutputFileName newFile = new InputOutputFileName();
            newFile = new InputOutputFileName();
            newFile.InputFileName = serverDataFileName;
            newFile.OutputFileName = mainParameterFile[1] + folderID + "_" + serverBlock + @".txt";
            newFile.OutputSQLFileName = mainParameterFile[2] + folderID + @".db";

            WriteVariablesToConsole(newFile, startScan, endScan, DataSpecificMassNeutron, part1SN, part2SN, newMemorySplitter);

            FileInfo fi = new FileInfo(newFile.InputFileName);
            bool exists = fi.Exists;

            Console.WriteLine("CreateRun: " + newFile.InputFileName + " and its existance is " + exists.ToString());
            RunFactory rf = new RunFactory();
            //Console.WriteLine("RunFactory Setup, press enter to continue");
            //Console.ReadKey();

            Run run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, newFile.InputFileName);
            //Run run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.Finnigan, inputDataFilename);
            Console.WriteLine("after run was created");
            //Console.ReadKey();
            #endregion

            #region Setup Parameters and MSMS controller

            parameters = new SimpleWorkflowParameters();

            //part 1 peak detector decontools only.  this gets overwritten below
            parameters.Part1Parameters.MSPeakDetectorPeakBR = 1.3;
            parameters.Part1Parameters.ElutingPeakNoiseThreshold = 2;
           
            //this contains the first call to Decon tools Engine V2
            //ElutingPeakFinderController controller = new ElutingPeakFinderController(run, parameters);
            GetMSMSSpectraController controller = new GetMSMSSpectraController(run, parameters);  
            controller.MSMSOmicsPeakDecection = true;//allways because false uses a non working decon
            controller.MSMSStorePoints = true;
            controller.MSMSStoreXYData = true;
            controller.OrbitrapThresholdToggle = false;

            controller.Parameters.FileNameUsed = newFile.InputFileName;

            //set here for omics typical run is 3
            //controller.Parameters.Part1Parameters.ElutingPeakNoiseThreshold = 3;//data 1 when NoiseRemoved, take 3 sigma off before the orbitrap filter
            controller.Parameters.Part1Parameters.ElutingPeakNoiseThreshold = part1SN;//when NoiseRemoved, take 3 sigma off before the orbitrap filter
            controller.Parameters.Part1Parameters.NoiseType = InstrumentDataNoiseType.NoiseRemoved;
            controller.Parameters.Part1Parameters.PeakFitType = DeconTools.Backend.Globals.PeakFitType.LORENTZIAN;//this makes a large difference in the number of peaks
            controller.Parameters.Part1Parameters.StartScan = startScan;
            controller.Parameters.Part1Parameters.StopScan = endScan;
            controller.Parameters.Part1Parameters.ParametersOrbitrap.massNeutron = DataSpecificMassNeutron;
            controller.Parameters.Part1Parameters.ParametersOrbitrap.DeltaMassTollerancePPM = 6000;//6000 is start point
  
            #endregion

            #region get precursor masses
            
            List<TandemObject> precursorList = new List<TandemObject>();
            GetDataController dataLoad = new GetDataController();

            int sizeOfDatabase = run.GetNumMSScans() - 1;//this is a crude way to get the size of the database.  -1 is need or the scan set creation will fail
            
            precursorList = dataLoad.GetTandemDataFromYAFMS(newFile, sizeOfDatabase, parameters.Part1Parameters.StartScan, parameters.Part1Parameters.StopScan);
            
            #endregion

            #region decon implementation to get XYData and peaks

            //Get XYData and convert to Peaks
            Console.WriteLine("     Loading XYData...");

            AttachXYDataAndPeaks(run, controller, precursorList);

            int precursorCount = precursorList.Count;
            Console.WriteLine("      " + precursorCount + " TandemObjects were loaded");
            #endregion

            
            
            stopWatch.Stop();

            TimeSpan ts = stopWatch.Elapsed;
            Console.WriteLine("This took " + ts + "seconds");

            return precursorList;
        }

        /// <summary>
        /// gets the data from the sqlite datafile
        /// </summary>
        /// <param name="args"></param>
        /// <param name="newFile"></param>
        /// <param name="parameters"></param>
        /// <param name="targetScanNumber"></param>
        /// <returns></returns>
        public TandemObject LoadScan(string[] args, InputOutputFileName newFile, out SimpleWorkflowParameters parameters, int targetScanNumber, int sizeOfDatabase)//create eluting peaks and store them in run.ResultsCollection.ElutingPeaks
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            TandemObject tandemScan = new TandemObject();
            //TODO pull out all args to the front

            #region setup main parameter file from args

            List<string> mainParameterFile;
            List<string> stringListFromParameterFile;
            ConvertArgsToStringList(args, out mainParameterFile, out stringListFromParameterFile);

            #endregion

            #region convert parameter file to variables
            string serverDataFileName = stringListFromParameterFile[0];
            string folderID = stringListFromParameterFile[1];
            int startScan = Convert.ToInt32(stringListFromParameterFile[2]);
            int endScan = Convert.ToInt32(stringListFromParameterFile[3]);
            int serverBlockTotal = Convert.ToInt32(stringListFromParameterFile[4]);
            int serverBlock = Convert.ToInt32(stringListFromParameterFile[5]);
            double DataSpecificMassNeutron = Convert.ToDouble(stringListFromParameterFile[6]);
            double part1SN = Convert.ToDouble(stringListFromParameterFile[7]);
            double part2SN = Convert.ToDouble(stringListFromParameterFile[8]);
            string DeconType = stringListFromParameterFile[9];

            int numberOfDeconvolutionThreads = Convert.ToInt32(stringListFromParameterFile[10]);

            //part1SN = 1;
            DeconvolutionType loadedDeconvolutionType = new DeconvolutionType();
            ConvertAToB converter = new ConvertAToB();
            loadedDeconvolutionType = converter.stringTODeconvolutionType(DeconType);

            MemorySplitObject newMemorySplitter = new MemorySplitObject();
            newMemorySplitter.NumberOfBlocks = serverBlockTotal;
            newMemorySplitter.BlockNumber = serverBlock;
            #endregion

            #region create run and setup files to load and save
            newFile.InputFileName = serverDataFileName;
            newFile.OutputFileName = mainParameterFile[1] + folderID + "_" + serverBlock + @".txt";
            newFile.OutputSQLFileName = mainParameterFile[2] + folderID + @".db";

            //WriteVariablesToConsole(newFile, startScan, endScan, DataSpecificMassNeutron, part1SN, part2SN, newMemorySplitter);

            FileInfo fi = new FileInfo(newFile.InputFileName);
            bool exists = fi.Exists;

            //Console.WriteLine("CreateRun: " + newFile.InputFileName + " and its existance is " + exists.ToString());
            RunFactory rf = new RunFactory();
            //Console.WriteLine("RunFactory Setup, press enter to continue");
            //Console.ReadKey();


            //Run run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, newFile.InputFileName);
            //Run run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.Finnigan, inputDataFilename);

            string fileType;
            Run run = GoCreateRun.CreateRun(newFile, out fileType);
            //Console.WriteLine("after run was created");
            //Console.ReadKey();
            #endregion

            #region Setup Parameters and MSMS controller

            parameters = new SimpleWorkflowParameters();

            //part 1 peak detector decontools only.  this gets overwritten below
            parameters.Part1Parameters.MSPeakDetectorPeakBR = 1.3;
            parameters.Part1Parameters.ElutingPeakNoiseThreshold = 2;

            //this contains the first call to Decon tools Engine V2
            //ElutingPeakFinderController controller = new ElutingPeakFinderController(run, parameters);
            GetMSMSSpectraController controller = new GetMSMSSpectraController(run, parameters);
            controller.MSMSOmicsPeakDecection = true;//allways because false uses a non working decon
            controller.MSMSStorePoints = false;
            controller.MSMSStoreXYData = true;
            controller.OrbitrapThresholdToggle = false;

            controller.Parameters.FileNameUsed = newFile.InputFileName;

            //set here for omics typical run is 3
            //controller.Parameters.Part1Parameters.ElutingPeakNoiseThreshold = 3;//data 1 when NoiseRemoved, take 3 sigma off before the orbitrap filter
            controller.Parameters.Part1Parameters.ElutingPeakNoiseThreshold = part1SN;//when NoiseRemoved, take 3 sigma off before the orbitrap filter
            controller.Parameters.Part1Parameters.NoiseType = InstrumentDataNoiseType.NoiseRemoved;
            controller.Parameters.Part1Parameters.PeakFitType = DeconTools.Backend.Globals.PeakFitType.LORENTZIAN;//this makes a large difference in the number of peaks
            //controller.Parameters.Part1Parameters.StartScan = startScan;
            //controller.Parameters.Part1Parameters.StopScan = endScan;
            controller.Parameters.Part1Parameters.StartScan = targetScanNumber;
            controller.Parameters.Part1Parameters.StopScan = targetScanNumber;
            controller.Parameters.Part1Parameters.ParametersOrbitrap.massNeutron = DataSpecificMassNeutron;
            controller.Parameters.Part1Parameters.ParametersOrbitrap.DeltaMassTollerancePPM = 6000;//6000 is start point

            #endregion

            #region get precursor masses

            List<TandemObject> precursorList = new List<TandemObject>();
            GetDataController dataLoad = new GetDataController();

            //int sizeOfDatabase = run.GetNumMSScans() - 1;//this is a crude way to get the size of the database.  -1 is need or the scan set creation will fail
///populate precursor information (Precursor MZ)
            switch (fileType)
            {
                case ("raw"):
                    {
                        precursorList = dataLoad.GetTandemDataFromRAW(newFile, sizeOfDatabase, parameters.Part1Parameters.StartScan, parameters.Part1Parameters.StopScan);
                    }
                    break;
                case ("yafms"):
                    { 
                        precursorList = dataLoad.GetTandemDataFromYAFMS(newFile, sizeOfDatabase, parameters.Part1Parameters.StartScan, parameters.Part1Parameters.StopScan);
                    }
                    break;
                default:
                    { }
                    break;

            }
            
            #endregion

            #region decon implementation to get XYData and peaks

            //Get XYData and convert to Peaks
            //Console.WriteLine("     Loading XYData...");

            AttachXYDataAndPeaks(run, controller, precursorList);

            int precursorCount = precursorList.Count;
            //Console.WriteLine("      " + precursorCount + " TandemObjects were loaded");
            #endregion



            stopWatch.Stop();

            TimeSpan ts = stopWatch.Elapsed;
            //Console.WriteLine("This took " + ts + "seconds");

            return precursorList[0];
        }

        #endregion

        #region private Methods

        private static void ConvertArgsToStringList(string[] args, out List<string> mainParameterFile, out List<string> stringListFromParameterFile)
        {
            //TODO remove file target and make everyting work
            FileTarget target = new FileTarget();
            target = FileTarget.WorkStandardTest;//<---used to target what computer we are working on:  Home or Work.  Server is covered by command line
            
            mainParameterFile = new List<string>();

            #region switch from server to desktop based on number of args
            if (args.Length == 0)//debugging
            {
                mainParameterFile.Add(""); mainParameterFile.Add(""); mainParameterFile.Add("");
            }
            else
            {
                //Console.WriteLine("ParseArgs");
                ParseStrings parser = new ParseStrings();
                mainParameterFile = parser.Parse3Args(args);
            }

            switch (target)
            {
                #region fileswitch
                case FileTarget.WorkStandardTest://work
                    {
                        //loaded from pre build events
                    }
                    break;
                case FileTarget.HomeStandardTest:
                    {
                        mainParameterFile[0] = @"G:\PNNL Files\PNNL CSharp\0_BatchFiles\0_HomeParameterFileSN09a.txt";
                        mainParameterFile[1] = @"G:\PNNL Files\CSharp\GetPeaksOutput\TextBatchResult";
                        mainParameterFile[2] = @"G:\PNNL Files\CSharp\GetPeaksOutput\SQLiteBatchResult";
                    }
                    break;
                #endregion
            }
            #endregion

            //load parameters
            stringListFromParameterFile = new List<string>();
            FileIteratorStringOnly loadParameter = new FileIteratorStringOnly();
            stringListFromParameterFile = loadParameter.loadStrings(mainParameterFile[0]);
        }

        private static void AttachXYDataAndPeaks(Run run, GetMSMSSpectraController controller, List<TandemObject> precursorList)
        {

            foreach (TandemObject precursorObject in precursorList)
            {
                XYDataAndPeakHolderObject dataXYandPeaks = controller.GrabXYData(run, precursorObject.DatasetScanNumber);

                precursorObject.ScanSet = dataXYandPeaks.ScanSet;
                precursorObject.PeaksDECON = dataXYandPeaks.PeaksDECON;
                precursorObject.PeaksOMICS = dataXYandPeaks.PeaksOMICS;
                precursorObject.SpectraDataDECON = dataXYandPeaks.SpectraDataDECON;
                precursorObject.SpectraDataOMICS = dataXYandPeaks.SpectraDataOMICS;
            }
        }

        private static void WriteVariablesToConsole(InputOutputFileName newFile, int startScan, int endScan, double DataSpecificMassNeutron, double part1SN, double part2SN, MemorySplitObject newMemorySplitter)
        {
            //Console.WriteLine("InputFileName: " + newFile.InputFileName +"\n");
               
            Console.WriteLine("SQLite Output: " + newFile.OutputSQLFileName + "\n");

            Console.WriteLine("Startscan: " + startScan + " EndScan: " + endScan);
            Console.WriteLine("Part1 SN: " + part1SN + " Part2 SN: " + part2SN);
            Console.WriteLine("DataSpecificMassNeutron: " + DataSpecificMassNeutron);
            Console.WriteLine("Processing Block# " + (newMemorySplitter.BlockNumber + 1).ToString() + " of " + newMemorySplitter.NumberOfBlocks);
            Console.WriteLine("Press Enter");
            //Console.ReadKey();
        }
        #endregion

    }
}
