using DeconTools.Backend.Algorithms.Quantifiers;
using DeconTools.Backend.Core;
using DeconTools.Utilities;
using IQGlyQ;
using DeconTools.Backend.Core.Results;

namespace IQGlyQ
{
    public class DeuteratedQuantifierTask:Task
    {

        BasicDeuteratedQuantifier _quantifier;

        #region Constructors
        public DeuteratedQuantifierTask()
        {
            _quantifier = new BasicDeuteratedQuantifier();
        }

        public DeuteratedQuantifierTask(double minValue, double labelingEfficiency)
        {
            _quantifier = new BasicDeuteratedQuantifier(minValue, labelingEfficiency);
        }

        #endregion

      

        public override void Execute(ResultCollection resultList)
        {
            
            TargetedResultBase result = resultList.CurrentTargetedResult;

            foreach (ChromPeakQualityData peakQuality in result.ChromPeakQualityList)
            {
                Check.Require(result is DeuteratedTargetedResultObject, "Deuterated quantifier failed. Result is not of the Deuterated type.");

                IsotopicProfile experimentalProfile = peakQuality.IsotopicProfile;
                //IsotopicProfile experimentalProfile = result.IsotopicProfile;
                IsotopicProfile theoreticalProfile = result.Target.IsotopicProfileLabelled;//be sure to select non labeled here??  We switched to labeled I think
                //the spots are switched so labled is unlabled etc.
                _quantifier.PopulateValues(experimentalProfile, theoreticalProfile);


                _quantifier.CalculateDandH(_quantifier.LabelingEfficiency);
                _quantifier.SetMinIntensity(_quantifier.MinIntensity);
                double ratioDH = _quantifier.GetRatio(experimentalProfile);



                DeuteratedTargetedResultObject deuteratedResult = (DeuteratedTargetedResultObject)result;


                deuteratedResult.HydrogenI0 = _quantifier.intensityHI0;
                deuteratedResult.HydrogenI1 = _quantifier.intensityHI1;
                deuteratedResult.HydrogenI2 = _quantifier.intensityHI2;
                deuteratedResult.HydrogenI3 = _quantifier.intensityHI3;
                deuteratedResult.HydrogenI4 = _quantifier.intensityHI4;

                deuteratedResult.DeuteriumI0 = _quantifier.intensityDI0;
                deuteratedResult.DeuteriumI1 = _quantifier.intensityDI1;
                deuteratedResult.DeuteriumI2 = _quantifier.intensityDI2;
                deuteratedResult.DeuteriumI3 = _quantifier.intensityDI3;
                deuteratedResult.DeuteriumI4 = _quantifier.intensityDI4;

                deuteratedResult.TheoryI0 = _quantifier.intensityTheoryI0;
                deuteratedResult.TheoryI1 = _quantifier.intensityTheoryI1;
                deuteratedResult.TheoryI2 = _quantifier.intensityTheoryI2;
                deuteratedResult.TheoryI3 = _quantifier.intensityTheoryI3;
                deuteratedResult.TheoryI4 = _quantifier.intensityTheoryI4;

                deuteratedResult.RawI0 = _quantifier.intensityExpI0;
                deuteratedResult.RawI1 = _quantifier.intensityExpI1;
                deuteratedResult.RawI2 = _quantifier.intensityExpI2;
                deuteratedResult.RawI3 = _quantifier.intensityExpI3;
                deuteratedResult.RawI4 = _quantifier.intensityExpI4;

                deuteratedResult.LabelingEfficiency = _quantifier.LabelingEfficiency;
                deuteratedResult.RatioDH = ratioDH;
                deuteratedResult.IntensityI0HydrogenMono = _quantifier.HydrogenAbundance;
                deuteratedResult.IndegratedLcAbundance = experimentalProfile.IntensityAggregateAdjusted;
                //deuteratedResult.IndegratedLcAbundance = experimentalProfile.IntensityAggregateAdjusted * ratioDH;
            }
        }
    }
}
