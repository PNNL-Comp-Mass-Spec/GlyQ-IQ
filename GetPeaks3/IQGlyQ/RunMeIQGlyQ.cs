using System;
using System.Collections.Generic;

//using GetPeaks_DLL.DataFIFO;
using System.IO;
using IQGlyQ.Enumerations;
using IQGlyQ.Processors;
using IQGlyQ.TargetGenerators;
using IQ_X64.Workflows;
using IQ_X64.Workflows.WorkFlowParameters;
using IQ_X64.Workflows.WorkFlowPile;
using PNNLOmics.Algorithms.PeakDetection;
using IQGlyQ.Functions;
using Run64.Backend;

namespace IQGlyQ
{
    public class RunMeIQGlyQ
    {
        public void ExecuteDeuteratedTargetedWorkflow(string executorParameterFile, string testDatasetPath, string testDatasetName)
        {
            
            
          
            //DeuteratedTargetedWorkflowExecutorParameters executorParameters = new DeuteratedTargetedWorkflowExecutorParameters();
            //executorParameters.LoadParameters(executorParameterFile);
            //string resultsFolderLocation = executorParameters.ResultsFolder;

            BasicTargetedWorkflowExecutorParameters executorParameters = new BasicTargetedWorkflowExecutorParameters();
            executorParameters.LoadParameters(executorParameterFile);
            string resultsFolderLocation = executorParameters.ResultsFolder;

            string expectedResultsFilename = resultsFolderLocation + "\\" + testDatasetName + "_results.txt";
            if (File.Exists(expectedResultsFilename))
            {
                File.Delete(expectedResultsFilename);
            }

            DeuteratedTargetedWorkflowParameters deuteriumTargetedWorkflowParameters = new DeuteratedTargetedWorkflowParameters();

            deuteriumTargetedWorkflowParameters.IsotopeLabelingEfficiency = 1.1;//1.1 looks best for 1:1 ratio mixing
            deuteriumTargetedWorkflowParameters.MolarMixingFractionOfH = 0.5;//0.5 default for 1:1 mixing
            deuteriumTargetedWorkflowParameters.ChromNETTolerance = 1.28;//min is 0.08.  0.28 gets more scanStart and scanEnd do not matter
            deuteriumTargetedWorkflowParameters.SummingMode = GlobalsWorkFlow.SummingModeEnum.SUMMINGMODE_STATIC;
            deuteriumTargetedWorkflowParameters.NumMSScansToSum = 5;
            deuteriumTargetedWorkflowParameters.NumChromPeaksAllowedDuringSelection = 50;
            deuteriumTargetedWorkflowParameters.MultipleHighQualityMatchesAreAllowed = true;
            deuteriumTargetedWorkflowParameters.ChromPeakSelectorMode = Globals.PeakSelectorMode.ClosestToTarget;
            deuteriumTargetedWorkflowParameters.ChromSmootherNumPointsInSmooth = 9;

            //BasicTargetedWorkflow workflow = new BasicTargetedWorkflow(basicTargetedWorkflowParameters);
            //DeuteratedTargetedWorkflow workflow = new DeuteratedTargetedWorkflow(deuteriumTargetedWorkflowParameters);
            DeuteratedTargetedWorkflow workflow = new DeuteratedTargetedWorkflow(deuteriumTargetedWorkflowParameters);

            TargetedWorkflowExecutor executor = new BasicTargetedWorkflowExecutor(executorParameters, workflow, testDatasetPath);
            executor.Execute();

            
            //Assert.IsTrue(File.Exists(expectedResultsFilename));
        }


        //public static BasicTargetedWorkflowExecutorParameters BasicTargetedWorkflowExecutorParametersSetIq(string executorParameterFile, string factorFile, string xyDataPath,  FragmentedTargetedPeakProcessingParameters processingParameters, out FragmentedTargetedWorkflowParametersIQ fragmentedTargetedWorkflowParameters, EnumerationDataset thisDataset, EnumerationIsPic isPic, bool isUnitTest)
        
        /// <summary>
        /// current best
        /// </summary>
        /// <param name="executorParameterFile"></param>
        /// <param name="FragmentsIQ"></param>
        /// <param name="fragmentedTargetedWorkflowParameters"></param>
        /// <returns></returns>
        //public static BasicTargetedWorkflowExecutorParameters BasicTargetedWorkflowExecutorParametersSetIq(string executorParameterFile, string factorFile, string xyDataPath,  FragmentedTargetedPeakProcessingParameters processingParameters, out FragmentedTargetedWorkflowParametersIQ fragmentedTargetedWorkflowParameters, EnumerationIsotopicProfileMode isoMode,  EnumerationIsPic isPic, bool isUnitTest)
        public static BasicTargetedWorkflowExecutorParameters BasicTargetedWorkflowExecutorParametersSetIq(string executorParameterFile, string factorFile, string xyDataPath,  FragmentedTargetedPeakProcessingParameters processingParameters, out FragmentedTargetedWorkflowParametersIQ fragmentedTargetedWorkflowParameters, EnumerationIsPic isPic, bool isUnitTest)
        
