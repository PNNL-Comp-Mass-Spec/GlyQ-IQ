using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.ProcessingTasks.MSGenerators;
using DeconTools.Backend.ProcessingTasks;
using DeconTools.Backend.Core;
using DeconTools.Backend.Runs;
using DeconTools.Backend.ProcessingTasks.PeakDetectors;
using NUnit.Framework;

namespace IQGlyQ.UnitTesting
{
    
    class Joe_Unit_Test
    {
        [Test]
        public void Test1()
        {
            //file name
            //static string testFile = @"D:\Data\From_Aaron\H37Rv_Ser3_B_15Aug11_Earth_11-04-21.raw";
        
            string testFile = @"\\protoapps\UserData\Slysz\DeconTools_TestFiles" +
                              "\\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";
                //make run
            
            Run run = new RunFactory().CreateRun(testFile);

                //make scan set
            
            ScanSet scan = new ScanSet(1000);

                //get mass spectra
        
            
            MSGenerator _msGenerator = MSGeneratorFactory.CreateMSGenerator(run.MSFileType);
            
            DeconTools.Backend.XYData msXydata = _msGenerator.GenerateMS(run, scan);
                //peak detection
            
            DeconTools.Backend.ProcessingTasks.PeakDetectors.DeconToolsPeakDetectorV2 peakDetector =
                new DeconToolsPeakDetectorV2();

            List<Peak> peaklist = peakDetector.FindPeaks(msXydata);
            //output

            //TestUtilities.DisplayPeaks(peaklist);
        }
    }
}
