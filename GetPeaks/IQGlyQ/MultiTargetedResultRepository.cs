using System;
using System.Collections.Generic;
using DeconTools.Backend.Core;
using DeconTools.Workflows.Backend.Results;

namespace IQGlyQ
{
    public class MultiTargetedResultRepository
    {

        #region Constructors
        public MultiTargetedResultRepository()
        {
            this.Results = new List<TargetedResultDTO>();
        }
        #endregion

        #region Properties
        public List<TargetedResultDTO> Results { get; set; }


        #endregion

        public void AddResult(TargetedResultBase resultToConvert)
        {
            //if (resultToConvert.ChromPeakQualityList.Count == 1)
            //{
            //    TargetedResultDTO result = ResultDTOFactory.CreateTargetedResult(resultToConvert);
            //    this.Results.Add(result);
            //}
            //else
            //{

            if (resultToConvert.FailedResult == false)
            {
                List<ChromPeakQualityData> tempList = new List<ChromPeakQualityData>();
                foreach (ChromPeakQualityData chromPeakQualityData in resultToConvert.ChromPeakQualityList)
                {
                    ChromPeakQualityData newPoint = new ChromPeakQualityData(chromPeakQualityData.Peak);
                    newPoint.ScanLc = chromPeakQualityData.ScanLc;
                    newPoint.IsotopicProfile = chromPeakQualityData.IsotopicProfile;
                    newPoint.FitScore = chromPeakQualityData.FitScore;
                    newPoint.Abundance = chromPeakQualityData.Abundance;

                   
                    newPoint = chromPeakQualityData;
                    tempList.Add(newPoint);
                }

                foreach (ChromPeakQualityData chromPeakQualityData in tempList)
                {
                    resultToConvert.ChromPeakQualityList = new List<ChromPeakQualityData>();
                    resultToConvert.ChromPeakQualityList.Add(chromPeakQualityData);
                    resultToConvert.Target.ChargeState = Convert.ToInt16(chromPeakQualityData.IsotopicProfile.ChargeState);
                    resultToConvert.Target.IsotopicProfile = chromPeakQualityData.IsotopicProfile;

                    TargetedResultDTO result = ResultDTOFactory.CreateTargetedResult(resultToConvert);
               

                    result.ScanLC = chromPeakQualityData.ScanLc;
                    result.ScanLCStart = Convert.ToInt16(chromPeakQualityData.ScanLc - Math.Round(chromPeakQualityData.Peak.Width, 0));
                    result.ScanLCEnd = Convert.ToInt16(chromPeakQualityData.ScanLc + Math.Round(chromPeakQualityData.Peak.Width, 0));
                    if (resultToConvert.ChromValues.Xvalues.Length > 0)
                    {
                        result.MonoMassCalibrated = resultToConvert.ChromValues.Xvalues[0];//this returns the future target
                    }
                    this.Results.Add(result);
                }
            }
            else
            {
                TargetedResultDTO result = ResultDTOFactory.CreateTargetedResult(resultToConvert);
                this.Results.Add(result);
            }

            //}
        }



        public void AddResults(List<TargetedResultDTO> featuresToAlign)
        {
            this.Results.AddRange(featuresToAlign);
        }

        public void AddResults(List<TargetedResultBase> resultsToConvert)
        {
            foreach (var item in resultsToConvert)
            {
                TargetedResultDTO result = ResultDTOFactory.CreateTargetedResult(item);
                this.Results.Add(result);
            }
        }


        public void Clear()
        {
            this.Results.Clear();
        }



        public bool HasResults
        {
            get
            { return (this.Results != null && this.Results.Count > 0); }
        }
    }
}
