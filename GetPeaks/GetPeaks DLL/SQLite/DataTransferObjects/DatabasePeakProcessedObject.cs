using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using YAFMS_DB.GetPeaks;

namespace GetPeaks_DLL.SQLite.DataTransferObjects
{
    public class DatabasePeakProcessedObject : DatabaseTransferObject
    {
        public int ScanNum { get; set; }

        public int PeakNumber { get; set; }

        public double XValue { get; set; }

        public double Height { get; set; }

        /// <summary>
        /// if we can get it. 
        /// </summary>
        public int Charge { get; set; }

        public double LocalSignalToNoise { get; set; }

        public double Background { get; set; }

        public double Width { get; set; }

        public double LocalLowestMinimaHeight { get; set; }

        public double SignalToBackground { get; set; }

        public double SignalToNoiseGlobal { get; set; }

        public double SignalToNoiseLocalMinima { get; set; }

        

        public DatabasePeakProcessedObject()
        {
            Columns.Add("Scan");
            Columns.Add("PeakNumber");
            Columns.Add("XValue");
            Columns.Add("Height");
            Columns.Add("Charge");
            Columns.Add("LocalSignalToNoise");
            Columns.Add("Background");
            Columns.Add("Width");
            Columns.Add("LocalLowestMinimaHeight");
            Columns.Add("SignalToBackground");
            Columns.Add("SignalToNoiseGlobal");
            Columns.Add("SignalToNoiseLocalMinima");

            IndexedColumns.Add("Scan");
            IndexedColumns.Add("XValue");

            ValuesTypes.Add(DbType.Int32);
            ValuesTypes.Add(DbType.Int32);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Int32);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);

            TableName = "T_Scan_Peaks";
        }
    }

    public class DatabaseProcessedPeakObjectList : DatabaseTransferObjectList
    {
        public override List<DatabaseTransferObject> DatabaseTransferObjects { get; set; }

        public DatabaseProcessedPeakObjectList()
        {
            DatabaseTransferObjects = new List<DatabaseTransferObject>();
        }
    }
}