        {
            BasicTargetedWorkflowExecutorParameters executorParameters = new BasicTargetedWorkflowExecutorParameters();
            executorParameters.LoadParameters(executorParameterFile);


            executorParameters.MinMzForDefiningChargeStateTargets = processingParameters.MinMzForDefiningChargeStateTargets;
            executorParameters.MaxMzForDefiningChargeStateTargets = processingParameters.MaxMzForDefiningChargeStateTargets;
            
            //this is not what you want because it starts at the highest charge state at the top of the list
            //executorParameters.MaxNumberOfChargeStateTargetsToCreate = processingParameters.ChargeStateMax;
            
            Console.WriteLine("Start Factors Setup");

            List<FragmentIQTarget> myFragments = ConvertFactorToIQTargets.factorsToFragments(factorFile, isUnitTest);

            Console.WriteLine("FactorsSet");
            //or
            //fragmentedTargetedWorkflowParameters.FragmentsIQ = SetUpTargets();

            //BasicTargetedWorkflowParameters basicTargetedWorkflowParameters = new BasicTargetedWorkflowParameters();
            fragmentedTargetedWorkflowParameters = new FragmentedTargetedWorkflowParametersIQ(myFragments);

            fragmentedTargetedWorkflowParameters.IsPic = isPic;

            //fragmentedTargetedWorkflowParameters.MSParameters.IsoParameters.IsotopeProfileMode = isoMode;
            //fragmentedTargetedWorkflowParameters.MSParameters.IsoParameters.NumberOfPeaksToLeftForPenalty = 1;

            fragmentedTargetedWorkflowParameters.ChargeStateMin = processingParameters.ChargeStateMin;
            fragmentedTargetedWorkflowParameters.ChargeStateMax = processingParameters.ChargeStateMax;

            if (fragmentedTargetedWorkflowParameters.IsPic == EnumerationIsPic.IsPic)
            {
                //fragmentedTargetedWorkflowParameters.LCParameters.XYDataWriterPath = @"F:\ScottK\Results\XYDataWriter";
                fragmentedTargetedWorkflowParameters.LCParameters.XYDataWriterPath = xyDataPath;
            }
            else
            {
                //fragmentedTargetedWorkflowParameters.LCParameters.XYDataWriterPath = @"E:\ScottK\WorkingResults\XYDataWriter";
                fragmentedTargetedWorkflowParameters.LCParameters.XYDataWriterPath = xyDataPath;
            }

            fragmentedTargetedWorkflowParameters.IsotopeLabelingEfficiency = 1.1; //1.1 looks best for 1:1 ratio mixing
            fragmentedTargetedWorkflowParameters.MolarMixingFractionOfH = 0.5; //0.5 default for 1:1 mixing
            fragmentedTargetedWorkflowParameters.ChromNETTolerance = 1.28; //min is 0.08.  0.28 gets more scanStart and scanEnd do not matter
            //fragmentedTargetedWorkflowParameters.SummingMode = DeconTools.Backend.ProcessingTasks.ChromatogramProcessing.SummingModeEnum.SUMMINGMODE_STATIC;
            fragmentedTargetedWorkflowParameters.SummingMode = GlobalsWorkFlow.SummingModeEnum.SUMMINGMODE_STATIC;

            fragmentedTargetedWorkflowParameters.ChromGenTolerance = 10;
            
            fragmentedTargetedWorkflowParameters.NumChromPeaksAllowedDuringSelection = 20;
            fragmentedTargetedWorkflowParameters.MultipleHighQualityMatchesAreAllowed = true;
            fragmentedTargetedWorkflowParameters.ChromPeakSelectorMode = Globals.PeakSelectorMode.ClosestToTarget;

            //processingParameters
            fragmentedTargetedWorkflowParameters.FitScoreCuttoff = processingParameters.FitScoreCuttoff;
            fragmentedTargetedWorkflowParameters.CorrelationScoreCuttoff = processingParameters.CorrelationScoreCuttoff;
            fragmentedTargetedWorkflowParameters.LCParameters.LM_RsquaredCuttoff = processingParameters.LM_RsquaredCuttoff;//not implemented yet
            fragmentedTargetedWorkflowParameters.MSToleranceInPPM = processingParameters.MSToleranceInPPM;

            fragmentedTargetedWorkflowParameters.LCParameters.MovingAveragePoints = processingParameters.MovingAveragePoints;
            fragmentedTargetedWorkflowParameters.NumMSScansToSum = processingParameters.NumMSScansToSum;
            fragmentedTargetedWorkflowParameters.ChromSmootherNumPointsInSmooth = processingParameters.ChromSmootherNumPointsInSmooth;//9
            fragmentedTargetedWorkflowParameters.LCParameters.ParametersOmicsThreshold.SignalToShoulderCuttoff = processingParameters.SignalToShoulderCuttoff;//0


            fragmentedTargetedWorkflowParameters.LCParameters.ProcessLcSectionCorrelationObject = EnumerationChromatogramProcessing.SmoothSection;
            fragmentedTargetedWorkflowParameters.LCParameters.ProcessLcChromatogram = EnumerationChromatogramProcessing.ChromatogramLevelPrint;

            if (processingParameters.ProcessLcSectionCorrelationObjectEnum == "SmoothSection") fragmentedTargetedWorkflowParameters.LCParameters.ProcessLcSectionCorrelationObject = EnumerationChromatogramProcessing.SmoothSection;
            if (processingParameters.ProcessLcSectionCorrelationObjectEnum == "SmoothSectionWithAverage") fragmentedTargetedWorkflowParameters.LCParameters.ProcessLcSectionCorrelationObject = EnumerationChromatogramProcessing.SmoothSectionWithAverage;
            
            if (processingParameters.ProcessLcChromatogramEnum == "ChromatogramLevelPrint") fragmentedTargetedWorkflowParameters.LCParameters.ProcessLcChromatogram = EnumerationChromatogramProcessing.ChromatogramLevelPrint;
            if (processingParameters.ProcessLcChromatogramEnum == "ChromatogramLevelWithAverage") fragmentedTargetedWorkflowParameters.LCParameters.ProcessLcChromatogram = EnumerationChromatogramProcessing.ChromatogramLevelWithAverage;


            //fragmentedTargetedWorkflowParameters.FitScoreCuttoff = 0.15;
            //fragmentedTargetedWorkflowParameters.FitScoreCuttoff = 0.10;
            //fragmentedTargetedWorkflowParameters.NumberOfPeaksToLeftForPenalty = 1;
            //fragmentedTargetedWorkflowParameters.MSParameters.IsoParameters.NumberOfPeaksToLeftForPenalty = 1;
            //fragmentedTargetedWorkflowParameters.CorrelationScoreCuttoff = 0.50;
            fragmentedTargetedWorkflowParameters.ChromPeakSigmaThreshold = -1;//0 is a safe level.  -1 will get more


            //fragmentedTargetedWorkflowParameters.LCParameters.LM_RsquaredCuttoff = 0.85;
            //fragmentedTargetedWorkflowParameters.LCParameters.LM_RsquaredCuttoff = -5;
            //fragmentedTargetedWorkflowParameters.MSParameters.LM_RsquaredCuttoff = 0.85;//not implemented yet

            //fragmentedTargetedWorkflowParameters.MSToleranceInPPM = 7;

            //fragmentedTargetedWorkflowParameters.LCParameters.PointsPerShoulderSlope = 0.570;//should be at the 5 point/raw peak equilivalent
            //fragmentedTargetedWorkflowParameters.LCParameters.PointsPerShoulderIntercept = 4.91;//should be at the 5 point/raw peak equilivalent//this should remove the 5s

            //this is the way to go
            //a bonus of 1 points is added to bump the threshold above the average.  we also round up
            fragmentedTargetedWorkflowParameters.LCParameters.PointsPerShoulderSlope = 0.7806;//should be at the 3 point/raw peak equilivalent
            fragmentedTargetedWorkflowParameters.LCParameters.PointsPerShoulderIntercept = 0.6621;//should be at the 3 point/raw peak equilivalent// this should remove threes
            fragmentedTargetedWorkflowParameters.LCParameters.PointsPerShoulderBonusPoint = 0;//1 will remove all 3 point equivalent.  0 is right at the cuttoff and best for the unit tests

            fragmentedTargetedWorkflowParameters.LCParameters.PointsPerShoulder = fragmentedTargetedWorkflowParameters.LCParameters.CalculatePointsPerShoulderAsAFunctionOfSgPoints(fragmentedTargetedWorkflowParameters);
            
                                    
            //for 2 points
            //fragmentedTargetedWorkflowParameters.LCParameters.PointsPerShoulderSlope = 0;//should be at the 3 point/raw peak equilivalent
            //fragmentedTargetedWorkflowParameters.LCParameters.PointsPerShoulderIntercept = 0;//should be at the 3 point/raw peak equilivalent// this should remove threes
            //fragmentedTargetedWorkflowParameters.LCParameters.PointsPerShoulderBonusPoint = 4;//1 will remove all 3 point equivalent.
            //switch (fragmentedTargetedWorkflowParameters.DatasetType)
            //{
            //    case EnumerationDataset.SPINExactive:
            //        {
            //            fragmentedTargetedWorkflowParameters.LCParameters.MovingAveragePoints = 3;//SPIN//3 should be the max/  0 is better
            //            fragmentedTargetedWorkflowParameters.NumMSScansToSum = 13;
            //            fragmentedTargetedWorkflowParameters.ChromSmootherNumPointsInSmooth = 13;//9
            //            fragmentedTargetedWorkflowParameters.LCParameters.ParametersOmicsThreshold.SignalToShoulderCuttoff = 0;//SPIN
            //        }
            //        break;
            //    case EnumerationDataset.SPINExactiveMuddiman:
            //        {
            //            fragmentedTargetedWorkflowParameters.LCParameters.MovingAveragePoints = 3;//SPIN//3 should be the max/  0 is better
            //            fragmentedTargetedWorkflowParameters.NumMSScansToSum = 13;
            //            fragmentedTargetedWorkflowParameters.ChromSmootherNumPointsInSmooth = 13;//9
            //            fragmentedTargetedWorkflowParameters.LCParameters.ParametersOmicsThreshold.SignalToShoulderCuttoff = 0;//SPIN
            //        }
            //        break;
            //    case EnumerationDataset.SN123R8:
            //        {
            //            fragmentedTargetedWorkflowParameters.LCParameters.MovingAveragePoints = 3;
            //            fragmentedTargetedWorkflowParameters.NumMSScansToSum = 9;
            //            fragmentedTargetedWorkflowParameters.ChromSmootherNumPointsInSmooth = 9;//9
            //            fragmentedTargetedWorkflowParameters.LCParameters.ParametersOmicsThreshold.SignalToShoulderCuttoff = -1;//DH
            //        }
            //        break;
            //    case EnumerationDataset.Diabetes:
            //        {
            //            fragmentedTargetedWorkflowParameters.LCParameters.MovingAveragePoints = 3;//this is hard to justify
            //            fragmentedTargetedWorkflowParameters.NumMSScansToSum = 9;//9 for testing
            //            fragmentedTargetedWorkflowParameters.ChromSmootherNumPointsInSmooth = 9;//9 for testing
            //            fragmentedTargetedWorkflowParameters.LCParameters.ParametersOmicsThreshold.SignalToShoulderCuttoff = -1;//DH

            //        }
            //        break;
            //    default:
            //        {
            //            fragmentedTargetedWorkflowParameters.NumMSScansToSum = 9;
            //            fragmentedTargetedWorkflowParameters.ChromSmootherNumPointsInSmooth = 9;//9
            //            fragmentedTargetedWorkflowParameters.LCParameters.ParametersOmicsThreshold.SignalToShoulderCuttoff = -1;//DH
            //        }
            //        break;
            //}

            //parents = target + fragment while children is target - fragment
            fragmentedTargetedWorkflowParameters.GenerationDirection = EnumerationParentOrChild.ParentsAndChildren;

            fragmentedTargetedWorkflowParameters.LCParameters.ParametersChromGenerator.ChromeGeneratorMode = Globals.ChromatogramGeneratorMode.MOST_ABUNDANT_PEAK;
            
            fragmentedTargetedWorkflowParameters.LCParameters.ParametersChromGenerator.ChromToleranceInPPM = fragmentedTargetedWorkflowParameters.ChromGenTolerance;
            fragmentedTargetedWorkflowParameters.LCParameters.ParametersChromGenerator.ErrorUnit = Globals.ToleranceUnit.PPM;
            fragmentedTargetedWorkflowParameters.LCParameters.ParametersSavitskyGolay.PointsForSmoothing = fragmentedTargetedWorkflowParameters.ChromSmootherNumPointsInSmooth;
            fragmentedTargetedWorkflowParameters.LCParameters.ParametersSavitskyGolay.PolynomialOrder = 2;
            fragmentedTargetedWorkflowParameters.LCParameters.ParametersOmicsPeakCentroid.FWHMPeakFitType = PeakFitType.Parabola;

            Console.WriteLine("executorParametersSet");

            return executorParameters;
        }

        
    }
}
