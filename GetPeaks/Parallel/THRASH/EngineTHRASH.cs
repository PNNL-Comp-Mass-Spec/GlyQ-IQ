using System;
using System.Collections.Generic;
using GetPeaks_DLL.Go_Decon_Modules;
using DeconTools.Backend.Core;
using PNNLOmics.Data;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.Functions;
using DeconTools.Backend.ProcessingTasks;

namespace Parallel.THRASH
{
    public class EngineTHRASH:ParalellEngine
    {
        /// <summary>
        /// allows for a finite number of transformers to exist
        /// </summary>
        //public DeconToolsV2.HornTransform.clsHornTransform TransformEngine { get; set; }
        public HornDeconvolutor Deconvolutor { get; set; }

        /// <summary>
        /// unique filename to write to
        /// </summary>
        public string SQLiteTranformerFileName { get; set; }
        public string SQLiteTranformerFolderPath { get; set; }

        /// <summary>
        /// unique name to read from //allows for input folder copying
        /// </summary>
        public string UniqueFileName { get; set; }


        public Run Run { get; set; }

        //details about scans
        public List<PrecursorInfo> PrecursorMetaData { get; set; }

        public EngineTHRASH()
        {
            //this.TransformEngine = new DeconToolsV2.HornTransform.clsHornTransform();
            Deconvolutor = new HornDeconvolutor();
            PrecursorMetaData = new List<PrecursorInfo>();
        }

        public EngineTHRASH(ParametersTHRASH transformerParameterSetup, Object databaseLock, int engineNumber)
        {
            
            //TransformEngine = new DeconToolsV2.HornTransform.clsHornTransform();
            

            //Deconvolutor = new HornDeconvolutor(parametersDT.HornTransformParameters);
            Deconvolutor = new HornDeconvolutor(transformerParameterSetup.ThrashParameters.DeconThrashParameters);
            PrecursorMetaData = new List<PrecursorInfo>();

            Parameters = transformerParameterSetup;

            //TransformEngine.TransformParameters = transformerParameterSetup.ThrashParameters.DeconThrashParameters;
            //this.TransformEngine.TransformParameters = transformerParameterSetup.loadDeconEngineHornParameters();

            SQLiteTranformerFileName = Parameters.FileInforamation.OutputSQLFileName;// SQLiteName;
            SQLiteTranformerFolderPath = Parameters.FileInforamation.OutputPath;// SQLiteFolder;
            //UniqueFileName = Parameters.FileInforamation.InputFileName;
            UniqueFileName = RemoveEnding.RAW(transformerParameterSetup.FileInforamation.InputFileName) + " (" + engineNumber + ").RAW";

            DatabaseLock = databaseLock;

            //uniqu number id for engine
            EngineNumber = engineNumber;

            ErrorLog = new List<string>();

            ErrorLog.Add("start Engine " + engineNumber.ToString());
        }

