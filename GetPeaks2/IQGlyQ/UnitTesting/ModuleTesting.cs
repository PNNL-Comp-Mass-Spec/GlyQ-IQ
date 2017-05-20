using System;
using System.Collections.Generic;
using System.Linq;
using GetPeaksDllLite.DataFIFO;
using GetPeaksDllLite.Functions;
using GetPeaksDllLite.PNNLOmics_Modules;
using GetPeaksDllLite.Switches;
using HammerPeakDetector.Enumerations;
using HammerPeakDetector.Objects;
using IQ.Backend.Core;
using IQ.Backend.ProcessingTasks.ChromatogramProcessing;
using IQ.Backend.ProcessingTasks.LCGenerators;
using IQ.Backend.ProcessingTasks.MSGenerators;
using IQ.Backend.ProcessingTasks.PeakDetectors;
using IQ.Backend.ProcessingTasks.Smoothers;
using IQ.Backend.ProcessingTasks.TargetedFeatureFinders;
using IQ.Backend.ProcessingTasks.TheorFeatureGenerator;
using IQ.Workflows;
using IQ.Workflows.Core;
using IQ.Workflows.Core.ChromPeakSelection;
using IQ.Workflows.Utilities;
using IQGlyQ.Enumerations;
using IQGlyQ.Objects.EverythingIsotope;
using IQGlyQ.Processors;
using IQGlyQ.Results;
using IQGlyQ.TargetGenerators;
using IQGlyQ.Tasks;
using IQGlyQ.Tasks.FitScores;
using NUnit.Framework;
using PNNLOmics.Algorithms.PeakDetection;
using PNNLOmics.Algorithms.Solvers.LevenburgMarquadt;
using PNNLOmics.Data;
using PNNLOmics.Data.Peaks;
using Run32.Backend;
using Run32.Backend.Core;
using Run32.Backend.Runs;
using XYData = PNNLOmics.Data.XYData;
using IQGlyQ.Objects;
using IQGlyQ.Functions;
using System.Diagnostics;
using Peak = Run32.Backend.Core.Peak;

namespace IQGlyQ.UnitTesting
{
    //all these must pass
    
    

    class ModuleTesting
    {
        private string massSpecFilePath = @"E:\PNNL_Data\2012_09_05 SPIN Q-TOF\Gly08_SQTOF_SL26_100_C2_13_HPIF30Torr_LPRF96_T160_18Sept12.d";//work
        //private string massSpecFilePath = @"L:\PNNL Files\PNNL Data for Tests\Gly08_SQTOF_SL26_100_C2_13_HPIF30Torr_LPRF96_T160_18Sept12.d";//home
        
        [Test]
        public void CreateCorrelationObject()
        {
            PopulateGlobalVariables();

            double chromToleranceInPPM = _workflowParameters.ChromGenTolerance;
            double MinRelativeIntensityForChromCorrelator = _workflowParameters.MinRelativeIntensityForChromCorrelator;

            //TEST CODE
            EnumerationError errorCode;
            Tuple<string, string> errorLog = errorlog;
            ProcessorChromatogram processorChromatogram = _lcProcessor;
            CorrelationObject targetPeak = Utiliites.CreateCorrelationObject(possibleFragmentTarget.TheorIsotopicProfile, targetResult.ScanBoundsInfo, runIn, ref errorLog, out errorCode, ref processorChromatogram, _workflowParameters);

            Assert.AreEqual(16, targetPeak.AcceptableChromList.Count);
            Assert.AreEqual(true, targetPeak.AcceptableChromList[0]);
            Assert.AreEqual(true, targetPeak.AreChromDataOK);
            Assert.AreEqual(862.8122251245054d, targetPeak.BaseMZValue);
            Assert.AreEqual(1, targetPeak.IndexMostAbundantPeak);
            Assert.AreEqual(true, targetPeak.IsosPeakListIsOK);
            Assert.AreEqual(16, targetPeak.PeakChromXYData.Count);
            Assert.AreEqual(1667, targetPeak.PeakChromXYData[0].Xvalues[6]);
            //Assert.AreEqual(15541531.181818185d, targetPeak.PeakChromXYData[0].Yvalues[6]);
            Assert.AreEqual(15541531.181818185d, targetPeak.PeakChromXYData[0].Yvalues[6]);
        }

        [Test]
        public void CreateCorrelationObjectAndtestConstancty()
        {
            PopulateGlobalVariables();

            double chromToleranceInPPM = _workflowParameters.ChromGenTolerance;
            double MinRelativeIntensityForChromCorrelator = _workflowParameters.MinRelativeIntensityForChromCorrelator;

            //TEST CODE
            EnumerationError errorCode;
            Tuple<string, string> errorLog = errorlog;
            ProcessorChromatogram processorChromatogram = _lcProcessor;
            CorrelationObject targetPeak = Utiliites.CreateCorrelationObject(possibleFragmentTarget.TheorIsotopicProfile, targetResult.ScanBoundsInfo, runIn, ref errorLog, out errorCode, ref processorChromatogram, _workflowParameters);

            Assert.AreEqual(16, targetPeak.AcceptableChromList.Count);
            Assert.AreEqual(true, targetPeak.AcceptableChromList[0]);
            Assert.AreEqual(true, targetPeak.AreChromDataOK);
            Assert.AreEqual(862.8122251245054d, targetPeak.BaseMZValue);
            Assert.AreEqual(1, targetPeak.IndexMostAbundantPeak);
            Assert.AreEqual(true, targetPeak.IsosPeakListIsOK);
            Assert.AreEqual(16, targetPeak.PeakChromXYData.Count);
            Assert.AreEqual(1667, targetPeak.PeakChromXYData[0].Xvalues[6]);
            //Assert.AreEqual(15541531.181818185d, targetPeak.PeakChromXYData[0].Yvalues[6]);
            Assert.AreEqual(15541531.181818185d, targetPeak.PeakChromXYData[0].Yvalues[6]);

            List<XYData> targetEic = Utiliites.PullBestEIC(targetPeak, ref errorLog);

            Assert.AreEqual(9, targetEic.Count);


            //lc way
            double xValue = possibleFragmentTarget.TheorIsotopicProfile.Peaklist[1].XValue;
            //DeconTools.Backend.XYData deconBufferedChromatogram = _workflowParameters.LCParameters.Engine_PeakChromGenerator.GenerateChromatogram(runIn, targetResult.ScanBoundsInfo.Min, targetResult.ScanBoundsInfo.Max, xValue, _workflowParameters.LCParameters.ParametersChromGenerator.ChromToleranceInPPM, Globals.ToleranceUnit.PPM);
            ScanObject ScanBoundsInfoMaxMin = new ScanObject(targetResult.ScanBoundsInfo.Min, targetResult.ScanBoundsInfo.Max);
            //DeconTools.Backend.XYData deconBufferedChromatogram = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(runIn, xValue, ScanBoundsInfoMaxMin, _workflowParameters.LCParameters.ParametersChromGenerator.ChromToleranceInPPM);//old
            Run32.Backend.Data.XYData deconBufferedChromatogram = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(runIn, xValue, ScanBoundsInfoMaxMin);

            Assert.AreEqual(897, deconBufferedChromatogram.Xvalues.Length);

            //2.  convert data
            List<PNNLOmics.Data.XYData> omcisBufferedChromatogram = ConvertXYData.DeconXYDataToOmicsXYData(deconBufferedChromatogram);

            //3.  process data
            List<ProcessedPeak> newMethodInfinity = processorChromatogram.Execute(omcisBufferedChromatogram, targetResult.ScanBoundsInfo, _workflowParameters.LCParameters.ProcessLcSectionCorrelationObject);

            List<XYData> simpleEIC = ConvertProcessedPeakToXYData.ConvertPoints(newMethodInfinity);

            Assert.AreEqual(9, simpleEIC.Count);

            for (int i = 0; i < targetEic.Count;i++ )
            {
                Assert.AreEqual(targetEic[i].Y, simpleEIC[i].Y);

            }

            //front LC processing
            
            //PeakChromatogramGenerator ChromGen = new PeakChromatogramGenerator(_workflowParameters.LCParameters.ParametersChromGenerator.ChromToleranceInPPM);

            //DeconTools.Backend.XYData upfrontEIC = ChromGen.GenerateChromatogram(runIn, possibleFragmentTarget.TheorIsotopicProfile);
            //Run32.Backend.Data.XYData upfrontEIC = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(runIn, possibleFragmentTarget.TheorIsotopicProfile, possibleFragmentTarget.ElutionTimeTheor);
            double massToExtract = Utiliites.GetMassToExtractFromIsotopeProfile(possibleFragmentTarget.TheorIsotopicProfile, _workflowParameters);

            Run32.Backend.Data.XYData upfrontEIC = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(runIn, massToExtract);
            

            List<PNNLOmics.Data.XYData> convertedXY = ConvertXYData.DeconXYDataToOmicsXYData(upfrontEIC);
            List<ProcessedPeak> smoothedSimple = processorChromatogram.Execute(convertedXY, targetResult.ScanBoundsInfo, EnumerationChromatogramProcessing.SmoothingOnly);

            List<ProcessedPeak> newRangeSimple = ChangeRange.ClipProcessedPeakToScanRange(smoothedSimple, targetResult.ScanBoundsInfo.Start, targetResult.ScanBoundsInfo.Stop);

            Assert.AreEqual(9, newRangeSimple.Count);
            //List<Peak> ChromPeakList = processorChromatogram.Execute(upfrontEIC, targetResult.ScanBoundsInfo, _workflowParameters.LCParameters.ProcessLcChromatogram);

            for (int i = 0; i < targetEic.Count; i++)
            {
                Assert.AreEqual(targetEic[i].Y, newRangeSimple[i].Height);

            }
        }

        //[Test]
        //public void GenerateCandidateList1661()
        //{
        //    PopulateGlobalVariables();

            
        //    //constants
        //    ChromPeakQualityData possibleFragmentPeakQuality = new ChromPeakQualityData(new ChromPeak(1660.8837478792, 18197090f, 25.5049114f, 4.605931f));
        //    possibleFragmentPeakQuality.FitScore = 0.0609229743850591;
        //    possibleFragmentPeakQuality.ScanLc = 1661;
        //    possibleFragmentPeakQuality.IsotopicProfile = iQresult.Target.TheorIsotopicProfile;
        //    //possibleFragmentPeakQuality.IsotopicProfile = _theorFeatureGen.GenerateTheorProfile(possibleFragmentTarget.EmpiricalFormula,2);
        //    possibleFragmentPeakQuality.IsotopicProfile = Utiliites.GenerateCombinedIsotopicProfile(iQresult, 1, 1);


        //    List<FragmentIQTarget> Fragments = new List<FragmentIQTarget>();
        //    Fragments = _workflowParameters.FragmentsIQ;

        //    Run run = runIn;
        //    List<FragmentIQTarget> passFragments = Fragments;
        //    FragmentedTargetedWorkflowParametersIQ passWorkflowParameters = _workflowParameters;

        //    //Test CODE HERE
        //    //FragmentIQTarget chargedParentsFromAllFragmentsToSearchForMass = Utiliites.GenerateListOfCandidateParentsTestIQ(run, iQresult, possibleFragmentPeakQuality, possibleFragmentTarget.StartScan, possibleFragmentTarget.StopScan, ref passFragments, ref passWorkflowParameters, ref _theorFeatureGen, ref _peakChromGen, ref _smoother, ref errorlog);
        //    ITheorFeatureGenerator joshTheorFeatureGenerator = _theorFeatureGen;
        //    Tuple<string, string> errorLog = errorlog;
        //    ProcessorChromatogram processorChromatogram = _lcProcessor;
        //    FragmentIQTarget chargedParentsFromAllFragmentsToSearchForMass = Utiliites.GenerateListOfCandidateParentsTestIQ(run, iQresult, possibleFragmentPeakQuality, targetResult.ScanBoundsInfo, EnumerationParentOrChild.ParentsOnly,  ref passFragments, ref passWorkflowParameters, ref joshTheorFeatureGenerator, ref errorLog, ref processorChromatogram);

        //    //Assert.AreEqual(2, chargedParentsFromAllFragmentsToSearchForMass.GetChildCount());
        //    Assert.AreEqual(1, chargedParentsFromAllFragmentsToSearchForMass.GetChildCount());
        //}

        [Test]
        public void GenerateCandidateList1706()
        {
            PopulateGlobalVariables();

            //constants

            iQresult.Target.ChargeState = 2;

            targetResult.ScanBoundsInfo.Start = 1667;
            targetResult.ScanBoundsInfo.Stop = 1745;
            int ScanLCTarget = 1706;

            //possibleFragmentTarget = new FragmentIQTarget();
            //possibleFragmentTarget.ScanInfo.Start = scans.Start;
            //possibleFragmentTarget.ScanInfo.Stop = scans.Stop;
            possibleFragmentTarget.ScanLCTarget = ScanLCTarget;

            ChromPeakQualityData possibleFragmentPeakQuality = new ChromPeakQualityData(new ChromPeak(1707.20070542704, 5623193.5f, 77.5042343f, 26.4798851f));
            possibleFragmentPeakQuality.FitScore = 0.0603449505835566;
            possibleFragmentPeakQuality.ScanLc = 1706;
            possibleFragmentPeakQuality.IsotopicProfile = iQresult.Target.TheorIsotopicProfile;
            possibleFragmentPeakQuality.IsotopicProfile = Utiliites.GenerateCombinedIsotopicProfile(iQresult, 1, 1);


            List<FragmentIQTarget> Fragments = new List<FragmentIQTarget>();
            Fragments = _workflowParameters.FragmentsIq;

            Run run = runIn;
            List<FragmentIQTarget> passFragments = Fragments;
            FragmentedTargetedWorkflowParametersIQ passWorkflowParameters = _workflowParameters;

            //FragmentIQTarget chargedParentsFromAllFragmentsToSearchForMass = Utiliites.GenerateListOfCandidateParentsTestIQ(run, iQresult, possibleFragmentPeakQuality, scans.Start, scans.Stop, ref passFragments, ref passWorkflowParameters, ref _theorFeatureGen, ref _peakChromGen, ref _smoother, ref errorlog);
            ITheorFeatureGenerator joshTheorFeatureGenerator = new JoshTheorFeatureGenerator();
            IGenerateIsotopeProfile TheorFeatureGenV2 = new IsotopeProfileBlended(new ParametersBlendedIsotope(new ParametersSimpleIsotope(joshTheorFeatureGenerator)));
            Tuple<string, string> errorLog = errorlog;
            ProcessorChromatogram processorChromatogram = _lcProcessor;
            TheoreticalIsotopicProfileWrapper localMonster = Monster;
            //FragmentIQTarget chargedParentsFromAllFragmentsToSearchForMass = Utiliites.GenerateListOfCandidateParentsTestIQ(run, iQresult, possibleFragmentPeakQuality, targetResult.ScanBoundsInfo, EnumerationParentOrChild.ParentsOnly, ref passWorkflowParameters, ref joshTheorFeatureGenerator, ref errorLog, ref processorChromatogram);
            FragmentIQTarget chargedParentsFromAllFragmentsToSearchForMass = Utiliites.GenerateListOfCandidateParentsTestIQ(run, iQresult, possibleFragmentPeakQuality, targetResult.ScanBoundsInfo, EnumerationParentOrChild.ParentsOnly, ref passWorkflowParameters, ref TheorFeatureGenV2, ref errorLog, ref processorChromatogram, ref localMonster);

            Assert.AreEqual(3, chargedParentsFromAllFragmentsToSearchForMass.GetChildCount());
        }

        [Test]
        public void GenerateCandidateList1790()
        {
            PopulateGlobalVariables();


            //constants
            targetResult.ScanBoundsInfo.Start = 1769;
            targetResult.ScanBoundsInfo.Stop = 1811;


            ChromPeakQualityData possibleFragmentPeakQuality = new ChromPeakQualityData(new ChromPeak(1791.0067138671875, 218052.828f, 19.6730652f, 1.0f));
            possibleFragmentPeakQuality.FitScore = 0.061945772099080795;
            possibleFragmentPeakQuality.ScanLc = 1790;
            //iQresult.Target.TheorIsotopicProfile = _theorFeatureGen.GenerateTheorProfile("C64H110N2O51", 2);//8200
            //ITheorFeatureGenerator theorFeatureGenerator = _theorFeatureGen;
            ITheorFeatureGenerator joshTheorFeatureGenerator = new JoshTheorFeatureGenerator();
            IGenerateIsotopeProfile TheorFeatureGenV2 = new IsotopeProfileBlended(new ParametersBlendedIsotope(new ParametersSimpleIsotope(joshTheorFeatureGenerator)));


            iQresult.Target.TheorIsotopicProfile = TheorFeatureGenV2.Generator("C64H110N2O51", 2);//8200)
            //iQresult.Target.TheorIsotopicProfile = TheoreticalIsotopicProfileWrapper.GenerateSimpleOld(ref theorFeatureGenerator,"C64H110N2O51", 2);//8200
            possibleFragmentPeakQuality.IsotopicProfile = iQresult.Target.TheorIsotopicProfile;
            //possibleFragmentPeakQuality.IsotopicProfile = _theorFeatureGen.GenerateTheorProfile(possibleFragmentTarget.EmpiricalFormula,2);
            possibleFragmentPeakQuality.IsotopicProfile = Utiliites.GenerateCombinedIsotopicProfile(iQresult, 1, 1);


            List<FragmentIQTarget> Fragments = new List<FragmentIQTarget>();
            Fragments = _workflowParameters.FragmentsIq;

            Run run = runIn;
            List<FragmentIQTarget> passFragments = Fragments;
            FragmentedTargetedWorkflowParametersIQ passWorkflowParameters = _workflowParameters;

            //Test CODE HERE
            //FragmentIQTarget chargedParentsFromAllFragmentsToSearchForMass = Utiliites.GenerateListOfCandidateParentsTestIQ(run, iQresult, possibleFragmentPeakQuality, possibleFragmentTarget.StartScan, possibleFragmentTarget.StopScan, ref passFragments, ref passWorkflowParameters, ref _theorFeatureGen, ref _peakChromGen, ref _smoother, ref errorlog);
            //ITheorFeatureGenerator joshTheorFeatureGenerator = _theorFeatureGen;
            //IGenerateIsotopeProfile TheorFeatureGenV2 = _TheorFeatureGenV2;
            Tuple<string, string> errorLog = errorlog;
            ProcessorChromatogram processorChromatogram = _lcProcessor;
            TheoreticalIsotopicProfileWrapper localMonster = Monster;
            FragmentIQTarget chargedParentsFromAllFragmentsToSearchForMass = Utiliites.GenerateListOfCandidateParentsTestIQ(run, iQresult, possibleFragmentPeakQuality, targetResult.ScanBoundsInfo, EnumerationParentOrChild.ParentsOnly, ref passWorkflowParameters, ref TheorFeatureGenV2, ref errorLog, ref processorChromatogram, ref localMonster);

            Assert.AreEqual(2, chargedParentsFromAllFragmentsToSearchForMass.GetChildCount());
            //Assert.AreEqual(1, chargedParentsFromAllFragmentsToSearchForMass.GetChildCount());
        }

