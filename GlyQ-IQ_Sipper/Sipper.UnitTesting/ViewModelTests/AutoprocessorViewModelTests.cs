using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using DeconTools.Backend.Utilities;
using DeconTools.Workflows.Backend.FileIO;
using NUnit.Framework;
using Sipper.ViewModel;

namespace Sipper.UnitTesting.ViewModelTests
{
    [TestFixture]
    public class AutoprocessorViewModelTests
    {
        [Test]
        public void createFileLinkageSingleXMLFile_Test1()
        {
            AutoprocessorViewModel viewModel = new AutoprocessorViewModel();

            string testWorkflowFile = FileRefs.TestFilePath + "\\" + "SipperTargetedWorkflowParameters1.XML";
            viewModel.FileInputs.CreateFileLinkage(testWorkflowFile);

            Assert.IsTrue(viewModel.FileInputs.ParameterFilePath == testWorkflowFile);
        }

        [Test]
        public void createFileLinkageSingleTextFile_Test1()
        {
            AutoprocessorViewModel viewModel = new AutoprocessorViewModel();

            string testTargetFile1 = FileRefs.TestFilePath + "\\" + "Yellow_C13_070_23Mar10_Griffin_10-01-28_LCMSFeatures.txt";
            viewModel.FileInputs.CreateFileLinkage(testTargetFile1);

            Assert.IsTrue(viewModel.FileInputs.TargetsFilePath == testTargetFile1);
        }


        [Test]
        public void createFileLinkageMultiple_XMLFile_Test1()
        {
            AutoprocessorViewModel viewModel = new AutoprocessorViewModel();

            string testWorkflowFile = FileRefs.TestFilePath + "\\" + "SipperTargetedWorkflowParameters1.XML";
            string testWorkflowFile2 = FileRefs.TestFilePath + "\\" + "SipperTargetedWorkflowParameters1.xml";

            string testTargetFile1 = FileRefs.TestFilePath + "\\" + "Yellow_C13_070_23Mar10_Griffin_10-01-28_LCMSFeatures.txt";

            List<string> inputFiles = new List<string>();
            inputFiles.Add(testWorkflowFile);
            inputFiles.Add(testWorkflowFile2);
            inputFiles.Add(testTargetFile1);


            viewModel.FileInputs.CreateFileLinkages(inputFiles);

            Assert.IsTrue(viewModel.FileInputs.ParameterFilePath == testWorkflowFile);
            Assert.IsTrue(viewModel.FileInputs.TargetsFilePath==testTargetFile1);
        }

        [Test]
        public void create_Run__Test1()
        {
            AutoprocessorViewModel viewModel = new AutoprocessorViewModel();

            string testFile =
                @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\SIPPER_standard_testing\Yellow_C13_070_23Mar10_Griffin_10-01-28.raw";
            viewModel.FileInputs.CreateFileLinkage(testFile);

            Assert.IsTrue(viewModel.FileInputs.DatasetPath == testFile);
        }

        [Test]
        public void process_test1()
        {
            AutoprocessorViewModel viewModel=new AutoprocessorViewModel();

            string testRawDataFile =
                @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\SIPPER_standard_testing\Yellow_C13_070_23Mar10_Griffin_10-01-28.raw";


            string expectedResultsFile = Path.GetDirectoryName(testRawDataFile) + "\\Results\\" + 
                                         RunUtilities.GetDatasetName(testRawDataFile) + "_results.txt";

            if (File.Exists(expectedResultsFile)) File.Delete(expectedResultsFile);



            string testWorkflowFile =
                @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\SIPPER_standard_testing\SipperTargetedWorkflowParameters1.xml";
            string testTargetFile1 =
                @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\SIPPER_standard_testing\Yellow_C13_070_23Mar10_Griffin_10-01-28_testing_results.txt";

            viewModel.FileInputs.CreateFileLinkage(testRawDataFile);
            viewModel.FileInputs.CreateFileLinkage(testTargetFile1);
            viewModel.FileInputs.CreateFileLinkage(testWorkflowFile);


            viewModel.Execute();

            Thread.Sleep(12000);   // the viewModel has a background worker and we need to pause to let the processing complete in the background


            Assert.That(File.Exists(expectedResultsFile));
            SipperResultFromTextImporter importer = new SipperResultFromTextImporter(expectedResultsFile);
            var resultRepo = importer.Import();

            Assert.AreEqual(19, resultRepo.Results.Count);

            var testResult = (DeconTools.Workflows.Backend.Results.SipperLcmsFeatureTargetedResultDTO)resultRepo.Results[1];
            Assert.AreEqual("Yellow_C13_070_23Mar10_Griffin_10-01-28", testResult.DatasetName);
            Assert.AreEqual(7585, testResult.TargetID);
            Assert.AreEqual("C63H109N17O21", testResult.EmpiricalFormula);
            Assert.AreEqual(11805, testResult.ScanLC);
        }

