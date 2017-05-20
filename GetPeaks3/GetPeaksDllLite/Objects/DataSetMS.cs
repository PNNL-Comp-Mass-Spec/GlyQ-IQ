using System.Collections.Generic;
using PNNLOmics.Data;

namespace GetPeaksDllLite.Objects
{
    public class DataSetMS
    {
        public DataSetMS()
        {
            this.XYList = new List<XYData>();
            this.DataSetInfo = new List<string>();
        }
        
        /// <summary>
        /// LoadedXYData, such as a library.  Perhaps extend this to RT and DT
        /// </summary>
        public List<XYData> XYList { get; set; }

        /// <summary>
        /// information about this XYData
        /// </summary>
        public List<string> DataSetInfo { get; set; }

        /// <summary>
        /// name of dataset
        /// </summary>
        public string Name { get; set; }
    }
}
