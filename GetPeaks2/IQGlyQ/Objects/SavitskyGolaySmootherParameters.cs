using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQGlyQ.Objects
{
    public class SavitskyGolaySmootherParameters
    {
        /// <summary>
        /// keep values positive
        /// </summary>
        public bool AllowNegativeValues { get; set; }

        /// <summary>
        /// how many points to smooth over
        /// </summary>
        public int PointsForSmoothing { get; set; }

        /// <summary>
        /// order of polynomial used for interpolation
        /// </summary>
        public int PolynomialOrder { get; set; }

        public SavitskyGolaySmootherParameters()
        {
            AllowNegativeValues = false;
            PointsForSmoothing = 9;
            PolynomialOrder = 2;
        }
    }
}
