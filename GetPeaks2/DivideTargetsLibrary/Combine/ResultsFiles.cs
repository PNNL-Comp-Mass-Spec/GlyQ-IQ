using System;
using System.Collections.Generic;
using System.Linq;
using GetPeaksDllLite.DataFIFO;

namespace DivideTargetsLibrary.Combine
{
    public static class ResultsFiles
    {
        public static void ConsolidateFiles(CombineParameters parameters)
        {
            List<DataHolderForSort> dataPile = new List<DataHolderForSort>();

            string header = null;

            foreach (string file in parameters.InputPaths)
            {
                GetPeaksDllLite.DataFIFO.StringLoadTextFileLine reader = new GetPeaksDllLite.DataFIFO.StringLoadTextFileLine();
                List<string> lines = reader.SingleFileByLine(file);

                if (lines.Count > 0)
                {
                    header = lines[0];

                    List<string> words = header.Split('\t').ToList();


                    Console.WriteLine("There are " + words.Count + " headers");

                    for (int i = 1; i < lines.Count; i++) //skip header
                    {
                        string line = lines[i];
                        List<string> dataWords = line.Split('\t').ToList();
                        DataHolderForSort data = new DataHolderForSort();
                        data.SortKeyDouble = Convert.ToDouble(dataWords[0]); //composition
                        data.SortKeyString = dataWords[36]; //final answer typer
                        data.SortKeyString2 = dataWords[38]; //final answer
                        data.LineOfText = line;

                        dataPile.Add(data);
                    }

                    Console.WriteLine("The DataPile is " + dataPile.Count + " lines long");
                }
            }

            Console.WriteLine("We are done reading in data");
            //sort
            List<DataHolderForSort> dataPileSortedByComposition = dataPile.OrderBy(p => p.SortKeyDouble).ToList();

            List<DataHolderForSort> dataPileSortedByFinalType = dataPileSortedByComposition.OrderBy(p => p.SortKeyString2).ToList();

            List<DataHolderForSort> dataPileSortedByFinalAnswer = dataPileSortedByFinalType.OrderByDescending(p => p.SortKeyString).ToList();

            //convert to stringlist
            List<string> stringsToWrite = new List<string>();
            stringsToWrite.Add(header);

            Console.WriteLine("Compile data to write");

            foreach (DataHolderForSort data in dataPileSortedByFinalAnswer)
            {
                stringsToWrite.Add(data.LineOfText);
            }

            Console.WriteLine("Writing data...");
            //write
            StringListToDisk writer = new StringListToDisk();
            writer.toDiskStringList(parameters.OutputPath, stringsToWrite);

            Console.WriteLine("Done!");
        }
    }
}
