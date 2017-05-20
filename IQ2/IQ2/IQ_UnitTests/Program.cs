
using System;
using System.Collections.Generic;
using GetPeaks_DLL.DataFIFO;
using IQ.DeconEngine.PeakDetection;


namespace IQ_UnitTests
{
    class Program
    {
        static void Main(string[] args)
        {
            GetPeaks_DLL.DataFIFO.LoadXYData loader = new LoadXYData();
            string rawData = @"D:\PNNL\Projects\GlyQ-IQ Paper\Failed Correlation\MSPeak\SN123Scan2104-2179Short.txt";
            List<PNNLOmics.Data.XYData> data = loader.Import(rawData);

            List<double> mz = new List<double>();
            List<double> intenstity = new List<double>();

            foreach (PNNLOmics.Data.XYData xyData in data)
            {
                mz.Add(xyData.X);intenstity.Add(xyData.Y);
            }
            
            PeakProcessor detector = new PeakProcessor();
            
            int peakdsFound = detector.DiscoverPeaks(mz, intenstity, 0, 9999);
            Console.WriteLine("THere are " + peakdsFound + " peaks");
        }
    }
}
