using System;
using System.Collections.Generic;
using System.Linq;
using DeconTools.Backend.Runs;
using DeconTools.Utilities;
using DeconTools.Workflows.Backend.Core;
using DeconTools.Backend.Core;
using DeconTools.Backend.Utilities;
using System.IO;
using DeconTools.Backend.ProcessingTasks.TheorFeatureGenerator;
using GetPeaks_DLL.Functions;
using IQGlyQ.Enumerations;
using IQGlyQ.Executor;
using IQGlyQ.Functions;
using IQGlyQ.Processors;
using IQGlyQ.Results;
using IQGlyQ.Tasks;
using IQGlyQ.Workflows;
using PNNLOmics.Algorithms.PeakDetection;
using DeconTools.Backend.ProcessingTasks.MSGenerators;
using DeconTools.Backend.ProcessingTasks.Smoothers;
using DeconTools.Backend.ProcessingTasks;
using DeconTools.Backend;
using DeconTools.Backend.ProcessingTasks.TargetedFeatureFinders;
using DeconTools.Backend.ProcessingTasks.FitScoreCalculators;
using PNNLOmics.Data.Constants;
using IQGlyQ.Objects;

namespace IQGlyQ.UnitTesting
{
    public static class IQGlyQTestingUtilities
    {
        //public static void SetupTargetAndEnginesForOneTargetRef(ref FragmentIQTarget possibleFragmentTarget, ref FragmentResultsObjectHolderIq targetResult, ref FragmentedTargetedWorkflowParametersIQ _workflowParameters, ref Run runIn, ref IqGlyQResult iQresult, ref Processors.ProcessorMassSpectra msProcessor, ref Tuple<string, string> errorlog, ref string printString, ref ITheorFeatureGenerator _theorFeatureGen, ref IterativeTFF _msfeatureFinder, ref Task _fitScoreCalc, ref ProcessorChromatogram lcProcessor, bool isUnitTest, EnumerationDataset thisDataset, EnumerationIsPic isPic, double deltaMassCalibrationMZ, double deltaMassCalibrationMono, bool toMassCalibrate)
        