        /// <summary>
        /// Sets up a finite number of Thrash Enines for multithreading
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="numberOfCoresPerComputer"></param>
        /// <returns></returns>
        public override ParalellEngineStation SetupEngines(ParalellParameters parameters)
        {
            EngineStationTHRASH engineStation = new EngineStationTHRASH((ParametersTHRASH)parameters);
            //ParalellEngineStation
            List<ParalellEngine> engines = engineStation.Engines;
            object engineLock = engineStation.EngineLock;

            ParametersTHRASH thrashParameters = (ParametersTHRASH)parameters;

            

            //for each core
            for (int i = 0; i < parameters.CoresPerComputer; i++)
            {
                int engineNumber = i;
                
                //we need to copies the parameters
                //ParametersTHRASH ThrashParametersUnique = new ParametersTHRASH(ThrashParameters);
                ParametersTHRASH ThrashParametersUnique = (ParametersTHRASH)thrashParameters.Clone();

                

                //ThrashParametersUnique.UniqueFileName = RemoveEnding.RAW(thrashParameters.FileInforamation.InputFileName) + " (" + engineNumber + ").RAW";
                ThrashParametersUnique.UniqueFileName = ParalellHardDrive.Select(ThrashParametersUnique.MultithreadedHardDriveMode, ThrashParametersUnique, engineNumber);



                EngineTHRASH ThrashEngine = new EngineTHRASH(ThrashParametersUnique, engineLock, engineNumber);
                //ThrashEngine.UniqueFileName = ThrashParametersUnique.LocalDataFile;

                ParalellLog.LogAddEngine_AddToStation(ThrashEngine);

                SetupRun(ThrashParametersUnique, ThrashEngine);

                SetupScanSet(parameters.MultithreadedHardDriveMode, ThrashParametersUnique, engineStation, ThrashEngine);

                engines.Add(ThrashEngine);
                engineStation.ErrorPile.Add(new List<string>());
            }

            int bonusEngineNumber = parameters.CoresPerComputer + 1;
            //ParametersTHRASH ThrashParametersUniqueBonus = new ParametersTHRASH(thrashParameters);
            //ThrashParametersUniqueBonus.LocalDataFile = RemoveEnding.RAW(ThrashParameters.FileInforamation.InputFileName) + " (" + bonusEngineNumber + ").RAW";
            ParametersTHRASH thrashParametersUniqueBonus = (ParametersTHRASH)thrashParameters.Clone();

            thrashParametersUniqueBonus.UniqueFileName = ParalellHardDrive.Select(thrashParametersUniqueBonus.MultithreadedHardDriveMode, thrashParametersUniqueBonus, bonusEngineNumber);
            //ThrashParametersUniqueBonus.EngineScanSetCollection = engineStation.ScanSetCollection;

            EngineTHRASH ThrashEngineBonus = new EngineTHRASH(thrashParametersUniqueBonus, engineLock, bonusEngineNumber);

            SetupRun(thrashParametersUniqueBonus, ThrashEngineBonus);

            SetupScanSet(parameters.MultithreadedHardDriveMode, thrashParametersUniqueBonus, engineStation, ThrashEngineBonus);

            engineStation.ExtraEngines.Add(ThrashEngineBonus);
            engineStation.ErrorPile.Add(new List<string>());

            return engineStation;
        }