        [Test]
        public void TestChromatogram()
        {
            PopulateGlobalVariables();

            double chromToleranceInPPM = _workflowParameters.ChromGenTolerance;
            double MinRelativeIntensityForChromCorrelator = _workflowParameters.MinRelativeIntensityForChromCorrelator;

            //ITheorFeatureGenerator TheorFeatureGen = _theorFeatureGen;
            IqGlyQResult result = iQresult;
            //result.Target.TheorIsotopicProfile = TheorFeatureGen.GenerateTheorProfile(result.Target.EmpiricalFormula, result.Target.ChargeState);
            //result.Target.TheorIsotopicProfile = TheoreticalIsotopicProfileWrapper.GenerateSimpleOld(ref TheorFeatureGen, result.Target.EmpiricalFormula, result.Target.ChargeState);
            result.Target.TheorIsotopicProfile = _TheorFeatureGenV2.Generator(result.Target.EmpiricalFormula, result.Target.ChargeState);

            //old
            //result.Target.TheorIsotopicProfile = Utiliites.GenerateCombinedIsotopicProfile(result, 1, 1);
            List<int> offsets = new int[] { 1 }.ToList();
            List<float> ratios = new float[] { 1 }.ToList();
            
            IsotopeProfileBlended _TheorFeatureGenV2_M = new IsotopeProfileBlended(new ParametersBlendedIsotope(new ParametersSimpleIsotope(new JoshTheorFeatureGenerator(Globals.LabellingType.NONE, 0.1))));
            result.Target.TheorIsotopicProfile = _TheorFeatureGenV2_M.Generator(result.Target.EmpiricalFormula, result.Target.ChargeState, ref offsets, ref ratios);
            result.Target.TheorIsotopicProfile.RefreshAlternatePeakIntenstiesFromPeakList();

            ChromPeakDetector ChromPeakDetector = new ChromPeakDetector(_workflowParameters.ChromPeakDetectorPeakBR, _workflowParameters.ChromPeakDetectorSigNoise);
            ChromPeakAnalyzer ChromPeakAnalyzer = new IQ.Workflows.Core.ChromPeakSelection.ChromPeakAnalyzer(_workflowParameters);

            PNNLOmics.Algorithms.SpectralProcessing.SavitzkyGolaySmoother ChromSmoother = _workflowParameters.LCParameters.Engine_Smoother;

            //TEST CODE
            Run32.Backend.Data.XYData unWrappedDecon = _workflowParameters.LCParameters.Engine_PeakChromGenerator.GenerateChromatogram(runIn, result.Target.TheorIsotopicProfile, result.Target.ElutionTimeTheor);
            
            //old
            //result.IqResultDetail.Chromatogram  = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(runIn, possibleFragmentTarget.TheorIsotopicProfile, possibleFragmentTarget.ElutionTimeTheor);
            double massToExtract = Utiliites.GetMassToExtractFromIsotopeProfile(result.Target.TheorIsotopicProfile, _workflowParameters);
            result.IqResultDetail.Chromatogram = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(runIn, massToExtract);


            Assert.AreEqual(unWrappedDecon.Xvalues.Length, result.IqResultDetail.Chromatogram.Xvalues.Length);
            Assert.AreEqual(unWrappedDecon.Xvalues[500], result.IqResultDetail.Chromatogram.Xvalues[500]);
            Assert.AreEqual(unWrappedDecon.Xvalues[500], result.IqResultDetail.Chromatogram.Xvalues[500]);
            Assert.AreEqual(unWrappedDecon.Yvalues[501], result.IqResultDetail.Chromatogram.Yvalues[501]);
            Assert.AreEqual(unWrappedDecon.Yvalues[501], result.IqResultDetail.Chromatogram.Yvalues[501]);

            //omics
            var convertedData = ConvertXYData.DeconXYDataToOmicsXYData(result.IqResultDetail.Chromatogram);
            List<XYData> omicsSmoothedChromatogram = ChromSmoother.Smooth(convertedData);
            //end omics

            //decon
            IQ.Backend.ProcessingTasks.Smoothers.SavitzkyGolaySmoother ChromSmootherDecon =
                new SavitzkyGolaySmoother(_workflowParameters.LCParameters.ParametersSavitskyGolay.PointsForSmoothing,
                                          _workflowParameters.LCParameters.ParametersSavitskyGolay.PolynomialOrder,
                                          _workflowParameters.LCParameters.ParametersSavitskyGolay.AllowNegativeValues);
            result.IqResultDetail.Chromatogram = ChromSmootherDecon.Smooth(result.IqResultDetail.Chromatogram);
            //end decon

            Assert.AreEqual(1722.6075472690109d, result.Target.MonoMassTheor);
            Assert.AreEqual(897, result.IqResultDetail.Chromatogram.Xvalues.Length);
            Assert.AreEqual(565, result.IqResultDetail.Chromatogram.Xvalues[140]);
            Assert.AreEqual(7749.2480849228923d, result.IqResultDetail.Chromatogram.Yvalues[140]);
            Assert.AreEqual(589.0d, result.IqResultDetail.Chromatogram.Xvalues[150]);
            Assert.AreEqual(61043.68441389341d, result.IqResultDetail.Chromatogram.Yvalues[150]);

            Assert.AreEqual(omicsSmoothedChromatogram.Count, result.IqResultDetail.Chromatogram.Yvalues.Length);
            Assert.AreEqual(omicsSmoothedChromatogram[3].Y, result.IqResultDetail.Chromatogram.Yvalues[3]);
            Assert.AreEqual(omicsSmoothedChromatogram[200].Y, result.IqResultDetail.Chromatogram.Yvalues[200]);
            Assert.AreEqual(omicsSmoothedChromatogram[omicsSmoothedChromatogram.Count - 3].Y, result.IqResultDetail.Chromatogram.Yvalues[omicsSmoothedChromatogram.Count - 3]);
        }

        [Test]
        public void TestChromatogramPeaks()
        {
            PopulateGlobalVariables();

            double chromToleranceInPPM = _workflowParameters.ChromGenTolerance;
            double MinRelativeIntensityForChromCorrelator = _workflowParameters.MinRelativeIntensityForChromCorrelator;

            //ITheorFeatureGenerator TheorFeatureGen = _theorFeatureGen;
            IqGlyQResult result = iQresult;
            //result.Target.TheorIsotopicProfile = TheorFeatureGen.GenerateTheorProfile(result.Target.EmpiricalFormula, result.Target.ChargeState);
            //result.Target.TheorIsotopicProfile = TheoreticalIsotopicProfileWrapper.GenerateSimpleOld(ref TheorFeatureGen, result.Target.EmpiricalFormula, result.Target.ChargeState);
            result.Target.TheorIsotopicProfile = _TheorFeatureGenV2.Generator(result.Target.EmpiricalFormula, result.Target.ChargeState);

            //result.Target.TheorIsotopicProfile = Utiliites.GenerateCombinedIsotopicProfile(result, 1, 1);

            List<int> offsets = new int[] { 1 }.ToList();
            List<float> ratios = new float[] { 1 }.ToList();

            IsotopeProfileBlended _TheorFeatureGenV2_M = new IsotopeProfileBlended(new ParametersBlendedIsotope(new ParametersSimpleIsotope(new JoshTheorFeatureGenerator(Globals.LabellingType.NONE, 0.1))));
            result.Target.TheorIsotopicProfile = _TheorFeatureGenV2_M.Generator(result.Target.EmpiricalFormula, result.Target.ChargeState, ref offsets, ref ratios);
            result.Target.TheorIsotopicProfile.RefreshAlternatePeakIntenstiesFromPeakList();


            ChromPeakDetector ChromPeakDetector = new ChromPeakDetector(_workflowParameters.ChromPeakDetectorPeakBR, _workflowParameters.ChromPeakDetectorSigNoise);
            ChromPeakAnalyzer ChromPeakAnalyzer = new IQ.Workflows.Core.ChromPeakSelection.ChromPeakAnalyzer(_workflowParameters);

            IQ.Backend.ProcessingTasks.Smoothers.SavitzkyGolaySmoother ChromSmoother = new SavitzkyGolaySmoother(_workflowParameters.LCParameters.ParametersSavitskyGolay.PointsForSmoothing,_workflowParameters.LCParameters.ParametersSavitskyGolay.PolynomialOrder,_workflowParameters.LCParameters.ParametersSavitskyGolay.AllowNegativeValues);

            //TEST CODE
            //result.IqResultDetail.Chromatogram = ChromGen.GenerateChromatogram(runIn, result.Target.TheorIsotopicProfile, result.Target.ElutionTimeTheor);
            // result.IqResultDetail.Chromatogram = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(runIn, possibleFragmentTarget.TheorIsotopicProfile, possibleFragmentTarget.ElutionTimeTheor);

            double massToExtract = Utiliites.GetMassToExtractFromIsotopeProfile(result.Target.TheorIsotopicProfile, _workflowParameters);
            result.IqResultDetail.Chromatogram = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(runIn, massToExtract);

            result.IqResultDetail.Chromatogram = ChromSmoother.Smooth(result.IqResultDetail.Chromatogram);

            //TestUtilities.DisplayXYValues(result.IqResultDetail.Chromatogram);

            //ExecuteTask(_chromPeakDetector);
            result.ChromPeakList = ChromPeakDetector.FindPeaks(result.IqResultDetail.Chromatogram);
            ChromPeakDetector.CalculateElutionTimes(runIn, result.ChromPeakList);

            result.IqResultDetail.ChromPeakQualityData = ChromPeakAnalyzer.GetChromPeakQualityData(runIn, result.Target, result.ChromPeakList);

            Assert.AreEqual(4, result.ChromPeakList.Count);
            Assert.AreEqual(18197090.0f, result.ChromPeakList[0].Height);
            Assert.AreEqual(1660.8837478792066d, result.ChromPeakList[0].XValue);
            //Assert.AreEqual(0, result.ElutionTimeObs);

            Assert.AreEqual(4, result.IqResultDetail.ChromPeakQualityData.Count);
            Assert.AreEqual(1661, result.IqResultDetail.ChromPeakQualityData[0].ScanLc);
            Assert.AreEqual(1706, result.IqResultDetail.ChromPeakQualityData[1].ScanLc);
            Assert.AreEqual(1790, result.IqResultDetail.ChromPeakQualityData[2].ScanLc);
            Assert.AreEqual(1820, result.IqResultDetail.ChromPeakQualityData[3].ScanLc);
        }

        [Test]
        public void TestCorrelation()
        {
            PopulateGlobalVariables();

            //ChromatogramCorrelatorBase _chromatogramCorrelator = new ChromatogramCorrelator(9,10, 0.01);
            ChromatogramCorrelatorBase _chromatogramCorrelator = new SimplePeakCorrelator(runIn, _workflowParameters, _workflowParameters.MinRelativeIntensityForChromCorrelator,_lcProcessor);

            double correlationscorecuttoff = 10;
            IqTarget tempTarget = new FragmentIQTarget();
            tempTarget.ChargeState = 2;

            possibleFragmentTarget = new FragmentIQTarget(tempTarget);
            FragmentIQTarget targetParent = new FragmentIQTarget(tempTarget);
            targetParent.ScanLCTarget = 1661;

            FragmentResultsObjectHolderIq noParentResult = new FragmentResultsObjectHolderIq(possibleFragmentTarget);
            FragmentResultsObjectHolderIq yesParentResult = new FragmentResultsObjectHolderIq(noParentResult);
            noParentResult.ScanBoundsInfo.Start = 1649;
            noParentResult.ScanBoundsInfo.Stop = 1673;
            yesParentResult.ScanBoundsInfo.Start = 1655;
            yesParentResult.ScanBoundsInfo.Stop = 1663;

            noParentResult.CorrelationCoefficients = new double[] { 10.3414454801327, 18238990.1090863, 1661.84987682769 };
            yesParentResult.CorrelationCoefficients = new double[] { 3.94812463326518, 18455.6784658179, 1665.11579601859 };

            ProcessedPeak fragmentCandiateFitPeak = new ProcessedPeak();
            fragmentCandiateFitPeak.MinimaOfLowerMassIndex = 0;
            fragmentCandiateFitPeak.MinimaOfHigherMassIndex = 8;


            ProcessedPeak parentCandiateFitPeak = new ProcessedPeak();
            parentCandiateFitPeak.MinimaOfLowerMassIndex = 0;
            parentCandiateFitPeak.MinimaOfHigherMassIndex = 6;

            List<XYData> fragmentEicFitXYData = new List<XYData>();
            fragmentEicFitXYData.Add(new XYData(1649, 8428195.54983544));
            fragmentEicFitXYData.Add(new XYData(1652, 11587949.5560651));
            fragmentEicFitXYData.Add(new XYData(1655, 14646387.7883955));
            fragmentEicFitXYData.Add(new XYData(1658, 17017917.0111495));
            fragmentEicFitXYData.Add(new XYData(1661, 18177502.5582393));
            fragmentEicFitXYData.Add(new XYData(1664, 17849003.2067264));
            fragmentEicFitXYData.Add(new XYData(1667, 16111859.3072063));
            fragmentEicFitXYData.Add(new XYData(1670, 13369934.9516055));
            fragmentEicFitXYData.Add(new XYData(1673, 10199170.8744788));

            List<XYData> parentEicFitXYData = new List<XYData>();
            parentEicFitXYData.Add(new XYData(1655, 692.806446202524));
            parentEicFitXYData.Add(new XYData(1658, 3637.1045665178));
            parentEicFitXYData.Add(new XYData(1661, 10718.8266176955));
            parentEicFitXYData.Add(new XYData(1664, 17733.1666956175));
            parentEicFitXYData.Add(new XYData(1667, 16469.2164985123));
            parentEicFitXYData.Add(new XYData(1670, 8586.32202489074));
            parentEicFitXYData.Add(new XYData(1673, 2512.9798007429));



            //FragmentResultsObjectHolderIQ yesParentResult = new FragmentResultsObjectHolderIQ(possibleFragmentTarget);

            int minNumberOfPointsToOverlap = 5;

            //TEST CODE
            FragmentResultsObjectHolderIq correlationResult = CorrelateWorkflow.Correlate(
                possibleFragmentTarget,
                targetParent,
                noParentResult,
                yesParentResult,
                targetParent.ScanLCTarget,
                fragmentCandiateFitPeak,
                parentCandiateFitPeak,
                fragmentEicFitXYData,
                parentEicFitXYData,
                ref _chromatogramCorrelator,
                correlationscorecuttoff,
                minNumberOfPointsToOverlap);


            Assert.AreEqual(0.68232565462731354d, correlationResult.CorrelationScore); //correlation of 100 points
            //Assert.AreEqual(0.30288147181456615d, yesParentResult.CorrelationScore); //correlation of data points
        }

        [Test]
        public void TestCreateChildTargets()
        {
            PopulateGlobalVariables();

            List<IqTarget> existingChildren = iQresult.Target.ChildTargets().ToList();

            foreach (IqTarget child in existingChildren)
            {
                iQresult.Target.RemoveTarget(child);
            }
            

            List<IqTarget> targetIn = new List<IqTarget>();
            targetIn.Add(iQresult.Target);

            //TEST CODE
            IqTargetUtilities util = new IqTargetUtilities();
            util.CreateChildTargets(targetIn);

            List<IqTarget> newChildren = iQresult.Target.ChildTargets().ToList();

            Assert.AreEqual(3, targetIn[0].GetChildCount());
            Assert.AreEqual(2, newChildren[0].ChargeState);
            Assert.AreEqual(3, newChildren[1].ChargeState);
            Assert.AreEqual(4, newChildren[2].ChargeState);
        }

        [Test]
        public void TestFitAndRetireve()
        {
            PNNLOmics.Algorithms.PeakDetection.PeakCentroider omicsPeakDetection = new PeakCentroider();
            
            List<XYData> data = new List<XYData>();
            data.Add(new XYData(1649, 8428195.54983544));
            data.Add(new XYData(1652, 11587949.5560651));
            data.Add(new XYData(1655, 14646387.7883955));
            data.Add(new XYData(1658, 17017917.0111495));
            data.Add(new XYData(1661, 18177502.5582393));
            data.Add(new XYData(1664, 17849003.2067264));
            data.Add(new XYData(1667, 16111859.3072063));
            data.Add(new XYData(1670, 13369934.9516055));
            data.Add(new XYData(1673, 10199170.8744788));
            
            double[] guess = new double[3];
            guess[0] = 2; //sigma
            guess[1] = data.Max(r => r.Y); //height
            //guess X by mid point of curve
            double center = data.Count / 2;
            int possibleCenter = Convert.ToInt32(Math.Truncate(center));
            guess[2] = data[possibleCenter].X; //m/z
            bool calculateArea = true;


            ///TEST CODE
            
            SolverReport fitMetrics;
            double area;
            List<XYData> truncatedfit = LevenburgMarquardt.FitGaussian(data, calculateArea, ref guess, out fitMetrics, out area);
            double[] coeffs = guess;

            Console.WriteLine("sigma=" + coeffs[0] + " height=" + coeffs[1] + " MZ=" + coeffs[2]);

            Assert.AreEqual(9, truncatedfit.Count);
            Assert.AreEqual(10.341445480132782d, coeffs[0]);//sigma
            Assert.AreEqual(18238990.109086305d, coeffs[1]);//height
            Assert.AreEqual(1661.8498768276909d, coeffs[2]);//m/z

            //TEST CODE
            List<PNNLOmics.Data.XYData> modelXY = LevenburgMarquardt.ReturnGaussianValues(coeffs, 100, 1, coeffs[2]);

            Console.WriteLine("X"+ "\t" +"Y");
            foreach (XYData xyData in modelXY)
            {
                Console.WriteLine("{0}\t{1}", xyData.X, xyData.Y);
            }

            Assert.AreEqual(101, modelXY.Count);
            Assert.AreEqual(1661.8498768276909d, modelXY[50].X);//sigma
            Assert.AreEqual(1645.8498768276909d, modelXY[34].X);//sigma
        }

        [Test]
        public void TestFitAndRetireveLorentzian()
        {
            List<XYData> data = new List<XYData>();
            data.Add(new XYData(1611.000424085, 888146.263638055));
            data.Add(new XYData(1621.000424085, 1350075.85796011));
            data.Add(new XYData(1631.000424085, 2267230.2561127));
            data.Add(new XYData(1641.000424085, 4404438.07527166));
            data.Add(new XYData(1651.000424085, 10138916.4851663));
            data.Add(new XYData(1661.000424085, 17913040.5754619));
            data.Add(new XYData(1671.000424085, 10138916.4851663));
            data.Add(new XYData(1681.000424085, 4404438.07527166));
            data.Add(new XYData(1691.000424085, 2267230.2561127));
            data.Add(new XYData(1701.000424085, 1350075.85796011));
            data.Add(new XYData(1711.000424085, 888146.263638055));


            double[] guess = new double[3];
            guess[0] = 100; //this guess is really important for lorentzian
            guess[1] = data.Max(r => r.Y)*50; //height.  this guess is really important for lorentzian
            //guess X by mid point of curve
            double center = data.Count / 2;
            int possibleCenter = Convert.ToInt32(Math.Truncate(center));
            guess[2] = data[possibleCenter].X; //m/z
            bool calculateArea = true;


            ///TEST CODE

            SolverReport fitMetrics;
            double area;
            List<XYData> truncatedfit = LevenburgMarquardt.FitLorentzian(data, calculateArea, ref guess, out fitMetrics, out area);
            double[] coeffs = guess;

            Console.WriteLine("sigma=" + coeffs[0] + " height=" + coeffs[1] + " MZ=" + coeffs[2]);

            Assert.AreEqual(11, truncatedfit.Count);
            Assert.AreEqual(15.935714585387393d, coeffs[0]);
            Assert.AreEqual(895652028.76752663d, coeffs[1]);//height
            Assert.AreEqual(1661.0004240850001d, coeffs[2]);//m/z
            
            //TEST CODE
            List<PNNLOmics.Data.XYData> modelXY = LevenburgMarquardt.ReturnLorentzianValues(coeffs, 100, 1, coeffs[2]);

            Console.WriteLine("X" + "\t" + "Y");
            foreach (XYData xyData in modelXY)
            {
                Console.WriteLine("{0}\t{1}", xyData.X, xyData.Y);
            }

            Assert.AreEqual(101, modelXY.Count);
            Assert.AreEqual(1661.0004240850001d, modelXY[50].X);//sigma
            Assert.AreEqual(1645.0004240850001d, modelXY[34].X);//sigma
        }

        //[Test]
        //public void TestIMSXIC()
        //{
        //    PopulateGlobalVariables();

        //    string UIMFMassSpecFilePath = @"D:\Csharp\ConosleApps\LocalServer\UIMF\Gly09_SN130_8Mar13_Cheetah_C14_220nL_IMS6_2700V_130C_Multi__BC.uimf";//work