        public static void SetupTargetAndEnginesForOneTargetRef(ref FragmentIQTarget possibleFragmentTarget, ref FragmentResultsObjectHolderIq targetResult, ref FragmentedTargetedWorkflowParametersIQ _workflowParameters, ref Run runIn, ref IqGlyQResult iQresult, ref Processors.ProcessorMassSpectra msProcessor, ref Tuple<string, string> errorlog, ref string printString, ref ITheorFeatureGenerator _theorFeatureGen, ref IterativeTFF _msfeatureFinder, ref Task _fitScoreCalc, ref ProcessorChromatogram lcProcessor, bool isUnitTest, EnumerationDataset thisDataset, EnumerationIsPic isPic)
        {
            int _numPointsInSmoother = 9;
            int numberOfScansToBuffer = 2 * _numPointsInSmoother;
            int scansToSum = 9; //perhapps 5 to decrease noise.  summing is required for orbitrap data

            int scanStart = 1648;
            int scanStop = 1674;

            //1.  set strings
            string testFile;
            string peaksTestFile;
            string resultsFolder;
            string targetsFile;
            string executorParameterFile;
            string factorsFile;
            string loggingFolder;
            string pathFragmentParameters;
            string filefolderPath;
            //EnumerationDataset thisDataset = EnumerationDataset.Diabetes;//TODO this is old
            IQ_UnitTest_SetStrings.Set(out testFile, out peaksTestFile, out resultsFolder, out targetsFile, out executorParameterFile, out factorsFile, out loggingFolder, out pathFragmentParameters, out filefolderPath, thisDataset, isPic);
            
            string XYDataFolder = "XYDataWriter";

            //2.  setup parameters, executor and targetsfile

            string expectedResultsFilename;
            string expectedResultsFilenameSummary;
            IqExecutor executor;

            //targetsFile = SetExecutorAndFragmentsAndParameters(testFile, peaksTestFile, resultsFolder, resultsFolder, targetsFile, executorParameterFile, factorsFile, loggingFolder, XYDataFolder, pathFragmentParameters, filefolderPath, out _workflowParameters, out expectedResultsFilename, out expectedResultsFilenameSummary, out executor, out runIn, isUnitTest, thisDataset, isPic, deltaMassCalibrationMZ, deltaMassCalibrationMono, toMassCalibrate);

            targetsFile = SetExecutorAndFragmentsAndParameters(testFile, peaksTestFile, resultsFolder, resultsFolder, targetsFile, executorParameterFile, factorsFile, loggingFolder, XYDataFolder, pathFragmentParameters, filefolderPath, out _workflowParameters, out expectedResultsFilename, out expectedResultsFilenameSummary, out executor, out runIn, isUnitTest, isPic);

            //3.  set workflows
            SetWorkflows(_workflowParameters, executor, runIn);

            //4.  other stuff

            iQresult = new IqGlyQResult(executor.Targets[0]);

            _theorFeatureGen = new JoshTheorFeatureGenerator(_workflowParameters.IsotopeProfileType, _workflowParameters.IsotopeLowPeakCuttoff);//perhaps simple constructor

            //_msGenerator = new GenericMSGenerator();

            IterativeTFFParameters _iterativeTFFParameters = new IterativeTFFParameters();

            _msfeatureFinder = new IterativeTFF(_iterativeTFFParameters);

            //_fitScoreCalc = new IsotopicProfileFitScoreCalculator();
            //int peaksToLeft = _workflowParameters.NumberOfPeaksToLeftForPenalty;

            //_workflowParameters.MSParameters.FitScoreParameters.NumberOfPeaksToLeftForPenalty = 1;
           
            //_fitScoreCalc = new IsotopicPeakFitScoreCalculator();
            //_fitScoreCalc = new IsotopicPeakFitScoreCalculator(peaksToLeft);
            errorlog = new Tuple<string, string>("Start", "Success");
            printString = "";


            //5.  fixed parameters
            iQresult.Target.ChargeState = 2;

            possibleFragmentTarget = new FragmentIQTarget();

            ScanObject scanInfo = new ScanObject(scanStart, scanStop);
            scanInfo.Max = runIn.MaxLCScan;
            scanInfo.Min = runIn.MinLCScan;
            scanInfo.ScansToSum = scansToSum;
            scanInfo.Buffer = numberOfScansToBuffer;

            possibleFragmentTarget.ScanLCTarget = 1661;

            //6.  test
            //this is the main one for the targets in testing
            iQresult.Target.TheorIsotopicProfile = Utiliites.TherortIsotopeicProfileWrapperLowLevel( ref _theorFeatureGen, iQresult.Target.EmpiricalFormula, iQresult.Target.ChargeState);

            possibleFragmentTarget.TheorIsotopicProfile = Utiliites.GenerateCombinedIsotopicProfile(iQresult, 1, 1);
            possibleFragmentTarget.ChargeState = possibleFragmentTarget.TheorIsotopicProfile.ChargeState;

            targetResult = new FragmentResultsObjectHolderIq(possibleFragmentTarget);//this is the base result for each parent 

            targetResult.ScanBoundsInfo = scanInfo;

            lcProcessor = new ProcessorChromatogram(_workflowParameters.LCParameters);

            msProcessor = new ProcessorMassSpectra(_workflowParameters.MSParameters);
        }

        //public static string SetExecutorAndFragmentsAndParameters(string testFile, string peaksTestFile, string resultsFolder, string resultsFolderSummary, string targetsFile, string executorParameterFile, string factorFile, string loggingFolder, string xyDataFolder, string pathFragmentParameters, string filefolderPath, out FragmentedTargetedWorkflowParametersIQ fragmentedTargetedWorkflowParameters, out string expectedResultsFilename, out string expectedResultsFilenameSummary, out IqExecutor executor, out Run run, bool isUnitTest, EnumerationDataset thisDataset, EnumerationIsPic isPic, double deltaMassCalibrationMZIn, double deltaMassCalibrationMonoIn, bool toMassCalibrateIn)
        
