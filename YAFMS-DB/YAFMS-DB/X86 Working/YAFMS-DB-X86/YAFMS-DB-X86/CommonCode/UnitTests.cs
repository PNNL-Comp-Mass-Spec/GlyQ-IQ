using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using YAFMS_DB.Reader;

namespace YAFMS_DB
{
    public class UnitTests
    {
        //string testFolderGlobal = @"E:\ScottK\WorkingResults\";
        private string testFolderGlobal = @"D:\Csharp\ConosleApps\LocalServer\IQ\YAFMS-DB\100\";//default unit tests
        //private string testFolderGlobal = @"D:\Csharp\ConosleApps\LocalServer\IQ\YAFMS-DB\100";
        //private string testFolderGlobal = @"T:\ScottK\WorkingResults\";//9.2 vs 13
        //private string testFolderGlobal = @"Z:\ScottK\WorkingResults\";//10 vs 12sec
        //private string testFolderGlobal = @"E:\ScottK\WorkingResults\";//9.2 vs 13
        //private string testFolderGlobal = @"G:\ScottK\WorkingResults\";//9.2 vs 13//PIC
        //private string testFolderGlobal = @"C:\GlycolyzerData\";
        //string testFolderGlobal = @"L:\PNNL Files\";
        private string testFileDBGlobal = @"Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12_Sum5";
        //Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12_Sum5_(0).db
        //string testFileDBGlobal = @"YAFMS-DB_Test";
        //string testFolderGlobal = @"L:\PNNL Files\";
        //private string testFolderGlobal = @"Y:\ScottK\WorkingResults\";

        [Test]
        public void GetMS1ScanNumbers()
        {
            string testFolder = testFolderGlobal;
            string testFileDB = testFileDBGlobal;

            string databaseFile = testFolder + testFileDB + "_(0).db";
            //string databaseFile = testFolder + testFileDB + "_(V2).db";

            ReadDatabase reader = new ReadDatabase(databaseFile);
            int[] scansArray;
            reader.GetMs1ScanNumbers(out scansArray);

            Console.WriteLine(scansArray.Length);
            Assert.AreEqual(scansArray.Length, 100);

            Assert.AreEqual(scansArray[45], 66);
            Assert.AreEqual(scansArray[99], 138);
        }

        [Test]
        public void ReadAllSimplePeaksToMemory()
        {
            string testFolder = testFolderGlobal;
            string testFileDB = testFileDBGlobal;

            string databaseFile = testFolder + testFileDB + "_(0).db";
            //string databaseFile = testFolder + testFileDB + "_(V2).db";

            ReadDatabase reader = new ReadDatabase(databaseFile);
            int[] scansArray;
            reader.GetMs1ScanNumbers(out scansArray);

            Console.WriteLine("There are " + scansArray.Length + " Scans" + Environment.NewLine);


            ICollection<PeakArrays> peakPile = new Collection<PeakArrays>();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            System.DateTime starttime = DateTime.Now;



            //for(int i=0;i< scans.Count;i++)
            for (int i = 0; i < 10; i++)
            {
                int scan = scansArray[i];

                double[] mzArray;
                double[] intensityArray;
                double[] widthArray;
                reader.GetPeaksSpectrum(scan, out mzArray, out intensityArray, out widthArray);

                Console.WriteLine("index is " + i + " and scan is " + scan);
                Console.WriteLine("Mass Spec is is " + intensityArray.Length + " points long");
            }

            System.DateTime stoptime = DateTime.Now;
            Console.WriteLine("This started at " + starttime + " and ended at" + stoptime);
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to find " + 100 + " eluting peaks");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("finished with tandem, press return to end");
            Console.WriteLine("");

            Console.WriteLine("peakPile has " + peakPile.Count + " spectra");
        }

        [Test]
        public void ReadAllProcessedPeaksToMemory()
        {
            string testFolder = testFolderGlobal;
            string testFileDB = testFileDBGlobal;

            string databaseFile = testFolder + testFileDB + "_(0).db";
            //string databaseFile = testFolder + testFileDB + "_(V2).db";

            ReadDatabase reader = new ReadDatabase(databaseFile);
            int[] scansArray;
            reader.GetMs1ScanNumbers(out scansArray);

            Console.WriteLine("There are " + scansArray.Length + " Scans" + Environment.NewLine);


            List<PeakArrays> peakPile = new List<PeakArrays>();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            System.DateTime starttime = DateTime.Now;



            //for(int i=0;i< scans.Count;i++)
            for (int i = 0; i < 10; i++)
            {
                int scan = scansArray[i];

                double[] mzArray;
                double[] intensityArray;
                double[] widthArray;
                reader.GetProcessedPeaksSpectrum(scan, out mzArray, out intensityArray, out widthArray);

                PeakArrays results = new PeakArrays(intensityArray.Length, EnumerationPeaksArrays.MS);
                results.IntensityArray = intensityArray;
                results.MzArray = mzArray;
                results.WidthArray = widthArray;

                peakPile.Add(results);

                Console.WriteLine("index is " + i + " and scan is " + scan);
                Console.WriteLine("Mass Spec is is " + intensityArray.Length + " points long");
            }

            Assert.AreEqual(peakPile.Count, 10);
            Assert.AreEqual(peakPile[5].IntensityArray[4], 1163.230224609375d);
            Assert.AreEqual(peakPile[7].MzArray[50], 413.26638793945312d);
            Assert.AreEqual(peakPile[8].WidthArray[25], 0.0048428145237267017d);


            System.DateTime stoptime = DateTime.Now;
            Console.WriteLine("This started at " + starttime + " and ended at" + stoptime);
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to find " + 100 + " eluting peaks");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("finished with tandem, press return to end");
            Console.WriteLine("");

            Console.WriteLine("peakPile has " + peakPile.Count + " spectra");
        }

