using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using GetPeaks_DLL.Functions;
using GetPeaks_DLL.SQLite;
using PNNLOmics.Data.Features;
using GetPeaks_DLL.DataFIFO.OmicsFIFO;

namespace GetPeaks_DLL.DataFIFO
{
    /// <summary>
    /// Usage
    /// ILoadFile loader = new LoadOptions();
    /// List<FeatureViper> loadedViperFeatures = loader.LoadFeatureViper(@"D:\PNNL\Projects\DATA 2012_03_08 Velos 4 Serum Cell Ant Rat\2_ViperFeatures\LCMSFeaturesInView_Ant01_3X.txt");
 
    /// </summary>
    
    
    public interface ILoadFile
    {
        List<FeatureViper> LoadFeatureViper(string fileName, out string columnHeaders);
        List<FeatureIMS> LoadFeatureIMS(string fileName, out string columnHeaders);
        List<FeatureMultiAlign> LoadFeatureMultiAlign(string filename, out string columnHeaders);
        List<IsosObject> LoadIsosObject(string fileName, out string columnHeaders);
        List<FeatureLight> LoadFeatureLight(string fileName);
        List<FeatureLight> LoadFeatureLightIMS(string fileName);
        List<IsotopeObject> LoadIsotopesData(string fileName);
        List<ElutingPeakLite> LoadElutingPeakLite(string fileName);
        List<ElutingPeakLite> LoadElutingPeakLiteIMS(string fileName);
    }

    public class LoadOptions : ILoadFile
    {
        List<FeatureViper> ILoadFile.LoadFeatureViper(string fileName, out string columnHeaders)
        {
            Console.WriteLine("Load FeatureViper Data");

            List<FeatureViper> featureViperList = new List<FeatureViper>();

            GetPeaks_DLL.DataFIFO.FileIterator.deliminator deliminatorFiletype;
            deliminatorFiletype = FileIterator.deliminator.Tab;

            StringLoadTextFileLine loadSpectraL = null;
            loadSpectraL = new StringLoadTextFileLine();
            List<string> stringListFromFiles = new List<string>();

            //load strings
            stringListFromFiles = loadSpectraL.SingleFileByLine(fileName);//loads all isos

            Console.WriteLine("Load file: " + fileName + "\n");

            //grab column header
            string isotopeColumnHeader = stringListFromFiles[0];

            #region load data and parse it
            ParseStringListToViperFeature newParser = new ParseStringListToViperFeature();

            List<FeatureViper> loadedViperFeatures;
            
            newParser.Parse(stringListFromFiles, deliminatorFiletype, out loadedViperFeatures, out columnHeaders);

            #endregion

            int fileLength = loadedViperFeatures.Count;

            Console.WriteLine("     " + fileLength + " Features loaded");

            return loadedViperFeatures;
        }

        List<FeatureIMS> ILoadFile.LoadFeatureIMS(string fileName, out string columnHeaders)
        {
            Console.WriteLine("Load FeatureIMS Data");

            List<FeatureIMS> featureIMSList = new List<FeatureIMS>();

            GetPeaks_DLL.DataFIFO.FileIterator.deliminator deliminatorFiletype;
            deliminatorFiletype = FileIterator.deliminator.Tab;

            StringLoadTextFileLine loadSpectraL = null;
            loadSpectraL = new StringLoadTextFileLine();
            List<string> stringListFromFiles = new List<string>();

            //load strings
            stringListFromFiles = loadSpectraL.SingleFileByLine(fileName);//loads all isos

            Console.WriteLine("Load file: " + fileName + "\n");

            //grab column header
            string isotopeColumnHeader = stringListFromFiles[0];

            #region load data and parse it
            ParseStringListToIMSFeature newParser = new ParseStringListToIMSFeature();

            List<FeatureIMS> loadedIMSFeatures;

            newParser.Parse(stringListFromFiles, deliminatorFiletype, out loadedIMSFeatures, out columnHeaders);

            #endregion

            int fileLength = featureIMSList.Count;

            Console.WriteLine("     " + fileLength + " Features loaded");

            return loadedIMSFeatures;
        }

