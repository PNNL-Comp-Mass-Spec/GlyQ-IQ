using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

namespace GetPeaks_DLL.Isotopes
{
    //TODO this class is strongly typed
    public class TheoryIsotope
    {
        public List<Peak> TheoryIsotopePoissonFX(double mass, Averagine currentAveragine)
        {
            List<Peak> isoDistribution = new List<Peak>();

            float factor = Convert.ToSingle(mass / currentAveragine.isoUnit);
            float numC = factor * Convert.ToSingle(currentAveragine.isoC);
            float numH = factor * Convert.ToSingle(currentAveragine.isoH);
            float numO = factor * Convert.ToSingle(currentAveragine.isoO);
            float numN = factor * Convert.ToSingle(currentAveragine.isoN);
            float numS = factor * Convert.ToSingle(currentAveragine.isoS);


            float a = numC * Convert.ToSingle(currentAveragine.NaturalC13) +
                       numH * Convert.ToSingle(currentAveragine.NaturalH2) +
                       numN * Convert.ToSingle(currentAveragine.NaturalN15) +
                       numO * Convert.ToSingle(currentAveragine.NaturalO17) +
                       numS * Convert.ToSingle(currentAveragine.NaturalS33);
            float b = numO * Convert.ToSingle(currentAveragine.NaturalO18) +
                       numS * Convert.ToSingle(currentAveragine.NaturalS34);

            float intensity = 1;
            float massTemp;
            float intensityTemp;

            massTemp = Convert.ToSingle(0 * currentAveragine.MassNeutron / currentAveragine.MassProton);
            intensityTemp = intensity;
            Peak newPeak0 = new Peak();
            newPeak0.Height = intensityTemp;
            newPeak0.XValue = massTemp;
            isoDistribution.Add(newPeak0);

            massTemp = Convert.ToSingle(1 * currentAveragine.MassNeutron / currentAveragine.MassProton);
            intensityTemp = (a) * intensity;
            Peak newPeak1 = new Peak();
            newPeak1.Height = intensityTemp;
            newPeak1.XValue = massTemp;
            isoDistribution.Add(newPeak1);

            massTemp = Convert.ToSingle(2 * currentAveragine.MassNeutron / currentAveragine.MassProton);
            intensityTemp = Convert.ToSingle((Math.Pow(a, 2) / 2 + b) * intensity);
            Peak newPeak2 = new Peak();
            newPeak2.Height = intensityTemp;
            newPeak2.XValue = massTemp;
            isoDistribution.Add(newPeak2);

            massTemp = Convert.ToSingle(3 * currentAveragine.MassNeutron / currentAveragine.MassProton);
            intensityTemp = Convert.ToSingle((Math.Pow(a, 3) / 6 + a * b) * intensity);
            Peak newPeak3 = new Peak();
            newPeak3.Height = intensityTemp;
            newPeak3.XValue = massTemp;
            isoDistribution.Add(newPeak3);

            massTemp = Convert.ToSingle(4 * currentAveragine.MassNeutron / currentAveragine.MassProton);
            intensityTemp = (float)(Math.Pow(a, 4) / 24 + Math.Pow(a, 2) / 2 * b + Math.Pow(b, 2)) * intensity;
            Peak newPeak4 = new Peak();
            newPeak4.Height = intensityTemp;
            newPeak4.XValue = massTemp;
            isoDistribution.Add(newPeak4);

            massTemp = Convert.ToSingle(5 * currentAveragine.MassNeutron / currentAveragine.MassProton);
            intensityTemp = (float)(Math.Pow(a, 5) / 120 + Math.Pow(a, 3) / 6 * b + a * Math.Pow(b, 2) / 2) * intensity;
            Peak newPeak5 = new Peak();
            newPeak5.Height = intensityTemp;
            newPeak5.XValue = massTemp;
            isoDistribution.Add(newPeak5);

            massTemp = Convert.ToSingle(6 * currentAveragine.MassNeutron / currentAveragine.MassProton);
            intensityTemp = (float)(Math.Pow(a, 6) / 720 + Math.Pow(a, 4) / 24 * b + Math.Pow(a, 2) * Math.Pow(b, 2) / 4 + Math.Pow(b, 3) / 6) * intensity;
            Peak newPeak6 = new Peak();
            newPeak6.Height = intensityTemp;
            newPeak6.XValue = massTemp;
            isoDistribution.Add(newPeak6);

            massTemp = Convert.ToSingle(7 * currentAveragine.MassNeutron / currentAveragine.MassProton);
            intensityTemp = (float)(Math.Pow(a, 7) / 5040 + Math.Pow(a, 5) / 120 * b + Math.Pow(a, 3) * Math.Pow(b, 2) / 12 + a * Math.Pow(b, 3) / 6) * intensity;
            Peak newPeak7 = new Peak();
            newPeak7.Height = intensityTemp;
            newPeak7.XValue = massTemp;
            isoDistribution.Add(newPeak7);

            return isoDistribution;
        }
    }
}