        //    runIn = new UIMFRun(UIMFMassSpecFilePath);
        //    _workflowParameters.LCParameters.isIMS = true;

        //    double chromToleranceInPPM = _workflowParameters.ChromGenTolerance;
        //    double MinRelativeIntensityForChromCorrelator = _workflowParameters.MinRelativeIntensityForChromCorrelator;

        //    ITheorFeatureGenerator TheorFeatureGen = _theorFeatureGen;
        //    IqGlyQResult result = iQresult;
        //    //result.Target.TheorIsotopicProfile = TheorFeatureGen.GenerateTheorProfile(result.Target.EmpiricalFormula, result.Target.ChargeState);
        //    result.Target.TheorIsotopicProfile = TheoreticalIsotopicProfileWrapper.LowLevel(ref TheorFeatureGen, result.Target.EmpiricalFormula, result.Target.ChargeState);

        //    //TEST CODE
        //    ProcessorChromatogram.ProcessingParameters.ParametersChromGenerator.ChromToleranceInPPM = 10;
        //    result.IqResultDetail.Chromatogram = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(runIn, possibleFragmentTarget.TheorIsotopicProfile, possibleFragmentTarget.ElutionTimeTheor);

        //    Assert.AreEqual(176, result.IqResultDetail.Chromatogram.Xvalues.Length);
        //    Assert.AreEqual(777, result.IqResultDetail.Chromatogram.Xvalues[50]);
        //    Assert.AreEqual(8339, result.IqResultDetail.Chromatogram.Yvalues[60]);

        //    //we still need to reduce dimensions(sum all together or take largest in pile) and add zeroes

        //    //

        //}

        [Test]
        public void TestInput()
        {
            PopulateGlobalVariables();

            Assert.AreEqual(18, targetResult.ScanBoundsInfo.Buffer);
            Assert.AreEqual(9, targetResult.ScanBoundsInfo.ScansToSum);

            Assert.AreEqual(2.0, _workflowParameters.AreaOfPeakToSumInDynamicSumming);
            Assert.AreEqual(2.0, _workflowParameters.ChromGenSourceDataPeakBR);
            Assert.AreEqual(3.0, _workflowParameters.ChromGenSourceDataSigNoise);
            Assert.AreEqual(10.0, _workflowParameters.ChromGenTolerance);
            Assert.AreEqual(Globals.ToleranceUnit.PPM, _workflowParameters.ChromGenToleranceUnit);
            Assert.AreEqual(Globals.ChromatogramGeneratorMode.MOST_ABUNDANT_PEAK, _workflowParameters.ChromGeneratorMode);
            Assert.AreEqual(1.28, _workflowParameters.ChromNETTolerance);
            Assert.AreEqual(1, _workflowParameters.ChromPeakDetectorPeakBR);
            Assert.AreEqual(1, _workflowParameters.ChromPeakDetectorSigNoise);
            Assert.AreEqual(Globals.PeakSelectorMode.ClosestToTarget, _workflowParameters.ChromPeakSelectorMode);
            Assert.AreEqual(9, _workflowParameters.ChromSmootherNumPointsInSmooth);
            Assert.AreEqual(false, _workflowParameters.ChromatogramCorrelationIsPerformed);
            //Assert.AreEqual(4, _workflowParameters.FragmentsIQ.Count);//4 monosachcaride
            Assert.AreEqual(1, _workflowParameters.FragmentsIq.Count);//1 monosaccharide
            Assert.AreEqual(1.1, _workflowParameters.IsotopeLabelingEfficiency);
            Assert.AreEqual(0.005, _workflowParameters.IsotopeLowPeakCuttoff);
            Assert.AreEqual(Globals.LabellingType.Deuterium, _workflowParameters.IsotopeProfileType);
            Assert.AreEqual(Globals.IsotopicProfileType.LABELLED, _workflowParameters.LabelingTypeSwitch);
            Assert.AreEqual(2.0, _workflowParameters.MSPeakDetectorPeakBR);
            Assert.AreEqual(2.0, _workflowParameters.MSPeakDetectorSigNoise);
            Assert.AreEqual(7.0, _workflowParameters.MSToleranceInPPM);
            Assert.AreEqual(100, _workflowParameters.MaxScansSummedInDynamicSumming);
            Assert.AreEqual(0.1, _workflowParameters.MinRelativeIntensityForChromCorrelator);
            Assert.AreEqual(0.5, _workflowParameters.MolarMixingFractionOfH);
            Assert.AreEqual(true, _workflowParameters.MultipleHighQualityMatchesAreAllowed);
            Assert.AreEqual(20, _workflowParameters.NumChromPeaksAllowedDuringSelection);
            Assert.AreEqual(9, _workflowParameters.NumMSScansToSum);
            Assert.AreEqual(false, _workflowParameters.ProcessMsMs);
            Assert.AreEqual(Globals.ResultType.DEUTERATED_TARGETED_RESULT, _workflowParameters.ResultType);
            Assert.AreEqual(1, _workflowParameters.SmartChromPeakSelectorNumMSSummed);
            Assert.AreEqual(GlobalsWorkFlow.SummingModeEnum.SUMMINGMODE_STATIC, _workflowParameters.SummingMode);
            Assert.AreEqual(GlobalsWorkFlow.TargetedWorkflowTypes.Deuterated, _workflowParameters.WorkflowType);

            Assert.AreEqual(EnumerationIsPic.IsNotPic, _workflowParameters.IsPic);
            Assert.AreEqual(EnumerationIsotopicProfileMode.DH, _workflowParameters.MSParameters.IsoParameters.IsotopeProfileMode);

            //not a very deep test
            Assert.AreEqual(1661, targetResult.Primary_Target.ScanLCTarget);

            Assert.IsNotNull(runIn);

            Assert.IsNotNull(iQresult);
            Assert.IsNotNull(iQresult.Target);
            Assert.AreEqual(2, iQresult.Target.ChargeState);
            Assert.AreEqual("C64H110N2O51", iQresult.Target.EmpiricalFormula);
            Assert.AreEqual(4, iQresult.Target.GetChildCount());
            Assert.AreEqual(true, iQresult.Target.HasChildren());
            Assert.AreEqual(862.31105012450541d, iQresult.Target.MZTheor);
            Assert.AreEqual(1722.6075472690109d, iQresult.Target.MonoMassTheor);

            Assert.IsNotNull(possibleFragmentTarget);
            Assert.AreEqual(1661, possibleFragmentTarget.ScanLCTarget);
        }

        [Test]
        public void TestIsotopeProfile()
        {
            PopulateGlobalVariables();

            double chromToleranceInPPM = _workflowParameters.ChromGenTolerance;
            double MinRelativeIntensityForChromCorrelator = _workflowParameters.MinRelativeIntensityForChromCorrelator;

            //ITheorFeatureGenerator TheorFeatureGen = _theorFeatureGen;
            IqResult result = iQresult;

            //TEST CODE
            //result.Target.TheorIsotopicProfile = TheorFeatureGen.GenerateTheorProfile(result.Target.EmpiricalFormula, result.Target.ChargeState);
            //result.Target.TheorIsotopicProfile = TheoreticalIsotopicProfileWrapper.GenerateSimpleOld( ref TheorFeatureGen, result.Target.EmpiricalFormula, result.Target.ChargeState);
            result.Target.TheorIsotopicProfile = _TheorFeatureGenV2.Generator(result.Target.EmpiricalFormula, result.Target.ChargeState);

            Assert.AreEqual(16, result.Target.TheorIsotopicProfile.Peaklist.Count);
            Assert.AreEqual(862.31105012450541d, result.Target.TheorIsotopicProfile.Peaklist[0].XValue);
            Assert.AreEqual(1, result.Target.TheorIsotopicProfile.Peaklist[0].Height);
            Assert.AreEqual(1722.6075472690109d, result.Target.MonoMassTheor);
            Assert.AreEqual(2, result.Target.ChargeState);

            result.Target.TheorIsotopicProfile = Utiliites.GenerateCombinedIsotopicProfile(result, 1, 1);

            Assert.AreEqual(862.8122251245054d, result.Target.TheorIsotopicProfile.Peaklist[1].XValue);
            Assert.AreEqual(0.573986948f, result.Target.TheorIsotopicProfile.Peaklist[0].Height);
            Assert.AreEqual(1, result.Target.TheorIsotopicProfile.Peaklist[1].Height);
            Assert.AreEqual(1722.6075472690109d, result.Target.MonoMassTheor);
        }

        [Test]
        public void TestPeakCentroiding()
        {
            string testFile = "";
            //testFile = @"E:\ScottK\GetPeaks Data\Diabetes_LC\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw";
            //testFile = @"E:\PNNL Data\2012_09_05 SPIN Q-TOF\Gly08_SQTOF_SP02_3X_C1_12_HPIF20Torr_LPRF96_T160_6Sept12.d";


            //testFile = @"E:\PNNL Data\2012_09_05 SPIN Q-TOF\Gly08_SQTOF_SL26_100_C2_13_HPIF30Torr_LPRF96_T160_18Sept12.d";//work
            //testFile = @"L:\PNNL Files\PNNL Data for Tests\Gly08_SQTOF_SL26_100_C2_13_HPIF30Torr_LPRF96_T160_18Sept12.d";//home
            testFile = massSpecFilePath;
            RunFactory factor = new RunFactory();
            Run runIn = factor.CreateRun(Globals.MSFileType.Agilent_D, testFile);
            //Run runIn = RunUtilities.CreateAndAlignRun(testFile);

            ScanObject scanInfo = new ScanObject(2615, 2684);
            //possibleFragmentTarget.ScanInfo.Start = 1648;
            //possibleFragmentTarget.ScanInfo.Stop = 1674;
            scanInfo.Max = runIn.MaxLCScan;
            scanInfo.Min = runIn.MinLCScan;
            scanInfo.ScansToSum = 51;
            scanInfo.Buffer = 9 * 2;

            runIn.ScanSetCollection.Create(runIn, scanInfo.Start, scanInfo.Stop, scanInfo.ScansToSum, 1, false);

            IqResult iQresult = new IqResult(new IqTargetBasic());

            ProcessingParametersMassSpectra msParameters = new ProcessingParametersMassSpectra();
            msParameters.MsGeneratorParameters.MsFileType = runIn.MSFileType;
            _msProcessor = new ProcessorMassSpectra(msParameters);

            //MSGenerator _msGenerator = _workflowParameters.MSParameters.Engine_msGenerator;

            iQresult.LCScanSetSelected = Utiliites.ScanSetFromStartStop(runIn, scanInfo);

            PeakCentroiderParameters centroidParameters = new PeakCentroiderParameters(PeakFitType.Parabola);
            PeakThresholderParameters thresholdParameters = new PeakThresholderParameters();


            PeakCentroider centroider = new PeakCentroider(centroidParameters);
            PeakThresholder thresholder = new PeakThresholder(thresholdParameters);

            //SPIN parameters
            int movingAverage = 7;
            int smoothPoints = 5;
            int pointsPerShoulder = 3;
            float sigmaAboveThreshold = 3;

            ProcessingParametersChromatogram parametersForXYDataPath = new ProcessingParametersChromatogram();

            bool isPic = false;
            if (isPic)
            {
                parametersForXYDataPath.XYDataWriterPath = @"F:\ScottK\Results\XYDataWriter";
            }
            else
            {
                parametersForXYDataPath.XYDataWriterPath = @"E:\ScottK\WorkingResults\XYDataWriter";
            }

            string thresholdPath3 = parametersForXYDataPath.XYDataWriterPath + @"\MS_5_OmicsThreshold_3S.txt";
            string rawPath = parametersForXYDataPath.XYDataWriterPath + @"\MS_0_Raw.txt";
            string averagedPath = parametersForXYDataPath.XYDataWriterPath + @"\MS_1_Averaged.txt";
            string smoothedPath = parametersForXYDataPath.XYDataWriterPath + @"\MS_2_Smoothed.txt";
            string hammerPath = parametersForXYDataPath.XYDataWriterPath + @"\MS_5_Hammer.txt";
            DataXYDataWriter writer = new DataXYDataWriter();

            //TEST CODE

            //0.  get spectra.  raw untouched summed data
            //iQresult.IqResultDetail.MassSpectrum = _msGenerator.GenerateMS(runIn, iQresult.LCScanSetSelected);
            iQresult.IqResultDetail.MassSpectrum = _msProcessor.DeconMSGeneratorWrapper(runIn, iQresult.LCScanSetSelected);
            writer.WriteDeconXYDataDeconTools(iQresult.IqResultDetail.MassSpectrum, rawPath);
            Assert.AreEqual(258907, iQresult.IqResultDetail.MassSpectrum.Xvalues.Length);

            //1.  moving average of mass spectra.  dejitters
            iQresult.IqResultDetail.MassSpectrum = Utiliites.MovingAverage(iQresult.IqResultDetail.MassSpectrum, movingAverage);
            writer.WriteDeconXYDataDeconTools(iQresult.IqResultDetail.MassSpectrum, averagedPath);
            Assert.AreEqual(258907, iQresult.IqResultDetail.MassSpectrum.Xvalues.Length);

            //2. smooth data with 2nd order savitsky golay
            SavitzkyGolaySmoother smoother = new SavitzkyGolaySmoother(smoothPoints, 2, false);
            iQresult.IqResultDetail.MassSpectrum = smoother.Smooth(iQresult.IqResultDetail.MassSpectrum);
            writer.WriteDeconXYDataDeconTools(iQresult.IqResultDetail.MassSpectrum, smoothedPath);
            Assert.AreEqual(258907, iQresult.IqResultDetail.MassSpectrum.Xvalues.Length);

            //3. convert to Omics world from Decon world
            List<XYData> omicsPeaks = ConvertXYData.DeconXYDataToOmicsXYData(iQresult.IqResultDetail.MassSpectrum);
            Assert.AreEqual(omicsPeaks.Count, iQresult.IqResultDetail.MassSpectrum.Xvalues.Length);

            //4.  centroid peaks
            List<ProcessedPeak> centroidedPeaks = centroider.DiscoverPeaks(omicsPeaks);
            Assert.AreEqual(23647, centroidedPeaks.Count);

            //5.  filter baseed on symetry.  we need at least 2 points on each side of the maximum
            List<ProcessedPeak> filteredPeaks = Utiliites.FilterByPointsPerSide(centroidedPeaks, pointsPerShoulder);
            //Assert.AreEqual(11303, filteredPeaks.Count);
            //Assert.AreEqual(14977, filteredPeaks.Count);
            //Assert.AreEqual(17748, filteredPeaks.Count);
            //Assert.AreEqual(18096, filteredPeaks.Count);//allowind =-2 asymetry//this is the old algorithm.  the one below is better
            Assert.AreEqual(21414, filteredPeaks.Count);//allowind =-2 asymetry
            //6.  threshold data
            thresholder.Parameters.SignalToShoulderCuttoff = sigmaAboveThreshold;
            List<ProcessedPeak> thresholdedPeaks3 = thresholder.ApplyThreshold(filteredPeaks);
            writer.WriteOmicsProcesedPeakData(thresholdedPeaks3, thresholdPath3);
            //Assert.AreEqual(2175, thresholdedPeaks3.Count);
            //Assert.AreEqual(2969, thresholdedPeaks3.Count);
            //Assert.AreEqual(3481, thresholdedPeaks3.Count);//this is the old algorithm for point filtering
            Assert.AreEqual(4389, thresholdedPeaks3.Count);
            //THIS IS NOT FIGURED OUT YET

            //OrbitrapPeakDetection.HammerThresholdParameters hammerParameters = new HammerThresholdParameters();
            //hammerParameters.CentroidPeakToBackgroundRatio = 1;
            //hammerParameters.OptimizeOrDefaultChoise = HammerThresholdParameters.OptimizeOrDefaultMassSpacing.Default;
            //hammerParameters.ThresholdOrClusterChoise = HammerThresholdParameters.OrbitrapFilteringMethod.Threshold;

            //OrbitrapPeakDetection.FindPeakClusters hammerPeakDetector = new FindPeakClusters();
            //ThresholdParameters parameters = new ThresholdParameters();
            //NumberOfPointsPerRegionFinder pointsPerRegionFinder = new NumberOfPointsPerRegionFinder();

            //parameters.NumberOfPointsPerNoiseRegion = pointsPerRegionFinder.FindNumberOfPointsPerRegionList(clusterCount);
            //parameters.SigmaMultiplier = orbitrapFilterSigmaMultiplier;//For CP thresholding

            //TestRawData rawDataTest = new TestRawData();

            //List<ClusterCP<double>> clustersAutomatedRefined = new List<ClusterCP<double>>();
            //List<XYData> signalPeaksAutomated = new List<XYData>();
            //List<XYData> noisePeaksAutomated = new List<XYData>();

            //rawDataTest.ParseSignalAndNoise(peaks, parameters, clusters, out clustersAutomatedRefined, out signalPeaksAutomated, out noisePeaksAutomated);


            OrbitrapFilterParameters ParametersOrbitrap = new OrbitrapFilterParameters();
            ParametersOrbitrap.DeltaMassTollerancePPM = 2000;
            ParametersOrbitrap.ExtraSigmaFactor = 0;
            ParametersOrbitrap.massNeutron = 1.00;


            //3.  threshold data
            PeakThresholderParameters parametersThreshold = new PeakThresholderParameters();

            parametersThreshold.isDataThresholded = false;// parameters.Part1Parameters.isDataAlreadyThresholded;
            //parametersThreshold.SignalToShoulderCuttoff = (float)parameters.Part1Parameters.ElutingPeakNoiseThreshold;
            parametersThreshold.SignalToShoulderCuttoff = sigmaAboveThreshold;// (float)parameters.Part1Parameters.MSPeakDetectorPeakBR;


            //                          parametersThreshold.ScanNumber = scanCounter;
            parametersThreshold.DataNoiseType = InstrumentDataNoiseType.NoiseRemoved;// parameters.Part1Parameters.NoiseType;

            SwitchThreshold newSwitchThreshold = new SwitchThreshold();
            newSwitchThreshold.Parameters = parametersThreshold;
            newSwitchThreshold.ParametersOrbitrap = ParametersOrbitrap;
            List<ProcessedPeak> hammerThresholdedData = newSwitchThreshold.ThresholdNow(ref filteredPeaks);

            writer.WriteOmicsProcesedPeakData(hammerThresholdedData, hammerPath);
            //Assert.AreEqual(2225, hammerThresholdedData.Count);
            //Assert.AreEqual(2949, hammerThresholdedData.Count);
            //Assert.AreEqual(3739, hammerThresholdedData.Count);
            //Assert.AreEqual(3854, hammerThresholdedData.Count);//this is the old algorithm for point filtering and perhaps old hammer
            Assert.AreEqual(5241, hammerThresholdedData.Count);
        }

