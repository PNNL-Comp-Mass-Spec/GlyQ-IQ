namespace IQ.DeconEngine.PeakDetection
{
    public class clsPeak2
    {
        /// <summary>
        /// mz of the peak.
        /// </summary>
        public double mdbl_mz { get; set; }
       
        /// <summary>
        /// intensity of peak.
        /// </summary>
        public double  mdbl_intensity { get; set; }
        
        /// <summary>
        /// Signal to noise ratio
        /// </summary>
        public double mdbl_SN  { get; set; }

        /// <summary>
        /// index in PeakData::mvect_peak_tops std::vector
        /// </summary>
        public int mint_peak_index { get; set; }
			
        /// <summary>
        /// index in mzs, intensity vectors that were used to create the peaks in PeakProcessor::DiscoverPeaks
        /// </summary>
        public int mint_data_index { get; set; }

        /// <summary>
        /// Full width at half maximum for peak
        /// </summary>
	    public double mdbl_FWHM { get; set; }

        public clsPeak2(double mz, double intensity, double signal2noise, int peak_idx, int data_idx, double fwhm)
        {
            Set(mz, intensity, signal2noise, peak_idx, data_idx, fwhm);
        }

        public void Set(double mz, double intensity, double signal2noise, int peak_idx, int data_idx, double fwhm)
        {
            mdbl_mz = mz;
            mdbl_intensity = intensity;
            mdbl_SN = signal2noise;
            mint_peak_index = peak_idx;
            mint_data_index = data_idx;
            mdbl_FWHM = fwhm;
        }
    }
}
