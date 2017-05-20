using System;
using System.Collections.Generic;
using IQGlyQ.Enumerations;
using IQGlyQ.Results;
using IQ_X64.Workflows.Core;
using IQ_X64.Workflows.WorkFlowParameters;
using Run64.Backend.Core;
using XYData = PNNLOmics.Data.XYData;
using IQGlyQ.Objects;

namespace IQGlyQ.IQTasks
{
    public class IqDeuteratedQuantifierTask:IqTask
    {
        ///// <summary>
        ///// EIC generation from peak data
        ///// </summary>
        //protected PeakChromatogramGenerator _peakChromGen;

        ///// <summary>
        ///// smoothing data.  Key variable in peak detection
        ///// </summary>
        //protected SavitzkyGolaySmoother _smoother;

        /// <summary>
        /// smoothing data and peak picking
        /// </summary>
        protected Processors.ProcessorChromatogram _LcProcessor { get; set; }

        protected FragmentedTargetedWorkflowParametersIQ _workflowParameters { get; set; }

        protected Run IqRun;

        public IqDeuteratedQuantifierTask(WorkflowParameters parameters, Run runIn, Processors.ProcessorChromatogram lcProcessor, FragmentedTargetedWorkflowParametersIQ workflowParameters)
            : base(parameters)
        {
            WorkflowParameters = parameters;
            QuantificationParameters = (IqDeuteratedParameter)parameters;

            _workflowParameters = workflowParameters;

            MinIntensity = QuantificationParameters.MinIntensity;
            LabelingEfficiency = QuantificationParameters.IsotopeLabelingEfficiency;

            _LcProcessor = lcProcessor;
            IqRun = runIn;
        }


        #region Properties

        private IqDeuteratedParameter QuantificationParameters { get; set; }

        public override sealed WorkflowParameters WorkflowParameters { get; set; }

        public double HydrogenAbundance { get; set; }

        public double MinIntensity { get; set; }

        public double LabelingEfficiency { get; set; }

        //public int _numPointsInSmoother { get; set; }


        public double intensityExpI0 { get; set; }

        public double intensityExpI1 { get; set; }

        public double intensityExpI2 { get; set; }

        public double intensityExpI3 { get; set; }

        public double intensityExpI4 { get; set; }

        public double intensityExpI5 { get; set; }


        public double intensityTheoryI0 { get; set; }

        public double intensityTheoryI1 { get; set; }

        public double intensityTheoryI2 { get; set; }

        public double intensityTheoryI3 { get; set; }

        public double intensityTheoryI4 { get; set; }

        public double intensityTheoryI5 { get; set; }

        

        public double intensityHI0 { get; set; }

        public double intensityHI1 { get; set; }

        public double intensityHI2 { get; set; }

        public double intensityHI3 { get; set; }

        public double intensityHI4 { get; set; }

        public double intensityHI5 { get; set; }


        public double intensityDI0 { get; set; }

        public double intensityDI1 { get; set; }

        public double intensityDI2 { get; set; }

        public double intensityDI3 { get; set; }

        public double intensityDI4 { get; set; }

        

        #endregion


        public override void Execute(IqResult result)
        {
            IqGlyQResult glyResult = (IqGlyQResult) result;
            Processors.ProcessorChromatogram LcProcessor = _LcProcessor;
            double ratio = CalculateRatio(glyResult, MinIntensity, ref LcProcessor, _workflowParameters);

            
        }