        [Test]
        public void process_unidentifiedFeatures_test1()
        {
            AutoprocessorViewModel viewModel = new AutoprocessorViewModel();

            string testRawDataFile =
                @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\SIPPER_standard_testing\Yellow_C13_070_23Mar10_Griffin_10-01-28.raw";


            string expectedResultsFile = Path.GetDirectoryName(testRawDataFile) + "\\" +
                                         RunUtilities.GetDatasetName(testRawDataFile) + "_results.txt";

            if (File.Exists(expectedResultsFile)) File.Delete(expectedResultsFile);

            string testWorkflowFile =
                @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\SIPPER_standard_testing\SipperTargetedWorkflowParameters1.xml";
            string testTargetFile1 =
                @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\SIPPER_standard_testing\Yellow_C13_070_23Mar10_Griffin_10-01-28_first20_unidentified_LCMSFeatures.txt";

            viewModel.FileInputs.CreateFileLinkage(testRawDataFile);
            viewModel.FileInputs.CreateFileLinkage(testTargetFile1);
            viewModel.FileInputs.CreateFileLinkage(testWorkflowFile);
            viewModel.Execute();

            Thread.Sleep(11000);   // the viewModel has a background worker and we need to pause to let the processing complete in the background

            Assert.That(File.Exists(expectedResultsFile));
            SipperResultFromTextImporter importer = new SipperResultFromTextImporter(expectedResultsFile);
            var resultRepo = importer.Import();

            Assert.AreEqual(20, resultRepo.Results.Count);

            var testResult = (DeconTools.Workflows.Backend.Results.SipperLcmsFeatureTargetedResultDTO)resultRepo.Results[1];
            Assert.AreEqual("Yellow_C13_070_23Mar10_Griffin_10-01-28", testResult.DatasetName);
            Assert.AreEqual(10091, testResult.TargetID);
            Assert.AreEqual(6185, testResult.ScanLC);
            Assert.AreEqual("C74H117N20O22S", testResult.EmpiricalFormula);
            

        }


        [Test]
        public void process_unidentifiedFeatures_test2()
        {
            AutoprocessorViewModel viewModel = new AutoprocessorViewModel();

            string testRawDataFile =
                @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\SIPPER_standard_testing\Yellow_C13_070_23Mar10_Griffin_10-01-28.raw";


            string expectedResultsFile = Path.GetDirectoryName(testRawDataFile) + "\\" +
                                         RunUtilities.GetDatasetName(testRawDataFile) + "_results.txt";

            if (File.Exists(expectedResultsFile)) File.Delete(expectedResultsFile);

            string testWorkflowFile =
                @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\SIPPER_standard_testing\SipperTargetedWorkflowParameters1.xml";
            string testTargetFile1 =
                @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\SIPPER_standard_testing\Yellow_C13_070_23Mar10_Griffin_10-01-28_20_UNIDENTIFIED_results.txt";

            viewModel.FileInputs.CreateFileLinkage(testRawDataFile);
            viewModel.FileInputs.CreateFileLinkage(testTargetFile1);
            viewModel.FileInputs.CreateFileLinkage(testWorkflowFile);
            viewModel.Execute();

            Thread.Sleep(11000);   // the viewModel has a background worker and we need to pause to let the processing complete in the background

            Assert.That(File.Exists(expectedResultsFile));
            SipperResultFromTextImporter importer = new SipperResultFromTextImporter(expectedResultsFile);
            var resultRepo = importer.Import();

            Assert.AreEqual(20, resultRepo.Results.Count);

            var testResult = (DeconTools.Workflows.Backend.Results.SipperLcmsFeatureTargetedResultDTO)resultRepo.Results[1];
            Assert.AreEqual("Yellow_C13_070_23Mar10_Griffin_10-01-28", testResult.DatasetName);
            Assert.AreEqual(10091, testResult.TargetID);
            Assert.AreEqual(6185, testResult.ScanLC);
            Assert.AreEqual("C74H117N20O22S", testResult.EmpiricalFormula);


        }

    }
}
