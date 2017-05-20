using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using Synthetic_Spectra_DLL_X64_Net_4.Framework;
using System.IO;

namespace Synthetic_Spectra_DLL_X64_Net_4.Synthetic_Spectra_Module
{
    public interface ISpectraGenerator
    {
        List<PeakDecimal> WithoutNoise(int massInt64Shift, List<PeakDecimal> existingGrid);
        List<PeakDecimal> WithNoiseRun(int massInt64Shift, List<PeakDecimal> existingGrid);//(List<DataSetGeneric<Int64,double>> MSData, string isotopeType, int massInt64Shift);
        SyntheticSpectraRun<Int64, double> SpectraParameters { get; set; }	
    }
    
    public class SyntheticSpectraController : ISpectraGenerator
	{
        public SyntheticSpectraRun<Int64, double> SpectraParameters { get; set; }
        
        //(MsRun contains a peak list spectra and parameter file and a printer
        List<PeakDecimal> ISpectraGenerator.WithNoiseRun(int massInt64Shift, List<PeakDecimal> existingGrid)//(List<DataSetGeneric<Int64, double>> MSData, string isotopeType)
        {
            #region 1. make synthetic spectra
            //bring in input parameters from MSData
            SyntheticSpectraParameters Parameters = SpectraParameters.MZParameters;

            #region 1A either make a new spectra or add to an existing one
            List<PeakDecimal> synSpectra = new List<PeakDecimal>(); // where the final spectra ends up
            SyntheticSpectra newSpectra = new SyntheticSpectra();
            if (existingGrid.Count == 0)
            {
                //set up a grid from which to make the pure spectra and later add on the noise
                synSpectra = newSpectra.SynSpectraGridSpectrum(ref Parameters);
            }
            else
            {
                synSpectra = existingGrid;
            }
            #endregion

            List<XYDataGeneric<Int64, double>> XYList = SpectraParameters.MonoIsotopicPeakList;  //list of masses to synthesize

            //create pure spectra here by adding each mass to the grid
            decimal XdataPoint;
            double YdataPoint;

            AveragineType isotopeType = SpectraParameters.MZParameters.AveragineType;

            double printmass = (double)XYList[0].X / (double)massInt64Shift;
            string stringmass = printmass.ToString();
            Console.WriteLine("Computing ion " + printmass.ToString());
            for (int i = 0; i < XYList.Count; i++)
            {
                XdataPoint = (decimal)XYList[i].X / massInt64Shift;//TODO one conversion
                YdataPoint = XYList[i].Y;
                newSpectra.AddMass(synSpectra, XdataPoint, YdataPoint, isotopeType, ref Parameters);
            }
            #endregion

            #region 2. interpolate noise
            //add the noise here
            List<XYDataGeneric<Int64, double>> peakListNoiseXY = SpectraParameters.NoiseSpectra;// MSData[1].XYList;

            //convert to old data types
            List<PeakDecimal> synSpectraOld = new List<PeakDecimal>();
            List<XYData> peakListNoiseXYOld = new List<XYData>();
            for (int i = 0; i < synSpectra.Count; i++)
            {
                PeakDecimal newPeakDecimal = new PeakDecimal();
                newPeakDecimal.Mass = synSpectra[i].Mass;
                newPeakDecimal.Intensity = synSpectra[i].Intensity;
                synSpectraOld.Add(newPeakDecimal);
            }
            for (int i = 0; i < peakListNoiseXY.Count; i++)
            {
                float X = (float)(peakListNoiseXY[i].X) / (float)massInt64Shift;
                float Y = (float)peakListNoiseXY[i].Y;
                XYData newXYData = new XYData(X, Y);
                //newXYData.X = (double)peakListNoiseXY[i].X / massInt64Shift;//TODO other conversion
                //newXYData.Y = peakListNoiseXY[i].Y;
                peakListNoiseXYOld.Add(newXYData);
            }
            SyntheticSpectraIncorporate addNoiseSpectra = new SyntheticSpectraIncorporate();
            List<PeakDecimal> noiseSpectra = addNoiseSpectra.AllignNoise(synSpectraOld, peakListNoiseXYOld, Parameters.PeakSpacing);

            //not that noise spectrum has been interpolated,add interpolated noise to the spectra grid
            decimal factor;
            decimal sum = 0;
            decimal average = 0;
            decimal stdev = 0;

            average = 0;
            for (int j = 0; j < noiseSpectra.Count; j++)
            {
                average += noiseSpectra[j].Intensity;
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
                if (synSpectra[j].Intensity > average + stdev)
                {
                    factor = 0;
                    synSpectra[j].Intensity += noiseSpectra[j].Intensity * factor;
                }
                else
                {
                    factor = 1;
                    synSpectra[j].Intensity += noiseSpectra[j].Intensity * factor;
                }
            }
            #endregion

            return synSpectra;
        }


