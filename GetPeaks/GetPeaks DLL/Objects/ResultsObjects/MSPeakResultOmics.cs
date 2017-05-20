using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

namespace GetPeaks_DLL.Objects.ResultsObjects
{
    public class MSPeakResultOmics
    {

        public MSPeakResultOmics()
        {
            this.ChromID = -1;
            this.PeakID = -1;
            this.Scan_num = -1;
            this.Frame_num = -1;
            this.MSPeak = null;

        }
        public MSPeakResultOmics(int peakID, int scanNum, Peak peak):this()
        {
            this.PeakID = peakID;
            this.Scan_num = scanNum;
            this.MSPeak = peak;

        }

        public MSPeakResultOmics(int peakID, int frameNum, int scanNum, Peak peak)
            : this(peakID, scanNum, peak)
        {
            this.Frame_num = frameNum;
        }

        public int PeakID { get; set; }
        public int Scan_num { get; set; }

        /// <summary>
        /// Use this property to assign a peak to particular LC chromatogram
        /// </summary>
        public int ChromID { get; set; }

        public int Frame_num { get; set; }


        public Peak MSPeak { get; set; }
    }
}
