using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;

namespace GetPeaks_DLL.Objects
{
    public class XYDataAndPeakHolderObject:IDisposable
    {
        public List<PNNLOmics.Data.XYData> SpectraDataOMICS { get; set; }
        public DeconTools.Backend.XYData SpectraDataDECON { get; set; }

        //public List<DeconTools.Backend.Core.IPeak> PeaksDECON { get; set; }
        public List<DeconTools.Backend.Core.Peak> PeaksDECON { get; set; }
        public List<PNNLOmics.Data.ProcessedPeak> PeaksOMICS { get; set; }

        public ScanSet ScanSet { get; set; }
        public Int32 DatasetScanNumber { get; set; }

        public XYDataAndPeakHolderObject()
        {
            SpectraDataOMICS = new List<PNNLOmics.Data.XYData>();
            SpectraDataDECON = new DeconTools.Backend.XYData();
            //PeaksDECON = new List<DeconTools.Backend.Core.IPeak>();
            PeaksDECON = new List<DeconTools.Backend.Core.Peak>();
            PeaksOMICS = new List<PNNLOmics.Data.ProcessedPeak>();
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.PeaksDECON = null;
            this.PeaksOMICS = null;
            this.ScanSet = null;
            this.SpectraDataDECON = null;
            this.SpectraDataOMICS = null;  
        }

        #endregion
    }

    public class XYDataAndPeakHolderObjectInt64 : IDisposable
    {
        public SortedList<int, Int64> PeakListX64 { get; set; }
        public double[] PeakListXdouble { get; set; }
        
        public double[] PeakListY { get; set; }

        public ScanSet ScanSet { get; set; }
        public Int32 DatasetScanNumber { get; set; }

        public XYDataAndPeakHolderObjectInt64(int size)
        {
            PeakListX64 = new SortedList<int, Int64>();
            PeakListY = new double[0];
            for (int i = 0; i < size; i++)
            {
                PeakListX64.Add(i, 0);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.PeakListX64 = null;
            this.PeakListXdouble = null;
            this.PeakListY = null;
            this.ScanSet = null;
            
        }

        #endregion
    }
}
