using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmniFinder.Objects.Enumerations;
using OmniFinder.Objects.BuildingBlocks;
using PNNLOmics.Data.Constants;

namespace OmniFinder.Objects
{
    public class GlycanMakerOutput
    {
        /// <summary>
        /// Contains the neutral mass
        /// </summary>
        public decimal MassNeutral { get; set; }

        /// <summary>
        /// m/z value
        /// </summary>
        public decimal MassToCharge { get; set; }
        
        /// <summary>
        /// Charged adduct attached to the molecule (Neutral, H+, Na+ etc.)
        /// </summary>
        public Adducts Adduct { get; set; }

        /// <summary>
        /// Terminal of the glycan
        /// </summary>
        public CarbType CarbType { get; set; }

        /// <summary>
        /// number of charges
        /// </summary>
        public int Charge { get; set; }

        /// <summary>
        /// combined building block from the maker
        /// </summary>
        public Dictionary<ElementName, int> ResultComposition { get; set; }
        
        public void OmniFinderOuput()
        {
            ResultComposition = new Dictionary<ElementName, int>();
        }
    }
}
