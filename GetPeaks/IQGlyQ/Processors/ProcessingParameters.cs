using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQGlyQ.Objects;
using PNNLOmics.Algorithms.PeakDetection;

namespace IQGlyQ.Processors
{
    public abstract class ProcessingParameters
    {
        /// <summary>
        /// calibrate peaks for chromatograms
        /// </summary>
        public double CalibrationDaltonOffset { get; set; }

        /// <summary>
        /// should we calibrate?
        /// </summary>
        public bool CalibrateData { get; set; }

        /// <summary>
        /// parameters for Savitsky Golay smoother.  works for omics or decon
        /// </summary>
        public SavitskyGolaySmootherParameters ParametersSavitskyGolay { get; set; }

        /// <summary>
        /// peak detection parameters for omics peak thresholding
        /// </summary>
        public PeakThresholderParameters ParametersOmicsThreshold { get; set; }

        /// <summary>
        /// peak detection paraemters
        /// </summary>
        public PeakCentroiderParameters ParametersOmicsPeakCentroid { get; set; }

        /// <summary>
        /// minimum rsquared cuttoff for LM curve fitting of peaks.  this can be different For LC and MS and IMS
        /// </summary>
        public double LM_RsquaredCuttoff { get; set; }

        /// <summary>
        /// how many points we will allow on each side of the apex.  this should be increased by a linear function F(number of smooth points)
        /// for a 3 point minimum, full points across peak minimum =  smooth pionts * slope + intercept where slop = 1.00 for 3 points and 0.90 for 5 pionts.  the intercept is 3 or 5
        /// so it should be divided by 2 since we are working in forms of shoulder rather than peaks accross PointsPerShoulder= 0.90* smoothPoints /2
        /// adding 1 bonus point moves the threshold up above the theoretical data rather than going through the middle where half pass and half fail.  If we want all 3 points peak equilivalent to fail, we should use bounus=1
        /// </summary>
        public double PointsPerShoulderSlope { get; set; }
        public double PointsPerShoulderIntercept { get; set; }
        public double PointsPerShoulderBonusPoint { get; set; }
        public int PointsPerShoulder { get; set; }

        public string XYDataWriterPath { get; set; }





        public int CalculatePointsPerShoulderAsAFunctionOfSgPoints(FragmentedTargetedWorkflowParametersIQ fragmentedTargetedWorkflowParameters)
        {
            double bonus = fragmentedTargetedWorkflowParameters.LCParameters.PointsPerShoulderBonusPoint;
            double slope = fragmentedTargetedWorkflowParameters.LCParameters.PointsPerShoulderSlope;
            double intercept = fragmentedTargetedWorkflowParameters.LCParameters.PointsPerShoulderIntercept;
            double points = (slope * fragmentedTargetedWorkflowParameters.ChromSmootherNumPointsInSmooth + intercept + bonus) / 2;//2 is since we are splitting peaks and looking at the minimum number of points in the shoulder.
            return Convert.ToInt32(Math.Truncate(points) + 1); //+1 for round up
        }

    }
}
