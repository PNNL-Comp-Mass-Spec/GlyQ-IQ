using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeconTools.Workflows.Backend.Core;
using DeconTools.Workflows.Backend.FileIO;
using IQGlyQ.Results;

namespace IQGlyQ.Exporter
{
    public class InSourceFragmentExporter : ResultExporter
    {
        public override string GetResultAsString(IqResult result, bool includeHeader = false)
        {
            string delim = "\t";

            //we need to set the lcscanset selelected

            //IQ
            string baseOutput = base.GetResultAsString(result, includeHeader);

            //GlyQ
            IqGlyQResult castResult = (IqGlyQResult) result;
            string addOnString = AddOnString(castResult, includeHeader);

            string resultString = baseOutput + delim +addOnString;

            return resultString;
        }

        public override string GetHeader()
        {
            string delim = "\t";

            //IQ
            string baseOutput = base.GetHeader();

            //GlyQ
            string addOnString = AddOnHeaderString();

            string resultHeader = baseOutput + delim + addOnString;

            return resultHeader;
        }

        private string AddOnString(IqGlyQResult result, bool includeHeader = false)
        {
            StringBuilder sb = new StringBuilder();

            string scanSetString;

            if (result.LCScanSetSelected != null)
            {
                scanSetString = result.ToChild.Scan.ToString();
            }
            else
            {
                scanSetString = "0";
            }

            string delim = "\t";


            if (includeHeader)
            {
                string header = GetHeader();
                sb.Append(header);
                sb.Append(Environment.NewLine);

            }


            sb.Append(result.ToChild.TargetFragment.ScanLCTarget);
            sb.Append(delim);
            sb.Append(result.ToChild.TargetFragment.ChargeState);
            sb.Append(delim);
            sb.Append(result.ToChild.TargetParent.ChargeState);
            sb.Append(delim);
            sb.Append(result.ToChild.CorrelationScore);
            sb.Append(delim);
            sb.Append(result.ToChild.IsIntact);
            sb.Append(delim);
            sb.Append(result.ToChild.IsAntiCorrelated);
            sb.Append(delim);
            sb.Append(result.ToChild.TargetParent.DifferenceName);
            sb.Append(delim);
            sb.Append(result.ToChild.FragmentFitAbundance);
            sb.Append(delim);
            sb.Append(result.ToChild.ParentFitAbundance);
            sb.Append(delim);
            sb.Append(result.ToChild.LMOptimizationCorrelationRsquared);
            sb.Append(delim);
            sb.Append(result.ToChild.TargetFragment.ScanInfo.Start);
            sb.Append(delim);
            sb.Append(result.ToChild.TargetFragment.ScanInfo.Stop);
            sb.Append(delim);
            sb.Append(result.ToChild.TargetParent.ScanInfo.Start);
            sb.Append(delim);
            sb.Append(result.ToChild.TargetParent.ScanInfo.Stop);
            sb.Append(delim);
            sb.Append(result.ToChild.Error);
            sb.Append(delim);
            sb.Append(result.ToChild.Error);//needs to be fixed
            sb.Append(delim);
            sb.Append(result.ToChild.GlobalChargeStateMin);
            sb.Append(delim);
            sb.Append(result.ToChild.GlobalChargeStateMax);
            sb.Append(delim);
            sb.Append(result.ToChild.GlobalAggregateAbundance);
            sb.Append(delim);
            sb.Append(result.ToChild.GlobalIntactCount);
            sb.Append(delim);
            sb.Append(result.ToChild.GlobalFutureTargetsCount);
            sb.Append(delim);
            sb.Append(result.ToChild.GlobalResult);
            sb.Append(delim);
            sb.Append(result.ToChild.TypeOfResultParentOrChildDifferenceApproach);
            sb.Append(delim);
            sb.Append(result.ToChild.FinalDecision);
            sb.Append(delim);
            sb.Append(result.ToChild.AverageMonoMass);
            sb.Append(delim);
            sb.Append(result.ToChild.PPMError);
            sb.Append(delim);
            sb.Append(result.ToChild.TypeOfResultTargetOrModifiedTarget);
            sb.Append(delim);
            sb.Append(result.ToChild.FragmentObservedIsotopeProfile.Score);
            sb.Append(delim);
            sb.Append(result.ToChild.InterfearenceScore);
            sb.Append(delim);
            sb.Append(result.ToChild.ChargeStateCorrelation);

            string outString = sb.ToString();
            return outString;
        }

        private string AddOnHeaderString()
        {
            StringBuilder sb = new StringBuilder();

            string delim = "\t";

            sb.Append("ScanNum");
            sb.Append(delim);
            sb.Append("FragmentCharge");
            sb.Append(delim);
            sb.Append("ParentCharge");
            sb.Append(delim);
            sb.Append("CorrelationScore");
            sb.Append(delim);
            sb.Append("IsIntact");
            sb.Append(delim);
            sb.Append("IsAntiCorrelated");
            sb.Append(delim);
            sb.Append("Name");
            sb.Append(delim);
            sb.Append("FragmentArea");
            sb.Append(delim);
            sb.Append("ParentArea");
            sb.Append(delim);
            sb.Append("Fragment_LM_R2");
            sb.Append(delim);
            sb.Append("FragmentStart");
            sb.Append(delim);
            sb.Append("FragmentStop");
            sb.Append(delim);
            sb.Append("ParentStart");
            sb.Append(delim);
            sb.Append("ParentStop");
            sb.Append(delim);
            sb.Append("FragmentError");
            sb.Append(delim);
            sb.Append("ParentError");

            sb.Append(delim);
            sb.Append("GlobalChargeStateMin");
            sb.Append(delim);
            sb.Append("GlobalChargeStateMax");
            sb.Append(delim);
            sb.Append("GlobalAggregateAbundance");
            sb.Append(delim);
            sb.Append("GlobalIntactCount");
            sb.Append(delim);
            sb.Append("GlobalFutureTargetsCount");
            sb.Append(delim);
            sb.Append("GlobalResult");
            sb.Append(delim);
            sb.Append("ParentOrChild");
            sb.Append(delim);
            sb.Append("FinalDecision");
            sb.Append(delim);
            sb.Append("AverageMonoMass");
            sb.Append(delim);
            sb.Append("PPM");
            sb.Append(delim);
            sb.Append("Type");
            sb.Append(delim);
            sb.Append("IsoFit");
            sb.Append(delim);
            sb.Append("InterferenceValue");
            sb.Append(delim);
            sb.Append("ChargeStateCorrelation");

            string outString = sb.ToString();
            return outString;
        }
    }
}
