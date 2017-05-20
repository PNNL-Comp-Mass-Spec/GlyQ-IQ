using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using DeconTools.Backend.Runs;
using DeconTools.Backend.ProcessingTasks.MSGenerators;
using DeconTools.Backend.ProcessingTasks;
using NUnit.Framework;

namespace GetPeaks.UnitTests
{
    class DeconToolsUnitTest
    {

        [Test]
        public void OldDeconvolutorTest_temp1()
        {
            
            
            //Run run = new XCaliburRun(@"D:\PNNL Files\DataCopy\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW");

            //OldDecon2LSParameters parameters = new OldDecon2LSParameters();
            ////string paramFile = @"\\protoapps\UserData\Slysz\DeconTools_TestFiles\ParameterFiles\LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_MaxFit1.xml";
            //string paramFile = @"K:\Data\LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_MaxFit1.xml";
            //parameters.Load(paramFile);

            //ScanSet scanSet = new ScanSetFactory().CreateScanSet(run, 5509, 1);

            //MSGenerator msgen = MSGeneratorFactory.CreateMSGenerator(run.MSFileType);
            //DeconToolsPeakDetector peakDetector = new DeconToolsPeakDetector(1.3, 2, DeconTools.Backend.Globals.PeakFitType.QUADRATIC, true);

            //var deconvolutor = new HornDeconvolutor(parameters.HornTransformParameters);
            //run.CurrentScanSet = scanSet;
            //msgen.Execute(run.ResultCollection);
            //peakDetector.Execute(run.ResultCollection);
            //deconvolutor.Execute(run.ResultCollection);

            ////run.ResultCollection.ResultList = run.ResultCollection.ResultList.OrderByDescending(p => p.IsotopicProfile.IntensityAggregate).ToList();
            //run.ResultCollection.ResultList = run.ResultCollection.ResultList.OrderByDescending(p => p.IsotopicProfile.IntensityAggregateAdjusted).ToList();

            //Assert.AreEqual(run.ResultCollection.ResultList.Count,188);



            

            //Run run2 = new XCaliburRun(@"D:\PNNL Files\DataCopy\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW");

            ////OldDecon2LSParameters parameters = new OldDecon2LSParameters();
            ////string paramFile = @"\\protoapps\UserData\Slysz\DeconTools_TestFiles\ParameterFiles\LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_MaxFit1.xml";
            //// string paramFile = @"K:\Data\LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_MaxFit1.xml";
            ////parameters.Load(paramFile);

            //ScanSet scanSet2 = new ScanSetFactory().CreateScanSet(run2, 5509, 1);

            //MSGenerator msgen2 = MSGeneratorFactory.CreateMSGenerator(run2.MSFileType);
            //DeconToolsPeakDetector peakDetector2 = new DeconToolsPeakDetector(0.33, 2, DeconTools.Backend.Globals.PeakFitType.QUADRATIC, true);

            //var deconvolutor2 = new HornDeconvolutor(parameters.HornTransformParameters);
            //run2.CurrentScanSet = scanSet;
            //msgen2.Execute(run2.ResultCollection);
            //peakDetector2.Execute(run2.ResultCollection);
            //deconvolutor2.Execute(run2.ResultCollection);

            //run2.ResultCollection.ResultList = run2.ResultCollection.ResultList.OrderByDescending(p => p.IsotopicProfile.IntensityAggregateAdjusted).ToList();
            ////run2.ResultCollection.ResultList = run2.ResultCollection.ResultList.OrderByDescending(p => p.IsotopicProfile.IntensityAggregate).ToList();

            //Assert.AreEqual(run2.ResultCollection.ResultList.Count, 360);

            //Run run3 = new XCaliburRun(@"D:\PNNL Files\DataCopy\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW");

            ////OldDecon2LSParameters parameters = new OldDecon2LSParameters();
            ////string paramFile = @"\\protoapps\UserData\Slysz\DeconTools_TestFiles\ParameterFiles\LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_MaxFit1.xml";
            //// string paramFile = @"K:\Data\LTQ_Orb_SN2_PeakBR1pt3_PeptideBR1_Thrash_MaxFit1.xml";
            ////parameters.Load(paramFile);

            //ScanSet scanSet3 = new ScanSetFactory().CreateScanSet(run3, 5509, 1);

            //MSGenerator msgen3 = MSGeneratorFactory.CreateMSGenerator(run3.MSFileType);
            //DeconToolsPeakDetector peakDetector3 = new DeconToolsPeakDetector(1.3, 2, DeconTools.Backend.Globals.PeakFitType.QUADRATIC, true);//1.3 AND 2 IS DEFAULT

            //parameters.HornTransformParameters.PeptideMinBackgroundRatio = 1;//1 IS DEFAULT
            //parameters.HornTransformParameters.MaxFit = 0.1;
            
            //var deconvolutor3 = new HornDeconvolutor(parameters.HornTransformParameters);
            //run3.CurrentScanSet = scanSet;
            //msgen3.Execute(run3.ResultCollection);
            //peakDetector3.Execute(run3.ResultCollection);
            //deconvolutor3.Execute(run3.ResultCollection);


            //run3.ResultCollection.ResultList = run3.ResultCollection.ResultList.OrderByDescending(p => p.IsotopicProfile.IntensityAggregateAdjusted).ToList();
            ////run3.ResultCollection.ResultList = run3.ResultCollection.ResultList.OrderByDescending(p => p.IsotopicProfile.IntensityAggregate).ToList();

            //Assert.AreEqual(run3.ResultCollection.ResultList.Count, 125);
            ////TestUtilities.DisplayMSFeatures(run.ResultCollection.ResultList);

            ////IsosResult testIso = run.ResultCollection.ResultList[0];

            ////TestUtilities.DisplayIsotopicProfileData(testIso.IsotopicProfile);

            ////TestUtilities.DisplayMSFeatures(run.ResultCollection.ResultList);
            ////TestUtilities.DisplayPeaks(run.PeakList);



        }


    }
}
