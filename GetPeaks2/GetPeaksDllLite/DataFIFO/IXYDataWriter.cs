using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Peaks;

namespace GetPeaksDllLite.DataFIFO
{
    public interface IXYDataWriter
    {
        int WriteOmicsXYData(List<PNNLOmics.Data.XYData> XYDataList, string path);
        int WriteDeconXYDataDeconTools(Run32.Backend.Data.XYData XYData, string path);
        int WriteOmicsProcesedPeakData(List<ProcessedPeak> peakDataList, string path);
    }
}
