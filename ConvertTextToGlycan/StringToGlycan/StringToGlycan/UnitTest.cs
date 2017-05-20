using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlycanTextToMass;
using NUnit.Framework;

namespace StringToGlycan
{
    public class UnitTest
    {
        [Test]
        public void convertstringToMZ()
        {
            string testGlycan = "5411";
            int charge = 2;

            double massToCharge;
            GlycanTextToMass.Convert.StringToMassFX(testGlycan, charge, out massToCharge);

            Assert.AreEqual(massToCharge, 1040.88785015808d);
            Console.Write("The m/z is " + massToCharge);
        }

        

    }
}
