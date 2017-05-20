using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using GetPeaks_DLL.DataFIFO;
using GetPeaksDllLite.DataFIFO;
using IQGlyQ.Objects;
using PNNLOmics.Algorithms.PeakDetection;
using PNNLOmics.Algorithms.Solvers.LevenburgMarquadt;
using PNNLOmics.Data;
using IQGlyQ.Enumerations;

namespace IQGlyQ.Functions
{
    public static class CurveFit
    {
        //public static List<XYData> Fit_LM(List<XYData> data, out SolverReport fitMetrics, out double area, ref Tuple<string, string> errorLog)
        //{
        //    bool test1 = Utiliites.SignPostRequire(data.Count > 2, "Enough data to fit");//perhaps more

        //    area = 0;

        //    List<PNNLOmics.Data.XYData> fit = new List<XYData>();
        //    if (test1)
        //    {
        //        double[] coefficents;
        //        fit = FitPeakShape(data, out fitMetrics, out area, out coefficents);

        //        //WriteXYData(fit);
        //    }
        //    else
        //    {
        //        fitMetrics = null;
        //        Utiliites.MakeSignPostForTrue(test1, "Not enough data points", "CurveFit_LM", ref errorLog);
        //        //Console.WriteLine("Failed LM");
        //    }

        //    return fit;
        //}

        //this returned a modeled peak based on the coefficients
        public static List<XYData> Fit_LM(List<XYData> data, out SolverReport fitMetrics, out double area, ref double[] coefficents, out List<XYData> modeledPeakList, int numberOfSamples, double sampleWidth, ref Tuple<string, string> errorLog, out ScanObject scanRange, EnumerationCurveType curveType, bool centerXAxisOnInteger)
        {
            bool test1 = Utiliites.SignPostRequire(data.Count > 2, "Enough data to fit");//perhaps more

            int counter = 0;
            int breakOut = 3;//we need at least 3 pionts to fit

            foreach (XYData xyData in data)
            {
                if (xyData.Y > 0) counter++;

                if (counter >= breakOut) break;
            }
            bool test2 = Utiliites.SignPostRequire(counter >= breakOut, "Enough data above zero to fit");

            area = 0;
            scanRange = new ScanObject(0, 0);

            bool writeData = false;
            if (writeData)
            {
                double[] checkArray = new double[data.Count];
                List<string> lines = new List<string>();
                for (int i = 0; i < data.Count; i++)
                {
                    checkArray[i] = data[i].Y;
                    lines.Add(data[i].X + @"," + data[i].Y);
                }
                StringListToDisk writer = new StringListToDisk();
                writer.toDiskStringList(@"D:\Csharp\0_TestDataFiles\SpecialLMFitFail.csv", lines);
            }

            List<PNNLOmics.Data.XYData> fit = new List<XYData>();//actual fit across real, bounded x axis
            modeledPeakList = new List<XYData>();//modeled fit across many points
            if (test1 && test2)
            {
                //double[] coefficents;
                fit = FitPeakShape(data, curveType, out fitMetrics, out area, ref coefficents);

                // http://www.alglib.net/translator/man/manual.csharp.html#struct_lsfitreport

                //new 1-9-2014  we need to converge if we want to predict the values
                if (fitMetrics.DidConverge == true)
                {
                    double halfWidthAtTenthHeightLower;
                    double halfWidthAtTenthHeightHigher;
                    LevenburgMarquardt.ReturnTenthHeight(coefficents, coefficents[2], out halfWidthAtTenthHeightLower, out halfWidthAtTenthHeightHigher);

                    scanRange.Start = Convert.ToInt32(halfWidthAtTenthHeightLower);
                    scanRange.Stop = Convert.ToInt32(halfWidthAtTenthHeightHigher);
                    //returns constant peak spacing but with variable range 


                    double distributionCenter = coefficents[2];


                    //do we want to center the x values on an integer.  the y values will reamin true to the coefficeints just evaluated at integer spacing
                    //bool centerXAxisOnInteger = true;

                    if (centerXAxisOnInteger)
                    {
                        distributionCenter = Math.Round(distributionCenter, 0);

                    }


                    switch (curveType)
                    {
                        case EnumerationCurveType.Gaussian:
                            {
                                modeledPeakList = LevenburgMarquardt.ReturnGaussianValues(coefficents, numberOfSamples, sampleWidth, distributionCenter);
                            }
                            break;
                        case EnumerationCurveType.Lorentzian:
                            {
                                modeledPeakList = LevenburgMarquardt.ReturnLorentzianValues(coefficents, numberOfSamples, sampleWidth, distributionCenter);
                            }
                            break;
                        defualt:
                            {
                                modeledPeakList = LevenburgMarquardt.ReturnGaussianValues(coefficents, numberOfSamples, sampleWidth, distributionCenter);
                            }
                            break;
                    }


                    //returns constant range with variable peak spacing
                    //modeledPeakList = LevenburgMarquardt.ReturnGaussianValues(coefficents, numberOfSamples, halfWidthAtTenthHeightLower, halfWidthAtTenthHeightHigher, coefficents[2]);
                }
                else
                {
                    //fitMetrics = null;
                    bool test3 = true;
                    //this would be nice  fitMetrics.RSquared = 0;  instead we need to check for convergence along with the equality
                    Utiliites.MakeSignPostForTrue(test3, "Not enough data points", "CurveFit_LM", ref errorLog);
                }
            }
            else
            {
                fitMetrics = null;
                Utiliites.MakeSignPostForTrue(test1, "Failed LM Curve Fit", "CurveFit_LM", ref errorLog);
                //Console.WriteLine("Failed LM");
            }

            return fit;
        }

