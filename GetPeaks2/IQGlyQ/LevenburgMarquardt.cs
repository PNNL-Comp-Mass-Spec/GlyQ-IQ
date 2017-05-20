using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNNLOmics.Algorithms.PeakDetection;
using PNNLOmics.Algorithms.Solvers;
using PNNLOmics.Algorithms.Solvers.LevenburgMarquadt;
using PNNLOmics.Algorithms.Solvers.LevenburgMarquadt.BasisFunctions;
using PNNLOmics.Data;
using PNNLOmics.Data.Peaks;

namespace IQGlyQ
{
    public static class LevenburgMarquardt
    {
        private static PNNLOmics.Algorithms.PeakDetection.PeakCentroider OmicsPeakDetection { get; set; }
        
        
        public static List<PNNLOmics.Data.XYData> FitGaussian(List<PNNLOmics.Data.XYData> data, bool calcuateArea, ref double[] guess, out SolverReport fitMetrics, out double area)
        {
            List<PNNLOmics.Data.XYData> fitFunction = new List<XYData>();
            area = 0;

            if (guess.Length != 3)
            {
                Console.WriteLine("guess array is wrong size");
                fitMetrics = null;
                return fitFunction;
            }

            List<double> x;
            List<double> y;
            ConvertXYDataToArrays(data, out x, out y);

            BasisFunctionsEnum functionChoise = BasisFunctionsEnum.Gaussian;

            BasisFunctionBase functionSelector = BasisFunctionFactory.BasisFunctionSelector(functionChoise);
            //coeffs = functionSelector.Coefficients;
            double[] coeffs = functionSelector.Coefficients;

            coeffs[0] = guess[0];//sigma
            coeffs[1] = guess[1];//height
            coeffs[2] = guess[2];//xoffset

            alglib.ndimensional_pfunc myDelegate = functionSelector.FunctionDelegate;

            LevenburgMarquadtSolver solver = new LevenburgMarquadtSolver();
            solver.BasisFunction = myDelegate;
            try
            {
                fitMetrics = solver.Solve(x, y, ref coeffs);
            }
            catch (Exception)
            {
               
               Console.WriteLine("Failed LM Solve with parameters");
               fitMetrics = solver.Solve(x, y, ref coeffs);
            }
            

            //Console.WriteLine("sortZ "+ fitMetrics.RSquared);

            if (fitMetrics.DidConverge)
            {
                // Uncomment to debug: Console.WriteLine("       LM Worked \t FitY \t ExpY \t" + coeffs[0] + "\t" + coeffs[1] + "\t" + coeffs[2]);
                
                
                for (int i = 0; i < x.Count; i++)
                {
                    guess[0] = coeffs[0];//sigma
                    guess[1] = coeffs[1];//height
                    guess[2] = coeffs[2];//xoffset
                                    
                    // This is what we are fitting 
                    double xValue = x[i];

                    // This is what it should fit to
                    double yValue = y[i];

                    double fitValue = 0;
                    myDelegate.Invoke(coeffs, new double[] {xValue}, ref fitValue, null);

                    fitFunction.Add(new XYData(xValue, fitValue));

                    //Console.WriteLine("{0}\t{1}\t{2}", xValue, fitValue, yValue);
                }
                if (calcuateArea)
                {
                    OmicsPeakDetection = new PeakCentroider();
                    
                    NumericalIntegrationBase integrator = new TrapezoidIntegration();
                    double extendIntegrationSigma = 4;
                    List<ProcessedPeak> peak = OmicsPeakDetection.DiscoverPeaks(fitFunction);

                    //here one possible the problem.  if the fit function does not have a maximum in the narrow range of fitFunction, the center can't be found and thus the area fails
                    if(peak.Count==0)
                    {
                        peak = OmicsPeakDetection.DiscoverPeaks(data);//take the center from the raw data if the fit fails
                    }

                    if (peak.Count == 1)
                    {
                        double sigmaFromCoeffiecients = guess[0];
                        //double distanceFromCenter = peak[0].Width/2.35482*extendIntegrationSigma;
                        double distanceFromCenter = sigmaFromCoeffiecients * extendIntegrationSigma;
                        double lowerBound = peak[0].XValue - distanceFromCenter;
                        double upperBound = peak[0].XValue + distanceFromCenter;
                        area = integrator.Integrate(functionSelector, coeffs, lowerBound, upperBound, 500);
                        }
                    else
                    {
                        area = -1;//there are no peaks here
                    }
                }
            }
            else
            {
                Console.WriteLine("       Fail LM, fitMetrics.didConverge is set to " + fitMetrics.DidConverge);
            }

            

            return fitFunction;
        }