        public static string SetExecutorAndFragmentsAndParameters(string testFile, string peaksTestFile, string resultsFolder, string resultsFolderSummary, string targetsFile, string executorParameterFile, string factorFile, string loggingFolder, string xyDataFolder, string pathFragmentParameters, string filefolderPath, out FragmentedTargetedWorkflowParametersIQ fragmentedTargetedWorkflowParameters, out string expectedResultsFilename, out string expectedResultsFilenameSummary, out IqExecutor executor, out Run run, bool isUnitTest, EnumerationIsPic isPic)
        {
            
            //load parameters-pathFragmentParameters
            FragmentedTargetedPeakProcessingParameters processingParameters = new FragmentedTargetedPeakProcessingParameters();
            processingParameters.SetParameters(pathFragmentParameters);

            Console.WriteLine(Environment.NewLine);
           
            Console.WriteLine("FragmentedTargetedPeakProcessingParameters have been loaded from " + pathFragmentParameters);
            Console.WriteLine("-Mass");
            Console.WriteLine("  Will We Calibrate ? : " + processingParameters.ToCalibrate);
            Console.WriteLine("  Shift The MZ by :+" + processingParameters.CalibrateShiftMZ);
            Console.WriteLine("  Shift The Mono by :+" + processingParameters.CalibrateShiftMono);
            Console.WriteLine("  Isotope fits are good it we are below this : " + processingParameters.FitScoreCuttoff);
            Console.WriteLine("  Isotope Mode is : " + processingParameters.IsotopicProfileMode);
            Console.WriteLine("  Exact Mass PPM tolerance : " + processingParameters.MSToleranceInPPM);
            Console.WriteLine("-LC");
            Console.WriteLine("  SG Smooth LC with this many points : " + processingParameters.ChromSmootherNumPointsInSmooth);
            Console.WriteLine("  LC peaks will correlate when the R is greater than : " + processingParameters.CorrelationScoreCuttoff);
            Console.WriteLine("  LM Curve Fitting Has to beat this : " + processingParameters.LM_RsquaredCuttoff);
            Console.WriteLine("  We are summing " + processingParameters.NumMSScansToSum + " scans");
            Console.WriteLine("  ParametersOmicsThreshold : " + processingParameters.ParametersOmicsThreshold);
            Console.WriteLine("  Min number of Points on a shoulder required to pass as a peak  : " + processingParameters.PointsPerShoulder);
            Console.WriteLine("  We want to divide the fit score by the number of the ions: " + processingParameters.DivideFitScoreByNumberOfIons);
            Console.WriteLine("  We want to apply the area cuttoff filter so " + processingParameters + " fraction of the isotopic envelope must be detected (by area)");
            Console.WriteLine(Environment.NewLine);

            Console.WriteLine("TargetsFile  : " + targetsFile);

            Console.WriteLine("Press a key to continue");
            //Console.ReadKey();

            //1.  set target and workflow parameters!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Parameters are in here
            string xyDataPath = resultsFolder + "\\" + xyDataFolder;
            //BasicTargetedWorkflowExecutorParameters executorParameters = RunMeIQGlyQ.BasicTargetedWorkflowExecutorParametersSetIq(executorParameterFile, factorFile,xyDataPath, processingParameters, out fragmentedTargetedWorkflowParameters, thisDataset, isPic, isUnitTest);
            BasicTargetedWorkflowExecutorParameters executorParameters = RunMeIQGlyQ.BasicTargetedWorkflowExecutorParametersSetIq(executorParameterFile, factorFile, xyDataPath, processingParameters, out fragmentedTargetedWorkflowParameters, processingParameters.IsotopicProfileMode, isPic, isUnitTest);

            Console.WriteLine("FragmentedTargetedWorkflowParameters is now ready to go");

            //fragmentedTargetedWorkflowParameters.CorrelationScoreCuttoff
            //fragmentedTargetedWorkflowParameters.LCParameters
            //fragmentedTargetedWorkflowParameters.MSParameters
            //fragmentedTargetedWorkflowParameters.DeltaMassCalibrationMZ
            //fragmentedTargetedWorkflowParameters.DeltaMassCalibrationMono
            //fragmentedTargetedWorkflowParameters.FitScoreCuttoff
            //fragmentedTargetedWorkflowParameters.IsotopeLabelingEfficiency
            //fragmentedTargetedWorkflowParameters.IsotopeLowPeakCuttoff
            //fragmentedTargetedWorkflowParameters.IsotopeProfileMode
            //fragmentedTargetedWorkflowParameters.IsotopeProfileType


            //fit score stuff (manual Parameters)
            EnumerationIsotopePenaltyMode mode = EnumerationIsotopePenaltyMode.PointToLeft;
            //FragmentedTargetedWorkflowParametersIQ tempParameters = fragmentedTargetedWorkflowParameters;
            ITheorFeatureGenerator theoryIsotopeGenerator = new JoshTheorFeatureGenerator();
            double theoreticaIntensityCuttoff = 0.1;//we need theoretical intenstities above this percentage

            fragmentedTargetedWorkflowParameters.MSParameters.IsoParameters = new IsotopeParameters(0, mode, fragmentedTargetedWorkflowParameters.MSParameters.IsoParameters.IsotopeProfileMode, theoryIsotopeGenerator, theoreticaIntensityCuttoff, fragmentedTargetedWorkflowParameters.MSParameters.IsoParameters.CuttOffArea);

            fragmentedTargetedWorkflowParameters.MSParameters.Engine_FitScoreCalculator = new IsotopicPeakFitScoreCalculatorSK(fragmentedTargetedWorkflowParameters.MSParameters.IsoParameters);


            //overwrite executorParameters
            executorParameters.LoggingFolder = resultsFolder;
            executorParameters.ResultsFolder = resultsFolder;
            executorParameters.TargetedAlignmentWorkflowParameterFile = filefolderPath + @"\" + "TargetedAlignmentWorkflowParameters.xml";
            executorParameters.TargetsFilePath = targetsFile;
            executorParameters.WorkflowParameterFile = filefolderPath + @"\" + "BasicTargetedWorkflowParameters.xml";

            fragmentedTargetedWorkflowParameters.MSParameters.IsoParameters.ToMassCalibrate = processingParameters.ToCalibrate;
            fragmentedTargetedWorkflowParameters.MSParameters.IsoParameters.DeltaMassCalibrationMZ = processingParameters.CalibrateShiftMZ;
            fragmentedTargetedWorkflowParameters.MSParameters.IsoParameters.DeltaMassCalibrationMono = processingParameters.CalibrateShiftMono;
            fragmentedTargetedWorkflowParameters.MSParameters.IsoParameters.IsotopeProfileMode = processingParameters.IsotopicProfileMode;
            fragmentedTargetedWorkflowParameters.MSParameters.IsoParameters.DivideFitScoreByNumberOfIons = processingParameters.DivideFitScoreByNumberOfIons;
            fragmentedTargetedWorkflowParameters.MSParameters.IsoParameters.CuttOffArea = processingParameters.CuttOffArea;

            //set inside BasicTargetedWorkflowExecutorParametersSetIq       fragmentedTargetedWorkflowParameters.LCParameters.ProcessLcChromatogram = processingParameters.ProcessLcChromatogramEnum;
            fragmentedTargetedWorkflowParameters.LCParameters.ParametersChromGenerator.ChromToleranceInPPMMax = processingParameters.ChromToleranceInPPMMax;
            fragmentedTargetedWorkflowParameters.LCParameters.ParametersChromGenerator.ChromToleranceInPPMInitial = processingParameters.ChromToleranceInPPMInitialFactor * processingParameters.MSToleranceInPPM;
            fragmentedTargetedWorkflowParameters.LCParameters.ParametersChromGenerator.AutoSelectEICAt_X_partOfPeakWidth = processingParameters.AutoSelectEICAt_X_partOfPeakWidth;
            

            executorParameters.ResultsFolder = resultsFolder;
            executorParameters.LoggingFolder = loggingFolder;
            //executorParameters.TargetsFilePath = targetsFile;

           

            //1.5  copy run
            string filename = testFile;
            bool folderExists = Directory.Exists(filename);
            bool fileExists = File.Exists(filename);


            Console.WriteLine("The file + path is: " + filename);
            Console.WriteLine("Does It Exist?: " + filename);

            Check.Require(folderExists || fileExists, "Dataset file not found error when RunUtilites tried to create Run.");


            Console.WriteLine("Try creating Run");


            RunFactory rf = new RunFactory();
            run = rf.CreateRun(filename);

            Console.WriteLine("RunCreated");

            Check.Ensure(run != null, "RunUtilites could not create run. Run is null.");
            //end copy run

            //make peaks file if we don't have one
            if (run != null)
            {
                peaksTestFile = LoadChromData(run, fragmentedTargetedWorkflowParameters);
            }
            run = RunUtilities.CreateAndAlignRun(testFile, peaksTestFile);


            ///this is only copy of Run scan set creation!!!!!!
            run.ScanSetCollection.Create(run, fragmentedTargetedWorkflowParameters.NumMSScansToSum, 1, false);//This is the only scan set collection creator!


            //2.  create executor from parameters
            //executor = new IqExecutor(executorParameters, run);//new
            executor = new IqExecutor(executorParameters);//old
            //3.  initialize targets
            if (isUnitTest)
            {
                executorParameters.TargetsFilePath = @"E:\ScottK\IQ\RunFiles\Gly09_Velos3_Jaguar_230nL30_C15_SN123_3X_01Jan13_R8_resultsSK_vIQ HM SNx1 Man8.txt";
            }
            
            targetsFile = executorParameters.TargetsFilePath;

            Console.WriteLine("The Targets file we are using is: " + targetsFile);

            executor.LoadAndInitializeTargets(targetsFile);

            //executor.InitializeTargets();
            //executor.Targets = (from n in executor.Targets where n.ElutionTimeTheor > 0.305 && n.ElutionTimeTheor < 0.325 select n).Take(10).ToList();

            




            //4.  create run
            executor.ChromSourceDataFilePath = peaksTestFile;

            

            //run = RunUtilities.CreateAndAlignRun(testFile, peaksTestFile);


            executor.SetRun(run);//old

            //5.  delete old results file
            expectedResultsFilename = resultsFolder + "\\" + RunUtilities.GetDatasetName(testFile) + "_iqResults.txt";
            string expectedResultsFilenameSummaryWithEnding = resultsFolder + "\\" + RunUtilities.GetDatasetName(testFile);//remove ending

            //remove characters from ending to get summary file

       //     char marker = '_';
       //     char[] endingInCharTarget = FindAndConvertEnding(expectedResultsFilenameSummaryWithEnding , marker);
       //     string baseName = expectedResultsFilenameSummaryWithEnding.TrimEnd(endingInCharTarget);
       //     expectedResultsFilenameSummary = baseName + "_iqResults.txt";
            
            expectedResultsFilenameSummary = resultsFolderSummary + "\\" + RunUtilities.GetDatasetName(testFile) + "_iqResults.txt";




            //delete old results file if it exists
            if (File.Exists(expectedResultsFilename)) File.Delete(expectedResultsFilename);

            //6. peaks to the left pentalty.  we need to finalize the engine with the parameter set above
            //int peaksToLeft = fragmentedTargetedWorkflowParameters.MSParameters.FitScoreParameters.NumberOfPeaksToLeftForPenalty;//basic iq
            //fragmentedTargetedWorkflowParameters.MSParameters.Engine_FitScoreCalculator = new IsotopicPeakFitScoreCalculator(peaksToLeft);//basic IQ


            

           
            return targetsFile;
        }

