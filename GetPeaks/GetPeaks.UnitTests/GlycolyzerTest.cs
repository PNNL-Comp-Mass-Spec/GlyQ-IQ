using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.Objects;
using NUnit.Framework;
using PNNLOmics.Data;
using GetPeaks_DLL.CompareContrast;
using GetPeaks_DLL.AnalysisSupport;
using OmniFinder;
using GetPeaks_DLL.Functions;
using OmniFinder.Objects;
using GetPeaks_DLL.Objects.ResultsObjects;
using OmniFinder.Objects.BuildingBlocks;
using GetPeaks_DLL.Glycolyzer;
using GetPeaks_DLL.Glycolyzer.Enumerations;
using CompareContrastDLL;

namespace GetPeaks.UnitTests
{
    class GlycolyzerTest
    {
        public void ToGlycolyzerTest()
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

            
            LibraryType glycanType;
            FeatureOriginType featureType;
            SetFile(0, out filenameIn, out filenameOut, out filenameOutNonGlycan, out folderIn, out folderOut, out numberOfDataSetsTotal, out glycanType, out featureType);

            //parameters
            double massTolleranceMatch = 10;//ppm
            double massTolleranceVerySmall = 100;//ppm//this will automatically be reduced till it works with a one-to one responce.
            OmniFinderParameters omniFinderParameter = SetOmniFinderRanges(glycanType);

            #region create OmnifinderLibrary

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
                SetFile(dataset, out filenameIn, out filenameOut, out filenameOutNonGlycan, out folderIn, out folderOut, out numberOfDataSets, out glycanType, out featureType);
                filenameIn = folderIn + filenameIn;

                #region 1.  Load data

                //load data
                List<FeatureAbstract> loadedFeatures;
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
                    default:
                        {
                            loadedFeatures = LoadViperData(filenameIn);
                        }
                        break;
                }

                
                //Assert.AreEqual(loadedViperFeatures.Count, 10292);//for dataset 1

                //load library
                List<DataSet> loadedLibrary = loadedLibraryData(glycanType);
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
                        if(valuesFromCompareOmni.DataNotInLibrary.Count>0)
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

                    for(int j=0;j<libraryHitsExactGlycanOnly.Count;j++)
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

        private static void WriteToDisk(string filenameOut, string folderOut, FeatureOriginType featureType, OmniFinderOutput omniResults, GlycolyzerResults glycolyzerResults)
        {
            WriteGlycolyzerToDisk writer = new WriteGlycolyzerToDisk();

            string fullFileNameOut = folderOut + filenameOut;

            writer.WriteGlycanFile(fullFileNameOut, glycolyzerResults, omniResults);


            

            switch (featureType)
            {
                case FeatureOriginType.Viper:
                    {
                        writer.WriteViperFeatureFile(fullFileNameOut, glycolyzerResults); ;
                    }
                    break;
                case FeatureOriginType.IMS:
                    {
                        writer.WriteIMSFeatureFile(fullFileNameOut, glycolyzerResults);
                    }
                    break;
                default:
                    {
                        writer.WriteViperFeatureFile(fullFileNameOut, glycolyzerResults); ;
                    }
                    break;
            }
        }

        private static void CondenseIsomers(OmniFinderOutput omniResults, GlycanResult result, int multipleHitDivisor, double massTolleranceVerySmall)
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

        private static void CondenseIsomers(GlycanResult result, int multipleHitDivisor)
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

        private static bool CheckConsisency(GlycanResult result, out int divisor)
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

        private static void DidThisWorkUnitTest(bool success)
        {
            if (success == false)
            {
                Assert.IsTrue(success);
            }
            Assert.IsTrue(success);
        }

        private static bool VerifytListLengths(List<int> checkLengths, int keyLenght)
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

        private static double CalculateVerySmallMassTolerance(double massTolleranceVerySmall, OmniFinderParameters omniFinderParameter, List<DataSet> loadedLibrary)
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

        private static CompareResultsIndexes CheckLibraryAndSplitIndexes(double massTolleranceVerySmall, OmniFinderParameters omniFinderParameter, List<DataSet> loadedLibrary, out List<DataSet> loadedLibraryNonGlycan, out List<DataSet> loadedLibraryGlycanOnly, out bool areGlycansPresent , out bool areNonGlycansPresent)
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

