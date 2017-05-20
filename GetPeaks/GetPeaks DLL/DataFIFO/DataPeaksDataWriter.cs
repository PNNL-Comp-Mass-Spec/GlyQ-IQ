using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

namespace GetPeaks_DLL.DataFIFO.OmicsFIFO
{
    public interface IPeakWriter
    {
        int WriteOmicsPeakData(List<PNNLOmics.Data.Peak> DataList, int scanNumber, string path);
        int WriteOmicsProcessedPeakData(List<PNNLOmics.Data.ProcessedPeak> DataList, int scanNumber, string path);
    }

    public class DataPeaksDataWriter : IPeakWriter
    {
        public int WriteOmicsPeakData(List<PNNLOmics.Data.Peak> XYDataList, int scanNumber, string path)
        {
            int isSuccess = 0;//nonsucessful outcome.  Still to be determined

            //part 1, set up column headders
            string seperator = "\t"; //or ","
            string columnHeader = ColumnHeader(seperator);

            //part 2 set up strings from peaks
            List<string> finalList = ConvertPeaksTOStingsPeak(XYDataList, scanNumber, seperator);

            //part 3, write to disk
            isSuccess = WriteHere(path, columnHeader, finalList);

            return isSuccess;
        }

        public int WriteOmicsProcessedPeakData(List<PNNLOmics.Data.ProcessedPeak> XYDataList, int scanNumber, string path)
        {
            int isSuccess = 0;//nonsucessful outcome.  Still to be determined

            //part 1, set up column headders
            string seperator = "\t"; //or ","
            string columnHeader = ColumnHeader(seperator);

            //part 2 set up strings from peaks
            List<string> finalList = ConvertPeaksTOStingsPeak(XYDataList, scanNumber, seperator);

            //part 3, write to disk
            isSuccess = WriteHere(path, columnHeader, finalList);

            return isSuccess;
        }

        public static List<string> ConvertPeaksTOStingsPeak(List<Peak> XYDataList, int scanNumber, string seperator)
        {
            List<string> finalList = new List<string>();

            int counter = 1;
            foreach (PNNLOmics.Data.Peak point in XYDataList)
            {
                string mass = SetPrecision(point.XValue, 5);
                string height = SetPrecision(point.Height, 4);
                string width = SetPrecision(point.Width, 6);

                string oneLine =
                    counter + seperator +
                    scanNumber + seperator +
                    mass + seperator +
                    height + seperator +
                    width + seperator +
                    Convert.ToString(point.LocalSignalToNoise) + seperator +
                    0; //for feature iD

                counter++;
                finalList.Add(oneLine);
            }
            return finalList;
        }

        public static List<string> ConvertPeaksTOStingsPeak(List<PNNLOmics.Data.ProcessedPeak> XYDataList, int scanNumber, string seperator)
        {
            List<string> finalList = new List<string>();

            int counter = 1;
            foreach (PNNLOmics.Data.ProcessedPeak point in XYDataList)
            {
                string mass = SetPrecision(point.XValue, 5);
                string height = SetPrecision(point.Height, 4);
                string width = SetPrecision(point.Width, 6);

                string oneLine =
                    counter + seperator +
                    scanNumber + seperator +
                    mass + seperator +
                    height + seperator +
                    width + seperator +
                    Convert.ToString(point.LocalSignalToNoise) + seperator +
                    0;//for feature iD

                counter++;
                finalList.Add(oneLine);
            }
            return finalList;
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

        private static string ColumnHeader(string seperator)
        {
            List<string> columnNames = new List<string>();
            columnNames.Add("peak_index");
            columnNames.Add("scan_num");
            columnNames.Add("mz");
            columnNames.Add("intensity");
            columnNames.Add("fwhm");
            columnNames.Add("signal_noise");
            columnNames.Add("MSFeatureID");


            string columnHeader = "";
            for (int i = 0; i < columnNames.Count - 1; i++)
            {
                columnHeader += columnNames[i] + seperator;
                //columnHeader += columnNames[i] + "\t";
            }
            columnHeader += columnNames[columnNames.Count - 1];
            return columnHeader;
        }

        private static string SetPrecision(double massin, int massPrecision)
        {
            double massDouble = Math.Round(massin, massPrecision);
            double massDoubleInt = Math.Truncate(massDouble);
            double massDecimal = Math.Round(massDouble - massDoubleInt, massPrecision);
            string mass = Convert.ToString(massDouble);
            string numberOfDecimals = Convert.ToString(massDecimal);
            while (numberOfDecimals.Length - 2 < massPrecision) //-2 for "0."
            {
                numberOfDecimals += "0";
                mass += "0";
            }
            return mass;
        }
    }
}
