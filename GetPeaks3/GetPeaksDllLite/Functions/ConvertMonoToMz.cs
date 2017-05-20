namespace GetPeaksDllLite.Functions
{
    public static class ConvertMonoToMz
    {
        public static double Execute(double monoisotopicMass, int charge, double massproton)
        {
            return monoisotopicMass/charge + massproton;//mass to charge
            
        }
    }
}
