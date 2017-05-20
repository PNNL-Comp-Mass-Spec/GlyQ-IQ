using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQGlyQ.FIFO;
using IQGlyQ.Objects;

namespace MultiDatasetToolBox.FIFO
{
    public static class CrossTabBuilder
    {
        public static List<string> BuildMatrix(List<List<double>> IsoFitsPile, List<string> headers, string filler)
        {
            //checks
            if (headers != null && headers.Count > 0)
            {
            }

            string seperator = "\t";
            int numberOfDatasets = IsoFitsPile.Count;

            //find max length from all data
            int maxRowLength = 0;
            List<string> maxtrixToWrite = new List<string>();
            for (int i = 0; i < IsoFitsPile.Count; i++)
            {
                List<double> fitScores = IsoFitsPile[i];

                if (fitScores.Count > maxRowLength)
                {
                    maxRowLength = fitScores.Count;
                }
            }

            //header
            maxtrixToWrite.Add(String.Join(seperator, headers));

            for (int row = 0; row < maxRowLength; row++)
            {
                string[] oneLineWide = new string[numberOfDatasets];

                for (int column = 0; column < numberOfDatasets; column++)
                {
                    List<double> fitScores = IsoFitsPile[column];
                    if (fitScores.Count > row)
                    {
                        oneLineWide[column] = fitScores[row].ToString();
                    }
                    else
                    {
                        oneLineWide[column] = filler.ToString();
                    }
                }

                string currentLine = String.Join(seperator, oneLineWide);

                maxtrixToWrite.Add(currentLine);
            }

            Console.WriteLine(maxtrixToWrite.Count);

            return maxtrixToWrite;
        }

        public static List<string> BuildOneToManyResults(List<Dataset> datasetWithResultsToPrint, string header)
        {
            //checks
            

            string seperator = "\t";
            int numberOfDatasets = datasetWithResultsToPrint.Count;

            //string header = String.Join(seperator, headers);

            //find max length from all data
            
            List<string> listToWrite = new List<string>();
            for (int i = 0; i < numberOfDatasets; i++)
            {
                Dataset currentDataset = datasetWithResultsToPrint[i];
                List<GlyQIqResult> data = currentDataset.results;

                listToWrite.Add(currentDataset.DataSetName);
                listToWrite.Add(header);
                foreach (var glyQIqResult in data)
                {
                    listToWrite.Add(glyQIqResult.GlyQIqResultToString(seperator));
                }
            }

            

            Console.WriteLine(listToWrite.Count);

            return listToWrite;
        }

    }
}
