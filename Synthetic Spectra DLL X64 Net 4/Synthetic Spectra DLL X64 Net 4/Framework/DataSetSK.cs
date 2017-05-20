using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Synthetic_Spectra_DLL_X64_Net_4.Synthetic_Spectra_Module;

namespace Synthetic_Spectra_DLL_X64_Net_4.Framework
{
    public class DataSetGeneric<T, U>
        where T : struct
        where U : struct
    {
        public List<XYDataGeneric<T,U>> XYList { get; set; }
        public List<FeatureSynthetic> FeatureList { get; set; }
        public SyntheticSpectraParameters SyntheticParameters { get; set; }
        public string DataSetName { get; set; }

        public DataSetGeneric()
        {

        }

        //MS data
        public void AddSpectra(List<XYDataGeneric<T,U>> XYListIN)
        {
            this.XYList = XYListIN;
        }

        //Add features
        public void AddFeatures(List<FeatureSynthetic> featureListIN)
        {
            this.FeatureList = featureListIN;
        }

        //parameters for synthetic spectra
        public void AddParameters(SyntheticSpectraParameters SyntheticParametersIN)
        {
            this.SyntheticParameters = SyntheticParametersIN;
        }

        public void Name(String NameIN)
        {
            this.DataSetName = NameIN;
        }
    }
}
