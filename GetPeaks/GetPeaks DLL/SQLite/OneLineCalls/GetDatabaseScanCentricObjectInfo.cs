using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.SQLite.DataTransferObjects;
using YAFMS_DB.GetPeaks;

namespace GetPeaks_DLL.SQLite.OneLineCalls
{
    public static class GetDatabaseScanCentricObjectInfo
    {
        /// <summary>
        /// scan number corresponds to any scan number
        /// </summary>
        /// <param name="scan"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<DatabaseScanCentricObject> Read(int scan, string fileName)
        {
            string tablename = "T_Scan_Centric";
            DatabaseScanCentricObject sampleObject = new DatabaseScanCentricObject();
            DatabaseLayer layer = new DatabaseLayer();

            List<DatabaseScanCentricObject> results = layer.SK2_SelectScansByScan(fileName, scan, tablename, sampleObject);
            int t = results.Count;
            Console.WriteLine(results.Count);
            return results;
        }
    }
}
