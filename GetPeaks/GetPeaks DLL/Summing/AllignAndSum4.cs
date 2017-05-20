using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.DataFIFO;
using PNNLOmics.Data;
using GetPeaks_DLL.Functions;
using System.Collections;

namespace GetPeaks_DLL.Summing
{
    public class AllignAndSum4
    {
        #region properties
        /// <summary>
        /// this gets us into INt64 space
        /// </summary>
        double bitshiftForRounding {get;set;} //allows for 5 zeroes precision and up to X digits before the point
        
        /// <summary>
        /// Sets lowerbound for X-axis
        /// </summary>
        double startMZ {get;set;}
        
        /// <summary>
        /// Sets upperbound for X-axis
        /// </summary>
        double endMZ {get;set;}
        
        /// <summary>
        /// this is only used to setup the linear spine used to calculated the nonlinear spine axis, in Daltons
        /// use the smallest point spacing observed in the data set
        /// </summary>
        double pointSpacing {get;set;}//Da.  

        /// <summary>
        /// this is the ratio increase between data points.  For example, for a value of 0.03, if the point spacing in the data is > 1.03
        /// spacing2/spacing1 A large ratio means that data is missing in the spectra so we should skip this difference in fitting
        /// low numbers use less points in the fit and higher numbers may include a false spacing
        /// </summary>
        double ratioError {get;set;}//0.03 should do it.

        #endregion

        /// <summary>
        /// Constructor with default values
        /// </summary>
        public AllignAndSum4()
        {
            this.bitshiftForRounding = 100000;
            this.startMZ = 400;
            this.endMZ = 2000;
            this.pointSpacing = 0.00175;//Da.  This is the smallest spacing
            this.ratioError = 0.03;//0.03 should do it.  
        }

