using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YAFMS_DB.GetPeaks;
using YAFMS_DB.PNNLOmics;

namespace YAFMS_DB.Reader
{
    public class GetProcessedPeaks
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

            DatabasePeakCentricObject sampleObjectPeakCentric = new DatabasePeakCentricObject();
            DatabaseScanCentricObject sampleObjectScanCentric = new DatabaseScanCentricObject();
            string tablenamePeaksCentric = "T_Peak_Centric";
            string tablenameScanCentric = "T_Scan_Centric";

            DatabasePeakCentricObjectList resultsPeaksCentric;
            bool returnMonoisotopicMassOnly = false;//false will pull all signal peaks
            layer.SK2_Final_SelectProcessedPeaks(fileName, tablenamePeaksCentric, tablenameScanCentric, scan, sampleObjectPeakCentric, sampleObjectScanCentric, out resultsPeaksCentric, returnMonoisotopicMassOnly);

            PeakArrays results = new PeakArrays(resultsPeaksCentric.DatabaseTransferObjects.Count, EnumerationPeaksArrays.Peak);

            int i = 0;
            foreach (DatabasePeakCentricObject data in resultsPeaksCentric.DatabaseTransferObjects)
            {
                ProcessedPeakYAFMS holdPeak = data.PeakCentricData;
                results.IntensityArray[i] = holdPeak.Height;
                results.MzArray[i] = holdPeak.XValue;
                results.ScanArray[i] = holdPeak.ScanNumber;
                results.WidthArray[i] = holdPeak.Width;
                i++;

            }

            Console.WriteLine("We have read in " + results.MzArray.Length + " MS1 peaks");
            return results;
        }

        private static void ConvertToProcessedPeak(List<ProcessedPeakYAFMS> peaks, DatabasePeakCentricLiteObjectList resultsPeaksCentric)
        {
            foreach (DatabasePeakCentricLiteObject data in resultsPeaksCentric.DatabaseTransferObjects)
            {
                ProcessedPeakYAFMS peak = new ProcessedPeakYAFMS();
                peak = data.PeakData;
                peaks.Add(peak);
            }
        }
    }
}
