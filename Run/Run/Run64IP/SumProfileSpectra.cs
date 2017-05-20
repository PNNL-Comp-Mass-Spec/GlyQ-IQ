using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InformedProteomics.Backend.Data.Spectrometry;
using InformedProteomics.Backend.MassSpecData;
using MathNet.Numerics.Interpolation;
using MathNet.Numerics.Interpolation.Algorithms;

namespace Run64IP
{
    public class SumProfileSpectra
    {
        /// <summary>
        /// ax2
        /// </summary>
        private double a { get; set; }

        /// <summary>
        /// bx
        /// </summary>
        private double b { get; set; }

        /// <summary>
        /// +c
        /// </summary>
        private double c { get; set; }

        /// <summary>
        /// used to simplify the fit coefficents to normal numbers
        /// </summary>
        private double factor = 1000000;
        

        public SumProfileSpectra()
        {
            a = 0.001;
            b = 10;
            c = 1000000;

        }

        public Peak[] getSummedSpectrum(ref XCaliburReader reader, List<int> scanSet, out List<Peak[]> individualSpectra)
        {
            // [gord] idea borrowed from Anuj! Jan 2010 

            //the idea is to convert the mz value to a integer. To avoid losing precision, we multiply it by 'precision'
            //the integer is added to a dictionary generic list (sorted)
            //
            individualSpectra = new List<Peak[]>();
            

            SortedDictionary<long, Peak> mz_intensityPair = new SortedDictionary<long, Peak>();
            double precision = 1e3;   // if the precision is set too high, can get artifacts in which the intensities for two m/z values should be added but are separately registered. 
            //double[] tempXvals = new double[0];
            //float[] tempYvals = new float[0];
            double[] tempXvals = null;
            Peak[] tempYvals = null;

            int spectraPointCount = 0;
            double[] baseAxis = new double[0];
            //long minXLong = (long)(minX * precision + 0.5);
            //long maxXLong = (long)(maxX * precision + 0.5);


            double[] rawMasses;
            List<Peak> differences;
            double[] rawIntensities;

            Spectrum rawSpecectra = reader.ReadMassSpectrum(scanSet[0]);
           

            spectraPointCount = SetCoefficientsAndXAxis(rawSpecectra, out rawMasses, out differences, out rawIntensities);
            double seedMassInitial = rawMasses[0];

            for (int scanCounter = 0; scanCounter < scanSet.Count; scanCounter++)
            {
                ////1.  pull unsummed mass spec
                //Spectrum rawSpecectra = reader.ReadMassSpectrum(scanSet[scanCounter]);
                //spectraPointCount = rawSpecectra.Peaks.Count();

                ////2.  create correct x range using FT and a b c
                //double[] rawMasses = new double[spectraPointCount];
                //double[] rawIntensities = new double[spectraPointCount];
                //List<Peak> differences = new List<Peak>();
               
                //for (var i = 0; i < rawSpecectra.Peaks.Count(); i++)
                //{
                //    rawMasses[i] = rawSpecectra.Peaks[i].Mz;
                //    rawIntensities[i] = rawSpecectra.Peaks[i].Intensity;
                //    if (i > 0)
                //    {
                //        if (rawIntensities[i] > 0 && rawIntensities[i - 1] > 0)
                //        {
                //            differences.Add(new Peak(rawMasses[i], Math.Exp(rawMasses[i] - rawMasses[i - 1])*factor));

                //        }
                //    }
                //}

                ////3.  calculate peak difference equation
               

                //double aOut = 0.001;
                //double bOut = 10;
                //double cOut = 1000000;
                //Parabola.ParabolaABC(differences,ref aOut, ref bOut, ref cOut);
                //double mxIn = rawMasses[0];
                //double mxIn = rawMasses[0];

                bool toTest = false;
                if (toTest)
                {
                    List<Peak> differencesCalc = new List<Peak>();
                    double FinalMass = rawMasses[rawMasses.Count() - 1];
                    double currentX = rawMasses[0];
                    while (currentX < FinalMass)
                    {
                        double differenceAtMz = DifferenceAtMz(currentX);
                        currentX += differenceAtMz;
                        differencesCalc.Add(new Peak(currentX, (Quadratic(currentX))));
                    }

                    FinalMass = 5000000;
                    

                    var closestMass = GetGridMassAboveBelowMass(seedMassInitial, FinalMass);

                    Console.WriteLine("The closest Mass to " + FinalMass + " is " + closestMass.Item1 +
                                      " and less than " + closestMass.Item2);

                    individualSpectra.Add(differencesCalc.ToArray());
                    individualSpectra.Add(differences.ToArray());
                }

                //if(scanCounter==0)
                //{
                //    spectraSize = rawSpecectra.Peaks.Count();
                //    double maxMass = rawSpecectra.Peaks[spectraSize - 1].Mz;
                //    double minMass = rawSpecectra.Peaks[0].Mz;
                //    double range = maxMass - minMass;
                //    baseAxis = new double[spectraSize];

                    
                //    for (int i = 0; i < spectraSize; i++)
                //    {
                //        baseAxis[i] = minMass + (double)i/ (spectraSize) * range;
                //    }

                    //create function for getting neatest point on grid
                    //interpolate to nearest point on grid.  linear might be good enough if the mass is close
                    //add to dictionary so that all points inthe dictionart are indexed to the nearest point
                    //this way we are adding interpolated intensties in the correct bins
                //}
               

               
                //MathNet.Numerics.Interpolation.Algorithms.EquidistantPolynomialInterpolation interpolator = new EquidistantPolynomialInterpolation(rawMasses, rawIntensities);
                //MathNet.Numerics.Interpolation.Algorithms.CubicSplineInterpolation interpolator = new CubicSplineInterpolation(rawMasses, rawIntensities);
                //MathNet.Numerics.Interpolation.Algorithms.LinearSplineInterpolation interpolator = new LinearSplineInterpolation(rawMasses, rawIntensities);
                //MathNet.Numerics.Interpolation.Algorithms.NevillePolynomialInterpolation interpolator = new NevillePolynomialInterpolation(rawMasses, rawIntensities);
                
                IInterpolation interpolator = MathNet.Numerics.Interpolation.Interpolate.Common(rawMasses, rawIntensities);

                Peak[] currentSpectra = new Peak[spectraPointCount];
                //double[] mapMz = new double[spectraPointCount];
                double[] mapIntensities = new double[spectraPointCount];
                for (int i = 0; i < spectraPointCount; i++)
                {
                    double rawMass = rawMasses[i];
                    var xToLookFor = GetGridMassAboveBelowMass(seedMassInitial,rawMasses[i]);
                    //find closes to interpolate to
                    double closestPole = SelectClosestPole(xToLookFor, rawMass);

                    //or use built in interpolation
                    double y = interpolator.Interpolate(closestPole);
                    mapIntensities[i] = y;

                    //if (rawMass > 275.35 || mapIntensities[i] > 0)
                    //{
                    //    Console.WriteLine(rawMass);
                    //}

                    currentSpectra[i] = new Peak(closestPole, mapIntensities[i]);

                    //long tempmz = Convert.ToInt64(Math.Truncate(closestPole * precision));
                    long tempmz = (long)(Math.Floor(closestPole * precision));
                    //if (tempmz < minXLong || tempmz > maxXLong) continue;

                    if (mz_intensityPair.ContainsKey(tempmz))
                    {
                        mz_intensityPair[tempmz].AddIntensity(mapIntensities[i]);
                    }
                    else
                    {
                        mz_intensityPair.Add(tempmz, new Peak(tempmz, mapIntensities[i]));
                    }
                }

                

                individualSpectra.Add(rawSpecectra.Peaks.ToArray());
                
                ////individualSpectra.Add(spec.Peaks);
                ////double value = spec.Peaks[0];
                //for (int i = 0; i < baseAxis.Length; i++)
                //{
                //    //piont shift the m/z to make bins
                //    long tempmz = (long)Math.Floor(baseAxis[i] * precision + 0.5);
                //    //if (tempmz < minXLong || tempmz > maxXLong) continue;

                //    if (mz_intensityPair.ContainsKey(tempmz))
                //    {
                //        mz_intensityPair[tempmz].AddIntensity(mapIntensities[i]);
                //    }
                //    else
                //    {
                //        mz_intensityPair.Add(tempmz, new Peak(tempmz, mapIntensities[i]));
                //    }
                //}
            }

            if (mz_intensityPair.Count == 0) return new Peak[0];

            List<long> summedXValsShifted = mz_intensityPair.Keys.ToList();
            
            Peak[] rawData = mz_intensityPair.Values.ToArray();

            for (int i = 0; i < summedXValsShifted.Count; i++)
            {
                rawData[i].UpdateMz(summedXValsShifted[i] / precision);
            }

            return rawData;
        }

