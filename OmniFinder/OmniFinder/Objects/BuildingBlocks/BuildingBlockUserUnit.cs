using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Constants;
using OmniFinder.Objects.Enumerations;

namespace OmniFinder.Objects.BuildingBlocks
{
    public class BuildingBlockUserUnit : BuildingBlock
    {
        public UserUnitName Name { get; set; }

        public BuildingBlockUserUnit(UserUnitName name, RangesMinMax range)
        {
            Name = name;
            Range = range;
            BlockType = BuildingBlockType.UserUnit;           
        }
        public BuildingBlockUserUnit()
        {
            BlockType = BuildingBlockType.UserUnit;
        }
    }
}