        private static char[] FindAndConvertEnding(string dataFileName, char marker)
        {
            char[] name = new char[dataFileName.Count()];
            for (int j = 0; j < dataFileName.Length; j++)
            {
                name[j] = dataFileName[j];
            }

            int endingLength = 0;

            for (int j = name.Length-1; j > 0;j-- )
            {
                char character = name[j];
                if (character == marker)
                {
                    endingLength = name.Length-j;
                    break;
                }

            }
            
            

            //selectEndingCahrs
            char[] endingInCharData = new char[endingLength];
            int charCounter = 0;
            for (int j = name.Length - 1; j > name.Length - 1 - endingLength; j--)
            {
                endingInCharData[charCounter] = name[j];
                charCounter++;
            }
            return endingInCharData;
        }

        #region iq executror delete stuff

        private static string LoadChromData(Run run, FragmentedTargetedWorkflowParametersIQ Parameters)
        {
            var peaksFilename = run.DataSetPath + "\\" + run.DatasetName + "_peaks.txt";

            bool peaksFileExists = checkForPeaksFile(run);
            if (!peaksFileExists)
            {
                peaksFilename = CreatePeaksForChromSourceData(run, Parameters);
            }



            //if (string.IsNullOrEmpty(peaksFilename))
            //{
            //    peaksFilename = GetPossiblePeaksFile(run);
            //}

            //if (string.IsNullOrEmpty(peaksFilename))
            //{
            //    //ReportGeneralProgress("Creating _Peaks.txt file for extracted ion chromatogram (XIC) source data ... takes 1-5 minutes");
            //    peaksFilename = CreatePeaksForChromSourceData(run, Parameters);

            //}
            //else
            //{
            //    //ReportGeneralProgress("Using existing _Peaks.txt file");
            //}


            return peaksFilename;
        }

