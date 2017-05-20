using System.Collections.Generic;
using GetPeaks_DLL.Parallel;

namespace GetPeaks_DLL.SQLiteEngine
{
    public class ParametersSQLite : ParalellParameters
    {

        public List<string> ColumnHeaders { get; set; }

        public List<string> ColumnHeadersCounts { get; set; }

        public List<List<string>> Indexes { get; set; } 

        public string PageName { get; set; }

        public ParametersSQLite(string SQLiteFolderIn, string SQLiteNameIn, int computersToDivideOver, int coresPerComputer)
        {
            this.FileInforamation.OutputSQLFileName = SQLiteNameIn;
            this.FileInforamation.OutputPath = SQLiteFolderIn;
            this.ComputersToDivideOver = computersToDivideOver;
            this.CoresPerComputer = coresPerComputer;
            ColumnHeaders = new List<string>();
            ColumnHeadersCounts = new List<string>();
            Indexes = new List<List<string>>();
            PageName = "Page1";
            this.UniqueFileName = this.FileInforamation.OutputSQLFileName + " (0).db";
        }
    }
}
