using System;
using System.Collections.Generic;
using DeconTools.Backend;
using DeconTools.Backend.Core;
using GetPeaks_DLL.TandemSupport;
using IQGlyQ.Objects;
using PNNLOmics.Data;
using GetPeaks_DLL.Objects;
using DeconTools.Backend.Runs;

namespace HammerPeakDetectorConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Started Peak Detector");
            
            //step 1:  load data here (1 spectra at a time (sum data together)
            bool overrideArgs = false;
            if (overrideArgs)
            {
                args[0] = @"D:\Csharp\0_TestDataFiles\HammerPeakDetectorParameters.txt";
                args[1] = "";
                args[2] = "raw";
            }

            PeakDetectionParameters peakParameters = new PeakDetectionParameters();
            
            peakParameters.LoadParameters(args[0]);

            peakParameters.DataFileName = args[1];
            peakParameters.DataFileExtension = args[2];
            peakParameters.UpdatePaths();

            //int mode = 1;
            //cluster and threshold. thresholde by 0 or some sigma above moving average.  we keep all peak above this moving threshold including peaks that are not in clusters
            //int mode = 2; //cluster only with optimize with 1 iteration.  
            //int mode = 3; //cluster and threshold with optimize with 1 iteration. we keep all peak above this moving threshold including peaks that are not in clusters
            //int mode = 4; //cluster optimize with many iterations. 

            bool overrideParameters = false;
            if (overrideParameters)
            {
                peakParameters.isHammer = true;

                peakParameters.ScansToSum = 5;
                peakParameters.PeakToBackgroundRatio = 1.3;
                peakParameters.SignalToNoiseThreshold = 3;
                peakParameters.HammerMode = 1;
            }

            //step 2 figure out data set information. i.e scan numbers etc.   setup run and scanset

            RunFactory factor = new RunFactory();
            Run runIn = factor.CreateRun(Globals.MSFileType.Finnigan, peakParameters.DataFileNameWithPath);

            string outputFolder = peakParameters.OutputDirectory;
            string outputPeaksFileName = peakParameters.OutputPeaksFileNameWithPath;
            bool writeMetrics = false;

            ScanObject scanInfo = new ScanObject(0, 0);
            scanInfo.Max = runIn.MaxLCScan;
            scanInfo.Min = runIn.MinLCScan;
            scanInfo.ScansToSum = peakParameters.ScansToSum;//5 is default, 15 for spin.  this number effects the EIC in GlyQIQ
            scanInfo.Buffer = 9 * 2;

            runIn.ScanSetCollection.Create(runIn, scanInfo.Min, scanInfo.Max, scanInfo.ScansToSum, 1, false);

            Console.WriteLine("There " + runIn.PrimaryLcScanNumbers.Count + " MS1 scans");

            if (peakParameters.isHammer)
            {
                Console.WriteLine("Using Hammer Peak Detector Mode " + peakParameters.HammerMode + Environment.NewLine);
                PeakDetectHammer.DetectPeaksWithHammer(peakParameters.DataFileName, outputFolder, outputPeaksFileName, runIn, writeMetrics, peakParameters.HammerMode);
            }
            else
            {
                Console.WriteLine("Using Decon Peak Detector" +  Environment.NewLine);
                PeakDetectDeconTools.DetectPeaksWithDecontools(scanInfo, runIn, peakParameters);
            }

            Console.WriteLine("Done");
            //Console.ReadKey();
            //write to YAFMS-DB
        }

        

        private static List<int> ScanLevelsWithTandemCalculator(InputOutputFileName newFile)
        {
            int sizeOfDatabase; //get number of scans in total
            int limitFileToThisManyScans = 50; //int limitFileToThisManyScans = 30000;
            List<PrecursorInfo> precursors; //MS1 and MS2 connectivity information
            GatherDatasetInfo.GetMSLevelandSize(newFile, limitFileToThisManyScans, out sizeOfDatabase, out precursors);

            bool areTandemDetected = false;
            foreach (PrecursorInfo scan in precursors)
            {
                if (scan.MSLevel > 1)
                {
                    areTandemDetected = true;
                    break;
                }
            }

            List<int> scanMSLevelList = new List<int>();
            List<int> scanLevelsWithTandem = new List<int>();
            if (areTandemDetected)
            {
                foreach (PrecursorInfo parent in precursors)
                {
                    scanMSLevelList.Add(parent.MSLevel);
                }
                scanLevelsWithTandem = SelectScans.Ms1PrecursorScansWithTandem(scanMSLevelList);
            }
            else
            {
                scanLevelsWithTandem = new List<int>();
            }
            return scanLevelsWithTandem;
        }
    }
}
