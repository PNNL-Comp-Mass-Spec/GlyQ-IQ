using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmniFinder.Objects;
using OmniFinder.Objects.Enumerations;

namespace GetPeaks_DLL.Glycolyzer.Objects
{
    public class OmnifinderCheckObject
    {
        public int holdBuildingBlockAminoAcid { get; set; }
        public int holdBuildingBlockCrossRing { get; set; }
        public int holdBuildingBlockElements { get; set; }
        public int holdBuildingBlockMiscellaneousMatter { get; set; }
        public int holdBuildingBlockMonosaccharides { get; set; }
        public int holdBuildingSubAtomicParticles { get; set; }
        public int holdBuildingBlockUserUnit { get; set; }

        public CarbType holdCarbType { get; set; }
        public Adducts holdAdduct { get; set; }

        public List<RangesMinMax> testRanges { get; set; }

        public OmnifinderCheckObject()
        {
            testRanges = new List<RangesMinMax>();
        }


    }
}
