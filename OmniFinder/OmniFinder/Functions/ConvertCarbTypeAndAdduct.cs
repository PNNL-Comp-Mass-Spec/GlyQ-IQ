using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Constants;
using OmniFinder.Objects;

namespace OmniFinder.Functions
{
    public static class ConvertCarbTypeAndAdduct
    {
        /// <summary>
        /// convert enum adduct mass to decimal
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static decimal SetAdductMass(OmniFinderParameters parameters, ref Dictionary<ElementName, int> compoundElementComposition)
        {
            decimal adductMass = 0;

            decimal massElectron = Convert.ToDecimal(Constants.SubAtomicParticles[SubAtomicParticleName.Electron].MassMonoIsotopic);

            switch (parameters.ChargeCarryingAdduct)
            {
                case Objects.Enumerations.Adducts.DeProtonated:
                    {
                        adductMass = -Convert.ToDecimal(Constants.SubAtomicParticles[SubAtomicParticleName.Proton].MassMonoIsotopic);
                        if (compoundElementComposition.ContainsKey(ElementName.Hydrogen))
                        {
                            compoundElementComposition[ElementName.Hydrogen] -= 1;
                        }
                    }
                    break;
                case Objects.Enumerations.Adducts.H:
                    {
                        adductMass = Convert.ToDecimal(Constants.SubAtomicParticles[SubAtomicParticleName.Proton].MassMonoIsotopic);
                        if (compoundElementComposition.ContainsKey(ElementName.Hydrogen))
                        {
                            compoundElementComposition[ElementName.Hydrogen] += 1;
                        }
                        else
                        {
                            compoundElementComposition.Add(ElementName.Hydrogen,1);
                        }
                    }
                    break;
                case Objects.Enumerations.Adducts.K:
                    {
                        adductMass = Convert.ToDecimal(Constants.Elements[ElementName.Potassium].MassMonoIsotopic) - massElectron;
                        if (compoundElementComposition.ContainsKey(ElementName.Potassium))
                        {
                            compoundElementComposition[ElementName.Potassium] += 1;
                        }
                        else
                        {
                            compoundElementComposition.Add(ElementName.Potassium, 1);
                        }
                    }
                    break;
                case Objects.Enumerations.Adducts.Na:
                    {
                        adductMass = Convert.ToDecimal(Constants.Elements[ElementName.Sodium].MassMonoIsotopic) - massElectron;
                        if (compoundElementComposition.ContainsKey(ElementName.Sodium))
                        {
                            compoundElementComposition[ElementName.Sodium] += 1;
                        }
                        else
                        {
                            compoundElementComposition.Add(ElementName.Sodium, 1);
                        }
                    }
                    break;
                case Objects.Enumerations.Adducts.Neutral:
                    {
                        //do nothing because we do not need to add an extra water.  The aldehyde group will add the water we need
                        //adductMass = Convert.ToDecimal(Constants.MiscellaneousMatter[MiscellaneousMatterName.Water].MassMonoIsotopic);
                    }
                    break;
                case Objects.Enumerations.Adducts.NH4:
                    {
                        adductMass = Convert.ToDecimal(Constants.MiscellaneousMatter[MiscellaneousMatterName.Ammonium].MassMonoIsotopic);
                        if (compoundElementComposition.ContainsKey(ElementName.Nitrogen))
                        {
                            compoundElementComposition[ElementName.Nitrogen] += 1;
                        }
                        else
                        {
                            compoundElementComposition.Add(ElementName.Nitrogen, 1);
                        }
                        if (compoundElementComposition.ContainsKey(ElementName.Hydrogen))
                        {
                            compoundElementComposition[ElementName.Hydrogen] += 4;
                        }
                        else
                        {
                            compoundElementComposition.Add(ElementName.Hydrogen, 4);
                        }
                    }
                    break;
                case Objects.Enumerations.Adducts.UserDefined:
                    {
                        adductMass = Convert.ToDecimal(Constants.MiscellaneousMatter[MiscellaneousMatterName.Water].MassMonoIsotopic);
                    }
                    break;
                case Objects.Enumerations.Adducts.Monoisotopic:
                    {
                        //do nothing because we do not need to add an extra water.  The aldehyde group will add the water we need
                        //adductMass = Convert.ToDecimal(Constants.MiscellaneousMatter[MiscellaneousMatterName.Water].MassMonoIsotopic);
                    }
                    break;
                case Objects.Enumerations.Adducts.Water:
                    {
                        adductMass = Convert.ToDecimal(Constants.MiscellaneousMatter[MiscellaneousMatterName.Water].MassMonoIsotopic);
                        if (compoundElementComposition.ContainsKey(ElementName.Hydrogen))
                        {
                            compoundElementComposition[ElementName.Hydrogen] += 2;
                        }
                        else
                        {
                            compoundElementComposition.Add(ElementName.Hydrogen, 2);
                        }
                        if (compoundElementComposition.ContainsKey(ElementName.Oxygen))
                        {
                            compoundElementComposition[ElementName.Oxygen] += 1;
                        }
                        else
                        {
                            compoundElementComposition.Add(ElementName.Oxygen, 1);
                        }
                    }
                    break;
                default:
                    {
                        adductMass = Convert.ToDecimal(Constants.MiscellaneousMatter[MiscellaneousMatterName.Water].MassMonoIsotopic);
                        if (compoundElementComposition.ContainsKey(ElementName.Hydrogen))
                        {
                            compoundElementComposition[ElementName.Hydrogen] += 2;
                        }
                        else
                        {
                            compoundElementComposition.Add(ElementName.Hydrogen, 2);
                        }
                        if (compoundElementComposition.ContainsKey(ElementName.Oxygen))
                        {
                            compoundElementComposition[ElementName.Oxygen] += 1;
                        }
                        else
                        {
                            compoundElementComposition.Add(ElementName.Oxygen, 1);
                        }
                    }
                    break;
            }

            return adductMass;
        }

