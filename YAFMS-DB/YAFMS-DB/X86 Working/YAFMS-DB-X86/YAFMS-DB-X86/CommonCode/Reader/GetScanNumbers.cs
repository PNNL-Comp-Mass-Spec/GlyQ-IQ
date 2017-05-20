using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YAFMS_DB.Reader
{
    public class GetScanNumbers
    {
        public static List<int> ReadFromDatabase(string fileName)
        {
            DatabaseLayerYAFMSDB layer = new DatabaseLayerYAFMSDB();

            string tablename = "T_Scan_Centric";
            List<int> results = layer.SK2_ReadAllScans(fileName, tablename);
            if (results != null)
            {
                results = results.Distinct().ToList();//removes duplicates
                results.Sort();
                return results;
            }
            
            return new List<int>();//if null return an empty list
        }
    }
}
