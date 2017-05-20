namespace HammerPeakDetector.Objects.GetPeaksDifferenceFinder
{
    public class DifferenceObjectIsos<T>
    {
        public DifferenceObjectIsos()
        {
            DifferenceObject = new DifferenceObject<T>();
            IsosObjectLower = new IsosObject();
            IsosObjectHigher = new IsosObject();
        }

        public DifferenceObject<T> DifferenceObject { get; set; }
        public IsosObject IsosObjectLower { get; set; }
        public IsosObject IsosObjectHigher { get; set; }
    }
}
