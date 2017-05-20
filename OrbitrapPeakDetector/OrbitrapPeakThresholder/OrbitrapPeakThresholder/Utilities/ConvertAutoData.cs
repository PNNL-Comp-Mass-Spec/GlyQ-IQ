using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using GetPeaks_DLL.Objects;

namespace OrbitrapPeakThresholder.Utilities
{
    public class ConvertAutoData
    {
        public List<ProcessedPeak> ConvertSortedDictionaryToProcessedPeaks(SortedDictionary<int, XYData> sortedDictionary)
        {
            List<ProcessedPeak> processedPeakResults = new List<ProcessedPeak>();
            ///Create a Sorted Dictionary from ProcessedPeaks
            for (int i = 0; i < sortedDictionary.Count; i++)
            {
                ProcessedPeak currentPeak = new ProcessedPeak();
                int currentElement = sortedDictionary.Keys.ElementAt(i);
                currentPeak.Height = sortedDictionary[currentElement].Y;
                currentPeak.XValue = sortedDictionary[currentElement].X;
                processedPeakResults.Add(currentPeak);
            }

            return processedPeakResults;
        }

        public List<ProcessedPeak> ConvertXYDataToProcessedPeaks(List<XYData> XYDataList)
        {
            List<ProcessedPeak> processedPeakResults = new List<ProcessedPeak>();
            ///Create ProcessedPeaks from XYData
            for (int i = 0; i < XYDataList.Count; i++)
            {
                ProcessedPeak currentPeak = new ProcessedPeak();
                currentPeak.Height = XYDataList[i].Y;
                currentPeak.XValue = XYDataList[i].X;
                processedPeakResults.Add(currentPeak);
            }

            return processedPeakResults;
        }

        public List<XYData> ConvertProcessedPeaksToXYData(List<ProcessedPeak> processedPeakList)
        {
            List<XYData> XYDataResults = new List<XYData>();
            ///Create XYData from ProcessedPeaks
            for (int i = 0; i < processedPeakList.Count; i++)
            {
                XYDataResults.Add(new XYData(processedPeakList[i].XValue, processedPeakList[i].Height));
            }
            return XYDataResults;
        }

        public List<XYData> ConvertSortedDictionaryToXYData(SortedDictionary<int, XYData> sortedDictionary)
        {
            List<XYData> XYDataResults = new List<XYData>();
            ///Create XYData from a Sorted Dictionary
            foreach (int index in sortedDictionary.Keys)
            {
                XYDataResults.Add(new XYData(sortedDictionary[index].X, sortedDictionary[index].Y));
            }

            return XYDataResults;
        }

        public List<XYData> ConvertIsosObjectsToXYData(List<IsosObject> isosObjectList)
        {
            List<XYData> XYDataResults = new List<XYData>();
            ///Create XYData from IsosObjects
            foreach (IsosObject index in isosObjectList)
            {
                XYDataResults.Add(new XYData(index.mz, index.mono_abundance));
            }

            return XYDataResults;
        }
    }
}
