using GetPeaks_DLL.Objects.Enumerations;

namespace Parallel.THRASH
{
    public class ParalellThreadDataTHRASH: ParalellThreadData
    {
        //public int NumberOfScansToSum { get; set; }

        //public DeconTools.Backend.Core.ScanSet scanSet { get; set; }

        public PeakDetectors PeakDetectionMethod { get; set; }


        public ParalellThreadDataTHRASH()
        {
          
        }

        public ParalellThreadDataTHRASH(ParalellEngine engine, int scan)
        {
            this.Engine = engine;
            this.Scan = scan;
        }
    }
}
