﻿using System;
using System.Collections.Generic;
using System.Linq;
using Run32.Backend;
using Run32.Backend.Core;
using Run32.Utilities;
using Run32.Backend.Data;

namespace IQ.Backend.ProcessingTasks.LCGenerators
{
    public class ChromatogramGenerator
    {
        #region Constructors
        #endregion

        #region Properties
        #endregion

        #region Public Methods





        /// <summary>
        /// Generates chromatogram based on a single m/z value and a given tolerance for a range of scans. 
        /// </summary>
        /// <param name="msPeakList"></param>
        /// <param name="minScan"></param>
        /// <param name="maxScan"></param>
        /// <param name="targetMZ"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public XYData GenerateChromatogram(List<Run32.Backend.DTO.MSPeakResult> msPeakList, int minScan, int maxScan, double targetMZ, double tolerance, Globals.ToleranceUnit toleranceUnit = Globals.ToleranceUnit.PPM)
        {
            List<double> targetMZList = new List<double>();
            targetMZList.Add(targetMZ);

            return GenerateChromatogram(msPeakList, minScan, maxScan, targetMZList, tolerance, toleranceUnit);
        }

        public XYData GenerateChromatogram(List<Run32.Backend.DTO.MSPeakResult> msPeakList, int minScan, int maxScan, double targetMZ, double tolerance, int chromIDToAssign, Globals.ToleranceUnit toleranceUnit = Globals.ToleranceUnit.PPM)
        {
            List<double> targetMZList = new List<double>();
            targetMZList.Add(targetMZ);

            return GenerateChromatogram(msPeakList, minScan, maxScan, targetMZList, tolerance, chromIDToAssign, toleranceUnit);
        }

        /// <summary>
        /// Will generate a chromatogram that is in fact a combination of chromatograms based on user-supplied target m/z values. 
        /// This is geared for producing a chromatogram for an isotopic profile, but only using narrow mass ranges
        /// that encompass individual peaks of an isotopic profile. 
        /// </summary>
        /// <param name="msPeakList"></param>
        /// <param name="minScan"></param>
        /// <param name="maxScan"></param>
        /// <param name="targetMZList"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public XYData GenerateChromatogram(List<Run32.Backend.DTO.MSPeakResult> msPeakList, int minScan, int maxScan, List<double> targetMZList, double tolerance, Globals.ToleranceUnit toleranceUnit = Globals.ToleranceUnit.PPM)
        {
            int defaultChromID = 0;

            return GenerateChromatogram(msPeakList, minScan, maxScan, targetMZList, tolerance, defaultChromID, toleranceUnit);
        }

        //TODO:  make a ChromatogramObject that will help handle my MSPeakResults, etc.

        public XYData GenerateChromatogram(List<Run32.Backend.DTO.MSPeakResult> msPeakList, int minScan, int maxScan, List<double> targetMZList, double tolerance, int chromIDToAssign, Globals.ToleranceUnit toleranceUnit = Globals.ToleranceUnit.PPM)
        {
            Check.Require(msPeakList != null && msPeakList.Count > 0, "Cannot generate chromatogram. Source msPeakList is empty or hasn't been defined.");

            int scanTolerance = 5;     // TODO:   keep an eye on this

            int indexOfLowerScan = getIndexOfClosestScanValue(msPeakList, minScan, 0, msPeakList.Count - 1, scanTolerance);
            int indexOfUpperScan = getIndexOfClosestScanValue(msPeakList, maxScan, 0, msPeakList.Count - 1, scanTolerance);

            XYData chromData = null;

            foreach (var targetMZ in targetMZList)
            {
                double lowerMZ;
                double upperMZ;

                if (toleranceUnit == Globals.ToleranceUnit.PPM)
                {
                    lowerMZ = targetMZ - tolerance * targetMZ / 1e6;
                    upperMZ = targetMZ + tolerance * targetMZ / 1e6;
                }
                else if (toleranceUnit == Globals.ToleranceUnit.MZ)
                {
                    lowerMZ = targetMZ - tolerance;
                    upperMZ = targetMZ + tolerance;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Trying to create chromatogram, but the " + toleranceUnit + " unit isn't supported");
                }

                List<Run32.Backend.DTO.MSPeakResult> tempPeakList = new List<Run32.Backend.DTO.MSPeakResult>();                

                for (int i = indexOfLowerScan; i <= indexOfUpperScan; i++)
                {
                    double xValue = msPeakList[i].MSPeak.XValue;

                    if (xValue >= lowerMZ && xValue <= upperMZ)
                    {
                        tempPeakList.Add(msPeakList[i]);
                    }
                }

                if (!tempPeakList.Any())
                {
                    //TODO: we want to return 0 intensity values. But need to make sure there are no downstream problems with this change. 
                }
                else
                {
                    XYData currentChromdata = getChromDataAndFillInZerosAndAssignChromID(tempPeakList, chromIDToAssign);
                    chromData = AddCurrentXYDataToBaseXYData(chromData, currentChromdata);
                }
            }

            return chromData;
        }
                
