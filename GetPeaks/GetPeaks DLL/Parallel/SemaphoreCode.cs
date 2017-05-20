using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace GetPeaks_DLL.Parallel
{
    class SemaphoreCode
    {

        public Semaphore MySemaphore { get; set; }

        public List<ParalellEngine> Engines { get; set; }

        public List<ParalellEngine> AssignedEngineList { get; set; }

        public List<int> AssignedScan { get; set; }

        public int Scan { get; set; }

        public int NumberOfChances { get; set; }

        public ParalellEngine SelectedEngine { get; set; }

        public List<int> AssignedList { get; set; }

        public object SemaphoreLock { get; set; }

        public bool AssignedYet {get;set;}

        /// <summary>
        /// This important because the MainThread needs to be released from the semaphore
        /// </summary>
        public bool firstRelease { get; set; }

        //http://www.dijksterhuis.org/using-semaphores-in-c/




        //ParalellAssignEngine newSemaphore = new ParalellAssignEngine(Cores, semaforeLock, Engines);


        //Console.WriteLine("We allow " + eScan.ToString() + " into semaphore");
        //newSemaphore.AssignedYet = false;
        //newSemaphore.SelectedEngine = null;
        //newSemaphore.Run(eScan);

        //if (newSemaphore.AssignedEngineList.Count>0)
        //{
        //    transformerMulti = newSemaphore.AssignedEngineList[0];
        //    newSemaphore.AssignedEngineList[0].Active = false;
        //    newSemaphore.AssignedEngineList.RemoveAt(0);
        //    newSemaphore.MySemaphore.Release();
        //    counter++;
        //    Console.WriteLine("+++++++++++Transform Released_" + counter);
        //}







        public SemaphoreCode(int cores, object semaphoreLock, List<ParalellEngine> engines)
        {
            MySemaphore = new Semaphore(0, 1);    // Capacity of 3
            SemaphoreLock = semaphoreLock;
            Engines = engines;
            firstRelease = true;//initial condition in which the MainTHread has not passed the smaphore yet
            AssignedEngineList = new List<ParalellEngine>();
            AssignedScan = new List<int>();
        }


        public void Run(int scan)
        {

            Scan = scan;
            //Thread workThread = new Thread(Worker);
            //workThread.Name = scan.ToString();
            //workThread.Start(scan);

            lock (SemaphoreLock)
            {
                ParalellEngine freeEngine = Engines.FirstOrDefault(c => c.Active == false);

                if (freeEngine != null)
                {

                    Thread workThread = new Thread(Enter2);
                    workThread.Name = scan.ToString();
                    workThread.Start();
                }
            }
            //Console.WriteLine("Main Releases 1.");

            //this is immportant to relase the main thread
            if (firstRelease)
            {
                MySemaphore.Release(1);
                firstRelease = false;
            }
            Console.WriteLine("Main exits.");
        }

        //static void Enter(object id)
        void Enter()
        {
            string Name = Thread.CurrentThread.Name;
            Console.WriteLine("Thread {0} has started", Name);

            MySemaphore.WaitOne();

            ParalellEngine freeEngine = Engines.FirstOrDefault(c => c.Active == false);

            if (freeEngine != null)
            {
                Console.WriteLine("We Found an open Engine for Scan " + Scan);
                AssignedYet = true;

                SelectedEngine = freeEngine;//select from engines list
                SelectedEngine.Active = true;

                ParalellLog.LogAddScan(SelectedEngine, Scan);
                
                SelectedEngine.Scan = Scan;

                Console.WriteLine(Scan.ToString() + " is released for further processing");       // a time.

                Console.WriteLine(this.SelectedEngine.Scan.ToString() + " is ended");
                //AssignedList.Add(Scan);
                //SelectedEngine.Active = false;
                int prevCount = MySemaphore.Release();

                Console.WriteLine("Thread {0} released the semaphore, previously there were {1} slots open.", 
                    Name,//{0}
                    prevCount); // {1}
            }
            //else
            //{
            //    Console.WriteLine("All used up, " + Scan + " will wait for next one");
                    
            //    //MySemaphore.WaitOne();
            //    Thread.Sleep(2);//sends the signal to release
            //    //Console.WriteLine(Scan.ToString() + " is waiting till it can get in");           // Only three threads
                    
            //}
            //}
        }

        void Enter2()
        {
            string Name = Thread.CurrentThread.Name;
            Console.WriteLine("Thread {0} has started", Name);

            Console.WriteLine("addTo waitone");
            MySemaphore.WaitOne();

            Thread.Sleep(5);

            MySemaphore.Release();

        }
        private void Worker(object data)
        {
            // Obtain the thread name
            string Name = Thread.CurrentThread.Name;
            Console.WriteLine("Thread {0} has started", Name);

            int scan = (int)data;

            Console.WriteLine("Thread {0} is working on scan {1}", 
                Name, //0
                scan.ToString());//1
            
            //for (int Lp = 0; Lp < 1; Lp++)
            //{
                // This funtion is not critical -- anyone can enter
                NonCriticalSection(Name);


                CriticalSection(Name, scan);

                // We need to think about this for a bit -- some algortihm

                Thread.Sleep(5);

                // We have finished our job, so release the semaphore

                int prevCount = MySemaphore.Release();
                // Report on how many places are available

                //Console.WriteLine("Thread {0} released the semaphore, previously there were {1} slots open.",
                //    Name,   // {0}
                //    prevCount); // {1}
            //}

            Console.WriteLine("Thread {0} has finished", Name);
        }

        private void NonCriticalSection(string name)
        {
            Console.WriteLine("Thread {0} enters the non-critical section", name);
        }
        
        private void CriticalSection(string name, int scan)
        {
            Console.WriteLine("Thread {0} enters the critical section", name);
            //ParalellEngine miniEngine = new EngineTHRASH();
            MySemaphore.WaitOne();
            lock(SemaphoreLock)
            {
            ParalellEngine freeEngine = Engines.FirstOrDefault(c => c.Active == false);

            if (freeEngine != null)
            {
                
                // We need to aquire access to be able to enter the critical section
                
                Console.WriteLine("Thread {0} FreeEngine was found", name);
                //this.SelectedEngine = freeEngine;
                AssignedEngineList.Add(freeEngine);
                freeEngine.Active = true;
                //CriticalSection(name, scan);
            }
            else
            { }
            
            AssignedScan.Add(scan);
            }
        }

        private static ParalellEngine Assign(List<ParalellEngine> transformerList)
        {
            //http://msdn.microsoft.com/en-us/magazine/cc817398.aspx
            int activeTransformers = 0;
            foreach (ParalellEngine tObject in transformerList)
            {
                if (tObject.Active == true)
                {
                    activeTransformers++;
                }
            }
            Console.WriteLine("there are " + activeTransformers + " present");

            //new stuff
            ParalellEngine transformerMulti;
            int counter = 0;
            bool assigned = false;
            while (assigned == false)
            {
                if (transformerList[counter].Active == false)
                {
                    transformerList[counter].Active = true;
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
