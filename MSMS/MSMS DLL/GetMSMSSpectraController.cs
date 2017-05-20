
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using DeconTools.Backend.ProcessingTasks;
using System.Diagnostics;
using GetPeaks_DLL.CompareContrast;
using DeconTools.Backend.Utilities;
using System.Threading;
using DeconTools.Backend.Runs;
using DeconTools.Backend;
using DeconTools.Backend.DTO;
using PNNLOmics.Data;
using GetPeaks_DLL.PNNLOmics_Modules;
using GetPeaks_DLL.Common_Switches;
using PNNLOmics.Algorithms.PeakDetection;
using System.Reflection;
using PNNLOmics.Data.Features;
using GetPeaks_DLL.SQLite;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.Go_Decon_Modules;
using GetPeaks_DLL;

namespace MSMS_DLL
{
    public class GetMSMSSpectraController:IDisposable
    {
        #region Properties

        public MSGeneratorFactory msgenFactory { get; set; }
        public Task msGenerator { get; set; }
        public SimpleWorkflowParameters Parameters;
        public DeconToolsPeakDetector msPeakDetector;
        public bool MSMSStoreXYData { get; set; }
        public bool MSMSStorePoints { get; set; }
        public bool MSMSOmicsPeakDecection { get; set; }//true for omics, false for decon
        public bool OrbitrapThresholdToggle { get; set; }

        #endregion

        #region Constructors
            
        public GetMSMSSpectraController()
        {
            this.msgenFactory = new MSGeneratorFactory();
            MSMSStoreXYData =true;
            MSMSStorePoints =true;
            MSMSOmicsPeakDecection = true;//or false decon
        }

        public GetMSMSSpectraController(Run run, SimpleWorkflowParameters parameters)
            : this()
        { 
            this.Parameters = parameters;
            InitializeTasks(run, this.Parameters);
        }

        private void InitializeTasks(Run run, SimpleWorkflowParameters parameters)
        {
            msGenerator = msgenFactory.CreateMSGenerator(run.MSFileType);
            //this is the first call to Decon Tools Engine V2.  Be sure to install .Net1.1
            msPeakDetector = new DeconToolsPeakDetector(parameters.Part1Parameters.MSPeakDetectorPeakBR, parameters.Part1Parameters.ElutingPeakNoiseThreshold, parameters.Part2Parameters.PeakFitType, false);
                
            msPeakDetector.StorePeakData = true;
            //msPeakDetector.PeakFitType = parameters.PeakFitType;   
        }

        #region idisposable
        public virtual void Close()
        {
            this.msgenFactory = null;
            this.msGenerator = null;
            this.Parameters = null;
            this.msPeakDetector = null;
        }

        public virtual void Dispose()
        {
            this.Close();
        }

        #endregion

        #endregion

