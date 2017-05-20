using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
//using PNNLOmics.Data.Constants.ConstantsDataLayer;
using PNNLOmics.Data.Constants.ConstantsDataUtilities;
using LoadProgram;
using PNNLOmics.Data.Constants.ConstantsDataLayer;

namespace ConsoleApplication1
{
    class ConstantsTest
    {

        [Test]
        public void TestAminoAcid()//test amino acid calls
        {
            char aminoAcidKey = 'N';
            //dictionary implementation
            Dictionary<char, AminoAcid> aminoAcidsDictionary = AminoAcidLibrary.LoadAminoAcidData();
            double mass = aminoAcidsDictionary[aminoAcidKey].MonoIsotopicMass;
            string name = aminoAcidsDictionary[aminoAcidKey].Name;
            string formula = aminoAcidsDictionary[aminoAcidKey].ChemicalFormula;

            Assert.AreEqual(mass, 114.04292745124599);
            Assert.AreEqual(name, "Asparagine");
            Assert.AreEqual(formula, "C4H6N2O2");
            
            //one line implementation
            double mass2 = AminoAcidStaticLibrary.GetMonoisotopicMass(aminoAcidKey);
            string name2 = AminoAcidStaticLibrary.GetName(aminoAcidKey);
            string formula2 = AminoAcidStaticLibrary.GetFormula(aminoAcidKey);

            Assert.AreEqual(mass2, 114.04292745124599);
            Assert.AreEqual(name2, "Asparagine");
            Assert.AreEqual(formula2, "C4H6N2O2");
                
            double massPeptide = 0;
            string peptideSequence = "NRTL";
            for (int y = 0; y < peptideSequence.Length; y++)
            {
                massPeptide += AminoAcidStaticLibrary.GetMonoisotopicMass(peptideSequence[y]);
            }//massPeptide = 484.27578094385393

            Assert.AreEqual(massPeptide, 484.27578094385393);         
        }

        [Test]
        public void TestMonosaccharide() //test monosacharide calls
        {
            string monosaccharideKey = "Hex";
            //dictionary implementation
            Dictionary<string, Monosaccharide> monosacchaideDictionary = MonosaccharideLibrary.LoadMonosaccharideData();
            double mass = monosacchaideDictionary[monosaccharideKey].MonoIsotopicMass;
            string name = monosacchaideDictionary[monosaccharideKey].Name;
            string formula = monosacchaideDictionary[monosaccharideKey].ChemicalFormula;

            Assert.AreEqual(mass, 162.05282343122502);
            Assert.AreEqual(name, "Hexose");
            Assert.AreEqual(formula, "C6H10O5");

            //one line implementaiton
            double mass2 = MonosaccharideStaticLibrary.GetMonoisotopicMass(monosaccharideKey);
            string name2 = MonosaccharideStaticLibrary.GetName(monosaccharideKey);
            string formula2 = MonosaccharideStaticLibrary.GetFormula(monosaccharideKey);

            Assert.AreEqual(mass2, 162.05282343122502);
            Assert.AreEqual(name2, "Hexose");
            Assert.AreEqual(formula2, "C6H10O5");
        }

        [Test]
        public void TestElements()
        {
            string elementKey = "C";
            //dictionary implementation

            Dictionary<string, Element> elementDictionary = ElementLibrary.LoadElementData();
            double elementC12Mass = elementDictionary[elementKey].IsotopeDictionary["C12"].Mass;
            double elementC13Mass = elementDictionary[elementKey].IsotopeDictionary["C13"].Mass;
            double elementC12Abund = elementDictionary[elementKey].IsotopeDictionary["C12"].NaturalAbundance;
            double elementC13Abund = elementDictionary[elementKey].IsotopeDictionary["C13"].NaturalAbundance;

            Assert.AreEqual(elementC12Mass, 12.0);
            Assert.AreEqual(elementC13Mass, 13.0033548385);
            Assert.AreEqual(elementC12Abund, 0.98894428);
            Assert.AreEqual(elementC13Abund, 0.01105628);

            double elementMass = elementDictionary[elementKey].MonoIsotopicMass;
            string elementName = elementDictionary[elementKey].Name;
            string elementSymbol = elementDictionary[elementKey].Symbol;

            Assert.AreEqual(elementMass, 12.0);
            Assert.AreEqual(elementName, "Carbon");
            Assert.AreEqual(elementSymbol, "C");

            //One line implementation

            double elementMass2 = ElementStaticLibrary.GetMonoisotopicMass(elementKey);
            string elementName2 = ElementStaticLibrary.GetName(elementKey);
            string elementSymbol2 = ElementStaticLibrary.GetSymbol(elementKey);

            Assert.AreEqual(elementMass2, 12.0);
            Assert.AreEqual(elementName2, "Carbon");
            Assert.AreEqual(elementSymbol2, "C");
        }

