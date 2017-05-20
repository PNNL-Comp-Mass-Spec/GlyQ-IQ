using System;
using System.Collections.Generic;
using System.Linq;
//using GetPeaks_DLL.Functions;
using GetPeaksDllLite.Functions;
using IQ.Backend.ProcessingTasks.TheorFeatureGenerator;
using IQ.Workflows.Core;
using IQ.Workflows.Utilities;
using IQGlyQ.Enumerations;
using IQGlyQ.Objects;
using IQGlyQ.Objects.EverythingIsotope;
using IQGlyQ.Results;
using Run32.Backend;
using Run32.Backend.Core;
using Run32.Utilities;

namespace IQGlyQ.TargetGenerators
{
    public static class GenerateListOfCandidateParents
    {
        //public static List<FragmentIQTarget> GenerateIQ(
        public static FragmentIQTarget GenerateIQ(
            Run run, 
            IqResult result,
            ChromPeakQualityData possibiity,
            ScanObject scans,
            EnumerationParentOrChild parentsOrChildren,
            out List<FragmentIQTarget> failedTargets,
            out List<FragmentResultsObjectHolderIq> resultsList,
            ref FragmentedTargetedWorkflowParametersIQ _workflowParameters,
            //ref ITheorFeatureGenerator _theorFeatureGen,
            ref IGenerateIsotopeProfile theorFeatureGenV2,
            ref Tuple<string, string> errorLog,
            ref Processors.ProcessorChromatogram _lcProcessor,
            ref TheoreticalIsotopicProfileWrapper monster
        )
        {
            //int scanStart = scans.Start;
            //int scanStop = scans.Stop;


            bool print = true;

            double MassProton = Globals.PROTON_MASS;

            //StepA 0  this is the parent of all the children (fragments x charges)

            //FragmentIQTarget ParentTarget = new FragmentIQTarget(result.Target,true);

            IqTargetUtilities util = new IqTargetUtilities();
            //IqTarget parentTargetcopy = util.DeepClone(result.Target);
            //IqTarget parentTargetcopy = util.DeepClone(result.Target);
            IqTarget parentTargetcopy = util.Clone(result.Target);
            FragmentIQTarget parentTarget = new FragmentIQTarget(parentTargetcopy);

            

            //update established in copy
            float existingMixingFraction = result.Target.TheorIsotopicProfile.EstablishedMixingFraction;
            bool localEstablishCheck = result.Target.TheorIsotopicProfile.isEstablished;
            if(localEstablishCheck)
            {
                parentTargetcopy.TheorIsotopicProfile.EstablishAlternatvePeakIntensites(existingMixingFraction);
                parentTarget.TheorIsotopicProfile.EstablishAlternatvePeakIntensites(existingMixingFraction);
            }

            failedTargets = new List<FragmentIQTarget>();

            List<FragmentIQTarget> finalChargedParentsToSearchFor = new List<FragmentIQTarget>();
            resultsList = new List<FragmentResultsObjectHolderIq>();//this includes all results (failed or passed)
            //List<FragmentIQTarget> ChargedParentsToSearchFor = new List<FragmentIQTarget>();

            //for each difference
            foreach (FragmentIQTarget fragment in _workflowParameters.FragmentsIq)
            {
                #region inside


                //double MonoDifference = fragment.MonoMassTheor;

                //StepA 1 make new target that includes the difference
                


                FragmentIQTarget targetPlusDifferenceNoCharge = new FragmentIQTarget(parentTarget);//set details?
                //if (localEstablishCheck)
                //{
                //    targetPlusDifferenceNoCharge.TheorIsotopicProfile.EstablishAlternatvePeakIntensites(existingMixingFraction);
                //}

                //apply properties for larger mass
                targetPlusDifferenceNoCharge.ScanLCTarget = possibiity.ScanLc;
                targetPlusDifferenceNoCharge.DifferenceName = fragment.DifferenceName;


                //targetPlusDifferenceNoCharge.EmpiricalFormula = result.Target.EmpiricalFormula; //
                //targetPlusDifferenceNoCharge.EmpiricalFormula = EmpiricalFormulaUtilities.AddFormula(targetPlusDifferenceNoCharge.EmpiricalFormula, fragment.EmpiricalFormula);

                string newformula = result.Target.EmpiricalFormula;

                switch (parentsOrChildren)
                {
                    case EnumerationParentOrChild.ParentsOnly:
                        {
                            newformula = EmpiricalFormulaUtilities.AddFormula(result.Target.EmpiricalFormula, fragment.EmpiricalFormula);
                            targetPlusDifferenceNoCharge.MonoMassTheor = result.Target.MonoMassTheor + fragment.MonoMassTheor;
                            
                        }
                        break;
                    case EnumerationParentOrChild.ChildrenOnly:
                        {
                            newformula = EmpiricalFormulaUtilities.SubtractFormula(result.Target.EmpiricalFormula, fragment.EmpiricalFormula);
                            targetPlusDifferenceNoCharge.MonoMassTheor = result.Target.MonoMassTheor - fragment.MonoMassTheor;
                        }
                        break;
                    default:
                        {
                            newformula = EmpiricalFormulaUtilities.AddFormula(result.Target.EmpiricalFormula, fragment.EmpiricalFormula);
                        }
                        break;
                }
                
                targetPlusDifferenceNoCharge.EmpiricalFormula = newformula;
                targetPlusDifferenceNoCharge.MZTheor = ConvertMonoToMz.Execute(targetPlusDifferenceNoCharge.MonoMassTheor, targetPlusDifferenceNoCharge.ChargeState, MassProton);

                //parent charge iso profile to be feed into the charge state targets
                //targetPlusDifferenceNoCharge.TheorIsotopicProfile = _theorFeatureGen.GenerateTheorProfile(targetPlusDifferenceNoCharge.EmpiricalFormula, targetPlusDifferenceNoCharge.ChargeState);

                //targetPlusDifferenceNoCharge.TheorIsotopicProfile = TheoreticalIsotopicProfileWrapper.LowLevel(ref _theorFeatureGen, targetPlusDifferenceNoCharge.EmpiricalFormula, targetPlusDifferenceNoCharge.ChargeState);

                //double deltaMassCalibrationMz = _workflowParameters.MSParameters.IsoParameters.DeltaMassCalibrationMZ;
                //double deltaMassCalibrationMono = _workflowParameters.MSParameters.IsoParameters.DeltaMassCalibrationMono;
                //EnumerationIsotopePenaltyMode penaltyMode = _workflowParameters.MSParameters.IsoParameters.PenaltyMode;
                //TheoreticalIsotopicProfileWrapper.Generate(ref _theorFeatureGen, targetPlusDifferenceNoCharge, _workflowParameters.MSParameters.IsoParameters.IsotopeProfileMode, deltaMassCalibrationMz, deltaMassCalibrationMono, toMassCalibrate, penaltyMode);
                //TheoreticalIsotopicProfileWrapper.Generate(ref theorFeatureGenV2, targetPlusDifferenceNoCharge, _workflowParameters.MSParameters.IsoParameters.IsotopeProfileMode, deltaMassCalibrationMz, deltaMassCalibrationMono, toMassCalibrate, penaltyMode);
                

                bool toMassCalibrate = _workflowParameters.MSParameters.IsoParameters.ToMassCalibrate;
                bool toShift = _workflowParameters.MSParameters.IsoParameters.ToShift;
                EnumerationIsotopicProfileMode isotopeProfileMode = _workflowParameters.MSParameters.IsoParameters.IsotopeProfileMode;

                if (localEstablishCheck)
                {
                    monster.Generate(ref theorFeatureGenV2, targetPlusDifferenceNoCharge, isotopeProfileMode, toMassCalibrate, toShift, existingMixingFraction);
                    targetPlusDifferenceNoCharge.TheorIsotopicProfile.EstablishAlternatvePeakIntensites(existingMixingFraction);
                }
                else
                {
                    monster.Generate(ref theorFeatureGenV2, targetPlusDifferenceNoCharge, isotopeProfileMode, toMassCalibrate, toShift);
                }

                //double testMonoMass = EmpiricalFormulaUtilities.GetMonoisotopicMassFromEmpiricalFormula(newformula);
                //double difference = targetPlusDifferenceNoCharge.MonoMassTheor - testMonoMass;

                //if (difference > 0.01)
                //{
                //    Console.WriteLine("Either the mass or the empericalformula is wrong");
                //}


                //StepA 2 calculate charged targets and add them to the parent
                IqTargetUtilities IqUtilities = new IqTargetUtilities();
                List<IqTarget> chargedTargets = IqUtilities.CreateChargeStateTargets(targetPlusDifferenceNoCharge,400,2500);
                foreach (IqTarget chargedTarget in chargedTargets)//this is not populated properly above
                {
                    //chargedTarget.TheorIsotopicProfile.MostAbundantIsotopeMass = chargedTarget.TheorIsotopicProfile.GetMZofMostAbundantPeak();
                    chargedTarget.TheorIsotopicProfile.MostAbundantIsotopeMass = Utiliites.GetMassToExtractFromIsotopeProfile(chargedTarget.TheorIsotopicProfile,_workflowParameters);
                }


                //check children of the charge before adding  <--- this is important since it easier to not add now rather than remove later
                foreach (IqTarget chargedTarget in chargedTargets)
                {


                    //calibrate targets.  no.  allready done in targetPlusDifferenceNoCharge
                    
                    


                    FragmentIQTarget chargedTargetCandidate = new FragmentIQTarget(chargedTarget);//set details?
                    Int32 charge = chargedTargetCandidate.ChargeState;
                    chargedTargetCandidate.DifferenceName = fragment.DifferenceName;
                    chargedTargetCandidate.DifferenceID = fragment.DifferenceID;
                    chargedTargetCandidate.RefID = fragment.RefID;

                    //int scanStartPossibleParent = Convert.ToInt32(possibiity.ScanLc - possibiity.Peak.Width / 2);//a nice wide range
                    //int scanStopPossibleParent = Convert.ToInt32(possibiity.ScanLc + possibiity.Peak.Width / 2);

                    //chargedTargetCandidate.ScanInfo = new ScanObject(0, 0, scans.Min, scans.Max, scans.Buffer, scans.ScansToSum);//scan details for parent
                    //chargedTargetCandidate.ScanInfo = new ScanObject(scanStartPossibleParent, scanStopPossibleParent, scans.Min, scans.Max, scans.Buffer, scans.ScansToSum);//scan details for parent
                    chargedTargetCandidate.ScanInfo = new ScanObject(scans);//a copy of the peak quality data
                    //start and stop are calculated and set below

                    chargedTargetCandidate.ScanLCTarget = targetPlusDifferenceNoCharge.ScanLCTarget;
                    //chargedTargetCandidate.IsotopicProfile = targetPlusDifferenceNoCharge.IsotopicProfile;
                    chargedTargetCandidate.TheorIsotopicProfile = targetPlusDifferenceNoCharge.TheorIsotopicProfile;
                    chargedTargetCandidate.MZTheor = ConvertMonoToMz.Execute(chargedTargetCandidate.MonoMassTheor, charge, MassProton);

                    FragmentResultsObjectHolderIq theoreticalchargedResult = new FragmentResultsObjectHolderIq(chargedTargetCandidate);
                    resultsList.Add(theoreticalchargedResult);
                    //FragmentTarget chargedTarget = new FragmentTarget(targetPlusDifferenceNoCharge);
                    //FragmentIQTarget chargedTargetCandidate = new FragmentIQTarget(chargedTarget);

                    //bring in pertinant FragmentIQTargetInfo
                    

                    //chargedTarget.ChargeState = charge;
                    //chargedTarget.MZTheor = ConvertMonoToMz.Execute(chargedTarget.MonoMassTheor, chargedTarget.ChargeState, MassProton);
                    //TODO this may not be needed since it is now in CreateChargeStateTargets
                    //chargedTargetCandidate.TheorIsotopicProfile = _theorFeatureGen.GenerateTheorProfile(chargedTarget.EmpiricalFormula, charge);

                    if (print) Console.WriteLine("Loading " + chargedTargetCandidate.MZTheor + " at charge " + chargedTargetCandidate.ChargeState + " scan " + chargedTargetCandidate.ScanLCTarget);




                    CorrelationObject loadchargedEIC = new CorrelationObject(chargedTarget.TheorIsotopicProfile, chargedTargetCandidate.ScanInfo, run, ref _lcProcessor, _workflowParameters);

                    if (loadchargedEIC.AreChromDataOK)
                    {
                        #region inside
                        List<PNNLOmics.Data.XYData> chargedTargetEic = Utiliites.PullBestEIC(loadchargedEIC, ref errorLog);

                        if (chargedTargetEic != null)
                        {
                            double maxValue = chargedTargetEic.Max(x => x.Y); //we may not need this
                            if (print) Console.WriteLine(maxValue);
                        }
                        //WriteXYData(chargedTargetEic);

                        

                        //find peak and set scan range
                        int newScanStart;
                        int newScanStop;
                        //Utiliites.SetNewScanStartStopOmicsIQ(chargedTargetEic, chargedTargetCandidate.ScanInfo, chargedTargetCandidate, out newScanStart, out newScanStop, print, ref errorLog);
                        ScanObject scanObject = chargedTargetCandidate.ScanInfo;
                        Utiliites.SetNewScanStartStopOmicsIQ(chargedTargetEic, ref scanObject, chargedTargetCandidate.ScanLCTarget,ref _lcProcessor, print, ref errorLog);

                        //chargedTargetCandidate.ScanInfo.Start = newScanStart;
                        //chargedTargetCandidate.ScanInfo.Stop = newScanStop;
                        chargedTargetCandidate.ScanInfo = scanObject;
                        

                        //validate each candidate
                        bool checkScanRange = chargedTargetCandidate.ScanInfo.Start > chargedTargetCandidate.ScanInfo.Min && chargedTargetCandidate.ScanInfo.Start < chargedTargetCandidate.ScanLCTarget && chargedTargetCandidate.ScanInfo.Stop > chargedTargetCandidate.ScanLCTarget && chargedTargetCandidate.ScanInfo.Stop < chargedTargetCandidate.ScanInfo.Max;
                        bool checkIso = chargedTargetCandidate.TheorIsotopicProfile != null;
                        bool checkChargeState = chargedTargetCandidate.ChargeState > 0;
                        bool checkMZ = chargedTargetCandidate.MZTheor > 0;
                        bool checkMono = chargedTargetCandidate.MonoMassTheor > 0;

                        if (checkScanRange && checkChargeState && checkIso && checkChargeState && checkMZ && checkMono)
                        {
                            if (print) Console.WriteLine("+++Pass Test");
                            //Finally Add to good List
                            targetPlusDifferenceNoCharge.AddTarget(chargedTargetCandidate);
                        }
                        else
                        {
                            //if (checkScanRange==false) chargedTargetCandidate.Error = ErrorEnumeration.FailedScanRange;
                            //if (checkIso == false) chargedTargetCandidate.Error = ErrorEnumeration.FailedIsotope;
                            //if (checkChargeState == false) chargedTargetCandidate.Error = ErrorEnumeration.FailedChargeState;
                            //if (checkMZ == false) chargedTargetCandidate.Error = ErrorEnumeration.FailedMass;
                            //if (checkMono == false) chargedTargetCandidate.Error = ErrorEnumeration.FailedMass;

                            if (checkScanRange == false) theoreticalchargedResult.Error = EnumerationError.FailedScanRange;
                            if (checkIso == false) theoreticalchargedResult.Error = EnumerationError.FailedIsotope;
                            if (checkChargeState == false) theoreticalchargedResult.Error = EnumerationError.FailedChargeState;
                            if (checkMZ == false) theoreticalchargedResult.Error = EnumerationError.FailedMass;
                            if (checkMono == false) theoreticalchargedResult.Error = EnumerationError.FailedMass;

                            failedTargets.Add(chargedTargetCandidate);
                            if (print) Console.WriteLine("---Fail Test");
                            if (print) Console.WriteLine("Check Scan Range: " + checkScanRange + "Iso: " + checkIso + "Charge State: " + checkChargeState + "MZ: " + checkMZ + "Mono: " + checkMono);
                            //the peak does not make it because it is missing information rather than statistial reason
                        }
                        if (print) Console.WriteLine("Parent " + chargedTargetCandidate.MZTheor + " at charge " + chargedTargetCandidate.ChargeState + " scan " + chargedTargetCandidate.ScanLCTarget + ": " + chargedTargetCandidate.ScanInfo.Start + "-" + chargedTargetCandidate.ScanInfo.Stop);
                        if (print) Console.WriteLine("Next Peak Quality, we have " + finalChargedParentsToSearchFor.Count + " candidate targets");
                        #endregion
                    }
                    else
                    {
                        theoreticalchargedResult.Error = EnumerationError.FailedEIC;
                        failedTargets.Add(chargedTargetCandidate);
                    }
                }

                Console.WriteLine(chargedTargets.Count);
                Console.WriteLine("Did Targets Pass " + targetPlusDifferenceNoCharge.HasChildren());
                #endregion

                if (targetPlusDifferenceNoCharge.HasChildren())
                {
                    List<IqTarget> children = targetPlusDifferenceNoCharge.ChildTargets().ToList();
                    foreach (IqTarget target in children)
                    {
                        parentTarget.AddTarget(target);
                    }
                }
                
            } //next fragment difference

            if (print) Console.WriteLine(Environment.NewLine);

            return parentTarget;
        }

