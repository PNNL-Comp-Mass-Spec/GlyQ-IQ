using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeleteDirectory
{
    class Program
    {
        static void Main(string[] args)
        {
            string directory = args[0];

            string[] filesToDelete = System.IO.Directory.GetFiles(directory);
            System.Threading.Thread.Sleep(5000);

            foreach (string fileToDelete in filesToDelete)
            {
                Console.WriteLine("Delete file  " + fileToDelete + " and it is a " + fileToDelete);


                try
                {
                    System.IO.File.Delete(fileToDelete);
                }
                catch
                {

                }
            }

            System.IO.Directory.Delete(directory, true);

        }
    }
}
