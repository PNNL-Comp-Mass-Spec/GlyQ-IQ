using System;
using System.Collections.Generic;
using System.IO;
using DivideTargetsLibraryX64.FromGetPeaks;

namespace CheckLocks
{
    public class ParametersLocks
    {
        public List<string> LockFiles { get; set; }

        public List<string> ClosedLockFiles { get; set; }

        public ParametersLocks(string pathToController)
        {
            
            LockFiles = new List<string>();
            ClosedLockFiles = new List<string>();

            StringLoadTextFileLine reader = new StringLoadTextFileLine();
            List<string> strings = reader.SingleFileByLine(pathToController);

            for(int i=1;i<strings.Count;i++)
            {
                string name = strings[i];
                LockFiles.Add(name);
            }
        }

        public void DetectClosedLocksFromLocksFiles(string locksFolderPath)
        {
            if (LockFiles.Count > 0)
            {
                foreach (string lockFile in LockFiles)
                {

                    string stringEnding = ".txt";
                    char[] ending = stringEnding.ToCharArray();

                    string baseName = lockFile.TrimEnd(ending);
                    string closedLockName = baseName + "_Done.txt";

                    if (File.Exists(locksFolderPath + @"/" + closedLockName))
                    {
                        ClosedLockFiles.Add(closedLockName);

                        Console.WriteLine(closedLockName + " Found");
                    }


                }
            }
        }

    }
}
