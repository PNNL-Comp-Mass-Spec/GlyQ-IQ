using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

namespace ConsoleApplication1
{
	public class DataSetSK
	{
        public List<XYData> XYList { get; set; }
        public List<FeatureSynthetic> FeatureList { get; set; }
        public SyntheticSpectraParameters SyntheticParameters { get; set; }
		public string DataSetName { get; set; }
		
		public DataSetSK()
		{

		}

		//MS data
        public void AddSpectra(List<XYData> XYListIN)
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
