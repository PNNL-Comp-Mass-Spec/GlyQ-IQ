using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GetPeaks_DLL.ParameterWriters;

namespace GetPeaks.UnitTests
{
    class ParameterFileTests
    {

        /// <summary>
        /// write a parameter file for getPeaks
        /// </summary>
        [Test]
        public void WriteGetPeaksFile()
        {
            GetPeaksParameterController newController = new GetPeaksParameterController();
            newController.GenerateFiles();

        }
    }
}
