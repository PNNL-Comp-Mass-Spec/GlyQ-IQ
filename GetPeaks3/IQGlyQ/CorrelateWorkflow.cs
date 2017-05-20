using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using IQGlyQ.Enumerations;
using IQGlyQ.Objects;
using IQGlyQ.Results;
using IQ_X64.Backend.ProcessingTasks.ChromatogramProcessing;
using IQ_X64.Workflows.Core;
using PNNLOmics.Data;
using IQGlyQ.Functions;
using Run64.Backend.Core;
using PNNLOmics.Data.Peaks;

namespace IQGlyQ
{
    /// <summary>
    /// check candidates to see if they correlate.  step 1 is isolate peak shapes.  step 2 is fit gaussians to shapes
    /// </summary>
    public static class CorrelateWorkflow
    {
        public static FragmentResultsObjectHolderIq Correlate(
            IqTarget fragmentTarget, IqTarget parentTarget,
            FragmentResultsObjectHolderIq fragmentResult, FragmentResultsObjectHolderIq parentResult,
            int scanLCTarget,
            ProcessedPeak fragmentCandiateFitPeak, ProcessedPeak parentCandiateFitPeak,
            List<XYData> fragmentEicFitXyData, List<XYData> parentEicFitXyData,
            ref ChromatogramCorrelatorBase chromatogramCorrelator, double correlationscorecuttoff, int minimumPointsForOverlap)
        {
            EnumerationError localError = EnumerationError.NoError;
            //check inputs
            bool test0 = Utiliites.SignPostRequire(fragmentTarget.ChargeState == 0, "Charge State Fragment");
            bool test1 = Utiliites.SignPostRequire(parentTarget.ChargeState == 0, "Charge State Parent");
            bool test2 = Utiliites.SignPostRequire(parentEicFitXyData == null || fragmentEicFitXyData.Count == 0, "PeakList Fragment");
            bool test3 = Utiliites.SignPostRequire(parentEicFitXyData==null || parentEicFitXyData.Count == 0, "PeakList Parent");
            bool test4 = Utiliites.SignPostRequire(fragmentCandiateFitPeak == null, "Peak Fragment");
            bool test5 = Utiliites.SignPostRequire(parentCandiateFitPeak == null, "Peak Parent");
            bool test6 = Utiliites.SignPostRequire(fragmentResult.CorrelationCoefficients == null, " fragment correlation Coefficients");
            bool test7 = Utiliites.SignPostRequire(parentResult.CorrelationCoefficients == null , " parent correlation Coefficients");

            bool test8 = true;
            bool test9 = true;
            if(!test6) test8 = Utiliites.SignPostRequire(fragmentResult.CorrelationCoefficients.Length !=3, " fragment correlation Coefficients");
            if(!test7) test9 = Utiliites.SignPostRequire(parentResult.CorrelationCoefficients.Length !=3, " parent correlation Coefficients");
            //bool test7 = Utiliites.SignPostRequire(fragmentResult.CorrelationCoefficients.Length=0 && parentResult.CorrelationCoefficients.Length == 0, "correlation Coefficients");

            if (test0) localError = EnumerationError.FailedChargeState;
            if (test1) localError = EnumerationError.FailedChargeState;
            if (test2) localError = EnumerationError.MissingPoints;
            if (test3) localError = EnumerationError.MissingPoints;
            if (test4) localError = EnumerationError.MissingPeak;
            if (test5) localError = EnumerationError.MissingPeak;
            if (test6) localError = EnumerationError.NoCorrealtionCoefficients;
            if (test7) localError = EnumerationError.NoCorrealtionCoefficients;
            if (test8) localError = EnumerationError.NoCorrealtionCoefficients;
            if (test9) localError = EnumerationError.NoCorrealtionCoefficients;
            //if (test7) localError = ErrorEnumeration.NoCorrealtionCoefficients;


            //default result
            ChromCorrelationData emptyChromeResults = new ChromCorrelationData();
            //result = SetUpObjectHolderResult(dataToReturn, fragmentTarget, targetParent, scanLCTarget, emptyChromeResults);
            FragmentResultsObjectHolderIq dataToReturn = SetUpObjectHolderResult(fragmentTarget, parentTarget, scanLCTarget, emptyChromeResults);

            if (localError == EnumerationError.NoError)
            {
                Tuple<string, string> errorlog = new Tuple<string, string>("CorrelateWorkflow", "Success");

                #region 4c, cut to same size and coorelate.  We need both fragment and parent processed at this point

                //find common range between both fit distributions
                int startScan = 0;
                int stopScan = 0;
                //bool checkScanRange = Utiliites.FindCommonStartStopBetweenCurves(fragmentEicFitXyData, fragmentCandiateFitPeak, parentEicFitXyData, parentCandiateFitPeak, ref startScan, ref stopScan, ref errorlog);
                bool checkScanRange = Utiliites.FindCommonStartStopBetweenCurves(fragmentResult.ScanBoundsInfo, parentResult.ScanBoundsInfo, ref startScan, ref stopScan, ref errorlog);

                if (checkScanRange)
                {
                    #region inside

                    //rewindow to common start and stop

                    //correlate fit data
                    List<XYData> possibleFragmentCorrelationEicWindow = SelectXYDataToCorrelate(startScan, stopScan, fragmentResult);
                    List<XYData> parentCorrelationEicWindow = SelectXYDataToCorrelate(startScan, stopScan, parentResult);
                    //SelectXYDataToCorrelate(fragmentEicFitXyData, parentEicFitXyData, stopScan, startScan, out possibleFragmentCorrelationEICWindow, out parentCorrelationEICWindow);
                    //SelectXYDataToCorrelate(startScan, stopScan, fragmentResult, parentResult, out possibleFragmentCorrelationEICWindow, out parentCorrelationEICWindow);

                    double maxOffragment = possibleFragmentCorrelationEicWindow.Max(x => x.Y);
                    double maxOfParent = parentCorrelationEicWindow.Max(x => x.Y);

                    //normalize?
                    //foreach (var xyData in parentCorrelationEIC)
                    //{
                    //    xyData.Y = xyData.Y * maxOffragment / maxOfParent;
                    //}

                    bool towWrite = false;

                    if(towWrite) Console.WriteLine("\t m/z \t Fragment \t Parent");
                    for (int k = 0; k < possibleFragmentCorrelationEicWindow.Count; k++)
                    {
                        string x1 = possibleFragmentCorrelationEicWindow[k].X.ToString(CultureInfo.InvariantCulture);
                        string y1 = possibleFragmentCorrelationEicWindow[k].Y.ToString(CultureInfo.InvariantCulture);
                        string y2 = parentCorrelationEicWindow[k].Y.ToString(CultureInfo.InvariantCulture);
                        if(towWrite) Console.WriteLine("Correlate \t" + x1 + "\t" + y1 + "\t" + y2);
                    }

                    //SimplePeakCorrelator correlator = new SimplePeakCorrelator(resultList.Run, _workflowParameters, _workflowParameters.MinRelativeIntensityForChromCorrelator);
                    SimplePeakCorrelator correlator = (SimplePeakCorrelator)chromatogramCorrelator;
                    correlator.BasePeakIso = fragmentTarget.TheorIsotopicProfile; //this is where the possibleFragmentTarget goes
                    correlator.IsoPeaklist = new List<IsotopicProfile>();
                    correlator.IsoPeaklist.Add(parentTarget.TheorIsotopicProfile); //simple correlation with largest intensity in candidate Parents

                    //ChromCorrelationData chromeResults = correlator.CorrelateDataXY(possibleFragmentCorrelationEICWindow, parentCorrelationEICWindow, startScan, stopScan);

                    ChromCorrelationData chromeResults = new ChromCorrelationData();
                    if (possibleFragmentCorrelationEicWindow.Count >= minimumPointsForOverlap)
                    {
                        chromeResults = correlator.CorrelateDataXYNonInteger(possibleFragmentCorrelationEicWindow, parentCorrelationEicWindow, startScan, stopScan);

                        List<XYData> normalizedPossibleFragmentCorrelationEicWindow = NormalizeData(possibleFragmentCorrelationEicWindow ,maxOffragment);
                        List<XYData> normalizedParentCorrelationEicWindow = NormalizeData(parentCorrelationEicWindow, maxOffragment);
                        //p values would be better if normalized by intensity
                        //PNNLOmics.Algorithms.Distance.PearsonCorrelation calc = new PearsonCorrelation();
                        //bool joesPiersonCorrelation = calc.Pearson(normalizedPossibleFragmentCorrelationEicWindow, normalizedParentCorrelationEicWindow);

                        //Console.WriteLine(Environment.NewLine + "Joe Correlation-pvalue: " + calc.Pvalue + " rsquared: " + calc.RSquared);
                    }
                    else
                    {
                        Console.WriteLine("Too few ponts overlap to run correlation");
                        chromeResults.CorrelationDataItems.Add(new ChromCorrelationDataItem(0, 0, -2));
                    }

                    //these results are associated with the parent!!!!!!!!
                    dataToReturn = SetUpObjectHolderResult(dataToReturn, fragmentTarget, parentTarget, scanLCTarget, chromeResults);

                    //correlation score
                    if (chromeResults.CorrelationDataItems != null && chromeResults.CorrelationDataItems.Count > 0)
                    {
                        dataToReturn.CorrelationScore = Convert.ToDouble(chromeResults.CorrelationDataItems[0].CorrelationRSquaredVal);
                    }

                    //intact assignments
                    if (chromeResults.CorrelationDataItems.Count > 0) //this should be true if the correlation returned data
                    {
                        #region inside

                        //Does the notch correlate at this charge state
                        if (chromeResults.RSquaredValsAverage > correlationscorecuttoff)
                        {
                            if (chromeResults.CorrelationDataItems[0].CorrelationSlope < 0) //this is anticorrelated
                            {
                                //anticorrelated means the other is not related
                                Console.WriteLine("#lc notches anti  correlate " + dataToReturn.Primary_Target.ScanLCTarget + "_" + dataToReturn.ParentCharge + " " + -1 * chromeResults.RSquaredValsAverage + Environment.NewLine);
                                dataToReturn.IsAntiCorrelated = true;
                                dataToReturn.IsIntact = true;
                            }
                            else
                            {
                                //this means the lc notches correlate and ar indeed insourse related
                                Console.WriteLine("#lc notches correlate " + dataToReturn.Primary_Target.ScanLCTarget + "_" + dataToReturn.ParentCharge + " " + chromeResults.RSquaredValsAverage + Environment.NewLine);
                                dataToReturn.IsAntiCorrelated = false;
                                dataToReturn.IsIntact = false;
                            }
                        }
                        else
                        {
                            //we have an intact fragment for this potential parrent
                            Console.WriteLine("#lc notches do not correlate " + dataToReturn.Primary_Target.ScanLCTarget + "_" + dataToReturn.ParentCharge + " " + chromeResults.RSquaredValsAverage + Environment.NewLine);
                            dataToReturn.IsAntiCorrelated = false;
                            dataToReturn.IsIntact = true;
                        }

                        #endregion
                    }
                    else
                    {
                        Console.WriteLine("#no parent data so it much be intact " + dataToReturn.Primary_Target.ScanLCTarget + "_" + dataToReturn.ParentCharge + " " + chromeResults.RSquaredValsAverage + Environment.NewLine);
                        dataToReturn.IsAntiCorrelated = false;
                        dataToReturn.IsIntact = true;
                    }

                    #endregion
                }
                else
                {
                    dataToReturn = SetUpObjectHolderResult(dataToReturn, fragmentTarget, parentTarget, scanLCTarget, emptyChromeResults);

                    //different distirbutions because the ranges do not match up
                    Console.WriteLine("#Failed Check, correlator failed.  Could be when the Xaxis does not overlap");
                }

                #endregion
            }
            else
            {
                if (test1 == true || test3 == true || test5 == true || test7 == true)
                {
                    dataToReturn.IsIntact = true; //because the parent failed
                }
            }
            return dataToReturn;
        }

