using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Backend.Core;
using DeconTools.Backend.ProcessingTasks.ChromatogramProcessing;
using DeconTools.Utilities;
using DeconTools.Workflows.Backend.Core;
using GetPeaks_DLL.Functions;
using IQGlyQ.Enumerations;
using IQGlyQ.Exporter;
using IQGlyQ.Objects;
using IQGlyQ.Processors;
using IQGlyQ.Results;
using IQGlyQ.TargetGenerators;
using PNNLOmics.Data.Constants;
using PNNLOmics.Data;

namespace IQGlyQ.Workflows
{
    public class ParentIQWorkflowWithChargeCorrelation : IqWorkflow
    {
        /// <summary>
        /// for coorelating between fragment EIC notches
        /// </summary>
        protected ChromatogramCorrelatorBase _chromatogramCorrelator;

        private FragmentedTargetedWorkflowParametersIQ _workflowParameters;
        #region Constructors

        public ParentIQWorkflowWithChargeCorrelation(Run run, FragmentedTargetedWorkflowParametersIQ parameters)
            : base(run, parameters)
        {

            InitializeWorkflow();

        }

        public ParentIQWorkflowWithChargeCorrelation(FragmentedTargetedWorkflowParametersIQ parameters)
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

            _workflowParameters = (FragmentedTargetedWorkflowParametersIQ)this.WorkflowParameters;

            var lcProcessor = new ProcessorChromatogram(_workflowParameters.LCParameters);
            _chromatogramCorrelator = new SimplePeakCorrelator(Run, _workflowParameters, _workflowParameters.MinRelativeIntensityForChromCorrelator, lcProcessor);
        }

        public override TargetedWorkflowParameters WorkflowParameters { get; set; }

