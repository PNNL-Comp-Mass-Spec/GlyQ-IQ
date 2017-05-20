using GetPeaks_DLL.Go_Decon_Modules;
using GetPeaks_DLL.Objects.Enumerations;
using HammerPeakDetector;
using DeconTools.Backend.Parameters;

namespace OrbitrapPeakDetection.Getpeaks
{
    public class ParametersPeakDetection
    {
        /// <summary>
        /// Orbitrap Filtering Parameters
        /// </summary>
        public HammerPeakDetector.HammerThresholdParameters HammerParameters { get; set; }

        /// <summary>
        /// scotts version of decon tools
        /// </summary>
        public ParametersDeconToolsPeakDetection DeconToolsPeakDetection { get; set; }

        /// <summary>
        /// deconVersion
        /// </summary>
        public DeconTools.Backend.Parameters.PeakDetectorParameters DeconToolsPeakDetectionDeconVersion { get; set; }
         
        /// <summary>
        /// how mant scans to sum.  I am not sure what happens with the tandem scans yet
        /// </summary>
        public int ScansToSum { get; set; }

        /// <summary>
        /// limits the data set to this scan.  perhaps this is not needed
        /// </summary>
        public int MaxScanLimitation { get; set; }


        /// <summary>
        /// true will just work on MS1 if you use the decontools scan set creator.  I think scotts version does not chare
        /// </summary>
        public bool ProcessMsms { get; set; }

        /// <summary>
        /// true works if the dat has allready been processed.  Since this is a peak detector, this is set to false
        /// </summary>
        public bool isDataThresholdedDecon { get; set; }//perhaps false for TOF and True for Orbitrap.  False has a lower threshold allowing for more hits and more false postiives

        /// <summary>
        /// this turn off the centroiding block and only runs the peak detector.  the centroiding block is normally on
        /// </summary>
        public bool shouldWeApplyCentroidToTandemHammer { get; set; }

        /// <summary>
        /// place to hold the scanssets so we only have one copy
        /// </summary>
        public DeconTools.Backend.Core.ScanSetCollection EngineScanSetCollection { get; set; }

        /// <summary>
        /// decon or hammer
        /// </summary>
        public PeakDetectors PeakDetectionMethod { get; set; }

        /// <summary>
        /// centroid or profile
        /// </summary>
        public SpectraCollectionMode SpectraCollectionModePrecursor { get; set; }

        /// <summary>
        /// centroid or profile
        /// </summary>
        public SpectraCollectionMode SpectraCollectionModeTandem { get; set; }
        
        public ParametersPeakDetection()
        {
            HammerParameters = new HammerThresholdParameters();
            DeconToolsPeakDetection = new ParametersDeconToolsPeakDetection();
            DeconToolsPeakDetectionDeconVersion = new PeakDetectorParameters();
            isDataThresholdedDecon = false;
            PeakDetectionMethod = PeakDetectors.DeconTools;
            ProcessMsms = false;
            shouldWeApplyCentroidToTandemHammer = true;
            SpectraCollectionModePrecursor = SpectraCollectionMode.Profile;
            SpectraCollectionModeTandem = SpectraCollectionMode.Profile;
        }
    }
}
