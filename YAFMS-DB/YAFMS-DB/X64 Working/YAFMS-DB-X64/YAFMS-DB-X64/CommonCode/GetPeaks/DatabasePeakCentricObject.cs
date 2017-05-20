using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using YAFMS_DB.GetPeaks;
using YAFMS_DB.PNNLOmics;

namespace YAFMS_DB.GetPeaks
{
    public class DatabasePeakCentricObject : DatabaseTransferObject
    {
        public int PeakID { get; set; }
        
        public int ScanID { get; set; }

        public int GroupID { get; set; }

        public int MonoisotopicClusterID { get; set; }

        public int FeatureClusterID { get; set; }

        public double Mz { get; set; }

        public double Height { get; set; }

        public double Width { get; set; }

        public double Background { get; set; }

        public double LocalSignalToNoise { get; set; }

        public int ChargeState { get; set; }

        public double MassMonoisotopic { get; set; }

        public double Score { get; set; }

        public double AmbiguityScore { get; set; }

        public ProcessedPeakYAFMS PeakCentricData { get; set; }

        public bool isSignal { get; set; }

        public bool isCentroided { get; set; }

        public bool isMonoisotopic { get; set; }

        public bool isIsotope { get; set; }

        public bool isMostAbundant { get; set; }

        public bool isCharged { get; set; }

        public bool isCorrected { get; set; }

        public DatabasePeakCentricObject()
        {
            PeakCentricData = new ProcessedPeakYAFMS();
            
            Columns.Add("PeakID");
            Columns.Add("ScanID");
            Columns.Add("GroupID");
            Columns.Add("MonoisotopicClusterID");
            Columns.Add("FeatureClusterID");

            Columns.Add("Mz");
            Columns.Add("Charge");

            Columns.Add("Height");
            Columns.Add("Width");
            Columns.Add("Background");
            Columns.Add("LocalSignalToNoise");
            
            Columns.Add("MassMonoisotopic");
            Columns.Add("Score");
            Columns.Add("AmbiguityScore");

            Columns.Add("isSignal");
            Columns.Add("isCentroided");
            Columns.Add("isMonoisotopic");

            Columns.Add("isIsotope");
            Columns.Add("isMostAbundant");
            Columns.Add("isCharged");
            Columns.Add("isCorrected");
            //Columns.Add("isPrecursorMass");


            IndexedColumns.Add("PeakID");
            //IndexedColumns.Add("ScanID,PeakID");
            IndexedColumns.Add("Mz");

            ValuesTypes.Add(DbType.Int32);
            ValuesTypes.Add(DbType.Int32);
            ValuesTypes.Add(DbType.Int32);
            ValuesTypes.Add(DbType.Int32);
            ValuesTypes.Add(DbType.Int32);

            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Int32);

            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);

            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);

            ValuesTypes.Add(DbType.Boolean);
            ValuesTypes.Add(DbType.Boolean);
            ValuesTypes.Add(DbType.Boolean);

            ValuesTypes.Add(DbType.Boolean);
            ValuesTypes.Add(DbType.Boolean);
            ValuesTypes.Add(DbType.Boolean);
            ValuesTypes.Add(DbType.Boolean);
            //ValuesTypes.Add(DbType.Boolean);

            TableName = "T_Peak_Centric";
        }
    }

    public class DatabasePeakCentricObjectList : DatabaseTransferObjectList
    {
        public override List<DatabaseTransferObject> DatabaseTransferObjects { get; set; }

        public DatabasePeakCentricObject PeakCentricProperty { get; set; }//this is not seeen?

        public DatabasePeakCentricObjectList()
        {
            DatabaseTransferObjects = new List<DatabaseTransferObject>();
            PeakCentricProperty = new DatabasePeakCentricObject(); 
        }
    }
}
