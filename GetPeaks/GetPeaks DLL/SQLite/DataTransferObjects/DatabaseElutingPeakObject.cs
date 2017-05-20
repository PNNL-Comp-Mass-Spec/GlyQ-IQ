using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using YAFMS_DB.GetPeaks;

namespace GetPeaks_DLL.SQLite.DataTransferObjects
{
    public class DatabaseElutingPeakObject : DatabaseTransferObject
    {
        public int ElutingPeakID { get; set; }
        public double ElutingPeakMass { get; set; }

        public int ElutingPeakScanStart { get; set; }
        public int ElutingPeakScanEnd { get; set; }
        public int ElutingPeakScanMaxIntensity { get; set; }

        public int ElutingPeakNumberofPeaks { get; set; }
        public int ElutingPeakNumberOfPeaksFlag { get; set; }
        public string ElutingPeakNumberOfPeaksMode { get; set; }

        public double ElutingPeakSummedIntensity { get; set; }
        public double ElutingPeakIntensityAggregate { get; set; }

        public DatabaseElutingPeakObject()
        {
            Columns.Add("ElutingPeakID");
            Columns.Add("ElutingPeakMass");
            Columns.Add("ElutingPeakScanStart");
            Columns.Add("ElutingPeakScanEnd");
            Columns.Add("ElutingPeakScanMaxIntensity");
            Columns.Add("ElutingPeakNumberofPeaks");
            Columns.Add("ElutingPeakNumberOfPeaksFlag");
            Columns.Add("ElutingPeakNumberOfPeaksMode");
            Columns.Add("ElutingPeakSummedIntensity");
            Columns.Add("ElutingPeakIntensityAggregate");

            IndexedColumns.Add("ElutingPeakMass");

            ValuesTypes.Add(DbType.Int32);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Int32);
            ValuesTypes.Add(DbType.Int32);
            ValuesTypes.Add(DbType.Int32);
            ValuesTypes.Add(DbType.Int32);
            ValuesTypes.Add(DbType.Int32);
            ValuesTypes.Add(DbType.String);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);

            TableName = "ElutingPeakTable";
        }
    }

    public class DatabaseElutingPeakObjectList : DatabaseTransferObjectList
    {
        public override List<DatabaseTransferObject> DatabaseTransferObjects { get; set; }

        public DatabaseElutingPeakObjectList()
        {
            DatabaseTransferObjects = new List<DatabaseTransferObject>();
        }
    }
}
