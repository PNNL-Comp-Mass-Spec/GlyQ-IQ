using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

using MathNet.Numerics;
using MathNet.Numerics.Interpolation;
using MathNet.Numerics.Interpolation.Algorithms;
using MathNet.Numerics.LinearAlgebra;
using Synthetic_Spectra_DLL_X64_Net_4.Framework;
using CompareContrast_DLL;

namespace Synthetic_Spectra_DLL_X64_Net_4.Synthetic_Spectra_Module
{
    class SyntheticSpectraIncorporate
    {
        //TODO: I am still not sur where the negative numbers are comming from
        const decimal negativeIntensityFillValue = 0.1M;
        bool fillNegativeNumbers = true;
        bool fillNegativeNumbersWithFixedValue = false;
        decimal PercentDecayValue = 0.25M; //when a point is interpolated to a negative value, fill with a value that is PercentDecayValue * previousValue.
        //this allows for adding non zero numbersthat are related to the data before by some factor.  Perhaps use 0.5 or 0.1 to get a value near zero
        
        public List<PeakDecimal> AllignNoise(List<PeakDecimal> spectra, List<XYData> noiseSpectra, decimal peak_spacing)
        {
            //returns a spectra interpoated onto the grid

            List<PeakDecimal> gridSpectra = new List<PeakDecimal>();  //where we store the new intensities

            //holds the results from the compare only algorithm
            List<int> splineIndexes = new List<int>();
            List<int> massIndexes = new List<int>();
            List<decimal> Masses = new List<decimal>();
            //CompareContrast CompareContrastX = new CompareContrast();
            SyntheticSpectraIncorporate CompareInternal = new SyntheticSpectraIncorporate();
            CompareInternal.CompareOnly(splineIndexes, massIndexes, Masses, spectra, noiseSpectra);


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
                    //Add negative number correctoin
                    NewPeak.Intensity = (decimal)addedValue;
                    if (fillNegativeNumbers)
                    {
                        NewPeak.Intensity = (decimal)addedValue;
                        if (addedValue < 0)
                        {
                            if (fillNegativeNumbersWithFixedValue)
                            {
                                NewPeak.Intensity = negativeIntensityFillValue;
                            }
                            else
                            {
                                NewPeak.Intensity = gridSpectra[gridSpectra.Count-1].Intensity * PercentDecayValue;//if point is splined to a negative value, use half of the previous points intensity
                            }
                        }

                    }
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

        public void CompareOnly(List<int> libraryIndexes, List<int> massIndexes, List<decimal>masses, List<PeakDecimal> spectraMasses, List<XYData> spectraLibrary)
        {
            int i = 0;
            int j = 0;

            //abstract out the 2 lists of masses
            //List<double> MassList2 = new List<double>();
            List<XYData> massList1 = new List<XYData>();
            int length1 = spectraMasses.Count;
            for (i = 0; i < length1; i++)
            {
                XYData XY = new XYData((float)spectraMasses[i].Mass,0);
                //XY.X = (double)spectraMasses[i].Mass;
                massList1.Add(XY);
            }

            //List<double> MassList1 = new List<double>();
            List<XYData> massList2 = new List<XYData>();
            int length2 = spectraLibrary.Count;
                for (i = 0; i < length2; i++)
            {
                XYData XY = new XYData(spectraLibrary[i].X, 0);
                //XY.X = spectraLibrary[i].X;
                massList2.Add(XY);
            }
        
            int mTol =0;
            
            int maxSize = length2;
            if(length1>maxSize)
            {
                maxSize = length1;
            }
		
            i=0;
		    j=0;
	
		    int sameCounter=0;  //in both lists
            int totalCounter = 0;
    		
            int lowest=0;  //lowest index unassigned


            List<double> match = new List<double>();
            List<int> matchIndex = new List<int>();
            List<int> matchMZIndex = new List<int>();
            List<double> matchPPM = new List<double>();
		    
            do
            {
                if (massList1[j].X > massList2[i].X - mTol) //method origional  //this finds the first of a series
                    {
                        //find all library matches associated with this Data	
					    lowest=j;

                        if (massList1[j].X < massList2[i+1].X + mTol)//+
					    {
                            for (j=lowest;j<length1;j++)
                            {
                                if (massList1[j].X <= massList2[i+1].X + mTol)  //method 2
                                {
								    //print "---accept"
                                    match.Add(massList1[j].X);//Spectra[i].Mass  the mass of the hit
								    matchIndex.Add(j);  //the index of the hit
								    matchMZIndex.Add(i);  //the index of the library  //in this case index of coeff to use
								    matchPPM.Add(Math.Abs((massList2[i].X-massList1[j].X)/massList1[j].X*1000000));//how close we were
								    sameCounter++; totalCounter++;

                                    if (massList1[j].X <= massList2[i + 1].X - mTol)//-1?
								    {
                                        lowest=j;
									    //print "new lowest=j", MassList1[j], j
								    }
                                }
							    else
                                {
	    //							print "exit loop", Spectra[i].Mass+mTol
								    break;//stop extra looping
							    }
						    }//for
						    i++;  //next mass
						    j=lowest;
						    //end "Match"
					    }
					    else
                        {
						    //data must be more
	    //					printf "bump data %.4f\r", Spectra[i].Mass
						    i++;
					    }
                    }
				    else
                    {
					    //library is less 
	    //				printf "bump library %.4f\r", MassList1[j]
					    j+=1;
                        
				    }							
            } while (i < length2-1 && j < length1);	//do	//-1 was included because =mTols is not index+1	

            for (i = 0; i < sameCounter; i++)
            {
                libraryIndexes.Add(matchMZIndex[i]);
                massIndexes.Add(matchIndex[i]);
                masses.Add(spectraMasses[matchIndex[i]].Mass);
                
            }
        }
    }
}
        