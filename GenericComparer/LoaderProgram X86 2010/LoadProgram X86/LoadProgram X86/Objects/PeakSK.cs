using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    public class PeakDecimal
	{
		public decimal Mass { get; set; }
		public decimal Intensity { get; set; }

		public PeakDecimal(decimal mass, decimal intensity)
		{
			this.Mass = mass;
			this.Intensity = intensity;
		}

        public PeakDecimal()
        { }

	}

	public class PeakTop
	{
		public decimal apexCenterMZ { get; set; }//calculated inside
		public decimal apexIntensity { get; set; }//calculated inside
		public decimal peakSpacing { get; set; }//input
		public decimal peakStartMZ { get; set; }//input
		public int peakIndex { get; set; }//input

		//List<double> m_ionMZ { get; set; }
		//List<double> m_ionIntensity { get; set; }

		public List<double> ionMZ = new List<double>();
		public List<double> ionIntensity = new List<double>();

		public void Add_MZ_Intensity(double MZ, double Intensity)
		{
			ionMZ.Add(MZ);
			ionIntensity.Add(Intensity);
		}
		
	}

}
