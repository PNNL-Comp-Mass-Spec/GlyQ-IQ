using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects.ResultsObjects;

namespace GetPeaks_DLL.TandemSupport
{
    public class TandemAnalysisResultsObject
    {
        public int Scan { get; set; }

        public int TandemScan { get; set; }

        public double ParentMZ { get; set; }

        public double ParentMZCorrected { get; set; }

        public double ParentMono { get; set; }

        /// <summary>
        /// from database and denovo charge determination
        /// </summary>
        public int ParentCharge { get; set; }

        /// <summary>
        /// from matched selected precursor to the mz of the selected
        /// </summary>
        public int ParentChargeDiscovered { get; set; }

        public bool IsFragmented { get; set; }
        
        /// <summary>
        /// Glycans Present in Precursor Scan
        /// </summary>
        public GlycolyzerResults HitsFromPrecursorScans { get; set; }

        /// <summary>
        /// Diagnostic Ions Present in Tandem Scan
        /// </summary>
        public GlycolyzerResults HitsFromDiagnosticIons { get; set; }

        /// <summary>
        /// Diagnostic Ions Present in Tandem Scan
        /// </summary>
        public GlycolyzerResults HitsFromPrecursorParentIon { get; set; }
    }
}
