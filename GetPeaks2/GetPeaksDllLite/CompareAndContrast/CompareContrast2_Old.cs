using System;
using System.Collections.Generic;
using PNNLOmics.Data;

//conditions tested, extra points added on before library and data, extra points after library and data,
//conditions tested, 1 library hitting several data points and 1 data point hitting severa libraries
//lists must be sorted first

//decimal test.  Floats will not cut it
//float flaotval = 100000.123456789123456789F;
//double doublaval = 100000.123456789123456789;
//decimal decilaval = 100000.123456789123456789M;
//decimal testF = Convert.ToDecimal(flaotval);
//decimal testD = Convert.ToDecimal(doublaval);
//decimal testDe = Convert.ToDecimal(decilaval);
////Console.WriteLine("Grand total:\t{0,8:c}", decimal test);
//Console.WriteLine(100000.123456789123456789 + " " + testF + " " + testD + " " + "  " + testDe);


namespace GetPeaksDllLite.CompareAndContrast
{
    public class CompareContrast2_Old : IRapidCompare
    {
        void IRapidCompare.CompareContrast(List<decimal> libraryMasses, List<decimal> dataMasses, CompareResults Results, double tolleranceIN)
        {
            int i = 0;
            int j = 0;
            int k = 0;

            //abstract out the 2 lists of masses
            List<XYData> massList1 = new List<XYData>();
            int length1 = dataMasses.Count;
            for (i = 0; i < length1; i++)
            {
                decimal hold = dataMasses[i];
                //XYData XY = new XYData((float)dataMasses[i],0);
                XYData XY = new XYData(Convert.ToDouble(dataMasses[i]), 0);
                //XY.X = (double)dataMasses[i];
                massList1.Add(XY);
            }

            List<XYData> massList2 = new List<XYData>();
            int length2 = libraryMasses.Count;
            for (i = 0; i < length2; i++)
            {
                //XYData XY = new XYData((float)libraryMasses[i],0);
                XYData XY = new XYData(Convert.ToDouble(libraryMasses[i]), 0);
                //XY.X = (double)libraryMasses[i];
                massList2.Add(XY);
            }

            int length2MinusOne = length2 - 1;
            int length2PlusONe = length2 + 1;
            int length1MinusOne = length1 - 1;

            double mTol = 0;//tolleranceIN;//currently Da

            int maxSize = length2;
            if (length1 > maxSize)
            {
                maxSize = length1;
            }

            int lowest = 0;  //lowest index unassigned
            
            //lists to store indexes
            List<int> indexesAnotB = new List<int>();//this is the index list that only are found in set A
            List<int> indexesBnotA = new List<int>();//this is the index list
            List<int> indexesMatchA = new List<int>();
            List<int> indexesMatchB = new List<int>();

            i = 0;
            j = 0;
            k = 0;

            do
            {
                mTol = ErrorCalculation(massList2[i].X, tolleranceIN, ErrorType.PPM);
                if (massList1[j].X >= massList2[i].X - mTol) //method origional  //this finds the first of a series
                {
                    //find all library matches associated with this Data	
                    lowest = j;//the lowest represents the lowest data that we are considering.  This prevents cycling through the entire data list each time

                    if (massList1[j].X <= massList2[i].X+mTol)//If true, there is at least one match
                    {
                        if (j < length1MinusOne)//if true, we are not an end point
                        {
                            if (massList1[j + 1].X <= massList2[i].X + mTol)//if true, there several hits data hits for a single library point
                            {
                                //several hits for a given library
                                for (k = lowest; k < length1; k++)//cycle thorugh multiple hits till then end of the list (usually breaked before end is reached)
                                {
                                    if (massList1[k].X <= massList2[i].X + mTol)  //if true, we are still in the window and can rack up several matches
                                    {
                                        indexesMatchB.Add(k);  //the index of the hit
                                        indexesMatchA.Add(i);  //the index of the library  //in this case index of coeff to use

                                        if (i < length2MinusOne)//if true, we are at the end of the library
                                        {
                                            if (massList1[k].X <= massList2[i + 1].X - mTol)//if true, the next library is less than this match so keep it as lowest
                                            {
                                                lowest = k + 1;//new lowest, +1 because we have exhaused the possible hits with the several hit process.  K+1 leads to next data point
                                            }
                                        }
                                        else 
                                        {
                                            //we are at the end so map K onto J and +1 because all possible hits are exhaused
                                            lowest = k +1;
                                        }
                                    }
                                    else//print "exit loop"
                                    {
                                        break;//stop extra looping.  we have filled in all of the possible matches for the library point
                                    }
                                }
                                i++;  //next mass
                                j = lowest;//update lowest 
                            }
                            else
                            {
                                //single hit for one library (in the middle of the library)
                                indexesMatchB.Add(j);  //the index of the hit
                                indexesMatchA.Add(i);  //the index of the library  //in this case index of coeff to use

                                //check to see if we should increment.  we may be able to match the next i
                                if (i < length2MinusOne)//if true, we are at the end of the library
                                {
                                    if (massList1[j].X < massList2[i + 1].X - mTol)//if the data we are looking at is greater than the next library mass, don't increment it
                                    {
                                        j++;
                                    }
                                    else
                                    {
                                        
                                    }
                                }
                                else//last point in library
                                {
                                    j++;
                                }
                                
                                i++;
                             }
                        }
                        else
                        {
                            //single hit  for one library (at the end of the data)
                            indexesMatchB.Add(j);  //the index of the hit
                            indexesMatchA.Add(i);  //the index of the library  //in this case index of coeff to use
                            
                            //don't increment the data if there are other libraries it can be matched to
                            if (i < length2MinusOne)
                            {
                                //if (massList1[j].X > massList2[i + 1].X + mTol)//old, data can end up adding to indexesAnotB.Add(j) 
                                if (massList1[j].X >= massList2[i+1].X - mTol && massList1[j].X <= massList2[i+1].X + mTol)//Is this mass in the windown of the next library new 8-2-2010
                                {
                                    //j++;//old //increment all because the last point data and library have been assigned and there are no several hits
                                    //data  has more hits//new add here 8-2-10
                                }
                                else 
                                {
                                    //data  has more hits//old remove from here 8-2-10
                                    j++;//8-2-2010 increment all because the last point data and library have been assigned and there are no several hits
                                }
                            }
                            else//last point in library, go ahead and increment
                            {
                                j++;
                            }
                            
                            i++;
                        }
                    }
                    else
                    {
                        //bump library up
                        indexesBnotA.Add(i);//add library index to B not A
                        i++;
                    }
                }
                else
                {
                    //bump data up
                    indexesAnotB.Add(j);//add data index to A not B
                    j++;
                }
            } while (i < length2 && j < length1);	//do	//-1 was included because =mTols is not index+1	
            
            //this takes care of the rest of points outside the scope of the library and outside of the data
            for (k = j; k < length1; k++)  //left over data points
            {
                indexesAnotB.Add(k);
            }

            for (k = i; k < length2; k++)//all massList2 points have been assigned
            {
                indexesBnotA.Add(k);//left over library points
            }
            
            Results.AddAandB(indexesMatchB, indexesMatchA);
            Results.AddAandNotB(indexesAnotB);
            Results.AddBandNotA(indexesBnotA);
        }

