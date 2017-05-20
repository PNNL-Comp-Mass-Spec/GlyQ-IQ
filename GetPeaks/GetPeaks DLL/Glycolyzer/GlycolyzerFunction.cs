using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects.ResultsObjects;
using GetPeaks_DLL.SQLite.DataTransferObjects;
using GlycolyzerGUImvvm.Models;
using GlycolyzerGUImvvm.ViewModels;
using GetPeaks_DLL.Objects.ParameterObjects;
using GetPeaks_DLL.Glycolyzer.Enumerations;
using CompareContrastDLL;
using GetPeaks_DLL.Functions;
using OmniFinder.Objects;
using GetPeaks_DLL.Objects;
using OmniFinder.Objects.BuildingBlocks;
using OmniFinder.Objects.Enumerations;
using PNNLOmics.Data.Constants;
using OmniFinder;
using GetPeaks_DLL.AnalysisSupport;
using PNNLOmics.Data;
using YAFMS_DB.GetPeaks;

namespace GetPeaks_DLL.Glycolyzer
{
    public static class GlycolyzerFunction
    {
        public static GlycolyzerResults Glycolyze(List<DatabasePeakCentricObject> dataToCompare, int scan, bool useDiagnosticIons, bool chargedMasses, bool runAsMono)
        //public static GlycolyzerResults Glycolyze(List<DatabasePeakProcessedObject> dataToCompare, int scan, bool useDiagnosticIons, bool chargedMasses)
        
