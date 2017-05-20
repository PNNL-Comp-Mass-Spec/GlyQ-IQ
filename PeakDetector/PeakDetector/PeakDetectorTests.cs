using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GetPeaks_DLL;
using DeconTools.Backend.Runs;
using DeconTools.Backend.Core;
using NUnit.Framework;
using PNNLOmics.Algorithms.PeakDetector;
using PNNLOmics.Data;

namespace PeakDetector
{
    public class PeakDetectorTest
    {
        [Test]
        public void tests()
        {
            //const int STARTSCAN = 5500;
            //const int ENDSCAN = 6500;
            const int STARTSCAN = 0;
            const int ENDSCAN = 120;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            #region Setup Parameters for complete data set to find eluting peaks
            SimpleWorkflowParameters parameters = new SimpleWorkflowParameters();

            int high = 0;//0=205, 1=13229
            if (high == 0)
            {
                int factor = 1;
                parameters.MSPeakDetectorPeakBR = 1.3;//default
                parameters.MSPeakDetectorSigNoise = 2;//default
                parameters.MSPeakDetectorPeakBR = factor * 1.3;//default
                parameters.MSPeakDetectorSigNoise = factor * 3;//default
            }
            else
            {
                parameters.MSPeakDetectorPeakBR = 1.3 * 30;
                parameters.MSPeakDetectorSigNoise = 2 * 30;
            }

            parameters.StartScan = STARTSCAN;
            parameters.StopScan = ENDSCAN;

            parameters.DynamicRangeToOne = 300000;

            parameters.MaxScanSpread = 100000;

            parameters.MaxHeightForNewPeak = 0.75;

            parameters.Multithread = false;

            //parameters.PeakFitType = DeconTools.Backend.Globals.PeakFitType.LORENTZIAN;
            parameters.PeakFitType = DeconTools.Backend.Globals.PeakFitType.QUADRATIC;

            parameters.DeconvolutionType = DeconvolutionType.Rapid;

            #endregion

            #region Setup Run

            //string inputDataFilename = @"d:\Csharp\YAFMS\20100722_glycan_SN24_NF.yafms";
            //string inputDataFilename = @"g:\PNNL Files\Csharp\YAFMS\20100722_glycan_SN24_NF.yafms";

            //string inputDataFilename = @"d:\Csharp\YAFMS\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.yafms";
            string inputDataFilename = @"d:\Csharp\Syn Output\14 scan test file for get peaks\yafmsOut.yafms";

            //string inputDataFilename = @"D:\PNNL Data\Gordon\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.RAW";
            //string inputDataFilename = @"g:\PNNL Files\Csharp\YAFMS\QC_Shew_08_04-pt5-2_11Jan09_Sphinx_08-11-18.yafms";

            RunFactory rf = new RunFactory();
            Run run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, inputDataFilename);
            //Run run = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.Finnigan, inputDataFilename);
            #endregion

            #region Run Program

            PeakFinderController controller = new PeakFinderController(run, parameters);
            
            List<CentroidedPeak> fromOmicsPeakDetector = new List<CentroidedPeak>();
            List<PNNLOmics.Data.XYData> fromDeconPeakList = new List<PNNLOmics.Data.XYData>();

            controller.SimpleWorkflowExecutePart1(run, ref fromDeconPeakList, ref fromOmicsPeakDetector);
            #endregion

            #region run tests to cound number of peaks found
            stopWatch.Stop();
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to find and assign features in eluting peaks");

            //the numbers are different because the thresholding is based on different characteristics
            Assert.AreEqual(fromDeconPeakList.Count, 11869);
            Assert.AreEqual(fromOmicsPeakDetector.Count, 114);
            #endregion
        }
    }
}
