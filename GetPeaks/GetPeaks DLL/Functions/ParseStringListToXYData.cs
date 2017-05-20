using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.DataFIFO;
using PNNLOmics.Data;

namespace GetPeaks_DLL.Functions
{
    public class ParseStringListToXYData
    {
        /// <summary>
        /// headders located on line 1
        /// </summary>
        /// <param name="stringList"></param>
        /// <param name="textDeliminator"></param>
        /// <param name="outputXYDataList"></param>
        /// <param name="columnHeadders"></param>
        public void Parse(List<string> stringList, FileIterator.deliminator textDeliminator, out List<XYData> outputXYDataList, out string columnHeadders)
        {
            int startline = 1;

            outputXYDataList = new List<XYData>();

            string[] wordArray;

            double x;
            double y;
            
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

            columnHeadders = "X" + spliter + "Y";

            int length = stringList.Count;
            for (int i = startline; i < length; i++)//i=0 is the headder
            {
                string line = stringList[i];

                wordArray = line.Split(spliter);

                //Tryparse is best and should be fastest
                double.TryParse(wordArray[0], out x);
                double.TryParse(wordArray[1], out y);

                XYData newXYData = new XYData(x, y);

                outputXYDataList.Add(newXYData);
            }
        }

        /// <summary>
        /// no headder
        /// </summary>
        /// <param name="stringList"></param>
        /// <param name="textDeliminator"></param>
        /// <param name="outputXYDataList"></param>
        public void Parse(List<string> stringList, FileIterator.deliminator textDeliminator, out List<XYData> outputXYDataList)
        {
            int startline = 0;

            outputXYDataList = new List<XYData>();

            string[] wordArray;

            double x;
            double y;

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

            //columnHeadders = "X" + spliter + "Y";

            int length = stringList.Count;
            for (int i = startline; i < length; i++)//i=0 is the headder
            {
                string line = stringList[i];

                wordArray = line.Split(spliter);

                //Tryparse is best and should be fastest
                double.TryParse(wordArray[0], out x);
                double.TryParse(wordArray[1], out y);

                XYData newXYData = new XYData(x, y);

                outputXYDataList.Add(newXYData);
            }
        }
    }
}
