namespace GetPeaksDllLite.Objects
{
    public abstract class InputOutputBase
    {
        public string InputFileName { get; set; }
        public string InputSQLFileName { get; set; }
        public string OutputFileName { get; set; }
        public string OutputSQLFileName { get; set; }
    }

    public class InputOutputFileName : InputOutputBase
    {
        public string OutputPath { get; set; }
        public string InputPath { get; set; }
    }
}
