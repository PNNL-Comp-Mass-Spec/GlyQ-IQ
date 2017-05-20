using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.AnalysisSupport.Tools;

namespace GetPeaks_DLL.Objects.ParameterObjects
{
    public class WizardAnalyzeGetPeaksSQLParameters
    {
        private string LibraryPath { get; set; }
        private string LibraryFileName { get; set; }
        private string DataFilePath { get; set; }
        private string DataFileName { get; set; }
        private string DataFileIdentifier { get; set; }
        private string DataFileSeperator { get; set; }
        private string DataFileType { get; set; }
        private string ViperResultsPath { get; set; }
        private string folderToExportTextTo { get; set; }
        private string writeViper { get; set; }
        private string glycanAnalysis { get; set; }
        private string isotopeAnalysis { get; set; }
        private string isotopeResultsPath { get; set; }

        public bool ShouldWeExportViperData { get; set; }
        public bool ShouldWeAnalyzeGlycans { get; set; }
        public bool ShouldWeExportIsotopes { get; set; }

        public string ParameterFileVersion { get; set; }
        public string FullLibraryFile { get; set; }
        public string FullSQLDatabaseFile { get; set; }
        public string FullResultsTextFileOutputFile { get; set; }
        public string FullViperTextResultsFile { get; set; }
        public string FullIsotopeTextResultsFile { get; set; }

        public string JustTheSQLDatabasePath { get; set; }
        public string JustTheSQLDatabaseFileBaseName { get; set; }
        public string JustTheSQLDatabaseCompleteName { get; set; }
        public string JustTheViperOutputFolderPath { get; set; }
        public string JustTheIsotopeOutputFolderPath { get; set; }
        public string JustTheIdentifier { get; set; }
        public string JustTheFolderToExportTo { get; set; }

        public WizardAnalyzeGetPeaksSQLParameters(List<string> stringListFromParameterFile, string identifier)
        {
            if (stringListFromParameterFile.Count != 14)
            {
                Console.WriteLine("Parameter file is missing parameters");
                Console.Write("There are only " + stringListFromParameterFile.Count + " parameters");
                Console.ReadKey();
            }
            
            ParameterFileVersion = stringListFromParameterFile[0];
            LibraryPath = stringListFromParameterFile[1];
            LibraryFileName = stringListFromParameterFile[2];
            DataFilePath = stringListFromParameterFile[3];
            DataFileName = stringListFromParameterFile[4];
            DataFileIdentifier = stringListFromParameterFile[5];
            DataFileSeperator = stringListFromParameterFile[6];
            DataFileType = stringListFromParameterFile[7];
            ViperResultsPath = stringListFromParameterFile[8];
            folderToExportTextTo = stringListFromParameterFile[9];
            writeViper = stringListFromParameterFile[10];
            glycanAnalysis = stringListFromParameterFile[11];
            isotopeAnalysis = stringListFromParameterFile[12];
            isotopeResultsPath = stringListFromParameterFile[13];

            ShouldWeExportViperData = ConvertStringToBool.Convert(writeViper);
            ShouldWeAnalyzeGlycans = ConvertStringToBool.Convert(glycanAnalysis);
            ShouldWeExportIsotopes = ConvertStringToBool.Convert(isotopeAnalysis);

            //Console.WriteLine(Convert.ToString(ShouldWeExportViperData) + Convert.ToString(ShouldWeAnalyzeGlycans) + Convert.ToString(ShouldWeExportIsotopes));
            //Console.ReadKey();

            FullLibraryFile = LibraryPath + LibraryFileName;
            
            //sql data base input
            FullSQLDatabaseFile = DataFilePath + DataFileName + DataFileIdentifier + @".db";
            FullResultsTextFileOutputFile = folderToExportTextTo + "Results" + DataFileName + @".txt";
            JustTheSQLDatabasePath = DataFilePath;
            JustTheSQLDatabaseFileBaseName = DataFileName;
            JustTheSQLDatabaseCompleteName = DataFileName + DataFileIdentifier + @".db";

            //viper output
            FullViperTextResultsFile = ViperResultsPath + DataFileName + DataFileSeperator + DataFileIdentifier + @".txt";
            JustTheViperOutputFolderPath = ViperResultsPath;

            //isotope file output
            FullIsotopeTextResultsFile = isotopeResultsPath + DataFileName + DataFileSeperator + DataFileIdentifier + @"_Iso.txt";
            JustTheIsotopeOutputFolderPath = isotopeResultsPath;
            JustTheIdentifier = identifier;
            JustTheFolderToExportTo = folderToExportTextTo;
        }
    }
}
