using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.DataFIFO;

namespace GetPeaks_DLL.Functions
{
    public class ParseScanFileToInt
    {
        
        public void Parse(List<string> stringList, FileIterator.deliminator textDeliminator, out List<int> outputIntList, out string columnHeadders)
        {
            int startline = 2;//0 and 1 are the headder

            outputIntList = new List<int>();

            string[] wordArray;
            
            int scan;
            
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

            columnHeadders = "Scan";

            int length = stringList.Count;
            for (int i = startline; i < length; i++)//i=0 is the headder
            {
                string line = stringList[i];

                wordArray = line.Split(spliter);

                //Tryparse is best and should be fastest
                int.TryParse(wordArray[1], out scan);
                
                
                outputIntList.Add(scan);
            }
        }
    }

}
