using System;
using System.Collections.Generic;
using GetPeaksDllLite.DataFIFO;

namespace IQGlyQ_Console_ParameterSetup
{
    class IQGlyQSetupParameters
    {
        public string Targets { get; set; }
        public string ProcessingParameters { get; set; }
        public string DivideTargetsParameters { get; set; }
        public string DivideTargetsParametersFolder { get; set; }
        public string DataSpecificParameters { get; set; }
        public string FactorsForTargets { get; set; }
        public string MasterLocation { get; set; }
        public string WorkerLocation { get; set; }
        public string MasterComputerName { get; set; }
        //public string DataFileLocation { get; set; }
        //public string DataFileBaseName { get; set; }
        //public string DataFileEnding { get; set; }

        public IQGlyQSetupParameters()
        {
        }

        public void SetParameters(string parameterFileName)
        {
        
            
            StringLoadTextFileLine loadSpectraL = new StringLoadTextFileLine();
            List<string> linesFromParameterFile = new List<string>();
            List<string> parameterList = new List<string>();
            //load strings
            linesFromParameterFile = loadSpectraL.SingleFileByLine(parameterFileName); //loads all isos

            foreach (string line in linesFromParameterFile)
            {
                char spliter = ',';
                string[] wordArray = line.Split(spliter);
                if (wordArray.Length == 2)
                    parameterList.Add(wordArray[1]);
            }

            if (parameterList.Count !=9)
            {
                Console.WriteLine("Missing Parameters");
                Console.ReadKey();
            }

            Targets = parameterList[0];
            ProcessingParameters = parameterList[1];
            DivideTargetsParameters = parameterList[2];
            DivideTargetsParametersFolder = parameterList[3];
            DataSpecificParameters = parameterList[4];
            FactorsForTargets = parameterList[5];
            MasterLocation = parameterList[6];
            WorkerLocation = parameterList[7];
            MasterComputerName = parameterList[8];
            //DataFileLocation = parameterList[9];
            //DataFileBaseName = parameterList[10];
            //DataFileEnding = parameterList[11];

            //DataLocation,E:\ScottK\Shared_PICFS\ToPIC\RawData
            //DataCoreName,Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12
            //DataExtension,.raw
        }
    }
}
