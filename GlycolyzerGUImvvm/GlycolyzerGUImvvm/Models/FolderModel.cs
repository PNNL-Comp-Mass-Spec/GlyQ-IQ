using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlycolyzerGUImvvm.Models
{
    public class FolderModel : ObservableObject
    {
        private String inputDataFolder_String = "";
        private List<String> inputDataFile_String = new List<string>();
        private String inputDataFileType_String = "Input Data File";
        private String inputDataPath_String = "";
        private String outputDataFolder_String = "";
        private String saveLocationFolder_String = "";
        private String saveLocationFile_String = "";
        private String saveAppParametersLocationFolder_String = "C:\\GlycolyzerApplicationFolder";
        private String previousPath_String = "";


        public String InputDataFolder_String
        {
            get { return inputDataFolder_String; }
            set { if (value != inputDataFolder_String) { inputDataFolder_String = value; OnPropertyChanged("inputDataFolder_String"); } }
        }

        public List<String> InputDataFile_String
        {
            get { return inputDataFile_String; }
            set { if (value != inputDataFile_String) { inputDataFile_String = value; OnPropertyChanged("inputDataFile_String"); } }
        }

        public String InputDataFileType_String
        {
            get { return inputDataFileType_String; }
            set { if (value != inputDataFileType_String) { inputDataFileType_String = value; OnPropertyChanged("inputDataFileType_String"); } }
        }

        public String InputDataPath_String
        {
            get { return inputDataPath_String; }
            set { if (value != inputDataPath_String) { inputDataPath_String = value; OnPropertyChanged("inputDataPath_String"); } }
        }

        public String OutputDataFolder_String
        {
            get { return outputDataFolder_String; }
            set { if (value != outputDataFolder_String) { outputDataFolder_String = value; OnPropertyChanged("outputDataFolder_String"); } }
        }

        public String SaveLocationFolder_String
        {
            get { return saveLocationFolder_String; }
            set { if (value != saveLocationFolder_String) { saveLocationFolder_String = value; OnPropertyChanged("saveLocationFolder_String"); } }
        }

        public String SaveLocationFile_String
        {
            get { return saveLocationFile_String; }
            set { if (value != saveLocationFile_String) { saveLocationFile_String = value; OnPropertyChanged("saveLocationFile_String"); } }
        }

        public String SaveAppParametersLocationFolder_String
        {
            get { return saveAppParametersLocationFolder_String; }
            set { if (value != saveAppParametersLocationFolder_String) { saveAppParametersLocationFolder_String = value; OnPropertyChanged("saveAppParametersLocationFolder_String"); } }
        }

        public String PreviousPath_String
        {
            get { return previousPath_String; }
            set { if (value != previousPath_String) { previousPath_String = value; OnPropertyChanged("previousPath_String"); } }
        }
    }
}
