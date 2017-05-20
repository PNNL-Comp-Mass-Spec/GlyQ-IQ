using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using DeconTools.Backend.DTO;
using DeconTools.Backend;
using DeconTools.Backend.Runs;
using System.Threading;
using GetPeaks_DLL.CompareContrast;
using System.Diagnostics;
using GetPeaks_DLL.Go_Decon_Modules;


namespace GetPeaks_DLL
{
    /// <summary>
    /// this is a self contained decon tools part 2
    /// make sure XYData is loaded into run
    /// results end up in GOAccelerate m_run
    /// </summary>
    public class GoAccelerate: IDisposable
    {
        #region properties
        private int m_compareResults;
        private int m_processor;
        private int m_startIndex;
        private int m_stopIndex;
        private int m_scanNumber;
        private double m_crossTollerance;
        private ElutingPeak m_ElutingPeak;
        private Run m_Run;
        private SimpleWorkflowParameters m_parameters;
        public List<ElutingPeak> m_elutingPeakResults = new List<ElutingPeak>();
        public GoResults m_IsotopeResults;
        #endregion

        public void SetValues(Run eRun, ElutingPeak ePeak, int results, int processor, SimpleWorkflowParameters parameters, string fileName, int scanNumber)
        {
            #region copy run
            //RunFactory rf = new RunFactory();
            //Run runGo = rf.CreateRun(DeconTools.Backend.Globals.MSFileType.YAFMS, fileName);
                                        
            //runGo.AreRunResultsSerialized = eRun.AreRunResultsSerialized;
            //runGo.ContainsMSMSData = eRun.ContainsMSMSData;
            //runGo.CurrentMassTag = eRun.CurrentMassTag;
            //runGo.CurrentScanSet = eRun.CurrentScanSet;
            //runGo.DatasetName = eRun.DatasetName;
            //runGo.DataSetPath = eRun.DataSetPath;
            //runGo.DeconToolsPeakList = eRun.DeconToolsPeakList;
            //runGo.Filename = eRun.Filename;
            //runGo.IsDataThresholded = eRun.IsDataThresholded;
            //runGo.MaxScan = eRun.MaxScan;
            //runGo.MinScan = eRun.MinScan;
            //runGo.MSFileType = eRun.MSFileType;
            //runGo.MSLevelMappings = eRun.MSLevelMappings;
            //runGo.MSParameters = eRun.MSParameters;
            //runGo.PeakList = eRun.PeakList;
            //runGo.ResultCollection = eRun.ResultCollection;
            //runGo.ScanSetCollection = eRun.ScanSetCollection;
            //runGo.ScanToNETAlignmentData = eRun.ScanToNETAlignmentData;
            //runGo.XYData = eRun.XYData;
            #endregion

            //m_Run = runGo;
            m_Run = eRun;
            m_ElutingPeak = ePeak;
            m_compareResults = results;
            m_processor = processor;
            m_startIndex = ePeak.ScanStart;
            m_stopIndex = ePeak.ScanEnd;
            m_crossTollerance = parameters.ConsistancyCrossErrorPPM;
            m_parameters = parameters;
            m_scanNumber = scanNumber;//perhaps this should be held by the decontools part 1 that goes with it
            m_IsotopeResults = new GoResults();

        }

        //static object myLockObjectPileupData = new Object(); 