        public override void Execute(IqResult result)
        {
            SimplePeakCorrelator correlator = (SimplePeakCorrelator) _chromatogramCorrelator;

            double chargeStateCorrelationGroupingCuttoff = _workflowParameters.CorrelationScoreCuttoff;//0.7 to 0.85

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
                //List<IqGlyQResult> validGlyQiqChildrenTargetsAllFitScores = (from n in validGlyQiqChildren where n.ToChild.TypeOfResultTargetOrModifiedTarget == TypeOfResult.Target select n).ToList();

                    //all results above fit score cuttoff
                //List<IqGlyQResult> validGlyQiqChildrenTargets = (from n in validGlyQiqChildrenTargetsAllFitScores where n.FitScore < WorkflowParameters.FitScoreCuttoff select n).ToList();
                List<IqGlyQResult> validGlyQiqChildrenTargets = (from n in validGlyQiqChildren where n.ToChild.TypeOfResultTargetOrModifiedTarget == TypeOfResult.Target select n).ToList();
                List<IqGlyQResult> validGlyQiqChildrenModfied = (from n in validGlyQiqChildren where n.ToChild.TypeOfResultTargetOrModifiedTarget == TypeOfResult.Modified select n).ToList();

                validGlyQiqChildren = validGlyQiqChildren.OrderBy(n => n.ToChild.TargetFragment.ScanLCTarget).ToList();
                
                List<IqGlyQResult> baseIsomers = new List<IqGlyQResult>();
                //label origiional targets

                WorkflowUtilities.PrintHits(result);

                //make summarized result that contains details overall.  This will have 0 charge and be identified by Flobalresult
                IqGlyQResult baseResult = new IqGlyQResult(result.Target);
                baseResult.ToChild.GlobalResult = true;

                //3.  find a unique scan list.  we may want to cluster these together so features from different charge states are to tegher.  a +1 peak may be 3 scans from a +2 peak
                //we restrict to children only to precent double counting.  targets hit on child EIC are the reference we use
                List<int> scanList = (from n in validGlyQiqChildren where n.ToChild.TypeOfResultParentOrChildDifferenceApproach == EnumerationParentOrChild.ChildrenOnly select n.ToChild.TargetFragment.ScanLCTarget).Distinct().ToList();
                scanList.Sort();

                List<double> scanListMaxAbundanceFitScores;
                List<double> scanListMaxAbundanceLmFit;
                List<float> msAbundances;
                List<float> scanListMaxAbundances = SetMaxAbundancePerDistinctScan(validGlyQiqChildren, scanList, out scanListMaxAbundanceFitScores, out scanListMaxAbundanceLmFit, out msAbundances);



                #region 1 set up ScanCharacteristics

                List<ScanCharacteristics> detailedScanList = new List<ScanCharacteristics>();
                int numberOfDistinctScans = scanList.Count;
                for (int i = 0; i < numberOfDistinctScans; i++)
                {
                    ScanCharacteristics scanInfo = new ScanCharacteristics(scanList[i], scanListMaxAbundances[i], scanListMaxAbundanceFitScores[i], scanListMaxAbundanceLmFit[i], msAbundances[i]);
                    if(i==0)//first scan
                    {
                        scanInfo.isFirstScan = true;
                        scanInfo.ClosestLowerScan = -1;
                        if(numberOfDistinctScans>1)
                        {
                            scanInfo.ClosestUpperScan = scanList[i+1];
                        }

                    }
                    if (i == numberOfDistinctScans-1)//last scan
                    {
                        scanInfo.isLastScan = true;
                        scanInfo.ClosestUpperScan = -1;
                        if (numberOfDistinctScans > 1)
                        {
                            scanInfo.ClosestLowerScan = scanList[i -1];
                        }
                    }
                    if(scanInfo.isFirstScan==false && scanInfo.isLastScan==false)
                    {
                        scanInfo.ClosestLowerScan = scanList[i - 1];
                        scanInfo.ClosestUpperScan = scanList[i + 1];
                    }
                    scanInfo.isConsolidated = false;
                    detailedScanList.Add(scanInfo);
                }


                detailedScanList = detailedScanList.OrderByDescending(n => n.MaxIntegratedAbundance).ToList();

                #endregion

                #region 2.  work down from max abundance and check along the way if peaks have been used up.  correlate tightly spaced peaks across charge states

                foreach (ScanCharacteristics scanCharacteristic in detailedScanList)
                {
                    Console.WriteLine("-CenterScan is " + scanCharacteristic.CenterScan);
                    
                    //a check to see if assigned
                    if (scanCharacteristic.isConsolidated == false)
                    {
                        bool iscurrentUpperScanPossible = true;
                        bool iscurrentLowerScanPossible = true;
                        if (scanCharacteristic.isFirstScan)
                        {
                            iscurrentUpperScanPossible = !(from n in detailedScanList where n.CenterScan == scanCharacteristic.ClosestUpperScan select n.isConsolidated).FirstOrDefault();
                            iscurrentLowerScanPossible = false;
                        }
                        if (scanCharacteristic.isLastScan)
                        {
                            iscurrentLowerScanPossible = !(from n in detailedScanList where n.CenterScan == scanCharacteristic.ClosestLowerScan select n.isConsolidated).FirstOrDefault();
                            iscurrentUpperScanPossible = false;
                        }
                        if (scanCharacteristic.isLastScan == false && scanCharacteristic.isFirstScan == false)
                        {
                            iscurrentUpperScanPossible = !(from n in detailedScanList where n.CenterScan == scanCharacteristic.ClosestUpperScan select n.isConsolidated).FirstOrDefault();
                            iscurrentLowerScanPossible = !(from n in detailedScanList where n.CenterScan == scanCharacteristic.ClosestLowerScan select n.isConsolidated).FirstOrDefault();
                        }

                        Console.WriteLine(scanCharacteristic.ClosestLowerScan + " is possible? " + iscurrentLowerScanPossible + " and " + scanCharacteristic.ClosestUpperScan + " is possible? " + iscurrentUpperScanPossible);


                        //b correlate those that have not been consolidated
                        if (iscurrentLowerScanPossible) //false means it has not been assigned yet
                        {
                            int centerScan = scanCharacteristic.CenterScan;
                            int candidateScan = scanCharacteristic.ClosestLowerScan;
                            double resultCorr = CorrelateModelsFromDifferentScans(correlator, validGlyQiqChildren, candidateScan, centerScan);

                            Console.WriteLine("base " + centerScan + " correlates with lower " + candidateScan + " @ " + resultCorr);

                            if (resultCorr > chargeStateCorrelationGroupingCuttoff)
                            {
                                var update =
                                    (from n in detailedScanList where n.CenterScan == candidateScan select n).FirstOrDefault();
                                if (update != null)
                                {
                                    update.isConsolidated = true;
                                    Console.WriteLine("   ^^" + update.CenterScan + " has been consolidated and removed");
                                    scanCharacteristic.scansToConsolidate.Add(centerScan); //add to larger one
                                    scanCharacteristic.scansToConsolidate.Add(candidateScan);
                                    scanCharacteristic.ChargeStateCorrelation = resultCorr;
                                }
                            }
                        }

                        if (iscurrentUpperScanPossible)
                        {
                            int centerScan = scanCharacteristic.CenterScan;
                            int candidateScan = scanCharacteristic.ClosestUpperScan;
                            double resultCorr = CorrelateModelsFromDifferentScans(correlator, validGlyQiqChildren, candidateScan, centerScan);

                            Console.WriteLine("base scan " + centerScan + " correlates with upper scan " + candidateScan + " with a score of " + resultCorr);

                            if (resultCorr > chargeStateCorrelationGroupingCuttoff)
                            {
                                var update = (from n in detailedScanList where n.CenterScan == candidateScan select n).FirstOrDefault();
                                if (update != null)
                                {
                                    update.isConsolidated = true;
                                    Console.WriteLine("   ^^" + update.CenterScan + " has been consolidated and removed from further examination");
                                    scanCharacteristic.scansToConsolidate.Add(centerScan); //add to larger one
                                    scanCharacteristic.scansToConsolidate.Add(candidateScan);
                                }

                            }
                        }

                        Console.WriteLine(Environment.NewLine);

                    }
                }

                detailedScanList = detailedScanList.OrderBy(n => n.CenterScan).ToList();

                foreach (var scanCharacteristicse in detailedScanList)
                {
                    Console.WriteLine("Scan " + scanCharacteristicse.CenterScan + " is Consolidated?" + scanCharacteristicse.isConsolidated);
                    if(scanCharacteristicse.scansToConsolidate.Count>0)
                    {
                        string writeLine = "+Scans ";
                        foreach (var scan in scanCharacteristicse.scansToConsolidate)
                        {
                            writeLine += scan + " ";
                        }
                        Console.WriteLine(writeLine + " will be added and tacked to " + scanCharacteristicse.CenterScan);
                    }
                }

                #endregion

                #region 3.  go scan by scan...

                for (int i = 0; i < numberOfDistinctScans;i++ )
                {

                    
                    
                    if (detailedScanList[i].isConsolidated == false)//this means it is that the charge states associated with this scan are clean.  Else, the charge states from multiple scans needs to be considered
                    {
                        int scanCurrent = detailedScanList[i].CenterScan;

                        //1.  make a global result
                        IqTargetUtilities util = new IqTargetUtilities();
                        IqTarget parentTargetcopy = util.Clone(result.Target);

                        IqGlyQResult isomerResult = new IqGlyQResult(parentTargetcopy);
                        //IqGlyQResult isomerResult = new IqGlyQResult(result.Target);
                        isomerResult.ToChild.GlobalResult = true;
                        isomerResult.ToChild.TargetFragment.ScanLCTarget = scanCurrent;

                        //List<IqResult> tempCastGlyQIQResult = result.ChildResults().ToList();
                        //initial population of values
                        isomerResult.ToChild.FragmentObservedIsotopeProfile.Score = detailedScanList[i].FitScoreFromMaxAbundance;
                        isomerResult.ToChild.LMOptimizationCorrelationRsquared = detailedScanList[i].LMFitFromMaxAbundance;
                        //isomerResult.ToChild.ChargeStateCorrelation = detailedScanList[i].ChargeStateCorrelation;

                        //2. find actual scan for global result.  this makes sure the scan exists in the dataset
                        isomerResult.LCScanSetSelected = new ScanSet(scanCurrent);

                        //3.  pull all results that go in this scanthis has targets and connecting modifieds all piled up

                        //this just has targets
                        List<IqGlyQResult> scanResultsListFragments = new List<IqGlyQResult>();
                        List<IqGlyQResult> scanResultsList = new List<IqGlyQResult>();
                        List<IqGlyQResult> scanResultsListModfied = new List<IqGlyQResult>();

                        double chargeStateCorrelation = 0;
                        double selectedIsoFitScore = 0;
                        double selectedLMFit = 0;
                        if(detailedScanList[i].scansToConsolidate.Count>0)//this means the scans have been identified that need to be consolidated  1000 and 1002 for example. 1000 came from +2 and 1001 came from +1  
                        {
                            foreach(int scan in detailedScanList[i].scansToConsolidate)
                            {
                                scanResultsListFragments.AddRange((from n in validGlyQiqChildrenTargets where n.ToChild.TargetFragment.ScanLCTarget == scan select n).ToList());//perhaps add a range of scansets
                                scanResultsList.AddRange((from n in validGlyQiqChildren where n.ToChild.TargetFragment.ScanLCTarget == scan select n).ToList());//perhaps add a range of scansets
                                scanResultsListModfied.AddRange((from n in validGlyQiqChildrenModfied where n.ToChild.TargetFragment.ScanLCTarget == scan select n).ToList());
                                
                                //chargeStateCorrelation = detailedScanList[i].ChargeStateCorrelation;
                                //selectedIsoFitScore = detailedScanList[i].FitScoreFromMaxAbundance;
                                //selectedLMFit = detailedScanList[i].LMFitFromMaxAbundance;
                            }

                            //update results with best hit
                            //the first one is from close spaced lc peaks across charge states
                            //ovverwite initial values with better ones
                            //isomerResult.ToChild.ChargeStateCorrelation = chargeStateCorrelation;
                            //isomerResult.ToChild.FragmentObservedIsotopeProfile.Score = selectedIsoFitScore;
                            //isomerResult.ToChild.LMOptimizationCorrelationRsquared = selectedLMFit;
                        }
                        else
                        {
                            scanResultsListFragments = (from n in validGlyQiqChildrenTargets where n.ToChild.TargetFragment.ScanLCTarget == scanCurrent select n).ToList();
                            scanResultsList = (from n in validGlyQiqChildren where n.ToChild.TargetFragment.ScanLCTarget == scanCurrent select n).ToList();
                            scanResultsListModfied = (from n in validGlyQiqChildrenModfied where n.ToChild.TargetFragment.ScanLCTarget == scanCurrent select n).ToList();
                        }

                        Console.WriteLine("in this scan, we have " + scanResultsListFragments.Count + " targets, " + scanResultsListModfied.Count + " modified out of a total of " + scanResultsList.Count);

                        Console.WriteLine("-Targets");
                        PrintIqGlyQResultList(scanResultsListFragments);

                        Console.WriteLine("-Modified");
                        PrintIqGlyQResultList(scanResultsListModfied);

                        //contains multiple charge states as well as "targets" and "modifieds"
                        //List<IqGlyQResult> childrenOfTheScan = (from n in scanResultsList where n.ToChild.TypeOfResultParentOrChildDifferenceApproach == EnumerationParentOrChild.ChildrenOnly select n).ToList();
                        //List<IqGlyQResult> parentsfTheScan = (from n in scanResultsList where n.ToChild.TypeOfResultParentOrChildDifferenceApproach == EnumerationParentOrChild.ParentsOnly select n).ToList();


                        //4.  summarize feature information

                        IqGlyQResult globalResultForThisScanChild = new IqGlyQResult(result.Target);
                        List<IqGlyQResult> childScanResultsListFragments = (from n in scanResultsListFragments where n.ToChild.TypeOfResultParentOrChildDifferenceApproach == EnumerationParentOrChild.ChildrenOnly select n).ToList();
                        List<IqGlyQResult> childScanResultsListModfied = (from n in scanResultsListModfied where n.ToChild.TypeOfResultParentOrChildDifferenceApproach == EnumerationParentOrChild.ChildrenOnly select n).ToList();

                        SummarizeFeature_Mono_Area_Corr(childScanResultsListFragments, childScanResultsListModfied, ref globalResultForThisScanChild);

                        isomerResult.ToChild.ChargeMsAbundance = globalResultForThisScanChild.ToChild.ChargeMsAbundance;
                        isomerResult.ToChild.ChargeArea = globalResultForThisScanChild.ToChild.ChargeArea;
                        isomerResult.ToChild.ChargeMonoMass = globalResultForThisScanChild.ToChild.ChargeMonoMass;

                        IqGlyQResult globalResultForThisScanParent = new IqGlyQResult(result.Target);
                        List<IqGlyQResult> parentScanResultsListFragments = (from n in scanResultsListFragments where n.ToChild.TypeOfResultParentOrChildDifferenceApproach == EnumerationParentOrChild.ParentsOnly select n).ToList();
                        List<IqGlyQResult> parentScanResultsListModfied = (from n in scanResultsListModfied where n.ToChild.TypeOfResultParentOrChildDifferenceApproach == EnumerationParentOrChild.ParentsOnly select n).ToList();

                        SummarizeFeature_Mono_Area_Corr(parentScanResultsListFragments, parentScanResultsListModfied, ref globalResultForThisScanParent);


                        Console.WriteLine("SummarizeFeature_Mono_Area_Corr Passed");


                        //5.  charge correlations
                        ////the first one is from close spaced lc peaks across charge states
                        //isomerResult.ToChild.ChargeStateCorrelation = chargeStateCorrelation;

                        

                        //the second one is for exactly ontop charge states//currentyl we can only correlate 2 charge states
                        if (childScanResultsListFragments.Count>1)
                        {
                            //we have multiple targets hitting in this scan
                            int centerScan = scanCurrent;
                            IqGlyQResult correlate1 = childScanResultsListFragments[0];
                            IqGlyQResult correlate2 = childScanResultsListFragments[1];
                            double resultCorr = CorrelateModelsFromSameScans(correlator, correlate1, correlate2, centerScan);
                            isomerResult.ToChild.ChargeStateCorrelation = resultCorr;

                            if (correlate1.ToChild.GlobalAggregateAbundance > correlate2.ToChild.GlobalAggregateAbundance)
                            {
                                isomerResult.ToChild.FragmentObservedIsotopeProfile.Score = correlate1.ToChild.FragmentObservedIsotopeProfile.Score;
                                isomerResult.ToChild.LMOptimizationCorrelationRsquared = correlate1.ToChild.LMOptimizationCorrelationRsquared;
                            }
                            else
                            {
                                isomerResult.ToChild.FragmentObservedIsotopeProfile.Score = correlate2.ToChild.FragmentObservedIsotopeProfile.Score;
                                isomerResult.ToChild.LMOptimizationCorrelationRsquared = correlate2.ToChild.LMOptimizationCorrelationRsquared;
                            }
                        }



                        Console.WriteLine("CorrelateModelsFromSameScans Passed");



                        //6.  charge state range
                        if (globalResultForThisScanChild != null && globalResultForThisScanChild.ToChild.ChargeMonoMass.Count > 0)//dictionary has multiple monos
                        {
                            //isomerResult.ToChild.GlobalChargeStateMin = globalResultForThisScanChild.ToChild.ChargeMonoMass.Keys.First();
                            //isomerResult.ToChild.GlobalChargeStateMax = globalResultForThisScanChild.ToChild.ChargeMonoMass.Keys.Last();

                            isomerResult.ToChild.GlobalChargeStateMin = globalResultForThisScanChild.ToChild.GlobalChargeStateMin;
                            isomerResult.ToChild.GlobalChargeStateMax = globalResultForThisScanChild.ToChild.GlobalChargeStateMax;

                            //this is where we put the charge state.  this is copied to "Charge" in the post run
                            //you can put the best charge for the global result here is the best way to do it
                            //we need the condition that 
                            //1.  EnumerationParentOrChild.ChildrenOnly (childrenOfTheScan)
                            //2.  FragmentArea>0
                            //3.  Not  EnumerationError.NoFragment (!EnumerationError.NoFragment).  bool fragmentPassesProcessing = false  No parent and Success are ok
                            IqResult baseCallTarget = isomerResult;

                            IqGlyQResult mostAbundantFromChildrenInScan = SelectMostIntenseIqGlyQResult(childScanResultsListFragments);
                            IqResult baseMostAbundantFromChildrenInScan = mostAbundantFromChildrenInScan;

                            if (mostAbundantFromChildrenInScan != null)//we were able to find the best one from the targets
                            {
                                int maxCharge = mostAbundantFromChildrenInScan.ToChild.TargetFragment.ChargeState;
                                float maxAbundance = mostAbundantFromChildrenInScan.Abundance;
                                
                                isomerResult.ToChild.TargetFragment.ChargeState = maxCharge;
                                baseCallTarget.Abundance = maxAbundance;
                               
                                baseCallTarget.MZObs = baseMostAbundantFromChildrenInScan.MZObs;
                                baseCallTarget.MonoMassObs = baseMostAbundantFromChildrenInScan.MonoMassObs;
                                baseCallTarget.ElutionTimeObs = baseMostAbundantFromChildrenInScan.ElutionTimeObs;

                               
                            }
                            else
                            {
                                //this is the case when all the fragment peaks do not have an abundance iqGlyQResult.ToChild.FragmentFitAbundance = -1 etc.
                                
                                Console.WriteLine("We failed to find a target with an abundance for reporting. looking at the modified is not the way to go"  );
                                //Console.ReadKey();
                                //IqGlyQResult mostAbundantInScan = SelectMostIntenseIqGlyQResult(scanResultsList);//children are not included by filter above
                                //IqResult baseMostAbundantInScan = mostAbundantInScan;
                                //if (mostAbundantInScan != null)
                                //{
                                //    int maxCharge = mostAbundantInScan.ToChild.TargetFragment.ChargeState;
                                //    float maxAbundance = mostAbundantInScan.ToChild.FragmentFitAbundance;

                                //    isomerResult.ToChild.TargetFragment.ChargeState = maxCharge;
                                //    baseCallTarget.Abundance = maxAbundance;
                                //    if (mostAbundantInScan.ToChild.FragmentObservedIsotopeProfile != null)
                                //    {
                                //        baseCallTarget.MZObs = mostAbundantInScan.ToChild.FragmentObservedIsotopeProfile.MonoPeakMZ;
                                //        baseCallTarget.MonoMassObs = mostAbundantInScan.ToChild.FragmentObservedIsotopeProfile.MonoIsotopicMass;
                                //    }
                                //}
                                //else//when nothing is found
                                {
                                    
                                    isomerResult.ToChild.TargetFragment.ChargeState = isomerResult.ToChild.GlobalChargeStateMax;//these have the same effect.  both charge and framgentcharge
                                    baseCallTarget.Abundance = 0;
                                    if (scanResultsListFragments != null && scanResultsListFragments.Count > 0)
                                    {
                                        baseCallTarget.MZObs = scanResultsListFragments[0].ToChild.FragmentObservedIsotopeProfile.MonoPeakMZ;
                                        baseCallTarget.MonoMassObs = scanResultsListFragments[0].ToChild.FragmentObservedIsotopeProfile.MonoIsotopicMass;
                                        baseCallTarget.ElutionTimeObs = -55;
                                    }
                                    else
                                    {
                                        //complete failure
                                        baseCallTarget.MZObs = -77;
                                        baseCallTarget.MonoMassObs = -77;
                                    }
                                }
                            }
                        }

                        Console.WriteLine("ChargeStateRange Passed");

                        //5.  mass and error only pulled from "fragment" results aka targets.  calculate average mass and total abundance and ppm
                        int avarageMassCounter = 0;
                        double sumExperimentalMonoMass = 0;
                        double sumArea = 0;
                        float sumMSArea = 0;
                        if (scanResultsListFragments != null && scanResultsListFragments.Count > 0)
                        {
                            //5a.  average mono mass
                            
                            Console.WriteLine("Averaging monoisotoipic Masses");
                            foreach (IqGlyQResult searchForMonoMassResult in scanResultsListFragments)
                            {
                                //double monoMass = searchForMonoMassResult.MonoMassObs;
                                double monoMass = searchForMonoMassResult.ToChild.FragmentObservedIsotopeProfile.MonoIsotopicMass;
                                if (monoMass > 0)
                                {
                                    Console.WriteLine("averaging..." + monoMass);
                                    sumExperimentalMonoMass += monoMass;
                                    avarageMassCounter++;
                                }
                            }


                            isomerResult.ToChild.AverageMonoMass = 0;
                            isomerResult.ToChild.PPMError = -99;

                            //5b.  calibrate mono and calculate new ppm
                            if (avarageMassCounter > 0)
                            {
                                isomerResult.ToChild.AverageMonoMass = sumExperimentalMonoMass / avarageMassCounter;

                                Console.WriteLine("Average Mono found:  " + isomerResult.ToChild.AverageMonoMass);
                                //calibrate MonoMassTheor here
                                double existingTheoreticalMonoMass = isomerResult.ToChild.TargetFragment.MonoMassTheor;//this is for MZ shift only because Mono is allready calibrated

                                double calibratedTheoreticalMono;//for new ppm calculation
                                double nonCalibratedMono = isomerResult.ToChild.TargetFragment.MonoMassTheor;
                                if (WorkflowParameters.MSParameters.IsoParameters.ToMassCalibrate)
                                {

                                    int charge = isomerResult.ToChild.TargetFragment.ChargeState;
                                    //move the mz and then move it back to get calibrated mono

                                    
                                    double calibrationOffsetMZ = WorkflowParameters.MSParameters.IsoParameters.DeltaMassCalibrationMZ;
                                    
                                    double calibratedTheoreticalMZ;
                                    MassCalibration.ConvertMonoToCalMonoAndMZ(nonCalibratedMono, charge, calibrationOffsetMZ, out calibratedTheoreticalMono, out calibratedTheoreticalMZ);
                                }
                                else
                                {
                                    calibratedTheoreticalMono = nonCalibratedMono;
                                }
                                Console.WriteLine("Calibrating Theoretcial Mono.  The average from the data is.." + isomerResult.ToChild.AverageMonoMass);
                                Console.WriteLine("old Mono mass theoretical was: " + isomerResult.ToChild.TargetFragment.MonoMassTheor + " calibrated is now: " + calibratedTheoreticalMono);

                                double oldPPM = ErrorCalculator.PPM(isomerResult.ToChild.AverageMonoMass, isomerResult.ToChild.TargetFragment.MonoMassTheor);

                                isomerResult.ToChild.PPMError = ErrorCalculator.PPM(isomerResult.ToChild.AverageMonoMass, calibratedTheoreticalMono);

                                Console.WriteLine(" the old ppm was " + oldPPM + "and the calibrated PPM is now ... " + isomerResult.ToChild.PPMError);
                                //isomerResult.ToChild.PPMError = ErrorCalculator.PPM(isomerResult.ToChild.AverageMonoMass, isomerResult.ToChild.TargetFragment.MonoMassTheor);
                            }

                            //5c  summed abundance.  also find max abundance so we can pull the associated abundance from ms
                            
                            
                            foreach (var selectedKey in isomerResult.ToChild.ChargeArea.Keys.ToList())
                            {
                                double area = isomerResult.ToChild.ChargeArea[selectedKey];
                                float areaMS = isomerResult.ToChild.ChargeMsAbundance[selectedKey];

                                Console.WriteLine("adding area..." + area);
                                sumArea += area;
                                sumMSArea += areaMS;
                            }

                            isomerResult.ToChild.GlobalAggregateAbundance = sumArea;
                            isomerResult.ToChild.GlobalAggregateMSAbundance = sumMSArea;
                            //}

                            //foreach (KeyValuePair<int, double> areaFromChild in isomerResult.ToChild.ChargeArea)
                            //{
                            //    //double monoMass = searchForMonoMassResult.MonoMassObs;
                            //    double area = areaFromChild.Value;
                                
                            //    if (area > 0)
                            //    {
                            //        Console.WriteLine("adding area..." + area);
                            //        sumArea += area;
                            //        sumMSArea += isomerResult.ToChild.ChargeMsAbundance[areaFromChild.Key];
                                    
                            //    }
                            //    isomerResult.ToChild.GlobalAggregateAbundance = sumArea;
                            //    isomerResult.ToChild.GlobalAggregateMSAbundance = sumMSArea;
                                

                            //}

                            ////6.  report summed abundance
                            //double summedAbundance = 0;
                            //foreach (KeyValuePair<int, double> keyvalue in isomerResult.ToChild.ChargeArea)
                            //{
                            //    summedAbundance += keyvalue.Value;
                            //}
                            //isomerResult.ToChild.FragmentFitAbundance = Convert.ToInt64(summedAbundance);

                        }
                        else//where else can this live?
                        {
                            isomerResult.ToChild.AverageMonoMass = -5;
                            isomerResult.ToChild.PPMError = -99;
                        }

                        //6.  find optimal scan

                        Console.WriteLine("Start Logic");

                        //7.  logic
                        bool hitAtLowerMass = globalResultForThisScanChild.ToChild.CorrelationScore > fragmentCuttoffThreshold;
                        bool hitAtHigherMass = globalResultForThisScanParent.ToChild.CorrelationScore > fragmentCuttoffThreshold;


                        if (hitAtLowerMass && hitAtHigherMass)
                        {
                            //This is a glycan fragment
                            SummarizeFeature_Mono_Area_Corr(childScanResultsListFragments, childScanResultsListModfied, ref isomerResult);
                            isomerResult.ToChild.FinalDecision = EnumerationFinalDecision.ValidatedGlycanFragment;
                        }

                        if (hitAtLowerMass && !hitAtHigherMass)
                        {
                            //This is a correct Glycan Yay!
                            SummarizeFeature_Mono_Area_Corr(childScanResultsListFragments, childScanResultsListModfied, ref isomerResult);
                            isomerResult.ToChild.FinalDecision = EnumerationFinalDecision.CorrectGlycan;
                        }

                        if (!hitAtLowerMass && hitAtHigherMass)
                        {
                            //the parent (fragment + hexose etc.) is a future Target
                            SummarizeFeature_Mono_Area_Corr(childScanResultsListFragments, childScanResultsListModfied, ref isomerResult);
                            isomerResult.ToChild.FinalDecision = EnumerationFinalDecision.FutureTarget;
                        }

                        if (!hitAtLowerMass && !hitAtHigherMass)
                        {
                            //This is a random hit that could be good but we can't verify it
                            SummarizeFeature_Mono_Area_Corr(childScanResultsListFragments, childScanResultsListModfied, ref isomerResult);
                            isomerResult.ToChild.FinalDecision = EnumerationFinalDecision.NonValidatedHit;
                        }

                        Console.WriteLine("Start report summed abundance");

                        
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
                }

                #endregion
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

                Console.WriteLine("Completed parent workflow");

            }
        }

        

