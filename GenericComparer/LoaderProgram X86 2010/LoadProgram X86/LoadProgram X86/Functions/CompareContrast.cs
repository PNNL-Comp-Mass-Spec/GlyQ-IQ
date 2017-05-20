using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

namespace ConsoleApplication1
{
    class CompareContrast
    {
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
                XYData XY = new XYData();
                XY.X = (double)spectraMasses[i].Mass;
                massList1.Add(XY);
            }

            //List<double> MassList1 = new List<double>();
            List<XYData> massList2 = new List<XYData>();
            int length2 = spectraLibrary.Count;
                for (i = 0; i < length2; i++)
            {
                XYData XY = new XYData();
                XY.X = spectraLibrary[i].X;
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
