using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ionic.Zip;

namespace UnZip
{
    class Program
    {
        static void Main(string[] args)
        {
            string zippedFile = args[0];
            string destinationFolder = args[1];

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
        }
    }
}
