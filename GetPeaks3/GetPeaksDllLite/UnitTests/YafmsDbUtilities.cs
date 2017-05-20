using System.Collections.Generic;
using GetPeaksDllLite.Functions;
using GetPeaksDllLite.Objects;
using GetPeaksDllLite.Objects.TandemMSObjects;
using GetPeaksDllLite.TandemSupport;
using GetPeaks_DLL.Functions;
using PNNLOmics.Data;
using PNNLOmics.Data.Peaks;
using PrecursorInfo = Run64.Backend.Data.PrecursorInfo;
using XYData = Run64.Backend.Data.XYData;

namespace GetPeaksDllLite.UnitTests
{
    public static class YafmsDbUtilities
    {
        public static void ProcessScanSpectraNumbers(InputOutputFileName newFile, int limitFileToThisManyScans, out List<PrecursorInfo> precursors, out List<int> scanMSLevelList, out List<int> Ms1ScansWithTandem)
        {
            int sizeOfDatabase;
            //int limitFileToThisManyScans = 30000;
            //int limitFileToThisManyScans = 3000;
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

            scanMSLevelList = new List<int>();
            if (areTandemDetected)
            {
                foreach (PrecursorInfo parent in precursors)
                {
                    scanMSLevelList.Add(parent.MSLevel);
                }
                Ms1ScansWithTandem = SelectScans.Ms1PrecursorScansWithTandem(scanMSLevelList);
            }
            else
            {
                Ms1ScansWithTandem = new List<int>();
            }
        }

        public static void SetLocationInDataset(ref int idCounterForScan, ref ScanCentric currentLocationInDataset, int msLevel, int parentScan, int tandemScan)
        {
            currentLocationInDataset.ScanID = idCounterForScan;
            currentLocationInDataset.MsLevel = msLevel;
            currentLocationInDataset.ParentScanNumber = parentScan;
            currentLocationInDataset.TandemScanNumber = tandemScan;
            idCounterForScan++;
        }

        public static ScanCentric SetLocationInDataset(ref int idCounterForScan, int msLevel, int parentScan, int tandemScan)
        {
            ScanCentric currentLocationInDataset = new ScanCentric();
            currentLocationInDataset.ScanID = idCounterForScan;
            currentLocationInDataset.MsLevel = msLevel;
            currentLocationInDataset.ParentScanNumber = parentScan;
            currentLocationInDataset.TandemScanNumber = tandemScan;
            idCounterForScan++;

            return currentLocationInDataset;
        }

        ////private static void SaveProcessedPeaksAtCentroidLevel(List<ProcessedPeak> centroidedPeaksHold, int scan, ref int IdCounterForPeakCentric, ref FragmentCentric currentLocationInDataset, out List<PeakCentric> centroidedPeaksToWrite, EngineSQLite currentSQLiteEngine, ParametersSQLite sqliteDetails)
        //public static void SaveProcessedPeaksAtCentroidLevel(MSSpectra spectra, int scan, ref int IdCounterForPeakCentric, ref ScanCentric currentLocationInDataset, out List<PeakCentric> centroidedProcessedPeaksToWrite, out List<PeakCentric> thresholdedProcessedPeaksToWrite, EngineSQLite currentSQLiteEngine, ParametersSQLite sqliteDetails)
        //{
        //    //RemapRegion
        //    //List<AttributeCentric> centroidedAttributesToWrite;
        //    List<ScanCentric> centroidedScansToWrite;
        //    //List<PeakCentric> centroidedPeaksToWrite;above
        //    //List<FragmentCentric> centroidedFragmentsToWrite;//not implemented

        //    //write simple peaks to Peaks-Scan
        //    List<XYData> simplePeaksHold = spectra.Peaks;
        //    RemapXYPeaksToCentricForWrite(simplePeaksHold, scan, ref IdCounterForPeakCentric, ref currentLocationInDataset, out centroidedProcessedPeaksToWrite, out centroidedScansToWrite);

        //    //only peaks table new
        //    Write_ScanPeak_ToDatabase(currentSQLiteEngine, centroidedProcessedPeaksToWrite, sqliteDetails);


        //    //write processed to database
        //    List<ProcessedPeak> centroidedPeaksHold = spectra.PeaksProcessed;
        //    RemapProcessedPeaksToCentricForWrite(centroidedPeaksHold, scan, ref IdCounterForPeakCentric, ref currentLocationInDataset, out thresholdedProcessedPeaksToWrite, out centroidedScansToWrite);

        //    //normal peak centric 5-18-2013
        //    Write_PeakCentric_ToDatabase(currentSQLiteEngine, thresholdedProcessedPeaksToWrite, sqliteDetails);

            

