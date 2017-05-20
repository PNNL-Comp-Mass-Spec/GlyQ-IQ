using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQGlyQ.Objects;
using PNNLOmics.Data.Constants;

namespace IQGlyQ.Functions
{
    public static class AddMonoSaccharide
    {
        public static void Add(List<FragmentIQTarget> myFragments, MonosaccharideName monosaccharide, ScanObject currentMinMax)
        {
            string name = Constants.Monosaccharides[monosaccharide].Name;
            double massMonoisotopic = Constants.Monosaccharides[monosaccharide].MassMonoIsotopic;
            string empericalFormula = Constants.Monosaccharides[monosaccharide].ChemicalFormula;//"C6H10O5";//162.05
            int min = currentMinMax.Min;
            int max = currentMinMax.Max;

            Add(myFragments, name, massMonoisotopic, empericalFormula, min, max);
        }

        public static void Add(List<FragmentIQTarget> myFragments, MonosaccharideName monosaccharide, int min, int max)
        {
            string name = Constants.Monosaccharides[monosaccharide].Name;
            double massMonoisotopic = Constants.Monosaccharides[monosaccharide].MassMonoIsotopic;
            string empericalFormula = Constants.Monosaccharides[monosaccharide].ChemicalFormula;//"C6H10O5";//162.05


            Add(myFragments, name, massMonoisotopic, empericalFormula, min, min);
        }

        public static void Add(List<FragmentIQTarget> myFragments, string name, double massMonoisotopic, string empericalFormula, int min, int max)
        {
            FragmentIQTarget target = new FragmentIQTarget();
            target.DifferenceName = name;
            target.MonoMassTheor = massMonoisotopic;
            target.EmpiricalFormula = empericalFormula;
            target.ScanInfo.Min = min;
            target.ScanInfo.Max = max;
            myFragments.Add(target);
        }
    }
}
