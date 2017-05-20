namespace GetPeaksDllLite.Objects
{
    public class PeakDecon
    {
        public int peak_index {get;set;}
        public int scan_num {get;set;}
        public double mz {get;set;}
        public double intensity {get;set;}
        public float fwhm {get;set;}
        public float signal_noise{get;set;}
        public int MSFeatureID { get; set; }
    }

    public class PeakDeconLite
    {
        public string start { get; set; }
        public double mz { get; set; }
        public string end { get; set; }
    }
}
