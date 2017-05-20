using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.Objects;
using NUnit.Framework;
using PNNLOmics.Data.Constants;

namespace GetPeaks.UnitTests
{
    public class GlycanRelationships
    {
        [Test]
        public void FindRelatinoshipsTestList()
        {
            List<MonosaccharideName> monosacharidesOfInterest = new List<MonosaccharideName>();
            monosacharidesOfInterest.Add(MonosaccharideName.Hexose);
            monosacharidesOfInterest.Add(MonosaccharideName.NAcetylhexosamine);
            monosacharidesOfInterest.Add(MonosaccharideName.Deoxyhexose);
            monosacharidesOfInterest.Add(MonosaccharideName.NeuraminicAcid);

            List<GlycanComposition> glycanCompositions = new List<GlycanComposition>();

            //load data
            GetPeaks_DLL.DataFIFO.StringLoadTextFileLine reader = new StringLoadTextFileLine();
            //string testFile = @"X:\Velos_SN129_GC.txt";
            //string testFile = @"X:\Velos_SN129_CGNV.txt";
            //string testFile = @"X:\SPIN_SN129.txt";
            //string testFile = @"X:\SPIN_Sn129H15_CG.txt";
            //string testFile = @"X:\SPIN_SN129H15_CGNV.txt";
            //string testFile = @"X:\SPIN_SN129H15avg_CGNV.txt";
            string testFile = @"X:\Velos_SN129_Hammer_L10PSA.txt";


            string testFileBase = Regex.Replace(testFile, @".txt" + @"$", String.Empty);
            string testFileOut = testFileBase + "_withRelationships.txt";

            Console.WriteLine(testFile);
            List<string> lines = reader.SingleFileByLine(testFile);

            foreach (var line in lines)
            {
                int code = Convert.ToInt32(line);
                List<int> composition = GlycanCodeToInts(code);
                GlycanComposition compareGlycan = new GlycanComposition(monosacharidesOfInterest, composition);
                glycanCompositions.Add(compareGlycan);
            }

            bool toPrint = true;
            if (toPrint)
            {
                CheckLoad(glycanCompositions);
            }

            foreach (var glycan in glycanCompositions)
            {
                //test each glycan individually
                GlycanComposition currentGlycan = glycan;
                
                foreach (var glycanToCompare in glycanCompositions)
                {
                    if (glycan != null) GlycanCompositionUtilities.SingleLinkCompositions(ref currentGlycan, glycanToCompare);

                    //bool hex = currentGlycan.Connetions[MonosaccharideName.Hexose];
                    //bool NAcetylhexosamine = currentGlycan.Connetions[MonosaccharideName.NAcetylhexosamine];
                    //bool Deoxyhexose = currentGlycan.Connetions[MonosaccharideName.Deoxyhexose];
                    //bool NeuraminicAcid = currentGlycan.Connetions[MonosaccharideName.NeuraminicAcid];
                    //Console.WriteLine(hex + " " + NAcetylhexosamine + " " + Deoxyhexose + " " + NeuraminicAcid);
                    //Console.WriteLine(Environment.NewLine);
                }

            }


            List<GlycanComposition> withRelationshps = glycanCompositions.Where(p => p.Contains1Connection.Equals(true)).ToList();

            List<string> pileOfResults = new List<string>();
            foreach (var glycanComposition in withRelationshps)
            {
                List<MonosaccharideName> keys = glycanComposition.Connetions.Keys.ToList();
                string writeLineTODisk = Convert.ToString(
                    Convert.ToInt32(glycanComposition.Compositions[keys[0]]) * 10000 + 
                    Convert.ToInt32(glycanComposition.Compositions[keys[1]]) * 1000 + 
                    Convert.ToInt32(glycanComposition.Compositions[keys[2]]) * 100 + 
                    Convert.ToInt32(glycanComposition.Compositions[keys[3]]) * 1);

                pileOfResults.Add(writeLineTODisk);

                string writelines = "glycan (" + 
                    glycanComposition.Compositions[keys[0]] + "," + 
                    glycanComposition.Compositions[keys[1]] + "," + 
                    glycanComposition.Compositions[keys[2]] + "," + 
                    glycanComposition.Compositions[keys[3]] + ") has ";

                foreach (var name in keys)
                {
                    bool connect = glycanComposition.Connetions[name];
                    if(connect)
                    {
                        writelines = writelines + name + "connected to it ";
                    }
                }
                if (toPrint)
                {
                    Console.WriteLine(writelines);
                }
            } 
            Console.WriteLine(" We Have " + withRelationshps.Count + " with relationships out of " + glycanCompositions.Count);

            StringListToDisk writer = new StringListToDisk();
            writer.toDiskStringList(testFileOut,pileOfResults);

        }

