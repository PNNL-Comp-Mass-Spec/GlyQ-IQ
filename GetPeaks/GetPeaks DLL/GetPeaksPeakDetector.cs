using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using PNNLOmics.Algorithms.PeakDetection;

namespace GetPeaks_DLL
{  
    public class GetPeaksPeakDetector
    {
        public static List<ProcessedPeak> GetProcessedPeaks(List<PNNLOmics.Data.XYData> rawXYData, GetPeaksPeakDetectorParameters detectorParameters, ThresholdType thresholdMethod)
        {      
            PeakCentroider newPeakCentroider = new PeakCentroider();
            newPeakCentroider.Parameters = detectorParameters.CentroidParameters;
            List<ProcessedPeak> centroidedPeakList = newPeakCentroider.DiscoverPeaks(rawXYData);

            PeakThresholder newPeakThresholder = new PeakThresholder();
            newPeakThresholder.Parameters = detectorParameters.ThresholdParameters;


            List<ProcessedPeak> thresholdedData = new List<ProcessedPeak>();
            switch (thresholdMethod)
            {
                case ThresholdType.AveragePlusSigma:
                {
                    thresholdedData = newPeakThresholder.ApplyThreshold(centroidedPeakList);
                }
                break;
                case ThresholdType.MassFilter:
                { 

                }
                break;
                default:
                {
                    thresholdedData = newPeakThresholder.ApplyThreshold(centroidedPeakList);
                }
            break;

            }
            
            return thresholdedData;
        }
    }

    public class GetPeaksPeakDetectorParameters
    {
        public PeakCentroiderParameters CentroidParameters { get; set; }
        public PeakThresholderParameters ThresholdParameters { get; set; }
        public int ScanNumber { get; set; }//so we can attach a scan number to each peak   

        public GetPeaksPeakDetectorParameters()
        {
            this.CentroidParameters = new PeakCentroiderParameters();
            this.ThresholdParameters = new PeakThresholderParameters();
            this.ScanNumber = 0;
        }
    }

    public enum ThresholdType
    {
        AveragePlusSigma,
        MassFilter
    }
}
