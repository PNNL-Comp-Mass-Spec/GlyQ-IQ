using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;
using System.IO;
using Synthetic_Spectra_DLL_X64_Net_4.Framework;

namespace Synthetic_Spectra_DLL_X64_Net_4.LoadFile
{
    public class FileLoadXYTextLine
    {
        /// <summary>
        /// This class loads text file data one line at a time
        /// </summary>
        /// <param name="pathSource">
        /// the file location
        /// </param>
        /// <returns>
        /// returns a list strings lists.  each string list contains comma parsed text from a given line in the file
        /// </returns>
        public List<List<string>> SingleFileByLine(string pathSource)
        {
            //set up variables
            string lineIn;          //single line loaded
            
            //set up arrays
            string[] wordArray = {""};     //array of words on a line
            List<List<string>> outputList = new List<List<string>>();
            
            StreamReader streamReadLine = new StreamReader(pathSource);
              
            // load one line at a time till end
            while ((lineIn = streamReadLine.ReadLine()) != null)
            {
                //split my line by comma and store in wordArray
                wordArray = lineIn.Split(',');

                //check first line of file
                //if(double.Parse(wordArray[0])==0)
                //{
                //    Console.WriteLine("Zero is the first point");
                //    //wordArray[0] = "g";
                //}
                List<string> stringsInARow = new List<string>();
                for (int i = 0; i < wordArray.Length; i++)
                {
                    stringsInARow.Add(wordArray[i]);
                }
                outputList.Add(stringsInARow); 
            }

            //close file
            streamReadLine.Close();

            return outputList;
        }
    }
}
