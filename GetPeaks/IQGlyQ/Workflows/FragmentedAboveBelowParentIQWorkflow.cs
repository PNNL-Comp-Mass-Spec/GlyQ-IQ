using System;
using System.Linq;
using DeconTools.Utilities;
using DeconTools.Workflows.Backend.Core;
using DeconTools.Backend.Core;
using GetPeaks_DLL.Functions;
using IQGlyQ.Enumerations;
using IQGlyQ.Exporter;
using System.Collections.Generic;
using IQGlyQ.Functions;
using IQGlyQ.Results;
using IQGlyQ.TargetGenerators;
using PNNLOmics.Data.Constants;

namespace IQGlyQ.Workflows
{
    public class FragmentedAboveBelowParentIQWorkflow : IqWorkflow
    {

        #region Constructors

        public FragmentedAboveBelowParentIQWorkflow(Run run, FragmentedTargetedWorkflowParametersIQ parameters)
            : base(run, parameters)
        {

            InitializeWorkflow();

        }

        public FragmentedAboveBelowParentIQWorkflow(FragmentedTargetedWorkflowParametersIQ parameters)
            : base(parameters)
        {

            InitializeWorkflow();
        }

        #endregion

        public override void InitializeWorkflow()
        {
            Check.Require(Run != null, "Run is null");

            base.DoPreInitialization();

            base.DoMainInitialization();

            base.DoPostInitialization();

            base.DoPostInitialization();

            //FragmentedTargetedWorkflowParametersIQ WorkflowParameters = (FragmentedTargetedWorkflowParametersIQ) this.WorkflowParameters;
        }

        public override TargetedWorkflowParameters WorkflowParameters { get; set; }

