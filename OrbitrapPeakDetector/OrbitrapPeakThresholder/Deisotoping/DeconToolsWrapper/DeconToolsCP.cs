using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using DeconTools.Backend.ProcessingTasks;
using GetPeaks_DLL;
using DeconTools.Backend.Core;
using GetPeaks_DLL.Go_Decon_Modules;
using GetPeaks_DLL.Objects;
using DeconTools.Backend.Runs;
using DeconTools.Backend.DTO;
using PNNLOmics.Algorithms.PeakDetection;
using GetPeaks_DLL.Common_Switches;
using GetPeaks_DLL.DataFIFO;
using Deisotoping.DeconToolsWrapper.Enumerations;

namespace Deisotoping.DeconToolsWrappers.DeconToolsWrapper
{
    public class DeconToolsCP
    {
        public List<List<ProcessedPeak>> runPeakDetector(PeakDetectorType filter, int scanNumber, string inputDataFilename,  SimpleWorkflowParameters parameters)
        {
            
            InputOutputFileName newFile = new InputOutputFileName();
            newFile.InputFileName = inputDataFilename;

            RunFactory rf = new RunFactory();
            //DeconvolutionType loadedDeconvolutionType = new DeconvolutionType();

            Run run = GoCreateRun.CreateRun(newFile);
            var msGenerator = MSGeneratorFactory.CreateMSGenerator(run.MSFileType);
            Console.WriteLine("after run was created");

            int scansToSum = 1;
            bool GetMSMSDataAlso = false;
            run.MinScan = scanNumber;
            run.MaxScan = scanNumber;
            run.ScanSetCollection = ScanSetCollection.Create(run, run.MinScan, run.MaxScan, scansToSum, 1, GetMSMSDataAlso);
            Console.WriteLine("LoadingData...");

            bool OmicsPeakDetection = false;//DeconTools
            parameters.Part1Parameters.NoiseType = InstrumentDataNoiseType.NoiseRemoved;//TOF

            switch(filter)
            {
                case PeakDetectorType.DeconTools:
                {
                    OmicsPeakDetection = false;//DeconTools
                }
                break;
                case PeakDetectorType.PNNLOmicsOrbitrap:
                {
                    OmicsPeakDetection = true;//PNNL Omics
                    parameters.Part1Parameters.NoiseType = InstrumentDataNoiseType.NoiseRemoved;//Orbitrap
                }
                break;
                case PeakDetectorType.PNNLOmicsTOF:
                {
                    OmicsPeakDetection = true;//PNNL Omics
                    parameters.Part1Parameters.NoiseType = InstrumentDataNoiseType.Standard;//TOF
                }
                break;
            }
            

           // 

            List<List<ProcessedPeak>> PileOfThresholdedData = new List<List<ProcessedPeak>>();

            int scanCounter = 0;
            foreach (ScanSet scanSet in run.ScanSetCollection.ScanSetList)
            {
                //int currentMSLevel = run.GetMSLevel(scanCounter);
                //scanCounter++;

                scanCounter = scanSet.PrimaryScanNumber;

                run.CurrentScanSet = scanSet;

                msGenerator.Execute(run.ResultCollection);

                List<ProcessedPeak> thresholdedData;

                if (OmicsPeakDetection)
                {
                    ///input: run.ResultCollection.Run.XYData
                    #region OmicsPeakDetection
                    //1.  extract XYData
                    List<double> tempXvalues = new List<double>();
                    List<double> tempYValues = new List<double>();
                    tempXvalues = run.ResultCollection.Run.XYData.Xvalues.ToList();
                    tempYValues = run.ResultCollection.Run.XYData.Yvalues.ToList();

                    List<PNNLOmics.Data.XYData> newData = new List<PNNLOmics.Data.XYData>();
                    for (int i = 0; i < run.ResultCollection.Run.XYData.Xvalues.Length; i++)
                    {
                        PNNLOmics.Data.XYData newPoint = new PNNLOmics.Data.XYData(tempXvalues[i], tempYValues[i]);
                        newData.Add(newPoint);
                    }

                    //2.  centroid peaks
                    PeakCentroider newPeakCentroider = new PeakCentroider();
                    //                            newPeakCentroider.Parameters.ScanNumber = scanCounter;
                    newPeakCentroider.Parameters.FWHMPeakFitType = SwitchPeakFitType.setPeakFitType(parameters.Part1Parameters.PeakFitType);

                    List<ProcessedPeak> discoveredPeakList = new List<ProcessedPeak>();
                    discoveredPeakList = newPeakCentroider.DiscoverPeaks(newData);

                    //writes data
                    //ConvertAutoData converter = new ConvertAutoData();
                    //List<XYData> xyPeakList = converter.ConvertProcessedPeakToXYData(discoveredPeakList);
                    //IXYDataWriter newWriter = new DataXYDataWriter();
                    //newWriter.WriteOmicsXYData(xyPeakList, @"V:\\PeakData2.txt");


                    //3.  threshold data
                    PeakThresholderParameters parametersThreshold = new PeakThresholderParameters();

                    parametersThreshold.isDataThresholded = parameters.Part1Parameters.isDataAlreadyThresholded;
                    //parametersThreshold.SignalToShoulderCuttoff = (float)parameters.Part1Parameters.ElutingPeakNoiseThreshold;
                    parametersThreshold.SignalToShoulderCuttoff = (float)parameters.Part1Parameters.MSPeakDetectorPeakBR;
                    

                    //                          parametersThreshold.ScanNumber = scanCounter;
                    parametersThreshold.DataNoiseType = parameters.Part1Parameters.NoiseType;

                    SwitchThreshold newSwitchThreshold = new SwitchThreshold();
                    newSwitchThreshold.Parameters = parametersThreshold;
                    newSwitchThreshold.ParametersOrbitrap = parameters.Part1Parameters.ParametersOrbitrap;
                    thresholdedData = newSwitchThreshold.ThresholdNow(ref discoveredPeakList);

                    #region vestigial print stuff off
                    //List<double> newList = new List<double>();
                    //foreach (ProcessedPeak peak in thresholdedData)
                    //{
                    //    if (peak.XValue > 1000f && peak.XValue < 1100f)
                    //    {
                    //        newList.Add(peak.XValue);
                    //    }
                    //}
                    #endregion

                    #region vestigial Add scan Number ON
                    for (int i = 0; i < thresholdedData.Count; i++)
                    {
                        thresholdedData[i].ScanNumber = scanSet.PrimaryScanNumber;
                    }
                    #endregion

                    //4. save data to Decon Tools
                    run.ResultCollection.PeakCounter = run.ResultCollection.MSPeakResultList.Count + thresholdedData.Count;

                    #region vestigial print stuff off
                    //List<double> printMasses = new List<double>();
                    //List<double> printInt = new List<double>();
                    // List<double> prinFWHM = new List<double>();
                    //string masses = "";
                    //string intensity = "";
                    //string FWHM = "";
                    #endregion

                    run.ResultCollection.Run.PeakList = new List<IPeak>();//this is a scan by scan peak list
                    for (int i = 0; i < thresholdedData.Count; i++)
                    {
                        DeconTools.Backend.Core.MSPeak newMSPeak = new DeconTools.Backend.Core.MSPeak();
                        newMSPeak.XValue = thresholdedData[i].XValue;
                        //TODO check this intensity = hegiht
                        newMSPeak.Height = Convert.ToSingle(thresholdedData[i].Height);
                        newMSPeak.Width = thresholdedData[i].Width;
                        newMSPeak.SN = Convert.ToSingle(thresholdedData[i].SignalToNoiseGlobal);
                        newMSPeak.DataIndex = thresholdedData[i].ScanNumber; //save scan here
                        //newMSPeak.DataIndex = scanCounter; //save scan here

                        #region vestigial print stuff off
                        //printMasses.Add(omicsList[i].X);
                        //printInt.Add(omicsList[i].Y);
                        //prinFWHM.Add(newMSPeak.Width);

                        //masses += printMasses[i].ToString() + ",";
                        //intensity += printInt[i].ToString() + ",";
                        //FWHM += prinFWHM[i].ToString() + ",";
                        #endregion

                        run.ResultCollection.Run.PeakList.Add(newMSPeak);

                        //update mspeakresults
                        MSPeakResult newMSPeakResult = new MSPeakResult();//this is a system wide peak list
                        newMSPeakResult.ChromID = -1;
                        newMSPeakResult.Frame_num = -1;
                        newMSPeakResult.MSPeak = newMSPeak;
                        newMSPeakResult.PeakID = i;
                        newMSPeakResult.Scan_num = thresholdedData[i].ScanNumber;
                        run.ResultCollection.MSPeakResultList.Add(newMSPeakResult);
                    }
                    #endregion
                    ///output: thresholdedData etc
                    ///output: run.ResultCollection.PeakCounter
                    ///output: run.ResultCollection.MSPeakResultList
                    ///output: run.ResultCollection.Run.PeakList 
                }
                else
                {
                    ///Input: run.ResultCollection.run.XYData
                    #region MSPeakDetector

                    run.ResultCollection.Run.PeakList = null;//clear it out because this is a scan by scan peak list
                    DeconToolsPeakDetector msPeakDetector = new DeconToolsPeakDetector(parameters.Part1Parameters.MSPeakDetectorPeakBR, parameters.Part1Parameters.ElutingPeakNoiseThreshold, parameters.Part2Parameters.PeakFitType, false);
                    msPeakDetector.Execute(run.ResultCollection);


                    #region vestigial convert to Omics Processed Peaks ON
                    thresholdedData = new List<ProcessedPeak>();
                    for (int i = 0; i < run.ResultCollection.Run.PeakList.Count; i++)
                    {
                        ProcessedPeak newPeakOmics = new ProcessedPeak();
                        newPeakOmics.XValue = run.ResultCollection.Run.PeakList[i].XValue;
                        newPeakOmics.Height = run.ResultCollection.Run.PeakList[i].Height;
                        newPeakOmics.Width = run.ResultCollection.Run.PeakList[i].Width;
                        newPeakOmics.ScanNumber = scanSet.PrimaryScanNumber;
                        thresholdedData.Add(newPeakOmics);
                    }
                    #endregion

                    #endregion
                    ///output: run.ResultCollection.PeakCounter
                    ///output: run.ResultCollection.MSPeakResultList
                    ///output: run.ResultCollection.Run.PeakList
                    ///output: run.ResultCollection.Run.DeconToolsPeakList
                }

                PileOfThresholdedData.Add(thresholdedData);
                int peakCount = thresholdedData.Count;
            }


            return PileOfThresholdedData;
        } 
    }
}
