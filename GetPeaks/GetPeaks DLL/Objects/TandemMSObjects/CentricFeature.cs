using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Objects.TandemMSObjects
{
    public class CentricFeature
    {
        public int ID { get; set; }

        //public AttributeCentric Attributes { get; set; }

        public PeakCentric Peaks { get; set; }

        public ScanCentric ScanInfo { get; set; }

        //public FragmentCentric TandemRelationships { get; set; }

        public CentricFeature()
        {
            //Attributes = new AttributeCentric();
            Peaks = new PeakCentric();
            ScanInfo = new ScanCentric();
            //TandemRelationships = new FragmentCentric();
        }
    }
}
