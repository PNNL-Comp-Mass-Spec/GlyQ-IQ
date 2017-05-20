﻿using NUnit.Framework;
using Sipper;
using Sipper.Model;

namespace GlyQIQViewer.UnitTesting.ModelTests
{
    public class HtmlGeneratorTests
    {

        [Test]
        public void generateHtmlReportTest1()
        {

            FileInputsInfo fileInputs = new FileInputsInfo();
            fileInputs.ResultImagesFolderPath = @"D:\Data\Temp\Results\Visuals";

            string resultFile =  @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\SIPPER_standard_testing\Yellow_C13_070_23Mar10_Griffin_10-01-28_testing_results.txt";

            SipperResultFromTextImporter importer = new SipperResultFromTextImporter(resultFile);
                var repo = importer.Import();

           


            fileInputs.TargetsFilePath =
                @"\\protoapps\UserData\Slysz\Standard_Testing\Targeted_FeatureFinding\SIPPER_standard_testing\Yellow_C13_070_23Mar10_Griffin_10-01-28_testing_results.txt";

            HTMLReportGenerator reportGenerator = new HTMLReportGenerator(repo, fileInputs);
            reportGenerator.GenerateHTMLReport();
        }
    }
}
