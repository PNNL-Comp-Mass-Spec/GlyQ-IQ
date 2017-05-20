using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Constants;
using OmniFinder.Objects.Enumerations;
using PNNLOmics.Data.Constants.Libraries;

namespace OmniFinder.Objects.BuildingBlocks
{
    public abstract class BuildingBlock
    {
        /// <summary>
        /// which type of block this is.  enum/generic would be better
        /// </summary>
        public BuildingBlockType BlockType { get; set; }
 
        /// <summary>
        /// ranges for how many building blocks to use
        /// </summary>
        public RangesMinMax Range { get; set; }

        /// <summary>
        /// elemental composition corresponding to this building block
        /// </summary>
        public Dictionary<ElementName, int> ElementalCompositions { get; set; }

        public BuildingBlock()
        {
            ElementalCompositions = new Dictionary<ElementName, int>();
        }
    }
}
