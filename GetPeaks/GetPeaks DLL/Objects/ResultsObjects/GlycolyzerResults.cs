using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using OmniFinder.Objects;

namespace GetPeaks_DLL.Objects.ResultsObjects
{
    public class GlycolyzerResults
    {
        /// <summary>
        /// mass accuracy used to allign with library
        /// </summary>
        public double Tollerance { get; set; }//error tolerance
        
        /// <summary>
        /// how many glycans with compositions were found
        /// </summary>
        public int NumberOfHits { get; set; }

        /// <summary>
        /// Pertinent information for the glycans that hit the glycan library
        /// </summary>
        public List<GlycanResult> GlycanHits { get; set; }
       
        /// <summary>
        /// list of unique masses detected in the sample
        /// </summary>
        public List<double> GlycanUniqueHitsLibraryExactMass { get; set; }//all masses from Hits with duplicates removed

        /// <summary>
        /// unique masses are alligned to the library.  zeroes exist when no data was found
        /// </summary>
        public List<GlycanResult> GlycanHitsInLibraryOrder { get; set; }

        /// <summary>
        /// headders for the omnifinder
        /// </summary>
        public List<string> GlycanLibraryHeaders { get; set; }//names corresponding to indexes

       
        public GlycolyzerResults()
        {
            GlycanHits = new List<GlycanResult>();
            GlycanUniqueHitsLibraryExactMass = new List<double>();
            GlycanHitsInLibraryOrder = new List<GlycanResult>();
            GlycanLibraryHeaders = new List<string>();
            NumberOfHits = 0;
            Tollerance = 0;
        }
    }
}
