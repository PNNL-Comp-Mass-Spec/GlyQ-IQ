using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using DeconTools.Backend.ProcessingTasks;
using System.Diagnostics;
using GetPeaks_DLL.CompareContrast;
using DeconTools.Backend.Utilities;
using System.Threading;
using DeconTools.Backend.Runs;
using DeconTools.Backend;
using DeconTools.Backend.DTO;
using PNNLOmics.Data;
using GetPeaks_DLL.PNNLOmics_Modules;
using GetPeaks_DLL.Common_Switches;
using PNNLOmics.Algorithms.PeakDetection;
using System.Reflection;
using PNNLOmics.Data.Features;
using GetPeaks_DLL.SQLite;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.Go_Decon_Modules;

namespace GetPeaks_DLL
{
    public class ElutingPeakFinderController:IDisposable
    {
        #region Properties

        //public MSGeneratorFactory msgenFactory { get; set; }
        //public Task msGenerator {get;set;}
        public SimpleWorkflowParameters Parameters;
        public DeconToolsPeakDetector msPeakDetector;
        private ScanSumSelectSwitch m_summingMethod;

        #endregion

        #region Constructors
            
            public ElutingPeakFinderController()
            {
                //this.msgenFactory = new MSGeneratorFactory();
            }

            public ElutingPeakFinderController(Run run, SimpleWorkflowParameters parameters)
                : this()
            { 
                this.Parameters = parameters;
                
                InitializeTasks(run, this.Parameters);
            }

            private void InitializeTasks(Run run, SimpleWorkflowParameters parameters)
            {
                //msGenerator = msgenFactory.CreateMSGenerator(run.MSFileType);
                //var msGenerator = MSGeneratorFactory.CreateMSGenerator(run.MSFileType);
                //this is the first call to Decon Tools Engine V2.  Be sure to install .Net1.1
                //you also need IMSCOMP.dll, System.Data.SQLite.dll, schema.yafms (TODO verify we need all these)
                msPeakDetector = new DeconToolsPeakDetector(parameters.Part1Parameters.MSPeakDetectorPeakBR, parameters.Part1Parameters.ElutingPeakNoiseThreshold, parameters.Part2Parameters.PeakFitType, false);
                msPeakDetector.PeaksAreStored = true;
                //msPeakDetector.PeakFitType = parameters.PeakFitType;
                m_summingMethod = parameters.SummingMethod;//load up summing method choise as a class variable because it is used so much
            }

            #region idisposable
            public virtual void Close()
            {
                //this.msgenFactory = null;
                //this.msGenerator = null;
                this.Parameters = null;
                this.msPeakDetector = null;
            }

            public virtual void Dispose()
            {
                this.Close();
            }

            #endregion
        #endregion