        [Test]
        public void TestPeakCentroidingSpeedOmicsDecon()
        {
            
            string testFile = "";
            testFile = @"E:\ScottK\GetPeaks Data\Diabetes_LC\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw";
            testFile = @"E:\PNNL_Data\2012_12_24_Velos_3\\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw";
            //testFile = @"E:\PNNL Data\2012_09_05 SPIN Q-TOF\Gly08_SQTOF_SP02_3X_C1_12_HPIF20Torr_LPRF96_T160_6Sept12.d";

            //testFile = @"E:\PNNL Data\2012_09_05 SPIN Q-TOF\Gly08_SQTOF_SL26_100_C2_13_HPIF30Torr_LPRF96_T160_18Sept12.d";//work
            //testFile = @"L:\PNNL Files\PNNL Data for Tests\Gly08_SQTOF_SL26_100_C2_13_HPIF30Torr_LPRF96_T160_18Sept12.d";//home

            RunFactory factor = new RunFactory();
            Run runIn = factor.CreateRun(testFile);

            ScanObject scanInfo = new ScanObject(0, 0);
            //possibleFragmentTarget.ScanInfo.Start = 1648;
            //possibleFragmentTarget.ScanInfo.Stop = 1674;
            scanInfo.Max = runIn.MaxLCScan;
            scanInfo.Min = runIn.MinLCScan;
            scanInfo.ScansToSum = 5;//or 5
            scanInfo.Buffer = 9 * 2;

            runIn.ScanSetCollection.Create(runIn, scanInfo.Min, scanInfo.Max, scanInfo.ScansToSum, 1, false);

            IqResult iQresult = new IqResult(new IqTargetBasic());

            ProcessingParametersMassSpectra msParameters = new ProcessingParametersMassSpectra();
            msParameters.MsGeneratorParameters.MsFileType = runIn.MSFileType;
            msParameters.Engine_OmicsPeakDetection.Parameters.FWHMPeakFitType = PeakFitType.Parabola;
            _msProcessor = new ProcessorMassSpectra(msParameters);

            //MSGenerator _msGenerator = _workflowParameters.MSParameters.Engine_msGenerator;

            iQresult.LCScanSetSelected = Utiliites.ScanSetFromCenterScan(runIn,10, scanInfo.ScansToSum);

            PeakCentroiderParameters centroidParameters = new PeakCentroiderParameters(PeakFitType.Parabola);
            PeakThresholderParameters thresholdParameters = new PeakThresholderParameters();


            PeakCentroider centroider = new PeakCentroider(centroidParameters);
            PeakThresholder thresholder = new PeakThresholder(thresholdParameters);

            //SPIN parameters
            int movingAverage = 7;
            int smoothPoints = 5;
            int pointsPerShoulder = 3;
            float sigmaAboveThreshold = 3;

            ProcessingParametersChromatogram parametersForXYDataPath = new ProcessingParametersChromatogram();

            bool isPic = false;
            if (isPic)
            {
                parametersForXYDataPath.XYDataWriterPath = @"F:\ScottK\Results\XYDataWriter";
            }
            else
            {
                parametersForXYDataPath.XYDataWriterPath = @"E:\ScottK\WorkingResults\XYDataWriter";
            }

            string thresholdPath3 = parametersForXYDataPath.XYDataWriterPath + @"\MS_5_OmicsThreshold_3S.txt";
            string rawPath = parametersForXYDataPath.XYDataWriterPath + @"\MS_0_Raw.txt";
            string averagedPath = parametersForXYDataPath.XYDataWriterPath + @"\MS_1_Averaged.txt";
            string smoothedPath = parametersForXYDataPath.XYDataWriterPath + @"\MS_2_Smoothed.txt";
            string hammerPath = parametersForXYDataPath.XYDataWriterPath + @"\MS_5_Hammer.txt";
            DataXYDataWriter writer = new DataXYDataWriter();

            //TEST CODE

            //0.  get spectra.  raw untouched summed data
            //iQresult.IqResultDetail.MassSpectrum = _msGenerator.GenerateMS(runIn, iQresult.LCScanSetSelected);
            iQresult.IqResultDetail.MassSpectrum = _msProcessor.DeconMSGeneratorWrapper(runIn, iQresult.LCScanSetSelected);
            //writer.WriteDeconXYDataDeconTools(iQresult.IqResultDetail.MassSpectrum, rawPath);
            //Assert.AreEqual(258905, iQresult.IqResultDetail.MassSpectrum.Xvalues.Length);
            Assert.AreEqual(76216, iQresult.IqResultDetail.MassSpectrum.Xvalues.Length);
            //1.  moving average of mass spectra.  dejitters
            List<ProcessedPeak> peaksTest = _msProcessor.Execute(iQresult.IqResultDetail.MassSpectrum, EnumerationMassSpectraProcessing.OmicsCentroid_Only);

            Assert.AreEqual(7379, peaksTest.Count);//sum5
            //Assert.AreEqual(43208, peaks.Count);//sum1

            int iterationScans = 10;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            System.DateTime starttime = DateTime.Now;

            List<List<ProcessedPeak>> peakPile = new List<List<ProcessedPeak>>();

            for (int i = 0; i < iterationScans; i++)
            {
                ScanSet scanSet = runIn.ScanSetCollection.ScanSetList[i];
                iQresult.IqResultDetail.MassSpectrum = _msProcessor.DeconMSGeneratorWrapper(runIn, scanSet);
                //int scan = i;
                //scan = 135;
                List<ProcessedPeak> peaks = _msProcessor.Execute(iQresult.IqResultDetail.MassSpectrum, EnumerationMassSpectraProcessing.OmicsCentroid_Only);

                Console.WriteLine("We have read in " + peaks.Count + " MS1 peaks");
                peakPile.Add(peaks);
                Console.WriteLine("index is " + i + " and scan is " + scanSet.PrimaryScanNumber);
            }

            System.DateTime stoptime = DateTime.Now;
            Console.WriteLine("This started at " + starttime + " and ended at" + stoptime);
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to find " + 100 + " eluting peaks");
            stopWatch.Stop();

            TimeSpan timeForOmics = stopWatch.Elapsed;
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("finished with tandem, press return to end");
            Console.WriteLine("");
            
            //Decon


            Stopwatch stopWatchDecon = new Stopwatch();
            stopWatchDecon.Start();
            System.DateTime starttimeDecon = DateTime.Now;

            //DeconTools.Backend.ProcessingTasks.PeakDetectors.PeakDetector deconPeakDetector = new DeconToolsPeakDetector(0, 0, Globals.PeakFitType.QUADRATIC, false);
            DeconTools.Backend.ProcessingTasks.PeakDetectors.PeakDetector deconPeakDetector = new DeconToolsPeakDetectorV2(0, 0, Globals.PeakFitType.QUADRATIC, false);

            List<List<Peak>> peakPileDecon = new List<List<Peak>>();

            for (int i = 0; i < iterationScans; i++)
            {
                ScanSet scanSet = runIn.ScanSetCollection.ScanSetList[i];
                Run32.Backend.Data.XYData deconMassSpecta = msParameters.Engine_msGenerator.GenerateMS(runIn, scanSet);
                //int scan = i;
                //scan = 135;

                List<Peak> peaksDecon = deconPeakDetector.FindPeaks(deconMassSpecta);

                Console.WriteLine("We have read in " + peaksDecon.Count + " MS1 peaks");
                peakPileDecon.Add(peaksDecon);
                Console.WriteLine("index is " + i + " and scan is " + scanSet.PrimaryScanNumber);
            }

            System.DateTime stoptimeDecon = DateTime.Now;
            Console.WriteLine("This started at " + starttimeDecon + " and ended at" + stoptimeDecon);
            Console.WriteLine("This took " + stopWatchDecon.Elapsed + " seconds to find " + 100 + " eluting peaks");
            stopWatchDecon.Stop();
            TimeSpan timeForDecon = stopWatchDecon.Elapsed;
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("finished with tandem, press return to end");
            Console.WriteLine("");

            Console.WriteLine("This took " + timeForOmics + " seconds for Omics and  " + timeForDecon + " for Decon");
        }

        [Test]
        public void TestProcessorLcAt2Levels()
        {
            PopulateGlobalVariables();
            Tuple<string, string> tuple = errorlog;

            targetResult.ScanBoundsInfo.Start = 1640;
            targetResult.ScanBoundsInfo.Stop = 1847;
            targetResult.ScanBoundsInfo.Buffer = 20;
            double chromTollerence = 10;
            //_workflowParameters.LCParameters.ParametersChromGenerator.ChromToleranceInPPM = chromTollerence;
            _workflowParameters.LCParameters.Engine_PeakChromGenerator = new PeakChromatogramGenerator(chromTollerence);
            double mass = 862.3110;

            //0.  generatre Chromatorgrams
            Run32.Backend.Data.XYData bufferedDeconChromatogram = _lcProcessor.GenerateBufferedChromatogramByPoint(runIn, targetResult.ScanBoundsInfo, mass);
            //DeconTools.Backend.XYData bufferedDeconChromatogram = ProcessorChromatogram.GenerateBufferedChromatogramByPoint(runIn, mass, targetResult.ScanBoundsInfo, _workflowParameters.LCParameters.ParametersChromGenerator.ChromToleranceInPPM, _lcProcessor);

            List<XYData> bufferedchromatogramRaw = ConvertXYData.DeconXYDataToOmicsXYData(bufferedDeconChromatogram);
            Assert.AreEqual(110, bufferedchromatogramRaw.Count);
 
            //DeconTools.Backend.XYData deconChromatogram = _workflowParameters.LCParameters.Engine_PeakChromGenerator.GenerateChromatogram(runIn, targetResult.ScanBoundsInfo.Start, targetResult.ScanBoundsInfo.Stop, mass, chromTollerence);
            //DeconTools.Backend.XYData deconChromatogram = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(runIn, mass, targetResult.ScanBoundsInfo, chromTollerence);
            Run32.Backend.Data.XYData deconChromatogram = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(runIn, mass, targetResult.ScanBoundsInfo);
            
            List<PNNLOmics.Data.XYData> omics = ConvertXYData.DeconXYDataToOmicsXYData(deconChromatogram);
            List<PNNLOmics.Data.XYData> omicsChromatogram = ChangeRange.ClipXyDataToScanRange(omics, targetResult.ScanBoundsInfo.Start, targetResult.ScanBoundsInfo.Stop);
            Assert.AreEqual(70, omicsChromatogram.Count);

            DataXYDataWriter writer = new DataXYDataWriter();
            
            string bufferPath = _workflowParameters.LCParameters.XYDataWriterPath + @"\MS_0_Buffered";

            writer.WriteOmicsXYData(bufferedchromatogramRaw, bufferPath + 0 + ".txt");

            string noBufferPath = _workflowParameters.LCParameters.XYDataWriterPath + @"\MS_0_NonBuffered";

             writer.WriteOmicsXYData(omicsChromatogram, noBufferPath + 0 + ".txt");
        }

