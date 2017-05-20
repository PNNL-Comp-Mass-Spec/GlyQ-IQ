using System;

namespace ParalellTargetsLibraryX64
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
        //public static void CalculateSplits(int numberOfElements, int numberOfSections, int rank, ref int startIndex, ref int stopIndex)
        //{
        //    // chunkSize = size / cores;
        //    double chunkSize = Math.Floor((double)numberOfElements / (double)numberOfSections);
        //    double remainder = (numberOfElements - (chunkSize * numberOfSections));//left over indexes to be tacked on the last chunk

        //    //             Console.WriteLine("chunk size is " + (int)chunkSize + " for " + numberOfChunks + " chunks. The rank is " + rank + ". LeftOverIndexes is=" + (int)remainder);
        //    //             Console.WriteLine("rank == numberOfChunks-1" + rank + "==" + (numberOfChunks - 1).ToString());
        //    int roundedChunkSize = (int)chunkSize;
        //    int minRange;
        //    int maxRange;

        //    //set the general case and then correct for the first and last rank
        //    //minRange = roundedChunkSize * rank + 1;
        //    //maxRange = roundedChunkSize * (rank + 1) + 1;

        //    if (rank == 0)//first batch
        //    {
        //        minRange = rank;
        //        maxRange = roundedChunkSize * (rank + 1);
        //    }
        //    else//middle
        //    {
        //        minRange = roundedChunkSize * (rank) + 1;
        //        maxRange = roundedChunkSize * (rank + 1);
        //    }
        //    if (rank == numberOfSections - 1)//last batch
        //    {
        //        minRange = roundedChunkSize * (rank) + 1;
        //        maxRange = numberOfElements - 1;//for <= in fore loop
        //    }

        //    //1v1 case
        //    if (rank == 0 && rank == numberOfSections - 1)//only one batch
        //    {
        //        minRange = rank;
        //        maxRange = numberOfElements - 1;//for <= in fore loop
        //    }


        //    startIndex = minRange;
        //    stopIndex = maxRange;
        //    //            Console.WriteLine("StartIndex = " + minRange + " StopIndex = " + maxRange);
        //}

        public static void CalculateSplits(int numberOfElements, int numberOfChunks, int rank, ref int startIndex, ref int stopIndex)
        {
            // chunkSize = size / cores;
            double chunkSize = Math.Floor((double)numberOfElements / (double)numberOfChunks);
            double remainder = (numberOfElements - (chunkSize * numberOfChunks));//left over indexes to be tacked on the last chunk


            int remainderMaxRangeAddOnPerRank;
            int remainderMinRangeAddOnPerRank;
            CalculateRemainderPerRank(rank, remainder, numberOfChunks, out remainderMinRangeAddOnPerRank, out remainderMaxRangeAddOnPerRank);

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
                //maxRange = roundedChunkSize * (rank + 1);
                maxRange = roundedChunkSize * (rank + 1) + remainderMaxRangeAddOnPerRank-1;//-1 for 0 point offiset
            }
            else//middle
            {
                //minRange = roundedChunkSize * (rank) + 1;
                //maxRange = roundedChunkSize * (rank + 1);

                minRange = roundedChunkSize * (rank) + 1 + remainderMinRangeAddOnPerRank - 1;//-1 for 0 point offiset
                maxRange = roundedChunkSize * (rank + 1) + remainderMaxRangeAddOnPerRank - 1;//-1 for 0 point offiset;
            }
            if (rank == numberOfChunks - 1)//last batch
            {
                minRange = roundedChunkSize * (rank) + 1 + remainderMinRangeAddOnPerRank - 1;//-1 for 0 point offiset
                //minRange = roundedChunkSize * (rank) + 1;
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

        private static int CalculateRemainderPerRank(int rank, double remainder, int numberOfChunks, out int remainderMinRangeAddOnPerRank, out int remainderMaxRangeAddOnPerRank)
        {
            remainderMinRangeAddOnPerRank = 0;
            remainderMaxRangeAddOnPerRank = 0;

            if (remainder > 0)//remaider should always be less than a chunk size
            {
                //now we need to find if we are on a rank that gets a remainder
                if (rank < remainder) //when rank = remainder, all the remainder has been used up
                {
                    if(rank==0)
                    {
                        remainderMinRangeAddOnPerRank = 0;//min has not been shifted
                        remainderMaxRangeAddOnPerRank += 1;
                    }
                    else
                    {
                        remainderMinRangeAddOnPerRank += 0;//takes care of +1 offst for first remainder
                        remainderMaxRangeAddOnPerRank += 1;

                        for (int r = 0; r < rank; r++)//-1 for first point
                        {
                            remainderMinRangeAddOnPerRank += 1;
                            remainderMaxRangeAddOnPerRank += 1;
                        }
                       
                    }
                }
                else
                {
                    for (int r = 0; r < remainder; r++)//perminent shift for those not getting a bonus
                    {
                        remainderMinRangeAddOnPerRank += 1;
                        remainderMaxRangeAddOnPerRank += 1;
                    }
                    //remainderMaxRangeAddOnPerRank++;//offset??
                }
            }
            else
            {
                //remainderMaxRangeAddOnPerRank++;
            }
            return remainderMinRangeAddOnPerRank;
        }
    }
}
