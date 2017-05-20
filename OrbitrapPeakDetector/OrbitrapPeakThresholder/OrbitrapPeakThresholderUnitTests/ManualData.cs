using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using OrbitrapPeakThresholder.Objects;

namespace OrbitrapPeakThresholderUnitTests
{
    public class ManualData
    {
        public List<XYData> PeaksAll { get; set; }

        public List<XYData> PeaksSignal { get; set; }

        public List<XYData> PeaksNoise { get; set; }

        public List<XYData> PeaksMono {get;set;}

        public List<ClusterCP<double>> Clusters { get; set; }

        public SortedDictionary<int,XYData> RawData {get;set;}

        public ManualData()
        {
            PeaksAll = new List<XYData>();
            PeaksSignal = new List<XYData>();
            PeaksNoise = new List<XYData>();
            PeaksMono = new List<XYData>();
            Clusters = new List<ClusterCP<double>>();
            RawData = new SortedDictionary<int, XYData>();
        }
    }
}
