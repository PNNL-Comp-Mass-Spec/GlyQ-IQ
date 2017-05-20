using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using GetPeaks_DLL.DataFIFO;

namespace GetPeaks_DLL.Functions
{
    public class ConvertXyDataToString
    {
        public void Convert(List<XYData> XYDataList, FileIterator.deliminator textDeliminator, out List<string> exportList)
        {
            exportList = new List<string>();

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

            foreach (XYData data in XYDataList)
            {
                exportList.Add(data.X.ToString() + spliter + data.Y.ToString());
            }
        }
    }
}
