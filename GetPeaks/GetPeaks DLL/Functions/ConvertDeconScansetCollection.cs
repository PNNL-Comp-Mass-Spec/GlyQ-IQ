using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;

namespace GetPeaks_DLL.Functions
{
    public static class ConvertDeconScansetCollection
    {
        public static void ToList(Run runWithScanSetCollection, out List<int> scansList)
        {
            scansList = new List<int>();

            ScanSetCollection HoldScanSet = runWithScanSetCollection.ScanSetCollection;

            foreach (ScanSet scan in HoldScanSet.ScanSetList)
            {
                scansList.Add(scan.PrimaryScanNumber);
            }
        }
    }
}
