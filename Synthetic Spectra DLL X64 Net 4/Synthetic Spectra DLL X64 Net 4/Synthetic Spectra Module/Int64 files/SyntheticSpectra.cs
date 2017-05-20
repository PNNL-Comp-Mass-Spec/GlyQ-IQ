using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Synthetic_Spectra_DLL.Framework;

namespace Synthetic_Spectra_DLL.Synthetic_Spectra_Module
{
    public class SyntheticSpectra
    {
        #region Public Methods
        public List<PeakGeneric<Int64,double>> SynSpectraGrid(ref SyntheticSpectraParameters Parameters)
        {
            //create a list of equal spaced points
            List<PeakGeneric<Int64, double>> synSpectra = new List<PeakGeneric<Int64, double>>();

            Console.WriteLine("the Range is " + Parameters.StartMass + " to " + Parameters.EndMass);

            Int64 expectedNumberOfPoints = (Int64)((Parameters.EndMass - Parameters.StartMass) / Parameters.PeakSpacing);
            Console.WriteLine("the Size " + expectedNumberOfPoints + " or  ");

            //int numberOfPoints = (int)Math.Floor(expectedNumberOfPoints);
            Int64 numberOfPoints = expectedNumberOfPoints;

            Console.WriteLine("reduced " + numberOfPoints);

            // add all empty peaks on the grid
            for (int i = 0; i < numberOfPoints; i++)
            {
                PeakGeneric<Int64, double> peak = new PeakGeneric<Int64, double>((Int64)(Parameters.StartMass + i * Parameters.PeakSpacing), 0);
                synSpectra.Add(peak);
            }
            return synSpectra;
        }

        //public void AddMass(List<PeakGeneric<Int64, double>> spectra, Int64 mass, double intensity, string isotopeType, ref SyntheticSpectraParameters Parameters)
        public void AddMass(List<PeakGeneric<Int64, double>> spectra, Int64 mass, double intensity, IsotopeType isotopeType, ref SyntheticSpectraParameters Parameters, int massInt64Shift)
        {
            //select averagine type to use.  Isotope type is sent in {peptide, glycan}
            Averagine currentAveragine = new Averagine();
            currentAveragine = currentAveragine.AveragineSetup(isotopeType);
            Int64 i = (Int64)(currentAveragine.isoUnit * massInt64Shift);

            //calculate isotope patterns for a given mass
            //TODO conversion happens here
            List<PeakGeneric<decimal, double>> isotopePatternD = new List<PeakGeneric<decimal, double>>();
            List<PeakGeneric<Int64, double>> isotopePattern = new List<PeakGeneric<Int64, double>>();
            TheoryIsotope isotopes = new TheoryIsotope();
            decimal massD = mass / massInt64Shift;
            isotopePatternD = isotopes.TheoryIsotopePoissonFX(massD, currentAveragine);
            for(int j = 0; j < isotopePatternD.Count; j++)
            {
                PeakGeneric<Int64,double> newIsotopePeak = new PeakGeneric<Int64,double>((Int64)(isotopePatternD[j].Mass*massInt64Shift),isotopePatternD[j].Intensity);
                isotopePattern.Add(newIsotopePeak);
            }
            

            SyntheticSpectra newSpectra = new SyntheticSpectra();

            Int64 massTemp;
            double intensityTemp;
            for (int j = 0; j < isotopePatternD.Count; j++)  //add up to 8 isotopes
            {
                massTemp = mass + isotopePattern[j].Mass;
                intensityTemp = intensity * isotopePattern[j].Intensity;
                //newSpectra.AddIon(spectra, massTemp, intensityTemp, resolution, peakSpacing, percentHanning, print);
                newSpectra.AddIon(spectra, massTemp, intensityTemp, ref Parameters, massInt64Shift);
            }
        }

