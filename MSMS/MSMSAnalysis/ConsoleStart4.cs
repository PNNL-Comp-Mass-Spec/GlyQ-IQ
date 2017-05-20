using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSMS_DLL;
using System.Diagnostics;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL;
using MemoryOverloadProfilierX86;
using PNNLOmics.Data;

namespace MSMSAnalysis
{
    class ConsoleStart4
    {

        //Work "D:\PNNL CSharp\0_BatchFiles\0_WorkParameterFileMSMS.txt" "D:\Csharp\GetPeaksOutput" "D:\Csharp\GetPeaksOutput\SQLiteBatchResult"
        //Work "D:\PNNL CSharp\0_BatchFiles\0_WorkParameterFileMSMS_SN25.txt" "D:\Csharp\GetPeaksOutput" "D:\Csharp\GetPeaksOutput\SQLiteBatchResult"
        //Work "D:\PNNL CSharp\0_BatchFiles\0_WorkParameterFileMSMS_SN32B.txt" "D:\Csharp\GetPeaksOutput" "D:\Csharp\GetPeaksOutput\SQLiteBatchResult"
        //Work "D:\PNNL CSharp\0_BatchFiles\0_WorkParameterFileMSMS_SN32B.txt" "D:\Csharp\GetPeaksOutput" "D:\Csharp\GetPeaksOutput\SQLiteBatchResult"
        //Work "D:\PNNL CSharp\0_BatchFiles\0_WorkParameterFileMSMS04.txt" "D:\Csharp\GetPeaksOutput" "D:\Csharp\GetPeaksOutput\SQLiteBatchResult"

        //Work "D:\PNNL CSharp\0_BatchFiles\0_WorkParameterFileMSMS_GlycoPeptide.txt" "D:\Csharp\GetPeaksOutput" "D:\Csharp\GetPeaksOutput\SQLiteBatchResult"
        
        //Home "G:\PNNL Files\PNNL CSharp\0_BatchFiles\0_HomeParameterFileMSMS04.txt" "G:\PNNL Files\CSharp\GetPeaksOutput" "G:\PNNL Files\CSharp\GetPeaksOutput\SQLiteBatchResult"
        //Home "G:\PNNL Files\PNNL CSharp\0_BatchFiles\0_HomeParameterFileMSMS_SN25.txt" "G:\PNNL Files\CSharp\GetPeaksOutput" "G:\PNNL Files\CSharp\GetPeaksOutput\SQLiteBatchResult"
        //Home "L:\PNNL Files\PNNL CSharp\0_BatchFiles\0_HomeParameterFileMSMS_GlycoPeptide.txt" "L:\PNNL Files\CSharp\GetPeaksOutput" "L:\PNNL Files\CSharp\GetPeaksOutput"

        static void Main(string[] args)
        {
            Console.WriteLine("\nFind Peaks");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            System.DateTime starttime = DateTime.Now;

            GetPeaksMSMSController newDataGenerator = new GetPeaksMSMSController();
            //go to disk and populate msms data and precursor m/z
            List<TandemObject> TandemScan = newDataGenerator.GetMonoPeaks(args);

            int scansCollected = TandemScan.Count;
            stopWatch.Stop();

            Profiler newProfiler = new Profiler();
            newProfiler.printMemory("All done!");

            System.DateTime stoptime = DateTime.Now;
            Console.WriteLine("This started at " + starttime + " and ended at" + stoptime);
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to collect and process " + scansCollected + " scans");
            Console.Write("Finished.  Press Return to Exit");
            Console.ReadKey();
        }
    }
}
