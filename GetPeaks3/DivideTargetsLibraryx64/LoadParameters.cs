using System;
using System.IO;
using DivideTargetsLibraryX64.Parameters;


namespace DivideTargetsLibraryX64
{
    public static class LoadParameters
    {

        public static ParameterDivideTargets SetupDivideTargetParameters(string divideTargetsParameterFile, out string baseTargetsFile, out string fullTargetPath, out string textFileEnding)
        {
            //1.  load parameter file

            ParameterDivideTargets parameters = new ParameterDivideTargets();
            parameters.SetParameters(divideTargetsParameterFile);

            //2.  check parameters


            //List<char> endingAsCharList = new List<char>();
            //endingAsCharList.Add('.');


            //char[] endingInCharData = ConvertEnding(parameters.DataFileEnding);
            //endingAsCharList.AddRange(endingInCharData);

            //char[] ending = endingAsCharList.ToArray();


            //string baseDataFile = parameters.DataFileParentPath.TrimEnd(ending);
            string baseDataFile = parameters.DataFileFolder;
            //string baseTargetsFile = parameters.DataFileFileName;

            textFileEnding = ".txt";
            char[] endingInCharTarget = Converter.ConvertEnding(textFileEnding);
            baseTargetsFile = parameters.TargetsFileName.TrimEnd(endingInCharTarget);

            fullTargetPath = parameters.TargetsFileFolder + "\\" + parameters.TargetsFileName;

            Console.WriteLine("The Targets file is: " + fullTargetPath);

            if (File.Exists(fullTargetPath))
            {
                Console.WriteLine("The Targets file exists. " + Environment.NewLine);
            }
            else
            {
                Console.WriteLine("The Targets file does not exist. " + Environment.NewLine);
            }

            Console.WriteLine("The Data File is: " + parameters.DataFileFolder + "\\" + parameters.DataFileFileName + "." +
                              parameters.DataFileEnding);

            if (File.Exists(parameters.DataFileFolder + "\\" + parameters.DataFileFileName + "." + parameters.DataFileEnding))
            {
                Console.WriteLine("The Data file exists. " + Environment.NewLine);
            }
            else
            {
                Console.WriteLine("The Data file does not exist. " + Environment.NewLine);
            }
            return parameters;
        }

        
    }

}
