using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using PNNLOmics.Data.Features;
using GetPeaks_DLL.CompareContrast;
using PNNLOmics.Data;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.AnalysisSupport.Tools;
using GetPeaks_DLL.Objects.ResultsObjects;

namespace GetPeaks_DLL.AnalysisSupport
{
    public class AppendToLibrary
    {
        public SampleResutlsObject AppendPrep(string newDataDatasetName, ref List<DataSet> newLibraryDataset, ref List<FeatureLight> featureLitelist, ref List<ElutingPeakLite> elutingPeakList, ref List<IsotopeObject> isotopesDataList, bool convertTOAmino, double massTollerance)
        {
            

            SampleResutlsObject dataSummary = new SampleResutlsObject();

            #region convert to amino glycan
            if (convertTOAmino)
            {
                int libraryIndex = 0;
                ConvertToAminoGlycan converter = new ConvertToAminoGlycan();
                converter.Convert(ref newLibraryDataset, libraryIndex);
            }
            #endregion

            #region sort data
            List<ElutingPeakLite> sortedElutingPeakList = elutingPeakList.OrderBy(p => p.Mass).ToList();
            List<FeatureLight> sortedFeatureLitelist = featureLitelist.OrderBy(p => p.MassMonoisotopic).ToList();
            List<IsotopeObject> sortedIsotopeObject = isotopesDataList.OrderBy(p => p.ExperimentMass).ToList();
            #endregion

            #region compare and contrast

            #region convert to data types

            List<decimal> dataList = new List<decimal>();
            List<decimal> libraryList = new List<decimal>();

            List<XYData> holdLibrary = newLibraryDataset[0].XYList;
            for (int i = 0; i < holdLibrary.Count; i++)
            {
                libraryList.Add((decimal)holdLibrary[i].X);
            }

            for (int i = 0; i < sortedFeatureLitelist.Count; i++)
            {
                dataList.Add((decimal)sortedFeatureLitelist[i].MassMonoisotopic);
            }

            #endregion


            CompareControllerOld newComparer = new CompareControllerOld();
            CompareResults matchResults = new CompareResults();
            double tollerance = massTollerance;//ppm
            newComparer.compareFX(libraryList, dataList, matchResults, tollerance);

            #region organize output


            dataSummary.SampleName = newDataDatasetName;

            #region convert hits to a list of elutingpeaks and isotopes
            List<ElutingPeakLite> glycanExperimentalHitsElutingPeak = new List<ElutingPeakLite>();
            List<FeatureLight> glycanExperimentalHitsFeatures = new List<FeatureLight>();
            List<IsotopeObject> glycanExperimentalHitsIsotopes = new List<IsotopeObject>();
            //convert compare results into glycan list
            for (int i = 0; i < matchResults.IndexListAMatch.Count; i++)
            {
                //this is a list of all glycans that hit the library
                glycanExperimentalHitsElutingPeak.Add(sortedElutingPeakList[matchResults.IndexListAMatch[i]]);
                glycanExperimentalHitsFeatures.Add(sortedFeatureLitelist[matchResults.IndexListAMatch[i]]);
                glycanExperimentalHitsIsotopes.Add(sortedIsotopeObject[matchResults.IndexListAMatch[i]]);
            }
            #endregion

            dataSummary.ElutingPeakList = glycanExperimentalHitsElutingPeak;
            dataSummary.FeatureList = glycanExperimentalHitsFeatures;
            dataSummary.IsotopeObjectList = glycanExperimentalHitsIsotopes;

            #region append data to library
            //loop variables
            int lengthOfMatches = matchResults.IndexListBMatch.Count;
            int current = 0;
            int next = 0;
            double mass1 = 0;
            double mass2 = 0;

            int counter = 0;

            List<double> spine = new List<double>();
            List<double> spineValue = new List<double>();
            List<int> hits = new List<int>();
            List<ElutingPeakLite> glycanLibraryHitsElutingPeak = new List<ElutingPeakLite>();
            List<FeatureLight> glycanLibraryHitsFeatures = new List<FeatureLight>();
            List<IsotopeObject> glycanLibraryHitsIsotopes = new List<IsotopeObject>();
            int numberofhits = 0;

            for (int i = 0; i < newLibraryDataset[0].XYList.Count; i++)
            {
                spine.Add(newLibraryDataset[0].XYList[i].X);
                spineValue.Add(0);
                hits.Add(0);
                ElutingPeakLite newEpeak = new ElutingPeakLite();
                newEpeak.ID = i;
                newEpeak.Mass = newLibraryDataset[0].XYList[i].X;
                newEpeak.NumberOfPeaks = 0;
                newEpeak.SummedIntensity = 0;
                glycanLibraryHitsElutingPeak.Add(newEpeak);
                FeatureLight newFeature = new FeatureLight();
                glycanLibraryHitsFeatures.Add(newFeature);
                IsotopeObject newIsotopeObject = new IsotopeObject();
                glycanLibraryHitsIsotopes.Add(newIsotopeObject);
            }

            for (int i = 0; i < newLibraryDataset[0].XYList.Count; i++)
            {
                if (counter < matchResults.IndexListBMatch.Count)//when the match is lower in quantiy than the spine
                {
                    current = matchResults.IndexListBMatch[counter];
                    if (current == i)//line up with spine
                    {
                        numberofhits++;
                        mass1 = newLibraryDataset[0].XYList[i].X;
                        mass2 = sortedElutingPeakList[matchResults.IndexListAMatch[counter]].Mass;
                        //this is a list of what hit
                        glycanLibraryHitsElutingPeak[i].Mass = sortedElutingPeakList[matchResults.IndexListAMatch[counter]].Mass;
                        glycanLibraryHitsElutingPeak[i].SummedIntensity = sortedElutingPeakList[matchResults.IndexListAMatch[counter]].SummedIntensity;
                        
                        //glycanLibraryHitsElutingPeak[i].NumberOfPeaks = sortedElutingPeakList[matchResults.IndexListAMatch[counter]].NumberOfPeaks;
                        glycanLibraryHitsElutingPeak[i].NumberOfPeaks = 1;

                        glycanLibraryHitsFeatures[i] = sortedFeatureLitelist[matchResults.IndexListAMatch[counter]];

                        glycanLibraryHitsIsotopes[i] = sortedIsotopeObject[matchResults.IndexListAMatch[counter]];

                        int k = counter;//look for additional hits to the library.  K steps through the IndexListAMatch
                        int loopCuttoff = 0; ;
                        if (k < lengthOfMatches - 2)//k+1 needed so we don't overrun
                        {
                            do
                            {
                                current = matchResults.IndexListBMatch[k];
                                next = matchResults.IndexListBMatch[k + 1];
                                if (current == next)
                                {
                                    mass2 = sortedElutingPeakList[matchResults.IndexListAMatch[k + 1]].Mass;
                                    //glycanLibraryHitsElutingPeak[i].NumberOfPeaks += sortedElutingPeakList[matchResults.IndexListAMatch[k + 1]].NumberOfPeaks;
                                    glycanLibraryHitsElutingPeak[i].NumberOfPeaks ++;//how many baseline resolved peaks
                                    
                                    k++;
                                }
                                current = matchResults.IndexListBMatch[k];
                                loopCuttoff = lengthOfMatches - 3;
                            }
                            while (matchResults.IndexListBMatch[k + 1] == i && k < loopCuttoff);//and less than end//+3 because we increment K, and we start at 0 and we need for +1

                            //while (matchResults.IndexListBMatch[k + 1] == i && k < loopCuttoff);//and less than end//+3 because we increment K, and we start at 0 and we need for +1
                            //all additional matches have been accounted for
                        }

                        spineValue[i] = glycanLibraryHitsElutingPeak[i].NumberOfPeaks;

                        k++;
                        counter = k;//-1 ?since it was incremented
                    }
                }
                else
                {
                    //we are in the land of excess spine and not enough mathches
                }
            }
            #endregion

            dataSummary.NumberOfHits = numberofhits;
            dataSummary.AppendedLibrary = spine;//this can be replaced with newLibraryDataset[0].XYList[i].X
            dataSummary.AppendedFeatureList = glycanLibraryHitsFeatures;//this does not deal with multiple hits, only use the first hit in the group
            dataSummary.AppendedElutingPeakList = glycanLibraryHitsElutingPeak; //this does deal with multiple hits
            dataSummary.AppendedIsotopeObjectList = glycanLibraryHitsIsotopes;//isotope info
            

            #endregion


            #endregion

            return dataSummary;
        }