        //private static void CheckEIC(Run run, ScanObject scans, FragmentedTargetedWorkflowParametersIQ _workflowParameters, ITheorFeatureGenerator _theorFeatureGen, ref Tuple<string, string> errorLog, List<IqTarget> children, FragmentIQTarget targetPlusDifferenceNoCharge, double MassProton, List<FragmentIQTarget> ChargedParentsToSearchFor, bool print, Processors.ProcessorChromatogram LcProcessor)
        //{
        //    int scanStart = scans.Start;
        //    int scanStop = scans.Stop;
            
        //    foreach (IqTarget child in children)
        //        //foreach (Int16 charge in targetPlusDifferenceNoCharge.ChargeStateTargets)
        //    {
        //        Int32 charge = child.ChargeState;
        //        //FragmentTarget chargedTarget = new FragmentTarget(targetPlusDifferenceNoCharge);
        //        FragmentIQTarget chargedTarget = new FragmentIQTarget(targetPlusDifferenceNoCharge);
        //        chargedTarget.ChargeState = charge;
        //        chargedTarget.MZTheor = ConvertMonoToMz.Execute(chargedTarget.MonoMassTheor, chargedTarget.ChargeState, MassProton);


        //        //_theorFeatureGen.GenerateTheorFeature(chargedTarget);
        //        //chargedTarget.IsotopicProfile = _theorFeatureGen.GenerateTheorProfile(chargedTarget.EmpiricalFormula, charge);
        //        //chargedTarget.TheorIsotopicProfile = _theorFeatureGen.GenerateTheorProfile(chargedTarget.EmpiricalFormula, charge);
        //        chargedTarget.TheorIsotopicProfile = TheoreticalIsotopicProfileWrapper.LowLevel(ref _theorFeatureGen, chargedTarget.EmpiricalFormula, charge);
        //        ChargedParentsToSearchFor.Add(chargedTarget);