        private static void PrintIqGlyQResultList(List<IqGlyQResult> scanResultsListFragments)
        {
            foreach (var iqGlyQResult in scanResultsListFragments)
            {
                Console.WriteLine("Scan: " + iqGlyQResult.ToChild.TargetFragment.ScanLCTarget.ToString() +
                                  ", TargetType: " + iqGlyQResult.ToChild.TypeOfResultTargetOrModifiedTarget +
                                  ", Charge: " + iqGlyQResult.ToChild.FragmentObservedIsotopeProfile.ChargeState +
                                  ", Direction: " + iqGlyQResult.ToChild.TypeOfResultParentOrChildDifferenceApproach +
                                  Environment.NewLine +
                                  "  FragAbund: " + iqGlyQResult.ToChild.FragmentFitAbundance + " " +
                                  iqGlyQResult.ToChild.FragmentCharge +
                                  ", ParentAbund: " + iqGlyQResult.ToChild.ParentFitAbundance + " " +
                                  iqGlyQResult.ToChild.ParentCharge + Environment.NewLine);
            }
        }

        private static double CorrelateModelsFromDifferentScans(SimplePeakCorrelator correlator, List<IqGlyQResult> validGlyQiqChildren, int candidateScan, int centerScan)
        {
            List<IqGlyQResult> scanResultsListCenterScan = (from n in validGlyQiqChildren
                                                            where n.ToChild.TargetFragment.ScanLCTarget == centerScan && 
                                                                n.ToChild.TypeOfResultTargetOrModifiedTarget == TypeOfResult.Target && 
                                                                n.ToChild.TypeOfResultParentOrChildDifferenceApproach == EnumerationParentOrChild.ChildrenOnly
                                                            select n).ToList(); //perhaps add a range of scansets

            //select largest in center scan to correlate to
            IqGlyQResult baseToCorrelateTo = SelectMaxBaseToCorrelateTo(scanResultsListCenterScan);
            double resultCorr = 0;
            if (baseToCorrelateTo != null)
            {
                FragmentResultsObjectHolderIq baseCoefficentsBox = GatherCoefficients(baseToCorrelateTo);

                //select hits that go along with neighboring scans 

                List<IqGlyQResult> resultsListFromLowerScan = (from n in validGlyQiqChildren
                                                               where n.ToChild.TargetFragment.ScanLCTarget == candidateScan && 
                                                               n.ToChild.TypeOfResultTargetOrModifiedTarget == TypeOfResult.Target &&
                                                               n.ToChild.TypeOfResultParentOrChildDifferenceApproach == EnumerationParentOrChild.ChildrenOnly
                                                               select n).ToList(); //perhaps add a range of scansets

                //coorelate with base
                IqGlyQResult candidateToCorrelateTo = SelectMaxBaseToCorrelateTo(resultsListFromLowerScan);

                if (candidateToCorrelateTo != null)
                {
                    FragmentResultsObjectHolderIq candidateCoefficentBox = GatherCoefficients(candidateToCorrelateTo);

                    //correlate fit data
                    int scanWindow = 100;
                    List<XYData> baseModel = CorrelateWorkflow.SelectXYDataToCorrelate(centerScan - scanWindow, centerScan + scanWindow, baseCoefficentsBox);
                    List<XYData> candidatemodel = CorrelateWorkflow.SelectXYDataToCorrelate(centerScan - scanWindow, centerScan + scanWindow, candidateCoefficentBox);

                    ChromCorrelationData chromeResults = new ChromCorrelationData();

                    chromeResults = correlator.CorrelateDataXYNonInteger(baseModel, candidatemodel, centerScan - scanWindow, centerScan + scanWindow);
                    resultCorr = Convert.ToDouble(chromeResults.CorrelationDataItems[0].CorrelationRSquaredVal);
                }
                else
                {
                    resultCorr = -1;
                    //missing base data.  this could be a parent scan
                }

                //if it correlates, remove it from the list
                return resultCorr;
            }
            resultCorr = -1;//missing base data.  this could be a parent scan
            return resultCorr;
        }

