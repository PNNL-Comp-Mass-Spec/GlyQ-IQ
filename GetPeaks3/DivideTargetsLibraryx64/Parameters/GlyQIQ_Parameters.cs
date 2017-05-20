using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using DivideTargetsLibraryX64.FromGetPeaks;

namespace DivideTargetsLibraryX64.Parameters
{
    public class GlyQIQ_Parameters
    {
        public string ResultsFolderPath { get; set; }
        public string LoggingFolderPath { get; set; }
        public string FactorsFile { get; set; }
        public string ExecutorParameterFile { get; set; }
        public string XYDataFolder { get; set; }
        public string WorkflowParametersFile { get; set; }
        public string Allignment { get; set; }
        public string BasicTargetedParameters { get; set; }

        public GlyQIQ_Parameters()
        {
            ResultsFolderPath = @"\\picfs\projects\DMS\PIC_HPC\Hot2\F_Local_V9_SPIN_SN138_16Dec13_C15_1\Results";
            LoggingFolderPath = @"\\picfs\projects\DMS\PIC_HPC\Hot2\F_Local_V9_SPIN_SN138_16Dec13_C15_1\Results";
            FactorsFile = "Factors_L10PSA.txt";
            ExecutorParameterFile = "ExecutorParametersSK.xml";
            XYDataFolder = "XYDataWriter";
            WorkflowParametersFile = "FragmentedParameters_Spin_HAVG_HolyCellC127.txt";
            Allignment = @"\\picfs\projects\DMS\PIC_HPC\Hot2\F_Local_V9_SPIN_SN138_16Dec13_C15_1\WorkingParameters\AlignmentParameters.xml";
            BasicTargetedParameters = @"\\picfs\projects\DMS\PIC_HPC\Hot2\F_Local_V9_SPIN_SN138_16Dec13_C15_1\WorkingParameters\BasicTargetedWorkflowParameters.xml";
        }

        public void LoadParameters(string parameterFileName)
        {
            int parameterLength = 8; //SET THIS

            StringLoadTextFileLine reader = new StringLoadTextFileLine();
            List<string> lines = reader.SingleFileByLine(parameterFileName);
            List<string> parameters = new List<string>();

            int parameterCount = 0;
            if (lines.Count == parameterLength)
            {
                foreach (var line in lines)
                {
                    string[] words = line.Split(',');
                    if (words.Length == 2)
                    {
                        parameterCount++;
                        parameters.Add(words[1]);
                    }
                }
            }

            if (parameterCount == parameterLength)
            {
                ResultsFolderPath = parameters[0];
                LoggingFolderPath = parameters[1];
                FactorsFile = parameters[2];
                ExecutorParameterFile = parameters[3];
                XYDataFolder = parameters[4];
                WorkflowParametersFile = parameters[5];
                Allignment = parameters[6];
                BasicTargetedParameters = parameters[7];

            }
            else
            {
                Console.WriteLine("Failed to load parameters in GlyQ-Console Parameters.  we loaded " + parameterCount +
                                  " out of " + parameterLength);
                Console.ReadKey();
            }
        }

        public void Write(string parameterFileName)
        {
            StringListToDisk writer = new StringListToDisk();
            List<string> data = new List<string>();
            data.Add("ResultsFolderPath" + "," + ResultsFolderPath);
            data.Add("LoggingFolderPath" + "," + LoggingFolderPath);
            data.Add("FactorsFile" + "," + FactorsFile);
            data.Add("ExecutorParameterFile" + "," + ExecutorParameterFile);
            data.Add("XYDataFolder" + "," + XYDataFolder);
            data.Add("WorkflowParametersFile" + "," + WorkflowParametersFile);
            data.Add("Allignment" + "," + Allignment);
            data.Add("BasicTargetedParameters" + "," + BasicTargetedParameters);
            
            writer.toDiskStringList(parameterFileName,data);
        }
        
    }
}