        //        if (print) Console.WriteLine("Loading " + chargedTarget.MZTheor + " at charge " + chargedTarget.ChargeState + " scan " + chargedTarget.ScanLCTarget);


        //        CorrelationObject loadchargedTargetPeakFragment = new CorrelationObject(chargedTarget.TheorIsotopicProfile, scans, run, ref LcProcessor, _workflowParameters);
        //        //CorrelationObject loadchargedTargetPeakFragment = new CorrelationObject(chargedTarget.TheorIsotopicProfile, scanStart, scanStop, _workflowParameters.ChromGenTolerance, _workflowParameters.MinRelativeIntensityForChromCorrelator, run, ref _peakChromGen, ref _smoother);

        //        int newScanStart;
        //        int newScanStop;

        //        if (loadchargedTargetPeakFragment.AreChromDataOK)
        //        {
        //            List<PNNLOmics.Data.XYData> chargedTargetEic = Utiliites.PullBestEIC(loadchargedTargetPeakFragment, ref errorLog);

        //            if (chargedTargetEic != null)
        //            {
        //                double maxValue = chargedTargetEic.Max(x => x.Y); //we may not need this
        //                if (print) Console.WriteLine(maxValue);
        //            }
        //            //WriteXYData(chargedTargetEic);

        //            //TODO this is probably broken
        //            //Utiliites.SetNewScanStartStopOmicsIQ(chargedTargetEic, scans, chargedTarget, out newScanStart, out newScanStop, print, ref errorLog);
        //            Utiliites.SetNewScanStartStopOmicsIQ(chargedTargetEic, ref scans, chargedTarget.ScanLCTarget, ref LcProcessor, print, ref errorLog);
        //        }
        //        else
        //        {
        //            //Console.WriteLine("the old scan Range is " + scanStartPossibiilty + "-" + scanStopPossibiilty);
        //            if (print) Console.WriteLine("FAIL");
        //            newScanStart = scanStart;
        //            newScanStop = scanStop;
        //        }

