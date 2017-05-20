using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data;

namespace ConsoleApplication1
{
	class GlycoPeptideGlycanResidules
	{
		public static void FindGlycanResidules(int siteIndex, List<XYData> glycoPeptideData, peptidePosibilitiesPerSite peptideStorage, GlycoPeptideParameters glycoPeptideParameter)
		{
			//For each experimental mass, find peptide parts that are less than it and the subtract it to get glycan residules
			//this is needed for each mass

			int i, j;
			List<double> GlycanResiduals = new List<double>();//~glycopeptide mass-peptide mass
			List<string> KeepersPeptideText = new List<string>();
			List<double> KeepersData = new List<double>();
            List<int> KeepersIndex = new List<int>();
			List<double> keepersPeptideMass = new List<double>();

			List<double> PeptidePartMass = peptideStorage.PeptideMassList[siteIndex];
			List<string> PeptidePartText = peptideStorage.PeptideSitesList[siteIndex];
			List<double> add = new List<double>();
            for (i = 0; i < glycoPeptideData.Count; i += 1)  //for each data mass, try each peptide part
			{
				for (j = 0; j < PeptidePartMass.Count; j++)
                {
                    double temp = glycoPeptideData[i].X - PeptidePartMass[j] - (glycoPeptideParameter.CoreMass * glycoPeptideParameter.CoreToggle);
                    if (glycoPeptideData[i].X - PeptidePartMass[j]-(glycoPeptideParameter.CoreMass*glycoPeptideParameter.CoreToggle) >= 0)
                    {
                        GlycanResiduals.Add(glycoPeptideData[i].X - PeptidePartMass[j]);
                        KeepersData.Add(glycoPeptideData[i].X);
                        KeepersIndex.Add(i);
                        keepersPeptideMass.Add(PeptidePartMass[j]);
                        KeepersPeptideText.Add(PeptidePartText[j]);
                        Console.WriteLine("--accept this part", PeptidePartText[j], PeptidePartMass[j]);
                    }
                }
			}

			peptideStorage.glycanResidulesMass.Add(GlycanResiduals);
			peptideStorage.glycanResidulesPeptideMass.Add(keepersPeptideMass);
			peptideStorage.glycanResidulesPeptides.Add(KeepersPeptideText);
            peptideStorage.glycanResidulesGlycoPeptideIndex.Add(KeepersIndex);
            peptideStorage.glycanResidulesGlycoPeptideMass.Add(KeepersData);
		}
	}
}
