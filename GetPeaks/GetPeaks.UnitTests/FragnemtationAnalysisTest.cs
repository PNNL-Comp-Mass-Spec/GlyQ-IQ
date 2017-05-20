using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.SQLite.OneLineCalls;
using GetPeaks_DLL.SQLite.DataTransferObjects;
using PNNLOmics.Data.Constants;
using GlycolyzerGUImvvm.ViewModels;
using GlycolyzerGUImvvm.Models;
using GetPeaks_DLL.Objects.ParameterObjects;
using GetPeaks_DLL.Glycolyzer;
using CompareContrastDLL;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.Objects.ResultsObjects;
using OmniFinder.Objects;
using GetPeaks_DLL.Glycolyzer.Enumerations;
using PNNLOmics.Data;
using GetPeaks_DLL.Functions;
using GetPeaks_DLL.AnalysisSupport;
using OmniFinder;
using System.Diagnostics;
using GetPeaks_DLL.TandemSupport;
using YAFMS_DB.GetPeaks;

namespace GetPeaks.UnitTests
{
    public static class FragnemtationAnalysisTest
    {
        //public static List<DatabasePeakProcessedWithMZObject> AddChargeStates(List<DatabasePeakProcessedObject> peaks )
        //{
        //    List<DatabasePeakProcessedWithMZObject> results = new List<DatabasePeakProcessedWithMZObject>();
        //    //TODO the other way is to iterate over all the charge states

        //    foreach (DatabasePeakProcessedObject peak in peaks)
        //    {
        //        DatabasePeakProcessedWithMZObject newPoint = new DatabasePeakProcessedWithMZObject();
                
        //        newPoint.Height = peak.Height;
        //        newPoint.Width = peak.Width;
        //        newPoint.XValue = peak.XValue;

        //        newPoint.Background = peak.Background;
        //        newPoint.LocalLowestMinimaHeight = peak.LocalLowestMinimaHeight;
        //        newPoint.LocalSignalToNoise = peak.LocalSignalToNoise;
        //        newPoint.PeakNumber = peak.PeakNumber;
        //        newPoint.ScanNumberTandem = peak.ScanNum;
        //        newPoint.SignalToBackground = peak.SignalToBackground;
        //        newPoint.SignalToNoiseGlobal = peak.SignalToNoiseGlobal;
        //        newPoint.SignalToNoiseLocalMinima = peak.SignalToNoiseLocalMinima;
               
        //        results.Add(newPoint);
        //    }
        //    return results;
        //}

