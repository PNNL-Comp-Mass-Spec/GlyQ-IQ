using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrbitrapPeakThresholder.Objects
{
    public class OrbitrapPeakParameters
    {
        /// <summary>
        /// low ppm for aligning compare lists via mass
        /// </summary>
        public double massTolleranceMatch {get;set;}//= 3;  
        
        /// <summary>
        /// Sigma above the nosie for CP thresholding.  0 for parameter free operation mode.  negative for better sensitiviity
        /// </summary>
        public double orbitrapFilterSigmaMultiplier {get;set;}//=0;
    }
}
