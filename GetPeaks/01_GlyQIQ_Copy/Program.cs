using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GetPeaks_DLL.DataFIFO;

namespace _01_GlyQIQ_Copy
{
    class Program
    {
        static void Main(string[] args)
        {
            string coresName = args[0];

            int cores = Convert.ToInt32(coresName.Remove(0,4));
            string datafolderName = args[1];
            string dataFileName = args[2];
            string dataFileEnding = args[3];
            string targetsFolderName = args[4];
            string targetsfileName = args[5];
            string writeLocation = args[6];
            

            string dataPath = datafolderName + "\\" + dataFileName;
            string targetsPath = targetsFolderName + "\\" + targetsfileName;

            bool testDataFolder = Directory.Exists(datafolderName);
            bool testDataFile = File.Exists(dataPath);
            bool testDataFileEnding = dataFileEnding != null;
            bool testTargetsFolder = Directory.Exists(targetsFolderName);
            bool testTargets = File.Exists(targetsPath);
            bool testWriteLocation = Directory.Exists(writeLocation);
            bool testCores = cores > 0;

            if(testDataFolder)
                Console.WriteLine("Data Folder " + "Exists");

            if (testDataFile)
                Console.WriteLine("Data File " + "Exists");

            if (testDataFileEnding)
                Console.WriteLine("Data File Ending " + "Exists");

            if (testTargetsFolder)
                Console.WriteLine("Targets Folder " + "Exists");

            if (testTargets)
                Console.WriteLine("Targets File " + "Exists");

            if (testWriteLocation)
                Console.WriteLine("Write Location " + "Exists");

            if (testCores)
                Console.WriteLine("Cores = " + cores);

            if (testDataFolder && testDataFile && testDataFileEnding && testTargetsFolder && testTargets && testWriteLocation && testCores)
                Console.WriteLine(Environment.NewLine + "All Go");

            string batchFile1Name = "01_WriteCopyBatchFiles.txt";
            string outPutLocationCopy = writeLocation + "\\" + batchFile1Name;

            string batchFile2Name = "02_WriteParalellBatchFiles.txt";
            string outPutLocationParallel = writeLocation + "\\" + batchFile2Name;


            

            





            StringListToDisk writer = new StringListToDisk();

            List<string> linesForCopy = new List<string>();
            writer.toDiskStringList(outPutLocationCopy, linesForCopy);

            List<string> linesForParallel = new List<string>();
            writer.toDiskStringList(outPutLocationParallel, linesForParallel);

            
            //Test code

            for (int i = 0; i < cores; i++)
            {
                string coreName = "_" + i;
                char[] endingInChar = new char[dataFileEnding.Count()];
                for (int j = 0; j < dataFileEnding.Length; j++)
                {
                    endingInChar[j] = dataFileEnding[j];
                }

                string basedataFile = dataFileName.TrimEnd(endingInChar);
                string copiedName = basedataFile + coreName + dataFileEnding;

                string sourceFile = dataPath;
                string destFile = writeLocation + "\\" + copiedName;

                Console.WriteLine("Copying " + sourceFile + " to " + destFile + Environment.NewLine + "...");
                System.IO.File.Copy(sourceFile, destFile, true);
                Console.WriteLine("Copying Complete" + Environment.NewLine);

            }
                


            //Console.ReadKey();
        }
    }
}
