using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
	class Averagine
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


		public Averagine AveragineSetup(string averagineType)
		{
			Averagine averagineCurrent = new Averagine();
			setAveragineFX(averagineType, averagineCurrent);

			return averagineCurrent;
		}

		private void setAveragineFX(string averagineType, Averagine AveragineIN)
		{
			//set up constants
            Averagine AveragineCurrent = new Averagine();
			
            switch(averagineType)
            {
                case "peptide":
                    AveragineIN.isoC = 4.9384;
                    AveragineIN.isoH = 7.7583;
                    AveragineIN.isoO = 1.4773;
                    AveragineIN.isoN = 1.3577;
                    AveragineIN.isoS = 0.0417;
                    break;
                case "glycan"://HNFS
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
			
			
			AveragineIN.NaturalC13=0.0107;
			AveragineIN.NaturalH2=0.000115;
			AveragineIN.NaturalN15=0.00368;
			AveragineIN.NaturalO17 = 0.00038;
			AveragineIN.NaturalO18=0.00205;
			AveragineIN.NaturalS33=0.0076;
			AveragineIN.NaturalS34=0.0429;
			AveragineIN.NaturalS36=0.0002;

			AveragineIN.MassNeutron = 1.00866491597m;
			AveragineIN.MassProton = 1.00727646677m;
			AveragineIN.MassElectron = 0.0005485799094300001m;

			decimal massC = 12m;
			decimal massH = 1.0078250321m;
			decimal massO = 15.9949146221m;
			decimal massN = 14.0030740052m;
			decimal massS = 31.97207069m;

            //Mass of the averagine unit. Like an average amino acid or monosacharide
			AveragineIN.isoUnit = 0;
			AveragineIN.isoUnit += (decimal)AveragineIN.isoC * massC;
			AveragineIN.isoUnit += (decimal)AveragineIN.isoH * massH;
			AveragineIN.isoUnit += (decimal)AveragineIN.isoO * massO;
			AveragineIN.isoUnit += (decimal)AveragineIN.isoN * massN;
			AveragineIN.isoUnit += (decimal)AveragineIN.isoS * massS;
			
		}
	}
}