        private static void CheckLoad(List<GlycanComposition> glycanCompositions)
        {
            foreach (var glycan in glycanCompositions)
            {
                List<MonosaccharideName> keys = glycan.Compositions.Keys.ToList();
                string lineToPrint = "";
                for (int j = 0; j < keys.Count - 1; j++)
                {
                    lineToPrint = lineToPrint + glycan.Compositions[keys[j]] + ",";
                }
                lineToPrint += glycan.Compositions[keys[keys.Count - 1]];
                Console.WriteLine(lineToPrint);
            }
            Console.WriteLine(glycanCompositions.Count + " glycan compositions were loaded and parsed");
        }

        private static List<int> GlycanCSVToInts(string line)
        {
            string[] words = line.Split(',');
            List<int> composition = words.Select(x => Int32.Parse(x)).ToList();
            return composition;
        }

        private static List<int> GlycanDashToInts(string line)
        {
            string[] words = line.Split('-');
            List<int> composition = words.Select(x => Int32.Parse(x)).ToList();
            return composition;
        }

        private static List<int> GlycanCodeToInts(int code)
        {
            int hexMultiplier = 10000;
            int hexNAcMultipier = 1000;
            int fucMultiplier = 100;
            int sialicAcidMultplier = 1;
            
            double hexDouble = code/hexMultiplier; 
            int hex = Convert.ToInt32(Math.Truncate(hexDouble));

            double hexNAcDouble = (code - hex*hexMultiplier) / hexNAcMultipier;
            int hexNAc = Convert.ToInt32(Math.Truncate(hexNAcDouble));

            double fucDouble = (code - hex * hexMultiplier - hexNAc * hexNAcMultipier) / fucMultiplier;
            int fuc = Convert.ToInt32(Math.Truncate(fucDouble));

            double NeuAcDouble = (code - hex * hexMultiplier - hexNAc * hexNAcMultipier - fuc * fucMultiplier) / sialicAcidMultplier;
            int NeuAc = Convert.ToInt32(Math.Truncate(NeuAcDouble));

            List<int> composition = new List<int>();
            composition.Add(hex);
            composition.Add(hexNAc);
            composition.Add(fuc);
            composition.Add(NeuAc);

            return composition;
        }

        [Test]
        public void CodeConversionTest()
        {

            string line = "5,4,0,1";

            List<int> composition = GlycanCSVToInts(line);

            Assert.AreEqual(composition[0],5);
            Assert.AreEqual(composition[1], 4);
            Assert.AreEqual(composition[2], 0);
            Assert.AreEqual(composition[3], 1);

            int code = 54102;

            List<int> composition2 = GlycanCodeToInts(code);

            Assert.AreEqual(composition2[0], 5);
            Assert.AreEqual(composition2[1], 4);
            Assert.AreEqual(composition2[2], 1);
            Assert.AreEqual(composition2[3], 2);

            string lineDash = "5-4-2-1";

            List<int> composition3 = GlycanDashToInts(lineDash);

            Assert.AreEqual(composition3[0], 5);
            Assert.AreEqual(composition3[1], 4);
            Assert.AreEqual(composition3[2], 2);
            Assert.AreEqual(composition3[3], 1);
        }


