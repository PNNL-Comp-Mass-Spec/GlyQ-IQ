using System.Collections.Generic;

using Run64.Backend.Core;

namespace IQ_X64.Backend.Utilities.Converters
{
    public static class PeakTypeConverter
    {
        #region Constructors
        #endregion

        #region Properties
        #endregion

        #region Public Methods
        public static List<MSPeak> ConvertToMSPeaks(List<Peak> peakList)
        {
            List<MSPeak> mspeakList = new List<MSPeak>();

            foreach (Peak peak in peakList)
            {
                if (peak is MSPeak)
                {
                    mspeakList.Add((MSPeak)peak);
                }

                
            }
            return mspeakList;

        }


        #endregion

        #region Private Methods
        #endregion
    }
}
