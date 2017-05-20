using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend;
using DeconTools.Backend.ProcessingTasks;
using DeconTools.Backend.ProcessingTasks.ChromatogramProcessing;
using DeconTools.Backend.Core;
using DeconTools.Backend.ProcessingTasks.Smoothers;
using GetPeaks_DLL.Functions;
using IQGlyQ.Objects;
using IQGlyQ.Processors;
using PNNLOmics.Algorithms.Distance;

namespace IQGlyQ
{
    public class SimplePeakCorrelator : ChromatogramCorrelatorBase
    {
        public List<IsotopicProfile> IsoPeaklist { get; set; }

        public IsotopicProfile BasePeakIso { get; set; }

        public FragmentedTargetedWorkflowParametersIQ _workflowParameters { get; set; }

        public Processors.ProcessorChromatogram _lcProcessor { get; set; }

        //public SimplePeakCorrelator(Run run, FragmentedTargetedWorkflowParameters parameters, double minRelativeIntensityForChromCorrelation, Processors.ProcessorChromatogram lcProcessor)
        //    : base(parameters.ChromSmootherNumPointsInSmooth, parameters.ChromGenTolerance, minRelativeIntensityForChromCorrelation)
        //{
        //    _workflowParameters = (FragmentedTargetedWorkflowParametersIQ)parameters;
        //    _lcProcessor = lcProcessor;
        //}

        public SimplePeakCorrelator(Run run, FragmentedTargetedWorkflowParametersIQ parameters, double minRelativeIntensityForChromCorrelation, Processors.ProcessorChromatogram lcProcessor)
            : base(parameters.ChromSmootherNumPointsInSmooth, parameters.ChromGenTolerance, minRelativeIntensityForChromCorrelation)
        {
            _workflowParameters = parameters;
            _lcProcessor = lcProcessor;
        }



