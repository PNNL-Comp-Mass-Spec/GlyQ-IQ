using System;
using System.Collections.Generic;
using DeconTools.Backend.Core;
using DeconTools.Workflows.Backend.Core;
using IQGlyQ.Enumerations;
using IQGlyQ.Objects;
using IQGlyQ.TargetGenerators;

namespace IQGlyQ.Results
{
    public class FragmentResultsObjectIq
    {
        public ChromPeakQualityData PeakQualityObject { get; set; }


        public List<FragmentIQTarget> FragmentsConsidered { get; set; }

        public List<FragmentResultsObjectHolderIq> Results { get; set; }

        public List<IqTarget> NewTargets { get; set; }

        public FragmentResultsObjectIq()
        {
            PeakQualityObject = new ChromPeakQualityData(new ChromPeak());
            FragmentsConsidered = new List<FragmentIQTarget>();
            Results = new List<FragmentResultsObjectHolderIq>();
            NewTargets = new List<IqTarget>();
        }
    }

    public class FragmentResultsObjectHolderIq
    {
        /// <summary>
        /// was this successfull
        /// </summary>
        public string ErrorValue { get; set; }

        /// <summary>
        /// which part failed
        /// </summary>
        public EnumerationError Error { get; set; }

        /// <summary>
        /// charge state of parent (does this exist?)
        /// </summary>
        public int ParentCharge { get; set; }

        /// <summary>
        /// charge state of fragment (target)
        /// </summary>
        public int FragmentCharge { get; set; }

        /// <summary>
        /// center scan of lc peak of fragment
        /// </summary>
        public int Scan { get; set; }

        /// <summary>
        /// Net or elution time associated with Scan
        /// </summary>
        public double ElutionTime { get; set; }

        /// <summary>
        /// center scan of lc peak of fragment in terms of decon run scanset
        /// </summary>
        //public ScanSet DeconScanSet { get; set; }

        /// <summary>
        /// scan range for the fit peak for determining correlation range
        /// </summary>
        public ScanObject ScanBoundsInfo { get; set; }

        /// <summary>
        /// full target of parent
        /// </summary>
        public FragmentIQTarget TargetParent { get; set; }

        /// <summary>
        /// full target of fragment
        /// </summary>
        public FragmentIQTarget TargetFragment { get; set; }
        
        /// <summary>
        /// did we fin this target to be intact
        /// </summary>
        public bool IsIntact { get; set; }

        /// <summary>
        /// is the peaks anticorrelated
        /// </summary>
        public bool IsAntiCorrelated { get; set; }

        /// <summary>
        /// summary of results from correlation
        /// </summary>
        public ChromCorrelationData CorrelationResults { get; set; }

        /// <summary>
        /// place to store the correlation score between the fit curves
        /// </summary>
        public double CorrelationScore { get; set; }

        //correaltion coefficients from the fit
        public double[] CorrelationCoefficients { get; set; }

        /// <summary>
        /// place to hold the observed mz
        /// </summary>
        public IsotopicProfile FragmentObservedIsotopeProfile { get; set; }

        /// <summary>
        /// so we can pull out the correasponding peak for later iq
        /// </summary>
        public int ChromPeakQualityIndex { get; set; }

        /// <summary>
        /// we need to knof if this correlated wiht a larger mass or a smaller mass
        /// </summary>
        public EnumerationParentOrChild TypeOfResultParentOrChildDifferenceApproach { get; set; }

        /// <summary>
        /// we need to knof if this correlated wiht a larger mass or a smaller mass
        /// </summary>
        public TypeOfResult TypeOfResultTargetOrModifiedTarget { get; set; }

        public Dictionary<int, double> ChargeMonoMass { get; set; }
        public Dictionary<int, double> ChargeArea { get; set; }
        public Dictionary<int, float> ChargeMsAbundance { get; set; }

        /// <summary>
        /// actual quality of the fit to the theoretical lineshape used in LM solver.  this is the R squared value reported from algilib
        /// </summary>
        public double LMOptimizationCorrelationRsquared { get; set; }

        /// <summary>
        /// finally we can make a call and identify our glycans
        /// </summary>
        public EnumerationFinalDecision FinalDecision { get; set; }

        /// <summary>
        /// sum of all LC peak areas
        /// </summary>
        public double GlobalAggregateAbundance { get; set; }

        /// <summary>
        /// sum of all MS peak areas
        /// </summary>
        public double GlobalAggregateMSAbundance { get; set; }