        {
            #region server
            //string parameterFilePathIn = @"E:\ScottK\0_Libraries\DB_NLinked_Alditol10_15_Alditol_Neutral.xml";
            //string parameterFilePathIn = @"E:\ScottK\0_Libraries\DB_NLinked_Alditol_PolySA_15_Alditol_Neutral.xml";
            //string parameterFilePathIn = @"E:\ScottK\0_Libraries\DB_Cell_Alditol_V2_15_Alditol_Neutral.xml";
            //string parameterFilePathIn = @"E:\ScottK\0_Libraries\DB_Ant_Alditol_15_Alditol_Neutral.xml";
            //string parameterFilePathIn = @"E:\ScottK\0_Libraries\DB_Hexose_15_Alditol_Neutral.xml";
            string parameterFilePathIn = @"E:\ScottK\0_Libraries\DB_Xylose_15_Alditol_Neutral.xml";
            //string parameterFilePathIn = @"E:\ScottK\0_Libraries\DB_Xylose_4_Alditol_Neutral.xml";

            string libraryFilePathIn = @"E:\ScottK\0_Libraries\GlycolyzerLibraryDirectorEDrive.txt";


            //DB_Cell_Alditol_V2_15_Alditol_H
            #endregion


            //string parameterFilePathIn = @"D:\Csharp\0_TestDataFiles\DB_NLinked_Alditol_15_Alditol_Neutral.xml";//standard n-linked
            //string parameterFilePathIn = @"D:\Csharp\0_TestDataFiles\DB_NLinked_Alditol_PolySA_15_Alditol_Neutral.xml";
            //string parameterFilePathIn = @"D:\Csharp\0_TestDataFiles\DB_NLinked_Alditol9_15_Alditol_Neutral.xml";
            //string parameterFilePathIn = @"D:\Csharp\0_TestDataFiles\DB_NLinked_Alditol10_15_Alditol_Neutral.xml";
            //parameterFilePathIn = @"V:\GlycolyzerOut\NLinked_Alditol_150_Alditol_Neutral.xml";
            //parameterFilePathIn = @"V:\GlycolyzerOut\Cell_Alditol_V2_15_Alditol_Neutral.xml";
            //parameterFilePathIn = @"V:\GlycolyzerOut\NLinked_Alditol_15_Alditol_Neutral.xml";
            //parameterFilePathIn = @"V:\GlycolyzerOut\Cell_Alditol_V2_15_Alditol_Neutral.xml";
            //parameterFilePathIn = @"V:\GlycolyzerOut\NLinked_Alditol_PolySA_15_Alditol_Neutral.xml";
            //parameterFilePathIn = @"V:\GlycolyzerOut\Hexose_15_Alditol_Neutral.xml";
            //Cell_Alditol_V2_15_Alditol_Neutral.xml
            //string parameterFilePathIn = args[0].ToString();
            //string libraryFilePathIn = args[1].ToString();
            //string parameterFilePathIn = @"D:\Backup not sync\V Drive Copy\GlycolyzerGUITestFolders\SaveLocation\NLinked_Alditol_10_Alditol_Neutral.xml";
            //string parameterFilePathIn = @"C:\GlycolyzerData\SaveLocation\NLinked_Alditol_15_Alditol_Neutral.xml";
            //string libraryFilePathIn = @"R:\GlycolyzerData\GlycolyzerLibraryDirectorRDrive.txt";
            //string libraryFilePathIn = @"C:\GlycolyzerData\GlycolyzerLibraryDirectorCDrive.txt";
            
            //string libraryFilePathIn = @"D:\Csharp\0_TestDataFiles\GlycolyzerLibraryDirectorCDrive.txt";

            Console.WriteLine(parameterFilePathIn + " is loaded");
            Console.WriteLine("I am running.  Press a key to continue, HI");
            //Console.ReadKey();


            GUIImport.ImportParameters(parameterFilePathIn);
            ParameterModel parameterModel_Input = GUIImport.parameterModel_Input;


            GlycolyzerParametersGUI parameters = new GlycolyzerParametersGUI();
            parameters.ConvertFromGUI(parameterModel_Input);

            if (useDiagnosticIons)
            {
                parameterModel_Input.LibrariesModel_Save.ChosenDefaultLibrary_String = "DiagnosticIons";
                parameters.LibraryTypeInput = LibraryType.DiagnosticIons;
            }

            string libraryLocation = libraryFilePathIn;
            string libraryPath = libraryLocation;
            //GlycolyzerController controller = new GlycolyzerController();
            //controller.Glycolyze(parameters, libraryLocation);

            //////////////////////////////////////////////////////////////////////////////////////////////////////////

            CompareController compareHere = new CompareController();
            SetListsToCompare prepCompare = new SetListsToCompare();
            IConvert letsConvert = new Converter();
            List<int> checkLengths = new List<int>();

            string filenameOutNonGlycan = parameters.FolderOut + "NonGlycan++";
            string folderOut = parameters.FolderOut;
            FeatureOriginType featureType = FeatureOriginType.Viper;
            string simpleFileName = parameters.FilesIn[0];
            simpleFileName = simpleFileName.Substring(0, simpleFileName.Length - 6) + "(" + scan + ")";//removes ().db
            string filenameOut = simpleFileName;
            string folderIn = parameters.FolderIn;
            double massTolleranceMatch = parameters.MassErrorPPM;//ppm
            double massTolleranceVerySmall = 100;//ppm//this will automatically be reduced till it works with a one-to one responce.
            LibraryType glycanType = parameters.LibraryTypeInput;
            OmniFinderParameters omniFinderParameter = GlycolyzerController.CheckOmniFinderRanges(glycanType, parameters.OmniFinderInput);

            List<FeatureAbstract> loadedFeatures = new List<FeatureAbstract>();

            double massProton = Constants.SubAtomicParticles[SubAtomicParticleName.Proton].MassMonoIsotopic;

            List<DatabasePeakProcessedWithMZObject> dataWithCharges = AddChargeStates(dataToCompare, runAsMono);

            foreach (DatabasePeakProcessedWithMZObject oPeak in dataWithCharges)
            {
                FeatureViper newFeature = new FeatureViper();
                newFeature.UMCMonoMW = oPeak.XValue;// -massProton;//subtract proton and assume everything is +1
                newFeature.UMCAbundance = oPeak.Height;
                newFeature.ScanStart = oPeak.ScanNumberTandem;
                newFeature.ClassStatsChargeBasis = oPeak.Charge;
                newFeature.ChargeStateMin = oPeak.Charge;
                loadedFeatures.Add(newFeature);
            }

            //////////////////////////////////////////////////////////////////////////////////////////////////////

            #region 0.  create OmnifinderLibrary

            OmniFinderController newController = new OmniFinderController();
            OmniFinderOutput omniResults = newController.FindCompositions(omniFinderParameter);

            //Console.WriteLine(omniResults.MassAndComposition[1].MassExact.ToString());

            #endregion

            #region 1.  get data/library
            List<DataSet> loadedLibrary = new List<DataSet>();
            if (glycanType != LibraryType.OmniFinder)
            {
                LoadGlycanLibraries libraryLoader = new LoadGlycanLibraries();
                Dictionary<LibraryType, string> libraries = libraryLoader.Load(libraryPath);

                InputOutputFileName library = new InputOutputFileName();
                library.InputFileName = libraries[glycanType];
                GetDataController newFetcher = new GetDataController();
                loadedLibrary = newFetcher.GetDataLibrary(library);
            }
            else
            {
                DataSet omniFinderLibrary = new DataSet();
                omniFinderLibrary.Name = "OmniFinder";
                double counter = 0;
                foreach (OmnifinderExactMassObject massObject in omniResults.MassAndComposition)
                {
                    omniFinderLibrary.XYList.Add(new XYData(Convert.ToDouble(massObject.MassExact), counter));
                    counter++;
                }
                loadedLibrary.Add(omniFinderLibrary);

            }
            //load library
            //List<DataSet> loadedLibrary = loadedLibraryData(glycanType);


            //Assert.AreEqual(loadedLibrary[0].XYList.Count, 436);//for dataset 1
            Console.WriteLine("Region 1 complete");
            #endregion

            #region 2.  preprocess data for comparing

            //establish massTolleranceVerySmall.  This is a smart way to determine how small of a tollerance needs to be used for exact matching 
            massTolleranceVerySmall = GlycolyzerController.CalculateVerySmallMassTolerance(massTolleranceVerySmall, omniFinderParameter, loadedLibrary);

            //store indexes in y value
            //Glycan
            List<DataSet> loadedLibraryNonGlycan;
            //NonGlycan
            List<DataSet> loadedLibraryGlycanOnly;
            bool areGlycansPresent;
            bool areNonGlycansPresent;

            CompareResultsIndexes checkindexesFromCompare = GlycolyzerController.CheckLibraryAndSplitIndexes(massTolleranceVerySmall, omniFinderParameter, loadedLibrary, out loadedLibraryNonGlycan, out loadedLibraryGlycanOnly, out areGlycansPresent, out areNonGlycansPresent);


            //sort data
            List<FeatureAbstract> sortedFeatures = loadedFeatures.OrderBy(p => p.UMCMonoMW).ToList();
            List<double> sortedDataList = letsConvert.FeatureViperToMass(sortedFeatures);

            //results collection
            GlycolyzerResults glycolyzerResults = new GlycolyzerResults();
            GlycolyzerResults glycolyzerResultsAllCharges = new GlycolyzerResults();
            GlycolyzerResults glycolyzerNonGlycanResults = new GlycolyzerResults();

            Console.WriteLine("Region 2 complete");
            #endregion

            int mincharge;
            int maxcharge;
            if (chargedMasses)
            {
                mincharge = 0;
                maxcharge = 4;  //it is unlikley to find more than a +1 diagnostic ion.  0 should give the correct fragment mass 204 and 366?
            }
            else//monoisootpic masses
            {
                mincharge = 0;
                maxcharge = 1;
            }

            for (int charge = mincharge; charge < maxcharge; charge++)//iterate 1-6
            {
                #region inside

                if (areGlycansPresent)
                {
                    #region 3. Compare Residules to Glycan Library. //TODO changed 11-9-12

                    //Glycan Only
                    List<double> libraryListGlycanOnly = letsConvert.XYDataToMass(loadedLibraryGlycanOnly[0].XYList);
                    //TODO changed 11-9-12
                    List<double> libraryListGlycanOnly_MZ = ConvertMonoisotopicMassToMassToCharge(libraryListGlycanOnly, charge, massProton);

                    //TODO changed 11-9-12
                    //CompareInputLists inputListsGlycanOnly = prepCompare.SetThem(sortedDataList, libraryListGlycanOnly);
                    CompareInputLists inputListsGlycanOnly = prepCompare.SetThem(sortedDataList, libraryListGlycanOnly_MZ);

                    CompareResultsIndexes indexesFromCompareGlycanOnly = new CompareResultsIndexes();
                    CompareResultsValues valuesFromCompareGlycanOnly = compareHere.compareFX(inputListsGlycanOnly, massTolleranceMatch, ref indexesFromCompareGlycanOnly);

                    Console.WriteLine("Glycan Region 3 complete");

                    #endregion

                    #region 4. Bring Data Together and Populate Results File.

                    //#region "Hits and Matches", convert to lists for export //Glycan Only
                    List<FeatureAbstract> featureHitsGlycanOnly = new List<FeatureAbstract>();
                    List<double> libraryHitsExactGlycanOnly = new List<double>();
                    foreach (int hit in indexesFromCompareGlycanOnly.IndexListAMatch)
                    {
                        featureHitsGlycanOnly.Add(sortedFeatures[hit]);
                    }
                    foreach (int hit in indexesFromCompareGlycanOnly.IndexListBMatch)
                    {
                        libraryHitsExactGlycanOnly.Add(libraryListGlycanOnly[hit]); //this brings us back the normal mono libries after the M/Z compare
                    }

                    Console.WriteLine("Glycan Region 4 complete");

                    #endregion

                    if (libraryHitsExactGlycanOnly.Count > 0)
                    {
                        #region 5.  convert glycan hits to composition via OmniFinder

                        Console.WriteLine("ApplyOmnifinder");

                        #region 5a. create OmnifinderLibrary

                        //extracted above

                        #endregion

                        #region 5b. compare glycan hits to the omnifinder library

                        List<decimal> LibraryListOmni = letsConvert.ListDoubleToListDecimal(libraryHitsExactGlycanOnly);

                        List<decimal> DataListOmni = new List<decimal>();
                        foreach (OmnifinderExactMassObject exactMass in omniResults.MassAndComposition)
                        {
                            DataListOmni.Add(exactMass.MassExact);
                        }

                        CompareInputLists inputListsOmni = prepCompare.SetThem(DataListOmni, LibraryListOmni);

                        CompareResultsIndexes indexesFromCompareOmni = new CompareResultsIndexes();
                        CompareResultsValues valuesFromCompareOmni = compareHere.compareFX(inputListsOmni, massTolleranceVerySmall, ref indexesFromCompareOmni);

                        #endregion

                        #region 5c. process hits from OmniFinder

                        //check numbers are conserved.  The question is wheather there is a problem or there is a non glycan mass in the list
                        if (libraryHitsExactGlycanOnly.Count != indexesFromCompareOmni.IndexListAMatch.Count)
                        {
                            Console.WriteLine("the data in lead to multiple hits in the library when it should not.  one in one out.");
                            if (valuesFromCompareOmni.DataNotInLibrary.Count > 0)
                            {
                                Console.WriteLine("There is a mass in the load library that is not found in the omnifinder list of possibiliites");
                                Console.WriteLine(valuesFromCompareOmni.LibraryNotMatched[0].ToString() + " was not found");
                            }
                            Console.ReadKey();
                        }

                        List<OmnifinderExactMassObject> glycanOnlyHits = new List<OmnifinderExactMassObject>();
                        for (int h = 0; h < indexesFromCompareOmni.IndexListAMatch.Count; h++)
                        {
                            glycanOnlyHits.Add(omniResults.MassAndComposition[indexesFromCompareOmni.IndexListAMatch[h]]);
                        }

                        List<OmnifinderExactMassObject> misses = new List<OmnifinderExactMassObject>();
                        if (indexesFromCompareOmni.IndexListBandNotA.Count > 0)
                        {
                            for (int h = 0; h < indexesFromCompareOmni.IndexListAMatch.Count; h++)
                            {
                                misses.Add(omniResults.MassAndComposition[indexesFromCompareOmni.IndexListBandNotA[h]]);
                            }
                        }

                        #endregion

                        Console.WriteLine("Glycan Region 5 complete");

                        #endregion

                        #region 6.  Set up Glycolyzer Results for Writing

                        #region 6a.GlycanOnly assign to results //TODO changed 11-9-12

                        glycolyzerResults.GlycanLibraryHeaders = omniResults.CompositionHeaders;
                        glycolyzerResults.GlycanHits = new List<GlycanResult>();

                        //1.  load results into GlycanResultFormat

                        //check lengths
                        checkLengths = new List<int>();
                        int keyLenght = libraryHitsExactGlycanOnly.Count;
                        checkLengths.Add(keyLenght);
                        checkLengths.Add(valuesFromCompareGlycanOnly.DataMatchingLibrary.Count);
                        checkLengths.Add(featureHitsGlycanOnly.Count);
                        checkLengths.Add(glycanOnlyHits.Count);
                        checkLengths.Add(indexesFromCompareGlycanOnly.IndexListAMatch.Count);
                        checkLengths.Add(indexesFromCompareOmni.IndexListAMatch.Count);

                        bool success = GlycolyzerController.VerifytListLengths(checkLengths, keyLenght);

                        GlycolyzerController.DidThisWorkUnitTest(success);

                        glycolyzerResults.GlycanUniqueHitsLibraryExactMass = libraryHitsExactGlycanOnly.Distinct().ToList();

                        //for (int j = 0; j < libraryHitsExactGlycanOnly.Count; j++)
                        //{
                        //    GlycanResult result = new GlycanResult();
                        //    double libraryExactMass = libraryHitsExactGlycanOnly[j];
                        //    result.GlycanHitsLibraryExactMass = libraryExactMass;
                        //    result.GlycanHitsExperimentalMass = Convert.ToDouble(valuesFromCompareGlycanOnly.DataMatchingLibrary[j]);
                        //    result.GlycanHitsExperimentalFeature = featureHitsGlycanOnly[j];
                        //    result.GlycanHitsComposition = glycanHits[j];
                        //    result.GlycanHitsIndexFeature = indexesFromCompareGlycanOnly.IndexListAMatch[j];
                        //    result.GlycanHitsExperimentalChargeMin = featureHitsGlycanOnly[j].ChargeStateMin;
                        //    result.GlycanHitsIndexOmniFinder = indexesFromCompareOmni.IndexListAMatch[j];
                        //    glycolyzerResults.GlycanHits.Add(result);
                        //}

                        glycolyzerResults.GlycanHits = SetUpHitResults(
                            libraryHitsExactGlycanOnly, 
                            valuesFromCompareGlycanOnly, 
                            featureHitsGlycanOnly, 
                            glycanOnlyHits, 
                            indexesFromCompareGlycanOnly,
                            indexesFromCompareOmni, 
                            omniFinderParameter);

                        //unique glycans.  Remove duplcates and save features into isomers and record indexes stored in glycolyzerResults.GlycanHits
                        List<Isomer> Isomers;
                        List<IsomerGlycanIndexes> isomerIndexes;

                        //TODO changed 11-9-12
                        //GlycolyzerController.AppendFeaturesToLibrarySpineList(massTolleranceMatch, glycolyzerResults, libraryListGlycanOnly, featureHitsGlycanOnly, out Isomers, out isomerIndexes);
                        GlycolyzerController.AppendFeaturesToLibrarySpineList(massTolleranceMatch, glycolyzerResults, libraryListGlycanOnly_MZ, featureHitsGlycanOnly, out Isomers, out isomerIndexes);


                        for (int j = 0; j < loadedLibraryGlycanOnly[0].XYList.Count; j++)
                        {
                            GlycanResult result = new GlycanResult();
                            double libraryExactMass = loadedLibraryGlycanOnly[0].XYList[j].X;
                            result.GlycanHitsLibraryExactMass = libraryExactMass;

                            result.GlycanPolyIsomer = Isomers[j];
                            result.GlycanPolyIsomerIndex = isomerIndexes[j];
                            //summarize common aspects of isomers
                            //if (j > 131)//125//248
                            //{
                            //    j = j;
                            //}
                            //CheckConsisency(result);//if we pass this, it assumes all the isomers are the same composition// multiple hits to library are addressed here
                            int multipleHitDivisor = 1;
                            GlycolyzerController.CheckConsisency(result, out multipleHitDivisor); //if we pass this, it assumes all the isomers are the same composition

                            if (result.GlycanPolyIsomerIndex.FeatureIndexes.Count > 0)
                            {
                                GlycolyzerController.CondenseIsomers(omniResults, result, multipleHitDivisor, massTolleranceVerySmall); //pull out one value
                            }
                            glycolyzerResults.GlycanHitsInLibraryOrder.Add(result);
                        }

                        glycolyzerResults.Tollerance = massTolleranceMatch;
                        glycolyzerResults.NumberOfHits = libraryHitsExactGlycanOnly.Count;

                        Console.WriteLine(glycanOnlyHits[0].MassExact);

                        #endregion

                        Console.WriteLine("Glycan Region 6 complete");

                        #endregion

                        #region 7. WriteData

                        bool write = true;
                        if (write)
                        {
                            SummarizeAllCharge(glycolyzerResults, glycolyzerResultsAllCharges, charge);

                            //TODO taken out for later once it is all compiled 11-9-12
                            //GlycolyzerController.WriteToDisk(filenameOut, folderOut, featureType, omniResults, glycolyzerResults);
                            Console.WriteLine("Glycan Region 7 complete, " + folderIn + filenameOut + " was written");
                        }

                        #endregion
                    }
                }
                if (areNonGlycansPresent)
                {
                    #region 3. Compare Residules to Glycan Library. //TODO changed 11-9-12

                    //Non Glycan
                    List<double> libraryListNonGlycan = letsConvert.XYDataToMass(loadedLibraryNonGlycan[0].XYList);
                    //TODO changed 11-9-12
                    List<double> libraryListNonGlycan_MZ = ConvertMonoisotopicMassToMassToCharge(libraryListNonGlycan, charge, massProton);

                    //TODO changed 11-9-12
                    //CompareInputLists inputListsNonGlycan = prepCompare.SetThem(sortedDataList, libraryListNonGlycan);
                    CompareInputLists inputListsNonGlycan = prepCompare.SetThem(sortedDataList, libraryListNonGlycan_MZ);

                    CompareResultsIndexes indexesFromCompareNonGlycan = new CompareResultsIndexes();
                    CompareResultsValues valuesFromCompareNonGlycan = compareHere.compareFX(inputListsNonGlycan, massTolleranceMatch, ref indexesFromCompareNonGlycan);

                    Console.WriteLine("Non Glycan Region 3 complete");

                    #endregion

                    #region 4. Bring Data Together and Populate Results File.

                    //#region "Hits and Matches", convert to lists for export //NonGlycan
                    List<FeatureAbstract> featureHitsNonGlycan = new List<FeatureAbstract>();
                    List<double> libraryHitsExactNonGlycan = new List<double>();
                    foreach (int hit in indexesFromCompareNonGlycan.IndexListAMatch)
                    {
                        featureHitsNonGlycan.Add(sortedFeatures[hit]);
                    }
                    foreach (int hit in indexesFromCompareNonGlycan.IndexListBMatch)
                    {
                        libraryHitsExactNonGlycan.Add(libraryListNonGlycan[hit]);
                    }

                    Console.WriteLine("Non Glycan Region 4 complete");

                    #endregion

                    #region 6.  Set up Glycolyzer Results for Writing

                    #region 6b non glycan

                    if (omniResults.CompositionHeaders != null)
                    {
                        glycolyzerNonGlycanResults.GlycanLibraryHeaders = omniResults.CompositionHeaders; //this is pulled from the glycan side
                    }
                    glycolyzerNonGlycanResults.GlycanHits = new List<GlycanResult>();

                    //1.  load results into GlycanResultFormat
                    //check lengths
                    checkLengths = new List<int>();
                    int keyLenght = libraryHitsExactNonGlycan.Count;
                    checkLengths.Add(keyLenght);
                    checkLengths.Add(valuesFromCompareNonGlycan.DataMatchingLibrary.Count);
                    checkLengths.Add(featureHitsNonGlycan.Count);
                    checkLengths.Add(indexesFromCompareNonGlycan.IndexListAMatch.Count);

                    bool success = GlycolyzerController.VerifytListLengths(checkLengths, keyLenght);

                    GlycolyzerController.DidThisWorkUnitTest(success);

                    glycolyzerNonGlycanResults.GlycanUniqueHitsLibraryExactMass = libraryHitsExactNonGlycan.Distinct().ToList();

                    //for (int j = 0; j < libraryHitsExactNonGlycan.Count; j++)
                    //{
                    //    GlycanResult result = new GlycanResult();
                    //    double libraryExactMass = libraryHitsExactNonGlycan[j];
                    //    result.GlycanHitsLibraryExactMass = libraryExactMass;
                    //    result.GlycanHitsExperimentalMass = Convert.ToDouble(valuesFromCompareNonGlycan.DataMatchingLibrary[j]);
                    //    result.GlycanHitsExperimentalFeature = featureHitsNonGlycan[j];
                    //    //result.GlycanHitsComposition = new OmnifinderExactMassObject();
                        
                    //    result.GlycanHitsIndexFeature = indexesFromCompareNonGlycan.IndexListAMatch[j];
                    //    glycolyzerNonGlycanResults.GlycanHits.Add(result);
                    //}

                    //omnifinder is not used here
                    glycolyzerNonGlycanResults.GlycanHits = SetUpHitResults(
                            libraryHitsExactNonGlycan,
                            valuesFromCompareNonGlycan,
                            featureHitsNonGlycan,
                            indexesFromCompareNonGlycan);

                    if (libraryHitsExactNonGlycan.Count > 0)
                    {
                        List<Isomer> IsomersNonGlycan;
                        List<IsomerGlycanIndexes> isomerIndexesNonGlycan;

                        //TODO changed 11-9-12
                        //GlycolyzerController.AppendFeaturesToLibrarySpineList(massTolleranceMatch, glycolyzerNonGlycanResults, libraryListNonGlycan, featureHitsNonGlycan, out IsomersNonGlycan, out isomerIndexesNonGlycan);
                        GlycolyzerController.AppendFeaturesToLibrarySpineList(massTolleranceMatch, glycolyzerNonGlycanResults, libraryListNonGlycan_MZ, featureHitsNonGlycan, out IsomersNonGlycan, out isomerIndexesNonGlycan);


                        for (int j = 0; j < loadedLibraryNonGlycan[0].XYList.Count; j++)
                        {
                            GlycanResult result = new GlycanResult();
                            double libraryExactMass = loadedLibraryNonGlycan[0].XYList[j].X;
                            result.GlycanHitsLibraryExactMass = libraryExactMass;

                            result.GlycanPolyIsomer = IsomersNonGlycan[j];
                            result.GlycanPolyIsomerIndex = isomerIndexesNonGlycan[j];
                            //summarize common aspects of isomers

                            int multipleHitDivisor = 1;
                            GlycolyzerController.CheckConsisency(result, out multipleHitDivisor); //if we pass this, it assumes all the isomers are the same composition

                            if (result.GlycanPolyIsomerIndex.FeatureIndexes.Count > 0)
                            {
                                //TODO we need to send in the divisor
                                GlycolyzerController.CondenseIsomers(result, multipleHitDivisor); //pull out one value
                            }
                            glycolyzerNonGlycanResults.GlycanHitsInLibraryOrder.Add(result);
                        }
                    }
                    glycolyzerNonGlycanResults.Tollerance = massTolleranceMatch;
                    glycolyzerNonGlycanResults.NumberOfHits = libraryHitsExactNonGlycan.Count;

                    #endregion

                    Console.WriteLine("Non Glycan Region 6 complete");

                    #endregion

                    #region 7. WriteData

                    bool write = true;
                    if (write)
                    {
                        SummarizeAllCharge(glycolyzerNonGlycanResults, glycolyzerResultsAllCharges, charge);


                        //GlycolyzerController.WriteToDisk(filenameOutNonGlycan, folderOut, featureType, omniResults, glycolyzerNonGlycanResults);
                        Console.WriteLine("Non Glycan Region 7 complete, " + folderOut + filenameOut + " was written");
                    }

                    #endregion
                }

                #endregion
            }

            return glycolyzerResultsAllCharges;
        }