        [Test]
        public void ReadSimplePeakEic()
        {
            string testFolder = testFolderGlobal;
            string testFileDb = testFileDBGlobal;

            string databaseFile = testFolder + testFileDb + "_(0).db";
            //string databaseFile = testFolder + testFileDB + "_(V2).db";
           
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            System.DateTime starttime = DateTime.Now;

            int iterations = 10;
            ReadDatabase reader = new ReadDatabase(databaseFile);

            double delta = 0.01;
            double massStart = 1000;

            for (int i = 0; i < iterations; i++)
            {
                double mass = massStart + i * 50;

                int[] scans;
                double[] intensities;
                reader.GetEIC(mass, delta, out scans, out intensities);

                Console.WriteLine("EIC is is " + mass + " mz and " + intensities.Length + " points long");

                if(i==3)
                {
                    Assert.AreEqual(intensities.Length,324);
                    Assert.AreEqual(intensities[121], 194.46208190917969d);
                }
            }

            System.DateTime stoptime = DateTime.Now;
            Console.WriteLine("This started at " + starttime + " and ended at" + stoptime);
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to find " + iterations + " eic");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("finished with tandem, press return to end");
            Console.WriteLine("");

            
        }

        [Test]
        public void ReadThresholdedPeakEic()
        {
            string testFolder = testFolderGlobal;
            string testFileDb = testFileDBGlobal;

            string databaseFile = testFolder + testFileDb + "_(0).db";
            //string databaseFile = testFolder + testFileDB + "_(V2).db";

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            System.DateTime starttime = DateTime.Now;

            int iterations = 10;
            ReadDatabase reader = new ReadDatabase(databaseFile);

            double delta = 0.01;
            double massStart = 1000;

            for (int i = 0; i < iterations; i++)
            {
                double mass = massStart + i * 50;

                int[] scans;
                double[] intensities;
                reader.GetEICFromThresholdedData(mass, delta, out scans, out intensities);

                if (i == 3)
                {
                    Assert.AreEqual(intensities.Length, 324);
                    Assert.AreEqual(intensities[121], 194.46208190917969d);
                }

                Console.WriteLine("EIC is is " + mass + " mz and " + intensities.Length + " points long");
            }

            System.DateTime stoptime = DateTime.Now;
            Console.WriteLine("This started at " + starttime + " and ended at" + stoptime);
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to find " + iterations + " eic");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("finished with tandem, press return to end");
            Console.WriteLine("");


        }

        
        public void ParalellReadAllSimplePeaksToMemory(int enginesIn = 8, int threads = 8, bool multithreading = false)
        {
            string testFolder = testFolderGlobal;
            string testFileDB = testFileDBGlobal;

            bool multithread = multithreading;

            List<ReadDatabase> readers = new List<ReadDatabase>();
            for (int readerIndex = 0; readerIndex < 8; readerIndex++)
            {
                string databaseFile = testFolder + testFileDB + "_(" +readerIndex +").db";
                ReadDatabase reader = new ReadDatabase(databaseFile);
                readers.Add(reader);
            }

            int[] scansArray;
            readers[0].GetMs1ScanNumbers(out scansArray);

            Console.WriteLine("There are " + scansArray.Length + " Scans" + Environment.NewLine);

            if (multithread)
            {
                Console.WriteLine("Multithread with " + enginesIn + " engines and " + threads + " threads");
            }
            else
            {
                Console.WriteLine("Single Thread");
            }

            Console.WriteLine("Reading...");



            //ICollection<PeakArrays> peakPile = new Collection<PeakArrays>();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            System.DateTime starttime = DateTime.Now;

            object myLock = new object();
            //SortedDictionary<int, double[]> results = new SortedDictionary<int, double[]>();

            int counter = 0;

            //int maxScan = 100;//100 for unit testing
            //int maxScan = 1000;//100 for unit testing
            int maxScan = scansArray.Length;
            int engines = enginesIn;
            int maxNumberOfThreadsToUse = threads;
            
            ConcurrentBag<Tuple<int,PeakArrays>> results = new ConcurrentBag<Tuple<int, PeakArrays>>();

            

            if (multithread)
            {
                System.Threading.Tasks.Parallel.For(0, maxScan, new ParallelOptions { MaxDegreeOfParallelism = maxNumberOfThreadsToUse }, i =>
                {
                    //counter = 0;
                    MultiRead(scansArray, i, ref counter, readers, myLock, results, engines);
                });
            }
            else
            {
                for (int i = 0; i < maxScan; i++)
                {
                    counter = 0;
                    MultiRead(scansArray, i, ref counter, readers, myLock, results, engines);
                }
            }
            
            //});

            //}

            double sum = 0;
            double sumMasses = 0;
            foreach (var doublese in results)
            {
                //Console.WriteLine(doublese.Key);
                //sum += doublese.Key;
                sum += doublese.Item1;

                sumMasses += doublese.Item2.MzArray[0];
                sumMasses += doublese.Item2.MzArray[doublese.Item2.MzArray.Length - 1];
            }

            //Assert.AreEqual(1081164, sum);//1000
            //Assert.AreEqual(2271928.645111084d, sumMasses);//1000
            //Assert.AreEqual(7142, sum);//100
            //Assert.AreEqual(227483.56823730469d, sumMasses);//100

            System.DateTime stoptime = DateTime.Now;
            Console.WriteLine("This started at " + starttime + " and ended at" + stoptime);
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to find " + 100 + " eluting peaks");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("finished with tandem, press return to end");
            Console.WriteLine("");

            //Console.WriteLine("peakPile has " + peakPile.Count + " spectra");
        }

