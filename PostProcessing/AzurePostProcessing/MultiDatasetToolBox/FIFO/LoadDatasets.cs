using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GetPeaksDllLite.DataFIFO;
using IQGlyQ.FIFO;
using IQGlyQ.Objects;

namespace MultiDatasetToolBox.FIFO
{
    public static class LoadDatasets
    {

        public static List<Dataset> Load(List<string> currentFileNames, string workingDirectory)
        {
            int fileCount = 0;

            List<Dataset> datasetPile = new List<Dataset>();

            string header = "";
            for (int i = 0; i < currentFileNames.Count; i++)
            {
                string testPath = Path.Combine(workingDirectory, currentFileNames[i]) + ".txt";

                Dataset localDataset = new Dataset();
                localDataset.DataSetName = currentFileNames[i];

                datasetPile.Add(localDataset);

                if (File.Exists(testPath))
                {
                    List<GlyQIqResult> resultsIn = new List<GlyQIqResult>();

                    GetPeaksDllLite.DataFIFO.StringLoadTextFileLine reader = new StringLoadTextFileLine();

                    int successCount = 0;

                    List<string> lines = reader.SingleFileByLine(testPath);

                    Console.WriteLine(lines.Count);
                    fileCount++;


                    bool firstLine = true;

                    foreach (var line in lines)
                    {
                        if (firstLine)
                        {
                            header = line;
                        }
                        else
                        {
                            string[] words = line.Split('\t');

                            if (words.Length >= 45)
                            {
                                GlyQIqResult incomming = new GlyQIqResult(words);
                                resultsIn.Add(incomming);
                                successCount++;
                            }
                        }

                        firstLine = false; //procede with rest of tata
                    }

                    localDataset.headerToResults = header;
                    localDataset.results = resultsIn;

                    //Assert.AreEqual(successCount + 1, lines.Count);
                }
            }

           // Assert.AreEqual(fileCount, currentFileNames.Count);

            return datasetPile;
        }

    }
}