        [Test]
        public void TestProcessorLc()
        {
            PopulateGlobalVariables();
            Tuple<string, string> tuple = errorlog;

            
            targetResult.ScanBoundsInfo.Start = 1640;
            //targetResult.ScanBoundsInfo.Stop = 1847;
            targetResult.ScanBoundsInfo.Stop = 1880;
            double chromTollerence = 10;
            //_workflowParameters.LCParameters.ParametersChromGenerator.ChromToleranceInPPM = chromTollerence;
            _workflowParameters.LCParameters.Engine_PeakChromGenerator = new PeakChromatogramGenerator(chromTollerence);

            targetResult.ScanBoundsInfo.Buffer = 0;//this is an absolute scan number.  for a ms evert 3 scans... a buffer of 30 will be 10 scans
            double mass = possibleFragmentTarget.TheorIsotopicProfile.MonoPeakMZ;


            //0.  generatre Chromatorgrams
            //Full simple
            //DeconTools.Backend.XYData deconChromatogram = _workflowParameters.LCParameters.Engine_PeakChromGenerator.GenerateChromatogram(runIn, targetResult.ScanBoundsInfo.Min, targetResult.ScanBoundsInfo.Max, mass, chromTollerence);
            ScanObject ScanBoundsInfoMaxMin = new ScanObject(targetResult.ScanBoundsInfo.Min, targetResult.ScanBoundsInfo.Max);
            //DeconTools.Backend.XYData deconChromatogram = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(runIn, mass, ScanBoundsInfoMaxMin, chromTollerence);//old
            Run32.Backend.Data.XYData deconChromatogram = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(runIn, mass, ScanBoundsInfoMaxMin);

            List<XYData> chromatogramRaw = ConvertXYData.DeconXYDataToOmicsXYData(deconChromatogram);
            Assert.AreEqual(1796, chromatogramRaw.Count);

            double rawValueAtScan1847 = 148702.40625d;
            double rawValueAtScan1817 = 557311.1875d;
            double smoothedValueAtScan1847 = 126319.24181547623d;
            double smoothedValueAtScan1817 = 606164.8125d;

            Assert.AreEqual(1847, chromatogramRaw[590].X);
            Assert.AreEqual(rawValueAtScan1847, chromatogramRaw[590].Y);
            Assert.AreEqual(1817, chromatogramRaw[580].X);
            Assert.AreEqual(rawValueAtScan1817, chromatogramRaw[580].Y);

            //full top peak
            _workflowParameters.LCParameters.Engine_PeakChromGenerator.ChromatogramGeneratorMode = Globals.ChromatogramGeneratorMode.MOST_ABUNDANT_PEAK;

            //DeconTools.Backend.XYData deconChromatogramTopPeak = _workflowParameters.LCParameters.Engine_PeakChromGenerator.GenerateChromatogram(runIn, targetResult.TargetFragment.TheorIsotopicProfile, targetResult.ScanBoundsInfo.Min, targetResult.ScanBoundsInfo.Max, chromTollerence, Globals.ToleranceUnit.PPM);
            Run32.Backend.Data.XYData deconChromatogramTopPeak = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(runIn, targetResult.Primary_Target.TheorIsotopicProfile, targetResult.ScanBoundsInfo, chromTollerence, Globals.ToleranceUnit.PPM, _workflowParameters.LCParameters.Engine_PeakChromGenerator);
            
            List<XYData> chromatogramRawTopPeak = ConvertXYData.DeconXYDataToOmicsXYData(deconChromatogramTopPeak);
            Assert.AreEqual(chromatogramRawTopPeak.Count, 897);

            string topPeakPath = _workflowParameters.LCParameters.XYDataWriterPath + @"\MS_0_TopPeak";
            DataXYDataWriter writer = new DataXYDataWriter();
            writer.WriteOmicsXYData(chromatogramRawTopPeak, topPeakPath + 0 + ".txt");

            //full top n
            _workflowParameters.LCParameters.Engine_PeakChromGenerator.ChromatogramGeneratorMode = Globals.ChromatogramGeneratorMode.TOP_N_PEAKS;
            _workflowParameters.LCParameters.Engine_PeakChromGenerator.TopNPeaksLowerCutOff = 0.56;

            //DeconTools.Backend.XYData deconChromatogramTopN = _workflowParameters.LCParameters.Engine_PeakChromGenerator.GenerateChromatogram(runIn, targetResult.TargetFragment.TheorIsotopicProfile, targetResult.ScanBoundsInfo.Min, targetResult.ScanBoundsInfo.Max, chromTollerence, Globals.ToleranceUnit.PPM);
            Run32.Backend.Data.XYData deconChromatogramTopN = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(runIn, targetResult.Primary_Target.TheorIsotopicProfile, targetResult.ScanBoundsInfo, chromTollerence, Globals.ToleranceUnit.PPM, _workflowParameters.LCParameters.Engine_PeakChromGenerator);
            List<XYData> chromatogramRawTopN = ConvertXYData.DeconXYDataToOmicsXYData(deconChromatogramTopN);
            Assert.AreEqual(chromatogramRawTopN.Count, 1796);

            string topNPeaksPath = _workflowParameters.LCParameters.XYDataWriterPath + @"\MS_0_TopNPeaks";
            writer.WriteOmicsXYData(chromatogramRawTopN, topNPeaksPath + _workflowParameters.LCParameters.Engine_PeakChromGenerator.TopNPeaksLowerCutOff*100 + ".txt");

            //full top 1
            _workflowParameters.LCParameters.Engine_PeakChromGenerator.ChromatogramGeneratorMode = Globals.ChromatogramGeneratorMode.TOP_N_PEAKS;
            _workflowParameters.LCParameters.Engine_PeakChromGenerator.TopNPeaksLowerCutOff = 0.99;

            //DeconTools.Backend.XYData deconChromatogramTop1 = _workflowParameters.LCParameters.Engine_PeakChromGenerator.GenerateChromatogram(runIn, targetResult.TargetFragment.TheorIsotopicProfile, targetResult.ScanBoundsInfo.Min, targetResult.ScanBoundsInfo.Max, chromTollerence, Globals.ToleranceUnit.PPM);
            Run32.Backend.Data.XYData deconChromatogramTop1 = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(runIn, targetResult.Primary_Target.TheorIsotopicProfile, targetResult.ScanBoundsInfo, chromTollerence, Globals.ToleranceUnit.PPM, _workflowParameters.LCParameters.Engine_PeakChromGenerator);
            
            List<XYData> chromatogramRawTop1 = ConvertXYData.DeconXYDataToOmicsXYData(deconChromatogramTop1);
            Assert.AreEqual(chromatogramRawTop1.Count, 897);




          


            //0 buffer
            targetResult.ScanBoundsInfo.Buffer = 0;//this is a point number
            Run32.Backend.Data.XYData deconBufferedChromatogramPoint = _lcProcessor.GenerateBufferedChromatogramByPoint(runIn, targetResult.ScanBoundsInfo, mass);
            List<XYData> chromatogramRawViaPoint = ConvertXYData.DeconXYDataToOmicsXYData(deconBufferedChromatogramPoint);
            //Assert.AreEqual(chromatogramRawViaPoint.Count, 70);
            Assert.AreEqual(81, chromatogramRawViaPoint.Count);

            //20 pt Buffered Omics
            targetResult.ScanBoundsInfo.Buffer = 20;//this is a point number
            //targetResult.ScanBoundsInfo.Buffer = 40;//this is a point number
            Run32.Backend.Data.XYData deconBufferedChromatogram = _lcProcessor.GenerateBufferedChromatogramByPoint(runIn, targetResult.ScanBoundsInfo, mass);
            List<XYData> chromatogramRawBuffered = ConvertXYData.DeconXYDataToOmicsXYData(deconBufferedChromatogram);
            //Assert.AreEqual(chromatogramRawBuffered.Count, 110);
            Assert.AreEqual(121, chromatogramRawBuffered.Count);
            //Assert.AreEqual(281, chromatogramRawBuffered.Count);

            Assert.AreEqual(1847, chromatogramRawBuffered[89].X);
            Assert.AreEqual(rawValueAtScan1847, chromatogramRawBuffered[89].Y);
            Assert.AreEqual(1817, chromatogramRawBuffered[79].X);
            Assert.AreEqual(rawValueAtScan1817, chromatogramRawBuffered[79].Y);


            //write if interestd
            //Console.WriteLine("x\tY\tRAW Full Chromatogram"); Utiliites.WriteXYData(chromatogramRaw, ref errorlog);
            //Console.WriteLine("\tx\tY\t0 Buffer By Point Chromatogram"); Utiliites.WriteXYData(bufferedChromatogramPoint, ref errorlog);
            //Console.WriteLine("\tx\tY\t10 Buffered Chromatogram " + targetResult.ScanBoundsInfo.Buffer); Utiliites.WriteXYData(bufferedChromatogram, ref errorlog);

            //2.  simple clip to desired range (not smoothed yet)
            List<XYData> clippedRawChromatorgram = ChangeRange.ClipXyDataToScanRange(chromatogramRaw, targetResult.ScanBoundsInfo.Start, targetResult.ScanBoundsInfo.Stop);
            List<XYData> chromatogramRawBufferClipped = ChangeRange.ClipXyDataToScanRange(chromatogramRawBuffered, targetResult.ScanBoundsInfo.Start, targetResult.ScanBoundsInfo.Stop);
            Assert.AreEqual(clippedRawChromatorgram.Count, chromatogramRawBufferClipped.Count);
            //Assert.AreEqual(chromatogramRawClipped.Count, 70);
            Assert.AreEqual(81, chromatogramRawBufferClipped.Count);

            Assert.AreEqual(1847, chromatogramRawBufferClipped[69].X);
            Assert.AreEqual(rawValueAtScan1847, chromatogramRawBufferClipped[69].Y);
            Assert.AreEqual(1817, chromatogramRawBufferClipped[59].X);
            Assert.AreEqual(rawValueAtScan1817, chromatogramRawBufferClipped[59].Y);

            //Console.WriteLine("x\tY\tSimpleClip"); Utiliites.WriteXYData(clippedEicRawBuffered, ref errorlog);

            //3.  smooth buffered chromatorgram
            List<ProcessedPeak> chromatogramRawBufferedClippedAndSmoothed =_lcProcessor.Execute(chromatogramRawBuffered, targetResult.ScanBoundsInfo, EnumerationChromatogramProcessing.SmoothSection);
            List<XYData> chromatogramRawBufferedClippedAndSmoothedXY = ConvertProcessedPeakToXYData.ConvertPoints(chromatogramRawBufferedClippedAndSmoothed);
            //Assert.AreEqual(chromatogramRawBufferedClippedAndSmoothed.Count, 70);
            Assert.AreEqual(81, chromatogramRawBufferedClippedAndSmoothed.Count);
            //Console.WriteLine("\tx\tY\t10 Buffered SmoothedCipped " + targetResult.ScanBoundsInfo.Buffer); Utiliites.WriteXYData(chromatogramRawBufferedClippedAndSmoothedXY, ref errorlog);

            Assert.AreEqual(1847, chromatogramRawBufferedClippedAndSmoothedXY[69].X);
            Assert.AreEqual(smoothedValueAtScan1847, chromatogramRawBufferedClippedAndSmoothedXY[69].Y);
            Assert.AreEqual(1817, chromatogramRawBufferedClippedAndSmoothedXY[59].X);
            Assert.AreEqual(smoothedValueAtScan1817, chromatogramRawBufferedClippedAndSmoothedXY[59].Y);

            
            //check raw data
            Assert.AreEqual(1847, chromatogramRaw[590].X);
            Assert.AreEqual(rawValueAtScan1847, chromatogramRaw[590].Y);
            Assert.AreEqual(1817, chromatogramRaw[580].X);
            Assert.AreEqual(rawValueAtScan1817, chromatogramRaw[580].Y);

            //4.  smooth chromatorgram without buffering
            List<ProcessedPeak> chromatogramRawSmoothed = _lcProcessor.Execute(chromatogramRaw, targetResult.ScanBoundsInfo, EnumerationChromatogramProcessing.SmoothingOnly);

            //repeat generation
            //DeconTools.Backend.XYData deconChromatogram = _workflowParameters.LCParameters.Engine_PeakChromGenerator.GenerateChromatogram(runIn, targetResult.ScanBoundsInfo.Min, targetResult.ScanBoundsInfo.Max, mass, chromTollerence);
            chromatogramRaw = ConvertXYData.DeconXYDataToOmicsXYData(deconChromatogram);

            //check raw data
            Assert.AreEqual(1847, chromatogramRaw[590].X);
            Assert.AreEqual(rawValueAtScan1847, chromatogramRaw[590].Y);
            Assert.AreEqual(1817, chromatogramRaw[580].X);
            Assert.AreEqual(rawValueAtScan1817, chromatogramRaw[580].Y);

            Assert.AreEqual(1847, chromatogramRawSmoothed[590].XValue);
            Assert.AreEqual(smoothedValueAtScan1847, chromatogramRawSmoothed[590].Height);
            Assert.AreEqual(1817, chromatogramRawSmoothed[580].XValue);
            Assert.AreEqual(smoothedValueAtScan1817, chromatogramRawSmoothed[580].Height);
           
            //this should give different numbers because of edge effects.  we are using the clipped buffer
            List<ProcessedPeak> chromatogramRawSmoothedClipped = _lcProcessor.Execute(chromatogramRawBufferClipped, targetResult.ScanBoundsInfo, EnumerationChromatogramProcessing.SmoothingOnly);
            List<XYData> chromatogramRawClippedAndSmoothed = ConvertProcessedPeakToXYData.ConvertPoints(chromatogramRawSmoothedClipped);
            //Assert.AreEqual(chromatogramRawSmoothedClipped.Count, 70);
            Assert.AreEqual(81, chromatogramRawSmoothedClipped.Count);
            //Console.WriteLine("\tx\tY\t10 SmoothedOnlyClipped " + targetResult.ScanBoundsInfo.Buffer); Utiliites.WriteXYData(chromatogramRawClippedAndSmoothed, ref errorlog);

            Assert.AreEqual(1847, chromatogramRawClippedAndSmoothed[69].X);
            Assert.AreEqual(118169.2151505489d, chromatogramRawClippedAndSmoothed[69].Y);
            Assert.AreEqual(1817, chromatogramRawClippedAndSmoothed[59].X);
            Assert.AreEqual(574955.27896983549d, chromatogramRawClippedAndSmoothed[59].Y);

            //5.  test out correlator
            //CorrelationObject correlator = new CorrelationObject();
            //+buffer, smooth, trim
            //SavitzkyGolaySmoother savitzkyGolaySmoother = _workflowParameters.LCParameters.Engine_Smoother;
            //PNNLOmics.Algorithms.SpectralProcessing.SavitzkyGolaySmoother savitzkyGolaySmoother = _workflowParameters.LCParameters.Engine_Smoother;
            //PeakChromatogramGenerator enginePeakChromGenerator = _workflowParameters.LCParameters.Engine_PeakChromGenerator;
            //List<PNNLOmics.Data.XYData> fullEIC = correlator.GetBaseChromXYData(runIn, targetResult.ScanBoundsInfo, mass, ref enginePeakChromGenerator, ref  savitzkyGolaySmoother, _workflowParameters.LCParameters.ParametersChromGenerator.ChromToleranceInPPM);
            //List<XYData> fullEIC = ConvertXYData.DeconXYDataToOmicsXYData(oldEICMethod);
            //Console.WriteLine(Environment.NewLine + "x\tY\tBuffered and Smoothed Old");
            
            //Utiliites.WriteXYData(fullEIC, ref tuple);
           // Assert.AreEqual(fullEIC.Count, 70);



            //6.  test out new lc processing
            //1.  full chromatrogram
            //2.  smooth, centroid and threshold
            //3.  filter by points

            //check raw data
            Assert.AreEqual(1847, chromatogramRaw[590].X);
            Assert.AreEqual(rawValueAtScan1847, chromatogramRaw[590].Y);
            Assert.AreEqual(1817, chromatogramRaw[580].X);
            Assert.AreEqual(rawValueAtScan1817, chromatogramRaw[580].Y);

            NUnit.Framework.Assert.AreEqual(1847, chromatogramRaw[590].X);
            NUnit.Framework.Assert.AreEqual(148702.40625d, chromatogramRaw[590].Y);
            NUnit.Framework.Assert.AreEqual(1817, chromatogramRaw[580].X);
            NUnit.Framework.Assert.AreEqual(557311.1875d, chromatogramRaw[580].Y);

            List<XYData> pointsInVersio1Peak = new List<XYData>();
            for (int i = 580; i <= 601; i++)
            {
                pointsInVersio1Peak.Add(new XYData(chromatogramRawSmoothed[i].XValue, chromatogramRawSmoothed[i].Height));
            }
            string writeHere = _workflowParameters.LCParameters.XYDataWriterPath + @"\LC_Subset_Full.txt";
            writer.WriteOmicsXYData(pointsInVersio1Peak, writeHere);

            List<ProcessedPeak> LCProcessorOnChromatogramLevel = _lcProcessor.Execute(chromatogramRaw, targetResult.ScanBoundsInfo, EnumerationChromatogramProcessing.ChromatogramLevelUnitTest);
            List<XYData> LCProcessorOnChromatogramLevelXYPeaks = ConvertProcessedPeakToXYData.ConvertPoints(LCProcessorOnChromatogramLevel);
            
            //follow up important code
            List<XYData> LCProcessorOnChromatogramLevelXYPeaksClipped = ChangeRange.ClipXyDataToScanRange(LCProcessorOnChromatogramLevelXYPeaks, targetResult.ScanBoundsInfo, false);

            Assert.AreEqual(LCProcessorOnChromatogramLevelXYPeaksClipped.Count, 4);//4 why is this comming up as 3?
            //which is compared to this


            //repeat generation
            chromatogramRawBuffered = ConvertXYData.DeconXYDataToOmicsXYData(deconBufferedChromatogram);

            //check raw data
            Assert.AreEqual(1847, chromatogramRawBuffered[89].X);
            Assert.AreEqual(rawValueAtScan1847, chromatogramRawBuffered[89].Y);
            Assert.AreEqual(1817, chromatogramRawBuffered[79].X);
            Assert.AreEqual(rawValueAtScan1817, chromatogramRawBuffered[79].Y);

            //1.  Generate buffered chromatogram
            //2.  Smooth and clip in SmoothSection
            //3.  centroid and threshold
            //4.  filter by points
            List<ProcessedPeak> LCProcessorOnSmoothSection = _lcProcessor.Execute(chromatogramRawBuffered, targetResult.ScanBoundsInfo, EnumerationChromatogramProcessing.SmoothSection);
            List<XYData> LCProcessorOnSmoothSectionXY = ConvertProcessedPeakToXYData.ConvertPoints(LCProcessorOnSmoothSection);

            Assert.AreEqual(1847, LCProcessorOnSmoothSection[69].XValue);
            Assert.AreEqual(smoothedValueAtScan1847, LCProcessorOnSmoothSection[69].Height);

            string writeHere3 = _workflowParameters.LCParameters.XYDataWriterPath + @"\LC_Subset_BufferedAll.txt";
            writer.WriteOmicsXYData(LCProcessorOnSmoothSectionXY, writeHere3);

            List<XYData> pointsInVersio2Peak = new List<XYData>();
            for (int i = 59; i <= 80; i++)
            {
                pointsInVersio2Peak.Add(new XYData(LCProcessorOnSmoothSectionXY[i].X, LCProcessorOnSmoothSectionXY[i].Y));
            }
            string writeHere2 = _workflowParameters.LCParameters.XYDataWriterPath + @"\LC_Subset_Buffered.txt";
            writer.WriteOmicsXYData(pointsInVersio2Peak, writeHere2);

            //List<XYData> LCProcessorOnSmoothSectionXY = ConvertProcessedPeakToXYData.ConvertPoints(LCProcessorOnSmoothSectionXY);

            List<ProcessedPeak> LCProcessorOnSmoothSectionPeaks = _lcProcessor.Execute(LCProcessorOnSmoothSectionXY, targetResult.ScanBoundsInfo, EnumerationChromatogramProcessing.LCPeakDetectOnly);

            List<ProcessedPeak> LCProcessorOnSmoothSectionPeaksFiltered = Utiliites.FilterByPointsPerSide(LCProcessorOnSmoothSectionPeaks, _workflowParameters.LCParameters.PointsPerShoulder);

            Assert.AreEqual(LCProcessorOnSmoothSectionPeaksFiltered.Count, 4);//4  why is this comming up as 3?

            Console.WriteLine(Environment.NewLine + "x\tY\tSmoothedNew");
            Tuple<string, string> errorLog = errorlog;
            Utiliites.WriteXYData(LCProcessorOnChromatogramLevelXYPeaks, ref errorLog);

            

            Console.WriteLine(Environment.NewLine + "Test clipped with buffered");
            Assert.AreEqual(LCProcessorOnChromatogramLevelXYPeaksClipped.Count, LCProcessorOnSmoothSectionPeaksFiltered.Count);

            double sum = 0;
            for (int i = 0; i < LCProcessorOnChromatogramLevelXYPeaks.Count; i++)
            {
                //sum += (fullEIC[i].Y - newMethodEIC[i].Y);
            }
            Assert.AreEqual(0, sum);

            int iterator = 50;

            Console.WriteLine(Environment.NewLine + "Speed Testing 1/2 GetBaseChromXYData");
            DateTime starttime;
            Stopwatch stopWatch = StartClock(out starttime);
            for (int i = 0; i < iterator; i++)
            {

                //DeconTools.Backend.XYData oldEICMethodInfinity = correlator.GetBaseChromXYData(runIn, targetResult.ScanBoundsInfo, mass, ref enginePeakChromGenerator, ref  savitzkyGolaySmoother, _workflowParameters.LCParameters.ParametersChromGenerator.ChromToleranceInPPM);
                //List<PNNLOmics.Data.XYData> oldEICMethodInfinity = correlator.GetBaseChromXYData(runIn, targetResult.ScanBoundsInfo, mass, ref enginePeakChromGenerator, ref  savitzkyGolaySmoother, _workflowParameters.LCParameters.ParametersChromGenerator.ChromToleranceInPPM);
           
            }

            StopClock(starttime, stopWatch);

            Console.WriteLine("Speed Testing 2/2 LcProcessor.GenerateBufferedChromatogramByPoint");

            //DateTime starttime;
            stopWatch = StartClock(out starttime);
            for (int i = 0; i < iterator; i++)
            {
                Run32.Backend.Data.XYData deconBufferedChromatogramInfinity = _lcProcessor.GenerateBufferedChromatogramByPoint(runIn, targetResult.ScanBoundsInfo, mass);//_workflowParameters.LCParameters.ParametersChromGenerator.ChromToleranceInPPM
                List<XYData> chromatogramRawBufferedInfinity = ConvertXYData.DeconXYDataToOmicsXYData(deconBufferedChromatogramInfinity);
                List<ProcessedPeak> newMethodInfinity = _lcProcessor.Execute(chromatogramRawBufferedInfinity, targetResult.ScanBoundsInfo, EnumerationChromatogramProcessing.SmoothSection);

            }

            StopClock(starttime, stopWatch);
        }

        [Test]
        public void TestProcessTarget()
        {
            PopulateGlobalVariables();

            
            //TEST CODE
            List<PNNLOmics.Data.XYData> fragmentEicFitXYData; //this is key XYdata for fitting

            FragmentResultsObjectHolderIq fragmentResultsObjectHolderIq = targetResult;
            MSGenerator msGenerator = _workflowParameters.MSParameters.Engine_msGenerator;
            IterativeTFF msfeatureFinder = _msfeatureFinder;
            TaskIQ isotopicProfileFitScoreCalculator = _fitScoreCalc;
            Tuple<string, string> fitScoreCalculator = errorlog;
            ProcessorChromatogram processorChromatogram = _lcProcessor;
            ProcessorMassSpectra processorMassSpectra = _msProcessor;

            //ProcessedPeak fragmentCandiateFitPeak = ProcessTarget.Process(
            //    possibleFragmentTarget, 
            //    ref fragmentResultsObjectHolderIq, 
            //    runIn,
                
            //    iQresult, 
            //    ref msGenerator,
            //    ref msfeatureFinder, 
            //    ref isotopicProfileFitScoreCalculator, 
            //    ref fitScoreCalculator, printString, 
            //    out fragmentEicFitXYData, 
            //    _workflowParameters.FitScoreCuttoff, 
            //    _workflowParameters.MSToleranceInPPM, 
            //    ref processorChromatogram, 
            //    _workflowParameters);

            
            ProcessedPeak fragmentCandiateFitPeak = ProcessTarget.Process(
                possibleFragmentTarget,
                ref fragmentResultsObjectHolderIq,
                runIn,
                iQresult,
                ref msfeatureFinder,
                ref fitScoreCalculator, printString,
                out fragmentEicFitXYData,
                _workflowParameters.FitScoreCuttoff,
                _workflowParameters.MSToleranceInPPM,
                ref processorChromatogram,
                ref processorMassSpectra,
                _workflowParameters);
            //ProcessedPeak fragmentCandiateFitPeak = Utiliites.ProcessTargetIQ(scans.Buffer, scans.ScansToSum, runIn, iQresult, _workflowParameters.ChromGenTolerance, _workflowParameters.MinRelativeIntensityForChromCorrelator, ref omicsPeakDetection, ref _msGenerator, ref _smoother, ref _peakChromGen, ref errorlog, printString, possibleFragmentTarget, out fragmentEicFitXYData);

            Assert.AreEqual(2, possibleFragmentTarget.ChargeState);
            Assert.AreEqual(101, fragmentEicFitXYData.Count);//new way.  old was 9
            Assert.AreEqual(101, fragmentEicFitXYData.Count);//new way.  odd was 10

            bool buffered = true;
            if (buffered)
            {
                Assert.AreEqual(153.06692771811356d, fragmentEicFitXYData[0].Y); //new way.  odd was 8428195.549835436d
                Assert.AreEqual(18238990.0d, fragmentCandiateFitPeak.Height); //new way.  odd was 18235522.0d
                Assert.AreEqual(1640, targetResult.ScanBoundsInfo.Start);
                Assert.AreEqual(1684, targetResult.ScanBoundsInfo.Stop);
            }
            else
            {
                Assert.AreEqual(0.42649194650064998d, fragmentEicFitXYData[0].Y);//new way with buffering.  odd was 8428195.549835436d
                Assert.AreEqual(19481978.0d, fragmentCandiateFitPeak.Height);//new way with buffering.  odd was 18235522.0d 
                Assert.AreEqual(1644, targetResult.ScanBoundsInfo.Start);
                Assert.AreEqual(1680, targetResult.ScanBoundsInfo.Stop);
            }
            Assert.AreEqual(862.30888177738859d, targetResult.Primary_Observed_IsotopeProfile.MonoPeakMZ);
            Assert.AreEqual(862.30888177738859d, iQresult.ObservedIsotopicProfile.MonoPeakMZ);
        }

