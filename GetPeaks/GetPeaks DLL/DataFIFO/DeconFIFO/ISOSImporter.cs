using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Data;
using DeconTools.Backend.Core;
using PNNLOmics.Data.Features;
using DeconTools.Backend;

namespace GetPeaks_DLL.DataFIFO.DeconFIFO
{
    public class ISOSImporterGetPeaks
    {
        public void Import(string fileName, out List<FeatureLight> OmicsFriendlyFeatureList, out List<int> ScanSet)
        {         
            Globals.MSFileType instrumentType = new Globals.MSFileType();

            List<IsosResult> importedData = new List<IsosResult>();
            OmicsFriendlyFeatureList = new List<FeatureLight>();
            ScanSet = new List<int>();

            instrumentType = Globals.MSFileType.Finnigan;

            int IDcounter = 0;
            switch (instrumentType)
            {
                case Globals.MSFileType.Finnigan:
                    {
                        IsosImporter newImporter = new IsosImporter(fileName, Globals.MSFileType.Finnigan);
                        importedData = newImporter.Import();
                        
                        foreach (IsosResult isos in importedData)
                        {
                            FeatureLight newFeature = new FeatureLight();
                            //newFeature.Abundance = Convert.ToInt32(isos.IsotopicProfile.IntensityAggregate);
                            newFeature.Abundance = Convert.ToInt32(isos.IsotopicProfile.IntensityAggregateAdjusted);
                            newFeature.ChargeState = isos.IsotopicProfile.ChargeState;
                            newFeature.DriftTime = 0;
                            newFeature.ID = IDcounter;
                            newFeature.MassMonoisotopic = isos.IsotopicProfile.MonoIsotopicMass;
                            newFeature.NET = Convert.ToDouble(isos.ScanSet.PrimaryScanNumber);
                            newFeature.Score = Convert.ToSingle(isos.InterferenceScore);
                            OmicsFriendlyFeatureList.Add(newFeature);

                            ScanSet.Add(isos.ScanSet.PrimaryScanNumber);

                            IDcounter++;
                        }
                    }
                    break;
                default:
                    {
                        Console.WriteLine("Select an instrument type in ISOS Importer");
                        Console.ReadKey();
                    }
                    break;
            }
        }      
    }
}