        //private static void SelectXYDataToCorrelate(List<XYData> fragmentEicFitXyData, List<XYData> parentEicFitXyData, int stopScan, int startScan, out List<XYData> possibleFragmentCorrelationEICWindow, out List<XYData> parentCorrelationEICWindow)
        //{
        //    //possibleFragmentCorrelationEICWindow = Utiliites.ReWindowDataListFX(startScan, stopScan, fragmentEicFitXyData, 0);
        //    //parentCorrelationEICWindow = Utiliites.ReWindowDataListFX(startScan, stopScan, parentEicFitXyData, 0);
        //    //TODO 3
        //    possibleFragmentCorrelationEICWindow = ChangeRange.ClipXyDataToScanRange(fragmentEicFitXyData, startScan, stopScan);
        //    parentCorrelationEICWindow = ChangeRange.ClipXyDataToScanRange(parentEicFitXyData, startScan, stopScan);
        //}

        public static List<XYData> SelectXYDataToCorrelate(int startScan, int stopScan, FragmentResultsObjectHolderIq results)
        {
            int numberOfSamples = 100;
            double sampleWidth =  (Convert.ToDouble(stopScan - startScan)) / numberOfSamples;

            //List<XYData> calculatedData = LevenburgMarquardt.ReturnGaussianValues(results.CorrelationCoefficients, numberOfSamples, sampleWidth, results.CorrelationCoefficients[2]);
            List<XYData> calculatedData = null;
            if (results.CorrelationCoefficients != null && results.CorrelationCoefficients.Length==3)
            {
                calculatedData = LevenburgMarquardt.ReturnGaussianValues(results.CorrelationCoefficients, numberOfSamples, startScan, stopScan, results.CorrelationCoefficients[2]);               
            }
            return calculatedData;
        }

