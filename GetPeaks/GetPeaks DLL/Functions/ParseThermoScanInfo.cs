using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GetPeaks_DLL.Functions
{
    public static class ParseThermoScanInfo
    {
        public static double ExtractMass(string scanInfo)
        {
            double precursorMass = 0;

            string pattern = @"(?<mz>[0-9.]+)@cid";

            var match = Regex.Match(scanInfo, pattern);

            if (match.Success)
            {
                precursorMass = Convert.ToDouble(match.Groups["mz"].Value);
            }
            else
            {
                precursorMass = -1;
            }

            return precursorMass;
        }
    }
}
