using Run32.Backend.Core;
using Run32.Backend.Data;

namespace IQ.Backend.Utilities.IsotopeDistributionCalculation.MercuryIsotopicDistribution
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