        //    //WriteAttributeCentricToDatabase(currentSQLiteEngine, centroidedAttributesToWrite, sqliteDetails);

        //    WriteScanCentricToDatabase(currentSQLiteEngine, centroidedScansToWrite, sqliteDetails);
        //    //WriteFragmentCentricToDatabase(currentSQLiteEngine, centroidedFragmentsToWrite, sqliteDetails);
        //}

        //public static void WriteIndexesToDatabases(EngineSQLite currentSQLiteEngine, ParametersSQLite sqliteDetails, int pageNumber)
        //{
        //    //sqliteDetails.PageName = sqliteDetails.ColumnHeadersCounts[4];//4 PeakCentric
        //    sqliteDetails.PageName = sqliteDetails.ColumnHeadersCounts[pageNumber];
        //    List<string> indexes = sqliteDetails.Indexes[pageNumber];
        //    //DatabaseTransferObject getIndexes = new DatabasePeakCentricLiteObject();
        //   // List<string> indexes = getIndexes.IndexedColumns;


        //    DatabaseLayer m_DatabaseLayer = new DatabaseLayer();
        //    string fileName = sqliteDetails.FileInforamation.OutputPath + sqliteDetails.FileInforamation.OutputSQLFileName + "_(0).db";

        //    bool didThisWorkFeature = m_DatabaseLayer.WriteDataTransferObjectIndex(indexes, sqliteDetails.PageName, fileName);

        //}

        //private static void RemapIsotpeInforIntoCentricFormat(List<IsotopeObject> precursorResultsOut, ref int IdCounterForMonoClusters, double massProton, out List<PeakCentric> monoResults, out List<AttributeCentric> monoAtributres)
        private static void RemapIsotpeInforIntoCentricFormat(List<IsotopeObject> precursorResultsOut, ref int IdCounterForMonoClusters, double massProton, out List<PeakCentric> monoResults)
        {
            monoResults = new List<PeakCentric>();
            //monoAtributres = new List<AttributeCentric>();

            foreach (IsotopeObject isotopeObject in precursorResultsOut)
            {
                int currentIsotope = IdCounterForMonoClusters;

                for (int j = 0; j < isotopeObject.IsotopeList.Count; j++)
                {
                    PNNLOmics.Data.Peak currentPeakInIsotope = isotopeObject.IsotopeList[j]; //for each

                    PeakCentric newPeak = new PeakCentric();
                    //AttributeCentric newAttribute = new AttributeCentric();

                    newPeak.MonoisotopicClusterID = currentIsotope;
                    newPeak.Score = isotopeObject.FitScore;
                    newPeak.ChargeState = isotopeObject.Charge;

                    newPeak.isSignal = true;
                    newPeak.isCentroided = true;
                    newPeak.isCharged = true;

                    MergePeakIntoPeakCentric(ref newPeak, currentPeakInIsotope);

                    //newAttribute.isSignal = true;
                    //newAttribute.isCentroided = true;
                    //newAttribute.isCharged = true;

                    if (j == 0) //dealing with monoisootpic mass
                    {
                        newPeak.MassMonoisotopic = isotopeObject.MonoIsotopicMass;
                        newPeak.isMonoisotopic = true;
                        //newAttribute.isMonoisotopic = true;
                    }
                    else //isotope
                    {
                        newPeak.MassMonoisotopic = ConvertMzToMono.Execute(currentPeakInIsotope.XValue, isotopeObject.Charge, massProton);
                        newPeak.isIsotope = true;
                        //newAttribute.isIsotope = true;
                    }

                    monoResults.Add(newPeak);
                    //monoAtributres.Add(newAttribute);
                }

                IdCounterForMonoClusters++;//next cluster
            }
        }

