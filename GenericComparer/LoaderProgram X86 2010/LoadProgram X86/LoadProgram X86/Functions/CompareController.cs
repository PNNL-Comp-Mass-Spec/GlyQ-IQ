using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using System.Threading;
using System.Collections.Specialized;

namespace ConsoleApplication1
{
    public class CompareController
    {
        //const int cores = 2;
        //toleracne is in Da?
        public void compareSK(List<DataSetSK> MSData, CompareResultsSet compareResults, string name, double tollerance, int startIndex)
        {

//            Console.WriteLine("Inside Compare");
            List<decimal> libraryMasses = new List<decimal>();
            List<decimal> dataMasses = new List<decimal>();
           
            //unit tests
            //libraryIndexes.Add(0); libraryIndexes.Add(1); libraryIndexes.Add(2); libraryIndexes.Add(3); libraryIndexes.Add(4);libraryIndexes.Add(5);
            //dataIndexes.Add(0); dataIndexes.Add(1); dataIndexes.Add(2); dataIndexes.Add(3); dataIndexes.Add(4); dataIndexes.Add(5);
            
            //standard// libraryMasses.Add(10m); libraryMasses.Add(12m); libraryMasses.Add(13m); libraryMasses.Add(14m); libraryMasses.Add(18m); libraryMasses.Add(18.1m); libraryMasses.Add(20m); libraryMasses.Add(25m); libraryMasses.Add(26m);
            //standard//dataMasses.Add(7m); dataMasses.Add(13.2m); dataMasses.Add(13.8m); dataMasses.Add(18m); dataMasses.Add(18.2m); dataMasses.Add(18.3m); dataMasses.Add(24m); dataMasses.Add(25.9m); dataMasses.Add(26m); dataMasses.Add(26.1m);

            //libraryMasses.Add(10m); libraryMasses.Add(11m); libraryMasses.Add(12m); libraryMasses.Add(13m); libraryMasses.Add(14m); libraryMasses.Add(15m); libraryMasses.Add(16m); libraryMasses.Add(17m); libraryMasses.Add(18m);
            //dataMasses.Add(10m); dataMasses.Add(11m); dataMasses.Add(12m); dataMasses.Add(13m); dataMasses.Add(14m); dataMasses.Add(15m); dataMasses.Add(16m);dataMasses.Add(17m); dataMasses.Add(18m);

            //libraryMasses.Add(10m);
            //dataMasses.Add(10m);

            //controller for comare.  make sure there are 2 lists of decimals going in
            for (int j = 0; j < MSData[0].XYList.Count; j++)
            {
                dataMasses.Add((decimal)MSData[0].XYList[j].X);
            }

            for (int j = 0; j < MSData[2].XYList.Count; j++)
            {
                libraryMasses.Add((decimal)MSData[2].XYList[j].X);
            }

            //pull inputs outside to be faster
            

            //make the call
            CompareResults Results = new CompareResults();
            compareFX(libraryMasses, dataMasses, Results, tollerance);// abstracted call:  accepts a library mass and data mass and returns indexes

            //append results
            Results.IndexName = name;
            Results.startIndex = startIndex;

            List<CompareResults> runningList= compareResults.resultsList;
            runningList.Add(Results);

            return;
        }

