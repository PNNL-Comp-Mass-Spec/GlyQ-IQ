using System.Collections.Generic;
using Run32.Backend.Core;
using Run32.Backend.Data;

namespace IQ.Workflows.Core
{
    public class IqResultDetail
    {
        #region Properties

        public XYData Chromatogram { get; set; }

        public XYData MassSpectrum { get; set; }

        public List<ChromPeakQualityData> ChromPeakQualityData { get; set; }

        #endregion

        #region Public Methods



        #endregion


    }
}