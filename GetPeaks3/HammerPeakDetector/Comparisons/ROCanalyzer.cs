using System.Collections.Generic;
using System.Linq;
using GetPeaks_DLL.CompareContrast;
using PNNLOmics.Data;
using PNNLOmics.Data.Peaks;

namespace HammerPeakDetector.Comparisons
{
    public class ROCanalyzer
    {
        public ROCobject analyze(List<ProcessedPeak> peaks, ROCstorage box, double massTolleranceMatch)
        {
            ROCcreator comparer = new ROCcreator();

            List<ProcessedPeak> signalPeaksManual = new List<ProcessedPeak>();


            CompareResultsIndexes signalResults = comparer.CompareResultsProcessedPeakDictionary(box.ManualSignal, box.TestSignal, massTolleranceMatch);

            CompareResultsIndexes noiseResults = comparer.CompareResultsProcessedPeakDictionary(box.ManualNoise, box.TestNoise, massTolleranceMatch);

            //ROCObject ROCResults = ConvertToROC(signalPeaksAutomated, noisePeaksAutomated, signalResults, noiseResults);
            ROCobject ROCResults = ConvertToROC(peaks, signalResults, noiseResults, box);

            return ROCResults;
        }

        private static ROCobject ConvertToROC(List<ProcessedPeak> peaks, CompareResultsIndexes signalResults, CompareResultsIndexes noiseResults, ROCstorage box)
        {
            ROCobject ROCResults = new ROCobject();
            int returnIndexForPeaks = 0;//holds index corresponding to the peaks file

            //True Positive, ListAMatch from signalResults, Signal Matched or Correct Hits
            foreach (int index in signalResults.IndexListBMatch)//AMatch for TP
            {
                KeyValuePair<int, ProcessedPeak> holdPoint = box.ManualSignal.ElementAt(index);
                returnIndexForPeaks = holdPoint.Key;
                ROCResults.TruePositives.Add(returnIndexForPeaks, peaks[returnIndexForPeaks]);
            }

            //False Positives, NotInLibraryB from signalResults, Noise Matched or False Hit
            foreach (int index in signalResults.IndexListAandNotB)
            {
                KeyValuePair<int, ProcessedPeak> holdPoint = box.TestSignal.ElementAt(index);
                returnIndexForPeaks = holdPoint.Key;
                ROCResults.FalsePositives.Add(returnIndexForPeaks, peaks[returnIndexForPeaks]);
            }

            //TODO there is something fishy here

            //False Negatives, NotInLibraryA from signalResults, Signal Missed that we should have gotten or False Miss
            foreach (int index in signalResults.IndexListBandNotA)
            {
                KeyValuePair<int, ProcessedPeak> holdPoint = box.ManualSignal.ElementAt(index);
                returnIndexForPeaks = holdPoint.Key;
                ROCResults.FalseNegatives.Add(returnIndexForPeaks, peaks[returnIndexForPeaks]);
            }

            //True Negative, ListAMatch from noiseResults, Noise matched or Correct Miss
            foreach (int index in noiseResults.IndexListBMatch)
            {
                KeyValuePair<int, ProcessedPeak> holdPoint = box.ManualNoise.ElementAt(index);
                returnIndexForPeaks = holdPoint.Key;
                ROCResults.TrueNegatives.Add(returnIndexForPeaks, peaks[returnIndexForPeaks]);
            }

            return ROCResults;
        }
    }
}
