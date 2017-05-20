using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Algorithms.PeakDetection;
using GetPeaks_DLL.PNNLOmics_Modules;
using GetPeaks_DLL.Objects;

namespace GetPeaks_DLL
{
    [Serializable]
    public class ElutingPeakFinderParametersPart2:IDisposable
    {
        /// <summary>
        /// Decon Tools peak Detector
        /// </summary>
        public double MSPeakDetectorPeakBR { get; set; }

        /// <summary>
        /// DeconTools peak detector
        /// </summary>
        public double MSPeakDetectorSigNoise { get; set; }

        public OrbitrapFilterParameters ParametersOrbitrap { get; set; }

        /// <summary>
        /// deconvolution type for decon tools
        /// </summary>
        public DeconvolutionType DeconvolutionType { get; set; }


        /// <summary>
        /// Peak fit type for decon tools
        /// </summary>
        public DeconTools.Backend.Globals.PeakFitType PeakFitType { get; set; }

        /// <summary>
        /// optional multithreading of eluting peak processing
        /// </summary>
        public bool Multithread { get; set; }

        /// <summary>
        /// possible cut off range for stopping eluting peak
        /// </summary>
        public int DynamicRangeToOne { get; set; }

        /// <summary>
        /// possible cut off ranage for stopping eluting peak
        /// </summary>
        public int MaxScanSpread { get; set; }

        /// <summary>
        /// Is the noise removed?  Orbitrap setting is to use the noise removed option
        /// </summary>
        public InstrumentDataNoiseType NoiseType { get; set; }

        /// <summary>
        /// how to break up Part 2 so we don't run out of memory
        /// </summary>
        public MemorySplitObject MemoryDivider { get; set; }

        /// <summary>
        /// is the data already thresholded.  This is especially usefull for the obritrap filter.  Some cases you do not want an additional horizon cuttoff such as msms
        /// </summary>
        public bool isDataThresholded { get; set; }

        public int numberOfDeconvolutionThreads { get; set; }

        /// <summary>
        /// true means we will only add scans with a level of 1.  false will add all scans
        /// </summary>
        public bool MSLevelOnly { get; set; }

        public ElutingPeakFinderParametersPart2()
        {
            Multithread = false;
            MSPeakDetectorPeakBR = 2;
            MSPeakDetectorSigNoise = 2;
            PeakFitType = DeconTools.Backend.Globals.PeakFitType.APEX;//default
            DeconvolutionType = DeconvolutionType.Rapid;//default 
            DynamicRangeToOne = 5000;
            MaxScanSpread = 1000;
            NoiseType = InstrumentDataNoiseType.Standard;
            ParametersOrbitrap = new OrbitrapFilterParameters();
            MemoryDivider = new MemorySplitObject();
            isDataThresholded = false;
            MSLevelOnly = false;
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.ParametersOrbitrap.Dispose();
            this.ParametersOrbitrap = null;
            this.MemoryDivider = null;
        }

        #endregion
    }
}
