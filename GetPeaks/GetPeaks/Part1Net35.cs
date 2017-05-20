using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL;
using GetPeaks_DLL.Objects;
using System.IO;
using PNNLOmics.Algorithms.PeakDetection;
using DeconToolsPart2;
using DeconTools.Backend.Runs;
using DeconTools.Backend.Core;
using MemoryOverloadProfilierX86;
using GetPeaks_DLL.ConosleUtilities;

namespace GetPeaks
{
    public class Part1Net35
    {
        public List<ElutingPeakOmics> RunMe(string[] args, out InputOutputFileName newFile, out SimpleWorkflowParameters parameters)
        {
            List<ElutingPeakOmics> discoveredOmicsPeaks = new List<ElutingPeakOmics>();
            
            FileTarget target = new FileTarget();
            target = FileTarget.WorkStandardTest;//<---used to target what computer we are working on:  Home or Work.  Server is covered by command line

            if (5 == 5)
            {

                #region real code

                //setup main parameter file from args
                List<string> mainParameterFile = new List<string>();
                #region switch from server to desktop based on number of args
                if (args.Length == 0)//debugging
                {
                    mainParameterFile.Add(""); mainParameterFile.Add(""); mainParameterFile.Add("");
                }
                else
                {
                    Console.WriteLine("ParseArgs");
                    ParseStrings parser = new ParseStrings();
                    mainParameterFile = parser.ParseArgs(args);
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
                List<string> stringListFromParameterFile = new List<string>();
                FileIteratorStringOnly loadParameter = new FileIteratorStringOnly();
                stringListFromParameterFile = loadParameter.loadStrings(mainParameterFile[0]);

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

                //part1SN = 1;
                DeconvolutionType loadedDeconvolutionType = new DeconvolutionType();
                ConvertAToB converter = new ConvertAToB();
                loadedDeconvolutionType = converter.stringTODeconvolutionType(DeconType);

                MemorySplitObject newMemorySplitter = new MemorySplitObject();
                newMemorySplitter.NumberOfBlocks = serverBlockTotal;
                newMemorySplitter.BlockNumber = serverBlock;
                #endregion

                //setup files to load and save
                //InputOutputFileName newFile = new InputOutputFileName();
                newFile = new InputOutputFileName();
                newFile.InputFileName = serverDataFileName;
                newFile.OutputFileName = mainParameterFile[1] + folderID + "_" + serverBlock + @".txt";
                newFile.OutputSQLFileName = mainParameterFile[2] + folderID + @".db";

                //Console.WriteLine("InputFileName: " + newFile.InputFileName +"\n");
                Console.WriteLine("SQLite Output: " + newFile.OutputSQLFileName + "\n");

                Console.WriteLine("Startscan: " + startScan + " EndScan: " + endScan);
                Console.WriteLine("Part1 SN: " + part1SN + " Part2 SN: " + part2SN);
                Console.WriteLine("DataSpecificMassNeutron: " + DataSpecificMassNeutron);
                Console.WriteLine("Processing Block# " + (newMemorySplitter.BlockNumber + 1).ToString() + " of " + newMemorySplitter.NumberOfBlocks);
                Console.WriteLine("Press Enter");
                //Console.ReadKey();


                FileInfo fi = new FileInfo(newFile.InputFileName);
                bool exists = fi.Exists;

                Console.WriteLine("CreateRun: " + newFile.InputFileName + " and its existance is " + exists.ToString());
                RunFactory rf = new RunFactory();
                Console.WriteLine("RunFactory Setup, press enter to continue");
                //Console.ReadKey();

                Run run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, newFile.InputFileName);
                //Run run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.Finnigan, inputDataFilename);

                #region Setup Parameters for complete data set to find eluting peaks

                Console.WriteLine("CreateController");
                //SimpleWorkflowParameters parameters = new SimpleWorkflowParameters();
                parameters = new SimpleWorkflowParameters();

                //part 1 peak detector decontools only.  this gets overwritten below
                parameters.Part1Parameters.MSPeakDetectorPeakBR = 1.3;
                parameters.Part1Parameters.ElutingPeakNoiseThreshold = 2;
                parameters.Part2Parameters.MSPeakDetectorPeakBR = 1.3;
                parameters.Part2Parameters.MSPeakDetectorSigNoise = 2;

                ElutingPeakFinderController controller = new ElutingPeakFinderController(run, parameters);
                controller.Parameters.FileNameUsed = newFile.InputFileName;

                //set here for omics typical run is 3
                //controller.Parameters.Part1Parameters.ElutingPeakNoiseThreshold = 3;//data 1 when NoiseRemoved, take 3 sigma off before the orbitrap filter
                controller.Parameters.Part1Parameters.ElutingPeakNoiseThreshold = part1SN;//when NoiseRemoved, take 3 sigma off before the orbitrap filter
                controller.Parameters.Part1Parameters.NoiseType = InstrumentDataNoiseType.NoiseRemoved;
                controller.Parameters.Part1Parameters.PeakFitType = DeconTools.Backend.Globals.PeakFitType.LORENTZIAN;//this makes a large difference in the number of peaks
                controller.Parameters.Part1Parameters.StartScan = startScan;
                controller.Parameters.Part1Parameters.StopScan = endScan;
                controller.Parameters.Part1Parameters.MaxHeightForNewPeak = 0.75;
                controller.Parameters.Part1Parameters.AllignmentToleranceInPPM = 15;
                controller.Parameters.Part1Parameters.ParametersOrbitrap.massNeutron = DataSpecificMassNeutron;
                controller.Parameters.Part1Parameters.ParametersOrbitrap.DeltaMassTollerancePPM = 6000;

                //set here for omics  typical run is 10.5
                //controller.Parameters.Part2Parameters.MSPeakDetectorSigNoise = 10.5;//run 1
                controller.Parameters.Part2Parameters.MSPeakDetectorSigNoise = part2SN;//run 1
                controller.Parameters.Part2Parameters.NoiseType = InstrumentDataNoiseType.Standard;
                controller.Parameters.Part2Parameters.PeakFitType = DeconTools.Backend.Globals.PeakFitType.LORENTZIAN;//this makes a large difference in the number of peaks
                controller.Parameters.Part2Parameters.DeconvolutionType = loadedDeconvolutionType;
                controller.Parameters.Part2Parameters.Multithread = false;
                controller.Parameters.Part2Parameters.DynamicRangeToOne = 300000;
                controller.Parameters.Part2Parameters.MaxScanSpread = 500;
                controller.Parameters.Part2Parameters.ParametersOrbitrap.massNeutron = DataSpecificMassNeutron;
                controller.Parameters.Part2Parameters.ParametersOrbitrap.DeltaMassTollerancePPM = 6000;
                controller.Parameters.Part2Parameters.MemoryDivider = newMemorySplitter;

                //controller.Parameters.Part2Parameters.DeconvolutionType = DeconvolutionType.Thrash; 

                parameters.ConsistancyCrossErrorPPM = 20;
                #endregion

                #region Run Program

                //finds eluting peaks and calculates the start and stop scans
                //controller.SimpleWorkflowExecutePart1(run);

                List<ElutingPeak> discoveredPeaks = controller.SimpleWorkflowExecutePart1(run);
                //sums the spectra and deisotopes along the spine
                // controller.SimpleWorkflowExecutePart2(run, newFile.InputFileName, newFile.OutputSQLFileName);
       //         SimplifiedRun SKRun = new SimplifiedRun();
       //         SKRun.CurrentScanSet = run.CurrentScanSet;

                //current
                //discoveredPeaks = discoveredPeaks.OrderBy(p => p.Mass).ToList();
                //controller.SimpleWorkflowExecutePart2b(discoveredPeaks, newFile.InputFileName, newFile.OutputSQLFileName);

                //Profiler newProfiler = new Profiler();
                //newProfiler.printMemory("StartOfApplication");

                ConvertToOmics newConverter = new ConvertToOmics();
                discoveredOmicsPeaks = newConverter.ConvertElutingPeakToElutingPeakOmics(discoveredPeaks);
          //      discoveredPeaks = null;

                

                //newProfiler.printMemory("After Copy");
                
                //TODO 1.convert ElutingPeaks into ElutingPeaksOmics so we can return a Omics list of ELutingPeaks

                //TODO 2.  start new 3.5 function that accepts parameters, 1 eluting peak, and file names



                if (1 == 1)
                {
                    //int elutingpeakHits = GoPart2Decon(newFile, controller.Parameters, discoveredPeaks);
                    //Part2Net35 newPart2 = new Part2Net35();

                   // int elutingpeakHits = Part2Net35.GoPart2Omics(newFile, controller.Parameters, discoveredOmicsPeaks);

                    //TODO end 2

                    //TODO 4.0  Check memory issues

                    #region method C
                    //discoveredPeaks = discoveredPeaks.OrderBy(p => p.Mass).ToList();

                    //int inr = 0;
                    //foreach (ElutingPeak ePeak in discoveredPeaks)
                    //{
                    //    Console.WriteLine("V2 eluting Peak " + (inr).ToString() + " out of " + discoveredPeaks.Count.ToString());

                    //    RunFactory rf2 = new RunFactory();
                    //    Run run2 = rf2.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, newFile.InputFileName);
                    //    using (ElutingPeakFinderController controller2 = new ElutingPeakFinderController(run2, parameters))
                    //    {
                    //        controller2.Parameters.FileNameUsed = newFile.InputFileName;
                    //        controller2.Parameters.Part2Parameters.MSPeakDetectorSigNoise = part2SN;//run 1
                    //        controller2.Parameters.Part2Parameters.NoiseType = InstrumentDataNoiseType.Standard;
                    //        controller2.Parameters.Part2Parameters.PeakFitType = DeconTools.Backend.Globals.PeakFitType.LORENTZIAN;//this makes a large difference in the number of peaks
                    //        controller2.Parameters.Part2Parameters.DeconvolutionType = loadedDeconvolutionType;
                    //        controller2.Parameters.Part2Parameters.Multithread = false;
                    //        controller2.Parameters.Part2Parameters.DynamicRangeToOne = 300000;
                    //        controller2.Parameters.Part2Parameters.MaxScanSpread = 500;
                    //        controller2.Parameters.Part2Parameters.ParametersOrbitrap.massNeutron = DataSpecificMassNeutron;
                    //        controller2.Parameters.Part2Parameters.ParametersOrbitrap.DeltaMassTollerancePPM = 6000;
                    //        controller2.Parameters.Part2Parameters.MemoryDivider = newMemorySplitter;

                    //        controller2.SimpleWorkflowExecutePart2c(ePeak, newFile.InputFileName, newFile.OutputSQLFileName);
                    //        controller2.Dispose();
                    //    }

                    //    inr++;
                    //}
                    #endregion

                #endregion

                    //timesitCleanedUp = GC.CollectionCount(0);
                    //Console.WriteLine(timesitCleanedUp);

                    #region Summarize test results
                    //          CrossCorrelateResults organzeResults = new CrossCorrelateResults();
                    //          organzeResults.summarize(run.ResultCollection.ElutingPeakCollection, newFile.OutputFileName, parameters.ConsistancyCrossErrorPPM, parameters);
                    #endregion

                #endregion

                   // Console.WriteLine(elutingpeakHits + " hits were written to the database");
                }
            }//end big iff

            return discoveredOmicsPeaks;
        }

       
    }
}
