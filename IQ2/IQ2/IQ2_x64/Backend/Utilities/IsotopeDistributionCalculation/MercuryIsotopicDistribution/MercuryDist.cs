 

using Run64.Backend.Core;
using Run64.Backend.Data;

namespace IQ_X64.Backend.Utilities.IsotopeDistributionCalculation.MercuryIsotopicDistribution
{
    public class MercuryDist
    {
        private IsotopicProfile isotopicProfile;

        public IsotopicProfile IsotopicProfile
        {
            get { return isotopicProfile; }
            set { isotopicProfile = value; }
        }
        private XYData xydata;

        public XYData Xydata
        {
            get { return xydata; }
            set { xydata = value; }
        }

        public MercuryDist()
        {
            isotopicProfile = new IsotopicProfile();
            xydata = new XYData();

        }
    }
}