        //public static void Run(string fileName, System.DateTime starttime, Stopwatch stopWatch, string outputLocation)
        public static void Run(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            System.DateTime starttime = DateTime.Now;
            
            double masssProton = Constants.SubAtomicParticles[SubAtomicParticleName.Proton].MassMonoIsotopic;
            
            //crack args
            string outputLocation;
            string outputSipperFileLocation;
            string fileName;
            string resutsFolder;
            string resultsFileName;
            CrackArgsDBAnalysis(args, out outputLocation, out outputSipperFileLocation, out fileName, out resutsFolder, out resultsFileName);

            Console.WriteLine("Args" + Environment.NewLine);
            Console.WriteLine("outputLocation:  " + outputLocation);
            Console.WriteLine("outputSipperFileLocation:  " + outputSipperFileLocation);
            Console.WriteLine("fileName:  " + fileName);
            Console.WriteLine("resutsFolder:  " + resutsFolder);
            Console.WriteLine("resultsFileName:  " + resultsFileName);

            //Console.ReadKey();

            bool useDiagnosticIons = false;
            bool useChargedMasses = false;
            //List<int> resultsTandemScans = GetAllFragmentationScanNumbers.Read(fileName);

            Console.WriteLine("1.  Gathering Scan Information...");
            List<DatabaseScanCentricObject> resultsScanTable = GetAllScanNumbers.ReadAllFromDatabase(fileName);

            List<int> resultsScans = GetAllScanNumbers.ReadScanNumbersFromMemory(resultsScanTable);//.FirstOrDefault(c => c > testValue - tolerance & c < testValue + tolerance);

            List<int> resultsPrecursorPeakIds = GetAllScanNumbers.ReadPrecursorPeakNumbersFromMemory(resultsScanTable);

            Console.WriteLine(resultsPrecursorPeakIds.Count);

            List<TandemAnalysisResultsObject> overallMS1Results = new List<TandemAnalysisResultsObject>();
            List<TandemAnalysisResultPairObject> overallMS2Results = new List<TandemAnalysisResultPairObject>();

            bool runAsMono = true;//true will use peak.PeakCentricData.MassMonoisotopic and false will use peak.PeakCentricData.Mz


            List<string> monosaccharideHeaders = new List<string>();

            //This will iterate over all scans
            for (int i = 0; i < resultsScans.Count; i++)//this will iterate over all scans//3240 will hit parent libraries with 3244 hitting tandem
            
            //for (int i = 500; i < 525; i++)//this will iterate over all scans//3240 will hit parent libraries with 3244 hitting tandem
            //for (int i = 3000; i < 3225; i++)//this will iterate over all scans//3240 will hit parent libraries with 3244 hitting tandem
            //for (int i = 3000; i < 3025; i++)//this will iterate over all scans//3240 will hit parent libraries with 3244 hitting tandem
            //for (int i = 3239; i < 3250; i++)//this will iterate over all scans//3240 will hit parent libraries with 3244 hitting tandem
            //for (int i = 0; i < 3250; i++)//this will iterate over all scans//3240 will hit parent libraries with 3244 hitting tandem
            {
                #region inside logic for scan analysis

                int scan = resultsScans[i];
                int precursorPeakID = resultsPrecursorPeakIds[i];

                //DatabaseScanCentricObject currentScanInfo = resultsScanTable.FirstOrDefault(c => c.ScanCentricData.ScanNumLc == scan);//.ToList() ;
                DatabaseScanCentricObject currentScanInfo = resultsScanTable[i];
                
                //List<DatabaseScanObject> scanInfo = GetScanInfo.Read(scan, fileName);
                //DatabaseScanObject scanInfoResult = scanInfo[0];//where scan = scan
                //List<DatabaseFragmentCentricObject> scanInfoCentric = GetScanInfo.ReadCentric(scan, fileName);
                //DatabaseFragmentCentricObject currentScanInfo = new DatabaseFragmentCentricObject();
       //         List<DatabaseScanCentricObject> scanInfoCentric = GetScanInfo.ReadCentric(scan, fileName);
       //         DatabaseScanCentricObject currentScanInfo = new DatabaseScanCentricObject();


                //DatabaseScanCentricObject currentScanInfo = new DatabaseScanCentricObject();
                //if (scanInfoCentric.Count == 1)
                //{
                //    currentScanInfo = scanInfoCentric[0];
                //}
                //else
                //{
                //    Console.WriteLine("More than one fragment centric file was returned");
                //    Console.ReadKey();

                //}

                if (currentScanInfo == null)
                {
                    Console.WriteLine("More than one fragment centric file was returned");
                    Console.ReadKey();
                }
                int currentscanID = currentScanInfo.ScanCentricData.ScanID;// or just as good scan
                int currentMSLevel = currentScanInfo.ScanCentricData.MsLevel;
                int parentScan = currentScanInfo.ScanCentricData.ParentScanNumber;
                int tandemScan = currentScanInfo.ScanCentricData.TandemScanNumber;

                TandemAnalysisResultsObject result = new TandemAnalysisResultsObject();

                switch (currentMSLevel)
                {
                    case 1:
                        {
                            #region Inside PrecursorScan Analysis
                            Console.WriteLine(Environment.NewLine + "We are working on Precursor Scan collection " + scan);

                            #region go after all monoisotopic masses in parent scan and glycolyze them

                            //List<DatabasePeakProcessedObject> monoListFromParentScan = GetMonoPeaksFromScan.Read(scan, fileName);//TODO //shooting for monomass 1723.61 in parent scan 1326 which is fragmented in 1329
                            PrecursorAndPeaksObject monoListFromParentScan = GetPrecursorPeakAndTandemPeaks.ReadMonoPeaks(scan, fileName);

                            //if (monoListFromParentScan.Count > 0)
                            if (monoListFromParentScan.TandemMonoPeakCentricList.Count > 0)
                            {
                                useDiagnosticIons = false;
                                useChargedMasses = false;
                                if (parentScan == 1326)
                                {
                                    Console.WriteLine("here");
                                }

                                runAsMono = true;//true will use peak.PeakCentricData.MassMonoisotopic and false will use peak.PeakCentricData.Mz
                                GlycolyzerResults glycanResultsFromMonoParentScan = GlycolyzerFunction.Glycolyze(monoListFromParentScan.TandemMonoPeakCentricList, parentScan, useDiagnosticIons, useChargedMasses, runAsMono); //this needs to be decharged! and it is

                                GrabMonoSacchardeHeaddersForWriting(ref monosaccharideHeaders, glycanResultsFromMonoParentScan);

                                if (glycanResultsFromMonoParentScan.GlycanHits.Count > 0)
                                {
                                    Console.WriteLine("Monos have a Hit");
                                    //TODO add all peaks to writer 
                                    result.Scan = scan;
                                    result.IsFragmented = false;
                                    result.HitsFromPrecursorScans = glycanResultsFromMonoParentScan;
                                    overallMS1Results.Add(result);
                                }
                            }
                            else
                            {
                                Console.WriteLine("There are monoisotopic peaks in parent scan " + scan);
                            }

                            #endregion

                            #endregion
                        }
                        break;
                    case 2:
                        {
                            #region Inside TandemScan Analysis
                            Console.WriteLine(Environment.NewLine + "We are working on Tandem Scan collection " + tandemScan);

                            TandemAnalysisResultPairObject coupledResult = new TandemAnalysisResultPairObject();

                            PrecursorAndPeaksObject resultsPile = GetPrecursorPeakAndTandemPeaksAndMonoPeaks.Read(tandemScan, precursorPeakID, fileName);//TODO


                            //if (resultsPile.TandemPeakList.Count > 0)
                            if (resultsPile.TandemPeakCentricList.Count > 0)
                            {
                                //go after precursor peak first
                                #region go after monoisotopic mass chosen for fragmentation.  since we don't know charge state, cycle through charges

                                //search mass through library

                                int maxCharge = 5;
                                List<DatabasePeakCentricObject> monoList = ConvertMzToMonoListViaCharge(resultsPile.PrecursorCentricPeak, masssProton, maxCharge);

                                //run the glycolyzer once on the set of monoisotopic masses

                                GlycolyzerResults glycanResultsFromScanPrecursor = new GlycolyzerResults();

                                if (monoList.Count > 0)
                                {
                                    useDiagnosticIons = false; //match precursor to library
                                    useChargedMasses = false;
                                    runAsMono = true;//true will use peak.PeakCentricData.MassMonoisotopic and false will use peak.PeakCentricData.Mz
                                    glycanResultsFromScanPrecursor = GlycolyzerFunction.Glycolyze(monoList, tandemScan, useDiagnosticIons, useChargedMasses, runAsMono); //this needs to be decharged!
                                    GrabMonoSacchardeHeaddersForWriting(ref monosaccharideHeaders, glycanResultsFromScanPrecursor);
                                }

                                if (glycanResultsFromScanPrecursor.GlycanHits.Count > 0)
                                {
                                    if (glycanResultsFromScanPrecursor.GlycanHits.Count == 1)
                                    {
                                        Console.WriteLine("We got one!");
                                        result.ParentMono = glycanResultsFromScanPrecursor.GlycanHits[0].GlycanHitsLibraryExactMass;
                                        result.ParentChargeDiscovered = glycanResultsFromScanPrecursor.GlycanHits[0].GlycanHitsIndexFeature+1;//0 is +1, 1 is +2, 2 is +3 etc
                                        result.HitsFromPrecursorParentIon = glycanResultsFromScanPrecursor;
                                        //overallResults.Add(result);//below
                                        coupledResult.ParentResult = glycanResultsFromScanPrecursor.GlycanHits[0];
                                    }
                                    else
                                    {
                                        //perhaps take both so we don't take the wrong one
                                        //GlycanResult desiredHit = SelectBasedOnPPM(glycanResultsFromScanPrecursor);

                                        GlycanResult desiredHit = SelectBasedOnLowestCharge(glycanResultsFromScanPrecursor);

                                        Console.WriteLine("We got one from the pile!");
                                        result.ParentMono = desiredHit.GlycanHitsLibraryExactMass;
                                        result.ParentChargeDiscovered = desiredHit.GlycanHitsIndexFeature + 1;//0 is +1, 1 is +2, 2 is +3 etc
                                        result.HitsFromPrecursorParentIon = glycanResultsFromScanPrecursor;
                                        //overallResults.Add(result);//below
                                        coupledResult.ParentResult = desiredHit;

                                        //Console.WriteLine("This is a magical place where one glycan mass folds onto another glycan mass or the mass spacing of the libary is very close to eachother");
                                        //foreach (GlycanResult glycanHit in glycanResultsFromScanPrecursor.GlycanHits)
                                        //{
                                        //    Console.WriteLine("We got one from the pile!");
                                        //    result.ParentMono = glycanHit.GlycanHitsLibraryExactMass;
                                        //    result.ParentChargeDiscovered = glycanHit.GlycanHitsIndexFeature + 1;//0 is +1, 1 is +2, 2 is +3 etc
                                        //    result.HitsFromPrecursorParentIon = glycanResultsFromScanPrecursor;
                                        //    //overallResults.Add(result);//below
                                        //    coupledResult.ParentResult = glycanHit;
                                        //}

                                        
                                    }
                                }

                                #endregion

                                #region go after diagnostic ions in tandem data

                                //check for non zero
                                useDiagnosticIons = true;
                                useChargedMasses = false;
                                //GlycolyzerResults glycanResultsFromTandemScanDiagnostic = GlycolyzerFunction.Glycolyze(resultsPile.TandemPeakList, tandemScan, useDiagnosticIons, useChargedMasses); //this needs to be decharged!
                                runAsMono = false;//true will use peak.PeakCentricData.MassMonoisotopic and false will use peak.PeakCentricData.Mz
                                GlycolyzerResults glycanResultsFromTandemScanDiagnostic = GlycolyzerFunction.Glycolyze(resultsPile.TandemPeakCentricList, tandemScan, useDiagnosticIons, useChargedMasses, runAsMono); //this needs to be decharged!
                                
                                if (glycanResultsFromTandemScanDiagnostic.GlycanHits.Count > 0)
                                {
                                    Console.WriteLine("We got one!");
                                    foreach (GlycanResult resultHit in glycanResultsFromTandemScanDiagnostic.GlycanHits)
                                    {
                                        resultHit.GlycanHitsComposition.MassExact = Convert.ToDecimal(resultsPile.XValue);
                                        
                                    }
                                    result.HitsFromDiagnosticIons = glycanResultsFromTandemScanDiagnostic;

                                    coupledResult.TandemResults = result.HitsFromDiagnosticIons.GlycanHits;
                                    //overallResults.Add(result);//below
                                }

                                #endregion

                                if (glycanResultsFromTandemScanDiagnostic.GlycanHits.Count > 0)//the or is here 
                                {
                                    //general adds
                                    coupledResult.Scan = scan;
                                    coupledResult.IsFragmented = true;
                                    coupledResult.TandemScan = tandemScan;
                                    //coupledResult.ParentMZ = resultsPile.PrecursorPeak.XValueRaw;
                                    //coupledResult.ParentMZCorrected = resultsPile.PrecursorPeak.XValue;
                                    //coupledResult.ParentCharge = resultsPile.PrecursorPeak.Charge;
                                    coupledResult.ParentMZ = resultsPile.PrecursorCentricPeak.PeakCentricData.XValue;
                                    coupledResult.ParentMZCorrected = resultsPile.PrecursorCentricPeak.PeakCentricData.XValue;
                                    coupledResult.ParentCharge = resultsPile.PrecursorCentricPeak.PeakCentricData.Charge;

                                    overallMS2Results.Add(coupledResult);//add to list
                                }

                                //go after monoisotopic masses for differences second

                                //TODO add all tandem analysis to writer


                                

                            }
                            else
                            {
                                Console.WriteLine("There are no signal peaks in tandem scan " + tandemScan);
                            }
                            #endregion
                        }
                        break;
                    default:
                        {
                            Console.WriteLine("Strange ScanNumber");
                        }
                        break;
                }

                Console.WriteLine("do we have results?  " + overallMS1Results.Count);

                #endregion
            }


            #region feature find ms1 results and append to spine

            Console.WriteLine("1");
            GlycolyzerResults appendedResults = CompressAndAppendAndIntegrate(overallMS1Results);

            #endregion

            //write to disk

            GetPeaks_DLL.DataFIFO.StringListToDisk writer = new StringListToDisk();
            string seperator = ",";

            #region write characterizationFile

            List<string> dataToWrite = new List<string>();
            List<string> sipperToWrite = new List<string>();

            List<string> columnsToWrite = new List<string>();
            List<string> sipperColumnsToWrite = new List<string>();
            int sipperCounter = 0;
            int extraColumns = 5;
            foreach (TandemAnalysisResultsObject result in overallMS1Results)
            {


                if (result.HitsFromPrecursorScans != null)
                {
                    if (result.HitsFromPrecursorScans.GlycanHits.Count > 0)
                    {
                        //MS1,peak etc,,,
                        
                        ResultsTOLines("MS1", result.HitsFromPrecursorScans.GlycanHits, seperator, monosaccharideHeaders, ref dataToWrite, ref columnsToWrite);
                        ResultsTOLinesSipper(ref sipperCounter, result.HitsFromPrecursorScans.GlycanHits, seperator, monosaccharideHeaders, ref sipperToWrite, ref sipperColumnsToWrite, masssProton);
                        
                        //blank data
                        for (int i = 0; i < extraColumns; i++)
                        {
                            dataToWrite[dataToWrite.Count - 1] += seperator;
                        }
                    }
                }
            }

            Console.WriteLine("2");

            foreach (TandemAnalysisResultPairObject resultMS2 in overallMS2Results)
            {

                if (resultMS2.ParentResult != null)
                {
                    List<GlycanResult> holder = new List<GlycanResult>();
                    holder.Add(resultMS2.ParentResult);

                    if (resultMS2.TandemResults.Count > 0)
                    {
                        foreach (GlycanResult diagnosticIon in resultMS2.TandemResults)
                        {
                            ResultsTOLines("MS2", holder, seperator, monosaccharideHeaders, ref dataToWrite, ref columnsToWrite);
                            
                            dataToWrite[dataToWrite.Count - 1] += seperator + Convert.ToString(diagnosticIon.GlycanHitsExperimentalMass);//continue adding here
                            dataToWrite[dataToWrite.Count - 1] += seperator + Convert.ToString(diagnosticIon.GlycanHitsExperimentalFeature.UMCAbundance);//continue adding here
                            
                        }
                    }
                    else
                    {
                        //MS2,peak etc,,,

                        ResultsTOLines("MS2c", holder, seperator, monosaccharideHeaders, ref dataToWrite, ref columnsToWrite);
                        //blank data
                        for (int i = 0; i < extraColumns; i++)
                        {
                            dataToWrite[dataToWrite.Count - 1] += seperator;
                        }
                        
                    }
                    
                   // #endregion
                }
                else
                {
                    if (resultMS2.TandemResults.Count > 0)
                    {
                        foreach (GlycanResult diagnosticIon in resultMS2.TandemResults)
                        {
                            //ms2,precursor,,,,,,,,,,,data,data
                            
                            //ResultsTOLines("MS2", holder, seperator, ref dataToWrite, ref columnsToWrite, 0);
                            dataToWrite.Add("MS2" + seperator + Convert.ToString(diagnosticIon.GlycanHitsExperimentalMass));//fragment experimental mass
                            columnsToWrite.Add("ScanType" + seperator + "PrecursorMass");
                            dataToWrite[dataToWrite.Count - 1] += seperator + Convert.ToString(diagnosticIon.GlycanHitsLibraryExactMass);
                            dataToWrite[dataToWrite.Count - 1] += seperator + seperator + seperator + seperator + seperator + seperator;
                            columnsToWrite[columnsToWrite.Count - 1] += seperator + "Data Mono";
                            columnsToWrite[columnsToWrite.Count - 1] += seperator + "Library Mono";
                            columnsToWrite[columnsToWrite.Count - 1] += seperator + "Hex";


                            //dataToWrite[dataToWrite.Count - 1] += Convert.ToString(diagnosticIon.GlycanHitsExperimentalMass);//continue adding exact mass here
                            columnsToWrite[columnsToWrite.Count - 1] += seperator + "Scan";
                            dataToWrite[dataToWrite.Count - 1] += Convert.ToString(diagnosticIon.GlycanHitsExperimentalFeature.ScanStart);//continue adding exact mass here
                            
                            columnsToWrite[columnsToWrite.Count - 1] += "Fragment";//cointinue adding here

                            dataToWrite[dataToWrite.Count - 1] += seperator + Convert.ToString(diagnosticIon.GlycanHitsExperimentalFeature.UMCAbundance);//continue adding here
                            columnsToWrite[columnsToWrite.Count - 1] += "Fragment Abundance";//cointinue adding here

                        }
                    }
                }
                
            }
            //List<string> output = new List<string>();

            Console.WriteLine("3");
            //string outputLocation = @"K:\SQLite\HereWeGo.csv";
            //string outputLocation = @"E:\ScottK\Populator\HereWeGo.csv";
            writer.toDiskStringList(outputLocation, dataToWrite, columnsToWrite[0]);
            writer.toDiskStringList(outputSipperFileLocation, sipperToWrite, sipperColumnsToWrite[0]);
            #endregion

            #region write quantitation file
            List<string> dataToWriteQuantification = new List<string>();
            List<string> columnsToWriteQuantification = new List<string>();
            Console.WriteLine("4");

            string filenameOut = resultsFileName + ".txt";
            string folderOut = resutsFolder;
            FeatureOriginType featureType = FeatureOriginType.Viper;
            OmniFinderController newController = new OmniFinderController();
            //string parameterFilePathIn = @"E:\ScottK\0_Libraries\DB_NLinked_Alditol_PolySA_15_Alditol_Neutral.xml";
            string parameterFilePathIn = @"E:\ScottK\0_Libraries\DB_NLinked_Alditol10_15_Alditol_Neutral.xml";
            GUIImport.ImportParameters(parameterFilePathIn);
            ParameterModel parameterModel_Input = GUIImport.parameterModel_Input;
            GlycolyzerParametersGUI parameters = new GlycolyzerParametersGUI();
            parameters.ConvertFromGUI(parameterModel_Input);
            OmniFinderParameters omniFinderParameter = parameters.OmniFinderInput;
            OmniFinderOutput omniResults = newController.FindCompositions(omniFinderParameter);
            omniResults.CompositionHeaders = appendedResults.GlycanLibraryHeaders;
            WriteToDisk(filenameOut, folderOut, featureType, omniResults, appendedResults);

            #endregion


            int counter = 0;

            #region off

            //for (int i = 3240; i < resultsTandemScans.Count; i++)//i=3240 is tandem scan 3788 and has 204 and 366 for parent 943.02
            ////for (int i = 0; i < results.Count; i++)//i=3240 is tandem scan 3788 and has 204 and 366 for parent 943.02
            //{
            //    int tandemScan = resultsTandemScans[i];

            //    Console.WriteLine(Environment.NewLine + "We are working on Tandem Scan collection " + tandemScan);

            //    //go get precursor mass and tandem data
            //    PrecursorAndPeaksObject resultsPile = GetPrecursorPeakAndTandemPeaksAndMonoPeaks.Read(tandemScan, fileName);

            //    List<DatabaseScanObject> scanInfo = GetScanInfo.Read(tandemScan, fileName);
            //    int parentScan = scanInfo[0].ParentScan;

            //    #region go after all monoisotopic masses in parent scan and glycolyze them

            //    List<DatabasePeakProcessedObject> monoListFromParentScan = GetMonoPeaksFromScan.Read(parentScan, fileName);//shooting for monomass 1723.61 in parent scan 1326 which is fragmented in 1329

            //    useDiagnosticIons = false;
            //    useChargedMasses = false;
            //    if (parentScan == 1326)
            //    {
            //        Console.WriteLine("here");
            //    }

            //    GlycolyzerResults glycanResultsFromMonoParentScan = GlycolyzerFunction.Glycolyze(monoListFromParentScan, parentScan, useDiagnosticIons, useChargedMasses);//this needs to be decharged! and it is

            //    if (glycanResultsFromMonoParentScan.GlycanHits.Count > 0)
            //    {
            //        //HitsFromPrecursorScans.Add(glycanResultsFromMonoParentScan);
            //        Console.WriteLine("Monos have a Hit");
            //    }

            //    #endregion

            //    if (resultsPile.XValue > 500)// if there are no monoisotopic masses, this will 
            //    {
            //        #region go after monoisotopic mass chosen for fragmentation off (off later)

            //        //    double monoisotopicMass = MonoisotopicMass(masssProton, resultsPile.XValue, resultsPile.Charge);

            //        //    List<DatabasePeakProcessedObject> monoPeak = new List<DatabasePeakProcessedObject>();
            //        //    DatabasePeakProcessedObject mono = new DatabasePeakProcessedObject();
            //        //    mono.XValue = monoisotopicMass;
            //        //    monoPeak.Add(mono);
            //        //    Console.WriteLine("This scan has a mass of " + resultsPile.PrecursorPeak.XValueRaw + " at charge " + resultsPile.PrecursorPeak.Charge + " and mono at " + monoisotopicMass);

            //        //    Console.WriteLine("There are " + resultsPile.TandemPeakList.Count  + " peaks to work with");
            //        //    Console.WriteLine("There are " + resultsPile.TandemMonoPeakList.Count + "  mono peaks to work with");

            //        //    useDiagnosticIons = false;

            //        //    GlycolyzerResults glycanResultsFromMono = GlycolyzerFunction.Glycolyze(monoPeak, tandemScan, useDiagnosticIons);//this needs to be decharged! and it is

            //        //    if (glycanResultsFromMono.GlycanHits.Count > 0)
            //        //    {
            //        //        Console.WriteLine("We got one!");
            //        //    }

            //        //    if(resultsPile.PrecursorPeak.XValueRaw > 967)
            //        //    {
            //        //        Console.WriteLine("Please!!!!!");
            //        //    }

            //        //    if (glycanResultsFromMonoParentScan.GlycanHits.Count > 0)
            //        //{
            //        //    HitsFromPrecursorParentIons.Add(glycanResultsFromMono);
            //        //    Console.WriteLine("Monos have a Hit");
            //        //}
            //        #endregion

            //        Clock(starttime, stopWatch);


            //        Console.WriteLine("good data " + counter);
            //        counter++;

            //        #region go after diagnostic ions in tandem data

            //        //DatabasePeakProcessedObject parent = new DatabasePeakProcessedObject();
            //        //parent.Height = resultsPile.PrecursorPeak.Height;
            //        //parent.XValue = (912.343430112886 + masssProton *2)/2;
            //        //parent.Width = resultsPile.PrecursorPeak.Width;
            //        //resultsPile.TandemPeakList.Add(parent);
            //        //204.086649001293
            //        //292.102692996108
            //        //366.13947243251
            //        //309.118008706895
            //        //657.234888961856

            //        //TODO the results from the diagnostic ions is not returned

            //        useDiagnosticIons = true;
            //        useChargedMasses = true;
            //        GlycolyzerResults glycanResultsFromTandemScan = GlycolyzerFunction.Glycolyze(resultsPile.TandemPeakList, tandemScan, useDiagnosticIons, useChargedMasses);//this needs to be decharged!
            //        if (glycanResultsFromTandemScan.GlycanHits.Count > 0)
            //        {
            //            Console.WriteLine("We got one!");
            //        }

            //        if (glycanResultsFromTandemScan.GlycanHits.Count > 0)
            //        {
            //            Console.WriteLine("We got one!");
            //        }


            //        #endregion


            //        #region go after monoisotopic mass chosen for fragmentation.  since we don't know charge state, cycle through charges

            //        //search mass through library
            //        List<DatabasePeakProcessedObject> monoList = new List<DatabasePeakProcessedObject>();
            //        DatabasePeakProcessedObject monoMassconverted = new DatabasePeakProcessedObject();
            //        monoMassconverted.Height = resultsPile.PrecursorPeak.Height;
            //        monoMassconverted.ScanNum = resultsPile.PrecursorPeak.ScanNumberTandem;
            //        monoMassconverted.Width = resultsPile.PrecursorPeak.Width;


            //        for (int charge = 1; charge < 4; charge++)
            //        {
            //            //TODO this is not handeled inside glycolyzer

            //            monoMassconverted.WENEEDCHARGESTATE = charge;
            //            monoMassconverted.XValue = (resultsPile.PrecursorPeak.XValueRaw + masssProton * charge) / charge;
            //            monoList.Add(monoMassconverted);
            //            useDiagnosticIons = true;
            //            useChargedMasses = true;
            //            GlycolyzerResults glycanResultsFromScanPrecursor = GlycolyzerFunction.Glycolyze(monoList, tandemScan, useDiagnosticIons, useChargedMasses); //this needs to be decharged!
            //            if (glycanResultsFromScanPrecursor.GlycanHits.Count > 0)
            //            {
            //                Console.WriteLine("We got one!");
            //            }

            //        }

            //        #endregion
            //        //search diagnosti masses through tandem

            //        //search for differences in tandem
            //    }
            //}

            #endregion

            Console.WriteLine("Finished");
            //Console.ReadKey();
        }

