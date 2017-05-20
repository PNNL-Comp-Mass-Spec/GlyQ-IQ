using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQGlyQ.Processors
{
    public enum EnumerationMassSpectraProcessing
    {
        OmicsCentroid_Only,
        OmicsCentroid_OmicsThreshold,
        OmicsCentroid_OmicsThreshold_OmicsPeakFilter,
        OmicsCentroid_OmicsHammer,
     }
}
