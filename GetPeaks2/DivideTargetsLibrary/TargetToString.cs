using IQ.Workflows.Core;
using IQ.Workflows.FileIO.DTO;

namespace DivideTargetsLibrary
{
    public static class TargetToString
    {
        public static string SetString(TargetedResultDTO selectTarget, IqTarget selectTargetSimple, out string header)
        {
            string spacer = "\t";
            //string header = "Dataset" + spacer;//default dataset
            header = "";
            header += "Dataset" + spacer;
            header += "TargetID" + spacer;
            header += "Code" + spacer;
            header += "EmpiricalFormula" + spacer;
            header += "ChargeState" + spacer;
            header += "Scan" + spacer;
            header += "ScanStart" + spacer;
            header += "ScanEnd" + spacer;
            header += "NumMSSummed" + spacer;
            header += "NET" + spacer;
            header += "NETError" + spacer;
            header += "NumChromPeaksWithinTol" + spacer;
            header += "NumQualityChromPeaksWithinTol" + spacer;
            header += "MonoisotopicMass" + spacer;
            header += "MonoisotopicMassCalibrated" + spacer;
            header += "MassErrorInPPM" + spacer;
            header += "MonoMZ" + spacer;
            header += "IntensityRep" + spacer;
            header += "FitScore" + spacer;
            header += "IScore" + spacer;
            header += "FailureType" + spacer;
            header += "ErrorDescription" + spacer;

            string targetString = ""; //add dataset

            if (selectTarget.DatasetName != null) targetString += selectTarget.DatasetName + spacer;
            if (selectTarget.TargetID != null) targetString += selectTarget.TargetID.ToString() + spacer;
            if (selectTargetSimple.Code != null) targetString += selectTargetSimple.Code + spacer;
            if (selectTargetSimple.EmpiricalFormula != null) targetString += selectTargetSimple.EmpiricalFormula + spacer;
            if (selectTarget.ChargeState != null) targetString += selectTarget.ChargeState.ToString() + spacer;
            if (selectTarget.ScanLC != null) targetString += selectTarget.ScanLC.ToString() + spacer;
            if (selectTarget.ScanLCStart != null) targetString += selectTarget.ScanLCStart.ToString() + spacer;
            if (selectTarget.ScanLCEnd != null) targetString += selectTarget.ScanLCEnd.ToString() + spacer;
            if (selectTarget.NumMSScansSummed != null) targetString += selectTarget.NumMSScansSummed.ToString() + spacer;
            if (selectTarget.NET != null) targetString += selectTarget.NET.ToString() + spacer;
            if (selectTarget.NETError != null) targetString += selectTarget.NETError.ToString() + spacer;
            if (selectTarget.NumChromPeaksWithinTol != null)
                targetString += selectTarget.NumChromPeaksWithinTol.ToString() + spacer;
            if (selectTarget.NumQualityChromPeaksWithinTol != null)
                targetString += selectTarget.NumQualityChromPeaksWithinTol.ToString() + spacer;
            if (selectTarget.MonoMass != null) targetString += selectTarget.MonoMass.ToString() + spacer;
            if (selectTarget.MonoMassCalibrated != null) targetString += selectTarget.MonoMassCalibrated.ToString() + spacer;
            if (selectTarget.MassErrorBeforeCalibration != null)
                targetString += selectTarget.MassErrorBeforeCalibration.ToString() + spacer;
            if (selectTarget.MonoMZ != null) targetString += selectTarget.MonoMZ.ToString() + spacer;
            if (selectTarget.Intensity != null) targetString += selectTarget.Intensity.ToString() + spacer;
            if (selectTarget.FitScore != null) targetString += selectTarget.FitScore.ToString() + spacer;
            if (selectTarget.IScore != null) targetString += selectTarget.IScore.ToString() + spacer;
            if (selectTarget.FailureType != null) targetString += selectTarget.FailureType + spacer;
            else
            {
                targetString += spacer;
            }
            if (selectTarget.ErrorDescription != null) targetString += selectTarget.ErrorDescription + spacer;
            else
            {
                targetString += spacer;
            }

            return targetString;
        }

    }
}
