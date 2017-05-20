using System;
using System.Collections.Generic;
using GetPeaks_DLL.CompareContrast;
using PNNLOmics.Data;
using HammerPeakDetector.Enumerations;
using PNNLOmics.Data.Peaks;

namespace HammerPeakDetector.Comparisons
{
    public class ROCresults
    {
        public ROCobject calculateROC(List<ProcessedPeak> allPeaks, List<ProcessedPeak> signalPeaksHammer, List<ProcessedPeak> signalPeaksManual)
        {
            ROCobject results = new ROCobject();

            #region Variables
            GetData getManualData = new GetData();

            // //QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW scan 5509

            List<ProcessedPeak> noiseAutomatic = new List<ProcessedPeak>();
            List<ProcessedPeak> signalManual = getManualData.LoadProcessedPeakAnswers(SpectraDataType.SignalPeaks);
            List<ProcessedPeak> noiseManual = getManualData.LoadProcessedPeakAnswers(SpectraDataType.NoisePeaksFull);

            SortedDictionary<int, ProcessedPeak> signalManualDictionary = new SortedDictionary<int, ProcessedPeak>();
            SortedDictionary<int, ProcessedPeak> noiseManualDictionary = new SortedDictionary<int, ProcessedPeak>();
            SortedDictionary<int, ProcessedPeak> signalHammerDictionary = new SortedDictionary<int, ProcessedPeak>();
            SortedDictionary<int, ProcessedPeak> noiseHammerDictionary = new SortedDictionary<int, ProcessedPeak>();

            double massToleranceMatch = 3;
            
            ROCcreator createROCboxes = new ROCcreator();

            #endregion

            #region Create dictionaries

            CompareResultsIndexes manualCompare = createROCboxes.CompareIndexes(massToleranceMatch, allPeaks, signalManual, signalManualDictionary, noiseManualDictionary);
            CompareResultsIndexes hammerCompare = createROCboxes.CompareIndexes(massToleranceMatch, allPeaks, signalPeaksHammer, signalHammerDictionary, noiseHammerDictionary);

            #endregion

            ROCstorage boxOrbitrapPeaksToManualPeaks = createROCboxes.CreateROCBoxes(signalManualDictionary, noiseManualDictionary, signalHammerDictionary, noiseHammerDictionary);

            if (boxOrbitrapPeaksToManualPeaks.ManualNoise.Count > 0 && boxOrbitrapPeaksToManualPeaks.ManualSignal.Count > 0 && boxOrbitrapPeaksToManualPeaks.TestNoise.Count > 0 && boxOrbitrapPeaksToManualPeaks.TestSignal.Count > 0)
            {
                ROCanalyzer analyzer = new ROCanalyzer();
                //ROCresultsHammerPeaksToManualPeaks = analyzer.analyze(allPeaks, boxOrbitrapPeaksToManualPeaks, hammerResults.NewParameters.SeedMassToleranceDa);
                results = analyzer.analyze(allPeaks, boxOrbitrapPeaksToManualPeaks, massToleranceMatch);

                ROCrates newtester = new ROCrates();
                newtester.Calculate(ref results);
            }
            else
            {
                Console.WriteLine("missing data for compare.  Check that the box is full");//we need to make this smarter so it can deal with 0
                Console.ReadKey();
            }

            return results;
        }
    }
}
