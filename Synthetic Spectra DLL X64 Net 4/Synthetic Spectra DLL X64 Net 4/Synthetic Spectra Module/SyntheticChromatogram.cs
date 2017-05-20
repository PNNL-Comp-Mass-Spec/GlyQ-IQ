using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Synthetic_Spectra_DLL_X64_Net_4.Framework;

namespace Synthetic_Spectra_DLL_X64_Net_4.Synthetic_Spectra_Module
{
    public class SyntheticChromatogram
    {
        public void AddHill(SyntheticChromatogramParameters ChromatogramParameters)
        {
            List<List<PeakGeneric<Int64, float>>> chromatogramGrid = ChromatogramParameters.Chromatogram;

            #region  generate spectrum
            ISpectraGenerator NewSpectraController = new SyntheticSpectraController();
            SyntheticSpectraRun<Int64, double> localSpectraFilesCopy = new SyntheticSpectraRun<long, double>();
            localSpectraFilesCopy.MonoIsotopicPeakList= new List<XYDataGeneric<long,double>>();
            localSpectraFilesCopy.MonoIsotopicPeakList.Add(ChromatogramParameters.PeakToAdd);
            localSpectraFilesCopy.LCParametersRTSigma = new List<XYDataGeneric<int, int>>();
            localSpectraFilesCopy.LCParametersRTSigma.Add(ChromatogramParameters.RTFWHMToAdd);
            localSpectraFilesCopy.NoiseSpectra = ChromatogramParameters.SpectraFiles.NoiseSpectra;
            localSpectraFilesCopy.MZParameters = ChromatogramParameters.SpectraFiles.MZParameters;

            NewSpectraController.SpectraParameters = localSpectraFilesCopy;//with these parameters.  is this a copy?
            
            //the centroid of the peak
            int peakcenterScan = ChromatogramParameters.RTFWHMToAdd.X;
            //the range for the eluting peak
            int scanrangeOffset = (int)(localSpectraFilesCopy.LCParametersRTSigma[0].Y*localSpectraFilesCopy.MZParameters.RTSigmaMultiplier);

            
            //TODO fix gausian function and find vlaues that work
            for (int elutingPeak = peakcenterScan - scanrangeOffset; elutingPeak < peakcenterScan + scanrangeOffset; elutingPeak++)//for LC start stop
            {
                if (elutingPeak >= 0 && elutingPeak<ChromatogramParameters.Chromatogram.Count)//ensutes the peak extends greater than the beginning of the chromatogram
                {
                    #region Setup spectrum grid by either creating a new one or coverting an existing int64 to decimal for use as a basis

                    List<PeakDecimal> existingGrid = new List<PeakDecimal>(); // where the final spectra ends up
                    SyntheticSpectra newSpectraSetup = new SyntheticSpectra();
                    if (ChromatogramParameters.Chromatogram[elutingPeak].Count > 0)
                    {
                        for (int i = 0; i < chromatogramGrid[elutingPeak].Count; i++)
                        {
                            PeakGeneric<Int64, float> holdLocation = ChromatogramParameters.Chromatogram[elutingPeak][i];
                            PeakDecimal GridPeak = new PeakDecimal((decimal)holdLocation.Mass / ChromatogramParameters.MassInt64Shift, (decimal)holdLocation.Intensity);
                            existingGrid.Add(GridPeak);
                        }
                    }
                    else
                    {
                        SyntheticSpectraParameters holdSpecParameters = new SyntheticSpectraParameters();
                        holdSpecParameters = ChromatogramParameters.SpectraFiles.MZParameters;
                        existingGrid = newSpectraSetup.SynSpectraGridSpectrum(ref holdSpecParameters);
                    }
                    #endregion

                    #region gausian factor
                    double gausianFactor;
                    double sigma = localSpectraFilesCopy.LCParametersRTSigma[0].Y;//X is RT center, Y is Sigma in units of scans
                    //double sigma = FWHM/2.35482;//X is RT center, Y is Sigma
                    double Offset = 0;
                    int X = elutingPeak - peakcenterScan;
                    //TODO Prefactor does not give full intensity at apex!
                    double prefactor = 1 / (Math.Pow((2 * Math.PI * sigma * sigma), 0.5));
                    prefactor = 1;
                    gausianFactor = prefactor * Math.Exp(-(((double)X - Offset) * ((double)X - Offset)) / (2 * sigma * sigma));
                    //Console.WriteLine(gausianFactor);
                    double holdIntensitytillAfter = ChromatogramParameters.PeakToAdd.Y;//hold base intensity and send in shortened version
                    ChromatogramParameters.PeakToAdd.Y = ChromatogramParameters.PeakToAdd.Y * gausianFactor;
                    #endregion

                    #region generate spectra
                    List<PeakDecimal> outputSpectra = new List<PeakDecimal>();
                    if (ChromatogramParameters.GenerateWithNoise)
                    {
                        outputSpectra = NewSpectraController.WithNoiseRun(ChromatogramParameters.MassInt64Shift, existingGrid);
                    }
                    else
                    {
                        outputSpectra = NewSpectraController.WithoutNoise(ChromatogramParameters.MassInt64Shift, existingGrid);
                    }
                    #endregion

                    ChromatogramParameters.PeakToAdd.Y = holdIntensitytillAfter;

                    //shrink spectra
                    #region convert to int64
                    List<PeakGeneric<Int64, float>> finalSpectraInt64 = new List<PeakGeneric<Int64, float>>();
                    for (int i = 0; i < outputSpectra.Count; i++)//m/z
                    {
                        PeakGeneric<Int64, float> newPointInt64 = new PeakGeneric<Int64, float>();
                        newPointInt64.Mass = (Int64)(outputSpectra[i].Mass * (decimal)ChromatogramParameters.MassInt64Shift);
                        newPointInt64.Intensity = (float)outputSpectra[i].Intensity;
                        finalSpectraInt64.Add(newPointInt64);
                    }
                    #endregion
                    chromatogramGrid[elutingPeak] = finalSpectraInt64;
                }
            }

            #endregion

        }
    }
}
