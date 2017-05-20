using System;
using System.Collections.Generic;
using System.Linq;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.Objects.ParameterObjects;
using GetPeaks_DLL.Glycolyzer.Enumerations;
using OmniFinder.Objects;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.Objects.ResultsObjects;
using OmniFinder;
using GetPeaks_DLL.Functions;
using PNNLOmics.Data;
using GetPeaks_DLL.AnalysisSupport;
using OmniFinder.Objects.BuildingBlocks;
using OmniFinder.Objects.Enumerations;
using GetPeaks_DLL.Glycolyzer.Objects;
using CompareContrastDLL;

namespace GetPeaks_DLL.Glycolyzer
{
    public class GlycolyzerController
    {
        public void Glycolyze(GlycolyzerParametersGUI parameters, string libraryPath)
        {
            string filenameIn;
            string filenameOut;
            string filenameOutNonGlycan;
            string folderIn;
            string folderOut;
            int numberOfDataSetsTotal;

            FeatureAbstract feature;
            FeatureViper viper = new FeatureViper();
            feature = viper;
            List<FeatureAbstract> listF = new List<FeatureAbstract>();

            
            LibraryType glycanType = parameters.LibraryTypeInput;
            FeatureOriginType featureType = parameters.FeatureOriginTypeInput;
     
            SetFile2(parameters, 0, out filenameIn, out filenameOut, out filenameOutNonGlycan, out folderIn, out folderOut, out numberOfDataSetsTotal, out glycanType, out featureType);

            //parameters
            double massTolleranceMatch = parameters.MassErrorPPM;//ppm
            double massTolleranceVerySmall = 100;//ppm//this will automatically be reduced till it works with a one-to one responce.
            Console.WriteLine("Test Glycan Ranges Now");
            OmniFinderParameters omniFinderParameter = CheckOmniFinderRanges(glycanType, parameters.OmniFinderInput);

            #region 0.  create OmnifinderLibrary

            OmniFinderController newController = new OmniFinderController();
            OmniFinderOutput omniResults = newController.FindCompositions(omniFinderParameter);

            //Console.WriteLine(omniResults.MassAndComposition[1].MassExact.ToString());

            #endregion   

            CompareController compareHere = new CompareController();
            SetListsToCompare prepCompare = new SetListsToCompare();
            IConvert letsConvert = new Converter();
            List<int> checkLengths = new List<int>();

            int numberOfDataSetsToRun = numberOfDataSetsTotal;
            for (int i = 0; i < numberOfDataSetsToRun; i++)
            {
                //setup filename
                int dataset = i;
                int numberOfDataSets;
                SetFile2(parameters, dataset, out filenameIn, out filenameOut, out filenameOutNonGlycan, out folderIn, out folderOut, out numberOfDataSets, out glycanType, out featureType);
                filenameIn = folderIn + "\\" + filenameIn;

                #region 1.  Load data

                //load data
                List<FeatureAbstract> loadedFeatures;
                if (glycanType != LibraryType.OmniFinder)
                {
                    switch (featureType)
                    {
                        case FeatureOriginType.Viper:
                            {
                                loadedFeatures = LoadViperData(filenameIn);
                            }
                            break;
                        case FeatureOriginType.IMS:
                            {
                                loadedFeatures = LoadViperDataIMS(filenameIn);
                            }
                            break;
                            case FeatureOriginType.YAFMS_DB:
                            {
                                //not set up yet
                                loadedFeatures = LoadViperData(filenameIn);
                            }
                            break;
                        default:
                            {
                                loadedFeatures = LoadViperData(filenameIn);
                            }
                            break;
                    }
                }
                else
                {
                    featureType = FeatureOriginType.Viper;
                    FeatureAbstract massFeature = new FeatureViper();
                    massFeature.UMCMonoMW = Convert.ToDouble(folderIn);
                    massFeature.ChargeStateMax = 1;
                    loadedFeatures = new List<FeatureAbstract>();
                    loadedFeatures.Add(massFeature);
                }
                
                //Assert.AreEqual(loadedViperFeatures.Count, 10292);//for dataset 1

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
                massTolleranceVerySmall = CalculateVerySmallMassTolerance(massTolleranceVerySmall, omniFinderParameter, loadedLibrary);
                
                //store indexes in y value
                //Glycan
                List<DataSet> loadedLibraryNonGlycan;
                //NonGlycan
                List<DataSet> loadedLibraryGlycanOnly;
                bool areGlycansPresent;
                bool areNonGlycansPresent;

                CompareResultsIndexes checkindexesFromCompare = CheckLibraryAndSplitIndexes(massTolleranceVerySmall, omniFinderParameter, loadedLibrary, out loadedLibraryNonGlycan, out loadedLibraryGlycanOnly, out areGlycansPresent, out areNonGlycansPresent);


                //sort data
                List<FeatureAbstract> sortedFeatures = loadedFeatures.OrderBy(p => p.UMCMonoMW).ToList();
                List<double> sortedDataList = letsConvert.FeatureViperToMass(sortedFeatures);

                //results collection
                GlycolyzerResults glycolyzerResults = new GlycolyzerResults();
                GlycolyzerResults glycolyzerNonGlycanResults = new GlycolyzerResults();

                Console.WriteLine("Region 2 complete");
                #endregion
                
                if (areGlycansPresent)
                {
                    #region 3. Compare Residules to Glycan Library.

                    //Glycan Only
                    List<double> libraryListGlycanOnly = letsConvert.XYDataToMass(loadedLibraryGlycanOnly[0].XYList);
                    CompareInputLists inputListsGlycanOnly = prepCompare.SetThem(sortedDataList, libraryListGlycanOnly);

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
                        libraryHitsExactGlycanOnly.Add(libraryListGlycanOnly[hit]);
                    }

                    Console.WriteLine("Glycan Region 4 complete");
                    #endregion

                    if (libraryHitsExactGlycanOnly.Count > 0)
                    {
                        #region 5.  convert glycan hits to composition via OmniFinder

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

                        List<OmnifinderExactMassObject> glycanHits = new List<OmnifinderExactMassObject>();
                        for (int h = 0; h < indexesFromCompareOmni.IndexListAMatch.Count; h++)
                        {
                            glycanHits.Add(omniResults.MassAndComposition[indexesFromCompareOmni.IndexListAMatch[h]]);
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

                        #region 6a.GlycanOnly assign to results
                        glycolyzerResults.GlycanLibraryHeaders = omniResults.CompositionHeaders;
                        glycolyzerResults.GlycanHits = new List<GlycanResult>();

                        //1.  load results into GlycanResultFormat

                        //check lengths
                        checkLengths = new List<int>();
                        int keyLenght = libraryHitsExactGlycanOnly.Count;
                        checkLengths.Add(keyLenght);
                        checkLengths.Add(valuesFromCompareGlycanOnly.DataMatchingLibrary.Count);
                        checkLengths.Add(featureHitsGlycanOnly.Count);
                        checkLengths.Add(glycanHits.Count);
                        checkLengths.Add(indexesFromCompareGlycanOnly.IndexListAMatch.Count);
                        checkLengths.Add(indexesFromCompareOmni.IndexListAMatch.Count);

                        bool success = VerifytListLengths(checkLengths, keyLenght);

                        DidThisWorkUnitTest(success);

                        glycolyzerResults.GlycanUniqueHitsLibraryExactMass = libraryHitsExactGlycanOnly.Distinct().ToList();

                        for (int j = 0; j < libraryHitsExactGlycanOnly.Count; j++)
                        {
                            GlycanResult result = new GlycanResult();
                            double libraryExactMass = libraryHitsExactGlycanOnly[j];
                            result.GlycanHitsLibraryExactMass = libraryExactMass;
                            result.GlycanHitsExperimentalMass = Convert.ToDouble(valuesFromCompareGlycanOnly.DataMatchingLibrary[j]);
                            result.GlycanHitsExperimentalFeature = featureHitsGlycanOnly[j];
                            result.GlycanHitsComposition = glycanHits[j];
                            result.GlycanHitsIndexFeature = indexesFromCompareGlycanOnly.IndexListAMatch[j];
                            result.GlycanHitsIndexOmniFinder = indexesFromCompareOmni.IndexListAMatch[j];
                            glycolyzerResults.GlycanHits.Add(result);
                        }

                        //unique glycans.  Remove duplcates and save features into isomers and record indexes stored in glycolyzerResults.GlycanHits
                        List<Isomer> Isomers;
                        List<IsomerGlycanIndexes> isomerIndexes;

                        AppendFeaturesToLibrarySpineList(massTolleranceMatch, glycolyzerResults, libraryListGlycanOnly, featureHitsGlycanOnly, out Isomers, out isomerIndexes);


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
                            CheckConsisency(result, out multipleHitDivisor);//if we pass this, it assumes all the isomers are the same composition

                            if (result.GlycanPolyIsomerIndex.FeatureIndexes.Count > 0)
                            {
                                CondenseIsomers(omniResults, result, multipleHitDivisor, massTolleranceVerySmall);//pull out one value
                            }
                            glycolyzerResults.GlycanHitsInLibraryOrder.Add(result);
                        }

                        glycolyzerResults.Tollerance = massTolleranceMatch;
                        glycolyzerResults.NumberOfHits = libraryHitsExactGlycanOnly.Count;

                        Console.WriteLine(glycanHits[0].MassExact);
                        #endregion

                        Console.WriteLine("Glycan Region 6 complete");
                        #endregion

                        #region 7. WriteData

                        bool write = true;
                        if (write)
                        {
                            //ViperToIgorWriter(filenameOut, folder, loadedViperFeatures);
                            WriteToDisk(filenameOut, folderOut, featureType, omniResults, glycolyzerResults);
                            Console.WriteLine("Glycan Region 7 complete, " + folderIn + filenameOut + " was written");
                        }

                        #endregion
                    }
                }
                if (areNonGlycansPresent)
                {
                    #region 3. Compare Residules to Glycan Library.

                    //Non Glycan
                    List<double> libraryListNonGlycan = letsConvert.XYDataToMass(loadedLibraryNonGlycan[0].XYList);
                    CompareInputLists inputListsNonGlycan = prepCompare.SetThem(sortedDataList, libraryListNonGlycan);

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
                        glycolyzerNonGlycanResults.GlycanLibraryHeaders = omniResults.CompositionHeaders;//this is pulled from the glycan side
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

                    bool success = VerifytListLengths(checkLengths, keyLenght);

                    DidThisWorkUnitTest(success);

                    glycolyzerNonGlycanResults.GlycanUniqueHitsLibraryExactMass = libraryHitsExactNonGlycan.Distinct().ToList();

                    for (int j = 0; j < libraryHitsExactNonGlycan.Count; j++)
                    {
                        GlycanResult result = new GlycanResult();
                        double libraryExactMass = libraryHitsExactNonGlycan[j];
                        result.GlycanHitsLibraryExactMass = libraryExactMass;
                        result.GlycanHitsExperimentalMass = Convert.ToDouble(valuesFromCompareNonGlycan.DataMatchingLibrary[j]);
                        result.GlycanHitsExperimentalFeature = featureHitsNonGlycan[j];
                        //result.GlycanHitsComposition = new OmnifinderExactMassObject();
                        result.GlycanHitsIndexFeature = indexesFromCompareNonGlycan.IndexListAMatch[j];
                        glycolyzerNonGlycanResults.GlycanHits.Add(result);
                    }



                    if (libraryHitsExactNonGlycan.Count > 0)
                    {
                        List<Isomer> IsomersNonGlycan;
                        List<IsomerGlycanIndexes> isomerIndexesNonGlycan;
                        AppendFeaturesToLibrarySpineList(massTolleranceMatch, glycolyzerNonGlycanResults, libraryListNonGlycan, featureHitsNonGlycan, out IsomersNonGlycan, out isomerIndexesNonGlycan);


                        for (int j = 0; j < loadedLibraryNonGlycan[0].XYList.Count; j++)
                        {
                            GlycanResult result = new GlycanResult();
                            double libraryExactMass = loadedLibraryNonGlycan[0].XYList[j].X;
                            result.GlycanHitsLibraryExactMass = libraryExactMass;

                            result.GlycanPolyIsomer = IsomersNonGlycan[j];
                            result.GlycanPolyIsomerIndex = isomerIndexesNonGlycan[j];
                            //summarize common aspects of isomers

                            int multipleHitDivisor = 1;
                            CheckConsisency(result, out multipleHitDivisor);//if we pass this, it assumes all the isomers are the same composition

                            if (result.GlycanPolyIsomerIndex.FeatureIndexes.Count > 0)
                            {
                                //TODO we need to send in the divisor
                                CondenseIsomers(result, multipleHitDivisor);//pull out one value
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
                        WriteToDisk(filenameOutNonGlycan, folderOut, featureType, omniResults, glycolyzerNonGlycanResults);
                        Console.WriteLine("Non Glycan Region 7 complete, " + folderOut + filenameOut + " was written");
                    }

                    #endregion
                } 
            }
        }

        #region private methods

        public static void WriteToDisk(string filenameOut, string folderOut, FeatureOriginType featureType, OmniFinderOutput omniResults, GlycolyzerResults glycolyzerResults)
        {
            WriteGlycolyzerToDisk writer = new WriteGlycolyzerToDisk();

            string fullFileNameOut = folderOut + "\\" + filenameOut;

            writer.WriteGlycanFile(fullFileNameOut, glycolyzerResults, omniResults);
            //writer.WriteGlycanFile(filenameOut, folderOut, glycolyzerResults, omniResults);

            

            switch (featureType)
            {
                case FeatureOriginType.Viper:
                    {
                        writer.WriteViperFeatureFile(fullFileNameOut, glycolyzerResults); ;
                        //writer.WriteViperFeatureFile(filenameOut, folderOut, glycolyzerResults); ;
                    }
                    break;
                case FeatureOriginType.IMS:
                    {
                        writer.WriteIMSFeatureFile(fullFileNameOut, glycolyzerResults);
                    }
                    break;
                case FeatureOriginType.Multialign:
                    {
                        Console.WriteLine("Multialign NotImplemented yet");
                        Console.ReadKey();
                    }
                    break;
                default:
                    {
                        writer.WriteViperFeatureFile(fullFileNameOut, glycolyzerResults); ;
                    }
                    break;
            }
        }

        public static void CondenseIsomers(OmniFinderOutput omniResults, GlycanResult result, int multipleHitDivisor, double massTolleranceVerySmall)
        {
            //number of isomers
            result.GlycanHitsExperimentalNumberOfIsomers = result.GlycanPolyIsomerIndex.OmniFinderIndexes.Count/multipleHitDivisor;
            //common composition
            //find best omnifinder when multipleHitDivisor is greater than 1.  This will be hard because you will need infor from previously assigned hits.  best is not correct, we want second and third etc
            if (multipleHitDivisor == 1)
            {
                result.GlycanHitsComposition = omniResults.MassAndComposition[result.GlycanPolyIsomerIndex.OmniFinderIndexes[0]];//since all are consistant, take first one
            }
            else
            {               
                Console.WriteLine("multiplehits");
                //we need to hit the omnifinder here!... compare and contrast mass with omnifinder to select the correct composition
                CompareController compareHere = new CompareController();
                SetListsToCompare prepCompare = new SetListsToCompare();
                List<double> currentMass = new List<double>();
                List<double> libraryOmniFinder = new List<double>();
                currentMass.Add(result.GlycanHitsLibraryExactMass);
                List<OmnifinderExactMassObject> sortedOmni = omniResults.MassAndComposition.OrderBy(p => p.MassExact).ToList();
                foreach(OmnifinderExactMassObject libraryMass in sortedOmni)
                {
                    libraryOmniFinder.Add(Convert.ToDouble(libraryMass.MassExact));
                }
                CompareInputLists inputList = prepCompare.SetThem(currentMass, libraryOmniFinder);

                CompareResultsIndexes indexesFromCompareIsomer = new CompareResultsIndexes();
                CompareResultsValues valuesFromCompareIsomer = compareHere.compareFX(inputList, massTolleranceVerySmall, ref indexesFromCompareIsomer);

                if (indexesFromCompareIsomer.IndexListBMatch.Count != 1)
                {
                    Console.WriteLine("more than one hit was found (or zero hits).  not good");
                }

                result.GlycanHitsComposition = sortedOmni[indexesFromCompareIsomer.IndexListBMatch[0]];//since all are consistant, take first one
            }
            //average mass
            result.GlycanHitsExperimentalMass = AverageMassFromIsomers(result);
            //summed abundance
            result.GlycanHitsExperimentalAbundance = SummedAbundanceFromIsomers(result)/multipleHitDivisor;
            //charge state min
            result.GlycanHitsExperimentalChargeMin = AppendChargeMin(result);
            //charge state max
            result.GlycanHitsExperimentalChargeMax = AppendChargeMax(result);
            //scan min
            result.GlycanHitsExperimentalScanMin = AppendScanStart(result);
            //scan max
            result.GlycanHitsExperimentalScanMax = AppendScanEnd(result);
        }

        public static void CondenseIsomers(GlycanResult result, int multipleHitDivisor)
        {
            //number of isomers
            result.GlycanHitsExperimentalNumberOfIsomers = result.GlycanPolyIsomerIndex.OmniFinderIndexes.Count / multipleHitDivisor; 
            //average mass
            result.GlycanHitsExperimentalMass = AverageMassFromIsomers(result);
            //summed abundance
            result.GlycanHitsExperimentalAbundance = SummedAbundanceFromIsomers(result) / multipleHitDivisor;
            //charge state min
            result.GlycanHitsExperimentalChargeMin = AppendChargeMin(result);
            //charge state max
            result.GlycanHitsExperimentalChargeMax = AppendChargeMax(result);
            //scan min
            result.GlycanHitsExperimentalScanMin = AppendScanStart(result);
            //scan max
            result.GlycanHitsExperimentalScanMax = AppendScanEnd(result);
        }

        public static bool CheckConsisency(GlycanResult result, out int divisor)
        {
            bool success = false;
            divisor = 1;
            List<int>checkLengths;
            int keyLenght;
            for (int k = 0; k < result.GlycanPolyIsomer.FeatureList.Count; k++)
            {
                //check consistant
                checkLengths = new List<int>();
                keyLenght = result.GlycanPolyIsomer.IsomerCount;
                checkLengths.Add(keyLenght);
                checkLengths.Add(result.GlycanPolyIsomer.FeatureList.Count);
                checkLengths.Add(result.GlycanPolyIsomerIndex.FeatureIndexes.Count);
                checkLengths.Add(result.GlycanPolyIsomerIndex.OmniFinderIndexes.Count);

                success = VerifytListLengths(checkLengths, keyLenght);

                DidThisWorkUnitTest(success);
            }

            success = false;
            checkLengths = new List<int>();

            foreach (int omniFinderIndex in result.GlycanPolyIsomerIndex.OmniFinderIndexes)
            {
                checkLengths.Add(omniFinderIndex);
            }
            if (checkLengths.Count > 0)
            {
                keyLenght = checkLengths[0];

                success = VerifytListLengths(checkLengths, keyLenght);

                //if there is more than one library hit, we need to divide the abundance by the number of hits.  If a mass hits 2 library ions, divide mass by 2
                //example will have 2 omnifinder indexes algernating 1,2,1,2,1,2 for three isomer masses in ppm window
                if (success == false)
                {
                    //check for multiple hits
                    checkLengths.Sort();
                    //count discontinuities
                    int difference = 0;
                    double remainderForNumberOfDifferentCompositions = 1;
                    for(int i=0;i<checkLengths.Count-1;i++)
                    {
                        difference = checkLengths[i + 1] - checkLengths[i];
                        if (difference > 0)
                        {
                            divisor++;
                        }

                    }
                    //example, 6 iems in checkLenghts and divisor = 3 so 6/3=2 with no remainder
                    remainderForNumberOfDifferentCompositions = Remainder(checkLengths.Count, divisor);
                    if (remainderForNumberOfDifferentCompositions == 0)
                    {
                        success = true;
                    }
                    else
                    {
                        //since there is more than one composition involved and more than one data point involved it gets complicated
                        int featureDivisor = 1;
                        int doubleCount = 1;
                        List<int> checkLengthsFeatures = new List<int>();
                        foreach (int featureIndex in result.GlycanPolyIsomerIndex.FeatureIndexes)
                        {
                            checkLengthsFeatures.Add(featureIndex);
                        }
                        checkLengthsFeatures.Sort();

                        for (int i = 0; i < checkLengthsFeatures.Count - 1; i++)
                        {
                            difference = checkLengthsFeatures[i + 1] - checkLengthsFeatures[i];
                            if (difference > 0)
                            {
                                featureDivisor++;
                            }
                            if (difference == 0)
                            {
                                doubleCount++;
                            }

                        }
                        doubleCount--;//double count comesout one too many
                        double duplicates = checkLengthsFeatures.Count - (featureDivisor - doubleCount);
                        double remainderforMultipleDataPointsAndMultipeCompositions = 0;
                        remainderforMultipleDataPointsAndMultipeCompositions = Remainder(duplicates, divisor);
                        if (remainderforMultipleDataPointsAndMultipeCompositions == 0)
                        {
                            success = true;
                            Console.WriteLine("tricky part of divisor calculations");
                            //TODO I suppose it is possible to have triplicates and duplicates mixed up
                            //TODO we will need to correct somehow since this is a messy hybrid divisor (1,2,1,2,1,2,3,4,5,6,7 etc)  the 1,2,1,2,1,2 is easy -->divisor=2 the 1,2,3,4,5 has no divisor since it is all one composition
                            //divisor = 
                        }
                        Console.WriteLine("down the rabit hole");
                    }

                    if (divisor == 100000)
                    {
                        Console.ReadKey();
                    }
                }
                DidThisWorkUnitTest(success);
            }
            else
            {  
                success = true;
            }
            return success;
        }

        private static double Remainder(double numerator, int denomenator)
        {
            double remainder = 0;
            remainder = numerator % denomenator;
            remainder = remainder / denomenator;
            return remainder;
        }

        public static void DidThisWorkUnitTest(bool success)
        {
            if (success == false)
            {
                //Assert.IsTrue(success);
            }
            //Assert.IsTrue(success);
        }

        public static bool VerifytListLengths(List<int> checkLengths, int keyLenght)
        {
            bool acceptable = false;
            int success = 1;
            
            foreach (int length in checkLengths)
            {
                if (length != keyLenght)
                {
                    success = -1;
                }
            }

            if (success == 1)
            {
                acceptable = true;
            }
            else
            {
                acceptable = false;
            }

            return acceptable;
        }

        private static void SplitToGlycanAndNonGlycan(List<DataSet> loadedLibrary, CompareResultsIndexes checkindexesFromCompare, out DataSet loadedLibraryNonGlycan, out DataSet loadedLibraryGlycanOnly)
        {
            loadedLibraryNonGlycan = new DataSet();
            if (checkindexesFromCompare.IndexListBandNotA.Count > 0)
            {
                //You have masses that are not glycans in the library
                foreach (int index in checkindexesFromCompare.IndexListBandNotA)
                {
                    
                    //Console.WriteLine(loadedLibrary[0].XYList[index].X.ToString().ToString() + " is not a glycan");
                    
                    //loadedLibraryNonGlycan.XYList.Add(loadedLibrary[0].XYList[index]);

                    XYData point = new XYData(loadedLibrary[0].XYList[index].X, index);
                    loadedLibraryNonGlycan.XYList.Add(point);
                }

            }

            loadedLibraryGlycanOnly = new DataSet();
            if (checkindexesFromCompare.IndexListBMatch.Count > 0)
            {
                //You have masses that are not glycans in the library
                foreach (int index in checkindexesFromCompare.IndexListBMatch)
                {
                    //Console.WriteLine(loadedLibrary[0].XYList[index].X.ToString() + " is a glycan");
                    //loadedLibraryGlycanOnly.XYList.Add(loadedLibrary[0].XYList[index]);

                    XYData point = new XYData(loadedLibrary[0].XYList[index].X, index);
                    loadedLibraryGlycanOnly.XYList.Add(point);
                }

            }
        }

        public static double CalculateVerySmallMassTolerance(double massTolleranceVerySmall, OmniFinderParameters omniFinderParameter, List<DataSet> loadedLibrary)
        {
            while (massTolleranceVerySmall > 0.000001)
            {

                CompareResultsValues checkValuesFromCompareOmni = VerifyLibraryPassesOmniFinder(massTolleranceVerySmall, omniFinderParameter, loadedLibrary);

                if (checkValuesFromCompareOmni.LibraryNotMatched.Count > 0)
                {
                    //You have masses that are not glycans in the library
                    foreach (double rejectMass in checkValuesFromCompareOmni.LibraryNotMatched)
                    {
                        Console.WriteLine(rejectMass.ToString() + " is not a glycan");
                    }

                }


                if (checkValuesFromCompareOmni.LibraryMatches.Count > loadedLibrary[0].XYList.Count - checkValuesFromCompareOmni.LibraryNotMatched.Count)
                {
                    Console.WriteLine("the ppm (" + massTolleranceVerySmall.ToString() + ") is too large to create a one-to-one relationship.  There are " + (checkValuesFromCompareOmni.LibraryMatches.Count - loadedLibrary[0].XYList.Count).ToString() + " extra masses");
                    massTolleranceVerySmall = massTolleranceVerySmall / 10;

                }
                else
                {
                    Console.WriteLine("One-To-One relationship achieved at " + massTolleranceVerySmall.ToString() + " ppm");
                    break;
                }
            }
            if (massTolleranceVerySmall < 0.000001)
            {
                //the list cannot be converted to compsoitions because the mass tollerance Very small is too small
                Console.ReadKey();
            }

            return massTolleranceVerySmall;
        }

        public static CompareResultsIndexes CheckLibraryAndSplitIndexes(double massTolleranceVerySmall, OmniFinderParameters omniFinderParameter, List<DataSet> loadedLibrary, out List<DataSet> loadedLibraryNonGlycan, out List<DataSet> loadedLibraryGlycanOnly, out bool areGlycansPresent , out bool areNonGlycansPresent)
        {
            OmniFinderController newController = new OmniFinderController();
            OmniFinderOutput omniResults = newController.FindCompositions(omniFinderParameter);

            Console.WriteLine(omniResults.MassAndComposition[1].MassExact.ToString());
            IConvert letsConvertOmni = new Converter();

            //data
            List<decimal> checkDataListOmni = new List<decimal>();
            foreach (OmnifinderExactMassObject exactMass in omniResults.MassAndComposition)
            {
                checkDataListOmni.Add((exactMass.MassExact));
            }

            CompareController compareHere = new CompareController();
            SetListsToCompare prepCompare = new SetListsToCompare();
            CompareInputLists checkInputLists = prepCompare.SetThem(checkDataListOmni, letsConvertOmni.ListDoubleToListDecimal(letsConvertOmni.XYDataToMass(loadedLibrary[0].XYList)));

            CompareResultsIndexes checkindexesFromCompare = new CompareResultsIndexes();
            CompareResultsValues checkValuesFromCompare = compareHere.compareFX(checkInputLists, massTolleranceVerySmall, ref checkindexesFromCompare);

            //parse library into glycans and non-glycans
            DataSet libraryNonGlycan;
            DataSet libraryGlycanOnly;
            SplitToGlycanAndNonGlycan(loadedLibrary, checkindexesFromCompare, out libraryNonGlycan, out libraryGlycanOnly);
            loadedLibraryNonGlycan = new List<DataSet>();
            loadedLibraryGlycanOnly = new List<DataSet>();
            loadedLibraryNonGlycan.Add(libraryNonGlycan);
            loadedLibraryGlycanOnly.Add(libraryGlycanOnly);

            areGlycansPresent = false;
            areNonGlycansPresent = false;
            if (loadedLibraryNonGlycan[0].XYList.Count > 0)
            {
                areNonGlycansPresent = true;
            }
            if (loadedLibraryGlycanOnly[0].XYList.Count > 0)
            {
                areGlycansPresent = true;
            }

            return checkindexesFromCompare;
        }

        private static double AverageMassFromIsomers(GlycanResult result)
        {
            double assignedMass = 0;
            double sum = 0;
            if (result.GlycanPolyIsomer.FeatureList.Count > 0)
            {
                foreach (FeatureAbstract isomerFeature in result.GlycanPolyIsomer.FeatureList)
                {
                    sum += isomerFeature.UMCMonoMW;
                }

                assignedMass = (sum / result.GlycanPolyIsomer.FeatureList.Count);//average
            }
            else
            {
                assignedMass = 0;
            }
            return assignedMass;
        }

        private static double SummedAbundanceFromIsomers(GlycanResult result)
        {
            double assignedAbundance = 0;
            double sum = 0;
            if (result.GlycanPolyIsomer.FeatureList.Count > 0)
            {
                foreach (FeatureAbstract isomerFeature in result.GlycanPolyIsomer.FeatureList)
                {
                    sum += isomerFeature.UMCAbundance;
                }

                assignedAbundance = sum;//sum
            }
            else
            {
                assignedAbundance = 0;
            }
            return assignedAbundance;
        }

        public static void AppendFeaturesToLibrarySpineList(double massTolleranceMatch, GlycolyzerResults glycolyzerResults, List<double> LibraryList, List<FeatureAbstract> featureHits, out List<Isomer> appendedGlycanFeatures, out List<IsomerGlycanIndexes> appendedIsomerIndexes)
        {
            appendedGlycanFeatures = new List<Isomer>();
            appendedIsomerIndexes = new List<IsomerGlycanIndexes>();
            List<double> indexesOfHits = new List<double>();
            CompareController compareHere = new CompareController();
            SetListsToCompare prepCompare = new SetListsToCompare();
            IConvert letsConvert = new Converter();

            CompareInputLists inputList;

            for (int i = 0; i < LibraryList.Count; i++)
            {
                double libraryMass = LibraryList[i];

                
                List<double> libraryCompare = new List<double>();
                libraryCompare.Add(libraryMass);

                List<double> dataCompareIsomers = new List<double>();
                foreach (FeatureAbstract feature in featureHits)
                {
                    dataCompareIsomers.Add(feature.UMCMonoMW);
                }

                inputList = prepCompare.SetThem(dataCompareIsomers, libraryCompare);

                CompareResultsIndexes indexesFromCompareIsomer = new CompareResultsIndexes();
                CompareResultsValues valuesFromCompareIsomer = compareHere.compareFX(inputList, massTolleranceMatch, ref indexesFromCompareIsomer);

                if (indexesFromCompareIsomer.IndexListAMatch.Count > 0)
                {
                    appendedGlycanFeatures.Add(new Isomer());
                    appendedIsomerIndexes.Add(new IsomerGlycanIndexes());

                    foreach (int index in indexesFromCompareIsomer.IndexListAMatch)
                    {
                        appendedGlycanFeatures[i].FeatureList.Add(featureHits[index]);
                        appendedGlycanFeatures[i].IsomerCount++;

                        //appendedIsomerIndexes[i].FeatureIndexes.Add(counterI);
                        appendedIsomerIndexes[i].FeatureIndexes.Add(glycolyzerResults.GlycanHits[index].GlycanHitsIndexFeature);
                        appendedIsomerIndexes[i].OmniFinderIndexes.Add(glycolyzerResults.GlycanHits[index].GlycanHitsIndexOmniFinder);
                    }
                }
                else
                {
                    appendedGlycanFeatures.Add(new Isomer());
                    appendedIsomerIndexes.Add(new IsomerGlycanIndexes());
                }
            }
            #region old
            //int counter = 0;//0 to GlycanUniqueHitsLibraryExactMass.Count
            //int counterI = 0;//0 to featureHits.Count

            //double test;
            //double high;
            //double low;
            //for (int i = 0; i < LibraryList.Count; i++)
            //{
            //    double libraryMass = LibraryList[i];

            //    if (counter < glycolyzerResults.GlycanUniqueHitsLibraryExactMass.Count)
            //    {
            //        test = glycolyzerResults.GlycanUniqueHitsLibraryExactMass[counter];
            //        high = libraryMass + ErrorCalculator.PPMtoDaTollerance(massTolleranceMatch, libraryMass);
            //        low = libraryMass - ErrorCalculator.PPMtoDaTollerance(massTolleranceMatch, libraryMass);

            //        if (test < high && test > low)
            //        {
            //            appendedGlycanFeatures.Add(new Isomer());
            //            appendedIsomerIndexes.Add(new IsomerGlycanIndexes());

            //            while (test < high && test > low)
            //            {
            //                if (counterI < featureHits.Count)
            //                {
            //                    appendedGlycanFeatures[i].FeatureList.Add(featureHits[counterI]);
            //                    appendedGlycanFeatures[i].IsomerCount++;

            //                    //appendedIsomerIndexes[i].FeatureIndexes.Add(counterI);
            //                    appendedIsomerIndexes[i].FeatureIndexes.Add(glycolyzerResults.GlycanHits[counterI].GlycanHitsIndexFeature);
            //                    appendedIsomerIndexes[i].OmniFinderIndexes.Add(glycolyzerResults.GlycanHits[counterI].GlycanHitsIndexOmniFinder);

            //                    counterI++;
            //                    if (counterI < featureHits.Count)
            //                    {
            //                        test = featureHits[counterI].UMCMonoMW;//this isi where it fails
            //                    }
            //                }
            //                else//for last point
            //                {
            //                    break;
            //                }
            //            }
            //            counter++;
            //        }
            //        else
            //        {
            //            appendedGlycanFeatures.Add(new Isomer());
            //            appendedIsomerIndexes.Add(new IsomerGlycanIndexes());
            //        }
            //    }
            //    else
            //    {
            //        appendedGlycanFeatures.Add(new Isomer());
            //        appendedIsomerIndexes.Add(new IsomerGlycanIndexes());
            //    }
            //}
            #endregion
        }

        private static int AppendChargeMin(GlycanResult result)
        {
            int assigned = 0;
            int sum = 0;
            if (result.GlycanPolyIsomer.FeatureList.Count > 0)
            {
                sum = result.GlycanPolyIsomer.FeatureList[0].ChargeStateMin;
                foreach (FeatureAbstract isomerFeature in result.GlycanPolyIsomer.FeatureList)
                {
                    if (isomerFeature.ChargeStateMin < sum)
                    {
                        sum = isomerFeature.ChargeStateMin;
                    }
                }
                assigned =sum;
            }
            else
            {
                assigned = 0;
            }
            return assigned;
        }

        private static int AppendChargeMax(GlycanResult result)
        {
            int assigned = 0;
            int sum = 0;
            if (result.GlycanPolyIsomer.FeatureList.Count > 0)
            {
                sum = result.GlycanPolyIsomer.FeatureList[0].ChargeStateMax;
                foreach (FeatureAbstract isomerFeature in result.GlycanPolyIsomer.FeatureList)
                {
                    if (isomerFeature.ChargeStateMax > sum)
                    {
                        sum = isomerFeature.ChargeStateMax;
                    }
                }
                assigned = sum;
            }
            else
            {
                assigned = 0;
            }
            return assigned;
        }

        private static int AppendScanStart(GlycanResult result)
        {
            int assigned = 0;
            int sum = 0;
            if (result.GlycanPolyIsomer.FeatureList.Count > 0)
            {
                sum = result.GlycanPolyIsomer.FeatureList[0].ScanStart;
                foreach (FeatureAbstract isomerFeature in result.GlycanPolyIsomer.FeatureList)
                {
                    if (isomerFeature.ScanStart < sum)
                    {
                        sum = isomerFeature.ScanStart;
                    }
                }
                assigned = sum;
            }
            else
            {
                assigned = 0;
            }
            return assigned;
        }

        private static int AppendScanEnd(GlycanResult result)
        {
            int assigned = 0;
            int sum = 0;
            if (result.GlycanPolyIsomer.FeatureList.Count > 0)
            {
                sum = result.GlycanPolyIsomer.FeatureList[0].ScanEnd;
                foreach (FeatureAbstract isomerFeature in result.GlycanPolyIsomer.FeatureList)
                {
                    if (isomerFeature.ScanEnd > sum)
                    {
                        sum = isomerFeature.ScanEnd;
                    }
                }
                assigned = sum;
            }
            else
            {
                assigned = 0;
            }
            return assigned;
        }

        private static void ViperToIgorWriter(string filenameOut, string folder, List<FeatureAbstract> loadedViperFeatures)
        {
            List<string> dataToWrite = new List<string>();
            foreach (FeatureAbstract feature in loadedViperFeatures)
            {
                string line = feature.UMCMonoMW + "\t" + feature.ScanStart;
                dataToWrite.Add(line);
            }

            StringListToDisk writer = new StringListToDisk();
            writer.toDiskStringList(folder + filenameOut, dataToWrite, "0\t0");
        }

        private static List<FeatureAbstract> LoadViperData(string filenameIn)
        {
            ILoadFile loader = new LoadOptions();
            string columnheaders;

            List<FeatureViper> loadedViperFeatures = loader.LoadFeatureViper(filenameIn, out columnheaders);

            List<FeatureAbstract> loadedFeaturesAbstract = new List<FeatureAbstract>();
            foreach(FeatureViper featureViper in loadedViperFeatures)
            {
                loadedFeaturesAbstract.Add(featureViper);
            }
            return loadedFeaturesAbstract;
        }

        private static List<FeatureAbstract> LoadYAFMSDBData(string filenameIn)
        {
            Console.WriteLine("Not setup yet");
            Console.ReadKey();
            
            ILoadFile loader = new LoadOptions();
            string columnheaders;

            List<FeatureViper> loadedViperFeatures = loader.LoadFeatureViper(filenameIn, out columnheaders);

            List<FeatureAbstract> loadedFeaturesAbstract = new List<FeatureAbstract>();
            foreach (FeatureViper featureViper in loadedViperFeatures)
            {
                loadedFeaturesAbstract.Add(featureViper);
            }
            return loadedFeaturesAbstract;
        }

        private static List<FeatureAbstract> LoadViperDataIMS(string filenameIn)
        {
            ILoadFile loader = new LoadOptions();
            string columnheaders;

            List<FeatureIMS> loadedIMSFeatures = loader.LoadFeatureIMS(filenameIn, out columnheaders);

            //ConvertFeatureIMSToFeatureViper converter = new ConvertFeatureIMSToFeatureViper();
            //List<FeatureIMS> loadedViperFeatures = converter.Convert(loadedIMSFeatures);

            List<FeatureAbstract> loadedFeaturesAbstract = new List<FeatureAbstract>();
            foreach (FeatureIMS feature in loadedIMSFeatures)
            {
                loadedFeaturesAbstract.Add(feature);
            }
            return loadedFeaturesAbstract;
        }

        private static CompareResultsValues VerifyLibraryPassesOmniFinder(double massTolleranceVerySmall, OmniFinderParameters omniFinderParameter, List<DataSet> loadedLibrary)
        {
            OmniFinderController newController = new OmniFinderController();
            OmniFinderOutput omniResults = newController.FindCompositions(omniFinderParameter);

            Console.WriteLine(omniResults.MassAndComposition[1].MassExact.ToString());
            IConvert letsConvertOmni = new Converter();

            //data
            List<double> checkDataListOmni = new List<double>();
            foreach (OmnifinderExactMassObject exactMass in omniResults.MassAndComposition)
            {
                checkDataListOmni.Add(Convert.ToDouble(exactMass.MassExact));
            }
            IConvert checkLetsConvertOmni = new Converter();
            //library
            List<double> checkLibraryListOmni = checkLetsConvertOmni.XYDataToMass(loadedLibrary[0].XYList);

            SetListsToCompare checkPrepCompareOmni = new SetListsToCompare();
            CompareInputLists checkInputListsOmni = checkPrepCompareOmni.SetThem(checkDataListOmni, checkLibraryListOmni);

            CompareController checkCompareHereOmni = new CompareController();
            CompareResultsIndexes checkIndexesFromCompareOmni = new CompareResultsIndexes();
            CompareResultsValues checkValuesFromCompareOmni = checkCompareHereOmni.compareFX(checkInputListsOmni, massTolleranceVerySmall, ref checkIndexesFromCompareOmni);
            return checkValuesFromCompareOmni;
        }


        #region set parameters

        private static void SetFile2(GlycolyzerParametersGUI parameters, int datasetToUse, out string filenameIn, out string filenameOutGlycan, out string filenameOutNonGlycan, out string folderIn, out string folderOut, out int numberofDataSets, out LibraryType glycanType, out FeatureOriginType featureType)
        {
            List<string> featureFilenamesPile = new List<string>();
            List<string> outFileNamePile = new List<string>();

            List<string> baseFileNames = new List<string>();

            if (parameters.LibraryTypeInput == LibraryType.OmniFinder)
            {
                baseFileNames.Add("OmniFinder_" + parameters.MassErrorPPM.ToString() + "_ppm");
                folderIn = parameters.FolderIn;//this is the unknown mass or a list of masses?  TODO
            }
            else
            {
                foreach (string fileName in parameters.FilesIn)
                {
                    string filenamesingle = ProcessFileName(fileName, parameters.FolderIn);
                    baseFileNames.Add(filenamesingle);
                }
                folderIn = parameters.FolderIn;
            }
            //baseFileName = System.Text.RegularExpressions.Regex.Replace(baseFileName, repacement, "");
           
            //folderIn = parameters.FolderIn;
            folderOut = parameters.FolderOut;

            featureFilenamesPile = baseFileNames;
            outFileNamePile = baseFileNames;
            numberofDataSets = featureFilenamesPile.Count;



            filenameIn = featureFilenamesPile[datasetToUse] + ".txt";
            //filenameIn = featureFilenamesPile[datasetToUse];
            filenameOutGlycan = System.Text.RegularExpressions.Regex.Replace(outFileNamePile[datasetToUse], ".txt$", "") + "_" + parameters.LibraryTypeInput.ToString() + "_Glycan.txt";
            filenameOutNonGlycan = System.Text.RegularExpressions.Regex.Replace(outFileNamePile[datasetToUse], ".txt$", "") + "_" + parameters.LibraryTypeInput.ToString() + "_NonGlycan.txt";

            glycanType = parameters.LibraryTypeInput;
            featureType = parameters.FeatureOriginTypeInput;
        }

        private static string ProcessFileName(string fileIN, string folderIN)
        {
            //int count  = charactersFile.Count(char.IsLetter);

            string baseFileName = fileIN;
            //List<char> charactersFile = new List<char>();
            //foreach (char letter in fileIN)
            //{
            //    charactersFile.Add(letter);
            //}

            //List<char> charactersFolder = new List<char>();
            //foreach (char letter in folderIN)
            //{
            //    charactersFolder.Add(letter);
            //}

            //string baseFileName = "";
            //for (int i = charactersFolder.Count + 1; i < charactersFile.Count; i++)
            //{

            //    baseFileName += charactersFile[i].ToString();
            //    if (Char.Equals(charactersFile[i], '\\'))
            //    {
            //        i++;
            //    }
            //}

            baseFileName = System.Text.RegularExpressions.Regex.Replace(baseFileName, ".txt$", "");

            return baseFileName;
        }

        private static List<DataSet> loadedLibraryData(LibraryType whichLibrary)
        {
            List<DataSet> loadedLibrary = new List<DataSet>();
            InputOutputFileName library = new InputOutputFileName();

            switch (whichLibrary)
            {
                case LibraryType.Ant_Alditol:
                    {
                        //library.InputFileName = @"D:\Csharp\Libraries\L_AntLibraryDirectoryWorkAlditol.txt";//*
                        library.InputFileName = @"D:\Csharp\Libraries\L_AntLibraryDirectoryWorkAlditol_Xylose.txt";//*3 xyloses will be isobar to 
                    }
                    break;
                case LibraryType.Cell_Alditol:
                    {
                        library.InputFileName = @"D:\Csharp\Libraries\L_CellLibraryDirectoryWorkAlditol.txt";//*
                    }
                    break;
                case LibraryType.Cell_Alditol_V2:
                    {
                        library.InputFileName = @"D:\Csharp\Libraries\L_CellLibraryDirectoryWorkAlditolV2.txt";//*
                    }
                    break;
                case LibraryType.Cell_Alditol_Vmini:
                    {
                        library.InputFileName = @"D:\Csharp\Libraries\L_CellLibraryDirectoryWorkAlditolVmini.txt";//*
                    }
                    break;
                case LibraryType.NLinked_Aldehyde:
                    {
                        library.InputFileName = @"D:\Csharp\Libraries\L_6LibraryDirectoryWorkAldehyde.txt";
                    }
                    break;
                case LibraryType.NLinked_Alditol:
                    {
                        library.InputFileName = @"D:\Csharp\Libraries\L_7LibraryDirectoryWorkAlditol.txt";//*
                        //library.InputFileName = @"L:\PNNL Files\CSharp\Libraries\L_7LibraryDirectoryHomeAlditol.txt";//*
                    }
                    break;
                case LibraryType.NLinked_Alditol_2ndIsotope:
                    {
                        library.InputFileName = @"D:\Csharp\Libraries\L_7LibraryDirectoryWorkAlditol+2nsIsotope.txt";//*
                        //library.InputFileName = @"L:\PNNL Files\CSharp\Libraries\L_7LibraryDirectoryHomeAlditol.txt";//*
                    }
                    break;
                case LibraryType.NonCalibrated:
                    {
                        library.InputFileName = @"D:\PNNL\Projects\DATA 2012_01_11 SPIN QTOF3\1_DeconTools Sum 5 scans yes-calibrated\L_CalibrationDirectoryWorkAlditolShift.txt";//*
                        //library.InputFileName = @"L:\PNNL Files\CSharp\Libraries\L_7LibraryDirectoryHomeAlditol.txt";//*
                    }
                    break;
                case LibraryType.NLinked_Alditol_PolySA:
                    {
                        //library.InputFileName = @"D:\Csharp\Libraries\L_7LibraryDirectoryWorkAlditol+SA8.txt";//*
                        //library.InputFileName = @"L:\PNNL Files\CSharp\Libraries\L_7LibraryDirectoryHomeAlditol+SA8.txt";//*
                        library.InputFileName = @"D:\Csharp\Libraries\L_7LibraryDirectoryAlditol+SA16 Work.txt";//*
                    }
                    break;
                case LibraryType.NLinked_Alditol8:
                    {
                        library.InputFileName = @"D:\Csharp\Libraries\L_8LibraryDirectoryWorkAlditol.txt";//*
                        //library.InputFileName = @"L:\PNNL Files\CSharp\Libraries\L_7LibraryDirectoryHomeAlditol.txt";//*
                    }
                    break;
                case LibraryType.NLinked_Alditol9:
                    {
                        library.InputFileName = @"D:\Csharp\Libraries\L_9LibraryDirectoryAlditol Work.txt";//*
                        //library.InputFileName = @"L:\PNNL Files\CSharp\Libraries\L_9LibraryDirectoryAlditol Home.txt";//*
                    }
                    break;
                case LibraryType.NLinked_Alditol10:
                    {
                        library.InputFileName = @"D:\Csharp\Libraries\L_10LibraryDirectoryAlditol Work.txt";//*
                        //library.InputFileName = @"L:\PNNL Files\CSharp\Libraries\L_10LibraryDirectoryAlditol Home.txt";//*
                    }
                    break;
                case LibraryType.Hexose:
                    {
                        library.InputFileName = @"D:\Csharp\Libraries\L_HexoseDirectoryWork.txt";//*

                    }
                    break;
                default:
                    {
                        Console.WriteLine("missing glycan type for this library");
                        Console.ReadKey();
                    }
                    break;
            }

            GetDataController newFetcher = new GetDataController();
            loadedLibrary = newFetcher.GetDataLibrary(library);
            return loadedLibrary;
        }

        public static OmniFinderParameters CheckOmniFinderRanges(LibraryType whichCarbType, OmniFinderParameters omniFinderParameterIn)
        {
            OmniFinderParameters omniFinderParameter = new OmniFinderParameters();

            int checkCount = 0;

            switch (whichCarbType)
            {
                case LibraryType.Ant_Alditol:
                    {
                        #region inside

                        #region force ranges for default libraries

                        omniFinderParameter.CarbohydrateType = OmniFinder.Objects.Enumerations.CarbType.Alditol;

                        BuildingBlockMonoSaccharide hexose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Hexose, new RangesMinMax(0, 40));
                        BuildingBlockMonoSaccharide hexNac = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.NAcetylhexosamine, new RangesMinMax(0, 9));
                        BuildingBlockMonoSaccharide fucose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Deoxyhexose, new RangesMinMax(0, 5));
                        BuildingBlockMonoSaccharide neuAc = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.NeuraminicAcid, new RangesMinMax(0, 4));//21 for PSAX16
                        BuildingBlockMonoSaccharide penotose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Pentose, new RangesMinMax(0, 2));//for xylose library  3 xylos will cause problems with the lowest mass needed to seperate because the masses have isobars
                        
