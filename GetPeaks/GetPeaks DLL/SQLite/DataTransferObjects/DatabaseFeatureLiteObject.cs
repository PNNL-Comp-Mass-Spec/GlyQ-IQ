using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using YAFMS_DB.GetPeaks;

namespace GetPeaks_DLL.SQLite.DataTransferObjects
{
    public class DatabaseFeatureLiteObject : DatabaseTransferObject
    {
        public int ID { get; set; }
        public float Abundance { get; set; }
        public int ChargeState { get; set; }
        public double DriftTime { get; set; }
        public double MassMonoisotopic { get; set; }
        public double Mass { get; set; }
        public double RetentionTime { get; set; }
        public double Score { get; set; }
        

        public DatabaseFeatureLiteObject()
        {
            Columns.Add("ID");
            Columns.Add("Abundance");
            Columns.Add("ChargeState");
            Columns.Add("DriftTime");
            Columns.Add("MonoisotopicMass");
            Columns.Add("Mass");
            Columns.Add("RetentionTime");
            Columns.Add("Score");
    
            IndexedColumns.Add("Mass");
            IndexedColumns.Add("Abundance");

            ValuesTypes.Add(DbType.Int32);
            ValuesTypes.Add(DbType.Single);
            ValuesTypes.Add(DbType.Int32);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);

            TableName = "FeatureLiteTable";
        }
    }

    public class DatabaseFeatureLiteObjectList : DatabaseTransferObjectList
    {
        public override List<DatabaseTransferObject> DatabaseTransferObjects { get; set; }

        public DatabaseFeatureLiteObjectList()
        {
            DatabaseTransferObjects = new List<DatabaseTransferObject>();
        }
    }
}