        List<FeatureMultiAlign> ILoadFile.LoadFeatureMultiAlign(string fileName, out string columnHeaders)
        {
            Console.WriteLine("Load FeatureLight Data");
            bool didThisWork = false;

            DatabaseLayer newDatabaseLayer = new DatabaseLayer();
            int databaseSize = newDatabaseLayer.ReadDatabaseSize(fileName, "TableInfo", "Count");

            List<FeatureMultiAlign> featureMultiAlignList = new List<FeatureMultiAlign>();

            DatabaseReader newReader = new DatabaseReader();

            Console.WriteLine("     Loading...");
            for (int i = 1; i < databaseSize + 1; i++)//+1 to prevent zero index
            //for (int i = 1; i < databaseSize - 1; i++)//TODO when database islocked the count isnot correct
            {
                FeatureMultiAlign loadedFeature;
                didThisWork = newReader.readMultiAllignFeatrueData(fileName, i, out loadedFeature);
                loadedFeature.Feature_ID = i;
                featureMultiAlignList.Add(loadedFeature);

                //Console.WriteLine(didThisWork.ToString() + " " + i + "/" + databaseSize);
            }
            Console.WriteLine("     " + databaseSize + " Features loaded");

            //TODO set this up
            columnHeaders = "Not set up yet";

            return featureMultiAlignList;
        }

        List<IsosObject> ILoadFile.LoadIsosObject(string fileName, out string columnHeaders)
        {

            List<IsosObject> isosList = new List<IsosObject>();
            GetPeaks_DLL.DataFIFO.FileIterator.deliminator deliminatorFiletype;
            deliminatorFiletype = FileIterator.deliminator.Comma;

            StringLoadTextFileLine loadSpectraL = null;
            loadSpectraL = new StringLoadTextFileLine();
            List<string> stringListFromFiles = new List<string>();

            //load strings
            stringListFromFiles = loadSpectraL.SingleFileByLine(fileName);//loads all isos

            Console.WriteLine("Load file: " + fileName + "\n");

            //grab column header
            string isotopeColumnHeader = stringListFromFiles[0];

            #region load data and parse it
            ParseStringListToIsos newParser = new ParseStringListToIsos();

            List<IsosObject> loadedIsos;

            newParser.Parse(stringListFromFiles, deliminatorFiletype, out loadedIsos, out columnHeaders);

            #endregion

            return loadedIsos;
        }

        List<FeatureLight> ILoadFile.LoadFeatureLight(string fileName)
        {
            Console.WriteLine("Load FeatureLight Data");
            bool didThisWork = false;

            DatabaseLayer newDatabaseLayer = new DatabaseLayer();
            int databaseSize = newDatabaseLayer.ReadDatabaseSize(fileName, "TableInfo", "Count");

            List<FeatureLight> featureLightList = new List<FeatureLight>();

            DatabaseReader newReader = new DatabaseReader();

            Console.WriteLine("     Loading...");
            for (int i = 1; i < databaseSize + 1; i++)//+1 to prevent zero index
            //for (int i = 1; i < databaseSize - 1; i++)//TODO when database islocked the count isnot correct
            {
                FeatureLight loadedFeature;
                didThisWork = newReader.readFeatrueLiteData(fileName, i, out loadedFeature);
                loadedFeature.ID = i;
                featureLightList.Add(loadedFeature);

                //Console.WriteLine(didThisWork.ToString() + " " + i + "/" + databaseSize);
            }
            Console.WriteLine("     " + databaseSize + " Features loaded");

            return featureLightList;
        }