        void IRapidCompare.CompareOnly(List<decimal> libraryMasses, List<decimal> dataMasses, CompareResults Results, double tolleranceIN)
        {
            int i = 0;
            int j = 0;

            //abstract out the 2 lists of masses
            List<XYData> massList1 = new List<XYData>();
            int length1 = dataMasses.Count;
            for (i = 0; i < length1; i++)
            {
                XYData XY = new XYData(Convert.ToDouble(dataMasses[i]),0);
                //XY.X = (double)dataMasses[i];
                massList1.Add(XY);
            }

            List<XYData> massList2 = new List<XYData>();
            int length2 = libraryMasses.Count;
            for (i = 0; i < length2; i++)
            {
                XYData XY = new XYData(Convert.ToDouble(libraryMasses[i]),0);
                //XY.X = (double)libraryMasses[i];
                massList2.Add(XY);
            }
            ////double XY;
            ////List<double> massList1 = new List<double>();
            ////int length1 = spectraMasses.Count;
            ////for (i = 0; i < length1; i++)
            ////{
            ////    XY = (double)spectraMasses[i];
            ////    massList1.Add(XY);
            ////}

            ////List<double> massList2 = new List<double>();
            ////int length2 = libraryMasses.Count;
            ////for (i = 0; i < length2; i++)
            ////{
            ////    XY = (double)libraryMasses[i];
            ////    massList2.Add(XY);
            ////}

            double mTol = 0;// tolleranceIN;

            int maxSize = length2;
            if (length1 > maxSize)
            {
                maxSize = length1;
            }

            i = 0;
            j = 0;

            int lowest = 0;  //lowest index unassigned

            //lists to store indexes
            //List<int> indexesAnotB = new List<int>();//this is the index list that only are found in set A
            //List<int> indexesBnotA = new List<int>();//this is the index list
            List<int> indexesMatchA = new List<int>();
            List<int> indexesMatchB = new List<int>();
            
            do
            {
                mTol = ErrorCalculation(massList2[i].X, tolleranceIN, ErrorType.PPM);
                if (massList1[j].X > massList2[i].X - mTol) //method origional  //this finds the first of a series
                {
                    //find all library matches associated with this Data	
                    lowest = j;//the lowest represents the lowest data that we are considering.  This prevents cycling through the entire data list each time
                   
                    if (massList1[j].X < massList2[i].X+mTol)//If true, there is at least one match
                    {
                        if (j < length1 - 1)//if true, we are not an end point
                        {
                            if (massList1[j + 1].X < massList2[i].X + mTol)//if true, there several hits data hits for a single library point
                            {
                                //several hits for a given library
                                for (int k = lowest; k < length1; k++)//cycle thorugh multiple hits till then end of the list (usually breaked before end is reached)
                                {
                                    if (massList1[k].X <= massList2[i].X + mTol)  //if true, we are still in the window and can rack up several matches
                                    {
                                        //print "---accept"
                                        indexesMatchB.Add(k);  //the index of the hit
                                        indexesMatchA.Add(i);  //the index of the library  //in this case index of coeff to use

                                        if (i < length2 - 1)//if true, we are at the end of the library
                                        {
                                            if (massList1[k].X <= massList2[i + 1].X - mTol)//if true, the next library is less than this match so keep it as lowest
                                            {
                                                lowest = k + 1;//new lowest, +1 because we have exhaused the possible hits with the several hit process.  K+1 leads to next data point
                                            }
                                        }
                                        else 
                                        {
                                            //we are at the end so map K onto J and +1 because all possible hits are exhaused
                                            lowest = k+1;
                                        }
                                    }
                                    else//print "exit loop"
                                    {
                                        break;//stop extra looping.  we have filled in all of the possible matches for the library point
                                    }
                                }

                                i++;  //next mass
                                j = lowest;//update lowest 
                            }
                            else
                            {
                                //single hit for one library (in the middle of the library)
                                indexesMatchB.Add(j);  //the index of the hit
                                indexesMatchA.Add(i);  //the index of the library  //in this case index of coeff to use

                                //check to see if we should increment.  we may be able to match the next i
                                if (massList1[j].X < massList2[i + 1].X - mTol)//if the data we are looking at is greater than the next library mass, don't increment it
                                {
                                    j++;
                                }
                                i++;
                            }
                        }
                        else
                        {
                            //single hit  for one library (at the end of the data)
                            //indexesMatchB.Add(j);  //the index of the hit
                            //indexesMatchA.Add(i);  //the index of the library  //in this case index of coeff to use
                            
                            j++;//increment all because the last point data and library have been assigned and there are no several hits
                            i++;
                        }
                    }
                    else
                    {
                        //bump library up
                        //indexesAnotB.Add(i);//add library index to A not B
                        i++;
                    }
                }
                else
                {
                    //bump data up
                    //indexesBnotA.Add(j);//add data index to B not A
                    j++;

                }
            } while (i < length2 && j < length1);	//do	//-1 was included because =mTols is not index+1	
            
            //this takes care of the rest of points outside the scope of the library and outside of the data
            for (int k = j; k < length1; k++)  //left over data points
            {
                //indexesBnotA.Add(k);
            }

            for (int k = i; k < length2; k++)//all massList2 points have been assigned
            {
                //indexesAnotB.Add(k);//left over library points
            }
            
            Results.AddAandB(indexesMatchB, indexesMatchA);
            //Results.AddAandNotB(indexesAnotB);
            //Results.AddBandNotA(indexesBnotA);
        }

        public static double ErrorCalculation(double mass, double magnitude, ErrorType errorType)
        {
            double massError = magnitude;

            switch (errorType)
            {
                case ErrorType.PPM:
                    {
                        massError = (magnitude * mass) / 1000000;
                    }
                    break;
                case ErrorType.Dalton:
                    {
                        massError = magnitude;
                    }
                    break;
                default:
                    {
                        massError = magnitude;
                    }
                    break;
            }

            return massError;
        }

        public enum ErrorType
        {
            PPM,
            Dalton
        }
    }   
}

