using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using ParalellTargetsLibrary;
using Parallel.SQLite;
using Parallel.THRASH;
using GetPeaks_DLL.Parallel;
using GetPeaks_DLL.SQLiteEngine;
using GetPeaks_DLL.Go_Decon_Modules;
using GetPeaks_DLL.THRASH;

namespace GetPeaks_DLL.Parallel
{
    public class ParalellController
    {
        /// <summary>
        /// Number of Cores per computer.  This creates a certain number of Engines
        /// </summary>
        public int Cores { get; set; }
        
        /// <summary>
        /// How many computers this problem is divided over
        /// </summary>
        public int Computers { get; set;}

        /// <summary>
        /// number of scans for undivided dataset
        /// </summary>
        public int TotalScans { get; set; }

        /// <summary>
        /// Contains Engine information
        /// </summary>
        public ParalellEngineStation EngineStation { get; set; }

        /// <summary>
        /// Contains Engine information
        /// </summary>
        public ParalellEngineStation SqlEngineStation { get; set; }

        /// <summary>
        /// Contoller lock used to select an engine
        /// </summary>
        private readonly Object _tranformLock = new Object();

        /// <summary>
        /// logs from each engine to be compiled at the end of the analysis
        /// </summary>
        private readonly List<string> _logs = new List<string>();

        /// <summary>
        /// place for thrash parameters
        /// </summary>
        public ParalellParameters ParameterStorage1 { get; set; }

        /// <summary>
        /// place for SQL parameters
        /// </summary>
        public ParalellParameters ParameterStorage2 { get; set; }

        /// <summary>
        /// default constructor
        /// </summary>
        public ParalellController(ParalellParameters parameters1)
        {
            Computers = parameters1.ComputersToDivideOver;
            Cores = parameters1.CoresPerComputer;
            //TotalScans = scans;
            //EngineStation = new ParalellEngineStation();
            //SQLEngineStation = new ParalellEngineStation();
            _tranformLock = new object();
            ParameterStorage1 = parameters1;
        }

        /// <summary>
        /// Perhaps bring in engines and data
        /// </summary>
        public List<string> RunAlgorithms()
        {
            #region CreateData
            
            //int numberOfScan = TotalScans;
            
            //List<int> scans = new List<int>();
            //for (int i = 0; i < numberOfScan; i++)
            //{
            //    scans.Add(i);
            //}
            
            EngineStationTHRASH engineStation = (EngineStationTHRASH)EngineStation;
            List<int> scans = new List<int>();
            for (int i = 0; i < engineStation.Iterations-1; i++ )
            {
                scans.Add(i);
            }

            #endregion

            #region Break problem down into seperate computers
            List<List<int>> pileOfSelectedScans = new List<List<int>>();
            List<int> allScans = new List<int>();
            for (int rank = 0; rank < Computers; rank++)
            {
                List<int> selectedScans = ParalellSelectRange.Range(scans, Computers, rank);
                pileOfSelectedScans.Add(selectedScans);
                allScans.AddRange(selectedScans);
            }

            if (allScans.Count == scans.Count){Console.WriteLine("Correct Process Divider"); }

            #endregion

            #region Do work and return results object

            var resultPile = new ConcurrentBag<ParalellResults>();

            int maxNumberOfThreadsToUse = Cores*engineStation.Engines[0].Parameters.ForEachCoreMultiplier;
            //maxNumberOfThreadsToUse = 1;
            int threadsWritten = 0;
            
            Computers = 1;

            bool runParalell = engineStation.Engines[0].Parameters.MultithreadOperation;
 
            for (int rank = 0; rank < Computers; rank++)
            {
                //List<int> selectedScans = SelectRange.Range(scans, Computers, rank);
                List<int> selectedScans = pileOfSelectedScans[rank];
                
                //checks (3/3)
                int sumOfAlleScans = 0;
                int sumOfAllinitialScans = 0;
                int sumOfAllAssignedScans = 0;

                //check (1/3)
                foreach (int initialScan in selectedScans)
                {
                    sumOfAllinitialScans += initialScan;
                }

                Console.WriteLine("__________________Start For Each Loop____________________");

                ParalellAssignEngine engineRouter = new ParalellAssignEngine(EngineStation);

                switch (runParalell)
                {
                    case (true):
                        {
                            System.Threading.Tasks.Parallel.ForEach(selectedScans, new ParallelOptions { MaxDegreeOfParallelism = maxNumberOfThreadsToUse }, eScan =>
                            {
                                bool isSuccessfull = false;
                                isSuccessfull = MythicalForEach(engineRouter, eScan, isSuccessfull, ref sumOfAlleScans, ref sumOfAllAssignedScans);
                            });//paralell forEach
                        }
                        break;
                    case (false):
                        {
                            //foreach (int eScan in selectedScans)
                            //    {    
                                for (int i = 0; i < selectedScans.Count;i++ )
                                //for (int i = 0; i < selectedScans.Count; i++)
                                {
                                    int eScan = selectedScans[i];
                                
                                    bool isSuccessfull = false;
                                    isSuccessfull = MythicalForEach(engineRouter, eScan, isSuccessfull, ref sumOfAlleScans, ref sumOfAllAssignedScans);
                                }
                        }
                        break;
                }
                //we may need to switch to scan set collections or something similar
                
                Console.WriteLine("There are {0} initial scans, {1} eScan threads iterated, and {2} assigned scans =4950 for 100 scans", sumOfAllinitialScans, sumOfAlleScans, sumOfAllAssignedScans);
                Console.WriteLine("");

                if (sumOfAllAssignedScans == sumOfAllinitialScans && sumOfAlleScans == sumOfAllinitialScans)
                {
                    Console.WriteLine("**********We Made It*********");
                    //Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("------Problem with consistancy------");
                    Console.ReadKey();
                }
            }

            foreach (List<string> log in engineStation.ErrorPile)
            {
                foreach (string word in log)
                {
                    _logs.Add(word);
                }
            }

            #endregion

            //ParalellResults sampleresult = sortedFinalResults[0];

            int sumCheck = threadsWritten;
            //foreach (ParalellResults thread in threadsWritten)
                //foreach (ParalellResults thread in sortedFinalResults)
            //{
            //    sumCheck += thread.ThreadNumber;
            //}



            //Console.WriteLine("\n" + sumCheck + " should equal 4950 for 100 scans");
            Console.WriteLine("Finished");

            return _logs;
        }

