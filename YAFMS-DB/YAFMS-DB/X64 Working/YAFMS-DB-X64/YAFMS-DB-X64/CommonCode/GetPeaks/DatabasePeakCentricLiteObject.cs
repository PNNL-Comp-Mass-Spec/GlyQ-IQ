using System.Collections.Generic;
using System.Data;
using YAFMS_DB.GetPeaks;
using YAFMS_DB.PNNLOmics;

namespace YAFMS_DB.GetPeaks
{
    public class DatabasePeakCentricLiteObject : DatabaseTransferObject
    {
        public ProcessedPeakYAFMS PeakData { get; set; }

        public int PeakID { get; set; }

        public int ScanID { get; set; }

        public DatabasePeakCentricLiteObject()
        {
            PeakData = new ProcessedPeakYAFMS();
            
            Columns.Add("PeakID");
            Columns.Add("ScanID");

            Columns.Add("Mz");

            Columns.Add("Height");
            //Columns.Add("Width");

            //IndexedColumns.Add("ScanID,Mz,Height,Width");
            //IndexedColumns.Add("Mz,ScanID,Height,Width");

            IndexedColumns.Add("ScanID");
            IndexedColumns.Add("Mz");

            ValuesTypes.Add(DbType.Int32);
            ValuesTypes.Add(DbType.Int32);

            ValuesTypes.Add(DbType.Double);

            ValuesTypes.Add(DbType.Double);
            //ValuesTypes.Add(DbType.Double);

            //TableName = "T_Peak_Centric";
            TableName = "T_Scan_Peaks";
        }
    }

    public class DatabasePeakCentricLiteObjectList : DatabaseTransferObjectList
    {
        public override List<DatabaseTransferObject> DatabaseTransferObjects { get; set; }

        public DatabasePeakCentricLiteObject PeakCentricProperty { get; set; }//this is not seeen?

        public DatabasePeakCentricLiteObjectList()
        {
            DatabaseTransferObjects = new List<DatabaseTransferObject>();
            PeakCentricProperty = new DatabasePeakCentricLiteObject();
        }
    }
}

