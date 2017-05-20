using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Functions
{
    public static class CopyFile
    {

        public static void RAW(string inputDatasetFileName, int coresPerComputer)
        {
            string fileName = inputDatasetFileName;

            string inputDatasetFileNameNoEnding = RemoveEnding.RAW(inputDatasetFileName);

            for (int i = 0; i < coresPerComputer; i++) //because of the bounus engine
            {


                string newFileName = inputDatasetFileNameNoEnding + " (" + i + ").RAW";

                //string sourcePath = @"C:\Users\Public\TestFolder";
                //string targetPath = @"C:\Users\Public\TestFolder\SubDir";

                // Use Path class to manipulate file and directory paths. 
                //string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
                //string destFile = System.IO.Path.Combine(targetPath, fileName);

                // To copy a folder's contents to a new location: 
                // Create a new target folder, if necessary. 
                //if (!System.IO.Directory.Exists(targetPath))
                //{
                //    System.IO.Directory.CreateDirectory(targetPath);
                //}


                string sourceFile = fileName;
                string destFile = newFileName;
                if (System.IO.File.Exists(destFile))
                {
                    Console.WriteLine("Initial Data File (" + i + ") allready exists");
                }
                else
                {
                    Console.WriteLine("Initial Data File (" + i + ") does not exist... create file");
                    // To copy a file to another location and  
                    // overwrite the destination file if it already exists.
                    System.IO.File.Copy(sourceFile, destFile, true);
                }
            }
        }

        
    }
}
