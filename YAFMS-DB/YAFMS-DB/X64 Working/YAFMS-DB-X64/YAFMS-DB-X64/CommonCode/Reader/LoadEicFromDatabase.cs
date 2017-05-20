using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YAFMS_DB;
using YAFMS_DB.DeconTools;
using YAFMS_DB.GetPeaks;
using YAFMS_DB.PNNLOmics;
using YAFMS_DB.Reader;

namespace YAFMS_DB.Reader
{
    public class LoadEicFromDatabase
    {
        /// <summary>
        /// Scan number corresponds to the tandem scan number
        /// </summary>
        /// <param name="scan"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static PeakArrays ReadPeaks(double lower, double upper, string fileName)
        {
            DatabaseLayerYAFMSDB layer = new DatabaseLayerYAFMSDB();

            DatabasePeakCentricLiteObject sampleObjectPeakCentric = new DatabasePeakCentricLiteObject();
            DatabaseScanCentricObject sampleObjectScanCentric = new DatabaseScanCentricObject();
            //string tablenamePeaksCentric = "T_Peak_Centric";
            string tablenamePeaksCentric = "T_Scan_Peaks";
            string tablenameScanCentric = "T_Scan_Centric";

            DatabasePeakCentricLiteObjectList resultsPeaksCentric;
            layer.SK2_Final_SelectEIC(fileName, tablenamePeaksCentric, tablenameScanCentric, lower, upper, sampleObjectPeakCentric, sampleObjectScanCentric, out resultsPeaksCentric);

            PeakArrays results = new PeakArrays(resultsPeaksCentric.DatabaseTransferObjects.Count, EnumerationPeaksArrays.LC);

            ConvertToArrays(resultsPeaksCentric, results);

            PeakArrays zeroFilledEIC = new PeakArrays(0,EnumerationPeaksArrays.LC);

            if (results.ScanArray.Length > 0)
            {
                zeroFilledEIC = EICUtilities.getChromDataAndFillInZerosAndAssignChromID(results);
            }

            Console.WriteLine("We have read in " + zeroFilledEIC.IntensityArray.Length + " MS1 peaks");

            return zeroFilledEIC;
        }

        public static PeakArrays ReadProcessedPeaks(double lower, double upper, string fileName)
        {
            DatabaseLayerYAFMSDB layer = new DatabaseLayerYAFMSDB();

            DatabasePeakCentricLiteObject sampleObjectPeakCentric = new DatabasePeakCentricLiteObject();
            DatabaseScanCentricObject sampleObjectScanCentric = new DatabaseScanCentricObject();
            //string tablenamePeaksCentric = "T_Peak_Centric";
            string tablenamePeaksCentric = "T_Scan_Peaks";
            string tablenameScanCentric = "T_Scan_Centric";

            DatabasePeakCentricLiteObjectList resultsPeaksCentric;
            layer.SK2_Final_SelectEIC(fileName, tablenamePeaksCentric, tablenameScanCentric, lower, upper, sampleObjectPeakCentric, sampleObjectScanCentric, out resultsPeaksCentric);

            PeakArrays results = new PeakArrays(resultsPeaksCentric.DatabaseTransferObjects.Count, EnumerationPeaksArrays.LC);

            ConvertToArrays(resultsPeaksCentric, results);

            PeakArrays zeroFilledEIC = new PeakArrays(0, EnumerationPeaksArrays.LC);

            if (results.ScanArray.Length > 0)
            {
                zeroFilledEIC = EICUtilities.getChromDataAndFillInZerosAndAssignChromID(results);
            }

            Console.WriteLine("We have read in " + zeroFilledEIC.IntensityArray.Length + " MS1 peaks");

            return zeroFilledEIC;
        }

        private static void ConvertToArrays(DatabasePeakCentricLiteObjectList resultsPeaksCentric, PeakArrays results)
        {
            int i = 0;
            foreach (DatabasePeakCentricLiteObject data in resultsPeaksCentric.DatabaseTransferObjects)
            {
                ProcessedPeakYAFMS holdPeak = data.PeakData;
                results.IntensityArray[i] = holdPeak.Height;
                results.ScanArray[i] = holdPeak.ScanNumber;
                i++;
            }
        }
    }
}
