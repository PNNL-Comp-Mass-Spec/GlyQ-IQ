using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL;
using DeconToolsPart2;
using System.Threading.Tasks;
using System.Threading;
using MemoryOverloadProfilierX86;
using GetPeaks_DLL.Go_Decon_Modules;

namespace GetPeaks_4._0
{
    public class Part2Net40
    {
        public SimpleWorkflowParameters Parameters { get; set; }
        
        //public static int GoPart2Omics(InputOutputFileName newFile, SimpleWorkflowParameters Parameters, List<ElutingPeakOmics> discoveredPeaks)
        public int GoPart2Omics(InputOutputFileName newFile, List<ElutingPeakOmics> discoveredPeaks)
        {
            SimpleWorkflowParameters cloneParameters = ObjectCopier.Clone<SimpleWorkflowParameters>(Parameters);

            ElutingPeakFinderPart2 EPPart2 = new ElutingPeakFinderPart2();

            //TODO 3.   Setup Parameters in GetPeaks 4.0
            //EPPart2.Parameters = Parameters;
            GoTransformParameters transformerParameterSetup = new GoTransformParameters();

            string SQLiteFileName = "SimpleWorkFlowPart2e";
            string SQLiteFolder = newFile.OutputSQLFileName;

            Object databaseLockOmics = new Object();
            TransformerObject transformer1 = new TransformerObject(transformerParameterSetup, SQLiteFileName, SQLiteFolder, databaseLockOmics);

            int elutingpeakHits = 0;
            EPPart2.SimpleWorkflowExecutePart2e(discoveredPeaks, newFile, cloneParameters, ref elutingpeakHits, transformer1);
            //EPPart2.SimpleWorkflowExecutePart2Memory(discoveredPeaks, newFile.InputFileName, newFile.OutputSQLFileName, cloneParameters, ref elutingpeakHits);
            
            return elutingpeakHits;
        }

        private Object tranformLock = new Object();

        #region crap
        //http://www.softwareverify.com/software-verify-blog/?cat=11
        //http://www.codeproject.com/KB/mcpp/cppclidtors.aspx
        //public static int MultiThreadDiscovery(InputOutputFileName newFile, SimpleWorkflowParameters Parameters, List<ElutingPeakOmics> discoveredPeaks)
        #endregion

        public int MultiThreadDiscovery(InputOutputFileName newFile, List<ElutingPeakOmics> discoveredPeaks, List<string> errorLog)
        {
            int elutingPeakTotal = 0;

            Object databaseLockMulti = new Object();//global lock for database

            Profiler newProfiler = new Profiler();
            newProfiler.printMemory("Start");
           
            //int maxNumberOfThreadsToUse = 8;//1 is default
            int maxNumberOfThreadsToUse = Parameters.Part2Parameters.numberOfDeconvolutionThreads;
            int sizeOfTransformerArmy = maxNumberOfThreadsToUse * 2+1;//+1 is for emergencies
            GoTransformParameters transformerParameterSetup = new GoTransformParameters();
            HornTransformParameters hornParameters = new HornTransformParameters();
            transformerParameterSetup.Parameters = hornParameters;

            GoTransformPrep setupTransformers = new GoTransformPrep();
            List<TransformerObject> transformerList = setupTransformers.PreparArmyOfTransformers(transformerParameterSetup, sizeOfTransformerArmy, newFile, databaseLockMulti);

            int threadToggle = 0;
            
            int counter = 0; 
            int hits = 0;
            int totalCount = discoveredPeaks.Count;
            int dataFileIterator = 0;

            Parallel.ForEach(discoveredPeaks, new ParallelOptions { MaxDegreeOfParallelism = maxNumberOfThreadsToUse }, ePeak =>
            {
                #region inside
                int threadName = System.Threading.Thread.CurrentThread.ManagedThreadId;
                Console.WriteLine("newThread" + threadName + "Peak " + counter + " out of " + totalCount + " with " + hits + " hits");

                TransformerObject transformerMulti;// = transformer2;

                lock (tranformLock)//assignes a transformer to a thread
                {
                    transformerMulti = AssignTransform(maxNumberOfThreadsToUse, transformerList, ref threadToggle);
                    counter++;
                }
                //Console.WriteLine("press enter"); //Console.ReadKey();
                ElutingPeakFinderPart2 EPPart2 = new ElutingPeakFinderPart2();
                //SimpleWorkflowParameters newpar = ObjectCopier.Clone<SimpleWorkflowParameters>(Parameters);
                //ElutingPeakOmics ThreadEPeak = ObjectCopier.Clone<ElutingPeakOmics>(ePeak);
                SimpleWorkflowParameters paralellParameters = new SimpleWorkflowParameters();
                paralellParameters = ObjectCopier.Clone<SimpleWorkflowParameters>(Parameters);
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
                //EPPart2.SimpleWorkflowExecutePart2e(newSingleList, newFile.InputFileName, newFile.OutputSQLFileName, ref elutingpeakHits);

                EPPart2.SimpleWorkflowExecutePart2Memory(newSingleList, newFile, paralellParameters, ref elutingpeakHits, transformerMulti, ref dataFileIterator);

                if (elutingpeakHits > 0)
                {
                    hits++;
                }

                paralellParameters.Dispose();
                paralellParameters = null;
                    //TODO memory leak.  perhaps need idisposable on single list inside
                newSingleList[0].Dispose();
                newSingleList = null;
                elutingPeakTotal += elutingpeakHits;

                EPPart2.Dispose();

                //this line comes from a custom Engine V2, not the standard Decontools dll
                //transformerMulti.TransformEngine.PercentDone = 0;
                transformerMulti.active = false;
                //newProfiler.printMemory("End of thread " + threadName);
                //Console.WriteLine("press enter"); Console.ReadKey();
                #endregion
            });


            foreach(TransformerObject tObject in transformerList)
            {
                foreach (string stringSelected in tObject.ErrorLog)
                {
                    errorLog.Add(stringSelected);
                }
            }
            return elutingPeakTotal;
        }

        ////TODO pull this out so we can use it publicly
        //public static List<TransformerObject> PreparArmyOfTransformers(GoTransformParameters transformerParameterSetup, int numberOfThreads, InputOutputFileName newFile, Object databaseLockMulti)
        //{
        //    List<TransformerObject> transformerList = new List<TransformerObject>();

        //    for (int i = 0; i < numberOfThreads; i++)
        //    {
        //        string SQLiteFileName = "SimpleWorkFlowPart2Multi_" + Convert.ToString(i); ;
        //        string SQLiteFolder = newFile.OutputPath;

        //        TransformerObject newTransformer = new TransformerObject(transformerParameterSetup, SQLiteFileName, SQLiteFolder, databaseLockMulti);
                
        //        transformerList.Add(newTransformer);
        //    }
            
        //    return transformerList;
        //}

        public static TransformerObject AssignTransform(int MaxThreadCount, List<TransformerObject> transformerList, ref int threadToggle)
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
            transformerMulti = transformerList[transformerList.Count-1];
            Console.WriteLine("not enough threads");
            Console.ReadKey();
            //possible fail safe is to add a new transformer to the list and use it
            return transformerMulti;
        }

    }
}
