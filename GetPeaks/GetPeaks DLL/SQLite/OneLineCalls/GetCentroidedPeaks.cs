using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.SQLite.DataTransferObjects;
using YAFMS_DB.GetPeaks;
using ConvertDatabaseTransferObject = GetPeaks_DLL.Functions.ConvertDatabaseTransferObject;

namespace GetPeaks_DLL.SQLite.OneLineCalls
{
    //YAFMS DB now
    
    //public static class GetCentroidedPeaks
    //{
    //    /// <summary>
    //    /// Scan number corresponds to the tandem scan number
    //    /// </summary>
    //    /// <param name="scan"></param>
    //    /// <param name="fileName"></param>
    //    /// <returns></returns>
    //    public static List<DatabasePeakCentricObject> Read(int scan, string fileName)
    //    {
    //        DatabaseLayer layer = new DatabaseLayer();

    //        List<DatabasePeakCentricObject> peaks = new List<DatabasePeakCentricObject>(); 

    //        //DatabasePeakCentricObject sampleObjectPeakCentric = new DatabasePeakCentricObject();
    //        DatabasePeakCentricObject sampleObjectPeakCentric = new DatabasePeakCentricObject();
    //        string tablenamePeaksCentric = "T_Peak_Centric";

    //        DatabasePeakCentricObjectList resultsPeaksCentric;
    //        bool returnMonoisotopicMassOnly = false;//false will pull all signal peaks
    //        layer.SK2_Final_SelectPeaksWithAttributes(fileName, tablenamePeaksCentric, scan, sampleObjectPeakCentric, out resultsPeaksCentric, returnMonoisotopicMassOnly);
    //        //layer.SK2_Final_SelectSimplePeaks(fileName, tablenamePeaksCentric, scan, sampleObjectPeakCentric, out resultsPeaksCentric, returnMonoisotopicMassOnly);

    //        //layer.SK2_Final_SelectPeak(fileName, tablenamePeaksCentric, scan, sampleObjectPeakCentric, out resultsPeaksCentric, returnMonoisotopicMassOnly);

            
           
    //        peaks = ConvertDatabaseTransferObject.ToDatabasePeakCentricObject(resultsPeaksCentric);
    //        //peaks = ConvertDatabaseTransferObject.ToDatabasePeakCentricObject(resultsPeaksCentric);
            
            
    //        Console.WriteLine("We have read in " + peaks.Count + " MS1 peaks");
    //        return peaks;
    //    }

    //    /// <summary>
    //    /// Scan number corresponds to the tandem scan number
    //    /// </summary>
    //    /// <param name="scan"></param>
    //    /// <param name="fileName"></param>
    //    /// <returns></returns>
    //    public static List<DatabasePeakCentricLiteObject> ReadLite(int scan, string fileName)
    //    {
    //        DatabaseLayer layer = new DatabaseLayer();

    //        List<DatabasePeakCentricLiteObject> peaks = new List<DatabasePeakCentricLiteObject>();

    //        DatabasePeakCentricLiteObject sampleObjectPeakCentric = new DatabasePeakCentricLiteObject();
    //        DatabaseScanCentricObject sampleObjectScanCentric = new DatabaseScanCentricObject();
    //        string tablenamePeaksCentric = "T_Peak_Centric";
    //        string tablenameScanCentric = "T_Scan_Centric";

    //        DatabasePeakCentricLiteObjectList resultsPeaksCentric;
    //        bool returnMonoisotopicMassOnly = false;//false will pull all signal peaks
    //        layer.SK2_Final_SelectSimplePeaks(fileName, tablenamePeaksCentric, tablenameScanCentric, scan, sampleObjectPeakCentric, sampleObjectScanCentric, out resultsPeaksCentric, returnMonoisotopicMassOnly);

    //        peaks = ConvertDatabaseTransferObject.ToDatabasePeakCentricLightObject(resultsPeaksCentric);


    //        Console.WriteLine("We have read in " + peaks.Count + " MS1 peaks");
    //        return peaks;
    //    }

    //}
}
