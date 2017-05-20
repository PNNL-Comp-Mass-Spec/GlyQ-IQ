using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Workflows.Backend.Core;

namespace IQGlyQ.Functions
{
    public static class CalibrateMS
    {
        public static List<IqTarget> CalibrateTargets(List<IqTarget> targets, double deltaMassCalibrationMZ, double deltaMassCalibrationMono)
        {
            
            
            Console.WriteLine("Calibrate Targets now...");
            if (targets.Count > 0)
            {
                for (int t = 0; t < targets.Count; t++)
                {
                    IqTarget baseTarget = targets[t];
                    TargetCalibration.Target(ref baseTarget, deltaMassCalibrationMZ, deltaMassCalibrationMono);


                    int childTargetsCount = baseTarget.GetChildCount();
                    if (childTargetsCount > 0)
                    {
                        List<IqTarget> currentTarget = baseTarget.ChildTargets().ToList();
                        for (int i = 0; i < childTargetsCount; i++)
                        {
                            IqTarget target = currentTarget[i];
                            TargetCalibration.Target(ref target, deltaMassCalibrationMZ, deltaMassCalibrationMono);
                        }
                    }
                }
                
            }
        
            return targets;
        }
    }
}
