using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL;
using GetPeaks_DLL.Go_Decon_Modules;
using GetPeaks_DLL.DeconToolsPart2;


namespace GetPeaks
{
    
    class ConsoleStart
    {
        /// <summary>
        /// Work Debug args:  "D:\PNNL CSharp\0_BatchFiles\0_WorkParameterFileSTD.txt" "D:\Csharp\GetPeaksOutput" "D:\Csharp\GetPeaksOutput\SQLiteBatchResult"
        /// Home Debug args:  "G:\PNNL Files\PNNL CSharp\0_BatchFiles\0_HomeParameterFileSTD.txt" "G:\PNNL Files\Csharp\GetPeaksOutput" "G:\PNNL Files\Csharp\GetPeaksOutput\SQLiteBatchResult"
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("\nFind Peaks");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            System.DateTime starttime = DateTime.Now;

            //int timesitCleanedUp = GC.CollectionCount(0);
            //Console.WriteLine(timesitCleanedUp);

            #region test reader off
            //test reader
            //List<string> stringListFromFiles2 = new List<string>();
            //FileIteratorStringOnly loadParameter2 = new FileIteratorStringOnly();
            //stringListFromFiles2 = loadParameter2.loadStrings(@"D:\PNNL CSharp\0_BatchFiles\ServerParameterFileSN09a.txt");
            #endregion

          //  FileTarget target = new FileTarget();
          //  target = FileTarget.WorkStandardTest;//<---used to target what computer we are working on:  Home or Work.  Server is covered by command line

            Part1Net35 newPart1Wrapper = new Part1Net35();
            
            InputOutputFileName newFile;// = new InputOutputFileName();
            SimpleWorkflowParameters Parameters;// = new SimpleWorkflowParameters();
            List<ElutingPeakOmics> discoveredOmicsPeaks = newPart1Wrapper.RunMe(args, out newFile, out Parameters);

            GoTransformParameters transformerParameterSetup = new GoTransformParameters();

            string SQLiteFileName = "SimpleWorkFlowPart2Multi";
            string SQLiteFolder = newFile.OutputSQLFileName;

            Object databaseLock1 = new Object();
            TransformerObject transformer1 = new TransformerObject(transformerParameterSetup,SQLiteFileName,SQLiteFolder, databaseLock1);

            int elutingpeakHits = Part2Net35.GoPart2Omics(newFile, Parameters, discoveredOmicsPeaks, transformer1);