        [Test]
        public void TestAtom()
        {
            string atomKey = "e";
            //dictinoary implementation

            Dictionary<string, Atom> atomDictionary = AtomLibrary.LoadAtomicData();
            double atomMass = atomDictionary[atomKey].MonoIsotopicMass;
            string atomName = atomDictionary[atomKey].Name;
            string atomSymbol = atomDictionary[atomKey].Symbol;

            Assert.AreEqual(atomMass, 0.00054857990943);
            Assert.AreEqual(atomName, "Electron");
            Assert.AreEqual(atomSymbol, "e");

            //one line implementation
            double atomMass2 = AtomStaticLibrary.GetMonoisotopicMass(atomKey);
            string atomName2 = AtomStaticLibrary.GetName(atomKey);
            string atomSymbol2 = AtomStaticLibrary.GetSymbol(atomKey);

            Assert.AreEqual(atomMass2, 0.00054857990943);
            Assert.AreEqual(atomName2, "Electron");
            Assert.AreEqual(atomSymbol2, "e");
        }

        [Test]
        public void TestOtherMolecule()
        {
            string otherKey = "Aldehyde";
            //dictionarty implementation
            Dictionary<string, OtherMolecule> otherMoleculeDictionary = OtherMoleculeLibrary.LoadOtherMoleculeData();
            double otherMass = otherMoleculeDictionary[otherKey].MonoIsotopicMass;
            string otherName = otherMoleculeDictionary[otherKey].Name;
            string otherFormula = otherMoleculeDictionary[otherKey].ChemicalFormula;

            Assert.AreEqual(otherMass, 18.010564686245);
            Assert.AreEqual(otherName, "Aldehyde");
            Assert.AreEqual(otherFormula, "H2O");

            //One line implememtation
            double otherMass2 = OtherMoleculeStaticLibrary.GetMonoisotopicMass(otherKey);
            string otherName2 = OtherMoleculeStaticLibrary.GetName(otherKey);
            string otherFormula2 = OtherMoleculeStaticLibrary.GetFormula(otherKey);

            Assert.AreEqual(otherMass2, 18.010564686245);
            Assert.AreEqual(otherName2, "Aldehyde");
            Assert.AreEqual(otherFormula2, "H2O");

            double mass3 = OtherMoleculeMass.Aldehyde();
            double mass4 = OtherMoleculeMass.Water();

            Assert.AreEqual(mass3, 18.010564686245);
            Assert.AreEqual(mass4, 18.010564686245);
        }

        [Test]
        public void TestCrossRing()
        {
            string crossRingKey = "CRFNeu5Ac_03_X1";
            //dictionarty implementation
            Dictionary<string, CrossRing> CrossRingDictionary = CrossRingLibrary.LoadCrossRingData();
            double mass = CrossRingDictionary[crossRingKey].MonoIsotopicMass;
            string formula = CrossRingDictionary[crossRingKey].ChemicalFormula;
            string name = CrossRingDictionary[crossRingKey].Name;

            Assert.AreEqual(mass, 100.01604399481499);
            Assert.AreEqual(formula, "C4H4O3");
            Assert.AreEqual(name, "CRFNeu5Ac_03_X1");

            //one line implementation
            double mass2 = CrossRingStaticLibrary.GetMonoisotopicMass(crossRingKey);
            string formula2 = CrossRingStaticLibrary.GetFormula(crossRingKey);
            string name2 = CrossRingStaticLibrary.GetName(crossRingKey);

            Assert.AreEqual(mass, 100.01604399481499);
            Assert.AreEqual(formula, "C4H4O3");
            Assert.AreEqual(name, "CRFNeu5Ac_03_X1");

            double mass3 = CrossRingMass.CRFHex_02_A2();
            double mass4 = CrossRingMass.CRFNeu5Ac_24_X1();

            Assert.AreEqual(mass3, 102.031694058735);
            Assert.AreEqual(mass4, 190.04773805355);
        }
    }
}