        public SampleResutlsObject AppendPrep(ref List<DataSet> newLibraryDatasetData, ref List<DataSet> newLibraryDatasetLibrary, bool convertTOAmino, double massTollerance)
        {
            string newDataDatasetName = "SimpleCommpare";

            SampleResutlsObject dataSummary = new SampleResutlsObject();

            #region convert to amino glycan
            if (convertTOAmino)
            {
                int libraryIndex = 0;
                ConvertToAminoGlycan converter = new ConvertToAminoGlycan();
                converter.Convert(ref newLibraryDatasetLibrary, libraryIndex);
            }
            #endregion

            #region sort data
            //List<ElutingPeakLite> sortedElutingPeakList = elutingPeakList.OrderBy(p => p.Mass).ToList();
            //List<FeatureLight> sortedFeatureLitelist = featureLitelist.OrderBy(p => p.MassMonoisotopic).ToList();
            //List<IsotopeObject> sortedIsotopeObject = isotopesDataList.OrderBy(p => p.ExperimentMass).ToList();
            #endregion

            #region compare and contrast

            #region convert to data types

            List<decimal> dataList = new List<decimal>();
            List<decimal> libraryList = new List<decimal>();

            List<XYData> holdLibrary = newLibraryDatasetLibrary[0].XYList;
            List<XYData> holdLibraryData = newLibraryDatasetData[0].XYList;
            for (int i = 0; i < holdLibrary.Count; i++)
            {
                //libraryList.Add((decimal)holdLibrary[i].X);
                libraryList.Add(Convert.ToDecimal(holdLibrary[i].X));
            }

            for (int i = 0; i < holdLibraryData.Count; i++)
            {
                //dataList.Add((decimal)holdLibraryData[i].X);
                dataList.Add(Convert.ToDecimal(holdLibraryData[i].X));
            }

            #endregion

            CompareControllerOld newComparer = new CompareControllerOld();
            CompareResults matchResults = new CompareResults();
            double tollerance = massTollerance;//ppm
            newComparer.compareFX(libraryList, dataList, matchResults, tollerance);

            #region organize output


            dataSummary.SampleName = newDataDatasetName;

            #region convert hits to a list of elutingpeaks and isotopes
            List<XYData> LibraryHits = new List<XYData>();
            List<XYData> DataHits = new List<XYData>();
            
            //convert compare results into glycan list
            for (int i = 0; i < matchResults.IndexListAMatch.Count; i++)
            {
                //this is a list of all glycans that hit the library
                LibraryHits.Add(holdLibrary[matchResults.IndexListAMatch[i]]);
                DataHits.Add(holdLibrary[matchResults.IndexListAMatch[i]]);
            }
            #endregion

            dataSummary.LibraryHitsList = LibraryHits;
            dataSummary.DataHitsList = DataHits;


            dataSummary.NumberOfHits = matchResults.IndexListAMatch.Count;
            

            #endregion


            #endregion

            return dataSummary;
        }