        //private static void RemapPeaksToCentricForWrite(List<ProcessedPeak> centroidedPeaksHold, int scan, ref int IdCounterForPeakCentric, ref FragmentCentric currentLocationInDataset, out List<PeakCentric> centroidedPeaksToWrite, out List<AttributeCentric> centroidedAttributesToWrite, out List<ScanCentric> centroidedScansToWrite, out List<FragmentCentric> centroidedFragmentsToWrite)
        private static void RemapProcessedPeaksToCentricForWrite(List<ProcessedPeak> centroidedPeaksHold, int scan, ref int IdCounterForPeakCentric, ref ScanCentric currentLocationInDataset, out List<PeakCentric> centroidedPeaksToWrite, out List<ScanCentric> centroidedScansToWrite)
        {
            centroidedPeaksToWrite = new List<PeakCentric>();
            //centroidedAttributesToWrite = new List<AttributeCentric>();
            centroidedScansToWrite = new List<ScanCentric>();
            //centroidedFragmentsToWrite = new List<FragmentCentric>();

            //FragmentCentric newFragmentInfo = new FragmentCentric();
            //newFragmentInfo.ScanID = currentLocationInDataset.ScanID;
            //newFragmentInfo.MsLevel = currentLocationInDataset.MsLevel;
            //newFragmentInfo.ParentScanNumber = currentLocationInDataset.ParentScanNumber;
            //newFragmentInfo.TandemScanNumber = currentLocationInDataset.TandemScanNumber;

            ScanCentric newScaninfo = new ScanCentric();
            newScaninfo.ScanID = currentLocationInDataset.ScanID;
            newScaninfo.ScanNumLc = scan;

            newScaninfo.MsLevel = currentLocationInDataset.MsLevel;
            newScaninfo.ParentScanNumber = currentLocationInDataset.ParentScanNumber;
            newScaninfo.TandemScanNumber = currentLocationInDataset.TandemScanNumber;

            foreach (ProcessedPeak peak in centroidedPeaksHold)
            {
                PeakCentric newPeak = ConvertMSPeakToPeakCentric.Convert(peak);
                newPeak.PeakID = IdCounterForPeakCentric;
                newPeak.ScanID = currentLocationInDataset.ScanID;

                newPeak.isCentroided = true;
                newPeak.isCharged = true;
                newPeak.isSignal = true;
                
                //FragmentCentric newFragmentInfo = new FragmentCentric();
                //newFragmentInfo.ScanID = idCounterForScan;
                //newFragmentInfo.MsLevel = msLevel;

                IdCounterForPeakCentric++;

                centroidedPeaksToWrite.Add(newPeak);
            }

            centroidedScansToWrite.Add(newScaninfo);
            //centroidedFragmentsToWrite.Add(newFragmentInfo);
        }

        private static void RemapXYPeaksToCentricForWrite(List<PNNLOmics.Data.XYData> simplePeaksHold, int scan, ref int IdCounterForPeakCentric, ref ScanCentric currentLocationInDataset, out List<PeakCentric> centroidedPeaksToWrite, out List<ScanCentric> centroidedScansToWrite)
        {
            centroidedPeaksToWrite = new List<PeakCentric>();
            centroidedScansToWrite = new List<ScanCentric>();

            ScanCentric newScaninfo = new ScanCentric();
            newScaninfo.ScanID = currentLocationInDataset.ScanID;
            newScaninfo.ScanNumLc = scan;

            newScaninfo.MsLevel = currentLocationInDataset.MsLevel;
            newScaninfo.ParentScanNumber = currentLocationInDataset.ParentScanNumber;
            newScaninfo.TandemScanNumber = currentLocationInDataset.TandemScanNumber;

            foreach (var peak in simplePeaksHold)
            {
                PeakCentric newPeak = new PeakCentric();
                newPeak.Mz = peak.X;
                newPeak.Height = peak.Y;

                newPeak.PeakID = IdCounterForPeakCentric;
                newPeak.ScanID = currentLocationInDataset.ScanID;

                newPeak.isCentroided = true;
                newPeak.isCharged = true;
                
                IdCounterForPeakCentric++;

                centroidedPeaksToWrite.Add(newPeak);
              
            }
            centroidedScansToWrite.Add(newScaninfo);
        }


        ////5-18-2013
        //private static void Write_PeakCentric_ToDatabase(EngineSQLite currentEngine, List<PeakCentric> peakList, ParametersSQLite sqliteDetails)
        //{
        //    List<PeakCentric> currentList = peakList;
        //    if (peakList.Count > 0)
        //    {
        //        sqliteDetails.PageName = sqliteDetails.ColumnHeadersCounts[4];//4 PeakCentric

        //        bool didthisworkTandem = currentEngine.WritePeakCentricList(currentEngine, currentList, sqliteDetails);
        //    }
        //}


        //private static void Write_ScanPeak_ToDatabase(EngineSQLite currentEngine, List<PeakCentric> peakList, ParametersSQLite sqliteDetails)
        //{
        //    List<PeakCentric> currentList = peakList;
        //    if (peakList.Count > 0)
        //    {
        //        sqliteDetails.PageName = sqliteDetails.ColumnHeadersCounts[0];//0 Scan Peaks

        //        bool didthisworkTandem = currentEngine.Write_XYData_List(currentEngine, currentList, sqliteDetails);
        //    }
        //}

