namespace Run64.DeconEngine.PeakDetection
{
    public enum PEAK_FIT_TYPE
    {
        APEX = 0,  /*!< The peak is the m/z value higher than the points before and after it */
        QUADRATIC, /*!< The peak is the m/z value which is a quadratic fit of the three points around the apex */
        LORENTZIAN /*!< The peak is the m/z value which is a lorentzian fit of the three points around the apex */
    }

    public enum PEAK_PROFILE_TYPE
    {
        PROFILE = 0,
        CENTROIDED
    }


}