        public void AllignXAxisAndSum(Run run, int peakFirstScan, int peakLastScan)
        {

            #region Constants
            //constants for 60,000 resolution
            double FFTPeakSpacingCoefficient = 1.84175648 * Math.Pow(10, -7);//these are recalculated based on the data
            double FFTPeakSpacingPower = 1.50359798;//these are recalculated based on the data

            int points = 999999999;//400000 will get to m/z 1991;402000 will get over 20000


            #endregion

            #region Setup XY Data for each scan

            List<int> scanIndexList = new List<int>();

            for (int scanIndex = peakFirstScan; scanIndex <= peakLastScan; scanIndex++)
            {
                if (run.GetMSLevel(scanIndex) == 1)
                {
                    scanIndexList.Add(scanIndex);
                }
            }

            List<XYDataAndPeakHolderObject> DataToSum;
            CacheXYData.CacheData(run, scanIndexList, out DataToSum);
            
            #endregion
           
            #region calculate power coefficients off of first scan
            //TODO implement for other scans and average?????

            List<double> differenceArray = new List<double>();
            List<double> ratioArray = new List<double>();

            differenceArray.Add(DataToSum[0].SpectraDataOMICS[1].X - DataToSum[0].SpectraDataOMICS[0].X);
            ratioArray.Add(1);

            double maxRatioError = 1 + ratioError;
            double minRatioError = 1 - ratioError;
            double ratio;

            double X = 0;
            double Y = 0;
            double XY = 0;
            double X2 = 0;

            double sumX = 0;
            double sumY = 0;
            double sumXY = 0;
            double sumX2 = 0;

            //we need to collect a nice set of point differences that do not include large m/z gaps
            List<int> keeperXValues = new List<int>();
            for (int i = 1; i < DataToSum[0].SpectraDataOMICS.Count; i++)//use the ratios of the differences to find a blank in the data
            {
                XYDataAndPeakHolderObject currentData = DataToSum[0];
                differenceArray.Add(currentData.SpectraDataOMICS[i].X - currentData.SpectraDataOMICS[i - 1].X);
                ratio = differenceArray[i] / differenceArray[i - 1];
                ratioArray.Add(ratio);
                if (ratio > minRatioError && ratio < maxRatioError)
                {
                    keeperXValues.Add(i);

                    //we need a sum of :X*Y, X, Y, X^2, and N

                    X = Math.Log10(currentData.SpectraDataOMICS[i].X);
                    Y = Math.Log10(differenceArray[i]);
                    XY = X * Y;
                    X2 = X * X;

                    sumX += X;
                    sumY += Y;
                    sumXY += XY;
                    sumX2 += X2;
                }
            }
            double N = keeperXValues.Count;
            double slope = (N * sumXY - sumX * sumY) / (N * sumX2 - sumX * sumX);
            double interceptt = (sumX2 * sumY - sumX * sumXY) / (N * sumX2 - sumX * sumX);

            //corrected values
            FFTPeakSpacingCoefficient = Math.Pow(10, interceptt);
            FFTPeakSpacingPower = slope;

            #endregion

            #region Calculate Theoretical X Axis Map and bins

            SortedList<double, int> MZspine = new SortedList<double, int>();//m/x, index
            SortedList<double, int> MZspineHalfPointOffset = new SortedList<double, int>();//sets up bin boundaries between the points
            //SortedList<Int64, int> MZspineInt64 = new SortedList<Int64, int>();
            //SortedList<Int64, int> MZspineHalfPointOffsetInt64 = new SortedList<Int64, int>();//sets up bin boundaries between the points

            List<XYData> newSpectra = new List<XYData>();//final aglomeration
            //first point setup

            MZspine.Add(startMZ, 0);
            MZspineHalfPointOffset.Add(startMZ - (FFTPeakSpacingCoefficient * Math.Pow(startMZ, FFTPeakSpacingPower)) / 2, 0);
            //MZspineInt64.Add(Convert.ToInt64(Math.Floor(startMZ * bitshiftForRounding)), 0);
            //MZspineHalfPointOffsetInt64.Add(Convert.ToInt64(Math.Floor((startMZ - pointSpacing / 2) * bitshiftForRounding)), 0);

            XYData firstPoint = new XYData(startMZ, 0);
            newSpectra.Add(firstPoint);
            double tempMZ = 0;
            double tempMZNonLinear = 0;

            //this creates the correct theoretical X-axis and new number of datapoints
            double pointSpacingNonLinear = 0;
            for (int i = 1; i < points; i++)
            {
                tempMZ = i * pointSpacing + startMZ;
                pointSpacingNonLinear = FFTPeakSpacingCoefficient * Math.Pow(tempMZ, FFTPeakSpacingPower);
                
                tempMZNonLinear = MZspine.Keys[i - 1] + pointSpacingNonLinear;//point before + new space
                MZspine.Add(tempMZNonLinear, i);//add the spaceing to the previous point
                MZspineHalfPointOffset.Add(tempMZNonLinear + pointSpacingNonLinear / 2, i);//add the spaceing to the previous point//

               // MZspineInt64.Add(Convert.ToInt64(Math.Floor(tempMZNonLinear * bitshiftForRounding)), i);
                //MZspineHalfPointOffsetInt64.Add(Convert.ToInt64(Math.Floor((tempMZNonLinear - pointSpacingNonLinear / 2) * bitshiftForRounding)), i);

                XYData nextPoint = new XYData(tempMZNonLinear, 0);//creates a blank list of y values
                newSpectra.Add(nextPoint);

                if (tempMZNonLinear > endMZ)
                {
                    points = MZspine.Count;
                    break;
                }
                //string string1 = MZspine.Keys[i].ToString();
                //string string2 = MZspineHalfPointOffset.Keys[i].ToString();
                //Console.WriteLine(string1 + "," + string2);//this is big
            }

            #endregion


            #region find actual x axis offset based on first point of raw data
            //this is a coarse allignment but it helps

            List<double> xAxisOffsets = new List<double>();
            for (int i = 0; i < DataToSum.Count; i++)
            {
                int counter = 0;
                double tempFirstPointX = DataToSum[i].SpectraDataOMICS[0].X;
                double tempBinSpine = 0;

                tempBinSpine = MZspine.Keys[counter];

                while (tempFirstPointX >= tempBinSpine)
                {
                    counter++;
                    tempBinSpine = MZspine.Keys[counter];
                }
                //double tempBinSpine1 = MZspine.Keys[counter - 1];
                double tempBinSpine2 = MZspine.Keys[counter];
                //double tempBinSpine3 = MZspine.Keys[counter + 1];

                xAxisOffsets.Add(tempBinSpine2 - tempFirstPointX);//correct offset
                double correctedValue = tempFirstPointX - xAxisOffsets[i];
                double error = tempFirstPointX + correctedValue; 
                error++;
                error--;
            }
            #endregion


            #region Now that we know the offset, correct data based on difference so we can center the bins.

            SortedDictionary<int, double> corrctedSpine = new SortedDictionary<int, double>();
            SortedDictionary<int, double> corrctedSpineBinUpper = new SortedDictionary<int, double>();

            //this may be caused by a automatic gain control calibration correction
            double errorInside = 0;
            //for (int i = 0; i < DataToSum.Count; i++)
            //{
                errorInside = xAxisOffsets[0]; ;
                //foreach (XYData dataPoint in DataToSum[i].SpectraDataOMICS)
               // {
                    //dataPoint.X += errorInside;//by adding the error, we bump the data to the center of the bin
                    //it is better to bump the spine so less mass errors are possible
                //}
                for(int j=0;j<MZspine.Count;j++)
                {
                    //MZspine.Keys[j]-=errorInside;//subtract from the spine so the data from [0] fits nicely into the middle
                    //MZspineHalfPointOffset.Keys[j] -= errorInside;
                    corrctedSpine.Add(j , MZspine.Keys[j] - errorInside);
                    corrctedSpineBinUpper.Add(j , MZspineHalfPointOffset.Keys[j] - errorInside);
                }
            //}

            #endregion

            #region create one list and zero out others to save memory

            List<XYData> longPile = new List<XYData>();
            foreach (XYDataAndPeakHolderObject scan in DataToSum)
            {
                longPile.AddRange(scan.SpectraDataOMICS);
            }

            longPile = longPile.OrderBy(p => p.X).ToList();
            //longPile = DataToSum[1].SpectraDataOMICS.OrderBy(p => p.X).ToList();

            #endregion


            List<double> difflist = new List<double>();
            for (int i = 0; i < longPile.Count; i++)
            {
                difflist.Add(0);
            }

            if (4 == 4)
            {
            #region bin data as quickly as possible

            
                int dataCounter = 0;
                //int spineCounter = 0;
                int dataLength = longPile.Count;
                int spineLength = newSpectra.Count;

                //jump to start of data
                //while (longPile[dataCounter].X > newSpectra[spineCounter].X && spineCounter < newSpectra.Count)
                //{
                //    spineCounter++;
                //}
                //spineCounter--;
                
                //TODO there is some overflow here
                double difference = 0;
                double binDifference = 0;
                double fractionAlong = 0;
                double massDifference = 0;
                for(int i=0;i<corrctedSpineBinUpper.Count;i++)
	            {
		            double DataX = longPile[dataCounter].X;
                    double BinCeling = corrctedSpineBinUpper[i];

                    while( DataX<=BinCeling && dataCounter<longPile.Count-1)
                    {
                        difference = corrctedSpineBinUpper[i] - DataX;
                        binDifference = corrctedSpineBinUpper[i + 1] - corrctedSpineBinUpper[i];
                        fractionAlong = Math.Round(difference / binDifference * 100,2);

                        massDifference = DataX - corrctedSpine[i];
                        //TODO Attribute around points 100, the data and points are offset by one
                        if (i > 1400)
                        {
                            fractionAlong++;
                            fractionAlong--;
                        }

                        if (i == 3000)
                        {
                            fractionAlong++;
                            fractionAlong--;
                        }

                        newSpectra[i].Y += longPile[dataCounter].Y;
                        difflist[dataCounter] = massDifference;
                        difflist[dataCounter] = fractionAlong;
                        dataCounter++;
                        DataX = longPile[dataCounter].X;
                    }
	            }

                //while (spineCounter < newSpectra.Count && dataCounter<longPile.Count)//for each point in the spine...//-1 for last point.  It is best to make sure the spine is longer than the data
                //{
                //    double DataX = longPile[dataCounter].X;
                //    double spineValue = newSpectra[spineCounter].X;//are we less thatn the upper bound for the bin
                //    //double spineValueBinUpper = MZspineHalfPointOffset.Keys[spineCounter+1];//+1 because 0 is below and 1 is above//are we less thatn the upper bound for the bin
                //    double spineValueBinUpper = corrctedSpineBinUpper.[spineCounter + 1];//+1 because 0 is below and 1 is above//are we less thatn the upper bound for the bin


                //    double difference = spineValueBinUpper - DataX;
                //    double binDifference = MZspineHalfPointOffset.Keys[spineCounter + 1] - MZspineHalfPointOffset.Keys[spineCounter];
                //    double fractionAlong = Math.Round(difference / binDifference * 100,2);
                //    string fracString = fractionAlong.ToString();
                //    //difference++;
                    
                //    if (DataX <= spineValueBinUpper)
                //    {
                //        //Console.WriteLine("we are " + fracString + "% of the way between bins");
                //        newSpectra[spineCounter].Y += longPile[dataCounter].Y;
                //        //newSpectra[spineCounter].X += Math.Abs(longPile[dataCounter].X - spineValue);//this is not needed.  this is only here so we can observe the differences propegating based on the linear shift
                        
                //        //if (difflist[dataCounter] > 0.05)
                //        //{
                //        //    difference++;
                //        //    binDifference++;
                //        //}
                //        difflist[dataCounter] = fractionAlong;
                //        dataCounter++;

                //    }
                //    else
                //    {
                //        spineCounter++;
                //    }
                //}

                double averageError = 0;
                for (int i = 0; i < differenceArray.Count; i++)
                {
                    averageError += differenceArray[i];
                }
                averageError = averageError/differenceArray.Count;

                Console.WriteLine("The Average along is " + averageError);
            #endregion

            //we need to check this
            #region check x errors.
            bool check = false;
            if (check)
            {

                List<double> XdifferenceList = new List<double>();
  //              Console.WriteLine("There are " + Misscounter.ToString() + " missing values");
                foreach (XYDataAndPeakHolderObject scanToSum in DataToSum)
                {
                    for (int i = 0; i < newSpectra.Count; i++)
                    {
                        XdifferenceList.Add(newSpectra[i].X - MZspine.Keys[i]);

                    }
                }

                //for (int y = 12158; y < 12170; y++)
                //{
                //    Console.WriteLine(XdifferenceList[y].ToString() + " " + y.ToString());
                //}
                //Console.WriteLine("h");
                //for (int y = 19980; y < 19990; y++)
                //{
                //    Console.WriteLine(XdifferenceList[y].ToString() + " " + y.ToString());
                //}
                //Console.WriteLine("h");
                //for (int y = 30770; y < 30780; y++)
                //{
                //    Console.WriteLine(XdifferenceList[y].ToString() + " " + y.ToString());
                //}

                //Console.WriteLine("h");
                //for (int y = 39860; y < 39870; y++)
                //{
                //    Console.WriteLine(XdifferenceList[y].ToString() + " " + y.ToString());
                //}
                //Console.WriteLine("h");
                XdifferenceList.Sort();
                Console.WriteLine("TheLargestDifference is " + XdifferenceList[0].ToString() + " or " + XdifferenceList[XdifferenceList.Count - 1].ToString());

                double highQualityCount = 0;
                double averageDiffereence = 0;
                for (int i = 0; i < XdifferenceList.Count; i++)
                {
                    double absDiference = Math.Abs(XdifferenceList[i]);
                    if (absDiference < 0.001)
                    {
                        highQualityCount++;
                    }
                    averageDiffereence += XdifferenceList[i];
                }
                averageDiffereence = averageDiffereence / XdifferenceList.Count;
                Console.WriteLine("TheAverageDifference is " + averageDiffereence.ToString());
                double size = Convert.ToDouble(XdifferenceList.Count);
                double quality = highQualityCount / size * 100;
                Console.WriteLine("ThePercentOfHighQuality is " + quality.ToString() + "%");
            }
            #endregion

            #region convert back to XYdata Decon and store in run as for return

            List<XYData> patchTogether = new List<XYData>();
            for (int i = 0; i < points; i++)//for each point in the spine...
            {
                XYData tempoint = new XYData(newSpectra[i].X, newSpectra[i].Y);
                patchTogether.Add(tempoint);
            }
            ConvertXYData.OmicsXYDataToRunXYDataRun(ref run, patchTogether);
            #endregion

            #region print alligned data
            bool print = true;
            if (print)//28
            {
                //IRapidCompare compareHere = new CompareContrast2();
                IXYDataWriter newXYWriter = new DataXYDataWriter();
                //string path = @"V:\XYOutAllign" + peakFirstScan.ToString() + ".txt";

                string scanStr = peakFirstScan.ToString();
                string massInt = peakLastScan.ToString();
                string path = @"V:\XYOut_" + scanStr + "_" + massInt + "_S" + "4B" + ".txt";

                int t = newXYWriter.WriteOmicsXYData(newSpectra, path);
            }
            #endregion

            }//if
        } 
    }  
}

 //public int GetClosestXVal(double targetXVal)
 //       {
 //           double minDiff = 1e10;


 //           int indexOfClosest = -1;
 //           int numWrongDirection = 0;

 //           for (int i = 0; i < this.xvalues.Length; i++)
 //           {
 //               double currentDiff = Math.Abs(targetXVal - this.xvalues[i]);


 //               if (currentDiff < minDiff)
 //               {
 //                   indexOfClosest = i;
 //                   minDiff = currentDiff;

 //               }
 //               else
 //               {
 //                   numWrongDirection++;
 //                   if (numWrongDirection > 3) break;    //three values in a row that indicate we are moving away from the target val
 //               }

 //           }

 //           return indexOfClosest;

 //       }