        private static List<XYData> NormalizeData(List<XYData> data, double scalar)
        {
            List<XYData> calculatedData = new List<XYData>();
            foreach (var xyData in data)
            {
                calculatedData.Add(new XYData(xyData.X, xyData.Y / scalar));
            }
            return calculatedData;
        }

        public static FragmentResultsObjectHolderIq CorrelateOld(
            IqTarget fragmentTarget, IqTarget targetParent, int scanLCTarget,
            ProcessedPeak fragmentCandiateFitPeak, ProcessedPeak parentCandiateFitPeak,
            List<XYData> fragmentEicFitXyData, List<XYData> parentEicFitXyData, 
            ref ChromatogramCorrelatorBase chromatogramCorrelator, double correlationscorecuttoff, int minimumPointsForOverlap)
        {
            EnumerationError localError = EnumerationError.NoError;
            //check inputs
            bool test0 = Utiliites.SignPostRequire(fragmentTarget.ChargeState == 0, "Charge State Fragment");
            bool test1 = Utiliites.SignPostRequire(targetParent.ChargeState == 0, "Charge State Parent");
            bool test2 = Utiliites.SignPostRequire(fragmentEicFitXyData.Count == 0, "PeakList Fragment");
            bool test3 = Utiliites.SignPostRequire(parentEicFitXyData.Count == 0, "PeakList Parent");
            bool test4 = Utiliites.SignPostRequire(fragmentCandiateFitPeak == null, "Peak Fragment");
            bool test5 = Utiliites.SignPostRequire(parentCandiateFitPeak == null, "Peak Parent");

            if (test0) localError = EnumerationError.FailedChargeState;
            if (test1) localError = EnumerationError.FailedChargeState;
            if (test2) localError = EnumerationError.MissingPoints;
            if (test3) localError = EnumerationError.MissingPoints;
            if (test4) localError = EnumerationError.MissingPeak;
            if (test5) localError = EnumerationError.MissingPeak;

            

            //default result
            ChromCorrelationData emptyChromeResults = new ChromCorrelationData();
            //result = SetUpObjectHolderResult(dataToReturn, fragmentTarget, targetParent, scanLCTarget, emptyChromeResults);
            FragmentResultsObjectHolderIq dataToReturn = SetUpObjectHolderResult(fragmentTarget, targetParent, scanLCTarget, emptyChromeResults);

            if (localError == EnumerationError.NoError)
            {    
                Tuple<string, string> errorlog = new Tuple<string, string>("CorrelateWorkflow", "Success");

                #region 4c, cut to same size and coorelate.  We need both fragment and parent processed at this point

                //find common range between both fit distributions
                //int startScan = 0;
                //int stopScan = 0;
                ScanObject scans = new ScanObject(0,0);

                bool checkScanRange = Utiliites.FindCommonStartStopBetweenCurves(fragmentEicFitXyData, fragmentCandiateFitPeak, parentEicFitXyData, parentCandiateFitPeak, ref scans, ref errorlog);

                if (checkScanRange)
                {
                    #region inside

                    //rewindow to common start and stop

                    //correlate fit data
                    //List<XYData> possibleFragmentCorrelationEICWindow = Utiliites.ReWindowDataListFX(startScan, stopScan, fragmentEicFitXyData, 0);
                    //List<XYData> parentCorrelationEICWindow = Utiliites.ReWindowDataListFX(startScan, stopScan, parentEicFitXyData, 0);
                    //TODO 1
                    List<XYData> possibleFragmentCorrelationEICWindow = ChangeRange.ClipXyDataToScanRange(fragmentEicFitXyData, scans, false);
                    List<XYData> parentCorrelationEICWindow = ChangeRange.ClipXyDataToScanRange(parentEicFitXyData, scans, false);

                    double maxOffragment = possibleFragmentCorrelationEICWindow.Max(x => x.Y);
                    double maxOfParent = parentCorrelationEICWindow.Max(x => x.Y);

                    //normalize?
                    //foreach (var xyData in parentCorrelationEIC)
                    //{
                    //    xyData.Y = xyData.Y * maxOffragment / maxOfParent;
                    //}

                    Console.WriteLine("\t m/z \t Fragment \t Parent");
                    for (int k = 0; k < possibleFragmentCorrelationEICWindow.Count; k++)
                    {
                        string x1 = possibleFragmentCorrelationEICWindow[k].X.ToString(CultureInfo.InvariantCulture);
                        string y1 = possibleFragmentCorrelationEICWindow[k].Y.ToString(CultureInfo.InvariantCulture);
                        string y2 = parentCorrelationEICWindow[k].Y.ToString(CultureInfo.InvariantCulture);
                        Console.WriteLine("Correlate \t" + x1 + "\t" + y1 + "\t" + y2);
                    }

                    //SimplePeakCorrelator correlator = new SimplePeakCorrelator(resultList.Run, _workflowParameters, _workflowParameters.MinRelativeIntensityForChromCorrelator);
                    SimplePeakCorrelator correlator = (SimplePeakCorrelator) chromatogramCorrelator;
                    correlator.BasePeakIso = fragmentTarget.TheorIsotopicProfile; //this is where the possibleFragmentTarget goes
                    correlator.IsoPeaklist = new List<IsotopicProfile>();
                    correlator.IsoPeaklist.Add(targetParent.TheorIsotopicProfile); //simple correlation with largest intensity in candidate Parents

                    //ChromCorrelationData chromeResults = correlator.CorrelateDataXY(possibleFragmentCorrelationEICWindow, parentCorrelationEICWindow, startScan, stopScan);

                    ChromCorrelationData chromeResults = new ChromCorrelationData();
                    if (possibleFragmentCorrelationEICWindow.Count >= minimumPointsForOverlap)
                    {
                        chromeResults = correlator.CorrelateDataXY(possibleFragmentCorrelationEICWindow, parentCorrelationEICWindow, scans.Start, scans.Stop);
                    }
                    else
                    {
                        Console.WriteLine("Too few ponts overlap to run correlation");
                        chromeResults.CorrelationDataItems.Add(new ChromCorrelationDataItem(0,0,-2));
                    }

                    //these results are associated with the parent!!!!!!!!
                    dataToReturn = SetUpObjectHolderResult(dataToReturn, fragmentTarget, targetParent, scanLCTarget, chromeResults);

                    //correlation score
                    if (chromeResults.CorrelationDataItems != null && chromeResults.CorrelationDataItems.Count>0)
                    {
                        dataToReturn.CorrelationScore = Convert.ToDouble(chromeResults.CorrelationDataItems[0].CorrelationRSquaredVal);
                    }

                    //intact assignments
                    if (chromeResults.CorrelationDataItems.Count > 0) //this should be true if the correlation returned data
                    {
                        #region inside

                        //Does the notch correlate at this charge state
                        if (chromeResults.RSquaredValsAverage > correlationscorecuttoff)
                        {
                            if (chromeResults.CorrelationDataItems[0].CorrelationSlope < 0) //this is anticorrelated
                            {
                                //anticorrelated means the other is not related
                                Console.WriteLine("#lc notches anti  correlate " + dataToReturn.Primary_Target.ScanLCTarget + "_" + dataToReturn.ParentCharge + " " + -1 * chromeResults.RSquaredValsAverage + Environment.NewLine);
                                dataToReturn.IsAntiCorrelated = true;
                                dataToReturn.IsIntact = true;
                            }
                            else
                            {
                                //this means the lc notches correlate and ar indeed insourse related
                                Console.WriteLine("#lc notches correlate " + dataToReturn.Primary_Target.ScanLCTarget + "_" + dataToReturn.ParentCharge + " " + chromeResults.RSquaredValsAverage + Environment.NewLine);
                                dataToReturn.IsAntiCorrelated = false;
                                dataToReturn.IsIntact = false;
                            }
                        }
                        else
                        {
                            //we have an intact fragment for this potential parrent
                            Console.WriteLine("#lc notches do not correlate " + dataToReturn.Primary_Target.ScanLCTarget + "_" + dataToReturn.ParentCharge + " " + chromeResults.RSquaredValsAverage + Environment.NewLine);
                            dataToReturn.IsAntiCorrelated = false;
                            dataToReturn.IsIntact = true;
                        }

                        #endregion
                    }
                    else
                    {
                        Console.WriteLine("#no parent data so it much be intact " + dataToReturn.Primary_Target.ScanLCTarget + "_" + dataToReturn.ParentCharge + " " + chromeResults.RSquaredValsAverage + Environment.NewLine);
                        dataToReturn.IsAntiCorrelated = false;
                        dataToReturn.IsIntact = true;
                    }

                    #endregion
                }
                else
                { 
                    dataToReturn = SetUpObjectHolderResult(dataToReturn, fragmentTarget, targetParent, scanLCTarget, emptyChromeResults);

                    //different distirbutions because the ranges do not match up
                    Console.WriteLine("#Failed Check, correlator failed.  Could be when the Xaxis does not overlap");
                }

                #endregion
            }
            return dataToReturn;
        }