                        List<BuildingBlockMonoSaccharide> forcedMonosaccharideList = new List<BuildingBlockMonoSaccharide>();
                        List<BuildingBlockMiscellaneousMatter> forcedMatterList = new List<BuildingBlockMiscellaneousMatter>();

                        forcedMonosaccharideList.Add(hexose);//hexose
                        forcedMonosaccharideList.Add(hexNac);//hexnac
                        forcedMonosaccharideList.Add(fucose);//fucose
                        forcedMonosaccharideList.Add(neuAc);//neuac
                        forcedMonosaccharideList.Add(penotose);
                        //forcedMatterList.Add(nah);

                        omniFinderParameterIn.BuildingBlocksMonosacchcarides = forcedMonosaccharideList;
                        omniFinderParameterIn.BuildingBlocksMiscellaneousMatter = forcedMatterList;
                        #endregion
                        
                        List<RangesMinMax> testRanges = new List<RangesMinMax>();
                        RangesMinMax range1 = new RangesMinMax(0, 40);
                        RangesMinMax range2 = new RangesMinMax(0, 9);
                        RangesMinMax range3 = new RangesMinMax(0, 5);
                        RangesMinMax range4 = new RangesMinMax(0, 4);
                        RangesMinMax range5 = new RangesMinMax(0, 2);
                        testRanges.Add(range1);
                        testRanges.Add(range2);
                        testRanges.Add(range3);
                        testRanges.Add(range4);
                        testRanges.Add(range5);

