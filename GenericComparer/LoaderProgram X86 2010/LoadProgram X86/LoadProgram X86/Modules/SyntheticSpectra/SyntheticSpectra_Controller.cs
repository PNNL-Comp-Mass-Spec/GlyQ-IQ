using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

namespace ConsoleApplication1
{
    public interface ISpectraGenerator
    {
        List<PeakDecimal> WithoutNoise(List<DataSetSK> MSData, string isotopeType, PrintBuffer print);
        List<PeakDecimal> WithNoise(List<DataSetSK> MSData, string isotopeType, PrintBuffer print);	
    }
    
    public class SyntheticSpectra_Controller : ISpectraGenerator
	{
        //(MsRun contains a peak list spectra and parameter file and a printer
        List<PeakDecimal> ISpectraGenerator.WithNoise(List<DataSetSK> MSData, string isotopeType, PrintBuffer print)
        {
            #region 1. make synthetic spectra
                //bring in input parameters from MSData
                SyntheticSpectraParameters Parameters = MSData[0].SyntheticParameters;        

			    List<PeakDecimal> synSpectra = new List<PeakDecimal>(); // where the final spectra ends up

                //set up a grid from which to make the pure spectra and later add on the noise
                SyntheticSpectra newSpectra = new SyntheticSpectra();
                synSpectra = newSpectra.SynSpectraGrid(ref Parameters, print);

                List<XYData> XYList = MSData[0].XYList;  //list of masses to synthesize
                
                //create pure spectra here by adding each mass to the grid
                decimal XdataPoint;
                double YdataPoint;

			    for (int i = 0; i < XYList.Count; i++)
			    {
                    XdataPoint = (decimal)XYList[i].X;
                    YdataPoint = XYList[i].Y;
				    newSpectra.AddMass(synSpectra, XdataPoint, YdataPoint, isotopeType, ref Parameters, print);
                }
            #endregion

            #region 2. interpolate noise
                //add the noise here
                List<XYData> peakListNoiseXY = MSData[1].XYList;

                SyntheticSpectraIncorporate addNoiseSpectra = new SyntheticSpectraIncorporate();
                List<PeakDecimal> noiseSpectra = addNoiseSpectra.AllignNoise(synSpectra, peakListNoiseXY, Parameters.PeakSpacing, print);

                //not that noise spectrum has been interpolated,add interpolated noise to the spectra grid
                decimal factor;
                decimal sum = 0;
                decimal average = 0;
                decimal stdev = 0;

                average = 0;
                for (int j = 0; j < noiseSpectra.Count; j++)
                {
                    average+= noiseSpectra[j].Intensity;     
                }
                average = average / noiseSpectra.Count;

                //standard deviation
                for (int j = 0; j < noiseSpectra.Count; j++)
                {
                     sum += (noiseSpectra[j].Intensity - average) * (noiseSpectra[j].Intensity - average);
                }
                stdev = (decimal)Math.Pow((double)sum / noiseSpectra.Count, 0.5);

                for (int j = 0; j < synSpectra.Count; j++)
                {
                    if (synSpectra[j].Intensity > average+stdev)
                    {
                        factor = 0;
                        synSpectra[j].Intensity += noiseSpectra[j].Intensity*factor;
                    }
                    else
                    {
                        factor = 1;
                        synSpectra[j].Intensity += noiseSpectra[j].Intensity*factor;
                    }
                }
            #endregion

            return synSpectra;  
		}


        //(MsRun contains a peak list spectra and parameter file and a printer
        List<PeakDecimal> ISpectraGenerator.WithoutNoise(List<DataSetSK> MSData, string isotopeType, PrintBuffer print)
        {
            #region 1. make synthetic spectra
                //bring in input parameters from MSData
                SyntheticSpectraParameters Parameters = MSData[0].SyntheticParameters;

                List<PeakDecimal> synSpectra = new List<PeakDecimal>(); // where the final spectra ends up

                //set up a grid from which to make the pure spectra and later add on the noise
                SyntheticSpectra newSpectra = new SyntheticSpectra();
                synSpectra = newSpectra.SynSpectraGrid(ref Parameters, print);

                List<XYData> XYList = MSData[0].XYList;  //list of masses to synthesize

                //create pure spectra here by adding each mass to the grid

                decimal XdataPoint;
                double YdataPoint;

                for (int i = 0; i < XYList.Count; i++)
                {
                    XdataPoint = (decimal)XYList[i].X;
                    YdataPoint = XYList[i].Y;
                    newSpectra.AddMass(synSpectra, XdataPoint, YdataPoint, isotopeType, ref Parameters, print);
                }
            #endregion

            return synSpectra;
        }	
	}
}
