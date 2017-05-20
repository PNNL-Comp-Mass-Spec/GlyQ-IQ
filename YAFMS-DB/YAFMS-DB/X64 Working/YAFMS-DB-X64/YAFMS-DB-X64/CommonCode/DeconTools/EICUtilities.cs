using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YAFMS_DB;
using YAFMS_DB.PNNLOmics;

namespace YAFMS_DB.DeconTools
{
    public static class EICUtilities
    {
        public static PeakArrays getChromDataAndFillInZerosAndAssignChromID(PeakArrays filteredPeakList, int chromID = 0)
        {
            int filteredPeakListCount = filteredPeakList.IntensityArray.Length;

            int leftZeroPadding = 200;   //number of scans to the left of the minscan for which zeros will be added
            int rightZeroPadding = 200;   //number of scans to the left of the minscan for which zeros will be added

            int peakListMinScan = filteredPeakList.ScanArray[0];
            int peakListMaxScan = filteredPeakList.ScanArray[filteredPeakListCount - 1];

            //will pad min and max scans with zeros, and add zeros in between. This allows smoothing to execute properly

            peakListMinScan = peakListMinScan - leftZeroPadding;
            peakListMaxScan = peakListMaxScan + rightZeroPadding;

            if (peakListMinScan < 0) peakListMinScan = 0;

            //populate array with zero intensities.
            SortedDictionary<int, double> xyValues = new SortedDictionary<int, double>();
            for (int i = peakListMinScan; i <= peakListMaxScan; i++)
            {
                xyValues[i] = 0;
            }

            //iterate over the peaklist, assign chromID,  and extract intensity values
            for (int i = 0; i < filteredPeakListCount; i++)
            {
                //MSPeakResult peakResult = filteredPeakList[i];

                //NOTE:   we assign the chromID here. 
                //peakResult.ChromID = chromID;

                double intensity = filteredPeakList.IntensityArray[i];
                int scanNumber = filteredPeakList.ScanArray[i];

                //because we have tolerances to filter the peaks, more than one m/z peak may occur for a given scan. So will take the most abundant...

                if (!xyValues.ContainsKey(scanNumber))
                {
                    string errorString = "Unexpected error in chromatogram generator!! Scan= " + scanNumber +
                                         "; num filtered peaks = " + filteredPeakListCount;

                    Console.WriteLine(errorString);

                    throw new InvalidProgramException(errorString);
                }

                if (intensity > xyValues[scanNumber])
                {
                    xyValues[scanNumber] = intensity;
                }
            }

            PeakArrays outputXYData = new PeakArrays(xyValues.Count, EnumerationPeaksArrays.LC);

            //outputXYData.ScanArray = XYDataYAFMS.ConvertIntsToDouble(xyValues.Keys.ToArray());
            outputXYData.ScanArray = xyValues.Keys.ToArray();
            outputXYData.IntensityArray = xyValues.Values.ToArray();

            return outputXYData;
        }

        public static double[] ConvertIntsToDouble(int[] inputVals)
        {
            if (inputVals == null) return null;
            if (inputVals.Length == 0) return null;
            double[] outputVals = new double[inputVals.Length];

            for (int i = 0; i < inputVals.Length; i++)
            {
                outputVals[i] = (double)inputVals[i];

            }
            return outputVals;
        }


        //public static XYDataYAFMS AddCurrentXYDataToBaseXYData(XYDataYAFMS baseData, XYDataYAFMS newdata)
        //{
        //    XYDataYAFMS returnedData = new XYDataYAFMS();

        //    if (baseData == null)
        //    {
        //        returnedData = newdata;
        //    }
        //    else
        //    {
        //        //this might need to be cleaned up   :)

        //        //first add the base data
        //        SortedDictionary<int, double> baseValues = new SortedDictionary<int, double>();
        //        for (int i = 0; i < baseData.Xvalues.Length; i++)
        //        {
        //            baseValues.Add((int)baseData.Xvalues[i], baseData.Yvalues[i]);
        //        }

        //        //now combine base data with the new
        //        for (int i = 0; i < newdata.Xvalues.Length; i++)
        //        {
        //            int scanToBeInserted = (int)newdata.Xvalues[i];
        //            double intensityToBeInserted = newdata.Yvalues[i];

        //            if (baseValues.ContainsKey(scanToBeInserted))
        //            {
        //                baseValues[scanToBeInserted] += intensityToBeInserted;
        //            }
        //            else
        //            {
        //                baseValues.Add(scanToBeInserted, intensityToBeInserted);
        //            }

        //        }

        //        returnedData.Xvalues = XYDataYAFMS.ConvertIntsToDouble(baseValues.Keys.ToArray());
        //        returnedData.Yvalues = baseValues.Values.ToArray();

        //    }

        //    return returnedData;
        //}

    }
}