        private static GlycanResult SelectBasedOnPPM(GlycolyzerResults glycanResultsFromScanPrecursor)
        {
            double minppm = ErrorCalculator.PPMAbsolute(glycanResultsFromScanPrecursor.GlycanHits[0].GlycanHitsExperimentalMass, glycanResultsFromScanPrecursor.GlycanHits[0].GlycanHitsLibraryExactMass);
            GlycanResult desiredHit = glycanResultsFromScanPrecursor.GlycanHits[0];
            Console.WriteLine("This is a magical place where one glycan mass folds onto another glycan mass or the mass spacing of the libary is very close to eachother");
            foreach (GlycanResult glycanHit in glycanResultsFromScanPrecursor.GlycanHits)
            {
                double testPPM = ErrorCalculator.PPMAbsolute(glycanHit.GlycanHitsExperimentalMass, glycanHit.GlycanHitsLibraryExactMass);
                if (testPPM <= minppm)
                {
                    desiredHit = glycanHit;
                    minppm = testPPM;
                }
            }
            return desiredHit;
        }

        private static GlycanResult SelectBasedOnLowestCharge(GlycolyzerResults glycanResultsFromScanPrecursor)
        {
            double minCharge = glycanResultsFromScanPrecursor.GlycanHits[0].GlycanHitsExperimentalFeature.ChargeStateMin;
            GlycanResult desiredHit = glycanResultsFromScanPrecursor.GlycanHits[0];
            Console.WriteLine("This is a magical place where one glycan mass folds onto another glycan mass or the mass spacing of the libary is very close to eachother");
            foreach (GlycanResult glycanHit in glycanResultsFromScanPrecursor.GlycanHits)
            {
                double testcharge = glycanHit.GlycanHitsExperimentalFeature.ChargeStateMin;
                if (testcharge <= minCharge)
                {
                    desiredHit = glycanHit;
                    minCharge = testcharge;
                }
            }
            return desiredHit;
        }

