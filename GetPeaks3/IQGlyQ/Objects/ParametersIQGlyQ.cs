using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaksDllLite.DataFIFO;

//using GetPeaks_DLL.DataFIFO;

namespace IQGlyQ.Objects
{
    public class ParametersIQGlyQ
    {
        public string dataFolderPath { get; set; }
        public string fileFolderPath { get; set; }
        public string resultsFolderPath { get; set; }
        //public string peaksFolderPath { get; set; }
        public string loggingFolderPath { get; set; }
        public string testFile { get; set; }
        public string targetsFile { get; set; }
        public string factorsFile { get; set; }
        public string executorParameterFile { get; set; }
        //public string peaksTestFile { get; set; }
        public string XYDataFolder { get; set; }
        public string datasetEnding { get; set; }
        public string fragmentWorkFlowParameters { get; set; }
        public string TargetedAlignmentWorkflowParameters { get; set; }
        public string BasicTargetedWorkflowParameters { get; set; }
        public string LocksFile { get; set; }
        
        public List<string> ParseArgsIQ(string[] args)
        {
            List<string> paramatersStrings = new List<string>();
            for (int i = 0; i < args.Length; i++)
            {
                paramatersStrings.Add("");
            }

            string[] words = { };
            string argument1 = args[0];
            Console.WriteLine(args[1]);
            words = argument1.Split(Environment.NewLine.ToCharArray(),  //'\n', '\r'
                StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < args.Length; i++)
            {
                paramatersStrings[i] = args[i];
            }

            return paramatersStrings;
        }

        public void SetParameters(string glyQIqParameterFile)
        {
            StringLoadTextFileLine loadSpectraL = new StringLoadTextFileLine();
            List<string> linesFromParameterFile = new List<string>();
            List<string> parameterList = new List<string>();
            //load strings
            Console.WriteLine("Loading Parameters from: " + glyQIqParameterFile);
            linesFromParameterFile = loadSpectraL.SingleFileByLine(glyQIqParameterFile); //loads all isos

            foreach (string line in linesFromParameterFile)
            {
                char spliter = ',';
                string[] wordArray = line.Split(spliter);
                if (wordArray.Length == 2)
                    parameterList.Add(wordArray[1]);
            }


            if (parameterList.Count != 8)
            {
                Console.WriteLine("Missing Parameters in ParametersIQGlyQ, File name " + glyQIqParameterFile + " should have 8 parameters");
                Console.ReadKey();
            }

            resultsFolderPath = parameterList[0];
            //peaksFolderPath = parameterList[1];
            loggingFolderPath = parameterList[1];
            factorsFile = parameterList[2];
            executorParameterFile = parameterList[3];
            //peaksTestFile = parameterList[5];
            XYDataFolder = parameterList[4];
            fragmentWorkFlowParameters = parameterList[5];
            TargetedAlignmentWorkflowParameters = parameterList[6];
            BasicTargetedWorkflowParameters = parameterList[7];
        }

        public void WriteParameters(string parameterFileName)
        {
            StringListToDisk writer = new StringListToDisk();
            
            List<string> data = new List<string>();
            data.Add("resultsFolderPath" + "," + resultsFolderPath);
            data.Add("loggingFolderPath" + "," + loggingFolderPath);
            data.Add("factorsFile" + "," + factorsFile);
            data.Add("executorParameterFile" + "," + executorParameterFile);
            data.Add("XYDataFolder" + "," + XYDataFolder);
            data.Add("WorkflowParametersFile" + "," + fragmentWorkFlowParameters);
            data.Add("TargetedAlignmentWorkflowParameters" + "," + TargetedAlignmentWorkflowParameters);
            data.Add("BasicTargetedWorkflowParameters" + "," + BasicTargetedWorkflowParameters);

            writer.toDiskStringList(parameterFileName,data);
        }
    }
}
