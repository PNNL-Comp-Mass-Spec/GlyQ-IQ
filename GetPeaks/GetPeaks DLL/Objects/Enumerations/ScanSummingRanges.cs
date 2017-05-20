using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Objects.Enumerations
{
    public enum ScanSummingRanges
    {
        OneScan,
        ThreeScans,
        FiveScans,
        SevenScans,
        NineScans,
        ElevenScans
    }

    public static class SumRange
    {
        public static int ConvertRangeToInt(ScanSummingRanges range)
        {
            int result;

            switch (range)
            {
                case ScanSummingRanges.OneScan:
                {
                    result = 1;
                }
                break;
                case ScanSummingRanges.ThreeScans:
                {
                    result = 3;
                }
                break;
                case ScanSummingRanges.FiveScans:
                {
                    result = 5;
                }
                break;
                case ScanSummingRanges.SevenScans:
                {
                    result = 7;
                }
                break;
                case ScanSummingRanges.NineScans:
                {
                    result = 9;
                }
                break;
                case ScanSummingRanges.ElevenScans:
                {
                    result = 11;
                }
                break;
                default:
                {
                    result = 1;
                }
                break;
            }

            return result;
        }
    }

}
