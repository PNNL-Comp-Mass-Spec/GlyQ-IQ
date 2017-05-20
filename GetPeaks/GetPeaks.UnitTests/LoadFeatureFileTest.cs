using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.Objects;

namespace GetPeaks.UnitTests
{
    class LoadFeatureFileTest
    {
         /// <summary>
        /// Load a ViperFeatureFile from am text file
        /// </summary>
        [Test]
        public void WriteGetPeaksFile()
        {
            string filename = @"D:\PNNL\Projects\DATA 2012_03_08 Velos 4 Serum Cell Ant Rat\2_ViperFeatures\LCMSFeaturesInView_Ant01_3X.txt";
            
            //IRapidCompare compareHere = new CompareContrast2();
            ILoadFile loader = new LoadOptions();
            string columnheaders;

            List<FeatureViper> loadedViperFeatures = loader.LoadFeatureViper(filename, out columnheaders);

            int count = loadedViperFeatures.Count;
            Assert.AreEqual(count, 10292);
        }

        [Test]
        public void ConvertViperToIgor()
        {
            
            
            string folder = @"D:\PNNL\Projects\DATA 2012_03_08 Velos 4 Serum Cell Ant Rat\2_ViperFeatures\";
            
            //string filenameIn = folder + "LCMSFeaturesInView_Ant01_3X.txt";
            //string filenameOut = "Ant01_3X.txt";

            

            //string filenameIn = folder + "LCMSFeaturesInView_SN107_3X HCD45.txt";
            //string filenameOut = "SN107_3X HCD45.txt";

            string filenameIn = folder + "LCMSFeaturesInView_SL26_100ppm.txt";
            string filenameOut = "SL26_100ppm.txt";

            //IRapidCompare compareHere = new CompareContrast2();
            ILoadFile loader = new LoadOptions();
            string columnheaders;

            List<FeatureViper> loadedViperFeatures = loader.LoadFeatureViper(filenameIn, out columnheaders);

            int count = loadedViperFeatures.Count;
            //Assert.AreEqual(count, 10292);

            List<string> dataToWrite = new List<string>();
            foreach (FeatureViper feature in loadedViperFeatures)
            {
                string line = feature.UMCMonoMW + "\t" + feature.ScanClassRep;
                dataToWrite.Add(line);
            }

            StringListToDisk writer = new StringListToDisk();
            writer.toDiskStringList(folder + filenameOut, dataToWrite, "0\t0");
        }
    }
}
