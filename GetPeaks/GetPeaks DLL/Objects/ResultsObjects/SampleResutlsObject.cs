using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Features;
using PNNLOmics.Data;

namespace GetPeaks_DLL.Objects.ResultsObjects
{
    public class SampleResutlsObject
    {
        public SampleResutlsObject()
        {
            this.FeatureList = new List<FeatureLight>();
            this.ElutingPeakList = new List<ElutingPeakLite>();
            this.DataHitsList = new List<XYData>();
            this.LibraryHitsList = new List<XYData>();
            this.AppendedLibrary = new List<double>();
            this.AppendedFeatureList = new List<FeatureLight>();
            this.AppendedElutingPeakList = new List<ElutingPeakLite>();
            this.AppendedIsotopeObjectList = new List<IsotopeObject>();
            
            this.NumberOfHits = 0;
        }
        public string SampleName { get; set; }
        public List<FeatureLight> FeatureList { get; set; }
        public List<ElutingPeakLite> ElutingPeakList { get; set; }
        public List<IsotopeObject> IsotopeObjectList { get; set; }
        public List<double> AppendedLibrary { get; set; }
        public List<FeatureLight> AppendedFeatureList { get; set; }
        public List<ElutingPeakLite> AppendedElutingPeakList { get; set; }
        public List<IsotopeObject> AppendedIsotopeObjectList { get; set; }
        public int NumberOfHits {get;set;}
        public List<XYData> DataHitsList { get; set; }
        public List<XYData> LibraryHitsList { get; set; }
    }
}