        [Test]
        public void TestSmoothers()
        {
            PopulateGlobalVariables();

            string testFile = "";
            //testFile = @"E:\ScottK\GetPeaks Data\Diabetes_LC\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw";
            //testFile = @"E:\PNNL Data\2012_09_05 SPIN Q-TOF\Gly08_SQTOF_SP02_3X_C1_12_HPIF20Torr_LPRF96_T160_6Sept12.d";

            //testFile = @"E:\PNNL Data\2012_09_05 SPIN Q-TOF\Gly08_SQTOF_SL26_100_C2_13_HPIF30Torr_LPRF96_T160_18Sept12.d";//work
            //testFile = @"L:\PNNL Files\PNNL Data for Tests\Gly08_SQTOF_SL26_100_C2_13_HPIF30Torr_LPRF96_T160_18Sept12.d";//home
            testFile = massSpecFilePath;
            RunFactory factor = new RunFactory();
            Run runIn = factor.CreateRun(Globals.MSFileType.Agilent_D, testFile);

            ScanObject scanInfo = new ScanObject(2615, 2684);
            //possibleFragmentTarget.ScanInfo.Start = 1648;
            //possibleFragmentTarget.ScanInfo.Stop = 1674;
            scanInfo.Max = runIn.MaxLCScan;
            scanInfo.Min = runIn.MinLCScan;
            scanInfo.ScansToSum = 51;
            scanInfo.Buffer = 9*2;

            runIn.ScanSetCollection.Create(runIn, scanInfo.Start, scanInfo.Stop, scanInfo.ScansToSum, 1, false);

            IqResult iQresult = new IqResult(new IqTargetBasic());

            //MSGenerator _msGenerator = _workflowParameters.MSParameters.Engine_msGenerator;

            iQresult.LCScanSetSelected = Utiliites.ScanSetFromStartStop(runIn, scanInfo);

            
            int smoothPoints = 9;

            string rawPath = _workflowParameters.LCParameters.XYDataWriterPath + @"\MS_0_RawPreSmoothing.txt";
            string smoothedpathOmics = _workflowParameters.LCParameters.XYDataWriterPath + @"\MS_2_SmoothedOmics.txt";
            string smoothedPathWrappedDecon = _workflowParameters.LCParameters.XYDataWriterPath + @"\MS_2_SmoothedWrDecon.txt";
            string smoothedPathPureDecon = _workflowParameters.LCParameters.XYDataWriterPath + @"\MS_2_SmoothedPureDecon.txt";
           
            DataXYDataWriter writer = new DataXYDataWriter();

            
            //0.  get spectra.  raw untouched summed data
            //iQresult.IqResultDetail.MassSpectrum = _msGenerator.GenerateMS(runIn, iQresult.LCScanSetSelected);
            iQresult.IqResultDetail.MassSpectrum = _msProcessor.DeconMSGeneratorWrapper(runIn, iQresult.LCScanSetSelected);
            List<XYData> convertedMassSpectrum = ConvertXYData.DeconXYDataToOmicsXYData(iQresult.IqResultDetail.MassSpectrum);


            writer.WriteDeconXYDataDeconTools(iQresult.IqResultDetail.MassSpectrum, rawPath);
            Assert.AreEqual(258907, iQresult.IqResultDetail.MassSpectrum.Xvalues.Length);


            bool pureDeconTrueOrFalse = true;
            //TEST CODE

            //1.  test Omics smoother
            pureDeconTrueOrFalse = false;
            PNNLOmics.Algorithms.SpectralProcessing.SavitzkyGolaySmoother smoootherOmics = new PNNLOmics.Algorithms.SpectralProcessing.SavitzkyGolaySmoother(smoothPoints, 2, false);
            List<XYData> smoothedDataOmics = smoootherOmics.Smooth(convertedMassSpectrum);//this is more accurate
            writer.WriteOmicsXYData(smoothedDataOmics, smoothedpathOmics);
            Assert.AreEqual(258907, smoothedDataOmics.Count);
            Assert.AreEqual(5.8831168831168847d, smoothedDataOmics[4000].Y);
            Assert.AreEqual(4.6515151515151523d, smoothedDataOmics[3].Y);
            Assert.AreEqual(4.2588744588744589d, smoothedDataOmics[smoothedDataOmics.Count - 3].Y);

            ////3.  SG smoother pure decon
            SavitzkyGolaySmoother deconSmoother = new SavitzkyGolaySmoother(smoothPoints, 2, false);
            Run32.Backend.Data.XYData smoothedResultsPureDecon = deconSmoother.Smooth(iQresult.IqResultDetail.MassSpectrum);
            writer.WriteDeconXYDataDeconTools(smoothedResultsPureDecon, smoothedPathPureDecon);
            Assert.AreEqual(5.8831168831168847d, smoothedResultsPureDecon.Yvalues[4000]);//this is the most correct number
            Assert.AreEqual(4.6515151515151523d, smoothedResultsPureDecon.Yvalues[3]);
            Assert.AreEqual(4.2588744588744589d, smoothedResultsPureDecon.Yvalues[smoothedDataOmics.Count - 3]);

            //4.  speed testing

            int iterator = 5;

            Console.WriteLine(Environment.NewLine + "Speed Testing 1/3 Smoothers, Decon only");

            DateTime starttime;
            Stopwatch stopWatch = StartClock(out starttime);
            for (int i = 0; i < iterator; i++)//3.78 for 500
            {
                Run32.Backend.Data.XYData chromatogramDeconProcessedInfinity = deconSmoother.Smooth(iQresult.IqResultDetail.MassSpectrum);
                chromatogramDeconProcessedInfinity.Yvalues[45] = 700;
            }

            StopClock(starttime, stopWatch);


            Console.WriteLine("Speed Testing 2/3 Smoothers. omics list<xydata>");

            //DateTime starttime;
            stopWatch = StartClock(out starttime);
            for (int i = 0; i < iterator; i++)//14.77 for 500
            {
                //List<XYData> smoothedDataOmicsWrappedInfinity = ProcessorChromatogram.SmoothWrapper(convertedMassSpectrum);
                pureDeconTrueOrFalse = false;
                List<XYData> smoothedDataOmicsInfinity = smoootherOmics.Smooth(convertedMassSpectrum);
                smoothedDataOmicsInfinity[45].Y = 700;
            }

            StopClock(starttime, stopWatch);

            Console.WriteLine(Environment.NewLine + "Speed Testing 3/3 Smoothers, Omics-->Decon-->Omics");

            //DateTime starttime;
            stopWatch = StartClock(out starttime);
            for (int i = 0; i < iterator; i++)//20.7 for 500
            {
                Run32.Backend.Data.XYData chromatogramDeconProcessed = ConvertXYData.OmicsXYDataToDeconXYData(convertedMassSpectrum);
                Run32.Backend.Data.XYData chromatogramDeconProcessedInfinity = deconSmoother.Smooth(chromatogramDeconProcessed);
                List<XYData> chromatogramProcessedInfinity = ConvertXYData.DeconXYDataToOmicsXYData(chromatogramDeconProcessedInfinity);
                chromatogramProcessedInfinity[45].Y = 700;
            }

            StopClock(starttime, stopWatch);

            Console.WriteLine(Environment.NewLine + "Speed Testing 3/4 Smoothers, get peaks Processor");

            //DateTime starttime;
            stopWatch = StartClock(out starttime);
            for (int i = 0; i < iterator; i++)//22.11 for 500
            {
                List<XYData> convertedMassSpectrumProcessor = ConvertXYData.DeconXYDataToOmicsXYData(iQresult.IqResultDetail.MassSpectrum);
                List<ProcessedPeak> chromatogramProcessedInfinity = _lcProcessor.Execute(convertedMassSpectrumProcessor, EnumerationChromatogramProcessing.SmoothingOnly);
                chromatogramProcessedInfinity[45].Height = 700;
            }

            StopClock(starttime, stopWatch);

        }

        [Test]
        public void TestSmoothingMS()
        {
            PopulateGlobalVariables();

            string testFile = "";
            //testFile = @"E:\ScottK\GetPeaks Data\Diabetes_LC\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw";
            //testFile = @"E:\PNNL Data\2012_09_05 SPIN Q-TOF\Gly08_SQTOF_SP02_3X_C1_12_HPIF20Torr_LPRF96_T160_6Sept12.d";

            //testFile = @"E:\PNNL Data\2012_09_05 SPIN Q-TOF\Gly08_SQTOF_SL26_100_C2_13_HPIF30Torr_LPRF96_T160_18Sept12.d";//work
            //testFile = @"L:\PNNL Files\PNNL Data for Tests\Gly08_SQTOF_SL26_100_C2_13_HPIF30Torr_LPRF96_T160_18Sept12.d";//home
            testFile = massSpecFilePath;
            RunFactory factor = new RunFactory();
            Run runIn = factor.CreateRun(Globals.MSFileType.Agilent_D, testFile);

            ScanObject scanInfo = new ScanObject(2615, 2684);
            //possibleFragmentTarget.ScanInfo.Start = 1648;
            //possibleFragmentTarget.ScanInfo.Stop = 1674;
            scanInfo.Max = runIn.MaxLCScan;
            scanInfo.Min = runIn.MinLCScan;
            scanInfo.ScansToSum = 51;
            scanInfo.Buffer = 9 * 2;

            runIn.ScanSetCollection.Create(runIn, scanInfo.Start, scanInfo.Stop, scanInfo.ScansToSum, 1, false);

            IqResult iQresult = new IqResult(new IqTargetBasic());

            //MSGenerator _msGenerator = _workflowParameters.MSParameters.Engine_msGenerator;

            iQresult.LCScanSetSelected = Utiliites.ScanSetFromStartStop(runIn, scanInfo);


            int smoothPoints = 9;

            string rawPath = _workflowParameters.LCParameters.XYDataWriterPath + @"\MS_0_RawPreSmoothing.txt";
            string smoothedpathOmics = _workflowParameters.LCParameters.XYDataWriterPath + @"\MS_2_SmoothedOmics.txt";
            string smoothedPathWrappedDecon = _workflowParameters.LCParameters.XYDataWriterPath + @"\MS_2_SmoothedWrDecon.txt";
            string smoothedPathPureDecon = _workflowParameters.LCParameters.XYDataWriterPath + @"\MS_2_SmoothedPureDecon.txt";

            DataXYDataWriter writer = new DataXYDataWriter();


            //0.  get spectra.  raw untouched summed data
            iQresult.IqResultDetail.MassSpectrum = _msProcessor.DeconMSGeneratorWrapper(runIn, iQresult.LCScanSetSelected);
            List<XYData> convertedMassSpectrum = ConvertXYData.DeconXYDataToOmicsXYData(iQresult.IqResultDetail.MassSpectrum);


            writer.WriteDeconXYDataDeconTools(iQresult.IqResultDetail.MassSpectrum, rawPath);
            Assert.AreEqual(258907, iQresult.IqResultDetail.MassSpectrum.Xvalues.Length);


            bool pureDeconTrueOrFalse = true;
            //TEST CODE

            //1.  test Omics smoother
            pureDeconTrueOrFalse = false;
            PNNLOmics.Algorithms.SpectralProcessing.SavitzkyGolaySmoother smoootherOmics = new PNNLOmics.Algorithms.SpectralProcessing.SavitzkyGolaySmoother(smoothPoints, 2, false);
            List<XYData> smoothedDataOmics = smoootherOmics.Smooth(convertedMassSpectrum);//this is more accurate
            writer.WriteOmicsXYData(smoothedDataOmics, smoothedpathOmics);
            Assert.AreEqual(258907, smoothedDataOmics.Count);
            Assert.AreEqual(5.8831168831168847d, smoothedDataOmics[4000].Y);
            Assert.AreEqual(4.6515151515151523d, smoothedDataOmics[3].Y);
            Assert.AreEqual(4.2588744588744589d, smoothedDataOmics[smoothedDataOmics.Count - 3].Y);

            ////3.  SG smoother pure decon
            SavitzkyGolaySmoother deconSmoother = new SavitzkyGolaySmoother(smoothPoints, 2, false);
            Run32.Backend.Data.XYData smoothedResultsPureDecon = deconSmoother.Smooth(iQresult.IqResultDetail.MassSpectrum);
            writer.WriteDeconXYDataDeconTools(smoothedResultsPureDecon, smoothedPathPureDecon);
            Assert.AreEqual(5.8831168831168847d, smoothedResultsPureDecon.Yvalues[4000]);//this is the most correct number
            Assert.AreEqual(4.6515151515151523d, smoothedResultsPureDecon.Yvalues[3]);
            Assert.AreEqual(4.2588744588744589d, smoothedResultsPureDecon.Yvalues[smoothedDataOmics.Count - 3]);

           

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
            List<XYData> convertedMassSpectrum = ConvertXYData.DeconXYDataToOmicsXYData(iQresult.IqResultDetail.MassSpectrum);


            writer.WriteDeconXYDataDeconTools(iQresult.IqResultDetail.MassSpectrum, rawPath);
            Assert.AreEqual(56963, iQresult.IqResultDetail.MassSpectrum.Xvalues.Length);

            List<ProcessedPeak> centroidOnly =  _msProcessor.Execute(convertedMassSpectrum, EnumerationMassSpectraProcessing.OmicsCentroid_Only);

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
                Assert.AreEqual(toHammer[i].XValue, toHammerV2[i].XValue,0.01);
            }

            HammerPeakDetector.Objects.HammerThresholdResults resultsOptimize = new HammerThresholdResults(hammerParameters);
            List<ProcessedPeak> results = HammerPeakDetector.Hammer.Detect(toHammer, hammerParameters,metricsFolder,printMetrics, out resultsOptimize);

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


        [Test]
        public void peakWriter()
        {
            string fileName = @"P:\test.txt";
            int scan = 2615;
            List<PNNLOmics.Data.Peak> toHammerV2 = LoadPeaksForHammer.Load();

            IPeakWriter writer = new DataPeaksDataWriter();
            writer.WriteOmicsPeakData(toHammerV2, scan, fileName);
            
        }

        [Test]
        public void TestSmoothingLC()
        {
            PopulateGlobalVariables();
            Tuple<string, string> tuple = errorlog;

            targetResult.ScanBoundsInfo.Start = 0;
            targetResult.ScanBoundsInfo.Stop = 4000;
            targetResult.ScanBoundsInfo.Buffer = 20;
            double chromTollerence = 10;
            //_workflowParameters.LCParameters.ParametersChromGenerator.ChromToleranceInPPM = chromTollerence;
            _workflowParameters.LCParameters.Engine_PeakChromGenerator = new PeakChromatogramGenerator(chromTollerence);
            double mass = 862.3110;

            int smoothPoints = 5;
            int pointsPerShoulder = 2;


            //0.  generatre Chromatorgrams
            //DeconTools.Backend.XYData deconChromatogram = _workflowParameters.LCParameters.Engine_PeakChromGenerator.GenerateChromatogram(runIn, targetResult.ScanBoundsInfo.Start,targetResult.ScanBoundsInfo.Stop, mass, _workflowParameters.LCParameters.ParametersChromGenerator.ChromToleranceInPPM);
            //DeconTools.Backend.XYData deconChromatogram = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(runIn, mass, targetResult.ScanBoundsInfo, _workflowParameters.LCParameters.ParametersChromGenerator.ChromToleranceInPPM);//old
            Run32.Backend.Data.XYData deconChromatogram = ProcessorChromatogram.DeconChromatogramGeneratorWrapper(runIn, mass, targetResult.ScanBoundsInfo);
            
            List<XYData> bufferedchromatogramRaw = ConvertXYData.DeconXYDataToOmicsXYData(deconChromatogram);
            Assert.AreEqual(719, bufferedchromatogramRaw.Count);
            Assert.AreEqual(77578.8984375d, bufferedchromatogramRaw[161].Y);

            string rawPath = _workflowParameters.LCParameters.XYDataWriterPath + @"\LC_0_RawPreSmooth_0.txt";
            string smoothedpathOmics = _workflowParameters.LCParameters.XYDataWriterPath + @"\LC_2_SmoothOmics";
            string smoothedpathOmicsFinal = _workflowParameters.LCParameters.XYDataWriterPath + @"\LC_2_SmoothOmicCalc";
            string peakspathOmicsFinal = _workflowParameters.LCParameters.XYDataWriterPath + @"\MS_4_PeaksCalc";
            string peaksPathOmics = _workflowParameters.LCParameters.XYDataWriterPath + @"\LC_4_Peaks";
            
            DataXYDataWriter writer = new DataXYDataWriter();

            writer.WriteOmicsXYData(bufferedchromatogramRaw, rawPath);


            _workflowParameters.LCParameters.Engine_OmicsPeakDetection.Parameters.FWHMPeakFitType = PeakFitType.Parabola;
            List<ProcessedPeak> peaksOmicsRaw = _workflowParameters.LCParameters.Engine_OmicsPeakDetection.DiscoverPeaks(bufferedchromatogramRaw);
            
            List<ProcessedPeak> peaksOmicsRaw2 = _lcProcessor.Execute(bufferedchromatogramRaw, EnumerationChromatogramProcessing.LCPeakDetectOnly);

            //check direct and manual give the same peaks
            Assert.AreEqual(peaksOmicsRaw.Count, peaksOmicsRaw2.Count);
            for (int i = 0; i < peaksOmicsRaw.Count;i++ )
            {
                Assert.AreEqual(peaksOmicsRaw[i].XValue,peaksOmicsRaw2[i].XValue);
            }
            
            List<ProcessedPeak> filteredPeaksRaw = Utiliites.FilterByPointsPerSide(peaksOmicsRaw, 1);
            //writer.WriteOmicsProcesedPeakData(filteredPeaksRaw, peaksPathOmics + "_" + smoothPoints + ".txt");

            ProcessedPeak examplePeak;
            
            int range;


            //I am not sure what happended to this peak
            examplePeak = (from peak in filteredPeaksRaw where peak.XValue > 2020 && peak.XValue < 2050 select peak).FirstOrDefault();
            range = examplePeak.MinimaOfHigherMassIndex - examplePeak.MinimaOfLowerMassIndex;
            Console.WriteLine(1 + " points " + examplePeak.XValue + " has a min index is " + examplePeak.MinimaOfLowerMassIndex + " max index is " + examplePeak.MinimaOfHigherMassIndex + " and range is " + range + " 5 points raw");

            examplePeak = (from peak in filteredPeaksRaw where peak.XValue > 1920 && peak.XValue < 1960 select peak).FirstOrDefault();
            range = examplePeak.MinimaOfHigherMassIndex - examplePeak.MinimaOfLowerMassIndex;
            Console.WriteLine(1 + " points " + examplePeak.XValue + " has a min index is " + examplePeak.MinimaOfLowerMassIndex + " max index is " + examplePeak.MinimaOfHigherMassIndex + " and range is " + range + " Raw");

            examplePeak = (from peak in filteredPeaksRaw where peak.XValue > 1600 && peak.XValue < 1700 select peak).FirstOrDefault();
            range = examplePeak.MinimaOfHigherMassIndex - examplePeak.MinimaOfLowerMassIndex;
            Console.WriteLine(1 + " points " + examplePeak.XValue + " has a min index is " + examplePeak.MinimaOfLowerMassIndex + " max index is " + examplePeak.MinimaOfHigherMassIndex + " and range is " + range + " Raw");

            examplePeak = (from peak in filteredPeaksRaw where peak.XValue > 680 && peak.XValue < 710 select peak).FirstOrDefault();
            range = examplePeak.MinimaOfHigherMassIndex - examplePeak.MinimaOfLowerMassIndex;
            Console.WriteLine(1 + " points " + examplePeak.XValue + " has a min index is " + examplePeak.MinimaOfLowerMassIndex + " max index is " + examplePeak.MinimaOfHigherMassIndex + " and range is " + range + " Raw");

            examplePeak = (from peak in filteredPeaksRaw where peak.XValue > 910 && peak.XValue < 940 select peak).FirstOrDefault();
            range = examplePeak.MinimaOfHigherMassIndex - examplePeak.MinimaOfLowerMassIndex;
            Console.WriteLine(1 + " points " + examplePeak.XValue + " has a min index is " + examplePeak.MinimaOfLowerMassIndex + " max index is " + examplePeak.MinimaOfHigherMassIndex + " and range is " + range + " 3 point Raw");


            examplePeak = (from peak in filteredPeaksRaw where peak.XValue > 1780 && peak.XValue < 1800 select peak).FirstOrDefault();
            range = examplePeak.MinimaOfHigherMassIndex - examplePeak.MinimaOfLowerMassIndex;
            Console.WriteLine(1 + " points " + examplePeak.XValue + " has a min index is " + examplePeak.MinimaOfLowerMassIndex + " max index is " + examplePeak.MinimaOfHigherMassIndex + " and range is " + range + " Raw");

            Console.WriteLine(Environment.NewLine);
            //TEST CODE

            //1.  test Omics smoother

            

            for (int i = 5; i < 20; i += 2)
            {
                smoothPoints = i;
                PNNLOmics.Algorithms.SpectralProcessing.SavitzkyGolaySmoother smoootherOmicsPre = new PNNLOmics.Algorithms.SpectralProcessing.SavitzkyGolaySmoother(3, 2, false);

                bufferedchromatogramRaw = ConvertXYData.DeconXYDataToOmicsXYData(deconChromatogram);
                Assert.AreEqual(77578.8984375d, bufferedchromatogramRaw[161].Y);

                bool extraSmooth = false;
                List<XYData> presmoothedsmoothedDataOmics = new List<XYData>();
                if(extraSmooth)
                {
                    presmoothedsmoothedDataOmics = smoootherOmicsPre.Smooth(bufferedchromatogramRaw); //this is more accurate
                }
                else
                {
                    presmoothedsmoothedDataOmics = bufferedchromatogramRaw;
                }


                PNNLOmics.Algorithms.SpectralProcessing.SavitzkyGolaySmoother smoootherOmics = new PNNLOmics.Algorithms.SpectralProcessing.SavitzkyGolaySmoother(smoothPoints, 2, false);
                List<XYData> smoothedDataOmics = smoootherOmics.Smooth(presmoothedsmoothedDataOmics); //this is more accurate
                writer.WriteOmicsXYData(smoothedDataOmics, smoothedpathOmics + "_" + smoothPoints + ".txt");

                //List<ProcessedPeak> peaksOmics = _lcProcessor.Execute(smoothedDataOmics, EnumerationChromatogramProcessing.LCPeakDetectOnly);
                List<ProcessedPeak> peaksOmics = _workflowParameters.LCParameters.Engine_OmicsPeakDetection.DiscoverPeaks(smoothedDataOmics);
            
                
                List<ProcessedPeak> filteredPeaks = Utiliites.FilterByPointsPerSide(peaksOmics, pointsPerShoulder);
                //writer.WriteOmicsProcesedPeakData(filteredPeaks, peaksPathOmics + "_" + smoothPoints + ".txt");

                examplePeak = (from peak in filteredPeaks where peak.XValue > 2020 && peak.XValue < 2050 select peak).FirstOrDefault();
                range = examplePeak.MinimaOfHigherMassIndex - examplePeak.MinimaOfLowerMassIndex;
                Console.WriteLine(smoothPoints + " points " + examplePeak.XValue + " has a min index is " + examplePeak.MinimaOfLowerMassIndex + " max index is " + examplePeak.MinimaOfHigherMassIndex + " and range is " + range + " 5 points raw");

                examplePeak = (from peak in filteredPeaks where peak.XValue >1930 && peak.XValue < 2050 select peak).FirstOrDefault();
                range = examplePeak.MinimaOfHigherMassIndex - examplePeak.MinimaOfLowerMassIndex;
                Console.WriteLine(smoothPoints + " points " + examplePeak.XValue + " has a min index is " + examplePeak.MinimaOfLowerMassIndex + " max index is " + examplePeak.MinimaOfHigherMassIndex + " and range is " + range);

                examplePeak = (from peak in filteredPeaks where peak.XValue > 1600 && peak.XValue < 1700 select peak).FirstOrDefault();
                range = examplePeak.MinimaOfHigherMassIndex - examplePeak.MinimaOfLowerMassIndex;
                Console.WriteLine(smoothPoints + " points " + examplePeak.XValue + " has a min index is " + examplePeak.MinimaOfLowerMassIndex + " max index is " + examplePeak.MinimaOfHigherMassIndex + " and range is " + range);

                examplePeak = (from peak in filteredPeaks where peak.XValue > 680 && peak.XValue < 710 select peak).FirstOrDefault();
                range = examplePeak.MinimaOfHigherMassIndex - examplePeak.MinimaOfLowerMassIndex;
                Console.WriteLine(smoothPoints + " points " + examplePeak.XValue + " has a min index is " + examplePeak.MinimaOfLowerMassIndex + " max index is " + examplePeak.MinimaOfHigherMassIndex + " and range is " + range);

                examplePeak = (from peak in filteredPeaks where peak.XValue > 910 && peak.XValue < 940 select peak).FirstOrDefault();
                range = examplePeak.MinimaOfHigherMassIndex - examplePeak.MinimaOfLowerMassIndex;
                Console.WriteLine(smoothPoints + " points " + examplePeak.XValue + " has a min index is " + examplePeak.MinimaOfLowerMassIndex + " max index is " + examplePeak.MinimaOfHigherMassIndex + " and range is " + range);

                examplePeak = (from peak in filteredPeaks where peak.XValue > 1750 && peak.XValue < 1800 select peak).FirstOrDefault();
                range = examplePeak.MinimaOfHigherMassIndex - examplePeak.MinimaOfLowerMassIndex;
                Console.WriteLine(smoothPoints + " points " + examplePeak.XValue + " has a min index is " + examplePeak.MinimaOfLowerMassIndex + " max index is " + examplePeak.MinimaOfHigherMassIndex + " and range is " + range);

                Console.WriteLine("for smoothing " + smoothPoints + " points we can detect " + filteredPeaks.Count + "/6 peaks");
            }

            

            


            for (int i = 5; i < 20; i += 2)
            {
                bufferedchromatogramRaw = ConvertXYData.DeconXYDataToOmicsXYData(deconChromatogram);
                Assert.AreEqual(77578.8984375d, bufferedchromatogramRaw[161].Y);
                //calculated answer
                smoothPoints = i;
                //_workflowParameters.LCParameters.PointsPerShoulder = _workflowParameters.LCParameters.CalculatePointsPerShoulderAsAFunctionOfSgPoints(_workflowParameters);
                _workflowParameters.ChromSmootherNumPointsInSmooth = smoothPoints;
                int calclulatedPointsPerShoulder = _workflowParameters.LCParameters.CalculatePointsPerShoulderAsAFunctionOfSgPoints(_workflowParameters);
                Console.WriteLine("For SG points " + i + " we require atleast or equal >= " + calclulatedPointsPerShoulder + " points");
                PNNLOmics.Algorithms.SpectralProcessing.SavitzkyGolaySmoother smoootherOmicsFinal = new PNNLOmics.Algorithms.SpectralProcessing.SavitzkyGolaySmoother(smoothPoints, 2, false);
                List<XYData> smoothedDataOmicsFinal = smoootherOmicsFinal.Smooth(bufferedchromatogramRaw);
                    //this is more accurate
                writer.WriteOmicsXYData(smoothedDataOmicsFinal, smoothedpathOmicsFinal + "_" + smoothPoints + ".txt");

                List<ProcessedPeak> peaksOmicsFinal = _lcProcessor.Execute(smoothedDataOmicsFinal, EnumerationChromatogramProcessing.LCPeakDetectOnly);
                List<ProcessedPeak> filteredPeaksFinal = Utiliites.FilterByPointsPerSide(peaksOmicsFinal, calclulatedPointsPerShoulder);
                writer.WriteOmicsProcesedPeakData(filteredPeaksFinal, peakspathOmicsFinal + "_" + smoothPoints + ".txt");
            }


        }

