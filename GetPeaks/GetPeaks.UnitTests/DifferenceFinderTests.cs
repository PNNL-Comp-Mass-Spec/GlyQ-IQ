using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Diagnostics;
using GetPeaks_DLL.Functions;
using PNNLOmics.Data.Constants;
using PNNLOmics.Data;
using GetPeaks_DLL.Objects.DifferenceFinderObjects;
using GetPeaks_DLL.Objects;

namespace GetPeaks.UnitTests
{
    class DifferenceFinderTests
    {
        [Test]
        public void FindDifferencesTest()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            System.DateTime starttime = DateTime.Now;

            List<string> errorLog = new List<string>();
            errorLog.Add("start");

            
            List<double> data = new List<double>(new double[] { 0, 2, 4, 12, 16, 26, 30 });

            //for (double u = 0; u < 10000; u++)
            //{
            //    data.Add(31 + u);
            //}

            List<double> differences = new List<double>(new double[] { 10, 4, 7});
            double ppmTolerance = 10;

            DifferenceFinder finder = new DifferenceFinder();
            List<DifferenceObject<double>> results = new List<DifferenceObject<double>>();

            results = finder.FindDifferences(differences, ref data, ppmTolerance);

            stopWatch.Stop();
            System.DateTime stoptime = DateTime.Now;
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to find " + results.Count + " differences");
            Console.WriteLine("");

            Assert.AreEqual(4, results.Count);

            Assert.AreEqual(1, results[0].IndexData);
            Assert.AreEqual(3, results[0].IndexMatch);
            Assert.AreEqual(2, results[0].Value1);
            Assert.AreEqual(12, results[0].Value2);

            Assert.AreEqual(4, results[1].IndexData);
            Assert.AreEqual(5, results[1].IndexMatch);
            Assert.AreEqual(16, results[1].Value1);
            Assert.AreEqual(26, results[1].Value2);

            Assert.AreEqual(3, results[2].IndexData);
            Assert.AreEqual(4, results[2].IndexMatch);
            Assert.AreEqual(12, results[2].Value1);
            Assert.AreEqual(16, results[2].Value2);

            Assert.AreEqual(5, results[3].IndexData);
            Assert.AreEqual(6, results[3].IndexMatch);
            Assert.AreEqual(26, results[3].Value1);
            Assert.AreEqual(30, results[3].Value2);


            List<XYData> newXYList = new List<XYData>();
           
            newXYList.Add(new XYData(100,500));
            newXYList.Add(new XYData(200,500));
            newXYList.Add(new XYData(300,500));

            List<double> doubles = finder.ConvertXYDataToDouble(newXYList);
        }

        [Test]
        public void FindDifferencesTestReal()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            System.DateTime starttime = DateTime.Now;

            List<string> errorLog = new List<string>();
            errorLog.Add("start");


            List<double> data = new List<double>();
            data.Add(100);
            data.Add(200);
            data.Add(100 + Constants.Monosaccharides[MonosaccharideName.Deoxyhexose].MassMonoIsotopic);
            data.Add(300);
            data.Add(200 + Constants.Monosaccharides[MonosaccharideName.Hexose].MassMonoIsotopic);
            data.Add(400);
            data.Add(300 + Constants.Monosaccharides[MonosaccharideName.HexuronicAcid].MassMonoIsotopic);
            data.Add(500);
            data.Add(600);
            data.Add(400 + Constants.Monosaccharides[MonosaccharideName.NAcetylhexosamine].MassMonoIsotopic);
            data.Add(500 + Constants.Monosaccharides[MonosaccharideName.NeuraminicAcid].MassMonoIsotopic);
            data.Add(600 + Constants.Monosaccharides[MonosaccharideName.NGlycolylneuraminicAcid].MassMonoIsotopic);


            //for (double u = 0; u < 10000; u++)
            //{
            //    data.Add(31 + u);
            //}
            
