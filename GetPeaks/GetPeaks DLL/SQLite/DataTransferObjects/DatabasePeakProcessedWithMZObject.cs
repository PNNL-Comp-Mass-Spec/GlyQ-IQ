using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using YAFMS_DB.GetPeaks;

namespace GetPeaks_DLL.SQLite.DataTransferObjects
{
    public class DatabasePeakProcessedWithMZObject : DatabaseTransferObject
    {
        public int ScanNumberTandem { get; set; }

        public int ScanNumberPrecursor { get; set; }

        public int PeakNumber { get; set; }

        public double XValue { get; set; }

        public double Height { get; set; }

        public double LocalSignalToNoise { get; set; }

        public double Background { get; set; }

        public double Width { get; set; }

        public double LocalLowestMinimaHeight { get; set; }

        public double SignalToBackground { get; set; }

        public double SignalToNoiseGlobal { get; set; }

        public double SignalToNoiseLocalMinima { get; set; }

        public double XValueRaw { get; set; }

        public int Charge { get; set; }

        public DatabasePeakProcessedWithMZObject()
        {
            Columns.Add("TandemScan");
            Columns.Add("PrecursorScan");
            Columns.Add("PeakNumber");
            Columns.Add("XValue");
            Columns.Add("XValueRaw");
            Columns.Add("Charge");
            
            Columns.Add("Height");
            Columns.Add("LocalSignalToNoise");
            Columns.Add("Background");
            Columns.Add("Width");
            Columns.Add("LocalLowestMinimaHeight");
            
            Columns.Add("SignalToBackground");
            Columns.Add("SignalToNoiseGlobal");
            Columns.Add("SignalToNoiseLocalMinima");

            IndexedColumns.Add("TandemScan");
            IndexedColumns.Add("XValue");

            ValuesTypes.Add(DbType.Int32);
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
            ValuesTypes.Add(DbType.Double);

            TableName = "T_Scan_Precursor_Peaks";
        }

        public class DatabasePeakProcessedWithMZObjectList : DatabaseTransferObjectList
        {
            public override List<DatabaseTransferObject> DatabaseTransferObjects { get; set; }

            public DatabasePeakProcessedWithMZObjectList()
            {
                DatabaseTransferObjects = new List<DatabaseTransferObject>();
            }
        }
    }
}