        public override ChromCorrelationData CorrelateData(Run run, TargetedResultBase result, int startScan, int stopScan)
        //public override ChromCorrelationData CorrelateData(Run run, TargetedResultBase result, int scans)
        {
            ScanObject scans = new ScanObject(startScan, stopScan);
            //scans.Start = startScan;
            //scans.Stop = stopScan;

            PeakChromatogramGenerator peakChromGen = PeakChromGen;
            SavitzkyGolaySmoother smoother = Smoother;

            ProcessorChromatogram processorChromatogram = _lcProcessor;
            CorrelationObject loadBasePeak = new CorrelationObject(BasePeakIso, scans, run, ref processorChromatogram, _workflowParameters);

            List<CorrelationObject> setupPile = new List<CorrelationObject>();
            foreach (IsotopicProfile isoToCorrelate in IsoPeaklist)
            {
                CorrelationObject loadCorrelationPeak = new CorrelationObject(isoToCorrelate, scans, run, ref processorChromatogram, _workflowParameters);
                setupPile.Add(loadCorrelationPeak);
            }

            #region check data

            bool testChromCoorelateioDataBase = loadBasePeak.AcceptableChromList[0] == true;
            List<bool> testChromCoorelateioDataPileList = new List<bool>();
            testChromCoorelateioDataPileList.Add(setupPile[0].AcceptableChromList[0] == true);
            bool testChromCoorelateioDataPile = false;
            foreach (bool test in testChromCoorelateioDataPileList)
            {
                if (test == true)
                {
                    testChromCoorelateioDataPile = true;//we only need one to make it true
                    break;
                }
            }

            if (testChromCoorelateioDataBase == true && testChromCoorelateioDataPile == true)
            {
                Console.WriteLine("we can correlate!");
            }
            else
            {
                Console.WriteLine("Missing Data");
            }

            #endregion
            //basePeakMetaData will give us an idea for what ions with the same elution profile look like. 
            //if the new ions behave like the isotopes here, it is same scan fragmentation

            #region within peak correlations

            ChromCorrelationData basePeakMetaData = new ChromCorrelationData();
            if (loadBasePeak.AreChromDataOK == true && loadBasePeak.IsosPeakListIsOK == true)
            {
                basePeakMetaData = CorrelatePeaksWithinIsotopicProfile(run, BasePeakIso, scans.Start, scans.Stop);
            }

            double rSquaredBase = Convert.ToDouble(basePeakMetaData.RSquaredValsMedian);

            ChromCorrelationData loadCorrelationPeakMetaData = new ChromCorrelationData();
            if (setupPile[0].AreChromDataOK == true && setupPile[0].IsosPeakListIsOK == true)
            {
                loadCorrelationPeakMetaData = CorrelatePeaksWithinIsotopicProfile(run, IsoPeaklist[0], scans.Start, scans.Stop);
            }

            double rSquaredCorrelationPeak = Convert.ToDouble(loadCorrelationPeakMetaData.RSquaredValsMedian);

            //these are the R values from within the isotope profiles!
            List<double> rValues = new List<double>();
            rValues.Add(rSquaredBase);
            rValues.Add(rSquaredCorrelationPeak);

            List<ChromCorrelationData> results = new List<ChromCorrelationData>();
            results.Add(basePeakMetaData);
            results.Add(loadCorrelationPeakMetaData);

            #endregion

            #region between peak correlations

            ChromCorrelationData correlationData = new ChromCorrelationData();
            if (loadBasePeak.AreChromDataOK == true && loadBasePeak.IsosPeakListIsOK == true)
            {
                double slope;
                double intercept;
                double rsquaredVal;

                //this data is not the same as before
                XYData basePeakChromXYData = loadBasePeak.PeakChromXYData[loadBasePeak.IndexMostAbundantPeak];
                XYData chromPeakXYData = setupPile[0].PeakChromXYData[setupPile[0].IndexMostAbundantPeak];

                chromPeakXYData = FillInAnyMissingValuesInChromatogram(basePeakChromXYData, chromPeakXYData);

                GetElutionCorrelationData(basePeakChromXYData, chromPeakXYData, out slope, out intercept, out rsquaredVal);

                correlationData.AddCorrelationData(slope, intercept, rsquaredVal);
            }



            #endregion
            //manwhitny U test for small N


            //now we have a base chromatogram +  a peak list to correlate to it
            //for (int i = 0; i < Peaklist.Count; i++)
            //{
            //    IsotopicProfile correlatedProfile = Peaklist[i];

            //    #region inside
            //    int correlatedindexMostAbundantPeak = correlatedProfile.GetIndexOfMostIntensePeak();

            //    double correlatedMZValue = correlatedProfile.Peaklist[correlatedindexMostAbundantPeak].Height;

            //    XYData chromPeakXYData = GetCorrelatedChromPeakXYData(run, startScan, stopScan, basePeakChromXYData, correlatedMZValue);

            //    bool chromDataIsOK = chromPeakXYData != null && chromPeakXYData.Xvalues != null && chromPeakXYData.Xvalues.Length > 3;

            //    bool baseCoorelatedIsosPeakIsOK = correlatedProfile.Peaklist.Count > 3;//this is needed for statistics

            //    if (Peaklist[i].Height >= minBaseIntensity)
            //    {

            //        if (chromDataIsOK)
            //        {
            //            double slope;
            //            double intercept;
            //            double rsquaredVal;

            //            chromPeakXYData = FillInAnyMissingValuesInChromatogram(basePeakChromXYData, chromPeakXYData);

            //            GetElutionCorrelationData(basePeakChromXYData, chromPeakXYData, out slope, out intercept, out rsquaredVal);

            //            correlationData.AddCorrelationData(slope, intercept, rsquaredVal);
            //        }
            //        else
            //        {
            //            ChromCorrelationDataItem defaultChromCorrDataItem = new ChromCorrelationDataItem();
            //            correlationData.AddCorrelationData(defaultChromCorrDataItem);
            //        }
            //    }
            //    else
            //    {
            //        var defaultChromCorrDataItem = new ChromCorrelationDataItem();
            //        correlationData.AddCorrelationData(defaultChromCorrDataItem);
            //    }

            //    #endregion
            //}



            return correlationData;
        }

