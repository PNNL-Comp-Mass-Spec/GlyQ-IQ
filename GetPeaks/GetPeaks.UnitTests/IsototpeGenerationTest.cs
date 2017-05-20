using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GetPeaks_DLL.Isotopes;
using PNNLOmics.Data;
using GetPeaks_DLL.DataFIFO;
using PNNLOmics.Data.FormulaBuilder;

namespace GetPeaks.UnitTests
{
    public class IsototpeGenerationTest
    {
        [Test]

        public void generateDistributions()
        {
            //load data
            FileIteratorStringOnly loadPeptides = new FileIteratorStringOnly();
            List<string> peptides = new List<string>();
            List<string> glycans = new List<string>();

            string peptideFilename = @"D:\PNNL\Projects\Isotope Fitting\315 Human blood Neutrophill.txt";
            //load strings
            StringLoadTextFileLine loadSpectraL = new StringLoadTextFileLine();
            peptides = loadSpectraL.SingleFileByLine(peptideFilename);//loads strings

            string glycanFileName = @"D:\PNNL\Projects\Isotope Fitting\331 Human Glycans.txt";
            glycans = loadSpectraL.SingleFileByLine(glycanFileName);//loads strings

            Averagine currentAveragine = new Averagine();

            //FormulaConverters convertToMass = new FormulaConverters();
            TheoryIsotope isotopeGenerator = new TheoryIsotope();

            List<Peak> returnedGlycanPeaks = new List<Peak>();
            List<string> returnedGlycanStringsExact = new List<string>();
            List<string> returnedGlycanStringsAverageose = new List<string>();
            List<string> returnedGlycanStringsAveragine = new List<string>();
            foreach (string glycan in glycans)
            {

                OligosaccharideFormulaBuilder osFormBuild = new OligosaccharideFormulaBuilder();
                Dictionary<string, int> formulaOS = new Dictionary<string, int>();
                formulaOS= osFormBuild.ConvertToMolecularFormula(glycan);

                //for a given mass we need the averageose and the exact
                //double monoMassOS = convertToMass.FormulaToMonoisotopicMass(formulaOS);
                double monoMassOS = osFormBuild.FormulaToMonoisotopicMass(formulaOS);
                //step 1, the exact
                SetCustomAveragine(currentAveragine, formulaOS);//unique pre required step for custom
                currentAveragine = currentAveragine.AveragineSetup(AveragineType.Custom);

                returnedGlycanPeaks = isotopeGenerator.TheoryIsotopePoissonFX(monoMassOS, currentAveragine);

                string start = "";
                for (int i = 0; i < returnedGlycanPeaks.Count - 1;i++)
                {
                    start += returnedGlycanPeaks[i].Height.ToString() + ",";
                }
                start += returnedGlycanPeaks[returnedGlycanPeaks.Count-1].Height.ToString();
                returnedGlycanStringsExact.Add(start);

                //step2, the averageose
                currentAveragine = currentAveragine.AveragineSetup(AveragineType.Glycan);

                returnedGlycanPeaks = isotopeGenerator.TheoryIsotopePoissonFX(monoMassOS, currentAveragine);

                string start2 = "";
                for (int i = 0; i < returnedGlycanPeaks.Count - 1; i++)
                {
                    start2 += returnedGlycanPeaks[i].Height.ToString() + ",";
                }
                start2 += returnedGlycanPeaks[returnedGlycanPeaks.Count - 1].Height.ToString();
                returnedGlycanStringsAverageose.Add(start2);

                //step 3, glycan mass against peptide averagine

                currentAveragine = currentAveragine.AveragineSetup(AveragineType.Peptide);

                returnedGlycanPeaks = isotopeGenerator.TheoryIsotopePoissonFX(monoMassOS, currentAveragine);

                string start3 = "";
                for (int i = 0; i < returnedGlycanPeaks.Count - 1; i++)
                {
                    start3 += returnedGlycanPeaks[i].Height.ToString() + ",";
                }
                start3 += returnedGlycanPeaks[returnedGlycanPeaks.Count - 1].Height.ToString();
                returnedGlycanStringsAveragine.Add(start3);
            }

            
            List<Peak> returnedPeptidePeaks = new List<Peak>();
            List<string> returnedPeptideStringsExact = new List<string>();
            List<string> returnedPeptideStringsAveragine = new List<string>();
            List<string> returnedPeptideStringsAverageose = new List<string>();
            foreach (string peptide in peptides)
            {

                AminoAcidFormulaBuilder formBuild = new AminoAcidFormulaBuilder();
                Dictionary<string, int> formula = formBuild.ConvertToMolecularFormula(peptide);

                double monoMass = formBuild.FormulaToMonoisotopicMass(formula);
                //step 1, the exact
                SetCustomAveragine(currentAveragine, formula);//unique pre required step for custom
                currentAveragine = currentAveragine.AveragineSetup(AveragineType.Custom);

                returnedPeptidePeaks = isotopeGenerator.TheoryIsotopePoissonFX(monoMass, currentAveragine);

                string start = "";
                for (int i = 0; i < returnedPeptidePeaks.Count - 1; i++)
                {
                    start += returnedPeptidePeaks[i].Height.ToString() + ",";
                }
                start += returnedPeptidePeaks[returnedPeptidePeaks.Count - 1].Height.ToString();
                returnedPeptideStringsExact.Add(start);

                //step2, the averagine
                currentAveragine = currentAveragine.AveragineSetup(AveragineType.Peptide);

                returnedPeptidePeaks = isotopeGenerator.TheoryIsotopePoissonFX(monoMass, currentAveragine);
                Assert.AreEqual(8, returnedPeptidePeaks.Count);

                string start2 = "";
                for (int i = 0; i < returnedPeptidePeaks.Count - 1; i++)
                {
                    start2 += returnedPeptidePeaks[i].Height.ToString() + ",";
                }
                start2 += returnedPeptidePeaks[returnedPeptidePeaks.Count - 1].Height.ToString();
                returnedPeptideStringsAveragine.Add(start2);

                //step 3, peptide mass against peptide averagose

                currentAveragine = currentAveragine.AveragineSetup(AveragineType.Glycan);

                returnedPeptidePeaks = isotopeGenerator.TheoryIsotopePoissonFX(monoMass, currentAveragine);
                Assert.AreEqual(8, returnedPeptidePeaks.Count);

                string start3 = "";
                for (int i = 0; i < returnedPeptidePeaks.Count - 1; i++)
                {
                    start3 += returnedPeptidePeaks[i].Height.ToString() + ",";
                }
                start3 += returnedPeptidePeaks[returnedPeptidePeaks.Count - 1].Height.ToString();
                returnedPeptideStringsAverageose.Add(start3);
            }

            StringListToDisk newWriter = new StringListToDisk();
            string outputLocation = @"D:\Csharp\IsotopeOutput\";
            string outputfile = "";
            string columnHeader = "";

            outputfile = outputLocation + "_Glycan-Exact_Isotopes.txt";
            columnHeader = "Exact Glycan Distributions";
            newWriter.toDiskStringList(outputfile, returnedGlycanStringsExact, columnHeader);

            outputfile = outputLocation + "_Glycan-Averageose_Isotopes.txt";
            columnHeader = "Averageose Distributions";
            newWriter.toDiskStringList(outputfile, returnedGlycanStringsAverageose, columnHeader);

            outputfile = outputLocation + "_Glycan-Averagine_Isotopes.txt";
            columnHeader = "Glycan+Averagine Distributions";
            newWriter.toDiskStringList(outputfile, returnedGlycanStringsAveragine, columnHeader);

            outputfile = outputLocation + "_Peptide-Exact_Isotopes.txt";
            columnHeader = "Exact Peptide Distributions";
            newWriter.toDiskStringList(outputfile, returnedPeptideStringsExact, columnHeader);

            outputfile = outputLocation + "_Peptide-Averagine_Isotopes.txt";
            columnHeader = "Averagine Distributions";
            newWriter.toDiskStringList(outputfile, returnedPeptideStringsAveragine, columnHeader);

            outputfile = outputLocation + "_Peptide-Averagose_Isotopes.txt";
            columnHeader = "Peptide+Averagose Distributions";
            newWriter.toDiskStringList(outputfile, returnedPeptideStringsAverageose, columnHeader);

        }

        private static void SetCustomAveragine(Averagine currentAveragine, Dictionary<string, int> formulaOS)
        {
            currentAveragine.isoC = 0;
            currentAveragine.isoH = 0;
            currentAveragine.isoN = 0;
            currentAveragine.isoO = 0;
            currentAveragine.isoS = 0;
            if (formulaOS.ContainsKey("C"))
            {
                currentAveragine.isoC = formulaOS["C"];
            }
            if (formulaOS.ContainsKey("H"))
            {
                currentAveragine.isoH = formulaOS["H"];
            }
            if (formulaOS.ContainsKey("N"))
            {
                currentAveragine.isoN = formulaOS["N"];
            }
            if (formulaOS.ContainsKey("O"))
            {
                currentAveragine.isoO = formulaOS["O"];
            }
            if (formulaOS.ContainsKey("S"))
            {
                currentAveragine.isoS = formulaOS["S"];
            }

        }
    }
}
