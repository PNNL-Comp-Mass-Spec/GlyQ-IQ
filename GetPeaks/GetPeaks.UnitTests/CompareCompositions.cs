using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CompareContrastDLL;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.Objects;
using NUnit.Framework;

namespace GetPeaks.UnitTests
{
    
    public class CompareCompositions
    {
        [Test]
        public void compareCodesToBackboneDataFile()
        {
            //place to store data
            List<TitleWithList<int>> matrixOfData = new List<TitleWithList<int>>();
            List<TitleWithList<int>> matrixOfResults = new List<TitleWithList<int>>();

            string pathForBaseFile = @"X:\SpineL10PSA.txt";
            matrixOfData.Add(new TitleWithList<int>("Library_L10PSA", LoadAndConvert(pathForBaseFile).Data));
            matrixOfResults.Add(new TitleWithList<int>("Library_L10PSA", LoadAndConvert(pathForBaseFile).Data));
            int spineCount = LoadAndConvert(pathForBaseFile).Data.Count;

            string pathForPSAFile = @"X:\PSA.txt";
            matrixOfData.Add(LoadAndConvert(pathForPSAFile));

            string pathForL10File = @"X:\L10.txt";
            matrixOfData.Add(LoadAndConvert(pathForL10File));

            

            string pathForCompareFile1 = @"X:\SPIN_SN129.txt";
            matrixOfData.Add(LoadAndConvert(pathForCompareFile1));

            string pathForCompareFile2 = @"X:\Velos_SN129_CGNV.txt";
            matrixOfData.Add(LoadAndConvert(pathForCompareFile2));

            string pathForCompareFile3 = @"X:\MechrefMultiGlycanVelos.txt";
            matrixOfData.Add(LoadAndConvert(pathForCompareFile3));

            string pathForCompareFile4 = @"X:\VelosResoft5Scan.txt";
            matrixOfData.Add(LoadAndConvert(pathForCompareFile4));

            string pathForCompareFile5 = @"X:\SPIN_SN129_CG.txt";
            matrixOfData.Add(LoadAndConvert(pathForCompareFile5));

            string pathForCompareFile6 = @"X:\Velos_SN129_CG.txt";
            matrixOfData.Add(LoadAndConvert(pathForCompareFile6));

            string pathForSinMGFile = @"X:\MechrefMultiGlycanSPIN.txt";
            matrixOfData.Add(LoadAndConvert(pathForSinMGFile));

            string pathForMLFile = @"X:\MechrefLibrary.txt";
            matrixOfData.Add(LoadAndConvert(pathForMLFile));

            string pathForALFile = @"X:\Aldredge2012.txt";
            matrixOfData.Add(LoadAndConvert(pathForALFile));

            string pathForSPFile = @"X:\StumpoPlasma.txt";
            matrixOfData.Add(LoadAndConvert(pathForSPFile));

            string pathForRetro2009File = @"X:\RetrosyntheticList2009.txt";
            matrixOfData.Add(LoadAndConvert(pathForRetro2009File));

           
            //new set

            string pathForCelos_R_CGNV_File = @"X:\Velos_SN129_CGNV_withRelationships.txt";
            matrixOfData.Add(LoadAndConvert(pathForCelos_R_CGNV_File));

            string pathForVELOS_R_File = @"X:\Velos_SN129_CG.txt";
            matrixOfData.Add(LoadAndConvert(pathForVELOS_R_File));


            string pathForSPIN_R_File15 = @"X:\SPIN_SN129H15_CGNV_withRelationships.txt";//sum 15 hammer
            matrixOfData.Add(LoadAndConvert(pathForSPIN_R_File15));

            string pathForSPIN_R_File15CG = @"X:\SPIN_Sn129H15_CG.txt";//sum 15 hammer
            matrixOfData.Add(LoadAndConvert(pathForSPIN_R_File15CG));


            string pathForSPIN_R_File15avgCGNV = @"X:\SPIN_SN129H15avg_CGNV_withRelationships.txt";//sum 15 hammer with average 
            matrixOfData.Add(LoadAndConvert(pathForSPIN_R_File15avgCGNV));

            string pathForSPIN_R_File15avgCG = @"X:\SPIN_SN129H15avg_CG.txt";//sum 15 hammer with average 
            matrixOfData.Add(LoadAndConvert(pathForSPIN_R_File15avgCG));

            string pathForVelosHammer = @"X:\Velos_SN129_Hammer_L10PSA_withRelationships.txt";//sum 15 hammer with average 
            matrixOfData.Add(LoadAndConvert(pathForVelosHammer));

            string pathForVelosHammerCG = @"X:\Velos_SN129_Hammer_L10PSA CG.txt";//sum 15 hammer with average 
            matrixOfData.Add(LoadAndConvert(pathForVelosHammerCG));
            

            //string writePath = @"D:\Csharp\ConosleApps\LocalServer\appendedResults.csv";
            string writePath = @"X:\appendedResults.txt";
            string writeBonusePath = @"X:\appendedBonus.txt";

            int filesCount = matrixOfData.Count;//remove base later

            List<int> bonusList = new List<int>();
            List<string> bonusStrings = new List<string>();

            for (int i = 1; i < filesCount; i++)//1 is the base
            {
                List<int> localDataToCompare = matrixOfData[i].Data;

                Assert.IsTrue(localDataToCompare.Count>0);
                
                SetListsToCompare setter = new SetListsToCompare();
                CompareInputLists offToComparer = setter.SetThem(localDataToCompare, matrixOfData[0].Data);

                CompareResultsIndexes resultsIndexes = new CompareResultsIndexes();
                CompareController controller = new CompareController();
                CompareResultsValues results = controller.compareFX(offToComparer, 0.1, ref resultsIndexes);

                //reconstruct aligned list
                List<int> alignedColumn = new List<int>(new int[spineCount]);//this give us the zeroes
                for (int j = 0; j < resultsIndexes.IndexListBMatch.Count; j++)
                {
                    int indexOfSpotToFill = resultsIndexes.IndexListBMatch[j];
                    //int valueToFillWith = localDataToCompare[j];
                    int valueToFillWith = localDataToCompare[resultsIndexes.IndexListAMatch[j]];
                    alignedColumn[indexOfSpotToFill] = valueToFillWith;
                }

                if (resultsIndexes.IndexListAandNotB.Count>0)
                {
                    Console.WriteLine(matrixOfData[i].Title + " has " + resultsIndexes.IndexListAandNotB.Count + " extra glycans");
                    for (int j = 0; j < resultsIndexes.IndexListAandNotB.Count; j++)
                    {
                        int valueToFillWith = localDataToCompare[resultsIndexes.IndexListAandNotB[j]];
                        bonusList.Add(valueToFillWith);
                        bonusStrings.Add(valueToFillWith.ToString());
                    }
                }

                matrixOfResults.Add(new TitleWithList<int>(matrixOfData[i].Title, alignedColumn));
            }


            WriteAppendedData(writePath, matrixOfResults);

            //write bonuse glycans
            StringListToDisk writer = new StringListToDisk();
            writer.toDiskStringList(writeBonusePath, bonusStrings);

        }

