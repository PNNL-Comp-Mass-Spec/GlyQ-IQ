using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.SQLite.DataTransferObjects;

namespace GetPeaks_DLL.SQLite.OneLineCalls
{
    public static class GetThresholdedPeaksFromScan
    {
        public static List<DatabasePeakProcessedWithMZObject> Read(int scan, string fileName)
        {
            DatabaseLayer layer = new DatabaseLayer();
            string tablename = "T_Scans_Precursor_Peaks";
            DatabasePeakProcessedWithMZObject sampleObject = new DatabasePeakProcessedWithMZObject();
            List<DatabasePeakProcessedWithMZObject> results = layer.SK_ReadProcessedPeakWithMZ(fileName, scan, tablename, sampleObject);

            Console.WriteLine(results.Count);
            return results;
        }
    }
}