        public void AddIon(List<PeakGeneric<Int64, double>> spectra, Int64 mass, double intensity, ref SyntheticSpectraParameters Parameters, int massInt64Shift)
        // public void AddIon(List<PeakDecimal> spectra, decimal mass, double intensity, ref SyntheticSpectraParameters Parameters, PrintBuffer print)
		//public void AddIon(List<PeakDecimal> spectra, decimal mass, double intensity, decimal resolution, decimal peakSpacing, double percentHanning, PrintBuffer print)
        {
			//resolution here is similar to peak witdh
			List<double> modelPeak = new List<double>();  //Peak Y
			List<decimal> modelX = new List<decimal>();//PeakX
			
			decimal startMZ = spectra[0].Mass;
			decimal endMZ = spectra[spectra.Count-1].Mass;
			decimal fractionAlong = (mass-startMZ) / (endMZ - startMZ);
            decimal holdMZ;

            double resolution = Parameters.Resolution;  //convert once
            double percentHanning=Parameters.PercentHanning;  //convert once
            decimal peakSpacing = Parameters.PeakSpacing; //convert once


            //location of mass of interest in main synSpectra
			int index = (int)Math.Floor(spectra.Count * fractionAlong);

            try//lets make sure the index is less than 1 so that we don't excede the array size
            {
                holdMZ=spectra[index].Mass;  //if index is larger than size of array this will fail.
            }
            catch (Exception)
            {
                Console.WriteLine("MZ_end is not large enough to contain all the data points");
                throw;
            }

			Console.WriteLine("Mass in " + mass + ". Closest Mass is "+ spectra[index].Mass);
			
			decimal offsetX = -(mass - spectra[index].Mass); //add this to all x going to FX so that it shifts the data appropriatly in between peaks
			decimal xScaleFactor = 100;  //how much to extend the peak tails as in multiples of the width, dx
			decimal width = mass / (decimal)Parameters.Resolution;
			decimal startPointD = width * xScaleFactor / Parameters.PeakSpacing;
			int startPoint = (int)Math.Floor(startPointD);

			decimal startMass = -startPoint * Parameters.PeakSpacing;
			decimal endMass = -startMass;

			decimal xMax = (decimal)Math.Pow(10, -28);  //smallest decimal
			double yMax = modelFX(xMax, mass, intensity, resolution, percentHanning);
			double maxIntensity = yMax;
			//maxIntensity = modelPeak.Max();

			for (int i = 0; i < startPoint * 2+1; i++)
			{
				//check for singularity
				decimal x = -startPoint * peakSpacing + i * peakSpacing + offsetX;  //shifts model so it is on grid
				double y = modelFX(x, mass, intensity, resolution, percentHanning);

				if ((!Double.IsNaN(y)))
				{
					x = -startPoint * peakSpacing + i * peakSpacing + offsetX;
					y = modelFX(x, mass, intensity, resolution, percentHanning);
				}
				else
				{	
					x = (decimal)Math.Pow(10,-28);  //smallest decimal
					y = modelFX(x, mass, intensity, resolution, percentHanning);
				}

				y = y * intensity / maxIntensity;  //scale Y

				modelX.Add(x);
				modelPeak.Add(y);

                Int64 xI = (Int64)(x * massInt64Shift);
                PeakGeneric<Int64, double> tempPeak = new PeakGeneric<Int64, double>(xI, (double)y);
				//Spectra[index - startPoint + i].Mass = tempPeak.Mass + mass;
				spectra[index - startPoint + i].Intensity += tempPeak.Intensity;			
			}
		
			Console.WriteLine("the mass is " + mass);

		}
		#endregion

        #region Private Methods
        //this is the numeric peak shape model derived from an excel file Peak Shapes
        private double modelFX(decimal xInt64, decimal centerMass, double intensity, double resolution, double percentHanning)
        {
            //make model so we can return a Y value for a given X value
            double x = (double)xInt64;
            double width = 0.02;  //constants for the model
            double standardIntensity = 2;  //constants for the model
            //double percentHanning = 0.81;//0-1 where 1=100% Hanning  //pulled out front


            //set up ratios for resolution
            width = (double)centerMass * Math.Pow((double)resolution, -1);
            width = 0.9074477 * width - 0.004008;

            //HanningK = 10 ^ ((-0.110140 * Math.Log(width)))^2  - 1.201610 * Math.Log(width) - 0.146960);
            double a = -0.1101408 * Math.Pow((Math.Log10(width)), 2);
            double b = -1.2016130 * Math.Log10(width);
            double c = -0.1469658;
            double abc = a + b + c;
            double HanningK = Math.Pow(10, abc);
            double HanningI = 15.2605274 * standardIntensity;

            //Lorentzian
            //=F$12*1/PI()*0.5*F$11/(I10^2+0.5*F$11^2)
            double lorentzianNumerator = standardIntensity * 1 / Math.PI * 0.5 * width;
            double lorentzianDenomenator = Math.Pow(x, 2) + 0.5 * Math.Pow(width, 2);
            double LorentzianY = lorentzianNumerator / lorentzianDenomenator;

            //Hanning
            //=F$18*(SIN(2*pi*k*I10)/(2*pi*k*I10)+0.5*SIN(2*pi*k*I10-pi)/(2*pi*k*I10-pi)+0.5*SIN(2*pi*k*I10+pi)/(2*pi*k*I10+pi))
            double k = HanningK;
            a = Math.Sin(2 * Math.PI * k * x) / (2 * Math.PI * k * x);
            b = 0.5 * Math.Sin(2 * Math.PI * k * x - Math.PI) / (2 * Math.PI * k * x - Math.PI);
            c = 0.5 * Math.Sin(2 * Math.PI * k * x + Math.PI) / (2 * Math.PI * k * x + Math.PI);
            double HanningY = HanningI * (a + b + c);

            //hyrid intensity at point x
            intensity = percentHanning * HanningY + (1 - percentHanning) * LorentzianY;
            return intensity;
        }
        #endregion
    }
}