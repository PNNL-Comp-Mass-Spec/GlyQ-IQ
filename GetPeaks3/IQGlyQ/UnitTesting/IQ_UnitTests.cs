using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
//using GetPeaks_DLL.DataFIFO;
using DivideTargetsLibraryX64;
using DivideTargetsLibraryX64.Parameters;
using GetPeaksDllLite.DataFIFO;
using IQGlyQ.Enumerations;
using IQ_X64.Backend.Core;
using IQ_X64.Backend.Utilities;
using IQ_X64.Workflows.FileIO;
using IQ_X64.Workflows.FileIO.DTO;
using IQ_X64.Workflows.WorkFlowParameters;
using IQ_X64.Workflows.WorkFlowPile;
using NUnit.Framework;
using System.IO;
using Run64.Backend;
using Run64.Backend.Core;
using Run64.Backend.Core.Results;
using StringLoadTextFileLine = GetPeaksDllLite.DataFIFO.StringLoadTextFileLine;

//using StringLoadTextFileLine = GetPeaks_DLL.DataFIFO.StringLoadTextFileLine;

//using System.Diagnostics;

namespace IQGlyQ.UnitTesting
{
    public class IQ_UnitTests
    {
        private string bruker9t_samplefile1 =
            //@"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\N14N15_standard_testing\RawData\RSPH_PtoA_L_28_rep1_28Feb08_Raptor_08-01-01";
            @"D:\Csharp\ConosleApps\LocalServer\IQ\IQDemo\N14N15\N14N15_standard_testing\RawData\RSPH_PtoA_L_28_rep1_28Feb08_Raptor_08-01-01";
        //"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\IQDemo\N14N15\N14N15_standard_testing\RawData\RSPH_PtoA_L_28_rep1_28Feb08_Raptor_08-01-01";

        private string bruker9t_peaksfile1 =
            //@"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\N14N15_standard_testing\RawData\RSPH_PtoA_L_28_rep1_28Feb08_Raptor_08-01-01\RSPH_PtoA_L_28_rep1_28Feb08_Raptor_08-01-01_scans1450_1800_peaks.txt";
            @"D:\Csharp\ConosleApps\LocalServer\IQ\IQDemo\N14N15\N14N15_standard_testing\RawData\RSPH_PtoA_L_28_rep1_28Feb08_Raptor_08-01-01\RSPH_PtoA_L_28_rep1_28Feb08_Raptor_08-01-01_scans1450_1800_peaks.txt";
        //@"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\IQDemo\N14N15\N14N15_standard_testing\RawData\RSPH_PtoA_L_28_rep1_28Feb08_Raptor_08-01-01\RSPH_PtoA_L_28_rep1_28Feb08_Raptor_08-01-01_scans1450_1800_peaks.txt";


        

        [Test]
        public void findSingleMassTag_test1()
        {
            //string RawDataBasePath = @"\\protoapps\UserData\Slysz\DeconTools_TestFiles";
            string RawDataBasePath = @"D:\Csharp\ConosleApps\LocalServer\IQ"; //1 of 2 Work
            //string RawDataBasePath = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ";//1 of 2 Home
            string OrbitrapStdFile1 = RawDataBasePath + "\\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW"; //B
            //"\\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW"

            //SK
            RawDataBasePath = @"E:\"; //1 of 2
            OrbitrapStdFile1 = RawDataBasePath + "\\Gly09_Velos3_Jaguar_230nL30_C15_DB01_30uL1X_30Dec12.raw";

            string testFile = OrbitrapStdFile1;
            string OrbitrapPeakFile_scans5500_6500 = RawDataBasePath + "\\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18_scans5500-6500_peaks.txt";

            //SK
            OrbitrapPeakFile_scans5500_6500 = RawDataBasePath + "\\Gly09_Velos3_Jaguar_230nL30_C15_DB01_30uL1X_30Dec12_peaks.txt";

            string peaksTestFile = OrbitrapPeakFile_scans5500_6500;


            //string massTagFile = @"\\protoapps\UserData\Slysz\Data\MassTags\QCShew_Formic_MassTags_Bin10_all.txt";
            string massTagFile = @"D:\Csharp\ConosleApps\LocalServer\IQ\QCShew_Formic_MassTags_Bin10_all.txt"; //2 of 2 work
            //string massTagFile = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\QCShew_Formic_MassTags_Bin10_all.txt";//home

            //SK
            massTagFile = @"D:\Csharp\ConosleApps\LocalServer\IQ\DB01_Targets.txt";//2 of 2 work DB01_Targets.txt
            //massTagFile = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\DB01_Targets.txt"; //2 of 2 work DB01_Targets.txt

            Run run = RunUtilities.CreateAndAlignRun(testFile, peaksTestFile);


            TargetCollection mtc = new TargetCollection();
            MassTagFromTextFileImporter mtimporter = new MassTagFromTextFileImporter(massTagFile);
            mtc = mtimporter.Import();

            int testMassTagID = 24800;

            //SK
            testMassTagID = 0;

            run.CurrentMassTag = (from n in mtc.TargetList where n.ID == testMassTagID && n.ChargeState == 2 select n).First();

            DeuteratedTargetedWorkflowParameters parameters = new DeuteratedTargetedWorkflowParameters();
            //TargetedWorkflowParameters parameters = new BasicTargetedWorkflowParameters();

            parameters.ChromatogramCorrelationIsPerformed = true;

            parameters.IsotopeProfileType = Globals.LabellingType.Deuterium;
            parameters.IsotopeLabelingEfficiency = 0.75;

            DeuteratedTargetedWorkflow workflow = new DeuteratedTargetedWorkflow(run, parameters);

            DeuteratedTargetedWorkflowParameters parametersTemp = (DeuteratedTargetedWorkflowParameters) workflow.WorkflowParameters;
            parametersTemp.IsotopeLabelingEfficiency = 0.60;
            //BasicTargetedWorkflow workflow = new BasicTargetedWorkflow(run, parameters);
            workflow.Execute();



            MassTagResult result = run.ResultCollection.GetTargetedResult(run.CurrentMassTag) as MassTagResult;

            if (result.FailedResult)
            {
                Console.WriteLine(result.ErrorDescription);
            }

            Assert.IsFalse(result.FailedResult);


            result.DisplayToConsole();

            Assert.IsNotNull(result.IsotopicProfile);
            Assert.IsNotNull(result.ScanSet);
            Assert.IsNotNull(result.ChromPeakSelected);
            Assert.AreEqual(2, result.IsotopicProfile.ChargeState);

            //Assert.AreEqual(718.41m, (decimal)Math.Round(result.IsotopicProfile.GetMZ(), 2));
            Assert.AreEqual(967.86m, (decimal) Math.Round(result.IsotopicProfile.GetMZ(), 2));

            //Assert.AreEqual(5947m, (decimal)Math.Round(result.ChromPeakSelected.XValue));

            Assert.IsNotNull(result.ChromCorrelationData);

            foreach (var dataItem in result.ChromCorrelationData.CorrelationDataItems)
            {
                Console.WriteLine(dataItem);
            }


        }

        [Test]
        public void AlternateConstructor_targetedWorkflowNoAlignment()
        {
            string executorParameterFile = @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\QCShew_OrbiStandard_workflowExecutorParameters.xml";
            BasicTargetedWorkflowExecutorParameters executorParameters = new BasicTargetedWorkflowExecutorParameters();
            executorParameters.LoadParameters(executorParameterFile);
            string resultsFolderLocation = executorParameters.ResultsFolder;

            string testDatasetPath = @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";

            //SK
            testDatasetPath = @"E:\Gly09_Velos3_Jaguar_230nL30_C15_DB01_30uL1X_30Dec12.raw";

            string testDatasetName = "QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18";

            //SK
            testDatasetName = "Gly09_Velos3_Jaguar_230nL30_C15_DB01_30uL1X_30Dec12.raw";

            string expectedResultsFilename = resultsFolderLocation + "\\" + testDatasetName + "_results.txt";
            if (File.Exists(expectedResultsFilename))
            {
                File.Delete(expectedResultsFilename);
            }

            var basicTargetedWorkflowParameters = new BasicTargetedWorkflowParameters();
            BasicTargetedWorkflow workflow = new BasicTargetedWorkflow(basicTargetedWorkflowParameters);

            TargetedWorkflowExecutor executor = new BasicTargetedWorkflowExecutor(executorParameters, workflow, testDatasetPath);
            executor.Execute();

            Assert.IsTrue(File.Exists(expectedResultsFilename));
        }
    
