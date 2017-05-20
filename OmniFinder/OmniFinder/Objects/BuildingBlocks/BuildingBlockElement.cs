using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmniFinder.Objects.Enumerations;
using PNNLOmics.Data.Constants;

namespace OmniFinder.Objects.BuildingBlocks
{
    public class BuildingBlockElement : BuildingBlock
    {
        public ElementName Name { get; set; }

        public BuildingBlockElement(ElementName name, RangesMinMax range)
        {
            Name = name;
            Range = range;
            BlockType = BuildingBlockType.Element;
            ElementalCompositions = SetElementsElement(name);
        }

        public BuildingBlockElement()
        {
            BlockType = BuildingBlockType.Element;
        }

        private static Dictionary<ElementName, int> SetElementsElement(ElementName name)
        {
            Dictionary<ElementName, int> ElementalCompositions = new Dictionary<ElementName, int>();
            ElementalCompositions.Add(name, 1);

            return ElementalCompositions;
        }
    }
}
