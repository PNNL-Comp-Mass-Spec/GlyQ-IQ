using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.AnalysisSupport.Tools
{
    public static class ConvertStringToBool
    {
        public static bool Convert(string incommingString)
        {
            bool tempBool = false;
            switch (incommingString)
            {
                case "TRUE":
                    tempBool = true;
                    break;
                case "FALSE":
                    tempBool = false;
                    break;
                case "True":
                    tempBool = true;
                    break;
                case "False":
                    tempBool = false;
                    break;
                default:
                    Console.WriteLine("Select TRUE/True or FALSE/False in the input file");
                    Console.ReadKey();
                    break;
            }
            return tempBool;
        }
    }
}
