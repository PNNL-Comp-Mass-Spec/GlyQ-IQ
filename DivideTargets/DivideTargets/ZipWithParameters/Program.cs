using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZipWithParameters
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Zipping " + args[0]);
            string folderToZip = args[0];
            string zippedFolderDestination = args[1];
            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
            {
                zip.AddDirectory(folderToZip);
                zip.Save(zippedFolderDestination);
            }
        }
    }
}
