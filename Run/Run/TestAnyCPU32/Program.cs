using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Run32.Backend.Core;
using Run32.Backend.Runs;

namespace TestAnyCPU32
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing AnyCPU X86");
            //string filename = @"L:\PNNL Files\PNNL\Projects\GlyQ-IQ Paper\Part 1\Dta Nov\FE_S_SN129_1\ResultsSummary\S_SN129_1.raw";
            string filename = @"D:\Csharp\0_TestDataFiles\F_Std_S_SN129_1\RawData\S_SN129_1.raw";
            RunFactory rf = new RunFactory();
            Run run = rf.CreateRun(filename);

            run.ScanSetCollection = new ScanSetCollection();
            run.ScanSetCollection.Create(run, 5, 1, false);
            run.XYData = run.GetMassSpectrum(run.ScanSetCollection.GetScanSet(501));

            Console.WriteLine(run.XYData.Xvalues.Length);
            Console.WriteLine("Success!");
            Console.ReadKey();
        }
    }
}
