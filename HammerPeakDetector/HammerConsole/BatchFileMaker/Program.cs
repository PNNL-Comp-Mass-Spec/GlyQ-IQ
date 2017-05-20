using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatchFileMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> dataFilesNoEnding = new List<string>();



            dataFilesNoEnding.Add("C14_DB02_30Dec12_1");
            dataFilesNoEnding.Add("C14_DB04_30Dec12_1");
            dataFilesNoEnding.Add("C14_DB06_31Dec12_1");
            dataFilesNoEnding.Add("C14_DB08_31Dec12_1");
            dataFilesNoEnding.Add("C14_DB10_31Dec12_1");
            dataFilesNoEnding.Add("C14_DB12_01Jan13_1");
            dataFilesNoEnding.Add("C14_DB14_01Jan13_1");
            dataFilesNoEnding.Add("C14_DB16_01Jan13_1");
            dataFilesNoEnding.Add("C14_DB18_01Jan13_1");
            dataFilesNoEnding.Add("C14_DB20_01Jan13_1");


            dataFilesNoEnding.Add("C15_DB01_30Dec12_1");
            dataFilesNoEnding.Add("C15_DB03_30Dec12_1");
            dataFilesNoEnding.Add("C15_DB05_30Dec12_1");
            dataFilesNoEnding.Add("C15_DB07_31Dec12_1");
            dataFilesNoEnding.Add("C15_DB09_31Dec12_1");
            dataFilesNoEnding.Add("C15_DB11_01Jan13_1");
            dataFilesNoEnding.Add("C15_DB13_01Jan13_1");
            dataFilesNoEnding.Add("C15_DB15_01Jan13_1");
            dataFilesNoEnding.Add("C15_DB17_01Jan13_1");
            dataFilesNoEnding.Add("C15_DB19_01Jan13_1");




            string folderToWriteTo = @"E:\PNNL_Data\2012_12_24_Velos_3\DiabetesStudy";

            string datafileFolder = @"E:\PNNL_Data\2012_12_24_Velos_3\DiabetesStudy";
            string dataFileEnding = ".raw";
            //string applicationDirectory = @"R:\RAM Files\GetPeaks\HammerPeakDetectorConsole\bin\Release";
            string applicationDirectory = @"E:\PNNL_Data\2012_12_24_Velos_3\Release";
            StringListToDisk writer = new StringListToDisk();
            List<string> batchfileNames = new List<string>();
            string q = @"""";

            foreach (string file in dataFilesNoEnding)
            {
                string batchFileName = folderToWriteTo + @"\" + "Hammer_" + file + ".bat";
                batchfileNames.Add("Start " + batchFileName);

                List<string> linesToWrite = new List<string>();
                string line1 = q + applicationDirectory + @"\" + "HammerPeakDetectorConsole.exe" + q;
                string line2 = q + datafileFolder + @"\" + file + dataFileEnding + q;
                string line3 = "HammerParameterFile_" + file;
                string line4 = q + folderToWriteTo + @"\" + file + "_peaks.txt" + q;
                string line5 = q + folderToWriteTo + q;
                linesToWrite.Add(line1 + " " + line2 + " " + line3 + " " + line4 + " " + line5);
                writer.toDiskStringList(batchFileName,linesToWrite);
            }

            string runAllBatchFileName = folderToWriteTo + @"\" + "HammerAll.bat";
            writer.toDiskStringList(runAllBatchFileName, batchfileNames);
        }
    }
}
