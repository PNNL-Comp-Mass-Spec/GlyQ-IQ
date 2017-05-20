using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

namespace GetPeaks_DLL.Objects.TandemMSObjects
{
    public abstract class TandemObjectAbstract : IDisposable
    {
        #region properties

        /// <summary>
        /// the mass of the ion in the center of the fragmentation window
        /// </summary>
        public double PrecursorMZ { get; set; }

        /// <summary>
        /// which scan number the precursor was fragmented from
        /// </summary>
        public Int32 PrecursorScanNumber { get; set; }

        /// <summary>
        /// storage for precursor raw XYData
        /// </summary>
        public List<XYData> PrecursorData { get; set; }

        /// <summary>
        /// the scan number of the fragmentation spectra
        /// </summary>
        public Int32 FragmentationScanNumber { get; set; }

        /// <summary>
        /// storage for fragmentation raw XYData
        /// </summary>
        public List<XYData> FragmentationData { get; set; }

        /// <summary>
        /// MSlevel from decon tools 2=msms
        /// </summary>
        public int FragmentationMSLevel { get; set; }

        /// <summary>
        /// This is needd to link the data on the disk so we can load XYdata
        /// </summary>
        public InputOutputFileName InputFileName { get; set; }

        #endregion

        public abstract void LoadFragmentationData();

        public abstract void LoadPrecursorData();

        public abstract void LoadPrecursorMasses();

        public abstract void PeakPickPrecursorData();


        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
