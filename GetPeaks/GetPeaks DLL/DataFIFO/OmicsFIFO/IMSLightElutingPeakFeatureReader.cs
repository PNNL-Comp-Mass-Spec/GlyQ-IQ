using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Features;
using System.IO;
using GetPeaks_DLL.Objects;

namespace GetPeaks_DLL.DataFIFO.OmicsFIFO
{
    public class IMSLightElutingPeakFeatureReader
    {
        #region Public Methods
        /// <summary>
        /// Reads the features from a LCMSFeatureFinder output.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ICollection<ElutingPeakLite> ReadFileIMS(string path)
        {
            List<ElutingPeakLite> features = null;
            using (StreamReader reader = new StreamReader(path))
            {
                string header = reader.ReadLine();
                Dictionary<string, int> columnMap = CreateColumnMapping(header);
                features = ReadFeatures(reader, columnMap);
            }
            return features;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Fills in the Column Map with the appropriate values.
        /// The Map will have a Column Property (e.g. ePeak.Mass) mapped to a Column Number.
        /// </summary>
        /// <returns>The column map as a Dictionary object</returns>
        private Dictionary<string, int> CreateColumnMapping(string headerLine)
        {
            Dictionary<string, int> columnMap = new Dictionary<String, int>();
            string[] columnTitles = headerLine.Split('\t', '\n');
            int numOfColumns = columnTitles.Length;

            for (int i = 0; i < numOfColumns; i++)
            {
                switch (columnTitles[i].Trim().ToLower())
                {
                    case "monoisotopic_mass":
                        columnMap.Add("ePeak.Mass", i);
                        break;
                    case "max_abundance":
                        columnMap.Add("ePeak.AbundanceMax", i);
                        break;
                    case "abundance":
                        columnMap.Add("ePeak.Abundance", i);
                        break;
                    case "scan_start":
                        columnMap.Add("ePeak.ScanStart", i);
                        break;
                    case "scan_end":
                        columnMap.Add("ePeak.ScanEnd", i);
                        break;
                    case "scan":
                        columnMap.Add("ePeak.Scan", i);
                        break;
                    case "class_rep_charge":
                        columnMap.Add("ePeak.ChargeState", i);
                        break;
                    default:
                        //Title not found.
                        break;
                }
            }

            return columnMap;
        }
        /// <summary>
        /// Saves the data from a UMC csv file to an array of clsUMC Objects.
        /// </summary>
        private List<ElutingPeakLite> ReadFeatures(StreamReader reader, Dictionary<string, int> columnMap)
        {
            List<ElutingPeakLite> umcList = new List<ElutingPeakLite>();
            string line;
            ElutingPeakLite ePeak;
            int previousId = -99;
            int currentId = -99;
            int idIndex = 0;

            // Read the rest of the Stream, 1 line at a time, and save the appropriate data into new Objects
            while ((line = reader.ReadLine()) != null)
            {
                string[] columns = line.Split(',', '\t', '\n');

                if (columnMap.ContainsKey("ePeak.Id"))
                {
                    currentId = Int32.Parse(columns[columnMap["ePeak.Id"]]);
                }
                else
                {
                    currentId = idIndex;
                    idIndex++;
                }

                /// If the UMC ID matches the previous UMC ID, then skip the UMC data.
                ///		- It is the same UMC, different peptide, and we have already stored this UMC data.
                if (previousId != currentId)
                {
                    ePeak = new ElutingPeakLite();
                    ePeak.ID = currentId;

                    if (columnMap.ContainsKey("ePeak.Mass")) ePeak.Mass = double.Parse(columns[columnMap["ePeak.Mass"]]);
                    if (columnMap.ContainsKey("ePeak.AbundanceMax")) ePeak.Intensity = float.Parse(columns[columnMap["ePeak.AbundanceMax"]]);
                    if (columnMap.ContainsKey("ePeak.Abundance")) ePeak.AggregateIntensity = float.Parse(columns[columnMap["ePeak.Abundance"]]);
                    if (columnMap.ContainsKey("ePeak.ScanStart")) ePeak.ScanStart = Int32.Parse(columns[columnMap["ePeak.ScanStart"]]);
                    if (columnMap.ContainsKey("ePeak.ScanEnd")) ePeak.ScanEnd = Int32.Parse(columns[columnMap["ePeak.ScanEnd"]]);
                    if (columnMap.ContainsKey("ePeak.Scan")) ePeak.ScanMaxIntensity = Int32.Parse(columns[columnMap["ePeak.Scan"]]);
                    if (columnMap.ContainsKey("ePeak.ChargeState")) ePeak.ChargeState = Int32.Parse(columns[columnMap["ePeak.ChargeState"]]);

                    umcList.Add(ePeak);
                    previousId = currentId;
                }
            }
            return umcList;
        }
        #endregion
    }
}
