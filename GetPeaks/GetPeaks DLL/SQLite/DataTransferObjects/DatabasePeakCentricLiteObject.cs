using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Engine.PeakProcessing;
using PNNLOmics.Data;
using Peak = PNNLOmics.Data.Peak;

namespace GetPeaks_DLL.SQLite.DataTransferObjects
{
    public class DatabasePeakCentricLiteObject : DatabaseTransferObject
    {
        public PNNLOmics.Data.ProcessedPeak PeakData { get; set; }

        public DatabasePeakCentricLiteObject()
        {
            PeakData = new ProcessedPeak();
            
            Columns.Add("PeakID");
            Columns.Add("ScanID");

            Columns.Add("Mz");

            Columns.Add("Height");
            Columns.Add("Width");

            IndexedColumns.Add("ScanID,Mz,Height,Width");
            IndexedColumns.Add("Mz,ScanID,Height,Width");

            ValuesTypes.Add(DbType.Int32);
            ValuesTypes.Add(DbType.Int32);

            ValuesTypes.Add(DbType.Double);

            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);

            TableName = "T_Peak_Centric";
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

