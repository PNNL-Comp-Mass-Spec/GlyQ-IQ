using DeconTools.Workflows.Backend.Results;

namespace Sipper.Model
{
    public class ResultWithImageInfo
    {

        #region Constructors
        public ResultWithImageInfo(SipperLcmsFeatureTargetedResultDTO result)
        {
            Result = result;

        }
        #endregion

        #region Properties

        public string MSImageFilePath { get; set; }

        public string TheorMSImageFilePath { get; set; }

        public string ChromImageFilePath { get; set; }
        #endregion

        public SipperLcmsFeatureTargetedResultDTO Result { get; set; }
    }
}
