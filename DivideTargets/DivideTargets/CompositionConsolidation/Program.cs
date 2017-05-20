using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DivideTargetsLibraryX64;
using DivideTargetsLibraryX64.Combine;
using DivideTargetsLibraryX64.FromGetPeaks;
using DivideTargetsLibraryX64.Objects;

namespace CompositionConsolidation
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //input data that we want to consolidate down to distinct compositinos and return the most abundanct
            string file = args[0];
            //output file name for compositionlist
            string filteredDataPath = args[1];

            bool overrideargs = false;
            if (overrideargs)
            {
                string folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\FA_V_SN129_1\ResultsSummary";
                file = folder + @"\" + "V_SN129_1_Family_iqResults.txt";
                filteredDataPath = folder + @"\" + "CompositionResults.txt";

                       folder = @"\\picfs\projects\DMS\PIC_HPC\Hot\F_Ant_Gly09_Velos3_Jaguar_200nL_C12_AntB1_3X_25Dec12_1\ResultsSummary";
                       file = folder + @"\" + "Gly09_Velos3_Jaguar_200nL_C12_AntB1_3X_25Dec12_1_Family_iqResults.txt";
                       filteredDataPath = folder + @"\" + "CompositionResults.txt";

            }

            bool reportMaxAbundantIsomer = true;
            bool sumMultipleEntries = true;

            Console.WriteLine("Report Max Abundant when there are replicates" + reportMaxAbundantIsomer);
            Console.WriteLine("Report summed abundance when there are replicates (overrides max) " + sumMultipleEntries);

            //\\picfs\projects\DMS\PIC_HPC\Hot\FC_ESI_Cell_Norm1_26Nov13_40uL_230nL_C14_1\ResultsSummary\ESI_Cell_Norm1_26Nov13_40uL_230nL_C14_1_Family_iqResults.txt  \\picfs\projects\DMS\PIC_HPC\Hot\FC_ESI_Cell_Norm1_26Nov13_40uL_230nL_C14_1\ResultsSummary\CompositionResults.txt

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
                    //List<int> compositionAsList = Converter.ConvertStringGlycanCodeToIntegers(code);

                    string code = dataHolderForSort.SortKeyComposition;
                    List<int> compositionAsList = Converter.ConvertStringGlycanCodeToIntegers(code);
                    
                    
                    Atom myAtom = new Atom(compositionAsList);
                    Whale myWhale = new Whale();
                    myWhale.ResultData = dataHolderForSort;
                    myWhale.MyAtom = myAtom;
                    whales.Add(myWhale);
                }

            }

            Console.WriteLine(whales.Count + " compositions were added");

            //List<Whale> whalesCompositionsUnique = whales.GroupBy(n => n.ResultData.SortKeyDouble).Select(group => group.First()).ToList().OrderBy(n=>n.ResultData.SortKeyDouble).ToList();
            List<string> compositionsUnique = whales.Select(n => n.ResultData.SortKeyComposition).Distinct().ToList().OrderBy(n=>n).ToList();

            Console.WriteLine("The compositon list has " + compositionsUnique.Count + " members");

            List<Whale> bestCompositionvisBestAbundance = new List<Whale>();
            foreach (var code in compositionsUnique)
            {
                //PrintComposition(whale.MyAtom);
                Console.WriteLine(code);

                List<Whale> whalesCompositionsByCode = (from whale in whales where whale.ResultData.SortKeyComposition == code select whale).ToList();

                //now filter by correct glycan or non validated glycan hit.

                List<Whale> whalesCompositionsByCodeCorrectGlycan = (from whale in whalesCompositionsByCode where whale.ResultData.SortKeyString2 == "CorrectGlycan" select whale).ToList();
                List<Whale> whalesCompositionsByCodeNonValidatedGlycanHit = (from whale in whalesCompositionsByCode where whale.ResultData.SortKeyString2 == "NonValidatedHit" select whale).ToList();

                List<Whale> whalesCompositionsByCodeToFinxMaxAbundance = new List<Whale>();
                if (whalesCompositionsByCodeCorrectGlycan !=null && whalesCompositionsByCodeCorrectGlycan.Count > 0)
                {
                    whalesCompositionsByCodeToFinxMaxAbundance = whalesCompositionsByCodeCorrectGlycan;
                }
                else
                {
                    if (whalesCompositionsByCodeNonValidatedGlycanHit != null && whalesCompositionsByCodeNonValidatedGlycanHit.Count > 0)
                    {
                        whalesCompositionsByCodeToFinxMaxAbundance = whalesCompositionsByCodeNonValidatedGlycanHit;
                    }
                }
                
                
                //break out abundance so we can pick the most abundant

                Console.WriteLine("Now find best to serve as a representitative");

                if (whalesCompositionsByCodeToFinxMaxAbundance.Count > 0)
                {
                    int indexOfAbundance = 33;//this is the index where abundance lives by column

                    char[] splitter = new char[] {'\t'};

                    string[] initalFullResults = whalesCompositionsByCodeToFinxMaxAbundance[0].ResultData.LineOfText.Split(splitter);

                    Whale selectedWhale = new Whale();

                    

                    if (reportMaxAbundantIsomer)
                    {
                        double abuncanceInitial = Convert.ToDouble(initalFullResults[indexOfAbundance]);
                        selectedWhale = whalesCompositionsByCodeToFinxMaxAbundance[0];
                        foreach (var whale in whalesCompositionsByCodeToFinxMaxAbundance)
                        {
                            string[] fullResults = whale.ResultData.LineOfText.Split(splitter, StringSplitOptions.None);
                            double abundance = Convert.ToDouble(fullResults[indexOfAbundance]);
                            if (abundance > abuncanceInitial)
                            {
                                abuncanceInitial = abuncanceInitial;
                                selectedWhale = whale;
                            }
                        }

                        if (sumMultipleEntries)
                        {
                            Console.WriteLine("report summed abundance in column " + indexOfAbundance);
                            
                            double summedAbundance = 0;
                            
                            foreach (var whale in whalesCompositionsByCodeToFinxMaxAbundance)
                            {
                                string[] fullResults = whale.ResultData.LineOfText.Split(splitter, StringSplitOptions.None);
                                summedAbundance += Convert.ToDouble(fullResults[indexOfAbundance]);
                            }

                            //update abundance for selected Whale.  this will report the characteristics of the most abundant whale but report the summed abundance across all isomers
                            string[] selectedResultsToUpdate = selectedWhale.ResultData.LineOfText.Split(splitter, StringSplitOptions.None);

                            //update field
                            double roundedSummedAbundance = Math.Round(summedAbundance, 0);

                            selectedResultsToUpdate[indexOfAbundance] = Convert.ToString(roundedSummedAbundance);
                            //Convert object to line of text
                            string newLineOfText = "";
                            for(int i=0;i<selectedResultsToUpdate.Length-1;i++)
                            {
                                newLineOfText = newLineOfText + selectedResultsToUpdate[i] + splitter[0];
                            }
                            newLineOfText = newLineOfText + selectedResultsToUpdate[selectedResultsToUpdate.Length-1];

                            selectedWhale.ResultData.LineOfText = newLineOfText;
                        }
                    }

                    
                    

                    bestCompositionvisBestAbundance.Add(selectedWhale);

                }

            }

            


            //step   print out the composition list

            //step 4.  sort 
            List<Whale> dataPileSortedByCompositionSorted = bestCompositionvisBestAbundance.OrderBy(p => p.ResultData.SortKeyComposition).ToList();

            List<Whale> dataPileSortedByFinalType = dataPileSortedByCompositionSorted.OrderBy(p => p.ResultData.SortKeyString2).ToList();

            List<Whale> dataPileSortedByFinalAnswer = dataPileSortedByFinalType.OrderByDescending(p => p.ResultData.SortKeyString).ToList();



            //step 5 write
            List<string> stringsToWrite = new List<string>();
            stringsToWrite.Add(firstLine);

            Console.WriteLine("Compile data to write");

            foreach (var whale in dataPileSortedByFinalAnswer)
            {
                stringsToWrite.Add(whale.ResultData.LineOfText);
            }

            Console.WriteLine("Writing data...");

            StringListToDisk writer = new StringListToDisk();
            writer.toDiskStringList(filteredDataPath, stringsToWrite);

            Console.WriteLine("Done!");
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
