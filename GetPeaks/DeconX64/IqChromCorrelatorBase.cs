﻿using System;
using System.Collections.Generic;
using System.Linq;
using DeconTools.Backend;
using DeconTools.Backend.Core;
using DeconTools.Backend.ProcessingTasks;
using DeconTools.Backend.ProcessingTasks.Smoothers;
using DeconTools.Backend.Utilities;
using DeconTools.Utilities;
using DeconX64;

namespace DeconTools.Workflows.Backend.Core.ChromPeakSelection
{
    public abstract class IqChromCorrelatorBase
    {
        protected SavitzkyGolaySmoother Smoother;
        protected PeakChromatogramGenerator PeakChromGen;

        #region Constructors

        protected IqChromCorrelatorBase(int numPointsInSmoother, double minRelativeIntensityForChromCorr,
            double chromTolerance, DeconTools.Backend.Globals.ToleranceUnit toleranceUnit= DeconTools.Backend.Globals.ToleranceUnit.PPM)
        {
            SavitzkyGolaySmoothingOrder = 2;
            NumPointsInSmoother = numPointsInSmoother;

            ChromTolerance = chromTolerance;
            MinimumRelativeIntensityForChromCorr = minRelativeIntensityForChromCorr;

            ChromToleranceUnit = toleranceUnit;

            PeakChromGen = new PeakChromatogramGenerator(ChromTolerance, DeconTools.Backend.Globals.ChromatogramGeneratorMode.MOST_ABUNDANT_PEAK,
                                                         DeconTools.Backend.Globals.IsotopicProfileType.UNLABELLED, toleranceUnit);


            Smoother = new SavitzkyGolaySmoother(NumPointsInSmoother, SavitzkyGolaySmoothingOrder, false);
        }

        
        #endregion

        #region Properties

        private double _chromTolerance;
        public double ChromTolerance
        {
            get { return _chromTolerance; }
            set
            {
                _chromTolerance = value;
                
                if (PeakChromGen!=null)
                {
                    PeakChromGen = new PeakChromatogramGenerator(ChromTolerance, DeconTools.Backend.Globals.ChromatogramGeneratorMode.MOST_ABUNDANT_PEAK,
                                                                DeconTools.Backend.Globals.IsotopicProfileType.UNLABELLED, ChromToleranceUnit);
                }
                
                
            }
        }

        /// <summary>
        /// Tolerence unit for chromatogram. Either PPM (default) or MZ. Can only be set in the class constructor
        /// </summary>
        public DeconTools.Backend.Globals.ToleranceUnit ChromToleranceUnit { get; private set; }



        public double MinimumRelativeIntensityForChromCorr { get; set; }

        private int _numPointsInSmoother;
        public int NumPointsInSmoother
        {
            get { return _numPointsInSmoother; }
            set
            {
                if (_numPointsInSmoother != value)
                {

                    _numPointsInSmoother = value;

                    if (Smoother != null)
                    {
                        Smoother = new SavitzkyGolaySmoother(_numPointsInSmoother, SavitzkyGolaySmoothingOrder);
                    }

                }

            }
        }

        private int _savitzkyGolaySmoothingOrder;
        public int SavitzkyGolaySmoothingOrder
        {
            get { return _savitzkyGolaySmoothingOrder; }
            set
            {
                if (_savitzkyGolaySmoothingOrder != value)
                {
                    _savitzkyGolaySmoothingOrder = value;

                    if (Smoother!=null)
                    {
                        Smoother = new SavitzkyGolaySmoother(NumPointsInSmoother, _savitzkyGolaySmoothingOrder);    
                    }
                    
                }

            }
        }

        #endregion

        #region Public Methods

