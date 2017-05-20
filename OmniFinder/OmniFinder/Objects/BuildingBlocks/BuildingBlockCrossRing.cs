using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Constants;
using OmniFinder.Objects.Enumerations;

namespace OmniFinder.Objects.BuildingBlocks
{
    public class BuildingBlockCrossRing : BuildingBlock
    {
        public CrossRingName Name { get; set; }
        public BuildingBlockCrossRing(CrossRingName name, RangesMinMax range)
        {
            Name = name;
            Range = range;
            BlockType = BuildingBlockType.CrossRing;
            ElementalCompositions = SetElementsCrossRing(name);
        }

        public BuildingBlockCrossRing()
        {
            BlockType = BuildingBlockType.CrossRing;
        }

        private static Dictionary<ElementName, int> SetElementsCrossRing(CrossRingName name)
        {
            Dictionary<ElementName, int> ElementalCompositions = new Dictionary<ElementName, int>();
            ElementalCompositions.Add(ElementName.Carbon, Constants.CrossRings[name].NumCarbon);
            ElementalCompositions.Add(ElementName.Hydrogen, Constants.CrossRings[name].NumHydrogen);
            ElementalCompositions.Add(ElementName.Nitrogen, Constants.CrossRings[name].NumNitrogen);
            ElementalCompositions.Add(ElementName.Oxygen, Constants.CrossRings[name].NumOxygen);
            ElementalCompositions.Add(ElementName.Phosphorous, Constants.CrossRings[name].NumPhosphorus);
            ElementalCompositions.Add(ElementName.Potassium, Constants.CrossRings[name].NumPotassium);
            ElementalCompositions.Add(ElementName.Sodium, Constants.CrossRings[name].NumSodium);
            ElementalCompositions.Add(ElementName.Sulfur, Constants.CrossRings[name].NumSulfur);

            return ElementalCompositions;
        }

    }
}
