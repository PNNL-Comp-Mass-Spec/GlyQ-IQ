using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using YAFMS_DB.GetPeaks;
using ScanCentric = GetPeaks_DLL.Objects.TandemMSObjects.ScanCentric;

namespace GetPeaks_DLL.SQLite.DataTransferObjects
{
   //in YAFMS DB
    
    //public class DatabaseScanCentricObject : DatabaseTransferObject
    //{
    // //public int ScanNumLc { get; set; }

    //    //public int ElutionTime { get; set; }

    //    //public int ScanNumDt { get; set; }

    //    //public int FrameNumberLc { get; set; }

    //    //public double DriftTime { get; set; }

    //    public ScanCentric ScanCentricData { get; set; }

    //    public DatabaseScanCentricObject()
    //    {
    //        ScanCentricData = new ScanCentric();
            
    //        Columns.Add("ScanID");
    //        Columns.Add("PeakID");//of precursor mass
    //        Columns.Add("ScanNumLc");
    //        Columns.Add("ElutionTime");

    //        Columns.Add("FrameNumberDt");
    //        Columns.Add("ScanNumDt");
    //        Columns.Add("DriftTime");

    //        Columns.Add("MsLevel");
    //        Columns.Add("ParentScanNumber");
    //        Columns.Add("TandemScanNumber");

    //        IndexedColumns.Add("ScanID");
    //        IndexedColumns.Add("ScanNumLc");

    //        ValuesTypes.Add(DbType.Int32);//ID
    //        ValuesTypes.Add(DbType.Int32);//ID for precursor

    //        ValuesTypes.Add(DbType.Int32);
    //        ValuesTypes.Add(DbType.Double);

    //        ValuesTypes.Add(DbType.Int32);
    //        ValuesTypes.Add(DbType.Int32);
    //        ValuesTypes.Add(DbType.Double);

    //        ValuesTypes.Add(DbType.Int32);
    //        ValuesTypes.Add(DbType.Int32);
    //        ValuesTypes.Add(DbType.Int32);

    //        TableName = "T_Scan_Centric";
    //    }
    //}

    //public class DatabaseScanCentricObjectList : DatabaseTransferObjectList
    //{
    //    public override List<DatabaseTransferObject> DatabaseTransferObjects { get; set; }

    //    public DatabaseScanCentricObjectList()
    //    {
    //        DatabaseTransferObjects = new List<DatabaseTransferObject>();
    //    }
    //}
}