        public static void ReturnTenthHeight(double[] coeffs, double centerX, out double halfWidthAtTenthHeightLower, out double halfWidthAtTenthHeightHigher)
        {
            double fullWidthAtTenthHeight = (4.2919320526)*coeffs[0];//? is coeff 0==c==sigma?
            halfWidthAtTenthHeightLower = centerX - (fullWidthAtTenthHeight) / 2;
            halfWidthAtTenthHeightHigher = centerX + (fullWidthAtTenthHeight) / 2;

            
        }

        public static List<PNNLOmics.Data.XYData> ReturnGaussianValues(double[] coeffs, int numberOfSamples, double sampleSpacing, double centerX)
        {
            List<PNNLOmics.Data.XYData> fitXYData = new List<XYData>();

            //make sure numberOfSamples = even

            BasisFunctionsEnum functionChoise = BasisFunctionsEnum.Gaussian;

            BasisFunctionBase functionSelector = BasisFunctionFactory.BasisFunctionSelector(functionChoise);
            functionSelector.Coefficients = coeffs;

            alglib.ndimensional_pfunc myDelegate = functionSelector.FunctionDelegate;

            LevenburgMarquadtSolver solver = new LevenburgMarquadtSolver();
            solver.BasisFunction = myDelegate;


            //for (int i = 0; i < numberOfSamples+1; i+=1)
            for (int i = 0; i < numberOfSamples + 1; i++)
            {
                double xValue = centerX - (numberOfSamples / 2 * sampleSpacing) + i * sampleSpacing;


                double fitValue = 0;
                myDelegate.Invoke(coeffs, new double[] { xValue }, ref fitValue, null);

                fitXYData.Add(new XYData(xValue, fitValue));

                //Console.WriteLine("{0}\t{1}", xValue, fitValue);
            }

            return fitXYData;
        }

        public static List<PNNLOmics.Data.XYData> ReturnGaussianValues(double[] coeffs, int numberOfSamples, double start, double stop, double centerX)
        {
            List<PNNLOmics.Data.XYData> fitXYData = new List<XYData>();

            //make sure numberOfSamples = even

            BasisFunctionsEnum functionChoise = BasisFunctionsEnum.Gaussian;

            BasisFunctionBase functionSelector = BasisFunctionFactory.BasisFunctionSelector(functionChoise);
            functionSelector.Coefficients = coeffs;

            alglib.ndimensional_pfunc myDelegate = functionSelector.FunctionDelegate;

            LevenburgMarquadtSolver solver = new LevenburgMarquadtSolver();
            solver.BasisFunction = myDelegate;

            double sampleSpacing = (stop - start +1)/numberOfSamples;//+1 so we have a apex at a point
            //for (int i = 0; i < numberOfSamples+1; i+=1)
            for (int i = 0; i < numberOfSamples + 1; i++)
            {
                double xValue = start + sampleSpacing*i;


                double fitValue = 0;
                myDelegate.Invoke(coeffs, new double[] { xValue }, ref fitValue, null);

                fitXYData.Add(new XYData(xValue, fitValue));

                //Console.WriteLine("{0}\t{1}", xValue, fitValue);
            }

            return fitXYData;
        }

