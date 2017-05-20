using System;
using System.Collections.Generic;
using System.Linq;
using IQ.Backend.ProcessingTasks.TargetedFeatureFinders;
using IQ.Workflows.Core;
using IQGlyQ.Enumerations;
using IQGlyQ.Functions;
using IQGlyQ.Objects;
using IQGlyQ.Results;
using Run32.Backend.Core;

namespace IQGlyQ
{
    public static class VerifyByTargetedFeatureFinding
    {
        //public static List<FragmentTarget> Verify(List<FragmentTarget> finalChargedParentsToSearchForMass, double fitScoreCutoff, TargetedResultBase Result, Run run, ref MSGenerator _msGenerator, ref IterativeTFF _msfeatureFinder, ref List<FragmentTarget> _futureTargets, ref IsotopicProfileFitScoreCalculator fitScoreCalculator, ref Tuple<string, string> errorLog)
        //{
        //    int scanAssociatedWithXYData;

        //    List<FragmentTarget> finalChargedParentsToSearchForLC = new List<FragmentTarget>();

        //    foreach (FragmentTarget fragmentTarget in finalChargedParentsToSearchForMass)
        //    {
        //        //check to see if they are correct mass within the scan window

        //        ScanSetCollection oldScanset = run.ScanSetCollection;
        //        int scansToSum = 5; //perhapps 5 to decrease noise.  summing is required for orbitrap data
        //        int startScan = fragmentTarget.StartScan;
        //        int stopScan = fragmentTarget.StopScan;
        //        //4-22-2013
        //        //run.ScanSetCollection.Create(run, startScan, stopScan, scansToSum, 1, false);
        //        double middleScansetIndex = run.ScanSetCollection.ScanSetList.Count / 2.0;
        //        int scansetIndex = Convert.ToInt32(Math.Truncate(middleScansetIndex));
        //        Result.Run.CurrentScanSet = run.ScanSetCollection.ScanSetList[scansetIndex];
        //        _msGenerator.Execute(run.ResultCollection);
        //        scanAssociatedWithXYData = run.ScanSetCollection.ScanSetList[scansetIndex].PrimaryScanNumber;

        //        IsotopicProfile iso = new IsotopicProfile();
        //        Console.WriteLine("Look for parent in PQ Scan " + scanAssociatedWithXYData);

        //        iso = _msfeatureFinder.IterativelyFindMSFeature(run.XYData, fragmentTarget.IsotopicProfile);

        //        //we need to look over several charge states.  does this bring back any positives like EVE
        //        if (iso != null)
        //        {
        //            Console.WriteLine("found? " + fragmentTarget.MZ.ToString());
        //            //set aside for furture targets

        //            //MassTagFitScoreCalculator calculator = new MassTagFitScoreCalculator();
        //            //fit score filter?
        //            IsotopicProfile theorProfile = fragmentTarget.IsotopicProfile;

        //            double fitscore = fitScoreCalculator.CalculateFitScore(theorProfile, iso, run.XYData);
        //            if (fitscore < fitScoreCutoff)
        //            {
        //                _futureTargets.Add(fragmentTarget);
        //                finalChargedParentsToSearchForLC.Add(fragmentTarget); //we still have to check LC. Pasing LC means it is insource
        //            }
        //            else
        //            {
        //                Console.WriteLine("Boot from candidate list based on fit score: " + fitscore);
        //                iso = null;
        //            }
        //        }
        //        else
        //        {
        //            Console.WriteLine("not found " + fragmentTarget.MZ.ToString());
        //            //potential intact
        //        }
        //    }
        //    return finalChargedParentsToSearchForLC;
        //}

        //public static List<FragmentIQTarget> VerifyIQ(List<FragmentIQTarget> finalChargedParentsToSearchForMass, double fitScoreCutoff, IqResult result, Run run, ref MSGenerator _msGenerator, ref IterativeTFF _msfeatureFinder, ref List<FragmentIQTarget> _futureTargets, ref IsotopicProfileFitScoreCalculator fitScoreCalculator, ref Tuple<string, string> errorLog)
        //public static FragmentIQTarget VerifyIQ(FragmentIQTarget targetIn, double fitScoreCutoff, Run run, out List<FragmentIQTarget> failedTargets, ref MSGenerator _msGenerator, ref IterativeTFF _msfeatureFinder, ref List<FragmentIQTarget> _futureTargets, ref IsotopicProfileFitScoreCalculator fitScoreCalculator, ref Tuple<string, string> errorLog, out EnumerationError error)
        
