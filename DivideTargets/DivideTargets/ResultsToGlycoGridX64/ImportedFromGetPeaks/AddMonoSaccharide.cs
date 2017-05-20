using System.Collections.Generic;
using PNNLOmics.Data.Constants;
using ResultsToGlycoGridX64.ImportedFromGetPeaks.Objects;

namespace ResultsToGlycoGridX64.ImportedFromGetPeaks
{
    public static class AddMonoSaccharide
    {
        public static void Add(List<FragmentIQTarget64> myFragments, MonosaccharideName monosaccharide, ScanObject currentMinMax)
        {
            string name = Constants.Monosaccharides[monosaccharide].Name;
            double massMonoisotopic = Constants.Monosaccharides[monosaccharide].MassMonoIsotopic;
            string empericalFormula = Constants.Monosaccharides[monosaccharide].ChemicalFormula;//"C6H10O5";//162.05
            int min = currentMinMax.Min;
            int max = currentMinMax.Max;

            Add(myFragments, name, massMonoisotopic, empericalFormula, min, max);
        }

        public static void Add(List<FragmentIQTarget64> myFragments, MonosaccharideName monosaccharide, int min, int max)
        {
            string name = Constants.Monosaccharides[monosaccharide].Name;
            double massMonoisotopic = Constants.Monosaccharides[monosaccharide].MassMonoIsotopic;
            string empericalFormula = Constants.Monosaccharides[monosaccharide].ChemicalFormula;//"C6H10O5";//162.05


            Add(myFragments, name, massMonoisotopic, empericalFormula, min, min);
        }

        public static void Add(List<FragmentIQTarget64> myFragments, string name, double massMonoisotopic, string empericalFormula, int min, int max)
        {
            FragmentIQTarget64 target = new FragmentIQTarget64();
            target.DifferenceName = name;
            target.MonoMassTheor = massMonoisotopic;
            target.EmpiricalFormula = empericalFormula;
            target.ScanInfo.Min = min;
            target.ScanInfo.Max = max;
            myFragments.Add(target);
        }
    }
}
