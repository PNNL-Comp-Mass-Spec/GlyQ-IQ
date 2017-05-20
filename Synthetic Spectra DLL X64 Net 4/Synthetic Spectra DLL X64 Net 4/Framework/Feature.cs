using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

namespace Synthetic_Spectra_DLL_X64_Net_4.Framework
{
    public abstract class Feature : BaseData
    {
        private const int CONST_DEFAULT_SCAN_VALUE = -1;

        #region Properties
        /// <summary>
        /// Gets or sets the ID 
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// Gets or sets the drift time of the feature.
        /// </summary>
        public float DriftTime { get; set; }
        /// <summary>
        /// Gets or sets the monoisotopic mass of the feature.
        /// </summary>
        public double MassMonoisotopic { get; set; }
        /// <summary>
        /// Gets or sets the monoisotopic mass (aligned) of the feature.
        /// </summary>
        public double MassMonoisotopicAligned { get; set; }
        /// <summary>
        /// Gets or sets the normalized elution time (NET) of the feature.
        /// </summary>
        public double NET { get; set; }
        /// <summary>
        /// Gets or sets the aligned NET of the feature.
        /// </summary>
        public double NETAligned { get; set; }
        /// <summary>
        /// Gets or sets the scan of the feature from the raw data.
        /// </summary>
        public int Scan { get; set; }
        /// <summary>
        /// Gets or sets the aligned scan of the feature.
        /// </summary>
        public int ScanAligned { get; set; }
        /// <summary>
        /// Gets or sets the abundance of the feature.
        /// </summary>
        public int Abundance { get; set; }
        /// <summary>
        /// Gets or sets the M/Z value of the feature.
        /// </summary>
        public double MZ { get; set; }
        /// <summary>
        /// Gets or sets the charge state of the feature.
        /// </summary>
        public int ChargeState { get; set; }
        #endregion

        #region BaseData Members
        /// <summary>
        /// Clears the datatype and resets the raw values to their default values.
        /// </summary>
        public override void Clear()
        {
            Abundance = 0;
            ChargeState = 0;
            DriftTime = 0;
            ID = -1;
            MassMonoisotopic = double.NaN;
            MassMonoisotopicAligned = double.NaN;
            MZ = double.NaN;
            NET = double.NaN;
            NETAligned = double.NaN;
            Scan = CONST_DEFAULT_SCAN_VALUE;
            ScanAligned = CONST_DEFAULT_SCAN_VALUE;
        }
        #endregion
    }
}
