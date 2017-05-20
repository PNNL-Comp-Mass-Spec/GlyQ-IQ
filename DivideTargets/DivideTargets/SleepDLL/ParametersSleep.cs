using System;
using System.Collections.Generic;

namespace SleepDLL
{
    public class ParametersSleep
    {
        public string FileToWaitFor { get; set; }

        public string BatchFileToRunAfterLoop { get; set; }

        public string WorkingFolderPath { get; set; }

        public string TimeString { get; set; }

        public int TimeInt { get; set; }

        public ParametersSleep()
        {
        }

        public void SetParameters(string parameterFile)
        {
            

            StringLoadTextFileLine loadSpectraL = new StringLoadTextFileLine();
            List<string> linesFromParameterFile = new List<string>();
            List<string> parameterList = new List<string>();
            //load strings
            linesFromParameterFile = loadSpectraL.SingleFileByLine(parameterFile); //loads all isos

            foreach (string line in linesFromParameterFile)
            {
                char spliter = ',';
                string[] wordArray = line.Split(spliter);
                if (wordArray.Length == 2)
                    parameterList.Add(wordArray[1]);
            }

            if (parameterList.Count != 4)
            {
                Console.WriteLine("Missing Parameters");
                Console.ReadKey();
            }


            FileToWaitFor = parameterList[0];
            BatchFileToRunAfterLoop = parameterList[1];
            WorkingFolderPath = parameterList[2];
            TimeString = parameterList[3];
            TimeInt = Convert.ToInt32(TimeString);
        }
    }
}
