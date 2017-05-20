using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.SQLite;
using DeconTools.Backend.Core;
using GetPeaks_DLL;
using DeconTools.Backend.Runs;
using GetPeaks_DLL.Go_Decon_Modules;
using DeconTools.Backend;
using MemoryOverloadProfilierX86;

namespace MSMS_DLL
{
    public class ApplyThrash
    {
        public ApplyThrash()
        {
            
        }

        private Object databaseLock = new Object();

        public void SimpleApplyThrash(ref TandemObject TandemScan, SimpleWorkflowParameters Parameters, TransformerObject transformer2, string fileName)//Analyze Eluting Peaks to find Monoisotopic peaks vis decon tools
        
        {
            lock (databaseLock)//some times the memory goes up inside this lock
            {
                int threadName = System.Threading.Thread.CurrentThread.ManagedThreadId;
                if (1 == 1)
                {
                    #region inside
                    DatabaseLayer newDataBaseLayer = new DatabaseLayer();
                    bool didThisWork = false;

                    if (3 == 3)
                    {
                        #region inside
                        #region setup

                        int peakCount = 0;
                        //int totalPeakCount = discoveredPeaks.Count;

                        List<Run> listofRuns = new List<Run>();

                       
                        #endregion
                        //sort Eluting peaks so we hit the most intense first
                        //List<ElutingPeak> tempList = discoveredPeaks;
                        //discoveredPeaks = discoveredPeaks.OrderByDescending(p => p.Intensity).ToList();

                        //int limitThreadCount = discoveredPeaks.Count;
                        //limitThreadCount = 10;
                        //float limitIntensity = (float)(discoveredPeaks[0].Intensity / Parameters.Part2Parameters.DynamicRangeToOne);

                        //int numberOfElements = discoveredPeaks.Count;
                        int numberOfChunks = Parameters.Part2Parameters.MemoryDivider.NumberOfBlocks;
                        int rank = Parameters.Part2Parameters.MemoryDivider.BlockNumber;
                        int startIndex = 0;
                        int stopIndex = 0;

                        ProcessDivision newProcesDivider = new ProcessDivision();
                        //newProcesDivider.CalculateSplits(numberOfElements, numberOfChunks, rank, ref startIndex, ref stopIndex);

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

                                if (1 == 1)//passes memory 6-23-11
                                {
                                    //Profiler newProfiler = new Profiler();

                                    //newProfiler.printMemory("++pre controllerB " + threadName);
                                    //newProfiler.printMemory("++pre controllerB " + threadName);
                                    //TODO dispose GoDeconToolsControllerB

                                    SimpleWorkflowParameters controllerBParameters = ObjectCopier.Clone<SimpleWorkflowParameters>(Parameters);
                                    GoDeconToolsControllerB newDeconToolsPart2 = new GoDeconToolsControllerB(controllerBParameters, DeconTools.Backend.Globals.MSFileType.Undefined);


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
                                                //ElutingPeakOmics ePeak = discoveredPeaks[e];
                                                

                                                #region inside
                                                //Console.WriteLine(" ");
                                                //newProfiler.printMemory("Pre Run creation " + threadName);
                                                //Console.WriteLine("before run is created");
                                                //Console.ReadKey();
                                                Run runGo = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, fileName);

                                                peakCount++;
                                                //create scansets

                                                if (1 == 1)//passed memory 3-21-11
                                                {
                                                    #region inside
                                                    #region -GoDeconToolsControllerA- get the data from the disk an sum it
                                                    //Stopwatch stopWatch = new Stopwatch();
                                                    //stopWatch.Start();

                                                                    

                                                    //    ePeak.Dispose();

                                                     //   newDeconToolsPart1 = new GoDeconToolsControllerA(runGo, controllerBParameters);
                                                    //newProfiler.printMemory("pre loop parametersX " + threadName);
                                                    //try//this is needed incase the scan information is not cashed.  It should be in this workflow and thus fast
                                                    //{
                                                    //    ElutingPeakScanSet getScanSet = new ElutingPeakScanSet();
                                                    //    //runGo.CurrentScanSet = getScanSet.CreateScanSetFromElutingPeakMax(runGo, resultPeak.ScanMaxIntensity, resultPeak.ScanStart, resultPeak.ScanEnd);
                                                    //    getScanSet.CreateScanSetFromElutingPeakMax(runGo, resultPeak.ScanMaxIntensity, resultPeak.ScanStart, resultPeak.ScanEnd);
                                                    //    runGo.CurrentScanSet = getScanSet.scanSet;
                                                    //    //getScanSet.Dispose();
                                                    //    //runGo.CurrentScanSet = getScanSet.CreateScanSetFromElutingPeakMax(runGo, ePeak.ScanMaxIntensity, ePeak.ScanStart, ePeak.ScanEnd);

                                                    //}
                                                    //catch (Exception ex)
                                                    //{
                                                    //    throw ex;
                                                    //}
                                                    if (5 == 5)
                                                    {
                                                        runGo.XYData = TandemScan.SpectraDataDECON;
                                                        //newDeconToolsPart1.GoLoadDataAndSumIt(runGo);
                                                    }

                                                    #endregion

                                                    if (2 == 2)//no pass
                                                    {
                                                        #region -GoDeconToolsControllerB- run part 2  to deisotope the the spine now that we have the correct XYData

                                                        //newProfiler.printMemory("RUN DECONTOOLS FX (Create isoOutput)" + threadName);
                                                        if (5 == 5)
                                                        {
                                                            //TODO why can't we get results out!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                                            
                                                            GoResults isotopeOutput = newDeconToolsPart2.GODeconToolsFX(runGo, controllerBParameters, transformer2);
                                                            //GoResults isotopeOutput = newDeconToolsPart2.GODeconToolsFX(runGo, insideLoopParameters);
                                                            //transformer2.PercentDone = 0;
                                                            //transformer2.StatusMessage = "";

                                                            //we need a stronger copy here

                                                            TandemScan.PeaksIsosResultsDECON = IsosResutlsCopier(isotopeOutput);
                                                            
                                                            //TandemScan.PeaksIsosResultsDECON = isotopeOutput.IsosResultList;


                                                            newDeconToolsPart2.Dispose();
                                                            newDeconToolsPart2 = null;
                                                            //this part needs to go before isotope output can be deleted
                                                            //newProfiler.printMemory("CanWeDeleteRun? " + threadName);
                                                            runGo.Dispose();

                                                            //newProfiler.printMemory("Run is disposed " + threadName);
                                                            runGo = null;

                                                            //Console.WriteLine("Run should be killed");
                                                            //Console.ReadKey();
                                                            //newProfiler.printMemory("Run is Null " + threadName);
                                                            int y = 4; int hg = y * 7;



                                                            //Console.WriteLine("_");    
                                                            //newProfiler.printMemory("Can we dispose isotopeOut? " + threadName);
                                                            isotopeOutput.Dispose();
                                                           // newProfiler.printMemory("Can we null isotopeOut " + threadName);
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

                                                    //resultPeak.PeakList[0].MSPeak.Clear();
                                                    //resultPeak.PeakList.Clear();
                                                    //resultPeak.PeakList = null;
                                                    //resultPeak = null;
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
            }//end lock

            //Profiler newProfiler = new Profiler();
            //newProfiler.printMemory("All done!");

        }

        private static List<StandardIsosResult> IsosResutlsCopier(GoResults isotopeOutput)
        {
            List<StandardIsosResult> IsosResultsBin = new List<StandardIsosResult>();
            foreach (DeconTools.Backend.StandardIsosResult isos in isotopeOutput.IsosResultList)
            {
                StandardIsosResult newIsos = new StandardIsosResult();
                IsotopicProfile newIsotopicProfilePeakList = ObjectCopier.Clone<IsotopicProfile>(isos.IsotopicProfile);
                newIsos.IsotopicProfile = newIsotopicProfilePeakList;
                
                IsosResultsBin.Add(newIsos);

                isos.IsotopicProfile.Peaklist.Clear();
                //isos.IsotopicProfile.Peaklist = null;
                //isos.IsotopicProfile = null;

                
            }

            return IsosResultsBin;
        }
    }
}
