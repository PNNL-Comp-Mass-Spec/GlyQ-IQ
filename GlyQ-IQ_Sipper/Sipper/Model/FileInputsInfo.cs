using System.Collections.Generic;
using System.IO;

namespace Sipper.Model
{
    public class FileInputsInfo
    {


        #region Constructors
        public FileInputsInfo()
        {
            DatasetPathsList = new List<string>();
        }


        public FileInputsInfo(string datasetPath, string parameterFilePath, string targetsFilePath)
            : this()
        {
            AddDatasetPath(datasetPath);
            DatasetPath = datasetPath;
            ParameterFilePath = parameterFilePath;
            TargetsFilePath = targetsFilePath;
        }
        #endregion

        #region Properties


        public string DatasetDirectory { get; set; }


        public IList<string> DatasetPathsList { get; set; }

        public string DatasetPath { get; set; }

        public string TargetsFilePath { get; set; }

        public string ParameterFilePath { get; set; }

        public string ResultImagesFolderPath { get; set; }

        public string ResultsSaveFilePath { get; set; }

        #endregion

        #region Public Methods
        public void AddDatasetPath(string fileOrFolderPath)
        {
            DatasetPathsList.Add(fileOrFolderPath);
        }


  

        #endregion

     
    }
}