        //        //chargedTarget.ScanInfo.Start = newScanStart;
        //        //chargedTarget.ScanInfo.Stop = newScanStop;
        //        chargedTarget.ScanInfo = scans;
                
        //    } //next charge
        //}

        #region old iQ
        //public static List<FragmentTarget> Generate(
        //    Run run, TargetedResultBase result,
        //    ChromPeakQualityData possibiity,
        //    int scanStartPossibiilty,
        //    int scanStopPossibiilty,
        //    ref List<FragmentTarget> Fragments,
        //    ref FragmentedTargetedWorkflowParameters _workflowParameters,
        //    ref JoshTheorFeatureGenerator _theorFeatureGen,
        //    ref PeakChromatogramGenerator _peakChromGen,
        //    ref SavitzkyGolaySmoother _smoother,
        //    ref Tuple<string, string> errorLog)
        //{

        //    bool print = false;

        //    double MassProton = DeconTools.Backend.Globals.PROTON_MASS;

        //    List<FragmentTarget> finalChargedParentsToSearchFor = new List<FragmentTarget>();
        //    List<FragmentTarget> ChargedParentsToSearchFor = new List<FragmentTarget>();

        //    //for each difference
        //    foreach (FragmentTarget fragment in Fragments)
        //    {
        //        #region inside

