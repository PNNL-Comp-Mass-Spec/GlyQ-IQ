using System.Collections.Generic;
using System.Linq;

namespace ParalellTargetsLibraryX64
{
    public static class ParalellSelectRange
    {
        public static List<int> Range(List<int> scans, int numberOfComputers, int rank)
        {
            int startIndex = 0;
            int stopIndex = 0;

            int totalNumberOfScans = scans.Count;

            ProcessDivisionParalell.CalculateSplits(totalNumberOfScans, numberOfComputers, rank, ref startIndex, ref stopIndex);

            var parallelQuery =
                from n in scans.AsParallel()
                where Enumerable.Range(0, totalNumberOfScans).All(i => n >= startIndex && n <= stopIndex)
                select n;

            List<int> selectedScans = parallelQuery.ToList<int>();
            return selectedScans;
        }
    }
}
