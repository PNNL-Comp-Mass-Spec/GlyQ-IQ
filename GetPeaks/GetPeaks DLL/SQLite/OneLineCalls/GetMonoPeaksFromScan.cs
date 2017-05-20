using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.SQLite.DataTransferObjects;

namespace GetPeaks_DLL.SQLite.OneLineCalls
{
    public static class GetMonoPeaksFromScan
    {
        public static List<DatabasePeakProcessedObject> Read(int scan, string fileName)
        {
            DatabaseLayer layer = new DatabaseLayer();
            string tablename = "T_Scan_MonoPeaks";
            DatabasePeakProcessedObject sampleObjectPeaks = new DatabasePeakProcessedObject();
            List<DatabasePeakProcessedObject> results = layer.SK_ReadProcessedPeak(fileName, scan, tablename, sampleObjectPeaks);

            Console.WriteLine("We have read in " + results.Count + " tandem peaks");

            return results;
        }
    }
}
      