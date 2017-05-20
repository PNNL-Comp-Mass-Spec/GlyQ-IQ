using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;

namespace GetPeaks_DLL.DataFIFO
{
    public class DataIsotopesToText
    {
        public void toDiskIsotopeOutput(List<IsotopeObject> isotopePeakList, string outputLocation)
        {
            List<string> isotopesToWrite = new List<string>();

            //part 1, set up column headders
            string isotopeColumnHeader = "";
            List<string> featureColumnNames = new List<string>();
            featureColumnNames.Add("MonoisotopicMass");
            featureColumnNames.Add("ExperimentalMass");
            featureColumnNames.Add("Isotopes");
            for (int i = 0; i < featureColumnNames.Count - 1; i++)
            {
                isotopeColumnHeader += featureColumnNames[i] + "\t";
            }

            //part 2, convert list to stringlist
            List<string> finalList = new List<string>();
            List<IsotopeObject> sortTheseIsotopes = new List<IsotopeObject>();
            //sortTheseIsotopes = elutingPEakList.OrderBy(p => p.MonoIsotopicMass).ToList();//yes sort  bad because we lose the indexes
            sortTheseIsotopes = isotopePeakList;//no sort

            for (int i = 0; i < sortTheseIsotopes.Count; i++)
            {

                string monoIsotopicMass = Convert.ToString(sortTheseIsotopes[i].MonoIsotopicMass);
                string isotopeIntensityStringLine = monoIsotopicMass + "\t";
                string isotopeMassStringLine = monoIsotopicMass + "\t";
                for (int j = 0; j < sortTheseIsotopes[i].IsotopeList.Count - 1; j++)
                {
                    isotopeIntensityStringLine += sortTheseIsotopes[i].IsotopeList[j].Height.ToString() + "\t";
                    isotopeMassStringLine += sortTheseIsotopes[i].IsotopeList[j].XValue.ToString() + "\t";
                }

                //last line
                isotopeIntensityStringLine += sortTheseIsotopes[i].IsotopeList[sortTheseIsotopes[i].IsotopeList.Count - 1].Height.ToString();
                isotopeMassStringLine += sortTheseIsotopes[i].IsotopeList[sortTheseIsotopes[i].IsotopeList.Count - 1].XValue.ToString();

                finalList.Add(isotopeIntensityStringLine);
                isotopePeakList[i].IsotopeIntensityString = isotopeIntensityStringLine;
                isotopePeakList[i].IsotopeMassString = isotopeMassStringLine;
            }


            //part 3, write to disk
            StringListToDisk newWriter = new StringListToDisk();

            string isotopeFile = outputLocation + "_Isotopes.txt";
            //write features
            newWriter.toDiskStringList(isotopeFile, finalList, isotopeColumnHeader);

        }
    }
}
