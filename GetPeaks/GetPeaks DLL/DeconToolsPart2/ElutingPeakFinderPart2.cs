using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;

using GetPeaks_DLL.SQLite;
using DeconTools.Backend.ProcessingTasks;
using GetPeaks_DLL;
using DeconTools.Backend.Runs;
using GetPeaks_DLL.Go_Decon_Modules;
using System.Diagnostics;
using GetPeaks_DLL.Objects;
using MemoryOverloadProfilierX86;
using DeconTools.Backend;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.Functions;

namespace GetPeaks_DLL.DeconToolsPart2
{
    public class ElutingPeakFinderPart2:IDisposable
    {
        #region Properties

        //public MSGeneratorFactory msgenFactory { get; set; }
        //public Task msGenerator {get;set;}
        //public SimpleWorkflowParameters Parameters;
        public DeconToolsPeakDetector msPeakDetector;

        #endregion

        #region Constructors
            
            public ElutingPeakFinderPart2()
            {
                //this.msgenFactory = new MSGeneratorFactory();
            }

            public ElutingPeakFinderPart2(Run run, SimpleWorkflowParameters parameters)
                : this()
            { 
                //this.Parameters = parameters;
                //InitializeTasks(run, this.Parameters);
                InitializeTasks(run, parameters);
            }

            private void InitializeTasks(Run run, SimpleWorkflowParameters parameters)
            {
                ///msGenerator = msgenFactory.CreateMSGenerator(run.MSFileType);
                msPeakDetector = new DeconToolsPeakDetector(parameters.Part1Parameters.MSPeakDetectorPeakBR, parameters.Part1Parameters.ElutingPeakNoiseThreshold, parameters.Part2Parameters.PeakFitType, false);
                msPeakDetector.PeaksAreStored = true;
                //msPeakDetector.PeakFitType = parameters.PeakFitType;
            }

        #endregion

            /// <summary>
        /// Analyze Eluting Peaks to find Monoisotopic Peaks.  This is better because it does not use Run
        /// </summary>
        /// <param name="run">Decon tools run containing run.ResultsColelction.ElutingPeaks</param>
        /// <param name="fileName">data file name</param>
        public void SimpleWorkflowExecutePart2d(List<ElutingPeak> discoveredPeaks, InputOutputFileName fileIn, SimpleWorkflowParameters Parameters, ref int numberOfHits, TransformerObject transformer2)//Analyze Eluting Peaks to find Monoisotopic peaks vis decon tools
        {
            DatabaseLayer newDataBaseLayer = new DatabaseLayer();
            bool didThisWork = false;
            //didThisWork = newDataBaseLayer.CreateFeatrueLiteTableInformation(SQLFileName);
            didThisWork = newDataBaseLayer.CreateFeatrueLiteTableInformation(fileIn.OutputSQLFileName);
            if (didThisWork == true)
            {
                Console.WriteLine("TableInfoCreated");
            }

            #region setup
            
            int peakCount = 0;
            int totalPeakCount = discoveredPeaks.Count;

           
            
            List<Run> listofRuns = new List<Run>();


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

            //pull these out front
            RunFactory rf = new RunFactory();

            //GoAccelerate accelerater = new GoAccelerate();
            GoDeconToolsControllerA newDeconToolsPart1;

            GoDeconToolsControllerB newDeconToolsPart2 = new GoDeconToolsControllerB(Parameters, DeconTools.Backend.Globals.MSFileType.Undefined);

            CrossCorrelateResults organzeResults = new CrossCorrelateResults();

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

                            //printMemory("New Eluting Peak");

                            //Run runGo = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, fileName);
                            Run runGo = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, fileIn.InputFileName);

                            peakCount++;
                            //create scansets

                            #region -GoDeconToolsControllerA- get the data from the disk an sum it
                            //Stopwatch stopWatch = new Stopwatch();
                            //stopWatch.Start();

                            ElutingPeak resultPeak = new ElutingPeak();
                            resultPeak.ScanMaxIntensity = ePeak.ScanMaxIntensity;
                            resultPeak.ScanStart = ePeak.ScanStart;
                            resultPeak.ScanEnd = ePeak.ScanEnd;
                            resultPeak.Mass = ePeak.Mass;
                            resultPeak.PeakList = ePeak.PeakList;
                            resultPeak.ID = -1;

                            SimpleWorkflowParameters insideLoopParameters = new SimpleWorkflowParameters();
                            insideLoopParameters.Part1Parameters = Parameters.Part1Parameters;
                            insideLoopParameters.Part2Parameters = Parameters.Part2Parameters;
                            ScanSumSelectSwitch summingMethod = insideLoopParameters.SummingMethod;

                            //printMemory("pre GoDeconToolsControllerA");

                            //sum spectra via msGenerator.  This hits the disk as it loads the file and returns a summed unTrimmed XYData
                            newDeconToolsPart1 = new GoDeconToolsControllerA(runGo, insideLoopParameters);

