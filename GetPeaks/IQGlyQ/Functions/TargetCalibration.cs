using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Workflows.Backend.Core;
using GetPeaks_DLL.Functions;

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
