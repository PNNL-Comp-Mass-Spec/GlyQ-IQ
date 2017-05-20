using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Go_Decon_Modules;
using DeconTools.Backend.Core;
using PNNLOmics.Data;
using GetPeaks_DLL.Objects.Enumerations;
using PNNLOmics.Algorithms.PeakDetection;
using DeconTools.Backend.ProcessingTasks;

namespace GetPeaks_DLL.TandemSupport
{
    public static class CentroidPeaksDetector
    {
        public static List<PNNLOmics.Data.Peak> CentroidPeaks(ParametersPeakDetection currentParameters, Run runGo, ResultsPeakDetection objectInsideThread, PeakCentroidOptions centroidSwitch)
        {
            List<PNNLOmics.Data.Peak> centroidedPeaks = new List<PNNLOmics.Data.Peak>();

            List<ProcessedPeak> omicsCentroidedPeaks;

            switch (centroidSwitch) //centroid data with either PNNL omics or DeconTools
            {
                #region inside switch

                case PeakCentroidOptions.PNNLOmics:
                    {
                        //this presents a slightyly different list
                        List<XYData> simplePeaks = new List<XYData>();
                        ////convert decon to XYData
                        for (int i = 0; i < runGo.XYData.Xvalues.Count(); i++)
                        {
                            double yValue = runGo.XYData.Yvalues[i];
                            //centroidedPeaks.Add(new XYData(runGo.XYData.Xvalues[i], yValue));
                            PNNLOmics.Data.Peak newPeak = new PNNLOmics.Data.Peak();
                            newPeak.XValue = runGo.XYData.Xvalues[i];
                            newPeak.Height = runGo.XYData.Yvalues[i];
                            centroidedPeaks.Add(newPeak);
                            simplePeaks.Add(new XYData(newPeak.XValue, newPeak.Height));
                        }

                        PNNLOmics.Algorithms.PeakDetection.PeakCentroiderParameters centerParameters = new PeakCentroiderParameters();
                        centerParameters.FWHMPeakFitType = PeakFitType.Parabola;
                        centerParameters.IsXYDataCentroided = false;
                        PNNLOmics.Algorithms.PeakDetection.PeakCentroider centroider = new PeakCentroider();
                        omicsCentroidedPeaks = centroider.DiscoverPeaks(simplePeaks);
                        //omicsCentroidedPeaks = centroider.DiscoverPeaks(centroidedPeaks);
                        //List<ProcessedPeak> centroidedPeaks = centroider.DiscoverPeaks(peaks);

                        foreach (ProcessedPeak pPeak in omicsCentroidedPeaks)
                        {
                            //ParalellLog.LogPeaksCentroid(currentEngine, scan, pPeak.XValue);
                        }

                        centroidedPeaks = new List<PNNLOmics.Data.Peak>();
                        for (int i = 0; i < omicsCentroidedPeaks.Count(); i++)
                        {
                            if (omicsCentroidedPeaks[i].Height > 0)
                            {
                                //centroidedPeaks.Add(new XYData(omicsCentroidedPeaks[i].XValue, omicsCentroidedPeaks[i].Height));
                                PNNLOmics.Data.Peak newPeak = new PNNLOmics.Data.Peak();
                                newPeak.XValue = omicsCentroidedPeaks[i].XValue;
                                newPeak.Height = omicsCentroidedPeaks[i].Height;
                                newPeak.Width = omicsCentroidedPeaks[i].Width;
                                centroidedPeaks.Add(newPeak);
                            }
                        }

                        objectInsideThread.CentroidedPeaksInScan = centroidedPeaks.Count;
                    }
                    break;
                case PeakCentroidOptions.DeconPeakDetector: //this is the centroid part of the msPeakDetector becasue SN = 0
                    {
                        runGo.ResultCollection.Run.PeakList = null;
                        //clear it out because this is a scan by scan peak list
                        //centroid only and let everything go through //DeconToolsPeakDetector msPeakDetector = new DeconToolsPeakDetector(0, 0, currentParameters.DeconToolsPeakDetection.PeakFitType, false);

                        DeconToolsPeakDetector msPeakDetector = new DeconToolsPeakDetector(currentParameters.HammerParameters.CentroidPeakToBackgroundRatio, 0, currentParameters.DeconToolsPeakDetection.PeakFitType, false);
                        //msPeakDetector.IsDataThresholded = true;
                        //this does not give the best results I think 9-24-12
                        msPeakDetector.Execute(runGo.ResultCollection);

                        //peaks = new List<XYData>();

                        //for (int i = 0; i < runGo.XYData.Xvalues.Count(); i++)
                        //{
                        //    peaks.Add(new XYData(runGo.XYData.Xvalues[i], runGo.XYData.Yvalues[i]));
                        //}

                        centroidedPeaks = new List<PNNLOmics.Data.Peak>();
                        if (runGo.ResultCollection.Run.PeakList != null)
                        {
                            for (int i = 0; i < runGo.ResultCollection.Run.PeakList.Count(); i++)
                            {
                                if (runGo.ResultCollection.Run.PeakList[i].Height > 0)
                                {
                                    //centroidedPeaks.Add(new XYData(runGo.ResultCollection.Run.PeakList[i].XValue, runGo.ResultCollection.Run.PeakList[i].Height));

                                    PNNLOmics.Data.Peak newPeak = new PNNLOmics.Data.Peak();
                                    newPeak.XValue = runGo.ResultCollection.Run.PeakList[i].XValue;
                                    newPeak.Height = runGo.ResultCollection.Run.PeakList[i].Height;
                                    newPeak.Width = runGo.ResultCollection.Run.PeakList[i].Width;
                                    centroidedPeaks.Add(newPeak);
                                }
                            }
                        }
                        objectInsideThread.CentroidedPeaksInScan = centroidedPeaks.Count;
                    }
                    break;

                #endregion
            }
            return centroidedPeaks;
        }

    }
}
