using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InformedProteomics.Backend.MassSpecData;
using NUnit.Framework;

namespace Run64IP
{
   
    class IpRun64UnitTests
    {
        [Test]
        public void ScanSetTest()
        {
            Console.WriteLine("Testing X64");
            string filename = @"L:\PNNL Files\PNNL\Projects\GlyQ-IQ Paper\Part 1\Dta Nov\FE_S_SN129_1\ResultsSummary\S_SN129_1.raw";

            IMassSpecDataReader reader = new XCaliburReader(filename);

            LcMsRun myFirstRun = new LcMsRun(reader);

            Console.WriteLine("Ther are " + myFirstRun.MaxLcScan + " scans");
            //string filename = @"D:\Csharp\0_TestDataFiles\F_Std_S_SN129_1\RawData\S_SN129_1.raw";
            //RunFactory rf = new RunFactory();
            //DeconTools.Backend.Core.Run run = rf.CreateRun(filename);

            //run.ScanSetCollection = new ScanSetCollection();
            //run.ScanSetCollection.Create(run, 5, 1, false);

            //Console.WriteLine("Success!");
            //Assert.AreEqual(run.ScanSetCollection.ScanSetList[600].PrimaryScanNumber, 601);
        }
    }
}
