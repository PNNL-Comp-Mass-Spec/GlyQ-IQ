using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Constants;
using OmniFinder.Objects.Enumerations;
using PNNLOmics.Data.Constants.Libraries;
using OmniFinder.Objects.BuildingBlocks;

namespace OmniFinder.Objects
{
    public class OmniFinderParameters
    {
        /// <summary>
        /// Which monosaccharide blocks are we loooking for
        /// </summary>
        public List<BuildingBlockMonoSaccharide> BuildingBlocksMonosacchcarides { get; set; }
        

        public List<BuildingBlockAminoAcid> BuildingBlocksAminoAcids { get; set; }
        
        public List<BuildingBlockCrossRing> BuildingBlocksCrossRings { get; set; }
        
        public List<BuildingBlockMiscellaneousMatter> BuildingBlocksMiscellaneousMatter { get; set; }
        
        public List<BuildingBlockElement> BuildingBlocksElements { get; set; }
        
        public List<BuildingBlockSubAtomcParticle> BuildingBlocksSubAtomicParticles { get; set; }

        public List<BuildingBlockUserUnit> BuildingBlocksUserUnit { get; set; }
        
        public Adducts ChargeCarryingAdduct { get; set; }

        public CarbType CarbohydrateType { get; set; }

        /// <summary>
        /// incase we need a user library
        /// </summary>
        public UserUnitLibrary UserUnitLibrary { get; set; }

        public OmniFinderParameters()
        {
            BuildingBlocksMonosacchcarides = new List<BuildingBlockMonoSaccharide>();

            BuildingBlocksAminoAcids = new List<BuildingBlockAminoAcid>();

            BuildingBlocksCrossRings = new List<BuildingBlockCrossRing>();

            BuildingBlocksMiscellaneousMatter = new List<BuildingBlockMiscellaneousMatter>();
            
            BuildingBlocksElements = new List<BuildingBlockElement>();

            BuildingBlocksSubAtomicParticles = new List<BuildingBlockSubAtomcParticle>();

            BuildingBlocksUserUnit = new List<BuildingBlockUserUnit>();

            UserUnitLibrary = new PNNLOmics.Data.Constants.Libraries.UserUnitLibrary();

            ChargeCarryingAdduct = Adducts.Monoisotopic;
            CarbohydrateType = CarbType.Aldehyde;
        }
    }
}
