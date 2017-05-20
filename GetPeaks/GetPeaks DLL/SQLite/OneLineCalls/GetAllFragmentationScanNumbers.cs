using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.SQLite.OneLineCalls
{
    public static class GetAllFragmentationScanNumbers
    {
        public static List<int> Read(string fileName)
        {
            string tablename = "T_Scans_Precursor_Peaks";

            DatabaseLayer layer = new DatabaseLayer();
            List<int> results = layer.SK_ReadAllFragmentationScans(fileName, tablename);

            return results;
        }
    }
}