        private static FragmentResultsObjectHolderIq SetUpObjectHolderResult(FragmentResultsObjectHolderIq result, IqTarget fragmentTarget, IqTarget targetParent, int scanLCTarget, ChromCorrelationData chromeResults)
        {
            //FragmentResultsObjectHolderIQ dataToReturn = new FragmentResultsObjectHolderIQ((FragmentIQTarget)fragmentTarget);
            FragmentResultsObjectHolderIq dataToReturn = result;
            dataToReturn.ParentCharge = targetParent.ChargeState;
            dataToReturn.FragmentCharge = fragmentTarget.ChargeState;
            //dataToReturn.Scan = scanLCTarget;
            dataToReturn.TargetParent = (FragmentIQTarget) targetParent;
            //dataToReturn.TargetFragment = (FragmentIQTarget) fragmentTarget;
            dataToReturn.CorrelationResults = chromeResults;
            return dataToReturn;
        }

        private static FragmentResultsObjectHolderIq SetUpObjectHolderResult(IqTarget fragmentTarget, IqTarget targetParent, int scanLCTarget, ChromCorrelationData chromeResults)
        {
            FragmentResultsObjectHolderIq dataToReturn = new FragmentResultsObjectHolderIq((FragmentIQTarget)fragmentTarget);
            dataToReturn.ParentCharge = targetParent.ChargeState;
            dataToReturn.FragmentCharge = fragmentTarget.ChargeState;
            //dataToReturn.Scan = scanLCTarget;
            dataToReturn.TargetParent = (FragmentIQTarget)targetParent;
            //dataToReturn.TargetFragment = (FragmentIQTarget) fragmentTarget;
            dataToReturn.CorrelationResults = chromeResults;
            return dataToReturn;
        }
    }
}