        public ChromCorrelationData CorrelateDataXY(List<PNNLOmics.Data.XYData> data1, List<PNNLOmics.Data.XYData> data2, int startScan, int stopScan)
        {
            PeakChromatogramGenerator peakChromGen = PeakChromGen;
            SavitzkyGolaySmoother smoother = Smoother;



            #region between peak correlations

            ChromCorrelationData correlationData = new ChromCorrelationData();
            if (data1.Count == data2.Count)
            {
                double slope;
                double intercept;
                double rsquaredVal;

                //this data is not the same as before
                XYData basePeakChromXYData = ConvertXYData.OmicsXYDataToRunXYData(data1);
                XYData chromPeakXYData = ConvertXYData.OmicsXYDataToRunXYData(data2);

                chromPeakXYData = FillInAnyMissingValuesInChromatogram(basePeakChromXYData, chromPeakXYData);

                GetElutionCorrelationData(basePeakChromXYData, chromPeakXYData, out slope, out intercept, out rsquaredVal);

                correlationData.AddCorrelationData(slope, intercept, rsquaredVal);
            }



            #endregion
            //manwhitny U test for small N


            //now we have a base chromatogram +  a peak list to correlate to it
            //for (int i = 0; i < Peaklist.Count; i++)
            //{
            //    IsotopicProfile correlatedProfile = Peaklist[i];

            //    #region inside
            //    int correlatedindexMostAbundantPeak = correlatedProfile.GetIndexOfMostIntensePeak();

            //    double correlatedMZValue = correlatedProfile.Peaklist[correlatedindexMostAbundantPeak].Height;

            //    XYData chromPeakXYData = GetCorrelatedChromPeakXYData(run, startScan, stopScan, basePeakChromXYData, correlatedMZValue);

            //    bool chromDataIsOK = chromPeakXYData != null && chromPeakXYData.Xvalues != null && chromPeakXYData.Xvalues.Length > 3;

            //    bool baseCoorelatedIsosPeakIsOK = correlatedProfile.Peaklist.Count > 3;//this is needed for statistics

            //    if (Peaklist[i].Height >= minBaseIntensity)
            //    {

            //        if (chromDataIsOK)
            //        {
            //            double slope;
            //            double intercept;
            //            double rsquaredVal;

            //            chromPeakXYData = FillInAnyMissingValuesInChromatogram(basePeakChromXYData, chromPeakXYData);

            //            GetElutionCorrelationData(basePeakChromXYData, chromPeakXYData, out slope, out intercept, out rsquaredVal);

            //            correlationData.AddCorrelationData(slope, intercept, rsquaredVal);
            //        }
            //        else
            //        {
            //            ChromCorrelationDataItem defaultChromCorrDataItem = new ChromCorrelationDataItem();
            //            correlationData.AddCorrelationData(defaultChromCorrDataItem);
            //        }
            //    }
            //    else
            //    {
            //        var defaultChromCorrDataItem = new ChromCorrelationDataItem();
            //        correlationData.AddCorrelationData(defaultChromCorrDataItem);
            //    }

            //    #endregion
            //}



            return correlationData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <param name="startScan"></param>
        /// <param name="stopScan"></param>
        /// <returns></returns>
        public ChromCorrelationData CorrelateDataXYNonInteger(List<PNNLOmics.Data.XYData> data1, List<PNNLOmics.Data.XYData> data2, int startScan, int stopScan)
        {
            PeakChromatogramGenerator peakChromGen = PeakChromGen;
            SavitzkyGolaySmoother smoother = Smoother;



            #region between peak correlations

            ChromCorrelationData correlationData = new ChromCorrelationData();
            if (data1.Count == data2.Count)
            {
                double slope;
                double intercept;
                double rsquaredVal;

                //this data is not the same as before
                XYData basePeakChromXYData = ConvertXYData.OmicsXYDataToRunXYData(data1);
                XYData chromPeakXYData = ConvertXYData.OmicsXYDataToRunXYData(data2);

                //this does not work on non integer scans//chromPeakXYData = FillInAnyMissingValuesInChromatogram(basePeakChromXYData, chromPeakXYData);

                

                GetElutionCorrelationData(basePeakChromXYData, chromPeakXYData, out slope, out intercept, out rsquaredVal);

                correlationData.AddCorrelationData(slope, intercept, rsquaredVal);
            }



            #endregion
            //manwhitny U test for small N


            //now we have a base chromatogram +  a peak list to correlate to it
            //for (int i = 0; i < Peaklist.Count; i++)
            //{
            //    IsotopicProfile correlatedProfile = Peaklist[i];

            //    #region inside
            //    int correlatedindexMostAbundantPeak = correlatedProfile.GetIndexOfMostIntensePeak();

            //    double correlatedMZValue = correlatedProfile.Peaklist[correlatedindexMostAbundantPeak].Height;

            //    XYData chromPeakXYData = GetCorrelatedChromPeakXYData(run, startScan, stopScan, basePeakChromXYData, correlatedMZValue);

            //    bool chromDataIsOK = chromPeakXYData != null && chromPeakXYData.Xvalues != null && chromPeakXYData.Xvalues.Length > 3;

            //    bool baseCoorelatedIsosPeakIsOK = correlatedProfile.Peaklist.Count > 3;//this is needed for statistics

            //    if (Peaklist[i].Height >= minBaseIntensity)
            //    {

            //        if (chromDataIsOK)
            //        {
            //            double slope;
            //            double intercept;
            //            double rsquaredVal;

            //            chromPeakXYData = FillInAnyMissingValuesInChromatogram(basePeakChromXYData, chromPeakXYData);

            //            GetElutionCorrelationData(basePeakChromXYData, chromPeakXYData, out slope, out intercept, out rsquaredVal);

            //            correlationData.AddCorrelationData(slope, intercept, rsquaredVal);
            //        }
            //        else
            //        {
            //            ChromCorrelationDataItem defaultChromCorrDataItem = new ChromCorrelationDataItem();
            //            correlationData.AddCorrelationData(defaultChromCorrDataItem);
            //        }
            //    }
            //    else
            //    {
            //        var defaultChromCorrDataItem = new ChromCorrelationDataItem();
            //        correlationData.AddCorrelationData(defaultChromCorrDataItem);
            //    }

            //    #endregion
            //}



            return correlationData;
        }
    
    }
}
