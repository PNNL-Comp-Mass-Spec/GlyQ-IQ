using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HammerPeakDetector.Enumerations
{
    public enum HammerFilteringMethod
    {
        Threshold,//Calculates a local threshold based on the average of the 30 last isotopes from neighboring clusters
        Cluster,//uses a 0 threshold and only allows peaks through that have an isotope
        SmartCluster//uses a p-value cutoff to filter noise; as a result, signal must belong to a cluster
    }
}
