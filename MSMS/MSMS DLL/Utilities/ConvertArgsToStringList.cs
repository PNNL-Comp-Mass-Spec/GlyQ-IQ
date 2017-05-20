using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.ConosleUtilities;
using GetPeaks_DLL.DataFIFO;

namespace MSMS_DLL.Utilities
{
    public class ConvertArgsToStringList
    {
        public void Convert(string[] args, out List<string> mainParameterFile, out List<string> stringListFromParameterFile)
        {
            //TODO remove file target and make everyting work
            FileTarget target = new FileTarget();
            target = FileTarget.WorkStandardTest;//<---used to target what computer we are working on:  Home or Work.  Server is covered by command line

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
                mainParameterFile = parser.Parse3Args(args);
            }

            switch (target)
            {
                #region fileswitch
                case FileTarget.WorkStandardTest://work
                    {
                        //loaded from pre build events
                    }
                    break;
                case FileTarget.HomeStandardTest:
                    {
                        mainParameterFile[0] = @"G:\PNNL Files\PNNL CSharp\0_BatchFiles\0_HomeParameterFileSN09a.txt";
                        mainParameterFile[1] = @"G:\PNNL Files\CSharp\GetPeaksOutput\TextBatchResult";
                        mainParameterFile[2] = @"G:\PNNL Files\CSharp\GetPeaksOutput\SQLiteBatchResult";
                    }
                    break;
                #endregion
            }
            #endregion

            //load parameters
            stringListFromParameterFile = new List<string>();
            FileIteratorStringOnly loadParameter = new FileIteratorStringOnly();
            stringListFromParameterFile = loadParameter.loadStrings(mainParameterFile[0]);
        }

    }


}