//public void getSummedSpectrum(ScanSet scanSet, ref double[] xvals, ref double[] yvals, double minX, double maxX)
//        {
//            // [gord] idea borrowed from Anuj! Jan 2010 

//            //the idea is to convert the mz value to a integer. To avoid losing precision, we multiply it by 'precision'
//            //the integer is added to a dictionary generic list (sorted)
//            //

//            SortedDictionary<long, double> mz_intensityPair = new SortedDictionary<long, double>();
//            double precision = 1e5;   // if the precision is set too high, can get artifacts in which the intensities for two m/z values should be added but are separately registered. 
//            double[] tempXvals = new double[0];
//            double[] tempYvals = new double[0];

//            long minXLong = (long)(minX * precision + 0.5);
//            long maxXLong = (long)(maxX * precision + 0.5);
//            for (int scanCounter = 0; scanCounter < scanSet.IndexValues.Count; scanCounter++)
//            {
//                this.RawData.GetSpectrum(scanSet.IndexValues[scanCounter], ref tempXvals, ref tempYvals);

//                for (int i = 0; i < tempXvals.Length; i++)
//                {
//                    long tempmz = (long)Math.Floor(tempXvals[i] * precision + 0.5);
//                    if (tempmz < minXLong || tempmz > maxXLong) continue;

//                    if (mz_intensityPair.ContainsKey(tempmz))
//                    {
//                        mz_intensityPair[tempmz] += tempYvals[i];
//                    }
//                    else
//                    {
//                        mz_intensityPair.Add(tempmz, tempYvals[i]);
//                    }
//                }
//            }

//            if (mz_intensityPair.Count == 0) return;
//            List<long> summedXVals = mz_intensityPair.Keys.ToList();

//            xvals = new double[summedXVals.Count];
//            yvals = mz_intensityPair.Values.ToArray();

//            for (int i = 0; i < summedXVals.Count; i++)
//            {
//                xvals[i] = summedXVals[i] / precision;
//            }
//        }


