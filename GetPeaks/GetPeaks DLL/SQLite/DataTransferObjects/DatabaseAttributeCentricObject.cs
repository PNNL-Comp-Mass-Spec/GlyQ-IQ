using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects.TandemMSObjects;

namespace GetPeaks_DLL.SQLite.DataTransferObjects
{
    //public class DatabaseAttributeCentricObject : DatabaseTransferObject
    //{
    //     //public bool isSignal { get; set; }

    //    //public bool isNoise { get; set; }

    //    //public bool isCentroided { get; set; }

    //    //public bool isMonoisotopic { get; set; }

    //    //public bool isIsotope { get; set; }

    //    //public bool isMostAbundant { get; set; }

    //    //public bool isCharged { get; set; }

    //    //public bool isCorrected { get; set; }

    //    //public bool isPrecursorMass { get; set; }

    //    public AttributeCentric AttributeCentricData { get; set; }

    //    public DatabaseAttributeCentricObject()
    //    {
    //        AttributeCentricData = new AttributeCentric();
            
    //        Columns.Add("PeakID");

    //        Columns.Add("isSignal");
    //        Columns.Add("isCentroided");
    //        Columns.Add("isMonoisotopic");

    //        Columns.Add("isIsotope");
    //        Columns.Add("isMostAbundant");
    //        Columns.Add("isCharged");
    //        Columns.Add("isCorrected");
    //        Columns.Add("isPrecursorMass");

    //        IndexedColumns.Add("PeakID");
    //        IndexedColumns.Add("isMonoisotopic");

    //        ValuesTypes.Add(DbType.Int32);//ID
    //        ValuesTypes.Add(DbType.Boolean);
    //        ValuesTypes.Add(DbType.Boolean);
    //        ValuesTypes.Add(DbType.Boolean);

    //        ValuesTypes.Add(DbType.Boolean);
    //        ValuesTypes.Add(DbType.Boolean);
    //        ValuesTypes.Add(DbType.Boolean);
    //        ValuesTypes.Add(DbType.Boolean);
    //        ValuesTypes.Add(DbType.Boolean);

    //        TableName = "T_Attribute_Centric";
    //    }
    //}

    //public class DatabaseAttributeCentricObjectList : DatabaseTransferObjectList
    //{
    //    public override List<DatabaseTransferObject> DatabaseTransferObjects { get; set; }

    //    public DatabaseAttributeCentricObjectList()
    //    {
    //        DatabaseTransferObjects = new List<DatabaseTransferObject>();
    //    }
    //}
}
