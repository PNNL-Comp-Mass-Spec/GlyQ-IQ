using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.DataFIFO;
using NUnit.Framework;
using GetPeaks_DLL.SQLite.DataTransferObjects;
using GetPeaks_DLL.SQLite;
using CompareContrastDLL;
using OmniFinder.Objects;
using PNNLOmics.Data;

namespace GetPeaks.UnitTests
{
    class ComparisonTest
    {
        [Test]
        public void CompareMonoFiles()
        {
            //string fileName1 = @"D:\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\Full data set\set25\PD_OrbitrapFilter_5sum_1cores_25divider_False-MULTIMode_0oLevel_1.3PBR_2SNT_10fit (0).db";
            //string fileName1 = @"D:\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\Full data set\set25\PD_OrbitrapFilter_5sum_24cores_25divider_True-MULTIMode_0oLevel_1.3PBR_2SNT_10fit (0).db";
            //string fileName2 = @"D:\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\Full data set\set25\PD_OrbitrapFilter_5sum_4cores_25divider_True-MULTIMode_0oLevel_1.3PBR_2SNT_10fit_(1).db";

            //string fileName1 = @"D:\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\Full data set\set01\PD_DeconTools_5sum_1cores_1divider_False-MULTIMode_0oLevel_1.3PBR_2SNT_10fit_(1).db";
            //string fileName2 = @"D:\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\Full data set\set01\PD_OrbitrapFilter_5sum_24cores_1divider_True-MULTIMode_0oLevel_1.3PBR_2SNT_10fit (0).db";

            string fileName2Data = @"V:\Databases\PD_DeconTools_5sum_1cores_1divider_False-MULTIMode_0oLevel_1.3PBR_2SNT_10fit_(1).db";
            string fileName1Library = @"V:\Databases\PD_OrbitrapFilter_5sum_24cores_1divider_True-MULTIMode_0oLevel_1.3PBR_2SNT_10fit (0).db";

            string Set25Sum5Core4OF = @"L:\PNNL Files\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\Full data set\set25\PD_OrbitrapFilter_5sum_4cores_25divider_True-MULTIMode_0oLevel_1.3PBR_2SNT_10fit_(1).db";
            string Set25Sum5Core24OF = @"L:\PNNL Files\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\Full data set\set25\PD_OrbitrapFilter_5sum_24cores_25divider_True-MULTIMode_0oLevel_1.3PBR_2SNT_10fit (0).db";
            string Set25Sum5Core1OF = @"L:\PNNL Files\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\Full data set\set25\PD_OrbitrapFilter_5sum_1cores_25divider_False-MULTIMode_0oLevel_1.3PBR_2SNT_10fit (0).db";

            string Set01Sum5Core1DT = @"L:\PNNL Files\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\Full data set\set01\PD_DeconTools_5sum_1cores_1divider_False-MULTIMode_0oLevel_1.3PBR_2SNT_10fit_(1).db";
            string Set01Sum5Core1OF = @"L:\PNNL Files\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\Full data set\set01\PD_OrbitrapFilter_5sum_1cores_1divider_False-MULTIMode_0oLevel_1.3PBR_2SNT_10fit (0).db";
            string Set01Sum5Core24OF = @"L:\PNNL Files\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\Full data set\set01\PD_OrbitrapFilter_5sum_24cores_1divider_True-MULTIMode_0oLevel_1.3PBR_2SNT_10fit (0).db";
            
            string Set01Sum1Core24OF = @"L:\PNNL Files\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\Full data set\set01\PD_OrbitrapFilter_1sum_24cores_1divider_True-MULTIMode_0oLevel_1.3PBR_2SNT_10fit_(1).db";
            string Set01Sum3Core24OF = @"L:\PNNL Files\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\Full data set\set01\PD_OrbitrapFilter_3sum_24cores_1divider_True-MULTIMode_0oLevel_1.3PBR_2SNT_10fit_(1).db";

            string ManualMono = @"L:\PNNL Files\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\5509\Igor For Allignment\MonoManualSpine.txt";
            string ManualPeptide = @"L:\PNNL Files\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\5509\Igor For Allignment\MonoPeptides.txt";//126

            //string ManualMonoWork = @"D:\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\5509\Igor For Allignment\MonoManualSpine.txt";
            string ManualPeptideWork = @"D:\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\5509\Igor For Allignment\MonoPeptides.txt";//126

            string HammerMonoK = @"K:\sortMonosHammer.txt";
            string ManualMonosK = @"K:\MonosManual.txt";//126
            string DeconMonoK = @"K:\sortMonosDecon.txt";

            string HammerMZK = @"K:\MZHammer.txt";
            string ManualMZK = @"K:\MZManual.txt";//126
            string DeconMZK = @"K:\MZDecon.txt";

            string HammerTP = @"K:\ROC_HammerMonoTP0.txt";
            string DeonTP = @"K:\ROC_DeconMonoTP130.txt";

            //final settings
            string FinalSet01Sum1Core1Decon = @"D:\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\Full data set\set01\smarter THRASH\Final Settings\PD_DeconTools_1sum_1cores_1divider_False-MULTIMode_0oLevel_1.3PBR_2SNT_10fit_(45min53sec).db";
            string FinalSet01Sum1Core24Hammer = @"D:\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\Full data set\set01\smarter THRASH\Final Settings\PD_OrbitrapFilter_1sum_24cores_1divider_True-MULTIMode_0oLevel_1.3PBR_2SNT_10fit_(18min18sec).db";
            string FinalSet01Sum1Core24HammerNoThreshold = @"D:\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\Full data set\set01\smarter THRASH\Final Settings\PD_OrbitrapFilter_1sum_24cores_1divider_True-MULTIMode_0oLevel_1.3PBR_2SNT_10fit_(20min52sec).db";
            string FinalSet01Sum1Core24Decon = @"D:\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\Full data set\set01\smarter THRASH\Final Settings\PD_DeconTools_1sum_24cores_1divider_True-MULTIMode_0oLevel_1.3PBR_2SNT_10fit_(8min0sec).db";
            string FinalSet01Sum1Core1Hammer = @"D:\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\Full data set\set01\smarter THRASH\Final Settings\PD_OrbitrapFilter_1sum_1cores_1divider_False-MULTIMode_0oLevel_1.3PBR_2SNT_10fit_(124min4sec).db";
            string FinalSet01Sum1Core24HammerNoThresholdOptimized = @"D:\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\Full data set\set01\smarter THRASH\Final Settings\PD_OrbitrapFilter_1sum_24cores_1divider_True-MULTIMode_0oLevel_1.3PBR_2SNT_10fit_(28min26secOptNoT).db";
            string FinalSet01Sum1Core1HammerNoThreshold = @"D:\PNNL\Lab Infrastructure\PNNL Institutational Computerin (PIC)\Multithreading\Full data set\set01\smarter THRASH\Final Settings\PD_OrbitrapFilter_1sum_1cores_1divider_False-MULTIMode_NoLevel_1.3PBR_2SNT_10fit_(158min2sec).db";
           
            
            int toggle = 0;

            //scan numbers are incremented by one here so that it will sync with the thermo viewer

            //toggle = 8;//text to peptide txt
            //toggle = 7;//hammer vs manual
            //toggle = 8;//decon vs manual
            //toggle = 10;//decon vs hammer
            //toggle = 11;//decon vs hammer at 1.3 and 0 levels
            //toggle = 12;//Hammer1 vs decon1 at 1.3 and 0 levels
            //toggle = 13;//Hammer24 vs decon1 at 1.3 and no threshold levels
            //toggle = 14;//Decon24 vs decon1 at 1.3 
            //toggle = 15;//Hammer24 vs Hammer1 at 0 
            //toggle = 16;//Decon24 vs decon24 at 1.3 
            //toggle = 17;//nothreshold optimized to decon
            toggle = 18;//nothreshold to decon
            //string type = "txt";
            FileType type = FileType.SQLite;
            bool headdersToggle = false;
            double massTolleranceVerySmall = 0.01;//ppm

            switch (toggle)
            {
                case 0: //compare single thread vs multi thread on full dataset
                    {
                        fileName1Library = Set01Sum5Core1OF;//more correct
                        fileName2Data = Set01Sum5Core24OF;
                        type = FileType.SQLite;
                    }
                    break;
                case 1://compare decon tools to orbirtrap filter
                    {
                        
                        fileName1Library = Set01Sum5Core1DT;
                        fileName2Data = Set01Sum5Core1OF;
                        type = FileType.SQLite;
                    }
                    break;
                case 2://compare 1 core vs 4 cores on set 25
                    {  
                        fileName1Library = Set25Sum5Core1OF;
                        fileName2Data = Set25Sum5Core4OF;
                        type = FileType.SQLite;
                    }
                    break;
                case 3://compare 24 core vs 4 cores on set 25
                    {
                        fileName1Library = Set25Sum5Core24OF;
                        fileName2Data = Set25Sum5Core4OF;
                        type = FileType.SQLite;
                    }
                    break;
                case 4://compare 24 core vs 4 cores on set 25
                    {
                        fileName1Library = Set01Sum1Core24OF;
                        fileName2Data = Set01Sum3Core24OF;
                        type = FileType.SQLite;
                    }
                    break;
                case 5://text to peptide txt
                    {
                        fileName1Library = ManualPeptide;
                        fileName2Data = ManualMono;
                        type = FileType.txt;
                        massTolleranceVerySmall = 15;//ppm
                    }
                    break;
                case 6://text to peptide txt
                    {
                        fileName1Library = ManualPeptideWork;
                        fileName2Data = ManualMonosK;
                        type = FileType.txt;
                        massTolleranceVerySmall = 15;//ppm
                    }
                    break;
                case 7://results from Hammer vs Manual
                    {
                        fileName1Library = ManualMonosK;
                        fileName2Data = HammerMonoK;
                        type = FileType.txt;
                        massTolleranceVerySmall = 15;//ppm
                        headdersToggle = false;
                    }
                    break;
                case 8://results from Decon vs Manual
                    {
                        fileName1Library = ManualMonosK;
                        fileName2Data = DeconMonoK;
                        type = FileType.txt;
                        massTolleranceVerySmall = 15;//ppm
                        headdersToggle = false;
                    }
                    break;
                case 9://results from Decon vs Manual
                    {
                        fileName1Library = HammerMonoK;
                        fileName2Data = DeconMonoK;
                        type = FileType.txt;
                        massTolleranceVerySmall = 15;//ppm
                        headdersToggle = false;
                    }
                    break;
                case 10://results from Decon vs Manual
                    {
                        fileName1Library = HammerMZK;
                        fileName2Data = DeconMZK;
                        type = FileType.txt;
                        massTolleranceVerySmall = 15;//ppm
                        headdersToggle = false;
                    }
                    break;
                case 11://results from Decon vs Manual
                    {
                        fileName1Library = HammerTP;
                        fileName2Data = DeonTP;
                        type = FileType.txt;
                        massTolleranceVerySmall = 10;//ppm
                        headdersToggle = false;
                    }
                    break;
                case 12://compare hammer to decon 1 cores on set 1
                    {
                        fileName1Library = FinalSet01Sum1Core1Hammer;
                        fileName2Data = FinalSet01Sum1Core1Decon;
                        type = FileType.SQLite;
                    }
                    break;
                case 13://compare 24 core vs 1 cores on set 1
                    {
                        fileName1Library = FinalSet01Sum1Core24Hammer;
                        fileName2Data = FinalSet01Sum1Core24HammerNoThreshold;
                        type = FileType.SQLite;
                    }
                    break;
                case 14://compare 24 core vs 1 cores on set 1
                    {
                        fileName1Library = FinalSet01Sum1Core24Decon;
                        fileName2Data = FinalSet01Sum1Core1Decon;
                        type = FileType.SQLite;
                    }
                    break;
                case 15://compare 24 core vs 1 cores on set 1
                    {
                        fileName1Library = FinalSet01Sum1Core24Hammer;//library
                        fileName2Data = FinalSet01Sum1Core1Hammer;//data
                        type = FileType.SQLite;
                    }
                    break;
                case 16://compare 24 core vs 1 cores on set 1
                    {
                        fileName1Library = FinalSet01Sum1Core24Decon;//library
                        fileName2Data = FinalSet01Sum1Core24Decon;//data
                        type = FileType.SQLite;
                    }
                    break;
                case 17://compare 24 core vs 1 cores on set 1
                    {
                        fileName1Library = FinalSet01Sum1Core24HammerNoThreshold;//library
                        fileName2Data = FinalSet01Sum1Core24HammerNoThresholdOptimized;//data
                        type = FileType.SQLite;
                    }
                    break;
                case 18://compare 24 core vs 1 cores on set 1
                    {
                        fileName1Library = FinalSet01Sum1Core1HammerNoThreshold;//library
                        fileName2Data = FinalSet01Sum1Core1Decon;//data
                        type = FileType.SQLite;
                    }
                    break;
               
            }


           

            List<DatabaseIsotopeObject> sortedmonosFromFileData = new List<DatabaseIsotopeObject>();
            List<DatabaseIsotopeObject> sortedmonosFromFileLibrary = new List<DatabaseIsotopeObject>();
            switch (type)
            {
                case FileType.SQLite:
                    {
                        List<DatabaseIsotopeObject> monosFromFile1 = DatabaseIsotopeObjects(fileName2Data);
                        sortedmonosFromFileData = monosFromFile1.OrderBy(p => p.MonoIsotopicMass).ToList();
                        
                        Console.WriteLine("There are " + sortedmonosFromFileData.Count + " monos in ");
                        Console.WriteLine(fileName2Data);
                        //Assert.AreEqual(985, monosFromFile1.Count);

                        List<DatabaseIsotopeObject> monosFromFile2 = DatabaseIsotopeObjects(fileName1Library);
                        sortedmonosFromFileLibrary = monosFromFile2.OrderBy(p => p.MonoIsotopicMass).ToList();

                        Console.WriteLine("There are " + sortedmonosFromFileLibrary.Count + " monos");
                        Console.WriteLine(fileName1Library);
                    }break;
                case FileType.txt:
                    {
                        GetPeaks_DLL.DataFIFO.LoadXYData loader = new GetPeaks_DLL.DataFIFO.LoadXYData();

                        List<XYData> list1 = new List<XYData>();
                        List<XYData> list2 = new List<XYData>();
                        switch(headdersToggle)
                        {
                            case true:
                                {
                                    string headders;

                                    list1 = loader.Import(fileName2Data, out headders);
                                    list2 = loader.Import(fileName1Library, out headders);//library

                                }
                                break;
                            case false:
                                {
                                    GetPeaks_DLL.DataFIFO.FileIterator.deliminator deliminatorFiletype = FileIterator.deliminator.Comma;
                                    list1 = loader.Import(fileName2Data, deliminatorFiletype);//data
                                    list2 = loader.Import(fileName1Library, deliminatorFiletype);//library
                                }
                                break;

                        }
                       
                        List<DatabaseIsotopeObject> monosFromFile1 = new List<DatabaseIsotopeObject>();
                        
                        foreach (XYData dataPoint in list1)
                        { 
                            DatabaseIsotopeObject tempIsos = new DatabaseIsotopeObject();
                            tempIsos.MonoIsotopicMass = dataPoint.X;
                            tempIsos.abundance = dataPoint.Y;
                            monosFromFile1.Add(tempIsos);//data
                        }

                        List<DatabaseIsotopeObject> monosFromFile2 = new List<DatabaseIsotopeObject>();
                        foreach (XYData dataPoint in list2)
                        {
                            DatabaseIsotopeObject tempIsos = new DatabaseIsotopeObject();
                            tempIsos.MonoIsotopicMass = dataPoint.X;
                            tempIsos.abundance = dataPoint.Y;
                            monosFromFile2.Add(tempIsos);
                        }

                        sortedmonosFromFileData = monosFromFile1.OrderBy(p => p.MonoIsotopicMass).ToList();//data
                        sortedmonosFromFileLibrary = monosFromFile2.OrderBy(p => p.MonoIsotopicMass).ToList();
                        
                    }break;
            }
           
            //Assert.AreEqual(981, monosFromFile2.Count);

            CompareController compareHere = new CompareController();
            SetListsToCompare prepCompare = new SetListsToCompare();

            List<double> massList1Data = new List<double>();
            //List<int> scanList1Data = new List<int>();
            List<double> massList2Library = new List<double>();
            //List<int> scanList2Library = new List<int>();

            foreach (DatabaseIsotopeObject isos in sortedmonosFromFileData)//data
            {
                massList1Data.Add(isos.MonoIsotopicMass);
                isos.scan_num++;//syncs to thermo data xcalibur viewer
            }

            foreach (DatabaseIsotopeObject isos in sortedmonosFromFileLibrary)
            {
                massList2Library.Add(isos.MonoIsotopicMass);
                isos.scan_num++;//syncs to thermo data xcalibur viewer
            }

            CompareInputLists inputList = prepCompare.SetThem(massList1Data, massList2Library);
            
            CompareResultsIndexes indexesFromCompareIsomer = new CompareResultsIndexes();
            CompareResultsValues valuesFromCompareIsomer = compareHere.compareFX(inputList, massTolleranceVerySmall, ref indexesFromCompareIsomer);

            
            List<DatabaseIsotopeObject> onlyInData = new List<DatabaseIsotopeObject>();
            foreach (int index in indexesFromCompareIsomer.IndexListAandNotB)
            {
                onlyInData.Add(sortedmonosFromFileData[index]);//data only
            }
            onlyInData = onlyInData.OrderBy(p => p.scan_num).ToList();


            List<DatabaseIsotopeObject> onlyInLibrary = new List<DatabaseIsotopeObject>();
            foreach (int index in indexesFromCompareIsomer.IndexListBandNotA)
            {
                onlyInLibrary.Add(sortedmonosFromFileLibrary[index]);
            }

            onlyInLibrary = onlyInLibrary.OrderBy(p => p.scan_num).ToList();

            List<DatabaseIsotopeObject> InBothDataAndLibrary = new List<DatabaseIsotopeObject>();
            foreach (int dataHit in indexesFromCompareIsomer.IndexListBMatch)
            {
                InBothDataAndLibrary.Add(sortedmonosFromFileLibrary[dataHit]);
            }

            InBothDataAndLibrary = InBothDataAndLibrary.OrderBy(p => p.scan_num).ToList();


            bool writeData = true;
            if (writeData)
            {
                string datafileName = @"K:\InDataOnly.txt";
                string libraryfileName = @"K:\InLibraryOnly.txt";
                string bothfileName = @"K:\InBoth.txt";

                WriteMissesToDisk(onlyInLibrary, onlyInData, InBothDataAndLibrary, datafileName, libraryfileName, fileName1Library, fileName2Data, bothfileName);
            }

            int failed = valuesFromCompareIsomer.DataNotInLibrary.Count + valuesFromCompareIsomer.LibraryNotMatched.Count;
            double successes = valuesFromCompareIsomer.DataMatchingLibrary.Count;

            double fractionMissed = failed/successes*100;
            

            switch (toggle)
            {
                case 0: //compare single thread vs multi thread on full dataset
                    {
                        Console.WriteLine("Misses = " + failed.ToString());
                        Assert.AreEqual(failed, 0);
                    }
                    break;
                case 1://compare decon tools to orbirtrap filter
                    {
                        Console.WriteLine("Misses = " + failed.ToString());
                        Assert.AreEqual(failed, 0);
                    }
                    break;
                case 2://compare 1 core vs 4 cores on set 25
                    {
                        Console.WriteLine("Misses = " + failed.ToString());
                        Assert.AreEqual(failed, 0);
                    }
                    break;
                case 3://compare 24 core vs 4 cores on set 25
                    {
                        Console.WriteLine("Misses = " + failed.ToString());
                        Assert.AreEqual(failed, 0);
                    }
                    break;
                case 4://compare 24 core vs 4 cores on set 25
                    {
                        Console.WriteLine("Misses = " + failed.ToString());
                        Assert.AreEqual(failed, 0);
                    }
                    break;
                case 5://text to peptide txt
                    {
                        Console.WriteLine("Hits = " + valuesFromCompareIsomer.DataMatchingLibrary.Count.ToString());
                        Assert.AreEqual(valuesFromCompareIsomer.DataMatchingLibrary.Count, 146);
                    }
                    break;
                case 6://text to peptide txt
                    {
                        Console.WriteLine("Hits = " + valuesFromCompareIsomer.DataMatchingLibrary.Count.ToString());
                        Assert.AreEqual(valuesFromCompareIsomer.DataMatchingLibrary.Count, 103);
                        Assert.AreEqual(valuesFromCompareIsomer.LibraryNotMatched.Count, 23);
                    }
                    break;
                case 7://results from Hammer vs Manual
                    {
                        Console.WriteLine("Hits = " + valuesFromCompareIsomer.DataMatchingLibrary.Count.ToString());//103 of 126
                        Assert.AreEqual(valuesFromCompareIsomer.DataMatchingLibrary.Count, 137);
                    }
                    break;
                case 8://results from Decon vs Manual
                    {
                        Console.WriteLine("Hits = " + valuesFromCompareIsomer.DataMatchingLibrary.Count.ToString());
                        Assert.AreEqual(valuesFromCompareIsomer.DataMatchingLibrary.Count, 108);
                    }
                    break;
            }
            
        }

