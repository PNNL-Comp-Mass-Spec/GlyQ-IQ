using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GetPeaks_DLL.DataFIFO
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
            StringBuilder sb = new StringBuilder();
            using (StreamWriter writer = new StreamWriter(outputFileDestination))
            {
                sb = new StringBuilder();
                //   sb.Append("The Data Filename is:\t" + outputLocation + "\n");

                sb.Append(columnHeader);
                writer.WriteLine(sb.ToString());

                for (int d = 0; d < dataToWrite.Count; d++)
                {
                    sb = new StringBuilder();
                    sb.Append(dataToWrite[d]);
                    writer.WriteLine(sb.ToString());
                }
            }
        }

        public void toDiskStringList(string outputLocation, List<string> dataToWrite)
        {
            string outputFileDestination = outputLocation;
            StringBuilder sb = new StringBuilder();
            using (StreamWriter writer = new StreamWriter(outputFileDestination))
            {
                sb = new StringBuilder();
                for (int d = 0; d < dataToWrite.Count; d++)
                {
                    sb = new StringBuilder();
                    sb.Append(dataToWrite[d]);
                    writer.WriteLine(sb.ToString());
                }
            }
        }
    }
}
