using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using SleepDLL;

namespace ZipFolders
{
    class Program
    {
        static void Main(string[] args)
        {
            //args[0] = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC";
            //args[1] = "FilesToZIP.txt";
            //string folderPath = args[0];
            //string fileWithNames = args[1];

            //string fileWithNames = @"F:\ScottK\ToPIC_Zip\FilesToZIP_F.txt";
            string fileWithNames = args[0];
            //zip data

            Console.WriteLine("Lets zip the files in thie folder:  " + fileWithNames);

            StringLoadTextFileLine reader = new StringLoadTextFileLine();
            List<string> foldersToZip = reader.SingleFileByLine(fileWithNames);

            //delete old files
            foreach (string individualfolder in foldersToZip)
            {
                String[] words = individualfolder.Split(',');
                string zippedFolderDestination = words[1];
                if (System.IO.File.Exists(zippedFolderDestination))
                {
                    System.IO.File.Delete(zippedFolderDestination);
                }
               
            }
            int counter = 0;
            foreach (string individualfolder in foldersToZip)
            {
                String[] words = individualfolder.Split(',');



                Console.WriteLine("Zipping " + words[0] + " which is " + counter + " out of " + foldersToZip.Count);
                string folderToZip = words[0];
                string zippedFolderDestination = words[1];
                using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
                {
                    zip.AddDirectory(folderToZip);
                    zip.Save(zippedFolderDestination);
                }

                counter++;
            }

            Console.WriteLine("Done");
        }
    }
}
