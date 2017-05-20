using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrbitrapPeakThresholderUnitTests.Enumerations
{
    public enum SpectraDataType
    {
        //All Data (all points in spectrum)
        RawData,
        //All Data above pre-established noise threshold
        Peaks,
        //All data found to be signal from manual annotation
        SignalPeaks,
        //All data found to be noise from manual annotation (1798)
        NoisePeaksFull,
        //All data not signal from manual annotation (1240)
        NoisePeaksSubset,
        //All data found to be monoisotopic masses from manual annotation
        MonoisotopicMasses,
        //All feature from manual annotation
        Clusters
    }
}
