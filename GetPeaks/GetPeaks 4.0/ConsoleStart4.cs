using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL;
using System.Diagnostics;
using GetPeaks_DLL.Objects;
using MemoryOverloadProfilierX86;
using GetPeaks_DLL.DeconToolsPart2;

namespace GetPeaks40
{
    class ConsoleStart4
    {
        /// <summary>
        /// Work Debug args:  "D:\PNNL CSharp\0_BatchFiles\0_WorkParameterFileSTD.txt" "D:\Csharp\GetPeaksOutput" "D:\Csharp\GetPeaksOutput\SQLiteBatchResult"
        /// Work Debug args:  "D:\PNNL CSharp\0_BatchFiles\0_WorkParameterFileSTDMemory.txt" "D:\Csharp\GetPeaksOutput" "D:\Csharp\GetPeaksOutput\SQLiteBatchResult"
        /// Work Debug args:  "D:\PNNL CSharp\0_BatchFiles\0_WorkParameterFileSN09.txt" "D:\Csharp\GetPeaksOutput" "D:\Csharp\GetPeaksOutput\SQLiteBatchResult"
        /// Work Debug args:  "D:\PNNL CSharp\0_BatchFiles\0_WorkParameterFileCell.txt" "D:\Csharp\GetPeaksOutput" "D:\Csharp\GetPeaksOutput\SQLiteBatchResult"
        ///// Work Debug args: "D:\PNNL CSharp\0_BatchFiles\0_WorkParameterFileCell.txt" "D:\Csharp\GetPeaksOutput" "V:\SQLiteBatchResult"
        
        /// Home Debug args:  "G:\PNNL Files\PNNL CSharp\0_BatchFiles\0_HomeParameterFileSTD.txt" "G:\PNNL Files\Csharp\GetPeaksOutput" "G:\PNNL Files\Csharp\GetPeaksOutput\SQLiteBatchResult"
        /// Home Debug args:  "G:\PNNL Files\PNNL CSharp\0_BatchFiles\0_HomeParameterFileSTD_RAM.txt" "G:\PNNL Files\Csharp\GetPeaksOutput" "X:\RAM GetPeaksOutput\RAMResult"
        /// Home Debug args:  "G:\PNNL Files\PNNL CSharp\0_BatchFiles\0_HomeParameterFileSN09_RAM.txt" "G:\PNNL Files\Csharp\GetPeaksOutput" "X:\RAM GetPeaksOutput\RAMResult"
        ///Home Debug args:  "L:\PNNL Files\PNNL CSharp\0_BatchFiles\0_HomeParameterFileSN29_RAM.txt" "L:\PNNL Files\Csharp\GetPeaksOutput" "V:\RAMResult"
        ///                
        /// 
        /// //TODO there is a bug where the number of eluting peaks and features do not equal.  may be caused by locked database!
        /// 
        /// 
        /// "C:\JetBrains\0_WorkParameterFileSN09.txt" "E:\GetPeaksExport\GetPeaksOutput" "E:\GetPeaksExport\SQLiteBatchResult"
        ///<?xml version="1.0"?>
        ///<configuration><startup useLegacyV2RuntimeActivationPolicy="true"><supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.0"/></startup></configuration>
        ///
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("\nFind Peaks");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            System.DateTime starttime = DateTime.Now;

            List<string> errorLog = new List<string>();
            errorLog.Add("start");

            Part1Net35 newPart1Wrapper = new Part1Net35();

            InputOutputFileName newFile;// = new InputOutputFileName();
            SimpleWorkflowParameters Parameters;// = new SimpleWorkflowParameters();
            List<ElutingPeakOmics> discoveredOmicsPeaks = newPart1Wrapper.RunMe(args, out newFile, out Parameters);

            int elutingpeakHits = 0;
            
            //int elutingpeakHits = Part2Net40.MultiThreadDiscovery(newFile, Parameters, discoveredOmicsPeaks);
           bool multithread = true;
           switch (multithread)
           {
               case true:
                   {
                       Part2Net40 newPart40 = new Part2Net40();
                       newPart40.Parameters = Parameters;
                       //elutingpeakHits = Part2Net40.MultiThreadDiscovery(newFile, Parameters, discoveredOmicsPeaks);
                       elutingpeakHits = newPart40.MultiThreadDiscovery(newFile, discoveredOmicsPeaks, errorLog);
                   }
                   break;
               case false:
                   {
                       Part2Net40 newPart40 = new Part2Net40();
                       newPart40.Parameters = Parameters;
                       //elutingpeakHits = Part2Net40.GoPart2Omics(newFile, Parameters, discoveredOmicsPeaks);
                       elutingpeakHits = newPart40.GoPart2Omics(newFile, discoveredOmicsPeaks);
                   }
                   break;
           }



            stopWatch.Stop();

            Profiler newProfiler = new Profiler();
            newProfiler.printMemory("All done!");

            int errorCount = 0;
            for (int i = 0; i < errorLog.Count; i++)
            {
                if (errorLog[i] != "start")
                {
                    Console.WriteLine(errorLog[i]);
                    errorCount++;
                }
            }
            Console.WriteLine("");
            Console.WriteLine("There are " + errorCount + " errors");
            Console.WriteLine("");
            System.DateTime stoptime = DateTime.Now;
            Console.WriteLine("This started at " + starttime + " and ended at" + stoptime);
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to find " +elutingpeakHits+ " eluting peaks");
            Console.WriteLine("");
            Console.WriteLine("The file " + newFile.OutputSQLFileName + " has been written");
            Console.WriteLine("");
            Console.Write("Finished.  Press Return to Exit");
            //Console.ReadKey();
        }
    }


}