        [Test]
        public void FindRelatinoshipsTest()
        {
            List<MonosaccharideName> monosacharidesOfInterest = new List<MonosaccharideName>();
            monosacharidesOfInterest.Add(MonosaccharideName.Hexose);
            monosacharidesOfInterest.Add(MonosaccharideName.NAcetylhexosamine);
            monosacharidesOfInterest.Add(MonosaccharideName.Deoxyhexose);
            monosacharidesOfInterest.Add(MonosaccharideName.NeuraminicAcid);


            GlycanComposition baseGlycan = new GlycanComposition(monosacharidesOfInterest, new int[4] { 5, 5, 5, 5 }.ToList());

            
            GlycanComposition compareGlycan1_HexAbove = new GlycanComposition(monosacharidesOfInterest, new int[4] { 5, 5, 6, 5 }.ToList());
            GlycanComposition compareGlycan2_HexNAcBelow = new GlycanComposition(monosacharidesOfInterest, new int[4] { 5, 4, 5, 5 }.ToList());
            GlycanComposition compareGlycan2_FucBelow = new GlycanComposition(monosacharidesOfInterest, new int[4] { 5, 5, 4, 5 }.ToList());
            GlycanComposition compareGlycan2_NeuAcAbove = new GlycanComposition(monosacharidesOfInterest, new int[4] { 5, 5, 5, 6 }.ToList());
            GlycanComposition compareGlycan3_UpDownUpDown = new GlycanComposition(monosacharidesOfInterest, new int[4] { 6, 4, 6, 4 }.ToList());
            GlycanComposition compareGlycan_2above = new GlycanComposition(monosacharidesOfInterest, new int[4] { 7, 5, 5, 7 }.ToList());
            GlycanComposition compareGlycan_2above_1_Below = new GlycanComposition(monosacharidesOfInterest, new int[4] { 7, 7, 5, 3 }.ToList());

            Console.WriteLine("HexAbove");
            GlycanCompositionUtilities.SingleLinkCompositions(ref baseGlycan, compareGlycan1_HexAbove); Print(baseGlycan); baseGlycan.ResetConnections();

            Console.WriteLine("HexNAcBelow");
            GlycanCompositionUtilities.SingleLinkCompositions(ref baseGlycan, compareGlycan2_HexNAcBelow); Print(baseGlycan); baseGlycan.ResetConnections();

            Console.WriteLine("FucBelow");
            GlycanCompositionUtilities.SingleLinkCompositions(ref baseGlycan, compareGlycan2_FucBelow); Print(baseGlycan); baseGlycan.ResetConnections();

            Console.WriteLine("NeuAcAbove");
            GlycanCompositionUtilities.SingleLinkCompositions(ref baseGlycan, compareGlycan2_NeuAcAbove); Print(baseGlycan); baseGlycan.ResetConnections();

            Console.WriteLine("UpDownUpDown");
            GlycanCompositionUtilities.SingleLinkCompositions(ref baseGlycan, compareGlycan3_UpDownUpDown);Print(baseGlycan); baseGlycan.ResetConnections();

            Console.WriteLine("Two Above, no change");
            GlycanCompositionUtilities.SingleLinkCompositions(ref baseGlycan, compareGlycan_2above); Print(baseGlycan); baseGlycan.ResetConnections();

            Console.WriteLine("Two Above one below, no change");
            GlycanCompositionUtilities.SingleLinkCompositions(ref baseGlycan, compareGlycan_2above_1_Below); Print(baseGlycan); baseGlycan.ResetConnections();


        }

        private static void Print(GlycanComposition baseGlycan)
        {
            Console.WriteLine("Hex:    " + baseGlycan.Connetions[MonosaccharideName.Hexose]);
            Console.WriteLine("HexNAc: " + baseGlycan.Connetions[MonosaccharideName.NAcetylhexosamine]);
            Console.WriteLine("Fuc:    " + baseGlycan.Connetions[MonosaccharideName.Deoxyhexose]);
            Console.WriteLine("NeuAc:  " + baseGlycan.Connetions[MonosaccharideName.NeuraminicAcid]);
            Console.WriteLine(Environment.NewLine);
        }
    }
}
