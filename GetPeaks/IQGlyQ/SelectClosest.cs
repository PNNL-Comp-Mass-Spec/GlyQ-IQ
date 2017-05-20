using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using PNNLOmics.Data;

namespace IQGlyQ
{
    public static class SelectClosest
    {

        public static ScanSet SelectClosestScanSetToScan(Run runIn, int approximateScanCentroidFromTallestPeak)
        {
            ScanSet scanSelected = null;
            scanSelected = runIn.ScanSetCollection.GetScanSet(approximateScanCentroidFromTallestPeak);

            //Console.WriteLine("The center is close to " + approximateScanCentroidFromTallestPeak);
            if (scanSelected == null)//we missed the center scanset so we need to find it so we sync with decon scanset collection
            {
                List<int> primaryScanNumbers = runIn.PrimaryLcScanNumbers.ToList();
                for (int i = 0; i < primaryScanNumbers.Count; i++)
                {
                    int placeHolderScanNumber = primaryScanNumbers[i] - approximateScanCentroidFromTallestPeak;
                    //placeHolderScanNumber = Math.Abs(primaryScanNumbers[i]);
                    placeHolderScanNumber = Math.Abs(placeHolderScanNumber);
                    primaryScanNumbers[i] = placeHolderScanNumber;

                    //primaryScanNumbers[i] =  primaryScanNumbers[i] - approximateScanCentroidFromTallestPeak;
                    //primaryScanNumbers[i] =  Math.Abs(primaryScanNumbers[i]);
                    //Console.WriteLine(primaryScanNumbers[i]);
                }

                //Console.WriteLine("start " + approximateScanCentroidFromTallestPeak);

                primaryScanNumbers.Sort(); //the lowest value will be our best mass match
                int closestPrimaryScanNumber = runIn.MinLCScan;
                try
                {
                    double testNumber = primaryScanNumbers.FirstOrDefault();

                    //Console.WriteLine(testNumber);

                    closestPrimaryScanNumber = Convert.ToInt32(testNumber) + approximateScanCentroidFromTallestPeak;
                    //Console.WriteLine(closestPrimaryScanNumber);
                    scanSelected = runIn.ScanSetCollection.GetScanSet(closestPrimaryScanNumber);

                    if (scanSelected == null)//we need to try the other root since it could be +-1 etc. due to the squaring
                    {
                        //Console.WriteLine("We have failed to find an appropriate scan");

                        closestPrimaryScanNumber = Convert.ToInt32(-Convert.ToInt32(testNumber)) + approximateScanCentroidFromTallestPeak;
                        scanSelected = runIn.ScanSetCollection.GetScanSet(closestPrimaryScanNumber);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("SquareRootFailed.");
                    Console.ReadKey();
                    throw;
                }
            }

            return scanSelected;
        }

        public static ProcessedPeak ClosestMsPeakToTheoretical(double massToExtract, List<ProcessedPeak> msPeaks)
        {
            ProcessedPeak closestMsPeakToTheoretical = new ProcessedPeak();
            if (msPeaks.Count > 0)
            {

                foreach (var processedPeak in msPeaks)
                {
                    processedPeak.XValue -= massToExtract;
                    processedPeak.XValue = Math.Abs(processedPeak.XValue); //this gets turns in to a RMS
                }

                msPeaks = msPeaks.OrderBy(n => n.XValue).ToList(); //the lowest value will be our best mass match
                closestMsPeakToTheoretical = msPeaks.FirstOrDefault();
            }
            else
            {
                Console.WriteLine("there are no peaks");
                Console.ReadKey();
            }
            return closestMsPeakToTheoretical;
        }

        public static ProcessedPeak SelectClosestPeakToCenter(List<ProcessedPeak> peaks, double peakCenter)
        {
            //processed Peaks must have a CenterIndexLeft

            ProcessedPeak selectedPeak = new ProcessedPeak();


            //Console.WriteLine("The center is close to " + peakCenter);
            if (peaks.Count > 1)//we missed the center scanset so we need to find it so we sync with decon scanset collection
            {
                List<ProcessedPeak> workingPeaks = new List<ProcessedPeak>();
                foreach (var processedPeak in peaks)
                {
                    ProcessedPeak newPeak = new ProcessedPeak(processedPeak.XValue, processedPeak.Height, processedPeak.ScanNumber);
                    newPeak.CenterIndexLeft = processedPeak.CenterIndexLeft;
                    workingPeaks.Add(newPeak);
                }

                for (int i = 0; i < workingPeaks.Count; i++)
                {
                    double placeHolderNumber = workingPeaks[i].XValue - peakCenter;
                    //placeHolderScanNumber = Math.Abs(primaryScanNumbers[i]);
                    placeHolderNumber = placeHolderNumber * placeHolderNumber;//RMS
                    workingPeaks[i].XValue = placeHolderNumber;

                    //primaryScanNumbers[i] =  primaryScanNumbers[i] - approximateScanCentroidFromTallestPeak;
                    //primaryScanNumbers[i] =  Math.Abs(primaryScanNumbers[i]);
                    //Console.WriteLine(primaryScanNumbers[i]);
                }

                //Console.WriteLine("start " + peakCenter);

                workingPeaks.OrderBy(r => r.XValue).ToList(); //the lowest value will be our best mass match

                try
                {
                    double testNumber = Math.Sqrt(workingPeaks[0].XValue);//get back to normal x value

                    int selectedPeakIdentifier = workingPeaks[0].CenterIndexLeft;//
                    selectedPeak = (from peak in peaks where peak.CenterIndexLeft == selectedPeakIdentifier select peak).FirstOrDefault();
                    //we need to reset the xvalue


                }
                catch (Exception)
                {
                    Console.WriteLine("SquareRootFailed.");
                    Console.ReadKey();
                    throw;
                }
            }
            else
            {
                //there is only one peak so return ir
                selectedPeak = peaks.FirstOrDefault();
            }

            return selectedPeak;
        }

        public static DeconTools.Backend.Core.Peak SelectNearestPeakToCenter(List<DeconTools.Backend.Core.Peak> peaks, double peakCenter)
        {
            //processed Peaks must have a CenterIndexLeft

            DeconTools.Backend.Core.Peak selectedPeak = new DeconTools.Backend.Core.Peak();


            //Console.WriteLine("The center is close to " + peakCenter);
            if (peaks.Count > 1)//we missed the center scanset so we need to find it so we sync with decon scanset collection
            {
                List<Tuple<double, DeconTools.Backend.Core.Peak>> tuplePile = new List<Tuple<double, DeconTools.Backend.Core.Peak>>();

                for (int i = 0; i < peaks.Count; i++)
                {
                    DeconTools.Backend.Core.Peak currentPeak = peaks[i];
                    double difference = currentPeak.XValue - peakCenter;
                    tuplePile.Add(new Tuple<double, DeconTools.Backend.Core.Peak>(difference * difference, currentPeak));
                }

                List<Tuple<double, DeconTools.Backend.Core.Peak>> sortedTuplePile = tuplePile.OrderBy(r => r.Item1).ToList();

                selectedPeak = sortedTuplePile[0].Item2;

            }
            else
            {
                //there is only one peak so return ir
                selectedPeak = peaks.FirstOrDefault();
            }

            return selectedPeak;
        }

        public static MSPeak SelectNearestMSPeakToCenter(List<MSPeak> peaks, double peakCenter)
        {
            //processed Peaks must have a CenterIndexLeft

            MSPeak selectedPeak = new MSPeak();


            //Console.WriteLine("The center is close to " + peakCenter);
            if (peaks.Count > 1)//we missed the center scanset so we need to find it so we sync with decon scanset collection
            {
                List<Tuple<double, MSPeak>> tuplePile = new List<Tuple<double, MSPeak>>();

                for (int i = 0; i < peaks.Count; i++)
                {
                    MSPeak currentPeak = peaks[i];
                    double difference = currentPeak.XValue - peakCenter;
                    tuplePile.Add(new Tuple<double, MSPeak>(difference * difference, currentPeak));
                }

                List<Tuple<double, MSPeak>> sortedTuplePile = tuplePile.OrderBy(r => r.Item1).ToList();

                selectedPeak = sortedTuplePile[0].Item2;

            }
            else
            {
                //there is only one peak so return ir
                selectedPeak = peaks.FirstOrDefault();
            }

            return selectedPeak;
        }
    }
}
