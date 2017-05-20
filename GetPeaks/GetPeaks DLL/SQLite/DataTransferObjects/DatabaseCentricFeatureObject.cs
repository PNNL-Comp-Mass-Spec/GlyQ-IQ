using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PNNLOmics.Data.Features;
using GetPeaks_DLL.Objects.TandemMSObjects;

namespace GetPeaks_DLL.SQLite.DataTransferObjects
{
    //public class DatabaseCentricFeatureObject : DatabaseTransferObject
    //{
    //    //peak centric

    //    //public int ID { get; set; }

    //    //public int GroupID { get; set; }

    //    //public int MonoisotopicClusterID { get; set; }

    //    //public double Mz { get; set; }

    //    //public double Height { get; set; }

    //    //public double Width { get; set; }

    //    //public double Background { get; set; }

    //    //public double LocalSignalToNoise { get; set; }

    //    //public int ChargeState { get; set; }

    //    //public double MassMonoisotopic { get; set; }

    //    //public double Score { get; set; }

    //    //public double AmbiguityScore { get; set; }
     
    //    public PeakCentric peakData { get; set; }

    //    //scan centric
        
    //    //public int ScanNumLc { get; set; }

    //    //public int ElutionTime { get; set; }

    //    //public int ScanNumDt { get; set; }

    //    //public int FrameNumberLc { get; set; }

    //    //public double DriftTime { get; set; }

    //    public ScanCentric scanData { get; set; }
       
    //    //Attributes centric

    //    //public bool isSignal { get; set; }

    //    //public bool isNoise { get; set; }

    //    //public bool isCentroided { get; set; }

    //    //public bool isMonoisotopic { get; set; }

    //    //public bool isIsotope { get; set; }

    //    //public bool isMostAbundant { get; set; }

    //    //public bool isCharged { get; set; }

    //    //public bool isCorrected { get; set; }

    //    //public bool isPrecursorMass { get; set; }

    //    public AttributeCentric Attributes { get; set; }

    //    //Fragment centric table

    //    //public int MsLevel { get; set; }

    //    //public int ParentScanNumber { get; set; }

    //    //public int TandemScanNumber { get; set; }

    //    public FragmentCentric fragmentData { get; set; }


    //    public DatabaseCentricFeatureObject()
    //    {
            
    //        Columns.Add("Scan");
    //        Columns.Add("PeakNumber");
    //        Columns.Add("XValue");
    //        Columns.Add("Height");
    //        Columns.Add("Charge");
    //        Columns.Add("LocalSignalToNoise");
    //        Columns.Add("Background");
    //        Columns.Add("Width");
    //        Columns.Add("LocalLowestMinimaHeight");
    //        Columns.Add("SignalToBackground");
    //        Columns.Add("SignalToNoiseGlobal");
    //        Columns.Add("SignalToNoiseLocalMinima");

    //        IndexedColumns.Add("Scan");
    //        IndexedColumns.Add("XValue");

    //        ValuesTypes.Add(DbType.Int32);
    //        ValuesTypes.Add(DbType.Int32);
    //        ValuesTypes.Add(DbType.Double);
    //        ValuesTypes.Add(DbType.Double);
    //        ValuesTypes.Add(DbType.Int32);
    //        ValuesTypes.Add(DbType.Double);
    //        ValuesTypes.Add(DbType.Double);
    //        ValuesTypes.Add(DbType.Double);
    //        ValuesTypes.Add(DbType.Double);
    //        ValuesTypes.Add(DbType.Double);
    //        ValuesTypes.Add(DbType.Double);
    //        ValuesTypes.Add(DbType.Double);

    //        TableName = "T_Scan_Peaks";
    //    }
    //}

    //public class DatabaseMSFeatureLightList : DatabaseTransferObjectList
    //{
    //    public override List<DatabaseTransferObject> DatabaseTransferObjects { get; set; }

    //    public DatabaseMSFeatureLightList()
    //    {
    //        DatabaseTransferObjects = new List<DatabaseTransferObject>();
    //    }
    //}
}
