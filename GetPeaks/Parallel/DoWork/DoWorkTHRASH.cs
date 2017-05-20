using System;
using System.Collections.Generic;
using System.Linq;
using CompareContrastDLL;
using DeconTools.Backend.Core;
using DeconTools.Backend.DTO;
using DeconTools.Backend.ProcessingTasks;
using DeconTools.Backend.Runs;
using DeconToolsV2.HornTransform;
using DeconToolsV2.Peaks;
using GetPeaks_DLL;
using GetPeaks_DLL.Functions;
using GetPeaks_DLL.Go_Decon_Modules;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.Objects.Enumerations;
using GetPeaks_DLL.Objects.ResultsObjects;
using OrbitrapPeakDetection;
using OrbitrapPeakDetection.Objects;
using Parallel.THRASH;
using PNNLOmics.Algorithms.PeakDetection;
using PNNLOmics.Data;
using Peak = PNNLOmics.Data.Peak;
using GetPeaks_DLL.Parallel;

namespace Parallel.DoWork
{
    public static class DoWorkTHRASH
    {
        public static ParalellResults WorkWork(ParalellThreadData data)
        {
            ParalellThreadDataTHRASH currentParameterObject = (ParalellThreadDataTHRASH) data;

            #region decompress parameters

            //int scanToSum = currentParameterObject.NumberOfScansToSum;

            int scan = currentParameterObject.Scan;

            //EngineTHRASH currentEngine = (EngineTHRASH)currentParameterObject.Engine;
            EngineThrashDeconvolutor currentEngine = (EngineThrashDeconvolutor) currentParameterObject.Engine;

            ParalellLog.LogAddScan(currentEngine, scan);

            currentEngine.CurrentScanSet = currentEngine.Run.ScanSetCollection.ScanSetList[scan];

            ParametersTHRASH currentParameters = (ParametersTHRASH) currentEngine.Parameters;

            #endregion

            ResultsTHRASH objectInsideThread = new ResultsTHRASH(scan);

            Run runGo = currentEngine.Run;

            #region Part 1, go get data and sum it

            SimpleWorkflowParameters controllerBParameters = new SimpleWorkflowParameters();
            controllerBParameters.SummingMethod = ScanSumSelectSwitch.SumScan;
            ScanSumSelectSwitch summingMethod = controllerBParameters.SummingMethod;
            controllerBParameters.Part2Parameters.MSLevelOnly = false;
            GoDeconToolsControllerA newDeconToolsPart1 = new GoDeconToolsControllerA(runGo, controllerBParameters);
            //GoDeconToolsControllerB newDeconToolsPart2 = new GoDeconToolsControllerB(controllerBParameters, DeconTools.Backend.Globals.MSFileType.Undefined);

            runGo.CurrentScanSet = currentEngine.CurrentScanSet;

            newDeconToolsPart1.GoLoadDataAndSumIt(runGo);

            int unprocessedPeaksCount = runGo.XYData.Xvalues.Length;

            objectInsideThread.RawPeaksInScan = unprocessedPeaksCount;

            #endregion

            #region Part 2, peakdetection

            //the mspeaks will be stored in the Run for THRASH
            ResultsPeakDetection peakObjectinsideThread = new ResultsPeakDetection();
            ParametersPeakDetection peakDetectionParameters = PeakDetectionParametersConvert(currentParameters);
            List<string> logAddition;
            GoPeakDetection.PeakProcessing(peakDetectionParameters, runGo, peakObjectinsideThread, out logAddition);

            objectInsideThread.Scan = scan;
            objectInsideThread.RawPeaksInScan = peakObjectinsideThread.RawPeaksInScan;//count
            objectInsideThread.CentroidedPeaksInScan = peakObjectinsideThread.CentroidedPeaksInScan;//count
            objectInsideThread.ThresholdedPeaksInScan = peakObjectinsideThread.ThresholdedPeaksInScan;//count
            objectInsideThread.NoisePeaks = peakObjectinsideThread.NoisePeaks;//data
            objectInsideThread.SignalPeaks = peakObjectinsideThread.SignalPeaks;//data
            objectInsideThread.ResultsFromPeakDetectorClusters = peakObjectinsideThread.ResultsFromPeakDetectorClusters;//data
           
            currentEngine.ErrorLog.AddRange(logAddition);
            #endregion

            #region Part 3, Deisotoping

            /*
            SortedDictionary<int, XYData> signalDeconDictionary = new SortedDictionary<int, XYData>();
            SortedDictionary<int, XYData> loadedRawDataFromFileDictionary = new SortedDictionary<int, XYData>();
            int counter = 0;
            foreach (DeconTools.Backend.Core.Peak dPeak in runGo.PeakList)
            {
                signalDeconDictionary.Add(counter, new XYData(dPeak.XValue, dPeak.Height));
                counter++;
            }

            //for debugging
            List<double> masz = new List<double>();
            foreach (KeyValuePair<int, XYData> pair in signalDeconDictionary)
            {
                masz.Add(pair.Value.X);
            }


            counter = 0;
            for (int i = 0; i < runGo.XYData.Xvalues.Length; i++)
            {
                loadedRawDataFromFileDictionary.Add(counter, new XYData(runGo.XYData.Xvalues[i], runGo.XYData.Yvalues[i]));
                counter++;
            }
            */

            GoThrashEngine.DeconvoluteWithEngine(scan, objectInsideThread, runGo, currentEngine);

            List<double> checkMasss = new List<double>();
            foreach (IsotopeObject iObject in objectInsideThread.ResultsFromRunConverted)
            {
                checkMasss.Add(iObject.MonoIsotopicMass);
            }
            checkMasss.Sort();

            //fill with mono data
            //objectInsideThread.ResultsFromRun = new ResultCollectionLite();
            //objectInsideThread.ResultsFromRunConverted = new List<IsotopeObject>();
            //objectInsideThread.MonoisotopicPeaksInScan = 0;


            #endregion

            return objectInsideThread;
        }

       
        private static ParametersPeakDetection PeakDetectionParametersConvert(ParametersTHRASH currentParameters)
        {
            ParametersPeakDetection peakDetectionParameters = new ParametersPeakDetection();
            peakDetectionParameters.DeconToolsPeakDetection.MsPeakDetectorPeakToBackground = currentParameters.MsPeakDetectorPeakToBackground;
            peakDetectionParameters.DeconToolsPeakDetection.PeakFitType = currentParameters.PeakFitType;
            peakDetectionParameters.DeconToolsPeakDetection.SignalToNoiseRatio = currentParameters.SignalToNoiseRatio;
            peakDetectionParameters.HammerParameters = currentParameters.HammerParameters;
            peakDetectionParameters.EngineScanSetCollection = currentParameters.EngineScanSetCollection;
            peakDetectionParameters.MaxScanLimitation = currentParameters.MaxScanLimitation;
            peakDetectionParameters.PeakDetectionMethod = currentParameters.PeakDetectionMethod;
            peakDetectionParameters.ProcessMsms = currentParameters.ProcessMsms;
            peakDetectionParameters.ScansToSum = currentParameters.ScansToSum;
            return peakDetectionParameters;
        }


