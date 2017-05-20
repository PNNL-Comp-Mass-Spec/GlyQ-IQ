using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace YAFMS_DB.PNNLOmics
{
    //TODO: Change name of processed peak to something more specific? like MSPeak
    public class ProcessedPeakYAFMS: Peak
    {
    //    //TODO set up new peak object so we can return a list
    //    //TODO break up regions into functions
        

        /// <summary>
        /// the lower of the two local minima (lowest between the minima lower in mass and the minima higher in mass)
        /// </summary>
        public double LocalLowestMinimaHeight { get; set; }

        /// <summary>
        /// the higher of the two local minima (highest between the minima lower in mass and the minima higher in mass)
        /// </summary>
        public double LocalHighestMinimaHeight { get; set; }
        
        /// <summary>
        /// the closes minima on the lower mass side of the peak has this index.
        /// </summary>
        public int MinimaOfLowerMassIndex { get; set; }

        /// <summary>
        /// the closes minima on the higher mass side of the peak has this index.
        /// </summary>
        public int MinimaOfHigherMassIndex { get; set; }

        /// <summary>
        /// the closes index to the apex on the left.  The closese index to the apex on the right is +1 point over
        /// </summary>
        public int CenterIndexLeft { get; set; }

        /// <summary>
        /// Maxintensity/noise threshold (Xsigma above the average noise)
        /// </summary>
        public double SignalToNoiseGlobal { get; set; }

        /// <summary>
        /// Maxintensity/local lowest minima
        /// </summary>
        public double SignalToNoiseLocalHighestMinima { get; set; }

        /// <summary>
        /// Maxintensity/average of local minima
        /// </summary>
        public double SignalToBackground { get; set; }

        /// <summary>
        /// Charge state based on nearby peaks
        /// </summary>
        public int Charge { get; set; }

        /// <summary>
        /// the Scan Number this peak was found in.
        /// </summary>
        public int ScanNumber { get; set; }

        //TODO: Remove the charge and scannumber and make them part of the feature.
        //public MSFeatureLight Feature {get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ProcessedPeakYAFMS()
        {
        }

        /// <summary>
        /// simple constructor
        /// </summary>
        /// <param name="xValue">x value</param>
        /// <param name="height">y value</param>
        public ProcessedPeakYAFMS(double xValue, double height)
        {
            XValue = xValue;
            Height = height;
        }

        /// <summary>
        /// simple constructor
        /// </summary>
        /// <param name="xValue">x value</param>
        /// <param name="height">y value</param>
        /// <param name="scan">center scan where peak was found</param>
        public ProcessedPeakYAFMS(double xValue, double height, int scan)
            :this(xValue,height)
        {
            ScanNumber = scan;
        }

        public override void Clear()
        {            
            this.ScanNumber = 0;
            this.LocalLowestMinimaHeight = 0;
            this.MinimaOfHigherMassIndex = 0;
            this.MinimaOfLowerMassIndex = 0;
        }

        //TODO: Change List to Collection?
        /// <summary>
        /// Peak is the standard object for the output collection and we need to convert processed peak lists.  
        /// </summary>
        /// <param name="processedPeakList">list of processed peaks</param>
        /// <returns>list of peaks</returns>
        public static Collection<Peak> ToPeaks(List<ProcessedPeakYAFMS> peaks)
        {
            Collection<Peak> outputPeakList = new Collection<Peak>();

            foreach (ProcessedPeakYAFMS inPeak in peaks)
            {
                Peak newPeak = new Peak();
                newPeak.Height = inPeak.Height;
                newPeak.LocalSignalToNoise = Convert.ToSingle(inPeak.SignalToBackground);
                newPeak.Width = inPeak.Width;
                newPeak.XValue = inPeak.XValue;
                outputPeakList.Add(newPeak);
            }

            return outputPeakList;
        }        
    }
}
