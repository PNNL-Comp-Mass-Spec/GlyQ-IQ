using System;
using System.Collections.Generic;
using System.Linq;
using IQGlyQ.Enumerations;
using IQGlyQ.Functions;
using IQGlyQ.Objects;
using PNNLOmics.Algorithms.Solvers.LevenburgMarquadt;
using PNNLOmics.Data;

namespace HammerPeakDetector
{
    class ProbabilityDistribution
    {
        /// <summary>
        /// Fits a gaussian or lorentzian curve shape to the xydata
        /// </summary>
        /// <param name="histogram"></param>
        /// <param name="coefficents">0 = sigma, 1 = height, 2 = center</param>
        /// <returns></returns>
        public List<XYData> FitDistribution(List<XYData> histogram, out double[] coefficents, out double areaUnderCurve, out List<XYData> modeledPeakList)
        {

            ScanObject scanRangeFit;

            SolverReport fitMetrics;
            Tuple<string, string> errorLog = new Tuple<string, string>("", "");


            int numberOfSamples = 100;
            double sampleWidth = 0.001;
            bool centerXAxisOnInteger = false;

            //these need to be learned from the data
            coefficents = new double[3];
            //Guess for mass tolerance
            coefficents[0] = 0.02;
            coefficents[2] = 1.00325;
            List<XYData> orderedData = histogram.OrderByDescending(r => r.Y).ToList();

            coefficents[1] = orderedData[0].Y;//guess for height
            coefficents[2] = orderedData[0].X;//guess for center

            List<XYData> clippedFitXyData = CurveFit.Fit_LM(histogram, out fitMetrics, out areaUnderCurve, ref coefficents, out modeledPeakList, numberOfSamples, sampleWidth, ref errorLog, out scanRangeFit, EnumerationCurveType.Gaussian, centerXAxisOnInteger);

            //make all sigma positive
            if (coefficents[0] < 0)
            {
                coefficents[0] *= -1;
            }

            int ScanBoundsInfoStart = scanRangeFit.Start;
            int ScanBoundsInfoStop = scanRangeFit.Stop;
            double[] CorrelationCoefficients = coefficents;
            double LMOptimizationCorrelationRsquared = 0;
            if (fitMetrics != null)
            {
                LMOptimizationCorrelationRsquared = fitMetrics.RSquared;
            }


            return clippedFitXyData;
        }
    }
}
