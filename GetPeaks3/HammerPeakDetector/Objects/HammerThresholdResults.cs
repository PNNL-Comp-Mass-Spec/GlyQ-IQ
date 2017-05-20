using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HammerPeakDetector.Parameters;

namespace HammerPeakDetector.Objects
{
    public class HammerThresholdResults
    {
        public bool IsSuccessful { get; set; }

        public HammerThresholdParameters NewParameters { get; set; }

        public HammerThresholdResults(HammerThresholdParameters parameters)
        {
            IsSuccessful = false;
            NewParameters = new HammerThresholdParameters();
            NewParameters.FilteringMethod = parameters.FilteringMethod;
            NewParameters.ThresholdSigmaMultiplier = parameters.ThresholdSigmaMultiplier;
            NewParameters.Iterations = parameters.Iterations;
            NewParameters.MassTolleranceSigmaMultiplier = parameters.MassTolleranceSigmaMultiplier;
        }
    }
}