        private static GlycolyzerResults CompressAndAppendAndIntegrate(List<TandemAnalysisResultsObject> overallMS1Results)
        {
            //decompress overall ms1 results into a huge xy data list
            //compare xydatalist to library
            //sum hits together and append to table
            List<GlycanResult> resultList = new List<GlycanResult>();

            //List<DatabasePeakProcessedObject> feedToGlycolyzer = new List<DatabasePeakProcessedObject>();
            //foreach (TandemAnalysisResultsObject result in overallMS1Results)
            //{
            //    foreach (GlycanResult hit in result.HitsFromPrecursorScans.GlycanHits)
            //    {
            //        resultList.Add(hit);
            //        DatabasePeakProcessedObject newpoint = new DatabasePeakProcessedObject();
            //        newpoint.XValue = hit.GlycanHitsExperimentalMass;
            //        newpoint.Height = hit.GlycanHitsExperimentalFeature.UMCAbundance;
            //        feedToGlycolyzer.Add(newpoint);
            //    }
            //}
             GlycolyzerResults glycanResultsSummary = new GlycolyzerResults();

            if (overallMS1Results.Count > 0)
            {
                List<DatabasePeakCentricObject> feedToGlycolyzer = new List<DatabasePeakCentricObject>();
                foreach (TandemAnalysisResultsObject result in overallMS1Results)
                {
                    foreach (GlycanResult hit in result.HitsFromPrecursorScans.GlycanHits)
                    {
                        resultList.Add(hit);
                        DatabasePeakCentricObject newpoint = new DatabasePeakCentricObject();
                        //newpoint.PeakCentricData.MassMonoisotopic = hit.GlycanHitsExperimentalMass;
                        newpoint.PeakCentricData.XValue = hit.GlycanHitsExperimentalMass;
                        newpoint.PeakCentricData.Height = hit.GlycanHitsExperimentalFeature.UMCAbundance;
                        feedToGlycolyzer.Add(newpoint);
                    }
                }

                bool useDiagnosticIons = false;
                bool useChargedMasses = false;
                bool runAsMono = true; //true will use peak.PeakCentricData.MassMonoisotopic, false will use .PeakCentricData.Mz 
                glycanResultsSummary = GlycolyzerFunction.Glycolyze(feedToGlycolyzer, 0, useDiagnosticIons, useChargedMasses, runAsMono); //this needs to be decharged! and it is

                foreach (GlycanResult isomerResult in glycanResultsSummary.GlycanHitsInLibraryOrder)
                {
                    isomerResult.GlycanHitsExperimentalAbundance = 0;
                    if (isomerResult.GlycanPolyIsomer.FeatureList != null && isomerResult.GlycanPolyIsomer.FeatureList.Count > 0)
                    {
                        double experimentalMassSum = 0;
                        foreach (FeatureViper feature in isomerResult.GlycanPolyIsomer.FeatureList)
                        {
                            isomerResult.GlycanHitsExperimentalAbundance += feature.UMCAbundance; //here is where we add isomers (since these are scan by scan it is like integration/feature finding)
                            experimentalMassSum += feature.UMCMonoMW;
                        }
                        isomerResult.GlycanHitsExperimentalMass = experimentalMassSum/isomerResult.GlycanPolyIsomer.FeatureList.Count;
                    }
                }
            }
            //List<GlycanResult> miniIntensityResults = new List<GlycanResult>();
            //foreach (GlycanResult isomerResult in glycanResultsSummary.GlycanHitsInLibraryOrder)
            //{
            //    if (isomerResult.GlycanHitsExperimentalAbundance > 0)
            //    {
            //        miniIntensityResults.Add(isomerResult);
            //    }
            //}
            return glycanResultsSummary;
        }

        private static void GrabMonoSacchardeHeaddersForWriting(ref List<string> monosaccharideHeadders, GlycolyzerResults glycanResultsFromMonoParentScan)
        {
            if (monosaccharideHeadders.Count == 0)
            {
                monosaccharideHeadders = glycanResultsFromMonoParentScan.GlycanLibraryHeaders;
            }
        }


        private static void WriteToDisk(string filenameOut, string folderOut, FeatureOriginType featureType, OmniFinderOutput omniResults, GlycolyzerResults glycolyzerResults)
        {
            WriteGlycolyzerToDisk writer = new WriteGlycolyzerToDisk();

            string fullFileNameOut = folderOut + filenameOut;

            writer.WriteGlycanFile(fullFileNameOut, glycolyzerResults, omniResults);

            //switch (featureType)
            //{
            //    case FeatureOriginType.Viper:
            //        {
            //            writer.WriteViperFeatureFile(fullFileNameOut, glycolyzerResults); ;
            //        }
            //        break;
            //    case FeatureOriginType.IMS:
            //        {
            //            writer.WriteIMSFeatureFile(fullFileNameOut, glycolyzerResults);
            //        }
            //        break;
            //    default:
            //        {
            //            writer.WriteViperFeatureFile(fullFileNameOut, glycolyzerResults); ;
            //        }
            //        break;
            //}
        }


        private static void CrackArgsDBAnalysis(string[] args, out string outputFileLocation, out string outputSipperFileLocation, out string fileNameInput, out string resutsFolder, out string resultsFileName)
        {
            DataBaseAnalysisArgs fileParameters = new DataBaseAnalysisArgs(args);

            string fileNameFolder = fileParameters.inputDatbaseFolder;
            string fileNameName = fileParameters.fileNameOnly;
            string inputExtension = fileParameters.fileExtension;
            //string fileNameFolder = @"E:\ScottK\Populator";
            //string fileNameTest = @"K:\Csharp\0_TestDataFiles\Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_OrbitrapFilter_ClusterDefault__sum1_1cor_1div_0oLev_130PBR_20fit_(0).db";
            //string fileNameName = "Gly08_Velos4_Jaguar_200nL_SP01_3X_7uL_1000A_31Aug12_OrbitrapFilter_ClusterDefault__sum1_1cor_1div_0oLev_130PBR_20fit_(0)";

            string outputFileName = fileNameName + ".csv";
            

            outputFileLocation = fileNameFolder + "\\" + outputFileName;
            outputSipperFileLocation = fileNameFolder + "\\" + fileNameName + "_IQ.txt";
            
            fileNameInput = fileNameFolder + "\\" + fileNameName + inputExtension;

            resutsFolder = fileNameFolder + "\\";
            resultsFileName = fileNameName + "_Quant";

        }

