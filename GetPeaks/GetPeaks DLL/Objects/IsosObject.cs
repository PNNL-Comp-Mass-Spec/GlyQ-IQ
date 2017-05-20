using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Objects
{
    public class IsosObject
    {
        public int scan_num {get;set;} 
        public int charge {get;set;} 
        public double abundance {get;set;}
        public double mz {get;set;}
        public float fit {get;set;}
        public double average_mw {get;set;}
        public double monoisotopic_mw {get;set;}
        public double mostabundant_mw {get;set;} 
        public float fwhm {get;set;} 
        public float signal_noise {get;set;} 
        public double mono_abundance {get;set;} 
        public double mono_plus2_abundance {get;set;} 
        public int flag {get;set;}
        public float interference_score {get;set;} 
    }
}
