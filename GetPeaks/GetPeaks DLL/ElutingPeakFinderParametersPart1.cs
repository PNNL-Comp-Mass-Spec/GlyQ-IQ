using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Algorithms.PeakDetection;

namespace GetPeaks_DLL.PNNLOmics_Modules
{

    [Serializable]
    public class ElutingPeakFinderParametersPart1:IDisposable
    {
        public InstrumentDataNoiseType NoiseType { get; set; }

        /// <summary>
        /// sigma above the noise?
        /// </summary>
        public double ElutingPeakNoiseThreshold { get; set; }

        /// <summary>
        /// should we apply a global average + x sigma threshold
        /// </summary>
        public bool isDataAlreadyThresholded { get; set; }

        public int StartScan { get; set; }

        public int StopScan { get; set; }

        /// <summary>
        /// filtering data based on mass instead of intensity threshold
        /// </summary>
        public OrbitrapFilterParameters ParametersOrbitrap { get; set; }

        /// <summary>
        /// CompareContrastTolerance
        /// </summary>
        public double AllignmentToleranceInPPM { get; set; }

        /// <summary>
        /// Peak fit type for decon tools
        /// </summary>
        public DeconTools.Backend.Globals.PeakFitType PeakFitType { get; set; }

        /// <summary>
        /// Decon Tools peak Detector
        /// </summary>
        public double MSPeakDetectorPeakBR { get; set; }
       
        /// <summary>
        /// store largest peak detected within an eluting peak
        /// </summary>
        public double MaxHeightForNewPeak { get; set; }

        /// <summary>
        /// number of scans to sum for part 1.  must use odd numbers.  1 means no summing, 3 means summing one scan before and one scan after
        /// </summary>
        public int ScansToBeSummed { get; set; }

        /// <summary>
        /// true means we will only add scans with a level of 1.  false will add all scans
        /// </summary>
        public bool MSLevelOnly { get; set; }


        public ElutingPeakFinderParametersPart1()
        {
            NoiseType = InstrumentDataNoiseType.Standard;
            ElutingPeakNoiseThreshold = 5;
            isDataAlreadyThresholded = false;
            AllignmentToleranceInPPM = 14;
            MaxHeightForNewPeak = 0;//0-1 range, a dip to 75% of maximum is required to start looking for a second peak
            StartScan = 0;
            StopScan = 100;
            PeakFitType = DeconTools.Backend.Globals.PeakFitType.APEX;//default
            ParametersOrbitrap = new OrbitrapFilterParameters();
            MSPeakDetectorPeakBR = 2;
            ScansToBeSummed = 1;
            MSLevelOnly = false;     
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.ParametersOrbitrap.Dispose();
        }

        #endregion
    }

    
}