        public static List<PNNLOmics.Data.XYData> FitLorentzian(List<PNNLOmics.Data.XYData> data, bool calcuateArea, ref double[] guess, out SolverReport fitMetrics, out double area)
        {
            List<PNNLOmics.Data.XYData> fitFunction = new List<XYData>();
            area = 0;

            if (guess.Length != 3)
            {
                Console.WriteLine("guess array is wrong size");
                fitMetrics = null;
                return fitFunction;
            }

            List<double> x;
            List<double> y;
            ConvertXYDataToArrays(data, out x, out y);

            BasisFunctionsEnum functionChoise = BasisFunctionsEnum.Lorentzian;

            BasisFunctionBase functionSelector = BasisFunctionFactory.BasisFunctionSelector(functionChoise);
            //coeffs = functionSelector.Coefficients;
            double[] coeffs = functionSelector.Coefficients;

            coeffs[0] = guess[0];//sigma
            coeffs[1] = guess[1];//height
            coeffs[2] = guess[2];//xoffset

            alglib.ndimensional_pfunc myDelegate = functionSelector.FunctionDelegate;

            LevenburgMarquadtSolver solver = new LevenburgMarquadtSolver();
            solver.BasisFunction = myDelegate;
            fitMetrics = solver.Solve(x, y, ref coeffs);

            Console.WriteLine("sortZ " + fitMetrics.RSquared);

            if (fitMetrics.DidConverge)
            {
                // Uncomment to debug: Console.WriteLine("LM Worked \t FitY \t ExpY \t" + coeffs[0] + "\t" + coeffs[1] + "\t" + coeffs[2]);


                for (int i = 0; i < x.Count; i++)
                {
                    guess[0] = coeffs[0];//sigma
                    guess[1] = coeffs[1];//height
                    guess[2] = coeffs[2];//xoffset

                    // This is what we are fitting 
                    double xValue = x[i];

                    // This is what it should fit to
                    double yValue = y[i];

                    double fitValue = 0;
                    myDelegate.Invoke(coeffs, new double[] { xValue }, ref fitValue, null);

                    fitFunction.Add(new XYData(xValue, fitValue));

                    Console.WriteLine("{0}\t{1}\t{2}", xValue, fitValue, yValue);
                }
                if (calcuateArea)
                {
                    OmicsPeakDetection = new PeakCentroider();

                    NumericalIntegrationBase integrator = new TrapezoidIntegration();
                    double extendIntegrationSigma = 4;
                    List<ProcessedPeak> peak = OmicsPeakDetection.DiscoverPeaks(fitFunction);

                    //here one possible the problem.  if the fit function does not have a maximum in the narrow range of fitFunction, the center can't be found and thus the area fails
                    if (peak.Count == 0)
                    {
                        peak = OmicsPeakDetection.DiscoverPeaks(data);//take the center from the raw data if the fit fails
                    }

                    if (peak.Count == 1)
                    {
                        double sigmaFromCoeffiecients = guess[0];
                        //double distanceFromCenter = peak[0].Width/2.35482*extendIntegrationSigma;
                        double distanceFromCenter = sigmaFromCoeffiecients * extendIntegrationSigma;
                        double lowerBound = peak[0].XValue - distanceFromCenter;
                        double upperBound = peak[0].XValue + distanceFromCenter;
                        area = integrator.Integrate(functionSelector, coeffs, lowerBound, upperBound, 500);
                    }
                    else
                    {
                        area = -1;//there are no peaks here
                    }
                }
            }
            else
            {
                Console.WriteLine("Fail LM");
            }



            return fitFunction;
        }

        public static List<PNNLOmics.Data.XYData> ReturnLorentzianValues(double[] coeffs, int numberOfSamples, double sampleSpacing, double centerX)
        {
            List<PNNLOmics.Data.XYData> fitXYData = new List<XYData>();

            //make sure numberOfSamples = even

            BasisFunctionsEnum functionChoise = BasisFunctionsEnum.Lorentzian;

            BasisFunctionBase functionSelector = BasisFunctionFactory.BasisFunctionSelector(functionChoise);
            functionSelector.Coefficients = coeffs;

            alglib.ndimensional_pfunc myDelegate = functionSelector.FunctionDelegate;

            LevenburgMarquadtSolver solver = new LevenburgMarquadtSolver();
            solver.BasisFunction = myDelegate;


            //for (int i = 0; i < numberOfSamples+1; i+=1)
            for (int i = 0; i < numberOfSamples + 1; i++)
            {
                double xValue = centerX - (numberOfSamples / 2 * sampleSpacing) + i * sampleSpacing;


                double fitValue = 0;
                myDelegate.Invoke(coeffs, new double[] { xValue }, ref fitValue, null);

                fitXYData.Add(new XYData(xValue, fitValue));

                //Console.WriteLine("{0}\t{1}", xValue, fitValue);
            }

            return fitXYData;
        }




        private static void ConvertXYDataToArrays(List<PNNLOmics.Data.XYData> data, out List<double> x, out List<double> y)
        {
            x = new List<double>();
            y = new List<double>();
            foreach (var xyData in data)
            {
                x.Add(xyData.X);
                y.Add(xyData.Y);
            }
        }
    
    
    }
}
