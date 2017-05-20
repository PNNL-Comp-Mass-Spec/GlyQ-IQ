using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using YAFMS_DB.GetPeaks;

namespace GetPeaks_DLL.SQLite.DataTransferObjects
{
    public class DatabaseIsotopeObject : DatabaseTransferObject
    {
        public double MonoIsotopicMass { get; set; }
        public double ExperimentMass { get; set; }
        public string IsotopeMassesCSV { get; set; }
        public string IsotopeIntensitiesCSV { get; set; }

        //ISOS
        public int scan_num { get; set; }
        public int charge { get; set; }
        public double abundance { get; set; }
        public double mz { get; set; }
        public double fit { get; set; }
        public double average_mw { get; set; }
        public double monoisotopic_mw { get; set; }
        public double mostabundant_mw { get; set; }
        public double fwhm { get; set; }
        public double signal_noise { get; set; }
        public double mono_abundance { get; set; }
        public double mono_plus2_abundance { get; set; }
        public double flag { get; set; }
        public double interference_score { get; set; }

        public DatabaseIsotopeObject()
        {
            Columns.Add("MonoisotopicMass");
            Columns.Add("ExperimentMass");
            Columns.Add("IsotopeMasses");
            Columns.Add("IsotopeIntensities");

            Columns.Add("scan_num");
            Columns.Add("charge");
            Columns.Add("abundance");
            Columns.Add("mz");
            Columns.Add("fit");
            Columns.Add("average_mw");
            Columns.Add("monoisotopic_mw");
            Columns.Add("mostabundant_mw");
            Columns.Add("fwhm");
            Columns.Add("signal_noise");
            Columns.Add("mono_abundance");
            Columns.Add("mono_plus2_abundance");
            Columns.Add("flag");
            Columns.Add("interference_score");

            IndexedColumns.Add("scan_num");
            IndexedColumns.Add("MonoisotopicMass");
            

            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.String);
            ValuesTypes.Add(DbType.String);

            ValuesTypes.Add(DbType.Int32);
            ValuesTypes.Add(DbType.Int32);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);
            ValuesTypes.Add(DbType.Double);

            TableName= "IsostopeTable";
        }
    }

    public class DatabaseIsotopeObjectList : DatabaseTransferObjectList
    {
        public override List<DatabaseTransferObject> DatabaseTransferObjects { get; set; }

        public DatabaseIsotopeObjectList()
        {
            DatabaseTransferObjects = new List<DatabaseTransferObject>();
            DatabaseIsotopeObject initial = new DatabaseIsotopeObject();
            this.Columns = initial.Columns;
            this.ValuesTypes = initial.ValuesTypes;
            this.TableName = initial.TableName;
        }
    }
}
