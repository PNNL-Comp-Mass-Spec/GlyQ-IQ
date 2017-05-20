using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using System.IO;

namespace GetPeaks_DLL.Functions
{
    public class ConvertArgsToFileName
    {  
        public static void GetFileName(string[] args, out InputOutputFileName newFile)
        {
            List<string> mainParameterFile;
            List<string> stringListFromParameterFile;
            ConvertArgsToStringList argsToList = new ConvertArgsToStringList();
            argsToList.Convert(args, out mainParameterFile, out stringListFromParameterFile);
            //argsToList.Convert2013(args, out mainParameterFile, out stringListFromParameterFile);

            string serverDataFileName = stringListFromParameterFile[1];

            newFile = new InputOutputFileName();
            newFile.InputFileName = serverDataFileName;//.yafms
            //newFile.OutputFileName = mainParameterFile[1] + folderID + "_" + serverBlock + @".txt";
            //newFile.OutputSQLFileName = mainParameterFile[2] + folderID + @".db";

            FileInfo fi = new FileInfo(newFile.InputFileName);
            bool exists = fi.Exists;
        }
    }
}
