using System.Collections.Generic;
using PNNLOmics.Data.Peaks;

namespace GetPeaksDllLite.DataFIFO
{
    public interface IPeakWriter
    {
        int WriteOmicsPeakData(List<PNNLOmics.Data.Peak> DataList, int scanNumber, string path);
        int WriteOmicsProcessedPeakData(List<ProcessedPeak> DataList, int scanNumber, string path);
    }
}
