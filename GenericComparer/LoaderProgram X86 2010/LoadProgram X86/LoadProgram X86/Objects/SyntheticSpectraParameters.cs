using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
	public class SyntheticSpectraParameters
	{
		public decimal PeakSpacing { get; set; }
		public decimal StartMass { get; set; }
		public decimal EndMass { get; set; }
        public double Resolution { get; set; }
        public double PercentHanning { get; set; }

		public SyntheticSpectraParameters()
		{
		}
	}
}
