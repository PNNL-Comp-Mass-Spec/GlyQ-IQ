namespace GetPeaksDllLite.Functions
{
    public static class ConvertMzToMono
    {
        public static double Execute(double mZ, int charge, double massproton)
        {
            return mZ * charge - massproton * charge;//mass to charge
        }
    }
}