            List<double> differences = new List<double>();
            differences.Add(Constants.Monosaccharides[MonosaccharideName.Deoxyhexose].MassMonoIsotopic);
            differences.Add(Constants.Monosaccharides[MonosaccharideName.Hexose].MassMonoIsotopic);
            differences.Add(Constants.Monosaccharides[MonosaccharideName.HexuronicAcid].MassMonoIsotopic);
            differences.Add(Constants.Monosaccharides[MonosaccharideName.KDN].MassMonoIsotopic);
            differences.Add(Constants.Monosaccharides[MonosaccharideName.NAcetylhexosamine].MassMonoIsotopic);
            differences.Add(Constants.Monosaccharides[MonosaccharideName.NeuraminicAcid].MassMonoIsotopic);
            differences.Add(Constants.Monosaccharides[MonosaccharideName.NGlycolylneuraminicAcid].MassMonoIsotopic);
            differences.Add(Constants.Monosaccharides[MonosaccharideName.Pentose].MassMonoIsotopic);

            double ppmTolerance = 10;

            DifferenceFinder finder = new DifferenceFinder();
            List<DifferenceObject<double>> results = new List<DifferenceObject<double>>();

            results = finder.FindDifferences(differences, ref data, ppmTolerance);

            stopWatch.Stop();
            System.DateTime stoptime = DateTime.Now;
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to find " + results.Count + " differences");
            Console.WriteLine("");

            Assert.AreEqual(6, results.Count);

            Assert.AreEqual(0, results[0].IndexData);
            Assert.AreEqual(2, results[0].IndexMatch);
            Assert.AreEqual(100.0d, results[0].Value1);
            Assert.AreEqual(246.05790880890001d, results[0].Value2);

