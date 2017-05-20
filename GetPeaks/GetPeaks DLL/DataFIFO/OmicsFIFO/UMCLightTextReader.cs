using System;
using System.IO;
using System.Collections.Generic;

using PNNLOmics.Data.Features;

namespace PNNLOmics.IO
{
    /// <summary>
    /// Reads LCMSFeatureFinder output files.
    /// </summary>
    public class UMCLightTextReader : IDataFileReader<UMCLight>
	{
        #region Public Methods
        /// <summary>
        /// Reads the features from a LCMSFeatureFinder output.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ICollection<UMCLight> ReadFile(string path)
        {
            List<UMCLight> features = null;
            using (StreamReader reader = new StreamReader(path))
            {
                string header = reader.ReadLine();
                Dictionary<string, int> columnMap = CreateColumnMapping(header);
                features      = ReadFeatures(reader, columnMap);
            }
            return features;    
        }
        #endregion

        #region Private Methods
        /// <summary>
		/// Fills in the Column Map with the appropriate values.
		/// The Map will have a Column Property (e.g. Umc.Mass) mapped to a Column Number.
		/// </summary>
		/// <returns>The column map as a Dictionary object</returns>
		private Dictionary<string, int> CreateColumnMapping(string headerLine)
		{            
			Dictionary<string, int> columnMap = new Dictionary<String, int>();
			string[] columnTitles             = headerLine.Split('\t', '\n');
			int numOfColumns                  = columnTitles.Length;

			for (int i = 0; i < numOfColumns; i++)
			{
				switch (columnTitles[i].Trim().ToLower())
				{
					case "feature_index":
						columnMap.Add("Umc.Id", i);
						break;
					case "scan":
						columnMap.Add("Umc.Scan", i);
						break;
					case "monoisotopic_mass":
						columnMap.Add("Umc.Mass", i);
						break;
					case "maxabundance":
						columnMap.Add("Umc.AbundanceMax", i);
						break;
					case "class_rep_charge":
						columnMap.Add("Umc.ChargeRepresentative", i);
						break;
					case "drift_time":
						columnMap.Add("Umc.DriftTime", i);
						break;
					case "conformation_fit_score":
						columnMap.Add("Umc.Score", i);
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
        private List<UMCLight> ReadFeatures(StreamReader reader, Dictionary<string, int> columnMap)
		{
            List<UMCLight> umcList = new List<UMCLight>();
			string  line;
            UMCLight umc;
			int     previousId   = -99;
			int     currentId    = -99;
			int     idIndex      = 0;

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

					if (columnMap.ContainsKey("Umc.Scan"))					umc.RetentionTime           = int.Parse(columns[columnMap["Umc.Scan"]]);
					if (columnMap.ContainsKey("Umc.Mass"))					umc.MassMonoisotopic        = double.Parse(columns[columnMap["Umc.Mass"]]);					
                    if (columnMap.ContainsKey("Umc.DriftTime"))				umc.DriftTime               = float.Parse(columns[columnMap["Umc.DriftTime"]]);
					if (columnMap.ContainsKey("Umc.AbundanceMax"))          umc.Abundance               = (long) Int32.Parse(columns[columnMap["Umc.AbundanceMax"]]);                        
                    if (columnMap.ContainsKey("Umc.ChargeRepresentative"))  umc.ChargeState             = (short)Int16.Parse(columns[columnMap["Umc.ChargeRepresentative"]]);                                                                
					if (columnMap.ContainsKey("Umc.Score"))					umc.Score                   = float.Parse(columns[columnMap["Umc.Score"]]);

					umcList.Add(umc);
					previousId = currentId;
				}
			}
			return umcList;
		}
		#endregion
	}
}