        public void GetElutionCorrelationData(XYData chromData1, XYData chromData2, out double slope, out double intercept, out double rsquaredVal)
        {
            Check.Require(chromData1 != null && chromData1.Xvalues != null, "Chromatogram1 intensities are null");
            Check.Require(chromData2 != null && chromData2.Xvalues != null, "Chromatogram2 intensities are null");

            Check.Require(chromData1.Xvalues[0] == chromData2.Xvalues[0], "Correlation failed. Chromatograms being correlated do not have the same scan values!");

            GetElutionCorrelationData(chromData1.Yvalues, chromData2.Yvalues, out slope, out intercept, out rsquaredVal);
        }

        public void GetElutionCorrelationData(double[] chromIntensities1, double[] chromIntensities2, out double slope, out double intercept, out double rsquaredVal)
        {
            Check.Require(chromIntensities1 != null, "Chromatogram1 intensities are null");
            Check.Require(chromIntensities2 != null, "Chromatogram2 intensities are null");

            Check.Require(chromIntensities1.Length == chromIntensities2.Length, "Correlation failed. Chromatogram1 and Chromatogram2 must be the same length");

            slope = -9999;
            intercept = -9999;
            rsquaredVal = -1;

            MathUtils.GetLinearRegression(chromIntensities1, chromIntensities2, out slope, out intercept, out rsquaredVal);
        }


        #endregion

        #region Private Methods

        #endregion


        public virtual ChromCorrelationData CorrelateData(Run run, IqResult iqResult, int startScan, int stopScan)
        {
            throw new NotImplementedException();
        }
       

        public ChromCorrelationData CorrelatePeaksWithinIsotopicProfile(Run run, IsotopicProfile iso, int startScan, int stopScan)
        {
            var correlationData = new ChromCorrelationData();
            int indexMostAbundantPeak = iso.GetIndexOfMostIntensePeak();

            double baseMZValue = iso.Peaklist[indexMostAbundantPeak].XValue;
            bool baseChromDataIsOK;
            var basePeakChromXYData = GetBaseChromXYData(run, startScan, stopScan, baseMZValue);

            baseChromDataIsOK = basePeakChromXYData != null && basePeakChromXYData.Xvalues != null; 
                //&&basePeakChromXYData.Xvalues.Length > 3;


            double minIntensity = iso.Peaklist[indexMostAbundantPeak].Height *
                                 MinimumRelativeIntensityForChromCorr;


            for (int i = 0; i < iso.Peaklist.Count; i++)
            {
                if (!baseChromDataIsOK)
                {
                    var defaultChromCorrDataItem = new ChromCorrelationDataItem();
                    correlationData.AddCorrelationData(defaultChromCorrDataItem);
                    break;
                }

                if (i == indexMostAbundantPeak)
                {
                    //peak is being correlated to itself
                    correlationData.AddCorrelationData(1.0, 0, 1);


                }
                else if (iso.Peaklist[i].Height >= minIntensity)
                {
                    double correlatedMZValue = iso.Peaklist[i].XValue;
                    bool chromDataIsOK;
                    var chromPeakXYData = GetCorrelatedChromPeakXYData(run, startScan, stopScan, basePeakChromXYData, correlatedMZValue);

                    chromDataIsOK = chromPeakXYData != null && chromPeakXYData.Xvalues != null;
                        //&&chromPeakXYData.Xvalues.Length > 3;

                    if (chromDataIsOK)
                    {
                        double slope;
                        double intercept;
                        double rsquaredVal;

                        chromPeakXYData = FillInAnyMissingValuesInChromatogram(basePeakChromXYData.Xvalues, chromPeakXYData);


                        GetElutionCorrelationData(basePeakChromXYData, chromPeakXYData,
                                                                          out slope, out intercept, out rsquaredVal);

                        correlationData.AddCorrelationData(slope, intercept, rsquaredVal);

                    }
                    else
                    {

                        var defaultChromCorrDataItem = new ChromCorrelationDataItem();
                        correlationData.AddCorrelationData(defaultChromCorrDataItem);
                    }

                }
                else
                {
                    var defaultChromCorrDataItem = new ChromCorrelationDataItem();
                    correlationData.AddCorrelationData(defaultChromCorrDataItem);
                }
            }

            return correlationData;
        }

