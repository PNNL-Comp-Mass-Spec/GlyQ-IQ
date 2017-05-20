using System;
using System.Collections.Generic;
using System.Linq;
using InformedProteomics.Backend.Data.Spectrometry;
using InformedProteomics.Backend.MassSpecData;
using Run64IP;

namespace TestIpRunx64
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing X64");
            //string filename = @"L:\PNNL Files\PNNL\Projects\GlyQ-IQ Paper\Part 1\Dta Nov\FE_S_SN129_1\ResultsSummary\S_SN129_1.raw";
            string filename = @"D:\Csharp\0_TestDataFiles\F_Std_S_SN129_1\RawData\S_SN129_1.raw";

            XCaliburReader reader = new XCaliburReader(filename);

            LcMsRun myFirstRun = new LcMsRun(reader);

            SumProfileSpectra summer = new SumProfileSpectra();

            //List<int> scans = new List<int>();
            //scans.Add(1000);
            //scans.Add(1001);
            //scans.Add(1002);

            //List<Peak[]> firstSpectra;
            
            //Peak[] summedSpectrum = summer.getSummedSpectrum(ref reader, scans, out firstSpectra);
            //Console.WriteLine("Ther are " + summedSpectrum.Count() + " points");
            ////Console.WriteLine("Ther are " + myFirstRun.MaxLcScan + " scans");


            //StringListToDisk writer = new StringListToDisk();
            //List<string> summedResults =SetUpPeaksToWrite(summedSpectrum);
            //List<string> spectra0 = SetUpPeaksToWrite(firstSpectra[0]);
            //List<string> spectra1 = SetUpPeaksToWrite(firstSpectra[1]);
            //List<string> spectra2 = SetUpPeaksToWrite(firstSpectra[2]);
            //writer.toDiskStringList(@"Y:\peaksS.txt", summedResults);
            //writer.toDiskStringList(@"Y:\peaks0.txt", spectra0);
            //writer.toDiskStringList(@"Y:\peaks1.txt", spectra1);
            //writer.toDiskStringList(@"Y:\peaks2.txt", spectra2);
        }

        private static List<string> SetUpPeaksToWrite(Peak[] summedSpectrum)
        {
            List<string> results = new List<string>();
            foreach (var peak in summedSpectrum)
            {
                results.Add(peak.Mz + "," + peak.Intensity);
            }

            return results;
        }
    }
}