                            try//this is needed incase the scan information is not cashed.  It should be in this workflow and thus fast
                            {
                                ElutingPeakScanSet getScanSet = new ElutingPeakScanSet();
                                
                                getScanSet.CreateScanSetFromElutingPeakMax(runGo, resultPeak.ScanMaxIntensity, resultPeak.ScanStart, resultPeak.ScanEnd, summingMethod, insideLoopParameters.Part2Parameters.MSLevelOnly);
                                runGo.CurrentScanSet = getScanSet.scanSet;
                                getScanSet.Dispose();
                                //runGo.CurrentScanSet = getScanSet.CreateScanSetFromElutingPeakMax(runGo, ePeak.ScanMaxIntensity, ePeak.ScanStart, ePeak.ScanEnd);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }

                            //printMemory("pre GoLoadDataAndSumIt");

                            //input: run.CurrentScanSet
                            //input: Parameters
                            //newDeconToolsPart1.GoLoadDataAndSumIt(runGo, insideLoopParameters);
                            newDeconToolsPart1.GoLoadDataAndSumIt(runGo, summingMethod, resultPeak.ScanStart, resultPeak.ScanEnd);


                            //printMemory("post GoLoadDataAndSumIt");
                                     // double[] tempXvalues = runGo.XYData.Xvalues;
                                     // double[] tempYValues = runGo.XYData.Yvalues;
                            //output: run.ResultCollection.Run.XYData
                            //Console.WriteLine("stop get scan");

                           
                            //stopWatch.Stop();
                            //Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to find and assign features in eluting peaks");
                            #endregion

                            #region -GoDeconToolsControllerB- run part 2  to deisotope the the spine now that we have the correct XYData

                                                             //GC.Collect();
                                                             //printMemory("before accelerate");

                            //GoAccelerate accelerater = new GoAccelerate();

                            //Run runGo2 = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, fileName);
                            //              Run runGo2 = runGo;
                            //             runGo2.CurrentScanSet = tempScanSet;
                            //             runGo2.XYData.Xvalues = tempXvalues;
                            //             runGo2.XYData.Yvalues = tempYValues;

                            //GC.Collect();
                            //printMemory("inside using");
                            //GoAccelerate accelerater = new GoAccelerate();
                            //int localProcessors = 1;
                            //accelerater.SetValues(runGo, resultPeak, 1, localProcessors, Parameters, fileName, resultPeak.ScanMaxIntensity);

                            //accelerater.GO2DeconTools();//run part 2

                            //printMemory("     Before FX");
                            //printMemory("     Before FX");

                            //GoDeconToolsControllerB newDeconToolsPart2 = new GoDeconToolsControllerB(runGo, insideLoopParameters);

                            //newDeconToolsPart2.Dispose();
                            //newDeconToolsPart2 = null;
                            //printMemory("     Middle FX");
                            
                            //DeconToolsV2.HornTransform.clsHornTransform transformer2 = new DeconToolsV2.HornTransform.clsHornTransform();

                            GoResults isotopeOutput = newDeconToolsPart2.GODeconToolsFX(runGo, insideLoopParameters, transformer2);

                            //transformer2.active = false;
                            //newDeconToolsPart2.Dispose();
                            
                           
                            //insideLoopParameters.Dispose();
                            //insideLoopParameters = null;
                            //int isotopesFound = isotopeOutput.IsosResultList.Count;

                            //isotopeOutput.Dispose();

                            

                            //newDeconToolsPart2.Dispose();
  //a                          newDeconToolsPart2 = null;
                            //resultPeak = accelerater.m_elutingPeakResults[0];
                            //GoResults resultPeakIsos = new GoResults();
                            //resultPeakIsos = accelerater.m_IsotopeResults;
                            //GC.Collect();
                            //printMemory("inside using after decontools");

                            ////dispose in reverse order
                            //runGo.CurrentScanSet = null;
                            //runGo.Dispose();
                           // accelerater.Dispose();

                            
                            ////                                   GC.Collect();
                            ////                                   printMemory("after dispose");

                            ////                                  GC.Collect();
                            ////                                  printMemory("after");

                            ////         tempXvalues = null;
                            ////        tempYValues = null;
                            ////         tempScanSet = null;
                            ////runGo.Dispose();
                            ////newDeconToolsPart1.Dispose();
                            ////value
                            ////ePeak.IsosResultList = null;

                            //GC.Collect();
                            //printMemory("baseline");

                            

                            

                            ////organzeResults.AssignCrossCorrelatedInformation(ePeak, Parameters);
                            organzeResults.AssignCrossCorrelatedInformation(resultPeak, isotopeOutput, Parameters);

                            
                            //isotopeOutput.Dispose();
                            ////if (ePeak.ID == 1)//write to disk
                            if (resultPeak.ID == 1)//write to disk
                            {
                                //                            discoveredPeaks[e].ID = 1;

                                bool isSuccessfull = false;

                                numberOfHits++;
                                //printMemory("baseline before write");

                                //using (DatabaseAdaptor newDataBaseAdaptor = new DatabaseAdaptor(SQLFileName))
                                using (DatabaseAdaptor newDataBaseAdaptor = new DatabaseAdaptor(fileIn.OutputSQLFileName))
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


                                    //printMemory("baseline during write");

                                    //ePeak = null;
                                    //resultPeak = null;
                                }

                                //printMemory("baseline after write");

                                
                            }
                            #endregion

                            

                            runGo.Dispose();
                            runGo = null;


                            newDeconToolsPart1.Dispose();
                            isotopeOutput.Dispose();
                            insideLoopParameters.Dispose();
                            resultPeak = null;
                            //clean up run
                            //runGo.Dispose();//kill root
                            //newDeconToolsPart1.Dispose();//kill GoDeconToolsControllerA
   //a                         resultPeak.Dispose();

                            //printMemory("NextElutingpPeak");

                            //Console.ReadKey();

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

                #endregion
            }//end for eluting peaks

