using System;
using System.Collections.Generic;
using System.Linq;
using DeconTools.Backend.Core;
using DeconTools.Backend.Runs;
using GetPeaks_DLL.TandemSupport;
using GetPeaks_DLL.Objects;
using PNNLOmics.Data;

namespace Parallel.THRASH
{
    public class EngineStationTHRASH : ParalellEngineStation
    {
        /// <summary>
        /// hold all the scans information for the run
        /// </summary>
        public DeconTools.Backend.Core.ScanSetCollection ScanSetCollection { get; set; }

        /// <summary>
        /// how many threads do we need in total.  A generic scan
        /// </summary>
        public int Iterations { get; set; }

        /// <summary>
        /// setup a scanset once
        /// </summary>
        /// <param name="parameters"></param>
        public EngineStationTHRASH(ParametersTHRASH parameters)
        {
            //string thermoFile1 = parameters.FileInforamation.InputFileName;//use the default file even for the multi threaded files

            string thermoFile1 = ParalellHardDrive.Select(false, parameters, 0);//false is fixed because the station should be set up on the core file
            
            Run run = new RunFactory().CreateRun(thermoFile1);

            //pull scan set out
            //var scanSetCollection = ScanSetCollection.Create(run, scanStart, scanStop, numScansSummed: 5, scanIncrement: 1, processMSMS: false);
            //var scanSetCollection = CreateStandardScanSetCollection(run, scanStart, scanStop, numScansSummed: 5, scanIncrement: 1, processMSMS: true);
            //ScanSets = (DeconTools.Backend.Core.ScanSetCollection)scanSetCollection;

            //Iterations = ScanSets.ScanSetList.Count;

            //this is where we can get the precursor infomration
            //DeconTools.Backend.Core.ScanSetCollection scottsScanSet = SetUpScanSet(parameters, run);
            var deconScanSet = ScanSetCollection.Create(run, parameters.ScansToSum, 1, parameters.ProcessMsms);

            Console.WriteLine("LoadingData for Station...");

            Iterations = deconScanSet.ScanSetList.Count;
            //ScanSetCollection = scottsScanSet;
            ScanSetCollection = deconScanSet;
        }

        private DeconTools.Backend.Core.ScanSetCollection SetUpScanSet(ParametersTHRASH parameters, Run run)
        {
            InputOutputFileName newFile = parameters.FileInforamation;
            int limitFileToThisManyScans = run.MaxScan;
            int sizeOfDatabase;
            List<PrecursorInfo> precursors;
            GatherDatasetInfo.GetMSLevelandSize(newFile, limitFileToThisManyScans, out sizeOfDatabase, out precursors);

            DeconTools.Backend.Core.ScanSetCollection scottsScanSet = CreateScottsScanSet(parameters, precursors);
            Iterations = scottsScanSet.ScanSetList.Count;
            Console.WriteLine("There are " + scottsScanSet.ScanSetList.Count() + " scan piles");
            return scottsScanSet;
        }

        private DeconTools.Backend.Core.ScanSetCollection CreateScottsScanSet(ParametersTHRASH parameters, List<PrecursorInfo> precursors)
        {
            DeconTools.Backend.Core.ScanSetCollection scottsScanSet = new ScanSetCollection();

            //which scans to include in the analysis
            List<PrecursorInfo> precursorsSelected = new List<PrecursorInfo>();
            switch (parameters.ProcessMsms)
            {
                case false:
                    {
                        precursorsSelected = precursors.Where(p => p.MSLevel.Equals(1)).ToList();
                    }
                    break;
                case true:
                    {
                        precursorsSelected = precursors;
                    }
                    break;
            }

            int scanCounter = 0;

            if (parameters.ScansToSum == 0)
            {
                parameters.ScansToSum = 1;
            }

            int scanHalf = 1;//how many scans on either side of primary scan
            if (parameters.ScansToSum % 2 == 0)
            {
                scanHalf = parameters.ScansToSum / 2;

            }
            else
            {
                scanHalf = (parameters.ScansToSum + 1) / 2;
            }


            while (scanCounter < precursorsSelected.Count)
            {
                ScanSet newScanSet = new ScanSet();
                newScanSet.IndexValues = new List<int>();
                newScanSet.BasePeak = new MSPeak();
                newScanSet.PrimaryScanNumber = precursorsSelected[scanCounter].PrecursorScan;

                for (int j = 0; j < scanHalf-1; j++)//-1 to exclude the middle
                {


                    if (scanCounter-j-1 >0)
                    {
                        newScanSet.IndexValues.Add(precursorsSelected[scanCounter-j-1].PrecursorScan);
                    }
                }

                newScanSet.IndexValues.Add(precursorsSelected[scanCounter].PrecursorScan);//the middle

                for (int j = 0; j < scanHalf-1; j++)//-1 to exclude the middle
                {


                    if (scanCounter+j+1 < precursorsSelected.Count)
                    {
                        newScanSet.IndexValues.Add(precursorsSelected[scanCounter+j+1].PrecursorScan);                     
                    }
                }

                newScanSet.IndexValues.Sort();
                scottsScanSet.ScanSetList.Add(newScanSet);
                scanCounter++;
            }
            return scottsScanSet;
        }
    }
}
