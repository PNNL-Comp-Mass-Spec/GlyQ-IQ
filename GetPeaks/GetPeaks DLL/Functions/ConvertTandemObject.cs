using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects.TandemMSObjects;

namespace GetPeaks_DLL.Functions
{
    public static class ConvertTandemObject
    {
        public static void YAFMSToRAW(TandemObjectYAFMS yafms, out TandemObjectRAW newRaw)
        {
            newRaw = new TandemObjectRAW();
            newRaw.FragmentationScanNumber = yafms.FragmentationScanNumber;
            newRaw.PeaksIsosResultsDECON = yafms.PeaksIsosResultsDECON;
            newRaw.PrecursorScanPeaks = yafms.PeaksOMICS;
            newRaw.PrecursorMZ = yafms.PrecursorMZ;
            newRaw.PrecursorScanNumber = yafms.PrecursorScanNumber;
            newRaw.FragmentationData = yafms.FragmentationData;
            newRaw.PrecursorData = yafms.PrecursorData;
        }
    }
}