        [Test]
        ///This calculates a raw peak with one or 5 points.  then smoothds it 20-80 times and writes the data
        public void TestSmoothingLCSynthetic()
        {
            PopulateGlobalVariables();
            Tuple<string, string> tuple = errorlog;

            int smoothrange = 10;//80

            targetResult.ScanBoundsInfo.Start = 0;
            targetResult.ScanBoundsInfo.Stop = 4000;
            targetResult.ScanBoundsInfo.Buffer = 20;
            double chromTollerence = 10;
            _workflowParameters.LCParameters.ParametersChromGenerator.ChromToleranceInPPM = chromTollerence;

            double mass = 862.3110;

            int smoothPoints = 5;
            int pointsPerShoulder = 2;


            //0.  generatre Chromatorgrams
            //List<XYData> bufferedchromatogramRaw = GenerateXyDataWithZeroes(); ;//step1 of 3
            List<XYData> bufferedchromatogramRaw = GenerateFiveXyDataWithZeroes();//step1 of 3
            Assert.AreEqual(1099, bufferedchromatogramRaw.Count);
            Assert.AreEqual(256, bufferedchromatogramRaw[249].Y);//3 point
            //Assert.AreEqual(128.0000192086062d, bufferedchromatogramRaw[248].Y);//5 point

            //step 0 of 4
            string indicator = "5";

            string rawPath = _workflowParameters.LCParameters.XYDataWriterPath + @"\LC" + indicator + "_0_RawPreSmooth_0.txt";
            string smoothedpathOmics = _workflowParameters.LCParameters.XYDataWriterPath + @"\LC" + indicator + "_2_SmoothOmics";
            string smoothedpathOmicsFinal = _workflowParameters.LCParameters.XYDataWriterPath + @"\LC" + indicator + "_2_SmoothOmicCalc";
            string peakspathOmicsFinal = _workflowParameters.LCParameters.XYDataWriterPath + @"\LC" + indicator + "_4_PeaksCalc";
            string peaksPathOmics = _workflowParameters.LCParameters.XYDataWriterPath + @"\LC" + indicator + "_4_Peaks";
           


            DataXYDataWriter writer = new DataXYDataWriter();

            writer.WriteOmicsXYData(bufferedchromatogramRaw, rawPath);

            List<ProcessedPeak> peaksOmicsRaw = _lcProcessor.Execute(bufferedchromatogramRaw, EnumerationChromatogramProcessing.LCPeakDetectOnly);
            List<ProcessedPeak> filteredPeaksRaw = Utiliites.FilterByPointsPerSide(peaksOmicsRaw, 1);
            //writer.WriteOmicsProcesedPeakData(filteredPeaksRaw, peaksPathOmics + "_" + smoothPoints + ".txt");

            ProcessedPeak examplePeak;

            int range;


            List<XYData> realValues = new List<XYData>();
            
            realValues.Add(new XYData(100, 2));
            realValues.Add(new XYData(150, 4));
            realValues.Add(new XYData(200, 16));
            realValues.Add(new XYData(250, 64));
            realValues.Add(new XYData(300, 256));
            realValues.Add(new XYData(350, 1024));
            realValues.Add(new XYData(400, 4096));
            realValues.Add(new XYData(450, 16384));
            realValues.Add(new XYData(500, 65536));
            realValues.Add(new XYData(550, 262144));
            realValues.Add(new XYData(600, 1048576));
            realValues.Add(new XYData(650, 4194304));
            realValues.Add(new XYData(700, 16777216));
            realValues.Add(new XYData(750, 67108864));
            realValues.Add(new XYData(800, 268435456));
            realValues.Add(new XYData(850, 1073741824));
            realValues.Add(new XYData(900, 4294967296));
            realValues.Add(new XYData(950, 17179869184));
            realValues.Add(new XYData(1000, 68719476736));


            foreach (XYData realValue in realValues)
            {
                examplePeak = (from peak in filteredPeaksRaw where peak.XValue > realValue.X - 10 && peak.XValue < realValue.X+10 select peak).FirstOrDefault();
                range = examplePeak.MinimaOfHigherMassIndex - examplePeak.MinimaOfLowerMassIndex;
                Console.WriteLine(1 + " points " + examplePeak.XValue + " has a min index is " + examplePeak.MinimaOfLowerMassIndex + " max index is " + examplePeak.MinimaOfHigherMassIndex + " and range is " + range + " raw");

            }
            
            
            Console.WriteLine(Environment.NewLine);
            //TEST CODE

            //1.  test Omics smoother



            for (int i = 5; i < smoothrange; i += 2)
            {
                smoothPoints = i;
                PNNLOmics.Algorithms.SpectralProcessing.SavitzkyGolaySmoother smoootherOmicsPre = new PNNLOmics.Algorithms.SpectralProcessing.SavitzkyGolaySmoother(3, 2, false);

                //bufferedchromatogramRaw = GenerateXyDataWithZeroes(); ;//step2 of 3
                bufferedchromatogramRaw = GenerateFiveXyDataWithZeroes();//step2 of 3

                Assert.AreEqual(1099, bufferedchromatogramRaw.Count);
                Assert.AreEqual(256, bufferedchromatogramRaw[249].Y);//3 point
                //Assert.AreEqual(128.0000192086062d, bufferedchromatogramRaw[248].Y);//5 point

                bool extraSmooth = false;
                List<XYData> presmoothedsmoothedDataOmics = new List<XYData>();
                if (extraSmooth)
                {
                    presmoothedsmoothedDataOmics = smoootherOmicsPre.Smooth(bufferedchromatogramRaw); //this is more accurate
                }
                else
                {
                    presmoothedsmoothedDataOmics = bufferedchromatogramRaw;
                }


                PNNLOmics.Algorithms.SpectralProcessing.SavitzkyGolaySmoother smoootherOmics = new PNNLOmics.Algorithms.SpectralProcessing.SavitzkyGolaySmoother(smoothPoints, 2, false);
                List<XYData> smoothedDataOmics = smoootherOmics.Smooth(presmoothedsmoothedDataOmics); //this is more accurate
                writer.WriteOmicsXYData(smoothedDataOmics, smoothedpathOmics + "_" + smoothPoints + ".txt");

                List<ProcessedPeak> peaksOmics = _lcProcessor.Execute(smoothedDataOmics, EnumerationChromatogramProcessing.LCPeakDetectOnly);
                List<ProcessedPeak> filteredPeaks = Utiliites.FilterByPointsPerSide(peaksOmics, pointsPerShoulder);
                //writer.WriteOmicsProcesedPeakData(filteredPeaks, peaksPathOmics + "_" + smoothPoints + ".txt");

                foreach (XYData realValue in realValues)
                {
                    examplePeak = (from peak in filteredPeaks where peak.XValue > realValue.X - 10 && peak.XValue < realValue.X + 10 select peak).FirstOrDefault();
                    range = examplePeak.MinimaOfHigherMassIndex - examplePeak.MinimaOfLowerMassIndex;
                    Console.WriteLine(smoothPoints + " points " + examplePeak.XValue + " has a min index is " + examplePeak.MinimaOfLowerMassIndex + " max index is " + examplePeak.MinimaOfHigherMassIndex + " and range is " + range);

                }
            }


            for (int i = 5; i < smoothrange; i += 2)
            {
                //bufferedchromatogramRaw = GenerateXyDataWithZeroes(); ;//step3 of 3
                bufferedchromatogramRaw = GenerateFiveXyDataWithZeroes();//step3 of 3

                Assert.AreEqual(1099, bufferedchromatogramRaw.Count);
                Assert.AreEqual(256, bufferedchromatogramRaw[249].Y);//3 point
                //Assert.AreEqual(128.0000192086062d, bufferedchromatogramRaw[248].Y);//5 point
                //calculated answer
                smoothPoints = i;
                //_workflowParameters.LCParameters.PointsPerShoulder = _workflowParameters.LCParameters.CalculatePointsPerShoulderAsAFunctionOfSgPoints(_workflowParameters);
                _workflowParameters.ChromSmootherNumPointsInSmooth = smoothPoints;
                int calclulatedPointsPerShoulder = _workflowParameters.LCParameters.CalculatePointsPerShoulderAsAFunctionOfSgPoints(_workflowParameters);
                Console.WriteLine("For SG points " + i + " we require atleast or equal >= " + calclulatedPointsPerShoulder + " points");
                PNNLOmics.Algorithms.SpectralProcessing.SavitzkyGolaySmoother smoootherOmicsFinal = new PNNLOmics.Algorithms.SpectralProcessing.SavitzkyGolaySmoother(smoothPoints, 2, false);
                List<XYData> smoothedDataOmicsFinal = smoootherOmicsFinal.Smooth(bufferedchromatogramRaw);
                //this is more accurate
                writer.WriteOmicsXYData(smoothedDataOmicsFinal, smoothedpathOmicsFinal + "_" + smoothPoints + ".txt");

                List<ProcessedPeak> peaksOmicsFinal = _lcProcessor.Execute(smoothedDataOmicsFinal, EnumerationChromatogramProcessing.LCPeakDetectOnly);
                List<ProcessedPeak> filteredPeaksFinal = Utiliites.FilterByPointsPerSide(peaksOmicsFinal, calclulatedPointsPerShoulder);
                writer.WriteOmicsProcesedPeakData(filteredPeaksFinal, peakspathOmicsFinal + "_" + smoothPoints + ".txt");
            }

           
        }

        private static List<XYData> GenerateFiveXyDataWithZeroes()
        {
            
            
            List<XYData> starterList = GenerateXyDataWithZeroes(); ;

            List<XYData> fiveList = GenerateXyDataWithZeroes(); ;

            List<XYData> FivePointGaussianFactors = new List<XYData>();
            
            FivePointGaussianFactors.Add(new XYData(-2, 0.0625000375168172));
            FivePointGaussianFactors.Add(new XYData(-1, 0.500000075033618));
            FivePointGaussianFactors.Add(new XYData(0, 1));
            FivePointGaussianFactors.Add(new XYData(1, 0.500000075033618));
            FivePointGaussianFactors.Add(new XYData(2, 0.0625000375168172));

            List<int> indexList = new List<int>();

            for (int i = 0; i < starterList.Count;i++ )
            {
                if (starterList[i].Y > 0)
                {
                    indexList.Add(Convert.ToInt32(starterList[i].X));
                    //Console.WriteLine(starterList[i].Y);
                }
                
            }

            foreach (int d in indexList)
            {
                int center = d - 1;
                fiveList[center - 2].Y = starterList[center].Y * FivePointGaussianFactors[0].Y;
                fiveList[center - 1].Y = starterList[center].Y * FivePointGaussianFactors[1].Y;
                fiveList[center - 0].Y = starterList[center].Y * FivePointGaussianFactors[2].Y;
                fiveList[center + 1].Y = starterList[center].Y * FivePointGaussianFactors[3].Y;
                fiveList[center + 2].Y = starterList[center].Y * FivePointGaussianFactors[4].Y;
                
            }

            


            return fiveList;
        }

        private static List<XYData> GenerateXyDataWithZeroes()
        {
            List<XYData> realValues = new List<XYData>();
            List<XYData> FivePointGaussianFactors = new List<XYData>();
            List<XYData> xyDataWithZeroes = new List<XYData>();
            realValues.Add(new XYData(100, 2));
            realValues.Add(new XYData(150, 4));
            realValues.Add(new XYData(200, 16));
            realValues.Add(new XYData(250, 64));
            realValues.Add(new XYData(300, 256));
            realValues.Add(new XYData(350, 1024));
            realValues.Add(new XYData(400, 4096));
            realValues.Add(new XYData(450, 16384));
            realValues.Add(new XYData(500, 65536));
            realValues.Add(new XYData(550, 262144));
            realValues.Add(new XYData(600, 1048576));
            realValues.Add(new XYData(650, 4194304));
            realValues.Add(new XYData(700, 16777216));
            realValues.Add(new XYData(750, 67108864));
            realValues.Add(new XYData(800, 268435456));
            realValues.Add(new XYData(850, 1073741824));
            realValues.Add(new XYData(900, 4294967296));
            realValues.Add(new XYData(950, 17179869184));
            realValues.Add(new XYData(1000, 68719476736));

            for (int real = 0; real < realValues.Count; real++)
            {
                for (int i = 0; i < 49; i++)
                {
                    xyDataWithZeroes.Add(new XYData(realValues[real].X - 49 + i, 0));
                }
                xyDataWithZeroes.Add(new XYData(realValues[real].X, realValues[real].Y));
            }
            //trailing zero
            for (int i = 1; i < 150; i++)
            {
                xyDataWithZeroes.Add(new XYData(realValues[realValues.Count - 1].X + i, 0));
            }

            return xyDataWithZeroes;
        }

        [Test]
        public void TestScanRange()
        {
            List<XYData> dataXY = new List<XYData>();
            List<XYData> clippedDataXY = new List<XYData>();
            List<XYData> bufferedDataXY = new List<XYData>();

            List<ProcessedPeak> dataProcessedPeaks = new List<ProcessedPeak>();
            List<ProcessedPeak> clippedProcessedPeaks = new List<ProcessedPeak>();
            List<ProcessedPeak> bufferedProcessedPeaks = new List<ProcessedPeak>();

            int scanStart = 5;
            int scanStop = 15;
            int maxScan = 20;
            int minScan = 1;
            int bufferSize = 3;
            int scansToSum = 3;

            ScanObject scans = new ScanObject(scanStart,scanStop,minScan,maxScan,bufferSize,scansToSum);

            dataXY.Add(new XYData(1, 144));
            dataXY.Add(new XYData(2, 127));
            dataXY.Add(new XYData(3, 155));
            dataXY.Add(new XYData(4, 119));
            dataXY.Add(new XYData(5, 133));
            dataXY.Add(new XYData(6, 120));
            dataXY.Add(new XYData(7, 144));
            dataXY.Add(new XYData(8, 178));
            dataXY.Add(new XYData(9, 141));
            dataXY.Add(new XYData(10, 154));
            dataXY.Add(new XYData(11, 187));
            dataXY.Add(new XYData(12, 144));
            dataXY.Add(new XYData(13, 191));
            dataXY.Add(new XYData(14, 134));
            dataXY.Add(new XYData(15, 191));
            dataXY.Add(new XYData(16, 185));
            dataXY.Add(new XYData(17, 182));
            dataXY.Add(new XYData(18, 159));
            dataXY.Add(new XYData(19, 116));
            dataXY.Add(new XYData(20, 152));

            dataProcessedPeaks.Add(new ProcessedPeak(1, 144));
            dataProcessedPeaks.Add(new ProcessedPeak(2, 127));
            dataProcessedPeaks.Add(new ProcessedPeak(3, 155));
            dataProcessedPeaks.Add(new ProcessedPeak(4, 119));
            dataProcessedPeaks.Add(new ProcessedPeak(5, 133));
            dataProcessedPeaks.Add(new ProcessedPeak(6, 120));
            dataProcessedPeaks.Add(new ProcessedPeak(7, 144));
            dataProcessedPeaks.Add(new ProcessedPeak(8, 178));
            dataProcessedPeaks.Add(new ProcessedPeak(9, 141));
            dataProcessedPeaks.Add(new ProcessedPeak(10, 154));
            dataProcessedPeaks.Add(new ProcessedPeak(11, 187));
            dataProcessedPeaks.Add(new ProcessedPeak(12, 144));
            dataProcessedPeaks.Add(new ProcessedPeak(13, 191));
            dataProcessedPeaks.Add(new ProcessedPeak(14, 134));
            dataProcessedPeaks.Add(new ProcessedPeak(15, 191));
            dataProcessedPeaks.Add(new ProcessedPeak(16, 185));
            dataProcessedPeaks.Add(new ProcessedPeak(17, 182));
            dataProcessedPeaks.Add(new ProcessedPeak(18, 159));
            dataProcessedPeaks.Add(new ProcessedPeak(19, 116));
            dataProcessedPeaks.Add(new ProcessedPeak(20, 152));

            Assert.AreEqual(20, dataXY.Count);
            Assert.AreEqual(20, dataProcessedPeaks.Count);

            clippedProcessedPeaks = ChangeRange.ClipProcessedPeakToScanRange(dataProcessedPeaks, 7, 13);
            clippedDataXY = ChangeRange.ClipXyDataToScanRange(dataXY, 7, 13);

            Assert.AreEqual(7, clippedProcessedPeaks.Count);
            Assert.AreEqual(7, clippedDataXY.Count);

            bufferedProcessedPeaks = ChangeRange.ClipProcessedPeakToScanRange(dataProcessedPeaks, scans, true);
            bufferedDataXY = ChangeRange.ClipXyDataToScanRange(dataXY, scans, true);

            Assert.AreEqual(17, bufferedProcessedPeaks.Count);
            Assert.AreEqual(17, bufferedDataXY.Count);

            scans.Buffer = 10;

            bufferedProcessedPeaks = ChangeRange.ClipProcessedPeakToScanRange(dataProcessedPeaks, scans, true);
            bufferedDataXY = ChangeRange.ClipXyDataToScanRange(dataXY, scans, true);

            Assert.AreEqual(20, bufferedProcessedPeaks.Count);
            Assert.AreEqual(20, bufferedDataXY.Count);
        }

