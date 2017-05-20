using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DivideTargetsLibraryX64.FromGetPeaks;

namespace DivideTargetsLibraryX64.Combine
{
    public class ResultsFilesCompostion
    {
        public List<Combine.DataHolderForSort> loadData2 (string file, out string header)
        {
            List<DataHolderForSort> dataPile = new List<DataHolderForSort>();

            header = null;

            StringLoadTextFileLine reader = new StringLoadTextFileLine();
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
                    //data.SortKeyComposition = Convert.ToDouble(dataWords[0]); //composition
                    data.SortKeyComposition = dataWords[1]; //composition
                    data.SortKeyString = dataWords[36]; //final answer typer
                    data.SortKeyString2 = dataWords[38]; //final answer
                    data.LineOfText = line;

                    dataPile.Add(data);
                }

                Console.WriteLine("The DataPile is " + dataPile.Count + " lines long");
            }


            Console.WriteLine("We are done reading in data");
            //sort
            //List<DataHolderForSort> dataPileSortedByComposition = dataPile.OrderBy(p => p.SortKeyComposition).ToList();
            
            return dataPile;
        }

        public List<Combine.DataHolderForSort> loadDataReject(string file, out string header)
        {
            List<DataHolderForSort> dataPile = new List<DataHolderForSort>();

            header = null;

            StringLoadTextFileLine reader = new StringLoadTextFileLine();
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
                    //data.SortKeyComposition = Convert.ToDouble(dataWords[0]); //composition
                    data.SortKeyComposition = dataWords[1]; //composition
                    data.SortKeyString = dataWords[2]; //code
                    data.SortKeyString2 = dataWords[39]; //manually denied = "No"
                    data.LineOfText = line;

                    dataPile.Add(data);
                }

                Console.WriteLine("The DataPile is " + dataPile.Count + " lines long");
            }


            Console.WriteLine("We are done reading in data");
            //sort
     //       List<DataHolderForSort> dataPileSortedByComposition = dataPile.OrderBy(p => p.SortKeyComposition).ToList();

     //       return dataPileSortedByComposition;
            return dataPile;
        }
    }
}
