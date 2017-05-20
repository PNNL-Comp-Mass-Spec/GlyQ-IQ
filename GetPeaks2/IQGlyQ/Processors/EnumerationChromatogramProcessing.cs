using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQGlyQ.Processors
{
    public enum EnumerationChromatogramProcessing
    {
        StandardIQ,
        StandardOmics,
        SmoothingOnly,
        SmoothSection,
        SmoothSectionWithAverage,
        SmoothSectionWithPreSmooth,
        ChromatogramLevel,
        ChromatogramLevelPrint,
        ChromatogramLevelUnitTest,
        ChromatogramLevelWithAverage,
        SPIN,
        Velos,
        QTof,
        IMS,
        LCPeakDetectOnly,
    }
}
