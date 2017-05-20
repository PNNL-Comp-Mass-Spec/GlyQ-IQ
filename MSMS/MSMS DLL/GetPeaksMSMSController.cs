using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL;
using MemoryOverloadProfilierX86;
using GetPeaks_DLL.Go_Decon_Modules;
using DeconTools.Backend.Runs;
using DeconTools.Backend.Core;
using MSMS_DLL.Utilities;
using System.IO;

namespace MSMS_DLL
{
    public class GetPeaksMSMSController
    {
        public List<TandemObject> GetMonoPeaks(string[] args)
        {
            //set up variables
            InputOutputFileName newFile;// = new InputOutputFileName();
            int sizeOfDatabase;
            GetSizeOfDatabase(args, out newFile, out sizeOfDatabase);//newfile is setup here

            SimpleWorkflowParameters Parameters;// = new SimpleWorkflowParameters();
            
            LoadMSMSController loadDataDeconTOOLS = new LoadMSMSController();

            List<TandemObject> TandemScanList = new List<TandemObject>();

            ///set up multithreaded thrash engines.  one engine per thread
            
            //int maxNumberOfThreadsToUse = Parameters.Part2Parameters.numberOfDeconvolutionThreads;
            int maxNumberOfThreadsToUse = 1;//fix to one to start so we can keep it simple
            int sizeOfTransformerArmy = maxNumberOfThreadsToUse * 2 + 1;//+1 is for emergencies
            int threadToggle = 0;

            ///set up parameters for THRASH
            
            HornTransformParameters newHornParameters = new HornTransformParameters();
            newHornParameters.MinMZ = 100;
            GoTransformParameters transformerParameterSetup = new GoTransformParameters();
            transformerParameterSetup.Parameters = newHornParameters; 

            ///create Thrash engines
            Object databaseLockMulti = new Object();//global lock for database
            GoTransformPrep setupTransformers = new GoTransformPrep();
            //List<TransformerObject> transformerList = setupTransformers.PreparArmyOfTransformers(transformerParameterSetup, sizeOfTransformerArmy);
            List<TransformerObject> transformerList = setupTransformers.PreparArmyOfTransformers(transformerParameterSetup, sizeOfTransformerArmy, newFile, databaseLockMulti);


            TransformerObject transformerMulti;// = transformer2;//creates a place holder in a thread for the engine of interest          

            //Profiler newProfiler = new Profiler();
            //newProfiler.printMemory("Start");

            //1.  get size of database

            //2.  for each scan
            //{
            //3.  get XYData in form of TandemObject
            //List<TandemObject> TandemScan = loadDataDeconTOOLS.LoadData(args, out newFile, out Parameters);

            //start
            ///get size of database via creating a run, getting the size
            

           //n-1 so scan 600 in xcalibur is i=599
            for (int i = 0; i < sizeOfDatabase; i++)//cycle through scans
            {
                //newProfiler.printMemory("before XYData");

                //TODO can we filter based on the precursor mz being present so we don't need to procsses all ms1?
                int targetScan = i;
                TandemObject TandemScan = loadDataDeconTOOLS.LoadScan(args, newFile, out Parameters, targetScan, sizeOfDatabase);//newfile is updated here
                Parameters.Part2Parameters.ParametersOrbitrap.DeltaMassTollerancePPM = 6000;
                Parameters.Part2Parameters.isDataThresholded = false;//TODO when this is true, the S/N ratio is not stored
                Parameters.Part2Parameters.MSPeakDetectorSigNoise = 1;
                Parameters.Part2Parameters.NoiseType = PNNLOmics.Algorithms.PeakDetection.InstrumentDataNoiseType.NoiseRemoved;
                Parameters.Part2Parameters.DeconvolutionType = DeconvolutionType.Thrash;//Rapid has a memory leak

                transformerMulti = AssignTransform(maxNumberOfThreadsToUse, transformerList, ref threadToggle);//passed memory 6-23-11

                //4.  XYData-->Peaks-->MonoisotopicPeaks
                ApplyThrash thrasher = new ApplyThrash();
                thrasher.SimpleApplyThrash(ref TandemScan, Parameters, transformerMulti, Parameters.FileNameUsed);

                transformerMulti.active = false;

                ///for deleting tandem scan
                //TandemScan.Dispose();
                //TandemScan = null;

                //perhaps we should write the tandem monoisotoic peaks to a YAFMS file


                TandemScan.SpectraDataOMICS.Clear();
                TandemScanList.Add(TandemScan);
                Console.WriteLine("We have completed " + i.ToString() + " of " + sizeOfDatabase);
                //newProfiler.printMemory("after XYData is deleted");
            }//next scan
            //}
            //part 2 takes us to monosiotopic peaks
           


           
            int counter = 0;

            //int maxNumberOfThreadsToUse = 8;//1 is default
            counter++;


            return TandemScanList;
        }

