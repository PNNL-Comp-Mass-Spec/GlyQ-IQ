using System.Collections.Generic;
using System.IO;
using GetPeaksDllLite.DataFIFO;
using FilesAndFolders;

namespace FindMissingFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            string workingDirectory = @"Z:\";

            string exdendedbase = "iqresults";

            int startID = 1;

            int finalID = 19383;

           
            bool getFolders = false;
            if (getFolders)
            {
                string[] workingDirectories = Directory.GetDirectories(@"Z:\");
                List<string> folderList = new List<string>();
                foreach (var directory in workingDirectories)
                {
                    folderList.Add(Path.GetFileName(directory));
                }
                StringListToDisk writer = new StringListToDisk();
                string fileName = "folders.txt";
                writer.toDiskStringList(workingDirectory + @"\" + fileName, folderList);
            }

            List<string> workingDirectoryPile = GetInfo.GetPICFolders();

            List<string> fileNameBases = GetInfo.DatasetID26();

            for (int i = 0; i < workingDirectoryPile.Count; i++)
            {
                MissingFilesDetection.WriteToDisk(workingDirectory, workingDirectoryPile[i], fileNameBases[i], exdendedbase, startID, finalID);
            }
        }
    }
}
