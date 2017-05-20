using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;

namespace GetPeaks_DLL.Functions
{
    public class ConvertOmicsProcessedPeaksToDeconPeaks
    {
        //public List<DeconTools.Backend.Core.Peak> ConvertPeaks(List<PNNLOmics.Data.ProcessedPeak> omicsPeaks)
        //{
        //    List<DeconTools.Backend.Core.Peak> results = new List<Peak>();
        //    foreach (PNNLOmics.Data.ProcessedPeak oPeak in omicsPeaks)
        //    {
        //        DeconTools.Backend.Core.Peak dPeak = new Peak();
        //        dPeak.XValue = oPeak.XValue;
        //        dPeak.Height = Convert.ToSingle(oPeak.Height);
        //        dPeak.Width = oPeak.Width;
        //        results.Add(dPeak);
        //    }
        //    return results;
        //}

        public List<DeconTools.Backend.Core.MSPeak> ConvertPeaks(List<PNNLOmics.Data.ProcessedPeak> omicsPeaks)
        {
            List<DeconTools.Backend.Core.MSPeak> results = new List<MSPeak>();
            foreach (PNNLOmics.Data.ProcessedPeak oPeak in omicsPeaks)
            {
                DeconTools.Backend.Core.MSPeak dPeak = new MSPeak();
                dPeak.XValue = oPeak.XValue;
                dPeak.Height = Convert.ToSingle(oPeak.Height);
                dPeak.Width = oPeak.Width;
                dPeak.SignalToNoise = (float)oPeak.SignalToNoiseGlobal;
                results.Add(dPeak);
            }
            return results;
        }

        public List<DeconTools.Backend.Core.Peak> ConvertPeaksForRun(List<PNNLOmics.Data.ProcessedPeak> omicsPeaks)
        {
            List<DeconTools.Backend.Core.MSPeak> results = new List<MSPeak>();
            foreach (PNNLOmics.Data.ProcessedPeak oPeak in omicsPeaks)
            {
                DeconTools.Backend.Core.MSPeak dPeak = new MSPeak();
                dPeak.XValue = oPeak.XValue;
                dPeak.Height = Convert.ToSingle(oPeak.Height);
                dPeak.Width = oPeak.Width;
                dPeak.SignalToNoise = (float)oPeak.SignalToNoiseGlobal;
                results.Add(dPeak);
            }


            var peakList = new List<Peak>();
            foreach (MSPeak msPeak in results)
            {
                peakList.Add(msPeak);
            }
            return peakList;
        }
        
    }
}