            Assert.AreEqual(1, results[1].IndexData);
            Assert.AreEqual(4, results[1].IndexMatch);
            Assert.AreEqual(200.0d, results[1].Value1);
            Assert.AreEqual(362.05282343122502d, results[1].Value2);
        }

        [Test]
        public void FindDifferencesTestIsos()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            System.DateTime starttime = DateTime.Now;

            List<string> errorLog = new List<string>();
            errorLog.Add("start");

            List<double> differences = new List<double>();
            differences.Add(Constants.Monosaccharides[MonosaccharideName.Deoxyhexose].MassMonoIsotopic);
            differences.Add(Constants.Monosaccharides[MonosaccharideName.Hexose].MassMonoIsotopic);
            differences.Add(Constants.Monosaccharides[MonosaccharideName.HexuronicAcid].MassMonoIsotopic);
            differences.Add(Constants.Monosaccharides[MonosaccharideName.KDN].MassMonoIsotopic);
            differences.Add(Constants.Monosaccharides[MonosaccharideName.NAcetylhexosamine].MassMonoIsotopic);
            differences.Add(Constants.Monosaccharides[MonosaccharideName.NeuraminicAcid].MassMonoIsotopic);
            differences.Add(Constants.Monosaccharides[MonosaccharideName.NGlycolylneuraminicAcid].MassMonoIsotopic);
            differences.Add(Constants.Monosaccharides[MonosaccharideName.Pentose].MassMonoIsotopic);

            List<IsosObject> peaksFromScan = new List<IsosObject>();
            peaksFromScan.Add(new IsosObject()); peaksFromScan[0].scan_num = 1;  peaksFromScan[0].monoisotopic_mw  = 25;
            peaksFromScan.Add(new IsosObject()); peaksFromScan[1].scan_num = 2;  peaksFromScan[1].monoisotopic_mw  = 75;
            peaksFromScan.Add(new IsosObject()); peaksFromScan[2].scan_num = 3;  peaksFromScan[2].monoisotopic_mw  = 90;
            peaksFromScan.Add(new IsosObject()); peaksFromScan[3].scan_num = 4;  peaksFromScan[3].monoisotopic_mw  = 95;
            peaksFromScan.Add(new IsosObject()); peaksFromScan[4].scan_num = 5;  peaksFromScan[4].monoisotopic_mw  = 1000;
            peaksFromScan.Add(new IsosObject()); peaksFromScan[5].scan_num = 5;  peaksFromScan[5].monoisotopic_mw  = 1000 + Constants.Monosaccharides[MonosaccharideName.Deoxyhexose].MassMonoIsotopic;
            peaksFromScan.Add(new IsosObject()); peaksFromScan[6].scan_num = 5;  peaksFromScan[6].monoisotopic_mw  = 2000;
            peaksFromScan.Add(new IsosObject()); peaksFromScan[7].scan_num = 5;  peaksFromScan[7].monoisotopic_mw  = 2000 + Constants.Monosaccharides[MonosaccharideName.Hexose].MassMonoIsotopic;
            peaksFromScan.Add(new IsosObject()); peaksFromScan[8].scan_num = 5;  peaksFromScan[8].monoisotopic_mw  = 3000;
            peaksFromScan.Add(new IsosObject()); peaksFromScan[9].scan_num = 5;  peaksFromScan[9].monoisotopic_mw  = 3000 + 1*Constants.Monosaccharides[MonosaccharideName.NeuraminicAcid].MassMonoIsotopic;
            peaksFromScan.Add(new IsosObject()); peaksFromScan[10].scan_num = 5; peaksFromScan[10].monoisotopic_mw = 3000 + 2*Constants.Monosaccharides[MonosaccharideName.NeuraminicAcid].MassMonoIsotopic;
            peaksFromScan.Add(new IsosObject()); peaksFromScan[11].scan_num = 6; peaksFromScan[11].monoisotopic_mw = 4000;
            peaksFromScan.Add(new IsosObject()); peaksFromScan[12].scan_num = 7; peaksFromScan[12].monoisotopic_mw = 5000;

            List<IsosObject> extractMSfromOneScan = (from n in peaksFromScan where n.scan_num == 5 select n).ToList();

            //convert to doubles
            List<double> data = new List<double>();
            foreach(IsosObject n in extractMSfromOneScan)
            {
                data.Add(n.monoisotopic_mw);
            }

            double ppmTolerance = 10;

            DifferenceFinder finder = new DifferenceFinder();
            List<DifferenceObject<double>> differenceResults = new List<DifferenceObject<double>>();

            differenceResults = finder.FindDifferences(differences, ref data, ppmTolerance);

            stopWatch.Stop();
            System.DateTime stoptime = DateTime.Now;
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to find " + differenceResults.Count + " differences");
            Console.WriteLine("");

            Assert.AreEqual(4, differenceResults.Count);

            Assert.AreEqual(0, differenceResults[0].IndexData);
            Assert.AreEqual(1, differenceResults[0].IndexMatch);
            Assert.AreEqual(1000.0d, differenceResults[0].Value1);
            Assert.AreEqual(1146.0579088089d, differenceResults[0].Value2);

            Assert.AreEqual(2, differenceResults[1].IndexData);
            Assert.AreEqual(3, differenceResults[1].IndexMatch);
            Assert.AreEqual(2000.0d, differenceResults[1].Value1);
            Assert.AreEqual(2162.0528234312251d, differenceResults[1].Value2);

            Assert.AreEqual(4, differenceResults[2].IndexData);
            Assert.AreEqual(5, differenceResults[2].IndexMatch);
            Assert.AreEqual(3000.0d, differenceResults[2].Value1);
            Assert.AreEqual(3291.0954165293379, differenceResults[2].Value2);

            Assert.AreEqual(5, differenceResults[3].IndexData);
            Assert.AreEqual(6, differenceResults[3].IndexMatch);
            Assert.AreEqual(3291.0954165293379d, differenceResults[3].Value1);
            Assert.AreEqual(3582.1908330586762d, differenceResults[3].Value2);
        }
    }
}
