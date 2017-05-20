using NUnit.Framework;
using Sipper.Model;
using Sipper.ViewModel;

namespace Sipper.UnitTesting.ViewModelTests
{
    [TestFixture]
    public class ManualViewingWithNoRawDataTests
    {
        [Test]
        public void Test1()
        {
            FileInputsInfo fileInputs = new FileInputsInfo();
            fileInputs.ResultImagesFolderPath = @"D:\Data\Temp\Results\Visuals";
            fileInputs.TargetsFilePath =@"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\SIPPER_standard_testing\Yellow_C13_070_23Mar10_Griffin_10-01-28_testing_results.txt";



            ManualViewingWithoutRawDataViewModel viewModel = new ManualViewingWithoutRawDataViewModel(fileInputs);

        
            viewModel.LoadResults(viewModel.FileInputs.TargetsFilePath);


        }

    }
}