        public double CalculateRatio(IqGlyQResult result, double minIntensity, ref Processors.ProcessorChromatogram LcProcessor, FragmentedTargetedWorkflowParametersIQ workflowParameters)
        {
            Tuple<string, string> errorlog = new Tuple<string, string>("Start", "Success");

            ScanObject scanStartStop = result.ToChild.ScanBoundsInfo;
            EnumerationError errorCode;
            CorrelationObject targetPeak = Utiliites.CreateCorrelationObject(result.Target.TheorIsotopicProfile, scanStartStop, IqRun, ref errorlog, out errorCode, ref LcProcessor, workflowParameters);



            double ratio = -1;

            List<double> DtoHratio = new List<double>();

            if (intensityHI0 > minIntensity && intensityDI0 > minIntensity)
            {
                ratio = intensityDI0 / intensityHI0;//perhaps sum or fit

                if (intensityDI0 > minIntensity && intensityDI1 > minIntensity && intensityDI0 > minIntensity && intensityHI1 > minIntensity)
                {
                    ratio = (intensityDI0 / intensityHI0 + intensityDI1 / intensityHI1) / 2;//average top 2
                }
            }
            else
            {
                if (intensityHI0 > minIntensity)
                {
                    ratio = 0;//since there is no D

                }
                if (intensityDI0 > minIntensity)
                {
                    ratio = 1;//since there no H

                }
                //if neither case is met, -1 will be returned
            }

            return ratio;
        }


        public void PopulateValues(IsotopicProfile iso, IsotopicProfile theoryIso)
        {
            resetIntensityValues();

            intensityExpI0 = getI0Intensity(iso);
            intensityExpI1 = getI1Intensity(iso);
            intensityExpI2 = getI2Intensity(iso);
            intensityExpI3 = getI3Intensity(iso);
            intensityExpI4 = getI4Intensity(iso);
            intensityExpI5 = getI5Intensity(iso);

            if (intensityExpI0 == 0) intensityExpI0 = double.Epsilon;
            if (intensityExpI1 == 0) intensityExpI1 = double.Epsilon;
            if (intensityExpI2 == 0) intensityExpI2 = double.Epsilon;
            if (intensityExpI3 == 0) intensityExpI3 = double.Epsilon;
            if (intensityExpI4 == 0) intensityExpI4 = double.Epsilon;

            intensityTheoryI0 = getI0Intensity(theoryIso);
            intensityTheoryI1 = getI1Intensity(theoryIso);
            intensityTheoryI2 = getI2Intensity(theoryIso);
            intensityTheoryI3 = getI3Intensity(theoryIso);
            intensityTheoryI4 = getI4Intensity(theoryIso);
            intensityTheoryI5 = getI4Intensity(theoryIso);

        }

