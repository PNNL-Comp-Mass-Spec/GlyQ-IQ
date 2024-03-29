﻿using System;
using System.Text;
using IQ.Workflows.FileIO.DTO;
using IQ.Workflows.WorkFlowParameters;
using Run32.Backend;

namespace IQ.Workflows.FileIO
{
    public abstract class TargetedResultToTextExporter : TextFileExporter<TargetedResultDTO>
    {


        public static TargetedResultToTextExporter CreateExporter(WorkflowParameters workflowParameters, string outputFileName)
        {
            TargetedResultToTextExporter exporter;

            switch (workflowParameters.WorkflowType)
            {
                case GlobalsWorkFlow.TargetedWorkflowTypes.Undefined:
                    exporter = new UnlabelledTargetedResultToTextExporter(outputFileName);
                    break;
                case GlobalsWorkFlow.TargetedWorkflowTypes.UnlabelledTargeted1:
                    exporter = new UnlabelledTargetedResultToTextExporter(outputFileName);
                    break;
                case GlobalsWorkFlow.TargetedWorkflowTypes.O16O18Targeted1:
                    exporter = new O16O18TargetedResultToTextExporter(outputFileName);
                    break;
                case GlobalsWorkFlow.TargetedWorkflowTypes.SipperTargeted1:
                    exporter = new SipperResultToLcmsFeatureExporter(outputFileName);
                    break;
                case GlobalsWorkFlow.TargetedWorkflowTypes.N14N15Targeted1:
                    exporter = new N14N15TargetedResultToTextExporter(outputFileName);
                    break;
                case GlobalsWorkFlow.TargetedWorkflowTypes.TopDownTargeted1:
					exporter = new TopDownTargetedResultToTextExporter(outputFileName);
            		break;
                case GlobalsWorkFlow.TargetedWorkflowTypes.Deuterated:
                    exporter = new DeuteratedTargetedResultToTextExporter(outputFileName);
                    break;

                case GlobalsWorkFlow.TargetedWorkflowTypes.TargetedAlignerWorkflow1:
                    throw new NotImplementedException("Cannot create exporter for this type of workflow");
                case GlobalsWorkFlow.TargetedWorkflowTypes.PeakDetectAndExportWorkflow1:
                    throw new NotImplementedException("Cannot create exporter for this type of workflow");
                case GlobalsWorkFlow.TargetedWorkflowTypes.BasicTargetedWorkflowExecutor1:
                    throw new NotImplementedException("Cannot create exporter for this type of workflow");
                default:
                    exporter = new UnlabelledTargetedResultToTextExporter(outputFileName);
                    break;
            }

            return exporter;



        }




        #region Constructors
        public TargetedResultToTextExporter(string filename) : base(filename, '\t') { }

        #endregion

   
        protected override string buildResultOutput(TargetedResultDTO result)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(addBasicTargetedResult(result));

            sb.Append(addAdditionalInfo(result));

            return sb.ToString();

        }

        protected virtual string addAdditionalInfo(TargetedResultDTO result)
        {
            return String.Empty;
        }

        protected virtual string addBasicTargetedResult(TargetedResultDTO result)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(result.DatasetName);
            sb.Append(Delimiter);
            sb.Append(result.TargetID);
            sb.Append(Delimiter);
            sb.Append(result.Code);
            sb.Append(Delimiter);
            sb.Append(result.EmpiricalFormula);
            sb.Append(Delimiter);
            sb.Append(result.ChargeState);
            sb.Append(Delimiter);
            
            
            sb.Append(result.ScanLC);
            sb.Append(Delimiter);
            sb.Append(result.ScanLCStart);
            sb.Append(Delimiter);
            sb.Append(result.ScanLCEnd);
            sb.Append(Delimiter);
            sb.Append(result.NumMSScansSummed);
            sb.Append(Delimiter);
            sb.Append(result.NET.ToString("0.00000"));
            sb.Append(Delimiter);
            sb.Append(result.NETError.ToString("0.000000"));
            sb.Append(Delimiter);
            sb.Append(result.NumChromPeaksWithinTol);
            sb.Append(Delimiter);
            sb.Append(result.NumQualityChromPeaksWithinTol);
            sb.Append(Delimiter);
            sb.Append(result.MonoMass.ToString("0.00000"));
            sb.Append(Delimiter);
            sb.Append(result.MonoMassCalibrated.ToString("0.00000"));
            sb.Append(Delimiter);
            sb.Append(result.MassErrorBeforeCalibration.ToString("0.00"));
            sb.Append(Delimiter);
            sb.Append(result.MassErrorAfterCalibration.ToString("0.00"));
            sb.Append(Delimiter);
          sb.Append(result.MonoMZ.ToString("0.00000"));
            sb.Append(Delimiter);
            sb.Append(result.Intensity);
            sb.Append(Delimiter);
            sb.Append(result.FitScore.ToString("0.0000"));
            sb.Append(Delimiter);
            sb.Append(result.IScore.ToString("0.0000"));
            sb.Append(Delimiter);
            sb.Append(result.FailureType);
            sb.Append(Delimiter);
            sb.Append(result.ErrorDescription);

            
            return sb.ToString();

        }

        protected override string buildHeaderLine()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Dataset");
            sb.Append(Delimiter);
            sb.Append("TargetID");
            sb.Append(Delimiter);
            sb.Append("Code");
            sb.Append(Delimiter);
            sb.Append("EmpiricalFormula");
            sb.Append(Delimiter);
            sb.Append("ChargeState");
            sb.Append(Delimiter);
            sb.Append("Scan");
            sb.Append(Delimiter);
            sb.Append("ScanStart");
            sb.Append(Delimiter);
            sb.Append("ScanEnd");
            sb.Append(Delimiter);
            sb.Append("NumMSSummed");
            sb.Append(Delimiter);
            sb.Append("NET");
            sb.Append(Delimiter);
            sb.Append("NETError");
            sb.Append(Delimiter);
            sb.Append("NumChromPeaksWithinTol");
            sb.Append(Delimiter);
            sb.Append("NumQualityChromPeaksWithinTol");
            sb.Append(Delimiter);
            sb.Append("MonoisotopicMass");
            sb.Append(Delimiter);
            sb.Append("MonoisotopicMassCalibrated");
            sb.Append(Delimiter);
            sb.Append("MassErrorBefore");
            sb.Append(Delimiter);
            sb.Append("MassErrorAfter");
            sb.Append(Delimiter);
            sb.Append("MonoMZ");
            sb.Append(Delimiter);
            sb.Append("IntensityRep");
            sb.Append(Delimiter);
            sb.Append("FitScore");
            sb.Append(Delimiter);
            sb.Append("IScore");
            sb.Append(Delimiter);
            sb.Append("FailureType");
            sb.Append(Delimiter);
            sb.Append("ErrorDescription");

            return sb.ToString();
        }
    }
}
