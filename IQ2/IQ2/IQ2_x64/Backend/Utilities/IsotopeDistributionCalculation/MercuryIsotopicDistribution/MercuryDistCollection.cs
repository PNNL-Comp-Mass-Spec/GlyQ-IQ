using System.Collections.Generic;

namespace IQ_X64.Backend.Utilities.IsotopeDistributionCalculation.MercuryIsotopicDistribution
{
    public class MercuryDistCollection
    {

        public HashSet<MercuryDist> mercDistList { get; set; }

        public MercuryDistCollection()
        {
            mercDistList = new HashSet<MercuryDist>();
        }

    }
}