        [Test]
        public void TEST_N14N15_WorkflowTest1()//Must pass N15
        {
            // See:  https://jira.pnnl.gov/jira/browse/OMCS-409

            Run run = RunUtilities.CreateAndAlignRun(bruker9t_samplefile1, bruker9t_peaksfile1);

            string targetsFile =
                //@"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\N14N15_standard_testing\Targets\POnly_MassTagsMatchingInHalfOfDatasets_Filtered0.45-0.47NET_first18.txt";
                @"D:\Csharp\ConosleApps\LocalServer\IQ\IQDemo\N14N15\N14N15_standard_testing\Targets\POnly_MassTagsMatchingInHalfOfDatasets_Filtered0.45-0.47NET_first18.txt";
            //@"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\IQDemo\N14N15\N14N15_standard_testing\Targets\POnly_MassTagsMatchingInHalfOfDatasets_Filtered0.45-0.47NET_first18.txt";
            MassTagFromTextFileImporter importer = new MassTagFromTextFileImporter(targetsFile);
            var targetCollection = importer.Import();


            run.CurrentMassTag = targetCollection.TargetList.FirstOrDefault(p => p.ChargeState == 1);

            N14N15Workflow2Parameters parameters = new N14N15Workflow2Parameters();

            //parameters.LoadParameters(FileRefs.ImportedData + "\\" + "importedN14N15WorkflowParameters.xml");
            parameters.LoadParameters(@"D:\Csharp\ConosleApps\LocalServer\IQ\IQDemo\N14N15\N14N15_standard_testing\Parameters" + "\\" + "N14N15WorkflowParameters1.xml");
            //parameters.LoadParameters(@"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\IQDemo\N14N15\N14N15_standard_testing\Parameters" + "\\" + "N14N15WorkflowParameters1.xml");

            //sk
            parameters.NumPeaksUsedInQuant = 3;
            parameters.ChromPeakDetectorPeakBR = 2;
            parameters.ChromPeakDetectorSigNoise = 2;
            parameters.ChromSmootherNumPointsInSmooth = 15;
            parameters.ChromGenTolerance = 10;

            //sk
            parameters.ChromGenTolerance = 25;
            parameters.MSToleranceInPPM = 25;
            parameters.TargetedFeatureFinderToleranceInPPM = 25;
            parameters.MultipleHighQualityMatchesAreAllowed = true;
            parameters.NumMSScansToSum = 5;


            parameters.SaveParametersToXML(
                //@"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\N14N15_standard_testing\Parameters\N14N15WorkflowParameters1_test.xml");
                @"D:\Csharp\ConosleApps\LocalServer\IQ\IQDemo\N14N15\N14N15_standard_testing\Parameters\N14N15WorkflowParameters1_test.xml");
            //@"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\IQDemo\N14N15\N14N15_standard_testing\Parameters\N14N15WorkflowParameters1_test.xml");
            Console.WriteLine(parameters.ToStringWithDetails());

            N14N15Workflow2 workflow = new N14N15Workflow2(run, parameters);


            workflow.Execute();
            Assert.IsTrue(run.ResultCollection.ResultType == Globals.ResultType.N14N15_TARGETED_RESULT);

            //TestUtilities.DisplayXYValues(workflow.ChromatogramXYData);

            var result = run.ResultCollection.GetTargetedResult(run.CurrentMassTag) as N14N15_TResult;

            result.DisplayToConsole();

            Assert.AreEqual(23085448, result.Target.ID);
            Assert.AreEqual(1, result.IsotopicProfile.ChargeState);

            Assert.IsNotNull(result.ScanSet);
            Assert.IsNotNull(result.ChromPeakSelected);
            Assert.IsNotNull(result.ChromPeakSelectedN15);

            Assert.AreEqual(1639.3m, (decimal)Math.Round(result.ChromPeakSelected.XValue, 1));
            Assert.AreEqual(1638.5m, (decimal)Math.Round(result.ChromPeakSelectedN15.XValue, 1));

            Assert.IsNotNull(result.IsotopicProfile);
            Assert.IsNotNull(result.IsotopicProfileLabeled);

            Console.WriteLine("theor monomass= \t" + result.Target.MonoIsotopicMass);
            Console.WriteLine("monomass= \t" + result.IsotopicProfile.MonoIsotopicMass);
            Console.WriteLine("monomassN15= \t" + result.IsotopicProfileLabeled.MonoIsotopicMass);

            Console.WriteLine("monoMZ= \t" + result.IsotopicProfile.MonoPeakMZ);
            Console.WriteLine("monoMZN15= \t" + result.IsotopicProfileLabeled.MonoPeakMZ);

            Console.WriteLine("ppmError= \t" + result.GetMassErrorBeforeAlignmentInPPM());

            Console.WriteLine("Database NET= " + result.Target.NormalizedElutionTime);
            Console.WriteLine("Result NET= " + result.GetNET());
            Console.WriteLine("Result NET Error= " + result.GetNETAlignmentError());
            Console.WriteLine("NumChromPeaksWithinTol= " + result.NumChromPeaksWithinTolerance);
            Console.WriteLine("NumChromPeaksWithinTolN15= " + result.NumChromPeaksWithinToleranceForN15Profile);

        }

        [Test]//Must Pass//current console
        public void AlternateConstructorTargetedWorkflowNoAlignmentDeuteratedConsole()
        {
            Console.WriteLine("\nGlyQIQ");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            System.DateTime starttime = DateTime.Now;

            string executorParameterFile = @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\QCShew_OrbiStandard_workflowExecutorParameters.xml";

            //SK
            executorParameterFile = @"D:\Csharp\ConosleApps\LocalServer\IQ\QCShew_OrbiStandard_workflowExecutorParametersSK.xml";
            executorParameterFile = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\QCShew_OrbiStandard_workflowExecutorParametersSK_Home.xml";
            executorParameterFile = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\DB01_OrbiStandard_workflowExecutorParametersSK_Home Full.xml";
            executorParameterFile = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\SN111-SN117_OrbiStandard_workflowExecutorParametersSK_Home.xml";
            executorParameterFile = @"E:\ScottK\IQ\RunFiles\SN111-SN117_OrbiStandard_workflowExecutorParametersSK_Home HM.xml";


            string testDatasetPath = @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";

            //SK
            testDatasetPath = @"E:\Gly09_Velos3_Jaguar_230nL30_C15_DB01_30uL1X_30Dec12.raw";
            testDatasetPath = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data\Gly09_Velos3_Jaguar_230nL30_C15_DB01_30uL1X_30Dec12.raw";
            testDatasetPath = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data\Gly09_Velos3_Jaguar_200nL_C12_SN111SN114_3X_23Dec12.raw";
            testDatasetPath = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data\Gly09_Velos3_Jaguar_200nL_C12_SN112SN115_3X_23Dec12.raw";
            testDatasetPath = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data\Gly09_Velos3_Jaguar_200nL_C12_SN113SN116_3X_23Dec12.raw";

            testDatasetPath = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data\Gly09_Velos3_Jaguar_200nL_C13_SN117SN120_3X_23Dec12.raw";
            testDatasetPath = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data\Gly09_Velos3_Jaguar_200nL_C13_SN118SN121_3X_23Dec12.raw";
            testDatasetPath = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data\Gly09_Velos3_Jaguar_200nL_C13_SN119SN122_3X_23Dec12.raw";
            testDatasetPath = @"E:\ScottK\GetPeaks Data\Sigma_Standards_LC\Gly09_Velos3_Jaguar_200nL_C13_SN119SN122_3X_23Dec12.raw";


            string testDatasetName = "QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18";

            //SK
            testDatasetName = "Gly09_Velos3_Jaguar_230nL30_C15_DB01_30uL1X_30Dec12";
            testDatasetName = "Gly09_Velos3_Jaguar_200nL_C12_SN111SN114_3X_23Dec12";
            testDatasetName = "Gly09_Velos3_Jaguar_200nL_C12_SN112SN115_3X_23Dec12";
            testDatasetName = "Gly09_Velos3_Jaguar_200nL_C12_SN113SN116_3X_23Dec12";

            testDatasetName = "Gly09_Velos3_Jaguar_200nL_C13_SN117SN120_3X_23Dec12";
            testDatasetName = "Gly09_Velos3_Jaguar_200nL_C13_SN118SN121_3X_23Dec12";
            testDatasetName = "Gly09_Velos3_Jaguar_200nL_C13_SN119SN122_3X_23Dec12";


            RunMeIQGlyQ wizard = new RunMeIQGlyQ();
            wizard.ExecuteDeuteratedTargetedWorkflow(executorParameterFile, testDatasetPath, testDatasetName);

            //Test
            //DeuteratedTargetedWorkflowExecutorParameters executorParameters = new DeuteratedTargetedWorkflowExecutorParameters();
            //executorParameters.LoadParameters(executorParameterFile);
            //string resultsFolderLocation = executorParameters.ResultsFolder;

            BasicTargetedWorkflowExecutorParameters executorParameters = new BasicTargetedWorkflowExecutorParameters();
            executorParameters.LoadParameters(executorParameterFile);
            string resultsFolderLocation = executorParameters.ResultsFolder;

            string expectedResultsFilename = resultsFolderLocation + "\\" + testDatasetName + "_results.txt";
            Assert.IsTrue(File.Exists(expectedResultsFilename));


            stopWatch.Stop();

            System.DateTime stoptime = DateTime.Now;
            Console.WriteLine("This started at " + starttime + " and ended at" + stoptime);
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to run the test");
            Console.WriteLine("");
            
            Console.WriteLine("");
            Console.Write("Finished.  Press Return to Exit");//home 2.26 seconds
        }

