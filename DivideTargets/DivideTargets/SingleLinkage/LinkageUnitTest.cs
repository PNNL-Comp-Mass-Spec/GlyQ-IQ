using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DivideTargetsLibraryX64;
using DivideTargetsLibraryX64.Combine;
using DivideTargetsLibraryX64.Objects;
using NUnit.Framework;

namespace SingleLinkage
{
    class LinkageUnitTest
    {
        [Test]
        public void simpleTest()
        {
            Atom baseAtom = new Atom(1,1,1,1);
            Atom testAtom = new Atom(2,1,1,1);

            bool digit1 = AtomCompare.Compare(baseAtom, testAtom, 1, 4);

            Assert.AreEqual(digit1, true);


            bool digit2 = AtomCompare.Compare(new Atom(1, 1, 1, 1), new Atom(1, 2, 1, 1), 1,4);

            Assert.AreEqual(digit2, true);

            bool digit3 = AtomCompare.Compare(new Atom(1, 1, 1, 1), new Atom(1, 1, 2, 1), 1,4);

            Assert.AreEqual(digit3, true);

            bool digit4 = AtomCompare.Compare(new Atom(1, 1, 1, 1), new Atom(1, 1, 1, 2), 1,4);

            Assert.AreEqual(digit4, true);

            bool digit1And4 = AtomCompare.Compare(new Atom(1, 1, 1, 1), new Atom(2, 1, 1, 2), 1,4);

            Assert.AreEqual(digit1And4, false);

            bool digit1by2 = AtomCompare.Compare(new Atom(1, 1, 1, 1), new Atom(3, 1, 1, 1), 1,4);

            Assert.AreEqual(digit1by2, false);

            bool digit1minus1 = AtomCompare.Compare(new Atom(2, 2, 2, 2), new Atom(1, 2, 2, 2), 1,4);

            Assert.AreEqual(digit1minus1, true);
        }

        [Test]
        public void ListCompare()
        {
            //load data
            List<Atom> data = new List<Atom>();
            data.Add(new Atom(5, 4, 1, 1, 0));
            data.Add(new Atom(5, 4, 1, 2, 0));
            data.Add(new Atom(5, 4, 0, 1, 0));
            data.Add(new Atom(5, 4, 0, 2, 0));

            foreach (var atom in data)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    if (atom.IsLinked == false)
                    {
                        atom.IsLinked = AtomCompare.Compare(atom, data[i], 1,5);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            foreach (var atom in data)
            {
                Assert.AreEqual(atom.IsLinked, true);
            }
        }

        [Test]
        public void ListCompareWithRedHarring()
        {
            //load data
            List<Atom> data = new List<Atom>();
            data.Add(new Atom(5, 4, 1, 1, 0));
            data.Add(new Atom(5, 4, 1, 2, 0));
            data.Add(new Atom(5, 4, 0, 1, 0));
            data.Add(new Atom(5, 4, 0, 2, 0));

            data.Add(new Atom(5, 4, 0, 100, 0));
            data.Add(new Atom(5, 4, 0, 200, 0));

            foreach (var atom in data)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    if (atom.IsLinked == false)
                    {
                        atom.IsLinked = AtomCompare.Compare(atom, data[i], 1,5);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            foreach (var atom in data)
            {
                if (atom.IsLinked == false)
                {
                    Console.WriteLine("The excluded composition is");
                    PrintComposition(atom);
                }
                else
                {
                    Assert.AreEqual(atom.IsLinked, true);
                }
            }
        }

        [Test]
        public void ListCompareWithRedHarringPairs()
        {
            //load data
            List<Atom> data = new List<Atom>();
            data.Add(new Atom(5, 4, 1, 1, 0));
            data.Add(new Atom(5, 4, 1, 2, 0));
            data.Add(new Atom(5, 4, 0, 1, 0));
            data.Add(new Atom(5, 4, 0, 2, 0));

            data.Add(new Atom(5, 4, 0, 100, 0));
            data.Add(new Atom(5, 4, 0, 101, 0));
            data.Add(new Atom(5, 4, 0, 102, 0));

            foreach (var atom in data)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    if (atom.IsLinked == false)
                    {
                        atom.IsLinked = AtomCompare.Compare(atom, data[i], 1,5);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            foreach (var atom in data)
            {
                if (atom.IsLinked == false)
                {
                    Console.WriteLine("The excluded composition is");
                    PrintComposition(atom);
                }
                else
                {
                    Assert.AreEqual(atom.IsLinked, true);
                }
            }
        }


        [Test]
        public void LoadFromFile()
        {
            ResultsFilesCompostion loader = new ResultsFilesCompostion();
            string file = @"D:\Csharp\0_TestDataFiles\S_SN129_1_Global_iqResults.txt";
            string firstLine = null;
            var dataPile2 = loader.loadData2(file, out firstLine);

            List<DataHolderForSort> dataPileSortedByComposition = dataPile2.OrderBy(p => p.SortKeyComposition).ToList();

            Assert.AreEqual(dataPileSortedByComposition.Count, 388785);
        }

        [Test]
        public void ConvertDoubleToIntArray()
        {
            ////last 2 digits are for PSA
            //double convolutedComposition = 32004;

            //string code = Convert.ToString(convolutedComposition);
            //List<int> compositionAsList = Converter.ConvertStringGlycanCodeToIntegers32000(code);

            //Assert.AreEqual(compositionAsList.Count,4);

            //Atom result = new Atom(compositionAsList[0], compositionAsList[1], compositionAsList[2], compositionAsList[3]);
            //Console.WriteLine("The first conversion give us this composition:");
            //PrintComposition(result);
            //Console.WriteLine(Environment.NewLine);

            ////add a lactone to the end
            //double convolutedCompositionLacone = 320211;

            //string codeLacone = Convert.ToString(convolutedCompositionLacone);
            //List<int> compositionAsListLacone = Converter.ConvertStringGlycanCodeToIntegers320000(codeLacone);

            //Assert.AreEqual(compositionAsListLacone.Count, 5);
            //Atom resultLctone = new Atom(compositionAsListLacone[0], compositionAsListLacone[1], compositionAsListLacone[2], compositionAsListLacone[3], compositionAsListLacone[4]);
            //Console.WriteLine("The second conversion give us this composition:");
            //PrintComposition(resultLctone);
            //Console.WriteLine(Environment.NewLine);

            ////add a lactone to the end
            //convolutedCompositionLacone = 1220211;

            //codeLacone = Convert.ToString(convolutedCompositionLacone);
            //compositionAsListLacone = Converter.ConvertStringGlycanCodeToIntegers320000(codeLacone);

            //Assert.AreEqual(compositionAsListLacone.Count, 5);
            //resultLctone = new Atom(compositionAsListLacone[0], compositionAsListLacone[1], compositionAsListLacone[2], compositionAsListLacone[3], compositionAsListLacone[4]);
            //Console.WriteLine("The second conversion give us this composition:");
            //PrintComposition(resultLctone);



        }

        private static void PrintComposition(Atom atom)
        {
            if (atom.Composition.Length == 4)
            {
                Console.WriteLine("The composition is " + atom.Composition[0] + "," + atom.Composition[1] + "," + atom.Composition[2] + "," + atom.Composition[3]);
            }

            if (atom.Composition.Length == 5)
            {
                Console.WriteLine("The composition is " + atom.Composition[0] + "," + atom.Composition[1] + "," + atom.Composition[2] + "," + atom.Composition[3] + "," + atom.Composition[4]);
            }
        }
    }
}
