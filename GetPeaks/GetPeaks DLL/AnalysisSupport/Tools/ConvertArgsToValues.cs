using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.ConosleUtilities;
using GetPeaks_DLL.DataFIFO;
using System.IO;
using GetPeaks_DLL.Objects;

namespace GetPeaks_DLL.AnalysisSupport.Tools
{
    public class ConvertArgsToValues
    {
        public void ArgsTo3Values(string[] args, bool readKeyToggle, out List<string> mainParameterFile)
        {
            #region args to values
            //setup main parameter file from args
            
            mainParameterFile = new List<string>();
            #region switch from server to desktop based on number of args
            if (args.Length == 0)//debugging
            {
                mainParameterFile.Add(""); mainParameterFile.Add(""); mainParameterFile.Add("");
            }
            else
            {
                Console.WriteLine("ParseArgs");
                ParseStrings parser = new ParseStrings();
                mainParameterFile = parser.Parse3Args(args); //three arguments are needed.  first one is the path to the parameter file
            }
            #endregion

            #endregion
        }

        public void ArgsTo4Values(string[] args, bool readKeyToggle, out List<string> mainParameterFile)
        {
            #region args to values
            //setup main parameter file from args

            mainParameterFile = new List<string>();
            #region switch from server to desktop based on number of args
            if (args.Length == 0)//debugging
            {
                mainParameterFile.Add(""); mainParameterFile.Add(""); mainParameterFile.Add(""); mainParameterFile.Add("");
            }
            else
            {
                Console.WriteLine("ParseArgs");
                ParseStrings parser = new ParseStrings();
                mainParameterFile = parser.Parse4Args(args); //three arguments are needed.  first one is the path to the parameter file
            }
            #endregion

            #endregion
        }
    }
}