        private static double CorrelateModelsFromSameScans(SimplePeakCorrelator correlator, IqGlyQResult validGlyQiqChildren1, IqGlyQResult validGlyQiqChildren2, int centerScan)
        {
            //select largest in center scan to correlate to
            IqGlyQResult baseToCorrelateTo = validGlyQiqChildren1;
            double resultCorr = 0;
            if (baseToCorrelateTo != null)
            {
                FragmentResultsObjectHolderIq baseCoefficentsBox = GatherCoefficients(baseToCorrelateTo);


                //coorelate with base
                IqGlyQResult candidateToCorrelateTo = validGlyQiqChildren2;

                if (candidateToCorrelateTo != null)
                {
                    FragmentResultsObjectHolderIq candidateCoefficentBox = GatherCoefficients(candidateToCorrelateTo);

                    //correlate fit data
                    int scanWindow = 100;
                    List<XYData> baseModel = CorrelateWorkflow.SelectXYDataToCorrelate(centerScan - scanWindow, centerScan + scanWindow, baseCoefficentsBox);
                    List<XYData> candidatemodel = CorrelateWorkflow.SelectXYDataToCorrelate(centerScan - scanWindow, centerScan + scanWindow, candidateCoefficentBox);

                    ChromCorrelationData chromeResults = new ChromCorrelationData();

                    chromeResults = correlator.CorrelateDataXYNonInteger(baseModel, candidatemodel, centerScan - scanWindow, centerScan + scanWindow);
                    resultCorr = Convert.ToDouble(chromeResults.CorrelationDataItems[0].CorrelationRSquaredVal);
                }
                else
                {
                    resultCorr = -1;
                    //missing base data.  this could be a parent scan
                }

                //if it correlates, remove it from the list
                return resultCorr;
            }
            resultCorr = -1;//missing base data.  this could be a parent scan
            return resultCorr;
        }

