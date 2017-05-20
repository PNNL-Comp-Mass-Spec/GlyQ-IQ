using IQ_X64.Backend.Utilities.IsotopeDistributionCalculation;
using Run64.Backend.Core;
using Run64.Utilities;

namespace IQ_X64.Backend.ProcessingTasks.TheorFeatureGenerator
{
    public class NominalMassFeatureGenerator : ITheorFeatureGenerator
    {
        private static readonly IsotopicDistributionCalculator IsotopicDistCalculator = IsotopicDistributionCalculator.Instance;
        private const double LowPeakCutOff = 0.005;

        public override void LoadRunRelatedInfo(ResultCollection results)
        {
            // do nothing
        }

        public override void GenerateTheorFeature(TargetBase mt)
        {
            Check.Require(mt != null, "FeatureGenerator failed. Target must not be null.");
            IsotopicProfile iso = IsotopicDistCalculator.GetAveraginePattern(mt.MZ);
            PeakUtilities.TrimIsotopicProfile(iso, LowPeakCutOff);
            iso.ChargeState = mt.ChargeState;
            mt.IsotopicProfile = iso;
        }
    }
}