        public XYData GenerateChromatogramFromRawData(Run run, int minScan, int maxScan, double targetMZ, double toleranceInPPM)
        {
            XYData xydata = new XYData();

            List<double> xvals = new List<double>();
            List<double> yvals = new List<double>();

            for (int scan = minScan; scan <= maxScan; scan++)
            {
                bool scanIsGoodToGet = true;

                bool currentScanContainsMSMS = (run.ContainsMSMSData && run.GetMSLevel(scan) > 1);
                if (currentScanContainsMSMS)
                {
                    scanIsGoodToGet = false;
                }

                if (!scanIsGoodToGet)
                {
                    continue;
                }

                ScanSet scanset = new ScanSet(scan);
                run.GetMassSpectrum(scanset);

                double chromDataPointIntensity = getChromDataPoint(run.XYData, targetMZ, toleranceInPPM);

                xvals.Add(scan);
                yvals.Add(chromDataPointIntensity);
            }

            xydata.Xvalues = xvals.ToArray();
            xydata.Yvalues = yvals.ToArray();

            return xydata;
        }

        private double getChromDataPoint(XYData xydata, double targetMZ, double toleranceInPPM)
        {
            bool dataIsEmpty = (xydata == null || xydata.Xvalues.Length == 0);
            if (dataIsEmpty)
            {
                return 0;
            }

            double toleranceInMZ = toleranceInPPM * targetMZ / 1e6;

            double lowerMZ = targetMZ - toleranceInMZ;
            double upperMZ = targetMZ + toleranceInMZ;

            double startingPointMZ = lowerMZ - 2;

            int indexOfGoodStartingPoint = MathUtils.BinarySearchWithTolerance(xydata.Xvalues, startingPointMZ, 0, xydata.Xvalues.Length - 1, 1.9);

            if (indexOfGoodStartingPoint == -1)
            {
                indexOfGoodStartingPoint = 0;
            }

            double intensitySum = 0;

            for (int i = indexOfGoodStartingPoint; i < xydata.Xvalues.Length; i++)
            {
                if (xydata.Xvalues[i] >= lowerMZ)
                {

                    if (xydata.Xvalues[i] > upperMZ)
                    {
                        break;
                    }
                    else
                    {
                        intensitySum = +xydata.Yvalues[i];
                    }
                }
            }

            return intensitySum;
        }



        [Obsolete("Use the other GeneratePeakChromatogram!")]
        public List<Run32.Backend.DTO.MSPeakResult> GeneratePeakChromatogram(List<Run32.Backend.DTO.MSPeakResult> msPeakList, int minScan, int maxScan, List<double> targetMZList, double toleranceInPPM)
        {
            int scanTolerance = 5;     // TODO:   keep an eye on this

            int indexOfLowerScan = getIndexOfClosestScanValue(msPeakList, minScan, 0, msPeakList.Count - 1, scanTolerance);
            int indexOfUpperScan = getIndexOfClosestScanValue(msPeakList, maxScan, 0, msPeakList.Count - 1, scanTolerance);

            int currentIndex = indexOfLowerScan;
            List<Run32.Backend.DTO.MSPeakResult> filteredPeakList = new List<Run32.Backend.DTO.MSPeakResult>();
            while (currentIndex <= indexOfUpperScan)
            {
                filteredPeakList.Add(msPeakList[currentIndex]);
                currentIndex++;
            }

            List<Run32.Backend.DTO.MSPeakResult> compiledChromPeakList = new List<Run32.Backend.DTO.MSPeakResult>();

            int counter = 0;
            foreach (var targetMZ in targetMZList)
            {
                counter++;
                double lowerMZ = targetMZ - toleranceInPPM * targetMZ / 1e6;
                double upperMZ = targetMZ + toleranceInPPM * targetMZ / 1e6;

                List<Run32.Backend.DTO.MSPeakResult> tempPeakList = filteredPeakList.Where(p => p.MSPeak.XValue >= lowerMZ && p.MSPeak.XValue <= upperMZ).ToList();

                compiledChromPeakList.AddRange(tempPeakList);
            }

            if (counter > 1) // if the list contains multiple peak chromatograms, then need to sort.  Otherwise, don't need to sort.
            {
                compiledChromPeakList.Sort(delegate(Run32.Backend.DTO.MSPeakResult peak1, Run32.Backend.DTO.MSPeakResult peak2)
                {
                    return peak2.Scan_num.CompareTo(peak1.Scan_num);
                });
            }

            return compiledChromPeakList;
        }