            if (5 == 6)
            {
                #region old crap

      //          #region real code

      //          //setup main parameter file from args
      //          List<string> mainParameterFile = new List<string>();
      //          #region switch from server to desktop based on number of args
      //          if (args.Length == 0)//debugging
      //          {
      //              mainParameterFile.Add(""); mainParameterFile.Add(""); mainParameterFile.Add("");
      //          }
      //          else
      //          {
      //              Console.WriteLine("ParseArgs");
      //              ParseStrings parser = new ParseStrings();
      //              mainParameterFile = parser.ParseArgs(args);
      //          }   

      //          switch (target)
      //          {
      //              #region fileswitch
      //              case FileTarget.WorkStandardTest://work
      //                  {
      //                      //loaded from pre build events
      //                  }
      //                  break;
      //              case FileTarget.HomeStandardTest:
      //                  {
      //                      mainParameterFile[0] = @"G:\PNNL Files\PNNL CSharp\0_BatchFiles\0_HomeParameterFileSN09a.txt";
      //                      mainParameterFile[1] = @"G:\PNNL Files\CSharp\GetPeaksOutput\TextBatchResult";
      //                      mainParameterFile[2] = @"G:\PNNL Files\CSharp\GetPeaksOutput\SQLiteBatchResult";
      //                  }
      //                  break;
      //               #endregion
      //          }
      //          #endregion

      //          //load parameters
      //          List<string> stringListFromParameterFile = new List<string>();
      //          FileIteratorStringOnly loadParameter = new FileIteratorStringOnly();
      //          stringListFromParameterFile = loadParameter.loadStrings(mainParameterFile[0]);

      //          #region convert parameter file to variables
      //          string serverDataFileName = stringListFromParameterFile[0];
      //          string folderID = stringListFromParameterFile[1];
      //          int startScan = Convert.ToInt32(stringListFromParameterFile[2]);
      //          int endScan = Convert.ToInt32(stringListFromParameterFile[3]);
      //          int serverBlockTotal = Convert.ToInt32(stringListFromParameterFile[4]);
      //          int serverBlock = Convert.ToInt32(stringListFromParameterFile[5]);
      //          double DataSpecificMassNeutron = Convert.ToDouble(stringListFromParameterFile[6]);
      //          double part1SN = Convert.ToDouble(stringListFromParameterFile[7]);
      //          double part2SN = Convert.ToDouble(stringListFromParameterFile[8]);
      //          string DeconType = stringListFromParameterFile[9];

      //          //part1SN = 1;
      //          DeconvolutionType loadedDeconvolutionType = new DeconvolutionType();
      //          ConvertAToB converter = new ConvertAToB();
      //          loadedDeconvolutionType = converter.stringTODeconvolutionType(DeconType);

      //          MemorySplitObject newMemorySplitter = new MemorySplitObject();
      //          newMemorySplitter.NumberOfBlocks = serverBlockTotal;
      //          newMemorySplitter.BlockNumber = serverBlock;
      //          #endregion

      //          //setup files to load and save
      //          InputOutputFileName newFile = new InputOutputFileName();
      //          newFile.InputFileName = serverDataFileName;
      //          newFile.OutputFileName = mainParameterFile[1] + folderID + "_" + serverBlock + @".txt";
      //          newFile.OutputSQLFileName = mainParameterFile[2] + folderID + @".db";

      //          //Console.WriteLine("InputFileName: " + newFile.InputFileName +"\n");
      //          Console.WriteLine("SQLite Output: " + newFile.OutputSQLFileName + "\n");

      //          Console.WriteLine("Startscan: " + startScan + " EndScan: " + endScan);
      //          Console.WriteLine("Part1 SN: " + part1SN + " Part2 SN: " + part2SN);
      //          Console.WriteLine("DataSpecificMassNeutron: " + DataSpecificMassNeutron);
      //          Console.WriteLine("Processing Block# " + (newMemorySplitter.BlockNumber + 1).ToString() + " of " + newMemorySplitter.NumberOfBlocks);
      //          Console.WriteLine("Press Enter");
      //          //Console.ReadKey();


      //          FileInfo fi = new FileInfo(newFile.InputFileName);
      //          bool exists = fi.Exists;

      //          Console.WriteLine("CreateRun: " + newFile.InputFileName + " and its existance is " + exists.ToString());
      //          RunFactory rf = new RunFactory();
      //          Console.WriteLine("RunFactory Setup, press enter to continue");
      //          //Console.ReadKey();

      //          Run run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, newFile.InputFileName);
      //          //Run run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.Finnigan, inputDataFilename);
                
      //          #region Setup Parameters for complete data set to find eluting peaks

      //          Console.WriteLine("CreateController");
      //          SimpleWorkflowParameters parameters = new SimpleWorkflowParameters();

      //          //part 1 peak detector decontools only.  this gets overwritten below
      //          parameters.Part1Parameters.MSPeakDetectorPeakBR = 1.3;
      //          parameters.Part1Parameters.ElutingPeakNoiseThreshold = 2;
      //          parameters.Part2Parameters.MSPeakDetectorPeakBR = 1.3;
      //          parameters.Part2Parameters.MSPeakDetectorSigNoise = 2;

      //          ElutingPeakFinderController controller = new ElutingPeakFinderController(run, parameters);
      //          controller.Parameters.FileNameUsed = newFile.InputFileName;
                
      //         //set here for omics typical run is 3
      //          //controller.Parameters.Part1Parameters.ElutingPeakNoiseThreshold = 3;//data 1 when NoiseRemoved, take 3 sigma off before the orbitrap filter
      //          controller.Parameters.Part1Parameters.ElutingPeakNoiseThreshold = part1SN;//when NoiseRemoved, take 3 sigma off before the orbitrap filter
      //          controller.Parameters.Part1Parameters.NoiseType = InstrumentDataNoiseType.NoiseRemoved;
      //          controller.Parameters.Part1Parameters.PeakFitType = DeconTools.Backend.Globals.PeakFitType.LORENTZIAN;//this makes a large difference in the number of peaks
      //          controller.Parameters.Part1Parameters.StartScan = startScan;
      //          controller.Parameters.Part1Parameters.StopScan = endScan;
      //          controller.Parameters.Part1Parameters.MaxHeightForNewPeak = 0.75;
      //          controller.Parameters.Part1Parameters.AllignmentToleranceInPPM = 15;
      //          controller.Parameters.Part1Parameters.ParametersOrbitrap.massNeutron = DataSpecificMassNeutron;
      //          controller.Parameters.Part1Parameters.ParametersOrbitrap.DeltaMassTollerancePPM = 6000;

      //          //set here for omics  typical run is 10.5
      //          //controller.Parameters.Part2Parameters.MSPeakDetectorSigNoise = 10.5;//run 1
      //          controller.Parameters.Part2Parameters.MSPeakDetectorSigNoise = part2SN;//run 1
      //          controller.Parameters.Part2Parameters.NoiseType = InstrumentDataNoiseType.Standard;
      //          controller.Parameters.Part2Parameters.PeakFitType = DeconTools.Backend.Globals.PeakFitType.LORENTZIAN;//this makes a large difference in the number of peaks
      //          controller.Parameters.Part2Parameters.DeconvolutionType = loadedDeconvolutionType;
      //          controller.Parameters.Part2Parameters.Multithread = false;
      //          controller.Parameters.Part2Parameters.DynamicRangeToOne = 300000;
      //          controller.Parameters.Part2Parameters.MaxScanSpread = 500;
      //          controller.Parameters.Part2Parameters.ParametersOrbitrap.massNeutron = DataSpecificMassNeutron;
      //          controller.Parameters.Part2Parameters.ParametersOrbitrap.DeltaMassTollerancePPM = 6000;
      //          controller.Parameters.Part2Parameters.MemoryDivider = newMemorySplitter;

      //          //controller.Parameters.Part2Parameters.DeconvolutionType = DeconvolutionType.Thrash; 

      //          parameters.ConsistancyCrossErrorPPM = 20;
      //          #endregion

      //          #region Run Program

      //          //finds eluting peaks and calculates the start and stop scans
      //          //controller.SimpleWorkflowExecutePart1(run);

      //          List<ElutingPeak> discoveredPeaks = controller.SimpleWorkflowExecutePart1(run);
      //          //sums the spectra and deisotopes along the spine
      //         // controller.SimpleWorkflowExecutePart2(run, newFile.InputFileName, newFile.OutputSQLFileName);
      //          SimplifiedRun SKRun = new SimplifiedRun();
      //          SKRun.CurrentScanSet = run.CurrentScanSet;

      //          //current
      //          //discoveredPeaks = discoveredPeaks.OrderBy(p => p.Mass).ToList();
      //          //controller.SimpleWorkflowExecutePart2b(discoveredPeaks, newFile.InputFileName, newFile.OutputSQLFileName);


               


      //          ElutingPeakFinderPart2 EPPart2 = new ElutingPeakFinderPart2();
      //          EPPart2.Parameters = controller.Parameters;

      //          int elutingpeakHits = 0;
      //          EPPart2.SimpleWorkflowExecutePart2d(discoveredPeaks, newFile.InputFileName, newFile.OutputSQLFileName, ref elutingpeakHits);


      //          #region method C
      //          //discoveredPeaks = discoveredPeaks.OrderBy(p => p.Mass).ToList();

      //          //int inr = 0;
      //          //foreach (ElutingPeak ePeak in discoveredPeaks)
      //          //{
      //          //    Console.WriteLine("V2 eluting Peak " + (inr).ToString() + " out of " + discoveredPeaks.Count.ToString());

      //          //    RunFactory rf2 = new RunFactory();
      //          //    Run run2 = rf2.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, newFile.InputFileName);
      //          //    using (ElutingPeakFinderController controller2 = new ElutingPeakFinderController(run2, parameters))
      //          //    {
      //          //        controller2.Parameters.FileNameUsed = newFile.InputFileName;
      //          //        controller2.Parameters.Part2Parameters.MSPeakDetectorSigNoise = part2SN;//run 1
      //          //        controller2.Parameters.Part2Parameters.NoiseType = InstrumentDataNoiseType.Standard;
      //          //        controller2.Parameters.Part2Parameters.PeakFitType = DeconTools.Backend.Globals.PeakFitType.LORENTZIAN;//this makes a large difference in the number of peaks
      //          //        controller2.Parameters.Part2Parameters.DeconvolutionType = loadedDeconvolutionType;
      //          //        controller2.Parameters.Part2Parameters.Multithread = false;
      //          //        controller2.Parameters.Part2Parameters.DynamicRangeToOne = 300000;
      //          //        controller2.Parameters.Part2Parameters.MaxScanSpread = 500;
      //          //        controller2.Parameters.Part2Parameters.ParametersOrbitrap.massNeutron = DataSpecificMassNeutron;
      //          //        controller2.Parameters.Part2Parameters.ParametersOrbitrap.DeltaMassTollerancePPM = 6000;
      //          //        controller2.Parameters.Part2Parameters.MemoryDivider = newMemorySplitter;

      //          //        controller2.SimpleWorkflowExecutePart2c(ePeak, newFile.InputFileName, newFile.OutputSQLFileName);
      //          //        controller2.Dispose();
      //          //    }

      //          //    inr++;
      //          //}
      //          #endregion

      //          #endregion

      //          //timesitCleanedUp = GC.CollectionCount(0);
      //          //Console.WriteLine(timesitCleanedUp);

      //          #region Summarize test results
      ////          CrossCorrelateResults organzeResults = new CrossCorrelateResults();
      ////          organzeResults.summarize(run.ResultCollection.ElutingPeakCollection, newFile.OutputFileName, parameters.ConsistancyCrossErrorPPM, parameters);
      //          #endregion

      //          #endregion

               
      //          Console.WriteLine(elutingpeakHits + " hits were written to the database");
                #endregion
            }//end big iff
            stopWatch.Stop();