        public void ExecuteOld(IqResult result)
        {
            Console.WriteLine("Start parent Workflow");
            
            if (result.HasChildren())
            {
                FragmentedTargetedWorkflowParametersIQ WorkflowParameters =
                    (FragmentedTargetedWorkflowParametersIQ) this.WorkflowParameters;
                double fragmentCuttoffThreshold = WorkflowParameters.CorrelationScoreCuttoff;
                double fitScoreCuttoff = WorkflowParameters.FitScoreCuttoff;

                //1.  collect all children
                List<IqGlyQResult> existingGlyQiqChildren = WorkflowUtilities.CastToGlyQiQResult(result, fitScoreCuttoff);

                //2.  collect all children that are valid features
                List<IqGlyQResult> validGlyQiqChildren = (from n in existingGlyQiqChildren where n.ToChild.Error == EnumerationError.Success || n.ToChild.Error == EnumerationError.NoParents select n).ToList();

                    //all targets (not modified.  Parents are modified)
                List<IqGlyQResult> validGlyQiqChildrenTargetsAllFitScores = (from n in validGlyQiqChildren where n.ToChild.TypeOfResultTargetOrModifiedTarget == TypeOfResult.Target select n).ToList();

                    //all results above fit score cuttoff
                List<IqGlyQResult> validGlyQiqChildrenTargets = (from n in validGlyQiqChildrenTargetsAllFitScores where n.FitScore < WorkflowParameters.FitScoreCuttoff select n).ToList();

                validGlyQiqChildren = validGlyQiqChildren.OrderBy(n => n.ToChild.TargetFragment.ScanLCTarget).ToList();
                
                List<IqGlyQResult> baseIsomers = new List<IqGlyQResult>();
                //label origiional targets

                WorkflowUtilities.PrintHits(result);

                //make summarized result that contains details overall.  This will have 0 charge and be identified by Flobalresult
                IqGlyQResult baseResult = new IqGlyQResult(result.Target);
                baseResult.ToChild.GlobalResult = true;

                //3.  find a unique scan list.  we may want to cluster these together so features from different charge states are to tegher.  a +1 peak may be 3 scans from a +2 peak
                List<int> scanList = (from n in validGlyQiqChildren select n.ToChild.TargetFragment.ScanLCTarget).Distinct().ToList();
               
                scanList.Sort();

                foreach (int scan in scanList)
                {
                    int scanCurrent = scan;

                    IqTargetUtilities util = new IqTargetUtilities();
                    IqTarget parentTargetcopy = util.Clone(result.Target);
                    
                    IqGlyQResult isomerResult = new IqGlyQResult(parentTargetcopy);
                    //IqGlyQResult isomerResult = new IqGlyQResult(result.Target);
                    isomerResult.ToChild.GlobalResult = true;
                    isomerResult.ToChild.TargetFragment.ScanLCTarget = scanCurrent;

                    //2. find scan for global result
                    isomerResult.LCScanSetSelected = new ScanSet(scanCurrent);

                    //3.  divide into children and parents
                    
                    //List<IqGlyQResult> scanResultsList = (from n in validGlyQiqChildren where n.ToChild.TargetFragment.ScanLCTarget == scanCurrent select n).ToList();//perhaps add a range of scansets
                    
                    //this has targets and connecting modifieds all piled up
                    List<IqGlyQResult> scanResultsList = (from n in validGlyQiqChildren where n.ToChild.TargetFragment.ScanLCTarget == scanCurrent select n).ToList();//perhaps add a range of scansets
                    
                    //this just has targets
                    List<IqGlyQResult> scanResultsListFragments = (from n in validGlyQiqChildrenTargets where n.ToChild.TargetFragment.ScanLCTarget == scanCurrent select n).ToList();//perhaps add a range of scansets

                    List<IqGlyQResult> childrenOfTheScan = (from n in scanResultsList where n.ToChild.TypeOfResultParentOrChildDifferenceApproach == EnumerationParentOrChild.ChildrenOnly select n).ToList();
                    List<IqGlyQResult> parentsfTheScan = (from n in scanResultsList where n.ToChild.TypeOfResultParentOrChildDifferenceApproach == EnumerationParentOrChild.ParentsOnly select n).ToList();


                    //4.  summarize feature information
                    
                    IqGlyQResult globalResultForThisScanChild = new IqGlyQResult(result.Target);
                    SummarizeFeature(childrenOfTheScan, ref globalResultForThisScanChild);
                    //globalResultForThisScanChild.ToChild.FragmentCharge = chargeAtMax * 10000;
                    ////populate charge state at max abundance

                    //return charge state at max area
                    //int chargeAtMaxArea = globalResultForThisScanChild.ToChild.ChargeArea.Aggregate((n, r) => n.Value > r.Value ? n : r).Key;
                    //result.Target.ChargeState = chargeAtMaxArea * 1000;
                    //this sets the deconcharge state of the global result
                    //SetChargeStateInBase(chargeAtMaxArea, ref isomerResult);
                    

                    IqGlyQResult globalResultForThisScanParent = new IqGlyQResult(result.Target);
                    SummarizeFeature(parentsfTheScan, ref globalResultForThisScanParent);
                    //globalResultForThisScanChild.ToChild.ParentCharge = chargeAtMax * 1000;

                    

                    //charge state range
                    if (globalResultForThisScanChild != null && globalResultForThisScanChild.ToChild.ChargeMonoMass.Count > 0)
                    {
                        isomerResult.ToChild.GlobalChargeStateMin = globalResultForThisScanChild.ToChild.ChargeMonoMass.Keys.First();
                        isomerResult.ToChild.GlobalChargeStateMax = globalResultForThisScanChild.ToChild.ChargeMonoMass.Keys.Last();
                        
                        //this is where we put the charge state.  this is copied to "Charge" in the post run
                        //you can put the best charge for the global result here is the best way to do it
                        //we need the condition that 
                        //1.  EnumerationParentOrChild.ChildrenOnly (childrenOfTheScan)
                        //2.  FragmentArea>0
                        //3.  Not  EnumerationError.NoFragment (!EnumerationError.NoFragment).  bool fragmentPassesProcessing = false  No parent and Success are ok
                        IqResult baseCallTarget = isomerResult;

                        IqGlyQResult mostAbundantFromChildrenInScan = SelectMostIntenseIqGlyQResult(childrenOfTheScan);
                        IqResult baseMostAbundantFromChildrenInScan = mostAbundantFromChildrenInScan;
                        if (mostAbundantFromChildrenInScan != null)
                        {
                            int maxCharge = mostAbundantFromChildrenInScan.ToChild.TargetFragment.ChargeState;
                            float maxAbundance = mostAbundantFromChildrenInScan.ToChild.FragmentFitAbundance;

                            isomerResult.ToChild.TargetFragment.ChargeState = maxCharge;
                            baseCallTarget.Abundance = maxAbundance;
                            baseCallTarget.MZObs = baseMostAbundantFromChildrenInScan.MZObs;
                            baseCallTarget.MonoMassObs = baseMostAbundantFromChildrenInScan.MonoMassObs;
                        }
                        else
                        {
                            IqGlyQResult mostAbundantInScan = SelectMostIntenseIqGlyQResult(scanResultsList);//children are not included by filter above
                            IqResult baseMostAbundantInScan = mostAbundantInScan;
                            if (mostAbundantInScan != null)
                            {
                                int maxCharge = mostAbundantInScan.ToChild.TargetFragment.ChargeState;
                                float maxAbundance = mostAbundantInScan.ToChild.FragmentFitAbundance;

                                isomerResult.ToChild.TargetFragment.ChargeState = maxCharge;
                                baseCallTarget.Abundance = maxAbundance;
                                if (mostAbundantInScan.ToChild.FragmentObservedIsotopeProfile != null)
                                {
                                    baseCallTarget.MZObs = mostAbundantInScan.ToChild.FragmentObservedIsotopeProfile.MonoPeakMZ;
                                    baseCallTarget.MonoMassObs = mostAbundantInScan.ToChild.FragmentObservedIsotopeProfile.MonoIsotopicMass;
                                }
                            }
                            else//when nothing is found
                            {
                                baseCallTarget.Abundance = 0;
                                isomerResult.ToChild.TargetFragment.ChargeState = isomerResult.ToChild.GlobalChargeStateMax;//these have the same effect.  both charge and framgentcharge
                            }
                        }
                    }

                   

                    //5.  mass and error only pulled from "fragment" results aka targets
                    int avarageMassCounter = 0;
                    double sumExperimentalMonoMass = 0;
                    if (scanResultsListFragments != null && scanResultsListFragments.Count > 0)
                    {
                        foreach (IqGlyQResult searchForMonoMassResult in scanResultsListFragments)
                        {
                            //double monoMass = searchForMonoMassResult.MonoMassObs;
                            double monoMass = searchForMonoMassResult.ToChild.FragmentObservedIsotopeProfile.MonoIsotopicMass;
                            if (monoMass > 0)
                            {
                                sumExperimentalMonoMass += monoMass;
                                avarageMassCounter++;
                            }
                        }

                        isomerResult.ToChild.AverageMonoMass = 0;
                        isomerResult.ToChild.PPMError = -99;

                        if (avarageMassCounter > 0)
                        {
                            isomerResult.ToChild.AverageMonoMass = sumExperimentalMonoMass/avarageMassCounter;

                            //calibrate MonoMassTheor here
                            double existingTheoreticalMonoMass = isomerResult.ToChild.TargetFragment.MonoMassTheor;//this is for MZ shift only because Mono is allready calibrated
                            double massProton = Constants.SubAtomicParticles[SubAtomicParticleName.Proton].MassMonoIsotopic;
                            int charge = isomerResult.ToChild.TargetFragment.ChargeState;
                            //move the mz and then move it back to get calibrated mono
                            double calibratedTheoreticalMZ = GetPeaks_DLL.Functions.ConvertMonoToMz.Execute(isomerResult.ToChild.TargetFragment.MonoMassTheor,charge,massProton) + WorkflowParameters.MSParameters.IsoParameters.DeltaMassCalibrationMZ;
                            double calibratedTheoreticalMono = GetPeaks_DLL.Functions.ConvertMzToMono.Execute(calibratedTheoreticalMZ,charge,massProton);

                            isomerResult.ToChild.PPMError = ErrorCalculator.PPM(isomerResult.ToChild.AverageMonoMass, calibratedTheoreticalMono);

                            //isomerResult.ToChild.PPMError = ErrorCalculator.PPM(isomerResult.ToChild.AverageMonoMass, isomerResult.ToChild.TargetFragment.MonoMassTheor);
                        }
                    }
                    else//where else can this live?
                    {
                        isomerResult.ToChild.AverageMonoMass = -5;
                        isomerResult.ToChild.PPMError = -99;
                    }

                    //6.  find optimal scan
                    


                    //7.  logic
                    bool hitAtLowerMass = globalResultForThisScanChild.ToChild.CorrelationScore > fragmentCuttoffThreshold;
                    bool hitAtHigherMass = globalResultForThisScanParent.ToChild.CorrelationScore > fragmentCuttoffThreshold;

                    
                    if (hitAtLowerMass && hitAtHigherMass)
                    {
                        //This is a glycan fragment
                        SummarizeFeature(childrenOfTheScan, ref isomerResult);
                        isomerResult.ToChild.FinalDecision = EnumerationFinalDecision.ValidatedGlycanFragment;
                    }

                    if (hitAtLowerMass && !hitAtHigherMass)
                    {
                        //This is a correct Glycan Yay!
                        SummarizeFeature(childrenOfTheScan, ref isomerResult);
                        isomerResult.ToChild.FinalDecision = EnumerationFinalDecision.CorrectGlycan;
                    }

                    if (!hitAtLowerMass && hitAtHigherMass)
                    {
                        //the parent (fragment + hexose etc.) is a future Target
                        SummarizeFeature(childrenOfTheScan, ref isomerResult);
                        isomerResult.ToChild.FinalDecision = EnumerationFinalDecision.FutureTarget;
                    }

                    if (!hitAtLowerMass && !hitAtHigherMass)
                    {
                        //This is a random hit that could be good but we can't verify it
                        SummarizeFeature(childrenOfTheScan, ref isomerResult);
                        isomerResult.ToChild.FinalDecision = EnumerationFinalDecision.NonValidatedHit;
                    }

                    //6.  report summed abundance
                    double summedAbundance = 0;
                    foreach (KeyValuePair<int, double> keyvalue in isomerResult.ToChild.ChargeArea)
                    {
                        summedAbundance += keyvalue.Value;
                    }
                    isomerResult.ToChild.FragmentFitAbundance =  Convert.ToInt64(summedAbundance);
                    //end


                    ////7.  for each charge state, pull abundance and select the best
                    //List<int> chargeStateList = (from n in childrenOfTheScan select n.ToChild.TargetFragment.ChargeState).Distinct().ToList();//this should give us all charges from those that are in the "scan" and are "ChildrenOnly"
                    //int maxCharge = 0;
                    //if (chargeStateList.Count > 0)
                    //{
                    //    Dictionary<int, double> chargeWithAbundance = new Dictionary<int, double>();
                    //    foreach (var i in chargeStateList)
                    //    {
                    //        List<FragmentIQTarget> chargeSpecificTargets = (from n in childrenOfTheScan where n.ToChild.TargetFragment.ChargeState == i select n.ToChild.TargetFragment).ToList();
                    //        List<double> areas = new List<double>();
                    //        double maxAbundance = 0;

                    //        foreach (var chargeSpecificTarget in chargeSpecificTargets)
                    //        {
                    //            double area = chargeSpecificTarget.TheorIsotopicProfile.IntensityAggregateAdjusted;
                    //            areas.Add(area);
                    //            if (area > maxAbundance)
                    //            {
                    //                maxAbundance = area;
                    //            }
                    //        }
                    //        chargeWithAbundance.Add(i, maxAbundance);
                    //    }
                    //    int chargeAtMax = chargeWithAbundance.Aggregate((n, r) => n.Value > r.Value ? n : r).Key;//charge at max abundance
                    //    //double overallMaxAbundance = chargeWithAbundance[chargeAtMax];
                    //    //maxCharge = chargeAtMax;
                    //    //isomerResult.ToChild.GlobalChargeStateMin = chargeWithAbundance.Keys.Min();
                    //    //isomerResult.ToChild.GlobalChargeStateMax = chargeWithAbundance.Keys.Max();
                    //    //return charge state at max area
                    //    //int chargeAtMaxArea = agretateResult.ToChild.ChargeArea.Aggregate((n, r) => n.Value > r.Value ? n : r).Key;
                    //    //isomerResult.Target.ChargeState = chargeAtMax * 1000;
                    //    //isomerResult.ToChild.TargetFragment.ChargeState = chargeAtMax*10000;
                    //    //this sets the deconcharge state of the global result
                    //    //SetChargeStateInBase(chargeAtMax, ref isomerResult);
                    //}
                    


                    baseIsomers.Add(isomerResult);
                }

                ////1.  setup base result summary
                ////double fragmentCuttoffThreshold = 0.6;//used for counting up how many fragments there are
                //List<IqGlyQResult> intactList;
                //IqGlyQResult baseResult = OrganizeBaseResult(result, existingGlyQiqChildren, replacementGlyQiqChildren,
                //                                             fragmentCuttoffThreshold, out intactList);

                ////2.  add to new list
                //replacementGlyQiqChildren.Add(baseResult);

                ////3.  sum all charge states within isomers so we have general isomer abundances
                //List<IqGlyQResult> baseIsomers = OrganizeIsomers(ref replacementGlyQiqChildren, baseResult, intactList);

                ////4.  add to new list
                //replacementGlyQiqChildren.AddRange(baseIsomers);

                ////5.  build into list for export

                foreach (IqGlyQResult agretateResult in baseIsomers)
                {
                   
                    
                    result.AddResult(agretateResult);
                    //replacementResult.AddResult(agretateResult);
                }

            }
        }

