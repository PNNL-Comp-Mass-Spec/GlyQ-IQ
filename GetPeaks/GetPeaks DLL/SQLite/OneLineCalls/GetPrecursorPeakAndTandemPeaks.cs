using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.SQLite.DataTransferObjects;
using GetPeaks_DLL.Objects.TandemMSObjects;
using YAFMS_DB.GetPeaks;
using ConvertDatabaseTransferObject = GetPeaks_DLL.Functions.ConvertDatabaseTransferObject;

namespace GetPeaks_DLL.SQLite.OneLineCalls
{
    public class GetPrecursorPeakAndTandemPeaks
    {
        /// <summary>
        /// returns the predursor mass and the tandem peaks above the noise.  No Deisotoping
        /// </summary>
        /// <param name="scan"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static PrecursorAndPeaksObject ReadMZPeaks(int scan, string fileName)
        {
            //this can all be slashed out
            DatabaseLayer layer = new DatabaseLayer();
            string tablename = "T_Scans_Precursor_Peaks";
            string tableNamePeaks = "T_Scan_Peaks";
            //tableNamePeaks = tablenameMonoPeaks;
            DatabasePeakProcessedWithMZObject sampleObjectPrecursor = new DatabasePeakProcessedWithMZObject();
            DatabasePeakProcessedObject sampleObjectPeaks = new DatabasePeakProcessedObject();
            PrecursorAndPeaksObject results = layer.SK_SelectPrecursorAndPeaks(fileName, tablename, tableNamePeaks, scan, sampleObjectPrecursor, sampleObjectPeaks);

            //TODO it looks like this works


            //start good stuff
       //     DatabaseAttributeCentricObject sampleObjectAttributeCentric = new DatabaseAttributeCentricObject();
            DatabasePeakCentricObject sampleObjectPeakCentric = new DatabasePeakCentricObject();
            string tablenamePeaks = "T_Peak_Centric";
      //      string tableNameAttribute = "T_Attribute_Centric";
      //      DatabaseAttributeCentricObjectList resultsAttributesCentric;
            DatabasePeakCentricObjectList resultsPeaksCentric;
            bool returnMonoisotopicMassOnly = false;//false will pull all signal peaks
      //      layer.SK2_Final_SelectPeaksWithAttributes(fileName, tablenamePeaks, tableNameAttribute, scan, sampleObjectAttributeCentric, sampleObjectPeakCentric, out resultsPeaksCentric, out resultsAttributesCentric, returnMonoisotopicMassOnly);
            layer.SK2_Final_SelectPeaksWithAttributes(fileName, tablenamePeaks, scan, sampleObjectPeakCentric, out resultsPeaksCentric, returnMonoisotopicMassOnly);
            
            //results.TandemAttributeCentricList = resultsAttributesCentric;
            results.TandemPeakCentricList = ConvertDatabaseTransferObject.ToDatabasePeakCentricObject(resultsPeaksCentric);



            Console.WriteLine("We have read in " + results.TandemMonoPeakList.Count + " tandem peaks" + resultsPeaksCentric.DatabaseTransferObjects.Count);
            return results;
        }

        public static PrecursorAndPeaksObject ReadMonoPeaks(int scan, string fileName)
        {
            DatabaseLayer layer = new DatabaseLayer();

            DatabasePeakCentricObject sampleObjectPeakCentric = new DatabasePeakCentricObject();
            string tablenamePeaks = "T_Peak_Centric";
            DatabasePeakCentricObjectList resultsPeaksCentric;
            bool returnMonoisotopicMassOnly = true;//false will pull all signal peaks
            layer.SK2_Final_SelectPeaksWithAttributes(fileName, tablenamePeaks, scan, sampleObjectPeakCentric, out resultsPeaksCentric, returnMonoisotopicMassOnly);

            PrecursorAndPeaksObject results = new PrecursorAndPeaksObject();
            results.TandemMonoPeakCentricList = ConvertDatabaseTransferObject.ToDatabasePeakCentricObject(resultsPeaksCentric);

            Console.WriteLine("We have read in " + results.TandemMonoPeakList.Count + " tandem peaks" + resultsPeaksCentric.DatabaseTransferObjects.Count);
            return results;
        }


        public static PrecursorAndPeaksObject ReadPrecursorPeak(int peakId, string fileName)
        {
            DatabaseLayer layer = new DatabaseLayer();

            DatabasePeakCentricObject sampleObjectPeakCentric = new DatabasePeakCentricObject();
            string tablenamePeaks = "T_Peak_Centric";
            DatabasePeakCentricObjectList resultsPeaksCentric;
            bool returnMonoisotopicMassOnly = false;//false will pull all signal peaks
            layer.SK2_Final_SelectPeak(fileName, tablenamePeaks, peakId, sampleObjectPeakCentric, out resultsPeaksCentric, returnMonoisotopicMassOnly);

            PrecursorAndPeaksObject results = new PrecursorAndPeaksObject();
            List<DatabasePeakCentricObject> tempList = ConvertDatabaseTransferObject.ToDatabasePeakCentricObject(resultsPeaksCentric);
            results.PrecursorCentricPeak = tempList[0];

            Console.WriteLine("We have read in " + results.TandemMonoPeakList.Count + " tandem peaks" + resultsPeaksCentric.DatabaseTransferObjects.Count);
            return results;
        }
    }
}
