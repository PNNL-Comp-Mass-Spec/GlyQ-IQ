using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.SQLite.DataTransferObjects;
using YAFMS_DB.GetPeaks;

namespace GetPeaks_DLL.SQLite.OneLineCalls
{
    public static class GetScanInfo
    {
        /// <summary>
        /// scan number corresponds to any scan number
        /// </summary>
        /// <param name="scan"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<DatabaseScanObject> Read(int scan, string fileName)
        {
            string tablename = "T_Scans";
            DatabaseScanObject sampleObject = new DatabaseScanObject();
            DatabaseLayer layer = new DatabaseLayer();
            List<DatabaseScanObject> results = layer.SK_SelectScansByScan(fileName, scan, tablename, sampleObject);
            
            Console.WriteLine(results.Count);
            return results;
        }

        //public static List<DatabaseFragmentCentricObject> ReadCentric(int scan, string fileName)
        //{
        //    string tablename = "T_Fragment_Centric";
        //    DatabaseFragmentCentricObject sampleObject = new DatabaseFragmentCentricObject();
        //    DatabaseLayer layer = new DatabaseLayer();
        //    List<DatabaseFragmentCentricObject> results = layer.SK_Centric_SelectScansByScan(fileName, scan, tablename, sampleObject);

        //    Console.WriteLine(results.Count);
        //    return results;
        //}

        public static List<DatabaseScanCentricObject> ReadCentric(int scan, string fileName)
        {
            string tablename = "T_Scan_Centric";
            DatabaseScanCentricObject sampleObject = new DatabaseScanCentricObject();
            DatabaseLayer layer = new DatabaseLayer();
            //List<DatabaseScanObject> results = layer.SK_Centric_SelectScansByScan(fileName, scan, tablename, sampleObject);
            List<DatabaseScanCentricObject> results = layer.SK2_SelectScansByScan(fileName, scan, tablename, sampleObject);

            Console.WriteLine(results.Count);
            return results;
        }
    }
}
