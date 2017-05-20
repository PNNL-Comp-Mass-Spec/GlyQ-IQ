using System;
using System.Collections.Generic;
using IQGlyQ.Objects.EverythingIsotope;
using IQ_X64.Backend.ProcessingTasks.TheorFeatureGenerator;
using NUnit.Framework;
using PNNLOmics.Data.Constants;
using Run64.Backend;

namespace IQGlyQ.UnitTesting
{
    
    class FitScoreIsotopeProfileCount
    {
        [Test]
        public void countIsotopeForMassAveragine()
        {
            FragmentedTargetedWorkflowParametersIQ tempParameters = new FragmentedTargetedWorkflowParametersIQ();

            tempParameters.IsotopeLowPeakCuttoff = 0.1;//take top 90% intensity wise
            double IsotopehighPeakCuttoff = 0.5;
            ITheorFeatureGenerator TheorFeatureGen = new JoshTheorFeatureGenerator(Globals.LabellingType.NONE, 1, tempParameters.IsotopeLowPeakCuttoff, tempParameters.MolarMixingFractionOfH);

            int charge = 1;

            Dictionary<double, int> massVsIsoCount = new Dictionary<double, int>();

            double maxMasForRange = 20000;
            double step = 60;
            for (int i = 1; i < step; i++)
            {
                double mass = i * maxMasForRange/step;

                string empericalFormula = MassToAveragineFormula(mass);

                var TheorIsotopicProfile = TheoreticalIsotopicProfileWrapper.GenerateSimpleOld(ref TheorFeatureGen, empericalFormula, charge);

                int isotopeCount = 0;
                int isotopeCountHigh = 0;
                foreach (var isotope in TheorIsotopicProfile.Peaklist)
                {
                    if (isotope.Height > tempParameters.IsotopeLowPeakCuttoff)
                    {
                        isotopeCount++;
                        if (isotope.Height > IsotopehighPeakCuttoff)
                        {
                            isotopeCountHigh++;
                        }
                    }
                }

                Console.WriteLine(Math.Truncate(mass) + "," + isotopeCount + "," + isotopeCountHigh);
                massVsIsoCount.Add(Math.Truncate(mass), isotopeCount);

                //under development
                //this is a penalty for missing key ions that are 50% of the abundance and should be detected
                //by adding multiplying the penalty (greater than1 ) we increase the fit score for each abundant ion missed
                double penaltyFormissingIonsFromTop50 = 0;
                double penalty = 0;
                double ionsMissing = 0;
                double ExperimentalIionsDetected = 1;
                if(ExperimentalIionsDetected<isotopeCountHigh)
                {
                    
                    if (isotopeCount - isotopeCountHigh == 0)
                    {
                        penalty = 1;
                    }
                    else
                    {
                        ionsMissing = isotopeCountHigh - ExperimentalIionsDetected;
                         
                        penalty = 1+ ionsMissing * 1 / isotopeCountHigh;
                    }
                    
                    Console.WriteLine("We are missing " + ionsMissing + " key ions and are penalizing by a factor of " + penalty);

                }

                
            }

            
        }

        private static string MassToAveragineFormula(double mass)
        {
            double averageineC = 4.9384;
            double averagineH = 7.7583;
            double averagineN = 1.3577;
            double averagineO = 1.4773;
            double averagineS = 0.0417;
            double massCarbon = Constants.Elements[PNNLOmics.Data.Constants.ElementName.Carbon].IsotopeDictionary["C12"].Mass;
            double massHydrogen = Constants.Elements[PNNLOmics.Data.Constants.ElementName.Hydrogen].IsotopeDictionary["H1"].Mass;
            double massNitrogen =
                Constants.Elements[PNNLOmics.Data.Constants.ElementName.Nitrogen].IsotopeDictionary["N14"].Mass;
            double massOxygen = Constants.Elements[PNNLOmics.Data.Constants.ElementName.Oxygen].IsotopeDictionary["O16"].Mass;
            double massSulfur = Constants.Elements[PNNLOmics.Data.Constants.ElementName.Sulfur].IsotopeDictionary["S32"].Mass;


            bool ifGlycanAveragose = false;
            if (ifGlycanAveragose)
            {
                averageineC = 31;
                averagineH = 50;
                averagineN = 2;
                averagineO = 22;
                averagineS = 0;
            }

            double massAveragine = massCarbon*averageineC + massHydrogen*averagineH + averagineO*massOxygen + massNitrogen*averagineN + massSulfur*averagineS;
            double unitsOfAveragineInMass = mass/massAveragine;

            int carbons = Convert.ToInt32(Math.Round(unitsOfAveragineInMass*averageineC, 0));
            int hydrogens = Convert.ToInt32(Math.Round(unitsOfAveragineInMass*averagineH, 0));
            int nitrogens = Convert.ToInt32(Math.Round(unitsOfAveragineInMass*averagineN, 0));
            int oxygens = Convert.ToInt32(Math.Round(unitsOfAveragineInMass*averagineO, 0));
            int sulfurs = Convert.ToInt32(Math.Round(unitsOfAveragineInMass*averagineS, 0));
            string empericalFormula = "C" + carbons + "H" + hydrogens + "N" + nitrogens + "O" + oxygens + "S" + sulfurs;
            return empericalFormula;
        }
    }
}
