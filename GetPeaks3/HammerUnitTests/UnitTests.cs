using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaksDllLite.DataFIFO;
using HammerPeakDetector.Enumerations;
using HammerPeakDetector.Objects;
using IQGlyQ;
using IQGlyQ.Enumerations;
using IQGlyQ.Objects;
using IQGlyQ.Processors;
using IQGlyQ.Results;
using IQGlyQ.UnitTesting;
using IQ_X64.Backend.Core;
using IQ_X64.Backend.ProcessingTasks.TargetedFeatureFinders;
using IQ_X64.Backend.ProcessingTasks.TheorFeatureGenerator;
using NUnit.Framework;
using PNNLOmics.Data;
using Run64.Backend;
using Run64.Backend.Core;
using Run64.Backend.Runs;
using IQ_X64.Workflows.Core;
using XYData = Run64.Backend.Data.XYData;
using GetPeaksDllLite.Functions;

namespace HammerUnitTests
{
    public class UnitTests
    {
        #region global variables

        private FragmentIQTarget possibleFragmentTarget { get; set; }
        private FragmentResultsObjectHolderIq targetResult { get; set; }
        private FragmentedTargetedWorkflowParametersIQ _workflowParameters { get; set; }
        private Run runIn { get; set; }
        private IqGlyQResult iQresult { get; set; }
        private ProcessorMassSpectra _msProcessor { get; set; }
        private Tuple<string, string> errorlog { get; set; }
        private string printString { get; set; }
        private ITheorFeatureGenerator _theorFeatureGen { get; set; }
        private IterativeTFF _msfeatureFinder { get; set; }
        private TaskIQ _fitScoreCalc { get; set; }
        private ProcessorChromatogram _lcProcessor { get; set; }

        #endregion

        private void PopulateGlobalVariables()
        {
            bool isUnitTest = true;
            EnumerationDataset thisDataset = EnumerationDataset.Diabetes;
            EnumerationIsPic isPic = EnumerationIsPic.IsNotPic;

            double deltaMassCalibrationMZ = 0;
            double deltaMassCalibrationMono = 0;
            bool toMassCalibrate = false;

            FragmentIQTarget fragmentIqTarget = possibleFragmentTarget;
            FragmentResultsObjectHolderIq fragmentResultsObjectHolderIq = targetResult;
            FragmentedTargetedWorkflowParametersIQ fragmentedTargetedWorkflowParametersIq = _workflowParameters;
            Run run = runIn;
            IqGlyQResult iqGlyQResult = iQresult;
            ProcessorMassSpectra msProcessor = _msProcessor;
            Tuple<string, string> tuple = errorlog;
            string s = printString;
            ITheorFeatureGenerator joshTheorFeatureGenerator = _theorFeatureGen;
            IterativeTFF msfeatureFinder = _msfeatureFinder;
            TaskIQ isotopicProfileFitScoreCalculator = _fitScoreCalc;
            ProcessorChromatogram processorChromatogram = _lcProcessor;
            IQGlyQTestingUtilities.SetupTargetAndEnginesForOneTargetRef(ref fragmentIqTarget, ref fragmentResultsObjectHolderIq, ref fragmentedTargetedWorkflowParametersIq, ref run, ref iqGlyQResult, ref msProcessor, ref tuple, ref s, ref joshTheorFeatureGenerator, ref msfeatureFinder, ref isotopicProfileFitScoreCalculator, ref processorChromatogram, isUnitTest, thisDataset, isPic);

            possibleFragmentTarget = fragmentIqTarget;
            targetResult = fragmentResultsObjectHolderIq;
            _workflowParameters = fragmentedTargetedWorkflowParametersIq;
            runIn = run;
            iQresult = iqGlyQResult;
            _msProcessor = msProcessor;
            errorlog = tuple;
            printString = s;
            _theorFeatureGen = joshTheorFeatureGenerator;
            _msfeatureFinder = msfeatureFinder;
            _fitScoreCalc = isotopicProfileFitScoreCalculator;
            _lcProcessor = processorChromatogram;

        }
        