        /// <summary>
        /// convert enum carb type to enum MiscellaneousMatterName
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static MiscellaneousMatterName SetCarbType(OmniFinderParameters parameters, ref Dictionary<ElementName, int> compoundElementComposition)
        {
            MiscellaneousMatterName convertToConstants = new MiscellaneousMatterName();

            switch (parameters.CarbohydrateType)
            {
                case Objects.Enumerations.CarbType.Aldehyde:
                    {
                        convertToConstants = MiscellaneousMatterName.Aldehyde;

                        if (compoundElementComposition.ContainsKey(ElementName.Hydrogen))
                        {
                            compoundElementComposition[ElementName.Hydrogen] += 2;
                        }
                        else
                        {
                            compoundElementComposition.Add(ElementName.Hydrogen, 2);
                        }
                        if (compoundElementComposition.ContainsKey(ElementName.Oxygen))
                        {
                            compoundElementComposition[ElementName.Oxygen] += 1;
                        }
                        else
                        {
                            compoundElementComposition.Add(ElementName.Oxygen, 1);
                        }
                    }
                    break;
                case Objects.Enumerations.CarbType.Alditol:
                    {
                        convertToConstants = MiscellaneousMatterName.Alditol;

                        if (compoundElementComposition.ContainsKey(ElementName.Hydrogen))
                        {
                            compoundElementComposition[ElementName.Hydrogen] += 4;
                        }
                        else
                        {
                            compoundElementComposition.Add(ElementName.Hydrogen, 4);
                        }
                        if (compoundElementComposition.ContainsKey(ElementName.Oxygen))
                        {
                            compoundElementComposition[ElementName.Oxygen] += 1;
                        }
                        else
                        {
                            compoundElementComposition.Add(ElementName.Oxygen, 1);
                        }
                    }
                    break;
                case Objects.Enumerations.CarbType.Fragment:
                    {
                        convertToConstants = MiscellaneousMatterName.Fragment;
                        //nothing is needed here because we do not need to add the water
                        
                    }
                    break;
                case Objects.Enumerations.CarbType.Glycopeptide:
                    {
                        convertToConstants = MiscellaneousMatterName.Water;

                        if (compoundElementComposition.ContainsKey(ElementName.Hydrogen))
                        {
                            compoundElementComposition[ElementName.Hydrogen] += 1;
                        }
                        else
                        {
                            compoundElementComposition.Add(ElementName.Hydrogen, 1);
                        }
                        if (compoundElementComposition.ContainsKey(ElementName.Oxygen))
                        {
                            compoundElementComposition[ElementName.Oxygen] += 1;
                        }
                        else
                        {
                            compoundElementComposition.Add(ElementName.Oxygen, 1);
                        }
                    }
                    break;
                default:
                    {
                        convertToConstants = MiscellaneousMatterName.Aldehyde;

                        if (compoundElementComposition.ContainsKey(ElementName.Hydrogen))
                        {
                            compoundElementComposition[ElementName.Hydrogen] += 2;
                        }
                        else
                        {
                            compoundElementComposition.Add(ElementName.Hydrogen, 2);
                        }
                        if (compoundElementComposition.ContainsKey(ElementName.Oxygen))
                        {
                            compoundElementComposition[ElementName.Oxygen] += 1;
                        }
                        else
                        {
                            compoundElementComposition.Add(ElementName.Oxygen, 1);
                        }
                    }
                    break;
            }
            return convertToConstants;
        }
    }
}
