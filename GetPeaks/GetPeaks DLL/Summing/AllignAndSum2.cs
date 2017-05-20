using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.DataFIFO;
using PNNLOmics.Data;
using GetPeaks_DLL.Functions;

namespace GetPeaks_DLL.Summing
{
    public class AllignAndSum2
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
        public AllignAndSum2()
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
            double FFTPeakSpacingCoefficient = (1.84175648 * Math.Pow(10, -7));//these are recalculated based on the data
            double FFTPeakSpacingPower = 1.50359798;//these are recalculated based on the data

            
            int points = 999999999;//400000 will get to m/z 1991;402000 will get over 20000

            
            #endregion

            #region convertToINt64Costants
            Int64 startMZ64 = Convert.ToInt64(startMZ * bitshiftForRounding);
            Int64 endMZ64 = Convert.ToInt64(endMZ*bitshiftForRounding);
            Int64 pointSpacing64 = Convert.ToInt64(pointSpacing);
            
            #endregion

            #region Setup XY Data for each scan (+89.3)

            List<int> scanIndexList = new List<int>();

            for (int scanIndex = peakFirstScan; scanIndex <= peakLastScan; scanIndex++)
            {
                if (run.GetMSLevel(scanIndex) == 1)
                {
                    scanIndexList.Add(scanIndex);
                }
            }

            List<XYDataAndPeakHolderObjectInt64> DataToSum;
            CacheXYData.CacheDataInt64(run, scanIndexList, bitshiftForRounding, out DataToSum);

            bool print1 = false;
            if (print1)//28
            {
                for (int i = 0; i < scanIndexList.Count;i++)
                {
                    //IRapidCompare compareHere = new CompareContrast2();
                    IXYDataWriter newXYWriter = new DataXYDataWriter();
                    string path = @"V:\XYOutRaw" + scanIndexList[i].ToString() + ".txt";


                    //int t = newXYWriter.WriteOmicsXYData(DataToSum[i].SpectraDataOMICS, path);
                }
            }

            if (DataToSum.Count > 1)
            {
                Console.WriteLine("sum Data");
                //Console.ReadKey();

            }
            #endregion

            #region convertToInt64(+0)
            //List<XYDataAndPeakHolderObjectInt64> DataToSumInt64 = new List<XYDataAndPeakHolderObjectInt64>();
            //for (int i = 0; i < DataToSum.Count; i++)
            //{
            //    XYDataAndPeakHolderObjectInt64 Data64 = new XYDataAndPeakHolderObjectInt64(DataToSum[i].PeakListY.Length);
            //    Data64.PeakListY = DataToSum[i].PeakListY;//fastest way to deal with y values
            //    Data64.PeakListXdouble = DataToSum[i].SpectraDataDECON.Xvalues;
            //    for (int j = 0; j < DataToSum[i].SpectraDataOMICS.Count; j++)
            //    {
            //        Data64.PeakListX[j] = Convert.ToInt64(DataToSum[i].SpectraDataOMICS[j].X);
                   
            //    }
            //    DataToSumInt64.Add(Data64);
            //}

            #endregion

            #region calculate power coefficients off of first scan (+18.9, 108.9)
            //TODO implement for other scans and average?????

            List<double> differenceArray = new List<double>();
            List<double> ratioArray = new List<double>();

            differenceArray.Add(DataToSum[0].PeakListX64[1] - DataToSum[0].PeakListX64[0]);
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

            pointSpacing64 = DataToSum[0].PeakListX64[1] - DataToSum[0].PeakListX64[0];

            pointSpacing = pointSpacing64 / bitshiftForRounding;//back to 32

            //we need to collect a nice set of point differences that do not include large m/z gaps
            List<int> keeperXValues = new List<int>();
            