        private static void SetupScanSet(bool multithreadedHardDriveMode, ParametersTHRASH thrashParameters, EngineStationTHRASH engineStation, EngineTHRASH ThrashEngine)
        {
           

            int scanStart = 0;
            int scanStop = thrashParameters.MaxScanLimitation;

            //var scanSetCollection = ScanSetCollection.Create(ThrashEngine.Run, scanStart, scanStop, numScansSummed: 5, scanIncrement: 1, processMSMS: false);
            //ScanSetCollection scanSetCollection = ScanSetCollection.Create(ThrashEngine.Run, scanStart, scanStop, numScansSummed: 5, scanIncrement: 1, processMSMS: false);
            //ThrashEngine.Run.ScanSetCollection = scanSetCollection;

            ParalellLog.LogAddEngine_ScanSet(ThrashEngine);

            if (multithreadedHardDriveMode == true)
            {
                //var deconScanSet = ScanSetCollection.Create(ThrashEngine.Run, ThrashParametersUnique.ScansToSum, 1, ThrashParametersUnique.ProcessMSMS);
                //THis is a new scan set from the file
                //ThrashEngine.Run.ScanSetCollection = deconScanSet;

                try
                {
                    ThrashEngine.Run.ScanSetCollection = ScanSetCollection.Create(ThrashEngine.Run, ThrashEngine.Run.MinScan, ThrashEngine.Run.MaxScan, thrashParameters.ScansToSum, 1, thrashParameters.ProcessMsms);
                    thrashParameters.EngineScanSetCollection = ThrashEngine.Run.ScanSetCollection;//stores from unique decontools
                    Console.WriteLine("LoadingData...");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                thrashParameters.EngineScanSetCollection = engineStation.ScanSetCollection; //this is not a copy!!!!!
                ThrashEngine.Run.ScanSetCollection = engineStation.ScanSetCollection;//stores from pooled scanset
                Console.WriteLine("Using Scanset from Station...");
            }
        }

        private static void SetupRun(ParalellParameters parameters, EngineTHRASH ThrashEngine)
        {
            InputOutputFileName tempName = new InputOutputFileName();
            tempName.InputFileName = ThrashEngine.Parameters.UniqueFileName;
            ThrashEngine.Run = GoCreateRun.CreateRun(tempName);

            ParametersTHRASH thrashParamaters = (ParametersTHRASH)parameters;

            ThrashEngine.SQLiteTranformerFileName = thrashParamaters.FileInforamation.OutputSQLFileName + "_Multi_" + Convert.ToString(ThrashEngine.EngineNumber);
            //ThrashEngine.UniqueFileName = ThrashParameters.FileInforamation.InputFileName + "_" + Convert.ToString(i);
            //ThrashEngine.UniqueFileName = thrashParamaters.FileInforamation.InputFileName;

            ParalellLog.LogAddEngine_Run(ThrashEngine);
        }

        private static void SetupPrecursors(ParalellParameters parameters, EngineTHRASH ThrashEngine)
        {
            ParametersTHRASH thrashParamaters = (ParametersTHRASH)parameters;
            
            int sizeOfDatabase = ThrashEngine.Run.GetNumMSScans() - 1;//this is a crude way to get the size of the database.  -1 is need or the scan set creation will fail

            //this will limit to part of the file if the scans are enumerated
            if (sizeOfDatabase > thrashParamaters.MaxScanLimitation)
            {
                sizeOfDatabase = thrashParamaters.MaxScanLimitation;
            }


            ThrashEngine.PrecursorMetaData = new List<PrecursorInfo>();
            for (int i = 0; i < sizeOfDatabase; i++)
            {
                Console.WriteLine("Scan " + i.ToString());
                //if (i > 3611)
                //{
                //    i = i++;
                //    i = i--;
                //}

                PrecursorInfo precursor = ThrashEngine.Run.GetPrecursorInfo(i);
                ThrashEngine.PrecursorMetaData.Add(precursor);
                //MSLevel.Add(run.GetMSLevel(i));
            }
        }
    }

    
}





//Here is one test in that project:
//    [Test]
//        public void MSGenerator_SummingDemo1()
//        {
          
//            Run run = new RunFactory().CreateRun(thermoFile1);

//            ScanSetFactory scanSetFactory = new ScanSetFactory();

//            int scanNum = 6005;
//            int numScansSummed = 5;
//            ScanSet testScan = scanSetFactory.CreateScanSet(run, scanNum, numScansSummed);

//            run.CurrentScanSet = testScan;
//            Console.WriteLine("Scanset=\t" + testScan);

//            var msgen = MSGeneratorFactory.CreateMSGenerator(run.MSFileType);

//            msgen.Execute(run.ResultCollection);

//            run.XYData.Display();
          
//        }

//Here is a scanSet collection demo:

//[Test]
//        public void Create_a_collection_of_Scansets_Demo1()
//        {
//            Run run = new RunFactory().CreateRun(thermoFile1);

//            int scanStart = 6005;
//            int scanStop = 6050;

//            var scanSetCollection = ScanSetCollection.Create(run, scanStart, scanStop, numScansSummed: 5, scanIncrement: 1, processMSMS: false);
//            var msgen = MSGeneratorFactory.CreateMSGenerator(run.MSFileType);


//            foreach (var scanSet in scanSetCollection.ScanSetList)
//            {
//                run.CurrentScanSet = scanSet;
//                msgen.Execute(run.ResultCollection);
//            }



