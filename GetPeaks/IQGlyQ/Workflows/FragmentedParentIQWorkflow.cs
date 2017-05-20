using System;
using System.Linq;
using DeconTools.Utilities;
using DeconTools.Workflows.Backend.Core;
using DeconTools.Backend.Core;
using IQGlyQ.Enumerations;
using IQGlyQ.Exporter;
using System.Collections.Generic;
using IQGlyQ.Results;

namespace IQGlyQ.Workflows
{
    public class FragmentedParentIQWorkflow : IqWorkflow
    {
        
        #region Constructors

        public FragmentedParentIQWorkflow(Run run, FragmentedTargetedWorkflowParametersIQ parameters)
            : base(run, parameters)
        {
            
            InitializeWorkflow();

        }

        public FragmentedParentIQWorkflow(FragmentedTargetedWorkflowParametersIQ parameters)
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

            FragmentedTargetedWorkflowParametersIQ WorkflowParameters = (FragmentedTargetedWorkflowParametersIQ)this.WorkflowParameters;
        }

        public override TargetedWorkflowParameters WorkflowParameters { get; set; }

        public override void Execute(IqResult result)
        {
            if (result.HasChildren())
            {
                FragmentedTargetedWorkflowParametersIQ WorkflowParameters = (FragmentedTargetedWorkflowParametersIQ)this.WorkflowParameters;
                double fragmentCuttoffThreshold = WorkflowParameters.CorrelationScoreCuttoff;
                double fitScoreCuttoff = WorkflowParameters.FitScoreCuttoff;

                List<IqGlyQResult> existingGlyQiqChildren = WorkflowUtilities.CastToGlyQiQResult(result, fitScoreCuttoff);
                List<IqGlyQResult> replacementGlyQiqChildren = new List<IqGlyQResult>();
                //label origiional targets

                WorkflowUtilities.PrintHits(result);

                

                //1.  setup base result summary
                //double fragmentCuttoffThreshold = 0.6;//used for counting up how many fragments there are
                List<IqGlyQResult> intactList;
                IqGlyQResult baseResult = OrganizeBaseResult(result, existingGlyQiqChildren, replacementGlyQiqChildren, fragmentCuttoffThreshold, out intactList);

                //2.  add to new list
                replacementGlyQiqChildren.Add(baseResult);

                //3.  sum all charge states within isomers so we have general isomer abundances
                List<IqGlyQResult> baseIsomers = OrganizeIsomers(ref replacementGlyQiqChildren, baseResult, intactList);

                //4.  add to new list
                replacementGlyQiqChildren.AddRange(baseIsomers);

                //5.  build into list for export

                IqGlyQResult replacementResult = new IqGlyQResult(result.Target);
                foreach (IqGlyQResult agretateResult in replacementGlyQiqChildren)
                {
                    result.AddResult(agretateResult);
                    //replacementResult.AddResult(agretateResult);
                }

                //result = replacementResult;

            }
        }

        

