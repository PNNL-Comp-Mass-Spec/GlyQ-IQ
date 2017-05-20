using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using GetPeaks_DLL.Objects;
using PNNLOmics.Data;
using GetPeaks_DLL.Functions;
using GetPeaks_DLL.DataFIFO;

namespace GetPeaks_DLL.Summing
{
    public class AllignAndSum
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
        public AllignAndSum()
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

            bool print1 = false;
            if (print1)//28
            {
                for (int i = 0; i < scanIndexList.Count;i++)
                {
                    //IRapidCompare compareHere = new CompareContrast2();
                    IXYDataWriter newXYWriter = new DataXYDataWriter();
                    string path = @"V:\XYOutRaw" + scanIndexList[i].ToString() + ".txt";


                    int t = newXYWriter.WriteOmicsXYData(DataToSum[i].SpectraDataOMICS, path);
                }
            }

            if (DataToSum.Count > 1)
            {
                Console.WriteLine("sum Data");
                //Console.ReadKey();

            }
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
                differenceArray.Add(currentData.SpectraDataOMICS[i].X - currentData.SpectraDataOMICS[i-1].X);
                ratio = differenceArray[i]/differenceArray[i-1];
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
            double slope = (N*sumXY - sumX*sumY)/(N*sumX2-sumX*sumX);
            double interceptt = (sumX2 * sumY - sumX * sumXY) / (N * sumX2 - sumX * sumX);
            
            //corrected values
            FFTPeakSpacingCoefficient = Math.Pow(10,interceptt);
            FFTPeakSpacingPower = slope;

            #endregion

            #region Calculate Theoretical X Axis Map and bins

            SortedList<double,int> MZspine = new SortedList<double,int>();//m/x, index
            SortedList<double,int> MZspineHalfPointOffset = new SortedList<double,int>();//sets up bin boundaries between the points
            SortedList<Int64,int> MZspineInt64 = new SortedList<Int64,int>();
            SortedList<Int64,int> MZspineHalfPointOffsetInt64 = new SortedList<Int64,int>();//sets up bin boundaries between the points

            List<XYData> newSpectra = new List<XYData>();//final aglomeration
            //first point setup

            MZspine.Add(startMZ, 0);
            MZspineHalfPointOffset.Add(startMZ - (FFTPeakSpacingCoefficient * Math.Pow(startMZ, FFTPeakSpacingPower)) / 2, 0);
            MZspineInt64.Add(Convert.ToInt64(Math.Floor(startMZ * bitshiftForRounding)), 0);
            MZspineHalfPointOffsetInt64.Add(Convert.ToInt64(Math.Floor((startMZ - pointSpacing / 2) * bitshiftForRounding)), 0);

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
                tempMZNonLinear = MZspine.Keys[i - 1] + pointSpacingNonLinear;
                MZspine.Add(tempMZNonLinear, i);//add the spaceing to the previous point
                MZspineHalfPointOffset.Add(tempMZNonLinear - pointSpacingNonLinear / 2, i);//add the spaceing to the previous point//

                MZspineInt64.Add(Convert.ToInt64(Math.Floor(tempMZNonLinear * bitshiftForRounding)), i);
                MZspineHalfPointOffsetInt64.Add(Convert.ToInt64(Math.Floor((tempMZNonLinear - pointSpacingNonLinear / 2) * bitshiftForRounding)), i);

                XYData nextPoint = new XYData(tempMZNonLinear, 0);//creates a blank list of y values
                newSpectra.Add(nextPoint);

