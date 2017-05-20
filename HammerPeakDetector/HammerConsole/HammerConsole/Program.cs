using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HammerPeakDetector;
using HammerPeakDetector.Enumerations;
using HammerPeakDetector.Utilities;
using IQGlyQ.UnitTesting;
using PNNLOmics.Data;
using GetPeaks_DLL.DataFIFO.OmicsFIFO;
using HammerPeakDetector.Objects;
using HammerPeakDetector.Parameters;
using GetPeaks_DLL.DataFIFO;
using HammerPeakDetector.Comparisons;
using MathNet.Numerics.Statistics;

namespace HammerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            bool writeDiagnostics = false;
            string writeFolder = @"D:\PNNL\Projects\Christina Polyukh\";    
            
            //double sigma = GetPeaks_DLL.Functions.ConvertSigmaAndPValue.PvalueToSigma(0.05);
            //sigma = GetPeaks_DLL.Functions.ConvertSigmaAndPValue.PvalueToSigma(0.1);
            //sigma = GetPeaks_DLL.Functions.ConvertSigmaAndPValue.PvalueToSigma(0.5);
            //sigma = GetPeaks_DLL.Functions.ConvertSigmaAndPValue.PvalueToSigma(0.8);





            //load data here (1 spectra at a time (sum data together)

            //details for peaks load
            //testFile = @"E:\ScottK\GetPeaks Data\Diabetes_LC\Gly09_Velos3_Jaguar_230nL30_C14_DB10_30uL1X_31Dec12.raw";
            // ScanObject scanInfo = new ScanObject(2615, 2684);
            //scanInfo.ScansToSum = 5;
            //scanInfo.Buffer = 9 * 2;

            List<Peak> toHammerV2 = LoadPeaksForHammer.Load();
            List<Peak> centroidedPeaks = toHammerV2;
            HammerThresholdParameters parameters = new HammerThresholdParameters();

            //For low noise, mode = 0; for high noise, mode = 1
            //int mode = 0; //cluster only.  pvalues will do the thresholding
            //int mode = 1; //cluster and threshold. thresholde by 0 or some sigma above moving average.  we keep all peak above this moving threshold including peaks that are not in clusters
            //int mode = 2; //cluster only with optimize with 1 iteration.  
            //int mode = 3; //cluster and threshold with optimize with 1 iteration. we keep all peak above this moving threshold including peaks that are not in clusters
            int mode = 4; //cluster and use p-value cutoff  to filter; use stone values
            SetParameters.SetHammerParameters(parameters, mode);

            HammerThresholdResults hammerResults;

            //parameters.ThresholdSigmaMultiplier = -0.5;

            List<ProcessedPeak> scottsResults = HammerPeakDetector.Hammer.Detect(centroidedPeaks, parameters, writeFolder, writeDiagnostics, out hammerResults);

            #region testing (off)

            //List<ProcessedPeak> filteredResults = new List<ProcessedPeak>();

            //foreach (ProcessedPeak peak in scottsResults)
            //{
            //    if (peak.Pvalue > 0.05)
            //    {
            //        filteredResults.Add(peak);
            //    }
            //}

            //List<double> pValueList = new List<double>();
            //int numNonClustered = 0;
            //foreach (ProcessedPeak peak in scottsResults)
            //{
            //    pValueList.Add(peak.Pvalue);
            //    if (peak.Pvalue == -1)
            //    {
            //        numNonClustered++;  
            //    }
            //}

            //int numBuckets = 50;

            ////Create histogram
            //Histogram histogram = new Histogram(pValueList, numBuckets, 0, 1);
            //DataConverter converter = new DataConverter();

            //List<XYData> xyPeaks = converter.PeakToXYData(centroidedPeaks);
            //List<XYData> xyHistogram = converter.HistogramToXYData(histogram);
            
            //GetPeaks_DLL.DataFIFO.IXYDataWriter write = new DataXYDataWriter();
            //StringListToDisk writerStrings = new StringListToDisk();
            //write.WriteOmicsXYData(xyHistogram, writeFolder + "HistogramOfPValueThroughThresholding.csv");

            #endregion

            //ROC testing
            if (writeDiagnostics)
            {

                #region ROC results setup
                DataConverter convertData = new DataConverter();
                GetData findData = new GetData();

                List<ProcessedPeak> allPeaks = convertData.PeaksToProcessedPeaks(centroidedPeaks);
                List<ProcessedPeak> signalManual = findData.LoadProcessedPeakAnswers(SpectraDataType.SignalPeaks);

                ROCresults findResults = new ROCresults();
                ROCobject ROCanalysisResults;

                #endregion

                #region Testing for specificity and sensitivity with varied parameters

                IXYDataWriter XYDataWriter = new DataXYDataWriter();
                List<XYData> xyResults = new List<XYData>();

                int testType = 1; //0 = test ThresholdSigmaMultiplier; 1 = test PValue

                switch (testType)
                {
                    case 0:

                        mode = 1;

                        for (double i = -1.25; i <= 3; i += 0.25)
                        {
                            SetParameters.SetHammerParameters(parameters, mode);

                            parameters.ThresholdSigmaMultiplier = i;

                            scottsResults = HammerPeakDetector.Hammer.Detect(centroidedPeaks, parameters, writeFolder, writeDiagnostics, out hammerResults);

                            ROCanalysisResults = findResults.calculateROC(allPeaks, scottsResults, signalManual);

                            xyResults.Add(new XYData(ROCanalysisResults.Sensitivity, ROCanalysisResults.Specificity));
                        }

                        XYDataWriter.WriteOmicsXYData(xyResults, writeFolder + "SensitivityVsSpecificityVariedThresholdSigmaMultiplier.csv");

                        break;
                    case 1:

                        mode = 4;

                        double pVal = 0;

                        for (int num = 1; num < 100; num += 1)
                        //for (int num = 50; num < 100; num += 1)
                        {
                            pVal = (double)num / 100;
                            Console.WriteLine(num);
                            SetParameters.SetHammerParameters(parameters, mode);

                            parameters.MassTolleranceSigmaMultiplier = GetPeaks_DLL.Functions.ConvertSigmaAndPValue.PvalueToSigma(pVal);

                            scottsResults = HammerPeakDetector.Hammer.Detect(centroidedPeaks, parameters, writeFolder, writeDiagnostics, out hammerResults);

                            ROCanalysisResults = findResults.calculateROC(allPeaks, scottsResults, signalManual);

                            xyResults.Add(new XYData(ROCanalysisResults.Sensitivity, ROCanalysisResults.Specificity));
                        }

                        XYDataWriter.WriteOmicsXYData(xyResults, writeFolder + "SensitivityVsSpecificityVariedPValueCutoff.csv");

                        break;
                }

                #endregion

            }

            //write to text file
            string fileName = @"D:\test.txt";
            int scan = 2615;

            IPeakWriter writer = new DataPeaksDataWriter();
            writer.WriteOmicsProcessedPeakData(scottsResults, scan, fileName);

            Console.WriteLine("Done");
            Console.ReadKey();
            //write to YAFMS-DB
        }


        

    }
}
