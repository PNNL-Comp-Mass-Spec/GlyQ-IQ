using System;
using System.Collections.Generic;
using DeconTools.Backend.Core;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.Functions;
using HammerPeakDetector.Objects;
using HammerPeakDetector.Parameters;
using IQGlyQ.Processors;
using PNNLOmics.Data;

namespace HammerPeakDetectorConsole
{
    public static class PeakDetectHammer
    {
        public static void DetectPeaksWithHammer(string dataFileName, string outputFolder, string outputPeaksFileName, Run runIn, bool writeMetrics, int hammerMode)
        {
            GetPeaks_DLL.DataFIFO.PeakListTextExporter peakExporter = new GetPeaks_DLL.DataFIFO.PeakListTextExporter(runIn.MSFileType, outputPeaksFileName);
            
            //step 3 setup hammer parameters
            HammerThresholdParameters parameters = new HammerThresholdParameters();

            //int mode = 0; //cluster only.  pvalues will do the thresholding
            //int mode = 1; //cluster and threshold. thresholde by 0 or some sigma above moving average.  we keep all peak above this moving threshold including peaks that are not in clusters
            //int mode = 2; //cluster only with optimize with 1 iteration.  
            //int mode = 3; //cluster and threshold with optimize with 1 iteration. we keep all peak above this moving threshold including peaks that are not in clusters
            //int mode = 4; //cluster optimize with many iterations. 

            int mode = hammerMode;

            HammerPeakDetector.Utilities.SetParameters.SetHammerParameters(parameters, mode);
            parameters.ThresholdSigmaMultiplier = -0.5; //2 is sort of default//-0.5 is close to where the lines cross

            // step 4 setup iq processing parameters
            ProcessingParametersMassSpectra msParameters = new ProcessingParametersMassSpectra();
            IQGlyQ.Processors.ProcessorMassSpectra msProcessor = new ProcessorMassSpectra(msParameters);

            //setup global peak list
            List<string> allPeaks = new List<string>();
            List<string> centerAndSigma = new List<string>();
            //step 5 iterate over scans

            int start = 0;
            int stop = runIn.PrimaryLcScanNumbers.Count;
            int idCounter = 0;
            for (int i = start; i < stop; i++)
            {
                //1  select scan
                int currentScan = runIn.PrimaryLcScanNumbers[i]; //4
                ScanSet LCScanSetSelected = runIn.ScanSetCollection.GetScanSet(currentScan);

                Console.WriteLine("working on scan " + currentScan + " out of " + i + @"\" + runIn.PrimaryLcScanNumbers.Count);

                //2  get raw data and convert to omics
                msProcessor.DeconMSGeneratorWrapper(runIn, LCScanSetSelected);

                DeconTools.Backend.XYData MassSpectrum = msProcessor.DeconMSGeneratorWrapper(runIn, LCScanSetSelected);
                List<PNNLOmics.Data.XYData> convertedMassSpectrum = ConvertXYData.DeconXYDataToOmicsXYData(MassSpectrum);

                //3.  centroid peaks
                List<ProcessedPeak> centroidedPeaks = msProcessor.Execute(convertedMassSpectrum,EnumerationMassSpectraProcessing.OmicsCentroid_Only);

                //4.  convert to peak
                List<PNNLOmics.Data.Peak> toHammerV2 = new List<PNNLOmics.Data.Peak>();
                foreach (ProcessedPeak processePeak in centroidedPeaks)
                {
                    toHammerV2.Add(processePeak);
                }

                //5.  hammer the data into shape
                HammerThresholdResults results;
                List<ProcessedPeak> scottsResults = HammerPeakDetector.Hammer.Detect(toHammerV2, parameters, outputFolder, writeMetrics, out results);
                List<int> currentIds = new List<int>();
                foreach (var peak in scottsResults)
                {
                    currentIds.Add(idCounter);
                    peak.ScanNumber = currentScan;
                    idCounter++;
                }

                peakExporter.WriteOutPeaksOmics(scottsResults, currentIds);


                //step 4 convert to text strings to write out later
                //List<string> peaksAsString = DataPeaksDataWriter.ConvertPeaksTOStingsPeak(scottsResults, currentScan, "\t");
                //allPeaks.AddRange(peaksAsString);
                centerAndSigma.Add(currentScan + "\t" + results.NewParameters.finalCenter + "\t" + results.NewParameters.finalSigma);


                //step 5 after loop, write to text file
                //string fileName = @"D:\HammerPeaks\Scan_" + currentScan + "_peaks.txt";

                //StringListToDisk writer = new StringListToDisk();
                //writer.toDiskStringList(fileName, allPeaks);
                //int scan2 = 2615;
                //IPeakWriter writer = new DataPeaksDataWriter();
                //writer.WriteOmicsProcessedPeakData(scottsResults, scan2, fileName);
            }


       //     StringListToDisk writer = new StringListToDisk();
       //     writer.toDiskStringList(outputPeaksFileName, allPeaks);

            string fileNameSummary = outputFolder + @"\" + "CenterAndSigma" + dataFileName + ".txt";
            StringListToDisk summarywriter = new StringListToDisk();
            summarywriter.toDiskStringList(fileNameSummary, centerAndSigma);
        }


    }
}
