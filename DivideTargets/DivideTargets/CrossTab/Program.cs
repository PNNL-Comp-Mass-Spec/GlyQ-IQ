using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DivideTargetsLibraryX64.Combine;
using DivideTargetsLibraryX64.FromGetPeaks;
using DivideTargetsLibraryX64.Objects;

namespace CrossTab
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileDirectory = @"D:\PNNL\Projects\Leaf_Cutter_Ants\Dec\12-20-13\Summary";
            string fileNameContainingFiles = "Files.txt";
            string outputFileName = "CrossTabAnt";
            int uniqueKey = 1;//0 is start
            int columnToAppend = 33;
            string fillValueForMissingData = "0";
            int headderOffset = 1;

            List<CrossTabObject> dataPile = LoadData(fileNameContainingFiles, fileDirectory, uniqueKey);

            //make master list from unique id
            List<string> uniqueSpine = GenerateUniqueSpine(dataPile);

            string comma = ",";

            //initialize output cross tab
            List<string> resultCrossTab = new List<string>();

            //file header
            string headder = ",";
            for(int i=0;i<dataPile.Count;i++)
            {
                CrossTabObject currentCrossTab = dataPile[i];
                headder = headder + currentCrossTab.FileName;

                if (i < dataPile.Count - 1)
                {
                    headder = headder + comma;
                }
            }

            resultCrossTab.Add(headder);

            foreach (string idInSpine in uniqueSpine)
            {
                string infoToAdd = idInSpine + comma;
                resultCrossTab.Add(infoToAdd);
            }

            //populate data
            for (int i = 0; i < dataPile.Count; i++)
            {
                for (int j = 0; j < uniqueSpine.Count;j++ )
                {
                    string currentUniqueID = uniqueSpine[j];
                    CrossTabObject currentDataset = dataPile[i];
                    DataHolderForSort pullHitIfItExists = (from n in currentDataset.DataPile where n.SortKeyString == currentUniqueID select n).FirstOrDefault();

                    string valueToAppend = "";
                    if (pullHitIfItExists != null)
                    {
                        string currentLineOfTextToParse = pullHitIfItExists.LineOfText;

                        char[] splitter = new char[] {'\t'};

                        string[] initalFullResults = currentLineOfTextToParse.Split(splitter);

                        valueToAppend = initalFullResults[columnToAppend];
                       
                    }
                    else
                    {
                        valueToAppend = fillValueForMissingData;
                    }

                    resultCrossTab[j + headderOffset] = resultCrossTab[j + headderOffset] + valueToAppend;
                    if (i < dataPile.Count - 1)//add comma
                    {
                        resultCrossTab[j + headderOffset] = resultCrossTab[j + headderOffset] + comma;
                    }
                }
            }

            StringListToDisk writer = new StringListToDisk();
            writer.toDiskStringList(fileDirectory + @"\" + outputFileName + @".csv", resultCrossTab);

        }

        private static List<string> GenerateUniqueSpine(List<CrossTabObject> dataPile)
        {
            List<string> spine = new List<string>();

            foreach (CrossTabObject crossTabObject in dataPile)
            {
                foreach (DataHolderForSort data in crossTabObject.DataPile)
                {
                    spine.Add(data.SortKeyString);
                }
            }

            List<string> uniqueSpine = (from n in spine select n).Distinct().ToList();
            return uniqueSpine;
        }

        private static List<CrossTabObject> LoadData(string fileNameContainingFiles, string fileDirectory, int uniqueKey)
        {
            List<CrossTabObject> dataPile = new List<CrossTabObject>();
            //1.  load data and convert to objects

            StringLoadTextFileLine simpleTextloader = new StringLoadTextFileLine();
            List<string> linesLoaded = simpleTextloader.SingleFileByLine(fileDirectory + @"\" + fileNameContainingFiles);

            char[] comma = new char[] {','};
            List<string> fileNamesloaded = new List<string>();
            List<string> pathsLoaded = new List<string>();
            //split filesToLoad
            foreach (var line in linesLoaded)
            {
                string[] words = line.Split(comma);
                if (words.Length == 2)
                {
                    pathsLoaded.Add(words[0]);
                    fileNamesloaded.Add(words[1]);
                }
                else
                {
                    Console.WriteLine("missing filepath,filename");
                    Console.ReadKey();
                }
            }

            

            for (int i = 0; i < fileNamesloaded.Count; i++)
            {
                //1.  load data and convert to objects
                ResultsFilesCompostion loader = new ResultsFilesCompostion();


                string firstLine = null;
                List<DataHolderForSort> dataPile2 = loader.loadData2(pathsLoaded[i], out firstLine);

                List<DataHolderForSort> dataPileSortedByComposition = dataPile2.OrderBy(p => p.SortKeyComposition).ToList();

                Console.WriteLine(dataPileSortedByComposition.Count + " results were loaded");

                
                char[] splitter = new char[] {'\t'};

                foreach (DataHolderForSort data in dataPileSortedByComposition)
                {
                    string[] fullResults = data.LineOfText.Split(splitter, StringSplitOptions.None);

                    if (uniqueKey < fullResults.Length)
                    {
                        data.SortKeyString = fullResults[uniqueKey];
                    }
                    else
                    {
                        Console.WriteLine("The unique ID is too large.  The column is not in the data");
                        Console.ReadKey();
                    }
                }


                CrossTabObject currentCrossTab = new CrossTabObject();
                currentCrossTab.DataPile = dataPileSortedByComposition;
                currentCrossTab.FileName = fileNamesloaded[i];

                dataPile.Add(currentCrossTab);
            }

            return dataPile;
        }
    }
}