        //        double MonoDifference = fragment.MonoIsotopicMass;

        //        //step 2 get theoretical isootpe profile
        //        FragmentTarget targetPlusDifferenceNoCharge = new FragmentTarget(result.Target);
        //        targetPlusDifferenceNoCharge.ScanLCTarget = possibiity.ScanLc;

        //        targetPlusDifferenceNoCharge.DifferenceName = fragment.DifferenceName;
        //        targetPlusDifferenceNoCharge.EmpiricalFormula = result.Target.EmpiricalFormula; //TODO +add new elements
        //        string newformula = EmpiricalFormulaUtilities.AddFormula(result.Target.EmpiricalFormula, fragment.EmpiricalFormula);
        //        targetPlusDifferenceNoCharge.EmpiricalFormula = newformula;
        //        targetPlusDifferenceNoCharge.MonoIsotopicMass = result.Target.MonoIsotopicMass + fragment.MonoIsotopicMass;


        //        //look one charge greater and one charge smaller for difference ion.  family should be similiary charged
        //        //targetPlusDifference.ChargeStateTargets = new List<int> { 1, 2, 3, 4 };//perhaps look one greater and one smaller than result

        //        Utiliites.DeviseChargeStates(possibiity, targetPlusDifferenceNoCharge, ref errorLog);

