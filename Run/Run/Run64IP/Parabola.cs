using System.Collections.Generic;
using InformedProteomics.Backend.Data.Spectrometry;

namespace Run64IP
{
    public static class Parabola
    {
        /// <summary>
        /// Find the coefficeints to the parabola that goes through the data points
        /// </summary>
        /// <param name="peakTopList">A list of PNNL Omics XYData</param>
        /// <returns>XYData point correspiding to the pair at the apex intensity and center of mass </returns>
        public static void ParabolaABC(List<Peak> peakSideList, ref double aOut, ref double bOut, ref double cOut)//aX^2+bX+c
        {
            #region copied code
            double apexMass;
            double apexIntensity;

            int numberOfPoints = peakSideList.Count;

            //switch x with Y


            //for (int i = 0; i < numberOfPoints; i++)
            //{
            //    XYData tempPoint = new XYData();
            //    tempPoint.Intensity = peakSideList[i].Mz;
            //    tempPoint.Mz = peakSideList[i].Intensity;
            //    peakSideList[i].Mz = tempPoint.Mz;
            //    peakSideList[i].Intensity = tempPoint.Intensity;
            //}

            //print "run Parabola"
            double InitialX;  //used to offset the parabola to zero
            InitialX = peakSideList[0].Mz;
            for (int i = 0; i < numberOfPoints; i++)
            {
                peakSideList[i].Mz = (float)(peakSideList[i].Mz - InitialX);
            }

            double T1 = 0, T2 = 0, T3 = 0;
            double X1 = 0, X2 = 0, X3 = 0;
            double Y1 = 0, Y2 = 0, Y3 = 0;
            double Z1 = 0, Z2 = 0, Z3 = 0;
            double Theta1, Theta2, Theta3, ThetaDenominator;
            double A = 0, B = 0, C = 0;
            double X = 0;

            //TODO unit test
            //peakTopList[0].Mz=1;
            //peakTopList[1].Mz=2;
            //peakTopList[2].Mz=3;
            //peakTopList[0].Intensity=2;
            //peakTopList[1].Intensity=5;
            //peakTopList[2].Intensity = 4;

            for (int i = 0; i < numberOfPoints; i += 1)
            {
                T1 += peakSideList[i].Mz * peakSideList[i].Mz * peakSideList[i].Intensity;
            }

            T1 = 2 * T1;

            for (int i = 0; i < numberOfPoints; i += 1)
            {
                T2 += peakSideList[i].Mz * peakSideList[i].Intensity;
            }

            T2 = 2 * T2;

            for (int i = 0; i < numberOfPoints; i += 1)
            {
                T3 += peakSideList[i].Intensity;
            }

            T3 = 2 * T3;

            for (int i = 0; i < numberOfPoints; i += 1)
            {
                X1 += peakSideList[i].Mz * peakSideList[i].Mz * peakSideList[i].Mz * peakSideList[i].Mz;
            }

            X1 = 2 * X1;

            for (int i = 0; i < numberOfPoints; i += 1)
            {
                X2 += peakSideList[i].Mz * peakSideList[i].Mz * peakSideList[i].Mz;
            }

            X2 = 2 * X2;

            for (int i = 0; i < numberOfPoints; i += 1)
            {
                X3 += peakSideList[i].Mz * peakSideList[i].Mz;
            }

            X3 = 2 * X3;

            Y1 = X2;
            Y2 = X3;

            for (int i = 0; i < numberOfPoints; i += 1)
            {
                Y3 += peakSideList[i].Mz;
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

            X = -0.5 * B / A;

            apexMass = X + InitialX;
            apexIntensity = A * X * X + B * X + C;

            #endregion

            aOut = A;
            bOut = B;
            cOut = C;
            //TODO unit test
            //T1=116.0000
            //T2=48.0000
            //T3=22.0000
            //X1=196.0000
            //X2=72.0000
            //X3=28.0000
            //Y1=72.0000
            //Y2=28.0000
            //Y3=12.0000
            //Z1=28.0000
            //Z2=12.0000
            //Z3=6.0000
            //A=-2
            //B=9
            //C=-5
            //apexIntensity = 5.125
        }
    }
}