        [Test]
        public void TestVerifyByTargetedFeatureFinding()
        {
            PopulateGlobalVariables();
            
            //Assert.AreEqual(_workflowParameters.NumberOfPeaksToLeftForPenalty, 1);
            Assert.AreEqual(_workflowParameters.MSParameters.IsoParameters.NumberOfPeaksToLeftForPenalty, 1);

            FragmentIQTarget chargedParentsFromAllFragmentsToSearchForMass = new FragmentIQTarget(iQresult.Target);

            //possibleFragmentTarget.ScanInfo.Start = 1667;
            //possibleFragmentTarget.ScanInfo.Stop = 1745;
            //int ScanLCTarget = 1706;

            //chargedParentsFromAllFragmentsToSearchForMass.ScanInfo.Start = 1667;
            //chargedParentsFromAllFragmentsToSearchForMass.ScanInfo.Stop = 1745;
            //chargedParentsFromAllFragmentsToSearchForMass.ScanLCTarget = 1706;

            //ChromPeakQualityData possibleFragmentPeakQuality = new ChromPeakQualityData(new ChromPeak(1707.20070542704, 5623193.5f, 77.5042343f, 26.4798851f));
            //possibleFragmentPeakQuality.FitScore = 0.0603449505835566;
            //possibleFragmentPeakQuality.ScanLc = 1706;
            //possibleFragmentPeakQuality.IsotopicProfile = iQresult.Target.TheorIsotopicProfile;
            //possibleFragmentPeakQuality.IsotopicProfile = Utiliites.GenerateCombinedIsotopicProfile(iQresult, 1, 1);

            chargedParentsFromAllFragmentsToSearchForMass.ScanInfo.Start = 1640;
            chargedParentsFromAllFragmentsToSearchForMass.ScanInfo.Stop = 1694;
            chargedParentsFromAllFragmentsToSearchForMass.ScanLCTarget = 1661;

            ChromPeakQualityData possibleFragmentPeakQuality = new ChromPeakQualityData(new ChromPeak(1660.8837478792, 18197090f, 25.5049114f, 4.605931f));
            possibleFragmentPeakQuality.FitScore = 0.0609229743850591;
            possibleFragmentPeakQuality.ScanLc = 1661;
            possibleFragmentPeakQuality.IsotopicProfile = iQresult.Target.TheorIsotopicProfile;
            //possibleFragmentPeakQuality.IsotopicProfile = _theorFeatureGen.GenerateTheorProfile(possibleFragmentTarget.EmpiricalFormula,2);
            possibleFragmentPeakQuality.IsotopicProfile = Utiliites.GenerateCombinedIsotopicProfile(iQresult, 1, 1);

            //create child here
            //possibleFragmentTarget = new FragmentIQTarget();

            possibleFragmentTarget.ScanInfo.Start = 1640;
            possibleFragmentTarget.ScanInfo.Stop = 1694;
            possibleFragmentTarget.ScanLCTarget = 1661;
            possibleFragmentTarget.ChargeState = 2;

            //key input
            int padZeroes = 1;

            //possibleFragmentTarget.TheorIsotopicProfile = _theorFeatureGen.GenerateTheorProfile(possibleFragmentTarget.EmpiricalFormula, possibleFragmentTarget.ChargeState);
            //ITheorFeatureGenerator theorFeatureGenerator = _theorFeatureGen;

            possibleFragmentTarget.TheorIsotopicProfile = _TheorFeatureGenV2.Generator(iQresult.Target.EmpiricalFormula, possibleFragmentTarget.ChargeState);

            possibleFragmentTarget.TheorIsotopicProfile = Utiliites.GenerateCombinedIsotopicProfile(iQresult, 1, 1);

            possibleFragmentTarget.TheorIsotopicProfile = TheoreticalIsotopicProfileWrapper.Add1DaPointsToIso(possibleFragmentTarget.TheorIsotopicProfile, padZeroes);

            chargedParentsFromAllFragmentsToSearchForMass.AddTarget(possibleFragmentTarget);

            FragmentIQTarget oneDaltonShiftedTarget = new FragmentIQTarget(possibleFragmentTarget);
            oneDaltonShiftedTarget.TheorIsotopicProfile = _TheorFeatureGenV2.Generator(iQresult.Target.EmpiricalFormula, possibleFragmentTarget.ChargeState);
            double delta = oneDaltonShiftedTarget.TheorIsotopicProfile.Peaklist[1].XValue - oneDaltonShiftedTarget.TheorIsotopicProfile.Peaklist[0].XValue;
            foreach (MSPeak peak in oneDaltonShiftedTarget.TheorIsotopicProfile.Peaklist)
            {
                peak.XValue += delta;
            }

            //oneDaltonShiftedTarget.TheorIsotopicProfile = Utiliites.GenerateCombinedIsotopicProfile(oneDaltonShiftedTarget.TheorIsotopicProfile, 1, 1);

            oneDaltonShiftedTarget.TheorIsotopicProfile = TheoreticalIsotopicProfileWrapper.Add1DaPointsToIso(oneDaltonShiftedTarget.TheorIsotopicProfile, padZeroes);

            chargedParentsFromAllFragmentsToSearchForMass.AddTarget(oneDaltonShiftedTarget);

            List<FragmentIQTarget> _futureTargets = new List<FragmentIQTarget>();

            //TEST CODE
            List<FragmentIQTarget> passFutureTargets = _futureTargets;

            //List<FragmentTarget> finalChargedParentsToSearchForLC = VerifyByTargetedFeatureFinding.Verify(chargedParentsToSearchForMass, result, run, ref _msGenerator, ref _msfeatureFinder, ref passFutureTargets, ref errorlog);
            double fitScoreCuttoff = 0.15;
            
            IterativeTFF msfeatureFinder = _msfeatureFinder;
            //Task isotopicProfileFitScoreCalculator = _fitScoreCalc;
            Tuple<string, string> errorLog = errorlog;
            //FragmentIQTarget finalChargedParentsFromAllFragmentsToSearchForLC = Utiliites.VerifyByTargetedFeatureFindingTestIQ(chargedParentsFromAllFragmentsToSearchForMass, fitScoreCuttoff, runIn, ref msGenerator, ref msfeatureFinder, ref passFutureTargets, ref isotopicProfileFitScoreCalculator, ref errorLog);

            //MSGenerator msGenerator = _workflowParameters.MSParameters.Engine_msGenerator;
            List<IqGlyQResult> finalChargedParentsFromAllFragmentsToSearchForLCResults = Utiliites.VerifyByTargetedFeatureFindingTestIQ(chargedParentsFromAllFragmentsToSearchForMass, fitScoreCuttoff, runIn, ref msfeatureFinder, ref passFutureTargets, _workflowParameters.MSParameters.IsoParameters,_msProcessor,_workflowParameters.MSToleranceInPPM,  ref errorLog);

            List<IqResult> glycanResults = Utiliites.UnfoldFeatureFinderResults(finalChargedParentsFromAllFragmentsToSearchForLCResults);


            //Assert.AreEqual(2, finalChargedParentsFromAllFragmentsToSearchForLc.ChargeState);

            FragmentIQTarget currentTargetHit = (FragmentIQTarget) glycanResults[0].Target;

            Assert.AreEqual(2, glycanResults.Count);
            Assert.AreEqual(1722.6075472690109d, currentTargetHit.TheorIsotopicProfile.MonoIsotopicMass);
            Assert.AreEqual(0, currentTargetHit.ScanLC);
            Assert.AreEqual(2, currentTargetHit.ChargeState);
            Assert.AreEqual(1640, currentTargetHit.ScanInfo.Start);
            Assert.AreEqual(1694, currentTargetHit.ScanInfo.Stop);
            Assert.AreEqual(1661, currentTargetHit.ScanLCTarget);
            
            Console.WriteLine("model 1 fit is: " + glycanResults[0].FitScore);
            Assert.AreEqual(0.0633942608423267, glycanResults[0].FitScore, 0.01);//non modified

            FragmentIQTarget currentTargetHit1Da = (FragmentIQTarget)glycanResults[1].Target;

            Assert.AreEqual(2, glycanResults.Count);
            Assert.AreEqual(1722.6075472690109d, currentTargetHit1Da.TheorIsotopicProfile.MonoIsotopicMass);
            Assert.AreEqual(0, currentTargetHit1Da.ScanLC);
            Assert.AreEqual(2, currentTargetHit1Da.ChargeState);
            Assert.AreEqual(1640, currentTargetHit1Da.ScanInfo.Start);
            Assert.AreEqual(1694, currentTargetHit1Da.ScanInfo.Stop);
            Assert.AreEqual(1661, currentTargetHit1Da.ScanLCTarget);
            //Assert.AreEqual(0.3154008992268727d, glycanResults[1].FitScore);//overlapped
            Assert.AreEqual(0.505228380870795, glycanResults[1].FitScore);//non modified

            Console.WriteLine("model 2 fit is: " + glycanResults[1].FitScore);

            Assert.AreEqual(8, glycanResults[0].ObservedIsotopicProfile.Peaklist.Count);
            Assert.AreEqual(862.30888177738859d, glycanResults[0].ObservedIsotopicProfile.Peaklist[1].XValue);
            Assert.AreEqual(12834731.0f, glycanResults[0].ObservedIsotopicProfile.Peaklist[1].Height);
            Assert.AreEqual(14248002.0f, glycanResults[0].ObservedIsotopicProfile.Peaklist[2].Height);

            Assert.AreEqual(2, glycanResults[0].ObservedIsotopicProfile.ChargeState);
            Assert.AreEqual(1722.6032105747772d, glycanResults[0].ObservedIsotopicProfile.MonoIsotopicMass);
            Assert.AreEqual(false, glycanResults[0].HasChildren());
            
        }


        [Test]
        public void FitScoreinterface()
        {
            PopulateGlobalVariables();
            IqTarget localTarget = iQresult.Target;
            
            List<MSPeak> theorProfile = new List<MSPeak>();

            

            List<MSPeak> observedProfile = new List<MSPeak>();
            for (int i = 0; i < 6;i++ )
            {
                theorProfile.Add(localTarget.TheorIsotopicProfile.Peaklist[i]);
                MSPeak msPeak = theorProfile[i];
                observedProfile.Add(new MSPeak(msPeak.XValue, (float)(msPeak.Height * (1 +i*i*0.03)), msPeak.Width, msPeak.SignalToNoise));
            }

            ParametersLeastSquares LS_Parameters = new ParametersLeastSquares();
            LS_Parameters.UseIsotopePeakCountCorrection = true;

            IFitScoring myFitScoreLS = FitScoreFactory.Build(LS_Parameters);

            double scoreLS = myFitScoreLS.CalculateFitScore(theorProfile, observedProfile);

            Assert.AreEqual(0.043686078101124272, scoreLS, 0.0001);

            ParametersPiersonCorrelation PC_Parameters = new ParametersPiersonCorrelation();

            IFitScoring myFitScorePC = FitScoreFactory.Build(PC_Parameters);

            double scorePC = myFitScorePC.CalculateFitScore(theorProfile, observedProfile);

            Assert.AreEqual(0.0007759103122245703, scorePC, 0.0001);

            ParametersCosineFit CF_Parameters = new ParametersCosineFit();

            IFitScoring myFitScoreCF = FitScoreFactory.Build(CF_Parameters);

            double scoreCF = myFitScoreCF.CalculateFitScore(theorProfile, observedProfile);

            Assert.AreEqual(0.00099396162251430553, scoreCF, 0.0001);
        }

        [Test]
        public void isotopeProfileSimple()
        {
            PopulateGlobalVariables();

            JoshTheorFeatureGenerator theorFeatureGen = new JoshTheorFeatureGenerator(_workflowParameters.IsotopeProfileType, _workflowParameters.IsotopeLowPeakCuttoff);//perhaps simple constructor
            ITheorFeatureGenerator localTheorFeatureGen = theorFeatureGen;
            IqTarget localTarget = iQresult.Target;

            //old
            IsotopicProfile TheorIsotopicProfile = TheoreticalIsotopicProfileWrapper.GenerateSimpleOld(ref localTheorFeatureGen, localTarget.EmpiricalFormula, localTarget.ChargeState);


            //new
            //var simpleParameters = new ParametersSimpleIsotope(localTheorFeatureGen);
            //IGenerateIsotopeProfile TheorFeatureGenV2 = new IsotopeProfileSimple(simpleParameters);
            IsotopicProfile TheorIsotopicProfileNew = _TheorFeatureGenV2.Generator(localTarget.EmpiricalFormula, localTarget.ChargeState);

            for(int i = 0; i< TheorIsotopicProfile.Peaklist.Count;i++)
            {
                Assert.AreEqual(TheorIsotopicProfile.Peaklist[i].Height, TheorIsotopicProfileNew.Peaklist[i].Height);
            }
        }

        [Test]
        public void isotopeProfilePileUp()
        {
            PopulateGlobalVariables();

            JoshTheorFeatureGenerator theorFeatureGen = new JoshTheorFeatureGenerator(_workflowParameters.IsotopeProfileType, _workflowParameters.IsotopeLowPeakCuttoff);//perhaps simple constructor
            ITheorFeatureGenerator localTheorFeatureGen = theorFeatureGen; 
            
            IqTarget localTarget = iQresult.Target;

            //old
            IsotopicProfile TheorIsotopicProfile = TheoreticalIsotopicProfileWrapper.GenerateSimpleOld(ref localTheorFeatureGen, localTarget.EmpiricalFormula, localTarget.ChargeState);

            TheorIsotopicProfile = TheoreticalIsotopicProfileWrapper.GenerateCombinedIsotopicProfile(TheorIsotopicProfile, 1, 1);

            //new
            var blenededParameters = new ParametersBlendedIsotope(new ParametersSimpleIsotope(localTheorFeatureGen));
            blenededParameters.Offsets.Add(1);
            blenededParameters.MixingFraction.Add(1);
            IsotopeProfileBlended TheorFeatureGenV2 = new IsotopeProfileBlended(blenededParameters);

            List<int> offsets = blenededParameters.Offsets;
            List<float> relativeRatiosToMainIsotope = blenededParameters.MixingFraction;
            IsotopicProfile TheorIsotopicProfileV2 = TheorFeatureGenV2.Generator(localTarget.EmpiricalFormula, localTarget.ChargeState, ref offsets, ref relativeRatiosToMainIsotope);
            TheorIsotopicProfileV2.RefreshAlternatePeakIntenstiesFromPeakList();

            for (int i = 0; i < TheorIsotopicProfile.Peaklist.Count; i++)
            {
                Assert.AreEqual(TheorIsotopicProfile.Peaklist[i].Height, TheorIsotopicProfileV2.AlternatePeakIntensities[i]);
            }
        }

        private static Stopwatch StartClock(out DateTime starttime)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            starttime = DateTime.Now;
            return stopWatch;
        }

        private static void StopClock(DateTime starttime, Stopwatch stopWatch)
        {
            System.DateTime stoptime = DateTime.Now;
            Console.WriteLine("This started at " + starttime + " and ended at" + stoptime);
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to find " + 4 + " eluting peaks");
            Console.WriteLine("");
        }


        


        #region global variables

        private FragmentIQTarget possibleFragmentTarget { get; set; }
        private FragmentResultsObjectHolderIq targetResult { get; set; }
        private FragmentedTargetedWorkflowParametersIQ _workflowParameters { get; set; }
        private Run runIn { get; set; }
        private IqGlyQResult iQresult { get; set; }
        private ProcessorMassSpectra _msProcessor { get; set; }
        private Tuple<string, string> errorlog { get; set; }
        private string printString { get; set; }
        private IGenerateIsotopeProfile _TheorFeatureGenV2 { get; set; }
        private IterativeTFF _msfeatureFinder { get; set; }
        private TaskIQ _fitScoreCalc { get; set; }
        private ProcessorChromatogram _lcProcessor { get; set; }
        private TheoreticalIsotopicProfileWrapper Monster { get; set; }

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
            //ITheorFeatureGenerator joshTheorFeatureGenerator = _theorFeatureGen;
            JoshTheorFeatureGenerator _theorFeatureGen = new JoshTheorFeatureGenerator();//perhaps simple constructor
            IGenerateIsotopeProfile theorFeatureGenV2 = new IsotopeProfileSimple(new ParametersSimpleIsotope(_theorFeatureGen));
            IterativeTFF msfeatureFinder = _msfeatureFinder;
            TaskIQ isotopicProfileFitScoreCalculator = _fitScoreCalc;
            ProcessorChromatogram processorChromatogram = _lcProcessor;

            IQGlyQTestingUtilities.SetupTargetAndEnginesForOneTargetRef(ref fragmentIqTarget, ref fragmentResultsObjectHolderIq, ref fragmentedTargetedWorkflowParametersIq, ref run, ref iqGlyQResult, ref msProcessor, ref tuple, ref s, ref theorFeatureGenV2, ref msfeatureFinder, ref isotopicProfileFitScoreCalculator, ref processorChromatogram, isUnitTest, thisDataset, isPic);

            ParametersIsoCalibration isoCalParameters = new ParametersIsoCalibration(_workflowParameters.MSParameters.IsoParameters.DeltaMassCalibrationMZ, _workflowParameters.MSParameters.IsoParameters.DeltaMassCalibrationMono);
            ParametersIsoShift isoShiftParameters = new ParametersIsoShift(_workflowParameters.MSParameters.IsoParameters.PenaltyMode, _workflowParameters.MSParameters.IsoParameters.NumberOfPeaksToLeftForPenalty);
            Monster = new TheoreticalIsotopicProfileWrapper(isoCalParameters, isoShiftParameters);


            possibleFragmentTarget = fragmentIqTarget;
            targetResult = fragmentResultsObjectHolderIq;
            _workflowParameters = fragmentedTargetedWorkflowParametersIq;
            runIn = run;
            iQresult = iqGlyQResult;
            _msProcessor = msProcessor;
            errorlog = tuple;
            printString = s;
            _TheorFeatureGenV2 = theorFeatureGenV2;
            _msfeatureFinder = msfeatureFinder;
            _fitScoreCalc = isotopicProfileFitScoreCalculator;
            _lcProcessor = processorChromatogram;

        }
    }
}