        private bool MythicalForEach(ParalellAssignEngine engineRouter, int eScan, bool success, ref int sumOfAlleScans, ref int sumOfAllAssignedScans)
        {
            
            Console.WriteLine("Thread {0} has started", eScan);

            int threadName = System.Threading.Thread.CurrentThread.ManagedThreadId;

            #region inside for algorithms

            ParalellEngine transformerMulti; // = transformer2;
            
            lock (_tranformLock)
            {
                Console.WriteLine("Thread {0} needs a transformer", eScan);
                transformerMulti = engineRouter.Assign();
                transformerMulti.Scan = eScan;
                Console.WriteLine("  + Thread {0} has a transformer", eScan);
            }

            //check (2/3)
            sumOfAllAssignedScans += transformerMulti.Scan;

            //check (3/3)
            sumOfAlleScans += eScan;

            //send data to worker thread here
            ParalellThreadDataTHRASH data = new ParalellThreadDataTHRASH(transformerMulti, eScan);

            //resultPile.Add(ParalellDoWork.DoWorkWithControllerThrash(data)); 

            ParalellResults results = ParalellDoWork.DoWorkWithControllerThrash(data);

            //reset engine
            engineRouter.ResetEngine(transformerMulti, EngineStation);

            #endregion

            #region save data

            //4. Run Writer but keep database layer open

            ResultsTHRASH thrashResults = (ResultsTHRASH) results;
            int scanNumber = eScan;

            //Peak resultPeak = new Peak();
            //resultPeak.XValue = thrashResults.RawPeaksInScan;
            //resultPeak.Height = thrashResults.CentroidedPeaksInScan;
            //resultPeak.LocalSignalToNoise = thrashResults.MonoisotopicPeaksInScan;
            //EngineSQLite currentEngine = (EngineSQLite)SQLEngineStation.Engines[0];
            //bool didthiswork = currentEngine.WritePeakData((EngineSQLite)SQLEngineStation.Engines[0], resultPeak, scanNumber);


            if (thrashResults.ResultsFromRunConverted.Count > 0)
            {
                #region write data
                try
                {
                    EngineSQLite currentEngine = (EngineSQLite)SqlEngineStation.Engines[0];

                    ////EngineSQLite currentEngine = (EngineSQLite)SQLEngineStation.Engines[0];
                    //bool didthiswork = currentEngine.WriteIsosData(currentEngine, thrashResults);
                    bool didthiswork = currentEngine.WriteIsosData(thrashResults);
                    //    //bool didthiswork = currentEngine.WriteIsosData((EngineSQLite)SQLEngineStation.Engines[0], thrashResults.ResultsFromRunConverted, scanNumber);

                    if(!didthiswork)
                    {
                        currentEngine = (EngineSQLite)SqlEngineStation.Engines[1];

                        ////EngineSQLite currentEngine = (EngineSQLite)SQLEngineStation.Engines[0];
                        //didthiswork = currentEngine.WriteIsosData(currentEngine, thrashResults);
                        didthiswork = currentEngine.WriteIsosData(thrashResults);
                        //    //bool didthiswork = currentEngine.WriteIsosData((EngineSQLite)SQLEngineStation.Engines[0], thrashResults.ResultsFromRunConverted, scanNumber);
                    }

                    if (didthiswork)
                    {

                    }
                    else
                    {
                        Console.WriteLine("Arg");
                        Console.ReadKey();
                    }
                }
                catch (Exception)
                {
                    EngineSQLite currentEngine = (EngineSQLite)SqlEngineStation.ExtraEngines[0];

                    ////EngineSQLite currentEngine = (EngineSQLite)SQLEngineStation.Engines[0];
                    //bool didthiswork = currentEngine.WriteIsosData(currentEngine, thrashResults);
                    bool didthiswork = currentEngine.WriteIsosData(thrashResults);
                    //    //bool didthiswork = currentEngine.WriteIsosData((EngineSQLite)SQLEngineStation.Engines[0], thrashResults.ResultsFromRunConverted, scanNumber);

                    if (didthiswork)
                    {

                    }
                    else
                    {
                        Console.WriteLine("Arg");
                        Console.ReadKey();
                    }
                }



                #endregion

            }

            //Console.WriteLine("Press a Key to End\n");

            #endregion

            success = true;
            return success;
        }
    }

}
