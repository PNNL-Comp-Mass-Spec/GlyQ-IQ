using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQGlyQ.FIFO;
using NUnit.Framework;

namespace IQGlyQ.UnitTesting
{
     
    class Import_UnitTest
    {
        [Test]
        public void LoadResults()
        {
            //string fileName = @"D:\Csharp\ConosleApps\LocalServer\IQ\GlyQ-IQ\UnitTest\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12_iqResults_777.txt";
            string fileName = @"D:\Csharp\ConosleApps\LocalServer\IQ\GlyQ-IQ\UnitTest\S_SN129_1_Family_iqResults.txt";
            ImportGlyQResult importer = new ImportGlyQResult();
            List<GlyQIqResult> results = importer.Import(fileName);

            foreach (var glyQIqResult in results)
            {
                Console.WriteLine(glyQIqResult.Code);
            }
           
            Assert.AreEqual(results.Count,130);

        }
    }
}
