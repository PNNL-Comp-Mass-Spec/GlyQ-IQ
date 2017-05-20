using System;
using System.Collections.Generic;
using PNNLOmics.Data;
using PNNLOmics.Data.Peaks;

namespace GetPeaksDllLite.DataFIFO
{
    public class DataXYDataWriter : IXYDataWriter
    {
        public int WriteOmicsXYData(List<PNNLOmics.Data.XYData> XYDataList, string path)
        {
            int isSuccess = 0;//nonsucessful outcome.  Still to be determined

            List<string> finalList = new List<string>();

            //part 1, set up column headders
            string columnHeader = "";
            List<string> columnNames = new List<string>();
            columnNames.Add("X");
            columnNames.Add("Y");
            for (int i = 0; i < columnNames.Count - 1; i++)
            {
                columnHeader += columnNames[i] + "\t";
            }

            //part 2, convert list to stringlist
            foreach (PNNLOmics.Data.XYData point in XYDataList)
            {
                //string oneLine = Convert.ToString(point.X) + "," + Convert.ToString(point.Y);
                string oneLine = Convert.ToString(point.X) + "\t" + Convert.ToString(point.Y);
                finalList.Add(oneLine);
            }

            //part 3, write to disk
            isSuccess = WriteHere(path, columnHeader, finalList);

            return isSuccess;
        }

        public int WriteOmicsProcesedPeakData(List<ProcessedPeak> peakDataList, string path)
        {
            int isSuccess = 0;//nonsucessful outcome.  Still to be determined

            List<string> finalList = new List<string>();

            //part 1, set up column headders
            string columnHeader = "";
            List<string> columnNames = new List<string>();
            columnNames.Add("X");
            columnNames.Add("Y");
            for (int i = 0; i < columnNames.Count - 1; i++)
            {
                columnHeader += columnNames[i] + "\t";
            }

            //part 2, convert list to stringlist
            List<PNNLOmics.Data.XYData> XYDataList = new List<PNNLOmics.Data.XYData>();
            foreach (ProcessedPeak peak in peakDataList)
            {
                XYDataList.Add(new PNNLOmics.Data.XYData(peak.XValue, peak.Height));
            }

            this.WriteOmicsXYData(XYDataList, path);

            return isSuccess;
        }

        public int WriteDeconXYDataDeconTools(Run32.Backend.Data.XYData XYData, string path)
        {
            int isSuccess = 0;//nonsucessful outcome.  Still to be determined

            if (XYData != null)
            {
                List<string> finalList = new List<string>();

                //part 1, set up column headders
                string columnHeader = "";
                List<string> columnNames = new List<string>();
                columnNames.Add("X");
                columnNames.Add("Y");
                for (int i = 0; i < columnNames.Count - 1; i++)
                {
                    columnHeader += columnNames[i] + "\t";
                }

                //part 2, convert list to stringlist
                for (int i = 0; i < XYData.Xvalues.Length; i++)
                {
                    string oneLine = Convert.ToString(XYData.Xvalues[i]) + "," + Convert.ToString(XYData.Yvalues[i]);
                    finalList.Add(oneLine);
                }

                //part 3, write to disk
                isSuccess = WriteHere(path, columnHeader, finalList);
            }
            else
            {
                isSuccess = 0;
            }
            return isSuccess;
        }

        private static int WriteHere(string path, string columnHeader, List<string> finalList)
        {
            int isSuccess;
            StringListToDisk newWriter = new StringListToDisk();

            string isotopeFile = path;
            //write features

            try
            {
                newWriter.toDiskStringList(isotopeFile, finalList, columnHeader);
            }
            catch
            {
                isSuccess = 0; //nonsucessful outcome.
            }

            isSuccess = 1; //sucessfull outcome
            return isSuccess;
        }
    }

    
}