        public void GO2DeconTools()
        {
            //TODO:  check XY data here as well
            
            //Console.WriteLine("Go2 started.....");
            //Console.WriteLine(m_ElutingPeak.Mass.ToString() +" thread");

//            GC.Collect();
//            printMemory("start  GO2DeconTools");

            //GoDeconToolsControllerB newDeconToolsPart2 = new GoDeconToolsControllerB(m_Run, m_parameters);
            GoDeconToolsControllerB newDeconToolsPart2 = new GoDeconToolsControllerB(m_parameters, m_Run.MSFileType);

 //           GC.Collect();
 //           printMemory("new  newDeconToolsPart2");

            //Monitor.Enter(myLockObjectPileupData);
            ///input: 
 //TODO 3-26           newDeconToolsPart2.GODeconToolsFX(m_Run, m_parameters);
            //Monitor.Exit(myLockObjectPileupData);

 //           GC.Collect();
//            printMemory("Fat  newDeconToolsPart2");

            //Monitor.Enter(myLockObjectPileupData);
            #region save Isos results into eluting peak and clear data
            foreach (StandardIsosResult isosResult in m_Run.ResultCollection.IsosResultBin)
            //for(int i=0;i<run.ResultCollection.IsosResultBin.Count;i++)
            {
                //m_ElutingPeak.IsosResultList.Add(isosResult);
                m_IsotopeResults.IsosResultList.Add(isosResult);
            }
            m_Run.ResultCollection.IsosResultBin.Clear();//this may not be needed if we are going to nuke the run
            m_Run.ResultCollection.IsosResultBin = null;
            m_Run.ResultCollection.IsosResultBin = new List<IsosResult>();//so the dispose works
            
            //Monitor.Exit(myLockObject);      // Release the lock.
            #endregion

            #region copy peaks to eluting peak so we have the data to work with later
            //TODO:  still looking for peak data.  It is likelt not here because we are looking in the other dimention
            //we want the eluting peaks vs scan.
            //this may go up one level
            #endregion

            #region cross correlate the eluting peak with the monoisotopic peaks.
            //pull out to function

            //deal with the isotope info
            double high = 0;
            double low = 0;
            double peakMZ = 0;
            float peakIntensity = 0;

            //foreach (IsosResult isoresult in m_ElutingPeak.IsosResultList)
            foreach (IsosResult isoresult in m_IsotopeResults.IsosResultList)
            {
                peakMZ = isoresult.IsotopicProfile.MonoPeakMZ;
                peakIntensity = (float)isoresult.IsotopicProfile.IntensityAggregateAdjusted;
                double daltonDeltaX = CompareContrast2_Old.ErrorCalculation(peakMZ, m_crossTollerance, GetPeaks_DLL.CompareContrast.CompareContrast2_Old.ErrorType.PPM);
                high = peakMZ + daltonDeltaX;
                low = peakMZ - daltonDeltaX;

                //there are 2 ways to get assigned.  if true, the principle mass is assgined
                //else true, monisiotopic mass may be in the rest of the eluting peak list and can be assigned here.  I expect this is faster but it may not be.  could just let it go through the loop
                if (m_ElutingPeak.Mass <= high && m_ElutingPeak.Mass >= low)
                {
                    m_ElutingPeak.RetentionTime = (float)(peakMZ - m_ElutingPeak.Mass);
                    m_ElutingPeak.Mass = peakMZ;
                    m_ElutingPeak.Intensity = peakIntensity;
                    m_ElutingPeak.ID = 0;
                    //m_ElutingPeak.IsosResultList = new List<StandardIsosResult>();//zero out list since we found our match//not good because we will do post procvessing on this information
                    //m_ElutingPeak.PeakList = new List<MSPeakResult>();//Why do we want to delete the eluting peak list
                }
                else
                {
                    //there is not else because there is only one monoisitopic peak optimized for
                }

                //keep these with us since we can use the data later for LC deconvolution and peak summing
                //m_ElutingPeak.IsosResultList = new List<StandardIsosResult>();//zero out list since we found our match
                //m_ElutingPeak.PeakList = new List<MSPeakResult>();

                if(high>m_ElutingPeak.Mass)//break since we have either found the monoisotopic peak or passed it
                {
                    break;
                }
            }
            #endregion

            #region  Then search for charge states and adducts in future
            if (m_ElutingPeak.ID == 0)//aka hit?
            {
                //foreach (IsosResult isoresult in m_ElutingPeak.IsosResultList)
                foreach (IsosResult isoresult in m_IsotopeResults.IsosResultList)
                {
                    //TODO :  if isos result = charge +2, +3 +X or +Na, +K
                    //do something
                }
            }

            #endregion

            m_elutingPeakResults.Add(m_ElutingPeak);
            
            string name = "processor " + Convert.ToString(m_processor);
            //Console.WriteLine("end go");

//            GC.Collect();
//            printMemory("Before newDeconToolsPart2.Dispose");

            m_Run.Dispose();
            newDeconToolsPart2.Dispose();//does nothing because run is still rooted

//           GC.Collect();
//           printMemory("After  newDeconToolsPart2 disposed");
        }

        public void Dispose()
        {
            m_Run = null;
            m_ElutingPeak = null;
            m_parameters = null;            
            m_elutingPeakResults = null;
        }

        public void printMemory(string printline)
        {
            string procName = Process.GetCurrentProcess().ProcessName;
            Process newProcess = Process.GetCurrentProcess();

            double printValue = 0;
            long memory = 0;
            memory = newProcess.PrivateMemorySize64;
            printValue = (double)memory / 1000000;
            Console.WriteLine("     PrivateMemorySize64 " + printValue.ToString() + " MB at " + printline);
            
        }
    }
}
