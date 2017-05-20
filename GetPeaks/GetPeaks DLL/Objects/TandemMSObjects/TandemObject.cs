using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using GetPeaks_DLL.CompareContrast;
using GetPeaks_DLL.Go_Decon_Modules;
using DeconTools.Backend.ProcessingTasks;
using DeconTools.Backend.Utilities;
using GetPeaks_DLL.Functions;
using GetPeaks_DLL.THRASH;
//using OrbitrapPeakDetection;
using PNNLOmics.Algorithms.PeakDetection;
using GetPeaks_DLL.Common_Switches;
using PNNLOmics.Data;
using GetPeaks_DLL.SQLite;
using System.Threading.Tasks;
using CompareContrastDLL;
using GetPeaks_DLL.Objects.Enumerations;
using DeconTools.Backend;
using Parallel.THRASH;

namespace GetPeaks_DLL.Objects.TandemMSObjects
{
    public class TandemObject:IDisposable
    {
        #region properties

        public List<DeconTools.Backend.StandardIsosResult> PeaksIsosResultsDECON { get; set; }
        
        /// <summary>
        /// Monoisotopic Peaks from the Precursor Scan (this is currently operated by the fragmentation methods)
        /// </summary>
        public List<PNNLOmics.Data.ProcessedPeak> PrecursorMonoIsotopicPeaks { get; set; }

        public List<PNNLOmics.Data.ProcessedPeak> PrecursorScanPeaks { get; set; }

        /// <summary>
        /// Monoisotopic Peaks Charge States from the Precursor Scan (this is currently operated by the fragmentation methods)
        /// </summary>
        public List<int> PrecursorScanChargeStates { get; set; }


        /// <summary>
        /// the mass of the ion in the center of the fragmentation window as read from the thermo text string
        /// </summary>
        public double PrecursorMZ { get; set; }

        /// <summary>
        /// which scan number the precursor was fragmented from
        /// </summary>
        public Int32 PrecursorScanNumber { get; set; }

        /// <summary>
        /// storage for precursor raw XYData
        /// </summary>
        public List<PNNLOmics.Data.XYData> PrecursorData { get; set; }

        /// <summary>
        /// the scan number of the fragmentation spectra
        /// </summary>
        public Int32 FragmentationScanNumber { get; set; }

        /// <summary>
        /// storage for fragmentation raw XYData
        /// </summary>
        public List<PNNLOmics.Data.XYData> FragmentationData { get; set; }

        /// <summary>
        /// storage for peaks from the fragmentation scan
        /// </summary>
        public List<PNNLOmics.Data.ProcessedPeak> FragmentationPeaks { get; set; }

        /// <summary>
        /// MSlevel from decon tools 2=msms
        /// </summary>
        public int FragmentationMSLevel { get; set; }

        /// <summary>
        /// This is needd to link the data on the disk so we can load XYdata
        /// </summary>
        public InputOutputFileName InputFileName { get; set; }
        
        /// <summary>
        /// the mass of the ion in the center of the fragmentation window found in the data
        /// </summary>
        public double PrecursorMZCorrected { get; set; }

        /// <summary>
        /// the inteninsity of the ion in the center of the fragmentation window found in the data
        /// </summary>
        public double PrecursorIntensityCorrected { get; set; }

        /// <summary>
        /// common run since we need it so much.  We need to add a private and deconstructor
        /// </summary>
        public Run m_run;

        ///// <summary>
        ///// common msgenFactory since we need it so much.  We need to add a private and deconstructor
        ///// </summary>
        //private MSGeneratorFactory m_msgenFactory;

        /// <summary>
        /// common msGenerator since we need it so much.    We need to add a private and deconstructor
        /// </summary>
        //private Task m_msGenerator;


        /// <summary>
        /// lock for deisotoping
        /// </summary>
        private Object tranformLock { get; set; }

        /// <summary>
        /// Parameters for thrash and peak picking
        /// </summary>
        //public SimpleWorkflowParameters Parameters { get; set; }

        /// <summary>
        /// Parameters for thrash and peak picking
        /// </summary>
        public ParametersTHRASH ThrashParameters { get; set; }

        /// <summary>
        /// FileInformation
        /// </summary>
        public InputOutputFileName NewFile { get; set; }

        
    

        #endregion

        #region constructors

