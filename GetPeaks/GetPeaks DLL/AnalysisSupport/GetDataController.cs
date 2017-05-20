using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using PNNLOmics.Data.Features;
using GetPeaks_DLL.DataFIFO;
using GetPeaks_DLL.DataFIFO.OmicsFIFO;
using GetPeaks_DLL.Objects.TandemMSObjects;
using DeconTools.Backend.Core;
using GetPeaks_DLL.Go_Decon_Modules;

namespace GetPeaks_DLL.AnalysisSupport
{
    public class GetDataController
    {
        /// <summary>
        /// load feature data from SQLite database produced by GetPeaks
        /// </summary>
        /// <param name="dataFile"></param>
        /// <returns></returns>
        public List<FeatureLight> GetDataFeatures(InputOutputFileName dataFile)
        {
            //load data
            ILoadFile loader = new LoadOptions();
            List<FeatureLight> featureLitelist = loader.LoadFeatureLight(dataFile.InputSQLFileName);

            return featureLitelist;
        }

        /// <summary>
        /// load eluting peak data from SQLite database produced by GetPeaks
        /// </summary>
        /// <param name="dataFile"></param>
        /// <returns></returns>
        public List<ElutingPeakLite> GetDataElutingPeak(InputOutputFileName dataFile)
        {
            //load data
            ILoadFile loader = new LoadOptions();
            
            List<ElutingPeakLite> elutingPeakList = loader.LoadElutingPeakLite(dataFile.InputSQLFileName);

            return elutingPeakList;
        }

        /// <summary>
        /// load library file for comparing to
        /// </summary>
        /// <param name="libraryFile"></param>
        /// <returns></returns>
        public List<DataSet> GetDataLibrary(InputOutputFileName libraryFile)
        {
            //load library
            FileLoadController newFileLoadController = new FileLoadController();
            List<DataSet> newLibraryDataset = newFileLoadController.GetData(libraryFile.InputFileName, GetPeaks_DLL.DataFIFO.FileIterator.deliminator.Comma);

            return newLibraryDataset;
        }

        /// <summary>
        /// load Isotopes from SQLite database file produced from getPeaks
        /// </summary>
        /// <param name="dataFile"></param>
        /// <returns></returns>
        public List<IsotopeObject> GetIsotopes(InputOutputFileName dataFile)
        {
            //load library
            ILoadFile loader = new LoadOptions();
            
            List<IsotopeObject> newIsotopeDataset = loader.LoadIsotopesData(dataFile.InputSQLFileName);

            return newIsotopeDataset;
        }

        /// <summary>
        /// load featureLight data from a IMS feature text file
        /// </summary>
        /// <param name="dataFile"></param>
        /// <returns></returns>
        public List<FeatureLight> GetDataFeaturesIMS(InputOutputFileName dataFile)
        {
            //load data
            ILoadFile loader = new LoadOptions();
            List<FeatureLight> featureLitelist = loader.LoadFeatureLightIMS(dataFile.InputFileName);

            return featureLitelist;
        }

        /// <summary>
        /// load ElutingPeaks data from a IMS feature text file
        /// </summary>
        /// <param name="dataFile"></param>
        /// <returns></returns>
        public List<ElutingPeakLite> GetDataElutingPeakIMS(InputOutputFileName dataFile)
        {
            //load data
            ILoadFile loader = new LoadOptions();
            List<ElutingPeakLite> elutingPeakList = loader.LoadElutingPeakLiteIMS(dataFile.InputFileName);
            foreach (ElutingPeakLite ePeak in elutingPeakList)
            {
                ePeak.SummedIntensity = (float)ePeak.AggregateIntensity;
            }

            return elutingPeakList;
        }

        
        /// <summary>
        /// load Tandem MS data from data file
        /// </summary>
        /// <param name="dataFile"></param>
        /// <returns></returns>
        public List<TandemObject> GetTandemData(InputOutputFileName dataFile, int precursorScan, ParametersTHRASH parametersThrash)
        {
            Run run = GoCreateRun.CreateRun(dataFile);
            int sizeOfDatabase = run.GetNumMSScans() - 1;//TODO this is a crude way to get the size of the database.  -1 is need or the scan set creation will fail
            run.Dispose();

            LoadTandemData loader = new LoadTandemData();
            List<TandemObject> tandemData = loader.LoadData(dataFile.InputFileName, sizeOfDatabase, precursorScan, parametersThrash);
            loader.Dispose();
            return tandemData;
        }
    }
}
