using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using GetPeaks_DLL.CompareContrast;
using DeconTools.Backend.DTO;

namespace GetPeaks_DLL
{
    public class CalculateElutingPeakRanges
    {
        #region Properties
        private Run MainRun {get;set;}
        private int ScanCounter  {get;set;}
        private int ScanCounterPrevious { get; set; }
        private List<int> PeakIndexList { get; set; }
        private int ScansetRangeOffset{ get; set; }
        private double ResolutionQualityPercentOfMax { get; set; }
        private double Tolerance { get; set; }
        private SimpleWorkflowParameters Parameters { get; set; }
        private int wActive = 1;
        private int wBeing = 2;
        private int wClosed = 3;

        #endregion

        public void SetElutingPeakProperties(Run run, int scanCounter, int scanCounterPrevious, List<int> peakIndexList, int scansetRangeOffset, double resolutionQualityPercentOfMax, SimpleWorkflowParameters parameters, double tolerance)
        {
            this.MainRun = run;
            this.ScanCounter = scanCounter;
            this.ScanCounterPrevious = scanCounterPrevious;
            this.PeakIndexList = peakIndexList;
            this.ScansetRangeOffset = scansetRangeOffset;
            this.ResolutionQualityPercentOfMax = resolutionQualityPercentOfMax;
            this.Parameters = parameters;
            this.Tolerance = tolerance;
        }
        
