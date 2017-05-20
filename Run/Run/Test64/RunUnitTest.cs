using System;
using NUnit.Framework;
using Run64.Backend.Core;
using Run64.Backend.Runs;

namespace Test64
{
    public class RunUnitTest
    {
        [Test]
        public void TestThermoGetMassSpectra()
        {
            Console.WriteLine("Testing X64");
            //string filename = @"L:\PNNL Files\PNNL\Projects\GlyQ-IQ Paper\Part 1\Dta Nov\FE_S_SN129_1\ResultsSummary\S_SN129_1.raw";
            string filename = @"D:\Csharp\0_TestDataFiles\F_Std_S_SN129_1\RawData\S_SN129_1.raw";
            RunFactory rf = new RunFactory();
            Run run = rf.CreateRun(filename);

            run.ScanSetCollection = new ScanSetCollection();
            run.ScanSetCollection.Create(run, 5, 1, false);
            run.XYData = run.GetMassSpectrum(run.ScanSetCollection.GetScanSet(501));

            Console.WriteLine(run.XYData.Xvalues.Length);
            Console.WriteLine("Success!");
            Assert.AreEqual(run.XYData.Xvalues.Length, 56711);
        }

        [Test]
        public void TestThermoScanSet()
        {
            Console.WriteLine("Testing X64");
            //string filename = @"L:\PNNL Files\PNNL\Projects\GlyQ-IQ Paper\Part 1\Dta Nov\FE_S_SN129_1\ResultsSummary\S_SN129_1.raw";
            string filename = @"D:\Csharp\0_TestDataFiles\F_Std_S_SN129_1\RawData\S_SN129_1.raw";
            RunFactory rf = new RunFactory();
            Run run = rf.CreateRun(filename);

            run.ScanSetCollection = new ScanSetCollection();
            run.ScanSetCollection.Create(run, 5, 1, false);
            
            Console.WriteLine("Success!");
            Assert.AreEqual(run.ScanSetCollection.ScanSetList[600].PrimaryScanNumber, 601);
        }
    }
}