        public void CalculateDandH(double DeuteriumLabelingEfficiency)//0 to 1
        {

            intensityHI0 = intensityExpI0 * intensityTheoryI0;
            intensityHI1 = intensityExpI0 * intensityTheoryI1;
            intensityHI2 = intensityExpI0 * intensityTheoryI2;
            intensityHI3 = intensityExpI0 * intensityTheoryI3;
            intensityHI4 = intensityExpI0 * intensityTheoryI4;
            intensityHI5 = intensityExpI0 * intensityTheoryI5;

            double tempIntensityDI0 = intensityExpI1 - intensityHI1;
            double tempIntensityDI1 = intensityExpI2 - intensityHI2;
            double tempIntensityDI2 = intensityExpI3 - intensityHI3;
            double tempIntensityDI3 = intensityExpI4 - intensityHI4;
            double tempIntensityDI4 = intensityExpI5 - intensityHI5;

            //zero test
            NegativeValueTest(ref tempIntensityDI0);
            NegativeValueTest(ref tempIntensityDI1);
            NegativeValueTest(ref tempIntensityDI2);
            NegativeValueTest(ref tempIntensityDI3);
            NegativeValueTest(ref tempIntensityDI4);

            intensityHI0 = intensityHI0 + tempIntensityDI0 * (1 - DeuteriumLabelingEfficiency);//un labled will add to the H
            intensityHI1 = intensityHI1 + tempIntensityDI1 * (1 - DeuteriumLabelingEfficiency);//un labled will add to the H
            intensityHI2 = intensityHI2 + tempIntensityDI2 * (1 - DeuteriumLabelingEfficiency);//un labled will add to the H
            intensityHI3 = intensityHI3 + tempIntensityDI3 * (1 - DeuteriumLabelingEfficiency);//un labled will add to the H
            intensityHI4 = intensityHI4 + tempIntensityDI4 * (1 - DeuteriumLabelingEfficiency);//un labled will add to the H

            tempIntensityDI4 = tempIntensityDI0 * DeuteriumLabelingEfficiency;
            tempIntensityDI4 = tempIntensityDI1 * DeuteriumLabelingEfficiency;
            tempIntensityDI4 = tempIntensityDI2 * DeuteriumLabelingEfficiency;
            tempIntensityDI4 = tempIntensityDI3 * DeuteriumLabelingEfficiency;
            tempIntensityDI4 = tempIntensityDI4 * DeuteriumLabelingEfficiency;

            NegativeValueTest(ref tempIntensityDI0);
            NegativeValueTest(ref tempIntensityDI1);
            NegativeValueTest(ref tempIntensityDI2);
            NegativeValueTest(ref tempIntensityDI3);
            NegativeValueTest(ref tempIntensityDI4);

            intensityDI0 = tempIntensityDI0;
            intensityDI1 = tempIntensityDI1;
            intensityDI2 = tempIntensityDI2;
            intensityDI3 = tempIntensityDI3;
            intensityDI4 = tempIntensityDI4;


            bool CorectMonoisotopicMasses = true;
            if (CorectMonoisotopicMasses)
            {
                double correctedH0;
                double correctedD0;
                CorrectMonoIsotopicMass(out correctedH0, out correctedD0);//require 3 points


                double HFixRatio = correctedH0 / intensityHI0;
                double DFixRatio = correctedD0 / intensityDI0;

                intensityHI0 = intensityHI0 * HFixRatio;
                intensityHI1 = intensityHI1 * HFixRatio;
                intensityHI2 = intensityHI2 * HFixRatio;
                intensityHI3 = intensityHI3 * HFixRatio;
                intensityHI4 = intensityHI4 * HFixRatio;


                intensityDI0 = intensityDI0 * DFixRatio;
                intensityDI1 = intensityDI1 * DFixRatio;
                intensityDI2 = intensityDI2 * DFixRatio;
                intensityDI3 = intensityDI3 * DFixRatio;
                intensityDI4 = intensityDI4 * DFixRatio;

                HydrogenAbundance = intensityHI0;//simple
            }
            else
            {
                HydrogenAbundance = intensityHI0;//simple


            }

        }

        private void CorrectMonoIsotopicMass(out double correctedH0, out double correctedD0)
        {
            correctedH0 = 0;
            correctedD0 = 0;

            List<XYData> peakList;
            peakList = ConvertToList(intensityHI0, intensityHI1, intensityHI2, intensityHI3, intensityHI4);

            List<XYData> theoryPeakList;
            theoryPeakList = ConvertToList(intensityTheoryI0, intensityTheoryI1, intensityTheoryI2, intensityTheoryI3, intensityTheoryI4);

            double intensityHI0Fit = CalculateFitIntensity(peakList, theoryPeakList, intensityHI0);

            double fractionDifference = (intensityHI0Fit - intensityHI0) / intensityHI0;
            double percentDiffereence = fractionDifference * 100;
            correctedH0 = intensityHI0Fit;

            peakList = ConvertToList(intensityDI0, intensityDI1, intensityDI2, intensityDI3, intensityDI4);

            double intensityDI0Fit = CalculateFitIntensity(peakList, theoryPeakList, intensityDI0);

            double fractionDDifference = (intensityDI0Fit - intensityDI0) / intensityDI0;
            double percentDDiffereence = fractionDifference * 100;

            double frigginDebug = percentDDiffereence * percentDiffereence;
            correctedD0 = intensityDI0Fit;
        }

