using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetPeaks_DLL.Functions
{
    public static class ModifyList
    {
        public static List<int> RemoveDuplicateInts(List<int> indexesWithDuplicates)
        {
            //convertToTuples
            List<Tuple<int, int>> tuples = new List<Tuple<int, int>>();
            for (int i = 0; i < indexesWithDuplicates.Count; i++)
            {
                Tuple<int, int> newTuple = new Tuple<int, int>(i, indexesWithDuplicates[i]);
                tuples.Add(newTuple);
            }

            //RemoveDuplicates
            List<Tuple<int, int>> distinctItems = tuples.GroupBy(x => x.Item2).Select(y => y.First()).ToList();

            //ConvertBackToInts
            List<int> indexesWithOutDuplicates = new List<int>();
            foreach (Tuple<int, int> point in distinctItems)
            {
                indexesWithOutDuplicates.Add(point.Item2);
            }

            return indexesWithOutDuplicates;
        }
    }
}
