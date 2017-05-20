using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.Functions;

namespace GetPeaks_DLL.Summing
{
    static class CacheXYData
    {
        public static void CacheData(Run run, List<int> scanIndexList, out List<XYDataAndPeakHolderObject> DataToSum)
        {
            DataToSum = new List<XYDataAndPeakHolderObject>();//this is needed incase scanindex=0

            foreach (int scan in scanIndexList)//create a single scan set so we can pull the data one by one and resum it
            {
                int[] intexArray = new int[1];//place holder for zero default summing
                intexArray[0] = scan;
                ScanSet scanSet = new ScanSet(scan, intexArray);
                run.CurrentScanSet = scanSet;
                GetXYData.GetData(run);//needs one or more scans in a scan set
              
                //allign scan here

                //1.  store first scan
                XYDataAndPeakHolderObject currentSpectra = new XYDataAndPeakHolderObject();
                currentSpectra.SpectraDataDECON.Yvalues = run.XYData.Yvalues;
                List<PNNLOmics.Data.XYData> omicsList;
                ConvertXYData.RunXYDataToOmicsXYData(run, out omicsList);
                currentSpectra.SpectraDataOMICS = omicsList;

                DataToSum.Add(currentSpectra);

                bool print = false;
                if (print)
                {
                    #region print XYdata to disk
                    if (scan >= 2591 && scanIndexList.Count > 8)
                    {
                        IXYDataWriter newXYWriter = new DataXYDataWriter();
                        string path = @"V:\XYOut" + scan.ToString() + ".txt";

                        //1.  extract XYData
                        List<PNNLOmics.Data.XYData> extractedXYData;
                        ConvertXYData.RunXYDataToOmicsXYData(run, out extractedXYData);

                        int t = newXYWriter.WriteOmicsXYData(extractedXYData, path);
                    }
                    #endregion
                }
            }
        }

        public static void CacheDataInt64(Run run, List<int> scanIndexList, double bitShiftforRounding, out List<XYDataAndPeakHolderObjectInt64> DataToSum)
        {
            DataToSum = new List<XYDataAndPeakHolderObjectInt64>();//this is needed incase scanindex=0

            foreach (int scan in scanIndexList)//create a single scan set so we can pull the data one by one and resum it
            {
                int[] intexArray = new int[1];//place holder for zero default summing
                intexArray[0] = scan;
                ScanSet scanSet = new ScanSet(scan, intexArray);
                run.CurrentScanSet = scanSet;
                GetXYData.GetData(run);//needs one or more scans in a scan set

                //allign scan here

                //1.  store first scan
                int size = run.XYData.Xvalues.Length;
                XYDataAndPeakHolderObjectInt64 currentSpectra = new XYDataAndPeakHolderObjectInt64(size);
                currentSpectra.PeakListY = run.XYData.Yvalues;
                currentSpectra.PeakListXdouble = run.XYData.Xvalues;

                for(int i=0;i<size;i++)
                {
                    currentSpectra.PeakListX64[i] = Convert.ToInt64(run.XYData.Xvalues[i] * bitShiftforRounding);
                }
                
                //List<PNNLOmics.Data.XYData> omicsList;
                //ConvertXYData.RunXYDataToOmicsXYData(run, out omicsList);
                //currentSpectra.SpectraDataOMICS = omicsList;

                DataToSum.Add(currentSpectra);

                bool print = false;
                if (print)
                {
                    #region print XYdata to disk
                    if (scan >= 2591 && scanIndexList.Count > 8)
                    {
                        IXYDataWriter newXYWriter = new DataXYDataWriter();
                        string path = @"V:\XYOut" + scan.ToString() + ".txt";

                        //1.  extract XYData
                        List<PNNLOmics.Data.XYData> extractedXYData;
                        ConvertXYData.RunXYDataToOmicsXYData(run, out extractedXYData);

                        int t = newXYWriter.WriteOmicsXYData(extractedXYData, path);
                    }
                    #endregion
                }
            }
        }

        public static void CacheDataLong(Run run, List<int> scanIndexList, out List<XYDataAndPeakHolderObject> DataToSum)
        {
            DataToSum = new List<XYDataAndPeakHolderObject>();//this is needed incase scanindex=0

            for(int i=0;i<scanIndexList.Count;i++)//create a single scan set so we can pull the data one by one and resum it
            {
                int[] intexArray = new int[1];//place holder for zero default summing
                intexArray[0] = scanIndexList[i];
                ScanSet scanSet = new ScanSet(scanIndexList[i], intexArray);
                run.CurrentScanSet = scanSet;
                GetXYData.GetData(run);//needs one or more scans in a scan set

                //allign scan here

                //1.  store first scan

                XYDataAndPeakHolderObject currentSpectra = new XYDataAndPeakHolderObject();
                List<PNNLOmics.Data.XYData> omicsList;
                ConvertXYData.RunXYDataToOmicsXYData(run, out omicsList);
                currentSpectra.SpectraDataOMICS = omicsList;
                

                if (i == 0)
                {
                    DataToSum.Add(currentSpectra);//first scan

                    XYDataAndPeakHolderObject currentSpectra2 = new XYDataAndPeakHolderObject();
                    List<PNNLOmics.Data.XYData> omicsList2;
                    ConvertXYData.RunXYDataToOmicsXYData(run, out omicsList2);
                    currentSpectra2.SpectraDataOMICS = omicsList2;
                    DataToSum.Add(currentSpectra2);//all additional scans
                }
                else
                { 
                    DataToSum[1].SpectraDataOMICS.AddRange(omicsList);
                }
                bool print = false;
                if (print)
                {
                    #region print XYdata to disk
                    if (scanIndexList[i] >= 2591 && scanIndexList.Count > 8)
                    {
                        IXYDataWriter newXYWriter = new DataXYDataWriter();
                        string path = @"V:\XYOut" + scanIndexList[i].ToString() + ".txt";

                        //1.  extract XYData
                        List<PNNLOmics.Data.XYData> extractedXYData;
                        ConvertXYData.RunXYDataToOmicsXYData(run, out extractedXYData);

                        int t = newXYWriter.WriteOmicsXYData(extractedXYData, path);
                    }
                    #endregion
                }
            }
        }
    }
}
