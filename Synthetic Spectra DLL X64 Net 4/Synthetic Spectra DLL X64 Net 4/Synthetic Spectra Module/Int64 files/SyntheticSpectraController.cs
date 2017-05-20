using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Synthetic_Spectra_DLL.Framework;

namespace Synthetic_Spectra_DLL.Synthetic_Spectra_Module
{
    public interface ISpectraGenerator 
    {
        List<PeakGeneric<Int64, double>> WithoutNoise(List<DataSetGeneric<Int64,double>> MSData, IsotopeType isotopeType, int massInt64Shift);
        List<PeakGeneric<Int64, double>> WithNoiseRun(int massInt64Shift);//(List<DataSetGeneric<Int64,double>> MSData, string isotopeType, int massInt64Shift);
        SyntheticSpectraObject<decimal, double> SpectraParameters { get; set; }
    }

    
    public class SyntheticSpectraController  : ISpectraGenerator
    {
        public SyntheticSpectraObject<decimal, double> SpectraParameters { get; set; }
        
        //(MsRun contains a peak list spectra and parameter file and a printer
        List<PeakGeneric<Int64, double>> ISpectraGenerator.WithNoiseRun(int massInt64Shift)//(List<DataSetGeneric<Int64, double>> MSData, string isotopeType)
        {
            #region 1. make synthetic spectra
            //bring in input parameters from MSData
            //SyntheticSpectraParameters Parameters = MSData[0].SyntheticParameters;
            SyntheticSpectraParameters Parameters = SpectraParameters.Parameters;

            List<PeakGeneric<Int64, double>> synSpectra = new List<PeakGeneric<Int64, double>>(); // where the final spectra ends up

            //set up a grid from which to make the pure spectra and later add on the noise
            SyntheticSpectra newSpectra = new SyntheticSpectra();
            synSpectra = newSpectra.SynSpectraGrid(ref Parameters);//TODO Int64

            //List<XYDataGeneric<Int64, double>> XYList = MSData[0].XYList;  //list of masses to synthesize
            List<XYDataGeneric<Int64, double>> XYList = SpectraParameters.MonoIsotopicPeakList;  //list of masses to synthesize
            
            ////convert to Int64s and doubles
            //List<XYDataGeneric<Int64, double>> XYInt64List = new List<XYDataGeneric<Int64, double>>();
            //for (int i = 0; i > XYList.Count; i++)
            //{
            //    XYDataGeneric<Int64, double> newPoint = new XYDataGeneric<Int64,double>((Int64)(XYList[i].X/massInt64Shift),(double)XYList[i].Y);
            //    XYInt64List.Add(newPoint);
            //}

            //TODO incorporate into parameter file
            IsotopeType isotopeType = IsotopeType.Peptide;

            //create pure spectra here by adding each mass to the grid
            Int64 XdataPoint;
            double YdataPoint;

            for (int i = 0; i < XYList.Count; i++)
            {
                XdataPoint = XYList[i].X;
                //XdataPoint = (double)XYList[i].X;
                YdataPoint = XYList[i].Y;
                //newSpectra.AddMass(synSpectra, XdataPoint, YdataPoint, isotopeType, ref Parameters);
                newSpectra.AddMass(synSpectra, XdataPoint, YdataPoint, isotopeType, ref Parameters, massInt64Shift);
            }
            #endregion

            #region 2. interpolate noise
            //add the noise here
            List<XYDataGeneric<Int64, double>> peakListNoiseXY = SpectraParameters.NoiseSpectra;// MSData[1].XYList;

            SyntheticSpectraIncorporate addNoiseSpectra = new SyntheticSpectraIncorporate();
            List<PeakGeneric<Int64, double>> noiseSpectra = addNoiseSpectra.AllignNoise(synSpectra, peakListNoiseXY, Parameters.PeakSpacing);
            //List<PeakGeneric<Int64, double>> noiseSpectra = addNoiseSpectra.AllignNoise(synSpectra, peakListNoiseXY, Parameters.PeakSpacing);

            //not that noise spectrum has been interpolated,add interpolated noise to the spectra grid
            double factor;
            double sum = 0;
            double average = 0;
            double stdev = 0;

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
            stdev = (double)Math.Pow((double)sum / noiseSpectra.Count, 0.5);

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
        List<PeakGeneric<Int64, double>> ISpectraGenerator.WithoutNoise(List<DataSetGeneric<Int64, double>> MSData, IsotopeType isotopeType, int massInt64Shift) 
        {
            #region 1. make synthetic spectra
            //bring in input parameters from MSData
            SyntheticSpectraParameters Parameters = MSData[0].SyntheticParameters;

            List<PeakGeneric<Int64, double>> synSpectra = new List<PeakGeneric<Int64, double>>(); // where the final spectra ends up

            //set up a grid from which to make the pure spectra and later add on the noise
            SyntheticSpectra newSpectra = new SyntheticSpectra();
            synSpectra = newSpectra.SynSpectraGrid(ref Parameters);

            List<XYDataGeneric<Int64, double>> XYList = MSData[0].XYList;  //list of masses to synthesize

            //create pure spectra here by adding each mass to the grid

            Int64 XdataPoint;
            double YdataPoint;

            for (int i = 0; i < XYList.Count; i++)
            {
                XdataPoint = (Int64)XYList[i].X;
                YdataPoint = XYList[i].Y;
                newSpectra.AddMass(synSpectra, XdataPoint, YdataPoint, isotopeType, ref Parameters, massInt64Shift);
            }
            #endregion

            return synSpectra;
        }
    }


    public enum SyntheticSpectraFileType
    { 
        TheoryData,
        Parameters,
        Noise
    }

    public enum IsotopeType
    {
        Peptide,
        Glycan
    }
}
