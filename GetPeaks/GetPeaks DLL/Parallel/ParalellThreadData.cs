using GetPeaks_DLL.Parallel;
namespace GetPeaks_DLL.Parallel
{
    public abstract class ParalellThreadData
    {
        public ParalellEngine Engine { get; set; }

        public int Scan { get; set; }

        public ParalellThreadData()
        {
 
        }

        public ParalellThreadData(ParalellEngine engine, int scan)
        {
            Engine = engine;
            Scan = scan;
        }
    }
}