            System.DateTime stoptime = DateTime.Now;
            Console.WriteLine("This started at " + starttime + " and ended at" +stoptime);
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to find and assign features in eluting peaks");
            Console.Write("Finished.  Press Return to Exit"); 
            Console.ReadKey();
        }

       

        

        #region private functions
        ///// <summary>
        ///// read in a parameter file name from the batch file.  seperate arguments by a space
        ///// </summary>
        ///// <param name="args"></param>
        //private static List<string> ParseArgs(string[] args)
        //{
        //    List<string> paramatersStrings = new List<string>();
        //    paramatersStrings.Add("");
        //    paramatersStrings.Add("");
        //    paramatersStrings.Add("");

        //    string[] words = { };
        //    string argument1 = args[0];
        //    Console.WriteLine(args[1]);
        //    words = argument1.Split(Environment.NewLine.ToCharArray(),  //'\n', '\r'
        //        StringSplitOptions.RemoveEmptyEntries);

        //    int countArguments = 0;
        //    foreach (string argument in args)
        //    {
        //        //Console.WriteLine(argument);
        //        countArguments++;
        //    }
        //    if (countArguments == 3)
        //    {
        //        paramatersStrings[0] = args[0];
        //        paramatersStrings[1] = args[1];
        //        paramatersStrings[2] = args[2];
        //    }
        //    else
        //    {
        //        Console.WriteLine("MissingArguments.  There are: ", countArguments);
        //        Console.ReadKey();
        //    }
        //    //string fileIn = args[countArguments - 1];