            for (int i = 1; i < DataToSum[0].PeakListX64.Count; i++)//use the ratios of the differences to find a blank in the data
            {
                XYDataAndPeakHolderObjectInt64 currentData = DataToSum[0];
                differenceArray.Add(currentData.PeakListX64[i] - currentData.PeakListX64[i - 1]);
                ratio = differenceArray[i] / differenceArray[i - 1];
                ratioArray.Add(ratio);
                if (ratio > minRatioError && ratio < maxRatioError)
                {
                    keeperXValues.Add(i);

                    //we need a sum of :X*Y, X, Y, X^2, and N

                    X = Math.Log10(currentData.PeakListX64[i]/bitshiftForRounding);
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

            #region Calculate Theoretical X Axis Map and bins (+4, 112.8)

            SortedList<double, int> MZspine = new SortedList<double, int>();//m/x, index
            SortedList<double, int> MZspineHalfPointOffset = new SortedList<double, int>();//sets up bin boundaries between the points
            SortedList<Int64, int> MZspineInt64 = new SortedList<Int64, int>();
            SortedList<Int64, int> MZspineHalfPointOffsetInt64 = new SortedList<Int64, int>();//sets up bin boundaries between the points

            List<XYDataInt64> newSpectra = new List<XYDataInt64>();//final aglomeration
            //first point setup

            MZspine.Add(startMZ, 0);
            MZspineHalfPointOffset.Add(startMZ - (FFTPeakSpacingCoefficient * Math.Pow(startMZ, FFTPeakSpacingPower)) / 2, 0);
            //MZspineInt64.Add(Convert.ToInt64(Math.Floor(startMZ * bitshiftForRounding)), 0);
            //MZspineHalfPointOffsetInt64.Add(Convert.ToInt64(Math.Floor((startMZ - pointSpacing / 2) * bitshiftForRounding)), 0);

            XYDataInt64 firstPoint = new XYDataInt64(startMZ64, 0);
            newSpectra.Add(firstPoint);
            double tempMZ = 0;
            double tempMZNonLinear = 0;

            //this creates the correct theoretical X-axis and new number of datapoints
            double pointSpacingNonLinear = 0;
            for (int i = 1; i < points; i++)
            {
                tempMZ = i * pointSpacing + startMZ;
                pointSpacingNonLinear = FFTPeakSpacingCoefficient * Math.Pow(tempMZ, FFTPeakSpacingPower);

                tempMZNonLinear = MZspine.Keys[i - 1] + pointSpacingNonLinear;
                MZspine.Add(tempMZNonLinear, i);//add the spaceing to the previous point
                MZspineHalfPointOffset.Add(tempMZNonLinear - pointSpacingNonLinear / 2, i);//add the spaceing to the previous point//

                MZspineInt64.Add(Convert.ToInt64(Math.Floor(tempMZNonLinear * bitshiftForRounding)), i);
                MZspineHalfPointOffsetInt64.Add(Convert.ToInt64(Math.Floor((tempMZNonLinear - pointSpacingNonLinear / 2) * bitshiftForRounding)), i);

                XYDataInt64 nextPoint = new XYDataInt64(Convert.ToInt64(tempMZNonLinear*bitshiftForRounding), 0);//creates a blank list of y values
                newSpectra.Add(nextPoint);

                if (tempMZNonLinear > endMZ64)
                {
                    points = MZspine.Count;
                    break;
                }
            }

            #endregion

            #region find actual x axis offset based on first point of raw data (+7, 119.8)
            //this is a coarse allignment but it helps

            List<Int64> xAxisOffsets = new List<Int64>();
            Int64 tempFirstPointX = 0;
            Int64 tempBinSpine = 0;
            Int64 tempBinSpine2 = 0;
            int counter = 0;

            for (int i = 0; i < DataToSum.Count; i++)
            {
                counter = 0;
                tempFirstPointX = DataToSum[i].PeakListX64[0];
                tempBinSpine = 0;

                tempBinSpine = MZspineInt64.Keys[counter];

                while (tempFirstPointX >= tempBinSpine)
                {
                    counter++;
                    tempBinSpine = MZspineInt64.Keys[counter];
                }
                //double tempBinSpine1 = MZspine.Keys[counter - 1];
                tempBinSpine2 = MZspineInt64.Keys[counter];
                //double tempBinSpine3 = MZspine.Keys[counter + 1];

                xAxisOffsets.Add(MZspineInt64.Keys[counter] - tempFirstPointX);//correct offset
                //Int64 correctedValue = tempFirstPointX + xAxisOffsets[i];
                //double error = tempFirstPointX + correctedValue; 
                //error++;
                //error--;
            }
            #endregion
           
            #region Now that we know the offset, correct data based on difference so we can center the bins. (+50, 172)

            ////this may be caused by a automatic gain control calibration correction
            Int64 errorInside = 0;
            for (int i = 0; i < DataToSum.Count; i++)
            {
                errorInside = xAxisOffsets[i]; ;
                for(int j=0;j<DataToSum[i].PeakListX64.Count;j++)
                //foreach (XYData dataPoint in DataToSum[i].SpectraDataOMICS)
                {
                    DataToSum[i].PeakListX64[j] += errorInside;//this helps
                    //dataPoint.X += errorInside;//this helps
                }
            }
            #endregion
                
            #region bin data as quickly as possible

            //1.  take experimental value and divide it by 

            List<Int64> topPoints = new List<Int64>();
            List<Int64> topErrors = new List<Int64>();
            List<int> topIndexes = new List<int>();


            Int64 DataX = 0;
            Int64 lower = 0;
            Int64 upper = 0;
            Int64 difference = 0;
            int NumberOfScansToSum = DataToSum.Count;

            int Misscounter = 0;//the miss counter is interesting.  it identifies places where the data is off
            foreach (XYDataAndPeakHolderObjectInt64 scanToSum in DataToSum)
            {
                int dataCounter = 0;
                int spineCounter = 0;
                int dataLength = scanToSum.PeakListX64.Count;
                int spineLength = newSpectra.Count;

                while (dataCounter < dataLength - 1 && spineCounter < newSpectra.Count)//for each point in the spine...//-1 for last point.  It is best to make sure the spine is longer than the data
                {
                    //Int64 DataX = Convert.ToInt64(scanToSum.SpectraDataOMICS[dataCounter].X * bitshiftForRounding);
                    DataX = scanToSum.PeakListX64[dataCounter];
                    //Int64 lower = MZspineHalfPointOffsetInt64.Keys[spineCounter];
                    //Int64 upper = MZspineHalfPointOffsetInt64.Keys[spineCounter + 1];
                    lower = MZspineHalfPointOffsetInt64.Keys[spineCounter];
                    upper = MZspineHalfPointOffsetInt64.Keys[spineCounter + 1];
                    difference = DataX - lower;
                    difference++; difference--;
                    if (DataX >= lower && DataX <= upper)
                    {
                        newSpectra[spineCounter].Y += scanToSum.PeakListY[dataCounter];
                        newSpectra[spineCounter].X = scanToSum.PeakListX64[dataCounter];//this is not needed.  this is only here so we can observe the differences propegating based on the linear shift
                        dataCounter++;
                        DataX = scanToSum.PeakListX64[dataCounter];
                        if (DataX >= upper)//if the next point is greater than upper, adcanve to next bin
                        {
                            spineCounter++;
                        }//else stay in the same bin
                        //Console.WriteLine("dataCounter = " + dataCounter.ToString() + "/" + (dataLength - 1).ToString() + " spineCounter = " + spineCounter.ToString() + "/" + spineLength.ToString() + " DataX: " + DataX.ToString());
                        //DataX = Convert.ToInt64(scanToSum.SpectraDataOMICS[dataCounter].X * bitshiftForRounding);

                        difference = DataX - newSpectra[spineCounter].X;
                        while (DataX < newSpectra[spineCounter].X)//if 
                        {
                            dataCounter++;//incase there are multiple data points mapping to one spine counter.  we need to bump up the data counter so we can procede
                            DataX = scanToSum.PeakListX64[dataCounter];
                        }
                    }
                    else
                    {
                        spineCounter++;
                        if (DataX < lower)
                        {
                            dataCounter++;
                            Misscounter++;
                            spineCounter--;
                        }

                    }
                }
            }

            #endregion

                ////we need to check this
                //bool check = false;
                #region check x errors.

                //if (check)
                //{

                //    List<double> XdifferenceList = new List<double>();
                //    Console.WriteLine("There are " + Misscounter.ToString() + " missing values");
                //    foreach (XYDataAndPeakHolderObject scanToSum in DataToSum)
                //    {
                //        for (int i = 0; i < newSpectra.Count; i++)
                //        {
                //            XdifferenceList.Add(newSpectra[i].X - MZspine.Keys[i]);

                //        }
                //    }

                //    //for (int y = 12158; y < 12170; y++)
                //    //{
                //    //    Console.WriteLine(XdifferenceList[y].ToString() + " " + y.ToString());
                //    //}
                //    //Console.WriteLine("h");
                //    //for (int y = 19980; y < 19990; y++)
                //    //{
                //    //    Console.WriteLine(XdifferenceList[y].ToString() + " " + y.ToString());
                //    //}
                //    //Console.WriteLine("h");
                //    //for (int y = 30770; y < 30780; y++)
                //    //{
                //    //    Console.WriteLine(XdifferenceList[y].ToString() + " " + y.ToString());
                //    //}

                //    //Console.WriteLine("h");
                //    //for (int y = 39860; y < 39870; y++)
                //    //{
                //    //    Console.WriteLine(XdifferenceList[y].ToString() + " " + y.ToString());
                //    //}
                //    //Console.WriteLine("h");
                //    XdifferenceList.Sort();
                //    Console.WriteLine("TheLargestDifference is " + XdifferenceList[0].ToString() + " or " + XdifferenceList[XdifferenceList.Count - 1].ToString());

                //    double highQualityCount = 0;
                //    double averageDiffereence = 0;
                //    for (int i = 0; i < XdifferenceList.Count; i++)
                //    {
                //        double absDiference = Math.Abs(XdifferenceList[i]);
                //        if (absDiference < 0.001)
                //        {
                //            highQualityCount++;
                //        }
                //        averageDiffereence += XdifferenceList[i];
                //    }
                //    averageDiffereence = averageDiffereence / XdifferenceList.Count;
                //    Console.WriteLine("TheAverageDifference is " + averageDiffereence.ToString());
                //    double size = Convert.ToDouble(XdifferenceList.Count);
                //    double quality = highQualityCount / size * 100;
                //    Console.WriteLine("ThePercentOfHighQuality is " + quality.ToString() + "%");
                //}
                #endregion

                #region convert back to XYdata Decon and store in run as for return

                //List<XYData> patchTogether = new List<XYData>();
                //for (int i = 0; i < points; i++)//for each point in the spine...
                //{
                //    XYData tempoint = new XYData(newSpectra[i].X, newSpectra[i].Y);
                //    patchTogether.Add(tempoint);
                //}
                //ConvertXYData.OmicsXYDataToRunXYData(ref run, patchTogether);
                //#endregion

                //#region print alligned data
                //bool print = false;
                //if (print)//28
                //{
                //    //IRapidCompare compareHere = new CompareContrast2();
                //    IXYDataWriter newXYWriter = new DataXYDataWriter();
                //    string path = @"V:\XYOutAllign" + peakFirstScan.ToString() + ".txt";



                //    int t = newXYWriter.WriteOmicsXYData(newSpectra, path);
                //}
                #endregion
            
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

