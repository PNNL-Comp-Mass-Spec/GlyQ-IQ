using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.SQLite.DataTransferObjects;
using YAFMS_DB.GetPeaks;
using ConvertDatabaseTransferObject = GetPeaks_DLL.Functions.ConvertDatabaseTransferObject;

namespace GetPeaks_DLL.SQLite.OneLineCalls
{
    public static class GetPrecursorPeakAndTandemMonoPeaks
    {
        /// <summary>
        /// Scan number corresponds to the tandem scan number
        /// </summary>
        /// <param name="scan"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static PrecursorAndPeaksObject Read(int scan, string fileName)
        {
            DatabaseLayer layer = new DatabaseLayer();
            string tablename = "T_Scans_Precursor_Peaks";
            string tablenamePeaks = "T_Scan_MonoPeaks";
            DatabasePeakProcessedWithMZObject sampleObjectPrecursor = new DatabasePeakProcessedWithMZObject();
            DatabasePeakProcessedObject sampleObjectPeaks = new DatabasePeakProcessedObject();
            PrecursorAndPeaksObject results = layer.SK_SelectPrecursorAndMonoPeaks(fileName, tablename, tablenamePeaks, scan, sampleObjectPrecursor, sampleObjectPeaks);



            //start new stuff
            //TODO bring in mono peak where isMonoisotopic = 1;
 //           DatabaseAttributeCentricObject sampleObjectAttributeCentric = new DatabaseAttributeCentricObject();
            DatabasePeakCentricObject sampleObjectPeakCentric = new DatabasePeakCentricObject();
            string tablenamePeaksCentric = "T_Peak_Centric";
//            string tableNameAttributeCentric = "T_Attribute_Centric";
//            DatabaseAttributeCentricObjectList resultsAttributesCentric;
            DatabasePeakCentricObjectList resultsPeaksCentric;
            bool returnMonoisotopicMassOnly = true;//false will pull all signal peaks
            //layer.SK2_Final_SelectPeaksWithAttributes(fileName, tablenamePeaksCentric, tableNameAttributeCentric, scan, sampleObjectAttributeCentric, sampleObjectPeakCentric, out resultsPeaksCentric, out resultsAttributesCentric, returnMonoisotopicMassOnly);
            layer.SK2_Final_SelectPeaksWithAttributes(fileName, tablenamePeaksCentric, scan, sampleObjectPeakCentric, out resultsPeaksCentric, returnMonoisotopicMassOnly);
            
            //results.TandemAttributeCentricList = resultsAttributesCentric;

            results.TandemPeakCentricList = ConvertDatabaseTransferObject.ToDatabasePeakCentricObject(resultsPeaksCentric);

            //precursor
            //DatabaseAttributeCentricObject sampleObjectAttributeCentric = new DatabaseAttributeCentricObject();
            //DatabasePeakCentricObject sampleObjectPeakCentric = new DatabasePeakCentricObject();
            DatabaseScanCentricObject sampleObjectScanCentric = new DatabaseScanCentricObject();
            //string tablenamePeaksCentric = "T_Peak_Centric";
            //string tableNameAttributeCentric = "T_Attribute_Centric";
            string tableNameScanCentric = "T_Scan_Centric";
            //DatabaseAttributeCentricObjectList resultsAttributesCentricPrecursor;
            List<DatabasePeakCentricObject> resultsPeaksCentricPrecursor;
            DatabaseScanCentricObjectList resultsScansCentricPrecursor;
            //layer.SK2_PrecursorPeakWithAttributes(fileName, tablenamePeaksCentric, tableNameAttributeCentric, tableNameScanCentric, scan, sampleObjectAttributeCentric, sampleObjectPeakCentric, sampleObjectScanCentric, out resultsPeaksCentricPrecursor, out resultsAttributesCentricPrecursor, out resultsScansCentricPrecursor);
            layer.SK2_PrecursorPeakWithAttributes(fileName, tablenamePeaksCentric, tableNameScanCentric, scan, sampleObjectPeakCentric, sampleObjectScanCentric, out resultsPeaksCentricPrecursor, out resultsScansCentricPrecursor);

            List<int> arg = new List<int>();
            foreach (DatabaseScanCentricObject scanCentric in resultsScansCentricPrecursor.DatabaseTransferObjects)
            {
                arg.Add(scanCentric.ScanCentricData.PeakID);
            }

            List<int> arg2 = new List<int>();
            foreach (DatabasePeakCentricObject peakCentric in resultsPeaksCentricPrecursor)
            {
                //arg2.Add(peakCentric.PeakCentricData.PeakID);
                arg2.Add(peakCentric.PeakID);
            }

            DatabaseScanCentricObject scanCentricHolder = (DatabaseScanCentricObject) resultsScansCentricPrecursor.DatabaseTransferObjects[0];
            int targetPeakID = scanCentricHolder.ScanCentricData.PeakID;

            //DatabasePeakCentricObject precursorPeakHit = resultsPeaksCentricPrecursor[resultsPeaksCentricPrecursor.FindIndex(c => c.PeakCentricData.PeakID == targetPeakID)];
            DatabasePeakCentricObject precursorPeakHit = resultsPeaksCentricPrecursor[resultsPeaksCentricPrecursor.FindIndex(c => c.PeakID == targetPeakID)];

            Console.WriteLine(precursorPeakHit.PeakID + " " + precursorPeakHit.ScanID);
            Console.WriteLine("We have read in " +results.TandemMonoPeakList.Count + " tandem peaks");
            return results;
        }

        
    }
}