        //(MsRun contains a peak list spectra and parameter file and a printer
        List<PeakDecimal> ISpectraGenerator.WithoutNoise(int massInt64Shift, List<PeakDecimal> existingGrid) 
        {
            #region 1. make synthetic spectra
                //bring in input parameters from MSData
                SyntheticSpectraParameters Parameters = SpectraParameters.MZParameters;

                #region 1A either make a new spectra or add to an existing one
                List<PeakDecimal> synSpectra = new List<PeakDecimal>(); // where the final spectra ends up
                SyntheticSpectra newSpectra = new SyntheticSpectra();    
                if (existingGrid.Count == 0)
                {
                    //set up a grid from which to make the pure spectra and later add on the noise 
                    synSpectra = newSpectra.SynSpectraGridSpectrum(ref Parameters);
                }
                else
                {
                    synSpectra = existingGrid;
                }
                #endregion

                List<XYDataGeneric<Int64, double>> XYList = SpectraParameters.MonoIsotopicPeakList;  //list of masses to synthesize
            
                //create pure spectra here by adding each mass to the grid
                decimal XdataPoint;
                double YdataPoint;

                AveragineType isotopeType = SpectraParameters.MZParameters.AveragineType;

                for (int i = 0; i < XYList.Count; i++)
                {
                    #region to write part 1/2
                    //if (1 == 2)//switch Add mass synspectra from synSpectra to synSpectra2 and in the writer
                    //{
                    //    List<PeakDecimal> synSpectra2 = new List<PeakDecimal>(); // where the final spectra ends up
                    //    SyntheticSpectra newSpectra2 = new SyntheticSpectra();
                    //    synSpectra2 = newSpectra2.SynSpectraGridSpectrum(ref Parameters);
                    //}
                    #endregion

                    XdataPoint = (decimal)XYList[i].X/massInt64Shift;
                    YdataPoint = XYList[i].Y;
                    newSpectra.AddMass(synSpectra, XdataPoint, YdataPoint, isotopeType, ref Parameters);

                    #region to write part 2/2
                    //if (1 == 2)
                    //{
                    //    string outputFileDestination = @"d:\Csharp\Syn Output\LCSynthetic";
                    //    StringBuilder sb = new StringBuilder();
                    //    string name;
                    //    if (i < 10)
                    //    {
                    //        name = outputFileDestination + "0" + i.ToString() + ".txt";
                    //    }
                    //    else
                    //    {
                    //        name = outputFileDestination + i.ToString() + ".txt";
                    //    }
                    //    using (StreamWriter writer = new StreamWriter(name))
                    //    {
                    //        for (int d = 0; d < synSpectra.Count; d++)
                    //        {
                    //            sb = new StringBuilder();
                    //            sb.Append((decimal)((decimal)synSpectra[d].Mass));
                    //            sb.Append("\t");
                    //            sb.Append(synSpectra[d].Intensity);

                    //            writer.WriteLine(sb.ToString());
                    //        }
                    //    }
                    //}
                    #endregion
                }
            #endregion

            return synSpectra;
        }	
	}
}
