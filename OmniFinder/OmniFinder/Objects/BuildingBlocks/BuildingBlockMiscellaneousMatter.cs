using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmniFinder.Objects.Enumerations;
using PNNLOmics.Data.Constants;

namespace OmniFinder.Objects.BuildingBlocks
{
    public class BuildingBlockMiscellaneousMatter : BuildingBlock
    {
        public MiscellaneousMatterName Name { get; set; }
        public BuildingBlockMiscellaneousMatter(MiscellaneousMatterName name, RangesMinMax range)
        {
            Name = name;
            Range = range;
            BlockType = BuildingBlockType.MiscellaneousMatter;
            ElementalCompositions = SetElementsMiscellaneousMatter(name);
        }

        public BuildingBlockMiscellaneousMatter()
        {
            BlockType = BuildingBlockType.MiscellaneousMatter;
        }

        private static Dictionary<ElementName, int> SetElementsMiscellaneousMatter(MiscellaneousMatterName name)
        {
            Dictionary<ElementName, int> ElementalCompositions = new Dictionary<ElementName, int>();
            ElementalCompositions.Add(ElementName.Carbon, Constants.MiscellaneousMatter[name].NumCarbon);
            ElementalCompositions.Add(ElementName.Hydrogen, Constants.MiscellaneousMatter[name].NumHydrogen);
            ElementalCompositions.Add(ElementName.Nitrogen, Constants.MiscellaneousMatter[name].NumNitrogen);
            ElementalCompositions.Add(ElementName.Oxygen, Constants.MiscellaneousMatter[name].NumOxygen);
            ElementalCompositions.Add(ElementName.Phosphorous, Constants.MiscellaneousMatter[name].NumPhosphorus);
            ElementalCompositions.Add(ElementName.Potassium, Constants.MiscellaneousMatter[name].NumPotassium);
            ElementalCompositions.Add(ElementName.Sodium, Constants.MiscellaneousMatter[name].NumSodium);
            ElementalCompositions.Add(ElementName.Sulfur, Constants.MiscellaneousMatter[name].NumSulfur);

            return ElementalCompositions;
        }
    }
}
