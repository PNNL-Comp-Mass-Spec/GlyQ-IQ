namespace Parallel
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