        /// <summary>
        /// constructor with precorsor scan and file name
        /// </summary>
        /// <param name="inputFileName"></param>
        /// <param name="precursorScanNumber"></param>
        public TandemObject(InputOutputFileName inputFileName, int precursorScanNumber, ParametersTHRASH thrashParameters)
        {
            InitializeObject();

            ThrashParameters = thrashParameters;
            InputFileName = inputFileName;
            //m_run = GoCreateRun.CreateRun(InputFileName);
            PrecursorScanNumber = precursorScanNumber;
        }

        /// <summary>
        /// default constructor
        /// </summary>
        public TandemObject()
        {
            InitializeObject();
        }

        /// <summary>
        /// prep several lists
        /// </summary>
        private void InitializeObject()
        {
            tranformLock = new Object();
            FragmentationScanNumber = -1;
            PrecursorScanNumber = -1;

            PrecursorData = new List<PNNLOmics.Data.XYData>();
            FragmentationData = new List<PNNLOmics.Data.XYData>();

            PeaksIsosResultsDECON = new List<DeconTools.Backend.StandardIsosResult>();
        }

        #endregion

        /// <summary>
        /// clears a run on a tandem object
        /// </summary>
        public void DisposeRun()
        {
            m_run.Dispose();
            m_run = null;
        }

