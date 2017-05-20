using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Diagnostics;

//threading pattern
//http://msdn.microsoft.com/en-us/library/ts553s52.aspx


namespace ConsoleApplication1
{
    public class Accelerate
    {
        
        // in general, the paralell will never work because of the overlaps of multiple hits.  it is better to break up the problem higher up
        public void RunGO(List<DataSetSK> MSData)
        {
            //System.Timers.Timer accelerateTimer = new System.Timers.Timer();
            Stopwatch AccelerateStopWatch = new Stopwatch();
            AccelerateStopWatch.Start();
            
            double tollerance = 0.5;  //master tollerance

            bool isParalell;
            int localProcessors = 1;//default
            int i;

            //set up an object so we can add the result lists.  a result list will contain the indexes and masses
            CompareResultsSet compareResults = new CompareResultsSet();
            List<CompareResults> compareListOfResutls = new List<CompareResults>();
            compareResults.AddResults(compareListOfResutls);

            CompareResults finalResults = new CompareResults();

            //isParalell = true;
            isParalell = false;

            if (isParalell)
            {
                #region paralell version
                    Console.WriteLine("Run Paralell");
                    string processorsStr = System.Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS");
                    if (processorsStr != null)
                    {
                        localProcessors = int.Parse(processorsStr);
                    }
                    //localProcessors = 2;
                    Console.WriteLine("There are " + localProcessors + " Processors");
                
                    //set up data lists.  These lists are used for breaking up the data into pieces
                    //slow but with linq
                        //var dataMasses = (from n in MSData[0].XYList select (decimal)n.X).ToList();
                        //var libraryMasses = (from n in MSData[2].XYList select (decimal)n.X).ToList();
                
                    //faster but with iterating
                        List<decimal> dataMasses = new List<decimal>();
                        List<decimal> libraryMasses = new List<decimal>();

                        for (i = 0; i < MSData[0].XYList.Count; i++)
                        {
                            dataMasses.Add((decimal)MSData[0].XYList[i].X);//pull out front so we have a list of masses
                        }
                        for (i = 0; i < MSData[2].XYList.Count; i++)
                        {
                            libraryMasses.Add((decimal)MSData[2].XYList[i].X);//pull out front so we have a list of masses
                        }


                    //check to see if there are more cores than points.  Perhaps make it more than half the number of points...
                    if (dataMasses.Count < localProcessors)
                    {
                        localProcessors = dataMasses.Count/2;//if there are more cores than data points, reduce to total number of points
                    }

                    List<ListResults> dividedDataPieces = new List<ListResults>();
                    AccelerateBlockFX breaker = new AccelerateBlockFX();
                    //this sets up the data for each core
                    //breaker.breakupXY(dataMasses, localProcessors, dividedDataPieces);

                    List<LibraryIndexStartStop> lotsOfStartStopData = new List<LibraryIndexStartStop>();
                    for (i = 0; i < localProcessors; i++)
                    {
                        LibraryIndexStartStop startStopData = new LibraryIndexStartStop();
                        lotsOfStartStopData.Add(startStopData);
                    }

                    breaker.breakupXYData(dataMasses, localProcessors, dividedDataPieces, lotsOfStartStopData);

                    //stopWatch.Stop();
                    TimeSpan ts = AccelerateStopWatch.Elapsed;
                    Console.WriteLine("This took " + ts + "seconds");

                    List<LibraryIndexStartStop> lotsOfStartStop = new List<LibraryIndexStartStop>();
                    List<List<DataSetSK>> lotsOfMiniData = new List<List<DataSetSK>>();

                    for (i = 0; i < localProcessors; i++)
                    {
                        List<DataSetSK> miniMSData = new List<DataSetSK>();
                        LibraryIndexStartStop startStopLibrary = new LibraryIndexStartStop();

                        breaker.breakupXYLibrary(libraryMasses, startStopLibrary, i, dividedDataPieces, miniMSData); //this does not take into avvount seam erors
                        lotsOfMiniData.Add(miniMSData);
                        lotsOfStartStop.Add(startStopLibrary);
                    }

                    AccelerateStopWatch.Stop();
                    TimeSpan ts2 = AccelerateStopWatch.Elapsed;
                    Console.WriteLine("This took " + ts2 + "seconds");

                    //create threads
                    //4.83 sec//7.716
                    //Thread[] lotsOfThreads = new Thread[localProcessors];
                    List<Thread> lotsOfThreads = new List<Thread>();
                    for (i = 0; i < localProcessors; i++)
                    {
                        //calculate one break point at a time, then launch the thread  //FX (thread number, max threads)
                        //Console.WriteLine("i is " + i.ToString());
                        //lotsOfThreads[i] = new Thread(delegate() { GO(lotsOfMiniData[i], compareResults, i, tollerance); });
                        //lotsOfThreads[i].Start();

                        //alternate mehthod
                        AlternateMethod accelerater = new AlternateMethod();
                        accelerater.SetValues(lotsOfMiniData[i], compareResults, i, tollerance, lotsOfStartStopData[i].startIndex);
                        //accelerater.SetValues(MSData, compareResults, i);
                        ThreadStart threadDelegate = new ThreadStart(accelerater.GO2);
                        lotsOfThreads.Add(new Thread(threadDelegate));
                        lotsOfThreads[i].Start();
                    }

                    List<int> holdMatchA = new List<int>();
                    List<int> holdMatchB = new List<int>();
                    List<int> holdMatchAnotB = new List<int>();
                    List<int> holdMatchBnotA = new List<int>();

                    //stop the threads
                    for (i = 0; i < localProcessors; i++)
                    {
                        lotsOfThreads[i].Join();
                        Console.WriteLine("here i=" + i.ToString() +compareResults.resultsList[i].IndexName +" ");
                        //remap indexes
                        //append indexes
                        //sort to get final result

                    
                        //we need to increment the data by the startIndex
                        int k;
                        for (k = 0; k < compareResults.resultsList[i].IndexListAMatch.Count; k++)
                        {
                            compareResults.resultsList[i].IndexListAMatch[k] += compareResults.resultsList[i].startIndex;
                        }

                        for (k = 0; k < compareResults.resultsList[i].IndexListAandNotB.Count; k++)
                        {
                            compareResults.resultsList[i].IndexListAandNotB[k] += compareResults.resultsList[i].startIndex;
                        }

                        //append                    
                        holdMatchA.AddRange(compareResults.resultsList[i].IndexListAMatch);
                        holdMatchB.AddRange(compareResults.resultsList[i].IndexListBMatch);
                        holdMatchAnotB.AddRange(compareResults.resultsList[i].IndexListAandNotB);
                        holdMatchBnotA.AddRange(compareResults.resultsList[i].IndexListBandNotA);

                    
                    }

                    //sort
                    //send to arrays for the sort and then back up to lists
                    if (1 == 1)
                    {
                        int[] newMatchA = holdMatchA.ToArray();
                        int[] newMatchB = holdMatchB.ToArray();

                        Array.Sort(newMatchB, newMatchA);

                        Array.Sort(newMatchA, newMatchB);

                        List<int> newerListA = newMatchA.ToList<int>();
                        List<int> newerListB = newMatchB.ToList<int>();

                        finalResults.AddAandB(newerListA, newerListB);
                    }
                    else
                    {
                        IQuickSortIndex sorter = new QuickSort();//this is slower
                        sorter.SortInt2(ref holdMatchA, ref holdMatchB);

                        finalResults.AddAandB(holdMatchA, holdMatchB);
                    }
                    //simple lists sorts
                    holdMatchAnotB.Sort();
                    holdMatchBnotA.Sort();

                    //
                
                    finalResults.AddAandNotB(holdMatchAnotB);
                    finalResults.AddBandNotA(holdMatchBnotA);
                    finalResults.IndexName = "Multi Thread";
                #endregion
            }
            else
            {
                Console.WriteLine("No Paralell");//non paralell
                //GO(MSData, compareResults, localProcessors, tollerance);
                
                AlternateMethod accelerater = new AlternateMethod();
                accelerater.SetValues(MSData, compareResults, 1, tollerance, 0);
                accelerater.GO2();
                finalResults.AddAandB(compareResults.resultsList[0].IndexListAMatch, compareResults.resultsList[0].IndexListBMatch);
                finalResults.AddAandNotB(compareResults.resultsList[0].IndexListAandNotB);
                finalResults.AddBandNotA(compareResults.resultsList[0].IndexListBandNotA);
                finalResults.IndexName = "Single Thread";
                finalResults.tollerance = tollerance;
            }
        }
 
        private static void GO(List<DataSetSK> MSData, CompareResultsSet compareResults, int processor, double tollerance)//
        {    
            Console.WriteLine("Go started.....");
            CompareController newCompare = new CompareController();
            string name="processor "+Convert.ToString(processor);
            newCompare.compareSK(MSData, compareResults, name, tollerance, 0);//accepts an MSData
            Console.WriteLine("end go");
        }
     }

    public class AlternateMethod
    {
        private List<DataSetSK> m_MSData;
        private CompareResultsSet m_compareResults;
        private int m_processor;
        private int m_startIndex;
        private double m_tollerance;

        public void SetValues(List<DataSetSK> MSData, CompareResultsSet compareResults, int processor, double tollerance, int startIndex)
        {
            m_MSData = MSData;
            m_compareResults = compareResults;
            m_processor = processor;
            m_startIndex = startIndex;
            m_tollerance = tollerance;
            
        }

        public void GO2()
        {
            Console.WriteLine("Go2 started.....");
            CompareController newCompare = new CompareController();
            string name = "processor " + Convert.ToString(m_processor);
            newCompare.compareSK(m_MSData, m_compareResults, name, m_tollerance, m_startIndex);
            
            Console.WriteLine("end go");

           
        }
    }

}

