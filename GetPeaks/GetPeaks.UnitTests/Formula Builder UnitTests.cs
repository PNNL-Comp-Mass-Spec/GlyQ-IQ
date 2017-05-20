using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Diagnostics;
using NUnit.Framework;
using PNNLOmics.Data.Constants;
using PNNLOmics.Data.FormulaBuilder;


namespace GetPeaks.UnitTests
{
	[TestFixture]
	public class FormulaBuilderUnitTests
	{


        AminoAcidFormulaBuilder formBuild = new AminoAcidFormulaBuilder();



		[Test]
		public void TestFormulaCalculator()
		{
            string peptide = "ANKYLSRRH";
            //peptide = "TNSNNYYRRLLIPTKA";//DWDANKYLSRRHCVPKGSV";
            Dictionary<string, int> formula = formBuild.ConvertToMolecularFormula(peptide);
			Assert.AreEqual(49, formula["C"]);
			Assert.AreEqual(79, formula["H"]);
			Assert.AreEqual(19, formula["N"]);
			Assert.AreEqual(12, formula["O"]);

            
            //string setupForCopy;
            double monoMass = formBuild.FormulaToMonoisotopicMass(formula);
            //double monoMass = FormulaToMonoisotopicMass(formula, out setupForCopy);
            Assert.AreEqual(1125.6155591336819, monoMass);
            //setupForCopy = peptide + "," + setupForCopy + "," + monoMass.ToString();
            //double monoMass2 = monoMass / 4;

		}

        //public static double FormulaToMonoisotopicMass(Dictionary<string, int> formula, out string printMe)
        //{

        //    int carbonAtoms = formula["C"];
        //    int hydrogenAtoms = formula["H"];
        //    int nitrogenAtoms = formula["N"];
        //    int oxygenAtoms = formula["O"];
        //    int sulfurAtoms = 0;//formula["S"];

        //    //double massCarbon = Constants.Elements[ElementName.Carbon].MassMonoIsotopic;
        //    //double massHydrogen = Constants.Elements[ElementName.Hydrogen].MassMonoIsotopic;
        //    //double massNitrogen = Constants.Elements[ElementName.Nitrogen].MassMonoIsotopic;
        //    //double massOxygen = Constants.Elements[ElementName.Oxygen].MassMonoIsotopic;
        //    //double massSulfur = Constants.Elements[ElementName.Sulfur].MassMonoIsotopic;

        //    //double monoIsotopicMass =
        //    //    massCarbon * carbonAtoms +
        //    //    massHydrogen * hydrogenAtoms +
        //    //    massNitrogen * nitrogenAtoms +
        //    //    massOxygen * oxygenAtoms +
        //    //    massSulfur * sulfurAtoms;
        //    double massMonoIsotopic = 0.0;

        //    foreach (string f in formula.Keys)
        //    {
        //        massMonoIsotopic += Constants.Elements[f].MassMonoIsotopic * formula[f];
        //    }
            



        //    printMe = carbonAtoms + "," + hydrogenAtoms + "," + nitrogenAtoms + "," + oxygenAtoms + "," + sulfurAtoms + "," + "0";


        //    return massMonoIsotopic;
        //}

	}
}
