﻿using System.Collections.Generic;
using IQ.Backend.Core;
using Run32.Backend.Core;

namespace IQ.Backend.ProcessingTasks.ResultValidators
{
    public class ResultValidatorTask : TaskIQ
    {
        #region Constructors

        public ResultValidatorTask(double minRelIntensityForScore = 0.025, bool usePeakBasedInterferenceValue = true)
        {
            //create a default collection
            ResultValidatorColl = new List<ResultValidator>();
            ResultValidatorColl.Add(new LeftOfMonoPeakLooker());
            ResultValidatorColl.Add(new IsotopicProfileInterferenceScorer(minRelIntensityForScore,usePeakBasedInterferenceValue));


        }
        #endregion

        #region Properties

        IList<ResultValidator> ResultValidatorColl { get; set; }

        


        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        #endregion
        public override void Execute(ResultCollection resultList)
        {
            if (resultList.IsosResultBin == null || resultList.IsosResultBin.Count == 0) return;

            //iterate over each ms feature
            foreach (var msFeature in resultList.IsosResultBin)
            {

                //execute each validator
                foreach (var validator in ResultValidatorColl)
                {
                    validator.CurrentResult = msFeature;
                    validator.Execute(resultList);

                }



            }


        }
    }
}
