using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.CompareContrast
{
    public class CompareControllerOld
    {
        public void compareFX(List<decimal> libraryMasses, List<decimal> dataMasses, CompareResults Results, double tollerance)
        {
            libraryMasses.Sort();
            dataMasses.Sort();

            Results.tollerance = tollerance;

            CompareResults ComparisonResults = new CompareResults();

            IRapidCompare compareHere = new CompareContrast2_Old();
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

            List<int> allIndexA = new List<int>(); List<int> allIndexB = new List<int>();

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
            for (i = 0; i < allIndexA.Count - 1; i++)
            {
                if (allIndexA[i] + 1 != allIndexA[i + 1])//if the indexes are not incrementing by 1
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
                //this is used to print some version of conservation
                //Console.WriteLine("Aye Captain +" + massMatchB.Count + " matches and " + extraMatches + " extra matches");
            }
            else
            {
                Console.WriteLine("Walk the Plank, " + conservationOfAassigned + " " + conservationOfAexact + " Library - Data" + conservationOfBassigned + " " + conservationOfBexact + " Extra Hits:" + Results.extraHitCounter);
            }
        }
    }
}