        private static void ResultsTOLines(string scanCode, List<GlycanResult> resultList, string seperator, List<string> monosaccharideHeaders, ref List<string> dataToWrite, ref List<string> columnsToWrite)
        {
            foreach (GlycanResult hit in resultList)
            {
                List<Tuple<string, string>> simpleGlycanStringArrayFromPrecursorScan = new List<Tuple<string, string>>();

                simpleGlycanStringArrayFromPrecursorScan.Add(new Tuple<string, string>("Data Mono", Convert.ToString(hit.GlycanHitsExperimentalMass)));

                simpleGlycanStringArrayFromPrecursorScan.Add(new Tuple<string, string>("Library Mono", Convert.ToString(hit.GlycanHitsLibraryExactMass)));

                simpleGlycanStringArrayFromPrecursorScan.Add(new Tuple<string, string>("PPM", Convert.ToString(ErrorCalculator.PPM(hit.GlycanHitsExperimentalMass, hit.GlycanHitsLibraryExactMass))));
                //simpleGlycanStringArrayFromPrecursorScan.Add(new Tuple<string, string>("Library Mono Exact", Convert.ToString(hit.GlycanHitsComposition.MassExact)));//calculated from composition
                
                SetCompositions(hit, simpleGlycanStringArrayFromPrecursorScan, monosaccharideHeaders);

                simpleGlycanStringArrayFromPrecursorScan.Add(new Tuple<string, string>("Scan", Convert.ToString(hit.GlycanHitsExperimentalFeature.ScanStart)));
                simpleGlycanStringArrayFromPrecursorScan.Add(new Tuple<string, string>("Abundance", Convert.ToString(hit.GlycanHitsExperimentalFeature.UMCAbundance)));
                simpleGlycanStringArrayFromPrecursorScan.Add(new Tuple<string, string>("Charge", Convert.ToString(hit.GlycanHitsExperimentalFeature.ChargeStateMin)));

                string dataLine;
                string columnLine;
                WordsToLine(simpleGlycanStringArrayFromPrecursorScan, seperator, out dataLine, out columnLine);

                string finalDataLine = scanCode + seperator;
                finalDataLine += dataLine;
                dataToWrite.Add(finalDataLine);

                string finalColumnLine = "ScanType" + seperator;
                finalColumnLine += columnLine;

                //extra column headers
                finalColumnLine += seperator + "Fragment";//cointinue adding here
                finalColumnLine += seperator + "Fragment Abundance";//cointinue adding here

                columnsToWrite.Add(finalColumnLine);
            }
        }

        private static void ResultsTOLinesSipper(ref int targetID, List<GlycanResult> resultList, string seperator, List<string> monosaccharideHeaders, ref List<string> dataToWrite, ref List<string> columnsToWrite, double massProton)
        {
            seperator = "\t";

            for (int i = 0; i < resultList.Count;i++)
            {
                GlycanResult hit = resultList[i];

                //iterate over charge state
                bool iterateOverChargeStates = true;
                int minCharge = 0;
                int maxCharge = 1;
                if (hit.GlycanHitsExperimentalChargeMax > hit.GlycanHitsExperimentalChargeMin)
                {
                    minCharge = Convert.ToInt32(hit.GlycanHitsExperimentalChargeMin);
                    maxCharge = Convert.ToInt32(hit.GlycanHitsExperimentalChargeMax);
                }
                else
                {
                    minCharge = Convert.ToInt32(hit.GlycanHitsExperimentalChargeMin);
                    maxCharge = Convert.ToInt32(hit.GlycanHitsExperimentalChargeMin+1);//set both to the same number so we go through once
                }

                for (int charge = minCharge; charge < maxCharge; charge++)
                {
                    List<Tuple<string, string>> simpleGlycanStringArrayFromPrecursorScan = new List<Tuple<string, string>>();

                    simpleGlycanStringArrayFromPrecursorScan.Add(new Tuple<string, string>("TargetID", Convert.ToString(targetID)));
                    targetID++;
                    int glycanCode = 0;
                    for (int j = hit.GlycanHitsComposition.ListOfCompositions.Count - 1; j >= 0; j--)
                    {
                        glycanCode += Convert.ToInt32(Math.Pow(10, hit.GlycanHitsComposition.ListOfCompositions.Count - j - 1))*hit.GlycanHitsComposition.ListOfCompositions[j];
                    }

                    simpleGlycanStringArrayFromPrecursorScan.Add(new Tuple<string, string>("Code", Convert.ToString(glycanCode)));

                    simpleGlycanStringArrayFromPrecursorScan.Add(new Tuple<string, string>("EmpiricalFormula", Convert.ToString(hit.GlycanElementalFormula)));

                    //perhaps iterate over charge state range
                    simpleGlycanStringArrayFromPrecursorScan.Add(new Tuple<string, string>("ChargeState", Convert.ToString(charge)));

                    simpleGlycanStringArrayFromPrecursorScan.Add(new Tuple<string, string>("Scan", Convert.ToString(hit.GlycanHitsExperimentalFeature.ScanStart)));
                    simpleGlycanStringArrayFromPrecursorScan.Add(new Tuple<string, string>("MonoMZ", Convert.ToString(ConvertMonoToMz.Execute(hit.GlycanHitsExperimentalMass,charge,massProton))));
                    simpleGlycanStringArrayFromPrecursorScan.Add(new Tuple<string, string>("MonoisotopicMass", Convert.ToString(hit.GlycanHitsLibraryExactMass)));

                    string dataLine;
                    string columnLine;
                    WordsToLine(simpleGlycanStringArrayFromPrecursorScan, seperator, out dataLine, out columnLine);

                    //string finalDataLine = scanCode + seperator;
                    //finalDataLine += dataLine;
                    dataToWrite.Add(dataLine);

                    //string finalColumnLine = "ScanType" + seperator;
                    //finalColumnLine += columnLine;

                    //extra column headers
                    //finalColumnLine += seperator + "Fragment";//cointinue adding here
                    //finalColumnLine += seperator + "Fragment Abundance";//cointinue adding here

                    columnsToWrite.Add(columnLine);
                }
            }
        }

        private static void SetCompositions(GlycanResult hit, List<Tuple<string, string>> simpleGlycanStringArrayFromPrecursorScan, List<string> monosaccharideHeaders)
        {
            //TODO we need to sync the columns because the list of compositios can have different headders
            for (int i = 0; i < hit.GlycanHitsComposition.ListOfCompositions.Count; i++)
            {
                string monosaccharideType = monosaccharideHeaders[i];
                int numberOfUnits = hit.GlycanHitsComposition.ListOfCompositions[i];
                switch (i)
                {
                    case 0:
                        {
                            //simpleGlycanStringArrayFromPrecursorScan.Add(new Tuple<string, string>("Hex", Convert.ToString(numberOfUnits)));
                            simpleGlycanStringArrayFromPrecursorScan.Add(new Tuple<string, string>(monosaccharideType, Convert.ToString(numberOfUnits)));
                        }
                        break;
                    case 1:
                        {
                            //simpleGlycanStringArrayFromPrecursorScan.Add(new Tuple<string, string>("HexNAc", Convert.ToString(numberOfUnits)));
                            simpleGlycanStringArrayFromPrecursorScan.Add(new Tuple<string, string>(monosaccharideType, Convert.ToString(numberOfUnits)));
                        }
                        break;
                    case 2:
                        {
                            //simpleGlycanStringArrayFromPrecursorScan.Add(new Tuple<string, string>("Fuc", Convert.ToString(numberOfUnits)));
                            simpleGlycanStringArrayFromPrecursorScan.Add(new Tuple<string, string>(monosaccharideType, Convert.ToString(numberOfUnits)));
                        }
                        break;
                    case 3:
                        {
                            //simpleGlycanStringArrayFromPrecursorScan.Add(new Tuple<string, string>("Neu5Ac", Convert.ToString(numberOfUnits)));
                            simpleGlycanStringArrayFromPrecursorScan.Add(new Tuple<string, string>(monosaccharideType, Convert.ToString(numberOfUnits)));
                        }
                        break;
                    case 4:
                        {
                            //simpleGlycanStringArrayFromPrecursorScan.Add(new Tuple<string, string>("NaH", Convert.ToString(numberOfUnits)));
                            simpleGlycanStringArrayFromPrecursorScan.Add(new Tuple<string, string>(monosaccharideType, Convert.ToString(numberOfUnits)));
                        }
                        break;
                }
            }
        }

        private static string WordsToLine(List<string> data, string seperator)
        {
            string dataLine = "";

            for (int i = 0; i < data.Count - 1; i++)
            {
                dataLine += data[i];
                dataLine += seperator;
            }
            dataLine += data[data.Count - 1]; //last word
            return dataLine;
        }

        private static void WordsToLine(List<Tuple<string,string>> data, string seperator, out string dataLine, out string columnLine)
        {
            dataLine = "";
            columnLine = "";

            for (int i = 0; i < data.Count - 1; i++)
            {
                dataLine += data[i].Item2;
                dataLine += seperator;

                columnLine += data[i].Item1;
                columnLine += seperator;
            }
            dataLine += data[data.Count - 1].Item2; //last word
            columnLine += data[data.Count - 1].Item1; //last word
        }

