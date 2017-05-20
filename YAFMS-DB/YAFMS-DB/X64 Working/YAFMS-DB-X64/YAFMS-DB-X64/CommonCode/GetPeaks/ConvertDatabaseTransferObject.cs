using System.Collections.Generic;
using YAFMS_DB.GetPeaks;

namespace YAFMS_DB.GetPeaks
{
    public static class ConvertDatabaseTransferObject
    {
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