        //        int chargeStateSuccessCounter = 0; //we need to turn up null across all charge states
        //        //in this loop, we scan across each charge state because the parent may be a different charge state than the fragement
        //        foreach (Int16 charge in targetPlusDifferenceNoCharge.ChargeStateTargets)
        //        {
        //            FragmentTarget chargedTarget = new FragmentTarget(targetPlusDifferenceNoCharge);
        //            chargedTarget.ChargeState = charge;
        //            chargedTarget.MZ = ConvertMonoToMz.Execute(chargedTarget.MonoIsotopicMass, chargedTarget.ChargeState, MassProton);


        //            _theorFeatureGen.GenerateTheorFeature(chargedTarget);

        //            ChargedParentsToSearchFor.Add(chargedTarget);

        //            if (print) Console.WriteLine("Loading " + chargedTarget.MZ + " at charge " + chargedTarget.ChargeState + " scan " + chargedTarget.ScanLCTarget);


        //            CorrelationObject loadchargedTargetPeakFragment = new CorrelationObject(chargedTarget.IsotopicProfile, scanStartPossibiilty, scanStopPossibiilty, _workflowParameters.ChromGenTolerance, _workflowParameters.MinRelativeIntensityForChromCorrelator, run, ref _peakChromGen, ref _smoother);

