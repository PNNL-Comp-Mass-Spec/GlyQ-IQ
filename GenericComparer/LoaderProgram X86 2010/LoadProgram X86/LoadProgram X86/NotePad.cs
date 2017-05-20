using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Constants.ConstantsDataLayer;
using PNNLOmics.Data.Constants.ConstantsDataUtilities;


namespace ConsoleApplication1
{
    public class NotePad
    {
        public void NotePadFX()
        {
            //amino acid          

            char aminoAcidKey = 'N';
            //dictionary implementation
            Dictionary<char, AminoAcid> aminoAcidsDictionary = AminoAcidLibrary.LoadAminoAcidData();
            double mass = aminoAcidsDictionary[aminoAcidKey].MonoIsotopicMass;
            string name = aminoAcidsDictionary[aminoAcidKey].Name;
            string formula = aminoAcidsDictionary[aminoAcidKey].ChemicalFormula;

           
            //one line implementation
            double mass2 = AminoAcidStaticLibrary.GetMonoisotopicMass(aminoAcidKey);
            string name2 = AminoAcidStaticLibrary.GetName(aminoAcidKey);
            string formula2 = AminoAcidStaticLibrary.GetFormula(aminoAcidKey);

         
            double massPeptide = 0;
            string peptideSequence = "NRTL";
            for (int y = 0; y < peptideSequence.Length; y++)
            {
                massPeptide += AminoAcidStaticLibrary.GetMonoisotopicMass(peptideSequence[y]);
            }//massPeptide = 484.27578094385393

            
                        
        }
    }
}