        private static List<GlycanResult> SetUpHitResults(
            List<double> libraryHitsExactGlycanOnly, 
            CompareResultsValues valuesFromCompareGlycanOnly, 
            List<FeatureAbstract> featureHitsGlycanOnly, 
            List<OmnifinderExactMassObject> glycanOnlyHits, 
            CompareResultsIndexes indexesFromCompareGlycanOnly, 
            CompareResultsIndexes indexesFromCompareOmni,
            OmniFinderParameters omniFinderParameter)
        {
            List<GlycanResult> hits = new List<GlycanResult>();
            for (int j = 0; j < libraryHitsExactGlycanOnly.Count; j++)
            {
                GlycanResult result = new GlycanResult();
                double libraryExactMass = libraryHitsExactGlycanOnly[j];
                result.GlycanHitsLibraryExactMass = libraryExactMass;
                result.GlycanHitsExperimentalMass = Convert.ToDouble(valuesFromCompareGlycanOnly.DataMatchingLibrary[j]);
                result.GlycanHitsExperimentalFeature = featureHitsGlycanOnly[j];
                result.GlycanHitsComposition = glycanOnlyHits[j];

                result.GlycanElementalFormula = DetermineElementalFormula(glycanOnlyHits, j, omniFinderParameter);

                result.GlycanHitsIndexFeature = indexesFromCompareGlycanOnly.IndexListAMatch[j];
                result.GlycanHitsExperimentalChargeMin = featureHitsGlycanOnly[j].ChargeStateMin;
                result.GlycanHitsIndexOmniFinder = indexesFromCompareOmni.IndexListAMatch[j];

                hits.Add(result);
            }

            return hits;
        }