        private void compareFX(List<decimal> libraryMasses, List<decimal> dataMasses, CompareResults Results, double tollerance)
        {
            libraryMasses.Sort();
            dataMasses.Sort();

            CompareResults ComparisonResults = new CompareResults();

            IRapidCompare compareHere = new CompareContrast2();
            for (int m = 0; m < 1; m++)//speed testing
            {
                compareHere.CompareContrast(libraryMasses, dataMasses, Results, tollerance);//final call to algorithm which will return indexes
                //compareHere.CompareOnly(libraryMasses, dataMasses, Results, tollerance);
            }

            ////put together output
            int i;

            //slow but uses linq
                //var massMatchA = (from n in Results.IndexListAMatch select libraryMasses[Results.IndexListAMatch[n]]).ToList();
                //var massMatchB = (from n in Results.IndexListAMatch select libraryMasses[Results.IndexListBMatch[n]]).ToList();
                //var massAonly = (from n in Results.IndexListAandNotB select dataMasses[Results.IndexListAandNotB[n]]).ToList();
                //var massBonly = (from n in Results.IndexListBandNotA select libraryMasses[Results.IndexListBandNotA[n]]).ToList();

           
            //faster but iterates and is more code
                List<decimal> massMatchA = new List<decimal>(); List<decimal> massMatchB = new List<decimal>();
                List<decimal> massAonly = new List<decimal>(); List<decimal> massBonly = new List<decimal>();
                
                List<int> allIndexA = new List<int>();List<int> allIndexB = new List<int>();

                for (i = 0; i < Results.IndexListAMatch.Count; i++)
                {
                    massMatchB.Add(libraryMasses[Results.IndexListBMatch[i]]);//library matches
                    massMatchA.Add(dataMasses[Results.IndexListAMatch[i]]);//data matches
                    allIndexA.Add(Results.IndexListAMatch[i]);
                    allIndexB.Add(Results.IndexListBMatch[i]);
                }

                for (i = 0; i < Results.IndexListAandNotB.Count; i++)
                {
                    massAonly.Add(dataMasses[Results.IndexListAandNotB[i]]);//library only
                    allIndexA.Add(Results.IndexListAandNotB[i]);
                    
                }

                for (i = 0; i < Results.IndexListBandNotA.Count; i++)
                {
                    massBonly.Add(libraryMasses[Results.IndexListBandNotA[i]]);//data only
                    allIndexB.Add(Results.IndexListBandNotA[i]);
                }


            //check to see that all points are accounted for
                int matchesA = 0; int matchesB = 0;
                int Alist = 0; int Blist = 0;
                int counter = 0;    
            
                allIndexA.Sort();
                for (i = 0; i < allIndexA.Count-1; i++)
                {
                    if (allIndexA[i]+1 != allIndexA[i+1])//if the indexes are not incrementing by 1
                    {
                        counter++;                  
                    }
                }
                matchesA = counter;
                Alist = allIndexA.Count - matchesA;

                counter = 0;
                allIndexB.Sort();
                for (i = 0; i < allIndexB.Count - 1; i++)
                {
                    if (allIndexB[i] + 1 != allIndexB[i + 1])//if the indexes are not incrementing by 1
                    {
                        counter++;
                    }
                }
                matchesB = counter;
                Blist = allIndexB.Count - matchesB;


            //int matchesOfassigned = massMatchB.Count - Results.extraHitCounter;
            //int conservationOfAassigned = massAonly.Count + massMatchA.Count;  //number of library points assigned
            //int conservationOfAexact = libraryMasses.Count;  //number of library points
            //int conservationOfBassigned = massBonly.Count + massMatchB.Count - Results.extraHitCounter;//number of data points assigned - multiple hits with near by libraries
            //int conservationOfBexact = dataMasses.Count;//number of data points

            int conservationOfAassigned = Alist;
            int conservationOfBassigned = Blist;
            int conservationOfAexact = dataMasses.Count;
            int conservationOfBexact = libraryMasses.Count;
            int extraMatches = matchesB + matchesA;

            if (conservationOfAassigned == conservationOfAexact && conservationOfBassigned == conservationOfBexact)
            {
                Console.WriteLine("Aye Captain +" + massMatchB.Count + " matches and " + extraMatches + " extra matches");
            }
            else
            {
                Console.WriteLine("Walk the Plank, " + conservationOfAassigned + " " + conservationOfAexact + " Library - Data" + conservationOfBassigned + " " + conservationOfBexact + " Extra Hits:" + Results.extraHitCounter);
            }
        }
    }
}
