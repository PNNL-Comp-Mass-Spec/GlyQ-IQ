using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GetPeaks_DLL;
using DeconTools.Backend.Core;
using DeconTools.Backend.Runs;
using DeconTools.Backend.Utilities;
using DeconTools.Backend.ProcessingTasks;
using GetPeaks_DLL.PNNLOmics_Modules;
using PNNLOmics.Data;
using PNNLOmics.Algorithms.PeakDetection;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.Go_Decon_Modules;
using GetPeaks_DLL.Summing;
using System.Diagnostics;
using GetPeaks_DLL.ConosleUtilities;
using System.IO;
using GetPeaks_DLL.Common_Switches;
using DeconTools.Backend.DTO;
using GetPeaks_DLL.CompareContrast;
using GetPeaks_DLL.DeconToolsPart2;
using GetPeaks_DLL.DataFIFO;

namespace GetPeaks.UnitTests
{
    public class FindPeaksTest
    {
        /// <summary>
        /// Tests synthetic YAFMS data file, 15 scans
        /// </summary>
        [Test]
        public void getElutingPeaks()
        {
            Console.WriteLine("Find Peaks");

            string inputDataFilename = @"d:\Csharp\Syn Output\14 scan test file for get peaks\from external\yafmsOut.yafms";
            double DataSpecificMassNeutron = 1.0013;//standard synthetic

            SimpleWorkflowParameters parameters = new SimpleWorkflowParameters();

            InputOutputFileName newFile = new InputOutputFileName();
            newFile.InputFileName = @"d:\Csharp\Syn Output\14 scan test file for get peaks\from external\yafmsOut.yafms";
            

            RunFactory rf = new RunFactory();
            Run run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, inputDataFilename);

            Console.WriteLine("CreateController");
            ElutingPeakFinderController controller = new ElutingPeakFinderController(run, parameters);
            controller.Parameters.FileNameUsed = newFile.InputFileName;

            controller.Parameters.Part1Parameters.NoiseType = InstrumentDataNoiseType.NoiseRemoved;
            //controller.Parameters.Part1Parameters.ElutingPeakNoiseThreshold = 11.6;
            controller.Parameters.Part1Parameters.ElutingPeakNoiseThreshold = 3;
            controller.Parameters.Part1Parameters.PeakFitType = DeconTools.Backend.Globals.PeakFitType.LORENTZIAN;//this makes a large difference in the number of peaks
            controller.Parameters.Part1Parameters.StartScan = 0;
            controller.Parameters.Part1Parameters.StopScan = 15;
            controller.Parameters.Part1Parameters.MaxHeightForNewPeak = 0.75;
            controller.Parameters.Part1Parameters.AllignmentToleranceInPPM = 15;
            controller.Parameters.Part1Parameters.ParametersOrbitrap.massNeutron = DataSpecificMassNeutron;
            controller.Parameters.Part1Parameters.ParametersOrbitrap.DeltaMassTollerancePPM = 6000;

            controller.Parameters.Part2Parameters.NoiseType = InstrumentDataNoiseType.Standard;
            //controller.Parameters.Part2Parameters.MSPeakDetectorSigNoise = 6.5;
            controller.Parameters.Part2Parameters.MSPeakDetectorSigNoise = 10.5;
            controller.Parameters.Part2Parameters.PeakFitType = DeconTools.Backend.Globals.PeakFitType.LORENTZIAN;//this makes a large difference in the number of peaks
            controller.Parameters.Part2Parameters.DeconvolutionType = DeconvolutionType.Rapid;
            controller.Parameters.Part2Parameters.Multithread = false;
            controller.Parameters.Part2Parameters.DynamicRangeToOne = 300000;
            controller.Parameters.Part2Parameters.MaxScanSpread = 500;
            controller.Parameters.Part2Parameters.ParametersOrbitrap.massNeutron = DataSpecificMassNeutron;
            controller.Parameters.Part2Parameters.ParametersOrbitrap.DeltaMassTollerancePPM = 6000;

            //parameters.ConsistancyCrossErrorPPM = 0.05;
            parameters.ConsistancyCrossErrorPPM = 15;
            
            //TODO update parameters so we can set the orbitrap from the outside
            //ParametersOrbitrap.massNeutron = 1.0013
            //finds eluting peaks and calculates the start and stop scans
            controller.SimpleWorkflowExecutePart1(run);

            Console.Write("Finished.  Press Return to Exit");

            int elutingPeaksFound = run.ResultCollection.ElutingPeakCollection.Count;

            for (int i = 0; i < run.ResultCollection.ElutingPeakCollection.Count; i++)
            {
                //Console.WriteLine(run.ResultCollection.ElutingPeakCollection[i].PeakList[0].MSPeak.XValue);
            }

            //Assert.AreEqual(2138, elutingPeaksFound);//quadratic
            Assert.AreEqual(102, elutingPeaksFound);//lorentzian
        }

        /// <summary>
        /// Tests gordon's data all the way to features YAFMS
        /// </summary>
        [Test]
        public void AllTheWayToFeaturesYAFMS()
        {

            const int STARTSCAN = 5500;
            const int ENDSCAN = 5550;
            //const int ENDSCAN = 6500;
            
            Console.WriteLine("Find Peaks");

            //string inputDataFilename = @"d:\Csharp\YAFMS\20100722_glycan_SN24_NF.yafms";
            //string inputDataFilename = @"g:\PNNL Files\Csharp\YAFMS\20100722_glycan_SN24_NF.yafms";

            //string inputDataFilename = @"d:\Csharp\Syn Output\14 scan test file for get peaks\from external\yafmsOut.yafms";
            string inputDataFilename = @"d:\Csharp\YAFMS\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.yafms";
            //double DataSpecificMassNeutron = 1.0013;//standard synthetic
            double DataSpecificMassNeutron = 1.002149286;//SN09


            MemorySplitObject newMemorySplitter = new MemorySplitObject();
                newMemorySplitter.NumberOfBlocks = 1;
                newMemorySplitter.BlockNumber = 0;

            //string inputDataFilename = @"g:\PNNL Files\Csharp\YAFMS\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.yafms";
            SimpleWorkflowParameters parameters = new SimpleWorkflowParameters();

            InputOutputFileName newFile = new InputOutputFileName();
            newFile.InputFileName = @"d:\Csharp\YAFMS\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.yafms";
            newFile.OutputSQLFileName = @"d:\Csharp\YAFMS\FirstSQLite.db";
            RunFactory rf = new RunFactory();
            Run run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, inputDataFilename);

            Console.WriteLine("CreateController");
            ElutingPeakFinderController controller = new ElutingPeakFinderController(run, parameters);
            controller.Parameters.FileNameUsed = newFile.InputFileName;

            controller.Parameters.Part1Parameters.ElutingPeakNoiseThreshold = 6;
            controller.Parameters.Part1Parameters.NoiseType = InstrumentDataNoiseType.NoiseRemoved;
            controller.Parameters.Part1Parameters.isDataAlreadyThresholded = false;//true for orbitrap    
            controller.Parameters.Part1Parameters.PeakFitType = DeconTools.Backend.Globals.PeakFitType.LORENTZIAN;//this makes a large difference in the number of peaks
            controller.Parameters.Part1Parameters.StartScan = STARTSCAN;
            controller.Parameters.Part1Parameters.StopScan = ENDSCAN;
            controller.Parameters.Part1Parameters.MaxHeightForNewPeak = 0.75;
            controller.Parameters.Part1Parameters.AllignmentToleranceInPPM = 15;
            controller.Parameters.Part1Parameters.ParametersOrbitrap.massNeutron = DataSpecificMassNeutron;
            controller.Parameters.Part1Parameters.ParametersOrbitrap.DeltaMassTollerancePPM = 6000;
            controller.Parameters.Part1Parameters.ScansToBeSummed = 3;//default is 3
            controller.Parameters.Part1Parameters.ParametersOrbitrap.withLowMassesAllowed = true;
            controller.Parameters.Part1Parameters.ParametersOrbitrap.ExtraSigmaFactor = 1.5;//1=default

            controller.Parameters.Part2Parameters.MSPeakDetectorSigNoise = 3;//run 1
            controller.Parameters.Part2Parameters.NoiseType = InstrumentDataNoiseType.Standard;
            //controller.Parameters.Part2Parameters.MSPeakDetectorSigNoise = 6.5;
            controller.Parameters.Part2Parameters.MSPeakDetectorSigNoise = 10.5;
            controller.Parameters.Part2Parameters.PeakFitType = DeconTools.Backend.Globals.PeakFitType.LORENTZIAN;//this makes a large difference in the number of peaks
            controller.Parameters.Part2Parameters.DeconvolutionType = DeconvolutionType.Thrash;
            controller.Parameters.Part2Parameters.Multithread = false;
            controller.Parameters.Part2Parameters.DynamicRangeToOne = 300000;
            controller.Parameters.Part2Parameters.MaxScanSpread = 500;
            controller.Parameters.Part2Parameters.ParametersOrbitrap.massNeutron = DataSpecificMassNeutron;
            controller.Parameters.Part2Parameters.ParametersOrbitrap.DeltaMassTollerancePPM = 6000;
            controller.Parameters.Part2Parameters.MemoryDivider = newMemorySplitter;
            controller.Parameters.Part2Parameters.numberOfDeconvolutionThreads = 1;//numberOfDeconvolutionThreads;
            controller.Parameters.Part2Parameters.ParametersOrbitrap.withLowMassesAllowed = true;
            controller.Parameters.Part2Parameters.ParametersOrbitrap.ExtraSigmaFactor = 1.5;//1=default

