using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Constants;
using PNNLOmics.Data.Constants.Enumerations;
using Synthetic_Spectra_DLL_X64_Net_4.Launch;

namespace Synthetic_Spectra_DLL_X64_Net_4.Framework
{
    public class Averagine
    {
        public double isoC { get; set; }
		public double isoH { get; set; }
		public double isoO { get; set; }
		public double isoN { get; set; }
		public double isoS { get; set; }
        public decimal isoUnit { get; set; }

		public double NaturalC13 { get; set; }
		public double NaturalH2 { get; set; }
		public double NaturalN15 { get; set; }
		public double NaturalO17 { get; set; }
		public double NaturalO18 { get; set; }
		public double NaturalS33 { get; set; }
		public double NaturalS34 { get; set; }
		public double NaturalS36 { get; set; }
		public decimal MassNeutron { get; set; }
        public decimal MassProton { get; set; }
        public decimal MassElectron { get; set; }


		public Averagine AveragineSetup(AveragineType averagineType)
		{
			Averagine averagineCurrent = new Averagine();
			setAveragineFX(averagineType, averagineCurrent);

			return averagineCurrent;
		}

        private void setAveragineFX(AveragineType averagineType, Averagine AveragineIN)
		{
			//set up constants
            Averagine AveragineCurrent = new Averagine();
			
            switch(averagineType)
            {
                case AveragineType.Peptide:
                    AveragineIN.isoC = 4.9384;
                    AveragineIN.isoH = 7.7583;
                    AveragineIN.isoO = 1.4773;
                    AveragineIN.isoN = 1.3577;
                    AveragineIN.isoS = 0.0417;
                    break;
                case AveragineType.Glycan://HNFS
                    AveragineIN.isoC = 31;
                    AveragineIN.isoH = 50;
                    AveragineIN.isoO = 22;
                    AveragineIN.isoN = 2;
                    AveragineIN.isoS = 0;
                    break;
                default:
                    AveragineIN.isoC = 4.9384;
                    AveragineIN.isoH = 7.7583;
                    AveragineIN.isoO = 1.4773;
                    AveragineIN.isoN = 1.3577;
                    AveragineIN.isoS = 0.0417;
                    break;
            }


            AveragineIN.NaturalC13 = Constants.Elements[ElementName.Carbon].IsotopeDictionary["C13"].NaturalAbundance;// 0.0107;
            AveragineIN.NaturalH2 = Constants.Elements[ElementName.Hydrogen].IsotopeDictionary["H2"].NaturalAbundance;// 0.000115;
            AveragineIN.NaturalN15 = Constants.Elements[ElementName.Nitrogen].IsotopeDictionary["N15"].NaturalAbundance;//0.00368;
            AveragineIN.NaturalO17 = Constants.Elements[ElementName.Oxygen].IsotopeDictionary["O17"].NaturalAbundance;//0.00038;
            AveragineIN.NaturalO18 = Constants.Elements[ElementName.Oxygen].IsotopeDictionary["O18"].NaturalAbundance;//0.00205;
            AveragineIN.NaturalS33 = Constants.Elements[ElementName.Sulfur].IsotopeDictionary["S33"].NaturalAbundance;//0.0076;
            AveragineIN.NaturalS34 = Constants.Elements[ElementName.Sulfur].IsotopeDictionary["S34"].NaturalAbundance;//0.0429;
            AveragineIN.NaturalS36 = Constants.Elements[ElementName.Sulfur].IsotopeDictionary["S36"].NaturalAbundance;//0.0002;

            AveragineIN.MassNeutron = (decimal)Constants.SubAtomicParticles[SubAtomicParticleName.Neutron].MassMonoIsotopic;// 1.00866491597m;
            AveragineIN.MassProton = (decimal)Constants.SubAtomicParticles[SubAtomicParticleName.Proton].MassMonoIsotopic;//1.00727646677m;
            AveragineIN.MassElectron = (decimal)Constants.SubAtomicParticles[SubAtomicParticleName.Electron].MassMonoIsotopic;//0.0005485799094300001m;

            decimal massC = (decimal)Constants.Elements[ElementName.Carbon].MassMonoIsotopic;// 12m;
            decimal massH = (decimal)Constants.Elements[ElementName.Hydrogen].MassMonoIsotopic;// 1.0078250321m;
            decimal massO = (decimal)Constants.Elements[ElementName.Oxygen].MassMonoIsotopic;// 15.9949146221m;
            decimal massN = (decimal)Constants.Elements[ElementName.Nitrogen].MassMonoIsotopic;// 14.0030740052m;
            decimal massS = (decimal)Constants.Elements[ElementName.Sulfur].MassMonoIsotopic;// 31.97207069m;

            //Mass of the averagine unit. Like an average amino acid or monosacharide
			AveragineIN.isoUnit = 0;//initialize
            AveragineIN.isoUnit += (decimal)AveragineIN.isoC * massC;
            AveragineIN.isoUnit += (decimal)AveragineIN.isoH * massH;
            AveragineIN.isoUnit += (decimal)AveragineIN.isoO * massO;
            AveragineIN.isoUnit += (decimal)AveragineIN.isoN * massN;
            AveragineIN.isoUnit += (decimal)AveragineIN.isoS * massS;
			
		}
	}

    public enum AveragineType
    {
        Peptide,
        Glycan
    }
}
