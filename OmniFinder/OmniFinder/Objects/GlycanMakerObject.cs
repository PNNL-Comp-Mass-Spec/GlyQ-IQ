using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Constants;
using OmniFinder.Objects.Enumerations;
using OmniFinder.Objects.BuildingBlocks;

namespace OmniFinder.Objects
{
    public class GlycanMakerObject
    {
        /// <summary>
        /// Buiding Blocks of interest
        /// </summary>
        public List<BuildingBlock> LegoBuildingBlocks { get; set; }

        /// <summary>
        /// Terminal group or extra matter adducts
        /// </summary>
        public CarbType CarbohydrateType { get; set; }

        /// <summary>
        /// Charge state
        /// </summary>
        public int Charge { get; set; }

        /// <summary>
        /// What type molecule is carrying the charge
        /// </summary>
        public Adducts ChargeCarryingAdduct { get; set; }

        /// <summary>
        /// Mass Tollerance value in ppm
        /// </summary>
        public double MassTollerance { get; set; }

        /// <summary>
        /// core parameter file for running the OmniFinder
        /// </summary>
        public OmniFinderParameters OmniFinderParameter { get; set; }

        public GlycanMakerObject()
        {
            LegoBuildingBlocks = new List<BuildingBlock>();
            OmniFinderParameter = new OmniFinderParameters(); 
        }
    }
}