        private static void GetSizeOfDatabase(string[] args, out InputOutputFileName newFile, out int sizeOfDatabase)
        {
            List<string> mainParameterFile;
            List<string> stringListFromParameterFile;
            ConvertArgsToStringList argsToList = new ConvertArgsToStringList();
            argsToList.Convert(args, out mainParameterFile, out stringListFromParameterFile);

            string serverDataFileName = stringListFromParameterFile[0];

            newFile = new InputOutputFileName();
            newFile.InputFileName = serverDataFileName;//.yafms
            //newFile.OutputFileName = mainParameterFile[1] + folderID + "_" + serverBlock + @".txt";
            //newFile.OutputSQLFileName = mainParameterFile[2] + folderID + @".db";

            FileInfo fi = new FileInfo(newFile.InputFileName);
            bool exists = fi.Exists;

            //Console.WriteLine("CreateRun: " + newFile.InputFileName + " and its existance is " + exists.ToString());
            
            RunFactory rf = new RunFactory();
            
            //Run run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, newFile.InputFileName);
            //Run run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.Finnigan, inputDataFilename);

            Run run = GoCreateRun.CreateRun(newFile);

            //Console.WriteLine("after run was created");

            sizeOfDatabase = run.GetNumMSScans() - 1;//this is a crude way to get the size of the database.  -1 is need or the scan set creation will fail
            run.Dispose();
            //Console.WriteLine("DeleteRun now that it is done");
            //end
        }

        //private static List<TransformerObject> PreparArmyOfTransformers(GoTransformParameters transformerParameterSetup, int numberOfThreads)
        //{
        //    List<TransformerObject> transformerList = new List<TransformerObject>();

        //    for (int i = 0; i < numberOfThreads; i++)
        //    {
        //        string SQLiteFileName = "SimpleWorkFlowPart2e";
        //        string SQLiteFolder = newFile.OutputSQLFileName;

        //        Object databaseLockOmics = new Object();
        //        TransformerObject newTransformer = new TransformerObject(transformerParameterSetup, SQLiteFileName, SQLiteFolder, databaseLockOmics);

                
        //        //TransformerObject newTransformer = new TransformerObject(transformerParameterSetup);
        //        transformerList.Add(newTransformer);
        //    }

        //    return transformerList;
        //}

        private static TransformerObject AssignTransform(int MaxThreadCount, List<TransformerObject> transformerList, ref int threadToggle)
        {

            int activeTransformers = 0;
            foreach (TransformerObject tObject in transformerList)
            {
                if (tObject.active == true)
                {
                    activeTransformers++;
                }
            }
            Console.WriteLine("there are " + activeTransformers + " present");

            //new stuff
            TransformerObject transformerMulti;
            int counter = 0;
            bool assigned = false;
            while (assigned == false)
            {
                if (transformerList[counter].active == false)
                {
                    transformerList[counter].active = true;
                    transformerMulti = transformerList[counter];
                    counter = 0;
                    return transformerMulti;
                }
                else
                {
                    counter++;
                }
            }
            //fail senario
            transformerMulti = transformerList[transformerList.Count - 1];
            Console.WriteLine("not enough threads");
            Console.ReadKey();
            //possible fail safe is to add a new transformer to the list and use it
            return transformerMulti;
        }


    }

   
}