        private double CalculateFitIntensity(List<XYData> peakList, List<XYData> theoryPeakList, double defaultValue)
        {
            int couterAbove1 = 0;
            foreach (var peak in peakList)
            {
                if (peak.Y > 1)
                {
                    couterAbove1++;
                }
            }

            double intensityFit = 0;
            if (couterAbove1 >= 3)//we have 3 points
            {

                double A = 0;
                double B = 0;
                double C = 0;
                ParabolaABC(theoryPeakList, ref A, ref B, ref C);

                Concavity curveShapeTheory = ConcavityAssignment(A);

                //now to see if real data follows trend
                ParabolaABC(peakList, ref A, ref B, ref C);

                Concavity curveShapeExperimental = ConcavityAssignment(A);


                if (curveShapeTheory == curveShapeExperimental)//second deravitive test being equal
                {
                    intensityFit = C;
                }
                else
                {
                    intensityFit = defaultValue;//not enough points so take first value
                }
            }
            else
            {
                intensityFit = defaultValue;//not enough points so take first value
            }
            return intensityFit;
        }

        private static Concavity ConcavityAssignment(double A)
        {
            Concavity curveShapeTheory;
            if (2 * A >= 0)//second deravitive test  + will be concaved down
            {
                curveShapeTheory = Concavity.Convex;//large mass
            }
            else
            {
                curveShapeTheory = Concavity.Concave;//small mass
            }
            return curveShapeTheory;
        }