        //public static List<IqGlyQResult> VerifyIq(FragmentIQTarget targetIn, double fitScoreCutoff, Run run, out List<FragmentIQTarget> failedTargets, ref MSGenerator _msGenerator, ref IterativeTFF _msfeatureFinder, ref List<FragmentIQTarget> _futureTargets, ref Task _fitScoreCalculator, ref Tuple<string, string> errorLog, out EnumerationError error, bool calibrateSpectra = false, double daltonOffset = 0)
        public static List<IqGlyQResult> VerifyIq(FragmentIQTarget targetIn, double fitScoreCutoff, Run run, out List<FragmentIQTarget> failedTargets, ref Processors.ProcessorMassSpectra msProcessor, ref IterativeTFF _msfeatureFinder, ref List<FragmentIQTarget> _futureTargets, ref Tuple<string, string> errorLog, out EnumerationError error, IsotopeParameters isoParameters, double ppmCuttoff, bool calibrateSpectra = false, double daltonOffset = 0)
        {
            //IsotopicPeakFitScoreCalculator fitScoreCalculator = (IsotopicPeakFitScoreCalculator)_fitScoreCalculator;
            
            int scanAssociatedWithXYData;
            error = EnumerationError.NoError;

            FragmentIQTarget targetToFind = targetIn;
            failedTargets = new List<FragmentIQTarget>();
            List<IqGlyQResult> childrenResults = new List<IqGlyQResult>();

            List<IqTarget> children = targetToFind.ChildTargets().ToList();
            foreach (FragmentIQTarget fragmentTarget in children)
            {
                IqResult resultVerify = new IqResult(fragmentTarget);
                
                IsotopicProfile theoreticalProfile = resultVerify.Target.TheorIsotopicProfile;
                //check to see if they are correct mass within the scan window
                int scansetIndex = 0;

                IqGlyQResult box = new IqGlyQResult(fragmentTarget);
                box.AddResult(resultVerify);
                

                bool old = false;
                if (old)
                {
                    ScanSetCollection oldScanset = run.ScanSetCollection;
                    int scansToSum = 5; //perhapps 5 to decrease noise.  summing is required for orbitrap data
                    int startScan = fragmentTarget.ScanInfo.Start;
                    int stopScan = fragmentTarget.ScanInfo.Stop;
                    //4-22-2013
                    //run.ScanSetCollection.Create(run, startScan, stopScan, scansToSum, 1, false);
                    double middleScansetIndex = run.ScanSetCollection.ScanSetList.Count/2.0;
                    scansetIndex = Convert.ToInt32(Math.Truncate(middleScansetIndex));
                    run.CurrentScanSet = run.ScanSetCollection.ScanSetList[scansetIndex];
                    //_msGenerator.Execute(run.ResultCollection);
                    run.ResultCollection.Run.XYData = msProcessor.DeconMSGeneratorWrapper(run, run.CurrentScanSet);
                    scanAssociatedWithXYData = run.ScanSetCollection.ScanSetList[scansetIndex].PrimaryScanNumber;

                    //IsotopicProfile iso = new IsotopicProfile();
                    Console.WriteLine("Look for parent in PQ Scan " + scanAssociatedWithXYData);

                    //resultVerify.IqResultDetail.MassSpectrum = _msGenerator.GenerateMS(run, run.ScanSetCollection.ScanSetList[scansetIndex]);
                    resultVerify.IqResultDetail.MassSpectrum = msProcessor.DeconMSGeneratorWrapper(run, run.ScanSetCollection.ScanSetList[scansetIndex]);
                }
                else
                {
                    run.CurrentScanSet = (from scanSet in run.ScanSetCollection.ScanSetList where scanSet.PrimaryScanNumber >= fragmentTarget.ScanLCTarget select scanSet).FirstOrDefault();
                    Console.WriteLine("Look for parent in PQ Scan " + run.CurrentScanSet.PrimaryScanNumber);
                    //resultVerify.IqResultDetail.MassSpectrum = _msGenerator.GenerateMS(run, run.CurrentScanSet);
                    resultVerify.IqResultDetail.MassSpectrum = msProcessor.DeconMSGeneratorWrapper(run, run.CurrentScanSet);
                }

                //iso = _msfeatureFinder.IterativelyFindMSFeature(run.XYData, fragmentTarget.IsotopicProfile);

                //clip and calibrate ms


                if (calibrateSpectra)
                {
                    Run32.Backend.Data.XYData calibratedMassSpectra = Processors.ProcessorMassSpectra.ReCalibrateMassSpectraForIFF(theoreticalProfile, resultVerify.IqResultDetail.MassSpectrum, daltonOffset);
                    resultVerify.ObservedIsotopicProfile = IterativelyFindMSFeatureWrapper.IterativeFeatureFind(theoreticalProfile, calibratedMassSpectra, _msfeatureFinder);
                    //resultVerify.ObservedIsotopicProfile = _msfeatureFinder.IterativelyFindMSFeature(calibratedMassSpectra, theoreticalProfile);
                }
                else
                {
                    resultVerify.ObservedIsotopicProfile = IterativelyFindMSFeatureWrapper.IterativeFeatureFind(theoreticalProfile, resultVerify.IqResultDetail.MassSpectrum, _msfeatureFinder);
                    //resultVerify.ObservedIsotopicProfile = _msfeatureFinder.IterativelyFindMSFeature(resultVerify.IqResultDetail.MassSpectrum, theoreticalProfile);
                }
                
                //truncate theoretical peak list before scoring

                //we need to look over several charge states.  does this bring back any positives like EVE
                if (resultVerify.ObservedIsotopicProfile != null)
                {
                    Console.WriteLine("found? " + fragmentTarget.MZTheor.ToString());
                    //set aside for furture targets

                    //float maxObserved = resultVerify.ObservedIsotopicProfile.Peaklist.Max(p => p.Height);
                    //float mintheory = theoreticalProfile.Peaklist.Min(p => p.Height);

                    //if we are dh we need to send in a special target that uses the hybeid profile
                    //otherwise send in fragmentTarget

                    ProcessTarget.PreProcessForFitScore(ref resultVerify, fragmentTarget, fitScoreCutoff, ppmCuttoff, msProcessor, isoParameters);

                    //iQresult.ObservedIsotopicProfile.Score = fitScoreCalculator.CalculateFitScore(target.TheorIsotopicProfile, iQresult.ObservedIsotopicProfile, iQresult.IqResultDetail.MassSpectrum);

                   
                        //resultVerify.FitScore = fitScoreCalculator.CalculateFitScore(theoreticalProfile, resultVerify.ObservedIsotopicProfile, resultVerify.IqResultDetail.MassSpectrum);//standard

                        //int peaksToLeft = fitScoreCalculator.NumberOfPeaksToLeftForPenalty;
                        //int peaksToLeft = fitScoreParameters.NumberOfPeaksToLeftForPenalty;

                        //resultVerify.FitScore = fitScoreCalculator.CalculateFitScore(theoreticalProfile, resultVerify.ObservedIsotopicProfile, resultVerify.IqResultDetail.MassSpectrum, peaksToLeft);
           //             resultVerify.FitScore = msProcessor.ExecuteFitScore(theoreticalProfile,resultVerify.ObservedIsotopicProfile);
                    


                    resultVerify.IqResultDetail.MassSpectrum = null;

                    if (resultVerify.FitScore < fitScoreCutoff)
                    {
                        //TODO this means the isotope profile and fit score are acceptable suggesting that this child charged target is a parent and a good candidate for next run
                        _futureTargets.Add(fragmentTarget);
                        box.DidThisWork = true;
                        childrenResults.Add(box);
                        //finalChargedParentsToSearchForLC.Add(fragmentTarget); //we still have to check LC. Pasing LC means it is insource
                        //it is allready added
                        //finalChargedParentsToSearchForMass.AddTarget(fragmentTarget); //we still have to check LC. Pasing LC means it is insource
                    }
                    else
                    {
                        //TODO this means the fit score is no good and thus this child target (parent) was not found.  If there is no parent, we can't prove it is a fragment
                        error = EnumerationError.FailedFeatureFinder;
                        //fragmentTarget.Error = ErrorEnumeration.FailedFeatureFinder;
                        failedTargets.Add(fragmentTarget);
                        Console.WriteLine("Boot from candidate list based on fit score: " + resultVerify.FitScore);
                        //resultVerify.ObservedIsotopicProfile = null;
                        targetToFind.RemoveTarget(fragmentTarget); //we still have to check LC. Pasing LC means it is insource
                        box.DidThisWork = false;
                        childrenResults.Add(box);

                    }
                }
                else
                {
                    //No isotpe profile was found using the IterativelyFindMSFeature
                    Console.WriteLine("No isotpe profile was found using the IterativelyFindMSFeature " + fragmentTarget.MZTheor.ToString());
                    //potential intact

                }
            }
            //return targetToFind;
            return childrenResults;
        }

        

        private static void MaxMinValueInObservedProfile(IqResult resultVerify, out double minValueInObservedProfile, out double maxValueInObservedProfile)
        {
            minValueInObservedProfile = resultVerify.ObservedIsotopicProfile.Peaklist[0].Height;
            foreach (MSPeak peak in resultVerify.ObservedIsotopicProfile.Peaklist)
            {
                if (peak.Height < minValueInObservedProfile)
                {
                    minValueInObservedProfile = peak.Height;
                }
            }

            maxValueInObservedProfile = resultVerify.ObservedIsotopicProfile.Peaklist[0].Height;
            foreach (MSPeak peak in resultVerify.ObservedIsotopicProfile.Peaklist)
            {
                if (peak.Height > maxValueInObservedProfile)
                {
                    maxValueInObservedProfile = peak.Height;
                }
            }
        }
    }
}