        private static string DetermineElementalFormula(List<OmnifinderExactMassObject> glycanOnlyHits, int j, OmniFinderParameters omniFinderParameter)
        {
            Dictionary<ElementName, int> ElementalCompositions = new Dictionary<ElementName, int>();
            ElementalCompositions.Add(ElementName.Carbon, 0);
            ElementalCompositions.Add(ElementName.Hydrogen, 0);
            ElementalCompositions.Add(ElementName.Nitrogen, 0);
            ElementalCompositions.Add(ElementName.Oxygen, 0);
            ElementalCompositions.Add(ElementName.Phosphorous, 0);
            ElementalCompositions.Add(ElementName.Potassium, 0);
            ElementalCompositions.Add(ElementName.Sodium, 0);
            ElementalCompositions.Add(ElementName.Sulfur, 0);

            for (int i = 0; i < glycanOnlyHits[j].ListOfCompositions.Count;i++ )
            {
                int monosaccharideCount = glycanOnlyHits[j].ListOfCompositions[i];
                BuildingBlockMonoSaccharide currentMonosaccharide = omniFinderParameter.BuildingBlocksMonosacchcarides[i];
                {
                    ElementalCompositions[ElementName.Carbon] += monosaccharideCount * currentMonosaccharide.ElementalCompositions[ElementName.Carbon];
                    ElementalCompositions[ElementName.Hydrogen] += monosaccharideCount * currentMonosaccharide.ElementalCompositions[ElementName.Hydrogen];
                    ElementalCompositions[ElementName.Nitrogen] += monosaccharideCount * currentMonosaccharide.ElementalCompositions[ElementName.Nitrogen];
                    ElementalCompositions[ElementName.Oxygen] += monosaccharideCount * currentMonosaccharide.ElementalCompositions[ElementName.Oxygen];
                    ElementalCompositions[ElementName.Phosphorous] += monosaccharideCount * currentMonosaccharide.ElementalCompositions[ElementName.Phosphorous];
                    ElementalCompositions[ElementName.Potassium] += monosaccharideCount * currentMonosaccharide.ElementalCompositions[ElementName.Potassium];
                    ElementalCompositions[ElementName.Sodium] += monosaccharideCount * currentMonosaccharide.ElementalCompositions[ElementName.Sodium];
                    ElementalCompositions[ElementName.Sulfur] += monosaccharideCount * currentMonosaccharide.ElementalCompositions[ElementName.Sulfur];
                }
            }
            omniFinderParameter.CarbohydrateType = CarbType.Fragment;
            AddCarbType(omniFinderParameter, ref ElementalCompositions);

            string glycanElementalFormula = "";
            if (ElementalCompositions[ElementName.Carbon] > 0) glycanElementalFormula += "C" + ElementalCompositions[ElementName.Carbon];
            if (ElementalCompositions[ElementName.Hydrogen] > 0) glycanElementalFormula += "H" + ElementalCompositions[ElementName.Hydrogen];
            if (ElementalCompositions[ElementName.Nitrogen] > 0) glycanElementalFormula += "N" + ElementalCompositions[ElementName.Nitrogen];
            if (ElementalCompositions[ElementName.Oxygen] > 0) glycanElementalFormula += "O" + ElementalCompositions[ElementName.Oxygen];
            if (ElementalCompositions[ElementName.Phosphorous] > 0) glycanElementalFormula += "P" + ElementalCompositions[ElementName.Phosphorous];
            if (ElementalCompositions[ElementName.Potassium] > 0) glycanElementalFormula += "K" + ElementalCompositions[ElementName.Potassium];
            if (ElementalCompositions[ElementName.Sodium] > 0) glycanElementalFormula += "Na" + ElementalCompositions[ElementName.Sodium];
            if (ElementalCompositions[ElementName.Sulfur] > 0) glycanElementalFormula += "S" + ElementalCompositions[ElementName.Sulfur];
            
            return glycanElementalFormula;
        }

