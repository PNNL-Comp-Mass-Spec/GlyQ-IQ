using System;
using System.Collections.Generic;
using DivideTargetsLibraryX64.FromGetPeaks;
using PNNLOmics.Data.Constants.Libraries;
using ResultsToGlycoGridX64.ImportedFromGetPeaks.Objects;
using PNNLOmics.Data.Constants;

namespace ResultsToGlycoGridX64.ImportedFromGetPeaks
{
    public static class ConvertFactorToIQTargets
    {
        public static List<FragmentIQTarget64> FactorsToFragments(string factorFile, bool isUnitTest)
        {
            //read factor file
            StringLoadTextFileLine loadString = new StringLoadTextFileLine();
            List<ScanObject> factorRanges = new List<ScanObject>();
            List<string> factors = loadString.SingleFileByLine(factorFile);
            for (int i = 0; i < factors.Count; i++)
            {
                string[] words = factors[i].Split(',');
                factors[i] = words[0];
                int min = Convert.ToInt32(words[1]);
                int max = Convert.ToInt32(words[2]);
                ScanObject ranges = new ScanObject(0, 0, min, max);

                factorRanges.Add(ranges);
            }
            Console.WriteLine(factors[0]);


            List<FragmentIQTarget64> myFragments = new List<FragmentIQTarget64>();

            if (isUnitTest)
            {
                ScanObject ranges = new ScanObject(0, 0, 3, 12);
                AddMonoSaccharide.Add(myFragments, MonosaccharideName.Hexose, ranges);
            }
            else
            {
                bool addCommonMonosaccharides = true;

                bool addSulfate = false;
                bool addWater = false;
                bool addBonus = false;
                bool addMuddiman = false;

                if (factors.Contains("Sulfate")) addSulfate = true;
                if (factors.Contains("Water")) addWater = true;
                if (factors.Contains("DeuteratedHexNAc")) addBonus = true;
                if (factors.Contains("HexNAc-ol")) addBonus = true;
                if (factors.Contains("Muddiman")) addMuddiman = true;

                Console.WriteLine("Just Before PNNL Omics");

                //this will change the order
                if (addCommonMonosaccharides)
                {
                    //loop through the list so the order is preseverd
                    foreach (string currentFactor in factors)
                    {
                        if (currentFactor == "Hexose")
                        {
                            ScanObject currentMinMax = factorRanges[factors.IndexOf("Hexose")];
                            AddMonoSaccharide.Add(myFragments, MonosaccharideName.Hexose, currentMinMax);
                        }
                        if (currentFactor == "Deoxyhexose")
                        {
                            ScanObject currentMinMax = factorRanges[factors.IndexOf("Deoxyhexose")];
                            AddMonoSaccharide.Add(myFragments, MonosaccharideName.Deoxyhexose, currentMinMax);
                        }
                        if (currentFactor == "NAcetylhexosamine")
                        {
                            ScanObject currentMinMax = factorRanges[factors.IndexOf("NAcetylhexosamine")];
                            AddMonoSaccharide.Add(myFragments, MonosaccharideName.NAcetylhexosamine, currentMinMax);
                        }
                        if (currentFactor == "NeuraminicAcid")
                        {
                            ScanObject currentMinMax = factorRanges[factors.IndexOf("NeuraminicAcid")];
                            AddMonoSaccharide.Add(myFragments, MonosaccharideName.NeuraminicAcid, currentMinMax);
                        }
                        if (currentFactor == "Pentose")
                        {
                            ScanObject currentMinMax = factorRanges[factors.IndexOf("Pentose")];
                            AddMonoSaccharide.Add(myFragments, MonosaccharideName.Pentose, currentMinMax);
                        }
                        if (currentFactor == "KDN")
                        {
                            ScanObject currentMinMax = factorRanges[factors.IndexOf("KDN")];
                            AddMonoSaccharide.Add(myFragments, MonosaccharideName.KDN, currentMinMax);
                        }
                    }
                    
                    //if (factors.Contains("Hexose"))
                    //{
                    //    ScanObject currentMinMax = factorRanges[factors.IndexOf("Hexose")];
                    //    Utiliites.AddMonosaccharide(myFragments, MonosaccharideName.Hexose, currentMinMax);
                    //}
                    //if (factors.Contains("Deoxyhexose"))
                    //{
                    //    ScanObject currentMinMax = factorRanges[factors.IndexOf("Deoxyhexose")];
                    //    Utiliites.AddMonosaccharide(myFragments, MonosaccharideName.Deoxyhexose, currentMinMax);
                    //}
                    //if (factors.Contains("NAcetylhexosamine"))
                    //{
                    //    ScanObject currentMinMax = factorRanges[factors.IndexOf("NAcetylhexosamine")];
                    //    Utiliites.AddMonosaccharide(myFragments, MonosaccharideName.NAcetylhexosamine, currentMinMax);
                    //}
                    //if (factors.Contains("NeuraminicAcid"))
                    //{
                    //    ScanObject currentMinMax = factorRanges[factors.IndexOf("NeuraminicAcid")];
                    //    Utiliites.AddMonosaccharide(myFragments, MonosaccharideName.NeuraminicAcid, currentMinMax);
                    //}
                    //if (factors.Contains("KDN"))
                    //{
                    //    ScanObject currentMinMax = factorRanges[factors.IndexOf("KDN")];
                    //    Utiliites.AddMonosaccharide(myFragments, MonosaccharideName.KDN, currentMinMax);
                    //}
                }

                if (addSulfate)
                {
                    MiscellaneousMatterLibrary matterLibrary = new MiscellaneousMatterLibrary();
                    matterLibrary.LoadLibrary();
                    string sulfateFormula = matterLibrary[MiscellaneousMatterName.Sulfate].ChemicalFormula;
                    string sulfateName = matterLibrary[MiscellaneousMatterName.Sulfate].Name;
                    double sulfateMass = matterLibrary[MiscellaneousMatterName.Sulfate].MassMonoIsotopic;

                    ScanObject currentMinMax = factorRanges[factors.IndexOf("Sulfate")];
                    AddMonoSaccharide.Add(myFragments, sulfateName, sulfateMass, sulfateFormula, currentMinMax.Min, currentMinMax.Max);
                }

                if (addWater)
                {
                    MiscellaneousMatterLibrary matterLibrary = new MiscellaneousMatterLibrary();
                    matterLibrary.LoadLibrary();
                    string waterFormula = matterLibrary[MiscellaneousMatterName.Water].ChemicalFormula;
                    string waterName = matterLibrary[MiscellaneousMatterName.Water].Name;
                    double waterMass = matterLibrary[MiscellaneousMatterName.Water].MassMonoIsotopic;

                    ScanObject currentMinMax = factorRanges[factors.IndexOf("Water")];
                    AddMonoSaccharide.Add(myFragments, waterName, waterMass, waterFormula, currentMinMax.Min, currentMinMax.Max);
                }

                if (addBonus)
                {
                    if (factors.Contains("HexNAc-ol"))
                    {
                        ScanObject currentMinMax = factorRanges[factors.IndexOf("HexNAc-ol")];
                        AddMonoSaccharide.Add(myFragments, "HexNAc-ol", Constants.Monosaccharides[MonosaccharideName.NAcetylhexosamine].MassMonoIsotopic + Constants.Elements[ElementName.Hydrogen].MassMonoIsotopic * 2, "C8H15NO5", currentMinMax.Min, currentMinMax.Max);
                    }

                    if (factors.Contains("DeuteratedHexNAc"))
                    {
                        ScanObject currentMinMax = factorRanges[factors.IndexOf("DeuteratedHexNAc")];
                        AddMonoSaccharide.Add(myFragments, "DeuteratedHexNAc", Constants.Monosaccharides[MonosaccharideName.NAcetylhexosamine].MassMonoIsotopic + Constants.SubAtomicParticles[SubAtomicParticleName.Neutron].MassMonoIsotopic, "C8H14NO5", currentMinMax.Min, currentMinMax.Max);
                    }
                        //Utiliites.AddMonosaccharide(myFragments, "DeuteratedHexNAc", Constants.Monosaccharides[MonosaccharideName.NAcetylhexosamine].MassMonoIsotopic + Constants.SubAtomicParticles[SubAtomicParticleName.Neutron].MassMonoIsotopic, Constants.Monosaccharides[MonosaccharideName.NAcetylhexosamine].ChemicalFormula + "D");
                }

                if (addMuddiman)
                {
                    SubAtomicParticleLibrary particleLibrary = new SubAtomicParticleLibrary();
                    particleLibrary.LoadLibrary();
                    string waterFormula = "H6"; //this is close as we will get here to 6 neutrons
                    string waterName = particleLibrary[SubAtomicParticleName.Neutron].Name + "6";
                    double waterMass = particleLibrary[SubAtomicParticleName.Neutron].MassMonoIsotopic*6;

                    ScanObject ranges = new ScanObject(0, 0, 0, 1);
                    AddMonoSaccharide.Add(myFragments, waterName, waterMass, waterFormula, ranges.Min, ranges.Max);
                }
            }
            return myFragments;
        }
    }
}
