using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class PolyCoefficients
    {
        public double A { get; set; }
		public double B { get; set; }
        public double C { get; set; }
        public double A0{ get; set; }
        public double ReferenceMass { get; set; }

		public void QuadraticCoefficients(double a, double b, double c, double referenceMass)
		{
			this.A = a;
			this.B = b;
            this.C = c;
            this.ReferenceMass = referenceMass;
		}

        public void SplineCoefficients(double a, double b, double c, double a0, double referenceMass)
        {
            this.A = a;
            this.B = b;
            this.C = c;
            this.A0 = a0;
            this.ReferenceMass = referenceMass;
        }
    }
}