        private static List<IqGlyQResult> OrganizeIsomers(ref List<IqGlyQResult> replacementGlyQiqChildren, IqGlyQResult baseResult, List<IqGlyQResult> intactList)
        {

            //2.  cycle through each isomer scan
            List<int> isomerScanList = baseResult.ToChild.GlobalIsomerScans;

            List<IqGlyQResult> isomerResultList = new List<IqGlyQResult>();
            //for each isomer
            foreach (int isomerScan in isomerScanList)
            {
                //A.  find all charge states in this scan
                List<IqGlyQResult> intactIsomerSubList = (from n in intactList where n.ToChild.TargetFragment.ScanLCTarget == isomerScan select n).ToList();

                //B.  find all charge states present in this scan
                List<int> chargeStateListWithinIsomer = (from n in intactIsomerSubList select n.ToChild.TargetParent.ChargeState).ToList();

                //C.  find all correlation scores in this scan
                List<double> correlationListWithinIsomer = (from n in intactIsomerSubList select n.ToChild.CorrelationScore).ToList();

                //D.  isomers found?  Create isomer summary
                IqGlyQResult sampleIsomer;
                if (intactIsomerSubList.Count > 0)
                {
                    //copy isomer 0 because all the information should be the pretty much the same across isomers
                    sampleIsomer = intactIsomerSubList[0];
                    IqGlyQResult isomerSummaryResult = new IqGlyQResult(sampleIsomer.Target);
                    

                    //polish off details for this isomer
                    isomerSummaryResult.ToChild = new FragmentResultsObjectHolderIq(sampleIsomer.ToChild);
                    isomerSummaryResult.ToChild.GlobalResult = true;
                    //add on global isomer properites
                    isomerSummaryResult.ToChild.GlobalChargeStateMax = chargeStateListWithinIsomer.Max();
                    isomerSummaryResult.ToChild.GlobalChargeStateMin = chargeStateListWithinIsomer.Min(); ;
                    isomerSummaryResult.ToChild.GlobalNumberOfIsomers = intactIsomerSubList.Count;
                    //isomerResult.ToChild.Scan = sampleIsomer.ToChild.Scan;//perhaps redundant

                    //report max correlation score across all charge states and fragments?
                    isomerSummaryResult.ToChild.CorrelationScore = correlationListWithinIsomer.Max();

                    //????????  I think we are going after a summed abundance across charge states in this scan
                    double correlationScoreTollerence = 0.1;
                    isomerSummaryResult.ToChild.ParentFitAbundance = (
                        from n in intactIsomerSubList
                        where n.ToChild.CorrelationScore < isomerSummaryResult.ToChild.CorrelationScore + correlationScoreTollerence && n.ToChild.CorrelationScore > isomerSummaryResult.ToChild.CorrelationScore - correlationScoreTollerence
                        select n.ToChild.FragmentFitAbundance).First();

                    double sumFragmentAbundance = 0;
                    foreach (var isomer in intactIsomerSubList)
                    {
                        sumFragmentAbundance += isomer.ToChild.FragmentFitAbundance;
                    }

                    isomerSummaryResult.ToChild.GlobalAggregateAbundance = sumFragmentAbundance;

                    isomerSummaryResult.ToChild.Error = EnumerationError.BaseIsomer;

                    isomerSummaryResult.MonoMassObs = sampleIsomer.MonoMassObs;
                    isomerSummaryResult.MZObs = sampleIsomer.MZObs;
                    //isomerSummaryResult.ElutionTimeObs = sampleIsomer.ElutionTimeObs;not synced
                    isomerSummaryResult.FitScore = sampleIsomer.FitScore;
                    isomerSummaryResult.InterferenceScore = sampleIsomer.InterferenceScore;
                    isomerSummaryResult.NumChromPeaksWithinTolerance = intactIsomerSubList.Count;

                    isomerResultList.Add(isomerSummaryResult);
                    //replacementGlyQiqChildren.Add(isomerResult);
                    Console.WriteLine(intactIsomerSubList.Count);
                }


            }

            return isomerResultList;
        }

        private static IqGlyQResult OrganizeBaseResult(IqResult result, List<IqGlyQResult> existingGlyQiqChildren, List<IqGlyQResult> replacementGlyQiqChildren, double framentCuttoff, out List<IqGlyQResult> intactList)
        {
            //1.  make summarized result that contains details overall.  This will have 0 charge and be identified by Flobalresult
            IqGlyQResult baseResult = new IqGlyQResult(result.Target);
            baseResult.ToChild.GlobalResult = true;

            //2.  find all intact hits
            intactList = (from n in existingGlyQiqChildren where n.ToChild.IsIntact == true select n).ToList();
            if (intactList.Count > 0)
            {
                baseResult.ToChild.IsIntact = true;
                baseResult.ToChild.GlobalIntactCount = intactList.Count;
            }

            //3.  find charge states represented in the intact list so we can get max and min charge states
            List<int> chargeStateList = (from n in intactList select n.ToChild.TargetFragment.ChargeState).ToList();
            if (chargeStateList.Count > 0)
            {
                baseResult.ToChild.GlobalChargeStateMax = chargeStateList.Max();
                baseResult.ToChild.GlobalChargeStateMin = chargeStateList.Min();
            }

            //4.  find isomer count of intact fragments.  distinct list of each scan with intact glycans so we can count isomers and iterate over scans
            baseResult.ToChild.GlobalIsomerScans = (from n in intactList select n.ToChild.TargetFragment.ScanLCTarget).Distinct().ToList();
            baseResult.ToChild.GlobalNumberOfIsomers = baseResult.ToChild.GlobalIsomerScans.Count;

            //5.  Count fragments so we know how many future targets there are
            int fragmentCount = 0;
            List<string> differences = new List<string>();
            foreach (IqGlyQResult i in replacementGlyQiqChildren)
            {
                double correlationValues = i.ToChild.CorrelationScore;
                if (correlationValues > framentCuttoff)
                {
                    fragmentCount++;
                }
            }
            baseResult.ToChild.GlobalFutureTargetsCount = fragmentCount;

            baseResult.ToChild.Error = EnumerationError.BaseResult;
            return baseResult;
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
