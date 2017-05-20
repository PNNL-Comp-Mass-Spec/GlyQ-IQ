using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OmniFinder.Objects.Enumerations;

namespace OmniFinder.Objects
{
    public class OmniFinderOutput
    {
        /// <summary>
        /// Contains the amount of each block that corresponds to the Composition Headders
        /// </summary>
        public List<OmnifinderExactMassObject> MassAndComposition { get; set; }

        /// <summary>
        /// Headders for the compositon blocks
        /// </summary>
        public List<string> CompositionHeaders { get; set; }

        /// <summary>
        /// Charged adduct attached to the molecule (Neutral, H+, Na+ etc.)
        /// </summary>
        public Adducts Adduct { get; set; }

        /// <summary>
        /// Terminal of the glycan
        /// </summary>
        public CarbType CarbType {get;set;}

        public void OmniFinderOuput()
        {
            MassAndComposition = new List<OmnifinderExactMassObject>();
            CompositionHeaders = new List<string>();
        } 
    }
}
