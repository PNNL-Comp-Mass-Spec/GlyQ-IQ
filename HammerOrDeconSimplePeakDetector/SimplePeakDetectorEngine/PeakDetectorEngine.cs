using System;
using System.Collections.Generic;
using System.IO;
using GetPeaks_DLL.TandemSupport;
using IQGlyQ.Objects;
using PNNLOmics.Data;


namespace HammerPeakDetectorConsole
{
    public static class PeakDetectorEngine
    {
        public static void Execute(string dataFilePathWithName, string outputFolderPath, string parameterFilePath)
        {
            Console.WriteLine("Started Peak Detector");
            
            //step 1:  load data here (1 spectra at a time (sum data together)
           
            var peakParameters = new PeakDetectionParameters();

            peakParameters.LoadParameters(parameterFilePath);

	        var fiInputFile = new FileInfo(dataFilePathWithName);	        
	        var fiOutputFolder = new DirectoryInfo(outputFolderPath);

			peakParameters.WorkingDirectory = fiInputFile.Directory.FullName;
			peakParameters.OutputDirectory = fiOutputFolder.FullName;

            peakParameters.DataFileName = Path.GetFileNameWithoutExtension(dataFilePathWithName);
            peakParameters.DataFileExtension = Path.GetExtension(dataFilePathWithName);
            peakParameters.UpdatePaths();

            //int mode = 1;
            //cluster and threshold. thresholde by 0 or some sigma above moving average.  we keep all peak above this moving threshold including peaks that are not in clusters
            //int mode = 2; //cluster only with optimize with 1 iteration.  
            //int mode = 3; //cluster and threshold with optimize with 1 iteration. we keep all peak above this moving threshold including peaks that are not in clusters
            //int mode = 4; //cluster optimize with many iterations. 

            const bool overrideParameters = false;
            if (overrideParameters)
            {
                peakParameters.isHammer = true;

                peakParameters.ScansToSum = 5;
                peakParameters.PeakToBackgroundRatio = 1.3;
                peakParameters.SignalToNoiseThreshold = 3;
                peakParameters.HammerMode = 1;
            }

            //step 2 figure out data set information. i.e scan numbers etc.   setup run and scanset

           
            string outputFolder = peakParameters.OutputDirectory;
            string outputPeaksFileName = peakParameters.OutputPeaksFileNameWithPath;
            const bool writeMetrics = false;

           
            
            if (peakParameters.isHammer)
            {
                var factor = new Run32.Backend.Runs.RunFactory();
                Run32.Backend.Core.Run runIn = factor.CreateRun(Run32.Backend.Globals.MSFileType.Finnigan, peakParameters.DataFileNameWithPath);

                var scanInfo = new ScanObject(0, 0)
				  {
	                Max = runIn.MaxLCScan,
	                Min = runIn.MinLCScan,
					ScansToSum = peakParameters.ScansToSum,          //5 is default, 15 for spin.  this number effects the EIC in GlyQIQ
	                Buffer = 9 * 2
                };
           
                runIn.ScanSetCollection.Create(runIn, scanInfo.Min, scanInfo.Max, scanInfo.ScansToSum, 1, false);

                Console.WriteLine("There are " + runIn.PrimaryLcScanNumbers.Count + " MS1 scans");
				
                
                Console.WriteLine("Using Hammer Peak Detector Mode " + peakParameters.HammerMode + Environment.NewLine);
                PeakDetectHammer.DetectPeaksWithHammer(peakParameters.DataFileName, outputFolder, outputPeaksFileName, runIn, writeMetrics, peakParameters.HammerMode);
            }
            else
            {
                var factor = new DeconTools.Backend.Runs.RunFactory();
                DeconTools.Backend.Core.Run runInDecon = factor.CreateRun(DeconTools.Backend.Globals.MSFileType.Finnigan, peakParameters.DataFileNameWithPath);

                var scanInfo = new ScanObject(0, 0)
                {
	                Max = runInDecon.MaxLCScan,
	                Min = runInDecon.MinLCScan,
					ScansToSum = peakParameters.ScansToSum, //5 is default, 15 for spin.  this number effects the EIC in GlyQIQ
	                Buffer = 9 * 2
                };

	            runInDecon.ScanSetCollection.Create(runInDecon, scanInfo.Min, scanInfo.Max, scanInfo.ScansToSum, 1, false);

                Console.WriteLine("There are " + runInDecon.PrimaryLcScanNumbers.Count + " MS1 scans");


                Console.WriteLine("Using Decon Peak Detector" +  Environment.NewLine);
                PeakDetectDeconTools.DetectPeaksWithDecontools(scanInfo, runInDecon, peakParameters);
            }

            Console.WriteLine("Done");
            
        }

        

        private static List<int> ScanLevelsWithTandemCalculator(GetPeaks_DLL.Objects.InputOutputFileName newFile)
        {
            int sizeOfDatabase; //get number of scans in total
            const int limitFileToThisManyScans = 50; //int limitFileToThisManyScans = 30000;
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

            var scanMSLevelList = new List<int>();
            List<int> scanLevelsWithTandem;
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
