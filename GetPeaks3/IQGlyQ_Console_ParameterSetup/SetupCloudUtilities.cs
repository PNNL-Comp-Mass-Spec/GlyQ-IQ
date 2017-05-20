

using System.Collections.Generic;
using GetPeaksDllLite.DataFIFO;

namespace IQGlyQ_Console_ParameterSetup
{
    public static class SetupCloudUtilities
    {
        public static void WriteBatchFile(List<string> stringList, string fileName, string folder)
        {
            StringListToDisk writer = new StringListToDisk();
            string outputLocation;
            outputLocation = folder + @"\" + fileName;
            writer.toDiskStringList(outputLocation, stringList);
        }

        public static List<string> SetBasicTargetedWorkflowParameters()
        {
            string ChromSmootherNumPointsInSmooth = "9";
            string ChromToleranceInPPM = "10";
            string MSPeakDetectorPeakBR = "1.3";
            string MSPeakDetectorSigNoise = "3";
            string MSToleranceInPPM = "20";
            string NumMSScansToSum = "5";


            List<string> basicTargetedWorkflowParameters = new List<string>();

            basicTargetedWorkflowParameters.Add(@"<?xml version=" + "\"" + "1.0" + "\"" + " encoding=" + "\"" + "utf-8" + "\"" + "?>");
            basicTargetedWorkflowParameters.Add(@"<WorkflowParameters>");
            basicTargetedWorkflowParameters.Add(@"  <AreaOfPeakToSumInDynamicSumming>2</AreaOfPeakToSumInDynamicSumming>");
            basicTargetedWorkflowParameters.Add(@"  <ChromatogramCorrelationIsPerformed>true</ChromatogramCorrelationIsPerformed>");
            basicTargetedWorkflowParameters.Add(@"  <ChromGeneratorMode>MOST_ABUNDANT_PEAK</ChromGeneratorMode>");
            basicTargetedWorkflowParameters.Add(@"  <ChromGenSourceDataPeakBR>2</ChromGenSourceDataPeakBR>");
            basicTargetedWorkflowParameters.Add(@"  <ChromGenSourceDataSigNoise>3</ChromGenSourceDataSigNoise>");
            basicTargetedWorkflowParameters.Add(@"  <ChromNETTolerance>0.5</ChromNETTolerance>");
            basicTargetedWorkflowParameters.Add(@"  <ChromPeakDetectorPeakBR>1</ChromPeakDetectorPeakBR>");
            basicTargetedWorkflowParameters.Add(@"  <ChromPeakDetectorSigNoise>1</ChromPeakDetectorSigNoise>");
            basicTargetedWorkflowParameters.Add(@"  <ChromPeakSelectorMode>Smart</ChromPeakSelectorMode>");
            basicTargetedWorkflowParameters.Add(@"  <ChromSmootherNumPointsInSmooth>" + ChromSmootherNumPointsInSmooth + "</ChromSmootherNumPointsInSmooth>");
            basicTargetedWorkflowParameters.Add(@"  <ChromToleranceInPPM>" + ChromToleranceInPPM + "</ChromToleranceInPPM>");
            basicTargetedWorkflowParameters.Add(@"  <MaxScansSummedInDynamicSumming>100</MaxScansSummedInDynamicSumming>");
            basicTargetedWorkflowParameters.Add(@"  <MSPeakDetectorPeakBR>" + MSPeakDetectorPeakBR + "</MSPeakDetectorPeakBR>");
            basicTargetedWorkflowParameters.Add(@"  <MSPeakDetectorSigNoise>" + MSPeakDetectorSigNoise + "</MSPeakDetectorSigNoise>");
            basicTargetedWorkflowParameters.Add(@"  <MSToleranceInPPM>" + MSToleranceInPPM + "</MSToleranceInPPM>");
            basicTargetedWorkflowParameters.Add(@"  <MultipleHighQualityMatchesAreAllowed>true</MultipleHighQualityMatchesAreAllowed>");
            basicTargetedWorkflowParameters.Add(@"  <NumChromPeaksAllowedDuringSelection>40</NumChromPeaksAllowedDuringSelection>");
            basicTargetedWorkflowParameters.Add(@"  <NumMSScansToSum>" + NumMSScansToSum + "</NumMSScansToSum>");
            basicTargetedWorkflowParameters.Add(@"  <ResultType>BASIC_TARGETED_RESULT</ResultType>");
            basicTargetedWorkflowParameters.Add(@"  <SummingMode>SUMMINGMODE_STATIC</SummingMode>");
            basicTargetedWorkflowParameters.Add(@"  <WorkflowType>UnlabelledTargeted1</WorkflowType>");
            basicTargetedWorkflowParameters.Add(@"</WorkflowParameters>");


            return basicTargetedWorkflowParameters;
        }

