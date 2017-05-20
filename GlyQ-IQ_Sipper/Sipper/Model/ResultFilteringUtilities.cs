using System.Collections.Generic;
using DeconTools.Workflows.Backend.Results;

namespace Sipper.Model
{
    public class ResultFilteringUtilities
    {

        #region Constructors
        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public static void ApplyFilteringScheme1(List<SipperLcmsFeatureTargetedResultDTO> allResults)
        {
            foreach (var sipperLcmsFeatureTargetedResultDto in allResults)
            {
                ApplyFilteringScheme1(sipperLcmsFeatureTargetedResultDto);

            }

        }
        public static void ApplyFilteringScheme2(List<SipperLcmsFeatureTargetedResultDTO> allResults)
        {
            foreach (var sipperLcmsFeatureTargetedResultDto in allResults)
            {
                ApplyFilteringScheme2(sipperLcmsFeatureTargetedResultDto);

            }

        }

        public static void ApplyFilteringScheme1(SipperLcmsFeatureTargetedResultDTO result)
        {
            if (result.AreaUnderRatioCurveRevised > 0)
            {
                if (result.ChromCorrelationAverage > 0.9 && result.ChromCorrelationMedian > 0.95)
                {
                    if (result.RSquaredValForRatioCurve > 0.85)
                    {

                        //high quality results
                        if (result.RSquaredValForRatioCurve > 0.985 && result.AreaUnderRatioCurve > 75)
                        {
                            if (result.IScore <= 0.4)
                            {
                                result.PassesFilter = true;
                            }
                        }
                        //high quality results
                        else if (result.RSquaredValForRatioCurve > 0.985 && result.AreaUnderRatioCurveRevised > 15)
                        {
                            if (result.IScore <= 0.4)
                            {
                                result.PassesFilter = true;
                            }
                        }
                        else if (result.ChromCorrelationMedian > 0.99 && result.ChromCorrelationAverage > 0.99)
                        {
                            if (result.IScore <= 0.4)
                            {
                                result.PassesFilter = true;
                            }
                        }
                        //high intensity results
                        else if (result.RSquaredValForRatioCurve > 0.975 && result.Intensity > 1e5)
                        {
                            if (result.IScore <= 0.4)
                            {
                                result.PassesFilter = true;
                            }

                        }
                        //all other results - 
                        else if (result.RSquaredValForRatioCurve > 0.975 && result.IScore <= 0.25)
                        {
                            result.PassesFilter = true;
                        }

                    }
                }

            }
        }

        public static void ApplyFilteringScheme2(SipperLcmsFeatureTargetedResultDTO result)
        {
            if (result.AreaUnderRatioCurveRevised > 0)
            {
                if (result.ChromCorrelationAverage > 0.85 && result.ChromCorrelationMedian > 0.9)
                {
                    if (result.RSquaredValForRatioCurve > 0.85)
                    {

                        //high quality results
                        if (result.RSquaredValForRatioCurve > 0.95 && result.AreaUnderRatioCurve > 75)
                        {
                            if (result.IScore <= 0.4)
                            {
                                result.PassesFilter = true;
                            }
                        }
                        //high quality results
                        else if (result.RSquaredValForRatioCurve > 0.5 && result.AreaUnderRatioCurveRevised > 15)
                        {
                            if (result.IScore <= 0.4)
                            {
                                result.PassesFilter = true;
                            }
                        }
                        else if (result.ChromCorrelationMedian > 0.99 && result.ChromCorrelationAverage > 0.99)
                        {
                            if (result.IScore <= 0.4)
                            {
                                result.PassesFilter = true;
                            }
                        }
                        //high intensity results
                        else if (result.RSquaredValForRatioCurve > 0.95 && result.Intensity > 1e5)
                        {
                            if (result.IScore <= 0.4)
                            {
                                result.PassesFilter = true;
                            }

                        }
                        //all other results - 
                        else if (result.RSquaredValForRatioCurve > 0.95 && result.IScore <= 0.25)
                        {
                            result.PassesFilter = true;
                        }

                    }
                }

            }
        }


        #endregion

        #region Private Methods

        #endregion

    }
}