        [Test]
        public void compareCodesToBackboneDataFileLibraryNegetaveSpace()
        {
            //place to store data
            List<TitleWithList<int>> matrixOfData = new List<TitleWithList<int>>();
            List<TitleWithList<int>> matrixOfResults = new List<TitleWithList<int>>();

            string pathForBaseFile = @"D:\PNNL\Projects\Libraries\NegativeSpaceLibrary\RetrosynteticList_L10PSA.txt";
            matrixOfData.Add(new TitleWithList<int>("Library_L10PSA", LoadAndConvert(pathForBaseFile).Data));
            matrixOfResults.Add(new TitleWithList<int>("Library_L10PSA", LoadAndConvert(pathForBaseFile).Data));
            int spineCount = LoadAndConvert(pathForBaseFile).Data.Count;

            string pathForPSAFile = @"D:\PNNL\Projects\Libraries\NegativeSpaceLibrary\CombinatorialList_L10PSA.txt";
            matrixOfData.Add(LoadAndConvert(pathForPSAFile));

  

            //string writePath = @"D:\Csharp\ConosleApps\LocalServer\appendedResults.csv";
            string writePath = @"D:\PNNL\Projects\Libraries\NegativeSpaceLibrary\appendedResults.txt";
            string writeBonusePath = @"D:\PNNL\Projects\Libraries\NegativeSpaceLibrary\appendedBonus.txt";

            int filesCount = matrixOfData.Count;//remove base later

            List<int> bonusList = new List<int>();
            List<string> bonusStrings = new List<string>();

            for (int i = 1; i < filesCount; i++)//1 is the base
            {
                List<int> localDataToCompare = matrixOfData[i].Data;

                Assert.IsTrue(localDataToCompare.Count > 0);

                SetListsToCompare setter = new SetListsToCompare();
                CompareInputLists offToComparer = setter.SetThem(localDataToCompare, matrixOfData[0].Data);

                CompareResultsIndexes resultsIndexes = new CompareResultsIndexes();
                CompareController controller = new CompareController();
                CompareResultsValues results = controller.compareFX(offToComparer, 0.1, ref resultsIndexes);

                //reconstruct aligned list
                List<int> alignedColumn = new List<int>(new int[spineCount]);//this give us the zeroes
                for (int j = 0; j < resultsIndexes.IndexListBMatch.Count; j++)
                {
                    int indexOfSpotToFill = resultsIndexes.IndexListBMatch[j];
                    //int valueToFillWith = localDataToCompare[j];
                    int valueToFillWith = localDataToCompare[resultsIndexes.IndexListAMatch[j]];
                    alignedColumn[indexOfSpotToFill] = valueToFillWith;
                }

                if (resultsIndexes.IndexListAandNotB.Count > 0)
                {
                    Console.WriteLine(matrixOfData[i].Title + " has " + resultsIndexes.IndexListAandNotB.Count + " extra glycans");
                    for (int j = 0; j < resultsIndexes.IndexListAandNotB.Count; j++)
                    {
                        int valueToFillWith = localDataToCompare[resultsIndexes.IndexListAandNotB[j]];
                        bonusList.Add(valueToFillWith);
                        bonusStrings.Add(valueToFillWith.ToString());
                    }
                }

                matrixOfResults.Add(new TitleWithList<int>(matrixOfData[i].Title, alignedColumn));
            }


            WriteAppendedData(writePath, matrixOfResults);

            //write bonuse glycans
            StringListToDisk writer = new StringListToDisk();
            writer.toDiskStringList(writeBonusePath, bonusStrings);

        }

