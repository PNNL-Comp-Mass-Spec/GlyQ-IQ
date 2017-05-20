using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.SQLite.DataTransferObjects;

namespace GetPeaks_DLL.SQLite.OneLineCalls
{
    public static class GetFragmentInfo
    {
        ///// <summary>
        ///// scan number corresponds to any scan number
        ///// </summary>
        ///// <param name="scan"></param>
        ///// <param name="fileName"></param>
        ///// <returns></returns>
        //public static List<DatabaseFragmentCentricObject> Read(string fileName)
        //{

        //    //string tablename = "T_Scans";
        //    //DatabaseScanObject sampleObject = new DatabaseScanObject();
        //    //DatabaseLayer layer = new DatabaseLayer();
        //    //List<DatabaseScanObject> results = layer.SK_SelectScansByScan(fileName, scan, tablename, sampleObject);

        //    string tablename = "T_Fragment_Centric";
        //    DatabaseFragmentCentricObject sampleObject = new DatabaseFragmentCentricObject();
        //    DatabaseLayer layer = new DatabaseLayer();

        //    List<DatabaseFragmentCentricObject> results = layer.SK2_SelectFragment(fileName, tablename, sampleObject);
        //    int t = results.Count;
        //    Console.WriteLine(results.Count);
        //    return results;
        //}
    }
}