        private static void WriteMissesToDisk(List<DatabaseIsotopeObject> onlyInLibrary, List<DatabaseIsotopeObject> onlyInData, List<DatabaseIsotopeObject> inBothLibraryAndData, string datafileName, string libraryfileName, string fileName1Library, string fileName2Data, string fileNameBoth)
        {
            GetPeaks_DLL.DataFIFO.StringListToDisk writer = new GetPeaks_DLL.DataFIFO.StringListToDisk();


            List<string> dataOnlyOut = new List<string>();
            List<string> libraryOnlyOut = new List<string>();
            List<string> bothOut = new List<string>();

            dataOnlyOut.Add(fileName2Data);
            libraryOnlyOut.Add(fileName1Library);
            bothOut.Add(fileNameBoth);

            string seperator = @",";

            foreach (DatabaseIsotopeObject iObject in onlyInLibrary)
            {
                string line = "";
                line += iObject.scan_num + seperator;
                line += iObject.mz + seperator;
                line += iObject.MonoIsotopicMass + seperator;
                line += iObject.charge + seperator;
                line += iObject.fit;
                libraryOnlyOut.Add(line);
            }

            foreach (DatabaseIsotopeObject iObject in onlyInData)
            {
                string line = "";
                line += iObject.scan_num + seperator;
                line += iObject.mz + seperator;
                line += iObject.MonoIsotopicMass + seperator;
                line += iObject.charge + seperator;
                line += iObject.fit;
                dataOnlyOut.Add(line);
            }

            foreach (DatabaseIsotopeObject iObject in inBothLibraryAndData)
            {
                string line = "";
                line += iObject.scan_num + seperator;
                line += iObject.mz + seperator;
                line += iObject.MonoIsotopicMass + seperator;
                line += iObject.charge + seperator;
                line += iObject.fit;
                bothOut.Add(line);
            }

            writer.toDiskStringList(datafileName, dataOnlyOut);
            writer.toDiskStringList(libraryfileName, libraryOnlyOut);
            writer.toDiskStringList(fileNameBoth, bothOut);
        }

        private static List<DatabaseIsotopeObject> DatabaseIsotopeObjects(string fileName)
        {
            DatabaseLayer layer = new DatabaseLayer();
            int numberOfEntries = layer.ReadDatabaseSize(fileName, "TableInfo", "MonoCount");

            List<DatabaseIsotopeObject> monosFromFile;
            DatabaseReader reader = new DatabaseReader();
            //List<DatabaseIsotopeObject> monosFromFile = new List<DatabaseIsotopeObject>();



            reader.readAllMonoIsotopeData(fileName, numberOfEntries, out monosFromFile);

            /*
            
            
            
            

            DatabaseIsotopeObject fromFile;
            DatabaseReader reader = new DatabaseReader();
            List<DatabaseIsotopeObject> monosFromFile = new List<DatabaseIsotopeObject>();

            for (int i = 0; i < numberOfEntries; i++)
            {
                reader.readMonoIsotopeData(fileName, 1, out fromFile);
                monosFromFile.Add(fromFile);
                Console.WriteLine(i);
            }
             */

            return monosFromFile;
        }

        enum FileType
        {
            txt,
            SQLite
        }
    }
}
