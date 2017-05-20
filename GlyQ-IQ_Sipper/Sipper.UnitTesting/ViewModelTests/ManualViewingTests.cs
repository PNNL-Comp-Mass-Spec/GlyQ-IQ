using System;
using System.Linq;
using System.Threading;
using DeconTools.Backend.Core.Results;
using DeconTools.UnitTesting2;
using DeconTools.Workflows.Backend.Core;
using NUnit.Framework;
using Sipper.Model;
using Sipper.ViewModel;

namespace Sipper.UnitTesting.ViewModelTests
{
    [TestFixture]
    public class ManualViewingTests
    {
        [Test]
        public void loadResultsTest()
        {
            string testParameterFile =
                @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\SIPPER_standard_testing\SipperTargetedWorkflowParameters1.xml";


            string testResultFile =
                @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\SIPPER_standard_testing\Yellow_C13_070_23Mar10_Griffin_10-01-28_testing_results.txt";

            


            FileInputsInfo fileInputs = new FileInputsInfo();
            fileInputs.ParameterFilePath = testParameterFile;
            fileInputs.TargetsFilePath = testResultFile;

            ViewAndAnnotateViewModel viewModel = new ViewAndAnnotateViewModel(fileInputs);


            viewModel.LoadResults(testResultFile);
            Assert.IsNotEmpty(viewModel.Results);
        }



        [Test]
        public void loadParametersTest1()
        {
            string testParameterFile =
                @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\SIPPER_standard_testing\SipperTargetedWorkflowParameters1.xml";

            FileInputsInfo fileInputs = new FileInputsInfo();
            fileInputs.ParameterFilePath = testParameterFile;

            ViewAndAnnotateViewModel viewModel = new ViewAndAnnotateViewModel(fileInputs);

            Assert.IsNotNull(viewModel.Workflow.WorkflowParameters);
            Assert.AreEqual(2,((TargetedWorkflowParameters)(viewModel.Workflow.WorkflowParameters)).MSPeakDetectorPeakBR);

        }


        [Test]
        public void executeWorkflowTest1()
        {
            string testDatafile =
                @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\SIPPER_standard_testing\Yellow_C13_070_23Mar10_Griffin_10-01-28.raw";

            string testResultFile =
                @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\SIPPER_standard_testing\Results\Yellow_C13_070_23Mar10_Griffin_10-01-28_temp_results.txt";

            string testParameterFile =
                @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\SIPPER_standard_testing\SipperTargetedWorkflowParameters1.xml";

            ViewAndAnnotateViewModel viewModel = new ViewAndAnnotateViewModel();

            viewModel.FileInputs.ParameterFilePath = testParameterFile;
            viewModel.FileInputs.TargetsFilePath = testResultFile;
            viewModel.FileInputs.DatasetPath = testDatafile;

            viewModel.LoadRun(testDatafile);

            //need to do this to ensure that _peaks file loads completely. Or else, no chromData error occurs
            Thread.Sleep(5000);

            viewModel.LoadResults(testResultFile);
            viewModel.CurrentResult = viewModel.Results.First(p => p.TargetID == 5555);
            
            viewModel.ExecuteWorkflow();

            Assert.IsNotNull(viewModel.Workflow.MassSpectrumXYData);
            Assert.IsNotNull(viewModel.Workflow.ChromatogramXYData);

            Assert.AreEqual(3728, viewModel.CurrentLcScan);
            Console.WriteLine("CurrentScanSet= " + viewModel.CurrentLcScan);

            var result =(SipperLcmsTargetedResult)viewModel.Workflow.Result;
            Assert.IsTrue(result.ChromCorrelationMedian > 0.9);

            viewModel.NavigateToNextMs1MassSpectrum();
            Assert.AreEqual(3739, viewModel.CurrentLcScan);
            Console.WriteLine("After manual navigating... CurrentScanSet= " + viewModel.CurrentLcScan);

            viewModel.NavigateToNextMs1MassSpectrum();
            Assert.AreEqual(3750, viewModel.CurrentLcScan);
            Console.WriteLine("After manual navigating... CurrentScanSet= " + viewModel.CurrentLcScan);

        }

    }
}
