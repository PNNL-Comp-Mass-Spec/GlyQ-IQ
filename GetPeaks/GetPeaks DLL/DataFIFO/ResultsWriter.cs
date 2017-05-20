using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using System.IO;
using GetPeaks_DLL.Functions;
using GetPeaks_DLL.Objects.ResultsObjects;

namespace AnalyzePeaks.DataFIFO
{
    public class ResultsWriter
    {
        public void toDiskSingle(SampleResutlsObject dataToWrite, InputOutputFileName outputLocation, InputOutputFileName librarInfo)
        {
            string outputFileDestination = outputLocation.OutputFileName;
            //write data to file
            StringBuilder sb = new StringBuilder();
            using (StreamWriter writer = new StreamWriter(outputFileDestination))
            {
                sb = new StringBuilder();
                sb.Append("The Data Filename is:\t" + outputLocation.InputSQLFileName + "\n");

                sb.Append("The Library Used is:\t" + librarInfo.InputFileName + "\n");

                sb.Append("Number of Hits:\t" + dataToWrite.NumberOfHits + "\n");

                sb.Append("exatc mass\tneutral mass\tppm error\tsummed volume\tpeaks\n");
                writer.WriteLine(sb.ToString());
                double ppm;
                for (int d = 0; d < dataToWrite.AppendedLibrary.Count; d++)
                {
                    sb = new StringBuilder();
                    sb.Append(dataToWrite.AppendedLibrary[d]); sb.Append("\t");
                    sb.Append(dataToWrite.AppendedFeatureList[d].MassMonoisotopic); sb.Append("\t");
                    ppm = ErrorCalculator.PPMExact(dataToWrite.AppendedLibrary[d],dataToWrite.AppendedFeatureList[d].MassMonoisotopic);
                    sb.Append(ppm); sb.Append("\t");
                    sb.Append(dataToWrite.AppendedElutingPeakList[d].SummedIntensity); sb.Append("\t");
                    sb.Append(dataToWrite.AppendedElutingPeakList[d].NumberOfPeaks); sb.Append("\t");
                    

                    writer.WriteLine(sb.ToString());
                }

            }
        }

        public void toDiskStringList(string outputLocation, List<string> dataToWrite, string columnHeader)
        {
            string outputFileDestination = outputLocation;
            StringBuilder sb = new StringBuilder();
            using (StreamWriter writer = new StreamWriter(outputFileDestination))
            {
                sb = new StringBuilder();
                sb.Append("The Data Filename is:\t" + outputLocation + "\n");

                sb.Append(columnHeader +"\n");
                writer.WriteLine(sb.ToString());

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
