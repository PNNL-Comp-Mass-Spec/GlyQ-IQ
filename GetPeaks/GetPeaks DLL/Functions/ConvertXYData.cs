using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using PNNLOmics.Data;

namespace GetPeaks_DLL.Functions
{
    public static class ConvertXYData
    {
        /// <summary>
        /// Takes a run and extracts out the XYData
        /// </summary>
        /// <param name="runGo">Run</param>
        /// <param name="newData">Omics List of XYData</param>
        public static void RunXYDataToOmicsXYData(Run runGo, out List<PNNLOmics.Data.XYData> newData)
        {
            List<double> tempXvalues = new List<double>();
            List<double> tempYValues = new List<double>();
            tempXvalues = runGo.ResultCollection.Run.XYData.Xvalues.ToList();
            tempYValues = runGo.ResultCollection.Run.XYData.Yvalues.ToList();

            newData = new List<PNNLOmics.Data.XYData>();
            for (int i = 0; i < runGo.ResultCollection.Run.XYData.Xvalues.Length; i++)
            {
                PNNLOmics.Data.XYData newPoint = new PNNLOmics.Data.XYData((float)tempXvalues[i], (float)tempYValues[i]);
                newData.Add(newPoint);
            }
        }

        /// <summary>
        /// Takes a run and extracts out the XYData
        /// </summary>
        /// <param name="runGo">Run</param>
        /// <param name="newData">Omics List of XYData</param>
        public static List<PNNLOmics.Data.XYData> DeconXYDataToOmicsXYData(DeconTools.Backend.XYData deconData)
        {
            if (deconData == null)
                return null;
            
            List<PNNLOmics.Data.XYData> newData = new List<PNNLOmics.Data.XYData>();

            List<double> tempXvalues = new List<double>();
            List<double> tempYValues = new List<double>();
            tempXvalues = deconData.Xvalues.ToList();
            tempYValues = deconData.Yvalues.ToList();

            newData = new List<PNNLOmics.Data.XYData>();
            for (int i = 0; i < deconData.Xvalues.Length; i++)
            {
                PNNLOmics.Data.XYData newPoint = new PNNLOmics.Data.XYData((float)tempXvalues[i], (float)tempYValues[i]);
                newData.Add(newPoint);
            }

            return newData;
        }

        public static void OmicsXYDataToRunXYDataRun(ref Run runGo, List<PNNLOmics.Data.XYData> existingData)
        {
            DeconTools.Backend.XYData createData = new DeconTools.Backend.XYData();
            double[] tempXvalues = new double[existingData.Count];
            double[] tempYValues = new double[existingData.Count];

            createData.Xvalues = tempXvalues;
            createData.Yvalues = tempYValues;

            for (int i = 0; i < existingData.Count; i++)
            {
                createData.Xvalues[i] = existingData[i].X;
                createData.Yvalues[i] = existingData[i].Y;
            }

            runGo.XYData = createData;
        }

        public static DeconTools.Backend.XYData OmicsXYDataToDeconXYData(List<PNNLOmics.Data.XYData> existingData)
        {
            DeconTools.Backend.XYData createData = new DeconTools.Backend.XYData();
            double[] tempXvalues = new double[existingData.Count];
            double[] tempYValues = new double[existingData.Count];

            createData.Xvalues = tempXvalues;
            createData.Yvalues = tempYValues;

            for (int i = 0; i < existingData.Count; i++)
            {
                createData.Xvalues[i] = existingData[i].X;
                createData.Yvalues[i] = existingData[i].Y;
            }

            return createData;
        }

        public static void OmicsXYDataToRunXYDataRun(ref ResultCollection resultList, List<PNNLOmics.Data.XYData> existingData)
        {
            DeconTools.Backend.XYData createData = new DeconTools.Backend.XYData();
            double[] tempXvalues = new double[existingData.Count];
            double[] tempYValues = new double[existingData.Count];

            createData.Xvalues = tempXvalues;
            createData.Yvalues = tempYValues;

            for (int i = 0; i < existingData.Count; i++)
            {
                createData.Xvalues[i] = existingData[i].X;
                createData.Yvalues[i] = existingData[i].Y;
            }

            resultList.Run.XYData = createData;
        }

        public static DeconTools.Backend.XYData OmicsXYDataToRunXYData(List<PNNLOmics.Data.XYData> existingData)
        {
            DeconTools.Backend.XYData createData = new DeconTools.Backend.XYData();
            double[] tempXvalues = new double[existingData.Count];
            double[] tempYValues = new double[existingData.Count];

            createData.Xvalues = tempXvalues;
            createData.Yvalues = tempYValues;

            for (int i = 0; i < existingData.Count; i++)
            {
                createData.Xvalues[i] = existingData[i].X;
                createData.Yvalues[i] = existingData[i].Y;
            }

            return createData;
        }

        public static DeconTools.Backend.XYData OmicsProcessedPeakToRunXYData(List<PNNLOmics.Data.ProcessedPeak> existingData)
        {
            DeconTools.Backend.XYData createData = new DeconTools.Backend.XYData();
            double[] tempXvalues = new double[existingData.Count];
            double[] tempYValues = new double[existingData.Count];

            createData.Xvalues = tempXvalues;
            createData.Yvalues = tempYValues;

            for (int i = 0; i < existingData.Count; i++)
            {
                createData.Xvalues[i] = existingData[i].XValue;
                createData.Yvalues[i] = existingData[i].Height;
            }

            return createData;
        }

        public static List<ProcessedPeak> OmicsXYDataToProcessedPeaks(List<PNNLOmics.Data.XYData> existingData)
        {
            List<ProcessedPeak> chromatogramOmicsPeaks = new List<ProcessedPeak>();
            foreach (var point in existingData)
            {
                ProcessedPeak peak = new ProcessedPeak();
                peak.XValue = point.X;
                peak.Height = point.Y;
                chromatogramOmicsPeaks.Add(peak);
            }

            return chromatogramOmicsPeaks;
        }
    }
}
