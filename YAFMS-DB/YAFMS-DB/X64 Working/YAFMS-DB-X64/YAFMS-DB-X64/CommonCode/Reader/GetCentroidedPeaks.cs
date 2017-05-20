using System;
using System.Collections.Generic;
using YAFMS_DB.GetPeaks;
using YAFMS_DB.PNNLOmics;
using YAFMS_DB.Reader;


namespace YAFMS_DB.Reader
{
    public static class GetCentroidedPeaks
    {
        /// <summary>
        /// Scan number corresponds to the tandem scan number
        /// </summary>
        /// <param name="scan"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static PeakArrays ReadPeaks(int scan, string fileName)
        {
            DatabaseLayerYAFMSDB layer = new DatabaseLayerYAFMSDB();

            DatabasePeakCentricLiteObject sampleObjectPeakCentric = new DatabasePeakCentricLiteObject();
            DatabaseScanCentricObject sampleObjectScanCentric = new DatabaseScanCentricObject();
            //string tablenamePeaksCentric = "T_Peak_Centric";
            string tablenamePeaksCentric = "T_Scan_Peaks";
            //string tablenamePeaksCentric = "T_Peak_Centric";
            string tablenameScanCentric = "T_Scan_Centric";

            DatabasePeakCentricLiteObjectList resultsPeaksCentric;
            bool returnMonoisotopicMassOnly = false;//false will pull all signal peaks
            layer.SK2_Final_SelectSimplePeaks(fileName, tablenamePeaksCentric, tablenameScanCentric, scan, sampleObjectPeakCentric, sampleObjectScanCentric, out resultsPeaksCentric, returnMonoisotopicMassOnly);

            PeakArrays results = new PeakArrays(resultsPeaksCentric.DatabaseTransferObjects.Count,EnumerationPeaksArrays.Peak);
            
            int i = 0;
            foreach (DatabasePeakCentricLiteObject data in resultsPeaksCentric.DatabaseTransferObjects)
            {
                ProcessedPeakYAFMS holdPeak = data.PeakData;
                results.IntensityArray[i] = holdPeak.Height;
                results.MzArray[i] = holdPeak.XValue;
                results.ScanArray[i] = holdPeak.ScanNumber;
                results.WidthArray[i] = holdPeak.Width;
                i++;
                
            }

            //Console.WriteLine("We have read in " + results.MzArray.Length + " MS1 peaks");
            return results;
        }
    }
}
