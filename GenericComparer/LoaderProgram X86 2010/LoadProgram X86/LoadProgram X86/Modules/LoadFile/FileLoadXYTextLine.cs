//Scott Kronewitter, 3-17-2010
//Loads a text file to memory one line at a time. (mass,intensity) 
//Classes:  Program.cs, Importer.cs, LoadTextFull.cs, LoadTextLine.cs


using System;
using System.Collections.Generic;   //List
using System.IO;                    //Stream Reader
using PNNLOmics.Data;

namespace ConsoleApplication1
{
    class FileLoadXYTextLine
    {
        #region Public Methods

        //parameter1=file path
        public void SingleFileByLine(string pathSource, List<XYData> XYList, PrintBuffer print)
        {
            //set up variables
            String lineIn;          //single line loaded

            //set up arrays
            string[] wordArray;     //array of words on a line

            //set up List
            //List<XYData> XYList = new List<XYData>();
            //List<FeatureSynthetic> featureList = new List<FeatureSynthetic>();

            //ArrayList myarray = new ArrayList();//slower because of boxing and unboxing
                  
            //Create a streamReader one line at at time
            StreamReader streamReadLine = new StreamReader(pathSource);
              
            // load one line at a time till end
            //int i=0;
            while ((lineIn = streamReadLine.ReadLine()) != null)
            {
    //            print.AddLine("line=" + lineIn);

                //split my line by comma and store in wordArray
                wordArray = lineIn.Split(',');

                //check first line of file
                if(double.Parse(wordArray[0])==0)
                {
                    Console.WriteLine("Zero is the first point");
                    //wordArray[0] = "g";
                }

                XYData XYDataPoint = new XYData(double.Parse(wordArray[0]), double.Parse(wordArray[1]));
                XYList.Add(XYDataPoint);
            }

            //close file
            streamReadLine.Close();

            //PrintToConsole(XYList, print);
            return;
        }

        #endregion

        #region private methods

        private void PrintToConsole(List<XYData> XYList, PrintBuffer print)
        {
            foreach (XYData XYDataPoint in XYList)
            {
                print.AddLine("The mass is " + XYDataPoint.X + " and the Int is " + XYDataPoint.Y);
            }
        }

        #endregion
    }
}
