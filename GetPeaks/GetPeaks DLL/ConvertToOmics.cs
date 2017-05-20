using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetPeaks_DLL.Objects;
using DeconTools.Backend.Core;
using PNNLOmics.Data;
using GetPeaks_DLL.Objects.ResultsObjects;

namespace GetPeaks_DLL
{
    public class ConvertToOmics
    {
        public List<ElutingPeakOmics> ConvertElutingPeakToElutingPeakOmics(List<ElutingPeak> elutingPeakList)
        {
            List<ElutingPeakOmics> omicsList = new List<ElutingPeakOmics>();

            for (int i = 0; i < elutingPeakList.Count;i++)
            {
                ElutingPeak ePeak = elutingPeakList[i];
                ElutingPeakOmics newOmicsPeak = new ElutingPeakOmics();
                newOmicsPeak.AggregateIntensity = ePeak.AggregateIntensity;
                newOmicsPeak.ChargeState = ePeak.ChargeState;
                newOmicsPeak.FitScore = ePeak.FitScore;
                newOmicsPeak.ID = ePeak.ID;
                newOmicsPeak.Intensity = ePeak.Intensity;
                newOmicsPeak.Mass = ePeak.Mass;
                newOmicsPeak.NumberOfPeaks = ePeak.NumberOfPeaks;
                newOmicsPeak.NumberOfPeaksFlag = ePeak.NumberOfPeaksFlag;
                newOmicsPeak.NumberOfPeaksMode = ePeak.NumberOfPeaksMode;
                newOmicsPeak.RetentionTime = ePeak.RetentionTime;
                newOmicsPeak.ScanEnd = ePeak.ScanEnd;
                newOmicsPeak.ScanMaxIntensity = ePeak.ScanMaxIntensity;
                newOmicsPeak.ScanStart = ePeak.ScanStart;
                newOmicsPeak.SummedIntensity = ePeak.SummedIntensity;

                foreach (DeconTools.Backend.DTO.MSPeakResult deconResult in ePeak.PeakList)
                {
                    MSPeakResultOmics newOmcisMSPeakResult = new MSPeakResultOmics();
                    newOmcisMSPeakResult.ChromID = deconResult.ChromID;
                    //newOmcisMSPeakResult.Frame_num = deconResult.Frame_num;
                    newOmcisMSPeakResult.PeakID = deconResult.PeakID;
                    newOmcisMSPeakResult.Scan_num = deconResult.Scan_num;

                    PNNLOmics.Data.Peak newOmicsMSPeak = new PNNLOmics.Data.Peak();
                    newOmicsMSPeak.Height = deconResult.MSPeak.Height;
                    //newOmicsMSPeak.LocalSignalToNoise = deconResult.MSPeak.SignalToNoise;
                    newOmicsMSPeak.Width = deconResult.MSPeak.Width;
                    //TODO get doubles in to OMICS!!!
                    float downgradedMass = Convert.ToUInt64(deconResult.MSPeak.XValue*100000);
                    newOmicsMSPeak.XValue = downgradedMass/100000;

                    newOmcisMSPeakResult.MSPeak = newOmicsMSPeak;
                    
                    deconResult.MSPeak = null;

                    newOmicsPeak.PeakList.Add(newOmcisMSPeakResult);
                }

                elutingPeakList[i].Dispose();
                elutingPeakList[i] = null;

                omicsList.Add(newOmicsPeak);
            }
            return omicsList;
        }

        //public SimpleWorkflowParameters ConvertSimpleWorkflowParameters(SimpleWorkflowParameters oldWorkFlowParameters)
        //{
        //    SimpleWorkflowParameters newParameters = new SimpleWorkflowParameters();

        //    return newParameters;
        //}
    }
}
