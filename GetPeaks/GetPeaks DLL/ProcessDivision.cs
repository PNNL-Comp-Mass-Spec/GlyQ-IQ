using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL
{
    public class ProcessDivision
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberOfElements"></param>
        /// <param name="numberOfChunks"></param>
        /// <param name="rank">starts with 0</param>
        /// <param name="startIndex"></param>
        /// <param name="stopIndex"></param>
        public void CalculateSplits(int numberOfElements, int numberOfChunks, int rank, ref int startIndex, ref int stopIndex)
        {
            // chunkSize = size / cores;
            double chunkSize = Math.Floor((double)numberOfElements / (double)numberOfChunks);
            double remainder = (numberOfElements - (chunkSize * numberOfChunks));//left over indexes to be tacked on the last chunk

            int remainderMinRangeAddOn = 0;
            int remainderMaxRangeAddOn = 0;

            if(remainder>0)
            {
                int counter = 0;
                for (int r = 0; r < rank;r++ )
                {
                    if(counter<remainder)
                    {
                        remainderMinRangeAddOn++;
                        remainderMaxRangeAddOn++;
                    }
                }
            }

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
                //minRange = rank;
                minRange = 0;
                maxRange = roundedChunkSize * (rank + 1);
            }
            else//middle
            {
                //minRange = roundedChunkSize * (rank) + 1;
                //maxRange = roundedChunkSize * (rank + 1);

                minRange = roundedChunkSize * (rank) + 1 + remainderMinRangeAddOn;
                maxRange = roundedChunkSize * (rank + 1) + remainderMaxRangeAddOn;
            }
            if (rank == numberOfChunks - 1)//last batch
            {
                minRange = roundedChunkSize * (rank) + 1 + remainderMinRangeAddOn;
                minRange = roundedChunkSize * (rank) + 1;
                maxRange = numberOfElements - 1;//for <= in fore loop
            }

            //1v1 case
            if (rank == 0 && rank == numberOfChunks - 1)//only one batch
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