        protected XYData GetCorrelatedChromPeakXYData(Run run, int startScan, int stopScan, XYData basePeakChromXYData, double correlatedMZValue)
        {
            var xydata= PeakChromGen.GenerateChromatogram(run, startScan, stopScan, correlatedMZValue, ChromTolerance,ChromToleranceUnit);

            XYData chromPeakXYData;
            if (xydata == null || xydata.Xvalues.Length == 0)
            {
                chromPeakXYData = new XYData();
                chromPeakXYData.Xvalues = basePeakChromXYData.Xvalues;
                chromPeakXYData.Yvalues = new double[basePeakChromXYData.Xvalues.Length];
            }
            else
            {
                chromPeakXYData = Smoother.Smooth(xydata);
            }


            var chromDataIsOK = chromPeakXYData != null && chromPeakXYData.Xvalues != null &&
                                chromPeakXYData.Xvalues.Length > 3;

            if (chromDataIsOK)
            {
                chromPeakXYData = chromPeakXYData.TrimData(startScan, stopScan);
            }
            return chromPeakXYData;
        }

        public XYData GetBaseChromXYData(Run run, int startScan, int stopScan, double baseMZValue)
        {
            //note: currently 'GenerateChromatogram' results in 0's being padded on both sides of start and stop scans.
            var xydata=   PeakChromGen.GenerateChromatogram(run, startScan, stopScan, baseMZValue, ChromTolerance,ChromToleranceUnit);

            XYData basePeakChromXYData;
            if (xydata == null || xydata.Xvalues.Length < 3)
            {
                basePeakChromXYData = new XYData();
                
                if (xydata==null)
                {
                    basePeakChromXYData.Xvalues=new double[0];
                    basePeakChromXYData.Yvalues = new double[0];
                }
                else
                {
                    basePeakChromXYData.Xvalues = xydata.Xvalues;
                    basePeakChromXYData.Yvalues = xydata.Yvalues;
                }
            }
            else
            {
                basePeakChromXYData = Smoother.Smooth(xydata);
            }    
            
            ScanSetCollection scanSetCollection =new ScanSetCollection();
            scanSetCollection.Create(run, startScan, stopScan, 1, 1, false);

            var validScanNums = scanSetCollection.ScanSetList.Select(p => (double)p.PrimaryScanNumber).ToArray();

            basePeakChromXYData = FillInAnyMissingValuesInChromatogram(validScanNums, basePeakChromXYData);
            return basePeakChromXYData;
        }


        /// <summary>
        /// Fills in any missing data in the chrom data being correlated. 
        /// This ensures base chrom data and the correlated chrom data are the same length
        /// </summary>
        /// <param name="scanList"> </param>
        /// <param name="chromPeakXyData"></param>
        /// <returns></returns>
        protected XYData FillInAnyMissingValuesInChromatogram(double[] scanList, XYData chromPeakXyData)
        {
            if (scanList == null || scanList.Length == 0) return null;

            var filledInData = new SortedDictionary<int, double>();

            //first fill with zeros
            for (int i = 0; i < scanList.Length; i++)
            {
                filledInData.Add((int)scanList[i], 0);
            }

            if (chromPeakXyData==null)
            {
                chromPeakXyData=new XYData();
            }

            if (chromPeakXyData.Xvalues==null)
            {
                chromPeakXyData.Xvalues =new double[0];
                chromPeakXyData.Yvalues=new double[0];
            }

            //then fill in other values
            for (int i = 0; i < chromPeakXyData.Xvalues.Length; i++)
            {
                int currentScan = (int)chromPeakXyData.Xvalues[i];
                if (filledInData.ContainsKey(currentScan))
                {
                    filledInData[currentScan] = chromPeakXyData.Yvalues[i];
                }
            }

            var xydata = new XYData { Xvalues = scanList, Yvalues = filledInData.Values.ToArray() };

            return xydata;


        }
    }
}