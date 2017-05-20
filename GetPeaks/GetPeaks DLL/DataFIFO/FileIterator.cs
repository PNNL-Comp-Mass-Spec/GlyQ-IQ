using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using PNNLOmics.Data;

namespace GetPeaks_DLL.DataFIFO
{
    public class FileIterator
    {
        public List<DataSet> IterateFiles(List<string> fileList, deliminator deliminatorFiletype)
		
        {
            Console.WriteLine("Iterate Now\n");

            List<DataSet> MZDataProject = new List<DataSet>();

            //how many files to load
            int length=fileList.Count;

            //Instantiate classes so they are not inside the loop;
            StringLoadTextFileLine loadSpectraL = null;
            
            for (int i = 0; i < length; i += 2)
            {
				Console.WriteLine("Load file: " + fileList[i] + "\n");
                DataSet newDataSet = new DataSet();
                MZDataProject.Add(newDataSet);

                loadSpectraL = new StringLoadTextFileLine();
                List<string> stringListFromFiles = new List<string>();

                #region load data and parse it

                //load XYData
                stringListFromFiles = loadSpectraL.SingleFileByLine(fileList[i]);//loads XYData

                //parse XYData
                List<XYData> newXYDataList = new List<XYData>();
                newXYDataList = this.ParseToXYData(stringListFromFiles, deliminatorFiletype);

                //load fileInfo
                stringListFromFiles = loadSpectraL.SingleFileByLine(fileList[i+1]);//loads info
                //parse fileInfo
                List<string> newXYDataListInfo = new List<string>();
                newXYDataListInfo = this.ParseToStringXword(stringListFromFiles, deliminator.Comma, 1);

                #endregion

                MZDataProject[i].XYList = newXYDataList;// AddSpectra
                MZDataProject[i].DataSetInfo = newXYDataListInfo; //add file info
                MZDataProject[i].Name = fileList[i];//add name
			}
            return MZDataProject;
        }

        #region private Methods
        private List<XYData> ParseToXYData(List<string> stringList, deliminator textDeliminator)
        {
            List<XYData> outputList = new List<XYData>();

            string[] wordArray;

            double xValue = 0;
            double yValue = 0;

            char spliter;
            switch (textDeliminator)
            {
                case GetPeaks_DLL.DataFIFO.FileIterator.deliminator.Comma:
                    {
                        spliter = ',';
                    }
                    break;
                case GetPeaks_DLL.DataFIFO.FileIterator.deliminator.Tab:
                    {
                        spliter = '\t';
                    }
                    break;
                default:
                    {
                        spliter = ',';
                    }
                    break;
            }

            foreach (string line in stringList)
            {
                wordArray = line.Split(spliter);
                
                //Tryparse is best and should be fastest
                double.TryParse(wordArray[0], out xValue);
                double.TryParse(wordArray[1], out yValue);

                XYData newDataPoint = new XYData(xValue, yValue);
                outputList.Add(newDataPoint);
            }
            return outputList;
        }

        private List<string> ParseToStringXword(List<string> stringList, deliminator textDeliminator, int selectedWord)
        {
            List<string> outputList = new List<string>();

            string[] wordArray;

            string word = "";

            char spliter;
            switch (textDeliminator)
            {
                case GetPeaks_DLL.DataFIFO.FileIterator.deliminator.Comma:
                    {
                        spliter = ',';
                    }
                    break;
                case GetPeaks_DLL.DataFIFO.FileIterator.deliminator.Tab:
                    {
                        spliter = '\t';
                    }
                    break;
                default:
                    {
                        spliter = ',';
                    }
                    break;
            }

            foreach (string line in stringList)
            {
                wordArray = line.Split(spliter);

                word = wordArray[selectedWord];
                outputList.Add(word);
            }
            return outputList;
        }

        #endregion

        public enum deliminator
        {
            Comma,
            Tab
        }
    }

    
}
