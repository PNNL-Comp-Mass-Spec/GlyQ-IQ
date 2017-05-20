using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Data.Constants;

namespace GetPeaks_DLL.Isotopes
{
    public class Averagine
    {
        public double isoC { get; set; }
		public double isoH { get; set; }
		public double isoO { get; set; }
		public double isoN { get; set; }
		public double isoS { get; set; }
        public double isoUnit { get; set; }

		public double NaturalC13 { get; set; }
		public double NaturalH2 { get; set; }
		public double NaturalN15 { get; set; }
		public double NaturalO17 { get; set; }
		public double NaturalO18 { get; set; }
		public double NaturalS33 { get; set; }
		public double NaturalS34 { get; set; }
		public double NaturalS36 { get; set; }
		public double MassNeutron { get; set; }
        public double MassProton { get; set; }
        public double MassElectron { get; set; }

		public Averagine AveragineSetup(AveragineType averagineType)
		{
			Averagine averagineCurrent = new Averagine();
            switch(averagineType)
            {
                case AveragineType.Glycan:
                    {
                        setAveragineFX(averagineType, averagineCurrent);
                    }
                    break;
                case AveragineType.Peptide:
                    {
                        setAveragineFX(averagineType, averagineCurrent);
                    }
                    break;
                case AveragineType.Custom:
                    {
                        Averagine AveragineIN = new Averagine();
                        AveragineIN.isoC = isoC;
                        AveragineIN.isoH = isoH;
                        AveragineIN.isoO = isoO;
                        AveragineIN.isoN = isoN;
                        AveragineIN.isoS = isoS;
                        setAveragineFXCustom(AveragineIN);
                        averagineCurrent = AveragineIN;
                    }
                    break;
                default:
                    {
                        setAveragineFX(averagineType, averagineCurrent);
                    }
                    break;
            }
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

            AveragineIN.MassNeutron = Constants.SubAtomicParticles[SubAtomicParticleName.Neutron].MassMonoIsotopic;// 1.00866491597m;
            AveragineIN.MassProton = Constants.SubAtomicParticles[SubAtomicParticleName.Proton].MassMonoIsotopic;//1.00727646677m;
            AveragineIN.MassElectron = Constants.SubAtomicParticles[SubAtomicParticleName.Electron].MassMonoIsotopic;//0.0005485799094300001m;

            double massC = Constants.Elements[ElementName.Carbon].MassMonoIsotopic;// 12m;
            double massH = Constants.Elements[ElementName.Hydrogen].MassMonoIsotopic;// 1.0078250321m;
            double massO = Constants.Elements[ElementName.Oxygen].MassMonoIsotopic;// 15.9949146221m;
            double massN = Constants.Elements[ElementName.Nitrogen].MassMonoIsotopic;// 14.0030740052m;
            double massS = Constants.Elements[ElementName.Sulfur].MassMonoIsotopic;// 31.97207069m;

            //Mass of the averagine unit. Like an average amino acid or monosacharide
			AveragineIN.isoUnit = 0.0;//initialize
            AveragineIN.isoUnit += AveragineIN.isoC * massC;
            AveragineIN.isoUnit += AveragineIN.isoH * massH;
            AveragineIN.isoUnit += AveragineIN.isoO * massO;
            AveragineIN.isoUnit += AveragineIN.isoN * massN;
            AveragineIN.isoUnit += AveragineIN.isoS * massS;
			
		}

        private void setAveragineFXCustom(Averagine AveragineIN)
        {
            //set up constants
            AveragineIN.NaturalC13 = Constants.Elements[ElementName.Carbon].IsotopeDictionary["C13"].NaturalAbundance;// 0.0107;
            AveragineIN.NaturalH2 = Constants.Elements[ElementName.Hydrogen].IsotopeDictionary["H2"].NaturalAbundance;// 0.000115;
            AveragineIN.NaturalN15 = Constants.Elements[ElementName.Nitrogen].IsotopeDictionary["N15"].NaturalAbundance;//0.00368;
            AveragineIN.NaturalO17 = Constants.Elements[ElementName.Oxygen].IsotopeDictionary["O17"].NaturalAbundance;//0.00038;
            AveragineIN.NaturalO18 = Constants.Elements[ElementName.Oxygen].IsotopeDictionary["O18"].NaturalAbundance;//0.00205;
            AveragineIN.NaturalS33 = Constants.Elements[ElementName.Sulfur].IsotopeDictionary["S33"].NaturalAbundance;//0.0076;
            AveragineIN.NaturalS34 = Constants.Elements[ElementName.Sulfur].IsotopeDictionary["S34"].NaturalAbundance;//0.0429;
            AveragineIN.NaturalS36 = Constants.Elements[ElementName.Sulfur].IsotopeDictionary["S36"].NaturalAbundance;//0.0002;

            AveragineIN.MassNeutron = Constants.SubAtomicParticles[SubAtomicParticleName.Neutron].MassMonoIsotopic;// 1.00866491597m;
            AveragineIN.MassProton = Constants.SubAtomicParticles[SubAtomicParticleName.Proton].MassMonoIsotopic;//1.00727646677m;
            AveragineIN.MassElectron = Constants.SubAtomicParticles[SubAtomicParticleName.Electron].MassMonoIsotopic;//0.0005485799094300001m;

            double massC = Constants.Elements[ElementName.Carbon].MassMonoIsotopic;// 12m;
            double massH = Constants.Elements[ElementName.Hydrogen].MassMonoIsotopic;// 1.0078250321m;
            double massO = Constants.Elements[ElementName.Oxygen].MassMonoIsotopic;// 15.9949146221m;
            double massN = Constants.Elements[ElementName.Nitrogen].MassMonoIsotopic;// 14.0030740052m;
            double massS = Constants.Elements[ElementName.Sulfur].MassMonoIsotopic;// 31.97207069m;

            //Mass of the averagine unit. Like an average amino acid or monosacharide
            AveragineIN.isoUnit = 0.0;//initialize
            AveragineIN.isoUnit += AveragineIN.isoC * massC;
            AveragineIN.isoUnit += AveragineIN.isoH * massH;
            AveragineIN.isoUnit += AveragineIN.isoO * massO;
            AveragineIN.isoUnit += AveragineIN.isoN * massN;
            AveragineIN.isoUnit += AveragineIN.isoS * massS;

        }
	}

    public enum AveragineType
    {
        Peptide,
        Glycan,
        Custom
    }
}
