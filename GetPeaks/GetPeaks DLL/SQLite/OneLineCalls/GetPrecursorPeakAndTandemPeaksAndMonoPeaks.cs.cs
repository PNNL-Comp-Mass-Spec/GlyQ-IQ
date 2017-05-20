using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.SQLite.DataTransferObjects;

namespace GetPeaks_DLL.SQLite.OneLineCalls
{
    public static class GetPrecursorPeakAndTandemPeaksAndMonoPeaks
    {
        /// <summary>
        /// returns the precursor peak + all the tandem peaks above the noise + all of the monos
        /// </summary>
        /// <param name="scan"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static PrecursorAndPeaksObject Read(int scan, int precursorPeakID, string fileName)
        {
            //DatabaseLayer layer = new DatabaseLayer();
            //string tablename = "T_Scans_Precursor_Peaks";
            //string tablenameMonoPeaks = "T_Scan_MonoPeaks";
            //string tableNamePeaks = "T_Scan_Peaks";
            ////tableNamePeaks = tablenameMonoPeaks;
            //DatabasePeakProcessedWithMZObject sampleObjectPrecursor = new DatabasePeakProcessedWithMZObject();
            //DatabasePeakProcessedObject sampleObjectPeaks = new DatabasePeakProcessedObject();
            //PrecursorAndPeaksObject results = layer.SK_SelectPrecursorAndPeaksAndMonoPeaks(fileName, tablename, tablenameMonoPeaks, tableNamePeaks, scan, sampleObjectPrecursor, sampleObjectPeaks);
            
            //Console.WriteLine("We have read in " + results.TandemMonoPeakList.Count + " tandem peaks");
            //return results;

            //PrecursorAndPeaksObject resultsPeaks = GetPrecursorPeakAndTandemPeaks.Read(scan, fileName);

            //TODO this first one it ok
            //PrecursorAndPeaksObject resultsPeaks = GetPrecursorPeakAndTandemPeaks.Read(scan, fileName);

            //PrecursorAndPeaksObject resultsMono = GetPrecursorPeakAndTandemMonoPeaks.Read(scan, fileName);

            //resultsMono.TandemPeakList = resultsPeaks.TandemPeakList;


            PrecursorAndPeaksObject tandemResults = new PrecursorAndPeaksObject();

            PrecursorAndPeaksObject resultsPeaks = GetPrecursorPeakAndTandemPeaks.ReadMZPeaks(scan, fileName);
            PrecursorAndPeaksObject resultsMono = GetPrecursorPeakAndTandemPeaks.ReadMonoPeaks(scan, fileName);
            PrecursorAndPeaksObject resultsPrecursor = GetPrecursorPeakAndTandemPeaks.ReadPrecursorPeak(precursorPeakID, fileName);


            tandemResults.TandemPeakCentricList = resultsPeaks.TandemPeakCentricList;
            tandemResults.TandemMonoPeakCentricList = resultsMono.TandemMonoPeakCentricList;
            tandemResults.PrecursorCentricPeak = resultsPrecursor.PrecursorCentricPeak;

           // tandemResults.XValue = tandemResults.PrecursorCentricPeak.PeakCentricData.Mz;
            //tandemResults.Charge = tandemResults.PrecursorCentricPeak.PeakCentricData.ChargeState;
            tandemResults.XValue = tandemResults.PrecursorCentricPeak.Mz;
            tandemResults.Charge = tandemResults.PrecursorCentricPeak.ChargeState;
            tandemResults.PeakNumber = precursorPeakID;
            tandemResults.ScanNum = scan;
            
            return tandemResults;
        }
    }
}
