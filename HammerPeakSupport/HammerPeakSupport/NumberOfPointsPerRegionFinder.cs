using System;
using System.Collections.Generic;
using HammerPeakDetector;

namespace HammerPeakSupport
{
    public class NumberOfPointsPerRegionFinder
    {
        /// <summary>
        /// Creates a list of integers which define how many clusters will be in each mass region
        /// </summary>
        /// <param name="numberOfClusters"></param>
        /// <returns></returns>
        public List<int> FindNumberOfPointsPerRegionList(int numberOfClusters, int minNumberOfPointsPerRegion)
        {
            HammerThresholdParameters parameters = new HammerThresholdParameters();
            bool numberFound = false;
            int bestDivider = 0;
            int minimumSizeOfRegion = minNumberOfPointsPerRegion; //This number should be at least 30 but can be almost anything greater than that. 
                                                                      //It should not be larger than the total number of clusters.
            //int minimumSizeOfRegion = parameters.MinimumSizeOfRegion; //This number should be at least 30 but can be almost anything greater than that. 
                                                                      //It should not be larger than the total number of clusters.
            List<int> numPointsPerRegionList = new List<int>();
            int divider = minimumSizeOfRegion;

            //This loop finds the best divider. I.e. It finds the number of noise thresholds there should be to produce mass
            //regions containing a the minimunSizeOfRegion number of points
            while (numberFound == false && divider > 0)
            {
                if (numberOfClusters / divider >= minimumSizeOfRegion)
                {
                    bestDivider = divider;
                    numberFound = true;
                }
                divider--;
            }

            if(divider ==0)
            {
                bestDivider = 1;
                numberFound = true;
            }

            int idealRegionSize = Convert.ToInt32(numberOfClusters/bestDivider);
            int numTotalRegions = bestDivider;
            int numRemaining = numberOfClusters - (idealRegionSize * numTotalRegions);

            //Creates as many thresholds of idealRegionSize as possible
            for (int i = 0; i < numTotalRegions - numRemaining; i++)
            {
                numPointsPerRegionList.Add(idealRegionSize);
            }

            //Creates remaining threholds that are 1 + idealRegionSize so that clusters can be evenly spread out among regions
            for (int i = 0; i < numRemaining; i++)
            {
                numPointsPerRegionList.Add(idealRegionSize + 1);
            }

            return numPointsPerRegionList;
        }
    }
}