        private static string CreatePeaksForChromSourceData(Run run, FragmentedTargetedWorkflowParametersIQ Parameters)
        {
            var parameters = new PeakDetectAndExportWorkflowParameters();

            parameters.PeakBR = Parameters.ChromGenSourceDataPeakBR;
            parameters.PeakFitType = DeconTools.Backend.Globals.PeakFitType.QUADRATIC;
            parameters.SigNoiseThreshold = Parameters.ChromGenSourceDataSigNoise;
            parameters.ProcessMSMS = Parameters.ProcessMsMs;
            parameters.IsDataThresholded = parameters.IsDataThresholded;//.ChromGenSourceDataIsThresholded;
            parameters.Num_LC_TimePointsSummed = Parameters.NumMSScansToSum;//scans to sum for peaks file

            var peakCreator = new PeakDetectAndExportWorkflow(run, parameters);
            peakCreator.Execute();

            var peaksFilename = run.DataSetPath + "\\" + run.DatasetName + "_peaks.txt";
            return peaksFilename;

        }

        private static bool checkForPeaksFile(Run run)
        {
            string baseFileName;
            baseFileName = run.DataSetPath + "\\" + run.DatasetName;

            string possibleFilename1 = baseFileName + "_peaks.txt";

            if (File.Exists(possibleFilename1))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static string GetPossiblePeaksFile(Run run)
        {
            string baseFileName;
            baseFileName = run.DataSetPath + "\\" + run.DatasetName;

            string possibleFilename1 = baseFileName + "_peaks.txt";

            if (File.Exists(possibleFilename1))
            {
                return possibleFilename1;
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion






        
        public static void SetWorkflows(FragmentedTargetedWorkflowParametersIQ fragmentedTargetedWorkflowParameters, IqExecutor executor, Run run)
        {
            
            Console.WriteLine("SetWorkFlows...");
            //FragmentedTargetedIQWorkflow workflow = new FragmentedTargetedIQWorkflow(run, fragmentedTargetedWorkflowParameters);
            //var workflow = new BasicIqWorkflow(run, targetedWorkflowParameters);

            //executor.AddIqWorkflow(workflow);
            //executor.AddIqWorkflow(workflow);

            //define workflows for parentTarget and childTargets
            //var parentWorkflow = new FragmentedTargetedIQWorkflow(run, fragmentedTargetedWorkflowParameters);


            //var parentWorkflow = new FragmentedParentIQWorkflow(run, fragmentedTargetedWorkflowParameters);
            
            //var parentWorkflow = new FragmentedAboveBelowParentIQWorkflow(run, fragmentedTargetedWorkflowParameters);//default no  charge correlation
            var parentWorkflow = new ParentIQWorkflowWithChargeCorrelation(run, fragmentedTargetedWorkflowParameters);//default with  charge consolidation
            
            
            var childWorkflow = new FragmentedTargetedIQWorkflow(run, fragmentedTargetedWorkflowParameters);

            IqWorkflowAssigner workflowAssigner = new IqWorkflowAssigner();

            workflowAssigner.AssignWorkflowToParent(parentWorkflow, executor.Targets);

            workflowAssigner.AssignWorkflowToChildren(childWorkflow, executor.Targets);

            //executor.InitializeWorkflows();
        }
    }
}
