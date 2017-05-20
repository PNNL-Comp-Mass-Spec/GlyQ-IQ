namespace Run64.DeconEngine.PeakDetection
{
    public static class PeakProcessorHelpers
    {
        public static double absolute(double x)
		{
            if (x >= 0)
            {
                return x;
            }
            return -1 * x ; 
		}

       
    }
}
