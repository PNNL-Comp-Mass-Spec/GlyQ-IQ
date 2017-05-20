namespace GetPeaks_DLL.Parallel
{
    public abstract class ParalellResults
    {
        public int ThreadNumber {get;set;}

        public int EngineNumber { get; set; }

        public ParalellResults()
        {

        }

        public ParalellResults(int thread, int engineNumber)
        {
            ThreadNumber = thread;
            EngineNumber = engineNumber;
        }
    }
}