        public override void Execute(IqResult result)
        {
            Console.WriteLine("Start parent Workflow");

            if (result.HasChildren())
            {
                FragmentedTargetedWorkflowParametersIQ WorkflowParameters = (FragmentedTargetedWorkflowParametersIQ)this.WorkflowParameters;
                double fragmentCuttoffThreshold = WorkflowParameters.CorrelationScoreCuttoff;
                double fitScoreCuttoff = WorkflowParameters.FitScoreCuttoff;

                //1.  collect all children
                List<IqGlyQResult> existingGlyQiqChildren = WorkflowUtilities.CastToGlyQiQResult(result, fitScoreCuttoff);

                //2.  collect all children that are valid features.  contains targets and modified targets
                List<IqGlyQResult> validGlyQiqChildren = (from n in existingGlyQiqChildren where n.ToChild.Error == EnumerationError.Success || n.ToChild.Error == EnumerationError.NoParents select n).ToList();

                //all targets that not modified.  Parents are modified
                List<IqGlyQResult> validGlyQiqChildrenTargetsAllFitScores = (from n in validGlyQiqChildren where n.ToChild.TypeOfResultTargetOrModifiedTarget == TypeOfResult.Target select n).ToList();

                //all results above fit score cuttoff
                List<IqGlyQResult> validGlyQiqChildrenTargets = (from n in validGlyQiqChildrenTargetsAllFitScores where n.FitScore < WorkflowParameters.FitScoreCuttoff select n).ToList();

                validGlyQiqChildren = validGlyQiqChildren.OrderBy(n => n.ToChild.TargetFragment.ScanLCTarget).ToList();

                List<IqGlyQResult> baseIsomers = new List<IqGlyQResult>();
                //label origiional targets

                WorkflowUtilities.PrintHits(result);

                //make summarized result that contains details overall.  This will have 0 charge and be identified by Flobalresult
                IqGlyQResult baseResult = new IqGlyQResult(result.Target);
                baseResult.ToChild.GlobalResult = true;

                //3.  find a unique scan list.  we may want to cluster these together so features from different charge states are to tegher.  a +1 peak may be 3 scans from a +2 peak
                List<int> scanList = (from n in validGlyQiqChildren select n.ToChild.TargetFragment.ScanLCTarget).Distinct().ToList();

                scanList.Sort();

                foreach (int scan in scanList)
                {
                    int scanCurrent = scan;

                    IqTargetUtilities util = new IqTargetUtilities();
                    IqTarget parentTargetcopy = util.Clone(result.Target);

                    IqGlyQResult isomerResult = new IqGlyQResult(parentTargetcopy);
                    //IqGlyQResult isomerResult = new IqGlyQResult(result.Target);
                    isomerResult.ToChild.GlobalResult = true;
                    isomerResult.ToChild.TargetFragment.ScanLCTarget = scanCurrent;

                    //2. find scan for global result
                    isomerResult.LCScanSetSelected = new ScanSet(scanCurrent);

                    //3.  divide into children and parents

                    //List<IqGlyQResult> scanResultsList = (from n in validGlyQiqChildren where n.ToChild.TargetFragment.ScanLCTarget == scanCurrent select n).ToList();//perhaps add a range of scansets

                    //this has targets and connecting modifieds all piled up
                    List<IqGlyQResult> scanResultsList = (from n in validGlyQiqChildren where n.ToChild.TargetFragment.ScanLCTarget == scanCurrent select n).ToList();//perhaps add a range of scansets

                    //this just has targets
                    List<IqGlyQResult> scanResultsListFragments = (from n in validGlyQiqChildrenTargets where n.ToChild.TargetFragment.ScanLCTarget == scanCurrent select n).ToList();//perhaps add a range of scansets

                    //this has targets and modifided targets from the child direction
                    List<IqGlyQResult> childrenOfTheScan = (from n in scanResultsList where n.ToChild.TypeOfResultParentOrChildDifferenceApproach == EnumerationParentOrChild.ChildrenOnly select n).ToList();
                    //this has targets and modifided targets from the parent direction
                    List<IqGlyQResult> parentsfTheScan = (from n in scanResultsList where n.ToChild.TypeOfResultParentOrChildDifferenceApproach == EnumerationParentOrChild.ParentsOnly select n).ToList();


                    //4.  summarize feature information

                    IqGlyQResult globalResultForThisScanChild = new IqGlyQResult(result.Target);
                    SummarizeFeature(childrenOfTheScan, ref globalResultForThisScanChild);
                    //globalResultForThisScanChild.ToChild.FragmentCharge = chargeAtMax * 10000;
                    ////populate charge state at max abundance

                    //return charge state at max area
                    //int chargeAtMaxArea = globalResultForThisScanChild.ToChild.ChargeArea.Aggregate((n, r) => n.Value > r.Value ? n : r).Key;
                    //result.Target.ChargeState = chargeAtMaxArea * 1000;
                    //this sets the deconcharge state of the global result
                    //SetChargeStateInBase(chargeAtMaxArea, ref isomerResult);


                    IqGlyQResult globalResultForThisScanParent = new IqGlyQResult(result.Target);
                    SummarizeFeature(parentsfTheScan, ref globalResultForThisScanParent);
                    //globalResultForThisScanChild.ToChild.ParentCharge = chargeAtMax * 1000;



                    //charge state range
                    if (globalResultForThisScanChild != null && globalResultForThisScanChild.ToChild.ChargeMonoMass.Count > 0)
                    {
                        isomerResult.ToChild.GlobalChargeStateMin = globalResultForThisScanChild.ToChild.ChargeMonoMass.Keys.First();
                        isomerResult.ToChild.GlobalChargeStateMax = globalResultForThisScanChild.ToChild.ChargeMonoMass.Keys.Last();

                        //this is where we put the charge state.  this is copied to "Charge" in the post run
                        //you can put the best charge for the global result here is the best way to do it
                        //we need the condition that 
                        //1.  EnumerationParentOrChild.ChildrenOnly (childrenOfTheScan)
                        //2.  FragmentArea>0
                        //3.  Not  EnumerationError.NoFragment (!EnumerationError.NoFragment).  bool fragmentPassesProcessing = false  No parent and Success are ok
                        IqResult baseCallTarget = isomerResult;

                        IqGlyQResult mostAbundantFromChildrenInScan = SelectMostIntenseIqGlyQResult(childrenOfTheScan);
                        IqResult baseMostAbundantFromChildrenInScan = mostAbundantFromChildrenInScan;
                        if (mostAbundantFromChildrenInScan != null)
                        {
                            int maxCharge = mostAbundantFromChildrenInScan.ToChild.TargetFragment.ChargeState;
                            float maxAbundance = mostAbundantFromChildrenInScan.ToChild.FragmentFitAbundance;

                            isomerResult.ToChild.TargetFragment.ChargeState = maxCharge;
                            baseCallTarget.Abundance = maxAbundance;
                            baseCallTarget.MZObs = baseMostAbundantFromChildrenInScan.MZObs;
                            baseCallTarget.MonoMassObs = baseMostAbundantFromChildrenInScan.MonoMassObs;
                        }
                        else
                        {
                            IqGlyQResult mostAbundantInScan = SelectMostIntenseIqGlyQResult(scanResultsList);//children are not included by filter above
                            IqResult baseMostAbundantInScan = mostAbundantInScan;
                            if (mostAbundantInScan != null)
                            {
                                int maxCharge = mostAbundantInScan.ToChild.TargetFragment.ChargeState;
                                float maxAbundance = mostAbundantInScan.ToChild.FragmentFitAbundance;

                                isomerResult.ToChild.TargetFragment.ChargeState = maxCharge;
                                baseCallTarget.Abundance = maxAbundance;
                                if (mostAbundantInScan.ToChild.FragmentObservedIsotopeProfile != null)
                                {
                                    baseCallTarget.MZObs = mostAbundantInScan.ToChild.FragmentObservedIsotopeProfile.MonoPeakMZ;
                                    baseCallTarget.MonoMassObs = mostAbundantInScan.ToChild.FragmentObservedIsotopeProfile.MonoIsotopicMass;
                                }
                            }
                            else//when nothing is found
                            {
                                baseCallTarget.Abundance = 0;
                                isomerResult.ToChild.TargetFragment.ChargeState = isomerResult.ToChild.GlobalChargeStateMax;//these have the same effect.  both charge and framgentcharge
                            }
                        }
                    }



                    //5.  mass and error only pulled from "fragment" results aka targets
                    int avarageMassCounter = 0;
                    double sumExperimentalMonoMass = 0;
                    if (scanResultsListFragments != null && scanResultsListFragments.Count > 0)
                    {
                        foreach (IqGlyQResult searchForMonoMassResult in scanResultsListFragments)
                        {
                            //double monoMass = searchForMonoMassResult.MonoMassObs;
                            double monoMass = searchForMonoMassResult.ToChild.FragmentObservedIsotopeProfile.MonoIsotopicMass;
                            if (monoMass > 0)
                            {
                                sumExperimentalMonoMass += monoMass;
                                avarageMassCounter++;
                            }
                        }

                        isomerResult.ToChild.AverageMonoMass = 0;
                        isomerResult.ToChild.PPMError = -99;

                        if (avarageMassCounter > 0)
                        {
                            isomerResult.ToChild.AverageMonoMass = sumExperimentalMonoMass / avarageMassCounter;

                            //calibrate MonoMassTheor here
                            double existingTheoreticalMonoMass = isomerResult.ToChild.TargetFragment.MonoMassTheor;//this is for MZ shift only because Mono is allready calibrated
                            double massProton = Constants.SubAtomicParticles[SubAtomicParticleName.Proton].MassMonoIsotopic;
                            int charge = isomerResult.ToChild.TargetFragment.ChargeState;
                            //move the mz and then move it back to get calibrated mono
                            double calibratedTheoreticalMZ = GetPeaks_DLL.Functions.ConvertMonoToMz.Execute(isomerResult.ToChild.TargetFragment.MonoMassTheor, charge, massProton) + WorkflowParameters.MSParameters.IsoParameters.DeltaMassCalibrationMZ;
                            double calibratedTheoreticalMono = GetPeaks_DLL.Functions.ConvertMzToMono.Execute(calibratedTheoreticalMZ, charge, massProton);

                            isomerResult.ToChild.PPMError = ErrorCalculator.PPM(isomerResult.ToChild.AverageMonoMass, calibratedTheoreticalMono);

                            //isomerResult.ToChild.PPMError = ErrorCalculator.PPM(isomerResult.ToChild.AverageMonoMass, isomerResult.ToChild.TargetFragment.MonoMassTheor);
                        }
                    }
                    else//where else can this live?
                    {
                        isomerResult.ToChild.AverageMonoMass = -5;
                        isomerResult.ToChild.PPMError = -99;
                    }

                    //6.  find optimal scan



                    //7.  logic
                    bool hitAtLowerMass = globalResultForThisScanChild.ToChild.CorrelationScore > fragmentCuttoffThreshold;
                    bool hitAtHigherMass = globalResultForThisScanParent.ToChild.CorrelationScore > fragmentCuttoffThreshold;


                    if (hitAtLowerMass && hitAtHigherMass)
                    {
                        //This is a glycan fragment
                        SummarizeFeature(childrenOfTheScan, ref isomerResult);
                        isomerResult.ToChild.FinalDecision = EnumerationFinalDecision.ValidatedGlycanFragment;
                    }

                    if (hitAtLowerMass && !hitAtHigherMass)
                    {
                        //This is a correct Glycan Yay!
                        SummarizeFeature(childrenOfTheScan, ref isomerResult);
                        isomerResult.ToChild.FinalDecision = EnumerationFinalDecision.CorrectGlycan;
                    }

                    if (!hitAtLowerMass && hitAtHigherMass)
                    {
                        //the parent (fragment + hexose etc.) is a future Target
                        SummarizeFeature(childrenOfTheScan, ref isomerResult);
                        isomerResult.ToChild.FinalDecision = EnumerationFinalDecision.FutureTarget;
                    }

                    if (!hitAtLowerMass && !hitAtHigherMass)
                    {
                        //This is a random hit that could be good but we can't verify it
                        SummarizeFeature(childrenOfTheScan, ref isomerResult);
                        isomerResult.ToChild.FinalDecision = EnumerationFinalDecision.NonValidatedHit;
                    }

                    //6.  report summed abundance
                    double summedAbundance = 0;
                    foreach (KeyValuePair<int, double> keyvalue in isomerResult.ToChild.ChargeArea)
                    {
                        summedAbundance += keyvalue.Value;
                    }
                    isomerResult.ToChild.FragmentFitAbundance = Convert.ToInt64(summedAbundance);
                    //end


                    ////7.  for each charge state, pull abundance and select the best
                    //List<int> chargeStateList = (from n in childrenOfTheScan select n.ToChild.TargetFragment.ChargeState).Distinct().ToList();//this should give us all charges from those that are in the "scan" and are "ChildrenOnly"
                    //int maxCharge = 0;
                    //if (chargeStateList.Count > 0)
                    //{
                    //    Dictionary<int, double> chargeWithAbundance = new Dictionary<int, double>();
                    //    foreach (var i in chargeStateList)
                    //    {
                    //        List<FragmentIQTarget> chargeSpecificTargets = (from n in childrenOfTheScan where n.ToChild.TargetFragment.ChargeState == i select n.ToChild.TargetFragment).ToList();
                    //        List<double> areas = new List<double>();
                    //        double maxAbundance = 0;

                    //        foreach (var chargeSpecificTarget in chargeSpecificTargets)
                    //        {
                    //            double area = chargeSpecificTarget.TheorIsotopicProfile.IntensityAggregateAdjusted;
                    //            areas.Add(area);
                    //            if (area > maxAbundance)
                    //            {
                    //                maxAbundance = area;
                    //            }
                    //        }
                    //        chargeWithAbundance.Add(i, maxAbundance);
                    //    }
                    //    int chargeAtMax = chargeWithAbundance.Aggregate((n, r) => n.Value > r.Value ? n : r).Key;//charge at max abundance
                    //    //double overallMaxAbundance = chargeWithAbundance[chargeAtMax];
                    //    //maxCharge = chargeAtMax;
                    //    //isomerResult.ToChild.GlobalChargeStateMin = chargeWithAbundance.Keys.Min();
                    //    //isomerResult.ToChild.GlobalChargeStateMax = chargeWithAbundance.Keys.Max();
                    //    //return charge state at max area
                    //    //int chargeAtMaxArea = agretateResult.ToChild.ChargeArea.Aggregate((n, r) => n.Value > r.Value ? n : r).Key;
                    //    //isomerResult.Target.ChargeState = chargeAtMax * 1000;
                    //    //isomerResult.ToChild.TargetFragment.ChargeState = chargeAtMax*10000;
                    //    //this sets the deconcharge state of the global result
                    //    //SetChargeStateInBase(chargeAtMax, ref isomerResult);
                    //}



                    baseIsomers.Add(isomerResult);
                }

                ////1.  setup base result summary
                ////double fragmentCuttoffThreshold = 0.6;//used for counting up how many fragments there are
                //List<IqGlyQResult> intactList;
                //IqGlyQResult baseResult = OrganizeBaseResult(result, existingGlyQiqChildren, replacementGlyQiqChildren,
                //                                             fragmentCuttoffThreshold, out intactList);

                ////2.  add to new list
                //replacementGlyQiqChildren.Add(baseResult);

                ////3.  sum all charge states within isomers so we have general isomer abundances
                //List<IqGlyQResult> baseIsomers = OrganizeIsomers(ref replacementGlyQiqChildren, baseResult, intactList);

                ////4.  add to new list
                //replacementGlyQiqChildren.AddRange(baseIsomers);

                ////5.  build into list for export

                foreach (IqGlyQResult agretateResult in baseIsomers)
                {


                    result.AddResult(agretateResult);
                    //replacementResult.AddResult(agretateResult);
                }

            }
        }
        private static IqGlyQResult SelectMostIntenseIqGlyQResult(List<IqGlyQResult> resultList)
        {
            List<IqGlyQResult> chargeCandidatesFromChildrenScan = (from n in resultList where n.ToChild.FragmentFitAbundance > 0 && (n.ToChild.Error == EnumerationError.Success || n.ToChild.Error == EnumerationError.NoParents) select n).ToList();
            if (chargeCandidatesFromChildrenScan != null && chargeCandidatesFromChildrenScan.Count > 0)
            {
                chargeCandidatesFromChildrenScan = chargeCandidatesFromChildrenScan.OrderByDescending(n => n.ToChild.FragmentFitAbundance).ToList();
                IqGlyQResult mostAbundant = chargeCandidatesFromChildrenScan[0];

                return mostAbundant;
            }
            return null;
        }
        private static void SetChargeStateInBase(int chargeAtMax, ref IqGlyQResult result)
        {
            IqResult baseCallToChargeState = result;
            baseCallToChargeState.Target.ChargeState = chargeAtMax;
            //result.ToChild.FragmentCharge = chargeAtMax*1000;
        }

