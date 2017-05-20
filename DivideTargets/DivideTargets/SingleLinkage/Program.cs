using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DivideTargetsLibraryX64;
using DivideTargetsLibraryX64.Combine;
using DivideTargetsLibraryX64.FromGetPeaks;
using DivideTargetsLibraryX64.Objects;

namespace SingleLinkage
{
    class Program
    {
        static void Main(string[] args)
        {
   
            //input data that we want to find family relationships in 
            string file = args[0];
            //output file name for data that has atleast one family member
            string filteredDataPath = args[1];
            //  how many digits to we link 5-4-0-1-3 will have 5 digits
            int digitsToCompare = Convert.ToInt32(args[2]);

            bool overrideargs = false;
            if(overrideargs)
            {
                string folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\FE_ESI_Cell_Norm1_26Nov13_40uL_230nL_C14_1\ResultsSummary";
                file = folder + @"\" + "ESI_Cell_Norm1_26Nov13_40uL_230nL_C14_1_Global_iqResults.txt";
                filteredDataPath = folder + @"\" + "FilteredResults.txt";
                digitsToCompare = 4;
            }


            //step 1.  load in data
            ResultsFilesCompostion loader = new ResultsFilesCompostion();
            
            string firstLine = null;
            List<DataHolderForSort> dataPile2 = loader.loadData2(file, out firstLine);

            List<DataHolderForSort> dataPileSortedByComposition = dataPile2.OrderBy(p => p.SortKeyComposition).ToList();

            Console.WriteLine(dataPileSortedByComposition.Count + " results were loaded");

            //step 2.  convert to Atoms for single linkage clustering
            List<Whale> whales = new List<Whale>();

            foreach (var dataHolderForSort in dataPileSortedByComposition)
            {
                if (dataHolderForSort.SortKeyString2 == "CorrectGlycan" || dataHolderForSort.SortKeyString2 == "NonValidatedHit" || dataHolderForSort.SortKeyString2 == "ValidatedGlycanFragment")
                {
                    //string code = Convert.ToString(dataHolderForSort.SortKeyComposition);
                    string code = dataHolderForSort.SortKeyComposition;
                    
                    //List<int> compositionAsList = Converter.ConvertStringGlycanCodeToIntegers320000(code);
                    //List<int> compositionAsList = Converter.ConvertStringGlycanCodeToIntegers0302000000(code);
                    List<int> compositionAsList = Converter.ConvertStringGlycanCodeToIntegers(code);
                    Atom myAtom = new Atom(compositionAsList);
                    Whale myWhale = new Whale();
                    myWhale.ResultData = dataHolderForSort;
                    myWhale.MyAtom = myAtom;
                    whales.Add(myWhale);
                }

            }

            Console.WriteLine(whales.Count + " compositions were added");

            //step 3, run single linkage clustering to catch all glycans 1 monosacharide apart
            SingleLink(ref whales, digitsToCompare);


            bool addAllCorrectGlycansByDefault = true;
            if (addAllCorrectGlycansByDefault)
            {
                foreach (var whale in whales)
                {
                    if (whale.ResultData.SortKeyString2 == "CorrectGlycan")
                    {
                        whale.MyAtom.IsLinked = true;
                    }
                }
            }

            int linkedCount = 0;
            int nonLinkedCount = 0;
            foreach (var whale in whales)
            {
                if(whale.MyAtom.IsLinked==true)
                {
                    linkedCount++;
                }
                else
                {
                    nonLinkedCount++;
                }
            }

            Console.WriteLine(linkedCount + " were linked and " + nonLinkedCount + " were one hit wonders");


            //step 4.  sort 
            List<Whale> dataPileSortedByCompositionSorted = whales.OrderBy(p => p.ResultData.SortKeyComposition).ToList();

            List<Whale> dataPileSortedByFinalType = dataPileSortedByCompositionSorted.OrderBy(p => p.ResultData.SortKeyString2).ToList();

            List<Whale> dataPileSortedByFinalAnswer = dataPileSortedByFinalType.OrderByDescending(p => p.ResultData.SortKeyString).ToList();


 
            //step 5 write
            List<string> stringsToWrite = new List<string>();
            stringsToWrite.Add(firstLine);

            Console.WriteLine("Compile data to write");

            foreach (var whale in dataPileSortedByFinalAnswer)
            {
                if (whale.MyAtom.IsLinked == true)
                {
                    stringsToWrite.Add(whale.ResultData.LineOfText);
                }
            }

            Console.WriteLine("Writing data...");
            
            StringListToDisk writer = new StringListToDisk();
            writer.toDiskStringList(filteredDataPath, stringsToWrite);

            Console.WriteLine("Done!");
        }

        public static void SingleLink(ref List<Whale> whales, int digitsToCompare)
        {
            foreach (var whale in whales)
            {
                for (int i = 0; i < whales.Count; i++)
                {
                    if (whale.MyAtom.IsLinked == false)
                    {
                        whale.MyAtom.IsLinked = AtomCompare.Compare(whale.MyAtom, whales[i].MyAtom, 1, digitsToCompare);
                    }
                    else
                    {
                        break;
                    }
                }
            }
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
