using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using PNNLOmics.Data;

namespace GetPeaks_DLL.Functions
{
    public static class ConvertIsotopeObjects
    {
        public static IsotopeObject Convert(IsosObject iObject)
        {
            
            IsotopeObject newIsotopeObject = new IsotopeObject();
            newIsotopeObject.Charge = iObject.charge;
            newIsotopeObject.ExperimentMass = iObject.mz;
            newIsotopeObject.FitScore = iObject.fit;
            newIsotopeObject.IsotopeList = new List<Peak>();
            newIsotopeObject.MonoIsotopicMass = iObject.monoisotopic_mw;
            newIsotopeObject.IsotopeIntensityString = iObject.abundance.ToString();

            return newIsotopeObject;
        }

        public static IsosObject Convert(IsotopeObject iObject, int scanNum)
        {
            IsosObject newIsosObject = new IsosObject();
            newIsosObject.abundance = iObject.IsotopeList[0].Height;
            newIsosObject.average_mw = iObject.ExperimentMass;
            newIsosObject.charge = iObject.Charge;
            newIsosObject.fit = (float)iObject.FitScore;

            Peak maxPeak = iObject.IsotopeList[0];
            foreach (Peak peak in iObject.IsotopeList)
            {
                if(peak.Height > maxPeak.Height)
                {
                    maxPeak = peak;
                }
            }
            newIsosObject.fwhm = maxPeak.Width;

            newIsosObject.mono_abundance = iObject.IsotopeList[0].Height;
            newIsosObject.monoisotopic_mw = iObject.MonoIsotopicMass;
            newIsosObject.mz = iObject.ExperimentMass;
            newIsosObject.scan_num = scanNum;

            newIsosObject.signal_noise = maxPeak.LocalSignalToNoise;

            return newIsosObject;
        }
    }
}
