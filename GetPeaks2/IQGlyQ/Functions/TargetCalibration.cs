using GetPeaksDllLite.Functions;
using IQ.Workflows.Core;

namespace IQGlyQ.Functions
{
    public class TargetCalibration
    {
        public static void Target(ref IqTarget target, double deltaMassCalibrationMZ, double deltaMassCalibrationMono)
        {
            double mZTheorBase = target.MZTheor;
            double monoTheorBase = target.MonoMassTheor;
            MassCalibration.MassAndMono(ref mZTheorBase, ref monoTheorBase, target.ChargeState, deltaMassCalibrationMZ, deltaMassCalibrationMono);
            target.MZTheor = mZTheorBase;
            target.MonoMassTheor = monoTheorBase;
        }
    }
}
