using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using GetPeaks_DLL.Objects;
using PNNLOmics.Data.Constants;

namespace GetPeaks_DLL.AnalysisSupport.Tools
{
    public class ConvertToAminoGlycan
    {
        public void Convert(ref List<DataSet> dataSetIn, int index)
        {
            double MassAminoGlycan = Constants.MiscellaneousMatter[MiscellaneousMatterName.AminoGlycan].MassMonoIsotopic;
            double massAldehyde = Constants.MiscellaneousMatter[MiscellaneousMatterName.Aldehyde].MassMonoIsotopic;
            double massDelta = massAldehyde - MassAminoGlycan;
            float massDeltaFloat = (float)massDelta;
            foreach (XYData datapoint in dataSetIn[index].XYList)
            {
                datapoint.X = datapoint.X - massDeltaFloat;
            }
        }
    }
}