        private static List<DatabasePeakCentricObject> ConvertMzToMonoListViaCharge(DatabasePeakCentricObject resultsPeak, double massProton, int maxCharge)
        {
            List<DatabasePeakCentricObject> monoList = new List<DatabasePeakCentricObject>();

            for (int charge = 1; charge < maxCharge; charge++)
            {
                //TODO this is not handeled inside glycolyzer
                //convert the precursor mz into a monoisotopic mass so it can be searched via glycolyzer
                DatabasePeakCentricObject monoMassconverted = new DatabasePeakCentricObject();
                //monoMassconverted.PeakCentricData.Mz = resultsPeak.PeakCentricData.Mz;
                //monoMassconverted.PeakCentricData.Height = resultsPeak.PeakCentricData.Height;
                monoMassconverted.Mz = resultsPeak.Mz;
                monoMassconverted.Height = resultsPeak.Height;
                //monoMassconverted.ScanNum = resultsPile.PrecursorPeak.ScanNumberTandem;
                monoMassconverted.PeakCentricData.Width = resultsPeak.PeakCentricData.Width;
                //monoMassconverted.PeakCentricData.ChargeState = charge;
                monoMassconverted.ChargeState = charge;
                //if (resultsPeak.PeakCentricData.Mz > 0)
                if (resultsPeak.Mz > 0)
                {

                    //monoMassconverted.PeakCentricData.MassMonoisotopic = ConvertMzToMono.Execute(resultsPeak.PeakCentricData.Mz, charge, massProton);
                    monoMassconverted.MassMonoisotopic = ConvertMzToMono.Execute(resultsPeak.Mz, charge, massProton);
                }

                else
                {
                    //xValueRaw.  not implemented yet
                    //monoMassconverted.PeakCentricData.MassMonoisotopic = ConvertMzToMono.Execute(resultsPeak.PeakCentricData.Mz, charge, massProton);
                    monoMassconverted.MassMonoisotopic = ConvertMzToMono.Execute(resultsPeak.Mz, charge, massProton);
                }
                monoList.Add(monoMassconverted);
            }

            return monoList;
        }

        private static void Clock(DateTime starttime, Stopwatch stopWatch)
        {
            System.DateTime stoptime = DateTime.Now;
            Console.WriteLine("This started at " + starttime + " and ended at" + stoptime);
            Console.WriteLine("This took " + stopWatch.Elapsed + " seconds to find " + 4 + " eluting peaks");
            Console.WriteLine("");
        }

        #region off
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="dataToCompare">DataToCompare</param>
        ///// <param name="scan">Scan Number/????</param>
        ///// <param name="useDiagnosticIons"></param>
        ///// <returns></returns>
        //private static GlycolyzerResults Glycolyze(List<DatabasePeakProcessedObject> dataToCompare, int scan, bool useDiagnosticIons)
        //{
        //    string parameterFilePathIn = @"D:\Csharp\0_TestDataFiles\DB_NLinked_Alditol_15_Alditol_Neutral.xml";
        //    //parameterFilePathIn = @"V:\GlycolyzerOut\NLinked_Alditol_150_Alditol_Neutral.xml";
        //    //parameterFilePathIn = @"V:\GlycolyzerOut\Cell_Alditol_V2_15_Alditol_Neutral.xml";
        //    //parameterFilePathIn = @"V:\GlycolyzerOut\NLinked_Alditol_15_Alditol_Neutral.xml";
        //    //parameterFilePathIn = @"V:\GlycolyzerOut\Cell_Alditol_V2_15_Alditol_Neutral.xml";
        //    //parameterFilePathIn = @"V:\GlycolyzerOut\NLinked_Alditol_PolySA_15_Alditol_Neutral.xml";
        //    //parameterFilePathIn = @"V:\GlycolyzerOut\Hexose_15_Alditol_Neutral.xml";
        //    //Cell_Alditol_V2_15_Alditol_Neutral.xml
        //    //string parameterFilePathIn = args[0].ToString();
        //    //string libraryFilePathIn = args[1].ToString();
        //    //string parameterFilePathIn = @"D:\Backup not sync\V Drive Copy\GlycolyzerGUITestFolders\SaveLocation\NLinked_Alditol_10_Alditol_Neutral.xml";
        //    //string parameterFilePathIn = @"C:\GlycolyzerData\SaveLocation\NLinked_Alditol_15_Alditol_Neutral.xml";
        //    //string libraryFilePathIn = @"R:\GlycolyzerData\GlycolyzerLibraryDirectorRDrive.txt";
        //    //string libraryFilePathIn = @"C:\GlycolyzerData\GlycolyzerLibraryDirectorCDrive.txt";
        //    string libraryFilePathIn = @"D:\Csharp\0_TestDataFiles\GlycolyzerLibraryDirectorCDrive.txt";

        //    Console.WriteLine(parameterFilePathIn + " is loaded");
        //    Console.WriteLine("I am running.  Press a key to continue, HI");
        //    //Console.ReadKey();


        //    GUIImport.ImportParameters(parameterFilePathIn);
        //    ParameterModel parameterModel_Input = GUIImport.parameterModel_Input;


        //    GlycolyzerParametersGUI parameters = new GlycolyzerParametersGUI();
        //    parameters.ConvertFromGUI(parameterModel_Input);

        //    if(useDiagnosticIons)
        //    {
        //        parameterModel_Input.LibrariesModel_Save.ChosenDefaultLibrary_String = "DiagnosticIons";
        //        parameters.LibraryTypeInput = LibraryType.DiagnosticIons;
        //    }

        //    string libraryLocation = libraryFilePathIn;
        //    string libraryPath = libraryLocation;
        //    //GlycolyzerController controller = new GlycolyzerController();
        //    //controller.Glycolyze(parameters, libraryLocation);

        //    //////////////////////////////////////////////////////////////////////////////////////////////////////////

        //    CompareController compareHere = new CompareController();
        //    SetListsToCompare prepCompare = new SetListsToCompare();
        //    IConvert letsConvert = new Converter();
        //    List<int> checkLengths = new List<int>();

        //    string filenameOutNonGlycan = parameters.FolderOut + "NonGlycan++";
        //    string folderOut = parameters.FolderOut;
        //    FeatureOriginType featureType = FeatureOriginType.Viper;
        //    string simpleFileName = parameters.FilesIn[0];
        //    simpleFileName = simpleFileName.Substring(0, simpleFileName.Length - 6) + "(" + scan + ")";//removes ().db
        //    string filenameOut = simpleFileName;
        //    string folderIn = parameters.FolderIn;
        //    double massTolleranceMatch = parameters.MassErrorPPM;//ppm
        //    double massTolleranceVerySmall = 100;//ppm//this will automatically be reduced till it works with a one-to one responce.
        //    LibraryType glycanType = parameters.LibraryTypeInput;
        //    OmniFinderParameters omniFinderParameter = GlycolyzerController.CheckOmniFinderRanges(glycanType, parameters.OmniFinderInput);

        //    List<FeatureAbstract> loadedFeatures = new List<FeatureAbstract>();

        //    double massProton = Constants.SubAtomicParticles[SubAtomicParticleName.Proton].MassMonoIsotopic;

        //    List<DatabasePeakProcessedWithMZObject> dataWithCharges = FragnemtationAnalysisTest.AddChargeStates(dataToCompare);

        //    foreach (DatabasePeakProcessedWithMZObject oPeak in dataWithCharges)
        //    {
        //        FeatureViper newFeature = new FeatureViper();
        //        newFeature.UMCMonoMW = oPeak.XValue;// -massProton;//subtract proton and assume everything is +1
        //        newFeature.UMCAbundance = oPeak.Height;
        //        newFeature.ScanStart = oPeak.ScanNumberTandem;
        //        loadedFeatures.Add(newFeature);
        //    }

        //    //////////////////////////////////////////////////////////////////////////////////////////////////////

        //    #region 0.  create OmnifinderLibrary

        //    OmniFinderController newController = new OmniFinderController();
        //    OmniFinderOutput omniResults = newController.FindCompositions(omniFinderParameter);

        //    //Console.WriteLine(omniResults.MassAndComposition[1].MassExact.ToString());

        //    #endregion   

        //    #region 1.  get data/library
        //    List<DataSet> loadedLibrary = new List<DataSet>();
        //    if (glycanType != LibraryType.OmniFinder)
        //    {
        //        LoadGlycanLibraries libraryLoader = new LoadGlycanLibraries();
        //        Dictionary<LibraryType, string> libraries = libraryLoader.Load(libraryPath);

        //        InputOutputFileName library = new InputOutputFileName();
        //        library.InputFileName = libraries[glycanType];
        //        GetDataController newFetcher = new GetDataController();
        //        loadedLibrary = newFetcher.GetDataLibrary(library);
        //    }
        //    else
        //    {
        //        DataSet omniFinderLibrary = new DataSet();
        //        omniFinderLibrary.Name = "OmniFinder";
        //        double counter = 0;
        //        foreach (OmnifinderExactMassObject massObject in omniResults.MassAndComposition)
        //        {
        //            omniFinderLibrary.XYList.Add(new XYData(Convert.ToDouble(massObject.MassExact), counter));
        //            counter++;
        //        }
        //        loadedLibrary.Add(omniFinderLibrary);

        //    }
        //    //load library
        //    //List<DataSet> loadedLibrary = loadedLibraryData(glycanType);


        //    //Assert.AreEqual(loadedLibrary[0].XYList.Count, 436);//for dataset 1
        //    Console.WriteLine("Region 1 complete");
        //    #endregion

        //    #region 2.  preprocess data for comparing

        //    //establish massTolleranceVerySmall.  This is a smart way to determine how small of a tollerance needs to be used for exact matching 
        //    massTolleranceVerySmall = GlycolyzerController.CalculateVerySmallMassTolerance(massTolleranceVerySmall, omniFinderParameter, loadedLibrary);

        //    //store indexes in y value
        //    //Glycan
        //    List<DataSet> loadedLibraryNonGlycan;
        //    //NonGlycan
        //    List<DataSet> loadedLibraryGlycanOnly;
        //    bool areGlycansPresent;
        //    bool areNonGlycansPresent;

        //    CompareResultsIndexes checkindexesFromCompare = GlycolyzerController.CheckLibraryAndSplitIndexes(massTolleranceVerySmall, omniFinderParameter, loadedLibrary, out loadedLibraryNonGlycan, out loadedLibraryGlycanOnly, out areGlycansPresent, out areNonGlycansPresent);


        //    //sort data
        //    List<FeatureAbstract> sortedFeatures = loadedFeatures.OrderBy(p => p.UMCMonoMW).ToList();
        //    List<double> sortedDataList = letsConvert.FeatureViperToMass(sortedFeatures);