        [Test]
        public void TestProcessorMSHammer()
        {
            PopulateGlobalVariables();

            string testFile = "";
            testFile = @"E:\ScottK\GetPeaks Data\Diabetes_LC\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw";
            //testFile = @"E:\PNNL Data\2012_09_05 SPIN Q-TOF\Gly08_SQTOF_SP02_3X_C1_12_HPIF20Torr_LPRF96_T160_6Sept12.d";

            //testFile = @"E:\PNNL Data\2012_09_05 SPIN Q-TOF\Gly08_SQTOF_SL26_100_C2_13_HPIF30Torr_LPRF96_T160_18Sept12.d";//work
            //testFile = @"L:\PNNL Files\PNNL Data for Tests\Gly08_SQTOF_SL26_100_C2_13_HPIF30Torr_LPRF96_T160_18Sept12.d";//home
            //testFile = massSpecFilePath;

            string metricsFolder = @"D:\HammerPeaks";
            bool printMetrics = false;
            RunFactory factor = new RunFactory();
            Run runIn = factor.CreateRun(Globals.MSFileType.Finnigan, testFile);

            ScanObject scanInfo = new ScanObject(2615, 2684);
            //possibleFragmentTarget.ScanInfo.Start = 1648;
            //possibleFragmentTarget.ScanInfo.Stop = 1674;
            scanInfo.Max = runIn.MaxLCScan;
            scanInfo.Min = runIn.MinLCScan;
            scanInfo.ScansToSum = 5;
            scanInfo.Buffer = 9 * 2;

            runIn.ScanSetCollection.Create(runIn, scanInfo.Start, scanInfo.Stop, scanInfo.ScansToSum, 1, false);

            IqResult iQresult = new IqResult(new IqTargetBasic());

            //MSGenerator _msGenerator = _workflowParameters.MSParameters.Engine_msGenerator;

            iQresult.LCScanSetSelected = Utiliites.ScanSetFromStartStop(runIn, scanInfo);



            string rawPath = _workflowParameters.LCParameters.XYDataWriterPath + @"\MS_0_RawPreSmoothing.txt";

            DataXYDataWriter writer = new DataXYDataWriter();


            //0.  get spectra.  raw untouched summed data
            iQresult.IqResultDetail.MassSpectrum = _msProcessor.DeconMSGeneratorWrapper(runIn, iQresult.LCScanSetSelected);
            List<PNNLOmics.Data.XYData> convertedMassSpectrum = ConvertXYData.DeconXYDataToOmicsXYData(iQresult.IqResultDetail.MassSpectrum);


            writer.WriteDeconXYDataDeconTools(iQresult.IqResultDetail.MassSpectrum, rawPath);
            Assert.AreEqual(56963, iQresult.IqResultDetail.MassSpectrum.Xvalues.Length);

            List<ProcessedPeak> centroidOnly = _msProcessor.Execute(convertedMassSpectrum, EnumerationMassSpectraProcessing.OmicsCentroid_Only);

            Assert.AreEqual(4286, centroidOnly.Count);

            HammerPeakDetector.Parameters.HammerThresholdParameters hammerParameters = new HammerPeakDetector.Parameters.HammerThresholdParameters();


            int mode = 0; //cluster only.  pvalues will do the thresholding
            mode = 1; //cluster and threshold. thresholde by 0 or some sigma above moving average
            mode = 2; //cluster only with optimize with 1 iteration.  
            mode = 3; //cluster and threshold with optimize with 1 iteration. 
            mode = 4; //cluster optimize with many iterations. 
            SetHammerParameters(hammerParameters, mode);


            List<PNNLOmics.Data.Peak> toHammer = new List<PNNLOmics.Data.Peak>();
            foreach (ProcessedPeak processedPeak in centroidOnly)
            {
                //PNNLOmics.Data.Peak peak = new PNNLOmics.Data.Peak();

                //Console.WriteLine(

                //    processedPeak.Height.ToString() + "," +
                //    processedPeak.XValue.ToString() + "," +
                //    processedPeak.Width.ToString() + "," +
                //    processedPeak.LeftWidth.ToString() + "," +
                //    processedPeak.RightWidth.ToString() + "," +
                //    processedPeak.LocalSignalToNoise.ToString() + "," +
                //    processedPeak.Background.ToString());
                toHammer.Add(processedPeak);
            }

            List<PNNLOmics.Data.Peak> toHammerV2 = LoadPeaksForHammer.Load();

            for (int i = 0; i < toHammer.Count; i++)
            {
                Assert.AreEqual(toHammer[i].XValue, toHammerV2[i].XValue, 0.01);
            }

            HammerPeakDetector.Objects.HammerThresholdResults resultsOptimize = new HammerThresholdResults(hammerParameters);
            List<ProcessedPeak> results = HammerPeakDetector.Hammer.Detect(toHammer, hammerParameters, metricsFolder, printMetrics, out resultsOptimize);

            Assert.AreEqual(2090, results.Count);
            //Assert.AreEqual(3640, results.Count);
        }


        private static void SetHammerParameters(HammerPeakDetector.Parameters.HammerThresholdParameters hammerParameters, int mode)
        {
            //set parameters here
            hammerParameters.MinChargeState = 1; //1 is default
            hammerParameters.MaxChargeState = 5; //5 is default
            hammerParameters.MinimumSizeOfRegion = 30; //30 is default
            hammerParameters.SeedClusterSpacingCenter = 1.00235; //1.00235 is default
            hammerParameters.SeedMassToleranceDa = 0.01; //0.018 is default

            switch (mode)
            {
                case 0:
                    {
                        //hammerParameters.OptimizeOrDefaultChoise = OptimizeOrDefaultMassSpacing.Default;
                        hammerParameters.FilteringMethod = HammerFilteringMethod.Cluster;
                    }
                    break;
                case 1:
                    {
                        //hammerParameters.OptimizeOrDefaultChoise = OptimizeOrDefaultMassSpacing.Default;
                        hammerParameters.FilteringMethod = HammerFilteringMethod.Threshold;
                        hammerParameters.ThresholdSigmaMultiplier = 0; //0 is parasmeter free and uses the average
                    }
                    break;
                case 2:
                    {
                        //hammerParameters.OptimizeOrDefaultChoise = OptimizeOrDefaultMassSpacing.Optimize;
                        hammerParameters.FilteringMethod = HammerFilteringMethod.Cluster;
                        hammerParameters.Iterations = 1;
                    }
                    break;
                case 3:
                    {
                        //hammerParameters.OptimizeOrDefaultChoise = OptimizeOrDefaultMassSpacing.Optimize;
                        hammerParameters.FilteringMethod = HammerFilteringMethod.Threshold;
                        hammerParameters.ThresholdSigmaMultiplier = 0; //0 is parasmeter free and uses the average
                        hammerParameters.Iterations = 1;
                    }
                    break;
                case 4:
                    {
                        //hammerParameters.OptimizeOrDefaultChoise = OptimizeOrDefaultMassSpacing.Optimize;
                        hammerParameters.FilteringMethod = HammerFilteringMethod.Cluster;
                        hammerParameters.Iterations = -1;
                    }
                    break;
            }
        }

    }
}
