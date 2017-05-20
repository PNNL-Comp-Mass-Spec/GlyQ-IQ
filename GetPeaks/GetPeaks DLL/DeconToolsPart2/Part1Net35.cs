using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL;
using GetPeaks_DLL.Objects;
using System.IO;
using DeconTools.Backend.Runs;
using DeconTools.Backend.Core;
using MemoryOverloadProfilierX86;
using GetPeaks_DLL.ConosleUtilities;
using PNNLOmics.Algorithms.PeakDetection;
using GetPeaks_DLL.Go_Decon_Modules;
using GetPeaks_DLL.Objects.ParameterObjects;

namespace GetPeaks_DLL.DeconToolsPart2
{
    public class Part1Net35
    {
        public List<ElutingPeakOmics> RunMe(string[] args, out InputOutputFileName newFile, out SimpleWorkflowParameters parameters)
        {
            List<ElutingPeakOmics> discoveredOmicsPeaks = new List<ElutingPeakOmics>();

            FileTarget target = new FileTarget();
            target = FileTarget.WorkStandardTest;//<---used to target what computer we are working on:  Home or Work.  Server is covered by command line
            
            List<string> mainParameterFile;
            List<string> stringListFromParameterFile;
            ArgsProcessing(args, target, out mainParameterFile, out stringListFromParameterFile);

            BatchFileGetPeaksParameterObject parametersFromOutside = AssignParameters(stringListFromParameterFile);//convert parameter file to variables

            newFile = SetupNewFileObject(mainParameterFile, parametersFromOutside);
            FileInfo fi = new FileInfo(newFile.InputFileName);
            bool exists = fi.Exists;
            Console.WriteLine("CreateRun: " + newFile.InputFileName + " and its existance is " + exists.ToString());
            
            //this contains the first call to Decon tools Engine V2
            Run run = GoCreateRun.CreateRun(newFile);//core run for part 1

            parameters = GetPeaksParameterSetup.ParameterRoundHouse(mainParameterFile, parametersFromOutside, newFile, run);
            ElutingPeakFinderController controller = new ElutingPeakFinderController(run, parameters);

            #region Run Program

            //finds eluting peaks and calculates the start and stop scans
            Console.WriteLine("before Part 1");
            List<ElutingPeak> discoveredPeaks = controller.SimpleWorkflowExecutePart1(run);
            
            //discoveredPeaks = discoveredPeaks.OrderBy(p => p.Mass).ToList();
            
            ConvertToOmics newConverter = new ConvertToOmics();
            discoveredOmicsPeaks = newConverter.ConvertElutingPeakToElutingPeakOmics(discoveredPeaks);
        
            #endregion

            return discoveredOmicsPeaks;
        }

       
        public static InputOutputFileName SetupNewFileObject(List<string> mainParameterFile, BatchFileGetPeaksParameterObject parametersFromOutside)
        {
            InputOutputFileName newFile;
            newFile = new InputOutputFileName();
            newFile.InputFileName = parametersFromOutside.serverDataFileName;
            newFile.OutputFileName = mainParameterFile[1] + parametersFromOutside.folderID + "_" + parametersFromOutside.serverBlock + @".txt";
            newFile.OutputSQLFileName = mainParameterFile[2] + parametersFromOutside.folderID + @".db";

            newFile.OutputPath = mainParameterFile[1];
            return newFile;
        }

        public static BatchFileGetPeaksParameterObject AssignParameters(List<string> stringListFromParameterFile)
        {
            BatchFileGetPeaksParameterObject parametersFromOutside = new BatchFileGetPeaksParameterObject();

            parametersFromOutside.version = stringListFromParameterFile[0];
            parametersFromOutside.serverDataFileName = stringListFromParameterFile[1];
            parametersFromOutside.folderID = stringListFromParameterFile[2];
            parametersFromOutside.startScan = Convert.ToInt32(stringListFromParameterFile[3]);
            parametersFromOutside.endScan = Convert.ToInt32(stringListFromParameterFile[4]);
            parametersFromOutside.serverBlockTotal = Convert.ToInt32(stringListFromParameterFile[5]);
            parametersFromOutside.serverBlock = Convert.ToInt32(stringListFromParameterFile[6]);
            parametersFromOutside.DataSpecificMassNeutron = Convert.ToDouble(stringListFromParameterFile[7]);
            parametersFromOutside.part1SN = Convert.ToDouble(stringListFromParameterFile[8]);
            parametersFromOutside.part2SN = Convert.ToDouble(stringListFromParameterFile[9]);
            parametersFromOutside.DeconType = stringListFromParameterFile[10];
            parametersFromOutside.numberOfDeconvolutionThreads = Convert.ToInt32(stringListFromParameterFile[11]);
            parametersFromOutside.part1ScansToSum = Convert.ToInt32(stringListFromParameterFile[12]);
            parametersFromOutside.sumMethod = stringListFromParameterFile[13];

            return parametersFromOutside;
        }

        public static void ArgsProcessing(string[] args, FileTarget target, out List<string> mainParameterFile, out List<string> stringListFromParameterFile)
        {
            mainParameterFile = new List<string>();

            #region switch from server to desktop based on number of args
            if (args.Length == 0)//debugging
            {
                mainParameterFile.Add(""); mainParameterFile.Add(""); mainParameterFile.Add("");
            }
            else
            {
                Console.WriteLine("ParseArgs");
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

            if (stringListFromParameterFile.Count != 14)
            {
                Console.WriteLine("Parameter file is missing parameters");
                Console.ReadKey();
            }
        }

    }
}