        private static void AddCarbType(OmniFinderParameters omniFinderParameter, ref Dictionary<ElementName, int> elementalCompositions)
        {
            switch (omniFinderParameter.CarbohydrateType)
            {
                case CarbType.Aldehyde:
                    {
                        elementalCompositions[ElementName.Oxygen] += 1;
                        elementalCompositions[ElementName.Hydrogen] += 2;
                    }
                    break;
                case CarbType.Alditol:
                    {
                        elementalCompositions[ElementName.Oxygen] += 1;
                        elementalCompositions[ElementName.Hydrogen] += 4;//H2O + H2
                    }
                    break;
                case CarbType.Fragment:
                    //add nothing
                    break;
                case CarbType.Glycopeptide:
                    {
                        Console.WriteLine("Check this in the Glycolyzer Function");
                        Console.ReadKey();
                        elementalCompositions[ElementName.Oxygen] += 1;
                        elementalCompositions[ElementName.Hydrogen] += 2;
                    }
                    break;
            }
        }

        private static List<GlycanResult> SetUpHitResults(
            List<double> libraryHitsExactGlycanOnly,
            CompareResultsValues valuesFromCompareGlycanOnly,
            List<FeatureAbstract> featureHitsGlycanOnly,
            CompareResultsIndexes indexesFromCompareGlycanOnly)
        {
            List<GlycanResult> hits = new List<GlycanResult>();
            for (int j = 0; j < libraryHitsExactGlycanOnly.Count; j++)
            {
                GlycanResult result = new GlycanResult();
                double libraryExactMass = libraryHitsExactGlycanOnly[j];
                result.GlycanHitsLibraryExactMass = libraryExactMass;
                result.GlycanHitsExperimentalMass = Convert.ToDouble(valuesFromCompareGlycanOnly.DataMatchingLibrary[j]);
                result.GlycanHitsExperimentalFeature = featureHitsGlycanOnly[j];
                //result.GlycanHitsComposition = glycanOnlyHits[j];//OmniFinder
                result.GlycanHitsIndexFeature = indexesFromCompareGlycanOnly.IndexListAMatch[j];
                result.GlycanHitsExperimentalChargeMin = featureHitsGlycanOnly[j].ChargeStateMin;
                //result.GlycanHitsIndexOmniFinder = indexesFromCompareOmni.IndexListAMatch[j];//OmniFinder

                hits.Add(result);
            }

            return hits;
        }