        /// <summary>
        /// creates a run on a tandem object
        /// </summary>
        public void LoadRun()
        {
            if (InputFileName != null)
            {
                if (m_run == null)
                {
                    m_run = GoCreateRun.CreateRun(InputFileName);
                }
                else
                {
                    Console.WriteLine("run alreadt exists");
                }
            }
            else
            {
                Console.WriteLine("FileName is Missing to create run");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// pulls in XYdata from a file that is corresponds to a fragment spectra
        /// </summary>
        public void LoadFragmentationData()
        {
            if (FragmentationScanNumber < 0)///check to see we have a scan number to work on
            {
                Console.WriteLine("we need a fragmentation scan number before we can populate the XYdata");
                Console.ReadKey();
            }

            FragmentationData = GetData(FragmentationScanNumber);  
        }

        /// <summary>
        /// Convert XYdata into peaks so we can deisotope later.  Orbitrap filter will be usefull here
        /// </summary>
        public void PeakPickFragmentationData()
        {
            if (FragmentationData.Count == 0)
            {
                this.LoadFragmentationData();
            }

            FragmentationPeaks = PeakFindAndThreshold(FragmentationData, m_run, ThrashParameters.PeakDetectionMultiParameters);
        }

        /// <summary>
        /// pull in XYdata from file
        /// </summary>
        public void LoadPrecursorData()
        {
            if (PrecursorScanNumber < 0)///check to see we have a scan number to work on
            {
                Console.WriteLine("we need a precursor scan number before we can populate the XYdata");
                Console.ReadKey();
            }

            PrecursorData = GetData(PrecursorScanNumber); 
        }

        /// <summary>
        /// convert XYdata into peaks so we can refine the MZ pulled from the text string
        /// </summary>
        public void PeakPickPrecursorData()
        {
            if (PrecursorData.Count == 0)
            {
                this.LoadPrecursorData();
            }

            PrecursorScanPeaks = PeakFindAndThreshold(PrecursorData, m_run, ThrashParameters.PeakDetectionMultiParameters);
        }

        /// <summary>
        /// loads the xydata from the precursor mass
        /// </summary>
        public void LoadPrecursorMass()
        {
            if (PrecursorScanPeaks == null)
            {
                this.PeakPickPrecursorData();
            }

            #region 1.  load mass from data file
            m_run.MinLCScan = FragmentationScanNumber;
            m_run.MaxLCScan = FragmentationScanNumber;

            int scansToSum = 1;
            try
            {
                bool GetMSMSDataAlso = true;

                m_run.ScanSetCollection.Create(m_run, m_run.MinLCScan, m_run.MaxLCScan, scansToSum, 1, GetMSMSDataAlso);
                //m_run.ScanSetCollection = ScanSetCollection.Create(m_run, m_run.MinLCScan, m_run.MaxLCScan, scansToSum, 1, GetMSMSDataAlso);
                Console.WriteLine("LoadingData2...");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            switch (m_run.MSFileType)
            {
                case DeconTools.Backend.Globals.MSFileType.YAFMS:
                    {
                        double PrecursorYAFMS = 0;
                        DatabaseReader newReader = new DatabaseReader();
                        newReader.readTandemMassSpectraData(m_run.Filename, FragmentationScanNumber, out PrecursorYAFMS);
                        PrecursorMZ = PrecursorYAFMS;
                    }
                    break;

                case DeconTools.Backend.Globals.MSFileType.Finnigan:
                    {
                        string scanInfo = m_run.GetScanInfo(m_run.ScanSetCollection.ScanSetList[0].PrimaryScanNumber);//0 because we are only allowing one scan through
                        //string scanInfo = m_run.GetScanInfo(m_run.CurrentScanSet.PrimaryScanNumber);//0 because we are only allowing one scan through
                        
                        PrecursorMZ = ParseThermoScanInfo.ExtractMass(scanInfo);
                    }
                    break;

                default:
                    {
                        Console.WriteLine("wrong file type in TandemObject");

                        string scanInfo = m_run.GetScanInfo(m_run.ScanSetCollection.ScanSetList[0].PrimaryScanNumber);//0 because we are only allowing one scan through
                        PrecursorMZ = ParseThermoScanInfo.ExtractMass(scanInfo);

                        Console.ReadKey();
                    }
                    break;
            }

            #endregion

            #region 2.  refine mass
            if (PrecursorMZ > 0)
            {
                double tollerance = 50;

                List<decimal> libraryList = new List<decimal>();
                libraryList.Add(Convert.ToDecimal(PrecursorMZ));

                List<decimal> dataList = new List<decimal>();
                foreach (ProcessedPeak peak in PrecursorScanPeaks)
                {
                    dataList.Add(Convert.ToDecimal(peak.XValue));
                }

                SetListsToCompare prepCompare = new SetListsToCompare();
                CompareInputLists inputLists = prepCompare.SetThem(dataList, libraryList);

                //CompareContrast.CompareController compareHere = new CompareController();
                //CompareResultsValues valuesFromCompare = compareHere.compareFX(inputLists, tollerance, ref indexesFromCompare);

                CompareResultsIndexes resultsFromCompare = new CompareResultsIndexes();
                CompareController compareHere = new CompareController();
                CompareResultsValues valuesFromCompare = compareHere.compareFX(inputLists, tollerance, ref resultsFromCompare);
                
                
                //if we can't find a mass within 50 ppm, use origional
                double bestHit;
                double bestCorrespondingIntensity;
                if (valuesFromCompare.DataMatchingLibrary.Count > 0)
                {
                    double lowestppm = ErrorCalculator.PPMAbsolute(Convert.ToDouble(valuesFromCompare.DataMatchingLibrary[0]), PrecursorMZ);
                    bestHit = Convert.ToDouble(valuesFromCompare.DataMatchingLibrary[0]);
                    //bestCorrespondingIntensity = PrecursorScanPeaks[indexesFromCompare.IndexListAMatch[0]].Height;
                    bestCorrespondingIntensity = PrecursorScanPeaks[resultsFromCompare.IndexListAMatch[0]].Height;
                    for (int k = 0; k < valuesFromCompare.DataMatchingLibrary.Count; k++)
                    //foreach (decimal mass in valuesFromCompare.DataMatchingLibrary)
                    {
                        decimal mass = valuesFromCompare.DataMatchingLibrary[k];
                        //double intensity = PrecursorScanPeaks[indexesFromCompare.IndexListAMatch[k]].Height;
                        double intensity = PrecursorScanPeaks[resultsFromCompare.IndexListAMatch[k]].Height;
                        double ppm = ErrorCalculator.PPMAbsolute(Convert.ToDouble(mass), PrecursorMZ);
                        if (ppm < lowestppm)
                        {
                            lowestppm = ppm;
                            bestHit = Convert.ToDouble(mass);
                            bestCorrespondingIntensity = intensity;
                        }
                    }
                }
                else
                {
                    bestHit = PrecursorMZ;//use origional data
                    bestCorrespondingIntensity = 1;//value greater than 0;
                }

                PrecursorMZCorrected = bestHit;
                PrecursorIntensityCorrected = bestCorrespondingIntensity;

            }
            else
            {
                PrecursorMZCorrected = -1;
                PrecursorIntensityCorrected = -1;
            }
            #endregion
        }

        /// <summary>
        /// Select MonoisotopicMasses from Fragmentation peak list
        /// </summary>
        //public void DeisotopeFragmentationPeaks(InputOutputFileName newFile)
        public void DeisotopeFragmentationPeaks()
        {
            //List<ProcessedPeak> CurrentListOfInterest = FragmentationPeaks;
            List<ProcessedPeak> CurrentListOfInterest = PrecursorScanPeaks;
            List<List<ProcessedPeak>> PileOfScans = new List<List<ProcessedPeak>>();
            PileOfScans.Add(CurrentListOfInterest);
            //this is a lock for each thransformer
            Object databaseLockMulti = new Object();//global lock for database
            
            //check parameters
            if (ThrashParameters == null)
            {
                Console.WriteLine("Parameters are not loaded yet.  please load them first");
                Console.ReadKey();
                //Parameters = new SimpleWorkflowParameters();
                //Parameters.Part2Parameters.numberOfDeconvolutionThreads = 1;
            }

            //if (Parameters.Part2Parameters.numberOfDeconvolutionThreads == 0)
            if (ThrashParameters.CoresPerComputer == 0)
            {
                Console.WriteLine("Number of Threads cannot be zero");
                Console.ReadKey();
            }

            int maxNumberOfThreadsToUse = ThrashParameters.CoresPerComputer;//  Parameters.Part2Parameters.numberOfDeconvolutionThreads;
            int sizeOfTransformerArmy = maxNumberOfThreadsToUse * 2 + 1;//+1 is for emergencies
            GoTransformParameters transformerParameterSetup = new GoTransformParameters();
            HornTransformParameters hornParameters = new HornTransformParameters();
            transformerParameterSetup.Parameters = hornParameters;

            GoTransformPrep setupTransformers = new GoTransformPrep();
            List<TransformerObject> transformerList = setupTransformers.PreparArmyOfTransformers(transformerParameterSetup, sizeOfTransformerArmy, NewFile, databaseLockMulti);

            int threadToggle = 0;
            int counter = 0;
            //int hits = 0;
            int totalCount = CurrentListOfInterest.Count;
            int dataFileIterator = 0;
            int elutingpeakHits = 0;
            
            //(1/2)
            //Parallel.ForEach(PrecursorScanPeaks, new ParallelOptions { MaxDegreeOfParallelism = maxNumberOfThreadsToUse }, oPeak =>
            foreach (List<ProcessedPeak> oPeakList in PileOfScans)
            {
                //ElutingPeakOmics ePeak = discoveredPeaks[0]; 

                #region inside
                int threadName = System.Threading.Thread.CurrentThread.ManagedThreadId;
                Console.WriteLine("newThread" + threadName + "Peak " + counter + " out of " + totalCount);

                TransformerObject transformerMulti;// = transformer2;

                lock (tranformLock)//assignes a transformer to a thread
                {
                    transformerMulti = GoAssignTransform.Assign(maxNumberOfThreadsToUse, transformerList, ref threadToggle);
                    counter++;
                }
                
                
                
                SimpleWorkflowParameters paralellParameters = new SimpleWorkflowParameters();
                //this line was removed because Parameters is not gone
                //paralellParameters = ObjectCopier.Clone<SimpleWorkflowParameters>(Parameters);
                
                List<ProcessedPeak> newSingleList = new List<ProcessedPeak>();
                for (int i = 0; i < oPeakList.Count; i++)
                {
                    ProcessedPeak newPoint = new ProcessedPeak();
                    newPoint = oPeakList[i];
                    newSingleList.Add(newPoint);
                }
                

                GoTHRASH newController = new GoTHRASH();
                newController.MultiThread(newSingleList, PrecursorData, NewFile, paralellParameters, ref elutingpeakHits, transformerMulti, ref dataFileIterator);

                PrecursorMonoIsotopicPeaks = newController.MonoIsotopicMasses;
                PrecursorScanChargeStates = newController.ChargeStates;

                paralellParameters.Dispose();
                paralellParameters = null;
                //TODO memory leak.  perhaps need idisposable on single list inside
                newSingleList[0].Clear();
                newSingleList = null;

                newController.Dispose();

                //this line comes from a custom Engine V2, not the standard Decontools dll
                //transformerMulti.TransformEngine.PercentDone = 0;
                transformerMulti.active = false;
                //Console.WriteLine("press enter"); Console.ReadKey();
                #endregion
            //(2//2)
            }
            //});
        }

        ///// <summary>
        ///// Select MonoisotopicMasses from Fragmentation peak list
        ///// </summary>
        ////public void DeisotopeFragmentationPeaks(InputOutputFileName newFile)
        //public ResultsTHRASH DeisotopePeaksV2()
        //{
        //    List<ProcessedPeak> currentListOfInterest = FragmentationPeaks;
            

        //    ConvertOmicsProcessedPeaksToDeconPeaks converter = new ConvertOmicsProcessedPeaksToDeconPeaks();
        //    List <DeconTools.Backend.Core.MSPeak> deconPeakList = converter.ConvertPeaks(currentListOfInterest);
        //    //convert to decon peaks
        //    Run runGo = m_run;
        //    runGo.PeakList = deconPeakList;
            
        //    //List<ProcessedPeak> CurrentListOfInterest = PrecursorScanPeaks;
        //    //List<List<ProcessedPeak>> PileOfScans = new List<List<ProcessedPeak>>();
        //    //PileOfScans.Add(CurrentListOfInterest);
        //    //this is a lock for each thransformer
        //    Object databaseLockMulti = new Object();//global lock for database

        //    //check parameters
        //    if (ThrashParameters == null)
        //    {
        //        Console.WriteLine("Parameters are not loaded yet");
        //        Console.ReadKey();
        //        //Parameters = new SimpleWorkflowParameters();
        //        //Parameters.Part2Parameters.numberOfDeconvolutionThreads = 1;
        //    }

        //    ResultsTHRASH results = new ResultsTHRASH(PrecursorScanNumber);
        //    EngineThrashDeconvolutor engine = new EngineThrashDeconvolutor();
        //    GoThrashEngine.DeconvoluteWithEngine(PrecursorScanNumber, results, runGo, engine);
        //    return results;
        //}

        
        #region private functions

        /// <summary>
        /// basic loader
        /// </summary>
        /// <param name="scanNumber">scan we want to load the XYdata for</param>
        /// <returns>returns a converted PNNLOnics list</returns>
        private List<PNNLOmics.Data.XYData> GetData(int scanNumber)
        {
            m_run.MinLCScan = scanNumber;
            m_run.MaxLCScan = scanNumber;

            try
            {
                bool GetMSMSDataAlso = true;
                int scansToSum = 1;


                m_run.ScanSetCollection.Create(m_run, m_run.MinLCScan, m_run.MaxLCScan, scansToSum, 1, GetMSMSDataAlso);
                //m_run.ScanSetCollection = ScanSetCollection.Create(m_run, m_run.MinLCScan, m_run.MaxLCScan, scansToSum, 1, GetMSMSDataAlso);
                Console.WriteLine("LoadingData...");

                m_run.CurrentScanSet = m_run.ScanSetCollection.ScanSetList[0];

                GoSpectraGenerator.GenerateMS(m_run);
                //var msGenerator = MSGeneratorFactory.CreateMSGenerator(m_run.MSFileType);
                //msGenerator.Execute(m_run.ResultCollection);
                //msGenerator.Cleanup();
                //msGenerator = null;
                //m_msGenerator.Execute(m_run.ResultCollection);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            List<PNNLOmics.Data.XYData> newList = new List<PNNLOmics.Data.XYData>();
            ConvertXYData.RunXYDataToOmicsXYData(m_run, out newList);
            return newList;
        }

        /// <summary>
        /// combines peak detection and thresholding
        /// </summary>
        /// <param name="currentData"></param>
        /// <returns></returns>
        public static List<ProcessedPeak> PeakFindAndThreshold(List<PNNLOmics.Data.XYData> currentData, Run runGo, ParametersPeakDetection peakDetectionParameters)
        {
            ////2.  centroid peaks
            //PeakCentroider newPeakCentroider = new PeakCentroider();
            //newPeakCentroider.Parameters.FWHMPeakFitType = PeakFitType.Lorentzian;

            //List<ProcessedPeak> discoveredPeakList = new List<ProcessedPeak>();
            //discoveredPeakList = newPeakCentroider.DiscoverPeaks(currentData);

            ////3.  threshold
            //PeakThresholder newPeakThresholder = new PeakThresholder();
            //newPeakThresholder.Parameters.SignalToShoulderCuttoff = (float)parameters.Part1Parameters.MSPeakDetectorPeakBR;

            //discoveredPeakList = newPeakThresholder.ApplyThreshold(discoveredPeakList);

            runGo.XYData = new DeconTools.Backend.XYData();
            runGo.XYData.Xvalues = new double[currentData.Count];
            runGo.XYData.Yvalues = new double[currentData.Count];
            for (int i = 0; i < currentData.Count; i++)
            {
                runGo.XYData.Xvalues[i] = currentData[i].X;
                runGo.XYData.Yvalues[i] = currentData[i].Y;
            }
            
            ResultsPeakDetection peakObjectinsideThread = new ResultsPeakDetection();

            

            List<string> logAddition;
            GoPeakDetection.PeakProcessing(peakDetectionParameters, runGo, peakObjectinsideThread, out logAddition);

            List<ProcessedPeak> discoveredPeakList = new List<ProcessedPeak>();
            foreach (DeconTools.Backend.Core.Peak dPeak in runGo.ResultCollection.Run.PeakList)
            {
                ProcessedPeak pPeak = new ProcessedPeak();
                pPeak.Height = dPeak.Height;
                pPeak.XValue = dPeak.XValue;
                pPeak.Width = dPeak.Width;
                pPeak.MinimaOfLowerMassIndex = dPeak.DataIndex;
                discoveredPeakList.Add(pPeak);
            }

            return discoveredPeakList;
        }



        /// <summary>
        /// combines peak detection and thresholding
        /// </summary>
        /// <param name="currentData"></param>
        /// <returns></returns>
        public static List<ProcessedPeak> PeakCentroidOnly(List<PNNLOmics.Data.XYData> currentData, Run runGo, ParametersPeakDetection peakDetectionParameters)
        {
            ////2.  centroid peaks
            //PeakCentroider newPeakCentroider = new PeakCentroider();
            //newPeakCentroider.Parameters.FWHMPeakFitType = PeakFitType.Lorentzian;

            //List<ProcessedPeak> discoveredPeakList = new List<ProcessedPeak>();
            //discoveredPeakList = newPeakCentroider.DiscoverPeaks(currentData);

            ////3.  threshold
            //PeakThresholder newPeakThresholder = new PeakThresholder();
            //newPeakThresholder.Parameters.SignalToShoulderCuttoff = (float)parameters.Part1Parameters.MSPeakDetectorPeakBR;

            //discoveredPeakList = newPeakThresholder.ApplyThreshold(discoveredPeakList);

            runGo.XYData = new DeconTools.Backend.XYData();
            runGo.XYData.Xvalues = new double[currentData.Count];
            runGo.XYData.Yvalues = new double[currentData.Count];
            for (int i = 0; i < currentData.Count; i++)
            {
                runGo.XYData.Xvalues[i] = currentData[i].X;
                runGo.XYData.Yvalues[i] = currentData[i].Y;
            }

            ResultsPeakDetection peakObjectinsideThread = new ResultsPeakDetection();



            List<string> logAddition;

            //applb new parameters for centroid only
            ParametersPeakDetection centroidParameters = new ParametersPeakDetection();


            centroidParameters.PeakDetectionMethod = PeakDetectors.DeconTools;
            centroidParameters.DeconToolsPeakDetection.MsPeakDetectorPeakToBackground = 0;
            centroidParameters.DeconToolsPeakDetection.SignalToNoiseRatio = 0;

            GoPeakDetection.PeakProcessing(centroidParameters, runGo, peakObjectinsideThread, out logAddition);

            List<ProcessedPeak> discoveredPeakList = new List<ProcessedPeak>();
            foreach (DeconTools.Backend.Core.Peak dPeak in runGo.ResultCollection.Run.PeakList)
            {
                ProcessedPeak pPeak = new ProcessedPeak();
                pPeak.Height = dPeak.Height;
                pPeak.XValue = dPeak.XValue;
                pPeak.Width = dPeak.Width;
                pPeak.MinimaOfLowerMassIndex = dPeak.DataIndex;
                discoveredPeakList.Add(pPeak);
            }

            return discoveredPeakList;
        }

        #endregion
        
        #region IDisposable Members

        /// <summary>
        /// Simple dispose linker
        /// </summary>
        public void Dispose()
        {
            DisposeTandemObject();
        }

        /// <summary>
        /// Clean up memory
        /// </summary>
        private void DisposeTandemObject()
        {
            if (PrecursorScanPeaks != null)
            {
                PrecursorScanPeaks.Clear();
            }
            PrecursorScanPeaks = null;
            if (PeaksIsosResultsDECON.Count > 0)
                foreach (DeconTools.Backend.StandardIsosResult isos in PeaksIsosResultsDECON)
                {
                    isos.IsotopicProfile.Peaklist.Clear();
                    isos.IsotopicProfile = null;
                }
            PeaksIsosResultsDECON.Clear();
            PeaksIsosResultsDECON = null;

            PrecursorData = null;
            FragmentationData = null;

            //m_msgenFactory = null;
            //m_msGenerator = null;  //TODO check to see if this needs to be nulled
            DisposeRun();

            InputFileName = null;
        }

        #endregion
    }
}
