using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YAFMS_DB.Reader;


namespace YAFMS_DB
{
    public class ReadDatabase
    {
        public string DatabaseFile { get; set; }

        public ReadDatabase(string databaseFile)
        {
            DatabaseFile = databaseFile;
        }

        public void LoadAllPeaksToMemory()
        {
        }


        /// <summary>
        /// Return all MS1 scan numbers from database
        /// </summary>
        /// <param name="scans">array of scan numbers</param>
        public void GetMs1ScanNumbers(out int[] scans)
        {
            scans = GetScanNumbers.ReadFromDatabase(DatabaseFile).ToArray();
        }

        /// <summary>
        /// Returns a peak spectrum from the centroided data
        /// </summary>
        /// <param name="scan"></param>
        /// <param name="mzArray"></param>
        /// <param name="intensityArray"></param>
        /// <param name="widthArray"></param>
        public void GetPeaksSpectrum(int scan, out double[] mzArray, out double[] intensityArray, out double[] widthArray)
        {
            PeakArrays results = GetCentroidedPeaks.ReadPeaks(scan, DatabaseFile);

            mzArray = results.MzArray;
            intensityArray = results.IntensityArray;
            widthArray = results.WidthArray;
        }

        public void GetProcessedPeaksSpectrum(int scan, out double[] mzArray, out double[] intensityArray, out double[] widthArray)
        {
            PeakArrays results = GetProcessedPeaks.ReadPeaks(scan, DatabaseFile);

            mzArray = results.MzArray;
            intensityArray = results.IntensityArray;
            widthArray = results.WidthArray;
        }


        public void GetEIC(double centerMz, double widthDa, out int[] scanArray, out double[] intensityArray)
        {
            double mzlower = centerMz - widthDa;
            double mzUpper = centerMz + widthDa;

            PeakArrays results = LoadEicFromDatabase.ReadPeaks(mzlower, mzUpper, DatabaseFile);

            scanArray = results.ScanArray;
            intensityArray = results.IntensityArray;
        }

        public void GetEICFromThresholdedData(double centerMz, double widthDa, out int[] scanArray, out double[] intensityArray)
        {
            double mzlower = centerMz - widthDa;
            double mzUpper = centerMz + widthDa;

            PeakArrays results = LoadEicFromDatabase.ReadProcessedPeaks(mzlower, mzUpper, DatabaseFile);

            scanArray = results.ScanArray;
            intensityArray = results.IntensityArray;
        }
    }
}