        //for (int j = 0; j < libraryHitsExactNonGlycan.Count; j++)
        //{
        //    GlycanResult result = new GlycanResult();
        //    double libraryExactMass = libraryHitsExactNonGlycan[j];
        //    result.GlycanHitsLibraryExactMass = libraryExactMass;
        //    result.GlycanHitsExperimentalMass = Convert.ToDouble(valuesFromCompareNonGlycan.DataMatchingLibrary[j]);
        //    result.GlycanHitsExperimentalFeature = featureHitsNonGlycan[j];
        //    //result.GlycanHitsComposition = new OmnifinderExactMassObject();

        //    result.GlycanHitsIndexFeature = indexesFromCompareNonGlycan.IndexListAMatch[j];
        //    glycolyzerNonGlycanResults.GlycanHits.Add(result);
        //}

        private static void SummarizeAllCharge(GlycolyzerResults glycolyzerResults, GlycolyzerResults glycolyzerResultsAllCharges, int charge)
        {
            //ViperToIgorWriter(filenameOut, folder, loadedViperFeatures);
            if (glycolyzerResults.GlycanHits.Count > 0)
            {
                if (glycolyzerResultsAllCharges.GlycanHitsInLibraryOrder.Count == 0)
                {
                    glycolyzerResultsAllCharges.GlycanHitsInLibraryOrder.AddRange(glycolyzerResults.GlycanHitsInLibraryOrder);
                    glycolyzerResultsAllCharges.GlycanLibraryHeaders = glycolyzerResults.GlycanLibraryHeaders;
                    glycolyzerResultsAllCharges.Tollerance = glycolyzerResults.Tollerance;
                }


                for (int i = 0; i < glycolyzerResults.GlycanHits.Count; i++)
                {
                    glycolyzerResults.GlycanHits[i].GlycanHitsExperimentalChargeMax = charge;
                    glycolyzerResultsAllCharges.GlycanHits.Add(glycolyzerResults.GlycanHits[i]);
                    //glycolyzerResultsAllCharges.GlycanUniqueHitsLibraryExactMass.Add(glycolyzerResults.GlycanUniqueHitsLibraryExactMass[i]);
                }

                foreach (double pureMass in glycolyzerResults.GlycanUniqueHitsLibraryExactMass)
                {
                    glycolyzerResultsAllCharges.GlycanUniqueHitsLibraryExactMass.Add(pureMass);
                }

                glycolyzerResultsAllCharges.NumberOfHits = glycolyzerResults.GlycanHits.Count;
            }
        }