        public void CalculateElutingPeaksRanges(ref bool isFirstScanSet, ref int scansetInitialOffset, ref List<ElutingPeak> elutingPeakCollectionStorage)
        {     
            //these are the states an eluting peak can be in.  Active = growing, Bring = close to end but not closed, Closed = no more peaks can be added
            
            List<ElutingPeak> currentElutingPeakCollection = MainRun.ResultCollection.ElutingPeakCollection;

            //scansetInitialOffset = 0;//how many scans we have processed so far

            //1.  Deal with first scan and assign everything as a peak and make them active
            if (isFirstScanSet)//the first scanset is an initial case
            {
                #region for the first scanset, make everything part of the library with ID=wActive
                if (this.PeakIndexList.Count > 0)//first scan with a peak
                {
                    //scansetInitialOffset = scanCounter;
                    //scansetInitialOffset++;
                    for (int i = 0; i < PeakIndexList.Count; i++)
                    {
                        ElutingPeak newElutingPeak = NewElutingPeak(PeakIndexList[i], ScanCounter);
                        currentElutingPeakCollection.Add(newElutingPeak);

                        #region legacy code
                        //ElutingPeak newElutingPeak = new ElutingPeak();
                        //int peakIDToAdd = PeakIndexList[i];
                        //newElutingPeak.PeakList.Add(MainRun.ResultCollection.MSPeakResultList[peakIDToAdd]);
                        ////newElutingPeak.ScanStart = scanCounter;//first scan containing the point
                        ////newElutingPeak.ScanEnd = scanCounter;//initialize last scan the same so we can increment it;
                        ////newElutingPeak.ScanMaxIntensity = scanCounter;//since there is one point it is the max intensity scan
                        //newElutingPeak.ScanStart = ScanCounter;//first scan containing the point
                        //newElutingPeak.ScanEnd = ScanCounter;//initialize last scan the same so we can increment it;
                        //newElutingPeak.ScanMaxIntensity = ScanCounter;//since there is one point it is the max intensity scan
                        //newElutingPeak.Intensity = newElutingPeak.PeakList[0].MSPeak.Height;////the intensity stored is the max intensity detected from a single scan
                        //newElutingPeak.Mass = MainRun.ResultCollection.MSPeakResultList[peakIDToAdd].MSPeak.XValue;
                        //newElutingPeak.ID = wActive;//set all peaks to active
                        //CurrentElutingPeakCollection.Add(newElutingPeak);
                        #endregion
                    }
                    isFirstScanSet = false;
                }
                #endregion
            }
            
            //2.  allign subsequent scans to the first scan.  The alligned eluting peaks are denoted as 
            //active (still eluting), being (almost done, decided by one zero was detected) and closed (2 zeroes are detected)
            else //all scans beyon th first
            {
                #region Now that we are beyond the first scan, we need to allign the scans back to the library
                {
                    // allign to List() via commpare contrast
                    scansetInitialOffset++;//if there are zero scans at the front of the data set
                    int j;
                    //TODO:  this should be pulled out into a property of the compare and contrast
                    List<decimal> libraryMasses = new List<decimal>();//list to feed to comparecontrast
                    List<decimal> dataMasses = new List<decimal>();//list to feed to compare contrast
                    List<float> dataIntensities = new List<float>();//List of intensities so we can find the max

                    string dataValues = "";//for printing
                    string dataIntensityValues = "";//for printing
                    string libraryValues = "";//for printing
                    string libraryString = "";//for printing

                    #region setup dataMases and LibraryMasses so we can feed the compare algorithm.  use local PeakList
                    //for (int i = 0; i < PeakIndexList.Count; i++)
                    for (int i = 0; i < MainRun.ResultCollection.Run.PeakList.Count; i++)
                    {
                        //dataMasses.Add((decimal)(MainRun.ResultCollection.MSPeakResultList[PeakIndexList[i]].MSPeak.XValue));
                        //dataIntensities.Add(MainRun.ResultCollection.MSPeakResultList[PeakIndexList[i]].MSPeak.Height);
                        //dataValues += ((decimal)(MainRun.ResultCollection.MSPeakResultList[PeakIndexList[i]].MSPeak.XValue)).ToString() + ",";//for printing
                        //dataIntensityValues += (MainRun.ResultCollection.MSPeakResultList[PeakIndexList[i]].MSPeak.Height.ToString() + ",");//for printing

                        dataMasses.Add((decimal)(MainRun.ResultCollection.Run.PeakList[i].XValue));
                        dataIntensities.Add(MainRun.ResultCollection.Run.PeakList[i].Height);
                        dataValues += ((decimal)(MainRun.ResultCollection.Run.PeakList[i].XValue)).ToString() + ",";//for printing
                        dataIntensityValues += (MainRun.ResultCollection.Run.PeakList[i].Height.ToString() + ",");//for printing
                    }

                    int key = 0;
                    Dictionary<int, int> remapLibrary = new Dictionary<int, int>();
                    key = 0;//dictionary key.  iterating the key, they will be independant and remap to the hits
                    for (int i = 0; i < currentElutingPeakCollection.Count; i++)
                    {
                        int testCase = currentElutingPeakCollection[i].ID;
                        //if (CurrentElutingPeakCollection[i].ID != wClosed)//if not closed
                        if (currentElutingPeakCollection[i].ID < wClosed)//if not closed
                        {
                            libraryMasses.Add((decimal)currentElutingPeakCollection[i].PeakList[0].MSPeak.XValue);
                            remapLibrary.Add(key, i);
                            key++;
                            libraryValues += ((decimal)currentElutingPeakCollection[i].PeakList[0].MSPeak.XValue).ToString() + ",";//for printing
                            libraryString += currentElutingPeakCollection[i].PeakList[0].MSPeak.Height.ToString() + ",";//for printing
                        }
                    }

                    Dictionary<int, int> remapData = new Dictionary<int, int>();
                    key = 0;//dictionary key.  iterating the key, they will be independant and remap to the hits
                    int offsetMSPeakResultList = 0;
                    for (int i = 0; i < MainRun.ResultCollection.Run.PeakList.Count; i++)
                    {
                        offsetMSPeakResultList = MainRun.ResultCollection.MSPeakResultList.Count - MainRun.ResultCollection.Run.PeakList.Count;
                        remapData.Add(key, offsetMSPeakResultList + i);
                        key++;                        
                    }
                    #endregion


                    CompareResults Results = new CompareResults();//generated results initialization so it can be filled by compareFX

                    //if (libraryMasses.Count > 0 && dataMasses.Count > 0)//the compareFX needs both values to operate
                    if (dataMasses.Count > 0)//the compareFX needs both values to operate
                    {
                        if (libraryMasses.Count > 0)//the compareFX needs both values to operate
                        {
                            #region if we have data in the spectrum, send it to compareFX
                            CompareControllerOld letsCompare = new CompareControllerOld();
                            letsCompare.compareFX(libraryMasses, dataMasses, Results, Tolerance);// abstracted call:  accepts a library mass and data mass and returns indexes

                            int counter2 = 0;//initialize counter2.  What is counter2

                            //if there are hits...
                            if (Results.IndexListBMatch.Count > 0)//by default IndexListAMatch must be <0 also because of a mached pair
                            {
                                #region are there matches from the compareFX

                                //remove duplicates because the list compare will line up several to the existing data base
                                #region remove duplicates.  if there are duplicate hits to one library, take the closest by mass
                                int last = Results.IndexListBMatch[0];
                                decimal errorLast = 0;
                                decimal dataMass = 0;
                                decimal libraryMass = 0;
                                decimal error = 0;
                                List<int> LibraryHitsList = new List<int>();
                                List<int> DataHitList = new List<int>();//it is also important to collect data hits so we can pull the incomming intensity

                                LibraryHitsList.Add(last);//we are adding the first point
                                DataHitList.Add(Results.IndexListAMatch[0]);//we are adding the first point
                                for (int i = 0; i < Results.IndexListBMatch.Count; i++)
                                {
                                    int current = Results.IndexListBMatch[i];
                                    if (current != last)//only add it if we have not added the index yet
                                    {
                                        LibraryHitsList.Add(current);
                                        DataHitList.Add(Results.IndexListAMatch[i]);//bring the data index match along
                                        last = Results.IndexListBMatch[i];//store so we can see if there are more of the same index in the list
                                        dataMass = dataMasses[Results.IndexListAMatch[i]];
                                        libraryMass = libraryMasses[current];
                                        errorLast = Math.Abs(dataMass - libraryMass);//this allows for the first hit to be not the best.  the error of the first poor hit will be greater than the next optimal hit
                                    }
                                    else//when current = last, we have multiple hits to the library.  add best of the multiple hits based on X error 
                                    {
                                        dataMass = dataMasses[Results.IndexListAMatch[i]];
                                        libraryMass = libraryMasses[current];
                                        error = Math.Abs(dataMass - libraryMass);
                                        if (error < errorLast)//this is the test to see if we replace an existing hit with a better hit (lower error)
                                        {
                                            //LibraryHitsList[LibraryHitsList.Count] = current;//this is not needed because it is one library hitting several data points
                                            int updatedIndex = Results.IndexListAMatch[i];
                                            DataHitList[DataHitList.Count - 1] = updatedIndex;//bring the data index match along .  -1 is because the count includes zero
                                            last = Results.IndexListBMatch[i];//store so we can see if there are more of them in the list            
                                        }
                                        else
                                        {
                                            //do nothing because the lowest error peak was already selected prior
                                        }
                                        errorLast = error;//the error is always stored incease the next hit is better
                                    }
                                }
                                #endregion

                                //remap for viewing
                                #region setup stings for viewing, OutMatch and bMatch.  No real code is here
                                string outMatch = "";
                                foreach (int h in Results.IndexListBandNotA)
                                {
                                    outMatch += remapLibrary[h].ToString() + ",";
                                }

                                string bmatch = "";
                                foreach (int g in LibraryHitsList)
                                {
                                    bmatch += remapLibrary[g].ToString() + ",";
                                }
                                #endregion

                                //assign new endpoint for active data (libraries with matches)
                                #region assign endpoints to matches

                                string added = "";
                                float hitIntensity = 0;

                                for (j = 0; j < LibraryHitsList.Count; j++)//iterate through the hits till matches are found
                                {
                                    int BmatchIndex = LibraryHitsList[j];//for testing
                                    if (counter2 == BmatchIndex)
                                    {
                                        int realLocation = remapLibrary[LibraryHitsList[j]];

                                        #region Filter #2, use intensity to find the maximum and potentially degrade the state of the eluting peak //active to being or being to closed
                                        //test for slope
                                        //check for new max intensity and update cumulative slope

                                        ElutingPeak currentElutingPeak = currentElutingPeakCollection[realLocation];

                                        double oldIntensity = currentElutingPeak.Intensity;
                                        //this is a mess because there can be more than one point within the window.  Currently we take the lower mass's intensity
                                        CheckIntensityForMax(realLocation, DataHitList[j], ScanCounter, currentElutingPeakCollection, dataIntensities);
                                        double newIntensity = currentElutingPeak.Intensity;
                                        //float change = dataIntensities[DataHitList[j]];
                                        hitIntensity = dataIntensities[DataHitList[j]];
                                        if ((double)newIntensity * Parameters.Part1Parameters.MaxHeightForNewPeak > dataIntensities[DataHitList[j]])
                                        {
                                            //since we dipped down from the maximum by a certain percentage, we can now look for a second peak
                                            currentElutingPeak.NumberOfPeaksMode = "NewPeak";
                                        }
                                        //now that we have updated the intensity lets see if it changed  we can only go +2
                                        int slopesInARow = 3;
                                        if (newIntensity > oldIntensity && currentElutingPeak.NumberOfPeaksFlag <= slopesInARow)//this is the new intensity
                                        {
                                            currentElutingPeak.NumberOfPeaksFlag += 1;//we are going up now
                                        }
                                        else
                                        {
                                            if (currentElutingPeak.NumberOfPeaksFlag > -slopesInARow)//we can only go -2
                                            {
                                                currentElutingPeak.NumberOfPeaksFlag -= 1;
                                            }
                                        }

                                        int previousScanEnd = currentElutingPeak.ScanEnd;
                                        currentElutingPeak.ScanEnd = ScanCounter;//the peak is good, update new scan end
                                        currentElutingPeak.ID = wActive;//lable as active

                                        if (currentElutingPeak.NumberOfPeaksFlag == slopesInARow || currentElutingPeak.NumberOfPeaksFlag == -slopesInARow)
                                        {
                                            if (currentElutingPeak.NumberOfPeaksMode == "NewPeak")
                                            {
                                                if (currentElutingPeak.NumberOfPeaksFlag == slopesInARow)
                                                {
                                                    //we went down and then went up
                                                    //currentElutingPeak.ScanEnd = previousScanEnd;

                                                    //add one to the peak count
                                                    //currentElutingPeak.numberOfPeaks++;
                                                }
                                            }
                                        }
                                        #endregion

                                        added += counter2.ToString() + ",";
                                    }
                                    else
                                    {
                                        j -= 1;
                                    }

                                    counter2++;
                                }

                                #endregion

                                //assign state to ElutingPeakList.  This is where we can keep peaks
                                UpdateID(currentElutingPeakCollection, LibraryHitsList, DataHitList, remapLibrary, remapData, ScanCounter, ScanCounterPrevious, hitIntensity, scansetInitialOffset, wActive, wBeing, wClosed);
                                //add data that were not hit as new library peaks

                                if (ScanCounter > 0)
                                {
                                    if (Results.IndexListAandNotB.Count > 0)
                                    {
                                        //string ToADDstring = "";
                                        for (int i = 0; i < Results.IndexListAandNotB.Count; i++)
                                        {
                                            ElutingPeak newElutingPeak = NewElutingPeak(Results.IndexListAandNotB[i], ScanCounter);
                                            currentElutingPeakCollection.Add(newElutingPeak);

                                            #region legacy code
                                            //ElutingPeak newElutingPeak = new ElutingPeak();
                                            //int peakIDToAdd = Results.IndexListAandNotB[i];
                                            //ToADDstring += peakIDToAdd.ToString() + ",";
                                            //newElutingPeak.PeakList.Add(MainRun.ResultCollection.MSPeakResultList[PeakIndexList[peakIDToAdd]]);
                                            //newElutingPeak.Mass = newElutingPeak.PeakList[0].MSPeak.XValue;
                                            //newElutingPeak.ScanStart = ScanCounter;//first scan containing the point
                                            //newElutingPeak.ScanEnd = ScanCounter;//initialize last scan the same so we can increment it;
                                            //newElutingPeak.ScanMaxIntensity = ScanCounter; //since there is one point it is the max intensity scan
                                            //newElutingPeak.Intensity = newElutingPeak.PeakList[0].MSPeak.Height;
                                            //newElutingPeak.ID = wActive;//set all peaks to active
                                            //newElutingPeak.numberOfPeaks = 1;
                                            //newElutingPeak.NumberOfPeaksMode = "Current";
                                            //CurrentElutingPeakCollection.Add(newElutingPeak);
                                            #endregion
                                        }

                                        //sort and update run
                                        MainRun.ResultCollection.ElutingPeakCollection = currentElutingPeakCollection.OrderBy(p => p.PeakList[0].MSPeak.XValue).ToList();
                                        currentElutingPeakCollection = MainRun.ResultCollection.ElutingPeakCollection; //for internal use
                                    }
                                }
                                #endregion
                            }
                            else  //there are no hits.  increase all ids
                            {
                                IncreaseID(currentElutingPeakCollection, wActive, wBeing, wClosed);
                            }

                            #endregion
                        }
                        else
                        {
                            Console.WriteLine("new spot");//we have new eluting peaks to start
                            #region inside
                            int createPeaksCount = dataMasses.Count;
                            for (int i = 0; i < createPeaksCount; i++)
                            {
                                //MainRun.ResultCollection.MSPeakResultList[PeakIndexList[peakIDToAdd]
                                
                                ElutingPeak newElutingPeak = NewElutingPeak(i, ScanCounter);
                                currentElutingPeakCollection.Add(newElutingPeak);
                            }
                            #endregion
                        }
                    }
                    else//if there is no data, mark all increase all IDs by 1 so that we are closer to terminating the eluting peak
                    {
                        IncreaseID(currentElutingPeakCollection, wActive, wBeing, wClosed);
                        //add new peaks that were not listed already

                    }

                    #region checking results section
                    //int asb = 0;
                    List<int> currentIDforAll = new List<int>();
                    string listforAll = "";
                    string listforAllend = "";
                    string newMassList = "";
                    for (j = 0; j < currentElutingPeakCollection.Count; j++)
                    {
                        currentIDforAll.Add(currentElutingPeakCollection[j].ID);
                        listforAll += currentElutingPeakCollection[j].ID.ToString() + ",";
                        listforAllend += currentElutingPeakCollection[j].ScanEnd.ToString() + ",";
                    }

                    for (j = 0; j < currentElutingPeakCollection.Count; j++)
                    {
                        if (currentElutingPeakCollection[j].ID < 3)//not closed
                        {
                            newMassList += currentElutingPeakCollection[j].PeakList[0].MSPeak.XValue + ",";
                        }
                        else
                        {
                            //this is hit when ID is =3 aka closed
                            elutingPeakCollectionStorage.Add(currentElutingPeakCollection[j]);
                            currentElutingPeakCollection.Remove(currentElutingPeakCollection[j]);
        
                        }
                    }
                    //append results
                    Results.IndexName = ScanCounter.ToString();

                    #endregion
                }
                #endregion
            }
        }

