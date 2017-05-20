using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmniFinder.Objects;
using PNNLOmics.Data.Constants;
using OmniFinder.Objects.BuildingBlocks;

namespace OmniFinder.Functions
{
    public class ConvertParameters
    {
        public void ConvertBuildingBlockEnumerationsToValues(
            OmniFinderParameters parameters, 
            out List<decimal> buildingblockMasses, 
            out List<string> buidingblockNames,
            out List<int> rangeStart,
            out List<int> rangeStop,
            out Dictionary<ElementName,int> compoundElementComposition
            )
        {
            buildingblockMasses = new List<decimal>();
            buidingblockNames = new List<string>();
            rangeStart = new List<int>();
            rangeStop = new List<int>();
            compoundElementComposition = new Dictionary<ElementName, int>();
            compoundElementComposition.Add(ElementName.Carbon, 0);
            compoundElementComposition.Add(ElementName.Hydrogen, 0);
            compoundElementComposition.Add(ElementName.Nitrogen, 0);
            compoundElementComposition.Add(ElementName.Oxygen, 0);
            compoundElementComposition.Add(ElementName.Sulfur, 0);
            compoundElementComposition.Add(ElementName.Phosphorous, 0);
            compoundElementComposition.Add(ElementName.Sodium, 0);
            compoundElementComposition.Add(ElementName.Potassium, 0);


            decimal massBlock = 0;
            string nameBlock;
            foreach (BuildingBlockMonoSaccharide block in parameters.BuildingBlocksMonosacchcarides)
            {
                massBlock = Convert.ToDecimal(Constants.Monosaccharides[block.Name].MassMonoIsotopic);
                nameBlock = Constants.Monosaccharides[block.Name].Name;

                buildingblockMasses.Add(massBlock);
                buidingblockNames.Add(nameBlock);
                rangeStart.Add(block.Range.MinRange);
                rangeStop.Add(block.Range.MaxRange);

                foreach (KeyValuePair<ElementName, int> element in block.ElementalCompositions)
                {
                    if (compoundElementComposition.ContainsKey(element.Key) && block.ElementalCompositions.ContainsKey(element.Key))
                    {
                        compoundElementComposition[element.Key] += block.ElementalCompositions[element.Key] * block.Range.MaxRange;
                    }
                    else 
                    {
                        compoundElementComposition.Add(element.Key, block.ElementalCompositions[element.Key] * block.Range.MaxRange);
                    }

                }
            }

            foreach (BuildingBlockAminoAcid block in parameters.BuildingBlocksAminoAcids)
            {
                massBlock = Convert.ToDecimal(Constants.AminoAcids[block.Name].MassMonoIsotopic);
                nameBlock = Constants.AminoAcids[block.Name].Name;

                buildingblockMasses.Add(massBlock);
                buidingblockNames.Add(nameBlock);
                rangeStart.Add(block.Range.MinRange);
                rangeStop.Add(block.Range.MaxRange);

                foreach (KeyValuePair<ElementName, int> element in block.ElementalCompositions)
                {
                    if (compoundElementComposition.ContainsKey(element.Key) && block.ElementalCompositions.ContainsKey(element.Key))
                    {
                        compoundElementComposition[element.Key] += block.ElementalCompositions[element.Key];
                    }
                    else
                    {
                        compoundElementComposition.Add(element.Key, block.ElementalCompositions[element.Key] * block.Range.MaxRange);
                    }

                }
            }
            foreach (BuildingBlockCrossRing block in parameters.BuildingBlocksCrossRings)
            {
                massBlock = Convert.ToDecimal(Constants.CrossRings[block.Name].MassMonoIsotopic);
                nameBlock = Constants.CrossRings[block.Name].Name;

                buildingblockMasses.Add(massBlock);
                buidingblockNames.Add(nameBlock);
                rangeStart.Add(block.Range.MinRange);
                rangeStop.Add(block.Range.MaxRange);

                foreach (KeyValuePair<ElementName, int> element in block.ElementalCompositions)
                {
                    if (compoundElementComposition.ContainsKey(element.Key) && block.ElementalCompositions.ContainsKey(element.Key))
                    {
                        compoundElementComposition[element.Key] += block.ElementalCompositions[element.Key];
                    }
                    else
                    {
                        compoundElementComposition.Add(element.Key, block.ElementalCompositions[element.Key] * block.Range.MaxRange);
                    }

                }
            }


            foreach (BuildingBlockMiscellaneousMatter block in parameters.BuildingBlocksMiscellaneousMatter)
            {
                massBlock = Convert.ToDecimal(Constants.MiscellaneousMatter[block.Name].MassMonoIsotopic);
                nameBlock = Constants.MiscellaneousMatter[block.Name].Name;

                buildingblockMasses.Add(massBlock);
                buidingblockNames.Add(nameBlock);
                rangeStart.Add(block.Range.MinRange);
                rangeStop.Add(block.Range.MaxRange);

                foreach (KeyValuePair<ElementName, int> element in block.ElementalCompositions)
                {
                    if (compoundElementComposition.ContainsKey(element.Key) && block.ElementalCompositions.ContainsKey(element.Key))
                    {
                        compoundElementComposition[element.Key] += block.ElementalCompositions[element.Key];
                    }
                    else
                    {
                        compoundElementComposition.Add(element.Key, block.ElementalCompositions[element.Key] * block.Range.MaxRange);
                    }

                }
            }

            foreach (BuildingBlockElement block in parameters.BuildingBlocksElements)
            {
                massBlock = Convert.ToDecimal(Constants.Elements[block.Name].MassMonoIsotopic);
                nameBlock = Constants.Elements[block.Name].Name;

                buildingblockMasses.Add(massBlock);
                buidingblockNames.Add(nameBlock);
                rangeStart.Add(block.Range.MinRange);
                rangeStop.Add(block.Range.MaxRange);

                foreach (KeyValuePair<ElementName, int> element in block.ElementalCompositions)
                {
                    if (compoundElementComposition.ContainsKey(element.Key) && block.ElementalCompositions.ContainsKey(element.Key))
                    {
                        compoundElementComposition[element.Key] += block.ElementalCompositions[element.Key];
                    }
                    else
                    {
                        compoundElementComposition.Add(element.Key, block.ElementalCompositions[element.Key] * block.Range.MaxRange);
                    }

                }
            }

            foreach (BuildingBlockSubAtomcParticle block in parameters.BuildingBlocksSubAtomicParticles)
            {
                massBlock = Convert.ToDecimal(Constants.SubAtomicParticles[block.Name].MassMonoIsotopic);
                nameBlock = Constants.SubAtomicParticles[block.Name].Name;

                buildingblockMasses.Add(massBlock);
                buidingblockNames.Add(nameBlock);
                rangeStart.Add(block.Range.MinRange);
                rangeStop.Add(block.Range.MaxRange);

                foreach (KeyValuePair<ElementName, int> element in block.ElementalCompositions)
                {
                    if (compoundElementComposition.ContainsKey(element.Key) && block.ElementalCompositions.ContainsKey(element.Key))
                    {
                        compoundElementComposition[element.Key] += block.ElementalCompositions[element.Key];
                    }
                    else
                    {
                        compoundElementComposition.Add(element.Key, block.ElementalCompositions[element.Key] * block.Range.MaxRange);
                    }

                }
            }

            foreach (BuildingBlockUserUnit block in parameters.BuildingBlocksUserUnit)
            {
                massBlock = Convert.ToDecimal(Constants.UserUnits[block.Name].MassMonoIsotopic);
                nameBlock = Constants.UserUnits[block.Name].Name;

                buildingblockMasses.Add(massBlock);
                buidingblockNames.Add(nameBlock);
                rangeStart.Add(block.Range.MinRange);
                rangeStop.Add(block.Range.MaxRange);

                foreach (KeyValuePair<ElementName, int> element in block.ElementalCompositions)
                {
                    if (compoundElementComposition.ContainsKey(element.Key) && block.ElementalCompositions.ContainsKey(element.Key))
                    {
                        compoundElementComposition[element.Key] += block.ElementalCompositions[element.Key];
                    }
                    else
                    {
                        compoundElementComposition.Add(element.Key, block.ElementalCompositions[element.Key] * block.Range.MaxRange);
                    }

                }
            }
        }
   
    }
}
