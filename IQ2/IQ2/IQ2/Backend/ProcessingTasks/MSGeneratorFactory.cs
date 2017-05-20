using IQ.Backend.Core;
using IQ.Backend.ProcessingTasks.MSGenerators;
using Run32.Backend;

namespace IQ.Backend.ProcessingTasks
{
    public class MSGeneratorFactory
    {

        public MSGeneratorFactory()
        {

        }
        
        public static MSGenerator CreateMSGenerator(Globals.MSFileType filetype)
        {
            MSGenerator msGenerator;
            
            switch (filetype)
            {
                case Globals.MSFileType.Undefined:
                    msGenerator = new GenericMSGenerator();
                    break;
                case Globals.MSFileType.Agilent_WIFF:
                    msGenerator = new GenericMSGenerator();
                    break;
                case Globals.MSFileType.Agilent_D:
                    msGenerator = new GenericMSGenerator();
                    break;

                case Globals.MSFileType.Ascii:
                    msGenerator = new GenericMSGenerator();
                    break;
                case Globals.MSFileType.Bruker:
                    msGenerator = new GenericMSGenerator();
                    break;
                case Globals.MSFileType.Bruker_Ascii:
                    msGenerator = new GenericMSGenerator();
                    break;
                case Globals.MSFileType.Finnigan:
                    msGenerator = new GenericMSGenerator();
                    break;
                case Globals.MSFileType.ICR2LS_Rawdata:
                    msGenerator = new GenericMSGenerator();
                    break;
                case Globals.MSFileType.Micromass_Rawdata:
                    msGenerator = new GenericMSGenerator();
                    break;
                case Globals.MSFileType.MZXML_Rawdata:
                    msGenerator = new GenericMSGenerator();
                    break;
                case Globals.MSFileType.PNNL_IMS:
                    msGenerator = new GenericMSGenerator();
                    break;
                //case Globals.MSFileType.PNNL_UIMF:
                //    msGenerator = new UIMF_MSGenerator();
                //    break;
                case Globals.MSFileType.SUNEXTREL:
                    msGenerator = new GenericMSGenerator();
                    break;
                default:
                    msGenerator = new GenericMSGenerator();
                    break;
            }
            return msGenerator;

        }


        internal TaskIQ CreateMSGenerator(Globals.MSFileType fileType, double minMZ, double maxMZ, bool useMZRange)
        {
            MSGenerator msgenerator = CreateMSGenerator(fileType);

            if (useMZRange)
            {
                msgenerator.MinMZ = minMZ;
                msgenerator.MaxMZ = maxMZ;
            }
            return msgenerator;
            
        }
    }
}
