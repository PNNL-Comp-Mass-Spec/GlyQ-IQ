using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL;
using DeconTools.Backend.Core;
using DeconToolsPart2;

namespace DeconToolsPart2
{
    public class Part2Net35
    {
        public static int GoPart2Decon(InputOutputFileName newFile, SimpleWorkflowParameters Parameters, List<ElutingPeak> discoveredPeaks, TransformerObject transformer2)
        {
            ElutingPeakFinderPart2 EPPart2 = new ElutingPeakFinderPart2();

            //TODO 3.   Setup Parameters in GetPeaks 4.0
            //EPPart2.Parameters = Parameters;

            int elutingpeakHits = 0;
            EPPart2.SimpleWorkflowExecutePart2d(discoveredPeaks, newFile, Parameters, ref elutingpeakHits, transformer2);
            return elutingpeakHits;
        }

        public static int GoPart2Omics(InputOutputFileName newFile, SimpleWorkflowParameters Parameters, List<ElutingPeakOmics> discoveredPeaks, TransformerObject transformer2)
        {
            ElutingPeakFinderPart2 EPPart2 = new ElutingPeakFinderPart2();

            //TODO 3.   Setup Parameters in GetPeaks 4.0
            //EPPart2.Parameters = Parameters;

            int elutingpeakHits = 0;
            EPPart2.SimpleWorkflowExecutePart2e(discoveredPeaks, newFile, Parameters, ref elutingpeakHits, transformer2);
            return elutingpeakHits;
        }


    }
}