        private static FragmentResultsObjectHolderIq GatherCoefficients(IqGlyQResult baseToCorrelateTo)
        {
            FragmentResultsObjectHolderIq baseCoefficentsBox = new FragmentResultsObjectHolderIq(new FragmentIQTarget());
            baseCoefficentsBox.CorrelationCoefficients = baseToCorrelateTo.ToChild.CorrelationCoefficients;
            return baseCoefficentsBox;
        }

        private static IqGlyQResult SelectMaxBaseToCorrelateTo(List<IqGlyQResult> scanResultsListCenterScan)
        {
            IqGlyQResult baseToCorrelateTo;
            if (scanResultsListCenterScan.Count > 0)
            {
                baseToCorrelateTo = scanResultsListCenterScan[0];
            }
            else
            {
                Console.WriteLine("fail");
                //baseToCorrelateTo = scanResultsListCenterScan[0];
                baseToCorrelateTo = null;
            }
            if (scanResultsListCenterScan.Count > 1)
            {
                baseToCorrelateTo = scanResultsListCenterScan[0];
                foreach (var iqGlyQResult in scanResultsListCenterScan)
                {
                    if (iqGlyQResult.ToChild.FragmentFitAbundance > baseToCorrelateTo.ToChild.FragmentFitAbundance)
                    {
                        baseToCorrelateTo = iqGlyQResult;
                    }
                }
            }

            return baseToCorrelateTo;
        }

