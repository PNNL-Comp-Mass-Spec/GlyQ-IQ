using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SleepDLL;

namespace DeleteFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            StringLoadTextFileLine loadSpectraL = new StringLoadTextFileLine();
            List<string> linesFromParameterFile = new List<string>();
           
            //load strings
            linesFromParameterFile = loadSpectraL.SingleFileByLine(args[0]); //loads all isos

            foreach (string fileToDelete in linesFromParameterFile)
            {
                if(File.Exists(fileToDelete))
                {
                    File.Delete(fileToDelete);
                }
            }
            
        }
    }
}
