using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PNNLOmics.Data;

namespace ConsoleApplication1
{
    public class FileLoadParameters 
	{
		#region Public Methods

        //needs the MS data XYData to work.  This converts the XYdata into the feature class and adds it to the dataset
        public void registerFeatures(List<DataSetSK> MZDataProject)
        {
            for (int i = 0; i < MZDataProject.Count; i++)
            {
                List<FeatureSynthetic> DataFeature = new List<FeatureSynthetic>();

                List<XYData> DataMZ = MZDataProject[i].XYList;
                //convert XYDaya ro feaures
                Converter ExchangeXY = new Converter();
                ExchangeXY.ConvertXYToFeature(DataMZ, DataFeature);

                //store data in MSRun
                DataSetSK NewRun = new DataSetSK();
                MZDataProject[i].AddFeatures(DataFeature);
            }
        }

        //adds the parameters to the dataset
        public void registerParameters(List<decimal> ParameterValues, List<DataSetSK> MZDataProject, int index)
        {
            SyntheticSpectraParameters SynPeaksParameters = new SyntheticSpectraParameters();
            SynPeaksParameters.StartMass = (decimal)ParameterValues[0];//first mass in range (line 1)
            SynPeaksParameters.EndMass = (decimal)ParameterValues[1];//last mass in range (line2)  this range must incorporate all data points and shoulders of neyly created peaks
            SynPeaksParameters.PeakSpacing = (decimal)ParameterValues[2];//difference between points in the mass domain (line 3)
            SynPeaksParameters.Resolution = (double)ParameterValues[3];//10,000 to 100,000  this range is important to ensure correct peak shapes (line 4)
            SynPeaksParameters.PercentHanning = (double)ParameterValues[4];//0-1 where 1=100% Hanning.  0.81 is experimentally good

            MZDataProject[index].AddParameters(SynPeaksParameters);
        }
        #endregion

        #region private methods Empty
        #endregion
    }
}