        private static List<DatabasePeakProcessedWithMZObject> AddChargeStates(List<DatabasePeakProcessedObject> peaks)
        {
            List<DatabasePeakProcessedWithMZObject> results = new List<DatabasePeakProcessedWithMZObject>();
            //TODO the other way is to iterate over all the charge states

            foreach (DatabasePeakProcessedObject peak in peaks)
            {
                DatabasePeakProcessedWithMZObject newPoint = new DatabasePeakProcessedWithMZObject();

                newPoint.Height = peak.Height;
                newPoint.Width = peak.Width;
                newPoint.XValue = peak.XValue;
                newPoint.Charge = peak.Charge;

                newPoint.Background = peak.Background;
                newPoint.LocalLowestMinimaHeight = peak.LocalLowestMinimaHeight;
                newPoint.LocalSignalToNoise = peak.LocalSignalToNoise;
                newPoint.PeakNumber = peak.PeakNumber;
                newPoint.ScanNumberTandem = peak.ScanNum;
                newPoint.SignalToBackground = peak.SignalToBackground;
                newPoint.SignalToNoiseGlobal = peak.SignalToNoiseGlobal;
                newPoint.SignalToNoiseLocalMinima = peak.SignalToNoiseLocalMinima;

                results.Add(newPoint);
            }
            return results;
        }

