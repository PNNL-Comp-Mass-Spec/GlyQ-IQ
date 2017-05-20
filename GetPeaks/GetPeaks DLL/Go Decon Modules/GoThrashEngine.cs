using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using GetPeaks_DLL.Objects.Enumerations;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.Objects.ResultsObjects;
using DeconToolsV2.HornTransform;
using GetPeaks_DLL.THRASH;
using Parallel.THRASH;
using DeconTools.Backend.Runs;
using GetPeaks_DLL.Parallel;
using GetPeaks_DLL.Functions;

namespace GetPeaks_DLL.Go_Decon_Modules
{
    public static class GoThrashEngine
    {
        public static void DeconvoluteWithEngine(int scan, ResultsTHRASH objectInsideThread, Run runGo, EngineThrashDeconvolutor currentEngine)
        {
            if (runGo.ResultCollection.Run.PeakList.Count > 2) //2 points will crash rapid.  we need atleast 3
            {
                Console.WriteLine("Start Thrash...");

                ParametersTHRASH currentParameters = (ParametersTHRASH)currentEngine.Parameters;

                const DeconvolutionOptions switchYard = DeconvolutionOptions.DeconvolutorVersion2;


                ResultCollectionLite deconvolutionOutput;

                switch (switchYard)
                {
                    case (DeconvolutionOptions.Transformer):
                        {
                            #region transformer code

                            /*
                            TransformerObject transformer2 = new TransformerObject();


                            transformer2.TransformEngine = currentEngine.TransformEngine;

                            float[] xvals;
                            float[] yvals;
                            DeconToolsV2.Peaks.clsPeak[] mspeakList;
                            DeconToolsV2.HornTransform.clsHornTransformResults[] transformResults2;
                            GoTransformPrep transformPrep = new GoTransformPrep();
                            //transformPrep.PrepDeconvolutor(runGo, transformer2, out xvals, out yvals, out mspeakList, out transformResults2);
                            transformPrep.PrepDeconvolutor(runGo, out xvals, out yvals, out mspeakList, out transformResults2);

                            for (int i = 0; i < xvals.Length; i++)
                            {
                                //yvals[i] = Convert.ToSingle(Math.Round(yvals[i], 3));
                                //xvals[i] = Convert.ToSingle(Math.Round(xvals[i],3));
                            }
                            List<double> initialMSPeakList = new List<double>();
                            foreach (DeconToolsV2.Peaks.clsPeak pair in mspeakList)
                            {
                                initialMSPeakList.Add(pair.mdbl_mz);
                            }

                            int storeinitialMSPeakListCount = initialMSPeakList.Count;
                            int storeXVals = xvals.Length;

                            if (currentEngine.CurrentScanSet.PrimaryScanNumber == 31)
                            {
                                int y = 4;
                                y++;
                            }


                            List<DeconToolsV2.HornTransform.clsHornTransformResults> copiedResults = new List<DeconToolsV2.HornTransform.clsHornTransformResults>();

                            ParalellLog.LogPeakCount(currentEngine, scan, mspeakList);

                            for (int i = 0; i < mspeakList.Length; i++)
                            {
                                double mass = mspeakList[i].mdbl_mz;
                                ParalellLog.LogMass(currentEngine, scan, mass);
                                //mspeakList[i].mdbl_mz = Math.Round(mspeakList[i].mdbl_mz,3); //did not help
                                //mspeakList[i].mdbl_intensity = Math.Round(mspeakList[i].mdbl_intensity, 3); //did not help
                            }

                            DeconToolsV2.Peaks.clsPeak[] PlacceHolderMspeakList = new DeconToolsV2.Peaks.clsPeak[100];//this might do something
                            for (int i = 0; i < 100; i++)
                            {
                                PlacceHolderMspeakList[i] = new DeconToolsV2.Peaks.clsPeak();
                            }
                            //this.Run.ResultCollection.IsosResultBin.Clear();

                            //float backgroundIntensity = 0;
                            //float minPeptideIntensity = 0;
                            float backgroundIntensity = (float)currentEngine.Run.CurrentBackgroundIntensity;
                            float minPeptideIntensity = (float)(currentEngine.Run.CurrentBackgroundIntensity * transformer2.TransformEngine.TransformParameters.PeptideMinBackgroundRatio);
                            transformer2.TransformEngine.PerformTransform(backgroundIntensity, minPeptideIntensity, ref xvals, ref yvals, ref mspeakList, ref transformResults2);//Why do we need 2 deconvolutors.  Something with prepping the transformResults?

                            //transformer2.TransformEngine.PerformTransform(backgroundIntensity, minPeptideIntensity, ref xvals, ref yvals, ref mspeakList, ref transformResults2);

                            ParalellLog.LogMonoCount(currentEngine, scan, transformResults2);

                            if (currentEngine.CurrentScanSet.PrimaryScanNumber == 23)
                            {
                                int y = 4;
                                y++;
                            }

                            int storeIsotopesDiscovered = transformResults2.Length;

                            foreach (DeconToolsV2.HornTransform.clsHornTransformResults hornResult in transformResults2)
                            {
                                copiedResults.Add(hornResult);
                            }

                            List<double> viewOfRawIsosFromTransformer = new List<double>();
                            foreach (DeconToolsV2.HornTransform.clsHornTransformResults hornResult in copiedResults)
                            {
                                viewOfRawIsosFromTransformer.Add(hornResult.mdbl_mono_mw);
                            }
                            viewOfRawIsosFromTransformer.Sort();

                            foreach (double mass in viewOfRawIsosFromTransformer)
                            {
                                ParalellLog.LogPeaks(currentEngine, scan, mass);
                            }
                            if (currentEngine.CurrentScanSet.PrimaryScanNumber == 31)
                            {
                                int y = 4;
                                y++;
                            }

                            int rawIsosFromTransformerCount = viewOfRawIsosFromTransformer.Count;

                            //ResultCollectionLite deconvolutionOutput = new ResultCollectionLite();
                            deconvolutionOutput = transformPrep.FormatResults(copiedResults, mspeakList, transformer2);
                            //deconvolutionOutput = transformPrep.FormatResults(transformResults2, mspeakList, transformer2);

                            List<double> viewOfCopnvertedIsos = new List<double>();
                            foreach (IsosResultLite iResult in deconvolutionOutput.IsosResultBin)
                            {
                                viewOfCopnvertedIsos.Add(iResult.IsotopicProfile.MonoIsotopicMass);
                            }
                            viewOfCopnvertedIsos.Sort();



                            //list for debugging
                            List<double> checkFromIsosResultBins = new List<double>();
                            foreach (IsosResultLite iResult in deconvolutionOutput.IsosResultBin)
                            {
                                //MS.Add(iResult.IsotopicProfile.Peaklist[0].XValue);
                                checkFromIsosResultBins.Add(iResult.IsotopicProfile.MonoIsotopicMass);
                            }
                            checkFromIsosResultBins.Sort();

                            List<double> checkRawIsosFromTransformer = new List<double>();
                            foreach (DeconToolsV2.HornTransform.clsHornTransformResults hornResult in transformResults2)
                            {
                                checkRawIsosFromTransformer.Add(hornResult.mdbl_mono_mw);
                            }
                            checkRawIsosFromTransformer.Sort();


                            for (int i = 0; i < viewOfRawIsosFromTransformer.Count; i++)
                            {
                                double check = Math.Abs(viewOfRawIsosFromTransformer[i] - checkRawIsosFromTransformer[i]);
                                if (check > 0.1)
                                {
                                    int y = 4;
                                    y++;
                                }

                                double check2 = Math.Abs(viewOfRawIsosFromTransformer[i] - checkFromIsosResultBins[i]);
                                if (check2 > 0.1)
                                {
                                    int y = 4;
                                    y++;
                                }
                            }

                            if (currentEngine.CurrentScanSet.PrimaryScanNumber == 24)
                            {
                                int y = 4;
                                y++;
                            }

                            //check to see if any of the data changed in the process
                            if (initialMSPeakList.Count != storeinitialMSPeakListCount || storeXVals != xvals.Length || viewOfRawIsosFromTransformer.Count != rawIsosFromTransformerCount || transformResults2.Length != storeIsotopesDiscovered)
                            {
                                int y = 4;
                                y++;
                            }
                            //deconvolutionOutput.Dispose();
                            transformResults2 = null;

                             */

                            #endregion
                        }
                        break;
                    case (DeconvolutionOptions.Deconvolutor):
                        {
                            #region Deconvolutor Code

                            float[] xvals;
                            float[] yvals;
                            DeconToolsV2.Peaks.clsPeak[] mspeakList;
                            DeconToolsV2.HornTransform.clsHornTransformResults[] transformResults2;
                            GoTransformPrep transformPrep2 = new GoTransformPrep();
                            transformPrep2.PrepDeconvolutor(runGo, out xvals, out yvals, out mspeakList, out transformResults2);

                            ResultCollection resultList = currentEngine.Run.ResultCollection;

                            clearCurrentScanIsosResultBin(resultList);

                            if (currentEngine.DeconvolutorEngine != null)
                            {
                                //currentEngine.DeconvolutorEngine.deconvolute(resultList);
                                currentEngine.DeconvolutorEngine.Execute(resultList);
                            }
                            else
                            {
                                Console.WriteLine("Missing Deconvolutor");
                                Console.ReadKey();
                            }
                            associatePeaksToMSFeatureID(resultList);

                            GoTransformPrep transformPrep = new GoTransformPrep();

                            HornTransformParameters parameters = currentParameters.DeisotopingParametersThrash.Parameters;

                            DeconToolsV2.Peaks.clsPeak[] mspeakList2 = mspeakList;

                            //addCurrentScanIsosResultsToOverallList(resultList);

                            List<clsHornTransformResults> copiedResults = transformPrep.ResultsBinToHornList(scan, resultList);


                            List<double> viewOfRawIsosFromTransformer = new List<double>();

                            foreach (DeconToolsV2.HornTransform.clsHornTransformResults hornResult in copiedResults)
                            {
                                ParalellLog.LogPeaksMZ(currentEngine, scan, hornResult.mdbl_mz, hornResult.mshort_cs);

                                viewOfRawIsosFromTransformer.Add(hornResult.mdbl_mono_mw);
                            }
                            viewOfRawIsosFromTransformer.Sort();

                            foreach (double mass in viewOfRawIsosFromTransformer)
                            {
                                //ParalellLog.LogPeaks(currentEngine, scan, mass);
                            }


                            deconvolutionOutput = transformPrep.FormatResults(copiedResults, mspeakList2, parameters, resultList);

                            #endregion
                        }
                        break;
                    case (DeconvolutionOptions.DeconvolutorVersion2):
                        {
                            #region Deconvolutor Code

                            DeconToolsV2.Peaks.clsPeak[] mspeakList = currentEngine.Run.DeconToolsPeakList;

                            //this should clear out the old results
                            currentEngine.Run.ResultCollection.IsosResultBin = new List<IsosResult>();

                            currentEngine.DeconvolutorEngine.Execute(currentEngine.Run.ResultCollection);

                            GoTransformPrep transformPrep = new GoTransformPrep();
                            List<clsHornTransformResults> copiedResults = transformPrep.ResultsBinToHornList(scan, currentEngine.Run.ResultCollection);

                            //Console.WriteLine("DeconMonoCount =" + currentEngine.Run.ResultCollection.IsosResultBin.Count);
                            ParametersTHRASH parameters = (ParametersTHRASH)currentEngine.Parameters;

                            deconvolutionOutput = transformPrep.FormatResults(copiedResults, mspeakList, parameters.DeisotopingParametersThrash.Parameters, currentEngine.Run.ResultCollection);

                            #endregion
                        }
                        break;
                }


                //this.Deconvolutor.deconvolute(ref xvals, ref yvals, backgroundIntensity, minPeptideIntensity, ref mspeakList);

                //this.Deconvolutor.cleanup();
                //this.Deconvolutor = null;
                //newProfiler.printMemory("After ");
                objectInsideThread.Scan = scan;
                objectInsideThread.MonoisotopicPeaksInScan = deconvolutionOutput.IsosResultBin.Count;

                Console.WriteLine("There are " + deconvolutionOutput.IsosResultBin.Count + " mono's detected");

                if (deconvolutionOutput.IsosResultBin.Count > 0)
                {
                    Console.WriteLine("yeay");
                }
                foreach (IsosResultLite isos in deconvolutionOutput.IsosResultBin)
                {
                    objectInsideThread.ResultsFromRun.AddIsosResult(isos);

                    IsotopeObject newIsosBox = new IsotopeObject();
                    newIsosBox.MonoIsotopicMass = isos.IsotopicProfile.MonoIsotopicMass;
                    newIsosBox.ExperimentMass = isos.IsotopicProfile.MonoPeakMZ; //this is the first point in the cluster peaklist
                    newIsosBox.Charge = isos.ChargeState;
                    newIsosBox.IsotopeMassString = "Engine" + currentEngine.EngineNumber.ToString();
                    newIsosBox.FitScore = isos.FitScore;
                    const bool keepPeak = true;
                    newIsosBox.IsotopeList = ConvertPeakListDeconToOmics.Convert(isos.IsotopicProfile.Peaklist, keepPeak);
                    objectInsideThread.ResultsFromRunConverted.Add(newIsosBox);
                }
                objectInsideThread.ResultsFromRunConverted.OrderBy(p => p.MonoIsotopicMass).ToList();
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
    }
}