        //    //results collection
        //    GlycolyzerResults glycolyzerResults = new GlycolyzerResults();
        //    GlycolyzerResults glycolyzerResultsAllCharges = new GlycolyzerResults();
        //    GlycolyzerResults glycolyzerNonGlycanResults = new GlycolyzerResults();

        //    Console.WriteLine("Region 2 complete");
        //    #endregion


        //    for (int charge = 0; charge < 4; charge++)//iterate 1-6
        //    {
        //        #region inside

        //        if (areGlycansPresent)
        //        {
        //            #region 3. Compare Residules to Glycan Library. //TODO changed 11-9-12

        //            //Glycan Only
        //            List<double> libraryListGlycanOnly = letsConvert.XYDataToMass(loadedLibraryGlycanOnly[0].XYList);
        //            //TODO changed 11-9-12
        //            List<double> libraryListGlycanOnly_MZ = ConvertMonoisotopicMassToMassToCharge(libraryListGlycanOnly, charge, massProton);

        //            //TODO changed 11-9-12
        //            //CompareInputLists inputListsGlycanOnly = prepCompare.SetThem(sortedDataList, libraryListGlycanOnly);
        //            CompareInputLists inputListsGlycanOnly = prepCompare.SetThem(sortedDataList, libraryListGlycanOnly_MZ);

        //            CompareResultsIndexes indexesFromCompareGlycanOnly = new CompareResultsIndexes();
        //            CompareResultsValues valuesFromCompareGlycanOnly = compareHere.compareFX(inputListsGlycanOnly, massTolleranceMatch, ref indexesFromCompareGlycanOnly);

        //            Console.WriteLine("Glycan Region 3 complete");

        //            #endregion

        //            #region 4. Bring Data Together and Populate Results File.

        //            //#region "Hits and Matches", convert to lists for export //Glycan Only
        //            List<FeatureAbstract> featureHitsGlycanOnly = new List<FeatureAbstract>();
        //            List<double> libraryHitsExactGlycanOnly = new List<double>();
        //            foreach (int hit in indexesFromCompareGlycanOnly.IndexListAMatch)
        //            {
        //                featureHitsGlycanOnly.Add(sortedFeatures[hit]);
        //            }
        //            foreach (int hit in indexesFromCompareGlycanOnly.IndexListBMatch)
        //            {
        //                libraryHitsExactGlycanOnly.Add(libraryListGlycanOnly[hit]); //this brings us back the normal mono libries after the M/Z compare
        //            }

        //            Console.WriteLine("Glycan Region 4 complete");

        //            #endregion

        //            if (libraryHitsExactGlycanOnly.Count > 0)
        //            {
        //                #region 5.  convert glycan hits to composition via OmniFinder

        //                Console.WriteLine("ApplyOmnifinder");

        //                #region 5a. create OmnifinderLibrary

        //                //extracted above

        //                #endregion

        //                #region 5b. compare glycan hits to the omnifinder library

        //                List<decimal> LibraryListOmni = letsConvert.ListDoubleToListDecimal(libraryHitsExactGlycanOnly);

        //                List<decimal> DataListOmni = new List<decimal>();
        //                foreach (OmnifinderExactMassObject exactMass in omniResults.MassAndComposition)
        //                {
        //                    DataListOmni.Add(exactMass.MassExact);
        //                }

        //                CompareInputLists inputListsOmni = prepCompare.SetThem(DataListOmni, LibraryListOmni);

        //                CompareResultsIndexes indexesFromCompareOmni = new CompareResultsIndexes();
        //                CompareResultsValues valuesFromCompareOmni = compareHere.compareFX(inputListsOmni, massTolleranceVerySmall, ref indexesFromCompareOmni);

        //                #endregion

        //                #region 5c. process hits from OmniFinder

        //                //check numbers are conserved.  The question is wheather there is a problem or there is a non glycan mass in the list
        //                if (libraryHitsExactGlycanOnly.Count != indexesFromCompareOmni.IndexListAMatch.Count)
        //                {
        //                    Console.WriteLine("the data in lead to multiple hits in the library when it should not.  one in one out.");
        //                    if (valuesFromCompareOmni.DataNotInLibrary.Count > 0)
        //                    {
        //                        Console.WriteLine("There is a mass in the load library that is not found in the omnifinder list of possibiliites");
        //                        Console.WriteLine(valuesFromCompareOmni.LibraryNotMatched[0].ToString() + " was not found");
        //                    }
        //                    Console.ReadKey();
        //                }

        //                List<OmnifinderExactMassObject> glycanHits = new List<OmnifinderExactMassObject>();
        //                for (int h = 0; h < indexesFromCompareOmni.IndexListAMatch.Count; h++)
        //                {
        //                    glycanHits.Add(omniResults.MassAndComposition[indexesFromCompareOmni.IndexListAMatch[h]]);
        //                }

        //                List<OmnifinderExactMassObject> misses = new List<OmnifinderExactMassObject>();
        //                if (indexesFromCompareOmni.IndexListBandNotA.Count > 0)
        //                {
        //                    for (int h = 0; h < indexesFromCompareOmni.IndexListAMatch.Count; h++)
        //                    {
        //                        misses.Add(omniResults.MassAndComposition[indexesFromCompareOmni.IndexListBandNotA[h]]);
        //                    }
        //                }

        //                #endregion

        //                Console.WriteLine("Glycan Region 5 complete");

        //                #endregion

        //                #region 6.  Set up Glycolyzer Results for Writing

        //                #region 6a.GlycanOnly assign to results //TODO changed 11-9-12

        //                glycolyzerResults.GlycanLibraryHeaders = omniResults.CompositionHeaders;
        //                glycolyzerResults.GlycanHits = new List<GlycanResult>();

        //                //1.  load results into GlycanResultFormat

        //                //check lengths
        //                checkLengths = new List<int>();
        //                int keyLenght = libraryHitsExactGlycanOnly.Count;
        //                checkLengths.Add(keyLenght);
        //                checkLengths.Add(valuesFromCompareGlycanOnly.DataMatchingLibrary.Count);
        //                checkLengths.Add(featureHitsGlycanOnly.Count);
        //                checkLengths.Add(glycanHits.Count);
        //                checkLengths.Add(indexesFromCompareGlycanOnly.IndexListAMatch.Count);
        //                checkLengths.Add(indexesFromCompareOmni.IndexListAMatch.Count);

        //                bool success = GlycolyzerController.VerifytListLengths(checkLengths, keyLenght);

        //                GlycolyzerController.DidThisWorkUnitTest(success);

        //                glycolyzerResults.GlycanUniqueHitsLibraryExactMass = libraryHitsExactGlycanOnly.Distinct().ToList();

        //                for (int j = 0; j < libraryHitsExactGlycanOnly.Count; j++)
        //                {
        //                    GlycanResult result = new GlycanResult();
        //                    double libraryExactMass = libraryHitsExactGlycanOnly[j];
        //                    result.GlycanHitsLibraryExactMass = libraryExactMass;
        //                    result.GlycanHitsExperimentalMass = Convert.ToDouble(valuesFromCompareGlycanOnly.DataMatchingLibrary[j]);
        //                    result.GlycanHitsExperimentalFeature = featureHitsGlycanOnly[j];
        //                    result.GlycanHitsComposition = glycanHits[j];
        //                    result.GlycanHitsIndexFeature = indexesFromCompareGlycanOnly.IndexListAMatch[j];
        //                    result.GlycanHitsIndexOmniFinder = indexesFromCompareOmni.IndexListAMatch[j];
        //                    glycolyzerResults.GlycanHits.Add(result);
        //                }

        //                //unique glycans.  Remove duplcates and save features into isomers and record indexes stored in glycolyzerResults.GlycanHits
        //                List<Isomer> Isomers;
        //                List<IsomerGlycanIndexes> isomerIndexes;

        //                //TODO changed 11-9-12
        //                //GlycolyzerController.AppendFeaturesToLibrarySpineList(massTolleranceMatch, glycolyzerResults, libraryListGlycanOnly, featureHitsGlycanOnly, out Isomers, out isomerIndexes);
        //                GlycolyzerController.AppendFeaturesToLibrarySpineList(massTolleranceMatch, glycolyzerResults, libraryListGlycanOnly_MZ, featureHitsGlycanOnly, out Isomers, out isomerIndexes);


        //                for (int j = 0; j < loadedLibraryGlycanOnly[0].XYList.Count; j++)
        //                {
        //                    GlycanResult result = new GlycanResult();
        //                    double libraryExactMass = loadedLibraryGlycanOnly[0].XYList[j].X;
        //                    result.GlycanHitsLibraryExactMass = libraryExactMass;

        //                    result.GlycanPolyIsomer = Isomers[j];
        //                    result.GlycanPolyIsomerIndex = isomerIndexes[j];
        //                    //summarize common aspects of isomers
        //                    //if (j > 131)//125//248
        //                    //{
        //                    //    j = j;
        //                    //}
        //                    //CheckConsisency(result);//if we pass this, it assumes all the isomers are the same composition// multiple hits to library are addressed here
        //                    int multipleHitDivisor = 1;
        //                    GlycolyzerController.CheckConsisency(result, out multipleHitDivisor); //if we pass this, it assumes all the isomers are the same composition

        //                    if (result.GlycanPolyIsomerIndex.FeatureIndexes.Count > 0)
        //                    {
        //                        GlycolyzerController.CondenseIsomers(omniResults, result, multipleHitDivisor, massTolleranceVerySmall); //pull out one value
        //                    }
        //                    glycolyzerResults.GlycanHitsInLibraryOrder.Add(result);
        //                }

