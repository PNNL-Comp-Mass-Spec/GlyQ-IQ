using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

namespace ConsoleApplication1
{
    public class AccelerateBlockFX
    {
        public void breakupXYData(List<decimal> data, int totalcores, List<ListResults> dividedDataPieces, List<LibraryIndexStartStop> lotsOfStartStopData)
        {
            int j;
            int k;

            if (totalcores <= data.Count)
            {
                for (j = 0; j < totalcores; j++)
                {
                    // chunkSize = size / cores;
//                    Console.WriteLine("there are " + data.Count + " librarypoints possible");
//                    Console.WriteLine("there are " + data.Count + " datapoints possible");

                    double chunkSize = Math.Floor((double)data.Count / (double)totalcores);
                    double remainder = (data.Count - (chunkSize * totalcores));

//                    Console.WriteLine("chunk size is " + (int)chunkSize + ". The rank is " + j + ". Remainder=" + (int)remainder);

                    double minRange;//I am not sure these need to be doubles
                    double maxRange;

                    if (j == 0)
                    {
                        minRange = chunkSize * j;
                        maxRange = chunkSize * (j + 1) + remainder;
                    }
                    else
                    {
                        minRange = chunkSize * j + remainder;
                        maxRange = chunkSize * (j + 1) + remainder;
                    }

//                    Console.WriteLine("the range is " + minRange + " to " + maxRange + " for process " + j);

                    lotsOfStartStopData[j].startIndex = (int)minRange;
                    lotsOfStartStopData[j].stopIndex = (int)maxRange-1;

                    List<int> SubIndexList = new List<int>();
                    List<XYData> SubMassList = new List<XYData>();

                    for (k = (int)minRange; k < (int)maxRange; k++)
                    {
                        XYData XY = new XYData();
                        XY.X = (double)data[k];
                        SubIndexList.Add(k);
                        SubMassList.Add(XY);
                    }

                    ListResults newLR = new ListResults();
                    newLR.AddListInt(SubIndexList);
                    newLR.AddListXY(SubMassList);

                    dividedDataPieces.Add(newLR);

                }
            }
            else//assign one point per core 
            {
                //fail fail fail
                int y;
                y = 6*4;
                y = y++;
            }
        }

        public void breakupXYLibrary(List<decimal> libraryData, LibraryIndexStartStop startStop, int currentCore, List<ListResults> ResultList, List<DataSetSK> miniDataSet)
        {
            //int i; 
            int k;
            
            DataSetSK data = new DataSetSK();
                data.XYList = ResultList[currentCore].XYList;

            DataSetSK blankData = new DataSetSK();

            DataSetSK library = new DataSetSK();
            
            List<XYData> LibraryXYData = new List<XYData>();
                
            //add full library

                bool isFullLibrary;
                isFullLibrary = true;
                //isFullLibrary = false;    

                if (isFullLibrary)
                {
                    for (k = 0; k < libraryData.Count; k++)
                    {
                        XYData XY = new XYData();
                        XY.X = (double)libraryData[k];
                        LibraryXYData.Add(XY);
                    }
                    library.XYList = LibraryXYData;
                }

                else
                {
                    //add mini libraries

                    library.XYList = LibraryXYData;
                    //int lowest;
                    double startingMass;
                    double endingMass;


                    //we need to be able to store intexes for each core and then add or subtract to cover the seams
                    startingMass = data.XYList[0].X;
                    endingMass = data.XYList[data.XYList.Count - 1].X;

                    //startStop.stopIndex = 3;

                    if ((decimal)endingMass > libraryData[0] && (decimal)startingMass < libraryData[libraryData.Count - 1])//make sure data points are in the library
                    {
                        int startAssigned = 0;
                        int stopAssigned = 0;
                        for (k = 0; k < libraryData.Count; k++)
                        {

                            if (libraryData[k] > (decimal)startingMass)
                            {
                                if (startAssigned == 0)
                                {
                                    if (k > 0)//cant backup for the first point
                                    {
                                        startStop.startIndex = k - 1;//-1 includes one extra point before range
                                        startAssigned = 1;
                                    }
                                    else
                                    {
                                        startStop.startIndex = 0;
                                        startAssigned = 1;
                                    }
                                }
                            }

                            if (libraryData[k] >= (decimal)endingMass)
                            {
                                if (stopAssigned == 0)
                                {
                                    if (k < libraryData.Count)//cand include an extra one if we are at the end of the library
                                    {
                                        startStop.stopIndex = k + 1;//+1 includes one extra point beyonde range
                                    }
                                    else
                                    {
                                        startStop.stopIndex = libraryData.Count;
                                    }
                                    stopAssigned = 1;
                                    k = libraryData.Count;//end loop now that stop is assigned
                                }
                            }
                        }
                        if (startStop.stopIndex > startStop.startIndex)
                        {
                            for (k = startStop.startIndex; k < startStop.stopIndex; k++)
                            {
                                XYData XY = new XYData();
                                XY.X = (double)libraryData[k];
                                LibraryXYData.Add(XY);
                            }
                        }
                    }
                    else
                    {
                        //provide gimme point
                        XYData XY = new XYData();
                        XY.X = (double)libraryData[0];
                        LibraryXYData.Add(XY);
                    }
                }
                
            miniDataSet.Add(data);
            miniDataSet.Add(blankData);
            miniDataSet.Add(library);
        }
    }
}
