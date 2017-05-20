using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using DeconTools.Backend.Core;
using GetPeaks_DLL.Go_Decon_Modules;
using DeconTools.Backend.Utilities;
using DeconTools.Backend.ProcessingTasks;
using GetPeaks_DLL.Functions;
using GetPeaks_DLL.Objects.TandemMSObjects;
using System.Text.RegularExpressions;












//This is old!










namespace GetPeaks_DLL.DataFIFO
{
    public class LoadTandemData:IDisposable
    {
        #region Properties

        
        //public MSGeneratorFactory msgenFactory { get; set; }
        

        #endregion

        public LoadTandemData()
        {
           //msgenFactory = new MSGeneratorFactory();     
        }

        public List<TandemObject> LoadData(string fileName, int sizeOfDatabase, int precursorScan, ParametersTHRASH parametersThrash)
        {
            ///TODO: switch this to a scan instead of a scan range.
            ///1.  the scan must be a precursor scan.  perhaps write a check for this
            ///once we are in, go from the start scan till the next precursor scan
            ///create a scanset for that range
            ///load data and store it in the object
            ///create a deconstructor



            #region initialize variables

            int scansToSum = 1;//1 since we can't really sum tandem data simply
            int nextPrecursorScan = 0;//the scan number of the next precursor scan.  used for stopping loops

            //perhaps bring in a run so we don't waste time makeing a new run each time
            InputOutputFileName newInputFileName = new InputOutputFileName();
            newInputFileName.InputFileName = fileName;

            List<TandemObject> tandemObjectList = new List<TandemObject>();
            

            Run run = GoCreateRun.CreateRun(newInputFileName);
            ///Task msGenerator = msgenFactory.CreateMSGenerator(run.MSFileType);

            #endregion

            #region verify the scan chosen is a precursor scan and find nextPrecursorScan

            Console.WriteLine("checking precursor scan");

            run.MinLCScan = precursorScan;
            run.MaxLCScan = (precursorScan+1)*99999;//end of file

            try
            {
                bool GetMSMSDataAlso = false;


                run.ScanSetCollection.Create(run, run.MinLCScan, run.MaxLCScan, scansToSum, 1, GetMSMSDataAlso);
                //run.ScanSetCollection = ScanSetCollection.Create(run, run.MinScan, run.MaxScan, scansToSum, 1, GetMSMSDataAlso);
                Console.WriteLine("LoadingData...");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            List<int> scansListCheck;
            ConvertDeconScansetCollection.ToList(run, out scansListCheck);

            bool checkIfprecursorScan = false;

            checkIfprecursorScan = scansListCheck.Contains(precursorScan);

            if (!checkIfprecursorScan)
            {
                Console.WriteLine("the entered scan number is not a precursor!");
            }

            nextPrecursorScan = scansListCheck.IndexOf(precursorScan);
            nextPrecursorScan = scansListCheck[nextPrecursorScan+1];

            #endregion

            //check to see if there are any tandem scans for this precursor scan
            if (nextPrecursorScan > 1 + precursorScan)//this needs to be true or the next scan is a seperate precursor
            {
                #region scan through fragmentaiton scans

                ///figures out all the important info about the scans associated with the precursor scan
                List<TandemObjectMassAndScan> fragmentationMetaData = PopulateFragmentationMetaData(run, precursorScan, nextPrecursorScan);


                #endregion

                #region preprocess precursor data so we only have to do this once

                TandemObject tandemObjectPrecursor = new TandemObject(newInputFileName, precursorScan, parametersThrash);
                tandemObjectPrecursor.m_run = run;
                tandemObjectPrecursor.LoadPrecursorData();
                tandemObjectPrecursor.PrecursorScanNumber = precursorScan;
                //tandemObjectPrecursor.Parameters = new SimpleWorkflowParameters();//this is needed to get to peaks and masses in prucorsor data
                tandemObjectPrecursor.PeakPickPrecursorData();  


                #endregion
                #region spawn of return list of TandemData for this precursor scan
                foreach (TandemObjectMassAndScan metaData in fragmentationMetaData)
                {
                    //for each tandem scan.  Load data and refine the mass

                    TandemObject tandemObject = new TandemObject(newInputFileName, precursorScan, parametersThrash);
                    tandemObject.PrecursorScanNumber = precursorScan;
                    tandemObject.PrecursorMZ = metaData.PrecursorMass;
                    tandemObject.FragmentationMSLevel = metaData.MSLevel;
                    tandemObject.FragmentationScanNumber = metaData.ScanNumber;//for this tandem scan number

                    tandemObjectList.Add(tandemObject);
                    
                    //populate data
                    #region Get precursorscan data and convert to peaks

                    tandemObject.m_run = run;
                    //tandemObject.LoadPrecursorData();
                    tandemObject.LoadFragmentationData();
                    //tandemObject.PeakPickPrecursorData();

                    //bring in data from precursor so we can identify the correct peak
                    //tandemObjectPrecursor.PrecursorScanNumber = precursorScan;
                    //tandemObject.m_run.CurrentScanSet = new ScanSet(precursorScan);
                    //tandemObject.m_run.ScanSetCollection.ScanSetList[0].PrimaryScanNumber = tandemObject.FragmentationScanNumber;
                    tandemObject.PrecursorScanPeaks = tandemObjectPrecursor.PrecursorScanPeaks;
                    //tandemObject.LoadPrecursorMasses();//must follow PeakPickPrecursorData

                    tandemObject.LoadPrecursorMass();//must follow PeakPickPrecursorData
                    //tandemObject.DisposeRun();

                    #endregion
                }

                foreach (TandemObject tandem in tandemObjectList)
                {
                    tandem.DisposeRun();
                }

                fragmentationMetaData = null;

                #endregion

            }
            else
            {
                Console.WriteLine("the entered scan number has no tandem data!");

                TandemObject tandemObject = new TandemObject(newInputFileName, precursorScan, parametersThrash);
                tandemObject.PrecursorScanNumber = precursorScan;
                tandemObjectList.Add(tandemObject);

            }

            #region cleanup
            run.Dispose();
            //msGenerator = null;  TODO Check to see if this needs to be deleted
            scansListCheck = null;
            
            
            #endregion

            return tandemObjectList;
        }

        private static List<TandemObjectMassAndScan> PopulateFragmentationMetaData(Run run, int precursorScan, int nextPrecursorScan)
        {
            List<TandemObjectMassAndScan> fragmentationInfoList = new List<TandemObjectMassAndScan>();

            if (nextPrecursorScan > 1 + precursorScan)//this needs to be true or the next scan is a seperate precursor
            {
                int scansToSum = 1;//1 since we can't really sum tandem data simply
                

                ///create scanset for fragmentation runs
                run.MinLCScan = precursorScan + 1;//this needs to be checked
                run.MaxLCScan = nextPrecursorScan - 1;//ths needs to be checked

                try
                {
                    bool GetMSMSDataAlso = true;

                    run.ScanSetCollection.Create(run, run.MaxLCScan, run.MaxLCScan, scansToSum, 1, GetMSMSDataAlso);

                    //run.ScanSetCollection = ScanSetCollection.Create(run, run.MinScan, run.MaxScan, scansToSum, 1, GetMSMSDataAlso);
                    //Console.WriteLine("LoadingData3...");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                
                int scanIndex = -1;//allign scansetListwithScanNumber

                for (int i = 0; i < run.ScanSetCollection.ScanSetList.Count; i++)
                {
                    scanIndex = i;

                    //ScanSet scan = ;

                    TandemObjectMassAndScan newMetaData = new TandemObjectMassAndScan();

                    newMetaData.ScanNumber = run.ScanSetCollection.ScanSetList[scanIndex].PrimaryScanNumber;
                    newMetaData.MSLevel = run.GetMSLevel(run.MinLCScan);

                    fragmentationInfoList.Add(newMetaData);
                }

                if (scanIndex < 0)
                {
                    Console.WriteLine("failed to find precursor spectra in scan list");
                    Console.ReadKey();
                }
            }
            else
            {
                TandemObjectMassAndScan newMetaData = new TandemObjectMassAndScan();
                newMetaData.MSLevel = 1;
                newMetaData.ScanNumber = precursorScan;

                fragmentationInfoList.Add(newMetaData);

            }
            return fragmentationInfoList;
        }

        #region IDisposable Members

        public void Dispose()
        {
            //msgenFactory = null;
        }

        #endregion
    }
}
