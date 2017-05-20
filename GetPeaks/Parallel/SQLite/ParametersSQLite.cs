using System.Collections.Generic;

namespace Parallel.SQLite
{
    public class ParametersSQLite : ParalellParameters
    {

        public List<string> ColumnHeaders { get; set; }

        public List<string> ColumnHeadersCounts { get; set; } 

        public string PageName { get; set; }

        public ParametersSQLite(string SQLiteFolderIn, string SQLiteNameIn, int computersToDivideOver, int coresPerComputer)
        {
            this.FileInforamation.OutputSQLFileName = SQLiteNameIn;
            this.FileInforamation.OutputPath = SQLiteFolderIn;
            this.ComputersToDivideOver = computersToDivideOver;
            this.CoresPerComputer = coresPerComputer;
            ColumnHeaders = new List<string>();
            ColumnHeadersCounts = new List<string>();
            PageName = "Page1";
            this.UniqueFileName = this.FileInforamation.OutputSQLFileName+" (0).db";
        }
    }
}
