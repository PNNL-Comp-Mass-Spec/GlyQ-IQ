﻿using System.Text;
using IQ.Workflows;
using IQ_X64.Workflows.FileIO.DTO;

namespace IQ_X64.Workflows.FileIO
{
    public class SipperResultToLcmsFeatureExporter : TargetedResultToTextExporter
    {

        #region Constructors
        public SipperResultToLcmsFeatureExporter(string filename)
            : base(filename)
        {

        }

        #endregion

        protected override string addAdditionalInfo(TargetedResultDTO result)
        {
            var sipperResult = (SipperLcmsFeatureTargetedResultDTO)result;

            StringBuilder sb = new StringBuilder();
            sb.Append(Delimiter);
            sb.Append(sipperResult.MatchedMassTagID);
            sb.Append(Delimiter);
            sb.Append(sipperResult.NumHighQualityProfilePeaks.ToString("0"));
            sb.Append(Delimiter);
            sb.Append(sipperResult.FitScoreLabeledProfile.ToString("0.0000"));
            sb.Append(Delimiter);
            sb.Append(sipperResult.AreaUnderDifferenceCurve.ToString("0.000"));
            sb.Append(Delimiter);
            sb.Append(sipperResult.AreaUnderRatioCurve.ToString("0.000"));
            sb.Append(Delimiter);
            sb.Append(sipperResult.AreaUnderRatioCurveRevised.ToString("0.000"));
            sb.Append(Delimiter);
            sb.Append(sipperResult.ChromCorrelationMin.ToString("0.00000"));
            sb.Append(Delimiter);
            sb.Append(sipperResult.ChromCorrelationMax.ToString("0.00000"));
            sb.Append(Delimiter);
            sb.Append(sipperResult.ChromCorrelationAverage.ToString("0.00000"));
            sb.Append(Delimiter);
            sb.Append(sipperResult.ChromCorrelationMedian.ToString("0.00000"));
            sb.Append(Delimiter);
            sb.Append(sipperResult.ChromCorrelationStdev.ToString("0.00000"));
            sb.Append(Delimiter);
            sb.Append(sipperResult.NumCarbonsLabelled.ToString("0.000"));
            sb.Append(Delimiter);
            sb.Append(sipperResult.PercentCarbonsLabelled.ToString("0.00"));
            sb.Append(Delimiter);
            sb.Append(sipperResult.PercentPeptideLabelled.ToString("0.00"));
            sb.Append(Delimiter);
            sb.Append(sipperResult.ContiguousnessScore.ToString("0"));
            sb.Append(Delimiter);
            sb.Append(sipperResult.RSquaredValForRatioCurve.ToString("0.0000"));
            sb.Append(Delimiter);
            //sb.Append(GetLabelDistributionDataAsString(sipperResult));
            //sb.Append(Delimiter);
            sb.Append(sipperResult.ValidationCode == ValidationCode.None ? string.Empty : sipperResult.ValidationCode.ToString());
            return sb.ToString();

        }

        private string GetLabelDistributionDataAsString(SipperLcmsFeatureTargetedResultDTO sipperResult)
        {
            if (sipperResult.LabelDistributionVals == null || sipperResult.LabelDistributionVals.Length == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            char delim = ',';
            foreach (var labelDistributionVal in sipperResult.LabelDistributionVals)
            {
                sb.Append(labelDistributionVal.ToString("0.#####"));
                sb.Append(delim);
            }

            return sb.ToString().TrimEnd(delim);

        }


        protected override string buildHeaderLine()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(base.buildHeaderLine());
            sb.Append(Delimiter);
            sb.Append("MatchedMassTagID");
            sb.Append(Delimiter);
            sb.Append("NumHQProfilePeaks");
            sb.Append(Delimiter);
            sb.Append("FitScoreLabeled");
            sb.Append(Delimiter); 
            sb.Append("AreaDifferenceCurve");
            sb.Append(Delimiter);
            sb.Append("AreaRatioCurve");
            sb.Append(Delimiter);
            sb.Append("AreaRatioCurveRevised");
            sb.Append(Delimiter);
            sb.Append("ChromCorrMin");
            sb.Append(Delimiter);
            sb.Append("ChromCorrMax");
            sb.Append(Delimiter);
            sb.Append("ChromCorrAverage");
            sb.Append(Delimiter);
            sb.Append("ChromCorrMedian");
            sb.Append(Delimiter);
            sb.Append("ChromCorrStdev");
            sb.Append(Delimiter);
            sb.Append("NumCarbonsLabelled");
            sb.Append(Delimiter);
            sb.Append("PercentCarbonsLabelled");
            sb.Append(Delimiter);
            sb.Append("PercentPeptidesLabelled");
            sb.Append(Delimiter);
            sb.Append("ContigScore");
            sb.Append(Delimiter);
            sb.Append("RSquared");
            sb.Append(Delimiter);
            sb.Append("ValidationCode");
            
            return sb.ToString();

        }


    }
}