            parameters.ConsistancyCrossErrorPPM = 20;

            //parameters.MSPeakDetectorPeakBR = 1.3;//default
            //parameters.MSPeakDetectorSigNoise = 2;//default

            //parameters.MSPeakDetectorPeakBR = 1.3 * 30;
            //parameters.MSPeakDetectorSigNoise = 2 * 30;

           

            //ElutingPeakFinderController controller = new ElutingPeakFinderController(run, parameters);

            ElutingPeakFinderParametersPart1 part1Parameters = new ElutingPeakFinderParametersPart1();
            part1Parameters.NoiseType = InstrumentDataNoiseType.Standard;

            //finds eluting peaks and calculates the start and stop scans
            controller.SimpleWorkflowExecutePart1(run);

            //old
            //controller.SimpleWorkflowExecutePart2(run, newFile.InputFileName, newFile.InputSQLFileName);
            //old


            //new


            //addd code


            List<ElutingPeakOmics> discoveredOmicsPeaks = new List<ElutingPeakOmics>();

            ConvertToOmics newConverter = new ConvertToOmics();
            discoveredOmicsPeaks = newConverter.ConvertElutingPeakToElutingPeakOmics(run.ResultCollection.ElutingPeakCollection);
            //  end added code  


            Object databaseLockMulti = new Object();//global lock for database

            
            //int maxNumberOfThreadsToUse = 8;//1 is default
            int maxNumberOfThreadsToUse = parameters.Part2Parameters.numberOfDeconvolutionThreads;
            int sizeOfTransformerArmy = maxNumberOfThreadsToUse * 2+1;//+1 is for emergencies
            GoTransformParameters transformerParameterSetup = new GoTransformParameters();
            HornTransformParameters hornParameters = new HornTransformParameters();
            transformerParameterSetup.Parameters = hornParameters;

            GoTransformPrep setupTransformers = new GoTransformPrep();
            List<TransformerObject> transformerList = setupTransformers.PreparArmyOfTransformers(transformerParameterSetup, sizeOfTransformerArmy, newFile, databaseLockMulti);

            int hits = 0;
            int counter = 0;
            int totalCount = discoveredOmicsPeaks.Count;
            
            TransformerObject transformerMulti;// = transformer2;
            transformerMulti = transformerList[0];

            ElutingPeakFinderPart2 EPPart2 = new ElutingPeakFinderPart2();

            foreach (ElutingPeakOmics ePeak in discoveredOmicsPeaks)
            {
               
                int threadName = System.Threading.Thread.CurrentThread.ManagedThreadId;
                Console.WriteLine("newThread" + threadName + "Peak " + counter + " out of " + totalCount + " with " + hits + " hits");

                //Console.WriteLine("press enter"); //Console.ReadKey();
                //ElutingPeakFinderPart2 EPPart2 = new ElutingPeakFinderPart2();
                //SimpleWorkflowParameters newpar = ObjectCopier.Clone<SimpleWorkflowParameters>(Parameters);
                //ElutingPeakOmics ThreadEPeak = ObjectCopier.Clone<ElutingPeakOmics>(ePeak);
                SimpleWorkflowParameters paralellParameters = new SimpleWorkflowParameters();
                paralellParameters = GetPeaks_DLL.ObjectCopier.Clone<SimpleWorkflowParameters>(parameters);
                //Parameters.Dispose();
                //paralellParameters.Dispose();
                //EPPart2.Parameters = Parameters;
                //TODO 3.   Setup Parameters in GetPeaks 4.0
                //EPPart2.Parameters = Parameters;
                
                //System.Threading.Thread.
                //Profiler newProfiler = new Profiler();
                
                //newProfiler.printMemory("Start of thread " + threadName);

                List<ElutingPeakOmics> newSingleList = new List<ElutingPeakOmics>(0);
                newSingleList.Add(ePeak);
                int elutingpeakHits = 0;

                EPPart2.SimpleWorkflowExecutePart2e(newSingleList, newFile, paralellParameters, ref elutingpeakHits, transformerMulti);
                if(elutingpeakHits>0)
                {
                    hits++;
                }
                counter++;
            }

            //there are this many monoisotopes returned*
            Console.WriteLine(hits);




            //#region test results
            //CrossCorrelateResults organzeResults = new CrossCorrelateResults();
            //organzeResults.summarize(run.ResultCollection.ElutingPeakCollection, newFile.OutputFileName, parameters.ConsistancyCrossErrorPPM, parameters);
           // #endregion

            //List<ElutingPeak> synchronizedElutingPeaks = new List<ElutingPeak>();//elusting peaks that yielded a monoisotopic peak 
            //foreach (ElutingPeak ePeak in run.ResultCollection.ElutingPeakCollection)
            //{
            //    if (ePeak.ID == 1)
            //    {
            //        synchronizedElutingPeaks.Add(ePeak);
            //    }

            //}
            
            //int howManyFeatures = 0;//how many eluting peaks have a monoisotopci peak somewhere in the summed spectra
            ////int howManyMulipleHits = 0;//more than one monoisotopic ion in the summed spectra
            //int hundredCount = 0;//how many have a monoisotopic peak in the summed spectra and it is the correct one
            //int extrapeakcount = 0;//how many lc peaks are split
            //foreach (ElutingPeak ePeak in run.ResultCollection.ElutingPeakCollection)
            //{
            //    if (ePeak.IsosResultList.Count > 0)
            //    {
            //        howManyFeatures++;
            //    }
            //    //howManyMulipleHits += ePeak.IsosResultList.Count;

            //    if (ePeak.ID == 0)
            //    {
            //        synchronizedElutingPeaks.Add(ePeak);
            //        hundredCount++;//reasons that they are missed is that the eluting peak is an isotope and when summed and deisotoped, the mass is off and not zeroed
            //        //other reasons is that the mass is no where near a monoisotopic peak
            //    }
            //    if (ePeak.NumberOfPeaks > 1)
            //    {
            //        extrapeakcount++;
            //    }
            //}


            Console.Write("Finished.  Press Return to Exit");

            int peakFound = run.ResultCollection.ElutingPeakCollection.Count;

            for (int i = 0; i < run.ResultCollection.ElutingPeakCollection.Count; i++)
            {
                //Console.WriteLine(run.ResultCollection.ElutingPeakCollection[i].PeakList[0].MSPeak.XValue);
            }
            //Assert.AreEqual(2887, peakFound);//was 203 for smaller set or different threshold
            //Assert.AreEqual(821, synchronizedElutingPeaks.Count);//was 53

