﻿using System.Collections.Generic;
using IQ_X64.Workflows.FileIO;
using IQ_X64.Workflows.FileIO.DTO;

namespace IQ.Workflows.FileIO
{
    public class UnlabelledTargetedResultFromTextImporter : TargetedResultFromTextImporter
    {
        
        #region Constructors
        public UnlabelledTargetedResultFromTextImporter(string filename) : base(filename) { }

        #endregion

        protected override TargetedResultDTO ConvertTextToDataObject(List<string> processedData)
        {
            UnlabelledTargetedResultDTO result = new UnlabelledTargetedResultDTO();

            result.DatasetName = LookupData(processedData, datasetHeaders);

            if (result.DatasetName == "-1")
            {
                result.DatasetName = TryGetDatasetNameFromFileName();
            }


            result.ChargeState = ParseIntField(LookupData(processedData, chargeStateHeaders));
            result.FitScore = ParseFloatField(LookupData(processedData, fitScoreHeaders));
            result.Intensity = ParseFloatField(LookupData(processedData, intensityRepHeaders));
            result.IntensityI0 = ParseFloatField(LookupData(processedData, intensityI0Headers));

            result.IScore = ParseFloatField(LookupData(processedData, iscoreHeaders));
            result.TargetID = ParseLongField(LookupData(processedData, targetIDHeaders));
            result.MonoMass = ParseDoubleField(LookupData(processedData, monomassHeaders));
            result.MonoMassCalibrated = ParseDoubleField(LookupData(processedData, monomassCalibratedHeaders));
            result.MassErrorBeforeCalibration = ParseDoubleField(LookupData(processedData, massErrorBeforeCalibrationHeaders));
            result.MassErrorAfterCalibration = ParseDoubleField(LookupData(processedData, massErrorAfterCalibrationHeaders));
            result.MonoMZ = ParseDoubleField(LookupData(processedData, mzHeaders));
            result.NET = ParseFloatField(LookupData(processedData, netHeaders));
            result.NETError = ParseFloatField(LookupData(processedData, netErrorHeaders));
            result.NumChromPeaksWithinTol = ParseIntField(LookupData(processedData, numchromPeaksWithinTolHeaders));
            result.NumQualityChromPeaksWithinTol = ParseIntField(LookupData(processedData, numQualitychromPeaksWithinTolHeaders));
            result.ScanLC = ParseIntField(LookupData(processedData, scanHeaders));
            result.ScanLCEnd = ParseIntField(LookupData(processedData, scanEndHeaders));
            result.ScanLCStart = ParseIntField(LookupData(processedData, scanStartHeaders));

            result.MatchedMassTagID = ParseIntField(LookupData(processedData, matchedMassTagIDHeaders,"-1"));


            return result;



        }


    }
}