        /// <summary>
        /// Find the coefficeints to the parabola that goes through the data points
        /// </summary>
        /// <param name="peakTopList">A list of PNNL Omics XYData</param>
        /// <returns>XYData point correspiding to the pair at the apex intensity and center of mass </returns>
        private void ParabolaABC(List<XYData> peakSideList, ref double aOut, ref double bOut, ref double cOut)//aX^2+bX+c
        {
            #region copied code
            double apexMass;
            double apexIntensity;

            int numberOfPoints = peakSideList.Count;

            //switch x with Y


            //for (int i = 0; i < numberOfPoints; i++)
            //{
            //    XYData tempPoint = new XYData();
            //    tempPoint.Y = peakSideList[i].X;
            //    tempPoint.X = peakSideList[i].Y;
            //    peakSideList[i].X = tempPoint.X;
            //    peakSideList[i].Y = tempPoint.Y;
            //}

            //print "run Parabola"
            double InitialX;  //used to offset the parabola to zero
            InitialX = peakSideList[0].X;
            for (int i = 0; i < numberOfPoints; i++)
            {
                peakSideList[i].X = (float)(peakSideList[i].X - InitialX);
            }

            double T1 = 0, T2 = 0, T3 = 0;
            double X1 = 0, X2 = 0, X3 = 0;
            double Y1 = 0, Y2 = 0, Y3 = 0;
            double Z1 = 0, Z2 = 0, Z3 = 0;
            double Theta1, Theta2, Theta3, ThetaDenominator;
            double A = 0, B = 0, C = 0;
            double X = 0;

            //TODO unit test
            //peakTopList[0].X=1;
            //peakTopList[1].X=2;
            //peakTopList[2].X=3;
            //peakTopList[0].Y=2;
            //peakTopList[1].Y=5;
            //peakTopList[2].Y = 4;

            for (int i = 0; i < numberOfPoints; i += 1)
            {
                T1 += peakSideList[i].X * peakSideList[i].X * peakSideList[i].Y;
            }

            T1 = 2 * T1;

            for (int i = 0; i < numberOfPoints; i += 1)
            {
                T2 += peakSideList[i].X * peakSideList[i].Y;
            }

            T2 = 2 * T2;

            for (int i = 0; i < numberOfPoints; i += 1)
            {
                T3 += peakSideList[i].Y;
            }

            T3 = 2 * T3;

            for (int i = 0; i < numberOfPoints; i += 1)
            {
                X1 += peakSideList[i].X * peakSideList[i].X * peakSideList[i].X * peakSideList[i].X;
            }

            X1 = 2 * X1;

            for (int i = 0; i < numberOfPoints; i += 1)
            {
                X2 += peakSideList[i].X * peakSideList[i].X * peakSideList[i].X;
            }

            X2 = 2 * X2;

            for (int i = 0; i < numberOfPoints; i += 1)
            {
                X3 += peakSideList[i].X * peakSideList[i].X;
            }

            X3 = 2 * X3;

            Y1 = X2;
            Y2 = X3;

            for (int i = 0; i < numberOfPoints; i += 1)
            {
                Y3 += peakSideList[i].X;
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


        private void NegativeValueTest(ref double value)
        {
            if (value < 0)
            {
                value = 0;
            }

        }

       
        public void SetMinIntensity(double minIntensity)
        {
            //double minIntensity = 1;//make sure there is a peak there so that we do not get infininty
            MinIntensity = minIntensity;
        }


        private void resetIntensityValues()
        {
            intensityExpI0 = 0;
            intensityExpI1 = 0;
            intensityExpI2 = 0;
            intensityExpI3 = 0;
            intensityExpI4 = 0;
            intensityExpI5 = 0;

            intensityTheoryI0 = 0;
            intensityTheoryI1 = 0;
            intensityTheoryI2 = 0;
            intensityTheoryI3 = 0;
            intensityTheoryI4 = 0;
            intensityTheoryI5 = 0;

            intensityHI0 = 0;
            intensityHI1 = 0;
            intensityHI2 = 0;
            intensityHI3 = 0;
            intensityHI4 = 0;
            intensityHI5 = 0;

            intensityDI0 = 0;
            intensityDI1 = 0;
            intensityDI2 = 0;
            intensityDI3 = 0;
            intensityDI4 = 0;

            HydrogenAbundance = 0;

        }


        private static List<XYData> ConvertToList(double val0, double val1, double val2, double val3, double val4)
        {
            List<XYData> peakList = new List<XYData>();
            bool keepInLoop = true;
            double massIsotopeSpace = 1.0031;
            while (keepInLoop)
            {
                if (val0 > 0)
                {
                    peakList.Add(new XYData(0, val0));
                }
                else
                {
                    break;
                }

                if (val1 > 0)
                {
                    peakList.Add(new XYData(1 * massIsotopeSpace, val1));
                }
                else
                {
                    break;
                }
                if (val2 > 0)
                {
                    peakList.Add(new XYData(2 * massIsotopeSpace, val2));
                }
                else
                {
                    break;
                }

                if (val3 > 0)
                {
                    peakList.Add(new XYData(3 * massIsotopeSpace, val3));
                }
                else
                {
                    break;
                }
                if (val4 > 0)
                {
                    peakList.Add(new XYData(4 * massIsotopeSpace, val4));
                }
                else
                {
                    break;
                }

                keepInLoop = false;
            }

            return peakList;
        }


        private double getI0Intensity(IsotopicProfile iso)
        {
            if (iso == null || iso.Peaklist == null || iso.Peaklist.Count < 1)
            {
                return 0;
            }
            double intensity = iso.Peaklist[0].Height;
            if (intensity == 0) intensity = 0;
            return intensity;
        }

        private double getI1Intensity(IsotopicProfile iso)
        {
            if (iso == null || iso.Peaklist == null || iso.Peaklist.Count < 2)
            {
                return 0;
            }
            double intensity = iso.Peaklist[1].Height;
            if (intensity == 0) intensity = 0;
            return intensity;
        }

        private double getI2Intensity(IsotopicProfile iso)
        {
            if (iso == null || iso.Peaklist == null || iso.Peaklist.Count < 3)
            {
                return 0;
            }
            double intensity = iso.Peaklist[2].Height;
            return intensity;
        }

        private double getI3Intensity(IsotopicProfile iso)
        {
            if (iso == null || iso.Peaklist == null || iso.Peaklist.Count < 4)
            {
                return 0;
            }
            double intensity = iso.Peaklist[3].Height;
            return intensity;
        }

        private double getI4Intensity(IsotopicProfile iso)
        {
            if (iso == null || iso.Peaklist == null || iso.Peaklist.Count < 5)
            {
                return 0;
            }
            double intensity = iso.Peaklist[4].Height;
            return intensity;
        }

        private double getI5Intensity(IsotopicProfile iso)
        {
            if (iso == null || iso.Peaklist == null || iso.Peaklist.Count < 6)
            {
                return 0;
            }
            double intensity = iso.Peaklist[5].Height;
            return intensity;
        }

        public enum Concavity
        {
            Convex,
            Concave
        }
    }
}
