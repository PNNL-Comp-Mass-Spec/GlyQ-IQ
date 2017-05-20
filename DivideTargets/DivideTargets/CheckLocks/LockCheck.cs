using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DivideTargetsLibraryX64.FromGetPeaks;

namespace CheckLocks
{
    public static class LockCheck
    {
        public static bool AreLocksReady(string locksFolderPath, string controllerName, string pathToController)
        {
            bool go = false;

            if (File.Exists(pathToController))
            {
                ParametersLocks parameters = new ParametersLocks(pathToController);

                int locksCounter = 0;
                int completedCounter = 0;
                for (int i = 0; i < parameters.LockFiles.Count; i++)
                {
                    string lockFile = parameters.LockFiles[i];

                    string textFileEnding = ".txt";
                    char[] endingInCharTarget = ConvertEnding(textFileEnding);
                    string baseName = lockFile.TrimEnd(endingInCharTarget);
                    string completedName = baseName + "_Done.txt";


                    parameters.ClosedLockFiles.Add(completedName);


                    if (File.Exists(locksFolderPath + @"\" + lockFile))
                    {
                        Console.WriteLine("Exists  " + lockFile);
                        locksCounter++;
                    }
                    else
                    {
                        Console.WriteLine("Missing " + lockFile);
                    }

                    if (File.Exists(locksFolderPath + @"\" + completedName))
                    {
                        Console.WriteLine("Exists  " + completedName);
                        completedCounter++;
                    }
                    else
                    {
                        Console.WriteLine("Missing " + completedName);
                    }
                }

                Console.WriteLine("We have " + locksCounter + " of " + parameters.LockFiles.Count + " files still working");
                Console.WriteLine("We have " + completedCounter + " of " + parameters.LockFiles.Count + " files completed");

                if (completedCounter == parameters.LockFiles.Count)
                {
                    Console.WriteLine(Environment.NewLine + "We Can Continue");
                    go = true;
                }
                else
                {
                    Console.WriteLine(Environment.NewLine + "We need to wait...");
                }

                if (go == true)
                {
                    Console.WriteLine(Environment.NewLine + "Procede to write parameter file for combining...");

                   StringLoadTextFileLine reader = new StringLoadTextFileLine();

                    List<string> inputFileNames = new List<string>();
                    string outputFileName;
                    //write parameterFileFor Combiner
                    for (int i = 0; i < parameters.LockFiles.Count; i++)
                    {
                        string lockFile = parameters.LockFiles[i];

                        string textFileEnding = ".txt";
                        char[] endingInCharTarget = ConvertEnding(textFileEnding);
                        string baseName = lockFile.TrimEnd(endingInCharTarget);
                        string completedName = baseName + "_Done.txt";

                        List<string> fileToAdd = reader.SingleFileByLine(locksFolderPath + @"\" + completedName);
                        outputFileName = fileToAdd[1];
                        inputFileNames.Add(fileToAdd[2]);
                    }
                }
            }

            return go;
        }


        private static char[] ConvertEnding(string dataFileEnding)
        {
            char[] endingInCharData = new char[dataFileEnding.Count()];
            for (int j = 0; j < dataFileEnding.Length; j++)
            {
                endingInCharData[j] = dataFileEnding[j];
            }
            return endingInCharData;
        }
    }
}