        private static TitleWithList<int> LoadAndConvert(string pathForBaseFile)
        {
            StringLoadTextFileLine reader = new StringLoadTextFileLine();
            List<string> baseSpineStrings = reader.SingleFileByLine(pathForBaseFile);
            List<int> codeBase = new List<int>();
            foreach (string baseSpineString in baseSpineStrings)
            {
                codeBase.Add(Convert.ToInt32(baseSpineString));
            }

            TitleWithList<int> result = new TitleWithList<int>();
            result.Data = codeBase;
            result.Title = pathForBaseFile;
            return result;
        }

        [Test]
        public void compareCodesToBackboneSimple()
        {
            List<int> codeBase = new List<int>();
            codeBase.Add(10);
            codeBase.Add(20);
            codeBase.Add(30);
            codeBase.Add(40);
            codeBase.Add(50);

            

            List<int> codeToCompare = new List<int>();
            codeToCompare.Add(40);

            List<TitleWithList<int>> matrixOfData = new List<TitleWithList<int>>();
            matrixOfData.Add(new TitleWithList<int>("Library", codeBase));

            CompareContrastDLL.SetListsToCompare setter = new SetListsToCompare();
            CompareInputLists offToComparer = setter.SetThem(codeToCompare, codeBase);

            CompareContrastDLL.CompareResultsIndexes resultsIndexes = new CompareResultsIndexes();
            CompareContrastDLL.CompareController controller = new CompareController();
            CompareResultsValues results = controller.compareFX(offToComparer, 0.1, ref resultsIndexes);

            Assert.AreEqual(results.DataMatchingLibrary.Count,1);
            Assert.AreEqual(results.DataMatchingLibrary[0], 40);
            Assert.AreEqual(results.LibraryNotMatched.Count,4);
            
            //reconstruct aligned list
            List<int> alignedColumn = new List<int>(new int[codeBase.Count]);
            for(int i=0;i<resultsIndexes.IndexListBMatch.Count;i++)
            {
                int indexOfSpotToFill = resultsIndexes.IndexListBMatch[i];
                int valueToFillWith = codeToCompare[i];
                alignedColumn[indexOfSpotToFill] = valueToFillWith;
            }


            matrixOfData.Add(new TitleWithList<int>("data1", alignedColumn));

            //string writePath = @"D:\Csharp\ConosleApps\LocalServer\appendedResults.csv";
            string writePath = @"X:\appendedResults.txt";
            WriteAppendedData(writePath, matrixOfData);

        }

        private static void WriteAppendedData(string writePath, List<TitleWithList<int>> matrixOfData)
        {
            List<string> linesToPrint = new List<string>();
            string seperator = "\t";
            string header = "";
            for (int j = 0; j < matrixOfData.Count - 1; j++)
            {
                header += matrixOfData[j].Title + seperator;
            }
            header += matrixOfData[matrixOfData.Count - 1].Title;

            linesToPrint.Add(header);

            List<int> codeBase = matrixOfData[0].Data;

            for (int i = 0; i < codeBase.Count; i++)
            {
                string line = "";
                for (int j = 0; j < matrixOfData.Count - 1; j++)
                {
                    line = line + matrixOfData[j].Data[i] + seperator;
                }
                line = line + matrixOfData[matrixOfData.Count - 1].Data[i];

                linesToPrint.Add(line);
            }

            StringListToDisk writer = new StringListToDisk();
            writer.toDiskStringList(writePath, linesToPrint);
           
        }
    }
}
