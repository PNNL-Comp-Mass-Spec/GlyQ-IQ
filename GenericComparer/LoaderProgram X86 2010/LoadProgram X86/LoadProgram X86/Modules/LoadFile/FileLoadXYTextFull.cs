//Scott Kronewitter, 3-17-2010
//Loads a text file to memory one file at a time. (mass,intensity) 
//Classes:  Program.cs, Importer.cs, LoadTextFull.cs, LoadTextLine.cs

using System;
using System.Collections.Generic;   //List
using System.IO;                    //Stream Reader
using PNNLOmics.Data;

namespace ConsoleApplication1
{
    class FileLoadXYTextFull
    {
        //parameter1=file path
        public void SingleFileAtOnce(string pathSource, List<XYData> XYList, PrintBuffer print)
        {

            //set up variables
            Int32 length;       //number of data points
            Int32 i;            //counter

            //set up arrays
            string[] lines;     //array of lines
            string[] wordArray; //array of words on a line

            //set up Lists
            //List<XYData> XYList = new List<XYData>();
            //ArrayList myarray = new ArrayList();//slower because of boxing and unboxing

            //__________________________________________________________________________________
            //Create a streamReader in one go
            StreamReader streamReadFile = new StreamReader(pathSource);

            //Read in entire file to fileContents
            string fileContents = streamReadFile.ReadToEnd();

            //close file
            streamReadFile.Close();

            //__________________________________________________________________________________
            //parse file by lines
            lines = fileContents.Split(
                Environment.NewLine.ToCharArray(),  //'\n', '\r'
                StringSplitOptions.RemoveEmptyEntries);//does not include empty lines

            //calculate number of lines in file
            length = lines.Length;
            
            //parese lines and store into list arrays
            for (i = 0; i < length; i += 1)
            {
                print.AddLine("line=" + lines[i]);
                
                //split my line by comma and store in wordArray
                wordArray = lines[i].Split(',');

                XYData XYDataPoint = new XYData(double.Parse(wordArray[0]), double.Parse(wordArray[1]));
                XYList.Add(XYDataPoint);
            }
            print.AddLine("\n");

            PrintToConsole(XYList, print);//private method
            return;
        }

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
