using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.SQLite.DataTransferObjects;
using YAFMS_DB.GetPeaks;

namespace GetPeaks_DLL.SQLite.OneLineCalls
{
    public class GetAllScanNumbers
    {
        public static List<int> ReadFromDatabase(string fileName)
        {
            DatabaseLayer layer = new DatabaseLayer();

            //string tablename = "T_Scans";
            //List<int> results = layer.SK_ReadAllScans(fileName, tablename);
            
            string tablename = "T_Scan_Centric";
            List<int> results = layer.SK2_ReadAllScans(fileName, tablename);
            if (results != null)
            {
                results = results.Distinct().ToList();//removes duplicates
                results.Sort();
                return results;
            }
            
            return new List<int>();//if null return an empty list
        }

        public static List<DatabaseScanCentricObject> ReadAllFromDatabase(string fileName)
        {
            DatabaseLayer layer = new DatabaseLayer();

            //string tablename = "T_Scans";
            //List<int> results = layer.SK_ReadAllScans(fileName, tablename);

            string tablename = "T_Scan_Centric";
            List<DatabaseScanCentricObject> results = layer.SK2_ReadAllScanCentric(fileName, tablename);

            if(results !=null)
            {
                results = results.OrderBy(p => p.ScanCentricData.ScanID).ToList();
                return results;
            }
            return new List<DatabaseScanCentricObject>();//if null return an empty list
        }

        public static List<int> ReadScanNumbersFromMemory(List<DatabaseScanCentricObject> inMemory)
        {
            List<int> scanNumbers = inMemory.Select(c => c.ScanCentricData.ScanID).ToList();
            return scanNumbers;
        }

        public static List<int> ReadPrecursorPeakNumbersFromMemory(List<DatabaseScanCentricObject> inMemory)
        {
            List<int> precursorPeakIds = inMemory.Select(c => c.ScanCentricData.PeakID).ToList();
            return precursorPeakIds;
        }
    }
}
