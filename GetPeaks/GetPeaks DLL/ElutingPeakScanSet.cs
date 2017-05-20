using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using GetPeaks_DLL.Objects;

namespace GetPeaks_DLL
{
    public class ElutingPeakScanSet:IDisposable
    {
        public ScanSet scanSet { get; set; }
        
        //public ScanSet CreateScanSetFromElutingPeakSum(Run run, int peakCenter, int peakFirstScan, int peakLastScan)
        //public void CreateScanSetFromElutingPeakSum(Run run, int peakCenter, int peakFirstScan, int peakLastScan)
        //{
        //    List<int> scanIndexList = new List<int>();

        //    for (int scanIndex = peakFirstScan; scanIndex <= peakLastScan; scanIndex++)
        //    {
        //        if (run.GetMSLevel(scanIndex) == 1)
        //        {
        //            scanIndexList.Add(scanIndex);
        //        }
        //    }

        //    scanSet = new ScanSet(peakCenter, scanIndexList.ToArray());
        //    scanIndexList = null;
        //    //return scanSet;
        //}

        //public ScanSet CreateScanSetFromElutingPeakMax(Run run, int peakCenter, int peakFirstScan, int peakLastScan)
        public void CreateScanSetFromElutingPeakMax(Run run, int peakCenter, int peakFirstScan, int peakLastScan, ScanSumSelectSwitch switchMaxScanOrSum, bool msOnly)
        {
            List<int> scanIndexList = new List<int>();

            //this tells wheather we sum the scans or take the best scan to send to the deisotoping
            switch(switchMaxScanOrSum)
            {
                case ScanSumSelectSwitch.SumScan:
                    {
                        for (int scanIndex = peakFirstScan; scanIndex <= peakLastScan; scanIndex++)
                        {
                            if (msOnly)
                            {
                                if (run.GetMSLevel(scanIndex) == 1)
                                {
                                    scanIndexList.Add(scanIndex);
                                }
                            }
                            else
                            {
                                scanIndexList.Add(scanIndex);
                            }
                        }
                        break;
                    }
                case ScanSumSelectSwitch.MaxScan:
                    {
                        scanIndexList.Add(peakCenter);
                        break;
                    }
                case ScanSumSelectSwitch.AlignScan:
                    {
                        for (int scanIndex = peakFirstScan; scanIndex <= peakLastScan; scanIndex++)
                        {
                            if (run.GetMSLevel(scanIndex) == 1)
                            {
                                scanIndexList.Add(scanIndex);
                            }
                        }
                        break;
                    }
                default:
                    {
                        scanIndexList.Add(peakCenter);
                        break;
                    }
            }

            scanSet = new ScanSet(peakCenter, scanIndexList.ToArray());
            scanIndexList = null;
            //return scanSet;
        }



        #region IDisposable Members

        public void Dispose()
        {
            //this.scanSet.BasePeak = null;
            //this.scanSet.IndexValues = null;
           //this.scanSet = null;
        }

        #endregion
    }
}