        private static string SetIsotopeIntensitiesCSV(IsosResultLite isosDecon)
        {
            string commaSeperatedString = "";
            for (int i = 0; i < isosDecon.IsotopicProfile.Peaklist.Count - 1; i++)
            {
                commaSeperatedString += isosDecon.IsotopicProfile.Peaklist[i].Height.ToString() + ",";
            }

            commaSeperatedString += isosDecon.IsotopicProfile.Peaklist[isosDecon.IsotopicProfile.Peaklist.Count - 1].Height.ToString();

            return commaSeperatedString;
        }

        private static string SetIsotopeMassesCSV(IsosResultLite isosDecon)
        {
            string commaSeperatedString = "";
            for (int i = 0; i < isosDecon.IsotopicProfile.Peaklist.Count - 1; i++)
            {
                commaSeperatedString += isosDecon.IsotopicProfile.Peaklist[i].XValue.ToString() + ",";
            }

            commaSeperatedString += isosDecon.IsotopicProfile.Peaklist[isosDecon.IsotopicProfile.Peaklist.Count - 1].XValue.ToString();

            return commaSeperatedString;
        }

        #region decon private functions

        private static void associatePeaksToMSFeatureID(ResultCollection resultList)
        {
            if (resultList.IsosResultBin == null || resultList.IsosResultBin.Count == 0) return;

            foreach (var msfeature in resultList.IsosResultBin)
            {
                foreach (MSPeak peak in msfeature.IsotopicProfile.Peaklist)
                {
                    peak.MSFeatureID = msfeature.MSFeatureID;

                }

            }
        }

        private static void clearCurrentScanIsosResultBin(ResultCollection resultList)
        {
            //remove the result if it was a result of a different scan. Otherwise keep it
            //this allows running of two back-to-back deconvolutors without clearing the results
            //between deconvolutions.   Going backwards through the list prevents exceptions. 
            if (resultList.IsosResultBin == null || resultList.IsosResultBin.Count == 0) return;

            if (resultList.Run is UIMFRun)
            {
                resultList.IsosResultBin.Clear();

            }
            else
            {
                for (int i = resultList.IsosResultBin.Count - 1; i >= 0; i--)
                {
                    if (resultList.IsosResultBin[i].ScanSet.PrimaryScanNumber != resultList.Run.CurrentScanSet.PrimaryScanNumber)
                    {
                        resultList.IsosResultBin.RemoveAt(i);
                    }

                }
            }
        }

        private static void addCurrentScanIsosResultsToOverallList(ResultCollection resultList)
        {
            resultList.ResultList.AddRange(resultList.IsosResultBin);
        }

        #endregion
    }
}
