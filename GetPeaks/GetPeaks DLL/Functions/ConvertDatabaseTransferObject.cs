using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.SQLite.DataTransferObjects;
using YAFMS_DB.GetPeaks;
using DatabaseTransferObject = YAFMS_DB.GetPeaks.DatabaseTransferObject;
using DatabaseTransferObjectList = YAFMS_DB.GetPeaks.DatabaseTransferObjectList;

namespace GetPeaks_DLL.Functions
{
    public static class ConvertDatabaseTransferObject
    {
        public static List<DatabasePeakCentricObject> ToDatabasePeakCentricObject(DatabaseTransferObjectList results)
        {
            List<DatabasePeakCentricObject> convertedResults = new List<DatabasePeakCentricObject>();
            foreach (DatabaseTransferObject dto in results.DatabaseTransferObjects)
            {
                convertedResults.Add((DatabasePeakCentricObject)dto);
            }

            return convertedResults;
        }

        public static List<DatabasePeakCentricLiteObject> ToDatabasePeakCentricLightObject(DatabasePeakCentricLiteObjectList results)
        {
            List<DatabasePeakCentricLiteObject> convertedResults = new List<DatabasePeakCentricLiteObject>();
            foreach (DatabaseTransferObject dto in results.DatabaseTransferObjects)
            {
                convertedResults.Add((DatabasePeakCentricLiteObject)dto);
            }

            return convertedResults;
        }
    }
}
