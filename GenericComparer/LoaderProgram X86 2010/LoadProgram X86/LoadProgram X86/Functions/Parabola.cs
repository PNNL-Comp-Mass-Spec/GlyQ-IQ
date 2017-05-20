using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
	class Parabola
	{
		public void ParabolaFX(PeakTop peakTopPoints, PrintBuffer print)
		{
			int numberOfPoints = peakTopPoints.ionMZ.Count;


			//peakTopPoints.ionMZ[0] = 5;
			//peakTopPoints.ionMZ[1] = 6;
			//peakTopPoints.ionMZ[2] = 7;

			//peakTopPoints.ionIntensity[0] = 5;
			//peakTopPoints.ionIntensity[1] = 6;
			//peakTopPoints.ionIntensity[2] = 4;

			//answer is A=-1.5, B=2.5, C=5

			double T1 = 0, T2 = 0, T3 = 0;
			double X1 = 0, X2 = 0, X3 = 0;
			double Y1 = 0, Y2 = 0, Y3 = 0;
			double Z1 = 0, Z2 = 0, Z3 = 0;
			double Theta1, Theta2, Theta3, ThetaDenominator;
			double A = 0, B = 0, C = 0;
			double MaxInt, X = 0;
            //double apexMass;
            int i;

			double InitialX;  //used to offset the parabola to zero
			InitialX = peakTopPoints.ionMZ[0];

			for (i = 0; i < numberOfPoints; i += 1)
			{
				peakTopPoints.ionMZ[i] = peakTopPoints.ionMZ[i] - InitialX;
			}

			for (i = 0; i < numberOfPoints; i += 1)
			{

				//T1+=Top_MZ[i]^2*Top_Int[i]
				T1 += Math.Pow(peakTopPoints.ionMZ[i], 2) * peakTopPoints.ionIntensity[i];
			}

			T1 = 2 * T1;

			for (i = 0; i < numberOfPoints; i += 1)
			{
				//T2+=Top_MZ[i]*Top_Int[i]
				T2 += peakTopPoints.ionMZ[i] * peakTopPoints.ionIntensity[i];
			}

			T2 = 2 * T2;

			for (i = 0; i < numberOfPoints; i += 1)
			{
				//T3+=Top_Int[i]
				T3 += peakTopPoints.ionIntensity[i];
			}

			T3 = 2 * T3;

			for (i = 0; i < numberOfPoints; i += 1)
			{
				//X1+=Top_MZ[i]^4
				X1 += Math.Pow(peakTopPoints.ionMZ[i], 4);
			}
			X1 = 2 * X1;

			for (i = 0; i < numberOfPoints; i += 1)
			{
				//X2+=Top_MZ[i]^3
				X2 += Math.Pow(peakTopPoints.ionMZ[i], 3);
			}

			X2 = 2 * X2;

			for (i = 0; i < numberOfPoints; i += 1)
			{
				//X3+=Top_MZ[i]^2
				X3 += Math.Pow(peakTopPoints.ionMZ[i], 2);
			}

			X3 = 2 * X3;
			Y1 = X2;
			Y2 = X3;

			for (i = 0; i < numberOfPoints; i += 1)
			{
				Y3 += peakTopPoints.ionMZ[i];
			}

			Y3 = 2 * Y3;

			Z1 = X3;
			Z2 = Y3;
			Z3 = 2 * numberOfPoints;

			ThetaDenominator = X1 * Y2 * Z3 - X1 * Z2 * Y3 - Y1 * X2 * Z3 + Z1 * X2 * Y3 + Y1 * X3 * Z2 - Z1 * X3 * Y2;

			Theta1 = 0;
			Theta2 = 0;
			Theta3 = 0;
			Theta1 = (Y2 * Z3 - Z2 * Y3) * T1 / ThetaDenominator;
			Theta2 = -(Y1 * Z3 - Z1 * Y3) * T2 / ThetaDenominator;
			Theta3 = (Y1 * Z2 - Z1 * Y2) * T3 / ThetaDenominator;

			A = Theta1 + Theta2 + Theta3;

			Theta1 = 0;
			Theta2 = 0;
			Theta3 = 0;
			Theta1 = -(X2 * Z3 - Z2 * X3) * T1 / ThetaDenominator;
			Theta2 = (X1 * Z3 - Z1 * X3) * T2 / ThetaDenominator;
			Theta3 = -(X1 * Z2 - Z1 * X2) * T3 / ThetaDenominator;

			//print theta1, theta2, theta3
			B = Theta1 + Theta2 + Theta3;

			Theta1 = 0;
			Theta2 = 0;
			Theta3 = 0;
			Theta1 = (X2 * Y3 - Y2 * X3) * T1 / ThetaDenominator;
			Theta2 = -(X1 * Y3 - Y1 * X3) * T2 / ThetaDenominator;
			Theta3 = (X1 * Y2 - Y1 * X2) * T3 / ThetaDenominator;
			C = Theta1 + Theta2 + Theta3;

            //PolyCoefficients QuadraticSet = new PolyCoefficients();
            //QuadraticSet.QuadraticCoefficients(A, B, C);

			//print.AddLine("A=" + A + "B=" + B + "C=" + C);

			//X=-0.5*B/A;
			//apexMass = X + InitialX;
			//X = (double)peakTopPoints.apexCenterMZ; 

			X = (double)(peakTopPoints.apexCenterMZ-peakTopPoints.peakStartMZ);
			MaxInt = A * X * X + B * X + C;

			peakTopPoints.apexIntensity = (decimal)MaxInt;
		}
	}
}
