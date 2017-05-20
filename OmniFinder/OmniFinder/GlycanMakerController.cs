using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmniFinder.Objects;
using OmniFinder.Functions;
using PNNLOmics.Data.Constants;
using OmniFinder.Objects.Enumerations;
using OmniFinder.Objects.BuildingBlocks;

namespace OmniFinder
{
    public static class GlycanMakerController
    {
        public static GlycanMakerOutput CalculateMass(GlycanMakerObject newBasket, int charge)
        {
            GlycanMakerObject updatedBasket = PreProcessGlycanMakerObject(newBasket);
            
            OmniFinderParameters parameters = updatedBasket.OmniFinderParameter;

            parameters.CarbohydrateType = newBasket.CarbohydrateType;
            parameters.ChargeCarryingAdduct = newBasket.ChargeCarryingAdduct;


            GlycanMakerOutput results = new GlycanMakerOutput();

            List<decimal> buildingblockMasses;
            List<string> buidingblockNames;
            List<int> rangeStart;
            List<int> rangeEnd;
            Dictionary<ElementName, int> compoundElementComposition;

            ConvertParameters converter = new ConvertParameters();
            converter.ConvertBuildingBlockEnumerationsToValues(parameters, out buildingblockMasses, out buidingblockNames, out rangeStart, out rangeEnd, out compoundElementComposition);

            //create integer matrix
            List<FactorialDesignFactor<int>> integerMatrix = new List<FactorialDesignFactor<int>>();
            FactorialDesign newDesigner = new FactorialDesign();
            newDesigner.CreateIntegerMatrix(buildingblockMasses.Count, rangeStart, rangeEnd);
            integerMatrix = newDesigner.Matrix;

            //convert integer matrix to Exact Mass objects
            OmniFinderExactMassGenerator newGenerator = new OmniFinderExactMassGenerator();
            List<OmnifinderExactMassObject> exactMasses;
            newGenerator.CalculateExactMasses(buildingblockMasses, integerMatrix, out exactMasses);

            //add adducts and carb types
            MiscellaneousMatterName convertCarbTypeToConstants = ConvertCarbTypeAndAdduct.SetCarbType(parameters, ref compoundElementComposition);

            decimal adductMass = ConvertCarbTypeAndAdduct.SetAdductMass(parameters, ref compoundElementComposition);

            foreach (OmnifinderExactMassObject massObject in exactMasses)
            {
                massObject.MassExact += Convert.ToDecimal(Constants.MiscellaneousMatter[convertCarbTypeToConstants].MassMonoIsotopic);
                //massObject.MassExact += adductMass;
            }

            results.ResultComposition = compoundElementComposition;

            //convert to elements

            results.MassNeutral = exactMasses[0].MassExact;
            results.MassToCharge = (results.MassNeutral + charge * adductMass) / charge;

            return results;
        }

        public static GlycanMakerObject PreProcessGlycanMakerObject(GlycanMakerObject newBasket)
        {
            //store inputs into omnifinder parameters
            for (int i = 0; i < newBasket.LegoBuildingBlocks.Count; i++)
            {
                switch (newBasket.LegoBuildingBlocks[i].BlockType)
                {
                    #region inside
                    case BuildingBlockType.AminoAcid:
                        {
                            BuildingBlockAminoAcid block = (BuildingBlockAminoAcid)newBasket.LegoBuildingBlocks[i];
                            newBasket.OmniFinderParameter.BuildingBlocksAminoAcids.Add(new BuildingBlockAminoAcid(block.Name, block.Range));
                        }
                        break;
                    case BuildingBlockType.CrossRing:
                        {
                            BuildingBlockCrossRing block = (BuildingBlockCrossRing)newBasket.LegoBuildingBlocks[i];
                            newBasket.OmniFinderParameter.BuildingBlocksCrossRings.Add(new BuildingBlockCrossRing(block.Name, block.Range));
                        }
                        break;
                    case BuildingBlockType.Element:
                        {
                            BuildingBlockElement block = (BuildingBlockElement)newBasket.LegoBuildingBlocks[i];
                            newBasket.OmniFinderParameter.BuildingBlocksElements.Add(new BuildingBlockElement(block.Name, block.Range));
                        }
                        break;
                    case BuildingBlockType.MiscellaneousMatter:
                        {
                            BuildingBlockMiscellaneousMatter block = (BuildingBlockMiscellaneousMatter)newBasket.LegoBuildingBlocks[i];
                            newBasket.OmniFinderParameter.BuildingBlocksMiscellaneousMatter.Add(new BuildingBlockMiscellaneousMatter(block.Name, block.Range));
                        }
                        break;
                    case BuildingBlockType.Monosaccharide:
                        {
                            BuildingBlockMonoSaccharide block = (BuildingBlockMonoSaccharide)newBasket.LegoBuildingBlocks[i];
                            newBasket.OmniFinderParameter.BuildingBlocksMonosacchcarides.Add(new BuildingBlockMonoSaccharide(block.Name, block.Range));
                        }
                        break;
                    case BuildingBlockType.SubAtomicParticle:
                        {
                            BuildingBlockSubAtomcParticle block = (BuildingBlockSubAtomcParticle)newBasket.LegoBuildingBlocks[i];
                            newBasket.OmniFinderParameter.BuildingBlocksSubAtomicParticles.Add(new BuildingBlockSubAtomcParticle(block.Name, block.Range));
                        }
                        break;
                    case BuildingBlockType.UserUnit:
                        {
                            BuildingBlockUserUnit block = (BuildingBlockUserUnit)newBasket.LegoBuildingBlocks[i];
                            BuildingBlockUserUnit test = new BuildingBlockUserUnit(block.Name, block.Range);
                            newBasket.OmniFinderParameter.BuildingBlocksUserUnit.Add(new BuildingBlockUserUnit(block.Name, block.Range));
                        }
                        break;
                    default:
                        {
                            Console.WriteLine("Wrong block type");
                        }
                        break;
                    #endregion
                }
            }

            return newBasket;
        }
    }
}