        private static void AppendFeaturesToLibrarySpineList(double massTolleranceMatch, GlycolyzerResults glycolyzerResults, List<double> LibraryList, List<FeatureAbstract> featureHits, out List<Isomer> appendedGlycanFeatures, out List<IsomerGlycanIndexes> appendedIsomerIndexes)
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
        
        #endregion

        #region set parameters

        private static void SetFile(int dataset, out string filenameIn, out string filenameOut, out string filenameOutNonGlycan, out string folderIn, out string folderOut, out int numberofDataSets, out LibraryType glycanType, out FeatureOriginType featureType)
        {
            List<string> featureFilenamesPile = new List<string>();
            List<string> outFileNamePile = new List<string>();
            
            //concatenate from excel
            int switchInt = 1;

            switch (switchInt)
            {
                #region inside

                case 0://IMS, N-Linked.  Be sure to change writer to IMS
                    {
                        #region IMS
                        glycanType = LibraryType.NLinked_Alditol;
                        glycanType = LibraryType.NLinked_Alditol_PolySA;
                        glycanType = LibraryType.NLinked_Alditol8;
                        featureType = FeatureOriginType.IMS;
                        folderIn = @"D:\PNNL\Projects\DATA 2012_01_10 IMS2\02 Feature Finder IMS\";
                        folderOut = @"D:\PNNL\Projects\DATA 2012_01_10 IMS2\03_Glycolyzer\";
                        //folderIn = @"D:\PNNL\Projects\DATA 2012_01_10 IMS2\02 Feature Finder IMS\To glycolyzer V2\";
                        //viper// featureFilenamesPile.Add("LCMSFeaturesInViewIMS_SN78_3X.txt"); outFileNamePile.Add("IMS2 2 SN78_3X.txt");
                        //featureFilenamesPile.Add("Glyco06_SN78_IMS2_LINX_Run1_CAL_LCMSFeatures.txt"); outFileNamePile.Add("IMS2 2 SN78_3X.txt");
                        //featureFilenamesPile.Add("Glyco06_SN78_IMS2_LINX_Run1_CAL_LCMSFeatures.txt"); outFileNamePile.Add("IMS2 2 SN78_3X_PolySA8.txt");
                        featureFilenamesPile.Add("Glyco06_SN78_IMS2_LINX_Run1_CAL_LCMSFeatures.txt"); outFileNamePile.Add("IMS2 2 SN78_3X_L8.txt");
                        
                        #endregion
                    }
                    break;
                case 1://SPIN, N-Linked
                    {
                        #region SPIN QTOF
                        //glycanType = LibraryType.NLinked_Alditol;
                        //glycanType = LibraryType.NLinked_Alditol_PolySA;
                        glycanType = LibraryType.NLinked_Alditol10;
                        featureType = FeatureOriginType.Viper;
                        //folderIn = @"D:\PNNL\Projects\DATA 2012_01_11 SPIN QTOF3\2b_Viper HexNAc X10\";//*
                        //folderOut = @"D:\PNNL\Projects\DATA 2012_01_11 SPIN QTOF3\3_Glycolyzer\";
                        //folderIn = @"L:\PNNL Files\PNNL\Projects\DATA 2012_01_11 SPIN QTOF3\2b_Viper HexNAc X10\";//*
                        //folderOut = @"L:\PNNL Files\PNNL\Projects\DATA 2012_01_11 SPIN QTOF3\3_Glycolyzer\";//*
                        //featureFilenamesPile.Add("LCMSFeaturesInView SPIN 2 sort.txt"); outFileNamePile.Add("SPIN 2 3X.txt");

                        //folderIn = @"D:\PNNL\Projects\DATA 2012_01_11 SPIN QTOF3\DATA 2012 Serum Orbitrap, IMS, QTOF\Calibrated Corrected- SPIN ESI Orb IMS\";
                        //folderOut = @"D:\PNNL\Projects\DATA 2012_01_11 SPIN QTOF3\DATA 2012 Serum Orbitrap, IMS, QTOF\Calibrated Corrected- SPIN ESI Orb IMS\3_Glycolyzer\";
                        //featureFilenamesPile.Add("FeatruesCorrectCAL ESI.txt"); outFileNamePile.Add("ESI 2 3XCAL.txt");
                        //featureFilenamesPile.Add("FeatruesCorrectCAL SPIN.txt"); outFileNamePile.Add("SPIN 2 3XCAL.txt");

                        //folderIn = @"D:\PNNL\Projects\DATA 2012_01_11 SPIN QTOF3\2_Viper Sum5 Pub61 zCalibrated\";
                        //folderOut = @"D:\PNNL\Projects\DATA 2012_01_11 SPIN QTOF3\3_Glycolyzer Sum5 Pub61 zCalibrated\";
                        //folderIn = @"D:\PNNL\Projects\DATA 2012_01_11 SPIN QTOF3\2_Viper Sum5 Pub61 zreCalibrated\";
                        //folderOut = @"D:\PNNL\Projects\DATA 2012_01_11 SPIN QTOF3\3_Glycolyzer Sum5 Pub61 zreCalibrated\";

                        //folderIn = @"D:\PNNL\Projects\DATA 2012_01_11 SPIN QTOF3\2_Viper Sum5 Poly Cal\";
                        //folderOut = @"D:\PNNL\Projects\DATA 2012_01_11 SPIN QTOF3\3_Glycolyzer Sum5 PolyCal\";
                        //folderIn = @"L:\PNNL Files\PNNL\Projects\DATA 2012_01_11 SPIN QTOF3\2_Viper Sum5 Poly Cal\";
                        folderIn = @"D:\PNNL\Projects\DATA 2012_01_11 SPIN QTOF3\2_Viper Sum5 PolyCal Ch7\";
                        //folderOut = @"L:\PNNL Files\PNNL\Projects\DATA 2012_01_11 SPIN QTOF3\3_Glycolyzer Sum5 PolyCal\";
                        folderOut = @"V:\";

                        //featureFilenamesPile.Add("LCMSFeaturesInView_Sum5_Cal_ESI2.txt"); outFileNamePile.Add("ESI 2 Sum5 3X CalMZ.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_Sum5_Cal_SPIN2.txt"); outFileNamePile.Add("SPIN 2 Sum5 3X CalMZ.txt");

                        //glycanType = LibraryType.NLinked_Alditol_2ndIsotope;
                        //featureFilenamesPile.Add("LCMSFeaturesInView_Sum5_Cal_ESI2.txt"); outFileNamePile.Add("ESI 2 Sum5 3X CalMZ+2nd.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_Sum5_Cal_SPIN2.txt"); outFileNamePile.Add("SPIN 2 Sum5 3X CalMZ+2nd.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_Sum5_Cal_1Scan_SPIN2.txt"); outFileNamePile.Add("SPIN 2 Sum5 3X CalMZ_1Scan+2nd.txt");
                        
                        //featureFilenamesPile.Add("LCMSFeaturesInView_Sum5_Cal_1Scan_SPIN2.txt"); outFileNamePile.Add("SPIN 2 Sum5 3X CalMZ_1Scan+PolySA8.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_Sum5_Cal_1Scan_ESI2.txt"); outFileNamePile.Add("ESI 2 Sum5 3X CalMZ_1Scan+PolySA8.txt");

                        //featureFilenamesPile.Add("LCMSFeaturesInView_Sum5_SPIN2_PolyCal_Fit1.txt"); outFileNamePile.Add("SPIN 2 Sum5 3X CalMZ_3Scan_fit1_PolySA8.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_Sum5_ESI2_PolyCal_Fit1.txt"); outFileNamePile.Add("ESI 2 Sum5 3X CalMZ_3Scan_fit1_PolySA8.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_Sum5_SPIN2_PolyCal_Fit15.txt"); outFileNamePile.Add("SPIN 2 Sum5 3X CalMZ_3Scan_fit15_PolySA8.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_Sum5_ESI2_PolyCal_Fit15.txt"); outFileNamePile.Add("ESI 2 Sum5 3X CalMZ_3Scan_fit15_PolySA8.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_Sum5_SPIN2_PolyCal_Fit1_1scan.txt"); outFileNamePile.Add("SPIN 2 Sum5 3X CalMZ_1Scan_fit1_PolySA8.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_Sum5_ESI2_PolyCal_Fit1_1scan.txt"); outFileNamePile.Add("ESI 2 Sum5 3X CalMZ_1Scan_fit1_PolySA8.txt");

                        //featureFilenamesPile.Add("LCMSFeaturesInView_Sum5_SPIN2_PolyCal_Fit1.txt"); outFileNamePile.Add("SPIN 2 Sum5 3X CalMZ_3Scan_fit1_A9.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_Sum5_ESI2_PolyCal_Fit1.txt"); outFileNamePile.Add("ESI 2 Sum5 3X CalMZ_3Scan_fit1_A9.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_Sum5_SPIN2_PolyCal_Fit15.txt"); outFileNamePile.Add("SPIN 2 Sum5 3X CalMZ_3Scan_fit15_A9.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_Sum5_ESI2_PolyCal_Fit15.txt"); outFileNamePile.Add("ESI 2 Sum5 3X CalMZ_3Scan_fit15_A9.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_Sum5_SPIN2_PolyCal_Fit1_1scan.txt"); outFileNamePile.Add("SPIN 2 Sum5 3X CalMZ_1Scan_fit1_A9.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_Sum5_ESI2_PolyCal_Fit1_1scan.txt"); outFileNamePile.Add("ESI 2 Sum5 3X CalMZ_1Scan_fit1_A9.txt");

                        //featureFilenamesPile.Add("LCMSFeaturesInView_Sum5_SPIN2_PolyCal_Fit1_Ch7.txt"); outFileNamePile.Add("SPIN 2 Sum5 3X CalMZ_3Scan_fit1_Ch7_PolySA16.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_Sum5_ESI2_PolyCal_Fit1_Ch7.txt"); outFileNamePile.Add("ESI 2 Sum5 3X CalMZ_3Scan_fit1_CH7_PolySA16.txt");
                        featureFilenamesPile.Add("LCMSFeaturesInView_Sum5_SPIN2_PolyCal_Fit1_Ch7.txt"); outFileNamePile.Add("SPIN 2 Sum5 3X CalMZ_3Scan_fit1_Ch7_A10.txt");
                        featureFilenamesPile.Add("LCMSFeaturesInView_Sum5_ESI2_PolyCal_Fit1_Ch7.txt"); outFileNamePile.Add("ESI 2 Sum5 3X CalMZ_3Scan_fit1_CH7_A10.txt");
                        

                        
                        //featureFilenamesPile.Add("LCMSFeaturesInViewSPIN2.txt"); outFileNamePile.Add("SPIN 2 Sum5 3X CalMZ.txt");
                        #endregion
                    }
                    break;
                case 2://Ant, Ant
                    {
                        #region Ant
                        //glycanType = LibraryType.Ant_Alditol;
                        //featureType = FeatureType.Viper;
                        //folderIn = @"D:\PNNL\Projects\DATA 2012_03_08 Velos 4 Serum Cell Ant Rat\2_ViperFeatures\";
                        //folderOut = @"D:\PNNL\Projects\DATA 2012_03_08 Velos 4 Serum Cell Ant Rat\3_GlycolyzerV2\";
                        //featureFilenamesPile.Add("LCMSFeaturesInView_Ant01_3X.txt"); outFileNamePile.Add("Ant01_3X_Xylose.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_Ant02_3X.txt"); outFileNamePile.Add("Ant02_3X_Xylose.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_Ant03_3X.txt"); outFileNamePile.Add("Ant03_3X_Xylose.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_Ant04_3X.txt"); outFileNamePile.Add("Ant04_3X_Xylose.txt");

                        glycanType = LibraryType.Hexose;
                        featureType = FeatureOriginType.Viper;
                        folderIn = @"D:\PNNL\Projects\DATA 2012_03_08 Velos 4 Serum Cell Ant Rat\2_ViperFeatures\";
                        folderOut = @"D:\PNNL\Projects\DATA 2012_03_08 Velos 4 Serum Cell Ant Rat\3_GlycolyzerV2\";
                        featureFilenamesPile.Add("LCMSFeaturesInView_Ant01_3X.txt"); outFileNamePile.Add("Ant01_3X_Hexose.txt");
                        featureFilenamesPile.Add("LCMSFeaturesInView_Ant02_3X.txt"); outFileNamePile.Add("Ant02_3X_Hexose.txt");
                        featureFilenamesPile.Add("LCMSFeaturesInView_Ant03_3X.txt"); outFileNamePile.Add("Ant03_3X_Hexose.txt");
                        featureFilenamesPile.Add("LCMSFeaturesInView_Ant04_3X.txt"); outFileNamePile.Add("Ant04_3X_Hexose.txt");

                        //glycanType = LibraryType.NLinked_Alditol;
                        //featureType = FeatureType.Viper;
                        //folderIn = @"D:\PNNL\Projects\DATA 2012_03_08 Velos 4 Serum Cell Ant Rat\2_ViperFeatures\";
                        //folderOut = @"D:\PNNL\Projects\DATA 2012_03_08 Velos 4 Serum Cell Ant Rat\3_GlycolyzerV2\";
                        //featureFilenamesPile.Add("LCMSFeaturesInView_Ant01_3X.txt"); outFileNamePile.Add("Ant01_3X Serum.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_Ant02_3X.txt"); outFileNamePile.Add("Ant02_3X Serum.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_Ant03_3X.txt"); outFileNamePile.Add("Ant03_3X Serum.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_Ant04_3X.txt"); outFileNamePile.Add("Ant04_3X Serum.txt");
                        #endregion
                    }
                    break;
                case 3://Cell velos, Cell
                    {
                        #region Cell Velos
                        //Cell V1
                        //glycanType = LibraryType.Cell_Alditol;
                        //featureType = FeatureType.Viper;
                        //folderIn = @"D:\PNNL\Projects\DATA 2012_03_08 Velos 4 Serum Cell Ant Rat\2_ViperFeatures\";
                        //folderOut = @"D:\PNNL\Projects\DATA 2012_03_08 Velos 4 Serum Cell Ant Rat\3_GlycolyzerV2\";

                        //featureFilenamesPile.Add("LCMSFeaturesInView_DPBS_60-3 2x.txt"); outFileNamePile.Add("DPBS_60-3 2x.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_DPBS_60-4 2x.txt"); outFileNamePile.Add("DPBS_60-4 2x.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_DPBS_60-5 2x.txt"); outFileNamePile.Add("DPBS_60-5 2x.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_DPBS_90-1 5x.txt"); outFileNamePile.Add("DPBS_90-1 5x.txt");
                        ////featureFilenamesPile.Add("LCMSFeaturesInView_NaPO4_60-3 3X 7-12.txt"); outFileNamePile.Add("NaPO4_60-3 3X 7-12.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_NaPO4_60-3 3X 8-12.txt"); outFileNamePile.Add("NaPO4_60-3 3X 8-12.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_NaPO4_90-1 2X.txt"); outFileNamePile.Add("NaPO4_90-1 2X_Cell.txt");

                        //Serum
                        //glycanType = LibraryType.NLinked_Alditol;
                        //featureType = FeatureType.Viper;
                        //folderIn = @"D:\PNNL\Projects\DATA 2012_03_08 Velos 4 Serum Cell Ant Rat\2_ViperFeatures\";
                        //folderOut = @"D:\PNNL\Projects\DATA 2012_03_08 Velos 4 Serum Cell Ant Rat\3_GlycolyzerV2\";

                        //featureFilenamesPile.Add("LCMSFeaturesInView_DPBS_60-3 2x.txt"); outFileNamePile.Add("DPBS_60-3 2x Serum.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_DPBS_60-4 2x.txt"); outFileNamePile.Add("DPBS_60-4 2x Serum.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_DPBS_60-5 2x.txt"); outFileNamePile.Add("DPBS_60-5 2x Serum.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_DPBS_90-1 5x.txt"); outFileNamePile.Add("DPBS_90-1 5x Serum.txt");
                        ////featureFilenamesPile.Add("LCMSFeaturesInView_NaPO4_60-3 3X 7-12.txt"); outFileNamePile.Add("NaPO4_60-3 3X 7-12.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_NaPO4_60-3 3X 8-12.txt"); outFileNamePile.Add("NaPO4_60-3 3X 8-12 Serum.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_NaPO4_90-1 2X.txt"); outFileNamePile.Add("NaPO4_90-1 2X_Cell Serum.txt");

                        ////Cell V2
                        //glycanType = LibraryType.Cell_Alditol_V2;
                        //featureType = FeatureType.Viper;
                        //folderIn = @"D:\PNNL\Projects\DATA 2012_03_08 Velos 4 Serum Cell Ant Rat\2_ViperFeatures\";
                        //folderOut = @"D:\PNNL\Projects\DATA 2012_03_08 Velos 4 Serum Cell Ant Rat\3_GlycolyzerV2\";

                        //featureFilenamesPile.Add("LCMSFeaturesInView_DPBS_60-3 2x.txt"); outFileNamePile.Add("V2 DPBS_60-3 2x.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_DPBS_60-4 2x.txt"); outFileNamePile.Add("V2 DPBS_60-4 2x.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_DPBS_60-5 2x.txt"); outFileNamePile.Add("V2 DPBS_60-5 2x.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_DPBS_90-1 5x.txt"); outFileNamePile.Add("V2 DPBS_90-1 5x.txt");
                        ////featureFilenamesPile.Add("LCMSFeaturesInView_NaPO4_60-3 3X 7-12.txt"); outFileNamePile.Add("V2 NaPO4_60-3 3X 7-12.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_NaPO4_60-3 3X 8-12.txt"); outFileNamePile.Add("V2 NaPO4_60-3 3X 8-12.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_NaPO4_90-1 2X.txt"); outFileNamePile.Add("V2 NaPO4_90-1 2x.txt");

                        //Cell Vmini
                        glycanType = LibraryType.Cell_Alditol_Vmini;
                        featureType = FeatureOriginType.Viper;
                        folderIn = @"D:\PNNL\Projects\DATA 2012_03_08 Velos 4 Serum Cell Ant Rat\2_ViperFeatures\";
                        folderOut = @"D:\PNNL\Projects\DATA 2012_03_08 Velos 4 Serum Cell Ant Rat\3_GlycolyzerV2\";

                        featureFilenamesPile.Add("LCMSFeaturesInView_DPBS_60-3 2x.txt"); outFileNamePile.Add("Vmini DPBS_60-3 2x.txt");
                        featureFilenamesPile.Add("LCMSFeaturesInView_DPBS_60-4 2x.txt"); outFileNamePile.Add("Vmini DPBS_60-4 2x.txt");
                        featureFilenamesPile.Add("LCMSFeaturesInView_DPBS_60-5 2x.txt"); outFileNamePile.Add("Vmini DPBS_60-5 2x.txt");
                        featureFilenamesPile.Add("LCMSFeaturesInView_DPBS_90-1 5x.txt"); outFileNamePile.Add("Vmini DPBS_90-1 5x.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_NaPO4_60-3 3X 7-12.txt"); outFileNamePile.Add("Vmini NaPO4_60-3 3X 7-12.txt");
                        featureFilenamesPile.Add("LCMSFeaturesInView_NaPO4_60-3 3X 8-12.txt"); outFileNamePile.Add("Vmini NaPO4_60-3 3X 8-12.txt");
                        featureFilenamesPile.Add("LCMSFeaturesInView_NaPO4_90-1 2X.txt"); outFileNamePile.Add("Vmini NaPO4_90-1 2x.txt");


                        #endregion
                    }
                    break;
                case 4://Cell Exactive, Cell
                    {
                        #region Cell Exactive
                        glycanType = LibraryType.Cell_Alditol;
                        featureType = FeatureOriginType.Viper;
                        folderIn = @"D:\PNNL\Projects\DATA 2011_06_29 Orbirap3 Cell + SN343536 150 n 75\2_Viper\";
                        folderOut = @"D:\PNNL\Projects\DATA 2011_06_29 Orbirap3 Cell + SN343536 150 n 75\3_Glycolyzer\";
                        featureFilenamesPile.Add("LCMSFeaturesInView_GlyCell01_19Jul11_DPBS1X_75.txt"); outFileNamePile.Add("GlyCell01_19Jul11_DPBS1X_75_Cell.txt");
                        featureFilenamesPile.Add("LCMSFeaturesInView_GlyCell01_19Jul11_PO4100_75.txt"); outFileNamePile.Add("GlyCell01_19Jul11_PO4100_75_Cell.txt");
                        featureFilenamesPile.Add("LCMSFeaturesInView_GlyCell01_21Jul11_PO415060_75.txt"); outFileNamePile.Add("GlyCell01_21Jul11_PO415060_75_Cell.txt");
                        featureFilenamesPile.Add("LCMSFeaturesInView_GlycoCell01_29Jul11_PO410060_150.txt"); outFileNamePile.Add("GlycoCell01_29Jul11_PO410060_150_Cell.txt");
                        featureFilenamesPile.Add("LCMSFeaturesInView_GlycoCell01_29Jul11_PO415030_150.txt"); outFileNamePile.Add("GlycoCell01_29Jul11_PO415030_150_Cell.txt");
                        #endregion
                    }
                    break;  
                case 5://Serum Velos, N-Linked
                    {
                        #region Serum
                        glycanType = LibraryType.NLinked_Alditol;
                        glycanType = LibraryType.NLinked_Alditol_PolySA;
                        glycanType = LibraryType.NLinked_Alditol8;
                        featureType = FeatureOriginType.Viper;
                        folderIn = @"D:\PNNL\Projects\DATA 2012_03_08 Velos 4 Serum Cell Ant Rat\2_ViperFeatures\";
                        folderOut = @"D:\PNNL\Projects\DATA 2012_03_08 Velos 4 Serum Cell Ant Rat\3_GlycolyzerV2\";
                        //featureFilenamesPile.Add("LCMSFeaturesInView_SN101_3X HCD45.txt"); outFileNamePile.Add("SN101_3X HCD45.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_SN101_3X HighMass.txt"); outFileNamePile.Add("SN101_3X HighMass.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_SN101_3X Top3.txt"); outFileNamePile.Add("SN101_3X Top3.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_SN103_3X 7-12.txt"); outFileNamePile.Add("SN103_3X 7-12.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_SN103_3X HCD45 7-12.txt"); outFileNamePile.Add("SN103_3X HCD45 7-12.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_SN103_3X HCD45 8-12.txt"); outFileNamePile.Add("SN103_3X HCD45 8-12.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_SN104_3X HCD45.txt"); outFileNamePile.Add("SN104_3X HCD45.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_SN105_3X 7419671113.txt"); outFileNamePile.Add("SN105_3X 7419671113.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_SN105_3X HCD45.txt"); outFileNamePile.Add("SN105_3X HCD45.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_SN107_3X ETD HighMass.txt"); outFileNamePile.Add("SN107_3X ETD HighMass.txt");
                        //featureFilenamesPile.Add("LCMSFeaturesInView_SN757677_3X.txt"); outFileNamePile.Add("SN757677_3X.txt");

                        //featureFilenamesPile.Add("LCMSFeaturesInView_SN107_3X HCD45.txt"); outFileNamePile.Add("SN107_3X HCD45.txt");

                        //featureFilenamesPile.Add("LCMSFeaturesInView_SN101_3X Top3.txt"); outFileNamePile.Add("SN101_3X Top3 PolySA 8.txt");
                        featureFilenamesPile.Add("LCMSFeaturesInView_SN101_3X Top3.txt"); outFileNamePile.Add("SN101_3X Top3 L8.txt");
                        #endregion
                    }
                    break;
                case 6://Rat Velos, N-Linked?
                    {
                        #region Rat
                        glycanType = LibraryType.NLinked_Alditol;
                        featureType = FeatureOriginType.Viper;
                        folderIn = @"D:\PNNL\Projects\DATA 2012_03_08 Velos 4 Serum Cell Ant Rat\2_ViperFeatures\";
                        folderOut = @"D:\PNNL\Projects\DATA 2012_03_08 Velos 4 Serum Cell Ant Rat\3_GlycolyzerV2\";
                        featureFilenamesPile.Add("LCMSFeaturesInView_Test2_3X.txt"); outFileNamePile.Add("Test2_3X.txt");
                        #endregion
                    }
                    break;
                case 7://SialylLactose Velos, SL
                    {
                        #region SialylLactose
                        glycanType = LibraryType.NLinked_Aldehyde;
                        featureType = FeatureOriginType.Viper;
                        folderIn = @"D:\PNNL\Projects\DATA 2012_03_08 Velos 4 Serum Cell Ant Rat\2_ViperFeatures\";
                        folderOut = @"D:\PNNL\Projects\DATA 2012_03_08 Velos 4 Serum Cell Ant Rat\3_GlycolyzerV2\";                    
                        featureFilenamesPile.Add("LCMSFeaturesInView_SL26_100ppm.txt"); outFileNamePile.Add("SL26_100ppm.txt");
                        #endregion
                    }
                    break;
                case 8://SPINCalibration, SL
                    {
                        #region SPIN QTOF Non Calibrated
                        glycanType = LibraryType.NonCalibrated;
                        featureType = FeatureOriginType.Viper;
                        folderIn = @"D:\PNNL\Projects\DATA 2012_01_11 SPIN QTOF3\2_Viper Sum5 non-Calibrated\";
                        folderOut = @"D:\PNNL\Projects\DATA 2012_01_11 SPIN QTOF3\2_Viper Sum5 non-Calibrated\";
                        featureFilenamesPile.Add("LCMSFeaturesInView_SPIN2_Sum5_NonCal.txt"); outFileNamePile.Add("SPIN2_Sum5_NonCal.txt");
                        featureFilenamesPile.Add("LCMSFeaturesInView_ESI2_Sum5_NonCal.txt"); outFileNamePile.Add("ESI2_Sum5_NonCal.txt");
                        #endregion
                    }
                    break;
                default:
                    {
                        #region default
                        Console.ReadKey();
                        glycanType = LibraryType.NLinked_Aldehyde;
                        featureType = FeatureOriginType.Viper;
                        folderIn = @"D:\PNNL\Projects\DATA 2012_03_08 Velos 4 Serum Cell Ant Rat\2_ViperFeatures\";
                        folderOut = @"D:\PNNL\Projects\DATA 2012_03_08 Velos 4 Serum Cell Ant Rat\3_GlycolyzerV2\";
                        featureFilenamesPile.Add("LCMSFeaturesInView_SL26_100ppm.txt"); outFileNamePile.Add("SL26_100ppm.txt");
                        #endregion
                    }
                    break;

                #endregion
            }

            filenameIn = featureFilenamesPile[dataset];
            //filenameOut = outFileNamePile[dataset];
            filenameOut = System.Text.RegularExpressions.Regex.Replace(outFileNamePile[dataset], ".txt$", "") + "_Glycan.txt";
            filenameOutNonGlycan = System.Text.RegularExpressions.Regex.Replace(outFileNamePile[dataset], ".txt$", "")+"_NonGlycan.txt";
            numberofDataSets = featureFilenamesPile.Count;
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

        private static OmniFinderParameters SetOmniFinderRanges(LibraryType whichCarbType)
        {
            OmniFinderParameters omniFinderParameter = new OmniFinderParameters();
            switch(whichCarbType)
            {
                case LibraryType.Ant_Alditol:
                    {
                        omniFinderParameter.CarbohydrateType = OmniFinder.Objects.Enumerations.CarbType.Alditol;
                    }
                    break;
                case LibraryType.Cell_Alditol:
                    {
                        omniFinderParameter.CarbohydrateType = OmniFinder.Objects.Enumerations.CarbType.Alditol;
                    }
                    break;
                case LibraryType.Cell_Alditol_V2:
                    {
                        omniFinderParameter.CarbohydrateType = OmniFinder.Objects.Enumerations.CarbType.Alditol;
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
                        omniFinderParameter.CarbohydrateType = OmniFinder.Objects.Enumerations.CarbType.Alditol;
                    }
                    break;
                case LibraryType.NLinked_Alditol_PolySA:
                    {
                        omniFinderParameter.CarbohydrateType = OmniFinder.Objects.Enumerations.CarbType.Alditol;
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
                        omniFinderParameter.CarbohydrateType = OmniFinder.Objects.Enumerations.CarbType.Alditol;
                    }
                    break;
                default:
                    {
                        Console.WriteLine("missing glycan type for this library");
                        Console.ReadKey();
                    }
                    break;
            }
            
            BuildingBlockMonoSaccharide hexose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Hexose, new RangesMinMax(0, 12));
            //BuildingBlockMonoSaccharide hexose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Hexose, new RangesMinMax(0, 40));//for hexose library
            BuildingBlockMonoSaccharide hexNac = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.NAcetylhexosamine, new RangesMinMax(0, 12));
            BuildingBlockMonoSaccharide fucose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Deoxyhexose, new RangesMinMax(0, 7));
            BuildingBlockMonoSaccharide neuAc = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.NeuraminicAcid, new RangesMinMax(0, 21));//21 for PSAX16
            //BuildingBlockMonoSaccharide penotose = new BuildingBlockMonoSaccharide(PNNLOmics.Data.Constants.MonosaccharideName.Pentose, new RangesMinMax(0, 3));//for xylos library
            BuildingBlockMiscellaneousMatter nah = new BuildingBlockMiscellaneousMatter(PNNLOmics.Data.Constants.MiscellaneousMatterName.NaMinusH, new RangesMinMax(0, 5));


            omniFinderParameter.BuildingBlocksMonosacchcarides.Add(hexose);
            omniFinderParameter.BuildingBlocksMonosacchcarides.Add(hexNac);
            omniFinderParameter.BuildingBlocksMonosacchcarides.Add(fucose);
            omniFinderParameter.BuildingBlocksMonosacchcarides.Add(neuAc);
            //omniFinderParameter.BuildingBlocksMonosacchcarides.Add(penotose);
            omniFinderParameter.BuildingBlocksMiscellaneousMatter.Add(nah);

            return omniFinderParameter;
        }

        #endregion
    }
}
