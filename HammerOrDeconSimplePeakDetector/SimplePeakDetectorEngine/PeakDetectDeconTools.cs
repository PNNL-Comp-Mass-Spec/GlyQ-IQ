using DeconTools.Backend.Core;
using DeconTools.Backend.Workflows;
using IQGlyQ.Objects;

namespace HammerPeakDetectorConsole
{
    public static class PeakDetectDeconTools
    {
        public static void DetectPeaksWithDecontools(ScanObject scanInfo, Run runIn, PeakDetectionParameters peakParameters)
        {
            PeakDetectAndExportWorkflow deconWorkflow = new PeakDetectAndExportWorkflow(runIn);
            deconWorkflow.InitializeWorkflow();

            deconWorkflow.WorkflowParameters = new PeakDetectAndExportWorkflowParameters();
            deconWorkflow.WorkflowParameters.PeakBR = peakParameters.PeakToBackgroundRatio;
            deconWorkflow.WorkflowParameters.SigNoiseThreshold = peakParameters.SignalToNoiseThreshold;
            deconWorkflow.WorkflowParameters.Num_LC_TimePointsSummed = peakParameters.ScansToSum;
            deconWorkflow.WorkflowParameters.LCScanMin = scanInfo.Min;
            deconWorkflow.WorkflowParameters.LCScanMax = scanInfo.Max;
            deconWorkflow.WorkflowParameters.OutputFolder = peakParameters.OutputDirectory;
            deconWorkflow.WorkflowParameters.ProcessMSMS = peakParameters.ProcessMSMS;
            deconWorkflow.WorkflowParameters.IsDataThresholded = peakParameters.IsDataThresholdedAndNoiseFloorRemoved;
            deconWorkflow.InitializeRunRelatedTasks();
            deconWorkflow.Execute();
        }
    }
}