        #region Public Methods
        /// <summary>
        /// Fetch XYData with DeconTools and convert to peaks with decon OMICS
        /// </summary>
        /// <param name="run"></param>
        public XYDataAndPeakHolderObject GrabXYData(Run run, int scanNumber)//create eluting peaks and store them in run.ResultsCollection.ElutingPeaks
        {
           
            //run.MinScan = this.Parameters.Part1Parameters.StartScan;
            //run.MaxScan = this.Parameters.Part1Parameters.StopScan;

            run.MinScan = scanNumber;//gets one scan
            run.MaxScan = scanNumber;
            //List<XYDataAndPeakHolderObject> dataObjectStorage = new List<XYDataAndPeakHolderObject>();

            try
            {
                bool GetMSMSDataAlso = true;//this is needed to get all the scans
                ScanSetCollectionCreator scanSetCollectionCreator = new ScanSetCollectionCreator(run, run.MinScan, run.MaxScan, 1, 1, GetMSMSDataAlso);
                //Console.WriteLine("LoadingData...");
                scanSetCollectionCreator.Create();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            int scanCounter = 0;
            bool isFirstScanSet = true;
            int scansetInitialOffset = 0; //offset of first scan with data
            int scanCounterPrevious = 0;//previous scanset scan number
            int scansetRangeOffset = Parameters.Part1Parameters.StartScan;//range offset.  linked to the start scan we are looping through

            XYDataAndPeakHolderObject newDataObject = new XYDataAndPeakHolderObject();

            foreach (ScanSet scanSet in run.ScanSetCollection.ScanSetList)//there should only be one scan set because range is limited
            {
                newDataObject = new XYDataAndPeakHolderObject();
                newDataObject.ScanSet = scanSet;
                newDataObject.DatasetScanNumber = scanSet.PrimaryScanNumber;
                 
                scanCounter = scanSet.PrimaryScanNumber;
                int currentMSLevel = run.GetMSLevel(scanCounter);

                //Console.WriteLine("Working on Scan " + scanCounter.ToString() + " with " + run.ResultCollection.ElutingPeakCollection.Count + " active " +
                //    " and " + dataObjectStorage.Count + " inactive ePeaks. Total: " + (run.ResultCollection.ElutingPeakCollection.Count + dataObjectStorage.Count).ToString());
                run.CurrentScanSet = scanSet;

                List<int> peakIndexList = new List<int>();

                #region get data and peaks
                try
                {
                    ///input: YAFMSRun
                    #region msGenerator

                    msGenerator.Execute(run.ResultCollection);

                    #endregion
                    ///output: run.ResultsCollection.Run.XYData and run.XYData

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

                    //1b. store XY data points
                        
                    if (MSMSStoreXYData)
                    {
                        //only one of these belongs with this scan??????
                        newDataObject.SpectraDataOMICS = newData;
                        newDataObject.SpectraDataDECON = run.ResultCollection.Run.XYData;
                    }
                    //dataObjectStorage.Add(newDataObject);

                    //2.  convert XYData to peaks
                    if (run.ResultCollection.Run.XYData.Xvalues.Length > 1)
                    {
                        #region Peak Detection
                        if (MSMSOmicsPeakDecection)
                        {
                            ///input: run.ResultCollection.Run.XYData
                            #region OmicsPeakDetection
                                
                            //2.  centroid peaks
                            PeakCentroider newPeakCentroider = new PeakCentroider();
//                            newPeakCentroider.Parameters.ScanNumber = scanCounter;
                            newPeakCentroider.Parameters.FWHMPeakFitType = SwitchPeakFitType.setPeakFitType(Parameters.Part1Parameters.PeakFitType);

                            List<ProcessedPeak> discoveredPeakList = new List<ProcessedPeak>();
                            discoveredPeakList = newPeakCentroider.DiscoverPeaks(newData);
           
                            //3.  threshold data
                            List<ProcessedPeak> thresholdedData;
                            if (OrbitrapThresholdToggle)
                            {
                                PeakThresholderParameters parametersThreshold = new PeakThresholderParameters();
                                parametersThreshold.SignalToShoulderCuttoff = (float)Parameters.Part1Parameters.ElutingPeakNoiseThreshold;
                                //                          parametersThreshold.ScanNumber = scanCounter;
                                parametersThreshold.DataNoiseType = Parameters.Part1Parameters.NoiseType;

                                SwitchThreshold newSwitchThreshold = new SwitchThreshold();
                                newSwitchThreshold.Parameters = parametersThreshold;
                                newSwitchThreshold.ParametersOrbitrap = Parameters.Part1Parameters.ParametersOrbitrap;
                                thresholdedData = newSwitchThreshold.ThresholdNow(ref discoveredPeakList);
                            }
                            else
                            {
                                thresholdedData = discoveredPeakList;
                            }
                            //4. save data to Decon Tools
                            run.ResultCollection.PeakCounter = run.ResultCollection.MSPeakResultList.Count + thresholdedData.Count;

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
                            
                            if (MSMSStorePoints)
                            {
                                newDataObject.PeaksDECON = run.ResultCollection.Run.PeakList;
                                newDataObject.PeaksOMICS = thresholdedData;
                            }
                        }
                        else
                        {
                            ///Input: run.ResultCollection.run.XYData
                            #region MSPeakDetector

                            run.ResultCollection.Run.PeakList = null;//clear it out because this is a scan by scan peak list
                            msPeakDetector.Execute(run.ResultCollection);

                            #region vestigial print stuff
                            List<double> printMasses = new List<double>();
                            List<double> printInt = new List<double>();
                            List<double> prinFWHM = new List<double>();
                            string masses2 = "";
                            string intensity2 = "";
                            string FWHM2 = "";


                            for (int i = 0; i < run.ResultCollection.Run.PeakList.Count; i++)
                            {
                                printMasses.Add(run.ResultCollection.Run.PeakList[i].XValue);

                                printInt.Add(run.ResultCollection.Run.PeakList[i].Height);

                                prinFWHM.Add(run.ResultCollection.Run.PeakList[i].Width);

                                masses2 += printMasses[i].ToString() + ",";
                                intensity2 += printInt[i].ToString() + ",";
                                FWHM2 += prinFWHM[i].ToString() + ",";
                            }
                            #endregion

                            #endregion
                            ///output: run.ResultCollection.PeakCounter
                            ///output: run.ResultCollection.MSPeakResultList
                            ///output: run.ResultCollection.Run.PeakList
                            ///output: run.ResultCollection.Run.DeconToolsPeakList
                                
                            if (MSMSStorePoints)
                            {
                                newDataObject.PeaksDECON = run.ResultCollection.Run.PeakList;
                                //newTandemObject.PeaksOMICS = thresholdedData;
                            }
                        }
                        #endregion

                        int MSpeakResultsListCount = run.ResultCollection.MSPeakResultList.Count;
                        int newPeaks = run.ResultCollection.Run.PeakList.Count;//new peaks found in PeakDetector

                        #region collect Peaks from msPeakDetector.  Keep track of indexes in MSpeakResultsList that coorespond to the local peakList
                        //this took 3.4 seconds and contains the int index reference to the full MSPeakResult
                        //List<double> peakmassesList = new List<double>();//used for debugging
                        for (int j = MSpeakResultsListCount - newPeaks; j < MSpeakResultsListCount; j++)
                        {
                            peakIndexList.Add(j);
                        }

                        #endregion
                    }

                    scanCounterPrevious = scanCounter;
                }

                catch (Exception ex)
                {
                    throw ex;
                }
                #endregion
            }

            return newDataObject;
        }
       
        #endregion
    }
}

