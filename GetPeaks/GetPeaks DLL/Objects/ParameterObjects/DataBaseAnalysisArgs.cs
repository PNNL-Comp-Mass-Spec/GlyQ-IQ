using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.ConosleUtilities;

namespace GetPeaks_DLL.Objects.ParameterObjects
{
    public class DataBaseAnalysisArgs
    {
        public string inputDatbaseFolder { get; set; }
        public string fileNameOnly { get; set; }
        public string fileExtension { get; set; }
        //public string sqLiteFolder { get; set; }
        //public string logFile { get; set; }
        //public string parameterFileFolder { get; set; }
        //public string parameterFileNameOnly { get; set; }

        public DataBaseAnalysisArgs(string[] args)
        {
            ParseStrings parser = new ParseStrings();
            List<string> results = parser.Parse3Args(args);

            inputDatbaseFolder = results[0];
            fileNameOnly = results[1];
            fileExtension = results[2];
            //sqLiteFolder = results[3];
            //logFile = results[4];
            //parameterFileFolder = results[5];
            //parameterFileNameOnly = results[6];
        }
    }
}
