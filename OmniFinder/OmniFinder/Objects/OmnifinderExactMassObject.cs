using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmniFinder.Objects
{
    public class OmnifinderExactMassObject
    {
        /// <summary>
        /// Exact mass
        /// </summary>
        public decimal MassExact { get; set; }

        /// <summary>
        /// Integers correspond to the number of each compsosition
        /// </summary>
        public List<int> ListOfCompositions { get; set; }

        public OmnifinderExactMassObject()
        {
            ListOfCompositions = new List<int>();
        }
    }
}