        //private static void WriteScanCentricToDatabase(EngineSQLite currentEngine, List<ScanCentric> peakList, ParametersSQLite sqliteDetails)
        //{
        //    List<ScanCentric> currentList = peakList;
        //    if (peakList.Count > 0)
        //    {
        //        sqliteDetails.PageName = sqliteDetails.ColumnHeadersCounts[5];//5 ScanCentric 
        //        bool didthisworkTandem = currentEngine.WriteScanCentricList(currentEngine, currentList, sqliteDetails);
        //    }
        //}



        /// <summary>
        /// like a convert. could be pulled out
        /// </summary>
        /// <param name="newPeak"></param>
        /// <param name="currentPeakInIsotope"></param>
        private static void MergePeakIntoPeakCentric(ref PeakCentric newPeak, Peak currentPeakInIsotope)
        {
            newPeak.Mass = currentPeakInIsotope.XValue;
            newPeak.Height = currentPeakInIsotope.Height;
            newPeak.Width = currentPeakInIsotope.Width;
            newPeak.LocalSignalToNoise = currentPeakInIsotope.LocalSignalToNoise;
            newPeak.Background = currentPeakInIsotope.Background;
        }


        //public static void SetUpSimplePeaksPage(ref ParametersSQLite sqliteDetails)
        //{
        //    sqliteDetails.PageName = "T_Scan_Peaks";//peaks
        //    sqliteDetails.ColumnHeadersCounts.Add("T_Scan_Peaks");
        //    List<string> indexes = new List<string>();
        //    //indexes.Add("Mz");//for EIC
        //    //indexes.Add("ScanID");//for Mass spectra
        //    indexes.Add("Mz,ScanID,Height");//for EIC  much faster!
        //    indexes.Add("ScanID,Mz,Height");//for Mass spectra  //much faster
        //    sqliteDetails.Indexes.Add(indexes);
        //}

        //public static void SetUpScansRelationshipPage(ref ParametersSQLite sqliteDetails)
        //{
        //    sqliteDetails.PageName = "T_Scans";//relationships
        //    sqliteDetails.ColumnHeadersCounts.Add("T_Scans");
        //    List<string> indexes = new List<string>();
        //    //indexes.Add("Mz");//for EIC
        //    //indexes.Add("ScanID");//for Mass spectra
        //    sqliteDetails.Indexes.Add(indexes);
        //}

        //public static void SetUpPrecursorPeakPage(ref ParametersSQLite sqliteDetails)
        //{
        //    sqliteDetails.PageName = "T_Scans_Precursor_Peaks";//precursor Peaks
        //    sqliteDetails.ColumnHeadersCounts.Add("T_Scans_Precursor_Peaks");
        //    List<string> indexes = new List<string>();
        //    //indexes.Add("Mz");//for EIC
        //    //indexes.Add("ScanID");//for Mass spectra
        //    sqliteDetails.Indexes.Add(indexes);
        //}

        //public static void SetUpMonoisotopicMassPeaksPage(ref ParametersSQLite sqliteDetails)
        //{
        //    sqliteDetails.PageName = "T_Scan_MonoPeaks";//mono peaks only
        //    sqliteDetails.ColumnHeadersCounts.Add("T_Scan_MonoPeaks");
        //    List<string> indexes = new List<string>();
        //    //indexes.Add("Mz");//for EIC
        //    //indexes.Add("ScanID");//for Mass spectra
        //    sqliteDetails.Indexes.Add(indexes);
        //}

        //public static void SetUpPeaksCentricPage(ref ParametersSQLite sqliteDetails)
        //{
        //    sqliteDetails.PageName = "T_Peak_Centric";//mono peaks only
        //    sqliteDetails.ColumnHeadersCounts.Add("T_Peak_Centric");
        //    List<string> indexes = new List<string>();
        //    //indexes.Add("Mz");//for EIC
        //    //indexes.Add("ScanID");//for Mass spectra
        //    indexes.Add("Mz,ScanID,Height");//for EIC  much faster!
        //    indexes.Add("ScanID,Mz,Height");//for Mass spectra  //much faster
        //    sqliteDetails.Indexes.Add(indexes);
        //}

        //public static void SetUpScanCentricPage(ref ParametersSQLite sqliteDetails)
        //{
        //    sqliteDetails.PageName = "T_Scan_Centric";//mono peaks only
        //    sqliteDetails.ColumnHeadersCounts.Add("T_Scan_Centric");
        //    List<string> indexes = new List<string>();
        //    //indexes.Add("Mz");//for EIC
        //    //indexes.Add("ScanID");//for Mass spectra
        //    sqliteDetails.Indexes.Add(indexes);
        //}
    }
}
