using System;
using System.Collections.Generic;
using GetPeaksDllLite.DataFIFO;

namespace DivideTargetsLibrary.Parameters
{
    public class ParameterDivideTargets
    {
        public string TargetsFileName { get; set; }
        public string TargetsFileFolder { get; set; }
        public string DataFileFolder { get; set; }
        public string DataFileFileName { get; set; }
        public string DataFileEnding { get; set; }
        public string FactorsFileName { get; set; }
        public string CoresString { get; set; }
        public string AppIqGlyQConsoleLocationPath { get; set; }
        public string OutputLocationPath { get; set; }
        public string GlyQIQparameterFile { get; set; }
        public string LockController { get; set; }
        public string LockControllerDone { get; set; }
        

        public string WriteFolder{ get; set; }
        public string DeleteXYDataName{ get; set; }
        public string CopyResultBackName{ get; set; }
        public string PIC_DeleteFilesFileName { get; set; }
        public string DuplicateDataBool { get; set; }

        public void SetParameters(string divideTargetsParameterFile)
        {
            StringLoadTextFileLine loadSpectraL = new StringLoadTextFileLine();
            List<string> linesFromParameterFile = new List<string>();
            List<string> parameterList = new List<string>();
            //load strings
            linesFromParameterFile = loadSpectraL.SingleFileByLine(divideTargetsParameterFile); //loads all isos

            foreach (string line in linesFromParameterFile)
            {
                char spliter = ',';
                string[] wordArray = line.Split(spliter);
                if (wordArray.Length == 2)
                    parameterList.Add(wordArray[1]);
            }

            if (parameterList.Count != 17)
            {
                Console.WriteLine("Missing Parameters");
                Console.ReadKey();
            }

            TargetsFileName = parameterList[0];
            TargetsFileFolder = parameterList[1];
            DataFileFolder = parameterList[2];
            DataFileFileName = parameterList[3];
            DataFileEnding = parameterList[4];
            FactorsFileName = parameterList[5];
            CoresString = parameterList[6];
            AppIqGlyQConsoleLocationPath = parameterList[7];
            OutputLocationPath = parameterList[8];
            GlyQIQparameterFile = parameterList[9];
            LockController = parameterList[10];
            LockControllerDone = parameterList[11];
            

            //from main parameterfile
            WriteFolder = parameterList[12];
            DeleteXYDataName = parameterList[13];
            CopyResultBackName = parameterList[14];
            PIC_DeleteFilesFileName = parameterList[15];

            DuplicateDataBool = parameterList[16];
        }

        //public void WriteParameters(ParameterDivideTargets parameters, string outputPath)
        //{
        //    StringListToDisk writer = new StringListToDisk();
                
        //    List<string> data = new List<string>();
        //    data.Add("TargetsFile" + "," + parameters.TargetsFileName);
        //    data.Add("TargetsFolder" + "," + parameters.TargetsFileFolder);
        //    data.Add("DataFolderPath" + "," + parameters.DataFileFolder);
        //    data.Add("DataFile" + "," + parameters.DataFileFileName);
        //    data.Add("DataFileType" + "," + parameters.DataFileEnding);
        //    data.Add("FactorsFile" + "," + parameters.FactorsFileName);
        //    data.Add("Cores" + "," + parameters.CoresString);
        //    data.Add("ConsoleLocation" + "," + parameters.AppIqGlyQConsoleLocationPath);
        //    data.Add("ResultsLocation" + "," + parameters.OutputLocationPath);
        //    data.Add("GlyQIQParameters" + "," + parameters.IQparameterFile);
        //    data.Add("LockController" + "," + parameters.LockController);
        //    data.Add("LockControllerDone" + "," + parameters.LockControllerDone);
        //    data.Add("WriteFolder" + "," + parameters.WriteFolder);
        //    data.Add("DeleteXYDataName" + "," + parameters.DeleteXYDataName);
        //    data.Add("CopyResultBackName" + "," + parameters.CopyResultBackName);
        //    data.Add("PIC_DeleteFilesFileName" + "," + parameters.PIC_DeleteFilesFileName);
        //    data.Add("duplicateDataBool" + "," + parameters.duplicateDataBool);

        //    writer.toDiskStringList(outputPath, data);
        //}

        public void WriteParameters(string outputPath)
        {
            StringListToDisk writer = new StringListToDisk();

            List<string> data = new List<string>();
            data.Add("TargetsFile" + "," + TargetsFileName);
            data.Add("TargetsFolder" + "," + TargetsFileFolder);
            data.Add("DataFolderPath" + "," + DataFileFolder);
            data.Add("DataFile" + "," + DataFileFileName);
            data.Add("DataFileType" + "," + DataFileEnding);
            data.Add("FactorsFile" + "," + FactorsFileName);
            data.Add("Cores" + "," + CoresString);
            data.Add("ConsoleLocation" + "," + AppIqGlyQConsoleLocationPath);
            data.Add("ResultsLocation" + "," + OutputLocationPath);
            data.Add("GlyQIQParameters" + "," + GlyQIQparameterFile);
            data.Add("LockController" + "," + LockController);
            data.Add("LockControllerDone" + "," + LockControllerDone);
            data.Add("WriteFolder" + "," + WriteFolder);
            data.Add("DeleteXYDataName" + "," + DeleteXYDataName);
            data.Add("CopyResultBackName" + "," + CopyResultBackName);
            data.Add("PIC_DeleteFilesFileName" + "," + PIC_DeleteFilesFileName);
            data.Add("DuplicateDataBool" + "," + DuplicateDataBool);

            writer.toDiskStringList(outputPath, data);
        }
    }
}
