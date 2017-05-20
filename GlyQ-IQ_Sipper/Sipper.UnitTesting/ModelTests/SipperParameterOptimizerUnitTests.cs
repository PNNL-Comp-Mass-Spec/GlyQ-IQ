using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Sipper.Model;

namespace Sipper.UnitTesting.ModelTests
{
    [TestFixture]
    public class SipperParameterOptimizerUnitTests
    {
        [Test]
        public void Test1()
        {
            string unlabeledResultsFilePath= @"D:\Data\Sipper\Yellow_C13_2009Study\Results_2013_04_09\Yellow_C12_075_19Mar10_Griffin_10-01-13_results.txt";
            string labeledResultsFilePath = @"D:\Data\Sipper\Yellow_C13_2009Study\Results_2013_04_09\Yellow_C13_070_23Mar10_Griffin_10-01-28_results.txt";


            SipperFilterOptimizer filterOptimizer = new SipperFilterOptimizer(unlabeledResultsFilePath, labeledResultsFilePath);
            

            string outputFileName = @"D:\Data\Sipper\Yellow_C13_2009Study\SipperFilterOptimizationOutput.txt";
            var results = filterOptimizer.DoFilterOptimization(outputFileName);



        }

    }
}