        List<FeatureLight> ILoadFile.LoadFeatureLightIMS(string fileName)
        {
            Console.WriteLine("Load FeatureLight Data");
            //            bool didThisWork = false;

            List<FeatureLight> featureLightList = new List<FeatureLight>();
            ICollection<UMCLight> featuresIn = null;
            List<UMCLight> featuresInList = null;

            IMSLightFeatureReader newReader = new IMSLightFeatureReader();
            featuresIn = newReader.ReadFile(fileName);
            featuresInList = featuresIn.ToList();

            for (int i = 0; i < featuresIn.Count; i++)
            {
                UMCLight eUMCLight = featuresInList[i];
                FeatureLight newFeatureLight = new FeatureLight();
                newFeatureLight.Abundance = eUMCLight.Abundance;
                newFeatureLight.ChargeState = eUMCLight.ChargeState;
                newFeatureLight.DriftTime = eUMCLight.DriftTime;
                newFeatureLight.MassMonoisotopic = eUMCLight.MassMonoisotopic;
                newFeatureLight.NET = eUMCLight.NET;
                newFeatureLight.RetentionTime = eUMCLight.RetentionTime;
                newFeatureLight.Score = eUMCLight.Score;//average fit score
                featureLightList.Add(newFeatureLight);
            }
            //          Console.WriteLine("     " + databaseSize + " Features loaded");

            return featureLightList;
        }

        List<IsotopeObject> ILoadFile.LoadIsotopesData(string fileName)
        {
            Console.WriteLine("Load Isotopes Data");
            bool didThisWork = false;

            DatabaseLayer newDatabaseLayer = new DatabaseLayer();
            int databaseSize = newDatabaseLayer.ReadDatabaseSize(fileName, "TableInfo", "Count");

            List<IsotopeObject> isotopeObjectList = new List<IsotopeObject>();

            DatabaseReader newReader = new DatabaseReader();

            Console.WriteLine("     Loading...");
            for (int i = 1; i < databaseSize + 1; i++)//+1 to prevent zero index
            {
                IsotopeObject loadedIsotopeObject;
                didThisWork = newReader.readIsotopeDataOld(fileName, i, out loadedIsotopeObject);

                isotopeObjectList.Add(loadedIsotopeObject);

                //Console.WriteLine(didThisWork.ToString() + " " + i + "/" + databaseSize);
            }
            Console.WriteLine("     " + databaseSize + " Isotopes loaded");

            return isotopeObjectList;
        }

        List<ElutingPeakLite> ILoadFile.LoadElutingPeakLite(string fileName)
        {
            Console.WriteLine("Load ElutingPeakLite Data");
            bool didThisWork = false;

            DatabaseLayer newDatabaseLayer = new DatabaseLayer();
            int databaseSize = newDatabaseLayer.ReadDatabaseSize(fileName, "TableInfo", "Count");

            DatabaseReader newReader = new DatabaseReader();

            Console.WriteLine("     Loading...");
            List<ElutingPeakLite> elutingPeakLiteList = new List<ElutingPeakLite>();
            for (int i = 1; i < databaseSize + 1; i++)//+1 to prevent zero index
            {
                ElutingPeakLite loadedElutingPeak;
                didThisWork = newReader.readElutingPeakdata(fileName, i, out loadedElutingPeak);
                loadedElutingPeak.ID = i;
                elutingPeakLiteList.Add(loadedElutingPeak);
                //Console.WriteLine(didThisWork.ToString() + " " + i + "/" + databaseSize);
            }

            Console.WriteLine("     " + databaseSize + " ElutingPeakLites loaded");
            return elutingPeakLiteList;
        }

        List<ElutingPeakLite> ILoadFile.LoadElutingPeakLiteIMS(string fileName)
        {
            Console.WriteLine("Load IMS ElutingPeakLite Data");
            //          bool didThisWork = false;

            List<ElutingPeakLite> elutingPeakLiteList = new List<ElutingPeakLite>();
            ICollection<ElutingPeakLite> elutingPeakLiteCollection = null;

            IMSLightElutingPeakFeatureReader loadElutingPeaks = new IMSLightElutingPeakFeatureReader();
            elutingPeakLiteCollection = loadElutingPeaks.ReadFileIMS(fileName);
            elutingPeakLiteList = elutingPeakLiteCollection.ToList();

            Console.WriteLine("     " + elutingPeakLiteList.Count + " ElutingPeakLites loaded");
            return elutingPeakLiteList;
        }
    }
}
