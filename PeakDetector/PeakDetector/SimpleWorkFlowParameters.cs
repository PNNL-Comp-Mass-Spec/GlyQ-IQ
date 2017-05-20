using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL
{
    public class SimpleWorkflowParameters
    {
        #region Constructors

            public SimpleWorkflowParameters()
            {
                this.AllignmentToleranceInPPM = 25;
                this.MSPeakDetectorPeakBR = 2;
                this.MSPeakDetectorSigNoise = 2;
                this.StartScan = 0;
                this.StopScan = 100;
                this.DynamicRangeToOne = 5000;
                this.MaxScanSpread = 1000;
                this.MaxHeightForNewPeak = 0;//0-1 range, a dip to 75% of maximum is required to start looking for a second peak
                this.Multithread = false;
                this.PeakFitType = DeconTools.Backend.Globals.PeakFitType.APEX;//default
                this.DeconvolutionType = DeconvolutionType.Rapid;//default
            }

        #endregion

        #region Properties
            /// <summary>
            /// CompareContrastTolerance
            /// </summary>
            public int AllignmentToleranceInPPM { get; set; }
            
            /// <summary>
            /// Decon Tools peak Detector
            /// </summary>
            public double MSPeakDetectorPeakBR { get; set; }

            /// <summary>
            /// DeconTools peak detector
            /// </summary>
            public double MSPeakDetectorSigNoise { get; set; }

            public int StartScan { get; set; }

            public int StopScan { get; set; }

            /// <summary>
            /// possible cut off range for stopping eluting peak
            /// </summary>
            public int DynamicRangeToOne { get; set; }

            /// <summary>
            /// possible cut off ranage for stopping eluting peak
            /// </summary>
            public int MaxScanSpread { get; set; }

            /// <summary>
            /// store largest peak detected within an eluting peak
            /// </summary>
            public double MaxHeightForNewPeak { get; set; }

            /// <summary>
            /// optional multithreading of eluting peak processing
            /// </summary>
            public bool Multithread { get; set; }

            /// <summary>
            /// Peak fit type for decon tools
            /// </summary>
            public DeconTools.Backend.Globals.PeakFitType PeakFitType {get;set;}

            /// <summary>
            /// deconvolution type for decon tools
            /// </summary>
            public DeconvolutionType DeconvolutionType { get; set; }

        #endregion
    }

    public enum DeconvolutionType
    { 
        Thrash,
        Rapid
    }
}