        [Test]//Must Pass// best simple peak find
        public void TargetedWorkflowNoAlignmentDeuteratedDb10HighMannose()
        {
            Console.WriteLine("\nGlyQIQ");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            System.DateTime starttime = DateTime.Now;

            string executorParameterFile = @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\QCShew_OrbiStandard_workflowExecutorParameters.xml";

            //SK
            executorParameterFile = @"D:\Csharp\ConosleApps\LocalServer\IQ\QCShew_OrbiStandard_workflowExecutorParametersSK.xml";
            executorParameterFile = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\QCShew_OrbiStandard_workflowExecutorParametersSK_Home.xml";
            executorParameterFile = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\DB01_OrbiStandard_workflowExecutorParametersSK_Home Full.xml";
            executorParameterFile = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\SN111-SN117_OrbiStandard_workflowExecutorParametersSK_Home.xml";
            executorParameterFile = @"E:\ScottK\IQ\RunFiles\SN111-SN117_OrbiStandard_workflowExecutorParametersSK_Work HM Test.xml";


            string testDatasetPath = @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";

            //SK
            testDatasetPath = @"E:\Gly09_Velos3_Jaguar_230nL30_C15_DB01_30uL1X_30Dec12.raw";
            testDatasetPath = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data\Gly09_Velos3_Jaguar_230nL30_C15_DB01_30uL1X_30Dec12.raw";
            testDatasetPath = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data\Gly09_Velos3_Jaguar_200nL_C12_SN111SN114_3X_23Dec12.raw";
            testDatasetPath = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data\Gly09_Velos3_Jaguar_200nL_C12_SN112SN115_3X_23Dec12.raw";
            testDatasetPath = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data\Gly09_Velos3_Jaguar_200nL_C12_SN113SN116_3X_23Dec12.raw";

            testDatasetPath = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data\Gly09_Velos3_Jaguar_200nL_C13_SN117SN120_3X_23Dec12.raw";
            testDatasetPath = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data\Gly09_Velos3_Jaguar_200nL_C13_SN118SN121_3X_23Dec12.raw";
            testDatasetPath = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data\Gly09_Velos3_Jaguar_200nL_C13_SN119SN122_3X_23Dec12.raw";
            testDatasetPath = @"E:\ScottK\GetPeaks Data\Diabetes_LC\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw";


            string testDatasetName = "QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18";

            //SK
            testDatasetName = "Gly09_Velos3_Jaguar_230nL30_C15_DB01_30uL1X_30Dec12";
            testDatasetName = "Gly09_Velos3_Jaguar_200nL_C12_SN111SN114_3X_23Dec12";
            testDatasetName = "Gly09_Velos3_Jaguar_200nL_C12_SN112SN115_3X_23Dec12";
            testDatasetName = "Gly09_Velos3_Jaguar_200nL_C12_SN113SN116_3X_23Dec12";

            testDatasetName = "Gly09_Velos3_Jaguar_200nL_C13_SN117SN120_3X_23Dec12";
            testDatasetName = "Gly09_Velos3_Jaguar_200nL_C13_SN118SN121_3X_23Dec12";
            testDatasetName = "Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12";


            RunMeIQGlyQ wizard = new RunMeIQGlyQ();
            wizard.ExecuteDeuteratedTargetedWorkflow(executorParameterFile, testDatasetPath, testDatasetName);

            //Test
            //DeuteratedTargetedWorkflowExecutorParameters executorParameters = new DeuteratedTargetedWorkflowExecutorParameters();
            //executorParameters.LoadParameters(executorParameterFile);
            //string resultsFolderLocation = executorParameters.ResultsFolder;

            BasicTargetedWorkflowExecutorParameters executorParameters = new BasicTargetedWorkflowExecutorParameters();
            executorParameters.LoadParameters(executorParameterFile);
            string resultsFolderLocation = executorParameters.ResultsFolder;

            string expectedResultsFilename = resultsFolderLocation + "\\" + testDatasetName + "_results.txt";
            Assert.IsTrue(File.Exists(expectedResultsFilename));


            var importer = new DeuteratedTargetedWorkflowImporter(expectedResultsFilename);
            TargetedResultRepository repository = importer.Import();

            Assert.AreEqual(3, repository.Results.Count);

            DeuteratedTargetedResultDTO result1 = repository.Results[0] as DeuteratedTargetedResultDTO;

            Assert.AreEqual(2174, result1.ScanLC);
            Assert.AreEqual(0.62070000171661377d, result1.RatioDH);

            DeuteratedTargetedResultDTO result2 = repository.Results[1] as DeuteratedTargetedResultDTO;

            Assert.AreEqual(2174, result2.ScanLC);
            Assert.AreEqual(0.42230001091957092d, result2.RatioDH);
            DeuteratedTargetedResultDTO result3 = repository.Results[2] as DeuteratedTargetedResultDTO;

            Assert.AreEqual(0, result3.ScanLC);
            

            

            stopWatch.Stop();

            System.DateTime stoptime = DateTime.Now;
            Console.WriteLine("This started at " + starttime + " and ended at" + stoptime);
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to run the test");
            Console.WriteLine("");

            Console.WriteLine("");
            Console.Write("Finished.  Press Return to Exit");//home 2.26 seconds
        }

        #region old iQ
        //[Test]//Must Pass// best fragmentation workflow
        //public void TargetedWorkflowNoAlignmentDeuteratedDb10HighMannoseInsource()
        //{
        //    //LMdemo test = new LMdemo();
        //    //FitHost test2 = new FitHost();

        //    //List<PNNLOmics.Data.XYData> loadFragmentBaseEic = new List<PNNLOmics.Data.XYData>();
        //    //MixtureModelFX.MixtureModel(loadFragmentBaseEic, 2);   

        //    Console.WriteLine("\nGlyQIQ");
        //    Stopwatch stopWatch = new Stopwatch();
        //    stopWatch.Start();
        //    System.DateTime starttime = DateTime.Now;

        //    string executorParameterFile = @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\QCShew_OrbiStandard_workflowExecutorParameters.xml";


        //    //IqWorkflow
        //    //ExecuteMultipleTargetsTest1


        //    //SK
        //    executorParameterFile = @"D:\Csharp\ConosleApps\LocalServer\IQ\QCShew_OrbiStandard_workflowExecutorParametersSK.xml";
        //    executorParameterFile = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\QCShew_OrbiStandard_workflowExecutorParametersSK_Home.xml";
        //    executorParameterFile = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\DB01_OrbiStandard_workflowExecutorParametersSK_Home Full.xml";
        //    executorParameterFile = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\SN111-SN117_OrbiStandard_workflowExecutorParametersSK_Home.xml";
        //    executorParameterFile = @"E:\ScottK\IQ\RunFiles\SN111-SN117_OrbiStandard_workflowExecutorParametersSK_Work HM Test.xml";
        //    //executorParameterFile = @"E:\ScottK\IQ\RunFiles\SN111-SN117_OrbiStandard_workflowExecutorParametersSK_Work Full.xml";
        //    //<TargetsFilePath>E:\ScottK\IQ\RunFiles\Gly09_Velos3_Jaguar_230nL30_C15_SN123_3X_01Jan13_R8_resultsSK_vIQ HM SNx1 Man7.txt</TargetsFilePath>

        //    string testDatasetPath = @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";

        //    //SK
        //    testDatasetPath = @"E:\Gly09_Velos3_Jaguar_230nL30_C15_DB01_30uL1X_30Dec12.raw";
        //    testDatasetPath = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data\Gly09_Velos3_Jaguar_230nL30_C15_DB01_30uL1X_30Dec12.raw";
        //    testDatasetPath = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data\Gly09_Velos3_Jaguar_200nL_C12_SN111SN114_3X_23Dec12.raw";
        //    testDatasetPath = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data\Gly09_Velos3_Jaguar_200nL_C12_SN112SN115_3X_23Dec12.raw";
        //    testDatasetPath = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data\Gly09_Velos3_Jaguar_200nL_C12_SN113SN116_3X_23Dec12.raw";

        //    testDatasetPath = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data\Gly09_Velos3_Jaguar_200nL_C13_SN117SN120_3X_23Dec12.raw";
        //    testDatasetPath = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data\Gly09_Velos3_Jaguar_200nL_C13_SN118SN121_3X_23Dec12.raw";
        //    testDatasetPath = @"L:\PNNL Files\CSharp\ConosleApps\LocalServer\IQ\Sarwall\Raw Data\Gly09_Velos3_Jaguar_200nL_C13_SN119SN122_3X_23Dec12.raw";
        //    testDatasetPath = @"E:\ScottK\GetPeaks Data\Diabetes_LC\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw";


        //    string testDatasetName = "QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18";

        //    //SK
        //    testDatasetName = "Gly09_Velos3_Jaguar_230nL30_C15_DB01_30uL1X_30Dec12";
        //    testDatasetName = "Gly09_Velos3_Jaguar_200nL_C12_SN111SN114_3X_23Dec12";
        //    testDatasetName = "Gly09_Velos3_Jaguar_200nL_C12_SN112SN115_3X_23Dec12";
        //    testDatasetName = "Gly09_Velos3_Jaguar_200nL_C12_SN113SN116_3X_23Dec12";

        //    testDatasetName = "Gly09_Velos3_Jaguar_200nL_C13_SN117SN120_3X_23Dec12";
        //    testDatasetName = "Gly09_Velos3_Jaguar_200nL_C13_SN118SN121_3X_23Dec12";
        //    testDatasetName = "Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12";


        //    RunMeIQGlyQ wizard = new RunMeIQGlyQ();
        //    wizard.ExecuteFragmentaterTargetedWorkflow(executorParameterFile, testDatasetPath, testDatasetName);
            

        //    //Test
        //    //DeuteratedTargetedWorkflowExecutorParameters executorParameters = new DeuteratedTargetedWorkflowExecutorParameters();
        //    //executorParameters.LoadParameters(executorParameterFile);
        //    //string resultsFolderLocation = executorParameters.ResultsFolder;

        //    BasicTargetedWorkflowExecutorParameters executorParameters = new BasicTargetedWorkflowExecutorParameters();
        //    executorParameters.LoadParameters(executorParameterFile);
        //    string resultsFolderLocation = executorParameters.ResultsFolder;

        //    Console.WriteLine("Parameter File: " + executorParameters.TargetsFilePath);

        //    string expectedResultsFilename = resultsFolderLocation + "\\" + testDatasetName + "_results.txt";
        //    Assert.IsTrue(File.Exists(expectedResultsFilename));

            

        //    #region import and check

        //    var importer = new DeuteratedTargetedWorkflowImporter(expectedResultsFilename);
        //    DeconTools.Workflows.Backend.Results.TargetedResultRepository repository = importer.Import();

