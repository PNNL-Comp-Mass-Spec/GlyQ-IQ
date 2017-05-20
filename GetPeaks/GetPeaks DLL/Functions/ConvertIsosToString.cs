using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.DataFIFO;

namespace GetPeaks_DLL.Functions
{
    public class ConvertIsosToString
    {
        public void Convert(List<IsosObject> isosList, FileIterator.deliminator textDeliminator, out List<string> exportList)
        {
            exportList = new List<string>();

            char spliter;
            switch (textDeliminator)
            {
                case GetPeaks_DLL.DataFIFO.FileIterator.deliminator.Comma:
                    {
                        spliter = ',';
                    }
                    break;
                case GetPeaks_DLL.DataFIFO.FileIterator.deliminator.Tab:
                    {
                        spliter = '\t';
                    }
                    break;
                default:
                    {
                        spliter = ',';
                    }
                    break;
            }

            foreach (IsosObject isos in isosList)
            {
                List<string> isosAsStringList = new List<string>();
                isosAsStringList.Add(isos.scan_num.ToString());
                isosAsStringList.Add(isos.charge.ToString());
                isosAsStringList.Add(isos.abundance.ToString());
                isosAsStringList.Add(isos.mz.ToString());
                isosAsStringList.Add(isos.fit.ToString());
                isosAsStringList.Add(isos.average_mw.ToString());
                isosAsStringList.Add(isos.monoisotopic_mw.ToString());
                isosAsStringList.Add(isos.mostabundant_mw.ToString());
                isosAsStringList.Add(isos.fwhm.ToString());
                isosAsStringList.Add(isos.signal_noise.ToString());
                isosAsStringList.Add(isos.mono_abundance.ToString());
                isosAsStringList.Add(isos.mono_plus2_abundance.ToString());
                isosAsStringList.Add(isos.flag.ToString());
                isosAsStringList.Add(isos.interference_score.ToString());

                string line = "";
                for(int j=0;j<isosAsStringList.Count-1;j++)//-1 for end point
                {
                    line += isosAsStringList[j];
                    line += spliter;

                }
                line += isosAsStringList[isosAsStringList.Count-1];//last point
                exportList.Add(line);
            }
            
        }
    }
}