        //    Console.WriteLine("Ther are " + countArguments + " arguments");
        //    Console.WriteLine("Our file is " + paramatersStrings[0]);

        //    FileInfo serverfi = new FileInfo(paramatersStrings[0]);
        //    bool serverFileExists = serverfi.Exists;

        //    Console.WriteLine("Does the parameter file exists? " + serverFileExists);
        //    //Console.ReadKey();

        //    return paramatersStrings;
        //}

        ///// <summary>
        ///// Various environments for working
        ///// </summary>
        //private enum FileTarget
        //{
        //    WorkStandardTest,
        //    HomeStandardTest,
        //}

        //private static void writeInputFile(string ServerFileName, string ID)
        //{
            

        //    //string ID = "SN09";
        //    int scanend = 3700;
        //    string outputFileDestination = "";
        //    //write data to file
        //    StringBuilder sb = new StringBuilder();
        //    outputFileDestination = @"D:\PNNL CSharp\0_BatchFiles\ServerParameterFile" + ID + @"a.txt";
        //    using (StreamWriter writer = new StreamWriter(outputFileDestination))
        //    {
        //        #region part 1a

        //        sb = new StringBuilder();
        //        sb.Append("data file," + ServerFileName + ", raw yafms data file\n");
        //        sb.Append("folderID," + ID + ", identifier that is added into the folder name and onto the files\n");
        //        sb.Append("startScan,0, first scan to start with\n");
        //        sb.Append("endScan," + scanend + ", last scan to use\n");
        //        sb.Append("number of serverblocks in total,4, unique block of data to run 4\n");
        //        sb.Append("serverblock,0, unique block of data to run 0,1,2,3\n");
        //        sb.Append("Mass neutron data specific,1.002149286, the difference between monoisotopic peak and c13 etc");
        //        writer.WriteLine(sb.ToString());

