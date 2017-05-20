using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TessingReferencesDLls
{
    class Program
    {
        static void Main(string[] args)
        {
            //string fileName = args[0];
            //string outFileName = args[1];

            string outFileName = @"\\picfs.pnl.gov\projects\DMS\PIC_HPC\Hot\DeceptionTests\TestingReferencesDLL\DllTest.txt";
            
            List<string> dataToWrite = new List<string>();
            dataToWrite.Add("Running");
            dataToWrite.Add("");
            StringListToDisk writer = new StringListToDisk();

            writer.toDiskStringList(outFileName, dataToWrite);
        }
    }
}
