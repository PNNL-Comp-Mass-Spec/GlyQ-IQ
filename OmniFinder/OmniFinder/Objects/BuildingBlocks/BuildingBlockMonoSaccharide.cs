using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Constants;
using OmniFinder.Objects.Enumerations;

namespace OmniFinder.Objects.BuildingBlocks
{
   public class BuildingBlockMonoSaccharide : BuildingBlock
    {
        public MonosaccharideName Name { get; set; }

        public BuildingBlockMonoSaccharide(MonosaccharideName name, RangesMinMax range)
        {
            Name = name;
            Range = range;
            BlockType = BuildingBlockType.Monosaccharide;
            ElementalCompositions = SetElementsMonosaccharide(name);
        }

        public BuildingBlockMonoSaccharide()
        {
            BlockType = BuildingBlockType.Monosaccharide;
        }

        private static Dictionary<ElementName, int> SetElementsMonosaccharide(MonosaccharideName name)
        {
            Dictionary<ElementName, int> ElementalCompositions = new Dictionary<ElementName, int>();
            ElementalCompositions.Add(ElementName.Carbon, Constants.Monosaccharides[name].NumCarbon);
            ElementalCompositions.Add(ElementName.Hydrogen, Constants.Monosaccharides[name].NumHydrogen);
            ElementalCompositions.Add(ElementName.Nitrogen, Constants.Monosaccharides[name].NumNitrogen);
            ElementalCompositions.Add(ElementName.Oxygen, Constants.Monosaccharides[name].NumOxygen);
            ElementalCompositions.Add(ElementName.Phosphorous, Constants.Monosaccharides[name].NumPhosphorus);
            ElementalCompositions.Add(ElementName.Potassium, Constants.Monosaccharides[name].NumPotassium);
            ElementalCompositions.Add(ElementName.Sodium, Constants.Monosaccharides[name].NumSodium);
            ElementalCompositions.Add(ElementName.Sulfur, Constants.Monosaccharides[name].NumSulfur);

            return ElementalCompositions;
        }
    }
}