        //            int newScanStart;
        //            int newScanStop;

        //            if (loadchargedTargetPeakFragment.AreChromDataIsOK)
        //            {
        //                List<PNNLOmics.Data.XYData> chargedTargetEic = Utiliites.PullBestEIC(loadchargedTargetPeakFragment, ref errorLog);

        //                if (chargedTargetEic != null)
        //                {
        //                    double maxValue = chargedTargetEic.Max(x => x.Y);//we may not need this
        //                    if (print) Console.WriteLine(maxValue);
        //                }
        //                //WriteXYData(chargedTargetEic);


        //                Utiliites.SetNewScanStartStopOmics(chargedTargetEic, scanStartPossibiilty, scanStopPossibiilty, chargedTarget, out newScanStart, out newScanStop, print, ref errorLog);
        //            }
        //            else
        //            {
        //                //Console.WriteLine("the old scan Range is " + scanStartPossibiilty + "-" + scanStopPossibiilty);
        //                if (print) Console.WriteLine("FAIL");
        //                newScanStart = scanStartPossibiilty;
        //                newScanStop = scanStopPossibiilty;
        //            }

        //            chargedTarget.StartScan = newScanStart;
        //            chargedTarget.StopScan = newScanStop;
        //        } //next charge

        //        #endregion
        //    } //next fragment

        //    if (print) Console.WriteLine(Environment.NewLine);


        //    //check targets
        //    foreach (FragmentTarget fragmentTarget in ChargedParentsToSearchFor)
        //    {
        //        //validate each candidate
        //        bool checkScanRange = fragmentTarget.StartScan > 0 && fragmentTarget.StartScan > 0 && fragmentTarget.StartScan < fragmentTarget.ScanLCTarget && fragmentTarget.StopScan > fragmentTarget.ScanLCTarget && fragmentTarget.StopScan < run.MaxLCScan; ;
        //        bool checkIso = fragmentTarget.IsotopicProfile != null;
        //        bool checkChargeState = fragmentTarget.ChargeState > 0;
        //        bool checkMZ = fragmentTarget.MZ > 0;
        //        bool checkMono = fragmentTarget.MonoIsotopicMass > 0;
        //        if (
        //            checkChargeState &&
        //            checkIso &&
        //            checkChargeState &&
        //            checkMZ &&
        //            checkMono
        //            )
        //        {
        //            if (print) Console.WriteLine("+++Pass Test");
        //            finalChargedParentsToSearchFor.Add(fragmentTarget);
        //        }
        //        else
        //        {
        //            if (print) Console.WriteLine("---Fail Test");
        //            //the peak does not make it because it is missing information rather than statistial reason
        //        }
        //        if (print) Console.WriteLine("Parent " + fragmentTarget.MZ + " at charge " + fragmentTarget.ChargeState + " scan " + fragmentTarget.ScanLCTarget + ": " + fragmentTarget.StartScan + "-" + fragmentTarget.StopScan);
        //    }

        //    if (print) Console.WriteLine("Next Peak Quality, we have " + finalChargedParentsToSearchFor.Count + " candidate targets");

        //    return finalChargedParentsToSearchFor;
        //}
        #endregion
    }
}
