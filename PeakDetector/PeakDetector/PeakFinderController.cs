using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using DeconTools.Backend.ProcessingTasks;
using System.Diagnostics;
using DeconTools.Backend.Utilities;
using System.Threading;
using DeconTools.Backend.Runs;
using DeconTools.Backend;
using DeconTools.Backend.DTO;
using PeakDetector;
using PNNLOmics.Algorithms.PeakDetector;
using PNNLOmics.Data;


namespace GetPeaks_DLL
{
    public class PeakFinderController
    {
        #region Properties

        public MSGeneratorFactory msgenFactory { get; set; }
        public Task msGenerator {get;set;}
        public SimpleWorkflowParameters Parameters;
        public DeconToolsPeakDetector msPeakDetector;
        const double TOLERANCE = 0.05; 

        #endregion

        #region Constructors
            
            public PeakFinderController()
            {
                this.msgenFactory = new MSGeneratorFactory();
            }

            public PeakFinderController(Run run, SimpleWorkflowParameters parameters)
                : this()
            { 
                this.Parameters = parameters;
                InitializeTasks(run, this.Parameters);
            }

            private void InitializeTasks(Run run, SimpleWorkflowParameters parameters)
            {
                msGenerator = msgenFactory.CreateMSGenerator(run.MSFileType);
                msPeakDetector = new DeconToolsPeakDetector(parameters.MSPeakDetectorPeakBR, parameters.MSPeakDetectorSigNoise, parameters.PeakFitType, false);
                msPeakDetector.StorePeakData = true;
                //msPeakDetector.PeakFitType = parameters.PeakFitType;
            }
        
        #endregion

        #region Public Methods
            /// <summary>
            /// Create Peaks
            /// </summary>
            /// <param name="run"></param>
            public void SimpleWorkflowExecutePart1(Run run, ref List<PNNLOmics.Data.XYData> fromDeconPeakList, ref List<CentroidedPeak> fromOmicsPeakDetector)//create eluting peaks and store them in run.ResultsCollection.ElutingPeaks
            {
                System.Timers.Timer aTimer = new System.Timers.Timer();
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                #region create scanset
                run.MinScan = this.Parameters.StartScan;
                run.MaxScan = this.Parameters.StopScan;
                try
                {
                    ScanSetCollectionCreator scanSetCollectionCreator = new ScanSetCollectionCreator(run, run.MinScan, run.MaxScan, 1, 1, false);
                    Console.WriteLine("LoadingData...");
                    scanSetCollectionCreator.Create();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                #endregion

                foreach (ScanSet scanSet in run.ScanSetCollection.ScanSetList)
                {
                    run.CurrentScanSet = scanSet;
                    int scanNumber = scanSet.PrimaryScanNumber;
                    List<int> peakIndexList = new List<int>();

                    #region Main Workflow
                    try
                    {
                        #region msGenerator, used to generate XYData
                        ///input file
                        ///output run.ResultsCollection.Run.XYData and run.XYData
                        msGenerator.Execute(run.ResultCollection);
                        #endregion

                        #region remap the Decon XYData variables into PNNLOmics XYData variables
                        List<double> tempXvalues = new List<double>();
                        List<double> tempYValues = new List<double>();
                        tempXvalues = run.ResultCollection.Run.XYData.Xvalues.ToList();
                        tempYValues = run.ResultCollection.Run.XYData.Yvalues.ToList();

                        List<PNNLOmics.Data.XYData> newData = new List<PNNLOmics.Data.XYData>();
                        for(int i=0;i<run.ResultCollection.Run.XYData.Xvalues.Length;i++)
                        {
                            PNNLOmics.Data.XYData newPoint = new PNNLOmics.Data.XYData();
                            newPoint.X = tempXvalues[i];
                            newPoint.Y = tempYValues[i];
                            newData.Add(newPoint);
                        }
                        #endregion

                        #region omics peak detector
                        //part 1:  Centroid Peaks
                        PeakCentroidParameters parametersPeakCentroid = new PeakCentroidParameters();
                        parametersPeakCentroid.ScanNumber = scanNumber;

                        List<CentroidedPeak> centroidedPeakList = new List<CentroidedPeak>();
                        centroidedPeakList=PeakCentroid.DiscoverPeaks(newData, parametersPeakCentroid);
                        
                        //Part 2:  Noise Thresholding
                        PeakThresholdParameters parametersThreshold = new PeakThresholdParameters();
                        float omicsPeakDetectorSigNoise = (float)(this.Parameters.MSPeakDetectorSigNoise);//this is where we set the value
                        parametersThreshold.SignalToShoulderCuttoff = omicsPeakDetectorSigNoise;

                        List<CentroidedPeak> thresholdedData = new List<CentroidedPeak>();
                        thresholdedData=PeakThreshold.ApplyThreshold(ref centroidedPeakList, parametersThreshold);
                        #endregion

                        #region Basic MSPeakDetector for comparison
                        ///Input is run.ResultCollection.run.XYData
                        ///output is run.ResultCollection.run.PeakList and run.PeakList
                        run.ResultCollection.Run.PeakList = null;
                        msPeakDetector.Execute(run.ResultCollection);

                        #endregion

                        #region remap decon peaks to PNNL Omics peaks for comparison
                        fromOmicsPeakDetector = thresholdedData; //parametersThreshold.ThresholdedPeakData;
                        for (int i = 0; i < run.ResultCollection.Run.PeakList.Count; i++)
                        {
                            PNNLOmics.Data.XYData newXYpoint = new PNNLOmics.Data.XYData();
                            newXYpoint.X = run.ResultCollection.Run.PeakList[i].XValue;
                            newXYpoint.Y = run.ResultCollection.Run.PeakList[i].Height;
                            fromDeconPeakList.Add(newXYpoint);
                        }
                        #endregion

                        break;  //only do first scan in scan set
                    }

                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    #endregion    
                }
          
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                Console.WriteLine("This took " + ts + "seconds");
            }
        #endregion
    }
}
