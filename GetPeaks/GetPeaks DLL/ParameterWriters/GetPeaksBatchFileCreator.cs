using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.DataFIFO;

namespace GetPeaks_DLL.ParameterWriters
{
    public class GetPeaksBatchFileCreator
    {
        public int CreateFile(BatchFileTransferObject parametersToSet,bool shouldWeWriteGetPeaks,bool shouldWeWriteAnalysis,  string folderID, string outputLocation)
        {
            int didThisWork = 0;

            List<string> whatToWrite = new List<string>();

            if (shouldWeWriteGetPeaks)
            {
                string lineA1 = @"""" + parametersToSet.FolderWithGetPeaks + @"""" + " ";
                string lineA2 = @"""" + parametersToSet.FolderWithParameters + "Parameter" + folderID + "_GetPeak.txt" + @"""" + " ";
                string lineA3 = @"""" + parametersToSet.FolderForTextFiles + parametersToSet.PrefixGetPeakstextOutput + @"""" + " ";
                string lineA4 = @"""" + parametersToSet.FolderForSQLFiles + @"""";

                whatToWrite.Add(lineA1 + lineA2 + lineA3 + lineA4);
                
            }

            if (shouldWeWriteAnalysis)
            {
                string lineB1 = @"""" + parametersToSet.FolderWithAnalysis + @"""" + " ";
                string lineB2 = @"""" + parametersToSet.FolderWithParameters + "Parameter" + folderID + "_Analysis.txt" + @"""" + " ";
                string lineB3 = @"""" + parametersToSet.FolderForTextFiles + parametersToSet.PrefixAnalysistextOutput + @"""" + " ";
                string lineB4 = @"""" + parametersToSet.FolderForViperTextFiles + parametersToSet.ViperPrefix+ @"""" + " ";
                string lineB5 = @"""" + folderID + @"""";
                //"D:\Csharp\ConosleApps\zGetPeaksOutAnalysis\GetPeaksOutAnalysis"
                //"D:\Csharp\ConosleApps\zVIPER Output\VIPER Output"
                //"SN343536_150_5uL_S3"
                whatToWrite.Add(lineB1 + lineB2 + lineB3 + lineB4 + lineB5);

            }
            whatToWrite.Add("pause");
            StringListToDisk writer = new StringListToDisk();
            writer.toDiskStringList(outputLocation, whatToWrite, "pause");
            didThisWork = 1;
            return didThisWork;
        }
    }

    public class BatchFileTransferObject
    {
        public string FolderWithGetPeaks { get; set; }

        public string FolderWithAnalysis { get; set; }

        public string FolderWithParameters { get; set; }

        public string FolderForTextFiles { get; set; }

        public string FolderForViperTextFiles { get; set; }

        public string FolderForSQLFiles { get; set; }

        public string ViperPrefix { get; set; }

        public string SQLPrefix { get; set; }

        public string PrefixGetPeakstextOutput {get;set;}

        public string PrefixAnalysistextOutput {get;set;}
        

        public BatchFileTransferObject()
        {
            FolderWithGetPeaks = @"D:\Csharp\ConosleApps\LocalServer\XRSGetPeaksServerProgram\GetPeaks 4.0.exe";
            FolderWithAnalysis = @"D:\Csharp\ConosleApps\LocalServer\XRSAnalysisProgram\WizardAnalyzeGetPeaksSQL";
            FolderWithParameters = @"D:\Csharp\ConosleApps\LocalServer\";
            FolderForTextFiles = @"D:\Csharp\ConosleApps\LocalServer\";
            FolderForViperTextFiles = @"D:\Csharp\ConosleApps\zVIPER Output";
            PrefixGetPeakstextOutput = "GetPeaksResult";
            PrefixAnalysistextOutput = "AnalysisResult";
            SQLPrefix = "SQLiteBatchResult";
            ViperPrefix = "Viper";
        }
    }
}