            #region Public Methods
            /// <summary>
            /// Create Eluting Peaks
            /// </summary>
            /// <param name="run"></param>
            public List<ElutingPeak> SimpleWorkflowExecutePart1(Run run)//create eluting peaks and store them in run.ResultsCollection.ElutingPeaks
            {
                System.Timers.Timer aTimer = new System.Timers.Timer();
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                run.MinLCScan = this.Parameters.Part1Parameters.StartScan;
                run.MaxLCScan = this.Parameters.Part1Parameters.StopScan;
                int scansToSum = this.Parameters.Part1Parameters.ScansToBeSummed;//1,3,5 etc centered on main scan

                List<ElutingPeak> elutingPeakCollectionStorage = new List<ElutingPeak>();

                double alignmentTolerance = Parameters.Part1Parameters.AllignmentToleranceInPPM;
                try
                {
                    bool GetMSMSDataAlso = false;


                    run.ScanSetCollection.Create(run, run.MinLCScan, run.MaxLCScan, scansToSum, 1, GetMSMSDataAlso);

                    //run.ScanSetCollection = ScanSetCollection.Create(run, run.MinScan, run.MaxScan, scansToSum, 1, GetMSMSDataAlso);
                    Console.WriteLine("LoadingData...");
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                int scanCounter = 0;
                bool isFirstScanSet = true;
                int scansetInitialOffset = 0; //offset of first scan with data
                int scanCounterPrevious = 0;//previous scanset scan number
                int scansetRangeOffset = Parameters.Part1Parameters.StartScan;//range offset.  linked to the start scan we are looping through
                foreach (ScanSet scanSet in run.ScanSetCollection.ScanSetList)
                {
                    //int currentMSLevel = run.GetMSLevel(scanCounter);
                    //scanCounter++;
                    
                    scanCounter = scanSet.PrimaryScanNumber;

                    //if (scanCounter > 450)
                    //{
                        run.CurrentScanSet = scanSet;
                    //}

                    Console.WriteLine("Working on Scan " + scanCounter.ToString() + " with " + run.ResultCollection.ElutingPeakCollection.Count + " active " +
                        " and " + elutingPeakCollectionStorage.Count + " inactive ePeaks. Total: " + (run.ResultCollection.ElutingPeakCollection.Count + elutingPeakCollectionStorage.Count).ToString());
                    run.CurrentScanSet = scanSet;

                    List<int> peakIndexList = new List<int>();

                    //if (scanSet.PrimaryScanNumber > 12)//35
                    //{
                    //    run.CurrentScanSet = scanSet;
                    //}

                    #region create Eluting Peaks
                    try
                    {
                        ///input: YAFMSRun
                        #region msGenerator

                        GoSpectraGenerator.GenerateMS(run);
                        //var msGenerator = MSGeneratorFactory.CreateMSGenerator(run.MSFileType);
                        //msGenerator.Execute(run.ResultCollection);
                        //msGenerator.Cleanup();
                        //msGenerator = null;

                        #endregion
                        ///output: run.ResultsCollection.Run.XYData and run.XYData
                        if (run.ResultCollection.Run.XYData.Xvalues.Length > 1)
                        {
                            bool OmicsPeakDecection = true;
                            List<ProcessedPeak> thresholdedData;

                            if (OmicsPeakDecection)
                            {
                                ///input: run.ResultCollection.Run.XYData
                                #region OmicsPeakDetection
                                //1.  extract XYData
                                List<double> tempXvalues = new List<double>();
                                List<double> tempYValues = new List<double>();
                                tempXvalues = run.ResultCollection.Run.XYData.Xvalues.ToList();
                                tempYValues = run.ResultCollection.Run.XYData.Yvalues.ToList();

                                List<PNNLOmics.Data.XYData> newData = new List<PNNLOmics.Data.XYData>();
                                for (int i = 0; i < run.ResultCollection.Run.XYData.Xvalues.Length; i++)
                                {
                                    PNNLOmics.Data.XYData newPoint = new PNNLOmics.Data.XYData(tempXvalues[i], tempYValues[i]);
                                    newData.Add(newPoint);
                                }

                                //2.  centroid peaks
                                PeakCentroider newPeakCentroider = new PeakCentroider();
    //                            newPeakCentroider.Parameters.ScanNumber = scanCounter;
                                newPeakCentroider.Parameters.FWHMPeakFitType = SwitchPeakFitType.setPeakFitType(Parameters.Part1Parameters.PeakFitType);

                                List<ProcessedPeak> discoveredPeakList = new List<ProcessedPeak>();
                                discoveredPeakList = newPeakCentroider.DiscoverPeaks(newData);
           
                                //3.  threshold data
                                PeakThresholderParameters parametersThreshold = new PeakThresholderParameters();
                                parametersThreshold.isDataThresholded = Parameters.Part1Parameters.isDataAlreadyThresholded;
                                parametersThreshold.SignalToShoulderCuttoff = (float)Parameters.Part1Parameters.ElutingPeakNoiseThreshold;
      //                          parametersThreshold.ScanNumber = scanCounter;
                                parametersThreshold.DataNoiseType = Parameters.Part1Parameters.NoiseType;

                                SwitchThreshold newSwitchThreshold = new SwitchThreshold();
                                newSwitchThreshold.Parameters = parametersThreshold;
                                newSwitchThreshold.ParametersOrbitrap = Parameters.Part1Parameters.ParametersOrbitrap;
                                thresholdedData = newSwitchThreshold.ThresholdNow(ref discoveredPeakList);

                                #region vestigial print stuff off
                                List<double> newList = new List<double>();
                                foreach (ProcessedPeak peak in thresholdedData)
                                {
                                    if (peak.XValue > 1000f && peak.XValue < 1100f)
                                    {
                                        newList.Add(peak.XValue);
                                    }
                                }
                                #endregion

                                #region vestigial Add scan Number off
                                //for (int i = 0; i < thresholdedData.Count; i++)
                                //{
                                //    thresholdedData[i].ScanNumber = scanSet.PrimaryScanNumber;
                                //}
                                #endregion

                                //4. save data to Decon Tools
                                run.ResultCollection.PeakCounter = run.ResultCollection.MSPeakResultList.Count + thresholdedData.Count;

                                #region vestigial print stuff off
                                //List<double> printMasses = new List<double>();
                                //List<double> printInt = new List<double>();
                               // List<double> prinFWHM = new List<double>();
                                //string masses = "";
                                //string intensity = "";
                                //string FWHM = "";
                                #endregion

                                //run.ResultCollection.Run.PeakList = new List<IPeak>();//this is a scan by scan peak list
                                run.ResultCollection.Run.PeakList = new List<DeconTools.Backend.Core.Peak>();//this is a scan by scan peak list
                                for (int i = 0; i < thresholdedData.Count; i++)
                                {
                                    DeconTools.Backend.Core.MSPeak newMSPeak = new DeconTools.Backend.Core.MSPeak();
                                    newMSPeak.XValue = thresholdedData[i].XValue;
                                    //TODO check this intensity = hegiht
                                    newMSPeak.Height = Convert.ToSingle(thresholdedData[i].Height);
                                    newMSPeak.Width = thresholdedData[i].Width;
                                    newMSPeak.SignalToNoise = Convert.ToSingle(thresholdedData[i].SignalToNoiseGlobal);
                                    newMSPeak.DataIndex = thresholdedData[i].ScanNumber; //save scan here
                                    //newMSPeak.DataIndex = scanCounter; //save scan here

                                    #region vestigial print stuff off
                                    //printMasses.Add(omicsList[i].X);
                                    //printInt.Add(omicsList[i].Y);
                                    //prinFWHM.Add(newMSPeak.Width);

                                    //masses += printMasses[i].ToString() + ",";
                                    //intensity += printInt[i].ToString() + ",";
                                    //FWHM += prinFWHM[i].ToString() + ",";
                                    #endregion

                                    run.ResultCollection.Run.PeakList.Add(newMSPeak);

                                    //update mspeakresults
                                    MSPeakResult newMSPeakResult = new MSPeakResult();//this is a system wide peak list
                                    newMSPeakResult.ChromID = -1;
                                    //newMSPeakResult.Frame_num = -1;
                                    newMSPeakResult.MSPeak = newMSPeak;
                                    newMSPeakResult.PeakID = i;
                                    newMSPeakResult.Scan_num = thresholdedData[i].ScanNumber;
                                    run.ResultCollection.MSPeakResultList.Add(newMSPeakResult);
                                }
                                #endregion
                                ///output: thresholdedData etc
                                ///output: run.ResultCollection.PeakCounter
                                ///output: run.ResultCollection.MSPeakResultList
                                ///output: run.ResultCollection.Run.PeakList 
                            }
                            else
                            {
                                ///Input: run.ResultCollection.run.XYData
                                #region MSPeakDetector

                                run.ResultCollection.Run.PeakList = null;//clear it out because this is a scan by scan peak list
                                msPeakDetector.Execute(run.ResultCollection);

                                #region vestigial print stuff
                                //List<double> printMasses = new List<double>();
                                //List<double> printInt = new List<double>();
                                //List<double> prinFWHM = new List<double>();
                                //string masses2 = "";
                                //string intensity2 = "";
                                //string FWHM2 = "";


                                //for (int i = 0; i < run.ResultCollection.Run.PeakList.Count; i++)
                                //{
                                //    printMasses.Add(run.ResultCollection.Run.PeakList[i].XValue);

                                //    printInt.Add(run.ResultCollection.Run.PeakList[i].Height);

                                //    prinFWHM.Add(run.ResultCollection.Run.PeakList[i].Width);

                                //    masses2 += printMasses[i].ToString() + ",";
                                //    intensity2 += printInt[i].ToString() + ",";
                                //    FWHM2 += prinFWHM[i].ToString() + ",";
                                //}
                                #endregion

                                #region vestigial convert to Omics Processed Peaks
                                //thresholdedData = new List<ProcessedPeak>();
                                //for (int i = 0; i < run.ResultCollection.Run.PeakList.Count; i++)
                                //{
                                //    ProcessedPeak newPeakOmics = new ProcessedPeak();
                                //    newPeakOmics.XValue = run.ResultCollection.Run.PeakList[i].XValue;
                                //    newPeakOmics.Height = run.ResultCollection.Run.PeakList[i].Height;
                                //    newPeakOmics.Width = run.ResultCollection.Run.PeakList[i].Width;
                                //    newPeakOmics.ScanNumber = scanSet.PrimaryScanNumber;
                                //    thresholdedData.Add(newPeakOmics);
                                //}
                                #endregion

                                #endregion
                                ///output: run.ResultCollection.PeakCounter
                                ///output: run.ResultCollection.MSPeakResultList
                                ///output: run.ResultCollection.Run.PeakList
                                ///output: run.ResultCollection.Run.DeconToolsPeakList
                            }

                            int MSpeakResultsListCount = run.ResultCollection.MSPeakResultList.Count;
                            int newPeaks = run.ResultCollection.Run.PeakList.Count;//new peaks found in PeakDetector

                            #region collect Peaks from msPeakDetector.  Keep track of indexes in MSpeakResultsList that coorespond to the local peakList
                            //this took 3.4 seconds and contains the int index reference to the full MSPeakResult
                            //List<double> peakmassesList = new List<double>();//used for debugging
                            for (int j = MSpeakResultsListCount - newPeaks; j < MSpeakResultsListCount; j++)
                            {
                                peakIndexList.Add(j);
                            }

                            #region alternate code
                            //collectPeaks  this iteration took 3.4 seconds
                            //for (int j = counter; j < counter + newPeaks; j++)
                            //{
                            //    newList.Add(run.ResultCollection.MSPeakResultList[j].MSPeak.XValue);
                            //}
                            //counter = run.ResultCollection.MSPeakResultList.Count;
                            //MasterList.Add(newList);

                            //List<int> extractMSfromOneScan = (from n in run.ResultCollection.MSPeakResultList where n.Scan_num==counter select n.MSPeak.DataIndex).ToList();
                            //counter++;
                            //run.ResultCollection.PeakResultIndexList.Add(extractMSfromOneScan);

                            //this linq section took 8.1 sections
                            //List<double> extractMSfromOneScan = (from n in run.ResultCollection.MSPeakResultList where n.Scan_num==counter select n.MSPeak.XValue).ToList();
                            //counter++;
                            //MasterList.Add(extractMSfromOneScan);
                            #endregion
                            #endregion


                            ///Input: run.ResultCollection.Run.PeakList, run.ResultCollection.MSPeakResultList
                            ///Input: scanCounter
                            ///Input: scanCounterPrevious
                            ///Input: peakIndexList.  Links the PeakList with the global MSPeakResultList
                            CalculateElutingPeakRanges RangeCalculator = new CalculateElutingPeakRanges();
                            RangeCalculator.SetElutingPeakProperties(run, scanCounter, scanCounterPrevious, peakIndexList, scansetRangeOffset, Parameters.Part1Parameters.MaxHeightForNewPeak, Parameters, alignmentTolerance);
                            RangeCalculator.CalculateElutingPeaksRanges(ref isFirstScanSet, ref scansetInitialOffset, ref elutingPeakCollectionStorage);
                            ///Ouput:  run.ResultCollection.ElutingPeakCollection

                            //RangeCalculator.CalculateElutingPeaksRanges(run, scanCounter, scanCounterPrevious, peakIndexList, ref isFirstScanSet, ref scansetInitialOffset, scansetRangeOffset, TOLERANCE);
                        }
                        scanCounterPrevious = scanCounter;
                    }

                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    #endregion
                }

                #region close all remaining open peaks by setting ID to 3
                int count3 = 0;
                int countNot3 = 0;
                
                foreach (ElutingPeak ePeak in run.ResultCollection.ElutingPeakCollection)
                {
                    if (ePeak.ID != 3)
                    {
                        countNot3++;
                        ePeak.ID = 3;
                    }
                    else
                    {
                        count3++;
                        
                    }
                    elutingPeakCollectionStorage.Add(ePeak);
                }

                Console.WriteLine(countNot3 + " " + count3);
                foreach (ElutingPeak ePeak in elutingPeakCollectionStorage)
                {
                    ePeak.ID = 3;
                }

                //run.ResultCollection.ElutingPeakCollection = null;
                run.ResultCollection.ElutingPeakCollection = elutingPeakCollectionStorage.OrderBy(p => p.Mass).ToList();

                #endregion
            
                stopWatch.Stop();

                TimeSpan ts = stopWatch.Elapsed;
                Console.WriteLine("This took " + ts + "seconds");

                return elutingPeakCollectionStorage;
            }
       
            /// <summary>
            /// Analyze Eluting Peaks to find Monoisotopic Peaks  This is the origional method
            /// </summary>
            /// <param name="run">Decon tools run containing run.ResultsColelction.ElutingPeaks</param>
            /// <param name="fileName">data file name</param>
            public void SimpleWorkflowExecutePart2(Run run, string fileName, string SQLFileName, bool msScanSetOnly)//Analyze Eluting Peaks to find Monoisotopic peaks vis decon tools
            {
                DatabaseLayer newDataBaseLayer = new DatabaseLayer();
                bool didThisWork = false;
                didThisWork = newDataBaseLayer.CreateFeatrueLiteTableInformation(SQLFileName);
                if (didThisWork == true)
                {
                    Console.WriteLine("TableInfoCreated");
                }
                
                #region setup
                //send to a thread
                int localProcessors = 1;
                string processorsStr = System.Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS");
                if (processorsStr != null)
                {
                    localProcessors = int.Parse(processorsStr);
                }
                //localProcessors = 2;
                //Console.WriteLine("There are " + localProcessors + " Processors");

                int peakCount = 0;
                int totalPeakCount = run.ResultCollection.ElutingPeakCollection.Count;

                bool multithread = Parameters.Part2Parameters.Multithread;
                
                int threadCount = 0;
                List<Thread> lotsOfThreads = new List<Thread>();

                List<Run> listofRuns = new List<Run>();
                int runcounter = 0;

                #region check to ensure all ID are accounted for and =3.  3 means the eluting peak has ended
                int count3 = 0;
                int countNot3 = 0;
                foreach (ElutingPeak ePeak in run.ResultCollection.ElutingPeakCollection)
                {
                    if (ePeak.ID != 3)
                    {
                        countNot3++;
                    }
                    count3++;
                }

                if (countNot3 > 0)
                {
                    Console.WriteLine("arg, not 3, PeakFinderController");
                }
                #endregion

                #endregion
                //sort Eluting peaks so we hit the most intense first
                List<ElutingPeak> tempList = run.ResultCollection.ElutingPeakCollection;
                run.ResultCollection.ElutingPeakCollection = run.ResultCollection.ElutingPeakCollection.OrderByDescending(p => p.Intensity).ToList();

                

                int limitThreadCount = run.ResultCollection.ElutingPeakCollection.Count;
                //limitThreadCount = 10;
                float limitIntensity = (float)(run.ResultCollection.ElutingPeakCollection[0].Intensity / Parameters.Part2Parameters.DynamicRangeToOne);



                int numberOfElements = run.ResultCollection.ElutingPeakCollection.Count;
                int numberOfChunks = Parameters.Part2Parameters.MemoryDivider.NumberOfBlocks;
                int rank = Parameters.Part2Parameters.MemoryDivider.BlockNumber;
                int startIndex = 0;
                int stopIndex = 0;


                ProcessDivision newProcesDivider = new ProcessDivision();
                newProcesDivider.CalculateSplits(numberOfElements, numberOfChunks, rank, ref startIndex, ref stopIndex);

                //foreach (ElutingPeak ePeak in run.ResultCollection.ElutingPeakCollection)
                //double cuttoff = run.ResultCollection.ElutingPeakCollection.Count/Parameters.Part2Parameters.MemoryDivider.NumberOfBlocks;
                //int split = (int)Math.Round(cuttoff, 0);

                Console.WriteLine("StartLoop at : " + startIndex + " and stop at : " + stopIndex);
                for (int e = startIndex; e <= stopIndex; e++)//+1 so we includ the stop
                //for (int e = 0; e < run.ResultCollection.ElutingPeakCollection.Count; e++)
                //for (int e = split; e < run.ResultCollection.ElutingPeakCollection.Count; e++)
                //for (int e = 0; e < split; e++)
                {
                    #region full loop
                    ElutingPeak ePeak = run.ResultCollection.ElutingPeakCollection[e];
                    //printMemory("start");

                    if (ePeak.Intensity > limitIntensity)//fiter:  remove low abundnt ions
                    {
                        if (ePeak.ScanEnd - ePeak.ScanStart < Parameters.Part2Parameters.MaxScanSpread)//filter:  ensure the peaks don't tail too long
                        {
                            if (ePeak.ID == 3)//filter:  I think all should be in this state so this filter is not needed
                            {
                                //Console.WriteLine("eluting Peak " + peakCount.ToString() + " out of " + totalPeakCount.ToString());
                                Console.WriteLine("eluting Peak " + (e).ToString() + " out of " + totalPeakCount.ToString());
                                
                                peakCount++;
                                //create scansets

                                if (multithread)
                                {
                                    #region multithread create threads
                                    if (threadCount < limitThreadCount)
                                    {
                                        //Monitor.Enter(myLockObject);
                                        RunFactory rf = new RunFactory();
                                        Run runGo = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, fileName);
                                        ////Run run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.Finnigan, inputDataFilename);

                                        //ElutingPeak tempElutingPeak = new ElutingPeak();
                                        //tempElutingPeak = ePeak;


                                        Stopwatch stopWatchMT = new Stopwatch();
                                        stopWatchMT.Start();

                                        #region -GoDeconToolsControllerA- get the data from the disk an sum it
                                        //sum spectra via msGenerator.  This hits the disk as it loads the file and returns a summed unTrimmed XYData
                                        GoDeconToolsControllerA newDeconToolsPart1 = new GoDeconToolsControllerA(run, Parameters);

                                        try//this is needed incase the scan information is not cashed.  It should be in this workflow and thus fast
                                        {
                                            ElutingPeakScanSet getScanSet = new ElutingPeakScanSet();
                                            //run.CurrentScanSet = getScanSet.CreateScanSetFromElutingPeakMax(run, ePeak.ScanMaxIntensity, ePeak.ScanStart, ePeak.ScanEnd);
                                            
                                            getScanSet.CreateScanSetFromElutingPeakMax(run, ePeak.ScanMaxIntensity, ePeak.ScanStart, ePeak.ScanEnd, m_summingMethod, Parameters.Part1Parameters.MSLevelOnly);
                                            run.CurrentScanSet = getScanSet.scanSet;
                                            getScanSet.Dispose();
                                        }
                                        catch (Exception ex)
                                        {
                                            throw ex;
                                        }
                                        Console.WriteLine("start get scan");

                                        //input: run.CurrentScanSet
                                        //input: Parameters
                                        newDeconToolsPart1.GoLoadDataAndSumIt(run, m_summingMethod, ePeak.ScanStart, ePeak.ScanEnd);
                                        //newDeconToolsPart1.GoLoadDataAndSumIt(run, Parameters);
                                        //output: run.ResultCollection.Run.XYData
                                        Console.WriteLine("stop get scan");

                                        #endregion

                                        stopWatchMT.Stop();
                                        Console.WriteLine("This took " + stopWatchMT.Elapsed + " seconds to find and assign features in eluting peaks");


                                        //Stopwatch stopWatchMT = new Stopwatch();
                                        //stopWatchMT.Start();

                                        //#region get the data from the disk an sum it
                                        ////run part one in serial since it is the one dealing with the disk
                                        //GoDeconToolsControllerA newDeconToolsPart1 = new GoDeconToolsControllerA(run, Parameters);
                                        //try//this is needed because we hit the disk.  However, the scanset should be stored somwwhere
                                        //{
                                        //    Console.WriteLine("get data and sum it ");
                                        //    ElutingPeakScanSet getScanSet = new ElutingPeakScanSet();
                                        //    run.CurrentScanSet = getScanSet.CreateScanSetFromElutingPeakSum(run, ePeak.ScanMaxIntensity, ePeak.ScanStart, ePeak.ScanEnd);
                                        //}
                                        //catch (Exception ex)
                                        //{
                                        //    throw ex;
                                        //}

                                        ////input: run.CurrentScanSet
                                        ////input: Parameters
                                        //newDeconToolsPart1.GoLoadDataAndSumIt(run, Parameters);
                                        ////output: run.ResultCollection.Run.XYData

                                        //#endregion

                                        //stopWatchMT.Stop();
                                        //Console.WriteLine("This took " + stopWatchMT.Elapsed + " seconds to find and assign features in eluting peaks");

                                        //TODO:  get and store all summed XYdata and store it in a List<List<XYData>>  Each spectra can then be farmed out into threads


                                        //TODO:  all this Go accelerate needs to be taken out of htis loop since this loop is used for pulling the XYdata

                                        //I am not sure if we need to copy run
                                        Console.WriteLine("add thread");

                                        //Monitor.Enter(myLockObject);
                                        GoAccelerate accelerater2 = new GoAccelerate();

                                        //new run
                                        runGo.XYData = new DeconTools.Backend.XYData();
                                        runGo.XYData = run.XYData;
                                        runGo.ResultCollection = new ResultCollection(run);
                                        runGo.ResultCollection = run.ResultCollection;
                                        runGo.ScanSetCollection = new ScanSetCollection();
                                        runGo.ScanSetCollection = run.ScanSetCollection;
                                        //runGo.PeakList = new List<IPeak>();
                                        runGo.PeakList = new List<DeconTools.Backend.Core.Peak>();
                                        runGo.MinLCScan = runcounter;
                                        listofRuns.Add(runGo);
                                        

                                        ElutingPeak localePeak = new ElutingPeak();
                                        localePeak = ePeak;

                                        //Console.WriteLine(ePeak.Mass.ToString());
                                        //accelerater2.SetValues(listofRuns[runcounter], localePeak, 1, localProcessors, TOLERANCE, Parameters, fileName, ePeak.ScanMaxIntensity);
                                        accelerater2.SetValues(listofRuns[runcounter], localePeak, 1, localProcessors, Parameters, fileName, ePeak.ScanMaxIntensity);
                                        
                                        runcounter++;

                                        //the problem here is too many threads calling the YAFMS reader at once.  we need to iterate through the main thread and load the data on one thread then statt the others
                                        //ThreadStart threadDelegate = new ThreadStart(accelerater2.GO2DeconTools);
                                        ThreadStart threadDelegate = new ThreadStart(accelerater2.GO2DeconTools);//run part 2 in threads
                                        lotsOfThreads.Add(new Thread(threadDelegate));

                                        //Monitor.Exit(myLockObject);
                                        //we need a semaphore to restrict the number of thresds that get started so we don't hit the disk all at once
                                        lotsOfThreads[threadCount].Name = "thread" + threadCount.ToString();
                                        //lotsOfThreads[threadCount].Start();
                                        //lotsOfThreads[threadCount].Join();
                                        threadCount++;
                                        if (threadCount == 160)
                                        {
                                            int t = 44;
                                            t = 66;
                                            int y = t;
                                        }
                                        Console.WriteLine("thread added");
                                        //Monitor.Exit(myLockObject);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region -GoDeconToolsControllerA- get the data from the disk an sum it
                                    //Stopwatch stopWatch = new Stopwatch();
                                    //stopWatch.Start();

                                    //sum spectra via msGenerator.  This hits the disk as it loads the file and returns a summed unTrimmed XYData
                                    GoDeconToolsControllerA newDeconToolsPart1 = new GoDeconToolsControllerA(run, Parameters);

                                    try//this is needed incase the scan information is not cashed.  It should be in this workflow and thus fast
                                    {
                                        ElutingPeakScanSet getScanSet = new ElutingPeakScanSet();
                                        //run.CurrentScanSet = getScanSet.CreateScanSetFromElutingPeakMax(run, ePeak.ScanMaxIntensity, ePeak.ScanStart, ePeak.ScanEnd);
                                        
                                        getScanSet.CreateScanSetFromElutingPeakMax(run, ePeak.ScanMaxIntensity, ePeak.ScanStart, ePeak.ScanEnd, m_summingMethod, Parameters.Part1Parameters.MSLevelOnly);
                                        run.CurrentScanSet = getScanSet.scanSet;
                                        getScanSet.Dispose();
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                    //Console.WriteLine("start get scan");

                                    //input: run.CurrentScanSet
                                    //input: Parameters
                                    //newDeconToolsPart1.GoLoadDataAndSumIt(run, Parameters);
                                    newDeconToolsPart1.GoLoadDataAndSumIt(run, m_summingMethod, ePeak.ScanStart, ePeak.ScanEnd);
                                    //output: run.ResultCollection.Run.XYData
                                    //Console.WriteLine("stop get scan");

                                    //stopWatch.Stop();
                                    //Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to find and assign features in eluting peaks");
                                    #endregion

                                    #region -GoDeconToolsControllerB- run part 2  to deisotope the the spine now that we have the correct XYData
                                    //GC.Collect();

                                    //printMemory("before");

                                    using (GoAccelerate accelerater = new GoAccelerate())
                                    {
                                        //GoAccelerate accelerater = new GoAccelerate();
                                        accelerater.SetValues(run, ePeak, 1, localProcessors, Parameters, fileName, ePeak.ScanMaxIntensity);

                                        accelerater.GO2DeconTools();//run part 2
                                    }
                                   // GC.Collect();
                                   // printMemory("after");


                                    CrossCorrelateResults organzeResults = new CrossCorrelateResults();

                                    //organzeResults.AssignCrossCorrelatedInformation(ePeak, this.Parameters);
                  //                  organzeResults.AssignCrossCorrelatedInformation(resultPeak, resultPeakIsos, Parameters);



                                    if (ePeak.ID == 1)//write to disk
                                    {
                                        bool isSuccessfull = false;

                                        using (DatabaseAdaptor newDataBaseAdaptor = new DatabaseAdaptor(SQLFileName))
                                        {

                                            isSuccessfull = newDataBaseAdaptor.WriteElutingPeakToSQLite_Old(ePeak);

                                            if (isSuccessfull)
                                            {
                                                Console.WriteLine("Data Stored");
                                            }
                                            else
                                            {
                                                Console.WriteLine("Data Not Stored");
                                            }
                                        }
                                    }
                                    #endregion
                                }

                                #region old code

                                //////deal with the isotope info
                                //if (multithread)
                                //{


                                //    double high=0;
                                //    double low=0;
                                //    double peakMZ=0;
                                //    float peakIntensity =0;
                                //    foreach (IsosResult isoresult in ePeak.IsosResultList)
                                //    {
                                //        peakMZ = isoresult.IsotopicProfile.MonoPeakMZ;
                                //        //peakIntensity = (float)isoresult.IsotopicProfile.Peaklist[0].Height;
                                //        peakIntensity = (float)isoresult.IsotopicProfile.IntensityAggregate;
                                //        high = peakMZ + TOLERANCE;
                                //        low = peakMZ - TOLERANCE;

                                //        //there are 2 ways to get assigned.  if true, the principle mass is assgined
                                //        //else true, monisiotopic mass may be in the rest of the eluting peak list and can be assigned here.  I expect this is faster but it may not be.  could just let it go through the loop
                                //        if (ePeak.Mass <= high && ePeak.Mass >= low)
                                //        {
                                //            ePeak.RetentionTime = (float)(peakMZ - ePeak.Mass);
                                //            ePeak.Mass = peakMZ;
                                //            ePeak.Intensity = peakIntensity;
                                //            ePeak.ID = 0;
                                //            ePeak.IsosResultList = new List<StandardIsosResult>();//zero out list since we found our match
                                //            ePeak.PeakList = new List<MSPeakResult>();
                                //        }
                                //        else
                                //        {
                                //            //there is not else because there is only one monoisitopic peak optomized for

                                //    #region old code
                                //    //        //foreach (ElutingPeak ePeak2 in run.ResultCollection.ElutingPeakCollection)
                                //    //        //{
                                //    //        //    high = ePeak2.Mass + TOLERANCE;
                                //    //        //    low = ePeak2.Mass - TOLERANCE;
                                //    //        //    if (peakMZ <= high && peakMZ >= low)
                                //    //        //    {
                                //    //        //        ePeak2.Mass = peakMZ;
                                //    //        //        ePeak2.Intensity = peakIntensity;
                                //    //        //        ePeak2.ID = 0;
                                //    //        //        break;
                                //    //        //    }

                                //    //            //if (peakMZ < low)//this will work if the eluting peak list is sorted by mass.  it is not
                                //    //            //{
                                //    //            //    break;//there is no reason to keep going down the list
                                //    //            //}
                                //    //        //}
                                //    #endregion
                                //        }

                                //        ePeak.IsosResultList = new List<StandardIsosResult>();//zero out list since we found our match
                                //        ePeak.PeakList = new List<MSPeakResult>();
                                //    }
                                //}
                                #endregion
                            }
                            else 
                            {
                                Console.WriteLine(ePeak.ID);
                                Console.ReadKey();
                                break;//this is to test the code
                            }
                        }
                    }

                    
                    //ElutingPeak ePeak2 = ePeak;
                    //SimplifyElutingPeak(ref ePeak2);
                    //ClearElutingPeak(ref ePeak2);

                    //printMemory("after clear peak list");
                    int k = 5;
                    k++;

                    #endregion
                }//end for eluting peaks


                if (multithread)
                {
                    #region multithread run threads
                    //Monitor.Enter(myLockObject);
                    for (int i = 0; i < lotsOfThreads.Count; i++)
                    {
                        Console.WriteLine("start thread " + i);
                        lotsOfThreads[i].Start();
                    }

                    //Monitor.Exit(myLockObject);
                    for (int i = 0; i < lotsOfThreads.Count; i++)
                    {
                        lotsOfThreads[i].Join();
                    }
                    #endregion
                }
            }

            /// <summary>
            /// Analyze Eluting Peaks to find Monoisotopic Peaks.  This is better because it does not use Run
            /// </summary>
            /// <param name="run">Decon tools run containing run.ResultsColelction.ElutingPeaks</param>
            /// <param name="fileName">data file name</param>
            public void SimpleWorkflowExecutePart2b(List<ElutingPeak> discoveredPeaks, string fileName, string SQLFileName, bool msScanSetOnly)//Analyze Eluting Peaks to find Monoisotopic peaks vis decon tools
            {
                DatabaseLayer newDataBaseLayer = new DatabaseLayer();
                bool didThisWork = false;
                didThisWork = newDataBaseLayer.CreateFeatrueLiteTableInformation(SQLFileName);
                if (didThisWork == true)
                {
                    Console.WriteLine("TableInfoCreated");
                }

                #region setup
                //send to a thread
                int localProcessors = 1;
                string processorsStr = System.Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS");
                if (processorsStr != null)
                {
                    localProcessors = int.Parse(processorsStr);
                }
                //localProcessors = 2;
                //Console.WriteLine("There are " + localProcessors + " Processors");

                int peakCount = 0;
                int totalPeakCount = discoveredPeaks.Count;

                bool multithread = Parameters.Part2Parameters.Multithread;

                int threadCount = 0;
                List<Thread> lotsOfThreads = new List<Thread>();

                List<Run> listofRuns = new List<Run>();
                int runcounter = 0;

                #region check to ensure all ID are accounted for and =3.  3 means the eluting peak has ended
                int count3 = 0;
                int countNot3 = 0;
                foreach (ElutingPeak ePeak in discoveredPeaks)
                {
                    if (ePeak.ID != 3)
                    {
                        countNot3++;
                    }
                    count3++;
                }

                if (countNot3 > 0)
                {
                    Console.WriteLine("arg, not 3, PeakFinderController");
                }
                #endregion

                #endregion
                //sort Eluting peaks so we hit the most intense first
                //List<ElutingPeak> tempList = discoveredPeaks;
                discoveredPeaks = discoveredPeaks.OrderByDescending(p => p.Intensity).ToList();

                int limitThreadCount = discoveredPeaks.Count;
                //limitThreadCount = 10;
                float limitIntensity = (float)(discoveredPeaks[0].Intensity / Parameters.Part2Parameters.DynamicRangeToOne);



                int numberOfElements = discoveredPeaks.Count;
                int numberOfChunks = Parameters.Part2Parameters.MemoryDivider.NumberOfBlocks;
                int rank = Parameters.Part2Parameters.MemoryDivider.BlockNumber;
                int startIndex = 0;
                int stopIndex = 0;


                ProcessDivision newProcesDivider = new ProcessDivision();
                newProcesDivider.CalculateSplits(numberOfElements, numberOfChunks, rank, ref startIndex, ref stopIndex);

                //foreach (ElutingPeak ePeak in run.ResultCollection.ElutingPeakCollection)
                //double cuttoff = run.ResultCollection.ElutingPeakCollection.Count/Parameters.Part2Parameters.MemoryDivider.NumberOfBlocks;
                //int split = (int)Math.Round(cuttoff, 0);

                //pull these out front
                RunFactory rf = new RunFactory();
                
                //GoAccelerate accelerater = new GoAccelerate();
                GoDeconToolsControllerA newDeconToolsPart1;

                Console.WriteLine("StartLoop at : " + startIndex + " and stop at : " + stopIndex);
                for (int e = startIndex; e <= stopIndex; e++)//+1 so we includ the stop
                //for (int e = 0; e < run.ResultCollection.ElutingPeakCollection.Count; e++)
                //for (int e = split; e < run.ResultCollection.ElutingPeakCollection.Count; e++)
                //for (int e = 0; e < split; e++)
                {
                    #region full loop
                    ElutingPeak ePeak = discoveredPeaks[e];
           //         ElutingPeak tempPeak = discoveredPeaks[e];
           //         ElutingPeak ePeak = new ElutingPeak();

                    //ePeak.AggregateIntensity = tempPeak.AggregateIntensity;
                    //ePeak.ChargeState = tempPeak.ChargeState;
                    //ePeak.FitScore = tempPeak.FitScore;
                    //ePeak.ID = tempPeak.ID;
                    //ePeak.Intensity = tempPeak.Intensity;
                    //ePeak.Mass = tempPeak.Mass;
                    //ePeak.NumberOfPeaks = tempPeak.NumberOfPeaks;
                    //ePeak.NumberOfPeaksFlag = tempPeak.NumberOfPeaksFlag;
                    //ePeak.NumberOfPeaksMode = tempPeak.NumberOfPeaksMode;
                    //ePeak.RetentionTime = tempPeak.RetentionTime;
                    //ePeak.ScanEnd = tempPeak.ScanEnd;
                    //ePeak.ScanMaxIntensity = tempPeak.ScanMaxIntensity;
                    //ePeak.ScanSet = tempPeak.ScanSet;
                    //ePeak.ScanStart = tempPeak.ScanStart;
                    //ePeak.SummedIntensity = tempPeak.SummedIntensity;
                    

                    //printMemory("start");

                    if (ePeak.Intensity > limitIntensity)//fiter:  remove low abundnt ions
                    {
                        if (ePeak.ScanEnd - ePeak.ScanStart < Parameters.Part2Parameters.MaxScanSpread)//filter:  ensure the peaks don't tail too long
                        {
                            if (ePeak.ID == 3)//filter:  I think all should be in this state so this filter is not needed
                            {
                                //Console.WriteLine("eluting Peak " + peakCount.ToString() + " out of " + totalPeakCount.ToString());
                                Console.WriteLine("eluting Peak " + (e).ToString() + " out of " + totalPeakCount.ToString());

                                Run runGo = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, fileName);

                                peakCount++;
                                //create scansets

                                if (multithread)
                                {
                                    #region multithread create threads
                                    if (threadCount < limitThreadCount)
                                    {
                                        //Monitor.Enter(myLockObject);
                                        //RunFactory rf = new RunFactory();
                                        runGo = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, fileName);
                                        ////Run run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.Finnigan, inputDataFilename);

                                        //ElutingPeak tempElutingPeak = new ElutingPeak();
                                        //tempElutingPeak = ePeak;


                                        Stopwatch stopWatchMT = new Stopwatch();
                                        stopWatchMT.Start();

                                        #region -GoDeconToolsControllerA- get the data from the disk an sum it
                                        //sum spectra via msGenerator.  This hits the disk as it loads the file and returns a summed unTrimmed XYData
                                        newDeconToolsPart1 = new GoDeconToolsControllerA(runGo, Parameters);

                                        try//this is needed incase the scan information is not cashed.  It should be in this workflow and thus fast
                                        {
                                            ElutingPeakScanSet getScanSet = new ElutingPeakScanSet();
                                            //runGo.CurrentScanSet = getScanSet.CreateScanSetFromElutingPeakMax(runGo, ePeak.ScanMaxIntensity, ePeak.ScanStart, ePeak.ScanEnd);
                                            
                                            getScanSet.CreateScanSetFromElutingPeakMax(runGo, ePeak.ScanMaxIntensity, ePeak.ScanStart, ePeak.ScanEnd, m_summingMethod, Parameters.Part1Parameters.MSLevelOnly);
                                            runGo.CurrentScanSet = getScanSet.scanSet;
                                            getScanSet.Dispose();
                                        }
                                        catch (Exception ex)
                                        {
                                            throw ex;
                                        }
                                        Console.WriteLine("start get scan");

                                        //input: run.CurrentScanSet
                                        //input: Parameters
                                        //newDeconToolsPart1.GoLoadDataAndSumIt(runGo, Parameters);
                                        newDeconToolsPart1.GoLoadDataAndSumIt(runGo, m_summingMethod, ePeak.ScanStart, ePeak.ScanEnd);
                                        //output: run.ResultCollection.Run.XYData
                                        Console.WriteLine("stop get scan");

                                        #endregion

                                        stopWatchMT.Stop();
                                        Console.WriteLine("This took " + stopWatchMT.Elapsed + " seconds to find and assign features in eluting peaks");


                                        //Stopwatch stopWatchMT = new Stopwatch();
                                        //stopWatchMT.Start();

                                        //#region get the data from the disk an sum it
                                        ////run part one in serial since it is the one dealing with the disk
                                        //GoDeconToolsControllerA newDeconToolsPart1 = new GoDeconToolsControllerA(run, Parameters);
                                        //try//this is needed because we hit the disk.  However, the scanset should be stored somwwhere
                                        //{
                                        //    Console.WriteLine("get data and sum it ");
                                        //    ElutingPeakScanSet getScanSet = new ElutingPeakScanSet();
                                        //    run.CurrentScanSet = getScanSet.CreateScanSetFromElutingPeakSum(run, ePeak.ScanMaxIntensity, ePeak.ScanStart, ePeak.ScanEnd);
                                        //}
                                        //catch (Exception ex)
                                        //{
                                        //    throw ex;
                                        //}

                                        ////input: run.CurrentScanSet
                                        ////input: Parameters
                                        //newDeconToolsPart1.GoLoadDataAndSumIt(run, Parameters);
                                        ////output: run.ResultCollection.Run.XYData

                                        //#endregion

                                        //stopWatchMT.Stop();
                                        //Console.WriteLine("This took " + stopWatchMT.Elapsed + " seconds to find and assign features in eluting peaks");

                                        //TODO:  get and store all summed XYdata and store it in a List<List<XYData>>  Each spectra can then be farmed out into threads


                                        //TODO:  all this Go accelerate needs to be taken out of htis loop since this loop is used for pulling the XYdata

                                        //I am not sure if we need to copy run
                                        Console.WriteLine("add thread");

                                        //Monitor.Enter(myLockObject);
                                        GoAccelerate accelerater2 = new GoAccelerate();

                                        //new run
                                        runGo.XYData = new DeconTools.Backend.XYData();
                                        runGo.XYData = runGo.XYData;
                                        runGo.ResultCollection = new ResultCollection(runGo);
                                        runGo.ResultCollection = runGo.ResultCollection;
                                        runGo.ScanSetCollection = new ScanSetCollection();
                                        runGo.ScanSetCollection = runGo.ScanSetCollection;
                                        //runGo.PeakList = new List<IPeak>();
                                        runGo.PeakList = new List<DeconTools.Backend.Core.Peak>();
                                        runGo.MinLCScan = runcounter;
                                        listofRuns.Add(runGo);


                                        ElutingPeak localePeak = new ElutingPeak();
                                        localePeak = ePeak;

                                        //Console.WriteLine(ePeak.Mass.ToString());
                                        //accelerater2.SetValues(listofRuns[runcounter], localePeak, 1, localProcessors, TOLERANCE, Parameters, fileName, ePeak.ScanMaxIntensity);
                                        accelerater2.SetValues(listofRuns[runcounter], localePeak, 1, localProcessors, Parameters, fileName, ePeak.ScanMaxIntensity);

                                        runcounter++;

                                        //the problem here is too many threads calling the YAFMS reader at once.  we need to iterate through the main thread and load the data on one thread then statt the others
                                        //ThreadStart threadDelegate = new ThreadStart(accelerater2.GO2DeconTools);
                                        ThreadStart threadDelegate = new ThreadStart(accelerater2.GO2DeconTools);//run part 2 in threads
                                        lotsOfThreads.Add(new Thread(threadDelegate));

                                       // Monitor.Exit(myLockObject);
                                        //we need a semaphore to restrict the number of thresds that get started so we don't hit the disk all at once
                                        lotsOfThreads[threadCount].Name = "thread" + threadCount.ToString();
                                        //lotsOfThreads[threadCount].Start();
                                        //lotsOfThreads[threadCount].Join();
                                        threadCount++;
                                        if (threadCount == 160)
                                        {
                                            int t = 44;
                                            t = 66;
                                            int y = t;
                                        }
                                        Console.WriteLine("thread added");
                                        //Monitor.Exit(myLockObject);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region -GoDeconToolsControllerA- get the data from the disk an sum it
                                    //Stopwatch stopWatch = new Stopwatch();
                                    //stopWatch.Start();

        //                            GC.Collect();
        //                            printMemory("start of DeconToolsControllerB");

                                    //runGo = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, fileName);

        //                            GC.Collect();
        //                            printMemory("Create Run");

                                    ElutingPeak resultPeak = new ElutingPeak();
                                    resultPeak.ScanMaxIntensity = ePeak.ScanMaxIntensity;
                                    resultPeak.ScanStart = ePeak.ScanStart;
                                    resultPeak.ScanEnd = ePeak.ScanEnd;
                                    resultPeak.Mass = ePeak.Mass;
                                    resultPeak.PeakList = ePeak.PeakList;

                                    //sum spectra via msGenerator.  This hits the disk as it loads the file and returns a summed unTrimmed XYData
                                    newDeconToolsPart1 = new GoDeconToolsControllerA(runGo, Parameters);

        //                            GC.Collect();
        //                            printMemory("Create GoDeconToolsControllerA");

                                   // ScanSet tempScanSet = new ScanSet();
                                    try//this is needed incase the scan information is not cashed.  It should be in this workflow and thus fast
                                    {
                                        ElutingPeakScanSet getScanSet = new ElutingPeakScanSet();
                                        //runGo.CurrentScanSet = getScanSet.CreateScanSetFromElutingPeakMax(runGo, resultPeak.ScanMaxIntensity, resultPeak.ScanStart, resultPeak.ScanEnd);
                                        
                                        getScanSet.CreateScanSetFromElutingPeakMax(runGo, resultPeak.ScanMaxIntensity, resultPeak.ScanStart, resultPeak.ScanEnd, m_summingMethod, Parameters.Part1Parameters.MSLevelOnly);
                                        runGo.CurrentScanSet = getScanSet.scanSet;
                                        getScanSet.Dispose();
                                        //runGo.CurrentScanSet = getScanSet.CreateScanSetFromElutingPeakMax(runGo, ePeak.ScanMaxIntensity, ePeak.ScanStart, ePeak.ScanEnd);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }

                                    //runGo.CurrentScanSet = tempScanSet;
                                    //Console.WriteLine("start get scan");
                                   

     //                               GC.Collect();
     //                               printMemory("pre GoLoadDataAndSumIt");

                                    //input: run.CurrentScanSet
                                    //input: Parameters
                                    //newDeconToolsPart1.GoLoadDataAndSumIt(runGo, Parameters);
                                    newDeconToolsPart1.GoLoadDataAndSumIt(runGo, m_summingMethod, resultPeak.ScanStart, resultPeak.ScanEnd);

                          //          double[] tempXvalues = runGo.XYData.Xvalues;
                          //          double[] tempYValues = runGo.XYData.Yvalues;
                                    //output: run.ResultCollection.Run.XYData
                                    //Console.WriteLine("stop get scan");

     //                               GC.Collect();
     //                               printMemory("Run GoLoadDataAndSumIt");

                          //          runGo.CurrentScanSet = null;
                          //          runGo.Dispose();
                          //          newDeconToolsPart1.Dispose();


    //                                GC.Collect();
    //                                printMemory("cleanup after GoLoadDataAndSumIt");

                                    //stopWatch.Stop();
                                    //Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to find and assign features in eluting peaks");
                                    #endregion

                                    #region -GoDeconToolsControllerB- run part 2  to deisotope the the spine now that we have the correct XYData

   //                                 GC.Collect();
   //                                 printMemory("before accelerate");

                                    GoAccelerate accelerater = new GoAccelerate();
                                    
                                    //Run runGo2 = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, fileName);
                      //              Run runGo2 = runGo;
                       //             runGo2.CurrentScanSet = tempScanSet;
                       //             runGo2.XYData.Xvalues = tempXvalues;
                       //             runGo2.XYData.Yvalues = tempYValues;

//                                    GC.Collect();
//                                    printMemory("inside using");
                                    //GoAccelerate accelerater = new GoAccelerate();
                                    accelerater.SetValues(runGo, resultPeak, 1, localProcessors, Parameters, fileName, resultPeak.ScanMaxIntensity);

                                    accelerater.GO2DeconTools();//run part 2

                                    
                                    resultPeak = accelerater.m_elutingPeakResults[0];
                                    GoResults resultPeakIsos = new GoResults();
                                    resultPeakIsos = accelerater.m_IsotopeResults;
//                                  GC.Collect();
//                                  printMemory("inside using after decontools");

                                    //dispose in reverse order
                                    runGo.CurrentScanSet = null;
                                    runGo.Dispose();
                                    accelerater.Dispose();                                      

                                    newDeconToolsPart1.Dispose();
//                                   GC.Collect();
//                                   printMemory("after dispose");
                                   
  //                                  GC.Collect();
  //                                  printMemory("after");

                           //         tempXvalues = null;
                            //        tempYValues = null;
                           //         tempScanSet = null;
                                    //runGo.Dispose();
                                    //newDeconToolsPart1.Dispose();
                                    //value
                                    //ePeak.IsosResultList = null;

                                    GC.Collect();
                                    printMemory("baseline");

                                    

                                    CrossCorrelateResults organzeResults = new CrossCorrelateResults();

                                    //organzeResults.AssignCrossCorrelatedInformation(ePeak, Parameters);
                                    organzeResults.AssignCrossCorrelatedInformation(resultPeak,resultPeakIsos, Parameters);


                                    //if (ePeak.ID == 1)//write to disk
                                    if (resultPeak.ID == 1)//write to disk
                                    {
            //                            discoveredPeaks[e].ID = 1;
                                        
                                        bool isSuccessfull = false;

                                        GC.Collect();
                                        printMemory("baseline before write");

                                        using (DatabaseAdaptor newDataBaseAdaptor = new DatabaseAdaptor(SQLFileName))
                                        {

                                            //isSuccessfull = newDataBaseAdaptor.WriteElutingPeakToSQLite(ePeak);
                                            isSuccessfull = newDataBaseAdaptor.WriteElutingPeakToSQLite_Old(resultPeak);

                                            if (isSuccessfull)
                                            {
                                                Console.WriteLine("Data Stored");
                                            }
                                            else
                                            {
                                                Console.WriteLine("Data Not Stored");
                                            }

                                            //GC.Collect();
                                            //printMemory("baseline during write");

                                            //ePeak = null;
                                            //resultPeak = null;
                                        }
                                        //GC.Collect();
                                        //printMemory("baseline after write");

                                        GC.Collect();
                                        printMemory("baseline before write");
                                    }
                                    #endregion

                                    //clean up run
                                    //runGo.Dispose();//kill root
                                    //newDeconToolsPart1.Dispose();//kill GoDeconToolsControllerA
                                    resultPeak.Dispose();
                                }

                                #region old code

                                //////deal with the isotope info
                                //if (multithread)
                                //{


                                //    double high=0;
                                //    double low=0;
                                //    double peakMZ=0;
                                //    float peakIntensity =0;
                                //    foreach (IsosResult isoresult in ePeak.IsosResultList)
                                //    {
                                //        peakMZ = isoresult.IsotopicProfile.MonoPeakMZ;
                                //        //peakIntensity = (float)isoresult.IsotopicProfile.Peaklist[0].Height;
                                //        peakIntensity = (float)isoresult.IsotopicProfile.IntensityAggregate;
                                //        high = peakMZ + TOLERANCE;
                                //        low = peakMZ - TOLERANCE;

                                //        //there are 2 ways to get assigned.  if true, the principle mass is assgined
                                //        //else true, monisiotopic mass may be in the rest of the eluting peak list and can be assigned here.  I expect this is faster but it may not be.  could just let it go through the loop
                                //        if (ePeak.Mass <= high && ePeak.Mass >= low)
                                //        {
                                //            ePeak.RetentionTime = (float)(peakMZ - ePeak.Mass);
                                //            ePeak.Mass = peakMZ;
                                //            ePeak.Intensity = peakIntensity;
                                //            ePeak.ID = 0;
                                //            ePeak.IsosResultList = new List<StandardIsosResult>();//zero out list since we found our match
                                //            ePeak.PeakList = new List<MSPeakResult>();
                                //        }
                                //        else
                                //        {
                                //            //there is not else because there is only one monoisitopic peak optomized for

                                //    #region old code
                                //    //        //foreach (ElutingPeak ePeak2 in run.ResultCollection.ElutingPeakCollection)
                                //    //        //{
                                //    //        //    high = ePeak2.Mass + TOLERANCE;
                                //    //        //    low = ePeak2.Mass - TOLERANCE;
                                //    //        //    if (peakMZ <= high && peakMZ >= low)
                                //    //        //    {
                                //    //        //        ePeak2.Mass = peakMZ;
                                //    //        //        ePeak2.Intensity = peakIntensity;
                                //    //        //        ePeak2.ID = 0;
                                //    //        //        break;
                                //    //        //    }

                                //    //            //if (peakMZ < low)//this will work if the eluting peak list is sorted by mass.  it is not
                                //    //            //{
                                //    //            //    break;//there is no reason to keep going down the list
                                //    //            //}
                                //    //        //}
                                //    #endregion
                                //        }

                                //        ePeak.IsosResultList = new List<StandardIsosResult>();//zero out list since we found our match
                                //        ePeak.PeakList = new List<MSPeakResult>();
                                //    }
                                //}
                                #endregion
                            }
                            else
                            {
                                //Console.WriteLine(resultPeak.ID);
                                Console.ReadKey();
                                break;//this is to test the code
                            }
                        }
                    }


                    //ElutingPeak ePeak2 = ePeak;
                    //SimplifyElutingPeak(ref ePeak2);
                    //ClearElutingPeak(ref ePeak2);
                    ePeak.Dispose();
                    discoveredPeaks[e].Dispose();
                    //printMemory("after clear peak list");
                    int k = 5;
                    k++;

                    GC.Collect();

                    #endregion
                }//end for eluting peaks


                if (multithread)
                {
                    #region multithread run threads
                    //Monitor.Enter(myLockObject);
                    for (int i = 0; i < lotsOfThreads.Count; i++)
                    {
                        Console.WriteLine("start thread " + i);
                        lotsOfThreads[i].Start();
                    }

                    //Monitor.Exit(myLockObject);
                    for (int i = 0; i < lotsOfThreads.Count; i++)
                    {
                        lotsOfThreads[i].Join();
                    }
                    #endregion
                }
            }

        
        #endregion

 //TODO remove this
        public void printMemory(string printline)
        {
            string procName = Process.GetCurrentProcess().ProcessName;
            Process newProcess = Process.GetCurrentProcess();

            double printValue = 0;
            long memory = 0;
            memory = newProcess.PrivateMemorySize64;
            printValue = (double)memory / 1000000;
            Console.WriteLine("     PrivateMemorySize64 " + printValue.ToString() + " MB at " + printline);

        }
    }
}