        private static List<DatabasePeakProcessedWithMZObject> AddChargeStates(List<DatabasePeakCentricObject> peaks, bool runAsMono)
        {
            List<DatabasePeakProcessedWithMZObject> results = new List<DatabasePeakProcessedWithMZObject>();
            //TODO the other way is to iterate over all the charge states

            foreach (DatabasePeakCentricObject peak in peaks)
            {
                DatabasePeakProcessedWithMZObject newPoint = new DatabasePeakProcessedWithMZObject();

                newPoint.Height = peak.PeakCentricData.Height;
                newPoint.Width = peak.PeakCentricData.Width;
                if (runAsMono)
                {
                    //newPoint.XValue = peak.PeakCentricData.MassMonoisotopic;
                    newPoint.XValue = peak.MassMonoisotopic;
                }
                else
                {
                    //newPoint.XValue = peak.PeakCentricData.Mz;
                    newPoint.XValue = peak.Mz;
                }

                //newPoint.Charge = peak.PeakCentricData.ChargeState;
                newPoint.Charge = peak.ChargeState;

                newPoint.Background = peak.PeakCentricData.Background;
                //newPoint.LocalLowestMinimaHeight = peak.PeakCentricData..LocalLowestMinimaHeight;
                newPoint.LocalSignalToNoise = peak.PeakCentricData.LocalSignalToNoise;
                //newPoint.PeakNumber = peak.PeakCentricData.PeakID;
                newPoint.PeakNumber = peak.PeakID;
                //newPoint.ScanNumberTandem = peak.PeakCentricData.ScanID;
                newPoint.ScanNumberTandem = peak.ScanID;
                //newPoint.SignalToBackground = peak.PeakCentricData.SignalToBackground;
                //newPoint.SignalToNoiseGlobal = peak.PeakCentricData.SignalToNoiseGlobal;
                //newPoint.SignalToNoiseLocalMinima = peak.PeakCentricData.SignalToNoiseLocalMinima;

                results.Add(newPoint);
            }
            return results;
        }

        private static List<double> ConvertMonoisotopicMassToMassToCharge(List<double> libraryListGlycanOnly, int chargeIn, double massProton)
        {
            List<double> massToChargeList = new List<double>();
            double charge = Convert.ToDouble(chargeIn);

            if (charge > 0)
            {
                foreach (double monoisotopicMass in libraryListGlycanOnly)
                {
                    double massToCharge = (monoisotopicMass + charge * massProton) / charge;
                    massToChargeList.Add(massToCharge);
                }
            }
            else
            {
                massToChargeList = libraryListGlycanOnly;//send it out
            }
            return massToChargeList;
        }


    }
}
