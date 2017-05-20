using PNNLOmics.Data.Features;

namespace GetPeaksDllLite.Objects.TandemMSObjects
{
    public class PeakCentric: MSFeatureLight
    {
        public int PeakID { get; set; }

        public int ScanID { get; set; }

        //public int GroupID { get; set; }

        //this needs to be pulled out to a group table
        public int MonoisotopicClusterID { get; set; }

        //this needs to be pulled out to a group table
        public int FeatureClusterID { get; set; }

        public double Mass { get; set; }

        public double Height { get; set; }

        public double Width { get; set; }

        public double Background { get; set; }

        public double LocalSignalToNoise { get; set; }

        //these are preprocessing parameters
        ////how many points define the peak
        //public int MinimaOfLowerMassIndex { get; set; }

        //public int MinimaOfHigherMassIndex { get; set; }

        //public int ChargeState { get; set; }

        //public double MassMonoisotopic { get; set; }

        //public double Score { get; set; }

        //public double AmbiguityScore { get; set; }

        //attributes
        public bool isSignal { get; set; }

        public bool isCentroided { get; set; }

        public bool isMonoisotopic { get; set; }

        public bool isIsotope { get; set; }

        public bool isMostAbundant { get; set; }

        public bool isCharged { get; set; }

        public bool isCorrected { get; set; }



        //public bool isPrecursorMass { get; set; }

        public PeakCentric()
        {
            isSignal = false;
            isCentroided = false;
            isMonoisotopic = false;
            isIsotope = false;
            isMostAbundant = false;
            isCharged = false;
            isCorrected = false;
            //isPrecursorMass = false;
        }
    }
}
