using System;

namespace Parallel
{
    public static class ProcessDivisionParalell
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberOfElements">how many things we have in total</param>
        /// <param name="numberOfSections">how many groups we want to divide it into</param>
        /// <param name="rank">starts with 0.  Which group we want to access</param>
        /// <param name="startIndex">start index from selected rank</param>
        /// <param name="stopIndex">stop index from selected rank</param>
        public static void CalculateSplits(int numberOfElements, int numberOfSections, int rank, ref int startIndex, ref int stopIndex)
        {
            // chunkSize = size / cores;
            double chunkSize = Math.Floor((double)numberOfElements / (double)numberOfSections);
            double remainder = (numberOfElements - (chunkSize * numberOfSections));//left over indexes to be tacked on the last chunk

            //             Console.WriteLine("chunk size is " + (int)chunkSize + " for " + numberOfChunks + " chunks. The rank is " + rank + ". LeftOverIndexes is=" + (int)remainder);
            //             Console.WriteLine("rank == numberOfChunks-1" + rank + "==" + (numberOfChunks - 1).ToString());
            int roundedChunkSize = (int)chunkSize;
            int minRange;
            int maxRange;

            //set the general case and then correct for the first and last rank
            //minRange = roundedChunkSize * rank + 1;
            //maxRange = roundedChunkSize * (rank + 1) + 1;

            if (rank == 0)//first batch
            {
                minRange = rank;
                maxRange = roundedChunkSize * (rank + 1);
            }
            else//middle
            {
                minRange = roundedChunkSize * (rank) + 1;
                maxRange = roundedChunkSize * (rank + 1);
            }
            if (rank == numberOfSections - 1)//last batch
            {
                minRange = roundedChunkSize * (rank) + 1;
                maxRange = numberOfElements - 1;//for <= in fore loop
            }

            //1v1 case
            if (rank == 0 && rank == numberOfSections - 1)//only one batch
            {
                minRange = rank;
                maxRange = numberOfElements - 1;//for <= in fore loop
            }


            startIndex = minRange;
            stopIndex = maxRange;
            //            Console.WriteLine("StartIndex = " + minRange + " StopIndex = " + maxRange);
        }

    }
}