            Profiler newProfiler = new Profiler();
            newProfiler.printMemory("All done!");  
        }

        /// Analyze Eluting Peaks to find Monoisotopic Peaks.  This is better because it does not use Run
        /// </summary>
        /// <param name="run">Decon tools run containing run.ResultsColelction.ElutingPeaks</param>
        /// <param name="fileName">data file name</param>
        public void SimpleWorkflowExecutePart2e(List<ElutingPeakOmics> discoveredPeaks, InputOutputFileName fileIn, SimpleWorkflowParameters Parameters, ref int numberOfHits, TransformerObject transformer2)//Analyze Eluting Peaks to find Monoisotopic peaks vis decon tools
        {
            DatabaseLayer newDataBaseLayer = new DatabaseLayer();
            bool didThisWork = false;
            //didThisWork = newDataBaseLayer.CreateFeatrueLiteTableInformation(SQLFileName);
            didThisWork = newDataBaseLayer.CreateFeatrueLiteTableInformation(fileIn.OutputSQLFileName);
            if (didThisWork == true)
            {
                //Console.WriteLine("TableInfoCreated");//just for printing
            }

            #region setup

            int peakCount = 0;
            int totalPeakCount = discoveredPeaks.Count;

            List<Run> listofRuns = new List<Run>();

            #region check to ensure all ID are accounted for and =3.  3 means the eluting peak has ended
            int count3 = 0;
            int countNot3 = 0;
            foreach (ElutingPeakOmics ePeak in discoveredPeaks)
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

            GoDeconToolsControllerB newDeconToolsPart2 = new GoDeconToolsControllerB(Parameters, DeconTools.Backend.Globals.MSFileType.Undefined);

            CrossCorrelateResults organzeResults = new CrossCorrelateResults();

   //print         Console.WriteLine("StartLoop at : " + startIndex + " and stop at : " + stopIndex);
            for (int e = startIndex; e <= stopIndex; e++)//+1 so we includ the stop
            //for (int e = 0; e < run.ResultCollection.ElutingPeakCollection.Count; e++)
            //for (int e = split; e < run.ResultCollection.ElutingPeakCollection.Count; e++)
            //for (int e = 0; e < split; e++)
            {
                #region full loop
                ElutingPeakOmics ePeak = discoveredPeaks[e];
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
                            Console.WriteLine("eluting Peak Omics " + (e).ToString() + " out of " + totalPeakCount.ToString());

                            //printMemory("New Eluting Peak");

                            Run runGo = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, fileIn.InputFileName);

                            peakCount++;
                            //create scansets

                            #region -GoDeconToolsControllerA- get the data from the disk an sum it
                            //Stopwatch stopWatch = new Stopwatch();
                            //stopWatch.Start();

                            ElutingPeakOmics resultPeak = new ElutingPeakOmics();
                            resultPeak.ScanMaxIntensity = ePeak.ScanMaxIntensity;
                            resultPeak.ScanStart = ePeak.ScanStart;
                            resultPeak.ScanEnd = ePeak.ScanEnd;
                            resultPeak.Mass = ePeak.Mass;
                            resultPeak.PeakList = ePeak.PeakList;
                            resultPeak.ID = -1;

                            //SimpleWorkflowParameters insideLoopParameters = new SimpleWorkflowParameters();
                            //insideLoopParameters.Part1Parameters = Parameters.Part1Parameters;
                            //insideLoopParameters.Part2Parameters = Parameters.Part2Parameters;

                            SimpleWorkflowParameters insideLoopParameters = ObjectCopier.Clone<SimpleWorkflowParameters>(Parameters);
                            ScanSumSelectSwitch summingMethod = insideLoopParameters.SummingMethod;
                            //printMemory("pre GoDeconToolsControllerA");

                            //sum spectra via msGenerator.  This hits the disk as it loads the file and returns a summed unTrimmed XYData
                            newDeconToolsPart1 = new GoDeconToolsControllerA(runGo, insideLoopParameters);

                          
                            try//this is needed incase the scan information is not cashed.  It should be in this workflow and thus fast
                            {
                                ElutingPeakScanSet getScanSet = new ElutingPeakScanSet();
                                //runGo.CurrentScanSet = getScanSet.CreateScanSetFromElutingPeakMax(runGo, resultPeak.ScanMaxIntensity, resultPeak.ScanStart, resultPeak.ScanEnd);
                                
                                getScanSet.CreateScanSetFromElutingPeakMax(runGo, resultPeak.ScanMaxIntensity, resultPeak.ScanStart, resultPeak.ScanEnd, summingMethod, insideLoopParameters.Part2Parameters.MSLevelOnly);
                                runGo.CurrentScanSet = getScanSet.scanSet;
                                getScanSet.Dispose();//TODO this is in the wrong place
                                //runGo.CurrentScanSet = getScanSet.CreateScanSetFromElutingPeakMax(runGo, ePeak.ScanMaxIntensity, ePeak.ScanStart, ePeak.ScanEnd);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }

                            //printMemory("pre GoLoadDataAndSumIt");

                            //input: run.CurrentScanSet
                            //input: Parameters
                            //newDeconToolsPart1.GoLoadDataAndSumIt(runGo, insideLoopParameters);
                            newDeconToolsPart1.GoLoadDataAndSumIt(runGo, summingMethod, resultPeak.ScanStart, resultPeak.ScanEnd);


                            //printMemory("post GoLoadDataAndSumIt");
                            // double[] tempXvalues = runGo.XYData.Xvalues;
                            // double[] tempYValues = runGo.XYData.Yvalues;
                            //output: run.ResultCollection.Run.XYData
                            //Console.WriteLine("stop get scan");


                            //stopWatch.Stop();
                            //Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to find and assign features in eluting peaks");
                            #endregion

                            #region -GoDeconToolsControllerB- run part 2  to deisotope the the spine now that we have the correct XYData

                            //GC.Collect();
                            //printMemory("before accelerate");

                            //GoAccelerate accelerater = new GoAccelerate();

                            //Run runGo2 = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, fileName);
                            //              Run runGo2 = runGo;
                            //             runGo2.CurrentScanSet = tempScanSet;
                            //             runGo2.XYData.Xvalues = tempXvalues;
                            //             runGo2.XYData.Yvalues = tempYValues;

                            //GC.Collect();
                            //printMemory("inside using");
                            //GoAccelerate accelerater = new GoAccelerate();
                            //int localProcessors = 1;
                            //accelerater.SetValues(runGo, resultPeak, 1, localProcessors, Parameters, fileName, resultPeak.ScanMaxIntensity);

                            //accelerater.GO2DeconTools();//run part 2

                            //printMemory("     Before FX");
                            //printMemory("     Before FX");

                            //GoDeconToolsControllerB newDeconToolsPart2 = new GoDeconToolsControllerB(runGo, insideLoopParameters);
                            
                            //newDeconToolsPart2.Dispose();
                            //newDeconToolsPart2 = null;
                            //printMemory("     Middle FX");
                            //DeconToolsV2.HornTransform.clsHornTransform transformer2 = new DeconToolsV2.HornTransform.clsHornTransform();


                            GoResults isotopeOutput = newDeconToolsPart2.GODeconToolsFX(runGo, insideLoopParameters, transformer2);

                            //newDeconToolsPart2.Dispose();


                            //insideLoopParameters.Dispose();
                            //insideLoopParameters = null;
                            //int isotopesFound = isotopeOutput.IsosResultList.Count;

                            //isotopeOutput.Dispose();



                            //newDeconToolsPart2.Dispose();
                            //a                          newDeconToolsPart2 = null;
                            //resultPeak = accelerater.m_elutingPeakResults[0];
                            //GoResults resultPeakIsos = new GoResults();
                            //resultPeakIsos = accelerater.m_IsotopeResults;
                            //GC.Collect();
                            //printMemory("inside using after decontools");

                            ////dispose in reverse order
                            //runGo.CurrentScanSet = null;
                            //runGo.Dispose();
                            // accelerater.Dispose();


                            ////                                   GC.Collect();
                            ////                                   printMemory("after dispose");

                            ////                                  GC.Collect();
                            ////                                  printMemory("after");

                            ////         tempXvalues = null;
                            ////        tempYValues = null;
                            ////         tempScanSet = null;
                            ////runGo.Dispose();
                            ////newDeconToolsPart1.Dispose();
                            ////value
                            ////ePeak.IsosResultList = null;

                            //GC.Collect();
                            //printMemory("baseline");




                            IsotopeObject isotopeStorage = new IsotopeObject();
                            ////organzeResults.AssignCrossCorrelatedInformation(ePeak, Parameters);
                            organzeResults.AssignCrossCorrelatedInformation(resultPeak, isotopeOutput, isotopeStorage, Parameters);


                            //isotopeOutput.Dispose();
                            ////if (ePeak.ID == 1)//write to disk
                            if (resultPeak.ID == 1)//write to disk
                            {
                                //                            discoveredPeaks[e].ID = 1;

                                bool isSuccessfull = false;

                                numberOfHits++;
                                //printMemory("baseline before write");

                                //using (DatabaseAdaptor newDataBaseAdaptor = new DatabaseAdaptor(SQLFileName))
                                using (DatabaseAdaptor newDataBaseAdaptor = new DatabaseAdaptor(fileIn.OutputSQLFileName))
                                {

                                    //isSuccessfull = newDataBaseAdaptor.WriteElutingPeakToSQLite(ePeak);
                                    isSuccessfull = newDataBaseAdaptor.WriteElutingPeakToSQLite_Old(resultPeak);
                                    isSuccessfull = newDataBaseAdaptor.WriteIsotopeStorageToSQLite_Old(isotopeStorage);
                                    if (isSuccessfull)
                                    {
                                        Console.WriteLine("Data Stored");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Data Not Stored");
                                    }

                                    //Profiler memoryCheck = new Profiler();
                                    //memoryCheck.printMemory("baseline during write");

                                    //ePeak = null;
                                    //resultPeak = null;
                                }

                                //printMemory("baseline after write");


                            }
                            #endregion

                            isotopeStorage.Dispose();

                            runGo.Dispose();
                            runGo = null;


                            newDeconToolsPart1.Dispose();
                            isotopeOutput.Dispose();
                            insideLoopParameters.Dispose();
                            resultPeak = null;
                            //clean up run
                            //runGo.Dispose();//kill root
                            //newDeconToolsPart1.Dispose();//kill GoDeconToolsControllerA
                            //a                         resultPeak.Dispose();

                            //printMemory("NextElutingpPeak");

                            //Console.ReadKey();

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

                #endregion
            }//end for eluting peaks

            //Profiler newProfiler = new Profiler();
            //newProfiler.printMemory("All done!");

        }



        //private Object databaseLock = new Object();
        //private Object deconvolutionLock = new Object();

        /// Analyze Eluting Peaks to find Monoisotopic Peaks.  This is better because it does not use Run
        /// </summary>
        /// <param name="run">Decon tools run containing run.ResultsColelction.ElutingPeaks</param>
        /// <param name="fileName">data file name</param>
        public void SimpleWorkflowExecutePart2Memory(List<ElutingPeakOmics> discoveredPeaks, InputOutputFileName fileIn, SimpleWorkflowParameters Parameters, ref int numberOfHits, TransformerObject transformer2, ref int dataFileIterator)//Analyze Eluting Peaks to find Monoisotopic peaks vis decon tools
        {
            //string fileName
            //string SQLFileName
            //lock (databaseLock)//some times the memory goes up inside this lock
            //lock (transformer2.DatabaseLock)//some times the memory goes up inside this lock
            //{
                int threadName = System.Threading.Thread.CurrentThread.ManagedThreadId;
                if (1 == 1)
                {
                    #region inside
                    DatabaseLayer newDataBaseLayer = new DatabaseLayer();
                    bool didThisWork = false;
                    string localSQLFileName = transformer2.SQLiteTranformerFolderPath + @"\" + transformer2.SQLiteTranformerFileName + @".db";
                    
                    //didThisWork = newDataBaseLayer.CreateFeatrueLiteTableInformation(SQLFileName);
                    didThisWork = newDataBaseLayer.CreateFeatrueLiteTableInformation(fileIn.OutputSQLFileName);
                    //didThisWork = newDataBaseLayer.CreateFeatrueLiteTableInformation(localSQLFileName);

                    if (didThisWork == true)
                    {
                        //Console.WriteLine("TableInfoCreated");//just for printing
                    }
                    else
                    {
                        transformer2.ErrorLog.Add("this did not work, ElutingPeakFinderPart2");
                    }

                    if (3 == 3)
                    {
                        #region inside
                        #region setup

                        int peakCount = 0;
                        int totalPeakCount = discoveredPeaks.Count;

                        List<Run> listofRuns = new List<Run>();

                        #region check to ensure all ID are accounted for and =3.  3 means the eluting peak has ended
                        int count3 = 0;
                        int countNot3 = 0;
                        foreach (ElutingPeakOmics ePeak in discoveredPeaks)
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
                        else
                        {
                            //transformer2.ErrorLog.Add("this is the correct path, ElutingPeakFindePart2");
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
                        
                        int startIndexLoop = startIndex;
                        int stopIndexLoop = stopIndex;
                        
                        newProcesDivider = null;
                        //foreach (ElutingPeak ePeak in run.ResultCollection.ElutingPeakCollection)
                        //double cuttoff = run.ResultCollection.ElutingPeakCollection.Count/Parameters.Part2Parameters.MemoryDivider.NumberOfBlocks;
                        //int split = (int)Math.Round(cuttoff, 0);

                        //pull these out front
                        if (4 == 4)
                        {
                            #region inside
                            RunFactory rf = new RunFactory();

                            //GoAccelerate accelerater = new GoAccelerate();

                            if (3 == 3)
                            {
                                #region inside
                                GoDeconToolsControllerA newDeconToolsPart1;

                                if (1 ==1)//passes memory 3-21-11
                                {
                                    //Profiler newProfiler = new Profiler();

                                    //newProfiler.printMemory("++pre controllerB " + threadName);
                                    //newProfiler.printMemory("++pre controllerB " + threadName);
                                    //TODO dispose GoDeconToolsControllerB

                                    SimpleWorkflowParameters controllerBParameters = ObjectCopier.Clone<SimpleWorkflowParameters>(Parameters);
                                    GoDeconToolsControllerB newDeconToolsPart2 = new GoDeconToolsControllerB(controllerBParameters, DeconTools.Backend.Globals.MSFileType.Undefined);
                                    ScanSumSelectSwitch summingMethod = controllerBParameters.SummingMethod;

                                    if (2 == 2)//passes memory 3-21-11
                                    {
                                        #region inside
                                        //newProfiler.printMemory("pre organize results " + threadName);
                                        CrossCorrelateResults organzeResults = new CrossCorrelateResults();

                                        //print         Console.WriteLine("StartLoop at : " + startIndex + " and stop at : " + stopIndex);
                                        for (int e = startIndexLoop; e <= stopIndexLoop; e++)//+1 so we includ the stop
                                        
                                        //for (int e = startIndex; e <= stopIndex; e++)//+1 so we includ the stop
                                        //for (int e = 0; e < run.ResultCollection.ElutingPeakCollection.Count; e++)
                                        //for (int e = split; e < run.ResultCollection.ElutingPeakCollection.Count; e++)
                                        //for (int e = 0; e < split; e++)
                                        {
                                            if (1 == 1)//passes memory 3-21-11
                                            {
                                                #region full loop
                                                //ElutingPeakOmics ePeak = ObjectCopier.Clone<ElutingPeakOmics>(discoveredPeaks[e]); //discoveredPeaks[e];
                                                //TODO this may hold on to it?
                                                ElutingPeakOmics ePeak = discoveredPeaks[e];
                                                if (ePeak.Intensity > limitIntensity)//fiter:  remove low abundnt ions
                                                {
                                                    int scanRange = ePeak.ScanEnd - ePeak.ScanStart;
                                                    if (scanRange < Parameters.Part2Parameters.MaxScanSpread)//filter:  ensure the peaks don't tail too long
                                                    {
                                                        if (ePeak.ID == 3)//filter:  I think all should be in this state so this filter is not needed
                                                        {
                                                            //Console.WriteLine("eluting Peak " + peakCount.ToString() + " out of " + totalPeakCount.ToString());
                                                            //Console.WriteLine("eluting Peak Omics " + (e).ToString() + " out of " + totalPeakCount.ToString());

                                                            //printMemory("New Eluting Peak");

                                                            if (7 == 7)//passed memory 3-21-11
                                                            {
                                                                #region inside
                                                                //Console.WriteLine(" ");
                                                                //newProfiler.printMemory("Pre Run creation " + threadName);
                                                                //Console.WriteLine("before run is created");
                                                                //Console.ReadKey();

                                                                //Run runGo = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, fileName);
                                                                
                                                                Run runGo = GoCreateRun.CreateRun(fileIn);
                                                                peakCount++;
                                                                //create scansets

                                                                if (1 == 1)//passed memory 3-21-11
                                                                {
                                                                    #region inside
                                                                    #region -GoDeconToolsControllerA- get the data from the disk an sum it
                                                                    //Stopwatch stopWatch = new Stopwatch();
                                                                    //stopWatch.Start();

                                                                    //TODO this may hold on to EPeak
                                                                    //newProfiler.printMemory("result peak creation " + threadName);
                                                                    ElutingPeakOmics resultPeak = new ElutingPeakOmics();
                                                                    resultPeak.ScanMaxIntensity = ePeak.ScanMaxIntensity;
                                                                    resultPeak.ScanStart = ePeak.ScanStart;
                                                                    resultPeak.ScanEnd = ePeak.ScanEnd;
                                                                    resultPeak.Mass = ePeak.Mass;
                                                                    resultPeak.PeakList = ePeak.PeakList;
                                                                    resultPeak.ID = -1;

                                                                    ePeak.Dispose();
                                                                    discoveredPeaks = null;
                                                                    //SimpleWorkflowParameters insideLoopParameters = new SimpleWorkflowParameters();
                                                                    //insideLoopParameters.Part1Parameters = Parameters.Part1Parameters;
                                                                    //insideLoopParameters.Part2Parameters = Parameters.Part2Parameters;
                                                                    //newProfiler.printMemory("after rezult peak creation " + threadName);
                                                                    //SimpleWorkflowParameters insideLoopParameters = ObjectCopier.Clone<SimpleWorkflowParameters>(Parameters);
                                                                    //newProfiler.printMemory("with loop " + threadName);
                                                                    //printMemory("pre GoDeconToolsControllerA");

                                                                    //sum spectra via msGenerator.  This hits the disk as it loads the file and returns a summed unTrimmed XYData
                                                                    //newDeconToolsPart1 = new GoDeconToolsControllerA(runGo, insideLoopParameters);
                                                                    newDeconToolsPart1 = new GoDeconToolsControllerA(runGo, controllerBParameters);
                                                                    //newProfiler.printMemory("pre loop parametersX " + threadName);
                                                                    try//this is needed incase the scan information is not cashed.  It should be in this workflow and thus fast
                                                                    {
                                                                        ElutingPeakScanSet getScanSet = new ElutingPeakScanSet();
                                                                        //runGo.CurrentScanSet = getScanSet.CreateScanSetFromElutingPeakMax(runGo, resultPeak.ScanMaxIntensity, resultPeak.ScanStart, resultPeak.ScanEnd);
                                                                        //getScanSet.CreateScanSetFromElutingPeakMax(runGo, resultPeak.ScanMaxIntensity, resultPeak.ScanStart, resultPeak.ScanEnd);
                                                                        
                                                                        getScanSet.CreateScanSetFromElutingPeakMax(runGo, resultPeak.ScanMaxIntensity, resultPeak.ScanStart, resultPeak.ScanEnd, summingMethod, controllerBParameters.Part2Parameters.MSLevelOnly);
                                                                        runGo.CurrentScanSet = getScanSet.scanSet;
                                                                        //getScanSet.Dispose();
                                                                        //runGo.CurrentScanSet = getScanSet.CreateScanSetFromElutingPeakMax(runGo, ePeak.ScanMaxIntensity, ePeak.ScanStart, ePeak.ScanEnd);

                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        transformer2.ErrorLog.Add("exception on try, ElutingPeakFinderPart2");
                                                                        throw ex;
                                                                    }
                                                                    //newProfiler.printMemory("pre go get data " + threadName);
                                                                    //printMemory("pre GoLoadDataAndSumIt");

                                                                    #region save XY file part 1
                                                                    bool print = false;

                                                                    int weNeedToCatchTheScanSetPriorToGoLoadAndSum = 0;
                                                                    if (print)
                                                                    {
                                                                        //if (runGo.CurrentScanSet.IndexValues.Count > 5)
                                                                        if (scanRange > 5)
                                                                        {
                                                                            weNeedToCatchTheScanSetPriorToGoLoadAndSum = runGo.CurrentScanSet.IndexValues.Count;
                                                                        }
                                                                    }
                                                                    #endregion

                                                                    //input: run.CurrentScanSet
                                                                    //input: Parameters
                                                                    //newDeconToolsPart1.GoLoadDataAndSumIt(runGo, insideLoopParameters);
                                                                    if (5 == 5)
                                                                    {
                                                                        newDeconToolsPart1.GoLoadDataAndSumIt(runGo, summingMethod, resultPeak.ScanStart, resultPeak.ScanEnd);
                                                                    }

                                                                    #region save XY file part 2
                                                                    if (print)
                                                                    {
                                                                        if (weNeedToCatchTheScanSetPriorToGoLoadAndSum > 1)
                                                                        {
                                                                            dataFileIterator++;

                                                                            if (dataFileIterator > 1)//28
                                                                            {
                                                                                //IRapidCompare compareHere = new CompareContrast2();
                                                                                IXYDataWriter newXYWriter = new DataXYDataWriter();
                                                                                int massInt = Convert.ToInt32(resultPeak.Mass);
                                                                                string scanStr = runGo.CurrentScanSet.IndexValues[0].ToString();
                                                                                string scanStr2 = runGo.CurrentScanSet.IndexValues[runGo.CurrentScanSet.IndexValues.Count-1].ToString();
                                                                                string path = @"V:\XYOut_" + scanStr + "_"+ scanStr2 +"_" + massInt + "_R" + dataFileIterator.ToString() + ".txt";

                                                                                //1.  extract XYData
                                                                                List<PNNLOmics.Data.XYData> extractedXYData;
                                                                                ConvertXYData.RunXYDataToOmicsXYData(runGo, out extractedXYData);

                                                                                int t = newXYWriter.WriteOmicsXYData(extractedXYData, path);
                                                                            }
                                                                        }
                                                                    }
                                                                    #endregion

                                                                    // newProfiler.printMemory("after we got data " + threadName);
                                                                    //stopWatch.Stop();
                                                                    //Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to find and assign features in eluting peaks");
                                                                    #endregion

                                                                    if (2 == 2)//no pass
                                                                    {
                                                                        #region -GoDeconToolsControllerB- run part 2  to deisotope the the spine now that we have the correct XYData

                                                                        #region old code off
                                                                        //GC.Collect();
                                                                        //printMemory("before accelerate");

                                                                        //GoAccelerate accelerater = new GoAccelerate();

                                                                        //Run runGo2 = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, fileName);
                                                                        //              Run runGo2 = runGo;
                                                                        //             runGo2.CurrentScanSet = tempScanSet;
                                                                        //             runGo2.XYData.Xvalues = tempXvalues;
                                                                        //             runGo2.XYData.Yvalues = tempYValues;

                                                                        //GC.Collect();
                                                                        //printMemory("inside using");
                                                                        //GoAccelerate accelerater = new GoAccelerate();
                                                                        //int localProcessors = 1;
                                                                        //accelerater.SetValues(runGo, resultPeak, 1, localProcessors, Parameters, fileName, resultPeak.ScanMaxIntensity);

                                                                        //accelerater.GO2DeconTools();//run part 2

                                                                        //printMemory("     Before FX");
                                                                        //printMemory("     Before FX");

                                                                        //GoDeconToolsControllerB newDeconToolsPart2 = new GoDeconToolsControllerB(runGo, insideLoopParameters);

                                                                        //newDeconToolsPart2.Dispose();
                                                                        //newDeconToolsPart2 = null;
                                                                        //printMemory("     Middle FX");\
                                                                        #endregion


                                                                        //newProfiler.printMemory("RUN DECONTOOLS FX (Create isoOutput)" + threadName);
                                                                        //lock (deconvolutionLock)//some times the memory goes up inside this lock
                                                                        if (5 == 5)
                                                                        {

                                                                            GoResults isotopeOutput = newDeconToolsPart2.GODeconToolsFX(runGo, controllerBParameters, transformer2);
                                                                            //GoResults isotopeOutput = newDeconToolsPart2.GODeconToolsFX(runGo, insideLoopParameters);
                                                                            //transformer2.PercentDone = 0;
                                                                            //transformer2.StatusMessage = "";


                                                                            newDeconToolsPart2.Dispose();
                                                                            newDeconToolsPart2 = null;
                                                                            //this part needs to go before isotope output can be deleted
                                                                            // newProfiler.printMemory("CanWeDeleteRun? " + threadName);
                                                                            runGo.Dispose();

                                                                            //newProfiler.printMemory("Run is disposed " + threadName);
                                                                            runGo = null;

                                                                            //Console.WriteLine("Run should be killed");
                                                                            //Console.ReadKey();
                                                                            //newProfiler.printMemory("Run is Null " + threadName);
                                                                            int y = 4; int hg = y * 7;


                                                                            #region old code off
                                                                            //newDeconToolsPart2.Dispose();


                                                                            //insideLoopParameters.Dispose();
                                                                            //insideLoopParameters = null;
                                                                            //int isotopesFound = isotopeOutput.IsosResultList.Count;

                                                                            //isotopeOutput.Dispose();



                                                                            //newDeconToolsPart2.Dispose();
                                                                            //a                          newDeconToolsPart2 = null;
                                                                            //resultPeak = accelerater.m_elutingPeakResults[0];
                                                                            //GoResults resultPeakIsos = new GoResults();
                                                                            //resultPeakIsos = accelerater.m_IsotopeResults;
                                                                            //GC.Collect();
                                                                            //printMemory("inside using after decontools");

                                                                            ////dispose in reverse order
                                                                            //runGo.CurrentScanSet = null;
                                                                            //runGo.Dispose();
                                                                            // accelerater.Dispose();


                                                                            ////                                   GC.Collect();
                                                                            ////                                   printMemory("after dispose");

                                                                            ////                                  GC.Collect();
                                                                            ////                                  printMemory("after");

                                                                            ////         tempXvalues = null;
                                                                            ////        tempYValues = null;
                                                                            ////         tempScanSet = null;
                                                                            ////runGo.Dispose();
                                                                            ////newDeconToolsPart1.Dispose();
                                                                            ////value
                                                                            ////ePeak.IsosResultList = null;

                                                                            //GC.Collect();
                                                                            //printMemory("baseline");

                                                                            #endregion

                                                                            if (2 == 2)
                                                                            {
                                                                                #region inside
                                                                                IsotopeObject isotopeStorage = new IsotopeObject();
                                                                                ////organzeResults.AssignCrossCorrelatedInformation(ePeak, Parameters);
                                                                                organzeResults.AssignCrossCorrelatedInformation(resultPeak, isotopeOutput, isotopeStorage, controllerBParameters);


                                                                                //isotopeOutput.Dispose();
                                                                                ////if (ePeak.ID == 1)//write to disk
                                                                                if (resultPeak.ID == 1)//write to disk
                                                                                {

                                                                                    //                            discoveredPeaks[e].ID = 1;
                                                                                    lock (transformer2.DatabaseLock)//some times the memory goes up inside this lock
                                                                                    {
                                                                                        #region inside

                                                                                        bool isSuccessfull = false;

                                                                                        numberOfHits++;
                                                                                        //printMemory("baseline before write");

                                                                                        //using (DatabaseAdaptor newDataBaseAdaptor = new DatabaseAdaptor(SQLFileName))
                                                                                        using (DatabaseAdaptor newDataBaseAdaptor = new DatabaseAdaptor(fileIn.OutputSQLFileName))
                                                                                        //using (DatabaseAdaptor newDataBaseAdaptor = new DatabaseAdaptor(localSQLFileName))
                                                                                        {

                                                                                            //isSuccessfull = newDataBaseAdaptor.WriteElutingPeakToSQLite(ePeak);
                                                                                            isSuccessfull = newDataBaseAdaptor.WriteElutingPeakToSQLite_Old(resultPeak);
                                                                                            isSuccessfull = newDataBaseAdaptor.WriteIsotopeStorageToSQLite_Old(isotopeStorage);
                                                                                            if (isSuccessfull)
                                                                                            {
                                                                                                Console.WriteLine("++Data Stored-Mass: " + resultPeak.Mass + " Charge: "+ resultPeak.ChargeState);
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                Console.WriteLine("Data Not Stored");
                                                                                                transformer2.ErrorLog.Add("data not stored, EluringPeakFinderPart2");
                                                                                            }

                                                                                            //Profiler memoryCheck = new Profiler();
                                                                                            //memoryCheck.printMemory("baseline during write");

                                                                                            //ePeak = null;
                                                                                            //resultPeak = null;
                                                                                        }
                                                                                        #endregion
                                                                                    }
                                                                                    //printMemory("baseline after write");


                                                                                }
                                                                                else
                                                                                {
                                                                                    //this is ok because not all peaks should not be writen, only the good ones
                                                                                    //transformer2.ErrorLog.Add("resultpeakID is not 1, ElutingPeakPart2");
                                                                                }

                                                                                isotopeStorage.Dispose();

                                                                                #endregion
                                                                            }



                                                                            //Console.WriteLine("_");    
                                                                            //newProfiler.printMemory("Can we dispose isotopeOut? " + threadName);
                                                                            isotopeOutput.Dispose();
                                                                            //newProfiler.printMemory("Can we null isotopeOut " + threadName);
                                                                            isotopeOutput = null;
                                                                            //newProfiler.printMemory("isotopeOut disposed " + threadName);

                                                                        }
                                                                        #endregion
                                                                    }
                                                                    //newProfiler.printMemory("Can we delete deconTools Part 1? " + threadName);
                                                                    newDeconToolsPart1 = null;
                                                                    //newProfiler.printMemory("newDeconToolsPart1 is null " + threadName);
                                                                    //insideLoopParameters.Dispose();
                                                                    //newProfiler.printMemory("loop parameters is disposed " + threadName);
                                                                    //newProfiler.printMemory("can we get rid of resutPeal? " + threadName);

                                                                    resultPeak.PeakList[0].MSPeak.Clear();
                                                                    resultPeak.PeakList.Clear();
                                                                    resultPeak.PeakList = null;
                                                                    resultPeak = null;
                                                                    //newProfiler.printMemory("post resultPeak " + threadName);
                                                                    //ElutingPeakOmics resultPeak2 = ObjectCopier.Clone<ElutingPeakOmics>(resultPeak);
                                                                    //newProfiler.printMemory("post resultPeak " + threadName);
                                                                    #endregion

                                                                }

                                                                //clean up run
                                                                //newProfiler.printMemory("CanWeDeleteRun? " + threadName);
                                                                //runGo.Dispose();
                                                                //newProfiler.printMemory("Run is disposed " + threadName);
                                                                //runGo = null;
                                                                //newProfiler.printMemory("Run is Null " + threadName);
                                                                #endregion
                                                            }
                                                            //runGo.Dispose();//kill root
                                                            //newDeconToolsPart1.Dispose();//kill GoDeconToolsControllerA
                                                            //a                         resultPeak.Dispose();
                                                            //Console.WriteLine("after run is created");
                                                            //Console.ReadKey();
                                                            //printMemory("NextElutingpPeak");

                                                            //Console.ReadKey();

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
                                                            transformer2.ErrorLog.Add("epeak ID is not 3, ElutingPeakFinderPart2");
                                                            Console.WriteLine("ePeak.ID == 3, ElutingPeakFinderController");
                                                            Console.ReadKey();
                                                            break;//this is to test the code
                                                        }
                                                    }
                                                    else
                                                    {
                                                        transformer2.ErrorLog.Add("the range is larger than the allowed range set for an eluting peak, ElutingpEakFinder2");
                                                    }
                                                }
                                                else
                                                {
                                                    transformer2.ErrorLog.Add("peak is below limit of intensity, ElutingPeakFinderPart2");
                                                }

                                                //ElutingPeak ePeak2 = ePeak;
                                                //SimplifyElutingPeak(ref ePeak2);
                                                //ClearElutingPeak(ref ePeak2);
                                                //ePeak.Dispose();
                                                //discoveredPeaks[e].Dispose();
                                                //printMemory("after clear peak list");
                                                int k = 5;
                                                k++;

                                                #endregion
                                            }
                                        }//end for eluting peaks

                                        organzeResults = null;
                                        //newProfiler.printMemory("post organize results " + threadName);
                                        #endregion
                                    }
                                    //newDeconToolsPart2.Parameters.Dispose();
                                    //newDeconToolsPart2.Parameters = null;
                                    //newDeconToolsPart2.Dispose();
                                    //newProfiler.printMemory("-after controllerB " + threadName);
                                    //newProfiler.printMemory("-after controllerB " + threadName);
                                    int kh = 3; int ge = kh * 4;
                                }
                                #endregion
                            }
                            #endregion
                        }
                        #endregion
                    }
                    newDataBaseLayer = null;

                    #endregion
                }
            //}//end lock

            //Profiler newProfiler = new Profiler();
            //newProfiler.printMemory("All done!");

        }

        ///// <summary>
        ///// Takes a run and extracts out the XYData
        ///// </summary>
        ///// <param name="runGo">Run</param>
        ///// <param name="newData">Omics List of XYData</param>
        //public static void RunXYDataToOmicsXYData(Run runGo, out List<PNNLOmics.Data.XYData> newData)
        //{
        //    List<double> tempXvalues = new List<double>();
        //    List<double> tempYValues = new List<double>();
        //    tempXvalues = runGo.ResultCollection.Run.XYData.Xvalues.ToList();
        //    tempYValues = runGo.ResultCollection.Run.XYData.Yvalues.ToList();

        //    newData = new List<PNNLOmics.Data.XYData>();
        //    for (int i = 0; i < runGo.ResultCollection.Run.XYData.Xvalues.Length; i++)
        //    {
        //        PNNLOmics.Data.XYData newPoint = new PNNLOmics.Data.XYData((float)tempXvalues[i], (float)tempYValues[i]);
        //        newData.Add(newPoint);
        //    }
        //}


        #region IDisposable Members

        public void Dispose()
        {
            //this.msgenFactory = null;
            //this.msGenerator = null;
            //this.Parameters.Dispose();
            this.msPeakDetector = null;
            //this.databaseLock = null;
        }

        #endregion
    }
}