        public void AppendToTable(List<SampleResutlsObject> dataCollection, string outputLocation)
        {
            int numberOfColumns = dataCollection.Count;
            int numberOfRows = dataCollection[0].AppendedLibrary.Count;

            int r = 0;
            int c = 0;
            List<string> table1 = new List<string>();
            List<string> table2 = new List<string>();
            List<string> table3 = new List<string>();
            List<string> table4 = new List<string>();
            for (c = 0; c < numberOfColumns; c++)
            {
                for (r = 0; r < numberOfRows; r++)
                {
                    table1.Add("");
                    table2.Add("");
                    table3.Add("");
                    table4.Add("");
                }
            }

            string columnheader = "";
            for (c = 0; c < numberOfColumns; c++)
            {
                columnheader+=dataCollection[c].SampleName + "\t";
            }

            for (r = 0; r < numberOfRows; r++)
            {
                for (c = 0; c < numberOfColumns; c++)
                {
                    table1[r] += dataCollection[c].AppendedFeatureList[r].MassMonoisotopic.ToString() + "\t";
                }
            }

            for (r = 0; r < numberOfRows; r++)
            {
                for (c = 0; c < numberOfColumns; c++)
                {
                    table2[r] += dataCollection[c].AppendedElutingPeakList[r].SummedIntensity.ToString() + "\t";
                }
            }

            for (r = 0; r < numberOfRows; r++)
            {
                for (c = 0; c < numberOfColumns; c++)
                {
                    table3[r] += dataCollection[c].AppendedElutingPeakList[r].NumberOfPeaks.ToString() + "\t";
                }
            }

            for (r = 0; r < numberOfRows; r++)
            {
                for (c = 0; c < numberOfColumns; c++)
                {
                    table4[r] += dataCollection[c].AppendedIsotopeObjectList[r].IsotopeIntensityString.ToString() + "\t";
                }
            }


            StringListToDisk newWriter = new StringListToDisk();
            string targetname1 = outputLocation + "_Mass.txt";
            newWriter.toDiskStringList(targetname1, table1, columnheader);
            string targetname2 = outputLocation + "_Intenstiy.txt";
            newWriter.toDiskStringList(targetname2, table2, columnheader);
            string targetname3 = outputLocation + "_Isomers.txt";
            newWriter.toDiskStringList(targetname3, table3, columnheader);
            string targetname4 = outputLocation + "_Isotopes.txt";
            newWriter.toDiskStringList(targetname4, table4, columnheader);
        }
    }
}
