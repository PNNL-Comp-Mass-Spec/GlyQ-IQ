using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GetPeaksDllLite.DataFIFO;

namespace FindMissingFiles
{
    public static class MissingFilesDetection
    {

        public static void WriteToDisk(string workingDirectory, string dataDirectory, string baseFileName, string exdendedbase, int startID, int finalID)
        {
            string[] existingFilNames = Directory.EnumerateFiles(workingDirectory + @"\" + dataDirectory, "*", SearchOption.AllDirectories).Select(Path.GetFileName).ToArray();
            
            string[] theoreticalFIleNames = new string[finalID - startID];

            int counter = 0;
            for (int i = startID; i < finalID; i++)
            {
                theoreticalFIleNames[counter] = baseFileName + "_" + exdendedbase + "_" + i + ".txt";
                counter++;
            }


            Dictionary<int, string> missingNames = new Dictionary<int, string>();

            for (int i = startID; i < finalID; i++)
            {
                string theoreticalFileName = baseFileName + "_" + exdendedbase + "_" + i + ".txt";
                if (!existingFilNames.Contains(theoreticalFileName))
                {
                    missingNames.Add(i, theoreticalFileName);
                }
                counter++;
            }


            Console.WriteLine("there are " + missingNames.Count + " files missing");

            StringListToDisk writer = new StringListToDisk();
            List<string> writeOut = new List<string>();
            foreach (var VARIABLE in missingNames)
            {
                writeOut.Add(VARIABLE.Key + "," + VARIABLE.Value);
            }
            string fileName = workingDirectory + @"\" + "0_MissingFiles_" + baseFileName + ".txt";
            writer.toDiskStringList(fileName, writeOut);
        }
    }
}
