using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using PNNLOmics.Data.Features;
using PNNLOmics.Data.Constants;

namespace GetPeaks_DLL.DataFIFO
{
    public class DataPeaksToViper
    {
        public void toDiskVIPEROutput(List<ElutingPeakLite> elutingPEakList, List<FeatureLight> featureList, string outputLocation)
        { 
            List<string> featureStringsToWrite = new List<string>();
            List<string> isosStringsToWrite = new List<string>();
            List<string> mapStringsToWrite = new List<string>();

            string featureColumnHeader = "";
            string isosColumnHeader = "";
            string mapColumnHeader = "";
            mapColumnHeader = "Feature_Index" + "\t" + "Peak_Index" + "\t" + "Filtered_Peak_Index";

            List<string> featureColumnNames = new List<string>();

            #region featureColumn Names
            featureColumnNames.Add("Feature_Index");
            featureColumnNames.Add("Monoisotopic_Mass");
            featureColumnNames.Add("Average_Mono_Mass");
            featureColumnNames.Add("UMC_MW_Min");
            featureColumnNames.Add("UMC_MW_Max");
            featureColumnNames.Add("Scan_Start");
            featureColumnNames.Add("Scan_End");
            featureColumnNames.Add("Scan");
            featureColumnNames.Add("UMC_Member_Count");
            featureColumnNames.Add("Max_Abundance");
            featureColumnNames.Add("Abundance");
            featureColumnNames.Add("Class_Rep_MZ");
            featureColumnNames.Add("Class_Rep_Charge");

            for (int i = 0; i < featureColumnNames.Count - 1; i++)
            {
                featureColumnHeader += featureColumnNames[i] + "\t";
            }
            #endregion

            featureColumnHeader += featureColumnNames[featureColumnNames.Count-1];
            
            List<string> isosColumnNames = new List<string>();

            #region isoscolumnNames
            isosColumnNames.Add("scan_num");
            isosColumnNames.Add("charge");
            isosColumnNames.Add("abundance");
            isosColumnNames.Add("mz");
            isosColumnNames.Add("fit");
            isosColumnNames.Add("average_mw");
            isosColumnNames.Add("monoisotopic_mw");
            isosColumnNames.Add("mostabundant_mw");
            isosColumnNames.Add("fwhm");
            isosColumnNames.Add("signal_noise");
            isosColumnNames.Add("mono_abundance");
            isosColumnNames.Add("mono_plus2_abundance");
           
            for (int i = 0; i < isosColumnNames.Count - 1; i++)
            {
                isosColumnHeader += isosColumnNames[i] + ",";
            }

            #endregion

            isosColumnHeader += isosColumnNames[isosColumnNames.Count - 1];

            List<string> peakIsosTriplicate = new List<string>();
            peakIsosTriplicate.Add("");
            peakIsosTriplicate.Add("");
            peakIsosTriplicate.Add("");

            List<ISOSObject> sortTheseIsosObjects = new List<ISOSObject>();//se we can sort the isoso file by scan


            int counter = 0;
            for (int i = 0; i < elutingPEakList.Count; i++)
            {
                #region assign featureVariables
                string Feature_Index = Convert.ToString(i);
                string Monoisotopic_Mass = Convert.ToString(featureList[i].MassMonoisotopic);
                string Average_Mono_Mass = Monoisotopic_Mass;
                string UMC_MW_Min = Monoisotopic_Mass;
                string UMC_MW_Max = Monoisotopic_Mass;
                string Scan_Start = Convert.ToString(elutingPEakList[i].ScanStart);
                string Scan_End = Convert.ToString(elutingPEakList[i].ScanEnd);
                string Scan = Convert.ToString(elutingPEakList[i].ScanMaxIntensity);
                string UMC_Member_Count = Convert.ToString(1);
                string Max_Abundance = Convert.ToString(Convert.ToInt64(elutingPEakList[i].SummedIntensity));
                string Abundance = Convert.ToString(Convert.ToInt64(elutingPEakList[i].SummedIntensity));
                int chargestate = featureList[i].ChargeState;
                double MZ = (featureList[i].MassMonoisotopic + Constants.SubAtomicParticles[SubAtomicParticleName.Proton].MassMonoIsotopic*chargestate)/chargestate;
                string Class_Rep_MZ = Convert.ToString(MZ);
                string Class_Rep_Charge = Convert.ToString(chargestate);

                string seperator = "\t";
                string line =
                    seperator +
                    Monoisotopic_Mass + seperator +
                    Average_Mono_Mass + seperator +
                    UMC_MW_Min + seperator +
                    UMC_MW_Max + seperator +
                    Scan_Start + seperator +
                    Scan_End + seperator +
                    Scan + seperator +
                    UMC_Member_Count + seperator +
                    Max_Abundance + seperator +
                    Abundance + seperator +
                    Class_Rep_MZ + seperator +
                    Class_Rep_Charge;//Feature_Index + was removed from the front

                #endregion

               // featureStringsToWrite.Add(line);

                //isos file
                
                for (int j = 0; j < 3; j++)
                { 
                    double isosAbundance = 0;
                    string isosAbundanceST = "";
                    string scan_numST = "";
                    int tempScanNum = 0;
                    switch(j)
                    {
                        #region set correct intensity and scan for start, max, and stop scans and abundance
                        case 0:
                            {
                                isosAbundance = Convert.ToInt64(elutingPEakList[i].SummedIntensity/10);
                                scan_numST = Convert.ToString(elutingPEakList[i].ScanStart);
                                tempScanNum = elutingPEakList[i].ScanStart;
                            }
                            break;
                        case 1:
                            {
                                isosAbundance =Convert.ToInt64(elutingPEakList[i].SummedIntensity);
                                scan_numST = Convert.ToString(elutingPEakList[i].ScanMaxIntensity);
                                tempScanNum = elutingPEakList[i].ScanMaxIntensity;
                            }
                            break;
                        case 2:
                            {
                                 isosAbundance = Convert.ToInt64(elutingPEakList[i].SummedIntensity/10);
                                 scan_numST = Convert.ToString(elutingPEakList[i].ScanEnd);
                                 tempScanNum = elutingPEakList[i].ScanEnd;
                            }
                            break;
                        #endregion
                    }

                    isosAbundanceST = Convert.ToString(isosAbundance);
                    
                    #region assign isosvariables
                    string scan_num = scan_numST;
                    string charge = Convert.ToString(featureList[i].ChargeState);
                    string abundance = isosAbundanceST;
                    //string mz = Monoisotopic_Mass; MZ
                    double MZisos = (featureList[i].MassMonoisotopic + Constants.SubAtomicParticles[SubAtomicParticleName.Proton].MassMonoIsotopic * featureList[i].ChargeState) / featureList[i].ChargeState;
                    string mz = Convert.ToString(MZisos);
                    string fit = Convert.ToString(featureList[i].Score);
                    string average_mw = Convert.ToString(featureList[i].MassMonoisotopic);
                    string monoisotopic_mw = Convert.ToString(featureList[i].MassMonoisotopic);
                    string mostabundant_mw = Convert.ToString(Convert.ToInt64(featureList[i].MassMonoisotopic));
                    string fwhm = Convert.ToString(0.005);
                   // string signal_noise = Convert.ToString(3);
                    string signal_noise = Convert.ToString(elutingPEakList[i].ScanStart);
                    string mono_abundance = Convert.ToString(isosAbundanceST);
                    string mono_plus2_abundance = Convert.ToString(Convert.ToInt64(isosAbundance/2));

                    ISOSObject objectforSorting = new ISOSObject();
                    seperator = ",";
                    string isosLine = scan_num + seperator +
                        charge + seperator +
                        abundance + seperator +
                        mz + seperator +
                        fit + seperator +
                        average_mw + seperator +
                        monoisotopic_mw + seperator +
                        mostabundant_mw + seperator +
                        fwhm + seperator +
                        signal_noise + seperator +
                        mono_abundance + seperator +
                        mono_plus2_abundance;

                    #endregion
                    

                    peakIsosTriplicate[j] = isosLine;
                    
                    //isosStringsToWrite.Add(isosLine);
                    objectforSorting.ScanNumber = tempScanNum;
                    objectforSorting.IsosString = isosLine;
                    objectforSorting.FeatureString = line;
                    objectforSorting.FeatureIndex = i;
                    objectforSorting.RefMass = monoisotopic_mw;
                    sortTheseIsosObjects.Add(objectforSorting);

                    
                    //mapStringsToWrite.Add(i.ToString() + "\t" + counter.ToString() + "\t" + counter.ToString());
                    counter++;
                }

                
            }

            //sortTheseIsosObjects = sortTheseIsosObjects.OrderBy(p => p.Index).ToList();

            //sortTheseIsosObjects=sortTheseIsosObjects.OrderBy(p => p.ScanNumber).ToList();
            sortTheseIsosObjects = sortTheseIsosObjects.OrderBy(p => p.RefMass).ToList();//this sorts the dat so it is ready for the feature file

            int sortObjectCounter = 0;
            string next = "";
            string current = "";
            int featureCounter = 0;

            int isotopeCounter = 0;
            List<ISOSObject> sortV2 = new List<ISOSObject>();
            List<ISOSObject> storeFeatuers = new List<ISOSObject>();
            //this sets up the feature list and a list of all isos 
            for (sortObjectCounter = 0; sortObjectCounter < sortTheseIsosObjects.Count-1; sortObjectCounter++)
            {
                current = sortTheseIsosObjects[sortObjectCounter].FeatureString;

                sortTheseIsosObjects[sortObjectCounter].FeatureIndex = featureCounter;
                storeFeatuers.Add(sortTheseIsosObjects[sortObjectCounter]);//store the first feature
                
                //featureStringsToWrite.Add(current);

                //add up objects for isos sorting later/
                sortTheseIsosObjects[sortObjectCounter].IsosIndex = isotopeCounter;
                sortV2.Add(sortTheseIsosObjects[sortObjectCounter]);//add first isos
                isotopeCounter++;
                sortObjectCounter++; //now that the first is added we need to look beyonde
                //isosStringsToWrite.Add(sortTheseIsosObjects[part2Counter].IsosString);
                //for (int j = 0; j < 3; j++)
                //{

                if (sortObjectCounter < sortTheseIsosObjects.Count)
                {
                    next = sortTheseIsosObjects[sortObjectCounter].FeatureString;

                    while (string.Compare(next, current) == 0 && sortObjectCounter < sortTheseIsosObjects.Count - 2)//-2 because next is set inside the loop
                    {
                        //add next isotope here
                        sortTheseIsosObjects[sortObjectCounter].IsosIndex = isotopeCounter;
                        sortTheseIsosObjects[sortObjectCounter].FeatureIndex = featureCounter;
                        sortV2.Add(sortTheseIsosObjects[sortObjectCounter]);//add up objects for isos sorting later

                        isotopeCounter++;
                        sortObjectCounter++;
                        //isotopeIndex++;
                        //isosStringsToWrite.Add(sortTheseIsosObjects[part2Counter].IsosString);
                        next = sortTheseIsosObjects[sortObjectCounter].FeatureString;//+1 for new next
                    }

                    sortObjectCounter--; //return to baseline
                }
                else
                {
                    //last point
                }

                featureCounter++;
            }


            //sortV2 = sortV2.OrderBy(p => p.ScanNumber).ToList();

            sortV2 = sortV2.OrderBy(p => p.ScanNumber).ToList();//sorts by scan nubmer

            for (int i = 0; i<sortV2.Count;i++)
            {
                sortV2[i].SortedIsosIndex = i;//this is now the position in the isos file since we are now sorted by scan number
            }

            foreach (ISOSObject isos in sortV2)
            {
                //featureStringsToWrite.Add(isos.FeatureString);
                isosStringsToWrite.Add(isos.IsosString);
            }

            sortV2 = sortV2.OrderBy(p => p.FeatureIndex).ToList();  //final sort by feature to get feature vs isos number

            foreach (ISOSObject isos in sortV2)
            {
                //featureStringsToWrite.Add(isos.FeatureString);
                mapStringsToWrite.Add(isos.FeatureIndex.ToString() + "\t" + isos.SortedIsosIndex.ToString() + "\t" + isos.SortedIsosIndex.ToString());
            }

            //storeFeatuers = storeFeatuers.OrderBy(p => p.RefMass).ToList();//this may not be needed
            featureCounter = 0;
            foreach (ISOSObject feature in storeFeatuers)
            {

                featureStringsToWrite.Add(featureCounter.ToString() + feature.FeatureString);
                featureCounter++;
                
            }

            StringListToDisk newWriter = new StringListToDisk();

            string featurefile = outputLocation + "_LCMSFeatures.txt";
            //write features
            newWriter.toDiskStringList(featurefile, featureStringsToWrite, featureColumnHeader);

            string isosFile = outputLocation + "_isos.csv";
            //write isos
            newWriter.toDiskStringList(isosFile, isosStringsToWrite, isosColumnHeader);

            string filteredIsosFile = outputLocation + "_Filtered_isos.csv";
            //write filtered isos, which is a copy
            newWriter.toDiskStringList(filteredIsosFile, isosStringsToWrite, isosColumnHeader);

            string isosFilterFile = outputLocation + "_Filtered_isos.csv";
            //write isos
            newWriter.toDiskStringList(isosFile, isosStringsToWrite, isosColumnHeader);

            string mapFile = outputLocation + "_LCMSFeatureToPeakMap.txt";
            //write isos
            newWriter.toDiskStringList(mapFile, mapStringsToWrite, mapColumnHeader);

        }

        

        private class ISOSObject
        {
            public int ScanNumber { get; set; }
            public string IsosString { get; set; }
            public int FeatureIndex { get; set; }
            public int IsosIndex { get; set; }
            public string FeatureString { get; set; }
            public string RefMass { get; set; }
            public int SortedIsosIndex { get; set; }
        }
    }
}