            Assert.AreEqual(400, peakFound);//was 203 for smaller set or different threshold
            Assert.AreEqual(33, hits);//was 53
        }

        /// <summary>
        /// Tests gordon's data all the way to features RAW
        /// </summary>
        [Test]
        public void AllTheWayToFeaturesRAW()
        {

            const int startScan = 5500;
            const int endScan = 5550;
            //const int ENDSCAN = 6500;

            Console.WriteLine("Find Peaks");

            //string inputDataFilename = @"d:\Csharp\YAFMS\20100722_glycan_SN24_NF.yafms";
            //string inputDataFilename = @"g:\PNNL Files\Csharp\YAFMS\20100722_glycan_SN24_NF.yafms";

            //string inputDataFilename = @"d:\Csharp\Syn Output\14 scan test file for get peaks\from external\yafmsOut.yafms";
            //string inputDataFilename = @"d:\Csharp\YAFMS\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.yafms";
            //string inputDataFilename = @"d:\PNNL Data\Gordon\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.raw";
            string inputDataFilename = @"E:\PNNL Data\2011_01_11 Gordon\2011_01_11 Standard QC Shew File\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.raw";
            
            //double DataSpecificMassNeutron = 1.0013;//standard synthetic
            double DataSpecificMassNeutron = 1.002149286;//SN09

            //Manual Variables
            double part1SN = 3;
            double part2SN = 3;
            string DeconType = "Thrash";
            int serverBlockTotal = 1;
            int serverBlock = 0;
            string serverDataFileName = inputDataFilename;
            List<string> mainParameterFile = new List<string>();
            mainParameterFile.Add("UnitTest"); mainParameterFile.Add(""); mainParameterFile.Add("");
            string folderID = "Testing";
            int part1ScansToSum = 1;
            int numberOfDeconvolutionThreads = 1;

            //string inputDataFilename = @"g:\PNNL Files\Csharp\YAFMS\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.yafms";
            SimpleWorkflowParameters parameters = new SimpleWorkflowParameters();

            InputOutputFileName newFile = new InputOutputFileName();
            newFile.InputFileName = inputDataFilename;
            newFile.OutputSQLFileName = @"d:\Csharp\RAW\FindPeakTest.db";
            newFile.OutputFileName = mainParameterFile[1] + folderID + "_" + serverBlock + @".txt";
            RunFactory rf = new RunFactory();
           
            DeconvolutionType loadedDeconvolutionType = new DeconvolutionType();
            ConvertAToB converter = new ConvertAToB();
            loadedDeconvolutionType = converter.stringTODeconvolutionType(DeconType);
            
            MemorySplitObject newMemorySplitter = new MemorySplitObject();
            newMemorySplitter.NumberOfBlocks = serverBlockTotal;
            newMemorySplitter.BlockNumber = serverBlock;
               

            //setup files to load and save
            //InputOutputFileName newFile = new InputOutputFileName();
            //newFile = new InputOutputFileName();
            //newFile.InputFileName = serverDataFileName;
            //newFile.OutputFileName = mainParameterFile[1] + folderID + "_" + serverBlock + @".txt";
            //newFile.OutputSQLFileName = mainParameterFile[2] + folderID + @".db";

            newFile.OutputPath = mainParameterFile[1];

            //WriteVariablesToConsole(newFile, startScan, endScan, DataSpecificMassNeutron, part1SN, part2SN, newMemorySplitter);
            Console.WriteLine("SQLite Output: " + newFile.OutputSQLFileName + "\n");
            Console.WriteLine("Startscan: " + startScan + " EndScan: " + endScan);
            Console.WriteLine("Part1 SN: " + part1SN + " Part2 SN: " + part2SN);
            Console.WriteLine("DataSpecificMassNeutron: " + DataSpecificMassNeutron);
            Console.WriteLine("Processing Block# " + (newMemorySplitter.BlockNumber + 1).ToString() + " of " + newMemorySplitter.NumberOfBlocks);
            Console.WriteLine("Press Enter");

            FileInfo fi = new FileInfo(newFile.InputFileName);
            bool exists = fi.Exists;

            Console.WriteLine("CreateRun: " + newFile.InputFileName + " and its existance is " + exists.ToString());

            Run run = GoCreateRun.CreateRun(newFile);
            Console.WriteLine("after run was created");
            //Console.ReadKey();

            #region Setup Parameters for complete data set to find eluting peaks

                //SimpleWorkflowParameters parameters = new SimpleWorkflowParameters();
                parameters = new SimpleWorkflowParameters();

                //part 1 peak detector decontools only.  this gets overwritten below
                parameters.Part1Parameters.MSPeakDetectorPeakBR = 1.3;
                parameters.Part1Parameters.ElutingPeakNoiseThreshold = 2;
                parameters.Part2Parameters.MSPeakDetectorPeakBR = 1.3;
                parameters.Part2Parameters.MSPeakDetectorSigNoise = 2;
                //this contains the first call to Decon tools Engine V2
                ElutingPeakFinderController controller = new ElutingPeakFinderController(run, parameters);

                controller.Parameters.FileNameUsed = newFile.InputFileName;

                controller.Parameters.SummingMethod = ScanSumSelectSwitch.SumScan;

                //controller.Parameters.SummingMethod = ScanSumSelectSwitch.AlignScan;//Scotts summing
                //controller.Parameters.SummingMethod = ScanSumSelectSwitch.SumScan;

                //set here for omics typical run is 3
                //controller.Parameters.Part1Parameters.ElutingPeakNoiseThreshold = 3;//data 1 when NoiseRemoved, take 3 sigma off before the orbitrap filter
                controller.Parameters.Part1Parameters.ElutingPeakNoiseThreshold = part1SN;//when NoiseRemoved, take 3 sigma off before the orbitrap filter
                controller.Parameters.Part1Parameters.NoiseType = InstrumentDataNoiseType.NoiseRemoved;
                controller.Parameters.Part1Parameters.isDataAlreadyThresholded = false;//true for orbitrap
                controller.Parameters.Part1Parameters.PeakFitType = DeconTools.Backend.Globals.PeakFitType.LORENTZIAN;//this makes a large difference in the number of peaks
                controller.Parameters.Part1Parameters.StartScan = startScan;
                controller.Parameters.Part1Parameters.StopScan = endScan;
                controller.Parameters.Part1Parameters.MaxHeightForNewPeak = 0.75;
                controller.Parameters.Part1Parameters.AllignmentToleranceInPPM = 15;
                controller.Parameters.Part1Parameters.ParametersOrbitrap.massNeutron = DataSpecificMassNeutron;
                controller.Parameters.Part1Parameters.ParametersOrbitrap.DeltaMassTollerancePPM = 6000;
                controller.Parameters.Part1Parameters.ScansToBeSummed = part1ScansToSum;//default is 3
                controller.Parameters.Part1Parameters.ParametersOrbitrap.withLowMassesAllowed = true;
                controller.Parameters.Part1Parameters.ParametersOrbitrap.ExtraSigmaFactor = 1.5;//1=default

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
                controller.Parameters.Part2Parameters.numberOfDeconvolutionThreads = numberOfDeconvolutionThreads;
                controller.Parameters.Part2Parameters.ParametersOrbitrap.withLowMassesAllowed = true;
                controller.Parameters.Part2Parameters.ParametersOrbitrap.ExtraSigmaFactor = 1.5;//1=default

                //controller.Parameters.Part2Parameters.DeconvolutionType = DeconvolutionType.Thrash; 

                parameters.ConsistancyCrossErrorPPM = 20;
            #endregion

            #region Run Program Part 1

            //finds eluting peaks and calculates the start and stop scans

            Console.WriteLine("before Part 1");
            List<ElutingPeak> discoveredPeaks = controller.SimpleWorkflowExecutePart1(run);

            List<ElutingPeakOmics> discoveredOmicsPeaks = new List<ElutingPeakOmics>();
            ConvertToOmics newConverter = new ConvertToOmics();
            discoveredOmicsPeaks = newConverter.ConvertElutingPeakToElutingPeakOmics(discoveredPeaks);

            
            //ElutingPeakFinderController controller = new ElutingPeakFinderController(run, parameters);

            int unprocessedHits = discoveredOmicsPeaks.Count;

            //there are this many peaks returned*
            Console.WriteLine("there are " + unprocessedHits + " peaks");

            #endregion

            #region Part 2 coppied from Net 4.0
            SimpleWorkflowParameters Parameters = controller.Parameters;

            CalculateElutingPeakFeatures newPart2 = new CalculateElutingPeakFeatures();

            int hits = newPart2.SingleThread(newFile, discoveredOmicsPeaks, Parameters);
            
            #endregion

            Console.Write("Finished.  Press Return to Exit");

        int peakFound = run.ResultCollection.ElutingPeakCollection.Count;

        for (int i = 0; i < run.ResultCollection.ElutingPeakCollection.Count; i++)
        {
            //Console.WriteLine(run.ResultCollection.ElutingPeakCollection[i].PeakList[0].MSPeak.XValue);
        }
        //Assert.AreEqual(2887, peakFound);//was 203 for smaller set or different threshold
        //Assert.AreEqual(821, synchronizedElutingPeaks.Count);//was 53

        Assert.AreEqual(384, peakFound);//was 203 for smaller set or different threshold
        Assert.AreEqual(117, hits);//was 53
    }

        [Test]
        public void TestMultiThreadedDeisotoping()
        {
            List<DataSet> dataIn = new List<DataSet>();

            FileLoadController loader = new FileLoadController();
            dataIn = loader.GetData(@"R:\RAM Files\GetPeaks\GetPeaks.UnitTests\TextFileList.txt", GetPeaks_DLL.DataFIFO.FileIterator.deliminator.Tab);

            Console.WriteLine(dataIn[0].Name);
        }
        
        [Test]
        public void getYAFMSPeaks()
        {        
            int STARTSCAN = 5500;
            int ENDSCAN = 6500;
        
            Console.WriteLine("Find Peaks");

            //string inputDataFilename = @"d:\Csharp\YAFMS\20100722_glycan_SN24_NF.yafms";
            //string inputDataFilename = @"g:\PNNL Files\Csharp\YAFMS\20100722_glycan_SN24_NF.yafms";

    //        string inputDataFilename = @"d:\Csharp\YAFMS\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.yafms";
            //string inputDataFilename = @"g:\PNNL Files\Csharp\YAFMS\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.yafms";
            SimpleWorkflowParameters parameters = new SimpleWorkflowParameters();

            InputOutputFileName newFile = new InputOutputFileName();
            newFile.InputFileName = @"d:\Csharp\YAFMS\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.yafms"; ;

            double DataSpecificMassNeutron = 1.0013;//standard synthetic

            //parameters.MSPeakDetectorPeakBR = 1.3;//default
            //parameters.MSPeakDetectorSigNoise = 2;//default

            //parameters.MSPeakDetectorPeakBR = 1.3*30;
            //parameters.MSPeakDetectorSigNoise = 2*30;

            RunFactory rf = new RunFactory();
            Run run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, newFile.InputFileName);

            //run.MinLCScan = STARTSCAN;
            //run.MaxLCScan = ENDSCAN;

            int minLCScan = STARTSCAN;
            int maxLCScan = ENDSCAN;
            try
            {
                //ScanSetCollectionCreator scanSetCollectionCreator = new ScanSetCollectionCreator(run, run.MinScan, run.MaxScan, 1, 1, false);

                //run.ScanSetCollection.Create(run, fragmentedTargetedWorkflowParameters.NumMSScansToSum, 1, false);//This is the only scan set collection creator!
            
                run.ScanSetCollection.Create(run, minLCScan, maxLCScan, 1, 1, false);
                //run.ScanSetCollection.Create(run, minLCScan, maxLCScan, false);
                //run.ScanSetCollection = ScanSetCollection.Create(run, run.MinScan, run.MaxScan, 1, 1, false);
                Console.WriteLine("LoadingData...");
                //scanSetCollectionCreator.Create();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            ElutingPeakFinderController controller = new ElutingPeakFinderController(run, parameters);
            controller.Parameters.FileNameUsed = newFile.InputFileName;

            controller.Parameters.Part1Parameters.NoiseType = InstrumentDataNoiseType.NoiseRemoved;
            //controller.Parameters.Part1Parameters.ElutingPeakNoiseThreshold = 11.6;
            controller.Parameters.Part1Parameters.ElutingPeakNoiseThreshold = 3;
            controller.Parameters.Part1Parameters.PeakFitType = DeconTools.Backend.Globals.PeakFitType.LORENTZIAN;//this makes a large difference in the number of peaks
            controller.Parameters.Part1Parameters.StartScan = STARTSCAN;
            controller.Parameters.Part1Parameters.StopScan = ENDSCAN;
            controller.Parameters.Part1Parameters.MaxHeightForNewPeak = 0.75;
            controller.Parameters.Part1Parameters.AllignmentToleranceInPPM = 15;
            controller.Parameters.Part1Parameters.ParametersOrbitrap.massNeutron = DataSpecificMassNeutron;
            controller.Parameters.Part1Parameters.ParametersOrbitrap.DeltaMassTollerancePPM = 6000;

            controller.Parameters.Part2Parameters.NoiseType = InstrumentDataNoiseType.Standard;
            //controller.Parameters.Part2Parameters.MSPeakDetectorSigNoise = 6.5;
            controller.Parameters.Part2Parameters.MSPeakDetectorSigNoise = 10.5;
            controller.Parameters.Part2Parameters.PeakFitType = DeconTools.Backend.Globals.PeakFitType.LORENTZIAN;//this makes a large difference in the number of peaks
            controller.Parameters.Part2Parameters.DeconvolutionType = DeconvolutionType.Rapid;
            controller.Parameters.Part2Parameters.Multithread = false;
            controller.Parameters.Part2Parameters.DynamicRangeToOne = 300000;
            controller.Parameters.Part2Parameters.MaxScanSpread = 500;
            controller.Parameters.Part2Parameters.ParametersOrbitrap.massNeutron = DataSpecificMassNeutron;
            controller.Parameters.Part2Parameters.ParametersOrbitrap.DeltaMassTollerancePPM = 6000;

            //ElutingPeakFinderController controller = new ElutingPeakFinderController(run, parameters);
            //MSGeneratorFactory msgenFactory = new MSGeneratorFactory();
            //Task msGenerator = msgenFactory.CreateMSGenerator(run.MSFileType);
            var msGenerator = MSGeneratorFactory.CreateMSGenerator(run.MSFileType);

            double MSPeakDetectorPeakBR = 1.3;//default
            double MSPeakDetectorSigNoise = 2;//default

            DeconToolsPeakDetector msPeakDetector = new DeconToolsPeakDetector(MSPeakDetectorPeakBR, MSPeakDetectorSigNoise, DeconTools.Backend.Globals.PeakFitType.QUADRATIC, false);
            msPeakDetector.PeaksAreStored = true;

            int counter = 0;//for finding peak data in result list
            //int scanCounter = 0;
            
            foreach (ScanSet scanSet in run.ScanSetCollection.ScanSetList)
            //for (int i = 0; i < 3; i++)
            {
                run.CurrentScanSet = scanSet;
                //run.CurrentScanSet = run.ScanSetCollection.ScanSetList[i];

                //Console.WriteLine("YAFMS ScanSet " + run.ScanSetCollection.ScanSetList[i].PrimaryScanNumber.ToString());
                msGenerator.Execute(run.ResultCollection);

                // Console.WriteLine("YAFMS XYData " + run.XYData.Xvalues.Length.ToString());
                msPeakDetector.Execute(run.ResultCollection);

                //Console.WriteLine("YAFMS peaks " + run.ResultCollection.MSPeakResultList.Count.ToString());
                counter += run.ResultCollection.MSPeakResultList.Count;
            }

            Console.WriteLine("YAFMS peaks " + counter.ToString());
                
            Assert.AreEqual(7596685, counter);//all scans
            //Assert.AreEqual(31718, counter);//10 scans
            //Assert.AreEqual(6925, counter);//5 scans
            
            //Assert.AreEqual(35, counter);//1 scans
        }

        [Test]
        public void getRawPeaks()
        {
            int STARTSCAN = 5500;
            int ENDSCAN = 6500;

            Console.WriteLine("Find Peaks");

            //string inputDataFilename = @"d:\Csharp\YAFMS\20100722_glycan_SN24_NF.yafms";
            //string inputDataFilename = @"g:\PNNL Files\Csharp\YAFMS\20100722_glycan_SN24_NF.yafms";

            //string inputDataFilename = @"d:\Csharp\YAFMS\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.yafms";
            //string inputDataFilename = @"D:\PNNL Data\Gordon\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";
            string inputDataFilename = @"d:\Csharp\YAFMS\fresh copy\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";

            //string inputDataFilename = @"g:\PNNL Files\Csharp\YAFMS\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.yafms";
            SimpleWorkflowParameters parameters = new SimpleWorkflowParameters();

            double MSPeakDetectorPeakBR = 1.3;//default
            double MSPeakDetectorSigNoise = 2;//default

            //parameters.MSPeakDetectorPeakBR = 1.3*30;
            //parameters.MSPeakDetectorSigNoise = 2*30;

            RunFactory rf = new RunFactory();
            Run run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.Finnigan, inputDataFilename);

            //run.MinLCScan = STARTSCAN;
            //run.MaxLCScan = ENDSCAN;

            int minLCScan = STARTSCAN;
            int maxLCScan = ENDSCAN;
            try
            {
                //ScanSetCollectionCreator scanSetCollectionCreator = new ScanSetCollectionCreator(run, run.MinScan, run.MaxScan, 1, 1, false);

                run.ScanSetCollection.Create(run, minLCScan, maxLCScan, 1, 1, false);
                //run.ScanSetCollection = ScanSetCollection.Create(run, minLCScan, maxLCScan, 1, 1, false);
                //run.ScanSetCollection.Create(run, minLCScan, maxLCScan, 1, 1, false);

                //run.ScanSetCollection = ScanSetCollection.Create(run, run.MinScan, run.MaxScan, 1, 1, false);
                Console.WriteLine("LoadingData...");
                //scanSetCollectionCreator.Create();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            ElutingPeakFinderController controller = new ElutingPeakFinderController();
            //MSGeneratorFactory msgenFactory = new MSGeneratorFactory();
            //Task msGenerator = msgenFactory.CreateMSGenerator(run.MSFileType);
            var msGenerator = MSGeneratorFactory.CreateMSGenerator(run.MSFileType);

            DeconToolsPeakDetector msPeakDetector = new DeconToolsPeakDetector(MSPeakDetectorPeakBR, MSPeakDetectorSigNoise, DeconTools.Backend.Globals.PeakFitType.QUADRATIC, false);
            msPeakDetector.PeaksAreStored = true;

            int counter = 0;//for finding peak data in result list
            //int scanCounter = 0;
            foreach (ScanSet scanSet in run.ScanSetCollection.ScanSetList)
            //for (int i = 0; i < 3; i++)
            {
                run.CurrentScanSet = scanSet;
                //run.CurrentScanSet = run.ScanSetCollection.ScanSetList[i];

                //Console.WriteLine("Raw ScanSet " + run.ScanSetCollection.ScanSetList[i].PrimaryScanNumber.ToString());

                msGenerator.Execute(run.ResultCollection);
                
                //Console.WriteLine("Raw XYData " + run.XYData.Xvalues.Length.ToString());
                msPeakDetector.Execute(run.ResultCollection);
                //Console.WriteLine("Raw peaks " + run.ResultCollection.MSPeakResultList.Count.ToString());
                counter += run.ResultCollection.MSPeakResultList.Count;
            }

            Console.WriteLine("Raw peaks " + counter.ToString());

            Assert.AreEqual(7599762, counter);//full yafms scan is different from Raw
            //Assert.AreEqual(31718, counter);
            //Assert.AreEqual(6925, counter);//5 scans
            //Assert.AreEqual(35, counter);//1 scans
        }

        [Test]
        public void OrbitrapFilterTestFirstDirection()
        {
            List<ProcessedPeak> DataIn = OrbitrapDataSimple();
            Assert.AreEqual(DataIn.Count, 96);

            OrbitrapThreshold newOrbitrapThreshold = new OrbitrapThreshold();
            newOrbitrapThreshold.Parameters = new PeakThresholderParameters();
            newOrbitrapThreshold.ParametersOrbitrap = new OrbitrapFilterParameters();
            newOrbitrapThreshold.ParametersOrbitrap.massNeutron = 1.0013;
            newOrbitrapThreshold.ParametersOrbitrap.DeltaMassTollerancePPM = 3000;

            List<ProcessedPeak> thresholdedData = newOrbitrapThreshold.ApplyThreshold(ref DataIn);

            //there are 32 input masses
            //pass 1 Assert.AreEqual(29,thresholdedData.Count);//100,125.25,133.33,140 are missed because we only take one pass.  123.97 is extra and dumb luck that it was included
            Assert.AreEqual(34, thresholdedData.Count);//123.72 and 123.97 is extra and dumb luck that it was included.  
        
        }

        [Test]
        public void SumPeaks()
        {
            int STARTSCAN = 5500;
            int ENDSCAN = 6500;

            Console.WriteLine("Find Peaks");

            //string inputDataFilename = @"d:\Csharp\YAFMS\20100722_glycan_SN24_NF.yafms";
            //string inputDataFilename = @"g:\PNNL Files\Csharp\YAFMS\20100722_glycan_SN24_NF.yafms";

            //string inputDataFilename = @"d:\Csharp\YAFMS\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.yafms";
            //string inputDataFilename = @"D:\PNNL Data\Gordon\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";
            string inputDataFilename = @"d:\Csharp\YAFMS\fresh copy\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";

            //string inputDataFilename = @"g:\PNNL Files\Csharp\YAFMS\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.yafms";
            SimpleWorkflowParameters parameters = new SimpleWorkflowParameters();

            RunFactory rf = new RunFactory();
            Run run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.Finnigan, inputDataFilename);

            //run.MinLCScan = STARTSCAN;
            //run.MaxLCScan = ENDSCAN;

            int minLCScan = STARTSCAN;
            int maxLCScan = ENDSCAN;
            try
            {
                //ScanSetCollectionCreator scanSetCollectionCreator = new ScanSetCollectionCreator(run, run.MinScan, run.MaxScan, 1, 1, false);

                run.ScanSetCollection.Create(run, minLCScan, maxLCScan, 1, 1, false);
                //run.ScanSetCollection = ScanSetCollection.Create(run, minLCScan, maxLCScan, 1, 1, false);
                //run.ScanSetCollection.Create(run, minLCScan, maxLCScan, 1, 1, false);

                //ScanSetCollection.Create(run, run.MinScan, run.MaxScan, 1, 1, false);
                Console.WriteLine("LoadingData...");
                //scanSetCollectionCreator.Create();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            AllignAndSum letsAllignAndSum = new AllignAndSum();
            AllignAndSum2 letsAllignAndSum2 = new AllignAndSum2();
            AllignAndSum3 letsAllignAndSum3 = new AllignAndSum3();
            AllignAndSum4 letsAllignAndSum4 = new AllignAndSum4();
            List<DeconTools.Backend.XYData> AllignedScanStorage = new List<DeconTools.Backend.XYData>();

            int peakFirstScan = 6005;
            int peakLastScan = 6033;

            int numIterations = 20;

            Stopwatch sw = new Stopwatch();
            sw.Start();


            //for (int i = 0; i < run.ScanSetCollection.ScanSetList.Count - 1; i++)
            for (int i = 0; i < numIterations; i++)
            {
                Console.WriteLine("Working on scan #" + i.ToString() + " out of " + numIterations.ToString());
                //int peakFirstScan = run.ScanSetCollection.ScanSetList[i].PrimaryScanNumber;
                //int peakLastScan = run.ScanSetCollection.ScanSetList[i+1].PrimaryScanNumber;

                int version1 = 4;
                switch (version1)
                {
                    case 1:
                        {
                            letsAllignAndSum.AllignXAxisAndSum(run, peakFirstScan, peakLastScan);
                            break;
                        }
                    case 2: 
                        {
                            letsAllignAndSum2.AllignXAxisAndSum(run, peakFirstScan, peakLastScan);
                            break;
                        }
                    case 3:
                        {
                            letsAllignAndSum3.AllignXAxisAndSum(run, peakFirstScan, peakLastScan);
                            break;
                        }
                    case 4:
                        {
                            letsAllignAndSum4.AllignXAxisAndSum(run, peakFirstScan, peakLastScan);
                            break;
                        }
                }
                DeconTools.Backend.XYData AllignedScan = new DeconTools.Backend.XYData();
                AllignedScan = run.XYData;
                AllignedScanStorage.Add(AllignedScan);


            }

            sw.Stop();
            long summingTime = sw.ElapsedMilliseconds;

            Console.WriteLine("avg time for summed = " + (double)summingTime / (double)numIterations);

            Assert.AreEqual(20, AllignedScanStorage.Count);//full yafms scan is different from Raw
            //Assert.AreEqual(31718, counter);
            //Assert.AreEqual(6925, counter);//5 scans
            //Assert.AreEqual(35, counter);//1 scans
        }

        [Test]
        public void PeakDetectionRAW()
        {
            const int startScan = 5500;
            const int endScan = 5550;
            //const int ENDSCAN = 6500;

            Console.WriteLine("Find Peaks");

            //string inputDataFilename = @"d:\Csharp\YAFMS\20100722_glycan_SN24_NF.yafms";
            //string inputDataFilename = @"g:\PNNL Files\Csharp\YAFMS\20100722_glycan_SN24_NF.yafms";

            //string inputDataFilename = @"d:\Csharp\Syn Output\14 scan test file for get peaks\from external\yafmsOut.yafms";
            //string inputDataFilename = @"d:\Csharp\YAFMS\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.yafms";
            //string inputDataFilename = @"d:\PNNL Data\Gordon\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.raw";
            string inputDataFilename = @"E:\PNNL Data\2011_01_11 Gordon\2011_01_11 Standard QC Shew File\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.raw";

            //double DataSpecificMassNeutron = 1.0013;//standard synthetic
            double DataSpecificMassNeutron = 1.002149286;//SN09//THRASH=1.00235

            //Manual Variables
            double part1SN = 3;
            double part2SN = 3;
            string DeconType = "Thrash";
            int serverBlockTotal = 1;
            int serverBlock = 0;
            string serverDataFileName = inputDataFilename;
            List<string> mainParameterFile = new List<string>();
            mainParameterFile.Add("UnitTest"); mainParameterFile.Add(""); mainParameterFile.Add("");
            string folderID = "Testing";
            int part1ScansToSum = 1;
            int numberOfDeconvolutionThreads = 1;

            //string inputDataFilename = @"g:\PNNL Files\Csharp\YAFMS\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.yafms";
            SimpleWorkflowParameters parameters = new SimpleWorkflowParameters();

            InputOutputFileName newFile = new InputOutputFileName();
            newFile.InputFileName = inputDataFilename;
            newFile.OutputSQLFileName = @"d:\Csharp\RAW\FindPeakTest.db";
            newFile.OutputFileName = mainParameterFile[1] + folderID + "_" + serverBlock + @".txt";
            RunFactory rf = new RunFactory();

            DeconvolutionType loadedDeconvolutionType = new DeconvolutionType();
            ConvertAToB converter = new ConvertAToB();
            loadedDeconvolutionType = converter.stringTODeconvolutionType(DeconType);

            MemorySplitObject newMemorySplitter = new MemorySplitObject();
            newMemorySplitter.NumberOfBlocks = serverBlockTotal;
            newMemorySplitter.BlockNumber = serverBlock;


            //setup files to load and save
            //InputOutputFileName newFile = new InputOutputFileName();
            //newFile = new InputOutputFileName();
            //newFile.InputFileName = serverDataFileName;
            //newFile.OutputFileName = mainParameterFile[1] + folderID + "_" + serverBlock + @".txt";
            //newFile.OutputSQLFileName = mainParameterFile[2] + folderID + @".db";

            newFile.OutputPath = mainParameterFile[1];

            //WriteVariablesToConsole(newFile, startScan, endScan, DataSpecificMassNeutron, part1SN, part2SN, newMemorySplitter);
            Console.WriteLine("SQLite Output: " + newFile.OutputSQLFileName + "\n");
            Console.WriteLine("Startscan: " + startScan + " EndScan: " + endScan);
            Console.WriteLine("Part1 SN: " + part1SN + " Part2 SN: " + part2SN);
            Console.WriteLine("DataSpecificMassNeutron: " + DataSpecificMassNeutron);
            Console.WriteLine("Processing Block# " + (newMemorySplitter.BlockNumber + 1).ToString() + " of " + newMemorySplitter.NumberOfBlocks);
            Console.WriteLine("Press Enter");

            FileInfo fi = new FileInfo(newFile.InputFileName);
            bool exists = fi.Exists;

            Console.WriteLine("CreateRun: " + newFile.InputFileName + " and its existance is " + exists.ToString());

            Run run = GoCreateRun.CreateRun(newFile);
            Console.WriteLine("after run was created");
            //Console.ReadKey();

            #region Setup Parameters for complete data set to find eluting peaks

            //SimpleWorkflowParameters parameters = new SimpleWorkflowParameters();
            parameters = new SimpleWorkflowParameters();

            //part 1 peak detector decontools only.  this gets overwritten below
            parameters.Part1Parameters.MSPeakDetectorPeakBR = 1.3;
            parameters.Part1Parameters.ElutingPeakNoiseThreshold = 2;
            parameters.Part2Parameters.MSPeakDetectorPeakBR = 1.3;
            parameters.Part2Parameters.MSPeakDetectorSigNoise = 2;
            //this contains the first call to Decon tools Engine V2
            ElutingPeakFinderController controller = new ElutingPeakFinderController(run, parameters);

            controller.Parameters.FileNameUsed = newFile.InputFileName;

            controller.Parameters.SummingMethod = ScanSumSelectSwitch.SumScan;

            //controller.Parameters.SummingMethod = ScanSumSelectSwitch.AlignScan;//Scotts summing
            //controller.Parameters.SummingMethod = ScanSumSelectSwitch.SumScan;

            //set here for omics typical run is 3
            //controller.Parameters.Part1Parameters.ElutingPeakNoiseThreshold = 3;//data 1 when NoiseRemoved, take 3 sigma off before the orbitrap filter
            controller.Parameters.Part1Parameters.ElutingPeakNoiseThreshold = part1SN;//when NoiseRemoved, take 3 sigma off before the orbitrap filter
            controller.Parameters.Part1Parameters.NoiseType = InstrumentDataNoiseType.NoiseRemoved;
            controller.Parameters.Part1Parameters.isDataAlreadyThresholded = false;//true for orbitrap
            controller.Parameters.Part1Parameters.PeakFitType = DeconTools.Backend.Globals.PeakFitType.LORENTZIAN;//this makes a large difference in the number of peaks
            controller.Parameters.Part1Parameters.StartScan = startScan;
            controller.Parameters.Part1Parameters.StopScan = endScan;
            controller.Parameters.Part1Parameters.MaxHeightForNewPeak = 0.75;
            controller.Parameters.Part1Parameters.AllignmentToleranceInPPM = 15;
            controller.Parameters.Part1Parameters.ParametersOrbitrap.massNeutron = DataSpecificMassNeutron;
            controller.Parameters.Part1Parameters.ParametersOrbitrap.DeltaMassTollerancePPM = 6000;
            controller.Parameters.Part1Parameters.ScansToBeSummed = part1ScansToSum;//default is 3
            controller.Parameters.Part1Parameters.ParametersOrbitrap.withLowMassesAllowed = true;
            controller.Parameters.Part1Parameters.ParametersOrbitrap.ExtraSigmaFactor = 1.5;//1=default

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
            controller.Parameters.Part2Parameters.numberOfDeconvolutionThreads = numberOfDeconvolutionThreads;
            controller.Parameters.Part2Parameters.ParametersOrbitrap.withLowMassesAllowed = true;
            controller.Parameters.Part2Parameters.ParametersOrbitrap.ExtraSigmaFactor = 1.5;//1=default

            //controller.Parameters.Part2Parameters.DeconvolutionType = DeconvolutionType.Thrash; 

            parameters.ConsistancyCrossErrorPPM = 20;
            #endregion

            #region Run Program Part 1

            //finds eluting peaks and calculates the start and stop scans

            //disected part 1
            //MSGeneratorFactory msgenFactory =new MSGeneratorFactory();
            //Task msGenerator = msgenFactory.CreateMSGenerator(run.MSFileType);
            var msGenerator = MSGeneratorFactory.CreateMSGenerator(run.MSFileType);
            SimpleWorkflowParameters Parameters = controller.Parameters;
            DeconToolsPeakDetector msPeakDetector = new DeconToolsPeakDetector(parameters.Part1Parameters.MSPeakDetectorPeakBR, parameters.Part1Parameters.ElutingPeakNoiseThreshold, parameters.Part2Parameters.PeakFitType, false);

            System.Timers.Timer aTimer = new System.Timers.Timer();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            //run.MinLCScan = Parameters.Part1Parameters.StartScan;
            //run.MaxLCScan = Parameters.Part1Parameters.StopScan;

            

            int minLCScan = Parameters.Part1Parameters.StartScan;
            int maxLCScan = Parameters.Part1Parameters.StopScan;
            int scansToSum = Parameters.Part1Parameters.ScansToBeSummed;//1,3,5 etc centered on main scan

            List<ElutingPeak> elutingPeakCollectionStorage = new List<ElutingPeak>();

            double alignmentTolerance = Parameters.Part1Parameters.AllignmentToleranceInPPM;
            try
            {
                //bool GetMSMSDataAlso = false;
                //ScanSetCollectionCreator scanSetCollectionCreator = new ScanSetCollectionCreator(run, run.MinScan, run.MaxScan, scansToSum, 1, GetMSMSDataAlso);
                //run.ScanSetCollection = ScanSetCollection.Create(run, minLCScan, maxLCScan, 1, 1, GetMSMSDataAlso);
                run.ScanSetCollection.Create(run, minLCScan, maxLCScan, 1, 1, false);
                //run.ScanSetCollection.Create(run, minLCScan, maxLCScan, 1, 1, GetMSMSDataAlso); 
                

                //run.ScanSetCollection = ScanSetCollection.Create(run, run.MinScan, run.MaxScan, scansToSum, 1, GetMSMSDataAlso);
                Console.WriteLine("LoadingData...");
                //scanSetCollectionCreator.Create();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            List<List<ProcessedPeak>> PileOfThresholdedData = new List<List<ProcessedPeak>>();

            int scanCounter = 0;
            //bool isFirstScanSet = true;
            //int scansetInitialOffset = 0; //offset of first scan with data
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

                    msGenerator.Execute(run.ResultCollection);

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
                            //List<double> newList = new List<double>();
                            //foreach (ProcessedPeak peak in thresholdedData)
                            //{
                            //    if (peak.XValue > 1000f && peak.XValue < 1100f)
                            //    {
                            //        newList.Add(peak.XValue);
                            //    }
                            //}
                            #endregion

                            #region vestigial Add scan Number ON
                            for (int i = 0; i < thresholdedData.Count; i++)
                            {
                                thresholdedData[i].ScanNumber = scanSet.PrimaryScanNumber;
                            }
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
                                //newMSPeak.SignalToNoise = Convert.ToSingle(thresholdedData[i].SignalToNoiseGlobal);
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

                            #region vestigial print stuff OFF
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


                            #region vestigial convert to Omics Processed Peaks ON
                            thresholdedData = new List<ProcessedPeak>();
                            for (int i = 0; i < run.ResultCollection.Run.PeakList.Count; i++)
                            {
                                ProcessedPeak newPeakOmics = new ProcessedPeak();
                                newPeakOmics.XValue = run.ResultCollection.Run.PeakList[i].XValue;
                                newPeakOmics.Height = run.ResultCollection.Run.PeakList[i].Height;
                                newPeakOmics.Width = run.ResultCollection.Run.PeakList[i].Width;
                                newPeakOmics.ScanNumber = scanSet.PrimaryScanNumber;
                                thresholdedData.Add(newPeakOmics);
                            }
                            #endregion

                            #endregion
                            ///output: run.ResultCollection.PeakCounter
                            ///output: run.ResultCollection.MSPeakResultList
                            ///output: run.ResultCollection.Run.PeakList
                            ///output: run.ResultCollection.Run.DeconToolsPeakList
                        }

                        int MSpeakResultsListCount = run.ResultCollection.MSPeakResultList.Count;
                        int newPeaks = run.ResultCollection.Run.PeakList.Count;//new peaks found in PeakDetector
                        int peaksThatPassedThreshold = thresholdedData.Count;

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

                        PileOfThresholdedData.Add(thresholdedData);
                            
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


            #region setup Manual annotation data
            List<ManualAnnotation> perfectData = new List<ManualAnnotation>();
            perfectData.Add(new ManualAnnotation(5502, 759.39855957));
            perfectData.Add(new ManualAnnotation(5502, 759.901184082));
            perfectData.Add(new ManualAnnotation(5502, 760.401367188));
            perfectData.Add(new ManualAnnotation(5502, 775.393859863));
            perfectData.Add(new ManualAnnotation(5502, 776.400024414));
            perfectData.Add(new ManualAnnotation(5502, 854.53704834));
            perfectData.Add(new ManualAnnotation(5502, 895.945861816));
            perfectData.Add(new ManualAnnotation(5502, 896.447570801));
            perfectData.Add(new ManualAnnotation(5502, 906.494934082));
            perfectData.Add(new ManualAnnotation(5502, 907.496582031));
            perfectData.Add(new ManualAnnotation(5502, 943.008056641));
            perfectData.Add(new ManualAnnotation(5502, 955.538635254));
            perfectData.Add(new ManualAnnotation(5502, 956.542236328));
            perfectData.Add(new ManualAnnotation(5502, 1011.94799805));
            perfectData.Add(new ManualAnnotation(5502, 1012.44934082));
            perfectData.Add(new ManualAnnotation(5502, 1012.95068359));
            perfectData.Add(new ManualAnnotation(5502, 1013.45037842));
            perfectData.Add(new ManualAnnotation(5502, 1047.51452637));
            perfectData.Add(new ManualAnnotation(5502, 1048.01782227));
            perfectData.Add(new ManualAnnotation(5502, 1048.51831055));
            perfectData.Add(new ManualAnnotation(5502, 1083.03137207));
            perfectData.Add(new ManualAnnotation(5502, 1083.5078125));
            perfectData.Add(new ManualAnnotation(5502, 1084.03210449));
            perfectData.Add(new ManualAnnotation(5502, 1084.50952148));
            perfectData.Add(new ManualAnnotation(5502, 1213.57019043));
            perfectData.Add(new ManualAnnotation(5502, 1214.57531738));




            #endregion

            #region test data for manually acceptable data

            List<ProcessedPeak> missedData = new List<ProcessedPeak>();

            CompareControllerOld compareHere = new CompareControllerOld();
            CompareResults resultsFromCompare = new CompareResults();
            //int counter = 0;
            double tollPPM = 10;
        
            int unprocessedHits = 0;
            List<decimal> libraryMassesB = new List<decimal>();
            List<decimal> ExperimentalMassesB = new List<decimal>();
            List<List<decimal>> HitsCollection = new List<List<decimal>>();
            List<List<decimal>> MissCollection = new List<List<decimal>>();
            List<List<decimal>> NonLibraryListCollection =  new List<List<decimal>>();

            for (int j = 0; j < PileOfThresholdedData.Count; j++)
            {
            
                ExperimentalMassesB = new List<decimal>();

                List<ProcessedPeak> thresholdedList = PileOfThresholdedData[j];

                //counter = 0;
                for (int i = 0; i < thresholdedList.Count; i++)
                {
                    ExperimentalMassesB.Add(Convert.ToDecimal(thresholdedList[i].XValue));
                }
                for (int i = 0; i < perfectData.Count; i++)
                {
                    if (thresholdedList[0].ScanNumber == perfectData[i].ScanNumber)
                    {
                        libraryMassesB.Add(Convert.ToDecimal(perfectData[i].MassMZ));
                    }
                }

                compareHere.compareFX(libraryMassesB, ExperimentalMassesB, resultsFromCompare, tollPPM);

                //decomission compare
                List<decimal> hitsList;
                List<decimal> missList;
                List<decimal> nonLibraryList;
                ProcessCompareAndContrastResults(resultsFromCompare, libraryMassesB, ExperimentalMassesB, out hitsList, out missList, out nonLibraryList);
            
                HitsCollection.Add(hitsList);
                MissCollection.Add(missList);
                NonLibraryListCollection.Add(nonLibraryList);
            }
            #endregion

            //ElutingPeakFinderController controller = new ElutingPeakFinderController(run, parameters);

            //int unprocessedHits = discoveredOmicsPeaks.Count;

            //there are this many peaks returned*
            Console.WriteLine("there are " + unprocessedHits + " peaks");

            #endregion

            for (int i = 0; i < HitsCollection.Count; i++)
            {
                double fraction = 0;
                fraction = Convert.ToDouble(HitsCollection[i].Count) / Convert.ToDouble(libraryMassesB.Count)*100;
                fraction = Math.Round(fraction,2);
                double falsePeaks = 0;
                falsePeaks = NonLibraryListCollection[i].Count;
                Console.WriteLine("We found " + fraction + "% of the desired peaks.  There are " + falsePeaks + " extra peaks");
            }
        
            Console.WriteLine("Finished.  Press Return to Exit");

            //current best
            Assert.AreEqual(13, HitsCollection[0].Count);//was 203 for smaller set or different threshold
            //Assert.AreEqual(117, hits);//was 53
        }

        //Private Functions

        private static void ProcessCompareAndContrastResults(CompareResults resultsFromCompare, List<decimal> libraryMassesB, List<decimal> ExperimentalMassesB, out List<decimal> hitsList, out List<decimal> missList, out List<decimal> nonLibraryList)
        {
            hitsList = new List<decimal>();
            missList = new List<decimal>();
            nonLibraryList = new List<decimal>();

            //create a list of the hits in the library.  The list is populated with the experimental data.
            //The first data is returned when there is more than one hit
            for (int i = 0; i < resultsFromCompare.IndexListAMatch.Count; i++)
            {
                if (i == 0)
                {
                    hitsList.Add(ExperimentalMassesB[resultsFromCompare.IndexListAMatch[i]]);
                }
                else
                {
                    int index1 = resultsFromCompare.IndexListBMatch[i];
                    int index2 = resultsFromCompare.IndexListBMatch[i - 1];
                    if (index1 != index2)
                    {
                        hitsList.Add(ExperimentalMassesB[resultsFromCompare.IndexListAMatch[i]]);
                    }
                }
            }

            //create a list of library ions that were not found
            for (int i = 0; i < resultsFromCompare.IndexListBandNotA.Count; i++)
            {
                missList.Add(libraryMassesB[resultsFromCompare.IndexListBandNotA[i]]);
            }

            //create a list of library ions that were not found
            for (int i = 0; i < resultsFromCompare.IndexListAandNotB.Count; i++)
            {
                nonLibraryList.Add(ExperimentalMassesB[resultsFromCompare.IndexListAandNotB[i]]);
            }
        }

        private class ManualAnnotation
        {
            public int ScanNumber { get; set; }
            public double MassMZ { get; set; }

            public ManualAnnotation(int scan, double mass)
            {
                ScanNumber = scan;
                MassMZ = mass;
            }
        }

        private static List<ProcessedPeak> OrbitrapDataSimple()
        {
            List<ProcessedPeak> DataList = new List<ProcessedPeak>();

            #region data input
            ProcessedPeak newPeak1 = new ProcessedPeak(); newPeak1.XValue = 96.81f; newPeak1.Height = 200; DataList.Add(newPeak1);
            ProcessedPeak newPeak2 = new ProcessedPeak(); newPeak2.XValue = 96.9f; newPeak2.Height = 200; DataList.Add(newPeak2);
            ProcessedPeak newPeak3 = new ProcessedPeak(); newPeak3.XValue = 97.87f; newPeak3.Height = 200; DataList.Add(newPeak3);
            ProcessedPeak newPeak4 = new ProcessedPeak(); newPeak4.XValue = 100f; newPeak4.Height = 200; DataList.Add(newPeak4);
            ProcessedPeak newPeak5 = new ProcessedPeak(); newPeak5.XValue = 100.02f; newPeak5.Height = 200; DataList.Add(newPeak5);
            ProcessedPeak newPeak6 = new ProcessedPeak(); newPeak6.XValue = 100.07f; newPeak6.Height = 200; DataList.Add(newPeak6);
            ProcessedPeak newPeak7 = new ProcessedPeak(); newPeak7.XValue = 101f; newPeak7.Height = 200; DataList.Add(newPeak7);
            ProcessedPeak newPeak8 = new ProcessedPeak(); newPeak8.XValue = 101.92f; newPeak8.Height = 200; DataList.Add(newPeak8);
            ProcessedPeak newPeak9 = new ProcessedPeak(); newPeak9.XValue = 102f; newPeak9.Height = 200; DataList.Add(newPeak9);
            ProcessedPeak newPeak10 = new ProcessedPeak(); newPeak10.XValue = 103f; newPeak10.Height = 200; DataList.Add(newPeak10);
            ProcessedPeak newPeak11 = new ProcessedPeak(); newPeak11.XValue = 103.64f; newPeak11.Height = 200; DataList.Add(newPeak11);
            ProcessedPeak newPeak12 = new ProcessedPeak(); newPeak12.XValue = 104f; newPeak12.Height = 200; DataList.Add(newPeak12);
            ProcessedPeak newPeak13 = new ProcessedPeak(); newPeak13.XValue = 105f; newPeak13.Height = 200; DataList.Add(newPeak13);
            ProcessedPeak newPeak14 = new ProcessedPeak(); newPeak14.XValue = 105.08f; newPeak14.Height = 200; DataList.Add(newPeak14);
            ProcessedPeak newPeak15 = new ProcessedPeak(); newPeak15.XValue = 105.88f; newPeak15.Height = 200; DataList.Add(newPeak15);
            ProcessedPeak newPeak16 = new ProcessedPeak(); newPeak16.XValue = 106f; newPeak16.Height = 200; DataList.Add(newPeak16);
            ProcessedPeak newPeak17 = new ProcessedPeak(); newPeak17.XValue = 106.2f; newPeak17.Height = 200; DataList.Add(newPeak17);
            ProcessedPeak newPeak18 = new ProcessedPeak(); newPeak18.XValue = 107f; newPeak18.Height = 200; DataList.Add(newPeak18);
            ProcessedPeak newPeak19 = new ProcessedPeak(); newPeak19.XValue = 107.44f; newPeak19.Height = 200; DataList.Add(newPeak19);
            ProcessedPeak newPeak20 = new ProcessedPeak(); newPeak20.XValue = 109.04f; newPeak20.Height = 200; DataList.Add(newPeak20);
            ProcessedPeak newPeak21 = new ProcessedPeak(); newPeak21.XValue = 112.93f; newPeak21.Height = 200; DataList.Add(newPeak21);
            ProcessedPeak newPeak22 = new ProcessedPeak(); newPeak22.XValue = 118.18f; newPeak22.Height = 200; DataList.Add(newPeak22);
            ProcessedPeak newPeak23 = new ProcessedPeak(); newPeak23.XValue = 120.66f; newPeak23.Height = 200; DataList.Add(newPeak23);
            ProcessedPeak newPeak24 = new ProcessedPeak(); newPeak24.XValue = 123.72f; newPeak24.Height = 200; DataList.Add(newPeak24);
            ProcessedPeak newPeak25 = new ProcessedPeak(); newPeak25.XValue = 123.84f; newPeak25.Height = 200; DataList.Add(newPeak25);
            ProcessedPeak newPeak26 = new ProcessedPeak(); newPeak26.XValue = 123.97f; newPeak26.Height = 200; DataList.Add(newPeak26);
            ProcessedPeak newPeak27 = new ProcessedPeak(); newPeak27.XValue = 125f; newPeak27.Height = 200; DataList.Add(newPeak27);
            ProcessedPeak newPeak28 = new ProcessedPeak(); newPeak28.XValue = 125.01f; newPeak28.Height = 200; DataList.Add(newPeak28);
            ProcessedPeak newPeak29 = new ProcessedPeak(); newPeak29.XValue = 125.25f; newPeak29.Height = 200; DataList.Add(newPeak29);
            ProcessedPeak newPeak30 = new ProcessedPeak(); newPeak30.XValue = 125.5f; newPeak30.Height = 200; DataList.Add(newPeak30);
            ProcessedPeak newPeak31 = new ProcessedPeak(); newPeak31.XValue = 125.75f; newPeak31.Height = 200; DataList.Add(newPeak31);
            ProcessedPeak newPeak32 = new ProcessedPeak(); newPeak32.XValue = 126f; newPeak32.Height = 200; DataList.Add(newPeak32);
            ProcessedPeak newPeak33 = new ProcessedPeak(); newPeak33.XValue = 126.25f; newPeak33.Height = 200; DataList.Add(newPeak33);
            ProcessedPeak newPeak34 = new ProcessedPeak(); newPeak34.XValue = 126.48f; newPeak34.Height = 200; DataList.Add(newPeak34);
            ProcessedPeak newPeak35 = new ProcessedPeak(); newPeak35.XValue = 126.5f; newPeak35.Height = 200; DataList.Add(newPeak35);
            ProcessedPeak newPeak36 = new ProcessedPeak(); newPeak36.XValue = 126.75f; newPeak36.Height = 200; DataList.Add(newPeak36);
            ProcessedPeak newPeak37 = new ProcessedPeak(); newPeak37.XValue = 128.03f; newPeak37.Height = 200; DataList.Add(newPeak37);
            ProcessedPeak newPeak38 = new ProcessedPeak(); newPeak38.XValue = 128.77f; newPeak38.Height = 200; DataList.Add(newPeak38);
            ProcessedPeak newPeak39 = new ProcessedPeak(); newPeak39.XValue = 128.79f; newPeak39.Height = 200; DataList.Add(newPeak39);
            ProcessedPeak newPeak40 = new ProcessedPeak(); newPeak40.XValue = 129.71f; newPeak40.Height = 200; DataList.Add(newPeak40);
            ProcessedPeak newPeak41 = new ProcessedPeak(); newPeak41.XValue = 130.46f; newPeak41.Height = 200; DataList.Add(newPeak41);
            ProcessedPeak newPeak42 = new ProcessedPeak(); newPeak42.XValue = 130.88f; newPeak42.Height = 200; DataList.Add(newPeak42);
            ProcessedPeak newPeak43 = new ProcessedPeak(); newPeak43.XValue = 131.77f; newPeak43.Height = 200; DataList.Add(newPeak43);
            ProcessedPeak newPeak44 = new ProcessedPeak(); newPeak44.XValue = 132.28f; newPeak44.Height = 200; DataList.Add(newPeak44);
            ProcessedPeak newPeak45 = new ProcessedPeak(); newPeak45.XValue = 133f; newPeak45.Height = 200; DataList.Add(newPeak45);
            ProcessedPeak newPeak46 = new ProcessedPeak(); newPeak46.XValue = 133.23f; newPeak46.Height = 200; DataList.Add(newPeak46);
            ProcessedPeak newPeak47 = new ProcessedPeak(); newPeak47.XValue = 133.333333333333f; newPeak47.Height = 200; DataList.Add(newPeak47);
            ProcessedPeak newPeak48 = new ProcessedPeak(); newPeak48.XValue = 133.4f; newPeak48.Height = 200; DataList.Add(newPeak48);
            ProcessedPeak newPeak49 = new ProcessedPeak(); newPeak49.XValue = 133.666666666666f; newPeak49.Height = 200; DataList.Add(newPeak49);
            ProcessedPeak newPeak50 = new ProcessedPeak(); newPeak50.XValue = 134f; newPeak50.Height = 200; DataList.Add(newPeak50);
            ProcessedPeak newPeak51 = new ProcessedPeak(); newPeak51.XValue = 134.01f; newPeak51.Height = 200; DataList.Add(newPeak51);
            ProcessedPeak newPeak52 = new ProcessedPeak(); newPeak52.XValue = 134.333333333333f; newPeak52.Height = 200; DataList.Add(newPeak52);
            ProcessedPeak newPeak53 = new ProcessedPeak(); newPeak53.XValue = 134.666666666666f; newPeak53.Height = 200; DataList.Add(newPeak53);
            ProcessedPeak newPeak54 = new ProcessedPeak(); newPeak54.XValue = 135f; newPeak54.Height = 200; DataList.Add(newPeak54);
            ProcessedPeak newPeak55 = new ProcessedPeak(); newPeak55.XValue = 135.27f; newPeak55.Height = 200; DataList.Add(newPeak55);
            ProcessedPeak newPeak56 = new ProcessedPeak(); newPeak56.XValue = 135.333333333333f; newPeak56.Height = 200; DataList.Add(newPeak56);
            ProcessedPeak newPeak57 = new ProcessedPeak(); newPeak57.XValue = 135.56f; newPeak57.Height = 200; DataList.Add(newPeak57);
            ProcessedPeak newPeak58 = new ProcessedPeak(); newPeak58.XValue = 136.01f; newPeak58.Height = 200; DataList.Add(newPeak58);
            ProcessedPeak newPeak59 = new ProcessedPeak(); newPeak59.XValue = 139.6f; newPeak59.Height = 200; DataList.Add(newPeak59);
            ProcessedPeak newPeak60 = new ProcessedPeak(); newPeak60.XValue = 140.24f; newPeak60.Height = 200; DataList.Add(newPeak60);
            ProcessedPeak newPeak61 = new ProcessedPeak(); newPeak61.XValue = 140.51f; newPeak61.Height = 200; DataList.Add(newPeak61);
            ProcessedPeak newPeak62 = new ProcessedPeak(); newPeak62.XValue = 142.55f; newPeak62.Height = 200; DataList.Add(newPeak62);
            ProcessedPeak newPeak63 = new ProcessedPeak(); newPeak63.XValue = 142.86f; newPeak63.Height = 200; DataList.Add(newPeak63);
            ProcessedPeak newPeak64 = new ProcessedPeak(); newPeak64.XValue = 143.14f; newPeak64.Height = 200; DataList.Add(newPeak64);
            ProcessedPeak newPeak65 = new ProcessedPeak(); newPeak65.XValue = 143.35f; newPeak65.Height = 200; DataList.Add(newPeak65);
            ProcessedPeak newPeak66 = new ProcessedPeak(); newPeak66.XValue = 144.95f; newPeak66.Height = 200; DataList.Add(newPeak66);
            ProcessedPeak newPeak67 = new ProcessedPeak(); newPeak67.XValue = 145.14f; newPeak67.Height = 200; DataList.Add(newPeak67);
            ProcessedPeak newPeak68 = new ProcessedPeak(); newPeak68.XValue = 145.49f; newPeak68.Height = 200; DataList.Add(newPeak68);
            ProcessedPeak newPeak69 = new ProcessedPeak(); newPeak69.XValue = 146.4f; newPeak69.Height = 200; DataList.Add(newPeak69);
            ProcessedPeak newPeak70 = new ProcessedPeak(); newPeak70.XValue = 147.39f; newPeak70.Height = 200; DataList.Add(newPeak70);
            ProcessedPeak newPeak71 = new ProcessedPeak(); newPeak71.XValue = 150f; newPeak71.Height = 200; DataList.Add(newPeak71);
            ProcessedPeak newPeak72 = new ProcessedPeak(); newPeak72.XValue = 150.5f; newPeak72.Height = 200; DataList.Add(newPeak72);
            ProcessedPeak newPeak73 = new ProcessedPeak(); newPeak73.XValue = 151f; newPeak73.Height = 200; DataList.Add(newPeak73);
            ProcessedPeak newPeak74 = new ProcessedPeak(); newPeak74.XValue = 151.5f; newPeak74.Height = 200; DataList.Add(newPeak74);
            ProcessedPeak newPeak75 = new ProcessedPeak(); newPeak75.XValue = 152f; newPeak75.Height = 200; DataList.Add(newPeak75);
            ProcessedPeak newPeak76 = new ProcessedPeak(); newPeak76.XValue = 152.13f; newPeak76.Height = 200; DataList.Add(newPeak76);
            ProcessedPeak newPeak77 = new ProcessedPeak(); newPeak77.XValue = 152.5f; newPeak77.Height = 200; DataList.Add(newPeak77);
            ProcessedPeak newPeak78 = new ProcessedPeak(); newPeak78.XValue = 152.92f; newPeak78.Height = 200; DataList.Add(newPeak78);
            ProcessedPeak newPeak79 = new ProcessedPeak(); newPeak79.XValue = 153f; newPeak79.Height = 200; DataList.Add(newPeak79);
            ProcessedPeak newPeak80 = new ProcessedPeak(); newPeak80.XValue = 153.5f; newPeak80.Height = 200; DataList.Add(newPeak80);
            ProcessedPeak newPeak81 = new ProcessedPeak(); newPeak81.XValue = 154.95f; newPeak81.Height = 200; DataList.Add(newPeak81);
            ProcessedPeak newPeak82 = new ProcessedPeak(); newPeak82.XValue = 155.79f; newPeak82.Height = 200; DataList.Add(newPeak82);
            ProcessedPeak newPeak83 = new ProcessedPeak(); newPeak83.XValue = 156.46f; newPeak83.Height = 200; DataList.Add(newPeak83);
            ProcessedPeak newPeak84 = new ProcessedPeak(); newPeak84.XValue = 156.9f; newPeak84.Height = 200; DataList.Add(newPeak84);
            ProcessedPeak newPeak85 = new ProcessedPeak(); newPeak85.XValue = 158.84f; newPeak85.Height = 200; DataList.Add(newPeak85);
            ProcessedPeak newPeak86 = new ProcessedPeak(); newPeak86.XValue = 164.13f; newPeak86.Height = 200; DataList.Add(newPeak86);
            ProcessedPeak newPeak87 = new ProcessedPeak(); newPeak87.XValue = 166.84f; newPeak87.Height = 200; DataList.Add(newPeak87);
            ProcessedPeak newPeak88 = new ProcessedPeak(); newPeak88.XValue = 167.36f; newPeak88.Height = 200; DataList.Add(newPeak88);
            ProcessedPeak newPeak89 = new ProcessedPeak(); newPeak89.XValue = 170.53f; newPeak89.Height = 200; DataList.Add(newPeak89);
            ProcessedPeak newPeak90 = new ProcessedPeak(); newPeak90.XValue = 170.63f; newPeak90.Height = 200; DataList.Add(newPeak90);
            ProcessedPeak newPeak91 = new ProcessedPeak(); newPeak91.XValue = 171.61f; newPeak91.Height = 200; DataList.Add(newPeak91);
            ProcessedPeak newPeak92 = new ProcessedPeak(); newPeak92.XValue = 172.56f; newPeak92.Height = 200; DataList.Add(newPeak92);
            ProcessedPeak newPeak93 = new ProcessedPeak(); newPeak93.XValue = 173.24f; newPeak93.Height = 200; DataList.Add(newPeak93);
            ProcessedPeak newPeak94 = new ProcessedPeak(); newPeak94.XValue = 173.66f; newPeak94.Height = 200; DataList.Add(newPeak94);
            ProcessedPeak newPeak95 = new ProcessedPeak(); newPeak95.XValue = 173.93f; newPeak95.Height = 200; DataList.Add(newPeak95);
            ProcessedPeak newPeak96 = new ProcessedPeak(); newPeak96.XValue = 174.51f; newPeak96.Height = 200; DataList.Add(newPeak96);
            #endregion

            return DataList;
        }
    }
}
