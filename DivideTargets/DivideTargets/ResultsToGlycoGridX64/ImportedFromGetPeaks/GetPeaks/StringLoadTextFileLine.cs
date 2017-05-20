using System.Collections.Generic;
using System.IO;

namespace ResultsToGlycoGridX64.ImportedFromGetPeaks.GetPeaks
{
    public class StringLoadTextFileLine
    {
        public List<string> SingleFileByLine(string pathSource)
        {
            //set up variables
            string lineIn;          //single line loaded

            //set up List
            List<string> nonParsedStrings = new List<string>();

            //Create a streamReader one line at at time
            using(StreamReader streamReadLine = new StreamReader(pathSource))
            {
                // load one line at a time till end
                while ((lineIn = streamReadLine.ReadLine()) != null)
                {
                   nonParsedStrings.Add(lineIn.ToString());
                }

                 //close file
                 streamReadLine.Close();
            }
            return nonParsedStrings;
        }
    }
}
