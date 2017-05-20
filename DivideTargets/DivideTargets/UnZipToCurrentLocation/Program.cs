using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ionic.Zip;
using SleepDLL;

namespace UnZipToCurrentLocation
{
    class Program
    {
        static void Main(string[] args)
        {
            string zippedFile = args[0];//F:\AzureStorageExplorer.zip
            string destinationFolder = System.IO.Directory.GetCurrentDirectory();

            if (System.IO.File.Exists(destinationFolder + @"\" + zippedFile))
            {
                try
                {
                    System.IO.File.Delete(destinationFolder + @"\" + zippedFile);
                }
                catch (Exception)
                {
                }
            }

            using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(zippedFile))
            {
                zip.ExtractAll(destinationFolder, ExtractExistingFileAction.OverwriteSilently);
            }

            StringListToDisk writer = new StringListToDisk();
            List<string> directoryFile = new List<string>();
            directoryFile.Add(destinationFolder);
            string furrentDirectoryFile = destinationFolder + @"\" + "WhereAmI.txt";
            writer.toDiskStringList(furrentDirectoryFile,directoryFile);
        }
    }
}