        public int GlobalChargeStateMin { get; set; }
        public int GlobalChargeStateMax { get; set; }
        public List<int> GlobalIsomerScans { get; set; }
        public int GlobalNumberOfIsomers { get; set; }
        public int GlobalIntactCount { get; set; }
        public int GlobalFutureTargetsCount { get; set; }
        public bool GlobalResult { get; set; }

        /// <summary>
        /// area under the curve (4sigma in both directions) for fit gaussian to lc peak
        /// </summary>
        public float FragmentFitAbundance { get; set; }

        /// <summary>
        /// area under the curve (4sigma in both directions) for fit gaussian to lc peak
        /// </summary>
        public float ParentFitAbundance { get; set; }

        /// <summary>
        /// averaging across charge states
        /// </summary>
        public double AverageMonoMass { get; set; }

        /// <summary>
        /// decontools interferenceScore
        /// </summary>
        public double InterfearenceScore { get; set; }

        /// <summary>
        /// ChargeStateCorrelation based on fit profiles
        /// </summary>
        public double ChargeStateCorrelation { get; set; }


        /// <summary>
        /// averave vs theoretical
        /// </summary>
        public double PPMError { get; set; }

        public FragmentResultsObjectHolderIq(FragmentIQTarget targetIn)
        {
            TargetFragment = targetIn;
            TargetParent = new FragmentIQTarget();
            CorrelationResults = new ChromCorrelationData();
            ErrorValue = "";
            Error = EnumerationError.NoError;
            ScanBoundsInfo = new ScanObject(0,0);
            //DeconScanSet = new ScanSet(targetIn.ScanLCTarget);
            FragmentObservedIsotopeProfile = new IsotopicProfile();
            //CorrelationCoefficients = new double[0];
            TypeOfResultParentOrChildDifferenceApproach = EnumerationParentOrChild.NotKnown;
            ChargeMonoMass = new Dictionary<int, double>();
            ChargeArea = new Dictionary<int, double>();
            ChargeMsAbundance = new Dictionary<int, float>();
        }

        public FragmentResultsObjectHolderIq(FragmentResultsObjectHolderIq copiedObjectHolder)
        {
            CorrelationResults = copiedObjectHolder.CorrelationResults;
            CorrelationScore = copiedObjectHolder.CorrelationScore;
            ErrorValue = copiedObjectHolder.ErrorValue;
            FragmentCharge = copiedObjectHolder.FragmentCharge;
            FragmentFitAbundance = copiedObjectHolder.FragmentFitAbundance;
            ScanBoundsInfo = copiedObjectHolder.ScanBoundsInfo;//test this;
            FragmentObservedIsotopeProfile = copiedObjectHolder.FragmentObservedIsotopeProfile;

            GlobalAggregateAbundance = copiedObjectHolder.GlobalAggregateAbundance;

            GlobalChargeStateMin = copiedObjectHolder.GlobalChargeStateMin;
            GlobalChargeStateMax = copiedObjectHolder.GlobalChargeStateMax;
            GlobalIsomerScans = copiedObjectHolder.GlobalIsomerScans;
            GlobalNumberOfIsomers = copiedObjectHolder.GlobalNumberOfIsomers;
            GlobalIntactCount = copiedObjectHolder.GlobalIntactCount;
            GlobalFutureTargetsCount = copiedObjectHolder.GlobalFutureTargetsCount;
            GlobalResult = copiedObjectHolder.GlobalResult;

            IsAntiCorrelated = copiedObjectHolder.IsAntiCorrelated;
            IsIntact = copiedObjectHolder.IsIntact;
            ParentCharge = copiedObjectHolder.ParentCharge;
            ParentFitAbundance = copiedObjectHolder.ParentFitAbundance;
            Scan = copiedObjectHolder.Scan;
            //DeconScanSet = copiedObjectHolder.DeconScanSet;
            TargetFragment = copiedObjectHolder.TargetFragment;
            TargetParent = copiedObjectHolder.TargetParent;
            TypeOfResultParentOrChildDifferenceApproach = copiedObjectHolder.TypeOfResultParentOrChildDifferenceApproach;

            ChargeMonoMass = copiedObjectHolder.ChargeMonoMass;
            ChargeArea = copiedObjectHolder.ChargeArea;

            Error = copiedObjectHolder.Error;
            ChargeStateCorrelation = copiedObjectHolder.ChargeStateCorrelation;
        }
    }
}