        //                glycolyzerResults.Tollerance = massTolleranceMatch;
        //                glycolyzerResults.NumberOfHits = libraryHitsExactGlycanOnly.Count;

        //                Console.WriteLine(glycanHits[0].MassExact);

        //                #endregion

        //                Console.WriteLine("Glycan Region 6 complete");

        //                #endregion

        //                #region 7. WriteData

        //                bool write = true;
        //                if (write)
        //                {
        //                    //ViperToIgorWriter(filenameOut, folder, loadedViperFeatures);
        //                    if(glycolyzerResults.GlycanHits.Count>0)
        //                    {
        //                        if(glycolyzerResultsAllCharges.GlycanHitsInLibraryOrder.Count==0)
        //                        {
        //                            glycolyzerResultsAllCharges.GlycanHitsInLibraryOrder = glycolyzerResults.GlycanHitsInLibraryOrder;
        //                            glycolyzerResultsAllCharges.GlycanLibraryHeaders = glycolyzerResults.GlycanLibraryHeaders;
        //                            glycolyzerResultsAllCharges.Tollerance = glycolyzerResults.Tollerance;
        //                        }


        //                        for(int i=0;i<glycolyzerResults.GlycanHits.Count;i++)
        //                        {
        //                            glycolyzerResults.GlycanHits[i].GlycanHitsExperimentalChargeMax = charge;
        //                            glycolyzerResultsAllCharges.GlycanHits.Add(glycolyzerResults.GlycanHits[i]);
        //                            glycolyzerResultsAllCharges.GlycanUniqueHitsLibraryExactMass.Add(glycolyzerResults.GlycanUniqueHitsLibraryExactMass[i]);
        //                        }
                                
        //                    }

        //                    //TODO taken out for later once it is all compiled 11-9-12
        //                    //GlycolyzerController.WriteToDisk(filenameOut, folderOut, featureType, omniResults, glycolyzerResults);
        //                    Console.WriteLine("Glycan Region 7 complete, " + folderIn + filenameOut + " was written");
        //                }

        //                #endregion
        //            }
        //        }
        //        if (areNonGlycansPresent)
        //        {
        //            #region 3. Compare Residules to Glycan Library. //TODO changed 11-9-12

        //            //Non Glycan
        //            List<double> libraryListNonGlycan = letsConvert.XYDataToMass(loadedLibraryNonGlycan[0].XYList);
        //            //TODO changed 11-9-12
        //            List<double> libraryListNonGlycan_MZ = ConvertMonoisotopicMassToMassToCharge(libraryListNonGlycan, charge, massProton);

        //            //TODO changed 11-9-12
        //            //CompareInputLists inputListsNonGlycan = prepCompare.SetThem(sortedDataList, libraryListNonGlycan);
        //            CompareInputLists inputListsNonGlycan = prepCompare.SetThem(sortedDataList, libraryListNonGlycan_MZ);

        //            CompareResultsIndexes indexesFromCompareNonGlycan = new CompareResultsIndexes();
        //            CompareResultsValues valuesFromCompareNonGlycan = compareHere.compareFX(inputListsNonGlycan, massTolleranceMatch, ref indexesFromCompareNonGlycan);

        //            Console.WriteLine("Non Glycan Region 3 complete");

        //            #endregion

        //            #region 4. Bring Data Together and Populate Results File.

        //            //#region "Hits and Matches", convert to lists for export //NonGlycan
        //            List<FeatureAbstract> featureHitsNonGlycan = new List<FeatureAbstract>();
        //            List<double> libraryHitsExactNonGlycan = new List<double>();
        //            foreach (int hit in indexesFromCompareNonGlycan.IndexListAMatch)
        //            {
        //                featureHitsNonGlycan.Add(sortedFeatures[hit]);
        //            }
        //            foreach (int hit in indexesFromCompareNonGlycan.IndexListBMatch)
        //            {
        //                libraryHitsExactNonGlycan.Add(libraryListNonGlycan[hit]);
        //            }

        //            Console.WriteLine("Non Glycan Region 4 complete");

        //            #endregion

        //            #region 6.  Set up Glycolyzer Results for Writing

        //            #region 6b non glycan

        //            if (omniResults.CompositionHeaders != null)
        //            {
        //                glycolyzerNonGlycanResults.GlycanLibraryHeaders = omniResults.CompositionHeaders; //this is pulled from the glycan side
        //            }
        //            glycolyzerNonGlycanResults.GlycanHits = new List<GlycanResult>();

        //            //1.  load results into GlycanResultFormat
        //            //check lengths
        //            checkLengths = new List<int>();
        //            int keyLenght = libraryHitsExactNonGlycan.Count;
        //            checkLengths.Add(keyLenght);
        //            checkLengths.Add(valuesFromCompareNonGlycan.DataMatchingLibrary.Count);
        //            checkLengths.Add(featureHitsNonGlycan.Count);
        //            checkLengths.Add(indexesFromCompareNonGlycan.IndexListAMatch.Count);

        //            bool success = GlycolyzerController.VerifytListLengths(checkLengths, keyLenght);

        //            GlycolyzerController.DidThisWorkUnitTest(success);

        //            glycolyzerNonGlycanResults.GlycanUniqueHitsLibraryExactMass = libraryHitsExactNonGlycan.Distinct().ToList();

        //            for (int j = 0; j < libraryHitsExactNonGlycan.Count; j++)
        //            {
        //                GlycanResult result = new GlycanResult();
        //                double libraryExactMass = libraryHitsExactNonGlycan[j];
        //                result.GlycanHitsLibraryExactMass = libraryExactMass;
        //                result.GlycanHitsExperimentalMass = Convert.ToDouble(valuesFromCompareNonGlycan.DataMatchingLibrary[j]);
        //                result.GlycanHitsExperimentalFeature = featureHitsNonGlycan[j];
        //                //result.GlycanHitsComposition = new OmnifinderExactMassObject();
        //                result.GlycanHitsIndexFeature = indexesFromCompareNonGlycan.IndexListAMatch[j];
        //                glycolyzerNonGlycanResults.GlycanHits.Add(result);
        //            }



        //            if (libraryHitsExactNonGlycan.Count > 0)
        //            {
        //                List<Isomer> IsomersNonGlycan;
        //                List<IsomerGlycanIndexes> isomerIndexesNonGlycan;

        //                //TODO changed 11-9-12
        //                //GlycolyzerController.AppendFeaturesToLibrarySpineList(massTolleranceMatch, glycolyzerNonGlycanResults, libraryListNonGlycan, featureHitsNonGlycan, out IsomersNonGlycan, out isomerIndexesNonGlycan);
        //                GlycolyzerController.AppendFeaturesToLibrarySpineList(massTolleranceMatch, glycolyzerNonGlycanResults, libraryListNonGlycan_MZ, featureHitsNonGlycan, out IsomersNonGlycan, out isomerIndexesNonGlycan);


        //                for (int j = 0; j < loadedLibraryNonGlycan[0].XYList.Count; j++)
        //                {
        //                    GlycanResult result = new GlycanResult();
        //                    double libraryExactMass = loadedLibraryNonGlycan[0].XYList[j].X;
        //                    result.GlycanHitsLibraryExactMass = libraryExactMass;

        //                    result.GlycanPolyIsomer = IsomersNonGlycan[j];
        //                    result.GlycanPolyIsomerIndex = isomerIndexesNonGlycan[j];
        //                    //summarize common aspects of isomers

        //                    int multipleHitDivisor = 1;
        //                    GlycolyzerController.CheckConsisency(result, out multipleHitDivisor); //if we pass this, it assumes all the isomers are the same composition

        //                    if (result.GlycanPolyIsomerIndex.FeatureIndexes.Count > 0)
        //                    {
        //                        //TODO we need to send in the divisor
        //                        GlycolyzerController.CondenseIsomers(result, multipleHitDivisor); //pull out one value
        //                    }
        //                    glycolyzerNonGlycanResults.GlycanHitsInLibraryOrder.Add(result);
        //                }
        //            }
        //            glycolyzerNonGlycanResults.Tollerance = massTolleranceMatch;
        //            glycolyzerNonGlycanResults.NumberOfHits = libraryHitsExactNonGlycan.Count;

        //            #endregion

        //            Console.WriteLine("Non Glycan Region 6 complete");

        //            #endregion

        //            #region 7. WriteData

        //            bool write = true;
        //            if (write)
        //            {
        //                //GlycolyzerController.WriteToDisk(filenameOutNonGlycan, folderOut, featureType, omniResults, glycolyzerNonGlycanResults);
        //                Console.WriteLine("Non Glycan Region 7 complete, " + folderOut + filenameOut + " was written");
        //            }

        //            #endregion
        //        }

        //        #endregion
        //    }

        //    return glycolyzerResultsAllCharges;
        //}

        //private static List<double> ConvertMonoisotopicMassToMassToCharge(List<double> libraryListGlycanOnly, int chargeIn, double massProton)
        //{
        //    List<double> massToChargeList = new List<double>();
        //    double charge = Convert.ToDouble(chargeIn);

        //    if (charge > 0)
        //    {
        //        foreach (double monoisotopicMass in libraryListGlycanOnly)
        //        {
        //            double massToCharge = (monoisotopicMass + charge*massProton)/charge;
        //            massToChargeList.Add(massToCharge);
        //        }
        //    }
        //    else
        //    {
        //        massToChargeList = libraryListGlycanOnly;//send it out
        //    }
        //    return massToChargeList;
        //}

        #endregion

        private static double MonoisotopicMass(double masssProton, double massToCharge, int charge)
        {
            //double masssProton = Constants.SubAtomicParticles[SubAtomicParticleName.Proton].MassMonoIsotopic;
            return (massToCharge*charge) - charge * masssProton;
        }




    }
}