        private static void SummarizeFeature(List<IqGlyQResult> childrenOfTheScan, ref IqGlyQResult globalResultForThisScan)
        {        
            //4.  summarize charge state and correlation values observed for the feature
            //List<int> chargeStateList = (from n in childrenOfTheScan select n.ToChild.TargetFragment.ChargeState).Distinct().ToList();
            List<int> chargeStateList = (from n in childrenOfTheScan where n.ToChild.TypeOfResultTargetOrModifiedTarget == TypeOfResult.Target select n.ToChild.TargetFragment.ChargeState).Distinct().ToList();//now we look at the charge state rangge of the targets only

            if (chargeStateList.Count > 0)
            {
                globalResultForThisScan.ToChild.GlobalChargeStateMax = chargeStateList.Max();
                globalResultForThisScan.ToChild.GlobalChargeStateMin = chargeStateList.Min();
            }

            ////for each charge state, pull abundance
            //maxCharge = 0;
            //if (chargeStateList.Count > 0)
            //{
            //    Dictionary<int,double> chargeWithAbundance = new Dictionary<int, double>();
            //    foreach (var i in chargeStateList)
            //    {
            //        List<FragmentIQTarget> chargeSpecificTargets = (from n in childrenOfTheScan where n.ToChild.TargetFragment.ChargeState == i select n.ToChild.TargetFragment).ToList();
            //        List<double> abundances = new List<double>();
            //        double maxAbundance = 0;
                    
            //        foreach (var chargeSpecificTarget in chargeSpecificTargets)
            //        {
            //            double area = chargeSpecificTarget.TheorIsotopicProfile.IntensityAggregateAdjusted;
            //            abundances.Add(area);
            //            if (area > maxAbundance)
            //            {
            //                maxAbundance = area;
            //            }
            //        }
            //        chargeWithAbundance.Add(i,maxAbundance);
            //    }
            //    int chargeAtMax = chargeWithAbundance.Aggregate((n, r) => n.Value > r.Value ? n : r).Key;
            //    //double overallMaxAbundance = chargeWithAbundance[chargeAtMax];
            //    maxCharge = chargeAtMax;
            //}    
                
            
            //5.  populate dictionaries
            foreach (int charge in chargeStateList)
            {
                double monoMass = 0;
                foreach (IqGlyQResult n in childrenOfTheScan)
                {
                    if (n.ToChild.FragmentObservedIsotopeProfile != null)
                        monoMass = n.ToChild.FragmentObservedIsotopeProfile.MonoIsotopicMass;
                        //monoMass = n.ToChild.TargetFragment.MonoMassTheor;//.FragmentObservedIsotopeProfile.MonoIsotopicMass;
                    break;
                }
                globalResultForThisScan.ToChild.ChargeMonoMass.Add(charge, monoMass);

                //double area = (from n in childrenOfTheScan select n.ToChild.FragmentFitAbundance).FirstOrDefault();//this is the area of the good hits.  abundance is post populate and is not an area
                //double area = (from n in childrenOfTheScan where n.ToChild.TypeOfResultTargetOrModifiedTarget == TypeOfResult.Target select n.ToChild.FragmentFitAbundance).FirstOrDefault();//this is the area of the good hits.  abundance is post populate and is not an area
                double area = 0;
                float msArea = 0;
                List<float> miniPileOfTargetsMS = (from n in childrenOfTheScan where n.ToChild.TypeOfResultTargetOrModifiedTarget == TypeOfResult.Target select n.ToChild.FragmentFitAbundance).ToList();
                foreach (float miniPileOfTarget in miniPileOfTargetsMS)
                {
                    msArea += miniPileOfTarget;
                }

                List<double> miniPileOfTargets = (from n in childrenOfTheScan where n.ToChild.TypeOfResultTargetOrModifiedTarget == TypeOfResult.Target select n.ToChild.GlobalAggregateAbundance).ToList();
                foreach (float miniPileOfTarget in miniPileOfTargets)
                {
                    area += miniPileOfTarget;
                }

                globalResultForThisScan.ToChild.ChargeMsAbundance.Add(charge, msArea);
                globalResultForThisScan.ToChild.ChargeArea.Add(charge, area);
            }

            

            //6.  find max correlaion value to represent the feature
            if (childrenOfTheScan != null)
            {
                List<double> correlationScoreList = (from n in childrenOfTheScan select n.ToChild.CorrelationScore).ToList();
                if (correlationScoreList != null && correlationScoreList.Count > 0)
                {
                    if (correlationScoreList.Max() > globalResultForThisScan.ToChild.CorrelationScore)
                    {
                        double max = double.MinValue;
                        string assiciatedDifferenceName = "";

                        foreach (IqGlyQResult d in childrenOfTheScan)
                        {
                            if (d.ToChild.CorrelationScore > max)
                            {
                                max = d.ToChild.CorrelationScore;
                                assiciatedDifferenceName = d.ToChild.TargetParent.DifferenceName;


                            }
                        }

                        globalResultForThisScan.ToChild.TargetParent.DifferenceName = assiciatedDifferenceName;
                        globalResultForThisScan.ToChild.CorrelationScore = max;

                    }
                }
            }
        }


        //this is good
        protected override IqResult CreateIQResult(IqTarget target)
        {
            IqResult result = new IqGlyQResult(target);
            return result;
        }

        //this is good
        public override DeconTools.Workflows.Backend.FileIO.ResultExporter CreateExporter()
        {
            InSourceFragmentExporter exporter = new InSourceFragmentExporter();

            return exporter;

        }



    }
}