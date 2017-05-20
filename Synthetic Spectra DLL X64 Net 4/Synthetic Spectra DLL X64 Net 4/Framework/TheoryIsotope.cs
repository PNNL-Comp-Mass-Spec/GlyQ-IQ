using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Synthetic_Spectra_DLL_X64_Net_4.Framework
{
    //TODO this class is strongly typed
    public class TheoryIsotope
    {
        public List<PeakGeneric<decimal, double>> TheoryIsotopePoissonFX(decimal mass, Averagine currentAveragine)
        {
            List<PeakGeneric<decimal, double>> isoDistribution = new List<PeakGeneric<decimal, double>>();

            double factor = (double)(mass / currentAveragine.isoUnit);
            double numC = factor * currentAveragine.isoC;
            double numH = factor * currentAveragine.isoH;
            double numO = factor * currentAveragine.isoO;
            double numN = factor * currentAveragine.isoN;
            double numS = factor * currentAveragine.isoS;


            double a = numC * currentAveragine.NaturalC13 +
                       numH * currentAveragine.NaturalH2 +
                       numN * currentAveragine.NaturalN15 +
                       numO * currentAveragine.NaturalO17 +
                       numS * currentAveragine.NaturalS33;
            double b = numO * currentAveragine.NaturalO18 +
                       numS * currentAveragine.NaturalS34;

            double intensity = 1;
            decimal massTemp;
            double intensityTemp;

            massTemp = 0 * currentAveragine.MassNeutron / currentAveragine.MassProton;
            intensityTemp = intensity;
            isoDistribution.Add(new PeakGeneric<decimal, double>(massTemp, intensityTemp));

            massTemp = 1 * currentAveragine.MassNeutron / currentAveragine.MassProton;
            intensityTemp = (a) * intensity;
            isoDistribution.Add(new PeakGeneric<decimal, double>(massTemp, intensityTemp));

            massTemp = 2 * currentAveragine.MassNeutron / currentAveragine.MassProton;
            intensityTemp = (Math.Pow(a, 2) / 2 + b) * intensity;
            isoDistribution.Add(new PeakGeneric<decimal, double>(massTemp, intensityTemp));

            massTemp = 3 * currentAveragine.MassNeutron / currentAveragine.MassProton;
            intensityTemp = (Math.Pow(a, 3) / 6 + a * b) * intensity;
            isoDistribution.Add(new PeakGeneric<decimal, double>(massTemp, intensityTemp));

            massTemp = 4 * currentAveragine.MassNeutron / currentAveragine.MassProton;
            intensityTemp = (float)(Math.Pow(a, 4) / 24 + Math.Pow(a, 2) / 2 * b + Math.Pow(b, 2)) * intensity;
            isoDistribution.Add(new PeakGeneric<decimal, double>(massTemp, intensityTemp));

            massTemp = 5 * currentAveragine.MassNeutron / currentAveragine.MassProton;
            intensityTemp = (float)(Math.Pow(a, 5) / 120 + Math.Pow(a, 3) / 6 * b + a * Math.Pow(b, 2) / 2) * intensity;
            isoDistribution.Add(new PeakGeneric<decimal, double>(massTemp, intensityTemp));

            massTemp = 6 * currentAveragine.MassNeutron / currentAveragine.MassProton;
            intensityTemp = (float)(Math.Pow(a, 6) / 720 + Math.Pow(a, 4) / 24 * b + Math.Pow(a, 2) * Math.Pow(b, 2) / 4 + Math.Pow(b, 3) / 6) * intensity;
            isoDistribution.Add(new PeakGeneric<decimal, double>(massTemp, intensityTemp));

            massTemp = 7 * currentAveragine.MassNeutron / currentAveragine.MassProton;
            intensityTemp = (float)(Math.Pow(a, 7) / 5040 + Math.Pow(a, 5) / 120 * b + Math.Pow(a, 3) * Math.Pow(b, 2) / 12 + a * Math.Pow(b, 3) / 6) * intensity;
            isoDistribution.Add(new PeakGeneric<decimal, double>(massTemp, intensityTemp));

            return isoDistribution;
        }
    }
}
