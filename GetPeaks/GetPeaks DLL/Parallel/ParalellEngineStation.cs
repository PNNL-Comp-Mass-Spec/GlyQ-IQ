using System.Collections.Generic;

namespace GetPeaks_DLL.Parallel
{
    public abstract class ParalellEngineStation
    {
        public List<ParalellEngine> Engines { get; set;}

        public List<ParalellEngine> ExtraEngines { get; set; }

        public object EngineLock { get; set; }

        public List<List<string>> ErrorPile { get; set; }

        public ParalellEngineStation()
        {
            Engines = new List<ParalellEngine>();

            ExtraEngines = new List<ParalellEngine>();

            ErrorPile = new List<List<string>>();

            EngineLock = new object();
        }
    }
}
