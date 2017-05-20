
namespace IQ.Workflows
{

    public enum ValidationCode
    {
        None,
        Yes,
        No,
        Maybe
    }
    
    public class GlobalsWorkFlow
    {


        public enum TargetedWorkflowTypes
        {
            Undefined,
            UnlabelledTargeted1,
            O16O18Targeted1,
            N14N15Targeted1,
            TargetedAlignerWorkflow1, 
            PeakDetectAndExportWorkflow1,
            SipperTargeted1,
            BasicTargetedWorkflowExecutor1,
            LcmsFeatureTargetedWorkflowExecutor1,
            SipperWorkflowExecutor1,
			TopDownTargeted1,
			TopDownTargetedWorkflowExecutor1,
			UIMFTargetedMSMSWorkflowCollapseIMS,
            IQMillionWorkflow,
            Deuterated
        }


        public enum TargetType
        {
            LcmsFeature,
            DatabaseTarget
        }

        public enum SummingModeEnum
        {
            SUMMINGMODE_STATIC,     // mode in which the number of scans summed is always the same
            SUMMINGMODE_DYNAMIC    // mode in which the number of scans summed is variable; depends on chromatograph peak dimensions

        }

        public enum RatioType
        {
            ISO1_OVER_ISO2,
            ISO2_OVER_ISO1

        }


    }
}
