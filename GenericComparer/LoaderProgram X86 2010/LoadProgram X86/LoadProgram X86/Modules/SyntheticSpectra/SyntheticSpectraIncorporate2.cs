using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

using MathNet.Numerics;
using MathNet.Numerics.Interpolation;
using MathNet.Numerics.Interpolation.Algorithms;
using MathNet.Numerics.LinearAlgebra;

namespace ConsoleApplication1
{
    class SyntheticSpectraIncorporate
    {
        public List<PeakDecimal> AllignNoise(List<PeakDecimal> spectra, List<XYData> noiseSpectra, decimal peak_spacing, PrintBuffer print)
        {
            //returns a spectra interpoated onto the grid

            List<PeakDecimal> gridSpectra = new List<PeakDecimal>();  //where we store the new intensities

            //holds the results from the compare only algorithm
            List<int> splineIndexes = new List<int>();
            List<int> massIndexes = new List<int>();
            List<decimal> Masses = new List<decimal>();
            CompareContrast CompareContrastX = new CompareContrast();
            CompareContrastX.CompareOnly(splineIndexes, massIndexes, Masses, spectra, noiseSpectra);


            //interpolated the points that were hits on the compare
            //we need to break apart the peaks so it will spline copyX and copyY
            List<double> copyX = new List<double>();  //X
            List<double> copyY = new List<double>();  //Y

            for (int r = 0; r < noiseSpectra.Count; r++)
            {
                copyX.Add((double)noiseSpectra[r].X);
                copyY.Add((double)noiseSpectra[r].Y);
            }

            //generate spline model
            IInterpolationMethod method2 = Interpolation.CreateNaturalCubicSpline(copyX, copyY);

            //interpolate data and add to gridspectra
            int counter = 0;
            decimal addedValue;
            for (int i = 0; i < spectra.Count; i++)
            {
                PeakDecimal NewPeak = new PeakDecimal();
                NewPeak.Mass = spectra[i].Mass;//a =1 offset is correct

                if (i == massIndexes[counter])
                {
                    Masses[counter] = Masses[counter];
                    addedValue = (decimal)method2.Interpolate((double)NewPeak.Mass);
                    NewPeak.Intensity = (decimal)addedValue;
                    if (counter < splineIndexes.Count - 1)
                    {
                        counter++;
                    }
                }
                else
                {
                    NewPeak.Intensity = 0;//add 0 for non interpolated points
                }
                gridSpectra.Add(NewPeak);
            }

            return gridSpectra;
        }
    }
}
        