        //        #endregion
        //    }

        //    StringBuilder sb1 = new StringBuilder();
        //    outputFileDestination = @"D:\PNNL CSharp\0_BatchFiles\ServerParameterFile" + ID + @"b.txt";
        //    using (StreamWriter writer = new StreamWriter(outputFileDestination))
        //    {
        //        #region part 2b

        //        sb1 = new StringBuilder();
        //        sb1.Append("data file," + ServerFileName + ", raw yafms data file\n");
        //        sb1.Append("folderID," + ID + ", identifier that is added into the folder name and onto the files\n");
        //        sb1.Append("startScan,0, first scan to start with\n");
        //        sb1.Append("endScan," + scanend + ", last scan to use\n");
        //        sb1.Append("number of serverblocks in total,4, unique block of data to run 4\n");
        //        sb1.Append("serverblock,1, unique block of data to run 0,1,2,3\n");
        //        sb1.Append("Mass neutron data specific,1.002149286, the difference between monoisotopic peak and c13 etc");
        //        writer.WriteLine(sb1.ToString());
   
        //        #endregion
        //    }

        //    StringBuilder sb2 = new StringBuilder();
        //    outputFileDestination = @"D:\PNNL CSharp\0_BatchFiles\ServerParameterFile" + ID + @"c.txt";
        //    using (StreamWriter writer = new StreamWriter(outputFileDestination))
        //    {
        //        #region part 3c

        //        sb2 = new StringBuilder();
        //        sb2.Append("data file," + ServerFileName + ", raw yafms data file\n");
        //        sb2.Append("folderID," + ID + ", identifier that is added into the folder name and onto the files\n");
        //        sb2.Append("startScan,0, first scan to start with\n");
        //        sb2.Append("endScan," + scanend + ", last scan to use\n");
        //        sb2.Append("number of serverblocks in total,4, unique block of data to run 4\n");
        //        sb2.Append("serverblock,2, unique block of data to run 0,1,2,3\n");
        //        sb2.Append("Mass neutron data specific,1.002149286, the difference between monoisotopic peak and c13 etc");
        //        writer.WriteLine(sb2.ToString());

        //        #endregion
        //    }

        //    StringBuilder sb3 = new StringBuilder();
        //    outputFileDestination = @"D:\PNNL CSharp\0_BatchFiles\ServerParameterFile" + ID + @"d.txt";
        //    using (StreamWriter writer = new StreamWriter(outputFileDestination))
        //    {
        //        #region part 4d

        //        sb3 = new StringBuilder();
        //        sb3.Append("data file," + ServerFileName + ", raw yafms data file\n");
        //        sb3.Append("folderID," + ID + ", identifier that is added into the folder name and onto the files\n");
        //        sb3.Append("startScan,0, first scan to start with\n");
        //        sb3.Append("endScan," + scanend + ", last scan to use\n");
        //        sb3.Append("number of serverblocks in total,4, unique block of data to run 4\n");
        //        sb3.Append("serverblock,3, unique block of data to run 0,1,2,3\n");
        //        sb3.Append("Mass neutron data specific,1.002149286, the difference between monoisotopic peak and c13 etc");
        //        writer.WriteLine(sb3.ToString());

        //        #endregion
        //    }

        //}

        ///// <summary>
        ///// convert a string from the parameter file to the internal enummeration of deconvolution type
        ///// </summary>
        ///// <param name="DeconType"></param>
        ///// <returns></returns>
        //private static DeconvolutionType stringTODeconvolutionType(string DeconType)
        //{
        //    DeconvolutionType loadedDeconvolutionType = new DeconvolutionType();
        //    switch (DeconType)
        //    {
        //        case "THRASH":
        //            {
        //                loadedDeconvolutionType = DeconvolutionType.Thrash;
        //            }
        //            break;
        //        case "RAPID":
        //            {
        //                loadedDeconvolutionType = DeconvolutionType.Rapid;
        //            }
        //            break;
        //        default:
        //            {
        //                Console.WriteLine("Missing Decon Type.  Use THRASH or RAPID");
        //                Console.ReadKey();
        //            }
        //            break;
        //    }
        //    return loadedDeconvolutionType;
        //}
        #endregion
    }

    
 
}