        //private static void MultiRead(int[] scansArray, int i, ref int counter, List<ReadDatabase> readers, object myLock, SortedDictionary<int, double[]> results, int engines)
        private static void MultiRead(int[] scansArray, int i, ref int counter, List<ReadDatabase> readers, object myLock, ConcurrentBag<Tuple<int, PeakArrays>> results, int engines)
        {
            int scan = scansArray[i];
            
            double[] mzArray;
            double[] intensityArray;
            double[] widthArray;

            
            //Console.WriteLine("reader" + counter);
            readers[counter].GetPeaksSpectrum(scan, out mzArray, out intensityArray, out widthArray);
            LockedAdd(ref counter, myLock, results, engines, scan, mzArray);
            
        }

        //private static void LockedAdd(ref int counter, object myLock, SortedDictionary<int, double[]> results, int engines, int scan,double[] mzArray)
        private static void LockedAdd(ref int counter, object myLock, ConcurrentBag<Tuple<int, PeakArrays>> results, int engines, int scan, double[] mzArray)
         {
            //lock (myLock)
            //{
                PeakArrays thisResult = new PeakArrays(mzArray.Length,EnumerationPeaksArrays.Peak);
                thisResult.MzArray = mzArray;
                results.Add(new Tuple<int, PeakArrays>(scan, thisResult));
                //Console.WriteLine("results added");
                counter++;
                if (counter == engines) counter = 0;

                int largeer = 100000000;
                largeer = 1;
                for (int i = 0; i < largeer; i++)
                {
                    double large = 4*4.333;
                    decimal evenLarger = (decimal) large*(decimal) large *i;

                }
                //Console.WriteLine("index is " + i + " and scan is " + scan);
                //Console.WriteLine("Mass Spec is is " + intensityArray.Length + " points long");
            //}
        }
    }
}

//    SELECT Min(TP.scanid) AS 'TP_ScanID', 
//       TS.scannumlc   AS 'TS_ScanNumLc', 
//       Max(TP.height) AS 'TP_Height' 
//FROM   t_peak_centric TP 
//       INNER JOIN t_scan_centric TS 
//               ON TP.scanid = TS.scanid 
//WHERE  TP.mz > 280 
//       AND TP.mz < 282 
//GROUP  BY TS.scannumlc;  



// while (continueTrying) {
//    retval = sqlite_exec(db, sqlQuery, callback, 0, &msg);
//    switch (retval) {
//      case SQLITE_BUSY:
//        Log("[%s] SQLITE_BUSY: sleeping fow a while...", threadName);
//        sleep a bit... (use something like sleep(), for example)
//        break;
//      case SQLITE_OK:
//        continueTrying = NO; // We're done
//        break;
//      default:
//        Log("[%s] Can't execute \"%s\": %s\n", threadName, sqlQuery, msg);
//        continueTrying = NO;
//        break;
//    }
//  }
//You may also want to try busy_timeout parameter on connection string as shown here and here.


//The busy_timeout parameter is implemented as a call to sqlite(3)_busy_timeout. The default value is 0, which means to throw a SqliteBusyException immediately if the database is locked.


