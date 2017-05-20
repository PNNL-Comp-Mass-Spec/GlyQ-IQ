using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmniFinder.Objects.Enumerations;
using PNNLOmics.Data.Constants;

namespace OmniFinder.Objects.BuildingBlocks
{
    public class BuildingBlockAminoAcid : BuildingBlock
    {
        public AminoAcidName Name { get; set; }

        public BuildingBlockAminoAcid(AminoAcidName name, RangesMinMax range)
        {
            Name = name;
            Range = range;
            BlockType = BuildingBlockType.AminoAcid;
            ElementalCompositions = SetElementsAminoAcid(name);
        }

        public BuildingBlockAminoAcid()
        {
            BlockType = BuildingBlockType.AminoAcid;
        }

        private static Dictionary<ElementName, int> SetElementsAminoAcid(AminoAcidName name)
        {
            Dictionary<ElementName, int> ElementalCompositions = new Dictionary<ElementName, int>();
            ElementalCompositions.Add(ElementName.Carbon, Constants.AminoAcids[name].NumCarbon);
            ElementalCompositions.Add(ElementName.Hydrogen, Constants.AminoAcids[name].NumHydrogen);
            ElementalCompositions.Add(ElementName.Nitrogen, Constants.AminoAcids[name].NumNitrogen);
            ElementalCompositions.Add(ElementName.Oxygen, Constants.AminoAcids[name].NumOxygen);
            ElementalCompositions.Add(ElementName.Phosphorous, Constants.AminoAcids[name].NumPhosphorus);
            ElementalCompositions.Add(ElementName.Potassium, Constants.AminoAcids[name].NumPotassium);
            ElementalCompositions.Add(ElementName.Sodium, Constants.AminoAcids[name].NumSodium);
            ElementalCompositions.Add(ElementName.Sulfur, Constants.AminoAcids[name].NumSulfur);

            return ElementalCompositions;
        }
    }
}
