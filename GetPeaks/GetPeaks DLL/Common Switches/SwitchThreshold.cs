using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using GetPeaks_DLL.PNNLOmics_Modules;
using PNNLOmics.Algorithms.PeakDetection;

namespace GetPeaks_DLL.Common_Switches
{
    public class SwitchThreshold:IDisposable
    {
        /// <summary>
        /// Gets or sets the peak centroider parameters.
        /// </summary>
        public PeakThresholderParameters Parameters { get; set; }

        /// <summary>
        /// Gets or sets the peak centroider parameters.
        /// </summary>
        public OrbitrapFilterParameters ParametersOrbitrap { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SwitchThreshold()
        {
            this.Parameters = new PeakThresholderParameters();
            this.ParametersOrbitrap = new OrbitrapFilterParameters();
        }
        
        public List<ProcessedPeak> ThresholdNow(ref List<ProcessedPeak> DiscoveredPeakList)
        {
            
            List<ProcessedPeak> thresholdedData = new List<ProcessedPeak>();

            PeakThresholder newPeakThresholder = new PeakThresholder();
            newPeakThresholder.Parameters = Parameters;

            switch (Parameters.DataNoiseType)
            {
                case InstrumentDataNoiseType.Standard:
                    {
                        thresholdedData = newPeakThresholder.ApplyThreshold(DiscoveredPeakList);
                    }
                    break;
                case InstrumentDataNoiseType.NoiseRemoved:
                    {
                        //Parameters.isDataThresholded = false;//pre filter//this should be controlled outside
                        //thresholdedData = newPeakThresholder.ApplyThreshold(DiscoveredPeakList);
                        thresholdedData = DiscoveredPeakList;
                        OrbitrapThreshold newOrbitrapThreshold = new OrbitrapThreshold();
                        newOrbitrapThreshold.Parameters = Parameters;
                        newOrbitrapThreshold.ParametersOrbitrap = ParametersOrbitrap;
                        //newOrbitrapThreshold.ParametersOrbitrap.DeltaMassTollerancePPM = 6000;// this is an important parameter
                        //newOrbitrapThreshold.ParametersOrbitrap.massNeutron = 1.002149286;//SN09// this is an important parameter
                        thresholdedData = newOrbitrapThreshold.ApplyThreshold(ref thresholdedData);
                    }
                    break;
                default:
                    {
                        thresholdedData = newPeakThresholder.ApplyThreshold(DiscoveredPeakList);
                    }
                    break;
            }

            
            return thresholdedData;
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.ParametersOrbitrap.Dispose();
            this.ParametersOrbitrap = null;
            this.Parameters = null;
        }

        #endregion
    }
}
