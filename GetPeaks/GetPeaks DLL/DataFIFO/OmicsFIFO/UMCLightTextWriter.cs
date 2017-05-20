using System;
using System.IO;
using System.Collections.Generic;

using PNNLOmics.Data.Features;

namespace PNNLOmics.IO
{
    /// <summary>
    /// Reads LCMSFeatureFinder output files.
    /// </summary>
    public class UMCLightTextWriter : IDataFileWriter<UMCLight>
    {
        private const string DELIMITER = "\t";

        #region Public Methods
        /// <summary>
        /// Writes the list of UMC Light data to the file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        public void WriteFile(string path, ICollection<UMCLight> data)
        {
            using (TextWriter writer = File.CreateText(path))
            {
                WriteHeader(writer);
                foreach (UMCLight feature in data)
                {
                    string dataString = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}",
                                                        Delimeter,
                                                        feature.ID,
                                                        feature.RetentionTime,
                                                        feature.NET,
                                                        feature.MassMonoisotopic,
                                                        feature.Score,
                                                        feature.Abundance,
                                                        feature.ChargeState,
                                                        feature.DriftTime);
                    writer.WriteLine(dataString);
                }
                        
            }
        }
        /// <summary>
        /// Gets or sets the string delimiter for the file.
        /// </summary>
        public string Delimeter
        { 
            get; 
            set; 
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Writes the header to the text file.
        /// </summary>
        private void WriteHeader(TextWriter writer)
        {
            string header = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}",
                                            Delimeter,
                                            "feature_index",
                                            "scan",
                                            "monoisotopic_mass",
                                            "maxabundance",
                                            "class_rep_charge",
                                            "drift_time",
                                            "conformation_fit_score");
            writer.WriteLine(header);   
        }
        /// <summary>
        /// Saves the data from a UMC csv file to an array of clsUMC Objects.
        /// </summary>
        private List<UMCLight> ReadFeatures(StreamReader reader, Dictionary<string, int> columnMap)
        {
            List<UMCLight> umcList = new List<UMCLight>();
            string line;
            UMCLight umc;
            int previousId = -99;
            int currentId = -99;
            int idIndex = 0;

            // Read the rest of the Stream, 1 line at a time, and save the appropriate data into new Objects
            while ((line = reader.ReadLine()) != null)
            {
                string[] columns = line.Split(',', '\t', '\n');

                if (columnMap.ContainsKey("Umc.Id"))
                {
                    currentId = Int32.Parse(columns[columnMap["Umc.Id"]]);
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
                    umc = new UMCLight();
                    umc.ID = currentId;

                    if (columnMap.ContainsKey("Umc.Scan")) umc.RetentionTime = int.Parse(columns[columnMap["Umc.Scan"]]);
                    if (columnMap.ContainsKey("Umc.Mass")) umc.MassMonoisotopic = double.Parse(columns[columnMap["Umc.Mass"]]);
                    if (columnMap.ContainsKey("Umc.DriftTime")) umc.DriftTime = float.Parse(columns[columnMap["Umc.DriftTime"]]);
                    if (columnMap.ContainsKey("Umc.AbundanceMax")) umc.Abundance = (long)Int32.Parse(columns[columnMap["Umc.AbundanceMax"]]);
                    if (columnMap.ContainsKey("Umc.ChargeRepresentative")) umc.ChargeState = (short)Int16.Parse(columns[columnMap["Umc.ChargeRepresentative"]]);
                    if (columnMap.ContainsKey("Umc.Score")) umc.Score = float.Parse(columns[columnMap["Umc.Score"]]);

                    umcList.Add(umc);
                    previousId = currentId;
                }
            }
            return umcList;
        }
        #endregion

        #region IDataFileWriter<UMCLight> Members


        #endregion
    }
}