        private static List<XYData> FitPeakShape(List<XYData> data, EnumerationCurveType curveType, out SolverReport fitMetrics, out double area, ref double[] coefficentsGuess)
        {
            List<XYData> fit = new List<XYData>();

            switch (curveType)
            {
                case EnumerationCurveType.Gaussian:
                    {
                        //double[] guess = new double[3];
                        //guess[0] = 2; //sigma
                        //guess[1] = data.Max(r => r.Y); //height
                        ////guess X by mid point of curve
                        //double center = data.Count / 2;
                        //int possibleCenter = Convert.ToInt32(Math.Truncate(center));
                        //guess[2] = data[possibleCenter].X; //m/z

                        bool calculateArea = true;
                        fit = LevenburgMarquardt.FitGaussian(data, calculateArea, ref coefficentsGuess, out fitMetrics, out area);

                        //coefficents = guess;
                        //Console.WriteLine("       sigma=" + coefficentsGuess[0] + " height=" + coefficentsGuess[1] + " MZ=" + coefficentsGuess[2]);
                    }
                    break;
                case EnumerationCurveType.Lorentzian:
                    {
                        //double[] guess = new double[3];
                        //guess[0] = 2; //sigma
                        //guess[1] = data.Max(r => r.Y); //height
                        ////guess X by mid point of curve
                        //double center = data.Count / 2;
                        //int possibleCenter = Convert.ToInt32(Math.Truncate(center));
                        //guess[2] = data[possibleCenter].X; //m/z
                        bool calculateArea = true;
                        fit = LevenburgMarquardt.FitLorentzian(data, calculateArea, ref coefficentsGuess, out fitMetrics, out area);

                        //coefficents = guess;
                        //Console.WriteLine("       sigma=" + coefficentsGuess[0] + " height=" + coefficentsGuess[1] + " MZ=" + coefficentsGuess[2]);
                    }
                    break;
                default:
                    {
                        //double[] guess = new double[3];
                        //guess[0] = 2; //sigma
                        //guess[1] = data.Max(r => r.Y); //height
                        ////guess X by mid point of curve
                        //double center = data.Count/2;
                        //int possibleCenter = Convert.ToInt32(Math.Truncate(center));
                        //guess[2] = data[possibleCenter].X; //m/z
                        bool calculateArea = true;
                        fit = LevenburgMarquardt.FitGaussian(data, calculateArea, ref coefficentsGuess, out fitMetrics, out area);

                        //coefficents = guess;
                        //Console.WriteLine("       sigma=" + coefficentsGuess[0] + " height=" + coefficentsGuess[1] + " MZ=" + coefficentsGuess[2]);
                    }
                    break;
            }



            return fit;
        }

    }
}