        public static List<string> SetBasicGlyQIQParameters(string TargetsFilePath, string WorkingParametersFolder, string workingRawDataFolder, string workingResultsFolder)
        {
            List<string> glyQIQParameters = new List<string>();

            glyQIQParameters.Add(@"<?xml version=" + "\"" + "1.0" + "\"" + " encoding=" + "\"" + "utf-8" + "\"" + "?>");
            glyQIQParameters.Add(@"<WorkflowParameters>");
            glyQIQParameters.Add(@"  <AlignmentInfoFolder>" + WorkingParametersFolder + @"\AllignmentInfo</AlignmentInfoFolder>");
            glyQIQParameters.Add(@"  <CopyRawFileLocal>false</CopyRawFileLocal>");
            glyQIQParameters.Add(@"  <DeleteLocalDatasetAfterProcessing>false</DeleteLocalDatasetAfterProcessing>");
            glyQIQParameters.Add(@"  <FileContainingDatasetPaths>\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\QCShew_OrbiStandard_FileReference.txt</FileContainingDatasetPaths>");
            glyQIQParameters.Add(@"  <FolderPathForCopiedRawDataset>" + workingRawDataFolder + "</FolderPathForCopiedRawDataset>");
            glyQIQParameters.Add(@"  <LoggingFolder>" + WorkingParametersFolder + @"\Logs</LoggingFolder>");
            glyQIQParameters.Add(@"  <TargetsUsedForAlignmentFilePath></TargetsUsedForAlignmentFilePath>");
            glyQIQParameters.Add(@"  <TargetsFilePath>" + WorkingParametersFolder + @"\" + TargetsFilePath + "</TargetsFilePath>");
            glyQIQParameters.Add(@"  <ResultsFolder>" + workingResultsFolder + "</ResultsFolder>");
            glyQIQParameters.Add(@"  <TargetedAlignmentIsPerformed>false</TargetedAlignmentIsPerformed>");
            glyQIQParameters.Add(@"  <TargetedAlignmentWorkflowParameterFile>" + WorkingParametersFolder + @"\TargetedAlignmentWorkflowParameters.xml</TargetedAlignmentWorkflowParameterFile>");
            glyQIQParameters.Add(@"  <WorkflowParameterFile>" + WorkingParametersFolder + @"\BasicTargetedWorkflowParameters.xml</WorkflowParameterFile>");
            glyQIQParameters.Add(@"  <WorkflowType>BasicTargetedWorkflowExecutor</WorkflowType>");
            glyQIQParameters.Add(@"</WorkflowParameters>");


            return glyQIQParameters;


            //glyQIQParameters.Add(@"  <AlignmentInfoFolder>E:\ScottK\WorkingParameters\AllignmentInfo</AlignmentInfoFolder>");
            //glyQIQParameters.Add(@"  <FolderPathForCopiedRawDataset>E:\ScottK\WorkingData</FolderPathForCopiedRawDataset>");
        }

        public static List<string> SetTargetedAlignmentWorkflowParameters()
        {
            List<string> targetedAlignmentWorkflowParameters = new List<string>();

            targetedAlignmentWorkflowParameters.Add(@"<?xml version=" + "\"" + "1.0" + "\"" + " encoding=" + "\"" + "utf-8" + "\"" + "?>");
            targetedAlignmentWorkflowParameters.Add(@"<WorkflowParameters>");
            targetedAlignmentWorkflowParameters.Add(@"  <AlignmentInfoIsExported>true</AlignmentInfoIsExported>");
            targetedAlignmentWorkflowParameters.Add(@"  <ChromGeneratorMode>MOST_ABUNDANT_PEAK</ChromGeneratorMode>");
            targetedAlignmentWorkflowParameters.Add(@"  <ChromGenSourceDataPeakBR>2</ChromGenSourceDataPeakBR>");
            targetedAlignmentWorkflowParameters.Add(@"  <ChromGenSourceDataSigNoise>3</ChromGenSourceDataSigNoise>");
            targetedAlignmentWorkflowParameters.Add(@"  <ChromNETTolerance>0.3</ChromNETTolerance>");
            targetedAlignmentWorkflowParameters.Add(@"  <ChromPeakDetectorPeakBR>2</ChromPeakDetectorPeakBR>");
            targetedAlignmentWorkflowParameters.Add(@"  <ChromPeakDetectorSigNoise>2</ChromPeakDetectorSigNoise>");
            targetedAlignmentWorkflowParameters.Add(@"  <ChromSmootherNumPointsInSmooth>9</ChromSmootherNumPointsInSmooth>");
            targetedAlignmentWorkflowParameters.Add(@"  <ChromToleranceInPPM>30</ChromToleranceInPPM>");
            targetedAlignmentWorkflowParameters.Add(@"  <ExportAlignmentFolder />");
            targetedAlignmentWorkflowParameters.Add(@"  <FeaturesAreSavedToTextFile>true</FeaturesAreSavedToTextFile>");
            targetedAlignmentWorkflowParameters.Add(@"  <ImportedFeaturesFilename />");
            targetedAlignmentWorkflowParameters.Add(@"  <IScoreAllowedCriteria>0.15</IScoreAllowedCriteria>");
            targetedAlignmentWorkflowParameters.Add(@"  <MinimumChromPeakIntensityCriteria>250000</MinimumChromPeakIntensityCriteria>");
            targetedAlignmentWorkflowParameters.Add(@"  <MSPeakDetectorPeakBR>2</MSPeakDetectorPeakBR>");
            targetedAlignmentWorkflowParameters.Add(@"  <MSPeakDetectorSigNoise>2</MSPeakDetectorSigNoise>");
            targetedAlignmentWorkflowParameters.Add(@"  <MSToleranceInPPM>25</MSToleranceInPPM>");
            targetedAlignmentWorkflowParameters.Add(@"  <NumChromPeaksAllowedDuringSelection>6</NumChromPeaksAllowedDuringSelection>");
            targetedAlignmentWorkflowParameters.Add(@"  <NumDesiredMassTagsPerNETGrouping>25</NumDesiredMassTagsPerNETGrouping>");
            targetedAlignmentWorkflowParameters.Add(@"  <NumMaxAttemptsPerNETGrouping>200</NumMaxAttemptsPerNETGrouping>");
            targetedAlignmentWorkflowParameters.Add(@"  <NumMSScansToSum>1</NumMSScansToSum>");
            targetedAlignmentWorkflowParameters.Add(@"  <ResultType>BASIC_TARGETED_RESULT</ResultType>");
            targetedAlignmentWorkflowParameters.Add(@"  <UpperFitScoreAllowedCriteria>0.3</UpperFitScoreAllowedCriteria>");
            targetedAlignmentWorkflowParameters.Add(@"  <WorkflowType>TargetedAlignerWorkflow1</WorkflowType>");
            targetedAlignmentWorkflowParameters.Add(@"</WorkflowParameters>");



            return targetedAlignmentWorkflowParameters;
        }

    }
}
