using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace IQGlyQ.UnitTesting
{
    public class GlycanCodeConversion_UnitTest
    {
        [Test]
        public void Convert()
        {
            string code = "104205";
            List<int> composition = Utiliites.ConvertStringGlycanCodeToIntegers54000(code);

            Assert.AreEqual(composition.Count,4);

            Assert.AreEqual(composition[0], 10);
            Assert.AreEqual(composition[1], 4);
            Assert.AreEqual(composition[2], 2);
            Assert.AreEqual(composition[3], 5);
        }

        [Test]
        public void ConvertDash()
        {
            string code = "10-2-1-0-4";
            List<int> composition = Utiliites.ConvertStringGlycanCodeToIntegers(code);

            Assert.AreEqual(composition.Count, 5);

            Assert.AreEqual(composition[0], 10);
            Assert.AreEqual(composition[1], 2);
            Assert.AreEqual(composition[2], 1);
            Assert.AreEqual(composition[3], 0);
            Assert.AreEqual(composition[4], 4);
        }
    }
}
