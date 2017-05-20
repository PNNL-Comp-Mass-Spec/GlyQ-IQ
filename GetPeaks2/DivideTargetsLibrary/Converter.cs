using System;
using System.Globalization;
using System.Linq;

namespace DivideTargetsLibrary
{
    public static class Converter
    {
        public static char[] ConvertEnding(string dataFileEnding)
        {
            char[] endingInCharData = new char[dataFileEnding.Count()];
            for (int j = 0; j < dataFileEnding.Length; j++)
            {
                endingInCharData[j] = dataFileEnding[j];
            }
            return endingInCharData;
        }

        public static int ConvertStringToInt(string coresString)
        {
            CultureInfo myCultureInfo = new CultureInfo("en-GB");
            double coresDouble = 0;
            Double.TryParse(coresString, System.Globalization.NumberStyles.Integer, myCultureInfo, out coresDouble);

            int cores = (int)coresDouble;
            return cores;
        }
    }
}
