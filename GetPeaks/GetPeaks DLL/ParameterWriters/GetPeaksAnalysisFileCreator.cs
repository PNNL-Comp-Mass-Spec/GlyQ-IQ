using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.DataFIFO;

namespace GetPeaks_DLL.ParameterWriters
{
    public class GetPeaksAnalysisFileCreator
    {
        public int CreateFile(AnalysisParamertersTransferObject parametersToSet, string fileName, string folderID, string outputLocation)
        {
            int didThisWork = 0;

            List<string> whatToWrite = new List<string>();
            whatToWrite.Add("Library Data Folder," + parametersToSet.libraryDataFolder + ", folder with Library");
            whatToWrite.Add("Library Data Name," + parametersToSet.libraryDataName +", glycan library file");
            whatToWrite.Add("Data Folder," + parametersToSet.locationOfSQLFileToAnalyze + @", Folder with data. \ at end");
            whatToWrite.Add("Data Name," + parametersToSet.prefixOnSQLFile + ", data filename");
            whatToWrite.Add("Data Identifier," + folderID +", this is the unique part of the file name that will show up in result file names");
            whatToWrite.Add("Data File Seperator," + parametersToSet.dataFileSeperator +", seperates the name and identifier in data file");
            whatToWrite.Add("Data Type," + parametersToSet.DataTypeToAnalyze +", SQLite or Feature");
            whatToWrite.Add("Viper Results Path," + parametersToSet.ViperResultPath +", where to ouput the viper files");
            whatToWrite.Add("Output Results Path," + parametersToSet.outputResultsPath +", where the results files go");
            whatToWrite.Add("Output Vipir type files," + parametersToSet.outputVipir +", TRUE or FALSE");
            whatToWrite.Add("Glycans," + parametersToSet.analyzeGlycans +", TRUE or FALSE");
            whatToWrite.Add("Isotopes," + parametersToSet.outputIsotopes +", TRUE or FALSE");
            whatToWrite.Add("isotopeResultsPath," + parametersToSet.isotopesResultsPath +", folder the isotope file will end up");

            StringListToDisk writer = new StringListToDisk();
            writer.toDiskStringList(outputLocation, whatToWrite, "GetPeaksAnalysis,Version 1.0, Parameter File version");
            didThisWork = 1;
            return didThisWork;
        }
    }

    public class AnalysisParamertersTransferObject
    {
        public string libraryDataFolder { get; set; }

        public string locationOfSQLFileToAnalyze { get; set; }

        public string ViperResultPath { get; set; }

        public string outputResultsPath { get; set; }

        public string isotopesResultsPath { get; set; }
        
        public string prefixOnSQLFile { get; set; }

        public string dataFileSeperator { get; set; }

        public string DataTypeToAnalyze { get; set; }

        public string libraryDataName { get; set; }

        public bool outputVipir { get; set; }

        public bool analyzeGlycans { get; set; }

        public bool outputIsotopes { get; set; }

        public AnalysisParamertersTransferObject()
        {
            libraryDataFolder = @"E:\ScottK\XRSzAnalysisOutput\Libraries\";
            locationOfSQLFileToAnalyze = @"E:\ScottK\XRSGetPeaksSQLOutputResults\";
            ViperResultPath = @"E:\ScottK\XRSzAnalysisOutput\VIPER Output\";
            outputResultsPath = @"E:\ScottK\XRSzAnalysisOutput\GetPeaksOutAnalysis\";
            isotopesResultsPath = @":\ScottK\XRSzAnalysisOutput\IsotopeOutput\";
            
            prefixOnSQLFile = "SQLiteBatchResult";
            dataFileSeperator = "_";
            DataTypeToAnalyze = "SQLite";//SQLite or Feature
            libraryDataName = "L_LibraryDirectoryServerAlditol.txt";
            outputVipir = true;
            analyzeGlycans = true;
            outputIsotopes = true;
        }
    }
}
