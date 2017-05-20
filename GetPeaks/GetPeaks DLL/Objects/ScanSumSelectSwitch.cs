using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Objects
{
    public enum ScanSumSelectSwitch
    {
        MaxScan,//this selects the max scan for XYdata
        SumScan,//this selects all the scans in the range
        AlignScan// this selects all scans and alligns them with errror tollerance
    }
}
