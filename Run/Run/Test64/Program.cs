using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Run64.Backend.Core;
using Run64.Backend.Runs;

namespace Test64
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing X64");
            string filename = @"L:\PNNL Files\PNNL\Projects\GlyQ-IQ Paper\Part 1\Dta Nov\FE_S_SN129_1\ResultsSummary\S_SN129_1.raw";
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
