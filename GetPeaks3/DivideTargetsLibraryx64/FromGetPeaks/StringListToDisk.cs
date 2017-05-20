using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DivideTargetsLibraryX64.FromGetPeaks
{
    public class StringListToDisk
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputLocation">file location to write the file to</param>
        /// <param name="dataToWrite">list of strings to write</param>
        /// <param name="columnHeader">list of strings for each column</param>
        public void toDiskStringList(string outputLocation, List<string> dataToWrite, string columnHeader)
        {
            string outputFileDestination = outputLocation;
	        using (var writer = new StreamWriter(outputFileDestination))
            {
				writer.WriteLine(columnHeader);

                for (int d = 0; d < dataToWrite.Count; d++)
                {
					writer.WriteLine(dataToWrite[d]);
                }
            }
        }

        public void toDiskStringList(string outputLocation, List<string> dataToWrite)
        {
            string outputFileDestination = outputLocation;
            
            using (var writer = new StreamWriter(outputFileDestination))
            {
                
                for (int d = 0; d < dataToWrite.Count; d++)
                {
					writer.WriteLine(dataToWrite[d]);
                }
            }
        }
    }
}