        private static List<float> SetMaxAbundancePerDistinctScan(List<IqGlyQResult> validGlyQiqChildren, List<int> scanList, out List<double> scanListMaxAbundanceFitScores, out List<double> scanListMaxAbundanceLMFit, out List<float> msAbundances)
        {
            List<float> scanListMaxAbundances = new List<float>();
            scanListMaxAbundanceFitScores = new List<double>();
            scanListMaxAbundanceLMFit = new List<double>();
            msAbundances = new List<float>();
            int numberOfDistinctScans = scanList.Count;

            for (int i = 0; i < numberOfDistinctScans; i++)
            {
                int scanCurrent = scanList[i];
                List<IqGlyQResult> scanResultsList = (from n in validGlyQiqChildren where n.ToChild.TargetFragment.ScanLCTarget == scanCurrent select n).ToList();
                    //perhaps add a range of scansets
                float abundance = scanResultsList[0].ToChild.FragmentFitAbundance;
                float msAbundance = scanResultsList[0].Abundance;
                double currentFitScore = 0;
                if (scanResultsList[0].ToChild.FragmentObservedIsotopeProfile != null) currentFitScore = scanResultsList[0].ToChild.FragmentObservedIsotopeProfile.Score;
                
                double currentLMfit = scanResultsList[0].ToChild.LMOptimizationCorrelationRsquared;

                foreach (var iqGlyQResult in scanResultsList)
                {
                    if (iqGlyQResult.ToChild.FragmentFitAbundance > abundance)
                    {
                        abundance = iqGlyQResult.ToChild.FragmentFitAbundance;
                        msAbundance = iqGlyQResult.Abundance;
                        if (iqGlyQResult.ToChild.FragmentObservedIsotopeProfile != null) currentFitScore = scanResultsList[0].ToChild.FragmentObservedIsotopeProfile.Score;
                        currentLMfit = iqGlyQResult.ToChild.LMOptimizationCorrelationRsquared;
                    }
                }

                scanListMaxAbundances.Add(abundance);
                scanListMaxAbundanceFitScores.Add(currentFitScore);
                scanListMaxAbundanceLMFit.Add(currentLMfit);
                msAbundances.Add(msAbundance);
            }
            return scanListMaxAbundances;
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

        private static void SummarizeFeature_Mono_Area_Corr(List<IqGlyQResult> targets, List<IqGlyQResult> modified, ref IqGlyQResult globalResultForThisScan)
        {        
            //4.  summarize charge state and correlation values observed for the feature
            //List<int> chargeStateList = (from n in childrenOfTheScan select n.ToChild.TargetFragment.ChargeState).Distinct().ToList();
            //List<int> chargeStateList = (from n in targets where n.ToChild.TypeOfResultTargetOrModifiedTarget == TypeOfResult.Target select n.ToChild.TargetFragment.ChargeState).Distinct().ToList();//now we look at the charge state rangge of the targets only
            List<int> chargeStateList = (from n in targets select n.ToChild.TargetFragment.ChargeState).Distinct().ToList();//now we look at the charge state rangge of the targets only

            if (chargeStateList.Count > 0)
            {
                globalResultForThisScan.ToChild.GlobalChargeStateMax = chargeStateList.Max();
                globalResultForThisScan.ToChild.GlobalChargeStateMin = chargeStateList.Min();
            }

            //5.  populate dictionaries since we have multiple monos and multiple areas
            foreach (int charge in chargeStateList)
            {
                //add mass from first target.  averaging could be better if there is more than one target at this charge state
                double monoMass = 0;
                double area = 0;
                float msAbundance = 0;
                foreach (IqGlyQResult n in targets)
                {
                    //if (n.ToChild.FragmentObservedIsotopeProfile != null && n.ToChild.TypeOfResultTargetOrModifiedTarget == TypeOfResult.Target)
                    if (n.ToChild.FragmentObservedIsotopeProfile != null && n.ToChild.FragmentObservedIsotopeProfile.ChargeState == charge)
                    {
                        monoMass = n.ToChild.FragmentObservedIsotopeProfile.MonoIsotopicMass;
                        area = n.ToChild.FragmentFitAbundance;
                        msAbundance = n.Abundance;
                        if (globalResultForThisScan.ToChild.ChargeMonoMass.ContainsKey(charge))
                        {
                            Console.WriteLine("We are trying to stuff the mono dictionary");
                            Console.WriteLine("trying to add " + charge + "," + monoMass + Environment.NewLine);
                            foreach (KeyValuePair<int, double> values in globalResultForThisScan.ToChild.ChargeMonoMass)
                            {
                                Console.WriteLine("we already have " + values.Key + "," + values.Value);

                            }
                            //Console.ReadKey();
                        }
                        else
                        {
                            globalResultForThisScan.ToChild.ChargeMonoMass.Add(charge, monoMass);
                            
                        }

                        if (globalResultForThisScan.ToChild.ChargeArea.ContainsKey(charge))
                        {
                            Console.WriteLine("We are trying to stuff the charge dictionary");
                            Console.WriteLine("trying to add " + charge + "," + area + Environment.NewLine);
                            foreach (KeyValuePair<int, double> values in globalResultForThisScan.ToChild.ChargeArea)
                            {
                                Console.WriteLine("we already have " + values.Key + "," + values.Value);

                            }
                            //Console.ReadKey();
                        }
                        else
                        {
                            globalResultForThisScan.ToChild.ChargeArea.Add(charge, area);
                            globalResultForThisScan.ToChild.ChargeMsAbundance.Add(charge, msAbundance);
                        }
                        
                    }
                }
            }

            

            //6.  find max correlaion value to represent the feature.  this comes from the modified list of connected differences
            if (modified != null && modified.Count > 0)
            {
                //modified = modified.OrderByDescending(n => n.ToChild.CorrelationScore).ToList();
                modified = modified.OrderByDescending(n => n.ToChild.ParentFitAbundance).ToList();
                globalResultForThisScan.ToChild.TargetParent.DifferenceName = modified[0].ToChild.TargetParent.DifferenceName;
                globalResultForThisScan.ToChild.CorrelationScore = modified[0].ToChild.CorrelationScore;
                globalResultForThisScan.ToChild.ParentFitAbundance = modified[0].ToChild.ParentFitAbundance;
                globalResultForThisScan.ToChild.ParentCharge = modified[0].ToChild.ParentCharge;
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
