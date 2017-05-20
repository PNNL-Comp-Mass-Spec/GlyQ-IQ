using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.AnalysisSupport.Tools;

namespace GetPeaks_DLL.Objects.ParameterObjects
{
    public class WizardAnalyzeGetPeaksFeature_IMSParameters
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

        public string FullLibraryFile { get; set; }
        public string FullFeaturesFile { get; set; }
        public string FullResultsTextFileOutputFile { get; set; }
        public string FullViperTextResultsFile { get; set; }
        public string FullIsotopeTextResultsFile { get; set; }

        public string JustTheFeatureFilePath { get; set; }
        public string JustTheFeatureFileBaseName { get; set; }
        public string JustTheFeatureFileCompleteName { get; set; }
        public string JustTheViperOutputFolderPath { get; set; }
        public string JustTheIsotopeOutputFolderPath { get; set; }

        public WizardAnalyzeGetPeaksFeature_IMSParameters(List<string> stringListFromParameterFile)
        {
            LibraryPath = stringListFromParameterFile[0];
            LibraryFileName = stringListFromParameterFile[1];
            DataFilePath = stringListFromParameterFile[2];
            DataFileName = stringListFromParameterFile[3];
            DataFileIdentifier = stringListFromParameterFile[4];
            DataFileSeperator = stringListFromParameterFile[5];
            DataFileType = stringListFromParameterFile[6];
            ViperResultsPath = stringListFromParameterFile[7];
            folderToExportTextTo = stringListFromParameterFile[8];
            writeViper = stringListFromParameterFile[9];
            glycanAnalysis = stringListFromParameterFile[10];
            isotopeAnalysis = stringListFromParameterFile[11];
            isotopeResultsPath = stringListFromParameterFile[12];

            ShouldWeExportViperData = ConvertStringToBool.Convert(writeViper);
            ShouldWeAnalyzeGlycans = ConvertStringToBool.Convert(glycanAnalysis);
            ShouldWeExportIsotopes = ConvertStringToBool.Convert(isotopeAnalysis);

            FullLibraryFile = LibraryPath + LibraryFileName;
            
            //sql data base input
            FullFeaturesFile = DataFilePath + DataFileName + DataFileSeperator+DataFileIdentifier + @"_LCMSFeatures.txt";
            FullResultsTextFileOutputFile = folderToExportTextTo + "Results" + DataFileName + DataFileSeperator + DataFileIdentifier + @".txt";
            JustTheFeatureFilePath = DataFilePath;
            JustTheFeatureFileBaseName = DataFileName;
            JustTheFeatureFileCompleteName = DataFileName + DataFileSeperator + DataFileIdentifier + @"_LCMSFeatures.txt";

            //viper output
            FullViperTextResultsFile = ViperResultsPath + DataFileName + DataFileSeperator + DataFileIdentifier + @".txt";
            JustTheViperOutputFolderPath = ViperResultsPath;

            //isotope file output
            FullIsotopeTextResultsFile = isotopeResultsPath + DataFileName + DataFileSeperator + DataFileIdentifier + @"_Iso.txt";
            JustTheIsotopeOutputFolderPath = isotopeResultsPath;
        }
    }
}
