using System;
using System.Collections.Generic;
using System.IO;
using DivideTargetsLibraryX64.Parameters;

namespace DivideTargetsLibraryX64
{
    public static class DataFileSetupForMultipleCopies
    {
        public static void DataFileSetup(int cores, ParameterDivideTargets parameters, out List<string> dividedDataFileNames, out List<string> dividedDataFileEnding)
        {
            dividedDataFileNames = new List<string>();
            dividedDataFileEnding = new List<string>();

            
            string baseDataFile = parameters.DataFileFolder + "\\" + parameters.DataFileFileName;
            string dataFileEnding = "." + parameters.DataFileEnding;
            //for (int i = 0; i < cores; i++)
            for (int i = 1; i <= cores; i++)
            {
                string coreName = "_" + i;
                string copiedName = baseDataFile + coreName + dataFileEnding;


                string dataFileParentPath = baseDataFile + dataFileEnding;

                if (File.Exists(dataFileParentPath))
                {
                    //runLines[i].DataFileName = parameters.DataFileFileName + coreName;
                    //runLines[i].DataFileEnding = parameters.DataFileEnding;
                    dividedDataFileNames.Add(parameters.DataFileFileName + coreName);
                    dividedDataFileEnding.Add(parameters.DataFileEnding);
                    if (!File.Exists(copiedName))
                    {
                        Console.WriteLine("CopyingFile: " + dataFileParentPath);
                        System.IO.File.Copy(dataFileParentPath, copiedName);
                    }
                }
            }
        }
    }
}