        #region private functions

        private ElutingPeak NewElutingPeak(int IndexAandNotB, int scanCounter)
        {
            ElutingPeak newElutingPeak = new ElutingPeak();
            int peakIDToAdd = IndexAandNotB;
            //ToADDstring += peakIDToAdd.ToString() + ",";
            newElutingPeak.PeakList.Add(MainRun.ResultCollection.MSPeakResultList[PeakIndexList[peakIDToAdd]]);
            newElutingPeak.Mass = newElutingPeak.PeakList[0].MSPeak.XValue;
            newElutingPeak.ScanStart = scanCounter;//first scan containing the point
            newElutingPeak.ScanEnd = scanCounter;//initialize last scan the same so we can increment it;
            newElutingPeak.ScanMaxIntensity = scanCounter; //since there is one point it is the max intensity scan
            newElutingPeak.Intensity = newElutingPeak.PeakList[0].MSPeak.Height;
            newElutingPeak.ID = wActive;//set all peaks to active
            newElutingPeak.NumberOfPeaks = 1;
            newElutingPeak.NumberOfPeaksMode = "Current";
            
            return newElutingPeak;
        }
             
        //This checks to see if the current intensity is greater than the last intensity so we can find the highest point in the eluting peak
        private void CheckIntensityForMax(int location, int dataIndex, int scanCounter, List<ElutingPeak> CurrentElutingPeakCollection, List<float> dataIntensities)
        {
            double existingIntensity = CurrentElutingPeakCollection[location].Intensity;
            double currentIntensity = dataIntensities[dataIndex];

            if (currentIntensity > existingIntensity)
            {
                CurrentElutingPeakCollection[location].ScanMaxIntensity = scanCounter;
                CurrentElutingPeakCollection[location].Intensity = currentIntensity;
            }
        }

