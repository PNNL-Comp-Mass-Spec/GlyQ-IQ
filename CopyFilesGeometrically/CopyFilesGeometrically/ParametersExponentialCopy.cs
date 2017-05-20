using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.DataFIFO;

namespace CopyFilesGeometrically
{
    public class ParametersExponentialCopy
    {
        /// <summary>
        /// location of MultiSleep.exe
        /// </summary>
        public string MultiSleepExecutionPath { get; set; } 

        /// <summary>
        /// location of WriteCheckFile.exe
        /// </summary>
        public string CheckFileExecutionPath { get; set; }

        /// <summary>
        /// location of DeleteFiles.exe
        /// </summary>
        public string DeletefilesFileExecutionPath { get; set; }

        /// <summary>
        /// where to write all the files.  same folder as initial file
        /// </summary>
        public string WriteLocation { get; set; }

        /// <summary>
        /// file to copy without the extension
        /// </summary>
        public string DataFileNameWithoutEnding { get; set; }

        /// <summary>
        /// .text .raw etc
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// .text .raw etc
        /// </summary>
        public int NumberOfCopies { get; set; }

        public ParametersExponentialCopy()
        {
        }

        public void SetParameters(string parameterFileName)
        {
            List<string> linesFromParameterFile = new List<string>();
            StringLoadTextFileLine reader = new StringLoadTextFileLine();
            linesFromParameterFile = reader.SingleFileByLine(parameterFileName);

            List<string> parameterList = new List<string>();
            foreach (string line in linesFromParameterFile)
            {
                char spliter = ',';
                string[] wordArray = line.Split(spliter);
                if (wordArray.Length == 2)
                    parameterList.Add(wordArray[1]);
            }

            if (parameterList.Count != 7)
            {
                Console.WriteLine("Missing Parameters");
                Console.ReadKey();
            }


            MultiSleepExecutionPath = parameterList[0];
            CheckFileExecutionPath = parameterList[1];
            DeletefilesFileExecutionPath = parameterList[2];
            WriteLocation = parameterList[3];
            DataFileNameWithoutEnding = parameterList[4];
            FileExtension = parameterList[5];
            NumberOfCopies = Convert.ToInt32(parameterList[6]);
        }

    }
}
