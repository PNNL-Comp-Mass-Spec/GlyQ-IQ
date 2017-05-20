using System;
using System.Collections.Generic;
using GetPeaks_DLL.CompareContrast;
using PNNLOmics.Data;
using HammerPeakDetector.Utilities;
using PNNLOmics.Data.Peaks;

namespace HammerPeakDetector.Comparisons
{
    public class ROCcreator
    {

        public CompareResultsIndexes CompareIndexes(double massToleranceMatch, List<ProcessedPeak> processedPeaks, List<ProcessedPeak> dataToCompare, SortedDictionary<int, ProcessedPeak> trueDictionary, SortedDictionary<int, ProcessedPeak> falseDictionary)
        {
            CompareResultsIndexes result = CompareResultsProcessedPeaks(dataToCompare, processedPeaks, massToleranceMatch);
            foreach (int index in result.IndexListAMatch)
            {
                trueDictionary.Add(index, processedPeaks[index]);
            }

            foreach (int index in result.IndexListAandNotB)
            {
                falseDictionary.Add(index, processedPeaks[index]);
            }
            
            return result;
        }

        public ROCstorage CreateROCBoxes(SortedDictionary<int, ProcessedPeak> trueDictionary1, SortedDictionary<int, ProcessedPeak> falseDictionary1, SortedDictionary<int, ProcessedPeak> trueDictionary2, SortedDictionary<int, ProcessedPeak> falseDictionary2)
        {
            //ROCstorage newBox = new ROCstorage();
            ////good box
            //newBox.ManualNoise = trueDictionary1;
            //newBox.ManualSignal = falseDictionary1;
            //newBox.TestNoise = trueDictionary2;
            //newBox.TestSignal = falseDictionary2;

            ROCstorage newBox = new ROCstorage();
            //good box
            newBox.ManualNoise = falseDictionary1;
            newBox.ManualSignal = trueDictionary1;
            newBox.TestNoise = falseDictionary2;
            newBox.TestSignal = trueDictionary2;

            return newBox;
        }

        public CompareResultsIndexes CompareResultsProcessedPeakDictionary(SortedDictionary<int, ProcessedPeak> peakManual, SortedDictionary<int, ProcessedPeak> peakAutomated, double massTollerance)
        {
            //IConvert letsConvert = new Converter();
            SetListsToCompare prepCompare = new SetListsToCompare();
            CompareController compareHere = new CompareController();

            //from manual annotation
            //List<double> manualMassLibrary = letsConvert.XYDataToMass(peakManual);
            List<double> manualMassLibrary = CreateProcessedPeakLibrary(peakManual);

            //from algorithm
            //List<double> automatedData = letsConvert.XYDataToMass(peakAutomated);
            List<double> automatedData = CreateProcessedPeakLibrary(peakAutomated);

            return CompareIndexes(massTollerance, prepCompare, compareHere, manualMassLibrary, automatedData);
        }

        private CompareResultsIndexes CompareResultsProcessedPeaks(List<ProcessedPeak> peakManual, List<ProcessedPeak> peakAutomated, double massTollerance)
        {
            DataConverter letsConvert = new DataConverter();
            SetListsToCompare prepCompare = new SetListsToCompare();
            CompareController compareHere = new CompareController();

            //from manual annotation
            List<double> manualMassLibrary = letsConvert.ProcessedPeaksToMass(peakManual);

            //from algorithm
            List<double> automatedData = letsConvert.ProcessedPeaksToMass(peakAutomated);

            return CompareIndexes(massTollerance, prepCompare, compareHere, manualMassLibrary, automatedData);
        }

        private static CompareResultsIndexes CompareIndexes(double massTollerance, SetListsToCompare prepCompare, CompareController compareHere, List<double> massLibrary, List<double> automatedData)
        {
            CompareInputLists inputListsPeakMasses = prepCompare.SetThem(automatedData, massLibrary);

            CompareResultsIndexes indexesFromCompare = new CompareResultsIndexes();
            CompareResultsValues valuesFromCompare = compareHere.compareFX(inputListsPeakMasses, massTollerance, ref indexesFromCompare);

            return indexesFromCompare;
        }

        private static List<double> CreateProcessedPeakLibrary(SortedDictionary<int, ProcessedPeak> peaks)
        {
            List<double> newLibrary = new List<double>();
            foreach (KeyValuePair<int, ProcessedPeak> point in peaks)
            {
                newLibrary.Add(point.Value.XValue);
            }
            return newLibrary;
        }
    }
}
