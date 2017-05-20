using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HammerPeakDetector.Parameters;

namespace HammerPeakDetector.Utilities
{
    public class PointsPerRegion
    {
        /// <summary>
        /// Finds the number of points to be used per noise threshold region (used with old thresholding method)
        /// </summary>
        /// <param name="numClusters"></param>
        /// <param name="parameters"></param>
        public void FindPointsPerRegion(int numClusters, HammerThresholdParameters parameters)
        {
            List<int> pointsPerRegion = new List<int>();
            int idealRegion = parameters.MinimumSizeOfRegion;
            int numRegions = numClusters / idealRegion;
            int leftToRegions = numClusters - (idealRegion * numRegions);

            //Adds the minimum number of peaks per noise region automatically
            for (int i = 0; i < numRegions; i++)
            {
                pointsPerRegion.Add(idealRegion);
            }

            //Distributes the remaining clusters evenly among noise regions
            int index = numRegions - 1;
            while (leftToRegions > 0)
            {
                pointsPerRegion[index]++;
                leftToRegions--;
                if (index < 1)
                {
                    index = numRegions - 1;
                }
                else 
                {
                    index--;
                }
            }

            //Changes parameters to include the noise threshold points
            parameters.NumberOfPointsPerNoiseRegion = pointsPerRegion;
        }
    }
}
