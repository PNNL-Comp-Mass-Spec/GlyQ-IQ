using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace FilesAndFolders
{
    public class UnitTests
    {
        [Test]
        public void CheckSummingFiles()
        {
            string workingDirectory = @"D:\PNNL\Projects\GlyQ-IQ Paper\Part 1\Summing\Results";
            
            List<string> currentFileNames = GetInfo.GetSummingFiles();

            CheckFilesOnDisk(workingDirectory, currentFileNames);
        }

       


        private static void CheckFilesOnDisk(string workingDirectory, List<string> currentFileNames)
        {
            int fileCount = 0;

            for (int i = 0; i < currentFileNames.Count; i++)
            {
                string testPath = Path.Combine(workingDirectory, currentFileNames[i]) + ".txt";


                if (File.Exists(testPath))
                {
                    Console.WriteLine(testPath);
                    fileCount++;
                }
            }
            Assert.AreEqual(fileCount, currentFileNames.Count);
        }
    }
}
