using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Functions;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using MathNet.Numerics;

namespace GetPeaks.UnitTests
{
    class ErfToSigma
    {
        [Test]
        public void PtoSigma()
        {
           

            double sigma = 3;

            double erfArea = SpecialFunctions.Erf(sigma/ Math.Sqrt(2));

            Assert.AreEqual(erfArea, 0.99730020393673979d);

            sigma = 2;

            erfArea = SpecialFunctions.Erf(sigma / Math.Sqrt(2));

            Assert.AreEqual(erfArea, 0.95449973610364158d);
            
            sigma = 1;

            erfArea = SpecialFunctions.Erf(sigma / Math.Sqrt(2));

            Assert.AreEqual(erfArea, 0.68268949213708585d);

            double areaInside = 0.68268949213708585d;

            double calculatedSigma = SpecialFunctions.ErfInv(areaInside) * Math.Sqrt(2);

            Assert.AreEqual(calculatedSigma, 1 );

            double pValue = 0.05;

            calculatedSigma = SpecialFunctions.ErfInv(1-pValue) * Math.Sqrt(2);

            Assert.AreEqual(calculatedSigma, 1.9599639845400538d);

            pValue = 0.01;

            calculatedSigma = SpecialFunctions.ErfInv(1 - pValue) * Math.Sqrt(2);

            Assert.AreEqual(calculatedSigma, 2.5758293035489008d);


            List<double> pValuePile = new List<double>();
            pValuePile.Add(0.005);
            pValuePile.Add(0.01);
            pValuePile.Add(0.05);
            pValuePile.Add(0.1);
            pValuePile.Add(0.15);

            List<double> sigmaPile = new List<double>();
            sigmaPile.Add(1);
            sigmaPile.Add(2);
            sigmaPile.Add(2.5);
            sigmaPile.Add(3);
            sigmaPile.Add(4);

            foreach (var sigmaTest in sigmaPile)
            {
                Console.WriteLine("At a sigma of " + sigmaTest + ", we have a p-value of " + ConvertSigmaAndPValue.SigmaToPvalue(sigmaTest));
            }
            Console.WriteLine(Environment.NewLine);
            foreach (var pValueTest in pValuePile)
            {
                Console.WriteLine("At a p-Value of " + pValueTest + ", we have a sigma of " + ConvertSigmaAndPValue.PvalueToSigma(pValueTest));
            }


        }
    }
}
