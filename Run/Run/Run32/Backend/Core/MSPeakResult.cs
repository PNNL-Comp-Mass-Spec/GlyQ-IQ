namespace Run32.Backend.Core
{
    public class MSPeakResult
    {
        private MSPeak mSPeak;

        public MSPeak MSPeak
        {
            get { return mSPeak; }
            set { mSPeak = value; }
        }

        private ScanSet scanSet;

        public ScanSet ScanSet
        {
            get { return scanSet; }
            set { scanSet = value; }
        }

    }
}
