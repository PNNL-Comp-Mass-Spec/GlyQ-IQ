using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilesAndFolders
{
    public static class UtilitiesForGet
    {
        public static void SetDataInput(DataSetGroup currentGroup, out List<string> currentFileNamesShort, out List<string> currentFileNames)
        {
            switch (currentGroup)
            {
                case DataSetGroup.Best3MixBeforeGilson:
                    {
                        currentFileNamesShort = GetInfo.DatasetID3_MixedBeforeGilson();
                        currentFileNames = GetInfo.DatasetDiabetesFiles3_MixedBeforeGilson();
                    }
                    break;
                case DataSetGroup.Best3MixBeforeMS:
                    {
                        currentFileNamesShort = GetInfo.DatasetID3_MixedBeforeMS();
                        currentFileNames = GetInfo.DatasetDiabetesFiles3_MixedBeforeMS();
                    }
                    break;
                case DataSetGroup.Control6:
                    {
                        currentFileNamesShort = GetInfo.DatasetID6();
                        currentFileNames = GetInfo.DatasetDiabetesFiles6();
                    }
                    break;
                case DataSetGroup.Data20:
                    {
                        currentFileNamesShort = GetInfo.DatasetID20();
                        currentFileNames = GetInfo.DatasetDiabetesFiles20();
                    }
                    break;
                case DataSetGroup.All26:
                    {
                        currentFileNamesShort = GetInfo.DatasetID26();
                        currentFileNames = GetInfo.DatasetDiabetesFiles26();
                    }
                    break;
                default:
                    {
                        currentFileNamesShort = new List<string>();
                        currentFileNames = new List<string>();
                        Console.WriteLine("mising case");
                        Console.ReadKey();
                    }
                    break;
            }
        }

        public static void SetHeader(List<string> datasetWithCode, List<string> codesToPull, string seperatorString)
        {
            string codeHeaderStart = "Codes";
            List<string> codeHeaderAsList = new List<string>();
            codeHeaderAsList.Add(codeHeaderStart);
            codeHeaderAsList.AddRange(codesToPull);
            codeHeaderAsList.Add("HitsCount");
            datasetWithCode.Add(String.Join(seperatorString, codeHeaderAsList.ToArray())); //add header
        }

        public enum DataSetGroup
        {
            Best3MixBeforeGilson,
            Best3MixBeforeMS,
            Control6,
            Data20,
            All26
        }
    }
}
