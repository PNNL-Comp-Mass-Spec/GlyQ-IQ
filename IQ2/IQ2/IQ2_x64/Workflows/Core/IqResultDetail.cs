using System.Collections.Generic;
using Run64.Backend.Core;
using Run64.Backend.Data;

namespace IQ_X64.Workflows.Core
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