        private int SetCoefficientsAndXAxis(Spectrum rawSpecectra, out double[] rawMasses, out List<Peak> differences,
                                            out double[] rawIntensities)
        {
            int spectraPointCount;
            //1.  pull unsummed mass spec
            
            spectraPointCount = rawSpecectra.Peaks.Count();

            //2.  create correct x range using FT and a b c
            rawMasses = new double[spectraPointCount];
            rawIntensities = new double[spectraPointCount];
            differences = new List<Peak>();

            for (var i = 0; i < rawSpecectra.Peaks.Count(); i++)
            {
                rawMasses[i] = rawSpecectra.Peaks[i].Mz;
                rawIntensities[i] = rawSpecectra.Peaks[i].Intensity;
                if (i > 0)
                {
                    if (rawIntensities[i] > 0 && rawIntensities[i - 1] > 0)
                    {
                        differences.Add(new Peak(rawMasses[i], Math.Exp(rawMasses[i] - rawMasses[i - 1])*factor));
                    }
                }
            }

            //3.  calculate peak difference equation


            double aOut = 0.001;
            double bOut = 10;
            double cOut = 1000000;
            Parabola.ParabolaABC(differences, ref aOut, ref bOut, ref cOut);

            a = aOut;
            b = bOut;
            c = cOut;
            
            return spectraPointCount;
        }

        private static double SelectClosestPole(Tuple<double, double> xToLookFor, double rawMass)
        {
            double closestPole;
            if (Math.Abs(rawMass - xToLookFor.Item1) < Math.Abs(xToLookFor.Item2 - rawMass))
            {
                return xToLookFor.Item1;
            }
                return xToLookFor.Item2;
        }

        private Tuple<double,double> GetGridMassAboveBelowMass(double seedMassInitial, double finalMass)
        {
            double massBefore = 0;
            double currentX = seedMassInitial;
            while (currentX < finalMass)
            {
                massBefore = currentX;
                currentX += DifferenceAtMz(currentX);
            }

            Tuple<double,double> results = new Tuple<double, double>(massBefore,currentX);
            return results;
        }

        private double GetGridMassAboveMass(double seedMassInitial, double finalMass)
        {
            double currentX = seedMassInitial;
            while (currentX < finalMass)
            {
                currentX += DifferenceAtMz(currentX);
            }

            return currentX;
        }

        private double DifferenceAtMz(double currentX)
        {
            return Math.Log(Quadratic(currentX) / factor);
        }

        private double Quadratic(double mxIn)
        {
            return a * mxIn * mxIn + b * mxIn + c;;
        }
    }
}