        //    Assert.AreEqual(3, repository.Results.Count);

        //    DeuteratedTargetedResultDTO result1 = repository.Results[0] as DeuteratedTargetedResultDTO;

        //    Assert.AreEqual(2174, result1.ScanLC);
        //    Assert.AreEqual(0.62070000171661377d, result1.RatioDH);

        //    DeuteratedTargetedResultDTO result2 = repository.Results[1] as DeuteratedTargetedResultDTO;

        //    Assert.AreEqual(2174, result2.ScanLC);
        //    Assert.AreEqual(0.42230001091957092d, result2.RatioDH);
        //    DeuteratedTargetedResultDTO result3 = repository.Results[2] as DeuteratedTargetedResultDTO;

        //    Assert.AreEqual(0, result3.ScanLC);




        //    stopWatch.Stop();

        //    System.DateTime stoptime = DateTime.Now;
        //    Console.WriteLine("This started at " + starttime + " and ended at" + stoptime);
        //    Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to run the test");
        //    Console.WriteLine("");

        //    Console.WriteLine("");
        //    Console.Write("Finished.  Press Return to Exit");//home 2.26 seconds

        //    #endregion
        //}
        #endregion

        
        [Test]
        public void writeParameterFilesForPic(string targetsFileNameIn, string processingParameters, string divideTargetsParametersName, string dividetargetsFolder, string specificParameters, string factors, string masterLocation, string workerLocation)
        {
            //input

            
            //string MasterComputerName = "Pub-6000";
            //TODO
            //we need to writeout a replacement Parameters_DivideTargetsPIC with the new library
            string divideTargetsParameterFile = dividetargetsFolder + @"\" + divideTargetsParametersName;

            string baseTargetsFile;
            string fullTargetPath;
            string textFileEnding;
            ParameterDivideTargets parameters = LoadParameters.SetupDivideTargetParameters(divideTargetsParameterFile, out baseTargetsFile, out fullTargetPath, out textFileEnding);
            parameters.TargetsFileName = targetsFileNameIn;
            Console.WriteLine("We are now updating the Parameters_DivideTargets file with the correct targets");
            if (File.Exists(dividetargetsFolder + @"\" + targetsFileNameIn))
            {
                Console.WriteLine("The New Target File Exists: " + dividetargetsFolder + @"\" + targetsFileNameIn);
            }
            else
            {
                Console.WriteLine("The New Targets File Does Not Exist");
            }
            //parameters.WriteParameters(parameters, divideTargetsParameterFile);
            parameters.WriteParameters(divideTargetsParameterFile);

           
            //FragmentedTargetedWorkflowParameters_Velos_DH.txt

            //changeable parameters
            string TargetsFilePath = "";
            string RawFile = "";
            string RawFolderHome = "";
            string RawFolderHomePICFS = "";
            //string DivideTargetsParameters = "";
            //string ProcessingParameters = "";

            EnumerationDataset dataType;
            dataType  = EnumerationDataset.Diabetes;
            //dataType = EnumerationDataset.SPINExactiveMuddiman;
            //dataType = EnumerationDataset.SPINExactive;

            //TargetsFilePath = "L_10_IQ_TargetsFirstAll.txt";
            //TargetsFilePath = "L_10_IQ_TargetsFirst3.txt";
            //TargetsFilePath = "L_PSA21_TargetsFirstAll.txt";
            //TargetsFilePath = "L_10_IQ_MuddimanTargetsH.txt";
            //TargetsFilePath = "L_PSA21_IQ_MuddimanH.txt";
            TargetsFilePath = targetsFileNameIn;

            //string basicTargetedAlignmentWorkflowParametersFileName = "TargetedAlignmentWorkflowParameters.xml";
            //string basicTargetedWorkflowParameters = "BasicTargetedWorkflowParameters.xml";

            
            //ProcessingParameters = "FragmentedTargetedWorkflowParameters_Velos_DH.txt";

            string randomIQFile = "SN111-SN117_OrbiStandard_workflowExecutorParametersSK_Work HM Test.xml";

            //string PexecLocation = @"C:\Download\SysinternalsSuite\psexec.exe";
            //string ipAddressForExecution = "192.168.3.20";
            //string NodeName = "pub-2000";

            string ipAddressForExecution = "192.168.3.22"; string NodeName = "PUB-2002";

            //string divideTargetsParameters = "Parameters_DivideTargetsPIC.txt";//input from parameters
            string divideTargetsParametersNode = "Parameters_DivideTargetsPICNodes.txt";

            //string ipAddressForExecution = "192.168.3.23"; string NodeName = "PUB-2003";
            string ZipBatchName = "Zip.bat";
            string ZipParameterFile= "FilesToZIP.txt";
            string LaunchJobsBatchFileName = "PIC_LaunchJobs.bat";

            string testBatchName = "testBatch.bat";
            string globalMultiSleepFile = "PIC_MultiSleepParameterFileGlobal.txt";
            string globalMultiSleepFileListFile = "PIC_MultiSleepParameterFileGlobal_List.txt";

            string ApplicationSetupParameters = "ApplicationSetupParameters.txt";
            string ApplicationSetupParametersWorker = "ApplicationSetupParametersF.txt";

            string adminAccount = "Administrator";
            string adminPassword = "Pub123456";
            //string SecondLaunchLocation = @"\\pub-1000\Shared_PICFS\ToPIC\PIC_RunMeSeccond_PUB100X.bat";
            string SecondLaunchLocation = @"PIC_RunMeSeccond_PUB100X.bat";
            string ConsolidationAtNodeLevelList = "ConsolidationParameterForAllNodes.txt";
            //string nodeConsolidation = @"PIC_NodeConsolidation.bat";

            string sleepParameterFile = "PIC_SleepParameterFile_" + NodeName + ".txt";
            string multiSleepParameterFile = "PIC_MultiSleepParameterFile.txt";
            string GoCrazyCopyParameterFile = "PIC_ParametarFileCopyExponential.txt";

            //C:\Download\SysinternalsSuite\psexec.exe \\192.168.3.22 -u 192.168.3.22\Administrator -p Pub123456 "E:\\NodeShareFolder\PIC_RunMeSeccond_PUB100X.bat"
            string adminAccountMasterPub10000 = "Administrator";
            string adminPasswordMasterPub10000 = "Pub123456";
            string ipAddressForExecutionMaster = "192.168.3.10";
            string NodeNameMaster = "pub-1000";

            string nodeShareFolderLocationOnNode = @"E:\";

            //string SecondLaunchLocation = @"F\ScottK\ToPIC\PUB100X_CopyResultBack.bat";//we need an xcopy here

            //this is for copying the raw file around.  actual run is via set strings
            switch (dataType)
            {
                case EnumerationDataset.SPINExactive:
                    {
                        
                        RawFile = "Gly09_SN130_4Mar13_Cheetah_C14_220nL_SPIN_1900V_1600mlmin_22Torr_100C_100kHDR2M2mbar.raw";
                        RawFolderHome = @"\\pub-1000\Shared_PICFS\RawData\2013_02_18 SPIN Exactive04";
                        //RawFolderHomePICFS = @"\\picfs\projects\DMS\ScottK\RawData\2012_12_24 Velos 3
                    }
                    break;
                case EnumerationDataset.SPINExactiveMuddiman:
                    {
                        //ALSO SET in SET STRINGS
                        //A 126-131
                        RawFile = "Gly09_SN126131_6Mar13_Cheetah_C18_50cm_220nL_SPIN_1900V_1600mlmin_22Torr_100C_100kHDR2M2mbar.raw";
                        
                        //B 125-128
                        //RawFile = "Gly09_SN125128_7Mar13_Cheetah_C18_50cm_220nL_SPIN_1900V_1600mlmin_22Torr_100C_100kHDR2M2mbar.raw";
                        
                        //C 127-132
                        //RawFile = "Gly09_SN127132_6Mar13_Cheetah_C18_50cm_220nL_SPIN_1900V_1600mlmin_22Torr_100C_100kHDR2M2mbar.raw";
                        
                        
                        RawFolderHome = @"\\pub-1000\Shared_PICFS\RawData\2013_02_18 SPIN Exactive04";
                        //RawFolderHomePICFS = @"\\picfs\projects\DMS\ScottK\RawData\2012_12_24 Velos 3
                    }
                    break;
                case EnumerationDataset.SN123R8:
                    {
                        RawFile = "Gly09_Velos3_Jaguar_230nL30_C15_SN123_3X_01Jan13_R8.raw";
                        RawFolderHome = @"\\pub-1000\Shared_PICFS\RawData\2012_12_24 Velos 3";
                        //RawFolderHomePICFS = @"\\picfs\projects\DMS\ScottK\RawData\2012_12_24 Velos 3
                    }
                    break;
                case EnumerationDataset.Diabetes:
                    {
                        RawFile = "Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw";
                        RawFolderHome = @"\\pub-1000\Shared_PICFS\RawData\2012_12_24 Velos 3";
                        //RawFolderHomePICFS = @"\\picfs\projects\DMS\ScottK\RawData\2012_12_24 Velos 3
                    }
                    break;
                default:
                    {
                        RawFile = "Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw";
                        RawFolderHome = @"\\pub-1000\Shared_PICFS\RawData\2012_12_24 Velos 3";
                        //RawFolderHomePICFS = @"\\picfs\projects\DMS\ScottK\RawData\2012_12_24 Velos 3
                    }
                    break;
            }
            
            string PubName = System.Environment.MachineName;

            
            //No change parameters
            //string writeFolder = @"F:\ScottK\ToPic";
            string Pub1000HomeFolder = @"E:\ScottK\Shared_PICFS\ToPIC";
            string onPub1000WorkingParametersFolder = Pub1000HomeFolder + @"\" + "WorkingParameters";
            string LaunchInitializion = @"\\pub-1000\Shared_PICFS\ToPIC";
            string LibraryHome = @"\\pub-1000\Shared_PICFS\ToPIC\WorkingParameters";
            
            string ApplciationHome = @"\\pub-1000\Shared_PICFS\ToPIC\GlyQ-IQ Application\Release";
            string ResultsFutureHome = @"\\" + NodeNameMaster + @"\Shared_PICFS\ToPIC\Results\" + PubName + "_Results";//Set destination pub
            string LibraryOnPICFS = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\WorkingParameters";
            
            //ZippedFolder
            string RunMeFirstFromPICFSZipped = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\Zipped";
            string RunMeFirstFromPub1000Zipped = @"E:\ScottK\Shared_PICFS\ToPIC\Zipped";
            string ZippedPub100XSetup = @"\\pub-1000\Shared_PICFS\ToPIC\Zipped";

            //ZippedApp
            string RunMeFirstFromPICFSZippedApp = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ Zip";
            string RunMeFirstFromPub1000ZippedApp = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ Zip";
           

            //initial pub 1000 setup
            //this is the uploaded application setup program files living on PICFS
            string RunMeFirstFromPICFSSetup = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ Application Setup";
            string RunMeFirstFromPub1000Setup = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ Application Setup";

            //this is the uploaded application program files living on PICFS
            string RunMeFirstFromPICFSApplication = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ Application";
            string RunMeFirstFromPub1000Application = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ Application";

            //this is the uploaded divideTargets program files living on PICFS
            string RunMeFirstFromPICFSdivideTargets = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ DivideTargets";
            string RunMeFirstFromPub1000divideTargets = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ DivideTargets";
            string DivideTargetsHomePub100XSetup = @"\\pub-1000\Shared_PICFS\ToPIC\GlyQ-IQ DivideTargets";
            string DivideTargets100XPub100XSetupLocation = workerLocation + @"\GlyQ-IQ DivideTargets\Release\DivideTargets.exe";


            //this is the uploaded postprocessing (from divide targets) program files living on PICFS
            string RunMeFirstFromPICFSpostProcessing = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ PostProcessing";
            string RunMeFirstFromPub1000postProcessing = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ PostProcessing";
            string PostProcessingHomePub100XSetup = @"\\pub-1000\Shared_PICFS\ToPIC\GlyQ-IQ PostProcessing";
            string PostProcessing100XPub100XSetupLocation = workerLocation + @"\GlyQ-IQ PostProcessing\Release\PostProcessing.exe";


            //this is the uploaded sleep (from divide targets) program files living on PICFS
            string RunMeFirstFromPICFSSleep = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ Sleep";
            string RunMeFirstFromPub1000Sleep = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ Sleep";
            string SleepExeLocation = Pub1000HomeFolder + @"\" + @"GlyQ-IQ Sleep\Release\Sleep.exe";

            //this is the uploaded sleep (from divide targets) program files living on PICFS
            string RunMeFirstFromPICFSSleepMulti = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ MultiSleep";
            string RunMeFirstFromPub1000SleepMulti = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ MultiSleep";
            string SleepMultiExeLocation = workerLocation + @"\" + @"GlyQ-IQ MultiSleep\Release\MultiSleep.exe";
            string SleepMultiHomePub100XSetup = @"\\pub-1000\Shared_PICFS\ToPIC\GlyQ-IQ MultiSleep";

            //initial pub100X setup
            //this is where the setup app will live on pub 1000
            string ApplciationHomePub100XSetupHome = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC";
            string ApplciationHomePub100XSetup = @"\\pub-1000\Shared_PICFS\ToPIC\GlyQ-IQ Application Setup";
            string Applciation100XPub100XSetupLocation = workerLocation + @"\GlyQ-IQ Application Setup\Release\IQGlyQ_Console_ParameterSetup.exe";

            string outputLocation = "";
            string nodeShareFolder = "NodeShareFolder";

            //this is the uploaded GlyQ-IQ CheckFile (from divide targets) program files living on PICFS
            string RunMeFirstFromPICFSCheckFile = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ CheckFile";
            string RunMeFirstFromPub1000CheckFile = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ CheckFile";
            string CheckFileExeLocation = workerLocation + @"\" + @"GlyQ-IQ MultiSleep\Release\WriteCheckFile.exe";
            string CheckFileHomePub100XSetup = @"\\pub-1000\Shared_PICFS\ToPIC\GlyQ-IQ CheckFile";

            //this is the uploaded GlyQ-IQ DeleteFiles (from divide targets) program files living on PICFS
            string RunMeFirstFromPICFSDeleteFilesExpCopy = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ DeleteFiles";
            string RunMeFirstFromPub1000DeleteFilesExpCopy = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ DeleteFiles";
            string DeleteFilesExpCopyExeLocation = workerLocation + @"\" + @"GlyQ-IQ DeleteFiles\Release\DeleteFiles.exe";
            string DeleteFilesExpCopyHomePub100XSetup = @"\\pub-1000\Shared_PICFS\ToPIC\GlyQ-IQ DeleteFiles";


            //this is the uploaded GlyQ-IQ CrazyFilesCopy (from Geometric copy) program files living on PICFS
            string RunMeFirstFromPICFSCrazyFilesCopy = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ CrazyFileCopy";
            string RunMeFirstFromPub1000CrazyFilesCopy = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ CrazyFileCopy";
            string CrazyFilesCopyExeLocation = workerLocation + @"\" + @"GlyQ-IQ CrazyFileCopy\Release\MultiSleep.exe";
            string CrazyFilesCopyHomePub100XSetup = @"\\pub-1000\Shared_PICFS\ToPIC\GlyQ-IQ CrazyFileCopy";

            //this is the uploaded GlyQ-IQ DivideTargetsNode (from divide targets) program files living on PICFS
            string RunMeFirstFromPICFSDivideTargetsNode = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ DivideTargetsNode";
            string RunMeFirstFromPub1000DivideTargetsNode = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ DivideTargetsNode";
            string DivideTargetsNodeExeLocation = Pub1000HomeFolder + @"\" + @"GlyQ-IQ DivideTargetsNode\Release\DivideTargetsNodes.exe";
            string DivideTargetsNodeHomePub100XSetup = @"\\pub-1000\Shared_PICFS\ToPIC\GlyQ-IQ DivideTargetsNode";

            //this is the uploaded GlyQ-IQ CombineNodeResults (from divide targets) program files living on PICFS
            string RunMeFirstFromPICFSCombineNodeResults = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ CombineNodeResults";
            string RunMeFirstFromPub1000CombineNodeResults = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ CombineNodeResults";
            string CombineNodeResultsExeLocation = Pub1000HomeFolder + @"\" + @"GlyQ-IQ CombineNodeResults\Release\CombineNodeResults.exe";
            string CombineNodeResultsHomePub100XSetup = @"\\pub-1000\Shared_PICFS\ToPIC\GlyQ-IQ CombineNodeResults";

            //this is the uploaded GlyQ-IQ GlyQ-IQ Timer (from divide targets) program files living on PICFS
            string RunMeFirstFromPICFSTimer = @"\\picfs\projects\DMS\ScottK\ScottK PUB-100X Launch Folder\ToPIC\GlyQ-IQ Timer";
            string RunMeFirstFromPub1000Timer = @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ Timer";
            string TimerExeLocation = Pub1000HomeFolder + @"\" + @"GlyQ-IQ Timer\Release\WriteTime.exe";
            string TImerResultsHomePub100XSetup = @"\\pub-1000\Shared_PICFS\ToPIC\GlyQ-IQ Timer";

            //code


            Directory.CreateDirectory(workerLocation);

            string workingRawDataFolder = workerLocation + @"\" + "RawData";

            Directory.CreateDirectory(workingRawDataFolder);

            string workingZippedFolder = workerLocation + @"\" + "Zipped";

            Directory.CreateDirectory(workingZippedFolder);

            string workingResultsFolder = masterLocation + @"\" + "Results";

            Directory.CreateDirectory(workingResultsFolder);

            string XYDataFolder = workingResultsFolder + @"\" + "XYDataWriter";

            Directory.CreateDirectory(XYDataFolder);

            string WorkingParametersFolder = workerLocation + @"\" + "WorkingParameters";

            Directory.CreateDirectory(WorkingParametersFolder);

            string AllignmentFolder = WorkingParametersFolder + @"\" + "AllignmentInfo";

            Directory.CreateDirectory(AllignmentFolder);

            string LogsFolder = WorkingParametersFolder + @"\" + "Logs";

            Directory.CreateDirectory(LogsFolder);

            string WorkingApplicationFolder = workerLocation + @"\" + @"GlyQ-IQ Application\Release";

            Directory.CreateDirectory(WorkingApplicationFolder);

            string WorkingLaunchConsoleFolder = workerLocation + @"\" + @"GlyQ-IQ Application Setup";

            Directory.CreateDirectory(WorkingLaunchConsoleFolder);

            string WorkingDivideTargetsFolder = workerLocation + @"\" + @"GlyQ-IQ DivideTargets";

            Directory.CreateDirectory(WorkingDivideTargetsFolder);

            string WorkingPostProcessingFolder = workerLocation + @"\" + @"GlyQ-IQ PostProcessing";

            Directory.CreateDirectory(WorkingPostProcessingFolder);

            string WorkingMultiSleepFolder = workerLocation + @"\" + @"GlyQ-IQ MultiSleep";

            Directory.CreateDirectory(WorkingMultiSleepFolder);

            string WorkingCheckFileFolder = workerLocation + @"\" + @"GlyQ-IQ CheckFile";

            Directory.CreateDirectory(WorkingCheckFileFolder);

            string WorkingDeleteFilesFolder = workerLocation + @"\" + @"GlyQ-IQ DeleteFiles";

            Directory.CreateDirectory(WorkingDeleteFilesFolder);

            string WorkingCrazyCopyFolder = workerLocation + @"\" + @"GlyQ-IQ CrazyFileCopy";

            Directory.CreateDirectory(WorkingCrazyCopyFolder);
            

            string LaunchFolder = masterLocation;
            Directory.CreateDirectory(LaunchFolder);


            StringListToDisk writer = new StringListToDisk();


            //workflow parameters
            string basicTargetedWorkflowParametersFileName = "BasicTargetedWorkflowParameters.xml";
            List<string> basicTargetedWorkflowParameters = SetBasicTargetedWorkflowParameters();
            outputLocation = WorkingParametersFolder + @"\" + basicTargetedWorkflowParametersFileName;
            writer.toDiskStringList(outputLocation, basicTargetedWorkflowParameters);



            //GlyQ-IQ Parameters
            string GlyQ_IQFileName = "L_RunFile.xml";
            List<string> GlyQIQParameters = SetBasicGlyQIQParameters(TargetsFilePath, WorkingParametersFolder, workingRawDataFolder, workingResultsFolder);
            outputLocation = WorkingParametersFolder + @"\" + GlyQ_IQFileName;
            writer.toDiskStringList(outputLocation, GlyQIQParameters);



            //allignment parameters
            string basicTargetedAlignmentWorkflowParametersFileName = "TargetedAlignmentWorkflowParameters.xml";
            List<string> targetedAlignmentWorkflowParametersData = SetTargetedAlignmentWorkflowParameters();
            outputLocation = WorkingParametersFolder + @"\" + basicTargetedAlignmentWorkflowParametersFileName;
            writer.toDiskStringList(outputLocation, targetedAlignmentWorkflowParametersData);

            ///batch files


            //PIC_DeleteFiles.bat
            string PIC_DeleteFilesFileName = "PIC_DeleteFiles.bat";
            List<string> DeleteFilesData = new List<string>();
            DeleteFilesData.Add("if exist " + "\"" + workerLocation + "\"" + " rmdir /s /q " + "\"" + workerLocation + "\"" + @" /s/q");
            WriteBatchFile(DeleteFilesData, PIC_DeleteFilesFileName, LaunchFolder);
            

            
            //PIC_RunGlyQConsole.bat
            string PIC_RunGlyQConsoleFileName = "PIC_RunGlyQConsole.bat";
            List<string> RunGlyQConsoleData = new List<string>();
            //RunGlyQConsoleData.Add("\"" + WorkingApplicationFolder + @"\IQGlyQ_Console.exe" + "\"");
            RunGlyQConsoleData.Add("Call " + WorkingParametersFolder + @"\RunMeThreads.bat");
            WriteBatchFile(RunGlyQConsoleData, PIC_RunGlyQConsoleFileName, LaunchFolder);


            //PIC_PostProcessing.bat
            string PIC_PostProcessingFileName = "PIC_RunPostProcessing.bat";
            List<string> PostProcessingData = new List<string>();
            PostProcessingData.Add("\"" + PostProcessing100XPub100XSetupLocation + "\"" + " " + "\"" + WorkingParametersFolder + @"\LocksFolder" + "\"" + " " + "\"" + "LockController.txt" + "\"" + " " + "\"" + workerLocation + "\"");
            WriteBatchFile(PostProcessingData, PIC_PostProcessingFileName, LaunchFolder);

            ////PIC_LockChecker.bat
            //string PIC_LockCheckerFileName = "PIC_LockChecker.bat";
            //List<string> LockCheckerData = new List<string>();
            //PostProcessingData.Add("\"" + SleepMultiExeLocation + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + "LockCheckerParameterFile.txt" + "\"");
            //WriteBatchFile(LockCheckerData, PIC_LockCheckerFileName, LaunchFolder);
            ////lock checker parameter file


            //PUB1000_CopyFiles.bat
            //string PUB100X_CopyFilesName = "PUB100X_CopyFiles.bat";
            //List<string> CopyFilesData = new List<string>();
            //CopyFilesData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + TargetsFilePath + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + TargetsFilePath + "\"" + @" /S");
            //CopyFilesData.Add("echo f | xcopy /Y " + "\"" + RawFolderHome + @"\" + RawFile + "\"" + " " + "\"" + workingRawDataFolder + @"\" + RawFile + "\"" + @" /S");
            //CopyFilesData.Add("echo D | xcopy /Y " + "\"" + ApplciationHome + "\"" + " " + "\"" + WorkingApplicationFolder + "\"" + @" /S");
            
           // WriteBatchFile(CopyFilesData, PUB100X_CopyFilesName, LaunchFolder);


            //PUB1000_CopyFiles.bat
            //string PUB100X_CopyLibrarysName = "PUB100X_CopyLibraryFiles.bat";
            //List<string> CopyLibraryData = new List<string>();
            //CopyLibraryData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + TargetsFilePath + "\"" + " " + "\"" + workingResultsFolder + @"\" + TargetsFilePath + "\"" + @" /S");
            //WriteBatchFile(CopyLibraryData, PUB100X_CopyLibrarysName, LaunchFolder);


            //POST PROCESSING SECTION
            //PUB1002_CopyResultBack.bat
            string CopyResultBackName = "PUB100X_CopyResultBack.bat";
            List<string> CopyResultBackData = new List<string>();
            CopyResultBackData.Add("MD " + "\"" + ResultsFutureHome + "\"");
                //copy locks
            CopyResultBackData.Add("echo D | xcopy /Y " + "\"" + WorkingParametersFolder + @"\LocksFolder" + "\"" + " " + "\"" + workingResultsFolder + @"\LocksFolder" + "\"" + @" /S");
            
            CopyResultBackData.Add("echo D | xcopy /Y " + "\"" + workingResultsFolder + "\"" + " " + "\"" + ResultsFutureHome + "\"" + @" /S");
            //CopyResultBackData.Add("echo f | xcopy /Y " + "\"" + workingResultsFolder + @"\" + "XYDataResults.zip" + "\"" + " " + "\"" + ResultsFutureHome + @"\" + "XYDataResults.zip" + "\"" + @" /S");
            //CopyResultBackData.Add("Pause");
            WriteBatchFile(CopyResultBackData, CopyResultBackName, LaunchFolder);


            //DeleteXYDataFolder
            string DeleteXYDataName = "PUB100X_DeleteXYDataFolder.bat";
            List<string> XYFolderDeleteData = new List<string>();
            XYFolderDeleteData.Add(@"if exist " + "\"" + XYDataFolder + "\"" + " rmdir /s /q " + "\"" + XYDataFolder + "\"" + " /s/q");
            WriteBatchFile(XYFolderDeleteData, DeleteXYDataName, LaunchFolder);


            ////RunMeThird.bat
            //string WorkflowFileName = "PIC_RunMeThird_" + PubName + ".bat";
            //List<string> WorkflowData = new List<string>();
            ////WorkflowData.Add("Call " + PUB100X_CopyFilesName);
            ////WorkflowData.Add("Call " + PUB100X_CopyLibrarysName);
            //WorkflowData.Add("Call " + writeFolder + @"\" + PIC_RunGlyQConsoleFileName);
            //WorkflowData.Add("Exit");
            ////WorkflowData.Add("Call " + writeFolder + @"\" + PIC_PostProcessingFileName);
            ////WorkflowData.Add("Pause");
            ////WorkflowData.Add("Call " + writeFolder + @"\" + DeleteXYDataName);
            ////WorkflowData.Add("Call " + writeFolder + @"\" + CopyResultBackName);
            ////WorkflowData.Add("Call " + writeFolder + @"\" + PIC_DeleteFilesFileName);
            ////WorkflowData.Add("Pause");
            //WriteBatchFile(WorkflowData, WorkflowFileName, LaunchFolder);


          

           
           

            //PIC_RunMeSeccondFromPub100X.bat
            string RunMeSeccond100XFileName = "PIC_RunMeSeccond_PUB100X.bat";
            List<string> RunMeSeccond100XData = new List<string>();

            RunMeSeccond100XData.Add("echo D | xcopy /Y " + "\"" + ZippedPub100XSetup + "\"" + " " + "\"" + workingZippedFolder + "\"" + @" /S");
            RunMeSeccond100XData.Add("echo D | xcopy /Y " + "\"" + ApplciationHomePub100XSetup + "\"" + " " + "\"" + WorkingLaunchConsoleFolder + "\"" + @" /S");
            RunMeSeccond100XData.Add("echo D | xcopy /Y " + "\"" + DivideTargetsHomePub100XSetup + "\"" + " " + "\"" + WorkingDivideTargetsFolder + "\"" + @" /S");
            RunMeSeccond100XData.Add("echo D | xcopy /Y " + "\"" + ApplciationHome + "\"" + " " + "\"" + WorkingApplicationFolder + "\"" + @" /S");
            RunMeSeccond100XData.Add("echo D | xcopy /Y " + "\"" + PostProcessingHomePub100XSetup + "\"" + " " + "\"" + WorkingPostProcessingFolder + "\"" + @" /S");
            RunMeSeccond100XData.Add("echo D | xcopy /Y " + "\"" + SleepMultiHomePub100XSetup + "\"" + " " + "\"" + WorkingMultiSleepFolder + "\"" + @" /S");
            RunMeSeccond100XData.Add("echo D | xcopy /Y " + "\"" + CheckFileHomePub100XSetup + "\"" + " " + "\"" + WorkingCheckFileFolder + "\"" + @" /S");
            RunMeSeccond100XData.Add("echo D | xcopy /Y " + "\"" + DeleteFilesExpCopyHomePub100XSetup + "\"" + " " + "\"" + WorkingDeleteFilesFolder + "\"" + @" /S");
            RunMeSeccond100XData.Add("echo D | xcopy /Y " + "\"" + CrazyFilesCopyHomePub100XSetup + "\"" + " " + "\"" + WorkingCrazyCopyFolder + "\"" + @" /S");

            //copy application specific files first.  then run parameter setup.  then copy specific files
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + nodeShareFolderLocationOnNode + @"\" + nodeShareFolder + @"\" + ApplicationSetupParameters + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + ApplicationSetupParameters + "\"");//this is the worker version as setup by divide targets
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + nodeShareFolderLocationOnNode + @"\" + nodeShareFolder + @"\" + divideTargetsParametersName + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + divideTargetsParametersName + "\"");
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + nodeShareFolderLocationOnNode + @"\" + nodeShareFolder + @"\" + TargetsFilePath + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + TargetsFilePath + "\"");
            
            RunMeSeccond100XData.Add("\"" + Applciation100XPub100XSetupLocation + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + ApplicationSetupParameters + "\"");
            

            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + RawFolderHome + @"\" + RawFile + "\"" + " " + "\"" + workingRawDataFolder + @"\" + RawFile + "\"");
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LaunchInitializion + @"\" + ZipParameterFile + "\"" + " " + "\"" + workerLocation + @"\" + ZipParameterFile + "\"");
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LaunchInitializion + @"\" + PIC_RunGlyQConsoleFileName + "\"" + " " + "\"" + workerLocation + @"\" + PIC_RunGlyQConsoleFileName + "\"");
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LaunchInitializion + @"\" + PIC_DeleteFilesFileName + "\"" + " " + "\"" + workerLocation + @"\" + PIC_DeleteFilesFileName + "\"");
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LaunchInitializion + @"\" + PIC_PostProcessingFileName + "\"" + " " + "\"" + workerLocation + @"\" + PIC_PostProcessingFileName + "\"");
            

            //RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + TargetsFilePath + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + TargetsFilePath + "\"" + @" /S");
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + processingParameters + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + processingParameters + "\"");
            //RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + divideTargetsParameters + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + divideTargetsParameters + "\"" + @" /S");
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + specificParameters + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + specificParameters + "\"");
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + factors + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + factors + "\"");
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + randomIQFile + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + randomIQFile + "\"");
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + multiSleepParameterFile + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + multiSleepParameterFile + "\"");
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + GoCrazyCopyParameterFile + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + GoCrazyCopyParameterFile + "\"");

            //RunPostProcessing
            //RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + CopyResultBackName + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + CopyResultBackName + "\"");
            //RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + DeleteXYDataName + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + DeleteXYDataName + "\"");
            
            //RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + LibraryHome + @"\" + ApplicationSetupParametersWorker + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + ApplicationSetupParametersWorker + "\"" + @" /S");

            //"FragmentedTargetedWorkflowParameters_Velos_DH.txt" "Parameters_DivideTargetsPIC.txt"
            //we are making a file in the node folder that contains all the node information so we need to pull Parameters_DivideTargetsPIC from the node folder to the normal run folder on the node.  same for the targets file
            //we are recopying the files to get the node specific files
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + nodeShareFolderLocationOnNode + @"\" + nodeShareFolder + @"\" + divideTargetsParametersName + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + divideTargetsParametersName + "\"");
            RunMeSeccond100XData.Add("echo f | xcopy /Y " + "\"" + nodeShareFolderLocationOnNode + @"\" + nodeShareFolder + @"\" + TargetsFilePath + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + TargetsFilePath + "\"");
            

            
            RunMeSeccond100XData.Add("\"" + DivideTargets100XPub100XSetupLocation + "\"" + " " + "\"" +  WorkingParametersFolder + @"\" + divideTargetsParametersName + "\"");

            RunMeSeccond100XData.Add("Call " + workerLocation + @"\" + PIC_RunGlyQConsoleFileName);//from run me third.  launched before multisleep
            RunMeSeccond100XData.Add("\"" + SleepMultiExeLocation + "\"" + " " + "\"" + WorkingParametersFolder + @"\" + multiSleepParameterFile + "\"");//post processing is launched from here
            //RunMeSeccond100XData.Add("Pause");
            WriteBatchFile(RunMeSeccond100XData, RunMeSeccond100XFileName, LaunchFolder);

            

            //PIC_NodeSetupPrep.bat
            string PIC_NodeSetupPrepName = "PIC_NodeSetup.bat";
            List<string> PIC_NodeSetupPrepData = new List<string>();
            //the library needs to be copied from picFS to library home first
            PIC_NodeSetupPrepData.Add("\"" + DivideTargetsNodeExeLocation + "\"" + " " + "\"" + LibraryHome + @"\" + divideTargetsParametersName + "\"" + " " + "\"" + LibraryHome + @"\" + divideTargetsParametersNode + "\"");
            //PIC_NodeSetupPrepData.Add("\"" + DivideTargetsNodeExeLocation + "\"" + " " + "\"" + LibraryOnPICFS + @"\" + divideTargetsParameters + "\"" + " " + "\"" + LibraryOnPICFS + @"\" + divideTargetsParametersNode + "\"");
            
            //PIC_NodeSetupPrepData.Add("Pause");
            WriteBatchFile(PIC_NodeSetupPrepData, PIC_NodeSetupPrepName, LaunchFolder);



            string PIC_NodeConsolidationFileName = "PIC_NodeConsolidation.bat";
            List<string> NodeConsolidationData = new List<string>();
            string lineTime = "\"" + @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ Timer\Release\WriteTime.exe" + "\"" + " " + "\"" + @"E:\ScottK\Shared_PICFS\ToPIC\PubFinal_Time.txt" + "\"";
            NodeConsolidationData.Add(lineTime);
            //string line = "E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ CombineNodeResults\Release\CombineNodeResults.exe" "E:\ScottK\Shared_PICFS\ToPIC\WorkingParameters\ConsolidationParameterForAllNodes.txt"
            string line = "\"" + CombineNodeResultsExeLocation + "\"" + " " + "\"" + LibraryHome + @"\" + ConsolidationAtNodeLevelList + "\"";
            NodeConsolidationData.Add(line);
            WriteBatchFile(NodeConsolidationData, PIC_NodeConsolidationFileName, LaunchFolder);
            



            ////PIC_SendJob.bat
            //string PIC_SendJobFileName = "PIC_SendJob_" + NodeName + ".bat";
            //List<string> SendJobFileNameData = new List<string>();
            ////node name needs to be hard coded so it know where to go
            //SendJobFileNameData.Add("echo f | xcopy /Y " + "\"" + Pub1000HomeFolder + @"\" + RunMeSeccond100XFileName + "\"" + " " + "\"" + @"\\" + NodeName + @"\" + nodeShareFolder + @"\" + RunMeSeccond100XFileName + "\"" + @" /S");
            //SendJobFileNameData.Add(PexecLocation + @" \\" + ipAddressForExecution + " -u " + ipAddressForExecution + @"\" + adminAccount + " -p " + adminPassword + " " + "\"" + nodeShareFolderLocationOnNode + @"\" + nodeShareFolder + @"\" + RunMeSeccond100XFileName + "\"");
            ////sleep and wait for the job to finish on the slave computers
            //SendJobFileNameData.Add("\"" + SleepExeLocation + "\"" + " " + "\"" + Pub1000HomeFolder + @"\" + "WorkingParameters" + @"\" + sleepParameterFile + "\"");
            //SendJobFileNameData.Add("Pause");
            //WriteBatchFile(SendJobFileNameData, PIC_SendJobFileName, LaunchFolder);


            
            //PIC_RunMeFirstFromPub1000.bat
            string RunMeFirstFileName = "PIC_RunMeFirst_PUB1000.bat";
            List<string> RunMeFirstData = new List<string>();
            //RunMeFirstData.Add(@"if exist "F:\ScottK" rmdir /s /q "F:\ScottK" /s/q")
            RunMeFirstData.Add(@"if exist " + "\"" + RunMeFirstFromPub1000Setup + "\"" + " rmdir /s /q " + "\"" + RunMeFirstFromPub1000Setup + "\"" + " /s/q");
            RunMeFirstData.Add(@"if exist " + "\"" + RunMeFirstFromPub1000Application + "\"" + " rmdir /s /q " + "\"" + RunMeFirstFromPub1000Application + "\"" + " /s/q");
            RunMeFirstData.Add(@"if exist " + "\"" + RunMeFirstFromPub1000divideTargets + "\"" + " rmdir /s /q " + "\"" + RunMeFirstFromPub1000divideTargets + "\"" + " /s/q");
            RunMeFirstData.Add(@"if exist " + "\"" + RunMeFirstFromPub1000postProcessing + "\"" + " rmdir /s /q " + "\"" + RunMeFirstFromPub1000postProcessing + "\"" + " /s/q");
            RunMeFirstData.Add(@"if exist " + "\"" + RunMeFirstFromPub1000Sleep + "\"" + " rmdir /s /q " + "\"" + RunMeFirstFromPub1000Sleep + "\"" + " /s/q");
            RunMeFirstData.Add(@"if exist " + "\"" + RunMeFirstFromPub1000SleepMulti + "\"" + " rmdir /s /q " + "\"" + RunMeFirstFromPub1000SleepMulti + "\"" + " /s/q");

            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSZipped + "\"" + " " + "\"" + RunMeFirstFromPub1000Zipped + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSSetup + "\"" + " " + "\"" + RunMeFirstFromPub1000Setup + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSApplication + "\"" + " " + "\"" + RunMeFirstFromPub1000Application + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSdivideTargets + "\"" + " " + "\"" + RunMeFirstFromPub1000divideTargets + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSpostProcessing + "\"" + " " + "\"" + RunMeFirstFromPub1000postProcessing + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSSleep + "\"" + " " + "\"" + RunMeFirstFromPub1000Sleep + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSSleepMulti + "\"" + " " + "\"" + RunMeFirstFromPub1000SleepMulti + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSCheckFile + "\"" + " " + "\"" + RunMeFirstFromPub1000CheckFile + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSDeleteFilesExpCopy + "\"" + " " + "\"" + RunMeFirstFromPub1000DeleteFilesExpCopy + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSCrazyFilesCopy + "\"" + " " + "\"" + RunMeFirstFromPub1000CrazyFilesCopy + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSDivideTargetsNode + "\"" + " " + "\"" + RunMeFirstFromPub1000DivideTargetsNode + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSCombineNodeResults + "\"" + " " + "\"" + RunMeFirstFromPub1000CombineNodeResults + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSTimer + "\"" + " " + "\"" + RunMeFirstFromPub1000Timer + "\"" + @" /S");
            RunMeFirstData.Add("echo D | xcopy /Y " + "\"" + RunMeFirstFromPICFSZippedApp + "\"" + " " + "\"" + RunMeFirstFromPub1000ZippedApp + "\"" + @" /S");

           

            //RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + ApplciationHomePub100XSetupHome + @"\" + RunMeSeccond100XFileName + "\"" + " " + "\"" + LaunchInitializion + @"\" + RunMeSeccond100XFileName + "\"" + @" /S");
            //RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + ApplciationHomePub100XSetupHome + @"\" + PIC_NodeSetupPrepName + "\"" + " " + "\"" + LaunchInitializion + @"\" + PIC_NodeSetupPrepName + "\"" + @" /S");
            //RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + ApplciationHomePub100XSetupHome + @"\" + ZipBatchName + "\"" + " " + "\"" + LaunchInitializion + @"\" + ZipBatchName + "\"" + @" /S");
            //RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + ApplciationHomePub100XSetupHome + @"\" + PIC_NodeConsolidationFileName + "\"" + " " + "\"" + LaunchInitializion + @"\" + PIC_NodeConsolidationFileName + "\"" + @" /S");
            //RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + ApplciationHomePub100XSetupHome + @"\" + ZipParameterFile + "\"" + " " + "\"" + LaunchInitializion + @"\" + ZipParameterFile + "\"" + @" /S");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + ApplciationHomePub100XSetupHome + @"\" + LaunchJobsBatchFileName + "\"" + " " + "\"" + LaunchInitializion + @"\" + LaunchJobsBatchFileName + "\"");

            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + ApplciationHomePub100XSetupHome + @"\" + PIC_NodeSetupPrepName + "\"" + " " + "\"" + LaunchInitializion + @"\" + PIC_NodeSetupPrepName + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + ApplciationHomePub100XSetupHome + @"\" + testBatchName + "\"" + " " + "\"" + LaunchInitializion + @"\" + testBatchName + "\"");
            //RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + ApplciationHomePub100XSetupHome + @"\" + PIC_SendJobBackFileName + "\"" + " " + "\"" + LaunchInitializion + @"\" + PIC_SendJobBackFileName + "\"" + @" /S");
           
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + TargetsFilePath + "\"" + " " + "\"" +LibraryHome + @"\" + TargetsFilePath + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + divideTargetsParametersName + "\"" + " " + "\"" + LibraryHome + @"\" + divideTargetsParametersName + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + processingParameters + "\"" + " " + "\"" + LibraryHome + @"\" + processingParameters + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + specificParameters + "\"" + " " + "\"" + LibraryHome + @"\" + specificParameters + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + factors + "\"" + " " + "\"" + LibraryHome + @"\" + factors + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + randomIQFile + "\"" + " " + "\"" + LibraryHome + @"\" + randomIQFile + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + sleepParameterFile + "\"" + " " + "\"" + LibraryHome + @"\" + sleepParameterFile + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + multiSleepParameterFile + "\"" + " " + "\"" + LibraryHome + @"\" + multiSleepParameterFile + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + GoCrazyCopyParameterFile + "\"" + " " + "\"" + LibraryHome + @"\" + GoCrazyCopyParameterFile + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + divideTargetsParametersNode + "\"" + " " + "\"" + LibraryHome + @"\" + divideTargetsParametersNode + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + ApplicationSetupParameters + "\"" + " " + "\"" + LibraryHome + @"\" + ApplicationSetupParameters + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + ApplicationSetupParametersWorker + "\"" + " " + "\"" + LibraryHome + @"\" + ApplicationSetupParametersWorker + "\"");

            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + globalMultiSleepFile + "\"" + " " + "\"" + LibraryHome + @"\" + globalMultiSleepFile + "\"");
            RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + globalMultiSleepFileListFile + "\"" + " " + "\"" + LibraryHome + @"\" + globalMultiSleepFileListFile + "\"");
            //RunMeFirstData.Add("echo f | xcopy /Y " + "\"" + LibraryOnPICFS + @"\" + ZipParameterFile + "\"" + " " + "\"" + LibraryHome + @"\" + ZipParameterFile + "\"" + @" /S");

            //run the node divider
            //RunMeFirstData.Add("Call " + @"E:\ScottK\Shared_PICFS\ToPIC\PIC_NodeSetup.bat");
            //RunMeFirstData.Add("\"" + @"E:\ScottK\Shared_PICFS\ToPIC\GlyQ-IQ Application Setup\Release\IQGlyQ_Console_ParameterSetup.exe" + "\"" + " " + "\"" + LibraryHome + @"\" + ApplicationSetupParameters + "\"");

            //RunMeFirstData.Add("Call " + @"E:\ScottK\Shared_PICFS\ToPIC\Zip.bat");
            // CopyFiles needed raw file to pub1000
            //RunMeFirst.Add("echo f | xcopy /Y " + "\"" + RawFolderHomePICFS + @"\" + RawFile + "\"" + " " + "\"" + RawFolderHome + @"\" + RawFile + "\"" + @" /S");
            WriteBatchFile(RunMeFirstData, RunMeFirstFileName, LaunchFolder);



          





            ////write sleep file
            //string sleepFileName = "PIC_SleepParameterFile_" + NodeName + ".txt";
            //string sleepFilePath = WorkingParametersFolder + @"\" + sleepFileName;
            //string fileTOwaitFor = ThisIsWhatItTakesToFindFinalResultsFile(LibraryHome, divideTargetsParameters);
            //string lineFileToWaitFor = "FileToWaitFor," + Pub1000HomeFolder + @"\" + "Results" + @"\" + NodeName + "_Results" + @"\" + fileTOwaitFor;
            //string lineBatchFileToRunAfterLoop = "BatchFileToRunAfterLoop," + Pub1000HomeFolder + @"\" + "testBatch.bat";//we need line 1 from line 0 in the controller
            //string lineWorkingFolder = "WorkingFolder," + Pub1000HomeFolder + @"\" + "WorkingParameters";;
            //string lineSeconds = "Seconds," + 20;

            //List<string> linesForSleepParameteFile = new List<string>();
            //linesForSleepParameteFile.Add(lineFileToWaitFor);
            //linesForSleepParameteFile.Add(lineBatchFileToRunAfterLoop);
            //linesForSleepParameteFile.Add(lineWorkingFolder);
            //linesForSleepParameteFile.Add(lineSeconds);

            //writer.toDiskStringList(sleepFilePath, linesForSleepParameteFile);

            //write parameters launch files from node file
           
      //      string nodeFilePath = @"F:\ScottK\ToPIC\WorkingParameters" + @"\" + sleepFileName;

            //string lineFileToWaitFor = "FileToWaitFor," + Pub1000HomeFolder + @"\" + "Results" + @"\" + NodeName + "_Results" + @"\" + fileTOwaitFor;
            

        }

        private static string ThisIsWhatItTakesToFindFinalResultsFile(string LibraryHome, string divideTargetsParameters)
        {
            StringLoadTextFileLine reader2 = new StringLoadTextFileLine();

            List<string> divideTargetsParametersLines = reader2.SingleFileByLine(LibraryHome + @"\" + divideTargetsParameters);
            string fileTOwaitForLine = divideTargetsParametersLines[2];

            List<string> words = fileTOwaitForLine.Split(',').ToList();
            string fileTOwaitFor = words[1] + "_iqResults.txt";



            return fileTOwaitFor;
        }

        #region private methods

        private static void WriteBatchFile(List<string> stringList, string fileName, string folder)
        {
            StringListToDisk writer = new StringListToDisk();
            string outputLocation;
            outputLocation = folder + @"\" + fileName;
            writer.toDiskStringList(outputLocation, stringList);
        }

        private static List<string> SetBasicTargetedWorkflowParameters()
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

        private static List<string> SetBasicGlyQIQParameters(string TargetsFilePath, string WorkingParametersFolder, string workingRawDataFolder, string workingResultsFolder)
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

        private static List<string> SetTargetedAlignmentWorkflowParameters()
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

       
        #endregion

    }

}