        [Obsolete("Use the other GeneratePeakChromatogram!")]
        public List<Run32.Backend.DTO.MSPeakResult> GeneratePeakChromatogram(List<Run32.Backend.DTO.MSPeakResult> msPeakList, int minScan, int maxScan, double targetMZ, double toleranceInPPM)
        {
            List<double> targetMZList = new List<double>();
            targetMZList.Add(targetMZ);

            return GeneratePeakChromatogram(msPeakList, minScan, maxScan, targetMZList, toleranceInPPM);
        }

        private XYData getChromDataAndFillInZerosAndAssignChromID(List<Run32.Backend.DTO.MSPeakResult> filteredPeakList, int chromID)
        {
            int filteredPeakListCount = filteredPeakList.Count;

            int leftZeroPadding = 200;   //number of scans to the left of the minscan for which zeros will be added
            int rightZeroPadding = 200;   //number of scans to the left of the minscan for which zeros will be added

            int peakListMinScan = filteredPeakList[0].Scan_num;
            int peakListMaxScan = filteredPeakList[filteredPeakList.Count - 1].Scan_num;

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
                Run32.Backend.DTO.MSPeakResult peakResult = filteredPeakList[i];

                //NOTE:   we assign the chromID here. 
                peakResult.ChromID = chromID;

                double intensity = peakResult.MSPeak.Height;
                int scanNumber = peakResult.Scan_num;

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

            XYData outputXYData = new XYData();

            outputXYData.Xvalues = XYData.ConvertIntsToDouble(xyValues.Keys.ToArray());
            outputXYData.Yvalues = xyValues.Values.ToArray();

            return outputXYData;
        }

        private XYData AddCurrentXYDataToBaseXYData(XYData baseData, XYData newdata)
        {
            XYData returnedData = new XYData();

            if (baseData == null)
            {
                returnedData = newdata;
            }
            else
            {
                //this might need to be cleaned up   :)

                //first add the base data
                SortedDictionary<int, double> baseValues = new SortedDictionary<int, double>();
                for (int i = 0; i < baseData.Xvalues.Length; i++)
                {
                    baseValues.Add((int)baseData.Xvalues[i], baseData.Yvalues[i]);
                }

                //now combine base data with the new
                for (int i = 0; i < newdata.Xvalues.Length; i++)
                {
                    int scanToBeInserted = (int)newdata.Xvalues[i];
                    double intensityToBeInserted = newdata.Yvalues[i];

                    if (baseValues.ContainsKey(scanToBeInserted))
                    {
                        baseValues[scanToBeInserted] += intensityToBeInserted;
                    }
                    else
                    {
                        baseValues.Add(scanToBeInserted, intensityToBeInserted);
                    }

                }

                returnedData.Xvalues = XYData.ConvertIntsToDouble(baseValues.Keys.ToArray());
                returnedData.Yvalues = baseValues.Values.ToArray();

            }

            return returnedData;
        }

        #endregion

        #region Private Methods
        private int getIndexOfClosestScanValue(List<Run32.Backend.DTO.MSPeakResult> peakList, int targetScan, int leftIndex, int rightIndex, int scanTolerance)
        {
            if (leftIndex < rightIndex)
            {
                int middle = (leftIndex + rightIndex) / 2;

                if (Math.Abs(targetScan - peakList[middle].Scan_num) <= scanTolerance)
                {
                    return middle;
                }
                else if (targetScan < peakList[middle].Scan_num)
                {
                    return getIndexOfClosestScanValue(peakList, targetScan, leftIndex, middle - 1, scanTolerance);
                }
                else
                {
                    return getIndexOfClosestScanValue(peakList, targetScan, middle + 1, rightIndex, scanTolerance);
                }
            }
            else if (leftIndex == rightIndex)
            {
                {
                    return leftIndex;
                }
            }
            return leftIndex;    // if fails to find...  will return the inputted left-most scan
        }

        #endregion
    }
}