                        OmnifinderCheckObject checkThese = new OmnifinderCheckObject();
                        checkThese.holdBuildingBlockAminoAcid = 0;
                        checkThese.holdBuildingBlockCrossRing = 0;
                        checkThese.holdBuildingBlockElements = 0;
                        checkThese.holdBuildingBlockMiscellaneousMatter = 0;
                        checkThese.holdBuildingBlockMonosaccharides = 5;
                        checkThese.holdBuildingSubAtomicParticles = 0;
                        checkThese.holdBuildingBlockUserUnit = 0;
                        checkThese.holdAdduct = Adducts.Neutral;
                        checkThese.holdCarbType = CarbType.Alditol;

                        checkThese.testRanges = testRanges;

                        checkCount = CheckLibrary(omniFinderParameterIn, checkThese);

                        #endregion
                    }
                    break;
                case LibraryType.Cell_Alditol:
                    {
                        omniFinderParameter.CarbohydrateType = OmniFinder.Objects.Enumerations.CarbType.Alditol;
                    }
                    break;
                case LibraryType.Cell_Alditol_V2:
                    {
                        #region inside

                        #region force ranges for default libraries

                        omniFinderParameter.CarbohydrateType = OmniFinder.Objects.Enumerations.CarbType.Alditol;
                        BuildingBlockMonoSaccharide hexose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Hexose, new RangesMinMax(2, 12));
                        //BuildingBlockMonoSaccharide hexose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Hexose, new RangesMinMax(0, 40));//for hexose library
                        BuildingBlockMonoSaccharide hexNac = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.NAcetylhexosamine, new RangesMinMax(0, 9));
                        BuildingBlockMonoSaccharide fucose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Deoxyhexose, new RangesMinMax(0, 5));
                        BuildingBlockMonoSaccharide neuAc = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.NeuraminicAcid, new RangesMinMax(0, 5));//21 for PSAX16
                        //BuildingBlockMonoSaccharide penotose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Pentose, new RangesMinMax(0, 3));//for xylose library
                        //BuildingBlockMiscellaneousMatter nah = new BuildingBlockMiscellaneousMatter(PNNLOmics.Data.Constants.MiscellaneousMatterName.NaMinusH, new RangesMinMax(0, 5));

                        List<BuildingBlockMonoSaccharide> forcedMonosaccharideList = new List<BuildingBlockMonoSaccharide>();
                        List<BuildingBlockMiscellaneousMatter> forcedMatterList = new List<BuildingBlockMiscellaneousMatter>();

                        forcedMonosaccharideList.Add(hexose);//hexose
                        forcedMonosaccharideList.Add(hexNac);//hexnac
                        forcedMonosaccharideList.Add(fucose);//fucose
                        forcedMonosaccharideList.Add(neuAc);//neuac
                        //forcedMonosaccharideList.Add(penotose);
                        //forcedMatterList.Add(nah);

                        omniFinderParameterIn.BuildingBlocksMonosacchcarides = forcedMonosaccharideList;
                        omniFinderParameterIn.BuildingBlocksMiscellaneousMatter = forcedMatterList;
                        #endregion

                        List<RangesMinMax> testRanges = new List<RangesMinMax>();
                        //testRanges.Add(new RangesMinMax(0, 5));
                        testRanges.Add(new RangesMinMax(2, 12));
                        testRanges.Add(new RangesMinMax(0, 9));
                        testRanges.Add(new RangesMinMax(0, 5));
                        testRanges.Add(new RangesMinMax(0, 5));

                        OmnifinderCheckObject checkThese = new OmnifinderCheckObject();
                        checkThese.holdBuildingBlockAminoAcid = 0;
                        checkThese.holdBuildingBlockCrossRing = 0;
                        checkThese.holdBuildingBlockElements = 0;
                        checkThese.holdBuildingBlockMiscellaneousMatter = 0;
                        checkThese.holdBuildingBlockMonosaccharides = 4;
                        checkThese.holdBuildingSubAtomicParticles = 0;
                        checkThese.holdBuildingBlockUserUnit = 0;
                        checkThese.holdAdduct = Adducts.Neutral;
                        checkThese.holdCarbType = CarbType.Alditol;

                        checkThese.testRanges = testRanges;

                        checkCount = CheckLibrary(omniFinderParameterIn, checkThese);

                        #endregion
                    }
                    break;
                case LibraryType.Cell_Alditol_Vmini:
                    {
                        omniFinderParameter.CarbohydrateType = OmniFinder.Objects.Enumerations.CarbType.Alditol;
                    }
                    break;
                case LibraryType.NLinked_Aldehyde:
                    {
                        omniFinderParameter.CarbohydrateType = OmniFinder.Objects.Enumerations.CarbType.Aldehyde;
                    }
                    break;
                case LibraryType.NLinked_Alditol:
                    {
                        #region inside

                        #region force ranges for default libraries
                        omniFinderParameter.CarbohydrateType = OmniFinder.Objects.Enumerations.CarbType.Alditol;

                        BuildingBlockMonoSaccharide hexose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Hexose, new RangesMinMax(3, 12));
                        //BuildingBlockMonoSaccharide hexose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Hexose, new RangesMinMax(0, 40));//for hexose library
                        BuildingBlockMonoSaccharide hexNac = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.NAcetylhexosamine, new RangesMinMax(2, 9));
                        BuildingBlockMonoSaccharide fucose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Deoxyhexose, new RangesMinMax(0, 5));
                        BuildingBlockMonoSaccharide neuAc = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.NeuraminicAcid, new RangesMinMax(0, 5));//21 for PSAX16
                        //BuildingBlockMonoSaccharide penotose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Pentose, new RangesMinMax(0, 3));//for xylos library
                        BuildingBlockMiscellaneousMatter nah = new BuildingBlockMiscellaneousMatter(PNNLOmics.Data.Constants.MiscellaneousMatterName.NaMinusH, new RangesMinMax(0, 5));

                        List<BuildingBlockMonoSaccharide> forcedMonosaccharideList = new List<BuildingBlockMonoSaccharide>();
                        List<BuildingBlockMiscellaneousMatter> forcedMatterList = new List<BuildingBlockMiscellaneousMatter>();
                        
                        forcedMonosaccharideList.Add(hexose);//hexose
                        forcedMonosaccharideList.Add(hexNac);//hexnac
                        forcedMonosaccharideList.Add(fucose);//fucose
                        forcedMonosaccharideList.Add(neuAc);//neuac
                        //forcedMonosaccharideList.Add(penotose);
                        forcedMatterList.Add(nah);

                        omniFinderParameterIn.BuildingBlocksMonosacchcarides = forcedMonosaccharideList;
                        omniFinderParameterIn.BuildingBlocksMiscellaneousMatter = forcedMatterList;
                        #endregion

                        List<RangesMinMax> testRanges = new List<RangesMinMax>();
                       
                        testRanges.Add(new RangesMinMax(3, 12));
                        testRanges.Add(new RangesMinMax(2, 9));
                        testRanges.Add(new RangesMinMax(0, 5));
                        testRanges.Add(new RangesMinMax(0, 5));
                        testRanges.Add(new RangesMinMax(0, 5));

                        OmnifinderCheckObject checkThese = new OmnifinderCheckObject();
                        checkThese.holdBuildingBlockAminoAcid = 0;
                        checkThese.holdBuildingBlockCrossRing = 0;
                        checkThese.holdBuildingBlockElements = 0;
                        checkThese.holdBuildingBlockMiscellaneousMatter = 1;
                        checkThese.holdBuildingBlockMonosaccharides = 4;
                        checkThese.holdBuildingSubAtomicParticles = 0;
                        checkThese.holdBuildingBlockUserUnit = 0;
                        checkThese.holdAdduct = Adducts.Neutral;
                        checkThese.holdCarbType = CarbType.Alditol;

                        checkThese.testRanges = testRanges;

                        checkCount = CheckLibrary(omniFinderParameterIn, checkThese);

                        #endregion
                    }
                    break;
                case LibraryType.NLinked_Alditol_PolySA:
                    {
                        #region inside

                        #region force ranges for default libraries
                        omniFinderParameter.CarbohydrateType = OmniFinder.Objects.Enumerations.CarbType.Alditol;

                        BuildingBlockMonoSaccharide hexose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Hexose, new RangesMinMax(3, 12));
                        //BuildingBlockMonoSaccharide hexose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Hexose, new RangesMinMax(0, 40));//for hexose library
                        BuildingBlockMonoSaccharide hexNac = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.NAcetylhexosamine, new RangesMinMax(2, 9));
                        BuildingBlockMonoSaccharide fucose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Deoxyhexose, new RangesMinMax(0, 5));
                        BuildingBlockMonoSaccharide neuAc = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.NeuraminicAcid, new RangesMinMax(0, 21));//21 for PSAX16
                        //BuildingBlockMonoSaccharide penotose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Pentose, new RangesMinMax(0, 3));//for xylose library
                        //BuildingBlockMiscellaneousMatter nah = new BuildingBlockMiscellaneousMatter(PNNLOmics.Data.Constants.MiscellaneousMatterName.NaMinusH, new RangesMinMax(0, 5));

                        List<BuildingBlockMonoSaccharide> forcedMonosaccharideList = new List<BuildingBlockMonoSaccharide>();
                        List<BuildingBlockMiscellaneousMatter> forcedMatterList = new List<BuildingBlockMiscellaneousMatter>();

                        forcedMonosaccharideList.Add(hexose);//hexose
                        forcedMonosaccharideList.Add(hexNac);//hexnac
                        forcedMonosaccharideList.Add(fucose);//fucose
                        forcedMonosaccharideList.Add(neuAc);//neuac
                        //forcedMonosaccharideList.Add(penotose);
                        //forcedMatterList.Add(nah);

                        omniFinderParameterIn.BuildingBlocksMonosacchcarides = forcedMonosaccharideList;
                        omniFinderParameterIn.BuildingBlocksMiscellaneousMatter = forcedMatterList;
                        #endregion

                        List<RangesMinMax> testRanges = new List<RangesMinMax>();

                        testRanges.Add(new RangesMinMax(3, 12));
                        testRanges.Add(new RangesMinMax(2, 9));
                        testRanges.Add(new RangesMinMax(0, 5));
                        testRanges.Add(new RangesMinMax(0, 21));
                        //testRanges.Add(new RangesMinMax(0, 5));

                        OmnifinderCheckObject checkThese = new OmnifinderCheckObject();
                        checkThese.holdBuildingBlockAminoAcid = 0;
                        checkThese.holdBuildingBlockCrossRing = 0;
                        checkThese.holdBuildingBlockElements = 0;
                        checkThese.holdBuildingBlockMiscellaneousMatter = 0;
                        checkThese.holdBuildingBlockMonosaccharides = 4;
                        checkThese.holdBuildingSubAtomicParticles = 0;
                        checkThese.holdBuildingBlockUserUnit = 0;
                        checkThese.holdAdduct = Adducts.Neutral;
                        checkThese.holdCarbType = CarbType.Alditol;

                        checkThese.testRanges = testRanges;

                        checkCount = CheckLibrary(omniFinderParameterIn, checkThese);

                        #endregion
                    }
                    break;
                case LibraryType.NLinked_Alditol8:
                    {
                        omniFinderParameter.CarbohydrateType = OmniFinder.Objects.Enumerations.CarbType.Alditol;
                    }
                    break;
                case LibraryType.NLinked_Alditol9:
                    {
                        omniFinderParameter.CarbohydrateType = OmniFinder.Objects.Enumerations.CarbType.Alditol;
                    }
                    break;
                case LibraryType.NLinked_Alditol10:
                    {
                        omniFinderParameter.CarbohydrateType = OmniFinder.Objects.Enumerations.CarbType.Alditol;
                    }
                    break;
                case LibraryType.Hexose:
                    {
                        #region inside

                        #region force ranges for default libraries
                        omniFinderParameter.CarbohydrateType = OmniFinder.Objects.Enumerations.CarbType.Alditol;

                        //BuildingBlockMonoSaccharide hexose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Hexose, new RangesMinMax(3, 12));
                        BuildingBlockMonoSaccharide hexose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Hexose, new RangesMinMax(0, 40));//for hexose library
                        //BuildingBlockMonoSaccharide hexNac = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.NAcetylhexosamine, new RangesMinMax(2, 9));
                        //BuildingBlockMonoSaccharide fucose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Deoxyhexose, new RangesMinMax(0, 5));
                        //BuildingBlockMonoSaccharide neuAc = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.NeuraminicAcid, new RangesMinMax(0, 5));//21 for PSAX16
                        //BuildingBlockMonoSaccharide penotose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Pentose, new RangesMinMax(0, 3));//for xylose library
                        //BuildingBlockMiscellaneousMatter nah = new BuildingBlockMiscellaneousMatter(PNNLOmics.Data.Constants.MiscellaneousMatterName.NaMinusH, new RangesMinMax(0, 5));

                        List<BuildingBlockMonoSaccharide> forcedMonosaccharideList = new List<BuildingBlockMonoSaccharide>();
                        //List<BuildingBlockMiscellaneousMatter> forcedMatterList = new List<BuildingBlockMiscellaneousMatter>();

                        forcedMonosaccharideList.Add(hexose);//hexose
                        //forcedMonosaccharideList.Add(hexNac);//hexnac
                        //forcedMonosaccharideList.Add(fucose);//fucose
                        //forcedMonosaccharideList.Add(neuAc);//neuac
                        //forcedMonosaccharideList.Add(penotose);
                        //forcedMatterList.Add(nah);

                        omniFinderParameterIn.BuildingBlocksMonosacchcarides = forcedMonosaccharideList;
                        omniFinderParameterIn.BuildingBlocksMiscellaneousMatter = new List<BuildingBlockMiscellaneousMatter>();
                        #endregion

                        List<RangesMinMax> testRanges = new List<RangesMinMax>();

                        testRanges.Add(new RangesMinMax(0, 40));
                        //testRanges.Add(new RangesMinMax(2, 9));
                        //testRanges.Add(new RangesMinMax(0, 5));
                        //testRanges.Add(new RangesMinMax(0, 5));
                        //testRanges.Add(new RangesMinMax(0, 5));

                        OmnifinderCheckObject checkThese = new OmnifinderCheckObject();
                        checkThese.holdBuildingBlockAminoAcid = 0;
                        checkThese.holdBuildingBlockCrossRing = 0;
                        checkThese.holdBuildingBlockElements = 0;
                        checkThese.holdBuildingBlockMiscellaneousMatter = 0;
                        checkThese.holdBuildingBlockMonosaccharides = 1;
                        checkThese.holdBuildingSubAtomicParticles = 0;
                        checkThese.holdBuildingBlockUserUnit = 0;
                        checkThese.holdAdduct = Adducts.Neutral;
                        checkThese.holdCarbType = CarbType.Alditol;

                        checkThese.testRanges = testRanges;

                        checkCount = CheckLibrary(omniFinderParameterIn, checkThese);

                        #endregion
                    }
                    break;
                    case LibraryType.Xylose:
                    {
                        #region inside

                        #region force ranges for default libraries
                        omniFinderParameter.CarbohydrateType = OmniFinder.Objects.Enumerations.CarbType.Alditol;

                        //BuildingBlockMonoSaccharide hexose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Hexose, new RangesMinMax(3, 12));
                        //BuildingBlockMonoSaccharide hexose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Hexose, new RangesMinMax(0, 40));//for hexose library
                        //BuildingBlockMonoSaccharide hexNac = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.NAcetylhexosamine, new RangesMinMax(2, 9));
                        //BuildingBlockMonoSaccharide fucose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Deoxyhexose, new RangesMinMax(0, 5));
                        //BuildingBlockMonoSaccharide neuAc = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.NeuraminicAcid, new RangesMinMax(0, 5));//21 for PSAX16
                        BuildingBlockMonoSaccharide penotose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Pentose, new RangesMinMax(0, 40));//for xylose library
                        //BuildingBlockMiscellaneousMatter nah = new BuildingBlockMiscellaneousMatter(PNNLOmics.Data.Constants.MiscellaneousMatterName.NaMinusH, new RangesMinMax(0, 5));

                        List<BuildingBlockMonoSaccharide> forcedMonosaccharideList = new List<BuildingBlockMonoSaccharide>();
                        List<BuildingBlockMiscellaneousMatter> forcedMatterList = new List<BuildingBlockMiscellaneousMatter>();
                        
                        //forcedMonosaccharideList.Add(hexose);//hexose
                        //forcedMonosaccharideList.Add(hexNac);//hexnac
                        //forcedMonosaccharideList.Add(fucose);//fucose
                        //forcedMonosaccharideList.Add(neuAc);//neuac
                        forcedMonosaccharideList.Add(penotose);
                        //forcedMatterList.Add(nah);

                        omniFinderParameterIn.BuildingBlocksMonosacchcarides = forcedMonosaccharideList;
                        omniFinderParameterIn.BuildingBlocksMiscellaneousMatter = forcedMatterList;
                        #endregion

                        List<RangesMinMax> testRanges = new List<RangesMinMax>();
                       
                        //testRanges.Add(new RangesMinMax(3, 12));
                        //testRanges.Add(new RangesMinMax(2, 9));
                        //testRanges.Add(new RangesMinMax(0, 5));
                        //testRanges.Add(new RangesMinMax(0, 5));
                        testRanges.Add(new RangesMinMax(0, 40));

                        OmnifinderCheckObject checkThese = new OmnifinderCheckObject();
                        checkThese.holdBuildingBlockAminoAcid = 0;
                        checkThese.holdBuildingBlockCrossRing = 0;
                        checkThese.holdBuildingBlockElements = 0;
                        checkThese.holdBuildingBlockMiscellaneousMatter = 0;
                        checkThese.holdBuildingBlockMonosaccharides = 1;
                        checkThese.holdBuildingSubAtomicParticles = 0;
                        checkThese.holdBuildingBlockUserUnit = 0;
                        checkThese.holdAdduct = Adducts.Neutral;
                        checkThese.holdCarbType = CarbType.Alditol;

                        checkThese.testRanges = testRanges;

                        checkCount = CheckLibrary(omniFinderParameterIn, checkThese);

                        #endregion
                    }
                    break;
                case LibraryType.DiagnosticIons:
                    {
                        #region inside

                        #region force ranges for default libraries
                        omniFinderParameter.CarbohydrateType = OmniFinder.Objects.Enumerations.CarbType.Alditol;

                        //BuildingBlockMonoSaccharide hexose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Hexose, new RangesMinMax(3, 12));
                        BuildingBlockMonoSaccharide hexose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Hexose, new RangesMinMax(0, 1));//for hexose library
                        //BuildingBlockMonoSaccharide hexNac = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.NAcetylhexosamine, new RangesMinMax(2, 9));
                        //BuildingBlockMonoSaccharide fucose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Deoxyhexose, new RangesMinMax(0, 5));
                        //BuildingBlockMonoSaccharide neuAc = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.NeuraminicAcid, new RangesMinMax(0, 5));//21 for PSAX16
                        //BuildingBlockMonoSaccharide penotose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Pentose, new RangesMinMax(0, 3));//for xylose library
                        //BuildingBlockMiscellaneousMatter nah = new BuildingBlockMiscellaneousMatter(PNNLOmics.Data.Constants.MiscellaneousMatterName.NaMinusH, new RangesMinMax(0, 5));

                        List<BuildingBlockMonoSaccharide> forcedMonosaccharideList = new List<BuildingBlockMonoSaccharide>();
                        //List<BuildingBlockMiscellaneousMatter> forcedMatterList = new List<BuildingBlockMiscellaneousMatter>();

                        forcedMonosaccharideList.Add(hexose);//hexose
                        //forcedMonosaccharideList.Add(hexNac);//hexnac
                        //forcedMonosaccharideList.Add(fucose);//fucose
                        //forcedMonosaccharideList.Add(neuAc);//neuac
                        //forcedMonosaccharideList.Add(penotose);
                        //forcedMatterList.Add(nah);

                        omniFinderParameterIn.BuildingBlocksMonosacchcarides = forcedMonosaccharideList;
                        omniFinderParameterIn.BuildingBlocksMiscellaneousMatter = new List<BuildingBlockMiscellaneousMatter>();
                        #endregion

                        List<RangesMinMax> testRanges = new List<RangesMinMax>();

                        testRanges.Add(new RangesMinMax(0, 1));
                        //testRanges.Add(new RangesMinMax(2, 9));
                        //testRanges.Add(new RangesMinMax(0, 5));
                        //testRanges.Add(new RangesMinMax(0, 5));
                        //testRanges.Add(new RangesMinMax(0, 5));

                        OmnifinderCheckObject checkThese = new OmnifinderCheckObject();
                        checkThese.holdBuildingBlockAminoAcid = 0;
                        checkThese.holdBuildingBlockCrossRing = 0;
                        checkThese.holdBuildingBlockElements = 0;
                        checkThese.holdBuildingBlockMiscellaneousMatter = 0;
                        checkThese.holdBuildingBlockMonosaccharides = 1;
                        checkThese.holdBuildingSubAtomicParticles = 0;
                        checkThese.holdBuildingBlockUserUnit = 0;
                        checkThese.holdAdduct = Adducts.Neutral;
                        checkThese.holdCarbType = CarbType.Alditol;

                        checkThese.testRanges = testRanges;

                        checkCount = CheckLibrary(omniFinderParameterIn, checkThese);

                        #endregion
                    }
                    break;
                case LibraryType.OmniFinder:
                    {
                        omniFinderParameter.CarbohydrateType = omniFinderParameterIn.CarbohydrateType;
                    }
                    break;
                default:
                    {
                        Console.WriteLine("missing glycan type for this library");
                        Console.ReadKey();
                    }
                    break;
            }


            if (checkCount == 0)
            {


                //BuildingBlockMonoSaccharide hexose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Hexose, new RangesMinMax(0, 12));
                ////BuildingBlockMonoSaccharide hexose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Hexose, new RangesMinMax(0, 40));//for hexose library
                //BuildingBlockMonoSaccharide hexNac = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.NAcetylhexosamine, new RangesMinMax(0, 12));
                //BuildingBlockMonoSaccharide fucose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Deoxyhexose, new RangesMinMax(0, 7));
                //BuildingBlockMonoSaccharide neuAc = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.NeuraminicAcid, new RangesMinMax(0, 21));//21 for PSAX16
                ////BuildingBlockMonoSaccharide penotose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Pentose, new RangesMinMax(0, 3));//for xylose library
                //BuildingBlockMiscellaneousMatter nah = new BuildingBlockMiscellaneousMatter(PNNLOmics.Data.Constants.MiscellaneousMatterName.NaMinusH, new RangesMinMax(0, 5));


                //omniFinderParameter.BuildingBlocksMonosacchcarides.Add(hexose);
                //omniFinderParameter.BuildingBlocksMonosacchcarides.Add(hexNac);
                //omniFinderParameter.BuildingBlocksMonosacchcarides.Add(fucose);
                //omniFinderParameter.BuildingBlocksMonosacchcarides.Add(neuAc);
                ////omniFinderParameter.BuildingBlocksMonosacchcarides.Add(penotose);
                //omniFinderParameter.BuildingBlocksMiscellaneousMatter.Add(nah);

                omniFinderParameter = omniFinderParameterIn;
            }
            else
            {
                Console.WriteLine("failed library check");
                Console.ReadKey();
            }
            

            return omniFinderParameter;
        }

        private static int CheckLibrary(OmniFinderParameters omniFinderParameterIn, OmnifinderCheckObject checkThese)
        {

            int checkCount = 0;
            bool check = true;

            if (omniFinderParameterIn.CarbohydrateType != checkThese.holdCarbType)
            {
                checkCount++;
            }
            if (omniFinderParameterIn.ChargeCarryingAdduct != checkThese.holdAdduct)
            {
                checkCount++;
            }
            if (omniFinderParameterIn.BuildingBlocksAminoAcids.Count != checkThese.holdBuildingBlockAminoAcid)
            {
                checkCount++;
            }
            if (omniFinderParameterIn.BuildingBlocksCrossRings.Count != checkThese.holdBuildingBlockCrossRing)
            {
                checkCount++;
            }
            if (omniFinderParameterIn.BuildingBlocksElements.Count != checkThese.holdBuildingBlockElements)
            {
                checkCount++;
            }
            if (omniFinderParameterIn.BuildingBlocksMiscellaneousMatter.Count != checkThese.holdBuildingBlockMiscellaneousMatter)
            {
                checkCount++;
            }
            if (omniFinderParameterIn.BuildingBlocksMonosacchcarides.Count != checkThese.holdBuildingBlockMonosaccharides)
            {
                checkCount++;
            }
            if (omniFinderParameterIn.BuildingBlocksSubAtomicParticles.Count != checkThese.holdBuildingSubAtomicParticles)
            {
                checkCount++;
            }
            if (omniFinderParameterIn.BuildingBlocksUserUnit.Count != checkThese.holdBuildingBlockUserUnit)
            {
                checkCount++;
            }

            for (int i = 0; i < omniFinderParameterIn.BuildingBlocksMonosacchcarides.Count; i++)
            {
                check = CompareRanges(omniFinderParameterIn.BuildingBlocksMonosacchcarides[i].Range, checkThese.testRanges[i]);
                if (check == false)
                {
                    checkCount++;
                }

            }

            return checkCount;
        }

        private static bool CompareRanges(RangesMinMax rangeIN, RangesMinMax rangeReference)
        {
            bool check = true;

            if (rangeIN.MinRange !=rangeReference.MinRange)
            {
                check = false;
            }

            if (rangeIN.MaxRange != rangeReference.MaxRange)
            {
                check = false;
            }
            return check;
        }


        #endregion

        #endregion
    }
}
