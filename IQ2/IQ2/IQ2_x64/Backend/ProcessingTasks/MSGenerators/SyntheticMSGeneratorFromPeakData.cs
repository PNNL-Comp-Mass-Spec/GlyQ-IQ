using System;
using System.Collections.Generic;
 


using IQ_X64.Backend.Utilities.IsotopeDistributionCalculation;
using Run64.Backend.Core;
using Run64.Backend.Data;
using Run64.Utilities;

namespace IQ_X64.Backend.ProcessingTasks.MSGenerators
{
    public enum SyntheticMSGeneratorFromPeakDataMode
    {
        WidthsCalculatedFromSingleValue,
        WidthsCalculatedOnAPerPeakBasis
    }


    public class SyntheticMSGeneratorFromPeakData : MSGenerator
    {
        #region Constructors
        public SyntheticMSGeneratorFromPeakData()
        {
            this.ModeOfPeakWidthCalculation = SyntheticMSGeneratorFromPeakDataMode.WidthsCalculatedOnAPerPeakBasis;
        }

        public SyntheticMSGeneratorFromPeakData(double peakWidthForAllPeaks)
            : this()
        {
            this.PeakWidthForAllPeaks = peakWidthForAllPeaks;
            this.ModeOfPeakWidthCalculation = SyntheticMSGeneratorFromPeakDataMode.WidthsCalculatedFromSingleValue;

        }
        #endregion

        #region Properties
        public double PeakWidthForAllPeaks { get; set; }

        public SyntheticMSGeneratorFromPeakDataMode ModeOfPeakWidthCalculation { get; set; }

        #endregion

        #region Public Methods



        #endregion

        #region Private Methods
        #endregion
        public override XYData GenerateMS(Run run, ScanSet lcScanset, ScanSet imsScanset=null)
        {
            Check.Require(run != null, String.Format("{0} failed. Run has not been defined.", this.Name));
            Check.Require(run.PeakList != null && run.PeakList.Count > 0, String.Format("{0} failed. Run has not been defined.", this.Name));

            XYData syntheticMSData = new XYData();
            List<double> xvals = new List<double>();
            List<double> yvals = new List<double>();

            foreach (MSPeak peak in run.PeakList)
            {
                XYData generatedXYData;

                switch (ModeOfPeakWidthCalculation)
                {
                    case SyntheticMSGeneratorFromPeakDataMode.WidthsCalculatedFromSingleValue:
                        //generatedXYData = peak.GetTheorPeakData(this.PeakWidthForAllPeaks, 11);
                        generatedXYData = IQ_X64.Backend.Utilities.IsotopeDistributionCalculation.TheorXYDataCalculationUtilities.GetTheorPeakData(this.PeakWidthForAllPeaks, peak.Height,peak.Width, 11);
                        break;
                    case SyntheticMSGeneratorFromPeakDataMode.WidthsCalculatedOnAPerPeakBasis:
                        //generatedXYData = peak.GetTheorPeakData(peak.Width, 11);
                        generatedXYData = IQ_X64.Backend.Utilities.IsotopeDistributionCalculation.TheorXYDataCalculationUtilities.GetTheorPeakData(peak.Width, peak.Height, peak.Width, 11);
                        break;
                    default:
                        //generatedXYData = peak.GetTheorPeakData(this.PeakWidthForAllPeaks, 11);
                        generatedXYData = IQ_X64.Backend.Utilities.IsotopeDistributionCalculation.TheorXYDataCalculationUtilities.GetTheorPeakData(this.PeakWidthForAllPeaks, peak.Height, peak.Width, 11);
                        break;
                }

                xvals.AddRange(generatedXYData.Xvalues);
                yvals.AddRange(generatedXYData.Yvalues);
            }

            XYData xydata=new XYData();
            xydata.Xvalues = xvals.ToArray();
            xydata.Yvalues = yvals.ToArray();
            return xydata;
        }

        
    }
}
