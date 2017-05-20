using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Constants;
using OmniFinder.Objects.Enumerations;

namespace OmniFinder.Objects.BuildingBlocks
{
    public class BuildingBlockSubAtomcParticle : BuildingBlock
    {
        public SubAtomicParticleName Name { get; set; }

        public BuildingBlockSubAtomcParticle(SubAtomicParticleName name, RangesMinMax range)
        {
            Name = name;
            Range = range;
            BlockType = BuildingBlockType.SubAtomicParticle;
        }

        public BuildingBlockSubAtomcParticle()
        {
            BlockType = BuildingBlockType.SubAtomicParticle;
        }
    }
}
