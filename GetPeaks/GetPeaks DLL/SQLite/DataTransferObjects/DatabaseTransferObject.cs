using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace GetPeaks_DLL.SQLite.DataTransferObjects
{
    public abstract class DatabaseTransferObject
    {
        /// <summary>
        /// list of column headers for SQLite page
        /// </summary>
        public List<string> Columns { get; set; }

        /// <summary>
        /// list of column headers for SQLite page
        /// </summary>
        public List<string> IndexedColumns { get; set; }

        /// <summary>
        /// data to Fill columns
        /// </summary>
        public List<object> Values { get; set; }

        /// <summary>
        /// what type each value is
        /// </summary>
        public List<DbType> ValuesTypes { get; set; }

        /// <summary>
        /// Page name
        /// </summary>
        public string TableName { get; set; }

        public DatabaseTransferObject()
        {
            Columns = new List<string>();
            IndexedColumns = new List<string>();
            ValuesTypes = new List<DbType>();
            Values = new List<object>();
            TableName = "NewTable";
        }

    }

    public abstract class DatabaseTransferObjectList : DatabaseTransferObject
    {
        public abstract List<DatabaseTransferObject> DatabaseTransferObjects { get; set; }
    }
}