        private void IncreaseID(List<ElutingPeak> CurrentElutingPeakCollection, int wActive, int wBeing, int wClosed)
        {
            for (int j = 0; j < CurrentElutingPeakCollection.Count; j++)
            {
                int rty = CurrentElutingPeakCollection[j].ID;
                if (CurrentElutingPeakCollection[j].ID != wClosed)
                {
                    CurrentElutingPeakCollection[j].ID += 1;// CurrentElutingPeakCollection[j].ID;
                }
            }
        }

        private void UpdateID(List<ElutingPeak> CurrentElutingPeakCollection, List<int> LibraryHitsList, List<int> DataHitList, Dictionary<int, int> remapLibrary, Dictionary<int, int> remapData, int scanCounter, int scanCounterPrevious, float currentIntensity, int scansetInitialOffset, int wActive, int wBeing, int wClosed)
        {
            //this case is to deal with the fist 3 scans of the dataset.  Usually we will fall to the default case
            int realLocationLibrary = 0;
            int realLocationData = 0;

            switch (scansetInitialOffset)
            {
                case 0:
                    //do notnihg because ID is already set
                    break;
                case 1:
                    for (int j = 0; j < LibraryHitsList.Count; j++)
                    {
                        realLocationLibrary = remapLibrary[LibraryHitsList[j]];
                        realLocationData = remapData[DataHitList[j]];
                        int scanEnd = CurrentElutingPeakCollection[realLocationLibrary].ScanEnd;
                        if (scanEnd == scanCounter)
                        {
                            //realLocation corresponds to the correct eluting peak to work with (mapped back from the commpare and contrast)
                            //dataLocation corresponds the the result mapped back to the PeakIndexList location
                            CurrentElutingPeakCollection[realLocationLibrary].ID = wActive;//wActive=1

                            MSPeakResult newMSPeakResult = AddPeakToElutingPeak(scanCounter, realLocationData, MainRun);
                            CurrentElutingPeakCollection[realLocationLibrary].PeakList.Add(newMSPeakResult);
                        }
                        //if (scanEnd == scanCounter - 1)
                        if (scanEnd == scanCounterPrevious)
                        {
                            CurrentElutingPeakCollection[realLocationLibrary].ID = wBeing;//wBeing=2
                            //there is no peak to add but keep the eluting peak going
                        }
                    }
                    break;
                default://most common case when we are in the middle of the chromatogram
                    for (int j = 0; j < LibraryHitsList.Count; j++)//for each eluting peak in the collection
                    {
                        //if (CurrentElutingPeakCollection[j].ID != wClosed)//if not closed, it is either 1 or 2
                        realLocationLibrary = remapLibrary[LibraryHitsList[j]];
                        //TODO do we need this if statement.  Are all matches open?
                        if (CurrentElutingPeakCollection[realLocationLibrary].ID != wClosed)//if not closed, it is either 1 or 2
                        { 
                            //if there is an wActive or wBeing state
                            #region if we have a specific match to increase.  this is found when the J we are on hits a remap(matchvalue)
                            realLocationLibrary = remapLibrary[LibraryHitsList[j]];
                            realLocationData = remapData[DataHitList[j]];
                                   
                            #region add a data peak to the eluting peak
                            MSPeakResult newMSPeakResult = AddPeakToElutingPeak(scanCounter, realLocationData, MainRun);
                            CurrentElutingPeakCollection[realLocationLibrary].PeakList.Add(newMSPeakResult);
                            #endregion

                            #region filter #1.  if peak is tailing for more than 5x the distance from start to maximum.  penalty is ID+=1
                            //check for symetry about maximum cuttoff
                            //if peak is tailing for more than 5x the distance from start to maximum, // future add on //and the intensity if <25% of the maximum
                            int rangeBefore = CurrentElutingPeakCollection[j].ScanMaxIntensity - CurrentElutingPeakCollection[j].ScanStart;
                            int rangeAfter = scanCounter - CurrentElutingPeakCollection[j].ScanMaxIntensity;
                            double MaxIntensity = CurrentElutingPeakCollection[j].Intensity;
                            double CurrentIntensity = currentIntensity;
                            double difference = MaxIntensity * (float)0.10 - currentIntensity;
                            if (rangeAfter > (rangeBefore * 5) && rangeBefore > 0 && difference>0)
                            {
                                CurrentElutingPeakCollection[j].ID += 1;// Active-->Being or Being-->Closed
                                CurrentElutingPeakCollection[j].NumberOfPeaks++;
                            }
                            #endregion

                            #endregion
                        }
                    }
                    #region increment all CurrentElutingPeaks that are not included in the librarty hit list a.k.a. eluting peaks without hits. Active-->Being or Being-->Closed    
                    int counter = 0;
                    for (int j = 0; j < CurrentElutingPeakCollection.Count; j++)//for each eluting peak in the collection
                    {
                        if (counter < LibraryHitsList.Count)
                        {
                            realLocationLibrary = remapLibrary[LibraryHitsList[counter]];
                            if (realLocationLibrary == j)//the library hit list was accepted and a peak was added in loop above
                            {
                                //do nothing because the eluting peak has been updated above
                                counter++;
                            }
                            else
                            {
                                //no match, increase level
                                CurrentElutingPeakCollection[j].ID += 1;// Active-->Being or Being-->Closed
                                //don't increment counter because the library hit has not been checked and needs another pass
                            }          
                        }
                        else
                        {
                            CurrentElutingPeakCollection[j].ID += 1;// Active-->Being or Being-->Closed
                        }
                    #endregion
                    }

                    break;//default case
            }
        }       
        #endregion

        private static MSPeakResult AddPeakToElutingPeak(int scanCounter, int realLocationIntex, Run MainRun)
        {
            MSPeakResult newMSPeakResult = new MSPeakResult();
            newMSPeakResult.Scan_num = scanCounter;
            MSPeak newPeak = new MSPeak();
            newMSPeakResult.MSPeak = newPeak;

            int dataLocation = realLocationIntex;
            newMSPeakResult.MSPeak.XValue = MainRun.ResultCollection.MSPeakResultList[dataLocation].MSPeak.XValue;
            newMSPeakResult.MSPeak.Height = MainRun.ResultCollection.MSPeakResultList[dataLocation].MSPeak.Height;
            newMSPeakResult.MSPeak.Width = MainRun.ResultCollection.MSPeakResultList[dataLocation].MSPeak.Width;

            return newMSPeakResult;        
        }
    }
}
