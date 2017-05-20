using Run64.Backend;

namespace IQGlyQ.Objects
{
    public class MSGeneratorParameters
    {
        public Globals.MSFileType MsFileType { get; set; }

        public MSGeneratorParameters()
        {
            MsFileType = Globals.MSFileType.Undefined;
        }

        public MSGeneratorParameters(Globals.MSFileType msFileType)
        {
            MsFileType = msFileType;
        }
    }
}
