using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using YAFMS_DB.GetPeaks;

namespace GetPeaks_DLL.SQLite.DataTransferObjects
{
    public class DatabaseScanObject : DatabaseTransferObject
    {

        public int IndexId { get; set; }

        public int Scan { get; set; }

        public int MSLevel { get; set; }

        public int ParentScan { get; set; }

        public int Peaks { get; set; }

        public int PeaksProcessed { get; set; }

        public string PeakProcessingLevel { get; set; }

        public DatabaseScanObject()
        {
            Columns.Add("IndexID");
            Columns.Add("Scan");
            Columns.Add("MSLevel");
            Columns.Add("ParentScan");
            Columns.Add("Peaks");
            Columns.Add("PeaksProcessed");
            Columns.Add("PeakProcessingLevel");

            IndexedColumns.Add("Scan");

            ValuesTypes.Add(DbType.Int32);
            ValuesTypes.Add(DbType.Int32);
            ValuesTypes.Add(DbType.Int32);
            ValuesTypes.Add(DbType.Int32);
            ValuesTypes.Add(DbType.Int32);
            ValuesTypes.Add(DbType.Int32);
            ValuesTypes.Add(DbType.String);

            TableName = "T_Scans";
        }
    }

    public class DatabaseScanObjectList : DatabaseTransferObjectList
    {
        public override List<DatabaseTransferObject> DatabaseTransferObjects { get; set; }

        public DatabaseScanObjectList()
        {
            DatabaseTransferObjects = new List<DatabaseTransferObject>();
        }
    }
}
