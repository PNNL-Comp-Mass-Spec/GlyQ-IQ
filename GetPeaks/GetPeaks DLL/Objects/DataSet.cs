using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

namespace GetPeaks_DLL.Objects
{
    public class DataSet
    {
        public DataSet()
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