                if (tempMZNonLinear > endMZ)
                {
                    points = MZspine.Count;
                    break;
                }
            }

            #endregion

            #region find actual x axis offset based on first point of raw data
            //this is a coarse allignment but it helps

            List<double> xAxisOffsets = new List<double>();
            for(int i=0;i < DataToSum.Count; i++)
            {
                int counter = 0;
                double tempFirstPointX = DataToSum[i].SpectraDataOMICS[0].X;
                double tempBinSpine = 0;
                
                tempBinSpine = MZspine.Keys[counter];

                while(tempFirstPointX >= tempBinSpine)
                {
                    counter++;
                    tempBinSpine = MZspine.Keys[counter];
                }
                //double tempBinSpine1 = MZspine.Keys[counter - 1];
                double tempBinSpine2 = MZspine.Keys[counter];
                //double tempBinSpine3 = MZspine.Keys[counter + 1];

                xAxisOffsets.Add(tempBinSpine2 - tempFirstPointX);//correct offset
                double correctedValue = tempFirstPointX + xAxisOffsets[i];
                //double error = tempFirstPointX + correctedValue; 
                //error++;
                //error--;
            }
            #endregion

            #region Now that we know the offset, correct data based on difference so we can center the bins.

            //this may be caused by a automatic gain control calibration correction
            double errorInside = 0;
            for (int i = 0; i < DataToSum.Count; i++)
            {   
                errorInside = xAxisOffsets[i]; ;
                foreach (XYData dataPoint in DataToSum[i].SpectraDataOMICS)
                {
                    dataPoint.X += errorInside;//this helps
                }
            }
            #endregion

            #region bin data as quickly as possible

            //1.  take experimental value and divide it by 

            List<double> topPoints = new List<double>();
            List<double> topErrors = new List<double>();
            List<int> topIndexes = new List<int>();
            int NumberOfScansToSum = DataToSum.Count;

            int Misscounter = 0;//the miss counter is interesting.  it identifies places where the data is off
            foreach (XYDataAndPeakHolderObject scanToSum in DataToSum)
            {
                int dataCounter = 0;
                int spineCounter = 0;
                int dataLength = scanToSum.SpectraDataOMICS.Count;
                int spineLength = newSpectra.Count;
                
                while (dataCounter < dataLength - 1 && spineCounter < newSpectra.Count)//for each point in the spine...//-1 for last point.  It is best to make sure the spine is longer than the data
                {
                    Int64 DataX = Convert.ToInt64(scanToSum.SpectraDataOMICS[dataCounter].X * bitshiftForRounding);

                    Int64 lower = MZspineHalfPointOffsetInt64.Keys[spineCounter];
                    Int64 upper = MZspineHalfPointOffsetInt64.Keys[spineCounter + 1];

                    if (DataX >= lower && DataX <= upper)
                    {
                        newSpectra[spineCounter].Y += scanToSum.SpectraDataOMICS[dataCounter].Y;
                        newSpectra[spineCounter].X = scanToSum.SpectraDataOMICS[dataCounter].X;//this is not needed.  this is only here so we can observe the differences propegating based on the linear shift
                        dataCounter++;
                        spineCounter++;

                        //Console.WriteLine("dataCounter = " + dataCounter.ToString() + "/" + (dataLength - 1).ToString() + " spineCounter = " + spineCounter.ToString() + "/" + spineLength.ToString() + " DataX: " + DataX.ToString());
                        DataX = Convert.ToInt64(scanToSum.SpectraDataOMICS[dataCounter].X * bitshiftForRounding);
                        while (DataX < newSpectra[spineCounter].X)
                        {
                            dataCounter++;//incase there are multiple data points mapping to one spine counter.  we need to bump up the data counter so we can procede
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

            //we need to check this
            #region check x errors.  
            bool check = false;
            if (check)
            {

                List<double> XdifferenceList = new List<double>();
                Console.WriteLine("There are " + Misscounter.ToString() + " missing values");
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
            bool print = false;
            if (print)//28
            {
                //IRapidCompare compareHere = new CompareContrast2();
                IXYDataWriter newXYWriter = new DataXYDataWriter();
                //string path = @"V:\XYOutAllign" + peakFirstScan.ToString() + ".txt";

                string scanStr = peakFirstScan.ToString();
                string massInt = peakLastScan.ToString();
                string path = @"V:\XYOut_" + scanStr + "_" + massInt + "_S" + "x" + ".txt";

                

                int t = newXYWriter.WriteOmicsXYData(newSpectra, path);
            }
            #endregion

        }
    }
}
